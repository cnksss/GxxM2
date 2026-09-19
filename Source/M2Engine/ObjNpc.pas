unit ObjNpc;

interface

uses
  Windows, Classes, SysUtils, StrUtils, ObjBase, Grobal2, SDK, IniFiles,
  ObjPlayer, M2Threads, NpcCommon, M2Definition;

const
  CMD_RACE_0 = 0; // self
  CMD_RACE_1 = 1; // hero
  CMD_RACE_2 = 2; // Master
  CMD_RACE_3 = 3; // 被攻击的目标
  CMD_RACE_4 = 4; // obj
  CMD_RACE_5 = 5; // 变量名
  CMD_RACE_6 = 6; // 最后攻击者
  CMD_RACE_7 = 7;
  CMD_RACE_8 = 8;
  CMD_RACE_9 = 9;
  CMD_RACE_10 = 10;
  CMD_RACE_11 = 11;
  CMD_RACE_12 = 12;

type
  TNormNpc = class;

  TUpgradeInfo = record // 0x40
    sUserName: string[30]; // 0x00
    UserItem: TUserItem; // 0x10
    btDc: Byte; // 0x28
    btSc: Byte; // 0x29
    btMc: Byte; // 0x2A
    btDura: Byte; // 0x2B
    n2C: Integer;
    dtTime: TDateTime; // 0x30
    dwGetBackTick: LongWord; // 0x38
    n3C: Integer;
  end;

  pTUpgradeInfo = ^TUpgradeInfo;

  TItemPrice = record
    wIndex: Word;
    nPrice: Integer;
  end;

  pTItemPrice = ^TItemPrice;

  TGoods = record // 0x1C
    sItemName: string[30];
    nCount: Integer;
    dwRefillTime: LongWord;
    dwRefillTick: LongWord;
  end;

  pTGoods = ^TGoods;

  TSellItemPrice = record
    wIndex: Word;
    nPrice: Integer;
  end;

  pTSellItemPrice = ^TSellItemPrice;

  TQuestActionInfo = record // 0x1C
    ScriptList: array of string;
    ScriptCmd: array of Integer;
    nCMDCode: Integer; // 0x00
    sCmd: string;
    sCmdLine: string;
    // sParams: string;
    sParam1: string; // 0x04
    nParam1: Integer; // 0x08
    sParam2: string; // 0x0C
    nParam2: Integer; // 0x10
    sParam3: string; // 0x14
    nParam3: Integer; // 0x18
    sParam4: string;
    nParam4: Integer;
    sParam5: string;
    nParam5: Integer;
    sParam6: string;
    nParam6: Integer;
    sParam7: string;
    nParam7: Integer;
    sParam8: string;
    nParam8: Integer;
    sParam9: string;
    nParam9: Integer;
    sParam10: string;
    nParam10: Integer;
    sRawParam1: string;
    sRawParam2: string;
    sRawParam3: string;
    sRawParam4: string;
    sRawParam5: string;
    sRawParam6: string;
    sRawParam7: string;
    sRawParam8: string;
    sRawParam9: string;
    sRawParam10: string;
    sSubParam1: string;
    sSubParam2: string;
    sSubParam3: string;
    sSubParam4: string;
    sSubParam5: string;
    sSubParam6: string;
    sSubParam7: string;
    sSubParam8: string;
    sSubParam9: string;
    sSubParam10: string;
    VarInfo1: TVarInfo;
    VarInfo2: TVarInfo;
    VarInfo3: TVarInfo;
    VarInfo4: TVarInfo;
    VarInfo5: TVarInfo;
    VarInfo6: TVarInfo;
    VarInfo7: TVarInfo;
    VarInfo8: TVarInfo;
    VarInfo9: TVarInfo;
    VarInfo10: TVarInfo;
    boCompleteFormat1: Boolean;
    boCompleteFormat2: Boolean;
    boCompleteFormat3: Boolean;
    boCompleteFormat4: Boolean;
    boCompleteFormat5: Boolean;
    boCompleteFormat6: Boolean;
    boCompleteFormat7: Boolean;
    boCompleteFormat8: Boolean;
    boCompleteFormat9: Boolean;
    boCompleteFormat10: Boolean;
  end;

  pTQuestActionInfo = ^TQuestActionInfo;

  TQuestConditionInfo = record // 0x14
    ScriptList: array of string;
    ScriptCmd: array of Integer;
    boNot: Boolean; // 是否取反
    nCMDCode: Integer; // 0x00
    sCmd: string;
    sCmdLine: string;
    sParam: string;
    sParam1: string; // 0x04
    nParam1: Integer; // 0x08
    sParam2: string; // 0x0C
    nParam2: Integer; // 0x10
    sParam3: string;
    nParam3: Integer;
    sParam4: string;
    nParam4: Integer;
    sParam5: string;
    nParam5: Integer;
    sParam6: string;
    nParam6: Integer;
    sParam7: string;
    nParam7: Integer;
    sParam8: string;
    nParam8: Integer;
    sParam9: string;
    nParam9: Integer;
    sParam10: string;
    nParam10: Integer;
    sRawParam1: string;
    sRawParam2: string;
    sRawParam3: string;
    sRawParam4: string;
    sRawParam5: string;
    sRawParam6: string;
    sRawParam7: string;
    sRawParam8: string;
    sRawParam9: string;
    sRawParam10: string;
    sSubParam1: string;
    sSubParam2: string;
    sSubParam3: string;
    sSubParam4: string;
    sSubParam5: string;
    sSubParam6: string;
    sSubParam7: string;
    sSubParam8: string;
    sSubParam9: string;
    sSubParam10: string;
    VarInfo1: TVarInfo;
    VarInfo2: TVarInfo;
    VarInfo3: TVarInfo;
    VarInfo4: TVarInfo;
    VarInfo5: TVarInfo;
    VarInfo6: TVarInfo;
    VarInfo7: TVarInfo;
    VarInfo8: TVarInfo;
    VarInfo9: TVarInfo;
    VarInfo10: TVarInfo;
    boCompleteFormat1: Boolean;
    boCompleteFormat2: Boolean;
    boCompleteFormat3: Boolean;
    boCompleteFormat4: Boolean;
    boCompleteFormat5: Boolean;
    boCompleteFormat6: Boolean;
    boCompleteFormat7: Boolean;
    boCompleteFormat8: Boolean;
    boCompleteFormat9: Boolean;
    boCompleteFormat10: Boolean;
  end;

  pTQuestConditionInfo = ^TQuestConditionInfo;

  TScriptParamter = record
    Npc: TNormNpc;
    BaseObj: TBaseObject;
    PlayObj: TPlayObject;
    QuestActionInfo: TQuestActionInfo;
  end;

  PScriptParamter = ^TScriptParamter;

  TConditionType = (ct_and, ct_or);

  TConditionList = class(TList)
  private
    FTrueCount: Byte;
    FConditionType: TConditionType;
  public
    constructor Create;
    property ConditionType: TConditionType read FConditionType write FConditionType;
    property TrueCount: Byte read FTrueCount write FTrueCount;
  end;

  TSayingProcedure = record // 0x14
    ConditionList: TConditionList; // 0x00
    ActionList: TList; // 0x04
    sSayMsg: string; // 0x08
    ElseActionList: TList; // 0x0C
    sElseSayMsg: string; // 0x10
  end;

  pTSayingProcedure = ^TSayingProcedure;

  TSayingRecord = record // 0x08
    sLabel: string;
    ProcedureList: TList; // 0x04
    boExtJmp: Boolean; // 是否允许外部跳转
  end;

  pTSayingRecord = ^TSayingRecord;

  TTimeLabel = record
    nType: Integer;
    nIndex: Integer;
    sLabel: string;
    dwTick: LongWord;
    dwTime: LongWord;
    boChangeMapDelete: Boolean;
    boDelete: Boolean;
    Npc: TNormNpc;
    Envir: TObject;
    nCount: Integer;
  end;

  pTTimeLabel = ^TTimeLabel;

  TNormNpc = class(TAnimalObject) // 0x564
    m_sScript: string; // 0x568
    m_nFlag: ShortInt; // 0x550 //用于标识此NPC是否有效，用于重新加载NPC列表(-1 为无效)
    m_ScriptList: TList; // 0x554
    m_sFilePath: string; // 0x558 脚本文件所在目录
    m_boIsHide: Boolean; // 0x55C 此NPC是否是隐藏的，不显示在地图中
    m_boIsQuest: Boolean; // 0x55D NPC类型为地图任务型的，加载脚本时的脚本文件名为 角色名-地图号.txt
    m_sPath: string; // 0x560
    m_boNpcAutoChangeColor: Boolean;
    m_dwNpcAutoChangeColorTick: LongWord;
    m_dwNpcAutoChangeColorTime: LongWord;
    m_nNpcAutoChangeIdx: Integer;
    m_nGlobalAValIndex: Integer;
    m_sDynamicName: string;
    m_NoUserSelectList: TStringList;
    m_boMapEvent: Boolean;
    m_boPlug: Boolean;
    m_sLastLabel: string;
    // 附加数据 chonchong 2015-01-15
    m_sDataFileName: string;
    m_sAddName: string;
    m_nAddLevel: Integer;
    m_boGrayShow: Boolean;
    m_boScaleShow: Boolean;
    m_nEffigyState: TFeature_New;
    m_nEffigyOffset: Integer;
    m_nValidTime: Integer;
    m_wValidTimeTick: LongWord;
    m_dwScriptCRC: LongWord;
    m_dwIconFileCRC: LongWord;
    m_CallFileListCRC: TStringList;
  private
    procedure LoadAddData();
    procedure QuickSortRecordList(List: TList; L, R: Integer);
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Initialize(); override;
    procedure DeleteSelectLable(sLabel: string);
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override;
    procedure Run; override;
    function GetShowName(boSuperUser: Boolean = False): string; override;
    procedure Click(PlayObject: TPlayObject); virtual;
    procedure UserSelect(PlayObject: TPlayObject; sData: string); virtual;
    function GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string; var IsBreakParseVar: Boolean; nPos: Integer = 0): Boolean; virtual; // FFE9
    function GetLineVariableText(PlayObject: TPlayObject; sMsg: string; var IsBreakParseVar: Boolean): string;
    function GotoLable(Player: TPlayObject; sLabel: string; boExtJmp: Boolean; UseParams: Boolean = False): Boolean;
    procedure LoadNpcScript();
    procedure LoadNpcIconFile();
    procedure ClearScript(); virtual;
    procedure SendMsgToUser(PlayObject: TPlayObject; sMsg: string; boShowNPCName: Boolean = True);
    procedure SendCustemMsg(PlayObject: TPlayObject; sMsg: string); virtual;
    procedure MessageBox(PlayObject: TPlayObject; sMsg: string);
    procedure ScriptActionError(BaseObject: TBaseObject; sErrMsg: string; QuestActionInfo: pTQuestActionInfo);
    procedure ScriptConditionError(BaseObject: TBaseObject; QuestConditionInfo: pTQuestConditionInfo);
    function AllowSelect(sLabel: string): Boolean; // 防止封包模拟用户点击NPC字段
    procedure AddSelectLable(sLabel: string);
    function GetDynamicVarList(PlayObject: TPlayObject; sType: string; var sName: string): TList;
    function GetValNameValue(PlayObject: TPlayObject; sVar: string; var sValue: string; var nValue: Integer): Boolean;
    function SetValNameValue(PlayObject: TPlayObject; sVar: string; sValue: string; nValue: Integer): Boolean;
    function GetDynamicValue(PlayObject: TPlayObject; sVar: string; var sValue: string; var nValue: Integer): Boolean;
    function SetDynamicValue(PlayObject: TPlayObject; sVar: string; sValue: string; nValue: Integer): Boolean;
    procedure GetVarValue(PlayObject: TPlayObject; sData: string; var sVar, sValue: string; var nValue: Integer); overload;
    procedure GetVarValue(PlayObject: TPlayObject; sData: string; var nValue: Integer); overload;
    procedure GetVarValue(PlayObject: TPlayObject; sData: string; var sValue: string); overload;
    procedure GetVarValue(PlayObject: TPlayObject; sData: string; var sValue: string; var nValue: Integer; var IsBreakParseVar: Boolean); overload;
    function SetVarValue(PlayObject: TPlayObject; const sData, sValue: string; const nValue: Integer): Boolean;
    procedure SaveAddData();
    procedure DoSort;
    function GetSayingRecordFromRecordList(List: TList; sLabel: string): pTSayingRecord;
  end;

  TMerchant = class(TNormNpc) // 0x594
    n56C: Integer;
    m_nPriceRate: Integer; // 0x570   物品价格倍率 默认为 100%
    bo574: Boolean;
    m_boCastle: Boolean; // 0x575
    dwRefillGoodsTick: LongWord; // 0x578
    dwClearExpreUpgradeTick: LongWord; // 0x57C
    m_ItemTypeList: TList; // 0x580  NPC买卖物品类型列表，脚本中前面的 +1 +30 之类的
    m_RefillGoodsList: TList; // 0x584
    m_GoodsList: TList; // 0x588
    m_ItemPriceList: TList; // 0x58C
    m_UpgradeWeaponList: TList;
    m_boCanMove: Boolean;
    m_dwMoveTime: LongWord;
    m_dwMoveTick: LongWord;
    m_boBuy: Boolean;
    m_boSell: Boolean;
    m_boMakeDrug: Boolean;
    m_boPrices: Boolean;
    m_boStorage: Boolean;
    m_boGetback: Boolean;
    m_boBigStorage: Boolean;
    m_boBigGetBack: Boolean;
    m_boGetNextPage: Boolean;
    m_boGetPreviousPage: Boolean;
    m_boUpgradenow: Boolean;
    m_boGetBackupgnow: Boolean;
    m_boRepair: Boolean;
    m_boS_repair: Boolean;
    m_boSendmsg: Boolean;
    m_boGetMarry: Boolean;
    m_boGetMaster: Boolean;
    m_boUseItemName: Boolean;
    m_boArmRemoveStone: Boolean;
    m_boGetSellGold: Boolean;
    m_boSellOff: Boolean;
    m_boBuyOff: Boolean;
    m_boofflinemsg: Boolean;
    m_boDealGold: Boolean;
    m_boCreateHeroName: Boolean;
    m_boUpgradeNew: Boolean;
    m_boPleaseDrink: Boolean; // 请酒
    m_boMakeWine: Boolean; // 酿酒NPC
    m_boBuHero: Boolean; // 卧龙英雄
    m_boReclaimItem: Boolean;
    m_boFB: Boolean; // 副本地图 -- 是否为副本NPC chongchong 2013-09-11
    m_sFBName: string; // 副本地图 -- 副本地图名 chongchong 2013-09-11
  private
    nIdx: Integer;
    nKeepCount: Integer;
    nKeepMaxCount: Integer;
  public
    MovePoint: array of TPoint;
  private
    FLastTrunTick: DWORD;
    FTrunTimeInterval: DWORD; // 转弯时间间隔
    procedure ClearExpreUpgradeListData();
    function GetRefillList(nIndex: Integer): TList;
    procedure AddItemPrice(nIndex, nPrice: Integer);
    function GetSellItemPrice(nPrice: Integer): Integer;
    function AddItemToGoodsList(UserItem: pTUserItem): Boolean;
    procedure GetBackupgWeapon(User: TPlayObject);
    procedure UpgradeWapon(User: TPlayObject);
    procedure ChangeUseItemName(PlayObject: TPlayObject; sLabel, sItemName: string);
    procedure SaveUpgradingList;
    // procedure GetMarry(PlayObject: TPlayObject; sDearName: string);
    // procedure GetMaster(PlayObject: TPlayObject; sMasterName: string);
  public
    constructor Create(); override;
    destructor Destroy; override;
    function GetItemPrice(nIndex: Integer): Integer;
    function GetUserPrice(PlayObject: TPlayObject; nPrice: Integer): Integer;
    function CheckItemType(nStdMode: Integer): Boolean;
    procedure CheckItemPrice(nIndex: Integer);
    function GetUserItemPrice(UserItem: pTUserItem; IsSellToNpc: Boolean): Integer;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override;
    procedure Run; override;
    procedure UserSelect(PlayObject: TPlayObject; sData: string); override;
    procedure LoadNPCData();
    procedure SaveNPCData();
    procedure LoadUpgradeList();
    procedure RefillGoods();
    procedure LoadNpcScript(IsAddMapName: Boolean = True);
    procedure LoadNpcIconFile(IsAddMapName: Boolean = True);
    procedure Click(PlayObject: TPlayObject); override;
    procedure ClearScript(); override;
    procedure ClearData();
    function GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string; var IsBreakParseVar: Boolean; nPos: Integer = 0): Boolean; override; // FFE9
    procedure ClientBuyItem(PlayObject: TPlayObject; sItemName: string; nCount, nInt: Integer; IsFromTradingDlg: Boolean);
    procedure ClientGetDetailGoodsList(PlayObject: TPlayObject; sItemName: string; nInt: Integer; IsFromTradingDlg: Boolean);
    procedure ClientQuerySellPrice(PlayObject: TPlayObject; UserItem: pTUserItem; IsFromTradingDlg: Boolean; WaitSetIndex: Integer);
    function ClientSellItem(PlayObject: TPlayObject; UserItem: pTUserItem; IsFromTradingDlg: Boolean): Boolean;
    procedure ClientMakeDrugItem(PlayObject: TPlayObject; sItemName: string);
    procedure ClientQueryRepairCost(PlayObject: TPlayObject; UserItem: pTUserItem);
    function ClientRepairItem(PlayObject: TPlayObject; UserItem: pTUserItem): Boolean;
    procedure SendCustemMsg(PlayObject: TPlayObject; sMsg: string); override;
  end;

  TGuildOfficial = class(TNormNpc) // 0x568
  private
    function ReQuestBuildGuild(PlayObject: TPlayObject; sGuildName: string): Integer;
    function ReQuestGuildWar(PlayObject: TPlayObject; sGuildName: string): Integer;
    procedure DoNate(PlayObject: TPlayObject);
    procedure ReQuestCastleWar(PlayObject: TPlayObject; sIndex: string);
  public
    constructor Create(); override;
    destructor Destroy; override;
    function GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string; var IsBreakParseVar: Boolean; nPos: Integer = 0): Boolean; override;
    procedure Run; override; // FFFB
    procedure Click(PlayObject: TPlayObject); override; // FFEB
    procedure UserSelect(PlayObject: TPlayObject; sData: string); override; // FFEA
    procedure SendCustemMsg(PlayObject: TPlayObject; sMsg: string); override;
  end;

  // ----- 溢出 2013-07-04---
  TTrainer = class(TNormNpc) // 0x574
    n564: LongWord; // Integer;
    m_dw568: LongWord;
    n56C: Int64; // Integer;
    n570: LongWord; // Integer;
  public
    constructor Create(); override;
    destructor Destroy; override;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override; // FFFC
    procedure Run; override;
  end;

  TBoxMonster = class(TAnimalObject) // 0x574
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Initialize(); override;
    function Operate(ProcessMsg: pTProcessMessage): Boolean; override; // FFFC
    procedure Run; override;
  end;

  // TCastleManager = class(TMerchant)
  TCastleOfficial = class(TMerchant)
  private
    procedure HireArcher(sIndex: string; PlayObject: TPlayObject);
    procedure HireGuard(sIndex: string; PlayObject: TPlayObject);
    procedure RepairDoor(PlayObject: TPlayObject);
    procedure RepairWallNow(nWallIndex: Integer; PlayObject: TPlayObject);
  public
    constructor Create(); override;
    destructor Destroy; override;
    procedure Click(PlayObject: TPlayObject); override; // FFEB
    procedure UserSelect(PlayObject: TPlayObject; sData: string); override; // FFEA
    function GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string; var IsBreakParseVar: Boolean; nPos: Integer = 0): Boolean; override;
    procedure SendCustemMsg(PlayObject: TPlayObject; sMsg: string); override;
  end;

function LoadLevelScriptAction(QuestActionInfo: pTQuestActionInfo; sCmd: string): string;

function LoadLevelScriptCondition(QuestConditionInfo: pTQuestConditionInfo; sCmd: string): string;

function GetLevelBaseObjectCondition(Npc: TNormNpc; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): TBaseObject;

function GetLevelBaseObjectAction(Npc: TNormNpc; PlayObject: TPlayObject; QuestActionInfo: pTQuestActionInfo): TBaseObject;

function CheckStrIsVar(sText: string): Boolean;

implementation

uses
  Math, Castle, M2Share, HUtil32, LocalDB, Envir, Guild, EDcode, ObjMon2,
  ObjHero, GameGoldDealDB, HandleNpcCmds, uCombatPowerUtils, M2DataCommon,
  ObjSmartMon;

{ TConditionList }
constructor TConditionList.Create;
begin
  FConditionType := ct_and;
end;

function LoadLevelScriptAction(QuestActionInfo: pTQuestActionInfo; sCmd: string): string;
var
  TempList: TStringList;
  I: Integer;
  S: string;
begin
  SetLength(QuestActionInfo.ScriptCmd, 0);
  SetLength(QuestActionInfo.ScriptList, 0);
  Result := sCmd;
  if (Pos('.', sCmd) > 0) and (sCmd[Length(sCmd)] <> '.') then
  begin
    TempList := TStringList.Create;
    ExtractStrings(['.'], [], PChar(sCmd), TempList);
    Result := UpperCase(Trim(TempList.Strings[TempList.Count - 1]));
    TempList.Delete(TempList.Count - 1);
    TempList.Strings[0] := UpperCase(Trim(TempList.Strings[0]));
    if TempList.Strings[0] <> 'SELF' then
      TempList.Insert(0, 'SELF');
    SetLength(QuestActionInfo.ScriptCmd, TempList.Count);
    SetLength(QuestActionInfo.ScriptList, TempList.Count);
    for I := 0 to TempList.Count - 1 do
    begin
      S := UpperCase(Trim(TempList.Strings[I]));
      if S = 'SELF' then
      begin
        QuestActionInfo.ScriptCmd[I] := CMD_RACE_0;
      end
      else if (S = 'H') or (S = 'HERO') then
      begin
        QuestActionInfo.ScriptCmd[I] := CMD_RACE_1;
      end
      else if S = 'O' then
      begin
        QuestActionInfo.ScriptCmd[I] := CMD_RACE_2;
      end
      else if S = 'M' then
      begin
        QuestActionInfo.ScriptCmd[I] := CMD_RACE_3;
      end
      else if S = 'P' then
      begin
        QuestActionInfo.ScriptCmd[I] := CMD_RACE_4;
      end
      else if S = 'L' then
      begin
        QuestActionInfo.ScriptCmd[I] := CMD_RACE_6;
      end
      else if S = 'HM' then
      begin
        if g_nKey_HeroExt = 1 then
        begin
          QuestActionInfo.ScriptCmd[I] := CMD_RACE_7;
        end;
      end
      else if S = 'HL' then
      begin
        if g_nKey_HeroExt = 1 then
        begin
          QuestActionInfo.ScriptCmd[I] := CMD_RACE_8;
        end;
      end
      else if S = 'PET' then
      begin
        QuestActionInfo.ScriptCmd[I] := CMD_RACE_9;
      end
      else if S = 'BB' then
      begin
        QuestActionInfo.ScriptCmd[I] := CMD_RACE_10;
      end
      else if S = 'BBR' then
      begin
        QuestActionInfo.ScriptCmd[I] := CMD_RACE_12;
      end
      else if S = 'FS' then
      begin
        QuestActionInfo.ScriptCmd[I] := CMD_RACE_11;
      end
      else
      begin
        QuestActionInfo.ScriptCmd[I] := CMD_RACE_5;
      end;
      QuestActionInfo.ScriptList[I] := S;
    end;
    TempList.Free;
  end;
end;

function LoadLevelScriptCondition(QuestConditionInfo: pTQuestConditionInfo; sCmd: string): string;
var
  TempList: TStringList;
  I: Integer;
  S: string;
begin
  SetLength(QuestConditionInfo.ScriptCmd, 0);
  SetLength(QuestConditionInfo.ScriptList, 0);
  Result := sCmd;
  if Pos('.', sCmd) > 0 then
  begin
    TempList := TStringList.Create;
    ExtractStrings(['.'], [], PChar(sCmd), TempList);
    Result := UpperCase(Trim(TempList.Strings[TempList.Count - 1]));
    TempList.Delete(TempList.Count - 1);
    TempList.Strings[0] := UpperCase(Trim(TempList.Strings[0]));
    if TempList.Strings[0] <> 'SELF' then
      TempList.Insert(0, 'SELF');
    SetLength(QuestConditionInfo.ScriptCmd, TempList.Count);
    SetLength(QuestConditionInfo.ScriptList, TempList.Count);
    for I := 0 to TempList.Count - 1 do
    begin
      S := UpperCase(Trim(TempList.Strings[I]));
      if S = 'SELF' then
      begin
        QuestConditionInfo.ScriptCmd[I] := CMD_RACE_0;
      end
      else if (S = 'H') or (S = 'HERO') then
      begin
        QuestConditionInfo.ScriptCmd[I] := CMD_RACE_1;
      end
      else if S = 'O' then
      begin
        QuestConditionInfo.ScriptCmd[I] := CMD_RACE_2;
      end
      else if S = 'M' then
      begin
        QuestConditionInfo.ScriptCmd[I] := CMD_RACE_3;
      end
      else if S = 'P' then
      begin
        QuestConditionInfo.ScriptCmd[I] := CMD_RACE_4;
      end
      else if S = 'L' then
      begin
        QuestConditionInfo.ScriptCmd[I] := CMD_RACE_6;
      end
      else if S = 'HM' then
      begin
        if g_nKey_HeroExt = 1 then
        begin
          QuestConditionInfo.ScriptCmd[I] := CMD_RACE_7;
        end;
      end
      else if S = 'HL' then
      begin
        if g_nKey_HeroExt = 1 then
        begin
          QuestConditionInfo.ScriptCmd[I] := CMD_RACE_8;
        end;
      end
      else if S = 'PET' then
      begin
        QuestConditionInfo.ScriptCmd[I] := CMD_RACE_9;
      end
      // 检测指令不支持宝宝
      {
        else if S = 'BB' then
        begin
        QuestConditionInfo.ScriptCmd[I] := CMD_RACE_10;
        end
        else if S = 'FS' then
        begin
        QuestConditionInfo.ScriptCmd[I] := CMD_RACE_11;
        end
      }
      else
      begin
        QuestConditionInfo.ScriptCmd[I] := CMD_RACE_5;
      end;
      QuestConditionInfo.ScriptList[I] := S;
    end;
    TempList.Free;
  end;
end;

function GetLevelBaseObjectCondition(Npc: TNormNpc; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): TBaseObject;
var
  I: Integer;
  sCharName: string;
  BaseObject: TBaseObject;
  sVar, sValue: string;
  nValue: Integer;
begin
  if Length(QuestConditionInfo.ScriptCmd) <= 0 then
  begin
    Result := PlayObject;
  end
  else
  begin
    BaseObject := PlayObject;
    for I := 0 to Length(QuestConditionInfo.ScriptCmd) - 1 do
    begin
      case QuestConditionInfo.ScriptCmd[I] of
        CMD_RACE_0:
          begin // self
            BaseObject := PlayObject;
          end;
        CMD_RACE_1:
          begin // hero
            if (BaseObject <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then
            begin
              BaseObject := TPlayObject(BaseObject).m_MyHero;
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_2:
          begin // Master
            if (BaseObject <> nil) then
            begin
              BaseObject := BaseObject.m_Master;
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_3:
          begin // mon
            if (BaseObject <> nil) then
            begin
              BaseObject := BaseObject.m_CurrTarget;
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_4:
          begin // mon
            if (BaseObject <> nil) then
            begin
              BaseObject := BaseObject.GetPoseCreate;
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_5:
          begin // obj
            if (BaseObject <> nil) then
            begin
              if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then
              begin
                Npc.GetVarValue(TPlayObject(BaseObject), QuestConditionInfo.ScriptList[I], sVar, sValue, nValue);
                sCharName := sValue;
                { end
                  else if (BaseObject.m_btRaceServer = RC_HEROOBJECT) then begin
                  NPC.GetVarValue(TPlayObject(BaseObject.m_Master), ScriptList[I], sValue);
                  sCharName := sValue; }
              end
              else
              begin
                sCharName := QuestConditionInfo.ScriptList[I];
              end;
              // MainOutMessage('GetLevelBaseObject QuestConditionInfo:' + sCharName + ' nCMDCode:' + inttostr(QuestConditionInfo.nCMDCode));
              // if not NPC.GetValValue(ActorObject, ScriptList[I], sCharName) then sCharName := ScriptList[I];
              BaseObject := UserEngine.GetPlayObject(sCharName);
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_6:
          begin // mon
            if (BaseObject <> nil) then
            begin
              BaseObject := BaseObject.m_LastHiter;
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_7: // Hero.mon
          begin
            if g_nKey_HeroExt = 1 then
            begin
              if (BaseObject <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_MyHero <> nil) then
              begin
                BaseObject := TPlayObject(BaseObject).m_MyHero.m_CurrTarget;
                if BaseObject = nil then
                  Break;
              end
              else
              begin
                BaseObject := nil;
                Break;
              end;
            end;
          end;
        CMD_RACE_8: // Hero.mon
          begin
            if g_nKey_HeroExt = 1 then
            begin
              if (BaseObject <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_MyHero <> nil) then
              begin
                BaseObject := TPlayObject(BaseObject).m_MyHero.m_LastHiter;
                if BaseObject = nil then
                  Break;
              end
              else
              begin
                BaseObject := nil;
                Break;
              end;
            end;
          end;
        CMD_RACE_9:
          begin
            if (BaseObject <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_MyGamePet <> nil) then
            begin
              BaseObject := TPlayObject(BaseObject).m_MyGamePet;
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
      else
        Break;
      end;
    end;
    Result := BaseObject;
  end;
end;

function GetLevelBaseObjectAction(Npc: TNormNpc; PlayObject: TPlayObject; QuestActionInfo: pTQuestActionInfo): TBaseObject;
var
  I, II, Count: Integer;
  sCharName: string;
  BaseObject, TempObject: TBaseObject;
  Player: TPlayObject;
  sVar, sValue: string;
  nValue: Integer;
begin
  Result := nil;
  // 修复 加载报错 chongchong 2013-12-10
  if QuestActionInfo = nil then
    Exit;
  if Length(QuestActionInfo.ScriptCmd) <= 0 then
  begin
    Result := PlayObject;
  end
  else
  begin
    BaseObject := PlayObject;
    for I := 0 to Length(QuestActionInfo.ScriptCmd) - 1 do
    begin
      case QuestActionInfo.ScriptCmd[I] of
        CMD_RACE_0:
          begin // self
            BaseObject := PlayObject;
          end;
        CMD_RACE_1:
          begin // hero
            if (BaseObject <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then
            begin
              BaseObject := TPlayObject(BaseObject).m_MyHero;
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_2:
          begin // Master
            if (BaseObject <> nil) then
            begin
              BaseObject := BaseObject.m_Master;
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_3:
          begin // mon
            if (BaseObject <> nil) then
            begin
              BaseObject := BaseObject.m_CurrTarget;
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_4:
          begin
            if (BaseObject <> nil) then
            begin
              BaseObject := BaseObject.GetPoseCreate;
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_5:
          begin // obj
            if (BaseObject <> nil) then
            begin
              if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then
              begin
                Npc.GetVarValue(TPlayObject(BaseObject), QuestActionInfo.ScriptList[I], sVar, sValue, nValue);
                sCharName := sValue;
                { end
                  else if (BaseObject.m_btRaceServer = RC_HEROOBJECT) then begin
                  NPC.GetVarValue(TPlayObject(BaseObject.m_Master), ScriptList[I], sValue);
                  sCharName := sValue; }
              end
              else
              begin
                sCharName := QuestActionInfo.ScriptList[I];
              end;
              // MainOutMessage('GetLevelBaseObject QuestActionInfo:' + sCharName + ' nCMDCode:' + inttostr(QuestActionInfo.nCMDCode));
              // if not NPC.GetValValue(ActorObject, ScriptList[I], sCharName) then sCharName := ScriptList[I];
              BaseObject := UserEngine.GetPlayObject(sCharName);
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_6:
          begin // mon
            if (BaseObject <> nil) then
            begin
              BaseObject := BaseObject.m_LastHiter;
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_7: // h.mon
          begin
            if g_nKey_HeroExt = 1 then
            begin
              if (BaseObject <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_MyHero <> nil) then
              begin
                BaseObject := TPlayObject(BaseObject).m_MyHero.m_CurrTarget;
                if BaseObject = nil then
                  Break;
              end
              else
              begin
                BaseObject := nil;
                Break;
              end;
            end;
          end;
        CMD_RACE_8: // h.mon
          begin
            if g_nKey_HeroExt = 1 then
            begin
              if (BaseObject <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_MyHero <> nil) then
              begin
                BaseObject := TPlayObject(BaseObject).m_MyHero.m_LastHiter;
                if BaseObject = nil then
                  Break;
              end
              else
              begin
                BaseObject := nil;
                Break;
              end;
            end;
          end;
        CMD_RACE_9:
          begin
            if (BaseObject <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_MyGamePet <> nil) then
            begin
              BaseObject := TPlayObject(BaseObject).m_MyGamePet;
              if BaseObject = nil then
                Break;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_10:
          begin
            if (BaseObject <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_SlaveList <> nil) then
            begin
              Player := TPlayObject(BaseObject);
              BaseObject := nil;
              for II := 0 to Player.m_SlaveList.Count - 1 do
              begin
                TempObject := Player.m_SlaveList.Items[II];
                if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) then
                begin
                  BaseObject := TempObject;
                  Break;
                end;
              end;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_11:
          begin
            if (BaseObject <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_SlaveList <> nil) then
            begin
              Player := TPlayObject(BaseObject);
              BaseObject := nil;
              for II := 0 to Player.m_SlaveList.Count - 1 do
              begin
                TempObject := Player.m_SlaveList.Items[II];
                if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) and (TempObject is TCopyMon) then
                begin
                  BaseObject := TempObject;
                  Break;
                end;
              end;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
        CMD_RACE_12:
          begin
            if (BaseObject <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(BaseObject).m_SlaveList <> nil) then
            begin
              Player := TPlayObject(BaseObject);
              BaseObject := nil;
              Count := 0;
              while (BaseObject = nil) and (Count < 20) do
              begin
                TempObject := Player.m_SlaveList.Items[Random(Player.m_SlaveList.Count)];
                if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) then
                begin
                  BaseObject := TempObject;
                  Break;
                end;
                Inc(Count);
              end;
            end
            else
            begin
              BaseObject := nil;
              Break;
            end;
          end;
      else
        Break;
      end;
    end;
    Result := BaseObject;
  end;
end;

procedure TCastleOfficial.Click(PlayObject: TPlayObject);
begin
  if m_Castle = nil then
  begin
    PlayObject.SysMsg('NPC不属于城堡！', c_Red, t_Hint);
    Exit;
  end;
  if TUserCastle(m_Castle).IsMasterGuild(TGUild(PlayObject.m_MyGuild)) or (PlayObject.m_btPermission >= 3) then
    inherited;
end;

function TCastleOfficial.GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string; var IsBreakParseVar: Boolean; nPos: Integer): Boolean;
var
  sText: string;
  CastleDoor: TCastleDoor;
begin
  Result := inherited GetVariableText(PlayObject, sMsg, sVariable, IsBreakParseVar, nPos);
  if not Result then
  begin
    Result := True;
    sVariable := UpperCase(sVariable);
    if m_Castle = nil then
    begin
      sMsg := '????';
      Exit;
    end;
    if sVariable = '$CASTLEGOLD' then
    begin
      sText := IntToStr(TUserCastle(m_Castle).m_nTotalGold);
      sMsg := sub_49ADB8(nPos, sMsg, '<$CASTLEGOLD>', sText);
    end
    else if sVariable = '$TODAYINCOME' then
    begin
      sText := IntToStr(TUserCastle(m_Castle).m_nTodayIncome);
      sMsg := sub_49ADB8(nPos, sMsg, '<$TODAYINCOME>', sText);
    end
    else if sVariable = '$CASTLEDOORSTATE' then
    begin
      CastleDoor := TCastleDoor(TUserCastle(m_Castle).m_MainDoor.BaseObj);
      if CastleDoor.m_boDeath then
        sText := '损坏'
      else if CastleDoor.m_boOpened then
        sText := '开启'
      else
        sText := '关闭';
      sMsg := sub_49ADB8(nPos, sMsg, '<$CASTLEDOORSTATE>', sText);
    end
    else if sVariable = '$REPAIRDOORGOLD' then
    begin
      sText := IntToStr(g_Config.nRepairDoorPrice);
      sMsg := sub_49ADB8(nPos, sMsg, '<$REPAIRDOORGOLD>', sText);
    end
    else if sVariable = '$REPAIRWALLGOLD' then
    begin
      sText := IntToStr(g_Config.nRepairWallPrice);
      sMsg := sub_49ADB8(nPos, sMsg, '<$REPAIRWALLGOLD>', sText);
    end
    else if sVariable = '$GUARDFEE' then
    begin
      sText := IntToStr(g_Config.nHireGuardPrice);
      sMsg := sub_49ADB8(nPos, sMsg, '<$GUARDFEE>', sText);
    end
    else if sVariable = '$ARCHERFEE' then
    begin
      sText := IntToStr(g_Config.nHireArcherPrice);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ARCHERFEE>', sText);
    end
    else if sVariable = '$GUARDRULE' then
    begin
      sText := '无效';
      sMsg := sub_49ADB8(nPos, sMsg, '<$GUARDRULE>', sText);
    end
    else
    begin
      Result := False;
    end;
  end;
end;

procedure TCastleOfficial.UserSelect(PlayObject: TPlayObject; sData: string);
var
  s18, s20, sMsg, sLabel: string;
  boCanJmp: Boolean;
resourcestring
  sExceptionMsg = '[Exception] TCastleManager.UserSelect... ';
begin
  inherited;
  try
    // PlayObject.m_nScriptGotoCount:=0;
    if m_Castle = nil then
    begin
      PlayObject.SysMsg('NPC不属于城堡！', c_Red, t_Hint);
      Exit;
    end;
    if (sData <> '') and (sData[1] = '@') then
    begin
      sMsg := GetValidStr3_Ex(sData, sLabel, #13);
      s18 := '';
      PlayObject.m_sScriptLable := sData;
      PlayObject.m_sInputData := sMsg;
      if TUserCastle(m_Castle).IsMasterGuild(TGUild(PlayObject.m_MyGuild)) and (PlayObject.IsGuildMaster) then
      begin
        boCanJmp := PlayObject.LableIsCanJmp(sLabel);
        if SameText(sLabel, sNF_SendMsg) then
        begin
          if sMsg = '' then
            Exit;
        end;
        GotoLable(PlayObject, sLabel, not boCanJmp);
        if not boCanJmp then
          Exit;
        if SameText(sLabel, sNF_SendMsg) then
        begin
          SendCustemMsg(PlayObject, sMsg);
          PlayObject.SendMsg(Self, RM_MENU_OK, 0, NativeInt(Self), 0, 0, s18);
        end
        else if SameText(sLabel, sNF_CastleName) then
        begin
          sMsg := Trim(sMsg);
          if sMsg <> '' then
          begin
            TUserCastle(m_Castle).m_sName := sMsg;
            TUserCastle(m_Castle).Save;
            TUserCastle(m_Castle).m_MasterGuild.RefMemberName;
            s18 := '城堡名称更改成功...';
          end
          else
          begin
            s18 := '城堡名称更改失败！';
          end;
          PlayObject.SendMsg(Self, RM_MENU_OK, 0, NativeInt(Self), 0, 0, s18);
        end
        else if SameText(sLabel, sNF_WithDrawal) then
        begin
          case TUserCastle(m_Castle).WithDrawalGolds(PlayObject, StrToIntDef(sMsg, 0)) of
            -4:
              s18 := '输入的金币数不正确！';
            -3:
              s18 := '您无法携带更多的东西了。';
            -2:
              s18 := '该城内没有这么多金币.';
            -1:
              s18 := '只有行会 ' + TUserCastle(m_Castle).m_sOwnGuild + ' 的掌门人才能使用！';
            1:
              GotoLable(PlayObject, sNF_Main, False);
          end;
          PlayObject.SendMsg(Self, RM_MENU_OK, 0, NativeInt(Self), 0, 0, s18);
        end
        else if SameText(sLabel, sNF_Receipts) then
        begin
          case TUserCastle(m_Castle).ReceiptGolds(PlayObject, StrToIntDef(sMsg, 0)) of
            -4:
              s18 := '输入的金币数不正确！';
            -3:
              s18 := '你已经达到在城内存放货物的限制了。';
            -2:
              s18 := '你没有那么多金币.';
            -1:
              s18 := '只有行会 ' + TUserCastle(m_Castle).m_sOwnGuild + ' 的掌门人才能使用！';
            1:
              GotoLable(PlayObject, sNF_Main, False);
          end;
          PlayObject.SendMsg(Self, RM_MENU_OK, 0, NativeInt(Self), 0, 0, s18);
        end
        else if SameText(sLabel, sNF_OpenMainDoor) then
        begin
          TUserCastle(m_Castle).MainDoorControl(False);
        end
        else if SameText(sLabel, sNF_CloseMainDoor) then
        begin
          TUserCastle(m_Castle).MainDoorControl(True);
        end
        else if SameText(sLabel, sNF_RepairDoorNow) then
        begin
          RepairDoor(PlayObject);
          GotoLable(PlayObject, sNF_Main, False);
        end
        else if SameText(sLabel, sNF_RepairWallNow1) then
        begin
          RepairWallNow(1, PlayObject);
          GotoLable(PlayObject, sNF_Main, False);
        end
        else if SameText(sLabel, sNF_RepairWallNow2) then
        begin
          RepairWallNow(2, PlayObject);
          GotoLable(PlayObject, sNF_Main, False);
        end
        else if SameText(sLabel, sNF_RepairWallNow3) then
        begin
          RepairWallNow(3, PlayObject);
          GotoLable(PlayObject, sNF_Main, False);
        end
        else if CompareLStr(sLabel, sNF_HireguardNow, Length(sNF_HireguardNow)) then
        begin
          s20 := Copy(sLabel, Length(sNF_HireguardNow) + 1, Length(sLabel));
          HireGuard(s20, PlayObject);
          PlayObject.SendMsg(Self, RM_MENU_OK, 0, NativeInt(Self), 0, 0, '');
          // GotoLable(PlayObject,sHIREGUARDOK,False);
        end
        else if CompareLStr(sLabel, sNF_HirearcherNow, Length(sNF_HirearcherNow)) then
        begin
          s20 := Copy(sLabel, Length(sNF_HirearcherNow) + 1, Length(sLabel));
          HireArcher(s20, PlayObject);
          PlayObject.SendMsg(Self, RM_MENU_OK, 0, NativeInt(Self), 0, 0, '');
        end
        else if SameText(sLabel, sNF_Exit) then
        begin
          PlayObject.SendMsg(Self, RM_MERCHANTDLGCLOSE, 0, NativeInt(Self), 0, 0, '');
        end
        else if SameText(sLabel, sNF_Back) then
        begin
          if PlayObject.m_sScriptGoBackLable = '' then
            PlayObject.m_sScriptGoBackLable := sNF_Main;
          GotoLable(PlayObject, PlayObject.m_sScriptGoBackLable, False);
        end;
      end
      else
      begin
        s18 := '你没有权利使用';
        PlayObject.SendMsg(Self, RM_MENU_OK, 0, NativeInt(Self), 0, 0, s18);
      end;
    end;
  except
    MainOutMessage(sExceptionMsg);
  end;
  // inherited;
end;

procedure TCastleOfficial.HireGuard(sIndex: string; PlayObject: TPlayObject);
var
  n10: Integer;
  ObjUnit: pTObjUnit;
begin
  if m_Castle = nil then
  begin
    PlayObject.SysMsg('NPC不属于城堡！', c_Red, t_Hint);
    Exit;
  end;
  if TUserCastle(m_Castle).m_nTotalGold >= g_Config.nHireGuardPrice then
  begin
    n10 := StrToIntDef(sIndex, 0) - 1;
    if n10 <= MAXCALSTEGUARD then
    begin
      if TUserCastle(m_Castle).m_Guard[n10].BaseObj = nil then
      begin
        if not TUserCastle(m_Castle).m_boUnderWar then
        begin
          ObjUnit := @TUserCastle(m_Castle).m_Guard[n10];
          ObjUnit.BaseObj := UserEngine.RegenMonsterByName(TUserCastle(m_Castle).m_sMapName, ObjUnit.nX, ObjUnit.nY, ObjUnit.sName);
          if ObjUnit.BaseObj <> nil then
          begin
            Dec(TUserCastle(m_Castle).m_nTotalGold, g_Config.nHireGuardPrice);
            ObjUnit.BaseObj.m_Castle := TUserCastle(m_Castle);
            if ObjUnit.BaseObj is TGuardUnit then
            begin
              // TGuardUnit(ObjUnit.BaseObj).m_nX550 := ObjUnit.nX;
              // TGuardUnit(ObjUnit.BaseObj).m_nY554 := ObjUnit.nY;
              TGuardUnit(ObjUnit.BaseObj).m_nDirection := 3;
            end;
            PlayObject.SysMsg('雇佣成功.', c_Green, t_Hint);
          end;
        end
        else
        begin
          PlayObject.SysMsg('现在无法雇佣！', c_Red, t_Hint);
        end;
      end
      else
      begin
        PlayObject.SysMsg('早已雇佣！', c_Red, t_Hint);
      end;
    end
    else
    begin
      PlayObject.SysMsg('指令错误！', c_Red, t_Hint);
    end;
  end
  else
  begin
    PlayObject.SysMsg('城内资金不足！', c_Red, t_Hint);
  end;
end;

procedure TCastleOfficial.HireArcher(sIndex: string; PlayObject: TPlayObject);
var
  n10: Integer;
  ObjUnit: pTObjUnit;
begin
  if m_Castle = nil then
  begin
    PlayObject.SysMsg('NPC不属于城堡！', c_Red, t_Hint);
    Exit;
  end;
  if TUserCastle(m_Castle).m_nTotalGold >= g_Config.nHireArcherPrice then
  begin
    n10 := StrToIntDef(sIndex, 0) - 1;
    if n10 <= MAXCASTLEARCHER then
    begin
      if TUserCastle(m_Castle).m_Archer[n10].BaseObj = nil then
      begin
        if not TUserCastle(m_Castle).m_boUnderWar then
        begin
          ObjUnit := @TUserCastle(m_Castle).m_Archer[n10];
          ObjUnit.BaseObj := UserEngine.RegenMonsterByName(TUserCastle(m_Castle).m_sMapName, ObjUnit.nX, ObjUnit.nY, ObjUnit.sName);
          if ObjUnit.BaseObj <> nil then
          begin
            Dec(TUserCastle(m_Castle).m_nTotalGold, g_Config.nHireArcherPrice);
            ObjUnit.BaseObj.m_Castle := TUserCastle(m_Castle);
            if ObjUnit.BaseObj is TGuardUnit then
            begin
              // TGuardUnit(ObjUnit.BaseObj).m_nX550 := ObjUnit.nX;
              // TGuardUnit(ObjUnit.BaseObj).m_nY554 := ObjUnit.nY;
              TGuardUnit(ObjUnit.BaseObj).m_nDirection := 3;
            end;
            PlayObject.SysMsg('雇佣成功.', c_Green, t_Hint);
          end;
        end
        else
        begin
          PlayObject.SysMsg('现在无法雇佣！', c_Red, t_Hint);
        end;
      end
      else
      begin
        PlayObject.SysMsg('早已雇佣！', c_Red, t_Hint);
      end;
    end
    else
    begin
      PlayObject.SysMsg('指令错误！', c_Red, t_Hint);
    end;
  end
  else
  begin
    PlayObject.SysMsg('城内资金不足！', c_Red, t_Hint);
  end;
end;

{ TMerchant }
procedure TMerchant.AddItemPrice(nIndex: Integer; nPrice: Integer);
var
  ItemPrice: pTItemPrice;
begin
  New(ItemPrice);
  ItemPrice.wIndex := nIndex;
  ItemPrice.nPrice := nPrice;
  m_ItemPriceList.Add(ItemPrice);
  FrmDB.SaveGoodPriceRecord(Self, m_sScript + '-' + m_sMapName);
end;

procedure TMerchant.CheckItemPrice(nIndex: Integer);
var
  I: Integer;
  ItemPrice: pTItemPrice;
  StdItem: pTStdItem;
begin
  for I := 0 to m_ItemPriceList.Count - 1 do
  begin
    ItemPrice := m_ItemPriceList.Items[I];
    if ItemPrice = nil then
      Continue;
    if ItemPrice.wIndex = nIndex then
    begin
      { n10 := ItemPrice.nPrice;
        if Round(n10 * 1.1) > n10 then
        begin
        n10 := Round(n10 * 1.1);
        end
        else
        Inc(n10);
      }
      Exit;
    end;
  end;
  StdItem := UserEngine.GetStdItem(nIndex);
  if StdItem <> nil then
  begin
    AddItemPrice(nIndex, Round(StdItem.Price * 1.1));
  end;
end;

function TMerchant.GetRefillList(nIndex: Integer): TList;
var
  I: Integer;
  List: TList;
begin
  Result := nil;
  if nIndex <= 0 then
    Exit;
  for I := 0 to m_GoodsList.Count - 1 do
  begin
    List := TList(m_GoodsList.Items[I]);
    if List = nil then
      Continue;
    if List.Count > 0 then
    begin
      if pTUserItem(List.Items[0]).wIndex = nIndex then
      begin
        Result := List;
        Break;
      end;
    end;
  end;
end;

procedure TMerchant.RefillGoods;

  procedure RefillItems(var List: TList; sItemName: string; nInt: Integer);
  var
    I: Integer;
    UserItem: pTUserItem;
  begin
    if List = nil then
    begin
      List := TList.Create;
      m_GoodsList.Add(List);
    end;
    for I := 0 to nInt - 1 do
    begin
      New(UserItem);
      if UserEngine.CopyToUserItemFromName(sItemName, UserItem) then
      begin
        List.Insert(0, UserItem);
      end
      else
        Dispose(UserItem);
    end;
  end;

  procedure DelReFillItem(var List: TList; nInt: Integer);
  var
    I: Integer;
  begin
    for I := List.Count - 1 downto 0 do
    begin
      if nInt <= 0 then
        Break;
      if pTUserItem(List.Items[I]) <> nil then
      begin
        Dispose(pTUserItem(List.Items[I]));
      end;
      List.Delete(I);
      Dec(nInt);
    end;
  end;

var
  I, II: Integer;
  Goods: pTGoods;
  nIndex, nRefillCount: Integer;
  RefillList, RefillList20: TList;
  bo21: Boolean;
resourcestring
  sExceptionMsg = '[Exception] TMerchant.RefillGoods %s/%d:%d [%s] Code:%d';
begin
  try
    for I := 0 to m_RefillGoodsList.Count - 1 do
    begin
      Goods := m_RefillGoodsList.Items[I];
      if Goods = nil then
        Continue;
      if (MyGetTickCount - Goods.dwRefillTick) > Goods.dwRefillTime * 60 * 1000 then
      begin
        Goods.dwRefillTick := MyGetTickCount();
        nIndex := UserEngine.GetStdItemIdx(Goods.sItemName);
        if nIndex >= 0 then
        begin
          RefillList := GetRefillList(nIndex);
          nRefillCount := 0;
          if RefillList <> nil then
            nRefillCount := RefillList.Count;
          if Goods.nCount > nRefillCount then
          begin
            CheckItemPrice(nIndex);
            RefillItems(RefillList, Goods.sItemName, Goods.nCount - nRefillCount);
            FrmDB.SaveGoodRecord(Self, m_sScript + '-' + m_sMapName);
            FrmDB.SaveGoodPriceRecord(Self, m_sScript + '-' + m_sMapName);
          end;
          if Goods.nCount < nRefillCount then
          begin
            DelReFillItem(RefillList, nRefillCount - Goods.nCount);
            FrmDB.SaveGoodRecord(Self, m_sScript + '-' + m_sMapName);
            FrmDB.SaveGoodPriceRecord(Self, m_sScript + '-' + m_sMapName);
          end;
        end;
      end;
    end;
    for I := 0 to m_GoodsList.Count - 1 do
    begin
      RefillList20 := TList(m_GoodsList.Items[I]);
      if RefillList20 = nil then
        Continue;
      if RefillList20.Count > 1000 then
      begin
        bo21 := False;
        for II := 0 to m_RefillGoodsList.Count - 1 do
        begin
          Goods := m_RefillGoodsList.Items[II];
          if Goods = nil then
            Continue;
          nIndex := UserEngine.GetStdItemIdx(Goods.sItemName);
          if (pTItemPrice(RefillList20.Items[0]) <> nil) and (pTItemPrice(RefillList20.Items[0]).wIndex = nIndex) then
          begin
            bo21 := True;
            Break;
          end;
        end;
        if not bo21 then
        begin
          DelReFillItem(RefillList20, RefillList20.Count - 1000);
        end
        else
        begin
          DelReFillItem(RefillList20, RefillList20.Count - 5000);
        end;
      end;
    end;
  except
    on E: Exception do
      MainOutMessage(Format(sExceptionMsg, [m_sCharName, m_nCurrX, m_nCurrY, E.Message, 0]));
  end;
end;

function TMerchant.CheckItemType(nStdMode: Integer): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 0 to m_ItemTypeList.Count - 1 do
  begin
    if Integer(m_ItemTypeList.Items[I]) = nStdMode then
    begin
      Result := True;
      Break;
    end;
  end;
end;

function TMerchant.GetItemPrice(nIndex: Integer): Integer;
var
  I: Integer;
  ItemPrice: pTItemPrice;
  StdItem: pTStdItem;
begin
  Result := -1;
  for I := 0 to m_ItemPriceList.Count - 1 do
  begin
    ItemPrice := m_ItemPriceList.Items[I];
    if ItemPrice = nil then
      Continue;
    if ItemPrice.wIndex = nIndex then
    begin
      Result := ItemPrice.nPrice;
      Break;
    end;
  end; // for
  if Result < 0 then
  begin
    StdItem := UserEngine.GetStdItem(nIndex);
    if StdItem <> nil then
    begin
      if CheckItemType(StdItem.StdMode) then
        Result := StdItem.Price;
    end;
  end;
end;

procedure TMerchant.SaveUpgradingList();
begin
  try
    // FrmDB.SaveUpgradeWeaponRecord(m_sCharName,m_UpgradeWeaponList);
    FrmDB.SaveUpgradeWeaponRecord(m_sScript + '-' + m_sMapName, m_UpgradeWeaponList);
  except
    MainOutMessage('Failure in saving upgradinglist - ' + m_sCharName);
  end;
end;

procedure TMerchant.UpgradeWapon(User: TPlayObject); // 004A0920

  procedure sub_4A0218(ItemList: TList; var btDc: Byte; var btSc: Byte; var btMc: Byte; var btDura: Byte);
  var
    I, II, nDelCount: Integer;
    DuraList: TList;
    UserItem: pTUserItem;
    StdItem: pTStdItem;
    StdItem80: TStdItem;
    DelItems: string;
    nDc, nSc, nMc, nDcMin, nDcMax, nScMin, nScMax, nMcMin, nMcMax, nDura, nItemCount: Integer;
  begin
    nDcMin := 0;
    nDcMax := 0;
    nScMin := 0;
    nScMax := 0;
    nMcMin := 0;
    nMcMax := 0;
    nDura := 0;
    nItemCount := 0;
    DelItems := '';
    nDelCount := 0;
    DuraList := TList.Create;
    for I := ItemList.Count - 1 downto 0 do
    begin
      UserItem := ItemList.Items[I];
      if UserItem = nil then
        Continue;
      if UserEngine.GetStdItemName(UserItem.wIndex) = g_Config.sBlackStone then
      begin
        DuraList.Add(Pointer(Round(UserItem.Dura / 1.0E3)));
        DelItems := DelItems + Format('%s/%d/', [g_Config.sBlackStone, UserItem.MakeIndex]);
        StdItem := UserEngine.GetStdItem(UserItem.wIndex);
        if (StdItem <> nil) and (StdItem.NeedIdentify = 1) then
        begin
          AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, User, StdItem.Name, UserItem.MakeIndex, m_sCharName, 0, 0, '使用升级材料');
        end;
        ItemList.Delete(I);
        Dispose(UserItem);
        Inc(nDelCount);
      end
      else
      begin
        if IsUseItem(UserItem.wIndex) then
        begin
          StdItem := UserEngine.GetStdItem(UserItem.wIndex);
          if StdItem <> nil then
          begin
            StdItem80 := StdItem^;
            ItemUnit.GetItemAddValue(UserItem, StdItem80);
            nDc := 0;
            nSc := 0;
            nMc := 0;
            case StdItem80.StdMode of
              19, 20, 21:
                begin // 004A0421
                  nDc := StdItem80.DC2 + StdItem80.DC1;
                  nSc := StdItem80.SC2 + StdItem80.SC1;
                  nMc := StdItem80.MC2 + StdItem80.MC1;
                end;
              22, 23:
                begin // 004A046E
                  nDc := StdItem80.DC2 + StdItem80.DC1;
                  nSc := StdItem80.SC2 + StdItem80.SC1;
                  nMc := StdItem80.MC2 + StdItem80.MC1;
                end;
              24, 26:
                begin
                  nDc := StdItem80.DC2 + StdItem80.DC1 + 1;
                  nSc := StdItem80.SC2 + StdItem80.SC1 + 1;
                  nMc := StdItem80.MC2 + StdItem80.MC1 + 1;
                end;
            end;
            if nDcMin < nDc then
            begin
              nDcMax := nDcMin;
              nDcMin := nDc;
            end
            else
            begin
              if nDcMax < nDc then
                nDcMax := nDc;
            end;
            if nScMin < nSc then
            begin
              nScMax := nScMin;
              nScMin := nSc;
            end
            else
            begin
              if nScMax < nSc then
                nScMax := nSc;
            end;
            if nMcMin < nMc then
            begin
              nMcMax := nMcMin;
              nMcMin := nMc;
            end
            else
            begin
              if nMcMax < nMc then
                nMcMax := nMc;
            end;
            if (UserItem.btValue[13] = 1) and (UserItem.Name <> '') then
              DelItems := DelItems + Format('%s/%d/', [UserItem.Name, UserItem.MakeIndex])
            else
              DelItems := DelItems + Format('%s/%d/', [StdItem.Name, UserItem.MakeIndex]);
            // 004A06DB
            if StdItem.NeedIdentify = 1 then
            begin
              AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, User, StdItem.Name, UserItem.MakeIndex, m_sCharName, 0, 0, '使用升级材料');
            end;
            ItemList.Delete(I);
            Dispose(UserItem);
            Inc(nDelCount);
          end;
        end;
      end;
    end; // for
    for I := 0 to DuraList.Count - 1 do
    begin
      if DuraList.Count <= 0 then
        Break;
      for II := DuraList.Count - 1 downto I + 1 do
      begin
        if Integer(DuraList.Items[II]) > Integer(DuraList.Items[II - 1]) then
          DuraList.Exchange(II, II - 1);
      end; // for
    end; // for
    for I := 0 to DuraList.Count - 1 do
    begin
      nDura := nDura + Integer(DuraList.Items[I]);
      Inc(nItemCount);
      if nItemCount >= 5 then
        Break;
    end;
    btDura := Round(Min(5, nItemCount) + Min(5, nItemCount) * ((nDura / nItemCount) / 5.0));
    btDc := nDcMin div 5 + nDcMax div 3;
    btSc := nScMin div 5 + nScMax div 3;
    btMc := nMcMin div 5 + nMcMax div 3;
    if DelItems <> '' then
      User.SendMsg(Self, RM_SENDDELITEMLIST, 0, nDelCount, 0, 0, DelItems);
    if DuraList <> nil then
      DuraList.Free;
  end;

var
  I: Integer;
  bo0D: Boolean;
  UpgradeInfo: pTUpgradeInfo;
  StdItem: pTStdItem;
  Msg: string;
  OldGold: LongWord;
begin
  bo0D := False;
  for I := 0 to m_UpgradeWeaponList.Count - 1 do
  begin
    UpgradeInfo := m_UpgradeWeaponList.Items[I];
    if UpgradeInfo = nil then
      Continue;
    if UpgradeInfo.sUserName = User.m_sCharName then
    begin
      GotoLable(User, sNF_Upgradeing, False);
      Exit;
    end;
  end;
  if (User.m_UseItems[U_WEAPON].wIndex <> 0) and (User.m_nGold >= g_Config.nUpgradeWeaponPrice) and (User.CheckItems(g_Config.sBlackStone) <> nil) then
  begin
    StdItem := UserEngine.GetStdItem(User.m_UseItems[U_WEAPON].wIndex);
    { TODO -ochongchong -c新增 : 物品规则 - 禁止升级【2013-07-27】 }
    if (StdItem <> nil) and (Length(StdItem.Name) > 0) and g_ItemRules.Get(User.m_UseItems[U_WEAPON].wIndex, 17) then
    begin
      Msg := AnsiReplaceStr(g_sCannotUpgradeWeapon, '%Item', StdItem.Name);
      User.SysMsg(Msg, g_Config.btRedMsgFColor, g_Config.btRedMsgBColor, t_Hint);
      Exit;
    end;
    OldGold := User.m_nGold;
    User.DecGold(g_Config.nUpgradeWeaponPrice);
    if g_boGameLogGold then
    begin
      AddGameDataLog(LOG_ItemUpgrade, LOG_GoldChange, User, sSTRING_GOLDNAME, 0, m_sCharName, User.m_nGold, OldGold, '扣费:' + IntToStr(g_Config.nUpgradeWeaponPrice));
    end;
    if m_boCastle or g_Config.boGetAllNpcTax then
    begin
      if m_Castle <> nil then
      begin
        TUserCastle(m_Castle).IncRateGold(g_Config.nUpgradeWeaponPrice);
      end
      else if g_Config.boGetAllNpcTax then
      begin
        g_CastleManager.IncRateGold(g_Config.nUpgradeWeaponPrice);
      end;
    end;
    User.GoldChanged();
    New(UpgradeInfo);
    UpgradeInfo.sUserName := User.m_sCharName;
    UpgradeInfo.UserItem := User.m_UseItems[U_WEAPON];
    if (StdItem <> nil) and (StdItem.NeedIdentify = 1) then
    begin
      AddGameDataLog(LOG_ItemUpgrade, LOG_ActionNone, User, StdItem.Name, User.m_UseItems[U_WEAPON].MakeIndex, m_sCharName, 0, 0, '使用升级武器');
    end;
    User.SendDelItem(@User.m_UseItems[U_WEAPON]);
    User.m_UseItems[U_WEAPON].wIndex := 0;
    User.RecalcAbilitys();
    User.FeatureChanged();
    User.SendMsg(User, RM_ABILITY, 0, 0, 0, 0, '');
    sub_4A0218(User.m_ItemList, UpgradeInfo.btDc, UpgradeInfo.btSc, UpgradeInfo.btMc, UpgradeInfo.btDura);
    UpgradeInfo.dtTime := Now();
    UpgradeInfo.dwGetBackTick := MyGetTickCount();
    m_UpgradeWeaponList.Add(UpgradeInfo);
    SaveUpgradingList();
    bo0D := True;
  end;
  if bo0D then
    GotoLable(User, sNF_UpgradeOK, False)
  else
    GotoLable(User, sNF_UpgradeFail, False);
end;

procedure TMerchant.GetBackupgWeapon(User: TPlayObject); // 004A0CB8
var
  I: Integer;
  UpgradeInfo: pTUpgradeInfo;
  n10, n18, n1C, n90: Integer;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
begin
  n18 := 0;
  UpgradeInfo := nil;
  if not User.IsEnoughBag then
  begin
    // User.SysMsg('你的背包已经满了，无法再携带任何物品了！',0);
    GotoLable(User, sNF_GetBackupgFull, False);
    Exit;
  end;
  for I := m_UpgradeWeaponList.Count - 1 downto 0 do
  begin // for i := 0 to m_UpgradeWeaponList.Count - 1 do begin
    if m_UpgradeWeaponList.Count <= 0 then
      Break;
    if pTUpgradeInfo(m_UpgradeWeaponList.Items[I]).sUserName = User.m_sCharName then
    begin
      n18 := 1;
      if ((MyGetTickCount - pTUpgradeInfo(m_UpgradeWeaponList.Items[I]).dwGetBackTick) > g_Config.dwUPgradeWeaponGetBackTime) or (User.m_btPermission >= 4) then
      begin
        UpgradeInfo := m_UpgradeWeaponList.Items[I];
        m_UpgradeWeaponList.Delete(I);
        SaveUpgradingList();
        n18 := 2;
        Break;
      end;
    end;
  end;
  if UpgradeInfo <> nil then
  begin
    case UpgradeInfo.btDura of //
      0..8:
        begin // 004A0DE5
          // n14:=Max(3000,UpgradeInfo.UserItem.DuraMax shr 1);
          if UpgradeInfo.UserItem.DuraMax > 3000 then
          begin
            Dec(UpgradeInfo.UserItem.DuraMax, 3000);
          end
          else
          begin
            UpgradeInfo.UserItem.DuraMax := UpgradeInfo.UserItem.DuraMax shr 1;
          end;
          if UpgradeInfo.UserItem.Dura > UpgradeInfo.UserItem.DuraMax then
            UpgradeInfo.UserItem.Dura := UpgradeInfo.UserItem.DuraMax;
        end;
      9..15:
        begin // 004A0E41
          if Random(UpgradeInfo.btDura) < 6 then
          begin
            if UpgradeInfo.UserItem.DuraMax > 1000 then
              Dec(UpgradeInfo.UserItem.DuraMax, 1000);
            if UpgradeInfo.UserItem.Dura > UpgradeInfo.UserItem.DuraMax then
              UpgradeInfo.UserItem.Dura := UpgradeInfo.UserItem.DuraMax;
          end;
        end;
      18..255:
        begin
          case Random(UpgradeInfo.btDura - 18) of
            1..4:
              Inc(UpgradeInfo.UserItem.DuraMax, 1000);
            5..7:
              Inc(UpgradeInfo.UserItem.DuraMax, 2000);
            8..255:
              Inc(UpgradeInfo.UserItem.DuraMax, 4000)
          end;
        end;
    end; // case
    if (UpgradeInfo.btDc = UpgradeInfo.btMc) and (UpgradeInfo.btMc = UpgradeInfo.btSc) then
    begin
      n1C := Random(3);
    end
    else
    begin
      n1C := -1;
    end;
    if ((UpgradeInfo.btDc >= UpgradeInfo.btMc) and (UpgradeInfo.btDc >= UpgradeInfo.btSc)) or (n1C = 0) then
    begin
      n90 := Min(11, UpgradeInfo.btDc);
      n10 := Min(85, n90 shl 3 - n90 + 10 + UpgradeInfo.UserItem.btValue[3] - UpgradeInfo.UserItem.btValue[4] + User.m_nBodyLuckLevel);

      // n10:=Min(85,n90 * 8 - n90 + 10 + UpgradeInfo.UserItem.btValue[3] - UpgradeInfo.UserItem.btValue[4] + User.m_nBodyLuckLevel);
      if Random(g_Config.nUpgradeWeaponDCRate) < n10 then
      begin // if Random(100) < n10 then begin
        UpgradeInfo.UserItem.btValue[10] := 10;
        if (n10 > 63) and (Random(g_Config.nUpgradeWeaponDCTwoPointRate) = 0) then // if (n10 > 63) and (Random(30) = 0) then
          UpgradeInfo.UserItem.btValue[10] := 11;
        if (n10 > 79) and (Random(g_Config.nUpgradeWeaponDCThreePointRate) = 0) then // if (n10 > 79) and (Random(200) = 0) then
          UpgradeInfo.UserItem.btValue[10] := 12;
      end
      else
        UpgradeInfo.UserItem.btValue[10] := 1; // 004A0F89
    end;
    if ((UpgradeInfo.btMc >= UpgradeInfo.btDc) and (UpgradeInfo.btMc >= UpgradeInfo.btSc)) or (n1C = 1) then
    begin
      n90 := Min(11, UpgradeInfo.btMc);
      n10 := Min(85, n90 shl 3 - n90 + 10 + UpgradeInfo.UserItem.btValue[3] - UpgradeInfo.UserItem.btValue[4] + User.m_nBodyLuckLevel);
      if Random(g_Config.nUpgradeWeaponMCRate) < n10 then
      begin // if Random(100) < n10 then begin
        UpgradeInfo.UserItem.btValue[10] := 20;
        if (n10 > 63) and (Random(g_Config.nUpgradeWeaponMCTwoPointRate) = 0) then // if (n10 > 63) and (Random(30) = 0) then
          UpgradeInfo.UserItem.btValue[10] := 21;
        if (n10 > 79) and (Random(g_Config.nUpgradeWeaponMCThreePointRate) = 0) then // if (n10 > 79) and (Random(200) = 0) then
          UpgradeInfo.UserItem.btValue[10] := 22;
      end
      else
        UpgradeInfo.UserItem.btValue[10] := 1;
    end;
    if ((UpgradeInfo.btSc >= UpgradeInfo.btMc) and (UpgradeInfo.btSc >= UpgradeInfo.btDc)) or (n1C = 2) then
    begin
      n90 := Min(11, UpgradeInfo.btMc);
      n10 := Min(85, n90 shl 3 - n90 + 10 + UpgradeInfo.UserItem.btValue[3] - UpgradeInfo.UserItem.btValue[4] + User.m_nBodyLuckLevel);
      if Random(g_Config.nUpgradeWeaponSCRate) < n10 then
      begin // if Random(100) < n10 then begin
        UpgradeInfo.UserItem.btValue[10] := 30;
        if (n10 > 63) and (Random(g_Config.nUpgradeWeaponSCTwoPointRate) = 0) then // if (n10 > 63) and (Random(30) = 0) then
          UpgradeInfo.UserItem.btValue[10] := 31;
        if (n10 > 79) and (Random(g_Config.nUpgradeWeaponSCThreePointRate) = 0) then // if (n10 > 79) and (Random(200) = 0) then
          UpgradeInfo.UserItem.btValue[10] := 32;
      end
      else
        UpgradeInfo.UserItem.btValue[10] := 1;
    end;
    New(UserItem);
    UserItem^ := UpgradeInfo.UserItem;
    Dispose(UpgradeInfo);
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    // 004A120E
    if StdItem.NeedIdentify = 1 then
    begin
      AddGameDataLog(LOG_ItemUpgrade, LOG_ActionNone, User, StdItem.Name, UserItem.MakeIndex, m_sCharName, 0, 0, '升级取回');
    end;
    User.AddItemToBag(UserItem);
    User.SendAddItem(UserItem);
  end;
  case n18 of //
    0:
      GotoLable(User, sNF_GetBackupgFail, False);
    1:
      GotoLable(User, sNF_GetBackupging, False);
    2:
      GotoLable(User, sNF_GetBackupgOK, False);
  end; // case
end;

function TMerchant.GetUserPrice(PlayObject: TPlayObject; nPrice: Integer): Integer; // 0049F6E0
var
  n14: Integer;
begin
  {
    if m_boCastle then begin
    if UserCastle.IsMasterGuild(TGuild(PlayObject.m_MyGuild)) then begin
    n14:=Max(60,ROUND(m_nPriceRate * 8.0000000000000000001e-1));// 80%
    Result:=ROUND(nPrice / 1.0e2 * n14); // 100
    end else begin
    Result:=ROUND(nPrice / 1.0e2 * m_nPriceRate);
    end;
    end else begin
    Result:=ROUND(nPrice / 1.0e2 * m_nPriceRate);
    end;
  }
  if m_boCastle then
  begin
    // if UserCastle.IsMasterGuild(TGuild(PlayObject.m_MyGuild)) then begin
    if (m_Castle <> nil) and TUserCastle(m_Castle).IsMasterGuild(TGUild(PlayObject.m_MyGuild)) then
    begin
      n14 := Max(60, Round(m_nPriceRate * (g_Config.nCastleMemberPriceRate / 100))); // 80%
      Result := Round(nPrice / 100 * n14); // 100
    end
    else
    begin
      Result := Round(nPrice / 100 * m_nPriceRate);
    end;
  end
  else
  begin
    Result := Round(nPrice / 100 * m_nPriceRate);
  end;
end;

procedure TMerchant.UserSelect(PlayObject: TPlayObject; sData: string);

  procedure SuperRepairItem(User: TPlayObject);
  begin
    User.SendMsg(Self, RM_SENDUSERSREPAIR, 0, NativeInt(Self), 0, 0, '');
  end;

  procedure BuyItem(User: TPlayObject; nInt: Integer);
  var
    I, n10, nStock, nPrice, nCount: Integer;
    nSubMenu: ShortInt;
    sSENDMSG, sName: string;
    UserItem: pTUserItem;
    StdItem: pTStdItem;
    List14: TList;
  label
    RefBuy;
  begin
RefBuy:
    sSENDMSG := '';
    n10 := 0;
    for I := 0 to m_GoodsList.Count - 1 do
    begin
      List14 := TList(m_GoodsList.Items[I]);
      if List14 = nil then
      begin
        m_GoodsList.Delete(I);
        goto RefBuy;
        Exit;
      end;
      if List14.Count <= 0 then
      begin
        List14.Free;
        m_GoodsList.Delete(I);
        goto RefBuy;
        Exit;
      end;
      UserItem := List14.Items[0];
      if UserItem = nil then
        Continue;
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if StdItem <> nil then
      begin
        // 取自定义物品名称
        {
          sName := '';
          if UserItem.btValue[13] = 1 then
          sName := UserItem.Name;
          if sName = '' then
        }
        sName := StdItem.Name;
        GetUserPrice(PlayObject, GetUserItemPrice(UserItem, False));
        nPrice := GetItemPrice(UserItem.wIndex);
        // if List14.Count = 1 then
        begin
          if CheckOverLapItem(StdItem) then
          begin
            nCount := (UserItem.Dura + 1);
            nPrice := nPrice * nCount;
          end
          else
            nCount := 1;
          nPrice := GetUserPrice(User, nPrice);
          nStock := List14.Count;
          if (StdItem.StdMode <= 4) or (StdItem.StdMode = 42) or (StdItem.StdMode = 31) then
          begin
            nSubMenu := 0;
            // nMakeIndex := UserItem.MakeIndex;
            nStock := UserItem.MakeIndex;
          end
          else
          begin
            nSubMenu := 1;
            // nMakeIndex := 0;
          end;
        end;
        sSENDMSG := sSENDMSG + sName + '/' + IntToStr(nSubMenu) + '/' + IntToStr(nPrice) + '/' + IntToStr(nStock) + '/' + IntToStr(nCount) + '/' + IntToStr(StdItem.Looks) + '/';
        Inc(n10);
      end;
    end;
    if (nInt = 1) then
    begin
      User.SendMsg(Self, RM_SENDGOODSLIST, 0, NativeInt(Self), n10, 1, sSENDMSG);
    end
    else
    begin
      User.SendMsg(Self, RM_SENDGOODSLIST, 0, NativeInt(Self), n10, 0, sSENDMSG);
    end;
  end;

  procedure RemoteMsg(User: TPlayObject; sLabel, sMsg: string); // 接受歌曲
  var
    sSENDMSG: string;
    TargetObject: TPlayObject;
  begin
    sMsg := Trim(sMsg);
    if sMsg <> '' then
    begin
      TargetObject := UserEngine.GetPlayObject(sMsg);
      if TargetObject <> nil then
      begin
        if TargetObject.m_boRemoteMsg then
        begin
          sLabel := Copy(sLabel, 2, Length(sLabel) - 1);
          sSENDMSG := '你的好友 ' + User.m_sCharName + ' 给你发送音乐\ \<播放歌曲/' + sLabel + '>\';
          SendMsgToUser(TargetObject, sSENDMSG);
        end
        else
        begin
          User.SysMsg(sMsg + '你的好友 ' + TargetObject.m_sCharName + ' 拒绝接受歌曲！', c_Red, t_Hint);
        end;
      end
      else
      begin
        User.SysMsg(sMsg + g_sUserNotOnLine { '  没有在线！' } , c_Red, t_Hint);
      end;
    end;
  end;

  procedure AutoGetExp(User: TPlayObject; sMsg: string);
  begin
    User.m_sAutoSendMsg := sMsg;
    // User.SysMsg('挂机成功！', c_Red, t_Hint);
  end;

  procedure DealGold(User: TPlayObject; sMsg: string);
  var
    PoseHuman: TPlayObject;
    nGameGold: Integer;
  begin
    nGameGold := StrToIntDef(sMsg, -1);
    if User.m_nDealGoldPose <> 1 then
    begin
      GotoLable(User, '@dealgoldPlayError', False);
      Exit;
    end;
    User.m_nDealGoldPose := 2;
    if nGameGold <= 0 then
    begin
      GotoLable(User, '@dealgoldInputFail', False);
    end
    else
    begin
      if User.m_nGameGold >= nGameGold then
      begin
        PoseHuman := TPlayObject(User.GetPoseCreate());
        if (PoseHuman <> nil) and (TPlayObject(PoseHuman.GetPoseCreate) = User) and (PoseHuman.m_btRaceServer = RC_PLAYOBJECT) then
        begin
          Inc(PoseHuman.m_nGameGold, nGameGold);
          Dec(User.m_nGameGold, nGameGold);
          PoseHuman.GameGoldChanged;
          User.GameGoldChanged;
          SendMsgToUser(User, '转帐成功：' + #10 + '转出' + g_Config.sGameGoldName + '：' + IntToStr(nGameGold) + #9 + '当前' + g_Config.sGameGoldName + '：' + IntToStr(User.m_nGameGold), False);
          SendMsgToUser(PoseHuman, '转帐成功：' + #10 + '增加' + g_Config.sGameGoldName + '：' + IntToStr(nGameGold) + #9 + '当前' + g_Config.sGameGoldName + '：' + IntToStr(PoseHuman.m_nGameGold), False);
          // AddGameDataLog(

          // LOG_ItemDisappear, SmartObject.m_sMapName, SmartObject.m_nCurrX, SmartObject.m_nCurrY, StdItem.Name, UserItem.MakeIndex, SmartObject.m_sCharName, Npc.m_sCharName, 0, 0, Format('装备破碎 - [%s]%s', [Npc.m_sLastLabel, QuestActionInfo.sCmdLine]));
        end
        else
        begin
          GotoLable(User, '@dealgoldpost', False);
        end;
      end
      else
      begin
        GotoLable(User, '@dealgoldFail', False);
      end;
    end;
  end;

  procedure SellItem(User: TPlayObject); // 004A1544
  begin
    User.SendMsg(Self, RM_SENDUSERSELL, 0, NativeInt(Self), 0, 0, '');
  end;

  procedure RepairItem(User: TPlayObject); // 004A1570
  begin
    User.SendMsg(Self, RM_SENDUSERREPAIR, 0, NativeInt(Self), 0, 0, '');
  end;

  procedure ArmRemoveStoneItem(User: TPlayObject); // 004A1570
  begin
    User.SendMsg(Self, RM_ARMREMOVESTONE, 0, NativeInt(Self), 0, 0, '');
  end;

  procedure MakeDurg(User: TPlayObject); // 004A16A0
  var
    I: Integer;
    List14: TList;
    UserItem: pTUserItem;
    StdItem: pTStdItem;
    sSENDMSG: string;
  label
    RefMakeDurg;
  begin
RefMakeDurg:
    sSENDMSG := '';
    for I := 0 to m_GoodsList.Count - 1 do
    begin
      List14 := TList(m_GoodsList.Items[I]);
      if List14.Count <= 0 then
      begin
        List14.Free;
        m_GoodsList.Delete(I);
        goto RefMakeDurg;
        Exit;
      end;
      UserItem := List14.Items[0];
      if UserItem = nil then
        Continue;
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if StdItem <> nil then
      begin
        sSENDMSG := sSENDMSG + StdItem.Name + '/' + IntToStr(0) + '/' + IntToStr(g_Config.nMakeDurgPrice) + '/' + IntToStr(1) + '/';
      end;
    end;
    if sSENDMSG <> '' then
      User.SendMsg(Self, RM_USERMAKEDRUGITEMS, 0, NativeInt(Self), 0, 0, sSENDMSG);
  end;

  procedure ItemPrices(User: TPlayObject); //
  begin
  end;

  procedure Storage(User: TPlayObject; nPage: Integer); // 004A1648
  begin
    User.SendMsg(Self, RM_USERSTORAGEITEM, 0, NativeInt(Self), nPage, 0, '');
  end;

  procedure GetBack(User: TPlayObject; nPage: Integer); // 004A1674
  begin
    User.SendMsg(Self, RM_USERGETBACKITEM, 0, NativeInt(Self), nPage, 0, '');
  end;

  procedure BigStorage(User: TPlayObject);
  begin
    User.SendMsg(Self, RM_USERSTORAGEITEM, 0, NativeInt(Self), 0, 0, '');
  end;

  procedure BigGetBack(User: TPlayObject);
  begin
    User.m_nBigStoragePage := 0;
    User.SendMsg(Self, RM_USERBIGGETBACKITEM, User.m_nBigStoragePage, NativeInt(Self), 0, 0, '');
  end;

  procedure GetPreviousPage(User: TPlayObject);
  begin
    if User.m_nBigStoragePage > 0 then
      Dec(User.m_nBigStoragePage)
    else
      User.m_nBigStoragePage := 0;
    User.SendMsg(Self, RM_USERBIGGETBACKITEM, User.m_nBigStoragePage, NativeInt(Self), 0, 0, '');
  end;

  procedure GetNextPage(User: TPlayObject);
  begin
    Inc(User.m_nBigStoragePage);
    User.SendMsg(Self, RM_USERBIGGETBACKITEM, User.m_nBigStoragePage, NativeInt(Self), 0, 0, '');
  end;

  procedure MakeHeroName(User: TPlayObject; sLabel, sMsg: string);
  var
    sGotoLabel: string;
  begin
    // MainOutMessage(sLabel +' sMsg:'+sMsg);
    if (User.m_sHeroName <> '') then
    begin
      GotoLable(User, '@HaveHero', False);
    end
    else
    begin
      { 修正输入英雄名，选择英雄职业时直接关闭，再创建英雄会一直提示等待创建的Bug chongchong 2013-09-11 }
      if { (User.m_sTempHeroName <> '') or } User.m_boWaitHeroDate then
      begin
        GotoLable(User, '@CreateingHero', False);
        Exit;
      end;
      // 英雄名称必须多于3个字符 chongchong 2013-08-27
      if Length(AnsiString(sMsg)) < 4 then
      begin
        GotoLable(User, '@SetHeroName', False);
        Exit;
      end;
      if (Length(sMsg) > 0) and (Length(sMsg) < 15) then
      begin
        // 英雄名字、装备改名、行会名字、行会封号字符过滤 add chongchong 【2013-07-24】
        {
          if GetNameInFilterList(sMsg) then
          if (g_FilterTexts <> nil) then
          begin
          if g_FilterTexts.Filter(sMsg, sNewMsg) then
          begin                                             // 检测英雄名称是否有非法字符
          User.m_sHeroName := '';
          User.m_sTempHeroName := '';
          GotoLable(User, '@HeroNameFilter', False);
          Exit;
          end;
          end;
          // 英雄改名搞到DBServer上面，这里不要 2020-05-12 22:05:11
          if GetNameInFilterList(sMsg) then
          begin                                                                                       // 检测英雄名称是否有非法字符
          User.m_sHeroName := '';
          User.m_sTempHeroName := '';
          GotoLable(User, '@HeroNameFilter', False);
          Exit;
          end;
        }
        User.m_sTempHeroName := sMsg;
        sGotoLabel := Copy(sLabel, 2, Length(sLabel) - 1);
        // MainOutMessage('sGotoLabel:'+sGotoLabel);
        GotoLable(User, sGotoLabel, False); // gotoLabel '@CreateHero'
      end
      else
      begin
        User.m_sHeroName := '';
        User.m_sTempHeroName := '';
        GotoLable(User, '@HeroNameFilter', False);
      end;
    end;
  end;

  procedure MakeDeputyHeroName(User: TPlayObject; sLabel, sMsg: string);
  var
    sGotoLabel: string;
  begin
    // MainOutMessage(sLabel +' sMsg:'+sMsg);
    if (User.m_sDeputyHeroName <> '') then
    begin
      GotoLable(User, '@HaveHero', False);
    end
    else
    begin
      if (User.m_sTempHeroName <> '') or User.m_boWaitHeroDate then
      begin
        GotoLable(User, '@CreateingHero', False);
        Exit;
      end;
      if (Length(sMsg) > 0) and (Length(sMsg) < 15) then
      begin
        // 英雄名字、装备改名、行会名字、行会封号字符过滤 add chongchong 【2013-07-24】
        {
          if (g_FilterTexts <> nil) then
          begin
          if g_FilterTexts.Filter(sMsg, sNewMsg) then
          begin                                             // 检测英雄名称是否有非法字符
          User.m_sHeroName := '';
          User.m_sTempHeroName := '';
          GotoLable(User, '@HeroNameFilter', False);
          Exit;
          end;
          end;
          // 英雄改名搞到DBServer上面，这里不要 2020-05-12 22:05:11
          if GetNameInFilterList(sMsg) then
          begin                                                                                       // 检测英雄名称是否有非法字符
          User.m_sHeroName := '';
          User.m_sTempHeroName := '';
          GotoLable(User, '@HeroNameFilter', False);
          Exit;
          end;
        }
        User.m_sTempHeroName := sMsg;
        sGotoLabel := Copy(sLabel, 2, Length(sLabel) - 1);
        // MainOutMessage('MakeDeputyHeroName:' + sGotoLabel + ' m_sTempHeroName；' + User.m_sTempHeroName);
        GotoLable(User, sGotoLabel, False);
      end
      else
      begin
        User.m_sDeputyHeroName := '';
        User.m_sTempHeroName := '';
        GotoLable(User, '@HeroNameFilter', False);
      end;
    end;
  end;

  procedure InPutInteger(User: TPlayObject; sLabel, sMsg: string);
  var
    nNo: Integer;
    nValue: Integer;
  begin
    if IsStringNumber(sMsg) then
    begin
      if (g_InputBoxFilterList <> nil) then
      begin
        if GetInputBoxInFilterList(sMsg) then
        begin // 检测用户输入是否有非法字符
          GotoLable(User, '@InputIntegerFilter', False);
          Exit;
        end;
      end;
      nValue := StrToIntDef(sMsg, 0);
      nNo := StrToIntDef(Copy(sLabel, 15, Length(sLabel) - 14), -1);
      if (nNo >= 0) and (nNo <= 999) then
      begin
        User.m_nInteger[nNo] := nValue;
        GotoLable(User, Copy(sLabel, 2, Length(sLabel) - 1), False);
      end;
    end;
  end;

  procedure InPutString(User: TPlayObject; sLabel, sMsg: string);
  var
    nNo: Integer;
  begin
    nNo := StrToIntDef(Copy(sLabel, 14, Length(sLabel) - 13), -1);
    // MainOutMessage('InPutString:' + Copy(sLabel, 14, Length(sLabel) - 13) + ' sMsg:' + sMsg);
    if (nNo >= 0) and (nNo <= 999) then
    begin
      if (g_InputBoxFilterList <> nil) then
      begin
        if GetInputBoxInFilterList(sMsg) then // g_FilterTexts.Filter(sMsg, sNewMsg) then
        begin // 检测用户输入是否有非法字符
          GotoLable(User, '@InputStringFilter', False);
          Exit;
        end;
      end;
      // MainOutMessage('User.m_sString');
      User.m_sString[nNo] := sMsg;
      GotoLable(User, Copy(sLabel, 2, Length(sLabel) - 1), False);
    end;
  end;

  procedure PlayDrink(User: TPlayObject); // 斗酒
  begin
    User.SendMsg(Self, RM_SENDUSERPLAYDRINK, 0, NativeInt(Self), 0, 0, '');
  end;

var
  sMsg: string;
  sLabel: string;
  boCanJmp: Boolean;
  nPos: Integer;
  boCanGoto: Boolean;
  boAllowSelect: Boolean;
  I, nIndex: Integer;
  SL: TStringList;
  S1, S2: WideString;
  nCode: Integer;
resourcestring
  sExceptionMsg = '[Exception] TMerchant.UserSelect... Data: %s; Code: %d';
begin
  inherited;
  nCode := 0;
  if not (ClassNameIs(TMerchant.ClassName)) then
    Exit; // 如果类名不是 TMerchant 则不执行以下处理函数
  try
    nCode := 1;
    if not m_boCastle or not ((m_Castle <> nil) and TUserCastle(m_Castle).m_boUnderWar) and (PlayObject <> nil) then
    begin
      if { not PlayObject.m_boDeath 死亡可以点击NPC and } (sData <> '') and (sData[1] = '@') then
      begin
        // MainOutMessage('TMerchant.UserSelect m_sCharName:'+m_sCharName);
        sMsg := GetValidStr3_Ex(sData, sLabel, #13);
        nCode := 2;
        if (Length(sLabel) >= 2) and (sLabel[2] = '@') and (sLabel[Length(sLabel)] = ')') then
        begin
          nPos := Pos('(', sLabel); // 检测 <输入/@@InputInteger1(请输入元宝数量：)>
          if (nPos > 0) then
          begin
            sLabel := Copy(sLabel, 1, nPos - 1);
          end;
        end;
        nCode := 3;
        if CompareLStr(sLabel, '@FOUNDRYITEM_', Length('@FOUNDRYITEM_')) then
        begin
          PlayObject.m_sNpcSelectItemName := Copy(sLabel, Length('@FOUNDRYITEM_') + 1, Length(sLabel) - Length('@FOUNDRYITEM_'));
          sLabel := '@FOUNDRYITEM_';
        end
        else if CompareLStr(sLabel, '@SHOWITEM_', Length('@SHOWITEM_')) then
        begin
          PlayObject.m_sNpcSelectItemName := Copy(sLabel, Length('@SHOWITEM_') + 1, Length(sLabel) - Length('@SHOWITEM_'));
          sLabel := '@SHOWITEM_';
        end
        else
        begin
          PlayObject.m_sNpcSelectItemName := '';
        end;
        // @@dealybme
        nCode := 4;
        PlayObject.m_sScriptLable := sData;
        PlayObject.m_sInputData := sMsg;
        // 修正 在某种情况下可以直接调用 @StdModeFunc222 类似的东东 chongchong 2016-07-04
        // boCanGoto := PlayObject.LableIsCanJmp(sLabel);
        nCode := 5;
        boAllowSelect := AllowSelect(sLabel);
        nCode := 6;
        if (Self = g_FunctionNPC) or (Self = g_ManageNPC) or (Self = g_MissionNPC) then
          boCanGoto := PlayObject.LableIsCanJmp(sLabel) and boAllowSelect
        else
          boCanGoto := PlayObject.LableIsCanJmp(sLabel);
        if (not boCanGoto) and (PlayObject.m_boMessageBox) then
        begin
          boCanGoto := CompareLStr(sLabel, PlayObject.m_sYesLable, Length(sLabel)) or CompareLStr(sLabel, PlayObject.m_sNoLable, Length(sLabel));
        end;
        nCode := 7;
        if (Self = g_FunctionNPC) or (Self = g_MissionNPC) then
        begin
          if not boAllowSelect then
          begin
            MainOutMessage(Format('用户:%s; NPC:%s 禁止点用该NPC触发字段:%s', [PlayObject.m_sCharName, Self.m_sCharName, sLabel]));
            Exit;
          end;
        end;
        nCode := 8;
        boCanJmp := boCanGoto or ((Self = g_FunctionNPC) and boAllowSelect) or ((Self = g_MissionNPC) and boAllowSelect);
        nCode := 9;
        if SameText(sLabel, sNF_SendMsg) then
        begin
          if sMsg = '' then
            Exit;
        end;
        nCode := 10;
        if CompareLStr(sLabel, sNF_InputInteger, Length(sNF_InputInteger)) then
        begin
          if boCanGoto or ((Self = g_MissionNPC) and boAllowSelect) then // 防止非法刷变量 2020-11-04 23:27:59
          begin
            nCode := 11;
            if Length(sMsg) > 10 then
            begin
              MainOutMessage(Format('%s长度错误; 用户:%s; 长度:%d', [sLabel, PlayObject.m_sCharName, Length(sMsg)]));
              Exit;
            end;
            InPutInteger(PlayObject, sLabel, sMsg);
            Exit;
          end;
        end
        else if CompareLStr(sLabel, sNF_InputString, Length(sNF_InputString)) then
        begin
          if boCanGoto or ((Self = g_MissionNPC) and boAllowSelect) then // 防止非法刷变量 2020-11-04 23:27:59
          begin
            nCode := 12;
            if Length(sMsg) > g_Config.nMaxInputStringLen then
            begin
              MainOutMessage(Format('%s长度错误; 用户:%s; 长度:%d', [sLabel, PlayObject.m_sCharName, Length(sMsg)]));
              Exit;
            end;
            InPutString(PlayObject, sLabel, sMsg);
            Exit;
          end;
        end
        else if CompareLStr(sLabel, '@@copytoclipboard', Length('@@copytoclipboard')) then
        begin
          nCode := 12;
          GotoLable(PlayObject, Copy(sLabel, 2, MaxInt), False);
          Exit;
        end;
        nCode := 13;
        // 修正客户端发送 CM_MERCHANTDLGSELECT 来时，可以随意发 chongchong 2015-04-20
        if boCanGoto or ((Self = g_MissionNPC) and boAllowSelect) then
        begin
          nCode := 14;
          if Length(PlayObject.m_sInputData) > 0 then
          begin
            SL := TStringList.Create;
            try
              SL.Delimiter := #13;
              SL.DelimitedText := PlayObject.m_sInputData;
              for I := 0 to SL.Count - 1 do
              begin
                S1 := SL.Strings[I];
                if Length(sMsg) > g_Config.nMaxInputStringLen then
                begin
                  MainOutMessage(Format('%s长度错误; 用户:%s; 长度:%d', [sLabel, PlayObject.m_sCharName, Length(S1)]));
                  Exit;
                end;
                nCode := 15;
                nPos := Pos(':', S1);
                if nPos > 0 then
                begin
                  S2 := Copy(S1, nPos + 1, MaxInt);
                  S1 := Copy(S1, 1, nPos - 1);
                  nCode := 16;
                  nIndex := StrToIntDef(S1, 0) - 1;
                  if (nIndex >= Low(PlayObject.m_NpcInputText)) and (nIndex <= High(PlayObject.m_NpcInputText)) then
                  begin
                    if (g_InputBoxFilterList <> nil) then
                    begin
                      if GetInputBoxInFilterList(S2) then // 检测用户输入是否有非法字符
                      begin
                        GotoLable(PlayObject, '@InputBoxFilter', False);
                        Exit;
                      end;
                    end;
                    PlayObject.m_NpcInputText[nIndex] := S2;
                  end;
                end;
              end;
            finally
              SL.Free;
            end;
          end;
          nCode := 17;
          // 在gotolabl中处理参数 chongchong 2017-10-20
          if not GotoLable(PlayObject, sLabel, not boCanJmp, True) then
          begin
            nCode := 18;
            if not CheckLableNpcProcessCommand(sLabel) then
              MainOutMessage(Format('用户:%s; NPC:%s; 不存在的NPC触发字段:%s', [PlayObject.m_sCharName, Self.m_sCharName, sLabel]));
          end;
        end;
        // MainOutMessage('UserSelect:' + sLabel + ' PlayObject.m_sNpcSelectItemName:' + PlayObject.m_sNpcSelectItemName + ' boCanJmp:' + BoolToStr(boCanJmp));
        nCode := 19;
        if not boCanJmp then
          Exit;
        nCode := 20;
        nIndex := g_NpcProcessCommand.IndexOf(sLabel);
        if nIndex >= 0 then
        begin
          nIndex := Integer(g_NpcProcessCommand.Objects[nIndex]);
          nCode := 21;
          case nIndex of
            nNF_SendMsg:
              begin
                if m_boSendmsg then
                  SendCustemMsg(PlayObject, sMsg);
              end;
            nNF_SuperRepair:
              begin
                if m_boS_repair then
                  SuperRepairItem(PlayObject);
              end;
            nNF_Buy:
              begin
                if m_boBuy then
                  BuyItem(PlayObject, 0);
              end;
            nNF_Trading:
              begin
                if (g_nKey_Trading <> 0) and m_boBuy then
                  BuyItem(PlayObject, 1);
              end;
            nNF_Rmst:
              begin // 接受歌曲
                if m_boofflinemsg then
                  RemoteMsg(PlayObject, sLabel, sMsg);
              end;
            nNF_OfflineMsg:
              begin // 离线挂机
                if m_boofflinemsg then
                  AutoGetExp(PlayObject, sMsg);
              end;
            nNF_DealGold:
              begin
                if m_boDealGold then
                  DealGold(PlayObject, sMsg);
              end;
            nNF_Sell:
              begin
                if m_boSell then
                  SellItem(PlayObject);
              end;
            nNF_Repair:
              begin
                if m_boRepair then
                  RepairItem(PlayObject);
              end;
            nNF_ArmRemoveStone:
              begin
                if m_boArmRemoveStone then
                  ArmRemoveStoneItem(PlayObject);
              end;
            nNF_MakedUrg:
              begin
                if m_boMakeDrug then
                  MakeDurg(PlayObject);
              end;
            nNF_Prices:
              begin
                if m_boPrices then
                  ItemPrices(PlayObject);
              end;
            nNF_Storage:
              begin
                if m_boStorage then
                  Storage(PlayObject, 0);
              end;
            nNF_Storage2:
              begin
                if m_boStorage then
                  Storage(PlayObject, 1);
              end;
            nNF_Storage3:
              begin
                if m_boStorage then
                  Storage(PlayObject, 2);
              end;
            nNF_Storage4:
              begin
                if m_boStorage then
                  Storage(PlayObject, 3);
              end;
            nNF_Getback:
              begin
                if m_boGetback then
                  GetBack(PlayObject, 0);
              end;
            nNF_Getback2:
              begin
                if m_boGetback then
                  GetBack(PlayObject, 1);
              end;
            nNF_Getback3:
              begin
                if m_boGetback then
                  GetBack(PlayObject, 2);
              end;
            nNF_Getback4:
              begin
                if m_boGetback then
                  GetBack(PlayObject, 3);
              end;
            nNF_BigStorage:
              begin
                if m_boBigStorage then
                  BigStorage(PlayObject);
              end;
            nNF_BigGetback:
              begin
                if m_boBigGetBack then
                  BigGetBack(PlayObject);
              end;
            nNF_GetPreviousPage:
              begin
                if m_boGetPreviousPage then
                  GetPreviousPage(PlayObject);
              end;
            nNF_GetNextPage:
              begin
                if m_boGetNextPage then
                  GetNextPage(PlayObject);
              end;
            nNF_UpgradeNow:
              begin
                if m_boUpgradenow then
                  UpgradeWapon(PlayObject);
              end;
            nNF_GetBackupgNow:
              begin
                if m_boGetBackupgnow then
                  GetBackupgWeapon(PlayObject);
              end;
            nNF_GetMarry:
              begin
                if m_boGetMarry then
                  GetBackupgWeapon(PlayObject);
              end;
            nNF_GetMaster:
              begin
                if m_boGetMaster then
                  GetBackupgWeapon(PlayObject);
              end;
            nNF_UseItemName:
              begin
                if m_boUseItemName then
                  ChangeUseItemName(PlayObject, sLabel, sMsg);
              end;
            nNF_Exit:
              begin
                PlayObject.SendMsg(Self, RM_MERCHANTDLGCLOSE, 0, NativeInt(Self), 0, 0, '');
              end;
            nNF_CreateHero:
              begin
                if m_boCreateHeroName then
                  MakeHeroName(PlayObject, sLabel, sMsg);
              end;
            nNF_CreateDeputy:
              begin
                if m_boBuHero then
                  MakeDeputyHeroName(PlayObject, sLabel, sMsg);
              end;
            nNF_Back:
              begin
                if PlayObject.m_sScriptGoBackLable = '' then
                  PlayObject.m_sScriptGoBackLable := sNF_Main;
                GotoLable(PlayObject, PlayObject.m_sScriptGoBackLable, False);
              end;
            nNF_PlayDrink:
              begin
                if m_boPleaseDrink then
                  PlayDrink(PlayObject);
              end;
          end;
        end
        else
        begin
          if CompareLStr(sLabel, sNF_Rmst, Length(sNF_Rmst)) then
          begin // 接受歌曲
            if m_boofflinemsg then
              RemoteMsg(PlayObject, sLabel, sMsg);
          end
          else if CompareLStr(sLabel, sNF_UseItemName, Length(sNF_UseItemName)) then
          begin
            if m_boUseItemName then
              ChangeUseItemName(PlayObject, sLabel, sMsg);
          end
          else if (g_PluginManager <> nil) then
          begin
            g_PluginManager.HookUserSelect(Self, PlayObject, PAnsiChar(AnsiString(sLabel)), PAnsiChar(AnsiString(sMsg)));
          end;
        end;
      end;
    end;

    // 每次点击完标签以后，清空 <$SCRIPTPARAM1> - <$SCRIPTPARAM8> Cursor 2023-08-31 14:50:40
    for I := Low(PlayObject.m_sScriptParams) to High(PlayObject.m_sScriptParams) do
      PlayObject.m_sScriptParams[I] := '';
  except
    on E: Exception do
    begin
      MainOutMessage(Format(sExceptionMsg, [sData, nCode]));
      MainOutMessage(E.Message);
    end;
  end;
end;

procedure TMerchant.Run();
var
  nCheckCode: Integer;
resourcestring
  sExceptionMsg1 = '[Exception] TMerchant.Run... Code = %d';
  sExceptionMsg2 = '[Exception] TMerchant.Run -> Move Code = %d';
begin
  nCheckCode := 0;
  try
    if (MyGetTickCount - dwRefillGoodsTick) > 30000 then
    begin
      // if (MyGetTickCount - dwTick578) > 3000 then begin
      dwRefillGoodsTick := MyGetTickCount();
      RefillGoods();
    end;
    nCheckCode := 1;
    if (MyGetTickCount - dwClearExpreUpgradeTick) > 10 * 60 * 1000 then
    begin
      dwClearExpreUpgradeTick := MyGetTickCount();
      ClearExpreUpgradeListData();
    end;
    nCheckCode := 2;
    // 修复攻城模式，城堡中的交易NPC隐藏后又显示 +and (not m_boFixedHideMode) chongchong 2013-12-20
    if MyGetTickCount - FLastTrunTick >= FTrunTimeInterval then
    begin
      if (Random(50) = 0) and (not m_boFixedHideMode) then
      begin
        // 优化数据包流量占用 chongchong 2015-11-20
        if (not ((m_wAppr in [54..59, 70..75, 81..84, 90..92, 94..101, 211..225, 226..235]) or (m_wAppr = 245) or (m_wAppr = 273))) and (not ((m_wAppr >= 10000) and (Length(MovePoint) > 0))) then
        begin
          FLastTrunTick := MyGetTickCount;
          FTrunTimeInterval := 5000 + Random(6000);
          // 优化数据包流量占用(单纯的转转，不要 Feature等信息) chongchong 2015-11-20
          TurnToEx(Random(8));
        end;
      end
      else
      begin
        // 修复攻城模式，城堡中的交易NPC隐藏后又显示 +and (not m_boFixedHideMode) chongchong 2013-12-20
        if (Random(50) = 0) and (not m_boFixedHideMode) then
        begin
          FLastTrunTick := MyGetTickCount;
          FTrunTimeInterval := 5000 + Random(6000);
          // 优化数据包流量占用 chongchong 2015-11-20
          if (not ((m_wAppr in [54..75, 90..92, 94..100, 226..235, 211..225]) or (m_wAppr = 273))) and (not ((m_wAppr >= 10000) and (Length(MovePoint) > 0))) then
          begin
            SendRefMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
          end;
        end;
      end;
    end;
    nCheckCode := 3;
    // if m_boCastle and (UserCastle.m_boUnderWar)then begin
    if m_boCastle and (m_Castle <> nil) and TUserCastle(m_Castle).m_boUnderWar then
    begin
      if not m_boFixedHideMode then
      begin
        SendRefMsg(RM_DISAPPEAR, 0, 0, 0, 0, '');
        m_boFixedHideMode := True;
      end;
    end
    else
    begin
      if m_boFixedHideMode then
      begin
        m_boFixedHideMode := False;
        if ((m_wAppr >= 10000) and (Length(MovePoint) > 0)) then
          SendRefMsg(RM_TURN, m_btDirection, m_nCurrX, m_nCurrY, 0, '')
        else
          SendRefMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
      end;
    end;
    nCheckCode := 4;
  except
    on E: Exception do
    begin
      MainOutMessage(Format(sExceptionMsg1, [nCheckCode]));
      MainOutMessage(E.Message);
    end;
  end;
  try
    if m_boCanMove and (MyGetTickCount - m_dwMoveTick > m_dwMoveTime * 1000) then
    begin
      m_dwMoveTick := MyGetTickCount();
      SendRefMsg(RM_SPACEMOVE_FIRE, 0, 0, 0, 0, '');
      MapRandomMove(m_sMapName, 0);
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(Format(sExceptionMsg2, [nCheckCode]));
      MainOutMessage(E.Message);
    end;
  end;
  if Length(MovePoint) > 1 then
  begin
    if m_boWalkWaitLocked then
    begin
      if (MyGetTickCount - m_dwWalkWaitTick) > m_dwWalkWait then
      begin
        m_boWalkWaitLocked := False;
      end;
    end;
    if not m_boWalkWaitLocked and (tick_diff(m_dwWalkTick, MyGetTickCount) > m_nWalkSpeed + m_nWalkDelay) then
    begin
      m_dwWalkTick := MyGetTickCount();
      m_nWalkDelay := 0;
      Inc(m_nWalkCount);
      if m_nWalkCount > m_nWalkStep then
      begin
        m_nWalkCount := 0;
        m_boWalkWaitLocked := True;
        m_dwWalkWaitTick := MyGetTickCount();
      end;
      if (m_nTargetX <> -1) then
      begin
        if ((m_nCurrX = m_nTargetX) and (m_nCurrY = m_nTargetY)) or (nKeepCount > nKeepMaxCount) then
        begin
          m_nTargetX := -1;
        end;
      end;
      if m_nTargetX = -1 then
      begin
        Inc(nIdx);
        if nIdx > High(MovePoint) then
          nIdx := 0;
        if nIdx < 0 then
          nIdx := 0;
        SetTargetXY(MovePoint[nIdx].X, MovePoint[nIdx].Y);
        nKeepMaxCount := Max(abs(m_nCurrX - m_nTargetX), abs(m_nCurrY - m_nTargetY));
        nKeepMaxCount := nKeepMaxCount + Round(nKeepMaxCount * 0.5);
        nKeepCount := 0;
      end;
      if m_nTargetX <> -1 then
      begin
        GotoTargetXY;
        Inc(nKeepCount);
      end;
    end;
  end;
  // 修复攻城模式，城堡中的交易NPC隐藏后又显示 chongchong 2013-12-20
  // inherited;
  if (not m_boFixedHideMode) then
    inherited;
end;

function TMerchant.Operate(ProcessMsg: pTProcessMessage): Boolean;
begin
  Result := inherited Operate(ProcessMsg);
end;

procedure TMerchant.LoadNPCData;
var
  sFile: string;
begin
  sFile := m_sScript + '-' + m_sMapName;
  FrmDB.LoadGoodRecord(Self, sFile);
  FrmDB.LoadGoodPriceRecord(Self, sFile);
  LoadUpgradeList();
end;

procedure TMerchant.SaveNPCData;
var
  sFile: string;
begin
  sFile := m_sScript + '-' + m_sMapName;
  FrmDB.SaveGoodRecord(Self, sFile);
  FrmDB.SaveGoodPriceRecord(Self, sFile);
end;

constructor TMerchant.Create; // 0049EC70
begin
  inherited;
  m_btRaceImg := RC_MERCHANT;
  m_wAppr := 0;
  m_nPriceRate := 100;
  m_boCastle := False;
  m_ItemTypeList := TList.Create;
  m_RefillGoodsList := TList.Create;
  m_GoodsList := TList.Create;
  m_ItemPriceList := TList.Create;
  m_UpgradeWeaponList := TList.Create;
  dwRefillGoodsTick := MyGetTickCount();
  dwClearExpreUpgradeTick := MyGetTickCount();
  m_boBuy := False;
  m_boSell := False;
  m_boMakeDrug := False;
  m_boPrices := False;
  m_boStorage := False;
  m_boGetback := False;
  m_boBigStorage := False;
  m_boBigGetBack := False;
  m_boGetNextPage := False;
  m_boGetPreviousPage := False;
  m_boUpgradenow := False;
  m_boGetBackupgnow := False;
  m_boRepair := False;
  m_boS_repair := False;
  m_boGetMarry := False;
  m_boGetMaster := False;
  m_boUseItemName := False;
  m_boCreateHeroName := False;
  m_boGetSellGold := False;
  m_boSellOff := False;
  m_boBuyOff := False;
  m_boofflinemsg := False;
  m_boDealGold := False;
  m_boUpgradeNew := False;
  m_boPleaseDrink := False;
  m_boMakeWine := False;
  m_boBuHero := False;
  m_boReclaimItem := False;
  m_boArmRemoveStone := False;
  m_boFB := False;
  m_sFBName := '';
  m_dwMoveTick := MyGetTickCount();
  FTrunTimeInterval := 5000;
  MovePoint := nil;
  nIdx := 0;
  nKeepCount := 0;
  nKeepMaxCount := 0;
end;

destructor TMerchant.Destroy; // 0049ED70
var
  I: Integer;
  II: Integer;
  List: TList;
begin
  m_ItemTypeList.Free;
  for I := 0 to m_RefillGoodsList.Count - 1 do
  begin
    Dispose(pTGoods(m_RefillGoodsList.Items[I]));
  end;
  m_RefillGoodsList.Free;
  for I := 0 to m_GoodsList.Count - 1 do
  begin
    List := TList(m_GoodsList.Items[I]);
    for II := 0 to List.Count - 1 do
    begin
      Dispose(pTUserItem(List.Items[II]));
    end;
    List.Free;
  end;
  m_GoodsList.Free;
  for I := 0 to m_ItemPriceList.Count - 1 do
  begin
    Dispose(pTItemPrice(m_ItemPriceList.Items[I]));
  end;
  m_ItemPriceList.Free;
  for I := 0 to m_UpgradeWeaponList.Count - 1 do
  begin
    Dispose(pTUpgradeInfo(m_UpgradeWeaponList.Items[I]));
  end;
  m_UpgradeWeaponList.Free;
  MovePoint := nil;
  inherited;
end;

procedure TMerchant.ClearExpreUpgradeListData; // 004A01A0
var
  I: Integer;
  UpgradeInfo: pTUpgradeInfo;
begin
  for I := m_UpgradeWeaponList.Count - 1 downto 0 do
  begin
    if m_UpgradeWeaponList.Count <= 0 then
      Break;
    UpgradeInfo := m_UpgradeWeaponList.Items[I];
    if UpgradeInfo = nil then
      Continue;
    if Integer(Round(Now - UpgradeInfo.dtTime)) >= g_Config.nClearExpireUpgradeWeaponDays then
    begin
      Dispose(UpgradeInfo);
      m_UpgradeWeaponList.Delete(I);
    end;
  end;
end;

procedure TMerchant.LoadNpcScript(IsAddMapName: Boolean = True);
var
  SC: string;
begin
  m_ItemTypeList.Clear;
  m_sPath := sMarket_Def;
  { 副本地图 -- 副本地图NPC对应脚本 chongchong 2013-09-11 }
  if IsAddMapName then
  begin
    if m_boFB then
      SC := m_sScript + '-' + m_sFBName
    else
      SC := m_sScript + '-' + m_sMapName;
  end
  else
  begin
    if m_boFB then
      SC := m_sScript
    else
      SC := m_sScript;
  end;
  FrmDB.LoadScriptFile(Self, sMarket_Def, SC, True);
  FrmDB.LoadIconFile(Self, @m_ActorIcons, sNpcIcons, SC);
  // call    sub_49ABE0
end;

procedure TMerchant.LoadNpcIconFile(IsAddMapName: Boolean = True);
var
  SC: string;
begin
  { 副本地图 -- 副本地图NPC对应脚本 chongchong 2013-09-11 }
  if IsAddMapName then
  begin
    if m_boFB then
      SC := m_sScript + '-' + m_sFBName
    else
      SC := m_sScript + '-' + m_sMapName;
  end
  else
  begin
    if m_boFB then
      SC := m_sScript
    else
      SC := m_sScript;
  end;
  FrmDB.LoadIconFile(Self, @m_ActorIcons, sNpcIcons, SC);
end;

procedure TMerchant.Click(PlayObject: TPlayObject); // 0049FF24
begin
  // GotoLable(PlayObject,'@main');
  inherited;
end;

function TMerchant.GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string; var IsBreakParseVar: Boolean; nPos: Integer): Boolean; // 0049FD04
var
  sText: string;
begin
  Result := inherited GetVariableText(PlayObject, sMsg, sVariable, IsBreakParseVar, nPos);
  if not Result then
  begin
    Result := True;
    sVariable := UpperCase(sVariable);
    if sVariable = '$PRICERATE' then
    begin
      sText := IntToStr(m_nPriceRate);
      sMsg := sub_49ADB8(nPos, sMsg, '<$PRICERATE>', sText);
      Exit;
    end;
    if sVariable = '$UPGRADEWEAPONFEE' then
    begin
      sText := IntToStr(g_Config.nUpgradeWeaponPrice);
      sMsg := sub_49ADB8(nPos, sMsg, '<$UPGRADEWEAPONFEE>', sText);
      Exit;
    end;
    if sVariable = '$USERWEAPON' then
    begin
      if PlayObject.m_UseItems[U_WEAPON].wIndex <> 0 then
      begin
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_WEAPON].wIndex);
      end
      else
      begin
        sText := '无';
      end;
      sMsg := sub_49ADB8(nPos, sMsg, '<$USERWEAPON>', sText);
      Exit;
    end;
    Result := False;
  end;
end;

function TMerchant.GetUserItemPrice(UserItem: pTUserItem; IsSellToNpc: Boolean): Integer;
var
  n10: Integer;
  StdItem: pTStdItem;
  n20: real;
  nC: Integer;
  n14: Integer;
begin
  StdItem := nil;
  n10 := GetItemPrice(UserItem.wIndex);
  if n10 > 0 then
  begin
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (StdItem <> nil) and (StdItem.StdMode > 4) and (StdItem.DuraMax > 0) and (UserItem.DuraMax > 0) then
    begin
      if StdItem.StdMode = 40 then
      begin // 肉
        // n10 := Max(2, Round(n10 * UserItem.Dura / UserItem.DuraMax * 100));  //修复矿石纯度越高出售价格越低的问题
        { if UserItem.Dura <= UserItem.DuraMax then begin
          n20 := (n10 / 2.0 / UserItem.DuraMax * (UserItem.DuraMax - UserItem.Dura));
          n10 := Max(2, Round(n10 - n20));
          end else begin
          n10 := n10 + Round(n10 / UserItem.DuraMax * 2.0 * (UserItem.DuraMax - UserItem.Dura));
          end; }
        n20 := (n10 / 2.0 / UserItem.DuraMax * (UserItem.DuraMax - UserItem.Dura));
        n10 := Max(2, Round(n10 - n20));
      end;
      if (StdItem.StdMode = 43) then
      begin
        // n10 := Max(2, Round(n10 * UserItem.Dura / UserItem.DuraMax * 100));  //修复矿石纯度越高出售价格越低的问题
        if UserItem.DuraMax < 10000 then
          UserItem.DuraMax := 10000;
        { if UserItem.Dura <= UserItem.DuraMax then begin
          n20 := (n10 / 2.0 / UserItem.DuraMax * (UserItem.DuraMax - UserItem.Dura));
          n10 := Max(2, Round(n10 - n20));
          end else begin
          n10 := n10 + Round(n10 / UserItem.DuraMax * 1.3 * (UserItem.DuraMax - UserItem.Dura));
          end; }
        n20 := (n10 / 2.0 / UserItem.DuraMax * (UserItem.DuraMax - UserItem.Dura));
        n10 := Max(2, Round(n10 - n20));
      end;
      if StdItem.StdMode > 4 then
      begin
        if (not IsSellToNpc) or (not g_Config.boSellItemToNpcShopNoCalcAddProperty) then
        begin
          n14 := 0;
          nC := 0;
          while (True) do
          begin
            if (StdItem.StdMode in [5, 6, 68, 69]) then
            begin
              if (nC <> 4) or (nC <> 9) then
              begin
                if nC = 6 then
                begin
                  if UserItem.btValue[nC] > 10 then
                  begin
                    n14 := n14 + (UserItem.btValue[nC] - 10) * 2;
                  end;
                end
                else
                begin
                  n14 := n14 + UserItem.btValue[nC];
                end;
              end;
            end
            else
            begin
              Inc(n14, UserItem.btValue[nC]);
            end;
            Inc(nC);
            if nC >= 8 then
              Break;
          end;
          if n14 > 0 then
          begin
            n10 := n10 + Round(n10 * n14 / 200); // 修复极品装备价格币普通装备低的问题
            // n10 := n10 div 5 * n14;
          end;
        end;
        // 叠加物品的装备价格计算错误 chongchong 2014-05-22
        if not CheckOverLapItem(StdItem) then
        begin
          n10 := Round(n10 / StdItem.DuraMax * UserItem.DuraMax);
          n20 := (n10 / 2.0 / UserItem.DuraMax * (UserItem.DuraMax - UserItem.Dura));
          n10 := Max(2, Round(n10 - n20));
        end;
      end;
    end;
  end;
  if (StdItem <> nil) and CheckOverLapItem(StdItem) then // 叠加物品
    n10 := n10 * (UserItem.Dura + 1);
  Result := n10;
end;

procedure TMerchant.ClientBuyItem(PlayObject: TPlayObject; sItemName: string; nCount, nInt: Integer; IsFromTradingDlg: Boolean);
var
  I, II: Integer;
  bo29: Boolean;
  List20: TList;
  UserItem: pTUserItem;
  OverLapItem: pTUserItem;
  StdItem: pTStdItem;
  n1C, nPrice, nNewPrice: Integer;
  sUserItemName: string;
  sSendText: string;
  nWeight: Integer;
  nDura: Integer;
  nMakeIndex: Integer;
  boDelete: Boolean;
  nItemCount: Integer;
  nStock: Integer;
  nSubMenu: Integer;
begin
  if g_OnlineMsgControl.boDisableBuy then
    Exit;
  bo29 := False;
  n1C := 1;
  // I := 0;
  sSendText := '';
  nItemCount := 0;
  boDelete := False;
  if nCount <= 0 then
    nCount := 1;
  for I := m_GoodsList.Count - 1 downto 0 do
  begin
    if bo29 or (bo574) then
      Break;
    List20 := TList(m_GoodsList.Items[I]);
    if List20 = nil then
      Continue;
    if List20.Count <= 0 then
      Continue;
    for II := List20.Count - 1 downto 0 do
    begin
      UserItem := List20.Items[II];
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if StdItem = nil then
        Continue;
      // 取自定义物品名称
      if (UserItem.btValue[13] = 1) and (UserItem.Name <> '') then
        sUserItemName := UserItem.Name
      else
        sUserItemName := UserEngine.GetStdItemName(UserItem.wIndex);
      if SameText(sUserItemName, sItemName) then
      begin
        if (UserItem.MakeIndex = nInt) then
        begin
          nWeight := StdItem.Weight;
          if CheckOverLapItem(StdItem) { and (UserItem.MakeIndex = nInt) } then
          begin // 计算购买叠加物品
            if StdItem.OverLap = 1 then
              nWeight := Min(Max(StdItem.Weight * nCount div 10, StdItem.Weight), High(Byte))
            else
              nWeight := Min(StdItem.Weight * nCount, High(Byte));
          end;
          if PlayObject.IsAddWeightAvailable(nWeight) then
          begin
            nPrice := GetUserPrice(PlayObject, GetUserItemPrice(UserItem, False));
            if CheckOverLapItem(StdItem) { and (UserItem.MakeIndex = nInt) } then
            begin // 叠加物品 计算购买数量的价格
              if nCount < UserItem.Dura + 1 then
                nPrice := Round(nCount * (nPrice / (UserItem.Dura + 1)))
                // 修正购买叠加物品，数量大于可买数量时，可以刷物品 2019-11-26 00:44:40
              else if nCount > UserItem.Dura + 1 then
                nCount := UserItem.Dura + 1;
            end;
            if (PlayObject.m_nGold >= nPrice) and (nPrice > 0) then
            begin
              OverLapItem := OverLapItems(PlayObject, StdItem, nCount - 1);
              if OverLapItem <> nil then
              begin // 包裹里有可以叠加的物品
                Dec(PlayObject.m_nGold, nPrice);
                if m_boCastle or g_Config.boGetAllNpcTax then
                begin
                  if m_Castle <> nil then
                  begin
                    TUserCastle(m_Castle).IncRateGold(nPrice);
                  end
                  else if g_Config.boGetAllNpcTax then
                  begin
                    g_CastleManager.IncRateGold(g_Config.nUpgradeWeaponPrice);
                  end;
                end;
                nDura := UserItem.Dura + 1;
                nDura := nDura - nCount;
                if nDura <= 0 then
                begin
                  nCount := 0;
                  List20.Delete(II);
                  Dispose(UserItem);
                  boDelete := True;
                end
                else
                begin
                  UserItem.Dura := nDura - 1;
                  nCount := nDura;
                  nItemCount := UserItem.Dura + 1;
                  // nPrice := GetItemPrice(UserItem.wIndex);
                  // if CheckOverLapItem(StdItem) then
                  // nPrice := nPrice * (UserItem.Dura + 1);
                  // sSendText := IntToStr(nPrice);
                end;
                UserItem := OverLapItem;
                if List20.Count <= 0 then
                begin
                  FreeAndNil(List20);
                  m_GoodsList.Delete(I);
                end;
                if StdItem.NeedIdentify = 1 then
                begin
                  AddGameDataLog(LOG_ItemBuy, LOG_GoldChange, PlayObject, StdItem.Name, UserItem.MakeIndex, m_sCharName, PlayObject.m_nGold, -nPrice, 'NPC购买 [' + sSTRING_GOLDNAME + ']');
                end;
                // 增加显示下一个物品
                if boDelete and (List20 <> nil) and (List20.Count > 0) then
                begin
                  // MainOutMessage('ClientBuyItem:'+IntToStr(nItemCount)+' UserItem.MakeIndex:'+IntToStr(UserItem.MakeIndex)+' nInt:'+IntToStr(nInt)+' List20.Count:'+IntToStr(List20.Count));
                  UserItem := List20.Items[0];
                  StdItem := UserEngine.GetStdItem(UserItem.wIndex);
                  if StdItem <> nil then
                  begin
                    sUserItemName := '';
                    if UserItem.btValue[13] = 1 then
                      sUserItemName := UserItem.Name;
                    if sUserItemName = '' then
                      sUserItemName := StdItem.Name;
                    if CompareText(sUserItemName, sItemName) = 0 then
                    begin
                      nNewPrice := GetItemPrice(UserItem.wIndex);
                      if CheckOverLapItem(StdItem) then
                      begin
                        nCount := (UserItem.Dura + 1);
                        nNewPrice := nNewPrice * nCount;
                      end
                      else
                        nCount := 1;
                      nNewPrice := GetUserPrice(PlayObject, nNewPrice);
                      // nPrice := GetUserPrice(PlayObject, GetItemPrice(UserItem.wIndex));
                      nStock := List20.Count;
                      if (StdItem.StdMode <= 4) or (StdItem.StdMode = 42) or (StdItem.StdMode = 31) then
                      begin
                        nSubMenu := 0;
                        nStock := UserItem.MakeIndex;
                      end
                      else
                      begin
                        nSubMenu := 1;
                      end;
                      if CheckOverLapItem(StdItem) or (nSubMenu = 0) then
                      begin
                        if nSubMenu = 0 then
                        begin
                          sSendText := sUserItemName + '/' + IntToStr(nSubMenu) + '/' + IntToStr(nPrice) + '/' + IntToStr(nStock) + '/' + IntToStr(nCount);
                        end
                        else
                          sSendText := '+' + sUserItemName + '/' + IntToStr(nNewPrice) + '/' + IntToStr(UserItem.MakeIndex) + '/' + IntToStr(UserItem.Dura) + '/' + IntToStr(nCount);
                      end;
                    end;
                  end;
                end;
                n1C := 0;
                Break;
              end
              else { // if OverLapItem <> nil then begin 包裹里没有可以叠加的物品 }
                if PlayObject.IsEnoughBag then
              begin
                if CheckOverLapItem(StdItem) { and (UserItem.MakeIndex = nInt) } then
                begin
                  nDura := UserItem.Dura + 1;
                  nDura := nDura - nCount;
                  if nDura <= 0 then
                  begin // 购买全部叠加物品
                      // nCount := nCount - nDura;
                    nCount := 0;
                    PlayObject.AddItemToBag(UserItem);
                    List20.Delete(II);
                    boDelete := True;
                  end
                  else
                  begin // 购买部分叠加物品
                    New(OverLapItem);
                    if UserEngine.CopyToUserItemFromName(StdItem.Name, OverLapItem) then
                    begin
                      nMakeIndex := OverLapItem.MakeIndex;
                      Move(UserItem^, OverLapItem^, SizeOf(TUserItem));
                        // OverLapItem^ := UserItem^;
                      OverLapItem.MakeIndex := nMakeIndex;
                      OverLapItem.Dura := nCount - 1;
                      UserItem.Dura := nDura - 1;
                      nCount := nDura;
                      nItemCount := UserItem.Dura + 1;
                        // nPrice := GetItemPrice(UserItem.wIndex);
                        // if CheckOverLapItem(StdItem) then
                        // nPrice := nPrice * (UserItem.Dura + 1);
                        // nPrice := GetUserPrice(PlayObject, nPrice);
                        // sSendText := IntToStr(nPrice);
                      PlayObject.AddItemToBag(OverLapItem);
                        // MainOutMessage('ClientBuyItem:'+IntToStr(nItemCount)+' UserItem.MakeIndex:'+IntToStr(UserItem.MakeIndex)+' nInt:'+IntToStr(nInt));
                      UserItem := OverLapItem;
                    end
                    else
                    begin
                      Dispose(OverLapItem);
                      Break;
                    end;
                  end;
                end
                else
                begin
                  nCount := 0;
                  PlayObject.AddItemToBag(UserItem);
                  List20.Delete(II);
                  boDelete := True;
                end;
                  // if PlayObject.AddItemToBag(UserItem) then begin
                Dec(PlayObject.m_nGold, nPrice);
                if m_boCastle or g_Config.boGetAllNpcTax then
                begin
                  if m_Castle <> nil then
                  begin
                    TUserCastle(m_Castle).IncRateGold(nPrice);
                  end
                  else if g_Config.boGetAllNpcTax then
                  begin
                    g_CastleManager.IncRateGold(g_Config.nUpgradeWeaponPrice);
                  end;
                end;
                  {
                    if m_boCastle or g_Config.boGetAllNpcTax then
                    UserCastle.IncRateGold(nPrice);
                  }
                if UserItem.ItemFrom.ItemForm = ifUnknow then
                begin
                  UserItem.ItemFrom.ItemForm := ifShopBuy;
                  UserItem.ItemFrom.sMakerName := PlayObject.m_sCharName;
                  UserItem.ItemFrom.DateTime := Now();
                end;
                PlayObject.SendAddItem(UserItem);
                if StdItem.NeedIdentify = 1 then
                begin
                  AddGameDataLog(LOG_ItemBuy, LOG_GoldChange, PlayObject, StdItem.Name, UserItem.MakeIndex, m_sCharName, PlayObject.m_nGold, -nPrice, 'NPC购买 [' + sSTRING_GOLDNAME + ']');
                end;
                  // List20.Delete(II);
                if (List20 <> nil) and (List20.Count <= 0) then
                begin
                  FreeAndNil(List20);
                  m_GoodsList.Delete(I);
                end;
                  // 增加显示下一个物品
                if boDelete and (List20 <> nil) and (List20.Count > 0) then
                begin
                  UserItem := List20.Items[0];
                    // 取自定义物品名称
                  StdItem := UserEngine.GetStdItem(UserItem.wIndex);
                  if StdItem <> nil then
                  begin
                    sUserItemName := '';
                    if UserItem.btValue[13] = 1 then
                      sUserItemName := UserItem.Name;
                    if sUserItemName = '' then
                      sUserItemName := StdItem.Name;
                    if CompareText(sUserItemName, sItemName) = 0 then
                    begin
                      nNewPrice := GetItemPrice(UserItem.wIndex);
                      if CheckOverLapItem(StdItem) then
                      begin
                        nCount := (UserItem.Dura + 1);
                        nNewPrice := nNewPrice * nCount;
                      end
                      else
                        nCount := 1;
                      nNewPrice := GetUserPrice(PlayObject, nNewPrice);
                      nStock := List20.Count;
                      if (StdItem.StdMode <= 4) or (StdItem.StdMode = 42) or (StdItem.StdMode = 31) then
                      begin
                        nSubMenu := 0;
                        nStock := UserItem.MakeIndex;
                      end
                      else
                      begin
                        nSubMenu := 1;
                      end;
                        // MainOutMessage(IntToStr(nSubMenu));
                      if CheckOverLapItem(StdItem) or (nSubMenu = 0) then
                      begin
                        if nSubMenu = 0 then
                        begin
                          sSendText := sUserItemName + '/' + IntToStr(nSubMenu) + '/' + IntToStr(nPrice) + '/' + IntToStr(nStock) + '/' + IntToStr(nCount);
                        end
                        else
                          sSendText := '+' + sUserItemName + '/' + IntToStr(nNewPrice) + '/' + IntToStr(UserItem.MakeIndex) + '/' + IntToStr(UserItem.Dura) + '/' + IntToStr(nCount);
                      end;
                    end;
                  end;
                end;
                n1C := 0;
                Break;
              end
              else
                n1C := 2; // if PlayObject.IsEnoughBag then begin
            end
            else
              n1C := 3;
          end
          else
            n1C := 2; // 004A2639
          bo29 := True;
        end;
      end;
    end;
  end;
  // for
  if n1C = 0 then
    PlayObject.SendMsg(Self, RM_BUYITEM_SUCCESS, Integer(IsFromTradingDlg), PlayObject.m_nGold, nInt, nItemCount, sSendText)
  else
    PlayObject.SendMsg(Self, RM_BUYITEM_FAIL, 0, n1C, 0, 0, '');
end;

procedure TMerchant.ClientGetDetailGoodsList(PlayObject: TPlayObject; sItemName: string; nInt: Integer; IsFromTradingDlg: Boolean);
var
  I, II, nCount: Integer;
  List20: TList;
  UserItem: pTUserItem;
  StdItem: pTStdItem;
  InBuf: array[0..SizeOf(TClientItem) * 20] of AnsiChar;
  ClientItem: pTClientItem;
  OnePageCount: Integer;
  S: AnsiString;
begin
  if IsFromTradingDlg then
    OnePageCount := 18
  else
    OnePageCount := 10;
  // MainOutMessage('sItemName 1 :' + sItemName + ' nInt:' + IntToStr(nInt));
  nCount := 0;
  ClientItem := @InBuf;
  for I := 0 to m_GoodsList.Count - 1 do
  begin
    List20 := TList(m_GoodsList.Items[I]);
    if List20 = nil then
      Continue;
    if List20.Count <= 0 then
      Continue;
    UserItem := List20.Items[0];
    if UserItem = nil then
      Continue;
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (StdItem <> nil) and (CompareText(StdItem.Name, sItemName) = 0) then
    begin
      if (List20.Count - 1) < nInt then
      begin
        nInt := Max(0, List20.Count - OnePageCount);
      end;
      for II := nInt to List20.Count - 1 do
      begin
        if List20.Count <= 0 then
          Break;
        UserItem := List20.Items[II];
        if UserItem <> nil then
        begin
          UserItemToClientItem(UserItem, StdItem, ClientItem, g_Config.boShowNewValueFromBuyNpcItem, False);
          // 不用能DuraMax来放价格，这样会限制在65535及以内 chongchong 2014-05-22
          // ClientItem.DuraMax := nPrice;
          ClientItem.S.Expand1 := GetUserPrice(PlayObject, GetUserItemPrice(UserItem, False));
          Inc(nCount);
          Inc(ClientItem);
          if nCount >= OnePageCount then
            Break;
        end;
      end;
      Break;
    end;
  end;
  SetLength(S, nCount * SizeOf(TClientItem));
  if Length(S) > 0 then
  begin
    Move(InBuf[0], S[1], Length(S));
  end;
  PlayObject.SendMsg(Self, RM_SENDDETAILGOODSLIST, Integer(IsFromTradingDlg) { 是否为新的交易框 } , NativeInt(Self), nCount, nInt, S);
end;

procedure TMerchant.ClientQuerySellPrice(PlayObject: TPlayObject; UserItem: pTUserItem; IsFromTradingDlg: Boolean; WaitSetIndex: Integer);

  function sub_4A1C84(UserItem: pTUserItem): Boolean;
  var
    StdItem: pTStdItem;
  begin
    Result := True;
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (StdItem <> nil) and ((StdItem.StdMode = 25) or (StdItem.StdMode = 30)) then
    begin
      if UserItem.Dura < 4000 then
        Result := False;
    end;
  end;

var
  nC: Integer;
begin
  if IsFromTradingDlg then
  begin
    if (GetUserItemBindValue(UserItem, ubNoSell) and UserItem.boIsBind) or g_ItemRules.Get(UserItem.wIndex, 4) then
    begin
      PlayObject.SendMsg(Self, RM_SENDBUYPRICE, Integer(IsFromTradingDlg), 0, WaitSetIndex, 0, '');
      Exit;
    end;
    nC := GetSellItemPrice(GetUserItemPrice(UserItem, True));
    if (nC > 0) and (not bo574) and sub_4A1C84(UserItem) then
    begin
      PlayObject.SendMsg(Self, RM_SENDBUYPRICE, Integer(IsFromTradingDlg), nC, WaitSetIndex, 1, '');
    end
    else
    begin
      PlayObject.SendMsg(Self, RM_SENDBUYPRICE, Integer(IsFromTradingDlg), 0, WaitSetIndex, 0, '');
    end;
    Exit;
  end;
  nC := GetSellItemPrice(GetUserItemPrice(UserItem, True));
  PlayObject.SendMsg(Self, RM_SENDBUYPRICE, Integer(False), nC, WaitSetIndex, 0, '');
end;

function TMerchant.GetSellItemPrice(nPrice: Integer): Integer;
begin
  Result := Round(nPrice / 2.0);
end;

function TMerchant.ClientSellItem(PlayObject: TPlayObject; UserItem: pTUserItem; IsFromTradingDlg: Boolean): Boolean;

  function sub_4A1C84(UserItem: pTUserItem): Boolean;
  var
    StdItem: pTStdItem;
  begin
    Result := True;
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (StdItem <> nil) and ((StdItem.StdMode = 25) or (StdItem.StdMode = 30)) then
    begin
      if UserItem.Dura < 4000 then
        Result := False;
    end;
  end;

var
  nPrice: Integer;
  StdItem: pTStdItem;
begin
  Result := False;
  if g_OnlineMsgControl.boDisableSell then
    Exit;
  // 禁止卖
  if (GetUserItemBindValue(UserItem, ubNoSell) and UserItem.boIsBind) or g_ItemRules.Get(UserItem.wIndex, 4) then
  begin
    if not IsFromTradingDlg then
    begin
      PlayObject.SendMsg(Self, RM_USERSELLITEM_FAIL, 0, 0, 0, 0, '');
      MessageBox(PlayObject, g_sCanotUserSellItem);
    end;
    Exit;
  end;
  nPrice := GetSellItemPrice(GetUserItemPrice(UserItem, True));
  if (nPrice > 0) and (not bo574) and sub_4A1C84(UserItem) then
  begin
    if PlayObject.IncGold(nPrice) then
    begin
      {
        if m_boCastle or g_Config.boGetAllNpcTax then
        UserCastle.IncRateGold(nPrice);
      }
      if m_boCastle or g_Config.boGetAllNpcTax then
      begin
        if m_Castle <> nil then
        begin
          TUserCastle(m_Castle).IncRateGold(nPrice);
        end
        else if g_Config.boGetAllNpcTax then
        begin
          g_CastleManager.IncRateGold(g_Config.nUpgradeWeaponPrice);
        end;
      end;
      if not IsFromTradingDlg then
      begin
        PlayObject.SendMsg(Self, RM_USERSELLITEM_OK, 0, PlayObject.m_nGold, 0, 0, '');
      end;
      AddItemToGoodsList(UserItem);
      StdItem := UserEngine.GetStdItem(UserItem.wIndex);
      if StdItem.NeedIdentify = 1 then
      begin
        AddGameDataLog(LOG_ItemSell, LOG_GoldChange, PlayObject, StdItem.Name, UserItem.MakeIndex, m_sCharName, PlayObject.m_nGold, nPrice, 'NPC卖出 [' + sSTRING_GOLDNAME + ']');
      end;
      Result := True;
    end
    else if not IsFromTradingDlg then
      PlayObject.SendMsg(Self, RM_USERSELLITEM_FAIL, 0, -1, 0, 0, '');
  end
  else if not IsFromTradingDlg then
    PlayObject.SendMsg(Self, RM_USERSELLITEM_FAIL, 0, 0, 0, 0, '');
end;

function TMerchant.AddItemToGoodsList(UserItem: pTUserItem): Boolean;
var
  ItemList: TList;
  StdItem: pTStdItem;
begin
  Result := False;
  if UserItem.Dura <= 0 then
  begin
    StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    if (StdItem = nil) then
      Exit;
    // 叠加物品 dura=0不用管，药dura=0也不用管
    if (not CheckOverLapItem(StdItem)) and (not (StdItem.StdMode in [0, 1, 3])) then
      Exit;
  end;
  ItemList := GetRefillList(UserItem.wIndex);
  if ItemList = nil then
  begin
    ItemList := TList.Create;
    m_GoodsList.Add(ItemList);
  end;
  ItemList.Insert(0, UserItem);
  Result := True;
end;

procedure TMerchant.ClientMakeDrugItem(PlayObject: TPlayObject; sItemName: string);

  function sub_4A28FC(PlayObject: TPlayObject; sItemName: string): Boolean;
  var
    I, II, n1C, nDelCount: Integer;
    List10: TStringList;
    s20: string;
    DelItems: string;
    UserItem: pTUserItem;
  begin
    Result := False;
    List10 := GetMakeItemInfo(sItemName);
    if List10 = nil then
      Exit;
    Result := True;
    for I := 0 to List10.Count - 1 do
    begin
      s20 := List10.Strings[I];
      n1C := Integer(List10.Objects[I]);
      for II := 0 to PlayObject.m_ItemList.Count - 1 do
      begin
        if UserEngine.GetStdItemName(pTUserItem(PlayObject.m_ItemList.Items[II]).wIndex) = s20 then
          Dec(n1C);
      end;
      if n1C > 0 then
      begin
        Result := False;
        Break;
      end;
    end; // for
    if Result then
    begin
      DelItems := '';
      nDelCount := 0;
      for I := 0 to List10.Count - 1 do
      begin
        s20 := List10.Strings[I];
        n1C := Integer(List10.Objects[I]);
        for II := PlayObject.m_ItemList.Count - 1 downto 0 do
        begin
          if n1C <= 0 then
            Break;
          if PlayObject.m_ItemList.Count <= 0 then
            Break;
          UserItem := PlayObject.m_ItemList.Items[II];
          if UserEngine.GetStdItemName(UserItem.wIndex) = s20 then
          begin
            DelItems := DelItems + Format('%s/%d/', [s20, UserItem.MakeIndex]);
            PlayObject.m_ItemList.Delete(II);
            Dispose(UserItem);
            Dec(n1C);
            Inc(nDelCount);
          end;
        end;
      end;
      if DelItems <> '' then
        PlayObject.SendMsg(Self, RM_SENDDELITEMLIST, 0, nDelCount, 0, 0, DelItems);
    end;
  end;

var
  I: Integer;
  List1C: TList;
  MakeItem, UserItem: pTUserItem;
  StdItem: pTStdItem;
  n14: Integer;
begin
  n14 := 1;
  for I := 0 to m_GoodsList.Count - 1 do
  begin
    List1C := TList(m_GoodsList.Items[I]);
    if List1C = nil then
      Continue;
    if List1C.Count <= 0 then
      Continue;
    MakeItem := List1C.Items[0];
    if MakeItem = nil then
      Continue;
    StdItem := UserEngine.GetStdItem(MakeItem.wIndex);
    if (StdItem <> nil) and (StdItem.Name = sItemName) then
    begin
      if PlayObject.m_nGold >= g_Config.nMakeDurgPrice then
      begin
        if sub_4A28FC(PlayObject, sItemName) then
        begin
          New(UserItem);
          UserEngine.CopyToUserItemFromName(sItemName, UserItem);
          UserItem.ItemFrom.ItemForm := ifShopBuy;
          UserItem.ItemFrom.sMakerName := PlayObject.m_sCharName;
          UserItem.ItemFrom.DateTime := Now();
          if PlayObject.AddItemToBag(UserItem) then
          begin
            if (StdItem.Need in [103, 104]) and (UserItem.boStartTime) then
            begin
              UserItem.nLimitTime := StdItem.NeedLevel;
              UserItem.boStartTime := True;
              if StdItem.Need = 103 then
                PlayObject.SysMsg(Format('您的限时物品[%s]开始计时，有效时间%d分钟。', [StdItem.Name, StdItem.NeedLevel]), c_Red, t_System)
              else
              begin
                PlayObject.SysMsg(Format('您的限时物品[%s]开始计时，到期时间%s', [StdItem.Name, GetIncMinuteTime(UserItem.ItemFrom.DateTime, StdItem.NeedLevel)]), c_Red, t_System)
              end;
            end;
            Dec(PlayObject.m_nGold, g_Config.nMakeDurgPrice);
            PlayObject.SendAddItem(UserItem);
            StdItem := UserEngine.GetStdItem(UserItem.wIndex);
            if StdItem.NeedIdentify = 1 then
            begin
              AddGameDataLog(LOG_ItemRefining, LOG_GoldChange, PlayObject, StdItem.Name, UserItem.MakeIndex, m_sCharName, PlayObject.m_nGold, -g_Config.nMakeDurgPrice, 'NPC炼药 [' + sSTRING_GOLDNAME + ']');
            end;
            n14 := 0;
            Break;
          end
          else
          begin
            Dispose(UserItem);
            n14 := 2;
          end;
        end
        else
          n14 := 4;
      end
      else
        n14 := 3;
    end;
  end; // for
  if n14 = 0 then
  begin
    PlayObject.SendMsg(Self, RM_MAKEDRUG_SUCCESS, 0, PlayObject.m_nGold, 0, 0, '');
  end
  else
  begin
    PlayObject.SendMsg(Self, RM_MAKEDRUG_FAIL, 0, n14, 0, 0, '');
  end;
end;

procedure TMerchant.ClientQueryRepairCost(PlayObject: TPlayObject; UserItem: pTUserItem);
var
  nPrice, nRepairPrice: Integer;
begin
  nPrice := GetUserPrice(PlayObject, GetUserItemPrice(UserItem, False));
  if (nPrice > 0) and (UserItem.DuraMax > UserItem.Dura) then
  begin
    if UserItem.DuraMax > 0 then
    begin
      nRepairPrice := Round(nPrice div 3 / UserItem.DuraMax * (UserItem.DuraMax - UserItem.Dura));
    end
    else
    begin
      nRepairPrice := nPrice;
    end;
    if SameText(PlayObject.m_sScriptLable, sNF_SuperRepair) then
    begin
      if m_boS_repair then
        nRepairPrice := nRepairPrice * g_Config.nSuperRepairPriceRate { 3 }
      else
        nRepairPrice := -1;
    end
    else
    begin
      if not m_boRepair then
        nRepairPrice := -1;
    end;
    PlayObject.SendMsg(Self, RM_SENDREPAIRCOST, 0, nRepairPrice, 0, 0, '');
  end
  else
  begin
    PlayObject.SendMsg(Self, RM_SENDREPAIRCOST, 0, -1, 0, 0, '');
  end;
end;

function TMerchant.ClientRepairItem(PlayObject: TPlayObject; UserItem: pTUserItem): Boolean;
var
  nPrice, nRepairPrice: Integer;
  StdItem: pTStdItem;
  boCanRepair: Boolean;
  OldDura: Word;
begin
  Result := False;
  if g_OnlineMsgControl.boDisableRepair then
    Exit;
  boCanRepair := True;
  if SameText(PlayObject.m_sScriptLable, sNF_SuperRepair) and not m_boS_repair then
  begin
    boCanRepair := False;
  end;
  if (not SameText(PlayObject.m_sScriptLable, sNF_SuperRepair)) and not m_boRepair then
  begin
    boCanRepair := False;
  end;
  if PlayObject.m_sScriptLable = '@fail_s_repair' then
  begin
    SendMsgToUser(PlayObject, 'Sorry, I cant special repair this item\ \ \<Main/@main>');
    PlayObject.SendMsg(Self, RM_USERREPAIRITEM_FAIL, 0, 0, 0, 0, '');
    Exit;
  end;
  nPrice := GetUserPrice(PlayObject, GetUserItemPrice(UserItem, False));
  if SameText(PlayObject.m_sScriptLable, sNF_SuperRepair) then
  begin
    nPrice := nPrice * g_Config.nSuperRepairPriceRate { 3 };
  end;
  StdItem := UserEngine.GetStdItem(UserItem.wIndex);
  if StdItem <> nil then
  begin
    if (GetUserItemBindValue(UserItem, ubNoRepair) and UserItem.boIsBind) or g_ItemRules.Get(UserItem.wIndex, 3) then
    begin
      PlayObject.SendMsg(Self, RM_USERREPAIRITEM_FAIL, 0, 0, 0, 0, '');
      MessageBox(PlayObject, g_sCanotUserRepairItem);
      Exit;
    end;
    // 禁止叠加物品修理 chongchong 2014-09-26
    if CheckOverLapItem(StdItem) then
    begin
      PlayObject.SendMsg(Self, RM_USERREPAIRITEM_FAIL, 0, 0, 0, 0, '');
      MessageBox(PlayObject, g_sCanotUserRepairItem);
      Exit;
    end;
    if boCanRepair and (nPrice > 0) and (UserItem.DuraMax > UserItem.Dura) and (StdItem.StdMode <> 43) then
    begin
      if UserItem.DuraMax > 0 then
      begin
        nRepairPrice := Round(nPrice div 3 / UserItem.DuraMax * (UserItem.DuraMax - UserItem.Dura));
      end
      else
      begin
        nRepairPrice := nPrice;
      end;
      if PlayObject.DecGold(nRepairPrice) then
      begin
        AddGameDataLog(LOG_GoldChange, LOG_ActionNone, PlayObject, sSTRING_GOLDNAME, 0, m_sCharName, PlayObject.m_nGold, -nRepairPrice, '修复');
        // if m_boCastle or g_Config.boGetAllNpcTax then UserCastle.IncRateGold(nRepairPrice);
        if m_boCastle or g_Config.boGetAllNpcTax then
        begin
          if m_Castle <> nil then
          begin
            TUserCastle(m_Castle).IncRateGold(nRepairPrice);
          end
          else if g_Config.boGetAllNpcTax then
          begin
            g_CastleManager.IncRateGold(g_Config.nUpgradeWeaponPrice);
          end;
        end;
        if SameText(PlayObject.m_sScriptLable, sNF_SuperRepair) then
        begin
          UserItem.Dura := UserItem.DuraMax;
          PlayObject.SendMsg(Self, RM_USERREPAIRITEM_OK, 0, PlayObject.m_nGold, UserItem.Dura, UserItem.DuraMax, '');
          GotoLable(PlayObject, sNF_SuperRepairOK, False);
        end
        else
        begin
          OldDura := UserItem.Dura;
          Dec(UserItem.DuraMax, (UserItem.DuraMax - UserItem.Dura) div g_Config.nRepairItemDecDura { 30 } );
          UserItem.Dura := UserItem.DuraMax;
          PlayObject.SendMsg(Self, RM_USERREPAIRITEM_OK, 0, PlayObject.m_nGold, UserItem.Dura, UserItem.DuraMax, '');
          GotoLable(PlayObject, sNF_RepairOK, False);
          if (StdItem.NeedIdentify = 1) then
          begin
            AddGameDataLog(LOG_ItemUpdate, LOG_ActionNone, PlayObject, StdItem.Name, UserItem.MakeIndex, m_sCharName, UserItem.Dura, OldDura, '持久改变');
          end;
        end;
        Result := True;
      end
      else
        PlayObject.SendMsg(Self, RM_USERREPAIRITEM_FAIL, 0, 0, 0, 0, ''); // 004A2238
    end
    else
      PlayObject.SendMsg(Self, RM_USERREPAIRITEM_FAIL, 0, 0, 0, 0, ''); // 004A2253
  end;
end;

procedure TMerchant.ClearScript;
begin
  m_boBuy := False;
  m_boSell := False;
  m_boMakeDrug := False;
  m_boPrices := False;
  m_boStorage := False;
  m_boGetback := False;
  m_boBigStorage := False;
  m_boBigGetBack := False;
  m_boGetNextPage := False;
  m_boGetPreviousPage := False;
  m_boUpgradenow := False;
  m_boGetBackupgnow := False;
  m_boRepair := False;
  m_boS_repair := False;
  m_boGetMarry := False;
  m_boGetMaster := False;
  m_boUseItemName := False;
  m_boCreateHeroName := False;
  m_boGetSellGold := False;
  m_boSellOff := False;
  m_boBuyOff := False;
  m_boofflinemsg := False;
  m_boDealGold := False;
  m_boPleaseDrink := False;
  m_boMakeWine := False;
  m_boBuHero := False;
  m_boReclaimItem := False;
  inherited;
end;

procedure TMerchant.LoadUpgradeList;
var
  I: Integer;
begin
  for I := 0 to m_UpgradeWeaponList.Count - 1 do
  begin
    Dispose(pTUpgradeInfo(m_UpgradeWeaponList.Items[I]));
  end; // for
  m_UpgradeWeaponList.Clear;
  try
    // FrmDB.LoadUpgradeWeaponRecord(m_sCharName,m_UpgradeWeaponList);
    FrmDB.LoadUpgradeWeaponRecord(m_sScript + '-' + m_sMapName, m_UpgradeWeaponList);
  except
    MainOutMessage('Failure in loading upgradinglist - ' + m_sCharName);
  end;
end;

(*
  procedure TMerchant.GetMarry(PlayObject: TPlayObject; sDearName: string);
  var
  MarryHuman: TPlayObject;
  begin
  MarryHuman := UserEngine.GetPlayObject(sDearName);
  if (MarryHuman <> nil) and
  (MarryHuman.m_PEnvir = PlayObject.m_PEnvir) and
  (abs(PlayObject.m_nCurrX - MarryHuman.m_nCurrX) < 5) and
  (abs(PlayObject.m_nCurrY - MarryHuman.m_nCurrY) < 5) then
  begin
  SendMsgToUser(MarryHuman, PlayObject.m_sCharName + ' 向你求婚，你是否愿意嫁给他为妻？');
  end
  else
  begin
  Self.SendMsgToUser(PlayObject, sDearName + ' 没有在你身边，你的请求无效！');
  end;
  end;
  procedure TMerchant.GetMaster(PlayObject: TPlayObject; sMasterName: string);
  begin
  end;
*)
procedure TMerchant.SendCustemMsg(PlayObject: TPlayObject; sMsg: string);
begin
  inherited;
end;

// 清除临时文件，包括交易库存，价格表
procedure TMerchant.ClearData;
var
  I, II: Integer;
  UserItem: pTUserItem;
  ItemList: TList;
  ItemPrice: pTItemPrice;
resourcestring
  sExceptionMsg = '[Exception] TMerchant.ClearData';
begin
  try
    for I := 0 to m_GoodsList.Count - 1 do
    begin
      ItemList := TList(m_GoodsList.Items[I]);
      if ItemList = nil then
        Continue;
      for II := 0 to ItemList.Count - 1 do
      begin
        UserItem := ItemList.Items[II];
        if UserItem <> nil then
          Dispose(UserItem);
      end;
      ItemList.Free;
    end;
    m_GoodsList.Clear;
    for I := 0 to m_ItemPriceList.Count - 1 do
    begin
      ItemPrice := m_ItemPriceList.Items[I];
      if ItemPrice <> nil then
        Dispose(ItemPrice);
    end;
    m_ItemPriceList.Clear;
    SaveNPCData();
  except
    on E: Exception do
    begin
      MainOutMessage(sExceptionMsg);
      MainOutMessage(E.Message);
    end;
  end;
end;

procedure TMerchant.ChangeUseItemName(PlayObject: TPlayObject; sLabel, sItemName: string);
var
  sWhere: string;
  btWhere: Byte;
  UserItem: pTUserItem;
  sMsg: string;
begin
  { 装备改名控制 chongchong 2013-07-24 }
  if not PlayObject.m_boChangeItemNameFlag then
    Exit;
  if (Length(sItemName) >= 15) or GetNameInFilterList(sItemName) then
  begin
    GotoLable(PlayObject, '@UseItemName_Fail', False);
    Exit;
  end;
  PlayObject.m_boChangeItemNameFlag := False;
  sWhere := Copy(sLabel, Length(sNF_UseItemName) + 1, Length(sLabel) - Length(sNF_UseItemName));
  btWhere := StrToIntDef(sWhere, -1);
  if btWhere in [Low(THumanUseItems)..High(THumanUseItems)] then
  begin
    UserItem := @PlayObject.m_UseItems[btWhere];
    if UserItem.wIndex = 0 then
    begin
      sMsg := Format(g_sYourUseItemIsNul, [GetUseItemName(btWhere)]);
      PlayObject.SendMsg(Self, RM_MENU_OK, 0, NativeInt(Self), 0, 0, sMsg);
      Exit;
    end;
    if sItemName <> '' then
    begin
      if g_Config.boChangeUseItemNameByPlayName then
      begin
        UserItem.Name := PlayObject.m_sCharName + '的' + sItemName;
        UserItem.btValue[13] := 1;
      end
      else
      begin
        UserItem.Name := g_Config.sChangeUseItemName + sItemName;
        UserItem.btValue[13] := 1;
      end;
    end
    else
    begin
      UserItem.Name := '';
      UserItem.btValue[13] := 0;
    end;
    PlayObject.SendMsg(PlayObject, RM_SENDUSEITEMS, 0, 0, 0, 0, '');
    PlayObject.SendMsg(Self, RM_MENU_OK, 0, NativeInt(Self), 0, 0, '');
    GotoLable(PlayObject, '@UseItemName_OK', False);
    Exit;
  end;
end;

{ TTrainer }
constructor TTrainer.Create; // 004A385C
begin
  inherited;
  m_dw568 := MyGetTickCount();
  n56C := 0;
  n570 := 0;
end;

destructor TTrainer.Destroy;
begin
  inherited;
end;

function TTrainer.Operate(ProcessMsg: pTProcessMessage): Boolean; // 004A38C4
begin
  Result := False;
  if (ProcessMsg.wIdent = RM_STRUCK) or (ProcessMsg.wIdent = RM_MAGSTRUCK) then
  begin
    // if (ProcessMsg.wIdent = RM_10101) or (ProcessMsg.wIdent = RM_MAGSTRUCK) then begin
    if (TObject(ProcessMsg.BaseObject) = Self) { and (ProcessMsg.nParam3 <> 0) } then
    begin
      Inc(n56C, ProcessMsg.wParam);
      m_dw568 := MyGetTickCount();
      Inc(n570);
      ProcessSayMsg('破坏力为 ' + IntToStr(ProcessMsg.wParam) + ',平均值为 ' + IntToStr(n56C div n570));
    end;
  end;
  if ProcessMsg.wIdent = RM_MAGSTRUCK then
    Result := inherited Operate(ProcessMsg);
end;

procedure TTrainer.Run;
begin
  m_Abil.HP := m_Abil.MaxHP;
  m_WAbil.HP := m_WAbil.MaxHP;
  if n570 > 0 then
  begin
    if (MyGetTickCount - m_dw568) > 3 * 1000 then
    begin
      ProcessSayMsg('总破坏力为  ' + IntToStr(n56C) + ',平均值为 ' + IntToStr(n56C div n570));
      n570 := 0;
      n56C := 0;
    end;
  end;
  inherited;
end;

{ TNormNpc }
procedure TNormNpc.ClearScript;
var
  III, IIII: Integer;
  I, II: Integer;
  Script: pTScript;
  SayingRecord: pTSayingRecord;
  SayingProcedure: pTSayingProcedure;
  QuestConditionInfo: pTQuestConditionInfo;
  QuestActionInfo: pTQuestActionInfo;
begin
  for I := 0 to m_ScriptList.Count - 1 do
  begin
    Script := m_ScriptList.Items[I];
    for II := 0 to Script.RecordList.Count - 1 do
    begin
      SayingRecord := Script.RecordList.Items[II];
      for III := 0 to SayingRecord.ProcedureList.Count - 1 do
      begin
        SayingProcedure := SayingRecord.ProcedureList.Items[III];
        for IIII := 0 to SayingProcedure.ConditionList.Count - 1 do
        begin
          QuestConditionInfo := pTQuestConditionInfo(SayingProcedure.ConditionList.Items[IIII]);
          Dispose(QuestConditionInfo);
        end;
        for IIII := 0 to SayingProcedure.ActionList.Count - 1 do
        begin
          QuestActionInfo := pTQuestActionInfo(SayingProcedure.ActionList.Items[IIII]);
          Dispose(QuestActionInfo);
        end;
        for IIII := 0 to SayingProcedure.ElseActionList.Count - 1 do
        begin
          QuestActionInfo := pTQuestActionInfo(SayingProcedure.ElseActionList.Items[IIII]);
          Dispose(QuestActionInfo);
        end;
        SayingProcedure.ConditionList.Free;
        SayingProcedure.ActionList.Free;
        SayingProcedure.ElseActionList.Free;
        Dispose(SayingProcedure);
      end; // for
      SayingRecord.ProcedureList.Free;
      Dispose(SayingRecord);
    end; // for
    Script.RecordList.Free;
    Dispose(Script);
  end; // for
  m_ScriptList.Clear;
end;

procedure TNormNpc.Click(PlayObject: TPlayObject); // 0049EC18
begin
  PlayObject.m_nScriptGotoCount := 0;
  PlayObject.m_sScriptGoBackLable := '';
  PlayObject.m_sScriptCurrLable := '';
  PlayObject.m_sRandomString := '';
  PlayObject.m_sInputData := '';
  PlayObject.m_sNpcSelectItemName := '';

  GotoLable(PlayObject, '@main', False);
end;

procedure TNormNpc.GetVarValue(PlayObject: TPlayObject; sData: string; var nValue: Integer);
var
  sValue: string;
  IsBreakParseVar: Boolean;
begin
  if (Length(sData) > 3) and (Pos('<', sData) > 0) and (Pos('$', sData) > 0) and (Pos('>', sData) > 0) then
  begin
    sValue := GetLineVariableText(PlayObject, sData, IsBreakParseVar);
    nValue := StrToInt64Def(sValue, nValue);
  end
  else
    nValue := 0;
end;

procedure TNormNpc.GetVarValue(PlayObject: TPlayObject; sData: string; var sValue: string);
var
  IsBreakParseVar: Boolean;
begin
  if (Length(sData) > 3) and (Pos('<', sData) > 0) and (Pos('$', sData) > 0) and (Pos('>', sData) > 0) then
    sValue := GetLineVariableText(PlayObject, sData, IsBreakParseVar)
  else
    sValue := sData;
end;

procedure TNormNpc.GetVarValue(PlayObject: TPlayObject; sData: string; var sValue: string; var nValue: Integer; var IsBreakParseVar: Boolean);
begin
  IsBreakParseVar := False;
  if (Length(sData) > 3) and (Pos('<', sData) > 0) and (Pos('$', sData) > 0) and (Pos('>', sData) > 0) then
  begin
    sValue := GetLineVariableText(PlayObject, sData, IsBreakParseVar);
    nValue := StrToInt64Def(sValue, 0); // chongchong 2016-08-31
  end
  else
  begin
    sValue := sData;
    nValue := StrToInt64Def(sValue, 0);
  end;
end;

procedure TNormNpc.GetVarValue(PlayObject: TPlayObject; sData: string; var sVar, sValue: string; var nValue: Integer);
var
  IsBreakParseVar: Boolean;
begin
  sVar := sData;
  if GetValNameValue(PlayObject, sData, sValue, nValue) then
    Exit;
  if GetDynamicValue(PlayObject, sData, sValue, nValue) then
    Exit;
  sValue := GetLineVariableText(PlayObject, sData, IsBreakParseVar);
  nValue := StrToInt64Def(sValue, 0);
end;

function TNormNpc.SetVarValue(PlayObject: TPlayObject; const sData, sValue: string; const nValue: Integer): Boolean;
begin
  Result := False;
  if sData = '' then
    Exit;
  if SetValNameValue(PlayObject, sData, sValue, nValue) then
  begin
    Result := True;
    Exit;
  end;
  if SetDynamicValue(PlayObject, sData, sValue, nValue) then
  begin
    Result := True;
    Exit;
  end;
end;

function TNormNpc.GetDynamicValue(PlayObject: TPlayObject; sVar: string; var sValue: string; var nValue: Integer): Boolean;
var
  I: Integer;
  DynamicVar: pTDynamicVar;
  DynamicVarList: TList;
  sVarName, sVarType, sData, sName: string;
begin
  Result := False;
  sValue := sVar;
  nValue := 0;
  sVarName := '';
  sVarType := '';
  sName := '';
  sData := sVar;
  if (Length(sData) > 2) and (sData[1] = '<') and (sData[2] = '$') and (sData[Length(sData)] = '>') then
  begin
    sData := ArrestStringEx(sData, '<', '>', sName);
    if CompareLStr(sName, '$HUMAN(', Length('$HUMAN(')) then
    begin
      sData := ArrestStringEx(sName, '(', ')', sVarName);
      sVarType := 'HUMAN';
    end
    else if CompareLStr(sName, '$GUILD(', Length('$GUILD(')) then
    begin
      sData := ArrestStringEx(sName, '(', ')', sVarName);
      sVarType := 'GUILD';
    end
    else if CompareLStr(sName, '$GLOBAL(', Length('$GLOBAL(')) then
    begin
      sData := ArrestStringEx(sName, '(', ')', sVarName);
      sVarType := 'GLOBAL';
    end;
    if (sVarName = '') or (sVarType = '') then
      Exit;
    DynamicVarList := GetDynamicVarList(PlayObject, sVarType, sName);
    if DynamicVarList = nil then
    begin
      Exit;
    end;
    for I := 0 to DynamicVarList.Count - 1 do
    begin
      DynamicVar := DynamicVarList.Items[I];
      if CompareText(DynamicVar.sName, sVarName) = 0 then
      begin
        case DynamicVar.VarType of
          vInteger:
            begin
              nValue := DynamicVar.nInternet;
              sValue := IntToStr(nValue);
              Result := True;
            end;
          vString:
            begin
              sValue := DynamicVar.sString;
              nValue := StrToInt64Def(sValue, nValue);
              Result := True;
            end;
        end;
        Break;
      end;
    end;
  end;
end;

function TNormNpc.SetDynamicValue(PlayObject: TPlayObject; sVar: string; sValue: string; nValue: Integer): Boolean;
var
  I: Integer;
  DynamicVar: pTDynamicVar;
  DynamicVarList: TList;
  sVarName, sVarType, sData, sName: string;
begin
  Result := False;
  sVarName := '';
  sVarType := '';
  sName := '';
  sData := sVar;
  if (Length(sData) > 2) and (sData[1] = '<') and (sData[2] = '$') and (sData[Length(sData)] = '>') then
  begin
    sData := ArrestStringEx(sData, '<', '>', sName);
    if CompareLStr(sName, '$HUMAN(', Length('$HUMAN(')) then
    begin
      sData := ArrestStringEx(sName, '(', ')', sVarName);
      sVarType := 'HUMAN';
    end
    else if CompareLStr(sName, '$GUILD(', Length('$GUILD(')) then
    begin
      sData := ArrestStringEx(sName, '(', ')', sVarName);
      sVarType := 'GUILD';
    end
    else if CompareLStr(sName, '$GLOBAL(', Length('$GLOBAL(')) then
    begin
      sData := ArrestStringEx(sName, '(', ')', sVarName);
      sVarType := 'GLOBAL';
    end;
    if (sVarName = '') or (sVarType = '') then
      Exit;
    DynamicVarList := GetDynamicVarList(PlayObject, sVarType, sName);
    if DynamicVarList = nil then
    begin
      Exit;
    end;
    for I := 0 to DynamicVarList.Count - 1 do
    begin
      DynamicVar := DynamicVarList.Items[I];
      if CompareText(DynamicVar.sName, sVarName) = 0 then
      begin
        case DynamicVar.VarType of
          vInteger:
            begin
              DynamicVar.nInternet := nValue;
            end;
          vString:
            begin
              DynamicVar.sString := sValue;
            end;
        end;
        Result := True;
        Break;
      end;
    end;
  end;
end;

(*
  0..99:                   0..999:                P
  100..199:                1000..1999:            D
  200..299:                2000..2999:            M
  300..399:                3000..3999:            N
  400..499:                4000..4999:            I
  500..999:                5000..5999             G
  1000..1499:              6000..6999:            A
  1500..1699:              7000..7999:            S
*)
function SetBoxItemValue(sVar: string; PlayObject: TPlayObject; sValue: string; nValue: Integer): Boolean;
var
  sIndex: string;
  I, nIndex, nMakeIndex: Integer;
  UseItem: pTUserItem;
  sSubName: string;
  StdItem: pTStdItem;
  NewStdItem: TStdItem;
  nAddValue, nNewValue: Integer;
  nIndexLen: Integer;
begin
  Result := False;
  if Length(sVar) <= 14 then
    Exit;
  if (sVar[1] <> '<') or (sVar[Length(sVar)] <> '>') then
    Exit;
  if (sVar[10] <> '[') then
    Exit;
  nIndexLen := 0;
  if (sVar[12] = ']') and (sVar[13] = '.') then
    nIndexLen := 1
  else if (sVar[13] = ']') and (sVar[14] = '.') then
    nIndexLen := 2;
  if nIndexLen = 0 then
    Exit;
  sIndex := Copy(sVar, 11, nIndexLen);
  nIndex := StrToIntDef(sIndex, -1);
  if not (nIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)]) then
    Exit;
  nMakeIndex := PlayObject.m_ItemBoxItems[nIndex];
  if nMakeIndex = 0 then
    Exit;
  UseItem := nil;
  for I := 0 to PlayObject.m_ItemList.Count - 1 do
  begin
    if pTUserItem(PlayObject.m_ItemList[I]).MakeIndex = nMakeIndex then
    begin
      UseItem := PlayObject.m_ItemList[I];
      Break;
    end;
  end;
  if UseItem = nil then
    Exit;
  StdItem := UserEngine.GetStdItem(UseItem.wIndex);
  if StdItem = nil then
    Exit;
  NewStdItem := StdItem^;
  ItemUnit.GetItemAddValue(UseItem, NewStdItem);
  sSubName := Copy(sVar, 13 + nIndexLen, MaxInt);
  if Length(sSubName) = 0 then
    Exit;
  sSubName := Copy(sSubName, 1, Length(sSubName) - 1);
  {
    if SameText(sSubName, 'name') then
    begin
    StdItem.Name := sValue;
    Result := True;
    end
    else }
  if SameText(sSubName, 'dura') then
  begin
    UseItem.Dura := nValue;
    Result := True;
  end
  else if SameText(sSubName, 'duramax') then
  begin
    UseItem.DuraMax := nValue;
    Result := True;
  end
  {
    else if SameText(sSubName, 'makeindex') then
    begin
    Ret := IntToStr(UseItem.MakeIndex);
    Result := True;
    end
    else if SameText(sSubName, 'stdmode') then
    begin
    Ret := IntToStr(StdItem.StdMode);
    Result := True;
    end
    else if SameText(sSubName, 'shape') then
    begin
    Ret := IntToStr(StdItem.Shape);
    Result := True;
    end
    else if SameText(sSubName, 'looks') then
    begin
    Ret := IntToStr(StdItem.Looks);
    Result := True;
    end
  }
  else if SameText(sSubName, 'color') then
  begin
    UseItem.btColor := nValue;
    Result := True;
  end
  else if SameText(sSubName, 'upgradecount') then
  begin
    UseItem.btUpgradeCount := nValue;
    Result := True;
  end
  {
    else if SameText(sSubName, 'hp') then
    begin
    StdItem.HP := nValue;
    Result := True;
    end
    else if SameText(sSubName, 'mp') then
    begin
    StdItem.HP := nValue;
    Result := True;
    end
  }
  {
    else if SameText(sSubName, 'lac') then
    begin
    Result := True;
    end
  }
  else if SameText(sSubName, 'hac') then
  begin
    case NewStdItem.StdMode of
      5, 6, 68, 69:
        begin
          nIndex := 5;
          nAddValue := nValue - NewStdItem.AC2;
          nNewValue := UseItem.btValue[nIndex] + nAddValue;
          if nNewValue < 0 then
            nNewValue := 0
          else if nNewValue > 255 then
            nNewValue := 255;
          UseItem.btValue[nIndex] := nNewValue;
          Result := True;
        end;
      10, 11, 15, 12 { 盾牌 } , 28 { 马牌 } , 16 { 斗笠 } , 19, 20, 21, 22, 23, 24, 26, 29, 30, 51, 52, 53, 54, 62, 63, 64, 65, 66, 67:
        begin
          nIndex := 0;
          nAddValue := nValue - NewStdItem.AC2;
          nNewValue := UseItem.btValue[nIndex] + nAddValue;
          if nNewValue < 0 then
            nNewValue := 0
          else if nNewValue > 255 then
            nNewValue := 255;
          UseItem.btValue[nIndex] := nNewValue;
          Result := True;
        end;
    end
  end
  {
    else if SameText(sSubName, 'lmac') then
    begin
    Result := True;
    end
  }
  else if SameText(sSubName, 'hmac') then
  begin
    case NewStdItem.StdMode of
      5, 6, 68, 69:
        begin
          nIndex := 6;
          nAddValue := nValue - NewStdItem.MAC2;
          nNewValue := UseItem.btValue[nIndex] + nAddValue;
          if nNewValue < 0 then
            nNewValue := 0
          else if nNewValue > 255 then
            nNewValue := 255;
          UseItem.btValue[nIndex] := nNewValue;
          Result := True;
        end;
      10, 11, 15, 12 { 盾牌 } , 28 { 马牌 } , 16 { 斗笠 } , 19, 20, 21, 22, 23, 24, 26, 29, 30, 51, 52, 53, 54, 62, 63, 64, 65, 66, 67:
        begin
          nIndex := 1;
          nAddValue := nValue - NewStdItem.MAC2;
          nNewValue := UseItem.btValue[nIndex] + nAddValue;
          if nNewValue < 0 then
            nNewValue := 0
          else if nNewValue > 255 then
            nNewValue := 255;
          UseItem.btValue[nIndex] := nNewValue;
          Result := True;
        end;
    end
  end
  {
    else if SameText(sSubName, 'ldc') then
    begin
    Result := True;
    end
  }
  else if SameText(sSubName, 'hdc') then
  begin
    case NewStdItem.StdMode of
      5, 6, 68, 69:
        begin
          nIndex := 0;
          nAddValue := nValue - NewStdItem.DC2;
          nNewValue := UseItem.btValue[nIndex] + nAddValue;
          if nNewValue < 0 then
            nNewValue := 0
          else if nNewValue > 255 then
            nNewValue := 255;
          UseItem.btValue[nIndex] := nNewValue;
          Result := True;
        end;
      10, 11, 15, 12 { 盾牌 } , 28 { 马牌 } , 16 { 斗笠 } , 19, 20, 21, 22, 23, 24, 26, 29, 30, 51, 52, 53, 54, 62, 63, 64, 65, 66, 67:
        begin
          nIndex := 2;
          nAddValue := nValue - NewStdItem.DC2;
          nNewValue := UseItem.btValue[nIndex] + nAddValue;
          if nNewValue < 0 then
            nNewValue := 0
          else if nNewValue > 255 then
            nNewValue := 255;
          UseItem.btValue[nIndex] := nNewValue;
          Result := True;
        end;
    end
  end
  {
    else if SameText(sSubName, 'lmc') then
    begin
    Result := True;
    end
  }
  else if SameText(sSubName, 'hmc') then
  begin
    case NewStdItem.StdMode of
      5, 6, 68, 69:
        begin
          nIndex := 1;
          nAddValue := nValue - NewStdItem.MC2;
          nNewValue := UseItem.btValue[nIndex] + nAddValue;
          if nNewValue < 0 then
            nNewValue := 0
          else if nNewValue > 255 then
            nNewValue := 255;
          UseItem.btValue[nIndex] := nNewValue;
          Result := True;
        end;
      10, 11, 15, 12 { 盾牌 } , 28 { 马牌 } , 16 { 斗笠 } , 19, 20, 21, 22, 23, 24, 26, 29, 30, 51, 52, 53, 54, 62, 63, 64, 65, 66, 67:
        begin
          nIndex := 3;
          nAddValue := nValue - NewStdItem.MC2;
          nNewValue := UseItem.btValue[nIndex] + nAddValue;
          if nNewValue < 0 then
            nNewValue := 0
          else if nNewValue > 255 then
            nNewValue := 255;
          UseItem.btValue[nIndex] := nNewValue;
          Result := True;
        end;
    end
  end
  {
    else if SameText(sSubName, 'lsc') then
    begin
    Result := True;
    end
  }
  else if SameText(sSubName, 'hsc') then
  begin
    case NewStdItem.StdMode of
      5, 6, 68, 69:
        begin
          nIndex := 2;
          nAddValue := nValue - NewStdItem.SC2;
          nNewValue := UseItem.btValue[nIndex] + nAddValue;
          if nNewValue < 0 then
            nNewValue := 0
          else if nNewValue > 255 then
            nNewValue := 255;
          UseItem.btValue[nIndex] := nNewValue;
          Result := True;
        end;
      10, 11, 15, 12 { 盾牌 } , 28 { 马牌 } , 16 { 斗笠 } , 19, 20, 21, 22, 23, 24, 26, 29, 30, 51, 52, 53, 54, 62, 63, 64, 65, 66, 67:
        begin
          nIndex := 4;
          nAddValue := nValue - NewStdItem.SC2;
          nNewValue := UseItem.btValue[nIndex] + nAddValue;
          if nNewValue < 0 then
            nNewValue := 0
          else if nNewValue > 255 then
            nNewValue := 255;
          UseItem.btValue[nIndex] := nNewValue;
          Result := True;
        end;
    end;
  end;
end;

function TNormNpc.SetValNameValue(PlayObject: TPlayObject; sVar: string; sValue: string; nValue: Integer): Boolean;
var
  n01: Integer;
  sData, sName: string;
  sParam1: string;
  nParam1, CombatPower: Integer;
  IsBreakParseVar: Boolean;
  VarRecord: PCombatPowerVarRecord;
  aryLValue: TArray<string>;
  arySIndex: string;
  aryNIndex: Integer;
  oldValue: string;
begin
  Result := False;
  if sVar = '' then
    Exit;
  sData := sVar;
  sName := sVar;
  if (Length(sData) > 8) and CompareLStr(sData, '<$STR(', Length('<$STR(')) and (sData[Length(sData) - 1] = ')') and (sData[Length(sData)] = '>') then
  begin
    sData := ArrestStringEx(sData, '(', ')', sName);
    if sName = '' then
      Exit;
  end;
  if SameText(Copy(sName, 1, 10), '<$BOXITEM[') then
  begin
    Result := SetBoxItemValue(sName, PlayObject, sValue, nValue);
    Exit;
  end;
  n01 := GetValNameNo(sName);
  if n01 >= 0 then
  begin
    case n01 of
      0..999: // P
        begin
          CombatPower := 0;
          if g_Config.boOpenCombatPowerVarCalc then
          begin
            g_CombatPowerVarMgr.Lock;
            try
              VarRecord := g_CombatPowerVarMgr.GetValueRecord(sName);
              if VarRecord <> nil then
              begin
                if PlayObject.m_btJob = 1 then
                  CombatPower := VarRecord.Value1
                else if PlayObject.m_btJob = 2 then
                  CombatPower := VarRecord.Value2
                else
                  CombatPower := VarRecord.Value0;
              end;
            finally
              g_CombatPowerVarMgr.UnLock;
            end;
          end;
          if CombatPower <> 0 then
          begin
            PlayObject.m_nCombatPower := PlayObject.m_nCombatPower + (nValue - PlayObject.m_nVal[n01]) * CombatPower;
          end;
          PlayObject.m_nVal[n01] := nValue;
          Result := True;
        end;
      1000..1999: // D
        begin
          CombatPower := 0;
          if g_Config.boOpenCombatPowerVarCalc then
          begin
            g_CombatPowerVarMgr.Lock;
            try
              VarRecord := g_CombatPowerVarMgr.GetValueRecord(sName);
              if VarRecord <> nil then
              begin
                if PlayObject.m_btJob = 1 then
                  CombatPower := VarRecord.Value1
                else if PlayObject.m_btJob = 2 then
                  CombatPower := VarRecord.Value2
                else
                  CombatPower := VarRecord.Value0;
              end;
            finally
              g_CombatPowerVarMgr.UnLock;
            end;
          end;
          if CombatPower <> 0 then
          begin
            PlayObject.m_nCombatPower := PlayObject.m_nCombatPower + (nValue - PlayObject.m_DyVal[n01 - 1000]) * CombatPower;
          end;
          PlayObject.m_DyVal[n01 - 1000] := nValue;
          Result := True;
        end;
      2000..2999: // M
        begin
          CombatPower := 0;
          if g_Config.boOpenCombatPowerVarCalc then
          begin
            g_CombatPowerVarMgr.Lock;
            try
              VarRecord := g_CombatPowerVarMgr.GetValueRecord(sName);
              if VarRecord <> nil then
              begin
                if PlayObject.m_btJob = 1 then
                  CombatPower := VarRecord.Value1
                else if PlayObject.m_btJob = 2 then
                  CombatPower := VarRecord.Value2
                else
                  CombatPower := VarRecord.Value0;
              end;
            finally
              g_CombatPowerVarMgr.UnLock;
            end;
          end;
          if CombatPower <> 0 then
          begin
            PlayObject.m_nCombatPower := PlayObject.m_nCombatPower + (nValue - PlayObject.m_nMval[n01 - 2000]) * CombatPower;
          end;
          PlayObject.m_nMval[n01 - 2000] := nValue;
          Result := True;
        end;
      3000..3999: // N
        begin
          CombatPower := 0;
          if g_Config.boOpenCombatPowerVarCalc then
          begin
            g_CombatPowerVarMgr.Lock;
            try
              VarRecord := g_CombatPowerVarMgr.GetValueRecord(sName);
              if VarRecord <> nil then
              begin
                if PlayObject.m_btJob = 1 then
                  CombatPower := VarRecord.Value1
                else if PlayObject.m_btJob = 2 then
                  CombatPower := VarRecord.Value2
                else
                  CombatPower := VarRecord.Value0;
              end;
            finally
              g_CombatPowerVarMgr.UnLock;
            end;
          end;
          if CombatPower <> 0 then
          begin
            PlayObject.m_nCombatPower := PlayObject.m_nCombatPower + (nValue - PlayObject.m_nInteger[n01 - 3000]) * CombatPower;
          end;
          PlayObject.m_nInteger[n01 - 3000] := nValue;
          Result := True;
        end;
      4000..4999: // I
        begin
          g_Config.GlobaDyMval[n01 - 4000] := nValue;
          Result := True;
        end;
      5000..5999: // G
        begin
          g_Config.GlobalVal[n01 - 5000] := nValue;
          Result := True;
        end;
      6000..6999: // A
        begin
          g_Config.GlobalAVal[n01 - 6000] := sValue;
          Result := True;
        end;
      7000..7999: // S
        begin
          PlayObject.m_sString[n01 - 7000] := sValue;
          Result := True;
        end;
      // 私有变量 U-数字型 chongchong 2014-10-18
      8000..8499: // U
        begin
          CombatPower := 0;
          if g_Config.boOpenCombatPowerVarCalc then
          begin
            g_CombatPowerVarMgr.Lock;
            try
              VarRecord := g_CombatPowerVarMgr.GetValueRecord(sName);
              if VarRecord <> nil then
              begin
                if PlayObject.m_btJob = 1 then
                  CombatPower := VarRecord.Value1
                else if PlayObject.m_btJob = 2 then
                  CombatPower := VarRecord.Value2
                else
                  CombatPower := VarRecord.Value0;
              end;
            finally
              g_CombatPowerVarMgr.UnLock;
            end;
          end;
          if CombatPower <> 0 then
          begin
            PlayObject.m_nCombatPower := PlayObject.m_nCombatPower + (nValue - PlayObject.m_UVal[n01 - 8000]) * CombatPower;
          end;
          PlayObject.m_UVal[n01 - 8000] := nValue;
          Result := True;
        end;
      // 私有变量 T-字符串型 chongchong 2014-10-18
      8500..8999: // T
        begin
          PlayObject.m_TVal[n01 - 8500] := sValue;
          Result := True;
        end;
      // 私有变量 J-数字型(1天1清) chongchong 2014-10-18
      9000..9499: // J
        begin
          CombatPower := 0;
          if g_Config.boOpenCombatPowerVarCalc then
          begin
            g_CombatPowerVarMgr.Lock;
            try
              VarRecord := g_CombatPowerVarMgr.GetValueRecord(sName);
              if VarRecord <> nil then
              begin
                if PlayObject.m_btJob = 1 then
                  CombatPower := VarRecord.Value1
                else if PlayObject.m_btJob = 2 then
                  CombatPower := VarRecord.Value2
                else
                  CombatPower := VarRecord.Value0;
              end;
            finally
              g_CombatPowerVarMgr.UnLock;
            end;
          end;
          if CombatPower <> 0 then
          begin
            PlayObject.m_nCombatPower := PlayObject.m_nCombatPower + (nValue - PlayObject.m_JVal[n01 - 9000]) * CombatPower;
          end;
          PlayObject.m_JVal[n01 - 9000] := nValue;
          Result := True;
        end;
      9500..9999: // J
        begin
          PlayObject.m_ZVal[n01 - 9500] := sValue;
          Result := True;
        end;
    end;
  end
  //L变量
  else if (Length(sName) > 2) and (UpCase(sName[1]) = 'L') and (sName[2] = '$') then
  begin
    // 修正 S$开关的变量不支持子变量解析 chongchong 2017-09-17 - add--
    // MOV S$1 23
    // MOV S$2<$STR(S$1)> 777
    // SENDMSG 6 结S果【<$STR(S$223)>】并不是等于=777，而是等于空
    // ---------------------------------  add begin
    GetVarValue(PlayObject, sName, sParam1, nParam1, IsBreakParseVar);
    if (not SameText(sName, sParam1)) and SetVarValue(PlayObject, sParam1, sValue, nValue) then
    begin
      Result := True;
    end
    else
    // ---------------------------------  add end
    begin
      if (pos('[', sName) > 0) or (pos(']', sName) > 0) then
      begin
        sData := GetValidStr3(sData, sName, ['[']);
        sData := GetValidStr3(sData, arySIndex, [']']);
      end;
      n01 := PlayObject.m_ArrayList.GetIndex(UpperCase(sName));
      if n01 >= 0 then
      begin
        if arySIndex <> '' then
        begin
          aryNIndex := StrToInt(arySIndex);
          //判断新值是不是一个数组
          if (pos('[', sValue) > 0) or (pos(']', sValue) > 0) then
          begin
            sData := ArrestStringEx(sData, '[', ']', sValue);
            oldValue := PlayObject.m_ArrayList.Strings[n01];
            sData := ArrestStringEx(sData, '[', ']', oldValue);
            sValue := sValue + oldValue;
            PlayObject.m_ArrayList.Strings[n01] := sValue;
            Result := True;
          end
          else
          begin
            //不是一个新的数组 直接解析老的覆盖指定位置
            aryLValue := PlayObject.m_ArrayList.Strings[n01].Split([',']);
            //判断长度是不是超了
            if aryNIndex <= length(aryLValue) then
            begin
              aryLValue[aryNIndex] := sValue;
              PlayObject.m_ArrayList.Strings[n01] := string.Join(',', aryLValue);
              Result := True;
            end;

          end;

        end
        else
        begin
          if (pos('[', sValue) > 0) and (pos(']', sValue) > 0) then
          begin
            sData := ArrestStringEx(sValue, '[', ']', sValue);
          end;
          PlayObject.m_ArrayList.Strings[n01] := sValue;
          Result := True;
        end;

      end
      else
      begin
        if (pos('[', sValue) > 0) and (pos(']', sValue) > 0) and (pos('[', sVar) = 0) and (pos(']', sVar) = 0) then
        begin
          sData := ArrestStringEx(sValue, '[', ']', sValue);
          PlayObject.m_ArrayList.AddRecord(UpperCase(sName), sValue);
          Result := True;
        end;
      end;
    end;
  end
  else if (Length(sName) > 2) and (UpCase(sName[1]) = 'S') and (sName[2] = '$') then
  begin
    // 修正 S$开关的变量不支持子变量解析 chongchong 2017-09-17 - add--
    // MOV S$1 23
    // MOV S$2<$STR(S$1)> 777
    // SENDMSG 6 结S果【<$STR(S$223)>】并不是等于=777，而是等于空
    // ---------------------------------  add begin
    GetVarValue(PlayObject, sName, sParam1, nParam1, IsBreakParseVar);
    if (not SameText(sName, sParam1)) and SetVarValue(PlayObject, sParam1, sValue, nValue) then
    begin
      Result := True;
    end
    else
    // ---------------------------------  add end
    begin
      n01 := PlayObject.m_StringList.GetIndex(UpperCase(sName));
      if n01 >= 0 then
      begin
        PlayObject.m_StringList.Strings[n01] := sValue;
        Result := True;
      end
      else
      begin
        PlayObject.m_StringList.AddRecord(UpperCase(sName), sValue);
        Result := True;
      end;
    end;
  end
  else if (Length(sName) > 2) and (UpCase(sName[1]) = 'N') and (sName[2] = '$') then
  begin
    // 修正 N$开关的变量不支持子变量解析 chongchong 2017-09-17 - add--
    // MOV N$1 23
    // MOV N$2<$STR(N$1)> 777
    // SENDMSG 6 结S果【<$STR(N$223)>】并不是等于=777，而是等于空
    // ---------------------------------  add begin
    GetVarValue(PlayObject, sName, sParam1, nParam1, IsBreakParseVar);
    if (not SameText(sName, sParam1)) and SetVarValue(PlayObject, sParam1, sValue, nValue) then
    begin
      Result := True;
    end
    else
    // ---------------------------------  add end
    begin
      n01 := PlayObject.m_IntegerList.GetIndex(UpperCase(sName));
      if n01 >= 0 then
      begin
        CombatPower := 0;
        if g_Config.boOpenCombatPowerVarCalc then
        begin
          g_CombatPowerVarMgr.Lock;
          try
            VarRecord := g_CombatPowerVarMgr.GetValueRecord(sName);
            if VarRecord <> nil then
            begin
              if PlayObject.m_btJob = 1 then
                CombatPower := VarRecord.Value1
              else if PlayObject.m_btJob = 2 then
                CombatPower := VarRecord.Value2
              else
                CombatPower := VarRecord.Value0;
            end;
          finally
            g_CombatPowerVarMgr.UnLock;
          end;
        end;
        if CombatPower <> 0 then
        begin
          PlayObject.m_nCombatPower := PlayObject.m_nCombatPower + (nValue - Integer(PlayObject.m_IntegerList.Objects[n01])) * CombatPower;
        end;
        PlayObject.m_IntegerList.Objects[n01] := TObject(nValue);
        Result := True;
      end
      else
      begin
        PlayObject.m_IntegerList.AddRecord(UpperCase(sName), nValue);
        Result := True;
      end;
    end;
  end;
end;

function GetBoxItemValue(sVar: string; PlayObject: TPlayObject; var Ret: string): Boolean;
var
  sIndex: string;
  I, nIndex, nMakeIndex: Integer;
  nIndexLen: Integer;
  UseItem: pTUserItem;
  sSubName, sUserItemName: string;
  StdItem: pTStdItem;
  NewStdItem: TStdItem;
begin
  Result := False;
  if Length(sVar) <= 14 then
    Exit;
  if (sVar[1] <> '<') or (sVar[Length(sVar)] <> '>') then
    Exit;
  if (sVar[10] <> '[') then
    Exit;
  nIndexLen := 0;
  if (sVar[12] = ']') and (sVar[13] = '.') then
    nIndexLen := 1
  else if (sVar[13] = ']') and (sVar[14] = '.') then
    nIndexLen := 2;
  if nIndexLen = 0 then
    Exit;
  sIndex := Copy(sVar, 11, nIndexLen);
  nIndex := StrToIntDef(sIndex, -1);
  if not (nIndex in [Low(THumanItemBoxItems)..High(THumanItemBoxItems)]) then
    Exit;
  nMakeIndex := PlayObject.m_ItemBoxItems[nIndex];
  // 当OK框中没有放入物品时，变量值置空，并将函数返回真，以便变量替换 chongchong 2014-03-06
  if nMakeIndex = 0 then
  begin
    Ret := '';
    Result := True;
    Exit;
  end;
  UseItem := nil;
  for I := 0 to PlayObject.m_ItemList.Count - 1 do
  begin
    if pTUserItem(PlayObject.m_ItemList[I]).MakeIndex = nMakeIndex then
    begin
      UseItem := PlayObject.m_ItemList[I];
      Break;
    end;
  end;
  if UseItem = nil then
    Exit;
  StdItem := UserEngine.GetStdItem(UseItem.wIndex);
  if StdItem = nil then
    Exit;
  NewStdItem := StdItem^;
  ItemUnit.GetItemAddValue(UseItem, NewStdItem);
  sSubName := Copy(sVar, 13 + nIndexLen, MaxInt);
  if Length(sSubName) = 0 then
    Exit;
  sSubName := Copy(sSubName, 1, Length(sSubName) - 1);
  if SameText(sSubName, 'name') then
  begin
    Ret := NewStdItem.Name;
    Result := True;
  end
  else if SameText(sSubName, 'name_g') then
  begin
    Ret := NewStdItem.Name;
    { 获取自定义名称 }
    sUserItemName := '';
    if UseItem.btValue[13] = 1 then
      sUserItemName := UseItem.Name;
    if sUserItemName <> '' then
      Ret := sUserItemName;
    Result := True;
  end
  else if SameText(sSubName, 'dura') then
  begin
    Ret := IntToStr(UseItem.Dura);
    Result := True;
  end
  else if SameText(sSubName, 'duramax') then
  begin
    Ret := IntToStr(UseItem.DuraMax);
    Result := True;
  end
  else if SameText(sSubName, 'makeindex') then
  begin
    Ret := IntToStr(UseItem.MakeIndex);
    Result := True;
  end
  else if SameText(sSubName, 'stdmode') then
  begin
    Ret := IntToStr(NewStdItem.StdMode);
    Result := True;
  end
  else if SameText(sSubName, 'shape') then
  begin
    Ret := IntToStr(NewStdItem.Shape);
    Result := True;
  end
  else if SameText(sSubName, 'looks') then
  begin
    Ret := IntToStr(NewStdItem.Looks);
    Result := True;
  end
  else if SameText(sSubName, 'color') then
  begin
    if StdItem.Color = 251 then
      Ret := IntToStr(g_Config.btThrowAwayItemColor)
    else
      Ret := IntToStr(StdItem.Color);
    if UseItem.btColor > 0 then
      Ret := IntToStr(UseItem.btColor);
    Result := True;
  end
  else if SameText(sSubName, 'upgradecount') then
  begin
    Ret := IntToStr(UseItem.btUpgradeCount);
    Result := True;
  end
  else if SameText(sSubName, 'hp') then
  begin
    Ret := IntToStr(NewStdItem.HP);
    Result := True;
  end
  else if SameText(sSubName, 'mp') then
  begin
    Ret := IntToStr(NewStdItem.MP);
    Result := True;
  end
  else if SameText(sSubName, 'lac') then
  begin
    Ret := IntToStr(NewStdItem.AC1);
    Result := True;
  end
  else if SameText(sSubName, 'hac') then
  begin
    Ret := IntToStr(NewStdItem.AC2);
    Result := True;
  end
  else if SameText(sSubName, 'lmac') then
  begin
    Ret := IntToStr(NewStdItem.Mac1);
    Result := True;
  end
  else if SameText(sSubName, 'hmac') then
  begin
    Ret := IntToStr(NewStdItem.MAC2);
    Result := True;
  end
  else if SameText(sSubName, 'ldc') then
  begin
    Ret := IntToStr(NewStdItem.DC1);
    Result := True;
  end
  else if SameText(sSubName, 'hdc') then
  begin
    Ret := IntToStr(NewStdItem.DC2);
    Result := True;
  end
  else if SameText(sSubName, 'lmc') then
  begin
    Ret := IntToStr(NewStdItem.MC1);
    Result := True;
  end
  else if SameText(sSubName, 'hmc') then
  begin
    Ret := IntToStr(NewStdItem.MC2);
    Result := True;
  end
  else if SameText(sSubName, 'lsc') then
  begin
    Ret := IntToStr(NewStdItem.SC1);
    Result := True;
  end
  else if SameText(sSubName, 'hsc') then
  begin
    Ret := IntToStr(NewStdItem.SC2);
    Result := True;
  end
  else if SameText(sSubName, 'idx') then
  begin
    Ret := IntToStr(UseItem.wIndex - 1);
    Result := True;
  end
  else if SameText(sSubName, 'need') then
  begin
    Ret := IntToStr(NewStdItem.Need);
    Result := True;
  end
  else if SameText(sSubName, 'needlevel') then
  begin
    Ret := IntToStr(NewStdItem.NeedLevel);
    Result := True;
  end
  else if SameText(sSubName, 'price') then
  begin
    Ret := IntToStr(NewStdItem.Price);
    Result := True;
  end
  else if SameText(sSubName, 'element') then
  begin
    Ret := IntToStr(NewStdItem.Elements[0]);
    Result := True;
  end
  else if SameText(sSubName, 'element1') then
  begin
    Ret := IntToStr(NewStdItem.Elements[1]);
    Result := True;
  end
  else if SameText(sSubName, 'element2') then
  begin
    Ret := IntToStr(NewStdItem.Elements[2]);
    Result := True;
  end
  else if SameText(sSubName, 'element3') then
  begin
    Ret := IntToStr(NewStdItem.Elements[3]);
    Result := True;
  end
  else if SameText(sSubName, 'element4') then
  begin
    Ret := IntToStr(NewStdItem.Elements[4]);
    Result := True;
  end
  else if SameText(sSubName, 'element5') then
  begin
    Ret := IntToStr(NewStdItem.Elements[5]);
    Result := True;
  end
  else if SameText(sSubName, 'element6') then
  begin
    Ret := IntToStr(NewStdItem.Elements[6]);
    Result := True;
  end
  else if SameText(sSubName, 'element7') then
  begin
    Ret := IntToStr(NewStdItem.Elements[7]);
    Result := True;
  end
  else if SameText(sSubName, 'element8') then
  begin
    Ret := IntToStr(NewStdItem.Elements[8]);
    Result := True;
  end
  else if SameText(sSubName, 'element9') then
  begin
    Ret := IntToStr(NewStdItem.Elements[9]);
    Result := True;
  end
  else if SameText(sSubName, 'element10') then
  begin
    Ret := IntToStr(NewStdItem.Elements[10]);
    Result := True;
  end
  else if SameText(sSubName, 'element11') then
  begin
    Ret := IntToStr(NewStdItem.Elements[11]);
    Result := True;
  end
  else if SameText(sSubName, 'element12') then
  begin
    Ret := IntToStr(NewStdItem.Elements[12]);
    Result := True;
  end
  else if SameText(sSubName, 'element13') then
  begin
    Ret := IntToStr(NewStdItem.Elements[13]);
    Result := True;
  end
  else if SameText(sSubName, 'element14') then
  begin
    Ret := IntToStr(NewStdItem.Elements[14]);
    Result := True;
  end
  else if SameText(sSubName, 'element15') then
  begin
    Ret := IntToStr(NewStdItem.Elements[15]);
    Result := True;
  end
  else if SameText(sSubName, 'element16') then
  begin
    Ret := IntToStr(NewStdItem.Elements[16]);
    Result := True;
  end
  else if SameText(sSubName, 'element17') then
  begin
    Ret := IntToStr(NewStdItem.Elements[17]);
    Result := True;
  end
  else if SameText(sSubName, 'element18') then
  begin
    Ret := IntToStr(NewStdItem.Elements[18]);
    Result := True;
  end
  else if SameText(sSubName, 'element19') then
  begin
    Ret := IntToStr(NewStdItem.Elements[19]);
    Result := True;
  end
  else if SameText(sSubName, 'element20') then
  begin
    Ret := IntToStr(NewStdItem.Elements[20]);
    Result := True;
  end
  else if SameText(sSubName, 'element21') then
  begin
    Ret := IntToStr(NewStdItem.Elements[21]);
    Result := True;
  end
  else if SameText(sSubName, 'element22') then
  begin
    Ret := IntToStr(NewStdItem.Elements[22]);
    Result := True;
  end
  else if SameText(sSubName, 'element23') then
  begin
    Ret := IntToStr(NewStdItem.Elements[23]);
    Result := True;
  end
  else if SameText(sSubName, 'element24') then
  begin
    Ret := IntToStr(NewStdItem.Elements[24]);
    Result := True;
  end
  else if SameText(sSubName, 'expand1') then
  begin
    Ret := IntToStr(NewStdItem.Expand1);
    Result := True;
  end
  else if SameText(sSubName, 'expand2') then
  begin
    Ret := IntToStr(NewStdItem.Expand3);
    Result := True;
  end
  else if SameText(sSubName, 'expand3') then
  begin
    Ret := IntToStr(NewStdItem.Expand3);
    Result := True;
  end
  else if SameText(sSubName, 'expand4') then
  begin
    Ret := IntToStr(NewStdItem.Expand4);
    Result := True;
  end
  else if SameText(sSubName, 'expand5') then
  begin
    Ret := IntToStr(NewStdItem.Expand5);
    Result := True;
  end
  else if SameText(sSubName, 'InsuranceCurrency') then
  begin
    Ret := IntToStr(NewStdItem.InsuranceCurrency);
    Result := True;
  end
  else if SameText(sSubName, 'InsuranceGold') then
  begin
    Ret := IntToStr(NewStdItem.InsuranceGold);
    Result := True;
  end
  else if SameText(sSubName, 'InsuranceCount') then
  begin
    Ret := IntToStr(UseItem.wInsuranceCount);
    Result := True;
  end;
end;

function TNormNpc.GetValNameValue(PlayObject: TPlayObject; sVar: string; var sValue: string; var nValue: Integer): Boolean;
var
  n01: Integer;
  sData, sName: string;
  arySIndex: string;
begin
  Result := False;
  if sVar = '' then
    Exit;
  sData := sVar;
  sValue := sVar;
  nValue := 0;
  sName := sVar;
  if (Length(sData) > 8) and CompareLStr(sData, '<$STR(', Length('<$STR(')) and (sData[Length(sData) - 1] = ')') and (sData[Length(sData)] = '>') then
  begin
    sData := ArrestStringEx(sData, '(', ')', sName);
    if sName = '' then
      Exit;
  end;
  if SameText(Copy(sName, 1, 10), '<$BOXITEM[') then
  begin
    Result := GetBoxItemValue(sName, PlayObject, sData);
    if Result then
    begin
      sValue := sData;
      nValue := StrToInt64Def(sValue, nValue);
    end;
    Exit;
  end;
  {
    else
    begin
    // 自定义OK框变量
    sVarName := '';
    Index1 := Pos('<$', sName);
    if Index1 > 0 then
    begin
    sBeforeName := Copy(sName, 1, Index1 - 1);
    sTempName := Copy(sName, Index1, MaxInt);
    Index2 := Pos('>', sTempName);
    begin
    sVarName := Copy(sTempName, 1, Index2);
    sAfterName := Copy(sTempName, Index2 + 1, MaxInt);
    end;
    end;
    if Length(sVarName) > 0 then
    begin
    if GetBoxItemValue(sVarName, PlayObject, sData) then
    begin
    Result := True;
    SValue := sBeforeName + sData + sAfterName;
    Exit;
    end;
    end;
    end;
  }
  n01 := GetValNameNo(sName);
  if n01 >= 0 then
  begin
    case n01 of
      0..999:
        begin // P
          nValue := PlayObject.m_nVal[n01];
          sValue := IntToStr(nValue);
          Result := True;
        end;
      1000..1999:
        begin // D
          nValue := PlayObject.m_DyVal[n01 - 1000];
          sValue := IntToStr(nValue);
          Result := True;
        end;
      2000..2999:
        begin // M
          nValue := PlayObject.m_nMval[n01 - 2000];
          sValue := IntToStr(nValue);
          Result := True;
        end;
      3000..3999:
        begin // N
          nValue := PlayObject.m_nInteger[n01 - 3000];
          sValue := IntToStr(nValue);
          Result := True;
        end;
      4000..4999:
        begin // I
          nValue := g_Config.GlobaDyMval[n01 - 4000];
          sValue := IntToStr(nValue);
          Result := True;
        end;
      5000..5999:
        begin // G
          nValue := g_Config.GlobalVal[n01 - 5000];
          sValue := IntToStr(nValue);
          Result := True;
        end;
      6000..6999:
        begin // A
          sValue := g_Config.GlobalAVal[n01 - 6000];
          nValue := StrToInt64Def(sValue, nValue);
          Result := True;
        end;
      7000..7999:
        begin // S
          sValue := PlayObject.m_sString[n01 - 7000];
          nValue := StrToInt64Def(sValue, nValue);
          Result := True;
        end;
      // 私有变量 U-数字型 chongchong 2014-10-18
      8000..8499:
        begin
          nValue := PlayObject.m_UVal[n01 - 8000];
          sValue := IntToStr(nValue);
          Result := True;
        end;
      // 私有变量 T-字符串型 chongchong 2014-10-18
      8500..8999:
        begin
          sValue := PlayObject.m_TVal[n01 - 8500];
          nValue := StrToInt64Def(sValue, nValue);
          Result := True;
        end;
      // 私有变量 J-数字型(1天1清) chongchong 2014-10-18
      9000..9499:
        begin
          nValue := PlayObject.m_JVal[n01 - 9000];
          sValue := IntToStr(nValue);
          Result := True;
        end;
      // 私有变量 Z-字符串型(1天1清) chongchong 2014-10-18
      9500..9999:
        begin
          sValue := PlayObject.m_ZVal[n01 - 9500];
          nValue := StrToInt64Def(sValue, nValue);
          Result := True;
        end;
    end;
  end
  //L变量
  else if (Length(sName) > 2) and (UpCase(sName[1]) = 'L') and (sName[2] = '$') then
  begin
    if (Pos('[', sName) > 0) and (Pos(']', sName) > 0) then
    begin
      sData := GetValidStr3(sName, sName, ['[']);
      sData := GetValidStr3(sData, arySIndex, [']']);
    end;

    n01 := PlayObject.m_ArrayList.GetIndex(UpperCase(sName));
    if n01 >= 0 then
    begin
      sValue := PlayObject.m_ArrayList.Strings[n01];
    end
    else
    begin
      sValue := '';
    end;
    nValue := StrToInt64Def(sValue, 0);
    Result := True;
  end
  else if (Length(sName) > 2) and (UpCase(sName[1]) = 'S') and (sName[2] = '$') then
  begin
    n01 := PlayObject.m_StringList.GetIndex(UpperCase(sName));
    if n01 >= 0 then
    begin
      sValue := PlayObject.m_StringList.Strings[n01];
    end
    else
    begin
      sValue := '';
    end;
    nValue := StrToInt64Def(sValue, 0);
    Result := True;
  end
  else if (Length(sName) > 2) and (UpCase(sName[1]) = 'N') and (sName[2] = '$') then
  begin
    n01 := PlayObject.m_IntegerList.GetIndex(UpperCase(sName));
    if n01 >= 0 then
    begin
      nValue := Integer(PlayObject.m_IntegerList.Objects[n01]);
    end
    else
    begin
      nValue := 0;
    end;
    sValue := IntToStr(nValue);
    Result := True;
  end;
end;

constructor TNormNpc.Create; // 0049AA38
begin
  inherited;
  m_boSuperMan := True;
  m_btRaceServer := RC_NPC;
  m_nLight := 2;
  m_btAntiPoison := 99;
  m_ScriptList := TList.Create;
  m_boStickMode := True;
  m_sFilePath := '';
  m_boIsHide := False;
  m_boIsQuest := True;
  m_boNpcAutoChangeColor := False;
  m_dwNpcAutoChangeColorTick := MyGetTickCount;
  m_dwNpcAutoChangeColorTime := 0;
  m_nNpcAutoChangeIdx := 0;
  m_nGlobalAValIndex := -1;
  m_sDynamicName := '';
  m_btNameColor := g_Config.btMerchantNameColor;
  m_NoUserSelectList := TStringList.Create;
  m_boMapEvent := False;
  m_boPlug := False;
  m_sDataFileName := '';
  m_sAddName := '';
  m_nAddLevel := 0;
  m_nEffigyState.Value1 := 0;
  m_nEffigyState.Value2 := 0;
  m_nEffigyOffset := 8;
  m_boGrayShow := True;
  m_boScaleShow := True;
  m_nValidTime := -1;
  m_wValidTimeTick := MyGetTickCount;
  m_sLastLabel := '';
  m_dwScriptCRC := 0;
  m_dwIconFileCRC := 0;
  m_CallFileListCRC := TStringList.Create;
end;

destructor TNormNpc.Destroy; // 0049AAE4
// var
// I: Integer;
begin
  ClearScript();
  {
    for I := 0 to ScriptList.Count - 1 do begin
    Dispose(pTScript(ScriptList.Items[I]));
    end;
  }
  m_ScriptList.Free;
  m_NoUserSelectList.Free;
  m_CallFileListCRC.Free;
  SaveAddData;
  inherited;
end;

function TNormNpc.AllowSelect(sLabel: string): Boolean;
var
  I: Integer;
begin
  Result := True;
  for I := 0 to m_NoUserSelectList.Count - 1 do
  begin
    if CompareLStr(sLabel, m_NoUserSelectList.Strings[I], Length(m_NoUserSelectList.Strings[I])) then
    begin
      if (Self = g_FunctionNPC) or (Self = g_MissionNPC) then
      begin
        MainOutMessage(Format('不可点字段:%s', [m_NoUserSelectList.Strings[I]]));
      end;
      Result := False;
      Exit;
    end;
  end;
end;

procedure TNormNpc.AddSelectLable(sLabel: string);
var
  I: Integer;
begin
  for I := 0 to m_NoUserSelectList.Count - 1 do
  begin
    if CompareLStr(sLabel, m_NoUserSelectList.Strings[I], Length(m_NoUserSelectList.Strings[I])) then
    begin
      Exit;
    end;
  end;
  m_NoUserSelectList.Add(sLabel);
end;

procedure TNormNpc.DeleteSelectLable(sLabel: string);
var
  I: Integer;
begin
  for I := 0 to m_NoUserSelectList.Count - 1 do
  begin
    if CompareLStr(sLabel, m_NoUserSelectList.Strings[I], Length(m_NoUserSelectList.Strings[I])) then
    begin
      m_NoUserSelectList.Delete(I);
      Break;
    end;
  end;
end;

function TNormNpc.GetLineVariableText(PlayObject: TPlayObject; sMsg: string; var IsBreakParseVar: Boolean): string;
var
  nC: Integer;
  nPos: Integer;
  nStartPos: Integer;
  s14, s10: string;
begin
  nC := 0;
  nStartPos := 1;
  while (True) do
  begin
    s14 := sMsg;
    if (Pos('>', s14) <= 0) or (Pos('$', s14) <= 0) or (nStartPos >= Length(s14)) then
      Break;

    nPos := ArrestVariable(s14, '<', '$', '>', nStartPos, s10);
    if s10 = '' then
      Break;

    if not GetVariableText(PlayObject, sMsg, s10, IsBreakParseVar, nPos) then
      nStartPos := nPos + 2;

    Inc(nC);
    if nC >= 1001 then
      Break;
  end;

  Result := sMsg;
end;

function TNormNpc.GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string; var IsBreakParseVar: Boolean; nPos: Integer): Boolean;
resourcestring
  HookError = '[Exception] HookGetVariableText';
var
  sValue: string;
  sText, s14: string;
  sData, arySIndex: string;
  aryNIndex: Integer;
  aryLValue: TArray<string>;
  I, II, nIndex: Integer;
  n18: Integer;
  wHour: Word;
  nSecond: Integer;
  DynamicVar: pTDynamicVar;
  boFoundVar: Boolean;
  s1C: string;
  PoseHuman, TempObject: TBaseObject;
  Dealing: TGameGoldDeal;
  RememberItem: pTRememberItem;
  Envir: TEnvirnoment;
  Year, Month, Day: Word;
  Hour, Min, Sec, MSec: Word;
  FoundryItem: pTFoundryItem;
  FoundryNeedItem: pTFoundryNeedItem;
  UserCastle: TUserCastle;
  Buffer: PAnsiChar;
  BufLen: DWORD;
  NationInfo: pTNationInfo;
  StdItem: pTStdItem;
  UserItem: pTUserItem;
  Master1, Master2: string;
  Hero: THeroObject;
  GamePet: TBaseObject;
  UserShop: TUserShop;
begin
  Result := True;
  sVariable := UpperCase(sVariable);
  if PlayObject.m_MyHero <> nil then
    Hero := THeroObject(PlayObject.m_MyHero)
  else
    Hero := nil;

  if PlayObject.m_MyGamePet <> nil then
    GamePet := PlayObject.m_MyGamePet
  else
    GamePet := nil;

  sText := '';
  nIndex := g_NpcVarNameList.IndexOf(sVariable);
  if nIndex >= 0 then
  begin
    nIndex := Integer(g_NpcVarNameList.Objects[nIndex]);
    case nIndex of
      nNVN_ServerName:
        sText := g_Config.sServerName;
      nNVN_ServerIP:
        sText := g_Config.sServerIPaddr;
      nNVN_Website:
        sText := g_Config.sWebSite;
      nNVN_BBSSite:
        sText := g_Config.sBbsSite;
      nNVN_ClientDownload:
        sText := g_Config.sClientDownload;
      nNVN_QQ:
        sText := g_Config.sQQ;
      nNVN_ConfigPhone:
        sText := g_Config.sPhone;
      nNVN_BankAccount0:
        sText := g_Config.sBankAccount0;
      nNVN_BankAccount1:
        sText := g_Config.sBankAccount1;
      nNVN_BankAccount2:
        sText := g_Config.sBankAccount2;
      nNVN_BankAccount3:
        sText := g_Config.sBankAccount3;
      nNVN_BankAccount4:
        sText := g_Config.sBankAccount4;
      nNVN_BankAccount5:
        sText := g_Config.sBankAccount5;
      nNVN_BankAccount6:
        sText := g_Config.sBankAccount6;
      nNVN_BankAccount7:
        sText := g_Config.sBankAccount7;
      nNVN_BankAccount8:
        sText := g_Config.sBankAccount8;
      nNVN_BankAccount9:
        sText := g_Config.sBankAccount9;
      nNVN_GameGoldName:
        sText := g_Config.sGameGoldName;
      nNVN_GamePointName:
        sText := g_Config.sGamePointName;
      nNVN_UserCount:
        sText := IntToStr(UserEngine.PlayObjectCount);
      nNVN_LiUserCount:
        sText := IntToStr(UserEngine.GetOfflineCount);
      nNVN_DummyCount:
        sText := IntToStr(UserEngine.GetDummyObjectCount);
      nNVN_OnUserCount:
        sText := IntToStr(UserEngine.GetReallyCount);
      nNVN_MacRunTime:
        sText := CurrToStr(MyGetTickCount / (24 * 60 * 60 * 1000));
      nNVN_RunDateTime:
        sText := CurrToStr(MyGetTickCount() / (24 * 60 * 60 * 1000)) + '天';
      nNVN_ServerRunTime:
        begin
          nSecond := (MyGetTickCount() - g_dwStartTick) div 1000;
          wHour := nSecond div 3600;
          sText := IntToStr(wHour);
        end;
      nNVN_StatServerTime:
        sText := GetStartTime(g_nStartTime);
      nNVN_DateTime:
        sText := FormatDateTime('yyyy/mm/dd,dddd,hh:nn:ss', Now);
      nNVN_Date:
        sText := FormatDateTime('yyyy/mm/dd', Now);
      nNVN_Time:
        sText := FormatDateTime('hh:nn:ss', Now);
      nNVN_Year:
        begin
          DecodeDate(Now, Year, Month, Day);
          sText := IntToStr(Year);
        end;
      nNVN_Month:
        begin
          DecodeDate(Now, Year, Month, Day);
          sText := IntToStr(Month);
        end;
      nNVN_Day:
        begin
          DecodeDate(Now, Year, Month, Day);
          sText := IntToStr(Day);
        end;
      nNVN_Hour:
        begin
          DecodeTime(Time, Hour, Min, Sec, MSec);
          sText := IntToStr(Hour);
        end;
      nNVN_Minute:
        begin
          DecodeTime(Time, Hour, Min, Sec, MSec);
          sText := IntToStr(Min);
        end;
      nNVN_Second:
        begin
          DecodeTime(Time, Hour, Min, Sec, MSec);
          sText := IntToStr(Sec);
        end;
      // -------------------------------------------------------------------------
      nNVN_HighLevelInfo:
        begin
          sText := '????';
          if g_HighLevelHuman <> nil then
            sText := TPlayObject(g_HighLevelHuman).GetMyInfo;
        end;
      nNVN_HighPKInfo:
        begin
          sText := '????';
          if g_HighPKPointHuman <> nil then
            sText := TPlayObject(g_HighPKPointHuman).GetMyInfo;
        end;
      nNVN_HighDCInfo:
        begin
          sText := '????';
          if g_HighDCHuman <> nil then
            sText := TPlayObject(g_HighDCHuman).GetMyInfo;
        end;
      nNVN_HighMCInfo:
        begin
          sText := '????';
          if g_HighMCHuman <> nil then
            sText := TPlayObject(g_HighMCHuman).GetMyInfo;
        end;
      nNVN_HighSCInfo:
        begin
          sText := '????';
          if g_HighSCHuman <> nil then
            sText := TPlayObject(g_HighSCHuman).GetMyInfo;
        end;
      nNVN_HighOnlineInfo:
        begin
          sText := '????';
          if g_HighOnlineHuman <> nil then
            sText := TPlayObject(g_HighOnlineHuman).GetMyInfo;
        end;
      // ----------------------------------------------------------------------------------
      nNVN_RequestCastLewarItem:
        sText := g_Config.sZumaPiece;
      nNVN_RequestCastLewarDay:
        sText := g_Config.sZumaPiece;
      nNVN_RequestBuildGuildItem:
        sText := g_Config.sWomaHorn;
      nNVN_GuildWarFee:
        sText := IntToStr(g_Config.nGuildWarPrice);
      nNVN_BuildGuildFee:
        sText := IntToStr(g_Config.nBuildGuildPrice);
      nNVN_CMD_Date:
        sText := g_GameCommand.Data.sCmd;
      nNVN_CMD_AllowMsg:
        sText := g_GameCommand.ALLOWMSG.sCmd;
      nNVN_CMD_LetShout:
        sText := g_GameCommand.LETSHOUT.sCmd;
      nNVN_CMD_LetTrade:
        sText := g_GameCommand.LETTRADE.sCmd;
      nNVN_CMD_LetGuild:
        sText := g_GameCommand.LETGUILD.sCmd;
      nNVN_CMD_LetChallenge:
        sText := g_GameCommand.LETCHALLENGE.sCmd;
      nNVN_CMD_EndGuild:
        sText := g_GameCommand.ENDGUILD.sCmd;
      nNVN_CMD_BanGuildChat:
        sText := g_GameCommand.BANGUILDCHAT.sCmd;
      nNVN_CMD_AutHally:
        sText := g_GameCommand.AUTHALLY.sCmd;
      nNVN_CMD_Auth:
        sText := g_GameCommand.AUTH.sCmd;
      nNVN_CMD_AuthCancel:
        sText := g_GameCommand.AUTHCANCEL.sCmd;
      nNVN_CMD_UserMove:
        sText := g_GameCommand.USERMOVE.sCmd;
      nNVN_CMD_Searching:
        sText := g_GameCommand.SEARCHING.sCmd;
      nNVN_CMD_AllowGroupCall:
        sText := g_GameCommand.ALLOWGROUPCALL.sCmd;
      nNVN_CMD_GroupRecall:
        sText := g_GameCommand.GROUPRECALLL.sCmd;
      nNVN_CMD_AttackMode:
        sText := g_GameCommand.ATTACKMODE.sCmd;
      nNVN_CMD_Rest:
        sText := g_GameCommand.REST.sCmd;
      nNVN_CMD_StorageSetPassword:
        sText := g_GameCommand.SETPASSWORD.sCmd;
      nNVN_CMD_StorageChgPassword:
        sText := g_GameCommand.CHGPASSWORD.sCmd;
      nNVN_CMD_StorageLock:
        sText := g_GameCommand.Lock.sCmd;
      nNVN_CMD_StorageUnlock:
        sText := g_GameCommand.UNLOCKSTORAGE.sCmd;
      nNVN_CMD_Unlock:
        sText := g_GameCommand.UnLock.sCmd;
      // --------------------------------------------------------------------------------------
      nNVN_Item:
        sText := PlayObject.m_sNpcSelectItemName;
      nNVN_NpcSetUser:
        sText := m_sAddName;
      nNVN_NpcSetLevel:
        sText := IntToStr(m_nAddLevel);
      nNVN_Param0:
        sText := g_sInputParam0;
      nNVN_Param1:
        sText := g_sInputParam1;
      nNVN_Param2:
        sText := g_sInputParam2;
      nNVN_Param3:
        sText := g_sInputParam3;
      nNVN_Param4:
        sText := g_sInputParam4;
      nNVN_Param5:
        sText := g_sInputParam5;
      nNVN_Param6:
        sText := g_sInputParam6;
      nNVN_Params:
        sText := g_sInputParams;
      nNVN_ScriptParam1:
        sText := PlayObject.m_sScriptParams[0];
      nNVN_ScriptParam2:
        sText := PlayObject.m_sScriptParams[1];
      nNVN_ScriptParam3:
        sText := PlayObject.m_sScriptParams[2];
      nNVN_ScriptParam4:
        sText := PlayObject.m_sScriptParams[3];
      nNVN_ScriptParam5:
        sText := PlayObject.m_sScriptParams[4];
      nNVN_ScriptParam6:
        sText := PlayObject.m_sScriptParams[5];
      nNVN_ScriptParam7:
        sText := PlayObject.m_sScriptParams[6];
      nNVN_ScriptParam8:
        sText := PlayObject.m_sScriptParams[7];
      nNVN_ScriptParam9:
        sText := PlayObject.m_sScriptParams[8];
      // --------------------------------------------------------------------------------------
      nNVN_UserID:
        sText := PlayObject.m_sUserID;
      nNVN_UserName:
        sText := PlayObject.m_sCharName;
      nNVN_UserNewName:
        sText := PlayObject.m_sCharNewName;
      nNVN_Password:
        sText := PlayObject.m_sUserEntryPassword;
      nNVN_Birthday:
        sText := PlayObject.m_sUserEntryBirthDay;
      nNVN_Quiz1:
        sText := PlayObject.m_sUserEntryQuiz;
      nNVN_Answer1:
        sText := PlayObject.m_sUserEntryAnswer;
      nNVN_Quiz2:
        sText := PlayObject.m_sUserEntryQuiz2;
      nNVN_Answer2:
        sText := PlayObject.m_sUserEntryAnswer2;
      nNVN_Email:
        sText := PlayObject.m_sUserEntryEMail;
      nNVN_Phone:
        sText := PlayObject.m_sUserEntryPhone;
      nNVN_MobilePhone:
        sText := PlayObject.m_sUserEntryMobilePhone;
      nNVN_AccountUserName:
        sText := PlayObject.m_sUserEntryUserName;
      // nNVN_IDCard:
      // sText := PlayObject.m_sUserEntryUserName;
      nNVN_ClientBuildVer:
        sText := IntToStr(PlayObject.m_ClientBuildVersion);
      nNVN_GamePromotionFlag:
        sText := PlayObject.m_sGamePromotionFlag;
      nNVN_MobileNumber:
        sText := PlayObject.m_sMobileNumber;
      nNVN_BindMobileNumber:
        begin
          if PlayObject.m_boMobileBind then
            sText := PlayObject.m_sMobileNumber;
        end;
      nNVN_Gender:
        sText := IntToStr(PlayObject.m_btGender);
      nNVN_Job:
        sText := IntToStr(PlayObject.m_btJob);
      nNVN_Map:
        sText := PlayObject.m_PEnvir.sMapName;
      nNVN_MapTitle:
        sText := PlayObject.m_PEnvir.sMapDesc;
      nNVN_MapName:
        sText := PlayObject.m_PEnvir.sMapDesc;
      nNVN_FBMapName:
        begin
          if (PlayObject.m_PEnvir <> nil) and PlayObject.m_PEnvir.m_boFB then
            sText := PlayObject.m_PEnvir.m_sFBName;
        end;
      nNVN_FBmap:
        begin
          if (PlayObject.m_PEnvir <> nil) and PlayObject.m_PEnvir.m_boFB then
            sText := PlayObject.m_PEnvir.sMapName;
        end;
      nNVN_X:
        sText := IntToStr(PlayObject.m_nCurrX);
      nNVN_Y:
        sText := IntToStr(PlayObject.m_nCurrY);
      nNVN_Level:
        sText := IntToStr(PlayObject.m_Abil.Level);
      nNVN_ReLevel:
        sText := IntToStr(PlayObject.m_btReLevel);
      nNVN_Hit:
        sText := IntToStr(PlayObject.m_btHitPoint);
      nNVN_Spd:
        sText := IntToStr(PlayObject.m_btSpeedPoint);
      nNVN_HitSpd:
        sText := IntToStr(PlayObject.m_nHitSpeed);
      nNVN_Luck:
        sText := IntToStr(PlayObject.m_nLuck);
      nNVN_MoveSpeed:
        sText := IntToStr(PlayObject.m_nMoveSpeed);
      nNVN_AttackSpeed:
        sText := IntToStr(PlayObject.m_nAttackSpeed);
      nNVN_SpellSpeed:
        sText := IntToStr(PlayObject.m_nSpellSpeed);
      nNVN_PoisonRecover:
        sText := IntToStr(PlayObject.m_nPoisonRecover); // 中毒恢复
      nNVN_AntiPoison:
        sText := IntToStr(PlayObject.m_btAntiPoison); // 毒躲避
      nNVN_SpellRecover:
        sText := IntToStr(PlayObject.m_nSpellRecover); // 魔法恢复
      nNVN_AntiMagic:
        sText := IntToStr(PlayObject.m_nAntiMagic); // 魔法躲避
      nNVN_HealthRecover:
        sText := IntToStr(PlayObject.m_nHealthRecover); // 体力恢复
      nNVN_HP:
        sText := IntToStr(PlayObject.m_WAbil.HP);
      nNVN_MaxHP:
        sText := IntToStr(PlayObject.m_WAbil.MaxHP);
      nNVN_MP:
        sText := IntToStr(PlayObject.m_WAbil.MP);
      nNVN_MaxMP:
        sText := IntToStr(PlayObject.m_WAbil.MaxMP);
      nNVN_AC:
        sText := IntToStr(PlayObject.m_WAbil.AC1);
      nNVN_MaxAC:
        sText := IntToStr(PlayObject.m_WAbil.AC2);
      nNVN_MAC:
        sText := IntToStr(PlayObject.m_WAbil.Mac1);
      nNVN_MaxMAC:
        sText := IntToStr(PlayObject.m_WAbil.MAC2);
      nNVN_DC:
        sText := IntToStr(PlayObject.m_WAbil.DC1);
      nNVN_MaxDC:
        sText := IntToStr(PlayObject.m_WAbil.DC2);
      nNVN_MC:
        sText := IntToStr(PlayObject.m_WAbil.MC1);
      nNVN_MaxMC:
        sText := IntToStr(PlayObject.m_WAbil.MC2);
      nNVN_SC:
        sText := IntToStr(PlayObject.m_WAbil.SC1);
      nNVN_MaxSC:
        sText := IntToStr(PlayObject.m_WAbil.SC2);
      nNVN_EXP:
        sText := IntToStr(PlayObject.m_Abil.Exp);
      nNVN_MaxEXP:
        sText := IntToStr(PlayObject.m_Abil.MaxExp);
      nNVN_HW:
        sText := IntToStr(PlayObject.m_WAbil.HandWeight);
      nNVN_MaxHW:
        sText := IntToStr(PlayObject.m_WAbil.MaxHandWeight);
      nNVN_BW:
        sText := IntToStr(PlayObject.m_WAbil.Weight);
      nNVN_MaxBW:
        sText := IntToStr(PlayObject.m_WAbil.MaxWeight);
      nNVN_AddMaxBW:
        sText := IntToStr(PlayObject.m_dwAddMaxWeight);
      nNVN_WW:
        sText := IntToStr(PlayObject.m_WAbil.WearWeight);
      nNVN_MaxWW:
        sText := IntToStr(PlayObject.m_WAbil.MaxWearWeight);
      nNVN_NGLEVEL:
        sText := IntToStr(PlayObject.m_AbilNG.Level);
      nNVN_NH:
        sText := IntToStr(PlayObject.m_AbilNG.NH);
      nNVN_MaxNH:
        sText := IntToStr(PlayObject.m_AbilNG.MaxNH);
      nNVN_NGExp:
        sText := IntToStr(PlayObject.m_AbilNG.Exp);
      nNVN_NGMaxExp:
        sText := IntToStr(PlayObject.m_AbilNG.MaxExp);
      nNVN_PKPoint:
        sText := IntToStr(PlayObject.m_nPkPoint);
      nNVN_PKPower:
        if PlayObject.m_CurrTarget <> nil then
          sText := IntToStr(PlayObject.m_nLastPKPower);
      nNVN_GoldCount:
        sText := IntToStr(PlayObject.m_nGold);
      nNVN_GameGold:
        sText := IntToStr(PlayObject.m_nGameGold);
      nNVN_GameGoldEX:
        sText := IntToStr(PlayObject.m_nGameGoldEx);
      nNVN_GamePoint:
        sText := IntToStr(PlayObject.m_nGamePoint);
      nNVN_GameGird:
        sText := IntToStr(PlayObject.m_nGameGird);
      nNVN_GameDiamond:
        sText := IntToStr(PlayObject.m_nGameDiamond);
      nNVN_GameGlory:
        sText := IntToStr(PlayObject.m_nGameGlory);
      nNVN_Credit:
        sText := IntToStr(PlayObject.m_nNationCredit);
      nNVN_CreditPoint:
        sText := IntToStr(PlayObject.m_WAbil.CreditPoint);
      nNVN_Element:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[0]);
      nNVN_Element1:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[1]);
      nNVN_Element2:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[2]);
      nNVN_Element3:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[3]);
      nNVN_Element4:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[4]);
      nNVN_Element5:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[5]);
      nNVN_Element6:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[6]);
      nNVN_Element7:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[7]);
      nNVN_Element8:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[8]);
      nNVN_Element9:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[9]);
      nNVN_Element10:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[10]);
      nNVN_Element11:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[11]);
      nNVN_Element12:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[12]);
      nNVN_Element13:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[13]);
      nNVN_Element14:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[14]);
      nNVN_Element15:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[15]);
      nNVN_Element16:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[16]);
      nNVN_Element17:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[17]);
      nNVN_Element18:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[18]);
      nNVN_Element19:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[19]);
      nNVN_Element20:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[20]);
      nNVN_Element21:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[21]);
      nNVN_Element22:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[22]);
      nNVN_Element23:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[23]);
      nNVN_Element24:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[24]);
      nNVN_UnParalysisRate:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[13]);
      nNVN_UnMagicShieldRate:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[14]);
      nNVN_UnRevivalRate:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[15]);
      nNVN_UnPosionRate:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[16]);
      nNVN_UnTammingRate:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[17]);
      nNVN_UnFirecrossRate:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[18]);
      nNVN_UnFrozenRate:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[19]);
      nNVN_UnCobwebwindingRate:
        sText := IntToStr(PlayObject.m_WAbil.NewValue[20]);
      nNVN_UnForeverFrozenRate:
        sText := IntToStr(PlayObject.m_nUnForeverFrozenRate);
      nNVN_RevivalTime:
        begin
          if (PlayObject.m_nCheckRevivalTime * 1000 >= PlayObject.m_nRevivalTime) then
            sText := '0'
          else
            sText := IntToStr(Max(0, (PlayObject.m_nRevivalTime - PlayObject.m_nCheckRevivalTime * 1000) div 1000));
        end;
      nNVN_Hunger:
        sText := IntToStr(PlayObject.GetMyStatus);
      nNVN_LoginTime:
        sText := DateTimeToStr(PlayObject.m_dLogonTime);
      nNVN_LoginLong:
        sText := IntToStr((MyGetTickCount - PlayObject.m_dwLogonTick) div 60000) + '分钟';
      nNVN_AttackHumPowerRate:
        sText := Format('%g', [PlayObject.m_nAttackHumPowerRate / 100]); // 当前攻击人物伤害力倍数 chongchong 2013-12-09
      nNVN_AttackHumPowerRateTime:
        sText := IntToStr(PlayObject.m_dwAttackHumPowerRateTime); // 当前攻击人物伤害力倍数剩余时间 chongchong 2013-12-09
      nNVN_AttackMonPowerRate:
        sText := Format('%g', [PlayObject.m_nAttackMonPowerRate / 100]); // 当前攻击怪物伤害力倍数 chongchong 2013-12-09
      nNVN_AttackMonPowerRateTime:
        sText := IntToStr(PlayObject.m_dwAttackMonPowerRateTime); // 当前攻击怪物伤害力倍数剩余时间 chongchong 2013-12-09
      nNVN_KillMonExpRate:
        sText := Format('%g', [PlayObject.m_nKillMonExpRate / 100]);
      nNVN_KillMonExpRateTime:
        sText := IntToStr(PlayObject.m_dwKillMonExpRateTime);
      nNVN_KillMonBurstRate:
        sText := Format('%g', [PlayObject.m_dwKillMonBurstRate / 100]);
      nNVN_KillMonBurstRateTime:
        sText := IntToStr(PlayObject.m_dwKillMonBurstRateTime);
      nNVN_SaveKillMonBurstRate:
        sText := IntToStr(Integer(PlayObject.m_boSaveKillMonBurstRate));
      // -----------------------------------------------------------------------------------
      nNVN_RandomNO:
        sText := PlayObject.m_sRandomString;
      nNVN_MachineID:
        sText := PlayObject.m_sMachineID;
      nNVN_UserMachineID:
        sText := PlayObject.m_sUserMachineID;
      nNVN_IPLocal:
        sText := PlayObject.m_sIPLocal;
      nNVN_IPaddr:
        sText := PlayObject.m_sIPaddr;
      nNVN_ScreenWidth:
        sText := IntToStr(PlayObject.m_wScreenWidth);
      nNVN_ScreenHeight:
        sText := IntToStr(PlayObject.m_wScreenHeight);
      nNVN_ClientWidth:
        sText := IntToStr(PlayObject.m_nClientWidth);
      nNVN_ClientHeight:
        sText := IntToStr(PlayObject.m_nClientHeight);
      nNVN_CurUseMagicId:
        sText := IntToStr(PlayObject.m_wCurrMagicId);
      nNVN_GetExp:
        sText := IntToStr(PlayObject.m_dwGetExp);
      nNVN_DearName:
        sText := PlayObject.m_sDearName;
      nNVN_Stname:
        sText := PlayObject.m_sMasterName;
      nNVN_HeroName:
        begin
          if Hero <> nil then
            sText := Hero.m_sCharName;
        end;
      nNVN_GuildName:
        begin
          if PlayObject.m_MyGuild <> nil then
            sText := TGUild(PlayObject.m_MyGuild).sGuildName;
        end;
      nNVN_RankName:
        sText := PlayObject.m_sGuildRankName;
      nNVN_GuildMaster1:
        begin
          if PlayObject.m_MyGuild <> nil then
          begin
            TGUild(PlayObject.m_MyGuild).GetGuildMasterName(Master1, Master2);
            sText := Master1;
          end;
        end;
      nNVN_GuildMaster2:
        begin
          if PlayObject.m_MyGuild <> nil then
          begin
            TGUild(PlayObject.m_MyGuild).GetGuildMasterName(Master1, Master2);
            sText := Master2;
          end;
        end;
      nNVN_GuildMemberCount:
        begin
          if PlayObject.m_MyGuild <> nil then
            sText := IntToStr(TGUild(PlayObject.m_MyGuild).Count);
        end;
      nNVN_GamePetName:
        begin
          if (PlayObject.m_MyGamePet <> nil) and (not PlayObject.m_MyGamePet.m_boDeath) and (not PlayObject.m_MyGamePet.m_boGhost) then
            sText := PlayObject.m_sMyGamePetName;
        end;
      nNVN_CurSlaveName:
        sText := DelNumber(PlayObject.CurrentSlaveName);
      nNVN_CurSlaveFullName:
        sText := PlayObject.CurrentSlaveName;
      nNVN_CurSlaveTargetName:
        sText := DelNumber(PlayObject.CurrentSlaveTargetName);
      nNVN_OwnerGuild:
        begin
          sText := '????';
          g_CastleManager.Lock;
          try
            for I := 0 to g_CastleManager.m_CastleList.Count - 1 do
            begin
              UserCastle := TUserCastle(g_CastleManager.m_CastleList.Items[I]);
              if UserCastle.m_MasterGuild <> nil then
              begin
                sText := UserCastle.m_sOwnGuild;
                if sText = '' then
                  sText := '游戏管理';
                Break;
              end;
            end;
          finally
            g_CastleManager.UnLock;
          end;
        end;
      nNVN_GuildBuildPoint:
        begin
          if PlayObject.m_MyGuild = nil then
            sText := '无'
          else
            sText := IntToStr(TGUild(PlayObject.m_MyGuild).nBuildPoint);
        end;
      nNVN_GuildAuraePoint:
        begin
          if PlayObject.m_MyGuild = nil then
            sText := '无'
          else
            sText := IntToStr(TGUild(PlayObject.m_MyGuild).nAurae)
        end;
      nNVN_GuildStabilityPoint:
        begin
          if PlayObject.m_MyGuild = nil then
            sText := '无'
          else
            sText := IntToStr(TGUild(PlayObject.m_MyGuild).nStability)
        end;
      nNVN_GuildMemberMaxLimit:
        begin
          if PlayObject.m_MyGuild = nil then
            sText := '无'
          else
            sText := IntToStr(TGUild(PlayObject.m_MyGuild).m_nMemberMaxLimit);
        end;
      nNVN_GuildFlourishPoint:
        begin
          if PlayObject.m_MyGuild = nil then
            sText := '无'
          else
            sText := IntToStr(TGUild(PlayObject.m_MyGuild).nFlourishing);
        end;
      nNVN_Nation:
        sText := IntToStr(PlayObject.m_btNation);
      nNVN_NationPeople:
        begin
          sText := '0';
          if PlayObject.m_btNation > 0 then
          begin
            NationInfo := g_NationManage.Items[PlayObject.m_btNation];
            if NationInfo <> nil then
              sText := IntToStr(NationInfo.nPeoples);
          end;
        end;
      nNVN_NationName:
        begin
          if PlayObject.m_btNation > 0 then
          begin
            NationInfo := g_NationManage.Items[PlayObject.m_btNation];
            if NationInfo <> nil then
              sText := NationInfo.sName;
          end;
        end;
      nNVN_NationGold:
        begin
          sText := '0';
          if PlayObject.m_btNation > 0 then
          begin
            NationInfo := g_NationManage.Items[PlayObject.m_btNation];
            if NationInfo <> nil then
              sText := IntToStr(NationInfo.nGold);
          end;
        end;
      nNVN_NationBuilding:
        begin
          sText := '0';
          if PlayObject.m_btNation > 0 then
          begin
            NationInfo := g_NationManage.Items[PlayObject.m_btNation];
            if NationInfo <> nil then
              sText := IntToStr(NationInfo.wBuilding);
          end;
        end;
      nNVN_NationArm:
        begin
          sText := '0';
          if PlayObject.m_btNation > 0 then
          begin
            NationInfo := g_NationManage.Items[PlayObject.m_btNation];
            if NationInfo <> nil then
              sText := IntToStr(NationInfo.wArm);
          end;
        end;
      nNVN_NationEconomy:
        begin
          sText := '0';
          if PlayObject.m_btNation > 0 then
          begin
            NationInfo := g_NationManage.Items[PlayObject.m_btNation];
            if NationInfo <> nil then
              sText := IntToStr(NationInfo.wEconomy);
          end;
        end;
      nNVN_NationPolitics:
        begin
          sText := '0';
          if PlayObject.m_btNation > 0 then
          begin
            NationInfo := g_NationManage.Items[PlayObject.m_btNation];
            if NationInfo <> nil then
              sText := IntToStr(NationInfo.wPolitics);
          end;
        end;
      nNVN_NationContribution:
        begin
          sText := '0';
          if PlayObject.m_btNation > 0 then
          begin
            NationInfo := g_NationManage.Items[PlayObject.m_btNation];
            if NationInfo <> nil then
              sText := IntToStr(NationInfo.wContribution);
          end;
        end;
      nNVN_NationMaps:
        begin
          sText := '0';
          if PlayObject.m_btNation > 0 then
          begin
            NationInfo := g_NationManage.Items[PlayObject.m_btNation];
            if NationInfo <> nil then
              sText := IntToStr(NationInfo.btMaps);
          end;
        end;
      nNVN_NationKing:
        begin
          if PlayObject.m_btNation > 0 then
          begin
            NationInfo := g_NationManage.Items[PlayObject.m_btNation];
            if NationInfo <> nil then
              sText := NationInfo.sKingName;
          end;
        end;
      nNVN_CastleName:
        begin
          if m_Castle <> nil then
            sText := TUserCastle(m_Castle).m_sName
          else
            sText := '????';
        end;
      nNVN_SuckDamage:
        sText := IntToStr(PlayObject.m_nSuckDamagePoint);
      nNVN_CastleWarDate:
        begin
          if m_Castle = nil then
          begin
            m_Castle := g_CastleManager.GetCastle(0);
          end;
          sText := '????';
          if m_Castle <> nil then
          begin
            if not TUserCastle(m_Castle).m_boUnderWar then
            begin
              sText := TUserCastle(m_Castle).GetWarDate();
              if sText <> '' then
              begin
                sMsg := sub_49ADB8(nPos, sMsg, '<$CASTLEWARDATE>', sText);
              end
              else
                sMsg := '暂时没有行会攻城！\ \<返回/@main>';
            end
            else
              sMsg := '现正在攻城中！\ \<返回/@main>';
            Exit;
          end;
        end;
      nNVN_ListOfWar:
        begin
          if m_Castle <> nil then
          begin
            sText := TUserCastle(m_Castle).GetAttackWarList();
          end
          else
          begin
            sMsg := '现在没有行会申请攻城战\ \<返回/@main>';
            Exit;
          end;
        end;
      nNVN_CastleChangeDate:
        begin
          if m_Castle <> nil then
            sText := DateTimeToStr(TUserCastle(m_Castle).m_ChangeDate)
          else
            sText := '????';
        end;
      nNVN_CastleWarlastDate:
        begin
          if m_Castle <> nil then
            sText := DateTimeToStr(TUserCastle(m_Castle).m_WarDate)
          else
            sText := '????';
        end;
      nNVN_CastleGetDays:
        begin
          if m_Castle <> nil then
            sText := IntToStr(GetDayCount(Now, TUserCastle(m_Castle).m_ChangeDate))
          else
            sText := '????';
        end;
      nNVN_Lord:
        begin
          sText := '????';
          g_CastleManager.Lock;
          try
            for I := 0 to g_CastleManager.m_CastleList.Count - 1 do
            begin
              UserCastle := TUserCastle(g_CastleManager.m_CastleList.Items[I]);
              if UserCastle.m_MasterGuild <> nil then
              begin
                sText := UserCastle.m_MasterGuild.GetChiefName();
                if sText = '' then
                  sText := '游戏管理';
                Break;
                Break;
              end;
            end;
          finally
            g_CastleManager.UnLock;
          end;
        end;
      nNVN_CurTargetName:
        begin
          if PlayObject.m_CurrTarget <> nil then
          begin
            sText := PlayObject.m_CurrTarget.m_sCharName;
            if not (PlayObject.m_CurrTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
            begin
              sText := DelNumber(sText);
            end;
          end;
        end;
      nNVN_CurTargetMasterName:
        begin
          if (PlayObject.m_CurrTarget <> nil) and (PlayObject.m_CurrTarget.m_Master <> nil) then
          begin
            sText := PlayObject.m_CurrTarget.m_Master.m_sCharName;
          end;
        end;
      nNVN_KillMonName:
        begin
          sText := PlayObject.m_sLastKillMonName;
        end;
      nNVN_KillMonX:
        begin
          if PlayObject.m_CurrTarget <> nil then
            sText := IntToStr(PlayObject.m_CurrTarget.m_nCurrX);
        end;
      nNVN_KillMonY:
        begin
          if PlayObject.m_CurrTarget <> nil then
            sText := IntToStr(PlayObject.m_CurrTarget.m_nCurrY);
        end;
      nNVN_CurTargetFullName:
        begin
          if PlayObject.m_CurrTarget <> nil then
            sText := PlayObject.m_CurrTarget.m_sCharName;
        end;
      nNVN_DieSlaveName:
        sText := PlayObject.CurrentSlaveName;
      nNVN_Killer:
        begin
          if PlayObject.m_LastHiter <> nil then
          begin
            sText := PlayObject.m_LastHiter.m_sCharName;
            if not (PlayObject.m_LastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
            begin
              sText := DelNumber(sText);
            end;
          end;
        end;
      nNVN_MonKiller:
        begin
          if (PlayObject.m_LastHiter <> nil) and (not (PlayObject.m_LastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT])) then
          begin
            sText := PlayObject.m_LastHiter.m_sCharName; // PlayObject.m_sKillerName;
            sText := DelNumber(sText);
          end;
        end;
      nNVN_HumKiller:
        begin
          if (PlayObject.m_LastHiter <> nil) and (PlayObject.m_LastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
          begin
            sText := PlayObject.m_LastHiter.m_sCharName;
          end;
        end;
      nNVN_AttackMonster_Name:
        begin
          if (PlayObject.m_TargetCret <> nil) then
            sText := PlayObject.m_TargetCret.m_sCharName;
        end;
      nNVN_AttackMonster_X:
        begin
          if (PlayObject.m_TargetCret <> nil) then
            sText := IntToStr(PlayObject.m_TargetCret.m_nCurrX);
        end;
      nNVN_AttackMonster_Y:
        begin
          if (PlayObject.m_TargetCret <> nil) then
            sText := IntToStr(PlayObject.m_TargetCret.m_nCurrY);
        end;
      nNVN_AttackMonster_HP:
        begin
          sText := '0';
          if (PlayObject.m_TargetCret <> nil) then
            sText := IntToStr(PlayObject.m_TargetCret.m_WAbil.HP);
        end;
      nNVN_AttackMonster_MaxHP:
        begin
          sText := '0';
          if (PlayObject.m_TargetCret <> nil) then
            sText := IntToStr(PlayObject.m_TargetCret.m_WAbil.MaxHP);
        end;
      nNVN_AttackMonster_Name_Ex:
        begin
          if PlayObject.m_CurrTargetEx <> nil then
            sText := PlayObject.m_CurrTargetEx.m_sCharName;
        end;
      nNVN_AttackMonster_MaxHP_Ex:
        begin
          sText := '0';
          if (PlayObject.m_CurrTargetEx <> nil) then
            sText := IntToStr(PlayObject.m_CurrTargetEx.m_WAbil.MaxHP);
        end;
      nNVN_AttackMonster_HP_Ex:
        begin
          sText := '0';
          if (PlayObject.m_CurrTargetEx <> nil) then
            sText := IntToStr(PlayObject.m_CurrTargetEx.m_WAbil.HP);
        end;
      nNVN_AttackMonster_X_Ex:
        begin
          if (PlayObject.m_CurrTargetEx <> nil) then
            sText := IntToStr(PlayObject.m_CurrTargetEx.m_nCurrX);
        end;
      nNVN_AttackMonster_Y_Ex:
        begin
          if (PlayObject.m_CurrTargetEx <> nil) then
            sText := IntToStr(PlayObject.m_CurrTargetEx.m_nCurrY);
        end;
      nNVN_GroupMemberCount:
        begin
          if (PlayObject.m_GroupOwner <> nil) and (PlayObject.m_GroupOwner.m_GroupMembers <> nil) then
            sText := IntToStr(PlayObject.m_GroupOwner.m_GroupMembers.Count)
          else
            sText := '0';
        end;
      nNVN_BoxItemCount:
        begin
          II := 0;
          for I := Low(PlayObject.m_ItemBoxItems) to High(PlayObject.m_ItemBoxItems) do
          begin
            if PlayObject.m_ItemBoxItems[I] <> 0 then
            begin
              Inc(II);
            end;
          end;
          sText := IntToStr(II);
        end;
      nNVN_DealGoldPlay:
        begin
          PoseHuman := TBaseObject(PlayObject.GetPoseCreate());
          if (PoseHuman <> nil) and (PoseHuman.GetPoseCreate = PlayObject) and (PoseHuman.m_btRaceServer = RC_PLAYOBJECT) then
            sText := PoseHuman.m_sCharName
          else
            sText := '????';
        end;
      nNVN_QueryYBDealLog:
        begin
          if g_GameGoldDealDB.QueryDealLog(PlayObject.m_sCharName, @Dealing) then
          begin
            if Dealing.SellChrName = PlayObject.m_sCharName then
            begin
              sText := '最后一笔出售记录:\  ' + FormatDateTime('dddddd hh时mm分，\', Dealing.SellDateTime);
              sText := sText + Format('  您与%s交易成功，获得了%d个%s。\', [Dealing.BuyChrName, Dealing.GameGold, g_Config.sGameGoldName]);
            end
            else
            begin
              sText := '最后一笔购买记录:\  ' + FormatDateTime('dddddd hh时mm分，\', Dealing.SellDateTime);
              sText := sText + Format('  您与%s交易成功，支付了%d个%s。\', [Dealing.SellChrName, Dealing.GameGold, g_Config.sGameGoldName]);
            end;
          end
          else
            sText := '您未进行任何寄售交易！';
        end;
      nNVN_Dress:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_DRESS].wIndex);
      nNVN_Weapon:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_WEAPON].wIndex);
      nNVN_Righthand:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_RIGHTHAND].wIndex);
      nNVN_Helmet:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_HELMET].wIndex);
      nNVN_Necklace:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_NECKLACE].wIndex);
      nNVN_Ring_R:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_RINGR].wIndex);
      nNVN_Ring_L:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_RINGL].wIndex);
      nNVN_Armring_R:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_ARMRINGR].wIndex);
      nNVN_Armring_L:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_ARMRINGL].wIndex);
      nNVN_Bujuk:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_BUJUK].wIndex);
      nNVN_Belt:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_BELT].wIndex);
      nNVN_Boots:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_BOOTS].wIndex);
      nNVN_Charm:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_CHARM].wIndex);
      nNVN_Hat:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_HAT].wIndex);
      nNVN_Drum:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_DRUM].wIndex);
      nNVN_Shield:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_SHIELD].wIndex);
      nNVN_Horse:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_HORSE].wIndex);
      nNVN_Jade:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_JADE].wIndex);
      nNVN_FashionDress:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_FASHIONDRESS].wIndex);
      nNVN_FashionWeapon:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_FASHIONWEAPON].wIndex);
      nNVN_FashionNecklace:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_FASHIONNECKLACE].wIndex);
      nNVN_FashionHelmet:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_FASHIONHELMET].wIndex);
      nNVN_FashionArmringl:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_FASHIONARMRINGL].wIndex);
      nNVN_FashionArmringr:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_FASHIONARMRINGR].wIndex);
      nNVN_FashionRingl:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_FASHIONRINGL].wIndex);
      nNVN_FashionRingr:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_FASHIONRINGR].wIndex);
      nNVN_FashionRighthand:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_FASHIONRIGHTHAND].wIndex);
      nNVN_FashionBelt:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_FASHIONBELT].wIndex);
      nNVN_FashionBoots:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_FASHIONBOOTS].wIndex);
      nNVN_FashionCharm:
        sText := UserEngine.GetStdItemName(PlayObject.m_UseItems[U_FASHIONCHARM].wIndex);
      nNVN_JewelryItem1:
        sText := UserEngine.GetStdItemName(PlayObject.m_JewelryBoxItems[0].wIndex);
      nNVN_JewelryItem2:
        sText := UserEngine.GetStdItemName(PlayObject.m_JewelryBoxItems[1].wIndex);
      nNVN_JewelryItem3:
        sText := UserEngine.GetStdItemName(PlayObject.m_JewelryBoxItems[2].wIndex);
      nNVN_JewelryItem4:
        sText := UserEngine.GetStdItemName(PlayObject.m_JewelryBoxItems[3].wIndex);
      nNVN_JewelryItem5:
        sText := UserEngine.GetStdItemName(PlayObject.m_JewelryBoxItems[4].wIndex);
      nNVN_JewelryItem6:
        sText := UserEngine.GetStdItemName(PlayObject.m_JewelryBoxItems[5].wIndex);
      nNVN_GodBlessItem1:
        sText := UserEngine.GetStdItemName(PlayObject.m_GodBlessItems[0].wIndex);
      nNVN_GodBlessItem2:
        sText := UserEngine.GetStdItemName(PlayObject.m_GodBlessItems[1].wIndex);
      nNVN_GodBlessItem3:
        sText := UserEngine.GetStdItemName(PlayObject.m_GodBlessItems[2].wIndex);
      nNVN_GodBlessItem4:
        sText := UserEngine.GetStdItemName(PlayObject.m_GodBlessItems[3].wIndex);
      nNVN_GodBlessItem5:
        sText := UserEngine.GetStdItemName(PlayObject.m_GodBlessItems[4].wIndex);
      nNVN_GodBlessItem6:
        sText := UserEngine.GetStdItemName(PlayObject.m_GodBlessItems[5].wIndex);
      nNVN_GodBlessItem7:
        sText := UserEngine.GetStdItemName(PlayObject.m_GodBlessItems[6].wIndex);
      nNVN_GodBlessItem8:
        sText := UserEngine.GetStdItemName(PlayObject.m_GodBlessItems[7].wIndex);
      nNVN_GodBlessItem9:
        sText := UserEngine.GetStdItemName(PlayObject.m_GodBlessItems[8].wIndex);
      nNVN_GodBlessItem10:
        sText := UserEngine.GetStdItemName(PlayObject.m_GodBlessItems[9].wIndex);
      nNVN_GodBlessItem11:
        sText := UserEngine.GetStdItemName(PlayObject.m_GodBlessItems[10].wIndex);
      nNVN_GodBlessItem12:
        sText := UserEngine.GetStdItemName(PlayObject.m_GodBlessItems[11].wIndex);
      nNVN_G_Dress:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_DRESS]);
      nNVN_G_Weapon:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_WEAPON]);
      nNVN_G_Righthand:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_RIGHTHAND]);
      nNVN_G_Helmet:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_HELMET]);
      nNVN_G_Necklace:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_NECKLACE]);
      nNVN_G_Ring_R:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_RINGR]);
      nNVN_G_Ring_L:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_RINGL]);
      nNVN_G_Armring_R:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_ARMRINGR]);
      nNVN_G_Armring_L:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_ARMRINGL]);
      nNVN_G_Bujuk:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_BUJUK]);
      nNVN_G_Belt:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_BELT]);
      nNVN_G_Boots:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_BOOTS]);
      nNVN_G_Charm:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_CHARM]);
      nNVN_G_Hat:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_HAT]);
      nNVN_G_Drum:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_DRUM]);
      nNVN_G_Shield:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_SHIELD]);
      nNVN_G_Horse:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_HORSE]);
      nNVN_G_Jade:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_JADE]);
      nNVN_G_FashionDress:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_FASHIONDRESS]);
      nNVN_G_FashionWeapon:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_FASHIONWEAPON]);
      nNVN_G_FashionNecklace:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_FASHIONNECKLACE]);
      nNVN_G_FashionHelmet:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_FASHIONHELMET]);
      nNVN_G_FashionArmringL:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_FASHIONARMRINGL]);
      nNVN_G_FashionArmringR:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_FASHIONARMRINGR]);
      nNVN_G_FashionRingL:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_FASHIONRINGL]);
      nNVN_G_FashionRingR:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_FASHIONRINGR]);
      nNVN_G_FashionRighthand:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_FASHIONRIGHTHAND]);
      nNVN_G_FashionBelt:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_FASHIONBELT]);
      nNVN_G_FashionBoots:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_FASHIONBOOTS]);
      nNVN_G_FashionCharm:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_UseItems[U_FASHIONCHARM]);
      nNVN_G_JewelryItem1:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_JewelryBoxItems[0]);
      nNVN_G_JewelryItem2:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_JewelryBoxItems[1]);
      nNVN_G_JewelryItem3:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_JewelryBoxItems[2]);
      nNVN_G_JewelryItem4:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_JewelryBoxItems[3]);
      nNVN_G_JewelryItem5:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_JewelryBoxItems[4]);
      nNVN_G_JewelryItem6:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_JewelryBoxItems[5]);
      nNVN_G_GodblessItem1:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_GodBlessItems[0]);
      nNVN_G_GodblessItem2:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_GodBlessItems[1]);
      nNVN_G_GodblessItem3:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_GodBlessItems[2]);
      nNVN_G_GodblessItem4:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_GodBlessItems[3]);
      nNVN_G_GodblessItem5:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_GodBlessItems[4]);
      nNVN_G_GodblessItem6:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_GodBlessItems[5]);
      nNVN_G_GodblessItem7:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_GodBlessItems[6]);
      nNVN_G_GodblessItem8:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_GodBlessItems[7]);
      nNVN_G_GodblessItem9:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_GodBlessItems[8]);
      nNVN_G_GodblessItem10:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_GodBlessItems[9]);
      nNVN_G_GodblessItem11:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_GodBlessItems[10]);
      nNVN_G_GodblessItem12:
        sText := UserEngine.GetStdItemChangeName(@PlayObject.m_GodBlessItems[11]);
      // ------------------------------------------------------------------------
      nNVN_StoneUpgrade_Name:
        begin
          if PlayObject.m_UpgradeDialogUserItem.wIndex > 0 then
          begin
            StdItem := UserEngine.GetStdItem(PlayObject.m_UpgradeDialogUserItem.wIndex);
            if StdItem <> nil then
            begin
              if PlayObject.m_UpgradeDialogUserItem.btValue[13] = 1 then
                sText := PlayObject.m_UpgradeDialogUserItem.Name;
              if sText = '' then
                sText := StdItem.Name;
            end;
          end;
        end;
      nNVN_StoneUpgrade_StdMode:
        begin
          if PlayObject.m_UpgradeDialogUserItem.wIndex > 0 then
          begin
            StdItem := UserEngine.GetStdItem(PlayObject.m_UpgradeDialogUserItem.wIndex);
            if StdItem <> nil then
              sText := IntToStr(StdItem.StdMode);
          end;
        end;
      nNVN_StoneUpgrade_Dura:
        begin
          if PlayObject.m_UpgradeDialogUserItem.wIndex > 0 then
            sText := IntToStr(PlayObject.m_UpgradeDialogUserItem.Dura);
        end;
      nNVN_StoneUpgrade_DuraMax:
        begin
          if PlayObject.m_UpgradeDialogUserItem.wIndex > 0 then
            sText := IntToStr(PlayObject.m_UpgradeDialogUserItem.DuraMax);
        end;
      nNVN_StoneUpgrade_UpgCount:
        begin
          if PlayObject.m_UpgradeDialogUserItem.wIndex > 0 then
            sText := IntToStr(PlayObject.m_UpgradeDialogUserItem.btUpgradeCount);
        end;
      nNVN_DlgItem_Name:
        begin
          if PlayObject.m_UpgradeStdItem.Name <> '' then
            sText := PlayObject.m_UpgradeStdItem.Name
          else
            sText := '????';
        end;
      nNVN_DlgItem_Index:
        begin
          if PlayObject.m_UpgradeItem <> nil then
            sText := IntToStr(PlayObject.m_UpgradeItem.wIndex)
          else
            sText := '????';
        end;
      nNVN_DlgItem_MakeIndex:
        begin
          if PlayObject.m_UpgradeItem <> nil then
            sText := IntToStr(PlayObject.m_UpgradeItem.MakeIndex)
          else
            sText := '????';
        end;
      nNVN_DlgItem_StdMode:
        begin
          if PlayObject.m_UpgradeStdItem.Name <> '' then
            sText := IntToStr(PlayObject.m_UpgradeStdItem.StdMode)
          else
            sText := '????';
        end;
      nNVN_DlgItem_Dura:
        begin
          if PlayObject.m_UpgradeItem <> nil then
            sText := IntToStr(PlayObject.m_UpgradeItem.Dura)
          else
            sText := '????';
        end;
      nNVN_DlgItem_DuraMax:
        begin
          if PlayObject.m_UpgradeStdItem.Name <> '' then
            sText := IntToStr(PlayObject.m_UpgradeStdItem.DuraMax)
          else
            sText := '????';
        end;
      nNVN_PickDropItemName:
        sText := PlayObject.m_sPickUpOrDropItemName;
      nNVN_PickDropItem:
        sText := Format('{[%s]/%d}', [PlayObject.m_sPickUpOrDropItemName, PlayObject.m_PickUpOrDropItem.MakeIndex]);
      nNVN_PickDropItemMakeIndex:
        sText := IntToStr(PlayObject.m_nPickUpOrDropItemMakeIndex);
      nNVN_DropInsuranceItemName:
        sText := PlayObject.m_sDropInsuranceItemName;
      nNVN_DropInsuranceItemCount:
        sText := IntToStr(PlayObject.m_nDropInsuranceItemCount);
      nNVN_DropInsuranceItemCurrency:
        sText := IntToStr(PlayObject.m_nDropInsuranceItemCurrency);
      nNVN_DropInsuranceItemGold:
        sText := IntToStr(PlayObject.m_nDropInsuranceItemGold);
      nNVN_ScatterItemName:
        sText := PlayObject.m_sScatterItemName;
      nNVN_ScatterItemX:
        sText := IntToStr(PlayObject.m_nScatterItemX);
      nNVN_ScatterItemY:
        sText := IntToStr(PlayObject.m_nScatterItemY);
      nNVN_ShowItem:
        begin
          PlayObject.m_NeedFoundryItemList.Clear;
          FoundryItem := GetFoundryItem(PlayObject.m_sNpcSelectItemName);
          if FoundryItem <> nil then
          begin
            sText := '';
            for I := 0 to FoundryItem.ItemList.Count - 1 do
            begin
              FoundryNeedItem := FoundryItem.ItemList.Items[I];
              PlayObject.m_NeedFoundryItemList.Add(Format('%s%s%s', [AddSpace(FoundryNeedItem.sItemName, 16), AddSpace(IntToStr(FoundryNeedItem.nItemCount), 6), AddSpace(BooleanToStr(FoundryNeedItem.btDelete = 1), 9)]));
              // sText := sText + FoundryNeedItem.sItemName + ' ' + IntToStr(FoundryNeedItem.nItemCount) + '\';
            end;
            if PlayObject.m_NeedFoundryItemList.Count < 2 then
            begin
              sText := '<' + AddSpace('名称', 16) + AddSpace('数量', 6) + AddSpace('是否消失', 6) + '/FCOLOR=249>\';
              for I := 0 to PlayObject.m_NeedFoundryItemList.Count - 1 do
              begin
                sText := sText + PlayObject.m_NeedFoundryItemList.Strings[I] + '\';
              end;
            end
            else
            begin
              sText := '<' + AddSpace('名称', 16) + AddSpace('数量', 6) + AddSpace('是否消失', 6) + '/FCOLOR=249>|<' + AddSpace('名称', 16) + AddSpace('数量', 6) + AddSpace('是否消失', 6) + '/FCOLOR=249>\';
              n18 := 0;
              for I := 0 to PlayObject.m_NeedFoundryItemList.Count - 1 do
              begin
                sText := sText + PlayObject.m_NeedFoundryItemList.Strings[I];
                Inc(n18);
                if n18 >= 2 then
                begin
                  sText := sText + '\';
                  n18 := 0;
                end;
              end;
            end;
          end;
        end;
      nNVN_AuctionItemName:
        sText := PlayObject.m_sAuctionItemName;
      nNVN_AuctionItemHumanName:
        sText := PlayObject.m_sAuctionItemHumanName; // 物品拍卖者
      nNVN_AuctionItemBidHumanName:
        sText := PlayObject.m_sAuctionItemBidHumanName; // 竞拍出价者
      nNVN_AuctionItemStartPrice:
        sText := IntToStr(PlayObject.m_nAuctionItemStartPrice);
      nNVN_AuctionItemSellPrice:
        sText := IntToStr(PlayObject.m_nAuctionItemSellPrice);
      nNVN_AuctionItemInvalidPrice:
        sText := IntToStr(PlayObject.m_nAuctionItemInvalidPrice);
      nNVN_AuctionItemFinaPrice:
        sText := IntToStr(PlayObject.m_nAuctionItemFinaPrice);
      nNVN_AuctionItemMoneyTypeValue:
        sText := IntToStr(PlayObject.m_nAuctionItemMoneyType);
      nNVN_AuctionItemMoneyType:
        begin
          case PlayObject.m_nAuctionItemMoneyType of
            0:
              sText := g_Config.sGameGoldName;
            1:
              sText := g_Config.sGamePointName;
            2:
              sText := sSTRING_GOLDNAME;
            3:
              sText := g_Config.sGameDiamondName;
            4:
              sText := g_Config.sGameGirdName;
          end;
        end;
      nNVN_AuctionItemSelled:
        sText := IntToStr(Integer(PlayObject.m_boAuctionItemSelled));
      NNVN_LearnMagicID:
        sText := IntToStr(PlayObject.m_nLearnMagicID);
      NNVN_NpcRebornCount:
        sText := IntToStr(PlayObject.m_nNpcRebornCount);
      NNVN_TriggerNpcRebornCount:
        sText := IntToStr(PlayObject.m_nTriggerNpcRebornCount);
      NNVN_NGAddPower:
        if PlayObject.m_boTrainingNG then
          sText := IntToStr(PlayObject.GetNGAddPower);
      NNVN_NGDecPower:
        if PlayObject.m_boTrainingNG then
          sText := IntToStr(PlayObject.GetNGDecPower);
      NNVN_EXPIREDITEMNAME:
        sText := PlayObject.m_sExpiredItemName;
      NNVN_CustomButtonID:
        sText := IntToStr(PlayObject.m_nCustomButtonID);
      nNVN_UpgradeCount:
        begin
          n18 := 0;
          for I := Low(THumanUseItems) to High(THumanUseItems) do
          begin
            UserItem := @PlayObject.m_UseItems[I];
            if (UserItem.wIndex > 0) then
              Inc(n18, UserItem.btUpgradeCount);
          end;
          for I := Low(THumanJewelryBoxItems) to High(THumanJewelryBoxItems) do
          begin
            UserItem := @PlayObject.m_JewelryBoxItems[I];
            if (UserItem.wIndex > 0) then
              Inc(n18, UserItem.btUpgradeCount);
          end;
          for I := Low(THumanGodBlessItems) to High(THumanGodBlessItems) do
          begin
            UserItem := @PlayObject.m_GodBlessItems[I];
            if (UserItem.wIndex > 0) then
              Inc(n18, UserItem.btUpgradeCount);
          end;
          sText := IntToStr(n18);
        end;
      nNVN_StruckHP:
        begin
          sText := IntToStr(PlayObject.m_nStruckHP);
        end;
      nNVN_RecallRemainingTime:
        begin
          if PlayObject.m_boTimeRecall then
          begin
            sText := IntToStr(Round((PlayObject.m_dwTimeRecallTick - MyGetTickCount) / 1000));
          end;
        end;
      nNVN_CombatPower:
        sText := IntToStr(PlayObject.m_nCombatPower);
      nNVN_ExtBagPageCount:
        sText := IntToStr(PlayObject.m_btExtBagPageCount);
      nNVN_ExtBagOpenItemCount:
        sText := IntToStr(PlayObject.m_btExtBagOpenItemCount);
      nNVN_ExtBagCloseItemCount:
        sText := IntToStr(PlayObject.m_btExtBagPageCount * MAX_EXT_ONE_PAGE_ITEM_COUNT - PlayObject.m_btExtBagOpenItemCount);
      nNVN_SelectGamePetIndex:
        begin
          sText := IntToStr(PlayObject.m_nSelectGamePetIndex);
        end;
      nNVN_SelectGamePetName:
        begin
          sText := PlayObject.m_sSelectGamePetName;
        end;
      nNVN_GamePetLevel:
        begin
          if PlayObject.m_MyGamePet <> nil then
            sText := IntToStr(PlayObject.m_MyGamePet.m_Abil.Level);
        end;
      nNVN_GamePetCount:
        begin
          sText := IntToStr(PlayObject.m_GamePetList.Count);
        end;
      nNVN_GamePetMagicID:
        begin
          sText := IntToStr(PlayObject.m_nGamePetMagicID);
        end;
      nNVN_GamePetMagicName:
        begin
          sText := PlayObject.m_sGamePetMagicName;
        end;
      nNVN_GamePetMagicIndex:
        begin
          sText := IntToStr(PlayObject.m_nGamePetMagicIndex);
        end;
      nNVN_UseStoneItemIndex:
        sText := IntToStr(PlayObject.m_UseStoneItemIndex);
      nNVN_UserStateName:
        sText := PlayObject.m_sLastQueryUserName;
      nNVN_PlayerBuyUser:
        begin
          sText := PlayObject.m_sPlayerBuyUser;
        end;
      nNVN_PlayerBuyUserDelegater:
        begin
          sText := PlayObject.m_sPlayerBuyUserDelegater;
        end;
      nNVN_PlayerBuyPrices:
        begin
          sText := IntToStr(PlayObject.m_nPlayerBuyPrices);
        end;
      nNVN_PlayerBuyMoneyType:
        begin
          sText := PlayObject.m_sPlayerBuyMoneyType;
        end;
      nNVN_PlayerBuyMoneyTypeValue:
        begin
          sText := IntToStr(PlayObject.m_btPlayerBuyMoneyTypeValue);
        end;
      nNVN_SellPlayDelegater:
        begin
          if PlayObject.m_WantAddSellPlayerInfo <> nil then
            sText := PlayObject.m_WantAddSellPlayerInfo.Delegater
          else
            sText := '';
        end;
      nNVN_SellPlayMoneyType:
        begin
          if PlayObject.m_WantAddSellPlayerInfo <> nil then
            sText := IntToStr(PlayObject.m_WantAddSellPlayerInfo.SellPricesType + 1)
          else
            sText := '';
        end;
      nNVN_SellPlayMoneyValue:
        begin
          if PlayObject.m_WantAddSellPlayerInfo <> nil then
            sText := IntToStr(PlayObject.m_WantAddSellPlayerInfo.SellPrices)
          else
            sText := '';
        end;
      nNVN_UseItemFrom:
        begin
          sText := IntToStr(Integer(PlayObject.m_UseItemFrom));
        end;
      nNVN_UseItemMakeIndex:
        begin
          sText := IntToStr(PlayObject.m_nUseItemMakeIndex);
        end;
      nNVN_UseItemName:
        begin
          sText := PlayObject.m_sUseItemName;
        end;
      nNVN_G_UseItemName:
        begin
          sText := PlayObject.m_sUseItemNewName;
        end;
      nNVN_BagItemMakeIndex:
        begin
          sText := IntToStr(PlayObject.m_nBagItemMakeIndex);
        end;
      nNVN_BagItemName:
        begin
          sText := PlayObject.m_sBagItemName;
        end;
      nNVN_G_BagItemName:
        begin
          sText := PlayObject.m_sBagItemNewName;
        end;
      nNVN_SlaveX:
        begin
          sText := '';
          for II := 0 to PlayObject.m_SlaveList.Count - 1 do
          begin
            TempObject := PlayObject.m_SlaveList.Items[II];
            if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) then
            begin
              sText := IntToStr(TempObject.m_nCurrX);
              Break;
            end;
          end;
        end;
      nNVN_SlaveY:
        begin
          sText := '';
          for II := 0 to PlayObject.m_SlaveList.Count - 1 do
          begin
            TempObject := PlayObject.m_SlaveList.Items[II];
            if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) then
            begin
              sText := IntToStr(TempObject.m_nCurrY);
              Break;
            end;
          end;
        end;
      nNVN_SlaveTargetX:
        begin
          sText := '';
          for II := 0 to PlayObject.m_SlaveList.Count - 1 do
          begin
            TempObject := PlayObject.m_SlaveList.Items[II];
            if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) and (TempObject.m_boTarget) and (TempObject.m_TargetCret <> nil) then
            begin
              sText := IntToStr(TempObject.m_TargetCret.m_nCurrX);
              Break;
            end;
          end;
          if sText = '' then
          begin
            for II := 0 to PlayObject.m_SlaveList.Count - 1 do
            begin
              TempObject := PlayObject.m_SlaveList.Items[II];
              if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) and (TempObject.m_TargetCret <> nil) then
              begin
                sText := IntToStr(TempObject.m_TargetCret.m_nCurrX);
                Break;
              end;
            end;
          end;
        end;
      nNVN_SlaveTargetY:
        begin
          sText := '';
          for II := 0 to PlayObject.m_SlaveList.Count - 1 do
          begin
            TempObject := PlayObject.m_SlaveList.Items[II];
            if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) and (TempObject.m_boTarget) and (TempObject.m_TargetCret <> nil) then
            begin
              sText := IntToStr(TempObject.m_TargetCret.m_nCurrY);
              Break;
            end;
          end;
          if sText = '' then
          begin
            for II := 0 to PlayObject.m_SlaveList.Count - 1 do
            begin
              TempObject := PlayObject.m_SlaveList.Items[II];
              if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) and (TempObject.m_TargetCret <> nil) then
              begin
                sText := IntToStr(TempObject.m_TargetCret.m_nCurrY);
                Break;
              end;
            end;
          end;
        end;
      nNVN_SlaveCount:
        begin
          nIndex := 0;
          for II := 0 to PlayObject.m_SlaveList.Count - 1 do
          begin
            TempObject := PlayObject.m_SlaveList.Items[II];
            if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) then
            begin
              Inc(nIndex);
            end;
          end;
          sText := IntToStr(nIndex);
        end;
      nNVN_HumCloneCount:
        begin
          nIndex := 0;
          for II := 0 to PlayObject.m_SlaveList.Count - 1 do
          begin
            TempObject := PlayObject.m_SlaveList.Items[II];
            if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) and (TempObject is TCopyMon) then
            begin
              Inc(nIndex);
            end;
          end;
          sText := IntToStr(nIndex);
        end;
      nNVN_CurItemName:
        begin
          sText := PlayObject.m_sCurrentItemName;
        end;
      nNVN_DamageValue:
        begin
          sText := IntToStr(PlayObject.m_nLastDamageValue);
        end;
      nNVN_CurrentItemMakeIndex:
        begin
          sText := IntToStr(PlayObject.m_nCurrentItemMakeIndex);
        end;
      nNVN_CurrentItemPos:
        begin
          sText := IntToStr(PlayObject.m_nCurrentItemPos);
        end;
      nNVN_G_CurrentItemName:
        begin
          sText := PlayObject.m_sCurrentItemNewName;
        end;
      nNVN_CurrentItemOverlapCount:
        begin
          sText := IntToStr(PlayObject.m_CurrentItemOverlapCount);
        end;
      nNVN_MagicID:
        begin
          sText := IntToStr(PlayObject.m_nSpellMagicID);
        end;
      nNVN_MagicName:
        begin
          sText := PlayObject.m_sSpellMagicName;
        end;
      nNVN_MagicTarget:
        begin
          sText := PlayObject.m_sSpellMagicTarget;
        end;
      nNVN_MagicTargetRace:
        begin
          sText := IntToStr(PlayObject.m_sSpellMagicTargetRace);
        end;
      nNVN_CurItemMoneyTypeValue:
        begin
          sText := IntToStr(PlayObject.m_btCurItemMoneyTypeValue);
        end;
      nNVN_CurItemMoneyType:
        begin
          sText := PlayObject.m_sCurItemMoneyType;
        end;
      nNVN_CurItemPrices:
        begin
          sText := IntToStr(PlayObject.m_nCurItemPrices);
        end;
      nNVN_CurUserName:
        begin
          sText := PlayObject.m_sCurrentOperateUserName;
        end;
      nNVN_RegMonName:
        begin
          sText := PlayObject.m_sRegMonName;
        end;
      nNVN_RegMonMap:
        begin
          sText := PlayObject.m_sRegMonMap;
        end;
      nNVN_RegMonMapDesc:
        begin
          sText := PlayObject.m_sRegMonMapDesc;
        end;
      nNVN_RegMonX:
        begin
          sText := IntToStr(PlayObject.m_nRegMonX);
        end;
      nNVN_RegMonY:
        begin
          sText := IntToStr(PlayObject.m_nRegMonY);
        end;
      nNVN_JoinGuildHuman:
        begin
          sText := PlayObject.m_sJoinGuildUserName;
        end;
      nNVN_UserShopName:
        begin
          g_M2DataDB.UserShopDB.GetUserShopInfo(PlayObject.m_sCharName, UserShop);
          sText := UserShop.sShopName;
        end;
      nNVN_BeadExp:
        begin
          sText := IntToStr(PlayObject.m_dwBeadExp);
        end;
      nNVN_BeadExpSource:
        begin
          sText := IntToStr(PlayObject.m_nBeadSource);
        end;

      nNVN_H_Gender:
        if Hero <> nil then
          sText := IntToStr(Hero.m_btGender);
      nNVN_H_Job:
        if Hero <> nil then
          sText := IntToStr(Hero.m_btJob);
      nNVN_H_Map:
        if Hero <> nil then
          sText := Hero.m_PEnvir.sMapName;
      nNVN_H_MapTitle:
        if Hero <> nil then
          sText := Hero.m_PEnvir.sMapDesc;
      nNVN_H_MapName:
        if Hero <> nil then
          sText := Hero.m_PEnvir.sMapDesc;
      nNVN_H_X:
        if Hero <> nil then
          sText := IntToStr(Hero.m_nCurrX);
      nNVN_H_Y:
        if Hero <> nil then
          sText := IntToStr(Hero.m_nCurrY);
      nNVN_H_Level:
        if Hero <> nil then
          sText := IntToStr(Hero.m_Abil.Level);
      nNVN_H_Relevel:
        if Hero <> nil then
          sText := IntToStr(Hero.m_btReLevel);
      nNVN_H_Hit:
        if Hero <> nil then
          sText := IntToStr(Hero.m_btHitPoint);
      nNVN_H_Spd:
        if Hero <> nil then
          sText := IntToStr(Hero.m_btSpeedPoint);
      nNVN_H_HitSpd:
        if Hero <> nil then
          sText := IntToStr(Hero.m_nHitSpeed);
      nNVN_H_HP:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.HP);
      nNVN_H_MaxHP:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.MaxHP);
      nNVN_H_MP:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.MP);
      nNVN_H_MaxMP:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.MaxMP);
      nNVN_H_AC:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.AC1);
      nNVN_H_MaxAC:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.AC2);
      nNVN_H_MAC:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.Mac1);
      nNVN_H_MaxMAC:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.MAC2);
      nNVN_H_DC:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.DC1);
      nNVN_H_MaxDC:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.DC2);
      nNVN_H_MC:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.MC1);
      nNVN_H_MaxMC:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.MC2);
      nNVN_H_SC:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.SC1);
      nNVN_H_MaxSC:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.SC2);
      nNVN_H_EXP:
        if Hero <> nil then
          sText := IntToStr(Hero.m_Abil.Exp);
      nNVN_H_MaxEXP:
        if Hero <> nil then
          sText := IntToStr(Hero.m_Abil.MaxExp);
      nNVN_H_HW:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.HandWeight);
      nNVN_H_MaxHW:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.MaxHandWeight);
      nNVN_H_BW:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.Weight);
      nNVN_H_MaxBW:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.MaxWeight);
      nNVN_H_WW:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.WearWeight);
      nNVN_H_MaxWW:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.MaxWearWeight);
      nNVN_H_NGLevel:
        if Hero <> nil then
          sText := IntToStr(Hero.m_AbilNG.Level);
      nNVN_H_NH:
        if Hero <> nil then
          sText := IntToStr(Hero.m_AbilNG.NH);
      nNVN_H_MaxNH:
        if Hero <> nil then
          sText := IntToStr(Hero.m_AbilNG.MaxNH);
      nNVN_H_NGExp:
        if Hero <> nil then
          sText := IntToStr(Hero.m_AbilNG.Exp);
      nNVN_H_NGMaxExp:
        if Hero <> nil then
          sText := IntToStr(Hero.m_AbilNG.MaxExp);
      nNVN_H_PKPoint:
        if Hero <> nil then
          sText := IntToStr(TSmartObject(Hero).m_nPkPoint);
      nNVN_H_PKPower:
        if Hero <> nil then
          sText := IntToStr(Hero.m_nLastPKPower);
      nNVN_H_Element:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[0]);
      nNVN_H_Element1:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[1]);
      nNVN_H_Element2:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[2]);
      nNVN_H_Element3:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[3]);
      nNVN_H_Element4:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[4]);
      nNVN_H_Element5:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[5]);
      nNVN_H_Element6:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[6]);
      nNVN_H_Element7:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[7]);
      nNVN_H_Element8:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[8]);
      nNVN_H_Element9:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[9]);
      nNVN_H_Element10:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[10]);
      nNVN_H_Element11:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[11]);
      nNVN_H_Element12:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[12]);
      nNVN_H_Element13:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[13]);
      nNVN_H_Element14:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[14]);
      nNVN_H_Element15:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[15]);
      nNVN_H_Element16:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[16]);
      nNVN_H_Element17:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[17]);
      nNVN_H_Element18:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[18]);
      nNVN_H_Element19:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[19]);
      nNVN_H_Element20:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[20]);
      nNVN_H_Element21:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[21]);
      nNVN_H_Element22:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[22]);
      nNVN_H_Element23:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[23]);
      nNVN_H_Element24:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[24]);
      nNVN_H_UnParalysisRate:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[13]);
      nNVN_H_UnMagicShieldRate:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[14]);
      nNVN_H_UnRevivalRate:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[15]);
      nNVN_H_UnPosionRate:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[16]);
      nNVN_H_UnTammingRate:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[17]);
      nNVN_H_UnFirecrossRate:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[18]);
      nNVN_H_UnFrozenRate:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[19]);
      nNVN_H_UnCobwebwindingRate:
        if Hero <> nil then
          sText := IntToStr(Hero.m_WAbil.NewValue[20]);
      nNVN_H_UnForeverFrozenRate:
        if Hero <> nil then
          sText := IntToStr(Hero.m_nUnForeverFrozenRate);
      nNVN_H_RevivalTime:
        begin
          if Hero <> nil then
          begin
            if (Hero.m_nCheckRevivalTime * 1000 >= Hero.m_nRevivalTime) then
              sText := '0'
            else
              sText := IntToStr(Max(0, (Hero.m_nRevivalTime - Hero.m_nCheckRevivalTime * 1000) div 1000));
          end;
        end;
      nNVN_H_CurUseMagicId:
        if Hero <> nil then
          sText := IntToStr(Hero.m_wCurrMagicId);
      nNVN_H_GetExp:
        if Hero <> nil then
          sText := IntToStr(Hero.m_dwGetExp);
      nNVN_H_SuckDamage:
        if Hero <> nil then
          sText := IntToStr(Hero.m_nSuckDamagePoint);
      nNVN_H_CurTargetName:
        begin
          if (Hero <> nil) and (Hero.m_CurrTarget <> nil) then
          begin
            sText := Hero.m_CurrTarget.m_sCharName;
            if not (Hero.m_CurrTarget.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
            begin
              sText := DelNumber(sText);
            end;
          end;
        end;
      nNVN_H_CurTargetFullName:
        begin
          if (Hero <> nil) and (Hero.m_CurrTarget <> nil) then
          begin
            sText := Hero.m_CurrTarget.m_sCharName;
          end;
        end;
      nNVN_H_KillMonName:
        if Hero <> nil then
          sText := Hero.m_sLastKillMonName;
      nNVN_H_KillMonX:
        begin
          if (Hero <> nil) and (Hero.m_CurrTarget <> nil) then
            sText := IntToStr(Hero.m_CurrTarget.m_nCurrX);
        end;
      nNVN_H_KillMonY:
        begin
          if (Hero <> nil) and (Hero.m_CurrTarget <> nil) then
            sText := IntToStr(Hero.m_CurrTarget.m_nCurrY);
        end;
      nNVN_H_AttackMonster_Name:
        begin
          if (Hero <> nil) and (Hero.m_CurrTarget <> nil) then
            sText := Hero.m_CurrTarget.m_sCharName;
        end;
      nNVN_H_AttackMonster_X:
        begin
          if (Hero <> nil) and (Hero.m_CurrTarget <> nil) then
            sText := IntToStr(Hero.m_CurrTarget.m_nCurrX);
        end;
      nNVN_H_AttackMonster_Y:
        begin
          if (Hero <> nil) and (Hero.m_CurrTarget <> nil) then
            sText := IntToStr(Hero.m_CurrTarget.m_nCurrY);
        end;
      nNVN_H_AttackMonster_HP:
        begin
          if (Hero <> nil) and (Hero.m_CurrTarget <> nil) then
            sText := IntToStr(Hero.m_CurrTarget.m_WAbil.HP);
        end;
      nNVN_H_AttackMonster_MaxHP:
        begin
          if (Hero <> nil) and (Hero.m_CurrTarget <> nil) then
            sText := IntToStr(Hero.m_CurrTarget.m_WAbil.MaxHP);
        end;
      nNVN_H_Dress:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_DRESS].wIndex);
      nNVN_H_Weapon:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_WEAPON].wIndex);
      nNVN_H_Righthand:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_RIGHTHAND].wIndex);
      nNVN_H_Helmet:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_HELMET].wIndex);
      nNVN_H_Necklace:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_NECKLACE].wIndex);
      nNVN_H_Ring_R:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_RINGR].wIndex);
      nNVN_H_Ring_L:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_RINGL].wIndex);
      nNVN_H_Armring_R:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_ARMRINGR].wIndex);
      nNVN_H_Armring_L:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_ARMRINGL].wIndex);
      nNVN_H_Bujuk:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_BUJUK].wIndex);
      nNVN_H_Belt:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_BELT].wIndex);
      nNVN_H_Boots:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_BOOTS].wIndex);
      nNVN_H_Charm:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_CHARM].wIndex);
      nNVN_H_Hat:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_HAT].wIndex);
      nNVN_H_Drum:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_DRUM].wIndex);
      nNVN_H_Shield:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_SHIELD].wIndex);
      nNVN_H_Horse:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_HORSE].wIndex);
      nNVN_H_Jade:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_JADE].wIndex);
      nNVN_H_FashionDress:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_FASHIONDRESS].wIndex);
      nNVN_H_FashionWeapon:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_FASHIONWEAPON].wIndex);
      nNVN_H_FashionNecklace:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_FASHIONNECKLACE].wIndex);
      nNVN_H_FashionHelmet:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_FASHIONHELMET].wIndex);
      nNVN_H_FashionArmringL:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_FASHIONARMRINGL].wIndex);
      nNVN_H_FashionArmringR:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_FASHIONARMRINGR].wIndex);
      nNVN_H_FashionRingL:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_FASHIONRINGL].wIndex);
      nNVN_H_FashionRingR:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_FASHIONRINGR].wIndex);
      nNVN_H_FashionRighthand:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_FASHIONRIGHTHAND].wIndex);
      nNVN_H_FashionBelt:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_FASHIONBELT].wIndex);
      nNVN_H_FashionBoots:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_FASHIONBOOTS].wIndex);
      nNVN_H_FashionCharm:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_UseItems[U_FASHIONCHARM].wIndex);
      nNVN_H_JewelryItem1:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_JewelryBoxItems[0].wIndex);
      nNVN_H_JewelryItem2:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_JewelryBoxItems[1].wIndex);
      nNVN_H_JewelryItem3:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_JewelryBoxItems[2].wIndex);
      nNVN_H_JewelryItem4:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_JewelryBoxItems[3].wIndex);
      nNVN_H_JewelryItem5:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_JewelryBoxItems[4].wIndex);
      nNVN_H_JewelryItem6:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_JewelryBoxItems[5].wIndex);
      nNVN_H_GodBlessItem1:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_GodBlessItems[0].wIndex);
      nNVN_H_GodBlessItem2:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_GodBlessItems[1].wIndex);
      nNVN_H_GodBlessItem3:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_GodBlessItems[2].wIndex);
      nNVN_H_GodBlessItem4:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_GodBlessItems[3].wIndex);
      nNVN_H_GodBlessItem5:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_GodBlessItems[4].wIndex);
      nNVN_H_GodBlessItem6:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_GodBlessItems[5].wIndex);
      nNVN_H_GodBlessItem7:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_GodBlessItems[6].wIndex);
      nNVN_H_GodBlessItem8:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_GodBlessItems[7].wIndex);
      nNVN_H_GodBlessItem9:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_GodBlessItems[8].wIndex);
      nNVN_H_GodBlessItem10:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_GodBlessItems[9].wIndex);
      nNVN_H_GodBlessItem11:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_GodBlessItems[10].wIndex);
      nNVN_H_GodBlessItem12:
        if Hero <> nil then
          sText := UserEngine.GetStdItemName(Hero.m_GodBlessItems[11].wIndex);
      nNVN_H_G_Dress:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_DRESS]);
      nNVN_H_G_Weapon:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_WEAPON]);
      nNVN_H_G_Righthand:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_RIGHTHAND]);
      nNVN_H_G_Helmet:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_HELMET]);
      nNVN_H_G_Necklace:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_NECKLACE]);
      nNVN_H_G_Ring_R:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_RINGR]);
      nNVN_H_G_Ring_L:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_RINGL]);
      nNVN_H_G_Armring_R:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_ARMRINGR]);
      nNVN_H_G_Armring_L:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_ARMRINGL]);
      nNVN_H_G_Bujuk:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_BUJUK]);
      nNVN_H_G_Belt:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_BELT]);
      nNVN_H_G_Boots:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_BOOTS]);
      nNVN_H_G_Charm:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_CHARM]);
      nNVN_H_G_Hat:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_HAT]);
      nNVN_H_G_Drum:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_DRUM]);
      nNVN_H_G_Shield:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_SHIELD]);
      nNVN_H_G_Horse:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_HORSE]);
      nNVN_H_G_Jade:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_JADE]);
      nNVN_H_G_FashionDress:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_FASHIONDRESS]);
      nNVN_H_G_FashionWeapon:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_FASHIONWEAPON]);
      nNVN_H_G_FashionNecklace:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_FASHIONNECKLACE]);
      nNVN_H_G_FashionHelmet:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_FASHIONHELMET]);
      nNVN_H_G_FashionArmringL:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_FASHIONARMRINGL]);
      nNVN_H_G_FashionArmringR:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_FASHIONARMRINGR]);
      nNVN_H_G_FashionRingL:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_FASHIONRINGL]);
      nNVN_H_G_FashionRingR:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_FASHIONRINGR]);
      nNVN_H_G_FashionRighthand:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_FASHIONRIGHTHAND]);
      nNVN_H_G_FashionBelt:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_FASHIONBELT]);
      nNVN_H_G_FashionBoots:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_FASHIONBOOTS]);
      nNVN_H_G_FashionCharm:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_UseItems[U_FASHIONCHARM]);
      nNVN_H_G_JewelryItem1:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_JewelryBoxItems[0]);
      nNVN_H_G_JewelryItem2:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_JewelryBoxItems[1]);
      nNVN_H_G_JewelryItem3:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_JewelryBoxItems[2]);
      nNVN_H_G_JewelryItem4:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_JewelryBoxItems[3]);
      nNVN_H_G_JewelryItem5:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_JewelryBoxItems[4]);
      nNVN_H_G_JewelryItem6:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_JewelryBoxItems[5]);
      nNVN_H_G_GodBlessItem1:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_GodBlessItems[0]);
      nNVN_H_G_GodBlessItem2:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_GodBlessItems[1]);
      nNVN_H_G_GodBlessItem3:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_GodBlessItems[2]);
      nNVN_H_G_GodBlessItem4:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_GodBlessItems[3]);
      nNVN_H_G_GodBlessItem5:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_GodBlessItems[4]);
      nNVN_H_G_GodBlessItem6:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_GodBlessItems[5]);
      nNVN_H_G_GodBlessItem7:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_GodBlessItems[6]);
      nNVN_H_G_GodBlessItem8:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_GodBlessItems[7]);
      nNVN_H_G_GodBlessItem9:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_GodBlessItems[8]);
      nNVN_H_G_GodBlessItem10:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_GodBlessItems[9]);
      nNVN_H_G_GodBlessItem11:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_GodBlessItems[10]);
      nNVN_H_G_GodBlessItem12:
        if Hero <> nil then
          sText := UserEngine.GetStdItemChangeName(@Hero.m_GodBlessItems[11]);
      nNVN_H_DropInsuranceItemName:
        if Hero <> nil then
          sText := Hero.m_sDropInsuranceItemName;
      nNVN_H_DropInsuranceItemCount:
        if Hero <> nil then
          sText := IntToStr(Hero.m_nDropInsuranceItemCount);
      nNVN_H_DropInsuranceItemCurrency:
        if Hero <> nil then
          sText := IntToStr(Hero.m_nDropInsuranceItemCurrency);
      nNVN_H_DropInsuranceItemGold:
        if Hero <> nil then
          sText := IntToStr(Hero.m_nDropInsuranceItemGold);
      NNVN_H_LearnMagicID:
        if Hero <> nil then
          sText := IntToStr(Hero.m_nLearnMagicID);
      NNVN_H_NpcRebornCount:
        if Hero <> nil then
          sText := IntToStr(Hero.m_nNpcRebornCount);
      NNVN_H_TriggerNpcRebornCount:
        if Hero <> nil then
          sText := IntToStr(Hero.m_nTriggerNpcRebornCount);
      NNVN_H_NGAddPower:
        if (Hero <> nil) and Hero.m_boTrainingNG then
          sText := IntToStr(Hero.GetNGAddPower);
      NNVN_H_NGDecPower:
        if (Hero <> nil) and Hero.m_boTrainingNG then
          sText := IntToStr(Hero.GetNGDecPower);
      NNVN_H_EXPIREDITEMNAME:
        if Hero <> nil then
          sText := Hero.m_sExpiredItemName;
      nNVN_H_UpgradeCount:
        begin
          n18 := 0;
          if Hero <> nil then
          begin
            for I := Low(THumanUseItems) to High(THumanUseItems) do
            begin
              UserItem := @Hero.m_UseItems[I];
              if (UserItem.wIndex > 0) then
                Inc(n18, UserItem.btUpgradeCount);
            end;
            for I := Low(THumanJewelryBoxItems) to High(THumanJewelryBoxItems) do
            begin
              UserItem := @Hero.m_JewelryBoxItems[I];
              if (UserItem.wIndex > 0) then
                Inc(n18, UserItem.btUpgradeCount);
            end;
            for I := Low(THumanGodBlessItems) to High(THumanGodBlessItems) do
            begin
              UserItem := @Hero.m_GodBlessItems[I];
              if (UserItem.wIndex > 0) then
                Inc(n18, UserItem.btUpgradeCount);
            end;
          end;
          sText := IntToStr(n18);
        end;
      nNVN_H_StruckHP:
        begin
          if Hero <> nil then
            sText := IntToStr(Hero.m_nStruckHP);
        end;
      nNVN_H_CURSLAVENAME:
        begin
          if Hero <> nil then
            sText := DelNumber(Hero.CurrentSlaveName);
        end;
      nNVN_H_CurSlaveFullName:
        begin
          if Hero <> nil then
            sText := Hero.CurrentSlaveName;
        end;
      nNVN_H_DieSlaveName:
        begin
          if Hero <> nil then
            sText := DelNumber(Hero.CurrentSlaveName);
        end;
      nNVN_H_LoyalPoint:
        begin
          if Hero <> nil then
            sText := IntToStr(Round(Hero.m_rLoyalPoint));
        end;
      nNVN_H_UseItemMakeIndex:
        begin
          if Hero <> nil then
            sText := IntToStr(Hero.m_nUseItemMakeIndex);
        end;
      nNVN_H_UseItemFrom:
        begin
          if Hero <> nil then
            sText := IntToStr(Integer(Hero.m_UseItemFrom));
        end;
      nNVN_H_UseItemName:
        begin
          if Hero <> nil then
            sText := Hero.m_sUseItemName;
        end;
      nNVN_H_BagItemMakeIndex:
        begin
          if Hero <> nil then
            sText := IntToStr(Hero.m_nBagItemMakeIndex);
        end;
      nNVN_H_BagItemName:
        begin
          if Hero <> nil then
            sText := Hero.m_sBagItemName;
        end;
      nNVN_H_G_UseItemName:
        begin
          if Hero <> nil then
            sText := Hero.m_sUseItemNewName;
        end;
      nNVN_H_G_BagItemName:
        begin
          if Hero <> nil then
            sText := Hero.m_sBagItemNewName;
        end;
      nNVN_H_CombatPower:
        begin
          if Hero <> nil then
            sText := IntToStr(Hero.m_nCombatPower);
        end;
      nNVN_H_AttackHumPowerRate:
        begin
          if Hero <> nil then
            sText := Format('%g', [Hero.m_nAttackHumPowerRate / 100]); // 当前攻击人物伤害力倍数 chongchong 2013-12-09
        end;
      nNVN_H_AttackHumPowerRateTime:
        begin
          if Hero <> nil then
            sText := IntToStr(Hero.m_dwAttackHumPowerRateTime); // 当前攻击人物伤害力倍数剩余时间 chongchong 2013-12-09
        end;
      nNVN_H_AttackMonPowerRate:
        begin
          if Hero <> nil then
            sText := Format('%g', [Hero.m_nAttackMonPowerRate / 100]); // 当前攻击怪物伤害力倍数 chongchong 2013-12-09
        end;
      nNVN_H_AttackMonPowerRateTime:
        begin
          if Hero <> nil then
            sText := IntToStr(Hero.m_dwAttackMonPowerRateTime); // 当前攻击怪物伤害力倍数剩余时间 chongchong 2013-12-09
        end;
      nNVN_H_SlaveCount:
        begin
          nIndex := 0;
          if Hero <> nil then
          begin
            for II := 0 to Hero.m_SlaveList.Count - 1 do
            begin
              TempObject := Hero.m_SlaveList.Items[II];
              if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) then
              begin
                Inc(nIndex);
              end;
            end;
          end;
          sText := IntToStr(nIndex);
        end;
      nNVN_H_HumCloneCount:
        begin
          nIndex := 0;
          if Hero <> nil then
          begin
            for II := 0 to Hero.m_SlaveList.Count - 1 do
            begin
              TempObject := Hero.m_SlaveList.Items[II];
              if (not TempObject.m_boDeath) and (not TempObject.m_boGhost) and (TempObject is TCopyMon) then
              begin
                Inc(nIndex);
              end;
            end;
          end;
          sText := IntToStr(nIndex);
        end;
      nNVN_H_CurItemName:
        begin
          if Hero <> nil then
            sText := Hero.m_sCurrentItemName;
        end;
      nNVN_H_DamageValue:
        begin
          if Hero <> nil then
            sText := IntToStr(Hero.m_nLastDamageValue);
        end;
      nNVN_H_CurrentItemMakeIndex:
        begin
          if Hero <> nil then
            sText := IntToStr(Hero.m_nCurrentItemMakeIndex);
        end;
      nNVN_H_CurrentItemPos:
        begin
          if Hero <> nil then
            sText := IntToStr(Hero.m_nCurrentItemPos);
        end;
      nNVN_H_G_CurrentItemName:
        begin
          if Hero <> nil then
            sText := Hero.m_sCurrentItemNewName;
        end;
      nNVN_H_AttackMonster_Name_Ex:
        begin
          if (Hero <> nil) and (Hero.m_CurrTargetEx <> nil) then
            sText := Hero.m_CurrTargetEx.m_sCharName;
        end;
      nNVN_H_AttackMonster_MaxHP_Ex:
        begin
          sText := '0';
          if (Hero <> nil) and (Hero.m_CurrTargetEx <> nil) then
            sText := IntToStr(Hero.m_CurrTargetEx.m_WAbil.MaxHP);
        end;
      nNVN_H_AttackMonster_HP_Ex:
        begin
          sText := '0';
          if (Hero <> nil) and (Hero.m_CurrTargetEx <> nil) then
            sText := IntToStr(Hero.m_CurrTargetEx.m_WAbil.HP);
        end;
      nNVN_H_AttackMonster_X_Ex:
        begin
          if (Hero <> nil) and (Hero.m_CurrTargetEx <> nil) then
            sText := IntToStr(Hero.m_CurrTargetEx.m_nCurrX);
        end;
      nNVN_H_AttackMonster_Y_Ex:
        begin
          if (Hero <> nil) and (Hero.m_CurrTargetEx <> nil) then
            sText := IntToStr(Hero.m_CurrTargetEx.m_nCurrY);
        end;
      nNVN_H_Killer:
        begin
          if (Hero <> nil) and (Hero.m_LastHiter <> nil) then
          begin
            sText := Hero.m_LastHiter.m_sCharName;
            if not (Hero.m_LastHiter.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]) then
            begin
              sText := DelNumber(sText);
            end;
          end;
        end;
      nNVN_H_MagicID:
        begin
          if (Hero <> nil) then
          begin
            sText := IntToStr(Hero.m_nSpellMagicID);
          end;
        end;
      nNVN_H_MagicName:
        begin
          if (Hero <> nil) then
          begin
            sText := Hero.m_sSpellMagicName;
          end;
        end;
      nNVN_H_MagicTarget:
        begin
          if (Hero <> nil) then
          begin
            sText := Hero.m_sSpellMagicTarget;
          end;
        end;
      nNVN_H_MagicTargetRace:
        begin
          if (Hero <> nil) then
          begin
            sText := IntToStr(Hero.m_sSpellMagicTargetRace);
          end;
        end;
      nNVN_H_LUCK:
        begin
          if (Hero <> nil) then
          begin
            sText := IntToStr(Hero.m_nLuck);
          end;
        end;
      nNVN_H_ANGRYVALUE:
        begin
          if (Hero <> nil) then
          begin
            sText := IntToStr(Hero.m_btAngryValue);
          end;
        end;
      nNVN_H_BEADEXP:
        begin
          if (Hero <> nil) then
          begin
            sText := IntToStr(Hero.m_dwBeadExp);
          end;
        end;
      nNVN_H_BEADEXPSOURCE:
        begin
          if (Hero <> nil) then
          begin
            sText := IntToStr(Hero.m_nBeadSource);
          end;
        end;
      nNVN_H_CurTargetMasterName:
        begin
          if (Hero <> nil) and (Hero.m_TargetCret <> nil) and (Hero.m_TargetCret.m_Master <> nil) then
          begin
            sText := Hero.m_TargetCret.m_Master.m_sCharName;
          end;
        end;
      nNVN_H_Direction:
        begin
          if (Hero <> nil) then
          begin
            sText := IntToStr(Hero.m_btDirection);
          end;
        end;
      // ---------------------------
      nNVN_PET_X:
        begin
          if GamePet <> nil then
            sText := IntToStr(GamePet.m_nCurrX);
        end;
      nNVN_PET_Y:
        begin
          if GamePet <> nil then
            sText := IntToStr(GamePet.m_nCurrY);
        end;
      nNVN_PET_HP:
        begin
          if GamePet <> nil then
            sText := IntToStr(GamePet.m_WAbil.HP);
        end;
      nNVN_PET_MAXHP:
        begin
          if GamePet <> nil then
            sText := IntToStr(GamePet.m_WAbil.MaxHP);
        end;
      nNVN_PET_MP:
        begin
          if GamePet <> nil then
            sText := IntToStr(GamePet.m_WAbil.MP);
        end;
      nNVN_PET_MAXMP:
        begin
          if GamePet <> nil then
            sText := IntToStr(GamePet.m_WAbil.MaxMP);
        end;
      nNVN_PET_CURTARGETNAME:
        begin
          if (GamePet <> nil) and (GamePet.m_CurrTarget <> nil) then
          begin
            sText := GamePet.m_CurrTarget.m_sCharName;
            sText := DelNumber(sText);
          end;
        end;
      nNVN_PET_CURTARGETFULLNAME:
        begin
          if (GamePet <> nil) and (GamePet.m_CurrTarget <> nil) then
          begin
            sText := GamePet.m_CurrTarget.m_sCharName;
          end;
        end;
      nNVN_PET_CURTARGETX:
        begin
          if (GamePet <> nil) and (GamePet.m_CurrTarget <> nil) then
          begin
            sText := IntToStr(GamePet.m_CurrTarget.m_nCurrX);
          end;
        end;
      nNVN_PET_CURTARGETY:
        begin
          if (GamePet <> nil) and (GamePet.m_CurrTarget <> nil) then
          begin
            sText := IntToStr(GamePet.m_CurrTarget.m_nCurrY);
          end;
        end;
      nNVN_PET_CURTARGETHP:
        begin
          if (GamePet <> nil) and (GamePet.m_CurrTarget <> nil) then
          begin
            sText := IntToStr(GamePet.m_CurrTarget.m_WAbil.HP);
          end;
        end;
      nNVN_PET_CURTARGETMAXHP:
        begin
          if (GamePet <> nil) and (GamePet.m_CurrTarget <> nil) then
          begin
            sText := IntToStr(GamePet.m_CurrTarget.m_WAbil.MaxHP);
          end;
        end;
      nNVN_PET_DAMAGEVALUE:
        begin
          if GamePet <> nil then
            sText := IntToStr(GamePet.m_nLastDamageValue);
        end;
      nNVN_PET_KILLMONNAME:
        begin
          if (GamePet <> nil) and (GamePet.m_CurrTarget <> nil) then
          begin
            sText := GamePet.m_CurrTarget.m_sCharName;
            sText := DelNumber(sText);
          end;
        end;
    end;
    IsBreakParseVar := True;
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    Exit;
  end;
  // 加入自定义OK框变量 chongchong 2013-11-08
  if SameText(Copy(sVariable, 1, 9), '$BOXITEM[') then
  begin
    Result := GetBoxItemValue('<' + sVariable + '>', PlayObject, sText);
    if Result then
    begin
      sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    end;
    IsBreakParseVar := True;
    Exit;
  end;
  if SameText(Copy(sVariable, 1, 13), '$NATIONPEOPLE') then
  begin
    s14 := Copy(sVariable, 14, MaxInt);
    II := StrToIntDef(s14, 0);
    NationInfo := g_NationManage.Items[II];
    if NationInfo <> nil then
      sText := IntToStr(NationInfo.nPeoples)
    else
      sText := '0';
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if SameText(Copy(sVariable, 1, 11), '$NATIONNAME') then
  begin
    s14 := Copy(sVariable, 12, MaxInt);
    II := StrToIntDef(s14, 0);
    NationInfo := g_NationManage.Items[II];
    if NationInfo <> nil then
      sText := NationInfo.sName
    else
      sText := '';
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if SameText(Copy(sVariable, 1, 11), '$NATIONGOLD') then
  begin
    s14 := Copy(sVariable, 12, MaxInt);
    II := StrToIntDef(s14, 0);
    NationInfo := g_NationManage.Items[II];
    if NationInfo <> nil then
      sText := IntToStr(NationInfo.nGold)
    else
      sText := '0';
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if SameText(Copy(sVariable, 1, 15), '$NATIONBUILDING') then
  begin
    s14 := Copy(sVariable, 16, MaxInt);
    II := StrToIntDef(s14, 0);
    NationInfo := g_NationManage.Items[II];
    if NationInfo <> nil then
      sText := IntToStr(NationInfo.wBuilding)
    else
      sText := '0';
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if SameText(Copy(sVariable, 1, 10), '$NATIONARM') then
  begin
    s14 := Copy(sVariable, 11, MaxInt);
    II := StrToIntDef(s14, 0);
    NationInfo := g_NationManage.Items[II];
    if NationInfo <> nil then
      sText := IntToStr(NationInfo.wArm)
    else
      sText := '0';
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if SameText(Copy(sVariable, 1, 14), '$NATIONECONOMY') then
  begin
    s14 := Copy(sVariable, 15, MaxInt);
    II := StrToIntDef(s14, 0);
    NationInfo := g_NationManage.Items[II];
    if NationInfo <> nil then
      sText := IntToStr(NationInfo.wEconomy)
    else
      sText := '0';
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if SameText(Copy(sVariable, 1, 15), '$NATIONPOLITICS') then
  begin
    s14 := Copy(sVariable, 16, MaxInt);
    II := StrToIntDef(s14, 0);
    NationInfo := g_NationManage.Items[II];
    if NationInfo <> nil then
      sText := IntToStr(NationInfo.wPolitics)
    else
      sText := '0';
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if SameText(Copy(sVariable, 1, 19), '$NATIONCONTRIBUTION') then
  begin
    s14 := Copy(sVariable, 20, MaxInt);
    II := StrToIntDef(s14, 0);
    NationInfo := g_NationManage.Items[II];
    if NationInfo <> nil then
      sText := IntToStr(NationInfo.wContribution)
    else
      sText := '0';
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if SameText(Copy(sVariable, 1, 11), '$NATIONMAPS') then
  begin
    s14 := Copy(sVariable, 12, MaxInt);
    II := StrToIntDef(s14, 0);
    NationInfo := g_NationManage.Items[II];
    if NationInfo <> nil then
      sText := IntToStr(NationInfo.btMaps)
    else
      sText := '0';
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if SameText(Copy(sVariable, 1, 11), '$NATIONKING') then
  begin
    s14 := Copy(sVariable, 12, MaxInt);
    II := StrToIntDef(s14, 0);
    NationInfo := g_NationManage.Items[II];
    if NationInfo <> nil then
      sText := NationInfo.sKingName
    else
      sText := '';
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if CompareLStr(sVariable, '$TEAM', Length('$TEAM')) then
  begin
    if (PlayObject.m_GroupOwner <> nil) and (PlayObject.m_GroupOwner.m_GroupMembers <> nil) then
    begin
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        PlayObject.m_GroupOwner.m_GroupMembers.LockR(2);
      try
{$IFEND}
        s1C := Copy(sVariable, Length('$TEAM') + 1, Length(sVariable) - Length('$TEAM'));
        // s1C := Copy(sVariable, Length(sVariable) - 1, 1);
        n18 := StrToIntDef(s1C, -1);
        if (n18 >= 0) and (n18 < PlayObject.m_GroupOwner.m_GroupMembers.Count) then
        begin
          sText := PlayObject.m_GroupOwner.m_GroupMembers.Strings[n18];
        end
        else
          sText := '????';
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          PlayObject.m_GroupOwner.m_GroupMembers.UnLockR;
      end;
{$IFEND}
    end
    else
      sText := '????';
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if CompareLStr(sVariable, '$TAGMAPNAME', Length('$TAGMAPNAME')) then
  begin
    s1C := sVariable[Length(sVariable)]; // Copy(sVariable, Length(sVariable) - 1, 1);
    n18 := StrToIntDef(s1C, -1);
    RememberItem := GetRememberItem(PlayObject.m_nRememberItemIndex, n18);
    if RememberItem <> nil then
    begin
      if RememberItem.sMapName <> '' then
      begin
        Envir := g_MapManager.FindMap(RememberItem.sMapName);
        if Envir <> nil then
          sText := Envir.sMapDesc
        else
          sText := '???';
      end
      else
        sText := '未记忆';
    end
    else
      sText := '未记忆';
    sMsg := sub_49ADB8(nPos, sMsg, '<$TAGMAPNAME' + IntToStr(n18) + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if CompareLStr(sVariable, '$TAGX', Length('$TAGX')) then
  begin
    s1C := sVariable[Length(sVariable)]; // s1C := Copy(sVariable, Length(sVariable) - 1, 1);
    n18 := StrToIntDef(s1C, -1);
    RememberItem := GetRememberItem(PlayObject.m_nRememberItemIndex, n18);
    if RememberItem <> nil then
    begin
      if RememberItem.sMapName <> '' then
      begin
        sText := IntToStr(RememberItem.nCurrX);
      end
      else
        sText := '未记忆';
    end
    else
      sText := '未记忆';
    sMsg := sub_49ADB8(nPos, sMsg, '<$TAGX' + IntToStr(n18) + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if CompareLStr(sVariable, '$TAGY', Length('$TAGY')) then
  begin
    s1C := sVariable[Length(sVariable)]; // s1C := Copy(sVariable, Length(sVariable) - 1, 1);
    n18 := StrToIntDef(s1C, -1);
    RememberItem := GetRememberItem(PlayObject.m_nRememberItemIndex, n18);
    if RememberItem <> nil then
    begin
      if RememberItem.sMapName <> '' then
      begin
        sText := IntToStr(RememberItem.nCurrY);
      end
      else
        sText := '未记忆';
    end
    else
      sText := '未记忆';
    sMsg := sub_49ADB8(nPos, sMsg, '<$TAGY' + IntToStr(n18) + '>', sText);
    IsBreakParseVar := True;
    Exit;
  end;
  if CompareLStr(sVariable, '$HUMAN(', Length('$HUMAN(')) then
  begin
    ArrestStringEx(sVariable, '(', ')', s14);
    boFoundVar := False;
    for I := 0 to PlayObject.m_DynamicVarList.Count - 1 do
    begin
      DynamicVar := PlayObject.m_DynamicVarList.Items[I];
      if CompareText(DynamicVar.sName, s14) = 0 then
      begin
        case DynamicVar.VarType of
          vInteger:
            begin
              sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(DynamicVar.nInternet));
              boFoundVar := True;
            end;
          vString:
            begin
              sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', DynamicVar.sString);
              boFoundVar := True;
            end;
        end;
        Break;
      end;
    end;
    if not boFoundVar then
      sMsg := '??';
    IsBreakParseVar := SameText(sMsg, '<' + sVariable + '>');
    Exit;
  end;
  if CompareLStr(sVariable, '$GUILD(', Length('$GUILD(')) then
  begin
    if PlayObject.m_MyGuild = nil then
      Exit;
    ArrestStringEx(sVariable, '(', ')', s14);
    boFoundVar := False;
    for I := 0 to TGUild(PlayObject.m_MyGuild).m_DynamicVarList.Count - 1 do
    begin
      DynamicVar := TGUild(PlayObject.m_MyGuild).m_DynamicVarList.Items[I];
      if CompareText(DynamicVar.sName, s14) = 0 then
      begin
        case DynamicVar.VarType of
          vInteger:
            begin
              sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(DynamicVar.nInternet));
              boFoundVar := True;
            end;
          vString:
            begin
              sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', DynamicVar.sString);
              boFoundVar := True;
            end;
        end;
        Break;
      end;
    end;
    if not boFoundVar then
      sMsg := '??';
    IsBreakParseVar := True;
    Exit;
  end;
  if CompareLStr(sVariable, '$NPCINPUT(', Length('$NPCINPUT(')) then
  begin
    ArrestStringEx(sVariable, '(', ')', s14);
    n18 := StrToIntDef(s14, 0) - 1;
    if (n18 >= Low(PlayObject.m_NpcInputText)) and (n18 <= High(PlayObject.m_NpcInputText)) then
    begin
      sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', PlayObject.m_NpcInputText[n18]);
    end;
    IsBreakParseVar := True;
    Exit;
  end;
  if CompareLStr(sVariable, '$GLOBAL(', Length('$GLOBAL(')) then
  begin
    ArrestStringEx(sVariable, '(', ')', s14);
    boFoundVar := False;
    for I := 0 to g_DynamicVarList.Count - 1 do
    begin
      DynamicVar := g_DynamicVarList.Items[I];
      if CompareText(DynamicVar.sName, s14) = 0 then
      begin
        case DynamicVar.VarType of
          vInteger:
            begin
              sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(DynamicVar.nInternet));
              boFoundVar := True;
            end;
          vString:
            begin
              sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', DynamicVar.sString);
              boFoundVar := True;
            end;
        end;
        Break;
      end;
    end;
    if not boFoundVar then
      sMsg := '??';
    IsBreakParseVar := SameText(sMsg, '<' + sVariable + '>');
    Exit;
  end;
  if CompareLStr(sVariable, '$MONEY(', Length('$MONEY(')) then
  begin
    ArrestStringEx(sVariable, '(', ')', s14);
    I := PlayObject.m_MoneyList.GetIndex(UpperCase(s14));
    if I >= 0 then
    begin
      sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(Integer(PlayObject.m_MoneyList.Objects[I])));
      boFoundVar := True;
    end
    else
    begin
      PlayObject.m_MoneyList.AddRecord(UpperCase(s14), 0);
      boFoundVar := True;
    end;
    if not boFoundVar then
      sMsg := '??';
    IsBreakParseVar := SameText(sMsg, '<' + sVariable + '>');
    Exit;
  end;
  if CompareLStr(sVariable, '$STR(', Length('$STR(')) then
  begin
    ArrestStringEx(sVariable, '(', ')', s14);
    n18 := GetValNameNo(s14);
    if n18 >= 0 then
    begin
      case n18 of
        0..999:
          begin // P
            sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(PlayObject.m_nVal[n18]));
          end;
        1000..1999:
          begin
            sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(PlayObject.m_DyVal[n18 - 1000]));
          end;
        2000..2999:
          begin
            sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(PlayObject.m_nMval[n18 - 2000]));
          end;
        3000..3999:
          begin
            sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(PlayObject.m_nInteger[n18 - 3000]));
          end;
        4000..4999:
          begin
            sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(g_Config.GlobaDyMval[n18 - 4000]));
          end;
        5000..5999:
          begin
            sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(g_Config.GlobalVal[n18 - 5000]));
          end;
        6000..6999:
          begin
            sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', g_Config.GlobalAVal[n18 - 6000]);
          end;
        7000..7999:
          begin
            sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', PlayObject.m_sString[n18 - 7000]);
          end;
        // 私有变量 U-数字型 chongchong 2014-10-18
        8000..8499:
          begin
            sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(PlayObject.m_UVal[n18 - 8000]));
          end;
        // 私有变量 T-字符串型 chongchong 2014-10-18
        8500..8999:
          begin
            sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', PlayObject.m_TVal[n18 - 8500]);
          end;
        // 私有变量 J-数字型(1天1清) chongchong 2014-10-18
        9000..9499:
          begin
            sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(PlayObject.m_JVal[n18 - 9000]));
          end;
        // 私有变量 Z-数字型(1天1清) chongchong 2014-10-18
        9500..9999:
          begin
            sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', PlayObject.m_ZVal[n18 - 9500]);
          end;
      end;
      IsBreakParseVar := SameText(sMsg, '<' + sVariable + '>');
    end
    //L变量
    else if (Length(s14) > 2) and (UpCase(s14[1]) = 'L') and (s14[2] = '$') then
    begin
      arySIndex := 'not';
      if (Pos('[', s14) > 0) and (Pos(']', s14) > 0) then
      begin
        sData := GetValidStr3(s14, s14, ['[']);
        sData := GetValidStr3(sData, arySIndex, [']']);
      end;

      n18 := PlayObject.m_ArrayList.GetIndex(UpperCase(s14));

      if n18 >= 0 then
      begin
        sValue := PlayObject.m_ArrayList.Strings[n18];
        if arySIndex <> 'not' then
        begin
          aryNIndex := strtoint(arySIndex);
          aryLValue := sValue.Split([',']);
          if (aryNIndex >= 0) and (aryNIndex <= length(aryLValue)) then
          begin
            sValue := aryLValue[aryNIndex];
          end
          else if (aryNIndex < 0) and (Abs(aryNIndex) <= length(aryLValue)) then
          begin
            sValue := aryLValue[Abs(aryNIndex)-1];
          end;

        end;

        sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sValue);
      end
      else
      begin
        sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', '');
      end;
      IsBreakParseVar := SameText(sMsg, '<' + sVariable + '>');
    end
    else if (Length(s14) > 2) and (UpCase(s14[1]) = 'S') and (s14[2] = '$') then
    begin
      n18 := PlayObject.m_StringList.GetIndex(UpperCase(s14));
      if n18 >= 0 then
      begin
        sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', PlayObject.m_StringList.Strings[n18]);
      end
      else
      begin
        sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', '');
      end;
      IsBreakParseVar := SameText(sMsg, '<' + sVariable + '>');
    end
    else if (Length(s14) > 2) and (UpCase(s14[1]) = 'N') and (s14[2] = '$') then
    begin
      n18 := PlayObject.m_IntegerList.GetIndex(UpperCase(s14));
      if n18 >= 0 then
      begin
        sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', IntToStr(Integer(PlayObject.m_IntegerList.Objects[n18])));
      end
      else
      begin
        sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', '0');
      end;
      IsBreakParseVar := SameText(sMsg, '<' + sVariable + '>');
    end
    else
    begin
      Result := False;
    end;
    Exit;
  end
  // 添加常量标识 chongchong 2017-02-21
  else if CompareLStr(sVariable, '$CONST(', Length('$CONST(')) then
  begin
    ArrestStringEx(sVariable, '(', ')', s14);
    s14 := Copy(sMsg, nPos + 8, Length(s14)); // sVariable中的s14全是大写，这里要取原始的大小写 chongchong 2017-02-21
    sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', s14);
    IsBreakParseVar := True;
    Exit;
  end;
  if (g_PluginManager <> nil) then
  begin
    BufLen := 100;
    GetMem(Buffer, BufLen);
    try
      FillChar(Buffer^, BufLen, 0);
      if g_PluginManager.HookGetVariableText(Self, PlayObject, PAnsiChar(AnsiString(sVariable)), Buffer, BufLen) then
      begin
        sText := '';
        if BufLen > 0 then
        begin
          sText := Buffer;
        end;
        sMsg := sub_49ADB8(nPos, sMsg, '<' + sVariable + '>', sText);
        Exit;
      end;
    finally
      FreeMem(Buffer, BufLen);
    end;
  end;
  Result := False;
end;

function TNormNpc.GotoLable(Player: TPlayObject; sLabel: string; boExtJmp: Boolean; UseParams: Boolean): Boolean;
var
  I, II, III: Integer;
  bo11: Boolean;
  sSENDMSG, sParams: string;
  sWideParams: WideString;
  Script: pTScript;
  Script3C: pTScript;
  SayingRecord: pTSayingRecord;
  SayingProcedure: pTSayingProcedure;
  IsResetVarP: Boolean;

  function CheckQuestStatus(ScriptInfo: pTScript): Boolean; // 0049BA00
  var
    I: Integer;
  begin
    Result := True;
    if not ScriptInfo.boQuest then
      Exit;

    I := 0;
    while (True) do
    begin
      if (ScriptInfo.QuestInfo[I].nRandRage > 0) and (Random(ScriptInfo.QuestInfo[I].nRandRage) <> 0) then
      begin
        Result := False;
        Break;
      end;

      if Player.GetQuestFlagStatus(ScriptInfo.QuestInfo[I].wFlag) <> ScriptInfo.QuestInfo[I].btValue then
      begin
        Result := False;
        Break;
      end;

      Inc(I);
      if I >= 10 then
        Break;
    end;
  end;

  procedure SendMerChantSayMsg(sMsg: string; boFlag: Boolean);
  var
    s10, s14: string;
    nC, nPos: Integer;
    nStartPos: Integer;
    IsBreakParseVar: Boolean;
  begin
    if g_OnlineMsgControl.boDisableUseNpc then
      Exit;

    if (not Player.m_boOffLine) and (not Player.m_boDummyObject) then
    begin // 离线人物和假人不发送NPC对话
      s14 := sMsg;
      nC := 0;
      nStartPos := 1;
      // if self.='综合12_激情派对' then
      // if g_boTestMode then
      // MainOutMessage('SendMerChantSayMsg 1:' + sMsg);
      while (True) do
      begin
        // if TagCount(s14, '>') < 1 then Break;  $ $
        s14 := sMsg;
        if (Pos('>', s14) <= 0) or (Pos('$', s14) <= 0) or (nStartPos >= Length(s14)) then
          Break;

        nPos := ArrestVariable(s14, '<', '$', '>', nStartPos, s10);
        if s10 = '' then
          Break;

        // MainOutMessage('SendMerChantSayMsg:' + IntToStr(nPos) + ' ' + s10);
        // MainOutMessage('SendMerChantSayMsg:' + IntToStr(Length(PosArray)));
        if not GetVariableText(Player, sMsg, s10, IsBreakParseVar, nPos) then
        begin
          nStartPos := nPos + 2;
          // MainOutMessage('SendMerChantSayMsg:' + s10);
        end;

        Inc(nC);
        if nC >= 1001 then
          Break;
      end;

      // if m_sCharName = '英雄服务' then
      // MainOutMessage('SendMerChantSayMsg:' + sMsg);
      // 在QF中显示OK框，触发到别的npc去了
      if Pos('ITEMBOX:', UpperCase(sMsg)) > 0 then
        Player.m_ItemBoxNpc := Self;

      Player.GetScriptLabel(sMsg);
      if Self = g_MissionNPC then
      begin
        // MainOutMessage('SendMerChantSayMsg:'+sLabel + '/' + sMsg);
        if sLabel = '' then
          s10 := sMsg
        else
          s10 := sLabel + #13 + sMsg;

        if boFlag then
          Player.SendFirstMsg(Self, RM_MERCHANTSAY, 0, 0, 0, 0, s10)
        else
          Player.SendMsg(Self, RM_MERCHANTSAY, 0, 0, 0, 0, s10);
      end
      else
      begin
        if m_sCharName = '' then
          s10 := sMsg
        else
          s10 := m_sCharName + #13 + sMsg;

        if boFlag then
          Player.SendFirstMsg(Self, RM_MERCHANTSAY, 0, 0, 0, 0, s10)
        else
          Player.SendMsg(Self, RM_MERCHANTSAY, 0, 0, 0, 0, s10);
      end;
    end;
  end;

begin
  Result := False;
  IsResetVarP := False;
  // ?????????????????? 修复在系统默认NPC中有Timer执行时，会给Player.m_NPC重置值 chongchong 2013-11-13
  if (Player.m_NPC <> Self) //
    and (Self <> g_FunctionNPC) //
    and (Self <> g_ManageNPC) //
    and (Self <> g_RobotNPC) //
    and (Self <> g_MissionNPC) //
    and (Self <> g_BatterNPC) then
  begin
    Player.m_NPC := nil;
    Player.m_Script := nil;
    FillChar(Player.m_nVal[0], SizeOf(Player.m_nVal), 0); // P变量重点对话框时置0 chongchong 2014-06-25
    IsResetVarP := True;
  end;

  Script := nil;
  SayingRecord := nil;
  if CompareText(sLabel, '@main') = 0 then
  begin
    for I := 0 to m_ScriptList.Count - 1 do
    begin
      Script3C := m_ScriptList.Items[I];
      if Script3C = nil then
        Continue;

      // 优化 2020-05-27
      SayingRecord := GetSayingRecordFromRecordList(Script3C.RecordList, sLabel);
      if SayingRecord <> nil then
      begin
        Script := Script3C;
        Player.m_Script := Script;
        if not IsResetVarP then
        begin
          // P变量重点对话框时置0 chongchong 2014-06-25
          FillChar(Player.m_nVal[0], SizeOf(Player.m_nVal), 0);
          // IsResetVarP := True;
        end;
        Player.m_NPC := Self;
        Break;
      end;
    end;
  end;

  if Script = nil then
  begin
    // 这里可以排序处理，然后用二分查找
    if (Player.m_Script <> nil) and (m_ScriptList <> nil) and (m_ScriptList.Count > 0) then
    begin
      for I := m_ScriptList.Count - 1 downto 0 do
      begin
        if m_ScriptList.Count <= 0 then
          Break;

        if (m_ScriptList.Items[I] <> nil) and (m_ScriptList.Items[I] = Player.m_Script) then
          Script := m_ScriptList.Items[I];
      end;
    end;

    if Script = nil then
    begin
      if (m_ScriptList <> nil) and (m_ScriptList.Count > 0) then
      begin
        for I := m_ScriptList.Count - 1 downto 0 do
        begin
          if m_ScriptList.Count <= 0 then
            Break;

          if (pTScript(m_ScriptList.Items[I]) <> nil) and CheckQuestStatus(pTScript(m_ScriptList.Items[I])) then
          begin
            Script := m_ScriptList.Items[I];
            Player.m_Script := Script;
            // ?????????????????? 修复在系统默认NPC中有Timer执行时，会给Player.m_NPC重置值 chongchong 2013-11-13
            if (Self <> g_FunctionNPC) //
              and (Self <> g_ManageNPC) //
              and (Self <> g_RobotNPC) //
              and (Self <> g_MissionNPC) //
              and (Self <> g_BatterNPC) then
              Player.m_NPC := Self;
          end;
        end;
      end;
    end;
  end;

  // 这里可以排序处理，然后用二分查找
  // 跳转到指定示签，执行
  if (Script <> nil) and (Script.RecordList <> nil) then
  begin
    sParams := '';
    III := Pos('(', sLabel);
    if III > 0 then
    begin
      m_sLastLabel := Copy(sLabel, III + 1, MaxInt);
      if (Length(m_sLastLabel) > 0) and (m_sLastLabel[Length(m_sLastLabel)] = ')') then
      begin
        sLabel := Trim(Copy(sLabel, 1, III - 1));
        sParams := Copy(m_sLastLabel, 1, Length(m_sLastLabel) - 1);
        sWideParams := sParams;
        III := 0;
        for I := Low(Player.m_sScriptParams) to High(Player.m_sScriptParams) do
        begin
          II := Pos(WideString(','), sWideParams);
          if II > 0 then
          begin
            Player.m_sScriptParams[III] := Trim(Copy(sWideParams, 1, II - 1));
            sWideParams := Copy(sWideParams, II + 1, MaxInt);
            Inc(III);
          end
          else if Length(sWideParams) > 0 then
          begin
            Player.m_sScriptParams[III] := Trim(sWideParams);
            sWideParams := '';
            Break;
          end;
        end;
      end;
    end;

    if (tick_diff(Player.m_dwLastGotoLabelTick, MyGetTickCount) <= 80) and SameText(sLabel, Player.m_sLastGotoLabel) then
    begin
      Inc(Player.m_nOneLabelGotoCount);
      if Player.m_nOneLabelGotoCount > Max(g_Config.nLimitScriptGotoCount * 5, 1000) then
      begin
        MainOutMessage(Format('[脚本死循环] NPC:%s 位置:%s(%d,%d) 命令:GOTO %s', [m_sCharName, m_sMapName, m_nCurrX, m_nCurrY, sLabel]));
        Player.m_nOneLabelGotoCount := 0;
        Exit;
      end;
    end
    else
    begin
      Player.m_nOneLabelGotoCount := 0;
      Player.m_sLastGotoLabel := sLabel;
    end;

    Player.m_dwLastGotoLabelTick := MyGetTickCount;
    m_sLastLabel := sLabel;

    // 优化 2020-05-27
    if SayingRecord = nil then
      SayingRecord := GetSayingRecordFromRecordList(Script.RecordList, sLabel);

    if SayingRecord <> nil then
    begin
      Result := True;
      // MainOutMessage('sLabel '+sLabel+' boExtJmp '+BooleanToStr(boExtJmp)+' SayingRecord.boExtJmp '+BooleanToStr(SayingRecord.boExtJmp));
      if boExtJmp and not SayingRecord.boExtJmp then
        Exit;

      sSENDMSG := '';
      for III := 0 to SayingRecord.ProcedureList.Count - 1 do
      begin
        SayingProcedure := SayingRecord.ProcedureList.Items[III];
        // if SayingProcedure = nil then Continue;
        bo11 := False;
        if HandleNpcCmds.QuestCheckCondition(Self, Player, SayingProcedure.ConditionList) then
        begin
          sSENDMSG := sSENDMSG + SayingProcedure.sSayMsg;
          if not HandleNpcCmds.QuestActionProcess(Self, sLabel, Player, SayingProcedure.ActionList, bo11) then
            Break;

          if bo11 then
            SendMerChantSayMsg(sSENDMSG, True);
        end
        else
        begin
          {
            // 在QF中显示OK框，触发到别的npc去了
            if Pos('<ITEMBOX:', UpperCase(SayingProcedure.sSayMsg)) = 1 then
            Player.m_NPC := Self;
          }
          sSENDMSG := sSENDMSG + SayingProcedure.sElseSayMsg;
          if not HandleNpcCmds.QuestActionProcess(Self, sLabel, Player, SayingProcedure.ElseActionList, bo11) then
            Break;

          // if not QuestActionProcess(SayingProcedure.ElseActionList) then Break;
          if bo11 then
            SendMerChantSayMsg(sSENDMSG, True);
        end;
      end;

      if sSENDMSG <> '' then
        SendMerChantSayMsg(sSENDMSG, False);
    end;

    // if Length(sParams) > 0 then
    // begin
    // for I := Low(Player.m_sScriptParams) to High(Player.m_sScriptParams) do
    // Player.m_sScriptParams[I] := '';
    // end;
  end;
end;

procedure TNormNpc.LoadNpcScript;
var
  s08: string;
begin
  if m_boIsQuest then
  begin
    m_sPath := sNpc_def;
    s08 := m_sCharName + '-' + m_sMapName;
    FrmDB.LoadNpcScript(Self, m_sFilePath, s08);
    FrmDB.LoadIconFile(Self, @m_ActorIcons, sNpcIcons, s08);
  end
  else
  begin
    m_sPath := m_sFilePath;
    FrmDB.LoadNpcScript(Self, m_sFilePath, m_sCharName);
  end;
end;

procedure TNormNpc.LoadNpcIconFile;
var
  s08: string;
begin
  if m_boIsQuest then
  begin
    s08 := m_sCharName + '-' + m_sMapName;
    FrmDB.LoadIconFile(Self, @m_ActorIcons, sNpcIcons, s08);
  end;
end;

function TNormNpc.Operate(ProcessMsg: pTProcessMessage): Boolean;
begin
  Result := inherited Operate(ProcessMsg);
end;

function TNormNpc.GetShowName(boSuperUser: Boolean): string;
var
  sShowName: string;
begin
  if (m_nGlobalAValIndex >= 0) and (m_nGlobalAValIndex <= 999) then
  begin
    Result := m_sCharName;
    // g_Config.GlobalAVal[m_nGlobalAValIndex];
  end
  else
  begin
    sShowName := m_sCharName;
    Result := FilterShowName(sShowName);
    if (m_Master <> nil) and not m_Master.m_boObMode then
      Result := Result + '(' + m_Master.m_sCharName + ')';
  end;
end;

procedure TNormNpc.Run;
var
  nInteger: Integer;
  ObjList: TList;
  Obj: TBaseObject;
  I: Integer;
  SetImageInfo: TSetImageInfo;
begin
  if m_Master <> nil then
    m_Master := nil; // 不允许召唤为宝宝

  if (m_PEnvir <> nil) //
    and (m_wAppr = 273) //
    and (m_nValidTime >= 0) //
    and ((m_nEffigyState.Value1 > 0) or (m_nEffigyState.Value2 > 0)) then
  begin
    if m_nValidTime = 0 then
    begin
      m_nEffigyState.Value1 := 0;
      m_nEffigyState.Value2 := 0;
      m_sAddName := '';
      m_nEffigyOffset := 0;
      m_nAddLevel := 0;
      m_boGrayShow := True;
      m_boScaleShow := True;
      m_nValidTime := -1;
      ObjList := TList.Create;
      GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, g_nSendRefMsgRange, ObjList);
      for I := 0 to ObjList.Count - 1 do
      begin
        Obj := TBaseObject(ObjList[I]);
        if (Obj.m_btRaceServer = RC_PLAYOBJECT) //
          and (not Obj.m_boGhost) //
          and (not Obj.m_boDeath) //
          and (not TPlayObject(Obj).m_boDummyObject) then
        begin
          SetImageInfo.EffigyState := m_nEffigyState;
          SetImageInfo.ChrName := m_sCharName + '\' + m_sAddName;
          Obj.SendRefMsg(RM_SETNPCIMAGE, NativeInt(Self), m_nEffigyOffset, Integer(m_boGrayShow), Integer(m_boScaleShow), EncodeBuffer(@SetImageInfo, SizeOf(SetImageInfo)));

          Break;
        end;
      end;
    end
    else if MyGetTickCount - m_wValidTimeTick >= 60000 then
    begin
      m_wValidTimeTick := MyGetTickCount;
      Dec(m_nValidTime);
    end;
  end;

  if (m_nGlobalAValIndex >= 0) //
    and (m_nGlobalAValIndex <= 999) //
    and (CompareText(m_sDynamicName, g_Config.GlobalAVal[m_nGlobalAValIndex]) <> 0) then
  begin
    m_sDynamicName := g_Config.GlobalAVal[m_nGlobalAValIndex];
    m_sCharName := m_sDynamicName;
    RefShowName;
  end;

  // NPC变色
  if (m_boNpcAutoChangeColor) //
    and (m_dwNpcAutoChangeColorTime > 0) //
    and (MyGetTickCount - m_dwNpcAutoChangeColorTick > m_dwNpcAutoChangeColorTime) then
  begin
    m_dwNpcAutoChangeColorTick := MyGetTickCount();
    case m_nNpcAutoChangeIdx of
      0:
        nInteger := STATE_TRANSPARENT;
      1:
        nInteger := POISON_STONE;
      2:
        nInteger := POISON_DONTMOVE;
      3:
        nInteger := POISON_68;
      4:
        nInteger := POISON_DECHEALTH;
      5:
        nInteger := POISON_LOCKSPELL;
      6:
        nInteger := POISON_DAMAGEARMOR;
      7:
        nInteger := STATE_FROZEN;
    else
      m_nNpcAutoChangeIdx := 0;
      nInteger := STATE_TRANSPARENT;
    end;

    Inc(m_nNpcAutoChangeIdx);
    m_nCharStatus := (m_nCharStatusEx and $FFFFF) or (($80000000 shr nInteger) or 0);
    StatusChanged();
  end;
  {
    if m_boFixColor and (m_nFixStatus <> m_nCharStatus) then
    begin
    case m_nFixColorIdx of
    0: nInteger := STATE_TRANSPARENT;
    1: nInteger := POISON_STONE;
    2: nInteger := POISON_DONTMOVE;
    3: nInteger := POISON_68;
    4: nInteger := POISON_DECHEALTH;
    5: nInteger := POISON_LOCKSPELL;
    6: nInteger := POISON_DAMAGEARMOR;
    7: nInteger := STATE_FROZEN;
    else
    begin
    m_nFixColorIdx := 0;
    nInteger := STATE_TRANSPARENT;
    end;
    end;
    m_nCharStatus := (m_nCharStatusEx and $FFFFF) or (($80000000 shr nInteger) or 0);
    m_nFixStatus := m_nCharStatus;
    StatusChanged();
    end;
  }
  inherited;
end;

procedure TNormNpc.ScriptActionError(BaseObject: TBaseObject; sErrMsg: string; QuestActionInfo: pTQuestActionInfo);
var
  sMsg: string;
resourcestring
  sOutMessage = '[脚本错误] %s 脚本命令:%s NPC名称:%s 地图:%s(%d:%d) 参数1:%s 参数2:%s 参数3:%s 参数4:%s 参数5:%s 参数6:%s 参数7:%s 参数8:%s 参数9:%s 参数10:%s';
begin
  sMsg := Format(sOutMessage, [sErrMsg, QuestActionInfo.sCmd, m_sCharName, m_sMapName, m_nCurrX, m_nCurrY, QuestActionInfo.sParam1, QuestActionInfo.sParam2, QuestActionInfo.sParam3, QuestActionInfo.sParam4, QuestActionInfo.sParam5, QuestActionInfo.sParam6, QuestActionInfo.sParam7, QuestActionInfo.sParam8, QuestActionInfo.sParam9, QuestActionInfo.sParam10]);
  {
    sMsg:='脚本命令:' + sCmd +
    ' NPC名称:' + m_sCharName +
    ' 地图:' + m_sMapName +
    ' 座标:' + IntToStr(m_nCurrX) + ':' + IntToStr(m_nCurrY) +
    ' 参数1:' + QuestActionInfo.sParam1 +
    ' 参数2:' + QuestActionInfo.sParam2 +
    ' 参数3:' + QuestActionInfo.sParam3 +
    ' 参数4:' + QuestActionInfo.sParam4 +
    ' 参数5:' + QuestActionInfo.sParam5 +
    ' 参数6:' + QuestActionInfo.sParam6;
  }
  MainOutMessage(sMsg);
end;

procedure TNormNpc.ScriptConditionError(BaseObject: TBaseObject; QuestConditionInfo: pTQuestConditionInfo);
var
  sMsg: string;
resourcestring
  sOutMessage = '[脚本错误] 脚本命令:%s NPC名称:%s 地图:%s(%d:%d) 参数1:%s 参数2:%s 参数3:%s 参数4:%s 参数5:%s 参数6:%s 参数7:%s 参数8:%s 参数9:%s 参数10:%s';
begin
  sMsg := Format(sOutMessage, [QuestConditionInfo.sCmd, m_sCharName, m_sMapName, m_nCurrX, m_nCurrY, QuestConditionInfo.sParam1, QuestConditionInfo.sParam2, QuestConditionInfo.sParam3, QuestConditionInfo.sParam4, QuestConditionInfo.sParam5, QuestConditionInfo.sParam6, QuestConditionInfo.sParam7, QuestConditionInfo.sParam8, QuestConditionInfo.sParam9, QuestConditionInfo.sParam10]);
  { var
    sMsg: string;
    begin
    sMsg := 'Cmd:' + sCmd +
    ' NPC名称:' + m_sCharName +
    ' 地图:' + m_sMapName +
    ' 座标:' + IntToStr(m_nCurrX) + ':' + IntToStr(m_nCurrY) +
    ' 参数1:' + QuestConditionInfo.sParam1 +
    ' 参数2:' + QuestConditionInfo.sParam2 +
    ' 参数3:' + QuestConditionInfo.sParam3 +
    ' 参数4:' + QuestConditionInfo.sParam4 +
    ' 参数5:' + QuestConditionInfo.sParam5;
    MainOutMessage('[脚本参数不正确] ' + sMsg); }
end;

procedure TNormNpc.SendMsgToUser(PlayObject: TPlayObject; sMsg: string; boShowNPCName: Boolean); // 0049AD14
begin
  if g_OnlineMsgControl.boDisableUseNpc then
    Exit;

  if boShowNPCName then
    PlayObject.SendMsg(Self, RM_MERCHANTSAY, 0, 0, 0, 0, m_sCharName + '/' + sMsg)
  else
    PlayObject.SendMsg(Self, RM_MERCHANTSAY, 0, 0, 0, 0, sMsg)
end;

procedure TNormNpc.MessageBox(PlayObject: TPlayObject; sMsg: string);
var
  IsBreakParseVar: Boolean;
begin
  PlayObject.SendMsg(Self, RM_MENU_OK, 0, NativeInt(Self), 0, 0, GetLineVariableText(PlayObject, sMsg, IsBreakParseVar));
end;

procedure TNormNpc.UserSelect(PlayObject: TPlayObject; sData: string);
var
  sMsg, sLabel: string;
begin
  PlayObject.m_nScriptGotoCount := 0;

  // 处理脚本命令 @back 返回上级标签内容
  if (sData <> '') and (sData[1] = '@') then
  begin
    sMsg := GetValidStr3_Ex(sData, sLabel, #13);
    if sLabel = '@HeroMap' then
      GotoLable(PlayObject, sLabel, False) // 支持卧龙笔记移动 piaoyun 2013-08-20
    else if (PlayObject.m_sScriptCurrLable <> sLabel) then
    begin
      if not SameText(sLabel, sNF_Back) then
      begin
        PlayObject.m_sScriptGoBackLable := PlayObject.m_sScriptCurrLable;
        PlayObject.m_sScriptCurrLable := sLabel;
      end
      else
      begin
        if PlayObject.m_sScriptCurrLable <> '' then
          PlayObject.m_sScriptCurrLable := ''
        else
          PlayObject.m_sScriptGoBackLable := '';
      end;
    end;
  end;
end;

procedure TNormNpc.SendCustemMsg(PlayObject: TPlayObject; sMsg: string);
var
  sNewMsg: string;
begin
  if not g_Config.boSendCustemMsg then
  begin
    PlayObject.SysMsg(g_sSendCustMsgCanNotUseNowMsg, c_Red, t_Hint);
    Exit;
  end;

  if (g_FilterTexts <> nil) and (sMsg <> '') then
  begin
    if g_FilterTexts.Filter(sMsg, sNewMsg) then
    begin // 检测用户输入是否有非法字符
      sMsg := sNewMsg;
      if sMsg = '' then
        Exit;
    end;
  end;

  if PlayObject.m_boSendMsgFlag then
  begin
    PlayObject.m_boSendMsgFlag := False;
    UserEngine.SendBroadCastMsg(PlayObject.m_sCharName + ': ' + sMsg, t_Cust);
  end;
end;

procedure TNormNpc.Initialize;
begin
  inherited;
  m_Castle := g_CastleManager.InCastleWarArea(Self);
  LoadAddData;

  if m_wAppr >= 10000 then
  begin
    m_nWalkSpeed := g_Config.dwCustomNpcMoveTime * 1000;
    m_nInitWalkSpeed := m_nWalkSpeed;
  end;
end;

function TNormNpc.GetDynamicVarList(PlayObject: TPlayObject; sType: string; var sName: string): TList;
begin
  Result := nil;
  if CompareLStr(sType, 'HUMAN', Length('HUMAN')) then
  begin
    Result := PlayObject.m_DynamicVarList;
    sName := PlayObject.m_sCharName;
  end
  else if CompareLStr(sType, 'GUILD', Length('GUILD')) then
  begin
    if PlayObject.m_MyGuild = nil then
      Exit;

    Result := TGUild(PlayObject.m_MyGuild).m_DynamicVarList;
    sName := TGUild(PlayObject.m_MyGuild).sGuildName;
  end
  else if CompareLStr(sType, 'GLOBAL', Length('GLOBAL')) then
  begin
    Result := g_DynamicVarList;
    sName := 'GLOBAL';
  end;
end;

procedure TNormNpc.LoadAddData;
var
  FileName, S: string;
  IniFile: TIniFile;
begin
  if m_sDataFileName <> '' then
  begin
    FileName := g_Config.sEnvirDir + 'Npc_Data\' + m_sDataFileName;
    if FileExists(FileName) then
    begin
      IniFile := TIniFile.Create(FileName);
      m_sAddName := IniFile.ReadString('AddData', 'AddName', '');
      m_nAddLevel := IniFile.ReadInteger('AddData', 'AddLevel', 0);
      m_boGrayShow := IniFile.ReadBool('AddData', 'GrayShow', True);
      m_boScaleShow := IniFile.ReadBool('AddData', 'ScaleShow', True);
      S := IniFile.ReadString('AddData', 'EffigyState', '');
      m_nEffigyState.Value1 := StrToInt64Def(S, 0);
      S := IniFile.ReadString('AddData', 'EffigyState2', '');
      m_nEffigyState.Value2 := StrToInt64Def(S, 0);
      m_nEffigyOffset := IniFile.ReadInteger('AddData', 'EffigyOffset', 0);
      m_nValidTime := IniFile.ReadInteger('AddData', 'ValidTime', -1);
      IniFile.Free;
    end;
  end;
end;

procedure TNormNpc.SaveAddData;
var
  Dir, S: string;
  FileName: string;
  IniFile: TIniFile;
begin
  if m_sDataFileName <> '' then
  begin
    FileName := g_Config.sEnvirDir + 'Npc_Data\' + m_sDataFileName;
    Dir := ExtractFileDir(FileName);
    if not DirectoryExists(Dir) then
      ForceDirectories(Dir);

    IniFile := TIniFile.Create(FileName);
    IniFile.WriteString('AddData', 'AddName', m_sAddName);
    IniFile.WriteInteger('AddData', 'AddLevel', m_nAddLevel);
    IniFile.WriteBool('AddData', 'GrayShow', m_boGrayShow);
    IniFile.WriteBool('AddData', 'ScaleShow', m_boScaleShow);
    S := IntToStr(m_nEffigyState.Value1);
    IniFile.WriteString('AddData', 'EffigyState', S);
    S := IntToStr(m_nEffigyState.Value2);
    IniFile.WriteString('AddData', 'EffigyState2', S);
    IniFile.WriteInteger('AddData', 'EffigyOffset', m_nEffigyOffset);
    IniFile.WriteInteger('AddData', 'ValidTime', m_nValidTime);
    IniFile.Free;
  end;
end;

// 加入排序及二分查找来提高脚本执行效率  chongchong 2016-12-24
procedure TNormNpc.QuickSortRecordList(List: TList; L, R: Integer);
var
  I, J, P: Integer;
  SayingRecord: pTSayingRecord;
begin
  repeat
    I := L;
    J := R;
    P := (L + R) shr 1;
    SayingRecord := List.Items[P];
    repeat
      while CompareText(pTSayingRecord(List[I]).sLabel, SayingRecord.sLabel) < 0 do
        Inc(I);

      while CompareText(pTSayingRecord(List[J]).sLabel, SayingRecord.sLabel) > 0 do
        Dec(J);

      if I <= J then
      begin
        List.Exchange(I, J);
        if P = I then
        begin
          P := J;
          SayingRecord := List.Items[P];
        end
        else if P = J then
        begin
          P := I;
          SayingRecord := List.Items[P];
        end;
        Inc(I);
        Dec(J);
      end;
    until I > J;

    if L < J then
      QuickSortRecordList(List, L, J);
    L := I;
  until I >= R;
end;

procedure TNormNpc.DoSort;
var
  I { , J } : Integer;
  Script: pTScript;
  // SayingRecord: pTSayingRecord;
begin
  for I := 0 to m_ScriptList.Count - 1 do
  begin
    Script := m_ScriptList.Items[I];
    if (Script.RecordList <> nil) and (Script.RecordList.Count > 0) then
    begin
      QuickSortRecordList(Script.RecordList, 0, Script.RecordList.Count - 1);
      {
        for J := 0 to Script.RecordList.Count - 1 do
        begin
        SayingRecord := Script.RecordList.Items[J];
        OutputDebugString(PChar(SayingRecord.sLabel));
        end;
      }
    end;
  end;
end;

function TNormNpc.GetSayingRecordFromRecordList(List: TList; sLabel: string): pTSayingRecord;
var
  L, H, I, C: Integer;
  // SayingRecord: pTSayingRecord;
begin
  Result := nil;
  if (List <> nil) and (Length(sLabel) > 0) then
  begin
    L := 0;
    H := List.Count - 1;
    while L <= H do
    begin
      I := (L + H) shr 1;
      C := CompareText(pTSayingRecord(List.Items[I]).sLabel, sLabel);
      if C < 0 then
        L := I + 1
      else
      begin
        H := I - 1;
        if C = 0 then
        begin
          Result := List.Items[I];
          Break;
        end;
      end;
    end;
  end;
end;

{ TGuildOfficial }
procedure TGuildOfficial.Click(PlayObject: TPlayObject); // 004A30F4
begin
  // GotoLable(PlayObject,'@main');
  inherited;
end;

function TGuildOfficial.GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string; var IsBreakParseVar: Boolean; nPos: Integer): Boolean;
var
  I, II: Integer;
  sText: string;
  List: TStringList;
  sStr: string;
begin
  Result := inherited GetVariableText(PlayObject, sMsg, sVariable, IsBreakParseVar, nPos);
  if not Result then
  begin
    Result := True;
    sVariable := UpperCase(sVariable);
    if sVariable = '$REQUESTCASTLELIST' then
    begin
      sText := '';
      List := TStringList.Create;
      g_CastleManager.GetCastleNameList(List);
      for I := 0 to List.Count - 1 do
      begin
        II := I + 1;
        if ((II div 2) * 2 = II) then
          sStr := '\'
        else
          sStr := '';
        sText := sText + Format('<%s/@requestcastlewarnow%d> %s', [List.Strings[I], I, sStr]);
      end;

      sText := sText + '\ \';
      List.Free;
      sMsg := sub_49ADB8(nPos, sMsg, '<$REQUESTCASTLELIST>', sText);
      Exit;
    end;
    Result := False;
  end;
end;

procedure TGuildOfficial.Run; // 004A37F0
begin
  if Random(40) = 0 then
    TurnTo(Random(8))
  else if Random(30) = 0 then
    SendRefMsg(RM_HIT, m_btDirection, m_nCurrX, m_nCurrY, 0, '');

  inherited;
end;

procedure TGuildOfficial.UserSelect(PlayObject: TPlayObject; sData: string);
var
  sMsg, sLabel: string;
  boCanJmp: Boolean;
resourcestring
  sExceptionMsg = '[Exception] TGuildOfficial.UserSelect... ';
begin
  inherited;
  try
    // PlayObject.m_nScriptGotoCount:=0;
    if (sData <> '') and (sData[1] = '@') then
    begin
      sMsg := GetValidStr3_Ex(sData, sLabel, #13);
      boCanJmp := PlayObject.LableIsCanJmp(sLabel);
      GotoLable(PlayObject, sLabel, not boCanJmp);
      // GotoLable(PlayObject,sLabel,not PlayObject.LableIsCanJmp(sLabel));
      if not boCanJmp then
        Exit;

      if SameText(sLabel, sNF_BuildGuildNow) then
      begin
        ReQuestBuildGuild(PlayObject, sMsg);
      end
      else if SameText(sLabel, sNF_GuildWar) then
      begin
        ReQuestGuildWar(PlayObject, sMsg);
      end
      else if SameText(sLabel, sNF_Donate) then
      begin
        DoNate(PlayObject);
      end
      else if CompareLStr(sLabel, sNF_RequestCastleWar, Length(sNF_RequestCastleWar)) then
      begin
        ReQuestCastleWar(PlayObject, Copy(sLabel, Length(sNF_RequestCastleWar) + 1, Length(sLabel) - Length(sNF_RequestCastleWar)));
      end
      else if SameText(sLabel, sNF_Exit) then
      begin
        PlayObject.SendMsg(Self, RM_MERCHANTDLGCLOSE, 0, NativeInt(Self), 0, 0, '');
      end
      else if SameText(sLabel, sNF_Back) then
      begin
        if PlayObject.m_sScriptGoBackLable = '' then
          PlayObject.m_sScriptGoBackLable := sNF_Main;
        GotoLable(PlayObject, PlayObject.m_sScriptGoBackLable, False);
      end;
    end;
  except
    MainOutMessage(sExceptionMsg);
  end;
  // inherited;
end;

function TGuildOfficial.ReQuestBuildGuild(PlayObject: TPlayObject; sGuildName: string): Integer; // 004A3124
var
  UserItem: pTUserItem;
begin
  Result := 0;
  sGuildName := Trim(sGuildName);
  UserItem := nil;
  { TODO -ochongchong -c新增 : 创建行会时名称过滤 【2013-07-24】 }
  if Length(sGuildName) = 0 then
  begin
    GotoLable(PlayObject, '@GuildNameFilter', False);
    Result := -4;
  end;

  if GetNameInFilterList(sGuildName) then
  begin
    GotoLable(PlayObject, '@GuildNameFilter', False);
    Result := -4;
  end;

  { TODO -ochongchong -c新增 : 创建行会时名称长度过滤 【2013-07-24】 }
  if not CheckGuildName(sGuildName) then
  begin
    GotoLable(PlayObject, '@GuildNameFilter', False);
    Result := -4;
  end;

  if Result = 0 then
  begin
    if PlayObject.m_MyGuild = nil then
    begin
      if PlayObject.m_nGold >= g_Config.nBuildGuildPrice then
      begin
        UserItem := PlayObject.CheckItems(g_Config.sWomaHorn);
        if UserItem = nil then
        begin
          Result := -3;
          // '你没有准备好需要的全部物品。'
        end;
      end
      else
        Result := -2; // '缺少创建费用。'
    end
    else
      Result := -1; // '您已经加入其它行会。'
  end;

  if Result = 0 then
  begin
    if g_GuildManager.AddGuild(sGuildName, PlayObject.m_sCharName) then
    begin
      PlayObject.SendDelItem(UserItem);
      PlayObject.DelBagItem(UserItem.MakeIndex, g_Config.sWomaHorn);
      PlayObject.DecGold(g_Config.nBuildGuildPrice);
      PlayObject.GoldChanged();
      PlayObject.m_MyGuild := g_GuildManager.MemberOfGuild(PlayObject.m_sCharName);
      if PlayObject.m_MyGuild <> nil then
      begin
        PlayObject.m_sGuildRankName := TGUild(PlayObject.m_MyGuild).GetRankName2(PlayObject, PlayObject.m_nGuildRankNo);
        PlayObject.RefShowName();
        // 创建行会触发 chongchong 2013-10-28
        if (g_FunctionNPC <> nil) then
        begin
          PlayObject.m_nScriptGotoCount := 0;
          g_FunctionNPC.GotoLable(PlayObject, '@CreateGuild', False);
        end;
      end;
    end
    else
      Result := -4;
  end;

  if Result >= 0 then
    PlayObject.SendMsg(Self, RM_BUILDGUILD_OK, 0, 0, 0, 0, '')
  else
    PlayObject.SendMsg(Self, RM_BUILDGUILD_FAIL, 0, Result, 0, 0, '');
end;

function TGuildOfficial.ReQuestGuildWar(PlayObject: TPlayObject; sGuildName: string): Integer; // 004A3368
begin
  if g_GuildManager.FindGuild(sGuildName) <> nil then
  begin
    if PlayObject.m_nGold >= g_Config.nGuildWarPrice then
    begin
      PlayObject.DecGold(g_Config.nGuildWarPrice);
      PlayObject.GoldChanged();
      PlayObject.ReQuestGuildWar(sGuildName);
    end
    else
    begin
      PlayObject.SysMsg('你没有足够的金币！', c_Red, t_Hint);
    end;
  end
  else
    PlayObject.SysMsg('行会 ' + sGuildName + ' 不存在！', c_Red, t_Hint);

  Result := 1;
end;

procedure TGuildOfficial.DoNate(PlayObject: TPlayObject); // 004A346C
begin
  PlayObject.SendMsg(Self, RM_DONATE_OK, 0, 0, 0, 0, '');
end;

procedure TGuildOfficial.ReQuestCastleWar(PlayObject: TPlayObject; sIndex: string); // 004A3498
var
  UserItem: pTUserItem;
  Castle: TUserCastle;
  nIndex: Integer;
begin
  // if PlayObject.IsGuildMaster and
  // (not UserCastle.IsMasterGuild(TGuild(PlayObject.m_MyGuild))) then begin
  nIndex := StrToIntDef(sIndex, -1);
  if nIndex < 0 then
    nIndex := 0;

  Castle := g_CastleManager.GetCastle(nIndex);
  if PlayObject.IsGuildMaster and not Castle.IsMember(PlayObject) then
  begin
    UserItem := PlayObject.CheckItems(g_Config.sZumaPiece);
    if UserItem <> nil then
    begin
      if Castle.AddAttackerInfo(TGUild(PlayObject.m_MyGuild)) then
      begin
        PlayObject.SendDelItem(UserItem);
        PlayObject.DelBagItem(UserItem.MakeIndex, g_Config.sZumaPiece);
        GotoLable(PlayObject, '~@request_ok', False);
      end
      else
        PlayObject.SysMsg('你现在无法请求攻城！', c_Red, t_Hint);
      (* {$IFEND} *)
    end
    else
      PlayObject.SysMsg('你没有' + g_Config.sZumaPiece + '！', c_Red, t_Hint);
  end
  else
    PlayObject.SysMsg('你的请求被取消！', c_Red, t_Hint);
end;

procedure TCastleOfficial.RepairDoor(PlayObject: TPlayObject); // 004A3FB8
begin
  if m_Castle = nil then
  begin
    PlayObject.SysMsg('NPC不属于城堡！', c_Red, t_Hint);
    Exit;
  end;

  if TUserCastle(m_Castle).m_nTotalGold >= g_Config.nRepairDoorPrice then
  begin
    if TUserCastle(m_Castle).RepairDoor then
    begin
      Dec(TUserCastle(m_Castle).m_nTotalGold, g_Config.nRepairDoorPrice);
      PlayObject.SysMsg('修理成功。', c_Green, t_Hint);
    end
    else
    begin
      PlayObject.SysMsg('城门不需要修理！', c_Green, t_Hint);
    end;
  end
  else
    PlayObject.SysMsg('城内资金不足！', c_Red, t_Hint);
  {
    if UserCastle.m_nTotalGold >= g_Config.nRepairDoorPrice then begin
    if UserCastle.RepairDoor then begin
    Dec(UserCastle.m_nTotalGold,g_Config.nRepairDoorPrice);
    PlayObject.SysMsg('修理成功。',c_Green,t_Hint);
    end else begin
    PlayObject.SysMsg('城门不需要修理！',c_Green,t_Hint);
    end;
    end else begin
    PlayObject.SysMsg('城内资金不足！',c_Red,t_Hint);
    end;
  }
end;

procedure TCastleOfficial.RepairWallNow(nWallIndex: Integer; PlayObject: TPlayObject); // 004A4074
begin
  if m_Castle = nil then
  begin
    PlayObject.SysMsg('NPC不属于城堡！', c_Red, t_Hint);
    Exit;
  end;

  if TUserCastle(m_Castle).m_nTotalGold >= g_Config.nRepairWallPrice then
  begin
    if TUserCastle(m_Castle).RepairWall(nWallIndex) then
    begin
      Dec(TUserCastle(m_Castle).m_nTotalGold, g_Config.nRepairWallPrice);
      PlayObject.SysMsg('修理成功。', c_Green, t_Hint);
    end
    else
    begin
      PlayObject.SysMsg('城门不需要修理！', c_Green, t_Hint);
    end;
  end
  else
    PlayObject.SysMsg('城内资金不足！', c_Red, t_Hint);
  {
    if UserCastle.m_nTotalGold >= g_Config.nRepairWallPrice then begin
    if UserCastle.RepairWall(nWallIndex) then begin
    Dec(UserCastle.m_nTotalGold,g_Config.nRepairWallPrice);
    PlayObject.SysMsg('修理成功。',c_Green,t_Hint);
    end else begin
    PlayObject.SysMsg('城门不需要修理！',c_Green,t_Hint);
    end;
    end else begin
    PlayObject.SysMsg('城内资金不足！',c_Red,t_Hint);
    end;
  }
end;

constructor TCastleOfficial.Create;
begin
  inherited;
end;

destructor TCastleOfficial.Destroy;
begin
  inherited;
end;

constructor TGuildOfficial.Create;
begin
  inherited;
  m_btRaceImg := RC_MERCHANT;
  m_wAppr := 8;
end;

destructor TGuildOfficial.Destroy;
begin
  inherited;
end;

procedure TGuildOfficial.SendCustemMsg(PlayObject: TPlayObject; sMsg: string);
begin
  inherited;
end;

procedure TCastleOfficial.SendCustemMsg(PlayObject: TPlayObject; sMsg: string);
begin
  if not g_Config.boSubkMasterSendMsg then
  begin
    PlayObject.SysMsg(g_sSubkMasterMsgCanNotUseNowMsg, c_Red, t_Hint);
    Exit;
  end;

  if PlayObject.m_boSendMsgFlag then
  begin
    PlayObject.m_boSendMsgFlag := False;
    UserEngine.SendBroadCastMsg(PlayObject.m_sCharName + ': ' + sMsg, t_Castle);
  end;
end;

function CheckStrIsVar(sText: string): Boolean;
var
  n18: Integer;
  s14: string;
begin
  Result := False;
  if (Length(sText) > 3) and (Pos('<', sText) > 0) and (Pos('$', sText) > 0) and (Pos('>', sText) > 0) then
  begin
    sText := UpperCase(sText);
    ArrestVariable(sText, '<', '$', '>', 0, sText);
    if CompareLStr(sText, '$HUMAN(', Length('$HUMAN(')) then
    begin
      Result := True;
    end;
    if CompareLStr(sText, '$GUILD(', Length('$GUILD(')) then
    begin
      Result := True;
      Exit;
    end;
    if CompareLStr(sText, '$GLOBAL(', Length('$GLOBAL(')) then
    begin
      Result := True;
      Exit;
    end;
    if CompareLStr(sText, '$STR(', Length('$STR(')) then
    begin
      ArrestStringEx(sText, '(', ')', s14);

      n18 := GetValNameNo(s14);
      if n18 >= 0 then
      begin
        case n18 of
          0..999:
            begin // P
              Result := True;
            end;
          1000..1999:
            begin
              Result := True;
            end;
          2000..2999:
            begin
              Result := True;
            end;
          3000..3999:
            begin
              Result := True;
            end;
          4000..4999:
            begin
              Result := True;
            end;
          5000..5999:
            begin
              Result := True;
            end;
          6000..6999:
            begin
              Result := True;
            end;
          7000..7999:
            begin
              Result := True;
            end;
          // 私有变量 U-数字型 chongchong 2014-10-18
          8000..8499:
            begin
              Result := True;
            end;
          // 私有变量 T-字符串型 chongchong 2014-10-18
          8500..8999:
            begin
              Result := True;
            end;
          // 私有变量 J-数字型(1天1清) chongchong 2014-10-18
          9000..9499:
            begin
              Result := True;
            end;
          9500..9999:
            begin
              Result := True;
            end;
        end;
      end
      //L变量
      else if (Length(s14) > 2) and (UpCase(s14[1]) = 'L') and (s14[2] = '$') then
      begin
        Result := True;
      end
      else if (Length(s14) > 2) and (UpCase(s14[1]) = 'S') and (s14[2] = '$') then
      begin
        Result := True;
      end
      else if (Length(s14) > 2) and (UpCase(s14[1]) = 'N') and (s14[2] = '$') then
      begin
        Result := True;
      end;
    end;
  end;
end;

{ TBoxMonster }

constructor TBoxMonster.Create;
begin
  inherited;
  m_btRaceServer := RC_BOX;
end;

destructor TBoxMonster.Destroy;
begin
  inherited;
end;

procedure TBoxMonster.Initialize;
begin
  m_btDirection := Random(3);
  inherited;
end;

function TBoxMonster.Operate(ProcessMsg: pTProcessMessage): Boolean;
begin
  Result := False;
  { if ProcessMsg.wIdent = RM_MAGSTRUCK then
    Result := inherited Operate(ProcessMsg); }
end;

procedure TBoxMonster.Run;
begin
  if not m_boDeath then // 采集怪物死亡生命值归零 用于CHECKHPPER判断是否死亡 By 一支笔 at:2021-07-03 11:07:34
    m_WAbil.HP := m_WAbil.MaxHP
  else
    m_WAbil.HP := 0;
  if m_Master <> nil then
    m_Master := nil;
  inherited Run;
end;

end.


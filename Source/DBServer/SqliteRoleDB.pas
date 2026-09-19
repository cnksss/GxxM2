unit SqliteRoleDB;

interface

uses
  Windows, Classes, SysUtils, RoleDB, SQLite3DataBase, SQLiteCli, Grobal2, DBShare,
  SqliteCreateTableSql;

type
  TSqliteHumanDB = class(THumanDB)
  private
    FStatementGetID: TSQLStatement;
    FStatementCheckHumanExists: TSQLStatement;
    FStatementGetHumanCount: TSQLStatement;
    FStatementGetOtherHumanName: TSQLStatement;
    FStatementGetHumanHeroName: TSQLStatement;
    FStatementGetHumanHeroID: TSQLStatement;
    FStatementGetHumanGoldInfo: TSQLStatement;
    FStatementUpdateHumanGoldInfo: TSQLStatement;
    FStatementGetHumanBaseInfo: TSQLStatement;
    FStatementQueryHumans: TSQLStatement;
    FStatementSearchByAccountMatchComplete: TSQLStatement;
    FStatementSearchByAccountMatchFuzzy: TSQLStatement;
    FStatementSearchByNameMatchComplete: TSQLStatement;
    FStatementSearchByNameMatchFuzzy: TSQLStatement;
    FStatementSearchByLevel: TSQLStatement;
    FStatementGetMobileNumbers1: TSQLStatement;
    FStatementGetMobileNumbers2: TSQLStatement;
    FStatementSelect: TSQLStatement;
    FStatementUnSelect: TSQLStatement;
    FStatementGetHuman: TSQLStatement;
    FStatementGetAbil: TSQLStatement;
    FStatementGetAbilNG: TSQLStatement;
    FStatementGetAbilWine: TSQLStatement;
    FStatementGetAbilNpcAdd: TSQLStatement;
    FStatementGetGamePetData: TSQLStatement;
    FStatementGetGodBlessState: TSQLStatement;
    FStatementGetMagic: TSQLStatement;
    FStatementGetMagicUseTick: TSQLStatement;
    FStatementGetStatusTime: TSQLStatement;
    FStatementGetQuestFlag: TSQLStatement;
    FStatementGetVariableU: TSQLStatement;
    FStatementGetVariableT: TSQLStatement;
    FStatementGetVariableJ: TSQLStatement;
    FStatementGetVariableZ: TSQLStatement;
    FStatementGetItems: TSQLStatement;
    FStatementGetItemValueAdd: TSQLStatement;
    FStatementGetItemElementAdd: TSQLStatement;
    FStatementGetItemAddDataByte: TSQLStatement;
    FStatementGetItemAddDataInt: TSQLStatement;
    FStatementGetItemAddDataText: TSQLStatement;
    FStatementGetItemFlute: TSQLStatement;
    FStatementGetItemProgress: TSQLStatement;
    FStatementGetItemProperty: TSQLStatement;
    FStatementGetSkillPower: TSQLStatement;
    FStatementAddHuman: TSQLStatement;
    FStatementDeleteOrRestore: TSQLStatement;
    FStatementRecordLoginTime: TSQLStatement;
    FStatementUpdateHumanName: TSQLStatement;
    FStatementUpdateHumanNameDearName: TSQLStatement;
    FStatementUpdateHumanNameMasterName: TSQLStatement;
    FStatementUpdateHuman: TSQLStatement;
    FStatementInsertAbil: TSQLStatement;
    FStatementInsertAbilNG: TSQLStatement;
    FStatementInsertAbilWine: TSQLStatement;
    FStatementUpdateAbil: TSQLStatement;
    FStatementUpdateAbilNG: TSQLStatement;
    FStatementUpdateAbilWine: TSQLStatement;
    FStatementInsertAbilNpcAdd: TSQLStatement;
    FStatementInsertGamePetData: TSQLStatement;
    FStatementInsertGodBlessState: TSQLStatement;
    FStatementInsertMagic: TSQLStatement;
    FStatementInsertMagicUseTick: TSQLStatement;
    FStatementInsertStatusTime: TSQLStatement;
    FStatementInsertQuestFlag: TSQLStatement;
    FStatementInsertVariableU: TSQLStatement;
    FStatementInsertVariableT: TSQLStatement;
    FStatementInsertVariableJ: TSQLStatement;
    FStatementInsertVariableZ: TSQLStatement;
    FStatementInsertItems: TSQLStatement;
    FStatementInsertItemValueAdd: TSQLStatement;
    FStatementInsertItemElementAdd: TSQLStatement;
    FStatementInsertItemAddDataByte: TSQLStatement;
    FStatementInsertItemAddDataInt: TSQLStatement;
    FStatementInsertItemAddDataText: TSQLStatement;
    FStatementInsertItemFlute: TSQLStatement;
    FStatementInsertItemProgress: TSQLStatement;
    FStatementInsertItemProperty: TSQLStatement;
    FStatementInsertSkillPower: TSQLStatement;
    FStatementGetLevelRankCheckLevel: TSQLStatement;
    FStatementGetLevelRankTopCount: TSQLStatement;
    FStatementGetLevelRankCheckLevelAndCount: TSQLStatement;
    FStatementGetMasterRankCheckLevel: TSQLStatement;
    FStatementGetMasterRankTopCount: TSQLStatement;
    FStatementGetMasterRankCheckLevelAndCount: TSQLStatement;
    FStatementBuyPlayer: TSQLStatement;
    FStatementGetCustomMoney: TSQLStatement;
    FStatementInsertCustomMoney: TSQLStatement;
    FStatementGetCustomMoneyByName: TSQLStatement;
    FStatementUpdateCustomMoney: TSQLStatement;
  private
    procedure BeginTransaction;
    procedure Commit;
    procedure RollBack;
    procedure Execute(Sql: string);
    function GetHumanUserItem(HumData: PTHumData; const ItemType, ItemIndex: Integer): PTUserItem;
    function SaveHumanData(HumanID: Integer; HumData: PTHumData): Boolean;
    procedure AddHumanItemToDB(UserItem: PTUserItem; HumanID, ItemType, ItemIndex: Integer);
    procedure ResetAllGetDataStatement;
    procedure ResetAllSaveDataStatement;
  protected
    procedure DoInit; override;
    procedure DoFinal; override;
    function DoGetID(HumanName: string): Integer; override;
    function DoCheckHumanExists(Account, HumanName: string): Boolean; override;
    function DoGetHumanCount(Account: string): Integer; override;
    function DoGetOtherHumanName(Account, HumanName: string): string; override;
    function DoGetHumanHeroName(Account, HumanName: string; var HeroName, DeputyHeroName: string): Boolean; override;
    function DoGetBaseInfo(HumanName: string; var Sex, Job, Level, LastLogin: Integer): Boolean; override;
    function DoQueryHumans(Account: string; HumanList: TQueryHumanList): Integer; override;
    function DoQueryDeleteHumans(Account: string; HumanList: TQueryHumanList): Integer; override;
    function DoSearchByAccount(Account: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer; override;
    function DoSearchByName(HumanName: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer; override;
    function DoSearchByLevel(LimitCount, MinLevel: Integer; RoleList: TSerarchRoleList): Integer; override;
    function DoGetMobileNumbers(OnlyBindMobile: Boolean; MobileNumberList: TStrings): Integer; override;
    function DoSelect(Account, HumanName: string): Boolean; override;
    function DoGet(Account, HumanName: string; var HumData: THumData; var HumanID: Integer): Boolean; override;
    function DoAdd(Account, HumanName: string; IsSelect: Boolean; Sex, Job, Hair: Byte): Boolean; override;
    function DoDelete(Account, HumanName: string): Boolean; override;
    function DoDeleteRestore(Account, HumanName: string): Boolean; override;
    function DoSetEnabled(Account, HumanName: string; Enabled: Integer): Boolean; override;
    function DoErase(Account, HumanName: string): Boolean; override;
    function DoRecordLoginTime(Account, HumanName: string): Boolean; override;
    function DoSave(HumanID: Integer; HumData: PTHumData): Boolean; override;
    function DoRename(Account, HumanName: string; HumanID: Integer; NewName: string): Boolean; override;
    function DoChangedGold(HumanName: string; ChangeType: TDBChangeGoldType; ChangedValue: Integer; var ResultValue: LongWord): Boolean; override;
    function DoChangedCustomMoney(HumanID: Integer; CustomMoneyName: string; ChangedValue: Integer; var ResultValue: LongWord): Boolean; override;
    procedure DoGetRankData(MinLevel, MaxLevel, TopCount: LongWord; HumanRankList, WarriorRankList, WizardRankList, TaoistRankList, MasterRankList: TRoleRankList); override;
    function DoBuyPlayer(const sSellAccount, sSellHumanName, sBuyAccount, sBuyHumanName: string): Boolean; override;
  end;

  TSqliteHeroDB = class(THeroDB)
    FStatementGetID: TSQLStatement;
    FStatementGetHeroCount: TSQLStatement;
    FStatementGetHumanInfo: TSQLStatement;
    FStatementGetHumanInfo2: TSQLStatement;
    FStatementSearchByAccountMatchComplete: TSQLStatement;
    FStatementSearchByAccountMatchFuzzy: TSQLStatement;
    FStatementSearchByNameMatchComplete: TSQLStatement;
    FStatementSearchByNameMatchFuzzy: TSQLStatement;
    FStatementGetHero: TSQLStatement;
    FStatementGetAbil: TSQLStatement;
    FStatementGetAbilNG: TSQLStatement;
    FStatementGetAbilWine: TSQLStatement;
    FStatementGetAbilNpcAdd: TSQLStatement;
    FStatementGetGodBlessState: TSQLStatement;
    FStatementGetMagic: TSQLStatement;
    FStatementGetStatusTime: TSQLStatement;
    FStatementGetQuestFlag: TSQLStatement;
    FStatementGetItems: TSQLStatement;
    FStatementGetItemValueAdd: TSQLStatement;
    FStatementGetItemElementAdd: TSQLStatement;
    FStatementGetItemAddDataByte: TSQLStatement;
    FStatementGetItemAddDataInt: TSQLStatement;
    FStatementGetItemAddDataText: TSQLStatement;
    FStatementGetItemFlute: TSQLStatement;
    FStatementGetItemProgress: TSQLStatement;
    FStatementGetItemProperty: TSQLStatement;
    FStatementGetSkillPower: TSQLStatement;
    FStatementAddHero: TSQLStatement;
    FStatementDeleteOrRestore: TSQLStatement;
    FStatementUpdateHeroName: TSQLStatement;
    FStatementUpdateHumanHeroName: TSQLStatement;
    FStatementUpdateHumanHeroName2: TSQLStatement;
    FStatementUpdateHero: TSQLStatement;
    FStatementInsertAbil: TSQLStatement;
    FStatementInsertAbilNG: TSQLStatement;
    FStatementInsertAbilWine: TSQLStatement;
    FStatementUpdateAbil: TSQLStatement;
    FStatementUpdateAbilNG: TSQLStatement;
    FStatementUpdateAbilWine: TSQLStatement;
    FStatementInsertAbilNpcAdd: TSQLStatement;
    FStatementInsertGodBlessState: TSQLStatement;
    FStatementInsertMagic: TSQLStatement;
    FStatementInsertStatusTime: TSQLStatement;
    FStatementInsertQuestFlag: TSQLStatement;
    FStatementInsertItems: TSQLStatement;
    FStatementInsertItemValueAdd: TSQLStatement;
    FStatementInsertItemElementAdd: TSQLStatement;
    FStatementInsertItemAddDataByte: TSQLStatement;
    FStatementInsertItemAddDataInt: TSQLStatement;
    FStatementInsertItemAddDataText: TSQLStatement;
    FStatementInsertItemFlute: TSQLStatement;
    FStatementInsertItemProgress: TSQLStatement;
    FStatementInsertItemProperty: TSQLStatement;
    FStatementInsertSkillPower: TSQLStatement;
    FStatementGetLevelRankCheckLevel: TSQLStatement;
    FStatementGetLevelRankTopCount: TSQLStatement;
    FStatementGetLevelRankCheckLevelAndCount: TSQLStatement;
  private
    procedure BeginTransaction;
    procedure Commit;
    procedure RollBack;
    procedure Execute(Sql: string);
    function GetHeroUserItem(HeroData: PTHeroData; const ItemType, ItemIndex: Integer): PTUserItem;
    function SaveHeroData(HeroID: Integer; HeroData: PTHeroData): Boolean;
    procedure AddHeroItemToDB(UserItem: PTUserItem; HeroID, ItemType, ItemIndex: Integer);
    procedure ResetAllGetDataStatement;
    procedure ResetAllSaveDataStatement;
  protected
    procedure DoInit; override;
    procedure DoFinal; override;
    function DoGetID(HeroName: string): Integer; override;
    function DoSearchByAccount(Account: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer; override;
    function DoSearchByName(HeroName: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer; override;
    function DoGet(HeroName: string; var HeroData: THeroData; var HeroID: Integer): Boolean; override;
    function DoAdd(Account, HumanName: string; HumanID: Integer; HeroName: string; Sex, Job, Hair: Byte; IsDeputyHero: Boolean): Boolean; override;
    //function DoDelete(HeroName: string): Boolean; override;
    //function DoDeleteRestore(HeroName: string): Boolean; override;
    function DoErase(HeroName: string): Boolean; override;
    function DoSave(HeroID: Integer; HeroData: PTHeroData): Boolean; override;
    function DoRename(HeroID: Integer; HeroName, NewName: string): Boolean; override;
    function DoAssess(HeroID: Integer; HeroName, DeputyHeroName: string): Boolean; override;
    procedure DoGetRankData(MinLevel, MaxLevel, TopCount: LongWord; HeroRankList, WarriorRankList, WizardRankList, TaoistRankList: TRoleRankList); override;
  end;

  TSqliteRoleDB = class(TRoleDB)
  private
    FFileName: string;
    FDB: TSqlite3Database;
    procedure UpdateDB_1;
    procedure UpdateDB_2;
    procedure UpdateDB_3;
    procedure UpdateDB_4;
    procedure UpdateDB_5;
    procedure UpdateDB_6;
    procedure UpdateDB_7;
    procedure UpdateDB_8;
    procedure UpdateDB_9;
    procedure UpdateDB_10;
    procedure UpdateDB_11;
    procedure UpdateDB_12;
    procedure UpdateDB_13;
    procedure UpdateDB_14;
    procedure UpdateDB_15;
    procedure UpdateDB_16;
    procedure UpdateDB_17;
    procedure UpdateDB_18;
  protected
    procedure DoInit; override;
    procedure DoFainal; override;
    function GetHumanDBClass: THumanDBClass; override;
    function GetHeroDBClass: THeroDBClass; override;
  public
    constructor Create; override;
    property FileName: string read FFileName write FFileName;
    procedure BeginTransaction;
    procedure Commit;
    procedure RollBack;
  end;

implementation

const
  AlterFieldHuamName_HeroName = 'ALTER TABLE "main"."Human" RENAME TO "_Human_old_20170420";' + sLineBreak +
'DROP INDEX "main"."idx_Account";' + sLineBreak +
'DROP INDEX "main"."idx_HumanName";' + sLineBreak +
'CREATE TABLE "main"."Human" (' + sLineBreak + '	"HumanID" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,' + sLineBreak + '	"Account" TEXT (10) NOT NULL COLLATE NOCASE,' + sLineBreak +                    // 忽略大小写
  '	"HumanName" TEXT (14) NOT NULL COLLATE NOCASE,' + sLineBreak +                  // 忽略大小写
  '	"IsDelete" INTEGER,' + sLineBreak + '	"IsSelect" INTEGER,' + sLineBreak + '	"CreateDate" INTEGER,' + sLineBreak + '	"LoginDate" INTEGER,' + sLineBreak + '	"Sex" INTEGER,' + sLineBreak + '	"Job" INTEGER,' + sLineBreak + '	"Hair" INTEGER,' + sLineBreak + '	"Dir" INTEGER,' + sLineBreak + '	"Level" INTEGER DEFAULT 0,' + sLineBreak + '	"ReLevel" INTEGER DEFAULT 0,' + sLineBreak + '	"Map" TEXT (30),' + sLineBreak + '	"X" INTEGER,' + sLineBreak + '	"Y" INTEGER,' + sLineBreak + '	"HomeMap" TEXT (30),' +
  sLineBreak + '	"HomeX" INTEGER,' + sLineBreak + '	"HomeY" INTEGER,' + sLineBreak + '	"AttackMode" INTEGER,' + sLineBreak + '	"StoragePassword" TEXT (7),' + sLineBreak + '	"CreditPoint" INTEGER,' + sLineBreak + '	"Gold" INTEGER,' + sLineBreak + '	"GameGold" INTEGER,' + sLineBreak + '	"GamePoint" INTEGER,' + sLineBreak + '	"GameDiamond" INTEGER,' + sLineBreak + '	"GameGird" INTEGER,' + sLineBreak + '	"GameGoldEx" INTEGER,' + sLineBreak + '	"GameGlory" INTEGER,' + sLineBreak + '	"PKPoint" INTEGER,' +
  sLineBreak + '	"MasterName" TEXT (14),' + sLineBreak + '	"MasterCount" INTEGER,' + sLineBreak + '	"MarryCount" INTEGER,' + sLineBreak + '	"DearName" TEXT (14),' + sLineBreak + '	"IncHP" INTEGER,' + sLineBreak + '	"IncMP" INTEGER,' + sLineBreak + '	"IncHP2" INTEGER,' + sLineBreak + '	"FightZoneDieCount" INTEGER,' + sLineBreak + '	"BodyLuck" REAL,' + sLineBreak + '	"Contribution" INTEGER,' + sLineBreak + '	"HungerStatus" INTEGER,' + sLineBreak + '	"KickCount" INTEGER,' + sLineBreak +
  '	"IsLockLogin" INTEGER,' + sLineBreak + '	"IsAllowGroup" INTEGER,' + sLineBreak + '	"IsAllowGroupRecall" INTEGER,' + sLineBreak + '	"GroupRecallTime" INTEGER,' + sLineBreak + '	"IsAllowGuildReCall" INTEGER,' + sLineBreak + '	"IsDisableTrading" INTEGER,' + sLineBreak + '	"IsDisableInviteHorseRiding" INTEGER,' + sLineBreak + '	"IsGameGoldTrading" INTEGER,' + sLineBreak + '	"IsNewServer" INTEGER,' + sLineBreak +
    //'	"IsFilterGlobalMsg" INTEGER,' + sLineBreak +
  '	"IsFilterGlobalDropItemMsg" INTEGER,' + sLineBreak + '	"IsFilterGlobalCenterMsg" INTEGER,' + sLineBreak + '	"IsFilterGolbalSendMsg" INTEGER,' + sLineBreak +
'	"IsFixedHero" INTEGER,' + sLineBreak + '	"IsStorageHero" INTEGER,' + sLineBreak + '	"IsStorageDeputyHero" INTEGER,' + sLineBreak + '	"HeroName" TEXT,' + sLineBreak + '	"DeputyHeroName" TEXT,' + sLineBreak + '	"DeputyHeroJob" INTEGER,' + sLineBreak + '	"Nation" INTEGER,' + sLineBreak + '	"NationCredit" INTEGER,' + sLineBreak + '	"RevivalTime" INTEGER,' + sLineBreak + '	"InfinityStorageExtCount" INTEGER,' + sLineBreak + '	"IsSaveKillMonExpRate" INTEGER,' + sLineBreak + '	"KillMonExpRate" INTEGER,' +
  sLineBreak + '	"KillMonExpRateTime" INTEGER,' + sLineBreak + '	"IsSavePowerRate" INTEGER,' + sLineBreak + '	"PowerRate" INTEGER,' + sLineBreak + '	"PowerRateTime" INTEGER,' + sLineBreak + '	"IsAttackMonSavePowerRate" INTEGER,' + sLineBreak + '	"AttackMonPowerRate" INTEGER,' + sLineBreak + '	"AttackMonPowerRateTime" INTEGER,' + sLineBreak + '	"IsSaveKillMonBurstRate" INTEGER,' + sLineBreak + '	"KillMonBurstRate" INTEGER,' + sLineBreak + '	"KillMonBurstRateTime" INTEGER,' + sLineBreak + '	"FBCreateTime" INTEGER,' + sLineBreak + '	"PayMentPoint" INTEGER,' + sLineBreak +
    //------------------------------------------------------------2018-02-06 添加新字段
  '	"MemberType" INTEGER DEFAULT 0,' + sLineBreak + '	"MemberLevel" INTEGER DEFAULT 0,' + sLineBreak +
'	"IsMaster" INTEGER,' + sLineBreak + '	"JewelryBoxStatus" INTEGER,' + sLineBreak + '	"IsShowFashion" INTEGER,' + sLineBreak + '	"IsShowGodBless" INTEGER,' + sLineBreak + '	"ActiveFengHao" INTEGER,' + sLineBreak +
    //------------------------------------------------------------2017-05-06 添加新字段
  '	"IsOpenStorage1" INTEGER,' + sLineBreak + '	"IsOpenStorage2" INTEGER,' + sLineBreak + '	"IsOpenStorage3" INTEGER,' + sLineBreak +
    //------------------------------------------------------------2019-05-12 添加新字段
  '	"ExtBagPageCount" INTEGER DEFAULT 0,' + sLineBreak + '	"ExtBagOpenItemCount" INTEGER DEFAULT 0,' + sLineBreak +
    //------------------------------------------------------------ 2019-08-02
  '	"AddMaxWeight" INTEGER DEFAULT 0,' + sLineBreak +
'	"HighLevelKillMonFixExpTimeLeft" INTEGER,' + sLineBreak +
    //------------------------------------------------------------20170425 添加新字段
  ' "MobileNumber"  TEXT(20) COLLATE NOCASE ,' + sLineBreak + ' "IsMobileBind"  INTEGER,' + sLineBreak + ' "MobileVerifyCode"  TEXT(8) COLLATE NOCASE ,' + sLineBreak + ' "MobileSendTick"  INTEGER,' + sLineBreak + ' "MobileResendCount"  INTEGER,' + sLineBreak +
'	CONSTRAINT "uk_HumanName" UNIQUE ("HumanName" ASC)' + sLineBreak + ');' + sLineBreak +
'INSERT INTO "main"."Human" (' + sLineBreak + '	"HumanID",' + sLineBreak + '	"Account",' + sLineBreak + '	"HumanName",' + sLineBreak + '	"IsDelete",' + sLineBreak + '	"IsSelect",' + sLineBreak + '	"CreateDate",' + sLineBreak + '	"LoginDate",' + sLineBreak + '	"Sex",' + sLineBreak + '	"Job",' + sLineBreak + '	"Hair",' + sLineBreak + '	"Dir",' + sLineBreak + '	"Level",' + sLineBreak + '	"ReLevel",' + sLineBreak + '	"Map",' + sLineBreak + '	"X",' + sLineBreak + '	"Y",' + sLineBreak + '	"HomeMap",' + sLineBreak
  + '	"HomeX",' + sLineBreak + '	"HomeY",' + sLineBreak + '	"AttackMode",' + sLineBreak + '	"StoragePassword",' + sLineBreak + '	"CreditPoint",' + sLineBreak + '	"Gold",' + sLineBreak + '	"GameGold",' + sLineBreak + '	"GamePoint",' + sLineBreak + '	"GameDiamond",' + sLineBreak + '	"GameGird",' + sLineBreak + '	"GameGoldEx",' + sLineBreak + '	"GameGlory",' + sLineBreak + '	"PKPoint",' + sLineBreak + '	"PayMentPoint",' + sLineBreak + '	"IsMaster",' + sLineBreak + '	"MasterName",' + sLineBreak +
  '	"MasterCount",' + sLineBreak + '	"MarryCount",' + sLineBreak + '	"DearName",' + sLineBreak + '	"IncHP",' + sLineBreak + '	"IncMP",' + sLineBreak + '	"IncHP2",' + sLineBreak + '	"FightZoneDieCount",' + sLineBreak + '	"BodyLuck",' + sLineBreak + '	"Contribution",' + sLineBreak + '	"HungerStatus",' + sLineBreak + '	"KickCount",' + sLineBreak + '	"IsLockLogin",' + sLineBreak + '	"IsAllowGroup",' + sLineBreak + '	"IsAllowGroupRecall",' + sLineBreak + '	"GroupRecallTime",' + sLineBreak + '	"IsAllowGuildReCall",' + sLineBreak + '	"IsDisableTrading",' + sLineBreak + '	"IsDisableInviteHorseRiding",' + sLineBreak + '	"IsGameGoldTrading",' + sLineBreak + '	"IsNewServer",' + sLineBreak +
    //'	"IsFilterGlobalMsg",' + sLineBreak +
  '	"IsFilterGlobalDropItemMsg",' + sLineBreak + '	"IsFilterGlobalCenterMsg",' + sLineBreak + '	"IsFilterGolbalSendMsg",' + sLineBreak +
'	"IsFixedHero",' + sLineBreak + '	"IsStorageHero",' + sLineBreak + '	"IsStorageDeputyHero",' + sLineBreak + '	"HeroName",' + sLineBreak + '	"DeputyHeroName",' + sLineBreak + '	"DeputyHeroJob",' + sLineBreak + '	"Nation",' + sLineBreak + '	"NationCredit",' + sLineBreak + '	"RevivalTime",' + sLineBreak + '	"InfinityStorageExtCount",' + sLineBreak + '	"IsSaveKillMonExpRate",' + sLineBreak + '	"KillMonExpRate",' + sLineBreak + '	"KillMonExpRateTime",' + sLineBreak + '	"IsSavePowerRate",' + sLineBreak +
  '	"PowerRate",' + sLineBreak + '	"PowerRateTime",' + sLineBreak + '	"IsAttackMonSavePowerRate",' + sLineBreak + '	"AttackMonPowerRate",' + sLineBreak + '	"AttackMonPowerRateTime",' + sLineBreak + '	"IsSaveKillMonBurstRate",' + sLineBreak + '	"KillMonBurstRate",' + sLineBreak + '	"KillMonBurstRateTime",' + sLineBreak + '	"FBCreateTime",' + sLineBreak + '	"JewelryBoxStatus",' + sLineBreak + '	"IsShowFashion",' + sLineBreak + '	"IsShowGodBless",' + sLineBreak + '	"ActiveFengHao",' + sLineBreak +
  '	"HighLevelKillMonFixExpTimeLeft" ' + sLineBreak + ') SELECT' + sLineBreak + '	"HumanID",' + sLineBreak + '	"Account",' + sLineBreak + '	"HumanName",' + sLineBreak + '	"IsDelete",' + sLineBreak + '	"IsSelect",' + sLineBreak + '	"CreateDate",' + sLineBreak + '	"LoginDate",' + sLineBreak + '	"Sex",' + sLineBreak + '	"Job",' + sLineBreak + '	"Hair",' + sLineBreak + '	"Dir",' + sLineBreak + '	"Level",' + sLineBreak + '	"ReLevel",' + sLineBreak + '	"Map",' + sLineBreak + '	"X",' + sLineBreak + '	"Y",' +
  sLineBreak + '	"HomeMap",' + sLineBreak + '	"HomeX",' + sLineBreak + '	"HomeY",' + sLineBreak + '	"AttackMode",' + sLineBreak + '	"StoragePassword",' + sLineBreak + '	"CreditPoint",' + sLineBreak + '	"Gold",' + sLineBreak + '	"GameGold",' + sLineBreak + '	"GamePoint",' + sLineBreak + '	"GameDiamond",' + sLineBreak + '	"GameGird",' + sLineBreak + '	"GameGoldEx",' + sLineBreak + '	"GameGlory",' + sLineBreak + '	"PKPoint",' + sLineBreak + '	"PayMentPoint",' + sLineBreak + '	"IsMaster",' + sLineBreak +
  '	"MasterName",' + sLineBreak + '	"MasterCount",' + sLineBreak + '	"MarryCount",' + sLineBreak + '	"DearName",' + sLineBreak + '	"IncHP",' + sLineBreak + '	"IncMP",' + sLineBreak + '	"IncHP2",' + sLineBreak + '	"FightZoneDieCount",' + sLineBreak + '	"BodyLuck",' + sLineBreak + '	"Contribution",' + sLineBreak + '	"HungerStatus",' + sLineBreak + '	"KickCount",' + sLineBreak + '	"IsLockLogin",' + sLineBreak + '	"IsAllowGroup",' + sLineBreak + '	"IsAllowGroupRecall",' + sLineBreak + '	"GroupRecallTime",' + sLineBreak + '	"IsAllowGuildReCall",' + sLineBreak + '	"IsDisableTrading",' + sLineBreak + '	"IsDisableInviteHorseRiding",' + sLineBreak + '	"IsGameGoldTrading",' + sLineBreak + '	"IsNewServer",' + sLineBreak +
    //'	"IsFilterGlobalMsg",' + sLineBreak +
  '	"IsFilterGlobalDropItemMsg",' + sLineBreak + '	"IsFilterGlobalCenterMsg",' + sLineBreak + '	"IsFilterGolbalSendMsg",' + sLineBreak +
'	"IsFixedHero",' + sLineBreak + '	"IsStorageHero",' + sLineBreak + '	"IsStorageDeputyHero",' + sLineBreak + '	"HeroName",' + sLineBreak + '	"DeputyHeroName",' + sLineBreak + '	"DeputyHeroJob",' + sLineBreak + '	"Nation",' + sLineBreak + '	"NationCredit",' + sLineBreak + '	"RevivalTime",' + sLineBreak + '	"InfinityStorageExtCount",' + sLineBreak + '	"IsSaveKillMonExpRate",' + sLineBreak + '	"KillMonExpRate",' + sLineBreak + '	"KillMonExpRateTime",' + sLineBreak + '	"IsSavePowerRate",' + sLineBreak +
  '	"PowerRate",' + sLineBreak + '	"PowerRateTime",' + sLineBreak + '	"IsSavePowerRate",' + sLineBreak + '	"PowerRate",' + sLineBreak + '	"PowerRateTime",' + sLineBreak + '	"IsSaveKillMonBurstRate",' + sLineBreak + '	"KillMonBurstRate",' + sLineBreak + '	"KillMonBurstRateTime",' + sLineBreak + '	"FBCreateTime",' + sLineBreak + '	"JewelryBoxStatus",' + sLineBreak + '	"IsShowFashion",' + sLineBreak + '	"IsShowGodBless",' + sLineBreak + '	"ActiveFengHao",' + sLineBreak + '	"HighLevelKillMonFixExpTimeLeft" ' + sLineBreak + 'FROM "_Human_old_20170420";' + sLineBreak +
'CREATE INDEX "main"."idx_Account" ON "Human" ("Account" ASC);' + sLineBreak +
'CREATE INDEX "main"."idx_HumanName" ON "Human" ("HumanName" ASC);' + sLineBreak +
'UPDATE "sqlite_sequence"' + sLineBreak + 'SET seq = (SELECT HumanID + 1 FROM human ORDER BY HumanID desc LIMIT 1)' + sLineBreak + 'WHERE name = "Human";' + sLineBreak +
'DROP TABLE _Human_old_20170420;' + sLineBreak +
'----------------------------------------------------------------------------------------------------' + sLineBreak +
'ALTER TABLE "main"."Hero" RENAME TO "_Hero_old_20170420";' + sLineBreak + 'DROP INDEX "main"."idx_HeroName";' + sLineBreak +
'CREATE TABLE "main"."Hero" (' + sLineBreak + '	"HeroID" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,' + sLineBreak + '	"HumanID" INTEGER NOT NULL,' + sLineBreak + '	"HeroName" TEXT (14) NOT NULL COLLATE NOCASE,' + sLineBreak +                                  // 忽略大小写
  '	"IsDelete" INTEGER,' + sLineBreak + '	"CreateDate" INTEGER,' + sLineBreak + '	"Sex" INTEGER,' + sLineBreak + '	"Job" INTEGER,' + sLineBreak + '	"Hair" INTEGER,' + sLineBreak + '	"Status" INTEGER,' + sLineBreak + '	"Dir" INTEGER,' + sLineBreak + '	"Level" INTEGER,' + sLineBreak + '	"ReLevel" INTEGER,' + sLineBreak + ' "LoyalPoint"  REAL, ' + sLineBreak + '	"Map" TEXT (30),' + sLineBreak + '	"X" INTEGER,' + sLineBreak + '	"Y" INTEGER,' + sLineBreak + '	"HomeMap" TEXT (30),' + sLineBreak +
  '	"HomeX" INTEGER,' + sLineBreak + '	"HomeY" INTEGER,' + sLineBreak + '	"AttackMode" INTEGER,' + sLineBreak + '	"CreditPoint" INTEGER,' + sLineBreak + '	"PKPoint" INTEGER,' + sLineBreak + '	"IncHP" INTEGER,' + sLineBreak + '	"IncMP" INTEGER,' + sLineBreak + '	"IncHP2" INTEGER,' + sLineBreak + '	"FightZoneDieCount" INTEGER,' + sLineBreak + '	"BodyLuck" REAL,' + sLineBreak + '	"HungerStatus" INTEGER,' + sLineBreak + '	"RevivalTime" INTEGER,' + sLineBreak + '	"IsSaveKillMonExpRate" INTEGER,' + sLineBreak +
  '	"KillMonExpRate" INTEGER,' + sLineBreak + '	"KillMonExpRateTime" INTEGER,' + sLineBreak + '	"IsSavePowerRate" INTEGER,' + sLineBreak + '	"PowerRate" INTEGER,' + sLineBreak + '	"PowerRateTime" INTEGER,' + sLineBreak + '	"IsAttackMonSavePowerRate" INTEGER,' + sLineBreak + '	"AttackMonPowerRate" INTEGER,' + sLineBreak + '	"AttackMonPowerRateTime" INTEGER,' + sLineBreak + '	"IsSaveKillMonBurstRate" INTEGER,' + sLineBreak + '	"KillMonBurstRate" INTEGER,' + sLineBreak + '	"KillMonBurstRateTime" INTEGER,' +
  sLineBreak + '	"JewelryBoxStatus" INTEGER,' + sLineBreak + '	"IsShowFashion" INTEGER,' + sLineBreak + '	"IsShowGodBless" INTEGER,' + sLineBreak + '	"ActiveFengHao" INTEGER,' + sLineBreak + '	CONSTRAINT "fk_HuamID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL,' + sLineBreak + '	CONSTRAINT "uk_HeroName" UNIQUE ("HeroName" ASC)' + sLineBreak + ');' + sLineBreak +
'INSERT INTO "main"."Hero" (' + sLineBreak + '	"HeroID",' + sLineBreak + '	"HumanID",' + sLineBreak + '	"HeroName",' + sLineBreak + '	"IsDelete",' + sLineBreak + '	"CreateDate",' + sLineBreak + '	"Sex",' + sLineBreak + '	"Job",' + sLineBreak + '	"Hair",' + sLineBreak + '	"Status",' + sLineBreak + '	"Dir",' + sLineBreak + '	"Level",' + sLineBreak + '	"ReLevel",' + sLineBreak + '	"Map",' + sLineBreak + '	"X",' + sLineBreak + '	"Y",' + sLineBreak + '	"HomeMap",' + sLineBreak + '	"HomeX",' + sLineBreak +
  '	"HomeY",' + sLineBreak + '	"AttackMode",' + sLineBreak + '	"CreditPoint",' + sLineBreak + '	"PKPoint",' + sLineBreak + '	"IncHP",' + sLineBreak + '	"IncMP",' + sLineBreak + '	"IncHP2",' + sLineBreak + '	"FightZoneDieCount",' + sLineBreak + '	"BodyLuck",' + sLineBreak + '	"HungerStatus",' + sLineBreak + '	"RevivalTime",' + sLineBreak + '	"IsSaveKillMonExpRate",' + sLineBreak + '	"KillMonExpRate",' + sLineBreak + '	"KillMonExpRateTime",' + sLineBreak + '	"IsSavePowerRate",' + sLineBreak + '	"PowerRate",'
  + sLineBreak + '	"PowerRateTime",' + sLineBreak + '	"IsAttackMonSavePowerRate",' + sLineBreak + '	"AttackMonPowerRate",' + sLineBreak + '	"AttackMonPowerRateTime",' + sLineBreak + '	"IsSaveKillMonBurstRate",' + sLineBreak + '	"KillMonBurstRate",' + sLineBreak + '	"KillMonBurstRateTime",' + sLineBreak + '	"JewelryBoxStatus",' + sLineBreak + '	"IsShowFashion",' + sLineBreak + '	"IsShowGodBless",' + sLineBreak + '	"ActiveFengHao"' + sLineBreak + ') SELECT' + sLineBreak + '	"HeroID",' + sLineBreak +
  '	"HumanID",' + sLineBreak + '	"HeroName",' + sLineBreak + '	"IsDelete",' + sLineBreak + '	"CreateDate",' + sLineBreak + '	"Sex",' + sLineBreak + '	"Job",' + sLineBreak + '	"Hair",' + sLineBreak + '	"Status",' + sLineBreak + '	"Dir",' + sLineBreak + '	"Level",' + sLineBreak + '	"ReLevel",' + sLineBreak + '	"Map",' + sLineBreak + '	"X",' + sLineBreak + '	"Y",' + sLineBreak + '	"HomeMap",' + sLineBreak + '	"HomeX",' + sLineBreak + '	"HomeY",' + sLineBreak + '	"AttackMode",' + sLineBreak + '	"CreditPoint",'
  + sLineBreak + '	"PKPoint",' + sLineBreak + '	"IncHP",' + sLineBreak + '	"IncMP",' + sLineBreak + '	"IncHP2",' + sLineBreak + '	"FightZoneDieCount",' + sLineBreak + '	"BodyLuck",' + sLineBreak + '	"HungerStatus",' + sLineBreak + '	"RevivalTime",' + sLineBreak + '	"IsSaveKillMonExpRate",' + sLineBreak + '	"KillMonExpRate",' + sLineBreak + '	"KillMonExpRateTime",' + sLineBreak + '	"IsSavePowerRate",' + sLineBreak + '	"PowerRate",' + sLineBreak + '	"PowerRateTime",' + sLineBreak + '	"IsSavePowerRate",' +
  sLineBreak + '	"PowerRate",' + sLineBreak + '	"PowerRateTime",' + sLineBreak + '	"IsSaveKillMonBurstRate",' + sLineBreak + '	"KillMonBurstRate",' + sLineBreak + '	"KillMonBurstRateTime",' + sLineBreak + '	"JewelryBoxStatus",' + sLineBreak + '	"IsShowFashion",' + sLineBreak + '	"IsShowGodBless",' + sLineBreak + '	"ActiveFengHao"' + sLineBreak + 'FROM "_Hero_old_20170420";' + sLineBreak +
'CREATE INDEX "main"."idx_HeroName" ON "Hero" ("HeroName" ASC);' + sLineBreak +
'UPDATE "sqlite_sequence"' + sLineBreak + 'SET seq = (SELECT HEROID + 1 FROM HERO ORDER BY HEROID desc LIMIT 1)' + sLineBreak + 'WHERE	name = "Hero";' + sLineBreak +
'DROP TABLE _Hero_old_20170420;' +
'CREATE TABLE IF NOT EXISTS "db_constant" (' + sLineBreak + '"ConstName"  TEXT(30) NOT NULL COLLATE NOCASE ,' + sLineBreak + '"ConstValue"  INTEGER,' + sLineBreak + 'PRIMARY KEY ("ConstName")' + sLineBreak + ');' + sLineBreak + 'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
  AlterHeroTable_BodyLuck_LoyalPoint = 'ALTER TABLE "main"."Hero" RENAME TO "_Hero_old_20170504";' + sLineBreak + 'DROP INDEX "main"."idx_HeroName";' + sLineBreak +
'CREATE TABLE "main"."Hero" (' + sLineBreak + '	"HeroID" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,' + sLineBreak + '	"HumanID" INTEGER NOT NULL,' + sLineBreak + '	"HeroName" TEXT (14) NOT NULL COLLATE NOCASE,' + sLineBreak +                                  // 忽略大小写
  '	"IsDelete" INTEGER,' + sLineBreak + '	"CreateDate" INTEGER,' + sLineBreak + '	"Sex" INTEGER,' + sLineBreak + '	"Job" INTEGER,' + sLineBreak + '	"Hair" INTEGER,' + sLineBreak + '	"Status" INTEGER,' + sLineBreak + '	"Dir" INTEGER,' + sLineBreak + '	"Level" INTEGER,' + sLineBreak + '	"ReLevel" INTEGER,' + sLineBreak + ' "LoyalPoint"  REAL, ' + sLineBreak +                  // 添加字段
  '	"Map" TEXT (30),' + sLineBreak + '	"X" INTEGER,' + sLineBreak + '	"Y" INTEGER,' + sLineBreak + '	"HomeMap" TEXT (30),' + sLineBreak + '	"HomeX" INTEGER,' + sLineBreak + '	"HomeY" INTEGER,' + sLineBreak + '	"AttackMode" INTEGER,' + sLineBreak + '	"CreditPoint" INTEGER,' + sLineBreak + '	"PKPoint" INTEGER,' + sLineBreak + '	"IncHP" INTEGER,' + sLineBreak + '	"IncMP" INTEGER,' + sLineBreak + '	"IncHP2" INTEGER,' + sLineBreak + '	"FightZoneDieCount" INTEGER,' + sLineBreak + '	"BodyLuck" REAL,' + sLineBreak +                      // 修改数据类型
  '	"HungerStatus" INTEGER,' + sLineBreak + '	"RevivalTime" INTEGER,' + sLineBreak + '	"IsSaveKillMonExpRate" INTEGER,' + sLineBreak + '	"KillMonExpRate" INTEGER,' + sLineBreak + '	"KillMonExpRateTime" INTEGER,' + sLineBreak + '	"IsSavePowerRate" INTEGER,' + sLineBreak + '	"PowerRate" INTEGER,' + sLineBreak + '	"PowerRateTime" INTEGER,' + sLineBreak + '	"IsSaveKillMonBurstRate" INTEGER,' + sLineBreak + '	"KillMonBurstRate" INTEGER,' + sLineBreak + '	"KillMonBurstRateTime" INTEGER,' + sLineBreak +
  '	"JewelryBoxStatus" INTEGER,' + sLineBreak + '	"IsShowFashion" INTEGER,' + sLineBreak + '	"IsShowGodBless" INTEGER,' + sLineBreak + '	"ActiveFengHao" INTEGER,' + sLineBreak + '	CONSTRAINT "fk_HuamID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL,' + sLineBreak + '	CONSTRAINT "uk_HeroName" UNIQUE ("HeroName" ASC)' + sLineBreak + ');' + sLineBreak +
'INSERT INTO "main"."Hero" (' + sLineBreak + '	"HeroID",' + sLineBreak + '	"HumanID",' + sLineBreak + '	"HeroName",' + sLineBreak + '	"IsDelete",' + sLineBreak + '	"CreateDate",' + sLineBreak + '	"Sex",' + sLineBreak + '	"Job",' + sLineBreak + '	"Hair",' + sLineBreak + '	"Status",' + sLineBreak + '	"Dir",' + sLineBreak + '	"Level",' + sLineBreak + '	"ReLevel",' + sLineBreak + '	"Map",' + sLineBreak + '	"X",' + sLineBreak + '	"Y",' + sLineBreak + '	"HomeMap",' + sLineBreak + '	"HomeX",' + sLineBreak +
  '	"HomeY",' + sLineBreak + '	"AttackMode",' + sLineBreak + '	"CreditPoint",' + sLineBreak + '	"PKPoint",' + sLineBreak + '	"IncHP",' + sLineBreak + '	"IncMP",' + sLineBreak + '	"IncHP2",' + sLineBreak + '	"FightZoneDieCount",' + sLineBreak + '	"BodyLuck",' + sLineBreak + '	"HungerStatus",' + sLineBreak + '	"RevivalTime",' + sLineBreak + '	"IsSaveKillMonExpRate",' + sLineBreak + '	"KillMonExpRate",' + sLineBreak + '	"KillMonExpRateTime",' + sLineBreak + '	"IsSavePowerRate",' + sLineBreak + '	"PowerRate",'
  + sLineBreak + '	"PowerRateTime",' + sLineBreak + '	"IsSaveKillMonBurstRate",' + sLineBreak + '	"KillMonBurstRate",' + sLineBreak + '	"KillMonBurstRateTime",' + sLineBreak + '	"JewelryBoxStatus",' + sLineBreak + '	"IsShowFashion",' + sLineBreak + '	"IsShowGodBless",' + sLineBreak + '	"ActiveFengHao"' + sLineBreak + ') SELECT' + sLineBreak + '	"HeroID",' + sLineBreak + '	"HumanID",' + sLineBreak + '	"HeroName",' + sLineBreak + '	"IsDelete",' + sLineBreak + '	"CreateDate",' + sLineBreak + '	"Sex",' +
  sLineBreak + '	"Job",' + sLineBreak + '	"Hair",' + sLineBreak + '	"Status",' + sLineBreak + '	"Dir",' + sLineBreak + '	"Level",' + sLineBreak + '	"ReLevel",' + sLineBreak + '	"Map",' + sLineBreak + '	"X",' + sLineBreak + '	"Y",' + sLineBreak + '	"HomeMap",' + sLineBreak + '	"HomeX",' + sLineBreak + '	"HomeY",' + sLineBreak + '	"AttackMode",' + sLineBreak + '	"CreditPoint",' + sLineBreak + '	"PKPoint",' + sLineBreak + '	"IncHP",' + sLineBreak + '	"IncMP",' + sLineBreak + '	"IncHP2",' + sLineBreak +
  '	"FightZoneDieCount",' + sLineBreak + '	"BodyLuck",' + sLineBreak + '	"HungerStatus",' + sLineBreak + '	"RevivalTime",' + sLineBreak + '	"IsSaveKillMonExpRate",' + sLineBreak + '	"KillMonExpRate",' + sLineBreak + '	"KillMonExpRateTime",' + sLineBreak + '	"IsSavePowerRate",' + sLineBreak + '	"PowerRate",' + sLineBreak + '	"PowerRateTime",' + sLineBreak + '	"IsSaveKillMonBurstRate",' + sLineBreak + '	"KillMonBurstRate",' + sLineBreak + '	"KillMonBurstRateTime",' + sLineBreak + '	"JewelryBoxStatus",' + sLineBreak + '	"IsShowFashion",' + sLineBreak + '	"IsShowGodBless",' + sLineBreak + '	"ActiveFengHao"' + sLineBreak + 'FROM "_Hero_old_20170504";' + sLineBreak +
'CREATE INDEX "main"."idx_HeroName" ON "Hero" ("HeroName" ASC);' + sLineBreak +
'UPDATE "sqlite_sequence"' + sLineBreak + 'SET seq = (SELECT HEROID + 1 FROM HERO ORDER BY HEROID desc LIMIT 1)' + sLineBreak + 'WHERE	name = "Hero";' + sLineBreak +
'DROP TABLE _Hero_old_20170504;' +
'CREATE TABLE IF NOT EXISTS "db_constant" (' + sLineBreak + '"ConstName"  TEXT(30) NOT NULL COLLATE NOCASE ,' + sLineBreak + '"ConstValue"  INTEGER,' + sLineBreak + 'PRIMARY KEY ("ConstName")' + sLineBreak + ');' + sLineBreak +
'ALTER TABLE Human ADD COLUMN "IsOpenStorage1" INTEGER;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "IsOpenStorage2" INTEGER;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "IsOpenStorage3" INTEGER;' + sLineBreak +
'ALTER TABLE Human ADD COLUMN "MemberType" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "MemberLevel" INTEGER DEFAULT 0;' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';

{ TSqliteHumanDB }

procedure TSqliteHumanDB.DoInit;
var
  Stms: TSQLStatements;
begin
  Assert(Owner is TSqliteRoleDB, 'TSqliteHumanDB owner type error.');
  Assert(TSqliteRoleDB(Owner).FDB <> nil, 'TSqliteRoleDB.DB not create');
  Stms := TSqliteRoleDB(Owner).FDB.Statements;

  FStatementGetID := Stms.AddSQLStatement('HumanGetID');
  FStatementGetID.Sql := 'select HumanID from Human where HumanName = ?;';                     // COLLATE NOCASE

  FStatementCheckHumanExists := Stms.AddSQLStatement('CheckHumanExists');
  FStatementCheckHumanExists.Sql := 'select HumanID from Human where Account = ? and HumanName = ?;';                     // COLLATE NOCASE

  FStatementGetHumanCount := Stms.AddSQLStatement('HumanGetCount');
  FStatementGetHumanCount.Sql := 'select count(*) from Human where (Account = ?) and (IsDelete = 0);';

  FStatementGetOtherHumanName := Stms.AddSQLStatement('GetOtherHumanName');
  FStatementGetOtherHumanName.Sql := 'select HumanName from Human where (Account = ?) and (HumanName <> ?) and (IsDelete = 0);';

  FStatementGetHumanHeroName := Stms.AddSQLStatement('HumanGetHumanHeroName');
  FStatementGetHumanHeroName.Sql := 'select HeroName, DeputyHeroName from Human where (HumanName = ?);';  // COLLATE NOCASE

  FStatementGetHumanHeroID := Stms.AddSQLStatement('HumanGetHumanHeroID');
  FStatementGetHumanHeroID.Sql := 'select HeroID from Hero where (HumanID = ?);';

  FStatementGetHumanGoldInfo := Stms.AddSQLStatement('HumanGetHumanGoldInfo');
  FStatementGetHumanGoldInfo.Sql := 'Select Gold, GameGold, GamePoint, GameDiamond, GameGird from Human where (HumanName = ?);';

  FStatementUpdateHumanGoldInfo := Stms.AddSQLStatement('HumanUpdateHumanGoldInfo');
  FStatementUpdateHumanGoldInfo.Sql := 'update Human set ' + 'Gold = ?,' + 'GameGold = ?,' + 'GamePoint = ?,' + 'GameDiamond = ?, ' + 'GameGird = ? ' + 'where (HumanName = ?);';

  FStatementGetHumanBaseInfo := Stms.AddSQLStatement('HumanGetBaseInfo');
  FStatementGetHumanBaseInfo.Sql := 'select ' + 'Sex, ' + 'Job, ' + 'Level, ' + 'LoginDate ' + 'from Human ' + 'where HumanName = ?;';

  FStatementQueryHumans := Stms.AddSQLStatement('HumanQuery');
  FStatementQueryHumans.Sql := 'select a.HumanName, ' + 'a.IsSelect, ' + 'a.Sex, ' + 'a.Job, ' + 'a.Hair, ' + 'a.Level ' + 'from Human a ' + 'where (Account = ?) and (IsDelete = ?);';

  FStatementSearchByAccountMatchComplete := Stms.AddSQLStatement('HumanSearchByAccount1');
  FStatementSearchByAccountMatchComplete.Sql := 'select ' + 'a.Account, ' + 'a.HumanName, ' + 'a.IsDelete, ' + 'a.Sex, ' + 'a.Job, ' + 'a.Level ' + 'from Human a ' + 'where (Account = ?);';

  FStatementSearchByAccountMatchFuzzy := Stms.AddSQLStatement('HumanSearchByAccount2');
  FStatementSearchByAccountMatchFuzzy.Sql := 'select ' + 'a.Account, ' + 'a.HumanName, ' + 'a.IsDelete, ' + 'a.Sex, ' + 'a.Job, ' + 'a.Level ' + 'from Human a ' + 'where (Account like ?);';

  FStatementSearchByNameMatchComplete := Stms.AddSQLStatement('HumanSearchByName1');
  FStatementSearchByNameMatchComplete.Sql := 'select ' + 'a.Account, ' + 'a.HumanName, ' + 'a.IsDelete, ' + 'a.Sex, ' + 'a.Job, ' + 'a.Level ' + 'from Human a ' + 'where (HumanName = ?);';

  FStatementSearchByNameMatchFuzzy := Stms.AddSQLStatement('HumanSearchByName2');
  FStatementSearchByNameMatchFuzzy.Sql := 'select ' + 'a.Account, ' + 'a.HumanName, ' + 'a.IsDelete, ' + 'a.Sex, ' + 'a.Job, ' + 'a.Level ' + 'from Human a ' + 'where (HumanName like ?);';

  FStatementSearchByLevel := Stms.AddSQLStatement('HumanSearchByLevel');
  FStatementSearchByLevel.Sql := 'select ' + 'a.Account, ' + 'a.HumanName, ' + 'a.IsDelete, ' + 'a.Sex, ' + 'a.Job, ' + 'a.Level ' + 'from Human a ' + 'where (a.Level >= ?) LIMIT ?;';

  FStatementGetMobileNumbers1 := Stms.AddSQLStatement('HumanGetMobileNumbers1');
  FStatementGetMobileNumbers1.Sql := 'SELECT DISTINCT MobileNumber FROM  human WHERE Length(MobileNumber) > 0;';

  FStatementGetMobileNumbers2 := Stms.AddSQLStatement('HumanGetMobileNumbers2');
  FStatementGetMobileNumbers2.Sql := 'select DISTINCT MobileNumber from human where (Length(MobileNumber) > 0) and (IsMobileBind = 1);';

  FStatementSelect := Stms.AddSQLStatement('HumanSetSelect');
  FStatementSelect.Sql := 'update Human set IsSelect = 1 where (Account = ?) and (HumanName = ?);';

  FStatementUnSelect := Stms.AddSQLStatement('HumanSetUnSelect');
  FStatementUnSelect.Sql := 'update Human set IsSelect = 0 where (Account = ?) and (HumanName <> ?);';

  FStatementGetHuman := Stms.AddSQLStatement('HumanSelect');
  FStatementGetHuman.Sql := 'select ' + 'HumanID,' + 'Account,' + 'HumanName,' + 'Sex,' + 'Job,' + 'Hair,' + 'Dir,' + 'Level,' + 'ReLevel,' + 'Map,' + 'X,' + 'Y,' + 'HomeMap,' + 'HomeX,' + 'HomeY,' + 'AttackMode,' + 'StoragePassword,' + 'CreditPoint,' + 'Gold,' + 'GameGold,' + 'GamePoint,' + 'GameDiamond,' + 'GameGird,' + 'GameGoldEx,' + 'GameGlory,' + 'PKPoint,' + 'PayMentPoint,' + 'MemberType,' + 'MemberLevel,' + 'IsMaster,' + 'MasterName,' + 'MasterCount,' + 'MarryCount,' + 'DearName,' + 'IncHP,' +
    'IncMP,' + 'IncHP2,' + 'FightZoneDieCount,' + 'BodyLuck,' + 'Contribution,' + 'HungerStatus,' + 'KickCount,' + 'IsLockLogin,' + 'IsAllowGroup,' + 'IsAllowGroupRecall,' + 'GroupRecallTime,' + 'IsAllowGuildReCall,' + 'IsDisableTrading,' + 'IsDisableInviteHorseRiding,' + 'IsGameGoldTrading,' + 'IsNewServer,' +
      //'IsFilterGlobalMsg,' +
    'IsFilterGlobalDropItemMsg,' +              // 过滤掉落提示信息 chongchong 2017-04-16
    'IsFilterGlobalCenterMsg,' +                // 过滤SendCenterMsg chongchong 2017-04-16
    'IsFilterGolbalSendMsg,' +                  // 过滤SendMsg全局信息 chongchong 2017-04-16
    'IsFixedHero,' + 'IsStorageHero,' + 'IsStorageDeputyHero,' + 'HeroName,' + 'DeputyHeroName,' + 'DeputyHeroJob,' + 'Nation,' + 'NationCredit,' + 'RevivalTime,' + 'InfinityStorageExtCount,' + 'IsSaveKillMonExpRate,' + 'KillMonExpRate,' + 'KillMonExpRateTime,' + 'IsSavePowerRate,' + 'PowerRate,' + 'PowerRateTime,' + 'IsAttackMonSavePowerRate,' + 'AttackMonPowerRate,' + 'AttackMonPowerRateTime,' + 'IsSaveKillMonBurstRate,' + 'KillMonBurstRate,' + 'KillMonBurstRateTime,' + 'FBCreateTime,' +
    'JewelryBoxStatus,' + 'IsShowFashion,' + 'IsShowGodBless,' + 'ActiveFengHao,' + 'IsOpenStorage1,' + 'IsOpenStorage2,' + 'IsOpenStorage3,' + 'ExtBagPageCount,' + 'ExtBagOpenItemCount,' + 'AddMaxWeight,' + 'HighLevelKillMonFixExpTimeLeft,' + 'MobileNumber,' + 'IsMobileBind,' + 'MobileVerifyCode,' + 'MobileSendTick,' + 'MobileResendCount, ' + 'ClearDayVarTime ' + 'from Human ' + 'where (Account = ?) and (HumanName = ?);';

  FStatementGetAbil := Stms.AddSQLStatement('HumanSelectAbil');
  FStatementGetAbil.Sql := 'select ' + 'AC1,' + 'AC2,' + 'MAC1,' + 'MAC2,' + 'DC1,' + 'DC2,' + 'MC1,' + 'MC2,' + 'SC1,' + 'SC2,' + 'HP,' + 'MaxHP,' + 'MP,' + 'MaxMP,' + 'Exp,' + 'MaxExp,' + 'Weight,' + 'MaxWeight,' + 'WearWeight,' + 'MaxWearWeight,' + 'HandWeight,' + 'MaxHandWeight,' + 'AdjustAbilPoint,' + 'AdjustAbilDC,' + 'AdjustAbilMC,' + 'AdjustAbilSC,' + 'AdjustAbilAC,' + 'AdjustAbilMAC,' + 'AdjustAbilHP,' + 'AdjustAbilMP,' + 'AdjustAbilHit,' + 'AdjustAbilSpeed,' + 'AdjustAbilMaxRate ' + 'from HumanAbil ' + 'where HumanID = ?;';

  FStatementGetAbilNG := Stms.AddSQLStatement('HumanSelectAbilNG');
  FStatementGetAbilNG.Sql := 'select ' + 'IsTrainingNG,' + 'IsTrainingXF,' + 'AbilNGLevel,' + 'AbilNGValue,' + 'AbilNGMaxValue,' + 'AbilNGExp,' + 'AbilNGMaxExp,' + 'ContinuousMagicOrder1,' + 'ContinuousMagicOrder2,' + 'ContinuousMagicOrder3,' + 'IsOpenLastContinuous,' + 'LastContinuousMagicOrder,' + 'Meridians1Level,' + 'Meridians1BlastHitRate1,' + 'Meridians1Acupoints1,' + 'Meridians1Acupoints2,' + 'Meridians1Acupoints3,' + 'Meridians1Acupoints4,' + 'Meridians1Acupoints5,' + 'Meridians2Level,' +
    'Meridians2BlastHitRate,' + 'Meridians2Acupoints1,' + 'Meridians2Acupoints2,' + 'Meridians2Acupoints3,' + 'Meridians2Acupoints4,' + 'Meridians2Acupoints5,' + 'Meridians3Level,' + 'Meridians3BlastHitRate,' + 'Meridians3Acupoints1,' + 'Meridians3Acupoints2,' + 'Meridians3Acupoints3,' + 'Meridians3Acupoints4,' + 'Meridians3Acupoints5,' + 'Meridians4Level,' + 'Meridians4BlastHitRate,' + 'Meridians4Acupoints1,' + 'Meridians4Acupoints2,' + 'Meridians4Acupoints3,' + 'Meridians4Acupoints4,' + 'Meridians4Acupoints5,' + 'Meridians5Level,' + 'Meridians5BlastHitRate,' + 'Meridians5Acupoints1,' + 'Meridians5Acupoints2,' + 'Meridians5Acupoints3,' + 'Meridians5Acupoints4,' + 'Meridians5Acupoints5 ' + 'from HumanAbilNG ' + 'where HumanID = ?;';

  FStatementGetAbilWine := Stms.AddSQLStatement('HumanSelectAbilWine');
  FStatementGetAbilWine.Sql := 'select ' + 'IsDrinkedWine,' + 'IsDrinkWineDrunk,' + 'DrinkWineQuality,' + 'DrinkWineAlcohol,' + 'AbilAlcohol,' + 'AbilMaxAlcohol,' + 'AbilDrinkValue,' + 'AbilMedicineLevel,' + 'AbilMedicineValue,' + 'AbilMaxMedicineValue ' + 'from HumanAbilWine ' + 'where HumanID = ?;';

  FStatementGetAbilNpcAdd := Stms.AddSQLStatement('HumanSelectAbilNpcAdd');
  FStatementGetAbilNpcAdd.Sql := 'select ' + '"Index",' + 'Value ' + 'from HumanAbilNpcAdd ' + 'where HumanID = ?;';

  FStatementGetGamePetData := Stms.AddSQLStatement('HumanSelectGamePetData');
  FStatementGetGamePetData.Sql := 'select ' + '"Index",' + 'Name,' + 'Level,' + 'HP,' + 'MP,' + 'Exp,' + 'Magic1,' + 'Magic2,' + 'Magic3,' + 'Magic4,' + 'Magic5,' + 'Magic6,' + 'Magic7,' + 'Magic8 ' + 'from HumanGamePetData ' + 'where HumanID = ?;';

  FStatementGetGodBlessState := Stms.AddSQLStatement('HumanSelectGodBlessState');
  FStatementGetGodBlessState.Sql := 'select ' + '"Index",' + 'Value ' + 'from HumanGodBlessState ' + 'where HumanID = ?;';

  FStatementGetMagic := Stms.AddSQLStatement('HumanSelectMagic');
  FStatementGetMagic.Sql := 'select ' + 'MagicType,' + 'MagicIndex,' + 'MagicID,' + 'MagicAttr,' + 'MagicLevel,' + 'MagicNewLevel,' + 'MagicKey,' + 'MagicTranPoint,' + 'MagicIsUseItemAdd ' + 'from HumanMagic ' + 'where HumanID = ?;';

  FStatementGetMagicUseTick := Stms.AddSQLStatement('HumanSelectMagicUseTick');
  FStatementGetMagicUseTick.Sql := 'select ' + 'MagicID,' + 'Value ' + 'from HumanMagicUseTick ' + 'where HumanID = ?;';

  FStatementGetStatusTime := Stms.AddSQLStatement('HumanSelectStatusTime');
  FStatementGetStatusTime.Sql := 'select ' + '"Index",' + 'Value ' + 'from HumanStatusTime ' + 'where HumanID = ?;';

  FStatementGetQuestFlag := Stms.AddSQLStatement('HumanSelectQuestFlag');
  FStatementGetQuestFlag.Sql := 'select ' + '"Index",' + 'Value ' + 'from HumanQuestFlag ' + 'where HumanID = ?;';

  FStatementGetVariableU := Stms.AddSQLStatement('HumanSelectVariableU');
  FStatementGetVariableU.Sql := 'select ' + '"Index",' + 'Value ' + 'from HumanVariableU ' + 'where HumanID = ?;';

  FStatementGetVariableT := Stms.AddSQLStatement('HumanSelectVariableT');
  FStatementGetVariableT.Sql := 'select ' + '"Index",' + 'Value ' + 'from HumanVariableT ' + 'where HumanID = ?;';

  FStatementGetVariableJ := Stms.AddSQLStatement('HumanSelectVariableJ');
  FStatementGetVariableJ.Sql := 'select ' + '"Index",' + 'Value ' + 'from HumanVariableJ ' + 'where HumanID = ?;';

  FStatementGetVariableZ := Stms.AddSQLStatement('HumanSelectVariableZ');
  FStatementGetVariableZ.Sql := 'select ' + '"Index",' + 'Value ' + 'from HumanVariableZ ' + 'where HumanID = ?;';

  FStatementGetItems := Stms.AddSQLStatement('HumanSelectitems');
  FStatementGetItems.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'MakeIndex,' + 'DBIndex,' + 'Name,' + 'Dura,' + 'DuraMax,' + 'HeroM2DressEffect,' + 'UpgradeCount,' + 'IsStartTime,' + 'LimitTime,' + 'HeroM2Light,' + 'Color,' + 'IsBind,' + 'BindOption,' + 'Effect,' + 'NewLooks,' + 'NewShape,' + 'FluteCount,' + 'PropertyText,' + 'PropertyTextColor,' + 'ItemFrom,' + 'ItemFromMap,' + 'ItemFromMon,' + 'ItemFromMaker,' + 'ItemFromDate,' + 'InsuranceCount, ' + 'NewExpand3, ' + 'NewExpand4 ' + 'from HumanItems ' + 'where HumanID = ?;';

  FStatementGetItemValueAdd := Stms.AddSQLStatement('HumanSelectitemValueAdd');
  FStatementGetItemValueAdd.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Value ' + 'from HumanItemValueAdd ' + 'where HumanID = ?;';

  FStatementGetItemElementAdd := Stms.AddSQLStatement('HumanSelectitemElementAdd');
  FStatementGetItemElementAdd.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Value ' + 'from HumanItemElementAdd ' + 'where HumanID = ?;';

  FStatementGetItemAddDataByte := Stms.AddSQLStatement('HumanSelectitemAddDataByte');
  FStatementGetItemAddDataByte.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Value ' + 'from HumanItemAddDataByte ' + 'where HumanID = ?;';

  FStatementGetItemAddDataInt := Stms.AddSQLStatement('HumanSelectitemAddDataInt');
  FStatementGetItemAddDataInt.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Value ' + 'from HumanItemAddDataInt ' + 'where HumanID = ?;';

  FStatementGetItemAddDataText := Stms.AddSQLStatement('HumanSelectitemAddDataText');
  FStatementGetItemAddDataText.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Value ' + 'from HumanItemAddDataText ' + 'where HumanID = ?;';

  FStatementGetItemFlute := Stms.AddSQLStatement('HumanSelectitemFlute');
  FStatementGetItemFlute.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Value, ' + 'OverlapCount ' + 'from HumanItemFlute ' + 'where HumanID = ?;';

  FStatementGetItemProgress := Stms.AddSQLStatement('HumanSelectItemProgress');
  FStatementGetItemProgress.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'IsOpen,' + 'NameColor,' + 'Count,' + 'ShowType,' + 'Max,' + 'Value,' + 'Level,' + 'Name ' + 'from HumanItemProgress ' + 'where HumanID = ?;';

  FStatementGetItemProperty := Stms.AddSQLStatement('HumanSelectItemProperty');
  FStatementGetItemProperty.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Color,' + 'BindType,' + 'ShowFlag,' + 'IsPercent,' + 'HintModule,' + 'Value, ' + 'Value2, ' + 'Value3 ' + 'from HumanItemProperty ' + 'where HumanID = ?;';

  FStatementGetSkillPower := Stms.AddSQLStatement('HumanSelectSkillPower');
  FStatementGetSkillPower.Sql := 'select ' + 'SkillID,' + 'HumanAttackPercent,' + 'HumanAttackValue,' + 'MonAttackPercent,' + 'MonAttackValue,' + 'DefensePercent,' + 'DefenseValue,' + 'RemainingTime  ' + 'from HumanSkillPower ' + 'where HumanID = ?;';

  FStatementAddHuman := Stms.AddSQLStatement('HumanAdd');
  FStatementAddHuman.Sql := 'insert into Human(' + 'Account, ' + 'HumanName, ' + 'IsDelete, ' + 'IsSelect, ' + 'CreateDate, ' + 'Sex, ' + 'Job, ' + 'Hair) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementDeleteOrRestore := Stms.AddSQLStatement('HumanDelOrRestore');
  FStatementDeleteOrRestore.Sql := 'update Human set IsDelete = ? where (Account = ?) and (HumanName = ?);';

  FStatementRecordLoginTime := Stms.AddSQLStatement('HumanRecordLoginTime');
  FStatementRecordLoginTime.Sql := 'update Human set LoginDate = ? where (Account = ?) and (HumanName = ?);';

  FStatementUpdateHumanName := Stms.AddSQLStatement('HumanRename');
  FStatementUpdateHumanName.Sql := 'update Human set HumanName = ? where HumanID = ?;';

  FStatementUpdateHumanNameDearName := Stms.AddSQLStatement('HumanRenameSyncDearName');
  FStatementUpdateHumanNameDearName.Sql := 'update Human set DearName = ? where DearName = ?;';

  FStatementUpdateHumanNameMasterName := Stms.AddSQLStatement('HumanRenameSyncMasterName');
  FStatementUpdateHumanNameMasterName.Sql := 'update Human set MasterName = ? where MasterName = ?;';

  FStatementUpdateHuman := Stms.AddSQLStatement('HumanUpdate');
  FStatementUpdateHuman.Sql := 'update Human set ' + 'Sex = ?,' + 'Job = ?,' + 'Hair = ?,' + 'Dir = ?,' + 'Level = ?,' + 'ReLevel = ?,' + 'Map = ?,' + 'X = ?,' + 'Y = ?,' + 'HomeMap = ?,' + 'HomeX = ?,' + 'HomeY = ?,' + 'AttackMode = ?,' + 'StoragePassword = ?,' + 'CreditPoint = ?,' + 'Gold = ?,' + 'GameGold = ?,' + 'GamePoint = ?,' + 'GameDiamond = ?,' + 'GameGird = ?,' + 'GameGoldEx = ?,' + 'GameGlory = ?,' + 'PKPoint = ?,' + 'PayMentPoint = ?,' + 'MemberType = ?,' + 'MemberLevel = ?,' + 'IsMaster = ?,' +
    'MasterName = ?,' + 'MasterCount = ?,' + 'MarryCount = ?,' + 'DearName = ?,' + 'IncHP = ?,' + 'IncMP = ?,' + 'IncHP2 = ?,' + 'FightZoneDieCount = ?,' + 'BodyLuck = ?,' + 'Contribution = ?,' + 'HungerStatus = ?,' + 'KickCount = ?,' + 'IsLockLogin = ?,' + 'IsAllowGroup = ?,' + 'IsAllowGroupRecall = ?,' + 'GroupRecallTime = ?,' + 'IsAllowGuildReCall = ?,' + 'IsDisableTrading = ?,' + 'IsDisableInviteHorseRiding = ?,' + 'IsGameGoldTrading = ?,' + 'IsNewServer = ?,' +

      //'IsFilterGlobalMsg = ?,' +
    'IsFilterGlobalDropItemMsg = ?,' +          // 过滤掉落提示信息 chongchong 2017-04-16
    'IsFilterGlobalCenterMsg = ?,' +            // 过滤SendCenterMsg chongchong 2017-04-16
    'IsFilterGolbalSendMsg = ?,' +              // 过滤SendMsg全局信息 chongchong 2017-04-16
      //'IsFixedHero = ?,' +
    'IsStorageHero = ?,' + 'IsStorageDeputyHero = ?,' +      //'HeroName = ?,' +
      //'DeputyHeroName = ?,' +
    'DeputyHeroJob = ?,' + 'Nation = ?,' + 'NationCredit = ?,' + 'RevivalTime = ?,' + 'InfinityStorageExtCount = ?,' + 'IsSaveKillMonExpRate = ?,' + 'KillMonExpRate = ?,' + 'KillMonExpRateTime = ?,' + 'IsSavePowerRate = ?,' + 'PowerRate = ?,' + 'PowerRateTime = ?,' + 'IsAttackMonSavePowerRate = ?,' + 'AttackMonPowerRate = ?,' + 'AttackMonPowerRateTime = ?,' + 'IsSaveKillMonBurstRate = ?,' + 'KillMonBurstRate = ?,' + 'KillMonBurstRateTime = ?,' + 'FBCreateTime = ?,' + 'JewelryBoxStatus = ?,' +
    'IsShowFashion = ?,' + 'IsShowGodBless = ?,' + 'ActiveFengHao = ?,' + 'IsOpenStorage1 = ?,' + 'IsOpenStorage2 = ?,' + 'IsOpenStorage3 = ?,' + 'ExtBagPageCount = ?,' + 'ExtBagOpenItemCount = ?,' + 'AddMaxWeight = ?,' + 'HighLevelKillMonFixExpTimeLeft = ?, ' + 'MobileNumber = ?, ' + 'IsMobileBind = ?, ' + 'MobileVerifyCode = ?, ' + 'MobileSendTick = ?, ' + 'MobileResendCount = ?, ' + 'ClearDayVarTime = ? ' + 'where HumanID = ?;';

  FStatementInsertAbil := Stms.AddSQLStatement('HumanInsertAbil');
  FStatementInsertAbil.Sql := 'insert into HumanAbil(' + 'HumanID, ' + 'AC1, ' + 'AC2, ' + 'MAC1, ' + 'MAC2, ' + 'DC1, ' + 'DC2, ' + 'MC1, ' + 'MC2, ' + 'SC1, ' + 'SC2, ' + 'HP, ' + 'MaxHP,' + 'MP, ' + 'MaxMP, ' + 'Exp, ' + 'MaxExp, ' + 'Weight, ' + 'MaxWeight, ' + 'WearWeight, ' + 'MaxWearWeight, ' + 'HandWeight, ' + 'MaxHandWeight,' + 'AdjustAbilPoint, ' + 'AdjustAbilDC, ' + 'AdjustAbilMC, ' + 'AdjustAbilSC, ' + 'AdjustAbilAC, ' + 'AdjustAbilMAC, ' + 'AdjustAbilHP,' + 'AdjustAbilMP, ' + 'AdjustAbilHit, ' + 'AdjustAbilSpeed, ' + 'AdjustAbilMaxRate) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertAbilNG := Stms.AddSQLStatement('HumanInsertAbilNG');
  FStatementInsertAbilNG.Sql := 'insert into HumanAbilNG(' + 'HumanID, ' + 'IsTrainingNG, ' + 'IsTrainingXF, ' + 'AbilNGLevel, ' + 'AbilNGValue, ' + 'AbilNGMaxValue, ' + 'AbilNGExp, ' + 'AbilNGMaxExp,' + 'ContinuousMagicOrder1, ' + 'ContinuousMagicOrder2, ' + 'ContinuousMagicOrder3, ' + 'IsOpenLastContinuous, ' + 'LastContinuousMagicOrder, ' + 'Meridians1Level, ' + 'Meridians1BlastHitRate1, ' + 'Meridians1Acupoints1, ' + 'Meridians1Acupoints2, ' + 'Meridians1Acupoints3, ' + 'Meridians1Acupoints4, ' +
    'Meridians1Acupoints5, ' + 'Meridians2Level, ' + 'Meridians2BlastHitRate, ' + 'Meridians2Acupoints1, ' + 'Meridians2Acupoints2, ' + 'Meridians2Acupoints3, ' + 'Meridians2Acupoints4, ' + 'Meridians2Acupoints5, ' + 'Meridians3Level, ' + 'Meridians3BlastHitRate, ' + 'Meridians3Acupoints1, ' + 'Meridians3Acupoints2, ' + 'Meridians3Acupoints3, ' + 'Meridians3Acupoints4, ' + 'Meridians3Acupoints5, ' + 'Meridians4Level, ' + 'Meridians4BlastHitRate, ' + 'Meridians4Acupoints1, ' + 'Meridians4Acupoints2, ' +
    'Meridians4Acupoints3, ' + 'Meridians4Acupoints4, ' + 'Meridians4Acupoints5, ' + 'Meridians5Level, ' + 'Meridians5BlastHitRate, ' + 'Meridians5Acupoints1, ' + 'Meridians5Acupoints2, ' + 'Meridians5Acupoints3, ' + 'Meridians5Acupoints4, ' + 'Meridians5Acupoints5) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ' + '?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertAbilWine := Stms.AddSQLStatement('HumanInsertAbilWine');
  FStatementInsertAbilWine.Sql := 'insert into HumanAbilWine(' + 'HumanID, ' + 'IsDrinkedWine, ' + 'IsDrinkWineDrunk, ' + 'DrinkWineQuality, ' + 'DrinkWineAlcohol, ' + 'AbilAlcohol, ' + 'AbilMaxAlcohol, ' + 'AbilDrinkValue, ' + 'AbilMedicineLevel, ' + 'AbilMedicineValue, ' + 'AbilMaxMedicineValue) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementUpdateAbil := Stms.AddSQLStatement('HumanUpdateAbil');
  FStatementUpdateAbil.Sql := 'update HumanAbil set ' + 'AC1 = ?,' + 'AC2 = ?,' + 'MAC1 = ?,' + 'MAC2 = ?,' + 'DC1 = ?,' + 'DC2 = ?,' + 'MC1 = ?,' + 'MC2 = ?,' + 'SC1 = ?,' + 'SC2 = ?,' + 'HP = ?,' + 'MaxHP = ?,' + 'MP = ?,' + 'MaxMP = ?,' + 'Exp = ?,' + 'MaxExp = ?,' + 'Weight = ?,' + 'MaxWeight = ?,' + 'WearWeight = ?,' + 'MaxWearWeight = ?,' + 'HandWeight = ?,' + 'MaxHandWeight = ?,' + 'AdjustAbilPoint = ?,' + 'AdjustAbilDC = ?,' + 'AdjustAbilMC = ?,' + 'AdjustAbilSC = ?,' + 'AdjustAbilAC = ?,' + 'AdjustAbilMAC = ?,' + 'AdjustAbilHP = ?,' + 'AdjustAbilMP = ?,' + 'AdjustAbilHit = ?,' + 'AdjustAbilSpeed = ?,' + 'AdjustAbilMaxRate = ? ' + 'where HumanID = ?;';

  FStatementUpdateAbilNG := Stms.AddSQLStatement('HumanUpdateAbilNG');
  FStatementUpdateAbilNG.Sql := 'update HumanAbilNG set ' + 'IsTrainingNG = ?,' + 'IsTrainingXF = ?,' + 'AbilNGLevel = ?,' + 'AbilNGValue = ?,' + 'AbilNGMaxValue = ?,' + 'AbilNGExp = ?,' + 'AbilNGMaxExp = ?,' + 'ContinuousMagicOrder1 = ?,' + 'ContinuousMagicOrder2 = ?,' + 'ContinuousMagicOrder3 = ?,' + 'IsOpenLastContinuous = ?,' + 'LastContinuousMagicOrder = ?,' + 'Meridians1Level = ?,' + 'Meridians1BlastHitRate1 = ?,' + 'Meridians1Acupoints1 = ?,' + 'Meridians1Acupoints2 = ?,' +
    'Meridians1Acupoints3 = ?,' + 'Meridians1Acupoints4 = ?,' + 'Meridians1Acupoints5 = ?,' + 'Meridians2Level = ?,' + 'Meridians2BlastHitRate = ?,' + 'Meridians2Acupoints1 = ?,' + 'Meridians2Acupoints2 = ?,' + 'Meridians2Acupoints3 = ?,' + 'Meridians2Acupoints4 = ?,' + 'Meridians2Acupoints5 = ?,' + 'Meridians3Level = ?,' + 'Meridians3BlastHitRate = ?,' + 'Meridians3Acupoints1 = ?,' + 'Meridians3Acupoints2 = ?,' + 'Meridians3Acupoints3 = ?,' + 'Meridians3Acupoints4 = ?,' + 'Meridians3Acupoints5 = ?,' +
    'Meridians4Level = ?,' + 'Meridians4BlastHitRate = ?,' + 'Meridians4Acupoints1 = ?,' + 'Meridians4Acupoints2 = ?,' + 'Meridians4Acupoints3 = ?,' + 'Meridians4Acupoints4 = ?,' + 'Meridians4Acupoints5 = ?,' + 'Meridians5Level = ?,' + 'Meridians5BlastHitRate = ?,' + 'Meridians5Acupoints1 = ?,' + 'Meridians5Acupoints2 = ?,' + 'Meridians5Acupoints3 = ?,' + 'Meridians5Acupoints4 = ?,' + 'Meridians5Acupoints5 = ? ' + 'where HumanID = ?;';

  FStatementUpdateAbilWine := Stms.AddSQLStatement('HumanUpdateAbilWine');
  FStatementUpdateAbilWine.Sql := 'update HumanAbilWine set ' + 'IsDrinkedWine = ?,' + 'IsDrinkWineDrunk = ?,' + 'DrinkWineQuality = ?,' + 'DrinkWineAlcohol = ?,' + 'AbilAlcohol = ?,' + 'AbilMaxAlcohol = ?,' + 'AbilDrinkValue = ?,' + 'AbilMedicineLevel = ?,' + 'AbilMedicineValue = ?,' + 'AbilMaxMedicineValue = ? ' + 'where HumanID = ?;';

  FStatementInsertAbilNpcAdd := Stms.AddSQLStatement('HumanInsertAbilNpcAdd');
  FStatementInsertAbilNpcAdd.Sql := 'insert into HumanAbilNpcAdd(' + 'HumanID, ' + '"Index", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertGamePetData := Stms.AddSQLStatement('HumanInsertGamePetData');
  FStatementInsertGamePetData.Sql := 'insert into HumanGamePetData(' + 'HumanID, ' + '"Index", ' + 'Name, ' + 'Level, ' + 'HP, ' + 'MP, ' + 'Exp, ' + 'Magic1, ' + 'Magic2, ' + 'Magic3, ' + 'Magic4, ' + 'Magic5, ' + 'Magic6, ' + 'Magic7, ' + 'Magic8) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertGodBlessState := Stms.AddSQLStatement('HumanInsertGodBlessState');
  FStatementInsertGodBlessState.Sql := 'insert into HumanGodBlessState(' + 'HumanID, ' + '"Index", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertMagic := Stms.AddSQLStatement('HumanInsertMagic');
  FStatementInsertMagic.Sql := 'insert into HumanMagic(' + 'HumanID, ' + 'MagicType, ' + 'MagicIndex, ' + 'MagicID,' + 'MagicAttr, ' + 'MagicLevel, ' + 'MagicNewLevel, ' + 'MagicKey, ' + 'MagicTranPoint, ' + 'MagicIsUseItemAdd) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertMagicUseTick := Stms.AddSQLStatement('HumanInsertMagicUseTick');
  FStatementInsertMagicUseTick.Sql := 'insert into HumanMagicUseTick(' + 'HumanID, ' + 'MagicID, ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertStatusTime := Stms.AddSQLStatement('HumanInsertStatusTime');
  FStatementInsertStatusTime.Sql := 'insert into HumanStatusTime(' + 'HumanID, ' + '"Index", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertQuestFlag := Stms.AddSQLStatement('HumanInsertQuestFlag');
  FStatementInsertQuestFlag.Sql := 'insert into HumanQuestFlag(' + 'HumanID, ' + '"Index", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertVariableU := Stms.AddSQLStatement('HumanInsertVariableU');
  FStatementInsertVariableU.Sql := 'insert into HumanVariableU(' + 'HumanID, ' + '"Index", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertVariableT := Stms.AddSQLStatement('HumanInsertVariableT');
  FStatementInsertVariableT.Sql := 'insert into HumanVariableT(' + 'HumanID, ' + '"Index", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertVariableJ := Stms.AddSQLStatement('HumanInsertVariableJ');
  FStatementInsertVariableJ.Sql := 'insert into HumanVariableJ(' + 'HumanID, ' + '"Index", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertVariableZ := Stms.AddSQLStatement('HumanInsertVariableZ');
  FStatementInsertVariableZ.Sql := 'insert into HumanVariableZ(' + 'HumanID, ' + '"Index", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertCustomMoney := Stms.AddSQLStatement('HumanInsertCustomMoney');
  FStatementInsertCustomMoney.Sql := 'insert into HumanMoney(' + 'HumanID, ' + '"MoneyName", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertItems := Stms.AddSQLStatement('HumanInsertItems');
  FStatementInsertItems.Sql := 'insert into HumanItems(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'MakeIndex, ' + 'DBIndex, ' + 'Name, ' + 'Dura, ' + 'DuraMax, ' + 'HeroM2DressEffect, ' + 'UpgradeCount, ' + 'IsStartTime, ' + 'LimitTime, ' + 'HeroM2Light, ' + 'Color, ' + 'IsBind, ' + 'BindOption, ' + 'Effect, ' + 'NewLooks, ' + 'NewShape, ' + 'FluteCount, ' + 'PropertyText, ' + 'PropertyTextColor, ' + 'ItemFrom, ' + 'ItemFromMap, ' + 'ItemFromMon, ' + 'ItemFromMaker, ' + 'ItemFromDate, ' + 'InsuranceCount,' + 'NewExpand3, ' + 'NewExpand4 ' + ') ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ' + '?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertItemValueAdd := Stms.AddSQLStatement('HumanInsertItemValueAdd');
  FStatementInsertItemValueAdd.Sql := 'insert into HumanItemValueAdd(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemElementAdd := Stms.AddSQLStatement('HumanInsertItemElementAdd');
  FStatementInsertItemElementAdd.Sql := 'insert into HumanItemElementAdd(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemAddDataByte := Stms.AddSQLStatement('HumanInsertItemAddDataByte');
  FStatementInsertItemAddDataByte.Sql := 'insert into HumanItemAddDataByte(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemAddDataInt := Stms.AddSQLStatement('HumanInsertItemAddDataInt');
  FStatementInsertItemAddDataInt.Sql := 'insert into HumanItemAddDataInt(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemAddDataText := Stms.AddSQLStatement('HumanInsertItemAddDataText');
  FStatementInsertItemAddDataText.Sql := 'insert into HumanItemAddDataText(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemFlute := Stms.AddSQLStatement('HumanInsertItemFlute');
  FStatementInsertItemFlute.Sql := 'insert into HumanItemFlute(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Value,' + 'OverlapCount) ' + 'values(?, ?, ?, ?, ?, ?);';

  FStatementInsertItemProgress := Stms.AddSQLStatement('HumanInsertItemProgress');
  FStatementInsertItemProgress.Sql := 'insert into HumanItemProgress(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'IsOpen, ' + 'NameColor, ' + 'Count, ' + 'ShowType, ' + 'Max, ' + 'Value, ' + 'Level, ' + 'Name) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertItemProperty := Stms.AddSQLStatement('HumanInsertItemProperty');
  FStatementInsertItemProperty.Sql := 'insert into HumanItemProperty(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Color, ' + 'BindType, ' + 'ShowFlag, ' + 'IsPercent, ' + 'HintModule, ' + 'Value, ' + 'Value2, ' + 'Value3) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertSkillPower := Stms.AddSQLStatement('HumanInsertSkillPower');
  FStatementInsertSkillPower.Sql := 'insert into HumanSkillPower(' + 'HumanID, ' + 'SkillID, ' + 'HumanAttackPercent,' + 'HumanAttackValue,' + 'MonAttackPercent,' + 'MonAttackValue,' + 'DefensePercent,' + 'DefenseValue,' + 'RemainingTime) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementGetLevelRankCheckLevel := Stms.AddSQLStatement('HumanGetLevelRankCheckLevel');
  FStatementGetLevelRankCheckLevel.Sql := 'select HumanName, Level from Human where (Job in (?,?,?)) and (Level >= ?) and (Level <= ?) order by Level desc';

  FStatementGetLevelRankTopCount := Stms.AddSQLStatement('HumanGetLevelRankTopCount');
  FStatementGetLevelRankTopCount.Sql := 'select HumanName, Level from Human where (Job in (?,?,?)) order by Level desc limit ?';

  FStatementGetLevelRankCheckLevelAndCount := Stms.AddSQLStatement('HumanGetLevelRankCheckLevelAndCount');
  FStatementGetLevelRankCheckLevelAndCount.Sql := 'select HumanName, Level from Human where (Job in (?,?,?)) and (Level >= ?) and (Level <= ?) order by Level desc  limit ?';

  FStatementGetMasterRankCheckLevel := Stms.AddSQLStatement('HumanGetMasterRankCheckLevel');
  FStatementGetMasterRankCheckLevel.Sql := 'select HumanName, MasterCount from Human where (Job in (?,?,?)) and (Level >= ?) and (Level <= ?) order by MasterCount desc';

  FStatementGetMasterRankTopCount := Stms.AddSQLStatement('HumanGetMasterRankTopCount');
  FStatementGetMasterRankTopCount.Sql := 'select HumanName, MasterCount from Human where (Job in (?,?,?)) order by MasterCount desc limit ?';

  FStatementGetMasterRankCheckLevelAndCount := Stms.AddSQLStatement('GetMasterRankCheckLevelAndCount');
  FStatementGetMasterRankCheckLevelAndCount.Sql := 'select HumanName, MasterCount from Human where (Job in (?,?,?)) and (Level >= ?) and (Level <= ?) order by MasterCount desc  limit ?';

  FStatementBuyPlayer := Stms.AddSQLStatement('BuyPlayer');
  FStatementBuyPlayer.Sql := 'update Human set Account = ?, IsDelete = 0 where Account = ? and HumanName = ?';

  FStatementGetCustomMoney := Stms.AddSQLStatement('GetCustomMoney');
  FStatementGetCustomMoney.Sql := 'select ' + '"MoneyName",' + 'Value ' + 'from HumanMoney ' + 'where HumanID = ?;';
  FStatementGetCustomMoneyByName := Stms.AddSQLStatement('GetCustomMoneyByName');
  FStatementGetCustomMoneyByName.Sql := 'select ' + 'Value ' + 'from HumanMoney ' + 'where HumanID = ? and MoneyName = ?;';

  FStatementUpdateCustomMoney := Stms.AddSQLStatement('UpdateCustomMoney');
  FStatementUpdateCustomMoney.Sql := 'update HumanMoney set ' + 'Value = ?' + 'where HumanID = ? and MoneyName = ?;';

  FStatementGetID.Prepare;
  FStatementCheckHumanExists.Prepare;
  FStatementGetHumanCount.Prepare;
  FStatementGetOtherHumanName.Prepare;
  FStatementGetHumanHeroName.Prepare;
  FStatementGetHumanHeroID.Prepare;
  FStatementGetHumanGoldInfo.Prepare;
  FStatementUpdateHumanGoldInfo.Prepare;
  FStatementGetHumanBaseInfo.Prepare;

  FStatementQueryHumans.Prepare;

  FStatementSearchByAccountMatchComplete.Prepare;
  FStatementSearchByAccountMatchFuzzy.Prepare;

  FStatementSearchByNameMatchComplete.Prepare;
  FStatementSearchByNameMatchFuzzy.Prepare;
  FStatementSearchByLevel.Prepare;

  FStatementGetMobileNumbers1.Prepare;
  FStatementGetMobileNumbers2.Prepare;

  FStatementSelect.Prepare;
  FStatementUnSelect.Prepare;

  FStatementGetHuman.Prepare;
  FStatementGetAbil.Prepare;
  FStatementGetAbilNG.Prepare;
  FStatementGetAbilWine.Prepare;
  FStatementGetAbilNpcAdd.Prepare;
  FStatementGetGamePetData.Prepare;
  FStatementGetGodBlessState.Prepare;
  FStatementGetMagic.Prepare;
  FStatementGetMagicUseTick.Prepare;
  FStatementGetStatusTime.Prepare;
  FStatementGetQuestFlag.Prepare;
  FStatementGetVariableU.Prepare;
  FStatementGetVariableT.Prepare;
  FStatementGetVariableJ.Prepare;
  FStatementGetVariableZ.Prepare;
  FStatementGetItems.Prepare;
  FStatementGetItemValueAdd.Prepare;
  FStatementGetItemElementAdd.Prepare;
  FStatementGetItemAddDataByte.Prepare;
  FStatementGetItemAddDataInt.Prepare;
  FStatementGetItemAddDataText.Prepare;
  FStatementGetItemFlute.Prepare;
  FStatementGetItemProgress.Prepare;
  FStatementGetItemProperty.Prepare;
  FStatementGetSkillPower.Prepare;

  FStatementAddHuman.Prepare;

  FStatementDeleteOrRestore.Prepare;
  FStatementRecordLoginTime.Prepare;

  FStatementUpdateHumanName.Prepare;
  FStatementUpdateHumanNameDearName.Prepare;
  FStatementUpdateHumanNameMasterName.Prepare;

  FStatementUpdateHuman.Prepare;

  FStatementInsertAbil.Prepare;
  FStatementInsertAbilNG.Prepare;
  FStatementInsertAbilWine.Prepare;

  FStatementUpdateAbil.Prepare;
  FStatementUpdateAbilNG.Prepare;
  FStatementUpdateAbilWine.Prepare;

  FStatementInsertAbilNpcAdd.Prepare;
  FStatementInsertGamePetData.Prepare;
  FStatementInsertGodBlessState.Prepare;
  FStatementInsertMagic.Prepare;
  FStatementInsertMagicUseTick.Prepare;
  FStatementInsertStatusTime.Prepare;
  FStatementInsertQuestFlag.Prepare;
  FStatementInsertVariableU.Prepare;
  FStatementInsertVariableT.Prepare;
  FStatementInsertVariableJ.Prepare;
  FStatementInsertVariableZ.Prepare;
  FStatementInsertCustomMoney.Prepare;
  FStatementInsertItems.Prepare;
  FStatementInsertItemValueAdd.Prepare;
  FStatementInsertItemElementAdd.Prepare;
  FStatementInsertItemAddDataByte.Prepare;
  FStatementInsertItemAddDataInt.Prepare;
  FStatementInsertItemAddDataText.Prepare;
  FStatementInsertItemFlute.Prepare;
  FStatementInsertItemProgress.Prepare;
  FStatementInsertItemProperty.Prepare;
  FStatementInsertSkillPower.Prepare;

  FStatementGetLevelRankCheckLevel.Prepare;
  FStatementGetLevelRankTopCount.Prepare;
  FStatementGetLevelRankCheckLevelAndCount.Prepare;

  FStatementGetMasterRankCheckLevel.Prepare;
  FStatementGetMasterRankTopCount.Prepare;
  FStatementGetMasterRankCheckLevelAndCount.Prepare;
  FStatementBuyPlayer.Prepare;

  FStatementGetCustomMoney.Prepare;
  FStatementGetCustomMoneyByName.Prepare;
  FStatementUpdateCustomMoney.Prepare;
end;

procedure TSqliteHumanDB.DoFinal;
begin
  inherited;

  FStatementGetID.Finalize;
  FStatementCheckHumanExists.Finalize;
  FStatementGetHumanCount.Finalize;
  FStatementGetOtherHumanName.Finalize;
  FStatementGetHumanHeroName.Finalize;
  FStatementGetHumanHeroID.Finalize;
  FStatementGetHumanGoldInfo.Finalize;
  FStatementUpdateHumanGoldInfo.Finalize;
  FStatementGetHumanBaseInfo.Finalize;

  FStatementQueryHumans.Finalize;

  FStatementSearchByAccountMatchComplete.Finalize;
  FStatementSearchByAccountMatchFuzzy.Finalize;

  FStatementSearchByNameMatchComplete.Finalize;
  FStatementSearchByNameMatchFuzzy.Finalize;
  FStatementSearchByLevel.Finalize;

  FStatementGetMobileNumbers1.Finalize;
  FStatementGetMobileNumbers2.Finalize;

  FStatementSelect.Finalize;
  FStatementUnSelect.Finalize;

  FStatementGetHuman.Finalize;
  FStatementGetAbil.Finalize;
  FStatementGetAbilNG.Finalize;
  FStatementGetAbilWine.Finalize;
  FStatementGetAbilNpcAdd.Finalize;
  FStatementGetGamePetData.Finalize;
  FStatementGetGodBlessState.Finalize;
  FStatementGetMagic.Finalize;
  FStatementGetMagicUseTick.Finalize;
  FStatementGetStatusTime.Finalize;
  FStatementGetQuestFlag.Finalize;
  FStatementGetVariableU.Finalize;
  FStatementGetVariableT.Finalize;
  FStatementGetVariableJ.Finalize;
  FStatementGetVariableZ.Finalize;
  FStatementGetItems.Finalize;
  FStatementGetItemValueAdd.Finalize;
  FStatementGetItemElementAdd.Finalize;
  FStatementGetItemAddDataByte.Finalize;
  FStatementGetItemAddDataInt.Finalize;
  FStatementGetItemAddDataText.Finalize;
  FStatementGetItemFlute.Finalize;
  FStatementGetItemProgress.Finalize;
  FStatementGetItemProperty.Finalize;
  FStatementGetSkillPower.Finalize;

  FStatementAddHuman.Finalize;

  FStatementDeleteOrRestore.Finalize;
  FStatementRecordLoginTime.Finalize;

  FStatementUpdateHumanName.Finalize;
  FStatementUpdateHumanNameDearName.Finalize;
  FStatementUpdateHumanNameMasterName.Finalize;

  FStatementUpdateHuman.Finalize;

  FStatementInsertAbil.Finalize;
  FStatementInsertAbilNG.Finalize;
  FStatementInsertAbilWine.Finalize;

  FStatementUpdateAbil.Finalize;
  FStatementUpdateAbilNG.Finalize;
  FStatementUpdateAbilWine.Finalize;

  FStatementInsertAbilNpcAdd.Finalize;
  FStatementInsertGamePetData.Finalize;
  FStatementInsertGodBlessState.Finalize;
  FStatementInsertMagic.Finalize;
  FStatementInsertMagicUseTick.Finalize;
  FStatementInsertStatusTime.Finalize;
  FStatementInsertQuestFlag.Finalize;
  FStatementInsertVariableU.Finalize;
  FStatementInsertVariableT.Finalize;
  FStatementInsertVariableJ.Finalize;
  FStatementInsertVariableZ.Finalize;
  FStatementInsertCustomMoney.Finalize;
  FStatementInsertItems.Finalize;
  FStatementInsertItemValueAdd.Finalize;
  FStatementInsertItemElementAdd.Finalize;
  FStatementInsertItemAddDataByte.Finalize;
  FStatementInsertItemAddDataInt.Finalize;
  FStatementInsertItemAddDataText.Finalize;
  FStatementInsertItemFlute.Finalize;
  FStatementInsertItemProgress.Finalize;
  FStatementInsertItemProperty.Finalize;
  FStatementInsertSkillPower.Finalize;

  FStatementGetLevelRankCheckLevel.Finalize;
  FStatementGetLevelRankTopCount.Finalize;
  FStatementGetLevelRankCheckLevelAndCount.Finalize;

  FStatementGetMasterRankCheckLevel.Finalize;
  FStatementGetMasterRankTopCount.Finalize;
  FStatementGetMasterRankCheckLevelAndCount.Finalize;

  FStatementBuyPlayer.Finalize;

  FStatementGetCustomMoney.Finalize;
  FStatementGetCustomMoneyByName.Finalize;
  FStatementUpdateCustomMoney.Finalize;
end;

function TSqliteHumanDB.DoGetID(HumanName: string): Integer;
begin
  try
    FStatementGetID.Reset;
    FStatementGetID.OrderBindText(HumanName);
    if FStatementGetID.Step = SQLITE_ROW then
      Result := FStatementGetID.GetColumnValueInt(0)
    else
      Result := NO_ID;
  finally
    FStatementGetID.Reset;
  end;
end;

function TSqliteHumanDB.DoCheckHumanExists(Account, HumanName: string): Boolean;
begin
  try
    FStatementCheckHumanExists.Reset;
    FStatementCheckHumanExists.OrderBindText(Account);
    FStatementCheckHumanExists.OrderBindText(HumanName);
    Result := FStatementCheckHumanExists.Step = SQLITE_ROW;
  finally
    FStatementCheckHumanExists.Reset;
  end;
end;

function TSqliteHumanDB.DoGetHumanCount(Account: string): Integer;
begin
  try
    FStatementGetHumanCount.Reset;
    FStatementGetHumanCount.OrderBindText(Account);
    if FStatementGetHumanCount.Step = SQLITE_ROW then
      Result := FStatementGetHumanCount.GetColumnValueInt(0)
    else
      Result := 0;
  finally
    FStatementGetHumanCount.Reset;
  end;
end;

function TSqliteHumanDB.DoGetOtherHumanName(Account, HumanName: string): string;
begin
  try
    FStatementGetOtherHumanName.Reset;
    FStatementGetOtherHumanName.OrderBindText(Account);
    FStatementGetOtherHumanName.OrderBindText(HumanName);
    if FStatementGetOtherHumanName.Step = SQLITE_ROW then
      Result := FStatementGetOtherHumanName.GetColumnValueText(0)
    else
      Result := '';
  finally
    FStatementGetOtherHumanName.Reset;
  end;
end;

function TSqliteHumanDB.DoGetHumanHeroName(Account, HumanName: string; var HeroName, DeputyHeroName: string): Boolean;
begin
  Result := False;
  try
    FStatementGetHumanHeroName.Reset;
    FStatementGetHumanHeroName.OrderBindText(HumanName);
    if FStatementGetHumanHeroName.Step = SQLITE_ROW then
    begin
      Result := True;
      HeroName := FStatementGetHumanHeroName.GetColumnValueText(0);
      DeputyHeroName := FStatementGetHumanHeroName.GetColumnValueText(1);
    end;
  finally
    FStatementGetHumanHeroName.Reset;
  end;
end;

function TSqliteHumanDB.DoGetBaseInfo(HumanName: string; var Sex, Job, Level, LastLogin: Integer): Boolean;
begin
  Result := False;
  try
    FStatementGetHumanBaseInfo.Reset;
    FStatementGetHumanBaseInfo.OrderBindText(HumanName);
    if FStatementGetHumanBaseInfo.Step = SQLITE_ROW then
    begin
      Result := True;
      Sex := FStatementGetHumanBaseInfo.GetColumnValueInt(0);
      Job := FStatementGetHumanBaseInfo.GetColumnValueInt(1);
      Level := FStatementGetHumanBaseInfo.GetColumnValueInt(2);
      LastLogin := FStatementGetHumanBaseInfo.GetColumnValueInt(3);
    end;
  finally
    FStatementGetHumanBaseInfo.Reset;
  end;
end;

function TSqliteHumanDB.DoQueryHumans(Account: string; HumanList: TQueryHumanList): Integer;
var
  Ret: Integer;
  QueryData: TQueryHumanData;
begin
  Result := 0;

  try
    FStatementQueryHumans.Reset;
    FStatementQueryHumans.OrderBindText(Account);
    FStatementQueryHumans.OrderBindInt(0);
    Ret := FStatementQueryHumans.Step;

    while (Ret = SQLITE_ROW) do
    begin
      QueryData.HumanName := FStatementQueryHumans.OrderGetColumnValueText;
      QueryData.IsSelect := FStatementQueryHumans.OrderGetColumnValueBool;
      QueryData.Sex := FStatementQueryHumans.OrderGetColumnValueInt;
      QueryData.Job := FStatementQueryHumans.OrderGetColumnValueInt;
      QueryData.Hair := FStatementQueryHumans.OrderGetColumnValueInt;
      QueryData.Level := FStatementQueryHumans.OrderGetColumnValueInt;

      if (QueryData.Sex < 0) or (QueryData.Sex > 1) then
        QueryData.Sex := 0;
      if (QueryData.Job < 0) or (QueryData.Job > 2) then
        QueryData.Job := 0;

      HumanList.Add(@QueryData);

      Ret := FStatementQueryHumans.Step;
      Inc(Result);
    end;
  finally
    FStatementQueryHumans.Reset;
  end;
end;

function TSqliteHumanDB.DoQueryDeleteHumans(Account: string; HumanList: TQueryHumanList): Integer;
var
  Ret: Integer;
  QueryData: TQueryHumanData;
begin
  Result := 0;

  try
    FStatementQueryHumans.Reset;
    FStatementQueryHumans.OrderBindText(Account);
    FStatementQueryHumans.OrderBindInt(1);
    Ret := FStatementQueryHumans.Step;

    while (Ret = SQLITE_ROW) do
    begin
      QueryData.HumanName := FStatementQueryHumans.OrderGetColumnValueText;
      QueryData.IsSelect := FStatementQueryHumans.OrderGetColumnValueBool;
      QueryData.Sex := FStatementQueryHumans.OrderGetColumnValueInt;
      QueryData.Job := FStatementQueryHumans.OrderGetColumnValueInt;
      QueryData.Hair := FStatementQueryHumans.OrderGetColumnValueInt;
      QueryData.Level := FStatementQueryHumans.OrderGetColumnValueInt;
      HumanList.Add(@QueryData);

      Ret := FStatementQueryHumans.Step;
      Inc(Result);
    end;
  finally
    FStatementQueryHumans.Reset;
  end;
end;

function TSqliteHumanDB.DoSearchByAccount(Account: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer;
var
  Ret: Integer;
  SearchData: TSerarchRoleData;
begin
  Result := 0;
  if MatchType = smtComplete then
  begin
    try
      FStatementSearchByAccountMatchComplete.Reset;
      FStatementSearchByAccountMatchComplete.OrderBindText(Account);
      Ret := FStatementSearchByAccountMatchComplete.Step;
      while (Ret = SQLITE_ROW) do
      begin
        SearchData.Account := FStatementSearchByAccountMatchComplete.OrderGetColumnValueText;
        SearchData.RoleName := FStatementSearchByAccountMatchComplete.OrderGetColumnValueText;
        SearchData.IsDelete := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
        SearchData.Sex := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
        SearchData.Job := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
        SearchData.Level := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
        SearchData.IsHero := False;
        RoleList.Add(@SearchData);

        Ret := FStatementSearchByAccountMatchComplete.Step;
        Inc(Result);
      end;
    finally
      FStatementSearchByAccountMatchComplete.Reset;
    end;
  end
  else
  begin
    try
      FStatementSearchByAccountMatchFuzzy.Reset;
      FStatementSearchByAccountMatchFuzzy.OrderBindText('%' + Account + '%');
      Ret := FStatementSearchByAccountMatchFuzzy.Step;
      while (Ret = SQLITE_ROW) do
      begin
        SearchData.Account := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueText;
        SearchData.RoleName := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueText;
        SearchData.IsDelete := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
        SearchData.Sex := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
        SearchData.Job := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
        SearchData.Level := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
        SearchData.IsHero := False;
        RoleList.Add(@SearchData);

        Ret := FStatementSearchByAccountMatchFuzzy.Step;
        Inc(Result);
      end;
    finally
      FStatementSearchByAccountMatchFuzzy.Reset;
    end;
  end;
end;

function TSqliteHumanDB.DoSearchByName(HumanName: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer;
var
  Ret: Integer;
  SearchData: TSerarchRoleData;
begin
  Result := 0;

  if MatchType = smtComplete then
  begin
    try
      FStatementSearchByNameMatchComplete.Reset;
      FStatementSearchByNameMatchComplete.OrderBindText(HumanName);
      Ret := FStatementSearchByNameMatchComplete.Step;
      while (Ret = SQLITE_ROW) do
      begin
        SearchData.Account := FStatementSearchByNameMatchComplete.OrderGetColumnValueText;
        SearchData.RoleName := FStatementSearchByNameMatchComplete.OrderGetColumnValueText;
        SearchData.IsDelete := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
        SearchData.Sex := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
        SearchData.Job := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
        SearchData.Level := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
        SearchData.IsHero := False;
        RoleList.Add(@SearchData);

        Ret := FStatementSearchByNameMatchComplete.Step;
        Inc(Result);
      end;
    finally
      FStatementSearchByNameMatchComplete.Reset;
    end;
  end
  else
  begin
    try
      FStatementSearchByNameMatchFuzzy.Reset;
      FStatementSearchByNameMatchFuzzy.OrderBindText('%' + HumanName + '%');
      Ret := FStatementSearchByNameMatchFuzzy.Step;

      while (Ret = SQLITE_ROW) do
      begin
        SearchData.Account := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueText;
        SearchData.RoleName := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueText;
        SearchData.IsDelete := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
        SearchData.Sex := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
        SearchData.Job := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
        SearchData.Level := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
        SearchData.IsHero := False;
        RoleList.Add(@SearchData);

        Ret := FStatementSearchByNameMatchFuzzy.Step;
        Inc(Result);
      end;
    finally
      FStatementSearchByNameMatchFuzzy.Reset;
    end;
  end;
end;

function TSqliteHumanDB.DoSearchByLevel(LimitCount, MinLevel: Integer; RoleList: TSerarchRoleList): Integer;
var
  Ret: Integer;
  SearchData: TSerarchRoleData;
begin
  try
    FStatementSearchByLevel.Reset;
    FStatementSearchByLevel.OrderBindInt(MinLevel);
    FStatementSearchByLevel.OrderBindInt(LimitCount);
    Ret := FStatementSearchByLevel.Step;

    while (Ret = SQLITE_ROW) do
    begin
      SearchData.Account := FStatementSearchByLevel.OrderGetColumnValueText;
      SearchData.RoleName := FStatementSearchByLevel.OrderGetColumnValueText;
      SearchData.IsDelete := FStatementSearchByLevel.OrderGetColumnValueInt;
      SearchData.Sex := FStatementSearchByLevel.OrderGetColumnValueInt;
      SearchData.Job := FStatementSearchByLevel.OrderGetColumnValueInt;
      SearchData.Level := FStatementSearchByLevel.OrderGetColumnValueInt;
      SearchData.IsHero := False;
      RoleList.Add(@SearchData);

      Ret := FStatementSearchByLevel.Step;
      Inc(Result);
    end;
  finally
    FStatementSearchByNameMatchFuzzy.Reset;
  end;
end;

function TSqliteHumanDB.DoGetMobileNumbers(OnlyBindMobile: Boolean; MobileNumberList: TStrings): Integer;
var
  Ret: Integer;
begin
  if not OnlyBindMobile then
  begin
    try
      FStatementGetMobileNumbers1.Reset;
      Ret := FStatementGetMobileNumbers1.Step;
      while (Ret = SQLITE_ROW) do
      begin
        MobileNumberList.Add(FStatementGetMobileNumbers1.OrderGetColumnValueText);

        Ret := FStatementGetMobileNumbers1.Step;
        Inc(Result);
      end;
    finally
      FStatementGetMobileNumbers1.Reset;
    end;
  end
  else
  begin
    try
      FStatementGetMobileNumbers2.Reset;
      Ret := FStatementGetMobileNumbers2.Step;
      while (Ret = SQLITE_ROW) do
      begin
        MobileNumberList.Add(FStatementGetMobileNumbers2.OrderGetColumnValueText);

        Ret := FStatementGetMobileNumbers2.Step;
        Inc(Result);
      end;
    finally
      FStatementGetMobileNumbers2.Reset;
    end;
  end;
end;

function TSqliteHumanDB.DoSelect(Account, HumanName: string): Boolean;
begin
  Result := False;
  BeginTransaction;
  try
    try
      FStatementSelect.Reset;
      FStatementSelect.OrderBindText(Account);
      FStatementSelect.OrderBindText(HumanName);
      Result := FStatementSelect.Step in [SQLITE_OK, SQLITE_DONE];

      FStatementUnSelect.Reset;
      FStatementUnSelect.OrderBindText(Account);
      FStatementUnSelect.OrderBindText(HumanName);
      FStatementUnSelect.Step;
    finally
      FStatementSelect.Reset;
      FStatementUnSelect.Reset;
    end;

    Commit;
  except
    on E: Exception do
    begin
      RollBack;
      MainOutMessage(E.Message);
    end;
  end;
end;

function TSqliteHumanDB.DoGet(Account, HumanName: string; var HumData: THumData; var HumanID: Integer): Boolean;
var
  Ret: Integer;
  I, J, TheType, Index, Index2, Value: Integer;
  sTemp: string;
  GamePetData: pTGamePetData;
  Magic: PTHumMagic;
  UserItem: pTUserItem;
begin
  Result := False;

  try
    FStatementGetHuman.Reset;
    FStatementGetHuman.OrderBindText(Account);
    FStatementGetHuman.OrderBindText(HumanName);
    Ret := FStatementGetHuman.Step;

    if (Ret = SQLITE_ROW) then
    begin
      Result := True;
      FillChar(HumData, SizeOf(HumData), 0);
      HumanID := FStatementGetHuman.OrderGetColumnValueInt;

      HumData.sAccount := FStatementGetHuman.OrderGetColumnValueText;
      HumData.sChrName := FStatementGetHuman.OrderGetColumnValueText;
      HumData.btSex := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.btJob := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.btHair := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.btDir := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.Abil.Level := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.btReLevel := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.sCurMap := FStatementGetHuman.OrderGetColumnValueText;
      HumData.wCurX := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.wCurY := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.sHomeMap := FStatementGetHuman.OrderGetColumnValueText;
      HumData.wHomeX := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.wHomeY := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.btAttackMode := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.sStoragePwd := FStatementGetHuman.OrderGetColumnValueText;
      HumData.Abil.CreditPoint := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nGold := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nGameGold := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nGamePoint := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nGameDiamond := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nGameGird := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nGameGoldEx := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nGameGlory := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nPKPoint := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nPayMentPoint := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nMemberType := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nMemberLevel := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.boMaster := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.sMasterName := FStatementGetHuman.OrderGetColumnValueText;
      HumData.wMasterCount := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.btMarryCount := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.sDearName := FStatementGetHuman.OrderGetColumnValueText;
      HumData.btIncHealth := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.btIncSpell := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.btIncHealing := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.btFightZoneDieCount := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.dBodyLuck := FStatementGetHuman.OrderGetColumnValueDouble;
      HumData.wContribution := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nHungerStatus := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nKickCount := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.boLockLogin := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.boAllowGroup := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.boAllowGroupReCall := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.wGroupRecallTime := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.boAllowGuildReCall := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.boDisableTrading := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.boDisableInviteHorseRiding := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.boGameGoldTrading := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.boNewServer := FStatementGetHuman.OrderGetColumnValueBool;

      //HumData.boFilterGlobalMsg := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.boFilterGlobalDropItemMsg := FStatementGetHuman.OrderGetColumnValueBool;           // 过滤掉落提示信息 chongchong 2017-04-16
      HumData.boFilterGlobalCenterMsg := FStatementGetHuman.OrderGetColumnValueBool;             // 过滤SendCenterMsg chongchong 2017-04-16
      HumData.boFilterGolbalSendMsg := FStatementGetHuman.OrderGetColumnValueBool;               // 过滤SendMsg全局信息 chongchong 2017-04-16

      HumData.boFixedHero := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.boStorageHero := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.boStorageDeputyHero := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.sHeroName := FStatementGetHuman.OrderGetColumnValueText;
      HumData.sDeputyHeroName := FStatementGetHuman.OrderGetColumnValueText;
      HumData.btDeputyHeroJob := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.btNation := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nNationCredit := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.nRevivalTime := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.dwInfinityStorageExtCount := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.boSaveKillMonExpRate := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.nKillMonExpRate := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.dwKillMonExpRateTime := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.boAttackHumSavePowerRate := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.nAttackHumPowerRate := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.dwAttackHumPowerRateTime := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.boAttackMonSavePowerRate := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.nAttackMonPowerRate := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.dwAttackMonPowerRateTime := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.boSaveKillMonBurstRate := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.nKillMonBurstRate := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.dwKillMonBurstRateTime := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.dwFBCreateTime := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.JewelryBoxStatus := TJewelryBoxStatus(FStatementGetHuman.OrderGetColumnValueInt);
      HumData.boShowFashion := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.boShowGodBless := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.nActiveFengHao := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.boStorageOpen[0] := True;
      HumData.boStorageOpen[1] := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.boStorageOpen[2] := FStatementGetHuman.OrderGetColumnValueBool;
      HumData.boStorageOpen[3] := FStatementGetHuman.OrderGetColumnValueBool;

      HumData.btExtBagPageCount := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.btExtBagOpenItemCount := FStatementGetHuman.OrderGetColumnValueInt;
      HumData.dwAddMaxWeight := FStatementGetHuman.OrderGetColumnValueInt;

      HumData.dwHighLevelKillMonFixExpTimeLeft := FStatementGetHuman.OrderGetColumnValueInt;

      HumData.sMobileNumber := FStatementGetHuman.OrderGetColumnValueText;                       // 手机号码
      HumData.boMobileBind := FStatementGetHuman.OrderGetColumnValueBool;                        // 是否绑定
      HumData.sMobileVerifyCode := FStatementGetHuman.OrderGetColumnValueText;                   // 验证码
      HumData.dwMobileSendTick := FStatementGetHuman.OrderGetColumnValueInt;                     // 最后发送时间
      HumData.nMobileResendCount := FStatementGetHuman.OrderGetColumnValueInt;                   // 重发验证码次数

      HumData.nClearDayVarTime := FStatementGetHuman.OrderGetColumnValueInt;                   // 重发验证码次数
    end;

    if not Result then
      Exit;

    FStatementGetAbil.Reset;
    FStatementGetAbil.OrderBindInt(HumanID);
    Ret := FStatementGetAbil.Step;
    if (Ret = SQLITE_ROW) then
    begin
      HumData.Abil.AC1 := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.AC2 := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.MAC1 := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.MAC2 := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.DC1 := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.DC2 := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.MC1 := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.MC2 := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.SC1 := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.SC2 := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.HP := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.MaxHP := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.MP := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.MaxMP := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.Exp := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.MaxExp := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.Weight := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.MaxWeight := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.WearWeight := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.MaxWearWeight := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.HandWeight := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.Abil.MaxHandWeight := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.nBonusPoint := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.BonusAbil.DC := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.BonusAbil.MC := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.BonusAbil.SC := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.BonusAbil.AC := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.BonusAbil.MAC := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.BonusAbil.HP := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.BonusAbil.MP := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.BonusAbil.Hit := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.BonusAbil.Speed := FStatementGetAbil.OrderGetColumnValueInt;
      HumData.BonusAbil.X2 := FStatementGetAbil.OrderGetColumnValueInt;
    end;

    FStatementGetAbilNG.Reset;
    FStatementGetAbilNG.OrderBindInt(HumanID);
    Ret := FStatementGetAbilNG.Step;
    if (Ret = SQLITE_ROW) then
    begin
      HumData.boTrainingNG := FStatementGetAbilNG.OrderGetColumnValueBool;
      HumData.boTrainingXF := FStatementGetAbilNG.OrderGetColumnValueBool;
      HumData.AbilNG.Level := FStatementGetAbilNG.OrderGetColumnValueInt;
      HumData.AbilNG.NH := FStatementGetAbilNG.OrderGetColumnValueInt;
      HumData.AbilNG.MaxNH := FStatementGetAbilNG.OrderGetColumnValueInt;
      HumData.AbilNG.Exp := FStatementGetAbilNG.OrderGetColumnValueInt;
      HumData.AbilNG.MaxExp := FStatementGetAbilNG.OrderGetColumnValueInt;
      HumData.ContinuousMagicOrder[0] := FStatementGetAbilNG.OrderGetColumnValueInt;
      HumData.ContinuousMagicOrder[1] := FStatementGetAbilNG.OrderGetColumnValueInt;
      HumData.ContinuousMagicOrder[2] := FStatementGetAbilNG.OrderGetColumnValueInt;
      HumData.boOpenLastContinuous := FStatementGetAbilNG.OrderGetColumnValueBool;
      HumData.btLastContinuousMagicOrder := FStatementGetAbilNG.OrderGetColumnValueInt;

      for I := 0 to 4 do
      begin
        HumData.Meridians[I].Level := FStatementGetAbilNG.OrderGetColumnValueInt;
        HumData.Meridians[I].BlastHitRate := FStatementGetAbilNG.OrderGetColumnValueInt;

        for J := 0 to 4 do
        begin
          HumData.Meridians[I].Acupoints[J] := FStatementGetAbilNG.OrderGetColumnValueInt;
        end;
      end;
    end;

    FStatementGetAbilWine.Reset;
    FStatementGetAbilWine.OrderBindInt(HumanID);
    Ret := FStatementGetAbilWine.Step;
    if (Ret = SQLITE_ROW) then
    begin
      HumData.boPleaseDrink := FStatementGetAbilWine.OrderGetColumnValueBool;
      HumData.boDrinkWineDrunk := FStatementGetAbilWine.OrderGetColumnValueBool;
      HumData.nDrinkWineQuality := FStatementGetAbilWine.OrderGetColumnValueInt;
      HumData.nDrinkWineAlcohol := FStatementGetAbilWine.OrderGetColumnValueInt;
      HumData.Alcohol.Alcohol := FStatementGetAbilWine.OrderGetColumnValueInt;
      HumData.Alcohol.MaxAlcohol := FStatementGetAbilWine.OrderGetColumnValueInt;
      HumData.Alcohol.WineDrinkValue := FStatementGetAbilWine.OrderGetColumnValueInt;
      HumData.Alcohol.MedicineLevel := FStatementGetAbilWine.OrderGetColumnValueInt;
      HumData.Alcohol.MedicineValue := FStatementGetAbilWine.OrderGetColumnValueInt;
      HumData.Alcohol.MaxMedicineValue := FStatementGetAbilWine.OrderGetColumnValueInt;
    end;

    FStatementGetAbilNpcAdd.Reset;
    FStatementGetAbilNpcAdd.OrderBindInt(HumanID);
    Ret := FStatementGetAbilNpcAdd.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetAbilNpcAdd.OrderGetColumnValueInt;
      Value := FStatementGetAbilNpcAdd.OrderGetColumnValueInt;

      if (Index >= Low(HumData.AddSaveAbil)) and (Index <= High(HumData.AddSaveAbil)) then
        HumData.AddSaveAbil[Index] := Value;

      Ret := FStatementGetAbilNpcAdd.Step;
    end;

    FStatementGetGamePetData.Reset;
    FStatementGetGamePetData.OrderBindInt(HumanID);
    Ret := FStatementGetGamePetData.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetGamePetData.OrderGetColumnValueInt;
      if (Index >= Low(HumData.GamePetData)) and (Index <= High(HumData.GamePetData)) then
      begin
        GamePetData := @HumData.GamePetData[Index];

        GamePetData.sName := FStatementGetGamePetData.OrderGetColumnValueText;
        GamePetData.Level := FStatementGetGamePetData.OrderGetColumnValueInt;
        GamePetData.HP := FStatementGetGamePetData.OrderGetColumnValueInt;
        GamePetData.MP := FStatementGetGamePetData.OrderGetColumnValueInt;
        GamePetData.Exp := FStatementGetGamePetData.OrderGetColumnValueInt;
        GamePetData.wMagics[0] := FStatementGetGamePetData.OrderGetColumnValueInt;
        GamePetData.wMagics[1] := FStatementGetGamePetData.OrderGetColumnValueInt;
        GamePetData.wMagics[2] := FStatementGetGamePetData.OrderGetColumnValueInt;
        GamePetData.wMagics[3] := FStatementGetGamePetData.OrderGetColumnValueInt;
        GamePetData.wMagics[4] := FStatementGetGamePetData.OrderGetColumnValueInt;
        GamePetData.wMagics[5] := FStatementGetGamePetData.OrderGetColumnValueInt;
        GamePetData.wMagics[6] := FStatementGetGamePetData.OrderGetColumnValueInt;
        GamePetData.wMagics[7] := FStatementGetGamePetData.OrderGetColumnValueInt;
      end;

      Ret := FStatementGetGamePetData.Step;
    end;

    FStatementGetGodBlessState.Reset;
    FStatementGetGodBlessState.OrderBindInt(HumanID);
    Ret := FStatementGetGodBlessState.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetGodBlessState.OrderGetColumnValueInt;
      Value := FStatementGetGodBlessState.OrderGetColumnValueInt;

      if (Index >= Low(HumData.GodBlessItemsState)) and (Index <= High(HumData.GodBlessItemsState)) then
        HumData.GodBlessItemsState[Index] := Value;

      Ret := FStatementGetGodBlessState.Step;
    end;

    FStatementGetMagic.Reset;
    FStatementGetMagic.OrderBindInt(HumanID);
    Ret := FStatementGetMagic.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetMagic.OrderGetColumnValueInt;
      Index := FStatementGetMagic.OrderGetColumnValueInt;
      Magic := nil;
      case TheType of
        1:
          begin
            if (Index >= Low(HumData.Magics)) and (Index <= High(HumData.Magics)) then
              Magic := @HumData.Magics[Index];
          end;
        2:
          begin
            if (Index >= Low(HumData.NGMagics)) and (Index <= High(HumData.NGMagics)) then
              Magic := @HumData.NGMagics[Index];
          end;
        3:
          begin
            if (Index >= Low(HumData.ContinuousMagics)) and (Index <= High(HumData.ContinuousMagics)) then
              Magic := @HumData.ContinuousMagics[Index];
          end;
      end;

      if Magic <> nil then
      begin
        Magic.wMagIdx := FStatementGetMagic.OrderGetColumnValueInt;
        Magic.MagicAttr := TMagicAttr(FStatementGetMagic.OrderGetColumnValueInt);
        Magic.btLevel := FStatementGetMagic.OrderGetColumnValueInt;
        Magic.btNewLevel := FStatementGetMagic.OrderGetColumnValueInt;
        Magic.btKey := FStatementGetMagic.OrderGetColumnValueInt;
        Magic.nTranPoint := FStatementGetMagic.OrderGetColumnValueInt;
        Magic.boUsesItemAdd := FStatementGetMagic.OrderGetColumnValueBool;
      end;

      Ret := FStatementGetMagic.Step;
    end;

    FStatementGetMagicUseTick.Reset;
    FStatementGetMagicUseTick.OrderBindInt(HumanID);
    Ret := FStatementGetMagicUseTick.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetMagicUseTick.OrderGetColumnValueInt - CUSTOM_MAGIC_START_ID;
      Value := FStatementGetMagicUseTick.OrderGetColumnValueInt;

      if (Index >= Low(HumData.CustomSkillUseTicks)) and (Index <= High(HumData.CustomSkillUseTicks)) then
        HumData.CustomSkillUseTicks[Index] := Value;

      Ret := FStatementGetMagicUseTick.Step;
    end;

    FStatementGetStatusTime.Reset;
    FStatementGetStatusTime.OrderBindInt(HumanID);
    Ret := FStatementGetStatusTime.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetStatusTime.OrderGetColumnValueInt;
      Value := FStatementGetStatusTime.OrderGetColumnValueInt;

      if (Index >= Low(HumData.wStatusTimeArr)) and (Index <= High(HumData.wStatusTimeArr)) then
        HumData.wStatusTimeArr[Index] := Value;

      Ret := FStatementGetStatusTime.Step;
    end;

    FStatementGetQuestFlag.Reset;
    FStatementGetQuestFlag.OrderBindInt(HumanID);
    Ret := FStatementGetQuestFlag.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetQuestFlag.OrderGetColumnValueInt;
      Value := FStatementGetQuestFlag.OrderGetColumnValueInt;

      if (Index >= Low(HumData.QuestFlag)) and (Index <= High(HumData.QuestFlag)) then
        HumData.QuestFlag[Index] := Value;

      Ret := FStatementGetQuestFlag.Step;
    end;

    FStatementGetVariableU.Reset;
    FStatementGetVariableU.OrderBindInt(HumanID);
    Ret := FStatementGetVariableU.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetVariableU.OrderGetColumnValueInt;
      Value := FStatementGetVariableU.OrderGetColumnValueInt;

      if (Index >= Low(HumData.UValues)) and (Index <= High(HumData.UValues)) then
        HumData.UValues[Index] := Value;

      Ret := FStatementGetVariableU.Step;
    end;

    FStatementGetVariableT.Reset;
    FStatementGetVariableT.OrderBindInt(HumanID);
    Ret := FStatementGetVariableT.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetVariableT.OrderGetColumnValueInt;
      if (Index >= Low(HumData.TValues)) and (Index <= High(HumData.TValues)) then
        HumData.TValues[Index] := FStatementGetVariableT.OrderGetColumnValueText;

      Ret := FStatementGetVariableT.Step;
    end;

    FStatementGetVariableJ.Reset;
    FStatementGetVariableJ.OrderBindInt(HumanID);
    Ret := FStatementGetVariableJ.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetVariableJ.OrderGetColumnValueInt;
      if (Index >= Low(HumData.JValues)) and (Index <= High(HumData.JValues)) then
        HumData.JValues[Index] := FStatementGetVariableJ.OrderGetColumnValueInt;

      Ret := FStatementGetVariableJ.Step;
    end;

    FStatementGetVariableZ.Reset;
    FStatementGetVariableZ.OrderBindInt(HumanID);
    Ret := FStatementGetVariableZ.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetVariableZ.OrderGetColumnValueInt;
      if (Index >= Low(HumData.ZValues)) and (Index <= High(HumData.ZValues)) then
        HumData.ZValues[Index] := FStatementGetVariableZ.OrderGetColumnValueText;

      Ret := FStatementGetVariableZ.Step;
    end;

    FStatementGetItems.Reset;
    FStatementGetItems.OrderBindInt(HumanID);
    Ret := FStatementGetItems.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItems.OrderGetColumnValueInt;
      Index := FStatementGetItems.OrderGetColumnValueInt;
      UserItem := GetHumanUserItem(@HumData, TheType, Index);
      if UserItem <> nil then
      begin
        UserItem.MakeIndex := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.wIndex := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.Name := FStatementGetItems.OrderGetColumnValueText;
        UserItem.Dura := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.DuraMax := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.dwHeroM2DressEffect := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.btUpgradeCount := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.boStartTime := FStatementGetItems.OrderGetColumnValueBool;
        UserItem.nLimitTime := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.btHeroM2Light := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.btColor := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.boIsBind := FStatementGetItems.OrderGetColumnValueBool;
        UserItem.btBindOption := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.wEffect := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.wNewLooks := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.wNewShape := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.btFluteCount := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.CustomProperty.sText := FStatementGetItems.OrderGetColumnValueText;
        UserItem.CustomProperty.btTextColor := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.ItemFrom.ItemForm := TItemFormType(FStatementGetItems.OrderGetColumnValueInt);
        UserItem.ItemFrom.sMapName := FStatementGetItems.OrderGetColumnValueText;
        UserItem.ItemFrom.sMonName := FStatementGetItems.OrderGetColumnValueText;
        UserItem.ItemFrom.sMakerName := FStatementGetItems.OrderGetColumnValueText;
        UserItem.ItemFrom.DateTime := FStatementGetItems.OrderGetColumnValueDouble;
        UserItem.wInsuranceCount := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.wNewExpand3 := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.wNewExpand4 := FStatementGetItems.OrderGetColumnValueInt;
      end;

      Ret := FStatementGetItems.Step;
    end;

    FStatementGetItemValueAdd.Reset;
    FStatementGetItemValueAdd.OrderBindInt(HumanID);
    Ret := FStatementGetItemValueAdd.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemValueAdd.OrderGetColumnValueInt;
      Index := FStatementGetItemValueAdd.OrderGetColumnValueInt;
      Index2 := FStatementGetItemValueAdd.OrderGetColumnValueInt;
      UserItem := GetHumanUserItem(@HumData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.btValue)) and (Index2 <= High(UserItem.btValue)) then
      begin
        Value := FStatementGetItemValueAdd.OrderGetColumnValueInt;
        UserItem.btValue[Index2] := Value;
      end;

      Ret := FStatementGetItemValueAdd.Step;
    end;

    FStatementGetItemElementAdd.Reset;
    FStatementGetItemElementAdd.OrderBindInt(HumanID);
    Ret := FStatementGetItemElementAdd.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemElementAdd.OrderGetColumnValueInt;
      Index := FStatementGetItemElementAdd.OrderGetColumnValueInt;
      Index2 := FStatementGetItemElementAdd.OrderGetColumnValueInt;
      UserItem := GetHumanUserItem(@HumData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.btNewValue)) and (Index2 <= High(UserItem.btNewValue)) then
      begin
        Value := FStatementGetItemElementAdd.OrderGetColumnValueInt;
        UserItem.btNewValue[Index2] := Value;
      end;

      Ret := FStatementGetItemElementAdd.Step;
    end;

    FStatementGetItemAddDataByte.Reset;
    FStatementGetItemAddDataByte.OrderBindInt(HumanID);
    Ret := FStatementGetItemAddDataByte.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
      Index := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
      Index2 := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
      UserItem := GetHumanUserItem(@HumData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.btAddDataByte)) and (Index2 <= High(UserItem.btAddDataByte)) then
      begin
        Value := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
        UserItem.btAddDataByte[Index2] := Value;
      end;

      Ret := FStatementGetItemAddDataByte.Step;
    end;

    FStatementGetItemAddDataInt.Reset;
    FStatementGetItemAddDataInt.OrderBindInt(HumanID);
    Ret := FStatementGetItemAddDataInt.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
      Index := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
      Index2 := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
      UserItem := GetHumanUserItem(@HumData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.nAddDataInt)) and (Index2 <= High(UserItem.nAddDataInt)) then
      begin
        Value := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
        UserItem.nAddDataInt[Index2] := Value;
      end;

      Ret := FStatementGetItemAddDataInt.Step;
    end;

    FStatementGetItemAddDataText.Reset;
    FStatementGetItemAddDataText.OrderBindInt(HumanID);
    Ret := FStatementGetItemAddDataText.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemAddDataText.OrderGetColumnValueInt;
      Index := FStatementGetItemAddDataText.OrderGetColumnValueInt;
      Index2 := FStatementGetItemAddDataText.OrderGetColumnValueInt;
      UserItem := GetHumanUserItem(@HumData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.sAddDataText)) and (Index2 <= High(UserItem.sAddDataText)) then
      begin
        sTemp := FStatementGetItemAddDataText.OrderGetColumnValueText;
        UserItem.sAddDataText[Index2] := sTemp;
      end;

      Ret := FStatementGetItemAddDataText.Step;
    end;

    FStatementGetItemFlute.Reset;
    FStatementGetItemFlute.OrderBindInt(HumanID);
    Ret := FStatementGetItemFlute.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemFlute.OrderGetColumnValueInt;
      Index := FStatementGetItemFlute.OrderGetColumnValueInt;
      Index2 := FStatementGetItemFlute.OrderGetColumnValueInt;
      UserItem := GetHumanUserItem(@HumData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.Flutes)) and (Index2 <= High(UserItem.Flutes)) then
      begin
        UserItem.Flutes[Index2].GemIndex := FStatementGetItemFlute.OrderGetColumnValueInt;
        UserItem.Flutes[Index2].GemCount := FStatementGetItemFlute.OrderGetColumnValueInt;

        if (UserItem.Flutes[Index2].GemIndex > 0) and (UserItem.Flutes[Index2].GemCount = 0) then
          UserItem.Flutes[Index2].GemCount := 1;
      end;

      Ret := FStatementGetItemFlute.Step;
    end;

    FStatementGetItemProgress.Reset;
    FStatementGetItemProgress.OrderBindInt(HumanID);
    Ret := FStatementGetItemProgress.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemProgress.OrderGetColumnValueInt;
      Index := FStatementGetItemProgress.OrderGetColumnValueInt;
      Index2 := FStatementGetItemProgress.OrderGetColumnValueInt;
      UserItem := GetHumanUserItem(@HumData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.Progress)) and (Index2 <= High(UserItem.Progress)) then
      begin
        UserItem.Progress[Index2].boOpen := FStatementGetItemProgress.OrderGetColumnValueBool;
        UserItem.Progress[Index2].btNameColor := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].btCount := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].btShowType := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].wMax := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].wValue := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].wLevel := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].sName := FStatementGetItemProgress.OrderGetColumnValueText;
      end;

      Ret := FStatementGetItemProgress.Step;
    end;

    FStatementGetItemProperty.Reset;
    FStatementGetItemProperty.OrderBindInt(HumanID);
    Ret := FStatementGetItemProperty.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemProperty.OrderGetColumnValueInt;
      Index := FStatementGetItemProperty.OrderGetColumnValueInt;
      Index2 := FStatementGetItemProperty.OrderGetColumnValueInt;
      UserItem := GetHumanUserItem(@HumData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.CustomProperty.Properties)) and (Index2 <= High(UserItem.CustomProperty.Properties)) then
      begin
        UserItem.CustomProperty.Properties[Index2].btColor := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btBindType := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btShowFlag := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btPercent := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btHintModule := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].nValues[0] := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].nValues[1] := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].nValues[2] := FStatementGetItemProperty.OrderGetColumnValueInt;
      end;

      Ret := FStatementGetItemProperty.Step;
    end;

    FStatementGetSkillPower.Reset;
    FStatementGetSkillPower.OrderBindInt(HumanID);
    Ret := FStatementGetSkillPower.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetSkillPower.OrderGetColumnValueInt;
      if (Index >= Low(HumData.NpcSkillPowerAdd)) and (Index <= High(HumData.NpcSkillPowerAdd)) then
      begin
        HumData.NpcSkillPowerAdd[Index].HumanAttackPercent := FStatementGetSkillPower.OrderGetColumnValueInt;
        HumData.NpcSkillPowerAdd[Index].HumanAttackValue := FStatementGetSkillPower.OrderGetColumnValueInt;
        HumData.NpcSkillPowerAdd[Index].MonAttackPercent := FStatementGetSkillPower.OrderGetColumnValueInt;
        HumData.NpcSkillPowerAdd[Index].MonAttackValue := FStatementGetSkillPower.OrderGetColumnValueInt;
        HumData.NpcSkillPowerAdd[Index].DefensePercent := FStatementGetSkillPower.OrderGetColumnValueInt;
        HumData.NpcSkillPowerAdd[Index].DefenseValue := FStatementGetSkillPower.OrderGetColumnValueInt;
        HumData.NpcSkillPowerAdd[Index].RemainingTime := FStatementGetSkillPower.OrderGetColumnValueInt;
      end;

      Ret := FStatementGetSkillPower.Step;
    end;

    FStatementGetCustomMoney.Reset;
    FStatementGetCustomMoney.OrderBindInt(HumanID);
    Ret := FStatementGetCustomMoney.Step;
    Index := 0;
    while (Ret = SQLITE_ROW) do
    begin
      HumData.CustomMoney[Index].sName := FStatementGetCustomMoney.OrderGetColumnValueText;
      HumData.CustomMoney[Index].nCount := FStatementGetCustomMoney.OrderGetColumnValueInt;
      Inc(Index);
      Ret := FStatementGetCustomMoney.Step;
    end;
  finally
    ResetAllGetDataStatement;
  end;
end;

function TSqliteHumanDB.GetHumanUserItem(HumData: PTHumData; const ItemType, ItemIndex: Integer): PTUserItem;
begin
  Result := nil;

  case ItemType of
    1:
      begin
        if (ItemIndex >= Low(HumData.HumItems)) and (ItemIndex <= High(HumData.HumItems)) then
          Result := @HumData.HumItems[ItemIndex];
      end;
    2:
      begin
        if (ItemIndex >= Low(HumData.JewelryBoxItems)) and (ItemIndex <= High(HumData.JewelryBoxItems)) then
          Result := @HumData.JewelryBoxItems[ItemIndex];
      end;
    3:
      begin
        if (ItemIndex >= Low(HumData.GodBlessItems)) and (ItemIndex <= High(HumData.GodBlessItems)) then
          Result := @HumData.GodBlessItems[ItemIndex];
      end;
    4:
      begin
        if (ItemIndex >= Low(HumData.FengHaoItems)) and (ItemIndex <= High(HumData.FengHaoItems)) then
          Result := @HumData.FengHaoItems[ItemIndex];
      end;
    5:
      begin
        if (ItemIndex >= Low(HumData.BagItems)) and (ItemIndex <= High(HumData.BagItems)) then
          Result := @HumData.BagItems[ItemIndex];
      end;
    6:
      begin
        if (ItemIndex >= Low(HumData.StorageItems)) and (ItemIndex <= High(HumData.StorageItems)) then
          Result := @HumData.StorageItems[ItemIndex];
      end;
    7:
      begin
        if (ItemIndex >= Low(HumData.GamePetBagItems)) and (ItemIndex <= High(HumData.GamePetBagItems)) then
          Result := @HumData.GamePetBagItems[ItemIndex];
      end;
  end;
end;

function TSqliteHumanDB.DoAdd(Account, HumanName: string; IsSelect: Boolean; Sex, Job, Hair: Byte): Boolean;
begin
  try
    FStatementAddHuman.Reset;
    FStatementAddHuman.OrderBindText(Account);
    FStatementAddHuman.OrderBindText(HumanName);
    FStatementAddHuman.OrderBindBool(False);
    FStatementAddHuman.OrderBindBool(IsSelect);
    FStatementAddHuman.OrderBindInt(Date2MyDate(Now()));
    FStatementAddHuman.OrderBindInt(Sex);
    FStatementAddHuman.OrderBindInt(Job);
    FStatementAddHuman.OrderBindInt(Hair);
    Result := FStatementAddHuman.Step in [SQLITE_OK, SQLITE_DONE];
  finally
    FStatementAddHuman.Reset;
  end;
end;

function TSqliteHumanDB.DoDelete(Account, HumanName: string): Boolean;
begin
  try
    FStatementDeleteOrRestore.Reset;
    FStatementDeleteOrRestore.OrderBindInt(1);
    FStatementDeleteOrRestore.OrderBindText(Account);
    FStatementDeleteOrRestore.OrderBindText(HumanName);
    Result := FStatementDeleteOrRestore.Step in [SQLITE_OK, SQLITE_DONE];
  finally
    FStatementDeleteOrRestore.Reset;
  end;
end;

function TSqliteHumanDB.DoDeleteRestore(Account, HumanName: string): Boolean;
begin
  try
    FStatementDeleteOrRestore.Reset;
    FStatementDeleteOrRestore.OrderBindInt(0);
    FStatementDeleteOrRestore.OrderBindText(Account);
    FStatementDeleteOrRestore.OrderBindText(HumanName);
    Result := FStatementDeleteOrRestore.Step in [SQLITE_OK, SQLITE_DONE];
  finally
    FStatementDeleteOrRestore.Reset;
  end;
end;

function TSqliteHumanDB.DoSetEnabled(Account, HumanName: string; Enabled: Integer): Boolean;
begin
  try
    FStatementDeleteOrRestore.Reset;
    FStatementDeleteOrRestore.OrderBindInt(Enabled);
    FStatementDeleteOrRestore.OrderBindText(Account);
    FStatementDeleteOrRestore.OrderBindText(HumanName);
    Result := FStatementDeleteOrRestore.Step in [SQLITE_OK, SQLITE_DONE];
  finally
    FStatementDeleteOrRestore.Reset;
  end;
end;

function TSqliteHumanDB.DoErase(Account, HumanName: string): Boolean;
var
  Ret, HumanID, HeroID: Integer;
  strHeroID: string;
begin
  Result := False;
  HumanID := GetID(HumanName);
  if HumanID = NO_ID then
    Exit;

  strHeroID := '';
  try
    FStatementGetHumanHeroID.Reset;
    FStatementGetHumanHeroID.OrderBindInt(HumanID);
    Ret := FStatementGetHumanHeroID.Step;
    while (Ret = SQLITE_ROW) do
    begin
      HeroID := FStatementGetHumanHeroID.OrderGetColumnValueInt;
      strHeroID := strHeroID + IntToStr(HeroID) + ',';
      Ret := FStatementGetHumanHeroID.Step;
    end;
  finally
    FStatementGetHumanHeroID.Reset;
  end;

  BeginTransaction;
  try
    if Length(strHeroID) > 0 then
    begin
      strHeroID := Copy(strHeroID, 1, Length(strHeroID) - 1);

      Execute(Format('delete from HeroItemElementAdd where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroItemAddDataByte where HeroID = %d;', [HeroID]));
      Execute(Format('delete from HeroItemAddDataInt where HeroID = %d;', [HeroID]));
      Execute(Format('delete from HeroItemAddDataText where HeroID = %d;', [HeroID]));
      Execute(Format('delete from HeroItemFlute where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroItemProgress where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroItemProperty where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroItemValueAdd where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroItems where HeroID in (%s);', [strHeroID]));

      Execute(Format('delete from HeroAbil where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroAbilNG where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroAbilNpcAdd where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroAbilWine where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroSkillPower where HeroID in (%s);', [strHeroID]));

      Execute(Format('delete from HeroGodBlessState where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroMagic where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroQuestFlag where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroStatusTime where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from Hero where HeroID in (%s);', [strHeroID]));
    end;

    Execute(Format('delete from HumanItemElementAdd where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanItemAddDataByte where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanItemAddDataInt where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanItemAddDataText where HumanID = %d;', [HumanID]));

    Execute(Format('delete from HumanItemFlute where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanItemProgress where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanItemProperty where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanItems where HumanID = %d;', [HumanID]));

    Execute(Format('delete from HumanAbil where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanAbilNG where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanAbilNpcAdd where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanAbilWine where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanGamePetData where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanGodBlessState where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanMagic where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanMagicUseTick where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanQuestFlag where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanStatusTime where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanVariableT where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanVariableU where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanVariableJ where HumanID = %d;', [HumanID]));
    Execute(Format('delete from HumanVariableZ where HumanID = %d;', [HumanID]));

    Execute(Format('delete from HumanSkillPower where HumanID = %d;', [HumanID]));

    Execute(Format('delete from HumanMoney where HumanID = %d;', [HumanID]));

    Execute(Format('delete from Human where HumanID = %d;', [HumanID]));

    Commit;

    Result := True;
  except
    on E: Exception do
    begin
      RollBack;
      MainOutMessage(E.Message);
    end;
  end;
end;

function TSqliteHumanDB.DoRecordLoginTime(Account, HumanName: string): Boolean;
begin
  try
    FStatementRecordLoginTime.Reset;
    FStatementRecordLoginTime.OrderBindInt(Date2MyDate(Now()));
    FStatementRecordLoginTime.OrderBindText(Account);
    FStatementRecordLoginTime.OrderBindText(HumanName);
    Result := FStatementRecordLoginTime.Step in [SQLITE_OK, SQLITE_DONE];
  finally
    FStatementRecordLoginTime.Reset;
  end;
end;

function TSqliteHumanDB.DoSave(HumanID: Integer; HumData: PTHumData): Boolean;
begin
  Result := False;
  BeginTransaction;
  try
    Result := SaveHumanData(HumanID, HumData);
    Commit;
  except
    on E: Exception do
    begin
      RollBack;
      MainOutMessage(E.Message);
    end;
  end;
end;

function TSqliteHumanDB.DoRename(Account, HumanName: string; HumanID: Integer; NewName: string): Boolean;
begin
  Result := False;
  BeginTransaction;
  try
    try
      FStatementUpdateHumanName.Reset;
      FStatementUpdateHumanName.OrderBindText(NewName);
      FStatementUpdateHumanName.OrderBindInt(HumanID);
      Result := FStatementUpdateHumanName.Step in [SQLITE_OK, SQLITE_DONE];

      if Result then
      begin
        FStatementUpdateHumanNameDearName.Reset;
        FStatementUpdateHumanNameDearName.OrderBindText(NewName);
        FStatementUpdateHumanNameDearName.OrderBindText(HumanName);
        Result := FStatementUpdateHumanNameDearName.Step in [SQLITE_OK, SQLITE_DONE];

        FStatementUpdateHumanNameMasterName.Reset;
        FStatementUpdateHumanNameMasterName.OrderBindText(NewName);
        FStatementUpdateHumanNameMasterName.OrderBindText(HumanName);
        FStatementUpdateHumanNameMasterName.Step;
      end;

      Commit;
    finally
      FStatementUpdateHumanName.Reset;
      FStatementUpdateHumanNameDearName.Reset;
      FStatementUpdateHumanNameMasterName.Reset;
    end;
  except
    on E: Exception do
    begin
      RollBack;
      MainOutMessage(E.Message);
    end;
  end;
end;

function TSqliteHumanDB.DoChangedCustomMoney(HumanID: Integer; CustomMoneyName: string; ChangedValue: Integer; var ResultValue: LongWord): Boolean;
var
  nValue: LongWord;
  I64: Int64;
begin
  try
    FStatementGetCustomMoneyByName.Reset;
    FStatementGetCustomMoneyByName.OrderBindInt(HumanID);
    FStatementGetCustomMoneyByName.OrderBindText(CustomMoneyName);

    if FStatementGetCustomMoneyByName.Step in [SQLITE_ROW] then
    begin
      nValue := FStatementGetCustomMoneyByName.OrderGetColumnValueInt;

      I64 := Int64(nValue) + ChangedValue;
      if I64 < 0 then
        I64 := 0
      else if I64 > High(LongWord) then
        I64 := High(LongWord);
      nValue := I64;
      ResultValue := nValue;

      try
        FStatementUpdateCustomMoney.Reset;
        FStatementUpdateCustomMoney.OrderBindInt(nValue);
        FStatementUpdateCustomMoney.OrderBindInt(HumanID);
        FStatementUpdateCustomMoney.OrderBindText(CustomMoneyName);
        Result := FStatementUpdateCustomMoney.Step in [SQLITE_OK, SQLITE_DONE];
      finally
        FStatementUpdateCustomMoney.Reset;
      end;
    end;
  finally
    FStatementUpdateCustomMoney.Reset;
  end;
end;

function TSqliteHumanDB.DoChangedGold(HumanName: string; ChangeType: TDBChangeGoldType; ChangedValue: Integer; var ResultValue: LongWord): Boolean;
var
  Gold, GameGold, GamePoint, GameDiamond, GameGird: LongWord;
  I64: Int64;
begin
  try
    FStatementGetHumanGoldInfo.Reset;
    FStatementGetHumanGoldInfo.OrderBindText(HumanName);

    if FStatementGetHumanGoldInfo.Step in [SQLITE_ROW] then
    begin
      Gold := FStatementGetHumanGoldInfo.OrderGetColumnValueInt;
      GameGold := FStatementGetHumanGoldInfo.OrderGetColumnValueInt;
      GamePoint := FStatementGetHumanGoldInfo.OrderGetColumnValueInt;
      GameDiamond := FStatementGetHumanGoldInfo.OrderGetColumnValueInt;
      GameGird := FStatementGetHumanGoldInfo.OrderGetColumnValueInt;

      case ChangeType of
        cgtGold:
          begin
            I64 := Int64(Gold) + ChangedValue;
            if I64 < 0 then
              I64 := 0
            else if I64 > High(LongWord) then
              I64 := High(LongWord);
            Gold := I64;
            ResultValue := Gold;
          end;
        cgtGameGold:
          begin
            I64 := Int64(GameGold) + ChangedValue;
            if I64 < 0 then
              I64 := 0
            else if I64 > High(LongWord) then
              I64 := High(LongWord);
            GameGold := I64;
            ResultValue := GameGold;
          end;
        cgtGamePoint:
          begin
            I64 := Int64(GamePoint) + ChangedValue;
            if I64 < 0 then
              I64 := 0
            else if I64 > High(LongWord) then
              I64 := High(LongWord);
            GamePoint := I64;
            ResultValue := GamePoint;
          end;
        cgtGameDiamond:
          begin
            I64 := Int64(GameDiamond) + ChangedValue;
            if I64 < 0 then
              I64 := 0
            else if I64 > High(LongWord) then
              I64 := High(LongWord);
            GameDiamond := I64;
            ResultValue := GameDiamond;
          end;
        cgtGameGird:
          begin
            I64 := Int64(GameGird) + ChangedValue;
            if I64 < 0 then
              I64 := 0
            else if I64 > High(LongWord) then
              I64 := High(LongWord);
            GameGird := I64;
            ResultValue := GameGird;
          end;
      end;

      try
        FStatementUpdateHumanGoldInfo.Reset;
        FStatementUpdateHumanGoldInfo.OrderBindInt(Gold);
        FStatementUpdateHumanGoldInfo.OrderBindInt(GameGold);
        FStatementUpdateHumanGoldInfo.OrderBindInt(GamePoint);
        FStatementUpdateHumanGoldInfo.OrderBindInt(GameDiamond);
        FStatementUpdateHumanGoldInfo.OrderBindInt(GameGird);
        FStatementUpdateHumanGoldInfo.OrderBindText(HumanName);
        Result := FStatementUpdateHumanGoldInfo.Step in [SQLITE_OK, SQLITE_DONE];
      finally
        FStatementUpdateHumanGoldInfo.Reset;
      end;
    end;
  finally
    FStatementGetHumanGoldInfo.Reset;
  end;
end;

function TSqliteHumanDB.SaveHumanData(HumanID: Integer; HumData: PTHumData): Boolean;
var
  I, J: Integer;
  HumMagic: PTHumMagic;
  UserItem: PTUserItem;
begin
  Result := False;
  try
    FStatementUpdateHuman.Reset;
    FStatementUpdateHuman.OrderBindInt(HumData.btSex);
    FStatementUpdateHuman.OrderBindInt(HumData.btJob);
    FStatementUpdateHuman.OrderBindInt(HumData.btHair);
    FStatementUpdateHuman.OrderBindInt(HumData.btDir);
    FStatementUpdateHuman.OrderBindInt(HumData.Abil.Level);
    FStatementUpdateHuman.OrderBindInt(HumData.btReLevel);
    FStatementUpdateHuman.OrderBindText(HumData.sCurMap);
    FStatementUpdateHuman.OrderBindInt(HumData.wCurX);
    FStatementUpdateHuman.OrderBindInt(HumData.wCurY);
    FStatementUpdateHuman.OrderBindText(HumData.sHomeMap);
    FStatementUpdateHuman.OrderBindInt(HumData.wHomeX);
    FStatementUpdateHuman.OrderBindInt(HumData.wHomeY);
    FStatementUpdateHuman.OrderBindInt(HumData.btAttackMode);
    FStatementUpdateHuman.OrderBindText(HumData.sStoragePwd);
    FStatementUpdateHuman.OrderBindInt(HumData.Abil.CreditPoint);
    FStatementUpdateHuman.OrderBindInt(HumData.nGold);
    FStatementUpdateHuman.OrderBindInt(HumData.nGameGold);
    FStatementUpdateHuman.OrderBindInt(HumData.nGamePoint);
    FStatementUpdateHuman.OrderBindInt(HumData.nGameDiamond);
    FStatementUpdateHuman.OrderBindInt(HumData.nGameGird);
    FStatementUpdateHuman.OrderBindInt(HumData.nGameGoldEx);
    FStatementUpdateHuman.OrderBindInt(HumData.nGameGlory);
    FStatementUpdateHuman.OrderBindInt(HumData.nPKPoint);
    FStatementUpdateHuman.OrderBindInt(HumData.nPayMentPoint);
    FStatementUpdateHuman.OrderBindInt(HumData.nMemberType);
    FStatementUpdateHuman.OrderBindInt(HumData.nMemberLevel);
    FStatementUpdateHuman.OrderBindBool(HumData.boMaster);
    FStatementUpdateHuman.OrderBindText(HumData.sMasterName);
    FStatementUpdateHuman.OrderBindInt(HumData.wMasterCount);
    FStatementUpdateHuman.OrderBindInt(HumData.btMarryCount);
    FStatementUpdateHuman.OrderBindText(HumData.sDearName);
    FStatementUpdateHuman.OrderBindInt(HumData.btIncHealth);
    FStatementUpdateHuman.OrderBindInt(HumData.btIncSpell);
    FStatementUpdateHuman.OrderBindInt(HumData.btIncHealing);
    FStatementUpdateHuman.OrderBindInt(HumData.btFightZoneDieCount);
    FStatementUpdateHuman.OrderBindDouble(HumData.dBodyLuck);
    FStatementUpdateHuman.OrderBindInt(HumData.wContribution);
    FStatementUpdateHuman.OrderBindInt(HumData.nHungerStatus);
    FStatementUpdateHuman.OrderBindInt(HumData.nKickCount);
    FStatementUpdateHuman.OrderBindBool(HumData.boLockLogin);
    FStatementUpdateHuman.OrderBindBool(HumData.boAllowGroup);
    FStatementUpdateHuman.OrderBindBool(HumData.boAllowGroupReCall);
    FStatementUpdateHuman.OrderBindInt(HumData.wGroupRecallTime);
    FStatementUpdateHuman.OrderBindBool(HumData.boAllowGuildReCall);
    FStatementUpdateHuman.OrderBindBool(HumData.boDisableTrading);
    FStatementUpdateHuman.OrderBindBool(HumData.boDisableInviteHorseRiding);
    FStatementUpdateHuman.OrderBindBool(HumData.boGameGoldTrading);
    FStatementUpdateHuman.OrderBindBool(HumData.boNewServer);

    //FStatementUpdateHuman.OrderBindBool(HumData.boFilterGlobalMsg);
    FStatementUpdateHuman.OrderBindBool(HumData.boFilterGlobalDropItemMsg);           // 过滤掉落提示信息 chongchong 2017-04-16
    FStatementUpdateHuman.OrderBindBool(HumData.boFilterGlobalCenterMsg);             // 过滤SendCenterMsg chongchong 2017-04-16
    FStatementUpdateHuman.OrderBindBool(HumData.boFilterGolbalSendMsg);               // 过滤SendMsg全局信息 chongchong 2017-04-16
    //FStatementUpdateHuman.OrderBindBool(HumData.boFixedHero);
    FStatementUpdateHuman.OrderBindBool(HumData.boStorageHero);
    FStatementUpdateHuman.OrderBindBool(HumData.boStorageDeputyHero);
    //FStatementUpdateHuman.OrderBindText(HumData.sHeroName);
    //FStatementUpdateHuman.OrderBindText(HumData.sDeputyHeroName);
    FStatementUpdateHuman.OrderBindInt(HumData.btDeputyHeroJob);
    FStatementUpdateHuman.OrderBindInt(HumData.btNation);
    FStatementUpdateHuman.OrderBindInt(HumData.nNationCredit);
    FStatementUpdateHuman.OrderBindInt(HumData.nRevivalTime);
    FStatementUpdateHuman.OrderBindInt(HumData.dwInfinityStorageExtCount);
    FStatementUpdateHuman.OrderBindBool(HumData.boSaveKillMonExpRate);
    FStatementUpdateHuman.OrderBindInt(HumData.nKillMonExpRate);
    FStatementUpdateHuman.OrderBindInt(HumData.dwKillMonExpRateTime);
    FStatementUpdateHuman.OrderBindBool(HumData.boAttackHumSavePowerRate);
    FStatementUpdateHuman.OrderBindInt(HumData.nAttackHumPowerRate);
    FStatementUpdateHuman.OrderBindInt(HumData.dwAttackHumPowerRateTime);
    FStatementUpdateHuman.OrderBindBool(HumData.boAttackMonSavePowerRate);
    FStatementUpdateHuman.OrderBindInt(HumData.nAttackMonPowerRate);
    FStatementUpdateHuman.OrderBindInt(HumData.dwAttackMonPowerRateTime);
    FStatementUpdateHuman.OrderBindBool(HumData.boSaveKillMonBurstRate);
    FStatementUpdateHuman.OrderBindInt(HumData.nKillMonBurstRate);
    FStatementUpdateHuman.OrderBindInt(HumData.dwKillMonBurstRateTime);
    FStatementUpdateHuman.OrderBindInt(HumData.dwFBCreateTime);
    FStatementUpdateHuman.OrderBindInt(Integer(HumData.JewelryBoxStatus));
    FStatementUpdateHuman.OrderBindBool(HumData.boShowFashion);
    FStatementUpdateHuman.OrderBindBool(HumData.boShowGodBless);
    FStatementUpdateHuman.OrderBindInt(HumData.nActiveFengHao);
    FStatementUpdateHuman.OrderBindBool(HumData.boStorageOpen[1]);
    FStatementUpdateHuman.OrderBindBool(HumData.boStorageOpen[2]);
    FStatementUpdateHuman.OrderBindBool(HumData.boStorageOpen[3]);

    FStatementUpdateHuman.OrderBindInt(HumData.btExtBagPageCount);
    FStatementUpdateHuman.OrderBindInt(HumData.btExtBagOpenItemCount);
    FStatementUpdateHuman.OrderBindInt(HumData.dwAddMaxWeight);

    FStatementUpdateHuman.OrderBindInt(HumData.dwHighLevelKillMonFixExpTimeLeft);

    FStatementUpdateHuman.OrderBindText(HumData.sMobileNumber);                        // 手机号码
    FStatementUpdateHuman.OrderBindBool(HumData.boMobileBind);                         // 是否绑定
    FStatementUpdateHuman.OrderBindText(HumData.sMobileVerifyCode);                    // 验证码
    FStatementUpdateHuman.OrderBindInt(HumData.dwMobileSendTick);                      // 最后发送时间
    FStatementUpdateHuman.OrderBindInt(HumData.nMobileResendCount);                    // 重发验证码次数

    FStatementUpdateHuman.OrderBindInt(HumData.nClearDayVarTime);                       // 变量清空时间

    FStatementUpdateHuman.OrderBindInt(HumanID);

    if not (FStatementUpdateHuman.Step in [SQLITE_OK, SQLITE_DONE]) then
      Exit;

    FStatementUpdateAbil.Reset;
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.AC1);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.AC2);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.MAC1);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.MAC2);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.DC1);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.DC2);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.MC1);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.MC2);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.SC1);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.SC2);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.HP);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.MaxHP);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.MP);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.MaxMP);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.Exp);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.MaxExp);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.Weight);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.MaxWeight);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.WearWeight);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.MaxWearWeight);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.HandWeight);
    FStatementUpdateAbil.OrderBindInt(HumData.Abil.MaxHandWeight);
    FStatementUpdateAbil.OrderBindInt(HumData.nBonusPoint);
    FStatementUpdateAbil.OrderBindInt(HumData.BonusAbil.DC);
    FStatementUpdateAbil.OrderBindInt(HumData.BonusAbil.MC);
    FStatementUpdateAbil.OrderBindInt(HumData.BonusAbil.SC);
    FStatementUpdateAbil.OrderBindInt(HumData.BonusAbil.AC);
    FStatementUpdateAbil.OrderBindInt(HumData.BonusAbil.MAC);
    FStatementUpdateAbil.OrderBindInt(HumData.BonusAbil.HP);
    FStatementUpdateAbil.OrderBindInt(HumData.BonusAbil.MP);
    FStatementUpdateAbil.OrderBindInt(HumData.BonusAbil.Hit);
    FStatementUpdateAbil.OrderBindInt(HumData.BonusAbil.Speed);
    FStatementUpdateAbil.OrderBindInt(HumData.BonusAbil.X2);
    FStatementUpdateAbil.OrderBindInt(HumanID);
    FStatementUpdateAbil.Step;

    // 更新成功行数
    if TSqliteRoleDB(Owner).FDB.GetRowsAffected = 0 then
    begin
      FStatementInsertAbil.Reset;
      FStatementInsertAbil.OrderBindInt(HumanID);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.AC1);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.AC2);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.MAC1);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.MAC2);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.DC1);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.DC2);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.MC1);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.MC2);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.SC1);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.SC2);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.HP);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.MaxHP);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.MP);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.MaxMP);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.Exp);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.MaxExp);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.Weight);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.MaxWeight);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.WearWeight);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.MaxWearWeight);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.HandWeight);
      FStatementInsertAbil.OrderBindInt(HumData.Abil.MaxHandWeight);
      FStatementInsertAbil.OrderBindInt(HumData.nBonusPoint);
      FStatementInsertAbil.OrderBindInt(HumData.BonusAbil.DC);
      FStatementInsertAbil.OrderBindInt(HumData.BonusAbil.MC);
      FStatementInsertAbil.OrderBindInt(HumData.BonusAbil.SC);
      FStatementInsertAbil.OrderBindInt(HumData.BonusAbil.AC);
      FStatementInsertAbil.OrderBindInt(HumData.BonusAbil.MAC);
      FStatementInsertAbil.OrderBindInt(HumData.BonusAbil.HP);
      FStatementInsertAbil.OrderBindInt(HumData.BonusAbil.MP);
      FStatementInsertAbil.OrderBindInt(HumData.BonusAbil.Hit);
      FStatementInsertAbil.OrderBindInt(HumData.BonusAbil.Speed);
      FStatementInsertAbil.OrderBindInt(HumData.BonusAbil.X2);
      FStatementInsertAbil.Step;
    end;

    FStatementUpdateAbilNG.Reset;
    FStatementUpdateAbilNG.OrderBindBool(HumData.boTrainingNG);
    FStatementUpdateAbilNG.OrderBindBool(HumData.boTrainingXF);
    FStatementUpdateAbilNG.OrderBindInt(HumData.AbilNG.Level);
    FStatementUpdateAbilNG.OrderBindInt(HumData.AbilNG.NH);
    FStatementUpdateAbilNG.OrderBindInt(HumData.AbilNG.MaxNH);
    FStatementUpdateAbilNG.OrderBindInt(HumData.AbilNG.Exp);
    FStatementUpdateAbilNG.OrderBindInt(HumData.AbilNG.MaxExp);
    FStatementUpdateAbilNG.OrderBindInt(HumData.ContinuousMagicOrder[0]);
    FStatementUpdateAbilNG.OrderBindInt(HumData.ContinuousMagicOrder[1]);
    FStatementUpdateAbilNG.OrderBindInt(HumData.ContinuousMagicOrder[2]);
    FStatementUpdateAbilNG.OrderBindBool(HumData.boOpenLastContinuous);
    FStatementUpdateAbilNG.OrderBindInt(HumData.btLastContinuousMagicOrder);
    for I := 0 to 4 do
    begin
      FStatementUpdateAbilNG.OrderBindInt(HumData.Meridians[I].Level);
      FStatementUpdateAbilNG.OrderBindInt(HumData.Meridians[I].BlastHitRate);
      for J := 0 to 4 do
      begin
        FStatementUpdateAbilNG.OrderBindInt(HumData.Meridians[I].Acupoints[J])
      end;
    end;
    FStatementUpdateAbilNG.OrderBindInt(HumanID);
    FStatementUpdateAbilNG.Step;

    // 更新成功行数
    if TSqliteRoleDB(Owner).FDB.GetRowsAffected = 0 then
    begin
      FStatementInsertAbilNG.Reset;
      FStatementInsertAbilNG.OrderBindInt(HumanID);
      FStatementInsertAbilNG.OrderBindBool(HumData.boTrainingNG);
      FStatementInsertAbilNG.OrderBindBool(HumData.boTrainingXF);
      FStatementInsertAbilNG.OrderBindInt(HumData.AbilNG.Level);
      FStatementInsertAbilNG.OrderBindInt(HumData.AbilNG.NH);
      FStatementInsertAbilNG.OrderBindInt(HumData.AbilNG.MaxNH);
      FStatementInsertAbilNG.OrderBindInt(HumData.AbilNG.Exp);
      FStatementInsertAbilNG.OrderBindInt(HumData.AbilNG.MaxExp);
      FStatementInsertAbilNG.OrderBindInt(HumData.ContinuousMagicOrder[0]);
      FStatementInsertAbilNG.OrderBindInt(HumData.ContinuousMagicOrder[1]);
      FStatementInsertAbilNG.OrderBindInt(HumData.ContinuousMagicOrder[2]);
      FStatementInsertAbilNG.OrderBindBool(HumData.boOpenLastContinuous);
      FStatementInsertAbilNG.OrderBindInt(HumData.btLastContinuousMagicOrder);
      for I := 0 to 4 do
      begin
        FStatementInsertAbilNG.OrderBindInt(HumData.Meridians[I].Level);
        FStatementInsertAbilNG.OrderBindInt(HumData.Meridians[I].BlastHitRate);
        for J := 0 to 4 do
        begin
          FStatementInsertAbilNG.OrderBindInt(HumData.Meridians[I].Acupoints[J])
        end;
      end;
      FStatementInsertAbilNG.Step;
    end;

    FStatementUpdateAbilWine.Reset;
    FStatementUpdateAbilWine.OrderBindBool(HumData.boPleaseDrink);
    FStatementUpdateAbilWine.OrderBindBool(HumData.boDrinkWineDrunk);
    FStatementUpdateAbilWine.OrderBindInt(HumData.nDrinkWineQuality);
    FStatementUpdateAbilWine.OrderBindInt(HumData.nDrinkWineAlcohol);
    FStatementUpdateAbilWine.OrderBindInt(HumData.Alcohol.Alcohol);
    FStatementUpdateAbilWine.OrderBindInt(HumData.Alcohol.MaxAlcohol);
    FStatementUpdateAbilWine.OrderBindInt(HumData.Alcohol.WineDrinkValue);
    FStatementUpdateAbilWine.OrderBindInt(HumData.Alcohol.MedicineLevel);
    FStatementUpdateAbilWine.OrderBindInt(HumData.Alcohol.MedicineValue);
    FStatementUpdateAbilWine.OrderBindInt(HumData.Alcohol.MaxMedicineValue);
    FStatementUpdateAbilWine.OrderBindInt(HumanID);
    FStatementUpdateAbilWine.Step;

    // 更新成功行数
    if TSqliteRoleDB(Owner).FDB.GetRowsAffected = 0 then
    begin
      FStatementInsertAbilWine.Reset;
      FStatementInsertAbilWine.OrderBindInt(HumanID);
      FStatementInsertAbilWine.OrderBindBool(HumData.boPleaseDrink);
      FStatementInsertAbilWine.OrderBindBool(HumData.boDrinkWineDrunk);
      FStatementInsertAbilWine.OrderBindInt(HumData.nDrinkWineQuality);
      FStatementInsertAbilWine.OrderBindInt(HumData.nDrinkWineAlcohol);
      FStatementInsertAbilWine.OrderBindInt(HumData.Alcohol.Alcohol);
      FStatementInsertAbilWine.OrderBindInt(HumData.Alcohol.MaxAlcohol);
      FStatementInsertAbilWine.OrderBindInt(HumData.Alcohol.WineDrinkValue);
      FStatementInsertAbilWine.OrderBindInt(HumData.Alcohol.MedicineLevel);
      FStatementInsertAbilWine.OrderBindInt(HumData.Alcohol.MedicineValue);
      FStatementInsertAbilWine.OrderBindInt(HumData.Alcohol.MaxMedicineValue);
      FStatementInsertAbilWine.Step;
    end;

    Execute('delete from HumanAbilNpcAdd where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanGamePetData where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanGodBlessState where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanMagic where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanMagicUseTick where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanStatusTime where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanQuestFlag where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanVariableT where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanVariableU where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanVariableJ where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanVariableZ where HumanID = ' + IntToStr(HumanID));

    Execute('delete from HumanItemElementAdd where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanItemAddDataByte where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanItemAddDataInt where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanItemAddDataText where HumanID = ' + IntToStr(HumanID));

    Execute('delete from HumanItemFlute where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanItemProgress where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanItemProperty where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanItemValueAdd where HumanID = ' + IntToStr(HumanID));
    Execute('delete from HumanItems where HumanID = ' + IntToStr(HumanID));

    Execute('delete from HumanSkillPower where HumanID = ' + IntToStr(HumanID));

    Execute('delete from HumanMoney where HumanID = ' + IntToStr(HumanID));

    for I := Low(HumData.AddSaveAbil) to High(HumData.AddSaveAbil) do
    begin
      if HumData.AddSaveAbil[I] <> 0 then
      begin
        FStatementInsertAbilNpcAdd.Reset;
        FStatementInsertAbilNpcAdd.OrderBindInt(HumanID);
        FStatementInsertAbilNpcAdd.OrderBindInt(I);
        FStatementInsertAbilNpcAdd.OrderBindInt(HumData.AddSaveAbil[I]);
        FStatementInsertAbilNpcAdd.Step;
      end;
    end;

    for I := Low(HumData.GamePetData) to High(HumData.GamePetData) do
    begin
      if Length(HumData.GamePetData[I].sName) > 0 then
      begin
        FStatementInsertGamePetData.Reset;
        FStatementInsertGamePetData.OrderBindInt(HumanID);
        FStatementInsertGamePetData.OrderBindInt(I);
        FStatementInsertGamePetData.OrderBindText(HumData.GamePetData[I].sName);
        FStatementInsertGamePetData.OrderBindInt(HumData.GamePetData[I].Level);
        FStatementInsertGamePetData.OrderBindInt(HumData.GamePetData[I].HP);
        FStatementInsertGamePetData.OrderBindInt(HumData.GamePetData[I].MP);
        FStatementInsertGamePetData.OrderBindInt(HumData.GamePetData[I].Exp);
        FStatementInsertGamePetData.OrderBindInt(HumData.GamePetData[I].wMagics[0]);
        FStatementInsertGamePetData.OrderBindInt(HumData.GamePetData[I].wMagics[1]);
        FStatementInsertGamePetData.OrderBindInt(HumData.GamePetData[I].wMagics[2]);
        FStatementInsertGamePetData.OrderBindInt(HumData.GamePetData[I].wMagics[3]);
        FStatementInsertGamePetData.OrderBindInt(HumData.GamePetData[I].wMagics[4]);
        FStatementInsertGamePetData.OrderBindInt(HumData.GamePetData[I].wMagics[5]);
        FStatementInsertGamePetData.OrderBindInt(HumData.GamePetData[I].wMagics[6]);
        FStatementInsertGamePetData.OrderBindInt(HumData.GamePetData[I].wMagics[7]);
        FStatementInsertGamePetData.Step;
      end;
    end;

    for I := Low(HumData.GodBlessItemsState) to High(HumData.GodBlessItemsState) do
    begin
      if HumData.GodBlessItemsState[I] <> 0 then
      begin
        FStatementInsertGodBlessState.Reset;
        FStatementInsertGodBlessState.OrderBindInt(HumanID);
        FStatementInsertGodBlessState.OrderBindInt(I);
        FStatementInsertGodBlessState.OrderBindInt(HumData.GodBlessItemsState[I]);
        FStatementInsertGodBlessState.Step;
      end;
    end;

    for I := Low(HumData.Magics) to High(HumData.Magics) do
    begin
      HumMagic := @HumData.Magics[I];
      if HumMagic.wMagIdx > 0 then
      begin
        FStatementInsertMagic.Reset;
        FStatementInsertMagic.OrderBindInt(HumanID);
        FStatementInsertMagic.OrderBindInt(1);
        FStatementInsertMagic.OrderBindInt(I);
        FStatementInsertMagic.OrderBindInt(HumMagic.wMagIdx);
        FStatementInsertMagic.OrderBindInt(Integer(HumMagic.MagicAttr));
        FStatementInsertMagic.OrderBindInt(HumMagic.btLevel);
        FStatementInsertMagic.OrderBindInt(HumMagic.btNewLevel);
        FStatementInsertMagic.OrderBindInt(HumMagic.btKey);
        FStatementInsertMagic.OrderBindInt(HumMagic.nTranPoint);
        FStatementInsertMagic.OrderBindBool(HumMagic.boUsesItemAdd);
        FStatementInsertMagic.Step;
      end;
    end;

    for I := Low(HumData.NGMagics) to High(HumData.NGMagics) do
    begin
      HumMagic := @HumData.NGMagics[I];
      if HumMagic.wMagIdx > 0 then
      begin
        FStatementInsertMagic.Reset;
        FStatementInsertMagic.OrderBindInt(HumanID);
        FStatementInsertMagic.OrderBindInt(2);
        FStatementInsertMagic.OrderBindInt(I);
        FStatementInsertMagic.OrderBindInt(HumMagic.wMagIdx);
        FStatementInsertMagic.OrderBindInt(Integer(HumMagic.MagicAttr));
        FStatementInsertMagic.OrderBindInt(HumMagic.btLevel);
        FStatementInsertMagic.OrderBindInt(HumMagic.btNewLevel);
        FStatementInsertMagic.OrderBindInt(HumMagic.btKey);
        FStatementInsertMagic.OrderBindInt(HumMagic.nTranPoint);
        FStatementInsertMagic.OrderBindInt(Integer(HumMagic.boUsesItemAdd));
        FStatementInsertMagic.Step;
      end;
    end;

    for I := Low(HumData.ContinuousMagics) to High(HumData.ContinuousMagics) do
    begin
      HumMagic := @HumData.ContinuousMagics[I];
      if HumMagic.wMagIdx > 0 then
      begin
        FStatementInsertMagic.Reset;
        FStatementInsertMagic.OrderBindInt(HumanID);
        FStatementInsertMagic.OrderBindInt(3);
        FStatementInsertMagic.OrderBindInt(I);
        FStatementInsertMagic.OrderBindInt(HumMagic.wMagIdx);
        FStatementInsertMagic.OrderBindInt(Integer(HumMagic.MagicAttr));
        FStatementInsertMagic.OrderBindInt(HumMagic.btLevel);
        FStatementInsertMagic.OrderBindInt(HumMagic.btNewLevel);
        FStatementInsertMagic.OrderBindInt(HumMagic.btKey);
        FStatementInsertMagic.OrderBindInt(HumMagic.nTranPoint);
        FStatementInsertMagic.OrderBindInt(Integer(HumMagic.boUsesItemAdd));
        FStatementInsertMagic.Step;
      end;
    end;

    for I := Low(HumData.CustomSkillUseTicks) to High(HumData.CustomSkillUseTicks) do
    begin
      if HumData.CustomSkillUseTicks[I] <> 0 then
      begin
        FStatementInsertMagicUseTick.Reset;
        FStatementInsertMagicUseTick.OrderBindInt(HumanID);
        FStatementInsertMagicUseTick.OrderBindInt(I + CUSTOM_MAGIC_START_ID);
        FStatementInsertMagicUseTick.OrderBindInt(HumData.CustomSkillUseTicks[I]);
        FStatementInsertMagicUseTick.Step;
      end;
    end;

    for I := Low(HumData.wStatusTimeArr) to High(HumData.wStatusTimeArr) do
    begin
      if HumData.wStatusTimeArr[I] <> 0 then
      begin
        FStatementInsertStatusTime.Reset;
        FStatementInsertStatusTime.OrderBindInt(HumanID);
        FStatementInsertStatusTime.OrderBindInt(I);
        FStatementInsertStatusTime.OrderBindInt(HumData.wStatusTimeArr[I]);
        FStatementInsertStatusTime.Step;
      end;
    end;

    for I := Low(HumData.QuestFlag) to High(HumData.QuestFlag) do
    begin
      if HumData.QuestFlag[I] <> 0 then
      begin
        FStatementInsertQuestFlag.Reset;
        FStatementInsertQuestFlag.OrderBindInt(HumanID);
        FStatementInsertQuestFlag.OrderBindInt(I);
        FStatementInsertQuestFlag.OrderBindInt(HumData.QuestFlag[I]);
        FStatementInsertQuestFlag.Step;
      end;
    end;

    for I := Low(HumData.UValues) to High(HumData.UValues) do
    begin
      if HumData.UValues[I] <> 0 then
      begin
        FStatementInsertVariableU.Reset;
        FStatementInsertVariableU.OrderBindInt(HumanID);
        FStatementInsertVariableU.OrderBindInt(I);
        FStatementInsertVariableU.OrderBindInt(HumData.UValues[I]);
        FStatementInsertVariableU.Step;
      end;
    end;

    for I := Low(HumData.TValues) to High(HumData.TValues) do
    begin
      if Length(HumData.TValues[I]) > 0 then
      begin
        FStatementInsertVariableT.Reset;
        FStatementInsertVariableT.OrderBindInt(HumanID);
        FStatementInsertVariableT.OrderBindInt(I);
        FStatementInsertVariableT.OrderBindText(HumData.TValues[I]);
        FStatementInsertVariableT.Step;
      end;
    end;

    for I := Low(HumData.JValues) to High(HumData.JValues) do
    begin
      if HumData.JValues[I] <> 0 then
      begin
        FStatementInsertVariableJ.Reset;
        FStatementInsertVariableJ.OrderBindInt(HumanID);
        FStatementInsertVariableJ.OrderBindInt(I);
        FStatementInsertVariableJ.OrderBindInt(HumData.JValues[I]);
        FStatementInsertVariableJ.Step;
      end;
    end;

    for I := Low(HumData.ZValues) to High(HumData.ZValues) do
    begin
      if Length(HumData.ZValues[I]) > 0 then
      begin
        FStatementInsertVariableZ.Reset;
        FStatementInsertVariableZ.OrderBindInt(HumanID);
        FStatementInsertVariableZ.OrderBindInt(I);
        FStatementInsertVariableZ.OrderBindText(HumData.ZValues[I]);
        FStatementInsertVariableZ.Step;
      end;
    end;

    for I := Low(HumData.CustomMoney) to High(HumData.CustomMoney) do
    begin
      if HumData.CustomMoney[I].sName <> '' then
      begin
        FStatementInsertCustomMoney.Reset;
        FStatementInsertCustomMoney.OrderBindInt(HumanID);
        FStatementInsertCustomMoney.OrderBindText(HumData.CustomMoney[I].sName);
        FStatementInsertCustomMoney.OrderBindInt(HumData.CustomMoney[I].nCount);
        FStatementInsertCustomMoney.Step;
      end;
    end;

    for I := Low(HumData.HumItems) to High(HumData.HumItems) do
    begin
      UserItem := @HumData.HumItems[I];
      if (UserItem.MakeIndex > 0) and (UserItem.wIndex > 0) then
      begin
        AddHumanItemToDB(UserItem, HumanID, 1, I);
      end;
    end;

    for I := Low(HumData.JewelryBoxItems) to High(HumData.JewelryBoxItems) do
    begin
      UserItem := @HumData.JewelryBoxItems[I];
      if (UserItem.MakeIndex > 0) and (UserItem.wIndex > 0) then
      begin
        AddHumanItemToDB(UserItem, HumanID, 2, I);
      end;
    end;

    for I := Low(HumData.GodBlessItems) to High(HumData.GodBlessItems) do
    begin
      UserItem := @HumData.GodBlessItems[I];
      if (UserItem.MakeIndex > 0) and (UserItem.wIndex > 0) then
      begin
        AddHumanItemToDB(UserItem, HumanID, 3, I);
      end;
    end;

    for I := Low(HumData.FengHaoItems) to High(HumData.FengHaoItems) do
    begin
      UserItem := @HumData.FengHaoItems[I];
      if (UserItem.MakeIndex > 0) and (UserItem.wIndex > 0) then
      begin
        AddHumanItemToDB(UserItem, HumanID, 4, I);
      end;
    end;

    for I := Low(HumData.BagItems) to High(HumData.BagItems) do
    begin
      UserItem := @HumData.BagItems[I];
      if (UserItem.MakeIndex > 0) and (UserItem.wIndex > 0) then
      begin
        AddHumanItemToDB(UserItem, HumanID, 5, I);
      end;
    end;

    for I := Low(HumData.StorageItems) to High(HumData.StorageItems) do
    begin
      UserItem := @HumData.StorageItems[I];
      if (UserItem.MakeIndex > 0) and (UserItem.wIndex > 0) then
      begin
        AddHumanItemToDB(UserItem, HumanID, 6, I);
      end;
    end;

    for I := Low(HumData.GamePetBagItems) to High(HumData.GamePetBagItems) do
    begin
      UserItem := @HumData.GamePetBagItems[I];
      if (UserItem.MakeIndex > 0) and (UserItem.wIndex > 0) then
      begin
        AddHumanItemToDB(UserItem, HumanID, 7, I);
      end;
    end;

    for I := Low(HumData.NpcSkillPowerAdd) to High(HumData.NpcSkillPowerAdd) do
    begin
      if (HumData.NpcSkillPowerAdd[I].HumanAttackPercent <> 0) or (HumData.NpcSkillPowerAdd[I].HumanAttackValue <> 0) or (HumData.NpcSkillPowerAdd[I].MonAttackPercent <> 0) or (HumData.NpcSkillPowerAdd[I].MonAttackValue <> 0) or (HumData.NpcSkillPowerAdd[I].DefensePercent <> 0) or (HumData.NpcSkillPowerAdd[I].DefenseValue <> 0) then
      begin
        FStatementInsertSkillPower.Reset;
        FStatementInsertSkillPower.OrderBindInt(HumanID);
        FStatementInsertSkillPower.OrderBindInt(I);
        FStatementInsertSkillPower.OrderBindInt(HumData.NpcSkillPowerAdd[I].HumanAttackPercent);
        FStatementInsertSkillPower.OrderBindInt(HumData.NpcSkillPowerAdd[I].HumanAttackValue);
        FStatementInsertSkillPower.OrderBindInt(HumData.NpcSkillPowerAdd[I].MonAttackPercent);
        FStatementInsertSkillPower.OrderBindInt(HumData.NpcSkillPowerAdd[I].MonAttackValue);
        FStatementInsertSkillPower.OrderBindInt(HumData.NpcSkillPowerAdd[I].DefensePercent);
        FStatementInsertSkillPower.OrderBindInt(HumData.NpcSkillPowerAdd[I].DefenseValue);
        FStatementInsertSkillPower.OrderBindInt(HumData.NpcSkillPowerAdd[I].RemainingTime);

        FStatementInsertSkillPower.Step;
      end;
    end;

    Result := True;
  finally
    ResetAllSaveDataStatement;
  end;
end;

procedure TSqliteHumanDB.AddHumanItemToDB(UserItem: PTUserItem; HumanID, ItemType, ItemIndex: Integer);
var
  J: Integer;
begin
  FStatementInsertItems.Reset;
  FStatementInsertItems.OrderBindInt(HumanID);
  FStatementInsertItems.OrderBindInt(ItemType);
  FStatementInsertItems.OrderBindInt(ItemIndex);
  FStatementInsertItems.OrderBindInt(UserItem.MakeIndex);
  FStatementInsertItems.OrderBindInt(UserItem.wIndex);
  FStatementInsertItems.OrderBindText(UserItem.Name);
  FStatementInsertItems.OrderBindInt(UserItem.Dura);
  FStatementInsertItems.OrderBindInt(UserItem.DuraMax);
  FStatementInsertItems.OrderBindInt(UserItem.dwHeroM2DressEffect);
  FStatementInsertItems.OrderBindInt(UserItem.btUpgradeCount);
  FStatementInsertItems.OrderBindBool(UserItem.boStartTime);
  FStatementInsertItems.OrderBindInt(UserItem.nLimitTime);
  FStatementInsertItems.OrderBindInt(UserItem.btHeroM2Light);
  FStatementInsertItems.OrderBindInt(UserItem.btColor);
  FStatementInsertItems.OrderBindBool(UserItem.boIsBind);
  FStatementInsertItems.OrderBindInt(UserItem.btBindOption);
  FStatementInsertItems.OrderBindInt(UserItem.wEffect);
  FStatementInsertItems.OrderBindInt(UserItem.wNewLooks);
  FStatementInsertItems.OrderBindInt(UserItem.wNewShape);
  FStatementInsertItems.OrderBindInt(UserItem.btFluteCount);
  FStatementInsertItems.OrderBindText(UserItem.CustomProperty.sText);
  FStatementInsertItems.OrderBindInt(UserItem.CustomProperty.btTextColor);
  FStatementInsertItems.OrderBindInt(Integer(UserItem.ItemFrom.ItemForm));
  FStatementInsertItems.OrderBindText(UserItem.ItemFrom.sMapName);
  FStatementInsertItems.OrderBindText(UserItem.ItemFrom.sMonName);
  FStatementInsertItems.OrderBindText(UserItem.ItemFrom.sMakerName);
  FStatementInsertItems.OrderBindDouble(UserItem.ItemFrom.DateTime);
  FStatementInsertItems.OrderBindInt(UserItem.wInsuranceCount);
  FStatementInsertItems.OrderBindInt(UserItem.wNewExpand3);
  FStatementInsertItems.OrderBindInt(UserItem.wNewExpand4);
  FStatementInsertItems.Step;


  //---------------------------------------------------------------------------------------------

  for J := Low(UserItem.btValue) to High(UserItem.btValue) do
  begin
    if UserItem.btValue[J] <> 0 then
    begin
      FStatementInsertItemValueAdd.Reset;
      FStatementInsertItemValueAdd.OrderBindInt(HumanID);
      FStatementInsertItemValueAdd.OrderBindInt(ItemType);
      FStatementInsertItemValueAdd.OrderBindInt(ItemIndex);
      FStatementInsertItemValueAdd.OrderBindInt(J);
      FStatementInsertItemValueAdd.OrderBindInt(UserItem.btValue[J]);
      FStatementInsertItemValueAdd.Step;
    end;
  end;

  for J := Low(UserItem.btNewValue) to High(UserItem.btNewValue) do
  begin
    if UserItem.btNewValue[J] <> 0 then
    begin
      FStatementInsertItemElementAdd.Reset;
      FStatementInsertItemElementAdd.OrderBindInt(HumanID);
      FStatementInsertItemElementAdd.OrderBindInt(ItemType);
      FStatementInsertItemElementAdd.OrderBindInt(ItemIndex);
      FStatementInsertItemElementAdd.OrderBindInt(J);
      FStatementInsertItemElementAdd.OrderBindInt(UserItem.btNewValue[J]);
      FStatementInsertItemElementAdd.Step;
    end;
  end;

  for J := Low(UserItem.btAddDataByte) to High(UserItem.btAddDataByte) do
  begin
    if UserItem.btAddDataByte[J] <> 0 then
    begin
      FStatementInsertItemAddDataByte.Reset;
      FStatementInsertItemAddDataByte.OrderBindInt(HumanID);
      FStatementInsertItemAddDataByte.OrderBindInt(ItemType);
      FStatementInsertItemAddDataByte.OrderBindInt(ItemIndex);
      FStatementInsertItemAddDataByte.OrderBindInt(J);
      FStatementInsertItemAddDataByte.OrderBindInt(UserItem.btAddDataByte[J]);
      FStatementInsertItemAddDataByte.Step;
    end;
  end;

  for J := Low(UserItem.nAddDataInt) to High(UserItem.nAddDataInt) do
  begin
    if UserItem.nAddDataInt[J] <> 0 then
    begin
      FStatementInsertItemAddDataInt.Reset;
      FStatementInsertItemAddDataInt.OrderBindInt(HumanID);
      FStatementInsertItemAddDataInt.OrderBindInt(ItemType);
      FStatementInsertItemAddDataInt.OrderBindInt(ItemIndex);
      FStatementInsertItemAddDataInt.OrderBindInt(J);
      FStatementInsertItemAddDataInt.OrderBindInt(UserItem.nAddDataInt[J]);
      FStatementInsertItemAddDataInt.Step;
    end;
  end;

  for J := Low(UserItem.sAddDataText) to High(UserItem.sAddDataText) do
  begin
    if UserItem.sAddDataText[J] <> '' then
    begin
      FStatementInsertItemAddDataText.Reset;
      FStatementInsertItemAddDataText.OrderBindInt(HumanID);
      FStatementInsertItemAddDataText.OrderBindInt(ItemType);
      FStatementInsertItemAddDataText.OrderBindInt(ItemIndex);
      FStatementInsertItemAddDataText.OrderBindInt(J);
      FStatementInsertItemAddDataText.OrderBindText(UserItem.sAddDataText[J]);
      FStatementInsertItemAddDataText.Step;
    end;
  end;

  for J := Low(UserItem.Flutes) to High(UserItem.Flutes) do
  begin
    if UserItem.Flutes[J].GemIndex <> 0 then
    begin
      FStatementInsertItemFlute.Reset;
      FStatementInsertItemFlute.OrderBindInt(HumanID);
      FStatementInsertItemFlute.OrderBindInt(ItemType);
      FStatementInsertItemFlute.OrderBindInt(ItemIndex);
      FStatementInsertItemFlute.OrderBindInt(J);
      FStatementInsertItemFlute.OrderBindInt(UserItem.Flutes[J].GemIndex);
      FStatementInsertItemFlute.OrderBindInt(UserItem.Flutes[J].GemCount);
      FStatementInsertItemFlute.Step;
    end;
  end;

  for J := Low(UserItem.Progress) to High(UserItem.Progress) do
  begin
    if UserItem.Progress[J].boOpen then
    begin
      FStatementInsertItemProgress.Reset;
      FStatementInsertItemProgress.OrderBindInt(HumanID);
      FStatementInsertItemProgress.OrderBindInt(ItemType);
      FStatementInsertItemProgress.OrderBindInt(ItemIndex);
      FStatementInsertItemProgress.OrderBindInt(J);
      FStatementInsertItemProgress.OrderBindBool(UserItem.Progress[J].boOpen);
      FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].btNameColor);
      FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].btCount);
      FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].btShowType);
      FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].wMax);
      FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].wValue);
      FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].wLevel);
      FStatementInsertItemProgress.OrderBindText(UserItem.Progress[J].sName);
      FStatementInsertItemProgress.Step;
    end;
  end;

  for J := Low(UserItem.CustomProperty.Properties) to High(UserItem.CustomProperty.Properties) do
  begin
    if (UserItem.CustomProperty.Properties[J].nValues[0] > 0) or (UserItem.CustomProperty.Properties[J].nValues[1] > 0) or (UserItem.CustomProperty.Properties[J].nValues[2] > 0) then
    begin
      FStatementInsertItemProperty.Reset;
      FStatementInsertItemProperty.OrderBindInt(HumanID);
      FStatementInsertItemProperty.OrderBindInt(ItemType);
      FStatementInsertItemProperty.OrderBindInt(ItemIndex);
      FStatementInsertItemProperty.OrderBindInt(J);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btColor);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btBindType);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btShowFlag);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btPercent);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btHintModule);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].nValues[0]);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].nValues[1]);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].nValues[2]);
      FStatementInsertItemProperty.Step;
    end;
  end;
end;

procedure TSqliteHumanDB.DoGetRankData(MinLevel, MaxLevel, TopCount: LongWord; HumanRankList, WarriorRankList, WizardRankList, TaoistRankList, MasterRankList: TRoleRankList);
var
  Ret, Index: Integer;
  RankData: TRoleRankData;
  QueryCount: Integer;
  sHumanName: string;
begin
  HumanRankList.Clear;
  WarriorRankList.Clear;
  WizardRankList.Clear;
  TaoistRankList.Clear;
  MasterRankList.Clear;

  QueryCount := TopCount + 50;

  if (MinLevel = 0) and (MaxLevel = 0) then
  begin
    try
      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindInt(0);
      FStatementGetLevelRankTopCount.OrderBindInt(1);
      FStatementGetLevelRankTopCount.OrderBindInt(2);
      FStatementGetLevelRankTopCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankTopCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankTopCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHumanName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.Level := FStatementGetLevelRankTopCount.OrderGetColumnValueInt;
          RankData.HeroName := '';

          HumanRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;
        Ret := FStatementGetLevelRankTopCount.Step;
      end;

      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindInt(0);
      FStatementGetLevelRankTopCount.OrderBindInt(0);
      FStatementGetLevelRankTopCount.OrderBindInt(0);
      FStatementGetLevelRankTopCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankTopCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankTopCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHumanName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.Level := FStatementGetLevelRankTopCount.OrderGetColumnValueInt;
          RankData.HeroName := '';

          WarriorRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;
        Ret := FStatementGetLevelRankTopCount.Step;
      end;

      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindInt(1);
      FStatementGetLevelRankTopCount.OrderBindInt(1);
      FStatementGetLevelRankTopCount.OrderBindInt(1);
      FStatementGetLevelRankTopCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankTopCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankTopCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHumanName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.Level := FStatementGetLevelRankTopCount.OrderGetColumnValueInt;
          RankData.HeroName := '';

          WizardRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;

        Ret := FStatementGetLevelRankTopCount.Step;
      end;

      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindInt(2);
      FStatementGetLevelRankTopCount.OrderBindInt(2);
      FStatementGetLevelRankTopCount.OrderBindInt(2);
      FStatementGetLevelRankTopCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankTopCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankTopCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHumanName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.Level := FStatementGetLevelRankTopCount.OrderGetColumnValueInt;
          RankData.HeroName := '';

          TaoistRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;
        Ret := FStatementGetLevelRankTopCount.Step;
      end;
    finally
      FStatementGetLevelRankTopCount.Reset;
    end;

    try
      FStatementGetMasterRankTopCount.Reset;
      FStatementGetMasterRankTopCount.OrderBindInt(0);
      FStatementGetMasterRankTopCount.OrderBindInt(1);
      FStatementGetMasterRankTopCount.OrderBindInt(2);
      FStatementGetMasterRankTopCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetMasterRankTopCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetMasterRankTopCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHumanName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.MasterCount := FStatementGetMasterRankTopCount.OrderGetColumnValueInt;
          RankData.HeroName := '';

          MasterRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;

        Ret := FStatementGetMasterRankTopCount.Step;
      end;
    finally
      FStatementGetMasterRankTopCount.Reset;
    end;
  end
  else if TopCount = 0 then
  begin
    try
      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MaxLevel);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevel.Step;
      while (Ret = SQLITE_ROW) do
      begin
        RankData.RankIndex := Index;
        RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
        RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;
        RankData.HeroName := '';

        HumanRankList.Add(@RankData);
        Inc(Index);
        Ret := FStatementGetLevelRankCheckLevel.Step;
      end;

      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MaxLevel);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevel.Step;
      while (Ret = SQLITE_ROW) do
      begin
        RankData.RankIndex := Index;
        RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
        RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;
        RankData.HeroName := '';

        WarriorRankList.Add(@RankData);
        Inc(Index);
        Ret := FStatementGetLevelRankCheckLevel.Step;
      end;

      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MaxLevel);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevel.Step;
      while (Ret = SQLITE_ROW) do
      begin
        RankData.RankIndex := Index;
        RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
        RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;
        RankData.HeroName := '';

        WizardRankList.Add(@RankData);
        Inc(Index);
        Ret := FStatementGetLevelRankCheckLevel.Step;
      end;

      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MaxLevel);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevel.Step;
      while (Ret = SQLITE_ROW) do
      begin
        RankData.RankIndex := Index;
        RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
        RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;
        RankData.HeroName := '';

        TaoistRankList.Add(@RankData);
        Inc(Index);
        Ret := FStatementGetLevelRankCheckLevel.Step;
      end;
    finally
      FStatementGetLevelRankCheckLevel.Reset;
    end;

    try
      FStatementGetMasterRankCheckLevel.Reset;
      FStatementGetMasterRankCheckLevel.OrderBindInt(0);
      FStatementGetMasterRankCheckLevel.OrderBindInt(1);
      FStatementGetMasterRankCheckLevel.OrderBindInt(2);
      FStatementGetMasterRankCheckLevel.OrderBindInt(MinLevel);
      FStatementGetMasterRankCheckLevel.OrderBindInt(MaxLevel);
      Index := 0;
      Ret := FStatementGetMasterRankCheckLevel.Step;
      while (Ret = SQLITE_ROW) do
      begin
        RankData.RankIndex := Index;
        RankData.HumanName := FStatementGetMasterRankCheckLevel.OrderGetColumnValueText;
        RankData.MasterCount := FStatementGetMasterRankCheckLevel.OrderGetColumnValueInt;
        RankData.HeroName := '';

        MasterRankList.Add(@RankData);
        Inc(Index);
        Ret := FStatementGetMasterRankCheckLevel.Step;
      end;
    finally
      FStatementGetMasterRankCheckLevel.Reset;
    end;
  end
  else
  begin
    try
      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHumanName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.Level := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueInt;
          RankData.HeroName := '';

          HumanRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;

        Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      end;

      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHumanName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.Level := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueInt;
          RankData.HeroName := '';

          WarriorRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;

        Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      end;

      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHumanName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.Level := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueInt;
          RankData.HeroName := '';

          WizardRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;

        Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      end;

      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHumanName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.Level := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueInt;
          RankData.HeroName := '';

          TaoistRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;

        Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      end;
    finally
      FStatementGetLevelRankCheckLevelAndCount.Reset;
    end;

    try
      FStatementGetMasterRankCheckLevelAndCount.Reset;
      FStatementGetMasterRankCheckLevelAndCount.OrderBindInt(0);
      FStatementGetMasterRankCheckLevelAndCount.OrderBindInt(1);
      FStatementGetMasterRankCheckLevelAndCount.OrderBindInt(2);
      FStatementGetMasterRankCheckLevelAndCount.OrderBindInt(MinLevel);
      FStatementGetMasterRankCheckLevelAndCount.OrderBindInt(MaxLevel);
      FStatementGetMasterRankCheckLevelAndCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetMasterRankCheckLevelAndCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetMasterRankCheckLevelAndCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHumanName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.MasterCount := FStatementGetMasterRankCheckLevelAndCount.OrderGetColumnValueInt;
          RankData.HeroName := '';

          MasterRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;

        Ret := FStatementGetMasterRankCheckLevelAndCount.Step;
      end;
    finally
      FStatementGetMasterRankCheckLevelAndCount.Reset;
    end;
  end;
end;

function TSqliteHumanDB.DoBuyPlayer(const sSellAccount, sSellHumanName, sBuyAccount, sBuyHumanName: string): Boolean;
begin
  FStatementBuyPlayer.Reset;
  try
    FStatementBuyPlayer.OrderBindText(sBuyAccount);
    FStatementBuyPlayer.OrderBindText(sSellAccount);
    FStatementBuyPlayer.OrderBindText(sSellHumanName);
    FStatementBuyPlayer.Step;
    Result := FStatementBuyPlayer.Step in [SQLITE_OK, SQLITE_DONE];
  finally
    FStatementBuyPlayer.Reset;
  end;
end;

procedure TSqliteHumanDB.BeginTransaction;
begin
  TSqliteRoleDB(Owner).FDB.Execute('begin transaction');
end;

procedure TSqliteHumanDB.Commit;
begin
  TSqliteRoleDB(Owner).FDB.Execute('commit transaction');
end;

procedure TSqliteHumanDB.RollBack;
begin
  TSqliteRoleDB(Owner).FDB.Execute('rollback transaction');
end;

procedure TSqliteHumanDB.Execute(Sql: string);
begin
  TSqliteRoleDB(Owner).FDB.Execute(Sql);
end;

procedure TSqliteHumanDB.ResetAllGetDataStatement;
begin
  FStatementGetHuman.Reset;
  FStatementGetAbil.Reset;
  FStatementGetAbilNG.Reset;
  FStatementGetAbilWine.Reset;
  FStatementGetAbilNpcAdd.Reset;
  FStatementGetGamePetData.Reset;
  FStatementGetGodBlessState.Reset;
  FStatementGetMagic.Reset;
  FStatementGetMagicUseTick.Reset;
  FStatementGetStatusTime.Reset;
  FStatementGetQuestFlag.Reset;
  FStatementGetVariableU.Reset;
  FStatementGetVariableT.Reset;
  FStatementGetVariableJ.Reset;
  FStatementGetVariableZ.Reset;
  FStatementGetItems.Reset;
  FStatementGetItemValueAdd.Reset;
  FStatementGetItemElementAdd.Reset;
  FStatementGetItemAddDataByte.Reset;
  FStatementGetItemAddDataInt.Reset;
  FStatementGetItemAddDataText.Reset;
  FStatementGetItemFlute.Reset;
  FStatementGetItemProgress.Reset;
  FStatementGetItemProperty.Reset;
  FStatementGetSkillPower.Reset;
  FStatementGetCustomMoney.Reset;
end;

procedure TSqliteHumanDB.ResetAllSaveDataStatement;
begin
  FStatementUpdateHuman.Reset;

  FStatementInsertAbil.Reset;
  FStatementInsertAbilNG.Reset;
  FStatementInsertAbilWine.Reset;

  FStatementUpdateAbil.Reset;
  FStatementUpdateAbilNG.Reset;
  FStatementUpdateAbilWine.Reset;

  FStatementInsertAbilNpcAdd.Reset;
  FStatementInsertGamePetData.Reset;
  FStatementInsertGodBlessState.Reset;
  FStatementInsertMagic.Reset;
  FStatementInsertMagicUseTick.Reset;
  FStatementInsertStatusTime.Reset;
  FStatementInsertQuestFlag.Reset;
  FStatementInsertVariableU.Reset;
  FStatementInsertVariableT.Reset;
  FStatementInsertVariableJ.Reset;
  FStatementInsertVariableZ.Reset;
  FStatementInsertCustomMoney.Reset;
  FStatementInsertItems.Reset;
  FStatementInsertItemValueAdd.Reset;
  FStatementInsertItemElementAdd.Reset;
  FStatementInsertItemAddDataByte.Reset;
  FStatementInsertItemAddDataInt.Reset;
  FStatementInsertItemAddDataText.Reset;
  FStatementInsertItemFlute.Reset;
  FStatementInsertItemProgress.Reset;
  FStatementInsertItemProperty.Reset;
  FStatementInsertSkillPower.Reset;
end;

{ TSqliteHeroDB }

procedure TSqliteHeroDB.DoInit;
var
  Stms: TSQLStatements;
begin
  Assert(Owner is TSqliteRoleDB, 'TSqliteHeroDB owner type error.');
  Assert(TSqliteRoleDB(Owner).FDB <> nil, 'TSqliteRoleDB.DB not create');
  Stms := TSqliteRoleDB(Owner).FDB.Statements;

  FStatementGetID := Stms.AddSQLStatement('HeroGetID');
  FStatementGetID.Sql := 'select HeroID from Hero where HeroName = ?;';

  FStatementGetHeroCount := Stms.AddSQLStatement('HeroGetCount');
  FStatementGetHeroCount.Sql := 'select count(*) from Hero where (HumanID = ?);';

  FStatementGetHumanInfo := Stms.AddSQLStatement('HeroGetHumanInfo');
  FStatementGetHumanInfo.Sql := 'select b.Account, a.HumanID, b.HumanName, b.HeroName, b.DeputyHeroName from Hero a, Human b where (a.HumanID = b.HumanID) and (a.HeroID = ?);';

  FStatementGetHumanInfo2 := Stms.AddSQLStatement('HeroGetHumanInfo2');
  FStatementGetHumanInfo2.Sql := 'select a.HumanID, b.HumanName, b.IsStorageHero, b.IsStorageDeputyHero, b.HeroName, b.DeputyHeroName from Hero a, Human b where (a.HumanID = b.HumanID) and (a.HeroID = ?);';

  FStatementSearchByAccountMatchComplete := Stms.AddSQLStatement('HeroSearchByAccount1');
  FStatementSearchByAccountMatchComplete.Sql := 'select ' + 'b.Account, ' + 'a.HeroName, ' + 'a.Sex, ' + 'a.Job, ' + 'a.Level level ' + 'from Hero a, Human b ' + 'where (a.HumanID = b.HumanID) and (b.Account = ?);';

  FStatementSearchByAccountMatchFuzzy := Stms.AddSQLStatement('HeroSearchByAccount2');
  FStatementSearchByAccountMatchFuzzy.Sql := 'select ' + 'b.Account, ' + 'a.HeroName, ' + 'a.Sex, ' + 'a.Job, ' + 'a.Level ' + 'from Hero a, Human b ' + 'where (a.HumanID = b.HumanID) and (b.Account like ?);';

  FStatementSearchByNameMatchComplete := Stms.AddSQLStatement('HeroSearchByName1');
  FStatementSearchByNameMatchComplete.Sql := 'select ' + 'b.Account, ' + 'a.HeroName, ' + 'a.Sex, ' + 'a.Job, ' + 'a.Level ' + 'from Hero a, Human b ' + 'where (a.HumanID = b.HumanID) and (a.HeroName = ?);';

  FStatementSearchByNameMatchFuzzy := Stms.AddSQLStatement('HeroSearchByName2');
  FStatementSearchByNameMatchFuzzy.Sql := 'select ' + 'b.Account, ' + 'a.HeroName, ' + 'a.Sex, ' + 'a.Job, ' + 'a.Level ' + 'from Hero a, Human b ' + 'where (a.HumanID = b.HumanID) and (a.HeroName like ?);';

  FStatementGetHero := Stms.AddSQLStatement('HeroSelect');
  FStatementGetHero.Sql := 'select ' + 'a.HeroID,' + '(select Account from Human where HumanID = a.HumanID) account,' + 'a.HeroName,' + 'a.Sex,' + 'a.Job,' + 'a.Hair,' + 'a.Status,' + 'a.Dir,' + 'a.Level,' + 'a.ReLevel,' + 'a.LoyalPoint,' + 'a.Map,' + 'a.X,' + 'a.Y,' +      //'a.HomeMap,' +
      //'a.HomeX,' +
      //'a.HomeY,' +
    'a.AttackMode,' + 'a.CreditPoint,' + 'a.PKPoint,' + 'a.IncHP,' + 'a.IncMP,' + 'a.IncHP2,' + 'a.FightZoneDieCount,' + 'a.BodyLuck,' + 'a.HungerStatus,' + 'a.RevivalTime,' + 'a.IsSaveKillMonExpRate,' + 'a.KillMonExpRate,' + 'a.KillMonExpRateTime,' + 'a.IsSavePowerRate,' + 'a.PowerRate,' + 'a.PowerRateTime,' + 'a.IsAttackMonSavePowerRate,' + 'a.AttackMonPowerRate,' + 'a.AttackMonPowerRateTime,' + 'a.IsSaveKillMonBurstRate,' + 'a.KillMonBurstRate,' + 'a.KillMonBurstRateTime,' + 'a.JewelryBoxStatus,' + 'a.IsShowFashion,' + 'a.IsShowGodBless,' + 'a.ActiveFengHao ' + 'from Hero a ' + 'where (a.HeroName = ?);';

  FStatementGetAbil := Stms.AddSQLStatement('HeroSelectAbil');
  FStatementGetAbil.Sql := 'select ' + 'AC1,' + 'AC2,' + 'MAC1,' + 'MAC2,' + 'DC1,' + 'DC2,' + 'MC1,' + 'MC2,' + 'SC1,' + 'SC2,' + 'HP,' + 'MaxHP,' + 'MP,' + 'MaxMP,' + 'Exp,' + 'MaxExp,' + 'Weight,' + 'MaxWeight,' + 'WearWeight,' + 'MaxWearWeight,' + 'HandWeight,' + 'MaxHandWeight ' + 'from HeroAbil ' + 'where HeroID = ?;';

  FStatementGetAbilNG := Stms.AddSQLStatement('HeroSelectAbilNG');
  FStatementGetAbilNG.Sql := 'select ' + 'IsTrainingNG,' + 'IsTrainingXF,' + 'AbilNGLevel,' + 'AbilNGValue,' + 'AbilNGMaxValue,' + 'AbilNGExp,' + 'AbilNGMaxExp,' + 'ContinuousMagicOrder1,' + 'ContinuousMagicOrder2,' + 'ContinuousMagicOrder3,' + 'IsOpenLastContinuous,' + 'LastContinuousMagicOrder,' + 'Meridians1Level,' + 'Meridians1BlastHitRate1,' + 'Meridians1Acupoints1,' + 'Meridians1Acupoints2,' + 'Meridians1Acupoints3,' + 'Meridians1Acupoints4,' + 'Meridians1Acupoints5,' + 'Meridians2Level,' +
    'Meridians2BlastHitRate,' + 'Meridians2Acupoints1,' + 'Meridians2Acupoints2,' + 'Meridians2Acupoints3,' + 'Meridians2Acupoints4,' + 'Meridians2Acupoints5,' + 'Meridians3Level,' + 'Meridians3BlastHitRate,' + 'Meridians3Acupoints1,' + 'Meridians3Acupoints2,' + 'Meridians3Acupoints3,' + 'Meridians3Acupoints4,' + 'Meridians3Acupoints5,' + 'Meridians4Level,' + 'Meridians4BlastHitRate,' + 'Meridians4Acupoints1,' + 'Meridians4Acupoints2,' + 'Meridians4Acupoints3,' + 'Meridians4Acupoints4,' + 'Meridians4Acupoints5,' + 'Meridians5Level,' + 'Meridians5BlastHitRate,' + 'Meridians5Acupoints1,' + 'Meridians5Acupoints2,' + 'Meridians5Acupoints3,' + 'Meridians5Acupoints4,' + 'Meridians5Acupoints5 ' + 'from HeroAbilNG ' + 'where HeroID = ?;';

  FStatementGetAbilWine := Stms.AddSQLStatement('HeroSelectAbilWine');
  FStatementGetAbilWine.Sql := 'select ' + 'IsDrinkWineDrunk,' + 'DrinkWineQuality,' + 'DrinkWineAlcohol,' + 'AbilAlcohol,' + 'AbilMaxAlcohol,' + 'AbilDrinkValue,' + 'AbilMedicineLevel,' + 'AbilMedicineValue,' + 'AbilMaxMedicineValue ' + 'from HeroAbilWine ' + 'where HeroID = ?;';

  FStatementGetAbilNpcAdd := Stms.AddSQLStatement('HeroSelectAbilNpcAdd');
  FStatementGetAbilNpcAdd.Sql := 'select ' + '"Index",' + 'Value ' + 'from HeroAbilNpcAdd ' + 'where HeroID = ?;';

  FStatementGetGodBlessState := Stms.AddSQLStatement('HeroSelectGodBlessState');
  FStatementGetGodBlessState.Sql := 'select ' + '"Index",' + 'Value ' + 'from HeroGodBlessState ' + 'where HeroID = ?;';

  FStatementGetMagic := Stms.AddSQLStatement('HeroSelectMagic');
  FStatementGetMagic.Sql := 'select ' + 'MagicType,' + 'MagicIndex,' + 'MagicID,' + 'MagicAttr,' + 'MagicLevel,' + 'MagicNewLevel,' + 'MagicKey,' + 'MagicTranPoint,' + 'MagicIsUseItemAdd ' + 'from HeroMagic ' + 'where HeroID = ?;';

  FStatementGetStatusTime := Stms.AddSQLStatement('HeroSelectStatusTime');
  FStatementGetStatusTime.Sql := 'select ' + '"Index",' + 'Value ' + 'from HeroStatusTime ' + 'where HeroID = ?;';

  FStatementGetQuestFlag := Stms.AddSQLStatement('HeroSelectQuestFlag');
  FStatementGetQuestFlag.Sql := 'select ' + '"Index",' + 'Value ' + 'from HeroQuestFlag ' + 'where HeroID = ?;';

  FStatementGetItems := Stms.AddSQLStatement('HeroSelectitems');
  FStatementGetItems.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'MakeIndex,' + 'DBIndex,' + 'Name,' + 'Dura,' + 'DuraMax,' + 'HeroM2DressEffect,' + 'UpgradeCount,' + 'IsStartTime,' + 'LimitTime,' + 'HeroM2Light,' + 'Color,' + 'IsBind,' + 'BindOption,' + 'Effect,' + 'NewLooks,' + 'NewShape,' + 'FluteCount,' + 'PropertyText,' + 'PropertyTextColor,' + 'ItemFrom,' + 'ItemFromMap,' + 'ItemFromMon,' + 'ItemFromMaker,' + 'ItemFromDate,' + 'InsuranceCount, ' + 'NewExpand3, ' + 'NewExpand4 ' + 'from HeroItems ' + 'where HeroID = ?;';

  FStatementGetItemValueAdd := Stms.AddSQLStatement('HeroSelectitemValueAdd');
  FStatementGetItemValueAdd.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Value ' + 'from HeroItemValueAdd ' + 'where HeroID = ?;';

  FStatementGetItemElementAdd := Stms.AddSQLStatement('HeroSelectitemElementAdd');
  FStatementGetItemElementAdd.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Value ' + 'from HeroItemElementAdd ' + 'where HeroID = ?;';

  FStatementGetItemAddDataByte := Stms.AddSQLStatement('HeroSelectitemAddDataByte');
  FStatementGetItemAddDataByte.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Value ' + 'from HeroItemAddDataByte ' + 'where HeroID = ?;';

  FStatementGetItemAddDataInt := Stms.AddSQLStatement('HeroSelectitemAddDataInt');
  FStatementGetItemAddDataInt.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Value ' + 'from HeroItemAddDataInt ' + 'where HeroID = ?;';

  FStatementGetItemAddDataText := Stms.AddSQLStatement('HeroSelectitemAddDataText');
  FStatementGetItemAddDataText.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Value ' + 'from HeroItemAddDataText ' + 'where HeroID = ?;';

  FStatementGetItemFlute := Stms.AddSQLStatement('HeroSelectitemFlute');
  FStatementGetItemFlute.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Value,' + 'OverlapCount ' + 'from HeroItemFlute ' + 'where HeroID = ?;';

  FStatementGetItemProgress := Stms.AddSQLStatement('HeroSelectItemProgress');
  FStatementGetItemProgress.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'IsOpen,' + 'NameColor,' + 'Count,' + 'ShowType,' + 'Max,' + 'Value,' + 'Level,' + 'Name ' + 'from HeroItemProgress ' + 'where HeroID = ?;';

  FStatementGetItemProperty := Stms.AddSQLStatement('HeroSelectItemProperty');
  FStatementGetItemProperty.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Color,' + 'BindType,' + 'ShowFlag,' + 'IsPercent,' + 'HintModule,' + 'Value,' + 'Value2,' + 'Value3 ' + 'from HeroItemProperty ' + 'where HeroID = ?;';

  FStatementGetSkillPower := Stms.AddSQLStatement('HeroSelectSkillPower');
  FStatementGetSkillPower.Sql := 'select ' + 'SkillID,' + 'HumanAttackPercent,' + 'HumanAttackValue,' + 'MonAttackPercent,' + 'MonAttackValue,' + 'DefensePercent,' + 'DefenseValue,' + 'RemainingTime  ' + 'from HeroSkillPower ' + 'where HeroID = ?;';

  FStatementAddHero := Stms.AddSQLStatement('HeroAdd');
  FStatementAddHero.Sql := 'insert into Hero(' + 'HumanID, ' + 'HeroName, ' + 'IsDelete, ' + 'CreateDate, ' + 'Sex, ' + 'Job, ' + 'Hair) ' + 'values(?, ?, ?, ?, ?, ?, ?);';

  FStatementDeleteOrRestore := Stms.AddSQLStatement('HeroDelOrRestore');
  FStatementDeleteOrRestore.Sql := 'update Hero set IsDelete = ? where (HeroName = ?);';

  FStatementUpdateHeroName := Stms.AddSQLStatement('HeroRename');
  FStatementUpdateHeroName.Sql := 'update Hero set HeroName = ? where HeroID = ?;';

  FStatementUpdateHumanHeroName := Stms.AddSQLStatement('HeroRenameSyncHumanHero');
  FStatementUpdateHumanHeroName.Sql := 'update Human set HeroName = ?, DeputyHeroName = ? where HumanID = ?;';

  FStatementUpdateHumanHeroName2 := Stms.AddSQLStatement('HeroRenameSyncHumanHero2');
  FStatementUpdateHumanHeroName2.Sql := 'update Human set IsFixedHero = ?, IsStorageHero = ?, IsStorageDeputyHero = ?, HeroName = ?, DeputyHeroName = ? where HumanID = ?;';

  FStatementUpdateHero := Stms.AddSQLStatement('HeroUpdate');
  FStatementUpdateHero.Sql := 'update Hero set ' + 'Sex = ?,' + 'Job = ?,' + 'Hair = ?,' + 'Status = ?,' + 'Dir = ?,' + 'Level = ?,' + 'ReLevel = ?,' + 'LoyalPoint = ?,' + 'Map = ?,' + 'X = ?,' + 'Y = ?,' +      //'HomeMap = ?,' +
      //'HomeX = ?,' +
      //'HomeY = ?,' +
    'AttackMode = ?,' + 'CreditPoint = ?,' + 'PKPoint = ?,' + 'IncHP = ?,' + 'IncMP = ?,' + 'IncHP2 = ?,' + 'FightZoneDieCount = ?,' + 'BodyLuck = ?,' + 'HungerStatus = ?,' + 'RevivalTime = ?,' + 'IsSaveKillMonExpRate = ?,' + 'KillMonExpRate = ?,' + 'KillMonExpRateTime = ?,' + 'IsSavePowerRate = ?,' + 'PowerRate = ?,' + 'PowerRateTime = ?,' + 'IsAttackMonSavePowerRate = ?,' + 'AttackMonPowerRate = ?,' + 'AttackMonPowerRateTime = ?,' + 'IsSaveKillMonBurstRate = ?,' + 'KillMonBurstRate = ?,' + 'KillMonBurstRateTime = ?,' + 'JewelryBoxStatus = ?,' + 'IsShowFashion = ?,' + 'IsShowGodBless = ?,' + 'ActiveFengHao = ? ' + 'where HeroID = ?;';

  FStatementInsertAbil := Stms.AddSQLStatement('HeroInsertAbil');
  FStatementInsertAbil.Sql := 'insert into HeroAbil(' + 'HeroID, ' + 'AC1, ' + 'AC2, ' + 'MAC1, ' + 'MAC2, ' + 'DC1, ' + 'DC2, ' + 'MC1, ' + 'MC2, ' + 'SC1, ' + 'SC2, ' + 'HP, ' + 'MaxHP,' + 'MP, ' + 'MaxMP, ' + 'Exp, ' + 'MaxExp, ' + 'Weight, ' + 'MaxWeight, ' + 'WearWeight, ' + 'MaxWearWeight, ' + 'HandWeight, ' + 'MaxHandWeight) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertAbilNG := Stms.AddSQLStatement('HeroInsertAbilNG');
  FStatementInsertAbilNG.Sql := 'insert into HeroAbilNG(' + 'HeroID, ' + 'IsTrainingNG, ' + 'IsTrainingXF, ' + 'AbilNGLevel, ' + 'AbilNGValue, ' + 'AbilNGMaxValue, ' + 'AbilNGExp, ' + 'AbilNGMaxExp,' + 'ContinuousMagicOrder1, ' + 'ContinuousMagicOrder2, ' + 'ContinuousMagicOrder3, ' + 'IsOpenLastContinuous, ' + 'LastContinuousMagicOrder, ' + 'Meridians1Level, ' + 'Meridians1BlastHitRate1, ' + 'Meridians1Acupoints1, ' + 'Meridians1Acupoints2, ' + 'Meridians1Acupoints3, ' + 'Meridians1Acupoints4, ' +
    'Meridians1Acupoints5, ' + 'Meridians2Level, ' + 'Meridians2BlastHitRate, ' + 'Meridians2Acupoints1, ' + 'Meridians2Acupoints2, ' + 'Meridians2Acupoints3, ' + 'Meridians2Acupoints4, ' + 'Meridians2Acupoints5, ' + 'Meridians3Level, ' + 'Meridians3BlastHitRate, ' + 'Meridians3Acupoints1, ' + 'Meridians3Acupoints2, ' + 'Meridians3Acupoints3, ' + 'Meridians3Acupoints4, ' + 'Meridians3Acupoints5, ' + 'Meridians4Level, ' + 'Meridians4BlastHitRate, ' + 'Meridians4Acupoints1, ' + 'Meridians4Acupoints2, ' +
    'Meridians4Acupoints3, ' + 'Meridians4Acupoints4, ' + 'Meridians4Acupoints5, ' + 'Meridians5Level, ' + 'Meridians5BlastHitRate, ' + 'Meridians5Acupoints1, ' + 'Meridians5Acupoints2, ' + 'Meridians5Acupoints3, ' + 'Meridians5Acupoints4, ' + 'Meridians5Acupoints5) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ' + '?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertAbilWine := Stms.AddSQLStatement('HeroInsertAbilWine');
  FStatementInsertAbilWine.Sql := 'insert into HeroAbilWine(' + 'HeroID, ' + 'IsDrinkWineDrunk, ' + 'DrinkWineQuality, ' + 'DrinkWineAlcohol, ' + 'AbilAlcohol, ' + 'AbilMaxAlcohol, ' + 'AbilDrinkValue, ' + 'AbilMedicineLevel, ' + 'AbilMedicineValue, ' + 'AbilMaxMedicineValue) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementUpdateAbil := Stms.AddSQLStatement('HeroUpdateAbil');
  FStatementUpdateAbil.Sql := 'update HeroAbil set ' + 'AC1 = ?,' + 'AC2 = ?,' + 'MAC1 = ?,' + 'MAC2 = ?,' + 'DC1 = ?,' + 'DC2 = ?,' + 'MC1 = ?,' + 'MC2 = ?,' + 'SC1 = ?,' + 'SC2 = ?,' + 'HP = ?,' + 'MaxHP = ?,' + 'MP = ?,' + 'MaxMP = ?,' + 'Exp = ?,' + 'MaxExp = ?,' + 'Weight = ?,' + 'MaxWeight = ?,' + 'WearWeight = ?,' + 'MaxWearWeight = ?,' + 'HandWeight = ?,' + 'MaxHandWeight = ? ' + 'where HeroID = ?;';

  FStatementUpdateAbilNG := Stms.AddSQLStatement('HeroUpdateAbilNG');
  FStatementUpdateAbilNG.Sql := 'update HeroAbilNG set ' + 'IsTrainingNG = ?,' + 'IsTrainingXF = ?,' + 'AbilNGLevel = ?,' + 'AbilNGValue = ?,' + 'AbilNGMaxValue = ?,' + 'AbilNGExp = ?,' + 'AbilNGMaxExp = ?,' + 'ContinuousMagicOrder1 = ?,' + 'ContinuousMagicOrder2 = ?,' + 'ContinuousMagicOrder3 = ?,' + 'IsOpenLastContinuous = ?,' + 'LastContinuousMagicOrder = ?,' + 'Meridians1Level = ?,' + 'Meridians1BlastHitRate1 = ?,' + 'Meridians1Acupoints1 = ?,' + 'Meridians1Acupoints2 = ?,' + 'Meridians1Acupoints3 = ?,'
    + 'Meridians1Acupoints4 = ?,' + 'Meridians1Acupoints5 = ?,' + 'Meridians2Level = ?,' + 'Meridians2BlastHitRate = ?,' + 'Meridians2Acupoints1 = ?,' + 'Meridians2Acupoints2 = ?,' + 'Meridians2Acupoints3 = ?,' + 'Meridians2Acupoints4 = ?,' + 'Meridians2Acupoints5 = ?,' + 'Meridians3Level = ?,' + 'Meridians3BlastHitRate = ?,' + 'Meridians3Acupoints1 = ?,' + 'Meridians3Acupoints2 = ?,' + 'Meridians3Acupoints3 = ?,' + 'Meridians3Acupoints4 = ?,' + 'Meridians3Acupoints5 = ?,' + 'Meridians4Level = ?,' +
    'Meridians4BlastHitRate = ?,' + 'Meridians4Acupoints1 = ?,' + 'Meridians4Acupoints2 = ?,' + 'Meridians4Acupoints3 = ?,' + 'Meridians4Acupoints4 = ?,' + 'Meridians4Acupoints5 = ?,' + 'Meridians5Level = ?,' + 'Meridians5BlastHitRate = ?,' + 'Meridians5Acupoints1 = ?,' + 'Meridians5Acupoints2 = ?,' + 'Meridians5Acupoints3 = ?,' + 'Meridians5Acupoints4 = ?,' + 'Meridians5Acupoints5 = ? ' + 'where HeroID = ?;';

  FStatementUpdateAbilWine := Stms.AddSQLStatement('HeroUpdateAbilWine');
  FStatementUpdateAbilWine.Sql := 'update HeroAbilWine set ' + 'IsDrinkWineDrunk = ?,' + 'DrinkWineQuality = ?,' + 'DrinkWineAlcohol = ?,' + 'AbilAlcohol = ?,' + 'AbilMaxAlcohol = ?,' + 'AbilDrinkValue = ?,' + 'AbilMedicineLevel = ?,' + 'AbilMedicineValue = ?,' + 'AbilMaxMedicineValue = ? ' + 'where HeroID = ?;';

  FStatementInsertAbilNpcAdd := Stms.AddSQLStatement('HeroInsertAbilNpcAdd');
  FStatementInsertAbilNpcAdd.Sql := 'insert into HeroAbilNpcAdd(' + 'HeroID, ' + '"Index", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertGodBlessState := Stms.AddSQLStatement('HeroInsertGodBlessState');
  FStatementInsertGodBlessState.Sql := 'insert into HeroGodBlessState(' + 'HeroID, ' + '"Index", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertMagic := Stms.AddSQLStatement('HeroInsertMagic');
  FStatementInsertMagic.Sql := 'insert into HeroMagic(' + 'HeroID, ' + 'MagicType, ' + 'MagicIndex, ' + 'MagicID,' + 'MagicAttr, ' + 'MagicLevel, ' + 'MagicNewLevel, ' + 'MagicKey, ' + 'MagicTranPoint, ' + 'MagicIsUseItemAdd) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertStatusTime := Stms.AddSQLStatement('HeroInsertStatusTime');
  FStatementInsertStatusTime.Sql := 'insert into HeroStatusTime(' + 'HeroID, ' + '"Index", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertQuestFlag := Stms.AddSQLStatement('HeroInsertQuestFlag');
  FStatementInsertQuestFlag.Sql := 'insert into HeroQuestFlag(' + 'HeroID, ' + '"Index", ' + 'Value) ' + 'values(?, ?, ?);';

  FStatementInsertItems := Stms.AddSQLStatement('HeroInsertItems');
  FStatementInsertItems.Sql := 'insert into HeroItems(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'MakeIndex, ' + 'DBIndex, ' + 'Name, ' + 'Dura, ' + 'DuraMax, ' + 'HeroM2DressEffect, ' + 'UpgradeCount, ' + 'IsStartTime, ' + 'LimitTime, ' + 'HeroM2Light, ' + 'Color, ' + 'IsBind, ' + 'BindOption, ' + 'Effect, ' + 'NewLooks, ' + 'NewShape, ' + 'FluteCount, ' + 'PropertyText, ' + 'PropertyTextColor, ' + 'ItemFrom, ' + 'ItemFromMap, ' + 'ItemFromMon, ' + 'ItemFromMaker, ' + 'ItemFromDate, ' + 'InsuranceCount, ' + 'NewExpand3, ' + 'NewExpand4 ' + ') ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ' + '?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertItemValueAdd := Stms.AddSQLStatement('HeroInsertItemValueAdd');
  FStatementInsertItemValueAdd.Sql := 'insert into HeroItemValueAdd(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemElementAdd := Stms.AddSQLStatement('HeroInsertItemElementAdd');
  FStatementInsertItemElementAdd.Sql := 'insert into HeroItemElementAdd(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemAddDataByte := Stms.AddSQLStatement('HeroInsertItemAddDataByte');
  FStatementInsertItemAddDataByte.Sql := 'insert into HeroItemAddDataByte(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemAddDataInt := Stms.AddSQLStatement('HeroInsertItemAddDataInt');
  FStatementInsertItemAddDataInt.Sql := 'insert into HeroItemAddDataInt(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemAddDataText := Stms.AddSQLStatement('HeroInsertItemAddDataText');
  FStatementInsertItemAddDataText.Sql := 'insert into HeroItemAddDataText(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemFlute := Stms.AddSQLStatement('HeroInsertItemFlute');
  FStatementInsertItemFlute.Sql := 'insert into HeroItemFlute(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Value,' + 'OverlapCount) ' + 'values(?, ?, ?, ?, ?, ?);';

  FStatementInsertItemProgress := Stms.AddSQLStatement('HeroInsertItemProgress');
  FStatementInsertItemProgress.Sql := 'insert into HeroItemProgress(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'IsOpen, ' + 'NameColor, ' + 'Count, ' + 'ShowType, ' + 'Max, ' + 'Value, ' + 'Level, ' + 'Name) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertItemProperty := Stms.AddSQLStatement('HeroInsertItemProperty');
  FStatementInsertItemProperty.Sql := 'insert into HeroItemProperty(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'Color, ' + 'BindType, ' + 'ShowFlag, ' + 'IsPercent, ' + 'HintModule, ' + 'Value, ' + 'Value2, ' + 'Value3) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertSkillPower := Stms.AddSQLStatement('HeroInsertSkillPower');
  FStatementInsertSkillPower.Sql := 'insert into HeroSkillPower(' + 'HeroID, ' + 'SkillID, ' + 'HumanAttackPercent,' + 'HumanAttackValue,' + 'MonAttackPercent,' + 'MonAttackValue,' + 'DefensePercent,' + 'DefenseValue,' + 'RemainingTime) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementGetLevelRankCheckLevel := Stms.AddSQLStatement('HeroGetLevelRankCheckLevel');
  FStatementGetLevelRankCheckLevel.Sql := 'select (select HumanName from Human where HumanID = a.HumanID) as HumanName, a.HeroName, a.Level from Hero a where (a.Job in (?,?,?)) and (a.Level >= ?) and (a.Level <= ?) order by a.Level desc';

  FStatementGetLevelRankTopCount := Stms.AddSQLStatement('HeroGetLevelRankTopCount');
  FStatementGetLevelRankTopCount.Sql := 'select (select HumanName from Human where HumanID = a.HumanID) as HumanName,a.HeroName, a.Level from Hero a where (a.Job in (?,?,?)) order by a.Level desc limit ?';

  FStatementGetLevelRankCheckLevelAndCount := Stms.AddSQLStatement('HeroGetLevelRankCheckLevelAndCount');
  FStatementGetLevelRankCheckLevelAndCount.Sql := 'select (select HumanName from Human where HumanID = a.HumanID) as HumanName, a.HeroName, a.Level from Hero a where (a.Job in (?,?,?)) and (a.Level >= ?) and (a.Level <= ?) order by a.Level desc  limit ?';

  FStatementGetID.Prepare;
  FStatementGetHeroCount.Prepare;
  FStatementGetHumanInfo.Prepare;
  FStatementGetHumanInfo2.Prepare;

  FStatementSearchByAccountMatchComplete.Prepare;
  FStatementSearchByAccountMatchFuzzy.Prepare;

  FStatementSearchByNameMatchComplete.Prepare;
  FStatementSearchByNameMatchFuzzy.Prepare;

  FStatementGetHero.Prepare;
  FStatementGetAbil.Prepare;
  FStatementGetAbilNG.Prepare;
  FStatementGetAbilWine.Prepare;
  FStatementGetAbilNpcAdd.Prepare;
  FStatementGetGodBlessState.Prepare;
  FStatementGetMagic.Prepare;
  FStatementGetStatusTime.Prepare;
  FStatementGetQuestFlag.Prepare;
  FStatementGetItems.Prepare;
  FStatementGetItemValueAdd.Prepare;
  FStatementGetItemElementAdd.Prepare;
  FStatementGetItemAddDataByte.Prepare;
  FStatementGetItemAddDataInt.Prepare;
  FStatementGetItemAddDataText.Prepare;
  FStatementGetItemFlute.Prepare;
  FStatementGetItemProgress.Prepare;
  FStatementGetItemProperty.Prepare;
  FStatementGetSkillPower.Prepare;

  FStatementAddHero.Prepare;
  FStatementDeleteOrRestore.Prepare;

  FStatementUpdateHeroName.Prepare;
  FStatementUpdateHumanHeroName.Prepare;
  FStatementUpdateHumanHeroName2.Prepare;

  FStatementUpdateHero.Prepare;

  FStatementInsertAbil.Prepare;
  FStatementInsertAbilNG.Prepare;
  FStatementInsertAbilWine.Prepare;

  FStatementUpdateAbil.Prepare;
  FStatementUpdateAbilNG.Prepare;
  FStatementUpdateAbilWine.Prepare;

  FStatementInsertAbilNpcAdd.Prepare;
  FStatementInsertGodBlessState.Prepare;
  FStatementInsertMagic.Prepare;
  FStatementInsertStatusTime.Prepare;
  FStatementInsertQuestFlag.Prepare;
  FStatementInsertItems.Prepare;
  FStatementInsertItemValueAdd.Prepare;
  FStatementInsertItemElementAdd.Prepare;
  FStatementInsertItemAddDataByte.Prepare;
  FStatementInsertItemAddDataInt.Prepare;
  FStatementInsertItemAddDataText.Prepare;
  FStatementInsertItemFlute.Prepare;
  FStatementInsertItemProgress.Prepare;
  FStatementInsertItemProperty.Prepare;
  FStatementInsertSkillPower.Prepare;

  FStatementGetLevelRankCheckLevel.Prepare;
  FStatementGetLevelRankTopCount.Prepare;
  FStatementGetLevelRankCheckLevelAndCount.Prepare;
end;

procedure TSqliteHeroDB.DoFinal;
begin
  inherited;
  FStatementGetID.Finalize;
  FStatementGetHeroCount.Finalize;
  FStatementGetHumanInfo.Finalize;
  FStatementGetHumanInfo2.Finalize;

  FStatementSearchByAccountMatchComplete.Finalize;
  FStatementSearchByAccountMatchFuzzy.Finalize;

  FStatementSearchByNameMatchComplete.Finalize;
  FStatementSearchByNameMatchFuzzy.Finalize;

  FStatementGetHero.Finalize;
  FStatementGetAbil.Finalize;
  FStatementGetAbilNG.Finalize;
  FStatementGetAbilWine.Finalize;
  FStatementGetAbilNpcAdd.Finalize;
  FStatementGetGodBlessState.Finalize;
  FStatementGetMagic.Finalize;
  FStatementGetStatusTime.Finalize;
  FStatementGetQuestFlag.Finalize;
  FStatementGetItems.Finalize;
  FStatementGetItemValueAdd.Finalize;
  FStatementGetItemElementAdd.Finalize;
  FStatementGetItemAddDataByte.Finalize;
  FStatementGetItemAddDataInt.Finalize;
  FStatementGetItemAddDataText.Finalize;
  FStatementGetItemFlute.Finalize;
  FStatementGetItemProgress.Finalize;
  FStatementGetItemProperty.Finalize;
  FStatementGetSkillPower.Finalize;

  FStatementAddHero.Finalize;
  FStatementDeleteOrRestore.Finalize;

  FStatementUpdateHeroName.Finalize;
  FStatementUpdateHumanHeroName.Finalize;
  FStatementUpdateHumanHeroName2.Finalize;

  FStatementUpdateHero.Finalize;

  FStatementInsertAbil.Finalize;
  FStatementInsertAbilNG.Finalize;
  FStatementInsertAbilWine.Finalize;

  FStatementUpdateAbil.Finalize;
  FStatementUpdateAbilNG.Finalize;
  FStatementUpdateAbilWine.Finalize;

  FStatementInsertAbilNpcAdd.Finalize;
  FStatementInsertGodBlessState.Finalize;
  FStatementInsertMagic.Finalize;
  FStatementInsertStatusTime.Finalize;
  FStatementInsertQuestFlag.Finalize;
  FStatementInsertItems.Finalize;
  FStatementInsertItemValueAdd.Finalize;
  FStatementInsertItemElementAdd.Finalize;
  FStatementInsertItemAddDataByte.Finalize;
  FStatementInsertItemAddDataInt.Finalize;
  FStatementInsertItemAddDataText.Finalize;
  FStatementInsertItemFlute.Finalize;
  FStatementInsertItemProgress.Finalize;
  FStatementInsertItemProperty.Finalize;
  FStatementInsertSkillPower.Finalize;

  FStatementGetLevelRankCheckLevel.Finalize;
  FStatementGetLevelRankTopCount.Finalize;
  FStatementGetLevelRankCheckLevelAndCount.Finalize;
end;

function TSqliteHeroDB.DoGetID(HeroName: string): Integer;
begin
  try
    FStatementGetID.Reset;
    FStatementGetID.OrderBindText(HeroName);
    if FStatementGetID.Step = SQLITE_ROW then
      Result := FStatementGetID.GetColumnValueInt(0)
    else
      Result := NO_ID;
  finally
    FStatementGetID.Reset;
  end;
end;

function TSqliteHeroDB.DoSearchByAccount(Account: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer;
var
  Ret: Integer;
  SearchData: TSerarchRoleData;
begin
  Result := 0;
  if MatchType = smtComplete then
  begin
    try
      FStatementSearchByAccountMatchComplete.Reset;
      FStatementSearchByAccountMatchComplete.OrderBindText(Account);
      Ret := FStatementSearchByAccountMatchComplete.Step;

      while (Ret = SQLITE_ROW) do
      begin
        SearchData.Account := FStatementSearchByAccountMatchComplete.OrderGetColumnValueText;
        SearchData.RoleName := FStatementSearchByAccountMatchComplete.OrderGetColumnValueText;
        SearchData.IsDelete := 0;
        SearchData.Sex := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
        SearchData.Job := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
        SearchData.Level := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
        SearchData.IsHero := True;
        RoleList.Add(@SearchData);

        Ret := FStatementSearchByAccountMatchComplete.Step;
        Inc(Result);
      end;
    finally
      FStatementSearchByAccountMatchComplete.Reset;
    end;
  end
  else
  begin
    try
      FStatementSearchByAccountMatchFuzzy.Reset;
      FStatementSearchByAccountMatchFuzzy.OrderBindText('%' + Account + '%');
      Ret := FStatementSearchByAccountMatchFuzzy.Step;

      while (Ret = SQLITE_ROW) do
      begin
        SearchData.Account := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueText;
        SearchData.RoleName := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueText;
        SearchData.IsDelete := 0;
        SearchData.Sex := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
        SearchData.Job := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
        SearchData.Level := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
        SearchData.IsHero := True;
        RoleList.Add(@SearchData);

        Ret := FStatementSearchByAccountMatchFuzzy.Step;
        Inc(Result);
      end;
    finally
      FStatementSearchByAccountMatchFuzzy.Reset;
    end;
  end;
end;

function TSqliteHeroDB.DoSearchByName(HeroName: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer;
var
  Ret: Integer;
  SearchData: TSerarchRoleData;
begin
  Result := 0;

  if MatchType = smtComplete then
  begin
    try
      FStatementSearchByNameMatchComplete.Reset;
      FStatementSearchByNameMatchComplete.OrderBindText(HeroName);
      Ret := FStatementSearchByNameMatchComplete.Step;

      while (Ret = SQLITE_ROW) do
      begin
        SearchData.Account := FStatementSearchByNameMatchComplete.OrderGetColumnValueText;
        SearchData.RoleName := FStatementSearchByNameMatchComplete.OrderGetColumnValueText;
        SearchData.IsDelete := 0;
        SearchData.Sex := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
        SearchData.Job := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
        SearchData.Level := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
        SearchData.IsHero := True;
        RoleList.Add(@SearchData);

        Ret := FStatementSearchByNameMatchComplete.Step;
        Inc(Result);
      end;
    finally
      FStatementSearchByNameMatchComplete.Reset;
    end;
  end
  else
  begin
    try
      FStatementSearchByNameMatchFuzzy.Reset;
      FStatementSearchByNameMatchFuzzy.OrderBindText('%' + HeroName + '%');
      Ret := FStatementSearchByNameMatchFuzzy.Step;

      while (Ret = SQLITE_ROW) do
      begin
        SearchData.Account := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueText;
        SearchData.RoleName := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueText;
        SearchData.IsDelete := 0;
        SearchData.Sex := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
        SearchData.Job := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
        SearchData.Level := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
        SearchData.IsHero := True;
        RoleList.Add(@SearchData);

        Ret := FStatementSearchByNameMatchFuzzy.Step;
        Inc(Result);
      end;
    finally
      FStatementSearchByNameMatchFuzzy.Reset;
    end;
  end;
end;

function TSqliteHeroDB.DoGet(HeroName: string; var HeroData: THeroData; var HeroID: Integer): Boolean;
var
  Ret: Integer;
  I, J, TheType, Index, Index2, Value: Integer;
  Magic: PTHumMagic;
  UserItem: pTUserItem;
  sTemp: string;
begin
  Result := False;

  try
    FStatementGetHero.Reset;
    FStatementGetHero.OrderBindText(HeroName);
    Ret := FStatementGetHero.Step;

    if (Ret = SQLITE_ROW) then
    begin
      Result := True;
      FillChar(HeroData, SizeOf(HeroData), 0);
      HeroID := FStatementGetHero.OrderGetColumnValueInt;                           // HeroID


      HeroData.sAccount := FStatementGetHero.OrderGetColumnValueText;               // Account
      HeroData.sChrName := FStatementGetHero.OrderGetColumnValueText;               // HeroName
      HeroData.btSex := FStatementGetHero.OrderGetColumnValueInt;                   // Sex
      HeroData.btJob := FStatementGetHero.OrderGetColumnValueInt;                   // Job
      HeroData.btHair := FStatementGetHero.OrderGetColumnValueInt;                  // Hair
      HeroData.btStatus := FStatementGetHero.OrderGetColumnValueInt;                // Status
      HeroData.btDir := FStatementGetHero.OrderGetColumnValueInt;                   // Dir
      HeroData.Abil.Level := FStatementGetHero.OrderGetColumnValueInt;              // Level
      HeroData.btReLevel := FStatementGetHero.OrderGetColumnValueInt;               // ReLevel
      HeroData.rLoyalPoint := FStatementGetHero.OrderGetColumnValueDouble;          // LoyalPoint
      HeroData.sCurMap := FStatementGetHero.OrderGetColumnValueText;                // Map
      HeroData.wCurX := FStatementGetHero.OrderGetColumnValueInt;                   // X
      HeroData.wCurY := FStatementGetHero.OrderGetColumnValueInt;                   // Y
      //HeroData.sHomeMap := FStatementGetHero.OrderGetColumnValueText;             // HomeMap
      //HeroData.wHomeX := FStatementGetHero.OrderGetColumnValueInt;                // HomeX
      //HeroData.wHomeY := FStatementGetHero.OrderGetColumnValueInt;                // HomeY
      HeroData.btAttackMode := FStatementGetHero.OrderGetColumnValueInt;            // AttackMode
      HeroData.Abil.CreditPoint := FStatementGetHero.OrderGetColumnValueInt;        // CreditPoint
      HeroData.nPKPoint := FStatementGetHero.OrderGetColumnValueInt;                // PKPoint
      HeroData.btIncHealth := FStatementGetHero.OrderGetColumnValueInt;             // IncHP
      HeroData.btIncSpell := FStatementGetHero.OrderGetColumnValueInt;              // IncMP
      HeroData.btIncHealing := FStatementGetHero.OrderGetColumnValueInt;            // IncHP2
      HeroData.btFightZoneDieCount := FStatementGetHero.OrderGetColumnValueInt;     // FightZoneDieCount
      HeroData.dBodyLuck := FStatementGetHero.OrderGetColumnValueDouble;            // BodyLuck
      HeroData.nHungerStatus := FStatementGetHero.OrderGetColumnValueInt;           // HungerStatus
      HeroData.nRevivalTime := FStatementGetHero.OrderGetColumnValueInt;            // RevivalTime
      HeroData.boSaveKillMonExpRate := FStatementGetHero.OrderGetColumnValueBool;   // IsSaveKillMonExpRate
      HeroData.nKillMonExpRate := FStatementGetHero.OrderGetColumnValueInt;         // KillMonExpRate
      HeroData.dwKillMonExpRateTime := FStatementGetHero.OrderGetColumnValueInt;    // KillMonExpRateTime
      HeroData.boAttackHumSavePowerRate := FStatementGetHero.OrderGetColumnValueBool;        // IsSavePowerRate
      HeroData.nAttackHumPowerRate := FStatementGetHero.OrderGetColumnValueInt;              // PowerRate
      HeroData.dwAttackHumPowerRateTime := FStatementGetHero.OrderGetColumnValueInt;         // PowerRateTime
      HeroData.boAttackMonSavePowerRate := FStatementGetHero.OrderGetColumnValueBool;        // IsSavePowerRate
      HeroData.nAttackMonPowerRate := FStatementGetHero.OrderGetColumnValueInt;              // PowerRate
      HeroData.dwAttackMonPowerRateTime := FStatementGetHero.OrderGetColumnValueInt;         // PowerRateTime
      HeroData.boSaveKillMonBurstRate := FStatementGetHero.OrderGetColumnValueBool; // IsSaveKillMonBurstRate
      HeroData.nKillMonBurstRate := FStatementGetHero.OrderGetColumnValueInt;       // KillMonBurstRate
      HeroData.dwKillMonBurstRateTime := FStatementGetHero.OrderGetColumnValueInt;  // KillMonBurstRateTime
      HeroData.JewelryBoxStatus := TJewelryBoxStatus(FStatementGetHero.OrderGetColumnValueInt);      // JewelryBoxStatus
      HeroData.boShowFashion := FStatementGetHero.OrderGetColumnValueBool;          // IsShowFashion
      HeroData.boShowGodBless := FStatementGetHero.OrderGetColumnValueBool;         // IsShowGodBless
      HeroData.nActiveFengHao := FStatementGetHero.OrderGetColumnValueInt;          // ActiveFengHao
    end;

    if not Result then
      Exit;

    FStatementGetAbil.Reset;
    FStatementGetAbil.OrderBindInt(HeroID);
    Ret := FStatementGetAbil.Step;
    if (Ret = SQLITE_ROW) then
    begin
      HeroData.Abil.AC1 := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.AC2 := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.MAC1 := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.MAC2 := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.DC1 := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.DC2 := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.MC1 := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.MC2 := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.SC1 := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.SC2 := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.HP := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.MaxHP := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.MP := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.MaxMP := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.Exp := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.MaxExp := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.Weight := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.MaxWeight := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.WearWeight := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.MaxWearWeight := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.HandWeight := FStatementGetAbil.OrderGetColumnValueInt;
      HeroData.Abil.MaxHandWeight := FStatementGetAbil.OrderGetColumnValueInt;
    end;

    FStatementGetAbilNG.Reset;
    FStatementGetAbilNG.OrderBindInt(HeroID);
    Ret := FStatementGetAbilNG.Step;
    if (Ret = SQLITE_ROW) then
    begin
      HeroData.boTrainingNG := FStatementGetAbilNG.OrderGetColumnValueBool;
      HeroData.boTrainingXF := FStatementGetAbilNG.OrderGetColumnValueBool;
      HeroData.AbilNG.Level := FStatementGetAbilNG.OrderGetColumnValueInt;
      HeroData.AbilNG.NH := FStatementGetAbilNG.OrderGetColumnValueInt;
      HeroData.AbilNG.MaxNH := FStatementGetAbilNG.OrderGetColumnValueInt;
      HeroData.AbilNG.Exp := FStatementGetAbilNG.OrderGetColumnValueInt;
      HeroData.AbilNG.MaxExp := FStatementGetAbilNG.OrderGetColumnValueInt;
      HeroData.ContinuousMagicOrder[0] := FStatementGetAbilNG.OrderGetColumnValueInt;
      HeroData.ContinuousMagicOrder[1] := FStatementGetAbilNG.OrderGetColumnValueInt;
      HeroData.ContinuousMagicOrder[2] := FStatementGetAbilNG.OrderGetColumnValueInt;
      HeroData.boOpenLastContinuous := FStatementGetAbilNG.OrderGetColumnValueBool;
      HeroData.btLastContinuousMagicOrder := FStatementGetAbilNG.OrderGetColumnValueInt;

      for I := 0 to 4 do
      begin
        HeroData.Meridians[I].Level := FStatementGetAbilNG.OrderGetColumnValueInt;
        HeroData.Meridians[I].BlastHitRate := FStatementGetAbilNG.OrderGetColumnValueInt;

        for J := 0 to 4 do
        begin
          HeroData.Meridians[I].Acupoints[J] := FStatementGetAbilNG.OrderGetColumnValueInt;
        end;
      end;
    end;

    FStatementGetAbilWine.Reset;
    FStatementGetAbilWine.OrderBindInt(HeroID);
    Ret := FStatementGetAbilWine.Step;
    if (Ret = SQLITE_ROW) then
    begin
      HeroData.boDrinkWineDrunk := FStatementGetAbilWine.OrderGetColumnValueBool;               //  IsDrinkWineDrunk
      HeroData.nDrinkWineQuality := FStatementGetAbilWine.OrderGetColumnValueInt;               //  DrinkWineQuality
      HeroData.nDrinkWineAlcohol := FStatementGetAbilWine.OrderGetColumnValueInt;               //  DrinkWineAlcohol
      HeroData.Alcohol.Alcohol := FStatementGetAbilWine.OrderGetColumnValueInt;                 //  AbilAlcohol
      HeroData.Alcohol.MaxAlcohol := FStatementGetAbilWine.OrderGetColumnValueInt;              //  AbilMaxAlcohol
      HeroData.Alcohol.WineDrinkValue := FStatementGetAbilWine.OrderGetColumnValueInt;          //  AbilDrinkValue
      HeroData.Alcohol.MedicineLevel := FStatementGetAbilWine.OrderGetColumnValueInt;           //  AbilMedicineLevel
      HeroData.Alcohol.MedicineValue := FStatementGetAbilWine.OrderGetColumnValueInt;           //  AbilMedicineValue
      HeroData.Alcohol.MaxMedicineValue := FStatementGetAbilWine.OrderGetColumnValueInt;        // AbilMaxMedicineValue
    end;

    FStatementGetAbilNpcAdd.Reset;
    FStatementGetAbilNpcAdd.OrderBindInt(HeroID);
    Ret := FStatementGetAbilNpcAdd.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetAbilNpcAdd.OrderGetColumnValueInt;
      Value := FStatementGetAbilNpcAdd.OrderGetColumnValueInt;

      if (Index >= Low(HeroData.AddSaveAbil)) and (Index <= High(HeroData.AddSaveAbil)) then
        HeroData.AddSaveAbil[Index] := Value;

      Ret := FStatementGetAbilNpcAdd.Step;
    end;

    FStatementGetGodBlessState.Reset;
    FStatementGetGodBlessState.OrderBindInt(HeroID);
    Ret := FStatementGetGodBlessState.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetGodBlessState.OrderGetColumnValueInt;
      Value := FStatementGetGodBlessState.OrderGetColumnValueInt;

      if (Index >= Low(HeroData.GodBlessItemsState)) and (Index <= High(HeroData.GodBlessItemsState)) then
        HeroData.GodBlessItemsState[Index] := Value;

      Ret := FStatementGetGodBlessState.Step;
    end;

    FStatementGetMagic.Reset;
    FStatementGetMagic.OrderBindInt(HeroID);
    Ret := FStatementGetMagic.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetMagic.OrderGetColumnValueInt;
      Index := FStatementGetMagic.OrderGetColumnValueInt;
      Magic := nil;
      case TheType of
        1:
          begin
            if (Index >= Low(HeroData.Magics)) and (Index <= High(HeroData.Magics)) then
              Magic := @HeroData.Magics[Index];
          end;
        2:
          begin
            if (Index >= Low(HeroData.NGMagics)) and (Index <= High(HeroData.NGMagics)) then
              Magic := @HeroData.NGMagics[Index];
          end;
        3:
          begin
            if (Index >= Low(HeroData.ContinuousMagics)) and (Index <= High(HeroData.ContinuousMagics)) then
              Magic := @HeroData.ContinuousMagics[Index];
          end;
      end;

      if Magic <> nil then
      begin
        Magic.wMagIdx := FStatementGetMagic.OrderGetColumnValueInt;
        Magic.MagicAttr := TMagicAttr(FStatementGetMagic.OrderGetColumnValueInt);
        Magic.btLevel := FStatementGetMagic.OrderGetColumnValueInt;
        Magic.btNewLevel := FStatementGetMagic.OrderGetColumnValueInt;
        Magic.btKey := FStatementGetMagic.OrderGetColumnValueInt;
        Magic.nTranPoint := FStatementGetMagic.OrderGetColumnValueInt;
        Magic.boUsesItemAdd := FStatementGetMagic.OrderGetColumnValueBool;
      end;

      Ret := FStatementGetMagic.Step;
    end;

    FStatementGetStatusTime.Reset;
    FStatementGetStatusTime.OrderBindInt(HeroID);
    Ret := FStatementGetStatusTime.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetStatusTime.OrderGetColumnValueInt;
      Value := FStatementGetStatusTime.OrderGetColumnValueInt;

      if (Index >= Low(HeroData.wStatusTimeArr)) and (Index <= High(HeroData.wStatusTimeArr)) then
        HeroData.wStatusTimeArr[Index] := Value;

      Ret := FStatementGetStatusTime.Step;
    end;

    FStatementGetQuestFlag.Reset;
    FStatementGetQuestFlag.OrderBindInt(HeroID);
    Ret := FStatementGetQuestFlag.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetQuestFlag.OrderGetColumnValueInt;
      Value := FStatementGetQuestFlag.OrderGetColumnValueInt;

      if (Index >= Low(HeroData.QuestFlag)) and (Index <= High(HeroData.QuestFlag)) then
        HeroData.QuestFlag[Index] := Value;

      Ret := FStatementGetQuestFlag.Step;
    end;

    FStatementGetItems.Reset;
    FStatementGetItems.OrderBindInt(HeroID);
    Ret := FStatementGetItems.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItems.OrderGetColumnValueInt;
      Index := FStatementGetItems.OrderGetColumnValueInt;
      UserItem := GetHeroUserItem(@HeroData, TheType, Index);
      if UserItem <> nil then
      begin
        UserItem.MakeIndex := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.wIndex := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.Name := FStatementGetItems.OrderGetColumnValueText;
        UserItem.Dura := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.DuraMax := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.dwHeroM2DressEffect := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.btUpgradeCount := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.boStartTime := FStatementGetItems.OrderGetColumnValueBool;
        UserItem.nLimitTime := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.btHeroM2Light := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.btColor := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.boIsBind := FStatementGetItems.OrderGetColumnValueBool;
        UserItem.btBindOption := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.wEffect := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.wNewLooks := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.wNewShape := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.btFluteCount := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.CustomProperty.sText := FStatementGetItems.OrderGetColumnValueText;
        UserItem.CustomProperty.btTextColor := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.ItemFrom.ItemForm := TItemFormType(FStatementGetItems.OrderGetColumnValueInt);
        UserItem.ItemFrom.sMapName := FStatementGetItems.OrderGetColumnValueText;
        UserItem.ItemFrom.sMonName := FStatementGetItems.OrderGetColumnValueText;
        UserItem.ItemFrom.sMakerName := FStatementGetItems.OrderGetColumnValueText;
        UserItem.ItemFrom.DateTime := FStatementGetItems.OrderGetColumnValueDouble;
        UserItem.wInsuranceCount := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.wNewExpand3 := FStatementGetItems.OrderGetColumnValueInt;
        UserItem.wNewExpand4 := FStatementGetItems.OrderGetColumnValueInt;
      end;

      Ret := FStatementGetItems.Step;
    end;

    FStatementGetItemValueAdd.Reset;
    FStatementGetItemValueAdd.OrderBindInt(HeroID);
    Ret := FStatementGetItemValueAdd.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemValueAdd.OrderGetColumnValueInt;
      Index := FStatementGetItemValueAdd.OrderGetColumnValueInt;
      Index2 := FStatementGetItemValueAdd.OrderGetColumnValueInt;
      UserItem := GetHeroUserItem(@HeroData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.btValue)) and (Index2 <= High(UserItem.btValue)) then
      begin
        Value := FStatementGetItemValueAdd.OrderGetColumnValueInt;
        UserItem.btValue[Index2] := Value;
      end;

      Ret := FStatementGetItemValueAdd.Step;
    end;

    FStatementGetItemElementAdd.Reset;
    FStatementGetItemElementAdd.OrderBindInt(HeroID);
    Ret := FStatementGetItemElementAdd.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemElementAdd.OrderGetColumnValueInt;
      Index := FStatementGetItemElementAdd.OrderGetColumnValueInt;
      Index2 := FStatementGetItemElementAdd.OrderGetColumnValueInt;
      UserItem := GetHeroUserItem(@HeroData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.btNewValue)) and (Index2 <= High(UserItem.btNewValue)) then
      begin
        Value := FStatementGetItemElementAdd.OrderGetColumnValueInt;
        UserItem.btNewValue[Index2] := Value;
      end;

      Ret := FStatementGetItemElementAdd.Step;
    end;

    FStatementGetItemAddDataByte.Reset;
    FStatementGetItemAddDataByte.OrderBindInt(HeroID);
    Ret := FStatementGetItemAddDataByte.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
      Index := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
      Index2 := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
      UserItem := GetHeroUserItem(@HeroData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.btAddDataByte)) and (Index2 <= High(UserItem.btAddDataByte)) then
      begin
        Value := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
        UserItem.btAddDataByte[Index2] := Value;
      end;

      Ret := FStatementGetItemAddDataByte.Step;
    end;

    FStatementGetItemAddDataInt.Reset;
    FStatementGetItemAddDataInt.OrderBindInt(HeroID);
    Ret := FStatementGetItemAddDataInt.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
      Index := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
      Index2 := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
      UserItem := GetHeroUserItem(@HeroData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.nAddDataInt)) and (Index2 <= High(UserItem.nAddDataInt)) then
      begin
        Value := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
        UserItem.nAddDataInt[Index2] := Value;
      end;

      Ret := FStatementGetItemAddDataInt.Step;
    end;

    FStatementGetItemAddDataText.Reset;
    FStatementGetItemAddDataText.OrderBindInt(HeroID);
    Ret := FStatementGetItemAddDataText.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemAddDataText.OrderGetColumnValueInt;
      Index := FStatementGetItemAddDataText.OrderGetColumnValueInt;
      Index2 := FStatementGetItemAddDataText.OrderGetColumnValueInt;
      UserItem := GetHeroUserItem(@HeroData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.sAddDataText)) and (Index2 <= High(UserItem.sAddDataText)) then
      begin
        sTemp := FStatementGetItemAddDataText.OrderGetColumnValueText;
        UserItem.sAddDataText[Index2] := sTemp;
      end;

      Ret := FStatementGetItemAddDataText.Step;
    end;

    FStatementGetItemFlute.Reset;
    FStatementGetItemFlute.OrderBindInt(HeroID);
    Ret := FStatementGetItemFlute.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemFlute.OrderGetColumnValueInt;
      Index := FStatementGetItemFlute.OrderGetColumnValueInt;
      Index2 := FStatementGetItemFlute.OrderGetColumnValueInt;
      UserItem := GetHeroUserItem(@HeroData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.Flutes)) and (Index2 <= High(UserItem.Flutes)) then
      begin
        UserItem.Flutes[Index2].GemIndex := FStatementGetItemFlute.OrderGetColumnValueInt;
        UserItem.Flutes[Index2].GemCount := FStatementGetItemFlute.OrderGetColumnValueInt;

        if (UserItem.Flutes[Index2].GemIndex > 0) and (UserItem.Flutes[Index2].GemCount = 0) then
          UserItem.Flutes[Index2].GemCount := 1;
      end;

      Ret := FStatementGetItemFlute.Step;
    end;

    FStatementGetItemProgress.Reset;
    FStatementGetItemProgress.OrderBindInt(HeroID);
    Ret := FStatementGetItemProgress.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemProgress.OrderGetColumnValueInt;
      Index := FStatementGetItemProgress.OrderGetColumnValueInt;
      Index2 := FStatementGetItemProgress.OrderGetColumnValueInt;
      UserItem := GetHeroUserItem(@HeroData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.Progress)) and (Index2 <= High(UserItem.Progress)) then
      begin
        UserItem.Progress[Index2].boOpen := FStatementGetItemProgress.OrderGetColumnValueBool;
        UserItem.Progress[Index2].btNameColor := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].btCount := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].btShowType := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].wMax := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].wValue := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].wLevel := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].sName := FStatementGetItemProgress.OrderGetColumnValueText;
      end;

      Ret := FStatementGetItemProgress.Step;
    end;

    FStatementGetItemProperty.Reset;
    FStatementGetItemProperty.OrderBindInt(HeroID);
    Ret := FStatementGetItemProperty.Step;
    while (Ret = SQLITE_ROW) do
    begin
      TheType := FStatementGetItemProperty.OrderGetColumnValueInt;
      Index := FStatementGetItemProperty.OrderGetColumnValueInt;
      Index2 := FStatementGetItemProperty.OrderGetColumnValueInt;
      UserItem := GetHeroUserItem(@HeroData, TheType, Index);
      if (UserItem <> nil) and (Index2 >= Low(UserItem.CustomProperty.Properties)) and (Index2 <= High(UserItem.CustomProperty.Properties)) then
      begin
        UserItem.CustomProperty.Properties[Index2].btColor := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btBindType := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btShowFlag := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btPercent := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btHintModule := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].nValues[0] := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].nValues[1] := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].nValues[2] := FStatementGetItemProperty.OrderGetColumnValueInt;
      end;

      Ret := FStatementGetItemProperty.Step;
    end;

    FStatementGetSkillPower.Reset;
    FStatementGetSkillPower.OrderBindInt(HeroID);
    Ret := FStatementGetSkillPower.Step;
    while (Ret = SQLITE_ROW) do
    begin
      Index := FStatementGetSkillPower.OrderGetColumnValueInt;
      if (Index >= Low(HeroData.NpcSkillPowerAdd)) and (Index <= High(HeroData.NpcSkillPowerAdd)) then
      begin
        HeroData.NpcSkillPowerAdd[Index].HumanAttackPercent := FStatementGetSkillPower.OrderGetColumnValueInt;
        HeroData.NpcSkillPowerAdd[Index].HumanAttackValue := FStatementGetSkillPower.OrderGetColumnValueInt;
        HeroData.NpcSkillPowerAdd[Index].MonAttackPercent := FStatementGetSkillPower.OrderGetColumnValueInt;
        HeroData.NpcSkillPowerAdd[Index].MonAttackValue := FStatementGetSkillPower.OrderGetColumnValueInt;
        HeroData.NpcSkillPowerAdd[Index].DefensePercent := FStatementGetSkillPower.OrderGetColumnValueInt;
        HeroData.NpcSkillPowerAdd[Index].DefenseValue := FStatementGetSkillPower.OrderGetColumnValueInt;
        HeroData.NpcSkillPowerAdd[Index].RemainingTime := FStatementGetSkillPower.OrderGetColumnValueInt;
      end;

      Ret := FStatementGetSkillPower.Step;
    end;

  finally
    ResetAllGetDataStatement;
  end;
end;

function TSqliteHeroDB.GetHeroUserItem(HeroData: PTHeroData; const ItemType, ItemIndex: Integer): PTUserItem;
begin
  Result := nil;

  case ItemType of
    1:
      begin
        if (ItemIndex >= Low(HeroData.HumItems)) and (ItemIndex <= High(HeroData.HumItems)) then
          Result := @HeroData.HumItems[ItemIndex];
      end;
    2:
      begin
        if (ItemIndex >= Low(HeroData.JewelryBoxItems)) and (ItemIndex <= High(HeroData.JewelryBoxItems)) then
          Result := @HeroData.JewelryBoxItems[ItemIndex];
      end;
    3:
      begin
        if (ItemIndex >= Low(HeroData.GodBlessItems)) and (ItemIndex <= High(HeroData.GodBlessItems)) then
          Result := @HeroData.GodBlessItems[ItemIndex];
      end;
    4:
      begin
        if (ItemIndex >= Low(HeroData.FengHaoItems)) and (ItemIndex <= High(HeroData.FengHaoItems)) then
          Result := @HeroData.FengHaoItems[ItemIndex];
      end;
    5:
      begin
        if (ItemIndex >= Low(HeroData.BagItems)) and (ItemIndex <= High(HeroData.BagItems)) then
          Result := @HeroData.BagItems[ItemIndex];
      end;
  end;
end;

function TSqliteHeroDB.DoAdd(Account, HumanName: string; HumanID: Integer; HeroName: string; Sex, Job, Hair: Byte; IsDeputyHero: Boolean): Boolean;
var
  HumanHeroName, HumanDeputyHeroName: string;
begin
  Result := False;
  Owner.HumanDB.GetHumanHeroName(Account, HumanName, HumanHeroName, HumanDeputyHeroName);
  if not IsDeputyHero then
    HumanHeroName := HeroName
  else
    HumanDeputyHeroName := HeroName;

  BeginTransaction;
  try
    FStatementAddHero.Reset;
    try
      FStatementAddHero.OrderBindInt(HumanID);                   //'HumanID, ' +
      FStatementAddHero.OrderBindText(HeroName);                 //'HeroName, ' +
      FStatementAddHero.OrderBindBool(False);                    //'IsDelete, ' +
      FStatementAddHero.OrderBindInt(Date2MyDate(Now()));        //'CreateDate, ' +
      FStatementAddHero.OrderBindInt(Sex);                       //'Sex, ' +
      FStatementAddHero.OrderBindInt(Job);                       //'Job, ' +
      FStatementAddHero.OrderBindInt(Hair);                      //'Hair) ' +
      Result := FStatementAddHero.Step in [SQLITE_OK, SQLITE_DONE];

      if Result then
      begin
        try
          FStatementUpdateHumanHeroName.Reset;
          FStatementUpdateHumanHeroName.OrderBindText(HumanHeroName);
          FStatementUpdateHumanHeroName.OrderBindText(HumanDeputyHeroName);
          FStatementUpdateHumanHeroName.OrderBindInt(HumanID);
          FStatementUpdateHumanHeroName.Step;
        finally
          FStatementUpdateHumanHeroName.Reset;
        end;
      end;
      Commit;
    finally
      FStatementAddHero.Reset;
    end;
  except
    on E: Exception do
    begin
      RollBack;
      MainOutMessage(E.Message);
    end;
  end;
end;

{
function TSqliteHeroDB.DoDelete(HeroName: string): Boolean;
begin
  try
    FStatementDeleteOrRestore.Reset;
    FStatementDeleteOrRestore.OrderBindInt(1);
    FStatementDeleteOrRestore.OrderBindText(HeroName);
    Result := FStatementDeleteOrRestore.Step in [SQLITE_OK, SQLITE_DONE];
  finally
    FStatementDeleteOrRestore.Reset;
  end;
end;

function TSqliteHeroDB.DoDeleteRestore(HeroName: string): Boolean;
begin
  try
    FStatementDeleteOrRestore.Reset;
    FStatementDeleteOrRestore.OrderBindInt(0);
    FStatementDeleteOrRestore.OrderBindText(HeroName);
    Result := FStatementDeleteOrRestore.Step in [SQLITE_OK, SQLITE_DONE];
  finally
    FStatementDeleteOrRestore.Reset;
  end;
end;

}

function TSqliteHeroDB.DoErase(HeroName: string): Boolean;
var
  HeroID: Integer;
  HumanHeroName, HumanDeputyHeroName: string;
  IsStorageHero, IsStorageDeputyHero: Boolean;
  IsChanged: Boolean;
  HumanID: Integer;
  HumanName: string;
begin
  Result := False;

  HeroID := GetID(HeroName);
  if HeroID = NO_ID then
    Exit;

  HumanID := NO_ID;
  HumanName := '';
  HumanHeroName := '';
  HumanDeputyHeroName := '';
  IsStorageHero := False;
  IsStorageDeputyHero := False;

  Result := False;
  try
    FStatementGetHumanInfo2.Reset;
    FStatementGetHumanInfo2.OrderBindInt(HeroID);
    if FStatementGetHumanInfo2.Step = SQLITE_ROW then
    begin
      Result := True;
      HumanID := FStatementGetHumanInfo2.OrderGetColumnValueInt;
      HumanName := FStatementGetHumanInfo2.OrderGetColumnValueText;
      IsStorageHero := FStatementGetHumanInfo2.OrderGetColumnValueBool;
      IsStorageDeputyHero := FStatementGetHumanInfo2.OrderGetColumnValueBool;
      HumanHeroName := FStatementGetHumanInfo2.OrderGetColumnValueText;
      HumanDeputyHeroName := FStatementGetHumanInfo2.OrderGetColumnValueText;
    end;
  finally
    FStatementGetHumanInfo2.Reset;
  end;

  IsChanged := False;
  if SameText(HumanHeroName, HeroName) then
  begin
    IsStorageHero := False;
    HumanHeroName := '';
    IsChanged := True;
  end
  else if SameText(HumanDeputyHeroName, HeroName) then
  begin
    IsStorageDeputyHero := False;
    HumanDeputyHeroName := '';
    IsChanged := True;
  end;

  BeginTransaction;
  try
    Execute(Format('delete from HeroItemElementAdd where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroItemAddDataByte where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroItemAddDataInt where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroItemAddDataText where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroItemFlute where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroItemProgress where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroItemProperty where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroItemValueAdd where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroItems where HeroID = %d;', [HeroID]));

    Execute(Format('delete from HeroAbil where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroAbilNG where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroAbilNpcAdd where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroAbilWine where HeroID = %d;', [HeroID]));

    Execute(Format('delete from HeroSkillPower where HeroID = %d;', [HeroID]));

    Execute(Format('delete from HeroGodBlessState where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroMagic where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroQuestFlag where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroStatusTime where HeroID = %d;', [HeroID]));
    Execute(Format('delete from Hero where HeroID = %d;', [HeroID]));

    if (HumanID > 0) and IsChanged then
    begin
      try
        FStatementUpdateHumanHeroName2.Reset;
        FStatementUpdateHumanHeroName2.OrderBindBool(False);
        FStatementUpdateHumanHeroName2.OrderBindBool(IsStorageHero);
        FStatementUpdateHumanHeroName2.OrderBindBool(IsStorageDeputyHero);
        FStatementUpdateHumanHeroName2.OrderBindText(HumanHeroName);
        FStatementUpdateHumanHeroName2.OrderBindText(HumanDeputyHeroName);
        FStatementUpdateHumanHeroName2.OrderBindInt(HumanID);
        FStatementUpdateHumanHeroName2.Step;
      finally
        FStatementUpdateHumanHeroName2.Reset;
      end;
    end;

    Commit;
  except
    on E: Exception do
    begin
      RollBack;
      MainOutMessage(E.Message);
    end;
  end;
end;

function TSqliteHeroDB.DoSave(HeroID: Integer; HeroData: PTHeroData): Boolean;
begin
  Result := False;
  BeginTransaction;
  try
    Result := SaveHeroData(HeroID, HeroData);
    Commit;
  except
    on E: Exception do
    begin
      RollBack;
      MainOutMessage(E.Message);
    end;
  end;
end;

function TSqliteHeroDB.DoRename(HeroID: Integer; HeroName, NewName: string): Boolean;
var
  HumanID: Integer;
  Account, HumanName: string;
  HumanHeroName, HumanDeputyHeroName: string;
  IsChanged: Boolean;
begin
  HumanID := NO_ID;
  HumanName := '';
  HumanHeroName := '';
  HumanDeputyHeroName := '';

  Result := False;
  try
    FStatementGetHumanInfo.Reset;
    FStatementGetHumanInfo.OrderBindInt(HeroID);
    if FStatementGetHumanInfo.Step = SQLITE_ROW then
    begin
      Account := FStatementGetHumanInfo.GetColumnValueText(0);
      HumanID := FStatementGetHumanInfo.GetColumnValueInt(1);
      HumanName := FStatementGetHumanInfo.GetColumnValueText(2);
      HumanHeroName := FStatementGetHumanInfo.GetColumnValueText(3);
      HumanDeputyHeroName := FStatementGetHumanInfo.GetColumnValueText(4);
    end;
  finally
    FStatementGetHumanInfo.Reset;
  end;

  IsChanged := False;
  if SameText(HumanHeroName, HeroName) then
  begin
    HumanHeroName := NewName;
    IsChanged := True;
  end
  else if SameText(HumanDeputyHeroName, HeroName) then
  begin
    HumanDeputyHeroName := NewName;
    IsChanged := True;
  end;

  BeginTransaction;
  try
    try
      FStatementUpdateHeroName.Reset;
      FStatementUpdateHeroName.OrderBindText(NewName);
      FStatementUpdateHeroName.OrderBindInt(HeroID);
      Result := FStatementUpdateHeroName.Step in [SQLITE_OK, SQLITE_DONE];

      if (Result) and (HumanID > 0) and IsChanged then
      begin
        try
          FStatementUpdateHumanHeroName.Reset;
          FStatementUpdateHumanHeroName.OrderBindText(HumanHeroName);
          FStatementUpdateHumanHeroName.OrderBindText(HumanDeputyHeroName);
          FStatementUpdateHumanHeroName.OrderBindInt(HumanID);
          FStatementUpdateHumanHeroName.Step;
        finally
          FStatementUpdateHumanHeroName.Reset;
        end;
      end;
      Commit;
    finally
      FStatementUpdateHeroName.Reset;
    end;
  except
    on E: Exception do
    begin
      RollBack;
      MainOutMessage(E.Message);
    end;
  end;
end;

function TSqliteHeroDB.DoAssess(HeroID: Integer; HeroName, DeputyHeroName: string): Boolean;
var
  HumanID: Integer;
  HumanHeroName, HumanDeputyHeroName: string;
begin
  HumanID := NO_ID;
  HumanHeroName := '';
  HumanDeputyHeroName := '';

  Result := False;
  try
    FStatementGetHumanInfo.Reset;
    FStatementGetHumanInfo.OrderBindInt(HeroID);
    if FStatementGetHumanInfo.Step = SQLITE_ROW then
    begin
      HumanID := FStatementGetHumanInfo.GetColumnValueInt(1);
      HumanHeroName := FStatementGetHumanInfo.GetColumnValueText(3);
      HumanDeputyHeroName := FStatementGetHumanInfo.GetColumnValueText(4);
    end;
  finally
    FStatementGetHumanInfo.Reset;
  end;

  if (HumanID > 0) then
  begin
    if (SameText(HumanHeroName, HeroName) and SameText(HumanDeputyHeroName, DeputyHeroName)) or (SameText(HumanHeroName, DeputyHeroName) and SameText(HumanDeputyHeroName, HeroName)) then
    begin
      try
        Result := True;
        FStatementUpdateHumanHeroName2.Reset;
        FStatementUpdateHumanHeroName2.OrderBindBool(True);
        FStatementUpdateHumanHeroName2.OrderBindBool(False);
        FStatementUpdateHumanHeroName2.OrderBindBool(False);
        FStatementUpdateHumanHeroName2.OrderBindText(HeroName);
        FStatementUpdateHumanHeroName2.OrderBindText(DeputyHeroName);
        FStatementUpdateHumanHeroName2.OrderBindInt(HumanID);
        FStatementUpdateHumanHeroName2.Step;
      finally
        FStatementUpdateHumanHeroName2.Reset;
      end;
    end;
  end;
end;

function TSqliteHeroDB.SaveHeroData(HeroID: Integer; HeroData: PTHeroData): Boolean;
var
  I, J: Integer;
  HumMagic: PTHumMagic;
  UserItem: PTUserItem;
begin
  Result := False;
  try
    FStatementUpdateHero.Reset;
    FStatementUpdateHero.OrderBindInt(HeroData.btSex);                   //  Sex;
    FStatementUpdateHero.OrderBindInt(HeroData.btJob);                   //  Job;
    FStatementUpdateHero.OrderBindInt(HeroData.btHair);                  //  Hair;
    FStatementUpdateHero.OrderBindInt(HeroData.btStatus);                //  Status;
    FStatementUpdateHero.OrderBindInt(HeroData.btDir);                   //  Dir;
    FStatementUpdateHero.OrderBindInt(HeroData.Abil.Level);              //  Level;
    FStatementUpdateHero.OrderBindInt(HeroData.btReLevel);               //  ReLevel;
    FStatementUpdateHero.OrderBindDouble(HeroData.rLoyalPoint);          //  LoyalPoint;
    FStatementUpdateHero.OrderBindText(HeroData.sCurMap);                //  Map;
    FStatementUpdateHero.OrderBindInt(HeroData.wCurX);                   //  X;
    FStatementUpdateHero.OrderBindInt(HeroData.wCurY);                   //  Y;
    //FStatementUpdateHero.OrderBindText(HeroData.sHomeMap);             //  HomeMap;
    //FStatementUpdateHero.OrderBindInt(HeroData.wHomeX);                //  HomeX;
    //FStatementUpdateHero.OrderBindInt(HeroData.wHomeY);                //  HomeY;
    FStatementUpdateHero.OrderBindInt(HeroData.btAttackMode);            //  AttackMode;
    FStatementUpdateHero.OrderBindInt(HeroData.Abil.CreditPoint);        //  CreditPoint;
    FStatementUpdateHero.OrderBindInt(HeroData.nPKPoint);                //  PKPoint;
    FStatementUpdateHero.OrderBindInt(HeroData.btIncHealth);             //  IncHP;
    FStatementUpdateHero.OrderBindInt(HeroData.btIncSpell);              //  IncMP;
    FStatementUpdateHero.OrderBindInt(HeroData.btIncHealing);            //  IncHP2;
    FStatementUpdateHero.OrderBindInt(HeroData.btFightZoneDieCount);     //  FightZoneDieCount;
    FStatementUpdateHero.OrderBindDouble(HeroData.dBodyLuck);            //  BodyLuck;
    FStatementUpdateHero.OrderBindInt(HeroData.nHungerStatus);           //  HungerStatus;
    FStatementUpdateHero.OrderBindInt(HeroData.nRevivalTime);            //  RevivalTime;
    FStatementUpdateHero.OrderBindBool(HeroData.boSaveKillMonExpRate);   //  IsSaveKillMonExpRate;
    FStatementUpdateHero.OrderBindInt(HeroData.nKillMonExpRate);         //  KillMonExpRate;
    FStatementUpdateHero.OrderBindInt(HeroData.dwKillMonExpRateTime);    //  KillMonExpRateTime;
    FStatementUpdateHero.OrderBindBool(HeroData.boAttackHumSavePowerRate);        //  IsSavePowerRate;
    FStatementUpdateHero.OrderBindInt(HeroData.nAttackHumPowerRate);              //  PowerRate;
    FStatementUpdateHero.OrderBindInt(HeroData.dwAttackHumPowerRateTime);         //  PowerRateTime;
    FStatementUpdateHero.OrderBindBool(HeroData.boAttackMonSavePowerRate);        //  IsSavePowerRate;
    FStatementUpdateHero.OrderBindInt(HeroData.nAttackMonPowerRate);              //  PowerRate;
    FStatementUpdateHero.OrderBindInt(HeroData.dwAttackMonPowerRateTime);         //  PowerRateTime;
    FStatementUpdateHero.OrderBindBool(HeroData.boSaveKillMonBurstRate); //  IsSaveKillMonBurstRate;
    FStatementUpdateHero.OrderBindInt(HeroData.nKillMonBurstRate);       //  KillMonBurstRate;
    FStatementUpdateHero.OrderBindInt(HeroData.dwKillMonBurstRateTime);  //  KillMonBurstRateTime;
    FStatementUpdateHero.OrderBindInt(Integer(HeroData.JewelryBoxStatus));      //  JewelryBoxStatus;
    FStatementUpdateHero.OrderBindBool(HeroData.boShowFashion);
    ;         //  IsShowFashion;
    FStatementUpdateHero.OrderBindBool(HeroData.boShowGodBless);         //  IsShowGodBless;
    FStatementUpdateHero.OrderBindInt(HeroData.nActiveFengHao);          //  ActiveFengHao;
    FStatementUpdateHero.OrderBindInt(HeroID);

    if not (FStatementUpdateHero.Step in [SQLITE_OK, SQLITE_DONE]) then
      Exit;

    FStatementUpdateAbil.Reset;
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.AC1);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.AC2);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.MAC1);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.MAC2);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.DC1);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.DC2);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.MC1);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.MC2);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.SC1);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.SC2);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.HP);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.MaxHP);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.MP);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.MaxMP);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.Exp);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.MaxExp);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.Weight);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.MaxWeight);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.WearWeight);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.MaxWearWeight);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.HandWeight);
    FStatementUpdateAbil.OrderBindInt(HeroData.Abil.MaxHandWeight);
    FStatementUpdateAbil.OrderBindInt(HeroID);
    FStatementUpdateAbil.Step;

    // 更新成功行数
    if TSqliteRoleDB(Owner).FDB.GetRowsAffected = 0 then
    begin
      FStatementInsertAbil.Reset;
      FStatementInsertAbil.OrderBindInt(HeroID);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.AC1);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.AC2);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.MAC1);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.MAC2);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.DC1);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.DC2);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.MC1);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.MC2);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.SC1);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.SC2);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.HP);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.MaxHP);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.MP);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.MaxMP);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.Exp);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.MaxExp);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.Weight);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.MaxWeight);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.WearWeight);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.MaxWearWeight);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.HandWeight);
      FStatementInsertAbil.OrderBindInt(HeroData.Abil.MaxHandWeight);
      FStatementInsertAbil.Step;
    end;

    FStatementUpdateAbilNG.Reset;
    FStatementUpdateAbilNG.OrderBindBool(HeroData.boTrainingNG);
    FStatementUpdateAbilNG.OrderBindBool(HeroData.boTrainingXF);
    FStatementUpdateAbilNG.OrderBindInt(HeroData.AbilNG.Level);
    FStatementUpdateAbilNG.OrderBindInt(HeroData.AbilNG.NH);
    FStatementUpdateAbilNG.OrderBindInt(HeroData.AbilNG.MaxNH);
    FStatementUpdateAbilNG.OrderBindInt(HeroData.AbilNG.Exp);
    FStatementUpdateAbilNG.OrderBindInt(HeroData.AbilNG.MaxExp);
    FStatementUpdateAbilNG.OrderBindInt(HeroData.ContinuousMagicOrder[0]);
    FStatementUpdateAbilNG.OrderBindInt(HeroData.ContinuousMagicOrder[1]);
    FStatementUpdateAbilNG.OrderBindInt(HeroData.ContinuousMagicOrder[2]);
    FStatementUpdateAbilNG.OrderBindBool(HeroData.boOpenLastContinuous);
    FStatementUpdateAbilNG.OrderBindInt(HeroData.btLastContinuousMagicOrder);
    for I := 0 to 4 do
    begin
      FStatementUpdateAbilNG.OrderBindInt(HeroData.Meridians[I].Level);
      FStatementUpdateAbilNG.OrderBindInt(HeroData.Meridians[I].BlastHitRate);
      for J := 0 to 4 do
      begin
        FStatementUpdateAbilNG.OrderBindInt(HeroData.Meridians[I].Acupoints[J])
      end;
    end;
    FStatementUpdateAbilNG.OrderBindInt(HeroID);
    FStatementUpdateAbilNG.Step;

    // 更新成功行数
    if TSqliteRoleDB(Owner).FDB.GetRowsAffected = 0 then
    begin
      FStatementInsertAbilNG.Reset;
      FStatementInsertAbilNG.OrderBindInt(HeroID);
      FStatementInsertAbilNG.OrderBindBool(HeroData.boTrainingNG);
      FStatementInsertAbilNG.OrderBindBool(HeroData.boTrainingXF);
      FStatementInsertAbilNG.OrderBindInt(HeroData.AbilNG.Level);
      FStatementInsertAbilNG.OrderBindInt(HeroData.AbilNG.NH);
      FStatementInsertAbilNG.OrderBindInt(HeroData.AbilNG.MaxNH);
      FStatementInsertAbilNG.OrderBindInt(HeroData.AbilNG.Exp);
      FStatementInsertAbilNG.OrderBindInt(HeroData.AbilNG.MaxExp);
      FStatementInsertAbilNG.OrderBindInt(HeroData.ContinuousMagicOrder[0]);
      FStatementInsertAbilNG.OrderBindInt(HeroData.ContinuousMagicOrder[1]);
      FStatementInsertAbilNG.OrderBindInt(HeroData.ContinuousMagicOrder[2]);
      FStatementInsertAbilNG.OrderBindBool(HeroData.boOpenLastContinuous);
      FStatementInsertAbilNG.OrderBindInt(HeroData.btLastContinuousMagicOrder);
      for I := 0 to 4 do
      begin
        FStatementInsertAbilNG.OrderBindInt(HeroData.Meridians[I].Level);
        FStatementInsertAbilNG.OrderBindInt(HeroData.Meridians[I].BlastHitRate);
        for J := 0 to 4 do
        begin
          FStatementInsertAbilNG.OrderBindInt(HeroData.Meridians[I].Acupoints[J])
        end;
      end;
      FStatementInsertAbilNG.Step;
    end;

    FStatementUpdateAbilWine.Reset;
    FStatementUpdateAbilWine.OrderBindBool(HeroData.boDrinkWineDrunk);
    FStatementUpdateAbilWine.OrderBindInt(HeroData.nDrinkWineQuality);
    FStatementUpdateAbilWine.OrderBindInt(HeroData.nDrinkWineAlcohol);
    FStatementUpdateAbilWine.OrderBindInt(HeroData.Alcohol.Alcohol);
    FStatementUpdateAbilWine.OrderBindInt(HeroData.Alcohol.MaxAlcohol);
    FStatementUpdateAbilWine.OrderBindInt(HeroData.Alcohol.WineDrinkValue);
    FStatementUpdateAbilWine.OrderBindInt(HeroData.Alcohol.MedicineLevel);
    FStatementUpdateAbilWine.OrderBindInt(HeroData.Alcohol.MedicineValue);
    FStatementUpdateAbilWine.OrderBindInt(HeroData.Alcohol.MaxMedicineValue);
    FStatementUpdateAbilWine.OrderBindInt(HeroID);
    FStatementUpdateAbilWine.Step;

    // 更新成功行数
    if TSqliteRoleDB(Owner).FDB.GetRowsAffected = 0 then
    begin
      FStatementInsertAbilWine.Reset;
      FStatementInsertAbilWine.OrderBindInt(HeroID);
      FStatementInsertAbilWine.OrderBindBool(HeroData.boDrinkWineDrunk);
      FStatementInsertAbilWine.OrderBindInt(HeroData.nDrinkWineQuality);
      FStatementInsertAbilWine.OrderBindInt(HeroData.nDrinkWineAlcohol);
      FStatementInsertAbilWine.OrderBindInt(HeroData.Alcohol.Alcohol);
      FStatementInsertAbilWine.OrderBindInt(HeroData.Alcohol.MaxAlcohol);
      FStatementInsertAbilWine.OrderBindInt(HeroData.Alcohol.WineDrinkValue);
      FStatementInsertAbilWine.OrderBindInt(HeroData.Alcohol.MedicineLevel);
      FStatementInsertAbilWine.OrderBindInt(HeroData.Alcohol.MedicineValue);
      FStatementInsertAbilWine.OrderBindInt(HeroData.Alcohol.MaxMedicineValue);
      FStatementInsertAbilWine.Step;
    end;

    Execute('delete from HeroAbilNpcAdd where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroGodBlessState where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroMagic where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroStatusTime where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroQuestFlag where HeroID = ' + IntToStr(HeroID));

    Execute('delete from HeroItemElementAdd where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroItemAddDataByte where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroItemAddDataInt where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroItemAddDataText where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroItemFlute where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroItemProgress where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroItemProperty where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroItemValueAdd where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroItems where HeroID = ' + IntToStr(HeroID));
    Execute('delete from HeroSkillPower where HeroID = ' + IntToStr(HeroID));

    for I := Low(HeroData.AddSaveAbil) to High(HeroData.AddSaveAbil) do
    begin
      if HeroData.AddSaveAbil[I] <> 0 then
      begin
        FStatementInsertAbilNpcAdd.Reset;
        FStatementInsertAbilNpcAdd.OrderBindInt(HeroID);
        FStatementInsertAbilNpcAdd.OrderBindInt(I);
        FStatementInsertAbilNpcAdd.OrderBindInt(HeroData.AddSaveAbil[I]);
        FStatementInsertAbilNpcAdd.Step;
      end;
    end;

    for I := Low(HeroData.GodBlessItemsState) to High(HeroData.GodBlessItemsState) do
    begin
      if HeroData.GodBlessItemsState[I] <> 0 then
      begin
        FStatementInsertGodBlessState.Reset;
        FStatementInsertGodBlessState.OrderBindInt(HeroID);
        FStatementInsertGodBlessState.OrderBindInt(I);
        FStatementInsertGodBlessState.OrderBindInt(HeroData.GodBlessItemsState[I]);
        FStatementInsertGodBlessState.Step;
      end;
    end;

    for I := Low(HeroData.Magics) to High(HeroData.Magics) do
    begin
      HumMagic := @HeroData.Magics[I];
      if HumMagic.wMagIdx > 0 then
      begin
        FStatementInsertMagic.Reset;
        FStatementInsertMagic.OrderBindInt(HeroID);
        FStatementInsertMagic.OrderBindInt(1);
        FStatementInsertMagic.OrderBindInt(I);
        FStatementInsertMagic.OrderBindInt(HumMagic.wMagIdx);
        FStatementInsertMagic.OrderBindInt(Integer(HumMagic.MagicAttr));
        FStatementInsertMagic.OrderBindInt(HumMagic.btLevel);
        FStatementInsertMagic.OrderBindInt(HumMagic.btNewLevel);
        FStatementInsertMagic.OrderBindInt(HumMagic.btKey);
        FStatementInsertMagic.OrderBindInt(HumMagic.nTranPoint);
        FStatementInsertMagic.OrderBindBool(HumMagic.boUsesItemAdd);
        FStatementInsertMagic.Step;
      end;
    end;

    for I := Low(HeroData.NGMagics) to High(HeroData.NGMagics) do
    begin
      HumMagic := @HeroData.NGMagics[I];
      if HumMagic.wMagIdx > 0 then
      begin
        FStatementInsertMagic.Reset;
        FStatementInsertMagic.OrderBindInt(HeroID);
        FStatementInsertMagic.OrderBindInt(2);
        FStatementInsertMagic.OrderBindInt(I);
        FStatementInsertMagic.OrderBindInt(HumMagic.wMagIdx);
        FStatementInsertMagic.OrderBindInt(Integer(HumMagic.MagicAttr));
        FStatementInsertMagic.OrderBindInt(HumMagic.btLevel);
        FStatementInsertMagic.OrderBindInt(HumMagic.btNewLevel);
        FStatementInsertMagic.OrderBindInt(HumMagic.btKey);
        FStatementInsertMagic.OrderBindInt(HumMagic.nTranPoint);
        FStatementInsertMagic.OrderBindInt(Integer(HumMagic.boUsesItemAdd));
        FStatementInsertMagic.Step;
      end;
    end;

    for I := Low(HeroData.ContinuousMagics) to High(HeroData.ContinuousMagics) do
    begin
      HumMagic := @HeroData.ContinuousMagics[I];
      if HumMagic.wMagIdx > 0 then
      begin
        FStatementInsertMagic.Reset;
        FStatementInsertMagic.OrderBindInt(HeroID);
        FStatementInsertMagic.OrderBindInt(3);
        FStatementInsertMagic.OrderBindInt(I);
        FStatementInsertMagic.OrderBindInt(HumMagic.wMagIdx);
        FStatementInsertMagic.OrderBindInt(Integer(HumMagic.MagicAttr));
        FStatementInsertMagic.OrderBindInt(HumMagic.btLevel);
        FStatementInsertMagic.OrderBindInt(HumMagic.btNewLevel);
        FStatementInsertMagic.OrderBindInt(HumMagic.btKey);
        FStatementInsertMagic.OrderBindInt(HumMagic.nTranPoint);
        FStatementInsertMagic.OrderBindInt(Integer(HumMagic.boUsesItemAdd));
        FStatementInsertMagic.Step;
      end;
    end;

    for I := Low(HeroData.wStatusTimeArr) to High(HeroData.wStatusTimeArr) do
    begin
      if HeroData.wStatusTimeArr[I] <> 0 then
      begin
        FStatementInsertStatusTime.Reset;
        FStatementInsertStatusTime.OrderBindInt(HeroID);
        FStatementInsertStatusTime.OrderBindInt(I);
        FStatementInsertStatusTime.OrderBindInt(HeroData.wStatusTimeArr[I]);
        FStatementInsertStatusTime.Step;
      end;
    end;

    for I := Low(HeroData.QuestFlag) to High(HeroData.QuestFlag) do
    begin
      if HeroData.QuestFlag[I] <> 0 then
      begin
        FStatementInsertQuestFlag.Reset;
        FStatementInsertQuestFlag.OrderBindInt(HeroID);
        FStatementInsertQuestFlag.OrderBindInt(I);
        FStatementInsertQuestFlag.OrderBindInt(HeroData.QuestFlag[I]);
        FStatementInsertQuestFlag.Step;
      end;
    end;

    for I := Low(HeroData.HumItems) to High(HeroData.HumItems) do
    begin
      UserItem := @HeroData.HumItems[I];
      if (UserItem.MakeIndex > 0) and (UserItem.wIndex > 0) then
      begin
        AddHeroItemToDB(UserItem, HeroID, 1, I);
      end;
    end;

    for I := Low(HeroData.JewelryBoxItems) to High(HeroData.JewelryBoxItems) do
    begin
      UserItem := @HeroData.JewelryBoxItems[I];
      if (UserItem.MakeIndex > 0) and (UserItem.wIndex > 0) then
      begin
        AddHeroItemToDB(UserItem, HeroID, 2, I);
      end;
    end;

    for I := Low(HeroData.GodBlessItems) to High(HeroData.GodBlessItems) do
    begin
      UserItem := @HeroData.GodBlessItems[I];
      if (UserItem.MakeIndex > 0) and (UserItem.wIndex > 0) then
      begin
        AddHeroItemToDB(UserItem, HeroID, 3, I);
      end;
    end;

    for I := Low(HeroData.FengHaoItems) to High(HeroData.FengHaoItems) do
    begin
      UserItem := @HeroData.FengHaoItems[I];
      if (UserItem.MakeIndex > 0) and (UserItem.wIndex > 0) then
      begin
        AddHeroItemToDB(UserItem, HeroID, 4, I);
      end;
    end;

    for I := Low(HeroData.BagItems) to High(HeroData.BagItems) do
    begin
      UserItem := @HeroData.BagItems[I];
      if (UserItem.MakeIndex > 0) and (UserItem.wIndex > 0) then
      begin
        AddHeroItemToDB(UserItem, HeroID, 5, I);
      end;
    end;

    for I := Low(HeroData.NpcSkillPowerAdd) to High(HeroData.NpcSkillPowerAdd) do
    begin
      if (HeroData.NpcSkillPowerAdd[I].HumanAttackPercent <> 0) or (HeroData.NpcSkillPowerAdd[I].HumanAttackValue <> 0) or (HeroData.NpcSkillPowerAdd[I].MonAttackPercent <> 0) or (HeroData.NpcSkillPowerAdd[I].MonAttackValue <> 0) or (HeroData.NpcSkillPowerAdd[I].DefensePercent <> 0) or (HeroData.NpcSkillPowerAdd[I].DefenseValue <> 0) then
      begin
        FStatementInsertSkillPower.Reset;
        FStatementInsertSkillPower.OrderBindInt(HeroID);
        FStatementInsertSkillPower.OrderBindInt(I);
        FStatementInsertSkillPower.OrderBindInt(HeroData.NpcSkillPowerAdd[I].HumanAttackPercent);
        FStatementInsertSkillPower.OrderBindInt(HeroData.NpcSkillPowerAdd[I].HumanAttackValue);
        FStatementInsertSkillPower.OrderBindInt(HeroData.NpcSkillPowerAdd[I].MonAttackPercent);
        FStatementInsertSkillPower.OrderBindInt(HeroData.NpcSkillPowerAdd[I].MonAttackValue);
        FStatementInsertSkillPower.OrderBindInt(HeroData.NpcSkillPowerAdd[I].DefensePercent);
        FStatementInsertSkillPower.OrderBindInt(HeroData.NpcSkillPowerAdd[I].DefenseValue);
        FStatementInsertSkillPower.OrderBindInt(HeroData.NpcSkillPowerAdd[I].RemainingTime);

        FStatementInsertSkillPower.Step;
      end;
    end;

    Result := True;
  finally
    ResetAllSaveDataStatement;
  end;
end;

procedure TSqliteHeroDB.AddHeroItemToDB(UserItem: PTUserItem; HeroID, ItemType, ItemIndex: Integer);
var
  J: Integer;
begin
  FStatementInsertItems.Reset;
  FStatementInsertItems.OrderBindInt(HeroID);
  FStatementInsertItems.OrderBindInt(ItemType);
  FStatementInsertItems.OrderBindInt(ItemIndex);
  FStatementInsertItems.OrderBindInt(UserItem.MakeIndex);
  FStatementInsertItems.OrderBindInt(UserItem.wIndex);
  FStatementInsertItems.OrderBindText(UserItem.Name);
  FStatementInsertItems.OrderBindInt(UserItem.Dura);
  FStatementInsertItems.OrderBindInt(UserItem.DuraMax);
  FStatementInsertItems.OrderBindInt(UserItem.dwHeroM2DressEffect);
  FStatementInsertItems.OrderBindInt(UserItem.btUpgradeCount);
  FStatementInsertItems.OrderBindBool(UserItem.boStartTime);
  FStatementInsertItems.OrderBindInt(UserItem.nLimitTime);
  FStatementInsertItems.OrderBindInt(UserItem.btHeroM2Light);
  FStatementInsertItems.OrderBindInt(UserItem.btColor);
  FStatementInsertItems.OrderBindBool(UserItem.boIsBind);
  FStatementInsertItems.OrderBindInt(UserItem.btBindOption);
  FStatementInsertItems.OrderBindInt(UserItem.wEffect);
  FStatementInsertItems.OrderBindInt(UserItem.wNewLooks);
  FStatementInsertItems.OrderBindInt(UserItem.wNewShape);
  FStatementInsertItems.OrderBindInt(UserItem.btFluteCount);
  FStatementInsertItems.OrderBindText(UserItem.CustomProperty.sText);
  FStatementInsertItems.OrderBindInt(UserItem.CustomProperty.btTextColor);
  FStatementInsertItems.OrderBindInt(Integer(UserItem.ItemFrom.ItemForm));
  FStatementInsertItems.OrderBindText(UserItem.ItemFrom.sMapName);
  FStatementInsertItems.OrderBindText(UserItem.ItemFrom.sMonName);
  FStatementInsertItems.OrderBindText(UserItem.ItemFrom.sMakerName);
  FStatementInsertItems.OrderBindDouble(UserItem.ItemFrom.DateTime);
  FStatementInsertItems.OrderBindInt(UserItem.wInsuranceCount);
  FStatementInsertItems.OrderBindInt(UserItem.wNewExpand3);
  FStatementInsertItems.OrderBindInt(UserItem.wNewExpand4);
  FStatementInsertItems.Step;


  //---------------------------------------------------------------------------------------------

  for J := Low(UserItem.btValue) to High(UserItem.btValue) do
  begin
    if UserItem.btValue[J] <> 0 then
    begin
      FStatementInsertItemValueAdd.Reset;
      FStatementInsertItemValueAdd.OrderBindInt(HeroID);
      FStatementInsertItemValueAdd.OrderBindInt(ItemType);
      FStatementInsertItemValueAdd.OrderBindInt(ItemIndex);
      FStatementInsertItemValueAdd.OrderBindInt(J);
      FStatementInsertItemValueAdd.OrderBindInt(UserItem.btValue[J]);
      FStatementInsertItemValueAdd.Step;
    end;
  end;

  for J := Low(UserItem.btNewValue) to High(UserItem.btNewValue) do
  begin
    if UserItem.btNewValue[J] <> 0 then
    begin
      FStatementInsertItemElementAdd.Reset;
      FStatementInsertItemElementAdd.OrderBindInt(HeroID);
      FStatementInsertItemElementAdd.OrderBindInt(ItemType);
      FStatementInsertItemElementAdd.OrderBindInt(ItemIndex);
      FStatementInsertItemElementAdd.OrderBindInt(J);
      FStatementInsertItemElementAdd.OrderBindInt(UserItem.btNewValue[J]);
      FStatementInsertItemElementAdd.Step;
    end;
  end;

  for J := Low(UserItem.btAddDataByte) to High(UserItem.btAddDataByte) do
  begin
    if UserItem.btAddDataByte[J] <> 0 then
    begin
      FStatementInsertItemAddDataByte.Reset;
      FStatementInsertItemAddDataByte.OrderBindInt(HeroID);
      FStatementInsertItemAddDataByte.OrderBindInt(ItemType);
      FStatementInsertItemAddDataByte.OrderBindInt(ItemIndex);
      FStatementInsertItemAddDataByte.OrderBindInt(J);
      FStatementInsertItemAddDataByte.OrderBindInt(UserItem.btAddDataByte[J]);
      FStatementInsertItemAddDataByte.Step;
    end;
  end;

  for J := Low(UserItem.nAddDataInt) to High(UserItem.nAddDataInt) do
  begin
    if UserItem.nAddDataInt[J] <> 0 then
    begin
      FStatementInsertItemAddDataInt.Reset;
      FStatementInsertItemAddDataInt.OrderBindInt(HeroID);
      FStatementInsertItemAddDataInt.OrderBindInt(ItemType);
      FStatementInsertItemAddDataInt.OrderBindInt(ItemIndex);
      FStatementInsertItemAddDataInt.OrderBindInt(J);
      FStatementInsertItemAddDataInt.OrderBindInt(UserItem.nAddDataInt[J]);
      FStatementInsertItemAddDataInt.Step;
    end;
  end;

  for J := Low(UserItem.sAddDataText) to High(UserItem.sAddDataText) do
  begin
    if UserItem.sAddDataText[J] <> '' then
    begin
      FStatementInsertItemAddDataText.Reset;
      FStatementInsertItemAddDataText.OrderBindInt(HeroID);
      FStatementInsertItemAddDataText.OrderBindInt(ItemType);
      FStatementInsertItemAddDataText.OrderBindInt(ItemIndex);
      FStatementInsertItemAddDataText.OrderBindInt(J);
      FStatementInsertItemAddDataText.OrderBindText(UserItem.sAddDataText[J]);
      FStatementInsertItemAddDataText.Step;
    end;
  end;

  for J := Low(UserItem.Flutes) to High(UserItem.Flutes) do
  begin
    if UserItem.Flutes[J].GemIndex <> 0 then
    begin
      FStatementInsertItemFlute.Reset;
      FStatementInsertItemFlute.OrderBindInt(HeroID);
      FStatementInsertItemFlute.OrderBindInt(ItemType);
      FStatementInsertItemFlute.OrderBindInt(ItemIndex);
      FStatementInsertItemFlute.OrderBindInt(J);
      FStatementInsertItemFlute.OrderBindInt(UserItem.Flutes[J].GemIndex);
      FStatementInsertItemFlute.OrderBindInt(UserItem.Flutes[J].GemCount);
      FStatementInsertItemFlute.Step;
    end;
  end;

  for J := Low(UserItem.Progress) to High(UserItem.Progress) do
  begin
    if UserItem.Progress[J].boOpen then
    begin
      FStatementInsertItemProgress.Reset;
      FStatementInsertItemProgress.OrderBindInt(HeroID);
      FStatementInsertItemProgress.OrderBindInt(ItemType);
      FStatementInsertItemProgress.OrderBindInt(ItemIndex);
      FStatementInsertItemProgress.OrderBindInt(J);
      FStatementInsertItemProgress.OrderBindBool(UserItem.Progress[J].boOpen);
      FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].btNameColor);
      FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].btCount);
      FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].btShowType);
      FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].wMax);
      FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].wValue);
      FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].wLevel);
      FStatementInsertItemProgress.OrderBindText(UserItem.Progress[J].sName);
      FStatementInsertItemProgress.Step;
    end;
  end;

  for J := Low(UserItem.CustomProperty.Properties) to High(UserItem.CustomProperty.Properties) do
  begin
    if (UserItem.CustomProperty.Properties[J].nValues[0] > 0) or (UserItem.CustomProperty.Properties[J].nValues[1] > 0) or (UserItem.CustomProperty.Properties[J].nValues[2] > 0) then
    begin
      FStatementInsertItemProperty.Reset;
      FStatementInsertItemProperty.OrderBindInt(HeroID);
      FStatementInsertItemProperty.OrderBindInt(ItemType);
      FStatementInsertItemProperty.OrderBindInt(ItemIndex);
      FStatementInsertItemProperty.OrderBindInt(J);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btColor);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btBindType);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btShowFlag);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btPercent);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btHintModule);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].nValues[0]);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].nValues[1]);
      FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].nValues[2]);
      FStatementInsertItemProperty.Step;
    end;
  end;
end;

procedure TSqliteHeroDB.DoGetRankData(MinLevel, MaxLevel, TopCount: LongWord; HeroRankList, WarriorRankList, WizardRankList, TaoistRankList: TRoleRankList);
var
  Ret, Index: Integer;
  RankData: TRoleRankData;
  QueryCount: Integer;
  sHumanName, sHeroName: string;
begin
  HeroRankList.Clear;
  WarriorRankList.Clear;
  WizardRankList.Clear;
  TaoistRankList.Clear;

  QueryCount := TopCount + 50;

  if (MinLevel = 0) and (MaxLevel = 0) then
  begin
    try
      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindInt(0);
      FStatementGetLevelRankTopCount.OrderBindInt(1);
      FStatementGetLevelRankTopCount.OrderBindInt(2);
      FStatementGetLevelRankTopCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankTopCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankTopCount.OrderGetColumnValueText;
        sHeroName := FStatementGetLevelRankTopCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHeroName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.HeroName := sHeroName;
          RankData.Level := FStatementGetLevelRankTopCount.OrderGetColumnValueInt;

          HeroRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;

        Ret := FStatementGetLevelRankTopCount.Step;
      end;

      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindInt(0);
      FStatementGetLevelRankTopCount.OrderBindInt(0);
      FStatementGetLevelRankTopCount.OrderBindInt(0);
      FStatementGetLevelRankTopCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankTopCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankTopCount.OrderGetColumnValueText;
        sHeroName := FStatementGetLevelRankTopCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHeroName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.HeroName := sHeroName;
          RankData.Level := FStatementGetLevelRankTopCount.OrderGetColumnValueInt;

          WarriorRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;

        Ret := FStatementGetLevelRankTopCount.Step;
      end;

      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindInt(1);
      FStatementGetLevelRankTopCount.OrderBindInt(1);
      FStatementGetLevelRankTopCount.OrderBindInt(1);
      FStatementGetLevelRankTopCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankTopCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankTopCount.OrderGetColumnValueText;
        sHeroName := FStatementGetLevelRankTopCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHeroName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.HeroName := sHeroName;
          RankData.Level := FStatementGetLevelRankTopCount.OrderGetColumnValueInt;

          WizardRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;
        Ret := FStatementGetLevelRankTopCount.Step;
      end;

      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindInt(2);
      FStatementGetLevelRankTopCount.OrderBindInt(2);
      FStatementGetLevelRankTopCount.OrderBindInt(2);
      FStatementGetLevelRankTopCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankTopCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankTopCount.OrderGetColumnValueText;
        sHeroName := FStatementGetLevelRankTopCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHeroName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.HeroName := sHeroName;
          RankData.Level := FStatementGetLevelRankTopCount.OrderGetColumnValueInt;

          TaoistRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;
        Ret := FStatementGetLevelRankTopCount.Step;
      end;
    finally
      FStatementGetLevelRankTopCount.Reset;
    end;
  end
  else if TopCount = 0 then
  begin
    try
      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MaxLevel);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevel.Step;
      while (Ret = SQLITE_ROW) do
      begin
        RankData.RankIndex := Index;
        RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
        RankData.HeroName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
        RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;

        HeroRankList.Add(@RankData);
        Inc(Index);
        Ret := FStatementGetLevelRankCheckLevel.Step;
      end;

      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MaxLevel);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevel.Step;
      while (Ret = SQLITE_ROW) do
      begin
        RankData.RankIndex := Index;
        RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
        RankData.HeroName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
        RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;

        WarriorRankList.Add(@RankData);
        Inc(Index);
        Ret := FStatementGetLevelRankCheckLevel.Step;
      end;

      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MaxLevel);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevel.Step;
      while (Ret = SQLITE_ROW) do
      begin
        RankData.RankIndex := Index;
        RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
        RankData.HeroName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
        RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;

        WizardRankList.Add(@RankData);
        Inc(Index);
        Ret := FStatementGetLevelRankCheckLevel.Step;
      end;

      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindInt(MaxLevel);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevel.Step;
      while (Ret = SQLITE_ROW) do
      begin
        RankData.RankIndex := Index;
        RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
        RankData.HeroName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
        RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;

        TaoistRankList.Add(@RankData);
        Inc(Index);
        Ret := FStatementGetLevelRankCheckLevel.Step;
      end;
    finally
      FStatementGetLevelRankCheckLevel.Reset;
    end;
  end
  else
  begin
    try
      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueText;
        sHeroName := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHeroName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.HeroName := sHeroName;
          RankData.Level := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueInt;

          HeroRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;

        Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      end;

      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueText;
        sHeroName := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHeroName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.HeroName := sHeroName;
          RankData.Level := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueInt;

          WarriorRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;
        Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      end;

      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueText;
        sHeroName := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHeroName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.HeroName := sHeroName;
          RankData.Level := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueInt;

          WizardRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;
        Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      end;

      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindInt(QueryCount);
      Index := 0;
      Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      while (Ret = SQLITE_ROW) do
      begin
        sHumanName := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueText;
        sHeroName := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueText;

        if not CheckFilterRankingChrName(sHeroName) then
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := sHumanName;
          RankData.HeroName := sHeroName;
          RankData.Level := FStatementGetLevelRankCheckLevelAndCount.OrderGetColumnValueInt;

          TaoistRankList.Add(@RankData);
          Inc(Index);

          if Index >= TopCount then
            Break;
        end;

        Ret := FStatementGetLevelRankCheckLevelAndCount.Step;
      end;
    finally
      FStatementGetLevelRankCheckLevelAndCount.Reset;
    end;
  end;
end;

procedure TSqliteHeroDB.BeginTransaction;
begin
  TSqliteRoleDB(Owner).FDB.BeginTransaction;
end;

procedure TSqliteHeroDB.Commit;
begin
  TSqliteRoleDB(Owner).FDB.Commit;
end;

procedure TSqliteHeroDB.RollBack;
begin
  TSqliteRoleDB(Owner).FDB.RollBack;
end;

procedure TSqliteHeroDB.Execute(Sql: string);
begin
  TSqliteRoleDB(Owner).FDB.Execute(Sql);
end;

procedure TSqliteHeroDB.ResetAllGetDataStatement;
begin
  FStatementGetHero.Reset;
  FStatementGetAbil.Reset;
  FStatementGetAbilNG.Reset;
  FStatementGetAbilWine.Reset;
  FStatementGetAbilNpcAdd.Reset;
  FStatementGetGodBlessState.Reset;
  FStatementGetMagic.Reset;
  FStatementGetStatusTime.Reset;
  FStatementGetQuestFlag.Reset;
  FStatementGetItems.Reset;
  FStatementGetItemValueAdd.Reset;
  FStatementGetItemElementAdd.Reset;
  FStatementGetItemAddDataByte.Reset;
  FStatementGetItemAddDataInt.Reset;
  FStatementGetItemAddDataText.Reset;
  FStatementGetItemFlute.Reset;
  FStatementGetItemProgress.Reset;
  FStatementGetItemProperty.Reset;
  FStatementGetSkillPower.Reset;
end;

procedure TSqliteHeroDB.ResetAllSaveDataStatement;
begin
  FStatementUpdateHero.Reset;

  FStatementInsertAbil.Reset;
  FStatementInsertAbilNG.Reset;
  FStatementInsertAbilWine.Reset;

  FStatementUpdateAbil.Reset;
  FStatementUpdateAbilNG.Reset;
  FStatementUpdateAbilWine.Reset;

  FStatementInsertAbilNpcAdd.Reset;
  FStatementInsertGodBlessState.Reset;
  FStatementInsertMagic.Reset;
  FStatementInsertStatusTime.Reset;
  FStatementInsertQuestFlag.Reset;
  FStatementInsertItems.Reset;
  FStatementInsertItemValueAdd.Reset;
  FStatementInsertItemElementAdd.Reset;
  FStatementInsertItemAddDataByte.Reset;
  FStatementInsertItemAddDataInt.Reset;
  FStatementInsertItemAddDataText.Reset;
  FStatementInsertItemFlute.Reset;
  FStatementInsertItemProgress.Reset;
  FStatementInsertItemProperty.Reset;
  FStatementInsertSkillPower.Reset;
end;

{ TSqliteRoleDB }

constructor TSqliteRoleDB.Create;
begin
  inherited;
  FDB := nil;
end;

function TSqliteRoleDB.GetHumanDBClass: THumanDBClass;
begin
  Result := TSqliteHumanDB;
end;

function TSqliteRoleDB.GetHeroDBClass: THeroDBClass;
begin
  Result := TSqliteHeroDB;
end;

procedure TSqliteRoleDB.UpdateDB_1;
var
  sm: TSQLStatement;
  I, Count: Integer;
  ColumnNames: TStringList;
begin
  sm := FDB.Statements.AddSQLStatement('CheckFieldExists');
  try
    sm.Sql := 'select * from Human LIMIT 0;';
    sm.Prepare;
    sm.Reset;
    if sm.Step = SQLITE_DONE then
    begin
      ColumnNames := TStringList.Create;
      try
        Count := sm.GetColumnCount;
        for I := 0 to Count - 1 do
        begin
          ColumnNames.Add(sm.GetColumnName(I));
        end;

        ColumnNames.Sorted := True;

        // 扩展三个字段 chongchong 2017-04-16
        if ColumnNames.IndexOf('IsFilterGlobalDropItemMsg') < 0 then
        begin
          FDB.Execute('ALTER TABLE Human ADD COLUMN IsFilterGlobalDropItemMsg INTEGER;');
        end;

        if ColumnNames.IndexOf('IsFilterGlobalCenterMsg') < 0 then
        begin
          FDB.Execute('ALTER TABLE Human ADD COLUMN IsFilterGlobalCenterMsg INTEGER;');
        end;

        if ColumnNames.IndexOf('IsFilterGolbalSendMsg') < 0 then
        begin
          FDB.Execute('ALTER TABLE Human ADD COLUMN IsFilterGolbalSendMsg INTEGER;');
        end;

      finally
        ColumnNames.Free;
      end;
    end;

    sm.Reset;
  finally
    FDB.Statements.Clear;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_2;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'ALTER TABLE Human ADD COLUMN "MobileNumber" TEXT(20) COLLATE NOCASE;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "IsMobileBind" INTEGER;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "MobileVerifyCode" TEXT(8) COLLATE NOCASE;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "MobileSendTick" INTEGER;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "MobileResendCount" INTEGER;' + sLineBreak + 'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_3;
begin
  FDB.Execute('PRAGMA foreign_keys = OFF;');         // 更改字段准备工作
  try
    FDB.BeginTransaction;
    try
      FDB.Execute(AlterHeroTable_BodyLuck_LoyalPoint);
      FDB.Commit;

      //DB_Version := StrToIntDef(SQLITE_DBVERSION, 0);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
        FDB.RollBack;
      end;
    end;
  finally
    FDB.Execute('PRAGMA foreign_keys = ON;');
  end;
end;

procedure TSqliteRoleDB.UpdateDB_4;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'ALTER TABLE Human ADD COLUMN "IsOpenStorage1" INTEGER;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "IsOpenStorage2" INTEGER;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "IsOpenStorage3" INTEGER;' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_5;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'ALTER TABLE Human ADD COLUMN "MemberType" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "MemberLevel" INTEGER DEFAULT 0;' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_6;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'ALTER TABLE Human ADD COLUMN "ClearDayVarTime" INTEGER DEFAULT 0;' + sLineBreak +
'CREATE TABLE "HumanVariableJ" (' + sLineBreak + '"HumanID"  INTEGER NOT NULL,' + sLineBreak + '"Index"  INTEGER NOT NULL,' + sLineBreak + '"Value"  INTEGER DEFAULT 0,' + sLineBreak + 'PRIMARY KEY ("HumanID" ASC, "Index" ASC),' + sLineBreak + 'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak + ');' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_7;
var
  //sm: TSQLStatement;
  //I, Count, nRet, DB_Version: Integer;
  //ColumnNames: TStringList;
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'ALTER TABLE HumanItemProperty ADD COLUMN "Value2" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE HumanItemProperty ADD COLUMN "Value3" INTEGER DEFAULT 0;' + sLineBreak
      + 'ALTER TABLE HeroItemProperty ADD COLUMN "Value2" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE HeroItemProperty ADD COLUMN "Value3" INTEGER DEFAULT 0;' + sLineBreak
      + 'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_8;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    {
    S :=

    'CREATE TABLE "HumanSkillPower" (' + sLineBreak +
    '  "HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '  "SkillID"  INTEGER NOT NULL,' + sLineBreak +
    '  "AttackValue"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "DefenseValue"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "IsFixedValue"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "Target"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "RemainingTime"  INTEGER DEFAULT 0,' + sLineBreak +
    '  PRIMARY KEY ("HumanID", "SkillID")' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE "HeroSkillPower" (' + sLineBreak +
    '  "HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '  "SkillID"  INTEGER NOT NULL,' + sLineBreak +
    '  "AttackValue"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "DefenseValue"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "IsFixedValue"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "Target"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "RemainingTime"  INTEGER DEFAULT 0,' + sLineBreak +
    '  PRIMARY KEY ("HeroID", "SkillID")' + sLineBreak +
    ');' + sLineBreak +

    'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION  + ');';
    }
    S := 'DROP TABLE IF EXISTS "HumanSkillPower";' + sLineBreak + 'DROP TABLE IF EXISTS "HeroSkillPower";' + sLineBreak;

    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_9;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S :=
'CREATE TABLE "HumanSkillPower" (' + sLineBreak + '  "HumanID"  INTEGER NOT NULL,' + sLineBreak + '  "SkillID"  INTEGER NOT NULL,' + sLineBreak + '  "HumanAttackPercent"  INTEGER DEFAULT 0,' + sLineBreak + '  "HumanAttackValue"  INTEGER DEFAULT 0,' + sLineBreak + '  "MonAttackPercent"  INTEGER DEFAULT 0,' + sLineBreak + '  "MonAttackValue"  INTEGER DEFAULT 0,' + sLineBreak + '  "DefensePercent"  INTEGER DEFAULT 0,' + sLineBreak + '  "DefenseValue"  INTEGER DEFAULT 0,' + sLineBreak + '  "RemainingTime"  INTEGER DEFAULT 0,' + sLineBreak + '  PRIMARY KEY ("HumanID", "SkillID")' + sLineBreak + ');' + sLineBreak +
'CREATE TABLE "HeroSkillPower" (' + sLineBreak + '  "HeroID"  INTEGER NOT NULL,' + sLineBreak + '  "SkillID"  INTEGER NOT NULL,' + sLineBreak + '  "HumanAttackPercent"  INTEGER DEFAULT 0,' + sLineBreak + '  "HumanAttackValue"  INTEGER DEFAULT 0,' + sLineBreak + '  "MonAttackPercent"  INTEGER DEFAULT 0,' + sLineBreak + '  "MonAttackValue"  INTEGER DEFAULT 0,' + sLineBreak + '  "DefensePercent"  INTEGER DEFAULT 0,' + sLineBreak + '  "DefenseValue"  INTEGER DEFAULT 0,' + sLineBreak + '  "RemainingTime"  INTEGER DEFAULT 0,' + sLineBreak + '  PRIMARY KEY ("HeroID", "SkillID")' + sLineBreak + ');' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';

    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_10;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'ALTER TABLE Human ADD COLUMN "ExtBagPageCount" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "ExtBagOpenItemCount" INTEGER DEFAULT 0;' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_11;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'ALTER TABLE HumanItemFlute ADD COLUMN "OverlapCount" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE HeroItemFlute ADD COLUMN "OverlapCount" INTEGER DEFAULT 0;' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_12;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'ALTER TABLE Human ADD COLUMN "AddMaxWeight" INTEGER DEFAULT 0;' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_13;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'ALTER TABLE HumanItems ADD COLUMN "NewExpand3" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE HumanItems ADD COLUMN "NewExpand4" INTEGER DEFAULT 0;' + sLineBreak +
'ALTER TABLE HeroItems ADD COLUMN "NewExpand3" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE HeroItems ADD COLUMN "NewExpand4" INTEGER DEFAULT 0;' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_14;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'ALTER TABLE HumanItemProperty ADD COLUMN "HintModule" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE HeroItemProperty ADD COLUMN "HintModule" INTEGER DEFAULT 0;' + sLineBreak + 'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_15;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'ALTER TABLE Human ADD COLUMN "IsAttackMonSavePowerRate" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "AttackMonPowerRate" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE Human ADD COLUMN "AttackMonPowerRateTime" INTEGER DEFAULT 0;' + sLineBreak +
'ALTER TABLE Hero ADD COLUMN "IsAttackMonSavePowerRate" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE Hero ADD COLUMN "AttackMonPowerRate" INTEGER DEFAULT 0;' + sLineBreak + 'ALTER TABLE Hero ADD COLUMN "AttackMonPowerRateTime" INTEGER DEFAULT 0;' + sLineBreak +
'UPDATE Human set IsAttackMonSavePowerRate = IsSavePowerRate, AttackMonPowerRate = PowerRate, AttackMonPowerRateTime = PowerRateTime;' + sLineBreak + 'UPDATE Hero set IsAttackMonSavePowerRate = IsSavePowerRate, AttackMonPowerRate = PowerRate, AttackMonPowerRateTime = PowerRateTime;' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_16;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'CREATE TABLE "HumanItemAddDataByte" (' + sLineBreak + '"HumanID"  INTEGER NOT NULL,' + sLineBreak + '"ItemType"  INTEGER NOT NULL,' + sLineBreak + '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak + '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak + '"Value"  INTEGER,' + sLineBreak + 'PRIMARY KEY ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak + ');' + sLineBreak +
'CREATE TABLE "HumanItemAddDataInt" (' + sLineBreak + '"HumanID"  INTEGER NOT NULL,' + sLineBreak + '"ItemType"  INTEGER NOT NULL,' + sLineBreak + '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak + '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak + '"Value"  INTEGER,' + sLineBreak + 'PRIMARY KEY ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak + ');' + sLineBreak +
'CREATE TABLE "HumanItemAddDataText" (' + sLineBreak + '"HumanID"  INTEGER NOT NULL,' + sLineBreak + '"ItemType"  INTEGER NOT NULL,' + sLineBreak + '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak + '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak + '"Value"  TEXT,' + sLineBreak + 'PRIMARY KEY ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak + ');' + sLineBreak +
'CREATE TABLE "HeroItemAddDataByte" (' + sLineBreak + '"HeroID"  INTEGER NOT NULL,' + sLineBreak + '"ItemType"  INTEGER NOT NULL,' + sLineBreak + '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak + '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak + '"Value"  INTEGER,' + sLineBreak + 'PRIMARY KEY ("HeroID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak + ');' + sLineBreak +
'CREATE TABLE "HeroItemAddDataInt" (' + sLineBreak + '"HeroID"  INTEGER NOT NULL,' + sLineBreak + '"ItemType"  INTEGER NOT NULL,' + sLineBreak + '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak + '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak + '"Value"  INTEGER,' + sLineBreak + 'PRIMARY KEY ("HeroID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak + ');' + sLineBreak +
'CREATE TABLE "HeroItemAddDataText" (' + sLineBreak + '"HeroID"  INTEGER NOT NULL,' + sLineBreak + '"ItemType"  INTEGER NOT NULL,' + sLineBreak + '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak + '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak + '"Value"  TEXT,' + sLineBreak + 'PRIMARY KEY ("HeroID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak + ');' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_17;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'CREATE TABLE "HumanVariableZ" (' + sLineBreak + '"HumanID"  INTEGER NOT NULL,' + sLineBreak + '"Index"  INTEGER NOT NULL,' + sLineBreak + '"Value"  TEXT DEFAULT 0,' + sLineBreak + 'PRIMARY KEY ("HumanID" ASC, "Index" ASC),' + sLineBreak + 'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak + ');' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';
    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.UpdateDB_18;
var
  S: string;
begin
  FDB.BeginTransaction;
  try
    S := 'CREATE TABLE "HumanMoney" (' + sLineBreak + '"HumanID"  INTEGER NOT NULL,' + sLineBreak + '"MoneyName"  TEXT,' + sLineBreak + '"Value"  INTEGER DEFAULT 0,' + sLineBreak +//      'PRIMARY KEY ("HumanID" ASC, "Index" ASC),' + sLineBreak +
      'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak + ');' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';

    FDB.Execute(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TSqliteRoleDB.DoInit;
resourcestring
  SQL_CHCECK_db_constant = 'select 1 from sqlite_master where type = "table" and name = "db_constant";';
var
  sm: TSQLStatement;
  nRet, DB_Version: Integer;
  IsTryAgain: Boolean;
  _FileName{, S}: string;
begin
  Assert(Length(FFileName) > 0, 'RoleDB init must set file name');

  if not Assigned(FDB) then
  begin
    if FileExists(FFileName) then
    begin
      FDB := TSqlite3Database.Create;
      FDB.MustExist := True;
      FDB.Database := FFileName;

      IsTryAgain := False;
      try
        FDB.Connected := True;
      except
        on E: Exception do
        begin
          if FDB.ErrorCode = 11 then
          begin
            IsTryAgain := True;
          end
          else
          begin
            raise Exception.Create(E.Message);
          end;
        end;
      end;

      if IsTryAgain then
      begin
        _FileName := FFileName + '-shm';
        if FileExists(_FileName) then
        begin
          DeleteFile(_FileName);
        end;

        _FileName := FFileName + '-wal';
        if FileExists(_FileName) then
        begin
          DeleteFile(_FileName);
        end;

        FDB.Connected := True;
      end;
    end
    else
    begin
      FDB := TSqlite3Database.Create;
      FDB.MustExist := False;
      FDB.Database := FFileName;
      FDB.Connected := True;

      FDB.Execute(SQLITE_CREATE_ROLEDATA_TABLES_HUMAN);
      FDB.Execute(SQLITE_CREATE_ROLEDATA_TABLES_HERO1);
      FDB.Execute(SQLITE_CREATE_ROLEDATA_TABLES_HERO2);
      FDB.Execute(SQLITE_CREATE_ROLEDATA_TABLES_HERO3);
      FDB.Execute(SQLITE_CREATE_ROLEDATA_TABLES_HERO4);
    end;


    {
      修改字段类型：
      // 资料来源：https://www.sqlite.org/lang_altertable.html
      
      1、If foreign key constraints are enabled, disable them using PRAGMA foreign_keys=OFF.
      2、Start a transaction.
      3、Remember the format of all indexes and triggers associated with table X. This information will be needed in step 8 below. One way to do this is to run a query like the following: SELECT type, sql FROM sqlite_master WHERE tbl_name='X'.
      4、Use CREATE TABLE to construct a new table "new_X" that is in the desired revised format of table X. Make sure that the name "new_X" does not collide with any existing table name, of course.
      5、Transfer content from X into new_X using a statement like: INSERT INTO new_X SELECT ... FROM X.
      6、Drop the old table X: DROP TABLE X.
      7、Change the name of new_X to X using: ALTER TABLE new_X RENAME TO X.
      8、Use CREATE INDEX and CREATE TRIGGER to reconstruct indexes and triggers associated with table X. Perhaps use the old format of the triggers and indexes saved from step 3 above as a guide, making changes as appropriate for the alteration.
      9、If any views refer to table X in a way that is affected by the schema change, then drop those views using DROP VIEW and recreate them with whatever changes are necessary to accommodate the schema change using CREATE VIEW.
      10、If foreign key constraints were originally enabled then run PRAGMA foreign_key_check to verify that the schema change did not break any foreign key constraints.
      11、Commit the transaction started in step 2.
      12、If foreign keys constraints were originally enabled, reenable them now.
    }

    DB_Version := 0;
    sm := FDB.Statements.AddSQLStatement('check_table_db_constant');
    try
      sm.Sql := SQL_CHCECK_db_constant;
      sm.Prepare;
      nRet := sm.Step;
      sm.Reset;

      if nRet <> SQLITE_ROW then
      begin
        FDB.Execute('PRAGMA foreign_keys = OFF;');         // 更改字段准备工作
        try
          FDB.BeginTransaction;
          try
            FDB.Execute(AlterFieldHuamName_HeroName);
            FDB.Commit;

            DB_Version := StrToIntDef(SQLITE_DBVERSION, 0);
          except
            on E: Exception do
            begin
              MainOutMessage(E.Message);
              FDB.RollBack;
            end;
          end;
        finally
          FDB.Execute('PRAGMA foreign_keys = ON;');
        end;
      end
      else
      begin
        sm := FDB.Statements.AddSQLStatement('get_db_constant_value');
        sm.Sql := 'select ConstValue from db_constant where ConstName = ?;';
        sm.Prepare;
        sm.OrderBindText('version');
        if sm.Step = SQLITE_ROW then
        begin
          DB_Version := sm.OrderGetColumnValueInt;
        end;
        sm.Reset;
      end;
    finally
      FDB.Statements.Clear;
    end;

    if DB_Version = 0 then
    begin
      UpdateDB_1;
      UpdateDB_2;
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;
      UpdateDB_7;
      UpdateDB_9;
      UpdateDB_10;
      UpdateDB_11;
      UpdateDB_12;
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20170420 then
    begin
      UpdateDB_2;
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;
      UpdateDB_7;
      UpdateDB_9;
      UpdateDB_10;
      UpdateDB_11;
      UpdateDB_12;
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20170425 then
    begin
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;
      UpdateDB_7;
      UpdateDB_9;
      UpdateDB_10;
      UpdateDB_11;
      UpdateDB_12;
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20170504 then
    begin
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;
      UpdateDB_7;
      UpdateDB_9;
      UpdateDB_10;
      UpdateDB_11;
      UpdateDB_12;
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20170506 then
    begin
      UpdateDB_5;
      UpdateDB_6;
      UpdateDB_7;
      UpdateDB_9;
      UpdateDB_10;
      UpdateDB_11;
      UpdateDB_12;
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20180206 then
    begin
      UpdateDB_6;
      UpdateDB_7;
      UpdateDB_9;
      UpdateDB_10;
      UpdateDB_11;
      UpdateDB_12;
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20180719 then
    begin
      UpdateDB_7;
      UpdateDB_9;
      UpdateDB_10;
      UpdateDB_11;
      UpdateDB_12;
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20190314 then
    begin
      UpdateDB_9;
      UpdateDB_10;
      UpdateDB_11;
      UpdateDB_12;
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20190318 then
    begin
      UpdateDB_8;
      UpdateDB_9;
      UpdateDB_10;
      UpdateDB_11;
      UpdateDB_12;
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20190319 then
    begin
      UpdateDB_10;
      UpdateDB_11;
      UpdateDB_12;
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20190512 then
    begin
      UpdateDB_11;
      UpdateDB_12;
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20190606 then
    begin
      UpdateDB_12;
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20190802 then
    begin
      UpdateDB_13;
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20190928 then
    begin
      UpdateDB_14;
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20200813 then
    begin
      UpdateDB_15;
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20200912 then
    begin
      UpdateDB_16;
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20200916 then
    begin
      UpdateDB_17;
      UpdateDB_18;
    end
    else if DB_Version = 20211218 then
    begin
      UpdateDB_18;
    end;

    inherited;

    // pragma auto_vacuum ＝ 0|1 设置自动缩放文件
    if g_boSqliteFastSave then
      FDB.Execute('PRAGMA synchronous = OFF;')               // OFF = 0, NORMAL = 1, FULL = 2
    else
      FDB.Execute('PRAGMA synchronous = NORMAL;');           // OFF = 0, NORMAL = 1, FULL = 2
    FDB.Execute('PRAGMA cache_size = 8000;');
    FDB.Execute('PRAGMA temp_store = MEMORY;');              // DEFAULT = 0, FILE = 1, MEMORY = 2;
  end;
end;

procedure TSqliteRoleDB.DoFainal;
begin
  inherited;

  FDB.Connected := False;
  FDB.Free;
  FDB := nil;
end;

procedure TSqliteRoleDB.Commit;
begin
  FDB.Commit;
end;

procedure TSqliteRoleDB.RollBack;
begin
  FDB.RollBack;
end;

procedure TSqliteRoleDB.BeginTransaction;
begin
  FDB.BeginTransaction;
end;

end.


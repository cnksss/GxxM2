unit MySqlRoleDB;

interface

uses
  Windows, Classes, SysUtils, RoleDB, Grobal2, DBShare, MySqlDataBase, MySqlCli,
  MySQLWrap, MySqlCreateTableSql;

type
  TMySqlHumanDB = class(THumanDB)
  private
    FStatementGetID: TMySqlStatement;
    FStatementCheckHumanExists: TMySqlStatement;
    FStatementGetHumanCount: TMySqlStatement;
    FStatementGetOtherHumanName: TMySqlStatement;
    FStatementGetHumanHeroName: TMySqlStatement;
    FStatementGetHumanHeroID: TMySqlStatement;
    FStatementGetHumanGoldInfo: TMySqlStatement;
    FStatementUpdateHumanGoldInfo: TMySqlStatement;
    FStatementGetHumanBaseInfo: TMySqlStatement;
    FStatementQueryHumans: TMySqlStatement;
    FStatementSearchByAccountMatchComplete: TMySqlStatement;
    FStatementSearchByAccountMatchFuzzy: TMySqlStatement;
    FStatementSearchByNameMatchComplete: TMySqlStatement;
    FStatementSearchByNameMatchFuzzy: TMySqlStatement;
    FStatementSearchByLevel: TMySqlStatement;
    FStatementGetMobileNumbers1: TMySqlStatement;
    FStatementGetMobileNumbers2: TMySqlStatement;
    FStatementSelect: TMySqlStatement;
    FStatementUnSelect: TMySqlStatement;
    FStatementGetHuman: TMySqlStatement;
    FStatementGetAbil: TMySqlStatement;
    FStatementGetAbilNG: TMySqlStatement;
    FStatementGetAbilWine: TMySqlStatement;
    FStatementGetAbilNpcAdd: TMySqlStatement;
    FStatementGetGamePetData: TMySqlStatement;
    FStatementGetGodBlessState: TMySqlStatement;
    FStatementGetMagic: TMySqlStatement;
    FStatementGetMagicUseTick: TMySqlStatement;
    FStatementGetStatusTime: TMySqlStatement;
    FStatementGetQuestFlag: TMySqlStatement;
    FStatementGetVariableU: TMySqlStatement;
    FStatementGetVariableT: TMySqlStatement;
    FStatementGetVariableJ: TMySqlStatement;
    FStatementGetVariableZ: TMySqlStatement;
    FStatementGetItems: TMySqlStatement;
    FStatementGetItemValueAdd: TMySqlStatement;
    FStatementGetItemElementAdd: TMySqlStatement;
    FStatementGetItemAddDataByte: TMySqlStatement;
    FStatementGetItemAddDataInt: TMySqlStatement;
    FStatementGetItemAddDataText: TMySqlStatement;
    FStatementGetItemFlute: TMySqlStatement;
    FStatementGetItemProgress: TMySqlStatement;
    FStatementGetItemProperty: TMySqlStatement;
    FStatementGetSkillPower: TMySqlStatement;
    FStatementAddHuman: TMySqlStatement;
    FStatementDeleteOrRestore: TMySqlStatement;
    FStatementRecordLoginTime: TMySqlStatement;
    FStatementUpdateHumanName: TMySqlStatement;
    FStatementUpdateHumanNameDearName: TMySqlStatement;
    FStatementUpdateHumanNameMasterName: TMySqlStatement;
    FStatementUpdateHuman: TMySqlStatement;
    FStatementInsertAbil: TMySqlStatement;
    FStatementInsertAbilNG: TMySqlStatement;
    FStatementInsertAbilWine: TMySqlStatement;
    FStatementInsertAbilNpcAdd: TMySqlStatement;
    FStatementInsertGamePetData: TMySqlStatement;
    FStatementInsertGodBlessState: TMySqlStatement;
    FStatementInsertMagic: TMySqlStatement;
    FStatementInsertMagicUseTick: TMySqlStatement;
    FStatementInsertStatusTime: TMySqlStatement;
    FStatementInsertQuestFlag: TMySqlStatement;
    FStatementInsertVariableU: TMySqlStatement;
    FStatementInsertVariableT: TMySqlStatement;
    FStatementInsertVariableJ: TMySqlStatement;
    FStatementInsertVariableZ: TMySqlStatement;
    FStatementInsertCustomMoney: TMySqlStatement;
    FStatementInsertItems: TMySqlStatement;
    FStatementInsertItemValueAdd: TMySqlStatement;
    FStatementInsertItemElementAdd: TMySqlStatement;
    FStatementInsertItemAddDataByte: TMySqlStatement;
    FStatementInsertItemAddDataInt: TMySqlStatement;
    FStatementInsertItemAddDataText: TMySqlStatement;
    FStatementInsertItemFlute: TMySqlStatement;
    FStatementInsertItemProgress: TMySqlStatement;
    FStatementInsertItemProperty: TMySqlStatement;
    FStatementInsertSkillPower: TMySqlStatement;
    FStatementGetLevelRankCheckLevel: TMySqlStatement;
    FStatementGetLevelRankTopCount: TMySqlStatement;
    FStatementGetLevelRankCheckLevelAndCount: TMySqlStatement;
    FStatementGetMasterRankCheckLevel: TMySqlStatement;
    FStatementGetMasterRankTopCount: TMySqlStatement;
    FStatementGetMasterRankCheckLevelAndCount: TMySqlStatement;
    FStatementBuyPlayer: TMySqlStatement;
    FStatementGetCustomMoney: TMySqlStatement;
    FStatementGetCustomMoneyByName: TMySqlStatement;
    FStatementUpdateCustomMoney: TMySqlStatement;
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

  TMySqlHeroDB = class(THeroDB)
    FStatementGetID: TMySqlStatement;
    FStatementGetHeroCount: TMySqlStatement;
    FStatementGetHumanInfo: TMySqlStatement;
    FStatementGetHumanInfo2: TMySqlStatement;
    FStatementSearchByAccountMatchComplete: TMySqlStatement;
    FStatementSearchByAccountMatchFuzzy: TMySqlStatement;
    FStatementSearchByNameMatchComplete: TMySqlStatement;
    FStatementSearchByNameMatchFuzzy: TMySqlStatement;
    FStatementGetHero: TMySqlStatement;
    FStatementGetAbil: TMySqlStatement;
    FStatementGetAbilNG: TMySqlStatement;
    FStatementGetAbilWine: TMySqlStatement;
    FStatementGetAbilNpcAdd: TMySqlStatement;
    FStatementGetGodBlessState: TMySqlStatement;
    FStatementGetMagic: TMySqlStatement;
    FStatementGetStatusTime: TMySqlStatement;
    FStatementGetQuestFlag: TMySqlStatement;
    FStatementGetItems: TMySqlStatement;
    FStatementGetItemValueAdd: TMySqlStatement;
    FStatementGetItemElementAdd: TMySqlStatement;
    FStatementGetItemAddDataByte: TMySqlStatement;
    FStatementGetItemAddDataInt: TMySqlStatement;
    FStatementGetItemAddDataText: TMySqlStatement;
    FStatementGetItemFlute: TMySqlStatement;
    FStatementGetItemProgress: TMySqlStatement;
    FStatementGetItemProperty: TMySqlStatement;
    FStatementGetSkillPower: TMySqlStatement;
    FStatementAddHero: TMySqlStatement;
    FStatementDeleteOrRestore: TMySqlStatement;
    FStatementUpdateHeroName: TMySqlStatement;
    FStatementUpdateHumanHeroName: TMySqlStatement;
    FStatementUpdateHumanHeroName2: TMySqlStatement;
    FStatementUpdateHero: TMySqlStatement;
    FStatementInsertAbil: TMySqlStatement;
    FStatementInsertAbilNG: TMySqlStatement;
    FStatementInsertAbilWine: TMySqlStatement;
    FStatementInsertAbilNpcAdd: TMySqlStatement;
    FStatementInsertGodBlessState: TMySqlStatement;
    FStatementInsertMagic: TMySqlStatement;
    FStatementInsertStatusTime: TMySqlStatement;
    FStatementInsertQuestFlag: TMySqlStatement;
    FStatementInsertItems: TMySqlStatement;
    FStatementInsertItemValueAdd: TMySqlStatement;
    FStatementInsertItemElementAdd: TMySqlStatement;
    FStatementInsertItemAddDataByte: TMySqlStatement;
    FStatementInsertItemAddDataInt: TMySqlStatement;
    FStatementInsertItemAddDataText: TMySqlStatement;
    FStatementInsertItemFlute: TMySqlStatement;
    FStatementInsertItemProgress: TMySqlStatement;
    FStatementInsertItemProperty: TMySqlStatement;
    FStatementInsertSkillPower: TMySqlStatement;
    FStatementGetLevelRankCheckLevel: TMySqlStatement;
    FStatementGetLevelRankTopCount: TMySqlStatement;
    FStatementGetLevelRankCheckLevelAndCount: TMySqlStatement;
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

  TMySqlRoleDB = class(TRoleDB)
  private
    FLastRequestTick: LongWord;
    FMySqlLib: TMySQLLib;
    FDB: TMySqlDataBase;
  private
    procedure OnRequest(Sender: TObject);
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
  protected
    procedure DoInit; override;
    procedure DoFainal; override;
    function GetHumanDBClass: THumanDBClass; override;
    function GetHeroDBClass: THeroDBClass; override;
  public
    constructor Create; override;
    destructor Destroy; override;
    procedure Run; override;
    procedure BeginTransaction;
    procedure Commit;
    procedure RollBack;
  end;

implementation

{ TSqliteHumanDB }

procedure TMySqlHumanDB.DoInit;
var
  Stms: TMySqlStatements;
begin
  Assert(Owner is TMySqlRoleDB, 'TSqliteHumanDB owner type error.');
  Assert(TMySqlRoleDB(Owner).FDB <> nil, 'TMySqlRoleDB.DB not create');
  Stms := TMySqlRoleDB(Owner).FDB.Statements;

  FStatementGetID := Stms.AddSQLStatement('HumanGetID');
  FStatementGetID.Sql := 'select HumanID from Human where HumanName = ?;';                     // COLLATE NOCASE

  FStatementCheckHumanExists := Stms.AddSQLStatement('CheckHumanExists');
  FStatementCheckHumanExists.Sql := 'select HumanID from Human where Account = ? and HumanName = ?;';

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
  FStatementGetMobileNumbers1.Sql := 'SELECT DISTINCT MobileNumber FROM  Human WHERE Length(MobileNumber) > 0;';

  FStatementGetMobileNumbers2 := Stms.AddSQLStatement('HumanGetMobileNumbers2');
  FStatementGetMobileNumbers2.Sql := 'select DISTINCT MobileNumber from Human where (Length(MobileNumber) > 0) and (IsMobileBind = 1);';

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
    'IsFixedHero,' + 'IsStorageHero,' + 'IsStorageDeputyHero,' + 'HeroName,' + 'DeputyHeroName,' + 'DeputyHeroJob,' + 'Nation,' + 'NationCredit,' + 'RevivalTime,' + 'InfinityStorageExtCount,' + 'IsSaveKillMonExpRate,' + 'KillMonExpRate,' + 'KillMonExpRateTime,' + 'IsSavePowerRate,' + 'PowerRate,' + 'PowerRateTime,' + 'IsAttackMonSavePowerRate,' + 'AttackMonPowerRate,' + 'AttackMonPowerRateTime,' + 'IsSaveKillMonBurstRate,' + 'KillMonBurstRate,' + 'KillMonBurstRateTime,' + 'FBCreateTime,' + 'JewelryBoxStatus,' + 'IsShowFashion,' + 'IsShowGodBless,' + 'ActiveFengHao,' + 'IsOpenStorage1,' + 'IsOpenStorage2,' + 'IsOpenStorage3,' +
'ExtBagPageCount,' + 'ExtBagOpenItemCount,' + 'AddMaxWeight,' +
'HighLevelKillMonFixExpTimeLeft,' + 'MobileNumber,' + 'IsMobileBind,' + 'MobileVerifyCode,' + 'MobileSendTick,' + 'MobileResendCount, ' + 'ClearDayVarTime ' + 'from Human ' + 'where (Account = ?) and (HumanName = ?);';

  FStatementGetAbil := Stms.AddSQLStatement('HumanSelectAbil');
  FStatementGetAbil.Sql := 'select ' + 'AC1,' + 'AC2,' + 'MAC1,' + 'MAC2,' + 'DC1,' + 'DC2,' + 'MC1,' + 'MC2,' + 'SC1,' + 'SC2,' + 'HP,' + 'MaxHP,' + 'MP,' + 'MaxMP,' + 'Exp,' + 'MaxExp,' + 'Weight,' + 'MaxWeight,' + 'WearWeight,' + 'MaxWearWeight,' + 'HandWeight,' + 'MaxHandWeight,' + 'AdjustAbilPoint,' + 'AdjustAbilDC,' + 'AdjustAbilMC,' + 'AdjustAbilSC,' + 'AdjustAbilAC,' + 'AdjustAbilMAC,' + 'AdjustAbilHP,' + 'AdjustAbilMP,' + 'AdjustAbilHit,' + 'AdjustAbilSpeed,' + 'AdjustAbilMaxRate ' + 'from HumanAbil ' + 'where HumanID = ?;';

  FStatementGetAbilNG := Stms.AddSQLStatement('HumanSelectAbilNG');
  FStatementGetAbilNG.Sql := 'select ' + 'IsTrainingNG,' + 'IsTrainingXF,' + 'AbilNGLevel,' + 'AbilNGValue,' + 'AbilNGMaxValue,' + 'AbilNGExp,' + 'AbilNGMaxExp,' + 'ContinuousMagicOrder1,' + 'ContinuousMagicOrder2,' + 'ContinuousMagicOrder3,' + 'IsOpenLastContinuous,' + 'LastContinuousMagicOrder,' + 'Meridians1Level,' + 'Meridians1BlastHitRate1,' + 'Meridians1Acupoints1,' + 'Meridians1Acupoints2,' + 'Meridians1Acupoints3,' + 'Meridians1Acupoints4,' + 'Meridians1Acupoints5,' + 'Meridians2Level,' +
    'Meridians2BlastHitRate,' + 'Meridians2Acupoints1,' + 'Meridians2Acupoints2,' + 'Meridians2Acupoints3,' + 'Meridians2Acupoints4,' + 'Meridians2Acupoints5,' + 'Meridians3Level,' + 'Meridians3BlastHitRate,' + 'Meridians3Acupoints1,' + 'Meridians3Acupoints2,' + 'Meridians3Acupoints3,' + 'Meridians3Acupoints4,' + 'Meridians3Acupoints5,' + 'Meridians4Level,' + 'Meridians4BlastHitRate,' + 'Meridians4Acupoints1,' + 'Meridians4Acupoints2,' + 'Meridians4Acupoints3,' + 'Meridians4Acupoints4,' + 'Meridians4Acupoints5,' + 'Meridians5Level,' + 'Meridians5BlastHitRate,' + 'Meridians5Acupoints1,' + 'Meridians5Acupoints2,' + 'Meridians5Acupoints3,' + 'Meridians5Acupoints4,' + 'Meridians5Acupoints5 ' + 'from HumanAbilNG ' + 'where HumanID = ?;';

  FStatementGetAbilWine := Stms.AddSQLStatement('HumanSelectAbilWine');
  FStatementGetAbilWine.Sql := 'select ' + 'IsDrinkedWine,' + 'IsDrinkWineDrunk,' + 'DrinkWineQuality,' + 'DrinkWineAlcohol,' + 'AbilAlcohol,' + 'AbilMaxAlcohol,' + 'AbilDrinkValue,' + 'AbilMedicineLevel,' + 'AbilMedicineValue,' + 'AbilMaxMedicineValue ' + 'from HumanAbilWine ' + 'where HumanID = ?;';

  FStatementGetAbilNpcAdd := Stms.AddSQLStatement('HumanSelectAbilNpcAdd');
  FStatementGetAbilNpcAdd.Sql := 'select ' + '`Index`,' + '`Value` ' + 'from HumanAbilNpcAdd ' + 'where HumanID = ?;';

  FStatementGetGamePetData := Stms.AddSQLStatement('HumanSelectGamePetData');
  FStatementGetGamePetData.Sql := 'select ' + '`Index`,' + 'Name,' + 'Level,' + 'HP,' + 'MP,' + 'Exp,' + 'Magic1,' + 'Magic2,' + 'Magic3,' + 'Magic4,' + 'Magic5,' + 'Magic6,' + 'Magic7,' + 'Magic8 ' + 'from HumanGamePetData ' + 'where HumanID = ?;';

  FStatementGetGodBlessState := Stms.AddSQLStatement('HumanSelectGodBlessState');
  FStatementGetGodBlessState.Sql := 'select ' + '`Index`,' + '`Value` ' + 'from HumanGodBlessState ' + 'where HumanID = ?;';

  FStatementGetMagic := Stms.AddSQLStatement('HumanSelectMagic');
  FStatementGetMagic.Sql := 'select ' + 'MagicType,' + 'MagicIndex,' + 'MagicID,' + 'MagicAttr,' + 'MagicLevel,' + 'MagicNewLevel,' + 'MagicKey,' + 'MagicTranPoint,' + 'MagicIsUseItemAdd ' + 'from HumanMagic ' + 'where HumanID = ?;';

  FStatementGetMagicUseTick := Stms.AddSQLStatement('HumanSelectMagicUseTick');
  FStatementGetMagicUseTick.Sql := 'select ' + 'MagicID,' + '`Value` ' + 'from HumanMagicUseTick ' + 'where HumanID = ?;';

  FStatementGetStatusTime := Stms.AddSQLStatement('HumanSelectStatusTime');
  FStatementGetStatusTime.Sql := 'select ' + '`Index`,' + '`Value` ' + 'from HumanStatusTime ' + 'where HumanID = ?;';

  FStatementGetQuestFlag := Stms.AddSQLStatement('HumanSelectQuestFlag');
  FStatementGetQuestFlag.Sql := 'select ' + '`Index`,' + '`Value` ' + 'from HumanQuestFlag ' + 'where HumanID = ?;';

  FStatementGetVariableU := Stms.AddSQLStatement('HumanSelectVariableU');
  FStatementGetVariableU.Sql := 'select ' + '`Index`,' + '`Value` ' + 'from HumanVariableU ' + 'where HumanID = ?;';

  FStatementGetVariableT := Stms.AddSQLStatement('HumanSelectVariableT');
  FStatementGetVariableT.Sql := 'select ' + '`Index`,' + '`Value` ' + 'from HumanVariableT ' + 'where HumanID = ?;';

  FStatementGetVariableJ := Stms.AddSQLStatement('HumanSelectVariableJ');
  FStatementGetVariableJ.Sql := 'select ' + '`Index`,' + '`Value` ' + 'from HumanVariableJ ' + 'where HumanID = ?;';

  FStatementGetVariableZ := Stms.AddSQLStatement('HumanSelectVariableZ');
  FStatementGetVariableZ.Sql := 'select ' + '`Index`,' + '`Value` ' + 'from HumanVariableZ ' + 'where HumanID = ?;';

  FStatementGetItems := Stms.AddSQLStatement('HumanSelectitems');
  FStatementGetItems.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'MakeIndex,' + 'DBIndex,' + 'Name,' + 'Dura,' + 'DuraMax,' + 'HeroM2DressEffect,' + 'UpgradeCount,' + 'IsStartTime,' + 'LimitTime,' + 'HeroM2Light,' + 'Color,' + 'IsBind,' + 'BindOption,' + 'Effect,' + 'NewLooks,' + 'NewShape,' + 'FluteCount,' + 'PropertyText,' + 'PropertyTextColor,' + 'ItemFrom,' + 'ItemFromMap,' + 'ItemFromMon,' + 'ItemFromMaker,' + 'ItemFromDate,' + 'InsuranceCount, ' + 'NewExpand3, ' + 'NewExpand4 ' + 'from HumanItems ' + 'where HumanID = ?;';

  FStatementGetItemValueAdd := Stms.AddSQLStatement('HumanSelectitemValueAdd');
  FStatementGetItemValueAdd.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + '`Value` ' + 'from HumanItemValueAdd ' + 'where HumanID = ?;';

  FStatementGetItemElementAdd := Stms.AddSQLStatement('HumanSelectitemElementAdd');
  FStatementGetItemElementAdd.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + '`Value` ' + 'from HumanItemElementAdd ' + 'where HumanID = ?;';

  FStatementGetItemAddDataByte := Stms.AddSQLStatement('HumanSelectitemAddDataByte');
  FStatementGetItemAddDataByte.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + '`Value` ' + 'from HumanItemAddDataByte ' + 'where HumanID = ?;';

  FStatementGetItemAddDataInt := Stms.AddSQLStatement('HumanSelectItemAddDataInt');
  FStatementGetItemAddDataInt.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + '`Value` ' + 'from HumanItemAddDataInt ' + 'where HumanID = ?;';

  FStatementGetItemAddDataText := Stms.AddSQLStatement('HumanSelectitemAddDataText');
  FStatementGetItemAddDataText.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + '`Value` ' + 'from HumanItemAddDataText ' + 'where HumanID = ?;';

  FStatementGetItemFlute := Stms.AddSQLStatement('HumanSelectitemFlute');
  FStatementGetItemFlute.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + '`Value`,' + '`OverlapCount` ' + 'from HumanItemFlute ' + 'where HumanID = ?;';

  FStatementGetItemProgress := Stms.AddSQLStatement('HumanSelectItemProgress');
  FStatementGetItemProgress.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'IsOpen,' + 'NameColor,' + 'Count,' + 'ShowType,' + 'Max,' + '`Value`,' + 'Level,' + 'Name ' + 'from HumanItemProgress ' + 'where HumanID = ?;';

  FStatementGetItemProperty := Stms.AddSQLStatement('HumanSelectItemProperty');
  FStatementGetItemProperty.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Color,' + 'BindType,' + 'ShowFlag,' + 'IsPercent,' + 'HintModule,' + '`Value`,' + '`Value2`,' + '`Value3` ' + 'from HumanItemProperty ' + 'where HumanID = ?;';

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
  FStatementInsertAbil.Sql := 'replace into HumanAbil(' + 'HumanID, ' + 'AC1, ' + 'AC2, ' + 'MAC1, ' + 'MAC2, ' + 'DC1, ' + 'DC2, ' + 'MC1, ' + 'MC2, ' + 'SC1, ' + 'SC2, ' + 'HP, ' + 'MaxHP,' + 'MP, ' + 'MaxMP, ' + 'Exp, ' + 'MaxExp, ' + 'Weight, ' + 'MaxWeight, ' + 'WearWeight, ' + 'MaxWearWeight, ' + 'HandWeight, ' + 'MaxHandWeight,' + 'AdjustAbilPoint, ' + 'AdjustAbilDC, ' + 'AdjustAbilMC, ' + 'AdjustAbilSC, ' + 'AdjustAbilAC, ' + 'AdjustAbilMAC, ' + 'AdjustAbilHP,' + 'AdjustAbilMP, ' + 'AdjustAbilHit, ' + 'AdjustAbilSpeed, ' + 'AdjustAbilMaxRate) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertAbilNG := Stms.AddSQLStatement('HumanInsertAbilNG');
  FStatementInsertAbilNG.Sql := 'replace into HumanAbilNG(' + 'HumanID, ' + 'IsTrainingNG, ' + 'IsTrainingXF, ' + 'AbilNGLevel, ' + 'AbilNGValue, ' + 'AbilNGMaxValue, ' + 'AbilNGExp, ' + 'AbilNGMaxExp,' + 'ContinuousMagicOrder1, ' + 'ContinuousMagicOrder2, ' + 'ContinuousMagicOrder3, ' + 'IsOpenLastContinuous, ' + 'LastContinuousMagicOrder, ' + 'Meridians1Level, ' + 'Meridians1BlastHitRate1, ' + 'Meridians1Acupoints1, ' + 'Meridians1Acupoints2, ' + 'Meridians1Acupoints3, ' + 'Meridians1Acupoints4, ' +
    'Meridians1Acupoints5, ' + 'Meridians2Level, ' + 'Meridians2BlastHitRate, ' + 'Meridians2Acupoints1, ' + 'Meridians2Acupoints2, ' + 'Meridians2Acupoints3, ' + 'Meridians2Acupoints4, ' + 'Meridians2Acupoints5, ' + 'Meridians3Level, ' + 'Meridians3BlastHitRate, ' + 'Meridians3Acupoints1, ' + 'Meridians3Acupoints2, ' + 'Meridians3Acupoints3, ' + 'Meridians3Acupoints4, ' + 'Meridians3Acupoints5, ' + 'Meridians4Level, ' + 'Meridians4BlastHitRate, ' + 'Meridians4Acupoints1, ' + 'Meridians4Acupoints2, ' +
    'Meridians4Acupoints3, ' + 'Meridians4Acupoints4, ' + 'Meridians4Acupoints5, ' + 'Meridians5Level, ' + 'Meridians5BlastHitRate, ' + 'Meridians5Acupoints1, ' + 'Meridians5Acupoints2, ' + 'Meridians5Acupoints3, ' + 'Meridians5Acupoints4, ' + 'Meridians5Acupoints5) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ' + '?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertAbilWine := Stms.AddSQLStatement('HumanInsertAbilWine');
  FStatementInsertAbilWine.Sql := 'replace into HumanAbilWine(' + 'HumanID, ' + 'IsDrinkedWine, ' + 'IsDrinkWineDrunk, ' + 'DrinkWineQuality, ' + 'DrinkWineAlcohol, ' + 'AbilAlcohol, ' + 'AbilMaxAlcohol, ' + 'AbilDrinkValue, ' + 'AbilMedicineLevel, ' + 'AbilMedicineValue, ' + 'AbilMaxMedicineValue) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertAbilNpcAdd := Stms.AddSQLStatement('HumanInsertAbilNpcAdd');
  FStatementInsertAbilNpcAdd.Sql := 'insert into HumanAbilNpcAdd(' + 'HumanID, ' + '`Index`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertGamePetData := Stms.AddSQLStatement('HumanInsertGamePetData');
  FStatementInsertGamePetData.Sql := 'insert into HumanGamePetData(' + 'HumanID, ' + '`Index`, ' + 'Name, ' + 'Level, ' + 'HP, ' + 'MP, ' + 'Exp, ' + 'Magic1, ' + 'Magic2, ' + 'Magic3, ' + 'Magic4, ' + 'Magic5, ' + 'Magic6, ' + 'Magic7, ' + 'Magic8) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertGodBlessState := Stms.AddSQLStatement('HumanInsertGodBlessState');
  FStatementInsertGodBlessState.Sql := 'insert into HumanGodBlessState(' + 'HumanID, ' + '`Index`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertMagic := Stms.AddSQLStatement('HumanInsertMagic');
  FStatementInsertMagic.Sql := 'insert into HumanMagic(' + 'HumanID, ' + 'MagicType, ' + 'MagicIndex, ' + 'MagicID,' + 'MagicAttr, ' + 'MagicLevel, ' + 'MagicNewLevel, ' + 'MagicKey, ' + 'MagicTranPoint, ' + 'MagicIsUseItemAdd) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertMagicUseTick := Stms.AddSQLStatement('HumanInsertMagicUseTick');
  FStatementInsertMagicUseTick.Sql := 'insert into HumanMagicUseTick(' + 'HumanID, ' + 'MagicID, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertStatusTime := Stms.AddSQLStatement('HumanInsertStatusTime');
  FStatementInsertStatusTime.Sql := 'insert into HumanStatusTime(' + 'HumanID, ' + '`Index`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertQuestFlag := Stms.AddSQLStatement('HumanInsertQuestFlag');
  FStatementInsertQuestFlag.Sql := 'insert into HumanQuestFlag(' + 'HumanID, ' + '`Index`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertVariableU := Stms.AddSQLStatement('HumanInsertVariableU');
  FStatementInsertVariableU.Sql := 'insert into HumanVariableU(' + 'HumanID, ' + '`Index`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertVariableT := Stms.AddSQLStatement('HumanInsertVariableT');
  FStatementInsertVariableT.Sql := 'insert into HumanVariableT(' + 'HumanID, ' + '`Index`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertVariableJ := Stms.AddSQLStatement('HumanInsertVariableJ');
  FStatementInsertVariableJ.Sql := 'insert into HumanVariableJ(' + 'HumanID, ' + '`Index`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertVariableZ := Stms.AddSQLStatement('HumanInsertVariableZ');
  FStatementInsertVariableZ.Sql := 'insert into HumanVariableZ(' + 'HumanID, ' + '`Index`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertCustomMoney := Stms.AddSQLStatement('HumanInsertCustomMoney');
  FStatementInsertCustomMoney.Sql := 'insert into HumanMoney(' + 'HumanID, ' + '`MoneyName`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertItems := Stms.AddSQLStatement('HumanInsertItems');
  FStatementInsertItems.Sql := 'insert into HumanItems(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'MakeIndex, ' + 'DBIndex, ' + 'Name, ' + 'Dura, ' + 'DuraMax, ' + 'HeroM2DressEffect, ' + 'UpgradeCount, ' + 'IsStartTime, ' + 'LimitTime, ' + 'HeroM2Light, ' + 'Color, ' + 'IsBind, ' + 'BindOption, ' + 'Effect, ' + 'NewLooks, ' + 'NewShape, ' + 'FluteCount, ' + 'PropertyText, ' + 'PropertyTextColor, ' + 'ItemFrom, ' + 'ItemFromMap, ' + 'ItemFromMon, ' + 'ItemFromMaker, ' + 'ItemFromDate, ' + 'InsuranceCount, ' + 'NewExpand3, ' + 'NewExpand4 ' + ') ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ' + '?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertItemValueAdd := Stms.AddSQLStatement('HumanInsertItemValueAdd');
  FStatementInsertItemValueAdd.Sql := 'insert into HumanItemValueAdd(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + '`Value`) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemElementAdd := Stms.AddSQLStatement('HumanInsertItemElementAdd');
  FStatementInsertItemElementAdd.Sql := 'insert into HumanItemElementAdd(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + '`Value`) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemAddDataByte := Stms.AddSQLStatement('HumanInsertItemAddDataByte');
  FStatementInsertItemAddDataByte.Sql := 'insert into HumanItemAddDataByte(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + '`Value`) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemAddDataInt := Stms.AddSQLStatement('HumanInsertItemAddDataInt');
  FStatementInsertItemAddDataInt.Sql := 'insert into HumanItemAddDataInt(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + '`Value`) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemAddDataText := Stms.AddSQLStatement('HumanInsertItemAddDataText');
  FStatementInsertItemAddDataText.Sql := 'insert into HumanItemAddDataText(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + '`Value`) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemFlute := Stms.AddSQLStatement('HumanInsertItemFlute');
  FStatementInsertItemFlute.Sql := 'insert into HumanItemFlute(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + '`Value`,' + 'OverlapCount) ' + 'values(?, ?, ?, ?, ?, ?);';

  FStatementInsertItemProgress := Stms.AddSQLStatement('HumanInsertItemProgress');
  FStatementInsertItemProgress.Sql := 'insert into HumanItemProgress(' + 'HumanID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'IsOpen, ' + 'NameColor, ' + 'Count, ' + 'ShowType, ' + 'Max, ' + '`Value`, ' + 'Level, ' + 'Name) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

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
  FStatementGetCustomMoney.Sql := 'select ' + '`MoneyName`,' + '`Value` ' + 'from HumanMoney ' + 'where HumanID = ?;';

  FStatementGetCustomMoneyByName := Stms.AddSQLStatement('GetCustomMoneyByName');
  FStatementGetCustomMoneyByName.Sql := 'select ' + '`Value` ' + 'from HumanMoney ' + 'where HumanID = ? and MoneyName = ?;';

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

procedure TMySqlHumanDB.DoFinal;
begin
  inherited;

  if FStatementGetID <> nil then
    FStatementGetID.Finalize;
  if FStatementCheckHumanExists <> nil then
    FStatementCheckHumanExists.Finalize;
  if FStatementGetHumanCount <> nil then
    FStatementGetHumanCount.Finalize;
  if FStatementGetOtherHumanName <> nil then
    FStatementGetOtherHumanName.Finalize;
  if FStatementGetHumanHeroName <> nil then
    FStatementGetHumanHeroName.Finalize;
  if FStatementGetHumanHeroID <> nil then
    FStatementGetHumanHeroID.Finalize;
  if FStatementGetHumanGoldInfo <> nil then
    FStatementGetHumanGoldInfo.Finalize;
  if FStatementUpdateHumanGoldInfo <> nil then
    FStatementUpdateHumanGoldInfo.Finalize;
  if FStatementGetHumanBaseInfo <> nil then
    FStatementGetHumanBaseInfo.Finalize;

  if FStatementQueryHumans <> nil then
    FStatementQueryHumans.Finalize;

  if FStatementSearchByAccountMatchComplete <> nil then
    FStatementSearchByAccountMatchComplete.Finalize;
  if FStatementSearchByAccountMatchFuzzy <> nil then
    FStatementSearchByAccountMatchFuzzy.Finalize;

  if FStatementSearchByNameMatchComplete <> nil then
    FStatementSearchByNameMatchComplete.Finalize;
  if FStatementSearchByNameMatchFuzzy <> nil then
    FStatementSearchByNameMatchFuzzy.Finalize;
  if FStatementSearchByLevel <> nil then
    FStatementSearchByLevel.Finalize;

  if FStatementGetMobileNumbers1 <> nil then
    FStatementGetMobileNumbers1.Finalize;
  if FStatementGetMobileNumbers2 <> nil then
    FStatementGetMobileNumbers2.Finalize;

  if FStatementSelect <> nil then
    FStatementSelect.Finalize;
  if FStatementUnSelect <> nil then
    FStatementUnSelect.Finalize;

  if FStatementGetHuman <> nil then
    FStatementGetHuman.Finalize;
  if FStatementGetAbil <> nil then
    FStatementGetAbil.Finalize;
  if FStatementGetAbilNG <> nil then
    FStatementGetAbilNG.Finalize;
  if FStatementGetAbilWine <> nil then
    FStatementGetAbilWine.Finalize;
  if FStatementGetAbilNpcAdd <> nil then
    FStatementGetAbilNpcAdd.Finalize;
  if FStatementGetGamePetData <> nil then
    FStatementGetGamePetData.Finalize;
  if FStatementGetGodBlessState <> nil then
    FStatementGetGodBlessState.Finalize;
  if FStatementGetMagic <> nil then
    FStatementGetMagic.Finalize;
  if FStatementGetMagicUseTick <> nil then
    FStatementGetMagicUseTick.Finalize;
  if FStatementGetStatusTime <> nil then
    FStatementGetStatusTime.Finalize;
  if FStatementGetQuestFlag <> nil then
    FStatementGetQuestFlag.Finalize;
  if FStatementGetVariableU <> nil then
    FStatementGetVariableU.Finalize;
  if FStatementGetVariableT <> nil then
    FStatementGetVariableT.Finalize;
  if FStatementGetVariableJ <> nil then
    FStatementGetVariableJ.Finalize;
  if FStatementGetVariableZ <> nil then
    FStatementGetVariableZ.Finalize;
  if FStatementGetItems <> nil then
    FStatementGetItems.Finalize;
  if FStatementGetItemValueAdd <> nil then
    FStatementGetItemValueAdd.Finalize;
  if FStatementGetItemElementAdd <> nil then
    FStatementGetItemElementAdd.Finalize;
  if FStatementGetItemAddDataByte <> nil then
    FStatementGetItemAddDataByte.Finalize;
  if FStatementGetItemAddDataInt <> nil then
    FStatementGetItemAddDataInt.Finalize;
  if FStatementGetItemAddDataText <> nil then
    FStatementGetItemAddDataText.Finalize;
  if FStatementGetItemFlute <> nil then
    FStatementGetItemFlute.Finalize;
  if FStatementGetItemProgress <> nil then
    FStatementGetItemProgress.Finalize;
  if FStatementGetItemProperty <> nil then
    FStatementGetItemProperty.Finalize;
  if FStatementGetSkillPower <> nil then
    FStatementGetSkillPower.Finalize;

  if FStatementAddHuman <> nil then
    FStatementAddHuman.Finalize;

  if FStatementDeleteOrRestore <> nil then
    FStatementDeleteOrRestore.Finalize;
  if FStatementRecordLoginTime <> nil then
    FStatementRecordLoginTime.Finalize;

  if FStatementUpdateHumanName <> nil then
    FStatementUpdateHumanName.Finalize;
  if FStatementUpdateHumanNameDearName <> nil then
    FStatementUpdateHumanNameDearName.Finalize;
  if FStatementUpdateHumanNameMasterName <> nil then
    FStatementUpdateHumanNameMasterName.Finalize;

  if FStatementUpdateHuman <> nil then
    FStatementUpdateHuman.Finalize;

  if FStatementInsertAbil <> nil then
    FStatementInsertAbil.Finalize;
  if FStatementInsertAbilNG <> nil then
    FStatementInsertAbilNG.Finalize;
  if FStatementInsertAbilWine <> nil then
    FStatementInsertAbilWine.Finalize;

  if FStatementInsertAbilNpcAdd <> nil then
    FStatementInsertAbilNpcAdd.Finalize;
  if FStatementInsertGamePetData <> nil then
    FStatementInsertGamePetData.Finalize;
  if FStatementInsertGodBlessState <> nil then
    FStatementInsertGodBlessState.Finalize;
  if FStatementInsertMagic <> nil then
    FStatementInsertMagic.Finalize;
  if FStatementInsertMagicUseTick <> nil then
    FStatementInsertMagicUseTick.Finalize;
  if FStatementInsertStatusTime <> nil then
    FStatementInsertStatusTime.Finalize;
  if FStatementInsertQuestFlag <> nil then
    FStatementInsertQuestFlag.Finalize;
  if FStatementInsertVariableU <> nil then
    FStatementInsertVariableU.Finalize;
  if FStatementInsertVariableT <> nil then
    FStatementInsertVariableT.Finalize;
  if FStatementInsertVariableJ <> nil then
    FStatementInsertVariableJ.Finalize;
  if FStatementInsertVariableZ <> nil then
    FStatementInsertVariableZ.Finalize;
  if FStatementInsertCustomMoney <> nil then
    FStatementInsertCustomMoney.Finalize;
  if FStatementInsertItems <> nil then
    FStatementInsertItems.Finalize;
  if FStatementInsertItemValueAdd <> nil then
    FStatementInsertItemValueAdd.Finalize;
  if FStatementInsertItemElementAdd <> nil then
    FStatementInsertItemElementAdd.Finalize;
  if FStatementInsertItemAddDataByte <> nil then
    FStatementInsertItemAddDataByte.Finalize;
  if FStatementInsertItemAddDataInt <> nil then
    FStatementInsertItemAddDataInt.Finalize;
  if FStatementInsertItemAddDataText <> nil then
    FStatementInsertItemAddDataText.Finalize;
  if FStatementInsertItemFlute <> nil then
    FStatementInsertItemFlute.Finalize;
  if FStatementInsertItemProgress <> nil then
    FStatementInsertItemProgress.Finalize;
  if FStatementInsertItemProperty <> nil then
    FStatementInsertItemProperty.Finalize;
  if FStatementInsertSkillPower <> nil then
    FStatementInsertSkillPower.Finalize;

  if FStatementGetLevelRankCheckLevel <> nil then
    FStatementGetLevelRankCheckLevel.Finalize;
  if FStatementGetLevelRankTopCount <> nil then
    FStatementGetLevelRankTopCount.Finalize;
  if FStatementGetLevelRankCheckLevelAndCount <> nil then
    FStatementGetLevelRankCheckLevelAndCount.Finalize;

  if FStatementGetMasterRankCheckLevel <> nil then
    FStatementGetMasterRankCheckLevel.Finalize;
  if FStatementGetMasterRankTopCount <> nil then
    FStatementGetMasterRankTopCount.Finalize;
  if FStatementGetMasterRankCheckLevelAndCount <> nil then
    FStatementGetMasterRankCheckLevelAndCount.Finalize;

  if FStatementBuyPlayer <> nil then
    FStatementBuyPlayer.Finalize;
  if FStatementGetCustomMoney <> nil then
    FStatementGetCustomMoney.Finalize;
  if FStatementGetCustomMoneyByName <> nil then
    FStatementGetCustomMoneyByName.Finalize;
  if FStatementUpdateCustomMoney <> nil then
    FStatementUpdateCustomMoney.Finalize;
end;

function TMySqlHumanDB.DoGetID(HumanName: string): Integer;
begin
  try
    FStatementGetID.Reset;
    FStatementGetID.OrderBindParamText(HumanName);
    if FStatementGetID.Query and FStatementGetID.Fetch then
      Result := FStatementGetID.GetColumnValueInt(0)
    else
      Result := NO_ID;
  finally
    FStatementGetID.Reset;
  end;
end;

function TMySqlHumanDB.DoCheckHumanExists(Account, HumanName: string): Boolean;
begin
  try
    FStatementCheckHumanExists.Reset;
    FStatementCheckHumanExists.OrderBindParamText(Account);
    FStatementCheckHumanExists.OrderBindParamText(HumanName);
    if FStatementCheckHumanExists.Query and FStatementCheckHumanExists.Fetch then
      Result := True
    else
      Result := False;
    //Result := FStatementCheckHumanExists.Step = SQLITE_ROW;
  finally
    FStatementCheckHumanExists.Reset;
  end;
end;

function TMySqlHumanDB.DoGetHumanCount(Account: string): Integer;
begin
  try
    FStatementGetHumanCount.Reset;
    FStatementGetHumanCount.OrderBindParamText(Account);
    if FStatementGetHumanCount.Query and FStatementGetHumanCount.Fetch then
      Result := FStatementGetHumanCount.GetColumnValueInt(0)
    else
      Result := 0;
  finally
    FStatementGetHumanCount.Reset;
  end;
end;

function TMySqlHumanDB.DoGetOtherHumanName(Account, HumanName: string): string;
begin
  try
    FStatementGetOtherHumanName.Reset;
    FStatementGetOtherHumanName.OrderBindParamText(Account);
    FStatementGetOtherHumanName.OrderBindParamText(HumanName);
    if FStatementGetOtherHumanName.Query and FStatementGetOtherHumanName.Fetch then
      Result := FStatementGetOtherHumanName.GetColumnValueText(0)
    else
      Result := '';
  finally
    FStatementGetOtherHumanName.Reset;
  end;
end;

function TMySqlHumanDB.DoGetHumanHeroName(Account, HumanName: string; var HeroName, DeputyHeroName: string): Boolean;
begin
  Result := False;
  try
    FStatementGetHumanHeroName.Reset;
    FStatementGetHumanHeroName.OrderBindParamText(HumanName);
    if FStatementGetHumanHeroName.Query and FStatementGetHumanHeroName.Fetch then
    begin
      Result := True;
      HeroName := FStatementGetHumanHeroName.GetColumnValueText(0);
      DeputyHeroName := FStatementGetHumanHeroName.GetColumnValueText(1);
    end;
  finally
    FStatementGetHumanHeroName.Reset;
  end;
end;

function TMySqlHumanDB.DoGetBaseInfo(HumanName: string; var Sex, Job, Level, LastLogin: Integer): Boolean;
begin
  Result := False;
  try
    FStatementGetHumanBaseInfo.Reset;
    FStatementGetHumanBaseInfo.OrderBindParamText(HumanName);
    if FStatementGetHumanBaseInfo.Query and FStatementGetHumanBaseInfo.Fetch then
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

function TMySqlHumanDB.DoQueryHumans(Account: string; HumanList: TQueryHumanList): Integer;
var
  QueryData: TQueryHumanData;
begin
  Result := 0;

  try
    FStatementQueryHumans.Reset;
    FStatementQueryHumans.OrderBindParamText(Account);
    FStatementQueryHumans.OrderBindParamInt(0);
    if FStatementQueryHumans.Query then
    begin
      while (FStatementQueryHumans.Fetch) do
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

        Inc(Result);
      end;
    end;
  finally
    FStatementQueryHumans.Reset;
  end;
end;

function TMySqlHumanDB.DoQueryDeleteHumans(Account: string; HumanList: TQueryHumanList): Integer;
var
  QueryData: TQueryHumanData;
begin
  Result := 0;

  try
    FStatementQueryHumans.Reset;
    FStatementQueryHumans.OrderBindParamText(Account);
    FStatementQueryHumans.OrderBindParamInt(1);
    if FStatementQueryHumans.Query then
    begin
      while (FStatementQueryHumans.Fetch) do
      begin
        QueryData.HumanName := FStatementQueryHumans.OrderGetColumnValueText;
        QueryData.IsSelect := FStatementQueryHumans.OrderGetColumnValueBool;
        QueryData.Sex := FStatementQueryHumans.OrderGetColumnValueInt;
        QueryData.Job := FStatementQueryHumans.OrderGetColumnValueInt;
        QueryData.Hair := FStatementQueryHumans.OrderGetColumnValueInt;
        QueryData.Level := FStatementQueryHumans.OrderGetColumnValueInt;
        HumanList.Add(@QueryData);

        Inc(Result);
      end;
    end;
  finally
    FStatementQueryHumans.Reset;
  end;
end;

function TMySqlHumanDB.DoSearchByAccount(Account: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer;
var
  SearchData: TSerarchRoleData;
begin
  Result := 0;
  if MatchType = smtComplete then
  begin
    try
      FStatementSearchByAccountMatchComplete.Reset;
      FStatementSearchByAccountMatchComplete.OrderBindParamText(Account);
      if FStatementSearchByAccountMatchComplete.Query then
      begin
        while (FStatementSearchByAccountMatchComplete.Fetch) do
        begin
          SearchData.Account := FStatementSearchByAccountMatchComplete.OrderGetColumnValueText;
          SearchData.RoleName := FStatementSearchByAccountMatchComplete.OrderGetColumnValueText;
          SearchData.IsDelete := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
          SearchData.Sex := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
          SearchData.Job := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
          SearchData.Level := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
          SearchData.IsHero := False;
          RoleList.Add(@SearchData);

          Inc(Result);
        end;
      end;
    finally
      FStatementSearchByAccountMatchComplete.Reset;
    end;
  end
  else
  begin
    try
      FStatementSearchByAccountMatchFuzzy.Reset;
      FStatementSearchByAccountMatchFuzzy.OrderBindParamText('%' + Account + '%');
      if FStatementSearchByAccountMatchFuzzy.Query then
      begin
        while (FStatementSearchByAccountMatchFuzzy.Fetch) do
        begin
          SearchData.Account := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueText;
          SearchData.RoleName := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueText;
          SearchData.IsDelete := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
          SearchData.Sex := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
          SearchData.Job := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
          SearchData.Level := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
          SearchData.IsHero := False;
          RoleList.Add(@SearchData);

          Inc(Result);
        end;
      end;
    finally
      FStatementSearchByAccountMatchFuzzy.Reset;
    end;
  end;
end;

function TMySqlHumanDB.DoSearchByName(HumanName: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer;
var
  SearchData: TSerarchRoleData;
begin
  Result := 0;

  if MatchType = smtComplete then
  begin
    try
      FStatementSearchByNameMatchComplete.Reset;
      FStatementSearchByNameMatchComplete.OrderBindParamText(HumanName);
      if FStatementSearchByNameMatchComplete.Query then
      begin
        while (FStatementSearchByNameMatchComplete.Fetch) do
        begin
          SearchData.Account := FStatementSearchByNameMatchComplete.OrderGetColumnValueText;
          SearchData.RoleName := FStatementSearchByNameMatchComplete.OrderGetColumnValueText;
          SearchData.IsDelete := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
          SearchData.Sex := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
          SearchData.Job := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
          SearchData.Level := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
          SearchData.IsHero := False;
          RoleList.Add(@SearchData);

          Inc(Result);
        end;
      end;
    finally
      FStatementSearchByNameMatchComplete.Reset;
    end;
  end
  else
  begin
    try
      FStatementSearchByNameMatchFuzzy.Reset;
      FStatementSearchByNameMatchFuzzy.OrderBindParamText('%' + HumanName + '%');
      if FStatementSearchByNameMatchFuzzy.Query then
      begin
        while (FStatementSearchByNameMatchFuzzy.Fetch) do
        begin
          SearchData.Account := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueText;
          SearchData.RoleName := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueText;
          SearchData.IsDelete := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
          SearchData.Sex := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
          SearchData.Job := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
          SearchData.Level := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
          SearchData.IsHero := False;
          RoleList.Add(@SearchData);

          Inc(Result);
        end;
      end;
    finally
      FStatementSearchByNameMatchFuzzy.Reset;
    end;
  end;
end;

function TMySqlHumanDB.DoSearchByLevel(LimitCount, MinLevel: Integer; RoleList: TSerarchRoleList): Integer;
var
  SearchData: TSerarchRoleData;
begin
  try
    FStatementSearchByLevel.Reset;
    FStatementSearchByLevel.OrderBindParamInt(MinLevel);
    FStatementSearchByLevel.OrderBindParamInt(LimitCount);
    if FStatementSearchByLevel.Query then
    begin
      while (FStatementSearchByLevel.Fetch) do
      begin
        SearchData.Account := FStatementSearchByLevel.OrderGetColumnValueText;
        SearchData.RoleName := FStatementSearchByLevel.OrderGetColumnValueText;
        SearchData.IsDelete := FStatementSearchByLevel.OrderGetColumnValueInt;
        SearchData.Sex := FStatementSearchByLevel.OrderGetColumnValueInt;
        SearchData.Job := FStatementSearchByLevel.OrderGetColumnValueInt;
        SearchData.Level := FStatementSearchByLevel.OrderGetColumnValueInt;
        SearchData.IsHero := False;
        RoleList.Add(@SearchData);

        Inc(Result);
      end;
    end;
  finally
    FStatementSearchByNameMatchFuzzy.Reset;
  end;
end;

function TMySqlHumanDB.DoGetMobileNumbers(OnlyBindMobile: Boolean; MobileNumberList: TStrings): Integer;
begin
  if not OnlyBindMobile then
  begin
    try
      FStatementGetMobileNumbers1.Reset;
      if FStatementGetMobileNumbers1.Query then
      begin
        while (FStatementGetMobileNumbers1.Fetch) do
        begin
          MobileNumberList.Add(FStatementGetMobileNumbers1.OrderGetColumnValueText);
          Inc(Result);
        end;
      end;
    finally
      FStatementGetMobileNumbers1.Reset;
    end;
  end
  else
  begin
    try
      FStatementGetMobileNumbers2.Reset;
      if FStatementGetMobileNumbers2.Query then
      begin
        while (FStatementGetMobileNumbers2.Fetch) do
        begin
          MobileNumberList.Add(FStatementGetMobileNumbers2.OrderGetColumnValueText);
          Inc(Result);
        end;
      end;
    finally
      FStatementGetMobileNumbers2.Reset;
    end;
  end;
end;

function TMySqlHumanDB.DoSelect(Account, HumanName: string): Boolean;
begin
  Result := False;
  BeginTransaction;
  try
    try
      FStatementSelect.Reset;
      FStatementSelect.OrderBindParamText(Account);
      FStatementSelect.OrderBindParamText(HumanName);
      Result := FStatementSelect.Step;

      FStatementUnSelect.Reset;
      FStatementUnSelect.OrderBindParamText(Account);
      FStatementUnSelect.OrderBindParamText(HumanName);
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

function TMySqlHumanDB.DoGet(Account, HumanName: string; var HumData: THumData; var HumanID: Integer): Boolean;
var
  I, J, TheType, Index, Index2, Value: Integer;
  GamePetData: pTGamePetData;
  Magic: PTHumMagic;
  UserItem: pTUserItem;
  sTemp: string;
begin
  Result := False;

  try
    FStatementGetHuman.Reset;
    FStatementGetHuman.OrderBindParamText(Account);
    FStatementGetHuman.OrderBindParamText(HumanName);
    if (FStatementGetHuman.Query and FStatementGetHuman.Fetch) then
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
    FStatementGetAbil.OrderBindParamInt(HumanID);
    if (FStatementGetAbil.Query and FStatementGetAbil.Fetch) then
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
    FStatementGetAbilNG.OrderBindParamInt(HumanID);
    if (FStatementGetAbilNG.Query and FStatementGetAbilNG.Fetch) then
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
    FStatementGetAbilWine.OrderBindParamInt(HumanID);
    if (FStatementGetAbilWine.Query and FStatementGetAbilWine.Fetch) then
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
    FStatementGetAbilNpcAdd.OrderBindParamInt(HumanID);
    if FStatementGetAbilNpcAdd.Query then
    begin
      while (FStatementGetAbilNpcAdd.Fetch) do
      begin
        Index := FStatementGetAbilNpcAdd.OrderGetColumnValueInt;
        Value := FStatementGetAbilNpcAdd.OrderGetColumnValueInt;

        if (Index >= Low(HumData.AddSaveAbil)) and (Index <= High(HumData.AddSaveAbil)) then
          HumData.AddSaveAbil[Index] := Value;
      end;
    end;

    FStatementGetGamePetData.Reset;
    FStatementGetGamePetData.OrderBindParamInt(HumanID);
    if FStatementGetGamePetData.Query then
    begin
      while (FStatementGetGamePetData.Fetch) do
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
      end;
    end;

    FStatementGetGodBlessState.Reset;
    FStatementGetGodBlessState.OrderBindParamInt(HumanID);
    if FStatementGetGodBlessState.Query then
    begin
      while (FStatementGetGodBlessState.Fetch) do
      begin
        Index := FStatementGetGodBlessState.OrderGetColumnValueInt;
        Value := FStatementGetGodBlessState.OrderGetColumnValueInt;

        if (Index >= Low(HumData.GodBlessItemsState)) and (Index <= High(HumData.GodBlessItemsState)) then
          HumData.GodBlessItemsState[Index] := Value;
      end;
    end;

    FStatementGetMagic.Reset;
    FStatementGetMagic.OrderBindParamInt(HumanID);
    if FStatementGetMagic.Query then
    begin
      while (FStatementGetMagic.Fetch) do
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
      end;
    end;

    FStatementGetMagicUseTick.Reset;
    FStatementGetMagicUseTick.OrderBindParamInt(HumanID);
    if FStatementGetMagicUseTick.Query then
    begin
      while (FStatementGetMagicUseTick.Fetch) do
      begin
        Index := FStatementGetMagicUseTick.OrderGetColumnValueInt - CUSTOM_MAGIC_START_ID;
        Value := FStatementGetMagicUseTick.OrderGetColumnValueInt;

        if (Index >= Low(HumData.CustomSkillUseTicks)) and (Index <= High(HumData.CustomSkillUseTicks)) then
          HumData.CustomSkillUseTicks[Index] := Value;
      end;
    end;

    FStatementGetStatusTime.Reset;
    FStatementGetStatusTime.OrderBindParamInt(HumanID);
    if FStatementGetStatusTime.Query then
    begin
      while (FStatementGetStatusTime.Fetch) do
      begin
        Index := FStatementGetStatusTime.OrderGetColumnValueInt;
        Value := FStatementGetStatusTime.OrderGetColumnValueInt;

        if (Index >= Low(HumData.wStatusTimeArr)) and (Index <= High(HumData.wStatusTimeArr)) then
          HumData.wStatusTimeArr[Index] := Value;
      end;
    end;

    FStatementGetQuestFlag.Reset;
    FStatementGetQuestFlag.OrderBindParamInt(HumanID);
    if FStatementGetQuestFlag.Query then
    begin
      while (FStatementGetQuestFlag.Fetch) do
      begin
        Index := FStatementGetQuestFlag.OrderGetColumnValueInt;
        Value := FStatementGetQuestFlag.OrderGetColumnValueInt;

        if (Index >= Low(HumData.QuestFlag)) and (Index <= High(HumData.QuestFlag)) then
          HumData.QuestFlag[Index] := Value;
      end;
    end;

    FStatementGetVariableU.Reset;
    FStatementGetVariableU.OrderBindParamInt(HumanID);
    if FStatementGetVariableU.Query then
    begin
      while (FStatementGetVariableU.Fetch) do
      begin
        Index := FStatementGetVariableU.OrderGetColumnValueInt;
        Value := FStatementGetVariableU.OrderGetColumnValueInt;

        if (Index >= Low(HumData.UValues)) and (Index <= High(HumData.UValues)) then
          HumData.UValues[Index] := Value;
      end;
    end;

    FStatementGetVariableT.Reset;
    FStatementGetVariableT.OrderBindParamInt(HumanID);
    if FStatementGetVariableT.Query then
    begin
      while (FStatementGetVariableT.Fetch) do
      begin
        Index := FStatementGetVariableT.OrderGetColumnValueInt;
        if (Index >= Low(HumData.TValues)) and (Index <= High(HumData.TValues)) then
          HumData.TValues[Index] := FStatementGetVariableT.OrderGetColumnValueText;
      end;
    end;

    FStatementGetVariableJ.Reset;
    FStatementGetVariableJ.OrderBindParamInt(HumanID);
    if FStatementGetVariableJ.Query then
    begin
      while (FStatementGetVariableJ.Fetch) do
      begin
        Index := FStatementGetVariableJ.OrderGetColumnValueInt;
        if (Index >= Low(HumData.JValues)) and (Index <= High(HumData.JValues)) then
          HumData.JValues[Index] := FStatementGetVariableJ.OrderGetColumnValueInt;
      end;
    end;

    FStatementGetVariableZ.Reset;
    FStatementGetVariableZ.OrderBindParamInt(HumanID);
    if FStatementGetVariableZ.Query then
    begin
      while (FStatementGetVariableZ.Fetch) do
      begin
        Index := FStatementGetVariableZ.OrderGetColumnValueInt;
        if (Index >= Low(HumData.ZValues)) and (Index <= High(HumData.ZValues)) then
          HumData.ZValues[Index] := FStatementGetVariableZ.OrderGetColumnValueText;
      end;
    end;

    FStatementGetItems.Reset;
    FStatementGetItems.OrderBindParamInt(HumanID);
    if FStatementGetItems.Query then
    begin
      while (FStatementGetItems.Fetch) do
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
      end;
    end;

    FStatementGetItemValueAdd.Reset;
    FStatementGetItemValueAdd.OrderBindParamInt(HumanID);
    if FStatementGetItemValueAdd.Query then
    begin
      while (FStatementGetItemValueAdd.Fetch) do
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
      end;
    end;

    FStatementGetItemElementAdd.Reset;
    FStatementGetItemElementAdd.OrderBindParamInt(HumanID);
    if FStatementGetItemElementAdd.Query then
    begin
      while (FStatementGetItemElementAdd.Fetch) do
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
      end;
    end;

    FStatementGetItemAddDataByte.Reset;
    FStatementGetItemAddDataByte.OrderBindParamInt(HumanID);
    if FStatementGetItemAddDataByte.Query then
    begin
      while (FStatementGetItemAddDataByte.Fetch) do
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
      end;
    end;

    FStatementGetItemAddDataInt.Reset;
    FStatementGetItemAddDataInt.OrderBindParamInt(HumanID);
    if FStatementGetItemAddDataInt.Query then
    begin
      while (FStatementGetItemAddDataInt.Fetch) do
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
      end;
    end;

    FStatementGetItemAddDataText.Reset;
    FStatementGetItemAddDataText.OrderBindParamInt(HumanID);
    if FStatementGetItemAddDataText.Query then
    begin
      while (FStatementGetItemAddDataText.Fetch) do
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
      end;
    end;

    FStatementGetItemFlute.Reset;
    FStatementGetItemFlute.OrderBindParamInt(HumanID);
    if FStatementGetItemFlute.Query then
    begin
      while (FStatementGetItemFlute.Fetch) do
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
      end;
    end;

    FStatementGetItemProgress.Reset;
    FStatementGetItemProgress.OrderBindParamInt(HumanID);
    if FStatementGetItemProgress.Query then
    begin
      while (FStatementGetItemProgress.Fetch) do
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
      end;
    end;

    FStatementGetItemProperty.Reset;
    FStatementGetItemProperty.OrderBindParamInt(HumanID);
    if FStatementGetItemProperty.Query then
    begin
      while (FStatementGetItemProperty.Fetch) do
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
      end;
    end;

    FStatementGetSkillPower.Reset;
    FStatementGetSkillPower.OrderBindParamInt(HumanID);
    if FStatementGetSkillPower.Query then
    begin
      while FStatementGetSkillPower.Fetch do
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
      end;
    end;

    FStatementGetCustomMoney.Reset;
    FStatementGetCustomMoney.OrderBindParamInt(HumanID);
    Index := 0;
    if FStatementGetCustomMoney.Query then
    begin
      while (FStatementGetCustomMoney.Fetch) do
      begin
        HumData.CustomMoney[Index].sName := FStatementGetCustomMoney.OrderGetColumnValueText;
        HumData.CustomMoney[Index].nCount := FStatementGetCustomMoney.OrderGetColumnValueInt;
        Inc(Index);
      end;
    end;

  finally
    ResetAllGetDataStatement;
  end;
end;

function TMySqlHumanDB.GetHumanUserItem(HumData: PTHumData; const ItemType, ItemIndex: Integer): PTUserItem;
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

function TMySqlHumanDB.DoAdd(Account, HumanName: string; IsSelect: Boolean; Sex, Job, Hair: Byte): Boolean;
begin
  try
    FStatementAddHuman.Reset;
    FStatementAddHuman.OrderBindParamText(Account);
    FStatementAddHuman.OrderBindParamText(HumanName);
    FStatementAddHuman.OrderBindParamBool(False);
    FStatementAddHuman.OrderBindParamBool(IsSelect);
    FStatementAddHuman.OrderBindParamInt(Date2MyDate(Now()));
    FStatementAddHuman.OrderBindParamInt(Sex);
    FStatementAddHuman.OrderBindParamInt(Job);
    FStatementAddHuman.OrderBindParamInt(Hair);
    Result := FStatementAddHuman.Step;
  finally
    FStatementAddHuman.Reset;
  end;
end;

function TMySqlHumanDB.DoDelete(Account, HumanName: string): Boolean;
begin
  try
    FStatementDeleteOrRestore.Reset;
    FStatementDeleteOrRestore.OrderBindParamInt(1);
    FStatementDeleteOrRestore.OrderBindParamText(Account);
    FStatementDeleteOrRestore.OrderBindParamText(HumanName);
    Result := FStatementDeleteOrRestore.Step;
  finally
    FStatementDeleteOrRestore.Reset;
  end;
end;

function TMySqlHumanDB.DoDeleteRestore(Account, HumanName: string): Boolean;
begin
  try
    FStatementDeleteOrRestore.Reset;
    FStatementDeleteOrRestore.OrderBindParamInt(0);
    FStatementDeleteOrRestore.OrderBindParamText(Account);
    FStatementDeleteOrRestore.OrderBindParamText(HumanName);
    Result := FStatementDeleteOrRestore.Step;
  finally
    FStatementDeleteOrRestore.Reset;
  end;
end;

function TMySqlHumanDB.DoSetEnabled(Account, HumanName: string; Enabled: Integer): Boolean;
begin
  try
    FStatementDeleteOrRestore.Reset;
    FStatementDeleteOrRestore.OrderBindParamInt(Enabled);
    FStatementDeleteOrRestore.OrderBindParamText(Account);
    FStatementDeleteOrRestore.OrderBindParamText(HumanName);
    Result := FStatementDeleteOrRestore.Step;
  finally
    FStatementDeleteOrRestore.Reset;
  end;
end;

function TMySqlHumanDB.DoErase(Account, HumanName: string): Boolean;
var
  HumanID, HeroID: Integer;
  strHeroID: string;
begin
  Result := False;
  HumanID := GetID(HumanName);
  if HumanID = NO_ID then
    Exit;

  strHeroID := '';
  try
    FStatementGetHumanHeroID.Reset;
    FStatementGetHumanHeroID.OrderBindParamInt(HumanID);
    if FStatementGetHumanHeroID.Query then
    begin
      while (FStatementGetHumanHeroID.Fetch) do
      begin
        HeroID := FStatementGetHumanHeroID.OrderGetColumnValueInt;
        strHeroID := strHeroID + IntToStr(HeroID) + ',';
      end;
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
      Execute(Format('delete from HeroItemAddDataByte where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroItemAddDataInt where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroItemAddDataText where HeroID in (%s);', [strHeroID]));

      Execute(Format('delete from HeroItemFlute where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroItemProgress where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroItemProperty where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroItemValueAdd where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroItems where HeroID in (%s);', [strHeroID]));

      Execute(Format('delete from HeroAbil where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroAbilNG where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroAbilNpcAdd where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroAbilWine where HeroID in (%s);', [strHeroID]));

      Execute(Format('delete from HeroGodBlessState where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroMagic where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroQuestFlag where HeroID in (%s);', [strHeroID]));
      Execute(Format('delete from HeroStatusTime where HeroID in (%s);', [strHeroID]));

      Execute(Format('delete from HeroSkillPower where HeroID in (%s);', [strHeroID]));

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

function TMySqlHumanDB.DoRecordLoginTime(Account, HumanName: string): Boolean;
begin
  try
    FStatementRecordLoginTime.Reset;
    FStatementRecordLoginTime.OrderBindParamInt(Date2MyDate(Now()));
    FStatementRecordLoginTime.OrderBindParamText(Account);
    FStatementRecordLoginTime.OrderBindParamText(HumanName);
    Result := FStatementRecordLoginTime.Step;
  finally
    FStatementRecordLoginTime.Reset;
  end;
end;

function TMySqlHumanDB.DoSave(HumanID: Integer; HumData: PTHumData): Boolean;
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

function TMySqlHumanDB.DoRename(Account, HumanName: string; HumanID: Integer; NewName: string): Boolean;
begin
  Result := False;
  BeginTransaction;
  try
    try
      FStatementUpdateHumanName.Reset;
      FStatementUpdateHumanName.OrderBindParamText(NewName);
      FStatementUpdateHumanName.OrderBindParamInt(HumanID);
      Result := FStatementUpdateHumanName.Step;

      if Result then
      begin
        FStatementUpdateHumanNameDearName.Reset;
        FStatementUpdateHumanNameDearName.OrderBindParamText(NewName);
        FStatementUpdateHumanNameDearName.OrderBindParamText(HumanName);
        Result := FStatementUpdateHumanNameDearName.Step;

        FStatementUpdateHumanNameMasterName.Reset;
        FStatementUpdateHumanNameMasterName.OrderBindParamText(NewName);
        FStatementUpdateHumanNameMasterName.OrderBindParamText(HumanName);
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

function TMySqlHumanDB.DoChangedCustomMoney(HumanID: Integer; CustomMoneyName: string; ChangedValue: Integer; var ResultValue: LongWord): Boolean;
var
  nValue: LongWord;
  I64: Int64;
begin
  try
    FStatementGetCustomMoneyByName.Reset;
    FStatementGetCustomMoneyByName.OrderBindParamInt(HumanID); 
    FStatementGetCustomMoneyByName.OrderBindParamText(CustomMoneyName);

    if FStatementGetCustomMoneyByName.Query and FStatementGetCustomMoneyByName.Fetch then
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
        FStatementUpdateCustomMoney.OrderBindParamInt(nValue);
        FStatementUpdateCustomMoney.OrderBindParamInt(HumanID);   
        FStatementUpdateCustomMoney.OrderBindParamText(CustomMoneyName);
        Result := FStatementUpdateCustomMoney.Step;
      finally
        FStatementUpdateCustomMoney.Reset;
      end;
    end;
  finally
    FStatementUpdateCustomMoney.Reset;
  end;
end;

function TMySqlHumanDB.DoChangedGold(HumanName: string; ChangeType: TDBChangeGoldType; ChangedValue: Integer; var ResultValue: LongWord): Boolean;
var
  Gold, GameGold, GamePoint, GameDiamond, GameGird: LongWord;
  I64: Int64;
begin
  try
    FStatementGetHumanGoldInfo.Reset;
    FStatementGetHumanGoldInfo.OrderBindParamText(HumanName);

    if FStatementGetHumanGoldInfo.Query and FStatementGetHumanGoldInfo.Fetch then
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
        FStatementUpdateHumanGoldInfo.OrderBindParamInt(Gold);
        FStatementUpdateHumanGoldInfo.OrderBindParamInt(GameGold);
        FStatementUpdateHumanGoldInfo.OrderBindParamInt(GamePoint);
        FStatementUpdateHumanGoldInfo.OrderBindParamInt(GameDiamond);
        FStatementUpdateHumanGoldInfo.OrderBindParamInt(GameGird);
        FStatementUpdateHumanGoldInfo.OrderBindParamText(HumanName);
        Result := FStatementUpdateHumanGoldInfo.Step;
      finally
        FStatementUpdateHumanGoldInfo.Reset;
      end;
    end;
  finally
    FStatementGetHumanGoldInfo.Reset;
  end;
end;

function TMySqlHumanDB.SaveHumanData(HumanID: Integer; HumData: PTHumData): Boolean;
var
  I, J: Integer;
  HumMagic: PTHumMagic;
  UserItem: PTUserItem;
begin
  Result := False;
  try
    FStatementUpdateHuman.Reset;
    FStatementUpdateHuman.OrderBindParamInt(HumData.btSex);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btJob);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btHair);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btDir);
    FStatementUpdateHuman.OrderBindParamInt(HumData.Abil.Level);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btReLevel);
    FStatementUpdateHuman.OrderBindParamText(HumData.sCurMap);
    FStatementUpdateHuman.OrderBindParamInt(HumData.wCurX);
    FStatementUpdateHuman.OrderBindParamInt(HumData.wCurY);
    FStatementUpdateHuman.OrderBindParamText(HumData.sHomeMap);
    FStatementUpdateHuman.OrderBindParamInt(HumData.wHomeX);
    FStatementUpdateHuman.OrderBindParamInt(HumData.wHomeY);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btAttackMode);
    FStatementUpdateHuman.OrderBindParamText(HumData.sStoragePwd);
    FStatementUpdateHuman.OrderBindParamInt(HumData.Abil.CreditPoint);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nGold);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nGameGold);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nGamePoint);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nGameDiamond);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nGameGird);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nGameGoldEx);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nGameGlory);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nPKPoint);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nPayMentPoint);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nMemberType);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nMemberLevel);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boMaster);
    FStatementUpdateHuman.OrderBindParamText(HumData.sMasterName);
    FStatementUpdateHuman.OrderBindParamInt(HumData.wMasterCount);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btMarryCount);
    FStatementUpdateHuman.OrderBindParamText(HumData.sDearName);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btIncHealth);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btIncSpell);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btIncHealing);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btFightZoneDieCount);
    FStatementUpdateHuman.OrderBindParamDouble(HumData.dBodyLuck);
    FStatementUpdateHuman.OrderBindParamInt(HumData.wContribution);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nHungerStatus);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nKickCount);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boLockLogin);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boAllowGroup);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boAllowGroupReCall);
    FStatementUpdateHuman.OrderBindParamInt(HumData.wGroupRecallTime);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boAllowGuildReCall);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boDisableTrading);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boDisableInviteHorseRiding);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boGameGoldTrading);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boNewServer);

    //FStatementUpdateHuman.OrderBindParamBool(HumData.boFilterGlobalMsg);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boFilterGlobalDropItemMsg);           // 过滤掉落提示信息 chongchong 2017-04-16
    FStatementUpdateHuman.OrderBindParamBool(HumData.boFilterGlobalCenterMsg);             // 过滤SendCenterMsg chongchong 2017-04-16
    FStatementUpdateHuman.OrderBindParamBool(HumData.boFilterGolbalSendMsg);               // 过滤SendMsg全局信息 chongchong 2017-04-16
    //FStatementUpdateHuman.OrderBindParamBool(HumData.boFixedHero);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boStorageHero);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boStorageDeputyHero);
    //FStatementUpdateHuman.OrderBindParamText(HumData.sHeroName);
    //FStatementUpdateHuman.OrderBindParamText(HumData.sDeputyHeroName);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btDeputyHeroJob);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btNation);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nNationCredit);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nRevivalTime);
    FStatementUpdateHuman.OrderBindParamInt(HumData.dwInfinityStorageExtCount);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boSaveKillMonExpRate);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nKillMonExpRate);
    FStatementUpdateHuman.OrderBindParamInt(HumData.dwKillMonExpRateTime);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boAttackHumSavePowerRate);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nAttackHumPowerRate);
    FStatementUpdateHuman.OrderBindParamInt(HumData.dwAttackHumPowerRateTime);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boAttackMonSavePowerRate);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nAttackMonPowerRate);
    FStatementUpdateHuman.OrderBindParamInt(HumData.dwAttackMonPowerRateTime);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boSaveKillMonBurstRate);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nKillMonBurstRate);
    FStatementUpdateHuman.OrderBindParamInt(HumData.dwKillMonBurstRateTime);
    FStatementUpdateHuman.OrderBindParamInt(HumData.dwFBCreateTime);
    FStatementUpdateHuman.OrderBindParamInt(Integer(HumData.JewelryBoxStatus));
    FStatementUpdateHuman.OrderBindParamBool(HumData.boShowFashion);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boShowGodBless);
    FStatementUpdateHuman.OrderBindParamInt(HumData.nActiveFengHao);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boStorageOpen[1]);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boStorageOpen[2]);
    FStatementUpdateHuman.OrderBindParamBool(HumData.boStorageOpen[3]);

    FStatementUpdateHuman.OrderBindParamInt(HumData.btExtBagPageCount);
    FStatementUpdateHuman.OrderBindParamInt(HumData.btExtBagOpenItemCount);
    FStatementUpdateHuman.OrderBindParamInt(HumData.dwAddMaxWeight);

    FStatementUpdateHuman.OrderBindParamInt(HumData.dwHighLevelKillMonFixExpTimeLeft);

    FStatementUpdateHuman.OrderBindParamText(HumData.sMobileNumber);                        // 手机号码
    FStatementUpdateHuman.OrderBindParamBool(HumData.boMobileBind);                         // 是否绑定
    FStatementUpdateHuman.OrderBindParamText(HumData.sMobileVerifyCode);                    // 验证码
    FStatementUpdateHuman.OrderBindParamInt(HumData.dwMobileSendTick);                      // 最后发送时间
    FStatementUpdateHuman.OrderBindParamInt(HumData.nMobileResendCount);                    // 重发验证码次数

    FStatementUpdateHuman.OrderBindParamInt(HumData.nClearDayVarTime);                       // 变量清空时间

    FStatementUpdateHuman.OrderBindParamInt(HumanID);

    if not (FStatementUpdateHuman.Step) then
      Exit;

    FStatementInsertAbil.Reset;
    FStatementInsertAbil.OrderBindParamInt(HumanID);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.AC1);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.AC2);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MAC1);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MAC2);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.DC1);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.DC2);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MC1);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MC2);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.SC1);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.SC2);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.HP);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MaxHP);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MP);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MaxMP);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.Exp);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MaxExp);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.Weight);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MaxWeight);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.WearWeight);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MaxWearWeight);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.HandWeight);
    FStatementInsertAbil.OrderBindParamInt(HumData.Abil.MaxHandWeight);
    FStatementInsertAbil.OrderBindParamInt(HumData.nBonusPoint);
    FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.DC);
    FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.MC);
    FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.SC);
    FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.AC);
    FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.MAC);
    FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.HP);
    FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.MP);
    FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.Hit);
    FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.Speed);
    FStatementInsertAbil.OrderBindParamInt(HumData.BonusAbil.X2);
    FStatementInsertAbil.Step;

    FStatementInsertAbilNG.Reset;
    FStatementInsertAbilNG.OrderBindParamInt(HumanID);
    FStatementInsertAbilNG.OrderBindParamBool(HumData.boTrainingNG);
    FStatementInsertAbilNG.OrderBindParamBool(HumData.boTrainingXF);
    FStatementInsertAbilNG.OrderBindParamInt(HumData.AbilNG.Level);
    FStatementInsertAbilNG.OrderBindParamInt(HumData.AbilNG.NH);
    FStatementInsertAbilNG.OrderBindParamInt(HumData.AbilNG.MaxNH);
    FStatementInsertAbilNG.OrderBindParamInt(HumData.AbilNG.Exp);
    FStatementInsertAbilNG.OrderBindParamInt(HumData.AbilNG.MaxExp);
    FStatementInsertAbilNG.OrderBindParamInt(HumData.ContinuousMagicOrder[0]);
    FStatementInsertAbilNG.OrderBindParamInt(HumData.ContinuousMagicOrder[1]);
    FStatementInsertAbilNG.OrderBindParamInt(HumData.ContinuousMagicOrder[2]);
    FStatementInsertAbilNG.OrderBindParamBool(HumData.boOpenLastContinuous);
    FStatementInsertAbilNG.OrderBindParamInt(HumData.btLastContinuousMagicOrder);
    for I := 0 to 4 do
    begin
      FStatementInsertAbilNG.OrderBindParamInt(HumData.Meridians[I].Level);
      FStatementInsertAbilNG.OrderBindParamInt(HumData.Meridians[I].BlastHitRate);
      for J := 0 to 4 do
      begin
        FStatementInsertAbilNG.OrderBindParamInt(HumData.Meridians[I].Acupoints[J])
      end;
    end;
    FStatementInsertAbilNG.Step;

    FStatementInsertAbilWine.Reset;
    FStatementInsertAbilWine.OrderBindParamInt(HumanID);
    FStatementInsertAbilWine.OrderBindParamBool(HumData.boPleaseDrink);
    FStatementInsertAbilWine.OrderBindParamBool(HumData.boDrinkWineDrunk);
    FStatementInsertAbilWine.OrderBindParamInt(HumData.nDrinkWineQuality);
    FStatementInsertAbilWine.OrderBindParamInt(HumData.nDrinkWineAlcohol);
    FStatementInsertAbilWine.OrderBindParamInt(HumData.Alcohol.Alcohol);
    FStatementInsertAbilWine.OrderBindParamInt(HumData.Alcohol.MaxAlcohol);
    FStatementInsertAbilWine.OrderBindParamInt(HumData.Alcohol.WineDrinkValue);
    FStatementInsertAbilWine.OrderBindParamInt(HumData.Alcohol.MedicineLevel);
    FStatementInsertAbilWine.OrderBindParamInt(HumData.Alcohol.MedicineValue);
    FStatementInsertAbilWine.OrderBindParamInt(HumData.Alcohol.MaxMedicineValue);
    FStatementInsertAbilWine.Step;

    //end;

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

    Execute('delete from HumanSkillPower where HumanID = ' + IntToStr(HumanID));

    Execute('delete from HumanItems where HumanID = ' + IntToStr(HumanID));

    Execute('delete from HumanMoney where HumanID = ' + IntToStr(HumanID));

    for I := Low(HumData.AddSaveAbil) to High(HumData.AddSaveAbil) do
    begin
      if HumData.AddSaveAbil[I] <> 0 then
      begin
        FStatementInsertAbilNpcAdd.Reset;
        FStatementInsertAbilNpcAdd.OrderBindParamInt(HumanID);
        FStatementInsertAbilNpcAdd.OrderBindParamInt(I);
        FStatementInsertAbilNpcAdd.OrderBindParamInt(HumData.AddSaveAbil[I]);
        FStatementInsertAbilNpcAdd.Step;
      end;
    end;

    for I := Low(HumData.GamePetData) to High(HumData.GamePetData) do
    begin
      if Length(HumData.GamePetData[I].sName) > 0 then
      begin
        FStatementInsertGamePetData.Reset;
        FStatementInsertGamePetData.OrderBindParamInt(HumanID);
        FStatementInsertGamePetData.OrderBindParamInt(I);
        FStatementInsertGamePetData.OrderBindParamText(HumData.GamePetData[I].sName);
        FStatementInsertGamePetData.OrderBindParamInt(HumData.GamePetData[I].Level);
        FStatementInsertGamePetData.OrderBindParamInt(HumData.GamePetData[I].HP);
        FStatementInsertGamePetData.OrderBindParamInt(HumData.GamePetData[I].MP);
        FStatementInsertGamePetData.OrderBindParamInt(HumData.GamePetData[I].Exp);
        FStatementInsertGamePetData.OrderBindParamInt(HumData.GamePetData[I].wMagics[0]);
        FStatementInsertGamePetData.OrderBindParamInt(HumData.GamePetData[I].wMagics[1]);
        FStatementInsertGamePetData.OrderBindParamInt(HumData.GamePetData[I].wMagics[2]);
        FStatementInsertGamePetData.OrderBindParamInt(HumData.GamePetData[I].wMagics[3]);
        FStatementInsertGamePetData.OrderBindParamInt(HumData.GamePetData[I].wMagics[4]);
        FStatementInsertGamePetData.OrderBindParamInt(HumData.GamePetData[I].wMagics[5]);
        FStatementInsertGamePetData.OrderBindParamInt(HumData.GamePetData[I].wMagics[6]);
        FStatementInsertGamePetData.OrderBindParamInt(HumData.GamePetData[I].wMagics[7]);
        FStatementInsertGamePetData.Step;
      end;
    end;

    for I := Low(HumData.GodBlessItemsState) to High(HumData.GodBlessItemsState) do
    begin
      if HumData.GodBlessItemsState[I] <> 0 then
      begin
        FStatementInsertGodBlessState.Reset;
        FStatementInsertGodBlessState.OrderBindParamInt(HumanID);
        FStatementInsertGodBlessState.OrderBindParamInt(I);
        FStatementInsertGodBlessState.OrderBindParamInt(HumData.GodBlessItemsState[I]);
        FStatementInsertGodBlessState.Step;
      end;
    end;

    for I := Low(HumData.Magics) to High(HumData.Magics) do
    begin
      HumMagic := @HumData.Magics[I];
      if HumMagic.wMagIdx > 0 then
      begin
        FStatementInsertMagic.Reset;
        FStatementInsertMagic.OrderBindParamInt(HumanID);
        FStatementInsertMagic.OrderBindParamInt(1);
        FStatementInsertMagic.OrderBindParamInt(I);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.wMagIdx);
        FStatementInsertMagic.OrderBindParamInt(Integer(HumMagic.MagicAttr));
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btLevel);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btNewLevel);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btKey);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.nTranPoint);
        FStatementInsertMagic.OrderBindParamBool(HumMagic.boUsesItemAdd);
        FStatementInsertMagic.Step;
      end;
    end;

    for I := Low(HumData.NGMagics) to High(HumData.NGMagics) do
    begin
      HumMagic := @HumData.NGMagics[I];
      if HumMagic.wMagIdx > 0 then
      begin
        FStatementInsertMagic.Reset;
        FStatementInsertMagic.OrderBindParamInt(HumanID);
        FStatementInsertMagic.OrderBindParamInt(2);
        FStatementInsertMagic.OrderBindParamInt(I);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.wMagIdx);
        FStatementInsertMagic.OrderBindParamInt(Integer(HumMagic.MagicAttr));
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btLevel);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btNewLevel);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btKey);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.nTranPoint);
        FStatementInsertMagic.OrderBindParamInt(Integer(HumMagic.boUsesItemAdd));
        FStatementInsertMagic.Step;
      end;
    end;

    for I := Low(HumData.ContinuousMagics) to High(HumData.ContinuousMagics) do
    begin
      HumMagic := @HumData.ContinuousMagics[I];
      if HumMagic.wMagIdx > 0 then
      begin
        FStatementInsertMagic.Reset;
        FStatementInsertMagic.OrderBindParamInt(HumanID);
        FStatementInsertMagic.OrderBindParamInt(3);
        FStatementInsertMagic.OrderBindParamInt(I);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.wMagIdx);
        FStatementInsertMagic.OrderBindParamInt(Integer(HumMagic.MagicAttr));
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btLevel);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btNewLevel);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btKey);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.nTranPoint);
        FStatementInsertMagic.OrderBindParamInt(Integer(HumMagic.boUsesItemAdd));
        FStatementInsertMagic.Step;
      end;
    end;

    for I := Low(HumData.CustomSkillUseTicks) to High(HumData.CustomSkillUseTicks) do
    begin
      if HumData.CustomSkillUseTicks[I] <> 0 then
      begin
        FStatementInsertMagicUseTick.Reset;
        FStatementInsertMagicUseTick.OrderBindParamInt(HumanID);
        FStatementInsertMagicUseTick.OrderBindParamInt(I + CUSTOM_MAGIC_START_ID);
        FStatementInsertMagicUseTick.OrderBindParamInt(HumData.CustomSkillUseTicks[I]);
        FStatementInsertMagicUseTick.Step;
      end;
    end;

    for I := Low(HumData.wStatusTimeArr) to High(HumData.wStatusTimeArr) do
    begin
      if HumData.wStatusTimeArr[I] <> 0 then
      begin
        FStatementInsertStatusTime.Reset;
        FStatementInsertStatusTime.OrderBindParamInt(HumanID);
        FStatementInsertStatusTime.OrderBindParamInt(I);
        FStatementInsertStatusTime.OrderBindParamInt(HumData.wStatusTimeArr[I]);
        FStatementInsertStatusTime.Step;
      end;
    end;

    for I := Low(HumData.QuestFlag) to High(HumData.QuestFlag) do
    begin
      if HumData.QuestFlag[I] <> 0 then
      begin
        FStatementInsertQuestFlag.Reset;
        FStatementInsertQuestFlag.OrderBindParamInt(HumanID);
        FStatementInsertQuestFlag.OrderBindParamInt(I);
        FStatementInsertQuestFlag.OrderBindParamInt(HumData.QuestFlag[I]);
        FStatementInsertQuestFlag.Step;
      end;
    end;

    for I := Low(HumData.UValues) to High(HumData.UValues) do
    begin
      if HumData.UValues[I] <> 0 then
      begin
        FStatementInsertVariableU.Reset;
        FStatementInsertVariableU.OrderBindParamInt(HumanID);
        FStatementInsertVariableU.OrderBindParamInt(I);
        FStatementInsertVariableU.OrderBindParamInt(HumData.UValues[I]);
        FStatementInsertVariableU.Step;
      end;
    end;

    for I := Low(HumData.TValues) to High(HumData.TValues) do
    begin
      if Length(HumData.TValues[I]) > 0 then
      begin
        FStatementInsertVariableT.Reset;
        FStatementInsertVariableT.OrderBindParamInt(HumanID);
        FStatementInsertVariableT.OrderBindParamInt(I);
        FStatementInsertVariableT.OrderBindParamText(HumData.TValues[I]);
        FStatementInsertVariableT.Step;
      end;
    end;

    for I := Low(HumData.JValues) to High(HumData.JValues) do
    begin
      if HumData.JValues[I] <> 0 then
      begin
        FStatementInsertVariableJ.Reset;
        FStatementInsertVariableJ.OrderBindParamInt(HumanID);
        FStatementInsertVariableJ.OrderBindParamInt(I);
        FStatementInsertVariableJ.OrderBindParamInt(HumData.JValues[I]);
        FStatementInsertVariableJ.Step;
      end;
    end;

    for I := Low(HumData.ZValues) to High(HumData.ZValues) do
    begin
      if Length(HumData.ZValues[I]) > 0 then
      begin
        FStatementInsertVariableZ.Reset;
        FStatementInsertVariableZ.OrderBindParamInt(HumanID);
        FStatementInsertVariableZ.OrderBindParamInt(I);
        FStatementInsertVariableZ.OrderBindParamText(HumData.ZValues[I]);
        FStatementInsertVariableZ.Step;
      end;
    end;

    for I := Low(HumData.CustomMoney) to High(HumData.CustomMoney) do
    begin
      if HumData.CustomMoney[I].sName <> '' then
      begin
        FStatementInsertCustomMoney.Reset;
        FStatementInsertCustomMoney.OrderBindParamInt(HumanID);
        FStatementInsertCustomMoney.OrderBindParamText(HumData.CustomMoney[I].sName);
        FStatementInsertCustomMoney.OrderBindParamInt(HumData.CustomMoney[I].nCount);
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
        FStatementInsertSkillPower.OrderBindParamInt(HumanID);
        FStatementInsertSkillPower.OrderBindParamInt(I);
        FStatementInsertSkillPower.OrderBindParamInt(HumData.NpcSkillPowerAdd[I].HumanAttackPercent);
        FStatementInsertSkillPower.OrderBindParamInt(HumData.NpcSkillPowerAdd[I].HumanAttackValue);
        FStatementInsertSkillPower.OrderBindParamInt(HumData.NpcSkillPowerAdd[I].MonAttackPercent);
        FStatementInsertSkillPower.OrderBindParamInt(HumData.NpcSkillPowerAdd[I].MonAttackValue);
        FStatementInsertSkillPower.OrderBindParamInt(HumData.NpcSkillPowerAdd[I].DefensePercent);
        FStatementInsertSkillPower.OrderBindParamInt(HumData.NpcSkillPowerAdd[I].DefenseValue);
        FStatementInsertSkillPower.OrderBindParamInt(HumData.NpcSkillPowerAdd[I].RemainingTime);

        FStatementInsertSkillPower.Step;
      end;
    end;
    Result := True;
  finally
    ResetAllSaveDataStatement;
  end;
end;

procedure TMySqlHumanDB.AddHumanItemToDB(UserItem: PTUserItem; HumanID, ItemType, ItemIndex: Integer);
var
  J: Integer;
begin
  FStatementInsertItems.Reset;
  FStatementInsertItems.OrderBindParamInt(HumanID);
  FStatementInsertItems.OrderBindParamInt(ItemType);
  FStatementInsertItems.OrderBindParamInt(ItemIndex);
  FStatementInsertItems.OrderBindParamInt(UserItem.MakeIndex);
  FStatementInsertItems.OrderBindParamInt(UserItem.wIndex);
  FStatementInsertItems.OrderBindParamText(UserItem.Name);
  FStatementInsertItems.OrderBindParamInt(UserItem.Dura);
  FStatementInsertItems.OrderBindParamInt(UserItem.DuraMax);
  FStatementInsertItems.OrderBindParamInt(UserItem.dwHeroM2DressEffect);
  FStatementInsertItems.OrderBindParamInt(UserItem.btUpgradeCount);
  FStatementInsertItems.OrderBindParamBool(UserItem.boStartTime);
  FStatementInsertItems.OrderBindParamInt(UserItem.nLimitTime);
  FStatementInsertItems.OrderBindParamInt(UserItem.btHeroM2Light);
  FStatementInsertItems.OrderBindParamInt(UserItem.btColor);
  FStatementInsertItems.OrderBindParamBool(UserItem.boIsBind);
  FStatementInsertItems.OrderBindParamInt(UserItem.btBindOption);
  FStatementInsertItems.OrderBindParamInt(UserItem.wEffect);
  FStatementInsertItems.OrderBindParamInt(UserItem.wNewLooks);
  FStatementInsertItems.OrderBindParamInt(UserItem.wNewShape);
  FStatementInsertItems.OrderBindParamInt(UserItem.btFluteCount);
  FStatementInsertItems.OrderBindParamText(UserItem.CustomProperty.sText);
  FStatementInsertItems.OrderBindParamInt(UserItem.CustomProperty.btTextColor);
  FStatementInsertItems.OrderBindParamInt(Integer(UserItem.ItemFrom.ItemForm));
  FStatementInsertItems.OrderBindParamText(UserItem.ItemFrom.sMapName);
  FStatementInsertItems.OrderBindParamText(UserItem.ItemFrom.sMonName);
  FStatementInsertItems.OrderBindParamText(UserItem.ItemFrom.sMakerName);
  FStatementInsertItems.OrderBindParamDouble(UserItem.ItemFrom.DateTime);
  FStatementInsertItems.OrderBindParamInt(UserItem.wInsuranceCount);
  FStatementInsertItems.OrderBindParamInt(UserItem.wNewExpand3);
  FStatementInsertItems.OrderBindParamInt(UserItem.wNewExpand4);
  FStatementInsertItems.Step;


  //---------------------------------------------------------------------------------------------

  for J := Low(UserItem.btValue) to High(UserItem.btValue) do
  begin
    if UserItem.btValue[J] <> 0 then
    begin
      FStatementInsertItemValueAdd.Reset;
      FStatementInsertItemValueAdd.OrderBindParamInt(HumanID);
      FStatementInsertItemValueAdd.OrderBindParamInt(ItemType);
      FStatementInsertItemValueAdd.OrderBindParamInt(ItemIndex);
      FStatementInsertItemValueAdd.OrderBindParamInt(J);
      FStatementInsertItemValueAdd.OrderBindParamInt(UserItem.btValue[J]);
      FStatementInsertItemValueAdd.Step;
    end;
  end;

  for J := Low(UserItem.btNewValue) to High(UserItem.btNewValue) do
  begin
    if UserItem.btNewValue[J] <> 0 then
    begin
      FStatementInsertItemElementAdd.Reset;
      FStatementInsertItemElementAdd.OrderBindParamInt(HumanID);
      FStatementInsertItemElementAdd.OrderBindParamInt(ItemType);
      FStatementInsertItemElementAdd.OrderBindParamInt(ItemIndex);
      FStatementInsertItemElementAdd.OrderBindParamInt(J);
      FStatementInsertItemElementAdd.OrderBindParamInt(UserItem.btNewValue[J]);
      FStatementInsertItemElementAdd.Step;
    end;
  end;

  for J := Low(UserItem.btAddDataByte) to High(UserItem.btAddDataByte) do
  begin
    if UserItem.btAddDataByte[J] <> 0 then
    begin
      FStatementInsertItemAddDataByte.Reset;
      FStatementInsertItemAddDataByte.OrderBindParamInt(HumanID);
      FStatementInsertItemAddDataByte.OrderBindParamInt(ItemType);
      FStatementInsertItemAddDataByte.OrderBindParamInt(ItemIndex);
      FStatementInsertItemAddDataByte.OrderBindParamInt(J);
      FStatementInsertItemAddDataByte.OrderBindParamInt(UserItem.btNewValue[J]);
      FStatementInsertItemAddDataByte.Step;
    end;
  end;

  for J := Low(UserItem.nAddDataInt) to High(UserItem.nAddDataInt) do
  begin
    if UserItem.nAddDataInt[J] <> 0 then
    begin
      FStatementInsertItemAddDataInt.Reset;
      FStatementInsertItemAddDataInt.OrderBindParamInt(HumanID);
      FStatementInsertItemAddDataInt.OrderBindParamInt(ItemType);
      FStatementInsertItemAddDataInt.OrderBindParamInt(ItemIndex);
      FStatementInsertItemAddDataInt.OrderBindParamInt(J);
      FStatementInsertItemAddDataInt.OrderBindParamInt(UserItem.nAddDataInt[J]);
      FStatementInsertItemAddDataInt.Step;
    end;
  end;

  for J := Low(UserItem.sAddDataText) to High(UserItem.sAddDataText) do
  begin
    if UserItem.sAddDataText[J] <> '' then
    begin
      FStatementInsertItemAddDataText.Reset;
      FStatementInsertItemAddDataText.OrderBindParamInt(HumanID);
      FStatementInsertItemAddDataText.OrderBindParamInt(ItemType);
      FStatementInsertItemAddDataText.OrderBindParamInt(ItemIndex);
      FStatementInsertItemAddDataText.OrderBindParamInt(J);
      FStatementInsertItemAddDataText.OrderBindParamText(UserItem.sAddDataText[J]);
      FStatementInsertItemAddDataText.Step;
    end;
  end;

  for J := Low(UserItem.Flutes) to High(UserItem.Flutes) do
  begin
    if UserItem.Flutes[J].GemIndex <> 0 then
    begin
      FStatementInsertItemFlute.Reset;
      FStatementInsertItemFlute.OrderBindParamInt(HumanID);
      FStatementInsertItemFlute.OrderBindParamInt(ItemType);
      FStatementInsertItemFlute.OrderBindParamInt(ItemIndex);
      FStatementInsertItemFlute.OrderBindParamInt(J);
      FStatementInsertItemFlute.OrderBindParamInt(UserItem.Flutes[J].GemIndex);
      FStatementInsertItemFlute.OrderBindParamInt(UserItem.Flutes[J].GemCount);
      FStatementInsertItemFlute.Step;
    end;
  end;

  for J := Low(UserItem.Progress) to High(UserItem.Progress) do
  begin
    if UserItem.Progress[J].boOpen then
    begin
      FStatementInsertItemProgress.Reset;
      FStatementInsertItemProgress.OrderBindParamInt(HumanID);
      FStatementInsertItemProgress.OrderBindParamInt(ItemType);
      FStatementInsertItemProgress.OrderBindParamInt(ItemIndex);
      FStatementInsertItemProgress.OrderBindParamInt(J);
      FStatementInsertItemProgress.OrderBindParamBool(UserItem.Progress[J].boOpen);
      FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].btNameColor);
      FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].btCount);
      FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].btShowType);
      FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].wMax);
      FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].wValue);
      FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].wLevel);
      FStatementInsertItemProgress.OrderBindParamText(UserItem.Progress[J].sName);
      FStatementInsertItemProgress.Step;
    end;
  end;

  for J := Low(UserItem.CustomProperty.Properties) to High(UserItem.CustomProperty.Properties) do
  begin
    if (UserItem.CustomProperty.Properties[J].nValues[0] > 0) or (UserItem.CustomProperty.Properties[J].nValues[1] > 0) or (UserItem.CustomProperty.Properties[J].nValues[2] > 0) then
    begin
      FStatementInsertItemProperty.Reset;
      FStatementInsertItemProperty.OrderBindParamInt(HumanID);
      FStatementInsertItemProperty.OrderBindParamInt(ItemType);
      FStatementInsertItemProperty.OrderBindParamInt(ItemIndex);
      FStatementInsertItemProperty.OrderBindParamInt(J);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btColor);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btBindType);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btShowFlag);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btPercent);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btHintModule);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].nValues[0]);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].nValues[1]);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].nValues[2]);
      FStatementInsertItemProperty.Step;
    end;
  end;
end;

procedure TMySqlHumanDB.DoGetRankData(MinLevel, MaxLevel, TopCount: LongWord; HumanRankList, WarriorRankList, WizardRankList, TaoistRankList, MasterRankList: TRoleRankList);
var
  {Ret,} Index: Integer;
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
      FStatementGetLevelRankTopCount.OrderBindParamInt(0);
      FStatementGetLevelRankTopCount.OrderBindParamInt(1);
      FStatementGetLevelRankTopCount.OrderBindParamInt(2);
      FStatementGetLevelRankTopCount.OrderBindParamInt(QueryCount);
      Index := 0;
      if FStatementGetLevelRankTopCount.Query then
      begin
        while (FStatementGetLevelRankTopCount.Fetch) do
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
        end;
      end;

      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindParamInt(0);
      FStatementGetLevelRankTopCount.OrderBindParamInt(0);
      FStatementGetLevelRankTopCount.OrderBindParamInt(0);
      FStatementGetLevelRankTopCount.OrderBindParamInt(QueryCount);
      Index := 0;
      if FStatementGetLevelRankTopCount.Query then
      begin
        while (FStatementGetLevelRankTopCount.Fetch) do
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
        end;
      end;

      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindParamInt(1);
      FStatementGetLevelRankTopCount.OrderBindParamInt(1);
      FStatementGetLevelRankTopCount.OrderBindParamInt(1);
      FStatementGetLevelRankTopCount.OrderBindParamInt(QueryCount);
      Index := 0;
      if FStatementGetLevelRankTopCount.Query then
      begin
        while (FStatementGetLevelRankTopCount.Fetch) do
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
        end;
      end;

      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindParamInt(2);
      FStatementGetLevelRankTopCount.OrderBindParamInt(2);
      FStatementGetLevelRankTopCount.OrderBindParamInt(2);
      FStatementGetLevelRankTopCount.OrderBindParamInt(QueryCount);
      Index := 0;
      if FStatementGetLevelRankTopCount.Query then
      begin
        while (FStatementGetLevelRankTopCount.Fetch) do
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
        end;
      end;
    finally
      FStatementGetLevelRankTopCount.Reset;
    end;

    try
      FStatementGetMasterRankTopCount.Reset;
      FStatementGetMasterRankTopCount.OrderBindParamInt(0);
      FStatementGetMasterRankTopCount.OrderBindParamInt(1);
      FStatementGetMasterRankTopCount.OrderBindParamInt(2);
      FStatementGetMasterRankTopCount.OrderBindParamInt(QueryCount);
      Index := 0;
      if FStatementGetMasterRankTopCount.Query then
      begin
        while (FStatementGetMasterRankTopCount.Fetch) do
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
        end;
      end;
    finally
      FStatementGetMasterRankTopCount.Reset;
    end;
  end
  else if TopCount = 0 then
  begin
    try
      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MaxLevel);
      Index := 0;
      if FStatementGetLevelRankCheckLevel.Query then
      begin
        while (FStatementGetLevelRankCheckLevel.Fetch) do
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
          RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;
          RankData.HeroName := '';

          HumanRankList.Add(@RankData);
          Inc(Index);
        end;
      end;

      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MaxLevel);
      Index := 0;
      if FStatementGetLevelRankCheckLevel.Query then
      begin
        while (FStatementGetLevelRankCheckLevel.Fetch) do
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
          RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;
          RankData.HeroName := '';

          WarriorRankList.Add(@RankData);
          Inc(Index);
        end;
      end;

      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MaxLevel);
      Index := 0;
      if FStatementGetLevelRankCheckLevel.Query then
      begin
        while (FStatementGetLevelRankCheckLevel.Fetch) do
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
          RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;
          RankData.HeroName := '';

          WizardRankList.Add(@RankData);
          Inc(Index);
        end;
      end;

      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MaxLevel);
      Index := 0;
      if FStatementGetLevelRankCheckLevel.Query then
      begin
        while (FStatementGetLevelRankCheckLevel.Fetch) do
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
          RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;
          RankData.HeroName := '';

          TaoistRankList.Add(@RankData);
          Inc(Index);
        end;
      end;
    finally
      FStatementGetLevelRankCheckLevel.Reset;
    end;

    try
      FStatementGetMasterRankCheckLevel.Reset;
      FStatementGetMasterRankCheckLevel.OrderBindParamInt(0);
      FStatementGetMasterRankCheckLevel.OrderBindParamInt(1);
      FStatementGetMasterRankCheckLevel.OrderBindParamInt(2);
      FStatementGetMasterRankCheckLevel.OrderBindParamInt(MinLevel);
      FStatementGetMasterRankCheckLevel.OrderBindParamInt(MaxLevel);
      Index := 0;
      if FStatementGetMasterRankCheckLevel.Query then
      begin
        while (FStatementGetMasterRankCheckLevel.Fetch) do
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := FStatementGetMasterRankCheckLevel.OrderGetColumnValueText;
          RankData.MasterCount := FStatementGetMasterRankCheckLevel.OrderGetColumnValueInt;
          RankData.HeroName := '';

          MasterRankList.Add(@RankData);
          Inc(Index);
        end;
      end;
    finally
      FStatementGetMasterRankCheckLevel.Reset;
    end;
  end
  else
  begin
    try
      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(QueryCount);
      Index := 0;
      if FStatementGetLevelRankCheckLevelAndCount.Query then
      begin
        while (FStatementGetLevelRankCheckLevelAndCount.Fetch) do
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
        end;
      end;

      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(QueryCount);
      Index := 0;
      if FStatementGetLevelRankCheckLevelAndCount.Query then
      begin
        while (FStatementGetLevelRankCheckLevelAndCount.Fetch) do
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
        end;
      end;

      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(QueryCount);
      Index := 0;
      if FStatementGetLevelRankCheckLevelAndCount.Query then
      begin
        while (FStatementGetLevelRankCheckLevelAndCount.Fetch) do
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
        end;
      end;

      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(QueryCount);
      Index := 0;
      if FStatementGetLevelRankCheckLevelAndCount.Query then
      begin
        while (FStatementGetLevelRankCheckLevelAndCount.Fetch) do
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
        end;
      end;
    finally
      FStatementGetLevelRankCheckLevelAndCount.Reset;
    end;

    try
      FStatementGetMasterRankCheckLevelAndCount.Reset;
      FStatementGetMasterRankCheckLevelAndCount.OrderBindParamInt(0);
      FStatementGetMasterRankCheckLevelAndCount.OrderBindParamInt(1);
      FStatementGetMasterRankCheckLevelAndCount.OrderBindParamInt(2);
      FStatementGetMasterRankCheckLevelAndCount.OrderBindParamInt(MinLevel);
      FStatementGetMasterRankCheckLevelAndCount.OrderBindParamInt(MaxLevel);
      FStatementGetMasterRankCheckLevelAndCount.OrderBindParamInt(QueryCount);
      Index := 0;
      if FStatementGetMasterRankCheckLevelAndCount.Query then
      begin
        while (FStatementGetMasterRankCheckLevelAndCount.Fetch) do
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
        end;
      end;
    finally
      FStatementGetMasterRankCheckLevelAndCount.Reset;
    end;
  end;
end;

function TMySqlHumanDB.DoBuyPlayer(const sSellAccount, sSellHumanName, sBuyAccount, sBuyHumanName: string): Boolean;
begin
  FStatementBuyPlayer.Reset;
  try
    FStatementBuyPlayer.OrderBindParamText(sBuyAccount);
    FStatementBuyPlayer.OrderBindParamText(sSellAccount);
    FStatementBuyPlayer.OrderBindParamText(sSellHumanName);
    Result := FStatementBuyPlayer.Step;
  finally
    FStatementBuyPlayer.Reset;
  end;
end;

procedure TMySqlHumanDB.BeginTransaction;
begin
  TMySqlRoleDB(Owner).FDB.StartTransaction;
end;

procedure TMySqlHumanDB.Commit;
begin
  TMySqlRoleDB(Owner).FDB.Commit;
end;

procedure TMySqlHumanDB.RollBack;
begin
  TMySqlRoleDB(Owner).FDB.Rollback;
end;

procedure TMySqlHumanDB.Execute(Sql: string);
begin
  TMySqlRoleDB(Owner).FDB.Exec(Sql);
end;

procedure TMySqlHumanDB.ResetAllGetDataStatement;
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

procedure TMySqlHumanDB.ResetAllSaveDataStatement;
begin
  FStatementUpdateHuman.Reset;

  FStatementInsertAbil.Reset;
  FStatementInsertAbilNG.Reset;
  FStatementInsertAbilWine.Reset;

  {
  FStatementUpdateAbil.Reset;
  FStatementUpdateAbilNG.Reset;
  FStatementUpdateAbilWine.Reset;
  }

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

procedure TMySqlHeroDB.DoInit;
var
  Stms: TMySqlStatements;
begin
  Assert(Owner is TMySqlRoleDB, 'TSqliteHeroDB owner type error.');
  Assert(TMySqlRoleDB(Owner).FDB <> nil, 'TMySqlRoleDB.DB not create');
  Stms := TMySqlRoleDB(Owner).FDB.Statements;

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
  FStatementGetAbilNpcAdd.Sql := 'select ' + '`Index`,' + '`Value` ' + 'from HeroAbilNpcAdd ' + 'where HeroID = ?;';

  FStatementGetGodBlessState := Stms.AddSQLStatement('HeroSelectGodBlessState');
  FStatementGetGodBlessState.Sql := 'select ' + '`Index`,' + '`Value` ' + 'from HeroGodBlessState ' + 'where HeroID = ?;';

  FStatementGetMagic := Stms.AddSQLStatement('HeroSelectMagic');
  FStatementGetMagic.Sql := 'select ' + 'MagicType,' + 'MagicIndex,' + 'MagicID,' + 'MagicAttr,' + 'MagicLevel,' + 'MagicNewLevel,' + 'MagicKey,' + 'MagicTranPoint,' + 'MagicIsUseItemAdd ' + 'from HeroMagic ' + 'where HeroID = ?;';

  FStatementGetStatusTime := Stms.AddSQLStatement('HeroSelectStatusTime');
  FStatementGetStatusTime.Sql := 'select ' + '`Index`,' + '`Value` ' + 'from HeroStatusTime ' + 'where HeroID = ?;';

  FStatementGetQuestFlag := Stms.AddSQLStatement('HeroSelectQuestFlag');
  FStatementGetQuestFlag.Sql := 'select ' + '`Index`,' + '`Value` ' + 'from HeroQuestFlag ' + 'where HeroID = ?;';

  FStatementGetItems := Stms.AddSQLStatement('HeroSelectitems');
  FStatementGetItems.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'MakeIndex,' + 'DBIndex,' + 'Name,' + 'Dura,' + 'DuraMax,' + 'HeroM2DressEffect,' + 'UpgradeCount,' + 'IsStartTime,' + 'LimitTime,' + 'HeroM2Light,' + 'Color,' + 'IsBind,' + 'BindOption,' + 'Effect,' + 'NewLooks,' + 'NewShape,' + 'FluteCount,' + 'PropertyText,' + 'PropertyTextColor,' + 'ItemFrom,' + 'ItemFromMap,' + 'ItemFromMon,' + 'ItemFromMaker,' + 'ItemFromDate,' + 'InsuranceCount, ' + 'NewExpand3, ' + 'NewExpand4 ' + 'from HeroItems ' + 'where HeroID = ?;';

  FStatementGetItemValueAdd := Stms.AddSQLStatement('HeroSelectitemValueAdd');
  FStatementGetItemValueAdd.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + '`Value` ' + 'from HeroItemValueAdd ' + 'where HeroID = ?;';

  FStatementGetItemElementAdd := Stms.AddSQLStatement('HeroSelectitemElementAdd');
  FStatementGetItemElementAdd.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + '`Value` ' + 'from HeroItemElementAdd ' + 'where HeroID = ?;';

  FStatementGetItemAddDataByte := Stms.AddSQLStatement('HeroSelectitemAddDataByte');
  FStatementGetItemAddDataByte.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + '`Value` ' + 'from HeroItemAddDataByte ' + 'where HeroID = ?;';

  FStatementGetItemAddDataInt := Stms.AddSQLStatement('HeroSelectitemAddDataInt');
  FStatementGetItemAddDataInt.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + '`Value` ' + 'from HeroItemAddDataInt ' + 'where HeroID = ?;';

  FStatementGetItemAddDataText := Stms.AddSQLStatement('HeroSelectitemAddDataText');
  FStatementGetItemAddDataText.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + '`Value` ' + 'from HeroItemAddDataText ' + 'where HeroID = ?;';

  FStatementGetItemFlute := Stms.AddSQLStatement('HeroSelectitemFlute');
  FStatementGetItemFlute.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + '`Value`,' + 'OverlapCount ' + 'from HeroItemFlute ' + 'where HeroID = ?;';

  FStatementGetItemProgress := Stms.AddSQLStatement('HeroSelectItemProgress');
  FStatementGetItemProgress.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'IsOpen,' + 'NameColor,' + 'Count,' + 'ShowType,' + 'Max,' + '`Value`,' + 'Level,' + 'Name ' + 'from HeroItemProgress ' + 'where HeroID = ?;';

  FStatementGetItemProperty := Stms.AddSQLStatement('HeroSelectItemProperty');
  FStatementGetItemProperty.Sql := 'select ' + 'ItemType,' + 'ItemIndex,' + 'ValueIndex,' + 'Color,' + 'BindType,' + 'ShowFlag,' + 'IsPercent,' + 'HintModule,' + '`Value`,' + '`Value2`,' + '`Value3` ' + 'from HeroItemProperty ' + 'where HeroID = ?;';

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
  FStatementInsertAbil.Sql := 'replace into HeroAbil(' + 'HeroID, ' + 'AC1, ' + 'AC2, ' + 'MAC1, ' + 'MAC2, ' + 'DC1, ' + 'DC2, ' + 'MC1, ' + 'MC2, ' + 'SC1, ' + 'SC2, ' + 'HP, ' + 'MaxHP,' + 'MP, ' + 'MaxMP, ' + 'Exp, ' + 'MaxExp, ' + 'Weight, ' + 'MaxWeight, ' + 'WearWeight, ' + 'MaxWearWeight, ' + 'HandWeight, ' + 'MaxHandWeight) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertAbilNG := Stms.AddSQLStatement('HeroInsertAbilNG');
  FStatementInsertAbilNG.Sql := 'replace into HeroAbilNG(' + 'HeroID, ' + 'IsTrainingNG, ' + 'IsTrainingXF, ' + 'AbilNGLevel, ' + 'AbilNGValue, ' + 'AbilNGMaxValue, ' + 'AbilNGExp, ' + 'AbilNGMaxExp,' + 'ContinuousMagicOrder1, ' + 'ContinuousMagicOrder2, ' + 'ContinuousMagicOrder3, ' + 'IsOpenLastContinuous, ' + 'LastContinuousMagicOrder, ' + 'Meridians1Level, ' + 'Meridians1BlastHitRate1, ' + 'Meridians1Acupoints1, ' + 'Meridians1Acupoints2, ' + 'Meridians1Acupoints3, ' + 'Meridians1Acupoints4, ' +
    'Meridians1Acupoints5, ' + 'Meridians2Level, ' + 'Meridians2BlastHitRate, ' + 'Meridians2Acupoints1, ' + 'Meridians2Acupoints2, ' + 'Meridians2Acupoints3, ' + 'Meridians2Acupoints4, ' + 'Meridians2Acupoints5, ' + 'Meridians3Level, ' + 'Meridians3BlastHitRate, ' + 'Meridians3Acupoints1, ' + 'Meridians3Acupoints2, ' + 'Meridians3Acupoints3, ' + 'Meridians3Acupoints4, ' + 'Meridians3Acupoints5, ' + 'Meridians4Level, ' + 'Meridians4BlastHitRate, ' + 'Meridians4Acupoints1, ' + 'Meridians4Acupoints2, ' +
    'Meridians4Acupoints3, ' + 'Meridians4Acupoints4, ' + 'Meridians4Acupoints5, ' + 'Meridians5Level, ' + 'Meridians5BlastHitRate, ' + 'Meridians5Acupoints1, ' + 'Meridians5Acupoints2, ' + 'Meridians5Acupoints3, ' + 'Meridians5Acupoints4, ' + 'Meridians5Acupoints5) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ' + '?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertAbilWine := Stms.AddSQLStatement('HeroInsertAbilWine');
  FStatementInsertAbilWine.Sql := 'replace into HeroAbilWine(' + 'HeroID, ' + 'IsDrinkWineDrunk, ' + 'DrinkWineQuality, ' + 'DrinkWineAlcohol, ' + 'AbilAlcohol, ' + 'AbilMaxAlcohol, ' + 'AbilDrinkValue, ' + 'AbilMedicineLevel, ' + 'AbilMedicineValue, ' + 'AbilMaxMedicineValue) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  {
  FStatementUpdateAbil := Stms.AddSQLStatement('HeroUpdateAbil');
  FStatementUpdateAbil.Sql :=
    'update HeroAbil set ' +
      'AC1 = ?,' +
      'AC2 = ?,' +
      'MAC1 = ?,' +
      'MAC2 = ?,' +
      'DC1 = ?,' +
      'DC2 = ?,' +
      'MC1 = ?,' +
      'MC2 = ?,' +
      'SC1 = ?,' +
      'SC2 = ?,' +
      'HP = ?,' +
      'MaxHP = ?,' +
      'MP = ?,' +
      'MaxMP = ?,' +
      'Exp = ?,' +
      'MaxExp = ?,' +
      'Weight = ?,' +
      'MaxWeight = ?,' +
      'WearWeight = ?,' +
      'MaxWearWeight = ?,' +
      'HandWeight = ?,' +
      'MaxHandWeight = ? ' +
    'where HeroID = ?;';

  FStatementUpdateAbilNG := Stms.AddSQLStatement('HeroUpdateAbilNG');
  FStatementUpdateAbilNG.Sql :=
    'update HeroAbilNG set ' +
      'IsTrainingNG = ?,' +
      'IsTrainingXF = ?,' +
      'AbilNGLevel = ?,' +
      'AbilNGValue = ?,' +
      'AbilNGMaxValue = ?,' +
      'AbilNGExp = ?,' +
      'AbilNGMaxExp = ?,' +
      'ContinuousMagicOrder1 = ?,' +
      'ContinuousMagicOrder2 = ?,' +
      'ContinuousMagicOrder3 = ?,' +
      'IsOpenLastContinuous = ?,' +
      'LastContinuousMagicOrder = ?,' +
      'Meridians1Level = ?,' +
      'Meridians1BlastHitRate1 = ?,' +
      'Meridians1Acupoints1 = ?,' +
      'Meridians1Acupoints2 = ?,' +
      'Meridians1Acupoints3 = ?,' +
      'Meridians1Acupoints4 = ?,' +
      'Meridians1Acupoints5 = ?,' +
      'Meridians2Level = ?,' +
      'Meridians2BlastHitRate = ?,' +
      'Meridians2Acupoints1 = ?,' +
      'Meridians2Acupoints2 = ?,' +
      'Meridians2Acupoints3 = ?,' +
      'Meridians2Acupoints4 = ?,' +
      'Meridians2Acupoints5 = ?,' +
      'Meridians3Level = ?,' +
      'Meridians3BlastHitRate = ?,' +
      'Meridians3Acupoints1 = ?,' +
      'Meridians3Acupoints2 = ?,' +
      'Meridians3Acupoints3 = ?,' +
      'Meridians3Acupoints4 = ?,' +
      'Meridians3Acupoints5 = ?,' +
      'Meridians4Level = ?,' +
      'Meridians4BlastHitRate = ?,' +
      'Meridians4Acupoints1 = ?,' +
      'Meridians4Acupoints2 = ?,' +
      'Meridians4Acupoints3 = ?,' +
      'Meridians4Acupoints4 = ?,' +
      'Meridians4Acupoints5 = ?,' +
      'Meridians5Level = ?,' +
      'Meridians5BlastHitRate = ?,' +
      'Meridians5Acupoints1 = ?,' +
      'Meridians5Acupoints2 = ?,' +
      'Meridians5Acupoints3 = ?,' +
      'Meridians5Acupoints4 = ?,' +
      'Meridians5Acupoints5 = ? ' +
    'where HeroID = ?;';

  FStatementUpdateAbilWine := Stms.AddSQLStatement('HeroUpdateAbilWine');
  FStatementUpdateAbilWine.Sql :=
    'update HeroAbilWine set ' +
      'IsDrinkWineDrunk = ?,' +
      'DrinkWineQuality = ?,' +
      'DrinkWineAlcohol = ?,' +
      'AbilAlcohol = ?,' +
      'AbilMaxAlcohol = ?,' +
      'AbilDrinkValue = ?,' +
      'AbilMedicineLevel = ?,' +
      'AbilMedicineValue = ?,' +
      'AbilMaxMedicineValue = ? ' +
    'where HeroID = ?;';
  }

  FStatementInsertAbilNpcAdd := Stms.AddSQLStatement('HeroInsertAbilNpcAdd');
  FStatementInsertAbilNpcAdd.Sql := 'insert into HeroAbilNpcAdd(' + 'HeroID, ' + '`Index`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertGodBlessState := Stms.AddSQLStatement('HeroInsertGodBlessState');
  FStatementInsertGodBlessState.Sql := 'insert into HeroGodBlessState(' + 'HeroID, ' + '`Index`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertMagic := Stms.AddSQLStatement('HeroInsertMagic');
  FStatementInsertMagic.Sql := 'insert into HeroMagic(' + 'HeroID, ' + 'MagicType, ' + 'MagicIndex, ' + 'MagicID,' + 'MagicAttr, ' + 'MagicLevel, ' + 'MagicNewLevel, ' + 'MagicKey, ' + 'MagicTranPoint, ' + 'MagicIsUseItemAdd) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertStatusTime := Stms.AddSQLStatement('HeroInsertStatusTime');
  FStatementInsertStatusTime.Sql := 'insert into HeroStatusTime(' + 'HeroID, ' + '`Index`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertQuestFlag := Stms.AddSQLStatement('HeroInsertQuestFlag');
  FStatementInsertQuestFlag.Sql := 'insert into HeroQuestFlag(' + 'HeroID, ' + '`Index`, ' + '`Value`) ' + 'values(?, ?, ?);';

  FStatementInsertItems := Stms.AddSQLStatement('HeroInsertItems');
  FStatementInsertItems.Sql := 'insert into HeroItems(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'MakeIndex, ' + 'DBIndex, ' + 'Name, ' + 'Dura, ' + 'DuraMax, ' + 'HeroM2DressEffect, ' + 'UpgradeCount, ' + 'IsStartTime, ' + 'LimitTime, ' + 'HeroM2Light, ' + 'Color, ' + 'IsBind, ' + 'BindOption, ' + 'Effect, ' + 'NewLooks, ' + 'NewShape, ' + 'FluteCount, ' + 'PropertyText, ' + 'PropertyTextColor, ' + 'ItemFrom, ' + 'ItemFromMap, ' + 'ItemFromMon, ' + 'ItemFromMaker, ' + 'ItemFromDate, ' + 'InsuranceCount, ' + 'NewExpand3, ' + 'NewExpand4 ' + ') ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ' + '?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  FStatementInsertItemValueAdd := Stms.AddSQLStatement('HeroInsertItemValueAdd');
  FStatementInsertItemValueAdd.Sql := 'insert into HeroItemValueAdd(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + '`Value`) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemElementAdd := Stms.AddSQLStatement('HeroInsertItemElementAdd');
  FStatementInsertItemElementAdd.Sql := 'insert into HeroItemElementAdd(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + '`Value`) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemAddDataByte := Stms.AddSQLStatement('HeroInsertItemAddDataByte');
  FStatementInsertItemAddDataByte.Sql := 'insert into HeroItemAddDataByte(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + '`Value`) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemAddDataInt := Stms.AddSQLStatement('HeroInsertItemAddDataInt');
  FStatementInsertItemAddDataInt.Sql := 'insert into HeroItemAddDataInt(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + '`Value`) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemAddDataText := Stms.AddSQLStatement('HeroInsertItemAddDataText');
  FStatementInsertItemAddDataText.Sql := 'insert into HeroItemAddDataText(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + '`Value`) ' + 'values(?, ?, ?, ?, ?);';

  FStatementInsertItemFlute := Stms.AddSQLStatement('HeroInsertItemFlute');
  FStatementInsertItemFlute.Sql := 'insert into HeroItemFlute(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + '`Value`, ' + 'OverlapCount) ' + 'values(?, ?, ?, ?, ?, ?);';

  FStatementInsertItemProgress := Stms.AddSQLStatement('HeroInsertItemProgress');
  FStatementInsertItemProgress.Sql := 'insert into HeroItemProgress(' + 'HeroID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' + 'IsOpen, ' + 'NameColor, ' + 'Count, ' + 'ShowType, ' + 'Max, ' + '`Value`, ' + 'Level, ' + 'Name) ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

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

  {
  FStatementUpdateAbil.Prepare;
  FStatementUpdateAbilNG.Prepare;
  FStatementUpdateAbilWine.Prepare;
  }

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

procedure TMySqlHeroDB.DoFinal;
begin
  inherited;
  if FStatementGetID <> nil then
    FStatementGetID.Finalize;
  if FStatementGetHeroCount <> nil then
    FStatementGetHeroCount.Finalize;
  if FStatementGetHumanInfo <> nil then
    FStatementGetHumanInfo.Finalize;
  if FStatementGetHumanInfo2 <> nil then
    FStatementGetHumanInfo2.Finalize;

  if FStatementSearchByAccountMatchComplete <> nil then
    FStatementSearchByAccountMatchComplete.Finalize;
  if FStatementSearchByAccountMatchFuzzy <> nil then
    FStatementSearchByAccountMatchFuzzy.Finalize;

  if FStatementSearchByNameMatchComplete <> nil then
    FStatementSearchByNameMatchComplete.Finalize;
  if FStatementSearchByNameMatchFuzzy <> nil then
    FStatementSearchByNameMatchFuzzy.Finalize;

  if FStatementGetHero <> nil then
    FStatementGetHero.Finalize;
  if FStatementGetAbil <> nil then
    FStatementGetAbil.Finalize;
  if FStatementGetAbilNG <> nil then
    FStatementGetAbilNG.Finalize;
  if FStatementGetAbilWine <> nil then
    FStatementGetAbilWine.Finalize;
  if FStatementGetAbilNpcAdd <> nil then
    FStatementGetAbilNpcAdd.Finalize;
  if FStatementGetGodBlessState <> nil then
    FStatementGetGodBlessState.Finalize;
  if FStatementGetMagic <> nil then
    FStatementGetMagic.Finalize;
  if FStatementGetStatusTime <> nil then
    FStatementGetStatusTime.Finalize;
  if FStatementGetQuestFlag <> nil then
    FStatementGetQuestFlag.Finalize;
  if FStatementGetItems <> nil then
    FStatementGetItems.Finalize;
  if FStatementGetItemValueAdd <> nil then
    FStatementGetItemValueAdd.Finalize;
  if FStatementGetItemElementAdd <> nil then
    FStatementGetItemElementAdd.Finalize;
  if FStatementGetItemAddDataByte <> nil then
    FStatementGetItemAddDataByte.Finalize;
  if FStatementGetItemAddDataInt <> nil then
    FStatementGetItemAddDataInt.Finalize;
  if FStatementGetItemAddDataText <> nil then
    FStatementGetItemAddDataText.Finalize;
  if FStatementGetItemFlute <> nil then
    FStatementGetItemFlute.Finalize;
  if FStatementGetItemProgress <> nil then
    FStatementGetItemProgress.Finalize;
  if FStatementGetItemProperty <> nil then
    FStatementGetItemProperty.Finalize;
  if FStatementGetSkillPower <> nil then
    FStatementGetSkillPower.Finalize;

  if FStatementAddHero <> nil then
    FStatementAddHero.Finalize;
  if FStatementDeleteOrRestore <> nil then
    FStatementDeleteOrRestore.Finalize;

  if FStatementUpdateHeroName <> nil then
    FStatementUpdateHeroName.Finalize;
  if FStatementUpdateHumanHeroName <> nil then
    FStatementUpdateHumanHeroName.Finalize;
  if FStatementUpdateHumanHeroName2 <> nil then
    FStatementUpdateHumanHeroName2.Finalize;

  if FStatementUpdateHero <> nil then
    FStatementUpdateHero.Finalize;

  if FStatementInsertAbil <> nil then
    FStatementInsertAbil.Finalize;
  if FStatementInsertAbilNG <> nil then
    FStatementInsertAbilNG.Finalize;
  if FStatementInsertAbilWine <> nil then
    FStatementInsertAbilWine.Finalize;

  {
  if FStatementUpdateAbil <> nil then FStatementUpdateAbil.Finalize;
  if FStatementUpdateAbilNG <> nil then FStatementUpdateAbilNG.Finalize;
  if FStatementUpdateAbilWine <> nil then FStatementUpdateAbilWine.Finalize;
  }

  if FStatementInsertAbilNpcAdd <> nil then
    FStatementInsertAbilNpcAdd.Finalize;
  if FStatementInsertGodBlessState <> nil then
    FStatementInsertGodBlessState.Finalize;
  if FStatementInsertMagic <> nil then
    FStatementInsertMagic.Finalize;
  if FStatementInsertStatusTime <> nil then
    FStatementInsertStatusTime.Finalize;
  if FStatementInsertQuestFlag <> nil then
    FStatementInsertQuestFlag.Finalize;
  if FStatementInsertItems <> nil then
    FStatementInsertItems.Finalize;
  if FStatementInsertItemValueAdd <> nil then
    FStatementInsertItemValueAdd.Finalize;
  if FStatementInsertItemElementAdd <> nil then
    FStatementInsertItemElementAdd.Finalize;
  if FStatementInsertItemAddDataByte <> nil then
    FStatementInsertItemAddDataByte.Finalize;
  if FStatementInsertItemAddDataInt <> nil then
    FStatementInsertItemAddDataInt.Finalize;
  if FStatementInsertItemAddDataText <> nil then
    FStatementInsertItemAddDataText.Finalize;
  if FStatementInsertItemFlute <> nil then
    FStatementInsertItemFlute.Finalize;
  if FStatementInsertItemProgress <> nil then
    FStatementInsertItemProgress.Finalize;
  if FStatementInsertItemProperty <> nil then
    FStatementInsertItemProperty.Finalize;
  if FStatementInsertSkillPower <> nil then
    FStatementInsertSkillPower.Finalize;

  if FStatementGetLevelRankCheckLevel <> nil then
    FStatementGetLevelRankCheckLevel.Finalize;
  if FStatementGetLevelRankTopCount <> nil then
    FStatementGetLevelRankTopCount.Finalize;
  if FStatementGetLevelRankCheckLevelAndCount <> nil then
    FStatementGetLevelRankCheckLevelAndCount.Finalize;
end;

function TMySqlHeroDB.DoGetID(HeroName: string): Integer;
begin
  try
    FStatementGetID.Reset;
    FStatementGetID.OrderBindParamText(HeroName);
    if FStatementGetID.Query and FStatementGetID.Fetch then
      Result := FStatementGetID.GetColumnValueInt(0)
    else
      Result := NO_ID;
  finally
    FStatementGetID.Reset;
  end;
end;

function TMySqlHeroDB.DoSearchByAccount(Account: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer;
var
  //Ret: Integer;
  SearchData: TSerarchRoleData;
begin
  Result := 0;
  if MatchType = smtComplete then
  begin
    try
      FStatementSearchByAccountMatchComplete.Reset;
      FStatementSearchByAccountMatchComplete.OrderBindParamText(Account);

      if FStatementSearchByAccountMatchComplete.Query then
      begin
        while (FStatementSearchByAccountMatchComplete.Fetch) do
        begin
          SearchData.Account := FStatementSearchByAccountMatchComplete.OrderGetColumnValueText;
          SearchData.RoleName := FStatementSearchByAccountMatchComplete.OrderGetColumnValueText;
          SearchData.IsDelete := 0;
          SearchData.Sex := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
          SearchData.Job := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
          SearchData.Level := FStatementSearchByAccountMatchComplete.OrderGetColumnValueInt;
          SearchData.IsHero := True;
          RoleList.Add(@SearchData);

          Inc(Result);
        end;
      end;
    finally
      FStatementSearchByAccountMatchComplete.Reset;
    end;
  end
  else
  begin
    try
      FStatementSearchByAccountMatchFuzzy.Reset;
      FStatementSearchByAccountMatchFuzzy.OrderBindParamText('%' + Account + '%');

      if FStatementSearchByAccountMatchFuzzy.Query then
      begin
        while (FStatementSearchByAccountMatchFuzzy.Fetch) do
        begin
          SearchData.Account := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueText;
          SearchData.RoleName := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueText;
          SearchData.IsDelete := 0;
          SearchData.Sex := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
          SearchData.Job := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
          SearchData.Level := FStatementSearchByAccountMatchFuzzy.OrderGetColumnValueInt;
          SearchData.IsHero := True;
          RoleList.Add(@SearchData);

          Inc(Result);
        end;
      end;
    finally
      FStatementSearchByAccountMatchFuzzy.Reset;
    end;
  end;
end;

function TMySqlHeroDB.DoSearchByName(HeroName: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer;
var
  //Ret: Integer;
  SearchData: TSerarchRoleData;
begin
  Result := 0;

  if MatchType = smtComplete then
  begin
    try
      FStatementSearchByNameMatchComplete.Reset;
      FStatementSearchByNameMatchComplete.OrderBindParamText(HeroName);

      if FStatementSearchByNameMatchComplete.Query then
      begin
        while (FStatementSearchByNameMatchComplete.Fetch) do
        begin
          SearchData.Account := FStatementSearchByNameMatchComplete.OrderGetColumnValueText;
          SearchData.RoleName := FStatementSearchByNameMatchComplete.OrderGetColumnValueText;
          SearchData.IsDelete := 0;
          SearchData.Sex := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
          SearchData.Job := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
          SearchData.Level := FStatementSearchByNameMatchComplete.OrderGetColumnValueInt;
          SearchData.IsHero := True;
          RoleList.Add(@SearchData);

          Inc(Result);
        end;
      end;
    finally
      FStatementSearchByNameMatchComplete.Reset;
    end;
  end
  else
  begin
    try
      FStatementSearchByNameMatchFuzzy.Reset;
      FStatementSearchByNameMatchFuzzy.OrderBindParamText('%' + HeroName + '%');

      if FStatementSearchByNameMatchFuzzy.Query then
      begin
        while (FStatementSearchByNameMatchFuzzy.Fetch) do
        begin
          SearchData.Account := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueText;
          SearchData.RoleName := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueText;
          SearchData.IsDelete := 0;
          SearchData.Sex := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
          SearchData.Job := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
          SearchData.Level := FStatementSearchByNameMatchFuzzy.OrderGetColumnValueInt;
          SearchData.IsHero := True;
          RoleList.Add(@SearchData);

          Inc(Result);
        end;
      end;
    finally
      FStatementSearchByNameMatchFuzzy.Reset;
    end;
  end;
end;

function TMySqlHeroDB.DoGet(HeroName: string; var HeroData: THeroData; var HeroID: Integer): Boolean;
var
  //Ret: Integer;
  I, J, TheType, Index, Index2, Value: Integer;
  Magic: PTHumMagic;
  UserItem: pTUserItem;
  sTemp: string;
begin
  Result := False;

  try
    FStatementGetHero.Reset;
    FStatementGetHero.OrderBindParamText(HeroName);

    if (FStatementGetHero.Query and FStatementGetHero.Fetch) then
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
    FStatementGetAbil.OrderBindParamInt(HeroID);

    if (FStatementGetAbil.Query and FStatementGetAbil.Fetch) then
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
    FStatementGetAbilNG.OrderBindParamInt(HeroID);

    if (FStatementGetAbilNG.Query and FStatementGetAbilNG.Fetch) then
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
    FStatementGetAbilWine.OrderBindParamInt(HeroID);

    if (FStatementGetAbilWine.Query and FStatementGetAbilWine.Fetch) then
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
    FStatementGetAbilNpcAdd.OrderBindParamInt(HeroID);

    if FStatementGetAbilNpcAdd.Query then
    begin
      while (FStatementGetAbilNpcAdd.Fetch) do
      begin
        Index := FStatementGetAbilNpcAdd.OrderGetColumnValueInt;
        Value := FStatementGetAbilNpcAdd.OrderGetColumnValueInt;

        if (Index >= Low(HeroData.AddSaveAbil)) and (Index <= High(HeroData.AddSaveAbil)) then
          HeroData.AddSaveAbil[Index] := Value;
      end;
    end;

    FStatementGetGodBlessState.Reset;
    FStatementGetGodBlessState.OrderBindParamInt(HeroID);

    if FStatementGetGodBlessState.Query then
    begin
      while (FStatementGetGodBlessState.Fetch) do
      begin
        Index := FStatementGetGodBlessState.OrderGetColumnValueInt;
        Value := FStatementGetGodBlessState.OrderGetColumnValueInt;

        if (Index >= Low(HeroData.GodBlessItemsState)) and (Index <= High(HeroData.GodBlessItemsState)) then
          HeroData.GodBlessItemsState[Index] := Value;
      end;
    end;

    FStatementGetMagic.Reset;
    FStatementGetMagic.OrderBindParamInt(HeroID);

    if FStatementGetMagic.Query then
    begin
      while (FStatementGetMagic.Fetch) do
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
      end;
    end;

    FStatementGetStatusTime.Reset;
    FStatementGetStatusTime.OrderBindParamInt(HeroID);

    if FStatementGetStatusTime.Query then
    begin
      while (FStatementGetStatusTime.Fetch) do
      begin
        Index := FStatementGetStatusTime.OrderGetColumnValueInt;
        Value := FStatementGetStatusTime.OrderGetColumnValueInt;

        if (Index >= Low(HeroData.wStatusTimeArr)) and (Index <= High(HeroData.wStatusTimeArr)) then
          HeroData.wStatusTimeArr[Index] := Value;
      end;
    end;

    FStatementGetQuestFlag.Reset;
    FStatementGetQuestFlag.OrderBindParamInt(HeroID);

    if FStatementGetQuestFlag.Query then
    begin
      while (FStatementGetQuestFlag.Fetch) do
      begin
        Index := FStatementGetQuestFlag.OrderGetColumnValueInt;
        Value := FStatementGetQuestFlag.OrderGetColumnValueInt;

        if (Index >= Low(HeroData.QuestFlag)) and (Index <= High(HeroData.QuestFlag)) then
          HeroData.QuestFlag[Index] := Value;
      end;
    end;

    FStatementGetItems.Reset;
    FStatementGetItems.OrderBindParamInt(HeroID);

    if FStatementGetItems.Query then
    begin
      while (FStatementGetItems.Fetch) do
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
      end;
    end;

    FStatementGetItemValueAdd.Reset;
    FStatementGetItemValueAdd.OrderBindParamInt(HeroID);

    if FStatementGetItemValueAdd.Query then
    begin
      while (FStatementGetItemValueAdd.Fetch) do
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
      end;
    end;

    FStatementGetItemElementAdd.Reset;
    FStatementGetItemElementAdd.OrderBindParamInt(HeroID);

    if FStatementGetItemElementAdd.Query then
    begin
      while (FStatementGetItemElementAdd.Fetch) do
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
      end;
    end;

    FStatementGetItemAddDataByte.Reset;
    FStatementGetItemAddDataByte.OrderBindParamInt(HeroID);

    if FStatementGetItemAddDataByte.Query then
    begin
      while (FStatementGetItemAddDataByte.Fetch) do
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
      end;
    end;

    FStatementGetItemAddDataInt.Reset;
    FStatementGetItemAddDataInt.OrderBindParamInt(HeroID);

    if FStatementGetItemAddDataInt.Query then
    begin
      while (FStatementGetItemAddDataInt.Fetch) do
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
      end;
    end;

    FStatementGetItemAddDataText.Reset;
    FStatementGetItemAddDataText.OrderBindParamInt(HeroID);

    if FStatementGetItemAddDataText.Query then
    begin
      while (FStatementGetItemAddDataText.Fetch) do
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
      end;
    end;

    FStatementGetItemFlute.Reset;
    FStatementGetItemFlute.OrderBindParamInt(HeroID);

    if FStatementGetItemFlute.Query then
    begin
      while (FStatementGetItemFlute.Fetch) do
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
      end;
    end;

    FStatementGetItemProgress.Reset;
    FStatementGetItemProgress.OrderBindParamInt(HeroID);

    if FStatementGetItemProgress.Query then
    begin
      while (FStatementGetItemProgress.Fetch) do
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
      end;
    end;

    FStatementGetItemProperty.Reset;
    FStatementGetItemProperty.OrderBindParamInt(HeroID);

    if FStatementGetItemProperty.Query then
    begin
      while (FStatementGetItemProperty.Fetch) do
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
      end;
    end;

    FStatementGetSkillPower.Reset;
    FStatementGetSkillPower.OrderBindParamInt(HeroID);
    if (FStatementGetSkillPower.Query) then
    begin
      while FStatementGetSkillPower.Fetch do
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
      end;
    end;

  finally
    ResetAllGetDataStatement;
  end;
end;

function TMySqlHeroDB.GetHeroUserItem(HeroData: PTHeroData; const ItemType, ItemIndex: Integer): PTUserItem;
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

function TMySqlHeroDB.DoAdd(Account, HumanName: string; HumanID: Integer; HeroName: string; Sex, Job, Hair: Byte; IsDeputyHero: Boolean): Boolean;
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
      FStatementAddHero.OrderBindParamInt(HumanID);                   //'HumanID, ' +
      FStatementAddHero.OrderBindParamText(HeroName);                 //'HeroName, ' +
      FStatementAddHero.OrderBindParamBool(False);                    //'IsDelete, ' +
      FStatementAddHero.OrderBindParamInt(Date2MyDate(Now()));        //'CreateDate, ' +
      FStatementAddHero.OrderBindParamInt(Sex);                       //'Sex, ' +
      FStatementAddHero.OrderBindParamInt(Job);                       //'Job, ' +
      FStatementAddHero.OrderBindParamInt(Hair);                      //'Hair) ' +
      Result := FStatementAddHero.Step;

      if Result then
      begin
        try
          FStatementUpdateHumanHeroName.Reset;
          FStatementUpdateHumanHeroName.OrderBindParamText(HumanHeroName);
          FStatementUpdateHumanHeroName.OrderBindParamText(HumanDeputyHeroName);
          FStatementUpdateHumanHeroName.OrderBindParamInt(HumanID);
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
    FStatementDeleteOrRestore.OrderBindParamInt(1);
    FStatementDeleteOrRestore.OrderBindParamText(HeroName);
    Result := FStatementDeleteOrRestore.Step;
  finally
    FStatementDeleteOrRestore.Reset;
  end;
end;

function TSqliteHeroDB.DoDeleteRestore(HeroName: string): Boolean;
begin
  try
    FStatementDeleteOrRestore.Reset;
    FStatementDeleteOrRestore.OrderBindParamInt(0);
    FStatementDeleteOrRestore.OrderBindParamText(HeroName);
    Result := FStatementDeleteOrRestore.Step;
  finally
    FStatementDeleteOrRestore.Reset;
  end;
end;

}

function TMySqlHeroDB.DoErase(HeroName: string): Boolean;
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
    FStatementGetHumanInfo2.OrderBindParamInt(HeroID);
    if FStatementGetHumanInfo2.Query and FStatementGetHumanInfo2.Fetch then
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

    Execute(Format('delete from HeroGodBlessState where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroMagic where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroQuestFlag where HeroID = %d;', [HeroID]));
    Execute(Format('delete from HeroStatusTime where HeroID = %d;', [HeroID]));

    Execute(Format('delete from HeroSkillPower where HeroID = %d;', [HeroID]));

    Execute(Format('delete from Hero where HeroID = %d;', [HeroID]));

    if (HumanID > 0) and IsChanged then
    begin
      try
        FStatementUpdateHumanHeroName2.Reset;
        FStatementUpdateHumanHeroName2.OrderBindParamBool(False);
        FStatementUpdateHumanHeroName2.OrderBindParamBool(IsStorageHero);
        FStatementUpdateHumanHeroName2.OrderBindParamBool(IsStorageDeputyHero);
        FStatementUpdateHumanHeroName2.OrderBindParamText(HumanHeroName);
        FStatementUpdateHumanHeroName2.OrderBindParamText(HumanDeputyHeroName);
        FStatementUpdateHumanHeroName2.OrderBindParamInt(HumanID);
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

function TMySqlHeroDB.DoSave(HeroID: Integer; HeroData: PTHeroData): Boolean;
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

function TMySqlHeroDB.DoRename(HeroID: Integer; HeroName, NewName: string): Boolean;
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
    FStatementGetHumanInfo.OrderBindParamInt(HeroID);
    if FStatementGetHumanInfo.Query and FStatementGetHumanInfo.Fetch then
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
      FStatementUpdateHeroName.OrderBindParamText(NewName);
      FStatementUpdateHeroName.OrderBindParamInt(HeroID);
      Result := FStatementUpdateHeroName.Step;

      if (Result) and (HumanID > 0) and IsChanged then
      begin
        try
          FStatementUpdateHumanHeroName.Reset;
          FStatementUpdateHumanHeroName.OrderBindParamText(HumanHeroName);
          FStatementUpdateHumanHeroName.OrderBindParamText(HumanDeputyHeroName);
          FStatementUpdateHumanHeroName.OrderBindParamInt(HumanID);
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

function TMySqlHeroDB.DoAssess(HeroID: Integer; HeroName, DeputyHeroName: string): Boolean;
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
    FStatementGetHumanInfo.OrderBindParamInt(HeroID);
    if FStatementGetHumanInfo.Query and FStatementGetHumanInfo.Fetch then
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
        FStatementUpdateHumanHeroName2.OrderBindParamBool(True);
        FStatementUpdateHumanHeroName2.OrderBindParamBool(False);
        FStatementUpdateHumanHeroName2.OrderBindParamBool(False);
        FStatementUpdateHumanHeroName2.OrderBindParamText(HeroName);
        FStatementUpdateHumanHeroName2.OrderBindParamText(DeputyHeroName);
        FStatementUpdateHumanHeroName2.OrderBindParamInt(HumanID);
        FStatementUpdateHumanHeroName2.Step;
      finally
        FStatementUpdateHumanHeroName2.Reset;
      end;
    end;
  end;
end;

function TMySqlHeroDB.SaveHeroData(HeroID: Integer; HeroData: PTHeroData): Boolean;
var
  I, J: Integer;
  HumMagic: PTHumMagic;
  UserItem: PTUserItem;
begin
  Result := False;
  try
    FStatementUpdateHero.Reset;
    FStatementUpdateHero.OrderBindParamInt(HeroData.btSex);                   //  Sex;
    FStatementUpdateHero.OrderBindParamInt(HeroData.btJob);                   //  Job;
    FStatementUpdateHero.OrderBindParamInt(HeroData.btHair);                  //  Hair;
    FStatementUpdateHero.OrderBindParamInt(HeroData.btStatus);                //  Status;
    FStatementUpdateHero.OrderBindParamInt(HeroData.btDir);                   //  Dir;
    FStatementUpdateHero.OrderBindParamInt(HeroData.Abil.Level);              //  Level;
    FStatementUpdateHero.OrderBindParamInt(HeroData.btReLevel);               //  ReLevel;
    FStatementUpdateHero.OrderBindParamDouble(HeroData.rLoyalPoint);          //  LoyalPoint;
    FStatementUpdateHero.OrderBindParamText(HeroData.sCurMap);                //  Map;
    FStatementUpdateHero.OrderBindParamInt(HeroData.wCurX);                   //  X;
    FStatementUpdateHero.OrderBindParamInt(HeroData.wCurY);                   //  Y;
    //FStatementUpdateHero.OrderBindParamText(HeroData.sHomeMap);             //  HomeMap;
    //FStatementUpdateHero.OrderBindParamInt(HeroData.wHomeX);                //  HomeX;
    //FStatementUpdateHero.OrderBindParamInt(HeroData.wHomeY);                //  HomeY;
    FStatementUpdateHero.OrderBindParamInt(HeroData.btAttackMode);            //  AttackMode;
    FStatementUpdateHero.OrderBindParamInt(HeroData.Abil.CreditPoint);        //  CreditPoint;
    FStatementUpdateHero.OrderBindParamInt(HeroData.nPKPoint);                //  PKPoint;
    FStatementUpdateHero.OrderBindParamInt(HeroData.btIncHealth);             //  IncHP;
    FStatementUpdateHero.OrderBindParamInt(HeroData.btIncSpell);              //  IncMP;
    FStatementUpdateHero.OrderBindParamInt(HeroData.btIncHealing);            //  IncHP2;
    FStatementUpdateHero.OrderBindParamInt(HeroData.btFightZoneDieCount);     //  FightZoneDieCount;
    FStatementUpdateHero.OrderBindParamDouble(HeroData.dBodyLuck);            //  BodyLuck;
    FStatementUpdateHero.OrderBindParamInt(HeroData.nHungerStatus);           //  HungerStatus;
    FStatementUpdateHero.OrderBindParamInt(HeroData.nRevivalTime);            //  RevivalTime;
    FStatementUpdateHero.OrderBindParamBool(HeroData.boSaveKillMonExpRate);   //  IsSaveKillMonExpRate;
    FStatementUpdateHero.OrderBindParamInt(HeroData.nKillMonExpRate);         //  KillMonExpRate;
    FStatementUpdateHero.OrderBindParamInt(HeroData.dwKillMonExpRateTime);    //  KillMonExpRateTime;
    FStatementUpdateHero.OrderBindParamBool(HeroData.boAttackHumSavePowerRate);        //  IsSavePowerRate;
    FStatementUpdateHero.OrderBindParamInt(HeroData.nAttackHumPowerRate);              //  PowerRate;
    FStatementUpdateHero.OrderBindParamInt(HeroData.dwAttackHumPowerRateTime);         //  PowerRateTime;
    FStatementUpdateHero.OrderBindParamBool(HeroData.boAttackMonSavePowerRate);        //  IsSavePowerRate;
    FStatementUpdateHero.OrderBindParamInt(HeroData.nAttackMonPowerRate);              //  PowerRate;
    FStatementUpdateHero.OrderBindParamInt(HeroData.dwAttackMonPowerRateTime);         //  PowerRateTime;
    FStatementUpdateHero.OrderBindParamBool(HeroData.boSaveKillMonBurstRate); //  IsSaveKillMonBurstRate;
    FStatementUpdateHero.OrderBindParamInt(HeroData.nKillMonBurstRate);       //  KillMonBurstRate;
    FStatementUpdateHero.OrderBindParamInt(HeroData.dwKillMonBurstRateTime);  //  KillMonBurstRateTime;
    FStatementUpdateHero.OrderBindParamInt(Integer(HeroData.JewelryBoxStatus));      //  JewelryBoxStatus;
    FStatementUpdateHero.OrderBindParamBool(HeroData.boShowFashion);
    ;         //  IsShowFashion;
    FStatementUpdateHero.OrderBindParamBool(HeroData.boShowGodBless);         //  IsShowGodBless;
    FStatementUpdateHero.OrderBindParamInt(HeroData.nActiveFengHao);          //  ActiveFengHao;
    FStatementUpdateHero.OrderBindParamInt(HeroID);

    if not (FStatementUpdateHero.Step) then
      Exit;

    FStatementInsertAbil.Reset;
    FStatementInsertAbil.OrderBindParamInt(HeroID);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.AC1);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.AC2);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MAC1);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MAC2);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.DC1);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.DC2);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MC1);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MC2);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.SC1);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.SC2);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.HP);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MaxHP);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MP);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MaxMP);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.Exp);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MaxExp);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.Weight);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MaxWeight);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.WearWeight);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MaxWearWeight);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.HandWeight);
    FStatementInsertAbil.OrderBindParamInt(HeroData.Abil.MaxHandWeight);
    FStatementInsertAbil.Step;

    FStatementInsertAbilNG.Reset;
    FStatementInsertAbilNG.OrderBindParamInt(HeroID);
    FStatementInsertAbilNG.OrderBindParamBool(HeroData.boTrainingNG);
    FStatementInsertAbilNG.OrderBindParamBool(HeroData.boTrainingXF);
    FStatementInsertAbilNG.OrderBindParamInt(HeroData.AbilNG.Level);
    FStatementInsertAbilNG.OrderBindParamInt(HeroData.AbilNG.NH);
    FStatementInsertAbilNG.OrderBindParamInt(HeroData.AbilNG.MaxNH);
    FStatementInsertAbilNG.OrderBindParamInt(HeroData.AbilNG.Exp);
    FStatementInsertAbilNG.OrderBindParamInt(HeroData.AbilNG.MaxExp);
    FStatementInsertAbilNG.OrderBindParamInt(HeroData.ContinuousMagicOrder[0]);
    FStatementInsertAbilNG.OrderBindParamInt(HeroData.ContinuousMagicOrder[1]);
    FStatementInsertAbilNG.OrderBindParamInt(HeroData.ContinuousMagicOrder[2]);
    FStatementInsertAbilNG.OrderBindParamBool(HeroData.boOpenLastContinuous);
    FStatementInsertAbilNG.OrderBindParamInt(HeroData.btLastContinuousMagicOrder);
    for I := 0 to 4 do
    begin
      FStatementInsertAbilNG.OrderBindParamInt(HeroData.Meridians[I].Level);
      FStatementInsertAbilNG.OrderBindParamInt(HeroData.Meridians[I].BlastHitRate);
      for J := 0 to 4 do
      begin
        FStatementInsertAbilNG.OrderBindParamInt(HeroData.Meridians[I].Acupoints[J])
      end;
    end;
    FStatementInsertAbilNG.Step;

    FStatementInsertAbilWine.Reset;
    FStatementInsertAbilWine.OrderBindParamInt(HeroID);
    FStatementInsertAbilWine.OrderBindParamBool(HeroData.boDrinkWineDrunk);
    FStatementInsertAbilWine.OrderBindParamInt(HeroData.nDrinkWineQuality);
    FStatementInsertAbilWine.OrderBindParamInt(HeroData.nDrinkWineAlcohol);
    FStatementInsertAbilWine.OrderBindParamInt(HeroData.Alcohol.Alcohol);
    FStatementInsertAbilWine.OrderBindParamInt(HeroData.Alcohol.MaxAlcohol);
    FStatementInsertAbilWine.OrderBindParamInt(HeroData.Alcohol.WineDrinkValue);
    FStatementInsertAbilWine.OrderBindParamInt(HeroData.Alcohol.MedicineLevel);
    FStatementInsertAbilWine.OrderBindParamInt(HeroData.Alcohol.MedicineValue);
    FStatementInsertAbilWine.OrderBindParamInt(HeroData.Alcohol.MaxMedicineValue);
    FStatementInsertAbilWine.Step;

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
    Execute('delete from HeroSkillPower where HeroID = ' + IntToStr(HeroID));

    Execute('delete from HeroItems where HeroID = ' + IntToStr(HeroID));

    for I := Low(HeroData.AddSaveAbil) to High(HeroData.AddSaveAbil) do
    begin
      if HeroData.AddSaveAbil[I] <> 0 then
      begin
        FStatementInsertAbilNpcAdd.Reset;
        FStatementInsertAbilNpcAdd.OrderBindParamInt(HeroID);
        FStatementInsertAbilNpcAdd.OrderBindParamInt(I);
        FStatementInsertAbilNpcAdd.OrderBindParamInt(HeroData.AddSaveAbil[I]);
        FStatementInsertAbilNpcAdd.Step;
      end;
    end;

    for I := Low(HeroData.GodBlessItemsState) to High(HeroData.GodBlessItemsState) do
    begin
      if HeroData.GodBlessItemsState[I] <> 0 then
      begin
        FStatementInsertGodBlessState.Reset;
        FStatementInsertGodBlessState.OrderBindParamInt(HeroID);
        FStatementInsertGodBlessState.OrderBindParamInt(I);
        FStatementInsertGodBlessState.OrderBindParamInt(HeroData.GodBlessItemsState[I]);
        FStatementInsertGodBlessState.Step;
      end;
    end;

    for I := Low(HeroData.Magics) to High(HeroData.Magics) do
    begin
      HumMagic := @HeroData.Magics[I];
      if HumMagic.wMagIdx > 0 then
      begin
        FStatementInsertMagic.Reset;
        FStatementInsertMagic.OrderBindParamInt(HeroID);
        FStatementInsertMagic.OrderBindParamInt(1);
        FStatementInsertMagic.OrderBindParamInt(I);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.wMagIdx);
        FStatementInsertMagic.OrderBindParamInt(Integer(HumMagic.MagicAttr));
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btLevel);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btNewLevel);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btKey);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.nTranPoint);
        FStatementInsertMagic.OrderBindParamBool(HumMagic.boUsesItemAdd);
        FStatementInsertMagic.Step;
      end;
    end;

    for I := Low(HeroData.NGMagics) to High(HeroData.NGMagics) do
    begin
      HumMagic := @HeroData.NGMagics[I];
      if HumMagic.wMagIdx > 0 then
      begin
        FStatementInsertMagic.Reset;
        FStatementInsertMagic.OrderBindParamInt(HeroID);
        FStatementInsertMagic.OrderBindParamInt(2);
        FStatementInsertMagic.OrderBindParamInt(I);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.wMagIdx);
        FStatementInsertMagic.OrderBindParamInt(Integer(HumMagic.MagicAttr));
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btLevel);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btNewLevel);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btKey);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.nTranPoint);
        FStatementInsertMagic.OrderBindParamInt(Integer(HumMagic.boUsesItemAdd));
        FStatementInsertMagic.Step;
      end;
    end;

    for I := Low(HeroData.ContinuousMagics) to High(HeroData.ContinuousMagics) do
    begin
      HumMagic := @HeroData.ContinuousMagics[I];
      if HumMagic.wMagIdx > 0 then
      begin
        FStatementInsertMagic.Reset;
        FStatementInsertMagic.OrderBindParamInt(HeroID);
        FStatementInsertMagic.OrderBindParamInt(3);
        FStatementInsertMagic.OrderBindParamInt(I);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.wMagIdx);
        FStatementInsertMagic.OrderBindParamInt(Integer(HumMagic.MagicAttr));
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btLevel);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btNewLevel);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.btKey);
        FStatementInsertMagic.OrderBindParamInt(HumMagic.nTranPoint);
        FStatementInsertMagic.OrderBindParamInt(Integer(HumMagic.boUsesItemAdd));
        FStatementInsertMagic.Step;
      end;
    end;

    for I := Low(HeroData.wStatusTimeArr) to High(HeroData.wStatusTimeArr) do
    begin
      if HeroData.wStatusTimeArr[I] <> 0 then
      begin
        FStatementInsertStatusTime.Reset;
        FStatementInsertStatusTime.OrderBindParamInt(HeroID);
        FStatementInsertStatusTime.OrderBindParamInt(I);
        FStatementInsertStatusTime.OrderBindParamInt(HeroData.wStatusTimeArr[I]);
        FStatementInsertStatusTime.Step;
      end;
    end;

    for I := Low(HeroData.QuestFlag) to High(HeroData.QuestFlag) do
    begin
      if HeroData.QuestFlag[I] <> 0 then
      begin
        FStatementInsertQuestFlag.Reset;
        FStatementInsertQuestFlag.OrderBindParamInt(HeroID);
        FStatementInsertQuestFlag.OrderBindParamInt(I);
        FStatementInsertQuestFlag.OrderBindParamInt(HeroData.QuestFlag[I]);
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
        FStatementInsertSkillPower.OrderBindParamInt(HeroID);
        FStatementInsertSkillPower.OrderBindParamInt(I);
        FStatementInsertSkillPower.OrderBindParamInt(HeroData.NpcSkillPowerAdd[I].HumanAttackPercent);
        FStatementInsertSkillPower.OrderBindParamInt(HeroData.NpcSkillPowerAdd[I].HumanAttackValue);
        FStatementInsertSkillPower.OrderBindParamInt(HeroData.NpcSkillPowerAdd[I].MonAttackPercent);
        FStatementInsertSkillPower.OrderBindParamInt(HeroData.NpcSkillPowerAdd[I].MonAttackValue);
        FStatementInsertSkillPower.OrderBindParamInt(HeroData.NpcSkillPowerAdd[I].DefensePercent);
        FStatementInsertSkillPower.OrderBindParamInt(HeroData.NpcSkillPowerAdd[I].DefenseValue);
        FStatementInsertSkillPower.OrderBindParamInt(HeroData.NpcSkillPowerAdd[I].RemainingTime);

        FStatementInsertSkillPower.Step;
      end;
    end;

    Result := True;
  finally
    ResetAllSaveDataStatement;
  end;
end;

procedure TMySqlHeroDB.AddHeroItemToDB(UserItem: PTUserItem; HeroID, ItemType, ItemIndex: Integer);
var
  J: Integer;
begin
  FStatementInsertItems.Reset;
  FStatementInsertItems.OrderBindParamInt(HeroID);
  FStatementInsertItems.OrderBindParamInt(ItemType);
  FStatementInsertItems.OrderBindParamInt(ItemIndex);
  FStatementInsertItems.OrderBindParamInt(UserItem.MakeIndex);
  FStatementInsertItems.OrderBindParamInt(UserItem.wIndex);
  FStatementInsertItems.OrderBindParamText(UserItem.Name);
  FStatementInsertItems.OrderBindParamInt(UserItem.Dura);
  FStatementInsertItems.OrderBindParamInt(UserItem.DuraMax);
  FStatementInsertItems.OrderBindParamInt(UserItem.dwHeroM2DressEffect);
  FStatementInsertItems.OrderBindParamInt(UserItem.btUpgradeCount);
  FStatementInsertItems.OrderBindParamBool(UserItem.boStartTime);
  FStatementInsertItems.OrderBindParamInt(UserItem.nLimitTime);
  FStatementInsertItems.OrderBindParamInt(UserItem.btHeroM2Light);
  FStatementInsertItems.OrderBindParamInt(UserItem.btColor);
  FStatementInsertItems.OrderBindParamBool(UserItem.boIsBind);
  FStatementInsertItems.OrderBindParamInt(UserItem.btBindOption);
  FStatementInsertItems.OrderBindParamInt(UserItem.wEffect);
  FStatementInsertItems.OrderBindParamInt(UserItem.wNewLooks);
  FStatementInsertItems.OrderBindParamInt(UserItem.wNewShape);
  FStatementInsertItems.OrderBindParamInt(UserItem.btFluteCount);
  FStatementInsertItems.OrderBindParamText(UserItem.CustomProperty.sText);
  FStatementInsertItems.OrderBindParamInt(UserItem.CustomProperty.btTextColor);
  FStatementInsertItems.OrderBindParamInt(Integer(UserItem.ItemFrom.ItemForm));
  FStatementInsertItems.OrderBindParamText(UserItem.ItemFrom.sMapName);
  FStatementInsertItems.OrderBindParamText(UserItem.ItemFrom.sMonName);
  FStatementInsertItems.OrderBindParamText(UserItem.ItemFrom.sMakerName);
  FStatementInsertItems.OrderBindParamDouble(UserItem.ItemFrom.DateTime);
  FStatementInsertItems.OrderBindParamInt(UserItem.wInsuranceCount);
  FStatementInsertItems.OrderBindParamInt(UserItem.wNewExpand3);
  FStatementInsertItems.OrderBindParamInt(UserItem.wNewExpand4);
  FStatementInsertItems.Step;


  //---------------------------------------------------------------------------------------------

  for J := Low(UserItem.btValue) to High(UserItem.btValue) do
  begin
    if UserItem.btValue[J] <> 0 then
    begin
      FStatementInsertItemValueAdd.Reset;
      FStatementInsertItemValueAdd.OrderBindParamInt(HeroID);
      FStatementInsertItemValueAdd.OrderBindParamInt(ItemType);
      FStatementInsertItemValueAdd.OrderBindParamInt(ItemIndex);
      FStatementInsertItemValueAdd.OrderBindParamInt(J);
      FStatementInsertItemValueAdd.OrderBindParamInt(UserItem.btValue[J]);
      FStatementInsertItemValueAdd.Step;
    end;
  end;

  for J := Low(UserItem.btNewValue) to High(UserItem.btNewValue) do
  begin
    if UserItem.btNewValue[J] <> 0 then
    begin
      FStatementInsertItemElementAdd.Reset;
      FStatementInsertItemElementAdd.OrderBindParamInt(HeroID);
      FStatementInsertItemElementAdd.OrderBindParamInt(ItemType);
      FStatementInsertItemElementAdd.OrderBindParamInt(ItemIndex);
      FStatementInsertItemElementAdd.OrderBindParamInt(J);
      FStatementInsertItemElementAdd.OrderBindParamInt(UserItem.btNewValue[J]);
      FStatementInsertItemElementAdd.Step;
    end;
  end;

  for J := Low(UserItem.btAddDataByte) to High(UserItem.btAddDataByte) do
  begin
    if UserItem.btAddDataByte[J] <> 0 then
    begin
      FStatementInsertItemAddDataByte.Reset;
      FStatementInsertItemAddDataByte.OrderBindParamInt(HeroID);
      FStatementInsertItemAddDataByte.OrderBindParamInt(ItemType);
      FStatementInsertItemAddDataByte.OrderBindParamInt(ItemIndex);
      FStatementInsertItemAddDataByte.OrderBindParamInt(J);
      FStatementInsertItemAddDataByte.OrderBindParamInt(UserItem.btAddDataByte[J]);
      FStatementInsertItemAddDataByte.Step;
    end;
  end;

  for J := Low(UserItem.nAddDataInt) to High(UserItem.nAddDataInt) do
  begin
    if UserItem.nAddDataInt[J] <> 0 then
    begin
      FStatementInsertItemAddDataInt.Reset;
      FStatementInsertItemAddDataInt.OrderBindParamInt(HeroID);
      FStatementInsertItemAddDataInt.OrderBindParamInt(ItemType);
      FStatementInsertItemAddDataInt.OrderBindParamInt(ItemIndex);
      FStatementInsertItemAddDataInt.OrderBindParamInt(J);
      FStatementInsertItemAddDataInt.OrderBindParamInt(UserItem.nAddDataInt[J]);
      FStatementInsertItemAddDataInt.Step;
    end;
  end;

  for J := Low(UserItem.sAddDataText) to High(UserItem.sAddDataText) do
  begin
    if UserItem.sAddDataText[J] <> '' then
    begin
      FStatementInsertItemAddDataText.Reset;
      FStatementInsertItemAddDataText.OrderBindParamInt(HeroID);
      FStatementInsertItemAddDataText.OrderBindParamInt(ItemType);
      FStatementInsertItemAddDataText.OrderBindParamInt(ItemIndex);
      FStatementInsertItemAddDataText.OrderBindParamInt(J);
      FStatementInsertItemAddDataText.OrderBindParamText(UserItem.sAddDataText[J]);
      FStatementInsertItemAddDataText.Step;
    end;
  end;

  for J := Low(UserItem.Flutes) to High(UserItem.Flutes) do
  begin
    if UserItem.Flutes[J].GemIndex <> 0 then
    begin
      FStatementInsertItemFlute.Reset;
      FStatementInsertItemFlute.OrderBindParamInt(HeroID);
      FStatementInsertItemFlute.OrderBindParamInt(ItemType);
      FStatementInsertItemFlute.OrderBindParamInt(ItemIndex);
      FStatementInsertItemFlute.OrderBindParamInt(J);
      FStatementInsertItemFlute.OrderBindParamInt(UserItem.Flutes[J].GemIndex);
      FStatementInsertItemFlute.OrderBindParamInt(UserItem.Flutes[J].GemCount);
      FStatementInsertItemFlute.Step;
    end;
  end;

  for J := Low(UserItem.Progress) to High(UserItem.Progress) do
  begin
    if UserItem.Progress[J].boOpen then
    begin
      FStatementInsertItemProgress.Reset;
      FStatementInsertItemProgress.OrderBindParamInt(HeroID);
      FStatementInsertItemProgress.OrderBindParamInt(ItemType);
      FStatementInsertItemProgress.OrderBindParamInt(ItemIndex);
      FStatementInsertItemProgress.OrderBindParamInt(J);
      FStatementInsertItemProgress.OrderBindParamBool(UserItem.Progress[J].boOpen);
      FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].btNameColor);
      FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].btCount);
      FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].btShowType);
      FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].wMax);
      FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].wValue);
      FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].wLevel);
      FStatementInsertItemProgress.OrderBindParamText(UserItem.Progress[J].sName);
      FStatementInsertItemProgress.Step;
    end;
  end;

  for J := Low(UserItem.CustomProperty.Properties) to High(UserItem.CustomProperty.Properties) do
  begin
    if (UserItem.CustomProperty.Properties[J].nValues[0] > 0) or (UserItem.CustomProperty.Properties[J].nValues[1] > 0) or (UserItem.CustomProperty.Properties[J].nValues[2] > 0) then
    begin
      FStatementInsertItemProperty.Reset;
      FStatementInsertItemProperty.OrderBindParamInt(HeroID);
      FStatementInsertItemProperty.OrderBindParamInt(ItemType);
      FStatementInsertItemProperty.OrderBindParamInt(ItemIndex);
      FStatementInsertItemProperty.OrderBindParamInt(J);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btColor);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btBindType);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btShowFlag);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btPercent);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btHintModule);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].nValues[0]);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].nValues[1]);
      FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].nValues[2]);
      FStatementInsertItemProperty.Step;
    end;
  end;
end;

procedure TMySqlHeroDB.DoGetRankData(MinLevel, MaxLevel, TopCount: LongWord; HeroRankList, WarriorRankList, WizardRankList, TaoistRankList: TRoleRankList);
var
  {Ret,} Index: Integer;
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
      FStatementGetLevelRankTopCount.OrderBindParamInt(0);
      FStatementGetLevelRankTopCount.OrderBindParamInt(1);
      FStatementGetLevelRankTopCount.OrderBindParamInt(2);
      FStatementGetLevelRankTopCount.OrderBindParamInt(QueryCount);
      Index := 0;

      if FStatementGetLevelRankTopCount.Query then
      begin
        while (FStatementGetLevelRankTopCount.Fetch) do
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
        end;
      end;

      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindParamInt(0);
      FStatementGetLevelRankTopCount.OrderBindParamInt(0);
      FStatementGetLevelRankTopCount.OrderBindParamInt(0);
      FStatementGetLevelRankTopCount.OrderBindParamInt(QueryCount);
      Index := 0;

      if FStatementGetLevelRankTopCount.Query then
      begin
        while (FStatementGetLevelRankTopCount.Fetch) do
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
        end;
      end;

      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindParamInt(1);
      FStatementGetLevelRankTopCount.OrderBindParamInt(1);
      FStatementGetLevelRankTopCount.OrderBindParamInt(1);
      FStatementGetLevelRankTopCount.OrderBindParamInt(QueryCount);
      Index := 0;

      if FStatementGetLevelRankTopCount.Query then
      begin
        while (FStatementGetLevelRankTopCount.Fetch) do
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
        end;
      end;

      FStatementGetLevelRankTopCount.Reset;
      FStatementGetLevelRankTopCount.OrderBindParamInt(2);
      FStatementGetLevelRankTopCount.OrderBindParamInt(2);
      FStatementGetLevelRankTopCount.OrderBindParamInt(2);
      FStatementGetLevelRankTopCount.OrderBindParamInt(QueryCount);
      Index := 0;

      if FStatementGetLevelRankTopCount.Query then
      begin
        while (FStatementGetLevelRankTopCount.Fetch) do
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
        end;
      end;
    finally
      FStatementGetLevelRankTopCount.Reset;
    end;
  end
  else if TopCount = 0 then
  begin
    try
      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MaxLevel);
      Index := 0;

      if FStatementGetLevelRankCheckLevel.Query then
      begin
        while (FStatementGetLevelRankCheckLevel.Fetch) do
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
          RankData.HeroName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
          RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;

          HeroRankList.Add(@RankData);
          Inc(Index);
        end;
      end;

      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MaxLevel);
      Index := 0;

      if FStatementGetLevelRankCheckLevel.Query then
      begin
        while (FStatementGetLevelRankCheckLevel.Fetch) do
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
          RankData.HeroName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
          RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;

          WarriorRankList.Add(@RankData);
          Inc(Index);
        end;
      end;

      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MaxLevel);
      Index := 0;

      if FStatementGetLevelRankCheckLevel.Query then
      begin
        while (FStatementGetLevelRankCheckLevel.Fetch) do
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
          RankData.HeroName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
          RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;

          WizardRankList.Add(@RankData);
          Inc(Index);
        end;
      end;

      FStatementGetLevelRankCheckLevel.Reset;
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevel.OrderBindParamInt(MaxLevel);
      Index := 0;

      if FStatementGetLevelRankCheckLevel.Query then
      begin
        while (FStatementGetLevelRankCheckLevel.Fetch) do
        begin
          RankData.RankIndex := Index;
          RankData.HumanName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
          RankData.HeroName := FStatementGetLevelRankCheckLevel.OrderGetColumnValueText;
          RankData.Level := FStatementGetLevelRankCheckLevel.OrderGetColumnValueInt;

          TaoistRankList.Add(@RankData);
          Inc(Index);
        end;
      end;
    finally
      FStatementGetLevelRankCheckLevel.Reset;
    end;
  end
  else
  begin
    try
      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(QueryCount);
      Index := 0;

      if FStatementGetLevelRankCheckLevelAndCount.Query then
      begin
        while (FStatementGetLevelRankCheckLevelAndCount.Fetch) do
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
        end;
      end;

      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(0);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(QueryCount);
      Index := 0;

      if FStatementGetLevelRankCheckLevelAndCount.Query then
      begin
        while (FStatementGetLevelRankCheckLevelAndCount.Fetch) do
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
        end;
      end;

      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(1);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(QueryCount);
      Index := 0;

      if FStatementGetLevelRankCheckLevelAndCount.Query then
      begin
        while (FStatementGetLevelRankCheckLevelAndCount.Fetch) do
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
        end;
      end;

      FStatementGetLevelRankCheckLevelAndCount.Reset;
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(2);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MinLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(MaxLevel);
      FStatementGetLevelRankCheckLevelAndCount.OrderBindParamInt(QueryCount);
      Index := 0;

      if FStatementGetLevelRankCheckLevelAndCount.Query then
      begin
        while (FStatementGetLevelRankCheckLevelAndCount.Fetch) do
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
        end;
      end;
    finally
      FStatementGetLevelRankCheckLevelAndCount.Reset;
    end;
  end;
end;

procedure TMySqlHeroDB.BeginTransaction;
begin
  TMySqlRoleDB(Owner).FDB.StartTransaction;
end;

procedure TMySqlHeroDB.Commit;
begin
  TMySqlRoleDB(Owner).FDB.Commit;
end;

procedure TMySqlHeroDB.RollBack;
begin
  TMySqlRoleDB(Owner).FDB.RollBack;
end;

procedure TMySqlHeroDB.Execute(Sql: string);
begin
  TMySqlRoleDB(Owner).FDB.Exec(Sql);
end;

procedure TMySqlHeroDB.ResetAllGetDataStatement;
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

procedure TMySqlHeroDB.ResetAllSaveDataStatement;
begin
  FStatementUpdateHero.Reset;

  FStatementInsertAbil.Reset;
  FStatementInsertAbilNG.Reset;
  FStatementInsertAbilWine.Reset;

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

{ TMySqlRoleDB }

constructor TMySqlRoleDB.Create;
begin
  inherited;
  FDB := nil;
  FMySqlLib := nil;
end;

destructor TMySqlRoleDB.Destroy;
begin
  if FDB <> nil then
    FDB.Free;

  if FMySqlLib <> nil then
    FMySqlLib.Free;
  inherited;
end;

function TMySqlRoleDB.GetHumanDBClass: THumanDBClass;
begin
  Result := TMySqlHumanDB;
end;

function TMySqlRoleDB.GetHeroDBClass: THeroDBClass;
begin
  Result := TMySqlHeroDB;
end;

procedure TMySqlRoleDB.UpdateDB_1;
begin
  FDB.StartTransaction;
  try

    FDB.Exec('ALTER TABLE HumanItemProperty ADD COLUMN `Value2` INTEGER DEFAULT 0;');
    FDB.Exec('ALTER TABLE HumanItemProperty ADD COLUMN `Value3` INTEGER DEFAULT 0;');
    FDB.Exec('ALTER TABLE HeroItemProperty ADD COLUMN `Value2` INTEGER DEFAULT 0;');
    FDB.Exec('ALTER TABLE HeroItemProperty ADD COLUMN `Value3` INTEGER DEFAULT 0;');

    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("role_version", ' + MYSQL_DBVERSION + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlRoleDB.UpdateDB_2;
var
  S: string;
begin
  FDB.StartTransaction;
  try
    (*
    S :=

    'CREATE TABLE `HumanSkillPower` (' + sLineBreak +
    '  `HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '  `SkillID`  INTEGER NOT NULL,' + sLineBreak +
    '  `AttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `DefenseValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `IsFixedValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `Target`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `RemainingTime`  INTEGER DEFAULT 0,' + sLineBreak +
    '  PRIMARY KEY (`HumanID`, `SkillID`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HeroSkillPower` (' + sLineBreak +
    '  `HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '  `SkillID`  INTEGER NOT NULL,' + sLineBreak +
    '  `AttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `DefenseValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `IsFixedValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `Target`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `RemainingTime`  INTEGER DEFAULT 0,' + sLineBreak +
    '  PRIMARY KEY (`HeroID`, `SkillID`)' + sLineBreak +
    ');' + sLineBreak +

    'REPLACE INTO db_constant(ConstName, ConstValue) values ("role_version", ' + MYSQL_DBVERSION  + ');';
    *)

    S := 'DROP TABLE IF EXISTS `HumanSkillPower`;' + sLineBreak + 'DROP TABLE IF EXISTS `HeroSkillPower`;' + sLineBreak;

    FDB.Exec(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlRoleDB.UpdateDB_3;
var
  S: string;
begin
  FDB.StartTransaction;
  try
    S :=
'CREATE TABLE `HumanSkillPower` (' + sLineBreak + '  `HumanID`  INTEGER NOT NULL,' + sLineBreak + '  `SkillID`  INTEGER NOT NULL,' + sLineBreak + '  `HumanAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak + '  `HumanAttackValue`  INTEGER DEFAULT 0,' + sLineBreak + '  `MonAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak + '  `MonAttackValue`  INTEGER DEFAULT 0,' + sLineBreak + '  `DefensePercent`  INTEGER DEFAULT 0,' + sLineBreak + '  `DefenseValue`  INTEGER DEFAULT 0,' + sLineBreak + '  `RemainingTime`  INTEGER DEFAULT 0,' + sLineBreak + '  PRIMARY KEY (`HumanID`, `SkillID`)' + sLineBreak + ');' + sLineBreak +
'CREATE TABLE `HeroSkillPower` (' + sLineBreak + '  `HeroID`  INTEGER NOT NULL,' + sLineBreak + '  `SkillID`  INTEGER NOT NULL,' + sLineBreak + '  `HumanAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak + '  `HumanAttackValue`  INTEGER DEFAULT 0,' + sLineBreak + '  `MonAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak + '  `MonAttackValue`  INTEGER DEFAULT 0,' + sLineBreak + '  `DefensePercent`  INTEGER DEFAULT 0,' + sLineBreak + '  `DefenseValue`  INTEGER DEFAULT 0,' + sLineBreak + '  `RemainingTime`  INTEGER DEFAULT 0,' + sLineBreak + '  PRIMARY KEY (`HeroID`, `SkillID`)' + sLineBreak + ');' + sLineBreak +
'REPLACE INTO db_constant(ConstName, ConstValue) values ("role_version", ' + MYSQL_DBVERSION + ');';

    FDB.Exec(S);
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlRoleDB.UpdateDB_4;
begin
  FDB.StartTransaction;
  try

    FDB.Exec('ALTER TABLE Human ADD COLUMN `ExtBagPageCount` INTEGER DEFAULT 0;');
    FDB.Exec('ALTER TABLE Human ADD COLUMN `ExtBagOpenItemCount` INTEGER DEFAULT 0;');

    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("role_version", ' + MYSQL_DBVERSION + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlRoleDB.UpdateDB_5;
begin
  FDB.StartTransaction;
  try

    FDB.Exec('ALTER TABLE HumanItemFlute ADD COLUMN `OverlapCount` INTEGER DEFAULT 0;');
    FDB.Exec('ALTER TABLE HeroItemFlute ADD COLUMN `OverlapCount` INTEGER DEFAULT 0;');

    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("role_version", ' + MYSQL_DBVERSION + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlRoleDB.UpdateDB_6;
begin
  FDB.StartTransaction;
  try
    FDB.Exec('ALTER TABLE Human ADD COLUMN `AddMaxWeight` INTEGER DEFAULT 0;');

    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("role_version", ' + MYSQL_DBVERSION + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlRoleDB.UpdateDB_7;
begin
  FDB.StartTransaction;
  try
    FDB.Exec('ALTER TABLE HumanItems ADD COLUMN `NewExpand3` INTEGER DEFAULT 0;');
    FDB.Exec('ALTER TABLE HumanItems ADD COLUMN `NewExpand4` INTEGER DEFAULT 0;');

    FDB.Exec('ALTER TABLE HeroItems ADD COLUMN `NewExpand3` INTEGER DEFAULT 0;');
    FDB.Exec('ALTER TABLE HeroItems ADD COLUMN `NewExpand4` INTEGER DEFAULT 0;');

    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("role_version", ' + MYSQL_DBVERSION + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlRoleDB.UpdateDB_8;
begin
  FDB.StartTransaction;
  try
    FDB.Exec('ALTER TABLE HumanItemProperty ADD COLUMN `HintModule` INTEGER DEFAULT 0;');
    FDB.Exec('ALTER TABLE HeroItemProperty ADD COLUMN `HintModule` INTEGER DEFAULT 0;');

    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("role_version", ' + MYSQL_DBVERSION + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlRoleDB.UpdateDB_9;
var
  S: string;
begin
  FDB.StartTransaction;
  try
    S :=
'CREATE TABLE `HumanVariableZ` (' + sLineBreak + '`HumanID`  INTEGER NOT NULL,' + sLineBreak + '`Index`  INTEGER NOT NULL,' + sLineBreak + '`Value`  VARCHAR(100),' + sLineBreak + 'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak + 'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak + ');';

    FDB.Exec(S);
    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("role_version", ' + MYSQL_DBVERSION + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlRoleDB.UpdateDB_10;
var
  S: string;
begin
  FDB.StartTransaction;
  try
    S :=
'CREATE TABLE `HumanMoney` (' + sLineBreak + '`HumanID`  INTEGER NOT NULL,' + sLineBreak + '`MoneyName`  VARCHAR(30),' + sLineBreak + '`Value`  INTEGER DEFAULT 0,' + sLineBreak +//    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
  'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak + ');';

    FDB.Exec(S);
    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("role_version", ' + MYSQL_DBVERSION + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlRoleDB.DoInit;
var
  sm: TMySqlStatement;
  DB_Version: Integer;
begin
  //
  if not Assigned(FDB) then
  begin
    FMySqlLib := TMySQLLib.Create(nil);
    FMySqlLib.Load('', 'libmysql-32.dll');

    FDB := TMySQLDataBase.Create(FMySqlLib);
    FDB.Init;
    FDB.OnRequest := OnRequest;

    FDB.Connect(g_sDataSaveDBServer, g_sDataSaveDBUser, g_sDataSaveDBPassword, g_sDataSaveDataBase, g_wDataSaveDBPort, CLIENT_MULTI_STATEMENTS);
    FDB.CharacterSetName := 'utf8';

    DB_Version := 0;
    sm := FDB.Statements.AddSQLStatement('get_db_constant_value');
    try
      sm.Sql := 'select ConstValue from db_constant where ConstName = ''role_version'';';
      sm.Prepare;
      if sm.Query and sm.Fetch then
      begin
        DB_Version := sm.OrderGetColumnValueInt;
      end;
      sm.Reset;
    finally
      FDB.Statements.Clear;
    end;

    if DB_Version = 20180719 then
    begin
      UpdateDB_1;
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;
      UpdateDB_7;
      UpdateDB_8;
      UpdateDB_9;
      UpdateDB_10;
    end
    else if DB_Version = 20190314 then
    begin
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;
      UpdateDB_7;
      UpdateDB_8;
      UpdateDB_9;
      UpdateDB_10;
    end
    else if DB_Version = 20190318 then
    begin
      UpdateDB_2;
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;
      UpdateDB_7;
      UpdateDB_8;
      UpdateDB_9;
      UpdateDB_10;
    end
    else if DB_Version = 20190319 then
    begin
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;
      UpdateDB_7;
      UpdateDB_8;
      UpdateDB_9;
      UpdateDB_10;
    end
    else if DB_Version = 20190512 then
    begin
      UpdateDB_5;
      UpdateDB_6;
      UpdateDB_7;
      UpdateDB_8;
      UpdateDB_9;
      UpdateDB_10;
    end
    else if DB_Version = 20190606 then
    begin
      UpdateDB_6;
      UpdateDB_7;
      UpdateDB_8;
      UpdateDB_9;
      UpdateDB_10;
    end
    else if DB_Version = 20190802 then
    begin
      UpdateDB_7;
      UpdateDB_8;
      UpdateDB_9;
      UpdateDB_10;
    end
    else if DB_Version = 20190928 then
    begin
      UpdateDB_8;
      UpdateDB_9;
      UpdateDB_10;
    end
    else if DB_Version = 20200813 then
    begin
      UpdateDB_9;
      UpdateDB_10;
    end
    else if DB_Version = 20220105 then
    begin
      UpdateDB_10;
    end;

    FLastRequestTick := GetTickCount;
  end;
  inherited;
end;

procedure TMySqlRoleDB.DoFainal;
begin
  inherited;

  if FDB <> nil then
  begin
    FDB.Disconnect;
    FDB.Free;
    FDB := nil;
  end;

  if FMySqlLib <> nil then
  begin
    FMySqlLib.Free;
    FMySqlLib := nil;
  end;
end;

procedure TMySqlRoleDB.Commit;
begin
  FDB.Commit;
end;

procedure TMySqlRoleDB.RollBack;
begin
  FDB.RollBack;
end;

procedure TMySqlRoleDB.BeginTransaction;
begin
  FDB.StartTransaction;
end;

procedure TMySqlRoleDB.Run;
begin
  inherited;
  if (FDB <> nil) and (FDB.MySQL <> nil) and (GetTickCount - FLastRequestTick >= 10 * 60000) then
  begin
    FDB.Exec('select 1');
  end;
end;

procedure TMySqlRoleDB.OnRequest(Sender: TObject);
begin
  FLastRequestTick := GetTickCount;
end;

end.


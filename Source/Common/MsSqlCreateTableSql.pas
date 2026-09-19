unit MsSqlCreateTableSql;

interface

uses
  Windows, Classes, SysUtils;

const
  MSSQL_DBVERSION = '20180719';

const
  MSSQL_CEATE_DB_CONSTANT =
    'if object_id(''db_constant'', ''u'') is null ' + sLineBreak +
    'CREATE TABLE db_constant(' + sLineBreak +
    '  ConstName varchar(30) NOT NULL,' + sLineBreak +
    '  ConstValue INT,' + sLineBreak +
    '  PRIMARY KEY (ConstName) ' + sLineBreak +
    ');';

  MSSQL_CREATE_ACCOUNT_TABLES =
    'if object_id(''Account'', ''u'') is not null drop table Account;' + sLineBreak +
    'CREATE TABLE Account  (' + sLineBreak +
    '  Account varchar(10) NOT NULL,' + sLineBreak +
    '  Password varchar(10) NOT NULL,' + sLineBreak +
    '  UserName varchar(20) NOT NULL,' + sLineBreak +
    '  Disable INT DEFAULT 0,' + sLineBreak +
    '  IDCard varchar(18),' + sLineBreak +
    '  BirthDay varchar(10),' + sLineBreak +
    '  Questions1 varchar(20),' + sLineBreak +
    '  Answers1 varchar(12),' + sLineBreak +
    '  Questions2 varchar(20),' + sLineBreak +
    '  Answers2 varchar(12),' + sLineBreak +
    '  Phone varchar(14),' + sLineBreak +
    '  MobilePhone varchar(13),' + sLineBreak +
    '  Mail varchar(40),' + sLineBreak +
    '  L2Password varchar(20),' + sLineBreak +
    '  CreateDate INT,' + sLineBreak +
    '  LoginDate INT,' + sLineBreak +
    '  LoginMac varchar(32),' + sLineBreak +
    '  LoginIP INT,' + sLineBreak +
    '  LastActionTick INT,' + sLineBreak +
    '  ErrorCount INT,' + sLineBreak +
    '  Memo varchar(20),' + sLineBreak +
    '  PRIMARY KEY (Account)' + sLineBreak +
    ');' + sLineBreak +

    MSSQL_CEATE_DB_CONSTANT + sLineBreak +

    'IF EXISTS(SELECT 1 FROM db_constant WHERE ConstName = ''account_version'')' + sLineBreak +
    'BEGIN' + sLineBreak +
    'UPDATE db_constant SET ConstValue = ' + MSSQL_DBVERSION + ' WHERE ConstName = ''account_version''' + sLineBreak +
    'END' + sLineBreak +
    'ELSE' + sLineBreak +
    'BEGIN' + sLineBreak +
    'INSERT INTO db_constant(ConstName, ConstValue) VALUES(''account_version'', ' + MSSQL_DBVERSION + ')' + sLineBreak +
    'END';



  //////////////////////////////////////////////////////////////////////////////////

  MSSQL_CREATE_ROLEDATA_TABLES =
    'if object_id(''HeroAbil'', ''u'') is not null drop table HeroAbil;' + sLineBreak +
    'if object_id(''HeroAbilNG'', ''u'') is not null drop table HeroAbilNG;' + sLineBreak +
    'if object_id(''HeroAbilNpcAdd'', ''u'') is not null drop table HeroAbilNpcAdd;' + sLineBreak +
    'if object_id(''HeroAbilWine'', ''u'') is not null drop table HeroAbilWine;' + sLineBreak +
    'if object_id(''HeroGodBlessState'', ''u'') is not null drop table HeroGodBlessState;' + sLineBreak +
    'if object_id(''HeroMagic'', ''u'') is not null drop table HeroMagic;' + sLineBreak +
    'if object_id(''HeroQuestFlag'', ''u'') is not null drop table HeroQuestFlag;' + sLineBreak +
    'if object_id(''HeroStatusTime'', ''u'') is not null drop table HeroStatusTime;' + sLineBreak +
    'if object_id(''HeroItems'', ''u'') is not null drop table HeroItems;' + sLineBreak +
    'if object_id(''HeroItemElementAdd'', ''u'') is not null drop table HeroItemElementAdd;' + sLineBreak +
    'if object_id(''HeroItemFlute'', ''u'') is not null drop table HeroItemFlute;' + sLineBreak +
    'if object_id(''HeroItemProgress'', ''u'') is not null drop table HeroItemProgress;' + sLineBreak +
    'if object_id(''HeroItemProperty'', ''u'') is not null drop table HeroItemProperty;' + sLineBreak +
    'if object_id(''HeroItemValueAdd'', ''u'') is not null drop table HeroItemValueAdd;' + sLineBreak +
    'if object_id(''Hero'', ''u'') is not null drop table Hero;' + sLineBreak +

    'if object_id(''HumanAbil'', ''u'') is not null drop table HumanAbil;' + sLineBreak +
    'if object_id(''HumanAbilNG'', ''u'') is not null drop table HumanAbilNG;' + sLineBreak +
    'if object_id(''HumanAbilNpcAdd'', ''u'') is not null drop table HumanAbilNpcAdd;' + sLineBreak +
    'if object_id(''HumanAbilWine'', ''u'') is not null drop table HumanAbilWine;' + sLineBreak +
    'if object_id(''HumanGamePetData'', ''u'') is not null drop table HumanGamePetData;' + sLineBreak +
    'if object_id(''HumanGodBlessState'', ''u'') is not null drop table HumanGodBlessState;' + sLineBreak +
    'if object_id(''HumanMagic'', ''u'') is not null drop table HumanMagic;' + sLineBreak +
    'if object_id(''HumanMagicUseTick'', ''u'') is not null drop table HumanMagicUseTick;' + sLineBreak +
    'if object_id(''HumanQuestFlag'', ''u'') is not null drop table HumanQuestFlag;' + sLineBreak +
    'if object_id(''HumanStatusTime'', ''u'') is not null drop table HumanStatusTime;' + sLineBreak +
    'if object_id(''HumanVariableT'', ''u'') is not null drop table HumanVariableT;' + sLineBreak +
    'if object_id(''HumanVariableU'', ''u'') is not null drop table HumanVariableU;' + sLineBreak +
    'if object_id(''HumanVariableJ'', ''u'') is not null drop table HumanVariableJ;' + sLineBreak +
    'if object_id(''HumanItems'', ''u'') is not null drop table HumanItems;' + sLineBreak +
    'if object_id(''HumanItemElementAdd'', ''u'') is not null drop table HumanItemElementAdd;' + sLineBreak +
    'if object_id(''HumanItemFlute'', ''u'') is not null drop table HumanItemFlute;' + sLineBreak +
    'if object_id(''HumanItemProgress'', ''u'') is not null drop table HumanItemProgress;' + sLineBreak +
    'if object_id(''HumanItemProperty'', ''u'') is not null drop table HumanItemProperty;' + sLineBreak +
    'if object_id(''HumanItemValueAdd'', ''u'') is not null drop table HumanItemValueAdd;' + sLineBreak +
    'if object_id(''Human'', ''u'') is not null drop table Human;' + sLineBreak +


    'CREATE TABLE Human (' + sLineBreak +
    'HumanID  int IDENTITY(1,1) NOT NULL,' + sLineBreak +
    'Account  varchar(10) NOT NULL,' + sLineBreak +
    'HumanName  varchar(14) NOT NULL,' + sLineBreak +
    'IsDelete  INT,' + sLineBreak +
    'IsSelect  INT,' + sLineBreak +
    'CreateDate  INT,' + sLineBreak +
    'LoginDate  INT,' + sLineBreak +
    'Sex  INT,' + sLineBreak +
    'Job  INT,' + sLineBreak +
    'Hair  INT,' + sLineBreak +
    'Dir  INT,' + sLineBreak +
    'Level  INT DEFAULT 0,' + sLineBreak +
    'ReLevel  INT DEFAULT 0,' + sLineBreak +
    'Map  varchar(30),' + sLineBreak +
    'X  INT,' + sLineBreak +
    'Y  INT,' + sLineBreak +
    'HomeMap  varchar(30),' + sLineBreak +
    'HomeX  INT,' + sLineBreak +
    'HomeY  INT,' + sLineBreak +
    'AttackMode  INT,' + sLineBreak +
    'StoragePassword  varchar(7),' + sLineBreak +
    'CreditPoint  INT,' + sLineBreak +
    'Gold  INT,' + sLineBreak +
    'GameGold  INT,' + sLineBreak +
    'GamePoint  INT,' + sLineBreak +
    'GameDiamond  INT,' + sLineBreak +
    'GameGird  INT,' + sLineBreak +
    'GameGoldEx  INT,' + sLineBreak +
    'GameGlory  INT,' + sLineBreak +
    'PKPoint  INT,' + sLineBreak +
    'PayMentPoint  INT,' + sLineBreak +

    'MemberType  INT,' + sLineBreak +
    'MemberLevel  INT,' + sLineBreak +

    'IsMaster  INT,' + sLineBreak +
    'MasterName  varchar(14),' + sLineBreak +
    'MasterCount  INT,' + sLineBreak +
    'MarryCount  INT,' + sLineBreak +
    'DearName  varchar(14),' + sLineBreak +
    'IncHP  INT,' + sLineBreak +
    'IncMP  INT,' + sLineBreak +
    'IncHP2  INT,' + sLineBreak +
    'FightZoneDieCount  INT,' + sLineBreak +
    'BodyLuck  REAL,' + sLineBreak +
    'Contribution  INT,' + sLineBreak +
    'HungerStatus  INT,' + sLineBreak +
    'KickCount  INT,' + sLineBreak +
    'IsLockLogin  INT,' + sLineBreak +
    'IsAllowGroup  INT,' + sLineBreak +
    'IsAllowGroupRecall  INT,' + sLineBreak +
    'GroupRecallTime  INT,' + sLineBreak +
    'IsAllowGuildReCall  INT,' + sLineBreak +
    'IsDisableTrading  INT,' + sLineBreak +
    'IsDisableInviteHorseRiding  INT,' + sLineBreak +
    'IsGameGoldTrading  INT,' + sLineBreak +
    'IsNewServer  INT,' + sLineBreak +


    'IsFilterGlobalDropItemMsg  INT,' + sLineBreak +
    'IsFilterGlobalCenterMsg  INT,' + sLineBreak +
    'IsFilterGolbalSendMsg  INT,' + sLineBreak +

    'IsFixedHero  INT,' + sLineBreak +
    'IsStorageHero  INT,' + sLineBreak +
    'IsStorageDeputyHero  INT,' + sLineBreak +
    'HeroName  varchar(14),' + sLineBreak +
    'DeputyHeroName  varchar(14),' + sLineBreak +
    'DeputyHeroJob  INT,' + sLineBreak +
    'Nation  INT,' + sLineBreak +
    'NationCredit  INT,' + sLineBreak +
    'RevivalTime  INT,' + sLineBreak +
    'InfinityStorageExtCount  INT,' + sLineBreak +
    'IsSaveKillMonExpRate  INT,' + sLineBreak +
    'KillMonExpRate  INT,' + sLineBreak +
    'KillMonExpRateTime  INT,' + sLineBreak +
    'IsSavePowerRate  INT,' + sLineBreak +
    'PowerRate  INT,' + sLineBreak +
    'PowerRateTime  INT,' + sLineBreak +
    'IsSaveKillMonBurstRate  INT,' + sLineBreak +
    'KillMonBurstRate  INT,' + sLineBreak +
    'KillMonBurstRateTime  INT,' + sLineBreak +
    'FBCreateTime  INT,' + sLineBreak +
    'JewelryBoxStatus  INT,' + sLineBreak +
    'IsShowFashion  INT,' + sLineBreak +
    'IsShowGodBless  INT,' + sLineBreak +
    'ActiveFengHao  INT,' + sLineBreak +

    'IsOpenStorage1 INT,' + sLineBreak +
    'IsOpenStorage2 INT,' + sLineBreak +
    'IsOpenStorage3 INT,' + sLineBreak +

    'HighLevelKillMonFixExpTimeLeft  INT,' + sLineBreak +

    'MobileNumber  varchar(20) ,' + sLineBreak +
    'IsMobileBind  INT,' + sLineBreak +
    'MobileVerifyCode  varchar(8),' + sLineBreak +
    'MobileSendTick  INT,' + sLineBreak +
    'MobileResendCount  INT,' + sLineBreak +

    'ClearDayVarTime  INT DEFAULT 0,' + sLineBreak +

    'PRIMARY KEY (HumanID)' + sLineBreak +
    ');' + sLineBreak +

    ////'CREATE UNIQUE INDEX uk_HumanName on Human(HumanName);' + sLineBreak +
    ////'CREATE INDEX idx_Account on Human(Account);' + sLineBreak +
    ////'CREATE INDEX idx_HumanName on Human(HumanName);' + sLineBreak +


    'CREATE TABLE HumanAbil (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    'AC1  INT DEFAULT 0,' + sLineBreak +
    'AC2  INT DEFAULT 0,' + sLineBreak +
    'MAC1  INT DEFAULT 0,' + sLineBreak +
    'MAC2  INT DEFAULT 0,' + sLineBreak +
    'DC1  INT DEFAULT 0,' + sLineBreak +
    'DC2  INT DEFAULT 0,' + sLineBreak +
    'MC1  INT DEFAULT 0,' + sLineBreak +
    'MC2  INT DEFAULT 0,' + sLineBreak +
    'SC1  INT DEFAULT 0,' + sLineBreak +
    'SC2  INT,' + sLineBreak +
    'HP  INT DEFAULT 0,' + sLineBreak +
    'MaxHP  INT DEFAULT 0,' + sLineBreak +
    'MP  INT DEFAULT 0,' + sLineBreak +
    'MaxMP  INT DEFAULT 0,' + sLineBreak +
    'Exp  INT DEFAULT 0,' + sLineBreak +
    'MaxExp  INT DEFAULT 0,' + sLineBreak +
    'Weight  INT DEFAULT 0,' + sLineBreak +
    'MaxWeight  INT DEFAULT 0,' + sLineBreak +
    'WearWeight  INT DEFAULT 0,' + sLineBreak +
    'MaxWearWeight  INT DEFAULT 0,' + sLineBreak +
    'HandWeight  INT DEFAULT 0,' + sLineBreak +
    'MaxHandWeight  INT DEFAULT 0,' + sLineBreak +
    'AdjustAbilPoint  INT DEFAULT 0,' + sLineBreak +
    'AdjustAbilDC  INT DEFAULT 0,' + sLineBreak +
    'AdjustAbilMC  INT DEFAULT 0,' + sLineBreak +
    'AdjustAbilSC  INT DEFAULT 0,' + sLineBreak +
    'AdjustAbilAC  INT DEFAULT 0,' + sLineBreak +
    'AdjustAbilMAC  INT DEFAULT 0,' + sLineBreak +
    'AdjustAbilHP  INT DEFAULT 0,' + sLineBreak +
    'AdjustAbilMP  INT DEFAULT 0,' + sLineBreak +
    'AdjustAbilHit  INT DEFAULT 0,' + sLineBreak +
    'AdjustAbilSpeed  INT DEFAULT 0,' + sLineBreak +
    'AdjustAbilMaxRate  INT DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY (HumanID)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanAbilNG (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +   // FOREIGN KEY REFERENCES Human (HumanID)
    'IsTrainingNG  INT,' + sLineBreak +
    'IsTrainingXF  INT,' + sLineBreak +
    'AbilNGLevel  INT,' + sLineBreak +
    'AbilNGValue  INT,' + sLineBreak +
    'AbilNGMaxValue  INT,' + sLineBreak +
    'AbilNGExp  INT,' + sLineBreak +
    'AbilNGMaxExp  INT,' + sLineBreak +
    'ContinuousMagicOrder1  INT,' + sLineBreak +
    'ContinuousMagicOrder2  INT,' + sLineBreak +
    'ContinuousMagicOrder3  INT,' + sLineBreak +
    'IsOpenLastContinuous  INT,' + sLineBreak +
    'LastContinuousMagicOrder  INT,' + sLineBreak +
    'Meridians1Level  INT,' + sLineBreak +
    'Meridians1BlastHitRate1  INT,' + sLineBreak +
    'Meridians1Acupoints1  INT,' + sLineBreak +
    'Meridians1Acupoints2  INT,' + sLineBreak +
    'Meridians1Acupoints3  INT,' + sLineBreak +
    'Meridians1Acupoints4  INT,' + sLineBreak +
    'Meridians1Acupoints5  INT,' + sLineBreak +
    'Meridians2Level  INT,' + sLineBreak +
    'Meridians2BlastHitRate  INT,' + sLineBreak +
    'Meridians2Acupoints1  INT,' + sLineBreak +
    'Meridians2Acupoints2  INT,' + sLineBreak +
    'Meridians2Acupoints3  INT,' + sLineBreak +
    'Meridians2Acupoints4  INT,' + sLineBreak +
    'Meridians2Acupoints5  INT,' + sLineBreak +
    'Meridians3Level  INT,' + sLineBreak +
    'Meridians3BlastHitRate  INT,' + sLineBreak +
    'Meridians3Acupoints1  INT,' + sLineBreak +
    'Meridians3Acupoints2  INT,' + sLineBreak +
    'Meridians3Acupoints3  INT,' + sLineBreak +
    'Meridians3Acupoints4  INT,' + sLineBreak +
    'Meridians3Acupoints5  INT,' + sLineBreak +
    'Meridians4Level  INT,' + sLineBreak +
    'Meridians4BlastHitRate  INT,' + sLineBreak +
    'Meridians4Acupoints1  INT,' + sLineBreak +
    'Meridians4Acupoints2  INT,' + sLineBreak +
    'Meridians4Acupoints3  INT,' + sLineBreak +
    'Meridians4Acupoints4  INT,' + sLineBreak +
    'Meridians4Acupoints5  INT,' + sLineBreak +
    'Meridians5Level  INT,' + sLineBreak +
    'Meridians5BlastHitRate  INT,' + sLineBreak +
    'Meridians5Acupoints1  INT,' + sLineBreak +
    'Meridians5Acupoints2  INT,' + sLineBreak +
    'Meridians5Acupoints3  INT,' + sLineBreak +
    'Meridians5Acupoints4  INT,' + sLineBreak +
    'Meridians5Acupoints5  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanAbilNpcAdd (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    '[Index]  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, [Index])' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanAbilWine (' + sLineBreak +
    'HumanID  INT NOT NULL ,' + sLineBreak +    // FOREIGN KEY REFERENCES Human (HumanID)
    'IsDrinkedWine  INT,' + sLineBreak +
    'IsDrinkWineDrunk  INT,' + sLineBreak +
    'DrinkWineQuality  INT,' + sLineBreak +
    'DrinkWineAlcohol  INT,' + sLineBreak +
    'AbilAlcohol  INT,' + sLineBreak +
    'AbilMaxAlcohol  INT,' + sLineBreak +
    'AbilDrinkValue  INT,' + sLineBreak +
    'AbilMedicineLevel  INT,' + sLineBreak +
    'AbilMedicineValue  INT,' + sLineBreak +
    'AbilMaxMedicineValue  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanGamePetData (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    '[Index]  INT NOT NULL,' + sLineBreak +
    '[Name]  VARCHAR(30),' + sLineBreak +
    '[Level]  INT,' + sLineBreak +
    'HP  INT,' + sLineBreak +
    'MP  INT,' + sLineBreak +
    'Exp  INT,' + sLineBreak +
    'Magic1  INT,' + sLineBreak +
    'Magic2  INT,' + sLineBreak +
    'Magic3  INT,' + sLineBreak +
    'Magic4  INT,' + sLineBreak +
    'Magic5  INT,' + sLineBreak +
    'Magic6  INT,' + sLineBreak +
    'Magic7  INT,' + sLineBreak +
    'Magic8  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, [Index])' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanGodBlessState (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    '[Index]  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, [Index])' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanMagic (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    'MagicType  INT NOT NULL,' + sLineBreak +
    'MagicIndex  INT NOT NULL,' + sLineBreak +
    'MagicID  INT,' + sLineBreak +
    'MagicAttr  INT,' + sLineBreak +
    'MagicLevel  INT,' + sLineBreak +
    'MagicNewLevel  INT,' + sLineBreak +
    'MagicKey  INT,' + sLineBreak +
    'MagicTranPoint  INT,' + sLineBreak +
    'MagicIsUseItemAdd  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, MagicType, MagicIndex)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE HumanMagicUseTick (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    'MagicID  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, MagicID)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanQuestFlag (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    '[Index]  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, [Index])' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanStatusTime (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    '[Index]  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, [Index]) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanVariableT (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    '[Index]  INT NOT NULL,' + sLineBreak +
    'Value  varchar(100),' + sLineBreak +
    'PRIMARY KEY (HumanID, [Index])' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanVariableU (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    '[Index]  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, [Index])' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanVariableJ (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    '[Index]  INT NOT NULL,' + sLineBreak +
    'Value  INT DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY (HumanID, [Index])' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanItems (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    'ItemType  INT NOT NULL,' + sLineBreak +
    'ItemIndex  INT NOT NULL,' + sLineBreak +
    'MakeIndex  INT,' + sLineBreak +
    'DBIndex  INT,' + sLineBreak +
    '[Name]  VARCHAR(30),' + sLineBreak +
    'Dura  INT,' + sLineBreak +
    'DuraMax  INT,' + sLineBreak +
    'HeroM2DressEffect  INT,' + sLineBreak +
    'UpgradeCount  INT,' + sLineBreak +
    'IsStartTime  INT,' + sLineBreak +
    'LimitTime  INT,' + sLineBreak +
    'HeroM2Light  INT,' + sLineBreak +
    'Color  INT,' + sLineBreak +
    'IsBind  INT,' + sLineBreak +
    'BindOption  INT,' + sLineBreak +
    'Effect  INT,' + sLineBreak +
    'NewLooks  INT,' + sLineBreak +
    'NewShape  INT,' + sLineBreak +
    'FluteCount  INT,' + sLineBreak +
    'PropertyText  VARCHAR(64),' + sLineBreak +
    'PropertyTextColor  INT,' + sLineBreak +
    'ItemFrom  INT,' + sLineBreak +
    'ItemFromMap  VARCHAR(30),' + sLineBreak +
    'ItemFromMon  VARCHAR(40),' + sLineBreak +
    'ItemFromMaker  VARCHAR(40),' + sLineBreak +
    'ItemFromDate  REAL,' + sLineBreak +
    'InsuranceCount  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, ItemType, ItemIndex)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE HumanItemElementAdd (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +     // FOREIGN KEY REFERENCES Human (HumanID)
    'ItemType  INT NOT NULL,' + sLineBreak +
    'ItemIndex  INT NOT NULL,' + sLineBreak +
    'ValueIndex  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanItemFlute (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +
    'ItemType  INT NOT NULL,' + sLineBreak +
    'ItemIndex  INT NOT NULL,' + sLineBreak +
    'ValueIndex  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanItemProgress (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +
    'ItemType  INT NOT NULL,' + sLineBreak +
    'ItemIndex  INT NOT NULL,' + sLineBreak +
    'ValueIndex  INT NOT NULL,' + sLineBreak +
    'IsOpen  INT,' + sLineBreak +
    'NameColor  INT,' + sLineBreak +
    '[Count]  INT,' + sLineBreak +
    'ShowType  INT,' + sLineBreak +
    '[Max]  INT,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    '[Level]  INT,' + sLineBreak +
    '[Name]  varchar(31),' + sLineBreak +
    'PRIMARY KEY (HumanID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanItemProperty (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +
    'ItemType  INT NOT NULL,' + sLineBreak +
    'ItemIndex  INT NOT NULL,' + sLineBreak +
    'ValueIndex  INT NOT NULL,' + sLineBreak +
    'Color  INT,' + sLineBreak +
    'BindType  INT,' + sLineBreak +
    'ShowFlag  INT,' + sLineBreak +
    'IsPercent  INT,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HumanItemValueAdd (' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +
    'ItemType  INT NOT NULL,' + sLineBreak +
    'ItemIndex  INT NOT NULL,' + sLineBreak +
    'ValueIndex  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HumanID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE Hero (' + sLineBreak +
    'HeroID  int IDENTITY(1,1) NOT NULL FOREIGN KEY REFERENCES Human (HumanID),' + sLineBreak +
    'HumanID  INT NOT NULL,' + sLineBreak +
    'HeroName  VARCHAR(14) NOT NULL,' + sLineBreak +
    'IsDelete  INT,' + sLineBreak +
    'CreateDate  INT,' + sLineBreak +
    'Sex  INT,' + sLineBreak +
    'Job  INT,' + sLineBreak +
    'Hair  INT,' + sLineBreak +
    'Status  INT,' + sLineBreak +
    'Dir  INT,' + sLineBreak +
    'Level  INT,' + sLineBreak +
    'ReLevel  INT,' + sLineBreak +
    'LoyalPoint  REAL, ' + sLineBreak +
    'Map  VARCHAR(30),' + sLineBreak +
    'X  INT,' + sLineBreak +
    'Y  INT,' + sLineBreak +
    'HomeMap  VARCHAR(30),' + sLineBreak +
    'HomeX  INT,' + sLineBreak +
    'HomeY  INT,' + sLineBreak +
    'AttackMode  INT,' + sLineBreak +
    'CreditPoint  INT,' + sLineBreak +
    'PKPoint  INT,' + sLineBreak +
    'IncHP  INT,' + sLineBreak +
    'IncMP  INT,' + sLineBreak +
    'IncHP2  INT,' + sLineBreak +
    'FightZoneDieCount  INT,' + sLineBreak +
    'BodyLuck  REAL,' + sLineBreak +
    'HungerStatus  INT,' + sLineBreak +
    'RevivalTime  INT,' + sLineBreak +
    'IsSaveKillMonExpRate  INT,' + sLineBreak +
    'KillMonExpRate  INT,' + sLineBreak +
    'KillMonExpRateTime  INT,' + sLineBreak +
    'IsSavePowerRate  INT,' + sLineBreak +
    'PowerRate  INT,' + sLineBreak +
    'PowerRateTime  INT,' + sLineBreak +
    'IsSaveKillMonBurstRate  INT,' + sLineBreak +
    'KillMonBurstRate  INT,' + sLineBreak +
    'KillMonBurstRateTime  INT,' + sLineBreak +
    'JewelryBoxStatus  INT,' + sLineBreak +
    'IsShowFashion  INT,' + sLineBreak +
    'IsShowGodBless  INT,' + sLineBreak +
    'ActiveFengHao  INT,' + sLineBreak +

    'PRIMARY KEY (HeroID)' + sLineBreak +
    ');' + sLineBreak +


    ////'CREATE UNIQUE INDEX idx_HeroName on Hero(HeroName);' + sLineBreak +


    'CREATE TABLE HeroAbil (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +        // FOREIGN KEY REFERENCES Hero (HeroID)
    'AC1  INT DEFAULT 0,' + sLineBreak +
    'AC2  INT DEFAULT 0,' + sLineBreak +
    'MAC1  INT DEFAULT 0,' + sLineBreak +
    'MAC2  INT DEFAULT 0,' + sLineBreak +
    'DC1  INT DEFAULT 0,' + sLineBreak +
    'DC2  INT DEFAULT 0,' + sLineBreak +
    'MC1  INT DEFAULT 0,' + sLineBreak +
    'MC2  INT DEFAULT 0,' + sLineBreak +
    'SC1  INT DEFAULT 0,' + sLineBreak +
    'SC2  INT,' + sLineBreak +
    'HP  INT DEFAULT 0,' + sLineBreak +
    'MaxHP  INT DEFAULT 0,' + sLineBreak +
    'MP  INT DEFAULT 0,' + sLineBreak +
    'MaxMP  INT DEFAULT 0,' + sLineBreak +
    'Exp  INT DEFAULT 0,' + sLineBreak +
    'MaxExp  INT DEFAULT 0,' + sLineBreak +
    'Weight  INT DEFAULT 0,' + sLineBreak +
    'MaxWeight  INT DEFAULT 0,' + sLineBreak +
    'WearWeight  INT DEFAULT 0,' + sLineBreak +
    'MaxWearWeight  INT DEFAULT 0,' + sLineBreak +
    'HandWeight  INT DEFAULT 0,' + sLineBreak +
    'MaxHandWeight  INT DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY (HeroID)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroAbilNG (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +        // FOREIGN KEY REFERENCES Hero (HeroID)
    'IsTrainingNG  INT,' + sLineBreak +
    'IsTrainingXF  INT,' + sLineBreak +
    'AbilNGLevel  INT,' + sLineBreak +
    'AbilNGValue  INT,' + sLineBreak +
    'AbilNGMaxValue  INT,' + sLineBreak +
    'AbilNGExp  INT,' + sLineBreak +
    'AbilNGMaxExp  INT,' + sLineBreak +
    'ContinuousMagicOrder1  INT,' + sLineBreak +
    'ContinuousMagicOrder2  INT,' + sLineBreak +
    'ContinuousMagicOrder3  INT,' + sLineBreak +
    'IsOpenLastContinuous  INT,' + sLineBreak +
    'LastContinuousMagicOrder  INT,' + sLineBreak +
    'Meridians1Level  INT,' + sLineBreak +
    'Meridians1BlastHitRate1  INT,' + sLineBreak +
    'Meridians1Acupoints1  INT,' + sLineBreak +
    'Meridians1Acupoints2  INT,' + sLineBreak +
    'Meridians1Acupoints3  INT,' + sLineBreak +
    'Meridians1Acupoints4  INT,' + sLineBreak +
    'Meridians1Acupoints5  INT,' + sLineBreak +
    'Meridians2Level  INT,' + sLineBreak +
    'Meridians2BlastHitRate  INT,' + sLineBreak +
    'Meridians2Acupoints1  INT,' + sLineBreak +
    'Meridians2Acupoints2  INT,' + sLineBreak +
    'Meridians2Acupoints3  INT,' + sLineBreak +
    'Meridians2Acupoints4  INT,' + sLineBreak +
    'Meridians2Acupoints5  INT,' + sLineBreak +
    'Meridians3Level  INT,' + sLineBreak +
    'Meridians3BlastHitRate  INT,' + sLineBreak +
    'Meridians3Acupoints1  INT,' + sLineBreak +
    'Meridians3Acupoints2  INT,' + sLineBreak +
    'Meridians3Acupoints3  INT,' + sLineBreak +
    'Meridians3Acupoints4  INT,' + sLineBreak +
    'Meridians3Acupoints5  INT,' + sLineBreak +
    'Meridians4Level  INT,' + sLineBreak +
    'Meridians4BlastHitRate  INT,' + sLineBreak +
    'Meridians4Acupoints1  INT,' + sLineBreak +
    'Meridians4Acupoints2  INT,' + sLineBreak +
    'Meridians4Acupoints3  INT,' + sLineBreak +
    'Meridians4Acupoints4  INT,' + sLineBreak +
    'Meridians4Acupoints5  INT,' + sLineBreak +
    'Meridians5Level  INT,' + sLineBreak +
    'Meridians5BlastHitRate  INT,' + sLineBreak +
    'Meridians5Acupoints1  INT,' + sLineBreak +
    'Meridians5Acupoints2  INT,' + sLineBreak +
    'Meridians5Acupoints3  INT,' + sLineBreak +
    'Meridians5Acupoints4  INT,' + sLineBreak +
    'Meridians5Acupoints5  INT,' + sLineBreak +
    'PRIMARY KEY (HeroID)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroAbilNpcAdd (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +        // FOREIGN KEY REFERENCES Hero (HeroID)
    '[Index]  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HeroID, [Index])' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroAbilWine (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +        // FOREIGN KEY REFERENCES Hero (HeroID)
    'IsDrinkWineDrunk  INT,' + sLineBreak +
    'DrinkWineQuality  INT,' + sLineBreak +
    'DrinkWineAlcohol  INT,' + sLineBreak +
    'AbilAlcohol  INT,' + sLineBreak +
    'AbilMaxAlcohol  INT,' + sLineBreak +
    'AbilDrinkValue  INT,' + sLineBreak +
    'AbilMedicineLevel  INT,' + sLineBreak +
    'AbilMedicineValue  INT,' + sLineBreak +
    'AbilMaxMedicineValue  INT,' + sLineBreak +
    'PRIMARY KEY (HeroID)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroGodBlessState (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +        // FOREIGN KEY REFERENCES Hero (HeroID)
    '[Index]  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HeroID, [Index])' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroMagic (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +        // FOREIGN KEY REFERENCES Hero (HeroID)
    'MagicType  INT NOT NULL,' + sLineBreak +
    'MagicIndex  INT NOT NULL,' + sLineBreak +
    'MagicID  INT,' + sLineBreak +
    'MagicAttr  INT,' + sLineBreak +
    'MagicLevel  INT,' + sLineBreak +
    'MagicNewLevel  INT,' + sLineBreak +
    'MagicKey  INT,' + sLineBreak +
    'MagicTranPoint  INT,' + sLineBreak +
    'MagicIsUseItemAdd  INT,' + sLineBreak +
    'PRIMARY KEY (HeroID, MagicType, MagicIndex)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroQuestFlag (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +        // FOREIGN KEY REFERENCES Hero (HeroID)
    '[Index]  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HeroID, [Index])' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroStatusTime (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +
    '[Index]  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HeroID, [Index])' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroItems (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +        // FOREIGN KEY REFERENCES Hero (HeroID)
    'ItemType  INT NOT NULL,' + sLineBreak +
    'ItemIndex  INT NOT NULL,' + sLineBreak +
    'MakeIndex  INT,' + sLineBreak +
    'DBIndex  INT,' + sLineBreak +
    '[Name]  VARCHAR(30),' + sLineBreak +
    'Dura  INT,' + sLineBreak +
    'DuraMax  INT,' + sLineBreak +
    'HeroM2DressEffect  INT,' + sLineBreak +
    'UpgradeCount  INT,' + sLineBreak +
    'IsStartTime  INT,' + sLineBreak +
    'LimitTime  INT,' + sLineBreak +
    'HeroM2Light  INT,' + sLineBreak +
    'Color  INT,' + sLineBreak +
    'IsBind  INT,' + sLineBreak +
    'BindOption  INT,' + sLineBreak +
    'Effect  INT,' + sLineBreak +
    'NewLooks  INT,' + sLineBreak +
    'NewShape  INT,' + sLineBreak +
    'FluteCount  INT,' + sLineBreak +
    'PropertyText  VARCHAR(64),' + sLineBreak +
    'PropertyTextColor  INT,' + sLineBreak +
    'ItemFrom  INT,' + sLineBreak +
    'ItemFromMap  VARCHAR(30),' + sLineBreak +
    'ItemFromMon  VARCHAR(40),' + sLineBreak +
    'ItemFromMaker  VARCHAR(40),' + sLineBreak +
    'ItemFromDate  REAL,' + sLineBreak +
    'InsuranceCount  INT,' + sLineBreak +
    'PRIMARY KEY (HeroID, ItemType, ItemIndex)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroItemElementAdd (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +
    'ItemType  INT NOT NULL,' + sLineBreak +
    'ItemIndex  INT NOT NULL,' + sLineBreak +
    'ValueIndex  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HeroID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroItemFlute (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +
    'ItemType  INT NOT NULL,' + sLineBreak +
    'ItemIndex  INT NOT NULL,' + sLineBreak +
    'ValueIndex  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HeroID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroItemProgress (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +
    'ItemType  INT NOT NULL,' + sLineBreak +
    'ItemIndex  INT NOT NULL,' + sLineBreak +
    'ValueIndex  INT NOT NULL,' + sLineBreak +
    'IsOpen  INT,' + sLineBreak +
    'NameColor  INT,' + sLineBreak +
    'Count  INT,' + sLineBreak +
    'ShowType  INT,' + sLineBreak +
    '[Max]  INT,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    '[Level]  INT,' + sLineBreak +
    '[Name]  varchar(31),' + sLineBreak +
    'PRIMARY KEY (HeroID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroItemProperty (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +
    'ItemType  INT NOT NULL,' + sLineBreak +
    'ItemIndex  INT NOT NULL,' + sLineBreak +
    'ValueIndex  INT NOT NULL,' + sLineBreak +
    'Color  INT,' + sLineBreak +
    'BindType  INT,' + sLineBreak +
    'ShowFlag  INT,' + sLineBreak +
    'IsPercent  INT,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HeroID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE HeroItemValueAdd (' + sLineBreak +
    'HeroID  INT NOT NULL,' + sLineBreak +
    'ItemType  INT NOT NULL,' + sLineBreak +
    'ItemIndex  INT NOT NULL,' + sLineBreak +
    'ValueIndex  INT NOT NULL,' + sLineBreak +
    'Value  INT,' + sLineBreak +
    'PRIMARY KEY (HeroID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +

    MSSQL_CEATE_DB_CONSTANT + sLineBreak +

    'IF EXISTS(SELECT 1 FROM db_constant WHERE ConstName = ''role_version'')' + sLineBreak +
    'BEGIN' + sLineBreak +
    'UPDATE db_constant SET ConstValue = ' + MSSQL_DBVERSION + ' WHERE ConstName = ''role_version''' + sLineBreak +
    'END' + sLineBreak +
    'ELSE' + sLineBreak +
    'BEGIN' + sLineBreak +
    'INSERT INTO db_constant(ConstName, ConstValue) VALUES(''role_version'', ' + MSSQL_DBVERSION + ')' + sLineBreak +
    'END';


const
  MSSQL_M2DBVERSION = '20180615';

const
  MSSQL_CREATE_M2DATA_TABLES =
    'if object_id(''AuctionAttention'', ''u'') is not null drop table AuctionAttention;' + sLineBreak +
    'CREATE TABLE AuctionAttention (' + sLineBreak +
    'HumanName  VARCHAR(14) NOT NULL,' + sLineBreak +
    'AuctionID  INTEGER NOT NULL,' + sLineBreak +
    '[time] datetime DEFAULT (getdate()),' + sLineBreak +
    'PRIMARY KEY (HumanName, AuctionID)' + sLineBreak +
    ');' + sLineBreak +

    'if object_id(''AuctionData'', ''u'') is not null drop table AuctionData;' + sLineBreak +
    'CREATE TABLE AuctionData (' + sLineBreak +
    'AuctionID  INTEGER NOT NULL,' + sLineBreak +
    'HumanName   VARCHAR(14) NOT NULL,' + sLineBreak +
    'ItemGroup  INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    'ItemColor  INTEGER DEFAULT 0,' + sLineBreak +
    'AddDateTime  datetime DEFAULT (getdate()),' + sLineBreak +
    'AuctionTime  INTEGER NOT NULL,' + sLineBreak +
    'StartingPrice  INTEGER DEFAULT 0,' + sLineBreak +
    'SellingPrice  INTEGER DEFAULT 0,' + sLineBreak +
    'CurrencyType  INTEGER,' + sLineBreak +
    'LastBidder   VARCHAR(14),' + sLineBreak +
    'LastBidPrice  INTEGER,' + sLineBreak +
    'LastBidTime  INTEGER,' + sLineBreak +
    'TradingStatus  INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    'IsItemGive  INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    'IsTopmost  INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    'ItemDBName  VARCHAR(30),' + sLineBreak +
    'ItemName  VARCHAR(30),' + sLineBreak +
    'PRIMARY KEY (AuctionID)' + sLineBreak +
    ');' + sLineBreak +

    ////'CREATE INDEX idx_AuctionDataItemGroup ON AuctionData (ItemGroup); ' + sLineBreak +
    ////'CREATE INDEX idx_AuctionDataItemName ON AuctionData (ItemName); ' + sLineBreak +
    ////'CREATE INDEX idx_AuctionDataItemDBName ON AuctionData (ItemDBName); ' + sLineBreak +


    'if object_id(''ItemElementAdd'', ''u'') is not null drop table ItemElementAdd;' + sLineBreak +
    'CREATE TABLE ItemElementAdd (' + sLineBreak +
    'ParentID  INTEGER NOT NULL,' + sLineBreak +
    'ItemType  INTEGER NOT NULL,' + sLineBreak +
    'ItemIndex  INTEGER NOT NULL,' + sLineBreak +
    'ValueIndex  INTEGER NOT NULL,' + sLineBreak +
    '[Value]  INTEGER,' + sLineBreak +
    'PRIMARY KEY (ParentID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'if object_id(''ItemFlute'', ''u'') is not null drop table ItemFlute;' + sLineBreak +
    'CREATE TABLE ItemFlute (' + sLineBreak +
    'ParentID  INTEGER NOT NULL,' + sLineBreak +
    'ItemType  INTEGER NOT NULL,' + sLineBreak +
    'ItemIndex  INTEGER NOT NULL,' + sLineBreak +
    'ValueIndex  INTEGER NOT NULL,' + sLineBreak +
    '[Value]  INTEGER,' + sLineBreak +
    'PRIMARY KEY (ParentID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'if object_id(''ItemProgress'', ''u'') is not null drop table ItemProgress;' + sLineBreak +
    'CREATE TABLE ItemProgress (' + sLineBreak +
    'ParentID  INTEGER NOT NULL,' + sLineBreak +
    'ItemType  INTEGER NOT NULL,' + sLineBreak +
    'ItemIndex  INTEGER NOT NULL,' + sLineBreak +
    'ValueIndex  INTEGER NOT NULL,' + sLineBreak +
    'IsOpen  INTEGER,' + sLineBreak +
    'NameColor  INTEGER,' + sLineBreak +
    '[Count]  INTEGER,' + sLineBreak +
    'ShowType  INTEGER,' + sLineBreak +
    '[Max]  INTEGER,' + sLineBreak +
    '[Value]  INTEGER,' + sLineBreak +
    '[Level]  INTEGER,' + sLineBreak +
    '[Name]  varchar(31),' + sLineBreak +
    'PRIMARY KEY (ParentID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'if object_id(''ItemProperty'', ''u'') is not null drop table ItemProperty;' + sLineBreak +
    'CREATE TABLE ItemProperty (' + sLineBreak +
    'ParentID  INTEGER NOT NULL,' + sLineBreak +
    'ItemType  INTEGER NOT NULL,' + sLineBreak +
    'ItemIndex  INTEGER NOT NULL,' + sLineBreak +
    'ValueIndex  INTEGER NOT NULL,' + sLineBreak +
    'Color  INTEGER,' + sLineBreak +
    'BindType  INTEGER,' + sLineBreak +
    'ShowFlag  INTEGER,' + sLineBreak +
    'IsPercent  INTEGER,' + sLineBreak +
    '[Value]  INTEGER,' + sLineBreak +
    'PRIMARY KEY (ParentID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'if object_id(''Items'', ''u'') is not null drop table Items;' + sLineBreak +
    'CREATE TABLE Items (' + sLineBreak +
    'ParentID  INTEGER NOT NULL,' + sLineBreak +
    'ItemType  INTEGER NOT NULL,' + sLineBreak +
    'ItemIndex  INTEGER NOT NULL,' + sLineBreak +
    'MakeIndex  INTEGER,' + sLineBreak +
    'DBIndex  INTEGER,' + sLineBreak +
    '[Name]  VARCHAR(30),' + sLineBreak +
    'Dura  INTEGER,' + sLineBreak +
    'DuraMax  INTEGER,' + sLineBreak +
    'HeroM2DressEffect  INTEGER,' + sLineBreak +
    'UpgradeCount  INTEGER,' + sLineBreak +
    'IsStartTime  INTEGER,' + sLineBreak +
    'LimitTime  INTEGER,' + sLineBreak +
    'HeroM2Light  INTEGER,' + sLineBreak +
    'Color  INTEGER,' + sLineBreak +
    'IsBind  INTEGER,' + sLineBreak +
    'BindOption  INTEGER,' + sLineBreak +
    'Effect  INTEGER,' + sLineBreak +
    'NewLooks  INTEGER,' + sLineBreak +
    'NewShape  INTEGER,' + sLineBreak +
    'FluteCount  INTEGER,' + sLineBreak +
    'PropertyText  VARCHAR(64),' + sLineBreak +
    'PropertyTextColor  INTEGER,' + sLineBreak +
    'ItemFrom  INTEGER,' + sLineBreak +
    'ItemFromMap  VARCHAR(30),' + sLineBreak +
    'ItemFromMon  VARCHAR(40),' + sLineBreak +
    'ItemFromMaker  VARCHAR(40),' + sLineBreak +
    'ItemFromDate  REAL,' + sLineBreak +
    'InsuranceCount  INTEGER,' + sLineBreak +
    'PRIMARY KEY (ParentID, ItemType, ItemIndex)' + sLineBreak +
    ');' + sLineBreak +


    'if object_id(''ItemValueAdd'', ''u'') is not null drop table ItemValueAdd;' + sLineBreak +
    'CREATE TABLE ItemValueAdd (' + sLineBreak +
    'ParentID  INTEGER NOT NULL,' + sLineBreak +
    'ItemType  INTEGER NOT NULL,' + sLineBreak +
    'ItemIndex  INTEGER NOT NULL,' + sLineBreak +
    'ValueIndex  INTEGER NOT NULL,' + sLineBreak +
    '[Value]  INTEGER,' + sLineBreak +
    'PRIMARY KEY (ParentID, ItemType, ItemIndex, ValueIndex)' + sLineBreak +
    ');' + sLineBreak +


    'if object_id(''StorageEx'', ''u'') is not null drop table StorageEx;' + sLineBreak +
    'CREATE TABLE StorageEx (' + sLineBreak +
    'StorageID  int IDENTITY(1,1) NOT NULL,' + sLineBreak +
    'HumanName  VARCHAR(14) NOT NULL,' + sLineBreak +
    'PRIMARY KEY (StorageID)' + sLineBreak +
    ');' + sLineBreak +

    ////'CREATE UNIQUE INDEX uk_HumanName ON StorageEx (HumanName); ' + sLineBreak +


    'if object_id(''UserShop'', ''u'') is not null drop table UserShop;' + sLineBreak +
    'CREATE TABLE UserShop (' + sLineBreak +
    'ShopID  int IDENTITY(1,1) NOT NULL,' + sLineBreak +
    'HumanName  VARCHAR(14),' + sLineBreak +
    'ShopName  VARCHAR(14),' + sLineBreak +
    'IsBusiness  INTEGER,' + sLineBreak +
    'CreateDate datetime DEFAULT (getdate()),' + sLineBreak +
    'CareValue  INTEGER DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY (ShopID)' + sLineBreak +
    ');' + sLineBreak +

    ////'CREATE UNIQUE INDEX uk_UserShop_HumanName ON UserShop (HumanName); ' + sLineBreak +
    ////'CREATE INDEX idx_ShopName ON UserShop (ShopName); ' + sLineBreak +
    ////'CREATE INDEX idx_IsBusiness ON UserShop (IsBusiness); ' + sLineBreak +


    'if object_id(''UserShopItem'', ''u'') is not null drop table UserShopItem;' + sLineBreak +
    'CREATE TABLE UserShopItem (' + sLineBreak +
    'ShopID  INTEGER NOT NULL,' + sLineBreak +
    'ItemID  INTEGER NOT NULL,' + sLineBreak +
    'ItemType  INTEGER,' + sLineBreak +
    'CreateDate  datetime DEFAULT (getdate()),' + sLineBreak +
    'IsAllowSell  INTEGER,' + sLineBreak +
    'MoneyType  INTEGER,' + sLineBreak +
    'ItemPrice  INTEGER,' + sLineBreak +
    'IsGetMoney  INTEGER,' + sLineBreak +
    'BuyerName  VARCHAR(14),' + sLineBreak +
    'ItemDBName  VARCHAR(30),' + sLineBreak +
    'ItemName  VARCHAR(30) ,' + sLineBreak +
    'PRIMARY KEY (ShopID, ItemID)' + sLineBreak +
    ');' + sLineBreak +


    ////'CREATE INDEX idx_ItemPrices ON UserShopItem (ItemPrice); ' + sLineBreak +
    ////'CREATE INDEX idx_ItemName ON UserShopItem (ItemName); ' + sLineBreak +
    ////'CREATE INDEX idx_ItemDBName ON UserShopItem (ItemDBName); ' + sLineBreak +


    MSSQL_CEATE_DB_CONSTANT + sLineBreak +


    'IF EXISTS(SELECT 1 FROM db_constant WHERE ConstName = ''m2data_version'')' + sLineBreak +
    'BEGIN' + sLineBreak +
    'UPDATE db_constant SET ConstValue = ' + MSSQL_M2DBVERSION + ' WHERE ConstName = ''m2data_version''' + sLineBreak +
    'END' + sLineBreak +
    'ELSE' + sLineBreak +
    'BEGIN' + sLineBreak +
    'INSERT INTO db_constant(ConstName, ConstValue) VALUES(''m2data_version'', ' + MSSQL_M2DBVERSION + ')' + sLineBreak +
    'END';


implementation

end.

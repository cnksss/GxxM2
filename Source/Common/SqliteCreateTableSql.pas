unit SqliteCreateTableSql;

interface

uses
  Windows, Classes, SysUtils;

const
  SQLITE_DBVERSION = '20220219';//'20211218';//'20200916'; // 20190928';

const
  SQLITE_SQL_CEATE_DB_CONSTANT =
    'CREATE TABLE "db_constant" (' + sLineBreak +
    '"ConstName"  TEXT(30) NOT NULL COLLATE NOCASE ,' + sLineBreak +
    '"ConstValue"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("ConstName")' + sLineBreak +
    ');' + sLineBreak +
    'INSERT INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION + ');';

  SQLITE_SQL_CREATE_ACCOUNT_TABLES =
    'CREATE TABLE "Account" (' + sLineBreak +
    '"Account"  TEXT(10) NOT NULL COLLATE NOCASE,' + sLineBreak +
    '"Password"  TEXT(10) NOT NULL,' + sLineBreak +
    '"UserName"  TEXT(20) NOT NULL,' + sLineBreak +
    '"Disable"  INTEGER DEFAULT 0,' + sLineBreak +
    '"IDCard"  TEXT(18),' + sLineBreak +
    '"BirthDay"  TEXT(10),' + sLineBreak +
    '"Questions1"  TEXT(20),' + sLineBreak +
    '"Answers1"  TEXT(12),' + sLineBreak +
    '"Questions2"  TEXT(20),' + sLineBreak +
    '"Answers2"  TEXT(12),' + sLineBreak +
    '"Phone"  TEXT(14),' + sLineBreak +
    '"MobilePhone"  TEXT(13),' + sLineBreak +
    '"Mail"  TEXT(40),' + sLineBreak +
    '"L2Password"  TEXT(20),' + sLineBreak +
    '"CreateDate"  INTEGER,' + sLineBreak +
    '"LoginDate"  INTEGER,' + sLineBreak +
    '"LoginMac"  TEXT(32),' + sLineBreak +
    '"LoginIP"  INTEGER,' + sLineBreak +
    '"LastActionTick"  INTEGER,' + sLineBreak +
    '"ErrorCount"  INTEGER,' + sLineBreak +
    '"Memo"  TEXT(20),' + sLineBreak +        
    '"UID"  TEXT(70),' + sLineBreak +
    '"CID"  TEXT(30),' + sLineBreak +
    'PRIMARY KEY ("Account" ASC),' + sLineBreak +
    'CONSTRAINT "uk_Account" UNIQUE ("Account" ASC)' + sLineBreak +
    ');' +

    // '------------------------------------------------------' + sLineBreak +
    SQLITE_SQL_CEATE_DB_CONSTANT;



  //////////////////////////////////////////////////////////////////////////////////

const
  SQLITE_CREATE_ROLEDATA_TABLES_HUMAN =
    SQLITE_SQL_CEATE_DB_CONSTANT + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "Human" (' + sLineBreak +
    '"HumanID"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,' + sLineBreak +
    '"Account"  TEXT(10) NOT NULL COLLATE NOCASE,' + sLineBreak +
    '"HumanName"  TEXT(14) NOT NULL COLLATE NOCASE,' + sLineBreak +
    '"IsDelete"  INTEGER,' + sLineBreak +
    '"IsSelect"  INTEGER,' + sLineBreak +
    '"CreateDate"  INTEGER,' + sLineBreak +
    '"LoginDate"  INTEGER,' + sLineBreak +
    '"Sex"  INTEGER,' + sLineBreak +
    '"Job"  INTEGER,' + sLineBreak +
    '"Hair"  INTEGER,' + sLineBreak +
    '"Dir"  INTEGER,' + sLineBreak +
    '"Level"  INTEGER DEFAULT 0,' + sLineBreak +
    '"ReLevel"  INTEGER DEFAULT 0,' + sLineBreak +
    '"Map"  TEXT(30),' + sLineBreak +
    '"X"  INTEGER,' + sLineBreak +
    '"Y"  INTEGER,' + sLineBreak +
    '"HomeMap"  TEXT(30),' + sLineBreak +
    '"HomeX"  INTEGER,' + sLineBreak +
    '"HomeY"  INTEGER,' + sLineBreak +
    '"AttackMode"  INTEGER,' + sLineBreak +
    '"StoragePassword"  TEXT(7),' + sLineBreak +
    '"CreditPoint"  INTEGER,' + sLineBreak +
    '"Gold"  INTEGER,' + sLineBreak +
    '"GameGold"  INTEGER,' + sLineBreak +
    '"GamePoint"  INTEGER,' + sLineBreak +
    '"GameDiamond"  INTEGER,' + sLineBreak +
    '"GameGird"  INTEGER,' + sLineBreak +
    '"GameGoldEx"  INTEGER,' + sLineBreak +
    '"GameGlory"  INTEGER,' + sLineBreak +
    '"PKPoint"  INTEGER,' + sLineBreak +
    '"PayMentPoint"  INTEGER,' + sLineBreak +

    // 扩展2个字段 chongchong 2018-02-06
    '"MemberType"  INTEGER,' + sLineBreak +
    '"MemberLevel"  INTEGER,' + sLineBreak +

    '"IsMaster"  INTEGER,' + sLineBreak +
    '"MasterName"  TEXT(14),' + sLineBreak +
    '"MasterCount"  INTEGER,' + sLineBreak +
    '"MarryCount"  INTEGER,' + sLineBreak +
    '"DearName"  TEXT(14),' + sLineBreak +
    '"IncHP"  INTEGER,' + sLineBreak +
    '"IncMP"  INTEGER,' + sLineBreak +
    '"IncHP2"  INTEGER,' + sLineBreak +
    '"FightZoneDieCount"  INTEGER,' + sLineBreak +
    '"BodyLuck"  REAL,' + sLineBreak +
    '"Contribution"  INTEGER,' + sLineBreak +
    '"HungerStatus"  INTEGER,' + sLineBreak +
    '"KickCount"  INTEGER,' + sLineBreak +
    '"IsLockLogin"  INTEGER,' + sLineBreak +
    '"IsAllowGroup"  INTEGER,' + sLineBreak +
    '"IsAllowGroupRecall"  INTEGER,' + sLineBreak +
    '"GroupRecallTime"  INTEGER,' + sLineBreak +
    '"IsAllowGuildReCall"  INTEGER,' + sLineBreak +
    '"IsDisableTrading"  INTEGER,' + sLineBreak +
    '"IsDisableInviteHorseRiding"  INTEGER,' + sLineBreak +
    '"IsGameGoldTrading"  INTEGER,' + sLineBreak +
    '"IsNewServer"  INTEGER,' + sLineBreak +

    // 扩展三个字段 chongchong 2017-04-16

    //'"IsFilterGlobalMsg"  INTEGER,' + sLineBreak +
    '"IsFilterGlobalDropItemMsg"  INTEGER,' + sLineBreak +
    '"IsFilterGlobalCenterMsg"  INTEGER,' + sLineBreak +
    '"IsFilterGolbalSendMsg"  INTEGER,' + sLineBreak +

    '"IsFixedHero"  INTEGER,' + sLineBreak +
    '"IsStorageHero"  INTEGER,' + sLineBreak +
    '"IsStorageDeputyHero"  INTEGER,' + sLineBreak +
    '"HeroName"  TEXT,' + sLineBreak +
    '"DeputyHeroName"  TEXT,' + sLineBreak +
    '"DeputyHeroJob"  INTEGER,' + sLineBreak +
    '"Nation"  INTEGER,' + sLineBreak +
    '"NationCredit"  INTEGER,' + sLineBreak +
    '"RevivalTime"  INTEGER,' + sLineBreak +
    '"InfinityStorageExtCount"  INTEGER,' + sLineBreak +
    '"IsSaveKillMonExpRate"  INTEGER,' + sLineBreak +
    '"KillMonExpRate"  INTEGER,' + sLineBreak +
    '"KillMonExpRateTime"  INTEGER,' + sLineBreak +
    '"IsSavePowerRate"  INTEGER,' + sLineBreak +
    '"PowerRate"  INTEGER,' + sLineBreak +
    '"PowerRateTime"  INTEGER,' + sLineBreak +
    '"IsAttackMonSavePowerRate"  INTEGER,' + sLineBreak +
    '"AttackMonPowerRate"  INTEGER,' + sLineBreak +
    '"AttackMonPowerRateTime"  INTEGER,' + sLineBreak +
    '"IsSaveKillMonBurstRate"  INTEGER,' + sLineBreak +
    '"KillMonBurstRate"  INTEGER,' + sLineBreak +
    '"KillMonBurstRateTime"  INTEGER,' + sLineBreak +
    '"FBCreateTime"  INTEGER,' + sLineBreak +
    '"JewelryBoxStatus"  INTEGER,' + sLineBreak +
    '"IsShowFashion"  INTEGER,' + sLineBreak +
    '"IsShowGodBless"  INTEGER,' + sLineBreak +
    '"ActiveFengHao"  INTEGER,' + sLineBreak +

    //------------------------------------------------------------2017-05-06 添加新字段
    '	"IsOpenStorage1" INTEGER,' + sLineBreak +
    '	"IsOpenStorage2" INTEGER,' + sLineBreak +
    '	"IsOpenStorage3" INTEGER,' + sLineBreak +

    //------------------------------------------------------------2019-05-12 添加新字段
    '	"ExtBagPageCount" INTEGER DEFAULT 0,' + sLineBreak +
    '	"ExtBagOpenItemCount" INTEGER DEFAULT 0,' + sLineBreak +
    '	"AddMaxWeight" INTEGER DEFAULT 0,' + sLineBreak +
    
    '"HighLevelKillMonFixExpTimeLeft"  INTEGER,' + sLineBreak +

    //------------------------------------------------------------20170425 添加新字段
    '"MobileNumber"  TEXT(20) COLLATE NOCASE ,' + sLineBreak +
    '"IsMobileBind"  INTEGER,' + sLineBreak +
    '"MobileVerifyCode"  TEXT(8) COLLATE NOCASE ,' + sLineBreak +
    '"MobileSendTick"  INTEGER,' + sLineBreak +
    '"MobileResendCount"  INTEGER,' + sLineBreak +

    '"ClearDayVarTime"  INTEGER DEFAULT 0,' + sLineBreak +

    'CONSTRAINT "uk_HumanName" UNIQUE ("HumanName" ASC)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE INDEX "idx_Account" ON "Human" ("Account" ASC);' + sLineBreak +
    'CREATE INDEX "idx_HumanName" ON "Human" ("HumanName" ASC);' + sLineBreak +
    'INSERT INTO "sqlite_sequence" (name, seq) VALUES ("Human", 1);' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanAbil" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"AC1"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AC2"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MAC1"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MAC2"  INTEGER DEFAULT 0,' + sLineBreak +
    '"DC1"  INTEGER DEFAULT 0,' + sLineBreak +
    '"DC2"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MC1"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MC2"  INTEGER DEFAULT 0,' + sLineBreak +
    '"SC1"  INTEGER DEFAULT 0,' + sLineBreak +
    '"SC2"  INTEGER,' + sLineBreak +
    '"HP"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MaxHP"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MP"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MaxMP"  INTEGER DEFAULT 0,' + sLineBreak +
    '"Exp"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MaxExp"  INTEGER DEFAULT 0,' + sLineBreak +
    '"Weight"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MaxWeight"  INTEGER DEFAULT 0,' + sLineBreak +
    '"WearWeight"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MaxWearWeight"  INTEGER DEFAULT 0,' + sLineBreak +
    '"HandWeight"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MaxHandWeight"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AdjustAbilPoint"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AdjustAbilDC"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AdjustAbilMC"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AdjustAbilSC"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AdjustAbilAC"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AdjustAbilMAC"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AdjustAbilHP"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AdjustAbilMP"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AdjustAbilHit"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AdjustAbilSpeed"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AdjustAbilMaxRate"  INTEGER DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL ON UPDATE CASCADE' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanAbilNG" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"IsTrainingNG"  INTEGER,' + sLineBreak +
    '"IsTrainingXF"  INTEGER,' + sLineBreak +
    '"AbilNGLevel"  INTEGER,' + sLineBreak +
    '"AbilNGValue"  INTEGER,' + sLineBreak +
    '"AbilNGMaxValue"  INTEGER,' + sLineBreak +
    '"AbilNGExp"  INTEGER,' + sLineBreak +
    '"AbilNGMaxExp"  INTEGER,' + sLineBreak +
    '"ContinuousMagicOrder1"  INTEGER,' + sLineBreak +
    '"ContinuousMagicOrder2"  INTEGER,' + sLineBreak +
    '"ContinuousMagicOrder3"  INTEGER,' + sLineBreak +
    '"IsOpenLastContinuous"  INTEGER,' + sLineBreak +
    '"LastContinuousMagicOrder"  INTEGER,' + sLineBreak +
    '"Meridians1Level"  INTEGER,' + sLineBreak +
    '"Meridians1BlastHitRate1"  INTEGER,' + sLineBreak +
    '"Meridians1Acupoints1"  INTEGER,' + sLineBreak +
    '"Meridians1Acupoints2"  INTEGER,' + sLineBreak +
    '"Meridians1Acupoints3"  INTEGER,' + sLineBreak +
    '"Meridians1Acupoints4"  INTEGER,' + sLineBreak +
    '"Meridians1Acupoints5"  INTEGER,' + sLineBreak +
    '"Meridians2Level"  INTEGER,' + sLineBreak +
    '"Meridians2BlastHitRate"  INTEGER,' + sLineBreak +
    '"Meridians2Acupoints1"  INTEGER,' + sLineBreak +
    '"Meridians2Acupoints2"  INTEGER,' + sLineBreak +
    '"Meridians2Acupoints3"  INTEGER,' + sLineBreak +
    '"Meridians2Acupoints4"  INTEGER,' + sLineBreak +
    '"Meridians2Acupoints5"  INTEGER,' + sLineBreak +
    '"Meridians3Level"  INTEGER,' + sLineBreak +
    '"Meridians3BlastHitRate"  INTEGER,' + sLineBreak +
    '"Meridians3Acupoints1"  INTEGER,' + sLineBreak +
    '"Meridians3Acupoints2"  INTEGER,' + sLineBreak +
    '"Meridians3Acupoints3"  INTEGER,' + sLineBreak +
    '"Meridians3Acupoints4"  INTEGER,' + sLineBreak +
    '"Meridians3Acupoints5"  INTEGER,' + sLineBreak +
    '"Meridians4Level"  INTEGER,' + sLineBreak +
    '"Meridians4BlastHitRate"  INTEGER,' + sLineBreak +
    '"Meridians4Acupoints1"  INTEGER,' + sLineBreak +
    '"Meridians4Acupoints2"  INTEGER,' + sLineBreak +
    '"Meridians4Acupoints3"  INTEGER,' + sLineBreak +
    '"Meridians4Acupoints4"  INTEGER,' + sLineBreak +
    '"Meridians4Acupoints5"  INTEGER,' + sLineBreak +
    '"Meridians5Level"  INTEGER,' + sLineBreak +
    '"Meridians5BlastHitRate"  INTEGER,' + sLineBreak +
    '"Meridians5Acupoints1"  INTEGER,' + sLineBreak +
    '"Meridians5Acupoints2"  INTEGER,' + sLineBreak +
    '"Meridians5Acupoints3"  INTEGER,' + sLineBreak +
    '"Meridians5Acupoints4"  INTEGER,' + sLineBreak +
    '"Meridians5Acupoints5"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanAbilNpcAdd" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "Index" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanAbilWine" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"IsDrinkedWine"  INTEGER,' + sLineBreak +
    '"IsDrinkWineDrunk"  INTEGER,' + sLineBreak +
    '"DrinkWineQuality"  INTEGER,' + sLineBreak +
    '"DrinkWineAlcohol"  INTEGER,' + sLineBreak +
    '"AbilAlcohol"  INTEGER,' + sLineBreak +
    '"AbilMaxAlcohol"  INTEGER,' + sLineBreak +
    '"AbilDrinkValue"  INTEGER,' + sLineBreak +
    '"AbilMedicineLevel"  INTEGER,' + sLineBreak +
    '"AbilMedicineValue"  INTEGER,' + sLineBreak +
    '"AbilMaxMedicineValue"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC)' + sLineBreak +
     'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL ON UPDATE CASCADE' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanGamePetData" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Name"  TEXT(60),' + sLineBreak +
    '"Level"  INTEGER,' + sLineBreak +
    '"HP"  INTEGER,' + sLineBreak +
    '"MP"  INTEGER,' + sLineBreak +
    '"Exp"  INTEGER,' + sLineBreak +
    '"Magic1"  INTEGER,' + sLineBreak +
    '"Magic2"  INTEGER,' + sLineBreak +
    '"Magic3"  INTEGER,' + sLineBreak +
    '"Magic4"  INTEGER,' + sLineBreak +
    '"Magic5"  INTEGER,' + sLineBreak +
    '"Magic6"  INTEGER,' + sLineBreak +
    '"Magic7"  INTEGER,' + sLineBreak +
    '"Magic8"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "Index" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanGodBlessState" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "Index" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanMagic" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"MagicType"  INTEGER NOT NULL,' + sLineBreak +
    '"MagicIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"MagicID"  INTEGER,' + sLineBreak +
    '"MagicAttr"  INTEGER,' + sLineBreak +
    '"MagicLevel"  INTEGER,' + sLineBreak +
    '"MagicNewLevel"  INTEGER,' + sLineBreak +
    '"MagicKey"  INTEGER,' + sLineBreak +
    '"MagicTranPoint"  INTEGER,' + sLineBreak +
    '"MagicIsUseItemAdd"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "MagicType" ASC, "MagicIndex" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanMagicUseTick" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"MagicID"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "MagicID" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanMoney" (' +  sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"MoneyName"  TEXT,' + sLineBreak +
    '"Value"  INTEGER DEFAULT 0,' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak +
    ');'+ sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanQuestFlag" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "Index" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanStatusTime" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "Index" ASC)' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL ON UPDATE CASCADE' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanVariableT" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  TEXT(100),' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "Index" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanVariableU" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "Index" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE "HumanVariableJ" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "Index" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE "HumanVariableZ" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  TEXT(100),' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "Index" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +
    'CREATE TABLE "HumanItems" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"MakeIndex"  INTEGER,' + sLineBreak +
    '"DBIndex"  INTEGER,' + sLineBreak +
    '"Name"  TEXT,' + sLineBreak +
    '"Dura"  INTEGER,' + sLineBreak +
    '"DuraMax"  INTEGER,' + sLineBreak +
    '"HeroM2DressEffect"  INTEGER,' + sLineBreak +
    '"UpgradeCount"  INTEGER,' + sLineBreak +
    '"IsStartTime"  INTEGER,' + sLineBreak +
    '"LimitTime"  INTEGER,' + sLineBreak +
    '"HeroM2Light"  INTEGER,' + sLineBreak +
    '"Color"  INTEGER,' + sLineBreak +
    '"IsBind"  INTEGER,' + sLineBreak +
    '"BindOption"  INTEGER,' + sLineBreak +
    '"Effect"  INTEGER,' + sLineBreak +
    '"NewLooks"  INTEGER,' + sLineBreak +
    '"NewShape"  INTEGER,' + sLineBreak +
    '"FluteCount"  INTEGER,' + sLineBreak +
    '"PropertyText"  TEXT,' + sLineBreak +
    '"PropertyTextColor"  INTEGER,' + sLineBreak +
    '"ItemFrom"  INTEGER,' + sLineBreak +
    '"ItemFromMap"  TEXT,' + sLineBreak +
    '"ItemFromMon"  TEXT,' + sLineBreak +
    '"ItemFromMaker"  TEXT,' + sLineBreak +
    '"ItemFromDate"  REAL,' + sLineBreak +
    '"InsuranceCount"  INTEGER,' + sLineBreak +

    '"NewExpand3"  INTEGER DEFAULT 0,' + sLineBreak +
    '"NewExpand4"  INTEGER DEFAULT 0,' + sLineBreak +

    'PRIMARY KEY ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL,' + sLineBreak +
    'CONSTRAINT "uk_HumanItem" UNIQUE ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanItemElementAdd" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanItemAddDataByte" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanItemAddDataInt" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

  // '------------------------------------------------------' + sLineBreak +
  
   'CREATE TABLE "HumanItemAddDataText" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  TEXT,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanItemFlute" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    '"OverlapCount"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanItemProgress" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"IsOpen"  INTEGER,' + sLineBreak +
    '"NameColor"  INTEGER,' + sLineBreak +
    '"Count"  INTEGER,' + sLineBreak +
    '"ShowType"  INTEGER,' + sLineBreak +
    '"Max"  INTEGER,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    '"Level"  INTEGER,' + sLineBreak +
    '"Name"  TEXT(31),' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanItemProperty" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Color"  INTEGER,' + sLineBreak +
    '"BindType"  INTEGER,' + sLineBreak +
    '"ShowFlag"  INTEGER,' + sLineBreak +
    '"IsPercent"  INTEGER,' + sLineBreak +
    '"HintModule"  INTEGER,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    '"Value2"  INTEGER,' + sLineBreak +
    '"Value3"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HumanItemValueAdd" (' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HumanID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE "HumanSkillPower" (' + sLineBreak +
    '  "HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '  "SkillID"  INTEGER NOT NULL,' + sLineBreak +
    '  "HumanAttackPercent"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "HumanAttackValue"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "MonAttackPercent"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "MonAttackValue"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "DefensePercent"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "DefenseValue"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "RemainingTime"  INTEGER DEFAULT 0,' + sLineBreak +
    '  PRIMARY KEY ("HumanID", "SkillID")' + sLineBreak +
    ');';

    // '------------------------------------------------------' + sLineBreak +
    // '------------------------------------------------------' + sLineBreak +

{$IF CompilerVersion >= 22}
const
{$ELSE}
resourcestring
{$IFEND}
  // resourcestring 不能超过4096，拆成多个搞 2020-09-26 01:36:37
  
  SQLITE_CREATE_ROLEDATA_TABLES_HERO1 =
    'CREATE TABLE "Hero" (' + sLineBreak +
    '"HeroID"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,' + sLineBreak +
    '"HumanID"  INTEGER NOT NULL,' + sLineBreak +
    '"HeroName"  TEXT(14) NOT NULL COLLATE NOCASE,' + sLineBreak +
    '"IsDelete"  INTEGER,' + sLineBreak +
    '"CreateDate"  INTEGER,' + sLineBreak +
    '"Sex"  INTEGER,' + sLineBreak +
    '"Job"  INTEGER,' + sLineBreak +
    '"Hair"  INTEGER,' + sLineBreak +
    '"Status"  INTEGER,' + sLineBreak +
    '"Dir"  INTEGER,' + sLineBreak +
    '"Level"  INTEGER,' + sLineBreak +
    '"ReLevel"  INTEGER,' + sLineBreak +
    '"LoyalPoint"  REAL, ' + sLineBreak +
    '"Map"  TEXT(30),' + sLineBreak +
    '"X"  INTEGER,' + sLineBreak +
    '"Y"  INTEGER,' + sLineBreak +
    '"HomeMap"  TEXT(30),' + sLineBreak +
    '"HomeX"  INTEGER,' + sLineBreak +
    '"HomeY"  INTEGER,' + sLineBreak +
    '"AttackMode"  INTEGER,' + sLineBreak +
    '"CreditPoint"  INTEGER,' + sLineBreak +
    '"PKPoint"  INTEGER,' + sLineBreak +
    '"IncHP"  INTEGER,' + sLineBreak +
    '"IncMP"  INTEGER,' + sLineBreak +
    '"IncHP2"  INTEGER,' + sLineBreak +
    '"FightZoneDieCount"  INTEGER,' + sLineBreak +
    '"BodyLuck"  REAL,' + sLineBreak +
    '"HungerStatus"  INTEGER,' + sLineBreak +
    '"RevivalTime"  INTEGER,' + sLineBreak +
    '"IsSaveKillMonExpRate"  INTEGER,' + sLineBreak +
    '"KillMonExpRate"  INTEGER,' + sLineBreak +
    '"KillMonExpRateTime"  INTEGER,' + sLineBreak +
    '"IsSavePowerRate"  INTEGER,' + sLineBreak +
    '"PowerRate"  INTEGER,' + sLineBreak +
    '"PowerRateTime"  INTEGER,' + sLineBreak +
    '"IsAttackMonSavePowerRate"  INTEGER,' + sLineBreak +
    '"AttackMonPowerRate"  INTEGER,' + sLineBreak +
    '"AttackMonPowerRateTime"  INTEGER,' + sLineBreak +
    '"IsSaveKillMonBurstRate"  INTEGER,' + sLineBreak +
    '"KillMonBurstRate"  INTEGER,' + sLineBreak +
    '"KillMonBurstRateTime"  INTEGER,' + sLineBreak +
    '"JewelryBoxStatus"  INTEGER,' + sLineBreak +
    '"IsShowFashion"  INTEGER,' + sLineBreak +
    '"IsShowGodBless"  INTEGER,' + sLineBreak +
    '"ActiveFengHao"  INTEGER,' + sLineBreak +
    'CONSTRAINT "fk_HumanID" FOREIGN KEY ("HumanID") REFERENCES "Human" ("HumanID") ON DELETE SET NULL,' + sLineBreak +
    'CONSTRAINT "uk_HeroName" UNIQUE ("HeroName" ASC)' + sLineBreak +
    ');' + sLineBreak +
    'CREATE INDEX "idx_HeroName" ON "Hero" ("HeroName" ASC);' + sLineBreak +
    'INSERT INTO "sqlite_sequence" (name, seq) VALUES ("Hero", 1);' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroAbil" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"AC1"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AC2"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MAC1"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MAC2"  INTEGER DEFAULT 0,' + sLineBreak +
    '"DC1"  INTEGER DEFAULT 0,' + sLineBreak +
    '"DC2"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MC1"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MC2"  INTEGER DEFAULT 0,' + sLineBreak +
    '"SC1"  INTEGER DEFAULT 0,' + sLineBreak +
    '"SC2"  INTEGER,' + sLineBreak +
    '"HP"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MaxHP"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MP"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MaxMP"  INTEGER DEFAULT 0,' + sLineBreak +
    '"Exp"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MaxExp"  INTEGER DEFAULT 0,' + sLineBreak +
    '"Weight"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MaxWeight"  INTEGER DEFAULT 0,' + sLineBreak +
    '"WearWeight"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MaxWearWeight"  INTEGER DEFAULT 0,' + sLineBreak +
    '"HandWeight"  INTEGER DEFAULT 0,' + sLineBreak +
    '"MaxHandWeight"  INTEGER DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HeroID" FOREIGN KEY ("HeroID") REFERENCES "Hero" ("HeroID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroAbilNG" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"IsTrainingNG"  INTEGER,' + sLineBreak +
    '"IsTrainingXF"  INTEGER,' + sLineBreak +
    '"AbilNGLevel"  INTEGER,' + sLineBreak +
    '"AbilNGValue"  INTEGER,' + sLineBreak +
    '"AbilNGMaxValue"  INTEGER,' + sLineBreak +
    '"AbilNGExp"  INTEGER,' + sLineBreak +
    '"AbilNGMaxExp"  INTEGER,' + sLineBreak +
    '"ContinuousMagicOrder1"  INTEGER,' + sLineBreak +
    '"ContinuousMagicOrder2"  INTEGER,' + sLineBreak +
    '"ContinuousMagicOrder3"  INTEGER,' + sLineBreak +
    '"IsOpenLastContinuous"  INTEGER,' + sLineBreak +
    '"LastContinuousMagicOrder"  INTEGER,' + sLineBreak +
    '"Meridians1Level"  INTEGER,' + sLineBreak +
    '"Meridians1BlastHitRate1"  INTEGER,' + sLineBreak +
    '"Meridians1Acupoints1"  INTEGER,' + sLineBreak +
    '"Meridians1Acupoints2"  INTEGER,' + sLineBreak +
    '"Meridians1Acupoints3"  INTEGER,' + sLineBreak +
    '"Meridians1Acupoints4"  INTEGER,' + sLineBreak +
    '"Meridians1Acupoints5"  INTEGER,' + sLineBreak +
    '"Meridians2Level"  INTEGER,' + sLineBreak +
    '"Meridians2BlastHitRate"  INTEGER,' + sLineBreak +
    '"Meridians2Acupoints1"  INTEGER,' + sLineBreak +
    '"Meridians2Acupoints2"  INTEGER,' + sLineBreak +
    '"Meridians2Acupoints3"  INTEGER,' + sLineBreak +
    '"Meridians2Acupoints4"  INTEGER,' + sLineBreak +
    '"Meridians2Acupoints5"  INTEGER,' + sLineBreak +
    '"Meridians3Level"  INTEGER,' + sLineBreak +
    '"Meridians3BlastHitRate"  INTEGER,' + sLineBreak +
    '"Meridians3Acupoints1"  INTEGER,' + sLineBreak +
    '"Meridians3Acupoints2"  INTEGER,' + sLineBreak +
    '"Meridians3Acupoints3"  INTEGER,' + sLineBreak +
    '"Meridians3Acupoints4"  INTEGER,' + sLineBreak +
    '"Meridians3Acupoints5"  INTEGER,' + sLineBreak +
    '"Meridians4Level"  INTEGER,' + sLineBreak +
    '"Meridians4BlastHitRate"  INTEGER,' + sLineBreak +
    '"Meridians4Acupoints1"  INTEGER,' + sLineBreak +
    '"Meridians4Acupoints2"  INTEGER,' + sLineBreak +
    '"Meridians4Acupoints3"  INTEGER,' + sLineBreak +
    '"Meridians4Acupoints4"  INTEGER,' + sLineBreak +
    '"Meridians4Acupoints5"  INTEGER,' + sLineBreak +
    '"Meridians5Level"  INTEGER,' + sLineBreak +
    '"Meridians5BlastHitRate"  INTEGER,' + sLineBreak +
    '"Meridians5Acupoints1"  INTEGER,' + sLineBreak +
    '"Meridians5Acupoints2"  INTEGER,' + sLineBreak +
    '"Meridians5Acupoints3"  INTEGER,' + sLineBreak +
    '"Meridians5Acupoints4"  INTEGER,' + sLineBreak +
    '"Meridians5Acupoints5"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HeroID" FOREIGN KEY ("HeroID") REFERENCES "Hero" ("HeroID") ON DELETE SET NULL' + sLineBreak +
    ');';

    // '------------------------------------------------------' + sLineBreak +

  SQLITE_CREATE_ROLEDATA_TABLES_HERO2 =
    'CREATE TABLE "HeroAbilNpcAdd" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "Index" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HeroID" FOREIGN KEY ("HeroID") REFERENCES "Hero" ("HeroID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroAbilWine" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"IsDrinkWineDrunk"  INTEGER,' + sLineBreak +
    '"DrinkWineQuality"  INTEGER,' + sLineBreak +
    '"DrinkWineAlcohol"  INTEGER,' + sLineBreak +
    '"AbilAlcohol"  INTEGER,' + sLineBreak +
    '"AbilMaxAlcohol"  INTEGER,' + sLineBreak +
    '"AbilDrinkValue"  INTEGER,' + sLineBreak +
    '"AbilMedicineLevel"  INTEGER,' + sLineBreak +
    '"AbilMedicineValue"  INTEGER,' + sLineBreak +
    '"AbilMaxMedicineValue"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HeroID" FOREIGN KEY ("HeroID") REFERENCES "Hero" ("HeroID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroGodBlessState" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "Index" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HeroID" FOREIGN KEY ("HeroID") REFERENCES "Hero" ("HeroID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroMagic" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"MagicType"  INTEGER NOT NULL,' + sLineBreak +
    '"MagicIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"MagicID"  INTEGER,' + sLineBreak +
    '"MagicAttr"  INTEGER,' + sLineBreak +
    '"MagicLevel"  INTEGER,' + sLineBreak +
    '"MagicNewLevel"  INTEGER,' + sLineBreak +
    '"MagicKey"  INTEGER,' + sLineBreak +
    '"MagicTranPoint"  INTEGER,' + sLineBreak +
    '"MagicIsUseItemAdd"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "MagicType" ASC, "MagicIndex" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HeroID" FOREIGN KEY ("HeroID") REFERENCES "Hero" ("HeroID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroQuestFlag" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "Index" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HeroID" FOREIGN KEY ("HeroID") REFERENCES "Hero" ("HeroID") ON DELETE SET NULL' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroStatusTime" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"Index"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "Index" ASC)' + sLineBreak +
    ');';

    // '------------------------------------------------------' + sLineBreak +
  SQLITE_CREATE_ROLEDATA_TABLES_HERO3 =
    'CREATE TABLE "HeroItems" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"MakeIndex"  INTEGER,' + sLineBreak +
    '"DBIndex"  INTEGER,' + sLineBreak +
    '"Name"  TEXT,' + sLineBreak +
    '"Dura"  INTEGER,' + sLineBreak +
    '"DuraMax"  INTEGER,' + sLineBreak +
    '"HeroM2DressEffect"  INTEGER,' + sLineBreak +
    '"UpgradeCount"  INTEGER,' + sLineBreak +
    '"IsStartTime"  INTEGER,' + sLineBreak +
    '"LimitTime"  INTEGER,' + sLineBreak +
    '"HeroM2Light"  INTEGER,' + sLineBreak +
    '"Color"  INTEGER,' + sLineBreak +
    '"IsBind"  INTEGER,' + sLineBreak +
    '"BindOption"  INTEGER,' + sLineBreak +
    '"Effect"  INTEGER,' + sLineBreak +
    '"NewLooks"  INTEGER,' + sLineBreak +
    '"NewShape"  INTEGER,' + sLineBreak +
    '"FluteCount"  INTEGER,' + sLineBreak +
    '"PropertyText"  TEXT,' + sLineBreak +
    '"PropertyTextColor"  INTEGER,' + sLineBreak +
    '"ItemFrom"  INTEGER,' + sLineBreak +
    '"ItemFromMap"  TEXT,' + sLineBreak +
    '"ItemFromMon"  TEXT,' + sLineBreak +
    '"ItemFromMaker"  TEXT,' + sLineBreak +
    '"ItemFromDate"  REAL,' + sLineBreak +
    '"InsuranceCount"  INTEGER,' + sLineBreak +

    '"NewExpand3"  INTEGER DEFAULT 0,' + sLineBreak +
    '"NewExpand4"  INTEGER DEFAULT 0,' + sLineBreak +

    'PRIMARY KEY ("HeroID" ASC, "ItemType" ASC, "ItemIndex" ASC),' + sLineBreak +
    'CONSTRAINT "fk_HeroID" FOREIGN KEY ("HeroID") REFERENCES "Hero" ("HeroID") ON DELETE SET NULL ' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroItemElementAdd" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +


    // '------------------------------------------------------' + sLineBreak +
    'CREATE TABLE "HeroItemAddDataByte" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +
    'CREATE TABLE "HeroItemAddDataInt" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroItemAddDataText" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  TEXT,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');';
    
  SQLITE_CREATE_ROLEDATA_TABLES_HERO4 =
    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroItemFlute" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    '"OverlapCount"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroItemProgress" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"IsOpen"  INTEGER,' + sLineBreak +
    '"NameColor"  INTEGER,' + sLineBreak +
    '"Count"  INTEGER,' + sLineBreak +
    '"ShowType"  INTEGER,' + sLineBreak +
    '"Max"  INTEGER,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    '"Level"  INTEGER,' + sLineBreak +
    '"Name"  TEXT(31),' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroItemProperty" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Color"  INTEGER,' + sLineBreak +
    '"BindType"  INTEGER,' + sLineBreak +
    '"ShowFlag"  INTEGER,' + sLineBreak +
    '"IsPercent"  INTEGER,' + sLineBreak +
    '"HintModule"  INTEGER,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    '"Value2"  INTEGER,' + sLineBreak +
    '"Value3"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '------------------------------------------------------' + sLineBreak +

    'CREATE TABLE "HeroItemValueAdd" (' + sLineBreak +
    '"HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("HeroID" ASC, "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE "HeroSkillPower" (' + sLineBreak +
    '  "HeroID"  INTEGER NOT NULL,' + sLineBreak +
    '  "SkillID"  INTEGER NOT NULL,' + sLineBreak +
    '  "HumanAttackPercent"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "HumanAttackValue"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "MonAttackPercent"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "MonAttackValue"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "DefensePercent"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "DefenseValue"  INTEGER DEFAULT 0,' + sLineBreak +
    '  "RemainingTime"  INTEGER DEFAULT 0,' + sLineBreak +
    '  PRIMARY KEY ("HeroID", "SkillID")' + sLineBreak +
    ');';


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

const
  SQLITE_M2DBVERSION = '20200916';

const
  SQLITE_CREATE_M2DATA_TABLES =
    '-- ----------------------------' + sLineBreak +
    '-- Table structure for AuctionAttention' + sLineBreak +
    '-- ----------------------------' + sLineBreak +
    'DROP TABLE IF EXISTS "AuctionAttention";' + sLineBreak +
    'CREATE TABLE "AuctionAttention" (' + sLineBreak +
    '"HumanName"  TEXT NOT NULL,' + sLineBreak +
    '"AuctionID"  INTEGER NOT NULL,' + sLineBreak +
    '"Time"  INTEGER DEFAULT (strftime(''%s'', ''now'')),' + sLineBreak +
    'PRIMARY KEY ("HumanName" ASC, "AuctionID" ASC)' + sLineBreak +
    ');' + sLineBreak +

    '-- ----------------------------' + sLineBreak +
    '-- Table structure for AuctionData' + sLineBreak +
    '-- ----------------------------' + sLineBreak +
    'DROP TABLE IF EXISTS "AuctionData";' + sLineBreak +
    'CREATE TABLE "AuctionData" (' + sLineBreak +
    '"AuctionID"  INTEGER NOT NULL,' + sLineBreak +
    '"HumanName"  TEXT(14) NOT NULL COLLATE NOCASE,' + sLineBreak +
    '"ItemGroup"  INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    '"ItemColor"  INTEGER DEFAULT 0,' + sLineBreak +
    '"AddDateTime"  INTEGER NOT NULL DEFAULT (strftime(''%s'', ''now'')),' + sLineBreak +
    '"AuctionTime"  INTEGER NOT NULL,' + sLineBreak +
    '"StartingPrice"  INTEGER DEFAULT 0,' + sLineBreak +
    '"SellingPrice"  INTEGER DEFAULT 0,' + sLineBreak +
    '"CurrencyType"  INTEGER,' + sLineBreak +
    '"LastBidder"  TEXT(14) COLLATE NOCASE,' + sLineBreak +
    '"LastBidPrice"  INTEGER,' + sLineBreak +
    '"LastBidTime"  INTEGER,' + sLineBreak +
    '"TradingStatus"  INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    '"IsItemGive"  INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    '"IsTopmost"  INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    '"ItemDBName" TEXT(60) COLLATE NOCASE,' + sLineBreak +
    '"ItemName" TEXT(60) COLLATE NOCASE,' + sLineBreak +
    'PRIMARY KEY ("AuctionID" ASC)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE INDEX if not exists "idx_AuctionDataItemGroup" ON "AuctionData" ("ItemGroup" ASC); ' + sLineBreak +
    'CREATE INDEX if not exists "idx_AuctionDataItemName" ON "AuctionData" ("ItemName" ASC); ' + sLineBreak +
    'CREATE INDEX if not exists "idx_AuctionDataItemDBName" ON "AuctionData" ("ItemDBName" ASC); ' + sLineBreak +


    '-- ----------------------------' + sLineBreak +
    '-- Table structure for db_constant' + sLineBreak +
    '-- ----------------------------' + sLineBreak +
    'DROP TABLE IF EXISTS "db_constant";' + sLineBreak +
    'CREATE TABLE "db_constant" (' + sLineBreak +
    '"ConstName"  TEXT(30) NOT NULL COLLATE NOCASE ,' + sLineBreak +
    '"ConstValue"  INTEGER, ' + sLineBreak +
    'PRIMARY KEY ("ConstName" ASC)' + sLineBreak +
    ');' + sLineBreak +

    'INSERT INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_M2DBVERSION + ');' + sLineBreak +

    '-- ----------------------------' + sLineBreak +
    '-- Table structure for ItemElementAdd' + sLineBreak +
    '-- ----------------------------' + sLineBreak +
    'DROP TABLE IF EXISTS "ItemElementAdd";' + sLineBreak +
    'CREATE TABLE "ItemElementAdd" (' + sLineBreak +
    '"ParentID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("ParentID", "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    '-- ----------------------------' + sLineBreak +
    '-- Table structure for ItemAddDataByte' + sLineBreak +
    '-- ----------------------------' + sLineBreak +
    'DROP TABLE IF EXISTS "ItemAddDataByte";' + sLineBreak +
    'CREATE TABLE "ItemAddDataByte" (' + sLineBreak +
    '"ParentID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("ParentID", "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    '-- ----------------------------' + sLineBreak +
    '-- Table structure for ItemAddDataInt' + sLineBreak +
    '-- ----------------------------' + sLineBreak +
    'DROP TABLE IF EXISTS "ItemAddDataInt";' + sLineBreak +
    'CREATE TABLE "ItemAddDataInt" (' + sLineBreak +
    '"ParentID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("ParentID", "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    '-- ----------------------------' + sLineBreak +
    '-- Table structure for ItemAddDataText' + sLineBreak +
    '-- ----------------------------' + sLineBreak +
    'DROP TABLE IF EXISTS "ItemAddDataText";' + sLineBreak +
    'CREATE TABLE "ItemAddDataText" (' + sLineBreak +
    '"ParentID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  TEXT,' + sLineBreak +
    'PRIMARY KEY ("ParentID", "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    '-- ----------------------------' + sLineBreak +
    '-- Table structure for ItemFlute' + sLineBreak +
    '-- ----------------------------' + sLineBreak +
    'DROP TABLE IF EXISTS "ItemFlute";' + sLineBreak +
    'CREATE TABLE "ItemFlute" (' + sLineBreak +
    '"ParentID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    '"OverlapCount"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("ParentID", "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    '-- ----------------------------' + sLineBreak +
    '-- Table structure for ItemProgress' + sLineBreak +
    '-- ----------------------------' + sLineBreak +
    'DROP TABLE IF EXISTS "ItemProgress";' + sLineBreak +
    'CREATE TABLE "ItemProgress" (' + sLineBreak +
    '"ParentID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"IsOpen"  INTEGER,' + sLineBreak +
    '"NameColor"  INTEGER,' + sLineBreak +
    '"Count"  INTEGER,' + sLineBreak +
    '"ShowType"  INTEGER,' + sLineBreak +
    '"Max"  INTEGER,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    '"Level"  INTEGER,' + sLineBreak +
    '"Name"  TEXT(31),' + sLineBreak +
    'PRIMARY KEY ("ParentID", "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    '-- ----------------------------' + sLineBreak +
    '-- Table structure for ItemProperty' + sLineBreak +
    '-- ----------------------------' + sLineBreak +
    'DROP TABLE IF EXISTS "ItemProperty";' + sLineBreak +
    'CREATE TABLE "ItemProperty" (' + sLineBreak +
    '"ParentID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Color"  INTEGER,' + sLineBreak +
    '"BindType"  INTEGER,' + sLineBreak +
    '"ShowFlag"  INTEGER,' + sLineBreak +
    '"IsPercent"  INTEGER,' + sLineBreak +
    '"HintModule"  INTEGER,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    '"Value2"  INTEGER,' + sLineBreak +
    '"Value3"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("ParentID", "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +

    '-- ----------------------------' + sLineBreak +
    '-- Table structure for Items' + sLineBreak +
    '-- ----------------------------' + sLineBreak +
    'DROP TABLE IF EXISTS "Items";' + sLineBreak +
    'CREATE TABLE "Items" (' + sLineBreak +
    '"ParentID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"MakeIndex"  INTEGER,' + sLineBreak +
    '"DBIndex"  INTEGER,' + sLineBreak +
    '"Name"  TEXT,' + sLineBreak +
    '"Dura"  INTEGER,' + sLineBreak +
    '"DuraMax"  INTEGER,' + sLineBreak +
    '"HeroM2DressEffect"  INTEGER,' + sLineBreak +
    '"UpgradeCount"  INTEGER,' + sLineBreak +
    '"IsStartTime"  INTEGER,' + sLineBreak +
    '"LimitTime"  INTEGER,' + sLineBreak +
    '"HeroM2Light"  INTEGER,' + sLineBreak +
    '"Color"  INTEGER,' + sLineBreak +
    '"IsBind"  INTEGER,' + sLineBreak +
    '"BindOption"  INTEGER,' + sLineBreak +
    '"Effect"  INTEGER,' + sLineBreak +
    '"NewLooks"  INTEGER,' + sLineBreak +
    '"NewShape"  INTEGER,' + sLineBreak +
    '"FluteCount"  INTEGER,' + sLineBreak +
    '"PropertyText"  TEXT,' + sLineBreak +
    '"PropertyTextColor"  INTEGER,' + sLineBreak +
    '"ItemFrom"  INTEGER,' + sLineBreak +
    '"ItemFromMap"  TEXT,' + sLineBreak +
    '"ItemFromMon"  TEXT,' + sLineBreak +
    '"ItemFromMaker"  TEXT,' + sLineBreak +
    '"ItemFromDate"  REAL,' + sLineBreak +
    '"InsuranceCount"  INTEGER,' + sLineBreak +

    '"NewExpand3"  INTEGER DEFAULT 0,' + sLineBreak +
    '"NewExpand4"  INTEGER DEFAULT 0,' + sLineBreak +

    'PRIMARY KEY ("ParentID", "ItemType", "ItemIndex")' + sLineBreak +
    ');' + sLineBreak +

    '-- ----------------------------' + sLineBreak +
    '-- Table structure for ItemValueAdd' + sLineBreak +
    '-- ----------------------------' + sLineBreak +
    'DROP TABLE IF EXISTS "ItemValueAdd";' + sLineBreak +
    'CREATE TABLE "ItemValueAdd" (' + sLineBreak +
    '"ParentID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("ParentID", "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak +
    ');' + sLineBreak +


    // '---------------------------------------------------------------' + sLineBreak +
    'CREATE TABLE "StorageEx" (' + sLineBreak +
    '"StorageID"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL DEFAULT 0,' + sLineBreak +
    '"HumanName"  TEXT(14) NOT NULL COLLATE NOCASE ,' + sLineBreak +
    'CONSTRAINT "uk_HumanName" UNIQUE ("HumanName" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '---------------------------------------------------------------' + sLineBreak +
    'CREATE TABLE "UserShop" (' + sLineBreak +
    '"ShopID"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,' + sLineBreak +
    '"HumanName"  TEXT(14) COLLATE NOCASE ,' + sLineBreak +
    '"ShopName"  TEXT(14) COLLATE NOCASE ,' + sLineBreak +
    '"IsBusiness"  INTEGER,' + sLineBreak +
    '"CreateDate"  INTEGER DEFAULT (strftime(''%s'', ''now'')),' + sLineBreak +
    '"CareValue"  INTEGER DEFAULT 0,' + sLineBreak +
    'CONSTRAINT "uk_HumanName" UNIQUE ("HumanName" ASC)' + sLineBreak +
    ');' + sLineBreak +

    // '---------------------------------------------------------------' + sLineBreak +
    'CREATE TABLE "UserShopItem" (' + sLineBreak +
    '"ShopID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER,' + sLineBreak +
    '"CreateDate"  INTEGER,' + sLineBreak +
    '"IsAllowSell"  INTEGER,' + sLineBreak +
    '"MoneyType"  INTEGER,' + sLineBreak +
    '"ItemPrice"  INTEGER,' + sLineBreak +
    '"IsGetMoney"  INTEGER,' + sLineBreak +
    '"BuyerName"  TEXT(14),' + sLineBreak +
    '"ItemDBName"  TEXT(60) COLLATE NOCASE ,' + sLineBreak +
    '"ItemName"  TEXT(60) COLLATE NOCASE ,' + sLineBreak +
    'PRIMARY KEY ("ShopID" ASC, "ItemID" ASC)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE INDEX if not exists "idx_HumanName" ON "UserShop" ("HumanName" ASC); ' + sLineBreak +
    'CREATE INDEX if not exists "idx_ShopName" ON "UserShop" ("ShopName" ASC); ' + sLineBreak +
    'CREATE INDEX if not exists "idx_IsBusiness" ON "UserShop" ("IsBusiness" ASC); ' + sLineBreak +

    'CREATE INDEX if not exists "idx_ItemPrices" ON "UserShopItem" ("ItemPrice" ASC); ' + sLineBreak +
    'CREATE INDEX if not exists "idx_ItemName" ON "UserShopItem" ("ItemName" ASC); ' + sLineBreak +
    'CREATE INDEX if not exists "idx_ItemDBName" ON "UserShopItem" ("ItemDBName" ASC); ' + sLineBreak;

implementation

end.

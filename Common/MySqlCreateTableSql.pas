unit MySqlCreateTableSql;

interface

uses
  SysUtils;

const
  MYSQL_DBVERSION = '20220219';

const
  MYSQL_CEATE_DB_CONSTANT =
    'CREATE TABLE if not exists `db_constant`(' + sLineBreak +
    '  `ConstName` varchar(30) NOT NULL,' + sLineBreak +
    '  `ConstValue` INTEGER,' + sLineBreak +
    '  PRIMARY KEY (`ConstName`) USING BTREE' + sLineBreak +
    ');';

  MYSQL_CREATE_ACCOUNT_TABLES =
    'drop table IF EXISTS `Account`;' + sLineBreak +
    'CREATE TABLE `Account`  (' + sLineBreak +
    '  `Account` varchar(10) NOT NULL,' + sLineBreak +
    '  `Password` varchar(10) NOT NULL,' + sLineBreak +
    '  `UserName` varchar(20) NOT NULL,' + sLineBreak +
    '  `Disable` INTEGER DEFAULT 0,' + sLineBreak +
    '  `IDCard` varchar(18),' + sLineBreak +
    '  `BirthDay` varchar(10),' + sLineBreak +
    '  `Questions1` varchar(20),' + sLineBreak +
    '  `Answers1` varchar(12),' + sLineBreak +
    '  `Questions2` varchar(20),' + sLineBreak +
    '  `Answers2` varchar(12),' + sLineBreak +
    '  `Phone` varchar(14),' + sLineBreak +
    '  `MobilePhone` varchar(13),' + sLineBreak +
    '  `Mail` varchar(40),' + sLineBreak +
    '  `L2Password` varchar(20),' + sLineBreak +
    '  `CreateDate` INTEGER,' + sLineBreak +
    '  `LoginDate` INTEGER,' + sLineBreak +
    '  `LoginMac` varchar(32),' + sLineBreak +
    '  `LoginIP` INTEGER,' + sLineBreak +
    '  `LastActionTick` INTEGER,' + sLineBreak +
    '  `ErrorCount` INTEGER,' + sLineBreak +
    '  `Memo` varchar(20),' + sLineBreak +
    '  PRIMARY KEY (`Account`) USING BTREE,' + sLineBreak +
    '  UNIQUE INDEX `uk_account`(`Account`) USING BTREE' + sLineBreak +
    ');' + sLineBreak +

    MYSQL_CEATE_DB_CONSTANT +

    'REPLACE INTO db_constant(ConstName, ConstValue) Values("account_version", ' + MYSQL_DBVERSION + ')';

  //////////////////////////////////////////////////////////////////////////////////
                       
  MYSQL_CREATE_ROLEDATA_TABLES =
    'drop table IF EXISTS `HeroAbil`;' + sLineBreak +
    'drop table IF EXISTS `HeroAbilNG`;' + sLineBreak +
    'drop table IF EXISTS `HeroAbilNpcAdd`;' + sLineBreak +
    'drop table IF EXISTS `HeroAbilWine`;' + sLineBreak +
    'drop table IF EXISTS `HeroGodBlessState`;' + sLineBreak +
    'drop table IF EXISTS `HeroMagic`;' + sLineBreak +
    'drop table IF EXISTS `HeroQuestFlag`;' + sLineBreak +
    'drop table IF EXISTS `HeroStatusTime`;' + sLineBreak +
    'drop table IF EXISTS `HeroItems`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemElementAdd`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemAddDataByte`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemAddDataInt`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemAddDataText`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemFlute`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemProgress`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemProperty`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemValueAdd`;' + sLineBreak +
    'drop table IF EXISTS `HeroSkillPower`;' + sLineBreak +
    'drop table IF EXISTS `Hero`;' + sLineBreak +

    'drop table IF EXISTS `HumanAbil`;' + sLineBreak +
    'drop table IF EXISTS `HumanAbilNG`;' + sLineBreak +
    'drop table IF EXISTS `HumanAbilNpcAdd`;' + sLineBreak +
    'drop table IF EXISTS `HumanAbilWine`;' + sLineBreak +
    'drop table IF EXISTS `HumanGamePetData`;' + sLineBreak +
    'drop table IF EXISTS `HumanGodBlessState`;' + sLineBreak +
    'drop table IF EXISTS `HumanMagic`;' + sLineBreak +
    'drop table IF EXISTS `HumanMagicUseTick`;' + sLineBreak +
    'drop table IF EXISTS `HumanQuestFlag`;' + sLineBreak +
    'drop table IF EXISTS `HumanStatusTime`;' + sLineBreak +
    'drop table IF EXISTS `HumanVariableT`;' + sLineBreak +
    'drop table IF EXISTS `HumanVariableU`;' + sLineBreak +
    'drop table IF EXISTS `HumanVariableJ`;' + sLineBreak +  
    'drop table IF EXISTS `HumanVariableZ`;' + sLineBreak +
    'drop table IF EXISTS `HumanItems`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemElementAdd`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemAddDataByte`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemAddDataInt`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemAddDataText`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemFlute`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemProgress`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemProperty`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemValueAdd`;' + sLineBreak +
    'drop table IF EXISTS `HumanSkillPower`;' + sLineBreak +
    'drop table IF EXISTS `Human`;' + sLineBreak +

                      
    'CREATE TABLE `Human` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL AUTO_INCREMENT,' + sLineBreak +
    '`Account`  varchar(10) NOT NULL,' + sLineBreak +
    '`HumanName`  varchar(14) NOT NULL,' + sLineBreak +
    '`IsDelete`  INTEGER,' + sLineBreak +
    '`IsSelect`  INTEGER,' + sLineBreak +
    '`CreateDate`  INTEGER,' + sLineBreak +
    '`LoginDate`  INTEGER,' + sLineBreak +
    '`Sex`  INTEGER,' + sLineBreak +
    '`Job`  INTEGER,' + sLineBreak +
    '`Hair`  INTEGER,' + sLineBreak +
    '`Dir`  INTEGER,' + sLineBreak +
    '`Level`  INTEGER DEFAULT 0,' + sLineBreak +
    '`ReLevel`  INTEGER DEFAULT 0,' + sLineBreak +
    '`Map`  varchar(30),' + sLineBreak +
    '`X`  INTEGER,' + sLineBreak +
    '`Y`  INTEGER,' + sLineBreak +
    '`HomeMap`  varchar(30),' + sLineBreak +
    '`HomeX`  INTEGER,' + sLineBreak +
    '`HomeY`  INTEGER,' + sLineBreak +
    '`AttackMode`  INTEGER,' + sLineBreak +
    '`StoragePassword`  varchar(7),' + sLineBreak +
    '`CreditPoint`  INTEGER,' + sLineBreak +
    '`Gold`  INTEGER,' + sLineBreak +
    '`GameGold`  INTEGER,' + sLineBreak +
    '`GamePoint`  INTEGER,' + sLineBreak +
    '`GameDiamond`  INTEGER,' + sLineBreak +
    '`GameGird`  INTEGER,' + sLineBreak +
    '`GameGoldEx`  INTEGER,' + sLineBreak +
    '`GameGlory`  INTEGER,' + sLineBreak +
    '`PKPoint`  INTEGER,' + sLineBreak +
    '`PayMentPoint`  INTEGER,' + sLineBreak +

    '`MemberType`  INTEGER,' + sLineBreak +
    '`MemberLevel`  INTEGER,' + sLineBreak +

    '`IsMaster`  INTEGER,' + sLineBreak +
    '`MasterName`  varchar(14),' + sLineBreak +
    '`MasterCount`  INTEGER,' + sLineBreak +
    '`MarryCount`  INTEGER,' + sLineBreak +
    '`DearName`  varchar(14),' + sLineBreak +
    '`IncHP`  INTEGER,' + sLineBreak +
    '`IncMP`  INTEGER,' + sLineBreak +
    '`IncHP2`  INTEGER,' + sLineBreak +
    '`FightZoneDieCount`  INTEGER,' + sLineBreak +
    '`BodyLuck`  REAL,' + sLineBreak +
    '`Contribution`  INTEGER,' + sLineBreak +
    '`HungerStatus`  INTEGER,' + sLineBreak +
    '`KickCount`  INTEGER,' + sLineBreak +
    '`IsLockLogin`  INTEGER,' + sLineBreak +
    '`IsAllowGroup`  INTEGER,' + sLineBreak +
    '`IsAllowGroupRecall`  INTEGER,' + sLineBreak +
    '`GroupRecallTime`  INTEGER,' + sLineBreak +
    '`IsAllowGuildReCall`  INTEGER,' + sLineBreak +
    '`IsDisableTrading`  INTEGER,' + sLineBreak +
    '`IsDisableInviteHorseRiding`  INTEGER,' + sLineBreak +
    '`IsGameGoldTrading`  INTEGER,' + sLineBreak +
    '`IsNewServer`  INTEGER,' + sLineBreak +


    '`IsFilterGlobalDropItemMsg`  INTEGER,' + sLineBreak +
    '`IsFilterGlobalCenterMsg`  INTEGER,' + sLineBreak +
    '`IsFilterGolbalSendMsg`  INTEGER,' + sLineBreak +

    '`IsFixedHero`  INTEGER,' + sLineBreak +
    '`IsStorageHero`  INTEGER,' + sLineBreak +
    '`IsStorageDeputyHero`  INTEGER,' + sLineBreak +
    '`HeroName`  VARCHAR(14),' + sLineBreak +
    '`DeputyHeroName`  VARCHAR(14),' + sLineBreak +
    '`DeputyHeroJob`  INTEGER,' + sLineBreak +
    '`Nation`  INTEGER,' + sLineBreak +
    '`NationCredit`  INTEGER,' + sLineBreak +
    '`RevivalTime`  INTEGER,' + sLineBreak +
    '`InfinityStorageExtCount`  INTEGER,' + sLineBreak +
    '`IsSaveKillMonExpRate`  INTEGER,' + sLineBreak +
    '`KillMonExpRate`  INTEGER,' + sLineBreak +
    '`KillMonExpRateTime`  INTEGER,' + sLineBreak +
    '`IsSavePowerRate`  INTEGER,' + sLineBreak +
    '`PowerRate`  INTEGER,' + sLineBreak +
    '`PowerRateTime`  INTEGER,' + sLineBreak +
    '`IsAttackMonSavePowerRate`  INTEGER,' + sLineBreak +
    '`AttackMonPowerRate`  INTEGER,' + sLineBreak +
    '`AttackMonPowerRateTime`  INTEGER,' + sLineBreak +
    '`IsSaveKillMonBurstRate`  INTEGER,' + sLineBreak +
    '`KillMonBurstRate`  INTEGER,' + sLineBreak +
    '`KillMonBurstRateTime`  INTEGER,' + sLineBreak +
    '`FBCreateTime`  INTEGER,' + sLineBreak +
    '`JewelryBoxStatus`  INTEGER,' + sLineBreak +
    '`IsShowFashion`  INTEGER,' + sLineBreak +
    '`IsShowGodBless`  INTEGER,' + sLineBreak +
    '`ActiveFengHao`  INTEGER,' + sLineBreak +

    '`IsOpenStorage1` INTEGER,' + sLineBreak +
    '`IsOpenStorage2` INTEGER,' + sLineBreak +
    '`IsOpenStorage3` INTEGER,' + sLineBreak +

    //------------------------------------------------------------2019-05-12 添加新字段
    '`ExtBagPageCount`  INTEGER DEFAULT 0,' + sLineBreak +
    '`ExtBagOpenItemCount`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AddMaxWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    
    '`HighLevelKillMonFixExpTimeLeft`  INTEGER,' + sLineBreak +

    '`MobileNumber`  varchar(20) ,' + sLineBreak +
    '`IsMobileBind`  INTEGER,' + sLineBreak +
    '`MobileVerifyCode`  varchar(8),' + sLineBreak +
    '`MobileSendTick`  INTEGER,' + sLineBreak +
    '`MobileResendCount`  INTEGER,' + sLineBreak +

    '`ClearDayVarTime`  INTEGER DEFAULT 0,' + sLineBreak +

    'PRIMARY KEY (`HumanID`),' + sLineBreak +
    'UNIQUE INDEX `uk_HumanName`(`HumanName`),' + sLineBreak +
    'INDEX `idx_Account`(`Account`) USING BTREE,' + sLineBreak +
    'INDEX `idx_HumanName`(`HumanName`) USING BTREE' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanAbil` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`AC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MAC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MAC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`DC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`DC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`SC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`SC2`  INTEGER,' + sLineBreak +
    '`HP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxHP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxMP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`Exp`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxExp`  INTEGER DEFAULT 0,' + sLineBreak +
    '`Weight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`WearWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxWearWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`HandWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxHandWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilPoint`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilDC`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilMC`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilSC`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilAC`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilMAC`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilHP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilMP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilHit`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilSpeed`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilMaxRate`  INTEGER DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY (`HumanID`),' + sLineBreak +
    'CONSTRAINT `fk_HumanID` FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanAbilNG` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`IsTrainingNG`  INTEGER,' + sLineBreak +
    '`IsTrainingXF`  INTEGER,' + sLineBreak +
    '`AbilNGLevel`  INTEGER,' + sLineBreak +
    '`AbilNGValue`  INTEGER,' + sLineBreak +
    '`AbilNGMaxValue`  INTEGER,' + sLineBreak +
    '`AbilNGExp`  INTEGER,' + sLineBreak +
    '`AbilNGMaxExp`  INTEGER,' + sLineBreak +
    '`ContinuousMagicOrder1`  INTEGER,' + sLineBreak +
    '`ContinuousMagicOrder2`  INTEGER,' + sLineBreak +
    '`ContinuousMagicOrder3`  INTEGER,' + sLineBreak +
    '`IsOpenLastContinuous`  INTEGER,' + sLineBreak +
    '`LastContinuousMagicOrder`  INTEGER,' + sLineBreak +
    '`Meridians1Level`  INTEGER,' + sLineBreak +
    '`Meridians1BlastHitRate1`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians2Level`  INTEGER,' + sLineBreak +
    '`Meridians2BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians3Level`  INTEGER,' + sLineBreak +
    '`Meridians3BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians4Level`  INTEGER,' + sLineBreak +
    '`Meridians4BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians5Level`  INTEGER,' + sLineBreak +
    '`Meridians5BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints5`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanAbilNpcAdd` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanAbilWine` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`IsDrinkedWine`  INTEGER,' + sLineBreak +
    '`IsDrinkWineDrunk`  INTEGER,' + sLineBreak +
    '`DrinkWineQuality`  INTEGER,' + sLineBreak +
    '`DrinkWineAlcohol`  INTEGER,' + sLineBreak +
    '`AbilAlcohol`  INTEGER,' + sLineBreak +
    '`AbilMaxAlcohol`  INTEGER,' + sLineBreak +
    '`AbilDrinkValue`  INTEGER,' + sLineBreak +
    '`AbilMedicineLevel`  INTEGER,' + sLineBreak +
    '`AbilMedicineValue`  INTEGER,' + sLineBreak +
    '`AbilMaxMedicineValue`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanGamePetData` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Name`  VARCHAR(60),' + sLineBreak +
    '`Level`  INTEGER,' + sLineBreak +
    '`HP`  INTEGER,' + sLineBreak +
    '`MP`  INTEGER,' + sLineBreak +
    '`Exp`  INTEGER,' + sLineBreak +
    '`Magic1`  INTEGER,' + sLineBreak +
    '`Magic2`  INTEGER,' + sLineBreak +
    '`Magic3`  INTEGER,' + sLineBreak +
    '`Magic4`  INTEGER,' + sLineBreak +
    '`Magic5`  INTEGER,' + sLineBreak +
    '`Magic6`  INTEGER,' + sLineBreak +
    '`Magic7`  INTEGER,' + sLineBreak +
    '`Magic8`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanGodBlessState` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanMagic` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicType`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicID`  INTEGER,' + sLineBreak +
    '`MagicAttr`  INTEGER,' + sLineBreak +
    '`MagicLevel`  INTEGER,' + sLineBreak +
    '`MagicNewLevel`  INTEGER,' + sLineBreak +
    '`MagicKey`  INTEGER,' + sLineBreak +
    '`MagicTranPoint`  INTEGER,' + sLineBreak +
    '`MagicIsUseItemAdd`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `MagicType`, `MagicIndex`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanMagicUseTick` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicID`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `MagicID`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanQuestFlag` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanStatusTime` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanVariableT` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  VARCHAR(100),' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanVariableU` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanVariableJ` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +
                

    'CREATE TABLE `HumanVariableZ` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  VARCHAR(100),' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HumanItems` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`MakeIndex`  INTEGER,' + sLineBreak +
    '`DBIndex`  INTEGER,' + sLineBreak +
    '`Name`  VARCHAR(30),' + sLineBreak +
    '`Dura`  INTEGER,' + sLineBreak +
    '`DuraMax`  INTEGER,' + sLineBreak +
    '`HeroM2DressEffect`  INTEGER,' + sLineBreak +
    '`UpgradeCount`  INTEGER,' + sLineBreak +
    '`IsStartTime`  INTEGER,' + sLineBreak +
    '`LimitTime`  INTEGER,' + sLineBreak +
    '`HeroM2Light`  INTEGER,' + sLineBreak +
    '`Color`  INTEGER,' + sLineBreak +
    '`IsBind`  INTEGER,' + sLineBreak +
    '`BindOption`  INTEGER,' + sLineBreak +
    '`Effect`  INTEGER,' + sLineBreak +
    '`NewLooks`  INTEGER,' + sLineBreak +
    '`NewShape`  INTEGER,' + sLineBreak +
    '`FluteCount`  INTEGER,' + sLineBreak +
    '`PropertyText`  VARCHAR(64),' + sLineBreak +
    '`PropertyTextColor`  INTEGER,' + sLineBreak +
    '`ItemFrom`  INTEGER,' + sLineBreak +
    '`ItemFromMap`  VARCHAR(30),' + sLineBreak +
    '`ItemFromMon`  VARCHAR(40),' + sLineBreak +
    '`ItemFromMaker`  VARCHAR(40),' + sLineBreak +
    '`ItemFromDate`  REAL,' + sLineBreak +
    '`InsuranceCount`  INTEGER,' + sLineBreak +

    '`NewExpand3`  INTEGER DEFAULT 0,' + sLineBreak +
    '`NewExpand4`  INTEGER DEFAULT 0,' + sLineBreak +

    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanItemElementAdd` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HumanItemAddDataByte` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HumanItemAddDataInt` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HumanItemAddDataText` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  VARCHAR(30),' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanItemFlute` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`OverlapCount`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanItemProgress` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`IsOpen`  INTEGER,' + sLineBreak +
    '`NameColor`  INTEGER,' + sLineBreak +
    '`Count`  INTEGER,' + sLineBreak +
    '`ShowType`  INTEGER,' + sLineBreak +
    '`Max`  INTEGER,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`Level`  INTEGER,' + sLineBreak +
    '`Name`  VARCHAR(31),' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanItemProperty` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Color`  INTEGER,' + sLineBreak +
    '`BindType`  INTEGER,' + sLineBreak +
    '`ShowFlag`  INTEGER,' + sLineBreak +
    '`IsPercent`  INTEGER,' + sLineBreak +
    '`HintModule`  INTEGER,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`Value2`  INTEGER,' + sLineBreak +
    '`Value3`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanItemValueAdd` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanSkillPower` (' + sLineBreak +
    '  `HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '  `SkillID`  INTEGER NOT NULL,' + sLineBreak +
    '  `HumanAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `HumanAttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `MonAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `MonAttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `DefensePercent`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `DefenseValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `RemainingTime`  INTEGER DEFAULT 0,' + sLineBreak +
    '  PRIMARY KEY (`HumanID`, `SkillID`)' + sLineBreak +
    ');' + sLineBreak +
    
    'CREATE TABLE `Hero` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL AUTO_INCREMENT,' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`HeroName`  VARCHAR(14) NOT NULL,' + sLineBreak +
    '`IsDelete`  INTEGER,' + sLineBreak +
    '`CreateDate`  INTEGER,' + sLineBreak +
    '`Sex`  INTEGER,' + sLineBreak +
    '`Job`  INTEGER,' + sLineBreak +
    '`Hair`  INTEGER,' + sLineBreak +
    '`Status`  INTEGER,' + sLineBreak +
    '`Dir`  INTEGER,' + sLineBreak +
    '`Level`  INTEGER,' + sLineBreak +
    '`ReLevel`  INTEGER,' + sLineBreak +
    '`LoyalPoint`  REAL, ' + sLineBreak +
    '`Map`  VARCHAR(30),' + sLineBreak +
    '`X`  INTEGER,' + sLineBreak +
    '`Y`  INTEGER,' + sLineBreak +
    '`HomeMap`  VARCHAR(30),' + sLineBreak +
    '`HomeX`  INTEGER,' + sLineBreak +
    '`HomeY`  INTEGER,' + sLineBreak +
    '`AttackMode`  INTEGER,' + sLineBreak +
    '`CreditPoint`  INTEGER,' + sLineBreak +
    '`PKPoint`  INTEGER,' + sLineBreak +
    '`IncHP`  INTEGER,' + sLineBreak +
    '`IncMP`  INTEGER,' + sLineBreak +
    '`IncHP2`  INTEGER,' + sLineBreak +
    '`FightZoneDieCount`  INTEGER,' + sLineBreak +
    '`BodyLuck`  REAL,' + sLineBreak +
    '`HungerStatus`  INTEGER,' + sLineBreak +
    '`RevivalTime`  INTEGER,' + sLineBreak +
    '`IsSaveKillMonExpRate`  INTEGER,' + sLineBreak +
    '`KillMonExpRate`  INTEGER,' + sLineBreak +
    '`KillMonExpRateTime`  INTEGER,' + sLineBreak +
    '`IsSavePowerRate`  INTEGER,' + sLineBreak +
    '`PowerRate`  INTEGER,' + sLineBreak +
    '`PowerRateTime`  INTEGER,' + sLineBreak +
    '`IsAttackMonSavePowerRate`  INTEGER,' + sLineBreak +
    '`AttackMonPowerRate`  INTEGER,' + sLineBreak +
    '`AttackMonPowerRateTime`  INTEGER,' + sLineBreak +
    '`IsSaveKillMonBurstRate`  INTEGER,' + sLineBreak +
    '`KillMonBurstRate`  INTEGER,' + sLineBreak +
    '`KillMonBurstRateTime`  INTEGER,' + sLineBreak +
    '`JewelryBoxStatus`  INTEGER,' + sLineBreak +
    '`IsShowFashion`  INTEGER,' + sLineBreak +
    '`IsShowGodBless`  INTEGER,' + sLineBreak +
    '`ActiveFengHao`  INTEGER,' + sLineBreak +

    'PRIMARY KEY (`HeroID`),' + sLineBreak +
    'UNIQUE INDEX `idx_HeroName`(`HeroName`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroAbil` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`AC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MAC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MAC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`DC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`DC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`SC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`SC2`  INTEGER,' + sLineBreak +
    '`HP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxHP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxMP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`Exp`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxExp`  INTEGER DEFAULT 0,' + sLineBreak +
    '`Weight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`WearWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxWearWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`HandWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxHandWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY (`HeroID`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroAbilNG` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`IsTrainingNG`  INTEGER,' + sLineBreak +
    '`IsTrainingXF`  INTEGER,' + sLineBreak +
    '`AbilNGLevel`  INTEGER,' + sLineBreak +
    '`AbilNGValue`  INTEGER,' + sLineBreak +
    '`AbilNGMaxValue`  INTEGER,' + sLineBreak +
    '`AbilNGExp`  INTEGER,' + sLineBreak +
    '`AbilNGMaxExp`  INTEGER,' + sLineBreak +
    '`ContinuousMagicOrder1`  INTEGER,' + sLineBreak +
    '`ContinuousMagicOrder2`  INTEGER,' + sLineBreak +
    '`ContinuousMagicOrder3`  INTEGER,' + sLineBreak +
    '`IsOpenLastContinuous`  INTEGER,' + sLineBreak +
    '`LastContinuousMagicOrder`  INTEGER,' + sLineBreak +
    '`Meridians1Level`  INTEGER,' + sLineBreak +
    '`Meridians1BlastHitRate1`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians2Level`  INTEGER,' + sLineBreak +
    '`Meridians2BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians3Level`  INTEGER,' + sLineBreak +
    '`Meridians3BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians4Level`  INTEGER,' + sLineBreak +
    '`Meridians4BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians5Level`  INTEGER,' + sLineBreak +
    '`Meridians5BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints5`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroAbilNpcAdd` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroAbilWine` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`IsDrinkWineDrunk`  INTEGER,' + sLineBreak +
    '`DrinkWineQuality`  INTEGER,' + sLineBreak +
    '`DrinkWineAlcohol`  INTEGER,' + sLineBreak +
    '`AbilAlcohol`  INTEGER,' + sLineBreak +
    '`AbilMaxAlcohol`  INTEGER,' + sLineBreak +
    '`AbilDrinkValue`  INTEGER,' + sLineBreak +
    '`AbilMedicineLevel`  INTEGER,' + sLineBreak +
    '`AbilMedicineValue`  INTEGER,' + sLineBreak +
    '`AbilMaxMedicineValue`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HeroGodBlessState` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroMagic` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicType`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicID`  INTEGER,' + sLineBreak +
    '`MagicAttr`  INTEGER,' + sLineBreak +
    '`MagicLevel`  INTEGER,' + sLineBreak +
    '`MagicNewLevel`  INTEGER,' + sLineBreak +
    '`MagicKey`  INTEGER,' + sLineBreak +
    '`MagicTranPoint`  INTEGER,' + sLineBreak +
    '`MagicIsUseItemAdd`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `MagicType`, `MagicIndex`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroQuestFlag` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroStatusTime` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `Index`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroItems` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`MakeIndex`  INTEGER,' + sLineBreak +
    '`DBIndex`  INTEGER,' + sLineBreak +
    '`Name`  VARCHAR(30),' + sLineBreak +
    '`Dura`  INTEGER,' + sLineBreak +
    '`DuraMax`  INTEGER,' + sLineBreak +
    '`HeroM2DressEffect`  INTEGER,' + sLineBreak +
    '`UpgradeCount`  INTEGER,' + sLineBreak +
    '`IsStartTime`  INTEGER,' + sLineBreak +
    '`LimitTime`  INTEGER,' + sLineBreak +
    '`HeroM2Light`  INTEGER,' + sLineBreak +
    '`Color`  INTEGER,' + sLineBreak +
    '`IsBind`  INTEGER,' + sLineBreak +
    '`BindOption`  INTEGER,' + sLineBreak +
    '`Effect`  INTEGER,' + sLineBreak +
    '`NewLooks`  INTEGER,' + sLineBreak +
    '`NewShape`  INTEGER,' + sLineBreak +
    '`FluteCount`  INTEGER,' + sLineBreak +
    '`PropertyText`  VARCHAR(64),' + sLineBreak +
    '`PropertyTextColor`  INTEGER,' + sLineBreak +
    '`ItemFrom`  INTEGER,' + sLineBreak +
    '`ItemFromMap`  VARCHAR(30),' + sLineBreak +
    '`ItemFromMon`  VARCHAR(40),' + sLineBreak +
    '`ItemFromMaker`  VARCHAR(40),' + sLineBreak +
    '`ItemFromDate`  REAL,' + sLineBreak +
    '`InsuranceCount`  INTEGER,' + sLineBreak +
    '`NewExpand3`  INTEGER DEFAULT 0,' + sLineBreak +
    '`NewExpand4`  INTEGER DEFAULT 0,' + sLineBreak +
    
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`),' + sLineBreak +
    'CONSTRAINT `uk_HeroItem` UNIQUE (`HeroID`, `ItemType`, `ItemIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroItemElementAdd` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HeroItemAddDataByte` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HeroItemAddDataInt` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HeroItemAddDataText` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  VARCHAR(30),' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroItemFlute` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`OverlapCount`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroItemProgress` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`IsOpen`  INTEGER,' + sLineBreak +
    '`NameColor`  INTEGER,' + sLineBreak +
    '`Count`  INTEGER,' + sLineBreak +
    '`ShowType`  INTEGER,' + sLineBreak +
    '`Max`  INTEGER,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`Level`  INTEGER,' + sLineBreak +
    '`Name`  VARCHAR(31),' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroItemProperty` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Color`  INTEGER,' + sLineBreak +
    '`BindType`  INTEGER,' + sLineBreak +
    '`ShowFlag`  INTEGER,' + sLineBreak +
    '`IsPercent`  INTEGER,' + sLineBreak +
    '`HintModule`  INTEGER,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`Value2`  INTEGER,' + sLineBreak +
    '`Value3`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroItemValueAdd` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroSkillPower` (' + sLineBreak +
    '  `HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '  `SkillID`  INTEGER NOT NULL,' + sLineBreak +
    '  `HumanAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `HumanAttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `MonAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `MonAttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `DefensePercent`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `DefenseValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `RemainingTime`  INTEGER DEFAULT 0,' + sLineBreak +
    '  PRIMARY KEY (`HeroID`, `SkillID`)' + sLineBreak +
    ');' + sLineBreak +

    MYSQL_CEATE_DB_CONSTANT +

    'REPLACE INTO db_constant(ConstName, ConstValue) Values("role_version", ' + MYSQL_DBVERSION + ')';
         
//  MYSQL_CREATE_ROLEDATA_TABLES =
//    'drop table IF EXISTS `HeroAbil`;' + sLineBreak +
//    'drop table IF EXISTS `HeroAbilNG`;' + sLineBreak +
//    'drop table IF EXISTS `HeroAbilNpcAdd`;' + sLineBreak +
//    'drop table IF EXISTS `HeroAbilWine`;' + sLineBreak +
//    'drop table IF EXISTS `HeroGodBlessState`;' + sLineBreak +
//    'drop table IF EXISTS `HeroMagic`;' + sLineBreak +
//    'drop table IF EXISTS `HeroQuestFlag`;' + sLineBreak +
//    'drop table IF EXISTS `HeroStatusTime`;' + sLineBreak +
//    'drop table IF EXISTS `HeroItems`;' + sLineBreak +
//    'drop table IF EXISTS `HeroItemElementAdd`;' + sLineBreak +
//    'drop table IF EXISTS `HeroItemAddDataByte`;' + sLineBreak +
//    'drop table IF EXISTS `HeroItemAddDataInt`;' + sLineBreak +
//    'drop table IF EXISTS `HeroItemAddDataText`;' + sLineBreak +
//    'drop table IF EXISTS `HeroItemFlute`;' + sLineBreak +
//    'drop table IF EXISTS `HeroItemProgress`;' + sLineBreak +
//    'drop table IF EXISTS `HeroItemProperty`;' + sLineBreak +
//    'drop table IF EXISTS `HeroItemValueAdd`;' + sLineBreak +
//    'drop table IF EXISTS `HeroSkillPower`;' + sLineBreak +
//    'drop table IF EXISTS `Hero`;' + sLineBreak +
//
//    'drop table IF EXISTS `HumanAbil`;' + sLineBreak +
//    'drop table IF EXISTS `HumanAbilNG`;' + sLineBreak +
//    'drop table IF EXISTS `HumanAbilNpcAdd`;' + sLineBreak +
//    'drop table IF EXISTS `HumanAbilWine`;' + sLineBreak +
//    'drop table IF EXISTS `HumanGamePetData`;' + sLineBreak +
//    'drop table IF EXISTS `HumanGodBlessState`;' + sLineBreak +
//    'drop table IF EXISTS `HumanMagic`;' + sLineBreak +
//    'drop table IF EXISTS `HumanMagicUseTick`;' + sLineBreak +
//    'drop table IF EXISTS `HumanQuestFlag`;' + sLineBreak +
//    'drop table IF EXISTS `HumanStatusTime`;' + sLineBreak +
//    'drop table IF EXISTS `HumanVariableT`;' + sLineBreak +
//    'drop table IF EXISTS `HumanVariableU`;' + sLineBreak +
//    'drop table IF EXISTS `HumanVariableJ`;' + sLineBreak +
//    'drop table IF EXISTS `HumanItems`;' + sLineBreak +
//    'drop table IF EXISTS `HumanItemElementAdd`;' + sLineBreak +
//    'drop table IF EXISTS `HumanItemAddDataByte`;' + sLineBreak +
//    'drop table IF EXISTS `HumanItemAddDataInt`;' + sLineBreak +
//    'drop table IF EXISTS `HumanItemAddDataText`;' + sLineBreak +
//    'drop table IF EXISTS `HumanItemFlute`;' + sLineBreak +
//    'drop table IF EXISTS `HumanItemProgress`;' + sLineBreak +
//    'drop table IF EXISTS `HumanItemProperty`;' + sLineBreak +
//    'drop table IF EXISTS `HumanItemValueAdd`;' + sLineBreak +
//    'drop table IF EXISTS `HumanSkillPower`;' + sLineBreak +
//    'drop table IF EXISTS `Human`;' + sLineBreak +
//
//                      
//    'CREATE TABLE `Human` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL AUTO_INCREMENT,' + sLineBreak +
//    '`Account`  varchar(10) NOT NULL,' + sLineBreak +
//    '`HumanName`  varchar(14) NOT NULL,' + sLineBreak +
//    '`IsDelete`  INTEGER,' + sLineBreak +
//    '`IsSelect`  INTEGER,' + sLineBreak +
//    '`CreateDate`  INTEGER,' + sLineBreak +
//    '`LoginDate`  INTEGER,' + sLineBreak +
//    '`Sex`  INTEGER,' + sLineBreak +
//    '`Job`  INTEGER,' + sLineBreak +
//    '`Hair`  INTEGER,' + sLineBreak +
//    '`Dir`  INTEGER,' + sLineBreak +
//    '`Level`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`ReLevel`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`Map`  varchar(30),' + sLineBreak +
//    '`X`  INTEGER,' + sLineBreak +
//    '`Y`  INTEGER,' + sLineBreak +
//    '`HomeMap`  varchar(30),' + sLineBreak +
//    '`HomeX`  INTEGER,' + sLineBreak +
//    '`HomeY`  INTEGER,' + sLineBreak +
//    '`AttackMode`  INTEGER,' + sLineBreak +
//    '`StoragePassword`  varchar(7),' + sLineBreak +
//    '`CreditPoint`  INTEGER,' + sLineBreak +
//    '`Gold`  INTEGER,' + sLineBreak +
//    '`GameGold`  INTEGER,' + sLineBreak +
//    '`GamePoint`  INTEGER,' + sLineBreak +
//    '`GameDiamond`  INTEGER,' + sLineBreak +
//    '`GameGird`  INTEGER,' + sLineBreak +
//    '`GameGoldEx`  INTEGER,' + sLineBreak +
//    '`GameGlory`  INTEGER,' + sLineBreak +
//    '`PKPoint`  INTEGER,' + sLineBreak +
//    '`PayMentPoint`  INTEGER,' + sLineBreak +
//
//    '`MemberType`  INTEGER,' + sLineBreak +
//    '`MemberLevel`  INTEGER,' + sLineBreak +
//
//    '`IsMaster`  INTEGER,' + sLineBreak +
//    '`MasterName`  varchar(14),' + sLineBreak +
//    '`MasterCount`  INTEGER,' + sLineBreak +
//    '`MarryCount`  INTEGER,' + sLineBreak +
//    '`DearName`  varchar(14),' + sLineBreak +
//    '`IncHP`  INTEGER,' + sLineBreak +
//    '`IncMP`  INTEGER,' + sLineBreak +
//    '`IncHP2`  INTEGER,' + sLineBreak +
//    '`FightZoneDieCount`  INTEGER,' + sLineBreak +
//    '`BodyLuck`  REAL,' + sLineBreak +
//    '`Contribution`  INTEGER,' + sLineBreak +
//    '`HungerStatus`  INTEGER,' + sLineBreak +
//    '`KickCount`  INTEGER,' + sLineBreak +
//    '`IsLockLogin`  INTEGER,' + sLineBreak +
//    '`IsAllowGroup`  INTEGER,' + sLineBreak +
//    '`IsAllowGroupRecall`  INTEGER,' + sLineBreak +
//    '`GroupRecallTime`  INTEGER,' + sLineBreak +
//    '`IsAllowGuildReCall`  INTEGER,' + sLineBreak +
//    '`IsDisableTrading`  INTEGER,' + sLineBreak +
//    '`IsDisableInviteHorseRiding`  INTEGER,' + sLineBreak +
//    '`IsGameGoldTrading`  INTEGER,' + sLineBreak +
//    '`IsNewServer`  INTEGER,' + sLineBreak +
//
//
//    '`IsFilterGlobalDropItemMsg`  INTEGER,' + sLineBreak +
//    '`IsFilterGlobalCenterMsg`  INTEGER,' + sLineBreak +
//    '`IsFilterGolbalSendMsg`  INTEGER,' + sLineBreak +
//
//    '`IsFixedHero`  INTEGER,' + sLineBreak +
//    '`IsStorageHero`  INTEGER,' + sLineBreak +
//    '`IsStorageDeputyHero`  INTEGER,' + sLineBreak +
//    '`HeroName`  VARCHAR(14),' + sLineBreak +
//    '`DeputyHeroName`  VARCHAR(14),' + sLineBreak +
//    '`DeputyHeroJob`  INTEGER,' + sLineBreak +
//    '`Nation`  INTEGER,' + sLineBreak +
//    '`NationCredit`  INTEGER,' + sLineBreak +
//    '`RevivalTime`  INTEGER,' + sLineBreak +
//    '`InfinityStorageExtCount`  INTEGER,' + sLineBreak +
//    '`IsSaveKillMonExpRate`  INTEGER,' + sLineBreak +
//    '`KillMonExpRate`  INTEGER,' + sLineBreak +
//    '`KillMonExpRateTime`  INTEGER,' + sLineBreak +
//    '`IsSavePowerRate`  INTEGER,' + sLineBreak +
//    '`PowerRate`  INTEGER,' + sLineBreak +
//    '`PowerRateTime`  INTEGER,' + sLineBreak +
//    '`IsAttackMonSavePowerRate`  INTEGER,' + sLineBreak +
//    '`AttackMonPowerRate`  INTEGER,' + sLineBreak +
//    '`AttackMonPowerRateTime`  INTEGER,' + sLineBreak +
//    '`IsSaveKillMonBurstRate`  INTEGER,' + sLineBreak +
//    '`KillMonBurstRate`  INTEGER,' + sLineBreak +
//    '`KillMonBurstRateTime`  INTEGER,' + sLineBreak +
//    '`FBCreateTime`  INTEGER,' + sLineBreak +
//    '`JewelryBoxStatus`  INTEGER,' + sLineBreak +
//    '`IsShowFashion`  INTEGER,' + sLineBreak +
//    '`IsShowGodBless`  INTEGER,' + sLineBreak +
//    '`ActiveFengHao`  INTEGER,' + sLineBreak +
//
//    '`IsOpenStorage1` INTEGER,' + sLineBreak +
//    '`IsOpenStorage2` INTEGER,' + sLineBreak +
//    '`IsOpenStorage3` INTEGER,' + sLineBreak +
//
//    //------------------------------------------------------------2019-05-12 添加新字段
//    '`ExtBagPageCount`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`ExtBagOpenItemCount`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AddMaxWeight`  INTEGER DEFAULT 0,' + sLineBreak +
//    
//    '`HighLevelKillMonFixExpTimeLeft`  INTEGER,' + sLineBreak +
//
//    '`MobileNumber`  varchar(20) ,' + sLineBreak +
//    '`IsMobileBind`  INTEGER,' + sLineBreak +
//    '`MobileVerifyCode`  varchar(8),' + sLineBreak +
//    '`MobileSendTick`  INTEGER,' + sLineBreak +
//    '`MobileResendCount`  INTEGER,' + sLineBreak +
//
//    '`ClearDayVarTime`  INTEGER DEFAULT 0,' + sLineBreak +
//
//    'PRIMARY KEY (`HumanID`),' + sLineBreak +
//    'UNIQUE INDEX `uk_HumanName`(`HumanName`),' + sLineBreak +
//    'INDEX `idx_Account`(`Account`) USING BTREE,' + sLineBreak +
//    'INDEX `idx_HumanName`(`HumanName`) USING BTREE' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanAbil` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`AC1`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AC2`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MAC1`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MAC2`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`DC1`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`DC2`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MC1`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MC2`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`SC1`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`SC2`  INTEGER,' + sLineBreak +
//    '`HP`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MaxHP`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MP`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MaxMP`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`Exp`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MaxExp`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`Weight`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MaxWeight`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`WearWeight`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MaxWearWeight`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`HandWeight`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MaxHandWeight`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AdjustAbilPoint`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AdjustAbilDC`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AdjustAbilMC`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AdjustAbilSC`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AdjustAbilAC`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AdjustAbilMAC`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AdjustAbilHP`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AdjustAbilMP`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AdjustAbilHit`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AdjustAbilSpeed`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AdjustAbilMaxRate`  INTEGER DEFAULT 0,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`),' + sLineBreak +
//    'CONSTRAINT `fk_HumanID` FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanAbilNG` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`IsTrainingNG`  INTEGER,' + sLineBreak +
//    '`IsTrainingXF`  INTEGER,' + sLineBreak +
//    '`AbilNGLevel`  INTEGER,' + sLineBreak +
//    '`AbilNGValue`  INTEGER,' + sLineBreak +
//    '`AbilNGMaxValue`  INTEGER,' + sLineBreak +
//    '`AbilNGExp`  INTEGER,' + sLineBreak +
//    '`AbilNGMaxExp`  INTEGER,' + sLineBreak +
//    '`ContinuousMagicOrder1`  INTEGER,' + sLineBreak +
//    '`ContinuousMagicOrder2`  INTEGER,' + sLineBreak +
//    '`ContinuousMagicOrder3`  INTEGER,' + sLineBreak +
//    '`IsOpenLastContinuous`  INTEGER,' + sLineBreak +
//    '`LastContinuousMagicOrder`  INTEGER,' + sLineBreak +
//    '`Meridians1Level`  INTEGER,' + sLineBreak +
//    '`Meridians1BlastHitRate1`  INTEGER,' + sLineBreak +
//    '`Meridians1Acupoints1`  INTEGER,' + sLineBreak +
//    '`Meridians1Acupoints2`  INTEGER,' + sLineBreak +
//    '`Meridians1Acupoints3`  INTEGER,' + sLineBreak +
//    '`Meridians1Acupoints4`  INTEGER,' + sLineBreak +
//    '`Meridians1Acupoints5`  INTEGER,' + sLineBreak +
//    '`Meridians2Level`  INTEGER,' + sLineBreak +
//    '`Meridians2BlastHitRate`  INTEGER,' + sLineBreak +
//    '`Meridians2Acupoints1`  INTEGER,' + sLineBreak +
//    '`Meridians2Acupoints2`  INTEGER,' + sLineBreak +
//    '`Meridians2Acupoints3`  INTEGER,' + sLineBreak +
//    '`Meridians2Acupoints4`  INTEGER,' + sLineBreak +
//    '`Meridians2Acupoints5`  INTEGER,' + sLineBreak +
//    '`Meridians3Level`  INTEGER,' + sLineBreak +
//    '`Meridians3BlastHitRate`  INTEGER,' + sLineBreak +
//    '`Meridians3Acupoints1`  INTEGER,' + sLineBreak +
//    '`Meridians3Acupoints2`  INTEGER,' + sLineBreak +
//    '`Meridians3Acupoints3`  INTEGER,' + sLineBreak +
//    '`Meridians3Acupoints4`  INTEGER,' + sLineBreak +
//    '`Meridians3Acupoints5`  INTEGER,' + sLineBreak +
//    '`Meridians4Level`  INTEGER,' + sLineBreak +
//    '`Meridians4BlastHitRate`  INTEGER,' + sLineBreak +
//    '`Meridians4Acupoints1`  INTEGER,' + sLineBreak +
//    '`Meridians4Acupoints2`  INTEGER,' + sLineBreak +
//    '`Meridians4Acupoints3`  INTEGER,' + sLineBreak +
//    '`Meridians4Acupoints4`  INTEGER,' + sLineBreak +
//    '`Meridians4Acupoints5`  INTEGER,' + sLineBreak +
//    '`Meridians5Level`  INTEGER,' + sLineBreak +
//    '`Meridians5BlastHitRate`  INTEGER,' + sLineBreak +
//    '`Meridians5Acupoints1`  INTEGER,' + sLineBreak +
//    '`Meridians5Acupoints2`  INTEGER,' + sLineBreak +
//    '`Meridians5Acupoints3`  INTEGER,' + sLineBreak +
//    '`Meridians5Acupoints4`  INTEGER,' + sLineBreak +
//    '`Meridians5Acupoints5`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanAbilNpcAdd` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Index`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanAbilWine` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`IsDrinkedWine`  INTEGER,' + sLineBreak +
//    '`IsDrinkWineDrunk`  INTEGER,' + sLineBreak +
//    '`DrinkWineQuality`  INTEGER,' + sLineBreak +
//    '`DrinkWineAlcohol`  INTEGER,' + sLineBreak +
//    '`AbilAlcohol`  INTEGER,' + sLineBreak +
//    '`AbilMaxAlcohol`  INTEGER,' + sLineBreak +
//    '`AbilDrinkValue`  INTEGER,' + sLineBreak +
//    '`AbilMedicineLevel`  INTEGER,' + sLineBreak +
//    '`AbilMedicineValue`  INTEGER,' + sLineBreak +
//    '`AbilMaxMedicineValue`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanGamePetData` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Index`  INTEGER NOT NULL,' + sLineBreak +
//    '`Name`  VARCHAR(60),' + sLineBreak +
//    '`Level`  INTEGER,' + sLineBreak +
//    '`HP`  INTEGER,' + sLineBreak +
//    '`MP`  INTEGER,' + sLineBreak +
//    '`Exp`  INTEGER,' + sLineBreak +
//    '`Magic1`  INTEGER,' + sLineBreak +
//    '`Magic2`  INTEGER,' + sLineBreak +
//    '`Magic3`  INTEGER,' + sLineBreak +
//    '`Magic4`  INTEGER,' + sLineBreak +
//    '`Magic5`  INTEGER,' + sLineBreak +
//    '`Magic6`  INTEGER,' + sLineBreak +
//    '`Magic7`  INTEGER,' + sLineBreak +
//    '`Magic8`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanGodBlessState` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Index`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanMagic` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`MagicType`  INTEGER NOT NULL,' + sLineBreak +
//    '`MagicIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`MagicID`  INTEGER,' + sLineBreak +
//    '`MagicAttr`  INTEGER,' + sLineBreak +
//    '`MagicLevel`  INTEGER,' + sLineBreak +
//    '`MagicNewLevel`  INTEGER,' + sLineBreak +
//    '`MagicKey`  INTEGER,' + sLineBreak +
//    '`MagicTranPoint`  INTEGER,' + sLineBreak +
//    '`MagicIsUseItemAdd`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `MagicType`, `MagicIndex`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanMagicUseTick` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`MagicID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `MagicID`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanQuestFlag` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Index`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanStatusTime` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Index`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanVariableT` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Index`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  VARCHAR(100),' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanVariableU` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Index`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanVariableJ` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Index`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER DEFAULT 0,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanItems` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`MakeIndex`  INTEGER,' + sLineBreak +
//    '`DBIndex`  INTEGER,' + sLineBreak +
//    '`Name`  VARCHAR(30),' + sLineBreak +
//    '`Dura`  INTEGER,' + sLineBreak +
//    '`DuraMax`  INTEGER,' + sLineBreak +
//    '`HeroM2DressEffect`  INTEGER,' + sLineBreak +
//    '`UpgradeCount`  INTEGER,' + sLineBreak +
//    '`IsStartTime`  INTEGER,' + sLineBreak +
//    '`LimitTime`  INTEGER,' + sLineBreak +
//    '`HeroM2Light`  INTEGER,' + sLineBreak +
//    '`Color`  INTEGER,' + sLineBreak +
//    '`IsBind`  INTEGER,' + sLineBreak +
//    '`BindOption`  INTEGER,' + sLineBreak +
//    '`Effect`  INTEGER,' + sLineBreak +
//    '`NewLooks`  INTEGER,' + sLineBreak +
//    '`NewShape`  INTEGER,' + sLineBreak +
//    '`FluteCount`  INTEGER,' + sLineBreak +
//    '`PropertyText`  VARCHAR(64),' + sLineBreak +
//    '`PropertyTextColor`  INTEGER,' + sLineBreak +
//    '`ItemFrom`  INTEGER,' + sLineBreak +
//    '`ItemFromMap`  VARCHAR(30),' + sLineBreak +
//    '`ItemFromMon`  VARCHAR(40),' + sLineBreak +
//    '`ItemFromMaker`  VARCHAR(40),' + sLineBreak +
//    '`ItemFromDate`  REAL,' + sLineBreak +
//    '`InsuranceCount`  INTEGER,' + sLineBreak +
//
//    '`NewExpand3`  INTEGER, DEFAULT 0' + sLineBreak +
//    '`NewExpand4`  INTEGER, DEFAULT 0' + sLineBreak +
//
//    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanItemElementAdd` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//    'CREATE TABLE `HumanItemAddDataByte` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//    'CREATE TABLE `HumanItemAddDataInt` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//    'CREATE TABLE `HumanItemAddDataText` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  VARCHAR(30),' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanItemFlute` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    '`OverlapCount`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanItemProgress` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`IsOpen`  INTEGER,' + sLineBreak +
//    '`NameColor`  INTEGER,' + sLineBreak +
//    '`Count`  INTEGER,' + sLineBreak +
//    '`ShowType`  INTEGER,' + sLineBreak +
//    '`Max`  INTEGER,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    '`Level`  INTEGER,' + sLineBreak +
//    '`Name`  VARCHAR(31),' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanItemProperty` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Color`  INTEGER,' + sLineBreak +
//    '`BindType`  INTEGER,' + sLineBreak +
//    '`ShowFlag`  INTEGER,' + sLineBreak +
//    '`IsPercent`  INTEGER,' + sLineBreak +
//    '`HintModule`  INTEGER,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    '`Value2`  INTEGER,' + sLineBreak +
//    '`Value3`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanItemValueAdd` (' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HumanSkillPower` (' + sLineBreak +
//    '  `HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '  `SkillID`  INTEGER NOT NULL,' + sLineBreak +
//    '  `HumanAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  `HumanAttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  `MonAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  `MonAttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  `DefensePercent`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  `DefenseValue`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  `RemainingTime`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  PRIMARY KEY (`HumanID`, `SkillID`)' + sLineBreak +
//    ');' + sLineBreak +
//    
//    'CREATE TABLE `Hero` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL AUTO_INCREMENT,' + sLineBreak +
//    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
//    '`HeroName`  VARCHAR(14) NOT NULL,' + sLineBreak +
//    '`IsDelete`  INTEGER,' + sLineBreak +
//    '`CreateDate`  INTEGER,' + sLineBreak +
//    '`Sex`  INTEGER,' + sLineBreak +
//    '`Job`  INTEGER,' + sLineBreak +
//    '`Hair`  INTEGER,' + sLineBreak +
//    '`Status`  INTEGER,' + sLineBreak +
//    '`Dir`  INTEGER,' + sLineBreak +
//    '`Level`  INTEGER,' + sLineBreak +
//    '`ReLevel`  INTEGER,' + sLineBreak +
//    '`LoyalPoint`  REAL, ' + sLineBreak +
//    '`Map`  VARCHAR(30),' + sLineBreak +
//    '`X`  INTEGER,' + sLineBreak +
//    '`Y`  INTEGER,' + sLineBreak +
//    '`HomeMap`  VARCHAR(30),' + sLineBreak +
//    '`HomeX`  INTEGER,' + sLineBreak +
//    '`HomeY`  INTEGER,' + sLineBreak +
//    '`AttackMode`  INTEGER,' + sLineBreak +
//    '`CreditPoint`  INTEGER,' + sLineBreak +
//    '`PKPoint`  INTEGER,' + sLineBreak +
//    '`IncHP`  INTEGER,' + sLineBreak +
//    '`IncMP`  INTEGER,' + sLineBreak +
//    '`IncHP2`  INTEGER,' + sLineBreak +
//    '`FightZoneDieCount`  INTEGER,' + sLineBreak +
//    '`BodyLuck`  REAL,' + sLineBreak +
//    '`HungerStatus`  INTEGER,' + sLineBreak +
//    '`RevivalTime`  INTEGER,' + sLineBreak +
//    '`IsSaveKillMonExpRate`  INTEGER,' + sLineBreak +
//    '`KillMonExpRate`  INTEGER,' + sLineBreak +
//    '`KillMonExpRateTime`  INTEGER,' + sLineBreak +
//    '`IsSavePowerRate`  INTEGER,' + sLineBreak +
//    '`PowerRate`  INTEGER,' + sLineBreak +
//    '`PowerRateTime`  INTEGER,' + sLineBreak +
//    '`IsAttackMonSavePowerRate`  INTEGER,' + sLineBreak +
//    '`AttackMonPowerRate`  INTEGER,' + sLineBreak +
//    '`AttackMonPowerRateTime`  INTEGER,' + sLineBreak +
//    '`IsSaveKillMonBurstRate`  INTEGER,' + sLineBreak +
//    '`KillMonBurstRate`  INTEGER,' + sLineBreak +
//    '`KillMonBurstRateTime`  INTEGER,' + sLineBreak +
//    '`JewelryBoxStatus`  INTEGER,' + sLineBreak +
//    '`IsShowFashion`  INTEGER,' + sLineBreak +
//    '`IsShowGodBless`  INTEGER,' + sLineBreak +
//    '`ActiveFengHao`  INTEGER,' + sLineBreak +
//
//    'PRIMARY KEY (`HeroID`),' + sLineBreak +
//    'UNIQUE INDEX `idx_HeroName`(`HeroName`),' + sLineBreak +
//    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroAbil` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`AC1`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`AC2`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MAC1`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MAC2`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`DC1`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`DC2`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MC1`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MC2`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`SC1`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`SC2`  INTEGER,' + sLineBreak +
//    '`HP`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MaxHP`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MP`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MaxMP`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`Exp`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MaxExp`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`Weight`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MaxWeight`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`WearWeight`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MaxWearWeight`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`HandWeight`  INTEGER DEFAULT 0,' + sLineBreak +
//    '`MaxHandWeight`  INTEGER DEFAULT 0,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`),' + sLineBreak +
//    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroAbilNG` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`IsTrainingNG`  INTEGER,' + sLineBreak +
//    '`IsTrainingXF`  INTEGER,' + sLineBreak +
//    '`AbilNGLevel`  INTEGER,' + sLineBreak +
//    '`AbilNGValue`  INTEGER,' + sLineBreak +
//    '`AbilNGMaxValue`  INTEGER,' + sLineBreak +
//    '`AbilNGExp`  INTEGER,' + sLineBreak +
//    '`AbilNGMaxExp`  INTEGER,' + sLineBreak +
//    '`ContinuousMagicOrder1`  INTEGER,' + sLineBreak +
//    '`ContinuousMagicOrder2`  INTEGER,' + sLineBreak +
//    '`ContinuousMagicOrder3`  INTEGER,' + sLineBreak +
//    '`IsOpenLastContinuous`  INTEGER,' + sLineBreak +
//    '`LastContinuousMagicOrder`  INTEGER,' + sLineBreak +
//    '`Meridians1Level`  INTEGER,' + sLineBreak +
//    '`Meridians1BlastHitRate1`  INTEGER,' + sLineBreak +
//    '`Meridians1Acupoints1`  INTEGER,' + sLineBreak +
//    '`Meridians1Acupoints2`  INTEGER,' + sLineBreak +
//    '`Meridians1Acupoints3`  INTEGER,' + sLineBreak +
//    '`Meridians1Acupoints4`  INTEGER,' + sLineBreak +
//    '`Meridians1Acupoints5`  INTEGER,' + sLineBreak +
//    '`Meridians2Level`  INTEGER,' + sLineBreak +
//    '`Meridians2BlastHitRate`  INTEGER,' + sLineBreak +
//    '`Meridians2Acupoints1`  INTEGER,' + sLineBreak +
//    '`Meridians2Acupoints2`  INTEGER,' + sLineBreak +
//    '`Meridians2Acupoints3`  INTEGER,' + sLineBreak +
//    '`Meridians2Acupoints4`  INTEGER,' + sLineBreak +
//    '`Meridians2Acupoints5`  INTEGER,' + sLineBreak +
//    '`Meridians3Level`  INTEGER,' + sLineBreak +
//    '`Meridians3BlastHitRate`  INTEGER,' + sLineBreak +
//    '`Meridians3Acupoints1`  INTEGER,' + sLineBreak +
//    '`Meridians3Acupoints2`  INTEGER,' + sLineBreak +
//    '`Meridians3Acupoints3`  INTEGER,' + sLineBreak +
//    '`Meridians3Acupoints4`  INTEGER,' + sLineBreak +
//    '`Meridians3Acupoints5`  INTEGER,' + sLineBreak +
//    '`Meridians4Level`  INTEGER,' + sLineBreak +
//    '`Meridians4BlastHitRate`  INTEGER,' + sLineBreak +
//    '`Meridians4Acupoints1`  INTEGER,' + sLineBreak +
//    '`Meridians4Acupoints2`  INTEGER,' + sLineBreak +
//    '`Meridians4Acupoints3`  INTEGER,' + sLineBreak +
//    '`Meridians4Acupoints4`  INTEGER,' + sLineBreak +
//    '`Meridians4Acupoints5`  INTEGER,' + sLineBreak +
//    '`Meridians5Level`  INTEGER,' + sLineBreak +
//    '`Meridians5BlastHitRate`  INTEGER,' + sLineBreak +
//    '`Meridians5Acupoints1`  INTEGER,' + sLineBreak +
//    '`Meridians5Acupoints2`  INTEGER,' + sLineBreak +
//    '`Meridians5Acupoints3`  INTEGER,' + sLineBreak +
//    '`Meridians5Acupoints4`  INTEGER,' + sLineBreak +
//    '`Meridians5Acupoints5`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`),' + sLineBreak +
//    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroAbilNpcAdd` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Index`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `Index`),' + sLineBreak +
//    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroAbilWine` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`IsDrinkWineDrunk`  INTEGER,' + sLineBreak +
//    '`DrinkWineQuality`  INTEGER,' + sLineBreak +
//    '`DrinkWineAlcohol`  INTEGER,' + sLineBreak +
//    '`AbilAlcohol`  INTEGER,' + sLineBreak +
//    '`AbilMaxAlcohol`  INTEGER,' + sLineBreak +
//    '`AbilDrinkValue`  INTEGER,' + sLineBreak +
//    '`AbilMedicineLevel`  INTEGER,' + sLineBreak +
//    '`AbilMedicineValue`  INTEGER,' + sLineBreak +
//    '`AbilMaxMedicineValue`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`),' + sLineBreak +
//    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//    'CREATE TABLE `HeroGodBlessState` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Index`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `Index`),' + sLineBreak +
//    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroMagic` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`MagicType`  INTEGER NOT NULL,' + sLineBreak +
//    '`MagicIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`MagicID`  INTEGER,' + sLineBreak +
//    '`MagicAttr`  INTEGER,' + sLineBreak +
//    '`MagicLevel`  INTEGER,' + sLineBreak +
//    '`MagicNewLevel`  INTEGER,' + sLineBreak +
//    '`MagicKey`  INTEGER,' + sLineBreak +
//    '`MagicTranPoint`  INTEGER,' + sLineBreak +
//    '`MagicIsUseItemAdd`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `MagicType`, `MagicIndex`),' + sLineBreak +
//    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroQuestFlag` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Index`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `Index`),' + sLineBreak +
//    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroStatusTime` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`Index`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `Index`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroItems` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`MakeIndex`  INTEGER,' + sLineBreak +
//    '`DBIndex`  INTEGER,' + sLineBreak +
//    '`Name`  VARCHAR(30),' + sLineBreak +
//    '`Dura`  INTEGER,' + sLineBreak +
//    '`DuraMax`  INTEGER,' + sLineBreak +
//    '`HeroM2DressEffect`  INTEGER,' + sLineBreak +
//    '`UpgradeCount`  INTEGER,' + sLineBreak +
//    '`IsStartTime`  INTEGER,' + sLineBreak +
//    '`LimitTime`  INTEGER,' + sLineBreak +
//    '`HeroM2Light`  INTEGER,' + sLineBreak +
//    '`Color`  INTEGER,' + sLineBreak +
//    '`IsBind`  INTEGER,' + sLineBreak +
//    '`BindOption`  INTEGER,' + sLineBreak +
//    '`Effect`  INTEGER,' + sLineBreak +
//    '`NewLooks`  INTEGER,' + sLineBreak +
//    '`NewShape`  INTEGER,' + sLineBreak +
//    '`FluteCount`  INTEGER,' + sLineBreak +
//    '`PropertyText`  VARCHAR(64),' + sLineBreak +
//    '`PropertyTextColor`  INTEGER,' + sLineBreak +
//    '`ItemFrom`  INTEGER,' + sLineBreak +
//    '`ItemFromMap`  VARCHAR(30),' + sLineBreak +
//    '`ItemFromMon`  VARCHAR(40),' + sLineBreak +
//    '`ItemFromMaker`  VARCHAR(40),' + sLineBreak +
//    '`ItemFromDate`  REAL,' + sLineBreak +
//    '`InsuranceCount`  INTEGER,' + sLineBreak +
//    '`NewExpand3`  INTEGER, DEFAULT 0' + sLineBreak +
//    '`NewExpand4`  INTEGER, DEFAULT 0' + sLineBreak +
//    
//    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`),' + sLineBreak +
//    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`),' + sLineBreak +
//    'CONSTRAINT `uk_HeroItem` UNIQUE (`HeroID`, `ItemType`, `ItemIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroItemElementAdd` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//    'CREATE TABLE `HeroItemAddDataByte` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//    'CREATE TABLE `HeroItemAddDataInt` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//    'CREATE TABLE `HeroItemAddDataText` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  VARCHAR(30),' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroItemFlute` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    '`OverlapCount`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroItemProgress` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`IsOpen`  INTEGER,' + sLineBreak +
//    '`NameColor`  INTEGER,' + sLineBreak +
//    '`Count`  INTEGER,' + sLineBreak +
//    '`ShowType`  INTEGER,' + sLineBreak +
//    '`Max`  INTEGER,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    '`Level`  INTEGER,' + sLineBreak +
//    '`Name`  VARCHAR(31),' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroItemProperty` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Color`  INTEGER,' + sLineBreak +
//    '`BindType`  INTEGER,' + sLineBreak +
//    '`ShowFlag`  INTEGER,' + sLineBreak +
//    '`IsPercent`  INTEGER,' + sLineBreak +
//    '`HintModule`  INTEGER,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    '`Value2`  INTEGER,' + sLineBreak +
//    '`Value3`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroItemValueAdd` (' + sLineBreak +
//    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
//    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
//    '`Value`  INTEGER,' + sLineBreak +
//    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
//    ');' + sLineBreak +
//
//
//    'CREATE TABLE `HeroSkillPower` (' + sLineBreak +
//    '  `HeroID`  INTEGER NOT NULL,' + sLineBreak +
//    '  `SkillID`  INTEGER NOT NULL,' + sLineBreak +
//    '  `HumanAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  `HumanAttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  `MonAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  `MonAttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  `DefensePercent`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  `DefenseValue`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  `RemainingTime`  INTEGER DEFAULT 0,' + sLineBreak +
//    '  PRIMARY KEY (`HeroID`, `SkillID`)' + sLineBreak +
//    ');' + sLineBreak +
//
//    MYSQL_CEATE_DB_CONSTANT +
//
//    'REPLACE INTO db_constant(ConstName, ConstValue) Values("role_version", ' + MYSQL_DBVERSION + ')';
//         
  //////////////////////////////////////////////////////////////////////////////////

  MYSQL_CREATE_HERO_ROLEDATA_TABLES =
    'drop table IF EXISTS `HeroAbil`;' + sLineBreak +
    'drop table IF EXISTS `HeroAbilNG`;' + sLineBreak +
    'drop table IF EXISTS `HeroAbilNpcAdd`;' + sLineBreak +
    'drop table IF EXISTS `HeroAbilWine`;' + sLineBreak +
    'drop table IF EXISTS `HeroGodBlessState`;' + sLineBreak +
    'drop table IF EXISTS `HeroMagic`;' + sLineBreak +
    'drop table IF EXISTS `HeroQuestFlag`;' + sLineBreak +
    'drop table IF EXISTS `HeroStatusTime`;' + sLineBreak +
    'drop table IF EXISTS `HeroItems`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemElementAdd`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemAddDataByte`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemAddDataInt`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemAddDataText`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemFlute`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemProgress`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemProperty`;' + sLineBreak +
    'drop table IF EXISTS `HeroItemValueAdd`;' + sLineBreak +
    'drop table IF EXISTS `HeroSkillPower`;' + sLineBreak +
    'drop table IF EXISTS `Hero`;' + sLineBreak +
	
    'CREATE TABLE `Hero` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL AUTO_INCREMENT,' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`HeroName`  VARCHAR(14) NOT NULL,' + sLineBreak +
    '`IsDelete`  INTEGER,' + sLineBreak +
    '`CreateDate`  INTEGER,' + sLineBreak +
    '`Sex`  INTEGER,' + sLineBreak +
    '`Job`  INTEGER,' + sLineBreak +
    '`Hair`  INTEGER,' + sLineBreak +
    '`Status`  INTEGER,' + sLineBreak +
    '`Dir`  INTEGER,' + sLineBreak +
    '`Level`  INTEGER,' + sLineBreak +
    '`ReLevel`  INTEGER,' + sLineBreak +
    '`LoyalPoint`  REAL, ' + sLineBreak +
    '`Map`  VARCHAR(30),' + sLineBreak +
    '`X`  INTEGER,' + sLineBreak +
    '`Y`  INTEGER,' + sLineBreak +
    '`HomeMap`  VARCHAR(30),' + sLineBreak +
    '`HomeX`  INTEGER,' + sLineBreak +
    '`HomeY`  INTEGER,' + sLineBreak +
    '`AttackMode`  INTEGER,' + sLineBreak +
    '`CreditPoint`  INTEGER,' + sLineBreak +
    '`PKPoint`  INTEGER,' + sLineBreak +
    '`IncHP`  INTEGER,' + sLineBreak +
    '`IncMP`  INTEGER,' + sLineBreak +
    '`IncHP2`  INTEGER,' + sLineBreak +
    '`FightZoneDieCount`  INTEGER,' + sLineBreak +
    '`BodyLuck`  REAL,' + sLineBreak +
    '`HungerStatus`  INTEGER,' + sLineBreak +
    '`RevivalTime`  INTEGER,' + sLineBreak +
    '`IsSaveKillMonExpRate`  INTEGER,' + sLineBreak +
    '`KillMonExpRate`  INTEGER,' + sLineBreak +
    '`KillMonExpRateTime`  INTEGER,' + sLineBreak +
    '`IsSavePowerRate`  INTEGER,' + sLineBreak +
    '`PowerRate`  INTEGER,' + sLineBreak +
    '`PowerRateTime`  INTEGER,' + sLineBreak +
    '`IsAttackMonSavePowerRate`  INTEGER,' + sLineBreak +
    '`AttackMonPowerRate`  INTEGER,' + sLineBreak +
    '`AttackMonPowerRateTime`  INTEGER,' + sLineBreak +
    '`IsSaveKillMonBurstRate`  INTEGER,' + sLineBreak +
    '`KillMonBurstRate`  INTEGER,' + sLineBreak +
    '`KillMonBurstRateTime`  INTEGER,' + sLineBreak +
    '`JewelryBoxStatus`  INTEGER,' + sLineBreak +
    '`IsShowFashion`  INTEGER,' + sLineBreak +
    '`IsShowGodBless`  INTEGER,' + sLineBreak +
    '`ActiveFengHao`  INTEGER,' + sLineBreak +

    'PRIMARY KEY (`HeroID`),' + sLineBreak +
    'UNIQUE INDEX `idx_HeroName`(`HeroName`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroAbil` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`AC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MAC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MAC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`DC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`DC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`SC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`SC2`  INTEGER,' + sLineBreak +
    '`HP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxHP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxMP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`Exp`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxExp`  INTEGER DEFAULT 0,' + sLineBreak +
    '`Weight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`WearWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxWearWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`HandWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxHandWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY (`HeroID`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroAbilNG` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`IsTrainingNG`  INTEGER,' + sLineBreak +
    '`IsTrainingXF`  INTEGER,' + sLineBreak +
    '`AbilNGLevel`  INTEGER,' + sLineBreak +
    '`AbilNGValue`  INTEGER,' + sLineBreak +
    '`AbilNGMaxValue`  INTEGER,' + sLineBreak +
    '`AbilNGExp`  INTEGER,' + sLineBreak +
    '`AbilNGMaxExp`  INTEGER,' + sLineBreak +
    '`ContinuousMagicOrder1`  INTEGER,' + sLineBreak +
    '`ContinuousMagicOrder2`  INTEGER,' + sLineBreak +
    '`ContinuousMagicOrder3`  INTEGER,' + sLineBreak +
    '`IsOpenLastContinuous`  INTEGER,' + sLineBreak +
    '`LastContinuousMagicOrder`  INTEGER,' + sLineBreak +
    '`Meridians1Level`  INTEGER,' + sLineBreak +
    '`Meridians1BlastHitRate1`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians2Level`  INTEGER,' + sLineBreak +
    '`Meridians2BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians3Level`  INTEGER,' + sLineBreak +
    '`Meridians3BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians4Level`  INTEGER,' + sLineBreak +
    '`Meridians4BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians5Level`  INTEGER,' + sLineBreak +
    '`Meridians5BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints5`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroAbilNpcAdd` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroAbilWine` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`IsDrinkWineDrunk`  INTEGER,' + sLineBreak +
    '`DrinkWineQuality`  INTEGER,' + sLineBreak +
    '`DrinkWineAlcohol`  INTEGER,' + sLineBreak +
    '`AbilAlcohol`  INTEGER,' + sLineBreak +
    '`AbilMaxAlcohol`  INTEGER,' + sLineBreak +
    '`AbilDrinkValue`  INTEGER,' + sLineBreak +
    '`AbilMedicineLevel`  INTEGER,' + sLineBreak +
    '`AbilMedicineValue`  INTEGER,' + sLineBreak +
    '`AbilMaxMedicineValue`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HeroGodBlessState` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroMagic` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicType`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicID`  INTEGER,' + sLineBreak +
    '`MagicAttr`  INTEGER,' + sLineBreak +
    '`MagicLevel`  INTEGER,' + sLineBreak +
    '`MagicNewLevel`  INTEGER,' + sLineBreak +
    '`MagicKey`  INTEGER,' + sLineBreak +
    '`MagicTranPoint`  INTEGER,' + sLineBreak +
    '`MagicIsUseItemAdd`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `MagicType`, `MagicIndex`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroQuestFlag` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroStatusTime` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `Index`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroItems` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`MakeIndex`  INTEGER,' + sLineBreak +
    '`DBIndex`  INTEGER,' + sLineBreak +
    '`Name`  VARCHAR(30),' + sLineBreak +
    '`Dura`  INTEGER,' + sLineBreak +
    '`DuraMax`  INTEGER,' + sLineBreak +
    '`HeroM2DressEffect`  INTEGER,' + sLineBreak +
    '`UpgradeCount`  INTEGER,' + sLineBreak +
    '`IsStartTime`  INTEGER,' + sLineBreak +
    '`LimitTime`  INTEGER,' + sLineBreak +
    '`HeroM2Light`  INTEGER,' + sLineBreak +
    '`Color`  INTEGER,' + sLineBreak +
    '`IsBind`  INTEGER,' + sLineBreak +
    '`BindOption`  INTEGER,' + sLineBreak +
    '`Effect`  INTEGER,' + sLineBreak +
    '`NewLooks`  INTEGER,' + sLineBreak +
    '`NewShape`  INTEGER,' + sLineBreak +
    '`FluteCount`  INTEGER,' + sLineBreak +
    '`PropertyText`  VARCHAR(64),' + sLineBreak +
    '`PropertyTextColor`  INTEGER,' + sLineBreak +
    '`ItemFrom`  INTEGER,' + sLineBreak +
    '`ItemFromMap`  VARCHAR(30),' + sLineBreak +
    '`ItemFromMon`  VARCHAR(40),' + sLineBreak +
    '`ItemFromMaker`  VARCHAR(40),' + sLineBreak +
    '`ItemFromDate`  REAL,' + sLineBreak +
    '`InsuranceCount`  INTEGER,' + sLineBreak +
    '`NewExpand3`  INTEGER DEFAULT 0,' + sLineBreak +
    '`NewExpand4`  INTEGER DEFAULT 0,' + sLineBreak +
    
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`),' + sLineBreak +
    'FOREIGN KEY (`HeroID`) REFERENCES `Hero` (`HeroID`),' + sLineBreak +
    'CONSTRAINT `uk_HeroItem` UNIQUE (`HeroID`, `ItemType`, `ItemIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroItemElementAdd` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HeroItemAddDataByte` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HeroItemAddDataInt` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HeroItemAddDataText` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  VARCHAR(30),' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroItemFlute` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`OverlapCount`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroItemProgress` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`IsOpen`  INTEGER,' + sLineBreak +
    '`NameColor`  INTEGER,' + sLineBreak +
    '`Count`  INTEGER,' + sLineBreak +
    '`ShowType`  INTEGER,' + sLineBreak +
    '`Max`  INTEGER,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`Level`  INTEGER,' + sLineBreak +
    '`Name`  VARCHAR(31),' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroItemProperty` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Color`  INTEGER,' + sLineBreak +
    '`BindType`  INTEGER,' + sLineBreak +
    '`ShowFlag`  INTEGER,' + sLineBreak +
    '`IsPercent`  INTEGER,' + sLineBreak +
    '`HintModule`  INTEGER,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`Value2`  INTEGER,' + sLineBreak +
    '`Value3`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroItemValueAdd` (' + sLineBreak +
    '`HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HeroID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HeroSkillPower` (' + sLineBreak +
    '  `HeroID`  INTEGER NOT NULL,' + sLineBreak +
    '  `SkillID`  INTEGER NOT NULL,' + sLineBreak +
    '  `HumanAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `HumanAttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `MonAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `MonAttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `DefensePercent`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `DefenseValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `RemainingTime`  INTEGER DEFAULT 0,' + sLineBreak +
    '  PRIMARY KEY (`HeroID`, `SkillID`)' + sLineBreak +
    ');' + sLineBreak +
    
    MYSQL_CEATE_DB_CONSTANT +

    'REPLACE INTO db_constant(ConstName, ConstValue) Values("role_version", ' + MYSQL_DBVERSION + ')';

  //////////////////////////////////////////////////////////////////////////////////

  MYSQL_CREATE_HUMAN_ROLEDATA_TABLES =
    'drop table IF EXISTS `Human`;' + sLineBreak +
                        
       
    'CREATE TABLE `Human` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL AUTO_INCREMENT,' + sLineBreak +
    '`Account`  varchar(10) NOT NULL,' + sLineBreak +
    '`HumanName`  varchar(14) NOT NULL,' + sLineBreak +
    '`IsDelete`  INTEGER,' + sLineBreak +
    '`IsSelect`  INTEGER,' + sLineBreak +
    '`CreateDate`  INTEGER,' + sLineBreak +
    '`LoginDate`  INTEGER,' + sLineBreak +
    '`Sex`  INTEGER,' + sLineBreak +
    '`Job`  INTEGER,' + sLineBreak +
    '`Hair`  INTEGER,' + sLineBreak +
    '`Dir`  INTEGER,' + sLineBreak +
    '`Level`  INTEGER DEFAULT 0,' + sLineBreak +
    '`ReLevel`  INTEGER DEFAULT 0,' + sLineBreak +
    '`Map`  varchar(30),' + sLineBreak +
    '`X`  INTEGER,' + sLineBreak +
    '`Y`  INTEGER,' + sLineBreak +
    '`HomeMap`  varchar(30),' + sLineBreak +
    '`HomeX`  INTEGER,' + sLineBreak +
    '`HomeY`  INTEGER,' + sLineBreak +
    '`AttackMode`  INTEGER,' + sLineBreak +
    '`StoragePassword`  varchar(7),' + sLineBreak +
    '`CreditPoint`  INTEGER,' + sLineBreak +
    '`Gold`  INTEGER,' + sLineBreak +
    '`GameGold`  INTEGER,' + sLineBreak +
    '`GamePoint`  INTEGER,' + sLineBreak +
    '`GameDiamond`  INTEGER,' + sLineBreak +
    '`GameGird`  INTEGER,' + sLineBreak +
    '`GameGoldEx`  INTEGER,' + sLineBreak +
    '`GameGlory`  INTEGER,' + sLineBreak +
    '`PKPoint`  INTEGER,' + sLineBreak +
    '`PayMentPoint`  INTEGER,' + sLineBreak +

    '`MemberType`  INTEGER,' + sLineBreak +
    '`MemberLevel`  INTEGER,' + sLineBreak +

    '`IsMaster`  INTEGER,' + sLineBreak +
    '`MasterName`  varchar(14),' + sLineBreak +
    '`MasterCount`  INTEGER,' + sLineBreak +
    '`MarryCount`  INTEGER,' + sLineBreak +
    '`DearName`  varchar(14),' + sLineBreak +
    '`IncHP`  INTEGER,' + sLineBreak +
    '`IncMP`  INTEGER,' + sLineBreak +
    '`IncHP2`  INTEGER,' + sLineBreak +
    '`FightZoneDieCount`  INTEGER,' + sLineBreak +
    '`BodyLuck`  REAL,' + sLineBreak +
    '`Contribution`  INTEGER,' + sLineBreak +
    '`HungerStatus`  INTEGER,' + sLineBreak +
    '`KickCount`  INTEGER,' + sLineBreak +
    '`IsLockLogin`  INTEGER,' + sLineBreak +
    '`IsAllowGroup`  INTEGER,' + sLineBreak +
    '`IsAllowGroupRecall`  INTEGER,' + sLineBreak +
    '`GroupRecallTime`  INTEGER,' + sLineBreak +
    '`IsAllowGuildReCall`  INTEGER,' + sLineBreak +
    '`IsDisableTrading`  INTEGER,' + sLineBreak +
    '`IsDisableInviteHorseRiding`  INTEGER,' + sLineBreak +
    '`IsGameGoldTrading`  INTEGER,' + sLineBreak +
    '`IsNewServer`  INTEGER,' + sLineBreak +


    '`IsFilterGlobalDropItemMsg`  INTEGER,' + sLineBreak +
    '`IsFilterGlobalCenterMsg`  INTEGER,' + sLineBreak +
    '`IsFilterGolbalSendMsg`  INTEGER,' + sLineBreak +

    '`IsFixedHero`  INTEGER,' + sLineBreak +
    '`IsStorageHero`  INTEGER,' + sLineBreak +
    '`IsStorageDeputyHero`  INTEGER,' + sLineBreak +
    '`HeroName`  VARCHAR(14),' + sLineBreak +
    '`DeputyHeroName`  VARCHAR(14),' + sLineBreak +
    '`DeputyHeroJob`  INTEGER,' + sLineBreak +
    '`Nation`  INTEGER,' + sLineBreak +
    '`NationCredit`  INTEGER,' + sLineBreak +
    '`RevivalTime`  INTEGER,' + sLineBreak +
    '`InfinityStorageExtCount`  INTEGER,' + sLineBreak +
    '`IsSaveKillMonExpRate`  INTEGER,' + sLineBreak +
    '`KillMonExpRate`  INTEGER,' + sLineBreak +
    '`KillMonExpRateTime`  INTEGER,' + sLineBreak +
    '`IsSavePowerRate`  INTEGER,' + sLineBreak +
    '`PowerRate`  INTEGER,' + sLineBreak +
    '`PowerRateTime`  INTEGER,' + sLineBreak +
    '`IsAttackMonSavePowerRate`  INTEGER,' + sLineBreak +
    '`AttackMonPowerRate`  INTEGER,' + sLineBreak +
    '`AttackMonPowerRateTime`  INTEGER,' + sLineBreak +
    '`IsSaveKillMonBurstRate`  INTEGER,' + sLineBreak +
    '`KillMonBurstRate`  INTEGER,' + sLineBreak +
    '`KillMonBurstRateTime`  INTEGER,' + sLineBreak +
    '`FBCreateTime`  INTEGER,' + sLineBreak +
    '`JewelryBoxStatus`  INTEGER,' + sLineBreak +
    '`IsShowFashion`  INTEGER,' + sLineBreak +
    '`IsShowGodBless`  INTEGER,' + sLineBreak +
    '`ActiveFengHao`  INTEGER,' + sLineBreak +

    '`IsOpenStorage1` INTEGER,' + sLineBreak +
    '`IsOpenStorage2` INTEGER,' + sLineBreak +
    '`IsOpenStorage3` INTEGER,' + sLineBreak +

    //------------------------------------------------------------2019-05-12 添加新字段
    '`ExtBagPageCount`  INTEGER DEFAULT 0,' + sLineBreak +
    '`ExtBagOpenItemCount`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AddMaxWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    
    '`HighLevelKillMonFixExpTimeLeft`  INTEGER,' + sLineBreak +

    '`MobileNumber`  varchar(20) ,' + sLineBreak +
    '`IsMobileBind`  INTEGER,' + sLineBreak +
    '`MobileVerifyCode`  varchar(8),' + sLineBreak +
    '`MobileSendTick`  INTEGER,' + sLineBreak +
    '`MobileResendCount`  INTEGER,' + sLineBreak +

    '`ClearDayVarTime`  INTEGER DEFAULT 0,' + sLineBreak +

    'PRIMARY KEY (`HumanID`),' + sLineBreak +
    'UNIQUE INDEX `uk_HumanName`(`HumanName`),' + sLineBreak +
    'INDEX `idx_Account`(`Account`) USING BTREE,' + sLineBreak +
    'INDEX `idx_HumanName`(`HumanName`) USING BTREE' + sLineBreak +
    ');' + sLineBreak +
                 'CREATE TABLE `HumanItems` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`MakeIndex`  INTEGER,' + sLineBreak +
    '`DBIndex`  INTEGER,' + sLineBreak +
    '`Name`  VARCHAR(30),' + sLineBreak +
    '`Dura`  INTEGER,' + sLineBreak +
    '`DuraMax`  INTEGER,' + sLineBreak +
    '`HeroM2DressEffect`  INTEGER,' + sLineBreak +
    '`UpgradeCount`  INTEGER,' + sLineBreak +
    '`IsStartTime`  INTEGER,' + sLineBreak +
    '`LimitTime`  INTEGER,' + sLineBreak +
    '`HeroM2Light`  INTEGER,' + sLineBreak +
    '`Color`  INTEGER,' + sLineBreak +
    '`IsBind`  INTEGER,' + sLineBreak +
    '`BindOption`  INTEGER,' + sLineBreak +
    '`Effect`  INTEGER,' + sLineBreak +
    '`NewLooks`  INTEGER,' + sLineBreak +
    '`NewShape`  INTEGER,' + sLineBreak +
    '`FluteCount`  INTEGER,' + sLineBreak +
    '`PropertyText`  VARCHAR(64),' + sLineBreak +
    '`PropertyTextColor`  INTEGER,' + sLineBreak +
    '`ItemFrom`  INTEGER,' + sLineBreak +
    '`ItemFromMap`  VARCHAR(30),' + sLineBreak +
    '`ItemFromMon`  VARCHAR(40),' + sLineBreak +
    '`ItemFromMaker`  VARCHAR(40),' + sLineBreak +
    '`ItemFromDate`  REAL,' + sLineBreak +
    '`InsuranceCount`  INTEGER,' + sLineBreak +

    '`NewExpand3`  INTEGER DEFAULT 0,' + sLineBreak +
    '`NewExpand4`  INTEGER DEFAULT 0,' + sLineBreak +

    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +
    MYSQL_CEATE_DB_CONSTANT +


    'REPLACE INTO db_constant(ConstName, ConstValue) Values("role_version", ' + MYSQL_DBVERSION + ')';

  //////////////////////////////////////////////////////////////////////////////////

  MYSQL_TEST =

    'drop table IF EXISTS `HumanItems`;' + sLineBreak +

    'CREATE TABLE `HumanItems` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`MakeIndex`  INTEGER,' + sLineBreak +
    '`DBIndex`  INTEGER,' + sLineBreak +
    '`Name`  VARCHAR(30),' + sLineBreak +
    '`Dura`  INTEGER,' + sLineBreak +
    '`DuraMax`  INTEGER,' + sLineBreak +
    '`HeroM2DressEffect`  INTEGER,' + sLineBreak +
    '`UpgradeCount`  INTEGER,' + sLineBreak +
    '`IsStartTime`  INTEGER,' + sLineBreak +
    '`LimitTime`  INTEGER,' + sLineBreak +
    '`HeroM2Light`  INTEGER,' + sLineBreak +
    '`Color`  INTEGER,' + sLineBreak +
    '`IsBind`  INTEGER,' + sLineBreak +
    '`BindOption`  INTEGER,' + sLineBreak +
    '`Effect`  INTEGER,' + sLineBreak +
    '`NewLooks`  INTEGER,' + sLineBreak +
    '`NewShape`  INTEGER,' + sLineBreak +
    '`FluteCount`  INTEGER,' + sLineBreak +
    '`PropertyText`  VARCHAR(64),' + sLineBreak +
    '`PropertyTextColor`  INTEGER,' + sLineBreak +
    '`ItemFrom`  INTEGER,' + sLineBreak +
    '`ItemFromMap`  VARCHAR(30),' + sLineBreak +
    '`ItemFromMon`  VARCHAR(40),' + sLineBreak +
    '`ItemFromMaker`  VARCHAR(40),' + sLineBreak +
    '`ItemFromDate`  REAL,' + sLineBreak +
    '`InsuranceCount`  INTEGER,' + sLineBreak +
    '`NewExpand3`  INTEGER DEFAULT 0,' + sLineBreak +
    '`NewExpand4`  INTEGER DEFAULT 0,' + sLineBreak +

    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +

    MYSQL_CEATE_DB_CONSTANT +


    'REPLACE INTO db_constant(ConstName, ConstValue) Values("role_version", ' + MYSQL_DBVERSION + ')';
  //////////////////////////////////////////////////////////////////////////////////

  MYSQL_CREATE_HUMAN_2_ROLEDATA_TABLES =

    'drop table IF EXISTS `HumanItems`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemElementAdd`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemAddDataByte`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemAddDataInt`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemAddDataText`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemFlute`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemProgress`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemProperty`;' + sLineBreak +
    'drop table IF EXISTS `HumanItemValueAdd`;' + sLineBreak +
    'drop table IF EXISTS `HumanSkillPower`;' + sLineBreak +
	
    'CREATE TABLE `HumanItems` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`MakeIndex`  INTEGER,' + sLineBreak +
    '`DBIndex`  INTEGER,' + sLineBreak +
    '`Name`  VARCHAR(30),' + sLineBreak +
    '`Dura`  INTEGER,' + sLineBreak +
    '`DuraMax`  INTEGER,' + sLineBreak +
    '`HeroM2DressEffect`  INTEGER,' + sLineBreak +
    '`UpgradeCount`  INTEGER,' + sLineBreak +
    '`IsStartTime`  INTEGER,' + sLineBreak +
    '`LimitTime`  INTEGER,' + sLineBreak +
    '`HeroM2Light`  INTEGER,' + sLineBreak +
    '`Color`  INTEGER,' + sLineBreak +
    '`IsBind`  INTEGER,' + sLineBreak +
    '`BindOption`  INTEGER,' + sLineBreak +
    '`Effect`  INTEGER,' + sLineBreak +
    '`NewLooks`  INTEGER,' + sLineBreak +
    '`NewShape`  INTEGER,' + sLineBreak +
    '`FluteCount`  INTEGER,' + sLineBreak +
    '`PropertyText`  VARCHAR(64),' + sLineBreak +
    '`PropertyTextColor`  INTEGER,' + sLineBreak +
    '`ItemFrom`  INTEGER,' + sLineBreak +
    '`ItemFromMap`  VARCHAR(30),' + sLineBreak +
    '`ItemFromMon`  VARCHAR(40),' + sLineBreak +
    '`ItemFromMaker`  VARCHAR(40),' + sLineBreak +
    '`ItemFromDate`  REAL,' + sLineBreak +
    '`InsuranceCount`  INTEGER,' + sLineBreak +

    '`NewExpand3`  INTEGER DEFAULT 0,' + sLineBreak +
    '`NewExpand4`  INTEGER DEFAULT 0,' + sLineBreak +

    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanItemElementAdd` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HumanItemAddDataByte` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HumanItemAddDataInt` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'CREATE TABLE `HumanItemAddDataText` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  VARCHAR(30),' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanItemFlute` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`OverlapCount`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanItemProgress` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`IsOpen`  INTEGER,' + sLineBreak +
    '`NameColor`  INTEGER,' + sLineBreak +
    '`Count`  INTEGER,' + sLineBreak +
    '`ShowType`  INTEGER,' + sLineBreak +
    '`Max`  INTEGER,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`Level`  INTEGER,' + sLineBreak +
    '`Name`  VARCHAR(31),' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanItemProperty` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Color`  INTEGER,' + sLineBreak +
    '`BindType`  INTEGER,' + sLineBreak +
    '`ShowFlag`  INTEGER,' + sLineBreak +
    '`IsPercent`  INTEGER,' + sLineBreak +
    '`HintModule`  INTEGER,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`Value2`  INTEGER,' + sLineBreak +
    '`Value3`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanItemValueAdd` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanSkillPower` (' + sLineBreak +
    '  `HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '  `SkillID`  INTEGER NOT NULL,' + sLineBreak +
    '  `HumanAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `HumanAttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `MonAttackPercent`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `MonAttackValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `DefensePercent`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `DefenseValue`  INTEGER DEFAULT 0,' + sLineBreak +
    '  `RemainingTime`  INTEGER DEFAULT 0,' + sLineBreak +
    '  PRIMARY KEY (`HumanID`, `SkillID`)' + sLineBreak +
    ');' + sLineBreak +     


    MYSQL_CEATE_DB_CONSTANT +

    'REPLACE INTO db_constant(ConstName, ConstValue) Values("role_version", ' + MYSQL_DBVERSION + ')';
  //////////////////////////////////////////////////////////////////////////////////

  MYSQL_CREATE_HUMAN_1_ROLEDATA_TABLES =

    'drop table IF EXISTS `HumanAbil`;' + sLineBreak +
    'drop table IF EXISTS `HumanAbilNG`;' + sLineBreak +
    'drop table IF EXISTS `HumanAbilNpcAdd`;' + sLineBreak +
    'drop table IF EXISTS `HumanAbilWine`;' + sLineBreak +
    'drop table IF EXISTS `HumanGamePetData`;' + sLineBreak +
    'drop table IF EXISTS `HumanGodBlessState`;' + sLineBreak +
    'drop table IF EXISTS `HumanMagic`;' + sLineBreak +
    'drop table IF EXISTS `HumanMagicUseTick`;' + sLineBreak +
    'drop table IF EXISTS `HumanQuestFlag`;' + sLineBreak +
    'drop table IF EXISTS `HumanStatusTime`;' + sLineBreak +
    'drop table IF EXISTS `HumanVariableT`;' + sLineBreak +
    'drop table IF EXISTS `HumanVariableU`;' + sLineBreak +
    'drop table IF EXISTS `HumanVariableJ`;' + sLineBreak +

    'CREATE TABLE `HumanAbil` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`AC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MAC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MAC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`DC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`DC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MC2`  INTEGER DEFAULT 0,' + sLineBreak +
    '`SC1`  INTEGER DEFAULT 0,' + sLineBreak +
    '`SC2`  INTEGER,' + sLineBreak +
    '`HP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxHP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxMP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`Exp`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxExp`  INTEGER DEFAULT 0,' + sLineBreak +
    '`Weight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`WearWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxWearWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`HandWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`MaxHandWeight`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilPoint`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilDC`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilMC`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilSC`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilAC`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilMAC`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilHP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilMP`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilHit`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilSpeed`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AdjustAbilMaxRate`  INTEGER DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY (`HumanID`),' + sLineBreak +
    'CONSTRAINT `fk_HumanID` FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanAbilNG` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`IsTrainingNG`  INTEGER,' + sLineBreak +
    '`IsTrainingXF`  INTEGER,' + sLineBreak +
    '`AbilNGLevel`  INTEGER,' + sLineBreak +
    '`AbilNGValue`  INTEGER,' + sLineBreak +
    '`AbilNGMaxValue`  INTEGER,' + sLineBreak +
    '`AbilNGExp`  INTEGER,' + sLineBreak +
    '`AbilNGMaxExp`  INTEGER,' + sLineBreak +
    '`ContinuousMagicOrder1`  INTEGER,' + sLineBreak +
    '`ContinuousMagicOrder2`  INTEGER,' + sLineBreak +
    '`ContinuousMagicOrder3`  INTEGER,' + sLineBreak +
    '`IsOpenLastContinuous`  INTEGER,' + sLineBreak +
    '`LastContinuousMagicOrder`  INTEGER,' + sLineBreak +
    '`Meridians1Level`  INTEGER,' + sLineBreak +
    '`Meridians1BlastHitRate1`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians1Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians2Level`  INTEGER,' + sLineBreak +
    '`Meridians2BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians2Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians3Level`  INTEGER,' + sLineBreak +
    '`Meridians3BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians3Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians4Level`  INTEGER,' + sLineBreak +
    '`Meridians4BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians4Acupoints5`  INTEGER,' + sLineBreak +
    '`Meridians5Level`  INTEGER,' + sLineBreak +
    '`Meridians5BlastHitRate`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints1`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints2`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints3`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints4`  INTEGER,' + sLineBreak +
    '`Meridians5Acupoints5`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanAbilNpcAdd` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanAbilWine` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`IsDrinkedWine`  INTEGER,' + sLineBreak +
    '`IsDrinkWineDrunk`  INTEGER,' + sLineBreak +
    '`DrinkWineQuality`  INTEGER,' + sLineBreak +
    '`DrinkWineAlcohol`  INTEGER,' + sLineBreak +
    '`AbilAlcohol`  INTEGER,' + sLineBreak +
    '`AbilMaxAlcohol`  INTEGER,' + sLineBreak +
    '`AbilDrinkValue`  INTEGER,' + sLineBreak +
    '`AbilMedicineLevel`  INTEGER,' + sLineBreak +
    '`AbilMedicineValue`  INTEGER,' + sLineBreak +
    '`AbilMaxMedicineValue`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanGamePetData` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Name`  VARCHAR(60),' + sLineBreak +
    '`Level`  INTEGER,' + sLineBreak +
    '`HP`  INTEGER,' + sLineBreak +
    '`MP`  INTEGER,' + sLineBreak +
    '`Exp`  INTEGER,' + sLineBreak +
    '`Magic1`  INTEGER,' + sLineBreak +
    '`Magic2`  INTEGER,' + sLineBreak +
    '`Magic3`  INTEGER,' + sLineBreak +
    '`Magic4`  INTEGER,' + sLineBreak +
    '`Magic5`  INTEGER,' + sLineBreak +
    '`Magic6`  INTEGER,' + sLineBreak +
    '`Magic7`  INTEGER,' + sLineBreak +
    '`Magic8`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanGodBlessState` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanMagic` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicType`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicID`  INTEGER,' + sLineBreak +
    '`MagicAttr`  INTEGER,' + sLineBreak +
    '`MagicLevel`  INTEGER,' + sLineBreak +
    '`MagicNewLevel`  INTEGER,' + sLineBreak +
    '`MagicKey`  INTEGER,' + sLineBreak +
    '`MagicTranPoint`  INTEGER,' + sLineBreak +
    '`MagicIsUseItemAdd`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `MagicType`, `MagicIndex`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanMagicUseTick` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`MagicID`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `MagicID`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanQuestFlag` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanStatusTime` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanVariableT` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  VARCHAR(100),' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`) ' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanVariableU` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    'CREATE TABLE `HumanVariableJ` (' + sLineBreak +
    '`HumanID`  INTEGER NOT NULL,' + sLineBreak +
    '`Index`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY (`HumanID`, `Index`),' + sLineBreak +
    'FOREIGN KEY (`HumanID`) REFERENCES `Human` (`HumanID`)' + sLineBreak +
    ');' + sLineBreak +


    MYSQL_CEATE_DB_CONSTANT +

    'REPLACE INTO db_constant(ConstName, ConstValue) Values("role_version", ' + MYSQL_DBVERSION + ')';


const
  MYSQL_M2DBVERSION = '20200916';

const
  MYSQL_CREATE_M2DATA_TABLES =
    'drop table IF EXISTS `AuctionAttention`;' + sLineBreak +
    'CREATE TABLE `AuctionAttention` (' + sLineBreak +
    '`HumanName`  VARCHAR(14) NOT NULL,' + sLineBreak +
    '`AuctionID`  INTEGER NOT NULL,' + sLineBreak +
    '`time` datetime(0) NULL DEFAULT CURRENT_TIMESTAMP,' + sLineBreak +
    'PRIMARY KEY (`HumanName`, `AuctionID`)' + sLineBreak +
    ');' + sLineBreak +

    'drop table IF EXISTS `AuctionData`;' + sLineBreak +
    'CREATE TABLE `AuctionData` (' + sLineBreak +
    '`AuctionID`  INTEGER NOT NULL,' + sLineBreak +
    '`HumanName`   VARCHAR(14) NOT NULL,' + sLineBreak +
    '`ItemGroup`  INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    '`ItemColor`  INTEGER DEFAULT 0,' + sLineBreak +
    '`AddDateTime`  datetime(0) NULL DEFAULT CURRENT_TIMESTAMP,' + sLineBreak +
    '`AuctionTime`  INTEGER DEFAULT 0,' + sLineBreak +
    '`StartingPrice`  INTEGER DEFAULT 0,' + sLineBreak +
    '`SellingPrice`  INTEGER DEFAULT 0,' + sLineBreak +
    '`CurrencyType`  INTEGER,' + sLineBreak +
    '`LastBidder`   VARCHAR(14),' + sLineBreak +
    '`LastBidPrice`  INTEGER,' + sLineBreak +
    '`LastBidTime`  datetime(0) NULL,' + sLineBreak +
    '`TradingStatus`  INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    '`IsItemGive`  INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    '`IsTopmost`  INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    '`ItemDBName`  VARCHAR(30),' + sLineBreak +
    '`ItemName`  VARCHAR(30),' + sLineBreak +
    'PRIMARY KEY (AuctionID),' + sLineBreak +
    'INDEX `idx_AuctionDataItemGroup`(`ItemGroup`) USING BTREE,' + sLineBreak +
    'INDEX `idx_AuctionDataItemName`(`ItemName`) USING BTREE,' + sLineBreak +
    'INDEX `idx_AuctionDataItemDBName`(`ItemDBName`) USING BTREE' + sLineBreak +
    ');' + sLineBreak +

    'drop table IF EXISTS `ItemElementAdd`;' + sLineBreak +
    'CREATE TABLE `ItemElementAdd` (' + sLineBreak +
    '`ParentID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`ParentID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'drop table IF EXISTS `ItemAddDataByte`;' + sLineBreak +
    'CREATE TABLE `ItemAddDataByte` (' + sLineBreak +
    '`ParentID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`ParentID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'drop table IF EXISTS `ItemAddDataInt`;' + sLineBreak +
    'CREATE TABLE `ItemAddDataInt` (' + sLineBreak +
    '`ParentID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`ParentID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'drop table IF EXISTS `ItemAddDataText`;' + sLineBreak +
    'CREATE TABLE `ItemAddDataText` (' + sLineBreak +
    '`ParentID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  VARCHAR(30),' + sLineBreak +
    'PRIMARY KEY (`ParentID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'drop table IF EXISTS `ItemFlute`;' + sLineBreak +
    'CREATE TABLE `ItemFlute` (' + sLineBreak +
    '`ParentID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`OverlapCount`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`ParentID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'drop table IF EXISTS `ItemProgress`;' + sLineBreak +
    'CREATE TABLE `ItemProgress` (' + sLineBreak +
    '`ParentID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`IsOpen`  INTEGER,' + sLineBreak +
    '`NameColor`  INTEGER,' + sLineBreak +
    '`Count`  INTEGER,' + sLineBreak +
    '`ShowType`  INTEGER,' + sLineBreak +
    '`Max`  INTEGER,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`Level`  INTEGER,' + sLineBreak +
    '`Name`  VARCHAR(31),' + sLineBreak +
    'PRIMARY KEY (`ParentID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'drop table IF EXISTS `ItemProperty`;' + sLineBreak +
    'CREATE TABLE `ItemProperty` (' + sLineBreak +
    '`ParentID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Color`  INTEGER,' + sLineBreak +
    '`BindType`  INTEGER,' + sLineBreak +
    '`ShowFlag`  INTEGER,' + sLineBreak +
    '`IsPercent`  INTEGER,' + sLineBreak +
    '`HintModule`  INTEGER,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    '`Value2`  INTEGER,' + sLineBreak +
    '`Value3`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`ParentID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'drop table IF EXISTS `Items`;' + sLineBreak +
    'CREATE TABLE `Items` (' + sLineBreak +
    '`ParentID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`MakeIndex`  INTEGER,' + sLineBreak +
    '`DBIndex`  INTEGER,' + sLineBreak +
    '`Name`  VARCHAR(30),' + sLineBreak +
    '`Dura`  INTEGER,' + sLineBreak +
    '`DuraMax`  INTEGER,' + sLineBreak +
    '`HeroM2DressEffect`  INTEGER,' + sLineBreak +
    '`UpgradeCount`  INTEGER,' + sLineBreak +
    '`IsStartTime`  INTEGER,' + sLineBreak +
    '`LimitTime`  INTEGER,' + sLineBreak +
    '`HeroM2Light`  INTEGER,' + sLineBreak +
    '`Color`  INTEGER,' + sLineBreak +
    '`IsBind`  INTEGER,' + sLineBreak +
    '`BindOption`  INTEGER,' + sLineBreak +
    '`Effect`  INTEGER,' + sLineBreak +
    '`NewLooks`  INTEGER,' + sLineBreak +
    '`NewShape`  INTEGER,' + sLineBreak +
    '`FluteCount`  INTEGER,' + sLineBreak +
    '`PropertyText`  VARCHAR(64),' + sLineBreak +
    '`PropertyTextColor`  INTEGER,' + sLineBreak +
    '`ItemFrom`  INTEGER,' + sLineBreak +
    '`ItemFromMap`  varchar(30),' + sLineBreak +
    '`ItemFromMon`  varchar(40),' + sLineBreak +
    '`ItemFromMaker`  varchar(40),' + sLineBreak +
    '`ItemFromDate`  REAL,' + sLineBreak +
    '`InsuranceCount`  INTEGER,' + sLineBreak +
    '`NewExpand3`  INTEGER DEFAULT 0,' + sLineBreak +
    '`NewExpand4`  INTEGER DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY (`ParentID`, `ItemType`, `ItemIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'drop table IF EXISTS `ItemValueAdd`;' + sLineBreak +
    'CREATE TABLE `ItemValueAdd` (' + sLineBreak +
    '`ParentID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`ParentID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +


    'drop table IF EXISTS `StorageEx`;' + sLineBreak +
    'CREATE TABLE `StorageEx` (' + sLineBreak +
    'StorageID  INTEGER NOT NULL AUTO_INCREMENT,' + sLineBreak +
    '`HumanName`  VARCHAR(14) NOT NULL,' + sLineBreak +
    'PRIMARY KEY (StorageID),' + sLineBreak +
    'UNIQUE INDEX `uk_HumanName`(`HumanName`) USING BTREE' + sLineBreak +
    ');' + sLineBreak +


    'drop table IF EXISTS `UserShop`;' + sLineBreak +
    'CREATE TABLE `UserShop` (' + sLineBreak +
    '`ShopID` INTEGER NOT NULL AUTO_INCREMENT,' + sLineBreak +
    '`HumanName`  VARCHAR(14),' + sLineBreak +
    '`ShopName`  VARCHAR(14),' + sLineBreak +
    '`IsBusiness`  INTEGER,' + sLineBreak +
    '`CreateDate` datetime(0) NULL DEFAULT CURRENT_TIMESTAMP,' + sLineBreak +
    '`CareValue`  INTEGER DEFAULT 0,' + sLineBreak +
    'PRIMARY KEY (ShopID),' + sLineBreak +
    'UNIQUE INDEX `uk_UserShop_HumanName` (`HumanName`) USING BTREE,' + sLineBreak +
    'INDEX `idx_ShopName` (`ShopName`) USING BTREE,' + sLineBreak +
    'INDEX `idx_IsBusiness` (`IsBusiness`) USING BTREE' + sLineBreak +
    ');' + sLineBreak +


    'drop table IF EXISTS `UserShopItem`;' + sLineBreak +
    'CREATE TABLE `UserShopItem` (' + sLineBreak +
    '`ShopID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER,' + sLineBreak +
    '`CreateDate`  datetime(0) NULL DEFAULT CURRENT_TIMESTAMP,' + sLineBreak +
    '`IsAllowSell`  INTEGER,' + sLineBreak +
    '`MoneyType`  INTEGER,' + sLineBreak +
    '`ItemPrice`  INTEGER,' + sLineBreak +
    '`IsGetMoney`  INTEGER,' + sLineBreak +
    '`BuyerName`  VARCHAR(14),' + sLineBreak +
    '`ItemDBName`  VARCHAR(30),' + sLineBreak +
    '`ItemName`  VARCHAR(30) ,' + sLineBreak +
    'PRIMARY KEY (ShopID, ItemID),' + sLineBreak +
    'INDEX `idx_ItemPrices` (`ItemPrice`) USING BTREE,' + sLineBreak +
    'INDEX `idx_ItemName` (`ItemName`) USING BTREE,' + sLineBreak +
    'INDEX `idx_ItemDBName` (`ItemDBName`) USING BTREE' + sLineBreak +
    ');' + sLineBreak +

    MYSQL_CEATE_DB_CONSTANT + sLineBreak +

    'REPLACE INTO db_constant(ConstName, ConstValue) Values("m2data_version", ' + MYSQL_M2DBVERSION + ')';

implementation

end.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GXX.Core.Protocol;

// ============================================================================
// Grobal2.pas 类型部分 4：数组类型 + THumData / THeroData 巨型记录
// ============================================================================

// ---- 内联数组类型（对应 Delphi array[N] of T）----

[InlineArray(30)] public struct TUserItemArray30 { private TUserItem _e0; }
[InlineArray(206)] public struct TUserItemArray206 { private TUserItem _e0; }   // ALL_BAG_ITEM_COUNT
[InlineArray(196)] public struct TUserItemArray196 { private TUserItem _e0; }   // StorageItems
[InlineArray(60)] public struct TUserItemArray60 { private TUserItem _e0; }     // FengHaoItems
[InlineArray(12)] public struct TUserItemArray12 { private TUserItem _e0; }     // GodBlessItems
[InlineArray(6)] public struct TUserItemArray6 { private TUserItem _e0; }       // JewelryBoxItems
[InlineArray(40)] public struct TUserItemArray40 { private TUserItem _e0; }     // HeroBagItems
[InlineArray(30)] public struct TGamePetBagArray { private TUserItem _e0; }
[InlineArray(48)] public struct THumMagicArray48 { private THumMagic _e0; }
[InlineArray(6)] public struct THumMagicArray6 { private THumMagic _e0; }       // ContinuousMagics
[InlineArray(12)] public struct ByteArray12 { private byte _e0; }
[InlineArray(30)] public struct TGamePetDataArray30 { private TGamePetData _e0; }
[InlineArray(18)] public struct WordArray18 { private ushort _e0; }             // TStatusTime
[InlineArray(128)] public struct ByteArray128 { private byte _e0; }             // TQuestFlag
[InlineArray(500)] public struct IntArray500 { private int _e0; }
[InlineArray(30)] public struct IntArray30 { private int _e0; }
[InlineArray(300)] public struct IntArray300 { private int _e0; }
[InlineArray(550)] public struct TSaveNpcSkillPowerAddArray550 { private TSaveNpcSkillPowerAdd _e0; }
[InlineArray(30)] public struct TMoneyArray30 { private TMoney _e0; }
[InlineArray(8)] public struct BoolArray8 { private byte _e0; }
[InlineArray(24)] public struct BoolArray24 { private byte _e0; }
[InlineArray(12)] public struct BoolArray12 { private byte _e0; }
[InlineArray(7)] public struct BoolArray7 { private byte _e0; }
[InlineArray(6)] public struct BoolArray6 { private byte _e0; }
[InlineArray(4)] public struct BoolArray4 { private byte _e0; }
[InlineArray(4)] public struct ByteArray4 { private byte _e0; }
[InlineArray(3)] public struct ByteArray3 { private byte _e0; }
[InlineArray(10)] public struct WordArray10 { private ushort _e0; }
[InlineArray(8)] public struct ShortStr60Array8 { private ShortStr60 _e0; }
[InlineArray(12)] public struct ShortInt12x5 { private IntArray5 _e0; }         // SkillContinuousBlastHitRates [12][5]
[InlineArray(5)] public struct IntArray5 { private int _e0; }
[InlineArray(6)] public struct ByteArray6 { private byte _e0; }
[InlineArray(4)] public struct TItemCDTimeArray4 { private TItemCDTime _e0; }
[InlineArray(3)] public struct TItemCDTimeArray3 { private TItemCDTime _e0; }
[InlineArray(12)] public struct ShortStr30Array12 { private ShortStr30 _e0; }
[InlineArray(11)] public struct ShortStr30Array11 { private ShortStr30 _e0; }
[InlineArray(6)] public struct ShortStr30Array6 { private ShortStr30 _e0; }
[InlineArray(9)] public struct ShortStr60Array9 { private ShortStr60 _e0; }
[InlineArray(3)] public struct BoolArray3 { private byte _e0; }

// ---- 定长短字符串包装 ----

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ShortStr100
{
    public fixed byte b[101];
    public string Value { get { fixed (byte* p = b) return ShortStr.Get(p, 100); } set { fixed (byte* p = b) ShortStr.Set(p, 100, value); } }
}

[InlineArray(500)] public struct ShortStr100Array500 { private ShortStr100 _e0; }

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ShortStr60
{
    public fixed byte b[61];
    public string Value { get { fixed (byte* p = b) return ShortStr.Get(p, 60); } set { fixed (byte* p = b) ShortStr.Set(p, 60, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ShortStr30
{
    public fixed byte b[31];
    public string Value { get { fixed (byte* p = b) return ShortStr.Get(p, 30); } set { fixed (byte* p = b) ShortStr.Set(p, 30, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ShortStr21
{
    public fixed byte b[22];
    public string Value { get { fixed (byte* p = b) return ShortStr.Get(p, 21); } set { fixed (byte* p = b) ShortStr.Set(p, 21, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ShortStr51
{
    public fixed byte b[52];
    public string Value { get { fixed (byte* p = b) return ShortStr.Get(p, 51); } set { fixed (byte* p = b) ShortStr.Set(p, 51, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ShortStr16
{
    public fixed byte b[17];
    public string Value { get { fixed (byte* p = b) return ShortStr.Get(p, 16); } set { fixed (byte* p = b) ShortStr.Set(p, 16, value); } }
}

/// <summary>THumanUseItems 等 = array[0..29] of TUserItem。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TUserStateInfo
{
    public int RaceServer;
    public TFeature Feature;
    public fixed byte UserName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public int NAMECOLOR;
    public fixed byte GuildName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte GuildRankName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public TUseItemsArray UseItems;           // 30 x TClientItem
    public TJewelryBoxStatus JewelryBoxStatus;
    public TClientItemArray6 JewelryItems;
    public byte ShowGodBless;
    public ByteArray12 GodBlessItemsState;
    public TClientItemArray12 GodBlessItems;

    public string UserNameStr { get { fixed (byte* p = UserName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = UserName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string GuildNameStr { get { fixed (byte* p = GuildName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = GuildName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string GuildRankNameStr { get { fixed (byte* p = GuildRankName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = GuildRankName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[InlineArray(30)] public struct TUseItemsArray { private TClientItem _e0; }
[InlineArray(6)] public struct TClientItemArray6 { private TClientItem _e0; }
[InlineArray(12)] public struct TClientItemArray12 { private TClientItem _e0; }
[InlineArray(206)] public struct TClientItemArray206 { private TClientItem _e0; }

/// <summary>THumData：人物存档记录（M2 ↔ DBServer 传输体）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct THumData
{
    public fixed byte sAccount[Grobal2Const.ACCOUNT_LEN + 1];         // string[10]
    public fixed byte sChrName[Grobal2Const.ACTOR_NAME_LEN + 1];      // string[14]
    public byte btSex;
    public byte btJob;
    public byte btHair;
    public byte btDir;
    public byte btReLevel;
    public TOAbility Abil;                                            // +40
    public int nBonusPoint;
    public TNakedAbility BonusAbil;                                   // +20
    public fixed byte sCurMap[Grobal2Const.MAP_NAME_LEN + 1];
    public ushort wCurX;
    public ushort wCurY;
    public fixed byte sHomeMap[Grobal2Const.MAP_NAME_LEN + 1];
    public ushort wHomeX;
    public ushort wHomeY;
    public byte btAttackMode;
    public fixed byte sStoragePwd[8];                                 // string[7]
    public uint nGold;
    public uint nGameGold;
    public uint nGamePoint;
    public uint nGameDiamond;
    public uint nGameGird;
    public int nGameGoldEx;
    public int nGameGlory;
    public int nPKPoint;
    public int nPayMentPoint;
    public int nMemberType;
    public int nMemberLevel;
    public byte boMaster;
    public fixed byte sMasterName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public ushort wMasterCount;
    public byte btMarryCount;
    public fixed byte sDearName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public byte btIncHealth;
    public byte btIncSpell;
    public byte btIncHealing;
    public byte btFightZoneDieCount;
    public double dBodyLuck;
    public ushort wContribution;
    public int nHungerStatus;
    public int nKickCount;
    public byte boLockLogin;
    public byte boAllowGroup;
    public byte boAllowGroupReCall;
    public ushort wGroupRecallTime;
    public byte boAllowGuildReCall;
    public byte boDisableTrading;
    public byte boDisableInviteHorseRiding;
    public byte boGameGoldTrading;
    public byte boNewServer;
    public byte boFilterGlobalDropItemMsg;
    public byte boFilterGlobalCenterMsg;
    public byte boFilterGolbalSendMsg;
    public byte boFixedHero;
    public byte boStorageHero;
    public byte boStorageDeputyHero;
    public fixed byte sHeroName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sDeputyHeroName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public byte btDeputyHeroJob;
    public byte btNation;
    public int nNationCredit;
    public int nRevivalTime;
    public ushort dwInfinityStorageExtCount;
    public byte boTrainingNG;
    public byte boTrainingXF;
    public TAbilityNG AbilNG;
    public THumMagicArray48 NGMagics;
    public TMeridianArray5 Meridians;
    public byte boOpenLastContinuous;
    public byte btLastContinuousMagicOrder;
    public ByteArray3 ContinuousMagicOrder;
    public THumMagicArray6 ContinuousMagics;
    public byte boPleaseDrink;
    public int nDrinkWineQuality;
    public int nDrinkWineAlcohol;
    public byte boDrinkWineDrunk;
    public TAbilityAlcohol Alcohol;
    public byte boShowFashion;
    public byte btExtBagPageCount;
    public byte btExtBagOpenItemCount;
    public int dwAddMaxWeight;
    public TUserItemArray30 HumItems;
    public TUserItemArray206 BagItems;
    public TUserItemArray196 StorageItems;
    public BoolArray4 boStorageOpen;
    public THumMagicArray48 Magics;
    public TJewelryBoxStatus JewelryBoxStatus;
    public TUserItemArray6 JewelryBoxItems;
    public byte boShowGodBless;
    public ByteArray12 GodBlessItemsState;
    public TUserItemArray12 GodBlessItems;
    public sbyte nActiveFengHao;          // Shortint
    public TUserItemArray60 FengHaoItems;
    public TGamePetBagArray GamePetBagItems;
    public TGamePetDataArray30 GamePetData;
    public WordArray18 wStatusTimeArr;
    public ByteArray128 QuestFlag;
    public IntArray500 UValues;
    public ShortStr100Array500 TValues;
    public IntArray500 JValues;
    public ShortStr100Array500 ZValues;
    public IntArray30 AddSaveAbil;
    public IntArray300 CustomSkillUseTicks;
    public byte boSaveKillMonExpRate;
    public int nKillMonExpRate;
    public uint dwKillMonExpRateTime;
    public byte boAttackHumSavePowerRate;
    public int nAttackHumPowerRate;
    public uint dwAttackHumPowerRateTime;
    public byte boAttackMonSavePowerRate;
    public int nAttackMonPowerRate;
    public uint dwAttackMonPowerRateTime;
    public byte boSaveKillMonBurstRate;
    public int nKillMonBurstRate;
    public uint dwKillMonBurstRateTime;
    public uint dwFBCreateTime;
    public uint dwHighLevelKillMonFixExpTimeLeft;
    public fixed byte sMobileNumber[21];
    public byte boMobileBind;
    public fixed byte sMobileVerifyCode[9];
    public uint dwMobileSendTick;
    public int nMobileResendCount;
    public int nClearDayVarTime;
    public TSaveNpcSkillPowerAddArray550 NpcSkillPowerAdd;
    public TMoneyArray30 CustomMoney;

    // ---- 字符串属性 ----
    public string Account { get { fixed (byte* p = sAccount) return ShortStr.Get(p, Grobal2Const.ACCOUNT_LEN); } set { fixed (byte* p = sAccount) ShortStr.Set(p, Grobal2Const.ACCOUNT_LEN, value); } }
    public string ChrName { get { fixed (byte* p = sChrName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sChrName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string CurMap { get { fixed (byte* p = sCurMap) return ShortStr.Get(p, Grobal2Const.MAP_NAME_LEN); } set { fixed (byte* p = sCurMap) ShortStr.Set(p, Grobal2Const.MAP_NAME_LEN, value); } }
    public string HomeMap { get { fixed (byte* p = sHomeMap) return ShortStr.Get(p, Grobal2Const.MAP_NAME_LEN); } set { fixed (byte* p = sHomeMap) ShortStr.Set(p, Grobal2Const.MAP_NAME_LEN, value); } }
    public string StoragePwd { get { fixed (byte* p = sStoragePwd) return ShortStr.Get(p, 7); } set { fixed (byte* p = sStoragePwd) ShortStr.Set(p, 7, value); } }
    public string MasterName { get { fixed (byte* p = sMasterName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sMasterName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string DearName { get { fixed (byte* p = sDearName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sDearName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string HeroName { get { fixed (byte* p = sHeroName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sHeroName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string DeputyHeroName { get { fixed (byte* p = sDeputyHeroName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sDeputyHeroName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string MobileNumber { get { fixed (byte* p = sMobileNumber) return ShortStr.Get(p, 20); } set { fixed (byte* p = sMobileNumber) ShortStr.Set(p, 20, value); } }
    public string MobileVerifyCode { get { fixed (byte* p = sMobileVerifyCode) return ShortStr.Get(p, 8); } set { fixed (byte* p = sMobileVerifyCode) ShortStr.Set(p, 8, value); } }
}

[InlineArray(5)] public struct TMeridianArray5 { private TMeridian _e0; }

/// <summary>THeroData：英雄存档记录。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct THeroData
{
    public fixed byte sAccount[Grobal2Const.ACCOUNT_LEN + 1];
    public fixed byte sChrName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public byte btSex;
    public byte btHair;
    public byte btJob;
    public byte btStatus;
    public byte btDir;
    public fixed byte sCurMap[Grobal2Const.MAP_NAME_LEN + 1];
    public ushort wCurX;
    public ushort wCurY;
    public byte btAttackMode;
    public int nPKPoint;
    public fixed byte sMasterName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public byte btReLevel;
    public double rLoyalPoint;
    public byte btIncHealth;
    public byte btIncSpell;
    public byte btIncHealing;
    public byte btFightZoneDieCount;
    public double dBodyLuck;
    public int nHungerStatus;
    public int nRevivalTime;
    public byte boSaveKillMonExpRate;
    public int nKillMonExpRate;
    public uint dwKillMonExpRateTime;
    public byte boAttackHumSavePowerRate;
    public int nAttackHumPowerRate;
    public uint dwAttackHumPowerRateTime;
    public byte boAttackMonSavePowerRate;
    public int nAttackMonPowerRate;
    public uint dwAttackMonPowerRateTime;
    public byte boSaveKillMonBurstRate;
    public int nKillMonBurstRate;
    public uint dwKillMonBurstRateTime;
    public uint dwHighLevelKillMonFixExpTimeLeft;
    public TOAbility Abil;
    public byte boTrainingNG;
    public TAbilityNG AbilNG;
    public byte boOpenLastContinuous;
    public byte btLastContinuousMagicOrder;
    public ByteArray3 ContinuousMagicOrder;
    public byte boTrainingXF;
    public TMeridianArray5 Meridians;
    public int nDrinkWineQuality;
    public int nDrinkWineAlcohol;
    public byte boDrinkWineDrunk;
    public TAbilityAlcohol Alcohol;
    public byte boShowFashion;
    public byte boShowGodBless;
    public TJewelryBoxStatus JewelryBoxStatus;
    public sbyte nActiveFengHao;
    public TUserItemArray30 HumItems;
    public TUserItemArray206 BagItems;
    public TUserItemArray6 JewelryBoxItems;
    public ByteArray12 GodBlessItemsState;
    public TUserItemArray12 GodBlessItems;
    public TUserItemArray60 FengHaoItems;
    public THumMagicArray48 Magics;
    public THumMagicArray48 NGMagics;
    public THumMagicArray6 ContinuousMagics;
    public WordArray18 wStatusTimeArr;
    public ByteArray128 QuestFlag;
    public IntArray30 AddSaveAbil;
    public TSaveNpcSkillPowerAddArray550 NpcSkillPowerAdd;

    public string Account { get { fixed (byte* p = sAccount) return ShortStr.Get(p, Grobal2Const.ACCOUNT_LEN); } set { fixed (byte* p = sAccount) ShortStr.Set(p, Grobal2Const.ACCOUNT_LEN, value); } }
    public string ChrName { get { fixed (byte* p = sChrName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sChrName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string CurMap { get { fixed (byte* p = sCurMap) return ShortStr.Get(p, Grobal2Const.MAP_NAME_LEN); } set { fixed (byte* p = sCurMap) ShortStr.Set(p, Grobal2Const.MAP_NAME_LEN, value); } }
    public string MasterName { get { fixed (byte* p = sMasterName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sMasterName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

/// <summary>TDBLoadHuman：DB 请求加载人物。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDBLoadHuman
{
    public fixed byte sAccount[Grobal2Const.ACCOUNT_LEN + 1];
    public fixed byte sHumanName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sIPaddr[Grobal2Const.IP_ADDRESS_LEN + 1];
    public fixed byte sMachineID[33];
    public fixed byte sUserMachineID[33];
    public int nSessionID;
    public int nSoftVersionDate;
    public int nPayMent;
    public int nPayMode;
    public int nSocket;
    public int nGSocketIdx;
    public int nGateIdx;
    public int nKey;
    public byte boOffLine;
    public byte boReconnection;
    public int nClientWidth;
    public int nClientHeight;
    public int nClientBuildVer;
    public fixed byte sPromotionFlag[41];

    public string Account { get { fixed (byte* p = sAccount) return ShortStr.Get(p, Grobal2Const.ACCOUNT_LEN); } set { fixed (byte* p = sAccount) ShortStr.Set(p, Grobal2Const.ACCOUNT_LEN, value); } }
    public string HumanName { get { fixed (byte* p = sHumanName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sHumanName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string IPaddr { get { fixed (byte* p = sIPaddr) return ShortStr.Get(p, Grobal2Const.IP_ADDRESS_LEN); } set { fixed (byte* p = sIPaddr) ShortStr.Set(p, Grobal2Const.IP_ADDRESS_LEN, value); } }
    public string MachineID { get { fixed (byte* p = sMachineID) return ShortStr.Get(p, 32); } set { fixed (byte* p = sMachineID) ShortStr.Set(p, 32, value); } }
    public string UserMachineID { get { fixed (byte* p = sUserMachineID) return ShortStr.Get(p, 32); } set { fixed (byte* p = sUserMachineID) ShortStr.Set(p, 32, value); } }
    public string PromotionFlag { get { fixed (byte* p = sPromotionFlag) return ShortStr.Get(p, 40); } set { fixed (byte* p = sPromotionFlag) ShortStr.Set(p, 40, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDBLoadHero
{
    public fixed byte sAccount[Grobal2Const.ACCOUNT_LEN + 1];
    public fixed byte sHumanName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sHeroName1[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sHeroName2[Grobal2Const.ACTOR_NAME_LEN + 1];
    public byte btSex;
    public byte btJob;
    public byte btHair;
    public THeroDataType DataType;
    public long PlayObject;
    public long Npc;

    public string Account { get { fixed (byte* p = sAccount) return ShortStr.Get(p, Grobal2Const.ACCOUNT_LEN); } set { fixed (byte* p = sAccount) ShortStr.Set(p, Grobal2Const.ACCOUNT_LEN, value); } }
    public string HumanName { get { fixed (byte* p = sHumanName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sHumanName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string HeroName1 { get { fixed (byte* p = sHeroName1) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sHeroName1) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string HeroName2 { get { fixed (byte* p = sHeroName2) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sHeroName2) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDBLoadDummy
{
    public fixed byte sCharName[61];
    public fixed byte sMapName[Grobal2Const.MAP_NAME_LEN + 1];
    public int nX;
    public int nY;
    public long PlayObject;
    public long Npc;

    public string CharName { get { fixed (byte* p = sCharName) return ShortStr.Get(p, 60); } set { fixed (byte* p = sCharName) ShortStr.Set(p, 60, value); } }
    public string MapName { get { fixed (byte* p = sMapName) return ShortStr.Get(p, Grobal2Const.MAP_NAME_LEN); } set { fixed (byte* p = sMapName) ShortStr.Set(p, Grobal2Const.MAP_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDBRenameChr
{
    public fixed byte sAccount[Grobal2Const.ACCOUNT_LEN + 1];
    public fixed byte sOldName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sNewName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public byte boHuman;
    public long PlayObject;
    public long Npc;

    public string Account { get { fixed (byte* p = sAccount) return ShortStr.Get(p, Grobal2Const.ACCOUNT_LEN); } set { fixed (byte* p = sAccount) ShortStr.Set(p, Grobal2Const.ACCOUNT_LEN, value); } }
    public string OldName { get { fixed (byte* p = sOldName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sOldName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string NewName { get { fixed (byte* p = sNewName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sNewName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDBHumanChangeGold
{
    public TDBChangeGoldType ChangeType;
    public fixed byte sFromUser[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sChangGoldUser[Grobal2Const.ACTOR_NAME_LEN + 1];
    public int nGold;
    public long PlayObject;
    public long Npc;
    public fixed byte sCustomMoneyName[Grobal2Const.CUSTOMMONEY_NAME_LEN + 1];

    public string FromUser { get { fixed (byte* p = sFromUser) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sFromUser) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string ChangGoldUser { get { fixed (byte* p = sChangGoldUser) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sChangGoldUser) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string CustomMoneyName { get { fixed (byte* p = sCustomMoneyName) return ShortStr.Get(p, Grobal2Const.CUSTOMMONEY_NAME_LEN); } set { fixed (byte* p = sCustomMoneyName) ShortStr.Set(p, Grobal2Const.CUSTOMMONEY_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDBGetRankData
{
    public int nTabelPage;
    public int nTabelType;
    public int nPage;
    public fixed byte sChrName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public long PlayObject;
    public long Npc;

    public string ChrName { get { fixed (byte* p = sChrName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sChrName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDBQueryHumanInfo
{
    public TDBQueryHumanInfoType QueryType;
    public fixed byte HumanList[420];  // array[0..419] of AnsiChar
    public long PlayObject;
    public long Npc;
    public int nResultCount;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDBBuyPlayerInfo
{
    public fixed byte sSellAccount[Grobal2Const.ACCOUNT_LEN + 1];
    public fixed byte sSellPlayer[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sBuyAccount[Grobal2Const.ACCOUNT_LEN + 1];
    public fixed byte sBuyPlayer[Grobal2Const.ACTOR_NAME_LEN + 1];
    public int SellPricesType;
    public int SellPrices;
    public fixed byte sDelegater[Grobal2Const.ACTOR_NAME_LEN + 1];
    public long Buyer;

    public string SellAccount { get { fixed (byte* p = sSellAccount) return ShortStr.Get(p, Grobal2Const.ACCOUNT_LEN); } set { fixed (byte* p = sSellAccount) ShortStr.Set(p, Grobal2Const.ACCOUNT_LEN, value); } }
    public string SellPlayer { get { fixed (byte* p = sSellPlayer) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sSellPlayer) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string BuyAccount { get { fixed (byte* p = sBuyAccount) return ShortStr.Get(p, Grobal2Const.ACCOUNT_LEN); } set { fixed (byte* p = sBuyAccount) ShortStr.Set(p, Grobal2Const.ACCOUNT_LEN, value); } }
    public string BuyPlayer { get { fixed (byte* p = sBuyPlayer) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sBuyPlayer) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string Delegater { get { fixed (byte* p = sDelegater) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sDelegater) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDBSellPlayerInfo
{
    public fixed byte sSellAccount[Grobal2Const.ACCOUNT_LEN + 1];
    public fixed byte sSellPlayer[Grobal2Const.ACTOR_NAME_LEN + 1];
    public int SellPricesType;
    public int SellPrices;
    public fixed byte sDelegater[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sSetUser[Grobal2Const.ACTOR_NAME_LEN + 1];
    public long Seller;

    public string SellAccount { get { fixed (byte* p = sSellAccount) return ShortStr.Get(p, Grobal2Const.ACCOUNT_LEN); } set { fixed (byte* p = sSellAccount) ShortStr.Set(p, Grobal2Const.ACCOUNT_LEN, value); } }
    public string SellPlayer { get { fixed (byte* p = sSellPlayer) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sSellPlayer) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string Delegater { get { fixed (byte* p = sDelegater) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sDelegater) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string SetUser { get { fixed (byte* p = sSetUser) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sSetUser) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

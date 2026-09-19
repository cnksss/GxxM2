using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GXX.Core.Protocol;

// ============================================================================
// Grobal2.pas 类型部分 5：TClientConfig 巨型配置 + 自定义怪/技能/NPC 配置
// ============================================================================

/// <summary>TClientConfig：服务端发送给客户端的配置（巨大 packed 记录，1:1 字段顺序）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientConfig
{
    public byte btConfigDlgType;
    public byte boParalyCanRun;
    public byte boParalyCanWalk;
    public byte boParalyCanHit;
    public byte boParalyCanSpell;
    public byte boSkill43LockParaly;
    public byte btDieColor;
    public int nMagicItemRate;
    public byte boStartGameAuxiliary;
    public byte boCanOpenGameConfigDlg;
    public byte boNotCanUseClientConfig;
    public byte boGreenHintNewStyle;
    public byte boItemNewAbilAllowUse;
    public short nMoveSpeed;
    public short nAttackSpeed;
    public short nSpellSpeed;
    public byte DActionLogButton;
    public byte DBotMissionButton;
    public byte DBotFriendButton;
    public byte DControlHelpButton;
    public byte DBotRankButton;
    public byte DBotWhisperButton;
    public byte DOpenShopButton;
    public byte DBotUserShopButton;
    public byte DWebButton;
    public byte DOpenHeroButton;
    public byte DBotChallengeButton;
    public byte boShowGlory;
    public byte boShowHorseButton;
    public byte boShowHintWindowFrame;
    public byte boShowHintLines;
    public byte btHintWindowbackgroundColor;
    public byte btHintWindowbackgroundAlpha;
    public TByteRect HintWindowBorderWidth;
    public fixed byte sShowHintFontName[21];
    public byte btShowHintNameFontSize;
    public byte btShowHintNameFontBold;
    public byte btShowHintNameFontStroke;
    public byte btShowHintOtherFontSize;
    public byte btShowHintOtherFontBold;
    public byte btShowHintOtherFontStroke;
    public byte btSuspensionShowItem;
    public byte boEscCloseNPC;
    public byte boHintWithMouse;
    public byte boNpcDlgHintWithMouse;
    public byte boStateWindowsType;
    public byte boMoveItemShowID;
    public byte boShowItemForm;
    public byte boShowItemSellPrice;
    public byte boShowInsuranceInfo;
    public byte btShowItemFormColor;
    public byte btShowItemSellPriceColor;
    public byte btShowInsuranceInfoColor;
    public BoolArray7 boShowItemFromFields;
    public byte boShowHPLabel;
    public byte boShowNumberLable;
    public byte boShowJobAndLevel;
    public byte boFilterExp;
    public byte boShowGreenHint;
    public byte boShowUserName;
    public byte boOnlyShowCharName;
    public byte boShowMoveLable;
    public byte boAutoPickUpItem;
    public byte boNoCaton;
    public byte boDisableSelfStruck;
    public byte boSpeedSlow;
    public byte boMagicLock;
    public byte boPickupAll;
    public byte boAutoOrderItem;
    public byte boAutoCloseGroup;
    public byte boDuraWarning;
    public byte boNotNeedShift;
    public byte boHideGhost;
    public byte boHideHumEffect;
    public byte boHideWeaponEffect;
    public byte boShowMapDesc;
    public byte boShowHighlightHPLabel;
    public byte boAutoHideMode;
    public byte boSmart113Hit;
    public byte boHumAutoShield;
    public byte boHumStruckShield;
    public byte boSmartLongHit;
    public byte boSmartPosLongHit;
    public byte boSmartWalkLongHit;
    public byte boSmartWideHit;
    public byte boSmartFireHit;
    public byte boSmartSwordHit;
    public byte boSmartCrsHit;
    public byte boSmartTwnHit;
    public byte boBGMusic;
    public byte boRepeatBGMusic;
    public byte boShowMonName;
    public byte boShowNpcName;
    public byte boShowNpcHPLabel;
    public byte boShowNGLabel;
    public byte boNotParaly;
    public byte boHumManuallySnowWind;
    public byte boHumManuallyFireBoom;
    public byte boHumShootLightenLockTarget;
    public byte boHumManuallyMeteorShower;
    public byte boHumManuallyMove10Attack;
    public byte boHumManuallyFire;
    public byte boAutoCHangePoison;
    public byte boSmart66Hit;
    public byte boHeroAutoShield;
    public byte boAssistantHeroAutoShield;
    public byte boHeroDrug;
    public byte boAssistantHeroDrug;
    public byte boSceneShake;
    public byte boAutoDownHorse;
    public byte boDropOverLapItem;
    public byte boGetExpMsgAddChatBoardMsg;
    public byte boAddItemMsgXRightToLeft;
    public byte boAddItemMsgYBottomToTop;
    public byte boGetExpMsgXRightToLeft;
    public byte boGetExpMsgYBottomToTop;
    public byte boUpLevelMsgXRightToLeft;
    public byte boUpLevelMsgYBottomToTop;
    public byte boHeroAddItemMsgXRightToLeft;
    public byte boHeroAddItemMsgYBottomToTop;
    public byte boHeroGetExpMsgXRightToLeft;
    public byte boHeroGetExpMsgYBottomToTop;
    public byte boHeroUpLevelMsgXRightToLeft;
    public byte boHeroUpLevelMsgYBottomToTop;
    public byte boHideTabSheet2;
    public byte boHeroHideTabSheet2;
    public byte boHideTabSheet5;
    public byte boHeroHideTabSheet5;
    public byte boHideTabSheet7;
    public BoolArray24 NewAbilShowStateDlg;
    public byte btAddItemMsgFColor;
    public byte btAddItemMsgBColor;
    public short nAddItemMsgX;
    public short nAddItemMsgY;
    public byte btGetExpMsgFColor;
    public byte btGetExpMsgBColor;
    public short nGetExpMsgX;
    public short nGetExpMsgY;
    public byte btUpLevelMsgFColor;
    public byte btUpLevelMsgBColor;
    public short nUpLevelMsgX;
    public short nUpLevelMsgY;
    public byte btHeroAddItemMsgFColor;
    public byte btHeroAddItemMsgBColor;
    public short nHeroAddItemMsgX;
    public short nHeroAddItemMsgY;
    public byte btHeroGetExpMsgFColor;
    public byte btHeroGetExpMsgBColor;
    public short nHeroGetExpMsgX;
    public short nHeroGetExpMsgY;
    public byte btHeroUpLevelMsgFColor;
    public byte btHeroUpLevelMsgBColor;
    public short nHeroUpLevelMsgX;
    public short nHeroUpLevelMsgY;
    public byte boShowBagGameGoldSeparator;
    public byte boShowBagGameInfo;
    public byte boUseFindPath;
    public byte boUseOldSerialWindows;
    public byte boUseHeroM2Shop;
    public byte boMonStruckShowNumber;
    public byte boHumStruckShowNumber;
    public byte boCloseBookProtect;
    public byte boCloseLogoutProtect;
    public byte boBagRightkey;
    public byte boUseSuperMedica;
    public byte DMerchantDlgHelp;
    public BoolArray6 DBotFuncs;
    public byte boViewFog;
    public byte boCanStartRun;
    public BoolArray12 ClientConfigTabSheetVisibles;
    public ShortStr60Array9 UseSuperMedicaItemNames;
    public fixed byte sHomePage[200];       // string[199]
    public int nHumNeedMagicItem;
    public byte btSendWhisperMsgFColor;
    public byte btSendWhisperMsgBColor;
    public byte btRefreshGameGoldFColor;
    public byte btRefreshGameGoldBColor;
    public byte btShowWhisperFColor;
    public byte btShowWhisperBColor;
    public byte btCloseWhisperFColor;
    public byte btCloseWhisperBColor;
    public byte boNPCLabelFontStroke;
    public byte btNPCLabelNormalColor;
    public byte btNpcLabelMouseMoveColor;
    public byte btNpcLabelMouseDownColor;
    public byte boMinMapCloseRadar;
    public byte btMinMapType;
    public byte boMinMapUseFindPath;
    public byte boLoginShowMinMap;
    public uint dwMinMapFlagFlash;
    public byte btMinMapColorSelf;
    public byte btMinMapColorOther;
    public byte btMinMapColorNPC;
    public byte btMinMapColorGuard;
    public byte btMinMapColorMonster;
    public byte btMinMapColorHero;
    public byte btMinMapColorBoss;
    public byte boHideItemNameNum;
    public byte boShopHeadPic;
    public byte boSingleHint;
    public byte boHorseRun3Grid;
    public byte btPKLevel1NameColor;
    public uint dwMoveFrameTime;
    public uint dwHitFrameTime;
    public uint dwMagicHitFrameTime;
    public byte boGemUpgrade;
    public byte boShopGuiCanMove;
    public byte boNPCGuiCanMove;
    public ByteArray4 SkillContinueOrderBlastRates;
    public ShortInt12x5 SkillContinuousBlastHitRates;  // [12][5] of Integer
    public uint dwNpcButtonClickTime;
    public uint dwNpcActorClickTime;
    public uint dwPluginPickupTime;
    public uint dwPluginMinEatItemTime;
    public byte boShowDeputyHeroButton;
    public byte boShowBagArrange;
    public byte btPKLevel2NameColor;
    public byte boShiftSwitch;
    public uint dwIncSpeedDecInterval;
    public uint dwIncMoveSpeedDecInterval;
    public uint dwIncSpellSpeedDecInterval;
    public byte boKeyTabGetActor;
    public int nTitleFileIndex;
    public byte boHideTitle;
    public byte boContinueButchItem;
    public byte boAutoOpenSpell;
    public byte boDisableChartMemoSize;
    public byte boItemCompare;
    public byte boVolume;
    public byte boDisableDeal;
    public byte boShowUpdateStatus;
    public byte boSimpleShowActor;
    public byte boSimpleShowHumanDress;
    public byte boSimpleShowBB;
    public byte boSmartCustomHit1;
    public byte boSmartCustomHit2;
    public byte boSmartCustomHit3;
    public byte boSmartCustomHit4;
    public byte boSmartCustomHit5;
    public byte boSmartCustomHit6;
    public byte boSmartCustomHit7;
    public byte boSmartCustomHit8;
    public byte boShowDropValueItemEff;
    public byte boShowMagicShieldHP;
    public int nHumHPBarOffsetX;
    public int nHumHPBarOffsetY;
    public int nNpcHPBarOffsetX;
    public int nNpcHPBarOffsetY;
    public int nMonHPBarOffsetX;
    public int nMonHPBarOffsetY;
    public int nHumNameOffsetX;
    public int nHumNameOffsetY;
    public int nNpcNameOffsetX;
    public int nNpcNameOffsetY;
    public int nMonNameOffsetX;
    public int nMonNameOffsetY;
    public byte boHealthNumberText;
    public byte boBlastHitShowHealthNum;
    public short nHealthNumberOffsetX;
    public short nHealthNumberOffsetY;
    public int nHealthNumberMoveSpeed;
    public short nNewLeftGroupInfoOffsetX;
    public short nNewLeftGroupInfoOffsetY;
    public int nItemFluteStoneCount;
    public int nItemFluteStoneIdxCount;
    public int nItemFluteStoneOverlapCount;
    public byte boDisableRightClickFluteStone;
    public int nSayMsgMaxLen;
    public int btMaxHitPoint;
    public byte boMyShopGold;
    public byte boMyShopGameGold;
    public byte boMyShopGameDiamond;
    public byte boMyShopGameGird;
    public byte boMyShopGamePoint;
    public int nAuctionCurrencyTypeEx;
    public int nAuctionBroadcastCurrencyType;
    public int nAuctionBroadcastPrice;
    public byte boOpenAuctionItemColors;
    public ByteArray6 btAuctionItemColors;
    public byte boShowHeroShortKey;
    public short nShowHeroShortKeyX;
    public short nShowHeroShortKeyY;
    public byte btMagicFailMsgFColor;
    public byte btMagicFailMsgBColor;
    public byte btMagicOKMsgFColor;
    public byte btMagicOKMsgBColor;
    public short nMagicMsgX;
    public short nMagicMsgY;
    public byte boMagicMsgXRightToLeft;
    public byte boMagicMsgYBottomToTop;
    public byte boMagicMsgAddChatBoardMsg;
    public byte boFashionJewelryOpen;
    public byte boHelmetShowInBox;
    public byte boSkill31UseNewEffect;
    public byte boEnabledBuyShopItemGive;
    public int nGuildRankNameLen;
    public uint dwUserMoveTime;
    public fixed byte sJewelryBoxHint[21];
    public byte btMerchant273NameColor;
    public byte boHeroStateDlgNoMove;
    public byte nStarBaseNum;
    public byte nStarLineMaxCount;
    public byte boOpenNewGuild;
    public byte boDisableDuFuTakeArmRingL;
    public byte boOpenGamePet;
    public uint dwProtectValueCRC;
    public int nMaxInputStringLen;
    public byte boSlaveAlwaysShowName;
    public byte btMonsterShowLevel;
    public fixed byte sMonsterShowLevelFormat[31];
    public byte btMonStruckFrameDelayTime;
    public byte boTZSupportRenameItem;
    public byte boDescSupportRenamItem;
    public byte boNoRenameDescReadDefault;
    public byte boNoNeedFirDragon;
    public byte boHideItemEffect;
    public byte boAutoGroupAttack;
    public byte boAutoGroupNoAttackMon;
    public byte boMagicSetDir;
    public byte boEnableDoubleFireHitSkill;
    public byte boBagFastItemCompare;
    public byte boShowHPUnit;
    public byte boShowNewGroupInfo;
    public int nHMPDivDura;
    public byte boPetNoEntity;
    public byte boPetNoShowHPProgress;
    public byte boDisableWarrContinueHit;
    public uint nWarrContinueHitMinInterval;
    public WordArray10 ArrDisableWarrContinueHitIDs;
    public byte boAutoContinueAttack;
    public byte boHideActorIcons;
    public byte boHideMonsterIcons;
    public byte boDimFireEffect;
    public byte boAutoDetourPath;
    public byte boSimpleShowHumanWeapon;
    public fixed byte sNotEnoughNGPoint[48];
    public fixed byte sNotEnoughMPPoint[48];
    public byte btBagFastItemCompareMode;
    public int nEditionId;
    public int nAreaId;
    public int nBetterItemX;
    public int nBetterItemY;
    public int nSmallInfoX;
    public int nSmallInfoY;
    public int nJoyStickX;
    public int nJoyStickY;
    public int nJoyStickMaxX;
    public int nJoyStickMaxY;
    public int nSkillCtrX;
    public int nSkillCtrY;
    public byte boShowExSkillIcon;
    public byte btMapScale;
    public byte btGuiScale;
    public byte btMultiViewRange;
    public byte boShowMulitDlg;
    public byte boShowBetterItem;

    public string ShowHintFontName { get { fixed (byte* p = sShowHintFontName) return ShortStr.Get(p, 20); } set { fixed (byte* p = sShowHintFontName) ShortStr.Set(p, 20, value); } }
    public string HomePage { get { fixed (byte* p = sHomePage) return ShortStr.Get(p, 199); } set { fixed (byte* p = sHomePage) ShortStr.Set(p, 199, value); } }
    public string JewelryBoxHint { get { fixed (byte* p = sJewelryBoxHint) return ShortStr.Get(p, 20); } set { fixed (byte* p = sJewelryBoxHint) ShortStr.Set(p, 20, value); } }
    public string MonsterShowLevelFormat { get { fixed (byte* p = sMonsterShowLevelFormat) return ShortStr.Get(p, 30); } set { fixed (byte* p = sMonsterShowLevelFormat) ShortStr.Set(p, 30, value); } }
    public string NotEnoughNGPoint { get { fixed (byte* p = sNotEnoughNGPoint) return ShortStr.Get(p, 47); } set { fixed (byte* p = sNotEnoughNGPoint) ShortStr.Set(p, 47, value); } }
    public string NotEnoughMPPoint { get { fixed (byte* p = sNotEnoughMPPoint) return ShortStr.Get(p, 47); } set { fixed (byte* p = sNotEnoughMPPoint) ShortStr.Set(p, 47, value); } }
}

// ---- 自定义怪物相关 ----

public enum TMonsterClientActionType : byte { matStand, matWalk, matDefAttack, matStruck, matDie, matStoneRevive, matAttack1, matAttack2, matAttack3, matAttack4, matAttack5, matAttack6 }
public enum TMonsterType : byte { mtNormal, mtStoneMode, mtDigUP }
public enum TMoveOption : byte { moMoveNormal, moNoMove, moProtect }
public enum TCustomOperateMode : byte { momAttack, momProtect }
public enum TCustomAttackMode : byte { mamNear, mamFar }
public enum TCustomDrawMode : byte { mdmBlend, mdmNormal }
public enum TCustomDirCount : byte { mdcDir8, mdcDir16 }
public enum TCustomDirCalcType : byte { mdctNone, mdctNormal, mdctCenter }
public enum TCustomAttackTarget : byte { matSingle, matGroup, matLine, matSwordWide, matDir8, matDir16 }
public enum TCustomAttackPowerCalc : byte { mapcDC, mapcMC, mapcSC, mapcFromJob }
public enum TCustomDrawOrder : byte { mdoPriorSelf, mdoPriorMagic }
public enum TMonsterDrawOrder2 : byte { mdoSelf_Eff1_Eff2, mdoEff1_Self_Eff2, mdoEff1_Eff2_Self }
public enum TMonsterSoundType : byte { mstNormal, mstDigUP, mstAttack, mstStruck, mstDie, mstAttack1, mstAttack2, mstAttack3, mstAttack4, mstAttack5, mstAttack6 }

[InlineArray(11)] public struct TMonsterClientActionArray11 { private TMonsterClientAction _e0; }
[InlineArray(12)] public struct TMonsterClientActionTypeArray12 { private TMonsterClientAction _e0; }
[InlineArray(6)] public struct TClientAttackConfigArray6 { private TClientAttackConfig _e0; }
[InlineArray(4)] public struct TMagicClientConfigArray4 { private TMagicClientConfig _e0; }
[InlineArray(8)] public struct TNpcDirActionArray8 { private TNpcDirAction _e0; }
[InlineArray(7)] public struct TArrButtonGroupConfigArray7 { private TArrButtonGroupConfig _e0; }

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientBaseConfig
{
    public TCustomDrawMode DrawMode;
    public TCustomDrawMode DrawMode2;
    public TMonsterDrawOrder2 DrawOrder;
    public byte DieNoCalcDir;
    public int HPBgOffsetX;
    public int HPBgOffsetY;
    public int HPOffsetX;
    public int HPOffsetY;
    public int HPFile;
    public int HPStartIndex;
    public int HPTextOffsetX;
    public int HPTextOffsetY;
    public ShortStr30Array11 Sounds;  // array[TMonsterSoundType] of string[30]（11 项 × 31 字节 = 341）
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMonsterClientAction
{
    public TMonsterClientActionType ActionType;
    public short ActionFile;
    public short StartIndex;
    public ushort PlayCount;
    public ushort EmptyCount;
    public ushort PlayTime;
    public short EffectFile;
    public short EffectIndex;
    public short EffectFile2;
    public short EffectIndex2;
    public byte CalcDir;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientAttackConfig
{
    public short Fly_File;
    public short Fly_StartIndex;
    public ushort Fly_PlayCount;
    public ushort Fly_EmptyCount;
    public ushort Fly_PlayTime;
    public TCustomDrawMode Fly_DrawMode;
    public TCustomDirCount Fly_DirCount;
    public byte Fly_CalcDir;
    public byte Fly_LightRange;
    public short FlyEff_File;
    public short FlyEff_StartIndex;
    public TCustomDrawMode FlyEff_DrawMode;
    public short Self_File;
    public short Self_StartIndex;
    public ushort Self_PlayCount;
    public ushort Self_EmptyCount;
    public ushort Self_PlayTime;
    public TCustomDrawOrder Self_DrawOrder;
    public TCustomDrawMode Self_DrawMode;
    public TCustomDirCalcType Self_DirCalcType;
    public TCustomDirCount Self_DirCount;
    public byte Self_PlayDelayAction;
    public byte Self_LightRange;
    public short SelfKeep_File;
    public short SelfKeep_StartIndex;
    public short SelfKeep_StartIndex2;
    public ushort SelfKeep_PlayCount;
    public ushort SelfKeep_PlayTime;
    public TCustomDrawMode SelfKeep_DrawMode;
    public TCustomDrawMode SelfKeep_DrawMode2;
    public TCustomDrawOrder SelfKeep_DrawOrder;
    public ushort SelfKeep_KeepTime;
    public short Explosion_File;
    public short Explosion_StartIndex;
    public short Explosion_StartIndex2;
    public ushort Explosion_PlayCount;
    public TCustomDrawMode Explosion_DrawMode;
    public TCustomDrawMode Explosion_DrawMode2;
    public ushort Explosion_PlayTime;
    public byte Explosion_LockDraw;
    public byte Explosion_LightRange;
    public byte Explosion_KeepPlay;
    public ushort Explosion_KeepTime;
    public byte Explosion_KeepAttackRange;
    public byte Explosion_KeepMultiPlay;
    public byte Explosion_KeepAttackInterval;
    public byte Explosion_KeepLightRange;
    public short Target_File;
    public short Target_StartIndex;
    public short Target_StartIndex2;
    public ushort Target_PlayCount;
    public ushort Target_PlayTime;
    public TCustomDrawMode Target_DrawMode;
    public TCustomDrawMode Target_DrawMode2;
    public byte Target_MultiPlay;
    public byte Target_LockDraw;
    public byte Target_LightRange;
    public byte Target_KeepPlay;
    public ushort Target_KeepTime;
    public byte Target_KeepAttackRange;
    public byte Target_KeepMultiPlay;
    public byte Target_KeepAttackInterval;
    public byte Target_KeepLightRange;
}

/// <summary>TClientCustomMonsterConfig：发送到客户端的自定义怪物配置。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientCustomMonsterConfig
{
    public ushort wMonsterAppr;
    public TClientBaseConfig BaseConfig;
    public TMonsterClientActionTypeArray12 Actions;   // array[TMonsterClientActionType]
    public TClientAttackConfigArray6 AttackConfigs;   // array[0..5]
}

// ---- 自定义技能相关 ----

public enum TMagicActionType : byte { matSpell, matHit, matJumpHit, matNone, matCustom }
public enum TMagicPlusLevel : byte { mplNone, mpl1_3, mpl4_6, mpl7_9 }
public enum TMagicSwitchMode : byte { msmNone, msmSwitch, msmPowerHit }
public enum TMagicWarrNGOption : byte { mngoNone, mngoWideHit, mngoFireHit, mngoSwordHit, mngoTWNHit, mngoLongHit, mngoKTZHit, mngoCRSHit, mngo113Hit, mngoCustomHit1, mngoCustomHit2, mngoCustomHit3, mngoCustomHit4, mngoCustomHit5, mngoCustomHit6, mngoCustomHit7, mngoCustomHit8 }
public enum TMagicSoundType : byte { cmstManWarr, cmstWomanWarr, cmstUseMagic, cmstMagicFly, custMagicExplosion, custMagicFail }
public enum TMagicNeedItem : byte { meiNone, meiRedPoison, meiGreenPoison, meiFu, meiCustomItem }

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMagicClientBaseConfig
{
    public byte MagicLock;
    public TMagicActionType MagicActionType;
    public byte MagicLockSelf;
    public TMagicSwitchMode MagicSwitchMode;
    public byte SwitchModeNoClose;
    public byte MagicActionContinue;
    public byte MagicAutoOpen;
    public TMagicWarrNGOption MagicWarrNGOption;
    public int MagicActionStartIndex;
    public int MagicActionPlayCount;
    public int MagicActionEmptyCount;
    public byte NotRaiseHand;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMagicClientConfig
{
    public short Icon_File;
    public ushort Icon_Index;
    public ShortStr30Array6 Sounds;   // array[TMagicSoundType] of string[30]（6项）
    public short Fly_File;
    public ushort Fly_StartIndex;
    public ushort Fly_PlayCount;
    public ushort Fly_EmptyCount;
    public ushort Fly_PlayTime;
    public TCustomDrawMode Fly_DrawMode;
    public TCustomDirCount Fly_DirCount;
    public byte Fly_CalcDir;
    public byte Fly_FireGunMode;
    public byte Fly_LightRange;
    public short FlyEff_File;
    public ushort FlyEff_StartIndex;
    public TCustomDrawMode FlyEff_DrawMode;
    public short Self_File;
    public ushort Self_StartIndex;
    public byte Self_SyncHumAction;
    public ushort Self_PlayCount;
    public ushort Self_EmptyCount;
    public ushort Self_PlayTime;
    public TCustomDrawOrder Self_DrawOrder;
    public TCustomDrawMode Self_DrawMode;
    public TCustomDirCalcType Self_DirCalcType;
    public TCustomDirCount Self_DirCount;
    public byte Self_PlayDelayAction;
    public byte Self_LightRange;
    public byte Self_PlayFailNoDraw;
    public short SelfKeep_File;
    public ushort SelfKeep_StartIndex;
    public ushort SelfKeep_PlayCount;
    public ushort SelfKeep_PlayTime;
    public TCustomDrawMode SelfKeep_DrawMode;
    public ushort SelfKeep_KeepTime;
    public int SelfKeep_KeepTime2;
    public short FastMove_File;
    public ushort FastMove_StartIndex;
    public ushort FastMove_PlayCount;
    public ushort FastMove_EmptyCount;
    public ushort FastMove_PlayTime;
    public TCustomDrawOrder FastMove_DrawOrder;
    public TCustomDrawMode FastMove_DrawMode;
    public byte FastMove_CalcDir;
    public byte FastMove_NoHitAction;
    public byte FastMove_LightRange;
    public short PreTarget_File;
    public ushort PreTarget_StartIndex;
    public short PreTarget_StartIndex2;
    public ushort PreTarget_PlayCount;
    public ushort PreTarget_EmptyCount;
    public ushort PreTarget_PlayTime;
    public TCustomDrawMode PreTarget_DrawMode;
    public TCustomDrawMode PreTarget_DrawMode2;
    public byte PreTarget_CalcDir;
    public byte PreTarget_LockDraw;
    public byte PreTarget_LightRange;
    public short Target_File;
    public ushort Target_StartIndex;
    public short Target_StartIndex2;
    public ushort Target_PlayCount;
    public ushort Target_PlayTime;
    public TCustomDrawMode Target_DrawMode;
    public TCustomDrawMode Target_DrawMode2;
    public byte Target_MultiPlay;
    public byte Target_LockDraw;
    public byte Target_LightRange;
    public byte Target_KeepPlay;
    public ushort Target_KeepTime;
    public int Target_KeepTime2;
    public byte Target_KeepAttackRange;
    public byte Target_KeepMultiPlay;
    public byte Target_KeepAttackInterval;
    public byte Target_KeepLightRange;
    public short TargetStatus1_File;
    public ushort TargetStatus1_StartIndex;
    public ushort TargetStatus1_PlayCount;
    public ushort TargetStatus1_EmptyCount;
    public TCustomDrawMode TargetStatus1_DrawMode;
    public byte TargetStatus1_CalcDir;
    public short TargetStatus2_File;
    public ushort TargetStatus2_StartIndex;
    public ushort TargetStatus2_PlayCount;
    public ushort TargetStatus2_EmptyCount;
    public TCustomDrawMode TargetStatus2_DrawMode;
    public byte TargetStatus2_CalcDir;
}

[InlineArray(4)] public struct TMagicClientConfigs4 { private TMagicClientConfig _e0; }

/// <summary>TClientCustomMagicConfig：发送到客户端的自定义技能配置。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientCustomMagicConfig
{
    public ushort wMagicId;
    public byte boIsMagicWarr;
    public byte btNearAttackRange;
    public TMagicClientBaseConfig MagicBaseConfig;
    public TMagicClientConfigs4 MagicConfigs;   // array[TMagicPlusLevel]（4项）
    public TMagicNeedItem NeedItem;
    public int NeedItemCount;
    public fixed byte NeedItemCustomItemName[Grobal2Const.ITEM_NAME_LEN + 1];
    public byte NeedItemUseBagItem;
    public byte IsAttackUseNG;
    public byte NoChangeDir;

    public string NeedItemCustomItemNameStr { get { fixed (byte* p = NeedItemCustomItemName) return ShortStr.Get(p, Grobal2Const.ITEM_NAME_LEN); } set { fixed (byte* p = NeedItemCustomItemName) ShortStr.Set(p, Grobal2Const.ITEM_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TSelfKeepPlay
{
    public short SelfKeep_File;
    public short SelfKeep_StartIndex;
    public short SelfKeep_StartIndex2;
    public ushort SelfKeep_PlayCount;
    public ushort SelfKeep_PlayTime;
    public TCustomDrawOrder SelfKeep_DrawOrder;
    public TCustomDrawMode SelfKeep_DrawMode;
    public TCustomDrawMode SelfKeep_DrawMode2;
    public ushort SelfKeep_KeepTime;
}

// ---- 自定义 NPC 相关 ----

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TNpcDirAction
{
    public int Enabled;       // LongBool(4)
    public ushort Std_File;
    public short Std_Index;
    public ushort Std_Count;
    public ushort Std_Time;
    public ushort Std_EffFile;
    public short Std_EffIndex;
    public ushort Act_File;
    public short Act_Index;
    public ushort Act_Count;
    public ushort Act_Time;
    public ushort Act_EffFile;
    public short Act_EffIndex;
}

public enum TCustomNpcDrawOrder : byte { ndoKeep_Chr_Eff, ndoKeep_Eff_Chr, ndoChr_Keep_Eff, ndoChr_Eff_Keep, ndoEff_Keep_Chr, ndoEff_Chr_Keep }

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TNpcBaseConfig
{
    public int HPBgOffsetX;
    public int HPBgOffsetY;
    public int HPOffsetX;
    public int HPOffsetY;
    public int HPFile;
    public int HPStartIndex;
    public int HPTextOffsetX;
    public int HPTextOffsetY;
    public TCustomDrawMode StandDrawMode;
    public TCustomDrawMode StandEffectDrawMode;
    public TCustomDrawMode ActionDrawMode;
    public TCustomDrawMode ActionEffectDrawMode;
    public int KeepPlayFile;
    public int KeepPlayIndex;
    public int KeepPlayCount;
    public int KeepPlayTime;
    public byte KeepPlayBlendDraw;
    public int KeepPlayOffsetX;
    public int KeepPlayOffsetY;
    public TCustomNpcDrawOrder DrawOrder;
}

/// <summary>TClientCustomNpcConfig。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientCustomNpcConfig
{
    public ushort wNpcAppr;
    public ushort wDirCount;
    public TNpcBaseConfig BaseConfig;
    public TNpcDirActionArray8 Actions;   // array[DR_UP..DR_UPLEFT]（8 方向）
}

// ---- 其他杂项记录 ----

[StructLayout(LayoutKind.Sequential)]
public unsafe struct TItemCDTime
{
    public uint NormalHP;
    public uint NormalMP;
    public uint NormalHPMP;
    public uint SpecialHP;
    public uint SpecialMP;
    public uint SpecialHPMP;
    public uint Other;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct TEatItemCDConfig
{
    public TItemCDTimeArray3 Hum;    // array[0..2]
    public TItemCDTimeArray3 Hero;   // array[0..2]
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDropItemEffect
{
    public ushort ItemEffectIndex;
    public short FileIndex;
    public ushort StartIndex;
    public ushort Time;
    public byte ImageCount;
    public byte DrawCenter;
    public byte NoBlend;
    public byte BelowItem;
    public short OffsetX;
    public short OffsetY;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TGuardianLevelItemCounts
{
    public IntArray4 GetItemCounts;   // array[0..3]
    public IntArray4 CurItemCounts;
}

public enum TItemGroup : byte { igAll, igWeapon, igDress, igHelmet, igNecklace, igArmRing, igRing, igBelt, igBoots, igFashion, igDrug, igSpecial, igOther }

/// <summary>TClientDataCRC。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientDataCRC
{
    public uint ModulesCRC;
    public uint MonstersCRC;
    public uint MagicsCRC;
    public uint StdItemsCRC;
    public uint ItemDescCRC;
    public uint ItemDescTopCRC;
    public uint TzItemDescCRC;
    public uint FilterItemsCRC;
    public uint EffectImagesCRC;
    public uint SpecialCmdsCRC;
    public uint PlugClientsCRC;
    public uint BlackModulesCRC;
    public uint NpcsCRC;
    public uint DropItemEffectListCRC;
    public uint EnabledAuctionItemListCRC;
    public uint CustomItemPropertyCRC;
    public uint CustomItemPropertyTextVarListCRC;
    public uint ArrButtonConfigCRC;
    public fixed uint Reserved[3];
}

/// <summary>TGlobaSessionInfo（Delphi record 含引用类型 → 托管 class）。</summary>
public class TGlobaSessionInfo
{
    public string sAccount = "";
    public string sIPaddr = "";
    public int nSessionID;
    public int n24;
    public bool bo28;
    public bool boLoadRcd;
    public bool boStartPlay;
    public bool boHeroLoadRcd;
    public uint dwAddTick;
    public double dAddDate;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientUserShop
{
    public fixed byte sShopName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sMasterName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public double dCreateDate;
    public int nSellingItemCount;
    public int nSelledItemCount;
    public int nStorageItemCount;
    public byte boBusiness;
    public int nCareValue;
    public byte btState;

    public string ShopName { get { fixed (byte* p = sShopName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sShopName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string MasterName { get { fixed (byte* p = sMasterName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sMasterName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientUserShopItem
{
    public fixed byte sShopName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sMasterName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sBuyName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public byte IsMyShopeItem;
    public byte btMoneyType;
    public byte btItemType;
    public double dCreateDate;
    public byte btAllowSell;
    public byte boGetMoney;
    public TClientItem Item;

    public string ShopName { get { fixed (byte* p = sShopName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sShopName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string MasterName { get { fixed (byte* p = sMasterName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sMasterName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string BuyName { get { fixed (byte* p = sBuyName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sBuyName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientCustomMoney
{
    public fixed byte sName[Grobal2Const.CUSTOMMONEY_NAME_LEN + 1];
    public int nIndex;
    public byte boCanMyShop;
    public byte boCanAuction;
    public byte boCanSellPlayer;

    public string NameStr { get { fixed (byte* p = sName) return ShortStr.Get(p, Grobal2Const.CUSTOMMONEY_NAME_LEN); } set { fixed (byte* p = sName) ShortStr.Set(p, Grobal2Const.CUSTOMMONEY_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TServerWeateherEffect
{
    public byte boIsUsed;
    public byte boIsDark;
    public uint dwTick;
    public uint dwTime;
    public fixed byte sMusic[51];  // string[50]

    public string Music { get { fixed (byte* p = sMusic) return ShortStr.Get(p, 50); } set { fixed (byte* p = sMusic) ShortStr.Set(p, 50, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TResquestAllAuctionItem
{
    public int ShowAuctionID;
    public int MoneyType;
    public uint Prices1;
    public uint Prices2;
    public fixed byte KeyWord[31];  // string[30]

    public string KeyWordStr { get { fixed (byte* p = KeyWord) return ShortStr.Get(p, 30); } set { fixed (byte* p = KeyWord) ShortStr.Set(p, 30, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientItemRef { public TClientItem Item; }

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TAllAuctionItem
{
    public int AuctionID;
    public uint StartingPrice;
    public uint SellingPrice;
    public int CurrencyType;
    public int LastBidPrice;
    public int TimeLeft;
    public int TradingStatus;
    public byte IsItemGive;
    public byte IsAttention;
    public byte IsItemCanRetrieve;
    public byte IsItemFlash;
    public TClientItem Item;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMyAuctionItem
{
    public int AuctionID;
    public uint StartingPrice;
    public uint SellingPrice;
    public int CurrencyType;
    public int LastBidPrice;
    public int TimeLeft;
    public int TradingStatus;
    public byte IsItemGive;
    public TClientItem Item;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TAcutionItemPricesLime
{
    public LongArray5 Min;   // array[0..4] of LongWord
    public LongArray5 Max;
}

[InlineArray(5)] public struct LongArray5 { private uint _e0; }

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TRulesActionItem
{
    public fixed byte ItemName[Grobal2Const.ITEM_NAME_LEN + 1];
    public TAcutionItemPricesLime Prices;

    public string ItemNameStr { get { fixed (byte* p = ItemName) return ShortStr.Get(p, Grobal2Const.ITEM_NAME_LEN); } set { fixed (byte* p = ItemName) ShortStr.Set(p, Grobal2Const.ITEM_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TRefreshAuctionItem
{
    public int AuctionID;
    public int LastBidPrice;
    public int TradingStatus;
    public byte IsItemGive;
    public byte IsItemCanRetrieve;
    public byte IsCancelAttention;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TShopClientItem
{
    public TStdItem StdItem;
    public byte GameMoney;
    public int ImageIndex;
    public int ImageCount;
    public fixed byte Memo1[19];     // string[18]
    public fixed byte Memo2[151];    // string[150]
    public int ItemCount;
    public byte boBulkBuy;
    public int nBulkBuyCount;

    public string Memo1Str { get { fixed (byte* p = Memo1) return ShortStr.Get(p, 18); } set { fixed (byte* p = Memo1) ShortStr.Set(p, 18, value); } }
    public string Memo2Str { get { fixed (byte* p = Memo2) return ShortStr.Get(p, 150); } set { fixed (byte* p = Memo2) ShortStr.Set(p, 150, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TUserLevelRanking
{
    public int nIndex;
    public int nLevel;
    public fixed byte sChrName[Grobal2Const.ACTOR_NAME_LEN + 1];

    public string ChrName { get { fixed (byte* p = sChrName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sChrName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct THeroLevelRanking
{
    public int nIndex;
    public int nLevel;
    public fixed byte sChrName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sHeroName[Grobal2Const.ACTOR_NAME_LEN + 1];

    public string ChrName { get { fixed (byte* p = sChrName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sChrName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string HeroName { get { fixed (byte* p = sHeroName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sHeroName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TUserMasterRanking
{
    public int nIndex;
    public int nMasterCount;
    public fixed byte sChrName[Grobal2Const.ACTOR_NAME_LEN + 1];

    public string ChrName { get { fixed (byte* p = sChrName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sChrName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDeleteHumanInfo
{
    public fixed byte sChrName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public int nLevel;
    public byte btJob;
    public byte btSex;

    public string ChrName { get { fixed (byte* p = sChrName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sChrName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TStorageHeroInfo
{
    public fixed byte sHeroName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public int nLevel;
    public byte btJob;
    public byte btSex;

    public string HeroName { get { fixed (byte* p = sHeroName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sHeroName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TGameGoldDealItem
{
    public int nMakeIndex;
    public fixed byte sItemName[Grobal2Const.ITEM_NAME_LEN + 1];

    public string ItemName { get { fixed (byte* p = sItemName) return ShortStr.Get(p, Grobal2Const.ITEM_NAME_LEN); } set { fixed (byte* p = sItemName) ShortStr.Set(p, Grobal2Const.ITEM_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TGameGoldDeal
{
    public TGameGoldDealState DealState;
    public fixed byte SellChrName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte BuyChrName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public int GameGold;
    public int GameDiamond;
    public double SellDateTime;
    public byte ItemCount;

    public string SellChrNameStr { get { fixed (byte* p = SellChrName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = SellChrName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string BuyChrNameStr { get { fixed (byte* p = BuyChrName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = BuyChrName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMagicBallEffectInfo
{
    public int FileIndex;
    public int ImageIndex;
    public int ImageCount;
    public int FrameTime;
    public int ShowTime;
    public int ShowType;
    public int DrawHeigh;
    public int DrawType;
    public int OffsetX;
    public int OffsetY;
    public int IsNormalDraw;   // WordBool(4)
    public uint StarTick;
    public int FrameIndex;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TScreenEffectData
{
    public int nX;
    public int nY;
    public int nImageIndex;
    public int nStartImage;
    public int nImageCount;
    public int nPlayCount;
    public int nTime;
    public byte boBlend;
    public byte boDrawTopmost;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TGuildJoinUser
{
    public fixed byte sUserName[21];   // string[20]
    public byte btSex;
    public byte btJob;
    public byte boOnline;
    public uint nLevel;
    public double dtLastLogin;

    public string UserName { get { fixed (byte* p = sUserName) return ShortStr.Get(p, 20); } set { fixed (byte* p = sUserName) ShortStr.Set(p, 20, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TArrButtonGroupConfig
{
    public byte HorzAligment;    // TAlignment
    public TVerticalAlignment VertAligment;
    public int OffsetX;
    public int OffsetY;
    public int NextOffsetX;
    public int NextOffsetY;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TArrBuffAddData
{
    public int FlashTimeLeft;
    public int FlashStartIndex;
    public int FlashCount;
    public int nTextOffsetX;
    public int nTextOffsetY;
    public byte boTimeOffNoHide;
    public int nTimeOffImageIndex;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TSellPlayerAddAskInfo
{
    public fixed byte ChrName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public byte MoneyType;
    public uint Prices;

    public string ChrNameStr { get { fixed (byte* p = ChrName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = ChrName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TSellPlayerSearchInfo
{
    public fixed byte PlayerName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public byte Job;
    public uint LevelMin;
    public uint LevelMax;
    public uint PricesMin;
    public uint PricesMax;
    public byte MoneyType;
    public byte SortType;

    public string PlayerNameStr { get { fixed (byte* p = PlayerName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = PlayerName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TSellPlayerItem
{
    public ushort Index;
    public fixed byte PlayerName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public uint Level;
    public byte Job;
    public byte MoneyType;

    public string PlayerNameStr { get { fixed (byte* p = PlayerName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = PlayerName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

public enum TBBType : byte { bb_BoneFamm, bb_Dogz, bb_BigDogz, bb_MonthSpirit, bb_Other }

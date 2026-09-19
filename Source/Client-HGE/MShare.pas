unit MShare;

interface


{.$DEFINE USE_IDHTTP} //引用IDuRI和iDhTTP会造成内存泄漏

uses
  Windows,
  Messages,
  Classes,
  SysUtils,
  Forms,
  Graphics,
  Controls,
  DIB,
  dialogs,
  HGE,
  HGECanvas,
  DxComponents,
  magiceff,
  MemoryStreamEx,
  DxImageButton,
  MudUtil,
  GameImages,
  Wil,
  Wis,
  Uib,
  Pak,
  Wzl,
  Actor,
  Grobal2,
  BassSound,
  DxControls,
  SoundUtil,
  MD5Util,
  HardInfo,
  UnitDes,
  EDcode,
  SDK,
  ClFunc,
  DrawScrn,
  IntroScn,
  PlayScn,
  clEvent,
  MapUnit,
  PathFind,
  IniFiles,
  HashList,
  GameConfigDlg,
  FastStrings,
  UpdateEngine,
  CheckUnit,
  StrUtils,
  SyncObjs,
  MemoryModule,
  MemoryModuleDef,
  WinINet,
  {$IFDEF USE_IDHTTP}
  IdURI,
  IdHTTP,
  //IdSSLOpenSSL,
  {$ENDIF}
  uDropItemEffectList,
  GlobalString,
  MMSystem,
  DropItemsMgr,
  ShlwApi,
  superobject;

const
  CLIENT_MODE_NORMAL = 0;  //正常模式
  CLIENT_MODE_UC = 1;      //用户中心登录模式
  CLIENT_MODE_CUSTOM = 2;  //游戏盒子定制模式，按客户定义要求，隐藏注册和修改密码功能

const
  RUN_UP_STEPS = 3; //20230816 助跑步数定义

const
  AUCTION_BAG_ONE_PAGE_COUNT = 10; //拍卖中包裹物品每页显示10条

const
  RCM_UNBINDITEM = 10000;
  RCM_AutoUNBINDITEM = 10001;
  RCM_PLAYMAPMUSIC = 10002;
  RCM_RESTOREDEVICE = 10003;
  RCM_FREEMYHERO = 10004;
  RCM_AutoOrderItem = 10006;
  RCM_StartContinuousMagicAttack = 10007;
  RCM_PlayBGMgameover = 10008;
  RCM_FindModule = 10009;
  RCM_CHECKCRCERROR = 10010;
  RCM_AutoOrderItem_Auto = 10011;
  RCM_AutoUNBINDITEM_Auto = 10012;
  RCM_FINDPROCESS = 10013;

const
  DWhisperMemoUpColor = clWhite;
  DWhisperMemoHotColor = clYellow;
  DWhisperMemoDownColor = clRed;

const
  SM_ACTION_MIN = SM_TURN;
  SM_ACTION_MAX = SM_SPELL2;

const
  UNITX = 48;
  UNITY = 32;
  HALFX = 24;
  HALFY = 16;
  LOGICALMAPUNIT = 40;
  HINTTEXT_MAX_LEN = 80;

const
  U_DRESSNAME = '衣服             ';
  U_WEAPONNAME = '武器             ';
  U_RIGHTHANDNAME = '勋章             ';
  U_NECKLACENAME = '项链             ';
  U_HELMETNAME = '头盔             ';
  U_ARMRINGLNAME = '左手镯           ';
  U_ARMRINGRNAME = '右手镯           ';
  U_RINGLNAME = '左戒指           ';
  U_RINGRNAME = '右戒指           ';
  U_ARMRINGNAME = '手镯             ';
  U_RINGNAME = '戒指             ';
  U_BUJUKNAME = '物品             ';
  U_BELTNAME = '腰带             ';
  U_BOOTSNAME = '鞋子             ';
  U_CHARMNAME = '宝石             ';
  U_HATNAME = '斗笠             ';
  U_DRUMNAME = '军鼓             ';
  U_HORSENAME = '马             ';
  GL_QUIT = 100;
  GL_CHECKTICK = 101;
  GL_FINDFILTERPROCESS = 102;
  CM_HANDLE = 210;
  CM_QUIT = 211;
  CM_INITIALIZED = 212;
  CM_FINALIZE = 213;
  CM_USERLOGON = 214;
  mtBagItem = 0;
  mtUseItem = 1;
  mtHeroBagItem = 2;
  mtHeroUseItem = 3;
  mtDealItem = 4;
  mtBagGold = 5;
  mtDealGold = 6;
  mtGameGoldDeal = 7;
  mtChallengeGold = 8;
  mtChallengeItem = 9;
  mtUpgradeItem = 10;
  mtWineMatItem = 11;
  mtHeroM2ShopItem = 12;
  mtJewelryItem = 13;
  mtHeroJewelryItem = 14;
  mtItemBoxItem = 15;
  mtGodBlessItem = 16;
  mtHeroGodBlessItem = 17;
  mtGamePetBagItem = 18;

  // 自定义光标
  crMySelItem = 1;
  crSrepair = 2;

type
  TTimerCommand = (tcSoftClose, tcReSelConnect, tcFastQueryChr, tcQueryItemPrice);

  TChrAction = (caNone, caWalk, caRun, caHorseRun, caHit, caSpell, caSitdown);

  TConnectionStep = (cnsLogin, cnsSelChr, cnsReSelChr, cnsPlay);

  TAntiPlugFunc = function(A, B:Integer):Integer; stdcall;

  // TMoveItemType = (mtBagItem, mtUseItem, mtHeroBagItem, mtHeroUseItem, mtDealItem,
  // mtBagGold, mtDealGold, mtGameGoldDeal, mtChallengeGold, mtChallengeItem, mtUpgradeItem, mtWineMatItem);

  PInstanceInfo = ^TInstanceInfo;

  TInstanceInfo = packed record
    PreviousHandle:THandle;
    RunCount:Integer;
    FindWindow:Boolean;
    ClientHandle:array[0..9] of THandle;
    OpenPages:array[0..9] of string[255];
  end;

  TScreenEffectInfo = record
    nFileIndex:Integer;
    nImageIndex:Integer;
    wImageCount:Word;
    wFrameTime:Word; // 播放速度 毫秒
    nLoopCount:Integer;
    nX, nY:Integer;
    nCurrentFrame:Integer;
    dwFrameTimeTick:LongWord;
    boBlend:Boolean;
    boDrawTopmost:Boolean;
    LockObject:Int64;
  end;

  pTScreenEffectInfo = ^TScreenEffectInfo;

  TCheckData = packed record
    dwCrc:Cardinal;
    dwCode:Cardinal;
    nSize:Integer;
  end;

  // 客户端按钮信息 -- 新添加功能用的
  TClientBuffInfo = record
    Button:TDxImageButton;
    Time:Integer;
    Tick:LongWord;
    //Left: Integer;
  end;

  pTClientBuffInfo = ^TClientBuffInfo;

  // 客户端按钮信息 -- 新添加功能用的
  TArrBuffInfo = record
    GroupID:Integer;
    Button:TDxImageButton;
    Time:Integer;
    Tick:LongWord;
    FlashTimeLeft:Integer;
    FlashStartIndex:Integer;
    FlashCount:Integer;
    FlashTick:LongWord;
    FlashIndex:Integer;
    nTextOffsetX:Integer;
    nTextOffsetY:Integer;
    boTimeOffNoHide:Boolean;
    nTimeOffImageIndex:Integer;
  end;

  pTArrBuffInfo = ^TArrBuffInfo;

  TMirsClientConfig = record
    nBeginPos:Integer;
    nEndPos:Integer;
    nCrc:Integer;
    nSize:Integer;
    btVersion:Byte;
  end;

  pTMirsClientConfig = ^TMirsClientConfig;

  TMapDescInfo = record
    sMapName:string;
    sDescName:string;
    nX, nY:Integer;
    FColor:TColor;
    boBigMap:Boolean;
  end;

  pTMapDescInfo = ^TMapDescInfo;

  TMapDescList = record
    sMapName:string;
    Large:TList;
    Small:TList;
  end;

  pTMapDescList = ^TMapDescList;

  TBindItem = record
    btItemType:Byte;
    sItemName:string[14];
    sBindItemName:string[14];
  end;

  pTBindItem = ^TBindItem;

  TItemDesc = record
    Name:string;
    Desc:TStringList;
  end;

  pTItemDesc = ^TItemDesc;

  TTzItemDesc = record
    ItemName:string;
    NameColor:TColor;
    Sex:Byte;
    Job:Byte;
    ItemCount:Integer;
    ItemList:TList;
    ItemDesc:TStringList;
  end;

  pTTzItemDesc = ^TTzItemDesc;

  TDrawItemEffect = record
    btDrawCount:Integer;
    dwDrawTick:LongWord;
    btHeroM2DrawCount:Integer;
    dwHeroM2DrawTick:LongWord;
  end;

  pTDrawItemEffect = ^TDrawItemEffect;

  TSkillDesc = record
    SkillType:string;
    SkillName:string;
    SkillLevel:Integer;
    SkillDesc:TStringList;
  end;

  pTSkillDesc = ^TSkillDesc;

  TMovingItem = packed record
    Index:Integer;
    ItemType:Integer; // TMoveItemType;
    Item:TClientItem;
  end;

  pTMovingItem = ^TMovingItem;

  TClientGoods = record
    Name:string[ITEM_NAME_LEN];
    RealName:string[ITEM_NAME_LEN];
    SubMenu:Integer;
    Price:Integer;
    Stock:Integer;
    Grade:Integer;
    Count:Integer;
    Looks:Integer;
  end;

  PTClientGoods = ^TClientGoods;

  TPowerBlock = array[0..100 - 1] of Word;

  // 内挂自定义绑定物品
  TCustomBindItem = record
    UnBindItemType:TUnBindItemType;
    sItemName:string;
    sBindItemName:string;
    boSpecialMP:Boolean;
  end;

  pTCustomBindItem = ^TCustomBindItem;

  {$IF TESTMODE = 2}
  TClientParam = packed record
    Handle:THandle;
    sGameLoginFileName:array[0..251] of Char;
    sServerCaption:array[0..99] of Char;
    sServeraddr:array[0..99] of Char;
    nServerPort:Integer;
    sUpdateAddr:array[0..99] of Char; // 微端更新地址
    nUpdatePort:Integer; // 微端更新端口
    sUpdatePassWord:array[0..251] of Char; // 微端更新密码
    nClientCrc:Cardinal; // Client CRC
    nLoginCrc:Cardinal; // 登录器 CRC
    sDomainName:array[0..999] of Char; // 绑定的域名
    sHomePage:array[0..251] of Char;
    wScreenWidth:Word; // 分辨率
    wScreenHeight:Word; // 分辨率
    btBitCount:Byte; // 颜色位数   16或32
    boWindowMode:Boolean; // 是否是窗口模式  TRUE=窗口 FALSE=全屏
    boVSync:Boolean; // 垂直同步
    boHardware:Boolean; // 硬件加速  无效 Client内部固定
    ClientVersion:TClientVersion; // 176   185   英雄版  连击版  传奇续章  外传  归来

    sResourcesDir:array[0..99] of Char; // Resources目录位置
    sPakPassword:array[0..503] of Char; // pak密码
  end;

  {$ELSE}

  TClientParam = packed record
    Handle:THandle;
    sGameLoginFileName:string[255];
    sServerCaption:string[100];
    sServeraddr:string[100];
    nServerPort:Integer;
    sUpdateAddr:string[100]; //  微端更新地址
    nUpdatePort:Integer; //  微端更新端口
    sUpdatePassWord:string[100]; //微端更新密码
    nClientCrc:Cardinal; //Client CRC
    nLoginCrc:Cardinal; //登录器 CRC
    sDomainName:array[0..1000 - 1] of Char; //绑定的域名
    //nDomainNameLen: Integer;
    sHomePage:string[255];
    btMaxClientCount:Byte; //多开数量

    wScreenWidth:Word; //分辨率
    wScreenHeight:Word; //分辨率
    btBitCount:Byte; //颜色位数   16或32
    boWindowMode:Boolean; //是否是窗口模式  TRUE=窗口 FALSE=全屏
    boVSync:Boolean; //垂直同步
    boHardware:Boolean; //硬件加速  无效 Client内部固定
    ClientVersion:TClientVersion; //176   185   英雄版  连击版  传奇续章  外传  归来

    sSemaphoreName:string[40]; // CreateSemaphore的名字

    sMachineID:string[32]; // 硬件ID chongchong 2015-08-06
    sClientDataFile:string[255]; // 内核附加数据 chongchong 2016-01-06

    ConfigUrlMD5:MD5Digest; //服务器列表MD5

    sPromotionFlag:string[40]; // 推广ID 2021-01-21
  end;
  {$IFEND}

  pTClientParam = ^TClientParam;
  {TGameLoginConfig = packed record
    ClientVersion: TClientVersion; // 176   185   英雄版  连击版  传奇续章  外传  归来
    nGameLoginCrc: Cardinal;
    sLinkName: string[30];
    sConfigUrl: array[0..1000 - 1] of Char;
    sConfigUrl2: array[0..1000 - 1] of Char; // 备用地址
    sProgramFile: string[30];

    nClientOffSet: Integer;
    nClientSize: Integer;
    nClientCrc: Cardinal;
    nConfigUrl: Cardinal;
    nConfigUrl2: Cardinal;

    nGamePlanOffSet: Integer;
    nGamePlanSize: Integer;
    nGamePlanCrc: Cardinal;

    nGameLoginSkinOffSet: Integer;
    nGameLoginSkinSize: Integer;

    nPatchOffSet: Integer; // 补丁数据
    nPatchSize: Integer; // 补丁数据

    boAutoGetServerList: Boolean;
    nAutoGetServerListTime: Integer;

    nSearchMirDirectoryConditionOffSet: Integer;
    nSearchMirDirectoryConditionSize: Integer;

    nSearchMirFileNameConditionOffSet: Integer;
    nSearchMirFileNameConditionSize: Integer;

    btScreenMode: Byte;
    btBitCount: Byte;
    boWindowMode: Boolean;
    boHardware: Boolean;
    boVSync: Boolean;
    boDisableManyClient: Boolean; // 禁止多开
    sUpdatePassWord: string[50]; // 轻客户端更新密码
    sLoginConfigFileName: string[50]; // 登录器配置文件名称
  end;
  pTGameLoginConfig = ^TGameLoginConfig;}

  // 装备属性类型

  TArrHintLines = array of THintLines;

  PArrHintLines = ^TArrHintLines;

  THintPropertyType = (hptItemName, hptTopDesc, hptItemIcon, hptUpgradeStar, hptItemProgress, hptBase, hptFlutesInfo, {htpFluteStone, hptFluteNoStone, }
    hptElements, hptCustomProperty, hptInsuranceInfo, hptItemFrom, hptSellInfo, hptTZInfo, hptItemDesc);

  THintTextType = (httWeight, httDura, httCount, httQuality, httContent, httCapacity, httPurity, httStarCount, httTransferCount, httUseCount, httRepairDura, // httStrength2 add 2020-08-12 00:30:06
    httHP, httHP2, httHP3, httHP4, httHP5, httMP, httMP2, httMP3, httMP4, httMP5, // httHP2, httHP3, httHP4, httMP2, httMP3, httMP4 add 2020-08-12 00:30:06
    httHPMP, httHPMP2, httLevel, httExp, httNGPoint, httNGPoint2, httNGPoint3, httNGPoint4, httBurden, httBurden2, httBurden3, httDC, httDC2, httDC3, httMC, httMC2, httMC3, httSC, httSC2, httSC3, httAC, httAC2, httAC3, httMAC, httMAC2, httMAC3, httHintPoint, httHintPoint2, httHintPoint3, httSpeedPoint, httSpeedPoint2, httSpeedPoint3, httStrength, httStrength2, httHoly, httHoly2, httLucky, httLucky2, httLucky3, httCurse, httCurse2, httHPValue, httHPValue2, httMPValue, httMPValue2, httAttackSpeed,
    httAttackSpeed2, httAttackSpeed3, httAttackSpeed4, httAttackSpeed5, httBagWeight, httBagWeight2, httHealthRecover, httHealthRecover2, httHealthRecover3, httSpellRecover, httSpellRecover2, httSpellRecover3, httPoisonRecover, httPoisonRecover2, httPoisonRecover3, httMagicAvoid, httMagicAvoid2, httMagicAvoid3, httPoisonAvoid, httPoisonAvoid2, httPoisonAvoid3, httProtectiveRate, httFrozenRate, httCobwebRate, httReactiveReate, httParalysisRate, httMDParalysisRate, httNeedJob, httNeedFixedLvel, httNeedLevel,
    httNeedPrestige, httNeedDC, httNeedMC, httNeedSC, httNeedReLevel, httNeedReLevel2, httNeedGuildMaster, httNeedGuildMember, httNeedShabakMaster, httNeedShabakMember, httNeedMember, httNeedMemberLevel, httNeedMemberType, httNeedJobWarr, httNeedJobWizard, httNeedJobTaos, httLimitedItem, httLimitedItemTimeOut, httInsuranceGoldType, httInsurancePrice, httInsuranceCount, httItemFromTitle, httItemFromMaker, httItemFromGM, httItemFromNPC, httItemFromShop, httItemFromSysGive, httItemFromMine, httItemFromBox,
    httItemFromButchItem, httItemFromCaptureMon, httItemFromSellToShop, httItemFromMakerName, httItemFromTime, httItemFromMonster, httItemFromMap, httItemFromKiller, httItemFromBuyer, httElementNewProperty01, httElementNewProperty02, httElementNewProperty03, httElementNewProperty04, httElementNewProperty05, httElementNewProperty06, httElementNewProperty07, httElementNewProperty08, httElementNewProperty09, httElementNewProperty10, httElementNewProperty11, httElementNewProperty12, httElementNewProperty13,
    httElementNewProperty14, httElementNewProperty15, httElementNewProperty16, httElementNewProperty17, httElementNewProperty18, httElementNewProperty19, httElementNewProperty20, httElementNewProperty21, httElementNewProperty22, httElementNewProperty23, httElementNewProperty24, httElementNewProperty25, httElementNewProperty01_2, httElementNewProperty02_2, httElementNewProperty03_2, httElementNewProperty04_2, httElementNewProperty05_2, httElementNewProperty06_2, httElementNewProperty07_2,
    httElementNewProperty08_2, httElementNewProperty09_2, httElementNewProperty10_2, httElementNewProperty11_2, httElementNewProperty12_2, httElementNewProperty13_2, httElementNewProperty14_2, httElementNewProperty15_2, httElementNewProperty16_2, httElementNewProperty17_2, httElementNewProperty18_2, httElementNewProperty19_2, httElementNewProperty20_2, httElementNewProperty21_2, httElementNewProperty22_2, httElementNewProperty23_2, httElementNewProperty24_2, httElementNewProperty25_2, httExpFull,
    httCumulativeExp, httReleaseExpNeed, httPetEgg, httSkill, httOpenBoxHint, httOpenBoxKeyHint, httContinueUse, httCumulativeUse, httUpgradeItemType, httUpgradeItemRate, httUpgradeItemLevelLimte, httUpgradeItemOK, httUpgradeItemFail, httMagicBookHero, httMagicBookNG, httMagicBookContinue, httMagicBookGroup, httMagicBookWarr, httMagicBookWizard, httMagicBookTaos, httMagicBookGeneral, httHintDblClickOpenBagGrid);

  PItemHintInfo = ^TItemHintInfo;

  TItemHintInfo = record
    Index:Byte;
    PropertyType:THintPropertyType;
    ShowSpliter:Boolean;
    HintWindow:Byte;
    ShowNormalHint:Boolean;
    Alignment:TAlignment;
    Color:TColor;
    ShowText:string[80];
  end;

  PItemHintTextInfo = ^TItemHintTextInfo;

  TItemHintTextInfo = record
    Alignment:TAlignment;
    Text:string[HINTTEXT_MAX_LEN];
  end;

  TAutoCustomMagics = array of Word;

  TConfigClient = packed record  //登录器使用的配置
    nSize:Integer;
    nCrc:Cardinal;
    nBaseUIOffSet:Integer; // 基础游戏 UI
    nBaseUISize:Integer;
    nShareUIOffSet:Integer; // 共享游戏 UI 个人商店
    nShareUISize:Integer;
    nNewStateWindowUIOffSet:Integer; // 连击新人物属性界面 UI
    nNewStateWindowUISize:Integer;
    nConfigDlgUIOffSet:Integer; // 内挂UI
    nConfigDlgUISize:Integer;
    nJSYUIOffSet:Integer; // 及时雨UI
    nJSYUISize:Integer;
    nBackBmpOffSet:Integer;
    nBackBmpSize:Integer;
    nBackBmpCrc:Cardinal;
    nCursorDefOffset:Integer;
    nCursorDefSize:Integer;
    nCursorDefCrc:Cardinal;
    nCursorMountOffset:Integer;
    nCursorMountSize:Integer;
    nCursorMountCrc:Cardinal;
    nCursorUnmountOffset:Integer;
    nCursorUnmountSize:Integer;
    nCursorUnmountCrc:Cardinal;
    nPlugFileOffset:Integer;
    nPlugFileSize:Integer;
    nCustomMonsterConfigOffset:Integer;
    nCustomMonsterConfigSize:Integer;
    nCustomMagicConfigOffset:Integer;
    nCustomMagicConfigSize:Integer;
    nCustomNpcConfigOffset:Integer;
    nCustomNpcConfigSize:Integer;
    nCustomProtectItemsOffset:Integer;
    nCustomProtectItemsSize:Integer;
    nCustomBossListOffset:Integer;
    nCustomBossListSize:Integer;
    nItemDescListOffset:Integer;
    nItemDescListSize:Integer;
    nItemDescTopListOffset:Integer;
    nItemDescTopListSize:Integer;
    nFilterItemListOffset:Integer;
    nFilterItemListSize:Integer;
    nTZItemDescListOffset:Integer;
    nTZItemDescListSize:Integer;
    nGodBlessItemsOffset:Integer;
    nGodBlessItemsSize:Integer;
    nDataFileOffSet:Integer;
    nDataFileSize:Integer;
    nMapFileOffSet:Integer;
    nMapFileSize:Integer;
    nWavFileOffSet:Integer;
    nWavFileSize:Integer;

    //nHMapDataFileOffset: Integer;
    //nHMapDataFileSize: Integer;

    nPlugFileNameOffSet:Integer;
    nPlugFileNameSize:Integer;
    nImageFileOffSet:Integer;
    nImageFileSize:Integer;
    sGamePlanName:string[30]; // 必备补丁名 chongchong 2015-01-08
    boChangeSrceenBitCount:Boolean;
    boShowOpenDoor:Boolean;
    boShow1024:Boolean;
    sRunGatePassWord:string[50]; // RunGate密码

    //ClientConfigs:array[0..93 - 1] of Boolean; // 客户端内挂配置--来自配置器--数组大小需要同步修改 piaoyun 2013-11-14
    ClientConfigs:array[0..96 - 1] of Boolean; //HZQ 20230718 增加了三个选项
    ClientConfigs_Ex:array[0..0] of Boolean; // 内挂配置扩展
    HumManuallyCustomHits:array[0..4] of LongWord; // 手动技能

    // Resources 目录 piaoyun 2013-09-04
    sResourcesDir:string[50];
    boWindowBiMaximize:Boolean; // 窗口模式最大化
    nLoadResourcesOrder:Byte; // 资源读取顺序 piaoyun 2013-10-28
    boShowVersion:Boolean; // 显示版本信息 piaoyun 2013-11-24
    boShowHealthNotice:Boolean; // 显示健康公告
    ////////////////////////////////自定义UI////////////////////////////////////
    boCustomUI:Boolean; // 是否自定义UI
    boCustomConfigDlg:Boolean;

    // 自定义UI的一些参数
    NewUiFileNames:array[0..4] of string[50]; // 图库名称

    nGoodNumOffsetX:Integer; // 物品栏数字编号X修正
    nGoodNumOffsetY:Integer; // 物品栏数字编号Y修正

    nChatMemoItemFColor:TColor;
    nChatMemoItemBColor:TColor;
    ////////////////////////////////////////////////////////////////////////////

    nPivateClientKey1:LongWord;

    // 左侧区域
    ////////////////////////////////////////////////////////////////////////////
    boChatTopButtonNoMove:Boolean;
    nChatTopButtonXSpace:Integer;
    nChatTopButtonYSpace:Integer;
    boLeftButtonNoMove:Boolean;
    nLeftButtonXSpace:Integer;
    nLeftButtonYSpace:Integer;

    // 右侧区域
    boBottomLevelTextUseSystemDef:Boolean; // 等级字体使用系统默认
    boDefShowStateWinEx:Boolean;
    boHPPercentShow:Boolean;
    boMPPercentShow:Boolean;
    boChatSayItemHideClose:Boolean;
    boOverLapItemNumOldShow:Boolean;
    boHealthNumberSeparate:Boolean;
    nHealthNumberSelfOffset:Integer;
    nHealthNumberHumOffset:Integer;
    nPivateClientKey2:LongWord;
    boShowAniStarImage:Boolean;
    nShowAniStarImageIndex:Integer;
    nShowAniStarImageCount:Integer;
    nShowAniStarImageIncSpacing:Integer;
    nShowAniStarImagePlayTime:Integer;
    boShowAniBagCompareImg:Boolean;
    nShowAniBagCompareImgIndex:Integer;
    nShowAniBagCompareImgCount:Integer;
    nShowAniBagCompareImgPlayTime:Integer;
    nEquipmentImgOffsetX:Integer;
    nEquipmentImgOffsetY:Integer;
    boCustomActorSimpleShow:Boolean;
    nSimpleActorRace:Integer;
    nSimpleActorRaceImg:Integer;
    nSimpleActorAppr:Integer;
    boCustomHuamSimpleShow:Boolean;
    nSimpleDressShapeArr:array[0..2] of Integer;
    nSimpleWeaponShapeArr:array[0..2] of Integer;
    boCustomBBSimpleShow:Boolean;
    nSimpleBBRace:Integer;
    nSimpleBBRaceImg:Integer;
    nSimpleBBAppr:Integer;
    boShowExploreItemIcon:Boolean;
    btExploreItemIconShowType:Byte;
    dwExploreItemIconIndex:LongWord;
    dwExploreItemIconCount:LongWord;
    dwExploreItemIconPlayTime:LongWord;
    nExploreItemIconOffsetX:Integer;
    nExploreItemIconOffsetY:Integer;
    boShowValueItemEffect:Boolean;
    dwValueItemEffectIndex:LongWord;
    dwValueItemEffectCount:LongWord;
    dwValueItemEffectPlayTime:LongWord;
    nValueItemEffectOffsetX:Integer;
    nValueItemEffectOffsetY:Integer;
    sAttackModeTexts:array[0..7] of string[40]; // 攻击模式显示文字 chongchong 2014-04-06

    sExpAddHintText:string[60];
    sNGExpAddHintText:string[60];
    nPivateClientKey3:LongWord;
    nNPCMsgDlgTextOffsetX:Integer; // NPC对话框文字坐标微调X
    nNPCMsgDlgTextOffsetY:Integer; // NPC对话框文字坐标微调Y

    nAuctionBroadcastDlgX:Integer; // 拍卖全服公告对话框位置偏移X
    nAuctionBroadcastDlgY:Integer; // 拍卖全服公告对话框位置偏移Y

    UpdateStateDlgHorzAlign:TAlignment;
    UpdateStateDlgVertAlign:TVerticalAlignment;
    UpdateStateDlgOffsetX:Integer;
    UpdateStateDlgOffsetY:Integer;
    sElementNewPropertyTexts:array[0..24] of string[80]; // 元素新属性文字 chongchong 2014-04-06

    sHumPropertyGroupCaption:array[0..6] of string[40]; // 人物栏属性分组（\表示换行）

    sItemHintFluteStoneText:string[80];
    sItemHintNoFluteStoneText:string[80];
    nItemHintFluteStoneColor:Integer;
    nItemHintNoFluteStoneColor:Integer;
    nUserHairOffsetX:Integer;
    nUserHairOffsetY:Integer;
    nOtherUserHairOffsetX:Integer;
    nOtherUserHairOffsetY:Integer;
    nHeroUserHairOffsetX:Integer;
    nHeroUserHairOffsetY:Integer;
    nUserHairOffsetX2:Integer;
    nUserHairOffsetY2:Integer;
    nOtherUserHairOffsetX2:Integer;
    nOtherUserHairOffsetY2:Integer;
    nHeroUserHairOffsetX2:Integer;
    nHeroUserHairOffsetY2:Integer;
    boNoMoveNewChrDlg:Boolean;
    boSaveMagicIconPosition:Boolean;
    boDisableDrogMagicIcon:Boolean;
    boDisableShowFireHitCDTime:Boolean;
    boItemHintNoShowZeroValue:Boolean;
    nMaxClientCount:Integer;
    boWaitStart:Boolean; // 小退后过段时间才可以点击  chongchong 2014-08-18
    boEnableCtrlZ:Boolean; // 启用ctrl+Z  chongchong 2014-08-18
    boShowProgressOnRun:Boolean; // 启动时显示进度条 chongchong 2014-08-18
    boShowLoadResProgress:Boolean;
    boPlayOldVerSound:Boolean;
    boShowUrlVerNotEqual:Boolean;
    sShowUrlVerNotEqual:string[99];
    wUpdateThreadCount:Word;
    wUpdateQueueCount:Word;
    sGameLoginVersion:string[10]; // 登录器版本号，用于登录网关检测

    OtherHintAlignment:TAlignment;
    ItemHintConfig:array[THintPropertyType] of TItemHintInfo;
    ItemHintTextConfig:array[THintTextType] of TItemHintTextInfo;
    {$IF GAMELOGIN_MUST_PLUG = 1}
    boMustGameLoginPlugFile:Boolean; // 是否载入登录器必备插件 chongchong 2017-08-12
    {$IFEND}
    ////////////////////////////////////////////////////////////////////////////

    nLoginMode:Byte; //20231027增加
  end;

  TBoxConfig = record
    sName:string;
    boUse:Boolean;
    nShape:Integer;
    boOpen:Boolean;
    dwOpenTick:LongWord;
    nStartFrame:Integer;
    nEndFrame:Integer;
    nCurrentFrame:Integer;
    nFrameTime:Integer;
    Item:TClientItem;
  end;

  TItemBoxConfig = record // 宝箱
    boUse:Boolean;
    boRotation:Boolean;
    boRotationOK:Boolean;
    dwRotationTick:LongWord;
    dwRotationTime:LongWord;
    nRotationCount:Integer;
    nCycleCount:Integer;
    nSelRotationIndex:Integer;
    nSelIndex:Integer;
    nSelEffectIndex:Integer;
    dwSelEffectTick:LongWord;
    nOpenMoveGold:Integer;
    nOpenMoveGameGold:Integer;
    Items:array[0..9 - 1] of TClientItem;
    ItemEffects:array[0..9 - 1] of TDrawItemEffect;
  end;

  TWineMatInfo = record // 斗酒用的6坛酒信息
    boDrink:Boolean; // 是否喝掉了
    boSelect:Boolean;
  end;

  pTWineMatInfo = ^TWineMatInfo;

  TTriangleInfo = record
    Pt1, Pt2, Pt3:TPoint;
  end;

  THumanRunConfig = record
    boCanRunHuman:Boolean;
    boCanRunMon:Boolean;
    boCanRunNpc:Boolean;
    boCanRunGuard:Boolean;
  end;

  PSortClientItem = ^TSortClientItem;

  TSortClientItem = record
    ItemIndex:Word;
    ClientItem:PTClientItem;
  end;

  THttpThread = class(TThread)
  private
    FPostType:Byte;
    FParam1:string;
    FParam2:string;
    FParam3:string;
    FParam4:string;
    procedure GetPayMentURL(PayType:string; PayPrice:string; currencytype:string);
    procedure CallPayMentURL(PayMode:Boolean; PayInfo:string);
    procedure Post(URL, Data:string; Res:TStream);

  protected
    function WebPagePost(sURL, sPostData:string):string; //HZQ 从 Private移动到此
    procedure Execute; override;
  public
    constructor Create(const btPostType:Byte; Param1:string; Param2:string; Param3:string; Param4:string);
  end;

  TImageList = class(TObject)
    ImagesArr:array of TGameImages;
  private
    procedure SetIndex(Index:Integer; Images:TGameImages);
    function GetCount:Integer;
  protected
    function ImageOf(Index:Integer):TGameImages; virtual;
    function IndexOf(Index:Integer):TGameImages; virtual;
    procedure Initialize; virtual;
    procedure Finalize; virtual;
    property Images[Index:Integer]:TGameImages read ImageOf;
    property Indexs[Index:Integer]:TGameImages read IndexOf write SetIndex;
  public
    constructor Create();
    destructor Destroy; override;
    procedure ClearCache; virtual;
    procedure FreeOldMemorys; virtual;
    function GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
    function GetCachedGrayImage(Index:Integer; var PX, PY:Integer):TTexture;
    function GetCachedBrightImage(Index:Integer; var PX, PY:Integer):TTexture;
    property Count:Integer read GetCount;
  end;

  TTilesList = class(TImageList)
  private
    HMapImagesArr:array of TGameImages;
  public
    destructor Destroy; override;
    procedure ClearCache; override;
    procedure FreeOldMemorys; override;
    procedure Initialize; override;
    procedure Finalize; override;
    function GetGameImages(Index:Integer; IsHMap:Boolean):TGameImages;
  end;

  TSmTilesList = class(TImageList)
  private
    HMapImagesArr:array of TGameImages;
  public
    destructor Destroy; override;
    procedure ClearCache; override;
    procedure FreeOldMemorys; override;
    procedure Initialize; override;
    procedure Finalize; override;
    function GetGameImages(Index:Integer; IsHMap:Boolean):TGameImages;
  end;

  TMonImageList = class(TImageList)
  protected
    function ImageOf(Index:Integer):TGameImages; override;
    function IndexOf(Index:Integer):TGameImages; override;
  public
    procedure Initialize; override;
    procedure Finalize; override;
    property Images;
    property Indexs;
  end;

  THumImageList = class(TImageList)
  protected
    function IndexOf(Index:Integer):TGameImages; override;
  public
    procedure Initialize; override;
    procedure Finalize; override;
    function GetWHumGrayImg(Dress, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
    function GetWHumBrightImg(Dress, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
    function GetWHumImg(Dress, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
    property Indexs;
  end;

  THumEffectList = class(TImageList)
  private
    FBaseIndex:Integer; // 扩展后的起始编号
    FMaxPicIndex:Integer; // 图片资源中的最大图片序列号 0 - 24;
    FCustomUnitIndex:Integer; // 自定义当文件的起始编号，大于这个值就转到 HumEffectImageDir 路径
  protected
    function IndexOf(Index:Integer):TGameImages; override;
  public
    constructor Create();
    procedure Initialize; override;
    procedure Finalize; override;
    function GetWHumEffectGrayImg(Effect, Sex, Frame:Integer; IsWeapon:Boolean; var Ax, Ay:Integer; NoSex:Boolean):TTexture;
    function GetWHumEffectBrightImg(Effect, Sex, Frame:Integer; IsWeapon:Boolean; var Ax, Ay:Integer; NoSex:Boolean):TTexture;
    function GetWHumEffectImg(Effect, Sex, Frame:Integer; IsWeapon:Boolean; var Ax, Ay:Integer; NoSex:Boolean):TTexture;
  end;

  TWeaponImageList = class(TImageList)
    WisImagesArr:array of TGameImages;
  protected
    function IndexOf(Index:Integer):TGameImages; override;
  public
    constructor Create();
    destructor Destroy; override;
    procedure ClearCache; override;
    procedure FreeOldMemorys; override;
    procedure Initialize; override;
    procedure Finalize; override;
    function GetWWeaponImg(Weapon, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
    function GetWWeaponGrayImg(Weapon, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
    function GetWWeaponBrightImg(Weapon, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
    property Indexs;
  end;

  TWeaponEffectList = class(TImageList)
  private
    FBaseIndex:Integer; // 扩展后的起始编号
    FMaxPicIndex:Integer; // 图片资源中的最大图片序列号 0 - 24;
  protected
    function IndexOf(Index:Integer):TGameImages; override;
  public
    constructor Create();
    procedure Initialize; override;
    procedure Finalize; override;
    function GetWWeaponEffectGrayImg(Effect, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
    function GetWWeaponEffectBrightImg(Effect, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
    function GetWWeaponEffectImg(Effect, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
    property Indexs;
  end;

  // 对应 CboWeaponEffect*.wzl piaoyun 2013-07-30
  TCboWeaponEffectList = class(TImageList)
  protected
    function IndexOf(Index:Integer):TGameImages; override;
  public
    procedure Initialize; override;
    procedure Finalize; override;
    property Indexs;
  end;

  // 对应 CboWeapon*.wzl piaoyun 2013-07-29
  TCboWeaponList = class(TImageList)
  protected
    function IndexOf(Index:Integer):TGameImages; override;
  public
    procedure Initialize; override;
    procedure Finalize; override;
    property Indexs;
  end;

  // 对应 cboHum*.wzl piaoyun 2013-07-29
  TCboHumList = class(TImageList)
  protected
    function IndexOf(Index:Integer):TGameImages; override;
  public
    procedure Initialize; override;
    procedure Finalize; override;
    property Indexs;
  end;

  // 对应 cboHumEffect*.wzl piaoyun 2013-07-29
  TCboHumEffect = class(TImageList)
  protected
    function IndexOf(Index:Integer):TGameImages; override;
  public
    procedure Initialize; override;
    procedure Finalize; override;
    property Indexs;
  end;

  TStateItemImages = class
    ImagesArr:array of TGameImages;
  private
    function GetCount:Integer;
    function ImageOf(Index:Integer):TTexture;
    function IndexOf(Index:Integer):TGameImages;
    function LooksOf(Index:Integer):TGameImages;
  protected
  public
    constructor Create();
    destructor Destroy; override;
    procedure ClearCache;
    procedure Initialize;
    procedure Finalize;
    procedure FreeOldMemorys;
    property Count:Integer read GetCount;
    function GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
    property Images[Index:Integer]:TTexture read ImageOf;
    property Indexs[Index:Integer]:TGameImages read IndexOf;
    property Looks[Index:Integer]:TGameImages read LooksOf;
  end;

  TDnItemImages = class
    ImagesArr:array of TGameImages;
  private
    function GetCount:Integer;
    function ImageOf(Index:Integer):TTexture;
    function IndexOf(Index:Integer):TGameImages;
    function LooksOf(Index:Integer):TGameImages;
    function GetCachedGray(Index:Integer):TTexture;
    function GetCachedBright(Index:Integer):TTexture;
  protected
  public
    constructor Create();
    destructor Destroy; override;
    procedure ClearCache;
    procedure Initialize;
    procedure Finalize;
    procedure FreeOldMemorys;
    property Count:Integer read GetCount;
    function GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
    property Images[Index:Integer]:TTexture read ImageOf;
    property Grays[Index:Integer]:TTexture read GetCachedGray;
    property Brights[Index:Integer]:TTexture read GetCachedBright;
    property Indexs[Index:Integer]:TGameImages read IndexOf;
    property Looks[Index:Integer]:TGameImages read LooksOf;
  end;

  TBagItemImages = class
    ImagesArr:array of TGameImages;
  private
    function GetCount:Integer;
    function ImageOf(Index:Integer):TTexture;
    function IndexOf(Index:Integer):TGameImages;
    function LooksOf(Index:Integer):TGameImages;
  protected
  public
    constructor Create();
    destructor Destroy; override;
    procedure ClearCache;
    procedure Initialize;
    procedure Finalize;
    procedure FreeOldMemorys;
    property Count:Integer read GetCount;
    function GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
    property Images[Index:Integer]:TTexture read ImageOf;
    property Indexs[Index:Integer]:TGameImages read IndexOf;
    property Looks[Index:Integer]:TGameImages read LooksOf;
  end;

  TNpcImageList = class(TImageList)
  protected
    function IndexOf(Index:Integer):TGameImages; override;
  public
    procedure Initialize; override;
    procedure Finalize; override;
    property Indexs;
  end;

  TNotifyEventEx = procedure(Sender:TObject) of object; stdcall;

  TEventInfo = record
    FileName:string;
    Initialize:TNotifyEventEx;
    Finalize:TNotifyEventEx;
    FreeOldMemorys:TNotifyEvent;
    GameImages:TGameImages;
  end;

  pTEventInfo = ^TEventInfo;

  TImageEvent = class
    m_EventList:TStringList;
    m_DynamicEventList:TStringList;
    m_ImageList:TStringList;
  private
    function GetCount:Integer;
    function GetImages(Index:Integer):TGameImages;
    function GetDynamicCount:Integer;
    function GetDynamicGameImages(Index:Integer):TGameImages;
  public
    constructor Create();
    destructor Destroy; override;
    procedure LoadGameImages;
    procedure UnLoadGameImages;
    procedure Add(GameImages:TGameImages);
    procedure AddDynamic(GameImages:TGameImages);
    procedure AddImageList(GameImages:TGameImages);
    procedure Clear;
    procedure ClearCache(Obj:Byte = 0);
    procedure Initialize();
    procedure Finalize();
    procedure FreeOldMemorys();
    property Images[Index:Integer]:TGameImages read GetImages;
    property Count:Integer read GetCount;
    property DynamicImages[Index:Integer]:TGameImages read GetDynamicGameImages;
    property DynamicCount:Integer read GetDynamicCount;
  end;

  TMapDesc = class
  private
    FStringList:THashedStringList;
  public
    constructor Create();
    destructor Destroy; override;
    procedure LoadFromFile(const FileName:string);
    function Get(const MapName:string; const BigMap:Boolean; var DescList:TList):Boolean;
  end;

  TWarrContinueHitManager = class(TObject)
  private
    FLastUseMagicID:Word;
    FLastUseMagicTick:LongWord;
  public
    constructor Create;
    function CanOpenMagic(MagicID:Word):Boolean;
    function CanUseMagic(MagicID:Word):Boolean;
    procedure UseMagic(MagicID:Word);
  end;

  TArrayPoint = array of TPoint; //HZQ 为自动生成地图挂机点增加定义

type  
    THttpClient = class
    private const
       UserAgent = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.111 Safari/537.36';
    public type
       TUri = record
          UserName:string;
          Password:string;
          Host:string;
          Protocol:string;
          Path:string;
          Doc:string;
          Param:string;
          Port:string;
          BookMark:string;
       end;
       PUri = ^TUri;
    private
       m_nRetCode:Integer;
       m_nConnectTimeOut:Integer;
       m_nSendTimeOut:Integer;
       m_nRecvTimeOut: Integer;
    public
       class function ParseURL(sUrl:string):TUri;
       class function URLEncode(const sUrl: UTF8String): string;
    protected
       class function RevPos(chSub:Char; const sContent:string):Integer; //反向查找字符

       function SetInternetSecurityOption(hNet:HINTERNET):Boolean;
       function GetInternetStatusCode(hUrl:HINTERNET):Integer;
       function SetInternetTimeout(hNet:HINTERNET):Boolean;
    public
       constructor Create;
       destructor Destroy; override;

       function Get(sUrl:string; streamRlt:TStream):Boolean;
       function Post(sUrl:string; sHeader, sBody:string; streamRlt:TStream):Boolean;

       property ResponseCode:Integer read m_nRetCode;
       property SendTimeOut:Integer read m_nSendTimeOut write m_nSendTimeOut;
       property RecvTimeOut:Integer read m_nRecvTimeOut write m_nRecvTimeOut;
       property ConnectTimeOut:Integer read m_nConnectTimeOut write m_nConnectTimeOut;
    end;

  //用户中心Http使用线程定义 
  TUserCenterManager = Class
  public type
     TUcProcesser = procedure(sParam:string) of object;
     TMethodWithParam = record
        Code:Pointer;
        Data:Pointer;
        sParam:String;
     end;
     PMethodWithParam = ^TMethodWithParam; 

     TRoleItem = record
        sName:string;
        nLevel:Integer;
        nJob:Integer;
     end;
     pRoleItem = ^TRoleItem;

     TSubAccountItem = record
         sSubAccount:string;
         sOldGameZone:string;
         sLastLoginTime:string;
         arrRole:array[0..2-1] of TRoleItem;
     end;
     PSubAccountItem = ^TSubAccountItem;

     TSubAccountInfo = record
         sGameZoneName:string;
         arrSubAccount:array of TSubAccountItem;
     end;
     PSubAccountInfo = ^TSubAccountInfo;

     TUcLoginMode = (ulmAccount, ulmPhone, ulmWechat, ulmNone);
  public const
     MSG_USER_CENTER_NOTIFY = WM_USER + 1024; //通过消息中转完成在UI线程中执行事件
     //WPARAM事件类型，LPARAM记录参数或参数结构体指针 0 = OK, -N, ErrCode
     UC_LOGIN_RET = 1;         //登录结果返回，LPARAM = 0 登录成功 连接错误LPARAM = -1, 服务器返回其他结果LPARAM = ResponseCode
     UC_PHONE_CODE_RET = 2;    //获取验证码结果 成功 LPARAM = 0，连接错误LPARAM = -1, 服务器返回其他结果LPARAM = ResponseCode
     UC_WEICHAT_CODE_RET = 3;  //获取二维码结果, 成功 LPARAM = 0, 连接错误LPARAM = -1, 服务器返回其他结果LPARAM = ResponseCode
     UC_SUB_ACCOUNT_RET = 4;   //获取二维码结果, 成功 LPARAM = 0, 连接错误LPARAM = -1, 服务器返回其他结果LPARAM = ResponseCode

  private const
     UC_CMD_REQUEST_PHONE_CODE = 1;
     UC_CMD_REQUEST_WECHAT_CODE = 21;
     UC_CMD_REQUEST_SUB_ACCOUNT = 32;

     UC_CMD_ACCOUNT_LOGIN = 11;
     UC_CMD_PHONE_LOGIN = 2;
     UC_CMD_WECHAT_LOGIN = 22;

  private
     m_csThread:TCriticalSection;
     m_SubAccount:TSubAccountInfo; //子账号列表信息
     m_sBaseUrl:string;
     m_sPhoneToken:string;             //请求手机验证码成功后，手机登录时，需要带上Token
     m_sWechatToken:string;            //请求微信验证码后，带上Token轮询
     //m_sWechatQrCodeUrl:string;
     m_msQrCode:TMemoryStream;
     m_sAccount:string;                //登录后取得的账号
     m_sSession:string;                //SessionID;
     m_nUID:Int64;                     //账号ID

     m_hWechatPollingThread:THandle; //微信轮询线程标记
     m_nWechatRequestExitFlag:Integer;
     m_nSASelectedIndex:Integer;
     m_nCanRequestWechatQrCode:Integer;

     m_nLoginMode:Integer;

     m_hMainFormHandle:THandle;
     m_dwLastGetPhoneCodeTick:DWORD;    //
     m_nWechatQrCodeBusyFlag:Integer;
     m_nPhoneCodeBusyFlag:Integer;
     m_nLoginBusyFlag:Integer;

     m_WechatQrCode:TTexture;
  protected
     procedure ReadSubAccountFromJson(joRet:ISuperObject);

     function GetWechatQrCodeBusyStatus():Boolean;
     function GetPhoneCodeBusyStatus():Boolean;
     function GetLoginBusyStatus():Boolean;

     {$IFDEF USE_IDHTTP}
     function PostJsonData(sJson: string; IdHTTP:TIdHTTP; streamRequest, streamResponse:TStringStream):Boolean;
     {$ELSE}
     function PostJsonData(sJson: string; http:THttpClient; streamRequest, streamResponse:TStringStream):Boolean;
     {$ENDIF}

     procedure OnThreadFished();
     procedure RequestPhoneVerifyCodeProc(sParam:string);
     procedure RequestWechatQrcodeProc(sParam:string);
     procedure WechatPollingProc(sParam:string);
     procedure RequestPhoneLoginProc(sParam:string);
     procedure RequestAccountLoginProc(sParam:string);
     procedure RequestSubAccountProc(sParam:string);


     function GetRoleString(pRole:pRoleItem):string;
     function GetSubAccountItemString(pItem:PSubAccountItem):string;
     function GetSubAccountCount():Integer;
     function GetSubAccountString(nIdx:Integer):string;

     function GetCanRequestWechatQrCode():Boolean;
     function GetLoginMode():TUcLoginMode;
     procedure SetLoginMode(nMode:TUcLoginMode);

  public
     constructor Create(); reintroduce;
     destructor Destroy(); override;

     function IsPhoneNumber(sPhone:string):Boolean;
     function TryLock:Boolean;
     procedure Lock;
     procedure Unlock;
     procedure DoUserCenterEvent(const msg:TMessage);
     procedure SetHostInfo(sHost:string; nPort:Integer);

     procedure RequestPhoneVerifyCode(sPhone:string; nCid:Integer);
     procedure RequestWechatQrcode(nCid:Integer);
     procedure RequestPhoneLogin(sPhone:string; sVerfiyCode:string; nCid:Integer);
     procedure RequestAccountLogin(sAccount:string; sPassword:string; nCid:Integer);
     procedure RequestSubAccountList(nCid:Integer; nGid:Integer);

     procedure StartWechatPolling();
     procedure StopWechatPolling();

     procedure ResetWechatQrCode();
     procedure UpdateWechatQrCode();

     function GetSubAccountItem(nIdx:Integer):PSubAccountItem;

  public
     property MainFormMsgHandle:THandle read m_hMainFormHandle write m_hMainFormHandle;
     property LastGetPhoneCodeTick:DWORD read m_dwLastGetPhoneCodeTick; // write m_dwLastGetPhoneCodeTick;
     property CanRequestWechatQRCode:Boolean read GetCanRequestWechatQrCode;
     property WechatQrCodeIsBusy:Boolean read GetWechatQrcodeBusyStatus;
     property PhoneCodeIsBusy:Boolean read GetPhoneCodeBusyStatus;
     property LoginIsBusy:Boolean read GetLoginBusyStatus;
     property Account:string read m_sAccount;
     property SACount:Integer read GetSubAccountCount;
     property SubAccountSelected:Integer read m_nSASelectedIndex write m_nSASelectedIndex;
     property SubAccountString[nIdx:integer]:string read GetSubAccountString;
     property UcLoginMode:TUcLoginMode read GetLoginMode write SetLoginMode;
     property WechatQrCode:TTexture read m_WechatQrCode write m_WechatQrCode; //
  end;

var
  g_sSelfFileName:string;
  g_sSelfFilePath:string;
  g_sSelfResourcePath:string;
  g_HumanRunConfig:THumanRunConfig = (
    boCanRunHuman:False;
    boCanRunMon:False;
    boCanRunNpc:False;
    boCanRunGuard:False
    );
  g_ModulesCRC:LongWord = 0;
  g_MonstersCRC:LongWord = 0;
  g_MagicsCRC:LongWord = 0;
  g_StdItemsCRC:LongWord = 0;
  g_ItemDescCRC:LongWord = 0;
  g_ItemDescTopCRC:LongWord = 0;
  g_TzItemDescCRC:LongWord = 0;
  g_FilterItemsCRC:LongWord = 0;
  g_EffectImagesCRC:LongWord = 0;
  g_SpecialCmdsCRC:LongWord = 0;
  g_PlugClientsCRC:LongWord = 0;
  g_BlackModulesCRC:LongWord = 0;
  g_NpcsCRC:LongWord = 0;
  g_DropItemEffectListCRC:LongWord = 0;
  g_EnabledAuctionItemListCRC:LongWord = 0;
  g_CustomItemPropertyCRC:LongWord = 0;
  g_CustomItemPropertyTextVarListCRC:LongWord = 0;
  g_ArrButtonConfigCRC:LongWord = 0;
  g_CustomMoneyCRC:LongWord = 0;
  g_sElementNewPropertyTexts:array[0..24] of string[40]; // 元素新属性文字 chongchong 2014-04-06

var
  g_AutoGJPoints:TList;
  g_FluteItem_UseBS:TClientItem; // 待镶嵌时选择的宝石
  g_boMySelItemCursorFromFluteStone:Boolean = False; // 准星光标是宝石搞的
  g_boMySelItemCursorFromHero:Boolean = False;
  g_StoneRemove_UseQZ:TClientItem; // 待拆下宝石时用的锤子
  g_SendStoneRemove_UseQZ:TClientItem;
  g_StoneRemoveItem:TMovingItem; // 正在拆下宝石的装备
  g_StoneRemoveItemEffect:TDrawitemEffect;
  g_SendStorageViewDlgItem:TClientItem;
  g_SendGetBackStorageViewItem:Boolean = False;
  MappingHandle:THandle = 0;
  InstanceInfo:PInstanceInfo = nil;
  MappingName:string = '';

  {$IF TESTMODE = 0}
  g_BaseUIStream:TMemoryStream = nil;
  g_ShareUIStream:TMemoryStream = nil;
  g_NewStateWindowUIStream:TMemoryStream = nil;
  g_ConfigDlgUIStream:TMemoryStream = nil;
  g_JSYUIStream:TMemoryStream = nil;
  {$IFEND}

  g_boIOCP_Rungate:Boolean = False;
  g_StdItemList:TList;
  g_SortStdItemList:TList;
  g_LastHintMakeIndex:Integer = -1;
  {
  .<Item:D:F:X:Y>
  d= 物品ID
  F= 数量
  X Y = 微调坐标 排版的
  鼠标放上去显示物品属性。类似<Img>图标的用法
  }

  g_UploadDllBuffer:PByte;
  g_UpdateDllBufferSize:Integer;
  g_UpdateDllBufferRealSize:Integer;

  // 客户端新增加按钮 -- 新功能测试用
  g_ButtonList:TList;
  g_NumberButtonList:TList;
  g_NewDlgList:TList;
  g_ArrButtonList:TList;
  // 客户端控件组 -- 新功能测试用
  g_ClientBuffs:array[0..49] of TClientBuffInfo;
  g_ArrBuffs:array[0..49] of TArrBuffInfo;
  g_boCheckBug:Boolean = False;
  g_UpdateEngine:TUpdateEngine = nil;
  g_MainHandle:THandle;
  g_sAdapterMac:string = ''; // 网卡MAC地址
  g_sUserMachineID:string = ''; // 用户机器码

  g_boFirstNewMapMsg:Boolean = True;
  g_sUpdateGateAddr:string = '127.0.0.1'; // 微端网关IP  piaoyun 2013-11-21
  g_nUpdateGatePort:Integer = 8001; // 微端网关端口 piaoyun 2013-11-21

  g_sUpdateAddr:string = '127.0.0.1'; // 微端更新地址
  g_nUpdatePort:Integer = 8000; // 微端更新端口  修改为9999
  g_boAutoUpdate:Boolean = False; // 微端自动更新  修改为False piaoyun 2013-11-21
  g_sUpdatePassword:string = 'GxxM2';

  // 微端下载速度相关 chongchong
  g_UpdateSizeLock:TCriticalSection;
  g_UpdateSize:Int64 = 0;
  g_UpdateTotalSize:Int64 = 0;
  g_LastRefreshUpdateProgress:LongWord = 0;
  g_UpdateSpeedStr:string = '0.00B/s';

  // 更新重试时间;  由于可能存在更新资源未返回的情况，所以要重试 chongchong 2016-01-10
  g_UpdateRetryTime:LongWord = 10000;
  g_ClientDataFile:string = '';
  g_sPromotionFlag:string = ''; // 推广标识

  g_RandomCodeSurface:array[TRandCodeType] of TTexture = (nil, nil, nil, nil);
  g_GameLoginConfigUrlMD5:string = ' ';
  g_nScreenCenterX:Integer;
  g_nScreenCenterY:Integer;

  {  g_TriangleUp: TTriangleInfo;
    g_TriangleRight: TTriangleInfo;
    g_TriangleDown: TTriangleInfo;
    g_TriangleLeft: TTriangleInfo;}

  g_nRenderCode:Integer;
  g_nRunCode:Integer;

  {$IF IsMultiThreadRender = 1}
  g_CriticalSection:TRTLCriticalSection;
  g_ActorLock:TRTLCriticalSection;
  {$IFEND}

  g_LockDebugOutStr:TRTLCriticalSection;
  g_FreeMemorysLock:TRTLCriticalSection;
  g_SendSocketLock:TRTLCriticalSection;
  g_boRestore:Boolean = False;
  g_boMinimized:Boolean = False;
  g_MinimizedTick:LongWord = 0;
  g_dwLastSendBufTick:LongWord = 0;
  g_boClientCanSend:Boolean = True;
  g_dwClientCanSendTick:LongWord = 0;
  g_SendStream:TMemoryStreamEx;
  g_nCheckTimeCount:Integer = 0;
  g_boHardware:Boolean = True; // 硬件加速
  g_boDepthStencil:Boolean = True; // 深度缓存

  g_boUseWeather:Boolean = True;
  g_boDoorStatus:Boolean = False;
  g_boViewFog:Boolean = False; // 是否显示黑暗
  g_boForceNotViewFog:Boolean = True; // 免蜡烛
  g_nDayBright:Integer = 0;
  g_nDarkLevel:Integer = 0;
  g_nDarkValue:Integer = 50;
  g_ConfigDlg:TGameConfigObject;

  g_dwRunIntervalTime:Integer = 0;
  g_dwRunTick:LongWord = 0;

  g_boCanDrawTileMap:Boolean = False;
  g_boRenderTargetTileMap:Boolean = True;
  g_boRenderTarget:Boolean = True;
  g_RenderTarget:array[0..3] of TTarget;
  g_CurrRenderTarget:TTarget = nil;

  // ------------------------------------------------------------------------------
  g_boShowItemName:Boolean = True;
  g_MerchantImageIndex:TDxImageIndex; // 图片位置
  g_MerchantCloseButtonRect:TRect;
  g_AttackModeList:TStringList;
  g_dwFreeActorTick:LongWord = 0;
  g_dwFreeOldMemorysTick:LongWord = 0;
  g_boLockBeltShortcut:Boolean = False;
  g_btStartPrintScreenNow:Byte = 0;
  g_nMaxFPS:Integer;
  g_sScreenCaptureFileName:string;
  g_sLogoText:string = 'The Return of Legend';
  g_sGoldName:string = '金币';
  g_sGameGoldName:string = '元宝';
  g_sGamePointName:string = '游戏点';
  g_sGameDiamondName:string = '金刚石';
  g_sGameGirdName:string = '灵符';
  g_sCreditPointName:string = '声望';
  g_sWarriorName:string = '战士'; // 职业名称
  g_sWizardName:string = '法师'; // 职业名称
  g_sTaoistName:string = '道士'; // 职业名称

  g_sUnKnowName:string = '未知';
  g_sMainParam1:string; // 读取设置参数
  g_sMainParam2:string; // 读取设置参数
  g_sMainParam3:string; // 读取设置参数
  g_sMainParam4:string; // 读取设置参数
  g_sMainParam5:string; // 读取设置参数
  g_sMainParam6:string; // 读取设置参数

  DScreen:TDrawScreen;
  WelcomeScene:TWelcomeScene;
  LoginScene:TLoginScene;
  SelectChrScene:TSelectChrScene;
  PlayScene:TPlayScene;
  LoginNoticeScene:TLoginNotice;
  Map:TMap;
  EventMan:TClEventManager;
  HintWindows:THintWindows;
  g_boCanAttack:Boolean = True;
  g_boCanMove:Boolean = True;
  g_dwSendActMsgTick:LongWord = 0;
  g_ActionCode:Integer;
  g_PlaySound:TPlaySound;
  // g_nVolume: Integer;
  g_BassSound:TBassSound;
  g_WisMainImages:TGameImages;
  g_WisMain2Images:TGameImages;
  g_WisMain3Images:TGameImages;
  g_WisDnItemImages:TGameImages;
  g_WisBagItemImages:TGameImages;
  g_WisStateItemImages:TGameImages;
  g_WisWeapon2Images:TGameImages;

  // 衣服、武器内观 piaoyun 2013-08-10
  g_StateEffectImages:TGameImages;
  g_StateEffectExImages:array[0..STATEEFFECTIMAGESFILE_Ex_Count - 1] of TGameImages;

  {------------------------------------------------------------------------------}
  g_cboHair:TGameImages;
  g_cboHair10:TGameImages;
  g_cboHair11:TGameImages;
  g_cboEffect:TGameImages;
  g_cboHumDiys:array[0..9] of TGameImages;
  g_cboHumEffectDiys:array[0..9] of TGameImages;
  g_cboWeaponDiys:array[0..9] of TGameImages;
  g_cboWeaponEffectDiys:array[0..9] of TGameImages;
  {------------------------------------------------------------------------------}

  g_NewopUI170TextureArray:array[0..9] of TTexture = (nil, nil, nil, nil, nil, nil, nil, nil, nil, nil);
  g_WLightImages:TGameImages;
  g_WNewopUIImages:TGameImages;
  //  {$IFDEF BEIJING}
  g_WMobileImages:TGameImages;
  //  {$ENDIF}
  g_WUIImages:TGameImages;
  g_WUI1Images:TGameImages;
  g_WUI2Images:TGameImages;
  g_WUI3Images:TGameImages;
  g_WMainImages:TGameImages;
  g_WMain2Images:TGameImages;
  g_WMain3Images:TGameImages;
  g_WChrSelImages:TGameImages;
  g_WMainImages16:TGameImages;
  g_WMain2Images16:TGameImages;
  g_WMain3Images16:TGameImages;
  g_WChrSelImages16:TGameImages;

  //205新界面专用 piaoyun 2013-10-12
  g_WUINImages:TGameImages;
  g_WNSelectImages:TGameImages;
  G_WUICommonImages:TGameImages;

  // 自定义5个界面图片库 piaoyun 2013-10-23
  g_NewUIImages:array[0..4] of TGameImages;
  g_NewUiFileNames:array[0..4] of string[50] = ('NewUI1.PAK', 'NewUI2.PAK', 'NewUI3.PAK', 'NewUI4.PAK', 'NewUI5.PAK');
  //g_NewUiPasswords: array[0..4] of string = ('GEEM2','GEEM2','GEEM2','GEEM2','GEEM2');

  // 扩展两个首饰内观效果 piaoyun 2013-08-01
  g_WHeadgearEffect:TGameImages;
  g_WHeadgearEffect2:TGameImages;
  g_WHeadgearEffect3:TGameImages;
  g_WHeadgearEffect4:TGameImages;
  g_WHeadgearEffect5:TGameImages;
  g_WHeadgearEffect6:TGameImages;
  g_WMMapImages:TGameImages;
  g_WMMapImages10:TGameImages;
  { TODO -c变量 -opiaoyun : 小地图扩展mmap11.wil从20001开始 【2013-4-23】}
  g_WMMapImages11:TGameImages;
  { TODO -c变量 -opiaoyun : 小地图扩展mmap12.wil从30001开始 【2013-4-23】}
  g_WMMapImages12:TGameImages;
  g_WAniTilesImages1:TGameImages;
  g_WTilesImages:TTilesList;
  g_WSmTilesImages:TSmTilesList;

  // g_WTilesImages2: TGameImages;
 // g_WSmTilesImages2: TGameImages;

  g_WHumWingImages:TGameImages;

  // g_WBagItemImages: TGameImages;
  // g_WDnItemImages: TGameImages;

  // g_WBagItemImages1: TGameImages;
  // g_WDnItemImages1: TGameImages;

  g_WHairImgImages:TGameImages;
  g_WHair2ImgImages:TGameImages;
  g_WHair3ImgImages:TGameImages;
  g_WHair4ImgImages:TGameImages;
  g_WHair5ImgImages:TGameImages;
  g_WHair6ImgImages:TGameImages;

  // 扩展的两个发型文件
  g_WHair10ImgImages:TGameImages;
  g_WHair11ImgImages:TGameImages;
  // /////////////////////////////

  g_WMagIconImages:TGameImages;
  g_WMagIcon2Images:TGameImages;
  g_WMagicImages:TGameImages;
  g_WMagic2Images:TGameImages;
  g_WMagic3Images:TGameImages;
  g_WMagic4Images:TGameImages;
  g_WMagic5Images:TGameImages;
  g_WMagic6Images:TGameImages;
  // g_WMagic7Images: TGameImages;
  g_WMagic8Images:TGameImages;
  g_WMagic9Images:TGameImages;
  g_WMagic10Images:TGameImages;
  g_WMagic7Images16:TGameImages;
  g_WMagic8Images16:TGameImages;
  g_WMagicreImages:TGameImages;
  g_WEventEffectImages:TGameImages;
  g_WDragonImg:TGameImages;
  g_WEffectImg:TGameImages;
  g_WEffectImg_EX:TGameImages;
  g_WEffectImg_SE:TGameImages;
  g_WMonEffectImg:TGameImages;

  // 扩展的天气特效文件[专用] -- piaoyun 2013-07-14
  g_WEffectWeatherImg:TGameImages;
  g_WShieldImg:TGameImages; // 盾牌 chongchong 2013-09-16

  g_WHorseImg:TGameImages; // 骑马 官方 chongchong 2013-10-12
  g_WHorseImg2:TGameImages; // 骑马 官方 chongchong 2013-10-12

  g_WLHorseImg:TGameImages; // 三方骑马 马 (L-Horse) chongchong 2013-10-17
  g_WLHorseImg1:TGameImages; // 三方骑马 马 (L-Horse1) chongchong 2013-11-01
  g_WLHorseImg2:TGameImages; // 三方骑马 马 (L-Horse2) chongchong 2014-04-18

  g_WLHorseEffectImg:TGameImages; // 三方骑马 马 (L-HorseEffect) chongchong 2013-10-17
  g_WLHorseEffectImg1:TGameImages; // 三方骑马 马 (L-HorseEffect1) chongchong 2013-11-01
  g_WLHorseEffectImg2:TGameImages; // 三方骑马 马 (L-HorseEffect2) chongchong 2014-04-18

  g_WLHorseHairImg:TGameImages; // 三方骑马 马上面人物头发 (L-HairHorse) chongchong 2013-10-17
  g_WLHorseHumImg:TGameImages; // 三方骑马 马上面人 (L-HumHorse) chongchong 2013-10-17
  g_WLHorseHumImg1:TGameImages; // 三方骑马 马上面人 (L-HumHorse1) chongchong 2014-04-18
  g_WLHorseHumImg2:TGameImages;
  g_WLHorseHumImg3:TGameImages;
  g_WLHorseHumImg4:TGameImages;
  g_WLHorseHumImg5:TGameImages;
  g_WLHorseHumImg6:TGameImages;
  g_WMonImages:TMonImageList;
  g_WStateItemImages:TStateItemImages;
  g_WBagItemImages:TBagItemImages;
  g_WDnItemImages:TDnItemImages;
  g_WHumImgImages:THumImageList;
  // 翅膀扩展 piaoyun 2013-07-28
  g_WHumEffectImages:THumEffectList;
  g_WWeaponImages:TWeaponImageList;
  g_WNpcImgImages:TNpcImageList;
  // 连击武器外观扩展 piaoyun 2013-07-29
  g_WCboWeaponList:TCboWeaponList;
  // 连击人物特效扩展 piaoyun 2013-07-29
  g_WCboHumEffect:TCboHumEffect;
  // 连击人物外观扩展 piaoyun 2013-07-29
  g_WCboHum:TCboHumList;
  // 新武器特效WeaponEffect-WeaponEffect5.wzl piaoyun 2013-07-29
  g_WeaponEffectList:TWeaponEffectList;
  // 新连击武器特效cboWeaponEffect-cboWeaponEffect5.wzl piaoyun 2013-07-30
  g_CboWeaponEffectList:TCboWeaponEffectList;
  g_WBmpUIImages:TUibImages;
  g_WBmpMapImages:TUibImages;
  g_WBmpBookImages:TUibImages;
  g_WObjectArr:array[0..65535] of TGameImages;
  g_HMapWObjectArr:array[0..65535] of TGameImages;

  // Mir 3
  g_EIMapTitleArr:array[0..72] of TGameImages;

  // 20 - 75为扩展自定义安全区光圈特效 chongchong 2017-04-18
  g_SafePointEffect:TGameImages;
  g_PowerBlock:TPowerBlock = (// 10
    $55, $8B, $EC, $83, $C4, $E8, $89, $55, $F8, $89, $45, $FC, $C7, $45, $EC, $E8, $03, $00, $00, $C7, $45, $E8, $64, $00, $00, $00, $DB, $45, $EC, $DB, $45, $E8, $DE, $F9, $DB, $45, $FC, $DE, $C9, $DD, $5D, $F0, $9B, $8B, $45, $F8, $8B, $00, $8B, $55, $F8, $89, $02, $DD, $45, $F0, $8B, $E5, $5D, $C3, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00);
  g_PowerBlock1:TPowerBlock = ($55, $8B, $EC, $83, $C4, $E8, $89, $55, $F8, $89, $45, $FC, $C7, $45, $EC, $64, $00, $00, $00, $C7, $45, $E8, $64, $00, $00, $00, $DB, $45, $EC, $DB, $45, $E8, $DE, $F9, $DB, $45, $FC, $DE, $C9, $DD, $5D, $F0, $9B, $8B, $45, $F8, $8B, $00, $8B, $55, $F8, $89, $02, $DD, $45, $F0, $8B, $E5, $5D, $C3, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00);

  // g_RegInfo          :TRegInfo;
  // g_boDrawTileMap: Boolean = True;
  // g_boDrawChr: Boolean = True;
// g_boDrawEff: Boolean = True;
 // g_boShowActorName: Boolean = True;
 // g_boOpenHealth: Boolean = True;

  g_FileStream:TFileStream = nil;
  g_nFileTextLen:Integer = 0;
  g_sFileText:string = '';
  g_sFileName:string;
  g_ServiceFileStream:TFileStream = nil;
  g_sServiceFileName:string;
  g_sServiceFileNameMD5:string;
  g_sServerName:string; // 服务器显示名称
  g_sServerMiniName:string; // 服务器名称
  g_sServerAddr:string = '127.0.0.1';
  g_nServerPort:Integer = 7000;
  g_nClientLoginMode:Integer = 0; //0 = 普通登录器登录、1 = 用户中心登录、2 = 定制登录器登录（要求隐藏注册账号和修改密码功能）
  g_nUcGameId:Integer = 0; // 用户中心选择的游戏分区ID
  g_UserCenterManager:TUserCenterManager = nil; //HZQ 20231019 用户中心功能管理
  g_sSelChrAddr:string;
  g_nSelChrPort:Integer;
  g_sRunServerAddr:string;
  g_nRunServerPort:Integer;
  g_boWindowMode:Boolean = False;
  g_boVSync:Boolean = False;
  g_nScreenWidth:Integer = 800;
  g_nScreenHeight:Integer = 600;
  g_nBitCount:Integer = 16;
  g_sRunGatePassword:string = 'GxxM2'; //HZQ 20230511
  // g_nGameLoginHandle:THandle;
  g_boSendLogin:Boolean; // 是否发送登录消息
  g_boServerConnected:Boolean;
  g_SoftClosed:Boolean; // 小退游戏
  g_ChrAction:TChrAction;
  g_ConnectionStep:TConnectionStep;
  g_boSound:Boolean = True; // 开启声音
  g_boBGSound:Boolean = True; // 开启背景音乐
  g_boRepeatBGSound:Boolean = True; // 重复背景音乐

  // 自动喊话功能 piaoyun 2013-09-11
  g_AutoSysMsg:Boolean; // 自动喊话
  g_AutoMsg:string[90] = ''; // 自动喊话内容
  g_AutoMsgTick:LongWord;
  g_AutoMsgTime:LongWord = 10000; // 自动喊话间隔

  g_FontArr:array[0..MAXFONT - 1] of string = ('宋体', '新宋体', '仿宋', '楷体', 'Courier New', 'Arial', 'MS Sans Serif', 'Microsoft Sans Serif');
  g_nCurFont:Integer = 0;
  g_sCurFontName:string = '宋体';
  g_boDeviceInitializeOK:Boolean;
  g_boChatStatus:Boolean;
  g_ImgMixSurface:TTexture = nil;
  g_MiniMapSurface:TTexture = nil;
  g_sPayMentQRCodeURL:string = '';
  g_PayMentQRCodeSurface:TTexture = nil;
  g_PayMentQRCodeSize:Integer = 5;
  g_boFirstTime:Boolean = False;
  g_sMapTitle:string;
  g_sMapTitleA:string;
  g_sMapName:string;
  g_sMapMusic:string;
  g_boMapHumAndHeroPercentHP:Boolean;
  g_boMapNight:Boolean;
  g_ServerList:TGStringList;
  g_MagicList:TGList; // 技能列表
  g_MagicNGList:TGList; // 内功技能列表
  g_ContinuousMagicList:TGList; // 连击技能列表

  g_AcupointLevels:array[0..4, 0..4] of Integer; // 打通穴道需要内功等级 piaoyun 2013-08-16

  g_GroupMembers:TList; // 组成员列表
  g_SaveItemList:TGList;
  g_MenuItemList:TGList;
  g_DropItemsMgr:TDropItemsMgr;
  g_ChangeFaceReadyList:TGList; //

  g_FreeActorList:TList; // 释放角色列表
  g_FreeEffectList:TList;
  g_FreeEventList:TList;
  g_SoundList:TStringList; // 声音列表
  g_SoundUpDateList:array of Boolean; // 声音更新状态列表

  g_nBonusPoint:Integer;
  g_nSaveBonusPoint:Integer;
  g_BonusTick:TNakedAbility;
  g_BonusAbil:TNakedAbility;
  g_NakedAbil:TNakedAbility;
  g_BonusAbilChg:TNakedAbility;
  g_Self_Abil_NoBouns:TNoBoundAbility; // 未加属性点时属性值 chongchong 2014-10-15
  g_nNoBounsMyHitPoint:Integer;
  g_nNoBounsMySpeedPoint:Integer;
  g_sGuildName:string; // 行会名称
  g_sGuildRankName:string; // 职位名称

  g_dwLastAttackTick:longword; // 最后攻击时间(包括物理攻击及魔法攻击)
  g_dwLastMoveTick:longword; // 最后移动时间
  g_dwLatestStruckTick:longword; // 最后弯腰时间

  g_boLatestSpell:Boolean; // 最后是魔法 chongchong 2016-10-10
  g_dwLatestSpellTick:longword; // 最后魔法攻击时间

  g_dwLatestNoGroupSpellTick:LongWord; // 最后魔法时间(不算合击)
  g_dwLatestFireHitTick:longword; // 最后列火攻击时间
  g_dwLatestRushRushTick:longword; // 最后被推动时间
  g_dwLatestHitTick:longword = 0; // 最后物理攻击时间(用来控制攻击状态不能退出游戏)
  g_dwLatestMagicTick:longword; // 最后放魔法时间(用来控制攻击状态不能退出游戏)
  g_dwLatestSwordHitTick:longword; // 最后逐日剑法攻击时间
  g_dwLatest42HitTick:longword; // 最后//破空剑 龙影剑法攻击时间
  g_dwLatest66HitTick:longword; // 最后开天斩攻击时间
  g_dwLatest113HitTick:LongWord;
  g_dwLatest115HitTick:LongWord;
  g_dwLatestTry66HitTick:LongWord; // 等待开启开天斩 chongchong 2015-03-11
  g_dwLatestTry42HitTick:LongWord;
  g_dwLatestTrySWordHitTick:LongWord;
  g_dwLatestTryFireHitTick:LongWord;
  g_dwLatestTry113HitTick:LongWord;
  g_dwLastTryCustomMagicHitTick:array[0..CUSTOM_MAGIC_COUNT] of LongWord;
  g_LastGroupAttackTick:LongWord = 0;
  g_dwMagicDelayTime:longword = 0;
  g_dwMagicPKDelayTime:longword;
  g_dwAutoCheckFireHitTick:longword = 0;
  g_dwAutoCheckSWordHitTick:longword = 0;
  g_nMMCurrX:Integer;
  g_nMMCurrY:Integer;
  g_nMouseCurrX:Integer; // 鼠标所在地图位置座标X
  g_nMouseCurrY:Integer; // 鼠标所在地图位置座标Y
  g_nMouseX:Integer; // 鼠标所在屏幕位置座标X
  g_nMouseY:Integer; // 鼠标所在屏幕位置座标Y
  g_nMoveMouseX:Integer; // 鼠标所在屏幕位置座标X
  g_nMoveMouseY:Integer; // 鼠标所在屏幕位置座标Y
  g_boMouseMoveDown:Boolean = False;
  g_boOpenMerchantBigDlg:Boolean = False;
  g_boKeepBigDlg:Boolean = False;


  {$IF TESTMODE = 1}
  g_nMouseMoveX:Integer;
  g_nMouseMoveY:Integer;
  {$IFEND}

  g_dwOpenMerchantBigDlgTick:DWORD;
  g_Mouse:TPoint;
  g_nMoveCount:Integer = 0;
  g_IsInMouseMove:Boolean = True;
  g_nTargetX:Integer; // 目标座标
  g_nTargetY:Integer; // 目标座标
  g_TargetCret:TActor; // 目标角色
  g_FocusCret:TActor; // 焦点锁定的角色
  g_FocusCretTick:LongWord = 0;
  g_MagicTarget:TActor;
  g_nMagicTargetRecogId:Int64;
  g_OldFocusCret:TActor;

  // 锁定目标，用来整锁定光圈 chongchong 2019-03-11 12:00:22
  g_LockTarget:TActor;
  g_dwLockTargetTick:LongWord;
  g_boGJRun:BOOL;
  g_nGJStartX, g_nGJStartY:Integer;
  g_boSendGJRunToRunGate:Boolean = False;
  g_boAttackSlow:Boolean; // 腕力不够时慢动作攻击.
  g_boMoveSlow:Boolean; // 负重不够时慢动作跑
  g_nMoveSlowLevel:Integer;
  g_boMapMoving:Boolean; // 甘 捞悼吝, 钱副锭鳖瘤 捞悼 救凳
  g_boMapMovingWait:Boolean;
  g_dwMapMovingWaitTick:LongWord = 0;
  g_boCheckBadMapMode:Boolean; // 是否显示相关检查地图信息(用于调试)
  // g_boCheckSpeedHackDisplay: Boolean;                                                               // 是否显示机器速度数据
  g_boViewMiniMap:Boolean; // 是否显示小地图
  g_nViewMinMapLv:Integer; // Jacky 小地图显示模式(0为不显示，1为透明显示，2为清析显示)
  g_nMiniMapIndex:Integer = -1; // 小地图号

  g_boMinMapTransparent:Boolean = False;

  // NPC 相关
  g_nCurMerchant:Int64; // 弥辟俊 皋春甫 焊辰 惑牢
  g_nMissionMerchant:Int64; // 任务NPC

  g_nMDlgX:Integer;
  g_nMDlgY:Integer; // 皋春甫 罐篮 镑
  g_dwChangeGroupModeTick:longword;
  g_dwDealActionTick:longword;
  g_dwQueryMsgTick:longword;
  g_nDupSelection:Integer;
  g_boAllowGroup:Boolean;

  // 人物信息相关
  g_nMySpeedPoint:Integer; // 敏捷
  g_nMyHitPoint:Integer; // 准确
  g_nMyAntiPoison:Integer; // 魔法躲避
  g_nMyPoisonRecover:Integer; // 中毒恢复
  g_nMyHealthRecover:Integer; // 体力恢复
  g_nMySpellRecover:Integer; // 魔法恢复
  g_nMyAntiMagic:Integer; // 魔法躲避
  g_nMyHungryState:Integer; // 饥饿状态

  g_nMyNPRecoverTime:Integer; // 增加内力恢复速度 %
  g_nMyNPRecoverPoint:Integer; // 内力恢复速度加几点

  g_nGameDiamond:LongWord; // 金刚石
  g_nGameGird:LongWord; // 灵符
  g_nGameGlory:Integer; // 荣誉
  g_nLoyaltyPoint:Integer; // 忠诚度

  // 英雄信息相关
  g_nHeroSpeedPoint:Integer; // 敏捷
  g_nHeroHitPoint:Integer; // 准确
  g_nHeroAntiPoison:Integer; // 魔法躲避
  g_nHeroPoisonRecover:Integer; // 中毒恢复
  g_nHeroHealthRecover:Integer; // 体力恢复
  g_nHeroSpellRecover:Integer; // 魔法恢复
  g_nHeroAntiMagic:Integer; // 魔法躲避
  g_nHeroHungryState:Integer; // 饥饿状态
  g_nHeroNPRecoverTime:Integer; // 增加内力恢复速度 %
  g_nHeroNPRecoverPoint:Integer; // 内力恢复速度加几点

  g_wAvailIDDay:Word;
  g_wAvailIDHour:Word;
  g_wAvailIPDay:Word;
  g_wAvailIPHour:Word;
  g_BrightActor:TActor;
  g_MySelf:THumActor;
  g_MyHero:THeroActor;
  g_MyDrawActor:THumActor;
  g_boMySelfLock:Boolean = False;
  g_boDelayGroupAttack:Boolean = False;
  g_dwDelayGroupAttackTick:LongWord = 0;
  g_HPMagicBallEffectInfo:TMagicBallEffectInfo; // HP魔法球 chongchong 2013-11-18
  g_MPMagicBallEffectInfo:TMagicBallEffectInfo; // MP魔法球 chongchong 2013-11-18

  g_HeroMagicList:TGList; // 技能列表
  g_HeroMagicNGList:TGList; // 内功技能列表
  g_HeroContinuousMagicList:TGList; // 连击技能列表

  g_BetterItemList:TList; //更好的装备

  g_ExtBagOpenItemCount:Word = 0;
  g_SaveItem1_6:array[0..5] of TClientItem;
  g_ItemArr:TClientBagItems;
  g_ItemArrEffect:array[0..ALL_BAG_ITEM_COUNT - 1] of TDrawItemEffect;
  g_OldItemArr:array[0..6 - 1] of TClientItem;
  g_HeroItemArr:TClientHeroBagItems;
  g_HeroItemArrEffect:array[0..MAX_HERO_BAG_ITEM - 1] of TDrawItemEffect;
  g_UseItems:TUseItems;
  g_HeroUseItems:TUseItems;
  g_UserState1:TUserStateInfo;

  // 玩家首饰盒中的首饰 chongchong 2013-10-20
  g_JewelryBoxItems:TJewelryBoxItems;

  // 英雄首饰盒中的首饰 chongchong 2013-10-20
  g_HeroJewelryBoxItems:TJewelryBoxItems;

  // 玩家封号 chongchong 2014-05-24
  g_FengHaoItems:TGList;
  g_ActiveFengHaoIndex:Integer;
  g_HeroFengHaoItems:TGList;
  g_HeroActiveFengHaoIndex:Integer;
  g_USFengHaoItems:TGList;
  g_USActiveFengHaoIndex:Integer;

  // 自定义OK框中的物品 chongchong 2013-10-31
  g_ItemBoxItems:TItemBoxItems;

  // 神佑袋中的物品 chongchong 2014-04-18
  g_GodBlessItems:TGodBlessItems;
  g_GodBlessItemsState:TGodBlessItemsState;

  // 英雄神佑袋中的物品 chongchong 2014-04-18
  g_HeroGodBlessItems:TGodBlessItems;
  g_HeroGodBlessItemsState:TGodBlessItemsState;

  // 宠物相关
  g_CurrentRecallGamePetIndex:Integer = -1;
  g_GamePetList:TGList; // 宠物列表
  g_PetItemArr:array[0..MAX_GAMEPET_BAG_COUNT - 1] of TClientItem;
  g_PetItemArrEffect:array[0..MAX_GAMEPET_BAG_COUNT - 1] of TDrawItemEffect;
  g_PetMagicEffect:TDrawItemEffect;
  g_StorageOpenStatus:array[0..3] of Boolean;
  g_UseItemsEffect:array[Low(TUseItems)..High(TUseItems)] of TDrawItemEffect;
  g_HeroUseItemsEffect:array[Low(TUseItems)..High(TUseItems)] of TDrawItemEffect;
  g_UseItemsEffect1:array[Low(TUseItems)..High(TUseItems)] of TDrawItemEffect;

  // 首饰盒中首饰外观特效 chongchong 2013-10-20
  g_JewelryBoxItemsEffect:array[Low(TJewelryBoxItems)..High(TJewelryBoxItems)] of TDrawItemEffect;
  g_HeroJewelryBoxItemsEffect:array[Low(TJewelryBoxItems)..High(TJewelryBoxItems)] of TDrawItemEffect;
  g_JewelryBoxItemsUS1Effect:array[Low(TJewelryBoxItems)..High(TJewelryBoxItems)] of TDrawItemEffect;
  g_GodBlessItemsEffect:array[Low(TGodBlessItems)..High(TGodBlessItems)] of TDrawItemEffect;
  g_HeroGodBlessItemsEffect:array[Low(TGodBlessItems)..High(TGodBlessItems)] of TDrawItemEffect;
  g_GodBlessItemsUS1Effect:array[Low(TGodBlessItems)..High(TGodBlessItems)] of TDrawItemEffect;

  // 自定义OK框中的物品内观特效 chongchong 2013-10-31
  g_ItemBoxItemsEffect:array[Low(TItemBoxItems)..High(TItemBoxItems)] of TDrawItemEffect;
  g_TempEffect:TDrawItemEffect;
  g_SelDeleteHumanInfo:TDeleteHumanInfo;
  g_DeleteHumanInfoArray:array[0..10 - 1] of TDeleteHumanInfo;

  ///////////////////////////////////////////////////////////////////////////// 2020-02-27 20:05:01
  // 购买角色信息列中
  g_SellPlayerShopItems:array[0..4] of TSellPlayerItem;
  g_SellPlayerItemArr:array of TClientItem; // 出售角色背包
  g_SellPlayerItemArrEffect:array[0..ALL_BAG_ITEM_COUNT - 1] of TDrawItemEffect;
  g_SellPlayerStorageItemList:TGList;
  g_SellPlayerStorageExtItemList:TGList;
  g_SellPlayerUserInfo:TSellPlayerUserInfo;
  g_SellPlayerUseItemsEffect:array[Low(TUseItems)..High(TUseItems)] of TDrawItemEffect;
  g_SellJewelryBoxItemsEffect:array[Low(TJewelryBoxItems)..High(TJewelryBoxItems)] of TDrawItemEffect;
  g_SellPlayerGodBlessItemsEffect:array[Low(TGodBlessItems)..High(TGodBlessItems)] of TDrawItemEffect;
  g_SellPlayerFengHaoItems:TGList;
  g_SellPlayerActiveFengHaoIndex:Integer;
  g_SellPlayerMagicList:TGList; // 技能列表
  g_SellPlayerMagicNGList:TGList; // 内功技能列表
  g_SellPlayerContinuousMagicList:TGList; // 连击技能列表
  g_SellPlayerGamePetList:TGList;
  g_SellPlayerPetItemArr:array[0..MAX_GAMEPET_BAG_COUNT - 1] of TClientItem;
  g_SellPlayerPetItemArrEffect:array[0..MAX_GAMEPET_BAG_COUNT - 1] of TDrawItemEffect;
  g_SellPlayerPetMagicEffect:TDrawItemEffect;
  g_boUploadClientPickItems:Boolean = False;
  g_dwUploadClientPickItemsTime:LongWord = 30000;
  g_dwUploadClientPickItemsTick:LongWord = 0;
  g_boBagLoaded:Boolean;
  g_boServerChanging:Boolean;

  //////////////////////////////////////////////////////////////////////////////
  // 卧龙 piaoyun 2013-08-20
  g_LieDragonNpcIndex:Integer;
  g_LieDragonPage:Integer;
  //////////////////////////////////////////////////////////////////////////////

  { 副本地图相关 chongchong 2013-09-10 }
  g_FBTime:Integer = -1;
  g_FBExitTime:Integer = -1;
  g_FBFailTime:Integer = -1;
  g_LastTimeTick:LongWord;
  g_sFBTime:string;
  g_sFBExitTime:string;
  g_sFBFailTime:string;
  g_MirrorMapTime:Integer = -1;
  g_sMirrorMapTime:string;
  g_TimeMapTime:Integer = -1;
  g_sTimeMapTime:string;
  // 自动shift开关 chongchong 2013-12-21
  g_boShift:Boolean = False;
  g_RungateCheckInfoTick:LongWord;
  g_RungateCheckInfoTime:LongWord;

  // 键盘相关
  g_ToolMenuHook:HHOOK;
  g_nLastHookKey:Integer;
  g_dwLastHookKeyTime:longword;
  g_nCaptureSerial:Integer; // 抓图文件名序号
  g_nSendCount:Integer; // 发送操作计数
  g_nReceiveCount:Integer; // 接改操作状态计数
  g_nTestSendCount:Integer;
  g_nTestReceiveCount:Integer;
  g_nSpellCount:Integer; // 使用魔法计数
  g_nSpellFailCount:Integer; // 使用魔法失败计数
  g_nFireCount:Integer; //
  g_nDebugCount:Integer;
  g_nDebugCount1:Integer;
  g_nDebugCount2:Integer;
  g_ActionRecvTick:Int64 = 0;
  g_LastActionIsHit:Boolean = False;

  // 买卖相关
  g_SellDlgItem:TClientItem;
  g_WantSellItems:Boolean = False;
  g_SellDlgItems:array[0..20] of TClientItem;
  g_WaitSellDlgItem:TClientItem;

  // 拍卖相关
  g_IsRequeryAuctionMyItemsWait:Boolean = False;
  g_AuctionDlgItem:TClientItem;
  g_IsAuctionDlgItemWait:Boolean = False;
  g_AuctionBuyItem:TAllAuctionItem;
  g_RefreshAuctionAllItemsTick:LongWord;
  g_AuctionAllItems:array[0..AUCTION_PAGE_COUNT - 1] of TAllAuctionItem;

  // 我的拍卖物品
  g_RefreshAuctionMyItemsTick:LongWord;
  g_AuctionMyItems:array[0..AUCTION_PAGE_COUNT - 1] of TMyAuctionItem;
  g_RefreshAuctionMyAttentionTick:LongWord;
  g_AuctionMyAttention:array[0..AUCTION_PAGE_COUNT - 1] of TAllAuctionItem;
  g_SellDlgItemSellWait:TClientItem;
  g_dwSellDlgItemSellWaitTick:LongWord = 0;
  g_DealDlgItem:TClientItem;
  g_boQueryPrice:Boolean;
  g_dwQueryPriceTime:longword;
  g_sSellPriceStr:string;

  // 交易相关
  g_DealItems:array[0..9] of TClientItem;
  g_DealRemoteItems:array[0..9] of TClientItem;
  g_DealItemsEffect:array[0..9] of TDrawItemEffect;
  g_DealRemoteItemsEffect:array[0..9] of TDrawItemEffect;
  g_nDealGold:Integer;
  g_nDealRemoteGold:Integer;
  g_boDealEnd:Boolean;
  g_sDealWho:string; // 交易对方名字
  g_MouseItem:TClientItem;
  g_MouseStateItem:TClientItem;
  g_MouseUserStateItem:TClientItem;
  g_MouseHeroItem:TClientItem;
  g_MouseHeroStateItem:TClientItem;
  g_MouseHeroUserStateItem:TClientItem;

  // 元宝交易相关
  g_GameGoldDealItems:array[0..8] of TClientItem;
  g_GameGoldDealRemoteItems:array[0..8] of TClientItem;
  g_GameGoldDealItemsEffect:array[0..8] of TDrawItemEffect;
  g_GameGoldDealRemoteItemssEffect:array[0..8] of TDrawItemEffect;
  g_GameGoldDeal:TGameGoldDeal;
  g_nDealGameDiamond:Integer;
  g_boGameGoldDealing:Boolean;
  g_dwGameGoldDealTick:LongWord;
  g_UpgradeItemArr:array[0..2] of TClientItem;
  g_WaitingUpgradeItemArr:array[0..2] of TClientItem;
  g_UpgradeItemArrEffect:array[0..2] of TDrawItemEffect;

  //////////////////////  挑战相关 piaoyun 2013-07-22  /////////////////////////
  g_ChallengeDlgItem:TClientItem;
  g_ChallengeItems:array[0..3] of TClientItem;
  g_ChallengeRemoteItems:array[0..3] of TClientItem;
  g_ChallengeItemsEffect:array[0..3] of TDrawItemEffect;
  g_ChallengeRemoteItemsEffect:array[0..3] of TDrawItemEffect;
  g_nChallengeGold:Integer = 0;
  g_nChallengeRemoteGold:Integer = 0;
  g_nChallengeGameDiamond:Integer = 0;
  g_nChallengeRemoteGameDiamond:Integer = 0;
  g_btChallengeGoldIndex:Byte; // 附加币的类型 0金刚石 1元宝 2灵符

  g_boChallengeEnd:Boolean = False;
  g_sChallengeWho:string = ''; // 挑战对方名字
  g_dwChallengeActionTick:Longword = 0;

  //////////////////////////////////////////////////////////////////////////////

  g_boCheckDropItem:Boolean = False; // 外挂检测

  g_boItemMoving:Boolean; // 正在移动物品
  g_MovingItem:TMovingItem;
  g_WaitingUseItem:TMovingItem;
  g_WaitingHeroUseItem:TMovingItem;
  g_DropMoveItemTick:LongWord = 0;
  g_boMagicIconDown:Boolean;
  g_ptMagicIconDownPt:TPoint;
  g_boMagicMoving:Boolean;
  g_MovingMagic:PTClientMagic;
  //g_RemoveStoneItem: TMovingItem;                                                                   // 正在拆下镶嵌宝石的物品

  g_FocusItem:pTDropItem;
  g_OldFocusItem:pTDropItem;
  g_nAreaStateValue:Integer; // 显示当前所在地图状态(攻城区域、)

  g_boNoDarkness:Boolean;
  g_nRunReadyCount:Integer; // 助跑就绪次数，在跑前必须走几步助跑

  g_nUnPakItemMakeIndex:Integer = 0;
  g_nAutoUnPakItemMakeIndex:Integer = 0;
  g_EatingItem:TClientItem;
  g_EatingItemIndex:Integer = -1;
  g_dwEatTime:LongWord; // timeout...
  g_IsStopEat:Boolean = False;
  g_dwStopEatTime:LongWord = 0;
  g_PetUseingItem:TClientItem;
  g_dwPetUseingTime:LongWord;
  g_AutoEatingItem:TClientItem;
  g_dwAutoEatTime:LongWord; // timeout...

  g_HeroEatingItem:TClientItem;
  g_dwHeroEatTime:LongWord; // timeout...

  g_dwDizzyDelayStart:longword;
  g_dwDizzyDelayTime:longword;
  g_boDoFadeOut:Boolean;
  g_boDoFadeIn:Boolean;
  g_nFadeIndex:Integer;
  g_boDoFastFadeOut:Boolean;
  g_boAutoDig:Boolean; // 自动锄矿
  g_boSelectMyself:Boolean; // 鼠标是否指到自己

  // 游戏速度检测相关变量
  g_dwFirstServerTime:longword;
  g_dwFirstClientTime:longword;
  g_dwLatestClientTime2:longword;
  g_dwFirstClientTimerTime:longword; // timer 矫埃
  g_dwLatestClientTimerTime:longword;
  g_dwFirstClientGetTime:longword;
  g_dwLatestClientGetTime:longword;
  g_nTimeFakeDetectSum:Integer;
  g_nTimeFakeDetectTimer:Integer;
  g_dwLastestClientGetTime:longword;

  // 外挂功能变量开始
  g_dwDropItemFlashTime:longword = 5 * 1000; // 地面物品闪时间间隔
  g_nHitTime:Integer = 1400; // //1400; //攻击间隔时间间隔
  g_nItemSpeed:Integer = 60;
  g_dwSpellTime:longword = 600; // 500; //魔法攻间隔时间

  g_boShowAllItem:Boolean = False; // 显示地面所有物品名称

  { g_boAutoMagic: Boolean = False;
   g_dwAutoMagicTick: LongWord;
   g_nAutoMagicTime: Integer = 5;

   g_boAutoHideMode: Boolean = False;
   g_dwAutoHideModeTick: LongWord;
   g_nAutoHideModeTime: Integer = 5;

   g_boSmartLongHit: Boolean = True;
   g_boSmartWideHit: Boolean = False;
   g_boSmartFireHit: Boolean = False;
   g_boAutoSwordHit: Boolean = False;
   g_boAutoShield: Boolean = False;  }

  g_nMagicItemRate:Integer = 100;
  g_nMagicItemType:Integer = 1;
  g_nFireHitDelayTime:Integer = 20000;
  g_nKTZHitDelayTime:Integer = 20000;
  g_n42HitDelayTime:Integer = 20000;
  g_nSwordHitDelayTime:Integer = 20000; // 逐日剑法
  g_nDKZHitDelayTime:Integer = 15000; // 断空斩
  g_nXHYJHitDelayTime:Integer = 15000; // 血魄一击(战)

  g_DeathColorEffect:TColorEffect = ceGrayScale;
  g_dwRenewHPTick:Longword;
  g_dwRenewMPTick:Longword;
  g_dwRenewHeroHPTick:Longword;
  g_dwRenewHeroMPTick:Longword;
  g_dwRenewSpecialHPTick:Longword;
  g_dwRenewSpecialMPTick:Longword;
  g_dwRenewHeroSpecialTick:Longword;
  g_dwRenewBookTick:Longword;
  g_dwRenewHumBookHPTick:Longword;
  g_dwRenewHeroLogOutTick:Longword;
  g_dwRenewSelfLogOutTick:Longword;
  g_dwHintItemDuraTick:Longword;
  g_ConfigClient:TConfigClient;

  // 凹槽宝石属性
  g_AoCaoBaoShiAttTypes:array[1..18] of THintTextType;
  g_AutoCustomMagic:array[TMagicWarrNGOption] of TAutoCustomMagics;

  // g_AppFileStream: TFileStream = nil;
  // 外挂功能变量结束
  g_dwStartAutoPickupTick:longword;
  g_nStartPickupX, g_nStartPickupY:Integer;
  g_dwAutoPickupTime:longword = 100; // 自动捡物品间隔
  g_AutoPickupList:TList;
  g_MagicLockActor:TActor;
  g_dwAutoHideModeTick:longword;
  g_dwAutoHumShieldTick:longword;
  g_dwHumStruckShieldTick:longword;
  g_boNextTime113Hit:Boolean; // 断空斩 chongchong 2018-01-29
  g_boNextTime115Hit:Boolean; // 血魄一击(战) chongchong 2018-01-29
  g_boNextTime60Hit:Boolean;
  g_boNextTimeSwordHit:Boolean; // 逐日剑法
  g_boNextTime66Hit:Boolean; // 开天斩重击 piaoyun 2013-08-24
  g_boNextTime66Hit1:Boolean; // 开天斩轻击 piaoyun 2013-08-24

  g_boNextTime42Hit:Boolean; // 破空剑 龙影剑法

  g_boNextTimePowerHit:Boolean;
  g_boCanLongHit:Boolean;
  g_boCanWideHit:Boolean;
  g_boCanCrsHit:Boolean;
  // g_boCanTwnHit: Boolean;
  boNextTime43Hit:Boolean;
  g_boNextTimeFireHit:Boolean;
  g_boNextTimeCustomMagicList:TList;
  g_boHeroGroupHit:Boolean;
  g_nHeroGroupHit:Integer;
  g_ItemDescList:TGHashStringList; // 物品说明
  g_ItemDescTopList:TGHashStringList;
  g_TzItemDescList:TGStringList; // 套装物品说明
  g_SkillDescList:TGHashStringList;
  g_SkillUpgradeDescList:TGStringList;
  g_GodBlessItemList:TGHashStringList;
  g_FengHaoItemList:TGHashStringList;
  g_PlugFileNameList:TStringList = nil;
  g_UnbindItemList:TList = nil;
  g_CustomUnbindItemList:TList = nil; // 内挂自定义物品
  g_SpecialCmdList:TList = nil;
  g_CustomMonsterConfig:TList = nil;
  g_CustomMagicConfig:TList = nil;
  g_CustomNpcConfig:TList = nil;
  g_DropItemEffectList:TDropItemEffectList = nil;
  g_EnabledAuctionItemList:TStringList = nil;
  g_ImageFileList:TStringList = nil;
  g_DataFileList:TStringList = nil;
  g_MapFileList:TStringList = nil;
  g_WavFileList:TStringList = nil;

  //g_HMapDataFileList: TStringList = nil;

  // 内挂保护物品
  g_NGProtectItems:TStringList = nil;

  // 内挂默认BOSS列表
  g_NGBossList:TStringList = nil;
  g_InputBoxFilterList:TStringList = nil;

  // Resources目录自定义 piaoyun 2013-09-04
  g_ResourcesDir:string = 'Resources';

  {$IF TESTMODE = 2}
  g_PakDefaultPassword:string; //HZQ 去除原有的定制模式
  {$IFEND}

  // g_boDrawTileMap: Boolean = True;
  g_boDrawDropItem:Boolean = True;
  g_nTestX:Integer = 71;
  g_nTestY:Integer = 212;
  g_dwProcessInterval:Integer = 2;
  g_dwProcessTime:Longword;
  g_dwRunTime:Longword;
  g_boShowBagInfo:Boolean = False;
  g_boShowHeroBagInfo:Boolean = False;

  { TODO -c变量 -opiaoyun : 增加两个变量控制英雄和人物装备栏显示文字规则 【2013-4-24】}
  g_boShowItemInfo:Boolean = False;
  g_boShowHeroItemInfo:Boolean = False;
  g_boLoadUserConfig:Boolean = False;
  g_nAttactkMode:Integer = -1; // 攻击模式
  g_boMissionButonFlash:Boolean = True;
  g_dwMissionButonTick:LongWord;
  g_nMissionButonFaceIndex:Integer = 0;
  g_ShopItems:array[0..4, 0..9] of TShopClientItem;
  g_ShopItems1:array[0..4] of TShopClientItem;
  g_ShopItemsEffect:array[0..4, 0..9] of TDrawItemEffect;
  g_ShopItemsEffect1:array[0..4] of TDrawItemEffect;
  g_MouseShopItems:TShopClientItem;
  g_nShopPictureFaceIndex:Integer;
  g_dwShopPictureTick:LongWord;
  g_ShopTablePages:array[0..4] of Integer;
  g_ShopTablePageCounts:array[0..4] of Integer;
  g_nShopPageCount:Integer;
  g_nRankingsTablePage:Integer = 0;
  g_nRankingsTableType:Integer = 0;
  g_nRankingsPage:Integer = -1;
  g_nRankingsPageCount:Integer = 0;
  g_UserLevelRankings:array[0..9] of TUserLevelRanking;
  g_HeroLevelRankings:array[0..9] of THeroLevelRanking;
  g_UserMasterRankings:array[0..9] of TUserMasterRanking;
  g_dwAutoFindPathTick:LongWord;
  g_nMinMapMoveX:Integer;
  g_nMinMapMoveY:Integer;
  g_nMinMapX:Integer;
  g_nMinMapY:Integer;
  g_boShowMiniMapXY:Boolean;
  g_dwAngryTick:LongWord = 0;
  g_nAngryFaceIndex:Integer = 0;
  g_dwAutoOrderItemTick:LongWord;
  g_dwMoveItemTick:LongWord;
  g_boReSelConnect:Boolean = False;
  g_dwReSelConnectTick:LongWord;
  g_ClientRect:TRect;
  g_dwShowVersionBmpTick:LongWord = 0; // 显示LOGO已过去时间 piaoyun 2013-08-28
  g_dwShowVersionBmpTime:LongWord = 1000 * 3; // 显示LOGO时长 piaoyun 2013-08-28
  g_boShowNotice:Boolean = False;
  g_boShowClientDataFile:Boolean = False; // 显示客户端请求数据进度 chongchong 2015-01-07
  g_nShowClientDataFileProgress:Integer = 0; // 请求数据进度 chongchong 2015-01-07

  g_MustPlugFile:Boolean = False;
  g_dwGameLoginHandle:THandle;
  g_ClientParam:TClientParam;
  g_boShowVersionBmp:Boolean = False;
  g_nMapWidth:Integer = 0;
  g_nMapHeight:Integer = 0;
  g_MapDesc:TMapDesc;
  g_BackImage:TDIB = nil;
  g_UseCustomDefCursor:Boolean = False;
  g_CustomCursorDefFileName:string = 'DefCursor';
  g_UseCustomMountCursor:Boolean = False;
  g_CustomCursorMountFileName:string = 'MountCursor';
  g_UseCustomUnmountCursor:Boolean = False;
  g_CustomCursorUnmountFileName:string = 'UnmountCursor';
  LegendMap:TLegendMap;
  g_NotFilterMsg:array[0..4] of Boolean = (True, True, True, True, False);
  g_dwAutoSayMsgTick:LongWord;
  g_sAutoSayMsg:string;
  g_ClientVersion:TClientVersion = cvSerial; // 版本 0=热血传奇 1=传奇归来 2=传奇外传

  g_boAppExit:Boolean = False;
  g_WaitAppExit:Boolean = False;
  g_RecvBufTick:LongWord;
  g_MissionPageCaptionList:TStringList;
  g_EffectImageList:TGStringList;
  g_ImageEvent:TImageEvent;
  g_nShortMessage:Integer = 0;
  g_nCharDesc:Integer = 0;
  g_nMessageBodyL:Integer = 0;
  g_nMessageBodyW:Integer = 0;
  g_nMessageBodyWL:Integer = 0;
  g_nHumFeature:Integer = 0;
  g_nMonFeature:Integer = 0;
  g_RenderEventList:TStringList = nil;
  g_dwRenderEvent:LongWord = 0;
  g_UserShops:array[0..7] of TClientUserShop;
  g_UserShopItems:array[0..4] of TClientUserShopItem;
  g_SearchUserShopItems:array[0..4] of TClientUserShopItem;
  g_MyShopItems:array[0..2, 0..4] of TClientUserShopItem;
  g_UserShopItemEffects:array[0..4] of TDrawItemEffect;
  g_SearchUserShopItemEffects:array[0..4] of TDrawItemEffect;
  g_MyShopItemEffects:array[0..2, 0..4] of TDrawItemEffect;

  {piaoyun 注释 2013-07-21
  g_boOpenDGameShopDlg: Boolean = False;
  g_boOpenDUserShopDlg: Boolean = False;
  g_boOpenDMyShopDlg: Boolean = False;
  }

  g_nUserShopIndex:Integer = 0;
  g_nUserShopCount:Integer = 0;
  g_nSearchUserShopItemIndex:Integer = 0;
  g_nSearchUserShopItemCount:Integer = 0;
  g_nUserShopItemIndex:Integer = 0;
  g_nUserShopItemCount:Integer = 0;
  g_MyShopItemIndexs:array[0..2] of Integer;
  g_MyShopItemCounts:array[0..2] of Integer;
  g_SelectMyShopItems:array[0..2] of TClientUserShopItem;
  g_SelectMyShopItem:pTClientUserShopItem = nil;
  g_SelectMyShopItemEffect:TDrawItemEffect;
  g_MyShopDlgItem:TClientItem;
  g_MyShopDlgItemWait:TClientItem;
  g_MyShopDlgItemEffect:TDrawItemEffect;
  g_SelectMyShopItemWait:TClientItem;
  g_SelectUserShop:TClientUserShop;
  g_dwOpenDGameShopDlgTime:LongWord = 0;
  g_dwQueryUserShopTime:LongWord = 0;
  g_dwQueryUserShopItemTime:LongWord = 0;
  g_dwQueryMyShopItemTime:LongWord = 0;
  g_dwQueryBusinessTime:LongWord = 0;
  g_sActionName:string = '';
  g_nTop, g_nLeft:Integer;
  g_PKey:PInteger = nil; // PassWord

  g_boMouseRightDown:Boolean = False;
  g_BoxConfig:TBoxConfig = (// 箱子
    sName: '';
    boUse:False;
    nShape:0;
    boOpen:False;
    dwOpenTick:0;
    nStartFrame:0;
    nEndFrame:0;
    nCurrentFrame:0;
    );
  g_ItemBoxConfig:TItemBoxConfig; // 宝箱物品

  g_dwItemBoxLuckEffectTick:LongWord = 0;
  g_nItemBoxLuckEffectCount:Integer = 0;
  g_OpenBoxItemWait:TClientItem;

  // 请酒
  g_PleaseDrinkItem:array[0..1] of TClientItem;
  g_PleaseDrinkItemWait:array[0..1] of TClientItem;
  g_BotWineMatMouseEnter:array[0..1] of Boolean;

  // 斗酒
  g_boGuessfinger:Boolean = False; // 是否可以继续斗酒
  g_nDrinkIcon:Integer;
  g_nMouseEnterSelectWineIndex:Integer = -1;
  g_BotWinejarMouseEnter:array[0..5] of Boolean;
  g_DrinkItems:array[0..5] of Boolean; // 6坛酒是否喝掉

  g_nSelectWineIndex:Integer = -1; // NPC选酒序号
  g_nNpcSelectWineTime:Integer;
  g_dwNpcSelectWineTick:LongWord;
  g_nWineMatCount:Integer = 6;
  g_nGuessfingerResult:Integer = -1; // 猜拳结果
  g_nNpcfinger:Integer; // NPC的石头剪刀布
  g_nSelffinger:Integer; // 自己的石头剪刀布
  g_boNpcSelectWine:Boolean = False; // NPC开始选酒
  g_boSelfSelectWine:Boolean = False; // 自己开始选酒
  g_nSelectfinger:Integer = -1; // 选择石头剪刀布
  g_boIsSelectWine:Boolean = False; // 是否选完酒
  g_dwPlayGuessfingerTick:LongWord = 0; // 播放猜拳动作
  g_dwBotfingerDirectPaintTick:LongWord = 0; // 绘制拳头一闪一闪效果
  g_nBotfingerDirectPaintIndex:Integer = 0;
  g_boPlayDrink:Boolean = False; // 播放喝酒动作
  g_boPlayDrinkObject:Boolean = False; // 播放对象 True=NPC False=Self
  g_btPlayDrinkDrunk:Byte = 0; // 是否播放喝醉的动作
  g_dwPlayDrinkTick:LongWord = 0; // 播放喝醉TICK
  g_nPlayDrinkImageCount:Integer = 0; // 播放图片数量
  g_nNpcDrunkValue:Integer = 0; // NPC醉酒度  最大100
  g_nSelfDrunkValue:Integer = 0; // 自己醉酒度 最大100

  g_dwDBotHeroAppraisalStartEffectTick:LongWord = 0;
  g_nDBotHeroAppraisalStartEffectIndex:Integer = 0;
  g_boOpenLastContinuous:Boolean = False; // 第四个连击是否开启
  g_boHeroOpenLastContinuous:Boolean = False; // 第四个连击是否开启
  g_ContinuousMagicOrder:array[0..3] of Byte; // 连击顺序     0=空 1=随机
  g_HeroContinuousMagicOrder:array[0..3] of Byte; // 连击顺序  0=空 1=随机

  g_dwDrawAcupointTick:LongWord = 40;
  g_nAcupointTicks:Integer = 0;
  g_dwHeroDrawAcupointTick:LongWord = 40;
  g_nHeroAcupointTicks:Integer = 0;
  g_boContinuous:Boolean = False; // 是否正在连击
 
  g_dwContinuousTick:LongWord = 0;
  g_CurrContinuousMagic:TClientMagic; // 当前正在使用的连击魔法
  g_ContinuousMagic:array of TClientMagic; // 连击魔法
  g_nContinuousIndex:Integer = 0; // 连击魔法序号  
  g_boCanUseContinuous:Boolean = True; // 是否可以使用连击
  g_nContinuousStatus:Integer = 0;

  g_nBotPlusAbilFlashIndex:Integer = 218; // 属性点闪烁动画         piaoyun 2013-08-17
  g_dwBotPlusAbilFlashTick:LongWord = 0; // 属性点闪烁时间控制     piaoyun 2013-08-17

  g_boOfflineCloseMyShop:Boolean = False;
  g_ServerPlugFileMD5List:TStringList;
  g_boGetPlugFileMD5OK:PBoolean;
  g_boCheckModule:PBoolean;
  g_boAddModule:PBoolean;
  g_dwCheckModuleTick:LongWord;
  g_boFullScreenDrawScene:Boolean = False;
  g_MyTargetList:THashedStringList;
  g_MyBlacklist:THashedStringList;

  // ------------------------------------------------------------------------------
  g_nHeroM2SelectRecogId:Int64 = 0;
  g_HeroM2SelectActor:TActor = nil;
  g_HeroM2SelectShopItem:TClientItem;
  g_HeroM2ShopWaiting:TClientItem;
  g_HeroM2ShopMouseItem:TClientItem;
  g_HeroM2ShopItems:array[0..20 - 1] of TClientItem;
  g_HeroM2ShopItemEffects:array[0..20 - 1] of TDrawItemEffect;
  g_HeroM2ShopRemoteMouseItem:TClientItem;
  g_HeroM2ShopRemoteItems:array[0..20 - 1] of TClientItem;
  g_HeroM2ShopRemoteItemEffects:array[0..20 - 1] of TDrawItemEffect;
  dwUseMagic:LongWord = 0;
  g_sUserMoveCmd:string = '';

  {$IF DEBUG_RUN_DELAY_SELF = 1}
  // 在聊天框中输入 /clearlog 清空数据
  g_sSelfRunSendMsgFile:string = 'd:\self_run_sendmsg.txt';
  g_sSelfRunRecvMsgFile:string = 'd:\self_run_recvmsg.txt';
  g_sSelRunStartFile:string = 'd:\self_run_strart.txt';
  g_sSelfRunEndFile:string = 'd:\self_run_end.txt';
  g_SelfRunSendMsg:TStringList;
  g_SelfRunRecvMsg:TStringList;
  g_SelfRunStart:TStringList;
  g_SelfRunEnd:TStringList;
  {$IFEND}

  {$IF DEBUG_RUN_DELAY_OTHER = 1}
  g_sOtherRunMsgFile:string = 'd:\other_run_msg.txt';
  g_sOtherRunStartFile:string = 'd:\other_run_start.txt';
  g_sOtherRunEndFile:string = 'd:\other_run_end.txt';
  g_OtherRunStart:TStringList;
  g_OtherRunMsg:TStringList;
  g_OtherRunEnd:TStringList;
  {$IFEND}

  {$IF DEBUG_HIT_DELAY = 1}
  g_sSelfHitStartFile:string = 'd:\self_hit_start.txt';
  g_sSelfHitEndFile:string = 'd:\self_hit_end.txt';
  g_SelfHitStart, g_SelfHitEnd:TStringList;
  {$IFEND}

  {$IF DEBUG_SPELL_DELAY = 1}
  g_sSelfSpellStartFile:string = 'd:\self_Spell_start.txt';
  g_sOtherSpellStartFile:string = 'd:\other_Spell_end.txt';
  g_SelfSpellStart, g_OtherSpellStart:TStringList;
  {$IFEND}

  g_BossList:THashedStringList;
  g_GJMonList:THashedStringList;
  g_GJUseMagic1:TList;
  g_GJUseMagic2:TList;
  g_SaveMyBagItemList:TList;
  g_ProcessBlackList:THashedStringList;
  g_EatItemCDConfig:TEatItemCDConfig;
  g_CustomItemPropertyBindNames:array[1..CUSTOM_PROPERTY_BIND_TYPE_COUNT] of string = ('自身防御: +', '自定魔防: +', '自身攻击: +', '自身魔法: +', '自身道术: +', '生命值: +', '魔法值: +', '无属性1: +', '无属性2: +', '无属性3: +', '无属性4: +', '无属性5: +', '无属性6: +', '无属性7: +', '无属性8: +', '无属性9: +', '无属性10: +', '无属性11: +', '无属性12: +', '无属性13: +', '无属性14: +', '无属性15: +', '无属性16: +', '无属性17: +', '无属性18: +', '无属性19: +', '无属性20: +', '无属性21: +', '无属性22: +',
    '无属性23: +', '无属性24: +', '无属性25: +', '无属性26: +', '无属性27: +', '无属性28: +', '无属性29: +', '无属性30: +', '无属性31: +', '无属性32: +', '无属性33: +', '无属性34: +', '无属性35: +', '无属性36: +', '无属性37: +', '无属性38: +', '无属性39: +', '无属性40: +', '无属性41: +', '无属性42: +', '无属性43: +', '无属性44: +', '无属性45: +', '无属性46: +', '无属性47: +', '无属性48: +', '无属性49: +', '无属性50: +', '无属性51: +', '无属性52: +', '无属性53: +');
  g_CustomItemPropertyChecks:array[1..CUSTOM_PROPERTY_BIND_TYPE_COUNT] of Boolean = (True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True);
  g_CustomItemPropertyTextVarList:TStringList;
  g_ArrButtonConfig:TArrButtonConfig = ((
    HorzAligment:taLeftJustify;
    VertAligment:taAlignTop;
    OffsetX:0;
    OffsetY:0;
    NextOffsetX:0;
    NextOffsetY:0
    ), (
    HorzAligment:taLeftJustify;
    VertAligment:taAlignTop;
    OffsetX:0;
    OffsetY:0;
    NextOffsetX:0;
    NextOffsetY:0
    ), (
    HorzAligment:taLeftJustify;
    VertAligment:taAlignTop;
    OffsetX:0;
    OffsetY:0;
    NextOffsetX:0;
    NextOffsetY:0
    ), (
    HorzAligment:taLeftJustify;
    VertAligment:taAlignTop;
    OffsetX:0;
    OffsetY:0;
    NextOffsetX:0;
    NextOffsetY:0
    ), (
    HorzAligment:taLeftJustify;
    VertAligment:taAlignTop;
    OffsetX:0;
    OffsetY:0;
    NextOffsetX:0;
    NextOffsetY:0
    ), (
    HorzAligment:taLeftJustify;
    VertAligment:taAlignTop;
    OffsetX:0;
    OffsetY:0;
    NextOffsetX:0;
    NextOffsetY:0
    ), (
    HorzAligment:taLeftJustify;
    VertAligment:taAlignTop;
    OffsetX:0;
    OffsetY:0;
    NextOffsetX:0;
    NextOffsetY:0
    ));
  g_WarrContinueHitManager:TWarrContinueHitManager;
  g_nPreviewItemActorRecog:Int64 = 0;
  g_dwPreviewItemShowTick:LongWord = 0;
  g_dwPreviewItemShowTime:LongWord = 0;
  g_PreviewItem:array of TClientPreviewMonItem;

  g_MoneyList:TQuickList;
  g_CustomMoneyRule:TList;
const
  // ShiftState值定义，可组合
  ShiftState_Shift = 1;
  ShiftState_Alt = 2;
  ShiftState_Ctrl = 4;
  ShiftState_Left = 8;
  ShiftState_Right = 16;
  ShiftState_Middle = 32;
  ShiftState_Double = 64;

const
  // MouseButton值定义
  MouseButton_Left = 0;
  MouseButton_Righ = 1;
  MouseButton_Middle = 2;

type
  TAntiPlugReLogin = procedure; stdcall;

  // 人物移动附加数据
  PAntiPlugNotifyEventMoveData = ^TAntiPlugNotifyEventMoveData;

  TAntiPlugNotifyEventMoveData = record
    nX, nY:Integer;
    nDir:Integer;
  end;

  // 物理攻击附加数据
  PAntiPlugNotifyEventHitData = ^TAntiPlugNotifyEventHitData;

  TAntiPlugNotifyEventHitData = record
    nDir:Integer;
    cmMessage:Integer;
  end;

  // 魔法攻击附加数据
  PAntiPlugNotifyEventSpellData = ^TAntiPlugNotifyEventSpellData;

  TAntiPlugNotifyEventSpellData = record
    nX, nY:Integer;
    nMagicID:Integer;
  end;

  // 鼠标事件附加数据
  PAntiPlugNotifyEventMouseData = ^TAntiPlugNotifyEventMouseData;

  TAntiPlugNotifyEventMouseData = record
    nX, nY:Integer;
    MouseButton:Integer;
    ShiftState:Integer;
  end;

  // 键盘事件附加数据
  PAntiPlugNotifyEventKeyData = ^TAntiPlugNotifyEventKeyData;

  TAntiPlugNotifyEventKeyData = record
    Key:Word;
    Reseved:Word; // 无用
    ShiftState:Integer;
  end;

  // NPC对话框点击命令附加数据
  PAntiPlugNotifyEventNpcDlgCmdData = ^TAntiPlugNotifyEventNpcDlgCmdData;

  TAntiPlugNotifyEventNpcDlgCmdData = record
    nX, nY:Integer;
    Command:array[0..255] of Char;
  end;

  // 发现物品/物品消失事件
  PAntiPlugItemEventData = ^TAntiPlugItemEventData;

  TAntiPlugItemEventData = record
    nX, nY:Integer;
    ItemID:Integer;
  end;

  // 通知事件类型
  TAntiPlugNotifyEventType = (event_NewMap, // 新地图，只登录时发一次
    event_ChangedMap, // 切换地图
    event_ChangeAttackMode, // 更换攻击模式
    event_Run, // 跑
    event_Walk, // 走
    event_Turn, // 转身
    event_Hit, // 物理攻击
    event_Spell, // 魔法攻击
    event_MouseMove, // 鼠标移动
    event_MouseDown, // 鼠标按下
    event_MouseUp, // 鼠标松开
    event_KeyDown, // 键盘按下
    event_KeyUp, // 键盘松开
    event_Recv_VerifyCode, // 收到验证码
    event_VerifyCode_Click, // 验证码点击
    event_Recv_LoginNotice, // 收到登录信息
    event_LoginNotice_Click, // 登录信息确定
    event_MsgDlg_Click, // 弹出对话框确定
    event_NpcDlgCmd_Click, // NPC对话框中点击命令
    event_DropItem_Abnormal, // 物品丢弃异常
    event_Target_Click, // 点击攻击目标
    event_Logout_Abnormal, // 小退异常
    event_ItemToStorage_Abnormal, // 物品存仓库异常
    event_StorageBackItem_Abnormal, // 取仓库物品异常
    event_LoginRandomCodeAbnormal, // 登录验证码异常
    event_NoticeMsgDlgAbnormal, // 公告对话框点击异常
    event_ItemShow, // 发现物品
    event_ItemHide, // 消失物品
    event_SendPickUp, // 发送捡物品
    event_SocketDataAbnormal, // 异常的数据包请求
    event_RungateRandomCodeAbnormal, // 网关验证码异常 2020-08-29 增加
    event_M2RandomCodeAbnormal, // M2Server验证码异常 2020-08-29 增加
    event_Disconnect // 连接断开 2020-08-29 增加
    );

  // 客户端事件通知函数，导出函数编号 1727， 事件函数原型
  TAntiPlugNotifyEventFunc = procedure(EventType:TAntiPlugNotifyEventType; EventData:PByte; EventDataLen:Integer); stdcall;

  // 动作行为检测
var
  g_IsWaitLogout:Boolean = False;
  g_IsWaitItemToStorage:Boolean = False;
  g_IsWaitStorageBackItem:Boolean = False;

var
  g_AntiPlugCheckCode:Word = 0;
  g_AntiPlugDllStringLen:Integer = 0;
  g_AntiPlugDllBufferSize:Integer = 0;
  g_AntiPlugDllString:string = '';
  g_AntiPlugDllStringCRC:LongWord = 0;
  g_AntiPlugMsgIdent:array[0..3] of Word; // 4个预留消息头
  g_AntiPlugDllMemory:TMemoryStream = nil;
  g_AntiPlugDllMemoryModuleCS:TRTLCriticalSection;
  g_AntiPlugNotifyFunc:TAntiPlugNotifyEventFunc = nil;

  {$IF AntiPlugInMemory = 1}
  g_AntiPlugDllMemoryModule:PMemoryModule = nil;
  g_TempAntiPlugFreeTick:LongWord;
  g_TempAntiPlugDllMemoryModule:PMemoryModule = nil;
  g_boWaitLoadAntiPlugDllMemoryModule:Boolean;
  {$ELSE}
  g_TempAntiPlugFreeTick:LongWord;
  g_TempAntiPlugDllModule:HMODULE = 0;
  g_boWaitLoadAntiPlugDllModule:Boolean;
  g_AntiPlugDllModule:HMODULE = 0;
  {$IFEND}

  //------------------------------------------------

  g_AntiPlugDllStringLen2:Integer = 0;
  g_AntiPlugDllBufferSize2:Integer = 0;
  g_AntiPlugDllString2:string = '';
  g_AntiPlugDllStringCRC2:LongWord = 0;
  g_AntiPlugDllMemory2:TMemoryStream = nil;
  g_AntiPlugDllMemoryModuleCS2:TRTLCriticalSection;
  {$IF AntiPlugInMemory = 1}
  g_AntiPlugDllMemoryModule2:PMemoryModule = nil;
  g_TempAntiPlugFreeTick2:LongWord;
  g_TempAntiPlugDllMemoryModule2:PMemoryModule = nil;
  g_boWaitLoadAntiPlugDllMemoryModule2:Boolean;
  {$ELSE}
  g_TempAntiPlugFreeTick2:LongWord;
  g_TempAntiPlugDllModule2:HMODULE = 0;
  g_boWaitLoadAntiPlugDllModule2:Boolean;
  g_AntiPlugDllModule2:HMODULE = 0;
  {$IFEND}

  //------------------------------------------------

  g_AntiPlugDllStringLen_LEG:Integer = 0;
  g_AntiPlugDllBufferSize_LEG:Integer = 0;
  g_AntiPlugDllString_LEG:string = '';
  g_AntiPlugDllStringCRC_LEG:LongWord = 0;
  g_AntiPlugDllMemory_LEG:TMemoryStream = nil;
  g_AntiPlugDllMemoryModuleCS_LEG:TRTLCriticalSection;
  {$IF AntiPlugInMemory = 1}
  g_AntiPlugDllMemoryModule_LEG:PMemoryModule = nil;
  g_TempAntiPlugFreeTick_LEG:LongWord;
  g_TempAntiPlugDllMemoryModule_LEG:PMemoryModule = nil;
  g_boWaitLoadAntiPlugDllMemoryModule_LEG:Boolean;
  {$ELSE}
  g_TempAntiPlugFreeTick_LEG:LongWord;
  g_TempAntiPlugDllModule_LEG:HMODULE = 0;
  g_boWaitLoadAntiPlugDllModule_LEG:Boolean;
  g_AntiPlugDllModule_LEG:HMODULE = 0;
  {$IFEND}

  //------------------------------------------------

  g_AntiPlugDllStringLen_IOCP2:Integer = 0;
  g_AntiPlugDllBufferSize_IOCP2:Integer = 0;
  g_AntiPlugDllString_IOCP2:string = '';
  g_AntiPlugDllStringCRC_IOCP2:LongWord = 0;
  g_AntiPlugDllMemory_IOCP2:TMemoryStream = nil;
  g_AntiPlugDllMemoryModuleCS_IOCP2:TRTLCriticalSection;

  {$IF AntiPlugInMemory = 1}
  g_AntiPlugDllMemoryModule_IOCP2:PMemoryModule = nil;
  g_TempAntiPlugFreeTick_IOCP2:LongWord;
  g_TempAntiPlugDllMemoryModule_IOCP2:PMemoryModule = nil;
  g_boWaitLoadAntiPlugDllMemoryModule_IOCP2:Boolean;
  {$ELSE}
  g_TempAntiPlugFreeTick_IOCP2:LongWord;
  g_TempAntiPlugDllModule_IOCP2:HMODULE = 0;
  g_boWaitLoadAntiPlugDllModule_IOCP2:Boolean;
  g_AntiPlugDllModule_IOCP2:HMODULE = 0;
  {$IFEND}

  g_boGuide:Boolean = False; //引导功能 By 一支笔 at:2022-02-12 11:06:13
  g_boDblGuide:Boolean = False; //双击才能通过引导
  g_GuideX:Integer = 0;
  g_GuideY:Integer = 0;
  g_GuideR:Integer = 100;
  g_GuideB:Integer = 100;
  g_GuideID:Integer = -1;
  g_GuideOffset:Integer = 0;
  g_GuideSwitch:Boolean = False;
  g_GuideStr:string = '点击继续游戏';
  g_GuideStrLen:Integer = 0;

  //g_AntiPlug_ReLogin: TAntiPlugReLogin = nil;
  //g_AntiPlug_EncodeSendData: TAntiPlugEncodeSendData = nil;

procedure ResetNewopUI170TextureArray(Index:Integer);

procedure SetMachineID(S:string);

function GetMachineID:string;

function GetCustomMagicConfig(MagicID:Word):PClientCustomMagicConfig;

function GetCustomNpcConfig(Appr:Word):PClientCustomNpcConfig;

procedure SendGameCenterMsg(wIdent:Word; sSendMsg:string);

function GetObjNeedUpdate(nUnit, nIdx:Integer):Boolean; // 检测是否需要更新图库

function GetObjs(nUnit, nIdx:Integer; IsHMap:Boolean):TTexture;

function GetObjsEx(nUnit, nIdx:Integer; IsHMap:Boolean; var px, py:Integer):TTexture;

function GetObjInfo(nUnit, nIdx:Integer; IsHMap:Boolean; var ASize:TSize; var APoint:TPoint):Boolean;

function GetMonImg(nAppr:Integer):TGameImages;

function GetMonAction(nAppr:Integer):pTMonsterAction;

function GetJobName(nJob:Integer):string;

function GetSexName(nSex:Integer):string;

function GetTzItemDesc(btUser, btSex:Byte; sItemName:string):TList;

procedure LoadTzItemDescList(LoadList:TStringList);

procedure UnLoadTzItemDescList;

function GetLineVariableText(Item:PTClientItem; sMsg:string):string;

function GetItemDesc(Item:PTClientItem; sItemName:string):TStringList;

procedure LoadItemDescList(LoadList:TStringList);

procedure UnLoadItemDescList;

function GetItemDescTop(Item:PTClientItem; sItemName:string):TStringList;

procedure LoadItemDescTopList(LoadList:TStringList);

procedure UnLoadItemDescTopList;

procedure LoadSkillDescList;

procedure UnLoadSkillDescList;

procedure GetSkillDesc(sSkillName:string; var DescList:TStrings);

procedure LoadSkillUpgradeDescList;

procedure UnLoadSkillUpgradeDescList;

procedure GetSkillUpgradeDesc(sSkillName:string; NewLevel:Integer; var DescList:TStrings);

function GetGodBlessItem(sItemName:string):TStringList;

procedure LoadGodBlessItemList(LoadList:TStringList);

procedure UnLoadGodBlessItemList;

function GetFengHaoItem(sItemName:string):TStringList;

procedure LoadFengHaoItemList(LoadList:TStringList);

procedure UnLoadFengHaoItemList;

function GetInputKey(Key:Integer; var Shift:TShiftState):Word;

function GetKeyDownStr(Key:Word; Shift:TShiftState; var nKey:Integer):string;

function FindBoxKey(BoxType:Integer):Integer;

procedure AddFreeActorList(Actor:TActor);

procedure AddFreeEffectList(Effect:TMagicEff);

procedure AddFreeEventList(Event:TClEvent);

function HeroBagItemCount:Integer;

function HumBagNoUseItemCount:Integer;

procedure ActorXYToMapXY(nCurrX, nCurrY:Integer; var nX, nY:Integer);

procedure MapXYToActorXY(nMapX, nMapY:Integer; var nX, nY:Integer);

procedure MapToScreen(nMapWH, nScreenWH, nMapXY:Integer; var nXY:Integer);

procedure ScreenToMap(nMapWH, nScreenWH, nScreenXY:Integer; var nXY:Integer);

function GetRGB(c256:Byte):Integer;

function RGB32(c:LongInt; BitCount:Byte):TColor;

function GetFeatureLen(nLen:Integer):Integer;

function IsUnOverLapItem(Item:PTClientItem):Boolean;

function IsOverLapItem(Item:PTClientItem):Boolean; overload;

function IsOverLapItem(Item1, Item2:PTClientItem):Boolean; overload;

function GetUseItemName(nIndex:Integer):string;

procedure LoadEffectImageList;

function GetEffectImageListTexture(nResID, nResIndex:Integer):TTexture;

function GetUserShopItemType(btItemType:Byte):string;

function GetUserShopStatus(btState:Byte):string;

procedure DebugOutStr(Msg:string; boWriteDate:Boolean = True);

function GetNewMagicLevelString(NewLevel:Byte):string;

function GetNewMagicLevelIconOffset(Magic:PTClientMagic; var IconFile:TGameImages):Integer;

//优化HGE文字，这个鸟没用 chongchong 2014-12-02
{
procedure LoadMapItemFlash(TextureTile: TTextureTile);
procedure LoadMoveNumberTexture(TextureTile: TTextureTile);
}

function PlugInEnabled:Boolean;

procedure SetRepeatBGSound(Value:Boolean);

function GetGameImages(FileName:string):TGameImages;

function CreateGameImages(FileName:string):TGameImages;

function CreateHMapDataGameImages(FileName:string):TGameImages;

function GetGameImageFiles(FileName:string; var sPassword:string; CheckExists:Boolean = True):string;

function GetDataFiles(FileName:string):string;

function GetMapFiles(FileName:string):string;

function GetWavFiles(FileName:string):string;

function GetAbsolutePathEx(BasePath, RelativePath:string):string;
// 由于改成了，韩地图的地砖只读HMapData\中的内容，如果该目录没有文件则不读，故去掉规则
//function GetHMapDataFiles(FileName: string; var sGetFileName: string): Boolean;

function ShiftText(Shift:TShiftState):string;

function GetUpgradeItemName(nIndex:Integer; boAllItem:Boolean):string;

procedure LoadCustomMonsterConfigs(Stream:TMemoryStream);

procedure LoadCustomMagicConfigs(Stream:TMemoryStream);

procedure LoadCustomNpcConfigs(Stream:TMemoryStream);

procedure LoadNGCustomUnbindItemList(FileName:string);

procedure SaveNGCustomUnbindItemList(FileName:string);

function GetStdItem(nIndex:Integer):PTClientItem;

// 获取镶嵌凹槽的数量
function GetFluteCount(Item:PTClientItem):Integer;

// 获取相同Anicount宝石的数量
//function GetAnicountFluteCount(Item: PTClientItem; AniCount: Integer): Integer;

function CheckBlockListSys(Ident:Integer; sMsg:string):Boolean;

function IntToHexN(const V, Digits:Integer):string;

function ProcessFileNameSpecialChar(S:string):string;

function SaveOrLoadMyBagItemList(IsSave:Boolean):Boolean;

function tick_diff(tick_start, tick_end:Cardinal):Cardinal;

function HpAddUnit(V:LongWord):string;

function GetTempDir:string;

function MakeTempFileName(const FileExt:string):string;

function GetHintNameFontName:string;

function GetHintNameFontSize:Integer;

function GetHintNameFontStyle(FontStyles:TFontStyles):TFontStyles;

function GetHintNameFontStroke(IsStroke:Boolean = False):Boolean;

function GetHintFontSize:Integer;

function GetHintFontStyle(FontStyles:TFontStyles):TFontStyles;

function GetHintFontStroke(IsStroke:Boolean = False):Boolean;

function GetMaxBagCount:Integer;

function IsAttackAction(Action:Integer):Boolean;

procedure Clear_g_EnabledAuctionItemList;

function ShiftStateToPlugShiftState(Shift:TShiftState):Integer;

function MyGetTickCount:DWORD; stdcall; external mmsyst name 'timeGetTime';
// function MyGetTickCount: DWORD; stdcall;; external kernel32 name 'GetTickCount';

function CheckInAutoCustomMagic(MagicWarrNGOption:TMagicWarrNGOption; MagicID:Integer):Boolean;

procedure SortStdItemListDoSort(L, R:Integer);

function GetItemIdxByName(const ItemName:string):Word;

procedure RecalcDraw(VirtualRect, VisibleRect:TRect; IsOffset:Boolean; OffsetX, OffsetY:Integer; Texture:TTexture; ABlendMode:Integer; OnlyDrawRect:Boolean = False);

function GetInputBoxInFilterList(sInputBox:string):Boolean;

function DrawItemHintOldStyle(nX, nY:Integer; S:WideString; Color:TColor = clWhite):Integer;

function Makecode_T(pstr:Integer):Integer;

function Cutecode_T(pstr:Integer):Integer;

procedure CreatePayMentQRCodeSurface(var Str:string);

procedure HttpPost(btType:Byte; Param1:string; Param2:string; Param3:string; Param4:string);

function JsonLoad(Value:string; a:ISuperObject; Name:string):string; overload;

function JsonLoad(Value:Integer; a:ISuperObject; Name:string):Integer; overload;

function JsonLoad(Value:Boolean; a:ISuperObject; Name:string):Boolean; overload;

procedure JsonSave(a:ISuperObject; Name:string; Value:string; CheckValue:string = ''); overload;

procedure JsonSave(a:ISuperObject; Name:string; Value:Integer; CheckValue:Integer = 0); overload;

procedure JsonSave(a:ISuperObject; Name:string; Value:Boolean; CheckValue:Boolean = False); overload;

function GetCustomMoneyNameByRule(nIndex:Integer; btType:Byte):string;

function GetCustomMoneyIndexByName(sName:string; btType:Byte):Integer;

function CreateQRCodeTexture(sText:string; nSize:Integer; nColor:Cardinal):TTexture;


{$IF TESTMODE = 1}
function EncryptImageFileListPassword(sPassword:string):string; //HZQ
{$IFEND}

function IsInContinuous:Boolean;
function GetDefaultGJPointArray(nPrevX, nPrevY:Integer):TArrayPoint; //

implementation

uses
  DxCanvas,
  HUtil32,
  DelphiZXIngQRCode,
  SerialWindowsDlg,
  FState;

var
  sMachineID:string = '';

{$IF TESTMODE = 1}
function EncryptImageFileListPassword(sPassword:string):string; //hzq
var
  PakPassword:TPakPassword;
begin
  GetKeyDataPak(sPassWord, @PakPassword.Chain, @PakPassword.KeyData);
  GetKeyData(sPassWord, @PakPassword.ChainLz, @PakPassword.KeyDataLz);
  EncryptDes(PakPassword, PakPassword, SizeOf(PakPassword), #10#11#2#9#2#1#12#32#1#9#5#230#211#190);
  Result := EncodeBuffer(@PakPassword, SizeOf(TPakPassword));
end;
{$IFEND} // 机器码


//测试指定的点是否可以走到
function MapCanGoTest(nPrevX, nPrevY, nNextX, nNextY:integer):Boolean;
const
    ClearSize = 2;
begin
    Result := False;

    //对四个边角进行判断，确保不贴边
    if Map.NewCanMove(nNextX, nNextY)
    and Map.NewCanMove(nNextX-ClearSize, nNextY-ClearSize) and Map.NewCanMove(nNextX-ClearSize, nNextY+ClearSize)
    and Map.NewCanMove(nNextX+ClearSize, nNextY-ClearSize) and Map.NewCanMove(nNextX+ClearSize, nNextY+ClearSize) then begin
        //Do Nothing...
    end else begin
        Exit;
    end;

    //路径点太远，寻路太慢，所以需要对大地图做出处理
    if (Abs(nNextX - nPrevX) < 100) and (Abs(nNextY - nPrevY) < 100) then begin
        LegendMap.BeginX := nPrevX;
        LegendMap.BeginY := nPrevY;
        LegendMap.EndX := nNextX;
        LegendMap.EndY := nNextY;
        LegendMap.FindPath(LegendMap.BeginX, LegendMap.BeginY, LegendMap.EndX, LegendMap.EndY, 0, True);
        if Length(LegendMap.RunPath) > 0 then begin
            Result := True;
        end;
        LegendMap.Stop;
    end else begin
        Result := True;
    end;
end;

//以中心点为半径，逐渐向周边扩散
function GetMapCanGoPoint(nPrevX, nPrevY, nCentX, nCentY:Integer; nSearchHalfX, nSearchHalfY:Integer):TPoint;
var
    i, x, y, nRy, nSearchR:Integer;
begin
    Result.X := -1;
    Result.Y := -1;

    if MapCanGoTest(nPrevX, nPrevY, nCentX, nCentY) then begin
        Result.X := nCentX;
        Result.Y := nCentY;
        Exit;
    end;

    nSearchR := _Max(nSearchHalfX, nSearchHalfY);
    
    for i := 1 to nSearchR do begin
        for x := -i to i do begin
            if (i <= nSearchHalfY) and (Abs(x) <= nSearchHalfX) then begin
               if MapCanGoTest(nPrevX, nPrevY, nCentX + x, nCentY - i) then begin
                  Result.X := nCentX + x;
                  Result.Y := nCentY - i;
                  Exit;
               end;

               if MapCanGoTest(nPrevX, nPrevY, nCentX + x, nCentY + i) then begin
                  Result.X := nCentX + x;
                  Result.Y := nCentY + i;
                  Exit;
               end;
            end;
        end;

        nRy := i - 1;
        for y := -(nRy) to (nRy) do begin //去除重复的点
            if (i <= nSearchHalfX) and (Abs(y) <= nSearchHalfY) then begin
               if MapCanGoTest(nPrevX, nPrevY, nCentX - i, nCentY + y) then begin
                  Result.X := nCentX - i;
                  Result.Y := nCentY + y;
                  Exit;
               end;

               if MapCanGoTest(nPrevX, nPrevY, nCentX + i, nCentY + y) then begin
                  Result.X := nCentX + i;
                  Result.Y := nCentY + y;
                  Exit;
               end;
            end;
        end;
    end;
end;

//HZQ 20231007 增加自动获取地图挂机点数组
function GetDefaultGJPointArray(nPrevX, nPrevY:Integer):TArrayPoint;
var
    nCountX, nCountY, nStepX, nStepY, nCentX, nCentY:Integer;
    i, x, y, nSRX, nSRY:Integer; //nSRX, nSRY搜索半径
    ptGj:TPoint;
    nCount, nDir, nMapHeight:Integer;
const
    MiniBlockSize = 48;   //默认分隔块大小
    MaxAxisPoint = 5;     //轴向最大点数
    BigMapMaxSearchRadius = 5; //最大搜索半径
begin
    {$IF TESTMODE = 1}
    DScreen.AddChatBoardString(Format('Map[%d, %d]', [Map.m_nWidth, map.m_nHeight]), clGreen, clRed);
    {$IFEND}

    nCountX := Map.m_nWidth div MiniBlockSize;
    nCountY := Map.m_nHeight div MiniBlockSize;

    if nCountX < 1 then nCountX := 1 else if nCountX > MaxAxisPoint then nCountX := MaxAxisPoint;
    if nCountY < 1 then nCountY := 1 else if nCountY > MaxAxisPoint then nCountY := MaxAxisPoint;

    SetLength(Result, nCountX * nCountY);

    if nCountX = 1 then begin
        nStepX := map.m_nWidth div 2;
        nSRX := nStepX div 2;
    end else begin
        nStepX := map.m_nWidth div nCountX;
        nSRX := nStepX div 2;
    end;

    if nCountY = 1 then begin
        nStepY := map.m_nHeight div 2;
        nSRY := nStepY div 2;
    end else begin
        nStepY := map.m_nHeight div nCountY;
        nSRY := nStepY div 2;
    end;

    if Map.m_nWidth > 400 then begin
       if nSRX < 1 then nSRX := 1 else if nSRX > BigMapMaxSearchRadius then nSRX := BigMapMaxSearchRadius;
    end else begin
       if nSRX < 1 then nSRX := 1;
    end;

    if Map.m_nHeight > 400 then begin
       if nSRY < 1 then nSRY := 1 else if nSRY > BigMapMaxSearchRadius then nSRY := BigMapMaxSearchRadius;
    end else begin
       if nSRY < 1 then nSRY := 1;
    end;

    nCentX := - (nStepX div 2);//-nSRX;
    nCount := 0;

    nDir := 0;
    nMapHeight := Map.m_nHeight;
    for x := 0 to nCountX - 1 do begin
        Inc(nCentX, nStepX);
        if (nDir and $01) = 0 then begin
            nCentY := -(nStepY div 2); //-nSRY;
            for y := 0 to nCountY - 1 do begin
                Inc(nCentY, nStepY);
                ptGj := GetMapCanGoPoint(nPrevX, nPrevY, nCentX, nCentY, nSRX, nSRY);
                if ptGj.X >= 0 then begin
                    Result[nCount] := ptGj;
                    nPrevX := ptGj.X;
                    nPrevY := ptGj.Y;
                    Inc(nCount);
                end;
            end;
        end else begin
            nCentY := nMapHeight + (nStepY div 2); //-nSRY;
            for y := nCountY - 1 downto 0 do begin
                Dec(nCentY, nStepY);
                ptGj := GetMapCanGoPoint(nPrevX, nPrevY, nCentX, nCentY, nSRX, nSRY);
                if ptGj.X >= 0 then begin
                    Result[nCount] := ptGj;
                    nPrevX := ptGj.X;
                    nPrevY := ptGj.Y;
                    Inc(nCount);
                end;
            end;
        end;
        Inc(nDir);
    end;

    SetLength(Result, nCount); //去除不能到的点

   {$IF TESTMODE = 1}
   for i := Low(Result) to High(Result) do begin     
       DScreen.AddChatBoardString(Format('Point[%d, %d]', [Result[i].X, Result[i].Y]), clGreen, clRed);
   end;
   {$IFEND}
end;

function IsInContinuous:Boolean;
begin
    Result := g_boContinuous;//InterlockedCompareExchange(g_nContinuousStatus, 0, 0) <> 0;
end;

function GetCustomMoneyIndexByName(sName:string; btType:Byte):Integer;
var
  I:Integer;
  Money:pTClientCustomMoney;
begin
  //nIdx := nIndex - 5;
  Result := -1;
  for I := 0 to g_CustomMoneyRule.Count - 1 do begin
    Money := pTClientCustomMoney(g_CustomMoneyRule.Items[I]);
    if Money.sName = sName then begin
      Result := Money.nIndex + 5;
      case btType of
        0:if Money.boCanMyShop then Exit;
        //        1: if Money.boCanGameShop then Exit;
        2:if Money.boCanAuction then Exit;
        3:if Money.boCanSellPlayer then Exit;
      end;
      Result := -1;
      Exit;
    end;
  end;
end;


function GetCustomMoneyNameByRule(nIndex:Integer; btType:Byte):string;
var
  I:Integer;
  Money:pTClientCustomMoney;
  nIdx:Integer;
begin
  nIdx := nIndex - 5;
  Result := '';
  for I := 0 to g_CustomMoneyRule.Count - 1 do begin
    Money := pTClientCustomMoney(g_CustomMoneyRule.Items[I]);
    if Money.nIndex = nIdx then begin
      Result := Money.sName;
      case btType of
        0:if Money.boCanMyShop then Exit;
        //        1: if Money.boCanGameShop then Exit;
        2:if Money.boCanAuction then Exit;
        3:if Money.boCanSellPlayer then Exit;
      end;
      Result := '';
      Exit;
    end;
  end;
end;

function JsonLoad(Value:string; a:ISuperObject; Name:string):string;
begin
  if a.O[Name] <> nil then
    Result := a.S[Name]
  else
    Result := Value;
end;

function JsonLoad(Value:Integer; a:ISuperObject; Name:string):Integer;
begin
  if a.O[Name] <> nil then
    Result := a.I[Name]
  else
    Result := Value;
end;

function JsonLoad(Value:Boolean; a:ISuperObject; Name:string):Boolean;
begin
  if a.O[Name] <> nil then
    Result := a.B[Name]
  else
    Result := Value;
end;

procedure JsonSave(a:ISuperObject; Name:string; Value:string; CheckValue:string);
begin
  if Value <> CheckValue then begin
    if a = nil then
      a := SO('{}');
    a.S[Name] := Value;
  end;
end;

procedure JsonSave(a:ISuperObject; Name:string; Value:Integer; CheckValue:Integer);
begin
  if Value <> CheckValue then begin
    if a = nil then
      a := SO('{}');
    a.I[Name] := Value;
  end;
end;

procedure JsonSave(a:ISuperObject; Name:string; Value:Boolean; CheckValue:Boolean);
begin
  if Value <> CheckValue then begin
    if a = nil then
      a := SO('{}');
    a.B[Name] := Value;
  end;
end;

//HZQ 20230606

function StretchBmp(bmpSrc:TGraphic; nNewW, nNewH:Integer):TBitmap;
var
  rtNew:TRect;
  begin
 Result := TBitmap.Create;
  Result.SetSize(nNewW, nNewH);
  rtNew := Bounds(0, 0, nNewW, nNewH);
  Result.Canvas.StretchDraw(rtNew, bmpSrc);
end;

//HZQ 20230606

function SmoothStretch(bmpSrc:TBitmap; nNewW, nNewH:Integer):TBitmap;
var
  pt:TPoint;
  h:HDC;
begin
  Result := TBitmap.Create;
  Result.SetSize(nNewW, nNewH);

  h := Result.Canvas.Handle;
  GetBrushOrgEx(h, pt);
  SetStretchBltMode(h, HALFTONE);
  SetBrushOrgEx(h, pt.x, pt.y, @pt);
  StretchBlt(h, 0, 0, Result.Width, Result.Height, bmpSrc.Canvas.Handle, 0, 0, bmpSrc.Width, bmpSrc.Height, SRCCOPY);
end;

//HZQ 20230605

function CreateQRCodeTexture(sText:string; nSize:Integer; nColor:Cardinal):TTexture;
var
  bmpOrigin, bmpStretch:TBitmap;
  msData:TMemoryStream;
  nLen:Integer;
  nStretchSize, nDotSize:Integer;
  objQR:TDelphiZXingQRCode;
const
  DEFAULT_QR_DOT_SIZE = 2; //每个点占用几个像素
begin
  Result := nil;
  bmpStretch := nil;

  nLen := Length(sText);
  if (nLen = 0) or (nLen > 512) then Exit;

  if nSize < 0 then begin
    nStretchSize := 0;
    nDotSize := -nSize;
    if nDotSize > 16 then nDotSize := 16;
  end else if nSize = 0 then begin
    nStretchSize := 0;
    nDotSize := 1;
  end else begin
    if nSize < 64 then begin
      nStretchSize := 64;
    end else if nSize > 384 then begin
      nStretchSize := 384;
    end else begin
      nStretchSize := nSize;
    end;
    nDotSize := DEFAULT_QR_DOT_SIZE; //默认使用2倍绘制，方便拉伸
  end;

  objQR := TDelphiZXingQRCode.Create;
  objQR.QuietZone := 1; //边框
  objQR.Encoding := qrAuto; //编码
  bmpOrigin := objQR.EncodeToBitmap(sText, nDotSize, clBlack, clWhite);

  if bmpOrigin <> nil then begin
    msData := TMemoryStream.Create;
    if nStretchSize > 0 then begin
      bmpStretch := SmoothStretch(bmpOrigin, nStretchSize, nStretchSize);
      bmpStretch.SaveToStream(msData);
    end else begin
      bmpOrigin.SaveToStream(msData);
      nStretchSize := bmpOrigin.Width;
    end;
    Result := NewTexture(msData.Memory, msData.Size, nStretchSize, nStretchSize, clBlack, False);

    if bmpStretch <> nil then begin
      bmpStretch.Free;
    end;
    msData.Free;
    bmpOrigin.Free;
  end;
  objQR.Free;
end;

procedure CreatePayMentQRCodeSurface(var Str:string);
var
  bitmap:TBitmap;
  MS:TMemoryStream;
  objQR:TDelphiZXingQRCode;
begin
  if Str = '' then
    Exit;
  bitmap := nil;
  if g_PayMentQRCodeSurface <> nil then
    FreeAndNil(g_PayMentQRCodeSurface);
  //  if g_PayMentQRCodeSurface = nil then
  begin
    try
      objQR := TDelphiZXingQRCode.Create;
      objQR.QuietZone := 1;
      objQR.Encoding := qrAuto;
      bitmap := objQR.EncodeToBitmap(str, g_PayMentQRCodeSize, clBlack, clWhite);

      (*
     bitmap := qr(AnsiString(StringReplace(Str, #13#10, '', [rfReplaceAll])),  //AnsiString('D:\1.bmp'),
       False,
       0, //边框
       g_PayMentQRCodeSize, //大小
       0, //8bit
       1, //Casesens
       0, // L M Q H
       $020202, //Color
       clWhite//BackGroud Color
     );*)

      if bitmap <> nil then begin
        MS := TMemoryStream.Create;
        try
          bitmap.SaveToStream(MS);
          g_PayMentQRCodeSurface := NewTexture(MS.Memory, MS.Size, bitmap.Width, bitmap.Height, clBlack, false);
        finally
          FreeAndNil(MS);
          FreeAndNil(bitmap);
        end;
      end;
      objQR.Free;
      except
 //MessageDlg('生成失败！', mtInformation, [mbOK], -1);
    end;
  end;
  Str := '';
end;

function Makecode_T(pstr:Integer):Integer;
{$IF ENCRYPOINT = 1}
var
  a:Integer;
  {$IFEND}
begin
  {$IF ENCRYPOINT = 1}
  {.$I VMProtectBeginMutation.inc}
  a := Random(8) + 1;
  Result := (pstr xor a) * 10 + a;
  {.$I VMProtectEnd.inc}
  {$ELSE}
  Result := pstr;
  {$IFEND}
end;

function Cutecode_T(pstr:Integer):Integer;
{$IF ENCRYPOINT = 1}
var
  t:Integer;
  {$IFEND}
begin
  //Result := pstr; //HZQ
  {$IF ENCRYPOINT = 1}
  {.$I VMProtectBeginMutation.inc}
  t := pstr mod 10;
  Result := (pstr div 10) xor t;
  {.$I VMProtectEnd.inc}
  {$ELSE}
  Result := pstr;
  {$IFEND}
end;

procedure SetMachineID(S:string);
begin
  S := Trim(S);
  if S <> '' then begin
    sMachineID := Trim(S);
  end;
end;

function GetMachineID:string;
begin
  if Length(sMachineID) = 0 then begin
    sMachineID := RivestStr(GetIdeSerialNumber + GetWindowsVersion + GetCpuIDstringEx + GetAdapterMac(0));
  end;
  Result := sMachineID;
end;

function ShiftText(Shift:TShiftState):string;
begin
  Result := '';
  if ssShift in Shift then
    Result := Result + 'ssShift ';
  if ssAlt in Shift then
    Result := Result + 'ssAlt ';
  if ssCtrl in Shift then
    Result := Result + 'ssCtrl ';
  if ssLeft in Shift then
    Result := Result + 'ssLeft ';
  if ssRight in Shift then
    Result := Result + 'ssRight ';
  if ssMiddle in Shift then
    Result := Result + 'ssMiddle ';
  if ssDouble in Shift then
    Result := Result + 'ssDouble ';
  Result := Trim(Result);
end;

procedure SetRepeatBGSound(Value:Boolean);
begin
  g_BassSound.RepeatMusic(Value);
end;

function PlugInEnabled:Boolean;
begin
  Result := (g_ClientConfig.boStartGameAuxiliary and g_ConfigDlg.Enabled);
end;

{ 优化HGE文字，这个鸟没用 chongchong 2014-12-02
procedure LoadMoveNumberTexture(TextureTile: TTextureTile);
var
  I: Integer;
  d: TTexture;
  X, Y: Integer;

  nW, nH, nRow, nC: Integer;
begin
  nW := 0;
  nH := 0;
  for I := 50 to 101 do
  begin
    d := g_WNewopUIImages.GetCachedImage(I, X, Y);
    if d <> nil then
    begin
      if nW < d.Width then
        nW := d.Width;
      if nH < d.Height then
        nH := d.Height;
    end;
  end;
  if (nW > 4) and (nH > 4) then
  begin
    nC := TextureTile.Width div nW;
    nRow := 52 div nC;
    if 52 mod nC > 0 then Inc(nRow);
    if nRow <= 0 then nRow := 1;
    TextureTile.Height := nRow * nH;
    TextureTile.PatternWidth := nW;
    TextureTile.PatternHeight := nH;
    for I := 50 to 101 do
    begin
      d := g_WNewopUIImages.GetCachedImage(I, X, Y);
      if (d <> nil) and (d.Width * d.Height > 4) then
        TextureTile.Add(X, Y, d);
    end;
  end;
end;

procedure LoadMapItemFlash(TextureTile: TTextureTile);
var
  I: Integer;
  d: TTexture;
  X, Y: Integer;

  nW, nH, nRow, nC: Integer;
  boNotFind: Boolean;
begin
  nW := 0;
  nH := 0;
  for I := 410 to 418 do
  begin
    d := g_WMainImages.GetCachedImage(I, X, Y);
    if (not boNotFind) and (d <> nil) and (d.Width * d.Height > 16) then
    begin
      if nW < d.Width then
        nW := d.Width;
      if nH < d.Height then
        nH := d.Height;
    end
    else
    begin
      nW := 0;
      nH := 0;
      boNotFind := True;
      // break;
    end;
  end;
  if nW > 0 then
  begin
    nC := TextureTile.Width div nW;
    nRow := 9 div nC;
    if 9 mod nC > 0 then Inc(nRow);
    if nRow <= 0 then nRow := 1;
    TextureTile.Height := nRow * nH;
    TextureTile.PatternWidth := nW;
    TextureTile.PatternHeight := nH;
    for I := 410 to 418 do
    begin
      d := g_WMainImages.GetCachedImage(I, X, Y);
      if d <> nil then
        TextureTile.Add(X, Y, d);
    end;
  end;
end;
}

{ TODO -opiaoyun -c注释 : 获取技能栏小图标偏移【2013-6-16】 }

function GetNewMagicLevelIconOffset(Magic:PTClientMagic; var IconFile:TGameImages):Integer;
var
  Offset:Integer;
  nEffect:Integer;
  MagicConfig:PClientCustomMagicConfig;
  ClientConfig:PMagicClientConfig;
  MagicPlusLevel:TMagicPlusLevel;
begin
  IconFile := nil;
  Result := -1;

  if CheckIsCustomMagic(Magic.Def.wMagicId) then begin
    MagicConfig := GetCustomMagicConfig(Magic.Def.wMagicId);
    if MagicConfig <> nil then begin
      case Magic.NewLevel of
        0:
          MagicPlusLevel := mplNone;
        1..3:
          MagicPlusLevel := mpl1_3;
        4..6:
          MagicPlusLevel := mpl4_6;
        7..9:
          MagicPlusLevel := mpl7_9;
        else
          MagicPlusLevel := mpl7_9;
      end;

      ClientConfig := @MagicConfig.MagicConfigs[MagicPlusLevel];

      if (ClientConfig.Icon_File >= 0) and (ClientConfig.Icon_File < g_EffectImageList.Count) then begin
        IconFile := TGameImages(g_EffectImageList.Objects[ClientConfig.Icon_File]);
        Result := ClientConfig.Icon_Index;
      end;
    end;

    if Result < 0 then
      Result := Magic.Def.btEffect * 2;

    Exit;
  end;

  // 这几个技能，管你丫的强不强化，反正就这个图标 chongchong 2014-05-15
  if Magic.Def.wMagicId in [51, 52, 66, 69, 75, 201..210, 115, 116, 117] then begin
    nEffect := 0; //HZQ 前面已经判断过ID都是有效的，所以这里随便给个值，解决编译器的警告问题
    case Magic.Def.wMagicId of
      51:nEffect := 46; // 群体施毒术
      52:nEffect := 47; // 飓风破  42
      66:nEffect := 44; // 开天斩
      69:nEffect := 353;
      75:nEffect := 50; // 护体神盾
      201:nEffect := 1000 + 86; // 新技能
      202:nEffect := 1000 + 66; // 裂神符
      203:nEffect := 1000 + 293; // 死亡之眼
      204:nEffect := 1000 + 292; // 十步一杀
      205:nEffect := 1000 + 295; // 冰霜雪雨
      206:nEffect := 1000 + 87; // 冰霜群雨
      207:nEffect := 1000 + 50; // 金刚护体
      208:nEffect := 1000 + 342; // 旋风斩 piaoyun 2013-09-14
      209:nEffect := 1000 + 341; // 五雷轰 piaoyun 2013-09-14
      210:nEffect := 1000 + 340; // 幽冥火符 piaoyun 2013-09-14
      115:nEffect := 1000 + 85;
      116:nEffect := 1000 + 86;
      117:nEffect := 1000 + 87;
    end;

    Result := nEffect * 2;
    Exit;
  end;

  if (Magic.NewLevel <= 0) or (Magic.Def.MagicAttr in [mtDefense, mtAttack]) then begin
    // 修改上面冗余代码 -- piaoyun 2013-06-24
    case Magic.Def.wMagicId of
      42:
        nEffect := 1000 + 297; // 龙影剑法 chongchong 2013-11-19
      66:
        nEffect := 43; // 开天斩 chongchong 2013-11-19
      74:
        nEffect := 42; // 分身术 piaoyun 2013-07-31
      75:
        nEffect := 50;
      76:
        nEffect := 28;
      52:
        nEffect := 42; // 飓风破
      26:begin
          if (Magic.Level = 4) then // 4级烈火 chongchong
            nEffect := 71
          else
            nEffect := Magic.Def.btEffect;
        end;
      13:begin
          if (Magic.Level = 4) then // 4级灵魂火符 chongchong
            nEffect := 70
          else
            nEffect := Magic.Def.btEffect;
        end;
      31:begin
          if (Magic.Level = 4) then // 4级魔法盾 chongchong
            nEffect := 52
          else
            nEffect := Magic.Def.btEffect;
        end;
      45:begin
          if (Magic.Level = 4) then // 4级灭天火 chongchong
            nEffect := 72
          else
            nEffect := Magic.Def.btEffect;
        end;
      38: {// 诅咒术 chongchong 2015-05-23} begin
          nEffect := 49;
        end;
      46: {// 新诅咒术 chongchong 2015-07-25} begin
          nEffect := 46;
        end;
      51: {// 群体施毒 chongchong 2015-05-23} begin
          nEffect := 46;
        end;
      34: {// 解毒术 chongchong 2015-05-23} begin
          nEffect := 40;
        end;
      88, 89: {// 新武力盾 新道力盾 chongchong 2015-11-02} begin
          nEffect := 29;
        end;
      else
        nEffect := Magic.Def.btEffect;
    end;

    Result := nEffect * 2;
  end
  else begin
    // 针对新端的，技能强化，九重 --- piaoyun 2013-6-17
    if (g_ClientVersion <= cvMirSequel) or (g_ClientVersion = cvMirNewUI205) then begin
      if Magic.NewLevel <= 3 then
        Offset := 0
      else if Magic.NewLevel <= 6 then
        Offset := 2
      else if Magic.NewLevel <= 9 then
        Offset := 4
      else
        Offset := 6;
      case Magic.Def.wMagicId of
        3:
          Result := 450 + Offset; // 基本剑术
        7:
          Result := 440 + Offset; // 攻杀剑术
        12:
          Result := 430 + Offset; // 刺杀剑术
        25:
          Result := 420 + Offset; // 半月弯刀
        26:
          Result := 460 + Offset; // 烈火剑法
        56:
          Result := 470 + Offset; // 逐日剑法

        13, 202:
          Result := 490 + Offset; // 灵魂火符 -- 裂神符
        48:
          Result := 500 + Offset; // 气功波
        15:
          Result := 510 + Offset; // 神圣战甲术
        6, 51:
          Result := 520 + Offset; // 施毒术
        57:
          Result := 530 + Offset; // 噬血术
        14:
          Result := 540 + Offset; // 幽灵盾
        17:
          Result := 550 + Offset; // 召唤骷髅
        30, 76:
          Result := 560 + Offset; // 召唤神兽, 召唤圣兽

        23:
          Result := 580 + Offset; // 爆裂火焰
        33:
          Result := 590 + Offset; // 冰咆哮
        22:
          Result := 600 + Offset; // 火墙
        10:
          Result := 610 + Offset; // 疾光电影
        8:
          Result := 620 + Offset; // 抗拒火环
        11:
          Result := 630 + Offset; // 雷电术
        58:
          Result := 640 + Offset; // 流星火雨
        45:
          Result := 650 + Offset; // 灭天火
        31, 73, 88, 89:
          Result := 104; // 魔法盾 道力盾 新武力盾 新道力盾 piaoyun 2013-08-27
        else
          Result := Magic.Def.btEffect * 2;
      end;
    end {
      else if g_ClientVersion = cvMirReturn then
      begin
        if Magic.NewLevel <= 3 then
          Offset := 0
        else if Magic.NewLevel <= 6 then
          Offset := 2
        else if Magic.NewLevel <= 9 then
          Offset := 4
        else
          Offset := 6;
        case Magic.Def.wMagicId of
          25: Result := 570 + Offset;                                                                 // 半月弯刀
          12: Result := 578 + Offset;                                                                 // 刺杀剑术
          3: Result := 594 + Offset;                                                                  // 基本剑术
          7: Result := 586 + Offset;                                                                  // 攻杀剑术
          26: Result := 602 + Offset;                                                                 // 烈火剑法
          56: Result := 610 + Offset;                                                                 // 逐日剑法

          13: Result := 630 + Offset;                                                                 // 灵魂火符
          48: Result := 638 + Offset;                                                                 // 气功波
          15: Result := 646 + Offset;                                                                 // 神圣战甲术
          6, 51: Result := 654 + Offset;                                                              // 施毒术
          57: Result := 662 + Offset;                                                                 // 噬血术
          14: Result := 670 + Offset;                                                                 // 幽灵盾
          17: Result := 678 + Offset;                                                                 // 召唤骷髅
          76: Result := 586 + Offset;                                                                 // 召唤神兽

          23: Result := 720 + Offset;                                                                 // 爆裂火焰
          33: Result := 728 + Offset;                                                                 // 冰咆哮
          10: Result := 736 + Offset;                                                                 // 疾光电影
          8: Result := 744 + Offset;                                                                  // 抗拒火环
          11: Result := 752 + Offset;                                                                 // 雷电术
          58: Result := 760 + Offset;                                                                 // 流星火雨
          45: Result := 768 + Offset;                                                                 // 灭天火
          22: Result := 776 + Offset;                                                                 // 火墙
        else
          Result := Magic.Def.btEffect * 2;
        end;
      end
      }
    else
      Result := Magic.Def.btEffect * 2;
  end;
end;

function GetNewMagicLevelString(NewLevel:Byte):string;
const
  NewLevelStrings:array[0..10] of string = ('一', '二', '三', '四', '五', '六', '七', '八', '九', '十', '百');
var
  nNum:Integer;
begin
  if NewLevel > 0 then begin
    Result := '强化';
    if NewLevel >= 100 then
      Result := Result + NewLevelStrings[0] + NewLevelStrings[10]
    else begin
      if NewLevel < 10 then
        Result := Result + NewLevelStrings[NewLevel - 1]
      else begin
        nNum := NewLevel div 10;
        if nNum = 1 then
          Result := Result + NewLevelStrings[9]
        else begin
          Result := Result + NewLevelStrings[nNum - 1];
          Result := Result + NewLevelStrings[9];
        end;
        nNum := NewLevel mod 10;
        if nNum > 0 then
          Result := Result + NewLevelStrings[nNum - 1];
      end;
    end;
    Result := Result + '重';
  end
  else
    Result := '';
end;

function GetUserShopStatus(btState:Byte):string;
begin
  case btState of
    0:
      Result := '在线';
    1:
      Result := '摆摊';
    2:
      if g_boOfflineCloseMyShop then
        Result := '关闭'
      else
        Result := '离线';
  end;
end;

function GetUserShopItemType(btItemType:Byte):string;
begin
  case btItemType of //
    0:
      Result := '装饰';
    1:
      Result := '补给';
    2:
      Result := '强化';
    3:
      Result := '好友';
    4:
      Result := '限量';
    5:
      Result := '奇珍';
    else
      Result := '';
  end;
end;

function GetUpgradeItemName(nIndex:Integer; boAllItem:Boolean):string;
begin
  case nIndex of
    10, 11:
      Result := '衣服';
    5, 6:
      Result := '武器';
    12:
      Result := '盾牌';
    15:
      Result := '头盔';
    19, 20, 21:
      Result := '项链';
    22, 23:
      Result := '戒指';
    24, 26:
      Result := '手镯';
    28:
      Result := '马牌';
    29 {天使翅膀}, 30:
      Result := '勋章、照明物';
    // 25, 51: Result := '符'; //符
    52, 62:
      Result := '鞋';
    7, 53, 63:
      Result := '宝石'; //
    54, 64:
      Result := '腰带';
    16:
      Result := '斗笠';
    65:
      Result := '军鼓'; // 鼓
    66, 67:
      Result := '时装衣服';
    68, 69:
      Result := '时装武器';
    75, 76, 77:
      Result := '时装项链';
    78:
      Result := '时装头盔';
    79, 80:
      Result := '时装手镯';
    81, 82:
      Result := '时装戒指';
    83:
      Result := '时装照明物品';
    84, 85:
      Result := '时装腰带';
    86, 87:
      Result := '时装鞋';
    88, 89:
      Result := '时装宝石';
    90:
      Result := '灵玉';
    else
      if boAllItem then
        Result := '所有装备'
      else
        Result := '符合装备';
  end;
end;

function GetUseItemName(nIndex:Integer):string;
begin
  case nIndex of //
    0:
      Result := U_DRESSNAME;
    1:
      Result := U_WEAPONNAME;
    2:
      Result := U_RIGHTHANDNAME;
    3:
      Result := U_NECKLACENAME;
    4:
      Result := U_HELMETNAME;
    5:
      Result := U_ARMRINGLNAME;
    6:
      Result := U_ARMRINGRNAME;
    7:
      Result := U_RINGLNAME;
    8:
      Result := U_RINGRNAME;
    9:
      Result := U_BUJUKNAME;
    10:
      Result := U_BELTNAME;
    11:
      Result := U_BOOTSNAME;
    12:
      Result := U_CHARMNAME;
    13:
      Result := U_HATNAME;
    14:
      Result := U_DRUMNAME;
    15:
      Result := U_HORSENAME;
  end;
end;

function IsOverLapItem(Item:PTClientItem):Boolean;
begin
  // 叠加物品加入31 chongchong 2014-04-08
  Result := (Item.S.Name <> '') and (Item.s.StdMode in [0, 2, 3, 31, 40, 41, 42, 46, 47]) and (Item.s.OverLap > 0);
end;

function IsOverLapItem(Item1, Item2:PTClientItem):Boolean;
begin
  // 叠加物品加入31 chongchong 2014-04-08
  Result := (Item1.S.Name <> '') and (Item1.s.StdMode in [0, 2, 3, 31, 40, 41, 42, 46, 47]) and (Item1.s.OverLap > 0) and (Item2.S.Name <> '') and (Item2.s.StdMode in [0, 2, 3, 31, 40, 41, 42, 46, 47]) and (Item2.s.OverLap > 0) and (Item1.S.Name = Item2.S.Name) and (Item1.s.StdMode = Item2.s.StdMode);
end;

function IsUnOverLapItem(Item:PTClientItem):Boolean;
begin
  // 叠加物品加入31 chongchong 2014-04-08
  Result := (Item.S.Name <> '') and (Item.s.StdMode in [0, 2, 3, 31, 40, 41, 42, 46, 47]) and (Item.Dura > 0) and (Item.s.OverLap > 0);
end;

function GetFeatureLen(nLen:Integer):Integer;
begin
  Result := nLen;
  if nLen = SizeOf(THumFeature) then
    Result := g_nHumFeature
  else if nLen = SizeOf(TMonFeature) then
    Result := g_nMonFeature;
end;

function GetRGB(c256:byte):Integer;
begin
  Result := RGB(g_DefColorTable[c256].rgbRed, g_DefColorTable[c256].rgbGreen, g_DefColorTable[c256].rgbBlue);
end;

function RGB32(C:LongInt; BitCount:Byte):TColor;
begin
  if BitCount = 16 then
    Result := RGB(C and $F8 shr 8, C and $FC shr 3, C and $F8 shl 3)
  else
    Result := C;
end;

{ if BitCount = 16 then   Result := RGB(C and $F8 shr 8, C and $FC shr 3, C and $F8 shl 3)
   Result := Word((r and $F8 shl 8) or (g and $FC shl 3) or (b and $F8 shr 3))
 else
   Result := RGB(b, g, r);  }

function GetCustomMagicConfig(MagicID:Word):PClientCustomMagicConfig;
var
  I:Integer;
  CustomMagicConfig:PClientCustomMagicConfig;
begin
  Result := nil;
  if not CheckIsCustomMagic(MagicID) then
    Exit;

  for I := 0 to g_CustomMagicConfig.Count - 1 do begin
    CustomMagicConfig := g_CustomMagicConfig[I];

    if CustomMagicConfig.wMagicID = MagicID then begin
      Result := CustomMagicConfig;
      Exit;
    end;
  end;
end;

function GetCustomNpcConfig(Appr:Word):PClientCustomNpcConfig;
var
  I:Integer;
  CustomNpcConfig:PClientCustomNpcConfig;
begin
  Result := nil;
  if not Appr < 10000 then
    Exit;

  for I := 0 to g_CustomNpcConfig.Count - 1 do begin
    CustomNpcConfig := g_CustomNpcConfig[I];

    if CustomNpcConfig.wNpcAppr = Appr then begin
      Result := CustomNpcConfig;
      Exit;
    end;
  end;
end;

procedure SendGameCenterMsg(wIdent:Word; sSendMsg:string);
var
  SendData:TCopyDataStruct;
  nParam:Integer;
begin
  nParam := MakeLong(Word(0), wIdent);
  SendData.cbData := Length(sSendMsg) + 1;
  GetMem(SendData.lpData, SendData.cbData);
  StrCopy(SendData.lpData, PChar(sSendMsg));
  SendData.dwData := g_dwGameLoginHandle;
  SendMessage(g_dwGameLoginHandle, WM_COPYDATA, nParam, Cardinal(@SendData));
  FreeMem(SendData.lpData);
end;

procedure ActorXYToMapXY(nCurrX, nCurrY:Integer; var nX, nY:Integer);
begin
  nX := nCurrX * 48 div 32;
  nY := nCurrY * 32 div 32;
end;

procedure MapXYToActorXY(nMapX, nMapY:Integer; var nX, nY:Integer);
begin
  nX := nMapX * 32 div 48;
  nY := nMapY * 32 div 32;
end;

procedure MapToScreen(nMapWH, nScreenWH, nMapXY:Integer; var nXY:Integer);
begin
  nXY := Round(nScreenWH * nMapXY / nMapWH);
end;

procedure ScreenToMap(nMapWH, nScreenWH, nScreenXY:Integer; var nXY:Integer);
begin
  nXY := Round(nMapWH * nScreenXY / nScreenWH);
end;

procedure AddFreeActorList(Actor:TActor);
var
  I:Integer;
  boFind:Boolean;
begin
  if g_TargetCret = Actor then begin
    {$IF IsMultiThreadRender = 1}
    EnterCriticalSection(g_ActorLock);
    {$IFEND}
    g_TargetCret := nil;
    {$IF IsMultiThreadRender = 1}
    LeaveCriticalSection(g_ActorLock);
    {$IFEND}
  end;
  if g_FocusCret = Actor then begin
    {$IF IsMultiThreadRender = 1}
    EnterCriticalSection(g_ActorLock);
    {$IFEND}
    g_FocusCret := nil;
    g_FocusCretTick := MyGetTickCount;
    {$IF IsMultiThreadRender = 1}
    LeaveCriticalSection(g_ActorLock);
    {$IFEND}
  end;
  if g_MagicTarget = Actor then begin
    {$IF IsMultiThreadRender = 1}
    EnterCriticalSection(g_ActorLock);
    {$IFEND}
    g_MagicTarget := nil;
    {$IF IsMultiThreadRender = 1}
    LeaveCriticalSection(g_ActorLock);
    {$IFEND}
  end;
  if g_MyHero = Actor then begin
    {$IF IsMultiThreadRender = 1}
    EnterCriticalSection(g_ActorLock);
    {$IFEND}
    g_MyHero := nil;
    {$IF IsMultiThreadRender = 1}
    LeaveCriticalSection(g_ActorLock);
    {$IFEND}
  end;

  EnterCriticalSection(g_FreeMemorysLock);
  boFind := False;
  for I := g_FreeActorList.Count - 1 downto 0 do begin
    if g_FreeActorList.Items[I] = Actor then begin
      boFind := True;
      break;
    end;
  end;
  if not boFind then
    g_FreeActorList.Add(Actor);

  if (Actor.m_btRace = 0) and (g_MySelf <> nil) then begin
    I := g_MySelf.m_FriendHitList.IndexOf(Actor.m_sUserName);
    if I <> -1 then begin
      g_MySelf.m_FriendHitList.Delete(I);
    end;
  end;

  LeaveCriticalSection(g_FreeMemorysLock);
end;

// modify chongchong 2013-07-18

procedure AddFreeEffectList(Effect:TMagicEff);
var
  I:Integer;
  boFind:Boolean;
begin
  EnterCriticalSection(g_FreeMemorysLock);
  try
    boFind := False;
    for I := g_FreeEffectList.Count - 1 downto 0 do begin
      if g_FreeEffectList.Items[I] = Effect then begin
        boFind := True;
        break;
      end;
    end;
    if not boFind then begin
      g_FreeEffectList.Add(Effect);
    end;
  finally
    LeaveCriticalSection(g_FreeMemorysLock);
  end;
end;

procedure AddFreeEventList(Event:TClEvent);
var
  I:Integer;
  boFind:Boolean;
begin
  EnterCriticalSection(g_FreeMemorysLock);

  boFind := False;
  for I := g_FreeEventList.Count - 1 downto 0 do begin
    if g_FreeEventList.Items[I] = Event then begin
      boFind := True;
      break;
    end;
  end;
  if not boFind then
    g_FreeEventList.Add(Event);

  LeaveCriticalSection(g_FreeMemorysLock);
end;

function FindBoxKey(BoxType:Integer):Integer;
var
  I:Integer;
begin
  Result := -1;
  for I := 0 to GetMaxBagCount - 1 do begin
    if (g_ItemArr[I].S.Name <> '') and (g_ItemArr[I].S.StdMode = 46) and (g_ItemArr[I].S.Shape - 10 = BoxType) then begin
      Result := I;
      Exit;
    end;
  end;
end;

function GetInputKey(Key:Integer; var Shift:TShiftState):Word;
begin
  Shift := [];
  Result := Loword(Key);
  if Hiword(Key) = 1 then
    Shift := [ssShift];
  if Hiword(Key) = 2 then
    Shift := [ssCtrl];
  if Hiword(Key) = 4 then
    Shift := [ssAlt];
end;

function GetKeyDownStr(Key:Word; Shift:TShiftState; var nKey:Integer):string;

  function GetKey(Key:Word):string;
  begin
    nKey := 0;
    if ((Key >= 48) and (Key <= 57)) or ((Key >= 65) and (Key <= 90)) or ((Key >= 96) and (Key <= 105)) then begin
      Result := Chr(Key);
      nKey := Key;
    end;
    if Key = VK_TAB then begin
      nKey := Key;
      Result := 'Tab';
    end;
    if Key = VK_SCROLL then begin
      nKey := Key;
      Result := 'Scroll';
    end;
    if Key in [VK_F1..VK_F12] then begin
      nKey := Key;
      case Key of
        VK_F1:
          Result := 'F1';
        VK_F2:
          Result := 'F2';
        VK_F3:
          Result := 'F3';
        VK_F4:
          Result := 'F4';
        VK_F5:
          Result := 'F5';
        VK_F6:
          Result := 'F6';
        VK_F7:
          Result := 'F7';
        VK_F8:
          Result := 'F8';
        VK_F9:
          Result := 'F9';
        VK_F10:
          Result := 'F10';
        VK_F11:
          Result := 'F11';
        VK_F12:
          Result := 'F12';
      end;
    end;
    if (Key >= 186) and (Key <= 222) then {// 其他键} begin
      nKey := Key;
      case Key of
        186:
          Result := ';';
        187:
          Result := '=';
        188:
          Result := ',';
        189:
          Result := '-';
        190:
          Result := '.';
        191:
          Result := '/';
        192:
          Result := '`';
        219:
          Result := '[';
        220:
          Result := '\';
        221:
          Result := ']';
        222:
          Result := Char(27);
      end;
    end;

    if (Key >= 8) and (Key <= 46) then {// 方向键} begin
      nKey := Key;
      case Key of
        8:
          Result := '退格';
        9:
          Result := 'Tab';
        13:
          Result := 'Enter';
        32:
          Result := '空格';
        33:
          Result := 'PageUp';
        34:
          Result := 'PageDown';
        35:
          Result := 'End';
        36:
          Result := 'Home';
        45:
          Result := 'Insert';
        46:
          Result := 'Delete';
      end;
    end;
  end;

var
  nK:Integer;
  sKey:string;
begin
  nK := 0;
  sKey := '';
  Result := '';
  if (Key <> VK_MENU) and (Key <> VK_CONTROL) and (Key <> VK_SHIFT) and (Key <> VK_TAB) and (Key <> VK_SCROLL) and (((Key >= 48) and (Key <= 57)) or ((Key >= 65) and (Key <= 90)) or ((Key >= 96) and (Key <= 105)) or (Key >= 186) and (Key <= 222)) then begin
    if ssShift in Shift then begin
      Result := 'Shift+';
      nK := 1;
    end
    else if ssAlt in Shift then begin
      Result := 'Alt+';
      nK := 4;
    end
    else if ssCtrl in Shift then begin
      Result := 'Ctrl+';
      nK := 2;
    end;
  end;
  sKey := GetKey(Key);
  if sKey <> '' then begin
    nKey := MakeLong(nKey, nK);
    Result := Result + sKey;
  end;
end;

function THttpThread.WebPagePost(sURL, sPostData:string):string;
const
  RequestMethod = 'POST';
  HTTP_VERSION = 'HTTP/1.1'; //HTTP版本 我抓包看过 HTTP/1.0 HTTP/1.1。尚未仔细了解其区别。按MSDN来写的。留空默认是1.0
var
  dwSize:DWORD;
  dwFileSize:Int64;
  dwBytesRead, dwReserved:DWORD;
  hInte, hConnection, hRequest:HInternet;
  ContentSize:array[1..1024] of Char;
  HostPort:Integer;
  HostName, FileName, sHeader:string;
  Buffer:array[1..1024] of Char;

  procedure ParseURL(URL:string; var HostName, FileName:string; var HostPort:Integer);
  var
    i, p, k:Integer; //DWORD; HZQ 20230524

    function StrToIntDef(const S:string; Default:Integer):Integer;
    var
      E:Integer;
    begin
      Val(S, Result, E);
      if E <> 0 then
        Result := Default;
    end;

  begin
    if lstrcmpi('http://', PChar(Copy(URL, 1, 7))) = 0 then
      System.Delete(URL, 1, 7);
    HostName := URL;
    FileName := '/';
    HostPort := INTERNET_DEFAULT_HTTP_PORT;
    i := Pos('/', URL);
    if i > 0 then begin
      HostName := Copy(URL, 1, i - 1);
      FileName := Copy(URL, i, Length(URL) - i + 1);
    end;
    p := pos(':', HostName);
    if p <> 0 then begin
      k := Length(HostName) - p;
      HostPort := StrToIntDef(Copy(HostName, p + 1, k), INTERNET_DEFAULT_HTTP_PORT);
      Delete(HostName, p, k + 1);
    end;
  end;

begin
  Result := '';
  //dwFileSize := 0; //hzq
  ParseURL(sURL, HostName, FileName, HostPort); // 函数原型见 http://technet.microsoft.com/zh-cn/subscriptions/aa385096(v=vs.85).aspx
  hInte := InternetOpen('', //UserAgent
    INTERNET_OPEN_TYPE_PRECONFIG, nil, nil, 0);
  if hInte <> nil then begin
    hConnection := InternetConnect(hInte, // 函数原型见 http://technet.microsoft.com/zh-cn/query/ms909418
      PChar(HostName), HostPort, nil, nil, INTERNET_SERVICE_HTTP, 0, 0);
    if hConnection <> nil then begin
      hRequest := HttpOpenRequest(hConnection, // 函数原型见 http://msdn.microsoft.com/zh-cn/library/aa917871
        PChar(RequestMethod), PChar(FileName), HTTP_VERSION, '', //Referrer 来路
        nil, //AcceptTypes 接受的文件类型 TEXT/HTML */*
        INTERNET_FLAG_NO_CACHE_WRITE or INTERNET_FLAG_RELOAD, 0);
      if hRequest <> nil then begin
        sHeader := 'Content-Type: application/x-www-form-urlencoded' + #13#10; //    +'CLIENT-IP: 216.13.23.33'+#13#10
        //    'X-FORWARDED-FOR: 216.13.23.33' + #13#10+; 伪造代理IP
     // 函数原型见 http://msdn.microsoft.com/zh-cn/library/aa384227(v=VS.85)
        HttpAddRequestHeaders(hRequest, PChar(sHeader), Length(sHeader), HTTP_ADDREQ_FLAG_ADD or HTTP_ADDREQ_FLAG_REPLACE); // 函数原型见 http://msdn.microsoft.com/zh-cn/library/windows/desktop/aa384247(v=vs.85).aspx
        if HttpSendRequest(hRequest, nil, 0, PChar(sPostData), Length(sPostData)) then begin
          dwReserved := 0;
          dwSize := SizeOf(ContentSize); // 函数原型 http://msdn.microsoft.com/zh-cn/subscriptions/downloads/aa384238.aspx
          if HttpQueryInfo(hRequest, HTTP_QUERY_CONTENT_LENGTH, @ContentSize, dwSize, dwReserved) then begin
            dwFileSize := StrToInt(@ContentSize);
            //            GetMem(Result, dwFileSize);
            InternetReadFile(hRequest, @Buffer, dwFileSize, dwBytesRead);
            Result := Buffer;
          end;
        end;
      end;
      InternetCloseHandle(hRequest);
    end;
    InternetCloseHandle(hConnection);
  end;
  InternetCloseHandle(hInte);
end;

procedure THttpThread.Post(URL, Data:string; Res:TStream);
var
  hInt, hConn, hreq:HINTERNET;
  buffer:PChar;
  dwRead, dwFlags:cardinal;
  port:Word;
  {$IFDEF USE_IDHTTP}
  uri:TIdURI;
  {$ELSE}
   uri:THttpClient.TUri;
  {$ENDIF}
  proto, host, path:string;
  {dwError,} dwBuffLen:Cardinal;
begin
  {$IFDEF USE_IDHTTP}
  uri := TIdURI.Create(URL);
  host := uri.Host;
  path := uri.Path + uri.Document;
  proto := uri.Protocol;
  uri.Free;
  {$ELSE}
  uri := THttpClient.ParseURL(URL);
  host := uri.Host;
  path := uri.Path + uri.Doc;
  proto := uri.Protocol;
  {$ENDIF}

  if UpperCase(proto) = 'HTTPS' then begin
    port := INTERNET_DEFAULT_HTTPS_PORT;
    dwFlags := INTERNET_FLAG_SECURE;
  end else begin
    port := INTERNET_INVALID_PORT_NUMBER;
    dwFlags := INTERNET_FLAG_RELOAD;
  end;
  hInt := InternetOpen('Delphi', INTERNET_OPEN_TYPE_PRECONFIG, nil, nil, 0);
  hConn := InternetConnect(hInt, PChar(host), port, nil, nil, INTERNET_SERVICE_HTTP, 0, 0);
  hreq := HttpOpenRequest(hConn, 'POST', PChar(path), 'HTTP/1.1', nil, nil, dwFlags, 0);

  InternetQueryOption(hreq, INTERNET_OPTION_SECURITY_FLAGS, @dwFlags, dwBuffLen);

  dwFlags := dwFlags or SECURITY_FLAG_IGNORE_REVOCATION;

  InternetSetOption(hreq, INTERNET_OPTION_SECURITY_FLAGS, @dwFlags, sizeof(dwFlags));

  GetMem(buffer, 65536);
  if HttpSendRequest(hreq, nil, 0, PChar(Data), Length(Data)) then begin
    dwRead := 0;
    repeat
      InternetReadFile(hreq, buffer, 65536, dwRead);
      if dwRead <> 0 then
        Res.Write(buffer^, dwRead);
    until dwRead = 0;
  end;
  //  ShowMessage(IntToStr(GetLastError));
  InternetCloseHandle(hreq);
  InternetCloseHandle(hConn);
  InternetCloseHandle(hInt);
  FreeMem(buffer);
end;

procedure HttpPost(btType:Byte; Param1:string; Param2:string; Param3:string; Param4:string);
begin
  THttpThread.Create(btType, Param1, Param2, Param3, Param4);
end;

constructor THttpThread.Create(const btPostType:Byte; Param1:string; Param2:string; Param3:string; Param4:string);
begin
  FreeOnTerminate := True;
  FPostType := btPostType;
  FParam1 := Param1;
  FParam2 := Param2;
  FParam3 := Param3;
  FParam4 := Param4;
  inherited Create(False);
end;

procedure THttpThread.Execute;
begin
  FreeOnTerminate := True;
  case FPostType of
    0:
      GetPayMentURL(FParam1, FParam2, FParam3);
    //    1:
    //      CheckAD;
  end;
end;

{$MESSAGE HINT '此处调用支付接口，需要修改'} //HZQ

procedure THttpThread.CallPayMentURL(PayMode:Boolean; PayInfo:string);
//PayMode 0 :WX  1:Alipay
var
  Url:string; //请求地址
  ResponseStream:TStringStream; //返回信息
  ResponseStr:string;
  aJson:ISuperObject;
  //aSuperArray: TSuperArray;
begin
  {$I VMProtectBeginUltra.inc}
  if PayMode then begin
    //Url := 'https://wxpay.geem2.com/wx/gmuser/client/checkAlipay'
    Url := 'https://wxpay.gxxm2.com/wx/gmuser/client/checkAlipay'
  end else begin
    //Url := 'https://wxpay.geem2.com/wx/gmuser/client/queryPay';
    Url := 'https://wxpay.gxxm2.com/wx/gmuser/client/queryPay';
  end;
  ResponseStream := TStringStream.Create('');
  try
    Post(Url, '{"out_trade_no":"' + PayInfo + '"}', ResponseStream);
    ResponseStr := UTF8Decode(ResponseStream.DataString);
    aJson := SO(ResponseStr);
    //    if aJson['data.code_url'] <> nil then
    //    begin
    //      g_sPayMentQRCodeURL := aJson['data.code_url'].AsString;
    //    end;
  finally
    ResponseStream.Free;
  end;
  {$I VMProtectEnd.inc}
end;

{$MESSAGE HINT '此处获取付款URL，需要修改'} //HZQ

procedure THttpThread.GetPayMentURL(PayType:string; PayPrice:string; currencytype:string);
var
  //Url: string; //请求地址
  ResponseStream:TStringStream; //返回信息
  ResponseStr:string;
  aJson:ISuperObject;
  //aSuperArray: TSuperArray;
begin
  {$I VMProtectBeginUltra.inc}
  ResponseStream := TStringStream.Create('');
  try
    (*
    Post('https://wxpay.geem2.com/wx/gmuser/client/generateOrder', //
      '{"paytype":"' + PayType + '","pirce":"' + PayPrice + '","currencytype":"' +
       currencytype + '","account":"' + TSerialWindows(FrmDlg).m_sLoginId +
        '","playername":"' + Utf8Encode(g_MySelf.m_sUserName{'一支笔'}) +
         '","editionId":"' + IntToStr(g_ClientConfig.nEditionId) +
          '","areaId":"' + IntToStr(g_ClientConfig.nAreaId) + '"}', ResponseStream);
    *)
    Post('https://wxpay.gxxm2.com/wx/gmuser/client/generateOrder', //
      '{"paytype":"' + PayType + '","pirce":"' + PayPrice + '","currencytype":"' +
      currencytype + '","account":"' + TSerialWindows(FrmDlg).m_sLoginId +
      '","playername":"' + Utf8Encode(g_MySelf.m_sUserName {'一支笔'}) +
      '","editionId":"' + IntToStr(g_ClientConfig.nEditionId) +
      '","areaId":"' + IntToStr(g_ClientConfig.nAreaId) + '"}', ResponseStream);
    ResponseStr := UTF8Decode(ResponseStream.DataString);
    aJson := SO(ResponseStr);
    if aJson['data.code_url'] <> nil then begin
      g_sPayMentQRCodeURL := aJson['data.code_url'].AsString;
    end;
    if aJson['data.out_trade_no'] <> nil then begin
      CallPayMentURL(false, aJson['data.out_trade_no'].AsString);
    end;
  finally
    ResponseStream.Free;
  end;
  {$I VMProtectEnd.inc}
end;

constructor TImageList.Create();
begin
  // FillChar(WMonImagesArr, SizeOf(TGameImages) * 100, 0);
  // SetLength(ImagesArr, 0);
  ImagesArr := nil;
end;

destructor TImageList.Destroy;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
  ImagesArr := nil;
end;

function TImageList.ImageOf(Index:Integer):TGameImages;
begin
  Result := nil; //HZQ 20230524基类添加一个空值返回，消除编译器警告
end;

function TImageList.IndexOf(Index:Integer):TGameImages;
begin
  Result := nil; //HZQ 20230524基类添加一个空值返回，消除编译器警告
end;

procedure TImageList.Initialize;
begin
end;

procedure TImageList.Finalize;
begin
end;

function TImageList.GetCount:Integer;
begin
  Result := Length(ImagesArr);
end;

procedure TImageList.SetIndex(Index:Integer; Images:TGameImages);
begin
  if (Index >= Low(ImagesArr)) and (Index <= High(ImagesArr)) then begin
    ImagesArr[Index] := Images;
  end;
end;

function TImageList.GetCachedGrayImage(Index:Integer; var PX, PY:Integer):TTexture;
var
  Images:TGameImages;
begin
  Result := nil;
  Images := Indexs[0];
  if Images <> nil then
    Result := Images.GetCachedGrayImage(Index, PX, PY);
end;

function TImageList.GetCachedBrightImage(Index:Integer; var PX, PY:Integer):TTexture;
var
  Images:TGameImages;
begin
  Result := nil;
  Images := Indexs[0];
  if Images <> nil then
    Result := Images.GetCachedBrightImage(Index, PX, PY);
end;

function TImageList.GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
var
  Images:TGameImages;
begin
  Result := nil;
  Images := Indexs[0];
  if Images <> nil then
    Result := Images.GetCachedImage(Index, PX, PY);
end;

procedure TImageList.FreeOldMemorys;
var
  I:Integer;
begin
  for I := 0 to Length(ImagesArr) - 1 do begin
    if (ImagesArr[I] <> nil) and ImagesArr[I].Initialized then
      ImagesArr[I].FreeOldMemorys;
  end;
end;

procedure TImageList.ClearCache;
var
  I:Integer;
begin
  for I := 0 to Length(ImagesArr) - 1 do begin
    if (ImagesArr[I] <> nil) and ImagesArr[I].Initialized then
      ImagesArr[I].ClearCache;
  end;
end;
{------------------------------------------------------------------------------}

procedure TTilesList.ClearCache;
var
  I:Integer;
begin
  inherited;
  for I := 0 to Length(HMapImagesArr) - 1 do begin
    if (HMapImagesArr[I] <> nil) and HMapImagesArr[I].Initialized then
      HMapImagesArr[I].ClearCache;
  end;
end;

destructor TTilesList.Destroy;
var
  I:Integer;
begin
  for I := Low(HMapImagesArr) to High(HMapImagesArr) do begin
    if HMapImagesArr[I] <> nil then begin
      HMapImagesArr[I].Finalize;
      FreeAndNil(HMapImagesArr[I]);
    end;
  end;
  SetLength(HMapImagesArr, 0);
  HMapImagesArr := nil;
  inherited;
end;

procedure TTilesList.FreeOldMemorys;
var
  I:Integer;
begin
  inherited;

  for I := 0 to Length(HMapImagesArr) - 1 do begin
    if (HMapImagesArr[I] <> nil) and HMapImagesArr[I].Initialized then
      HMapImagesArr[I].FreeOldMemorys;
  end;
end;

function TTilesList.GetGameImages(Index:Integer; IsHMap:Boolean):TGameImages;
var
  sFileName, sGetFileName:string;
  nUnit:Integer;
begin
  nUnit := Index;

  if IsHMap then begin
    if nUnit = 0 then
      sFileName := g_sSelfResourcePath + HMAP_TITLESIMAGEFILE
    else
      sFileName := g_sSelfResourcePath + Format(HMAP_TITLESIMAGEFILES, [nUnit + 1]);

    sGetFileName := sFileName;
    //if GetHMapDataFiles(sFileName, sGetFileName) then
    begin
      if Index < Low(HMapImagesArr) then
        nUnit := 0;
      if Index > High(HMapImagesArr) then
        SetLength(HMapImagesArr, Index + 1);
      if HMapImagesArr[nUnit] = nil then begin
        HMapImagesArr[nUnit] := CreateHMapDataGameImages(sGetFileName);
        HMapImagesArr[nUnit].Initialize;
      end;
      Result := HMapImagesArr[nUnit];
      Exit;
    end;
  end;

  if Index < Low(ImagesArr) then
    nUnit := 0;
  if Index > High(ImagesArr) then
    SetLength(ImagesArr, Index + 1);
  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + TITLESIMAGEFILE
    else
      sFileName := g_sSelfFilePath + Format(TITLESIMAGEFILES, [nUnit + 1]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
    // DebugOutStr('TTilesList:'  + sFileName);
  end;
  Result := ImagesArr[nUnit];
end;

procedure TTilesList.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[nUnit] = nil then begin
      if nUnit = 0 then
        sFileName := g_sSelfFilePath + TITLESIMAGEFILE
      else
        sFileName := g_sSelfFilePath + Format(TITLESIMAGEFILES, [nUnit + 1]);
      ImagesArr[nUnit] := CreateGameImages(sFileName);
      ImagesArr[nUnit].Initialize;
    end;
  end;

  for nUnit := Low(HMapImagesArr) to High(HMapImagesArr) do begin
    if HMapImagesArr[nUnit] = nil then begin
      if nUnit = 0 then
        sFileName := g_sSelfResourcePath + HMAP_TITLESIMAGEFILE
      else
        sFileName := g_sSelfResourcePath + Format(HMAP_TITLESIMAGEFILES, [nUnit + 1]);
      HMapImagesArr[nUnit] := CreateHMapDataGameImages(sFileName);
      HMapImagesArr[nUnit].Initialize;
    end;
  end;
end;

procedure TTilesList.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);

  for I := Low(HMapImagesArr) to High(HMapImagesArr) do begin
    if HMapImagesArr[I] <> nil then begin
      HMapImagesArr[I].Finalize;
      FreeAndNil(HMapImagesArr[I]);
    end;
  end;
  SetLength(HMapImagesArr, 0);
end;
{------------------------------------------------------------------------------}

procedure TSmTilesList.ClearCache;
var
  I:Integer;
begin
  inherited;
  for I := 0 to Length(HMapImagesArr) - 1 do begin
    if (HMapImagesArr[I] <> nil) and HMapImagesArr[I].Initialized then
      HMapImagesArr[I].ClearCache;
  end;
end;

destructor TSmTilesList.Destroy;
var
  I:Integer;
begin
  for I := Low(HMapImagesArr) to High(HMapImagesArr) do begin
    if HMapImagesArr[I] <> nil then begin
      HMapImagesArr[I].Finalize;
      FreeAndNil(HMapImagesArr[I]);
    end;
  end;
  SetLength(HMapImagesArr, 0);
  HMapImagesArr := nil;
  inherited;
end;

procedure TSmTilesList.FreeOldMemorys;
var
  I:Integer;
begin
  inherited;

  for I := 0 to Length(HMapImagesArr) - 1 do begin
    if (HMapImagesArr[I] <> nil) and HMapImagesArr[I].Initialized then
      HMapImagesArr[I].FreeOldMemorys;
  end;
end;

function TSmTilesList.GetGameImages(Index:Integer; IsHMap:Boolean):TGameImages;
var
  sFileName, sGetFileName:string;
  nUnit:Integer;
begin
  nUnit := Index;

  if IsHMap then begin
    if nUnit = 0 then
      sFileName := g_sSelfResourcePath + HMAP_SMLTITLESIMAGEFILE
    else
      sFileName := g_sSelfResourcePath + Format(HMAP_SMLTITLESIMAGEFILES, [nUnit + 1]);

    sGetFileName := sFileName;
    //if GetHMapDataFiles(sFileName, sGetFileName) then
    begin
      if Index < Low(HMapImagesArr) then
        nUnit := 0;
      if Index > High(HMapImagesArr) then
        SetLength(HMapImagesArr, Index + 1);
      if HMapImagesArr[nUnit] = nil then begin
        HMapImagesArr[nUnit] := CreateHMapDataGameImages(sGetFileName);
        HMapImagesArr[nUnit].Initialize;
      end;
      Result := HMapImagesArr[nUnit];
      Exit;
    end;
  end;

  if Index < Low(ImagesArr) then
    nUnit := 0;
  if Index > High(ImagesArr) then
    SetLength(ImagesArr, Index + 1);
  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + SMLTITLESIMAGEFILE
    else
      sFileName := g_sSelfFilePath + Format(SMLTITLESIMAGEFILES, [nUnit + 1]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

procedure TSmTilesList.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[nUnit] = nil then begin
      if nUnit = 0 then
        sFileName := g_sSelfFilePath + SMLTITLESIMAGEFILE
      else
        sFileName := g_sSelfFilePath + Format(SMLTITLESIMAGEFILES, [nUnit + 1]);
      ImagesArr[nUnit] := CreateGameImages(sFileName);
      ImagesArr[nUnit].Initialize;
    end;
  end;

  for nUnit := Low(HMapImagesArr) to High(HMapImagesArr) do begin
    if HMapImagesArr[nUnit] = nil then begin
      if nUnit = 0 then
        sFileName := g_sSelfResourcePath + HMAP_SMLTITLESIMAGEFILE
      else
        sFileName := g_sSelfResourcePath + Format(HMAP_SMLTITLESIMAGEFILES, [nUnit + 1]);
      HMapImagesArr[nUnit] := CreateHMapDataGameImages(sFileName);
      HMapImagesArr[nUnit].Initialize;
    end;
  end;
end;

procedure TSmTilesList.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);

  for I := Low(HMapImagesArr) to High(HMapImagesArr) do begin
    if HMapImagesArr[I] <> nil then begin
      HMapImagesArr[I].Finalize;
      FreeAndNil(HMapImagesArr[I]);
    end;
  end;
  SetLength(HMapImagesArr, 0);
end;
// =============== TMonImageList================

function TMonImageList.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := Index;
  if nUnit = 80 then begin
    // sFileName := g_sSelfFilePath + DRAGONIMAGEFILE;
    Result := g_WDragonImg;
    Exit;
  end;
  if nUnit = 90 then begin
    // sFileName := g_sSelfFilePath + EFFECTIMAGEFILE;
    Result := g_WEffectImg;
    Exit;
  end;

  // DebugOutStr('Index1:' + IntToStr(Index) + ' nUnit:' + IntToStr(nUnit));
  if Index < Low(ImagesArr) then
    nUnit := 1;
  if Index > High(ImagesArr) then
    SetLength(ImagesArr, Index + 1);
  if ImagesArr[nUnit] = nil then begin
    sFileName := g_sSelfFilePath + Format(MONIMAGEFILE, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := sFileName;
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

function TMonImageList.ImageOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  Result := nil; //HZQ 20230524 默认空值，消除编译器警告

  nUnit := Index div 10; // 这里是数据Appr字段 div 10
  // MON36.wil 怪物超过10个了，需要特殊处理，从600 - 620 占用 piaoyun 2013-12-07
  if (nUnit = 60) or (nUnit = 61) or (nUnit = 62) then begin
    if nUnit > High(ImagesArr) then
      SetLength(ImagesArr, nUnit + 1);
    if ImagesArr[nUnit] = nil then begin
      sFileName := g_sSelfFilePath + Format(MONIMAGEFILE, [36]);
      ImagesArr[nUnit] := CreateGameImages(sFileName);
      // ImagesArr[nUnit].FileName := sFileName;
      ImagesArr[nUnit].Initialize;
    end;
    Result := ImagesArr[nUnit];
  end else if (nUnit = 63) or (nUnit = 64) then begin // MON38.wil 怪物超过10个了，需要特殊处理，从630 - 640 占用 piaoyun 2014-01-03
    if nUnit > High(ImagesArr) then
      SetLength(ImagesArr, nUnit + 1);
    if ImagesArr[nUnit] = nil then begin
      sFileName := g_sSelfFilePath + Format(MONIMAGEFILE, [38]);
      ImagesArr[nUnit] := CreateGameImages(sFileName);
      // ImagesArr[nUnit].FileName := sFileName;
      ImagesArr[nUnit].Initialize;
    end;
    Result := ImagesArr[nUnit];
  end else if (nUnit = 95) then begin // 新怪物测试 95(数据库appr字段950) 为我加的新骷髅 Mon-kulou.wzi piaoyun 2013-07-27
    if nUnit > High(ImagesArr) then
      SetLength(ImagesArr, nUnit + 1);
    if ImagesArr[nUnit] = nil then begin
      sFileName := g_sSelfFilePath + MONKULOUIMAGEFILE;
      ImagesArr[nUnit] := CreateGameImages(sFileName);
      // ImagesArr[nUnit].FileName := sFileName;
      ImagesArr[nUnit].Initialize;
    end;
    Result := ImagesArr[nUnit];
  end else if (nUnit = 80) or (nUnit = 90) then begin
    if nUnit = 80 then
      Result := g_WDragonImg;
    if nUnit = 90 then
      Result := g_WEffectImg;

    // 沙巴克一个城门、三个城墙分别设置为 900- 903  所以90保留了 80不知道什么东西 piaoyun 2013-07-27
    if (nUnit = 90) and (Index mod 10 > 3) then begin // Index mod 10 > 3  904开始 新沙巴克城墙素材
      nUnit := 34; // 新沙巴克城墙素材在Mon34.wzl里
      if nUnit > High(ImagesArr) then
        SetLength(ImagesArr, nUnit + 1);
      if ImagesArr[nUnit] = nil then begin
        sFileName := g_sSelfFilePath + Format(MONIMAGEFILE, [nUnit]);
        ImagesArr[nUnit] := CreateGameImages(sFileName);
        // ImagesArr[nUnit].FileName := sFileName;
        ImagesArr[nUnit].Initialize;
      end;
      Result := ImagesArr[nUnit];
      // DebugOutStr('Index2:' + IntToStr(Index) + ' nUnit:' + IntToStr(nUnit));
    end;
  end else begin
    Inc(nUnit);
    if nUnit < Low(ImagesArr) then
      nUnit := 1;
    if nUnit > High(ImagesArr) then
      SetLength(ImagesArr, nUnit + 1);
    if ImagesArr[nUnit] = nil then begin
      sFileName := g_sSelfFilePath + Format(MONIMAGEFILE, [nUnit]);
      ImagesArr[nUnit] := CreateGameImages(sFileName);
      // ImagesArr[nUnit].FileName := sFileName;
      ImagesArr[nUnit].Initialize;
    end;
    Result := ImagesArr[nUnit];
  end;
  //
end;

procedure TMonImageList.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    sFileName := g_sSelfFilePath + Format(MONIMAGEFILE, [nUnit]);
    if ImagesArr[nUnit] = nil then begin
      ImagesArr[nUnit] := CreateGameImages(sFileName);
      ImagesArr[nUnit].Initialize;
    end;
  end;
end;

procedure TMonImageList.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
end;
// =============== THumImageList================

function THumImageList.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := Index;
  if nUnit < 0 then
    nUnit := 0;

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit < 1000 then begin
      if nUnit = 0 then
        sFileName := g_sSelfFilePath + HUMIMGIMAGESFILE
      else
        sFileName := g_sSelfFilePath + Format(HUMIMGIMAGESFILEX, [nUnit + 1]);
    end
    else begin
      if Length(g_ResourcesDir) > 0 then
        sFileName := g_sSelfResourcePath + Format(HumImageDir, [nUnit])
      else
        sFileName := g_sSelfFilePath + Format(HumImageDir, [nUnit]);
    end;
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

function THumImageList.GetWHumGrayImg(Dress, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
var
  FileName:string;
  nUnit:Integer;
  nShape:Integer;
  nDress:Integer;
begin
  nShape := (Dress - Sex) div 2;
  if nShape < 1000 then begin
    if nShape <= 99 then begin // Hum.wzl  0~99
      nUnit := 0;
      nDress := Dress;
      { end
        else if nShape <= 119 then begin // Hum2.wzl 100~119
         nUnit := 1;
         nDress := (nShape - 100) * 2 + Sex;
       end
       else  if nShape <= 159 then begin // Hum3.wzl  120~159
         nUnit := 2;
         nDress := (nShape - 120) * 2 + Sex;   }
    end
    else begin
      nUnit := (nShape - 100) div 50 + 1;
      nDress := ((nShape - 100) mod 50) * 2 + Sex;
    end;

    if Length(ImagesArr) <= nUnit then
      SetLength(ImagesArr, nUnit + 1);

    if ImagesArr[nUnit] = nil then begin
      if nUnit = 0 then
        FileName := g_sSelfFilePath + HUMIMGIMAGESFILE
      else
        FileName := g_sSelfFilePath + Format(HUMIMGIMAGESFILEX, [nUnit + 1]);
      ImagesArr[nUnit] := CreateGameImages(FileName);
      // ImagesArr[nUnit].FileName := g_sSelfFilePath + FileName;
      ImagesArr[nUnit].Initialize;
    end;
    Result := ImagesArr[nUnit].GetCachedGrayImage(HUMANFRAME * nDress + Frame, Ax, Ay);
  end
  else begin
    if Length(ImagesArr) <= nShape then
      SetLength(ImagesArr, nShape + 1);
    if ImagesArr[nShape] = nil then begin
      if Length(g_ResourcesDir) > 0 then
        FileName := g_sSelfResourcePath + Format(HumImageDir, [nShape])
      else
        FileName := g_sSelfFilePath + Format(HumImageDir, [nShape]);
      ImagesArr[nShape] := CreateGameImages(FileName);
      ImagesArr[nShape].Initialize;
    end;
    Result := ImagesArr[nShape].GetCachedGrayImage(HUMANFRAME * Sex + Frame, Ax, Ay);
  end;
end;

function THumImageList.GetWHumBrightImg(Dress, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
var
  FileName:string;
  nUnit:Integer;
  nShape:Integer;
  nDress:Integer;
begin
  nShape := (Dress - Sex) div 2;
  if nShape < 1000 then begin
    if nShape <= 99 then begin // Hum.wzl  0~99
      nUnit := 0;
      nDress := Dress;
      {end
       else if nShape <= 119 then begin // Hum2.wzl 100~119
        nUnit := 1;
        nDress := (nShape - 100) * 2 + Sex;
      end
      else  if nShape <= 159 then begin // Hum3.wzl  120~159
        nUnit := 2;
        nDress := (nShape - 120) * 2 + Sex;    }
    end
    else begin
      nUnit := (nShape - 100) div 50 + 1;
      nDress := ((nShape - 100) mod 50) * 2 + Sex;
    end;

    if Length(ImagesArr) <= nUnit then
      SetLength(ImagesArr, nUnit + 1);

    if ImagesArr[nUnit] = nil then begin
      if nUnit = 0 then
        FileName := g_sSelfFilePath + HUMIMGIMAGESFILE
      else
        FileName := g_sSelfFilePath + Format(HUMIMGIMAGESFILEX, [nUnit + 1]);
      // DScreen.AddChatBoardString('FileName:'+FileName+' nDress:'+IntToStr(nDress), clyellow, clRed);
      ImagesArr[nUnit] := CreateGameImages(FileName);
      // ImagesArr[nUnit].FileName := g_sSelfFilePath + FileName;
      ImagesArr[nUnit].Initialize;
    end;
    Result := ImagesArr[nUnit].GetCachedBrightImage(HUMANFRAME * nDress + Frame, Ax, Ay);

  end
  else begin
    if Length(ImagesArr) <= nShape then
      SetLength(ImagesArr, nShape + 1);
    if ImagesArr[nShape] = nil then begin
      if Length(g_ResourcesDir) > 0 then
        FileName := g_sSelfResourcePath + Format(HumImageDir, [nShape])
      else
        FileName := g_sSelfFilePath + Format(HumImageDir, [nShape]);
      ImagesArr[nShape] := CreateGameImages(FileName);
      ImagesArr[nShape].Initialize;
    end;
    Result := ImagesArr[nShape].GetCachedBrightImage(HUMANFRAME * Sex + Frame, Ax, Ay);
  end;
end;

function THumImageList.GetWHumImg(Dress, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
var
  FileName:string;
  nUnit:Integer;
  nShape:Integer;
  nDress:Integer;
begin
  Result := nil;
  nShape := (Dress - Sex) div 2;

  if nShape < 1000 then begin

    if nShape <= 99 then begin // Hum.wzl  0~99
      nUnit := 0;
      nDress := Dress;
      { end
        else if nShape <= 119 then begin // Hum2.wzl 100~119
         nUnit := 1;
         nDress := (nShape - 100) * 2 + Sex;
       end
       else  if nShape <= 159 then begin // Hum3.wzl  120~159
         nUnit := 2;
         nDress := (nShape - 120) * 2 + Sex;  }
    end
    else begin
      nUnit := (nShape - 100) div 50 + 1;
      nDress := ((nShape - 100) mod 50) * 2 + Sex;
    end;

    { if nShape <= 99 then begin
       nUnit := 0;
       nDress := Dress;
       // 100 1 120 2
     end else begin
       nUnit := (nShape - 100) div 20 + 1;
      // 100=1                  hum2
      // 120=2                  hum3
      // 140=3                  hum4
       // if nUnit <= 0 then Inc(nUnit);
       nDress := ((nShape - 100) mod 20) * 2 + Sex;
     end; }

    if Length(ImagesArr) <= nUnit then
      SetLength(ImagesArr, nUnit + 1);

    if ImagesArr[nUnit] = nil then begin
      if nUnit = 0 then
        FileName := g_sSelfFilePath + HUMIMGIMAGESFILE
      else
        FileName := g_sSelfFilePath + Format(HUMIMGIMAGESFILEX, [nUnit + 1]);

      ImagesArr[nUnit] := CreateGameImages(FileName);

      {
      if nUnit = 7 then
        ImagesArr[nUnit].ResetWZLAlpha := False;
      }

      ImagesArr[nUnit].Initialize;
    end;
    if ImagesArr[nUnit] <> nil then
      Result := ImagesArr[nUnit].GetCachedImage(HUMANFRAME * nDress + Frame, Ax, Ay);
  end
  else begin
    if Length(ImagesArr) <= nShape then
      SetLength(ImagesArr, nShape + 1);
    if ImagesArr[nShape] = nil then begin
      if Length(g_ResourcesDir) > 0 then
        FileName := g_sSelfResourcePath + Format(HumImageDir, [nShape])
      else
        FileName := g_sSelfFilePath + Format(HumImageDir, [nShape]);
      ImagesArr[nShape] := CreateGameImages(FileName);
      ImagesArr[nShape].Initialize;
    end;
    Result := ImagesArr[nShape].GetCachedImage(HUMANFRAME * Sex + Frame, Ax, Ay);
  end;
end;

procedure THumImageList.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if nUnit < 1000 then begin
      if nUnit = 0 then
        sFileName := g_sSelfFilePath + HUMIMGIMAGESFILE
      else
        sFileName := g_sSelfFilePath + Format(HUMIMGIMAGESFILEX, [nUnit + 1]);
    end
    else begin
      if Length(g_ResourcesDir) > 0 then
        sFileName := g_sSelfResourcePath + Format(HumImageDir, [nUnit])
      else
        sFileName := g_sSelfFilePath + Format(HumImageDir, [nUnit]);
    end;

    // if FileExists(sFileName) then begin
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;
end;

procedure THumImageList.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;

  SetLength(ImagesArr, 0);
end;
// =============== TWeaponImageList================

{
Weapon.wzl Shape 1~99
Weapon2.wzl Shape 100~149
Weapon3.wzl Shape 150~199
Weapon4.wzl Shape 200~249
Weapon5.wzl Shape 250~299

Hum.wzl Shape 1~99
Hum2.wzl Shape 100~149
Hum3.wzl Shape 150~199
Hum4.wzl Shape 200~249
}

function TWeaponImageList.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin

  nUnit := Index;
  if nUnit < 0 then
    nUnit := 0;

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    { TODO -c待验证 -opiaoyun : 武器扩展遗留问题1 【2013-4-23】}
    if nUnit <= 1000 then begin
      if nUnit = 0 then
        sFileName := g_sSelfFilePath + WEAPONIMAGESFILE
      else
        sFileName := g_sSelfFilePath + Format(WEAPONIMAGESFILEX, [nUnit + 1]);
    end
    else begin
      if Length(g_ResourcesDir) > 0 then
        sFileName := g_sSelfResourcePath + Format(WeaponImageDir, [nUnit])
      else
        sFileName := g_sSelfFilePath + Format(WeaponImageDir, [nUnit]);
    end;

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

function TWeaponImageList.GetWWeaponGrayImg(Weapon, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
var
  FileName:string;
  FileIdx:Integer;
  nAppr:Integer;
  nShape:Integer;
  nWeapon:Integer;
begin
  Result := nil;
  FileIdx := Weapon - Sex;

  if (FileIdx <= 198) then begin
    if Length(ImagesArr) < 1 then
      SetLength(ImagesArr, 1);
    if ImagesArr[0] = nil then begin
      FileName := g_sSelfFilePath + WEAPONIMAGESFILE;
      ImagesArr[0] := CreateGameImages(FileName);
      ImagesArr[0].Initialize;
    end;
    if ImagesArr[0] <> nil then
      Result := ImagesArr[0].GetCachedGrayImage(HUMANFRAME * Weapon + Frame, Ax, Ay);
    Exit;
  end;

  nShape := FileIdx div 2;
  if nShape < 1000 then begin
    FileIdx := FileIdx - 200;
    nWeapon := Weapon - 200;

    nAppr := FileIdx div 100;
    nWeapon := nWeapon - nAppr * 100;
    nAppr := nAppr + 1;
    // end;

    FileName := g_sSelfFilePath + Format(WEAPONIMAGESFILEX, [nAppr + 1]);
    if nAppr > High(ImagesArr) then
      SetLength(ImagesArr, nAppr + 1);

    if ImagesArr[nAppr] = nil then begin
      ImagesArr[nAppr] := CreateGameImages(FileName);
      ImagesArr[nAppr].Initialize;
    end;

    if ImagesArr[nAppr] <> nil then
      Result := ImagesArr[nAppr].GetCachedGrayImage(HUMANFRAME * nWeapon + Frame, Ax, Ay);
  end
  else begin
    FileIdx := FileIdx div 2;

    if Length(g_ResourcesDir) > 0 then
      FileName := g_sSelfResourcePath + Format(WeaponImageDir, [FileIdx])
    else
      FileName := g_sSelfFilePath + Format(WeaponImageDir, [FileIdx]);

    if FileIdx > High(ImagesArr) then
      SetLength(ImagesArr, FileIdx + 1);

    if ImagesArr[FileIdx] = nil then begin
      ImagesArr[FileIdx] := CreateGameImages(FileName);
      ImagesArr[FileIdx].Initialize;
    end;

    if ImagesArr[FileIdx] <> nil then
      Result := ImagesArr[FileIdx].GetCachedGrayImage(HUMANFRAME * Sex + Frame, Ax, Ay);
  end;
end;

function TWeaponImageList.GetWWeaponBrightImg(Weapon, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
var
  FileName:string;
  FileIdx:Integer;
  nAppr:Integer;
  nShape:Integer;
  nWeapon:Integer;
begin
  Result := nil;
  FileIdx := Weapon - Sex;

  if (FileIdx <= 198) then begin
    if Length(ImagesArr) < 1 then
      SetLength(ImagesArr, 1);
    if ImagesArr[0] = nil then begin
      FileName := g_sSelfFilePath + WEAPONIMAGESFILE;
      ImagesArr[0] := CreateGameImages(FileName);
      ImagesArr[0].Initialize;
    end;
    if ImagesArr[0] <> nil then
      Result := ImagesArr[0].GetCachedBrightImage(HUMANFRAME * Weapon + Frame, Ax, Ay);
    Exit;
  end;
  nShape := FileIdx div 2;
  if nShape < 1000 then begin
    FileIdx := FileIdx - 200;
    nWeapon := Weapon - 200;

    nAppr := FileIdx div 100;
    nWeapon := nWeapon - nAppr * 100;
    nAppr := nAppr + 1;
    // end;

    FileName := g_sSelfFilePath + Format(WEAPONIMAGESFILEX, [nAppr + 1]);
    if nAppr > High(ImagesArr) then
      SetLength(ImagesArr, nAppr + 1);

    if ImagesArr[nAppr] = nil then begin
      ImagesArr[nAppr] := CreateGameImages(FileName);
      ImagesArr[nAppr].Initialize;
    end;

    if ImagesArr[nAppr] <> nil then
      Result := ImagesArr[nAppr].GetCachedBrightImage(HUMANFRAME * nWeapon + Frame, Ax, Ay);
  end else begin
    FileIdx := FileIdx div 2;

    if Length(g_ResourcesDir) > 0 then
      FileName := g_sSelfResourcePath + Format(WeaponImageDir, [FileIdx])
    else
      FileName := g_sSelfFilePath + Format(WeaponImageDir, [FileIdx]);

    if FileIdx > High(ImagesArr) then
      SetLength(ImagesArr, FileIdx + 1);

    if ImagesArr[FileIdx] = nil then begin
      ImagesArr[FileIdx] := CreateGameImages(FileName);
      ImagesArr[FileIdx].Initialize;
    end;

    if ImagesArr[FileIdx] <> nil then
      Result := ImagesArr[FileIdx].GetCachedBrightImage(HUMANFRAME * Sex + Frame, Ax, Ay);
  end;
end;

function TWeaponImageList.GetWWeaponImg(Weapon, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
var
  FileName:string;
  FileIdx:Integer;
  nAppr:Integer;
  nShape:Integer;
  nWeapon:Integer;
begin
  Result := nil;
  FileIdx := Weapon - Sex;

  // 198 div 2 = 99  [Weapon 在服务端已经乘以2了]
  // 1 .. 99
  if (FileIdx <= 198) then begin
    if Length(ImagesArr) < 1 then
      SetLength(ImagesArr, 1);
    if ImagesArr[0] = nil then begin
      FileName := g_sSelfFilePath + WEAPONIMAGESFILE;
      ImagesArr[0] := CreateGameImages(FileName);
      ImagesArr[0].Initialize;
    end;
    if ImagesArr[0] <> nil then
      Result := ImagesArr[0].GetCachedImage(HUMANFRAME * Weapon + Frame, Ax, Ay);
    Exit;
  end;

  nShape := FileIdx div 2;
  if (nShape < 1000) then begin
    FileIdx := FileIdx - 200;
    nWeapon := Weapon - 200;

    nAppr := FileIdx div 100;
    nWeapon := nWeapon - nAppr * 100;
    nAppr := nAppr + 1;
    // end;

    FileName := g_sSelfFilePath + Format(WEAPONIMAGESFILEX, [nAppr + 1]);
    if nAppr > High(ImagesArr) then
      SetLength(ImagesArr, nAppr + 1);

    if ImagesArr[nAppr] = nil then begin
      ImagesArr[nAppr] := CreateGameImages(FileName);
      ImagesArr[nAppr].Initialize;
    end;

    if ImagesArr[nAppr] <> nil then
      Result := ImagesArr[nAppr].GetCachedImage(HUMANFRAME * nWeapon + Frame, Ax, Ay);
  end
  else begin
    // 大于等于1000 则 1000.wil---1001.wil
    FileIdx := FileIdx div 2;
    if Length(g_ResourcesDir) > 0 then
      FileName := g_sSelfResourcePath + Format(WeaponImageDir, [FileIdx])
    else
      FileName := g_sSelfFilePath + Format(WeaponImageDir, [FileIdx]);

    if FileIdx > High(ImagesArr) then
      SetLength(ImagesArr, FileIdx + 1);

    if ImagesArr[FileIdx] = nil then begin
      ImagesArr[FileIdx] := CreateGameImages(FileName);
      ImagesArr[FileIdx].Initialize;
    end;

    if ImagesArr[FileIdx] <> nil then
      Result := ImagesArr[FileIdx].GetCachedImage(HUMANFRAME * Sex + Frame, Ax, Ay);
  end;
end;

procedure TWeaponImageList.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if nUnit <= 1000 then begin
      if nUnit = 0 then
        sFileName := g_sSelfFilePath + WEAPONIMAGESFILE
      else
        sFileName := g_sSelfFilePath + Format(WEAPONIMAGESFILEX, [nUnit + 1]);
    end
    else begin
      if Length(g_ResourcesDir) > 0 then
        sFileName := g_sSelfResourcePath + Format(WeaponImageDir, [nUnit])
      else
        sFileName := g_sSelfFilePath + Format(WeaponImageDir, [nUnit]);
    end;

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;

  for nUnit := Low(WisImagesArr) to High(WisImagesArr) do begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + WEAPONIMAGESFILWISEX
    else
      sFileName := g_sSelfFilePath + Format(WEAPONIMAGESFILWISEX, [nUnit + 1]);
    WisImagesArr[nUnit] := CreateGameImages(sFileName);
    WisImagesArr[nUnit].Initialize;
  end;
end;

constructor TWeaponImageList.Create();
begin
  ImagesArr := nil;
  inherited Create;
end;

destructor TWeaponImageList.Destroy;
var
  I:Integer;
begin
  for I := Low(WisImagesArr) to High(WisImagesArr) do begin
    if WisImagesArr[I] <> nil then begin
      WisImagesArr[I].Finalize;
      FreeAndNil(WisImagesArr[I]);
    end;
  end;
  SetLength(WisImagesArr, 0);
  WisImagesArr := nil;

  inherited Destroy;
end;

procedure TWeaponImageList.ClearCache;
var
  I:Integer;
begin
  inherited;
  for I := 0 to Length(WisImagesArr) - 1 do begin
    if (WisImagesArr[I] <> nil) and WisImagesArr[I].Initialized then
      WisImagesArr[I].ClearCache;
  end;
end;

procedure TWeaponImageList.FreeOldMemorys;
var
  I:Integer;
begin
  inherited;
  for I := 0 to Length(WisImagesArr) - 1 do begin
    if (WisImagesArr[I] <> nil) and WisImagesArr[I].Initialized then
      WisImagesArr[I].FreeOldMemorys;
  end;
end;

procedure TWeaponImageList.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  for I := Low(WisImagesArr) to High(WisImagesArr) do begin
    if WisImagesArr[I] <> nil then begin
      WisImagesArr[I].Finalize;
      FreeAndNil(WisImagesArr[I]);
    end;
  end;
  SetLength(WisImagesArr, 0);
  SetLength(ImagesArr, 0);
end;

{------------------------------TStateItemImages--------------------------------}

constructor TStateItemImages.Create();
begin
  ImagesArr := nil;
end;

destructor TStateItemImages.Destroy;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
  ImagesArr := nil;
end;

function TStateItemImages.GetCount:Integer;
begin
  Result := Length(ImagesArr);
end;

procedure TStateItemImages.ClearCache;
var
  I:Integer;
begin
  for I := 0 to Length(ImagesArr) - 1 do begin
    if (ImagesArr[I] <> nil) and ImagesArr[I].Initialized then
      ImagesArr[I].ClearCache;
  end;
end;

function TStateItemImages.ImageOf(Index:Integer):TTexture;
var
  sFileName:string;
  nUnit, nIndex:Integer;
begin
  nUnit := Index div 10000;

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + STATEITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(STATEITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;

  { TODO -c修改 -opiaoyun : StateItem1.wil扩展 数据库Looks起始编号10000开始【 2013-4-23】}
  if Index >= 10000 then
    nIndex := Index mod 10000
  else
    nIndex := Index;

  // DScreen.AddChatBoardString(Format('nUnit:%d nIndex:%d',[nUnit, nIndex]), clGreen, clWhite);
  Result := ImagesArr[nUnit].Images[nIndex];
end;

function TStateItemImages.LooksOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := _MAX(Index div 10000, 0);

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + STATEITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(STATEITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

function TStateItemImages.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := _MAX(Index, 0);

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + STATEITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(STATEITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

procedure TStateItemImages.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + STATEITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(STATEITEMIMAGESFILEX, [nUnit]);
    // if FileExists(sFileName) then begin

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
    // end;
  end;
end;

procedure TStateItemImages.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
end;

procedure TStateItemImages.FreeOldMemorys;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].FreeOldMemorys;
    end;
  end;
end;

function TStateItemImages.GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
var
  sFileName:string;
  nUnit, nIndex:Integer;
begin
  nUnit := Index div 10000;

  if nUnit >= Length(ImagesArr) - 1 then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + STATEITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(STATEITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;

  if Index >= 10000 then
    nIndex := Index mod 10000
  else
    nIndex := Index;

  Result := ImagesArr[nUnit].GetCachedImage(nIndex, PX, PY);

end;

{------------------------------TDnItemImages--------------------------------}

constructor TDnItemImages.Create();
begin
  ImagesArr := nil;
end;

destructor TDnItemImages.Destroy;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
  ImagesArr := nil;
  inherited;
end;

function TDnItemImages.GetCount:Integer;
begin
  Result := Length(ImagesArr);
end;

procedure TDnItemImages.ClearCache;
var
  I:Integer;
begin
  for I := 0 to Length(ImagesArr) - 1 do begin
    if (ImagesArr[I] <> nil) and ImagesArr[I].Initialized then
      ImagesArr[I].ClearCache;
  end;
end;

function TDnItemImages.GetCachedGray(Index:Integer):TTexture;
var
  sFileName:string;
  nUnit, nIndex:Integer;
begin
  nUnit := Index div 10000;

  if nUnit >= Length(ImagesArr) - 1 then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + DNITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(DNITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;

  if Index >= 10000 then
    nIndex := Index mod 10000
  else
    nIndex := Index;
  Result := ImagesArr[nUnit].Grays[nIndex];
end;

function TDnItemImages.GetCachedBright(Index:Integer):TTexture;
var
  sFileName:string;
  nUnit, nIndex:Integer;
begin
  nUnit := Index div 10000;

  if nUnit >= Length(ImagesArr) - 1 then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + DNITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(DNITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;

  if Index >= 10000 then
    nIndex := Index mod 10000
  else
    nIndex := Index;
  Result := ImagesArr[nUnit].Brights[nIndex];
end;

function TDnItemImages.ImageOf(Index:Integer):TTexture;
var
  sFileName:string;
  nUnit, nIndex:Integer;
begin
  nUnit := Index div 10000;

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + DNITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(DNITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;

  if Index >= 10000 then
    nIndex := Index mod 10000
  else
    nIndex := Index;

  Result := ImagesArr[nUnit].Images[nIndex];
end;

function TDnItemImages.LooksOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := _MAX(Index div 10000, 0);

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + DNITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(DNITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

function TDnItemImages.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := _MAX(Index, 0);

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + DNITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(DNITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

procedure TDnItemImages.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + DNITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(DNITEMIMAGESFILEX, [nUnit]);
    // if FileExists(sFileName) then begin

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
    // end;
  end;
end;

procedure TDnItemImages.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
end;

procedure TDnItemImages.FreeOldMemorys;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].FreeOldMemorys;
    end;
  end;
end;

function TDnItemImages.GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
var
  sFileName:string;
  nUnit, nIndex:Integer;
begin
  nUnit := Index div 10000;

  if nUnit >= Length(ImagesArr) - 1 then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + DNITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(DNITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;

  if Index >= 10000 then
    nIndex := Index mod 10000
  else
    nIndex := Index;

  Result := ImagesArr[nUnit].GetCachedImage(nIndex, PX, PY);

end;

{------------------------------TBagItemImages--------------------------------}

constructor TBagItemImages.Create();
begin
  ImagesArr := nil;
end;

destructor TBagItemImages.Destroy;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
  ImagesArr := nil;
  inherited;
end;

procedure TBagItemImages.ClearCache;
var
  I:Integer;
begin
  for I := 0 to Length(ImagesArr) - 1 do begin
    if (ImagesArr[I] <> nil) and ImagesArr[I].Initialized then
      ImagesArr[I].ClearCache;
  end;
end;

function TBagItemImages.GetCount:Integer;
begin
  Result := Length(ImagesArr);
end;

function TBagItemImages.ImageOf(Index:Integer):TTexture;
var
  sFileName:string;
  nUnit, nIndex:Integer;
begin
  nUnit := Index div 10000;

  if nUnit >= Length(ImagesArr) - 1 then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + BAGITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(BAGITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;

  if Index >= 10000 then
    nIndex := Index mod 10000
  else
    nIndex := Index;
  Result := ImagesArr[nUnit].Images[nIndex];
end;

function TBagItemImages.LooksOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := _MAX(Index div 10000, 0);

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + BAGITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(BAGITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

function TBagItemImages.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := _MAX(Index, 0);

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + BAGITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(BAGITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

procedure TBagItemImages.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + BAGITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(BAGITEMIMAGESFILEX, [nUnit]);
    // if FileExists(sFileName) then begin

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
    // end;
  end;
end;

procedure TBagItemImages.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
end;

procedure TBagItemImages.FreeOldMemorys;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].FreeOldMemorys;
    end;
  end;
end;

function TBagItemImages.GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
var
  sFileName:string;
  nUnit, nIndex:Integer;
begin
  nUnit := Index div 10000;

  if nUnit >= Length(ImagesArr) - 1 then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + BAGITEMIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(BAGITEMIMAGESFILEX, [nUnit]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;

  if Index >= 10000 then
    nIndex := Index mod 10000
  else
    nIndex := Index;

  Result := ImagesArr[nUnit].GetCachedImage(nIndex, PX, PY);

end;

// =============== TNpcImageList================

function TNpcImageList.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := Index;
  if nUnit < 0 then
    nUnit := 0;

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + NPCIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(NPCIMAGESFILEX, [nUnit + 1]);
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
    // DebugOutStr('TNpcImageList.IndexOf ' + ImagesArr[nUnit].FileName);
  end;
  Result := ImagesArr[nUnit];
end;

procedure TNpcImageList.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + NPCIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(NPCIMAGESFILEX, [nUnit + 1]);
    // if FileExists(sFileName) then begin
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
    // end;
  end;
end;

procedure TNpcImageList.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
end;

function WzlFileExists(FileName:string):Boolean;
var
  FileStream:TFileStream;
begin
  Result := False;
  if FileExists(FileName) then begin
    FileStream := TFileStream.Create(FileName, fmOpenRead or fmShareDenyNone);
    Result := FileStream.Size > SizeOf(TWzlImageHeader);
    FileStream.Free;
  end;
end;

function GetDataFiles(FileName:string):string;
var
  I, nPos:Integer;
  sResources_Data:string;
begin
  Result := FileName;
  nPos := FastPosNoCase(FileName, '\Data\', Length(FileName), Length('\Data\'), 1);
  if nPos > 0 then begin
    sResources_Data := g_sSelfResourcePath + Copy(FileName, Length(g_sSelfFilePath) + 1, Length(FileName) - Length(g_sSelfFilePath));
    //for I := 0 to g_MapFileList.Count - 1 do begin
    for I := 0 to g_DataFileList.Count - 1 do begin //HZQ 20230616 更改笔误
      if CompareText(sResources_Data, g_DataFileList.Strings[I]) = 0 then begin
        Result := sResources_Data;
        break;
      end;
    end;
  end;
end;

function GetMapFiles(FileName:string):string;
var
  I, nPos:Integer;
  sResources_Map:string;
begin
  Result := FileName;
  nPos := FastPosNoCase(FileName, '\Map\', Length(FileName), Length('\Map\'), 1);
  if nPos > 0 then begin
    sResources_Map := g_sSelfResourcePath + Copy(FileName, Length(g_sSelfFilePath) + 1, Length(FileName) - Length(g_sSelfFilePath));
    for I := 0 to g_MapFileList.Count - 1 do begin
      if CompareText(sResources_Map, g_MapFileList.Strings[I]) = 0 then begin
        Result := sResources_Map;
        break;
      end;
    end;
  end;
end;

function GetWavFiles(FileName:string):string;
var
  I, nPos:Integer;
  sResources_Wav:string;
begin
  Result := FileName;
  nPos := FastPosNoCase(FileName, '\Wav\', Length(FileName), Length('\Wav\'), 1);
  if nPos > 0 then begin
    sResources_Wav := g_sSelfResourcePath + Copy(FileName, Length(g_sSelfFilePath) + 1, Length(FileName) - Length(g_sSelfFilePath));
    for I := 0 to g_WavFileList.Count - 1 do begin
      if CompareText(sResources_Wav, g_WavFileList.Strings[I]) = 0 then begin
        Result := g_WavFileList.Strings[I];
        break;
      end;
    end;
  end;
end;

function GetAbsolutePathEx(BasePath, RelativePath:string):string;
var
  Dest:array[0..MAX_PATH] of char;
begin
  FillChar(Dest, MAX_PATH + 1, 0);
  PathCombine(Dest, PChar(BasePath), PChar(RelativePath));
  Result := string(Dest);
end;

{
function GetHMapDataFiles(FileName: string; var sGetFileName: string): Boolean;
var
  I, J: Integer;
  sFileNameOnly, S: string;
  ExtFileName: array[0..1] of string;
begin
  Result := False;
  if g_ConfigClient.nLoadResourcesOrder in [1, 2] then
  begin
    ExtFileName[0] := '.wil';
    ExtFileName[1] := '.wzl';
  end
  else
  begin
    ExtFileName[0] := '.wzl';
    ExtFileName[1] := '.wil';
  end;

  sFileNameOnly := ExtractFileNameOnly(FileName);
  for I := 0 to Length(ExtFileName) - 1 do
  begin
    S := g_sSelfFilePath + g_ResourcesDir + '\HMapData\' + sFileNameOnly + ExtFileName[I];
    for J := 0 to g_HMapDataFileList.Count - 1 do
    begin
      if SameText(S, g_HMapDataFileList.Strings[J]) then
      begin
        Result := True;
        sGetFileName := S;
        Exit;
      end;
    end;
  end;
end;
}

function GetGameImageFiles(FileName:string; var sPassword:string; CheckExists:Boolean = True):string;
var
  I, nPos:Integer;
  sLineText, sFileName, Password:string;
  sResources_Graphics:string;
  sResources_Data:string;
begin
  Result := '';
  sResources_Graphics := '';
  sResources_Data := '';
  sPassword := '';

  nPos := FastPosNoCase(FileName, '\Graphics\', Length(FileName), Length('\Graphics\'), 1);
  if nPos > 0 then begin
    sResources_Graphics := g_sSelfFilePath + {'Resources\' + } Copy(FileName, Length(g_sSelfFilePath) + 1, Length(FileName) - Length(g_sSelfFilePath));
    sResources_Graphics := ExtractFilePath(sResources_Graphics) + ExtractFileNameOnly(sResources_Graphics) + '.pak';
  end else begin
    nPos := FastPosNoCase(FileName, '\Data\', Length(FileName), Length('\Data\'), 1);
    if nPos > 0 then begin
      sResources_Data := g_sSelfResourcePath + Copy(FileName, Length(g_sSelfFilePath) + 1, Length(FileName) - Length(g_sSelfFilePath));
      sResources_Data := ExtractFilePath(sResources_Data) + ExtractFileNameOnly(sResources_Data) + '.pak';
    end;
  end;

  // 当使用微端时，规则列表中有pak的只读pak，不管客户端有没有这个文件chongchong 2015-08-02
  // 服务器列表中的微端网关端口号如果不为0，表示使用微端（是否连得上不管） chongchong 2015-11-23
  if g_nUpdateGatePort > 0 then begin
    for I := 0 to g_ImageFileList.Count - 1 do begin
      sLineText := g_ImageFileList.Strings[I];

      sFileName := sLineText;
      Password := '';
      nPos := Pos('|', sLineText);
      if nPos > 0 then begin
        Password := Copy(sLineText, nPos + 1, Length(sFileName) - nPos);
        sFileName := Copy(sLineText, 1, nPos - 1);
      end;

      if sFileName <> '' then begin
        if (sResources_Data <> '') and (SameText(sFileName, sResources_Data)) then begin
          sPassword := Password;
          Result := sResources_Data;
          break;
        end;

        if (sResources_Graphics <> '') and (CompareText(sFileName, sResources_Graphics) = 0) then begin
          sPassword := Password;
          Result := sResources_Graphics;
          break;
        end;
      end;
    end;

    if Length(Result) > 0 then
      Exit;
  end;

  for I := 0 to g_ImageFileList.Count - 1 do begin
    sLineText := g_ImageFileList.Strings[I];

    sFileName := sLineText;
    Password := '';
    nPos := Pos('|', sLineText);
    if nPos > 0 then begin
      Password := Copy(sLineText, nPos + 1, Length(sFileName) - nPos);
      sFileName := Copy(sLineText, 1, nPos - 1);
    end;

    if sFileName <> '' then begin
      // ★★★◆◆◆生成器配置错误读不到文件(如:MakeGameLogin.exe的配置文件wil.txt加入\Data\Prguse2.wil) +and FileExists(FileName) 2014-04-15
      if SameText(sFileName, FileName) then begin
        if CheckExists then begin
          if FileExists(sFileName) then begin
            sPassword := Password;
            Result := FileName;
            break;
          end;
        end
        else begin
          sPassword := Password;
          Result := FileName;
          break;
        end;
      end;

      if (sResources_Data <> '') and (CompareText(sFileName, sResources_Data) = 0) and FileExists(sFileName) then begin
        sPassword := Password;
        Result := sResources_Data;
        break;
      end;
      if (sResources_Graphics <> '') and (CompareText(sFileName, sResources_Graphics) = 0) and FileExists(sFileName) then begin
        sPassword := Password;
        Result := sResources_Graphics;
        break;
      end;
    end;
  end;
end;

function GetPakFile(FileName:string; var sPassword:string):string;
var
  I, nPos:Integer;
  sLineText, sFileName, Password:string;
begin
  Result := '';
  sPassword := '';

  FileName := g_sSelfResourcePath + 'Data\' + ExtractFileNameOnly(FileName) + '.pak';
  ;
  for I := 0 to g_ImageFileList.Count - 1 do begin
    sLineText := g_ImageFileList.Strings[I];

    sFileName := sLineText;
    Password := '';
    nPos := Pos('|', sLineText);

    if nPos > 0 then begin
      Password := Copy(sLineText, nPos + 1, Length(sFileName) - nPos);
      sFileName := Copy(sLineText, 1, nPos - 1);

      if SameText(FileName, sFileName) then begin
        sPassword := Password;
        Result := sFileName;
        Exit;
      end;
    end;
  end;

  FileName := ExtractFileNameOnly(FileName) + '.pak';
  ;
  for I := 0 to g_ImageFileList.Count - 1 do begin
    sLineText := g_ImageFileList.Strings[I];

    sFileName := sLineText;
    Password := '';
    nPos := Pos('|', sLineText);

    if nPos > 0 then begin
      Password := Copy(sLineText, nPos + 1, Length(sFileName) - nPos);
      sFileName := Copy(sLineText, 1, nPos - 1);

      if SameText(FileName, ExtractFileName(sFileName)) then begin
        sPassword := Password;
        Result := sFileName;
        Exit;
      end;
    end;
  end;
end;

function GetGameImagesFileName(FileName:string):string;
var
  nPos:Integer;
  sFileExt:string;
  sFileName:string;
  sFilePath:string;
  sPassword:string;
begin
  Result := FileName;
  sFileName := GetGameImageFiles(FileName, sPassword);
  if sFileName = '' then begin
    nPos := FastPosNoCase(FileName, '\Graphics\', Length(FileName), Length('\Graphics\'), 1);
    if nPos > 0 then begin
      sFileName := g_sSelfResourcePath + Copy(FileName, Length(g_sSelfFilePath) + 1, Length(FileName) - Length(g_sSelfFilePath));
      sFileName := ExtractFilePath(sFileName) + ExtractFileNameOnly(sFileName) + '.pak';
      if FileExists(sFileName) then begin
        FileName := sFileName;
        sFileExt := '.PAK'; // UpperCase(ExtractFileExt(sFileName));
      end
      else begin
        sFileExt := UpperCase(ExtractFileExt(ExtractFileName(FileName)));
      end;
    end
    else begin
      sFileExt := UpperCase(ExtractFileExt(ExtractFileName(FileName)));
      sFilePath := ExtractFilePath(FileName);
      try
        nPos := FastPosNoCase(sFilePath, '\Data\', Length(sFilePath), Length('\Data\'), 1);
      except
        nPos := Length(sFilePath) - 5;
      end;
      if nPos > 0 then
        sFilePath := Copy(sFilePath, 1, nPos);

      if (sFilePath <> '') and (sFilePath[Length(sFilePath)] <> '\') then
        sFilePath := sFilePath + '\';

      sFileName := sFilePath + g_ResourcesDir + '\' + {'Resources\'} 'Data\' + ExtractFileNameOnly(FileName) + '.pak';
      if FileExists(sFileName) then begin
        FileName := sFileName;
        sFileExt := '.PAK'; // UpperCase(ExtractFileExt(sFileName));
      end;
    end;

    // 资源读取方式---根据登陆器配置进行相应处理 piaoyun 2013-11-19
    if (sFileExt <> '.PAK') then begin
      case g_ConfigClient.nLoadResourcesOrder of
        0: {//PAK--WZL--WIL} begin
            sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wzl';
            if not FileExists(sFileName) {不存在wzl} then begin
              sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wil';
              if not FileExists(sFileName) then begin
                FileName := sFileName;
              end
              else begin
                FileName := sFileName;
                sFileExt := '.WIL';
              end;
            end
            else begin
              FileName := sFileName;
              sFileExt := '.WZL';
            end;
          end;

        1: {//PAK--WIL--WZL} begin
            sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wil';
            if not FileExists(sFileName) {不存在wil} then begin
              sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wzl';
              if not FileExists(sFileName) then begin
                FileName := sFileName;
              end
              else begin
                FileName := sFileName;
                sFileExt := '.WZL';
              end;
            end
            else begin
              FileName := sFileName;
              sFileExt := '.WIL';
            end;
          end;

        2: {//PAK--WIL} begin
            sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wil';
            if not FileExists(sFileName) {不存在wil} then begin
              FileName := '';
              //sFileExt := '';
            end
            else begin
              FileName := sFileName;
              sFileExt := '.WIL';
            end;
          end;

        3: {//PAK--WZL} begin
            sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wzl';
            if not FileExists(sFileName) {不存在wzl} then begin
              FileName := '';
              //sFileExt := '';
            end
            else begin
              FileName := sFileName;
              sFileExt := '.WZL';
            end;
          end;

        4: {//PAK} begin
            FileName := '';
          end;
      end;
    end;

    (*
    // 再次修改读取规则 PAK--WZL--WIL piaoyun 2013-6-27
    // 先判断WZL
    if (sFileExt <> '.PAK') { and (sFileExt <> '.WIS') } then
    begin
      if not FileExists(FileName) then                                                              // 排除 WIS
      begin
        sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wzl';
        if not FileExists(sFileName) {不存在wzl} then
        begin
          sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wil';
          if not FileExists(sFileName) then
          begin
            FileName := sFileName;
          end
          else
          begin
            FileName := sFileName;
            sFileExt := '.WIL';
          end;
        end
        else
        begin
          FileName := sFileName;
          sFileExt := '.WZL';
        end;
      end;
    end
    *)
  end
  else
    FileName := sFileName;
  Result := FileName;
end;

function CreateGameImages(FileName:string):TGameImages;
var
  nPos:Integer;
  sFileExt:string;
  sFileName:string;
  sFilePath:string;
  sPassword:string;
  APassWord:TPakPassword;
  S1, S2:string;
begin
  // 如果在读取规则中同时配置了D:\热血传奇\Data\ChrSel.wil， D:\热血传奇\Data\ChrSel.pak，
  // 程序会读取 D:\热血传奇\Data\ChrSel.wil

  sFileName := GetGameImageFiles(ChangeFileExt(FileName, '.pak'), sPassword);
  if sFileName = '' then begin
    case g_ConfigClient.nLoadResourcesOrder of
      0: {//PAK--WZL--WIL} begin
          sFileName := GetGameImageFiles(ChangeFileExt(FileName, '.wzl'), sPassword);

          if sFileName = '' then
            sFileName := GetGameImageFiles(ChangeFileExt(FileName, '.wil'), sPassword);
        end;
      1: {//PAK--WIL--WZL} begin
          sFileName := GetGameImageFiles(ChangeFileExt(FileName, '.wil'), sPassword);

          if sFileName = '' then
            sFileName := GetGameImageFiles(ChangeFileExt(FileName, '.wzl'), sPassword);
        end;
      2: {//PAK--WIL} begin
          if sFileName = '' then
            sFileName := GetGameImageFiles(ChangeFileExt(FileName, '.wil'), sPassword);
        end;
      3: {//PAK--WZL} begin
          if sFileName = '' then
            sFileName := GetGameImageFiles(ChangeFileExt(FileName, '.wzl'), sPassword);
        end;
    end;

    if sFileName = '' then
      sFileName := GetGameImageFiles(FileName, sPassword);
  end;

  if sFileName = '' then begin
    nPos := FastPosNoCase(FileName, '\Graphics\', Length(FileName), Length('\Graphics\'), 1);
    if nPos > 0 then begin
      sFileName := g_sSelfFilePath + {'Resources\' +} Copy(FileName, Length(g_sSelfFilePath) + 1, Length(FileName) - Length(g_sSelfFilePath));
      sFileName := ExtractFilePath(sFileName) + ExtractFileNameOnly(sFileName) + '.pak';
      // DebugOutStr('sFileName:'+sFileName);
      if FileExists(sFileName) then begin
        FileName := sFileName;
        sFileExt := '.PAK'; // UpperCase(ExtractFileExt(sFileName));
      end
      else begin
        sFileExt := UpperCase(ExtractFileExt(ExtractFileName(FileName)));
      end;
    end
    else begin
      sFileExt := UpperCase(ExtractFileExt(ExtractFileName(FileName)));
      sFilePath := ExtractFilePath(FileName);

      try
        nPos := FastPosNoCase(sFilePath, '\Data\', Length(sFilePath), Length('\Data\'), 1);
      except
        nPos := Length(sFilePath) - 5;
      end;
      if nPos > 0 then
        sFilePath := Copy(sFilePath, 1, nPos);

      if (sFilePath <> '') and (sFilePath[Length(sFilePath)] <> '\') then
        sFilePath := sFilePath + '\';

      sFileName := sFilePath + g_ResourcesDir + '\' + {'Resources\} 'Data\' + ExtractFileNameOnly(FileName) + '.pak';
      if FileExists(sFileName) then begin
        FileName := sFileName;
        sFileExt := '.PAK'; // UpperCase(ExtractFileExt(sFileName));
      end;

      // 资源读取方式---根据登陆器配置进行相应处理 piaoyun 2013-11-19
      if (sFileExt <> '.PAK') then begin
        case g_ConfigClient.nLoadResourcesOrder of
          0: {//PAK--WZL--WIL} begin
              sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wzl';
              if not FileExists(sFileName) {不存在wzl} then begin
                sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wil';
                if not FileExists(sFileName) then begin
                  {
                  // 都不存在改回WZL，方便微端更新 piaoyun 2014-01-08
                  sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wzl';
                  FileName := sFileName;
                  sFileExt := '.WZL';
                  }
                  // 如果都不存在，看看pak有没有 chongchong 2014-09-15
                  S1 := GetPakFile(FileName, S2);
                  if Length(S1) > 0 then begin
                    FileName := S1;
                    sPassword := S2;
                    sFileExt := '.PAK';
                  end
                  else begin
                    // 都不存在改回WZL，方便微端更新 piaoyun 2014-01-08
                    sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wzl';
                    FileName := sFileName;
                    sFileExt := '.WZL';
                  end;
                end
                else begin
                  FileName := sFileName;
                  sFileExt := '.WIL';
                end;
              end
              else begin
                FileName := sFileName;
                sFileExt := '.WZL';
              end;
            end;

          1: {//PAK--WIL--WZL} begin
              sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wil';
              if not FileExists(sFileName) {不存在wil} then begin
                sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wzl';
                if not FileExists(sFileName) then begin
                  {
                  // 都不存在改回WIL，方便微端更新 piaoyun 2014-01-08
                  sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wil';
                  FileName := sFileName;
                  sFileExt := '.WIL'
                  }
                  // 如果都不存在，看看pak有没有 chongchong 2014-09-15
                  S1 := GetPakFile(FileName, S2);
                  if Length(S1) > 0 then begin
                    FileName := S1;
                    sPassword := S2;
                    sFileExt := '.PAK';
                  end
                  else begin
                    // 都不存在改回WZL，方便微端更新 piaoyun 2014-01-08
                    sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wil';
                    FileName := sFileName;
                    sFileExt := '.WIL';
                  end;
                end
                else begin
                  FileName := sFileName;
                  sFileExt := '.WZL';
                end;
              end
              else begin
                FileName := sFileName;
                sFileExt := '.WIL';
              end;
            end;

          2: {//PAK--WIL} begin
              sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wil';
              if not FileExists(sFileName) {不存在wil} then begin
                // 如果都不存在，看看pak有没有 chongchong 2014-09-15
                S1 := GetPakFile(FileName, S2);
                if Length(S1) > 0 then begin
                  FileName := S1;
                  sPassword := S2;
                  sFileExt := '.PAK';
                end
                else begin
                  FileName := sFileName;
                  sFileExt := '.WIL';
                end;
              end
              else begin
                FileName := sFileName;
                sFileExt := '.WIL';
              end;
            end;

          3: {//PAK--WZL} begin
              sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wzl';
              if not FileExists(sFileName) {不存在wzl} then begin
                // 如果都不存在，看看pak有没有 chongchong 2014-09-15
                S1 := GetPakFile(FileName, S2);
                if Length(S1) > 0 then begin
                  FileName := S1;
                  sPassword := S2;
                  sFileExt := '.PAK';
                end
                else begin
                  FileName := sFileName;
                  sFileExt := '.WZL';
                end;
              end
              else begin
                FileName := sFileName;
                sFileExt := '.WZL';
              end;
            end;

          4: {//PAK} begin
              FileName := '';
            end;
        end;
      end;

      (*
      // 再次修改读取规则 PAK--WZL--WIL piaoyun 2013-6-27
      if (sFileExt <> '.PAK') { and (sFileExt <> '.WIS')} then                                      // 剩下 wzl wil
      begin
        if not FileExists(FileName) then                                                            // 排除 WIS
        begin
          sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wzl';
          if not FileExists(sFileName) {不存在wzl} then
          begin
            sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wil';
            if not FileExists(sFileName) then
            begin
              FileName := sFileName;
            end
            else
            begin
              FileName := sFileName;
              sFileExt := '.WIL';
            end;
          end
          else
          begin
            FileName := sFileName;
            sFileExt := '.WZL';
          end;
        end;
      end;
      *)
    end;
  end else begin
    FileName := sFileName;
    sFileExt := UpperCase(ExtractFileExt(ExtractFileName(sFileName)));
  end;
  // DebugOutStr('FileName ' + FileName);
  // DebugOutStr('GetGameImages:'+FileName);
  // 如果这个文件根本就不存在，看下列表中到底有没有pak吧
  if not FileExists(FileName) then begin
    // 检查的时候不判断文件是否存在
    sFileName := GetGameImageFiles(ChangeFileExt(FileName, '.pak'), sPassword, False);
    if sFileName <> '' then begin
      FileName := sFileName;
      sFileExt := UpperCase(ExtractFileExt(ExtractFileName(sFileName)));
    end
    else begin
      sFileName := sFilePath + g_ResourcesDir + '\' + {'Resources\} 'Data\' + ExtractFileNameOnly(FileName) + '.pak';
      sFileName := GetGameImageFiles(sFileName, sPassword, False);
      if sFileName <> '' then begin
        FileName := sFileName;
        sFileExt := UpperCase(ExtractFileExt(ExtractFileName(sFileName)));
      end
      else begin
        sFileName := GetGameImageFiles(ChangeFileExt(FileName, '.wzl'), sPassword, False);
        if sFileName <> '' then begin
          FileName := sFileName;
          sFileExt := UpperCase(ExtractFileExt(ExtractFileName(sFileName)));
        end
        else begin
          sFileName := GetGameImageFiles(ChangeFileExt(FileName, '.wil'), sPassword, False);
          if sFileName <> '' then begin
            FileName := sFileName;
            sFileExt := UpperCase(ExtractFileExt(ExtractFileName(sFileName)));
          end;
        end;
      end;
    end;
  end;

  if sFileExt = '.WZL' then begin
    Result := TWzlImages.Create();
    Result.FileName := FileName;
  end
  else if sFileExt = '.WIL' then begin
    Result := TWMImages.Create();
    Result.FileName := FileName;
  end
  else if sFileExt = '.WIS' then begin
    Result := TWisImages.Create();
    Result.FileName := FileName;
  end
  else if sFileExt = '.PAK' then begin
    {$I VMProtectBegin.inc}
    if sPassword = '' then begin
      sPassword := 'GEEM2'; //HZQ 20230511
      //    {$IF CustomGUITest = 1}
      //      if FileName = 'D:\热血传奇\Data\NewUI1.PAK' then
      //        sPassword := '123456789GXXM2';
      //    {$IFEND}
            // pak专用密码 piaoyun 2013-08-27
      GetKeyDataPAK(sPassword, @APassWord.Chain, @APassWord.KeyData);
      GetKeyData(sPassword, @APassWord.ChainLz, @APassWord.KeyDataLz);
    end else begin
      DecodeString(sPassword, @APassWord, SizeOf(TPakPassword));
      DecryptDes(APassWord, APassWord, SizeOf(APassWord), #10#11#2#9#2#1#12#32#1#9#5#230#211#190);
    end;

    // 专用定制版本统一pak密码
    {$MESSAGE HINT '需要修改统一PAK密码，但是现在不能处理'}
    {$IF TESTMODE = 2}
    sFileName := ExtractFileName(FileName);
    if SameText(sFileName, g_ConfigClient.sGamePlanName) or (Length(g_PakDefaultPassword) = 0) then begin
      sPassword := 'GEEM2';
      GetKeyDataPAK(sPassword, @APassWord.Chain, @APassWord.KeyData);
      GetKeyData(sPassword, @APassWord.ChainLz, @APassWord.KeyDataLz);
    end else begin
      DecodeBuffer(g_PakDefaultPassword, @APassWord, SizeOf(TPakPassword));
    end;
    {$IFEND}
    {$I VMProtectEnd.inc}

    Result := TPakImages.Create(APassWord); //增加了密码的传入
    Result.FileName := FileName;
  end else begin
    Result := TWzlImages.Create();
    Result.FileName := FileName;
  end;
end;

function CreateHMapDataGameImages(FileName:string):TGameImages;
{
var
  sFileExt: string;
  sFileName: string;
}
begin
  {
  // 如果在读取规则中同时配置了D:\热血传奇\Data\ChrSel.wil， D:\热血传奇\Data\ChrSel.pak，
  // 程序会读取 D:\热血传奇\Data\ChrSel.wil
  //if not GetHMapDataFiles(FileName, sFileName) then
  //  sFileName := FileName;

  sFileExt := UpperCase(ExtractFileExt(ExtractFileName(sFileName)));

  if sFileExt = '.WZL' then
  begin
    Result := TWzlImages.Create();
    Result.FileName := sFileName;
  end
  else if sFileExt = '.WIL' then
  begin
    Result := TWMImages.Create();
    Result.FileName := sFileName;
  end
  else
  begin
    Result := TWzlImages.Create();
    Result.FileName := sFileName;
  end;
  }

  Result := TWzlImages.Create();
  Result.FileName := FileName;

end;
// ------------------------------------------------------------------------------

constructor TImageEvent.Create();
begin
  m_EventList := TStringList.Create;
  m_DynamicEventList := TStringList.Create;
end;

destructor TImageEvent.Destroy;
begin
  UnLoadGameImages;
  m_EventList.Free;
  m_DynamicEventList.Free;
end;

procedure TImageEvent.UnLoadGameImages;
var
  I:Integer;
begin
  for I := Low(g_WObjectArr) to High(g_WObjectArr) do begin
    if g_WObjectArr[I] <> nil then begin
      g_WObjectArr[I].Finalize;
      FreeAndNil(g_WObjectArr[I]);
    end;
  end;

  { TODO -ochongchong -c内存泄露 : 去内存泄露 【2013-7-12】 }
  g_WTilesImages.Free;
  g_WSmTilesImages.Free;
  g_WBagItemImages.Free;
  g_WDnItemImages.Free;
  // ---------------------------------------end

  g_WMonImages.Free;

  g_WStateItemImages.Free;
  g_WHumImgImages.Free;
  // 翅膀效果扩展 piaoyun 2013-07-28
  g_WHumEffectImages.Free;
  g_WWeaponImages.Free;
  g_WNpcImgImages.Free;
  // 连击武器外观 piaoyun 2013-07-29
  g_WCboWeaponList.Free;
  // 连击人物特效 piaoyun 2013-07-29
  g_WCboHumEffect.Free;
  // 连击人物外观扩展 piaoyun 2013-07-29
  g_WCboHum.Free;
  // 新武器特效WeaponEffect-WeaponEffect5.wzl piaoyun 2013-07-29
  g_WeaponEffectList.Free;
  // 新连击武器特效cboWeaponEffect-cboWeaponEffect5.wzl piaoyun 2013-07-30
  g_CboWeaponEffectList.Free;

  for I := 0 to m_EventList.Count - 1 do
    m_EventList.Objects[I].Free;

  m_EventList.Clear;

  for I := 0 to m_DynamicEventList.Count - 1 do
    m_DynamicEventList.Objects[I].Free;

  m_DynamicEventList.Clear;

  for I := Low(g_NewopUI170TextureArray) to High(g_NewopUI170TextureArray) do begin
    if g_NewopUI170TextureArray[I] <> nil then begin
      g_NewopUI170TextureArray[I].Free;
      g_NewopUI170TextureArray[I] := nil;
    end;
  end;
end;

procedure ResetNewopUI170TextureArray(Index:Integer);
var
  I:Integer;
  D:TTexture;
begin
  if Index < 0 then begin
    for I := Low(g_NewopUI170TextureArray) to High(g_NewopUI170TextureArray) do begin
      if g_NewopUI170TextureArray[I] <> nil then begin
        g_NewopUI170TextureArray[I].Free;
        g_NewopUI170TextureArray[I] := nil;
      end;

      if g_WNewopUIImages <> nil then begin
        D := g_WNewopUIImages.Images[170 + I];
        if D <> nil then begin
          g_NewopUI170TextureArray[I] := GameCanvas.HGE.Texture_Create(D.Width, D.Height);
          CopyTexture(D, g_NewopUI170TextureArray[I], 0, 0);
        end;
      end;
    end;
  end else if (Index >= 170) and (Index <= 179) then begin
    if g_NewopUI170TextureArray[Index - 170] <> nil then begin
      g_NewopUI170TextureArray[Index - 170].Free;
      g_NewopUI170TextureArray[Index - 170] := nil;
    end;

    if g_WNewopUIImages <> nil then begin
      D := g_WNewopUIImages.Images[Index];
      if D <> nil then begin
        //g_NewopUI170TextureArray[I - 170] := GameCanvas.HGE.Texture_Create(D.Width, D.Height);
        //CopyTexture(D, g_NewopUI170TextureArray[I - 170], 0, 0);
        //HZQ 20230524这个地方可能是代码拷贝后未处理，此时I是未赋值的，根据程序意思修改
        g_NewopUI170TextureArray[Index - 170] := GameCanvas.HGE.Texture_Create(D.Width, D.Height);
        CopyTexture(D, g_NewopUI170TextureArray[Index - 170], 0, 0);
      end;
    end;
  end;
end;

procedure TImageEvent.LoadGameImages;
var
  i:Integer;
  APassWord:TPakPassword;
begin
  // ------------------------------------------------------------------------------
  // g_WLightImages := CreateGameImages(sFilePath + LIGHTIMAGEFILE);
  // Add(g_WLightImages);

  //HZQ NEWOPUIIMAGEFILE 和 MOBILEIMAGEFILE 在ClMain.Loadconfig中被重新赋值
  g_WNewopUIImages := CreateGameImages(g_sSelfFilePath + NEWOPUIIMAGEFILE);
  g_WNewopUIImages.m_boNeedUpdate := False;
  Add(g_WNewopUIImages);

  {.$IFDEF BEIJING}
  g_WMobileImages := CreateGameImages(g_sSelfFilePath + MOBILEIMAGEFILE);
  g_WMobileImages.m_boNeedUpdate := False;
  Add(g_WMobileImages);
  {.$ENDIF}

  g_WUIImages := CreateGameImages(g_sSelfFilePath + UIIMAGEFILE);
  Add(g_WUIImages);

  g_WUI1Images := CreateGameImages(g_sSelfFilePath + UIIMAGEFILE1);
  Add(g_WUI1Images);

  g_WUI2Images := CreateGameImages(g_sSelfFilePath + UIIMAGEFILE2);
  Add(g_WUI2Images);

  g_WUI3Images := CreateGameImages(g_sSelfFilePath + UIIMAGEFILE3);
  Add(g_WUI3Images);

  g_WMainImages := CreateGameImages(g_sSelfFilePath + MAINIMAGEFILE);
  Add(g_WMainImages);

  g_WMain2Images := CreateGameImages(g_sSelfFilePath + MAINIMAGEFILE2);
  Add(g_WMain2Images);

  g_WMain3Images := CreateGameImages(g_sSelfFilePath + MAINIMAGEFILE3);
  Add(g_WMain3Images);

  // 205新界面专用 piaoyun 2013-10-12
  g_WUINImages := CreateGameImages(g_sSelfFilePath + UINMAGEFILE3);
  Add(g_WUINImages);
  g_WUINImages.ResetWZLAlpha := True;

  g_WNSelectImages := CreateGameImages(g_sSelfFilePath + NSELECTMAGEFILE3);
  Add(g_WNSelectImages);
  g_WNSelectImages.ResetWZLAlpha := True;

  G_WUICommonImages := CreateGameImages(g_sSelfFilePath + UICOMMONMAGEFILE3);
  Add(G_WUICommonImages);
  G_WUICommonImages.ResetWZLAlpha := True;

  // 自定义5个界面图片库 piaoyun 2013-10-24
  for i := Low(g_NewUIImages) to High(g_NewUIImages) do begin
    g_NewUIImages[i] := CreateGameImages(g_sSelfResourcePath + 'data\' + g_NewUiFileNames[i]);
    Add(g_NewUIImages[i]);
  end;
  //////////////////////////////////////////////////////////////////////////////
  // 扩展两个首饰内观效果 piaoyun 2013-08-01
  g_WHeadgearEffect := CreateGameImages(g_sSelfFilePath + HEADGEAREFFECT);
  Add(g_WHeadgearEffect);

  g_WHeadgearEffect2 := CreateGameImages(g_sSelfFilePath + HEADGEAREFFECT2);
  Add(g_WHeadgearEffect2);

  g_WHeadgearEffect3 := CreateGameImages(g_sSelfFilePath + HEADGEAREFFECT3);
  Add(g_WHeadgearEffect3);

  g_WHeadgearEffect4 := CreateGameImages(g_sSelfFilePath + HEADGEAREFFECT4);
  Add(g_WHeadgearEffect4);

  g_WHeadgearEffect5 := CreateGameImages(g_sSelfFilePath + HEADGEAREFFECT5);
  Add(g_WHeadgearEffect5);

  g_WHeadgearEffect6 := CreateGameImages(g_sSelfFilePath + HEADGEAREFFECT6);
  Add(g_WHeadgearEffect6);

  g_WMainImages16 := CreateGameImages(g_sSelfFilePath + MAINIMAGEFILE_16);
  Add(g_WMainImages16);

  g_WMain2Images16 := CreateGameImages(g_sSelfFilePath + MAINIMAGEFILE2_16);
  Add(g_WMain2Images16);

  g_WMain3Images16 := CreateGameImages(g_sSelfFilePath + MAINIMAGEFILE3_16);
  Add(g_WMain3Images16);

  g_WChrSelImages16 := CreateGameImages(g_sSelfFilePath + CHRSELIMAGEFILE_16);
  Add(g_WChrSelImages16);

  g_WEffectImg := CreateGameImages(g_sSelfFilePath + EFFECTIMAGEFILE);
  Add(g_WEffectImg);

  g_WEffectImg_EX := CreateGameImages(g_sSelfFilePath + EFFECTEXIMAGEFILE);
  Add(g_WEffectImg_EX);

  g_WEffectImg_SE := CreateGameImages(g_sSelfFilePath + EFFECTSEIMAGEFILE);
  Add(g_WEffectImg_SE);

  g_WMonEffectImg := CreateGameImages(g_sSelfFilePath + MONEFFECTIMAGEFILE);
  Add(g_WMonEffectImg);

  // 扩展的天气特效文件[专用] -- piaoyun 2013-07-14
  g_WEffectWeatherImg := CreateGameImages(g_sSelfFilePath + EFFECTWEATHERIMAGEFILE);
  Add(g_WEffectWeatherImg);

  g_WShieldImg := CreateGameImages(g_sSelfFilePath + SHIELDIMAGEFILE);
  Add(g_WShieldImg);

  // 骑马 chongchong 2013-10-12
  g_WHorseImg := CreateGameImages(g_sSelfFilePath + HORSEIMAGEFILE);
  Add(g_WHorseImg);

  g_WHorseImg2 := CreateGameImages(g_sSelfFilePath + HORSEIMAGEFILE2);
  Add(g_WHorseImg2);
  // 骑马2的资源不重设置透明度，设置后反而是错误的chongchong 2015-11-12
  //g_WHorseImg2.ResetWZLAlpha := False;
  // 三方骑马 马 (L-Horse) chongchong 2013-10-17
  g_WLHorseImg := CreateGameImages(g_sSelfFilePath + LHorseImageFile);
  Add(g_WLHorseImg);

  // 三方骑马 马 (L-Horse1) chongchong 2013-10-17
  g_WLHorseImg1 := CreateGameImages(g_sSelfFilePath + LHorseImageFile1);
  Add(g_WLHorseImg1);

  // 三方骑马 马 (L-Horse1) chongchong 2014-04-18
  g_WLHorseImg2 := CreateGameImages(g_sSelfFilePath + LHorseImageFile2);
  Add(g_WLHorseImg2);

  // 三方骑马 马特效 (L-HorseEffect) chongchong 2013-10-17
  g_WLHorseEffectImg := CreateGameImages(g_sSelfFilePath + LHorseEffectImageFile);
  Add(g_WLHorseEffectImg);

  // 三方骑马 马特效 (L-HorseEffect1) chongchong 2013-10-17
  g_WLHorseEffectImg1 := CreateGameImages(g_sSelfFilePath + LHorseEffectImageFile1);
  Add(g_WLHorseEffectImg1);

  // 三方骑马 马特效 (L-HorseEffect2) chongchong 2014-04-18
  g_WLHorseEffectImg2 := CreateGameImages(g_sSelfFilePath + LHorseEffectImageFile2);
  Add(g_WLHorseEffectImg2);

  // 三方骑马 马上面人物头发 (L-HairHorse) chongchong 2013-10-17
  g_WLHorseHairImg := CreateGameImages(g_sSelfFilePath + LHorseHairImageFile);
  Add(g_WLHorseHairImg);

  // 三方骑马 马上面人 (L-HumHorse) chongchong 2013-10-17
  g_WLHorseHumImg := CreateGameImages(g_sSelfFilePath + LHorseHumImageFile);
  Add(g_WLHorseHumImg);

  // 三方骑马 马上面人 (L-HumHorse1) chongchong 2014-04-18
  g_WLHorseHumImg1 := CreateGameImages(g_sSelfFilePath + LHorseHumImageFile1);
  Add(g_WLHorseHumImg1);
  g_WLHorseHumImg2 := CreateGameImages(g_sSelfFilePath + LHorseHumImageFile2);
  Add(g_WLHorseHumImg2);
  g_WLHorseHumImg3 := CreateGameImages(g_sSelfFilePath + LHorseHumImageFile3);
  Add(g_WLHorseHumImg3);
  g_WLHorseHumImg4 := CreateGameImages(g_sSelfFilePath + LHorseHumImageFile4);
  Add(g_WLHorseHumImg4);
  g_WLHorseHumImg5 := CreateGameImages(g_sSelfFilePath + LHorseHumImageFile5);
  Add(g_WLHorseHumImg5);
  g_WLHorseHumImg6 := CreateGameImages(g_sSelfFilePath + LHorseHumImageFile6);
  Add(g_WLHorseHumImg6);

  g_WDragonImg := CreateGameImages(g_sSelfFilePath + DRAGONIMAGEFILE);
  Add(g_WDragonImg);

  g_WChrSelImages := CreateGameImages(g_sSelfFilePath + CHRSELIMAGEFILE);
  Add(g_WChrSelImages);

  g_WMMapImages := CreateGameImages(g_sSelfFilePath + MINMAPIMAGEFILE);
  Add(g_WMMapImages);

  g_WMMapImages10 := CreateGameImages(g_sSelfFilePath + MINMAPIMAGEFILE10);
  Add(g_WMMapImages10);

  { TODO -c初始化 -opiaoyun : 小地图扩展mmap11.wil从20001开始【2013-4-23】}
  g_WMMapImages11 := CreateGameImages(g_sSelfFilePath + MINMAPIMAGEFILE11);
  Add(g_WMMapImages11);
  { TODO -c初始化 -opiaoyun : 小地图扩展mmap12.wil从30001开始 【2013-4-23】}
  g_WMMapImages12 := CreateGameImages(g_sSelfFilePath + MINMAPIMAGEFILE12);
  Add(g_WMMapImages12);

  g_WHumWingImages := CreateGameImages(g_sSelfFilePath + HUMWINGIMAGESFILE);
  Add(g_WHumWingImages);

  g_WHairImgImages := CreateGameImages(g_sSelfFilePath + HAIRIMGIMAGESFILE);
  Add(g_WHairImgImages);

  g_WHair2ImgImages := CreateGameImages(g_sSelfFilePath + HAIRIMGIMAGESFILE2);
  Add(g_WHair2ImgImages);

  g_WHair3ImgImages := CreateGameImages(g_sSelfFilePath + HAIRIMGIMAGESFILE3);
  Add(g_WHair3ImgImages);

  g_WHair4ImgImages := CreateGameImages(g_sSelfFilePath + HAIRIMGIMAGESFILE4);
  Add(g_WHair4ImgImages);

  g_WHair5ImgImages := CreateGameImages(g_sSelfFilePath + HAIRIMGIMAGESFILE5);
  Add(g_WHair5ImgImages);

  g_WHair6ImgImages := CreateGameImages(g_sSelfFilePath + HAIRIMGIMAGESFILE6);
  Add(g_WHair6ImgImages);

  { TODO -opiaoyun -c扩展 : 扩展两个发型文件 Hair10.wil、Hair11.wil【2013-6-10】 }
  g_WHair10ImgImages := CreateGameImages(g_sSelfFilePath + HAIRIMGIMAGESFILE10);
  Add(g_WHair10ImgImages);

  g_WHair11ImgImages := CreateGameImages(g_sSelfFilePath + HAIRIMGIMAGESFILE11);
  Add(g_WHair11ImgImages);

  g_WMagIconImages := CreateGameImages(g_sSelfFilePath + MAGICONIMAGESFILE);
  Add(g_WMagIconImages);

  g_WMagIcon2Images := CreateGameImages(g_sSelfFilePath + MAGICON2IMAGESFILE);
  Add(g_WMagIcon2Images);

  g_WMagicImages := CreateGameImages(g_sSelfFilePath + MAGICIMAGESFILE);
  Add(g_WMagicImages);

  g_WMagic2Images := CreateGameImages(g_sSelfFilePath + MAGIC2IMAGESFILE);
  Add(g_WMagic2Images);

  g_WMagic3Images := CreateGameImages(g_sSelfFilePath + MAGIC3IMAGESFILE);
  Add(g_WMagic3Images);

  g_WMagic4Images := CreateGameImages(g_sSelfFilePath + MAGIC4IMAGESFILE);
  Add(g_WMagic4Images);

  g_WMagic5Images := CreateGameImages(g_sSelfFilePath + MAGIC5IMAGESFILE);
  Add(g_WMagic5Images);

  g_WMagic6Images := CreateGameImages(g_sSelfFilePath + MAGIC6IMAGESFILE);
  Add(g_WMagic6Images);

  g_WMagic7Images16 := CreateGameImages(g_sSelfFilePath + MAGIC7IMAGESFILE16);
  Add(g_WMagic7Images16);

  g_WMagic8Images := CreateGameImages(g_sSelfFilePath + MAGIC8IMAGESFILE);
  Add(g_WMagic8Images);

  g_WMagic8Images16 := CreateGameImages(g_sSelfFilePath + MAGIC8IMAGESFILE16);
  Add(g_WMagic8Images16);

  g_WMagic9Images := CreateGameImages(g_sSelfFilePath + MAGIC9IMAGESFILE);
  Add(g_WMagic9Images);

  g_WMagic10Images := CreateGameImages(g_sSelfFilePath + MAGIC10IMAGESFILE);
  Add(g_WMagic10Images);

  g_WMagicreImages := CreateGameImages(g_sSelfFilePath + MAGICREIMAGESFILE);
  Add(g_WMagicreImages);

  g_WAniTilesImages1 := CreateGameImages(g_sSelfFilePath + ANITILESFILE1);
  Add(g_WAniTilesImages1);

  FillChar(g_WObjectArr, SizeOf(g_WObjectArr), 0);

  FillChar(g_EIMapTitleArr, SizeOf(g_EIMapTitleArr), 0);

  g_SafePointEffect := CreateGameImages(g_sSelfFilePath + SAFEPOINTEFFECT);
  Add(g_SafePointEffect);

  g_WTilesImages := TTilesList.Create;
  g_WSmTilesImages := TSmTilesList.Create;

  g_WMonImages := TMonImageList.Create;

  g_WStateItemImages := TStateItemImages.Create();
  g_WBagItemImages := TBagItemImages.Create();
  g_WDnItemImages := TDnItemImages.Create();
  g_WHumImgImages := THumImageList.Create();
  // 翅膀效果扩展 piaoyun 2013-07-28
  g_WHumEffectImages := THumEffectList.Create;
  g_WWeaponImages := TWeaponImageList.Create();
  g_WNpcImgImages := TNpcImageList.Create();
  // 连击武器外观 piaoyun 2013-07-29
  g_WCboWeaponList := TCboWeaponList.Create();
  // 连击人物特效 piaoyun 2013-07-29
  g_WCboHumEffect := TCboHumEffect.Create();
  // 连击人物外观扩展 piaoyun 2013-07-29
  g_WCboHum := TCboHumList.Create();
  // 新武器特效WeaponEffect-WeaponEffect5.wzl piaoyun 2013-07-29
  g_WeaponEffectList := TWeaponEffectList.Create();
  // 新连击武器特效cboWeaponEffect-cboWeaponEffect5.wzl piaoyun 2013-07-30
  g_CboWeaponEffectList := TCboWeaponEffectList.Create();

  g_WBmpUIImages := TUibImages.Create();
  g_WBmpMapImages := TUibImages.Create();
  g_WBmpBookImages := TUibImages.Create();

  g_WBmpUIImages.FileName := g_sSelfFilePath + 'ui.bmp';
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\BankPaymentDown.uib'); // 0
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\BankPaymentNormal.uib'); // 1
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\BookBkgnd.uib'); // 2
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\BookCloseDown.uib'); // 3
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\BookCloseNormal.uib'); // 4
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\BookNextPageDown.uib'); // 5
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\BookNextPageNormal.uib'); // 6
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\BookPrevPageDown.uib'); // 7
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\BookPrevPageNormal.uib'); // 8
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\BoundMibaoWindow.uib'); // 9
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnAAgree1.uib'); // 10
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnAAgree2.uib'); // 11
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnACancel1.uib'); // 12
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnACancel2.uib'); // 13
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnAClear1.uib'); // 14

  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnAClear2.uib'); // 15
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnAClose1.uib'); // 16
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnAClose2.uib'); // 17
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnAOk1.uib'); // 18
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnAOk2.uib'); // 19
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnCommitDown.uib'); // 20
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnCommitNormal.uib'); // 21
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnStyle1Down.uib'); // 22
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnStyle1Normal.uib'); // 23
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnStyle2Down.uib'); // 24
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\btnStyle2Normal.uib'); // 25

  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\BuyLingfuDown.uib'); // 26
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\BuyLingfuNormal.uib'); // 27
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\ChangeBoundTypeWindow.uib'); // 28
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\ChangePasswordWindow.uib'); // 29
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\GloryButton.uib'); // 30
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\HeroStatusWindow.uib'); // 31
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\HotkeyButtonDown.uib'); // 32
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\HotkeyButtonNormal.uib'); // 33
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\MibaoInputWindow.uib'); // 34

  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\NumEditBkgnd.uib'); // 35
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\snda.uib'); // 36
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\SndaPassportRegWindow.uib'); // 37
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\StateWindowHero.uib'); // 38
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\StateWindowHuman.uib'); // 39

  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\SubmitEdCardWindow.uib'); // 40
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\UnboundMibaoWindow.uib'); // 41
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\UserIdentifyButton1.uib'); // 42
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\UserIdentifyButton2.uib'); // 43
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\UserIdentifyWindow.uib'); // 44
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\vigourbar1.uib'); // 45
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\vigourbar2.uib'); // 46
  g_WBmpUIImages.m_FileList.Add(g_sSelfFilePath + 'Data\ui\52gamew.uib'); // 47
  Add(g_WBmpUIImages);

  g_WBmpMapImages.FileName := g_sSelfFilePath + 'minimap.bmp';
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\301.mmap'); // 1000
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\302.mmap'); // 1001
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\303.mmap'); // 1002
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\304.mmap'); // 1003
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\306.mmap'); // 1004
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\308.mmap'); // 1005
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\309.mmap'); // 1006
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\310.mmap'); // 1007
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\311.mmap'); // 1008
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\312.mmap'); // 1009
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\313.mmap'); // 1010
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\314.mmap'); // 1011
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\315.mmap'); // 1012
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\316.mmap'); // 1013
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\317.mmap'); // 1014
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\318.mmap'); // 1015
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\319.mmap'); // 1016
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\320.mmap'); // 1017
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\321.mmap'); // 1018
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\322.mmap'); // 1019
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\323.mmap'); // 1020
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\324.mmap'); // 1021
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\325.mmap'); // 1022
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\326.mmap'); // 1023
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\327.mmap'); // 1024
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\328.mmap'); // 1025
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\329.mmap'); // 1026
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\330.mmap'); // 1027
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\331.mmap'); // 1028
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\402.mmap'); // 1029
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\3021.mmap'); // 1030
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\3022.mmap'); // 1031
  g_WBmpMapImages.m_FileList.Add(g_sSelfFilePath + 'Data\minimap\3023.mmap'); // 1032
  Add(g_WBmpMapImages);

  g_WBmpBookImages.FileName := g_sSelfFilePath + 'books.bmp';
  g_WBmpBookImages.m_FileList.Add(g_sSelfFilePath + 'Data\books\1\1.uib');
  g_WBmpBookImages.m_FileList.Add(g_sSelfFilePath + 'Data\books\1\2.uib');
  g_WBmpBookImages.m_FileList.Add(g_sSelfFilePath + 'Data\books\1\3.uib');
  g_WBmpBookImages.m_FileList.Add(g_sSelfFilePath + 'Data\books\1\4.uib');
  g_WBmpBookImages.m_FileList.Add(g_sSelfFilePath + 'Data\books\1\5.uib');
  g_WBmpBookImages.m_FileList.Add(g_sSelfFilePath + 'Data\books\1\CommandDown.uib');
  g_WBmpBookImages.m_FileList.Add(g_sSelfFilePath + 'Data\books\1\CommandNormal.uib');

  g_WBmpBookImages.m_FileList.Add(g_sSelfFilePath + 'Data\books\2\1.uib');
  g_WBmpBookImages.m_FileList.Add(g_sSelfFilePath + 'Data\books\3\1.uib');
  g_WBmpBookImages.m_FileList.Add(g_sSelfFilePath + 'Data\books\4\1.uib');
  g_WBmpBookImages.m_FileList.Add(g_sSelfFilePath + 'Data\books\5\1.uib');
  g_WBmpBookImages.m_FileList.Add(g_sSelfFilePath + 'Data\books\6\1.uib');
  Add(g_WBmpBookImages);
  // ----------------------------------wis-----------------------------------------
  g_WisMainImages := CreateGameImages(g_sSelfFilePath + WISMAINIMAGEFILE);
  Add(g_WisMainImages);
  // g_WisMain2Images.FileName := WISMAINIMAGEFILE2;
  // g_WisMain2Images.Initialize;
  // g_WisMain3Images.FileName := WISMAINIMAGEFILE3;
  // g_WisMain3Images.Initialize;

  g_WisDnItemImages := CreateGameImages(g_sSelfFilePath + WISDNITEMIMAGESFILE);
  Add(g_WisDnItemImages);

  g_WisBagItemImages := CreateGameImages(g_sSelfFilePath + WISBAGITEMIMAGESFILE);
  Add(g_WisBagItemImages);

  // 武器、衣服内观特效 piaoyun 2013-08-10
  g_StateEffectImages := CreateGameImages(g_sSelfFilePath + STATEEFFECTIMAGESFILE);
  Add(g_StateEffectImages);

  for i := 0 to STATEEFFECTIMAGESFILE_Ex_Count - 1 do begin
    g_StateEffectExImages[i] := CreateGameImages(g_sSelfFilePath + STATEEFFECTIMAGESFILE_Ex[i]);
    Add(g_StateEffectExImages[i]);
  end;

  g_WisStateItemImages := CreateGameImages(g_sSelfFilePath + WISSTATEITEMIMAGESFILE);
  Add(g_WisStateItemImages);

  g_WisWeapon2Images := CreateGameImages(g_sSelfFilePath + WISEAPONIMAGESFILE);
  Add(g_WisWeapon2Images);

  {---------------------------------连击-----------------------------------------}
  g_cboEffect := CreateGameImages(g_sSelfFilePath + CBOEFFECTIMAGEFILE);
  Add(g_cboEffect);

  g_cboHair := CreateGameImages(g_sSelfFilePath + CBOHAIRIMGIMAGESFILE);
  Add(g_cboHair);

  g_cboHair10 := CreateGameImages(g_sSelfFilePath + CBOHAIRIMGIMAGESFILE10);
  Add(g_cboHair10);

  g_cboHair11 := CreateGameImages(g_sSelfFilePath + CBOHAIRIMGIMAGESFILE11);
  Add(g_cboHair11);

  // 连击默认特效chongchong 2015-08-03
  g_cboHumDiys[0] := CreateGameImages(g_sSelfFilePath + CBOHUMDIYFILE);
  Add(g_cboHumDiys[0]);

  for i := 1 to CBOHUMDIYFILE_COUNT - 1 do begin
    g_cboHumDiys[i] := CreateGameImages(g_sSelfFilePath + Format(CBOHUMDIYFILE_ARR, [i + 1]));
    Add(g_cboHumDiys[i]);
  end;

  g_cboHumEffectDiys[0] := CreateGameImages(g_sSelfFilePath + CBOHUMEFFECTDIYFILE);
  Add(g_cboHumEffectDiys[0]);
  for i := 1 to CBOHUMDIYFILE_COUNT - 1 do begin
    g_cboHumEffectDiys[i] := CreateGameImages(g_sSelfFilePath + Format(CBOHUMEFFECTDIYFILE_ARR, [i + 1]));
    Add(g_cboHumEffectDiys[i]);
  end;

  g_cboWeaponDiys[0] := CreateGameImages(g_sSelfFilePath + CBOWEAPONDIYFILE);
  Add(g_cboWeaponDiys[0]);
  for i := 1 to CBOHUMDIYFILE_COUNT - 1 do begin
    g_cboWeaponDiys[i] := CreateGameImages(g_sSelfFilePath + Format(CBOWEAPONDIYFILE_ARR, [i + 1]));
    Add(g_cboWeaponDiys[i]);
  end;

  g_cboWeaponEffectDiys[0] := CreateGameImages(g_sSelfFilePath + CBOWEAPONEFFECTDIYFILE);
  Add(g_cboWeaponEffectDiys[0]);
  for i := 1 to CBOHUMDIYFILE_COUNT - 1 do begin
    g_cboWeaponEffectDiys[i] := CreateGameImages(g_sSelfFilePath + Format(CBOWEAPONEFFECTDIYFILE_ARR, [i + 1]));
    Add(g_cboWeaponEffectDiys[i]);
  end;

  g_WEffectImg_EX.D3DFormat := True;
  g_WEffectImg_SE.D3DFormat := True;
  // 扩展的天气特效文件[专用] -- piaoyun 2013-07-14
  g_WEffectWeatherImg.D3DFormat := True;

  g_WMonEffectImg.D3DFormat := True;

  g_WShieldImg.D3DFormat := True; // 盾牌 chongchong 2013-09-16
  g_WHorseImg.D3DFormat := True; // 骑马 chongchong 2013-10-12

  g_WBmpMapImages.D3DFormat := True;
  g_WMMapImages.D3DFormat := True;
  g_WMMapImages10.D3DFormat := True;

  { TODO -c修改 -opiaoyun : 小地图扩展mmap11.wil从20001开始 【2013-4-23】}
  g_WMMapImages11.D3DFormat := True;
  { TODO -c修改 -opiaoyun : 小地图扩展mmap12.wil从30001开始 【2013-4-23】}
  g_WMMapImages12.D3DFormat := True;

  for i := 0 to 72 do begin
    // pak专用密码 piaoyun 2013-08-27
    GetKeyDataPAK('GEEM2', @APassWord.Chain, @APassWord.KeyData); //GEEM2 原密码 HZQ 20230511
    GetKeyData('GEEM2', @APassWord.ChainLz, @APassWord.KeyDataLz); //GEEM2 原密码 HZQ 20230511

    g_EIMapTitleArr[i] := TPakImages.Create(APassWord); //增加了密码的传入
    Add(g_EIMapTitleArr[i]);
  end;

  g_EIMapTitleArr[0].FileName := g_sSelfResourcePath + 'Mir3MapData\Tilesc.pak';
  g_EIMapTitleArr[1].FileName := g_sSelfResourcePath + 'Mir3MapData\Tiles30c.pak';
  g_EIMapTitleArr[2].FileName := g_sSelfResourcePath + 'Mir3MapData\Tiles5c.pak';
  g_EIMapTitleArr[3].FileName := g_sSelfResourcePath + 'Mir3MapData\Smtilesc.pak';
  g_EIMapTitleArr[4].FileName := g_sSelfResourcePath + 'Mir3MapData\Housesc.pak';
  g_EIMapTitleArr[5].FileName := g_sSelfResourcePath + 'Mir3MapData\Cliffsc.pak';
  g_EIMapTitleArr[6].FileName := g_sSelfResourcePath + 'Mir3MapData\Dungeonsc.pak';
  g_EIMapTitleArr[7].FileName := g_sSelfResourcePath + 'Mir3MapData\Innersc.pak';
  g_EIMapTitleArr[8].FileName := g_sSelfResourcePath + 'Mir3MapData\Furnituresc.pak';
  g_EIMapTitleArr[9].FileName := g_sSelfResourcePath + 'Mir3MapData\Wallsc.pak';
  g_EIMapTitleArr[10].FileName := g_sSelfResourcePath + 'Mir3MapData\SmObjectsc.pak';
  g_EIMapTitleArr[11].FileName := g_sSelfResourcePath + 'Mir3MapData\Animationsc.pak';
  g_EIMapTitleArr[12].FileName := g_sSelfResourcePath + 'Mir3MapData\Object1c.pak';
  g_EIMapTitleArr[13].FileName := g_sSelfResourcePath + 'Mir3MapData\Object2c.pak';

  g_EIMapTitleArr[15].FileName := g_sSelfResourcePath + 'Mir3MapData\Wood\Tilesc.pak';
  g_EIMapTitleArr[16].FileName := g_sSelfResourcePath + 'Mir3MapData\Wood\Tiles30c.pak';
  g_EIMapTitleArr[17].FileName := g_sSelfResourcePath + 'Mir3MapData\Wood\Tiles5c.pak';
  g_EIMapTitleArr[18].FileName := g_sSelfResourcePath + 'Mir3MapData\Wood\Smtilesc.pak';
  g_EIMapTitleArr[19].FileName := g_sSelfResourcePath + 'Mir3MapData\Wood\Housesc.pak';
  g_EIMapTitleArr[20].FileName := g_sSelfResourcePath + 'Mir3MapData\Wood\Cliffsc.pak';

  g_EIMapTitleArr[21].FileName := g_sSelfResourcePath + 'Mir3MapData\Wood\Dungeonsc.pak';
  g_EIMapTitleArr[22].FileName := g_sSelfResourcePath + 'Mir3MapData\Wood\Innersc.pak'; //Furnituresc.pak
  g_EIMapTitleArr[23].FileName := g_sSelfResourcePath + 'Mir3MapData\Wood\Furnituresc.pak';
  g_EIMapTitleArr[24].FileName := g_sSelfResourcePath + 'Mir3MapData\Wood\Wallsc.pak';
  g_EIMapTitleArr[25].FileName := g_sSelfResourcePath + 'Mir3MapData\Wood\SmObjectsc.pak';
  g_EIMapTitleArr[26].FileName := g_sSelfResourcePath + 'Mir3MapData\Wood\Animationsc.pak';

  g_EIMapTitleArr[30].FileName := g_sSelfResourcePath + 'Mir3MapData\Sand\Tilesc.pak';
  g_EIMapTitleArr[31].FileName := g_sSelfResourcePath + 'Mir3MapData\Sand\Tiles30c.pak';
  g_EIMapTitleArr[32].FileName := g_sSelfResourcePath + 'Mir3MapData\Sand\Tiles5c.pak';
  g_EIMapTitleArr[33].FileName := g_sSelfResourcePath + 'Mir3MapData\Sand\Smtilesc.pak';
  g_EIMapTitleArr[34].FileName := g_sSelfResourcePath + 'Mir3MapData\Sand\Housesc.pak';
  g_EIMapTitleArr[35].FileName := g_sSelfResourcePath + 'Mir3MapData\Sand\Cliffsc.pak';
  g_EIMapTitleArr[36].FileName := g_sSelfResourcePath + 'Mir3MapData\Sand\Dungeonsc.pak';
  g_EIMapTitleArr[37].FileName := g_sSelfResourcePath + 'Mir3MapData\Sand\Innersc.pak';
  g_EIMapTitleArr[38].FileName := g_sSelfResourcePath + 'Mir3MapData\Sand\Furnituresc.pak';
  g_EIMapTitleArr[39].FileName := g_sSelfResourcePath + 'Mir3MapData\Sand\Wallsc.pak';
  g_EIMapTitleArr[40].FileName := g_sSelfResourcePath + 'Mir3MapData\Sand\SmObjectsc.pak';
  g_EIMapTitleArr[41].FileName := g_sSelfResourcePath + 'Mir3MapData\Sand\Animationsc.pak';

  g_EIMapTitleArr[45].FileName := g_sSelfResourcePath + 'Mir3MapData\Snow\Tilesc.pak';
  g_EIMapTitleArr[46].FileName := g_sSelfResourcePath + 'Mir3MapData\Snow\Tiles30c.pak';
  g_EIMapTitleArr[47].FileName := g_sSelfResourcePath + 'Mir3MapData\Snow\Tiles5c.pak';
  g_EIMapTitleArr[48].FileName := g_sSelfResourcePath + 'Mir3MapData\Snow\Smtilesc.pak';
  g_EIMapTitleArr[49].FileName := g_sSelfResourcePath + 'Mir3MapData\Snow\Housesc.pak';
  g_EIMapTitleArr[50].FileName := g_sSelfResourcePath + 'Mir3MapData\Snow\Cliffsc.pak';
  g_EIMapTitleArr[51].FileName := g_sSelfResourcePath + 'Mir3MapData\Snow\Dungeonsc.pak';
  g_EIMapTitleArr[52].FileName := g_sSelfResourcePath + 'Mir3MapData\Snow\Innersc.pak';
  g_EIMapTitleArr[53].FileName := g_sSelfResourcePath + 'Mir3MapData\Snow\Furnituresc.pak';
  g_EIMapTitleArr[54].FileName := g_sSelfResourcePath + 'Mir3MapData\Snow\Wallsc.pak';
  g_EIMapTitleArr[55].FileName := g_sSelfResourcePath + 'Mir3MapData\Snow\SmObjectsc.pak';

  g_EIMapTitleArr[56].FileName := g_sSelfResourcePath + 'Mir3MapData\Snow\Animationsc.pak';

  g_EIMapTitleArr[60].FileName := g_sSelfResourcePath + 'Mir3MapData\forest\Tilesc.pak';
  g_EIMapTitleArr[61].FileName := g_sSelfResourcePath + 'Mir3MapData\forest\Tiles30c.pak';
  g_EIMapTitleArr[62].FileName := g_sSelfResourcePath + 'Mir3MapData\forest\Tiles5c.pak';
  g_EIMapTitleArr[63].FileName := g_sSelfResourcePath + 'Mir3MapData\forest\Smtilesc.pak';
  g_EIMapTitleArr[64].FileName := g_sSelfResourcePath + 'Mir3MapData\forest\Housesc.pak';
  g_EIMapTitleArr[65].FileName := g_sSelfResourcePath + 'Mir3MapData\forest\Cliffsc.pak';
  g_EIMapTitleArr[66].FileName := g_sSelfResourcePath + 'Mir3MapData\forest\Dungeonsc.pak';
  g_EIMapTitleArr[67].FileName := g_sSelfResourcePath + 'Mir3MapData\forest\Innersc.pak';
  g_EIMapTitleArr[68].FileName := g_sSelfResourcePath + 'Mir3MapData\forest\Furnituresc.pak';
  g_EIMapTitleArr[69].FileName := g_sSelfResourcePath + 'Mir3MapData\forest\Wallsc.pak';
  g_EIMapTitleArr[70].FileName := g_sSelfResourcePath + 'Mir3MapData\forest\SmObjectsc.pak';
  g_EIMapTitleArr[71].FileName := g_sSelfResourcePath + 'Mir3MapData\forest\Animationsc.pak';
end;

procedure TImageEvent.Add(GameImages:TGameImages);
begin
  m_EventList.AddObject(GameImages.FileName, GameImages);
end;

procedure TImageEvent.AddDynamic(GameImages:TGameImages);
begin
  m_DynamicEventList.AddObject(GameImages.FileName, GameImages);
end;

procedure TImageEvent.AddImageList(GameImages:TGameImages);
begin

end;

function TImageEvent.GetCount:Integer;
begin
  Result := m_EventList.Count;
end;

function TImageEvent.GetImages(Index:Integer):TGameImages;
begin
  Result := TGameImages(m_EventList.Objects[Index]);
end;

function TImageEvent.GetDynamicCount:Integer;
begin
  Result := m_DynamicEventList.Count;
end;

function TImageEvent.GetDynamicGameImages(Index:Integer):TGameImages;
begin
  Result := TGameImages(m_DynamicEventList.Objects[Index]);
end;

procedure TImageEvent.Initialize();
var
  I:Integer;
  GameImages:TGameImages;
begin
  for I := 0 to m_EventList.Count - 1 do begin
    GameImages := TGameImages(m_EventList.Objects[I]);
    GameImages.Initialize();
  end;

  for I := 0 to m_DynamicEventList.Count - 1 do begin
    GameImages := TGameImages(m_DynamicEventList.Objects[I]);
    GameImages.Initialize();
  end;

  g_WBagItemImages.Initialize();
  g_WDnItemImages.Initialize();
  g_WStateItemImages.Initialize();
  g_WHumImgImages.Initialize();
  // 翅膀效果扩展 piaoyun 2013-07-28
  g_WHumEffectImages.Initialize;
  g_WWeaponImages.Initialize();
  g_WNpcImgImages.Initialize();
  g_WMonImages.Initialize();
  // 连击武器外观 piaoyun 2013-07-29
  g_WCboWeaponList.Initialize();
  // 连击人物特效 piaoyun 2013-07-29
  g_WCboHumEffect.Initialize;
  // 连击人物外观扩展 piaoyun 2013-07-29
  g_WCboHum.Initialize;
  // 新武器特效WeaponEffect-WeaponEffect5.wzl piaoyun 2013-07-29
  g_WeaponEffectList.Initialize;
  // 新连击武器特效cboWeaponEffect-cboWeaponEffect5.wzl piaoyun 2013-07-30
  g_CboWeaponEffectList.Initialize;

  LoadEffectImageList;

  ResetNewopUI170TextureArray(-1);
end;

procedure TImageEvent.Finalize();
var
  I:Integer;
begin
  g_EffectImageList.Lock;
  try
    for I := 0 to g_EffectImageList.Count - 1 do
      g_EffectImageList.Objects[I] := nil;
  finally
    g_EffectImageList.UnLock;
  end;

  for I := 0 to m_EventList.Count - 1 do
    TGameImages(m_EventList.Objects[I]).Finalize();

  for I := 0 to m_DynamicEventList.Count - 1 do
    TGameImages(m_DynamicEventList.Objects[I]).Finalize();

  g_WBagItemImages.Finalize();
  g_WDnItemImages.Finalize();
  g_WStateItemImages.Finalize();
  g_WHumImgImages.Finalize();
  // 翅膀效果扩展 piaoyun 2013-07-28
  g_WHumEffectImages.Finalize;
  g_WWeaponImages.Finalize();
  g_WNpcImgImages.Finalize();
  g_WMonImages.Finalize();
  g_WTilesImages.Finalize();
  g_WSmTilesImages.Finalize();
  // 连击武器外观 piaoyun 2013-07-29
  g_WCboWeaponList.Finalize();
  // 连击人物特效 piaoyun 2013-07-29
  g_WCboHumEffect.Finalize();
  // 连击人物外观扩展 piaoyun 2013-07-29
  g_WCboHum.Finalize();
  // 新武器特效WeaponEffect-WeaponEffect5.wzl piaoyun 2013-07-29
  g_WeaponEffectList.Finalize();
  // 新连击武器特效cboWeaponEffect-cboWeaponEffect5.wzl piaoyun 2013-07-30
  g_CboWeaponEffectList.Finalize;

  for I := Low(g_WObjectArr) to High(g_WObjectArr) do begin
    if g_WObjectArr[I] <> nil then begin
      g_WObjectArr[I].Finalize;
      FreeAndNil(g_WObjectArr[I]);
    end;
  end;
end;

procedure TImageEvent.ClearCache(Obj:Byte);
var
  I:Integer;
  GameImages:TGameImages;
begin
  if Obj = 0 then begin
    for I := 0 to m_EventList.Count - 1 do
      TGameImages(m_EventList.Objects[I]).ClearCache();

    for I := 0 to m_DynamicEventList.Count - 1 do
      TGameImages(m_DynamicEventList.Objects[I]).ClearCache();

    g_WBagItemImages.ClearCache();
    g_WDnItemImages.ClearCache();
    g_WStateItemImages.ClearCache();
    g_WHumImgImages.ClearCache();
    // 翅膀效果扩展 piaoyun 2013-07-28
    g_WHumEffectImages.ClearCache();
    g_WWeaponImages.ClearCache();
    g_WNpcImgImages.ClearCache();
    g_WMonImages.ClearCache();
    // 连击武器外观 piaoyun 2013-07-29
    g_WCboWeaponList.ClearCache();
    // 连击人物特效 piaoyun 2013-07-29
    g_WCboHumEffect.ClearCache();
    // 连击人物外观扩展 piaoyun 2013-07-29
    g_WCboHum.ClearCache();
    // 新武器特效WeaponEffect-WeaponEffect5.wzl piaoyun 2013-07-29
    g_WeaponEffectList.ClearCache();
    // 新连击武器特效cboWeaponEffect-cboWeaponEffect5.wzl piaoyun 2013-07-30
    g_CboWeaponEffectList.ClearCache();

    for I := Low(g_WObjectArr) to High(g_WObjectArr) do begin
      if g_WObjectArr[I] <> nil then begin
        g_WObjectArr[I].ClearCache();
      end;
    end;
  end
  else if Obj = 1 then begin
    g_WNewopUIImages.ClearCache();
    //    {$IFDEF BEIJING}
    g_WMobileImages.ClearCache();
    //    {$ENDIF}
    g_WUIImages.ClearCache();
    g_WUI1Images.ClearCache();
    g_WUI2Images.ClearCache();
    g_WUI3Images.ClearCache();
    g_WMainImages.ClearCache();
    g_WMain2Images.ClearCache();
    g_WMain3Images.ClearCache();
    g_WMainImages16.ClearCache();
    g_WMain2Images16.ClearCache();
    g_WMain3Images16.ClearCache();
    g_WChrSelImages16.ClearCache();

    g_WHeadgearEffect.ClearCache;
    g_WHeadgearEffect2.ClearCache;
    g_WHeadgearEffect3.ClearCache;
    g_WHeadgearEffect4.ClearCache;
    g_WHeadgearEffect5.ClearCache;
    g_WHeadgearEffect6.ClearCache;

    g_WUINImages.ClearCache;
    g_WNSelectImages.ClearCache;
    G_WUICommonImages.ClearCache;

    for I := Low(g_NewUIImages) to High(g_NewUIImages) do begin
      g_NewUIImages[I].ClearCache;
    end;
  end
  else begin
    for I := 0 to m_EventList.Count - 1 do begin
      GameImages := TGameImages(m_EventList.Objects[I]);
      if (GameImages = g_WNewopUIImages) or //  {$IFDEF BEIJING}
      (GameImages = g_WMobileImages) or //  {$ENDIF}
      (GameImages = g_WUIImages) or (GameImages = g_WUI1Images) or (GameImages = g_WUI2Images) or (GameImages = g_WUI3Images) or (GameImages = g_WMainImages) or (GameImages = g_WMain2Images) or (GameImages = g_WMain3Images) or (GameImages = g_WMainImages16) or (GameImages = g_WMain2Images16) or (GameImages = g_WMain3Images16) or (GameImages = g_WChrSelImages16) or (GameImages = g_WHairImgImages) or (GameImages = g_WHeadgearEffect) or (GameImages = g_WHeadgearEffect2) or (GameImages = g_WHeadgearEffect3) or (GameImages = g_WHeadgearEffect4) or (GameImages = g_WHeadgearEffect5) or (GameImages = g_WHeadgearEffect6) or (GameImages = g_WUINImages) or (GameImages = g_WNSelectImages) or (GameImages = G_WUICommonImages) then
        Continue;

      GameImages.ClearCache();
    end;

    // for I := 0 to m_DynamicEventList.Count - 1 do
      // TGameImages(m_DynamicEventList.Objects[I]).ClearCache();
  // g_WStateItemImages.ClearCache();
  // g_WHumImgImages.ClearCache();
  // g_WWeaponImages.ClearCache();
  // g_WNpcImgImages.ClearCache();
  // g_WMonImages.ClearCache();
    {for I := Low(g_WObjectArr) to High(g_WObjectArr) do begin
      if g_WObjectArr[I] <> nil then begin
        g_WObjectArr[I].ClearCache();
      end;
    end;}
  end;
end;

procedure TImageEvent.FreeOldMemorys();
var
  I:Integer;
begin
  for I := 0 to m_EventList.Count - 1 do
    TGameImages(m_EventList.Objects[I]).FreeOldMemorys();

  for I := 0 to m_DynamicEventList.Count - 1 do
    TGameImages(m_DynamicEventList.Objects[I]).FreeOldMemorys();

  g_WBagItemImages.FreeOldMemorys();
  g_WDnItemImages.FreeOldMemorys();
  g_WStateItemImages.FreeOldMemorys();
  g_WHumImgImages.FreeOldMemorys();
  // 翅膀效果扩展 piaoyun 2013-07-28
  g_WHumEffectImages.FreeOldMemorys();
  g_WWeaponImages.FreeOldMemorys();
  g_WNpcImgImages.FreeOldMemorys();
  g_WMonImages.FreeOldMemorys();
  // 连击武器外观 piaoyun 2013-07-29
  g_WCboWeaponList.FreeOldMemorys();
  // 连击人物特效 piaoyun 2013-07-29
  g_WCboHumEffect.FreeOldMemorys();
  // 连击人物外观扩展 piaoyun 2013-07-29
  g_WCboHum.FreeOldMemorys();
  // 新武器特效WeaponEffect-WeaponEffect5.wzl piaoyun 2013-07-29
  g_WeaponEffectList.FreeOldMemorys();
  // 新连击武器特效cboWeaponEffect-cboWeaponEffect5.wzl piaoyun 2013-07-30
  g_CboWeaponEffectList.FreeOldMemorys();

  for I := Low(g_WObjectArr) to High(g_WObjectArr) do begin
    if g_WObjectArr[I] <> nil then begin
      // 地图中的对象6秒清理一次 chongchong 2014-03-25
      g_WObjectArr[I].FreeOldMemorys(True);
    end;
  end;
end;

procedure TImageEvent.Clear();
begin
  m_EventList.Clear();
end;

function GetEffectGameImages(FileName:string):TGameImages;
var
  I:Integer;
  GameImages:TGameImages;
begin
  Result := nil;
  FileName := GetGameImagesFileName(g_sSelfFilePath + 'Data\' + FileName);
  for I := 0 to g_ImageEvent.Count - 1 do begin
    GameImages := g_ImageEvent.Images[I];
    if Comparetext(GameImages.FileName, FileName) = 0 then begin
      Result := GameImages;
      Exit;
    end;
  end;
  for I := 0 to g_ImageEvent.DynamicCount - 1 do begin
    GameImages := g_ImageEvent.DynamicImages[I];
    if Comparetext(GameImages.FileName, FileName) = 0 then begin
      Result := GameImages;
      Exit;
    end;
  end;
end;

function GetEffectHumImgImages(FileName:string):TGameImages;
var
  I:Integer;
  sFileName:string;
begin
  Result := nil;
  FileName := GetGameImagesFileName(g_sSelfFilePath + 'Data\' + FileName);
  for I := 0 to 20 do begin
    if I = 0 then
      sFileName := HUMIMGIMAGESFILE
    else
      sFileName := Format(HUMIMGIMAGESFILEX, [I + 1]);
    sFileName := GetGameImagesFileName(g_sSelfFilePath + sFileName);
    if Comparetext(sFileName, FileName) = 0 then begin
      Result := g_WHumImgImages.Indexs[I];
      Exit;
    end;
  end;
end;

function GetEffectMonImgImages(FileName:string):TGameImages;
var
  Index:Integer;
  sTemp, sFileName, sIndex:string;
begin
  Result := nil;

  sTemp := ExtractFileNameOnly(FileName);
  sFileName := Copy(sTemp, 1, Length('Mon'));
  sIndex := Copy(sTemp, Length('Mon') + 1, MaxInt);

  if not SameText(sFileName, 'Mon') then
    Exit;
  Index := StrToIntDef(sIndex, -1);

  if Index >= 0 then begin
    Result := g_WMonImages.Indexs[Index];
  end;

  {
  FileName := GetGameImagesFileName(g_sSelfFilePath + 'Data\' + FileName);
  for I := 0 to 100 do
  begin
    sFileName := Format(MONIMAGEFILE, [I]);
    sFileName := GetGameImagesFileName(g_sSelfFilePath + sFileName);
    if Comparetext(sFileName, FileName) = 0 then
    begin
      Result := g_WMonImages.Indexs[I];
      Exit;
    end;
    // 新怪物测试 piaoyun 2013-07-27
    sFileName := MONKULOUIMAGEFILE;
    sFileName := GetGameImagesFileName(g_sSelfFilePath + sFileName);
    if Comparetext(sFileName, FileName) = 0 then
    begin
      Result := g_WMonImages.Indexs[I];
      Exit;
    end;
  end;
  }
end;

function GetEffectStateItemImgImages(FileName:string):TGameImages;
var
  I:Integer;
  sFileName:string;
begin
  Result := nil;
  FileName := GetGameImagesFileName(g_sSelfFilePath + 'Data\' + FileName);
  for I := 0 to 20 do begin
    if I = 0 then
      sFileName := STATEITEMIMAGESFILE
    else
      sFileName := Format(STATEITEMIMAGESFILEX, [I]);
    sFileName := GetGameImagesFileName(g_sSelfFilePath + sFileName);
    if Comparetext(sFileName, FileName) = 0 then begin
      Result := g_WStateItemImages.Indexs[I];
      Exit;
    end;
  end;
end;

function GetEffectDnItemImgImages(FileName:string):TGameImages;
var
  I:Integer;
  sFileName:string;
begin
  Result := nil;
  FileName := GetGameImagesFileName(g_sSelfFilePath + 'Data\' + FileName);
  for I := 0 to 20 do begin
    if I = 0 then
      sFileName := DNITEMIMAGESFILE
    else
      sFileName := Format(DNITEMIMAGESFILEX, [I]);
    sFileName := GetGameImagesFileName(g_sSelfFilePath + sFileName);
    if Comparetext(sFileName, FileName) = 0 then begin
      Result := g_WDnItemImages.Indexs[I];
      Exit;
    end;
  end;
end;

function GetEffectBagItemImgImages(FileName:string):TGameImages;
var
  I:Integer;
  sFileName:string;
begin
  Result := nil;
  FileName := GetGameImagesFileName(g_sSelfFilePath + 'Data\' + FileName);
  for I := 0 to 20 do begin
    if I = 0 then
      sFileName := BAGITEMIMAGESFILE
    else
      sFileName := Format(BAGITEMIMAGESFILEX, [I]);
    sFileName := GetGameImagesFileName(g_sSelfFilePath + sFileName);
    if Comparetext(sFileName, FileName) = 0 then begin
      Result := g_WBagItemImages.Indexs[I];
      Exit;
    end;
  end;
end;

function GetEffectWeaponImgImages(FileName:string):TGameImages;
var
  I:Integer;
  sFileName:string;
begin
  Result := nil;
  FileName := GetGameImagesFileName(g_sSelfFilePath + 'Data\' + FileName);
  for I := 0 to 20 do begin
    if I = 0 then
      sFileName := WEAPONIMAGESFILE
    else
      sFileName := Format(WEAPONIMAGESFILEX, [I + 1]);
    sFileName := GetGameImagesFileName(g_sSelfFilePath + sFileName);
    if Comparetext(sFileName, FileName) = 0 then begin
      Result := g_WWeaponImages.Indexs[I];
      Exit;
    end;
  end;
end;

function GetEffectNpcImgImages(FileName:string):TGameImages;
var
  I:Integer;
  sFileName:string;
begin
  Result := nil;
  FileName := GetGameImagesFileName(g_sSelfFilePath + 'Data\' + FileName);
  for I := 0 to 20 do begin
    if I = 0 then
      sFileName := NPCIMAGESFILE
    else
      sFileName := Format(NPCIMAGESFILEX, [I + 1]);
    sFileName := GetGameImagesFileName(g_sSelfFilePath + sFileName);
    if Comparetext(sFileName, FileName) = 0 then begin
      Result := g_WNpcImgImages.Indexs[I];
      Exit;
    end;
  end;
end;

(*
function GetEffectObjsImgImages(FileName: string): TGameImages;
var
  I: Integer;
  sFileName: string;
begin
  Result := nil;
  FileName := GetGameImagesFileName(g_sSelfFilePath + 'Data\' + FileName);
  for I := Low(g_WObjectArr) to High(g_WObjectArr) do
  begin
    if I = 0 then
      sFileName := OBJECTIMAGEFILE
    else
      sFileName := Format(OBJECTIMAGEFILE1, [I + 1]);

    sFileName := GetGameImagesFileName(g_sSelfFilePath + sFileName);

    if Comparetext(sFileName, FileName) = 0 then
    begin
      if g_WObjectArr[I] = nil then
      begin
          // sFileName := g_sSelfFilePath + sFileName;
        g_WObjectArr[I] := CreateGameImages(sFileName);
        g_WObjectArr[I].Initialize;
      end;

      Result := g_WObjectArr[I];
      Exit;
    end;
  end;
end;

function GetTilesImgImages(FileName: string): TGameImages;
var
  I: Integer;
  sFileName: string;
begin
  Result := nil;
  FileName := GetGameImagesFileName(g_sSelfFilePath + 'Data\' + FileName);
  for I := 0 to 65535 do
  begin
    if I = 0 then
      sFileName := TITLESIMAGEFILE
    else
      sFileName := Format(TITLESIMAGEFILES, [I + 1]);
    sFileName := GetGameImagesFileName(g_sSelfFilePath + sFileName);
    if Comparetext(sFileName, FileName) = 0 then
    begin
      Result := g_WTilesImages.Indexs[I];
      Exit;
    end;
  end;
end;

function GetSmTilesImgImages(FileName: string): TGameImages;
var
  I: Integer;
  sFileName: string;
begin
  Result := nil;
  FileName := GetGameImagesFileName(g_sSelfFilePath + 'Data\' + FileName);
  for I := 0 to 65535 do
  begin
    if I = 0 then
      sFileName := TITLESIMAGEFILE
    else
      sFileName := Format(TITLESIMAGEFILES, [I + 1]);
    sFileName := GetGameImagesFileName(g_sSelfFilePath + sFileName);
    if Comparetext(sFileName, FileName) = 0 then
    begin
      Result := g_WSmTilesImages.Indexs[I];
      Exit;
    end;
  end;
end;
*)

function GetEffectObjsImgImages(FileName:string):TGameImages;
var
  Index:Integer;
  sTemp, sFileName, sIndex:string;
begin
  Result := nil;

  sTemp := ExtractFileNameOnly(FileName);
  sFileName := Copy(sTemp, 1, Length('Objects'));
  sIndex := Copy(sTemp, Length('Objects') + 1, MaxInt);

  if not SameText(sFileName, 'Objects') then
    Exit;

  if Length(sIndex) = 0 then
    Index := 0
  else begin
    Index := StrToIntDef(sIndex, -1);
    if Index >= 2 then
      Index := Index - 1
    else
      Index := -1;
  end;

  if Index >= 0 then begin
    if g_WObjectArr[Index] = nil then begin
      FileName := GetGameImagesFileName(g_sSelfFilePath + 'Data\' + FileName);
      g_WObjectArr[Index] := CreateGameImages(FileName);
      g_WObjectArr[Index].Initialize;
    end;

    Result := g_WObjectArr[Index];
    Exit;
  end;
end;

function GetTilesImgImages(FileName:string):TGameImages;
var
  Index:Integer;
  sTemp, sFileName, sIndex:string;
begin
  Result := nil;

  sTemp := ExtractFileNameOnly(FileName);
  sFileName := Copy(sTemp, 1, Length('Tiles'));
  sIndex := Copy(sTemp, Length('Tiles') + 1, MaxInt);

  if not SameText(sFileName, 'Tiles') then
    Exit;

  if Length(sIndex) = 0 then
    Index := 0
  else begin
    Index := StrToIntDef(sIndex, -1);
    if Index >= 2 then
      Index := Index - 1
    else
      Index := -1;
  end;

  if Index >= 0 then begin
    Result := g_WTilesImages.Indexs[Index];
    Exit;
  end;
end;

function GetSmTilesImgImages(FileName:string):TGameImages;
var
  Index:Integer;
  sTemp, sFileName, sIndex:string;
begin
  Result := nil;

  sTemp := ExtractFileNameOnly(FileName);
  sFileName := Copy(sTemp, 1, Length('SmTiles'));
  sIndex := Copy(sTemp, Length('SmTiles') + 1, MaxInt);

  if not SameText(sFileName, 'SmTiles') then
    Exit;

  if Length(sIndex) = 0 then
    Index := 0
  else begin
    Index := StrToIntDef(sIndex, -1);
    if Index >= 2 then
      Index := Index - 1
    else
      Index := -1;
  end;

  if Index >= 0 then begin
    Result := g_WSmTilesImages.Indexs[Index];
    Exit;
  end;
end;

function GetGameImages(FileName:string):TGameImages;

  function DeleteNumber(Src:string; var nNum:Integer):string;
  var
    I, Len, nC:Integer;
    sNum:string;
  begin
    Len := Length(Src);
    nC := 0;
    sNum := '';
    for I := Len downto 1 do begin
      if Src[I] in ['0'..'9'] then begin
        Inc(nC);
        sNum := Src[I] + sNum;
      end;
    end;
    nNum := StrToIntDef(sNum, 0);
    if nC = 0 then
      Result := Src
    else
      Result := Copy(Src, 1, Len - nC);
  end;

  function DeleteFileExt(Src:string):string; // 去掉扩展名
  var
    nPos:Integer;
  begin
    nPos := Pos('.', Src);
    if nPos > 0 then
      Result := Copy(Src, 1, nPos - 1)
    else
      Result := Src;
  end;

var
  sFileNameA:string;
  sFileExt:string;
  nNum:Integer;
begin
  FileName := ExtractFileName(FileName);
  Result := GetEffectGameImages(FileName);
  if Result <> nil then
    Exit;

  sFileExt := UpperCase(ExtractFileExt(FileName)); // 获取扩展名
  sFileNameA := DeleteFileExt(FileName); // 去掉扩展名
  sFileNameA := UpperCase(DeleteNumber(sFileNameA, nNum)); // 去掉文件后面的数字

  if sFileNameA = 'HUM' then begin
    Result := GetEffectHumImgImages(FileName);
  end
  else if sFileNameA = 'MON' then begin
    Result := GetEffectMonImgImages(FileName);
  end
  else if sFileNameA = 'STATEITEM' then begin
    Result := GetEffectStateItemImgImages(FileName);
  end
  else if sFileNameA = 'ITEMS' then begin
    Result := GetEffectBagItemImgImages(FileName);
  end
  else if sFileNameA = 'DNITEMS' then begin
    Result := GetEffectDnItemImgImages(FileName);
  end
  else if sFileNameA = 'WEAPON' then begin
    Result := GetEffectWeaponImgImages(FileName);
  end
  else if sFileNameA = 'NPC' then begin
    Result := GetEffectNpcImgImages(FileName);
  end
  else if sFileNameA = 'OBJECTS' then begin
    Result := GetEffectObjsImgImages(FileName);
  end
  else if sFileNameA = 'TILSE' then begin
    Result := GetTilesImgImages(FileName);
  end
  else if sFileNameA = 'SMTILSE' then begin
    Result := GetSmTilesImgImages(FileName);
  end
  else begin
    Result := nil;
  end;
end;

procedure LoadEffectImageList;

  function DeleteNumber(Src:string; var nNum:Integer):string;
  var
    I, Len, nC:Integer;
    sNum:string;
  begin
    Len := Length(Src);
    nC := 0;
    sNum := '';
    for I := Len downto 1 do begin
      if Src[I] in ['0'..'9'] then begin
        Inc(nC);
        sNum := Src[I] + sNum;
      end;
    end;
    nNum := StrToIntDef(sNum, 0);
    if nC = 0 then
      Result := Src
    else
      Result := Copy(Src, 1, Len - nC);
  end;

  function DeleteFileExt(Src:string):string; // 去掉扩展名
  var
    nPos:Integer;
  begin
    nPos := Pos('.', Src);
    if nPos > 0 then
      Result := Copy(Src, 1, nPos - 1)
    else
      Result := Src;
  end;

var
  I:Integer;
  sFileName:string;
  sFileNameA:string;
  sFileExt:string;
  nNum:Integer;
  GameImages:TGameImages;
begin
  for I := 0 to g_EffectImageList.Count - 1 do begin
    if g_EffectImageList.Objects[I] = nil then begin
      g_EffectImageList.Objects[I] := GetEffectGameImages(g_EffectImageList.Strings[I]);
    end;
  end;

  for I := 0 to g_EffectImageList.Count - 1 do begin
    if g_EffectImageList.Objects[I] = nil then begin
      sFileName := g_EffectImageList.Strings[I];
      sFileExt := UpperCase(ExtractFileExt(g_EffectImageList.Strings[I])); // 获取
      sFileNameA := DeleteFileExt(sFileName); // 去掉扩展名
      sFileNameA := UpperCase(DeleteNumber(sFileNameA, nNum)); // 去掉文件后面的数字

      if sFileNameA = 'HUM' then begin
        g_EffectImageList.Objects[I] := GetEffectHumImgImages(sFileName);
      end else if sFileNameA = 'MON' then begin
        g_EffectImageList.Objects[I] := GetEffectMonImgImages(sFileName);
      end else if sFileNameA = 'STATEITEM' then begin
        g_EffectImageList.Objects[I] := GetEffectStateItemImgImages(sFileName);
      end else if sFileNameA = 'ITEMS' then begin
        g_EffectImageList.Objects[I] := GetEffectBagItemImgImages(sFileName);
      end else if sFileNameA = 'DNITEMS' then begin
        g_EffectImageList.Objects[I] := GetEffectDnItemImgImages(sFileName);
      end else if sFileNameA = 'WEAPON' then begin
        g_EffectImageList.Objects[I] := GetEffectWeaponImgImages(sFileName);
      end else if sFileNameA = 'NPC' then begin
        g_EffectImageList.Objects[I] := GetEffectNpcImgImages(sFileName);
      end else if sFileNameA = 'OBJECTS' then begin
        g_EffectImageList.Objects[I] := GetEffectObjsImgImages(sFileName);
      end else if sFileNameA = 'TILSE' then begin
        g_EffectImageList.Objects[I] := GetTilesImgImages(sFileName);
      end else if sFileNameA = 'SMTILSE' then begin
        g_EffectImageList.Objects[I] := GetSmTilesImgImages(sFileName);
      end;
    end;
  end;

  for I := 0 to g_EffectImageList.Count - 1 do begin
    if g_EffectImageList.Objects[I] = nil then begin
      GameImages := CreateGameImages(g_sSelfFilePath + 'Data\' + g_EffectImageList.Strings[I]);
      GameImages.Initialize;
      g_EffectImageList.Objects[I] := GameImages;
      g_ImageEvent.AddDynamic(GameImages);
    end;
  end;
end;

function GetEffectImageListTexture(nResID, nResIndex:Integer):TTexture;
var
    GameImages:TGameImages;
begin
    Result := nil;
    g_EffectImageList.Lock;
    try
        if nResID < g_EffectImageList.Count then begin
            GameImages := TGameImages(g_EffectImageList.Objects[nResID]);
            if GameImages <> nil then begin
                Result := GameImages.Images[nResIndex];
            end;
        end;
    finally
        g_EffectImageList.UnLock;
    end;
end;

// 取地图图库

function GetObjNeedUpdate(nUnit, nIdx:Integer):Boolean; // 检测是否需要更新图库
var
  sFileName:string;
begin
  Result := False;
  if g_MySelf = nil then
    Exit;

  if (nUnit < Low(g_WObjectArr)) or (nUnit > High(g_WObjectArr)) then
    nUnit := 0;

  if g_WObjectArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + OBJECTIMAGEFILE
    else
      sFileName := g_sSelfFilePath + Format(OBJECTIMAGEFILE1, [nUnit + 1]);
    // if not FileExists(sFileName) then Exit;
    g_WObjectArr[nUnit] := CreateGameImages(sFileName);
    // g_WObjectArr[nUnit].FileName := sFileName;
    g_WObjectArr[nUnit].Initialize;
  end;
  Result := g_WObjectArr[nUnit].GetNeedUpdate(nIdx);
end;

function GetObjs(nUnit, nIdx:Integer; IsHMap:Boolean):TTexture;
var
  sFileName, sGetFileName:string;
begin
  Result := nil;
  if g_MySelf = nil then
    Exit;

  if IsHMap then begin
    if (nUnit < Low(g_HMapWObjectArr)) or (nUnit > High(g_HMapWObjectArr)) then
      nUnit := 0;

    if nUnit = 0 then
      sFileName := g_sSelfResourcePath + HMAP_OBJECTIMAGEFILE
    else
      sFileName := g_sSelfResourcePath + Format(HMAP_OBJECTIMAGEFILE1, [nUnit + 1]);

    sGetFileName := sFileName;
    //if GetHMapDataFiles(sFileName, sGetFileName) then
    begin
      if g_HMapWObjectArr[nUnit] = nil then begin
        g_HMapWObjectArr[nUnit] := CreateHMapDataGameImages(sGetFileName);
        g_HMapWObjectArr[nUnit].Initialize;
      end;

      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        Result := g_HMapWObjectArr[nUnit].Grays[nIdx]
      else
        Result := g_HMapWObjectArr[nUnit].Images[nIdx];

      Exit;
    end;
  end;

  if (nUnit < Low(g_WObjectArr)) or (nUnit > High(g_WObjectArr)) then
    nUnit := 0;

  if g_WObjectArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + OBJECTIMAGEFILE
    else
      sFileName := g_sSelfFilePath + Format(OBJECTIMAGEFILE1, [nUnit + 1]);
    // if not FileExists(sFileName) then Exit;
    g_WObjectArr[nUnit] := CreateGameImages(sFileName);
    // g_WObjectArr[nUnit].FileName := sFileName;
    g_WObjectArr[nUnit].Initialize;
  end;
  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    Result := g_WObjectArr[nUnit].Grays[nIdx]
  else
    Result := g_WObjectArr[nUnit].Images[nIdx];
end;

// 取地图图库

function GetObjsEx(nUnit, nIdx:Integer; IsHMap:Boolean; var px, py:Integer):TTexture;
var
  sFileName, sGetFileName:string;
begin
  Result := nil;
  if g_MySelf = nil then
    Exit;

  if IsHMap then begin
    if (nUnit < Low(g_HMapWObjectArr)) or (nUnit > High(g_HMapWObjectArr)) then
      nUnit := 0;

    if nUnit = 0 then
      sFileName := g_sSelfResourcePath + HMAP_OBJECTIMAGEFILE
    else
      sFileName := g_sSelfResourcePath + Format(HMAP_OBJECTIMAGEFILE1, [nUnit + 1]);

    sGetFileName := sFileName;
    //if GetHMapDataFiles(sFileName, sGetFileName) then
    begin
      if g_HMapWObjectArr[nUnit] = nil then begin
        g_HMapWObjectArr[nUnit] := CreateHMapDataGameImages(sGetFileName);
        g_HMapWObjectArr[nUnit].Initialize;
      end;

      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        Result := g_HMapWObjectArr[nUnit].GetCachedGrayImage(nIdx, px, py)
      else
        Result := g_HMapWObjectArr[nUnit].GetCachedImage(nIdx, px, py);

      Exit;
    end;
  end;

  if (nUnit < Low(g_WObjectArr)) or (nUnit > High(g_WObjectArr)) then
    nUnit := 0;
  if g_WObjectArr[nUnit] = nil then begin

    if nUnit = 0 then
      sFileName := g_sSelfFilePath + OBJECTIMAGEFILE
    else
      sFileName := g_sSelfFilePath + Format(OBJECTIMAGEFILE1, [nUnit + 1]);

    // if not FileExists(sFileName) then Exit;
    g_WObjectArr[nUnit] := CreateGameImages(sFileName);
    // g_WObjectArr[nUnit].FileName := sFileName;
    g_WObjectArr[nUnit].Initialize;
  end;
  if (g_MySelf <> nil) and g_MySelf.m_boDeath then
    Result := g_WObjectArr[nUnit].GetCachedGrayImage(nIdx, px, py)
  else
    Result := g_WObjectArr[nUnit].GetCachedImage(nIdx, px, py);
end;

// 不载入内容，直接取图片的大小 chongchong 2014-03-26

function GetObjInfo(nUnit, nIdx:Integer; IsHMap:Boolean; var ASize:TSize; var APoint:TPoint):Boolean;
var
  sFileName, sGetFileName:string;
  ErrorNum:Integer;
begin
  Result := False;
  if g_MySelf = nil then
    Exit;

  if IsHMap then begin
    if (nUnit < Low(g_HMapWObjectArr)) or (nUnit > High(g_HMapWObjectArr)) then
      nUnit := 0;

    if nUnit = 0 then
      sFileName := g_sSelfResourcePath + HMAP_OBJECTIMAGEFILE
    else
      sFileName := g_sSelfResourcePath + Format(HMAP_OBJECTIMAGEFILE1, [nUnit + 1]);

    sGetFileName := sFileName;
    //if GetHMapDataFiles(sFileName, sGetFileName) then
    begin
      if g_HMapWObjectArr[nUnit] = nil then begin
        g_HMapWObjectArr[nUnit] := CreateHMapDataGameImages(sGetFileName);
        g_HMapWObjectArr[nUnit].Initialize;
      end;

      Result := g_HMapWObjectArr[nUnit].GetCachedImageSize(nIdx, ASize, APoint);

      Exit;
    end;
  end;

  if (nUnit < Low(g_WObjectArr)) or (nUnit > High(g_WObjectArr)) then
    nUnit := 0;

  ErrorNum := 1;
  try
    if g_WObjectArr[nUnit] = nil then begin
      ErrorNum := 2;
      if nUnit = 0 then
        sFileName := g_sSelfFilePath + OBJECTIMAGEFILE
      else
        sFileName := g_sSelfFilePath + Format(OBJECTIMAGEFILE1, [nUnit + 1]);
      ErrorNum := 3;
      // if not FileExists(sFileName) then Exit;
      g_WObjectArr[nUnit] := CreateGameImages(sFileName);

      ErrorNum := 4;
      // g_WObjectArr[nUnit].FileName := sFileName;
      g_WObjectArr[nUnit].Initialize;

      ErrorNum := 5;
    end;
    ErrorNum := 6;
    Result := g_WObjectArr[nUnit].GetCachedImageSize(nIdx, ASize, APoint);
  except
    on E:Exception do begin
      DebugOutStr('GetObjInfo error');
      if g_WObjectArr[nUnit] <> nil then
        DebugOutStr('GetObjInfo error-' + IntToStr(ErrorNum) + ', [' + g_WObjectArr[nUnit].ClassName + '] ' + g_WObjectArr[nUnit].FileName + ' index:' + IntToStr(nIdx));
      DebugOutStr(E.Message);
    end;
  end;
end;

function GetMonImg(nAppr:Integer):TGameImages;
begin
  Result := nil; //HZQ 20230524 此函数可能未使用
end;

function GetMonAction(nAppr:Integer):pTMonsterAction;
var
  FileStream:TFileStream;
  sFileName:string;
  MonsterAction:TMonsterAction;
begin
  Result := nil;
  if Length(g_ResourcesDir) > 0 then
    sFileName := g_sSelfResourcePath + Format(MONPMFILE, [nAppr])
  else
    sFileName := g_sSelfFilePath + Format(MONPMFILE, [nAppr]);

  if FileExists(sFileName) then begin
    FileStream := TFileStream.Create(sFileName, fmOpenRead or fmShareDenyNone);
    FileStream.Read(MonsterAction, SizeOf(MonsterAction));
    New(Result);
    Result^ := MonsterAction;
    FileStream.Free;
  end;
end;

// 取得职业名称
// 0 武士
// 1 魔法师
// 2 道士

function GetJobName(nJob:Integer):string;
begin
  Result := '';
  case nJob of
    0:
      Result := g_sWarriorName;
    1:
      Result := g_sWizardName;
    2:
      Result := g_sTaoistName;
    else begin
        Result := g_sUnKnowName;
      end;
  end;
end;

function GetSexName(nSex:Integer):string;
begin
  Result := '';
  case nSex of
    0:
      Result := '男';
    1:
      Result := '女';
  end;
end;

function _FileSize(const fname:string):LongWord;
var
  SearchRec:TSearchRec;
begin
  if FindFirst(ExpandFileName(fname), faAnyFile, SearchRec) = 0 then
    Result := SearchRec.Size
  else
    Result := 0;
end;

procedure DebugOutStr(Msg:string; boWriteDate:Boolean);
var
  sFilePath:string;
  flname:string;
  fhandle:TextFile;
  Year, Month, Day:Word;
begin
  EnterCriticalSection(g_LockDebugOutStr);
  try
    if boWriteDate then
      Msg := FormatDateTime('yyyy-mm-dd hh:mm:ss', Now) + ' ' + Msg;

    DecodeDate(Now, Year, Month, Day);

    sFilePath := g_sSelfFilePath + 'debug\' + IntToStr(Year) + '-' + IntToStr2(Month) + '\';
    // showmessage(sFilePath);

    if not DirectoryExists(sFilePath) then begin
      ForceDirectories(sFilePath);
    end;

    try
      flname := sFilePath + IntToStr2(Day) + '.txt';

      // 修正日志文件超过2G会导致白屏 chongchong 2018-07-26 14:41:42
      if (_FileSize(flname) >= 2100000000) then begin
        DeleteFile(flname)
      end;

      try
        AssignFile(fhandle, flname);

        if FileExists(flname) then begin
          {$I-}
          Append(fhandle);
          {$I+}
        end
        else begin
          {$I-}
          Rewrite(fhandle);
          {$I+}
        end;

        if IOResult = 0 then
          Writeln(fhandle, Msg);
      finally
        CloseFile(fhandle);
      end;
    except
    end;
  finally
    LeaveCriticalSection(g_LockDebugOutStr);
  end;
end;

function HumBagNoUseItemCount:Integer;
var
  I:Integer;
begin
  Result := 0;
  for I := Low(g_ItemArr) to GetMaxBagCount - 1 do begin
    if g_ItemArr[I].S.Name = '' then
      Inc(Result);
  end;
end;

function HeroBagItemCount:Integer;
var
  I:Integer;
begin
  Result := 0;
  for I := Low(g_HeroItemArr) to High(g_HeroItemArr) do begin
    if g_HeroItemArr[I].S.Name <> '' then
      Inc(Result);
  end;
end;

{------------------------------------------------------------------------------}

procedure LoadSkillDescList;
var
  I, II:Integer;
  sFileName, sLineText, sSkillType, sSkillName, sSkillDesc:string;
  LoadList:TStringList;
  SkillDesc:pTSkillDesc;
begin
  LoadList := TStringList.Create;

  sFileName := g_sSelfResourcePath + SKILLDESCFILE;

  if FileExists(sFileName) then begin
    try
      LoadList.LoadFromFile(sFileName);
    except

    end;
  end
  else begin
    sFileName := g_sSelfFilePath + SKILLDESCFILE;
    if FileExists(sFileName) then begin
      try
        LoadList.LoadFromFile(sFileName);
      except

      end;
    end;
  end;

  for I := 0 to LoadList.Count - 1 do begin
    sLineText := Trim(LoadList.Strings[I]);
    if (sLineText <> '') and (sLineText[1] <> ';') then begin
      sLineText := GetValidStr3_Ex(sLineText, sSkillType, ',');
      sLineText := GetValidStr3_Ex(sLineText, sSkillName, ',');
      sLineText := GetValidStr3_Ex(sLineText, sSkillDesc, ',');
      if (sSkillType <> '') and (sSkillName <> '') and (sSkillDesc <> '') then begin
        New(SkillDesc);
        SkillDesc.SkillType := sSkillType;
        SkillDesc.SkillName := sSkillName;
        SkillDesc.SkillDesc := TStringList.Create;
        ExtractStrings([',', '\'], [' '], PChar(sSkillDesc), SkillDesc.SkillDesc);
        for II := 0 to SkillDesc.SkillDesc.Count - 1 do begin
          SkillDesc.SkillDesc.Objects[II] := TObject(clYellow);
        end;
        SkillDesc.SkillDesc.InsertObject(0, sSkillName + '：', TObject(clLime));
        if CompareText(sSkillType, '普通技能') = 0 then
          SkillDesc.SkillDesc.AddObject(DecodeResStr(SMagicSetKeyHint), TObject(clWhite));
        g_SkillDescList.AddObject(SkillDesc.SkillName, TObject(SkillDesc));
      end;
    end;
  end;
  LoadList.Free;
end;

procedure UnLoadSkillDescList;
var
  I:Integer;
  SkillDesc:pTSkillDesc;
begin
  for I := 0 to g_SkillDescList.Count - 1 do begin
    SkillDesc := pTSkillDesc(g_SkillDescList.Objects[I]);
    SkillDesc.SkillDesc.Free;
    Dispose(SkillDesc);
  end;
  g_SkillDescList.Free;
end;

procedure GetSkillDesc(sSkillName:string; var DescList:TStrings);
var
  I:Integer;
begin
  DescList := nil;
  g_SkillDescList.Lock;
  try
    I := g_SkillDescList.IndexOf(sSkillName);
    if I >= 0 then begin
      DescList := pTSkillDesc(g_SkillDescList.Objects[I]).SkillDesc;
    end;
  finally
    g_SkillDescList.UnLock;
  end;
end;

procedure LoadSkillUpgradeDescList;
var
  I, II:Integer;
  sFileName, sLineText, sSkillType, sSkillName, sSkillLevel, sSkillUpgradeDesc:string;
  LoadList:TStringList;
  SkillUpgradeDesc:pTSkillDesc;
begin
  LoadList := TStringList.Create;

  sFileName := g_sSelfResourcePath + SkillUpgradeDescFILE;

  if FileExists(sFileName) then begin
    try
      LoadList.LoadFromFile(sFileName);
    except

    end;
  end
  else begin
    sFileName := g_sSelfFilePath + SkillUpgradeDescFILE;
    if FileExists(sFileName) then begin
      try
        LoadList.LoadFromFile(sFileName);
      except

      end;
    end
    else begin
      LoadList.Add(DecodeResStr(SUpgradeDescText));
      LoadList.SaveToFile(sFileName);
    end;
  end;

  for I := 0 to LoadList.Count - 1 do begin
    sLineText := Trim(LoadList.Strings[I]);
    if (sLineText <> '') and (sLineText[1] <> ';') then begin
      sLineText := GetValidStr3_Ex(sLineText, sSkillType, ',');
      sLineText := GetValidStr3_Ex(sLineText, sSkillName, ',');
      sLineText := GetValidStr3_Ex(sLineText, sSkillLevel, ',');
      sSkillUpgradeDesc := sLineText; // GetValidStr3_Ex(sLineText, , ',');
      if (sSkillType <> '') and (sSkillName <> '') and (sSkillUpgradeDesc <> '') then begin
        New(SkillUpgradeDesc);
        SkillUpgradeDesc.SkillType := sSkillType;
        SkillUpgradeDesc.SkillName := sSkillName;
        SkillUpgradeDesc.SkillLevel := StrToIntDef(sSkillLevel, 0);
        SkillUpgradeDesc.SkillDesc := TStringList.Create;
        ExtractStrings([','], [' '], PChar(sSkillUpgradeDesc), SkillUpgradeDesc.SkillDesc);
        for II := 0 to SkillUpgradeDesc.SkillDesc.Count - 1 do begin
          SkillUpgradeDesc.SkillDesc.Objects[II] := TObject(clYellow);
        end;
        SkillUpgradeDesc.SkillDesc.InsertObject(0, sSkillName + '：', TObject(clLime));
        if CompareText(sSkillType, '普通技能') = 0 then
          SkillUpgradeDesc.SkillDesc.AddObject(DecodeResStr(SMagicUpgrateHint), TObject(clWhite));
        g_SkillUpgradeDescList.AddObject(SkillUpgradeDesc.SkillName, TObject(SkillUpgradeDesc));
      end;
    end;
  end;
  LoadList.Free;
end;

procedure UnLoadSkillUpgradeDescList;
var
  I:Integer;
  SkillUpgradeDesc:pTSkillDesc;
begin
  for I := 0 to g_SkillUpgradeDescList.Count - 1 do begin
    SkillUpgradeDesc := pTSkillDesc(g_SkillUpgradeDescList.Objects[I]);
    SkillUpgradeDesc.SkillDesc.Free;
    Dispose(SkillUpgradeDesc);
  end;
  g_SkillUpgradeDescList.Free;
end;

procedure GetSkillUpgradeDesc(sSkillName:string; NewLevel:Integer; var DescList:TStrings);
var
  I:Integer;
  SkillUpgradeDesc:pTSkillDesc;
begin
  g_SkillUpgradeDescList.Lock;
  try
    for I := 0 to g_SkillUpgradeDescList.Count - 1 do begin
      SkillUpgradeDesc := pTSkillDesc(g_SkillUpgradeDescList.Objects[I]);

      if (CompareText(g_SkillUpgradeDescList.Strings[I], sSkillName) = 0) and (SkillUpgradeDesc.SkillLevel = NewLevel) then begin
        DescList := SkillUpgradeDesc.SkillDesc;
        // DescList.AddStrings(pTSkillDesc(g_SkillUpgradeDescList.Objects[I]).SkillDesc);
        Break;
      end;
    end;
  finally
    g_SkillUpgradeDescList.UnLock;
  end;
end;

{------------------------------------------------------------------------------}

procedure LoadItemDescList(LoadList:TStringList);
var
  I, II, nPos:Integer;
  sLineText, sDesc, sItemDesc:string;
  ItemDesc:pTItemDesc;
  StringList:TStringList;
  btColor:Byte;
begin
  g_ItemDescList.Lock;
  try
    for I := 0 to g_ItemDescList.Count - 1 do begin
      ItemDesc := pTItemDesc(g_ItemDescList.Objects[I]);
      ItemDesc.Desc.Free;
      Dispose(ItemDesc);
    end;
    g_ItemDescList.Clear;
  finally
    g_ItemDescList.UnLock;
  end;

  g_ItemDescList.Lock;
  try
    StringList := TStringList.Create;
    for I := 0 to LoadList.Count - 1 do begin
      sLineText := Trim(LoadList.Strings[I]);
      if (sLineText <> '') and (sLineText[1] <> ';') then begin
        nPos := Pos('=', sLineText);
        if nPos > 0 then begin
          New(ItemDesc);
          ItemDesc.Name := Trim(Copy(sLineText, 1, nPos - 1));
          sDesc := Trim(Copy(sLineText, nPos + 1, Length(sLineText)));
          ItemDesc.Desc := TStringList.Create;
          StringList.Clear;

          ExtractStrings(['\'], [], PChar(sDesc), StringList);
          for II := 0 to StringList.Count - 1 do begin
            sItemDesc := StringList.Strings[II];
            if sItemDesc <> '' then begin
              btColor := 255;
              nPos := Pos('/', sItemDesc);
              if nPos > 0 then begin
                btColor := StrToIntDef(Trim(Copy(sItemDesc, 1, nPos - 1)), btColor);
                sItemDesc := Trim(Copy(sItemDesc, nPos + 1, Length(sItemDesc)));
              end;
              ItemDesc.Desc.AddObject(sItemDesc, TObject(GetRGB(btColor)));
            end;
          end;
          g_ItemDescList.AddObject(ItemDesc.Name, TObject(ItemDesc));
        end;
      end;
    end;
    StringList.Free;
  finally
    g_ItemDescList.UnLock;
  end;
end;

procedure UnLoadItemDescList;
var
  I:Integer;
  ItemDesc:pTItemDesc;
begin
  for I := 0 to g_ItemDescList.Count - 1 do begin
    ItemDesc := pTItemDesc(g_ItemDescList.Objects[I]);
    ItemDesc.Desc.Free;
    Dispose(ItemDesc);
  end;

  FreeAndNil(g_ItemDescList);
end;

procedure LoadItemDescTopList(LoadList:TStringList);
var
  I, II, nPos:Integer;
  sLineText, sDesc, sItemDesc:string;
  ItemDesc:pTItemDesc;
  StringList:TStringList;
  btColor:Byte;
begin
  g_ItemDescTopList.Lock;
  try
    for I := 0 to g_ItemDescTopList.Count - 1 do begin
      ItemDesc := pTItemDesc(g_ItemDescTopList.Objects[I]);
      ItemDesc.Desc.Free;
      Dispose(ItemDesc);
    end;
    g_ItemDescTopList.Clear;
  finally
    g_ItemDescTopList.UnLock;
  end;

  g_ItemDescTopList.Lock;
  try
    StringList := TStringList.Create;
    for I := 0 to LoadList.Count - 1 do begin
      sLineText := Trim(LoadList.Strings[I]);
      if (sLineText <> '') and (sLineText[1] <> ';') then begin
        nPos := Pos('=', sLineText);
        if nPos > 0 then begin
          New(ItemDesc);
          ItemDesc.Name := Trim(Copy(sLineText, 1, nPos - 1));
          sDesc := Trim(Copy(sLineText, nPos + 1, Length(sLineText)));
          ItemDesc.Desc := TStringList.Create;
          StringList.Clear;

          ExtractStrings(['\'], [], PChar(sDesc), StringList);
          for II := 0 to StringList.Count - 1 do begin
            sItemDesc := StringList.Strings[II];
            if sItemDesc <> '' then begin
              btColor := 255;
              nPos := Pos('/', sItemDesc);
              if nPos > 0 then begin
                btColor := StrToIntDef(Trim(Copy(sItemDesc, 1, nPos - 1)), btColor);
                sItemDesc := Trim(Copy(sItemDesc, nPos + 1, Length(sItemDesc)));
              end;
              ItemDesc.Desc.AddObject(sItemDesc, TObject(GetRGB(btColor)));
            end;
          end;
          g_ItemDescTopList.AddObject(ItemDesc.Name, TObject(ItemDesc));
        end;
      end;
    end;
    StringList.Free;
  finally
    g_ItemDescTopList.UnLock;
  end;
end;

procedure UnLoadItemDescTopList;
var
  I:Integer;
  ItemDesc:pTItemDesc;
begin
  for I := 0 to g_ItemDescTopList.Count - 1 do begin
    ItemDesc := pTItemDesc(g_ItemDescTopList.Objects[I]);
    ItemDesc.Desc.Free;
    Dispose(ItemDesc);
  end;

  FreeAndNil(g_ItemDescTopList);
end;

function sub_49ADB8(nPos:Integer; sMsg, sStr, sText:string):string; // 0049ADB8
var
  n10:Integer;
  s14, s18:string;
begin
  if nPos > 0 then begin
    s14 := Copy(sMsg, 1, nPos - 1);
    s18 := Copy(sMsg, Length(sStr) + nPos, Length(sMsg));
    Result := s14 + sText + s18;
  end
  else begin
    n10 := Pos(sStr, sMsg);
    if n10 > 0 then begin
      s14 := Copy(sMsg, 1, n10 - 1);
      s18 := Copy(sMsg, Length(sStr) + n10, Length(sMsg));
      Result := s14 + sText + s18;
    end
    else
      Result := sMsg;
  end;
end;

function GetVariableText(Item:PTClientItem; var sMsg:string; sVariable:string; nPos:Integer):Boolean;
var
  sText:string;
begin
  Result := True;
  sVariable := UpperCase(sVariable);

  if Item <> nil then begin
    if sVariable = '$NAME' then begin
      sText := Item.s.Name;
      sMsg := sub_49ADB8(nPos, sMsg, '<$NAME>', sText);
      Exit;
    end;

    if sVariable = '$STDMODE' then begin
      sText := IntToStr(Item.s.StdMode);
      sMsg := sub_49ADB8(nPos, sMsg, '<$STDMODE>', sText);
      Exit;
    end;

    if sVariable = '$SHAPE' then begin
      sText := IntToStr(Item.s.StdMode);
      sMsg := sub_49ADB8(nPos, sMsg, '<$SHAPE>', sText);
      Exit;
    end;

    if sVariable = '$WEIGHT' then begin
      sText := IntToStr(Item.s.Weight);
      sMsg := sub_49ADB8(nPos, sMsg, '<$WEIGHT>', sText);
      Exit;
    end;

    if sVariable = '$ANICOUNT' then begin
      sText := IntToStr(Item.s.Anicount);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ANICOUNT>', sText);
      Exit;
    end;

    if sVariable = '$SOURCE' then begin
      sText := IntToStr(Item.s.Source);
      sMsg := sub_49ADB8(nPos, sMsg, '<$SOURCE>', sText);
      Exit;
    end;

    if sVariable = '$RESERVED' then begin
      sText := IntToStr(Item.s.Reserved);
      sMsg := sub_49ADB8(nPos, sMsg, '<$RESERVED>', sText);
      Exit;
    end;

    if sVariable = '$LOOKS' then begin
      sText := IntToStr(Item.s.Looks);
      sMsg := sub_49ADB8(nPos, sMsg, '<$LOOKS>', sText);
      Exit;
    end;

    if sVariable = '$DURAMAX' then begin
      sText := IntToStr(Item.s.DuraMax);
      sMsg := sub_49ADB8(nPos, sMsg, '<$DURAMAX>', sText);
      Exit;
    end;

    if sVariable = '$AC' then begin
      sText := IntToStr(Item.s.Ac1);
      sMsg := sub_49ADB8(nPos, sMsg, '<$AC>', sText);
      Exit;
    end;

    if sVariable = '$AC2' then begin
      sText := IntToStr(Item.s.Ac2);
      sMsg := sub_49ADB8(nPos, sMsg, '<$AC2>', sText);
      Exit;
    end;

    if sVariable = '$MAC' then begin
      sText := IntToStr(Item.s.MAC1);
      sMsg := sub_49ADB8(nPos, sMsg, '<$MAC>', sText);
      Exit;
    end;

    if sVariable = '$MAC2' then begin
      sText := IntToStr(Item.s.MAC2);
      sMsg := sub_49ADB8(nPos, sMsg, '<$MAC2>', sText);
      Exit;
    end;

    if sVariable = '$DC' then begin
      sText := IntToStr(Item.s.DC1);
      sMsg := sub_49ADB8(nPos, sMsg, '<$DC>', sText);
      Exit;
    end;

    if sVariable = '$DC2' then begin
      sText := IntToStr(Item.s.DC2);
      sMsg := sub_49ADB8(nPos, sMsg, '<$DC2>', sText);
      Exit;
    end;

    if sVariable = '$MC' then begin
      sText := IntToStr(Item.s.MC1);
      sMsg := sub_49ADB8(nPos, sMsg, '<$MC>', sText);
      Exit;
    end;

    if sVariable = '$MC2' then begin
      sText := IntToStr(Item.s.MC2);
      sMsg := sub_49ADB8(nPos, sMsg, '<$MC2>', sText);
      Exit;
    end;

    if sVariable = '$SC' then begin
      sText := IntToStr(Item.s.SC1);
      sMsg := sub_49ADB8(nPos, sMsg, '<$SC>', sText);
      Exit;
    end;

    if sVariable = '$SC2' then begin
      sText := IntToStr(Item.s.SC2);
      sMsg := sub_49ADB8(nPos, sMsg, '<$SC2>', sText);
      Exit;
    end;

    if sVariable = '$NEED' then begin
      sText := IntToStr(Item.s.Need);
      sMsg := sub_49ADB8(nPos, sMsg, '<$NEED>', sText);
      Exit;
    end;

    if sVariable = '$NEEDLEVEL' then begin
      sText := IntToStr(Item.s.NeedLevel);
      sMsg := sub_49ADB8(nPos, sMsg, '<$NEEDLEVEL>', sText);
      Exit;
    end;

    if sVariable = '$STOCK' then begin
      sText := IntToStr(Item.s.Stock);
      sMsg := sub_49ADB8(nPos, sMsg, '<$STOCK>', sText);
      Exit;
    end;

    if sVariable = '$COLOR' then begin
      sText := IntToStr(Item.s.Color);
      sMsg := sub_49ADB8(nPos, sMsg, '<$COLOR>', sText);
      Exit;
    end;

    if sVariable = '$OVERLAP' then begin
      sText := IntToStr(Item.s.OverLap);
      sMsg := sub_49ADB8(nPos, sMsg, '<$OVERLAP>', sText);
      Exit;
    end;

    if sVariable = '$LIGHT' then begin
      sText := IntToStr(Item.s.Light);
      sMsg := sub_49ADB8(nPos, sMsg, '<$LIGHT>', sText);
      Exit;
    end;

    if sVariable = '$HORSE' then begin
      sText := IntToStr(Item.s.Horse);
      sMsg := sub_49ADB8(nPos, sMsg, '<$HORSE>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT' then begin
      sText := IntToStr(Item.s.Elements[0]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT1' then begin
      sText := IntToStr(Item.s.Elements[1]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT1>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT2' then begin
      sText := IntToStr(Item.s.Elements[2]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT2>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT3' then begin
      sText := IntToStr(Item.s.Elements[3]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT3>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT4' then begin
      sText := IntToStr(Item.s.Elements[4]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT4>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT5' then begin
      sText := IntToStr(Item.s.Elements[5]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT5>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT6' then begin
      sText := IntToStr(Item.s.Elements[6]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT6>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT7' then begin
      sText := IntToStr(Item.s.Elements[7]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT7>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT8' then begin
      sText := IntToStr(Item.s.Elements[8]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT8>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT9' then begin
      sText := IntToStr(Item.s.Elements[9]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT9>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT10' then begin
      sText := IntToStr(Item.s.Elements[10]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT10>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT11' then begin
      sText := IntToStr(Item.s.Elements[11]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT11>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT12' then begin
      sText := IntToStr(Item.s.Elements[12]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT12>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT13' then begin
      sText := IntToStr(Item.s.Elements[13]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT13>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT14' then begin
      sText := IntToStr(Item.s.Elements[14]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT14>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT15' then begin
      sText := IntToStr(Item.s.Elements[15]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT15>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT16' then begin
      sText := IntToStr(Item.s.Elements[16]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT16>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT17' then begin
      sText := IntToStr(Item.s.Elements[17]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT17>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT18' then begin
      sText := IntToStr(Item.s.Elements[18]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT18>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT19' then begin
      sText := IntToStr(Item.s.Elements[19]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT19>', sText);
      Exit;
    end;

    if sVariable = '$ELEMENT20' then begin
      sText := IntToStr(Item.s.Elements[20]);
      sMsg := sub_49ADB8(nPos, sMsg, '<$ELEMENT20>', sText);
      Exit;
    end;

    if sVariable = '$EXPAND1' then begin
      sText := IntToStr(Item.s.Expand1);
      sMsg := sub_49ADB8(nPos, sMsg, '<$EXPAND1>', sText);
      Exit;
    end;

    if sVariable = '$EXPAND2' then begin
      sText := IntToStr(Item.s.EXPAND2);
      sMsg := sub_49ADB8(nPos, sMsg, '<$EXPAND2>', sText);
      Exit;
    end;

    if sVariable = '$EXPAND3' then begin
      sText := IntToStr(Item.s.EXPAND3);
      sMsg := sub_49ADB8(nPos, sMsg, '<$EXPAND3>', sText);
      Exit;
    end;

    if sVariable = '$UPGRADECOUNT' then begin
      sText := IntToStr(Item.btUpgradeCount);
      sMsg := sub_49ADB8(nPos, sMsg, '<$UPGRADECOUNT>', sText);
      Exit;
    end;
  end;

  Result := False;
end;

function GetLineVariableText(Item:PTClientItem; sMsg:string):string;
var
  nC:Integer;
  nPos:Integer;
  nStartPos:Integer;
  s14, s10:string;
begin
  nC := 0;
  nStartPos := 1;
  while (True) do begin
    s14 := sMsg;
    if (Pos('>', s14) <= 0) or (Pos('$', s14) <= 0) or (nStartPos >= Length(s14)) then
      Break;
    nPos := ArrestVariable(s14, '<', '$', '>', nStartPos, s10);
    if s10 = '' then
      break;
    if not GetVariableText(Item, sMsg, s10, nPos) then begin
      nStartPos := nPos + 2;
    end;
    Inc(nC);
    if nC >= 1001 then
      Break;
  end;

  Result := sMsg;
end;

function GetItemDesc(Item:PTClientItem; sItemName:string):TStringList;
var
  I:Integer;
begin
  Result := nil;
  if g_ItemDescList = nil then
    Exit;
  g_ItemDescList.Lock;
  try
    I := g_ItemDescList.IndexOf(sItemName);
    if I >= 0 then begin
      Result := pTItemDesc(g_ItemDescList.Objects[I]).Desc;

      for I := 0 to Result.Count - 1 do begin
        Result.Strings[I] := GetLineVariableText(Item, Result.Strings[I]);
      end;
    end;
  finally
    g_ItemDescList.UnLock;
  end;
end;

function GetItemDescTop(Item:PTClientItem; sItemName:string):TStringList;
var
  I:Integer;
begin
  Result := nil;
  if g_ItemDescTopList = nil then
    Exit;
  g_ItemDescTopList.Lock;
  try
    I := g_ItemDescTopList.IndexOf(sItemName);
    if I >= 0 then begin
      Result := pTItemDesc(g_ItemDescTopList.Objects[I]).Desc;

      for I := 0 to Result.Count - 1 do begin
        Result.Strings[I] := GetLineVariableText(Item, Result.Strings[I]);
      end;
    end;
  finally
    g_ItemDescTopList.UnLock;
  end;
end;

procedure LoadGodBlessItemList(LoadList:TStringList);
var
  I, II, nPos:Integer;
  sLineText, sDesc, sGodBlessItem:string;
  GodBlessItem:pTItemDesc;
  StringList:TStringList;
  btColor:Byte;
begin
  g_GodBlessItemList.Lock;
  try
    for I := 0 to g_GodBlessItemList.Count - 1 do begin
      GodBlessItem := pTItemDesc(g_GodBlessItemList.Objects[I]);
      GodBlessItem.Desc.Free;
      Dispose(GodBlessItem);
    end;
    g_GodBlessItemList.Clear;
  finally
    g_GodBlessItemList.UnLock;
  end;

  g_GodBlessItemList.Lock;
  try
    StringList := TStringList.Create;
    for I := 0 to LoadList.Count - 1 do begin
      sLineText := Trim(LoadList.Strings[I]);
      if (sLineText <> '') and (sLineText[1] <> ';') then begin
        nPos := Pos('=', sLineText);
        if nPos > 0 then begin
          New(GodBlessItem);
          GodBlessItem.Name := Trim(Copy(sLineText, 1, nPos - 1));
          sDesc := Trim(Copy(sLineText, nPos + 1, Length(sLineText)));
          GodBlessItem.Desc := TStringList.Create;
          StringList.Clear;

          ExtractStrings(['\'], [], PChar(sDesc), StringList);
          for II := 0 to StringList.Count - 1 do begin
            sGodBlessItem := StringList.Strings[II];
            if sGodBlessItem <> '' then begin
              btColor := 255;
              nPos := Pos('/', sGodBlessItem);
              if nPos > 0 then begin
                btColor := StrToIntDef(Trim(Copy(sGodBlessItem, 1, nPos - 1)), btColor);
                sGodBlessItem := Trim(Copy(sGodBlessItem, nPos + 1, Length(sGodBlessItem)));
              end;
              GodBlessItem.Desc.AddObject(sGodBlessItem, TObject(GetRGB(btColor)));
            end;
          end;
          g_GodBlessItemList.AddObject(GodBlessItem.Name, TObject(GodBlessItem));
        end;
      end;
    end;
    StringList.Free;
  finally
    g_GodBlessItemList.UnLock;
  end;
end;

procedure UnLoadGodBlessItemList;
var
  I:Integer;
  GodBlessItem:pTItemDesc;
begin
  for I := 0 to g_GodBlessItemList.Count - 1 do begin
    GodBlessItem := pTItemDesc(g_GodBlessItemList.Objects[I]);
    GodBlessItem.Desc.Free;
    Dispose(GodBlessItem);
  end;

  FreeAndNil(g_GodBlessItemList);
end;

function GetGodBlessItem(sItemName:string):TStringList;
var
  I:Integer;
begin
  Result := nil;
  if g_GodBlessItemList = nil then
    Exit;
  g_GodBlessItemList.Lock;
  try
    I := g_GodBlessItemList.IndexOf(sItemName);
    if I >= 0 then begin
      Result := pTItemDesc(g_GodBlessItemList.Objects[I]).Desc;
    end;
  finally
    g_GodBlessItemList.UnLock;
  end;
end;

procedure LoadFengHaoItemList(LoadList:TStringList);
var
  I, II, nPos:Integer;
  sLineText, sDesc, sFengHaoItem:string;
  FengHaoItem:pTItemDesc;
  StringList:TStringList;
  btColor:Byte;
begin
  g_FengHaoItemList.Lock;
  try
    for I := 0 to g_FengHaoItemList.Count - 1 do begin
      FengHaoItem := pTItemDesc(g_FengHaoItemList.Objects[I]);
      FengHaoItem.Desc.Free;
      Dispose(FengHaoItem);
    end;
    g_FengHaoItemList.Clear;
  finally
    g_FengHaoItemList.UnLock;
  end;

  g_FengHaoItemList.Lock;
  try
    StringList := TStringList.Create;
    for I := 0 to LoadList.Count - 1 do begin
      sLineText := Trim(LoadList.Strings[I]);
      if (sLineText <> '') and (sLineText[1] <> ';') then begin
        nPos := Pos('=', sLineText);
        if nPos > 0 then begin
          New(FengHaoItem);
          FengHaoItem.Name := Trim(Copy(sLineText, 1, nPos - 1));
          sDesc := Trim(Copy(sLineText, nPos + 1, Length(sLineText)));
          FengHaoItem.Desc := TStringList.Create;
          StringList.Clear;

          ExtractStrings(['\'], [], PChar(sDesc), StringList);
          for II := 0 to StringList.Count - 1 do begin
            sFengHaoItem := StringList.Strings[II];
            if sFengHaoItem <> '' then begin
              btColor := 255;
              nPos := Pos('/', sFengHaoItem);
              if nPos > 0 then begin
                btColor := StrToIntDef(Trim(Copy(sFengHaoItem, 1, nPos - 1)), btColor);
                sFengHaoItem := Trim(Copy(sFengHaoItem, nPos + 1, Length(sFengHaoItem)));
              end;
              FengHaoItem.Desc.AddObject(sFengHaoItem, TObject(GetRGB(btColor)));
            end;
          end;
          g_FengHaoItemList.AddObject(FengHaoItem.Name, TObject(FengHaoItem));
        end;
      end;
    end;
    StringList.Free;
  finally
    g_FengHaoItemList.UnLock;
  end;
end;

procedure UnLoadFengHaoItemList;
var
  I:Integer;
  FengHaoItem:pTItemDesc;
begin
  for I := 0 to g_FengHaoItemList.Count - 1 do begin
    FengHaoItem := pTItemDesc(g_FengHaoItemList.Objects[I]);
    FengHaoItem.Desc.Free;
    Dispose(FengHaoItem);
  end;

  FreeAndNil(g_FengHaoItemList);
end;

function GetFengHaoItem(sItemName:string):TStringList;
var
  I:Integer;
begin
  Result := nil;
  if g_FengHaoItemList = nil then
    Exit;
  g_FengHaoItemList.Lock;
  try
    I := g_FengHaoItemList.IndexOf(sItemName);
    if I >= 0 then begin
      Result := pTItemDesc(g_FengHaoItemList.Objects[I]).Desc;
    end;
  finally
    g_FengHaoItemList.UnLock;
  end;
end;
{------------------------------------------------------------------------------}

procedure LoadTzItemDescList(LoadList:TStringList);
var
  I, II, III, nPos:Integer;
  sLineText, sNameData, sName, sItemName, sItemCount, sItemDesc, sSex:string;
  ItemDesc:pTTzItemDesc;
  StringList:TStringList;
  ItemNameList:TList;
  ItemNames:TStringList;
  nItemCount:Integer;
  btColor:Byte;
  nSex:Integer;
begin
  g_TzItemDescList.Lock;
  try
    for I := 0 to g_TzItemDescList.Count - 1 do begin
      ItemDesc := pTTzItemDesc(g_TzItemDescList.Objects[I]);
      for II := 0 to ItemDesc.ItemList.Count - 1 do begin
        TStringList(ItemDesc.ItemList.Items[II]).Free;
      end;
      ItemDesc.ItemList.Free;
      ItemDesc.ItemDesc.Free;
      Dispose(ItemDesc);
    end;
    g_TzItemDescList.Clear;
  finally
    g_TzItemDescList.UnLock;
  end;

  g_TzItemDescList.Lock;
  try
    StringList := TStringList.Create;
    ItemNameList := TList.Create;

    for I := 0 to LoadList.Count - 1 do begin
      sLineText := Trim(LoadList.Strings[I]);
      if (sLineText <> '') and (sLineText[1] <> ';') then begin
        StringList.Clear;
        ItemNameList.Clear;
        nPos := Pos(':', sLineText);
        if nPos > 0 then begin
          sNameData := Trim(Copy(sLineText, 1, nPos - 1));
          sItemDesc := Trim(Copy(sLineText, nPos + 1, Length(sLineText)));
          sNameData := GetValidStr3_Ex(sNameData, sName, '|');
          sNameData := GetValidStr3_Ex(sNameData, sItemCount, '|');

          nItemCount := StrToIntDef(sItemCount, 0);
          if (sName <> '') and (nItemCount > 0) and (sItemDesc <> '') then begin

            if nItemCount > 0 then begin
              for II := 0 to nItemCount - 1 do begin
                if sNameData = '' then
                  break;
                sNameData := GetValidStr3_Ex(sNameData, sItemName, '|');
                if sItemName <> '' then begin
                  btColor := 150;
                  nPos := Pos('/', sItemName);
                  if nPos > 0 then begin
                    btColor := StrToIntDef(Trim(Copy(sItemName, 1, nPos - 1)), btColor);
                    sItemName := Trim(Copy(sItemName, nPos + 1, Length(sItemName)));
                  end;
                  ItemNames := TStringList.Create;
                  if (sItemName[1] = '(') and (sItemName[Length(sItemName)] = ')') then begin
                    sItemName := Copy(sItemName, 2, Length(sItemName) - 2);
                    ExtractStrings([','], [], PChar(Trim(sItemName)), ItemNames);
                    for III := ItemNames.Count - 1 downto 0 do begin
                      ItemNames.Strings[III] := Trim(ItemNames.Strings[III]);
                      if ItemNames.Strings[III] = '' then begin
                        ItemNames.Delete(III);
                      end;
                    end;
                    for III := 0 to ItemNames.Count - 1 do begin
                      ItemNames.Objects[III] := TObject(GetRGB(btColor));
                    end;
                  end
                  else begin
                    ItemNames.AddObject(sItemName, TObject(GetRGB(btColor)));
                  end;
                  ItemNameList.Add(ItemNames);
                end;
              end;
            end;
            sName := Trim(sName);
            if nItemCount = ItemNameList.Count then begin
              nSex := 99;
              btColor := 250;
              nPos := Pos('/', sName); // 名称颜色
              if nPos > 0 then begin
                btColor := StrToIntDef(Trim(Copy(sName, 1, nPos - 1)), btColor);
                sName := Trim(Copy(sName, nPos + 1, Length(sName)));
              end;

              nPos := Pos('=', sName); // 套装性别
              if nPos > 0 then begin
                sSex := Trim(Copy(sName, nPos + 1, Length(sName) - nPos));
                sName := Copy(sName, 1, nPos - 1);
                nSex := StrToIntDef(sSex, 99);
                if not (nSex in [0, 1]) then
                  nSex := 99;
              end;

              // DebugOutStr(sName + ' nSex:' + IntToStr(nSex));
              New(ItemDesc);
              ItemDesc.ItemName := sName;
              ItemDesc.Sex := nSex;
              ItemDesc.NameColor := GetRGB(btColor);
              ItemDesc.ItemCount := nItemCount;
              ItemDesc.ItemList := TList.Create;
              ItemDesc.ItemDesc := TStringList.Create;
              for II := 0 to ItemNameList.Count - 1 do begin
                ItemDesc.ItemList.Add(ItemNameList.Items[II]);
              end;
              // ItemDesc.ItemList.AddStrings(ItemNameList);
              ExtractStrings(['\'], [], PChar(sItemDesc), StringList);
              for II := 0 to StringList.Count - 1 do begin
                sItemDesc := StringList.Strings[II];
                if sItemDesc <> '' then begin
                  btColor := 150;
                  nPos := Pos('/', sItemDesc);
                  if nPos > 0 then begin
                    btColor := StrToIntDef(Trim(Copy(sItemDesc, 1, nPos - 1)), btColor);
                    sItemDesc := Trim(Copy(sItemDesc, nPos + 1, Length(sItemDesc)));
                  end;
                  ItemDesc.ItemDesc.AddObject(sItemDesc, TObject(GetRGB(btColor)));
                end;
              end;
              g_TzItemDescList.AddObject(ItemDesc.ItemName, TObject(ItemDesc));
            end;
          end;
        end;
      end;
    end;
    StringList.Free;
    ItemNameList.Free;
  finally
    g_TzItemDescList.UnLock;
  end;
end;

procedure UnLoadTzItemDescList;
var
  I, II:Integer;
  ItemDesc:pTTzItemDesc;
begin
  for I := 0 to g_TzItemDescList.Count - 1 do begin
    ItemDesc := pTTzItemDesc(g_TzItemDescList.Objects[I]);
    for II := 0 to ItemDesc.ItemList.Count - 1 do begin
      TStringList(ItemDesc.ItemList.Items[II]).Free;
    end;
    ItemDesc.ItemList.Free;
    ItemDesc.ItemDesc.Free;
    Dispose(ItemDesc);
  end;
  FreeAndNil(g_TzItemDescList);
end;

function GetTzItemDesc(btUser, btSex:Byte; sItemName:string):TList;

  function FindUseItems(User:Byte; ItemName:string):Boolean;
  var
    I:Integer;
    UseItems:pTUseItems;
  begin
    Result := False;
    case User of
      1:
        UseItems := @g_HeroUseItems;
      2:
        UseItems := @g_UserState1.UseItems;
      else
        UseItems := @g_UseItems;
    end;
    if UseItems <> nil then begin
      for I := Low(TUseItems) to High(TUseItems) do begin
        if UseItems[I].S.Name <> '' then begin
          if CompareText(UseItems[I].S.Name, ItemName) = 0 then begin
            Result := True;
            break;
          end;
        end;
      end;
    end;
  end;

  function GetUseItems(User:Byte):pTUseItems;
  begin
    case User of
      1:
        Result := @g_HeroUseItems;
      2:
        Result := @g_UserState1.UseItems;
      else
        Result := @g_UseItems;
    end;
  end;

var
  I, II, III, IIII, nCount:Integer;
  ItemDesc:pTTzItemDesc;
  UseItems:pTUseItems;
  boFind:Boolean;
  ItemNameList:TStringList;
  ItemDescList:TList;
  ShowTzItemDescList:TList;
  LineText:TList;
  TextList:TStringList;
  ItemSelecteds:array[Low(TUseItems)..High(TUseItems)] of Boolean;
begin
  Result := nil;
  if not (btSex in [0, 1]) then
    btSex := 0;
  g_TzItemDescList.Lock;
  try
    ItemDescList := TList.Create;
    UseItems := GetUseItems(btUser);
    if UseItems <> nil then begin
      for I := 0 to g_TzItemDescList.Count - 1 do begin
        ItemDesc := pTTzItemDesc(g_TzItemDescList.Objects[I]);
        if (ItemDesc.Sex = btSex) or (ItemDesc.Sex = 99) then begin
          // DebugOutStr(ItemDesc.ItemName + ' 1 nSex:' + IntToStr(ItemDesc.Sex));
          for II := 0 to ItemDesc.ItemList.Count - 1 do begin
            ItemNameList := TStringList(ItemDesc.ItemList.Items[II]);
            boFind := False;
            for III := 0 to ItemNameList.Count - 1 do begin
              if CompareText(ItemNameList.Strings[III], sItemName) = 0 then begin
                ItemDescList.Add(ItemDesc);
                boFind := True;
                // DebugOutStr(ItemDesc.ItemName + ' 2 nSex:' + IntToStr(ItemDesc.Sex));
                break;
              end;
            end;
            if boFind then
              break;
          end;
        end;
      end;

      if (ItemDescList.Count > 0) then begin
        Result := TList.Create;

        for I := 0 to ItemDescList.Count - 1 do begin
          ItemDesc := ItemDescList.Items[I];

          for II := 0 to Length(ItemSelecteds) - 1 do
            ItemSelecteds[II] := False;

          nCount := 0;
          ShowTzItemDescList := TList.Create;
          Result.Add(ShowTzItemDescList);

          for II := 0 to ItemDesc.ItemList.Count - 1 do begin
            ItemNameList := TStringList(ItemDesc.ItemList.Items[II]);
            LineText := TList.Create;
            ShowTzItemDescList.Add(LineText);
            TextList := nil;
            for III := 0 to ItemNameList.Count - 1 do begin
              boFind := False;
              for IIII := Low(TUseItems) to High(TUseItems) do begin
                if (UseItems[IIII].S.DBName <> '') and (not ItemSelecteds[IIII]) then begin
                  if CompareText(UseItems[IIII].S.Name, ItemNameList.Strings[III]) = 0 then begin
                    ItemSelecteds[IIII] := True;
                    if TextList = nil then begin
                      TextList := TStringList.Create;
                      TextList.AddObject(ItemNameList.Strings[III], ItemNameList.Objects[III]);
                      LineText.Add(TextList);
                    end
                    else begin
                      TextList.AddObject(' or ', TObject(clWhite));
                      LineText.Add(TextList);

                      TextList := TStringList.Create;
                      TextList.AddObject(ItemNameList.Strings[III], ItemNameList.Objects[III]);
                      LineText.Add(TextList);
                    end;
                    Inc(nCount);
                    boFind := True;
                    break;
                  end;
                end;
              end; // for IIII := Low(TUseItems) to High(TUseItems) do begin
              // if boFind then break;
              if not boFind then begin
                if TextList = nil then begin
                  TextList := TStringList.Create;
                  TextList.AddObject(ItemNameList.Strings[III], TObject(clRed));
                  LineText.Add(TextList);
                end
                else begin
                  TextList.AddObject(' or ', TObject(clWhite));
                  LineText.Add(TextList);

                  TextList := TStringList.Create;
                  TextList.AddObject(ItemNameList.Strings[III], TObject(clRed));
                  LineText.Add(TextList);
                end;
              end;
            end; // for III := 0 to ItemNameList.Count - 1 do begin
            {if not boFind then begin
              if ItemNameList.Count > 1 then begin
                for III := 0 to ItemNameList.Count - 1 do begin
                  if CompareText(ItemNameList.Strings[III], sItemName) = 0 then begin
                    ShowTzItemDescList.AddObject(ItemNameList.Strings[III], TObject(clRed));
                    boFind := True;
                    break;
                  end;
                end;
                if not boFind then begin
                  ShowTzItemDescList.AddObject(ItemNameList.Strings[btSex], TObject(clRed));
                end;
              end else begin
                ShowTzItemDescList.AddObject(ItemNameList.Strings[0], TObject(clRed));
              end;
            end;}
          end; // for II := 0 to ItemDesc.ItemList.Count - 1 do begin

          LineText := TList.Create;
          ShowTzItemDescList.Insert(0, LineText);
          TextList := TStringList.Create;
          TextList.AddObject(Format('%s(%d/%d)', [ItemDesc.ItemName, nCount, ItemDesc.ItemList.Count]), TObject(ItemDesc.NameColor));

          // ShowTzItemDescList.InsertObject(0, Format('%s(%d/%d)', [ItemDesc.ItemName, nCount, ItemDesc.ItemList.Count]), TObject(ItemDesc.NameColor));
           // ShowTzItemDescList.AddStrings(ItemDesc.ItemDesc);
          for II := 0 to ItemDesc.ItemDesc.Count - 1 do begin
            LineText := TList.Create;
            ShowTzItemDescList.Add(LineText);
            TextList := TStringList.Create;
            TextList.AddObject(ItemDesc.ItemDesc.Strings[II], ItemDesc.ItemDesc.Objects[II]);
            LineText.Add(TextList);
          end;
        end; // for I := 0 to ItemDescList.Count - 1 do begin
      end;
    end;
    ItemDescList.Free;
  finally
    g_TzItemDescList.UnLock;
  end;
end;

{---------------------------------------------------------------}

constructor TMapDesc.Create;
begin
  FStringList := THashedStringList.Create;
end;

destructor TMapDesc.Destroy;
var
  I, II:Integer;
  MapDescList:pTMapDescList;
begin
  for I := 0 to FStringList.Count - 1 do begin
    MapDescList := pTMapDescList(FStringList.Objects[I]);
    for II := 0 to MapDescList.Large.Count - 1 do begin
      Dispose(pTMapDescInfo(MapDescList.Large.Items[II]));
    end;
    for II := 0 to MapDescList.Small.Count - 1 do begin
      Dispose(pTMapDescInfo(MapDescList.Small.Items[II]));
    end;
    MapDescList.Large.Free;
    MapDescList.Small.Free;
    Dispose(MapDescList);
  end;
  FStringList.Free;
  inherited;
end;

procedure TMapDesc.LoadFromFile(const FileName:string);
var
  nIndex:Integer;
  LoadList:TStringList;
  sFileName, sLineText, sMapName, sX, sY, sDescName, sColor, sBigMap:string;
  I, II:Integer;
  MapDescInfo:pTMapDescInfo;
  MapDescList:pTMapDescList;
begin
  for I := 0 to FStringList.Count - 1 do begin
    MapDescList := pTMapDescList(FStringList.Objects[I]);
    for II := 0 to MapDescList.Large.Count - 1 do begin
      Dispose(pTMapDescInfo(MapDescList.Large.Items[II]));
    end;
    for II := 0 to MapDescList.Small.Count - 1 do begin
      Dispose(pTMapDescInfo(MapDescList.Small.Items[II]));
    end;
    MapDescList.Large.Free;
    MapDescList.Small.Free;
    Dispose(MapDescList);
  end;
  FStringList.Clear;

  LoadList := TStringList.Create;

  sFileName := g_sSelfResourcePath + FileName;

  if FileExists(sFileName) then begin
    try
      LoadList.LoadFromFile(sFileName);
    except
      LoadList.Clear;
    end;
  end
  else begin
    if FileExists(g_sSelfFilePath + FileName) then begin
      try
        LoadList.LoadFromFile(g_sSelfFilePath + FileName);
      except
        LoadList.Clear;
      end;
    end;
  end;

  for I := 0 to LoadList.Count - 1 do begin
    sLineText := Trim(LoadList.Strings[I]);
    if sLineText = '' then
      Continue;
    if (sLineText <> '') and (sLineText[1] = ';') then
      Continue;
    sLineText := GetValidStr3(sLineText, sMapName, [',', #9]);
    sLineText := GetValidStr3(sLineText, sX, [',', #9]);
    sLineText := GetValidStr3(sLineText, sY, [',', #9]);
    sLineText := GetValidStr3(sLineText, sDescName, [',', #9]);
    sLineText := GetValidStr3(sLineText, sColor, [',', #9]);
    sLineText := GetValidStr3(sLineText, sBigMap, [',', #9]);
    if (sMapName <> '') and (sDescName <> '') and (sBigMap <> '') then begin
      nIndex := FStringList.IndexOf(sMapName);
      if nIndex < 0 then begin
        New(MapDescList);
        MapDescList.Large := TList.Create;
        MapDescList.Small := TList.Create;
        FStringList.AddObject(sMapName, TObject(MapDescList));
        nIndex := FStringList.IndexOf(sMapName);
      end;
      if nIndex >= 0 then begin
        New(MapDescInfo);
        MapDescInfo.sMapName := sMapName;
        MapDescInfo.sDescName := sDescName;
        MapDescInfo.nX := StrToIntDef(sX, -1);
        MapDescInfo.nY := StrToIntDef(sY, -1);
        MapDescInfo.FColor := TColor(StrToIntDef(sColor, 0));
        MapDescInfo.boBigMap := sBigMap = '0';
        MapDescList := pTMapDescList(FStringList.Objects[nIndex]);
        if MapDescInfo.boBigMap then begin
          MapDescList.Large.Add(MapDescInfo);
        end
        else begin
          MapDescList.Small.Add(MapDescInfo);
        end;
      end;
    end;
  end;
  LoadList.Free;
end;

function TMapDesc.Get(const MapName:string; const BigMap:Boolean; var DescList:TList):Boolean;
var
  nIndex:Integer;
begin
  Result := False;
  DescList := nil;
  nIndex := FStringList.IndexOf(MapName);
  if nIndex >= 0 then begin
    if BigMap then begin
      DescList := pTMapDescList(FStringList.Objects[nIndex]).Large;
    end
    else begin
      DescList := pTMapDescList(FStringList.Objects[nIndex]).Small;
    end;
    Result := True;
  end;
end;

{ THumEffectList }

constructor THumEffectList.Create;
begin
  FBaseIndex := 1000;
  FMaxPicIndex := 25;
  FCustomUnitIndex := 10;
end;

procedure THumEffectList.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;

  SetLength(ImagesArr, 0);
end;

function THumEffectList.GetWHumEffectBrightImg(Effect, Sex, Frame:Integer; IsWeapon:Boolean; var Ax, Ay:Integer; NoSex:Boolean):TTexture;
var
  nHumWinOffset:Integer;
  nUnit:Integer;
  sFileName:string;
  MaxOffset:Integer;
begin
  if not IsWeapon then begin
    nUnit := (Effect - FBaseIndex) div FMaxPicIndex; // 单元编号
    nUnit := nUnit + 1; // 排除第一个文件

    if not NoSex then begin
      nHumWinOffset := (Effect - FBaseIndex) * HUMANFRAME * 2 + Sex * HUMANFRAME;
      MaxOffset := FMaxPicIndex * HUMANFRAME * 2;
    end else begin
      nHumWinOffset := (Effect - FBaseIndex) * HUMANFRAME;
      MaxOffset := FMaxPicIndex * HUMANFRAME;
    end;

    if nHumWinOffset >= MaxOffset then
      nHumWinOffset := nHumWinOffset mod MaxOffset;

    //nHumWinOffset := nHumWinOffset + HUMANFRAME * Sex + Frame
    //nHumWinOffset := nHumWinOffset + (Dir * 8) + Frame;
  end else begin
    if (Effect >= 1000) and (Effect <= 1006) then begin
      nUnit := 1;
    end else if (Effect >= 1007) and (Effect <= 1015) then begin
      nUnit := 2;
    end else begin
      nUnit := 0; //HZQ 20230524 假设默认为0，未经过验证
    end;

    nHumWinOffset := 0; //HZQ 20230524 猜测可能是要为零值，只能等有问题再来排查
    case Effect of
      1000:nHumWinOffset := 0;
      1001:nHumWinOffset := 1200;
      1002:nHumWinOffset := 6000;
      1003:nHumWinOffset := 8400;
      1004:nHumWinOffset := 9600;
      1005:nHumWinOffset := 10800;
      1006:nHumWinOffset := 13200;
      1007:nHumWinOffset := 3600;
      1008:nHumWinOffset := 4800;
      1009:nHumWinOffset := 6000;
      1010:nHumWinOffset := 8400;
      1011:nHumWinOffset := 9600;
      1012:nHumWinOffset := 10800;
      1013:nHumWinOffset := 14400;
      1014:nHumWinOffset := 15600;
      1015:nHumWinOffset := 16800;
    end;
    //nHumWinOffset := nHumWinOffset + HUMANFRAME * Sex + Frame;
  end;
  nHumWinOffset := nHumWinOffset + Frame;
  //HUMANFRAME * m_btSex + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);

  if Length(ImagesArr) <= nUnit then begin
    SetLength(ImagesArr, nUnit + 1);
  end;

  if ImagesArr[nUnit] = nil then begin
    //if nUnit < FCustomUnitIndex then begin
    if nUnit = 0 then begin
      sFileName := g_sSelfFilePath + HUMWINGIMAGESFILE
    end else begin
      //nHumWinOffset := 0;
      sFileName := g_sSelfFilePath + Format(HUMWINGIMAGESFILEEX, [nUnit + 1]);
    end;
    //end else begin
    //  if Length(g_ResourcesDir) > 0 then
    //    sFileName := g_sSelfResourcePath + Format(HumEffectImageDir, [nUnit])
    //  else
    //    sFileName := g_sSelfFilePath + Format(HumEffectImageDir, [nUnit]);
    //end;

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + FileName;
    ImagesArr[nUnit].Initialize;
  end;

  Result := ImagesArr[nUnit].GetCachedBrightImage(nHumWinOffset, Ax, Ay);
end;

function THumEffectList.GetWHumEffectGrayImg(Effect, Sex, Frame:Integer; IsWeapon:Boolean; var Ax, Ay:Integer; NoSex:Boolean):TTexture;
var
  nHumWinOffset:Integer;
  nUnit:Integer;
  sFileName:string;
  MaxOffset:Integer;
begin
  if not IsWeapon then begin
    nUnit := (Effect - FBaseIndex) div FMaxPicIndex; // 单元编号
    nUnit := nUnit + 1; // 排除第一个文件

    if not NoSex then begin
      nHumWinOffset := (Effect - FBaseIndex) * HUMANFRAME * 2 + Sex * HUMANFRAME;
      MaxOffset := FMaxPicIndex * HUMANFRAME * 2;
    end else begin
      nHumWinOffset := (Effect - FBaseIndex) * HUMANFRAME;
      MaxOffset := FMaxPicIndex * HUMANFRAME;
    end;

    if nHumWinOffset >= MaxOffset then
      nHumWinOffset := nHumWinOffset mod MaxOffset;

    //nHumWinOffset := nHumWinOffset + HUMANFRAME * Sex + Frame
    //nHumWinOffset := nHumWinOffset + (Dir * 8) + Frame;
  end else begin
    if (Effect >= 1000) and (Effect <= 1006) then begin
      nUnit := 1;
    end else if (Effect >= 1007) and (Effect <= 1015) then begin
      nUnit := 2;
    end else begin
      nUnit := 0; //HZQ 20230524 补上一个默认值
    end;

    nHumWinOffset := 0; //HZQ 20230524 猜测可能是要为零值，只能等有问题再来排查
    case Effect of
      1000:nHumWinOffset := 0;
      1001:nHumWinOffset := 1200;
      1002:nHumWinOffset := 6000;
      1003:nHumWinOffset := 8400;
      1004:nHumWinOffset := 9600;
      1005:nHumWinOffset := 10800;
      1006:nHumWinOffset := 13200;
      1007:nHumWinOffset := 3600;
      1008:nHumWinOffset := 4800;
      1009:nHumWinOffset := 6000;
      1010:nHumWinOffset := 8400;
      1011:nHumWinOffset := 9600;
      1012:nHumWinOffset := 10800;
      1013:nHumWinOffset := 14400;
      1014:nHumWinOffset := 15600;
      1015:nHumWinOffset := 16800;
    end;
    //nHumWinOffset := nHumWinOffset + HUMANFRAME * Sex + Frame;
  end;
  nHumWinOffset := nHumWinOffset + Frame;
  //HUMANFRAME * m_btSex + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);

  if Length(ImagesArr) <= nUnit then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    //if nUnit < FCustomUnitIndex then begin
    if nUnit = 0 then begin
      sFileName := g_sSelfFilePath + HUMWINGIMAGESFILE
    end else begin
      //nHumWinOffset := 0;
      sFileName := g_sSelfFilePath + Format(HUMWINGIMAGESFILEEX, [nUnit + 1]);
    end;
    //end else begin
    //  if Length(g_ResourcesDir) > 0 then
    //    sFileName := g_sSelfResourcePath + Format(HumEffectImageDir, [nUnit])
     // else
     //   sFileName := g_sSelfFilePath + Format(HumEffectImageDir, [nUnit]);
    //end;

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + FileName;
    ImagesArr[nUnit].Initialize;
  end;

  Result := ImagesArr[nUnit].GetCachedGrayImage(nHumWinOffset, Ax, Ay);
end;

function THumEffectList.GetWHumEffectImg(Effect, Sex, Frame:Integer; IsWeapon:Boolean; var Ax, Ay:Integer; NoSex:Boolean):TTexture;
var
  nHumWinOffset:Integer;
  nUnit:Integer;
  sFileName:string;
  MaxOffset:Integer;
begin
  if not IsWeapon then begin
    nUnit := (Effect - FBaseIndex) div FMaxPicIndex; // 单元编号
    nUnit := nUnit + 1; // 排除第一个文件

    if not NoSex then begin
      nHumWinOffset := (Effect - FBaseIndex) * HUMANFRAME * 2 + Sex * HUMANFRAME;
      MaxOffset := FMaxPicIndex * HUMANFRAME * 2;
    end else begin
      nHumWinOffset := (Effect - FBaseIndex) * HUMANFRAME;
      MaxOffset := FMaxPicIndex * HUMANFRAME;
    end;

    if nHumWinOffset >= MaxOffset then
      nHumWinOffset := nHumWinOffset mod MaxOffset;

    //nHumWinOffset := nHumWinOffset + HUMANFRAME * Sex + Frame
    //nHumWinOffset := nHumWinOffset + Frame;
    //nHumWinOffset := nHumWinOffset + (Dir * 8) + Frame;
  end else begin
    if (Effect >= 1000) and (Effect <= 1006) then begin
      nUnit := 1;
    end else if (Effect >= 1007) and (Effect <= 1015) then begin
      nUnit := 2;
    end else begin
      nUnit := 0; //HZQ 20230524 补上一个默认值
    end;

    nHumWinOffset := 0; //HZQ 20230524添加一个默认值
    case Effect of
      1000:nHumWinOffset := 0;
      1001:nHumWinOffset := 1200;
      1002:nHumWinOffset := 6000;
      1003:nHumWinOffset := 8400;
      1004:nHumWinOffset := 9600;
      1005:nHumWinOffset := 10800;
      1006:nHumWinOffset := 13200;
      1007:nHumWinOffset := 3600;
      1008:nHumWinOffset := 4800;
      1009:nHumWinOffset := 6000;
      1010:nHumWinOffset := 8400;
      1011:nHumWinOffset := 9600;
      1012:nHumWinOffset := 10800;
      1013:nHumWinOffset := 14400;
      1014:nHumWinOffset := 15600;
      1015:nHumWinOffset := 16800;
    end;
    //nHumWinOffset := nHumWinOffset + HUMANFRAME * Sex + Frame;
    //nHumWinOffset := nHumWinOffset + Frame;
  end;

  // 女号武器特效效果不对 chongchong 2013-08-10
  // nHumWinOffset := nHumWinOffset + Frame;
  if IsWeapon then
    nHumWinOffset := nHumWinOffset + HUMANFRAME * Sex + Frame
  else
    nHumWinOffset := nHumWinOffset + Frame;
  //--------------------------------------------------------------------
  //HUMANFRAME * m_btSex + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);

  if Length(ImagesArr) <= nUnit then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + HUMWINGIMAGESFILE
    else begin
      //nHumWinOffset := 0;
      sFileName := g_sSelfFilePath + Format(HUMWINGIMAGESFILEEX, [nUnit + 1]);
    end;

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + FileName;
    ImagesArr[nUnit].Initialize;
  end;

  Result := ImagesArr[nUnit].GetCachedImage(nHumWinOffset, Ax, Ay);
end;

function THumEffectList.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := Index;
  if nUnit < 0 then
    nUnit := 0;

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    //if nUnit < FCustomUnitIndex then
    begin
      if nUnit = 0 then
        sFileName := g_sSelfFilePath + HUMWINGIMAGESFILE
      else
        sFileName := g_sSelfFilePath + Format(HUMWINGIMAGESFILEEX, [nUnit + 1]);
    end {
    else
    begin
      if Length(g_ResourcesDir) > 0 then
        sFileName := g_sSelfResourcePath + Format(HumEffectImageDir, [nUnit])
      else
        sFileName := g_sSelfFilePath + Format(HumEffectImageDir, [nUnit]);
    end}
    ;
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

procedure THumEffectList.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    //if nUnit < FCustomUnitIndex then
    begin
      if nUnit = 0 then
        sFileName := g_sSelfFilePath + HUMWINGIMAGESFILE
      else
        sFileName := g_sSelfFilePath + Format(HUMWINGIMAGESFILEEX, [nUnit + 1]);
    end {
    else
    begin
      if Length(g_ResourcesDir) > 0 then
        sFileName := g_sSelfResourcePath + Format(HumEffectImageDir, [nUnit])
      else
        sFileName := g_sSelfFilePath + Format(HumEffectImageDir, [nUnit]);
    end}
    ;

    // if FileExists(sFileName) then begin
    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + sFileName;
    ImagesArr[nUnit].Initialize;
  end;
end;

{ TWeaponEffectList }

constructor TWeaponEffectList.Create;
begin
  FBaseIndex := 1025;
  FMaxPicIndex := 25;
end;

procedure TWeaponEffectList.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
end;

function TWeaponEffectList.GetWWeaponEffectBrightImg(Effect, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
var
  nWeaponEffect:Integer;
  nUnit:Integer;
  sFileName:string;
  MaxOffset:Integer;
begin
  nUnit := (Effect - FBaseIndex) div FMaxPicIndex; // 单元编号
  nWeaponEffect := (Effect - FBaseIndex) * HUMANFRAME * 2 + Sex * HUMANFRAME;
  MaxOffset := FMaxPicIndex * HUMANFRAME * 2;
  if nWeaponEffect >= MaxOffset then
    nWeaponEffect := nWeaponEffect mod MaxOffset;

  nWeaponEffect := nWeaponEffect + Frame;

  if Length(ImagesArr) <= nUnit then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + WEAPONEFFECTIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(WEAPONEFFECTIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    // ImagesArr[nUnit].FileName := g_sSelfFilePath + FileName;
    ImagesArr[nUnit].Initialize;
  end;

  Result := ImagesArr[nUnit].GetCachedBrightImage(nWeaponEffect, Ax, Ay);
end;

function TWeaponEffectList.GetWWeaponEffectGrayImg(Effect, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
var
  nWeaponEffect:Integer;
  nUnit:Integer;
  sFileName:string;
  MaxOffset:Integer;
begin
  nUnit := (Effect - FBaseIndex) div FMaxPicIndex; // 单元编号
  nWeaponEffect := (Effect - FBaseIndex) * HUMANFRAME * 2 + Sex * HUMANFRAME;
  MaxOffset := FMaxPicIndex * HUMANFRAME * 2;
  if nWeaponEffect >= MaxOffset then
    nWeaponEffect := nWeaponEffect mod MaxOffset;

  nWeaponEffect := nWeaponEffect + Frame;

  if Length(ImagesArr) <= nUnit then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + WEAPONEFFECTIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(WEAPONEFFECTIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;

  Result := ImagesArr[nUnit].GetCachedGrayImage(nWeaponEffect, Ax, Ay);
end;

function TWeaponEffectList.GetWWeaponEffectImg(Effect, Sex, Frame:Integer; var Ax, Ay:Integer):TTexture;
var
  nWeaponEffect:Integer;
  nUnit:Integer;
  sFileName:string;
  MaxOffset:Integer;
begin
  nUnit := (Effect - FBaseIndex) div FMaxPicIndex; // 单元编号
  nWeaponEffect := (Effect - FBaseIndex) * HUMANFRAME * 2 + Sex * HUMANFRAME;
  MaxOffset := FMaxPicIndex * HUMANFRAME * 2;
  if nWeaponEffect >= MaxOffset then
    nWeaponEffect := nWeaponEffect mod MaxOffset;

  nWeaponEffect := nWeaponEffect + Frame;

  if Length(ImagesArr) <= nUnit then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + WEAPONEFFECTIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(WEAPONEFFECTIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;

  Result := ImagesArr[nUnit].GetCachedImage(nWeaponEffect, Ax, Ay);
end;

function TWeaponEffectList.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := Index;
  if nUnit < 0 then
    nUnit := 0;

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + WEAPONEFFECTIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(WEAPONEFFECTIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

procedure TWeaponEffectList.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + WEAPONEFFECTIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(WEAPONEFFECTIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;
end;

{ TCboWeaponEffectList }

procedure TCboWeaponEffectList.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
end;

function TCboWeaponEffectList.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := Index;
  if nUnit < 0 then
    nUnit := 0;

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + CBOWEAPONEFFECTIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(CBOWEAPONEFFECTIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

procedure TCboWeaponEffectList.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + CBOWEAPONEFFECTIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(CBOWEAPONEFFECTIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;
end;

{ TCboWeaponList }

procedure TCboWeaponList.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
end;

function TCboWeaponList.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := Index;
  if nUnit < 0 then
    nUnit := 0;

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + CBOWEAPONIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(CBOWEAPONIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

procedure TCboWeaponList.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + CBOWEAPONIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(CBOWEAPONIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;
end;

{ TCboHumList }

procedure TCboHumList.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
end;

function TCboHumList.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := Index;
  if nUnit < 0 then
    nUnit := 0;

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + CBOHUMIMGIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(CBOHUMIMGIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

procedure TCboHumList.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + CBOHUMIMGIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(CBOHUMIMGIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;
end;

{ TCboHumEffect }

procedure TCboHumEffect.Finalize;
var
  I:Integer;
begin
  for I := Low(ImagesArr) to High(ImagesArr) do begin
    if ImagesArr[I] <> nil then begin
      ImagesArr[I].Finalize;
      FreeAndNil(ImagesArr[I]);
    end;
  end;
  SetLength(ImagesArr, 0);
end;

function TCboHumEffect.IndexOf(Index:Integer):TGameImages;
var
  sFileName:string;
  nUnit:Integer;
begin
  nUnit := Index;
  if nUnit < 0 then
    nUnit := 0;

  if nUnit >= Length(ImagesArr) then
    SetLength(ImagesArr, nUnit + 1);

  if ImagesArr[nUnit] = nil then begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + CBOHUMEFFECTIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(CBOHUMEFFECTIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;
  Result := ImagesArr[nUnit];
end;

procedure TCboHumEffect.Initialize;
var
  sFileName:string;
  nUnit:Integer;
begin
  for nUnit := Low(ImagesArr) to High(ImagesArr) do begin
    if nUnit = 0 then
      sFileName := g_sSelfFilePath + CBOHUMEFFECTIMAGESFILE
    else
      sFileName := g_sSelfFilePath + Format(CBOHUMEFFECTIMAGESFILEEX, [nUnit + 1]);

    ImagesArr[nUnit] := CreateGameImages(sFileName);
    ImagesArr[nUnit].Initialize;
  end;
end;

procedure LoadCustomMonsterConfigs(Stream:TMemoryStream);
var
  Flag:TGUID;
  ReadCRC, CalcCRC:Cardinal;
  I, Count, Len:Integer;
  P:PChar;
  MonsterConfig:PClientCustomMonsterConfig;
begin
  Stream.Seek(0, soBeginning);
  Stream.Read(Flag, SizeOf(Flag));
  if not IsEqualGUID(Flag, ClientCustomMonsterConfigFlag) then
    Exit;
  Stream.Read(Count, SizeOf(Count));
  if Count = 0 then
    Exit;
  Stream.Read(ReadCRC, SizeOf(ReadCRC));
  Len := SizeOf(ClientCustomMonsterConfigFlag) + SizeOf(Count) + SizeOf(ReadCRC);
  P := Stream.Memory;
  Inc(P, Len);
  CalcCRC := BufferCRC(P, Stream.Size - Len);
  if ReadCRC <> CalcCRC then
    Exit;

  for I := 0 to g_CustomMonsterConfig.Count - 1 do begin
    Dispose(PClientCustomMonsterConfig(g_CustomMonsterConfig.Items[I]));
  end;
  g_CustomMonsterConfig.Clear;

  Stream.Seek(Len, soBeginning);
  for I := 0 to Count - 1 do begin
    New(MonsterConfig);
    Stream.Read(MonsterConfig^, SizeOf(TClientCustomMonsterConfig));
    g_CustomMonsterConfig.Add(MonsterConfig);
  end;
end;

procedure ClearCustomMonsterConfig;
var
  I:Integer;
begin
  for I := 0 to g_CustomMonsterConfig.Count - 1 do begin
    Dispose(PClientCustomMonsterConfig(g_CustomMonsterConfig.Items[I]));
  end;
end;

procedure LoadCustomMagicConfigs(Stream:TMemoryStream);
var
  Flag:TGUID;
  ReadCRC, CalcCRC:Cardinal;
  I, Count, Len:Integer;
  P:PChar;
  MagicConfig:PClientCustomMagicConfig;
  MagicWarrNGOption:TMagicWarrNGOption;
begin
  Stream.Seek(0, soBeginning);
  Stream.Read(Flag, SizeOf(Flag));
  if not IsEqualGUID(Flag, ClientCustomMagicConfigFlag) then
    Exit;
  Stream.Read(Count, SizeOf(Count));
  if Count = 0 then
    Exit;
  Stream.Read(Len, SizeOf(Len));
  if Len <> SizeOf(TClientCustomMagicConfig) then
    Exit;

  Stream.Read(ReadCRC, SizeOf(ReadCRC));
  Len := SizeOf(ClientCustomMagicConfigFlag) + SizeOf(Count) + SizeOf(Len) + SizeOf(ReadCRC);
  P := Stream.Memory;
  Inc(P, Len);
  CalcCRC := BufferCRC(P, Stream.Size - Len);
  if ReadCRC <> CalcCRC then
    Exit;

  for I := 0 to g_CustomMagicConfig.Count - 1 do begin
    Dispose(PClientCustomMagicConfig(g_CustomMagicConfig.Items[I]));
  end;
  g_CustomMagicConfig.Clear;

  for MagicWarrNGOption := Low(g_AutoCustomMagic) to High(g_AutoCustomMagic) do begin
    SetLength(g_AutoCustomMagic[MagicWarrNGOption], 0);
  end;

  Stream.Seek(Len, soBeginning);
  for I := 0 to Count - 1 do begin
    New(MagicConfig);
    Stream.Read(MagicConfig^, SizeOf(TClientCustomMagicConfig));
    g_CustomMagicConfig.Add(MagicConfig);

    MagicWarrNGOption := MagicConfig.MagicBaseConfig.MagicWarrNGOption;
    if MagicConfig.boIsMagicWarr and (MagicWarrNGOption <> mngoNone) then begin
      SetLength(g_AutoCustomMagic[MagicWarrNGOption], Length(g_AutoCustomMagic[MagicWarrNGOption]) + 1);
      g_AutoCustomMagic[MagicWarrNGOption][Length(g_AutoCustomMagic[MagicWarrNGOption]) - 1] := MagicConfig.wMagicID;
    end;
  end;
end;

procedure ClearCustomMagicConfig;
var
  I:Integer;
begin
  for I := 0 to g_CustomMagicConfig.Count - 1 do begin
    Dispose(PClientCustomMagicConfig(g_CustomMagicConfig.Items[I]));
  end;
end;

procedure LoadCustomNpcConfigs(Stream:TMemoryStream);
var
  Flag:TGUID;
  ReadCRC, CalcCRC:Cardinal;
  I, Count, Len:Integer;
  P:PChar;
  NpcConfig:PClientCustomNpcConfig;
begin
  Stream.Seek(0, soBeginning);
  Stream.Read(Flag, SizeOf(Flag));
  if not IsEqualGUID(Flag, ClientCustomNpcConfigFlag) then
    Exit;
  Stream.Read(Count, SizeOf(Count));
  if Count = 0 then
    Exit;
  Stream.Read(Len, SizeOf(Len));
  if Len <> SizeOf(TClientCustomNpcConfig) then
    Exit;

  Stream.Read(ReadCRC, SizeOf(ReadCRC));
  Len := SizeOf(ClientCustomNpcConfigFlag) + SizeOf(Count) + SizeOf(Len) + SizeOf(ReadCRC);
  P := Stream.Memory;
  Inc(P, Len);
  CalcCRC := BufferCRC(P, Stream.Size - Len);
  if ReadCRC <> CalcCRC then
    Exit;

  for I := 0 to g_CustomNpcConfig.Count - 1 do begin
    Dispose(PClientCustomNpcConfig(g_CustomNpcConfig.Items[I]));
  end;
  g_CustomNpcConfig.Clear;

  Stream.Seek(Len, soBeginning);
  for I := 0 to Count - 1 do begin
    New(NpcConfig);
    Stream.Read(NpcConfig^, SizeOf(TClientCustomNpcConfig));
    g_CustomNpcConfig.Add(NpcConfig);
  end;
end;

procedure ClearCustomNpcConfig;
var
  I:Integer;
begin
  for I := 0 to g_CustomNpcConfig.Count - 1 do begin
    Dispose(PClientCustomNpcConfig(g_CustomNpcConfig.Items[I]));
  end;
end;

procedure LoadNGCustomUnbindItemList(FileName:string);
var
  I:Integer;
  LoadList:TStringList;
  sLineText:string;
  sItemName:string;
  sBindItemName:string;
  sType:string;
  nType:Integer;
  BindItem:pTCustomBindItem;
begin
  for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
    Dispose(pTCustomBindItem(g_CustomUnbindItemList.Items[I]));
  end;
  g_CustomUnbindItemList.Clear;

  if not FileExists(FileName) then
    Exit;

  LoadList := TStringList.Create;
  try
    try
      LoadList.LoadFromFile(FileName);

      for I := 0 to LoadList.Count - 1 do begin
        sLineText := Trim(LoadList.Strings[I]);
        if sLineText <> '' then begin
          sLineText := GetValidStr3_Ex(sLineText, sType, '/');
          sLineText := GetValidStr3_Ex(sLineText, sItemName, '/');
          sLineText := GetValidStr3_Ex(sLineText, sBindItemName, '/');
          nType := StrToIntDef(sType, 0);
          if (nType in [1..4]) and (sItemName <> '') then begin
            New(BindItem);
            BindItem.UnBindItemType := TUnBindItemType(nType);
            BindItem.sItemName := sItemName;
            BindItem.sBindItemName := sBindItemName;
            BindItem.boSpecialMP := sLineText = '1';
            g_CustomUnbindItemList.Add(BindItem);
          end;
        end;
      end;
    except

    end;
  finally
    LoadList.Free;
  end;
end;

procedure SaveNGCustomUnbindItemList(FileName:string);
var
  I:Integer;
  SaveList:TStringList;
  BindItem:pTCustomBindItem;
  boSpecialMP:Boolean;
begin
  SaveList := TStringList.Create;
  for I := 0 to g_CustomUnbindItemList.Count - 1 do begin
    BindItem := g_CustomUnbindItemList.Items[I];
    boSpecialMP := False;
    if BindItem.UnBindItemType = t_Special then
      boSpecialMP := BindItem.boSpecialMP;
    SaveList.Add(IntToStr(Integer(BindItem.UnBindItemType)) + '/' + BindItem.sItemName + '/' + BindItem.sBindItemName + '/' + IntToStr(Integer(boSpecialMP)));
  end;
  try
    SaveList.SaveToFile(FileName);
  except
  end;
  SaveList.Free;
end;

function GetStdItem(nIndex:Integer):PTClientItem;
begin
  if (nIndex > 0) and (nIndex <= g_StdItemList.Count) then
    Result := g_StdItemList.Items[nIndex - 1]
  else
    Result := nil;
end;

function GetFluteCount(Item:PTClientItem):Integer;
var
  I:Integer;
begin
  Result := 0;
  for I := 0 to Item.btFluteCount - 1 do begin
    if (I in [0..MAX_FLUTE_COUNT - 1]) then begin
      if Item.Flutes[I].GemIndex > 0 then
        Inc(Result);
    end;
  end;
end;

{
// 获取相同Anicount宝石的数量
function GetAnicountFluteCount(Item: PTClientItem; AniCount: Integer): Integer;
var
  I: Integer;
  Item2: PTClientItem;
begin
  Result := 0;
  for I := 0 to Item.btFluteCount - 1 do
  begin
    if (I in [0..MAX_FLUTE_COUNT - 1]) then
    begin
      if Item.wFlute[I] > 0 then
      begin
        Item2 := GetStdItem(Item.wFlute[I]);
        if (Item2 <> nil) and (Item2.s.AniCount = AniCount) then
          Inc(Result);
      end;
    end;
  end;
end;

// 获取相同Anicount宝石的数量
function GetIdxFluteCount(Item: PTClientItem; nItemIdx: Integer): Integer;
var
  I: Integer;
begin
  Result := 0;
  for I := 0 to Item.btFluteCount - 1 do
  begin
    if (I in [0..MAX_FLUTE_COUNT - 1]) then
    begin
      if (Item.wFlute[I] > 0) and (Item.wFlute[I] = nItemIdx) then
      begin
        Inc(Result);
      end;
    end;
  end;
end;
}

function CheckBlockListSys(Ident:Integer; sMsg:string):Boolean;
var
  I:integer;
  sUserName:string;
begin
  try //程序自动增加
    Result := True;
    case Ident of
      SM_HEAR, SM_GROUPMESSAGE, SM_GUILDMESSAGE:begin
          GetValidStr3_Ex(sMsg, sUserName, ':');
        end;
      SM_CRY:begin
          GetValidStr3_Ex(sMsg, sUserName, ':');
          sUserName := RightStr(sUserName, Length(sUserName) - 3);
        end;
      SM_WHISPER:
        GetValidStr3_Ex(sMsg, sUserName, '=');
    end;
    if sUserName <> '' then begin
      // 私聊显示等级时，黑名单不过滤
      GetValidStr3_Ex(sUserName, sUserName, ' ');
      I := g_MyBlacklist.IndexOf(sUserName);
      if I > -1 then Result := False;
    end;
  except //程序自动增加
    Result := False; //HZQ 20230524添加异常后的返回值
    DebugOutStr('[Exception] MShare.CheckBlockListSys');
  end;
end;

function IntToHexN(const V, Digits:Integer):string;
const
  CSTR = '00000000000000000000000000000000';
  Convert2:array[0..9] of Char = ('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
var
  P, P1:PAnsiChar;
  I:Int64;
  NewLen:Integer;
begin
  Result := '';
  if Digits > 10 then
    Exit;
  if Digits < 2 then
    Exit;

  GetMem(P, 32);

  Move(CSTR, P^, 32);
  P1 := P + 32 - 1;
  I := V;
  while True do begin
    P1^ := Convert2[I mod Digits];
    I := I div Digits;
    if I = 0 then
      Break
    else begin
      Dec(P1);
      if P1 - P <= 0 then
        Break;
    end;
  end;
  NewLen := 32 - (P1 - P);
  SetString(Result, P1, NewLen);

  FreeMem(P);
end;

{$IF AntiPlugInMemory = 1}

procedure FreeAntiPlugDllMemoryModule;
type
  TAntiPlugDoUnInit = procedure; stdcall;
var
  Temp:PMemoryModule;
  DoUnInit:TAntiPlugDoUnInit;
begin
  g_AntiPlugNotifyFunc := nil;

  if g_AntiPlugDllMemoryModule <> nil then begin
    Temp := g_AntiPlugDllMemoryModule;
    g_AntiPlugDllMemoryModule := nil;

    DoUnInit := MemoryGetProcAddress(Temp, 'DoUnInit');
    if Assigned(DoUnInit) then begin
      try
        DoUnInit;
      except
        //OutputDebugString('aaaaa');
      end;
    end;

    MemoryFreeLibrary(Temp);
  end;
end;

{$ELSE}

procedure FreeAntiPlugDllMemoryModule;
var
  Temp:HMODULE;
begin
  g_AntiPlugNotifyFunc := nil;

  if g_AntiPlugDllModule <> 0 then begin
    Temp := g_AntiPlugDllModule;
    g_AntiPlugDllModule := 0;
    FreeLibrary(Temp);
  end;
end;
{$IFEND}

function ProcessFileNameSpecialChar(S:string):string;
var
  WS:WideString;
  I:Integer;
begin
  WS := S;
  for I := 1 to Length(WS) do begin
    if WS[I] in [WideChar('/'), WideChar('\'), WideChar(':'), WideChar('*'), WideChar('?'), WideChar('"'), WideChar('<'), WideChar('>'), WideChar('|')] then begin
      case WS[I] of
        '/':
          WS[I] := '{';
        '\':
          WS[I] := '}';
        ':':
          WS[I] := ';';
        '*':
          WS[I] := '@';
        '?':
          WS[I] := '!';
        '"':
          WS[I] := '~';
        '<':
          WS[I] := '(';
        '>':
          WS[I] := ')';
        '|':
          WS[I] := '-';
      end;
    end;
  end;
  Result := WS;
end;

function SaveOrLoadMyBagItemList(IsSave:Boolean):Boolean;
const
  BAGITMESFILE = 'Config\%s.%s.BagItems.set';
var
  I, MakeIndex:Integer;
  sDirectory, sFileName:string;
  SL:TStringList;
  ClientItem:PTClientItem;
  S:string;
begin
  Result := False;
  sDirectory := g_sSelfFilePath + 'Config\';
  if not DirectoryExists(sDirectory) then
    ForceDirectories(sDirectory);

  if (g_MySelf <> nil) and (g_MySelf.m_sUserName <> '') then begin
    S := ProcessFileNameSpecialChar(g_MySelf.m_sUserName);
  end
  else begin
    Exit;
  end;

  sFileName := g_sSelfFilePath + Format(BAGITMESFILE, [g_sServerName, S]);
  SL := TStringList.Create;
  try
    if IsSave then begin
      I := 0;
      while I <= GetMaxBagCount - 1 do begin
        ClientItem := @g_ItemArr[I];
        if Length(ClientItem.s.Name) > 0 then
          SL.Add(IntToStr(Integer(ClientItem.MakeIndex)))
        else
          SL.Add(IntToStr(Integer(0)));

        Inc(I);
      end;
      SL.SaveToFile(sFileName);

      Result := True;
    end
    else begin
      g_SaveMyBagItemList.Clear;
      if FileExists(sFileName) then begin
        SL.LoadFromFile(sFileName);

        for I := 0 to SL.Count - 1 do begin
          MakeIndex := StrToIntDef(SL.Strings[I], 0);
          g_SaveMyBagItemList.Add(Pointer(MakeIndex));

          if I >= GetMaxBagCount - 1 then
            Break;
        end;

        Result := True;
      end;
    end;
  finally
    SL.Free;
  end;
end;

function tick_diff(tick_start, tick_end:Cardinal):Cardinal;
begin
  if tick_end >= tick_start then
    result := tick_end - tick_start
  else
    result := High(Cardinal) - tick_start + tick_end;
end;

function GetTickCount_Ex():LongWord;
begin
  Result := TimeGetTime();
end;

function HpAddUnit(V:LongWord):string;
begin
  if V >= 100000000 then
    Result := Format('%.2fE', [V / 100000000])
  else if V >= 100000 then
    Result := IntToStr(V div 10000) + 'W'
  else
    Result := IntToStr(V);
end;

function GetTempDir:string;
var
  Buf:array[0..MAX_PATH - 1] of Char;
begin
  GetTempPath(SizeOf(Buf) div SizeOf(Buf[0]), Buf);
  Result := StrPas(Buf);
end;

function MakeTempFileName(const FileExt:string):string;
var
  N:Int64;
begin
  if QueryPerformanceCounter(N) then
    Result := Format('%x', [N])
  else
    Result := Format('%.8x%.4x', [MyGetTickCount, Random(MAXINT)]);

  if Length(FileExt) > 0 then
    Result := Result + '.' + FileExt;
end;

function GetHintNameFontName:string;
begin
  Result := g_ClientConfig.sShowHintFontName;
end;

function GetHintNameFontSize:Integer;
begin
  Result := g_ClientConfig.btShowHintNameFontSize;
end;

function GetHintNameFontStyle(FontStyles:TFontStyles):TFontStyles;
begin
  case g_ClientConfig.btShowHintNameFontBold of
    0:
      Result := FontStyles;
    1:
      Result := [];
    2:
      Result := [fsBold];
  end;
end;

function GetHintNameFontStroke(IsStroke:Boolean = False):Boolean;
begin
  case g_ClientConfig.btShowHintNameFontStroke of
    0:Result := IsStroke;
    1:Result := False;
    2:Result := True;
    else Result := False; //HZQ 20230524
  end;
end;

function GetHintFontSize:Integer;
begin
  Result := g_ClientConfig.btShowHintOtherFontSize;
end;

function GetHintFontStyle(FontStyles:TFontStyles):TFontStyles;
begin
  case g_ClientConfig.btShowHintOtherFontBold of
    0:
      Result := FontStyles;
    1:
      Result := [];
    2:
      Result := [fsBold];
  end;
end;

function GetHintFontStroke(IsStroke:Boolean = False):Boolean;
begin
  case g_ClientConfig.btShowHintOtherFontStroke of
    0:Result := IsStroke;
    1:Result := False;
    2:Result := True;
    else Result := False; //HZQ 20230524
  end;
end;

function GetMaxBagCount:Integer;
begin
  Result := DEF_MAX_BAG_ITEM + g_ExtBagOpenItemCount;
end;

procedure Clear_g_EnabledAuctionItemList;
var
  I:Integer;
  PricesLime:PTAcutionItemPricesLime;
begin
  for I := 0 to g_EnabledAuctionItemList.Count - 1 do begin
    PricesLime := PTAcutionItemPricesLime(g_EnabledAuctionItemList.Objects[I]);
    if PricesLime <> nil then begin
      Dispose(PricesLime);
    end;
  end;
  g_EnabledAuctionItemList.Clear;
end;

function IsAttackAction(Action:Integer):Boolean;
begin
  Result := (Action = SM_HIT) or (Action = SM_HEAVYHIT) or (Action = SM_BIGHIT) or (Action = SM_POWERHIT) or (Action = SM_LONGHIT) or (Action = SM_WIDEHIT) or (Action = SM_FIREHIT) or (Action = SM_CRSHIT) or (Action = SM_TWNHIT) or (Action = SM_SWORDHIT) or (Action = SM_43HIT) or (Action = SM_66HIT) or (Action = SM_66HIT1) or (Action = SM_101HIT) or (Action = SM_102HIT) or (Action = SM_103HIT) or (Action = SM_113HIT) or (Action = SM_115HIT) or ((Action >= SM_CUSTOM_HIT001) and (Action < SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT));
end;

function ShiftStateToPlugShiftState(Shift:TShiftState):Integer;
begin
  Result := 0;
  if ssShift in Shift then
    Result := Result + ShiftState_Shift;

  if ssAlt in Shift then
    Result := Result + ShiftState_Alt;

  if ssCtrl in Shift then
    Result := Result + ShiftState_Ctrl;

  if ssLeft in Shift then
    Result := Result + ShiftState_Left;

  if ssRight in Shift then
    Result := Result + ShiftState_Right;

  if ssMiddle in Shift then
    Result := Result + ShiftState_Middle;

  if ssDouble in Shift then
    Result := Result + ShiftState_Double;
end;

function CheckInAutoCustomMagic(MagicWarrNGOption:TMagicWarrNGOption; MagicID:Integer):Boolean;
var
  I:Integer;
begin
  Result := False;
  if not CheckIsCustomMagic(MagicID) then
    Exit;

  if (MagicWarrNGOption >= Low(g_AutoCustomMagic)) and (MagicWarrNGOption <= High(g_AutoCustomMagic)) then begin
    for I := 0 to Length(g_AutoCustomMagic[MagicWarrNGOption]) - 1 do begin
      if g_AutoCustomMagic[MagicWarrNGOption][I] = MagicID then begin
        Result := True;
        Break;
      end;
    end;
  end;
end;

procedure SortStdItemListDoSort(L, R:Integer);

  function CompareItem(Index1, Index2:Integer):Integer;
  var
    Item1, Item2:PSortClientItem;
  begin
    Item1 := g_SortStdItemList.Items[Index1];
    Item2 := g_SortStdItemList.Items[Index2];

    Result := AnsiCompareText(Item1.ClientItem.s.Name, Item2.ClientItem.s.Name);
  end;

var
  I, J, P:Integer;
begin
  repeat
    I := L;
    J := R;
    P := (L + R) shr 1;
    repeat
      while CompareItem(I, P) < 0 do
        Inc(I);
      while CompareItem(J, P) > 0 do
        Dec(J);
      if I <= J then begin
        g_SortStdItemList.Exchange(I, J);
        if P = I then
          P := J
        else if P = J then
          P := I;
        Inc(I);
        Dec(J);
      end;
    until I > J;
    if L < J then
      SortStdItemListDoSort(L, J);
    L := I;
  until I >= R;
end;

function GetItemIdxByName(const ItemName:string):Word;
var
  L, H, I, C:Integer;
  Item:PSortClientItem;
begin
  Result := 0;
  L := 0;
  H := g_SortStdItemList.Count - 1;
  while L <= H do begin
    I := L + (H - L) shr 1;
    Item := g_SortStdItemList.Items[I];
    C := AnsiCompareText(Item.ClientItem.s.Name, ItemName);
    if C < 0 then
      L := I + 1
    else begin
      H := I - 1;
      if C = 0 then begin
        L := I;
        Result := Item.ItemIndex;
      end;
    end;
  end;
end;

procedure RecalcDraw(VirtualRect, VisibleRect:TRect; IsOffset:Boolean; OffsetX, OffsetY:Integer; Texture:TTexture; ABlendMode:Integer; OnlyDrawRect:Boolean = False);
var
  PaintRect, R1:TRect;
  nX, nY, nLeft, nTop, nWidth, nHeight:Integer;
begin
  if Texture.Width * Texture.Height <= 4 then
    Exit;
  if (VisibleRect.Bottom <= VisibleRect.Top) or (VisibleRect.Right <= VisibleRect.Left) then
    Exit;

  if IsOffset then begin
    nLeft := OffsetX;
  end
  else begin
    if VisibleRect.Left > VirtualRect.Left then
      nLeft := VisibleRect.Left - VirtualRect.Left
    else
      nLeft := OffsetX;
  end;

  nX := VirtualRect.Left + nLeft;

  if IsOffset then begin
    nTop := OffsetY;
  end
  else begin
    if VisibleRect.Top > VirtualRect.Top then
      nTop := VisibleRect.Top - VirtualRect.Top
    else
      nTop := OffsetY;
  end;

  nY := VirtualRect.Top + nTop;

  if not OnlyDrawRect then begin
    GameCanvas.Draw(nX, nY, Texture, ABlendMode);
    Exit;
  end;

  if (VirtualRect.Bottom - VirtualRect.Top <= nTop) or (VirtualRect.Right - VirtualRect.Left <= nLeft) then
    Exit;

  if VisibleRect.Right < VirtualRect.Right then
    nWidth := VisibleRect.Right - VirtualRect.Left
  else
    nWidth := VirtualRect.Right - VirtualRect.Left;

  if VirtualRect.Bottom > VisibleRect.Bottom then
    nHeight := VisibleRect.Bottom - VirtualRect.Top
  else
    nHeight := VirtualRect.Bottom - VirtualRect.Top;

  nWidth := nWidth - nLeft - OffsetX;
  nHeight := nHeight - nTop - OffsetY;

  if (nHeight <= 0) or (nWidth <= 0) then
    Exit;

  R1 := Bounds(Texture.ClientRect.Left + nLeft, Texture.ClientRect.Top + nTop, nWidth, nHeight);
  PaintRect := ShortRect(R1, Texture.ClientRect);

  if (PaintRect.Bottom <= PaintRect.Top) or (PaintRect.Right <= PaintRect.Left) then
    Exit;
  GameCanvas.Draw(nX, nY, PaintRect, Texture, ABlendMode)
end;

function GetInputBoxInFilterList(sInputBox:string):Boolean;
var
  I:Integer;
  WS:WideString;
  WChr:WideChar;
begin
  Result := False;

  WS := sInputBox;
  for I := 1 to Length(WS) do begin
    WChr := WS[I];
    if WChr in [WideChar('@'), WideChar('<'), WideChar('>'), WideChar('$')] then begin
      Result := True;
      Exit;
    end;
  end;

  sInputBox := LowerCase(sInputBox);
  for I := 0 to g_InputBoxFilterList.Count - 1 do begin
    if Pos(g_InputBoxFilterList[I], sInputBox) > 0 then begin
      Result := True;
      Break;
    end;
  end;
end;

{ TWarrContinueHitManager }

constructor TWarrContinueHitManager.Create();
begin
  FLastUseMagicTick := 0;
  inherited Create;
end;

function TWarrContinueHitManager.CanOpenMagic(MagicID:Word):Boolean;
{
var
  I: Integer;
  TempID: Integer;
  IsFound: Boolean;
}
begin
  Result := True;

  {
  if not g_ClientConfig.boDisableWarrContinueHit then Exit;

  // 禁止同时召唤多个技能
  if not g_ClientConfig.boDisableCalcMutiContinueHit then Exit;   // 这个参数去掉了，此处后面的代码全无效了

  IsFound := False;
  for I := 0 to Length(g_ClientConfig.ArrDisableWarrContinueHitIDs) - 1 do
  begin
    TempID := g_ClientConfig.ArrDisableWarrContinueHitIDs[I];
    if TempID = 0 then Break;

    if TempID = MagicID then
    begin
      IsFound := True;
      Break;
    end;
  end;

  if not IsFound then Exit;

  for I := 0 to Length(g_ClientConfig.ArrDisableWarrContinueHitIDs) - 1 do
  begin
    TempID := g_ClientConfig.ArrDisableWarrContinueHitIDs[I];
    if TempID = 0 then Break;

    if TempID = MagicID then Continue;

    case TempID of
      12:         // 刺杀剑法
        Result := not g_boCanLongHit;

      25:         // 半月弯刀
        Result := not g_boCanWideHit;

      26:         // 烈火剑法
        Result := not g_boNextTimeFireHit;

      40:         // 双龙斩
        Result := not g_boCanCrsHit;

      42:         // 破空剑 龙影剑法
        Result := not g_boNextTime42Hit;

      43:         // 雷霆剑法
        Result := not boNextTime43Hit;

      56:         // 逐日剑法
        Result := not g_boNextTimeSwordHit;

      66:         // 开天斩
        Result := (not g_boNextTime66Hit) and (not g_boNextTime66Hit1);

      113:        // 断空斩
        Result := not g_boNextTime113Hit;

      115:        // 血魄一击
        Result := not g_boNextTime115Hit;

    else if CheckIsCustomMagic(TempID) then
        Result := g_boNextTimeCustomMagicList.IndexOf(Pointer(TempID - CUSTOM_MAGIC_START_ID)) < 0;
    end;

    if not Result then Exit;
  end;
  }
end;

function TWarrContinueHitManager.CanUseMagic(MagicID:Word):Boolean;
var
  I:Integer;
  TempID:Integer;
  IsFound:Boolean;
begin
  Result := True;

  if not g_ClientConfig.boDisableWarrContinueHit then
    Exit;

  if FLastUseMagicID = 0 then
    Exit;
  if FLastUseMagicID = MagicID then
    Exit;

  IsFound := False;
  for I := 0 to Length(g_ClientConfig.ArrDisableWarrContinueHitIDs) - 1 do begin
    TempID := g_ClientConfig.ArrDisableWarrContinueHitIDs[I];

    if TempID = 0 then
      Break;

    if TempID = MagicID then begin
      IsFound := True;
      Break;
    end;
  end;

  if IsFound then begin
    Result := tick_diff(FLastUseMagicTick, MyGetTickCount) >= g_ClientConfig.nWarrContinueHitMinInterval + 100;
  end;
end;

procedure TWarrContinueHitManager.UseMagic(MagicID:Word);
var
  I:Integer;
  TempID:Integer;
  IsFound:Boolean;
begin
  IsFound := False;
  for I := 0 to Length(g_ClientConfig.ArrDisableWarrContinueHitIDs) - 1 do begin
    TempID := g_ClientConfig.ArrDisableWarrContinueHitIDs[I];
    if TempID = 0 then
      Break;

    if TempID = MagicID then begin
      IsFound := True;
      Break;
    end;
  end;

  if IsFound then begin
    FLastUseMagicID := MagicID;
    FLastUseMagicTick := MyGetTickCount;
  end;
end;

function DrawItemHintOldStyle(nX, nY:Integer; S:WideString; Color:TColor = clWhite):Integer;

  function PaintText(sText:string):Integer;
  var
    Index:Integer;
    sColor:string;
    nColor:Integer;
    TempColor:TColor;
  begin
    Result := 0;
    if Length(sText) = 0 then
      Exit;

    TempColor := Color;

    if (Length(sText) > 3) and (sText[1] = '{') and (sText[Length(sText)] = '}') then begin
      Index := Pos('|', sText);
      if Index > 1 then begin
        sColor := Copy(sText, Index + 1, Length(sText) - Index - 1);
        if TryStrToInt(sColor, nColor) then begin
          if (nColor >= 0) and (nColor <= 255) then begin
            TempColor := GetRGB(nColor);
            sText := Copy(sText, 2, Index - 2);
          end;
        end;
      end;
    end;

    Result := CurrentFont.TextWidth(sText);
    CurrentFont.TextOut(nX, nY, sText, TempColor);
    nX := nX + Result;
  end;

var
  ItemText:WideString;
  I, J, Len, LastSave:Integer;
  FindEnd:WideChar;
begin
  Result := 0;
  Len := Length(S);
  if Len = 0 then
    Exit;

  LastSave := 1;
  I := 1;
  while I <= Len do
    begin
 // 找到<后，
    if S[I] in [WideChar('{')] then begin
      FindEnd := '}';
      // 一直往后找，直到找到 > 为止
      for J := I + 1 to Len do begin
        if S[J] = FindEnd then begin
          if LastSave <> I then begin
            ItemText := Copy(S, LastSave, I - LastSave);
            Result := Result + PaintText(ItemText);
          end;

          ItemText := Copy(S, I, J - I + 1);
          Result := Result + PaintText(ItemText);
          I := J;
          LastSave := J + 1; // 这个执行后，Inc(I)，固J+1
          Dec(I);
          Break;
        end;
      end;
    end;

  Inc(I);
end;

if LastSave <= Len then begin
  ItemText := Copy(S, LastSave, MaxInt);
  Result := Result + PaintText(ItemText);
end;
end;

//------------------------------------------------------------------------------

{ THttpClient }

constructor THttpClient.Create;
begin
    m_nConnectTimeOut := 2000;
    m_nSendTimeOut := 5000;
    m_nRecvTimeOut := 5000;
end;

destructor THttpClient.Destroy;
begin

  inherited;
end;

function THttpClient.GetInternetStatusCode(hUrl: HINTERNET): Integer;
var
    dwBuffLen:DWORD;
    dwReserved:DWORD;
begin
    dwReserved := 0;
    dwBuffLen := SizeOf(Result);
    if not HttpQueryInfo(hUrl, HTTP_QUERY_FLAG_NUMBER or HTTP_QUERY_STATUS_CODE, @Result, dwBuffLen, dwReserved) then begin
        Result := 500; //错误
    end;
end;

function THttpClient.Get(sUrl:string; streamRlt:TStream): Boolean;
var
    uri:TUri;
    
    hInt, hConn, hreq:HINTERNET;
    dwFlag, dwRead:DWORD;
    sPathDoc:string;
    nPort:Word;
    arrBuff:array of Byte;
begin
    Result := False;

    uri := ParseUrl(sUrl);
    if SameText(uri.Protocol, 'https') then begin
        nPort := StrToIntDef(uri.Port, INTERNET_DEFAULT_HTTPS_PORT);
        dwFlag := INTERNET_FLAG_SECURE;
    end else begin
        nPort := StrToIntDef(uri.Port, INTERNET_DEFAULT_HTTP_PORT);
        dwFlag := INTERNET_FLAG_RELOAD;
    end;
    sPathDoc := uri.Path + uri.Doc + uri.Param; 

    hInt := InternetOpen(UserAgent, INTERNET_OPEN_TYPE_PRECONFIG, nil, nil, 0);
    if hInt <> nil then begin
        hConn := InternetConnect(hInt, PChar(uri.Host), nPort, nil, nil, INTERNET_SERVICE_HTTP, 0, 0);
        if hConn <> nil then begin
            hreq :=  HttpOpenRequest(hConn, 'GET', PChar(sPathDoc), 'HTTP/1.1', nil, nil, dwFlag, 0);
            if hreq <> nil then begin
                if SetInternetSecurityOption(hreq) and SetInternetTimeout(hreq) then begin
                    if HttpSendRequest(hreq, nil, 0, nil, 0) then begin
                        m_nRetCode := GetInternetStatusCode(hreq);
                        if m_nRetCode = 200 then begin
                            SetLength(arrBuff, 64*1024);
                            dwRead := 0;
                            repeat
                                Result := InternetReadFile(hreq, @arrBuff[0], 64*1024, dwRead);
                                if dwRead <> 0 then begin
                                    streamRlt.Write(arrBuff[0], dwRead);
                                end;
                            until dwRead = 0;
                         end;
                    end;
                end;
                InternetCloseHandle(hreq);
            end;
            InternetCloseHandle(hConn);
        end;
        InternetCloseHandle(hInt);
    end;
end;

function THttpClient.Post(sUrl, sHeader, sBody: string; streamRlt: TStream): Boolean;
var
    uri:TURI;
    hInt, hConn, hreq:HINTERNET;
    dwFlag, dwRead:DWORD;
    sPathDoc:string;
    nPort:Word;
    arrBuff:array of Byte;
    pBody:Pointer;
    nBodySize:Integer;
begin
    Result := False;

    uri := ParseURL(sUrl);
    if SameText(uri.Protocol, 'https') then begin
        nPort := StrToIntDef(uri.Port, INTERNET_DEFAULT_HTTPS_PORT);
        dwFlag := INTERNET_FLAG_SECURE;
    end else begin
        nPort := StrToIntDef(uri.Port, INTERNET_DEFAULT_HTTP_PORT);
        dwFlag := INTERNET_FLAG_RELOAD;
    end;
    sPathDoc := uri.Path + uri.Doc + uri.Param;

    hInt := InternetOpen(UserAgent, INTERNET_OPEN_TYPE_PRECONFIG, nil, nil, 0);
    if hInt <> nil then begin
        hConn := InternetConnect(hInt, PChar(Uri.Host), nPort, nil, nil, INTERNET_SERVICE_HTTP, 0, 0);
        if hConn <> nil then begin
            hreq :=  HttpOpenRequest(hConn, 'POST', PChar(sPathDoc), 'HTTP/1.1', nil, nil, dwFlag, 0);
            if hreq <> nil then begin
                if SetInternetSecurityOption(hreq) and SetInternetTimeout(hreq) then begin
                    nBodySize := Length(sBody);
                    if nBodySize > 0 then pBody := PChar(sBody) else pBody := nil;
                    if HttpAddRequestHeaders(hreq, PChar(sHeader), Length(sHeader), HTTP_ADDREQ_FLAG_ADD or HTTP_ADDREQ_FLAG_REPLACE) then begin
                        if HttpSendRequest(hreq, nil, 0, pBody, nBodySize) then begin
                            m_nRetCode := GetInternetStatusCode(hreq);
                            if m_nRetCode = 200 then begin
                                SetLength(arrBuff, 64*1024);
                                dwRead := 0;
                                repeat
                                    Result := InternetReadFile(hreq, @arrBuff[0], 64*1024, dwRead);
                                    if dwRead <> 0 then begin
                                        streamRlt.Write(arrBuff[0], dwRead);
                                    end;
                                until dwRead = 0;
                            end;
                        end;
                    end;
                end;
                InternetCloseHandle(hreq);
            end;
            InternetCloseHandle(hConn);
        end;
        InternetCloseHandle(hInt);
    end;
end;

class function THttpClient.ParseURL(sUrl:string): TUri;
var
    nPos1, nPos2, nPos, nCount:Integer;
begin
    ZeroMemory(@Result, SizeOf(Result));

    if StartsText('https://', sURL) then BEGIN
        Delete(sURL, 1, 8);
        Result.Protocol := 'https';
    end else if StartsText('http://', sURL) then begin
        Delete(sURL, 1, 7);
        Result.Protocol := 'http';
    end else begin
        Exit;
    end;

    nPos := RevPos('#', sUrl);
    if nPos > 0 then begin
        Result.BookMark := Copy(sUrl, nPos+1, Length(sUrl)-nPos);
        sUrl := Copy(sUrl, 1, nPos-1);
    end;  

    nPos1 := Pos('/', sURL);
    if nPos1 > 0 then begin
        Result.Host := Copy(sUrl, 1, nPos1 - 1);
    end else begin
        Result.Host := sURL;
    end;

    nPos := Pos('@', Result.Host);
    if nPos > 0 then begin
        Result.UserName := Copy(Result.Host, 1, nPos-1);
        Result.Host := Copy(Result.Host, nPos+1, Length(Result.Host) -1);

        nPos := Pos(':', Result.UserName);
        if nPos > 0 then begin
            Result.Password := Copy(Result.UserName, nPos+1, Length(Result.UserName) - nPos);
            Result.UserName := Copy(Result.UserName, 1, nPos-1);
        end;
    end;

    nCount := Length(sUrl);
    nPos := Pos(':', Result.Host);
    if nPos > 0 then begin
        Result.Port := Copy(Result.Host, nPos+1, Length(Result.Host) - nPos);
        Result.Host := Copy(Result.Host, 1, nPos -1);
    end;
    
    if nPos1 = 0 then Exit;
    

    nPos2 := RevPos('/', sUrl);
    if nPos2 > 0 then begin
        Result.Path := Copy(sUrl, nPos1, nPos2-nPos1 + 1);
    end;

    nPos := Pos('?', sUrl);
    if (nPos2 > 0) and (nPos > nPos2) then begin
        Result.Doc := Copy(sUrl, nPos2+1, nPos-nPos2-1);
        Result.Param := Copy(sUrl, nPos , nCount- nPos + 1);
    end else begin
        Result.Doc := Copy(sUrl, nPos2+1, nCount - nPos2);
    end; 
end;

class function THttpClient.RevPos(chSub:Char; const sContent: string): Integer;
var
    i:Integer;
begin
    Result := 0;
    for i := Length(sContent) downto 1 do begin
        if chSub = sContent[i] then begin
            Result := i;
            Break;
        end;
    end;
end;

function THttpClient.SetInternetSecurityOption(hNet: HINTERNET): Boolean;
var
    dwFlag, dwFlagLen:DWORD;
begin
    dwFlagLen := SizeOf(dwFlag);
    if InternetQueryOption(hNet, INTERNET_OPTION_SECURITY_FLAGS, @dwFlag, dwFlagLen) then begin
        dwFlag := dwFlag or SECURITY_FLAG_IGNORE_REVOCATION;
        Result := InternetSetOption(hNet, INTERNET_OPTION_SECURITY_FLAGS, @dwFlag, sizeof(dwFlag));
    end else begin
        Result := True; //HTTP不能查询该选项
    end;
end;

function THttpClient.SetInternetTimeout(hNet: HINTERNET): Boolean;

begin
    Result := InternetSetOption(hNet, INTERNET_OPTION_CONNECT_TIMEOUT, @m_nConnectTimeOut, Sizeof(m_nConnectTimeOut))
          and InternetSetOption(hNet, INTERNET_OPTION_SEND_TIMEOUT, @m_nSendTimeOut, Sizeof(m_nSendTimeOut))
          and InternetSetOption(hNet, INTERNET_OPTION_RECEIVE_TIMEOUT, @m_nRecvTimeOut, Sizeof(m_nRecvTimeOut));
end;

class function THttpClient.URLEncode(const sUrl: UTF8String): string;
const
    NoConversion = ['A'..'Z', 'a'..'z', '*', '@', '#', '$', '.', '_', '-', ':', '/', '&', '=', '?'];
var
    Sp, Rp: PChar;
begin
    SetLength(Result, Length(sUrl) * 3);
    Sp := PChar(sUrl);
    Rp := PChar(Result);
    while Sp^ <> #0 do begin
      if Sp^ in NoConversion then begin
        Rp^ := Sp^;
      end else if Sp^ = ' ' then begin
        //Rp^ := '+'; 查资料有些表单的提交需要把空格变为 +
        FormatBuf(Rp^, 3, '%%%.2x', 6, [Ord(Sp^)]);
        Inc(Rp, 2);
      end else begin
        FormatBuf(Rp^, 3, '%%%.2x', 6, [Ord(Sp^)]);
        Inc(Rp, 2);
      end;
      Inc(Rp);
      Inc(Sp);
    end;
    SetLength(Result, Rp - PChar(Result));
end;

{ TUserCenterHtppThread }

function UserCenterProc(pfn:TUserCenterManager.PMethodWithParam):Integer;
var
    m:TMethod;
    objUc:TUserCenterManager;
begin
    if Assigned(pfn) then begin
        m.Code := pfn.Code;
        m.Data := pfn.Data;
        objUc := pfn.Data;
        TUserCenterManager.TUcProcesser(m)(pfn.sParam);
        Dispose(pfn);
    end else begin
        objUc := nil;
    end;
    if Assigned(objUc) then begin
        objUc.OnThreadFished();
    end;
    Result := 0;
    EndThread(Result);
end;

constructor TUserCenterManager.Create;
begin
    m_csThread := TCriticalSection.Create;
    Self.UcLoginMode := ulmAccount;
    StartWechatPolling();
end;

destructor TUserCenterManager.Destroy;
begin
    if m_msQrCode <> nil then begin
        m_msQrCode.Free;
    end;

    if m_WechatQrCode <> nil then begin
       m_WechatQrCode.Free;
       m_WechatQrCode := nil;
    end;

    SetLength(m_SubAccount.arrSubAccount, 0);

    StopWechatPolling();
    m_csThread.Free;
end;

function TUserCenterManager.GetSubAccountCount():Integer;
begin
    Result := Length(m_SubAccount.arrSubAccount);
end;

function TUserCenterManager.GetSubAccountItem(nIdx: Integer): PSubAccountItem;
begin
    if (nIdx >= 0) and (nIdx < Length(m_SubAccount.arrSubAccount)) then begin
        Result := @m_SubAccount.arrSubAccount[nIdx];
    end else begin
        Result := nil;
    end;
end;

function TUserCenterManager.GetRoleString(pRole:pRoleItem):string;
const
    Jobs:array[0..3-1] of Char = ('Z', 'F', 'D');
begin
    if pRole.sName <> '' then begin
        Result := Format('[%s%d %s]', [Jobs[pRole.nJob], pRole.nLevel, pRole.sName]);
    end else begin
        Result := '';
    end;
end;

function TUserCenterManager.GetSubAccountItemString(pItem:PSubAccountItem):string;
var
    sRoleInfo1, sRoleInfo2:string;
begin
    if pItem <> nil then begin
        sRoleInfo1 := GetRoleString(@pItem.arrRole[0]);
        sRoleInfo2 := GetRoleString(@pItem.arrRole[1]);

        if sRoleInfo1 <> '' then begin
            if (sRoleInfo2 <> '') then begin
                sRoleInfo1 := sRoleInfo1 + ' ' + sRoleInfo2;
            end;
            Result := Format('%-16s %s', [pItem.sSubAccount, sRoleInfo1]);
            if pItem.sOldGameZone <> '' then begin
                Result := Result + ' [' + pItem.sOldGameZone + ']';
            end;
        end else begin
            if pItem.sLastLoginTime <> '' then begin
                Result := Format('%-10s 最后登录时间: %s', [pItem.sSubAccount, pItem.sLastLoginTime]);
            end else begin
                Result := Format('%-10s', [pItem.sSubAccount]);
            end;
        end;
    end else begin
        Result := '';
    end;
end;

function TUserCenterManager.GetSubAccountString(nIdx: Integer): string;
var
    pItem:PSubAccountItem;
begin
    pItem := GetSubAccountItem(nIdx);
    Result := GetSubAccountItemString(pItem);
end;

function TUserCenterManager.GetCanRequestWechatQrCode:Boolean;
begin
    Result := InterlockedCompareExchange(m_nCanRequestWechatQrCode, 0, 0) = 0;
end;

procedure TUserCenterManager.DoUserCenterEvent(const msg: TMessage);
begin

end;

function TUserCenterManager.GetWechatQrCodeBusyStatus: Boolean;
begin
    Result := InterlockedCompareExchange(m_nWechatQrCodeBusyFlag, 0, 0) = 1;
end;

function TUserCenterManager.GetPhoneCodeBusyStatus():Boolean;
begin
    Result := InterlockedCompareExchange(m_nPhoneCodeBusyFlag, 0, 0) = 1;
end;

function TUserCenterManager.GetLoginBusyStatus():Boolean;
begin
    Result := InterlockedCompareExchange(m_nLoginBusyFlag, 0, 0) = 1;
end;

function TUserCenterManager.IsPhoneNumber(sPhone: string): Boolean;
var
    i, nCount:Integer;
begin
    Result := True;
    nCount := Length(sPhone);
    if (nCount = 11) and (sPhone[1] = '1') then begin
       for i := 2 to nCount do begin
           if not(sPhone[i] in ['0'..'9']) then begin
               Result := False;
               Break;
           end;
       end;
    end else begin
        Result := False;
    end;
end;

procedure TUserCenterManager.OnThreadFished;
begin
    m_csThread.Enter;
    try

    finally
        m_csThread.Leave;
    end;
end;

{$IFDEF USE_IDHTTP}
function TUserCenterManager.PostJsonData(sJson: string; IdHTTP:TIdHTTP; streamRequest, streamResponse:TStringStream):Boolean;
var
    sURL:string;
const
    //HTTP_POST_FORMLIST = 'application/x-www-form-urlencoded'; //charset=utf-8
    HTTP_POST_JSON = 'application/json;charset=UTF-8';
    //HTTP_POST_MULTIPART_FORMDATA = 'multipart/form-data';
    //HTTP_POST_TEXT_XML = 'text/xml';
begin
    Result := False;
        //m_sErrorMsg := '';
     idHttp.Request.CustomHeaders.Clear;
     streamRequest.Size := 0;
     streamResponse.Size := 0;
    try
        idHttp.Request.ContentType := HTTP_POST_JSON;
        //idHttp.Request.CustomHeaders.Add(Format('token:%s', [sToken]));   //AddPair('token', sToken); 一个是 '=', 一个是冒号
        //sURL := Format(HTTP_QUREY_FORMATTER, [g_sHttpPostUrl, sTimeStamp]); //timeStamp拼接到query
        sURL := AnsiToUtf8(m_sBaseUrl);
        streamRequest.WriteString(AnsiToUtf8(sJson)); //post body
        streamRequest.Position := 0;

        idHttp.Post(sURL, streamRequest, streamResponse);
        Result := True;
    except
        on e:Exception do begin
            m_sErrorMsg := e.Message;
        end;
    end;
    idHttp.Disconnect();
end;
{$ELSE}
function TUserCenterManager.PostJsonData(sJson: string; http:THttpClient; streamRequest, streamResponse:TStringStream):Boolean;
var
    sURL:string;
    sHeader:string;
const
    JSON_HEADER = 'Content-Type: application/json;charset=UTF-8' + #$D#$A;
begin
    Result := False;
    streamRequest.Size := 0;
    streamResponse.Size := 0;

    sURL := AnsiToUtf8(m_sBaseUrl);
    streamRequest.WriteString(AnsiToUtf8(sJson)); //post body
    streamRequest.Position := 0;
    sHeader := JSON_HEADER;

    if http.Post(sURL, sHeader, streamRequest.DataString, streamResponse) then begin 
        Result := True;
    end; 
end;
{$ENDIF}

{
SuperObject测试代码
var
    so:ISuperObject;
begin
    so := TSuperObject.Create();
    so.S['ID'] := 'ididid';
    so['Student'] := SuperObject.SO();
    so['Student'].S['Phone'] := '13617109875';
    so['Student'].S['Name'] := '张三';
    so['Student'].I['Age'] := 14;
    Memo1.Text := so.AsString;
    so := nil;
end;
}

procedure TUserCenterManager.RequestWechatQrcode(nCid:Integer);
var
    pm:PMethodWithParam;
    jo:ISuperObject;
    hThread:THandle;
    nThreadID:Cardinal;
begin
    if InterlockedCompareExchange(m_nWechatQrCodeBusyFlag, 1, 0) = 0 then begin
        InterlockedExchange(m_nCanRequestWechatQrCode, 1);

        jo := TSuperObject.Create();
        jo.I['cmd'] := UC_CMD_REQUEST_WECHAT_CODE;
        jo.I['tc'] := 0;
        jo.I['cid'] := nCid;

        pm := New(PMethodWithParam);
        pm^.Code := @TUserCenterManager.RequestWechatQrcodeProc; //当@用在类的方法中时,则方法的名称必须有类名
        pm^.Data := Self;
        pm^.sParam := jo.AsString;
        hThread := BeginThread(nil, 0, @UserCenterProc, Pointer(pm), 0, nThreadID);
        CloseHandle(hThread);

        jo := nil;
    end;
end;

procedure TUserCenterManager.RequestWechatQrcodeProc(sParam:string);
var
    httpPost:{$IFDEF USE_IDHTTP}TidHttp{$ELSE}THttpClient{$ENDIF};
    streamRequest:TStringStream;
    streamResponse: TStringStream;
    jo:ISuperObject;
    code:Integer;
    sQrcodeUrl:string;
    http:THttpClient;
    msData:TMemoryStream;
begin
    streamRequest := TStringStream.Create('');
    streamResponse := TStringStream.Create('');
    {$IFDEF USE_IDHTTP}
    httpPost := TidHttp.Create(nil);
    httpPost.ConnectTimeout := 3 * 1000; //走的内网通道
    httpPost.ReadTimeout := 5 * 1000;
    {$ELSE}
     httpPost := THttpClient.Create;
     httpPost.ConnectTimeOut := 3 * 1000;
     httpPost.RecvTimeOut := 5 * 1000;
     httpPost.SendTimeOut := 3 * 1000;
    {$ENDIF};
    try
        if PostJsonData(sParam, httpPost, streamRequest, streamResponse) then begin
            if httpPost.ResponseCode = 200 then begin
                jo := SO(Utf8ToAnsi(streamResponse.DataString));
                code := jo.I['code'];
                if code = 100200 then begin
                    sQrCodeUrl := jo.S['qrcodeUrl'];
                    http := THttpClient.Create;
                    msData := TMemoryStream.Create;
                    if http.Get(sQrcodeUrl, msData) and (http.ResponseCode = 200) then begin
                       Lock;
                       if m_msQrCode <> nil then m_msQrCode.Free;
                       m_msQrCode := msData;
                       m_sWechatToken := jo.S['token'];
                       Unlock;
                        PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_WEICHAT_CODE_RET, 0);
                    end else begin
                        msData.Free;
                        ResetWechatQrCode();
                        PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_WEICHAT_CODE_RET, code);
                    end;
                    http.Free;
                end else begin
                    ResetWechatQrCode();
                    PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_WEICHAT_CODE_RET, code);
                end;
                jo := nil;
            end else begin
                ResetWechatQrCode();
                PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_WEICHAT_CODE_RET, httpPost.ResponseCode);
            end;
        end else begin
            ResetWechatQrCode();
            PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_WEICHAT_CODE_RET, -1);
        end;
    finally
        httpPost.Free;
        streamRequest.Free;
        streamResponse.Free;
        InterlockedExchange(m_nWechatQrCodeBusyFlag, 0)
    end;
end;

procedure TUserCenterManager.ResetWechatQrCode;
begin
    Lock;
    try
        m_sWechatToken := '';
        FreeAndNil(m_msQrCode);
        if m_WechatQrCode <> nil then begin
            m_WechatQrCode.Free;
            m_WechatQrCode := nil;
        end;
        InterlockedExchange(m_nCanRequestWechatQrCode, 0); //清零准备下一次请求
    finally
        Unlock;
    end;
end;

procedure TUserCenterManager.RequestPhoneVerifyCode(sPhone:string; nCid:Integer);
var
    pm:PMethodWithParam;
    jo:ISuperObject;
    hThread:THandle;
    nThreadID:Cardinal;
begin
    if InterlockedCompareExchange(m_nPhoneCodeBusyFlag, 1, 0) = 0 then begin
        m_dwLastGetPhoneCodeTick := GetTickCount;

        jo := TSuperObject.Create();
        jo.I['cmd'] := UC_CMD_REQUEST_PHONE_CODE;
        jo.I['tc'] := 0;
        jo.I['cid'] := nCid;
        jo.S['mn'] := sPhone;

        pm := New(PMethodWithParam);
        pm^.Code := @TUserCenterManager.RequestPhoneVerifyCodeProc; //当@用在类的方法中时,则方法的名称必须有类名
        pm^.Data := Self;
        pm^.sParam := jo.AsString;
        hThread := BeginThread(nil, 0, @UserCenterProc, Pointer(pm), 0, nThreadID);
        CloseHandle(hThread);

        jo := nil;
    end;
end;

procedure TUserCenterManager.RequestPhoneLogin(sPhone:string; sVerfiyCode:string; nCid:Integer);
var
    pm:PMethodWithParam;
    jo:ISuperObject;
    hThread:THandle;
    nThreadID:Cardinal;
begin
    if InterlockedCompareExchange(m_nLoginBusyFlag, 1, 0) = 0 then begin
        m_dwLastGetPhoneCodeTick := GetTickCount;

        m_sAccount := sPhone;

        jo := TSuperObject.Create();
        jo.I['cmd'] := UC_CMD_PHONE_LOGIN;
        jo.I['tc'] := 0;
        jo.I['cid'] := nCid;
        jo.S['mn'] := sPhone;
        jo.S['vc'] := sVerfiyCode;
        Lock;
        jo.S['token'] := m_sPhoneToken;
        Unlock;

        pm := New(PMethodWithParam);
        pm^.Code := @TUserCenterManager.RequestPhoneLoginProc; //当@用在类的方法中时,则方法的名称必须有类名
        pm^.Data := Self;
        pm^.sParam := jo.AsString;
        hThread := BeginThread(nil, 0, @UserCenterProc, Pointer(pm), 0, nThreadID);
        CloseHandle(hThread);

        jo := nil;
    end;
end;

procedure TUserCenterManager.RequestPhoneVerifyCodeProc(sParam:string);
var
    httpPost:{$IFDEF USE_IDHTTP}TidHttp{$ELSE}THttpClient{$ENDIF};
    streamRequest:TStringStream;
    streamResponse: TStringStream;
    jo:ISuperObject;
    code:Integer;
begin
    streamRequest := TStringStream.Create('');
    streamResponse := TStringStream.Create('');
    {$IFDEF USE_IDHTTP}
    httpPost := TidHttp.Create(nil);
    httpPost.ConnectTimeout := 3 * 1000; //走的内网通道
    httpPost.ReadTimeout := 5 * 1000;
    {$ELSE}
    httpPost := THttpClient.Create;
    httpPost.ConnectTimeOut := 3 * 1000;
    httpPost.RecvTimeOut := 5 * 1000;
    httpPost.SendTimeOut := 3 * 1000;
    {$ENDIF};
    try
        if PostJsonData(sParam, httpPost, streamRequest, streamResponse) then begin
            if httpPost.ResponseCode = 200 then begin
                jo := SO(Utf8ToAnsi(streamResponse.DataString));
                code := jo.I['code'];
                if code = 100200 then begin
                    Lock;
                    m_sPhoneToken := jo.S['token'];
                    Unlock;
                    PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_PHONE_CODE_RET, 0);
                end else begin
                    if code <> 100405 then begin //100405请求过于频繁
                        PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_PHONE_CODE_RET, code);
                    end;
                end;
                jo := nil;
            end else begin
                PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_PHONE_CODE_RET, httpPost.ResponseCode);
            end;
        end else begin
            PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_PHONE_CODE_RET, -1);
        end;
    finally
        httpPost.Free;
        streamRequest.Free;
        streamResponse.Free;
        InterlockedExchange(m_nPhoneCodeBusyFlag, 0)
    end;
end;

procedure TUserCenterManager.RequestPhoneLoginProc(sParam:string);
var
    httpPost:{$IFDEF USE_IDHTTP}TidHttp{$ELSE}THttpClient{$ENDIF};
    streamRequest:TStringStream;
    streamResponse: TStringStream;
    jo:ISuperObject;
    code:Integer;
begin
    streamRequest := TStringStream.Create('');
    streamResponse := TStringStream.Create('');

    {$IFDEF USE_IDHTTP}
    httpPost := TidHttp.Create(nil);
    httpPost.ConnectTimeout := 2 * 1000; //走的内网通道
    httpPost.ReadTimeout := 3 * 1000;
    {$ELSE}
    httpPost := THttpClient.Create;
    httpPost.ConnectTimeOut := 2 * 1000;
    httpPost.RecvTimeOut := 3 * 1000;
    httpPost.SendTimeOut := 2 * 1000;
    {$ENDIF};

    try
        if PostJsonData(sParam, httpPost, streamRequest, streamResponse) then begin
            if httpPost.ResponseCode = 200 then begin
                jo := SO(Utf8ToAnsi(streamResponse.DataString));
                code := jo.I['code'];
                if code = 100200 then begin
                    UcLoginMode := ulmNone;
                    Lock;
                    m_nUID := jo.I['uid'];
                    m_sSession := jo.S['sid'];
                    Unlock;
                    PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_LOGIN_RET, 0);
                end else begin
                    PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_LOGIN_RET, code);
                end;
                jo := nil;
            end else begin
                PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_LOGIN_RET, 100404{IdHTTP.ResponseCode});
            end;
        end else begin
            PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_LOGIN_RET, -1);
        end;
    finally
        httpPost.Free;
        streamRequest.Free;
        streamResponse.Free;
        InterlockedExchange(m_nLoginBusyFlag, 0);
    end;
end;

procedure TUserCenterManager.RequestAccountLoginProc(sParam:string);
var
    httpPost:{$IFDEF USE_IDHTTP}TidHttp{$ELSE}THttpClient{$ENDIF};
    streamRequest:TStringStream;
    streamResponse: TStringStream;
    code:Integer;
    jo:ISuperObject;
begin
    streamRequest := TStringStream.Create('');
    streamResponse := TStringStream.Create('');
    {$IFDEF USE_IDHTTP}
    httpPost := TidHttp.Create(nil);
    httpPost.ConnectTimeout := 5 * 1000; //走的内网通道
    httpPost.ReadTimeout := 5 * 1000;
    {$ELSE}
    httpPost := THttpClient.Create;
    httpPost.ConnectTimeOut := 5 * 1000;
    httpPost.RecvTimeOut := 5 * 1000;
    httpPost.SendTimeOut := 5 * 1000;
    {$ENDIF};
    try
        if PostJsonData(sParam, httpPost, streamRequest, streamResponse) then begin
            if httpPost.ResponseCode = 200 then begin
                jo := SO(Utf8ToAnsi(streamResponse.DataString));
                code := jo.I['code'];
                if code = 100200 then begin
                    UcLoginMode := ulmNone;
                    Lock;
                    m_nUID := jo.I['uid'];
                    m_sSession := jo.S['sid'];
                    Unlock;
                    PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_LOGIN_RET, 0);
                end else begin
                    PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_LOGIN_RET, code);
                end;
                jo := nil;
            end else begin
                PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_LOGIN_RET, 100404{IdHTTP.ResponseCode});
            end;
        end else begin
            PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_LOGIN_RET, -1);
        end;
    finally
        httpPost.Free;
        streamRequest.Free;
        streamResponse.Free;
        InterlockedExchange(m_nLoginBusyFlag, 0)
    end;
end;

procedure TUserCenterManager.RequestAccountLogin(sAccount, sPassword: string; nCid:Integer);
var
    pm:PMethodWithParam;
    jo:ISuperObject;
    hThread:THandle;
    nThreadID:Cardinal;
    md:MD5Digest;
begin
    if InterlockedCompareExchange(m_nLoginBusyFlag, 1, 0) = 0 then begin
        m_dwLastGetPhoneCodeTick := GetTickCount;
        m_sAccount := sAccount; //设置账号  
        md := MD5Util.MD5String(AnsiToUtf8(sPassword));
        sPassword := MD5Util.MD5Print(md);

        jo := TSuperObject.Create();
        jo.I['cmd'] := UC_CMD_ACCOUNT_LOGIN;
        jo.I['tc'] := 0;
        jo.I['cid'] := nCid;
        jo.S['u'] := sAccount;
        jo.S['p'] := sPassword;

        pm := New(PMethodWithParam);
        pm^.Code := @TUserCenterManager.RequestAccountLoginProc; //当@用在类的方法中时,则方法的名称必须有类名
        pm^.Data := Self;
        pm^.sParam := jo.AsString;
        hThread := BeginThread(nil, 0, @UserCenterProc, Pointer(pm), 0, nThreadID);
        CloseHandle(hThread);

        jo := nil;
    end;
end;

procedure TUserCenterManager.ReadSubAccountFromJson(joRet:ISuperObject);
var
    i, j, nCount, nRoles:Integer;
    jo:ISuperObject;
    joRoles:TSuperArray;
    pItem:PSubAccountItem;
begin
    nCount := joRet.A['subAccountItems'].Length;
    SetLength(m_SubAccount.arrSubAccount, nCount);
    for i := 0 to nCount - 1 do begin
        jo := joRet.A['subAccountItems'].O[i];
        pItem := @m_SubAccount.arrSubAccount[i];
        pItem.sSubAccount := jo.s['accountName'];
        pItem.sOldGameZone:= jo.S['oldGameName'];
        pItem.sLastLoginTime := jo.S['lastLoginTime'];

        joRoles := jo.A['roles'];
        nRoles := joRoles.Length;
        if nRoles > 2 then nRoles := 2;

        ZeroMemory(@pItem.arrRole, Sizeof(pItem.arrRole));
        for j := 0 to nRoles - 1 do begin
            jo := joRoles.O[j];
            pItem.arrRole[j].sName :=  jo.S['roleName'];
            pItem.arrRole[j].nJob :=  jo.I['roleJob'];
            pItem.arrRole[j].nLevel :=  jo.I['roleLevel'];
        end; 
    end;
end;

procedure TUserCenterManager.RequestSubAccountProc(sParam:string);
var
    httpPost:{$IFDEF USE_IDHTTP}TidHttp{$ELSE}THttpClient{$ENDIF};
    streamRequest:TStringStream;
    streamResponse: TStringStream;
    code:Integer;
    jo:ISuperObject;
    sJson:string;
begin
    streamRequest := TStringStream.Create('');
    streamResponse := TStringStream.Create('');
    {$IFDEF USE_IDHTTP}
    httpPost := TidHttp.Create(nil);
    httpPost.ConnectTimeout := 3 * 1000; //走的内网通道
    httpPost.ReadTimeout := 5 * 1000;
    {$ELSE}
    httpPost := THttpClient.Create;
    httpPost.ConnectTimeOut := 3 * 1000;
    httpPost.RecvTimeOut := 5 * 1000;
    httpPost.SendTimeOut := 3 * 1000;
    {$ENDIF};
    try
        if PostJsonData(sParam, httpPost, streamRequest, streamResponse) then begin
            if httpPost.ResponseCode = 200 then begin
                sJson := Utf8ToAnsi(streamResponse.DataString);
                jo := SO(sJson);
                code := jo.I['code'];
                if code = 100200 then begin
                    Lock();
                    ReadSubAccountFromJson(jo);
                    Unlock();
                    PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_SUB_ACCOUNT_RET, 0);
                end else begin
                    PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_SUB_ACCOUNT_RET, code);
                end;
                jo := nil;
            end else begin
                PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_SUB_ACCOUNT_RET, 100404{IdHTTP.ResponseCode});
            end;
        end else begin
            PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_SUB_ACCOUNT_RET, -1);
        end;
    finally
        httpPost.Free;
        streamRequest.Free;
        streamResponse.Free;
        InterlockedExchange(m_nLoginBusyFlag, 0)
    end;
end;

procedure TUserCenterManager.RequestSubAccountList(nCid:Integer; nGid:Integer);
var
    pm:PMethodWithParam;
    jo:ISuperObject;
    hThread:THandle;
    nThreadID:Cardinal;
begin
    jo := TSuperObject.Create();
    jo.I['cmd'] := UC_CMD_REQUEST_SUB_ACCOUNT;
    jo.I['tc'] := 0;
    jo.I['cid'] := nCid;
    jo.I['gid'] := g_nUcGameId;
    jo.B['aca'] := true; //Auto Create game Account if not exists
    Lock;
    jo.I['uid'] := m_nUID;
    jo.S['sid'] := m_sSession;
    Unlock;

    pm := New(PMethodWithParam);
    pm^.Code := @TUserCenterManager.RequestSubAccountProc; //当@用在类的方法中时,则方法的名称必须有类名
    pm^.Data := Self;
    pm^.sParam := jo.AsString;
    hThread := BeginThread(nil, 0, @UserCenterProc, Pointer(pm), 0, nThreadID);
    CloseHandle(hThread); 

    jo := nil;
end;

procedure TUserCenterManager.SetHostInfo(sHost: string; nPort: Integer);
begin
    m_sBaseUrl := Format('http://%s:%d', [sHost, nPort]);
end;

function TUserCenterManager.GetLoginMode: TUcLoginMode;
begin
    Result := TUcLoginMode(InterlockedCompareExchange(m_nLoginMode, 0, 0));
end;

procedure TUserCenterManager.SetLoginMode(nMode: TUcLoginMode);
begin
    InterlockedExchange(m_nLoginMode, Ord(nMode));
end; 

(*
procedure TUserCenterManager.SetSubAccountTestData;
begin
    SetLength(m_arrSubAccount, 5);
    m_arrSubAccount[0].sSubAccount := 'olddragon';
    m_arrSubAccount[0].sRole1 := '天生神力';
    m_arrSubAccount[0].sRole2 := '神迹';
    m_arrSubAccount[0].nRoleLvl := 45;
    m_arrSubAccount[0].nRoleLv2 := 55;
    m_arrSubAccount[0].nRoleJob1 := 0;
    m_arrSubAccount[0].nRoleJob2 := 1;

    m_arrSubAccount[1].sSubAccount := 'olddragon';
    m_arrSubAccount[1].sRole1 := '兰剑B';
    m_arrSubAccount[1].sRole2 := '老山反击战';
    m_arrSubAccount[1].nRoleLvl := 50;
    m_arrSubAccount[1].nRoleLv2 := 40;
    m_arrSubAccount[1].nRoleJob1 := 2;
    m_arrSubAccount[1].nRoleJob2 := 0;

    m_arrSubAccount[2].sSubAccount := 'olddragon';
    m_arrSubAccount[2].sRole1 := '天生智障';
    m_arrSubAccount[2].sRole2 := '十二生肖';
    m_arrSubAccount[2].nRoleLvl := 45;
    m_arrSubAccount[2].nRoleLv2 := 55;
    m_arrSubAccount[2].nRoleJob1 := 1;
    m_arrSubAccount[2].nRoleJob2 := 2;

    m_arrSubAccount[3].sSubAccount := 'olddragon';
    m_arrSubAccount[3].sRole1 := '反重力';
    m_arrSubAccount[3].sRole2 := '神州七号';
    m_arrSubAccount[3].nRoleLvl := 45;
    m_arrSubAccount[3].nRoleLv2 := 55;
    m_arrSubAccount[3].nRoleJob1 := 1;
    m_arrSubAccount[3].nRoleJob2 := 0;

    m_arrSubAccount[4].sSubAccount := 'olddragon';
    m_arrSubAccount[4].sRole1 := '九月天';
    m_arrSubAccount[4].sRole2 := '好风飘';
    m_arrSubAccount[4].nRoleLvl := 45;
    m_arrSubAccount[4].nRoleLv2 := 55;
    m_arrSubAccount[4].nRoleJob1 := 2;
    m_arrSubAccount[4].nRoleJob2 := 1;
end;
*)

procedure TUserCenterManager.WechatPollingProc(sParam:string);
var
    httpPost:{$IFDEF USE_IDHTTP}TidHttp{$ELSE}THttpClient{$ENDIF};
    streamRequest:TStringStream;
    streamResponse: TStringStream;
    jo, joSend:ISuperObject;
    code:Integer;
    sToken:string;
    dwCurrTick, dwLastTick:DWORD;
begin
    streamRequest := TStringStream.Create('');
    streamResponse := TStringStream.Create('');
    {$IFDEF USE_IDHTTP}
    httpPost := TidHttp.Create(nil);
    httpPost.ConnectTimeout := 3 * 1000; //走的内网通道
    httpPost.ReadTimeout := 5 * 1000;
    {$ELSE}
    httpPost := THttpClient.Create;
    httpPost.ConnectTimeOut := 3 * 1000;
    httpPost.RecvTimeOut := 5 * 1000;
    httpPost.SendTimeOut := 3 * 1000;
    {$ENDIF};
    joSend := TSuperObject.Create();
    dwLastTick := 0;
    try
        while InterlockedCompareExchange(m_nWechatRequestExitFlag, 0, 0) = 0 do begin
            dwCurrTick := GetTickCount();
            if dwLastTick = 0 then dwLastTick := dwCurrTick;

            Lock;
            sToken := m_sWechatToken;
            Unlock;

            if (UcLoginMode = ulmWechat) and (sToken <> EmptyStr) then begin
                if dwCurrTick - dwLastTick > 2000 then begin
                    joSend.I['cmd'] := UC_CMD_WECHAT_LOGIN;
                    joSend.I['tc'] := 0;
                    joSend.I['cid'] := 0;
                    joSend.S['token'] := sToken;
                    if PostJsonData(joSend.AsString, httpPost, streamRequest, streamResponse) then begin
                        if httpPost.ResponseCode = 200 then begin
                            jo := SO(Utf8ToAnsi(streamResponse.DataString));
                            code := jo.I['code'];
                            if code = 100200 then begin
                                UcLoginMode := ulmNone; //
                                Lock();
                                m_nUID := jo.I['uid'];
                                m_sSession := jo.S['sid'];
                                m_sAccount := Format('wechat_u%d', [m_nUID]);
                                Unlock();
                                PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_LOGIN_RET, 0);
                            end else begin
                                if (code = 100202) or (code = 100203) then begin
                                    ResetWechatQrCode();
                                end;
                                PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_LOGIN_RET, code);
                            end;
                            jo := nil;
                        end else begin
                            PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_LOGIN_RET, httpPost.ResponseCode);
                        end;
                    end else begin
                        PostMessage(Self.MainFormMsgHandle, MSG_USER_CENTER_NOTIFY, UC_LOGIN_RET, -1);
                    end; 
                    dwLastTick := dwCurrTick;
                end;
            end;

            Sleep(100);
        end;
    finally
        joSend := nil;
        httpPost.Free;
        streamRequest.Free;
        streamResponse.Free;
    end;
end;

procedure TUserCenterManager.StartWechatPolling;
var
    pm:PMethodWithParam;
    nThreadID:Cardinal;
begin
    if m_hWechatPollingThread = 0 then begin
        m_nWechatRequestExitFlag := 0;
        pm := New(PMethodWithParam);
        pm^.Code := @TUserCenterManager.WechatPollingProc; //当@用在类的方法中时,则方法的名称必须有类名
        pm^.Data := Self;
        pm^.sParam := '';
        m_hWechatPollingThread := BeginThread(nil, 0, @UserCenterProc, Pointer(pm), 0, nThreadID);
    end;
end;

procedure TUserCenterManager.StopWechatPolling;
begin
    if m_hWechatPollingThread <> 0 then begin
        InterlockedExchange(m_nWechatRequestExitFlag, $FF);
        WaitForSingleObject(m_hWechatPollingThread, INFINITE);
        CloseHandle(m_hWechatPollingThread);
        m_hWechatPollingThread := 0;
    end;
end;

function TUserCenterManager.TryLock: Boolean;
begin
    Result := m_csThread.TryEnter;
end;

procedure TUserCenterManager.Lock;
begin
    m_csThread.Enter;
end;

procedure TUserCenterManager.Unlock;
begin
    m_csThread.Leave;
end;

procedure TUserCenterManager.UpdateWechatQrCode;
begin
    Lock;
    try
         if m_WechatQrCode <> nil then begin
             m_WechatQrCode.Free;
         end;
         if m_msQrCode <> nil then begin
             //m_WechatQrCode := CreateQRCodeTexture(m_sWechatQrCodeUrl, 160, clWhite);
             m_WechatQrCode := NewTexture(m_msQrCode.Memory, m_msQrCode.Size, 160, 160, clBlack, False);
             m_msQrCode.Free;
             m_msQrCode := nil;  //释放资源
         end;
    finally
        Unlock;
    end;
end;

initialization
  g_sSelfFileName := ParamStr(0);
  g_sSelfFilePath := ExtractFilePath(ParamStr(0));
  g_sSelfResourcePath := IncludeTrailingPathDelimiter(g_sSelfFilePath + g_ResourcesDir);

  {$IF IsMultiThreadRender = 1}
  InitializeCriticalSection(g_CriticalSection);
  InitializeCriticalSection(g_ActorLock);
  {$IFEND}
  g_AutoGJPoints := TList.Create;

  InitializeCriticalSection(g_LockDebugOutStr);
  InitializeCriticalSection(g_FreeMemorysLock);

  InitializeCriticalSection(g_SendSocketLock);
  g_SendStream := TMemoryStreamEx.Create();

  InitializeCriticalSection(g_AntiPlugDllMemoryModuleCS);
  InitializeCriticalSection(g_AntiPlugDllMemoryModuleCS2);
  InitializeCriticalSection(g_AntiPlugDllMemoryModuleCS_LEG);
  InitializeCriticalSection(g_AntiPlugDllMemoryModuleCS_IOCP2);
  g_sAdapterMac := GetAdapterMac(0);

  {
  if g_sAdapterMac <> '' then
    g_sMachineID := RivestStr(g_sAdapterMac + IntToStr(20120618))；
  try
    g_sMachineID := GetHwid2; // RivestStr(GetIdeSerialNumber + IntToStr(20120618) + GetCpuIDstringEx + GetAdapterMac(0));
  except
    ShowMessage('GetCPUID error');
  end;
  }

  g_CustomMonsterConfig := TList.Create;
  g_CustomMagicConfig := TList.Create;
  g_CustomNpcConfig := TList.Create;
  g_DropItemEffectList := TDropItemEffectList.Create;
  g_EnabledAuctionItemList := TStringList.Create;

  g_NGProtectItems := TStringList.Create;

  g_NGProtectItems.Add('回城卷');
  g_NGProtectItems.Add('随机传送卷');
  g_NGProtectItems.Add('地牢逃脱卷');
  g_NGProtectItems.Add('行会回城卷');
  g_NGProtectItems.Add('盟重传送石');
  g_NGProtectItems.Add('比奇传送石');
  g_NGProtectItems.Add('随机传送石');
  g_NGProtectItems.Add('还击');
  g_NGProtectItems.Add('小退');

  g_NGBossList := TStringList.Create;

  g_ItemDescList := TGHashStringList.Create; // 物品备注
  g_ItemDescTopList := TGHashStringList.Create; // 物品备注
  g_TzItemDescList := TGStringList.Create; // 套装备注
  g_GodBlessItemList := TGHashStringList.Create;

  g_UpdateSizeLock := TCriticalSection.Create;

  {$IF DEBUG_RUN_DELAY_SELF = 1}
  g_SelfRunSendMsg := TStringList.Create;
  g_SelfRunRecvMsg := TStringList.Create;
  g_SelfRunStart := TStringList.Create;
  g_SelfRunEnd := TStringList.Create;
  {$IFEND}
  {$IF DEBUG_RUN_DELAY_OTHER = 1}
  g_OtherRunStart := TStringList.Create;
  g_OtherRunMsg := TStringList.Create;
  g_OtherRunEnd := TStringList.Create;
  {$IFEND}
  {$IF DEBUG_HIT_DELAY = 1}
  g_SelfHitStart := TStringList.Create;
  g_SelfHitEnd := TStringList.Create;
  {$IFEND}
  {$IF DEBUG_SPELL_DELAY = 1}
  g_SelfSpellStart := TStringList.Create;
  g_OtherSpellStart := TStringList.Create;
  {$IFEND}

  g_BossList := THashedStringList.Create;
  g_GJMonList := THashedStringList.Create;
  g_GJUseMagic1 := TList.Create;
  g_GJUseMagic2 := TList.Create;

  g_SaveMyBagItemList := TList.Create;

  g_ProcessBlackList := THashedStringList.Create;
  g_CustomItemPropertyTextVarList := TStringList.Create;
  g_WarrContinueHitManager := TWarrContinueHitManager.Create;

  g_MoneyList := TQuickList.Create;
  FillChar(g_EatItemCDConfig, SizeOf(g_EatItemCDConfig), 0);

finalization
  g_SendStream.Free;

  g_AutoGJPoints.Free;

  {$IF IsMultiThreadRender = 1}
  DeleteCriticalSection(g_CriticalSection);
  DeleteCriticalSection(g_ActorLock);
  {$IFEND}
  DeleteCriticalSection(g_FreeMemorysLock);
  DeleteCriticalSection(g_SendSocketLock);
  DeleteCriticalSection(g_LockDebugOutStr);

  ClearCustomMonsterConfig;
  g_CustomMonsterConfig.Free;

  ClearCustomMagicConfig;
  g_CustomMagicConfig.Free;

  ClearCustomNpcConfig;
  g_CustomNpcConfig.Free;
  g_DropItemEffectList.Free;

  Clear_g_EnabledAuctionItemList;
  g_EnabledAuctionItemList.Free;

  g_NGProtectItems.Free;
  g_NGBossList.Free;

  g_ItemDescList.Free;
  g_ItemDescTopList.Free;
  g_TzItemDescList.Free;
  UnLoadGodBlessItemList;

  g_UpdateSizeLock.Free;

  {$IF DEBUG_RUN_DELAY_SELF = 1}
  g_SelfRunSendMsg.Free;
  g_SelfRunRecvMsg.Free;
  g_SelfRunStart.Free;
  g_SelfRunEnd.Free;
  {$IFEND}
  {$IF DEBUG_RUN_DELAY_OTHER = 1}
  g_OtherRunStart.Free;
  g_OtherRunMsg.Free;
  g_OtherRunEnd.Free;
  {$IFEND}
  {$IF DEBUG_HIT_DELAY = 1}
  g_SelfHitStart.Free;
  g_SelfHitEnd.Free;
  {$IFEND}

  g_BossList.Free;
  g_GJMonList.Free;
  g_GJUseMagic1.Free;
  g_GJUseMagic2.Free;

  g_SaveMyBagItemList.Free;

  g_ProcessBlackList.Free;

  g_CustomItemPropertyTextVarList.Free;
  g_WarrContinueHitManager.Free;
  g_MoneyList.Free;
  //  FreeAndNil(g_MoneyList);
  FreeAntiPlugDllMemoryModule;

  DeleteCriticalSection(g_AntiPlugDllMemoryModuleCS);
  DeleteCriticalSection(g_AntiPlugDllMemoryModuleCS2);
  DeleteCriticalSection(g_AntiPlugDllMemoryModuleCS_LEG);
  DeleteCriticalSection(g_AntiPlugDllMemoryModuleCS_IOCP2);

end.

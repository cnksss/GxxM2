unit Grobal2_Ex;

interface

uses
  Windows, Classes, SysUtils, MD5Util;

const
  // 加载客户端反外挂模块
  CLIENT_ANTIPLUG = 1;

  // 0: 无dll版本
  // 1: 定制带广告、Dll版
  // 2: 非定制无广告带dll版本
  VERSION_TYPE = 2;

  // 是否发布注册版本
  NEED_REGISTER = 1;

  // 是否为测试注册版本
  REGISTER_TEST = 0;

  // 是否多线程来运行客户端数据
  MultiThreadRunContext = 1;

{$IF NEED_REGISTER = 0}
  LOG_PLUG_DATA = 1;
{$ELSE}
  LOG_PLUG_DATA = 0;
{$IFEND}

  RungateLEG_IOCP = 6; //1;

const
  DEFBLOCKSIZE = 22;

  RUN_GATE_MSG_CODE = $AABBCCDD;

  MAX_UPLOAD_PICKITEMS = 10000;       // 最多内核上传物品数据限制 2020-03-09 00:47:24

{ ------------------------来自于Common中的定义-------------------------- }
const
  // 服务器模块之间
  SG_CHECKCODEADDR = 1006;
  GS_QUIT = 2000;                                 // 关闭
  SG_FORMHANDLE = 1000;                           // 服务器HANLD
  SG_STARTNOW = 1001;                             // 正在启动服务器...
  SG_STARTOK = 1002;                              // 服务器启动完成...
  SG_ACTIVE = 1003;

const
  CM_SENDSELLGAMEGOLDDALITEM = 102;               // 元宝交易装备


  //CM_QUERYUSERSHOPS = 109;                      // 搜索店铺名称
  CM_QUERYUSERSHOPITEMS = 111;                    // 搜索指定用户店铺物品
  CM_SEARCHSHOPITEMS = 113;                       // 搜索用户店铺物品
  CM_SENDBUYUSERSHOPITEM = 124;                   // 购买别人店铺的物品
  CM_HEROTAKEONITEM = 155;                        // 英雄穿装备

  CM_SENDUSERSPEEDING: Word = 126;                // 126;   // 用户超速
  CM_RUNGATE_SENDFILTERMSG: Word = 174;           // 174;


  CM_AUTOEAT = 170;
  CM_HEROEAT = 157;                               // 英雄吃药

  CM_RUNGATE_HEARTBEAT = 500;                     // 客户端发向网关的心跳

  CM_TAKEONITEM = 1003;                           // 穿戴装备
  CM_EAT = 1006;                                  // 吃药 52;

  CM_MERCHANTDLGSELECT = 1011;                    // NPC标签点击等

  CM_LOGINNOTICEOK = 1018;                        // 健康游戏忠告点了确实,进入游戏 62;

  CM_GUILDUPDATENOTICE = 1040;                    // 修改行会公告 84;
  CM_GUILDUPDATERANKINFO = 1041;                  // 更新联盟信息(取消或建立联盟) 85;

  CM_113HIT = 3113;                               // 断空斩 chongchong 2018-01-29
  CM_115HIT = 3115;                               // 血魂一击(战) chongchong 2018-01-30

  CM_CLIENTDATAFILE = 5120;                       // 请求相关资源 chongchong 2015-01-07
  
  //CM_DEALCANCEL = 1028;                         // 取消交易 72;
  CM_RUNGATE_CHECK_INFO = 5249;

  CM_SENDPROCESS_LIST = 5250;
  CM_SENDSCREENSHOT_GAME = 5251;
  CM_SENDSCREENSHOT = 5252;

  CM_ANTIPLUG_LOAD = 5255;
  CM_RECVRUNGATE_CHECKCODE = 5257;                // 客户端返回网关发的验证码
  CM_DISABLECONNECT = 5266;

  CM_RUNGATEDOOR = 5267;

  CM_TradingBUYITEM = 5277;                                                                         // 用户在交易市场买入东西 59;
  CM_TradingSELLITEMS = 5278;                                                                       // 用户在交易市场卖东西
  
  { ------------记录客户端日志用-------- }
  CM_PLUGINCONFIG = 5000;                         // 向服务器发送内挂配置信息 chongchong 2013-08-17

  CM_GETRUNGATEVERIFYCODE = 5268;
  CM_CHECKRUNGATEVERIFYCODE = 5269;
  CM_SENDUSERVERIFYFAIL = 5270;
  CM_SENDCHECKPLUGIN = 5274;                      // 插件检测到非法外挂

  CM_IOCP_SENDSCREENSHOT_GAME = 5300;
  CM_IOCP_SENDSCREENSHOT = 5301;
  CM_IOCP_SENDPROCESS_LIST = 5302;
  CM_IOCP_SENDPROCESS_LIST2 = 5303;
 
  CM_IOCP_SENDDIR_LIST = 5304;
  CM_IOCP_RESPONSE_FILE = 5305;

  CM_IOCP_APPEXIT = 5306;                         // 通知IOCP网关大退
  CM_IOCP_ANTIPLUG_CRC = 5307;                    // 客户端保留插件数据的CRC
  
  CM_IOCP_RESPONSE_FILE2 = 5310;

  CM_REQUEST_UPDATE_DLL = 5321;
  CM_UPDATE_RECV = 5322;

  CM_IOCP_PLUG_LOAD_FAIL = 5323;

  CM_UPLOAD_PICK_ITMES = 5333;

  CM_CUSTOM_HIT001 = 6000;

const
  SM_NEWMAP = 51;
  SM_NEWLINEMESSAGE = 96;                         // 换行消息 piaoyun 2013-08-03
  SM_SUPERMOVEMESSAGE = 97;                       // 仿盛大顶部渐隐消息 piaoyun 2013-08-01
  SM_SCREENMESSAGE = 98;  
  SM_MOVEMESSAGE = 99;                            // 1179;
  SM_SYSMESSAGE = 100;                            // 100;   // 系统消息,盛大一般红字,私服蓝字 1174;

  SM_ACTION_RET = 110;                            // '#+GOOD!' / '#+FAIL!'
  SM_RUNGATE_HEARTBEAT = 112;                     // 网关发往客户端心跳

  SM_MENU_OK = 767;                               // 767;   // ? 1286; //?
  SM_CHANGESPEED = 1369;                          // 1369;  // 游戏速度
  SM_LOCK_USER = 8956;                            // 8956;  // 锁定用户
  SM_MAGICFIRE_FAIL = 639;                        // 1210;
  SM_TOPCHATBOARDMESSAGE = 1182;

  SM_HEROABILITY = 1466;                          // 获取英雄Abil

  SM_ADDMAGIC = 210;
  SM_SENDMYMAGIC = 211;

  SM_ADDITEM = 200;                               // 1183;
  SM_BAGITEMS = 201;                              // 1184;
  SM_DELITEM = 202;                               // 1185;
  SM_DROPITEM_SUCCESS = 600;
  SM_DELITEMS = 709;                              // 1265;

  SM_TAKEON_FAIL = 616;                           // 穿装备失败

  SM_EAT_OK = 635;                                // 1206;

  SM_AUTOEAT_OK = 1511;                           // 自动吃药成功
  SM_AUTOEAT_FAIL = 1512;                         // 自动吃药失败

  SM_HEROTAKEON_FAIL = 1474;                      // 英雄穿装备FAIL

  SM_HEROEAT_OK = 1477;                           // 英雄吃药OK
  SM_HEROAUTOEAT_OK = 1513;                       // 英雄自动吃药成功

  SM_HEROBAGITEMS = 1468;                         // 获取英雄包裹     Tag:包裹物品数量 2 Series: 包裹总数量10
  SM_HEROADDITEM = 1471;                          // 英雄 Ident: 905 Recog: 738569296 Param: 0 Tag: 0 Series: 1   AddItem
  SM_HERODELITEM = 1472;                          // 英雄 Ident: 906 Recog: 738569296 Param: 0 Tag: 0 Series: 1   delItem
  SM_HERODELITEMS = 1492;                         // 删除英雄物品 1492;
  SM_HERODROPITEM_SUCCESS = 1484;                 // 英雄扔物品OK

  SM_MASTERBAGTOHEROBAG_OK = 1458;                // 主人包裹物品放到英雄包裹成功
  SM_HEROBAGTOMASTERBAG_OK = 1460;                // 英雄包裹物品放到主人包裹成功

  SM_CHECK_RUNGATE1 = 5008;                       // ★★★★★★★★★★★★★★★★★★★★★★★★★验证网关1
  SM_SENDDROPITEMEFFECTLIST = 8974;
  SM_SENDDROPITEMEFFECTLIST_CACHE   = 10117;

  SM_CHECK_RUNGATE2 = 5040;                       // ★★★★★★★★★★★★★★★★★★★★★★★★★验证网关2

  
  SM_ITEMEAT_CDTIME = 10270;                      // 吃药间隔 chongchong 2016-10-26
  SM_RUNGATE_VERIFYCODE = 10271;
  SM_RUNGATE_VERIFYCODE_CHECK_RET = 10272;
  SM_RUNGATE_SPEED_INTERVALS = 10277;
  SM_RUNGATE_DISABLE_LOGIN = 10278;

  SM_ANTIPLUGSTREAM_2 = 10317;
  SM_IOCP_ANTIPLUGSTREAM_CACHE_2 = 10318;
  SM_CONTINUEANTIPLUGSTREAM_2 = 10319;

  SM_IOCP_GETSCREENSHOT_GAME = 10400;
  SM_IOCP_GETSCREENSHOT = 10401;
  SM_IOCP_GETPROCESS_LIST = 10402;
  SM_IOCP_GETDIR_LIST = 10403;
  SM_IOCP_REQUEST_FILE = 10404;
  SM_IOCP_GETPROCESS_LIST2 = 10405;


  SM_ANTIPLUGSTREAM_IOCP2 = 10423;
  SM_IOCP_ANTIPLUGSTREAM_CACHE_IOCP2 = 10424;
  SM_CONTINUEANTIPLUGSTREAM_IOCP2 = 10425;
  SM_IOCP_ANTIPLUG_UNLOAD_IOCP2 = 10426;

  SM_RESPONSE_UPDATE_DLL = 10603;

  SM_ENABLE_UPLOAD_PICKITEMS = 10619;

  SM_SOFT_DELAY_EXIT = 10626;
  SM_SOFT_EXIT = 10627;

const
  MAP_NAME_LEN = 30;
  ACTOR_NAME_LEN = 14;                              // 30; -- Gxx // 角色长度
  ITEM_NAME_LEN = 60;
  MAX_FLUTE_COUNT = 8;                              // 最大凹槽数量

  ITEM_PROP_COUNT = 20;
  ITEM_PROP_VALUES_COUNT = 3;

  CUSTOM_MAGIC_COUNT = 300;

type
  TDefaultMessage = record
    Recog: Int64;
    Ident: Word;
    Param: Word;
    Tag: Word;
    Series: Word;
  end;
  pTDefaultMessage = ^TDefaultMessage;

  TM2MsgHeader = record
    dwCode: LongWord;
    nSocket: Integer;
    wGSocketIdx: Word;
    wIdent: Word;
    wUserListIndex: LongWord;
    nLength: Integer;
  end;
  pTM2MsgHeader = ^TM2MsgHeader;

  TRungateMsgHeader = record
    Code: LongWord;
    DataLen: LongWord;
    Msg: TDefaultMessage;
  end;
  pTRungateMsgHeader = ^TRungateMsgHeader;
  
  PCheckDBMsgHeader = ^TCheckDBMsgHeader;
  TCheckDBMsgHeader = record
    wIndent: Word;
    wData: Word;
  end;

  // 基本动作
  TBaseAction = (baOther, baHit, baSpell, baWalk, baRun, baTurn, baCutMeat);
  
  PRungateVerifyHeader = ^TRungateVerifyHeader;
  TRungateVerifyHeader = record
    dwCode: LongWord;
    dwCmd: LongWord;
    nLength: LongWord;
  end;

  TRungateVerifyData = packed record
    Key: LongWord;
    IP: array[0..14] of Char;
    HWID: array[0..7] of LongWord;
  end;

  PRungateVerifyData_New = ^TRungateVerifyData_New;
  TRungateVerifyData_New = packed record
    Key: LongWord;
    IP: array[0..14] of Char;
    HWID: array[0..7] of LongWord;
    UpdateDate: LongWord;
    Version: LongWord;
  end;


  // 插件文件头标记
  TPluginFileHeader = packed record
    PluginFlag1: LongWord;
    PluginFlag2: LongWord;
    PluginFlag3: LongWord;
    PluginFlag4: LongWord;
    PluginID: Integer;
    PluginType: Integer;
    Reserved: array[0..2] of Byte;
    PluginName: string[40];
    Reserved2: array[0..2] of Byte;
    UserDevOrganization: string[40];
    PluginVersion: Integer;
    PluginCRC: LongWord;
  end;
  
  /////////////////////////////////////////////////////////////////////////////////////////////

  // 基础技能 合击技能 连击技能 内功技能  , mtGroup
  TMagicAttr = (mtHum, mtHero, mtContinuous, mtDefense, mtAttack);
  TMagic_C = packed record
    MagicAttr: TMagicAttr;
    wMagicId: Word;
    sMagicName: string[ITEM_NAME_LEN];
    btEffectType: Byte;
    btEffect: Byte;
    wSpell: Word;
    MaxTrain: array[0..15] of Integer;
    btTrainLv: Byte;                                                                                // 最高可升级等级
    dwMagicDelayTime: LongWord;
    wDefSpell: Word;
    CanUpgrade: Integer;                                                                            // 是否允许升级 chongchong 2013-12-06
    MaxUpgradeLevel: Integer;                                                                       // 最高
  end;
  pTMagic_C = ^TMagic_C;

  TClientMagic = packed record                                                                      // 84
    Key: Char;
    Level: Byte;
    NewLevel: Byte;                                                                                 // 九重
    CurTrain: Integer;
    Def: TMagic_C;
    dwInterval: LongWord;
    dwRealInterval: LongWord;
    dwLastUseTick: LongWord;
  end;
  PTClientMagic = ^TClientMagic;

  /////////////////////////////////////////////////////////////////////////////////////////////


  TStdItemEffect = packed record
    FileIndex: SmallInt;                                                                            // 物品发光效果 文件编号 0
    ImageStart: Word;                                                                               // 物品发光效果 读取位置
    ImageCount: Byte;                                                                               // 物品发光效果 读取张数
    IsDrawCenter: Boolean;                                                                          // 居中播放
    IsDrawNoBlend: Boolean;                                                                         // 非透明绘制
    IsDrawBelow: Boolean;                                                                           // 底层绘制
    OffsetX: SmallInt;                                                                              // 物品发光效果 微调X
    OffsetY: SmallInt;                                                                              // 物品发光效果 微调Y
    Time: Word;                                                                                     // 播放速度
  end;

  TStdItem = packed record
    Name: string[ITEM_NAME_LEN];
    DBName: string[ITEM_NAME_LEN];
    StdMode: Byte;
    Shape: Word;
    Weight: Byte;
    AniCount: Word;
    Source: Integer;
    Reserved: Byte;
    NeedIdentify: Byte;
    Looks: Word;
    DuraMax: Word;
    Reserved1: Word;
    HP: Integer;
    MP: Integer;
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
    Need: Integer;
    NeedLevel: Integer;
    Price: Integer;
    OverLap: Word;                                                                                  // 是否是重叠物品
    Color: Byte;                                                                                    // 物品名称颜色
    Stock: Integer;
    Light: Integer;                                                                                 // 数据库增加Light字段 piaoyun 2013-08-01

    Horse: Integer;
    Expand1: Integer;
    Expand2: Integer;
    Expand3: Integer;
    Expand4: Integer;
    Expand5: Integer;

    Elements: array[0..20] of Word;

    InsuranceCurrency: Integer;
    InsuranceGold: Integer;

    BagEffect: TStdItemEffect;                                                                      // 包裹中的物品发光效果
    BodyEffect: TStdItemEffect;                                                                     // 内观中物品发光效果
    Effect: Pointer;
  end;
  pTStdItem = ^TStdItem;
  // 自定义物品进度条
  TUserItemProgress = packed record
    boOpen: Boolean;
    btNameColor: Byte;
    btCount: Byte;
    btShowType: Byte;       // 进度上的值显示方式(0:不显示; 1:百分比; 2:数值;)
    wMax: Word;
    wValue: Word;
    wLevel: Word;
    sName: string[31];
  end;


  // 单个属性
  PCustomProperty = ^TCustomProperty;
  TCustomProperty = packed record
    btColor: Byte;
    btBindType: Byte;
    btShowFlag: Byte;
    boPercent: Boolean;
    nValues: array[0..ITEM_PROP_VALUES_COUNT - 1] of Integer;
  end;

  PUserItemProperty = ^TUserItemProperty;
  TUserItemProperty = packed record
    sText: string[128];             // NND,从64又要整到128 2019-03-18 17:18:22
    btTextColor: Byte;
    Properties: array[0..ITEM_PROP_COUNT - 1] of TCustomProperty;
  end;

  // 物品来源
  TItemFormType = (ifUnknow{未知}, ifGM{GM制造}, ifScript{脚本}, ifShopBuy{商店购买}, ifMonDrop{打怪掉落}, ifSysGive{系统给予}, ifMine{挖矿得到}, ifBoxGive{宝箱取得}, ifButchItem{挖肉得到}, ifCaptureMon{捕捉得到});
  
  TUserItemFrom = packed record
    ItemForm: TItemFormType;
    sMapName: string[MAP_NAME_LEN];
    sMonName: string[40];
    sMakerName: string[ACTOR_NAME_LEN];
    DateTime: TDateTime;
  end;

  TClientItem = packed record                                                                       // OK
    s: TStdItem;
    MakeIndex: Integer;
    Dura: Word;
    DuraMax: Word;

    IsBind: Boolean;                                                                                // 是否绑定
    btFluteCount: Byte;

    btUpgradeCount: Byte;                                                                           // 升级次数
    btHeroM2Light: Byte;                                                                            // HeroM2 SetItemsLight

    btValue: array[0..13] of Integer;                                                               // 附加属性
    NewValue: array[0..30 - 1] of Word;

    wFlute: array[0..MAX_FLUTE_COUNT - 1] of Word;                                                  // 凹槽宝石信息     16
    Progress: array[0..1] of TUserItemProgress;

    CustomProperty: TUserItemProperty;

    ItemFrom: TUserItemFrom;
    wInsuranceCount: Integer;
  end;
  PTClientItem = ^TClientItem;


  /////////////////////////////////////////////////////////////////////////////////////////////

  TOStdItem = packed record                                                                         // OK
    Name: string[14];
    StdMode: Byte;
    Shape: Byte;
    Weight: Byte;
    AniCount: Byte;
    Source: ShortInt;
    Reserved: Byte;
    NeedIdentify: Byte;
    Looks: Word;
    DuraMax: Word;
    AC: Word;
    MAC: Word;
    DC: Word;
    MC: Word;
    SC: Word;
    Need: Byte;
    NeedLevel: Byte;
    w26: Word;
    Price: Integer;
  end;
  pTOStdItem = ^TOStdItem;

  TOClientItem = record                                                                            
    s: TOStdItem;
    MakeIndex: Integer;
    Dura: Word;
    DuraMax: Word;
  end;
  pTOClientItem = ^TOClientItem;

  /////////////////////////////////////////////////////////////////////////////////////////////

{ ------------------------------- 注册码相关 -------------------------------}
{$IF NEED_REGISTER = 1}
type
  // 服务器发到网关的原包头
  TServerMessageFlag = packed record
    gmData: Word;
    gmKick: Word;
    gmCompData: Word;
    gmDataCache: Word;
    gmNoCertification: Word;
    gmFullServiceMsg: Word;
  end;

  // 服务器发到客户端的包头
  TServerMessageClientFlag = packed record
    smModuleMD5: Word; // 模块md5
    smBlackModuleMd5: Word; // 黑名单模块md5
    smStdItemList: Word; // 物品列表
    smSendItemDescList: Word; // 物品备注
    smSendTZItemDescList: Word; // 套装物品备注
    smSendFilterItemList: Word; // 内挂捡取列表
    smEffectImageList: Word; // 特效文件列表
    smSpecialCmd: Word; // 特殊命令
    smSendCustomMagicConfig: Word; // 自定义技能配置
    smPlugFile: Word; // 插件文件
    smServerConfig: Word; // 服务器配置
    smSendCustomNpcConfig: Word; // 自定义NPC配置
    smSendItemDescTopList: Word;
    smSendCustomMonsterConfig: Word; // 自定义怪物配置

    smModuleMd5Cache: Word; // 模块md5缓存
    smSendCustomMonsterConfigCache: Word; // 自定义怪物配置缓存
    smStdItemListCache: word; // 物品列表缓存
    smSendItemDescListCache: Word; // 物品备注缓存
    smSendTZItemDescListCache: Word; // 套装物品备注缓存
    smSendFilterItemListCache: Word; // 内挂捡取列表缓存
    smEffectImageListCache: Word; // 特效文件列表缓存
    smSpecialCmdCache: Word; // 特殊命令缓存
    smSendCustomMagicConfigCache: Word; // 自定义技能配置缓存
    smPlugFileCache: Word; // 插件文件缓存
    smServerConfigCache: Word; // 服务器配置缓存
    smSendCustomNpcConfigCache: Word; // 自定义NPC配置缓存
    smSendItemDescTopListCache: Word; //

    smLogon: Word; // 登录
    smAbility: Word; // 属性
    smWhisper: Word; // 私聊
    smChangeMap: Word; // 换地图
    smEatFail: Word; // 吃东西失败
    smSendNotice: Word;
    smHeroEatFail: Word;

    smProcessBlacklist: Word; // 进程黑名单列表
  end;

  {
  TClientMessageFlag = packed record
    cmSoftClose: Word;                      // 小退
    cmSay: Word;                            // 说话
    cmQueryBagItems: Word;                  // 查询包裹
    cmDealTry: Word;                        // 尝试交易
    cmChallengeTry: Word;                   // 请求挑战

    cmSpell: Word;                          // 魔法

    cmHit: Word;                            // 物理攻击
    cmHeavyHit: Word;                       // 跳起来砍
    cmBigHit: Word;                         // 强攻
    cmPowerHit: Word;                       // 攻杀
    cmLongHit: Word;                        // 刺杀
    cmWideHit: Word;                        // 半月
    cmFireHit: Word;                        // 烈火
    cmCrsHit: Word;                         // 抱月
    cmTwnHit: Word;                         // 龙影
    cm43HIT: Word;                          // 雷霆剑法
    cmSwordHit: Word;                       // 逐日剑法     ID=56
    cm66HIT: Word;                          // 开天斩
    cm66HIT1: Word;                         // 开天斩
    cm101HIT: Word;                         // 三绝杀
    cm102HIT: Word;                         // 断岳斩
    cm103HIT: Word;                         // 横扫千军
    cmCustomHit1: Word;                     // 自定义技能1
    cmCustomHit100: Word;                   // 自定义技能100

    cmWalk: Word;                           // 走路
    cmRun: Word;                            // 跑步
    cmTurn: Word;                           // 转弯
    cmSitdown: Word;                        // 挖肉
    cmDropItem: Word;                       // 丢物品
    cmPickUp: Word;                         // 捡起
  end;
  }

  TClientMessageFlag = packed record
    cmSoftClose: Word; // 小退
    cmSay: Word; // 说话
    cmQueryBagItems: Word; // 查询包裹
    cmDealTry: Word; // 尝试交易
    cmChallengeTry: Word; // 请求挑战

    cmSpell: Word; // 魔法

    cmWalk: Word; // 走路
    cmRun: Word; // 跑步
    cmTurn: Word; // 转弯
    cmSitdown: Word; // 挖肉
    cmDropItem: Word; // 丢物品
    cmPickUp: Word; // 捡起

    cmHit: Word; // 物理攻击
    cmHeavyHit: Word; // 跳起来砍
    cmBigHit: Word; // 强攻
    cmPowerHit: Word; // 攻杀
    cmLongHit: Word; // 刺杀
    cmWideHit: Word; // 半月
    cmFireHit: Word; // 烈火
    cmCrsHit: Word; // 抱月
    cmTwnHit: Word; // 龙影
    cm43HIT: Word; // 雷霆剑法
    cmSwordHit: Word; // 逐日剑法     ID=56
    cm66HIT: Word; // 开天斩
    cm66HIT1: Word; // 开天斩
    cm101HIT: Word; // 三绝杀
    cm102HIT: Word; // 断岳斩
    cm103HIT: Word; // 横扫千军
    cmCustomHit1: Word; // 自定义技能1
    cmCustomHit100: Word; // 自定义技能100
  end;

  TLicenseDataInfo = packed record
    KeyVersion: DWORD; // 版本号

    Reseved: array[0..9] of DWORD; // 保留位置

    Key: array[0..7] of Byte; // Key 用于客户端插件解密

    SrvMsgClientFlag: TServerMessageClientFlag;
    RandomCode1Arr: array[0..19] of DWORD;

    SrvMsgClientFlagCRC: DWORD;
    SrvMsgClientFlagCRC_Enc: DWORD;

    KeyLastDay: Integer;
  end;

  TRemoteKeyDataInfo = packed record
    RandomCode1Arr: array[0..19] of DWORD;

    TimeLeft: Int64;

    SrvMsgFlag: TServerMessageFlag;
    SrvMsgFlagCRC: DWORD;

    ClientMsgFlag: TClientMessageFlag;
    ClientMsgFlagCRC: DWORD;
  end;
  
{$IFEND}

{ ------------------------来自于Common中的定义-------------------------- }
//{$IF NEED_REGISTER = 0}
const
//{$ELSE}
//var
//{$IFEND}
  GM_OPEN             = 1;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = 1;
  GM_CLOSE            = 2;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = 2;
  GM_CHECKSERVER      = 3; //{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = 3;                                                                // Send check signal to Server
  GM_CHECKCLIENT      = 4;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = 4;                                                                // Send check signal to Client
  GM_SERVERUSERINDEX  = 6;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = 6;
  GM_RECEIVE_OK       = 7;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = 7;
  GM_CLOSECONNECT     = 8;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = 8;
  GM_DELAY_CLOSE      = 20;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = 20;


  // 这几个值用变量搞
  GM_DATA             = 5;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = {$IF NEED_REGISTER = 1} 0 {$ELSE} 5 {$IFEND};
  GM_COMPDATA         = 9;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = {$IF NEED_REGISTER = 1} 0 {$ELSE} 9 {$IFEND};
  GM_KICK             = 10;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = {$IF NEED_REGISTER = 1} 0 {$ELSE} 10 {$IFEND};
  GM_DATA_CACHE       = 11;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = {$IF NEED_REGISTER = 1} 0 {$ELSE} 11 {$IFEND};                    // 数据缓存 chongchong 2015-05-15
  GM_NO_CERTIFICATION = 12;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = {$IF NEED_REGISTER = 1} 0 {$ELSE} 12 {$IFEND};
  GM_FULL_SERVICE_MSG = 13;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = {$IF NEED_REGISTER = 1} 0 {$ELSE} 13 {$IFEND};                    // 全服消息 chongchong 2016-07-12
  GM_RANDOM_DATA      = 14;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = {$IF NEED_REGISTER = 1} 14 {$ELSE} 14 {$IFEND};                   // 搞在线人的数据 chongchong 2016-08-01
  GM_RUN_GATE_VER     = 15;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = {$IF NEED_REGISTER = 1} 15 {$ELSE} 15 {$IFEND};                   // 网关版本号检测
  GM_RUN_GATE_MAGICS  = 16;//{$IF NEED_REGISTER = 1}: Integer {$IFEND}  = {$IF NEED_REGISTER = 1} 16 {$ELSE} 16 {$IFEND};                   // 获取技能列表

{$IF NEED_REGISTER = 0}
const
{$ELSE}
var
{$IFEND}
  RUNGATECODE         {$IF NEED_REGISTER = 0} = $AA55AA55 {$ELSE} : LongWord = $AA55AA55 {$IFEND};
  RUNGATECODEX        {$IF NEED_REGISTER = 0} = $AA9AAA9A {$ELSE} : LongWord = $AA9AAA9A {$IFEND};

const
  CM_QUERYBAGITEMS    = 81;//{$IF NEED_REGISTER = 0} = 81    {$ELSE} : Word = 0 {$IFEND};                // 查询包裹物品 90;
  CM_CHALLENGETRY     = 129;//{$IF NEED_REGISTER = 0} = 129   {$ELSE} : Word = 0 {$IFEND};                // 挑战
  CM_SOFTCLOSE        = 1009;//{$IF NEED_REGISTER = 0} = 1009  {$ELSE} : Word = 0 {$IFEND};                // 退出传奇(游戏程序,可能是游戏中大退,也可能时选人时退出) 92;
  CM_DEALTRY          = 1025;//{$IF NEED_REGISTER = 0} = 1025  {$ELSE} : Word = 0 {$IFEND};                // 开始交易,交易开始 69;
  CM_SAY              = 3030;//{$IF NEED_REGISTER = 0} = 3030  {$ELSE} : Word = 0 {$IFEND};                // 角色发言 88;

  CM_SPELL            = 3017;//{$IF NEED_REGISTER = 0} = 3017  {$ELSE} : Word = 0 {$IFEND};                // 施魔法
  CM_TURN             = 3010;//{$IF NEED_REGISTER = 0} = 3010  {$ELSE} : Word = 0 {$IFEND};                // 转身(方向改变)
  CM_WALK             = 3011;//{$IF NEED_REGISTER = 0} = 3011  {$ELSE} : Word = 0 {$IFEND};                // 走
  CM_SITDOWN          = 3012;//{$IF NEED_REGISTER = 0} = 3012  {$ELSE} : Word = 0 {$IFEND};                // 挖(蹲下)
  CM_RUN              = 3013;//{$IF NEED_REGISTER = 0} = 3013  {$ELSE} : Word = 0 {$IFEND};                // 跑
  CM_HIT              = 3014;//{$IF NEED_REGISTER = 0} = 3014  {$ELSE} : Word = 0 {$IFEND};                // 普通物理近身攻击
  CM_HEAVYHIT         = 3015;//{$IF NEED_REGISTER = 0} = 3015  {$ELSE} : Word = 0 {$IFEND};                // 跳起来打的动作
  CM_BIGHIT           = 3016;//{$IF NEED_REGISTER = 0} = 3016  {$ELSE} : Word = 0 {$IFEND};                // 强攻
  CM_POWERHIT         = 3018;//{$IF NEED_REGISTER = 0} = 3018  {$ELSE} : Word = 0 {$IFEND};                // 攻杀
  CM_LONGHIT          = 3019;//{$IF NEED_REGISTER = 0} = 3019  {$ELSE} : Word = 0 {$IFEND};                // 刺杀
  CM_WIDEHIT          = 3024;//{$IF NEED_REGISTER = 0} = 3024  {$ELSE} : Word = 0 {$IFEND};                // 半月
  CM_FIREHIT          = 3025;//{$IF NEED_REGISTER = 0} = 3025  {$ELSE} : Word = 0 {$IFEND};                // 烈火
  CM_CRSHIT           = 3036;//{$IF NEED_REGISTER = 0} = 3036  {$ELSE} : Word = 0 {$IFEND};                // 抱月刀 双龙斩 ID=40
  CM_TWNHIT           = 3037;//{$IF NEED_REGISTER = 0} = 3037  {$ELSE} : Word = 0 {$IFEND};                // 龙影剑法      ID=42

  // 下面开始是新技能
  CM_43HIT            = 3043;//{$IF NEED_REGISTER = 0} = 3043  {$ELSE} : Word = 0 {$IFEND};                // 雷霆剑法     ID=43
  CM_SWORDHIT         = 3056;//{$IF NEED_REGISTER = 0} = 3056  {$ELSE} : Word = 0 {$IFEND};                // 逐日剑法     ID=56

  CM_66HIT            = 3066;//{$IF NEED_REGISTER = 0} = 3066  {$ELSE} : Word = 0 {$IFEND};                // 开天斩
  CM_66HIT1           = 3166;//{$IF NEED_REGISTER = 0} = 3166  {$ELSE} : Word = 0 {$IFEND};

  CM_101HIT           = 3101;//{$IF NEED_REGISTER = 0} = 3101  {$ELSE} : Word = 0 {$IFEND};                // 三绝杀
  CM_102HIT           = 3102;//{$IF NEED_REGISTER = 0} = 3102  {$ELSE} : Word = 0 {$IFEND};                // 断岳斩
  CM_103HIT           = 3103;//{$IF NEED_REGISTER = 0} = 3103  {$ELSE} : Word = 0 {$IFEND};                // 横扫千军

  CM_DROPITEM         = 1000;//{$IF NEED_REGISTER = 0} = 1000  {$ELSE} : Word = 0 {$IFEND};                // 从包裹里扔出物品到地图,此时人物如果在安全区可能会提示安全区不允许扔东西 48;
  CM_PICKUP           = 1001;//{$IF NEED_REGISTER = 0} = 1001  {$ELSE} : Word = 0 {$IFEND};                // 捡东西 49;

  // CM_CUSTOM_HIT1      {$IF NEED_REGISTER = 0} = 5127  {$ELSE} : Word = 0 {$IFEND};
  // CM_CUSTOM_HIT100    {$IF NEED_REGISTER = 0} = 5226  {$ELSE} : Word = 0 {$IFEND};

  // ★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★

  SM_LOGON                          = 50;//{$IF NEED_REGISTER = 0} = 50    {$ELSE} : Word = 0 {$IFEND};  // logon 1169;
  SM_ABILITY                        = 52;//{$IF NEED_REGISTER = 0} = 52    {$ELSE} : Word = 0 {$IFEND};  // 打开属性对话框,F11 1171;

  SM_WHISPER                        = 103;//{$IF NEED_REGISTER = 0} = 103   {$ELSE}:  Word = 0 {$IFEND};  // 私聊 1177;

  SM_CHANGEMAP                      = 634;//{$IF NEED_REGISTER = 0} = 634   {$ELSE} : Word = 0 {$IFEND};  // 地图改变,进入新地图 1205;
  SM_EAT_FAIL                       = 636;//{$IF NEED_REGISTER = 0} = 636   {$ELSE} : Word = 0 {$IFEND};  // 吃药失败
  SM_HEROEAT_FAIL                   = 1478;//{$IF NEED_REGISTER = 0} = 1478  {$ELSE} : Word = 0 {$IFEND};  // 英雄吃药FAIL

  SM_SENDNOTICE                     = 658;//{$IF NEED_REGISTER = 0} = 658   {$ELSE} : Word = 0 {$IFEND};

  SM_EFFECTIMAGELIST                = 1353;//{$IF NEED_REGISTER = 0} = 1353  {$ELSE} : Word = 0 {$IFEND};  // WIL列表
  SM_SPECIALCMD                     = 1396;//{$IF NEED_REGISTER = 0} = 1396  {$ELSE} : Word = 0 {$IFEND};  // 特殊命令
  SM_PLUGFILE                       = 1454;//{$IF NEED_REGISTER = 0} = 1454  {$ELSE} : Word = 0 {$IFEND};  // 客户端插件MD5 M2发送过来进行检测
  SM_MODULEMD5                      = 1455;//{$IF NEED_REGISTER = 0} = 1455  {$ELSE} : Word = 0 {$IFEND};  // 白名单模块MD5‘
  SM_BLACKMODULEMD5                 = 1456;//{$IF NEED_REGISTER = 0} = 1456  {$ELSE} : Word = 0 {$IFEND};
  SM_STDITEMLIST                    = 1520;//{$IF NEED_REGISTER = 0} = 1520  {$ELSE} : Word = 0 {$IFEND};
  SM_SENDFILTERITEMLIST             = 1506;//{$IF NEED_REGISTER = 0} = 1506  {$ELSE} : Word = 0 {$IFEND};  // 内挂捡取过滤物品列表
  SM_SENDITEMDESCLIST               = 1507;//{$IF NEED_REGISTER = 0} = 1507  {$ELSE} : Word = 0 {$IFEND};  // 物品描述列表
  SM_SENDTZITEMDESCLIST             = 1508;//{$IF NEED_REGISTER = 0} = 1508  {$ELSE} : Word = 0 {$IFEND};
  SM_SERVERCONFIG                   = 5007;//{$IF NEED_REGISTER = 0} = 5007  {$ELSE} : Word = 0 {$IFEND};  // 20002; //1322;
  SM_SENDCUSTOMMONSTERCONFIG        = 8945;//{$IF NEED_REGISTER = 0} = 8945  {$ELSE} : Word = 0 {$IFEND};  // 发送自定义怪物配置到客户端 chongchong 2014-07-20
  SM_SENDCUSTOMMAGICCONFIG          = 8970;//{$IF NEED_REGISTER = 0} = 8970  {$ELSE} : Word = 0 {$IFEND};
  SM_SENDCUSTOMNPCCONFIG            = 8972;//{$IF NEED_REGISTER = 0} = 8972  {$ELSE} : Word = 0 {$IFEND};
  SM_SENDITEMDESCTOPLIST            = 8973;//{$IF NEED_REGISTER = 0} = 8973  {$ELSE} : Word = 0 {$IFEND};

  SM_SENDRUNGATE_CHECKCODE          = 9190;//{$IF NEED_REGISTER = 0} = 9190  {$ELSE} : Word = 9190 {$IFEND};

  SM_MODULEMD5_CACHE                = 10104;//{$IF NEED_REGISTER = 0} = 10104  {$ELSE} : Word = 0 {$IFEND};
  SM_SENDCUSTOMMONSTERCONFIG_CACHE  = 10105;//{$IF NEED_REGISTER = 0} = 10105  {$ELSE} : Word = 0 {$IFEND};
  SM_STDITEMLIST_CACHE              = 10106;//{$IF NEED_REGISTER = 0} = 10106  {$ELSE} : Word = 0 {$IFEND};
  SM_SENDITEMDESCLIST_CACHE         = 10107;//{$IF NEED_REGISTER = 0} = 10107  {$ELSE} : Word = 0 {$IFEND};
  SM_SENDTZITEMDESCLIST_CACHE       = 10108;//{$IF NEED_REGISTER = 0} = 10108  {$ELSE} : Word = 0 {$IFEND};
  SM_SENDFILTERITEMLIST_CACHE       = 10109;//{$IF NEED_REGISTER = 0} = 10109  {$ELSE} : Word = 0 {$IFEND};
  SM_EFFECTIMAGELIST_CACHE          = 10110;//{$IF NEED_REGISTER = 0} = 10110  {$ELSE} : Word = 0 {$IFEND};
  SM_SPECIALCMD_CACHE               = 10111;//{$IF NEED_REGISTER = 0} = 10111  {$ELSE} : Word = 0 {$IFEND};
  SM_SENDCUSTOMMAGICCONFIG_CACHE    = 10112;//{$IF NEED_REGISTER = 0} = 10112  {$ELSE} : Word = 0 {$IFEND};
  SM_PLUGFILE_CACHE                 = 10113;//{$IF NEED_REGISTER = 0} = 10113  {$ELSE} : Word = 0 {$IFEND};
  SM_SERVERCONFIG_CACHE             = 10114;//{$IF NEED_REGISTER = 0} = 10114  {$ELSE} : Word = 0 {$IFEND};
  SM_SENDCUSTOMNPCCONFIG_CACHE      = 10115;//{$IF NEED_REGISTER = 0} = 10115  {$ELSE} : Word = 0 {$IFEND};
  SM_SENDITEMDESCTOPLIST_CACHE      = 10116;//{$IF NEED_REGISTER = 0} = 10116  {$ELSE} : Word = 0 {$IFEND};

  SM_GETPROCESS_LIST                = 10238;//{$IF NEED_REGISTER = 0} = 10238  {$ELSE} : Word = 10238 {$IFEND};
  SM_GETSCREENSHOT_GAME             = 10239;//{$IF NEED_REGISTER = 0} = 10239  {$ELSE} : Word = 10239 {$IFEND};
  SM_GETSCREENSHOT                  = 10240;//{$IF NEED_REGISTER = 0} = 10240  {$ELSE} : Word = 10240 {$IFEND};
  SM_PROCESSBLACKLIST               = 10269;//{$IF NEED_REGISTER = 0} = 10269  {$ELSE} : Word = 0 {$IFEND};  // 进程黑名单列表 chongchong 2016-08-01


implementation

end.

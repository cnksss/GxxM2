unit GameConfigDlg;

interface
uses
  Classes,
  Grobal2,
  DxComponents;
type
  TConfigDlgType = (ptDefault, ptJSY);

  TItemType = (i_All, i_Other, i_HPMPDurg, i_Dress, i_Weapon, i_Jewelry, i_Decoration, i_Decorate, i_diy);

  TShowItem = record
    boHintMsg:Boolean;
    boPickup:Boolean;
    boShowName:Boolean;
    boShowSpecial:Boolean; // 特殊物品显示 piaoyun 2013-09-09
    boAutoMove:Boolean;
    ItemType:TItemType;
    sItemName:string;
    sItemType:string;
    boFromSystem:Boolean;
  end;
  pTShowItem = ^TShowItem;

  TConfigChecked =
    (
    ckShowHPLabel, // 显示血条         PlugCheckBoxItemHint
    ckShowNumberLable, // 数字显血
    ckShowJobAndLevel, // 显示职业等级
    ckFilterExp, // 经验过滤
    ckShowGreenHint, // 显示顶部绿色信息
    ckShowUserName, // 显示 人名
    ckOnlyShowCharName, // 只显示人名
    ckShowMoveLable, // 数字飘血
    ckAutoPickUpItem, // 自动捡取
    ckNoCaton, // 超级不卡
    ckSimpleShowHumanWeapon, // 武器简装 ********************************

    ckDisableSelfStruck, // 稳如泰山
    ckSpeedSlow, // 免负重 行动慢
    ckMagicLock, // 魔法锁定
    ckPickUpAll, // 全部拾取
    ckAutoOrderItem, // 自动放药
    ckAutoCloseGroup, // 自动关组
    ckDuraWarning, // 持久警告
    ckNotNeedShift, // 免Shift键
    ckShiftSwitch, // Shift开关
    ckHideGhost, // 隐藏尸体
    ckHideHumEffect, // 隐藏翅膀效果

    ckHideWeaponEffect, // 隐藏武器效果
    ckShowMapDesc, // 显示地图标识
    ckShowHighlightHPLabel, // 人物高亮显血
    ckAutoHideMode, // 自动隐身
    ckSmart113Hit, // 自动断空斩
    ckHumAutoShield, // 自动开盾
    ckHumStruckShield, // 被攻击开盾
    ckSmartLongHit, // 刀刀刺杀
    ckSmartPosLongHit, // 隔位刺杀
    ckSmartWalkLongHit, // 走位刺杀
    ckSmartWideHit, // 智能半月

    ckSmartFireHit, // 自动烈火
    ckSmartSwordHit, // 逐日剑法
    ckSmartCrsHit, // 抱月刀 双龙斩
    ckSmartTwnHit, // 龙影剑法
    ckBGMusic, // 背景音乐
    ckRepeatBGMusic, // 重复背景音乐
    ckShowMonName, // 显示怪名
    ckShowNGLabel, // 显示内功黄条 piaoyun 2013-07-31
    ckNotParaly, // 防止石化
    ckHumManuallySnowWind, // 手动控制冰咆哮
    ckHumManuallyFireBoom, // 手动控制爆裂火焰

    ckHumShootLightenLockTarget, // 疾光电影锁定目标
    ckHumManuallyMeteorShower, // 手动控制流星火雨
    ckAutoCHangePoison, // 红绿毒互换
    ckParam9, // 免助跑
    ckSmart66Hit, // 开天斩
    ckHeroAutoShield, // 自动开盾(英雄)
    ckAssistantHeroAutoShield, // 自动开盾(副英雄)
    ckParam10, // 主将英雄药品
    ckParam11, // 副将英雄药品
    ckSceneShake, // 屏幕震动
    ckShowNpcName, // 显示NPC名 piaoyun 2013-07-31

    ckShowNpcHPLabel, // 显示NPC血条 piaoyun 2013-07-31
    ckHideTitle, // 隐藏称号 chongchong 2014-09-25
    ckAutoOpenSpell, // 自动凝聚技能
    ckDisableChartMemoSize, // 禁止拉动聊天框
    ckItemCompare, // 装备对比
    ckVolume, // 音量
    ckContinueButchItem, // 持续挖取 chongchong 2015-08-20
    ckDisableDeal, // 禁止交易 chongchong 2015-11-12
    ckShowUpdateStatus, // 微端状态显示 c
    ckSimpleShowActor, // 简装显示 chongchong 2016-02-01
    ckShowFashion, // 外显时装 chongchong 2016-02-01
    ckHumManuallyMove10Attack, // 手动控制10步一杀 chongchong 2016-07-02
    ckSimpleShowHumanDress,
    ckHideItemEffect,
    ckAutoGroupAttack,
    ckAutoGroupNoAttackMon,
    ckBagFastItemCompare,
    ckHumManuallyFire, // 手动控制地狱火 chongchong 2017-08-31
    ckShowHPUnit,
    ckShowTargetAperture, // 显示目标光圈
    ckShowNewGroupInfo,
    ckSimpleShowBB,

    ckSmartCustomHit1,
    ckSmartCustomHit2,
    ckSmartCustomHit3,
    ckSmartCustomHit4,
    ckSmartCustomHit5,
    ckSmartCustomHit6,
    ckSmartCustomHit7,
    ckSmartCustomHit8,

    ckHumManuallyCustomHit1,
    ckHumManuallyCustomHit2,
    ckHumManuallyCustomHit3,
    ckHumManuallyCustomHit4,
    ckHumManuallyCustomHit5,
    ckShowValueItemEffect,

    ckAutoContinueAttack,
    ckHideActorIcons,

    ckHideMonsterIcons, //隐藏怪物顶戴花翎 20230601
    ckDimFireEffect, //火墙淡化 20230601
    ckAutoDetourPath, //自动绕行 20230601  //从这以后的，不在登录器的配置中

    ckParam12,
    ckParam13,
    ckParam14,
    ckParam15,
    ckParam16,
    ckParam17,
    ckParam18,
    ckParam19,
    ckParam20,

    ckUseSuperMedica, // 自动使用药品
    ckAutoUseMagic, // 自动练功

    ckHeroAutoReCallSlave,
    ckUseKeyBoard, // 启用自定义快捷键
    ckShowRadarPlayer,
    ckShowRadarActor,
    ckShowRadarNpc,
    ckShowRadarAttackNpc,
    ckNearHint, // 接近提示 piaoyun 2013-09-09
    ckAutoLock, // 自动锁定 piaoyun 2013-09-09
    ckColorShow, // 变色显示 piaoyun 2013-09-09
    ckSpecialQuickFlashing, // 特殊物品快闪 piaoyun 2013-09-10
    ckBlacklistHit, // 黑名单近身提示 piaoyun 2013-09-11
    ckFriendHit, // 好友近身提示 piaoyun 2013-09-11

    ckAutoDownHorse, // 魔法攻击时自动下马 chongchong 2013-10-19
    ckHeroContinuousNoHitMon, // 英雄连击不攻击怪物 chongchong 2013-11-10

    ckGJ_NoRedPoison, // 挂机 - 红药用完回城 chongchong 2014-11-24
    ckGJ_NoBluePoison, // 挂机 - 蓝药用完回城 chongchong 2014-11-24

    ckGJ_PlayAttack, // 挂机 - 受玩家攻击
    ckGJ_NotRushMon, // 挂机 - 不抢怪

    ckGJ_NoDuFu, // 挂机 - 毒符用完回城
    ckGJ_BagFull, // 挂机 - 包裹满时回城
    ckGJ_AutoPickup, // 挂机 - 自动捡物
    ckGJ_LimitScreen, // 挂机 - 限制在当前屏幕
    ckGJ_GroupAttack, // 挂机 - 群攻
    ckGJ_DFStopAvoid, // 挂机 - 道法不躲避

    ckHideBigHPProgress, // 隐藏怪物大血条

    ckHeroShowNumberState, // 英雄数字状态显示

    ckObjectHintEffect  //HZQ 20230828 对应PlugCheckBoxNearEffect.Checked
    );

  TGameConfigObject = class //(TPersistent) //HZQ 准备读取RTTI信息
  public
    function GetType:TConfigDlgType; virtual; abstract;
    function GetConfigChecked(Index:TConfigChecked):Boolean; virtual; abstract;
    procedure SetConfigChecked(Index:TConfigChecked; Value:Boolean); virtual; abstract;
    function GetVisible:Boolean; virtual; abstract;
    procedure SetVisible(Value:Boolean); virtual; abstract;
    function GetEnabled:Boolean; virtual; abstract;
    procedure SetEnabled(Value:Boolean); virtual; abstract;
    function GetProtectEnabled:Boolean; virtual; abstract;
    procedure SetProtectEnabled(Value:Boolean); virtual; abstract;
    procedure Open; virtual; abstract;
    procedure Close; virtual; abstract;

    procedure LoadConfig(const CharName:string); virtual; abstract;
    procedure Initialize(Handle:THandle; ScreenMode:Byte; ClientVersion:TClientVersion; WindowMode:Boolean); virtual; abstract;
    procedure Finalize; virtual; abstract;
    procedure Logon(const ServerName:string); virtual; abstract;
    procedure Logout; virtual; abstract;
    function FormKeyDown(var Key:Word; Shift:TShiftState):Boolean; virtual; abstract;
    function FormKeyPress(var Key:Char):Boolean; virtual; abstract;
    procedure RefreshMySelfAbil; virtual; abstract;
    procedure RefreshMyHeroAbil; virtual; abstract;
    procedure RefreshMySelfMagicList; virtual; abstract;
    procedure RefreshMyHeroMagicList; virtual; abstract;
    procedure RefreshUnBindItemList; virtual; abstract;
    procedure RefKeyboardConfig; virtual; abstract;
    procedure Struck(Actor:TObject; HP, MaxHP:LongInt); virtual; abstract;
    procedure HealthChange(Actor:TObject; HP, MP, MaxHP:LongInt); virtual; abstract;
    procedure LoadClientConfig(ClientConfig:pTClientConfig); virtual; abstract;
    procedure Run; virtual; abstract;
    procedure RefActorList; virtual; abstract;
    function CanFilterExp(Exp:LongWord):Boolean; virtual; abstract;
    function GetShowItem(const ItemName:string):pTShowItem; virtual; abstract;
    function FindShowItem(const ItemName:string):Boolean; virtual; abstract;
    function FindHintItem(const ItemName:string):Boolean; virtual; abstract;
    function FindPickItem(const ItemName:string):Boolean; virtual; abstract;
    procedure HintItem(const ItemName:string; X, Y:Integer); virtual; abstract;
    procedure ClearShowItem; virtual; abstract;
    procedure RefShowItem; virtual; abstract;
    procedure AddToBossList(sName:string); virtual; abstract;
    procedure RemoveFromBossList(sName:string); virtual; abstract;
    procedure AddOrRemoveBossList(sNamt:string); virtual; abstract;
    
    property ConfigDlgType:TConfigDlgType read GetType;
    property Visible:Boolean read GetVisible write SetVisible;
    property Enabled:Boolean read GetEnabled write SetEnabled;
    property ProtectEnabled:Boolean read GetProtectEnabled write SetProtectEnabled;
    property ConfigCheckeds[Index:TConfigChecked]:Boolean read GetConfigChecked write SetConfigChecked;
  end;
implementation

end.

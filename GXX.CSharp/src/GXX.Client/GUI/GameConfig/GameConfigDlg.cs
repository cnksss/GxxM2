using System;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig;

/// <summary>
/// GameConfigDlg.pas 1:1 移植（全文 1-235 行）：
/// 配置对话框公共层——配置项枚举 TConfigChecked、物品过滤记录 TShowItem、
/// 配置项类型 TItemType、以及抽象基类 TGameConfigObject。
/// </summary>
public enum TConfigDlgType
{
    /// <summary>ptDefault</summary>
    ptDefault = 0,
    /// <summary>ptJSY</summary>
    ptJSY = 1,
}

/// <summary>GameConfigDlg.pas:11 原文如此（TItemType = (i_All, i_Other, ... i_diy)）。</summary>
public enum TItemType
{
    i_All = 0,
    i_Other = 1,
    i_HPMPDurg = 2,
    i_Dress = 3,
    i_Weapon = 4,
    i_Jewelry = 5,
    i_Decoration = 6,
    i_Decorate = 7,
    i_diy = 8,
}

/// <summary>
/// GameConfigDlg.pas:13-23 的 TShowItem（**同单元内部是 record**）。
/// 原作通过 <c>pTShowItem = ^TShowItem</c> 以指针在 TList 中传递；托管侧没有"指向栈记录的
/// 指针"，C# 侧用 **class + 引用** 表达同一语义（引用传递、可原地修改、可判空），
/// 字段名与原文逐字一致。
/// </summary>
public sealed class TShowItem
{
    /// <summary>对应原文 TShowItem.boHintMsg:Boolean（Delphi Boolean 1 字节，托管侧 byte，与 0 比较）。</summary>
    public byte boHintMsg;
    /// <summary>对应原文 TShowItem.boPickup:Boolean。</summary>
    public byte boPickup;
    /// <summary>对应原文 TShowItem.boShowName:Boolean。</summary>
    public byte boShowName;
    /// <summary>特殊物品显示 piaoyun 2013-09-09</summary>
    public byte boShowSpecial;
    /// <summary>对应原文 TShowItem.boAutoMove:Boolean。</summary>
    public byte boAutoMove;
    /// <summary>对应原文 TShowItem.ItemType:TItemType。</summary>
    public TItemType ItemType;
    /// <summary>对应原文 TShowItem.sItemName:string。</summary>
    public string sItemName = "";
    /// <summary>对应原文 TShowItem.sItemType:string。</summary>
    public string sItemType = "";
    /// <summary>对应原文 TShowItem.boFromSystem:Boolean。</summary>
    public byte boFromSystem;

    /// <summary>原文 <c>FileItem^ := ShowItem^;</c>（FilterItems.pas:239）的逐字段整记录拷贝。</summary>
    public TShowItem Clone() => new TShowItem
    {
        boHintMsg = boHintMsg,
        boPickup = boPickup,
        boShowName = boShowName,
        boShowSpecial = boShowSpecial,
        boAutoMove = boAutoMove,
        ItemType = ItemType,
        sItemName = sItemName,
        sItemType = sItemType,
        boFromSystem = boFromSystem,
    };

    /// <summary>原文 <c>pTShowItem(...)^ := pTShowItem(...)^;</c>（FilterItems.pas:251）的整记录赋值。</summary>
    public void AssignFrom(TShowItem other)
    {
        boHintMsg = other.boHintMsg;
        boPickup = other.boPickup;
        boShowName = other.boShowName;
        boShowSpecial = other.boShowSpecial;
        boAutoMove = other.boAutoMove;
        ItemType = other.ItemType;
        sItemName = other.sItemName;
        sItemType = other.sItemType;
        boFromSystem = other.boFromSystem;
    }
}

/// <summary>
/// GameConfigDlg.pas:26-181 的 TConfigChecked。
/// **枚举成员顺序即原文顺序，也是 g_ConfigClient.ClientConfigs 的数组下标含义**，
/// 因此顺序不可调整。HostCheckBoxName 是原文注释里的控件名（DFM 关联线索）。
/// </summary>
public enum TConfigChecked
{
    ckShowHPLabel = 0,               // 显示血条         PlugCheckBoxItemHint
    ckShowNumberLable = 1,           // 数字显血
    ckShowJobAndLevel = 2,           // 显示职业等级
    ckFilterExp = 3,                 // 经验过滤
    ckShowGreenHint = 4,             // 显示顶部绿色信息
    ckShowUserName = 5,              // 显示 人名
    ckOnlyShowCharName = 6,          // 只显示人名
    ckShowMoveLable = 7,             // 数字飘血
    ckAutoPickUpItem = 8,            // 自动捡取
    ckNoCaton = 9,                   // 超级不卡
    ckSimpleShowHumanWeapon = 10,    // 武器简装 ********************************

    ckDisableSelfStruck = 11,        // 稳如泰山
    ckSpeedSlow = 12,                // 免负重 行动慢
    ckMagicLock = 13,                // 魔法锁定
    ckPickUpAll = 14,                // 全部拾取
    ckAutoOrderItem = 15,            // 自动放药
    ckAutoCloseGroup = 16,           // 自动关组
    ckDuraWarning = 17,              // 持久警告
    ckNotNeedShift = 18,             // 免Shift键
    ckShiftSwitch = 19,              // Shift开关
    ckHideGhost = 20,                // 隐藏尸体
    ckHideHumEffect = 21,            // 隐藏翅膀效果

    ckHideWeaponEffect = 22,         // 隐藏武器效果
    ckShowMapDesc = 23,              // 显示地图标识
    ckShowHighlightHPLabel = 24,     // 人物高亮显血
    ckAutoHideMode = 25,             // 自动隐身
    ckSmart113Hit = 26,              // 自动断空斩
    ckHumAutoShield = 27,            // 自动开盾
    ckHumStruckShield = 28,          // 被攻击开盾
    ckSmartLongHit = 29,             // 刀刀刺杀
    ckSmartPosLongHit = 30,          // 隔位刺杀
    ckSmartWalkLongHit = 31,         // 走位刺杀
    ckSmartWideHit = 32,             // 智能半月

    ckSmartFireHit = 33,             // 自动烈火
    ckSmartSwordHit = 34,            // 逐日剑法
    ckSmartCrsHit = 35,              // 抱月刀 双龙斩
    ckSmartTwnHit = 36,              // 龙影剑法
    ckBGMusic = 37,                  // 背景音乐
    ckRepeatBGMusic = 38,            // 重复背景音乐
    ckShowMonName = 39,              // 显示怪名
    ckShowNGLabel = 40,              // 显示内功黄条 piaoyun 2013-07-31
    ckNotParaly = 41,                // 防止石化
    ckHumManuallySnowWind = 42,      // 手动控制冰咆哮
    ckHumManuallyFireBoom = 43,      // 手动控制爆裂火焰

    ckHumShootLightenLockTarget = 44, // 疾光电影锁定目标
    ckHumManuallyMeteorShower = 45,  // 手动控制流星火雨
    ckAutoCHangePoison = 46,         // 红绿毒互换
    ckParam9 = 47,                   // 免助跑
    ckSmart66Hit = 48,               // 开天斩
    ckHeroAutoShield = 49,           // 自动开盾(英雄)
    ckAssistantHeroAutoShield = 50,  // 自动开盾(副英雄)
    ckParam10 = 51,                  // 主将英雄药品
    ckParam11 = 52,                  // 副将英雄药品
    ckSceneShake = 53,               // 屏幕震动
    ckShowNpcName = 54,              // 显示NPC名 piaoyun 2013-07-31

    ckShowNpcHPLabel = 55,           // 显示NPC血条 piaoyun 2013-07-31
    ckHideTitle = 56,                // 隐藏称号 chongchong 2014-09-25
    ckAutoOpenSpell = 57,            // 自动凝聚技能
    ckDisableChartMemoSize = 58,     // 禁止拉动聊天框
    ckItemCompare = 59,              // 装备对比
    ckVolume = 60,                   // 音量
    ckContinueButchItem = 61,        // 持续挖取 chongchong 2015-08-20
    ckDisableDeal = 62,              // 禁止交易 chongchong 2015-11-12
    ckShowUpdateStatus = 63,         // 微端状态显示 c
    ckSimpleShowActor = 64,          // 简装显示 chongchong 2016-02-01
    ckShowFashion = 65,              // 外显时装 chongchong 2016-02-01
    ckHumManuallyMove10Attack = 66,  // 手动控制10步一杀 chongchong 2016-07-02
    ckSimpleShowHumanDress = 67,
    ckHideItemEffect = 68,
    ckAutoGroupAttack = 69,
    ckAutoGroupNoAttackMon = 70,
    ckBagFastItemCompare = 71,
    ckHumManuallyFire = 72,          // 手动控制地狱火 chongchong 2017-08-31
    ckShowHPUnit = 73,
    ckShowTargetAperture = 74,       // 显示目标光圈
    ckShowNewGroupInfo = 75,
    ckSimpleShowBB = 76,

    ckSmartCustomHit1 = 77,
    ckSmartCustomHit2 = 78,
    ckSmartCustomHit3 = 79,
    ckSmartCustomHit4 = 80,
    ckSmartCustomHit5 = 81,
    ckSmartCustomHit6 = 82,
    ckSmartCustomHit7 = 83,
    ckSmartCustomHit8 = 84,

    ckHumManuallyCustomHit1 = 85,
    ckHumManuallyCustomHit2 = 86,
    ckHumManuallyCustomHit3 = 87,
    ckHumManuallyCustomHit4 = 88,
    ckHumManuallyCustomHit5 = 89,
    ckShowValueItemEffect = 90,

    ckAutoContinueAttack = 91,
    ckHideActorIcons = 92,

    ckHideMonsterIcons = 93,         // 隐藏怪物顶戴花翎 20230601
    ckDimFireEffect = 94,            // 火墙淡化 20230601
    ckAutoDetourPath = 95,           // 自动绕行 20230601  //从这以后的，不在登录器的配置中

    ckParam12 = 96,
    ckParam13 = 97,
    ckParam14 = 98,
    ckParam15 = 99,
    ckParam16 = 100,
    ckParam17 = 101,
    ckParam18 = 102,
    ckParam19 = 103,
    ckParam20 = 104,

    ckUseSuperMedica = 105,          // 自动使用药品
    ckAutoUseMagic = 106,            // 自动练功

    ckHeroAutoReCallSlave = 107,
    ckUseKeyBoard = 108,             // 启用自定义快捷键
    ckShowRadarPlayer = 109,
    ckShowRadarActor = 110,
    ckShowRadarNpc = 111,
    ckShowRadarAttackNpc = 112,
    ckNearHint = 113,                // 接近提示 piaoyun 2013-09-09
    ckAutoLock = 114,                // 自动锁定 piaoyun 2013-09-09
    ckColorShow = 115,               // 变色显示 piaoyun 2013-09-09
    ckSpecialQuickFlashing = 116,    // 特殊物品快闪 piaoyun 2013-09-10
    ckBlacklistHit = 117,            // 黑名单近身提示 piaoyun 2013-09-11
    ckFriendHit = 118,               // 好友近身提示 piaoyun 2013-09-11

    ckAutoDownHorse = 119,           // 魔法攻击时自动下马 chongchong 2013-10-19
    ckHeroContinuousNoHitMon = 120,  // 英雄连击不攻击怪物 chongchong 2013-11-10

    ckGJ_NoRedPoison = 121,          // 挂机 - 红药用完回城 chongchong 2014-11-24
    ckGJ_NoBluePoison = 122,         // 挂机 - 蓝药用完回城 chongchong 2014-11-24

    ckGJ_PlayAttack = 123,           // 挂机 - 受玩家攻击
    ckGJ_NotRushMon = 124,           // 挂机 - 不抢怪

    ckGJ_NoDuFu = 125,               // 挂机 - 毒符用完回城
    ckGJ_BagFull = 126,              // 挂机 - 包裹满时回城
    ckGJ_AutoPickup = 127,           // 挂机 - 自动捡物
    ckGJ_LimitScreen = 128,          // 挂机 - 限制在当前屏幕
    ckGJ_GroupAttack = 129,          // 挂机 - 群攻
    ckGJ_DFStopAvoid = 130,          // 挂机 - 道法不躲避

    ckHideBigHPProgress = 131,       // 隐藏怪物大血条

    ckHeroShowNumberState = 132,     // 英雄数字状态显示

    ckObjectHintEffect = 133         //HZQ 20230828 对应PlugCheckBoxNearEffect.Checked
}

/// <summary>TConfigChecked 的 Low/High 边界（原文 <c>Low(TConfigChecked)</c>/<c>High(...)</c>）。</summary>
public static class TConfigCheckedBounds
{
    public const TConfigChecked Low = TConfigChecked.ckShowHPLabel;
    public const TConfigChecked High = TConfigChecked.ckObjectHintEffect;
    /// <summary>Delphi <c>High(TConfigChecked)</c> 的序数值。</summary>
    public const int HighOrdinal = (int)TConfigChecked.ckObjectHintEffect;
}

/// <summary>
/// GameConfigDlg.pas:183-232 的 TGameConfigObject 1:1 移植：
/// 原文全部是 <c>virtual; abstract;</c>，C# 侧对应 <c>abstract</c> 成员。
/// 说明：C# 的 <c>Finalize</c> 与 <c>object.Finalize</c> 同名（原文方法就叫 Finalize），
/// 用 <c>new</c> 隐藏基类终结器以保留原名，语义上是一个普通可覆写方法。
/// </summary>
public abstract class TGameConfigObject
{
    public abstract TConfigDlgType GetType();
    public abstract bool GetConfigChecked(TConfigChecked Index);
    public abstract void SetConfigChecked(TConfigChecked Index, bool Value);
    public abstract bool GetVisible();
    public abstract void SetVisible(bool Value);
    public abstract bool GetEnabled();
    public abstract void SetEnabled(bool Value);
    public abstract bool GetProtectEnabled();
    public abstract void SetProtectEnabled(bool Value);
    public abstract void Open();
    public abstract void Close();

    public abstract void LoadConfig(string CharName);
    public abstract void Initialize(IntPtr Handle, byte ScreenMode, TClientVersion ClientVersion, bool WindowMode);
    public new abstract void Finalize();
    public abstract void Logon(string ServerName);
    public abstract void Logout();
    public abstract bool FormKeyDown(ref ushort Key, DelphiShiftState Shift);
    public abstract bool FormKeyPress(ref char Key);
    public abstract void RefreshMySelfAbil();
    public abstract void RefreshMyHeroAbil();
    public abstract void RefreshMySelfMagicList();
    public abstract void RefreshMyHeroMagicList();
    public abstract void RefreshUnBindItemList();
    public abstract void RefKeyboardConfig();
    public abstract void Struck(object Actor, int HP, int MaxHP);
    public abstract void HealthChange(object Actor, int HP, int MP, int MaxHP);
    public abstract void LoadClientConfig(TClientConfig ClientConfig);
    public abstract void Run();
    public abstract void RefActorList();
    public abstract bool CanFilterExp(uint Exp);
    public abstract TShowItem GetShowItem(string ItemName);
    public abstract bool FindShowItem(string ItemName);
    public abstract bool FindHintItem(string ItemName);
    public abstract bool FindPickItem(string ItemName);
    public abstract void HintItem(string ItemName, int X, int Y);
    public abstract void ClearShowItem();
    public abstract void RefShowItem();
    public abstract void AddToBossList(string sName);
    public abstract void RemoveFromBossList(string sName);
    public abstract void AddOrRemoveBossList(string sNamt);

    /// <summary>原文 <c>property ConfigDlgType:TConfigDlgType read GetType;</c></summary>
    public TConfigDlgType ConfigDlgType => GetType();
    /// <summary>原文 <c>property Visible:Boolean read GetVisible write SetVisible;</c></summary>
    public bool Visible { get => GetVisible(); set => SetVisible(value); }
    /// <summary>原文 <c>property Enabled:Boolean read GetEnabled write SetEnabled;</c></summary>
    public bool Enabled { get => GetEnabled(); set => SetEnabled(value); }
    /// <summary>原文 <c>property ProtectEnabled:Boolean read GetProtectEnabled write SetProtectEnabled;</c></summary>
    public bool ProtectEnabled { get => GetProtectEnabled(); set => SetProtectEnabled(value); }
    /// <summary>原文 <c>property ConfigCheckeds[Index:TConfigChecked]:Boolean read GetConfigChecked write SetConfigChecked;</c></summary>
    public bool this[TConfigChecked Index]
    {
        get => GetConfigChecked(Index);
        set => SetConfigChecked(Index, value);
    }

    /// <summary>
    /// C# **不允许命名索引器**，故把原文的 <c>ConfigCheckeds[idx]</c> 暴露为同名的勾选位数组，
    /// 用法 <c>o.ConfigCheckeds[(int)idx]</c> 与原文 <c>o.ConfigCheckeds[idx]</c> 一一对应。
    /// 数组实例由各实现类持有（原文抽象基类本身也不持有该数组，
    /// 见 TMirsConfigDlg 的 <c>FConfigCheckeds:array[TConfigChecked] of Boolean</c>）。
    /// </summary>
    public abstract bool[] ConfigCheckeds { get; }
}

// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：ObjNpc.pas 的**类字段面**（interface 段的字段声明 1:1）。
//   · TNormNpc       262-292（字段）+ 293-332（方法面；实现见同目录各 partial 文件）
//   · TMerchant      334-389（字段）
//   · TGuildOfficial 432-437（无字段）
//   · TTrainer       449-453
//   · TBoxMonster    461-468（无字段）
//   · TCastleOfficial 471-476（无字段）
//
// 继承链原文：TNormNpc = class(TAnimalObject)（ObjBase.pas:817）
//             TAnimalObject = class(TBaseObject)（ObjBase.pas:817/94）
//             TBaseObject   = class(TGameObject)（ObjBase.pas:94）
// 托管侧：ObjBase 尚未全量移植，最小可用代表是既有接缝
//   `GXX.M2Server.Engine.TCreature`（Engine/ObjBase.cs:13）。
// 本车道**不复制第二份 TBaseObject/TAnimalObject**，直接以 TCreature 作祖先；
// 原文 TBaseObject/TAnimalObject 上 ObjNpc.pas 读到的少数成员
// （m_CurrTarget/m_LastHiter/GetPoseCreate/m_MyHero）走 NpcSeams 委托。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

/// <summary>
/// 原文 `TNormNpc = class(TAnimalObject) // 0x564`（ObjNpc.pas:262）。
/// 字段按原文顺序与命名 1:1；类型按 §3.1 映射
/// （ShortInt→sbyte、LongWord→uint、TStringList→GXX.Core.Util.TStringList、TList→List&lt;object&gt;）。
/// </summary>
public partial class TNormNpc : TCreature
{
    /// <summary>原文 `m_sScript: string; // 0x568`（ObjNpc.pas:263）。</summary>
    public string m_sScript = "";
    /// <summary>原文 `m_nFlag: ShortInt; // 0x550 //用于标识此NPC是否有效，用于重新加载NPC列表(-1 为无效)`（ObjNpc.pas:264）。</summary>
    public sbyte m_nFlag;
    /// <summary>原文 `m_ScriptList: TList; // 0x554`（ObjNpc.pas:265）。</summary>
    public List<object> m_ScriptList = new();
    /// <summary>原文 `m_sFilePath: string; // 0x558 脚本文件所在目录`（ObjNpc.pas:266）。</summary>
    public string m_sFilePath = "";
    /// <summary>原文 `m_boIsHide: Boolean; // 0x55C 此NPC是否是隐藏的，不显示在地图中`（ObjNpc.pas:267）。</summary>
    public bool m_boIsHide;
    /// <summary>原文 `m_boIsQuest: Boolean; // 0x55D NPC类型为地图任务型的，加载脚本时的脚本文件名为 角色名-地图号.txt`（ObjNpc.pas:268）。</summary>
    public bool m_boIsQuest;
    /// <summary>原文 `m_sPath: string; // 0x560`（ObjNpc.pas:269）。</summary>
    public string m_sPath = "";
    /// <summary>原文 `m_boNpcAutoChangeColor: Boolean;`（ObjNpc.pas:270）。</summary>
    public bool m_boNpcAutoChangeColor;
    /// <summary>原文 `m_dwNpcAutoChangeColorTick: LongWord;`（ObjNpc.pas:271）。</summary>
    public uint m_dwNpcAutoChangeColorTick;
    /// <summary>原文 `m_dwNpcAutoChangeColorTime: LongWord;`（ObjNpc.pas:272）。</summary>
    public uint m_dwNpcAutoChangeColorTime;
    /// <summary>原文 `m_nNpcAutoChangeIdx: Integer;`（ObjNpc.pas:273）。</summary>
    public int m_nNpcAutoChangeIdx;
    /// <summary>原文 `m_nGlobalAValIndex: Integer;`（ObjNpc.pas:274）。</summary>
    public int m_nGlobalAValIndex;
    /// <summary>原文 `m_sDynamicName: string;`（ObjNpc.pas:275）。</summary>
    public string m_sDynamicName = "";
    /// <summary>原文 `m_NoUserSelectList: TStringList;`（ObjNpc.pas:276）。</summary>
    public TStringList m_NoUserSelectList = new();
    /// <summary>原文 `m_boMapEvent: Boolean;`（ObjNpc.pas:277）。</summary>
    public bool m_boMapEvent;
    /// <summary>原文 `m_boPlug: Boolean;`（ObjNpc.pas:278）。</summary>
    public bool m_boPlug;
    /// <summary>原文 `m_sLastLabel: string;`（ObjNpc.pas:279）。</summary>
    public string m_sLastLabel = "";
    // 原文 `// 附加数据 chonchong 2015-01-15`（ObjNpc.pas:280）—— 原文注释，保留。
    /// <summary>原文 `m_sDataFileName: string;`（ObjNpc.pas:281）。</summary>
    public string m_sDataFileName = "";
    /// <summary>原文 `m_sAddName: string;`（ObjNpc.pas:282）。</summary>
    public string m_sAddName = "";
    /// <summary>原文 `m_nAddLevel: Integer;`（ObjNpc.pas:283）。</summary>
    public int m_nAddLevel;
    /// <summary>原文 `m_boGrayShow: Boolean;`（ObjNpc.pas:284）。</summary>
    public bool m_boGrayShow;
    /// <summary>原文 `m_boScaleShow: Boolean;`（ObjNpc.pas:285）。</summary>
    public bool m_boScaleShow;
    /// <summary>原文 `m_nEffigyState: TFeature_New;`（ObjNpc.pas:286）。</summary>
    public TFeature_New m_nEffigyState;
    /// <summary>原文 `m_nEffigyOffset: Integer;`（ObjNpc.pas:287）。</summary>
    public int m_nEffigyOffset;
    /// <summary>原文 `m_nValidTime: Integer;`（ObjNpc.pas:288）。</summary>
    public int m_nValidTime;
    /// <summary>原文 `m_wValidTimeTick: LongWord;`（ObjNpc.pas:289）。</summary>
    public uint m_wValidTimeTick;
    /// <summary>原文 `m_dwScriptCRC: LongWord;`（ObjNpc.pas:290）。</summary>
    public uint m_dwScriptCRC;
    /// <summary>原文 `m_dwIconFileCRC: LongWord;`（ObjNpc.pas:291）。</summary>
    public uint m_dwIconFileCRC;
    /// <summary>原文 `m_CallFileListCRC: TStringList;`（ObjNpc.pas:292）。</summary>
    public TStringList m_CallFileListCRC = new();
}

/// <summary>
/// 原文 `TMerchant = class(TNormNpc) // 0x594`（ObjNpc.pas:334）。
/// </summary>
public partial class TMerchant : TNormNpc
{
    /// <summary>原文 `n56C: Integer;`（ObjNpc.pas:335）。</summary>
    public int n56C;
    /// <summary>原文 `m_nPriceRate: Integer; // 0x570   物品价格倍率 默认为 100%`（ObjNpc.pas:336）。</summary>
    public int m_nPriceRate = 100;
    /// <summary>原文 `bo574: Boolean;`（ObjNpc.pas:337）。</summary>
    public bool bo574;
    /// <summary>原文 `m_boCastle: Boolean; // 0x575`（ObjNpc.pas:338）。</summary>
    public bool m_boCastle;
    /// <summary>原文 `dwRefillGoodsTick: LongWord; // 0x578`（ObjNpc.pas:339）。</summary>
    public uint dwRefillGoodsTick;
    /// <summary>原文 `dwClearExpreUpgradeTick: LongWord; // 0x57C`（ObjNpc.pas:340）。</summary>
    public uint dwClearExpreUpgradeTick;
    /// <summary>原文 `m_ItemTypeList: TList; // 0x580  NPC买卖物品类型列表，脚本中前面的 +1 +30 之类的`（ObjNpc.pas:341）。</summary>
    public List<object> m_ItemTypeList = new();
    /// <summary>原文 `m_RefillGoodsList: TList; // 0x584`（ObjNpc.pas:342）。</summary>
    public List<object> m_RefillGoodsList = new();
    /// <summary>原文 `m_GoodsList: TList; // 0x588`（ObjNpc.pas:343）。</summary>
    public List<object> m_GoodsList = new();
    /// <summary>原文 `m_ItemPriceList: TList; // 0x58C`（ObjNpc.pas:344）。</summary>
    public List<object> m_ItemPriceList = new();
    /// <summary>原文 `m_UpgradeWeaponList: TList;`（ObjNpc.pas:345）。</summary>
    public List<object> m_UpgradeWeaponList = new();
    /// <summary>原文 `m_boCanMove: Boolean;`（ObjNpc.pas:346）。</summary>
    public bool m_boCanMove;
    /// <summary>原文 `m_dwMoveTime: LongWord;`（ObjNpc.pas:347）。</summary>
    public uint m_dwMoveTime;
    /// <summary>原文 `m_dwMoveTick: LongWord;`（ObjNpc.pas:348）。</summary>
    public uint m_dwMoveTick;
    /// <summary>原文 `m_boBuy: Boolean;`（ObjNpc.pas:349）。</summary>
    public bool m_boBuy;
    /// <summary>原文 `m_boSell: Boolean;`（ObjNpc.pas:350）。</summary>
    public bool m_boSell;
    /// <summary>原文 `m_boMakeDrug: Boolean;`（ObjNpc.pas:351）。</summary>
    public bool m_boMakeDrug;
    /// <summary>原文 `m_boPrices: Boolean;`（ObjNpc.pas:352）。</summary>
    public bool m_boPrices;
    /// <summary>原文 `m_boStorage: Boolean;`（ObjNpc.pas:353）。</summary>
    public bool m_boStorage;
    /// <summary>原文 `m_boGetback: Boolean;`（ObjNpc.pas:354）。</summary>
    public bool m_boGetback;
    /// <summary>原文 `m_boBigStorage: Boolean;`（ObjNpc.pas:355）。</summary>
    public bool m_boBigStorage;
    /// <summary>原文 `m_boBigGetBack: Boolean;`（ObjNpc.pas:356）。</summary>
    public bool m_boBigGetBack;
    /// <summary>原文 `m_boGetNextPage: Boolean;`（ObjNpc.pas:357）。</summary>
    public bool m_boGetNextPage;
    /// <summary>原文 `m_boGetPreviousPage: Boolean;`（ObjNpc.pas:358）。</summary>
    public bool m_boGetPreviousPage;
    /// <summary>原文 `m_boUpgradenow: Boolean;`（ObjNpc.pas:359）。</summary>
    public bool m_boUpgradenow;
    /// <summary>原文 `m_boGetBackupgnow: Boolean;`（ObjNpc.pas:360）。</summary>
    public bool m_boGetBackupgnow;
    /// <summary>原文 `m_boRepair: Boolean;`（ObjNpc.pas:361）。</summary>
    public bool m_boRepair;
    /// <summary>原文 `m_boS_repair: Boolean;`（ObjNpc.pas:362）。</summary>
    public bool m_boS_repair;
    /// <summary>原文 `m_boSendmsg: Boolean;`（ObjNpc.pas:363）。</summary>
    public bool m_boSendmsg;
    /// <summary>原文 `m_boGetMarry: Boolean;`（ObjNpc.pas:364）。</summary>
    public bool m_boGetMarry;
    /// <summary>原文 `m_boGetMaster: Boolean;`（ObjNpc.pas:365）。</summary>
    public bool m_boGetMaster;
    /// <summary>原文 `m_boUseItemName: Boolean;`（ObjNpc.pas:366）。</summary>
    public bool m_boUseItemName;
    /// <summary>原文 `m_boArmRemoveStone: Boolean;`（ObjNpc.pas:367）。</summary>
    public bool m_boArmRemoveStone;
    /// <summary>原文 `m_boGetSellGold: Boolean;`（ObjNpc.pas:368）。</summary>
    public bool m_boGetSellGold;
    /// <summary>原文 `m_boSellOff: Boolean;`（ObjNpc.pas:369）。</summary>
    public bool m_boSellOff;
    /// <summary>原文 `m_boBuyOff: Boolean;`（ObjNpc.pas:370）。</summary>
    public bool m_boBuyOff;
    /// <summary>原文 `m_boofflinemsg: Boolean;`（ObjNpc.pas:371）。</summary>
    public bool m_boofflinemsg;
    /// <summary>原文 `m_boDealGold: Boolean;`（ObjNpc.pas:372）。</summary>
    public bool m_boDealGold;
    /// <summary>原文 `m_boCreateHeroName: Boolean;`（ObjNpc.pas:373）。</summary>
    public bool m_boCreateHeroName;
    /// <summary>原文 `m_boUpgradeNew: Boolean;`（ObjNpc.pas:374）。</summary>
    public bool m_boUpgradeNew;
    /// <summary>原文 `m_boPleaseDrink: Boolean; // 请酒`（ObjNpc.pas:375）。</summary>
    public bool m_boPleaseDrink;
    /// <summary>原文 `m_boMakeWine: Boolean; // 酿酒NPC`（ObjNpc.pas:376）。</summary>
    public bool m_boMakeWine;
    /// <summary>原文 `m_boBuHero: Boolean; // 卧龙英雄`（ObjNpc.pas:377）。</summary>
    public bool m_boBuHero;
    /// <summary>原文 `m_boReclaimItem: Boolean;`（ObjNpc.pas:378）。</summary>
    public bool m_boReclaimItem;
    /// <summary>原文 `m_boFB: Boolean; // 副本地图 -- 是否为副本NPC chongchong 2013-09-11`（ObjNpc.pas:379）。</summary>
    public bool m_boFB;
    /// <summary>原文 `m_sFBName: string; // 副本地图 -- 副本地图名 chongchong 2013-09-11`（ObjNpc.pas:380）。</summary>
    public string m_sFBName = "";

    // ---- private（ObjNpc.pas:381-384）----
    /// <summary>原文 `nIdx: Integer;`（ObjNpc.pas:382）。</summary>
    public int nIdx;
    /// <summary>原文 `nKeepCount: Integer;`（ObjNpc.pas:383）。</summary>
    public int nKeepCount;
    /// <summary>原文 `nKeepMaxCount: Integer;`（ObjNpc.pas:384）。</summary>
    public int nKeepMaxCount;

    // ---- public（ObjNpc.pas:385-386）----
    /// <summary>原文 `MovePoint: array of TPoint;`（ObjNpc.pas:386）—— 动态数组，托管侧 `(int X, int Y)[]`。</summary>
    public (int X, int Y)[] MovePoint = Array.Empty<(int, int)>();

    // ---- private（ObjNpc.pas:387-389）----
    /// <summary>原文 `FLastTrunTick: DWORD;`（ObjNpc.pas:388）。</summary>
    public uint FLastTrunTick;
    /// <summary>原文 `FTrunTimeInterval: DWORD; // 转弯时间间隔`（ObjNpc.pas:389）。</summary>
    public uint FTrunTimeInterval;
}

/// <summary>原文 `TGuildOfficial = class(TNormNpc) // 0x568`（ObjNpc.pas:432）—— 无自有字段。</summary>
public partial class TGuildOfficial : TNormNpc
{
}

/// <summary>原文 `TTrainer = class(TNormNpc) // 0x574`（ObjNpc.pas:449）。</summary>
public partial class TTrainer : TNormNpc
{
    /// <summary>原文 `n564: LongWord; // Integer;`（ObjNpc.pas:450）。</summary>
    public uint n564;
    /// <summary>原文 `m_dw568: LongWord;`（ObjNpc.pas:451）。</summary>
    public uint m_dw568;
    /// <summary>原文 `n56C: Int64; // Integer;`（ObjNpc.pas:452）。</summary>
    public long n56C;
    /// <summary>原文 `n570: LongWord; // Integer;`（ObjNpc.pas:453）。</summary>
    public uint n570;
}

/// <summary>原文 `TBoxMonster = class(TAnimalObject) // 0x574`（ObjNpc.pas:461）—— 无自有字段。</summary>
public partial class TBoxMonster : TCreature
{
}

/// <summary>
/// 原文 `TCastleOfficial = class(TMerchant)`（ObjNpc.pas:471）—— 无自有字段。
/// 上一行原文 `// TCastleManager = class(TMerchant)`（ObjNpc.pas:470）为注释，保留。
/// </summary>
public partial class TCastleOfficial : TMerchant
{
}

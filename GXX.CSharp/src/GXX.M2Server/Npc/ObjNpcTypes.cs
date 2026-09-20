// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：ObjNpc.pas **interface 段 25..495** 的全部记录/类声明 1:1 落地。
//   · 常量 CMD_RACE_*（10-22）→ ObjNpcSeams.cs 的 ObjNpcConst
//   · TUpgradeInfo 27-38 / pTUpgradeInfo 40
//   · TItemPrice 42-45 / pTItemPrice 47
//   · TGoods 49-54 / pTGoods 56
//   · TSellItemPrice 58-61 / pTSellItemPrice 63
//   · TQuestActionInfo 65-132 / pTQuestActionInfo 134
//   · TQuestConditionInfo 136-204 / pTQuestConditionInfo 206
//   · TScriptParamter 208-213 / PScriptParamter 215
//   · TConditionType 217 / TConditionList 219-227 / TSayingProcedure 229-235
//   · TSayingRecord 239-243 / TTimeLabel 247-258
//   · TNormNpc 262-332 / TMerchant 334-430 / TGuildOfficial 432-446
//   · TTrainer 449-459 / TBoxMonster 461-468 / TCastleOfficial 471-484
//   · 四个单元级函数声明 486-494 → ObjNpcLevelBase.cs
//
// 未覆盖方法的登记见 ObjNpcUnported.cs（逐条带原文行号）。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

/// <summary>原文 `TUpgradeInfo = record // 0x40`（ObjNpc.pas:27-38）。</summary>
public class TUpgradeInfo
{
    /// <summary>原文 `sUserName: string[30]; // 0x00`（ObjNpc.pas:28）。</summary>
    public string sUserName = "";
    /// <summary>原文 `UserItem: TUserItem; // 0x10`（ObjNpc.pas:29）。</summary>
    public TUserItem UserItem;
    /// <summary>原文 `btDc: Byte; // 0x28`（ObjNpc.pas:30）。</summary>
    public byte btDc;
    /// <summary>原文 `btSc: Byte; // 0x29`（ObjNpc.pas:31）。</summary>
    public byte btSc;
    /// <summary>原文 `btMc: Byte; // 0x2A`（ObjNpc.pas:32）。</summary>
    public byte btMc;
    /// <summary>原文 `btDura: Byte; // 0x2B`（ObjNpc.pas:33）。</summary>
    public byte btDura;
    /// <summary>原文 `n2C: Integer;`（ObjNpc.pas:34）。</summary>
    public int n2C;
    /// <summary>原文 `dtTime: TDateTime; // 0x30`（ObjNpc.pas:35）。</summary>
    public DateTime dtTime;
    /// <summary>原文 `dwGetBackTick: LongWord; // 0x38`（ObjNpc.pas:36）。</summary>
    public uint dwGetBackTick;
    /// <summary>原文 `n3C: Integer;`（ObjNpc.pas:37）。</summary>
    public int n3C;
}

/// <summary>原文 `TItemPrice = record wIndex: Word; nPrice: Integer; end;`（ObjNpc.pas:42-45）。</summary>
public class TItemPrice
{
    /// <summary>原文 `wIndex: Word;`（ObjNpc.pas:43）。</summary>
    public ushort wIndex;
    /// <summary>原文 `nPrice: Integer;`（ObjNpc.pas:44）。</summary>
    public int nPrice;
}

/// <summary>原文 `TGoods = record // 0x1C`（ObjNpc.pas:49-54）。</summary>
public class TGoods
{
    /// <summary>原文 `sItemName: string[30];`（ObjNpc.pas:50）。</summary>
    public string sItemName = "";
    /// <summary>原文 `nCount: Integer;`（ObjNpc.pas:51）。</summary>
    public int nCount;
    /// <summary>原文 `dwRefillTime: LongWord;`（ObjNpc.pas:52）。</summary>
    public uint dwRefillTime;
    /// <summary>原文 `dwRefillTick: LongWord;`（ObjNpc.pas:53）。</summary>
    public uint dwRefillTick;
}

/// <summary>原文 `TSellItemPrice = record wIndex: Word; nPrice: Integer; end;`（ObjNpc.pas:58-61）。</summary>
public class TSellItemPrice
{
    /// <summary>原文 `wIndex: Word;`（ObjNpc.pas:59）。</summary>
    public ushort wIndex;
    /// <summary>原文 `nPrice: Integer;`（ObjNpc.pas:60）。</summary>
    public int nPrice;
}

/// <summary>
/// 原文 `TQuestActionInfo = record // 0x1C`（ObjNpc.pas:65-132）。
/// `ScriptList: array of string` / `ScriptCmd: array of Integer` → 托管侧 `string[]` / `int[]`。
/// </summary>
public class TQuestActionInfo
{
    /// <summary>原文 `ScriptList: array of string;`（ObjNpc.pas:66）。</summary>
    public string[] ScriptList = Array.Empty<string>();
    /// <summary>原文 `ScriptCmd: array of Integer;`（ObjNpc.pas:67）。</summary>
    public int[] ScriptCmd = Array.Empty<int>();
    /// <summary>原文 `nCMDCode: Integer; // 0x00`（ObjNpc.pas:68）。</summary>
    public int nCMDCode;
    /// <summary>原文 `sCmd: string;`（ObjNpc.pas:69）。</summary>
    public string sCmd = "";
    /// <summary>原文 `sCmdLine: string;`（ObjNpc.pas:70）。</summary>
    public string sCmdLine = "";
    // 原文 `// sParams: string;`（ObjNpc.pas:71）—— 原文即已注释掉，此处保留注释不留字段。
    /// <summary>原文 `sParam1: string; // 0x04`（ObjNpc.pas:72）。</summary>
    public string sParam1 = "";
    /// <summary>原文 `nParam1: Integer; // 0x08`（ObjNpc.pas:73）。</summary>
    public int nParam1;
    /// <summary>原文 `sParam2: string; // 0x0C`（ObjNpc.pas:74）。</summary>
    public string sParam2 = "";
    /// <summary>原文 `nParam2: Integer; // 0x10`（ObjNpc.pas:75）。</summary>
    public int nParam2;
    /// <summary>原文 `sParam3: string; // 0x14`（ObjNpc.pas:76）。</summary>
    public string sParam3 = "";
    /// <summary>原文 `nParam3: Integer; // 0x18`（ObjNpc.pas:77）。</summary>
    public int nParam3;
    /// <summary>原文 `sParam4: string;`（ObjNpc.pas:78）。</summary>
    public string sParam4 = "";
    /// <summary>原文 `nParam4: Integer;`（ObjNpc.pas:79）。</summary>
    public int nParam4;
    /// <summary>原文 `sParam5: string;`（ObjNpc.pas:80）。</summary>
    public string sParam5 = "";
    /// <summary>原文 `nParam5: Integer;`（ObjNpc.pas:81）。</summary>
    public int nParam5;
    /// <summary>原文 `sParam6: string;`（ObjNpc.pas:82）。</summary>
    public string sParam6 = "";
    /// <summary>原文 `nParam6: Integer;`（ObjNpc.pas:83）。</summary>
    public int nParam6;
    /// <summary>原文 `sParam7: string;`（ObjNpc.pas:84）。</summary>
    public string sParam7 = "";
    /// <summary>原文 `nParam7: Integer;`（ObjNpc.pas:85）。</summary>
    public int nParam7;
    /// <summary>原文 `sParam8: string;`（ObjNpc.pas:86）。</summary>
    public string sParam8 = "";
    /// <summary>原文 `nParam8: Integer;`（ObjNpc.pas:87）。</summary>
    public int nParam8;
    /// <summary>原文 `sParam9: string;`（ObjNpc.pas:88）。</summary>
    public string sParam9 = "";
    /// <summary>原文 `nParam9: Integer;`（ObjNpc.pas:89）。</summary>
    public int nParam9;
    /// <summary>原文 `sParam10: string;`（ObjNpc.pas:90）。</summary>
    public string sParam10 = "";
    /// <summary>原文 `nParam10: Integer;`（ObjNpc.pas:91）。</summary>
    public int nParam10;

    /// <summary>原文 `sRawParam1: string;`（ObjNpc.pas:92）。</summary>
    public string sRawParam1 = "";
    /// <summary>原文 `sRawParam2: string;`（ObjNpc.pas:93）。</summary>
    public string sRawParam2 = "";
    /// <summary>原文 `sRawParam3: string;`（ObjNpc.pas:94）。</summary>
    public string sRawParam3 = "";
    /// <summary>原文 `sRawParam4: string;`（ObjNpc.pas:95）。</summary>
    public string sRawParam4 = "";
    /// <summary>原文 `sRawParam5: string;`（ObjNpc.pas:96）。</summary>
    public string sRawParam5 = "";
    /// <summary>原文 `sRawParam6: string;`（ObjNpc.pas:97）。</summary>
    public string sRawParam6 = "";
    /// <summary>原文 `sRawParam7: string;`（ObjNpc.pas:98）。</summary>
    public string sRawParam7 = "";
    /// <summary>原文 `sRawParam8: string;`（ObjNpc.pas:99）。</summary>
    public string sRawParam8 = "";
    /// <summary>原文 `sRawParam9: string;`（ObjNpc.pas:100）。</summary>
    public string sRawParam9 = "";
    /// <summary>原文 `sRawParam10: string;`（ObjNpc.pas:101）。</summary>
    public string sRawParam10 = "";

    /// <summary>原文 `sSubParam1: string;`（ObjNpc.pas:102）。</summary>
    public string sSubParam1 = "";
    /// <summary>原文 `sSubParam2: string;`（ObjNpc.pas:103）。</summary>
    public string sSubParam2 = "";
    /// <summary>原文 `sSubParam3: string;`（ObjNpc.pas:104）。</summary>
    public string sSubParam3 = "";
    /// <summary>原文 `sSubParam4: string;`（ObjNpc.pas:105）。</summary>
    public string sSubParam4 = "";
    /// <summary>原文 `sSubParam5: string;`（ObjNpc.pas:106）。</summary>
    public string sSubParam5 = "";
    /// <summary>原文 `sSubParam6: string;`（ObjNpc.pas:107）。</summary>
    public string sSubParam6 = "";
    /// <summary>原文 `sSubParam7: string;`（ObjNpc.pas:108）。</summary>
    public string sSubParam7 = "";
    /// <summary>原文 `sSubParam8: string;`（ObjNpc.pas:109）。</summary>
    public string sSubParam8 = "";
    /// <summary>原文 `sSubParam9: string;`（ObjNpc.pas:110）。</summary>
    public string sSubParam9 = "";
    /// <summary>原文 `sSubParam10: string;`（ObjNpc.pas:111）。</summary>
    public string sSubParam10 = "";

    /// <summary>原文 `VarInfo1: TVarInfo;`（ObjNpc.pas:112）。</summary>
    public TVarInfo VarInfo1;
    /// <summary>原文 `VarInfo2: TVarInfo;`（ObjNpc.pas:113）。</summary>
    public TVarInfo VarInfo2;
    /// <summary>原文 `VarInfo3: TVarInfo;`（ObjNpc.pas:114）。</summary>
    public TVarInfo VarInfo3;
    /// <summary>原文 `VarInfo4: TVarInfo;`（ObjNpc.pas:115）。</summary>
    public TVarInfo VarInfo4;
    /// <summary>原文 `VarInfo5: TVarInfo;`（ObjNpc.pas:116）。</summary>
    public TVarInfo VarInfo5;
    /// <summary>原文 `VarInfo6: TVarInfo;`（ObjNpc.pas:117）。</summary>
    public TVarInfo VarInfo6;
    /// <summary>原文 `VarInfo7: TVarInfo;`（ObjNpc.pas:118）。</summary>
    public TVarInfo VarInfo7;
    /// <summary>原文 `VarInfo8: TVarInfo;`（ObjNpc.pas:119）。</summary>
    public TVarInfo VarInfo8;
    /// <summary>原文 `VarInfo9: TVarInfo;`（ObjNpc.pas:120）。</summary>
    public TVarInfo VarInfo9;
    /// <summary>原文 `VarInfo10: TVarInfo;`（ObjNpc.pas:121）。</summary>
    public TVarInfo VarInfo10;

    /// <summary>原文 `boCompleteFormat1: Boolean;`（ObjNpc.pas:122）。</summary>
    public bool boCompleteFormat1;
    /// <summary>原文 `boCompleteFormat2: Boolean;`（ObjNpc.pas:123）。</summary>
    public bool boCompleteFormat2;
    /// <summary>原文 `boCompleteFormat3: Boolean;`（ObjNpc.pas:124）。</summary>
    public bool boCompleteFormat3;
    /// <summary>原文 `boCompleteFormat4: Boolean;`（ObjNpc.pas:125）。</summary>
    public bool boCompleteFormat4;
    /// <summary>原文 `boCompleteFormat5: Boolean;`（ObjNpc.pas:126）。</summary>
    public bool boCompleteFormat5;
    /// <summary>原文 `boCompleteFormat6: Boolean;`（ObjNpc.pas:127）。</summary>
    public bool boCompleteFormat6;
    /// <summary>原文 `boCompleteFormat7: Boolean;`（ObjNpc.pas:128）。</summary>
    public bool boCompleteFormat7;
    /// <summary>原文 `boCompleteFormat8: Boolean;`（ObjNpc.pas:129）。</summary>
    public bool boCompleteFormat8;
    /// <summary>原文 `boCompleteFormat9: Boolean;`（ObjNpc.pas:130）。</summary>
    public bool boCompleteFormat9;
    /// <summary>原文 `boCompleteFormat10: Boolean;`（ObjNpc.pas:131）。</summary>
    public bool boCompleteFormat10;
}

/// <summary>
/// 原文 `TQuestConditionInfo = record // 0x14`（ObjNpc.pas:136-204）。
/// 与 TQuestActionInfo 的差异：多 `boNot`（139）与 `sParam`（143），且字段顺序里
/// `boNot` 在 `nCMDCode` **之前** —— 照抄，不得"顺手对齐"。
/// </summary>
public class TQuestConditionInfo
{
    /// <summary>原文 `ScriptList: array of string;`（ObjNpc.pas:137）。</summary>
    public string[] ScriptList = Array.Empty<string>();
    /// <summary>原文 `ScriptCmd: array of Integer;`（ObjNpc.pas:138）。</summary>
    public int[] ScriptCmd = Array.Empty<int>();
    /// <summary>原文 `boNot: Boolean; // 是否取反`（ObjNpc.pas:139）。</summary>
    public bool boNot;
    /// <summary>原文 `nCMDCode: Integer; // 0x00`（ObjNpc.pas:140）。</summary>
    public int nCMDCode;
    /// <summary>原文 `sCmd: string;`（ObjNpc.pas:141）。</summary>
    public string sCmd = "";
    /// <summary>原文 `sCmdLine: string;`（ObjNpc.pas:142）。</summary>
    public string sCmdLine = "";
    /// <summary>原文 `sParam: string;`（ObjNpc.pas:143）。</summary>
    public string sParam = "";
    /// <summary>原文 `sParam1: string; // 0x04`（ObjNpc.pas:144）。</summary>
    public string sParam1 = "";
    /// <summary>原文 `nParam1: Integer; // 0x08`（ObjNpc.pas:145）。</summary>
    public int nParam1;
    /// <summary>原文 `sParam2: string; // 0x0C`（ObjNpc.pas:146）。</summary>
    public string sParam2 = "";
    /// <summary>原文 `nParam2: Integer; // 0x10`（ObjNpc.pas:147）。</summary>
    public int nParam2;
    /// <summary>原文 `sParam3: string;`（ObjNpc.pas:148）。</summary>
    public string sParam3 = "";
    /// <summary>原文 `nParam3: Integer;`（ObjNpc.pas:149）。</summary>
    public int nParam3;
    /// <summary>原文 `sParam4: string;`（ObjNpc.pas:150）。</summary>
    public string sParam4 = "";
    /// <summary>原文 `nParam4: Integer;`（ObjNpc.pas:151）。</summary>
    public int nParam4;
    /// <summary>原文 `sParam5: string;`（ObjNpc.pas:152）。</summary>
    public string sParam5 = "";
    /// <summary>原文 `nParam5: Integer;`（ObjNpc.pas:153）。</summary>
    public int nParam5;
    /// <summary>原文 `sParam6: string;`（ObjNpc.pas:154）。</summary>
    public string sParam6 = "";
    /// <summary>原文 `nParam6: Integer;`（ObjNpc.pas:155）。</summary>
    public int nParam6;
    /// <summary>原文 `sParam7: string;`（ObjNpc.pas:156）。</summary>
    public string sParam7 = "";
    /// <summary>原文 `nParam7: Integer;`（ObjNpc.pas:157）。</summary>
    public int nParam7;
    /// <summary>原文 `sParam8: string;`（ObjNpc.pas:158）。</summary>
    public string sParam8 = "";
    /// <summary>原文 `nParam8: Integer;`（ObjNpc.pas:159）。</summary>
    public int nParam8;
    /// <summary>原文 `sParam9: string;`（ObjNpc.pas:160）。</summary>
    public string sParam9 = "";
    /// <summary>原文 `nParam9: Integer;`（ObjNpc.pas:161）。</summary>
    public int nParam9;
    /// <summary>原文 `sParam10: string;`（ObjNpc.pas:162）。</summary>
    public string sParam10 = "";
    /// <summary>原文 `nParam10: Integer;`（ObjNpc.pas:163）。</summary>
    public int nParam10;

    /// <summary>原文 `sRawParam1: string;`（ObjNpc.pas:164）。</summary>
    public string sRawParam1 = "";
    /// <summary>原文 `sRawParam2: string;`（ObjNpc.pas:165）。</summary>
    public string sRawParam2 = "";
    /// <summary>原文 `sRawParam3: string;`（ObjNpc.pas:166）。</summary>
    public string sRawParam3 = "";
    /// <summary>原文 `sRawParam4: string;`（ObjNpc.pas:167）。</summary>
    public string sRawParam4 = "";
    /// <summary>原文 `sRawParam5: string;`（ObjNpc.pas:168）。</summary>
    public string sRawParam5 = "";
    /// <summary>原文 `sRawParam6: string;`（ObjNpc.pas:169）。</summary>
    public string sRawParam6 = "";
    /// <summary>原文 `sRawParam7: string;`（ObjNpc.pas:170）。</summary>
    public string sRawParam7 = "";
    /// <summary>原文 `sRawParam8: string;`（ObjNpc.pas:171）。</summary>
    public string sRawParam8 = "";
    /// <summary>原文 `sRawParam9: string;`（ObjNpc.pas:172）。</summary>
    public string sRawParam9 = "";
    /// <summary>原文 `sRawParam10: string;`（ObjNpc.pas:173）。</summary>
    public string sRawParam10 = "";

    /// <summary>原文 `sSubParam1: string;`（ObjNpc.pas:174）。</summary>
    public string sSubParam1 = "";
    /// <summary>原文 `sSubParam2: string;`（ObjNpc.pas:175）。</summary>
    public string sSubParam2 = "";
    /// <summary>原文 `sSubParam3: string;`（ObjNpc.pas:176）。</summary>
    public string sSubParam3 = "";
    /// <summary>原文 `sSubParam4: string;`（ObjNpc.pas:177）。</summary>
    public string sSubParam4 = "";
    /// <summary>原文 `sSubParam5: string;`（ObjNpc.pas:178）。</summary>
    public string sSubParam5 = "";
    /// <summary>原文 `sSubParam6: string;`（ObjNpc.pas:179）。</summary>
    public string sSubParam6 = "";
    /// <summary>原文 `sSubParam7: string;`（ObjNpc.pas:180）。</summary>
    public string sSubParam7 = "";
    /// <summary>原文 `sSubParam8: string;`（ObjNpc.pas:181）。</summary>
    public string sSubParam8 = "";
    /// <summary>原文 `sSubParam9: string;`（ObjNpc.pas:182）。</summary>
    public string sSubParam9 = "";
    /// <summary>原文 `sSubParam10: string;`（ObjNpc.pas:183）。</summary>
    public string sSubParam10 = "";

    /// <summary>原文 `VarInfo1: TVarInfo;`（ObjNpc.pas:184）。</summary>
    public TVarInfo VarInfo1;
    /// <summary>原文 `VarInfo2: TVarInfo;`（ObjNpc.pas:185）。</summary>
    public TVarInfo VarInfo2;
    /// <summary>原文 `VarInfo3: TVarInfo;`（ObjNpc.pas:186）。</summary>
    public TVarInfo VarInfo3;
    /// <summary>原文 `VarInfo4: TVarInfo;`（ObjNpc.pas:187）。</summary>
    public TVarInfo VarInfo4;
    /// <summary>原文 `VarInfo5: TVarInfo;`（ObjNpc.pas:188）。</summary>
    public TVarInfo VarInfo5;
    /// <summary>原文 `VarInfo6: TVarInfo;`（ObjNpc.pas:189）。</summary>
    public TVarInfo VarInfo6;
    /// <summary>原文 `VarInfo7: TVarInfo;`（ObjNpc.pas:190）。</summary>
    public TVarInfo VarInfo7;
    /// <summary>原文 `VarInfo8: TVarInfo;`（ObjNpc.pas:191）。</summary>
    public TVarInfo VarInfo8;
    /// <summary>原文 `VarInfo9: TVarInfo;`（ObjNpc.pas:192）。</summary>
    public TVarInfo VarInfo9;
    /// <summary>原文 `VarInfo10: TVarInfo;`（ObjNpc.pas:193）。</summary>
    public TVarInfo VarInfo10;

    /// <summary>原文 `boCompleteFormat1: Boolean;`（ObjNpc.pas:194）。</summary>
    public bool boCompleteFormat1;
    /// <summary>原文 `boCompleteFormat2: Boolean;`（ObjNpc.pas:195）。</summary>
    public bool boCompleteFormat2;
    /// <summary>原文 `boCompleteFormat3: Boolean;`（ObjNpc.pas:196）。</summary>
    public bool boCompleteFormat3;
    /// <summary>原文 `boCompleteFormat4: Boolean;`（ObjNpc.pas:197）。</summary>
    public bool boCompleteFormat4;
    /// <summary>原文 `boCompleteFormat5: Boolean;`（ObjNpc.pas:198）。</summary>
    public bool boCompleteFormat5;
    /// <summary>原文 `boCompleteFormat6: Boolean;`（ObjNpc.pas:199）。</summary>
    public bool boCompleteFormat6;
    /// <summary>原文 `boCompleteFormat7: Boolean;`（ObjNpc.pas:200）。</summary>
    public bool boCompleteFormat7;
    /// <summary>原文 `boCompleteFormat8: Boolean;`（ObjNpc.pas:201）。</summary>
    public bool boCompleteFormat8;
    /// <summary>原文 `boCompleteFormat9: Boolean;`（ObjNpc.pas:202）。</summary>
    public bool boCompleteFormat9;
    /// <summary>原文 `boCompleteFormat10: Boolean;`（ObjNpc.pas:203）。</summary>
    public bool boCompleteFormat10;
}

/// <summary>
/// 原文 `TScriptParamter = record`（ObjNpc.pas:208-213）。
/// 三个对象字段用托管既有接缝类型；`TPlayObject` 即 `GXX.M2Server.Engine.TPlayObject`。
/// </summary>
public class TScriptParamter
{
    /// <summary>原文 `Npc: TNormNpc;`（ObjNpc.pas:209）。</summary>
    public TNormNpc? Npc;
    /// <summary>原文 `BaseObj: TBaseObject;`（ObjNpc.pas:210）。原文即命名为 `BaseObj`（非 `BaseObject`）。</summary>
    public TCreature? BaseObj;
    /// <summary>原文 `PlayObj: TPlayObject;`（ObjNpc.pas:211）。</summary>
    public TPlayObject? PlayObj;
    /// <summary>原文 `QuestActionInfo: TQuestActionInfo;`（ObjNpc.pas:212）。</summary>
    public TQuestActionInfo? QuestActionInfo;
}

/// <summary>原文 `TConditionType = (ct_and, ct_or);`（ObjNpc.pas:217）。</summary>
public enum TConditionType
{
    /// <summary>原文 `ct_and`。</summary>
    ct_and = 0,
    /// <summary>原文 `ct_or`。</summary>
    ct_or = 1,
}

/// <summary>
/// 原文 `TConditionList = class(TList)`（ObjNpc.pas:219-227）。
/// `TList` → 托管 `List&lt;object&gt;`；`Create` 已实现（原文 :504-507），
/// `FConditionType := ct_and` 且 **`FTrueCount` 不初始化**（原文如此，Delphi 字段默认 0）。
/// </summary>
public class TConditionList
{
    /// <summary>原文 `FTrueCount: Byte;`（ObjNpc.pas:221）。</summary>
    public byte FTrueCount;
    /// <summary>原文 `FConditionType: TConditionType;`（ObjNpc.pas:222）。</summary>
    public TConditionType FConditionType;

    /// <summary>原文 `ConditionList` 承载的元素（TList 内容）。</summary>
    public readonly List<object> Items = new();

    /// <summary>原文 `constructor Create;`（ObjNpc.pas:224；实现 :504-507）。</summary>
    public TConditionList()
    {
        FConditionType = TConditionType.ct_and;
    }

    /// <summary>原文 `property ConditionType: TConditionType read FConditionType write FConditionType;`（:225）。</summary>
    public TConditionType ConditionType
    {
        get => FConditionType;
        set => FConditionType = value;
    }

    /// <summary>原文 `property TrueCount: Byte read FTrueCount write FTrueCount;`（:226）。</summary>
    public byte TrueCount
    {
        get => FTrueCount;
        set => FTrueCount = value;
    }
}

/// <summary>原文 `TSayingProcedure = record // 0x14`（ObjNpc.pas:229-235）。</summary>
public class TSayingProcedure
{
    /// <summary>原文 `ConditionList: TConditionList; // 0x00`（ObjNpc.pas:230）。</summary>
    public TConditionList? ConditionList;
    /// <summary>原文 `ActionList: TList; // 0x04`（ObjNpc.pas:231）。</summary>
    public List<object>? ActionList;
    /// <summary>原文 `sSayMsg: string; // 0x08`（ObjNpc.pas:232）。</summary>
    public string sSayMsg = "";
    /// <summary>原文 `ElseActionList: TList; // 0x0C`（ObjNpc.pas:233）。</summary>
    public List<object>? ElseActionList;
    /// <summary>原文 `sElseSayMsg: string; // 0x10`（ObjNpc.pas:234）。</summary>
    public string sElseSayMsg = "";
}

/// <summary>原文 `TSayingRecord = record // 0x08`（ObjNpc.pas:239-243）。</summary>
public class TSayingRecord
{
    /// <summary>原文 `sLabel: string;`（ObjNpc.pas:240）。</summary>
    public string sLabel = "";
    /// <summary>原文 `ProcedureList: TList; // 0x04`（ObjNpc.pas:241）。</summary>
    public List<object>? ProcedureList;
    /// <summary>原文 `boExtJmp: Boolean; // 是否允许外部跳转`（ObjNpc.pas:242）。</summary>
    public bool boExtJmp;
}

/// <summary>原文 `TTimeLabel = record`（ObjNpc.pas:247-258）。</summary>
public class TTimeLabel
{
    /// <summary>原文 `nType: Integer;`（ObjNpc.pas:248）。</summary>
    public int nType;
    /// <summary>原文 `nIndex: Integer;`（ObjNpc.pas:249）。</summary>
    public int nIndex;
    /// <summary>原文 `sLabel: string;`（ObjNpc.pas:250）。</summary>
    public string sLabel = "";
    /// <summary>原文 `dwTick: LongWord;`（ObjNpc.pas:251）。</summary>
    public uint dwTick;
    /// <summary>原文 `dwTime: LongWord;`（ObjNpc.pas:252）。</summary>
    public uint dwTime;
    /// <summary>原文 `boChangeMapDelete: Boolean;`（ObjNpc.pas:253）。</summary>
    public bool boChangeMapDelete;
    /// <summary>原文 `boDelete: Boolean;`（ObjNpc.pas:254）。</summary>
    public bool boDelete;
    /// <summary>原文 `Npc: TNormNpc;`（ObjNpc.pas:255）。</summary>
    public TNormNpc? Npc;
    /// <summary>原文 `Envir: TObject;`（ObjNpc.pas:256）—— 原文即为 TObject，非 TEnvirnoment。</summary>
    public object? Envir;
    /// <summary>原文 `nCount: Integer;`（ObjNpc.pas:257）。</summary>
    public int nCount;
}

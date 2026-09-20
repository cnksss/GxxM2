// 源单元：Source/M2Engine/SqliteM2DataDB.pas / MySqlM2DataDB.pas / SqliteAuctionDB.pas /
//          MySqlAuctionDB.pas（本车道四个单元的共用支撑面）
//
// 本文件只放"四个单元共同需要、但不属于 DbSeam.cs（上一车道已冻结）语义"的三块内容：
//
//   1. M2ItemDbAccess —— <c>TUserItem</c>（GXX.Core/Protocol/Grobal2.Types6.cs）在托管侧是
//      unsafe struct，只公开了 btValue / btNewValue / Flutes 三个访问器，缺
//      btAddDataByte / nAddDataInt / sAddDataText / CustomProperty.Properties / Progress，
//      而这四个单元的 DoLoadItemFromDB / DoSaveItemToDB **逐个字段**读写它们。
//      GXX.DBServer 侧已有同语义的 MySqlRoleDB.ItemAccess.cs，但那个类在别的程序集/命名空间，
//      且本车道的字段集合更大；按"不另造第二套接缝、就近提供同语义访问器"的原则，
//      这里提供 DbLayer 自己的访问器（对应原文 `UserItem.btValue[J]` 这类直接下标）。
//
//   2. AuctionPlayer/AuctionStdItem 视图 —— AuctionDB 的 DoRun 需要在线玩家与 StdItem 的
//      **少量字段**。原文直接引用 ObjPlayer/UsrEngn 的具体类型（未移植），
//      按 docs/转换开发文档.md §2.3 以最小接口表达原文实际用到的字段，测试注入内存实现。
//
//   3. AuctionDbRunSeam —— AuctionDB DoRun 的宿主能力（在线玩家、HumanChangeGold、
//      g_FunctionNPC.GotoLable、AddGameDataLog、g_boGameLog*）。
//      接缝：待 ObjPlayer/UsrEngn/DataEngn/M2Share 移植后接入；默认实现"无宿主"。
//
// 本文件属本车道独占区（DbLayer/AuctionDb*.cs 的同族支撑文件）。

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server.DbLayer;

/// <summary>
/// <c>TUserItem</c> 的 fixed-buffer 字段访问器（原文这些字段都是可下标数组）。
/// 语义与原文字面一致：<c>UserItem.btValue[Index] := Value</c> / <c>:= UserItem.btValue[J]</c>。
/// </summary>
public static unsafe class M2ItemDbAccess
{
    // ---------------- btValue: array[0..13] of Integer ----------------

    /// <summary>原文 <c>UserItem.btValue[Index]</c>。</summary>
    public static int GetValue(ref TUserItem item, int index)
    {
        fixed (TUserItem* p = &item) { return p->btValue[index]; }
    }

    /// <summary>原文 <c>UserItem.btValue[Index] := Value</c>。</summary>
    public static void SetValue(ref TUserItem item, int index, int value)
    {
        fixed (TUserItem* p = &item) { p->btValue[index] = value; }
    }

    // ---------------- btNewValue: array[0..29] of Word ----------------

    public static int GetNewValue(ref TUserItem item, int index)
    {
        fixed (TUserItem* p = &item) { return p->btNewValue[index]; }
    }

    public static void SetNewValue(ref TUserItem item, int index, int value)
    {
        fixed (TUserItem* p = &item) { p->btNewValue[index] = (ushort)value; }
    }

    // ---------------- btAddDataByte: array[0..19] of Byte ----------------

    public static int GetAddDataByte(ref TUserItem item, int index)
    {
        fixed (TUserItem* p = &item) { return p->btAddDataByte[index]; }
    }

    public static void SetAddDataByte(ref TUserItem item, int index, int value)
    {
        fixed (TUserItem* p = &item) { p->btAddDataByte[index] = (byte)value; }
    }

    // ---------------- nAddDataInt: array[0..9] of Integer ----------------

    public static int GetAddDataInt(ref TUserItem item, int index)
    {
        fixed (TUserItem* p = &item) { return p->nAddDataInt[index]; }
    }

    public static void SetAddDataInt(ref TUserItem item, int index, int value)
    {
        fixed (TUserItem* p = &item) { p->nAddDataInt[index] = value; }
    }

    // ---------------- sAddDataText: array[0..1] of string[20] ----------------

    /// <summary>原文 <c>UserItem.sAddDataText[Index]</c>。</summary>
    public static string GetAddDataText(ref TUserItem item, int index) => item.GetAddDataText(index);

    /// <summary>原文 <c>UserItem.sAddDataText[Index] := Value</c>（string[20]，按 GBK 截断）。</summary>
    public static void SetAddDataText(ref TUserItem item, int index, string value) => item.SetAddDataText(index, value);

    // ---------------- Progress: array[0..1] of TUserItemProgress ----------------

    /// <summary>原文 <c>UserItem.Progress[Index]</c>（值拷贝，与 Delphi 记录下标读取一致）。</summary>
    public static TUserItemProgress GetProgress(ref TUserItem item, int index)
    {
        fixed (TUserItem* p = &item) { return index == 0 ? p->Progress0 : p->Progress1; }
    }

    /// <summary>原文 <c>UserItem.Progress[Index].Xxx := ...</c>（整槽回写）。</summary>
    public static void SetProgress(ref TUserItem item, int index, TUserItemProgress value)
    {
        fixed (TUserItem* p = &item) { if (index == 0) p->Progress0 = value; else p->Progress1 = value; }
    }

    // ---------------- CustomProperty.Properties: array[0..19] of TCustomProperty ----------------

    /// <summary>原文 <c>UserItem.CustomProperty.Properties[Index]</c>。</summary>
    public static TCustomProperty GetProperty(ref TUserItem item, int index) => item.CustomProperty.GetProp(index);

    /// <summary>原文 <c>UserItem.CustomProperty.Properties[Index].Xxx := ...</c>（整槽回写）。</summary>
    public static void SetProperty(ref TUserItem item, int index, TCustomProperty value)
        => item.CustomProperty.SetProp(index, value);

    // ---------------- 各定长数组的长度 ----------------

    /// <summary>原文 <c>Low(UserItem.btValue)</c>=0 / <c>High(UserItem.btValue)</c>=13。</summary>
    public const int ValueLow = 0;
    public const int ValueHigh = 13;

    /// <summary>原文 <c>Low/High(UserItem.btNewValue)</c> = 0/29。</summary>
    public const int NewValueLow = 0;
    public const int NewValueHigh = 29;

    /// <summary>原文 <c>Low/High(UserItem.btAddDataByte)</c> = 0/19（Grobal2Const.USER_ITEM_ADD_DATA_BYTE_COUNT = 20）。</summary>
    public const int AddDataByteLow = 0;
    public const int AddDataByteHigh = 19;

    /// <summary>原文 <c>Low/High(UserItem.nAddDataInt)</c> = 0/9（USER_ITEM_ADD_DATA_INT_COUNT = 10）。</summary>
    public const int AddDataIntLow = 0;
    public const int AddDataIntHigh = 9;

    /// <summary>原文 <c>Low/High(UserItem.sAddDataText)</c> = 0/1。</summary>
    public const int AddDataTextLow = 0;
    public const int AddDataTextHigh = 1;

    /// <summary>原文 <c>Low/High(UserItem.Flutes)</c> = 0/7。</summary>
    public const int FluteLow = 0;
    public const int FluteHigh = 7;

    /// <summary>原文 <c>Low/High(UserItem.Progress)</c> = 0/1。</summary>
    public const int ProgressLow = 0;
    public const int ProgressHigh = 1;

    /// <summary>原文 <c>Low/High(UserItem.CustomProperty.Properties)</c> = 0/19。</summary>
    public const int PropertyLow = 0;
    public const int PropertyHigh = 19;

    /// <summary>原文 <c>FillChar(UserItem^, SizeOf(TUserItem), 0)</c> / <c>FillChar(UserItem, SizeOf(UserItem), 0)</c>。</summary>
    public static TUserItem Zero() => default;
}

/// <summary>
/// AuctionDB <c>DoRun</c> 用到的在线玩家视图（ObjPlayer.pas <c>TPlayObject</c> 的最小面）。
/// 字段名逐字对应原文，类型按 C# 语义收窄（原文 m_n* 都是 Integer，赋 Int64 后截断）。
/// </summary>
public interface IAuctionPlayer
{
    /// <summary>ObjPlayer.pas <c>m_nGameGold</c>（元宝）。</summary>
    int m_nGameGold { get; set; }

    /// <summary>ObjPlayer.pas <c>m_nGamePoint</c>（游戏点）。</summary>
    int m_nGamePoint { get; set; }

    /// <summary>ObjPlayer.pas <c>m_nGold</c>（金币）。</summary>
    int m_nGold { get; set; }

    /// <summary>ObjPlayer.pas <c>m_nGameDiamond</c>（金刚石）。</summary>
    int m_nGameDiamond { get; set; }

    /// <summary>ObjPlayer.pas <c>m_nGameGird</c>（灵符）。</summary>
    int m_nGameGird { get; set; }

    /// <summary>ObjPlayer.pas <c>m_nScriptGotoCount</c>。</summary>
    int m_nScriptGotoCount { get; set; }

    /// <summary>ObjPlayer.pas <c>m_sAuctionItemName</c>。</summary>
    string m_sAuctionItemName { get; set; }

    /// <summary>ObjPlayer.pas <c>m_sAuctionItemHumanName</c>（物品拍卖者）。</summary>
    string m_sAuctionItemHumanName { get; set; }

    /// <summary>ObjPlayer.pas <c>m_sAuctionItemBidHumanName</c>（竞拍出价者）。</summary>
    string m_sAuctionItemBidHumanName { get; set; }

    /// <summary>ObjPlayer.pas <c>m_nAuctionItemStartPrice</c>（底价）。</summary>
    int m_nAuctionItemStartPrice { get; set; }

    /// <summary>ObjPlayer.pas <c>m_nAuctionItemSellPrice</c>（一口价）。</summary>
    int m_nAuctionItemSellPrice { get; set; }

    /// <summary>ObjPlayer.pas <c>m_nAuctionItemFinaPrice</c>（成交价）。</summary>
    int m_nAuctionItemFinaPrice { get; set; }

    /// <summary>ObjPlayer.pas <c>m_nAuctionItemInvalidPrice</c>（失效价）。</summary>
    int m_nAuctionItemInvalidPrice { get; set; }

    /// <summary>ObjPlayer.pas <c>m_nAuctionItemMoneyType</c>（货币类型）。</summary>
    int m_nAuctionItemMoneyType { get; set; }

    /// <summary>ObjPlayer.pas <c>m_boAuctionItemSelled</c>（物品是否被秒杀/出售）。</summary>
    bool m_boAuctionItemSelled { get; set; }

    /// <summary>ObjPlayer.pas <c>GameGoldChanged</c>（元宝/游戏点变动后的刷新）。</summary>
    void GameGoldChanged();

    /// <summary>ObjPlayer.pas <c>GoldChanged</c>（金币变动后的刷新）。</summary>
    void GoldChanged();

    /// <summary>ObjPlayer.pas <c>NewGamePointChanged</c>（金刚石/灵符变动后的刷新）。</summary>
    void NewGamePointChanged();
}

/// <summary>
/// <c>DoAddAuctionItem</c> 用到的 StdItem 视图（Grobal2.pas <c>TStdItem</c> 的 5 个字段）。
/// 既有 <c>GXX.Core.Protocol.TStdItem</c> 是 unsafe struct，不适合做可空引用，
/// 故按接缝惯例以最小接口表达（见 docs/转换开发文档.md §2.3）。
/// </summary>
public interface IAuctionStdItem
{
    /// <summary>Grobal2.pas <c>TStdItem.StdMode</c>。</summary>
    int StdMode { get; }

    /// <summary>Grobal2.pas <c>TStdItem.OverLap</c>。</summary>
    int OverLap { get; }

    /// <summary>Grobal2.pas <c>TStdItem.Shape</c>。</summary>
    int Shape { get; }

    /// <summary>Grobal2.pas <c>TStdItem.Color</c>。</summary>
    int Color { get; }

    /// <summary>Grobal2.pas <c>TStdItem.DBName</c>。</summary>
    string DBName { get; }
}

/// <summary>
/// AuctionDB <c>DoRun</c> 的宿主接缝（ObjPlayer / UsrEngn / DataEngn / M2Share / g_FunctionNPC）。
/// 全部是可替换委托；默认实现"无宿主"（GetPlayObject 返回 null、GetStdItem 返回 null、
/// GotoLable/AddGameDataLog 为空操作），因此默认装配下 <c>DoRun</c> 只消费语句、不产生副作用。
/// </summary>
public static class AuctionDbRunSeam
{
    /// <summary>原文 <c>UserEngine.GetPlayObject(PlayerName): TPlayObject</c>。</summary>
    public static Func<string, IAuctionPlayer?> GetPlayObject { get; set; } = _ => null;

    /// <summary>原文 <c>UserEngine.GetStdItem(DBIndex): pTStdItem</c>（DoRun 用于取物品名）。</summary>
    public static Func<int, IAuctionStdItem?> GetStdItem { get; set; } = _ => null;

    /// <summary>原文 <c>DataEngine.HumanChangeGold(nil, nil, GoldType, PlayerName, PlayerName, Prices)</c>（玩家不在线时记账）。</summary>
    public static Action<TDBChangeGoldType, string, int> HumanChangeGold { get; set; } = (_, _, _) => { };

    /// <summary>原文 <c>g_FunctionNPC.GotoLable(Player, '@AuctionSellItem'|'@AuctionBuyItem', False)</c>。</summary>
    public static Action<IAuctionPlayer, string> GotoLable { get; set; } = (_, _) => { };

    /// <summary>原文 <c>AddGameDataLog(...)</c> 的 14 个实参（按顺序原样透传，便于断言）。</summary>
    public static Action<AuctionGameDataLogArgs> AddGameDataLog { get; set; } = _ => { };

    /// <summary>原文 M2Share 全局 <c>g_boGameLogGameGold</c>。</summary>
    public static bool GBoGameLogGameGold { get; set; }

    /// <summary>原文 M2Share 全局 <c>g_boGameLogGold</c>。</summary>
    public static bool GBoGameLogGold { get; set; }

    /// <summary>原文 M2Share 常量 <c>sSTRING_GOLDNAME</c>（金币名称）。</summary>
    public static string SStringGoldName { get; set; } = "金币";

    /// <summary>恢复默认（测试用）。</summary>
    public static void ResetDefaults()
    {
        GetPlayObject = _ => null;
        GetStdItem = _ => null;
        HumanChangeGold = (_, _, _) => { };
        GotoLable = (_, _) => { };
        AddGameDataLog = _ => { };
        GBoGameLogGameGold = false;
        GBoGameLogGold = false;
        SStringGoldName = "金币";
    }
}

/// <summary><c>AddGameDataLog</c> 的 14 个实参（原文逐参对应，测试据此断言）。</summary>
public sealed class AuctionGameDataLogArgs
{
    /// <summary>LOG_GameGoldChange / LOG_GamePointChange / LOG_GoldChange / LOG_GameDiamondChange / LOG_GameGirdChange / LOG_ItemSell / LOG_ItemBuy。</summary>
    public int LogType;

    /// <summary>LOG_ActionNone。</summary>
    public int ActionType;

    /// <summary>latHuman。</summary>
    public int ActorType;

    /// <summary>'0'。</summary>
    public string Param0 = "0";

    /// <summary>0。</summary>
    public int Param1;

    /// <summary>0。</summary>
    public int Param2;

    /// <summary>物品名 / 货币名。</summary>
    public string Name = "";

    /// <summary>MakeIndex。</summary>
    public int MakeIndex;

    /// <summary>玩家名。</summary>
    public string HumanName = "";

    /// <summary>'拍卖行-到期' / '待取回, 卖出:xxx'。</summary>
    public string Remark = "";

    /// <summary>0。</summary>
    public int Param3;

    /// <summary>Prices（金钱数额）或 0。</summary>
    public int Prices;

    /// <summary>LogAdd。</summary>
    public string LogAdd = "";
}

/// <summary>
/// M2Share.pas:87-120 的 AddGameDataLog 日志类型常量 + M2Share.pas:136 的 <c>latHuman</c>。
/// 数值逐字取自原文（与 GXX.LogDataServer/LogManage.cs 的既有常量一致）。
/// </summary>
public static class AuctionLogTypes
{
    /// <summary>M2Share.pas:87 <c>LOG_ActionNone = 00</c>。</summary>
    public const int LOG_ActionNone = 0;

    /// <summary>M2Share.pas:97 <c>LOG_ItemSell = 10</c>。</summary>
    public const int LOG_ItemSell = 10;

    /// <summary>M2Share.pas:98 <c>LOG_ItemBuy = 11</c>。</summary>
    public const int LOG_ItemBuy = 11;

    /// <summary>M2Share.pas:116 <c>LOG_GoldChange = 50</c>。</summary>
    public const int LOG_GoldChange = 50;

    /// <summary>M2Share.pas:117 <c>LOG_GameGoldChange = 51</c>。</summary>
    public const int LOG_GameGoldChange = 51;

    /// <summary>M2Share.pas:118 <c>LOG_GamePointChange = 52</c>。</summary>
    public const int LOG_GamePointChange = 52;

    /// <summary>M2Share.pas:119 <c>LOG_GameDiamondChange = 53</c>。</summary>
    public const int LOG_GameDiamondChange = 53;

    /// <summary>M2Share.pas:120 <c>LOG_GameGirdChange = 54</c>。</summary>
    public const int LOG_GameGirdChange = 54;

    /// <summary>M2Share.pas:136 <c>TLogActorType = (latNone, latHuman, ...)</c> → <c>latHuman = 1</c>。</summary>
    public const int latHuman = 1;
}

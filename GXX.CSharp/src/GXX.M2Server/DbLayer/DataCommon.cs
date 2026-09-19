// 源单元：Source/M2Engine/M2DataCommon.pas（类型与常量部分：8-12 / 54 / 56-122 / 126-157 / 259-314）
//
// 本文件只搬运"六个 DB 访问单元实际用到"的公开面，字段名/顺序/语义与原文一致。
// 原文的 <c>string[ACTOR_NAME_LEN]</c>（ACTOR_NAME_LEN = 14，Grobal2.pas:29，
// 即 GBK 容量 14 字节）用 <see cref="ShortString13"/> 表达；超过容量按 GBK 字节截断
// ——与 Delphi ShortString 赋值语义相同（DBShare 车道已用同名手法，见 TShortString15）。

using System;
using System.Collections.Generic;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.DbLayer;

/// <summary>
/// M2DataCommon.pas:9-12 的 4 个 ITEM_TYPE 常量（被 Items/ItemElementAdd/... 系列表共用）。
/// </summary>
public static class DataItemTypes
{
    /// <summary>M2DataCommon.pas:9 <c>STORAGEEX_ITEM_TYPE = 0</c>。</summary>
    public const int StorageExItemType = 0;

    /// <summary>M2DataCommon.pas:10 <c>USERSHOP_ITEM_TYPE = 1</c>。</summary>
    public const int UserShopItemType = 1;

    /// <summary>M2DataCommon.pas:11 <c>GOLDDEAL_ITEM_TYPE = 2</c>。</summary>
    public const int GoldDealItemType = 2;

    /// <summary>M2DataCommon.pas:12 <c>AUCTION_ITEM_TYPE = 5</c>。</summary>
    public const int AuctionItemType = 5;
}

/// <summary>
/// Delphi <c>string[13]</c>（GBK 容量 13，即 <c>string[ACTOR_NAME_LEN]</c>）语义：
/// 超长按 GBK 字节截断。用于 <c>sBuyName / sShopName / sMasterName</c>。
/// </summary>
public readonly struct ShortString13
{
    /// <summary>GBK 字节容量。</summary>
    public const int Capacity = 13;

    private readonly string _value;

    public ShortString13(string value) => _value = Truncate(value, Capacity);

    public string Value => _value ?? "";

    /// <summary>按 GBK 字节截断到 cap 字节（Delphi ShortString 赋值语义）。</summary>
    public static string Truncate(string s, int cap)
    {
        if (string.IsNullOrEmpty(s)) return "";
        byte[] bytes = EncodingInit.GBK.GetBytes(s);
        if (bytes.Length <= cap) return s;
        return EncodingInit.GBK.GetString(bytes, 0, cap);
    }

    public static implicit operator ShortString13(string v) => new(v);
    public static implicit operator string(ShortString13 v) => v.Value;

    public static bool operator ==(ShortString13 a, ShortString13 b) => a.Value == b.Value;
    public static bool operator !=(ShortString13 a, ShortString13 b) => a.Value != b.Value;
    public override bool Equals(object? obj) => obj is ShortString13 o && o.Value == Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}

/// <summary>M2DataCommon.pas:54 <c>TShopItemType</c>（序数与原文一致，默认参数值 sitSelling）。</summary>
public enum TShopItemType
{
    sitSelling = 0,
    sitSelled = 1,
    sitStorage = 2,
    sitSellingAndStorage = 3,
}

/// <summary>M2DataCommon.pas:56-67 <c>TUserShop = packed record // size = 116</c>。</summary>
public sealed class TUserShop
{
    /// <summary>M2DataCommon.pas:57。</summary>
    public int ShopID;

    /// <summary>M2DataCommon.pas:58 <c>boBusiness: Boolean</c>（是否营业）。</summary>
    public bool boBusiness;

    /// <summary>M2DataCommon.pas:59 <c>sShopName: string[ACTOR_NAME_LEN]</c>（店铺名称）。</summary>
    public ShortString13 sShopName = new("");

    /// <summary>M2DataCommon.pas:60 <c>sMasterName: string[ACTOR_NAME_LEN]</c>（店主名称）。</summary>
    public ShortString13 sMasterName = new("");

    /// <summary>M2DataCommon.pas:61 <c>dCreateDate: TDateTime</c>。</summary>
    public DateTime dCreateDate;

    /// <summary>M2DataCommon.pas:62 <c>nCareValue: Integer</c>（关注度）。</summary>
    public int nCareValue;

    /// <summary>M2DataCommon.pas:64。</summary>
    public int SellItemCount;

    /// <summary>M2DataCommon.pas:65。</summary>
    public int SelledItemCount;

    /// <summary>M2DataCommon.pas:66。</summary>
    public int StorageItemCount;
}

/// <summary>
/// M2DataCommon.pas:126-140 <c>TUserShopList</c>。
/// Add = <c>New(Result); Result^ := UserShop^; FList.Add(Result)</c> —— **值拷贝入列**
/// （调用方之后复用同一个局部记录不会影响已入列元素，与原文一致）。
/// </summary>
public sealed class UserShopList
{
    private readonly List<TUserShop> _list = new();

    public int Count => _list.Count;

    /// <summary>M2DataCommon.pas:136 <c>property Items[Index]: pTUserShop read GetItems; default;</c>
    /// 越界返回 nil（原文 GetItems 的 (Index&gt;=0) and (Index&lt;Count) 判定）。</summary>
    public TUserShop? this[int index] => index >= 0 && index < _list.Count ? _list[index] : null;

    /// <summary>M2DataCommon.pas:654-659。</summary>
    public TUserShop Add(TUserShop userShop)
    {
        var copy = new TUserShop
        {
            ShopID = userShop.ShopID,
            boBusiness = userShop.boBusiness,
            sShopName = userShop.sShopName,
            sMasterName = userShop.sMasterName,
            dCreateDate = userShop.dCreateDate,
            nCareValue = userShop.nCareValue,
            SellItemCount = userShop.SellItemCount,
            SelledItemCount = userShop.SelledItemCount,
            StorageItemCount = userShop.StorageItemCount,
        };
        _list.Add(copy);
        return copy;
    }

    /// <summary>M2DataCommon.pas:661-670 <c>Clear</c>。</summary>
    public void Clear() => _list.Clear();

    /// <summary>M2DataCommon.pas:685-689 <c>SetCapacity</c>（仅当容量不同才设置）。</summary>
    public void SetCapacity(int value)
    {
        if (_list.Capacity != value) _list.Capacity = value;
    }

    public IReadOnlyList<TUserShop> Snapshot() => _list;
}

/// <summary>M2DataCommon.pas:71-84 <c>TUserShopItem = packed record</c>。</summary>
public sealed class TUserShopItem
{
    public int ShopID;
    public int ItemID;
    /// <summary>允许出售 (0:仓库物品; 1:出售物品; 2:出售超期不能放入仓库物品)。</summary>
    public byte btAllowSell;
    /// <summary>是否已取款。</summary>
    public bool boGetMoney;
    /// <summary>物品类型。</summary>
    public byte btItemType;
    /// <summary>货币类型。</summary>
    public byte btMoneyType;
    /// <summary>出售价格。</summary>
    public int nPrice;
    /// <summary>创建时间 TDateTime。</summary>
    public DateTime dCreateDate;
    /// <summary>M2DataCommon.pas:80 <c>UserItem: TUserItem</c>（Items 表里的物品实体）。</summary>
    public TUserItem UserItem;
    /// <summary>购买人名称（已经出售）string[ACTOR_NAME_LEN]。</summary>
    public ShortString13 sBuyName = new("");
    /// <summary>店铺名称 string[ACTOR_NAME_LEN]。</summary>
    public ShortString13 sShopName = new("");
    /// <summary>店主名称 string[ACTOR_NAME_LEN]。</summary>
    public ShortString13 sMasterName = new("");
}

/// <summary>
/// M2DataCommon.pas:142-157 <c>TUserShopItemList</c>。
/// Add / Delete 均为值拷贝 / Dispose 语义。
/// </summary>
public sealed class UserShopItemList
{
    private readonly List<TUserShopItem> _list = new();

    public int Count => _list.Count;

    public TUserShopItem? this[int index] => index >= 0 && index < _list.Count ? _list[index] : null;

    /// <summary>M2DataCommon.pas:705-710。</summary>
    public TUserShopItem Add(TUserShopItem item)
    {
        var copy = Copy(item);
        _list.Add(copy);
        return copy;
    }

    /// <summary>M2DataCommon.pas:712-721。</summary>
    public void Clear() => _list.Clear();

    /// <summary>M2DataCommon.pas:736-740。</summary>
    public void SetCapacity(int value)
    {
        if (_list.Capacity != value) _list.Capacity = value;
    }

    /// <summary>M2DataCommon.pas:742-749 <c>Delete</c>（越界静默）。</summary>
    public void Delete(int index)
    {
        if (index >= 0 && index < _list.Count) _list.RemoveAt(index);
    }

    public IReadOnlyList<TUserShopItem> Snapshot() => _list;

    private static TUserShopItem Copy(TUserShopItem s) => new()
    {
        ShopID = s.ShopID,
        ItemID = s.ItemID,
        btAllowSell = s.btAllowSell,
        boGetMoney = s.boGetMoney,
        btItemType = s.btItemType,
        btMoneyType = s.btMoneyType,
        nPrice = s.nPrice,
        dCreateDate = s.dCreateDate,
        UserItem = s.UserItem,
        sBuyName = s.sBuyName,
        sShopName = s.sShopName,
        sMasterName = s.sMasterName,
    };
}

/// <summary>
/// M2DataCommon.pas:88-114 <c>TSimpleUserShopItem = packed record</c>
/// （在 TUserShopItem 基础上追加 Items 表字段 MakeIndex/wIndex/boIsBind/btBindOption/Dura）。
/// </summary>
public sealed class TSimpleUserShopItem
{
    public int ShopID;
    public int ItemID;
    public byte btAllowSell;
    public bool boGetMoney;
    public byte btItemType;
    public byte btMoneyType;
    public int nPrice;
    public DateTime dCreateDate;
    public ShortString13 sBuyName = new("");
    public ShortString13 sShopName = new("");
    public ShortString13 sMasterName = new("");

    public int MakeIndex;
    /// <summary>物品 id（Word）。</summary>
    public ushort wIndex;
    /// <summary>是否绑定。</summary>
    public bool boIsBind;
    public byte btBindOption;
    /// <summary>持久度（Word）。</summary>
    public ushort Dura;
}

/// <summary>
/// M2DataCommon.pas:118-122 <c>TSelledAndNoGetMoneyTotal = packed record</c>。
/// </summary>
public sealed class TSelledAndNoGetMoneyTotal
{
    /// <summary>店主名称 string[ACTOR_NAME_LEN]。</summary>
    public ShortString13 sMasterName = new("");
    /// <summary>货币类型。</summary>
    public byte btMoneyType;
    /// <summary>出售价格（Int64）。</summary>
    public long nSumPrice;
}

/// <summary>
/// M2DataCommon.pas:262-278 <c>TAuctionRecord = record</c>（拍卖物品完整记录）。
/// 原文是**非 packed** <c>record</c>，含 <c>string</c>（引用计数长串）与 TUserItem。
/// </summary>
public sealed class TAuctionRecord
{
    public int AuctionID;
    /// <summary>拍卖人。</summary>
    public string HumanName = "";
    /// <summary>开始时间。</summary>
    public DateTime AddDateTime;
    /// <summary>拍卖时间。</summary>
    public int AuctionTime;
    /// <summary>剩余时间。</summary>
    public int TimeLeft;
    /// <summary>底价（LongWord）。</summary>
    public uint StartingPrice;
    /// <summary>一口价（LongWord）。</summary>
    public uint SellingPrice;
    /// <summary>货币类型。</summary>
    public int CurrencyType;
    /// <summary>最后价格。</summary>
    public int LastBidPrice;
    /// <summary>最后出价人。</summary>
    public string LastBidder = "";
    /// <summary>交易状态。</summary>
    public int TradingStatus;
    /// <summary>物品是否交接。</summary>
    public bool IsItemGive;
    /// <summary>是否被关注。</summary>
    public bool IsAttention;
    /// <summary>拍卖物品。</summary>
    public TUserItem ActionItem;
}

/// <summary>M2DataCommon.pas:280-296 <c>TAuctionInfo = record</c>（拍卖物品摘要，无 ActionItem）。</summary>
public sealed class TAuctionInfo
{
    public int AuctionID;
    public string HumanName = "";
    public DateTime AddDateTime;
    public int AuctionTime;
    public int TimeLeft;
    public uint StartingPrice;
    public uint SellingPrice;
    public int CurrencyType;
    public int LastBidPrice;
    public string LastBidder = "";
    public int TradingStatus;
    public bool IsItemGive;
}

/// <summary>
/// M2DataCommon.pas:299-314 <c>TAuctionItemList</c>：
/// Add 追加；Insert **插到 0 号位**（原文 <c>FList.Insert(0, Result)</c>）。
/// </summary>
public sealed class TAuctionItemList
{
    private readonly List<TAuctionRecord> _list = new();

    public int Count => _list.Count;

    public TAuctionRecord? this[int index] => index >= 0 && index < _list.Count ? _list[index] : null;

    /// <summary>M2DataCommon.pas:1184-1189。</summary>
    public TAuctionRecord Add(TAuctionRecord r)
    {
        var copy = Copy(r);
        _list.Add(copy);
        return copy;
    }

    /// <summary>M2DataCommon.pas:1191-1196。</summary>
    public TAuctionRecord Insert(TAuctionRecord r)
    {
        var copy = Copy(r);
        _list.Insert(0, copy);
        return copy;
    }

    /// <summary>M2DataCommon.pas:1198-1207。</summary>
    public void Clear() => _list.Clear();

    /// <summary>M2DataCommon.pas:1222-1226。</summary>
    public void SetCapacity(int value)
    {
        if (_list.Capacity != value) _list.Capacity = value;
    }

    public IReadOnlyList<TAuctionRecord> Snapshot() => _list;

    private static TAuctionRecord Copy(TAuctionRecord s) => new()
    {
        AuctionID = s.AuctionID,
        HumanName = s.HumanName,
        AddDateTime = s.AddDateTime,
        AuctionTime = s.AuctionTime,
        TimeLeft = s.TimeLeft,
        StartingPrice = s.StartingPrice,
        SellingPrice = s.SellingPrice,
        CurrencyType = s.CurrencyType,
        LastBidPrice = s.LastBidPrice,
        LastBidder = s.LastBidder,
        TradingStatus = s.TradingStatus,
        IsItemGive = s.IsItemGive,
        IsAttention = s.IsAttention,
        ActionItem = s.ActionItem,
    };
}

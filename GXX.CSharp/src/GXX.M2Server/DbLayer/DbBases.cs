// 源单元：Source/M2Engine/M2DataCommon.pas
//   TUserShopDB（159-257 声明 / 751-1168 实现）
//   TAuctionDB（317-454 声明 / 1228-1613 实现）
//   TM2DataDB （460-504 声明 / 1615-1702 实现）
//
// 原文所有 public 方法都是同一个模板：
//     FOwner.Lock;
//     try
//       Result := <初值>;
//       try
//         Result := DoXxx(...);
//       except
//         on E: Exception do MainOutMessage('[Exception] TUserShopDB:Xxx;' + E.Message);
//       end;
//     finally
//       FOwner.UnLock;
//     end;
// 本文件逐方法转写该模板（异常吞掉、Result 初值、锁边界逐字保留）。
// 原文 TM2DataDB 的临界区（InitializeCriticalSection/EnterCriticalSection）由宿主
// <see cref="IDbLayerHost"/> 提供（接缝）。

using System;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.DbLayer;

/// <summary>
/// M2DataCommon.pas:159-257 <c>TUserShopDB</c>。
/// public 面加锁 + 吞异常后转调 <c>DoXxx</c> 虚方法（由 Sqlite/MySql 两个派生类实现）。
/// </summary>
public abstract class TUserShopDB : IUserShopDb
{
    private readonly IDbLayerHost _owner;
    private uint _runTick;

    /// <summary>M2DataCommon.pas:753-757 <c>constructor TUserShopDB.Create(AOwner)</c>：FRunTick := MyGetTickCount; FOwner := AOwner。</summary>
    protected TUserShopDB(IDbLayerHost owner)
    {
        _runTick = DbLayerGlobals.GetTickCount();
        _owner = owner;
    }

    /// <summary>M2DataCommon.pas:213 <c>property Owner: TM2DataDB read FOwner;</c>。</summary>
    public IDbLayerHost Owner => _owner;

    /// <summary>M2DataCommon.pas:214 <c>property RunTick2: LongWord read FRunTick;</c>。</summary>
    public uint RunTick2 => _runTick;

    // -------------------- protected 虚方法（= 原文 DoXxx） --------------------

    /// <summary>M2DataCommon.pas:164 <c>procedure DoInit; virtual; abstract;</c></summary>
    public abstract void DoInit();

    /// <summary>M2DataCommon.pas:165 <c>procedure DoFinal; virtual; abstract;</c></summary>
    public abstract void DoFinal();

    /// <summary>M2DataCommon.pas:167-168。</summary>
    protected abstract int DoGetAllShop(int startIndex, string keyword, bool isKeywordHumanName, int sortType, UserShopList shopList);

    /// <summary>M2DataCommon.pas:169。</summary>
    protected abstract int DoGetAllShopCount(string keyword, bool isKeywordHumanName);

    /// <summary>M2DataCommon.pas:171。</summary>
    protected abstract int DoGetAllShopEx(UserShopList shopList);

    /// <summary>M2DataCommon.pas:173-175。</summary>
    protected abstract int DoGetSellItems(bool isMyShop, int startIndex, string humanName, string keyword, int itemType,
        int moneyType, int minPrice, int maxPrice, int sortType, UserShopItemList itemList, TShopItemType shopItemType);

    /// <summary>M2DataCommon.pas:176-177。</summary>
    protected abstract int DoGetSellItemsCount(bool isMyShop, string humanName, string keyword, int itemType, int moneyType,
        int minPrice, int maxPrice, TShopItemType shopItemType);

    /// <summary>M2DataCommon.pas:181-182（var UserShopItem → out）。</summary>
    protected abstract bool DoGetHumanItemWithMakeIndex(bool isMyShop, string humanName, TShopItemType shopItemType,
        int itemMakeIndex, out TSimpleUserShopItem userShopItem);

    /// <summary>M2DataCommon.pas:184。</summary>
    protected abstract int DoGetSelledAndNoGetMoneyTotal(System.Collections.Generic.List<TSelledAndNoGetMoneyTotal> itemList);

    /// <summary>M2DataCommon.pas:186（var UserShop → out）。</summary>
    protected abstract bool DoGetUserShopInfo(string humanName, out TUserShop userShop);

    /// <summary>M2DataCommon.pas:188。</summary>
    protected abstract void DoIncUserShopCareValue(string humanName);

    /// <summary>M2DataCommon.pas:190。</summary>
    protected abstract bool DoHumanNameExists(string humanName);

    /// <summary>M2DataCommon.pas:191。</summary>
    protected abstract bool DoShopNameExists(string shopName);

    /// <summary>M2DataCommon.pas:193。</summary>
    protected abstract bool DoShopAdd(string shopName, string humanName);

    /// <summary>M2DataCommon.pas:194。</summary>
    protected abstract bool DoShopDelete(int shopId);

    /// <summary>M2DataCommon.pas:195。</summary>
    protected abstract bool DoShopRename(int shopId, string newShopName);

    /// <summary>M2DataCommon.pas:197。</summary>
    protected abstract bool DoHumanRename(string oldName, string newName);

    /// <summary>M2DataCommon.pas:199。</summary>
    protected abstract bool DoUpdateHumanBusiness(string humanName, bool isBusiness);

    /// <summary>M2DataCommon.pas:201。</summary>
    protected abstract bool DoAddItem(int shopId, TUserShopItem shopItem, string sItemName);

    /// <summary>M2DataCommon.pas:202-203。</summary>
    protected abstract bool DoUpdateItem(int shopId, int itemId, int btItemType, int btAllowSell, int btMoneyType, int nPrice);

    /// <summary>M2DataCommon.pas:205。</summary>
    protected abstract bool DoBuyItem(int shopId, int itemId, string buyer);

    /// <summary>M2DataCommon.pas:206。</summary>
    protected abstract bool DoGetMoneyItem(int shopId, int itemId);

    /// <summary>M2DataCommon.pas:207。</summary>
    protected abstract bool DoDeleteItem(int shopId, int itemId);

    /// <summary>M2DataCommon.pas:209。</summary>
    protected abstract void DoRun();

    // -------------------- public 模板（加锁 + 吞异常） --------------------

    /// <summary>M2DataCommon.pas:1640 <c>FAuctionDB.DoInit; FUserShopDB.DoInit;</c>（TM2DataDB.Init 直接调 DoInit，不经包装）。</summary>
    public void Init() => DoInit();

    /// <summary>M2DataCommon.pas:1649 <c>FUserShopDB.DoFinal;</c>。</summary>
    public void Final() => DoFinal();

    /// <summary>M2DataCommon.pas:759-776 <c>GetAllShop</c>。</summary>
    public int GetAllShop(int startIndex, string keyword, bool isKeywordHumanName, int sortType, UserShopList shopList)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try
            {
                result = DoGetAllShop(startIndex, keyword, isKeywordHumanName, sortType, shopList);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:GetAllShop;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:778-794 <c>GetAllShopCount</c>。</summary>
    public int GetAllShopCount(string keyword, bool isKeywordHumanName)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try
            {
                result = DoGetAllShopCount(keyword, isKeywordHumanName);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:GetAllShopCount;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:796-812 <c>GetAllShopEx</c>。</summary>
    public int GetAllShopEx(UserShopList shopList)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try
            {
                result = DoGetAllShopEx(shopList);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:GetAllShopEx;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:814-832 <c>GetSellItems</c>。</summary>
    public int GetSellItems(bool isMyShop, int startIndex, string humanName, string keyword, int itemType, int moneyType,
        int minPrice, int maxPrice, int sortType, UserShopItemList itemList,
        TShopItemType shopItemType = TShopItemType.sitSelling)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try
            {
                result = DoGetSellItems(isMyShop, startIndex, humanName, keyword, itemType, moneyType, minPrice, maxPrice,
                    sortType, itemList, shopItemType);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:GetSellItems;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:834-851 <c>GetSellItemsCount</c>。</summary>
    public int GetSellItemsCount(bool isMyShop, string humanName, string keyword, int itemType, int moneyType,
        int minPrice, int maxPrice, TShopItemType shopItemType = TShopItemType.sitSelling)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try
            {
                result = DoGetSellItemsCount(isMyShop, humanName, keyword, itemType, moneyType, minPrice, maxPrice, shopItemType);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:GetSellItemsCount;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:869-886 <c>GetHumanItemWithMakeIndex</c>（原文异常串写的是 :GetMyItemWithMakeIndex）。</summary>
    public bool GetHumanItemWithMakeIndex(bool isMyShop, string humanName, TShopItemType shopItemType,
        int itemMakeIndex, out TSimpleUserShopItem userShopItem)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoGetHumanItemWithMakeIndex(isMyShop, humanName, shopItemType, itemMakeIndex, out userShopItem);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:GetMyItemWithMakeIndex;" + e.Message);
                userShopItem = new TSimpleUserShopItem();
                return false;
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:888-904 <c>GetSelledAndNoGetMoneyTotal</c>。</summary>
    public int GetSelledAndNoGetMoneyTotal(System.Collections.Generic.List<TSelledAndNoGetMoneyTotal> itemList)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try
            {
                result = DoGetSelledAndNoGetMoneyTotal(itemList);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:GetSelledAndNoGetMoneyTotal;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:906-922 <c>GetUserShopInfo</c>。</summary>
    public bool GetUserShopInfo(string humanName, out TUserShop userShop)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoGetUserShopInfo(humanName, out userShop);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:GetUserShopInfo;" + e.Message);
                userShop = new TUserShop();
                return false;
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:924-939 <c>IncUserShopCareValue</c>。</summary>
    public void IncUserShopCareValue(string humanName)
    {
        _owner.Lock();
        try
        {
            try
            {
                DoIncUserShopCareValue(humanName);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:IncUserShopCareValue;" + e.Message);
            }
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:941-957 <c>HumanNameExists</c>。</summary>
    public bool HumanNameExists(string humanName)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoHumanNameExists(humanName);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:HumanNameExists;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:959-975 <c>ShopNameExists</c>。</summary>
    public bool ShopNameExists(string shopName)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoShopNameExists(shopName);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:ShopNameExists;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:977-993 <c>ShopAdd</c>。</summary>
    public bool ShopAdd(string shopName, string humanName)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoShopAdd(shopName, humanName);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:ShopAdd;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:995-1011 <c>ShopDelete</c>。</summary>
    public bool ShopDelete(int shopId)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoShopDelete(shopId);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:ShopDelete;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:1013-1029 <c>ShopRename</c>。</summary>
    public bool ShopRename(int shopId, string newShopName)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoShopRename(shopId, newShopName);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:ShopRename;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:1031-1047 <c>HumanRename</c>。</summary>
    public bool HumanRename(string oldName, string newName)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoHumanRename(oldName, newName);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:HumanRename;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:1049-1065 <c>UpdateHumanBusiness</c>。</summary>
    public bool UpdateHumanBusiness(string humanName, bool isBusiness)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoUpdateHumanBusiness(humanName, isBusiness);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:UpdateHumanBusiness;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:1067-1083 <c>AddItem</c>。</summary>
    public bool AddItem(int shopId, TUserShopItem shopItem, string sItemName)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoAddItem(shopId, shopItem, sItemName);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:AddItem;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:1085-1101 <c>UpdateItem</c>。</summary>
    public bool UpdateItem(int shopId, int itemId, int btItemType, int btAllowSell, int btMoneyType, int nPrice)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoUpdateItem(shopId, itemId, btItemType, btAllowSell, btMoneyType, nPrice);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:UpdateItem;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:1103-1119 <c>GetMoneyItem</c>。</summary>
    public bool GetMoneyItem(int shopId, int itemId)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoGetMoneyItem(shopId, itemId);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:GetMoneyItem;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:1121-1137 <c>BuyItem</c>。</summary>
    public bool BuyItem(int shopId, int itemId, string buyer)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoBuyItem(shopId, itemId, buyer);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:BuyItem;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:1139-1155 <c>DeleteItem</c>。</summary>
    public bool DeleteItem(int shopId, int itemId)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoDeleteItem(shopId, itemId);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:DeleteItem;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:1157-1168 <c>Run</c>（注意：**不加锁**，与其它 public 方法不同）。</summary>
    public void Run()
    {
        try
        {
            DoRun();
            _runTick = DbLayerGlobals.GetTickCount();
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage("[Exception] TUserShopDB:Run;" + e.Message);
        }
    }
}

/// <summary>M2DataCommon.pas:317-454 <c>TAuctionDB</c>。</summary>
public abstract class TAuctionDB : IAuctionDb
{
    private readonly IDbLayerHost _owner;
    private uint _runTick;

    /// <summary>M2DataCommon.pas:1230-1234 <c>constructor TAuctionDB.Create(AOwner)</c>。</summary>
    protected TAuctionDB(IDbLayerHost owner)
    {
        _runTick = DbLayerGlobals.GetTickCount();
        _owner = owner;
    }

    /// <summary>M2DataCommon.pas:390 <c>property Owner: TM2DataDB read FOwner;</c>。</summary>
    public IDbLayerHost Owner => _owner;

    /// <summary>M2DataCommon.pas:391 <c>property RunTick2</c>。</summary>
    public uint RunTick2 => _runTick;

    // -------------------- protected 虚方法 --------------------

    public abstract void DoInit();
    public abstract void DoFinal();

    protected abstract int DoQueryAllItems(string itemName, int itemGroup, string humanName, int nPage, int topmostAuctionId,
        int itemColors, int sortField, bool sortAsc, int moneyType, uint minPrices, uint maxPrices, TAuctionItemList itemList);

    protected abstract int DoQueryMyItems(string humanName, int nPage, TAuctionItemList itemList);
    protected abstract int DoQueryMyAttentionItems(string humanName, int nPage, TAuctionItemList itemList);

    protected abstract int DoGetAllItemsPageCount(string itemName, int itemGroup, int itemColors, int moneyType,
        uint minPrices, uint maxPrices);

    protected abstract int DoGetMyItemsPageCount(string humanName);
    protected abstract int DoGetMyAttentionPageCount(string humanName);
    protected abstract int DoGetMyAuctioningItemsCount(string humanName);
    protected abstract int DoGetMySellFailItemsCount(string humanName);
    protected abstract int DoGetMyBuyOKItemsCount(string humanName);

    protected abstract int DoAddAuctionItem(string humanName, int auctionTime, int startingPrice, int sellingPrice,
        int currencyType, TUserItem userItem, object? stdItem);

    protected abstract bool DoCancelAuctionItem(string humanName, int auctionId);
    protected abstract bool DoRetrieveAuctionItem(int auctionId);
    protected abstract bool DoDeleteAuctionItem(string humanName, int auctionId);
    protected abstract bool DoAddAttentionItem(string humanName, int index);
    protected abstract bool DoDeleteAttentionItem(string humanName, int index);
    protected abstract bool DoJoinItemBid(string humanName, int index, int prices, bool isSell);

    protected abstract bool DoGetAuctionInfo(int index, out TAuctionInfo auctionInfo);
    protected abstract bool DoGetAuctionRecord(int index, out TAuctionRecord auctionRecord);
    protected abstract bool DoHumanRename(string oldName, string newName);
    protected abstract void DoRun();

    // -------------------- public 模板 --------------------

    public void Init() => DoInit();
    public void Final() => DoFinal();

    /// <summary>M2DataCommon.pas:1236-1264 <c>QueryAllItems</c>
    /// （注意 Min/Max 互换守卫：<c>if (MinPrices &lt;&gt; 0) and (MaxPrices &lt;&gt; 0) and (MinPrices &gt; MaxPrices) then</c> 交换）。</summary>
    public int QueryAllItems(string itemName, int itemGroup, string humanName, int nPage, int topmostAuctionId,
        int itemColors, int sortField, bool sortAsc, int moneyType, uint minPrices, uint maxPrices, TAuctionItemList itemList)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try
            {
                if (minPrices != 0 && maxPrices != 0 && minPrices > maxPrices)
                {
                    uint tempPrices = minPrices;
                    minPrices = maxPrices;
                    maxPrices = tempPrices;
                }

                result = DoQueryAllItems(itemName, itemGroup, humanName, nPage, topmostAuctionId, itemColors, sortField,
                    sortAsc, moneyType, minPrices, maxPrices, itemList);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:QueryAllItems;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:1266-1282 <c>QueryMyItems</c>。</summary>
    public int QueryMyItems(string humanName, int nPage, TAuctionItemList itemList)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try
            {
                result = DoQueryMyItems(humanName, nPage, itemList);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:QueryMyItems;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:1284-1300 <c>QueryMyAttentionItems</c>。</summary>
    public int QueryMyAttentionItems(string humanName, int nPage, TAuctionItemList itemList)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try
            {
                result = DoQueryMyAttentionItems(humanName, nPage, itemList);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:QueryMyAttentionItems;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    /// <summary>M2DataCommon.pas:1302-1328 <c>GetAllItemsPageCount</c>（含 Min/Max 互换守卫）。</summary>
    public int GetAllItemsPageCount(string itemName, int itemGroup, int itemColors, int moneyType, uint minPrices, uint maxPrices)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try
            {
                if (minPrices != 0 && maxPrices != 0 && minPrices > maxPrices)
                {
                    uint tempPrices = minPrices;
                    minPrices = maxPrices;
                    maxPrices = tempPrices;
                }

                result = DoGetAllItemsPageCount(itemName, itemGroup, itemColors, moneyType, minPrices, maxPrices);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:GetAllItemsPageCount;" + e.Message);
            }
            return result;
        }
        finally
        {
            _owner.UnLock();
        }
    }

    public int GetMyItemsPageCount(string humanName)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try { result = DoGetMyItemsPageCount(humanName); }
            catch (Exception e) { DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:GetMyItemsPageCount;" + e.Message); }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public int GetMyAttentionPageCount(string humanName)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try { result = DoGetMyAttentionPageCount(humanName); }
            catch (Exception e) { DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:GetMyAttentionPageCount;" + e.Message); }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public int GetMyAuctioningItemsCount(string humanName)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try { result = DoGetMyAuctioningItemsCount(humanName); }
            catch (Exception e) { DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:GetMyAuctioningItemsCount;" + e.Message); }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public int GetMySellFailItemsCount(string humanName)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try { result = DoGetMySellFailItemsCount(humanName); }
            catch (Exception e) { DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:GetMySellFailItemsCount;" + e.Message); }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public int GetMyBuyOKItemsCount(string humanName)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try { result = DoGetMyBuyOKItemsCount(humanName); }
            catch (Exception e) { DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:GetMyBuyOKItemsCount;" + e.Message); }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public int AddAuctionItem(string humanName, int auctionTime, int startingPrice, int sellingPrice, int currencyType,
        TUserItem userItem, object? stdItem)
    {
        _owner.Lock();
        try
        {
            int result = 0;
            try
            {
                result = DoAddAuctionItem(humanName, auctionTime, startingPrice, sellingPrice, currencyType, userItem, stdItem);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:AddAuctionItem;" + e.Message);
            }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public bool CancelAuctionItem(string humanName, int auctionId)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try { result = DoCancelAuctionItem(humanName, auctionId); }
            catch (Exception e) { DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:CancelAuctionItem;" + e.Message); }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public bool RetrieveAuctionItem(int auctionId)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try { result = DoRetrieveAuctionItem(auctionId); }
            catch (Exception e) { DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:RetrieveAuctionItem;" + e.Message); }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public bool DeleteAuctionItem(string humanName, int auctionId)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try { result = DoDeleteAuctionItem(humanName, auctionId); }
            catch (Exception e) { DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:DeleteAuctionItem;" + e.Message); }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public bool AddAttentionItem(string humanName, int index)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try { result = DoAddAttentionItem(humanName, index); }
            catch (Exception e) { DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:AddAttentionItem;" + e.Message); }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public bool DeleteAttentionItem(string humanName, int index)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try { result = DoDeleteAttentionItem(humanName, index); }
            catch (Exception e) { DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:DeleteAttentionItem;" + e.Message); }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public bool JoinItemBid(string humanName, int index, int prices, bool isSell)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try { result = DoJoinItemBid(humanName, index, prices, isSell); }
            catch (Exception e) { DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:JoinItemBid;" + e.Message); }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public bool GetAuctionInfo(int index, out TAuctionInfo auctionInfo)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoGetAuctionInfo(index, out auctionInfo);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:GetAuctionInfo;" + e.Message);
                auctionInfo = new TAuctionInfo();
                return false;
            }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public bool GetAuctionRecord(int index, out TAuctionRecord auctionRecord)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try
            {
                result = DoGetAuctionRecord(index, out auctionRecord);
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:GetAuctionRecord;" + e.Message);
                auctionRecord = new TAuctionRecord();
                return false;
            }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    public bool HumanRename(string oldName, string newName)
    {
        _owner.Lock();
        try
        {
            bool result = false;
            try { result = DoHumanRename(oldName, newName); }
            catch (Exception e) { DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:HumanRename;" + e.Message); }
            return result;
        }
        finally { _owner.UnLock(); }
    }

    /// <summary>M2DataCommon.pas:1602-1613 <c>Run</c>（同样**不加锁**）。</summary>
    public void Run()
    {
        try
        {
            DoRun();
            _runTick = DbLayerGlobals.GetTickCount();
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage("[Exception] TAuctionDB:Run;" + e.Message);
        }
    }
}

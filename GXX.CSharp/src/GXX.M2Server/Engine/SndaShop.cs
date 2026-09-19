using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>TShopItem.StdItem 视图子集（Name/Looks/Price；SndaShop.pas 仅使用这三个字段）。</summary>
public sealed class TShopStdItemView
{
    public string Name = "";
    public int Looks;
    public int Price;
}

/// <summary>SndaShop.pas TShopItem record 镜像。</summary>
public sealed class TShopItem
{
    public int ShopType;
    public TShopStdItemView StdItem = new();
    public int GameMoney;
    public int ImageIndex;
    public int ImageCount;
    public string Memo1 = "";
    public string Memo2 = "";
    public int ItemCount;
    public bool boBulkBuy;
    public int nBulkBuyCount;
}

/// <summary>SndaShop 数据层接缝：GetStdItem（UserEngine.GetStdItem）。</summary>
public static class SndaShopEnv
{
    public static Func<string, TShopStdItemView?> GetStdItemFn = _ => null;

    public static void Reset() => GetStdItemFn = _ => null;
}

/// <summary>
/// SndaShop.pas TSndaShopList 1:1（批次J56）：6 商店页签（0 装饰/1 补给/2 强化/3 好友/4 限量/5 奇珍）、
/// Get SameText 查找、GetEx 加货币维度、Add 全页签 CompareText 重名拒绝、Delete 引用匹配、Up/Down 移位、
/// UpdateStdItem（同名首个替换并保留原价格）、LoadFromFile/SaveToFile（ShopItemList.txt 行格式：
/// type TAB name TAB looks TAB price'|'money TAB imgIdx TAB imgCnt TAB memo1'|'memo2 TAB count TAB bulk TAB bulkCount，
/// 缺省 ImageIndex=380/ImageCount=1/ItemCount&lt;1→1/bulkCount&lt;0→1）。
/// </summary>
public class TSndaShopList
{
    private readonly List<List<TShopItem>> _itemLists = new();
    private int _recordCount;

    public TSndaShopList()
    {
        for (int i = 0; i < 6; i++)
            _itemLists.Add(new List<TShopItem>());
    }

    /// <summary>Items[Index]：Index ∈ [0..5] 否则 null。</summary>
    public List<TShopItem>? GetList(int index)
        => index is >= 0 and <= 5 ? _itemLists[index] : null;

    public int RecordCount => _recordCount;

    public void Up(TShopItem shopItem)
    {
        var list = GetList(shopItem.ShopType);
        if (list == null || list.Count <= 1)
            return;
        int nIndex = list.IndexOf(shopItem);
        if (nIndex >= 0 && nIndex - 1 >= 0 && nIndex - 1 < list.Count)
        {
            list.RemoveAt(nIndex);
            nIndex--;
            list.Insert(nIndex, shopItem);
        }
    }

    public void Down(TShopItem shopItem)
    {
        var list = GetList(shopItem.ShopType);
        if (list == null)
            return;
        int nIndex = list.IndexOf(shopItem);
        if (nIndex >= 0 && nIndex + 1 >= 0 && nIndex + 1 < list.Count)
        {
            list.RemoveAt(nIndex);
            nIndex++;
            list.Insert(nIndex, shopItem);
        }
    }

    /// <summary>UpdateStdItem：同名（CompareText）首个条目替换 StdItem 并保留原价格。</summary>
    public void UpdateStdItem(TShopStdItemView stdItem)
    {
        for (int index = 0; index <= 5; index++)
        {
            var list = _itemLists[index];
            for (int i = 0; i < list.Count; i++)
            {
                var shopItem = list[i];
                if (string.Compare(shopItem.StdItem.Name, stdItem.Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    int price = shopItem.StdItem.Price;
                    // Delphi ShopItem.StdItem := StdItem^ 为值拷贝
                    shopItem.StdItem = new TShopStdItemView { Name = stdItem.Name, Looks = stdItem.Looks, Price = price };
                    return; // break（仅跳出内层）
                }
            }
        }
    }

    public void SaveToFile()
    {
        var saveList = new List<string>();
        for (int i = 0; i <= 5; i++)
        {
            foreach (var shopItem in _itemLists[i])
            {
                string sMemo2 = shopItem.Memo2.Replace("\r\n", "|");
                saveList.Add(
                    shopItem.ShopType.ToString() + '\t' + shopItem.StdItem.Name + '\t' + shopItem.StdItem.Looks + '\t' +
                    shopItem.StdItem.Price + "|" + shopItem.GameMoney + '\t' + shopItem.ImageIndex + '\t' +
                    shopItem.ImageCount + '\t' + shopItem.Memo1 + "|" + sMemo2 + '\t' + shopItem.ItemCount + '\t' +
                    (shopItem.boBulkBuy ? 1 : 0) + '\t' + shopItem.nBulkBuyCount);
            }
        }
        try
        {
            File.WriteAllLines(Path.Combine(M2Config.sEnvirDir, "ShopItemList.txt"), saveList, Encoding.GetEncoding(936));
        }
        catch
        {
            // Delphi try-except 吞异常
        }
    }

    public void LoadFromFile()
    {
        for (int i = 0; i <= 5; i++)
            _itemLists[i].Clear();
        _recordCount = 0;
        string sFileName = Path.Combine(M2Config.sEnvirDir, "ShopItemList.txt");
        if (!File.Exists(sFileName))
            return;

        foreach (var line in ReadLinesDefault(sFileName))
        {
            string tStr = line;
            if (tStr.Length == 0 || tStr[0] == ';')
                continue;

            string sShopType = "", sItemName = "", s01 = "", sGameMoney = "", sPrice = "", sImageIndex = "",
                sImageCount = "", sMemo2 = "", sMemo1 = "", sItemCount = "", sBulkBuy = "", sBulkBuyCount = "";

            tStr = GetValidStr3(tStr, ref sShopType, ' ', '\t');
            tStr = GetValidStr3(tStr, ref sItemName, ' ', '\t');
            tStr = GetValidStr3(tStr, ref s01, ' ', '\t');
            tStr = GetValidStr3(tStr, ref sGameMoney, ' ', '\t');
            sGameMoney = GetValidStr3(sGameMoney, ref sPrice, '|', '\t');
            tStr = GetValidStr3(tStr, ref sImageIndex, ' ', '\t');
            tStr = GetValidStr3(tStr, ref sImageCount, ' ', '\t');
            // 修复商铺物品描述不支持空格 chongchong 2014-01-14（仅 TAB 分隔）
            tStr = GetValidStr3(tStr, ref sMemo2, '\t');
            sMemo2 = GetValidStr3(sMemo2, ref sMemo1, '|', '\t');
            tStr = GetValidStr3(tStr, ref sItemCount, ' ', '\t');
            tStr = GetValidStr3(tStr, ref sBulkBuy, ' ', '\t');
            sBulkBuyCount = tStr;

            if (sMemo2.Length > 0)
                sMemo2 = sMemo2.Replace("|", "\r\n");

            int nShopType = StrToIntDef(sShopType, -1);
            int nPrice = StrToIntDef(sPrice, -1);
            int nGameMoney = StrToIntDef(sGameMoney, 0);
            int nImageIndex = StrToIntDef(sImageIndex, -1);
            int nImageCount = StrToIntDef(sImageCount, -1);
            int nItemCount = StrToIntDef(sItemCount, 1);
            if (nItemCount < 1)
                nItemCount = 1;
            if (nImageCount == 0)
                nImageCount = 1;
            if (nImageIndex == 0)
                nImageIndex = 380;

            if (nShopType is >= 0 and <= 5 && nPrice >= 0 && nGameMoney is >= 0 and <= 4 &&
                sItemName.Length > 0 && sMemo1.Length > 0)
            {
                var stdItem = SndaShopEnv.GetStdItemFn(sItemName);
                if (stdItem != null)
                {
                    _recordCount++;
                    var shopItem = new TShopItem
                    {
                        ShopType = nShopType,
                        // Delphi ShopItem.StdItem := StdItem^ 为值拷贝
                        StdItem = new TShopStdItemView { Name = stdItem.Name, Looks = stdItem.Looks, Price = nPrice },
                        GameMoney = nGameMoney,
                        ImageIndex = nImageIndex,
                        ImageCount = nImageCount,
                        Memo1 = sMemo1,
                        Memo2 = sMemo2,
                        ItemCount = nItemCount,
                        boBulkBuy = StrToIntDef(sBulkBuy, 0) != 0,
                        nBulkBuyCount = StrToIntDef(sBulkBuyCount, 99),
                    };
                    if (shopItem.nBulkBuyCount < 0)
                        shopItem.nBulkBuyCount = 1;
                    _itemLists[nShopType].Add(shopItem);
                }
            }
        }
    }

    /// <summary>Add：ShopType ∈ [0..5] 且全部页签 CompareText 无重名 → 加入对应页签。</summary>
    public bool Add(TShopItem shopItem)
    {
        if (shopItem.ShopType is < 0 or > 5)
            return false;
        foreach (var list in _itemLists)
        {
            foreach (var existing in list)
            {
                if (string.Compare(shopItem.StdItem.Name, existing.StdItem.Name, StringComparison.OrdinalIgnoreCase) == 0)
                    return false;
            }
        }
        _itemLists[shopItem.ShopType].Add(shopItem);
        _recordCount++;
        return true;
    }

    /// <summary>Delete：按引用匹配移除。</summary>
    public bool Delete(TShopItem shopItem)
    {
        foreach (var list in _itemLists)
        {
            for (int ii = 0; ii < list.Count; ii++)
            {
                if (ReferenceEquals(list[ii], shopItem))
                {
                    _recordCount--;
                    list.RemoveAt(ii);
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>Get：全页签 SameText 查找。</summary>
    public TShopItem? Get(string itemName)
    {
        foreach (var list in _itemLists)
        {
            foreach (var shopItem in list)
            {
                if (string.Equals(shopItem.StdItem.Name, itemName, StringComparison.OrdinalIgnoreCase))
                    return shopItem;
            }
        }
        return null;
    }

    /// <summary>GetEx：SameText + 货币维度。</summary>
    public TShopItem? GetEx(string itemName, int moneyType)
    {
        foreach (var list in _itemLists)
        {
            foreach (var shopItem in list)
            {
                if (string.Equals(shopItem.StdItem.Name, itemName, StringComparison.OrdinalIgnoreCase) &&
                    shopItem.GameMoney == moneyType)
                    return shopItem;
            }
        }
        return null;
    }

    private static IEnumerable<string> ReadLinesDefault(string path)
    {
        foreach (var line in File.ReadLines(path, Encoding.GetEncoding(936)))
            yield return line;
    }

    internal static int StrToIntDef(string s, int def)
        => int.TryParse(s?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : def;

    /// <summary>
    /// HUtil32.GetValidStr3（Delphi 实际链接的 UNICODE 实现语义）：跳过前导分隔符（连续任意个），
    /// Dest=到下一分隔符前的内容，返回剩余；无后续分隔符时丢弃前导分隔符后整段返回。
    /// （Core 共享版仅跳空格，与此 Delphi 语义不同，故此处内置 1:1 实现。）
    /// </summary>
    internal static string GetValidStr3(string str, ref string dest, params char[] divider)
    {
        int len = str.Length;
        if (len == 0 || divider.Length == 0)
        {
            dest = str;
            return "";
        }
        bool isStart = false;
        int startIndex = 0;
        for (int i = 0; i < len; i++)
        {
            char c = str[i];
            bool isFound = Array.IndexOf(divider, c) >= 0;
            if (isFound)
            {
                if (isStart)
                {
                    dest = str.Substring(startIndex, i - startIndex);
                    return str.Substring(i + 1, len - i - 1);
                }
            }
            else if (!isStart)
            {
                isStart = true;
                startIndex = i;
            }
        }
        if (startIndex > 0)
        {
            dest = str.Substring(startIndex, len - startIndex);
            return "";
        }
        dest = str;
        return "";
    }
}

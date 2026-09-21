using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Engine;

/// <summary>Boxs.pas TBoxItem record 1:1（批次J57）：宝箱物品（物品名 + 数量）。</summary>
public sealed class TBoxItem
{
    public string ItemName = "";   // Delphi string[50]
    public int ItemCount;
}

/// <summary>Boxs.pas TBoxList 1:1：宝箱专用链表（CompareText 按名/名+数量查找与删除）。</summary>
public class TBoxList
{
    private readonly List<TBoxItem> _items = new();

    public int Count => _items.Count;

    public TBoxItem GetItems(int index) => _items[index];

    public void Add(TBoxItem item) => _items.Add(item);

    public void Clear() => _items.Clear();

    public TBoxItem? GetByName(string itemName)
    {
        foreach (var it in _items)
            if (string.Compare(it.ItemName, itemName, StringComparison.OrdinalIgnoreCase) == 0)
                return it;
        return null;
    }

    public TBoxItem? GetByName(string itemName, int itemCount)
    {
        foreach (var it in _items)
            if (string.Compare(it.ItemName, itemName, StringComparison.OrdinalIgnoreCase) == 0 && it.ItemCount == itemCount)
                return it;
        return null;
    }

    public bool DeleteByName(string itemName)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (string.Compare(_items[i].ItemName, itemName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                _items.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public bool DeleteByName(string itemName, int itemCount)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (string.Compare(_items[i].ItemName, itemName, StringComparison.OrdinalIgnoreCase) == 0 && _items[i].ItemCount == itemCount)
            {
                _items.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
}

/// <summary>Boxs.pas TBoxSet record 1:1：宝箱配置。</summary>
public struct TBoxSet
{
    public bool boNext;
    public int nGold;
    public int nGameGold;
    public int nAddGold;
    public int nAddGameGold;
    public int nEndGold;
    public int nEndGameGold;
    public byte nCount;
    public int nNowGold;
    public int nNowGameGold;
    public byte nNowCount;
}

/// <summary>Boxs.pas TBox record 1:1：宝箱箱体。</summary>
public sealed class TBox
{
    public TBoxSet BoxSet;
    public string Name = "";
    public int Source;
    public TBoxList Give = new();       // 可得
    public TBoxList Center = new();     // 中间
    public TBoxList NoGive = new();     // 不可得
    public TBoxList EndNoGive = new();  // 永远不可得
}

/// <summary>宝箱物品视图（GetBoxsItem 输出 ClientItem 子集）。</summary>
public sealed class TClientItemView
{
    public int MakeIndex;
    public string Name = "";
    public byte StdMode;
    public ushort Looks;
    public byte Color;
    public int Price;
    public ushort Dura;
    public ushort DuraMax;
}

/// <summary>宝箱用标准物品视图（UserEngine.GetStdItem 返回子集）。</summary>
public sealed class TBoxStdItemView
{
    public string Name = "";
    public byte StdMode;
    public ushort Shape;
    public ushort Looks;
    public byte Color;
    public int Price;
    public ushort DuraMax;
}

/// <summary>
/// Boxs.pas TBoxsList 1:1（批次J57）：宝箱表。
/// LoadFromFile：BoxsList.txt 逐行编号 → &lt;BoxsDir&gt;&lt;n&gt;.txt（首行为宝箱配置 TAB 8 段，
/// 其余为 '物品名 [/] 宝物类型 [/] 数量'，经验/声望/金刚石支持 '名(数量)' 形态）；
/// SaveToFile：逐箱回写同格式（MakeNewName 三类特殊物品写 '名(数量)'）。
/// </summary>
public class TBoxsList
{
    private readonly List<TBox> _boxList = new();

    public int Count => _boxList.Count;
    public TBox GetBox(int idx) => _boxList[idx];

    /// <summary>MainOutMessage 接缝（宝箱物品不存在时的日志）。</summary>
    public Action<string>? MainOutMessageHandler;

    /// <summary>UserEngine.GetStdItem(sName) 接缝：命中返回非 null。</summary>
    public Func<string, TBoxStdItemView?>? GetStdItemHandler;

    /// <summary>CheckOverLapItem 接缝（可叠加物品判定）。</summary>
    public Func<TClientItemView, bool>? CheckOverLapItemHandler;

    public TBox? Find(string itemName)
    {
        foreach (var box in _boxList)
            if (string.Compare(box.Name, itemName, StringComparison.OrdinalIgnoreCase) == 0)
                return box;
        return null;
    }

    public TBox? Find(int source)
    {
        foreach (var box in _boxList)
            if (box.Source == source)
                return box;
        return null;
    }

    /// <summary>清空宝箱表（窗体 RefBoxList / 测试隔离用）。</summary>
    public void Reset() => _boxList.Clear();

    public void LoadFromFile()
    {
        _boxList.Clear();

        if (!Directory.Exists(M2Config.sBoxsDir))
            Directory.CreateDirectory(M2Config.sBoxsDir);

        if (!File.Exists(M2Config.sBoxsFile))
        {
            var tempList = new List<string>
            {
                ";宝箱设置:StdMode=31 Shape=15--19(15=檀木宝箱,16=紫铜宝箱,17=白银宝箱,18=赤金宝箱,19=黄金宝箱 20-24=扩展的5个宝箱)",
                ";钥匙设置:StdMode=40 Shape=15--24",
                ";Source值对应宝箱开启对应的X.txt 例：赤金宝箱Source值为4，开启后对应4.txt的物品",
            };
            try
            {
                // GBK 一律经 GXX.Core.EncodingInit.GBK 获取：其内部先 Ensure() 注册 CodePagesEncodingProvider，消除加载顺序依赖（CP936 实例等价）。
                File.WriteAllLines(M2Config.sBoxsFile, tempList, GXX.Core.EncodingInit.GBK);
            }
            catch
            {
                // CreateDir/SaveToFile 异常忽略
            }
        }

        string sFileName = M2Config.sBoxsFile;
        if (!File.Exists(sFileName))
            return;

        int idx = 0;
        foreach (var line in File.ReadLines(sFileName, GXX.Core.EncodingInit.GBK))
        {
            int nStr = Str_ToInt(line, -1);
            if (nStr > -1)
            {
                if (idx == nStr)
                {
                    string sBoxFileName = M2Config.sBoxsDir + nStr.ToString(CultureInfo.InvariantCulture) + ".txt";
                    if (!LoadBoxsItemList(sBoxFileName, nStr))
                        return;
                    idx++;
                }
                else
                {
                    return;
                }
            }
        }
    }

    /// <summary>LoadBoxsItemList（377-492）：单箱文件装载，成功即加入 BoxList 并返回 true。</summary>
    public bool LoadBoxsItemList(string sFileName, int idx)
    {
        if (!File.Exists(sFileName))
            return false;

        var box = new TBox { Name = "", Source = idx };
        box.BoxSet.nNowGold = 0;
        box.BoxSet.nNowGameGold = 0;
        box.BoxSet.nNowCount = 0;

        var tempList = File.ReadAllLines(sFileName, GXX.Core.EncodingInit.GBK);
        if (tempList.Length > 0)
        {
            string sMsg = tempList[0];
            string sName = "";
            sMsg = TSndaShopList.GetValidStr3(sMsg, ref sName, ' ', '/', '\t');
            box.BoxSet.boNext = sName == "1";
            sMsg = TSndaShopList.GetValidStr3(sMsg, ref sName, ' ', '/', '\t');
            box.BoxSet.nGold = Str_ToInt(sName, -1);
            sMsg = TSndaShopList.GetValidStr3(sMsg, ref sName, ' ', '/', '\t');
            box.BoxSet.nGameGold = Str_ToInt(sName, -1);
            sMsg = TSndaShopList.GetValidStr3(sMsg, ref sName, ' ', '/', '\t');
            box.BoxSet.nAddGold = Str_ToInt(sName, -1);
            sMsg = TSndaShopList.GetValidStr3(sMsg, ref sName, ' ', '/', '\t');
            box.BoxSet.nAddGameGold = Str_ToInt(sName, -1);
            sMsg = TSndaShopList.GetValidStr3(sMsg, ref sName, ' ', '/', '\t');
            box.BoxSet.nEndGold = Str_ToInt(sName, -1);
            sMsg = TSndaShopList.GetValidStr3(sMsg, ref sName, ' ', '/', '\t');
            box.BoxSet.nEndGameGold = Str_ToInt(sName, -1);
            sMsg = TSndaShopList.GetValidStr3(sMsg, ref sName, ' ', '/', '\t');
            box.BoxSet.nCount = (byte)Str_ToInt(sName, -1);

            for (int i = 1; i < tempList.Length; i++)
            {
                sMsg = tempList[i];
                if (sMsg.Length == 0 || sMsg[0] == ';')
                    continue;

                sName = "";
                string sTemp = "";
                sMsg = TSndaShopList.GetValidStr3(sMsg, ref sName, ' ', '/', '\t');

                // 行格式：'物品名<TAB>宝物类型<TAB>数量'。
                // Delphi 原文：sTemp := GetValidStr3(sMsg, sMsg, [' ', '/', #9]); 把返回值又写回 sMsg，
                // 于是 sMsg 变成第三段（宝物类型，供 case 分支使用），sTemp 拿到第二段（数量）。
                string sRest = sMsg;                                        // 第二段 + 第三段
                string sDummy = "";
                string sTypeKey = "";                                       // 宝物类型（第二段）
                sMsg = TSndaShopList.GetValidStr3(sRest, ref sTypeKey, ' ', '/', '\t');
                sMsg = TSndaShopList.GetValidStr3(sMsg, ref sTemp, ' ', '/', '\t');    // 数量（第三段）

                int nCount;
                if (GXX.Core.Util.HUtil32.CompareLStr(sName, "经验" + "(", "经验".Length + 1))
                {
                    string sCount = "";
                    GXX.Core.Util.HUtil32.ArrestStringEx(sName, '(', ')', ref sCount);
                    nCount = Str_ToInt(sCount, 0);
                    sName = "经验";
                }
                else if (GXX.Core.Util.HUtil32.CompareLStr(sName, M2Config.sCreditPointName + "(", M2Config.sCreditPointName.Length + 1))
                {
                    string sCount = "";
                    GXX.Core.Util.HUtil32.ArrestStringEx(sName, '(', ')', ref sCount);
                    nCount = Str_ToInt(sCount, 0);
                    sName = M2Config.sCreditPointName;
                }
                else if (GXX.Core.Util.HUtil32.CompareLStr(sName, M2Config.sGameDiamondName + "(", M2Config.sGameDiamondName.Length + 1))
                {
                    string sCount = "";
                    GXX.Core.Util.HUtil32.ArrestStringEx(sName, '(', ')', ref sCount);
                    nCount = Str_ToInt(sCount, 0);
                    sName = M2Config.sGameDiamondName;
                }
                else
                {
                    if (GetStdItemHandler?.Invoke(sName) == null)
                    {
                        MainOutMessageHandler?.Invoke($"宝箱[{idx}] 物品[{sName}] 不存在.");
                        continue;
                    }
                    nCount = Str_ToInt(sTemp, 1);
                }

                int nIdx = Str_ToInt(sTypeKey, 0);
                var boxItem = new TBoxItem { ItemName = sName, ItemCount = nCount };
                switch (nIdx)
                {
                    case 0: box.Give.Add(boxItem); break;
                    case 1: box.NoGive.Add(boxItem); break;
                    case 2: box.Center.Add(boxItem); break;
                    default: box.EndNoGive.Add(boxItem); break;
                }
            }
        }

        _boxList.Add(box);
        // Delphi: if BoxList.Add(Box) = Idx then Result := True;（TList.Add 返回新元素下标）
        return _boxList.Count - 1 == idx;
    }

    public void SaveToFile()
    {
        string MakeNewName(TBoxItem boxItem)
        {
            string sItemName = boxItem.ItemName;
            int sItemCount = boxItem.ItemCount;
            if (GXX.Core.Util.HUtil32.CompareLStr(sItemName, "经验", "经验".Length)
                || GXX.Core.Util.HUtil32.CompareLStr(sItemName, M2Config.sCreditPointName, M2Config.sCreditPointName.Length)
                || GXX.Core.Util.HUtil32.CompareLStr(sItemName, M2Config.sGameDiamondName, M2Config.sGameDiamondName.Length))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}({1})", sItemName, sItemCount);
            }
            return sItemName;
        }

        if (!string.IsNullOrEmpty(M2Config.sBoxsDir) && !Directory.Exists(M2Config.sBoxsDir))
            Directory.CreateDirectory(M2Config.sBoxsDir);

        foreach (var box in _boxList)
        {
            string sFileName = M2Config.sBoxsDir + box.Source.ToString(CultureInfo.InvariantCulture) + ".txt";
            var saveList = new List<string>
            {
                string.Format(CultureInfo.InvariantCulture, "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t",
                    box.BoxSet.boNext ? 1 : 0, box.BoxSet.nGold, box.BoxSet.nGameGold, box.BoxSet.nAddGold,
                    box.BoxSet.nAddGameGold, box.BoxSet.nEndGold, box.BoxSet.nEndGameGold, box.BoxSet.nCount),
            };

            for (int ii = 0; ii < box.Give.Count; ii++)
                saveList.Add(MakeNewName(box.Give.GetItems(ii)) + "\t0\t" + box.Give.GetItems(ii).ItemCount.ToString(CultureInfo.InvariantCulture));
            for (int ii = 0; ii < box.NoGive.Count; ii++)
                saveList.Add(MakeNewName(box.NoGive.GetItems(ii)) + "\t1\t" + box.NoGive.GetItems(ii).ItemCount.ToString(CultureInfo.InvariantCulture));
            for (int ii = 0; ii < box.Center.Count; ii++)
                saveList.Add(MakeNewName(box.Center.GetItems(ii)) + "\t2\t" + box.Center.GetItems(ii).ItemCount.ToString(CultureInfo.InvariantCulture));
            for (int ii = 0; ii < box.EndNoGive.Count; ii++)
                saveList.Add(MakeNewName(box.EndNoGive.GetItems(ii)) + "\t3\t" + box.EndNoGive.GetItems(ii).ItemCount.ToString(CultureInfo.InvariantCulture));

            try
            {
                File.WriteAllLines(sFileName, saveList, GXX.Core.EncodingInit.GBK);
            }
            catch
            {
                // Delphi try-finally SaveToFile（异常向外）；此处容错
            }
        }
    }

    /// <summary>Random 接缝（GetBoxsItem 抽物品）；返回值须落在 [0, count)。</summary>
    public Func<int, int>? RandomHandler;



    /// <summary>
    /// GetBoxsItem（578-669）1:1：按 nListIdx 取箱、按 nIdx（0 可得/1 不可得/2 中间/其余 永不可得）随机抽一件，
    /// 经验/声望/金刚石三特殊物品合成（StdMode=255，Looks 1186/1185/1187，Color=255，Price=数量），
    /// 其余取标准物品；结尾按 CheckOverLapItem 决定 Dura/DuraMax，Price 统一置为物品数量。
    /// </summary>
    public TBoxSet GetBoxsItem(int nListIdx, byte nIdx, int makeIndex, TClientItemView clientItem)
    {
        var result = new TBoxSet();
        try
        {
            // ZeroMemory(ClientItem, SizeOf(TClientItem))
            clientItem.MakeIndex = 0;
            clientItem.Name = "";
            clientItem.StdMode = 0;
            clientItem.Looks = 0;
            clientItem.Color = 0;
            clientItem.Price = 0;
            clientItem.Dura = 0;
            clientItem.DuraMax = 0;

            if (nListIdx < _boxList.Count)
            {
                var box = _boxList[nListIdx];
                result = box.BoxSet;
                TBoxList list = nIdx switch
                {
                    0 => box.Give,
                    1 => box.NoGive,
                    2 => box.Center,
                    _ => box.EndNoGive,
                };
                if (list.Count > 0)
                {
                    // Delphi Random(List.Count) 的契约：0 <= idx < Count。
                    // 托管侧 RandomHandler 接缝（测试注入）必须满足同一契约。
                    int idx = (RandomHandler ?? Random.Shared.Next)(list.Count);
                    if (idx < 0 || idx >= list.Count)
                    {
                        MainOutMessageHandler?.Invoke($"[Exception] TBoxsList:GetBoxsItem idx={idx} count={list.Count}");
                        return result;
                    }
                    var boxItem = list.GetItems(idx);
                    string sName = boxItem.ItemName;
                    clientItem.MakeIndex = makeIndex;

                    if (string.CompareOrdinal(sName, "经验") == 0)
                    {
                        clientItem.Name = sName;
                        clientItem.StdMode = 255;
                        clientItem.Looks = 1186;
                        clientItem.Color = 255;
                        clientItem.Price = boxItem.ItemCount;
                    }
                    else if (string.CompareOrdinal(sName, "声望") == 0)
                    {
                        clientItem.Name = M2Config.sCreditPointName;
                        clientItem.StdMode = 255;
                        clientItem.Looks = 1185;
                        clientItem.Color = 255;
                        clientItem.Price = boxItem.ItemCount;
                    }
                    else if (string.CompareOrdinal(sName, "金刚石") == 0)
                    {
                        clientItem.Name = M2Config.sGameDiamondName;
                        clientItem.StdMode = 255;
                        clientItem.Looks = 1187;
                        clientItem.Color = 255;
                        clientItem.Price = boxItem.ItemCount;
                    }
                    else
                    {
                        var stdItem = GetStdItemHandler?.Invoke(sName);
                        if (stdItem != null)
                        {
                            clientItem.Name = stdItem.Name;
                            clientItem.StdMode = stdItem.StdMode;
                            clientItem.Looks = stdItem.Looks;
                            clientItem.Color = stdItem.Color;
                            clientItem.Price = stdItem.Price;
                            clientItem.DuraMax = stdItem.DuraMax;
                        }
                    }

                    // 宝箱叠加物品数量错误 chongchong 2014-06-03
                    if (CheckOverLapItemHandler?.Invoke(clientItem) ?? false)
                    {
                        clientItem.Dura = 0;
                        clientItem.DuraMax = clientItem.DuraMax;
                        clientItem.Price = boxItem.ItemCount;
                    }
                    else
                    {
                        clientItem.Dura = clientItem.DuraMax;
                        clientItem.Price = boxItem.ItemCount;
                    }
                }
            }
        }
        catch
        {
            MainOutMessageHandler?.Invoke("[Exception] TBoxsList:GetBoxsItem");
        }
        return result;
    }

    /// <summary>HUtil32 Str_ToInt（十六进制 $ 前缀支持，Delphi StrToIntDef 等效）。</summary>
    internal static int Str_ToInt(string s, int def)
    {
        s = (s ?? "").Trim();
        if (s.Length == 0)
            return def;
        try
        {
            if (s[0] == '$')
                return Convert.ToInt32(s.Substring(1), 16);
            return int.Parse(s, CultureInfo.InvariantCulture);
        }
        catch
        {
            return def;
        }
    }

    public IEnumerable<TBox> All()
    {
        foreach (var b in _boxList) yield return b;
    }

    /// <summary>测试/窗体用：直接加入宝箱。</summary>
    public void AddBox(TBox box) => _boxList.Add(box);
}

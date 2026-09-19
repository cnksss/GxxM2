using GXX.Core.Util;

using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>ItemDropLog 窗体数据条目（uFrmItemDropLog.pas TDropItemLogData 1:1）。</summary>
public class TDropItemLogData
{
    public DateTime DropDate;
    public string ItemOwner = "";
    public string DropMonName = "";
    public string MapName = "";
    public int nX;
    public int nY;
}

/// <summary>寄售未取货币汇总条目（uFrmUserShopGetMoneyTotal.pas TSelledAndNoGetMoneyTotal 1:1）。</summary>
public class TSelledAndNoGetMoneyTotal
{
    public string sMasterName = "";
    public int nSumPrice;
    public byte btMoneyType; // 0..4：元宝/游戏点/金币/金刚石/灵符
}

/// <summary>用户商铺条目（uFrmUserShopView.pas TUserShop 1:1）。</summary>
public class TUserShop
{
    public int ShopID;
    public string sMasterName = "";
    public string sShopName = "";
    public DateTime dCreateDate;
}

/// <summary>
/// 批次J15：三个查看类窗体的数据源引擎层：
/// ① 掉落日志解析（LoadLog：'日期 时间 物主 怪物 地图 X Y' 制表/空格分隔，逆序扫描，地图过滤）；
/// ② 寄售未取货币汇总（按物主分组累计 5 种货币）；
/// ③ 用户商铺列表（含改名/搜索语义所需的名称检查接缝）。
/// </summary>
public static class ViewFormsData
{
    /// <summary>TFrmItemDropLog.LoadLog 1:1（逆序扫描 + 日期有效性 + 地图过滤 '*' 全部）。</summary>
    public static List<TDropItemLogData> LoadItemDropLog(string fileName, string mapName)
    {
        var result = new List<TDropItemLogData>();
        if (!File.Exists(fileName))
            return result;

        foreach (var raw in File.ReadAllLines(fileName, System.Text.Encoding.GetEncoding(936)))
        {
            var s = raw;
            var fields = new string[8];
            // GetValidStr3(S, Sn, [' ', #9]) × 7
            for (int f = 0; f < 7; f++)
            {
                int pos = -1;
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == ' ' || s[i] == '\t')
                    {
                        pos = i;
                        break;
                    }
                }
                if (pos < 0)
                {
                    fields[f] = s;
                    s = "";
                }
                else
                {
                    fields[f] = s[..pos];
                    s = s[(pos + 1)..].TrimStart(' ', '\t');
                }
            }
            fields[7] = s;

            var s1 = fields[0];
            var s2 = fields[1];
            var s3 = fields[2];
            var s4 = fields[3];
            var s5 = fields[4];
            var s6 = fields[5];
            var s7 = fields[6];

            if (DateTime.TryParse(s1 + " " + s2, out var d) && d > DateTime.MinValue
                && (mapName == "*" || string.Equals(mapName, s4, StringComparison.OrdinalIgnoreCase)))
            {
                result.Add(new TDropItemLogData
                {
                    DropDate = d,
                    ItemOwner = s3,
                    DropMonName = s4,
                    MapName = s5,
                    nX = DelphiRTL.StrToIntDef(s6, 0),
                    nY = DelphiRTL.StrToIntDef(s7, 0)
                });
            }
        }
        return result;
    }

    /// <summary>vstLogsGetText 列文本 1:1。</summary>
    public static string LogColumnText(TDropItemLogData log, int column)
    {
        return column switch
        {
            0 => log.DropDate.ToString("yyyy/MM/dd HH:mm:ss"),
            1 => log.ItemOwner,
            2 => log.DropMonName,
            3 => log.MapName,
            4 => $"{log.nX}, {log.nY}",
            _ => ""
        };
    }

    /// <summary>TFrmUserShopGetMoneyTotal.RefreshData 1:1（按物主分组累计 5 种货币 + 合计）。
    /// 返回（行数据[物主, 5 项], 合计[5]）。</summary>
    public static (List<(string Master, long[] Moneys)> Rows, long[] Sums) RefreshUserShopMoneyTotal(
        List<TSelledAndNoGetMoneyTotal> items)
    {
        var rows = new List<(string, long[])>();
        var sums = new long[5];
        var moneys = new long[5];
        var lastHuman = "";

        foreach (var moneyItem in items)
        {
            if (!string.Equals(moneyItem.sMasterName, lastHuman, StringComparison.OrdinalIgnoreCase))
            {
                if (lastHuman.Length > 0)
                {
                    if (moneys.Any(m => m != 0))
                        rows.Add((lastHuman, (long[])moneys.Clone()));
                }
                moneys = new long[5];
                lastHuman = moneyItem.sMasterName;
            }

            if (moneyItem.btMoneyType is >= 0 and <= 4)
            {
                moneys[moneyItem.btMoneyType] += moneyItem.nSumPrice;
                sums[moneyItem.btMoneyType] += moneyItem.nSumPrice;
            }
        }

        if (moneys.Any(m => m != 0))
            rows.Add((lastHuman, (long[])moneys.Clone()));

        return (rows, sums);
    }
}

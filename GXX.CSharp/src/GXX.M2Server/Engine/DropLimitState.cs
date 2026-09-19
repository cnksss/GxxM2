using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>
/// ItemDropLimit.pas 1:1（掉落限制子系统：物品按地图限速限量掉落 + 落地日志）。
/// TIntervalType/TDropItemRule/TDropLimitItem/TDropLimitManager + M2Share 全局
/// g_nKey_DropLimitExt / g_DropLimitMgr。
/// </summary>
public enum TIntervalType
{
    itDay = 0,
    itHour = 1,
    itMinute = 2,
}

public static class DropLimitConsts
{
    /// <summary>Delphi TIntervalTypeNames: array[TIntervalType] of string。</summary>
    public static readonly string[] TIntervalTypeNames = { "天", "时", "分" };
}

public class TDropItemRule
{
    public string MapName = "";
    public int ClearInterval;
    public int DropInterval;
    public TIntervalType IntervalType;
    public int LimitCount;
    public int DropedCount;
    public int AllDropedCount;
    public double LastClearDate;   // TDateTime（OA 天数）
    public double LastDropTime;    // TDateTime（OA 天数）
}

public class TDropLimitItem
{
    private readonly TDropLimitManager _owner;
    private readonly List<TDropItemRule> _rules = new();

    public TDropLimitItem(TDropLimitManager owner, string name)
    {
        _owner = owner;
        Name = name;
    }

    public string Name { get; }
    public bool IsChanged { get; set; }
    public bool IsRecordLog { get; set; }

    public int Count => _rules.Count;

    public TDropItemRule? GetItemRule(int index)
        => index >= 0 && index < _rules.Count ? _rules[index] : null;

    public void Clear() => _rules.Clear();

    /// <summary>Delphi Add：New 堆拷贝后返回存储指针（调用方后续改局部不影响存储）。</summary>
    public TDropItemRule Add(TDropItemRule itemRule)
    {
        var copy = new TDropItemRule();
        ShallowCopy(itemRule, copy);
        _rules.Add(copy);
        return copy;
    }

    public bool Remove(TDropItemRule itemRule)
    {
        var index = _rules.IndexOf(itemRule);
        if (index < 0)
            return false;
        _rules.RemoveAt(index);
        Save();
        return true;
    }

    /// <summary>Delphi TDropLimitItem.Save（g_nKey_DropLimitExt=0 门控；RecordLog 先写、规则为 10 段 #9 串按序号键）。</summary>
    public void Save()
    {
        if (DropLimitGlobals.g_nKey_DropLimitExt == 0)
            return;
        try
        {
            var ini = _owner.IniFile;
            if (ini == null)
                return;
            ini.ClearSection(Name);
            ini.WriteBool(Name, "RecordLog", IsRecordLog);
            for (int i = 0; i < _rules.Count; i++)
            {
                var r = _rules[i];
                string strValue =
                    r.MapName + "\t" +
                    r.ClearInterval + "\t" +
                    r.LimitCount + "\t" +
                    r.DropedCount + "\t" +
                    Date2MyDate(r.LastClearDate) + "\t" +
                    Hour2MyHour(r.LastClearDate) + "\t" +
                    r.DropInterval + "\t" +
                    (int)r.IntervalType + "\t" +
                    r.AllDropedCount + "\t" +
                    FloatToStr(r.LastDropTime);
                ini.WriteString(Name, i.ToString(), strValue);
            }
            ini.UpdateFile();
        }
        catch
        {
            // Delphi except end 吞异常
        }
    }

    /// <summary>Delphi TDropLimitItem.Load（逐序号键读 0..1000，空键 Break；S1 空则跳过该行；IntervalType 越界回 itDay）。</summary>
    public void Load()
    {
        if (DropLimitGlobals.g_nKey_DropLimitExt == 0)
            return;
        var ini = _owner.IniFile;
        if (ini == null)
            return;
        Clear();
        IsRecordLog = ini.ReadBool(Name, "RecordLog", false);
        for (int i = 0; i <= 1000; i++)
        {
            string s = ini.ReadString(Name, i.ToString(), "");
            if (s.Length == 0)
                break;
            var parts = SplitTabs(s, 10);
            string s1 = parts[0], s2 = parts[1], s3 = parts[2], s4 = parts[3], s5 = parts[4],
                   s6 = parts[5], s7 = parts[6], s8 = parts[7], s9 = parts[8], s10 = parts[9];
            if (s1.Length > 0)
            {
                var itemRule = new TDropItemRule
                {
                    MapName = s1,
                    ClearInterval = StrToIntDef(s2, 0),
                    LimitCount = StrToIntDef(s3, 0),
                    DropedCount = StrToIntDef(s4, 0),
                    LastClearDate = MyDate2Date(StrToIntDef(s5, 0)) + MyHour2Hour(StrToIntDef(s6, 0)),
                    DropInterval = StrToIntDef(s7, 0),
                    AllDropedCount = StrToIntDef(s9, 0),
                    LastDropTime = StrToFloatDef(s10, 0),
                };
                int nTemp = StrToIntDef(s8, 0);
                itemRule.IntervalType = nTemp >= (int)TIntervalType.itDay && nTemp <= (int)TIntervalType.itMinute
                    ? (TIntervalType)nTemp
                    : TIntervalType.itDay;
                Add(itemRule);
            }
        }
    }

    internal static void ShallowCopy(TDropItemRule src, TDropItemRule dst)
    {
        dst.MapName = src.MapName;
        dst.ClearInterval = src.ClearInterval;
        dst.DropInterval = src.DropInterval;
        dst.IntervalType = src.IntervalType;
        dst.LimitCount = src.LimitCount;
        dst.DropedCount = src.DropedCount;
        dst.AllDropedCount = src.AllDropedCount;
        dst.LastClearDate = src.LastClearDate;
        dst.LastDropTime = src.LastDropTime;
    }

    internal static string[] SplitTabs(string s, int count)
    {
        var parts = new string[count];
        for (int i = 0; i < count; i++)
            parts[i] = "";
        int idx = 0;
        foreach (var seg in s.Split('\t'))
        {
            if (idx >= count)
                break;
            parts[idx++] = seg;
        }
        return parts;
    }

    internal static int StrToIntDef(string s, int def)
        => int.TryParse(s, out int v) ? v : def;

    internal static double StrToFloatDef(string s, double def)
        => double.TryParse(s, out double v) ? v : def;

    internal static string FloatToStr(double v)
        => v.ToString("R"); // Delphi FloatToStr 最短往返表示

    // ---- Delphi 日期辅助（ItemDropLimit.pas 140-182） ----

    public static int Date2MyDate(double dt)
    {
        var d = DateTime.FromOADate(dt);
        return d.Year * 10000 + d.Month * 100 + d.Day;
    }

    public static double MyDate2Date(int dt)
    {
        double result = 0;
        if (dt > 10000000)
        {
            int y = dt / 10000;
            int m = (dt - y * 10000) / 100;
            int d = dt % 100;
            if (DateTime.TryParseExact($"{y:D4}-{m:D2}-{d:D2}", "yyyy-MM-dd", null,
                    System.Globalization.DateTimeStyles.None, out var parsed))
                result = parsed.ToOADate();
        }
        return result;
    }

    public static int Hour2MyHour(double dt)
    {
        var t = DateTime.FromOADate(dt);
        return t.Hour * 10000 + t.Minute * 100 + t.Second;
    }

    public static double MyHour2Hour(int dt)
    {
        double result = 0;
        if (dt > 0)
        {
            int hour = dt / 10000;
            int min = (dt - hour * 10000) / 100;
            int sec = dt % 100;
            result = (hour * 3600 + min * 60 + sec) / 86400.0;
        }
        return result;
    }
}

public class TDropLimitManager
{
    private readonly List<TDropLimitItem> _items = new();
    private readonly List<TDropLimitItem> _sortItems = new();

    public TDropLimitManager()
    {
        // Delphi 构造：确保掉落限制目录与日志目录存在
        try
        {
            if (!Directory.Exists(M2Config.sItemDropLimit))
                Directory.CreateDirectory(M2Config.sItemDropLimit);
        }
        catch { }
        try
        {
            if (!Directory.Exists(M2Config.sItemDropLogDir))
                Directory.CreateDirectory(M2Config.sItemDropLogDir);
        }
        catch { }
    }

    /// <summary>Delphi FIniFile（LoadConfig 创建；Remove 直接使用）。</summary>
    public TFastIniFile? IniFile { get; private set; }

    public int Count => _items.Count;

    public TDropLimitItem? GetItems(int index)
        => index >= 0 && index < _items.Count ? _items[index] : null;

    public void Clear()
    {
        _items.Clear();
        _sortItems.Clear();
    }

    /// <summary>Delphi AddItem（g_nKey_DropLimitExt=0 门控返回 nil；二分定位插入排序表，重复返回既有项）。</summary>
    public TDropLimitItem? AddItem(string itemName)
    {
        if (DropLimitGlobals.g_nKey_DropLimitExt == 0)
            return null;
        if (!Search(itemName, out int index))
        {
            var item = new TDropLimitItem(this, itemName);
            _items.Add(item);
            _sortItems.Insert(index, item);
            return item;
        }
        return _sortItems[index];
    }

    /// <summary>Delphi Search：AnsiCompareText 二分；Index=插入位置（找到时为命中位）。</summary>
    public bool Search(string itemName, out int index)
    {
        bool result = false;
        int l = 0, h = _sortItems.Count - 1;
        index = 0;
        while (l <= h)
        {
            int i = (l + h) >> 1;
            var item = _sortItems[i];
            int c = string.Compare(item.Name, itemName, StringComparison.OrdinalIgnoreCase);
            if (c < 0)
            {
                l = i + 1;
            }
            else
            {
                h = i - 1;
                if (c == 0)
                {
                    result = true;
                    l = i;
                }
            }
        }
        index = l;
        return result;
    }

    public bool Remove(TDropLimitItem item)
    {
        if (DropLimitGlobals.g_nKey_DropLimitExt == 0)
            return false;
        int index = _items.IndexOf(item);
        if (index < 0)
            return false;
        string itemName = item.Name;
        _items.RemoveAt(index);
        _sortItems.Remove(item);
        IniFile?.ClearSection(itemName);
        IniFile?.EraseSection(itemName);
        IniFile?.UpdateFile();
        return true;
    }

    /// <summary>Delphi LoadConfig（DropLimitConfig.ini 节名=物品名，逐节 AddItem+Load）。</summary>
    public void LoadConfig()
    {
        if (DropLimitGlobals.g_nKey_DropLimitExt == 0)
            return;
        IniFile = new TFastIniFile(M2Config.sItemDropLimit + "DropLimitConfig.ini");
        Clear();
        var sections = new List<string>();
        IniFile.ReadSections(sections);
        foreach (var section in sections)
        {
            var limitItem = AddItem(section);
            limitItem?.Load();
        }
    }

    /// <summary>
    /// Delphi CanDropItem（Envir.m_boMirror/m_boFB 的 sMainMapName 解析留在地图层，此处接收已解析地图名）：
    /// 命中地图（SameText 或 '*'）任一规则 LimitCount&lt;=DropedCount → false。
    /// </summary>
    public bool CanDropItem(string sMapName, string itemName)
    {
        if (!Search(itemName, out int index))
            return true;
        var item = _sortItems[index];
        for (int i = 0; i < item.Count; i++)
        {
            var rule = item.GetItemRule(i)!;
            if (string.Equals(rule.MapName, sMapName, StringComparison.OrdinalIgnoreCase) || rule.MapName == "*")
            {
                if (rule.LimitCount <= rule.DropedCount)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Delphi DropItem：先限次/间隔两轮检查，再统一计数并落盘；IsRecordLog 时追加落地日志
    /// （时间 \t 物品创建名 \t 掉落怪名 \t 地图名[后缀] \t X \t Y）。logMapSuffix 对应 Mirror→'(Mirror)'/FB→'(FB)'。
    /// </summary>
    public bool DropItem(string sMapName, string itemName, string dropMonName, string itemCreateName,
        int dropPointX, int dropPointY, string logMapSuffix = "")
    {
        if (DropLimitGlobals.g_nKey_DropLimitExt == 0)
            return true;
        if (!Search(itemName, out int index))
            return true;
        var item = _sortItems[index];
        if (item == null)
            return true;

        bool isChange = false;
        for (int i = 0; i < item.Count; i++)
        {
            var rule = item.GetItemRule(i);
            if (rule == null)
                continue;
            if (string.Equals(rule.MapName, sMapName, StringComparison.OrdinalIgnoreCase) || rule.MapName == "*")
            {
                if (rule.LimitCount <= rule.DropedCount)
                    return false;
                if (rule.DropInterval > 0 && MinutesBetween(DropLimitGlobals.NowFn(), rule.LastDropTime) < rule.DropInterval)
                    return false;
            }
        }

        double now = DropLimitGlobals.NowFn().ToOADate();
        for (int i = 0; i < item.Count; i++)
        {
            var rule = item.GetItemRule(i);
            if (rule == null)
                continue;
            if (string.Equals(rule.MapName, sMapName, StringComparison.OrdinalIgnoreCase) || rule.MapName == "*")
            {
                rule.LastDropTime = now;
                rule.DropedCount++;
                rule.AllDropedCount++;
                isChange = true;
            }
        }

        if (isChange)
            item.Save();

        if (item.IsRecordLog && item.Count > 0)
        {
            try
            {
                string logFileName = M2Config.sItemDropLogDir + item.Name + ".txt";
                string sMapNameLog = sMapName + logMapSuffix;
                string line = DropLimitGlobals.NowFn().ToString("yyyy/MM/dd HH:mm:ss")
                    + "\t" + itemCreateName + "\t" + dropMonName + "\t" + sMapNameLog
                    + "\t" + dropPointX + "\t" + dropPointY;
                File.AppendAllText(logFileName, line + "\r\n", EncodingInit.GBK);
            }
            catch
            {
                // Delphi except end 吞异常
            }
        }

        return true;
    }

    /// <summary>Delphi DateUtils.MinutesBetween（整分钟数）。</summary>
    internal static int MinutesBetween(DateTime now, double lastDropTime)
        => (int)(Math.Abs(now.ToOADate() - lastDropTime) * 1440.0);
}

/// <summary>M2Share 层掉落限制全局（g_nKey_DropLimitExt: Integer = 1 + g_DropLimitMgr）。</summary>
public static class DropLimitGlobals
{
    public static int g_nKey_DropLimitExt = 1;
    public static readonly TDropLimitManager g_DropLimitMgr = new();

    /// <summary>Now 注入接缝（DropItem 限间隔/日志时间戳测试确定性）。</summary>
    public static Func<DateTime> NowFn = () => DateTime.Now;

    public static void ResetForTests()
    {
        g_nKey_DropLimitExt = 1;
        g_DropLimitMgr.Clear();
        NowFn = () => DateTime.Now;
    }
}

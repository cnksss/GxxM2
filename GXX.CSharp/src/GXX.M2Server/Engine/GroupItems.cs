using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Engine;

/// <summary>
/// GroupItems.pas TGroupItem record 1:1（批次J57）：套装组条目。
/// FLD_FLAG/FLD_RATE/FLD_VALUE 各 40 项；AttackSkillPercent/DefenseSkillPercent 为 [1..114]。
/// </summary>
public sealed class TGroupItemModel
{
    public int FLD_INDEX;
    public int FLD_COUNT;
    public string FLD_DESC = "";
    public readonly List<string> FLD_ITEMNAMES = new();
    public readonly bool[] FLD_FLAG = new bool[40];
    public readonly int[] FLD_RATE = new int[40];
    public readonly int[] FLD_VALUE = new int[40];

    /// <summary>
    /// 技能威力攻击百分比 array[1..DEF_MAGIC_COUNT+CUSTOM_MAGIC_COUNT] = [1..550]（下标 0 恒 0）。
    /// 窗体侧 GroupItemSkillPowerConfig.pas 只显示 1..114 行。
    /// </summary>
    public readonly byte[] AttackSkillPercent = new byte[TGroupItems.SKILL_POWER_MAGIC_COUNT + 1];

    /// <summary>技能威力防御百分比 array[1..550]（下标 0 恒 0）。</summary>
    public readonly byte[] DefenseSkillPercent = new byte[TGroupItems.SKILL_POWER_MAGIC_COUNT + 1];

    public string FLD_HINTMSG = "";
}

/// <summary>
/// GroupItems.pas TGroupItems 1:1（批次J57）：套装组表。
/// LoadFromFile：GroupItemList.txt（TAB 分隔 8 段：索引/数量/说明/物品名(|)/标志(|)/比率(|)/数值(|)/提示串）
/// + GroupItemSkillPowerList.txt（INI，节名=套装编号，键 Attack&lt;n&gt;/Defense&lt;n&gt;，非技能威力物品跳过）。
/// SaveToFile：同名两文件反向落盘（技能威力仅写非 0 项）。Get 为佩戴命中判定（四类容器四找函数 + 提前 Break）。
/// </summary>
public class TGroupItems
{
    /// <summary>High(THumanFengHaoItems) = 59（array[0..59]）——封号槽遍历上界（Delphi 语义：达到即退出）。</summary>
    public const int FENGHAO_ITEM_COUNT_HIGH = 59;

    /// <summary>
    /// TSkillPowerItem 的技能槽上界 = DEF_MAGIC_COUNT + CUSTOM_MAGIC_COUNT = 250 + 300 = 550
    /// （M2Share.pas 807 / Grobal2.pas 58-60）。
    /// </summary>
    public const int SKILL_POWER_MAGIC_COUNT = 550;

    private readonly List<TGroupItemModel> _list = new();
    private int _recordCount;

    public int Count => _list.Count;
    public int RecordCount => _recordCount;
    public TGroupItemModel GetItems(int index) => _list[index];

    /// <summary>IsValidMagicInSkillPowerItem（M2Share.pas 16491）：无效技能集合判定。</summary>
    public static bool IsValidMagicInSkillPowerItem(int magicId)
        => !(magicId == 2 || magicId == 4 || magicId == 8
             || (magicId >= 14 && magicId <= 21)
             || magicId == 29 || magicId == 28 || magicId == 30 || magicId == 31 || magicId == 32
             || magicId == 34 || magicId == 38 || magicId == 41 || magicId == 48 || magicId == 49
             || magicId == 50 || magicId == 55 || magicId == 68 || magicId == 67
             || (magicId >= 70 && magicId <= 80));

    /// <summary>窗体侧 115 行版本的有效性判定（含 3，用于 GroupItemSkillPowerConfig 1:1）。</summary>
    public static bool IsValidMagicInSkillPowerItemForm(int magicId)
        => !(magicId == 2 || magicId == 3 || magicId == 4 || magicId == 8
             || (magicId >= 14 && magicId <= 21)
             || magicId == 29 || magicId == 28 || magicId == 30 || magicId == 31 || magicId == 32
             || magicId == 34 || magicId == 38 || magicId == 41 || magicId == 48 || magicId == 49
             || magicId == 50 || magicId == 55 || magicId == 68 || magicId == 67
             || (magicId >= 70 && magicId <= 80));

    public bool FindIndex(int index)
    {
        foreach (var g in _list)
            if (g.FLD_INDEX == index) return true;
        return false;
    }

    public bool Find(TGroupItemModel groupItem) => _list.Contains(groupItem);

    public bool Add(TGroupItemModel item)
    {
        _list.Add(item);
        _recordCount++;
        return true;
    }

    public bool Delete(TGroupItemModel item)
    {
        int i = _list.IndexOf(item);
        if (i < 0) return false;
        _list.RemoveAt(i);
        _recordCount--;
        return true;
    }

    public void Clear()
    {
        _list.Clear();
        _recordCount = 0;
    }

    /// <summary>RateValue：Rate &gt; 0 时 Value + Round(Value * Rate / 100)，Int64 计算后钳制 [0, High(Integer)]。</summary>
    public static int RateValue(int rate, int value)
    {
        if (rate > 0)
        {
            long int64Value = value + (long)Math.Round(value * (rate / 100.0), MidpointRounding.ToEven);
            return (int)Math.Max(Math.Min(int64Value, int.MaxValue), 0);
        }
        return value;
    }

    /// <summary>RateValue2：Rate &gt; 0 时 Max(0, Value + Round(Value * Rate / 100))（Int64 结果）。</summary>
    public static long RateValue2(int rate, uint value)
    {
        if (rate > 0)
            return Math.Max(0, value + (long)Math.Round(value * (rate / 100.0), MidpointRounding.ToEven));
        return value;
    }

    public void LoadFromFile()
    {
        _recordCount = 0;
        _list.Clear();

        string sFileName = M2Config.sEnvirDir + "GroupItemList.txt";
        if (File.Exists(sFileName))
        {
            // GBK 一律经 GXX.Core.EncodingInit.GBK 获取：其内部先 Ensure() 注册 CodePagesEncodingProvider，消除加载顺序依赖（CP936 实例等价）。
            foreach (var raw in File.ReadLines(sFileName, GXX.Core.EncodingInit.GBK))
            {
                string sLineText = raw;
                if (sLineText.Length == 0 || sLineText[0] == ';')
                    continue;
                string sIndex = "", sCount = "", sItemDesc = "", sItemNames = "";
                string sBooleans = "", sRateValues = "", sValues = "", sHintMsg = "";
                sLineText = TSndaShopList.GetValidStr3(sLineText, ref sIndex, ' ', '\t');
                sLineText = TSndaShopList.GetValidStr3(sLineText, ref sCount, ' ', '\t');
                sLineText = TSndaShopList.GetValidStr3(sLineText, ref sItemDesc, ' ', '\t');
                sLineText = TSndaShopList.GetValidStr3(sLineText, ref sItemNames, ' ', '\t');
                int nIndex = TSndaShopList.StrToIntDef(sIndex, -1);
                int nCount = TSndaShopList.StrToIntDef(sCount, -1);
                if (sItemDesc.Length > 0 && sItemNames.Length > 0 && nIndex >= 0 && nCount > 0)
                {
                    var tempList = ExtractStrings('|', sItemNames.Trim());
                    TrimStringList(tempList);

                    var groupItem = new TGroupItemModel
                    {
                        FLD_INDEX = nIndex,
                        FLD_COUNT = nCount,
                        FLD_DESC = sItemDesc,
                    };
                    groupItem.FLD_ITEMNAMES.AddRange(tempList);

                    sLineText = TSndaShopList.GetValidStr3(sLineText, ref sBooleans, ' ', '\t');
                    sLineText = TSndaShopList.GetValidStr3(sLineText, ref sRateValues, ' ', '\t');
                    sLineText = TSndaShopList.GetValidStr3(sLineText, ref sValues, ' ', '\t');
                    sLineText = TSndaShopList.GetValidStr3(sLineText, ref sHintMsg, ' ', '\t');
                    groupItem.FLD_HINTMSG = sHintMsg;

                    var flags = ExtractStrings('|', sBooleans.Trim());
                    int nC = Math.Min(40, flags.Count);
                    for (int ii = 0; ii < nC; ii++)
                        groupItem.FLD_FLAG[ii] = flags[ii] == "1";

                    var rates = ExtractStrings('|', sRateValues.Trim());
                    nC = Math.Min(40, rates.Count);
                    for (int ii = 0; ii < nC; ii++)
                        groupItem.FLD_RATE[ii] = TSndaShopList.StrToIntDef(rates[ii], 0);

                    var values = ExtractStrings('|', sValues.Trim());
                    nC = Math.Min(40, values.Count);
                    for (int ii = 0; ii < nC; ii++)
                        groupItem.FLD_VALUE[ii] = TSndaShopList.StrToIntDef(values[ii], 0);

                    _list.Add(groupItem);
                    _recordCount++;
                }
            }
        }

        sFileName = M2Config.sEnvirDir + "GroupItemSkillPowerList.txt";
        if (File.Exists(sFileName))
        {
            var ini = ReadIniAll(sFileName);
            foreach (var groupItem in _list)
            {
                for (int ii = 1; ii <= SKILL_POWER_MAGIC_COUNT; ii++)
                {
                    if (!IsValidMagicInSkillPowerItem(ii))
                        continue;
                    groupItem.AttackSkillPercent[ii] = (byte)ReadIniInt(ini, groupItem.FLD_INDEX.ToString(CultureInfo.InvariantCulture), "Attack" + ii, 0);
                    groupItem.DefenseSkillPercent[ii] = (byte)ReadIniInt(ini, groupItem.FLD_INDEX.ToString(CultureInfo.InvariantCulture), "Defense" + ii, 0);
                }
            }
        }
    }

    public void SaveToFile()
    {
        string sFileName = M2Config.sEnvirDir + "GroupItemList.txt";
        var saveList = new List<string>();
        foreach (var groupItem in _list)
        {
            string sItemName = "";
            for (int ii = 0; ii < groupItem.FLD_ITEMNAMES.Count; ii++)
                sItemName += groupItem.FLD_ITEMNAMES[ii] + "|";
            if (sItemName.Length > 0 && sItemName[sItemName.Length - 1] == '|')
                sItemName = sItemName.Substring(0, sItemName.Length - 1);

            string sFlag = "";
            for (int ii = 0; ii < 40; ii++)
                sFlag += (groupItem.FLD_FLAG[ii] ? 1 : 0) + "|";
            if (sFlag.Length > 0 && sFlag[sFlag.Length - 1] == '|')
                sFlag = sFlag.Substring(0, sFlag.Length - 1);

            string sRate = "";
            for (int ii = 0; ii < 40; ii++)
                sRate += groupItem.FLD_RATE[ii].ToString(CultureInfo.InvariantCulture) + "|";
            if (sRate.Length > 0 && sRate[sRate.Length - 1] == '|')
                sRate = sRate.Substring(0, sRate.Length - 1);

            string sValue = "";
            for (int ii = 0; ii < 40; ii++)
                sValue += groupItem.FLD_VALUE[ii].ToString(CultureInfo.InvariantCulture) + "|";
            if (sValue.Length > 0 && sValue[sValue.Length - 1] == '|')
                sValue = sValue.Substring(0, sValue.Length - 1);

            saveList.Add(groupItem.FLD_INDEX.ToString(CultureInfo.InvariantCulture) + "\t"
                + groupItem.FLD_COUNT.ToString(CultureInfo.InvariantCulture) + "\t"
                + groupItem.FLD_DESC + "\t" + sItemName + "\t" + sFlag + "\t" + sRate + "\t" + sValue + "\t"
                + groupItem.FLD_HINTMSG);
        }

        try
        {
            File.WriteAllLines(sFileName, saveList, GXX.Core.EncodingInit.GBK);
        }
        catch
        {
            // Delphi SaveList.SaveToFile(sFileName) 在 try-finally 内（异常向外）；此处容错
        }

        // GroupItemSkillPowerList.txt（原文件不删除，仅写非 0 项）
        sFileName = M2Config.sEnvirDir + "GroupItemSkillPowerList.txt";
        var lines = new List<string>();
        foreach (var groupItem in _list)
        {
            string section = groupItem.FLD_INDEX.ToString(CultureInfo.InvariantCulture);
            bool wroteSection = false;
            for (int ii = 1; ii <= SKILL_POWER_MAGIC_COUNT; ii++)
            {
                if (!IsValidMagicInSkillPowerItem(ii))
                    continue;
                if (groupItem.AttackSkillPercent[ii] != 0)
                {
                    if (!wroteSection) { lines.Add("[" + section + "]"); wroteSection = true; }
                    lines.Add("Attack" + ii.ToString(CultureInfo.InvariantCulture) + "=" + groupItem.AttackSkillPercent[ii].ToString(CultureInfo.InvariantCulture));
                }
                if (groupItem.DefenseSkillPercent[ii] != 0)
                {
                    if (!wroteSection) { lines.Add("[" + section + "]"); wroteSection = true; }
                    lines.Add("Defense" + ii.ToString(CultureInfo.InvariantCulture) + "=" + groupItem.DefenseSkillPercent[ii].ToString(CultureInfo.InvariantCulture));
                }
            }
        }
        try
        {
            File.WriteAllLines(sFileName, lines, GXX.Core.EncodingInit.GBK);
        }
        catch
        {
            // 同上
        }
    }

    /// <summary>ExtractStrings(['|'], [], ...) 等效：跳空串。</summary>
    internal static List<string> ExtractStrings(char separator, string source)
    {
        var result = new List<string>();
        if (source.Length == 0)
            return result;
        foreach (var part in source.Split(separator))
        {
            if (part.Length > 0)
                result.Add(part);
        }
        return result;
    }

    /// <summary>TrimStringList（HUtil32）：逐项 Trim，空项删除。</summary>
    internal static void TrimStringList(List<string> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            list[i] = list[i].Trim();
            if (list[i].Length == 0)
                list.RemoveAt(i);
        }
    }

    internal static Dictionary<string, Dictionary<string, string>> ReadIniAll(string path)
    {
        var ini = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        // Delphi TIniFile 在文件不存在时按"空 INI"工作（读全默认值、写时新建）；
        // 缺此守卫会让首次运行的保存路径 File.ReadLines 抛 FileNotFoundException。
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return ini;
        string section = "";
        foreach (var line in File.ReadLines(path, GXX.Core.EncodingInit.GBK))
        {
            string s = line.Trim();
            if (s.Length == 0 || s[0] == ';')
                continue;
            if (s[0] == '[' && s[s.Length - 1] == ']')
            {
                section = s.Substring(1, s.Length - 2).Trim();
                if (!ini.ContainsKey(section))
                    ini[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                continue;
            }
            int eq = s.IndexOf('=');
            if (eq <= 0)
                continue;
            if (!ini.ContainsKey(section))
                ini[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            ini[section][s.Substring(0, eq).Trim()] = s.Substring(eq + 1).Trim();
        }
        return ini;
    }

    internal static int ReadIniInt(Dictionary<string, Dictionary<string, string>> ini, string section, string key, int def)
    {
        if (ini.TryGetValue(section, out var sec) && sec.TryGetValue(key, out var val))
            return TSndaShopList.StrToIntDef(val, def);
        return def;
    }

    /// <summary>INI 字符串读取（Delphi TIniFile.ReadString 等效）。</summary>
    internal static string ReadIniString(Dictionary<string, Dictionary<string, string>> ini, string section, string key, string def)
    {
        if (ini.TryGetValue(section, out var sec) && sec.TryGetValue(key, out var val))
            return val;
        return def;
    }

    public IEnumerable<TGroupItemModel> All()
    {
        foreach (var g in _list) yield return g;
    }

    // ================= Get：套装佩戴命中判定（1:1） =================

    /// <summary>UserEngine.GetStdItem 接缝（返回 null 表示未找到）。</summary>
    public Func<ushort, TStdItemView?>? GetStdItemHandler;

    /// <summary>g_Config.boTZSupportRenameItem / boJewelryCalcGroupAbilitys 取值接缝。</summary>
    public Func<bool>? GetTZSupportRenameItem;
    public Func<bool>? GetJewelryCalcGroupAbilitys;

    /// <summary>
    /// TGroupItems.Get（GroupItems.pas 273-627）1:1：四类容器（装备/首饰盒/神佑盒/封号）
    /// 逐名匹配（CompareText）并标记已用，命中数量补足 FLD_COUNT 后加入 GroupList。
    /// UseItems 为 THumanUseItems（0..29）、JewelryBoxItems 0..5、GodBlessItems 0..11、FengHaoItems 0..58。
    /// </summary>
    public int Get(
        TUserItem[] useItems,
        TUserItem[] jewelryBoxItems,
        TUserItem[] godBlessItems,
        IReadOnlyList<TUserItem> fengHaoItems,
        int activeFengHao,
        List<TGroupItemModel> groupList)
    {
        bool boRename = GetTZSupportRenameItem?.Invoke() ?? M2Config.boTZSupportRenameItem;
        bool boJewelry = GetJewelryCalcGroupAbilitys?.Invoke() ?? M2Config.boJewelryCalcGroupAbilitys;

        var useItemArray = new bool[useItems.Length];
        var jewelryBoxItemsArray = new bool[jewelryBoxItems.Length];
        var godBlessItemsArray = new bool[godBlessItems.Length];
        var fengHaoItemsArray = new bool[FENGHAO_ITEM_COUNT_HIGH];

        bool FindUseItems(string sItemName)
        {
            for (int i = 0; i < useItems.Length; i++)
            {
                if (useItems[i].wIndex <= 0 || useItemArray[i])
                    continue;
                var stdItem = GetStdItemHandler?.Invoke(useItems[i].wIndex);
                if (stdItem == null)
                    continue;
                string sUserItemName = boRename && useItems[i].GetBtValue(13) == 1 ? useItems[i].NameStr : stdItem.Name;
                if (string.Compare(sItemName, sUserItemName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    useItemArray[i] = true;
                    return true;
                }
            }
            return false;
        }

        bool FindJewelryBoxItems(string sItemName)
        {
            for (int i = 0; i < jewelryBoxItems.Length; i++)
            {
                if (jewelryBoxItems[i].wIndex <= 0 || jewelryBoxItemsArray[i])
                    continue;
                var stdItem = GetStdItemHandler?.Invoke(jewelryBoxItems[i].wIndex);
                if (stdItem == null)
                    continue;
                string sUserItemName = boRename && jewelryBoxItems[i].GetBtValue(13) == 1 ? jewelryBoxItems[i].NameStr : stdItem.Name;
                if (string.Compare(sItemName, sUserItemName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    jewelryBoxItemsArray[i] = true;
                    return true;
                }
            }
            return false;
        }

        bool FindGodBlessItems(string sItemName)
        {
            int i = 0;
            while (i < godBlessItems.Length)
            {
                if (godBlessItems[i].wIndex <= 0 || godBlessItemsArray[i])
                {
                    i++;
                    continue;
                }
                var stdItem = GetStdItemHandler?.Invoke(godBlessItems[i].wIndex);
                if (stdItem == null)
                {
                    i++;
                    continue;
                }
                string sUserItemName = boRename && godBlessItems[i].GetBtValue(13) == 1 ? godBlessItems[i].NameStr : stdItem.Name;
                if (string.Compare(sItemName, sUserItemName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    godBlessItemsArray[i] = true;
                    return true;
                }
                i++;
            }
            return false;
        }

        bool FindFengHaoItems(string sItemName)
        {
            int i = 0;
            while (i <= fengHaoItems.Count - 1)
            {
                // Delphi: if I >= High(THumanFengHaoItems) then Exit;（High=59）
                if (i >= FENGHAO_ITEM_COUNT_HIGH)
                    return false;
                if (fengHaoItemsArray[i])
                {
                    i++;
                    continue;
                }
                var userItem = fengHaoItems[i];
                var stdItem = GetStdItemHandler?.Invoke(userItem.wIndex);
                if (stdItem == null)
                {
                    i++;
                    continue;
                }
                        if (stdItem.AniCount == 0 && activeFengHao != i)
                {
                    i++;
                    continue;
                }
                string sUserItemName = boRename && userItem.GetBtValue(13) == 1 ? userItem.NameStr : stdItem.Name;
                if (string.Compare(sItemName, sUserItemName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    fengHaoItemsArray[i] = true;
                    return true;
                }
                i++;
            }
            return false;
        }

        foreach (var groupItem in _list)
        {
            int nCount = groupItem.FLD_COUNT;

            Array.Clear(useItemArray);
            Array.Clear(jewelryBoxItemsArray);
            Array.Clear(godBlessItemsArray);
            Array.Clear(fengHaoItemsArray);

            foreach (var name in groupItem.FLD_ITEMNAMES)
            {
                if (FindUseItems(name))
                    nCount--;
                else if (FindGodBlessItems(name))
                    nCount--;
                else if (boJewelry && FindJewelryBoxItems(name))
                    nCount--;
                else if (FindFengHaoItems(name))
                    nCount--;

                if (nCount <= 0)
                {
                    groupList.Add(groupItem);
                    break;
                }
            }
        }
        return groupList.Count;
    }
}

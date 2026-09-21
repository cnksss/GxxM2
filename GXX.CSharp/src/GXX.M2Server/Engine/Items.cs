using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>物品规则标志：0 禁止丢弃 / 1 禁止交易 / 5 上线消失 / 6 死亡必爆 ...（共 41 位）。</summary>
public struct TFlagArray
{
    public byte Flags0;
    public byte Flags1;
    public byte Flags2;
    public byte Flags3;
    public byte Flags4;

    public bool this[int i]
    {
        get
        {
            if (i < 0 || i >= 41) return false;
            int word = i / 8;
            int bit = i % 8;
            byte v = word switch { 0 => Flags0, 1 => Flags1, 2 => Flags2, 3 => Flags3, _ => Flags4 };
            return (v & (1 << bit)) != 0;
        }
        set
        {
            if (i < 0 || i >= 41) return;
            int word = i / 8;
            int bit = i % 8;
            switch (word)
            {
                case 0: Flags0 = value ? (byte)(Flags0 | (1 << bit)) : (byte)(Flags0 & ~(1 << bit)); break;
                case 1: Flags1 = value ? (byte)(Flags1 | (1 << bit)) : (byte)(Flags1 & ~(1 << bit)); break;
                case 2: Flags2 = value ? (byte)(Flags2 | (1 << bit)) : (byte)(Flags2 & ~(1 << bit)); break;
                case 3: Flags3 = value ? (byte)(Flags3 | (1 << bit)) : (byte)(Flags3 & ~(1 << bit)); break;
                case 4: Flags4 = value ? (byte)(Flags4 | (1 << bit)) : (byte)(Flags4 & ~(1 << bit)); break;
            }
        }
    }
}

/// <summary>TItemRule（ItemRules.pas TItemRule record 1:1，含拍卖价格区间 PricesLime）。</summary>
public class TItemRule
{
    public int ItemIdx;
    public string ItemName = "";
    public TFlagArray FlagArray;
    public LongArray5 PricesMin;
    public LongArray5 PricesMax;

    /// <summary>PricesLime.Min[i]（Delphi TAcutionItemPricesLime 视图）。</summary>
    public uint PricesLimeMin(int i) => PricesMin[i];

    /// <summary>PricesLime.Max[i]。</summary>
    public uint PricesLimeMax(int i) => PricesMax[i];
}

/// <summary>
/// ItemRules.pas TItemRules 1:1：物品规则表（名称/索引 → 41 位标志 + 拍卖价格区间），
/// LoadFromFile/SaveToFile 与原 EDCode 编码行格式对应（名称;标志串）。
/// </summary>
public class TItemRules
{
    private readonly List<TItemRule> _list = new();

    /// <summary>Add 调用轨迹（诊断用）。</summary>
    public string DebugAddTrace = "";

    public int Count => _list.Count;

    public TItemRule GetItems(int index) => _list[index];

    /// <summary>UserEngine.GetStdItemIdx 接缝（Delphi 依赖；未注入时按列表序号回退）。</summary>
    public Func<string, int>? GetStdItemIdxHandler;

    /// <summary>按序号查找（Delphi TItemRules.Find：ItemIdx &lt; 0 → null）。</summary>
    public TItemRule? FindByIndex(int itemIdx)
    {
        if (itemIdx < 0) return null;
        foreach (var r in _list)
            if (r.ItemIdx == itemIdx) return r;
        return null;
    }

    /// <summary>Add：按名称唯一添加。</summary>
    public bool Add(string sItemName, TFlagArray flagArray)
        => Add(sItemName, flagArray, null) != null;

    /// <summary>
    /// Add（ItemRules.pas 294-314）1:1：GetStdItemIdx &lt; 0 → nil（拒绝）；
    /// 已存在同序号 → nil；否则按序号有序插入。prices 为 PricesLime（null 视作全 0）。
    /// </summary>
    public TItemRule? Add(string sItemName, TFlagArray flagArray, (uint[] Min, uint[] Max)? prices)
    {
        int itemIdx = GetStdItemIdxHandler?.Invoke(sItemName) ?? _list.Count;
        if (itemIdx < 0)
            return null;
        // 名称唯一（Delphi GetStdItemIdx 同名同号；未注入接缝时按名兜底保证 Add 幂等）
        if (Find(sItemName) != null)
            return null;
        if (FindByIndex(itemIdx) != null)
            return null;

        var rule = new TItemRule { ItemIdx = itemIdx, ItemName = sItemName, FlagArray = flagArray };
        if (prices.HasValue)
        {
            for (int i = 0; i < 5; i++)
            {
                rule.PricesMin[i] = prices.Value.Min[i];
                rule.PricesMax[i] = prices.Value.Max[i];
            }
        }

        int insertAt = 0;
        while (insertAt < _list.Count && _list[insertAt].ItemIdx < itemIdx)
            insertAt++;
        _list.Insert(insertAt, rule);
        return rule;
    }

    public bool Delete(string sItemName)
    {
        var rule = Find(sItemName);
        if (rule == null) return false;
        _list.Remove(rule);
        return true;
    }

    /// <summary>Delete（ItemRules.pas 316）：GetStdItemIdx &lt; 0 或未命中 → false。</summary>
    public bool DeleteByStdItemIdx(string sItemName)
    {
        int itemIdx = GetStdItemIdxHandler?.Invoke(sItemName) ?? -1;
        if (itemIdx < 0) return false;
        var rule = FindByIndex(itemIdx);
        if (rule == null) return false;
        _list.Remove(rule);
        return true;
    }

    public void Clear() => _list.Clear();

    public TItemRule? Find(string sItemName)
    {
        foreach (var r in _list)
            if (r.ItemName.Equals(sItemName, StringComparison.OrdinalIgnoreCase)) return r;
        return null;
    }

    /// <summary>Get：物品任一规则记录启用标志 nFlag → true。</summary>
    public bool Get(int itemIdx, int nFlag)
    {
        foreach (var r in _list)
        {
            if (r.ItemIdx == itemIdx && r.FlagArray[nFlag])
                return true;
        }
        return false;
    }

    public bool GetEx(int itemIdx, int nFlag, ref LongArray5 prices)
    {
        foreach (var r in _list)
        {
            if (r.ItemIdx == itemIdx && r.FlagArray[nFlag])
            {
                prices = new LongArray5();
                for (int i = 0; i < 5; i++)
                {
                    prices[i] = r.PricesMin[i];
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>GetEx（ItemRules.pas 354）1:1：命中即同时带出 Min/Max 两段区间。</summary>
    public bool GetExPrices(int itemIdx, int nFlag, out uint[] min, out uint[] max)
    {
        min = new uint[5];
        max = new uint[5];
        if (nFlag < 0 || nFlag > 40)
            return false;
        var rule = FindByIndex(itemIdx);
        if (rule == null)
            return false;
        bool result = rule.FlagArray[nFlag];
        for (int i = 0; i < 5; i++)
        {
            min[i] = rule.PricesMin[i];
            max[i] = rule.PricesMax[i];
        }
        return result;
    }

    /// <summary>LoadFromFile：行格式 "名称/标志0,标志1,..."（对应原 EDCode 解码行）。</summary>
    public void LoadFromFile(string fileName)
    {
        Clear();
        if (!File.Exists(fileName)) return;
        foreach (var line in File.ReadAllLines(fileName))
        {
            string s = line.Trim();
            if (s.Length == 0 || s.StartsWith(';')) continue;
            int slash = s.IndexOf('/');
            if (slash <= 0) continue;
            string name = s.Substring(0, slash).Trim();
            var flags = new TFlagArray();
            int idx = 0;
            foreach (var flag in s.Substring(slash + 1).Split(','))
            {
                if (flag.Trim() == "1") flags[idx] = true;
                idx++;
                if (idx >= 41) break;
            }
            Add(name, flags);
        }
    }

    /// <summary>LoadFromFile（ItemRules.pas 127）1:1：从 sEnvirDir\ItemRuleList.txt 读取。</summary>
    public void LoadFromEnvirFile()
        => LoadFromFile(System.IO.Path.Combine(M2Config.sEnvirDir, "ItemRuleList.txt"));

    /// <summary>
    /// SaveToFile（ItemRules.pas 208）1:1：ItemRuleList.txt 每行
    /// '物品名 TAB 41标志(空格分隔) TAB | TAB 5×(Min Max)'，尾部整体 Trim。
    /// </summary>
    public void SaveToEnvirFile()
    {
        var saveList = new List<string>();
        foreach (var r in _list)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(r.ItemName).Append('\t');
            for (int i = 0; i <= 40; i++)
                sb.Append(r.FlagArray[i] ? '1' : '0').Append(' ');
            sb.Append('|');
            for (int i = 0; i < 5; i++)
            {
                sb.Append(r.PricesMin[i].ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(' ');
                sb.Append(r.PricesMax[i].ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(' ');
            }
            saveList.Add(sb.ToString().Trim());
        }
        try
        {
            // GBK 一律经 GXX.Core.EncodingInit.GBK 获取：其内部先 Ensure() 注册 CodePagesEncodingProvider，消除加载顺序依赖（CP936 实例等价）。
            File.WriteAllLines(System.IO.Path.Combine(M2Config.sEnvirDir, "ItemRuleList.txt"), saveList, GXX.Core.EncodingInit.GBK);
        }
        catch
        {
            // Delphi try-except 吞异常
        }
    }

    public void SaveToFile(string fileName)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var r in _list)
        {
            sb.Append(r.ItemName).Append('/');
            for (int i = 0; i < 41; i++)
            {
                sb.Append(r.FlagArray[i] ? '1' : '0');
                if (i < 40) sb.Append(',');
            }
            sb.AppendLine();
        }
        File.WriteAllText(fileName, sb.ToString());
    }
}

/// <summary>
/// ItmUnit.pas 核心转换：GetRandomRange + 随机升级（武器/衣服，g_Config 概率字段入 M2Config）。
/// btValue[0]=DC [1]=MC [2]=SC（武器）/ AC MAC DC MC SC（衣服），DuraMax 上限 65000。
/// </summary>
public static unsafe class ItemUnit
{
    private static readonly Random Rnd = new();

    /// <summary>GetRandomRange(nCount, nRate)：nRate 内抽中一次计 1，循环 nCount 次。</summary>
    public static int GetRandomRange(int nCount, int nRate)
    {
        if (nRate <= 0) return 0;
        int result = 0;
        for (int i = 0; i < nCount; i++)
            if (Rnd.Next(nRate) == 0)
                result++;
        return result;
    }

    /// <summary>RandomUpgradeWeapon。</summary>
    public static void RandomUpgradeWeapon(ref TUserItem userItem)
    {
        int nC = GetRandomRange(M2Config.nWeaponDCAddValueMaxLimit, M2Config.nWeaponDCAddValueRate);
        if (M2Config.nWeaponDCAddRate > 0 && Rnd.Next(M2Config.nWeaponDCAddRate) == 0)
        {
            userItem.btValue[0] = nC + 1;
            if (userItem.btValue[0] > M2Config.nWeaponDCAddValueMaxLimit)
                userItem.btValue[0] = M2Config.nWeaponDCAddValueMaxLimit;
        }

        nC = GetRandomRange(M2Config.nWeaponHitSpeedAddValueMaxLimit, M2Config.nWeaponHitSpeedAddValueRate);
        if (M2Config.nWeaponHitSpeedAddRate > 0 && Rnd.Next(M2Config.nWeaponHitSpeedAddRate) == 0)
        {
            int n14 = (nC + 1) / 3;
            if (n14 > 0)
                userItem.btValue[6] = n14;
        }

        nC = GetRandomRange(M2Config.nWeaponMCAddValueMaxLimit, M2Config.nWeaponMCAddValueRate);
        if (M2Config.nWeaponMCAddRate > 0 && Rnd.Next(M2Config.nWeaponMCAddRate) == 0)
        {
            userItem.btValue[1] = nC + 1;
            if (userItem.btValue[1] > M2Config.nWeaponMCAddValueMaxLimit)
                userItem.btValue[1] = M2Config.nWeaponMCAddValueMaxLimit;
        }

        nC = GetRandomRange(M2Config.nWeaponSCAddValueMaxLimit, M2Config.nWeaponSCAddValueRate);
        if (M2Config.nWeaponSCAddRate > 0 && Rnd.Next(M2Config.nWeaponSCAddRate) == 0)
        {
            userItem.btValue[2] = nC + 1;
            if (userItem.btValue[2] > M2Config.nWeaponSCAddValueMaxLimit)
                userItem.btValue[2] = M2Config.nWeaponSCAddValueMaxLimit;
        }

        nC = GetRandomRange(12, 15);
        if (Rnd.Next(15) == 0)
            userItem.btValue[5] = nC / 2 + 1;

        nC = GetRandomRange(12, 12);
        if (Rnd.Next(3) < 2)
        {
            int n10 = (nC + 1) * 2000;
            userItem.DuraMax = (ushort)Math.Min(65000, userItem.DuraMax + n10);
            userItem.Dura = (ushort)Math.Min(65000, userItem.Dura + n10);
        }

        nC = GetRandomRange(12, 15);
        if (Rnd.Next(10) == 0)
            userItem.btValue[7] = nC / 2 + 1;
    }

    /// <summary>RandomUpgradeDress。</summary>
    public static void RandomUpgradeDress(ref TUserItem userItem)
    {
        int nC = GetRandomRange(M2Config.nDressACAddValueMaxLimit, M2Config.nDressACAddValueRate);
        if (M2Config.nDressACAddRate > 0 && Rnd.Next(M2Config.nDressACAddRate) == 0)
        {
            userItem.btValue[0] = nC + 1;
            if (userItem.btValue[0] > M2Config.nDressACAddValueMaxLimit)
                userItem.btValue[0] = M2Config.nDressACAddValueMaxLimit;
        }

        nC = GetRandomRange(M2Config.nDressMACAddValueMaxLimit, M2Config.nDressMACAddValueRate);
        if (M2Config.nDressMACAddRate > 0 && Rnd.Next(M2Config.nDressMACAddRate) == 0)
        {
            userItem.btValue[1] = nC + 1;
            if (userItem.btValue[1] > M2Config.nDressMACAddValueMaxLimit)
                userItem.btValue[1] = M2Config.nDressMACAddValueMaxLimit;
        }

        nC = GetRandomRange(M2Config.nDressDCAddValueMaxLimit, M2Config.nDressDCAddValueRate);
        if (M2Config.nDressDCAddRate > 0 && Rnd.Next(M2Config.nDressDCAddRate) == 0)
        {
            userItem.btValue[2] = nC + 1;
            if (userItem.btValue[2] > M2Config.nDressDCAddValueMaxLimit)
                userItem.btValue[2] = M2Config.nDressDCAddValueMaxLimit;
        }

        nC = GetRandomRange(M2Config.nDressMCAddValueMaxLimit, M2Config.nDressMCAddValueRate);
        if (M2Config.nDressMCAddRate > 0 && Rnd.Next(M2Config.nDressMCAddRate) == 0)
        {
            userItem.btValue[3] = nC + 1;
            if (userItem.btValue[3] > M2Config.nDressMCAddValueMaxLimit)
                userItem.btValue[3] = M2Config.nDressMCAddValueMaxLimit;
        }

        nC = GetRandomRange(M2Config.nDressSCAddValueMaxLimit, M2Config.nDressSCAddValueRate);
        if (M2Config.nDressSCAddRate > 0 && Rnd.Next(M2Config.nDressSCAddRate) == 0)
        {
            userItem.btValue[4] = nC + 1;
            if (userItem.btValue[4] > M2Config.nDressSCAddValueMaxLimit)
                userItem.btValue[4] = M2Config.nDressSCAddValueMaxLimit;
        }

        nC = GetRandomRange(6, 10);
        if (Rnd.Next(8) < 6)
        {
            int n10 = (nC + 1) * 2000;
            userItem.DuraMax = (ushort)Math.Min(65000, userItem.DuraMax + n10);
            userItem.Dura = (ushort)Math.Min(65000, userItem.Dura + n10);
        }
    }

    /// <summary>ItemNewAbilRandomUpgrade：新属性随机（btWhere 部位配置启用集）。</summary>
    public static void ItemNewAbilRandomUpgrade(TUserItem userItem, byte btWhere, M2ItemNewAbilConfig cfg)
    {
        if (btWhere >= cfg.Enabled.Length) return;
        int nMaxIndex = 13;
        if (cfg.Enabled[btWhere][12])
        {
            int nIndex = Rnd.Next(nMaxIndex);
            if (nIndex >= 0 && nIndex <= nMaxIndex - 1 && Rnd.Next(cfg.AddRate[btWhere]) == 0)
            {
                ApplyNewAbil(userItem, cfg, btWhere, nIndex);
            }
        }
        else
        {
            for (int nIndex = 0; nIndex <= nMaxIndex - 1; nIndex++)
            {
                if (cfg.Enabled[btWhere][nIndex] && Rnd.Next(cfg.AddRate[btWhere]) == 0)
                {
                    ApplyNewAbil(userItem, cfg, btWhere, nIndex);
                }
            }
        }
    }

    private static void ApplyNewAbil(TUserItem userItem, M2ItemNewAbilConfig cfg, byte btWhere, int nIndex)
    {
        if (nIndex is 1 or 7 or 8 or 10 or 22)
        {
            int nValue = GetRandomRange(cfg.AddValueMaxLimit2[btWhere], cfg.AddValueRate[btWhere]);
            userItem.SetNewValue(nIndex, (ushort)Math.Min(nValue + 1, cfg.AddValueMaxLimit2[btWhere]));
        }
        else
        {
            int nValue = GetRandomRange(cfg.AddValueMaxLimit[btWhere], cfg.AddValueRate[btWhere]);
            userItem.SetNewValue(nIndex, (ushort)Math.Min(nValue + 1, cfg.AddValueMaxLimit[btWhere]));
        }
    }
}

/// <summary>新属性随机配置（g_Config.ItemNewAbil 系列）。</summary>
public class M2ItemNewAbilConfig
{
    public bool[][] Enabled = CreateMatrix(13, 13, false);
    public int[] AddRate = new int[13];
    public int[] AddValueMaxLimit = new int[13];
    public int[] AddValueMaxLimit2 = new int[13];
    public int[] AddValueRate = new int[13];

    private static bool[][] CreateMatrix(int rows, int cols, bool init)
    {
        var m = new bool[rows][];
        for (int i = 0; i < rows; i++)
        {
            m[i] = new bool[cols];
            for (int j = 0; j < cols; j++) m[i][j] = init;
        }
        return m;
    }
}

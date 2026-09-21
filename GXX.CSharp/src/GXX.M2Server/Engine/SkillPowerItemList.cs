using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Engine;

/// <summary>M2Share.pas TSkillPowerItem record 1:1：装备技能威力百分比（攻击/防御各 [1..550]）。</summary>
public sealed class TSkillPowerItem
{
    public readonly byte[] AttackSkillPercent = new byte[TGroupItems.SKILL_POWER_MAGIC_COUNT + 1];
    public readonly byte[] DefenseSkillPercent = new byte[TGroupItems.SKILL_POWER_MAGIC_COUNT + 1];
}

/// <summary>
/// M2Share.pas g_SkillPowerItemList 1:1（批次J57）：装备技能威力表（SkillPowerItemList.txt INI，
/// 节名=装备名，键 Attack&lt;n&gt;/Defense&lt;n&gt;，仅非 0 项落盘；节名排序）。
/// </summary>
public class TSkillPowerItemList
{
    private readonly List<string> _strings = new();
    private readonly List<TSkillPowerItem> _objects = new();

    public int Count => _strings.Count;
    public string GetStrings(int index) => _strings[index];
    public TSkillPowerItem GetObjects(int index) => _objects[index];

    public int IndexOf(string sItemName)
    {
        for (int i = 0; i < _strings.Count; i++)
            if (string.Compare(sItemName, _strings[i], StringComparison.OrdinalIgnoreCase) == 0)
                return i;
        return -1;
    }

    public TSkillPowerItem? GetSkillPowerItem(string sItemName)
    {
        int i = IndexOf(sItemName);
        return i >= 0 ? _objects[i] : null;
    }

    public void Add(string sItemName, TSkillPowerItem item)
    {
        _strings.Add(sItemName);
        _objects.Add(item);
    }

    public void DeleteAt(int index)
    {
        _strings.RemoveAt(index);
        _objects.RemoveAt(index);
    }

    public void Clear()
    {
        _strings.Clear();
        _objects.Clear();
    }

    public List<string> Snapshot() => new(_strings);

    /// <summary>LoadSkillPowerItemList（M2Share.pas 16496）1:1。</summary>
    public void LoadFromFile()
    {
        string sFileName = M2Config.sEnvirDir + "SkillPowerItemList.txt";
        if (!File.Exists(sFileName))
            return;

        Clear();
        var ini = TGroupItems.ReadIniAll(sFileName);
        var sections = new List<string>(ini.Keys);
        sections.Sort(StringComparer.Ordinal);
        foreach (var sItemName in sections)
        {
            if (sItemName.Length == 0 || sItemName[0] == ';')
                continue;
            var item = new TSkillPowerItem();
            for (int ii = 1; ii <= TGroupItems.SKILL_POWER_MAGIC_COUNT; ii++)
            {
                if (!TGroupItems.IsValidMagicInSkillPowerItem(ii))
                    continue;
                item.AttackSkillPercent[ii] = (byte)TGroupItems.ReadIniInt(ini, sItemName, "Attack" + ii.ToString(CultureInfo.InvariantCulture), 0);
                item.DefenseSkillPercent[ii] = (byte)TGroupItems.ReadIniInt(ini, sItemName, "Defense" + ii.ToString(CultureInfo.InvariantCulture), 0);
            }
            Add(sItemName, item);
        }
    }

    /// <summary>SaveSkillPowerItemList（M2Share.pas 16547）1:1：先删文件，再逐节仅写非 0 项。</summary>
    public void SaveToFile()
    {
        string sFileName = M2Config.sEnvirDir + "SkillPowerItemList.txt";
        if (File.Exists(sFileName))
        {
            try { File.Delete(sFileName); } catch { /* 忽略 */ }
        }

        var sb = new StringBuilder();
        for (int i = 0; i < _strings.Count; i++)
        {
            var item = _objects[i];
            sb.Append('[').Append(_strings[i]).Append(']').Append("\r\n");
            for (int ii = 1; ii <= TGroupItems.SKILL_POWER_MAGIC_COUNT; ii++)
            {
                if (!TGroupItems.IsValidMagicInSkillPowerItem(ii))
                    continue;
                if (item.AttackSkillPercent[ii] != 0)
                    sb.Append("Attack").Append(ii).Append('=').Append(item.AttackSkillPercent[ii]).Append("\r\n");
                if (item.DefenseSkillPercent[ii] != 0)
                    sb.Append("Defense").Append(ii).Append('=').Append(item.DefenseSkillPercent[ii]).Append("\r\n");
            }
        }

        try
        {
            // GBK 一律经 GXX.Core.EncodingInit.GBK 获取：其内部先 Ensure() 注册 CodePagesEncodingProvider，消除加载顺序依赖（CP936 实例等价）。
            File.WriteAllText(sFileName, sb.ToString(), GXX.Core.EncodingInit.GBK);
        }
        catch
        {
            // 忽略
        }
    }
}

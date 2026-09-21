using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

// ============================================================================
// uCombatPowerUtils.pas 1:1 移植（批次J58）
// 战力计算：属性加权表（3 职业）+ 自定义变量加成 + 配置持久化（CombatPower.ini）
// ============================================================================

/// <summary>
/// uCombatPowerUtils.pas TCombatPowerAttrib 枚举 1:1（顺序即数组下标，勿改）。
/// </summary>
public enum TCombatPowerAttrib
{
    cpaMaxHP, cpaMaxMP, cpaAC1, cpaAC2, cpaMAC1, cpaMAC2, cpaDC1, cpaDC2, cpaMC1, cpaMC2, cpaSC1, cpaSC2,
    cpaAntiMagic, cpaAntiPoison, cpaPoisonRecover, cpaHPRecover, cpaMPRecover, cpaHitSpeed, cpaSpellSpeed, cpaHitPoint,
    cpaSpeedPoint, cpaLuckOrUnLuck, cpaParalysisRate, cpaMDParalysisRate, cpaFrozenRate, cpaCobwebWindingRate, cpaRevival,
    cpaMagicShield, cpaBlastHit, cpaDamageAdd, cpaDamageDec, cpaSpellDamageDec, cpaCloseDefense, cpaDamageRebound,
    cpaAddMonDropRate, cpaMaxHPAdd, cpaMaxMPAdd, cpaAngryValueTimAdd, cpaGroupDamageAdd, cpaAddHuamDropRate,
    cpaAddUndropRate, cpaUnParalysis, cpaUnMagicShield, cpaUnRevival, cpaUnPosion, cpaUnTamming, cpaUnFireCross,
    cpaUnFrozen, cpaUnCobwebWinding, cpaFatalBlowRate, cpaFatalBlowPower, cpaFatalBlowDefense, cpaUnBlastHit,
}

/// <summary>TCombatPowerVarRecord：战力变量记录（VarName string[40]）。</summary>
public sealed class TCombatPowerVarRecord
{
    public string VarName = "";   // string[40]
    public int Value0;            // 战士
    public int Value1;            // 法师
    public int Value2;            // 道士
    public string Desc = "";
}

/// <summary>
/// uCombatPowerUtils.pas TCombatPowerVarMgr 1:1：战力变量表。
/// FVarList 保持插入序，FSortVarList 按 VarName 做 AnsiCompareText 有序插入（二分 DoSearch）。
/// </summary>
public sealed class TCombatPowerVarMgr
{
    private readonly object _lock = new();
    private readonly List<TCombatPowerVarRecord> _varList = new();
    private readonly List<TCombatPowerVarRecord> _sortVarList = new();

    public int Count => _varList.Count;

    public TCombatPowerVarRecord GetItems(int index) => _varList[index];

    public void Lock() { /* lock(_lock) 由调用方 using 语义等价；此处保留显式 API */ }
    public void UnLock() { }

    /// <summary>遍历（供 RecalcPlayCombatPower 在锁内使用）。</summary>
    public IEnumerable<TCombatPowerVarRecord> Items
    {
        get { foreach (var r in _varList) yield return r; }
    }

    public void Clear()
    {
        _sortVarList.Clear();
        _varList.Clear();
    }

    /// <summary>DoSearch（402-426）：AnsiCompareText 二分查找，Index = 插入位。</summary>
    public bool DoSearch(string varName, out int index)
    {
        bool result = false;
        int l = 0;
        int h = Count - 1;
        while (l <= h)
        {
            int i = (l + h) >> 1;
            int c = string.Compare(_sortVarList[i].VarName, varName, StringComparison.OrdinalIgnoreCase);
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
                    // Delphi 原文注释：if Duplicates <> dupAccept then L := I;
                }
            }
        }
        index = l;
        return result;
    }

    /// <summary>Add（361-378）：重名拒绝；新记录按 VarName 有序插入 FSortVarList。</summary>
    public TCombatPowerVarRecord? Add(string varName, int value0, int value1, int value2, string desc)
    {
        if (DoSearch(varName, out int index))
            return null;

        var rec = new TCombatPowerVarRecord
        {
            VarName = varName,
            Value0 = value0,
            Value1 = value1,
            Value2 = value2,
            Desc = desc,
        };
        _varList.Add(rec);
        _sortVarList.Insert(index, rec);
        return rec;
    }

    /// <summary>Remove（380-400）1:1：有序表按二分位删除；原始表按 SameText 全扫并删（原文无 break）。</summary>
    public bool Remove(string varName)
    {
        bool result = false;

        if (DoSearch(varName, out int index))
            _sortVarList.RemoveAt(index);

        for (int i = 0; i < _varList.Count; i++)
        {
            var rec = _varList[i];
            if (string.Equals(rec.VarName, varName, StringComparison.OrdinalIgnoreCase))
            {
                _varList.RemoveAt(i);
                i--;
                result = true;
            }
        }
        return result;
    }

    /// <summary>GetValueRecord（443-452）：按有序表二分取记录。</summary>
    public TCombatPowerVarRecord? GetValueRecord(string varName)
        => DoSearch(varName, out int index) ? _sortVarList[index] : null;

    /// <summary>LoadConfig（459-497）：CombatPower.ini 除 DefaultAttrib 外的节 → 变量表。</summary>
    public void LoadConfig()
    {
        string fileName = M2Config.sEnvirDir + "CombatPower.ini";
        if (!File.Exists(fileName))
            return;

        Clear();
        var ini = TGroupItems.ReadIniAll(fileName);
        foreach (var section in ini.Keys)
        {
            if (string.Equals("DefaultAttrib", section, StringComparison.OrdinalIgnoreCase))
                continue;
            int v0 = ParseInt(TGroupItems.ReadIniString(ini, section, "Value0", "0"), 0);
            int v1 = ParseInt(TGroupItems.ReadIniString(ini, section, "Value1", "0"), 0);
            int v2 = ParseInt(TGroupItems.ReadIniString(ini, section, "Value2", "0"), 0);
            string desc = TGroupItems.ReadIniString(ini, section, "Desc", "");
            Add(section, v0, v1, v2, desc);
        }
    }

    /// <summary>SaveConfig（499-535）：擦除除 DefaultAttrib 外的全部节后逐条重写。</summary>
    public void SaveConfig()
    {
        string fileName = M2Config.sEnvirDir + "CombatPower.ini";
        if (!string.IsNullOrEmpty(M2Config.sEnvirDir) && !Directory.Exists(M2Config.sEnvirDir))
            Directory.CreateDirectory(M2Config.sEnvirDir);
        var ini = TGroupItems.ReadIniAll(fileName);
        var keep = new List<(string Section, Dictionary<string, string> Values)>();
        foreach (var kv in ini)
        {
            if (!string.Equals("DefaultAttrib", kv.Key, StringComparison.OrdinalIgnoreCase))
                continue;
            keep.Add((kv.Key, kv.Value));
        }

        var sb = new StringBuilder();
        foreach (var (section, values) in keep)
        {
            sb.Append('[').Append(section).Append(']').Append("\r\n");
            foreach (var kv in values)
                sb.Append(kv.Key).Append('=').Append(kv.Value).Append("\r\n");
        }
        foreach (var rec in _varList)
        {
            sb.Append('[').Append(rec.VarName).Append(']').Append("\r\n");
            sb.Append("Value0=").Append(rec.Value0.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("Value1=").Append(rec.Value1.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("Value2=").Append(rec.Value2.ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            sb.Append("Desc=").Append(rec.Desc).Append("\r\n");
        }

        try
        {
            // GBK 一律经 GXX.Core.EncodingInit.GBK 获取：其内部先 Ensure() 注册 CodePagesEncodingProvider，消除加载顺序依赖（CP936 实例等价）。
            File.WriteAllText(fileName, sb.ToString(), GXX.Core.EncodingInit.GBK);
        }
        catch
        {
            // Delphi TIniFile 写失败忽略
        }
    }

    private static int ParseInt(string s, int def)
        => int.TryParse(s?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : def;
}

/// <summary>
/// uCombatPowerUtils.pas 单元级（批次J58）：属性名/标识表 + 三职业加权默认值 + 全局管理器。
/// </summary>
public static class CombatPowerUtils
{
    /// <summary>CombatPowerAttribNames（74-79）：中文显示名（窗体列头用）。</summary>
    public static readonly string[] AttribNames =
    {
        "MaxHP", "MaxMP", "防御下限", "防御上限", "魔防下限", "魔防上限", "攻击下限", "攻击上限", "魔法下限", "魔法上限",
        "道术下限", "道术上限", "魔法躲避", "毒物躲避", "中毒恢复", "体力恢复", "魔法恢复", "攻击速度", "魔法速度", "精确度",
        "敏捷度", "幸运 / 诅咒", "麻痹机率", "魔道麻痹机率", "冰冻机率", "蛛网机率", "复活", "护身", "暴击几率", "攻击伤害",
        "伤害吸收", "魔法防御", "忽视防御", "伤害反弹", "怪物暴率", "体力增加", "魔力增加", "怒气恢复", "合击伤害", "人物爆率",
        "防爆出率", "防止麻痹", "防止护身", "防止复活", "防止全毒", "防止诱惑", "防止火墙", "防止冰冻", "防止蛛网",
        "致命一击几率", "致命一击伤害", "致命一击防御", "暴击抗性",
    };

    /// <summary>CombatPowerAttribIdents（94-100）：INI 键前缀（顺序与 TCombatPowerAttrib 一致）。</summary>
    public static readonly string[] AttribIdents =
    {
        "MaxHP", "MaxMP", "AC1", "AC2", "MAC1", "MAC2", "DC1", "DC2", "MC1", "MC2", "SC1", "SC2",
        "AntiMagic", "AntiPoison", "PoisonRecover", "HPRecover", "MPRecover", "HitSpeed", "SpellSpeed", "HitPoint",
        "SpeedPoint", "LuckOrUnLuck", "ParalysisRate", "MDParalysisRate", "FrozenRate", "CobwebWindingRate", "Revival",
        "MagicShield", "BlastHit", "DamageAdd", "DamageDec", "SpellDamageDec", "CloseDefense", "DamageRebound",
        "AddMonDropRate", "MaxHPAdd", "MaxMPAdd", "AngryValueTimAdd", "GroupDamageAdd", "AddHuamDropRate",
        "AddUndropRate", "UnParalysis", "UnMagicShield", "UnRevival", "UnPosion", "UnTamming", "UnFireCross",
        "UnFrozen", "UnCobwebWinding", "FatalBlowRate", "FatalBlowPower", "FatalBlowDefense", "UnBlastHit",
    };

    public const int AttribCount = 53;   // High(TCombatPowerAttrib) + 1

    /// <summary>g_DefCombatPowerValue[0..2][AttribCount]（职业 0 战士 / 1 法师 / 2 道士）。</summary>
    public static readonly int[][] DefCombatPowerValue = CreateDefaultTable();

    private static int[][] CreateDefaultTable()
    {
        var t = new int[3][];
        for (int i = 0; i < 3; i++)
            t[i] = new int[AttribCount];
        return t;
    }

    /// <summary>g_CombatPowerVarMgr。</summary>
    public static readonly TCombatPowerVarMgr CombatPowerVarMgr = new();

    /// <summary>LoadDefCombatPowerConfig（102-123）：CombatPower.ini [DefaultAttrib] 读入 3×53。</summary>
    /// <summary>确保 sEnvirDir 存在（Delphi ForceDirectories 等效；TIniFile 写盘前提）。</summary>
    private static void EnsureEnvirDir()
    {
        if (!string.IsNullOrEmpty(M2Config.sEnvirDir) && !Directory.Exists(M2Config.sEnvirDir))
            Directory.CreateDirectory(M2Config.sEnvirDir);
    }

    public static void LoadDefCombatPowerConfig()
    {
        string fileName = M2Config.sEnvirDir + "CombatPower.ini";
        if (!File.Exists(fileName))
            return;

        var ini = TGroupItems.ReadIniAll(fileName);
        for (int attr = 0; attr < AttribCount; attr++)
        {
            string ident = AttribIdents[attr];
            for (int job = 0; job < 3; job++)
            {
                string key = ident + "_" + job.ToString(CultureInfo.InvariantCulture);
                string cur = TGroupItems.ReadIniString(ini, "DefaultAttrib", key,
                    DefCombatPowerValue[job][attr].ToString(CultureInfo.InvariantCulture));
                DefCombatPowerValue[job][attr] = int.TryParse(cur?.Trim(), NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out var v) ? v : DefCombatPowerValue[job][attr];
            }
        }
    }

    /// <summary>SaveDefCombatPowerConfig（125-143）：3×53 写回 [DefaultAttrib]。</summary>
    public static void SaveDefCombatPowerConfig()
    {
        string fileName = M2Config.sEnvirDir + "CombatPower.ini";
        EnsureEnvirDir();
        var ini = TGroupItems.ReadIniAll(fileName);

        var sb = new StringBuilder();
        sb.Append("[DefaultAttrib]").Append("\r\n");
        for (int attr = 0; attr < AttribCount; attr++)
        {
            string ident = AttribIdents[attr];
            for (int job = 0; job < 3; job++)
            {
                sb.Append(ident).Append('_').Append(job).Append('=')
                  .Append(DefCombatPowerValue[job][attr].ToString(CultureInfo.InvariantCulture)).Append("\r\n");
            }
        }

        // 其他节保持原状（Delphi TIniFile 只改 [DefaultAttrib] 键，其余节内容不变）
        var sbOthers = new StringBuilder();
        foreach (var kv in ini)
        {
            if (string.Equals("DefaultAttrib", kv.Key, StringComparison.OrdinalIgnoreCase))
                continue;
            sbOthers.Append('[').Append(kv.Key).Append(']').Append("\r\n");
            foreach (var kvv in kv.Value)
                sbOthers.Append(kvv.Key).Append('=').Append(kvv.Value).Append("\r\n");
        }

        try
        {
            File.WriteAllText(fileName, sb.ToString() + sbOthers, GXX.Core.EncodingInit.GBK);
        }
        catch
        {
            // 忽略
        }
    }

    /// <summary>
    /// GetValNameNo（M2Share.pas 11453）1:1：变量名前缀 → 全局槽号。
    /// P=0 / D=+1000 / M=+2000 / N=+3000（N$ 走自定义）/ I=+4000 / G=+5000 / A=+6000 /
    /// S=+7000（S$ 走自定义）/ U=+8000（&lt;500）/ T=+8500（&lt;500）/ J=+9000（&lt;500）/
    /// Z=+9500（&lt;500）/ L=+10000；未识别返回 -1。
    /// </summary>
    public static int GetValNameNo(string sText)
    {
        sText = (sText ?? "").Trim();
        if (sText.Length < 2)
            return -1;

        char chr1 = char.ToUpperInvariant(sText[0]);
        string sValNo = sText.Substring(1);

        int Parse() => int.TryParse(sValNo.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : -1;
        bool DollarPrefixed = sValNo.Length > 0 && sValNo[0] == '$';

        switch (chr1)
        {
            case 'P': { int n = Parse(); return n >= 0 && n < 1000 ? n : -1; }
            case 'D': { int n = Parse(); return n >= 0 && n < 1000 ? n + 1000 : -1; }
            case 'M': { int n = Parse(); return n >= 0 && n < 1000 ? n + 2000 : -1; }
            case 'N':
                if (DollarPrefixed) return -1;
                { int n = Parse(); return n >= 0 && n < 1000 ? n + 3000 : -1; }
            case 'I': { int n = Parse(); return n >= 0 && n < 1000 ? n + 4000 : -1; }
            case 'G': { int n = Parse(); return n >= 0 && n < 1000 ? n + 5000 : -1; }
            case 'A': { int n = Parse(); return n >= 0 && n < 1000 ? n + 6000 : -1; }
            case 'S':
                if (DollarPrefixed) return -1;
                { int n = Parse(); return n >= 0 && n < 1000 ? n + 7000 : -1; }
            case 'U': { int n = Parse(); return n >= 0 && n < 500 ? n + 8000 : -1; }
            case 'T': { int n = Parse(); return n >= 0 && n < 500 ? n + 8500 : -1; }
            case 'J': { int n = Parse(); return n >= 0 && n < 500 ? n + 9000 : -1; }
            case 'Z': { int n = Parse(); return n >= 0 && n < 500 ? n + 9500 : -1; }
            case 'L':
                if (DollarPrefixed) return -1;
                { int n = Parse(); return n >= 0 && n < 1000 ? n + 10000 : -1; }
            default: return -1;
        }
    }

    /// <summary>
    /// RecalcPlayCombatPower（145-329）1:1：职业加权求和 → 变量加成 → m_nCombatPower。
    /// boOpenCombatPowerCalc 关闭时直接置 0；变量加成需 boOpenCombatPowerVarCalc 且为玩家。
    /// </summary>
    public static void RecalcPlayCombatPower(TCreature smartObject)
    {
        if (!M2Config.boOpenCombatPowerCalc)
        {
            smartObject.m_nCombatPower = 0;
            return;
        }

        int[] jobValues = smartObject.m_btJob switch
        {
            1 => DefCombatPowerValue[1],
            2 => DefCombatPowerValue[2],
            _ => DefCombatPowerValue[0],
        };

        ref var abil = ref smartObject.m_wAbil;
        long i64 =
            (long)DelphiRound(abil.MaxHP / 1000.0 * jobValues[(int)TCombatPowerAttrib.cpaMaxHP])
            + (long)DelphiRound(abil.MaxMP / 1000.0 * jobValues[(int)TCombatPowerAttrib.cpaMaxMP])
            + (long)abil.AC1 * jobValues[(int)TCombatPowerAttrib.cpaAC1]
            + (long)abil.AC2 * jobValues[(int)TCombatPowerAttrib.cpaAC2]
            + (long)abil.MAC1 * jobValues[(int)TCombatPowerAttrib.cpaMAC1]
            + (long)abil.MAC2 * jobValues[(int)TCombatPowerAttrib.cpaMAC2]
            + (long)abil.DC1 * jobValues[(int)TCombatPowerAttrib.cpaDC1]
            + (long)abil.DC2 * jobValues[(int)TCombatPowerAttrib.cpaDC2]
            + (long)abil.MC1 * jobValues[(int)TCombatPowerAttrib.cpaMC1]
            + (long)abil.MC2 * jobValues[(int)TCombatPowerAttrib.cpaMC2]
            + (long)abil.SC1 * jobValues[(int)TCombatPowerAttrib.cpaSC1]
            + (long)abil.SC2 * jobValues[(int)TCombatPowerAttrib.cpaSC2]
            + (long)smartObject.m_nAntiMagic * jobValues[(int)TCombatPowerAttrib.cpaAntiMagic]
            + (long)smartObject.m_btAntiPoison * jobValues[(int)TCombatPowerAttrib.cpaAntiPoison]
            + (long)smartObject.m_nPoisonRecover * jobValues[(int)TCombatPowerAttrib.cpaPoisonRecover]
            + (long)smartObject.m_nHealthRecover * jobValues[(int)TCombatPowerAttrib.cpaHPRecover]
            + (long)smartObject.m_nSpellRecover * jobValues[(int)TCombatPowerAttrib.cpaMPRecover]
            + (long)smartObject.m_nAttackSpeed * jobValues[(int)TCombatPowerAttrib.cpaHitSpeed]
            + (long)smartObject.m_nSpellSpeed * jobValues[(int)TCombatPowerAttrib.cpaSpellSpeed]
            + (long)smartObject.m_btHitPoint * jobValues[(int)TCombatPowerAttrib.cpaHitPoint]
            + (long)smartObject.m_btSpeedPoint * jobValues[(int)TCombatPowerAttrib.cpaSpeedPoint]
            + (long)smartObject.m_nLuck * jobValues[(int)TCombatPowerAttrib.cpaLuckOrUnLuck]
            + (long)smartObject.m_dwParalysisRate * jobValues[(int)TCombatPowerAttrib.cpaParalysisRate]
            + (long)smartObject.m_dwMDParalysisRate * jobValues[(int)TCombatPowerAttrib.cpaMDParalysisRate]
            + (long)smartObject.m_dwFrozenRate * jobValues[(int)TCombatPowerAttrib.cpaFrozenRate]
            + (long)smartObject.m_dwCobwebWindingRate * jobValues[(int)TCombatPowerAttrib.cpaCobwebWindingRate]
            + (smartObject.m_boRevival ? 1 : 0) * (long)jobValues[(int)TCombatPowerAttrib.cpaRevival]
            + (smartObject.m_boMagicShield ? 1 : 0) * (long)jobValues[(int)TCombatPowerAttrib.cpaMagicShield]
            + (long)abil.GetNewValue(0) * jobValues[(int)TCombatPowerAttrib.cpaBlastHit]
            + (long)abil.GetNewValue(1) * jobValues[(int)TCombatPowerAttrib.cpaDamageAdd]
            + (long)abil.GetNewValue(2) * jobValues[(int)TCombatPowerAttrib.cpaDamageDec]
            + (long)abil.GetNewValue(3) * jobValues[(int)TCombatPowerAttrib.cpaSpellDamageDec]
            + (long)abil.GetNewValue(4) * jobValues[(int)TCombatPowerAttrib.cpaCloseDefense]
            + (long)abil.GetNewValue(5) * jobValues[(int)TCombatPowerAttrib.cpaDamageRebound]
            + (long)abil.GetNewValue(6) * jobValues[(int)TCombatPowerAttrib.cpaAddMonDropRate]
            + (long)abil.GetNewValue(7) * jobValues[(int)TCombatPowerAttrib.cpaMaxHPAdd]
            + (long)abil.GetNewValue(8) * jobValues[(int)TCombatPowerAttrib.cpaMaxMPAdd]
            + (long)abil.GetNewValue(9) * jobValues[(int)TCombatPowerAttrib.cpaAngryValueTimAdd]
            + (long)abil.GetNewValue(10) * jobValues[(int)TCombatPowerAttrib.cpaGroupDamageAdd]
            + (long)abil.GetNewValue(11) * jobValues[(int)TCombatPowerAttrib.cpaAddHuamDropRate]
            + (long)abil.GetNewValue(12) * jobValues[(int)TCombatPowerAttrib.cpaAddUndropRate]
            + (long)abil.GetNewValue(13) * jobValues[(int)TCombatPowerAttrib.cpaUnParalysis]
            + (long)abil.GetNewValue(14) * jobValues[(int)TCombatPowerAttrib.cpaUnMagicShield]
            + (long)abil.GetNewValue(15) * jobValues[(int)TCombatPowerAttrib.cpaUnRevival]
            + (long)abil.GetNewValue(16) * jobValues[(int)TCombatPowerAttrib.cpaUnPosion]
            + (long)abil.GetNewValue(17) * jobValues[(int)TCombatPowerAttrib.cpaUnTamming]
            + (long)abil.GetNewValue(18) * jobValues[(int)TCombatPowerAttrib.cpaUnFireCross]
            + (long)abil.GetNewValue(19) * jobValues[(int)TCombatPowerAttrib.cpaUnFrozen]
            + (long)abil.GetNewValue(20) * jobValues[(int)TCombatPowerAttrib.cpaUnCobwebWinding]
            + (long)abil.GetNewValue(21) * jobValues[(int)TCombatPowerAttrib.cpaFatalBlowRate]
            + (long)abil.GetNewValue(22) * jobValues[(int)TCombatPowerAttrib.cpaFatalBlowPower]
            + (long)abil.GetNewValue(23) * jobValues[(int)TCombatPowerAttrib.cpaFatalBlowDefense]
            + (long)abil.GetNewValue(24) * jobValues[(int)TCombatPowerAttrib.cpaUnBlastHit];

        if (M2Config.boOpenCombatPowerVarCalc && smartObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT
            && smartObject is TPlayObject playerObj)
        {
            TPlayObject player = playerObj;
            foreach (var varRecord in CombatPowerVarMgr.Items)
            {
                string sVarName = varRecord.VarName;
                if (sVarName.Length == 0)
                    continue;

                int n01 = GetValNameNo(sVarName);
                int nVarValue = 0;

                if (n01 >= 0)
                {
                    switch (n01)
                    {
                        case >= 0 and <= 999:            // P（原文注释掉）
                            break;
                        case >= 1000 and <= 1999:        // D
                            nVarValue = player.m_DyVal[n01 - 1000];
                            break;
                        case >= 2000 and <= 2999:        // M
                            nVarValue = player.m_nMval[n01 - 2000];
                            break;
                        case >= 3000 and <= 3999:        // N
                            nVarValue = player.m_nInteger[n01 - 3000];
                            break;
                        case >= 4000 and <= 4999:        // I（原文注释掉）
                            break;
                        case >= 5000 and <= 5999:        // G（原文注释掉）
                            break;
                        case >= 6000 and <= 6999:        // A（原文注释掉）
                            break;
                        case >= 7000 and <= 7999:        // S（原文注释掉）
                            break;
                        case >= 8000 and <= 8499:        // U 私有数字型
                            nVarValue = player.m_UVal[n01 - 8000];
                            break;
                        case >= 8500 and <= 8999:        // T 私有字符串型（原文注释掉）
                            break;
                        case >= 9000 and <= 9499:        // J 私有数字型（1天1清）
                            nVarValue = player.m_JVal[n01 - 9000];
                            break;
                    }
                }
                else if (sVarName.Length > 2 && char.ToUpperInvariant(sVarName[0]) == 'S' && sVarName[1] == '$')
                {
                    int idx = player.m_StringList.GetIndex(sVarName.ToUpperInvariant());
                    if (idx >= 0)
                        nVarValue = ParseIntDef(player.m_StringList.Strings[idx], 0);
                }
                else if (sVarName.Length > 2 && char.ToUpperInvariant(sVarName[0]) == 'N' && sVarName[1] == '$')
                {
                    int idx = player.m_IntegerList.GetIndex(sVarName.ToUpperInvariant());
                    if (idx >= 0)
                        nVarValue = Convert.ToInt32(player.m_IntegerList.Objects[idx]);
                }

                if (nVarValue != 0)
                {
                    int powerValue = player.m_btJob switch
                    {
                        1 => varRecord.Value1,
                        2 => varRecord.Value2,
                        _ => varRecord.Value0,
                    };
                    if (powerValue != 0)
                        i64 += (long)nVarValue * powerValue;
                }
            }
        }

        // Delphi: SmartObject.m_nCombatPower := I64（Int64 → Integer 隐式截断）
        smartObject.m_nCombatPower = unchecked((int)i64);
    }

    /// <summary>Delphi Round（银行家舍入，与 M2ShareFuncs.DelphiRound 同源）。</summary>
    private static int DelphiRound(double v)
        => (int)Math.Round(v, MidpointRounding.ToEven);

    private static int ParseIntDef(string s, int def)
        => int.TryParse(s?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : def;
}

/// <summary>TCreature 批次J58 扩展：战力计算依赖字段。</summary>
public abstract partial class TCreature
{
    /// <summary>m_nCombatPower：战力值（Int64 截断为 Integer，与 Delphi 赋值语义一致）。</summary>
    public int m_nCombatPower;

    // m_btJob 已由 MagicEx.cs 的 TCreature partial 声明（0 战士 / 1 法师 / 2 道士），此处不重复
    public int m_nAttackSpeed;            // 攻击速度
    public int m_nSpellSpeed;             // 魔法速度
    public uint m_dwParalysisRate;        // 麻痹机率
    public uint m_dwMDParalysisRate;      // 魔道麻痹机率
    public uint m_dwFrozenRate;           // 冰冻机率
    public uint m_dwCobwebWindingRate;    // 蛛网机率
}

/// <summary>TPlayObject 批次J58 扩展：战力变量依赖的私有变量容器。</summary>
public partial class TPlayObject
{
    /// <summary>m_DyVal: array[0..999] of Integer（D 变量）。</summary>
    public readonly int[] m_DyVal = new int[1000];

    /// <summary>m_nMval: array[0..999] of Integer（M 变量）。</summary>
    public readonly int[] m_nMval = new int[1000];

    /// <summary>m_nInteger: array[0..999] of Integer（N 变量）。</summary>
    public readonly int[] m_nInteger = new int[1000];

    /// <summary>m_UVal: array[0..499] of Integer（U 私有数字型）。</summary>
    public readonly int[] m_UVal = new int[500];

    /// <summary>m_JVal: array[0..499] of Integer（J 私有数字型，1 天 1 清）。</summary>
    public readonly int[] m_JVal = new int[500];

    /// <summary>m_ZVal: array[0..499] of string[100]（Z 私有字符串型，1 天 1 清）。</summary>
    public readonly string[] m_ZVal = new string[500];

    /// <summary>m_StringList（S$ 自定义字符串变量表）。</summary>
    public readonly TValueListStub m_StringList = new();

    /// <summary>m_IntegerList（N$ 自定义整数变量表）。</summary>
    public readonly TQuickListStub m_IntegerList = new();
}

/// <summary>TValueList 最小等效（GetIndex / Strings / Count）——S$ 变量容器。</summary>
public sealed class TValueListStub
{
    private readonly List<string> _keys = new();
    private readonly List<string> _values = new();

    public int Count => _keys.Count;

    public IReadOnlyList<string> Strings => _values;

    /// <summary>GetIndex：UpperCompare 语义（不区分大小写）。</summary>
    public int GetIndex(string key)
    {
        for (int i = 0; i < _keys.Count; i++)
        {
            if (string.Equals(_keys[i], key, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    public void Add(string key, string value)
    {
        _keys.Add(key);
        _values.Add(value);
    }
}

/// <summary>TQuickList 最小等效（GetIndex / Objects / Count）——N$ 变量容器。</summary>
public sealed class TQuickListStub
{
    private readonly List<string> _keys = new();
    private readonly List<object> _objects = new();

    public int Count => _keys.Count;

    public IReadOnlyList<object> Objects => _objects;

    public int GetIndex(string key)
    {
        for (int i = 0; i < _keys.Count; i++)
        {
            if (string.Equals(_keys[i], key, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    public void Add(string key, object value)
    {
        _keys.Add(key);
        _objects.Add(value);
    }
}

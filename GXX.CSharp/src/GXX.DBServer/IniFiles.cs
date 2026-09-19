using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.Core;
using GXX.Core.Util;

namespace GXX.DBServer;

// ============================================================================================
// 接缝：Delphi RTL `IniFiles.TIniFile`（Win32 profile API 版）。
//   DBShare.pas / Setting.pas / Ranking.pas 使用 IniFiles.TIniFile（非 FastIniFile），
//   本文件按 RTL 语义逐条复刻：节/键的**首次出现顺序**即落盘顺序，每次写即落盘。
//   待 GXX.Core 移植 RTL IniFiles 后并入并删除本文件。
// ============================================================================================

/// <summary>
/// Delphi `IniFiles.TIniFile` 1:1 语义：
///   · 节顺序 = 节首次创建顺序；键顺序 = 键首次写入顺序（已存在的键就地改值，不移动位置）
///   · 写穿（每次 Write* 立即重写文件，等价 Win32 WritePrivateProfileString 的即时生效）
///   · 文本格式：[Section]\r\nKey=Value\r\n…（段间无空行，行尾 CRLF，GBK 编码）
///   · ReadSection 先 Clear；ReadSectionValues **不** Clear（原文如此，见 IniFiles.pas TIniFile.ReadSectionValues）
/// </summary>
public class TIniFile : IDisposable
{
    private readonly string _fileName;
    private readonly List<string> _sectionOrder = new List<string>();
    private readonly Dictionary<string, List<KeyValuePair<string, string>>> _sections
        = new Dictionary<string, List<KeyValuePair<string, string>>>(StringComparer.OrdinalIgnoreCase);

    public TIniFile(string fileName)
    {
        _fileName = fileName;
        Load();
    }

    public string FileName => _fileName;

    // ---------------- 内部 ----------------

    private void Load()
    {
        _sectionOrder.Clear();
        _sections.Clear();
        if (!File.Exists(_fileName)) return;

        string[] lines;
        try
        {
            lines = File.ReadAllLines(_fileName, EncodingInit.GBK);
        }
        catch
        {
            return;   // Delphi TIniFile.Create 对不存在/读失败的文件不抛异常，只是空
        }

        string cur = null;
        foreach (string raw in lines)
        {
            string line = raw.Trim();
            if (line.Length == 0) continue;                        // 空行（FastIniFile 写的段间空行也吃下）
            if (line[0] == ';' || line[0] == '#') continue;        // 注释
            if (line[0] == '[' && line[line.Length - 1] == ']')
            {
                cur = line.Substring(1, line.Length - 2).Trim();
                EnsureSection(cur);
                continue;
            }
            if (cur == null) continue;
            int eq = line.IndexOf('=');
            if (eq < 0) continue;
            SetKeyRaw(cur, line.Substring(0, eq).Trim(), line.Substring(eq + 1));
        }
    }

    private List<KeyValuePair<string, string>> EnsureSection(string section)
    {
        if (!_sections.TryGetValue(section, out var list))
        {
            list = new List<KeyValuePair<string, string>>();
            _sections[section] = list;
            _sectionOrder.Add(section);
        }
        return list;
    }

    private void SetKeyRaw(string section, string key, string value)
    {
        var list = EnsureSection(section);
        for (int i = 0; i < list.Count; i++)
        {
            if (string.Equals(list[i].Key, key, StringComparison.OrdinalIgnoreCase))
            {
                list[i] = new KeyValuePair<string, string>(list[i].Key, value);
                return;
            }
        }
        list.Add(new KeyValuePair<string, string>(key, value));
    }

    /// <summary>重写整个 INI 文件（节序/键序即内存顺序）。</summary>
    public void UpdateFile()
    {
        var sb = new StringBuilder();
        foreach (string sec in _sectionOrder)
        {
            sb.Append('[').Append(sec).Append(']').Append("\r\n");
            foreach (var kv in _sections[sec])
                sb.Append(kv.Key).Append('=').Append(kv.Value).Append("\r\n");
        }
        string dir = Path.GetDirectoryName(Path.GetFullPath(_fileName));
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(_fileName, sb.ToString(), EncodingInit.GBK);
    }

    // ---------------- 读 ----------------

    public string ReadString(string section, string key, string defaultValue)
    {
        if (_sections.TryGetValue(section, out var list))
            foreach (var kv in list)
                if (string.Equals(kv.Key, key, StringComparison.OrdinalIgnoreCase))
                    return kv.Value;
        return defaultValue;
    }

    public int ReadInteger(string section, string key, int defaultValue)
    {
        string s = ReadString(section, key, "");
        if (s.Length == 0) return defaultValue;
        s = s.Trim();
        if (s.Length == 0) return defaultValue;
        if (s[0] == '$')
        {
            try { return Convert.ToInt32(s.Substring(1), 16); } catch { return defaultValue; }
        }
        return int.TryParse(s, System.Globalization.NumberStyles.Integer | System.Globalization.NumberStyles.AllowLeadingSign,
            System.Globalization.CultureInfo.InvariantCulture, out int v) ? v : defaultValue;
    }

    /// <summary>Delphi `TCustomIniFile.ReadBool`：ReadInteger(...) &lt;&gt; 0。</summary>
    public bool ReadBool(string section, string key, bool defaultValue)
        => ReadInteger(section, key, defaultValue ? 1 : 0) != 0;

    /// <summary>Delphi `ReadSection`：先 Clear，再按文件序填入键名。</summary>
    public void ReadSection(string section, TStringList strings)
    {
        strings.Clear();
        if (_sections.TryGetValue(section, out var list))
            foreach (var kv in list) strings.Add(kv.Key);
    }

    public void ReadSections(TStringList strings)
    {
        strings.Clear();
        foreach (string s in _sectionOrder) strings.Add(s);
    }

    /// <summary>
    /// Delphi `TIniFile.ReadSectionValues`：**不清空** Strings（原文如此，LoadServerInfo 因此跨节累积）。
    /// </summary>
    public void ReadSectionValues(string section, TStringList strings)
    {
        var keyList = new TStringList();
        ReadSection(section, keyList);
        for (int i = 0; i < keyList.Count; i++)
            strings.Add(keyList[i] + "=" + ReadString(section, keyList[i], ""));
    }

    public bool SectionExists(string section) => _sections.ContainsKey(section);

    public bool ValueExists(string section, string key)
    {
        if (_sections.TryGetValue(section, out var list))
            foreach (var kv in list)
                if (string.Equals(kv.Key, key, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    // ---------------- 写 ----------------

    public void WriteString(string section, string key, string value)
    {
        SetKeyRaw(section, key, value);
        UpdateFile();
    }

    public void WriteInteger(string section, string key, int value)
        => WriteString(section, key, value.ToString(System.Globalization.CultureInfo.InvariantCulture));

    /// <summary>Delphi `TCustomIniFile.WriteBool`：写 '1'/'0'。</summary>
    public void WriteBool(string section, string key, bool value)
        => WriteString(section, key, value ? "1" : "0");

    /// <summary>Delphi `EraseSection`：整节删除；节不存在时不创建。</summary>
    public void EraseSection(string section)
    {
        if (_sections.Remove(section)) _sectionOrder.Remove(section);
        UpdateFile();
    }

    public void DeleteKey(string section, string key)
    {
        if (_sections.TryGetValue(section, out var list))
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Equals(list[i].Key, key, StringComparison.OrdinalIgnoreCase)) { list.RemoveAt(i); break; }
            }
        }
        UpdateFile();
    }

    public void Dispose() { }
}

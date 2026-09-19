using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GXX.Core.Util;

/// <summary>
/// FastIniFile.pas / TIniFile 1:1 公共 API（GBK 读写，节→键→值，注释保留简化为内存缓存）。
/// </summary>
public class TFastIniFile : IDisposable
{
    private readonly string _fileName;
    private readonly Dictionary<string, Dictionary<string, string>> _sections = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _sectionOrder = new();

    public TFastIniFile(string fileName)
    {
        _fileName = fileName;
        Load();
    }

    public string FileName => _fileName;

    private void Load()
    {
        _sections.Clear();
        _sectionOrder.Clear();
        if (!File.Exists(_fileName)) return;
        string current = "";
        foreach (string raw in File.ReadAllLines(_fileName, EncodingInit.GBK))
        {
            string line = raw.Trim();
            if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#')) continue;
            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                current = line.Substring(1, line.Length - 2).Trim();
                if (!_sections.ContainsKey(current))
                {
                    _sections[current] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    _sectionOrder.Add(current);
                }
                continue;
            }
            int eq = line.IndexOf('=');
            if (eq > 0 && current != "")
            {
                _sections[current][line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
            }
        }
    }

    public void UpdateFile() => Save();

    public void Save()
    {
        var sb = new StringBuilder();
        foreach (string sec in _sectionOrder)
        {
            sb.Append('[').Append(sec).Append(']').Append("\r\n");
            foreach (var kv in _sections[sec])
                sb.Append(kv.Key).Append('=').Append(kv.Value).Append("\r\n");
            sb.Append("\r\n");
        }
        string dir = Path.GetDirectoryName(_fileName);
        if (dir != "" && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(_fileName, sb.ToString(), EncodingInit.GBK);
    }

    public string ReadString(string section, string key, string defaultValue)
    {
        if (_sections.TryGetValue(section, out var keys) && keys.TryGetValue(key, out string? v))
            return v;
        return defaultValue;
    }

    public void WriteString(string section, string key, string value)
    {
        if (!_sections.TryGetValue(section, out var keys))
        {
            keys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _sections[section] = keys;
            _sectionOrder.Add(section);
        }
        keys[key] = value;
    }

    public int ReadInteger(string section, string key, int defaultValue)
        => int.TryParse(ReadString(section, key, ""), out int v) ? v : defaultValue;

    public void WriteInteger(string section, string key, int value)
        => WriteString(section, key, value.ToString());

    public bool ReadBool(string section, string key, bool defaultValue)
    {
        string s = ReadString(section, key, defaultValue ? "1" : "0");
        return s == "1" || s.Equals("True", StringComparison.OrdinalIgnoreCase);
    }

    public void WriteBool(string section, string key, bool value)
        => WriteString(section, key, value ? "1" : "0");

    public void ReadSection(string section, List<string> strings)
    {
        strings.Clear();
        if (_sections.TryGetValue(section, out var keys))
            strings.AddRange(keys.Keys);
    }

    public void ReadSections(List<string> strings)
    {
        strings.Clear();
        strings.AddRange(_sectionOrder);
    }

    public void EraseSection(string section)
    {
        if (_sections.Remove(section))
            _sectionOrder.Remove(section);
    }

    /// <summary>Delphi FastIniFile.ClearSection（清空节内全部键，节保留）。</summary>
    public void ClearSection(string section)
    {
        if (_sections.TryGetValue(section, out var keys))
            keys.Clear();
    }

    public void DeleteKey(string section, string key)
    {
        if (_sections.TryGetValue(section, out var keys))
            keys.Remove(key);
    }

    public bool SectionExists(string section) => _sections.ContainsKey(section);

    /// <summary>Delphi TIniFileEx.ValueExists（节内键存在判定）。</summary>
    public bool ValueExists(string section, string key)
        => _sections.TryGetValue(section, out var keys) && keys.ContainsKey(key);

    public void Dispose() { }
}

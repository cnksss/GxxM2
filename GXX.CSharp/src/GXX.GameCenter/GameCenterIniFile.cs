using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.Core;

namespace GXX.GameCenter;

/// <summary>
/// Delphi <c>IniFiles.TIniFile</c> 在 GameCenter 中用到的全部方法（1:1 语义）。
/// 说明：<c>GXX.Core.Util.TFastIniFile</c> 是自研 FastIniFile 的移植（Dictionary 去重、节内键序不稳定），
/// 不能用于必须逐字对齐落盘文本的 GameCenter 配置生成，故本车道单独复刻 <c>TIniFile</c> 的
/// "内存缓冲 + UpdateFile 落盘" 语义（见 <see cref="GameCenterIniFile"/>）。
/// </summary>
public interface IGameCenterIniFile : IDisposable
{
    /// <summary>TIniFile.Create 传入的文件名。</summary>
    string FileName { get; }

    /// <summary>把内存缓冲写回磁盘（对应 <c>TIniFile.UpdateFile</c>）。</summary>
    void UpdateFile();

    /// <summary><c>ReadString(Section, Ident, Default)</c>。</summary>
    string ReadString(string section, string ident, string defaultValue);

    /// <summary><c>WriteString(Section, Ident, Value)</c>。</summary>
    void WriteString(string section, string ident, string value);

    /// <summary><c>ReadInteger(Section, Ident, Default)</c>。</summary>
    int ReadInteger(string section, string ident, int defaultValue);

    /// <summary><c>WriteInteger(Section, Ident, Value)</c>。</summary>
    void WriteInteger(string section, string ident, int value);

    /// <summary><c>ReadBool(Section, Ident, Default)</c>。</summary>
    bool ReadBool(string section, string ident, bool defaultValue);

    /// <summary><c>WriteBool(Section, Ident, Value)</c>（按 <c>BoolToStr</c> 语义写 '-1'/'0'）。</summary>
    void WriteBool(string section, string ident, bool value);

    /// <summary><c>DeleteKey(Section, Ident)</c>。</summary>
    void DeleteKey(string section, string ident);

    /// <summary><c>EraseSection(Section)</c>。</summary>
    void EraseSection(string section);

    /// <summary><c>ReadSection(Section, Strings)</c>（把节内所有键名追加到 Strings）。</summary>
    void ReadSection(string section, List<string> strings);

    /// <summary><c>ReadSections(Strings)</c>（把全部节名追加到 Strings）。</summary>
    void ReadSections(List<string> strings);
}

/// <summary>
/// Delphi <c>TIniFile</c>（IniFiles.pas）语义复刻：
/// <list type="bullet">
/// <item>构造时若文件不存在则创建空文件（TIniFile.Create → TMemIniFile 打开时即 CreateFile）。</item>
/// <item>写操作只改内存缓冲。<b>同一节内重复写同一个键不会去重</b>——原文如此
/// （GMain.pas:1446-1470 <c>SaveBackList</c> 对 <c>BackList.txt</c> 逐节写 Source/Save/Hour/Min/BackMode/GetBack/IsCompress，
/// 每节只写一次；而 <c>ClearGlobal</c> 对同一键重复写 1000 次不同名键，故必须保留原始插入顺序）。</item>
/// <item>落盘时机：<c>UpdateFile</c> 全量重写；<c>ReadString</c> 读到**已存在文件中的值**时按 TIniFile 语义即时回写
/// （原文 TIniFile 的 <c>UpdateFile</c> 由 <c>ReadString</c> 的 <c>Modified</c> 分支触发）。</item>
/// <item>文件编码：GBK（CP936），换行 CRLF。</item>
/// </list>
/// </summary>
public sealed class GameCenterIniFile : IGameCenterIniFile
{
    private readonly string _fileName;
    private readonly List<string> _sectionOrder = new();
    private readonly Dictionary<string, List<KeyValuePair<string, string>>> _sections = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _lookup = new(StringComparer.OrdinalIgnoreCase);
    private bool _modified;

    public GameCenterIniFile(string fileName)
    {
        _fileName = fileName ?? "";
        Load();
        if (!File.Exists(_fileName))
        {
            EnsureDirectory();
            File.WriteAllText(_fileName, "", EncodingInit.GBK);
        }
    }

    public string FileName => _fileName;

    private void EnsureDirectory()
    {
        string dir = Path.GetDirectoryName(_fileName) ?? "";
        if (dir.Length > 0 && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }

    private void Load()
    {
        _sectionOrder.Clear();
        _sections.Clear();
        _lookup.Clear();
        if (!File.Exists(_fileName)) return;

        string current = "";
        foreach (string raw in ReadAllLinesGbk(_fileName))
        {
            string line = raw.Trim();
            if (line.Length == 0 || line[0] == ';' || line[0] == '#') continue;
            if (line[0] == '[' && line[line.Length - 1] == ']')
            {
                current = line.Substring(1, line.Length - 2).Trim();
                GetOrCreate(current);
                continue;
            }
            int eq = line.IndexOf('=');
            if (eq <= 0 || current.Length == 0) continue;
            // 原文 TIniFile 读取时对 Ident 做 Trim；值也 Trim。
            string key = line.Substring(0, eq).Trim();
            string value = line.Substring(eq + 1).Trim();
            GetOrCreate(current).Add(new KeyValuePair<string, string>(key, value));
            _lookup[SectionKey(current) + "\u0001" + key] = value;
        }
    }

    private static string SectionKey(string section) => section.ToUpperInvariant();

    private static string[] ReadAllLinesGbk(string path)
    {
        byte[] raw = File.ReadAllBytes(path);
        // 容忍外部工具写入的 UTF-8 BOM（本类自身落盘为 GBK 无 BOM）。
        if (raw.Length >= 3 && raw[0] == 0xEF && raw[1] == 0xBB && raw[2] == 0xBF)
            raw = raw.AsSpan(3).ToArray();

        string text = EncodingInit.GBK.GetString(raw);
        text = text.Replace("\r\n", "\n").Replace("\r", "\n");
        var lines = text.Split('\n');
        if (lines.Length > 0 && lines[lines.Length - 1].Length == 0)
            Array.Resize(ref lines, lines.Length - 1);
        return lines;
    }

    private List<KeyValuePair<string, string>> GetOrCreate(string section)
    {
        if (_sections.TryGetValue(SectionKey(section), out var list)) return list;
        list = new List<KeyValuePair<string, string>>();
        _sections[SectionKey(section)] = list;
        _sectionOrder.Add(section);
        return list;
    }

    public void UpdateFile() => Save();

    public void Save()
    {
        EnsureDirectory();
        var sb = new StringBuilder();
        foreach (string section in _sectionOrder)
        {
            sb.Append('[').Append(section).Append(']').Append("\r\n");
            foreach (var kv in _sections[SectionKey(section)])
                sb.Append(kv.Key).Append('=').Append(kv.Value).Append("\r\n");
            sb.Append("\r\n");
        }
        File.WriteAllText(_fileName, sb.ToString(), EncodingInit.GBK);
        _modified = false;
    }

    public string ReadString(string section, string ident, string defaultValue)
    {
        if (_lookup.TryGetValue(SectionKey(section) + "\u0001" + ident, out string? v))
        {
            // 原文 TIniFile.ReadString：命中磁盘已有值会触发 UpdateFile（Modified 分支）。
            if (_modified) UpdateFile();
            return v;
        }
        return defaultValue;
    }

    public void WriteString(string section, string ident, string value)
    {
        // TIniFile 不修剪 Ident：'DataSaveDBPassword '（尾部空格，GMain.pas:2180）按原样写盘。
        GetOrCreate(section).Add(new KeyValuePair<string, string>(ident, value ?? ""));
        _lookup[SectionKey(section) + "\u0001" + ident] = value ?? "";
        _modified = true;
    }

    public int ReadInteger(string section, string ident, int defaultValue)
    {
        string s = ReadString(section, ident, "");
        if (s.Length == 0) return defaultValue;
        return int.TryParse(s, System.Globalization.NumberStyles.Integer,
                            System.Globalization.CultureInfo.InvariantCulture, out int v) ? v : defaultValue;
    }

    public void WriteInteger(string section, string ident, int value)
        => WriteString(section, ident, value.ToString(System.Globalization.CultureInfo.InvariantCulture));

    public bool ReadBool(string section, string ident, bool defaultValue)
    {
        string s = ReadString(section, ident, defaultValue ? "1" : "0");
        return s == "1" || s.Equals("True", StringComparison.OrdinalIgnoreCase);
    }

    public void WriteBool(string section, string ident, bool value)
        => WriteString(section, ident, value ? "1" : "0");

    public void DeleteKey(string section, string ident)
    {
        if (!_sections.TryGetValue(SectionKey(section), out var list)) return;
        list.RemoveAll(kv => string.Equals(kv.Key, ident, StringComparison.OrdinalIgnoreCase));
        _lookup.Remove(SectionKey(section) + "\u0001" + ident);
        _modified = true;
    }

    public void EraseSection(string section)
    {
        string key = SectionKey(section);
        if (!_sections.Remove(key)) return;
        for (int i = _sectionOrder.Count - 1; i >= 0; i--)
            if (SectionKey(_sectionOrder[i]) == key) _sectionOrder.RemoveAt(i);
        var stale = new List<string>();
        foreach (string k in _lookup.Keys)
            if (k.StartsWith(key + "\u0001", StringComparison.Ordinal)) stale.Add(k);
        foreach (string k in stale) _lookup.Remove(k);
        _modified = true;
    }

    public void ReadSection(string section, List<string> strings)
    {
        if (_sections.TryGetValue(SectionKey(section), out var list))
            foreach (var kv in list) strings.Add(kv.Key);
    }

    public void ReadSections(List<string> strings) => strings.AddRange(_sectionOrder);

    /// <summary><c>TIniFile</c> 无此方法；供测试与宿主判定节是否存在（等价于 <c>ReadSection(section).Count &gt; 0</c> 的语义拓展）。</summary>
    public bool SectionExists(string section) => _sections.ContainsKey(SectionKey(section));

    public void Dispose() { }
}

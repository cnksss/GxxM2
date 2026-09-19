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

    /// <summary>
    /// Delphi <c>TCustomIniFile.ReadInteger</c>（FastIniFile 未重写，继承自 TCustomIniFile）：
    /// 先把 <c>'0x'/'0X'</c> 前缀改写成 <c>'$'</c>，再交给 <c>StrToIntDef</c>（失败回退 Default）。
    ///
    /// 原文依据：<c>Source/Common/FastIniFile/FastIniFile.pas:370</c>
    /// <c>TFastIniFile = class(TCustomIniFile)</c>、:428「And the rest of the Readers/Writers are
    /// inherited from TCustomIniFile」；本仓库无 Delphi RTL 源码，同源镜像见
    /// <c>Source/Common/MemoryIniFiles.pas:657-666</c>。GXX 内同一原文的已落地镜像：
    /// <c>src/GXX.DBServer/IniFiles.cs:127-139</c>。
    /// </summary>
    public int ReadInteger(string section, string key, int defaultValue)
    {
        string s = ReadString(section, key, "");
        if (s.Length == 0) return defaultValue;
        if (s.Length > 2 && s[0] == '0' && (s[1] == 'x' || s[1] == 'X')) s = "$" + s.Substring(2);
        if (s[0] == '$')
            return int.TryParse(s.Substring(1), System.Globalization.NumberStyles.HexNumber,
                System.Globalization.CultureInfo.InvariantCulture, out int hex) ? hex : defaultValue;
        return int.TryParse(s, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture, out int v) ? v : defaultValue;
    }

    /// <summary>
    /// Delphi <c>TCustomIniFile.WriteInteger</c>：<c>WriteString(Section, Ident, IntToStr(Value))</c>
    /// —— 十进制、带负号、无千分位（原文 IntToStr 与区域设置无关，故这里固定 InvariantCulture）。
    /// 原文依据：<c>FastIniFile.pas:370/428</c>；同源镜像 <c>MemoryIniFiles.pas:668-671</c>。
    /// </summary>
    public void WriteInteger(string section, string key, int value)
        => WriteString(section, key, value.ToString(System.Globalization.CultureInfo.InvariantCulture));

    /// <summary>
    /// Delphi <c>TCustomIniFile.ReadBool</c>：<c>ReadInteger(Section, Ident, Ord(Default)) &lt;&gt; 0</c>
    /// —— <b>任何非 0 整数都算 True</b>（'1'、'-1'、'2'、'$1'…），非数字串（含 <c>'True'</c>）
    /// 回退到 Default。
    ///
    /// 原文依据：<c>FastIniFile.pas:370</c>（继承 <c>TCustomIniFile</c>）+ <c>:428</c>；
    /// 同源镜像 <c>Source/Common/MemoryIniFiles.pas:673-676</c>；
    /// GXX 内同一原文的已落地镜像 <c>src/GXX.DBServer/IniFiles.cs:141-143</c>。
    ///
    /// 修正记录（并行批次 P2）：旧实现是 <c>s == "1" || s.Equals("True", OrdinalIgnoreCase)</c>，
    /// 与原文有两处不符 —— ① <c>'-1'</c> 被读成 False（LoginSrv BasicSet 落的正是 '-1'，
    /// BasicSet.cs:741，于是布尔键读回全假）；② <c>'True'</c> 被无条件读成 True（原文应回退 Default）。
    /// </summary>
    public bool ReadBool(string section, string key, bool defaultValue)
        => ReadInteger(section, key, defaultValue ? 1 : 0) != 0;

    /// <summary>
    /// Delphi <c>TCustomIniFile.WriteBool</c>：<c>const Values: array[Boolean] of string = ('0','1')</c>
    /// —— False 写 <c>'0'</c>、True 写 <c>'1'</c>。
    ///
    /// 原文依据：<c>FastIniFile.pas:370</c>（<c>TFastIniFile = class(TCustomIniFile)</c>）与 <c>:428</c>
    /// 「And the rest of the Readers/Writers are inherited from TCustomIniFile」—— 本单元**没有**自己的
    /// WriteBool；同源镜像 <c>Source/Common/MemoryIniFiles.pas:678-683</c>。
    ///
    /// 注意（原文如此，勿"统一"）：本方法写 <c>'1'/'0'</c>，而 SysUtils.BoolToStr（Delphi 7 默认重载，
    /// 见 <see cref="GXX.Core.Rtl.DelphiRTL.BoolToStr"/>）给的是 <c>'-1'/'0'</c>；两者都通过
    /// <c>WriteString</c> 落盘时文本不同 —— 前者是 TCustomIniFile 的整数约定，后者是 BoolToStr 的约定。
    /// </summary>
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

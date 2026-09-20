using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// 别名：与 Delphi TEncoding 逐字对照（FastIniFile.pas:1762-1773 的 Encoding 变量）
using TEncoding = System.Text.Encoding;

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

    /// <summary>
    /// 读取并解析 INI 文件（原文 <c>FastIniFile.pas</c> 的 <c>TIniItems.LoadFromFile</c> /
    /// <c>LoadFromStream</c>；<c>:1762-1773</c> 的 <c>COMPILER12_UP</c> 分支）。
    ///
    /// <para>
    /// 修正记录（车道 p8-m2-itemprop-misc 请求 #4）：旧实现固定
    /// <c>File.ReadAllLines(fileName, GBK)</c> ⇒ **无 BOM 的 UTF-8 文件被当 GBK 读**（乱码）。
    /// 原文的 <c>TIniItems.LoadFromStream</c> 走的是
    /// <c>Size := TEncoding.GetBufferEncoding(Buffer, Encoding)</c> +
    /// <c>SetTextStr(Encoding.GetString(Buffer, Size, Length(Buffer) - Size))</c>，
    /// 现按此接线（本工程编译配置为 Unicode Delphi ⇒ <c>COMPILER12_UP</c> 分支生效）。
    /// </para>
    /// <para>
    /// 行切分沿用原文的 <c>SetTextStr</c> 语义（<c>#13</c>/<c>#10</c>/<c>#13#10</c>），
    /// 与 <see cref="GXX.Core.Util.TStringList.SetTextStr"/> 同一实现。
    /// </para>
    /// <para>
    /// ★ 不对称（登记 D-P8-15，本次范围外）：<c>Save</c> 仍固定写 GBK；Delphi 的 TIniItems 会把
    /// 读入时的 Encoding 用于回写。即"从 UTF-8 文件载入的 INI 再落盘会变回 GBK"。
    /// </para>
    /// </summary>
    private void Load()
    {
        _sections.Clear();
        _sectionOrder.Clear();
        if (!File.Exists(_fileName)) return;

        byte[] buffer = File.ReadAllBytes(_fileName);
        TEncoding? encoding = null;
        int skip = GXX.Core.EncodingHelper.TEncodingHelper.GetBufferEncoding(buffer, ref encoding, EncodingInit.GBK);
        string text = encoding!.GetString(buffer, skip, buffer.Length - skip);

        var lines = new TStringList();
        lines.SetTextStr(text);

        string current = "";
        foreach (string raw in lines.AsEnumerable())
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

    /// <summary>
    /// Delphi <c>TFastIniFile.ReadFixedDateTime(const Section, Ident: string; Default: TDateTime): TDateTime</c>
    /// （<c>FastIniFile.pas:2953-2971</c>）1:1。
    ///
    /// <para>逐分支保真（三条都不可省）：</para>
    /// <list type="number">
    /// <item><c>S := ReadString(Section, Ident, '')</c>；<c>I := Pos(' ', S)</c>；
    ///   <b>没有空格时直接返回 Default —— 连日期段都不解析</b>（<c>:2963-2964</c>）；</item>
    /// <item>日期段取 <c>Copy(S, 1, Length(FIXED_DATE))</c> = 前 **10** 字符，
    ///   经 <c>StrToDateDef(..., -1)</c>（把 <c>DateSeparator</c> 临时改成 <c>'-'</c>、
    ///   <c>ShortDateFormat</c> 改成 <c>'dd-mm-yyyy'</c>）解析，失败得 <c>-1</c> → 返回 Default；</item>
    /// <item>时间段取 <c>Copy(S, I+1, Length(FIXED_TIME))</c> = 其后 **8** 字符，
    ///   经 <c>StrToTimeDef(..., 0)</c> 解析。</item>
    /// </list>
    ///
    /// <para>
    /// ★★ <b>原文缺陷（照抄，勿"修"）</b>：时间段那一句的 Default 传的是 <b>0</b>，而判据写的是
    /// <c>if (D &lt;&gt; -1) and (T &lt;&gt; -1) then</c> —— 于是**时间解析失败（T = 0）仍被接受**，
    /// 结果静默变成"当天 00:00:00"。例：<c>'25-12-2023 garbage!'</c> → 2023-12-25 00:00:00。
    /// </para>
    ///
    /// <para>原文依据：<c>FastIniFile.pas:2953-2971</c>；<c>FIXED_*</c> 常量见 <c>:189-194</c>；
    /// 局部 <c>StrToDateDef/StrToTimeDef</c> 见 <c>:988-1024</c>。</para>
    /// </summary>
    public double ReadFixedDateTime(string section, string ident, double defaultValue)
    {
        double result = defaultValue;
        string s = ReadString(section, ident, "");
        int i = s.IndexOf(' ') + 1;                       // Delphi Pos(' ', S)：1-based，未找到为 0
        if (i > 0)
        {
            double d = TIniFixedDateTime.StrToDateDef(
                GXX.Core.Rtl.DelphiRTL.Copy(s, 1, TIniFixedDateTime.FIXED_DATE.Length), -1);
            double t = TIniFixedDateTime.StrToTimeDef(
                GXX.Core.Rtl.DelphiRTL.Copy(s, i + 1, TIniFixedDateTime.FIXED_TIME.Length), 0);
            if (d != -1 && t != -1)                       // ← 原文瑕疵：t 的 Default 是 0，故恒真
                result = d + t;
        }
        return result;
    }

    /// <summary>
    /// Delphi <c>TFastIniFile.WriteFixedDateTime(const Section, Ident: string; Value: TDateTime)</c>
    /// （<c>FastIniFile.pas:2985-2989</c>）1:1：
    /// <c>WriteString(Section, Ident, FormatDateTime(FIXED_DATETIME, Value))</c>，
    /// 即 <c>'dd-mm-yyyy hh:nn:ss'</c>（<c>'-'</c>/<c>':'</c> 在格式串里是字面量 → 与本地区域设置无关）。
    /// </summary>
    public void WriteFixedDateTime(string section, string ident, double value)
        => WriteString(section, ident, TIniFixedDateTime.FormatFixedDateTime(value));

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

/// <summary>
/// FastIniFile.pas 的固定格式日期时间常量与解析器（<c>:189-194</c>、<c>:988-1024</c>、
/// <c>:2985-2989</c>）1:1。
///
/// <para>
/// 为何独立成类：原文里 <c>FIXED_*</c> 是单元级 <c>const</c>、<c>StrToDateDef/StrToTimeDef</c> 是
/// <c>implementation</c> 段内的单元级函数（不是 <c>TFastIniFile</c> 的成员），托管侧用静态类承载。
/// </para>
/// <para>
/// 迁移记录（车道 p8-m2-itemprop-misc，请求 #2）：本类内容原在
/// <c>GXX.M2Server/Misc/SellPlayerSeams.cs</c> 的 <c>SellPlayerIni</c> 里（因 SellPlayer.pas:216/:255
/// 需要而临时落地）。按"<c>GXX.Core</c> 不得反向依赖 <c>GXX.M2Server</c>"的方向约束**整体搬进本文件**，
/// <c>SellPlayerIni</c> 现只保留转调。
/// </para>
/// </summary>
public static class TIniFixedDateTime
{
    /// <summary>FastIniFile.pas:190 <c>FIXED_DS = '-'</c>。</summary>
    public const char FIXED_DS = '-';

    /// <summary>FastIniFile.pas:191 <c>FIXED_DATE = 'dd-mm-yyyy'</c>（10 字符）。</summary>
    public const string FIXED_DATE = "dd" + "-" + "mm" + "-" + "yyyy";

    /// <summary>FastIniFile.pas:192 <c>FIXED_TS = ':'</c>。</summary>
    public const char FIXED_TS = ':';

    /// <summary>FastIniFile.pas:193 <c>FIXED_TIME = 'hh:nn:ss'</c>（8 字符；Delphi 里 <c>nn</c> 才是分钟）。</summary>
    public const string FIXED_TIME = "hh" + ":" + "nn" + ":" + "ss";

    /// <summary>FastIniFile.pas:194 <c>FIXED_DATETIME = FIXED_DATE + ' ' + FIXED_TIME</c>。</summary>
    public const string FIXED_DATETIME = FIXED_DATE + " " + FIXED_TIME;

    /// <summary>
    /// Delphi <c>FormatDateTime('dd-mm-yyyy hh:nn:ss', Value)</c>：<c>'-'</c>/<c>':'</c> 是字面量，
    /// 故结果与本地区域设置无关（托管侧固定 <c>InvariantCulture</c>）。
    /// <para>超出 OLE 自动化日期可表示范围的值返回空串（Delphi 侧 <c>FormatDateTime</c> 会抛
    /// <c>EConvertError</c>；托管侧选择"写空串"而非抛，见报告偏离 D-P8-11）。</para>
    /// </summary>
    public static string FormatFixedDateTime(double value)
    {
        if (double.IsNaN(value) || value < -657435.0 || value > 2958465.99999999)
            return "";
        return DateTime.FromOADate(value).ToString("dd-MM-yyyy HH:mm:ss",
            System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// FastIniFile.pas:988-1007 的局部 <c>StrToDateDef</c> 1:1：临时把
    /// <c>DateSeparator := '-'</c>、<c>ShortDateFormat := 'dd-mm-yyyy'</c> 再解析，失败返回 Default。
    /// <para>偏离 D-P8-4：托管侧用 "dd-MM-yyyy" 精确格式解析（不受区域设置影响）；
    /// Delphi 的 <c>StrToDate</c> 另有若干宽松形态（AM/PM、单数字月日等）不在复刻范围。</para>
    /// </summary>
    public static double StrToDateDef(string Value, double Default)
    {
        if (DateTime.TryParseExact(Value, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime dt))
            return dt.ToOADate();
        return Default;
    }

    /// <summary>
    /// FastIniFile.pas:1009-1024 的局部 <c>StrToTimeDef</c> 1:1：临时把
    /// <c>TimeSeparator := ':'</c> 再解析，失败返回 Default。
    /// <para>偏离 D-P8-4 同上（"HH:mm:ss" 精确格式）。</para>
    /// </summary>
    public static double StrToTimeDef(string Value, double Default)
    {
        if (DateTime.TryParseExact(Value, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime dt))
            return dt.TimeOfDay.TotalDays;
        return Default;
    }
}

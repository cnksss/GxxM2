using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.Core.Rtl;

// 别名：与 Delphi TStrings 的 FEncoding/DefaultEncoding 类型逐字对照
using TEncoding = System.Text.Encoding;

namespace GXX.Core.Util;

/// <summary>
/// Delphi Classes.TStringList 1:1 语义复刻：
/// Strings + Objects 双列、Sorted、CaseSensitive、Exchange/InsertObject/Delete、LoadFrom/SaveTo（GBK）。
///
/// <para>
/// 补齐记录（车道 p8-m2-itemprop-misc，请求 #3）：原实现缺 <c>TStrings.Text</c>
/// （<c>GetTextStr</c>/<c>SetTextStr</c>）与编码状态（<c>DefaultEncoding</c>/<c>SetEncoding</c>/<c>GetEncoding</c>），
/// 导致 <c>uFrmCustomItemProperty.pas:304/:472</c> 等调用点无法 1:1。现已补上，
/// 并把 <c>LoadFromFile</c>/<c>SaveToFile</c> 接到 <c>TEncodingHelper.GetBufferEncoding</c>
/// （=Delphi RTL <c>TStrings.LoadFromStream</c>/<c>SaveToStream</c> 的 BOM/UTF-16/无 BOM UTF-8 嗅探语义）。
/// </para>
/// </summary>
public class TStringList
{
    private readonly List<string> _strings = new();
    private readonly List<object> _objects = new();
    private bool _sorted;
    private bool _caseSensitive;

    /// <summary>原文 TStrings.FDefaultEncoding（懒取 TEncoding.Default = 系统 ANSI = 936）。</summary>
    private TEncoding? _defaultEncoding;

    /// <summary>原文 TStrings.FEncoding（由 LoadFrom* 的嗅探结果或 SetEncoding 写入）。</summary>
    private TEncoding? _encoding;

    /// <summary>原文 TStrings.FLineBreak（GetTextStr 的换行符；默认 sLineBreak = CRLF）。</summary>
    private string _lineBreak = "\r\n";

    public int Count => _strings.Count;

    /// <summary>
    /// Delphi <c>TStrings.DefaultEncoding</c>：懒取 <c>TEncoding.Default</c>
    /// （系统 ANSI 代码页；本工程 936/GBK，不是 .NET Core 的 <c>Encoding.Default</c>=UTF-8）。
    /// </summary>
    public TEncoding DefaultEncoding
    {
        get => _defaultEncoding ??= EncodingInit.GBK;
        set => _defaultEncoding = value;
    }

    /// <summary>
    /// Delphi <c>TStrings.GetEncoding</c>：未显式设置时取 <see cref="DefaultEncoding"/> 并缓存。
    /// </summary>
    public TEncoding GetEncoding() => _encoding ??= DefaultEncoding;

    /// <summary>
    /// Delphi <c>TStrings.SetEncoding(Value: TEncoding)</c>（原文为 protected；class helper
    /// <c>StringListHelper</c> 需要调用它，故托管侧公开）。
    /// </summary>
    public void SetEncoding(TEncoding value) => _encoding = value;

    /// <summary>
    /// Delphi <c>TStrings.LineBreak</c>：GetTextStr 用的换行串，默认 <c>sLineBreak</c>（CRLF）。
    /// <para>原文 <c>SetLineBreak</c> 在传入空串时抛 <c>EArgumentException</c>；此处抛
    /// <see cref="ArgumentException"/> 对齐。</para>
    /// </summary>
    public string LineBreak
    {
        get => _lineBreak;
        set => _lineBreak = string.IsNullOrEmpty(value)
            ? throw new ArgumentException("LineBreak 不能为空串（Delphi TStrings.SetLineBreak）", nameof(value))
            : value;
    }

    /// <summary>
    /// Delphi <c>TStrings.Text</c>（<c>GetTextStr</c>/<c>SetTextStr</c>）1:1。
    /// <para>
    /// 读：<b>每行后都追加 <see cref="LineBreak"/></b>（含最后一行）⇒ <c>Count &gt; 0</c> 时结果必以换行结尾；
    /// <c>Count = 0</c> 时空串。<br/>
    /// 写：先 <c>Clear</c>，再按 <c>#13</c>/<c>#10</c>/<c>#13#10</c> 切行逐条 <c>Add</c>。
    /// </para>
    /// </summary>
    public string Text
    {
        get => GetTextStr();
        set => SetTextStr(value);
    }

    /// <summary>
    /// Delphi <c>TStrings.GetTextStr</c>：<c>for I := 0 to Count - 1 do Result := Result + Strings[I] + LineBreak</c>。
    /// </summary>
    public string GetTextStr()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < _strings.Count; i++)
            sb.Append(_strings[i]).Append(_lineBreak);
        return sb.ToString();
    }

    /// <summary>
    /// Delphi <c>TStrings.SetTextStr</c>：按 <c>#13</c>/<c>#10</c>/<c>#13#10</c> 切行，**先 Clear 再逐行 Add**。
    /// <para>边界（1:1）：<c>''</c> → 0 行；<c>"a\r\n"</c> → 1 行；<c>"\r\n"</c> → 1 个空行；
    /// <c>"a\n\nb"</c> → <c>["a","","b"]</c>；裸 <c>\r</c> 也算换行。</para>
    /// <para>注意 SetTextStr **不使用** <see cref="LineBreak"/>（原文硬编码 #13/#10），
    /// 只有 GetTextStr 用它。</para>
    /// </summary>
    public void SetTextStr(string value)
    {
        Clear();
        value ??= "";
        int p = 0;
        int len = value.Length;
        while (p < len)
        {
            int start = p;
            while (p < len && value[p] != '\n' && value[p] != '\r')
                p++;
            Add(value.Substring(start, p - start));
            if (p < len && value[p] == '\r') p++;
            if (p < len && value[p] == '\n') p++;
        }
    }

    /// <summary>Delphi TStringList.Sorted：置 True 时立即排序（后续插入保持有序）。</summary>
    public bool Sorted
    {
        get => _sorted;
        set
        {
            if (value && !_sorted)
                Sort();
            _sorted = value;
        }
    }
    public bool CaseSensitive { get => _caseSensitive; set => _caseSensitive = value; }

    public string this[int index]
    {
        get => _strings[index];
        set => _strings[index] = value;
    }

    public object GetObject(int index) => _objects[index];
    public void PutObject(int index, object value) => _objects[index] = value;

    public int CompareStrings(string a, string b)
        => _caseSensitive ? string.CompareOrdinal(a, b) : string.Compare(a, b, StringComparison.OrdinalIgnoreCase);

    public int Add(string s)
    {
        if (_sorted)
        {
            int lo = 0, hi = Count;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (CompareStrings(s, _strings[mid]) > 0) lo = mid + 1; else hi = mid;
            }
            _strings.Insert(lo, s);
            _objects.Insert(lo, null);
            return lo;
        }
        _strings.Add(s);
        _objects.Add(null);
        return Count - 1;
    }

    public int AddObject(string s, object obj)
    {
        int idx = Add(s);
        _objects[idx] = obj;
        return idx;
    }

    public void Insert(int index, string s)
    {
        _strings.Insert(index, s);
        _objects.Insert(index, null);
    }

    public void InsertObject(int index, string s, object obj)
    {
        _strings.Insert(index, s);
        _objects.Insert(index, obj);
    }

    public void Delete(int index)
    {
        _strings.RemoveAt(index);
        _objects.RemoveAt(index);
    }

    public void Exchange(int i, int j)
    {
        (_strings[i], _strings[j]) = (_strings[j], _strings[i]);
        (_objects[i], _objects[j]) = (_objects[j], _objects[i]);
    }

    public void Clear()
    {
        _strings.Clear();
        _objects.Clear();
    }

    public int IndexOf(string s)
    {
        for (int i = 0; i < Count; i++)
            if (string.Equals(_strings[i], s, StringComparison.Ordinal)) return i;
        return -1;
    }

    public int IndexOfName(string name)
    {
        for (int i = 0; i < Count; i++)
        {
            string s = _strings[i];
            int eq = s.IndexOf('=');
            if (eq > 0 && string.Equals(s.Substring(0, eq), name, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    public string GetValueFromName(string name)
    {
        int i = IndexOfName(name);
        if (i < 0) return "";
        int eq = _strings[i].IndexOf('=');
        return eq >= 0 ? _strings[i].Substring(eq + 1) : "";
    }

    public void Sort()
    {
        // 简单稳定排序（保持 Objects 同步）
        var idx = new List<int>(Count);
        for (int i = 0; i < Count; i++) idx.Add(i);
        idx.Sort((a, b) => CompareStrings(_strings[a], _strings[b]));
        var ns = new List<string>(Count);
        var no = new List<object>(Count);
        foreach (int i in idx) { ns.Add(_strings[i]); no.Add(_objects[i]); }
        _strings.Clear(); _strings.AddRange(ns);
        _objects.Clear(); _objects.AddRange(no);
        _sorted = true;
    }

    /// <summary>
    /// Delphi <c>TStrings.LoadFromFile(const FileName: string)</c> 的**编码嗅探**版本
    /// （原文 <c>LoadFromFile → LoadFromStream(Stream)</c> →
    /// <c>TEncoding.GetBufferEncoding(Buffer, Encoding, TEncoding.Default)</c>）。
    ///
    /// <para>
    /// 修正记录（请求 #3）：旧实现固定 <c>File.ReadAllText(GBK)</c> ⇒ **无 BOM 的 UTF-8 文件被当 GBK 读**（乱码）。
    /// 现按文案原文走 BOM/UTF-16/无 BOM UTF-8 嗅探链路，并用嗅探结果解码、写入 <c>SetEncoding</c>。
    /// </para>
    /// <para>
    /// 偏离 D-P8-12（保留既有托管行为）：文件不存在时**不抛**（Delphi 的 <c>TFileStream.Create</c> 会抛
    /// <c>EFOpenError</c>）。既有各车道调用点依赖"缺失即空表"，本次只改编码路径、不动缺文件语义。
    /// </para>
    /// </summary>
    public void LoadFromFile(string fileName)
    {
        Clear();
        if (!File.Exists(fileName)) return;

        byte[] buffer = File.ReadAllBytes(fileName);
        TEncoding? encoding = null;
        int skip = GXX.Core.EncodingHelper.TEncodingHelper.GetBufferEncoding(buffer, ref encoding, DefaultEncoding);
        SetEncoding(encoding!);
        SetTextStr(encoding!.GetString(buffer, skip, buffer.Length - skip));
    }

    /// <summary>
    /// Delphi <c>TStrings.SaveToFile(const FileName: string)</c> →
    /// <c>SaveToStream(Stream, GetEncoding)</c>：**先写 <c>GetPreamble</c>，再写
    /// <c>GetBytes(GetTextStr)</c>**。
    ///
    /// <para>
    /// 行为变更（请求 #3，登记 D-P8-13）：旧实现固定写 GBK 且不写 BOM。现按
    /// <see cref="GetEncoding"/> 写 —— 对"从未 LoadFrom* 过"或"从 GBK/默认编码文件载入"的列表，
    /// <c>GetEncoding()</c> = GBK（preamble 为空）⇒ **落盘字节与旧实现完全一致**；
    /// 只有从 UTF-8/UTF-16 文件载入过的列表才改按该编码写回（这正是 BOM 嗅探的意义）。
    /// </para>
    /// </summary>
    public void SaveToFile(string fileName)
    {
        TEncoding encoding = GetEncoding();
        using var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
        byte[] preamble = encoding.GetPreamble();
        if (preamble.Length > 0)
            stream.Write(preamble, 0, preamble.Length);
        byte[] body = encoding.GetBytes(GetTextStr());
        stream.Write(body, 0, body.Length);
    }

    public System.Collections.Generic.IEnumerable<string> AsEnumerable() => _strings;
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.Core.Rtl;

namespace GXX.Core.Util;

/// <summary>
/// Delphi Classes.TStringList 1:1 语义复刻：
/// Strings + Objects 双列、Sorted、CaseSensitive、Exchange/InsertObject/Delete、LoadFrom/SaveTo（GBK）。
/// </summary>
public class TStringList
{
    private readonly List<string> _strings = new();
    private readonly List<object> _objects = new();
    private bool _sorted;
    private bool _caseSensitive;

    public int Count => _strings.Count;

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

    public void LoadFromFile(string fileName)
    {
        Clear();
        if (File.Exists(fileName))
        {
            string content = File.ReadAllText(fileName, EncodingInit.GBK);
            var lines = content.Replace("\r\n", "\n").Split('\n');
            // Delphi SetTextStr 语义：文件以换行结尾时最后一个空串不构成新行
            if (lines.Length > 0 && lines[^1].Length == 0 && content.EndsWith("\n"))
                lines = lines[..^1];
            foreach (var line in lines)
                Add(line);
        }
    }

    public void SaveToFile(string fileName)
    {
        var sb = new StringBuilder();
        foreach (var s in _strings) sb.Append(s).Append("\r\n");
        File.WriteAllText(fileName, sb.ToString(), EncodingInit.GBK);
    }

    public System.Collections.Generic.IEnumerable<string> AsEnumerable() => _strings;
}

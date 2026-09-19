using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GXX.Core;

namespace GXX.M2Server.Plugins;

// =====================================================================================
// PluginInterface.pas / PluginImplement.pas 的 ABI 运行时支撑（挂接 PluginInterfaceHost）。
//
// 原文里 `PAnsiChar` 就是 `AnsiString` 的裸指针，GNU 语义要点：
//   1) `Dest: PAnsiChar` 是"调用方预分配的 GBK 缓冲"，不是 C# string；本层一律用 byte[]。
//   2) `var DestLen: DWORD` 的**入参是缓冲容量**，出参是"实际字符串字节数（不含结尾 0）"。
//   3) 原文判定为 `if (Dest <> nil) and (DestLen > Length(S)) then ... Result := True;`，
//      即**必须留出 1 字节给结尾 0**，且写入成功才把 Result 置 True；无论如何 DestLen 都会被改写。
//      照抄其边界：DestLen == Length(S) 时**不写**（不满足 `>`）。
//   4) `TList.GetItem` 等返回 `Pointer`：托管侧用 IntPtr 承载，句柄表保证往返一致。
//
// 托管侧不使用 free/alloc 语义：GCHandle 与 AllocHGlobal 都在本运行时内成对管理。
// =====================================================================================

/// <summary>原文 `_TList` / `TList` 的托管承载（接缝：待 Classes 的 C# 对应物移植后接入）。</summary>
public sealed class TListHandle : IListHandle
{
    private readonly List<IntPtr> _items = new();

    public IReadOnlyList<IntPtr> Items => _items;

    public int Count => _items.Count;
    public void Clear() => _items.Clear();
    public void Add(IntPtr item) => _items.Add(item);
    public void Insert(int index, IntPtr item) => _items.Insert(index, item);
    public void Remove(IntPtr item) => _items.Remove(item);
    public void Delete(int index) => _items.RemoveAt(index);
    public IntPtr GetItem(int index) => _items[index];
    public void SetItem(int index, IntPtr item) => _items[index] = item;
    public int IndexOf(IntPtr item) => _items.IndexOf(item);
    public void Exchange(int index1, int index2) => (_items[index1], _items[index2]) = (_items[index2], _items[index1]);
    public void CopyTo(IListHandle dest)
    {
        dest.Clear();
        foreach (var it in _items) dest.Add(it);
    }
}

/// <summary>
/// 原文 `_TStringList` / `TStringList` 的托管承载（接缝：待 GXX.Core.Util.TStringList 接入）。
/// 文本行按 Delphi TStringList 语义：以 #13#10 连接。
/// </summary>
public sealed class TStringListHandle : IStringListHandle
{
    private readonly List<string> _lines = new();
    private readonly List<object?> _objects = new();

    public bool CaseSensitive { get; set; }
    public bool Sorted { get; set; }
    public bool Duplicates { get; set; }
    public int Count => _lines.Count;

    public string Text
    {
        get => string.Join("\r\n", _lines);
        set
        {
            var text = value ?? string.Empty;
            _lines.Clear();
            _objects.Clear();
            if (text.Length == 0) return;
            foreach (var line in text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
            {
                _lines.Add(line);
                _objects.Add(null);
            }
        }
    }

    public void Add(string s) { _lines.Add(s); _objects.Add(null); }
    public void AddObject(string s, object? aObject) { _lines.Add(s); _objects.Add(aObject); }
    public void Insert(int index, string s) { _lines.Insert(index, s); _objects.Insert(index, null); }
    public void InsertObject(int index, string s, object? aObject) { _lines.Insert(index, s); _objects.Insert(index, aObject); }
    public void Remove(string s)
    {
        var i = _lines.IndexOf(s);
        if (i >= 0) { _lines.RemoveAt(i); _objects.RemoveAt(i); }
    }
    public void Delete(int index) { _lines.RemoveAt(index); _objects.RemoveAt(index); }
    public string GetItem(int index) => _lines[index];
    public void SetItem(int index, string s) => _lines[index] = s;
    public object? GetObject(int index) => _objects[index];
    public void SetObject(int index, object? aObject) => _objects[index] = aObject;
    public int IndexOf(string s) => _lines.IndexOf(s);
    public int IndexOfObject(object? aObject) => _objects.IndexOf(aObject!);
    public bool Find(string s, ref int index) => (index = _lines.IndexOf(s)) >= 0;
    public void Exchange(int index1, int index2)
    {
        (_lines[index1], _lines[index2]) = (_lines[index2], _lines[index1]);
        (_objects[index1], _objects[index2]) = (_objects[index2], _objects[index1]);
    }
    public void LoadFromFile(string fileName)
    {
        Text = System.IO.File.ReadAllText(fileName, EncodingInit.GBK);
    }
    public void SaveToFile(string fileName)
    {
        System.IO.File.WriteAllText(fileName, Text, EncodingInit.GBK);
    }
    public void CopyTo(IStringListHandle dest)
    {
        dest.Text = Text;
    }
}

/// <summary>原文 `_TMemoryStream` / `TMemoryStream` 的托管承载。</summary>
public sealed class TMemoryStreamHandle : IMemoryStreamHandle
{
    private readonly System.IO.MemoryStream _stream = new();

    public long Size => _stream.Length;
    public void SetSize(int newSize) => _stream.SetLength(newSize);
    public void Clear() => _stream.SetLength(0);

    public int Read(byte[] buffer, int count)
    {
        if (buffer is null || count <= 0) return 0;
        return _stream.Read(buffer, 0, Math.Min(count, buffer.Length));
    }

    public int Write(byte[] buffer, int count)
    {
        if (buffer is null || count <= 0) return 0;
        _stream.Write(buffer, 0, Math.Min(count, buffer.Length));
        return Math.Min(count, buffer.Length);
    }

    /// <summary>原文 `TMemoryStream.Seek(Offset, Origin)`：0=开头 1=当前位置 2=末尾；返回新位置。</summary>
    public int Seek(int offset, ushort origin)
    {
        long origin2 = origin switch { 0 => 0, 1 => _stream.Position, 2 => _stream.Length, _ => 0 };
        _stream.Position = origin2 + offset;
        return (int)_stream.Position;
    }

    public IntPtr Memory { get; private set; }
    public long Position { get => _stream.Position; set => _stream.Position = value; }
    public void LoadFromFile(string fileName)
    {
        var bytes = System.IO.File.ReadAllBytes(fileName);
        _stream.SetLength(0);
        _stream.Write(bytes, 0, bytes.Length);
        _stream.Position = 0;
    }
    public void SaveToFile(string fileName) => System.IO.File.WriteAllBytes(fileName, _stream.ToArray());
}

/// <summary>
/// 原文 `_TIniFile` / `TIniFile` 的托管承载（接缝：待 GXX.Core.Util.FastIniFile 接入）。
/// 直接落盘到原 INI 文本，行为对齐 VCL TIniFile 的读写/存在性判定。
/// </summary>
public sealed class TIniFileHandle : IIniFileHandle
{
    private readonly string _fileName;
    private readonly Dictionary<string, Dictionary<string, string>> _sections = new(StringComparer.OrdinalIgnoreCase);

    public TIniFileHandle(string fileName)
    {
        _fileName = fileName;
        Load();
    }

    private void Load()
    {
        if (!System.IO.File.Exists(_fileName)) return;
        string? current = null;
        foreach (var raw in System.IO.File.ReadAllLines(_fileName, EncodingInit.GBK))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line[0] is ';' or '#') continue;
            if (line[0] == '[' && line[^1] == ']')
            {
                current = line.Substring(1, line.Length - 2).Trim();
                if (!_sections.ContainsKey(current)) _sections[current] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                continue;
            }
            var eq = line.IndexOf('=');
            if (eq <= 0 || current is null) continue;
            _sections[current][line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
        }
    }

    /// <summary>原文 `TIniFile.WriteXXX` 每次立即落盘（VCL 语义）。</summary>
    private void Save()
    {
        var sb = new System.Text.StringBuilder();
        foreach (var kv in _sections)
        {
            sb.Append('[').Append(kv.Key).Append(']').Append("\r\n");
            foreach (var p in kv.Value) sb.Append(p.Key).Append('=').Append(p.Value).Append("\r\n");
        }
        System.IO.File.WriteAllText(_fileName, sb.ToString(), EncodingInit.GBK);
    }

    public bool SectionExists(string section) => _sections.ContainsKey(section);

    public bool ValueExists(string section, string ident)
        => _sections.TryGetValue(section, out var s) && s.ContainsKey(ident);

    public bool ReadString(string section, string ident, string @default, byte[]? dest, ref uint destLen)
    {
        var value = ReadString(section, ident, @default);
        return PluginHostText.WriteText(value, dest, ref destLen, requireRoomForTerminator: true);
    }

    public string ReadString(string section, string ident, string @default)
        => _sections.TryGetValue(section, out var s) && s.TryGetValue(ident, out var v) ? v : @default;

    public void WriteString(string section, string ident, string value)
    {
        if (!_sections.TryGetValue(section, out var s)) _sections[section] = s = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        s[ident] = value;
        Save();
    }

    public int ReadInteger(string section, string ident, int @default)
        => _sections.TryGetValue(section, out var s) && s.TryGetValue(ident, out var v) && int.TryParse(v, out var r) ? r : @default;

    public void WriteInteger(string section, string ident, int value) => WriteString(section, ident, value.ToString());

    /// <summary>原文 `TIniFile.ReadBool`：只有 '1'/'True' 之类为真（VCL 语义按字符串比较）。</summary>
    public int ReadBool(string section, string ident, int @default)
    {
        var def = @default != 0 ? "1" : "0";
        var v = _sections.TryGetValue(section, out var s) && s.TryGetValue(ident, out var raw) ? raw : def;
        if (v.Length == 0) return @default;
        return v[0] is '1' or 'T' or 't' or 'Y' or 'y' ? 1 : 0;
    }

    public void WriteBool(string section, string ident, int value) => WriteString(section, ident, value != 0 ? "1" : "0");
}

/// <summary>
/// `Dest: PAnsiChar; var DestLen: DWORD` 的公共实现，逐字对齐原文的 5 处同构代码：
/// <code>
///   Result := False;
///   S := ...;
///   if (Dest &lt;&gt; nil) and (DestLen &gt; Length(S)) then begin Move(...); FillChar(结尾 0); Result := True; end;
///   DestLen := Length(S);
/// </code>
/// </summary>
public static class PluginHostText
{
    /// <summary>把 GBK 文本写入调用方缓冲，返回原文 `Result`（是否真正写入）。</summary>
    public static bool WriteText(string? s, byte[]? dest, ref uint destLen, bool requireRoomForTerminator = true)
    {
        s ??= string.Empty;
        var bytes = EncodingInit.GBK.GetBytes(s);
        var ok = false;
        if (dest is not null)
        {
            var enough = requireRoomForTerminator ? destLen > (uint)bytes.Length : destLen >= (uint)bytes.Length;
            if (enough)
            {
                var n = Math.Min(bytes.Length, dest.Length);
                Array.Copy(bytes, 0, dest, 0, n);
                if (n < dest.Length) dest[n] = 0;
                ok = true;
            }
        }
        destLen = (uint)bytes.Length;
        return ok;
    }

    /// <summary>读取 `PAnsiChar`（可能为 null，或以 0 结尾的 GBK 字节）。</summary>
    public static string ReadAnsi(byte[]? src)
    {
        if (src is null || src.Length == 0) return string.Empty;
        var n = Array.IndexOf<byte>(src, 0);
        if (n < 0) n = src.Length;
        return n == 0 ? string.Empty : EncodingInit.GBK.GetString(src, 0, n);
    }

    /// <summary>读取"定长 + 显式长度"的 GBK 缓冲（原文 `SetLength(S, SrcLen); Move(Src^, S[1], SrcLen);`）。</summary>
    public static string ReadAnsiLen(byte[]? src, uint srcLen)
    {
        if (src is null || srcLen == 0) return string.Empty;
        var n = (int)Math.Min(srcLen, (uint)src.Length);
        return EncodingInit.GBK.GetString(src, 0, n);
    }
}

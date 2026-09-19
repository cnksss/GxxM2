using System;
using System.Globalization;
using System.IO;
using System.Text;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.RunGate;

/// <summary>
/// IniFilesEx.pas 1:1 转换（Source\RunGate\IniFilesEx.pas，734 行）。
/// 相对 Delphi 自带 IniFiles 的差异（本单元原创点，全部保留）：
///  1) 全部类名带 Ex 后缀；
///  2) TStringHashEx / THashedStringListEx 为自研哈希索引（支持 CaseSensitive）；
///  3) TMemIniFileEx 全部操作在内存镜像上，TIniFileEx 仅在析构（Dispose）时 UpdateFile 落盘。
/// 保真要点：
///  - 节容器 FSections 为 TStringList（字符串 + 对应节行对象）；
///  - 节内容为 THashedStringListEx（Name=Value 行），键顺序即写入顺序；
///    WriteString 对已存在键执行覆盖（IndexOfName 命中即替换该行），
///    而 LoadValues 阶段的重复键按原样共存且 ReadString 只命中"最后写入者"（哈希链头语义）；
///  - TStringHashEx.Find 返回"槽位引用"（PPHashItem 语义），删除时改写槽位 —— 原文刻意行为；
///  - 哈希取模、旋转哈希、CRC 无关的字符串哈希算法逐行复刻。
/// 未移植依赖：TMemoryStreamEx（MemoryStreamEx.pas 尚未移植）—— 见文件末尾接缝定义。
/// </summary>
public class EIniFileExceptionEx : Exception
{
    public EIniFileExceptionEx(string message) : base(message) { }
}

/// <summary>SysUtils.EConvertError（原 IniFilesEx.pas:189 `on EConvertError do`）。</summary>
public class EConvertError : Exception
{
    public EConvertError(string message) : base(message) { }
}

/// <summary>TCustomIniFileEx：INI 抽象基类（原 IniFilesEx.pas:21-51、138-347）。</summary>
public abstract class TCustomIniFileEx
{
    private string FFileName;

    protected TCustomIniFileEx(string fileName)
    {
        FFileName = fileName;   // 原 IniFilesEx.pas:138-141
    }

    public string FileName
    {
        get => FFileName;
        protected set => FFileName = value;   // 原 IniFilesEx.pas:719 直接写 FFileName
    }

    public bool SectionExists(string section)
    {
        // 原 IniFilesEx.pas:143-154
        var s = new GXX.Core.Util.TStringList();
        ReadSection(section, s);
        return s.Count > 0;
    }

    public abstract string ReadString(string section, string ident, string def);
    public abstract void WriteString(string section, string ident, string value);

    public int ReadInteger(string section, string ident, int def)
    {
        // 原 IniFilesEx.pas:156-166：'0x'/'0X' 前缀 → '$' 前缀（Delphi StrToIntDef 认 $ 十六进制）
        string intStr = ReadString(section, ident, "");
        if (intStr.Length > 2 && intStr[0] == '0' && (intStr[1] == 'X' || intStr[1] == 'x'))
            intStr = "$" + DelphiRTL.Copy(intStr, 3, DelphiRTL.MaxInt);
        return RtlIni.StrToIntDef(intStr, def);
    }

    public void WriteInteger(string section, string ident, int value)
        => WriteString(section, ident, DelphiRTL.IntToStr(value));   // 原 IniFilesEx.pas:168-171

    /// <summary>Delphi Boolean 为 1 字节，原文以 Ord(Default)/ReadInteger 读写，C# 侧保持 byte 与 0 比较。</summary>
    public byte ReadBool(string section, string ident, byte def)
        => (byte)(ReadInteger(section, ident, (int)def) != 0 ? 1 : 0);    // 原 IniFilesEx.pas:173-177

    public void WriteBool(string section, string ident, byte value)
    {
        // 原 IniFilesEx.pas:267-272：const Values: array[Boolean] of string = ('0', '1')
        WriteString(section, ident, value != 0 ? "1" : "0");
    }

    public DateTime ReadDate(string section, string name, DateTime def)
    {
        // 原 IniFilesEx.pas:179-194：解析失败忽略 EConvertError，保留 Default
        string dateStr = ReadString(section, name, "");
        if (dateStr == "") return def;
        return RtlIni.TryStrToDate(dateStr, out var v) ? v : def;
    }

    public DateTime ReadDateTime(string section, string name, DateTime def)
    {
        // 原 IniFilesEx.pas:196-211
        string dateStr = ReadString(section, name, "");
        if (dateStr == "") return def;
        return RtlIni.TryStrToDateTime(dateStr, out var v) ? v : def;
    }

    public double ReadFloat(string section, string name, double def)
    {
        // 原 IniFilesEx.pas:213-228
        string floatStr = ReadString(section, name, "");
        if (floatStr == "") return def;
        return DelphiRTL.StrToFloatDef(floatStr, def);
    }

    public DateTime ReadTime(string section, string name, DateTime def)
    {
        // 原 IniFilesEx.pas:230-245
        string timeStr = ReadString(section, name, "");
        if (timeStr == "") return def;
        return RtlIni.TryStrToTime(timeStr, out var v) ? v : def;
    }

    public void WriteDate(string section, string name, DateTime value)
        => WriteString(section, name, RtlIni.DateToStr(value));      // 原 IniFilesEx.pas:247-250

    public void WriteDateTime(string section, string name, DateTime value)
        => WriteString(section, name, RtlIni.DateTimeToStr(value));  // 原 IniFilesEx.pas:252-255

    public void WriteFloat(string section, string name, double value)
        => WriteString(section, name, DelphiRTL.FloatToStr(value));  // 原 IniFilesEx.pas:257-260

    public void WriteTime(string section, string name, DateTime value)
        => WriteString(section, name, RtlIni.TimeToStr(value));      // 原 IniFilesEx.pas:262-265

    public bool ValueExists(string section, string ident)
    {
        // 原 IniFilesEx.pas:274-285
        var s = new GXX.Core.Util.TStringList();
        ReadSection(section, s);
        return s.IndexOf(ident) > -1;
    }

    /// <summary>ReadBinaryStream：HEX 文本 → 字节流（原 IniFilesEx.pas:287-317）。
    /// 原文中 Value 为 TMemoryStream 时 Stream := Value（同一对象），故 `Value &lt;&gt; Stream` 恒假，
    /// 不会二次 CopyFrom —— 此处以同一对象语义复刻，返回值 Stream.Size - Pos。</summary>
    public int ReadBinaryStream(string section, string name, TMemoryStreamEx value)
    {
        string text = ReadString(section, name, "");
        if (text == "")
            return 0;
        int pos = (int)value.Position;
        value.SetSize(value.Size + text.Length / 2);
        RtlIni.HexToBin(text, value.Buffer, pos, text.Length / 2);
        value.Position = pos;
        return (int)(value.Size - pos);
    }

    /// <summary>WriteBinaryStream：字节流 → HEX 文本（原 IniFilesEx.pas:319-347）。
    /// 原文先 SetLength(Text, (Size-Position)*2) 再 BinToHex，长度一致故无截断。</summary>
    public void WriteBinaryStream(string section, string name, TMemoryStreamEx value)
    {
        long avail = value.Size - value.Position;
        if (avail <= 0)
        {
            WriteString(section, name, "");
            return;
        }
        string text = RtlIni.BinToHex(value.Buffer, (int)value.Position, (int)avail);
        WriteString(section, name, text);
    }

    public abstract void ReadSection(string section, GXX.Core.Util.TStringList strings);
    public abstract void ReadSections(GXX.Core.Util.TStringList strings);
    public abstract void ReadSectionValues(string section, GXX.Core.Util.TStringList strings);
    public abstract void EraseSection(string section);
    public abstract void DeleteKey(string section, string ident);
    public abstract void UpdateFile();
}

/// <summary>
/// TStringHashEx：开放链式哈希（原 IniFilesEx.pas:57-77、351-456）。
/// Buckets 存"链头槽位"，Find 返回该槽位（PPHashItem 语义）—— 删除时可直接改写槽位。
/// </summary>
public class TStringHashEx
{
    /// <summary>PHashItem（THashItem 指针）。</summary>
    public sealed class PHashItem
    {
        public PHashItem Next;
        public string Key;
        public int Value;
    }

    /// <summary>PPHashItem：指向槽位的引用（*PPHashItem == PHashItem）。</summary>
    public sealed class PPHashItem
    {
        public PPHashItem Prev;       // 上一级槽位（用于把新链头写回）
        public PHashItem Slot;        // Result^
    }

    private readonly PHashItem[] Buckets;

    public TStringHashEx(uint size = 256)
    {
        // 原 IniFilesEx.pas:382-386：SetLength(Buckets, Size)
        Buckets = new PHashItem[size];
    }

    public int BucketCount => Buckets.Length;

    /// <summary>测试可见：直接读取某桶链头（不改动行为）。</summary>
    public PHashItem BucketAt(int index) => Buckets[index];

    public void Add(string key, int value)
    {
        // 原 IniFilesEx.pas:351-362：新项插在链头（LIFO → 索引优先命中最后写入者）
        int hash = (int)(HashOf(key) % (uint)Buckets.Length);
        var bucket = new PHashItem { Key = key, Value = value, Next = Buckets[hash] };
        Buckets[hash] = bucket;
    }

    public void Clear()
    {
        // 原 IniFilesEx.pas:364-380
        for (int i = 0; i < Buckets.Length; i++)
            Buckets[i] = null;
    }

    public PPHashItem Find(string key)
    {
        // 原 IniFilesEx.pas:394-407
        int hash = (int)(HashOf(key) % (uint)Buckets.Length);
        var result = new PPHashItem { Prev = null, Slot = Buckets[hash] };
        while (result.Slot != null)
        {
            if (result.Slot.Key == key)
                return result;
            result = new PPHashItem { Prev = result, Slot = result.Slot.Next };
        }
        return result;   // 未找到：Slot == nil（原文返回链尾槽位）
    }

    public uint HashOf(string key)
    {
        // 原 IniFilesEx.pas:409-417：Result := ((Result shl 2) or (Result shr 30)) xor Ord(Key[I])
        uint result = 0;
        for (int i = 1; i <= key.Length; i++)
            result = ((result << 2) | (result >> (sizeof(uint) * 8 - 2))) ^ key[i - 1];
        return result;
    }

    public bool Modify(string key, int value)
    {
        // 原 IniFilesEx.pas:419-431
        var p = Find(key).Slot;
        if (p != null) { p.Value = value; return true; }
        return false;
    }

    public void Remove(string key)
    {
        // 原 IniFilesEx.pas:433-445：Prev^ := P^.Next（改写槽位，可能是 Buckets[Hash] 本身）
        var prev = Find(key);
        var p = prev.Slot;
        if (p == null) return;
        if (prev.Prev == null)
            Buckets[(int)(HashOf(key) % (uint)Buckets.Length)] = p.Next;
        else
            prev.Prev.Slot.Next = p.Next;
    }

    public int ValueOf(string key)
    {
        // 原 IniFilesEx.pas:447-456：未找到返回 -1
        var p = Find(key).Slot;
        return p != null ? p.Value : -1;
    }
}

/// <summary>
/// THashedStringListEx：带哈希索引的 TStringList（原 IniFilesEx.pas:81-95、460-536）。
/// 索引惰性重建；大小写敏感性由 CaseSensitive 决定。
/// </summary>
public class THashedStringListEx : TStringList
{
    private TStringHashEx FValueHash;
    private TStringHashEx FNameHash;
    private bool FValueHashValid;
    private bool FNameHashValid;

    /// <summary>NameValueSeparator（Delphi TStrings 默认 '='）。</summary>
    public char NameValueSeparator { get; set; } = '=';

    /// <summary>Changed：置脏（原 IniFilesEx.pas:460-465；Delphi 中由 TStrings 各方法自动调用，
    /// 托管侧由本类的 Add/Insert/Delete/Clear/索引器显式调用）。</summary>
    public new void Changed()
    {
        FValueHashValid = false;
        FNameHashValid = false;
    }

    public new int Add(string s)
    {
        int r = base.Add(s);
        Changed();
        return r;
    }

    public new void Insert(int index, string s)
    {
        base.Insert(index, s);
        Changed();
    }

    public new void Delete(int index)
    {
        base.Delete(index);
        Changed();
    }

    public new void Clear()
    {
        base.Clear();
        Changed();
    }

    public new string this[int index]
    {
        get => base[index];
        set { base[index] = value; Changed(); }
    }

    /// <summary>IndexOf：走值哈希（原 IniFilesEx.pas:474-481）。</summary>
    public new int IndexOf(string s)
    {
        UpdateValueHash();
        return CaseSensitive ? FValueHash.ValueOf(s) : FValueHash.ValueOf(RtlAnsi.AnsiUpperCase(s));
    }

    /// <summary>IndexOfName：走名字哈希（原 IniFilesEx.pas:483-490）。</summary>
    public new int IndexOfName(string name)
    {
        UpdateNameHash();
        return CaseSensitive ? FNameHash.ValueOf(name) : FNameHash.ValueOf(RtlAnsi.AnsiUpperCase(name));
    }

    /// <summary>Names[]：取 NameValueSeparator 之前的键名（TStrings.Names 语义；ReadSection 依赖）。</summary>
    public string Names(int index)
    {
        string s = base[index];
        int p = s.IndexOf(NameValueSeparator);
        return p > 0 ? s.Substring(0, p) : "";
    }

    /// <summary>测试可见：哈希索引是否已建立（断言惰性重建行为）。</summary>
    public bool ValueHashValidForTest => FValueHashValid;
    public bool NameHashValidForTest => FNameHashValid;

    private void UpdateNameHash()
    {
        // 原 IniFilesEx.pas:492-518
        if (FNameHashValid) return;
        if (FNameHash == null) FNameHash = new TStringHashEx();
        else FNameHash.Clear();
        for (int i = 0; i < Count; i++)
        {
            string key = base[i];
            int p = RtlAnsi.AnsiPos(NameValueSeparator, key);
            if (p != 0)
            {
                key = CaseSensitive
                    ? DelphiRTL.Copy(key, 1, p - 1)
                    : RtlAnsi.AnsiUpperCase(DelphiRTL.Copy(key, 1, p - 1));
                FNameHash.Add(key, i);
            }
        }
        FNameHashValid = true;
    }

    private void UpdateValueHash()
    {
        // 原 IniFilesEx.pas:520-536
        if (FValueHashValid) return;
        if (FValueHash == null) FValueHash = new TStringHashEx();
        else FValueHash.Clear();
        for (int i = 0; i < Count; i++)
            FValueHash.Add(CaseSensitive ? base[i] : RtlAnsi.AnsiUpperCase(base[i]), i);
        FValueHashValid = true;
    }
}

/// <summary>TMemIniFileEx：全内存 INI（原 IniFilesEx.pas:101-124、540-801）。</summary>
public class TMemIniFileEx : TCustomIniFileEx
{
    private readonly TStringList FSections;

    public TMemIniFileEx(string fileName) : base(fileName)
    {
        // 原 IniFilesEx.pas:540-548
        FSections = new TStringList();
        LoadValues();
    }

    /// <summary>CaseSensitive（原 IniFilesEx.pas:613-616、724-739）。</summary>
    public bool CaseSensitive
    {
        get => FSections.CaseSensitive;
        set
        {
            if (value != FSections.CaseSensitive)
            {
                FSections.CaseSensitive = value;
                for (int i = 0; i < FSections.Count; i++)
                {
                    // 原 IniFilesEx.pas:732-736：with THashedStringListEx(FSections.Objects[I]) do
                    //   begin CaseSensitive := Value; Changed; end;
                    var sl = (THashedStringListEx)FSections.GetObject(i);
                    sl.CaseSensitive = value;
                    sl.Changed();
                }
                // 原 IniFilesEx.pas:737：THashedStringListEx(FSections).Changed（节名索引置脏）
                FSectionsChanged();
            }
        }
    }

    /// <summary>对应 THashedStringListEx(FSections).Changed（节名索引失效）。
    /// 原文 FSections 是 THashedStringListEx，本移植中节名查找由 TStringList.IndexOf 线性完成，
    /// 无索引可置脏，保留空实现以标记原文调用点（原 IniFilesEx.pas:737）。</summary>
    private static void FSectionsChanged() { }

    public void Clear()
    {
        // 原 IniFilesEx.pas:577-584（托管侧对象由 GC 回收）
        FSections.Clear();
    }

    private THashedStringListEx AddSection(string section)
    {
        // 原 IniFilesEx.pas:558-575
        int index = FSections.IndexOf(section);
        if (index >= 0)
            return (THashedStringListEx)FSections.GetObject(index);
        var result = new THashedStringListEx { CaseSensitive = CaseSensitive };
        FSections.AddObject(section, result);
        return result;
    }

    public override void DeleteKey(string section, string ident)
    {
        // 原 IniFilesEx.pas:586-599
        int i = FSections.IndexOf(section);
        if (i >= 0)
        {
            var strings = (THashedStringListEx)FSections.GetObject(i);
            int j = strings.IndexOfName(ident);
            if (j >= 0) strings.Delete(j);
        }
    }

    public override void EraseSection(string section)
    {
        // 原 IniFilesEx.pas:601-611
        int i = FSections.IndexOf(section);
        if (i >= 0) FSections.Delete(i);
    }

    /// <summary>GetStrings：内存镜像 → [Section] 行文本（原 IniFilesEx.pas:618-635）。</summary>
    public void GetStrings(GXX.Core.Util.TStringList list)
    {
        for (int i = 0; i < FSections.Count; i++)
        {
            list.Add("[" + FSections[i] + "]");
            var strings = (THashedStringListEx)FSections.GetObject(i);
            for (int j = 0; j < strings.Count; j++) list.Add(strings[j]);
            list.Add("");
        }
    }

    private void LoadValues()
    {
        // 原 IniFilesEx.pas:637-653
        if (FileName != "" && File.Exists(FileName))
        {
            var list = new GXX.Core.Util.TStringList();
            list.LoadFromFile(FileName);
            SetStrings(list);
        }
        else
            Clear();
    }

    public override void ReadSection(string section, GXX.Core.Util.TStringList strings)
    {
        // 原 IniFilesEx.pas:655-674：只取键名（SectionStrings.Names[J]）
        strings.Clear();
        int i = FSections.IndexOf(section);
        if (i >= 0)
        {
            var sectionStrings = (THashedStringListEx)FSections.GetObject(i);
            for (int j = 0; j < sectionStrings.Count; j++)
                strings.Add(sectionStrings.Names(j));
        }
    }

    public override void ReadSections(GXX.Core.Util.TStringList strings)
    {
        // 原 IniFilesEx.pas:676-679：Strings.Assign(FSections)（节名按顺序拷贝，丢弃对象）
        strings.Clear();
        for (int i = 0; i < FSections.Count; i++)
            strings.Add(FSections[i]);
    }

    public override void ReadSectionValues(string section, GXX.Core.Util.TStringList strings)
    {
        // 原 IniFilesEx.pas:681-695：整行（Name=Value）按顺序拷贝
        strings.Clear();
        int i = FSections.IndexOf(section);
        if (i >= 0)
        {
            var sectionStrings = (THashedStringListEx)FSections.GetObject(i);
            for (int j = 0; j < sectionStrings.Count; j++)
                strings.Add(sectionStrings[j]);
        }
    }

    public override string ReadString(string section, string ident, string def)
    {
        // 原 IniFilesEx.pas:697-715：命中即 Copy(Strings[I], Length(Ident)+2, Maxint)
        int i = FSections.IndexOf(section);
        if (i >= 0)
        {
            var strings = (THashedStringListEx)FSections.GetObject(i);
            i = strings.IndexOfName(ident);
            if (i >= 0)
                return DelphiRTL.Copy(strings[i], ident.Length + 2, DelphiRTL.MaxInt);
        }
        return def;
    }

    public void Rename(string fileName, bool reload)
    {
        // 原 IniFilesEx.pas:717-722
        FileName = fileName;
        if (reload) LoadValues();
    }

    /// <summary>SetStrings：文本行 → 内存镜像（原 IniFilesEx.pas:741-769）。</summary>
    public void SetStrings(GXX.Core.Util.TStringList list)
    {
        Clear();
        THashedStringListEx strings = null;
        for (int i = 0; i < list.Count; i++)
        {
            string s = DelphiRTL.Trim(list[i]);
            if (s != "" && s[0] != ';')
            {
                if (s[0] == '[' && s[s.Length - 1] == ']')
                {
                    // Delete(S, 1, 1) 然后 SetLength(S, Length(S)-1)
                    s = s.Substring(1);
                    if (s.Length > 0) s = s.Substring(0, s.Length - 1);
                    strings = AddSection(DelphiRTL.Trim(s));
                }
                else if (strings != null)
                {
                    int j = DelphiRTL.Pos("=", s);
                    if (j > 0)   // 原注释：remove spaces before and after '='
                        strings.Add(DelphiRTL.Trim(DelphiRTL.Copy(s, 1, j - 1)) + "=" +
                                    DelphiRTL.Trim(DelphiRTL.Copy(s, j + 1, DelphiRTL.MaxInt)));
                    else
                        strings.Add(s);
                }
            }
        }
    }

    public override void UpdateFile()
    {
        // 原 IniFilesEx.pas:771-782：List.SaveToFile(FFileName)（GBK 落盘）
        var list = new GXX.Core.Util.TStringList();
        GetStrings(list);
        if (FileName != "") list.SaveToFile(FileName);
    }

    public override void WriteString(string section, string ident, string value)
    {
        // 原 IniFilesEx.pas:784-801
        int i = FSections.IndexOf(section);
        THashedStringListEx strings = i >= 0
            ? (THashedStringListEx)FSections.GetObject(i)
            : AddSection(section);
        string s = ident + "=" + value;
        i = strings.IndexOfName(ident);
        if (i >= 0) strings[i] = s;
        else strings.Add(s);
    }
}

/// <summary>
/// TIniFileEx：析构时自动 UpdateFile 落盘（原 IniFilesEx.pas:126-129、803-807）。
/// 托管侧对应 IDisposable.Dispose。
/// </summary>
public class TIniFileEx : TMemIniFileEx, IDisposable
{
    public TIniFileEx(string fileName) : base(fileName) { }

    public void Dispose()
    {
        // 原 IniFilesEx.pas:803-807：UpdateFile; inherited Destroy;
        UpdateFile();
    }
}

/// <summary>
/// 接缝：待 MemoryStreamEx.pas 移植后接入。
/// 原文 ReadBinaryStream/WriteBinaryStream 使用 Classes.TMemoryStream 的
/// Memory/Size/Position/SetSize/Clear 语义；此处定义最小可用替身，接口面与 Delphi 一致。
/// </summary>
public class TMemoryStreamEx
{
    private byte[] _buf = Array.Empty<byte>();
    private long _size;
    private long _position;

    public byte[] Buffer => _buf;
    public long Size => _size;

    public long Position
    {
        get => _position;
        set => _position = value;
    }

    /// <summary>Delphi TMemoryStream.SetSize。</summary>
    public void SetSize(long newSize)
    {
        if (newSize < 0) newSize = 0;
        if (newSize > _buf.Length)
        {
            int cap = _buf.Length == 0 ? 64 : _buf.Length;
            while (cap < newSize) cap *= 2;
            Array.Resize(ref _buf, cap);
        }
        if (newSize > _size) Array.Clear(_buf, (int)_size, (int)(newSize - _size));
        _size = newSize;
        if (_position > _size) _position = _size;
    }

    /// <summary>Delphi TMemoryStream.Clear。</summary>
    public void Clear()
    {
        _size = 0;
        _position = 0;
    }

    public void LoadFromBytes(byte[] data)
    {
        Clear();
        SetSize(data.Length);
        Array.Copy(data, _buf, data.Length);
        _position = 0;
    }

    public byte[] ToArray()
    {
        var r = new byte[_size];
        Array.Copy(_buf, r, (int)_size);
        return r;
    }
}

/// <summary>
/// 本单元用到的 Delphi RTL 语义垫片（DelphiRTL 未覆盖部分在此逐字复刻；
/// 不改动 GXX.Core 既有文件）。
/// </summary>
internal static class RtlIni
{
    /// <summary>
    /// Delphi SysUtils.StrToIntDef：支持 '$' 十六进制前缀（原 IniFilesEx.pas:165 依赖此语义，
    /// 因为 ReadInteger 会把 '0x' 前缀改写为 '$'）。GXX.Core.Rtl.DelphiRTL.StrToIntDef 不认 '$'，
    /// 故在此补全（Delphi RTL 语义）。
    /// </summary>
    public static int StrToIntDef(string s, int def)
    {
        s = DelphiRTL.Trim(s);
        if (s == "") return def;
        bool neg = false;
        int i = 0;
        if (s[0] == '+' || s[0] == '-') { neg = s[0] == '-'; i = 1; }
        string body = s.Substring(i);
        long v;
        if (body.StartsWith("$", StringComparison.Ordinal))
        {
            if (!TryParseHex(body.Substring(1), out v)) return def;
        }
        else if (body.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            if (!TryParseHex(body.Substring(2), out v)) return def;
        }
        else if (!long.TryParse(body, NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
            return def;
        if (neg) v = -v;
        if (v < int.MinValue || v > int.MaxValue) return def;
        return (int)v;
    }

    private static bool TryParseHex(string hex, out long value)
    {
        value = 0;
        if (hex == "") return false;
        foreach (char c in hex)
        {
            int d;
            if (c >= '0' && c <= '9') d = c - '0';
            else if (c >= 'A' && c <= 'F') d = c - 'A' + 10;
            else if (c >= 'a' && c <= 'f') d = c - 'a' + 10;
            else return false;
            value = (value << 4) | (uint)d;
        }
        return true;
    }

    /// <summary>Delphi Classes.HexToBin：HEX 文本 → 目标字节缓冲（原 IniFilesEx.pas:305）。</summary>
    public static void HexToBin(string text, byte[] buf, int offset, int count)
    {
        for (int i = 0; i < count; i++)
        {
            string pair = DelphiRTL.Copy(text, i * 2 + 1, 2);
            buf[offset + i] = (byte)ParseHexPair(pair);
        }
    }

    /// <summary>Delphi Classes.BinToHex：字节缓冲 → HEX 文本（原 IniFilesEx.pas:339）。</summary>
    public static string BinToHex(byte[] buf, int offset, int count)
    {
        var sb = new StringBuilder(count * 2);
        for (int i = 0; i < count; i++)
            sb.Append(buf[offset + i].ToString("X2", CultureInfo.InvariantCulture));
        return sb.ToString();
    }

    private static int ParseHexPair(string hex)
    {
        int res = 0;
        foreach (char ch in hex)
        {
            if (ch >= '0' && ch <= '9') res = res * 16 + ch - '0';
            else if (ch >= 'A' && ch <= 'F') res = res * 16 + ch - 'A' + 10;
            else if (ch >= 'a' && ch <= 'f') res = res * 16 + ch - 'a' + 10;
            else throw new EConvertError("Invalid hex digit: " + ch);
        }
        return res;
    }

    public static bool TryStrToDate(string s, out DateTime value)
        => DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);

    public static bool TryStrToDateTime(string s, out DateTime value)
        => DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);

    public static bool TryStrToTime(string s, out DateTime value)
        => DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);

    public static string DateToStr(DateTime v) => v.ToString("d", CultureInfo.InvariantCulture);
    public static string DateTimeToStr(DateTime v) => v.ToString("g", CultureInfo.InvariantCulture);
    public static string TimeToStr(DateTime v) => v.ToString("T", CultureInfo.InvariantCulture);
}

/// <summary>Delphi 8 位 RTL 文本函数（AnsiUpperCase / AnsiPos），按 ANSI 大小写语义处理。</summary>
internal static class RtlAnsi
{
    /// <summary>Delphi SysUtils.AnsiUpperCase（不变文化大写；GBK 双字节不参与大小写）。</summary>
    public static string AnsiUpperCase(string s) => s == null ? "" : s.ToUpperInvariant();

    /// <summary>Delphi SysUtils.AnsiPos：1-based，未找到返回 0。</summary>
    public static int AnsiPos(char sub, string s) => s.IndexOf(sub) + 1;
}

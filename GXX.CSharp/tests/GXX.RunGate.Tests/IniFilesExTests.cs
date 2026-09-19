using System;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// IniFilesEx.pas 1:1 移植测试（原 IniFilesEx.pas:1-809）。
/// 重点覆盖：键顺序稳定、大小写敏感、重复键覆盖/共存差异、默认值、
/// ReadInteger 的 '0x'→'$' 改写、HEX 二进制流、UpdateFile 落盘。
/// </summary>
public class IniFilesExTests
{
    // ---------------- TStringHashEx ----------------

    [Fact]
    public void HashOf_RotateLeft2Xor_MatchesOriginalSemantics()
    {
        // 原 IniFilesEx.pas:409-417：Result := ((Result shl 2) or (Result shr 30)) xor Ord(Key[I])
        var h = new TStringHashEx(64);
        Assert.Equal(0u, h.HashOf(""));
        // 'A' = 65：0<<2 | 0>>>30 = 0；0 xor 65 = 65
        Assert.Equal(65u, h.HashOf("A"));
        // 两字符 "AB"：(65<<2)|(65>>30) = 260；260 xor 66 = 326
        Assert.Equal(326u, h.HashOf("AB"));
        // 4 字符触发回绕语义："ABCD"
        uint expected = 0;
        foreach (char c in "ABCD")
            expected = ((expected << 2) | (expected >> 30)) ^ c;
        Assert.Equal(expected, h.HashOf("ABCD"));
    }

    [Fact]
    public void AddModifyRemove_ValueOf_DefaultMinusOne()
    {
        var h = new TStringHashEx(8);
        Assert.Equal(-1, h.ValueOf("nope"));   // 原 447-456：未找到返回 -1

        h.Add("a", 1);
        h.Add("b", 2);
        Assert.Equal(1, h.ValueOf("a"));
        Assert.Equal(2, h.ValueOf("b"));

        Assert.True(h.Modify("a", 11));
        Assert.Equal(11, h.ValueOf("a"));
        Assert.False(h.Modify("zzz", 1));      // 原 419-431：不存在返回 False

        h.Remove("a");
        Assert.Equal(-1, h.ValueOf("a"));
        h.Remove("a");                         // 重复删除不抛（原 433-445 对 nil 无操作）
        Assert.Equal(-1, h.ValueOf("a"));
    }

    [Fact]
    public void Add_SameKeyTwice_LastWriteWins_BecauseChainHead()
    {
        // 原 IniFilesEx.pas:351-362 新项插在链头 → Find 先命中最后写入者
        var h = new TStringHashEx(8);
        h.Add("dup", 1);
        h.Add("dup", 2);
        Assert.Equal(2, h.ValueOf("dup"));
    }

    [Fact]
    public void Find_ReturnsSlotReference_RemoveCanRewriteBucket()
    {
        // 原 394-407 Find 返回 PPHashItem（槽位本身）；Remove 通过 Prev^ := P^.Next 改写
        var h = new TStringHashEx(4);
        h.Add("first", 1);
        var slot = h.Find("first");
        Assert.NotNull(slot.Slot);
        Assert.Same(h.BucketAt((int)(h.HashOf("first") % 4)), slot.Slot);
        h.Remove("first");
        Assert.Null(h.BucketAt((int)(h.HashOf("first") % 4)));
    }

    [Fact]
    public void Clear_EmptiesAllBuckets()
    {
        var h = new TStringHashEx(4);
        for (int i = 0; i < 20; i++) h.Add("k" + i, i);
        h.Clear();
        for (int i = 0; i < 4; i++) Assert.Null(h.BucketAt(i));
        Assert.Equal(-1, h.ValueOf("k0"));
    }

    // ---------------- THashedStringListEx ----------------

    [Fact]
    public void HashedStringList_IndexOfName_IsCaseInsensitiveByDefault()
    {
        // 原 483-490 + 492-518：CaseSensitive=False 时用 AnsiUpperCase 建索引
        var sl = new THashedStringListEx();
        sl.Add("Alpha=1");
        Assert.Equal(0, sl.IndexOfName("alpha"));
        Assert.Equal(0, sl.IndexOfName("ALPHA"));
        Assert.Equal(-1, sl.IndexOfName("beta"));
    }

    [Fact]
    public void HashedStringList_IndexOfName_CaseSensitiveMode()
    {
        var sl = new THashedStringListEx { CaseSensitive = true };
        sl.Add("Alpha=1");
        Assert.Equal(0, sl.IndexOfName("Alpha"));
        Assert.Equal(-1, sl.IndexOfName("alpha"));   // 原 483-490 分支差异断言
    }

    [Fact]
    public void HashedStringList_ValueHash_LazyRebuildAfterChanged()
    {
        var sl = new THashedStringListEx();
        sl.Add("x=1");
        Assert.False(sl.ValueHashValidForTest);      // 尚未建索引（惰性）
        Assert.Equal(0, sl.IndexOf("x=1"));
        Assert.True(sl.ValueHashValidForTest);       // 查询后建立
        sl.Add("y=2");
        Assert.False(sl.ValueHashValidForTest);      // Changed 置脏（原 460-465）
    }

    [Fact]
    public void HashedStringList_Names_OnlyBeforeSeparator()
    {
        var sl = new THashedStringListEx();
        sl.Add("Key=Value");
        sl.Add("NoEq");
        sl.Add("=OnlyValue");
        Assert.Equal("Key", sl.Names(0));
        Assert.Equal("NoEq", sl.Names(1));         // 无 '=' → P=0 → 返回整行
        Assert.Equal("=OnlyValue", sl.Names(2));   // '=' 在首位 → 仍 P=0 → 返回整行（差异断言）
    }

    [Fact]
    public void HashedStringList_Add_ReturnsInsertIndex()
    {
        var sl = new THashedStringListEx();
        Assert.Equal(0, sl.Add("a"));
        Assert.Equal(1, sl.Add("b"));
        sl.Delete(0);
        Assert.Equal(0, sl.IndexOf("b"));
    }

    // ---------------- TMemIniFileEx 读取 ----------------

    private static string TempFile() => Path.Combine(Path.GetTempPath(), "gxx_ini_" + Guid.NewGuid().ToString("N") + ".ini");

    [Fact]
    public void ReadString_Missing_ReturnsDefault()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "K", "V");
        Assert.Equal("V", ini.ReadString("S", "K", "DEF"));
        Assert.Equal("DEF", ini.ReadString("S", "K2", "DEF"));
        Assert.Equal("DEF", ini.ReadString("S2", "K", "DEF"));
    }

    [Fact]
    public void ReadString_EmptyValue_ReturnsEmptyNotNull()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "K", "");
        Assert.Equal("", ini.ReadString("S", "K", "DEF"));    // 命中即返回空串（原 697-715）
    }

    [Fact]
    public void ReadString_ValueWithEquals_KeepsRestIntact()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "K", "a=b=c");
        Assert.Equal("a=b=c", ini.ReadString("S", "K", ""));
    }

    [Fact]
    public void WriteString_ExistingKey_OverwritesInPlace_KeepingOrder()
    {
        // 原 784-801：IndexOfName 命中即替换该行 → 键顺序稳定
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "A", "1");
        ini.WriteString("S", "B", "2");
        ini.WriteString("S", "A", "9");
        var list = new GXX.Core.Util.TStringList();
        ini.ReadSectionValues("S", list);
        Assert.Equal(2, list.Count);
        Assert.Equal("A=9", list[0]);
        Assert.Equal("B=2", list[1]);
    }

    [Fact]
    public void ReadInteger_HandlesHexPrefix0x_AsDollar()
    {
        // 原 156-166：'0x'/'0X' 前缀被改写为 '$'，再交给 Delphi StrToIntDef
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "Hex", "0xFF");
        Assert.Equal(255, ini.ReadInteger("S", "Hex", -1));

        ini.WriteString("S", "HexUp", "0X10");
        Assert.Equal(16, ini.ReadInteger("S", "HexUp", -1));

        ini.WriteString("S", "Dollar", "$20");
        Assert.Equal(32, ini.ReadInteger("S", "Dollar", -1));

        ini.WriteString("S", "Neg", "-7");
        Assert.Equal(-7, ini.ReadInteger("S", "Neg", 0));

        ini.WriteString("S", "Bad", "xyz");
        Assert.Equal(42, ini.ReadInteger("S", "Bad", 42));
    }

    [Fact]
    public void ReadInteger_ShortHexString_NotRewritten()
    {
        // 原 156-166 边界：Length(IntStr) > 2 才改写 → "0x" 长度为 2，不改写 → StrToIntDef 失败取默认
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "K", "0x");
        Assert.Equal(7, ini.ReadInteger("S", "K", 7));
    }

    [Fact]
    public void ReadBool_WriteBool_UsesZeroOne()
    {
        // 原 267-272：Values: array[Boolean] of string = ('0','1')
        var ini = new TMemIniFileEx("");
        ini.WriteBool("S", "T", 1);
        ini.WriteBool("S", "F", 0);
        Assert.Equal("1", ini.ReadString("S", "T", ""));
        Assert.Equal("0", ini.ReadString("S", "F", ""));
        Assert.Equal((byte)1, ini.ReadBool("S", "T", 0));
        Assert.Equal((byte)0, ini.ReadBool("S", "F", 1));
        // "2" != 0 → True（原 173-177 用 <> 0 判定）
        ini.WriteString("S", "Two", "2");
        Assert.Equal((byte)1, ini.ReadBool("S", "Two", 0));
    }

    [Fact]
    public void ReadFloat_Invalid_ReturnsDefault()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "F", "1.5");
        Assert.Equal(1.5, ini.ReadFloat("S", "F", 0.0), 10);
        ini.WriteString("S", "Bad", "abc");
        Assert.Equal(9.25, ini.ReadFloat("S", "Bad", 9.25), 10);
        Assert.Equal(3.5, ini.ReadFloat("S", "Missing", 3.5), 10);
    }

    [Fact]
    public void SectionExists_ValueExists_EmptySectionIsFalse()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "K", "V");
        Assert.True(ini.SectionExists("S"));
        Assert.False(ini.SectionExists("Nope"));
        Assert.True(ini.ValueExists("S", "K"));
        Assert.False(ini.ValueExists("S", "K2"));

        // 原 143-154：SectionExists 走 ReadSection（只列键名），空节 → False
        ini.WriteString("Empty", "A", "1");
        ini.DeleteKey("Empty", "A");
        Assert.False(ini.SectionExists("Empty"));
    }

    [Fact]
    public void ReadSection_ReturnsKeyNamesOnly_InOrder()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "B", "2");
        ini.WriteString("S", "A", "1");
        var list = new GXX.Core.Util.TStringList();
        ini.ReadSection("S", list);
        Assert.Equal(new[] { "B", "A" }, new[] { list[0], list[1] });
    }

    [Fact]
    public void ReadSections_ReturnsSectionNamesInInsertionOrder()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("Z", "k", "1");
        ini.WriteString("A", "k", "1");
        ini.WriteString("M", "k", "1");
        var list = new GXX.Core.Util.TStringList();
        ini.ReadSections(list);
        Assert.Equal(new[] { "Z", "A", "M" }, new[] { list[0], list[1], list[2] });
    }

    [Fact]
    public void EraseSection_And_DeleteKey()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "A", "1");
        ini.WriteString("S", "B", "2");
        ini.DeleteKey("S", "A");
        Assert.False(ini.ValueExists("S", "A"));
        Assert.True(ini.ValueExists("S", "B"));
        ini.DeleteKey("Nope", "X");       // 不存在不抛（原 586-599）
        ini.EraseSection("S");
        Assert.False(ini.SectionExists("S"));
        ini.EraseSection("Nope");         // 原 601-611
    }

    [Fact]
    public void CaseSensitive_DefaultFalse_SectionLookupIgnored()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("Sec", "Key", "V");
        Assert.Equal("V", ini.ReadString("sec", "KEY", ""));   // 大小写不敏感
    }

    [Fact]
    public void CaseSensitive_True_KeyLookupDistinguishesCase()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("Sec", "Key", "V");
        ini.CaseSensitive = true;
        Assert.Equal("V", ini.ReadString("Sec", "Key", ""));
        Assert.Equal("DEF", ini.ReadString("Sec", "KEY", "DEF"));   // 差异断言
        Assert.True(ini.CaseSensitive);
    }

    // ---------------- SetStrings 解析 ----------------

    [Fact]
    public void SetStrings_ParsesSections_TrimsSpacesAroundEquals_SkipsComments()
    {
        var src = new GXX.Core.Util.TStringList();
        src.Add("; comment line");
        src.Add("");
        src.Add("  [Sec]  ");
        src.Add("  Key   =   Value  ");
        src.Add("Bare");
        src.Add("Other=1=2");

        var ini = new TMemIniFileEx("");
        ini.SetStrings(src);

        Assert.Equal("Value", ini.ReadString("Sec", "Key", ""));
        Assert.Equal("1=2", ini.ReadString("Sec", "Other", ""));   // 只按第一个 '=' 切分
        var names = new GXX.Core.Util.TStringList();
        ini.ReadSection("Sec", names);
        Assert.Equal(new[] { "Key", "Bare", "Other" }, new[] { names[0], names[1], names[2] });
    }

    [Fact]
    public void SetStrings_LineBeforeAnySection_IsIgnored()
    {
        var src = new GXX.Core.Util.TStringList();
        src.Add("Orphan=1");
        src.Add("[S]");
        src.Add("K=V");
        var ini = new TMemIniFileEx("");
        ini.SetStrings(src);
        var sections = new GXX.Core.Util.TStringList();
        ini.ReadSections(sections);
        Assert.Equal(1, sections.Count);
        Assert.Equal("S", sections[0]);
        Assert.Equal("V", ini.ReadString("S", "K", ""));
    }

    [Fact]
    public void SetStrings_DuplicateKeys_BothKept_ReadHitsLastInFile()
    {
        // LoadValues 阶段不覆盖：重复键共存；ReadString 走哈希链头 → 命中文件中最后一个
        var src = new GXX.Core.Util.TStringList();
        src.Add("[S]");
        src.Add("K=first");
        src.Add("K=second");
        var ini = new TMemIniFileEx("");
        ini.SetStrings(src);
        var vals = new GXX.Core.Util.TStringList();
        ini.ReadSectionValues("S", vals);
        Assert.Equal(2, vals.Count);
        Assert.Equal("K=first", vals[0]);
        Assert.Equal("K=second", vals[1]);
        Assert.Equal("second", ini.ReadString("S", "K", ""));
    }

    [Fact]
    public void SetStrings_SectionNameIsTrimmed()
    {
        var src = new GXX.Core.Util.TStringList();
        src.Add("[  Spaced  ]");
        src.Add("K=V");
        var ini = new TMemIniFileEx("");
        ini.SetStrings(src);
        Assert.Equal("V", ini.ReadString("Spaced", "K", ""));
    }

    [Fact]
    public void Clear_RemovesEverything()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "K", "V");
        ini.Clear();
        var sections = new GXX.Core.Util.TStringList();
        ini.ReadSections(sections);
        Assert.Equal(0, sections.Count);
    }

    // ---------------- 文件读写 ----------------

    [Fact]
    public void Constructor_NonExistentFile_YieldsEmpty()
    {
        string path = TempFile();
        try
        {
            var ini = new TMemIniFileEx(path);
            var sections = new GXX.Core.Util.TStringList();
            ini.ReadSections(sections);
            Assert.Equal(0, sections.Count);
        }
        finally { if (File.Exists(path)) File.Delete(path); }
    }

    [Fact]
    public void UpdateFile_RoundTrip_PreservesOrderAndBlankLineBetweenSections()
    {
        string path = TempFile();
        try
        {
            var ini = new TMemIniFileEx("");
            ini.WriteString("S1", "A", "1");
            ini.WriteString("S1", "B", "2");
            ini.WriteString("S2", "C", "3");
            ini.Rename(path, false);
            ini.UpdateFile();

            string text = File.ReadAllText(path, GXX.Core.EncodingInit.GBK);
            Assert.Equal("[S1]\r\nA=1\r\nB=2\r\n\r\n[S2]\r\nC=3\r\n\r\n", text);

            // 重载后内容一致
            var again = new TMemIniFileEx(path);
            Assert.Equal("1", again.ReadString("S1", "A", ""));
            Assert.Equal("3", again.ReadString("S2", "C", ""));
        }
        finally { if (File.Exists(path)) File.Delete(path); }
    }

    [Fact]
    public void Rename_WithReloadTrue_ReloadsFromNewPath()
    {
        string path = TempFile();
        try
        {
            var a = new TMemIniFileEx("");
            a.WriteString("S", "K", "onDisk");
            a.Rename(path, false);
            a.UpdateFile();

            var b = new TMemIniFileEx("");
            b.WriteString("S", "K", "inMemory");
            b.Rename(path, true);                      // 原 717-722
            Assert.Equal("onDisk", b.ReadString("S", "K", ""));
            Assert.Equal(path, b.FileName);
        }
        finally { if (File.Exists(path)) File.Delete(path); }
    }

    [Fact]
    public void Rename_WithReloadFalse_KeepsMemoryImage()
    {
        string path = TempFile();
        try
        {
            var a = new TMemIniFileEx("");
            a.WriteString("S", "K", "onDisk");
            a.Rename(path, false);
            a.UpdateFile();

            var b = new TMemIniFileEx("");
            b.WriteString("S", "K", "inMemory");
            b.Rename(path, false);
            Assert.Equal("inMemory", b.ReadString("S", "K", ""));
        }
        finally { if (File.Exists(path)) File.Delete(path); }
    }

    [Fact]
    public void TIniFileEx_Dispose_WritesFile()
    {
        string path = TempFile();
        try
        {
            using (var f = new TIniFileEx(path))
            {
                f.WriteString("S", "K", "V");
            }
            Assert.True(File.Exists(path));
            Assert.Contains("K=V", File.ReadAllText(path, GXX.Core.EncodingInit.GBK));
        }
        finally { if (File.Exists(path)) File.Delete(path); }
    }

    [Fact]
    public void GetStrings_EmitsSectionHeadersAndTrailingBlankLine()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("A", "x", "1");
        ini.WriteString("B", "y", "2");
        var outList = new GXX.Core.Util.TStringList();
        ini.GetStrings(outList);
        Assert.Equal(new[] { "[A]", "x=1", "", "[B]", "y=2", "" },
            new[] { outList[0], outList[1], outList[2], outList[3], outList[4], outList[5] });
    }

    [Fact]
    public void GetCodepageFormat_NotApplicable_Placeholder()
    {
        // 占位：确认 IniFilesEx 不依赖 GetCodepage（ParadoxConv 才有）
        var ini = new TMemIniFileEx("");
        Assert.Equal("", ini.FileName);
    }

    // ---------------- 二进制流（HEX） ----------------

    [Fact]
    public void WriteBinaryStream_ProducesUppercaseHex()
    {
        var ini = new TMemIniFileEx("");
        var ms = new TMemoryStreamEx();
        ms.LoadFromBytes(new byte[] { 0x00, 0x0F, 0xA5, 0xFF });
        ini.WriteBinaryStream("S", "Bin", ms);
        Assert.Equal("000FA5FF", ini.ReadString("S", "Bin", ""));
    }

    [Fact]
    public void ReadBinaryStream_RoundTripsBytes()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "Bin", "000FA5FF");
        var ms = new TMemoryStreamEx();
        int n = ini.ReadBinaryStream("S", "Bin", ms);
        Assert.Equal(4, n);
        Assert.Equal(new byte[] { 0x00, 0x0F, 0xA5, 0xFF }, ms.ToArray());
        Assert.Equal(0, ms.Position);   // 原 306-307：读完 Position 复位到进入时的位置
    }

    [Fact]
    public void ReadBinaryStream_EmptyText_ReturnsZero()
    {
        var ini = new TMemIniFileEx("");
        var ms = new TMemoryStreamEx();
        Assert.Equal(0, ini.ReadBinaryStream("S", "Missing", ms));
        Assert.Equal(0, ms.Size);
    }

    [Fact]
    public void WriteBinaryStream_EmptyStream_WritesEmptyValue()
    {
        var ini = new TMemIniFileEx("");
        var ms = new TMemoryStreamEx();
        ini.WriteBinaryStream("S", "Bin", ms);
        Assert.Equal("", ini.ReadString("S", "Bin", ""));
    }

    [Fact]
    public void ReadBinaryStream_OddLengthText_UsesIntegerDivision()
    {
        // 原 304-305：Length(Text) div 2 → 奇数长度丢弃最后一个 nibble
        var ini = new TMemIniFileEx("");
        ini.WriteString("S", "Bin", "ABC");
        var ms = new TMemoryStreamEx();
        int n = ini.ReadBinaryStream("S", "Bin", ms);
        Assert.Equal(1, n);
        Assert.Equal(new byte[] { 0xAB }, ms.ToArray());
    }
}

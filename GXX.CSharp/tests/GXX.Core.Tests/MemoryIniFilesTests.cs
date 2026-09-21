using System;
using System.IO;
using System.Linq;
using GXX.Core.MemoryIni;
using GXX.Core.Util;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// <c>Source/Common/MemoryIniFiles.pas</c>（888 行）1:1 移植的用例集。
///
/// <para>
/// 计数取证（派发要求 #6：否定性断言必须计数取证）：
/// </para>
/// <list type="bullet">
/// <item>原文 `TQuickSortList` 成员 **12**（GetIndex/SortString/AddRecord/Get+SetCaseSensitive/Lock/UnLock/
///   构造/析构/boCaseSensitive 属性）→ 托管同名成员 **12**（`TQuickSortList`）；</item>
/// <item>原文 `TIniValueList` 成员 **1**（GetIndex override）→ 本类 `TIniValueList` 覆盖该语义
///   （`TIniValueLookup.LinearCaseSensitive`，默认值）；</item>
/// <item>原文 `TMemoryIniFile` 成员 **29**（4 构造 + 析构 + LoadFromFile/SaveToFile/SaveToList/ReadSection/
///   Get/Changed + OnChange + 4×2 字符串节 + 4×2 整数节 Read/Write 共 16 + 4 个带节名字符串重载的读/写）→ 
///   托管公开成员逐条对应（见本文件逐组用例）；</item>
/// <item>DFM：本单元**无窗体**（0 控件 / 0 绑定）⇒ 无 DFM 对账项。</item>
/// </list>
/// <para>原文缺陷全部以 `// 原文如此` 标注并另有 <c>*_OriginalFlaw</c> 用例锁定（不"修正"）。</para>
/// </summary>
public class MemoryIniFilesTests : IDisposable
{
    private readonly string _dir;

    public MemoryIniFilesTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx-core-memoryini-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { }
    }

    private string Tmp(string name) => Path.Combine(_dir, name);

    private static TStringList Lines(params string[] lines)
    {
        var l = new TStringList();
        foreach (string s in lines) l.Add(s);
        return l;
    }

    private static TMemoryIniFile Sample() => new TMemoryIniFile(Lines(
        ";comment at top",
        "[Server]",
        "Name=Mir2",
        "Port=7000",
        "",
        "[Client]",
        "Name=cli"));

    // =====================================================================================
    // TQuickSortList（MemoryIniFiles.pas:7-22 / 101-409）
    // =====================================================================================

    [Fact]
    public void QuickSortList_GetIndex_EmptyList_ReturnsMinusOne()
    {
        var l = new TQuickSortList();
        Assert.Equal(-1, l.GetIndex("a"));
    }

    [Fact]
    public void QuickSortList_GetIndex_SingleElement_HitAndMiss()
    {
        var l = new TQuickSortList();
        l.Add("Only");
        Assert.Equal(0, l.GetIndex("Only"));
        Assert.Equal(-1, l.GetIndex("Other"));
    }

    [Fact]
    public void QuickSortList_GetIndex_UnsortedBranch_IsCaseInsensitive()
    {
        // Sorted=False（原文从不在 Get 之后置 True）⇒ 走 CompareText 分支
        var l = new TQuickSortList();
        l.Add("Client");
        l.Add("Server");
        Assert.False(l.Sorted);
        Assert.Equal(1, l.GetIndex("Server"));
        Assert.Equal(1, l.GetIndex("SERVER"));
        Assert.Equal(0, l.GetIndex("client"));
    }

    [Fact]
    public void QuickSortList_GetIndex_FourElements_ExercisesBinaryElseBranch()
    {
        var l = new TQuickSortList();
        foreach (string s in new[] { "s1", "s2", "s3", "s4" }) l.Add(s);
        Assert.Equal(2, l.GetIndex("s3"));
        Assert.Equal(0, l.GetIndex("s1"));
        Assert.Equal(3, l.GetIndex("s4"));
        Assert.Equal(-1, l.GetIndex("s0"));
    }

    [Fact]
    public void QuickSortList_GetIndex_SortedBranch_IsCaseSensitive()
    {
        var l = new TQuickSortList();
        l.Add("Client");
        l.Add("Server");
        l.Sorted = true;                       // 触发 Sort()（继承）→ GetIndex 改走 CompareStr 分支
        Assert.True(l.Sorted);
        Assert.Equal(1, l.GetIndex("Server"));
        Assert.Equal(-1, l.GetIndex("SERVER")); // CompareStr：大小写敏感
    }

    [Fact]
    public void QuickSortList_boCaseSensitive_DoesNotAffectGetIndex_OriginalFlaw()
    {
        // ★ 原文如此：GetIndex 的分支判据是 Self.Sorted，**不是** CaseSensitive
        //   ⇒ 暴露出来的 boCaseSensitive 对查找毫无影响（只有 Sorted=True 才会大小写敏感）。
        var l = new TQuickSortList();
        l.Add("Client");
        l.Add("Server");
        l.boCaseSensitive = true;
        Assert.True(l.CaseSensitive);
        Assert.Equal(1, l.GetIndex("SERVER"));
    }

    [Fact]
    public void QuickSortList_SortString_SortsByCompareText()
    {
        var l = new TQuickSortList();
        foreach (string s in new[] { "delta", "Alpha", "charlie", "Bravo" }) l.Add(s);
        l.SortString(0, l.Count - 1);
        Assert.Equal(new[] { "Alpha", "Bravo", "charlie", "delta" },
            new[] { l[0], l[1], l[2], l[3] });
    }

    [Fact]
    public void QuickSortList_SortString_EmptyList_IsNoOp()
    {
        var l = new TQuickSortList();
        l.SortString(0, l.Count - 1);          // (0, -1)
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void QuickSortList_SortString_SingleElement_Terminates()
    {
        var l = new TQuickSortList();
        l.Add("x");
        l.SortString(0, 0);
        Assert.Equal("x", l[0]);
    }

    [Fact]
    public void QuickSortList_AddRecord_InsertsInCompareTextOrder()
    {
        var l = new TQuickSortList();
        Assert.True(l.AddRecord("b", null));
        Assert.True(l.AddRecord("a", null));
        Assert.True(l.AddRecord("c", null));
        Assert.Equal(new[] { "a", "b", "c" }, new[] { l[0], l[1], l[2] });
    }

    [Fact]
    public void QuickSortList_AddRecord_CountOneDuplicate_ReturnsTrueWithoutInserting_OriginalFlaw()
    {
        // ★ 原文如此（MemoryIniFiles.pas:236-245）：Count=1 且同名 ⇒ 既不插入、也不置 Result := False
        var l = new TQuickSortList();
        l.AddRecord("a", null);
        Assert.True(l.AddRecord("a", null));
        Assert.Equal(1, l.Count);
    }

    [Fact]
    public void QuickSortList_AddRecord_CountTwoDuplicate_InsertsDuplicate_OriginalFlaw()
    {
        // ★ 原文如此：二分推进的收口条件 (nHigh-nLow)=1 会先把 nMed 与 nHigh 比、再与 nLow 比，
        //   命中"与 nHigh 相等"时 nMed 归零后又被下一句覆写 ⇒ 走成 InsertObject(nLow+1) ⇒ **插入重复项**且返回 True。
        var l = new TQuickSortList();
        l.AddRecord("a", null);
        l.AddRecord("c", null);
        Assert.True(l.AddRecord("c", null));
        Assert.Equal(3, l.Count);
        Assert.Equal(new[] { "a", "c", "c" }, new[] { l[0], l[1], l[2] });
    }

    [Fact]
    public void QuickSortList_AddRecord_CountThreeMiddleDuplicate_ReturnsFalse()
    {
        var l = new TQuickSortList();
        l.AddRecord("a", null);
        l.AddRecord("b", null);
        l.AddRecord("c", null);
        Assert.False(l.AddRecord("b", null));
        Assert.Equal(3, l.Count);
    }

    [Fact]
    public void QuickSortList_LockUnLock_AreReentrantSafe()
    {
        var l = new TQuickSortList();
        l.Lock();
        try
        {
            l.Add("x");
        }
        finally
        {
            l.UnLock();
        }
        Assert.Equal(1, l.Count);
    }

    [Fact]
    public void QuickSortList_OnChange_FiresOnMutations()
    {
        var l = new TQuickSortList();
        int n = 0;
        l.OnChange = _ => n++;
        l.Add("a");
        Assert.Equal(1, n);
        l.AddObject("b", "obj");
        Assert.Equal(2, n);
        l.InsertObject(0, "c", null);
        Assert.Equal(3, n);
        l.Delete(0);
        Assert.Equal(4, n);
        l.Exchange(0, 1);
        Assert.Equal(5, n);
        l.Clear();
        Assert.Equal(6, n);
        l.Clear();                     // 原文 TStringList.Clear 仅在非空时 Changed
        Assert.Equal(6, n);
    }

    // =====================================================================================
    // TIniValueList（MemoryIniFiles.pas:24-27 / 84-97 + SDK.pas TValueList 的插入算法）
    // =====================================================================================

    [Fact]
    public void IniValueList_LinearLookup_IsCaseSensitive()
    {
        var v = new TIniValueList();     // 默认 Lookup = LinearCaseSensitive
        v.AddRecord("Key", "1");
        Assert.Equal(0, v.GetIndex("Key"));
        Assert.Equal(-1, v.GetIndex("key"));   // CompareStr
    }

    [Fact]
    public void IniValueList_BinaryLookup_IsCaseInsensitiveWhenNotSorted()
    {
        var v = new TIniValueList { Lookup = TIniValueLookup.Binary };
        v.AddRecord("Key", "1");
        Assert.Equal(0, v.GetIndex("key"));
        Assert.Equal(0, v.GetIndex("KEY"));
    }

    [Fact]
    public void IniValueList_NamesStringsSetStrings_RoundTrip()
    {
        var v = new TIniValueList();
        v.AddRecord("A", "1");
        v.AddRecord("B", "2");
        Assert.Equal(2, v.Count);
        Assert.Equal("A", v.Names(0));
        Assert.Equal("1", v.Strings(0));
        v.SetStrings(0, "9");
        Assert.Equal("9", v.Strings(0));
    }

    [Fact]
    public void IniValueList_OnChange_FiresOnAddAndSet()
    {
        var v = new TIniValueList();
        int n = 0;
        v.OnChange = _ => n++;
        v.AddObject("A", "1", null);
        Assert.Equal(1, n);
        v.SetStrings(0, "2");            // 原文 Put → Changed
        Assert.Equal(2, n);
        v.Delete(0);
        Assert.Equal(3, n);
    }

    [Fact]
    public void IniValueList_SortedTrue_SortsAndSwitchesToCompareStr()
    {
        var v = new TIniValueList { Lookup = TIniValueLookup.Binary };
        v.AddRecord("b", "2");
        v.AddRecord("a", "1");
        v.Sorted = true;                 // SetSorted：先 Sort() 再置位
        Assert.True(v.Sorted);
        Assert.Equal("a", v.Names(0));
        Assert.Equal(0, v.GetIndex("a"));
        Assert.Equal(-1, v.GetIndex("A"));   // Sorted=True → CompareStr
    }

    [Fact]
    public void IniValueList_AddRecord_CountOneDuplicate_ReturnsTrueWithoutInserting()
    {
        var v = new TIniValueList();
        v.AddRecord("a", "1");
        Assert.True(v.AddRecord("a", "2"));
        Assert.Equal(1, v.Count);
        Assert.Equal("1", v.Strings(0));
    }

    // =====================================================================================
    // TMemoryIniFile：构造 / Get / 查找语义
    // =====================================================================================

    [Fact]
    public void Ctor_Default_EmptySections()
    {
        var ini = new TMemoryIniFile();
        Assert.Equal(0, ini.Sections.Count);
        Assert.False(ini.LoadOKFlag);
        Assert.False(ini.ChangedFlag);
    }

    [Fact]
    public void Ctor_FileList_ParsesSections()
    {
        var ini = Sample();
        Assert.Equal(2, ini.Sections.Count);
        Assert.True(ini.LoadOKFlag);
        Assert.Equal("Mir2", ini.ReadString("Server", "Name", ""));
        Assert.Equal("cli", ini.ReadString("Client", "Name", ""));
    }

    [Fact]
    public void Ctor_Text_ParsesSections()
    {
        var ini = new TMemoryIniFile("[S]\r\nK=V");
        Assert.Equal("V", ini.ReadString("S", "K", ""));
    }

    [Fact]
    public void Ctor_DataBytes_DecodesGbk()
    {
        string text = "[S]\r\nK=" + "值";
        byte[] data = System.Text.Encoding.GetEncoding(936).GetBytes(text);
        var ini = new TMemoryIniFile(data, data.Length);
        Assert.Equal("值", ini.ReadString("S", "K", ""));
    }

    [Fact]
    public void Get_SortsSectionNamesByCompareText()
    {
        var ini = Sample();
        // SortString(0, Count-1) → CompareText 升序：Client 在 Server 之前
        Assert.Equal("Client", ini.Sections[0]);
        Assert.Equal("Server", ini.Sections[1]);
        Assert.False(ini.Sections.Sorted);      // ★ 原文只手工快排，**不置 Sorted**
    }

    [Fact]
    public void Get_LineBeforeFirstSection_IsIgnored()
    {
        var ini = new TMemoryIniFile(Lines("Orphan=1", "[S]", "K=V"));
        Assert.Equal(1, ini.Sections.Count);
        Assert.Equal("V", ini.ReadString("S", "K", ""));
        Assert.Equal("", ini.ReadString("S", "Orphan", ""));
    }

    [Fact]
    public void Get_LineWithoutEquals_IsIgnored()
    {
        var ini = new TMemoryIniFile(Lines("[S]", "K=V", "noequals"));
        Assert.Equal("V", ini.ReadString("S", "K", ""));
        Assert.Equal(1, ((TIniValueList)ini.Sections.GetObject(0)).Count);
    }

    [Fact]
    public void Get_LeadingWhitespaceIsTrimmedLeft()
    {
        var ini = new TMemoryIniFile(Lines("   [S]", "   K=V"));
        Assert.Equal("V", ini.ReadString("S", "K", ""));
    }

    [Fact]
    public void Get_SectionNameEndsAtFirstBracket_OriginalFlaw()
    {
        // ★ 原文如此：nPos := Pos(']', LineText) 取**第一个** ']'，且不要求它在行尾
        //   ⇒ "[a]b]" 的节名是 "a"（'b]' 被丢弃）；"[a] 尾注" 的节名也是 "a"。
        var ini = new TMemoryIniFile(Lines("[a]b]", "K=V"));
        Assert.Equal(1, ini.Sections.Count);
        Assert.Equal("a", ini.Sections[0]);
        Assert.Equal("V", ini.ReadString("a", "K", ""));
    }

    [Fact]
    public void Get_DuplicateSectionNames_AreKeptTwice_OriginalFlaw()
    {
        // ★ 原文如此：节的登记走 TQuickSortList.AddObject（**不去重**）⇒ 同名节会有两份；
        //   随后 SortString 的 Exchange 会把两份的顺序（连同 Objects）翻过来，GetIndex 的二分
        //   在 (nHigh-nLow)=1 时先比 nHigh、再比 nLow ⇒ 最终命中 **0 号**。
        //   两份内容在这里是 "K=1"/"K=2"，故命中哪一份就返回哪一个值。
        var ini = new TMemoryIniFile(Lines("[S]", "K=1", "[S]", "K=2"));
        Assert.Equal(2, ini.Sections.Count);
        Assert.Equal("S", ini.Sections[0]);
        Assert.Equal("S", ini.Sections[1]);
        Assert.Equal(0, ini.Sections.GetIndex("S"));
        Assert.Contains(ini.ReadString("S", "K", ""), new[] { "1", "2" });
    }

    [Fact]
    public void ReadString_SectionLookupIsCaseInsensitive_KeyLookupIsCaseSensitive_OriginalFlaw()
    {
        // ★ 原文如此（类注释第 1 条）：节名 CompareText、键名 CompareStr
        var ini = Sample();
        Assert.Equal("cli", ini.ReadString("client", "Name", ""));
        Assert.Equal("cli", ini.ReadString("CLIENT", "Name", ""));
        Assert.Equal("", ini.ReadString("Client", "NAME", ""));
        Assert.Equal("dflt", ini.ReadString("Client", "NAME", "dflt"));
    }

    [Fact]
    public void ReadString_MissingSectionOrKey_ReturnsDefault()
    {
        var ini = Sample();
        Assert.Equal("D", ini.ReadString("Nope", "Name", "D"));
        Assert.Equal("D", ini.ReadString("Client", "Nope", "D"));
    }

    [Fact]
    public void WriteString_NewSection_UsesBinaryLookup_KeyBecomesCaseInsensitive_OriginalFlaw()
    {
        // ★ 原文如此（类注释第 2 条）：WriteString 在"节不存在"时建的是 **TValueList**（不是 TIniValueList）
        //   ⇒ 该节的键查找走 SDK.pas:702 的二分 + CompareText（大小写不敏感），与解析出来的节相反。
        var ini = Sample();
        ini.WriteString("NewSec", "k", "v");
        Assert.Equal("v", ini.ReadString("NewSec", "k", ""));
        Assert.Equal("v", ini.ReadString("NEWSEC", "K", "v-miss"));   // 节 + 键都大小写不敏感
    }

    [Fact]
    public void WriteString_ExistingParsedSection_CaseDifferentKeyIsSilentlyDropped_OriginalFlaw()
    {
        // ★★ 原文缺陷（本单元最有价值的一条，回读 MemoryIniFiles.pas:625-655 + :223-377 逐段确认）：
        //   往"解析出来的节"里写一个**与已有键仅大小写不同**的键时：
        //     ① 查找走 TIniValueList.GetIndex（**CompareStr**）⇒ 认为不存在；
        //     ② 插入走 TValueList.AddRecord，而节内只有 1 个键 ⇒ 命中 `Count = 1` 分支，
        //        该分支用 **CompareText** 比较、"相等则什么都不做、且不置 Result := False"；
        //     ③ 于是**既没插入、也没覆盖**，新值被静默丢弃，方法却返回成功。
        //   后果：`WriteString(Section, 'name', …)` 在 INI 里已有 'Name' 时**永久失效**。
        var ini = Sample();
        var clientList = (TIniValueList)ini.Sections.GetObject(ini.Sections.GetIndex("Client"));
        Assert.Equal(1, clientList.Count);

        ini.WriteString("Client", "name", "X");       // 被静默丢弃

        Assert.Equal(1, clientList.Count);            // 键数不变
        Assert.Equal("", ini.ReadString("Client", "name", ""));   // 新键读不到
        Assert.Equal("cli", ini.ReadString("Client", "Name", "")); // 原值也没被改
    }

    [Fact]
    public void WriteString_ExistingParsedSection_MultiKey_CaseDifferentKeyIsInserted()
    {
        // 对照组：节内已有 **2** 个键时，AddRecord 走 (nHigh-nLow)=1 分支 ⇒ 会真的插入新键，
        // 于是出现 'Port' 与 'port' 两个"看起来同名"的键（CompareText 相等、CompareStr 不等）。
        var ini = Sample();
        ini.WriteString("Server", "port", "7001");
        var serverList = (TIniValueList)ini.Sections.GetObject(ini.Sections.GetIndex("Server"));
        Assert.Equal(3, serverList.Count);
        Assert.Equal("7001", ini.ReadString("Server", "port", ""));
        Assert.Equal("7000", ini.ReadString("Server", "Port", ""));
    }

    [Fact]
    public void WriteString_ExistingKey_UpdatesInPlaceWithoutAdding()
    {
        var ini = Sample();
        ini.WriteString("Server", "Name", "Changed");
        Assert.Equal("Changed", ini.ReadString("Server", "Name", ""));
        Assert.Equal(2, ((TIniValueList)ini.Sections.GetObject(ini.Sections.GetIndex("Server"))).Count);
    }

    [Fact]
    public void WriteString_SetsChangedFlagAndRaisesOnChange()
    {
        var ini = new TMemoryIniFile();
        int n = 0;
        TMemoryIniFile? sender = null;
        ini.OnChange = s => { n++; sender = s; };
        ini.WriteString("S", "K", "V");
        // 原文的触发源不止一处：节表 AddRecord（TStringList.Changed）+ 值表 AddRecord（TValueList.Changed）
        // ⇒ 只断言"确有通知且回调拿到自己"，不锁次数（次数由两层 Changed 叠加决定）。
        Assert.True(ini.ChangedFlag);
        Assert.True(n >= 1);
        Assert.Same(ini, sender);
        int before = n;
        ini.WriteString("S", "K", "V2");     // 已存在 → 值表 Put（SetStrings）→ Changed
        Assert.True(n > before);
    }

    // =====================================================================================
    // 整数 / 布尔
    // =====================================================================================

    [Fact]
    public void Integer_RoundTrip()
    {
        var ini = new TMemoryIniFile();
        ini.WriteInteger("S", "N", -255);
        Assert.Equal(-255, ini.ReadInteger("S", "N", 0));
        Assert.Equal("-255", ini.ReadString("S", "N", ""));
    }

    [Fact]
    public void ReadInteger_Rewrites0xPrefixToDollar_AndParsesHex()
    {
        // 原文 MemoryIniFiles.pas:657-666：'0x'/'0X' → '$'，随后 StrToIntDef 认 '$' 十六进制
        var ini = new TMemoryIniFile();
        ini.WriteString("S", "A", "0x1F");
        ini.WriteString("S", "B", "0X1f");
        ini.WriteString("S", "C", "$2A");
        ini.WriteString("S", "D", "42");
        Assert.Equal(31, ini.ReadInteger("S", "A", 0));
        Assert.Equal(31, ini.ReadInteger("S", "B", 0));
        Assert.Equal(42, ini.ReadInteger("S", "C", 0));
        Assert.Equal(42, ini.ReadInteger("S", "D", 0));
    }

    [Fact]
    public void ReadInteger_NonNumeric_ReturnsDefault()
    {
        var ini = new TMemoryIniFile();
        ini.WriteString("S", "K", "abc");
        Assert.Equal(7, ini.ReadInteger("S", "K", 7));
        ini.WriteString("S", "E", "");
        Assert.Equal(7, ini.ReadInteger("S", "E", 7));
    }

    [Fact]
    public void Bool_WriteIsOneZero_ReadIsAnyNonZeroInteger()
    {
        // 原文 WriteBool 678-683（'0'/'1'）、ReadBool 673-676（ReadInteger(...) <> 0）
        var ini = new TMemoryIniFile();
        ini.WriteBool("S", "T", true);
        ini.WriteBool("S", "F", false);
        Assert.Equal("1", ini.ReadString("S", "T", ""));
        Assert.Equal("0", ini.ReadString("S", "F", ""));
        Assert.True(ini.ReadBool("S", "T", false));
        Assert.False(ini.ReadBool("S", "F", true));

        ini.WriteString("S", "Neg", "-1");
        Assert.True(ini.ReadBool("S", "Neg", false));      // 任何非 0 整数都算真
        ini.WriteString("S", "Two", "2");
        Assert.True(ini.ReadBool("S", "Two", false));
    }

    [Fact]
    public void Bool_TextTrueFallsBackToDefault_OriginalFlaw()
    {
        // ★ 原文如此：'True' 不是整数 ⇒ StrToIntDef 回退 Default（不是无条件真）
        var ini = new TMemoryIniFile();
        ini.WriteString("S", "K", "True");
        Assert.False(ini.ReadBool("S", "K", false));
        Assert.True(ini.ReadBool("S", "K", true));
    }

    // =====================================================================================
    // 日期 / 时间 / 浮点（4 组 × 字符串节 + 整数节）
    // =====================================================================================

    [Fact]
    public void Date_DateTime_Time_Float_RoundTrip_StringSection()
    {
        var ini = new TMemoryIniFile();
        ini.WriteString("S", "D", "2024-03-05");
        ini.WriteString("S", "DT", "2024-03-05 06:07:08");
        ini.WriteString("S", "T", "06:07:08");
        ini.WriteString("S", "F", "3.5");

        Assert.Equal(new DateTime(2024, 3, 5).ToOADate(), ini.ReadDate("S", "D", -1.0), 10);
        Assert.Equal(new DateTime(2024, 3, 5, 6, 7, 8).ToOADate(), ini.ReadDateTime("S", "DT", -1.0), 10);
        Assert.Equal(new TimeSpan(6, 7, 8).TotalDays, ini.ReadTime("S", "T", -1.0), 10);
        Assert.Equal(3.5, ini.ReadFloat("S", "F", -1.0), 10);
    }

    [Fact]
    public void Date_DateTime_Time_Float_InvalidOrEmpty_ReturnsDefault()
    {
        var ini = new TMemoryIniFile();
        ini.WriteString("S", "Bad", "not-a-date");
        ini.WriteString("S", "Empty", "");
        foreach (string key in new[] { "Bad", "Empty" })
        {
            Assert.Equal(-9.0, ini.ReadDate("S", key, -9.0), 10);
            Assert.Equal(-9.0, ini.ReadDateTime("S", key, -9.0), 10);
            Assert.Equal(-9.0, ini.ReadTime("S", key, -9.0), 10);
            Assert.Equal(-9.0, ini.ReadFloat("S", key, -9.0), 10);
        }
        // 键完全不存在 ⇒ ReadString 返回 '' ⇒ 同样回退 Default
        Assert.Equal(-9.0, ini.ReadDate("S", "Missing", -9.0), 10);
    }

    [Fact]
    public void Date_DateTime_Time_Float_RoundTrip_IntegerSection()
    {
        var ini = new TMemoryIniFile(Lines("[S]", "K=V"));
        Assert.Equal(0, ini.Sections.GetIndex("S"));
        ini.WriteString(0, "D", "2024-03-05");
        ini.WriteString(0, "DT", "2024-03-05 06:07:08");
        ini.WriteString(0, "T", "06:07:08");
        ini.WriteString(0, "F", "3.5");
        ini.WriteString(0, "Bad", "zzz");

        Assert.Equal(new DateTime(2024, 3, 5).ToOADate(), ini.ReadDate(0, "D", -1.0), 10);
        Assert.Equal(new DateTime(2024, 3, 5, 6, 7, 8).ToOADate(), ini.ReadDateTime(0, "DT", -1.0), 10);
        Assert.Equal(new TimeSpan(6, 7, 8).TotalDays, ini.ReadTime(0, "T", -1.0), 10);
        Assert.Equal(3.5, ini.ReadFloat(0, "F", -1.0), 10);
        Assert.Equal(-1.0, ini.ReadDate(0, "Bad", -1.0), 10);
    }

    // =====================================================================================
    // 整数节序号重载
    // =====================================================================================

    [Fact]
    public void IntegerSection_ReadString_OutOfRange_ReturnsDefault()
    {
        var ini = Sample();
        Assert.Equal("cli", ini.ReadString(0, "Name", ""));
        Assert.Equal("D", ini.ReadString(-1, "Name", "D"));
        Assert.Equal("D", ini.ReadString(9, "Name", "D"));
    }

    [Fact]
    public void IntegerSection_WriteString_OutOfRange_IsSilentNoOp_OriginalFlaw()
    {
        // ★ 原文如此（MemoryIniFiles.pas:703-721）：越界**不新建节、不抛异常、也不置 FChanged**
        var fresh = new TMemoryIniFile();
        fresh.WriteString(9, "K", "V");
        Assert.Equal(0, fresh.Sections.Count);        // 不建节
        Assert.False(fresh.ChangedFlag);              // 直接置位那句在越界分支之外 ⇒ 一次通知都没有
        Assert.Equal("D", fresh.ReadString(9, "K", "D"));

        var ini = Sample();
        ini.WriteString(9, "K", "V");
        Assert.Equal(2, ini.Sections.Count);
        Assert.Equal("D", ini.ReadString(9, "K", "D"));
    }

    [Fact]
    public void IntegerSection_WriteString_InRange_SetsChangedOnlyViaValueListOnChange_OriginalFlaw()
    {
        // ★ 原文如此：整数节序号的 WriteString 体内**没有** `FChanged := True`（字符串节名版才有，:630）；
        //   但它会走值表 AddRecord/Put ⇒ 值表 OnChange → TMemoryIniFile.Changed ⇒ FChanged **仍会**变 True。
        //   即"少写一句"在净效果上不可观察 —— 除非节序号越界（见上一条用例）。
        var fresh = new TMemoryIniFile();
        Assert.False(fresh.ChangedFlag);
        fresh.WriteString(0, "K", "V");               // 0 号节不存在 ⇒ 越界 ⇒ 无操作
        Assert.False(fresh.ChangedFlag);

        var ini = Sample();
        ini.WriteString(0, "New", "V");
        Assert.Equal("V", ini.ReadString(0, "New", ""));
        Assert.True(ini.ChangedFlag);
    }

    [Fact]
    public void IntegerSection_IntegerAndBool_RoundTrip()
    {
        var ini = Sample();
        ini.WriteInteger(0, "N", 12);
        ini.WriteBool(0, "B", true);
        Assert.Equal(12, ini.ReadInteger(0, "N", 0));
        Assert.True(ini.ReadBool(0, "B", false));
        ini.WriteString(0, "H", "0x10");
        Assert.Equal(16, ini.ReadInteger(0, "H", 0));
    }

    // =====================================================================================
    // ReadSection / SaveToList / SaveToFile / LoadFromFile / OnChange
    // =====================================================================================

    [Fact]
    public void ReadSection_AppendsValuesInInsertionOrder()
    {
        var ini = Sample();
        var outList = new TStringList();
        ini.ReadSection("Server", outList);
        // Server 的键插入序为 Name, Port（AddRecord 只在有多项时二分插入：Count=1 的 Name 先落，
        // 之后 "Port" 与 "Name" 比 → 追加）⇒ 顺序 = Name, Port
        Assert.Equal(new[] { "Mir2", "7000" }, new[] { outList[0], outList[1] });
    }

    [Fact]
    public void ReadSection_MissingSection_LeavesListUntouched()
    {
        var ini = Sample();
        var outList = new TStringList();
        outList.Add("keep");
        ini.ReadSection("Nope", outList);
        Assert.Equal(1, outList.Count);
        Assert.Equal("keep", outList[0]);
    }

    [Fact]
    public void SaveToList_WritesSectionsAndKeyValueLines()
    {
        var ini = Sample();
        var save = new TStringList();
        ini.SaveToList(save);
        Assert.Equal(new[] { "[Client]", "Name=cli", "[Server]", "Name=Mir2", "Port=7000" },
            save.AsEnumerable().ToArray());
    }

    [Fact]
    public void SaveToList_EmptyOrSemicolonSectionName_WritesRawLine_OriginalFlaw()
    {
        // ★ 原文如此（MemoryIniFiles.pas:513-516）：节名空 ⇒ 写**空行**；节名以 ';' 开头 ⇒ 写 ';x'（**丢方括号**）
        var ini = new TMemoryIniFile(Lines("[]", "K=V", "[;x]", "K=V"));
        var save = new TStringList();
        ini.SaveToList(save);
        Assert.Equal(new[] { "", ";x" }, save.AsEnumerable().ToArray());
    }

    [Fact]
    public void SaveToFile_ThenLoadFromFile_RoundTrip()
    {
        string path = Tmp("rt.ini");
        var ini = Sample();
        ini.SaveToFile(path);
        Assert.True(File.Exists(path));

        var again = new TMemoryIniFile();
        again.LoadFromFile(path);
        Assert.Equal(2, again.Sections.Count);
        Assert.Equal("Mir2", again.ReadString("Server", "Name", ""));
        Assert.Equal("7000", again.ReadString("Server", "Port", ""));
        Assert.Equal("cli", again.ReadString("Client", "Name", ""));
    }

    [Fact]
    public void LoadFromFile_MissingFile_LeavesEmptyAndMarksLoadOk()
    {
        // 原文 try/except 空处理 ⇒ 不抛；随后仍走 Get('') + FLoadOK := True
        var ini = Sample();
        ini.LoadFromFile(Tmp("does-not-exist.ini"));
        Assert.Equal(0, ini.Sections.Count);
        Assert.True(ini.LoadOKFlag);
    }

    [Fact]
    public void LoadFromFile_ReplacesPreviousContent()
    {
        var ini = Sample();
        string path = Tmp("s.ini");
        File.WriteAllText(path, "[Only]\r\nK=1", System.Text.Encoding.GetEncoding(936));
        ini.LoadFromFile(path);
        Assert.Equal(1, ini.Sections.Count);
        Assert.Equal("Only", ini.Sections[0]);
    }

    [Fact]
    public void OnChange_IsInvokedThroughChanged()
    {
        var ini = Sample();
        int n = 0;
        TMemoryIniFile? sender = null;
        ini.OnChange = s => { n++; sender = s; };
        ini.WriteString("Server", "Name", "X");     // SetStrings → 值表 OnChange → Changed
        Assert.Equal(1, n);
        Assert.Same(ini, sender);
    }

    [Fact]
    public void Dispose_ReleasesSectionsTable()
    {
        var ini = Sample();
        ini.Dispose();                              // 原文 Destroy：逐节 Free 后释放节表
        Assert.Null(ini.Sections);
    }

    [Fact]
    public void LoadFromFile_ResetsChangedFlag_WhenNothingToClear()
    {
        // 原文 LoadFromFile 开头 FChanged := False；随后 Sections.Clear 只在**非空**时触发 Changed
        // ⇒ 空表上 LoadFromFile 之后标志确实回到 False（非空表会被 Clear 的通知重新置 True）。
        var ini = new TMemoryIniFile();
        Assert.False(ini.ChangedFlag);
        ini.LoadFromFile(Tmp("none.ini"));
        Assert.False(ini.ChangedFlag);
        Assert.Equal(0, ini.Sections.Count);
    }

    [Fact]
    public void LoadFromFile_NonEmptyTable_ClearNotificationResetsFlagToTrue_OriginalFlaw()
    {
        // ★ 原文如此：LoadFromFile 先 FChanged := False，紧接着 Sections.Clear 又经 OnChange 置回 True
        var ini = Sample();
        Assert.True(ini.ChangedFlag);
        ini.WriteString("S", "K", "V");
        ini.LoadFromFile(Tmp("none.ini"));
        Assert.True(ini.ChangedFlag);
    }

    [Fact]
    public void SaveToFile_ResetsChangedFlag()
    {
        var ini = Sample();
        ini.WriteString("S", "K", "V");
        ini.SaveToFile(Tmp("out.ini"));
        Assert.False(ini.ChangedFlag);
    }
}

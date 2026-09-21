// ============================================================================
// NoticeM.pas（119 行）1:1 测试
//   构造（:32-42）/ 析构（:44-54）/ LoadingNotice（:56-75）/ GetNoticeMsg（:77-118）
//   ＋ 4 条原文缺陷的差异断言（见源文件头「原文缺陷」1-4）
//
// ★ 用**真实临时目录 + 真实文件**（GBK 写入）跑通/跑失败两条路径 ——
//   不用假 FileExists 掩盖 `LoadFromFile` 的真实行为。
// ============================================================================

using GXX.Core;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Sweep;
using GXX.M2Server.Sweep9.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("Sweep9FormsSerial")]
public sealed class Sweep9FormsNoticeMTests : IDisposable
{
    private readonly string Dir;

    public Sweep9FormsNoticeMTests()
    {
        Dir = Path.Combine(Path.GetTempPath(), "p9notice_" + Guid.NewGuid().ToString("N")) + "\\";
        Directory.CreateDirectory(Dir);
        M2Config.sNoticeDir = Dir;
        SweepSeam.ResetDefaults();
        Sweep9FormsNoticeGlobals.Reset();
    }

    public void Dispose()
    {
        SweepSeam.ResetDefaults();
        Sweep9FormsNoticeGlobals.Reset();
        try { Directory.Delete(Dir, true); } catch { /* best effort */ }
    }

    private void WriteNoticeFile(string name, params string[] lines)
        => File.WriteAllBytes(Dir + name + ".txt", EncodingInit.GBK.GetBytes(string.Join("\r\n", lines) + "\r\n"));

    // ------------------------------------------------------------------
    // 构造 / 析构
    // ------------------------------------------------------------------

    [Fact]
    public void Ctor_InitialisesAll100Entries()
    {
        var m = new TNoticeManager();
        for (int i = 0; i <= 99; i++)
        {
            Assert.Equal("", m.NoticeList[i].sMsg);
            Assert.Null(m.NoticeList[i].sList);
            Assert.True(m.NoticeList[i].bo0C);          // :40 bo0C := True
        }
    }

    [Fact]
    public void Bounds_MatchDelphiArrayRange()
    {
        // 原文 :14 `TNoticeList = array[0..99] of TNoticeMsg` ⇒ Low=0 / High=99
        Assert.Equal(0, TNoticeList.Low);
        Assert.Equal(99, TNoticeList.High);
    }

    [Fact]
    public void Dispose_ClearsEveryListReference()
    {
        var m = new TNoticeManager();
        for (int i = 0; i <= 99; i++)
            m.NoticeList[i].sList = new TStringList();
        m.Dispose();
        for (int i = 0; i <= 99; i++)
            Assert.Null(m.NoticeList[i].sList);
    }

    // ------------------------------------------------------------------
    // LoadingNotice（:56-75）
    // ------------------------------------------------------------------

    [Fact]
    public void LoadingNotice_SkipsEntriesWithEmptyMsg_AndDoesNotLoad()
    {
        var m = new TNoticeManager();
        m.LoadingNotice();                              // 100 项全空 ⇒ :63 Continue
        for (int i = 0; i <= 99; i++)
            Assert.Null(m.NoticeList[i].sList);
    }

    [Fact]
    public void LoadingNotice_MissingFile_LeavesListNull()
    {
        var m = new TNoticeManager();
        m.NoticeList[7].sMsg = "NoSuchNotice";

        m.LoadingNotice();

        Assert.Null(m.NoticeList[7].sList);             // :65 FileExists 为假 ⇒ 不建列表
    }

    [Fact]
    public void LoadingNotice_ExistingFile_LoadsGbkContent()
    {
        WriteNoticeFile("公告", "第一行", "第二行");
        var m = new TNoticeManager();
        m.NoticeList[3].sMsg = "公告";

        m.LoadingNotice();

        Assert.NotNull(m.NoticeList[3].sList);
        Assert.Equal(2, m.NoticeList[3].sList!.Count);
        Assert.Equal("第一行", m.NoticeList[3].sList![0]);
        Assert.Equal("第二行", m.NoticeList[3].sList![1]);
    }

    [Fact]
    public void LoadingNotice_ReusesExistingList_AndReloadsIntoIt()
    {
        WriteNoticeFile("N", "A");
        var m = new TNoticeManager();
        m.NoticeList[0].sMsg = "N";
        var pre = new TStringList();
        pre.Add("旧内容");
        m.NoticeList[0].sList = pre;

        m.LoadingNotice();

        // 原文 :68 `if sList = nil then Create` —— 非 nil 则**复用**，LoadFromFile 会覆盖内容
        Assert.Same(pre, m.NoticeList[0].sList);
        Assert.Equal("A", m.NoticeList[0].sList![0]);
    }

    [Fact]
    public void LoadingNotice_OnlyMsgEntriesTouched()
    {
        WriteNoticeFile("A", "1");
        WriteNoticeFile("C", "3");
        var m = new TNoticeManager();
        m.NoticeList[0].sMsg = "A";
        m.NoticeList[50].sMsg = "B";                    // 文件不存在
        m.NoticeList[99].sMsg = "C";

        m.LoadingNotice();

        Assert.NotNull(m.NoticeList[0].sList);
        Assert.Null(m.NoticeList[50].sList);
        Assert.NotNull(m.NoticeList[99].sList);
    }

    // ------------------------------------------------------------------
    // GetNoticeMsg（:77-118）
    // ------------------------------------------------------------------

    [Fact]
    public void GetNoticeMsg_NotRegistered_NoFile_ReturnsFalse()
    {
        var m = new TNoticeManager();
        var load = new TStringList();

        Assert.False(m.GetNoticeMsg("X", load));
        Assert.Equal(0, load.Count);
        // 未命中任何槽 ⇒ 第二段扫完 100 个空槽都因 FileExists 假而落空 ⇒ 仍全空
        Assert.Equal("", m.NoticeList[0].sMsg);
    }

    [Fact]
    public void GetNoticeMsg_NotRegistered_ExistingFile_RegistersAndLoads()
    {
        WriteNoticeFile("X", "内容一");
        var m = new TNoticeManager();
        var load = new TStringList();

        Assert.True(m.GetNoticeMsg("X", load));         // :113 Result := True
        Assert.Equal("X", m.NoticeList[0].sMsg);        // :112 登记进**第一个空槽**
        Assert.Equal(1, load.Count);                    // :108 LoadList.AddStrings(...)
        Assert.Equal("内容一", load[0]);
    }

    [Fact]
    public void GetNoticeMsg_Registered_AppendsListContents_ReturnsTrue()
    {
        WriteNoticeFile("Notice", "行1", "行2");
        var m = new TNoticeManager();
        m.NoticeList[10].sMsg = "Notice";
        m.NoticeList[10].sList = new TStringList();
        m.NoticeList[10].sList!.Add("行1");
        m.NoticeList[10].sList!.Add("行2");
        var load = new TStringList();

        Assert.True(m.GetNoticeMsg("Notice", load));
        Assert.Equal(2, load.Count);
        Assert.Equal("行1", load[0]);
        Assert.Equal("行2", load[1]);
        Assert.Equal("Notice", m.NoticeList[10].sMsg);  // bo15=False ⇒ 不走新登记
    }

    [Fact]
    public void GetNoticeMsg_CompareTextIsCaseInsensitive()
    {
        var m = new TNoticeManager();
        m.NoticeList[10].sMsg = "Notice";
        m.NoticeList[10].sList = new TStringList();
        m.NoticeList[10].sList!.Add("内容");
        var load = new TStringList();

        Assert.True(m.GetNoticeMsg("NOTICE", load));    // CompareText = 0
        Assert.Equal(1, load.Count);
    }

    [Fact]
    public void GetNoticeMsg_DuplicateRegistrations_AppendsAll_NoBreak()
    {
        // ★ 原文缺陷 1：:85-96 命中循环**没有 Break** ⇒ 两份都追加。
        var m = new TNoticeManager();
        m.NoticeList[1].sMsg = "Dup";
        m.NoticeList[1].sList = new TStringList();
        m.NoticeList[1].sList!.Add("A");
        m.NoticeList[2].sMsg = "dup";                   // 大小写不同也命中（CompareText）
        m.NoticeList[2].sList = new TStringList();
        m.NoticeList[2].sList!.Add("B");
        var load = new TStringList();

        Assert.True(m.GetNoticeMsg("DUP", load));
        Assert.Equal(2, load.Count);
        Assert.Equal("A", load[0]);
        Assert.Equal("B", load[1]);
    }

    [Fact]
    public void GetNoticeMsg_RegisteredButListNil_ReturnsFalse_AndSkipsNewRegistration()
    {
        // ★ 原文缺陷 2：命中项 sList = nil ⇒ Result 保持 False、bo15 := False
        //   ⇒ :97 `if not bo15 then Exit` 直接返回；**不再走"新登记"分支**
        //   （即使文件存在也不加载、不登记）⇒ 该名字被永久卡死。
        WriteNoticeFile("Stuck", "本该加载");
        var m = new TNoticeManager();
        m.NoticeList[4].sMsg = "Stuck";
        var load = new TStringList();

        Assert.False(m.GetNoticeMsg("Stuck", load));
        Assert.Equal(0, load.Count);
        Assert.Null(m.NoticeList[0].sList);             // 空槽没被占用
        Assert.Equal("", m.NoticeList[0].sMsg);
    }

    [Fact]
    public void GetNoticeMsg_LoadThrowsOutsideExcept_StillRegistersAndReturnsTrue()
    {
        // ★ 原文缺陷 3：:105-111 的 try/except 只包住"建列表 + LoadFromFile + AddStrings"，
        //   而 :112 登记 / :113 Result := True **在 except 之外**。
        //   构造"FileExists 为真但读取必抛"：路径指向一个**目录**（FileExists 打桩为真）。
        Directory.CreateDirectory(Dir + "AsDir.txt");
        SweepSeam.FileExists = p => p == Dir + "AsDir.txt";
        var m = new TNoticeManager();
        var load = new TStringList();

        Assert.True(m.GetNoticeMsg("AsDir", load));     // 异常被吞 ⇒ 仍返回 True
        Assert.Equal("AsDir", m.NoticeList[0].sMsg);    // 并且**已登记**
        Assert.NotNull(m.NoticeList[0].sList);
        Assert.Equal(0, load.Count);                    // 但内容为空
    }

    [Fact]
    public void GetNoticeMsg_FillsFirstEmptySlotOnly_AndBreaks()
    {
        WriteNoticeFile("New", "N");
        var m = new TNoticeManager();
        m.NoticeList[0].sMsg = "占位";                   // 0 号非空 ⇒ 从 1 号开始找
        var load = new TStringList();

        Assert.True(m.GetNoticeMsg("New", load));
        Assert.Equal("New", m.NoticeList[1].sMsg);       // :114 Break ⇒ 只占一个槽
        Assert.Equal("", m.NoticeList[2].sMsg);
    }

    [Fact]
    public void GetNoticeMsg_AllSlotsFull_AndNameNotRegistered_ReturnsFalse()
    {
        WriteNoticeFile("Brand", "x");
        var m = new TNoticeManager();
        for (int i = 0; i <= 99; i++)
            m.NoticeList[i].sMsg = "Z" + i;
        var load = new TStringList();

        Assert.False(m.GetNoticeMsg("Brand", load));
        Assert.Equal(0, load.Count);
    }

    [Fact]
    public void GetNoticeMsg_RegisteredButListEmpty_ReturnsTrueWithNoContent()
    {
        // 命中且 sList <> nil（但为空表）⇒ Result := True、bo15 := False、不追加任何行。
        var m = new TNoticeManager();
        m.NoticeList[9].sMsg = "Empty";
        m.NoticeList[9].sList = new TStringList();
        var load = new TStringList();

        Assert.True(m.GetNoticeMsg("Empty", load));
        Assert.Equal(0, load.Count);
    }

    // ------------------------------------------------------------------
    // 全局接缝 / 扩展方法
    // ------------------------------------------------------------------

    [Fact]
    public void NoticeManagerGlobal_IsNullUntilWired()
    {
        // 原文 M2Share.pas:3685 声明、svMain.pas:1945 创建 ⇒ 托管侧接缝未接线时为 null
        Assert.Null(Sweep9FormsNoticeGlobals.NoticeManager);
        var m = new TNoticeManager();
        Sweep9FormsNoticeGlobals.NoticeManager = m;
        Assert.Same(m, Sweep9FormsNoticeGlobals.NoticeManager);
        Sweep9FormsNoticeGlobals.Reset();
        Assert.Null(Sweep9FormsNoticeGlobals.NoticeManager);
    }

    [Fact]
    public void AddStrings_Extension_MatchesDelphiSemantics()
    {
        var a = new TStringList();
        a.AddObject("x", "obj-x");
        a.Add("y");
        var b = new TStringList();
        b.AddStrings(a);                                  // 扩展方法（与 TNoticeManager 同文件）
        Assert.Equal(2, b.Count);
        Assert.Equal("x", b[0]);
        Assert.Equal("obj-x", b.GetObject(0));            // Objects[] 一并搬运（Delphi AddStrings）
        Assert.Equal("y", b[1]);
    }

    [Fact]
    public void Bo0C_IsDeadField_NeverReadOrWrittenByTheUnit()
    {
        // ★ 原文缺陷 4：`bo0C`（:12）在构造里被置 True 之后，全单元 0 处读取/写入。
        //   用计数取证：本测试文件外，源码里对 bo0C 的引用只应出现在
        //   TNoticeMsg 声明 + 构造赋值 + 注释三处 ⇒ 此处断言"改它不影响任何行为"。
        var m = new TNoticeManager();
        var before = m.GetNoticeMsg("Nothing", new TStringList());
        m.NoticeList[0].bo0C = false;
        var after = m.GetNoticeMsg("Nothing", new TStringList());
        Assert.Equal(before, after);
        Assert.False(m.NoticeList[0].bo0C);
    }
}

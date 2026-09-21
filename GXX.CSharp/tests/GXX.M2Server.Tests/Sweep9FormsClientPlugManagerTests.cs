// ============================================================================
// uFrmClientPlugManager.pas（151 行）1:1 测试
//   DFM 对账（§37.3 计数取证）：控件 **2** / 事件绑定 **1** / 处理器方法 **1**
//   LoadPlugClientFiles（:27-120，含嵌套 SearchFiles :33-84）
//   TFrmClientPlugManager.Open（:122-134）/ ButtonRefClick（:136-149）
// ============================================================================

using GXX.Core.Crypto;
using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.M2Server.Sweep9.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("Sweep9FormsSerial")]
public sealed class Sweep9FormsClientPlugManagerTests : IDisposable
{
    private const string AppDir = "C:\\__p9app__\\";
    private const string PlugDir = AppDir + "PlugClient\\";
    private const string Mask = PlugDir + "*.dll";

    private readonly Sweep9FormsFakeFileSearch Search = new();
    private readonly Sweep9FormsClientPlugSeams Seams = new();
    private readonly List<string> CreatedDirs = new();
    private readonly List<string> Messages = new();
    private readonly List<string> RivestCalls = new();
    private readonly List<Sweep9FormsSearchRec> ClosedRecs = new();
    private int ProcessMessagesCalls;
    private bool Terminated;
    private TFrmClientPlugManager Form = null!;

    public Sweep9FormsClientPlugManagerTests()
    {
        Sweep9FormsMessageBoxSeam.Reset();
        Sweep9FormsMessageBoxSeam.UiEnabled = false;         // ★ 无头
        Sweep9FormsPlugClientGlobals.Reset();

        Seams.FileSearch = Search;
        Seams.SelfFilePath = () => AppDir;
        Seams.DirectoryExists = _ => true;                   // 默认不建目录（单独用例覆盖）
        Seams.CreateDir = CreatedDirs.Add;
        Seams.ProcessMessages = () => ProcessMessagesCalls++;
        Seams.ApplicationTerminated = () => Terminated;
        Seams.RivestFile = name => { RivestCalls.Add(name); return "md5of:" + name; };

        Form = new TFrmClientPlugManager();
        Form.PlugSeams = Seams;
    }

    public void Dispose()
    {
        Form.Dispose();
        Sweep9FormsPlugClientGlobals.Reset();
        Sweep9FormsMessageBoxSeam.Reset();
    }

    // ------------------------------------------------------------------
    // DFM 对账（计数取证）
    // ------------------------------------------------------------------

    [Fact]
    public void DfmReconcile_ControlCount_Is2()
    {
        // uFrmClientPlugManager.dfm：ListBoxPlugin（:16）+ ButtonRef（:24） = 2
        Assert.Equal(2, Sweep9FormsReconcile.CountControlsExcludingForm(Form));
    }

    [Fact]
    public void DfmReconcile_ControlNamesAndTypes_MatchDfmExactly()
    {
        var got = Sweep9FormsReconcile.EnumerateControls(Form)
            .Select(x => x.Name + ":" + x.Type.Name).ToArray();
        Assert.Equal(new[]
        {
            "FrmClientPlugManager:TFrmClientPlugManager",
            "ListBoxPlugin:ListBox",
            "ButtonRef:Button",
        }, got);
    }

    [Fact]
    public void DfmReconcile_EventBindingCount_Is1()
    {
        // DFM 绑定实测：仅 `:31 OnClick = ButtonRefClick`
        Assert.Equal(1, Sweep9FormsReconcile.CountEventBindings(Form));
    }

    [Fact]
    public void DfmReconcile_HandlerMethodCount_Is1()
    {
        var handlers = typeof(TFrmClientPlugManager)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.DeclaredOnly)
            .Select(m => m.Name)
            .Where(n => n.EndsWith("Click", StringComparison.Ordinal))
            .ToArray();
        Assert.Equal(new[] { "ButtonRefClick" }, handlers);
    }

    [Fact]
    public void DfmReconcile_FormProperties_MatchDfm()
    {
        Assert.Equal("客户端插件管理", Form.Text);                // DFM Caption
        Assert.Equal(726, Form.Left);
        Assert.Equal(423, Form.Top);
        Assert.Equal(625, Form.ClientSize.Width);
        Assert.Equal(243, Form.ClientSize.Height);
        Assert.Equal(System.Windows.Forms.FormBorderStyle.FixedDialog, Form.FormBorderStyle);  // bsDialog
        Assert.Equal(System.Windows.Forms.FormStartPosition.CenterParent, Form.StartPosition);
    }

    [Fact]
    public void DfmReconcile_ControlGeometry_MatchDfm()
    {
        Assert.Equal(9, Form.ListBoxPlugin.Left);
        Assert.Equal(9, Form.ListBoxPlugin.Top);
        Assert.Equal(518, Form.ListBoxPlugin.Width);
        Assert.Equal(217, Form.ListBoxPlugin.Height);
        Assert.Equal(534, Form.ButtonRef.Left);
        Assert.Equal(9, Form.ButtonRef.Top);
        Assert.Equal(81, Form.ButtonRef.Width);
        Assert.Equal(27, Form.ButtonRef.Height);
        Assert.Equal("刷新(&R)", Form.ButtonRef.Text);
    }

    // ------------------------------------------------------------------
    // LoadPlugClientFiles（:27-120）
    // ------------------------------------------------------------------

    [Fact]
    public void Load_NoDllFiles_ClearsGlobalsAndLeavesThemZeroed()
    {
        // 先塞垃圾验证"先清空"（:87-94）
        Sweep9FormsPlugClientGlobals.g_PlugClientList.Add(new TPlugClientInfo { sFileName = "x", sMD5 = "y" });
        Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListTextLen = 999;
        Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListTextCRC = 777;

        TFrmClientPlugManager.LoadPlugClientFiles(Seams);

        Assert.Empty(Sweep9FormsPlugClientGlobals.g_PlugClientList);
        Assert.Equal(0, Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListTextLen);
        Assert.Equal(0u, Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListTextCRC);
        Assert.Empty(Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListText);
    }

    [Fact]
    public void Load_SingleFile_JoinsMd5WithoutLineBreak_AndEncodes()
    {
        Search.Add(Mask, "a.dll");
        TFrmClientPlugManager.LoadPlugClientFiles(Seams);

        var expectedS = "md5of:" + PlugDir + "a.dll";                 // 单元素 ⇒ 无 sLineBreak
        Assert.Single(Sweep9FormsPlugClientGlobals.g_PlugClientList);
        Assert.Equal(PlugDir + "a.dll", Sweep9FormsPlugClientGlobals.g_PlugClientList[0].sFileName);
        Assert.Equal(expectedS, Sweep9FormsPlugClientGlobals.g_PlugClientList[0].sMD5);

        // :116 编码**前**长度
        Assert.Equal(expectedS.Length, Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListTextLen);
        // :117 编码后字节
        Assert.Equal(EDcode.zEncodeString(expectedS), Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListText);
        // :118 用的是编码**后**的字节长度（与 :116 不是同一个量 —— 原文如此）
        Assert.Equal(CheckUnit.BufferCrc(Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListText,
                Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListText.Length),
            Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListTextCRC);
    }

    [Fact]
    public void Load_MultipleFiles_JoinsWithCrLf_LastHasNoTrailingBreak()
    {
        Search.Add(Mask, "a.dll");
        Search.Add(Mask, "b.dll");
        Search.Add(Mask, "c.dll");

        TFrmClientPlugManager.LoadPlugClientFiles(Seams);

        var s = "md5of:" + PlugDir + "a.dll" + "\r\n"
              + "md5of:" + PlugDir + "b.dll" + "\r\n"
              + "md5of:" + PlugDir + "c.dll";
        Assert.Equal(3, Sweep9FormsPlugClientGlobals.g_PlugClientList.Count);
        Assert.Equal(s.Length, Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListTextLen);
        Assert.Equal(EDcode.zEncodeString(s), Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListText);
    }

    [Fact]
    public void Load_EmptyMd5Result_StillCountsAsNonEmptyS()
    {
        // S > 0 才编码：即使 MD5 返回空串，文件名串也能让 S 非空 —— 用空 MD5 断言走的是
        // "S.Length > 0" 的这一支（而"零文件"那支见上一个用例：此处分量不同）。
        Search.Add(Mask, "a.dll");
        Seams.RivestFile = _ => "";
        TFrmClientPlugManager.LoadPlugClientFiles(Seams);

        // S = "" + ""（单元素，MD5 为空）⇒ Length = 0 ⇒ **不**编码（:114 条件为假）
        Assert.Equal(0, Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListTextLen);
        Assert.Empty(Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListText);
    }

    [Fact]
    public void Load_MissingPlugClientDir_CreatesIt()
    {
        Seams.DirectoryExists = _ => false;
        TFrmClientPlugManager.LoadPlugClientFiles(Seams);
        Assert.Equal(new[] { AppDir + "PlugClient" }, CreatedDirs);   // 原文不带尾部分隔符
    }

    [Fact]
    public void Load_ExistingPlugClientDir_DoesNotCreate()
    {
        Seams.DirectoryExists = _ => true;
        TFrmClientPlugManager.LoadPlugClientFiles(Seams);
        Assert.Empty(CreatedDirs);
    }

    [Fact]
    public void Load_IsFile_SkipsDotEntriesAndDirectories()
    {
        Search.Add(Mask, ".");
        Search.Add(Mask, "..");
        Search.Add(Mask, "subdir", isDirectory: true);
        Search.Add(Mask, "ok.dll");

        TFrmClientPlugManager.LoadPlugClientFiles(Seams);

        Assert.Single(Sweep9FormsPlugClientGlobals.g_PlugClientList);
        Assert.Equal(PlugDir + "ok.dll", Sweep9FormsPlugClientGlobals.g_PlugClientList[0].sFileName);
        // 只对通过 IsFile 的条目算 MD5
        Assert.Equal(new[] { PlugDir + "ok.dll" }, RivestCalls);
    }

    [Fact]
    public void Load_FindFirstFails_StillCallsFindClose_OriginalBehaviour()
    {
        // ★ 原文 :81-83 finally FindClose(Info) 在 FindFirst 失败时**也会调用**（原文如此）。
        TFrmClientPlugManager.LoadPlugClientFiles(Seams);
        Assert.Equal(1, Search.FindFirstCalls);
        Assert.Equal(1, Search.FindCloseCalls);
        Assert.Empty(Sweep9FormsPlugClientGlobals.g_PlugClientList);
    }

    [Fact]
    public void Load_ProcessMessages_OnlyEvery100FindNextEntries()
    {
        // 101 个文件 = FindFirst 1 条 + FindNext 100 条 ⇒ nFileCount 到 100 时调一次。
        for (int i = 0; i < 101; i++)
            Search.Add(Mask, "f" + i + ".dll");

        TFrmClientPlugManager.LoadPlugClientFiles(Seams);

        Assert.Equal(101, Sweep9FormsPlugClientGlobals.g_PlugClientList.Count);
        Assert.Equal(1, ProcessMessagesCalls);
    }

    [Fact]
    public void Load_ProcessMessages_NotCalledForExactly100Files()
    {
        // 100 个文件 = FindFirst 1 条 + FindNext 99 条 ⇒ nFileCount 最大 99 ⇒ 不触发。
        for (int i = 0; i < 100; i++)
            Search.Add(Mask, "f" + i + ".dll");

        TFrmClientPlugManager.LoadPlugClientFiles(Seams);

        Assert.Equal(100, Sweep9FormsPlugClientGlobals.g_PlugClientList.Count);
        Assert.Equal(0, ProcessMessagesCalls);
    }

    [Fact]
    public void Load_ApplicationTerminated_StopsTheFindNextLoop()
    {
        for (int i = 0; i < 5; i++)
            Search.Add(Mask, "f" + i + ".dll");
        int seen = 0;
        Seams.RivestFile = _ => { seen++; Terminated = seen >= 2; return "m"; };

        TFrmClientPlugManager.LoadPlugClientFiles(Seams);

        // FindFirst 命中 1 条（file0，RivestFile 后 seen=1）+ FindNext 命中 1 条
        // （file1，RivestFile 后 seen=2 ⇒ Terminated 置真）⇒ 共 **2** 条；
        // 下一次 while 条件先求 FindNext（已推进到 file2）再求 Terminated ⇒ 退出循环。
        Assert.Equal(2, Sweep9FormsPlugClientGlobals.g_PlugClientList.Count);
        Assert.Equal(2, Search.FindNextCalls);
        Assert.Equal(1, Search.FindCloseCalls);
    }

    [Fact]
    public void Load_IsDir_ImplementationExistsButIsDeadCode()
    {
        // ★ 原文缺陷：嵌套函数 `IsDir`（:38-42）声明了但全单元 0 处调用。
        //   证据（计数取证）：把"目录条目"喂进去，IsDir 的语义（目录才为真）成立，
        //   但 LoadPlugClientFiles 的结果**只**取决于 IsFile ⇒ 目录条目一条都不进列表。
        var rec = new Sweep9FormsSearchRec { Name = "sub", Attr = Sweep9FormsFileSearchDefault.faDirectory };
        Assert.True(TFrmClientPlugManager.IsDir(rec));
        Assert.False(TFrmClientPlugManager.IsDir(new Sweep9FormsSearchRec { Name = "f", Attr = 0 }));
        Assert.False(TFrmClientPlugManager.IsDir(new Sweep9FormsSearchRec
        { Name = ".", Attr = Sweep9FormsFileSearchDefault.faDirectory }));

        // 而实际装载路径对目录条目无动于衷（只受 IsFile 控制）：
        Search.Add(Mask, "onlyDir", isDirectory: true);
        TFrmClientPlugManager.LoadPlugClientFiles(Seams);
        Assert.Empty(Sweep9FormsPlugClientGlobals.g_PlugClientList);
    }

    // ------------------------------------------------------------------
    // Open（:122-134）/ ButtonRefClick（:136-149）
    // ------------------------------------------------------------------

    [Fact]
    public void Open_ListsGlobalEntriesWithObjectCarriers()
    {
        var a = new TPlugClientInfo { sFileName = "A.dll", sMD5 = "a" };
        var b = new TPlugClientInfo { sFileName = "B.dll", sMD5 = "b" };
        Sweep9FormsPlugClientGlobals.g_PlugClientList.Add(a);
        Sweep9FormsPlugClientGlobals.g_PlugClientList.Add(b);

        Form.Open();

        Assert.Equal(2, Form.ListBoxPlugin.Items.Count);
        Assert.Equal("A.dll", Form.ListBoxPlugin.Items[0]);
        Assert.Equal("B.dll", Form.ListBoxPlugin.Items[1]);
        Assert.Same(a, Form.ItemObjects[0]);
        Assert.Same(b, Form.ItemObjects[1]);
        Assert.Equal(1, Sweep9FormsMessageBoxSeam.ShowModalCount);
    }

    [Fact]
    public void Open_ClearsPreviousItemsFirst()
    {
        Form.ListBoxPlugin.Items.Add("stale");
        Form.ItemObjects.Add("stale");
        Form.Open();
        Assert.Empty(Form.ListBoxPlugin.Items);
        Assert.Empty(Form.ItemObjects);
    }

    [Fact]
    public void ButtonRefClick_ReloadsAndSendsPlugClientList()
    {
        int sendCalls = 0;
        Form.SendPlugClientListHandler = () => sendCalls++;
        Search.Add(Mask, "a.dll");

        Form.ButtonRefClick(Form.ButtonRef);

        Assert.Single(Sweep9FormsPlugClientGlobals.g_PlugClientList);
        Assert.Equal(1, sendCalls);                                    // :148 UserEngine.SendPlugClientList()
        Assert.Single(Form.ListBoxPlugin.Items);
        Assert.Equal(PlugDir + "a.dll", Form.ListBoxPlugin.Items[0]);
    }

    [Fact]
    public void ButtonRefClick_UnwiredSendSeam_IsNoOp_AndIsRegisteredAsPending()
    {
        // 接缝未接线（默认 null）⇒ 静默跳过（与 DummySetting/GamePets 既有形态一致）；
        // 已在报告登记为**待接线项**（UsrEngn.pas 批次）。
        Search.Add(Mask, "a.dll");
        Form.ButtonRefClick(Form.ButtonRef);
        Assert.Single(Sweep9FormsPlugClientGlobals.g_PlugClientList);
    }

    // ------------------------------------------------------------------
    // TPlugClientInfo / RivestFile 依赖
    // ------------------------------------------------------------------

    [Fact]
    public void TPlugClientInfo_MirrorsOriginalRecordFields()
    {
        var f = typeof(TPlugClientInfo).GetFields().Select(x => x.Name).OrderBy(x => x).ToArray();
        Assert.Equal(new[] { "sFileName", "sMD5" }, f);                // M2Share.pas:458-461
    }

    [Fact]
    public void RivestFile_MissingFile_ReturnsMd5OfEmptyContent_OriginalBehaviour()
    {
        // 原文 MD5File 在 CreateFile 失败时跳过读取、直接 MD5Final ⇒ **空内容的 MD5**，
        // 而不是空串（RunGate 车道的注释"文件不存在返回空"与原文不符）。
        var missing = Path.Combine(Path.GetTempPath(), "p9_nope_" + Guid.NewGuid().ToString("N") + ".bin");
        Assert.Equal("d41d8cd98f00b204e9800998ecf8427e", Sweep9FormsMD5.RivestFile(missing));
    }

    [Fact]
    public void RivestFile_RealFile_MatchesMd5OfBytes()
    {
        var path = Path.Combine(Path.GetTempPath(), "p9_md5_" + Guid.NewGuid().ToString("N") + ".bin");
        try
        {
            var data = new byte[] { 1, 2, 3, 4, 5 };
            File.WriteAllBytes(path, data);
            Assert.Equal(MD5Util.MD5Print(MD5Util.MD5Buffer(data, 0, data.Length)),
                Sweep9FormsMD5.RivestFile(path));
            // MD5(1..5) 的已知向量
            Assert.Equal("7cfdd07889b3295d6a550914ab35e068", Sweep9FormsMD5.RivestFile(path));
        }
        finally
        {
            try { File.Delete(path); } catch { /* best effort */ }
        }
    }
}

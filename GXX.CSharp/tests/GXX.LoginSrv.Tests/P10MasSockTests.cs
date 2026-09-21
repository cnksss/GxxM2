// ============================================================================
// P10MasSockTests —— MasSock.pas（1017 行）→ Forms/MasSock.cs 的逐成员用例
//
// DFM 对账期望值（Source/LoginSrv/MasSock.dfm，**二进制 DFM** 实测解码）：
//   Objects = 2（TFrmMasSoc 根 + MSocket: TServerSocket）
//   Events  = 6（窗体 OnCreate/OnDestroy + MSocket OnClientConnect/Disconnect/Read/Error）
// 计数一律走 P10FormReconcile（台账 §37.3 / §41.3），本文件**不**自写反射。
//
// 本类触碰静态接缝（MasSockGlobals / LoginSrvShare / LoginSrvForms）⇒
// 归入 [Collection("LoginSrvSequential")]（DisableParallelization = true，
// 定义在 P10GrobalSessionTests.cs，同一程序集内共用）。
//
// 无任何真实网络/数据路径：文件类接缝一律指向 Path.GetTempPath() 下的临时目录。
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.LoginSrv.Tests;
using Xunit;

namespace GXX.LoginSrv.Forms.Tests;

[Collection("LoginSrvSequential")]
public sealed class P10MasSockTests : IDisposable
{
    private readonly string _dir;

    public P10MasSockTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "p10-massock-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);

        TFrmMasSoc.SocketFactory = null;
        // Application.MessageBox 接缝：不弹窗（LoadUserLimit 的"文件缺失"分支会走到这里）
        LoginSrvForms.MessageBoxHandler = (t, c, f) => LoginSrvForms.IDOK;
        LoginSrvForms.NextAnswer = null;

        MasSockGlobals.ResetForTests();
        LoginSrvShare.ResetForTests();

        // 路径接缝必须在 ResetForTests **之后**设置（Reset 会把它们还原成原文字面量）
        MasSockGlobals.ServerAddrFileName = Path.Combine(_dir, "!ServerAddr.txt");
        MasSockGlobals.UserLimitFileName = Path.Combine(_dir, "!UserLimit.txt");
    }

    public void Dispose()
    {
        TFrmMasSoc.SocketFactory = null;
        LoginSrvForms.MessageBoxHandler = null;
        LoginSrvForms.NextAnswer = null;
        LoginSrvForms.LastMessage = null;
        LoginSrvForms.LastCaption = null;
        MasSockGlobals.ResetForTests();
        LoginSrvShare.ResetForTests();
        try { Directory.Delete(_dir, true); } catch { /* 临时目录清理失败不影响断言 */ }
    }

    // ========================================================================
    // 测试替身 / 工具
    // ========================================================================

    /// <summary>允许 Active := True 的 TServerSocket 替身（默认接缝在 True 时显式抛"未接线"）。</summary>
    private sealed class TTestServerSocket : TServerSocket
    {
        public override bool Active { get => ActiveState; set => ActiveState = value; }
    }

    /// <summary>UpdateAccount 恒失败的库替身（覆盖原文 nErrCode := -20 分支）。</summary>
    private sealed class TUpdateFailDb : TAccountDB
    {
        private readonly TAccountInfo _row;

        public TUpdateFailDb(string account) : base("")
        {
            _row = new TAccountInfo();
            _row.AccountNameStr = account;
        }

        protected override void DoInit() { }
        protected override void DoFinal() { }
        protected override bool DoGetAccountByQuick(string UID, string ID, ref TAccountInfo AccountInfo) { AccountInfo = _row; return true; }
        protected override bool DoGetAccountByPhone(string Phone, ref TAccountInfo AccountInfo) { AccountInfo = _row; return true; }
        protected override bool DoGetAccount(string AccountName, ref TAccountInfo AccountInfo) { AccountInfo = _row; return true; }
        protected override int DoFindAccount(string AccountName, TAccountList AccountList) => 0;
        protected override bool DoUpdateAccount(TAccountInfo AccountInfo, TAccountUpdateField UpdateField) => false;  // ★ 恒失败
        protected override void DoGetAllAccount(TStringList AccountList) { }
        protected override bool DoEnabledAccounts(TStringList AccountList, bool Enabled) => false;
        protected override bool DoCheckAccountExists(string AccountName) => true;
        protected override bool DoAddAccount(TAccountInfo AccountInfo) => false;
        protected override bool DoUnLockAccount(string AccountName) => false;
    }

    private TFrmMasSoc NewForm()
    {
        var f = new TFrmMasSoc();
        f.FormCreate(f);
        return f;
    }

    /// <summary>往 m_ServerList 里塞一个"已连接"的服务器条目。</summary>
    private static TMsgServerInfo AddServer(TFrmMasSoc f, string name, int index, int online, string ip)
    {
        var sock = new TCustomWinSocket { RemoteAddress = ip, Connected = true };
        var info = new TMsgServerInfo
        {
            Socket = sock,
            sServerName = name,
            nServerIndex = index,
            nOnlineCount = online,
            sIPaddr = ip,
        };
        f.m_ServerList.Add(info);
        return info;
    }

    private static string FileText(string[] lines)
        => lines.Length == 0 ? "" : string.Join("\r\n", lines) + "\r\n";

    /// <summary>按 GBK（= Delphi AnsiString/系统 ANSI 代码页）写文件，与 TStringList.LoadFromFile 的嗅探回落一致。</summary>
    private void WriteServerAddrFile(params string[] lines)
        => File.WriteAllText(MasSockGlobals.ServerAddrFileName, FileText(lines), EncodingInit.GBK);

    private void WriteUserLimitFile(params string[] lines)
        => File.WriteAllText(MasSockGlobals.UserLimitFileName, FileText(lines), EncodingInit.GBK);

    private static InMemoryAccountDb DbWithAccount(string account, string password = "oldpw")
    {
        var db = new InMemoryAccountDb();
        var info = new TAccountInfo();
        info.AccountNameStr = account;
        info.PasswordStr = password;
        db.Store[account] = info;
        LoginSrvShare.g_AccountDB = db;
        return db;
    }

    /// <summary>
    /// 按 Grobal2.Types6.cs 的 TAccountInfo2 偏移拼出 218 字节的 packed 记录并 6-bit 编码
    /// （偏移与 Delphi packed record 一致：16 / 31 / 42 / 63 / 74 / 95 / 108 / 129 / 142 / 156 / 197）。
    /// </summary>
    private static string EncodeAccountInfo2Record(
        string account,
        string password,
        string userName = "u1",
        string birthDay = "19800101",
        string q1 = "q1",
        string a1 = "a1",
        string q2 = "q2",
        string a2 = "a2",
        string mobile = "13800000000",
        string mail = "m@x.com",
        string l2 = "")
    {
        byte[] buf = new byte[MasSockFns.TAccountInfo2PackedSize];
        ShortStr.Set(buf, 16, 14, account);
        ShortStr.Set(buf, 31, 10, password);
        ShortStr.Set(buf, 42, 20, userName);
        ShortStr.Set(buf, 63, 10, birthDay);
        ShortStr.Set(buf, 74, 20, q1);
        ShortStr.Set(buf, 95, 12, a1);
        ShortStr.Set(buf, 108, 20, q2);
        ShortStr.Set(buf, 129, 12, a2);
        ShortStr.Set(buf, 142, 13, mobile);
        ShortStr.Set(buf, 156, 40, mail);
        ShortStr.Set(buf, 197, 20, l2);
        return EncodingInit.GBK.GetString(EDcode.EncodeBuffer(buf, buf.Length));
    }

    /// <summary>发一帧 SS_ChangeAccountInfo，返回**本次**回包（无回包返回 ""）。</summary>
    private static string SendChangeAccountInfo(TFrmMasSoc f, TMsgServerInfo t, string encoded)
    {
        int before = t.Socket.SentTexts.Count;
        t.Socket.ReceiveText = "(1135/" + encoded + ")";
        f.MSocketClientRead(f, t.Socket);
        return t.Socket.SentTexts.Count > before ? t.Socket.SentTexts[t.Socket.SentTexts.Count - 1] : "";
    }

    // ========================================================================
    // DFM 对账（台账 §37.3 / §41.3：一律计数取证）
    // ========================================================================

    [Fact]
    public void Dfm_ObjectCount_IsTwo()
    {
        using var f = new TFrmMasSoc();
        Assert.Equal(2, P10FormReconcile.CountDfmObjects(f));      // 窗体自身 + MSocket
        Assert.Equal(1, P10FormReconcile.CountChildrenOf(f));
    }

    [Fact]
    public void Dfm_ObjectNames_MatchDfm()
    {
        using var f = new TFrmMasSoc();
        var names = P10FormReconcile.EnumerateDfmObjects(f).Select(x => x.Name).ToList();
        // 根节点无 Name ⇒ 工具回落到类名（与 GrobalSession 车道同一口径）
        Assert.Equal(new List<string> { "TFrmMasSoc", "MSocket" }, names);
        Assert.Same(f.MSocket, P10FormReconcile.FindByName(f, "MSocket"));
    }

    [Fact]
    public void Dfm_EventBindingCount_IsSix()
    {
        using var f = new TFrmMasSoc();
        Assert.Equal(6, P10FormReconcile.CountEventBindings(f));
    }

    [Fact]
    public void Dfm_FormHasExactlyTwoBindings_OnCreateAndOnDestroy()
    {
        using var f = new TFrmMasSoc();
        Assert.Equal(2, P10FormReconcile.CountEventBindingsOn(f));
        Assert.True(P10FormReconcile.IsBound(f, "Load"));         // DFM OnCreate = FormCreate
        Assert.True(P10FormReconcile.IsBound(f, "FormClosed"));   // DFM OnDestroy = FormDestroy
    }

    [Fact]
    public void Dfm_MSocketHasExactlyFourBindings()
    {
        using var f = new TFrmMasSoc();
        Assert.Equal(4, P10FormReconcile.CountEventBindingsOn(f.MSocket));
        Assert.True(P10FormReconcile.IsBound(f.MSocket, "OnClientConnect"));
        Assert.True(P10FormReconcile.IsBound(f.MSocket, "OnClientDisconnect"));
        Assert.True(P10FormReconcile.IsBound(f.MSocket, "OnClientRead"));
        Assert.True(P10FormReconcile.IsBound(f.MSocket, "OnClientError"));
    }

    [Fact]
    public void Dfm_FormProperties_MatchDfm()
    {
        using var f = new TFrmMasSoc();
        Assert.Equal("FrmMasSoc", f.Text);                        // Caption = 'FrmMasSoc'
        Assert.Equal(new System.Drawing.Size(137, 107), f.ClientSize);
        Assert.Equal(new System.Drawing.Point(780, 172), f.Location);
    }

    [Fact]
    public void Dfm_MSocketProperties_MatchDfm()
    {
        using var f = new TFrmMasSoc();
        Assert.False(f.MSocket.Active);                            // Active = False
        Assert.Equal("0.0.0.0", f.MSocket.Address);                // Address = '0.0.0.0'
        Assert.Equal(0, f.MSocket.Port);                           // Port = 0
        Assert.Equal(TServerType.stNonBlocking, f.MSocket.ServerType);
        Assert.Equal(40, f.MSocket.Left);                          // Left = 40
        Assert.Equal(32, f.MSocket.Top);                           // Top = 32
    }

    // ========================================================================
    // DFM 的 6 条绑定确实接上了（经接缝的 Raise* 驱动）
    // ========================================================================

    [Fact]
    public void Dfm_Bindings_ActuallyInvokeFormHandlers()
    {
        using var f = NewForm();
        // ⚠ 地址表必须在 FormCreate（内部 LoadServerAddr 会 FillChar 清空数组）之后设置
        MasSockGlobals.g_ServerAddr[0] = "1.2.3.4";
        MasSockGlobals.g_ServerAddrCount = 1;

        var sock = new TCustomWinSocket { RemoteAddress = "1.2.3.4", Connected = true };

        f.MSocket.RaiseClientConnect(sock);                        // → MSocketClientConnect
        Assert.Single(f.m_ServerList);
        Assert.Same(sock, f.m_ServerList[0].Socket);

        f.MSocket.RaiseClientRead(sock);                           // → MSocketClientRead（空文本，无副作用）
        Assert.Empty(sock.SentTexts);

        // OnClientError：事件参数初值给 99，只有处理器（MSocketClientError）跑过才会变 0
        int code = f.MSocket.RaiseClientError(sock, TErrorEvent.eeReceive, 99);
        Assert.Equal(0, code);
        Assert.True(sock.CloseCalled);

        f.MSocket.RaiseClientDisconnect(sock);                     // → MSocketClientDisconnect
        Assert.Empty(f.m_ServerList);
    }

    // ========================================================================
    // 单元级全局与 ResetForTests
    // ========================================================================

    [Fact]
    public void MasSockGlobals_ResetForTests_RestoresDelphiInitialValues()
    {
        MasSockGlobals.FrmMasSoc = null;
        MasSockGlobals.nUserLimit = 7;
        MasSockGlobals.UserLimit[3].sServerName = "junk";
        MasSockGlobals.g_ServerAddr[0] = "1.2.3.4";
        MasSockGlobals.g_ServerAddrCount = 4;
        MasSockGlobals.nOnlineCountMin = 5;
        MasSockGlobals.nOnlineCountMax = 6;
        MasSockGlobals.GetSessionIDHandler = () => 1;
        MasSockGlobals.CloseUserHandler = (a, b, c) => { };

        MasSockGlobals.ResetForTests();

        Assert.Equal(0, MasSockGlobals.nUserLimit);                 // MasSock.pas:58 初值
        Assert.Equal(100, MasSockGlobals.UserLimit.Length);         // array[0..99]
        Assert.Equal("", MasSockGlobals.UserLimit[3].sServerName);  // 静态数组零初始化
        Assert.Equal(0, MasSockGlobals.UserLimit[3].nLimitCountMax);
        Assert.Equal("", MasSockGlobals.g_ServerAddr[0]);           // LSShare.pas:370 空串（不是 null）
        Assert.Equal(0, MasSockGlobals.g_ServerAddrCount);          // LSShare.pas:371 = 0
        Assert.Equal(0, MasSockGlobals.nOnlineCountMin);
        Assert.Equal(0, MasSockGlobals.nOnlineCountMax);
        Assert.Null(MasSockGlobals.GetSessionIDHandler);
        Assert.Null(MasSockGlobals.CloseUserHandler);
        Assert.Equal(@".\!ServerAddr.txt", MasSockGlobals.ServerAddrFileName);   // 原文字面量
        Assert.Equal(@".\!UserLimit.txt", MasSockGlobals.UserLimitFileName);
    }

    [Fact]
    public void MasSockGlobals_UnwiredGetSessionID_ThrowsExplicitly()
    {
        // 台账 §25.2：接缝不得静默返回中性值（0 会被当合法会话号）
        var ex = Assert.Throws<InvalidOperationException>(() => MasSockGlobals.GetSessionID());
        Assert.Contains("未接线", ex.Message);
    }

    [Fact]
    public void MasSockGlobals_UnwiredCloseUser_ThrowsExplicitly()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => MasSockGlobals.CloseUser("s", "a", 1));
        Assert.Contains("未接线", ex.Message);
    }

    // ========================================================================
    // MasSock.pas:217-266 单元级函数
    // ========================================================================

    [Theory]
    [InlineData("abc123", true)]
    [InlineData("ABCxyz09", true)]
    [InlineData("", true)]            // 空串 ⇒ 循环不执行 ⇒ True
    [InlineData("a_b", false)]
    [InlineData("a b", false)]
    [InlineData("a.b", false)]
    [InlineData("中文", false)]
    public void CheckAccountValid_Theory(string account, bool expected)
        => Assert.Equal(expected, MasSockFns.CheckAccountValid(account));

    [Theory]
    [InlineData("abc", true)]
    [InlineData("", true)]
    [InlineData("中文", true)]
    [InlineData("a/b", false)]
    [InlineData("a@b", false)]
    [InlineData("a$b", false)]
    [InlineData("a<b", false)]
    [InlineData("a>b", false)]
    public void CheckStringValid_ForbidsSlashAtDollarLtGt(string s, bool expected)
        => Assert.Equal(expected, MasSockFns.CheckStringValid(s));

    [Theory]
    [InlineData("abc", true)]
    [InlineData("a@b.com", true)]     // ★ CheckStringValid2 少了 '@'（邮箱字段用它）
    [InlineData("a/b", false)]
    [InlineData("a$b", false)]
    [InlineData("a<b", false)]
    [InlineData("a>b", false)]
    public void CheckStringValid2_ForbidsSlashDollarLtGt_AllowsAt(string s, bool expected)
        => Assert.Equal(expected, MasSockFns.CheckStringValid2(s));

    [Fact]
    public void CompareText_IsCaseInsensitive()
    {
        Assert.Equal(0, MasSockFns.CompareText("SRVA", "srva"));
        Assert.True(MasSockFns.SameText("AbC", "aBc"));
        Assert.NotEqual(0, MasSockFns.CompareText("a", "b"));
    }

    [Fact]
    public void AnsiStringCharAt_ReplicatesDelphiOutOfRangeAsNul()
    {
        Assert.Equal('a', MasSockFns.AnsiStringCharAt("abc", 1));   // 1-based 第 1 字符
        Assert.Equal('c', MasSockFns.AnsiStringCharAt("abc", 3));
        Assert.Equal('\0', MasSockFns.AnsiStringCharAt("abc", 0));  // 原文 S[0] = 长度字段高位（<256 时 #0）
        Assert.Equal('\0', MasSockFns.AnsiStringCharAt("abc", 4));  // 越界
        Assert.Equal('\0', MasSockFns.AnsiStringCharAt("", 1));
    }

    [Fact]
    public void TruncateToShortString15_TruncatesLikeDelphiString15()
    {
        Assert.Equal("123456789012345", MasSockFns.TruncateToShortString15("123456789012345"));
        Assert.Equal("123456789012345", MasSockFns.TruncateToShortString15("1234567890123456"));
    }

    // ---- ArrestStringEx_Ansi 逐字复刻 vs GXX.Core 版（跨区缺口 B-P10-17）----

    [Fact]
    public void ArrestStringExAnsi_FoundPath_MatchesDelphiAndGxxCore()
    {
        string dest1 = "", dest2 = "";
        Assert.Equal("(2/b)", MasSockFns.ArrestStringExAnsi("(1/a)(2/b)", '(', ')', ref dest1));
        Assert.Equal("1/a", dest1);

        Assert.Equal("(2/b)", HUtil32.ArrestStringEx("(1/a)(2/b)", '(', ')', ref dest2));
        Assert.Equal(dest1, dest2);
    }

    [Fact]
    public void ArrestStringExAnsi_NotFoundPath_KeepsSourceUnlikeGxxCore()
    {
        string dest1 = "", dest2 = "";

        // 原文 HUtil32.pas:1766 `Result := Source` —— 未命中 '(' ⇒ 返回**原串**
        Assert.Equal("garbage)", MasSockFns.ArrestStringExAnsi("garbage)", '(', ')', ref dest1));
        Assert.Equal("", dest1);

        // 原文同样：有 '(' 但后面没有 ')' ⇒ 返回原串
        string dest3 = "";
        Assert.Equal("(abc", MasSockFns.ArrestStringExAnsi("(abc", '(', ')', ref dest3));
        Assert.Equal("", dest3);

        // GXX.Core.Util.HUtil32.ArrestStringEx 的 Result 初值是 "" ⇒ 两条"未命中"路径都返回空串
        Assert.Equal("", HUtil32.ArrestStringEx("garbage)", '(', ')', ref dest2));
        Assert.Equal("", HUtil32.ArrestStringEx("(abc", '(', ')', ref dest2));

        // 空串两边一致
        string dest4 = "";
        Assert.Equal("", MasSockFns.ArrestStringExAnsi("", '(', ')', ref dest4));
    }

    // ---- TAccountInfo2 线格式尺寸（跨区缺口 B-P10-18）----

    [Fact]
    public unsafe void AccountInfo2_ManagedSizeEqualsDelphiPackedSize()
    {
        Assert.Equal(218, MasSockFns.TAccountInfo2PackedSize);   // Delphi packed record：8+8+202
        // 托管类型 GXX.Core.Protocol.TAccountInfo2 的字段顺序/容量与 packed 记录逐字段相同，
        // 实测 sizeof 也是 218（前 16 字节是两个 8 字节 Int64，其后全是字节缓冲 ⇒ 无中间填充，
        // 也无尾部填充）⇒ 按 218 字节解码后直接 blit 进结构体，字段偏移完全重合。
        Assert.Equal(218, sizeof(TAccountInfo2));
    }

    // ========================================================================
    // FormCreate / FormDestroy
    // ========================================================================

    [Fact]
    public void NewForm_WithoutFormCreate_ServerListIsNull()
    {
        using var f = new TFrmMasSoc();
        Assert.Null(f.m_ServerList);                              // 原文：TList 在 FormCreate 里才 Create
    }

    [Fact]
    public void FormCreate_CreatesEmptyServerList()
    {
        using var f = NewForm();
        Assert.NotNull(f.m_ServerList);
        Assert.Empty(f.m_ServerList);
    }

    [Fact]
    public void FormCreate_MissingUserLimitFile_ShowsCriticalMessageAndLeavesGlobalsUntouched_OriginalFlaw()
    {
        // ★ F7 原文如此：文件缺失只 ShowMessage，nUserLimit / UserLimit 都不动
        MasSockGlobals.nUserLimit = 42;

        using var f = NewForm();

        Assert.Equal(42, MasSockGlobals.nUserLimit);              // 未被重置
        Assert.Equal(@"[Critical Failure] file not found. .\!UserLimit.txt", LoginSrvForms.LastMessage);
        Assert.Equal("", LoginSrvForms.LastCaption);
    }

    [Fact]
    public void FormDestroy_MarksListFreedButKeepsDanglingField_OriginalFlaw()
    {
        using var f = NewForm();
        f.m_ServerList.Add(new TMsgServerInfo { sServerName = "x" });
        var list = f.m_ServerList;

        f.FormDestroy(f);

        // ★ F8：原文 m_ServerList.Free 后未置 nil（悬垂指针）⇒ 托管以 null 表达"已释放"
        Assert.Null(f.m_ServerList);
        // 原 Dispose 只释放条目，不清空 TList 本身（托管条目交 GC）⇒ 旧引用仍看得到 1 条
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void FormDestroy_WithoutFormCreate_Throws_DanglingAccess()
    {
        var f = new TFrmMasSoc();
        Assert.Throws<NullReferenceException>(() => f.FormDestroy(f));   // 原文：m_ServerList = nil ⇒ 访问违例
        f.Dispose();
    }

    [Fact]
    public void GetOnlineHumCount_AfterDestroy_SwallowsExceptionAndReturnsZero_OriginalFlaw()
    {
        using var f = NewForm();
        f.FormDestroy(f);

        Assert.Equal(0, f.GetOnlineHumCount());                   // ★ F9：空 except 吞掉 NRE
        Assert.Equal(1, LoginSrvShare.g_MainMsgList.Count);
        Assert.Contains("TFrmMasSoc.GetOnlineHumCount", LoginSrvShare.g_MainMsgList[0]);
    }

    [Fact]
    public void CheckReadyServers_AfterDestroy_Throws_NoExceptHere()
    {
        // 对照：CheckReadyServers 原文**没有** try/except ⇒ 悬垂访问直接炸
        using var f = NewForm();
        f.FormDestroy(f);
        Assert.Throws<NullReferenceException>(() => f.CheckReadyServers());
    }

    // ========================================================================
    // StartService
    // ========================================================================

    [Fact]
    public void StartService_UnwiredSocketSeam_LogsFailureAndSwallows_OriginalFlaw()
    {
        using var f = NewForm();
        LoginSrvShare.g_Config.sServerAddr = "0.0.0.0";
        LoginSrvShare.g_Config.nServerPort = 5600;

        f.StartService();

        Assert.Equal("0.0.0.0", f.MSocket.Address);                // 赋值在 try 之前，照样生效
        Assert.Equal(5600, f.MSocket.Port);
        Assert.False(f.MSocket.Active);
        // ★ F9：JSocket 未接线（Active := True 抛"未接线"）被空 except 吞掉，只留一行日志
        Assert.Equal(1, LoginSrvShare.g_MainMsgList.Count);
        Assert.Contains("TFrmMasSoc.StartService", LoginSrvShare.g_MainMsgList[0]);
    }

    [Fact]
    public void StartService_InjectedSocket_ActivatesAndLogsSuccess()
    {
        var fake = new TTestServerSocket();
        TFrmMasSoc.SocketFactory = () => fake;

        using var f = NewForm();
        LoginSrvShare.g_Config.sServerAddr = "10.0.0.1";
        LoginSrvShare.g_Config.nServerPort = 6000;

        f.StartService();

        Assert.Same(fake, f.MSocket);
        Assert.Equal("10.0.0.1", fake.Address);
        Assert.Equal(6000, fake.Port);
        Assert.True(fake.Active);
        Assert.Equal(1, LoginSrvShare.g_MainMsgList.Count);
        Assert.Contains("游戏中心服务启动成功(10.0.0.1:6000)...", LoginSrvShare.g_MainMsgList[0]);
    }

    [Fact]
    public void StartService_InjectedSocket_KeepsDfmEventBindings()
    {
        // 注入替身之后 DFM 的 4 条事件绑定必须仍然挂在**替身**上（InitializeComponent 里挂接）
        TFrmMasSoc.SocketFactory = () => new TTestServerSocket();

        using var f = NewForm();
        Assert.Equal(4, P10FormReconcile.CountEventBindingsOn(f.MSocket));

        MasSockGlobals.g_ServerAddr[0] = "7.7.7.7";              // 地址表须在 FormCreate 之后设置
        MasSockGlobals.g_ServerAddrCount = 1;

        var sock = new TCustomWinSocket { RemoteAddress = "7.7.7.7", Connected = true };
        f.MSocket.RaiseClientConnect(sock);
        Assert.Single(f.m_ServerList);
    }

    // ========================================================================
    // MSocketClientConnect
    // ========================================================================

    [Fact]
    public void Connect_AllowedAddress_AddsZeroedEntry()
    {
        using var f = NewForm();
        MasSockGlobals.g_ServerAddr[0] = "1.2.3.4";               // 须在 FormCreate 之后设置
        MasSockGlobals.g_ServerAddrCount = 1;

        var sock = new TCustomWinSocket { RemoteAddress = "1.2.3.4" };

        f.MSocketClientConnect(f, sock);

        Assert.Single(f.m_ServerList);
        var info = f.m_ServerList[0];
        Assert.Same(sock, info.Socket);
        Assert.Equal("", info.sReceiveMsg);      // 原文 FillChar 之后再冗余地赋一次空串
        Assert.Equal("", info.sServerName);
        Assert.Equal(0, info.nServerIndex);
        Assert.Equal(0, info.nOnlineCount);
        Assert.Equal(0u, info.dwKeepAliveTick);
        Assert.Equal("", info.sIPaddr);
        Assert.False(sock.CloseCalled);
    }

    [Fact]
    public void Connect_DeniedAddress_LogsAndCloses()
    {
        using var f = NewForm();
        MasSockGlobals.g_ServerAddr[0] = "1.2.3.4";               // 须在 FormCreate 之后设置
        MasSockGlobals.g_ServerAddrCount = 1;

        var sock = new TCustomWinSocket { RemoteAddress = "9.9.9.9" };

        f.MSocketClientConnect(f, sock);

        Assert.Empty(f.m_ServerList);
        Assert.True(sock.CloseCalled);
        Assert.Equal(1, LoginSrvShare.g_MainMsgList.Count);
        Assert.Contains("非法地址连接:9.9.9.9", LoginSrvShare.g_MainMsgList[0]);
    }

    [Fact]
    public void Connect_EmptyAddressTable_RejectsEveryone_Boundary()
    {
        using var f = NewForm();
        var sock = new TCustomWinSocket { RemoteAddress = "1.2.3.4" };

        f.MSocketClientConnect(f, sock);

        Assert.Empty(f.m_ServerList);
        Assert.True(sock.CloseCalled);
    }

    [Fact]
    public void Connect_AddressCompareIsCaseSensitive_UnlikeLMain()
    {
        // 原文 :167 用 `=`（区分大小写）；LMain.pas:281 的同类白名单用的是 SameText ⇒ 两处语义不同，原文如此
        using var f = NewForm();
        MasSockGlobals.g_ServerAddr[0] = "Server1";               // 须在 FormCreate 之后设置
        MasSockGlobals.g_ServerAddrCount = 1;

        var sock = new TCustomWinSocket { RemoteAddress = "server1" };
        f.MSocketClientConnect(f, sock);

        Assert.Empty(f.m_ServerList);
        Assert.True(sock.CloseCalled);
    }

    [Fact]
    public void Connect_CountBeyondArray_Throws_OriginalFlaw()
    {
        // ★ 原文如此：循环上界是 g_ServerAddrCount，不夹紧到 100 ⇒ 原文读数组外内存
        MasSockGlobals.g_ServerAddrCount = 101;

        using var f = NewForm();
        var sock = new TCustomWinSocket { RemoteAddress = "1.2.3.4" };
        Assert.Throws<IndexOutOfRangeException>(() => f.MSocketClientConnect(f, sock));
    }

    // ========================================================================
    // MSocketClientDisconnect / MSocketClientError
    // ========================================================================

    [Fact]
    public void Disconnect_RemovesMatchingEntryAndKeepsOthers()
    {
        using var f = NewForm();
        var a = AddServer(f, "a", 0, 0, "1.1.1.1");
        var b = AddServer(f, "b", 1, 0, "2.2.2.2");

        f.MSocketClientDisconnect(f, a.Socket);

        Assert.Single(f.m_ServerList);
        Assert.Same(b, f.m_ServerList[0]);
    }

    [Fact]
    public void Disconnect_NoMatch_LeavesListUntouched_Boundary()
    {
        using var f = NewForm();
        AddServer(f, "a", 0, 0, "1.1.1.1");
        var stranger = new TCustomWinSocket { RemoteAddress = "9.9.9.9" };

        f.MSocketClientDisconnect(f, stranger);

        Assert.Single(f.m_ServerList);
    }

    [Fact]
    public void Disconnect_NullSocketParameter_RemovesFirstNullSocketEntry_OriginalFlaw()
    {
        // ★ 原文如此：`MsgServer.Socket = Socket` 是指针相等 ⇒ 两个 nil 也算相等
        using var f = NewForm();
        f.m_ServerList.Add(new TMsgServerInfo { Socket = null, sServerName = "nilEntry" });
        var real = AddServer(f, "a", 0, 0, "1.1.1.1");

        f.MSocketClientDisconnect(f, null);

        Assert.Single(f.m_ServerList);
        Assert.Same(real, f.m_ServerList[0]);
        Assert.NotNull(real.Socket);          // 真正在线的条目没被误删
    }

    [Fact]
    public void ClientError_ZeroesErrorCodeAndCloses()
    {
        using var f = NewForm();
        var sock = new TCustomWinSocket { RemoteAddress = "1.1.1.1", Connected = true };
        int errorCode = 12345;

        f.MSocketClientError(f, sock, TErrorEvent.eeSend, ref errorCode);

        Assert.Equal(0, errorCode);
        Assert.True(sock.CloseCalled);
    }

    // ========================================================================
    // MSocketClientRead —— 帧解析
    // ========================================================================

    [Fact]
    public void Read_EmptyReceiveText_NoSideEffects()
    {
        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");

        f.MSocketClientRead(f, t.Socket);

        Assert.Empty(t.Socket.SentTexts);
        Assert.Equal("", t.sReceiveMsg);
    }

    [Fact]
    public void Read_PartialFrame_KeepsBuffer()
    {
        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        t.Socket.ReceiveText = "(1007/abc";

        f.MSocketClientRead(f, t.Socket);

        Assert.Empty(t.Socket.SentTexts);
        Assert.Equal("(1007/abc", t.sReceiveMsg);                 // 无 ')' ⇒ while 不进 ⇒ 整串保留
    }

    [Fact]
    public void Read_PartialThenComplete_Concatenates()
    {
        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");

        t.Socket.ReceiveText = "(1007/abc";
        f.MSocketClientRead(f, t.Socket);
        t.Socket.ReceiveText = "def)";
        f.MSocketClientRead(f, t.Socket);

        Assert.Equal(new[] { "(1007/abcdef)" }, t.Socket.SentTexts.ToArray());
        Assert.Equal("", t.sReceiveMsg);
    }

    [Fact]
    public void Read_MultipleFrames_ProcessedSequentially()
    {
        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        t.Socket.ReceiveText = "(1007/a)(1007/b)";

        f.MSocketClientRead(f, t.Socket);

        Assert.Equal(new[] { "(1007/a)", "(1007/b)" }, t.Socket.SentTexts.ToArray());
        Assert.Equal("", t.sReceiveMsg);
    }

    [Fact]
    public void Read_UnhandledCode_SendsNothing()
    {
        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        t.Socket.ReceiveText = "(9999/body)";

        f.MSocketClientRead(f, t.Socket);

        Assert.Empty(t.Socket.SentTexts);
        Assert.Equal("", t.sReceiveMsg);
    }

    [Fact]
    public void Read_UnknownMsgCode_EchoesBodyToAllConnected()
    {
        using var f = NewForm();
        var a = AddServer(f, "a", 0, 0, "1.1.1.1");
        var t = AddServer(f, "srvA", 1, 0, "2.2.2.2");
        t.Socket.ReceiveText = "(1007/ping)";

        f.MSocketClientRead(f, t.Socket);

        Assert.Equal(new[] { "(1007/ping)" }, t.Socket.SentTexts.ToArray());
        Assert.Equal(new[] { "(1007/ping)" }, a.Socket.SentTexts.ToArray());   // SendServerMsgA 不过滤
    }

    [Fact]
    public void Read_CloseParenWithoutOpenParen_KeepsBuffer_DelphiArrestStringExSemantics()
    {
        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        t.Socket.ReceiveText = "garbage)";

        f.MSocketClientRead(f, t.Socket);

        // ★ 原文 HUtil32.pas:1766 `Result := Source` ⇒ 未命中 '(' 时返回原串 ⇒ 缓冲**保留**（永不丢弃）
        Assert.Equal("garbage)", t.sReceiveMsg);
        Assert.Empty(t.Socket.SentTexts);
    }

    // ---- ★ F1：:628 的回写在 if 之外 ----

    [Fact]
    public void Read_PendingFrameClonedIntoForeignEntries_OriginalFlaw()
    {
        // ★ F1 原文如此：`MsgServer.sReceiveMsg := sReviceMsg` 在 `if Socket = Socket` 之外
        using var f = NewForm();
        var t = AddServer(f, "t", 0, 0, "1.1.1.1");
        var other1 = AddServer(f, "o1", 1, 0, "2.2.2.2");
        var other2 = AddServer(f, "o2", 2, 0, "3.3.3.3");
        t.sReceiveMsg = "(1007/x)(1000";

        f.MSocketClientRead(f, t.Socket);

        Assert.Equal("(1000", t.sReceiveMsg);        // 本条目：帧被吃掉，剩半包
        Assert.Equal("(1000", other1.sReceiveMsg);   // ★ 无关条目被写入同一条半包（串包）
        Assert.Equal("(1000", other2.sReceiveMsg);
    }

    [Fact]
    public void Read_NonMatchingEntriesBufferWipedWithEmptyString_OriginalFlaw()
    {
        // ★ F1 的另一面：首次未命中时 sReviceMsg 是编译器零初始化的 "" ⇒ 无关条目的缓冲被清空
        using var f = NewForm();
        var a = AddServer(f, "a", 0, 0, "1.1.1.1");
        a.sReceiveMsg = "(partialA";
        var t = AddServer(f, "t", 1, 0, "2.2.2.2");
        t.Socket.ReceiveText = "(1007/x)";

        f.MSocketClientRead(f, t.Socket);

        Assert.Equal("", a.sReceiveMsg);             // ★ 半包丢失
        Assert.Equal("", t.sReceiveMsg);
    }

    [Fact]
    public void Read_StrangerSocket_StillWipesEveryEntryBuffer_OriginalFlaw()
    {
        // ★ F1 极端面：Socket 不在列表里时，循环一次都不进 if，但仍对每个条目写 sReviceMsg（= ""）
        using var f = NewForm();
        var a = AddServer(f, "a", 0, 0, "1.1.1.1");
        a.sReceiveMsg = "(pending";
        var stranger = new TCustomWinSocket { RemoteAddress = "9.9.9.9", Connected = true, ReceiveText = "(1007/x)" };

        f.MSocketClientRead(f, stranger);

        Assert.Equal("", a.sReceiveMsg);
        Assert.Empty(a.Socket.SentTexts);            // 陌生 socket 的帧根本没被解析
    }

    // ---- SS_SOFTOUTSESSION（1020）----

    [Fact]
    public void Read_SsSoftOutSession_InvokesCloseUserSeam()
    {
        var calls = new List<(string, string, int)>();
        MasSockGlobals.CloseUserHandler = (srv, acc, sid) => calls.Add((srv, acc, sid));

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        t.Socket.ReceiveText = "(1020/acc1/33)";

        f.MSocketClientRead(f, t.Socket);

        Assert.Single(calls);
        Assert.Equal("srvA", calls[0].Item1);
        Assert.Equal("acc1", calls[0].Item2);
        Assert.Equal(33, calls[0].Item3);
    }

    [Fact]
    public void Read_SsSoftOutSession_UnwiredCloseUser_ThrowsExplicitly()
    {
        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        t.Socket.ReceiveText = "(1020/acc1/33)";

        var ex = Assert.Throws<InvalidOperationException>(() => f.MSocketClientRead(f, t.Socket));
        Assert.Contains("未接线", ex.Message);
    }

    // ---- SS_SERVERINFO（1030）----

    [Fact]
    public void Read_SsServerInfo_SetsFieldsSortsAndBroadcastsKeepAlive()
    {
        MasSockGlobals.UserLimit[0].sServerName = "srvA";
        MasSockGlobals.UserLimit[0].sName = "授权A";
        MasSockGlobals.UserLimit[0].nLimitCountMax = 100;

        using var f = NewForm();
        var mirror = AddServer(f, "mirror", 99, 5, "2.2.2.2");     // 99 号镜像：不计入在线数
        var t = AddServer(f, "srvA", 0, 10, "1.1.1.1");
        t.Socket.ReceiveText = "(1030/srvA/0/7/x)";

        f.MSocketClientRead(f, t.Socket);

        Assert.Equal("srvA", t.sServerName);
        Assert.Equal(0, t.nServerIndex);
        Assert.Equal(7, t.nOnlineCount);
        Assert.NotEqual(0u, t.dwKeepAliveTick);

        // nOnlineCountMin := GetOnlineHumCount()（只累加 index <> 99）
        Assert.Equal(7, MasSockGlobals.nOnlineCountMin);
        Assert.Equal(7, MasSockGlobals.nOnlineCountMax);
        // RefServerLimit("srvA")：同名且 index<>99 的在线数合计 → nLimitCountMin
        Assert.Equal(7, MasSockGlobals.UserLimit[0].nLimitCountMin);

        Assert.Contains("(1040/7)", t.Socket.SentTexts);
        Assert.Contains("(1040/7)", mirror.Socket.SentTexts);
    }

    [Fact]
    public void Read_SsServerInfo_SortsSameNameByIndexDescending_OriginalSortServerList()
    {
        // 原文 SortServerList 的判定是 `Items[nC].nServerIndex < MsgServerSort.nServerIndex`
        // ⇒ 同名服务器按 nServerIndex **降序**排列（原文如此，不是直观的升序）
        using var f = NewForm();
        var a = AddServer(f, "srvs", 1, 0, "1.1.1.1");
        a.Socket.ReceiveText = "(1030/srvs/1/1/x)";
        f.MSocketClientRead(f, a.Socket);                     // 只有 1 条 ⇒ 取出再插回，仍是它自己

        var b = AddServer(f, "srvs", 9, 0, "2.2.2.2");
        b.Socket.ReceiveText = "(1030/srvs/9/1/x)";
        f.MSocketClientRead(f, b.Socket);

        Assert.Equal(2, f.m_ServerList.Count);
        Assert.Same(b, f.m_ServerList[0]);
        Assert.Same(a, f.m_ServerList[1]);
    }

    [Fact]
    public void Read_SsServerInfo_RemovesDuplicateNameAndIndex_OriginalSortServerList()
    {
        // 覆盖 SortServerList 最深的一支：n10 分支插入后，再扫 n14 找到同名同索引的重复项并删除
        using var f = NewForm();
        var a = AddServer(f, "srvs", 5, 0, "1.1.1.1");          // 0
        var s = AddServer(f, "srvs", 3, 0, "2.2.2.2");          // 1 ← 由它的读取触发排序
        var x = AddServer(f, "srvs", 1, 0, "3.3.3.3");          // 2
        var y = AddServer(f, "srvs", 3, 0, "4.4.4.4");          // 3 ← s 的重复项（同名同索引）

        s.Socket.ReceiveText = "(1030/srvs/3/1/x)";
        f.MSocketClientRead(f, s.Socket);

        Assert.Equal(3, f.m_ServerList.Count);                 // y 被删除
        Assert.DoesNotContain(y, f.m_ServerList);
        Assert.Same(a, f.m_ServerList[0]);
        Assert.Same(s, f.m_ServerList[1]);
        Assert.Same(x, f.m_ServerList[2]);
    }

    [Fact]
    public void Read_SsServerInfo_MaxOnlineCountOnlyGrows()
    {
        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");

        t.Socket.ReceiveText = "(1030/srvA/0/50/x)";
        f.MSocketClientRead(f, t.Socket);
        Assert.Equal(50, MasSockGlobals.nOnlineCountMax);

        t.Socket.ReceiveText = "(1030/srvA/0/3/x)";     // 在线数下降
        f.MSocketClientRead(f, t.Socket);
        Assert.Equal(3, MasSockGlobals.nOnlineCountMin);   // min 跟着当前值走
        Assert.Equal(50, MasSockGlobals.nOnlineCountMax);  // ★ max 只增不减（原文如此）
    }

    // ---- SS_PASSWORDSUCCESS（1132）----

    [Fact]
    public void Read_SsPasswordSuccess_BuildsOpenSessionPayload()
    {
        MasSockGlobals.GetSessionIDHandler = () => 7;

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        t.Socket.ReceiveText = "(1132/acc1)";

        f.MSocketClientRead(f, t.Socket);

        // 原文：sAccount + '/' + IntToStr(nSessionID) + '/' + IntToStr(Integer(True)) + '/' + IntToStr(5) + '/' + RemoteAddress
        // Integer(True) = 1（GXX.Core/Util/HUtil32.cs:672 取证）⇒ 不是 -1
        Assert.Equal(new[] { "(1000/acc1/7/1/5/1.1.1.1)" }, t.Socket.SentTexts.ToArray());
    }

    [Fact]
    public void Read_SsPasswordSuccess_UnwiredGetSessionID_ThrowsExplicitly()
    {
        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        t.Socket.ReceiveText = "(1132/acc1)";

        var ex = Assert.Throws<InvalidOperationException>(() => f.MSocketClientRead(f, t.Socket));
        Assert.Contains("未接线", ex.Message);
    }

    // ---- SS_GetAccountInfo（1133）----

    [Fact]
    public void Read_SsGetAccountInfo_SendsTabSeparatedFields()
    {
        var db = DbWithAccount("acc1", "pwd");
        db.Store["acc1"] = BuildAccount("acc1", "pwd", "uname", "Q1", "A1", "Q2", "A2", "19800101", "13800000000", "010-1234", "m@x.com");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        t.Socket.ReceiveText = "(1133/acc1/nick)";

        f.MSocketClientRead(f, t.Socket);

        Assert.Equal(
            new[] { "(1134/nick /uname \tpwd \tQ1 \tA1 \tQ2 \tA2 \t19800101 \t13800000000 \t010-1234 \tm@x.com \t)" },
            t.Socket.SentTexts.ToArray());
    }

    [Fact]
    public void Read_SsGetAccountInfo_AccountNotFound_SendsNothing()
    {
        DbWithAccount("acc1");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        t.Socket.ReceiveText = "(1133/nosuch/nick)";

        f.MSocketClientRead(f, t.Socket);

        Assert.Empty(t.Socket.SentTexts);
    }

    [Fact]
    public void Read_SsGetAccountInfo_EmptyAccount_SkipsLookup_Boundary()
    {
        DbWithAccount("acc1");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        t.Socket.ReceiveText = "(1133)";                 // 去掉 code 后 body 为空 ⇒ sAccount = ""

        f.MSocketClientRead(f, t.Socket);

        Assert.Empty(t.Socket.SentTexts);
    }

    [Fact]
    public void Read_SsGetAccountInfo_UnwiredAccountDb_Throws_OriginalAccessViolation()
    {
        // 原文直接解引用 g_AccountDB（nil ⇒ 访问违例，且本过程无 try/except）
        LoginSrvShare.g_AccountDB = null;

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        t.Socket.ReceiveText = "(1133/acc1/nick)";

        Assert.Throws<NullReferenceException>(() => f.MSocketClientRead(f, t.Socket));
    }

    // ---- SS_ChangeAccountInfo（1135）----

    [Fact]
    public void Read_ChangeAccountInfo_WrongPayloadSize_Ignored()
    {
        DbWithAccount("acc1");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");

        string reply = SendChangeAccountInfo(f, t, "AAAA");      // GetDecodeSize(4)=3 ≠ 218

        Assert.Equal("", reply);
        Assert.Equal("", t.sReceiveMsg);
    }

    [Fact]
    public void Read_ChangeAccountInfo_AccountNotFound_SendsMinusOneAndExitsWholeProcedure_OriginalFlaw()
    {
        LoginSrvShare.g_AccountDB = new InMemoryAccountDb();      // 空库

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "newpw1");
        t.Socket.ReceiveText = "(1135/" + enc + ")(1007/ping)";

        f.MSocketClientRead(f, t.Socket);

        Assert.Single(t.Socket.SentTexts);
        Assert.Equal("(1136/-1/" + enc + ")", t.Socket.SentTexts[0]);   // 回包里的 sMsg 是**未解码**的原编码串
        // ★ F2：:383 的 Exit 退出整个过程 ⇒ 紧跟其后的第二帧从未被解析
        Assert.DoesNotContain("(1007/ping)", t.Socket.SentTexts);
    }

    [Theory]
    [InlineData("ab", "u1", "m@x.com", -2)]                  // 密码 < 3 位
    [InlineData("a/b", "u1", "m@x.com", -2)]                 // 密码含 '/'
    [InlineData("pw$1", "u1", "m@x.com", -2)]                // 密码含 '$'
    [InlineData("newpw1", "u$1", "m@x.com", -8)]             // 用户名含 '$'
    [InlineData("newpw1", "u1", "m<x.com", -14)]             // 邮箱含 '<'（CheckStringValid2）
    [InlineData("newpw1", "u1", "m@x.com", 0)]               // 全合法 → 0
    public void Read_ChangeAccountInfo_ValidationChain_Codes(
        string password, string userName, string mail, int expectedErrCode)
    {
        DbWithAccount("acc1");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", password, userName: userName, mail: mail);

        string reply = SendChangeAccountInfo(f, t, enc);

        Assert.Equal("(1136/" + expectedErrCode + "/" + enc + ")", reply);
    }

    [Fact]
    public void Read_ChangeAccountInfo_QuestionAndAnswerFields_MapToMinus9ToMinus12()
    {
        DbWithAccount("acc1");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");

        string q1 = EncodeAccountInfo2Record("acc1", "newpw1", q1: "q$1");
        Assert.Equal("(1136/-9/" + q1 + ")", SendChangeAccountInfo(f, t, q1));

        string a1 = EncodeAccountInfo2Record("acc1", "newpw1", a1: "a$1");
        Assert.Equal("(1136/-10/" + a1 + ")", SendChangeAccountInfo(f, t, a1));

        string q2 = EncodeAccountInfo2Record("acc1", "newpw1", q2: "q$2");
        Assert.Equal("(1136/-11/" + q2 + ")", SendChangeAccountInfo(f, t, q2));

        string a2 = EncodeAccountInfo2Record("acc1", "newpw1", a2: "a$2");
        Assert.Equal("(1136/-12/" + a2 + ")", SendChangeAccountInfo(f, t, a2));
    }

    [Fact]
    public void Read_ChangeAccountInfo_DisableIdSamePassword_ErrMinus3()
    {
        DbWithAccount("acc1");
        LoginSrvShare.g_Config.boDisableIDSamePassword = true;

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "acc1");     // 帐号 = 密码

        Assert.Equal("(1136/-3/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_DisablePwdSameChr_ErrMinus4()
    {
        DbWithAccount("acc1");
        LoginSrvShare.g_Config.boDisablePwdSameChr = true;

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "aaaa");

        Assert.Equal("(1136/-4/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_PwdSameChr_ThreeChars_ErrMinus4()
    {
        // 3 位是长度下限（合法）⇒ 走"同字符"分支
        DbWithAccount("acc1");
        LoginSrvShare.g_Config.boDisablePwdSameChr = true;

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "aaa");

        Assert.Equal("(1136/-4/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_DisablePwdAllNum_ErrMinus5()
    {
        DbWithAccount("acc1");
        LoginSrvShare.g_Config.boDisablePwdAllNum = true;

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "12345");

        Assert.Equal("(1136/-5/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_DisablePwdAllLetter_ErrMinus6()
    {
        DbWithAccount("acc1");
        LoginSrvShare.g_Config.boDisablePwdAllLetter = true;

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "abcde");

        Assert.Equal("(1136/-6/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_ForbiddenPasswordList_ErrMinus7()
    {
        DbWithAccount("acc1");
        LoginSrvShare.g_DisablePasswordList.Add("123");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "abc123xyz");

        Assert.Equal("(1136/-7/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_MobilePhoneNonDigit_ErrMinus13_InvertedFlag_OriginalFlaw()
    {
        // ★ F11 原文如此：这一段的标志是反的（boTemp=False，遇"非数字"才置 True），
        //   且错误码与上面 CheckStringValid(MobilePhone) 失败**共用 -13**
        DbWithAccount("acc1");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "newpw1", mobile: "abc");

        Assert.Equal("(1136/-13/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_MobilePhoneWithForbiddenChar_AlsoMinus13()
    {
        // 同一个 -13：先撞 CheckStringValid(MobilePhone) 的 '$'
        DbWithAccount("acc1");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "newpw1", mobile: "$38");

        Assert.Equal("(1136/-13/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_EmptyMobilePhone_PassesDigitScan()
    {
        // 边界：空串 ⇒ 数字扫描循环不执行、boTemp 保持 False ⇒ 不报 -13
        DbWithAccount("acc1");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "newpw1", mobile: "");

        Assert.Equal("(1136/0/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_DisableQuizSameAnswer_ErrMinus15And16()
    {
        DbWithAccount("acc1");
        LoginSrvShare.g_Config.boDisableQuizSameAnswer = true;

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");

        string enc15 = EncodeAccountInfo2Record("acc1", "newpw1", q1: "same", a1: "same");
        Assert.Equal("(1136/-15/" + enc15 + ")", SendChangeAccountInfo(f, t, enc15));

        string enc16 = EncodeAccountInfo2Record("acc1", "newpw1", q2: "same", a2: "same");
        Assert.Equal("(1136/-16/" + enc16 + ")", SendChangeAccountInfo(f, t, enc16));
    }

    [Fact]
    public void Read_ChangeAccountInfo_L2PasswordForbiddenChar_ErrMinus17()
    {
        DbWithAccount("acc1");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "newpw1", l2: "l2$1");

        Assert.Equal("(1136/-17/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_L2SameAsAccount_ErrMinus18()
    {
        DbWithAccount("acc1");
        LoginSrvShare.g_Config.boDisableIDSameL2Password = true;

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "newpw1", l2: "acc1");

        Assert.Equal("(1136/-18/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_L2SameAsPassword_ErrMinus19()
    {
        DbWithAccount("acc1");
        LoginSrvShare.g_Config.boDisableL2SamePassword = true;

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "same12", l2: "same12");

        Assert.Equal("(1136/-19/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_EmptyL2Password_SkipsLengthGuardedChecks_Boundary()
    {
        DbWithAccount("acc1");
        LoginSrvShare.g_Config.boDisableIDSameL2Password = true;
        LoginSrvShare.g_Config.boDisableL2SamePassword = true;

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "newpw1", l2: "");

        Assert.Equal("(1136/0/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_UpdateFail_ErrMinus20()
    {
        LoginSrvShare.g_AccountDB = new TUpdateFailDb("acc1");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("acc1", "newpw1");

        Assert.Equal("(1136/-20/" + enc + ")", SendChangeAccountInfo(f, t, enc));
    }

    [Fact]
    public void Read_ChangeAccountInfo_Success_WritesEveryFieldThroughUfAllField()
    {
        var db = DbWithAccount("acc1");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record(
            "acc1", "newpw1", userName: "unew", birthDay: "19990102",
            q1: "q1n", a1: "a1n", q2: "q2n", a2: "a2n",
            mobile: "13900000000", mail: "n@y.com", l2: "l2new");

        Assert.Equal("(1136/0/" + enc + ")", SendChangeAccountInfo(f, t, enc));

        var saved = db.Store["acc1"];
        Assert.Equal("newpw1", saved.PasswordStr);
        Assert.Equal("unew", saved.UserNameStr);
        Assert.Equal("19990102", saved.BirthDayStr);
        Assert.Equal("q1n", saved.Questions1Str);
        Assert.Equal("a1n", saved.Answers1Str);
        Assert.Equal("q2n", saved.Questions2Str);
        Assert.Equal("a2n", saved.Answers2Str);
        Assert.Equal("13900000000", saved.MobilePhoneStr);
        Assert.Equal("n@y.com", saved.MailStr);
        Assert.Equal("l2new", saved.L2PasswordStr);
    }

    [Fact]
    public void Read_ChangeAccountInfo_AccountNameNeverValidated_CommentedBlock_OriginalFlaw()
    {
        // ★ F12 原文如此：:389-398 的帐号名校验整块被 {...} 注释掉 ⇒ 非法帐号名照样通过
        var db = DbWithAccount("a$b");

        using var f = NewForm();
        var t = AddServer(f, "srvA", 0, 0, "1.1.1.1");
        string enc = EncodeAccountInfo2Record("a$b", "newpw1");

        Assert.Equal("(1136/0/" + enc + ")", SendChangeAccountInfo(f, t, enc));
        Assert.Equal("newpw1", db.Store["a$b"].PasswordStr);
    }

    // ========================================================================
    // LimitName / SendServerMsg / SendServerMsgA
    // ========================================================================

    [Fact]
    public void SendServerMsg_NoLimitEntry_SendsToAllConnected()
    {
        using var f = NewForm();
        var a = AddServer(f, "a", 0, 0, "1.1.1.1");
        var b = AddServer(f, "b", 1, 0, "2.2.2.2");

        f.SendServerMsg((ushort)CommonConst.SS_CLOSESESSION, "whatever", "acc/1");

        Assert.Equal(new[] { "(1010/acc/1)" }, a.Socket.SentTexts.ToArray());
        Assert.Equal(new[] { "(1010/acc/1)" }, b.Socket.SentTexts.ToArray());
    }

    [Fact]
    public void SendServerMsg_LimitNameMatch_IsCaseInsensitive()
    {
        // UserLimit[0]: sServerName = 'srva' → sName = 'SRVA'；目标条目名 'srva'
        // LimitName('srva') = 'SRVA'，过滤用 CompareText（忽略大小写）⇒ 命中
        MasSockGlobals.UserLimit[0].sServerName = "srva";
        MasSockGlobals.UserLimit[0].sName = "SRVA";

        using var f = NewForm();
        var a = AddServer(f, "srva", 0, 0, "1.1.1.1");

        f.SendServerMsg((ushort)CommonConst.SS_CLOSESESSION, "srva", "acc/1");

        Assert.Equal(new[] { "(1010/acc/1)" }, a.Socket.SentTexts.ToArray());
    }

    [Fact]
    public void SendServerMsg_LimitNameMismatch_Skips()
    {
        MasSockGlobals.UserLimit[0].sServerName = "srva";
        MasSockGlobals.UserLimit[0].sName = "OTHER";

        using var f = NewForm();
        var a = AddServer(f, "srva", 0, 0, "1.1.1.1");

        f.SendServerMsg((ushort)CommonConst.SS_CLOSESESSION, "srva", "acc/1");

        Assert.Empty(a.Socket.SentTexts);
    }

    [Fact]
    public void SendServerMsg_EmptyServerNameEntry_AlwaysReceives()
    {
        MasSockGlobals.UserLimit[0].sServerName = "srva";
        MasSockGlobals.UserLimit[0].sName = "OTHER";

        using var f = NewForm();
        var blank = AddServer(f, "", 0, 0, "1.1.1.1");      // 名字为空 ⇒ 无条件收
        var named = AddServer(f, "srva", 1, 0, "2.2.2.2");

        f.SendServerMsg((ushort)CommonConst.SS_CLOSESESSION, "srva", "acc/1");

        Assert.Single(blank.Socket.SentTexts);
        Assert.Empty(named.Socket.SentTexts);
    }

    [Fact]
    public void SendServerMsg_Index99Entry_AlwaysReceives()
    {
        MasSockGlobals.UserLimit[0].sServerName = "srva";
        MasSockGlobals.UserLimit[0].sName = "OTHER";

        using var f = NewForm();
        var mirror = AddServer(f, "srva", 99, 0, "1.1.1.1");

        f.SendServerMsg((ushort)CommonConst.SS_CLOSESESSION, "srva", "acc/1");

        Assert.Single(mirror.Socket.SentTexts);
    }

    [Fact]
    public void SendServerMsg_DisconnectedEntry_Skipped()
    {
        using var f = NewForm();
        var off = AddServer(f, "a", 0, 0, "1.1.1.1");
        off.Socket.Connected = false;

        f.SendServerMsg((ushort)CommonConst.SS_CLOSESESSION, "x", "acc/1");

        Assert.Empty(off.Socket.SentTexts);
    }

    [Fact]
    public void SendServerMsg_UsesSecureSendTextOverload()
    {
        using var f = NewForm();
        var a = AddServer(f, "a", 0, 0, "1.1.1.1");

        f.SendServerMsg((ushort)CommonConst.SS_CLOSESESSION, "x", "acc/1");

        Assert.Equal(new[] { "(1010/acc/1)" }, a.Socket.SentTextsSecure.ToArray());   // 原文 :786 传 True
    }

    [Fact]
    public void SendServerMsg_NullSocket_AbortsWholeLoopAndLogs_OriginalFlaw()
    {
        // ★ F13 原文如此：坏条目（Socket = nil）让整轮循环中断，后面的条目一条都收不到
        using var f = NewForm();
        f.m_ServerList.Add(new TMsgServerInfo { Socket = null });
        var ok = AddServer(f, "ok", 0, 0, "1.1.1.1");

        f.SendServerMsg((ushort)CommonConst.SS_CLOSESESSION, "", "acc/1");

        Assert.Empty(ok.Socket.SentTexts);
        Assert.Equal(1, LoginSrvShare.g_MainMsgList.Count);
        Assert.Contains("TFrmMasSoc.SendServerMsg", LoginSrvShare.g_MainMsgList[0]);
    }

    [Fact]
    public void SendServerMsg_FormatIsParenIdentSlashBodyParen()
    {
        using var f = NewForm();
        var a = AddServer(f, "a", 0, 0, "1.1.1.1");

        f.SendServerMsg(999, "x", "body");

        Assert.Equal(new[] { "(999/body)" }, a.Socket.SentTexts.ToArray());
    }

    [Fact]
    public void SendServerMsgA_NoFilterAndPlainSendText()
    {
        MasSockGlobals.UserLimit[0].sServerName = "srva";
        MasSockGlobals.UserLimit[0].sName = "OTHER";

        using var f = NewForm();
        var a = AddServer(f, "srva", 0, 0, "1.1.1.1");
        var off = AddServer(f, "off", 1, 0, "2.2.2.2");
        off.Socket.Connected = false;

        f.SendServerMsgA((ushort)CommonConst.SS_KEEPALIVE, "5");

        Assert.Equal(new[] { "(1040/5)" }, a.Socket.SentTexts.ToArray());
        Assert.Empty(off.Socket.SentTexts);
        Assert.Empty(a.Socket.SentTextsSecure);                 // :885 用的是不带 bSecure 的重载
    }

    [Fact]
    public void SendServerMsgA_NullSocket_LogsMethodNameAndExceptionMessage_OriginalFlaw()
    {
        // 对照 SendServerMsg：本方法用 `on e: Exception` ⇒ 多记一行 E.Message
        using var f = NewForm();
        f.m_ServerList.Add(new TMsgServerInfo { Socket = null });

        f.SendServerMsgA((ushort)CommonConst.SS_KEEPALIVE, "5");

        Assert.Equal(2, LoginSrvShare.g_MainMsgList.Count);
        Assert.Contains("TFrmMasSoc.SendServerMsgA", LoginSrvShare.g_MainMsgList[0]);
        Assert.DoesNotContain("TFrmMasSoc.SendServerMsgA", LoginSrvShare.g_MainMsgList[1]);
    }

    // ========================================================================
    // GetOnlineHumCount / IsNotUserFull / ServerStatus / CheckReadyServers
    // ========================================================================

    [Fact]
    public void GetOnlineHumCount_SumsEveryoneExceptIndex99()
    {
        using var f = NewForm();
        AddServer(f, "a", 0, 3, "1.1.1.1");
        AddServer(f, "b", 1, 4, "2.2.2.2");
        AddServer(f, "mirror", 99, 100, "3.3.3.3");

        Assert.Equal(7, f.GetOnlineHumCount());
    }

    [Fact]
    public void GetOnlineHumCount_EmptyList_Zero_Boundary()
    {
        using var f = NewForm();
        Assert.Equal(0, f.GetOnlineHumCount());
    }

    [Fact]
    public void IsNotUserFull_UnknownServer_True_Boundary()
    {
        using var f = NewForm();
        Assert.True(f.IsNotUserFull("nosuch"));
    }

    [Fact]
    public void IsNotUserFull_OverLimit_False()
    {
        MasSockGlobals.UserLimit[0].sServerName = "srva";
        MasSockGlobals.UserLimit[0].nLimitCountMin = 11;
        MasSockGlobals.UserLimit[0].nLimitCountMax = 10;

        using var f = NewForm();
        Assert.False(f.IsNotUserFull("srva"));
    }

    [Fact]
    public void IsNotUserFull_ExactlyFull_StillTrue_OriginalFlaw()
    {
        // ★ 原文如此：判定是 `Min > Max`（不是 >=）⇒ 正好满员时仍报"未满"
        MasSockGlobals.UserLimit[0].sServerName = "srva";
        MasSockGlobals.UserLimit[0].nLimitCountMin = 10;
        MasSockGlobals.UserLimit[0].nLimitCountMax = 10;

        using var f = NewForm();
        Assert.True(f.IsNotUserFull("srva"));
    }

    [Fact]
    public void IsNotUserFull_FirstMatchWins_Break()
    {
        MasSockGlobals.UserLimit[0].sServerName = "srva";
        MasSockGlobals.UserLimit[0].nLimitCountMin = 5;
        MasSockGlobals.UserLimit[0].nLimitCountMax = 10;
        MasSockGlobals.UserLimit[1].sServerName = "srva";       // 第二个同名条目：超限
        MasSockGlobals.UserLimit[1].nLimitCountMin = 99;
        MasSockGlobals.UserLimit[1].nLimitCountMax = 1;

        using var f = NewForm();
        Assert.True(f.IsNotUserFull("srva"));                   // 首个条目定胜负（break）
    }

    [Fact]
    public void ServerStatus_OfflineServer_Zero()
    {
        using var f = NewForm();
        Assert.Equal(0, f.ServerStatus("nosuch"));
    }

    [Fact]
    public void ServerStatus_OnlineButNoLimitEntry_Zero_Boundary()
    {
        using var f = NewForm();
        AddServer(f, "srva", 0, 0, "1.1.1.1");
        Assert.Equal(0, f.ServerStatus("srva"));
    }

    [Fact]
    public void ServerStatus_OnlyIndex99Entry_CountsAsOffline()
    {
        using var f = NewForm();
        AddServer(f, "srva", 99, 0, "1.1.1.1");
        MasSockGlobals.UserLimit[0].sServerName = "srva";
        MasSockGlobals.UserLimit[0].nLimitCountMax = 10;

        Assert.Equal(0, f.ServerStatus("srva"));
    }

    [Theory]
    [InlineData(0, 100, 1)]        // 0 <= 50      → 空闲
    [InlineData(50, 100, 1)]       // 50 <= 50     → 空闲
    [InlineData(51, 100, 2)]       // 51 <= 100-20 → 良好
    [InlineData(80, 100, 2)]       // 80 <= 80     → 良好
    [InlineData(81, 100, 3)]       // 81 < 100     → 繁忙
    [InlineData(99, 100, 3)]       // 99 < 100     → 繁忙
    [InlineData(100, 100, 4)]      // 100 >= 100   → 满员
    [InlineData(120, 100, 4)]      // 超限         → 满员
    public void ServerStatus_Levels(int min, int max, int expected)
    {
        MasSockGlobals.UserLimit[0].sServerName = "srva";
        MasSockGlobals.UserLimit[0].nLimitCountMin = min;
        MasSockGlobals.UserLimit[0].nLimitCountMax = max;

        using var f = NewForm();
        AddServer(f, "srva", 0, 0, "1.1.1.1");

        Assert.Equal(expected, f.ServerStatus("srva"));
    }

    [Fact]
    public void ServerStatus_ZeroMaxWithZeroMin_ReportsIdle_OriginalFlaw()
    {
        // ★ F10 原文如此：div 2 / div 5 都是整数截断；Max = 0 且 Min = 0 时第一条 `0 <= 0` 恒真
        //   ⇒ 上限根本没配也会报"空闲"
        MasSockGlobals.UserLimit[0].sServerName = "srva";
        MasSockGlobals.UserLimit[0].nLimitCountMin = 0;
        MasSockGlobals.UserLimit[0].nLimitCountMax = 0;

        using var f = NewForm();
        AddServer(f, "srva", 0, 0, "1.1.1.1");

        Assert.Equal(1, f.ServerStatus("srva"));
    }

    [Fact]
    public void ServerStatus_ZeroMaxWithPositiveMin_ReportsFull()
    {
        // 同一条原文判定的另一面：Max = 0 且 Min > 0 ⇒ 一路落到最后一条 `Min >= Max` ⇒ 满员
        MasSockGlobals.UserLimit[0].sServerName = "srva";
        MasSockGlobals.UserLimit[0].nLimitCountMin = 5;
        MasSockGlobals.UserLimit[0].nLimitCountMax = 0;

        using var f = NewForm();
        AddServer(f, "srva", 0, 0, "1.1.1.1");

        Assert.Equal(4, f.ServerStatus("srva"));
    }

    [Fact]
    public void CheckReadyServers_ZeroRequired_TrueEvenWhenEmpty_OriginalBoundary()
    {
        // g_Config.nReadyServers 初值 0 ⇒ 0 >= 0 ⇒ True
        using var f = NewForm();
        Assert.Equal(0, LoginSrvShare.g_Config.nReadyServers);
        Assert.True(f.CheckReadyServers());
    }

    [Fact]
    public void CheckReadyServers_ThresholdBoundary()
    {
        LoginSrvShare.g_Config.nReadyServers = 2;

        using var f = NewForm();
        Assert.False(f.CheckReadyServers());

        AddServer(f, "a", 0, 0, "1.1.1.1");
        Assert.False(f.CheckReadyServers());

        AddServer(f, "b", 1, 0, "2.2.2.2");
        Assert.True(f.CheckReadyServers());
    }

    // ========================================================================
    // LoadServerAddr（含 ★ F3/F4 缺陷）
    // ========================================================================

    [Fact]
    public void LoadServerAddr_ParsesThreeDotLines()
    {
        WriteServerAddrFile("1.2.3.4", "5.6.7.8");

        using var f = NewForm();

        Assert.Equal(2, MasSockGlobals.g_ServerAddrCount);
        Assert.Equal("1.2.3.4", MasSockGlobals.g_ServerAddr[0]);
        Assert.Equal("5.6.7.8", MasSockGlobals.g_ServerAddr[1]);
    }

    [Fact]
    public void LoadServerAddr_TrimsEachLine()
    {
        WriteServerAddrFile("  1.2.3.4  ");

        using var f = NewForm();

        Assert.Equal(1, MasSockGlobals.g_ServerAddrCount);
        Assert.Equal("1.2.3.4", MasSockGlobals.g_ServerAddr[0]);
    }

    [Fact]
    public void LoadServerAddr_SkipsLinesWithoutExactlyThreeDots()
    {
        WriteServerAddrFile("1.2.3", "1.2.3.4.5", "1.2.3.4");

        using var f = NewForm();

        Assert.Equal(1, MasSockGlobals.g_ServerAddrCount);
        Assert.Equal("1.2.3.4", MasSockGlobals.g_ServerAddr[0]);
    }

    [Fact]
    public void LoadServerAddr_SkipsEmptyLines()
    {
        WriteServerAddrFile("", "1.2.3.4");

        using var f = NewForm();

        Assert.Equal(1, MasSockGlobals.g_ServerAddrCount);
        Assert.Equal("1.2.3.4", MasSockGlobals.g_ServerAddr[0]);
    }

    [Fact]
    public void LoadServerAddr_TruncatesToShortString15()
    {
        WriteServerAddrFile("1111.2222.3333.4444");

        using var f = NewForm();

        Assert.Equal("1111.2222.3333.", MasSockGlobals.g_ServerAddr[0]);   // string[15] 赋值截断
    }

    [Fact]
    public void LoadServerAddr_MissingFile_ClearsArrayButKeepsCount_OriginalFlaw()
    {
        // ★ F4(a) 原文如此：FillChar 清空了数组，但计数只在 for 体内赋值 ⇒ 0 行时保留旧值
        using var f = NewForm();
        MasSockGlobals.g_ServerAddr[0] = "1.2.3.4";
        MasSockGlobals.g_ServerAddrCount = 5;

        f.LoadServerAddr();

        Assert.Equal("", MasSockGlobals.g_ServerAddr[0]);
        Assert.Equal(5, MasSockGlobals.g_ServerAddrCount);
    }

    [Fact]
    public void LoadServerAddr_EmptyFile_CountNotAssigned_OriginalFlaw()
    {
        WriteServerAddrFile();
        using var f = NewForm();
        MasSockGlobals.g_ServerAddrCount = 7;

        f.LoadServerAddr();

        Assert.Equal("", MasSockGlobals.g_ServerAddr[0]);
        Assert.Equal(7, MasSockGlobals.g_ServerAddrCount);          // ★ F4(a)
    }

    [Fact]
    public void LoadServerAddr_HundredValidLines_CountIsOffByOne_OriginalFlaw()
    {
        // ★ F4(b)：满 100 条时在赋值 count 之前 break ⇒ 计数停在 99
        WriteServerAddrFile(Enumerable.Range(0, 101).Select(i => "10.0.0." + i).ToArray());

        using var f = NewForm();

        Assert.Equal(99, MasSockGlobals.g_ServerAddrCount);
        Assert.Equal("10.0.0.99", MasSockGlobals.g_ServerAddr[99]);  // 第 100 条已入表但没被计数
    }

    [Fact]
    public void LoadServerAddr_CommentedLineAtFirstIndex_IsSkipped()
    {
        WriteServerAddrFile("1.1.1.1", ";2.2.2.2");                 // I=1 → sLineText[1] = ';' → 跳过
        using var f = NewForm();
        Assert.Equal(1, MasSockGlobals.g_ServerAddrCount);
    }

    [Fact]
    public void LoadServerAddr_CommentedLineAtOtherIndex_StillAccepted_OriginalFlaw()
    {
        // ★ F3 原文如此：判注释用的是 `sLineText[I]`，I 是**行号** ⇒ 只有第 1 行（I=1）判对了。
        //    行 3 的 ";4.4.4.4" 会被当成合法地址（判的是 1-based 第 3 个字符 '4'）。
        WriteServerAddrFile("1.1.1.1", "2.2.2.2", "3.3.3.3", ";4.4.4.4");

        using var f = NewForm();

        Assert.Equal(4, MasSockGlobals.g_ServerAddrCount);
        Assert.Equal(";4.4.4.4", MasSockGlobals.g_ServerAddr[3]);
    }

    // ========================================================================
    // LoadUserLimit（经 FormCreate 驱动；含 ★ F5/F6）
    // ========================================================================

    [Fact]
    public void LoadUserLimit_ParsesThreeTokensAndDefaults()
    {
        WriteUserLimitFile("srvA authA 500", "srvB authB");

        using var f = NewForm();

        Assert.Equal(2, MasSockGlobals.nUserLimit);
        Assert.Equal("srvA", MasSockGlobals.UserLimit[0].sServerName);
        Assert.Equal("authA", MasSockGlobals.UserLimit[0].sName);
        Assert.Equal(500, MasSockGlobals.UserLimit[0].nLimitCountMax);
        Assert.Equal(0, MasSockGlobals.UserLimit[0].nLimitCountMin);
        Assert.Equal("srvB", MasSockGlobals.UserLimit[1].sServerName);
        Assert.Equal("authB", MasSockGlobals.UserLimit[1].sName);
        Assert.Equal(3000, MasSockGlobals.UserLimit[1].nLimitCountMax);   // StrToIntDef(...,3000)
    }

    [Fact]
    public void LoadUserLimit_TabSeparatedAndChineseName()
    {
        WriteUserLimitFile("srvA\t授权甲\t800");

        using var f = NewForm();

        Assert.Equal(1, MasSockGlobals.nUserLimit);
        Assert.Equal("srvA", MasSockGlobals.UserLimit[0].sServerName);
        Assert.Equal("授权甲", MasSockGlobals.UserLimit[0].sName);          // GBK 往返
        Assert.Equal(800, MasSockGlobals.UserLimit[0].nLimitCountMax);
    }

    [Fact]
    public void LoadUserLimit_WhitespaceOnlyLine_BecomesEntryWithBlankName_OriginalFlaw()
    {
        // 原文如此：行内**全是分隔符**时 GetValidStr3 的 `StartIndex > 1` 不成立 ⇒
        // Dest 保持**原串**（"   "）⇒ `sServerName <> ''` 为真 ⇒ 仍然占一个条目位（名字是 3 个空格）
        WriteUserLimitFile("   ", "srvA authA 10");

        using var f = NewForm();

        Assert.Equal(2, MasSockGlobals.nUserLimit);
        Assert.Equal("   ", MasSockGlobals.UserLimit[0].sServerName);
        Assert.Equal("", MasSockGlobals.UserLimit[0].sName);
        Assert.Equal(3000, MasSockGlobals.UserLimit[0].nLimitCountMax);   // s14 = '' ⇒ 缺省
        Assert.Equal("srvA", MasSockGlobals.UserLimit[1].sServerName);
    }

    [Fact]
    public void LoadUserLimit_EmptyFile_ZeroEntries()
    {
        WriteUserLimitFile();

        using var f = NewForm();

        Assert.Equal(0, MasSockGlobals.nUserLimit);
    }

    [Fact]
    public void LoadUserLimit_Over100Entries_Throws_OriginalFlaw()
    {
        // ★ F5 原文如此：写 UserLimit[nC] 无上界检查（Delphi：写越界内存；托管：IndexOutOfRangeException）
        WriteUserLimitFile(Enumerable.Range(0, 101).Select(i => "srv" + i + " auth" + i + " 10").ToArray());

        var f = new TFrmMasSoc();
        Assert.Throws<IndexOutOfRangeException>(() => f.FormCreate(f));
        f.Dispose();
    }

    [Fact]
    public void LoadUserLimit_StaleEntriesSurviveReload_OriginalFlaw()
    {
        // ★ F6 原文如此：装载前不清空 UserLimit ⇒ 残留条目仍参与 LimitName 判定
        WriteUserLimitFile("srvA authA 10", "srvB authB 20");
        using var f = NewForm();
        Assert.Equal(2, MasSockGlobals.nUserLimit);

        WriteUserLimitFile("srvA authA 10");        // 第二次只剩 1 条
        f.FormCreate(f);
        Assert.Equal(1, MasSockGlobals.nUserLimit);
        Assert.Equal("srvB", MasSockGlobals.UserLimit[1].sServerName);     // ★ 残留

        // 残留条目仍能被 LimitName 命中 ⇒ 名字为 srvB 的服务器一条消息都收不到
        var t = AddServer(f, "srvB", 0, 0, "1.1.1.1");
        f.SendServerMsg((ushort)CommonConst.SS_CLOSESESSION, "srvB", "acc/1");
        Assert.Empty(t.Socket.SentTexts);
    }

    [Fact]
    public void LoadUserLimit_NoStaleEntry_MessageReachesServer_Contrast()
    {
        // 与上一用例对照：一开始就只有 1 条 ⇒ 没有残留 ⇒ LimitName('srvB') = '' ⇒ 无条件发送
        WriteUserLimitFile("srvA authA 10");
        using var f = NewForm();

        var t = AddServer(f, "srvB", 0, 0, "1.1.1.1");
        f.SendServerMsg((ushort)CommonConst.SS_CLOSESESSION, "srvB", "acc/1");
        Assert.Equal(new[] { "(1010/acc/1)" }, t.Socket.SentTexts.ToArray());
    }

    // ========================================================================
    // 局部工具（放在末尾，避免打断用例阅读）
    // ========================================================================

    private static TAccountInfo BuildAccount(string account, string password, string userName,
        string q1, string a1, string q2, string a2, string birthDay, string mobile, string phone, string mail)
    {
        var info = new TAccountInfo();
        info.AccountNameStr = account;
        info.PasswordStr = password;
        info.UserNameStr = userName;
        info.Questions1Str = q1;
        info.Answers1Str = a1;
        info.Questions2Str = q2;
        info.Answers2Str = a2;
        info.BirthDayStr = birthDay;
        info.MobilePhoneStr = mobile;
        info.PhoneStr = phone;
        info.MailStr = mail;
        return info;
    }
}

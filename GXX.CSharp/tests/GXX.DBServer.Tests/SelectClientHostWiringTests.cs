using System;
using System.Collections.Generic;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// 宿主接线后的**真实接缝状态**（不是替身）：
/// `DBServerService` 构造时接上 `g_RoleDB.HumanDB/HeroDB`，
/// 而 `IDSocCli` / `DBShare 名校验族` 保持**未接线**（集成方裁定 §12：保持抛异常，不做放行桩）。
///
/// ★ 本组刻意**不派生** <see cref="SelectClientTestBase"/> —— 那个基类会把两个未移植的接缝替换成替身，
///   就测不到"生产默认态"了。
/// </summary>
public class SelectClientHostWiringTests : TempDirTest
{
    private readonly List<string> Logs = new();

    public SelectClientHostWiringTests()
    {
        TSelectClient.ResetSeams();
        SelectClientGateWiring.DetachAll();
        IDSocCliSeam.Reset();
        SelectClientDbShareSeam.Reset();
        SelectClientRoleDbSeam.Reset();
        SelectClientGlobals.Reset();
        RoleDbSeam.MainOutMessage = s => Logs.Add(s);
    }

    // =====================================================================================
    // DBServerService 构造 → RoleDB 接缝
    // =====================================================================================

    [Fact]
    public void 构造DBServerService_把RoleDatabase接到HumanDB与HeroDB()
    {
        using var srv = new DBServerService(Path2("host.db"));

        Assert.NotNull(SelectClientRoleDbSeam.HumanDB);
        Assert.NotNull(SelectClientRoleDbSeam.HeroDB);
        Assert.IsType<SelectClientHumanDb>(SelectClientRoleDbSeam.HumanDB);
        Assert.IsType<SelectClientHeroDb>(SelectClientRoleDbSeam.HeroDB);
    }

    [Fact]
    public void 接线后HumanDB闭环_Add到QueryHumans()
    {
        using var srv = new DBServerService(Path2("host.db"));
        var human = SelectClientRoleDbSeam.RequireHuman;

        Assert.True(human.Add("acct", "Aaaa", true, 1, 2, 3));
        Assert.Equal(1, human.GetHumanCount("acct"));
        Assert.NotEqual(RoleDbConst.NO_ID, human.GetID("Aaaa"));

        var list = new TQueryHumanList();
        Assert.Equal(1, human.QueryHumans("acct", list));
        Assert.Equal("Aaaa", list.Items(0)!.HumanName);
        Assert.True(list.Items(0)!.IsSelect);
    }

    [Fact]
    public void 接线后HeroDB闭环_GetID只认英雄表()
    {
        using var srv = new DBServerService(Path2("host.db"));
        SelectClientRoleDbSeam.RequireHuman.Add("acct", "Aaaa", false, 0, 0, 0);

        Assert.Equal(RoleDbConst.NO_ID, SelectClientRoleDbSeam.RequireHero.GetID("Aaaa"));
    }

    [Fact]
    public void Dispose_断开RoleDB接缝_不留悬垂()
    {
        var srv = new DBServerService(Path2("host.db"));
        Assert.NotNull(SelectClientRoleDbSeam.HumanDB);

        srv.Dispose();

        Assert.Null(SelectClientRoleDbSeam.HumanDB);
        Assert.Null(SelectClientRoleDbSeam.HeroDB);
        Assert.Throws<NotSupportedException>(() => SelectClientRoleDbSeam.RequireHuman);
    }

    // =====================================================================================
    // 未移植的接缝：**没有放行桩**（集成方裁定 §12 方案 (a)）
    // =====================================================================================

    [Fact]
    public void IDSocCli接缝_未接线_访问即抛NotSupportedException()
    {
        using var srv = new DBServerService(Path2("host.db"));

        Assert.Null(IDSocCliSeam.FrmIDSoc);
        var ex = Assert.Throws<NotSupportedException>(() => IDSocCliSeam.Require);
        Assert.Contains("IDSocCli.pas", ex.Message);
    }

    [Fact]
    public void DBShare名校验接缝_未接线_访问即抛NotSupportedException()
    {
        using var srv = new DBServerService(Path2("host.db"));

        Assert.Throws<NotSupportedException>(() => SelectClientDbShareSeam.CheckChrName("Aaaa"));
        Assert.Throws<NotSupportedException>(() => SelectClientDbShareSeam.CheckSpecialChar("Aaaa"));
        Assert.Throws<NotSupportedException>(() => SelectClientDbShareSeam.CheckDenyChrName("Aaaa"));
        Assert.Throws<NotSupportedException>(() => SelectClientDbShareSeam.CheckNumberName("Aaaa"));
        Assert.Throws<NotSupportedException>(() => SelectClientDbShareSeam.CheckLetterName("Aaaa"));
        Assert.Throws<NotSupportedException>(() => SelectClientDbShareSeam.CheckFilterNewHumanChrName("Aaaa"));
    }

    [Fact]
    public void 主动网关路由接缝_未接线_访问即抛NotSupportedException()
    {
        using var srv = new DBServerService(Path2("host.db"));

        Assert.Throws<NotSupportedException>(() => SelectClientDbShareSeam.GateActiveRouteIP("127.0.0.1", out _));
        Assert.Throws<NotSupportedException>(() => SelectClientDbShareSeam.CheckActiveRunGate("127.0.0.1", 7200));
    }

    // =====================================================================================
    // ★ 因此这几条命令目前**响亮地抛异常**（而不是静默放行）
    // =====================================================================================

    private static string UserDataFrame(string connID, int ident, string arg)
    {
        byte[] payload = arg == ""
            ? EDcode.EncodeMessage(TDefaultMessage.Make((ushort)ident, 0, 0, 0, 0))
            : Concat(EDcode.EncodeMessage(TDefaultMessage.Make((ushort)ident, 0, 0, 0, 0)), EDcode.EncodeString(arg));
        return "%A" + connID + "/#1" + System.Text.Encoding.Latin1.GetString(payload) + "!$";
    }

    private static byte[] Concat(byte[] a, byte[] b)
    {
        byte[] r = new byte[a.Length + b.Length];
        Array.Copy(a, 0, r, 0, a.Length);
        Array.Copy(b, 0, r, a.Length, b.Length);
        return r;
    }

    private TSelectClient AttachAndOpenSlot(out TUserInfo slot)
    {
        var c = SelectClientGateWiring.Attach(_ => { }, "10.9.9.9");
        byte[] open = System.Text.Encoding.Latin1.GetBytes("%O7/1.1.1.1/2.2.2.2$");
        SelectClientGateWiring.Feed(c, open, 0, open.Length);
        slot = c.SelectCharList.OnLineItems(0)!;
        Assert.Equal("7", slot.sConnID);
        slot.dwChrTick = 0;
        DelphiTick.GetTickCount = () => 100000;
        return c;
    }

    [Theory]
    [InlineData(Grobal2Const.CM_QUERYCHR)]
    [InlineData(Grobal2Const.CM_RANDOMNAME)]
    [InlineData(Grobal2Const.CM_NEWCHR)]
    [InlineData(Grobal2Const.CM_DELCHR)]
    [InlineData(Grobal2Const.CM_SELCHR)]
    public void 依赖IDSocCli的五条命令_目前抛NotSupportedException(int ident)
    {
        using var srv = new DBServerService(Path2("host.db"));
        var c = AttachAndOpenSlot(out var slot);
        slot.sAccount = "acct";                                                   // 绕开 `sAccount <> ''` 短路，直达 CheckSession

        string frame = UserDataFrame("7", ident, "acct/42");
        byte[] bytes = System.Text.Encoding.Latin1.GetBytes(frame);
        Exception? ex = SelectClientGateWiring.FeedSafe(c, bytes, 0, bytes.Length);

        Assert.IsType<NotSupportedException>(ex);
        Assert.Contains(Logs, s => s.Contains("SelectClient 命令处理抛异常"));
    }

    [Fact]
    public void 账号为空的短路径不发包也不抛_走OutOfConnect()
    {
        // DeCodeUserMsg 里 `(sAccount <> '') and CheckSession(...)` 是**短路与**：
        // 账号为空时不会碰 IDSocCli ⇒ 不抛，而是 OutOfConnect。
        using var srv = new DBServerService(Path2("host.db"));
        var sent = new List<byte[]>();
        var c = SelectClientGateWiring.Attach(sent.Add, "10.9.9.9");
        byte[] open = System.Text.Encoding.Latin1.GetBytes("%O7/1.1.1.1$");
        SelectClientGateWiring.Feed(c, open, 0, open.Length);
        c.SelectCharList.OnLineItems(0)!.dwChrTick = 0;
        DelphiTick.GetTickCount = () => 100000;

        string frame = UserDataFrame("7", Grobal2Const.CM_DELCHR, "Aaaa");
        byte[] bytes = System.Text.Encoding.Latin1.GetBytes(frame);
        Assert.Null(SelectClientGateWiring.FeedSafe(c, bytes, 0, bytes.Length));

        Assert.Single(sent);
        Assert.Equal(Grobal2Const.SM_OUTOFCONNECTION,
                     EDcode.DecodeMessage(Slice(sent[0])).Ident);
    }

    [Fact]
    public void 不依赖IDSocCli的两条命令_目前可以真正跑通()
    {
        // CM_QUERYDELCHR / CM_GETBACKDELCHR 在原文里**不做** CheckSession ⇒ 接线后立即可用。
        using var srv = new DBServerService(Path2("host.db"));
        SelectClientRoleDbSeam.RequireHuman.Add("acct", "Gone", false, 0, 0, 0);
        SelectClientRoleDbSeam.RequireHuman.Delete("acct", "Gone");

        var sent = new List<byte[]>();
        var c = SelectClientGateWiring.Attach(sent.Add, "10.9.9.9");
        byte[] open = System.Text.Encoding.Latin1.GetBytes("%O7/1.1.1.1$");
        SelectClientGateWiring.Feed(c, open, 0, open.Length);
        c.SelectCharList.OnLineItems(0)!.dwChrTick = 0;
        DelphiTick.GetTickCount = () => 100000;

        string frame = UserDataFrame("7", Grobal2Const.CM_QUERYDELCHR, "acct");
        byte[] bytes = System.Text.Encoding.Latin1.GetBytes(frame);
        Assert.Null(SelectClientGateWiring.FeedSafe(c, bytes, 0, bytes.Length));

        Assert.Single(sent);
        TDefaultMessage msg = EDcode.DecodeMessage(Slice(sent[0]));
        Assert.Equal(Grobal2Const.SM_QUERYDELCHR, msg.Ident);
        Assert.Equal(1, msg.Recog);                                               // 已删角色数在 Recog
    }

    [Fact]
    public void 帧X_命中槽位时不抛_跳过清理_留痕_且不断连接()
    {
        // ★ 裁定（偏差 **D-p7-13**，唯一窄口子）：`CloseUser`（SelectClient.pas:714 `GetGlobaSessionStatus`）
        //   在 FrmIDSoc 未接线时 **记日志 + 跳过清理 + 继续**，**不抛**、**不断连接**。
        //   理由：它只决定"要不要发一条清理通知"，不是校验判定；而抛出去的代价是
        //   **整条 SelGate 连接被断开**（一条连接上通常挂着多个无关玩家）。
        //   其余接缝（CheckSession / 名校验族 / 主动网关路由）**一条都不放宽** —— 见本文件的另两条用例。
        using var srv = new DBServerService(Path2("host.db"));
        var sent = new List<byte[]>();
        var c = SelectClientGateWiring.Attach(sent.Add, "10.9.9.9");
        byte[] open = System.Text.Encoding.Latin1.GetBytes("%O7/1.1.1.1$");
        SelectClientGateWiring.Feed(c, open, 0, open.Length);
        c.SelectCharList.OnLineItems(0)!.nSessionID = 99;

        byte[] close = System.Text.Encoding.Latin1.GetBytes("%X7$");
        Exception? ex = SelectClientGateWiring.FeedSafe(c, close, 0, close.Length);

        Assert.Null(ex);                                                          // 不抛
        Assert.Equal(1, SelectClientGateWiring.Count);                            // 绑定还在 ⇒ 宿主没有 Detach
        Assert.Empty(sent);                                                       // 清理通知确实被跳过（没发包给 LoginSrv）
        Assert.Contains(Logs, s => s.Contains("D-p7-13") && s.Contains("99"));     // 每次触发都留痕，带编号与会话号
        Assert.Equal(0, c.SelectCharList.OnLineCount);                            // 槽位照常回收（原文 :719 的 Finalize 仍然执行）
        Assert.Equal("", c.m_sReceiveText);
    }

    [Fact]
    public void 帧X_命中槽位但FrmIDSoc已接线时_回到原文分支()
    {
        // 对照组：一旦 IDSocCli 移植并接线，窄口子自动失效（`CloseUser_ShouldCloseSession` 直接问 FrmIDSoc）。
        using var srv = new DBServerService(Path2("host.db"));
        var idSoc = new FakeFrmIDSoc { SessionStatus = false };                    // 会话已失效 ⇒ 应当清理
        IDSocCliSeam.FrmIDSoc = idSoc;
        try
        {
            var c = SelectClientGateWiring.Attach(_ => { }, "10.9.9.9");
            byte[] open = System.Text.Encoding.Latin1.GetBytes("%O7/1.1.1.1$");
            SelectClientGateWiring.Feed(c, open, 0, open.Length);
            c.SelectCharList.OnLineItems(0)!.nSessionID = 99;
            c.SelectCharList.OnLineItems(0)!.sAccount = "acct";

            byte[] close = System.Text.Encoding.Latin1.GetBytes("%X7$");
            Assert.Null(SelectClientGateWiring.FeedSafe(c, close, 0, close.Length));

            Assert.Single(idSoc.SocketMsgs);
            Assert.Equal((ushort)CommonConst.SS_SOFTOUTSESSION, idSoc.SocketMsgs[0].Ident);
            Assert.Single(idSoc.ClosedSessions);
            Assert.DoesNotContain(Logs, s => s.Contains("D-p7-13"));               // 接线后不留 D-p7-13 痕迹
        }
        finally
        {
            IDSocCliSeam.Reset();
        }
    }

    [Fact]
    public void 帧X_会话仍活跃时不做清理()
    {
        using var srv = new DBServerService(Path2("host.db"));
        var idSoc = new FakeFrmIDSoc { SessionStatus = true };
        IDSocCliSeam.FrmIDSoc = idSoc;
        try
        {
            var c = SelectClientGateWiring.Attach(_ => { }, "10.9.9.9");
            byte[] open = System.Text.Encoding.Latin1.GetBytes("%O7/1.1.1.1$");
            SelectClientGateWiring.Feed(c, open, 0, open.Length);
            c.SelectCharList.OnLineItems(0)!.nSessionID = 99;

            byte[] close = System.Text.Encoding.Latin1.GetBytes("%X7$");
            Assert.Null(SelectClientGateWiring.FeedSafe(c, close, 0, close.Length));

            Assert.Empty(idSoc.SocketMsgs);                                       // 原文 `if not ...Status` 为假 ⇒ 不清理
            Assert.Single(idSoc.StatusCalls);
        }
        finally
        {
            IDSocCliSeam.Reset();
        }
    }

    [Fact]
    public void 帧X_不命中槽位时不抛()
    {
        // 对照：没有匹配 ConnID 时 CloseUser 的 for 循环体根本不执行 ⇒ 不碰 FrmIDSoc。
        using var srv = new DBServerService(Path2("host.db"));
        var c = SelectClientGateWiring.Attach(_ => { }, "10.9.9.9");
        byte[] close = System.Text.Encoding.Latin1.GetBytes("%X999$");

        Assert.Null(SelectClientGateWiring.FeedSafe(c, close, 0, close.Length));
    }

    /// <summary>从 <c>%&lt;sid&gt;/#…!$</c> 里取出 22 字节编码头。</summary>
    private static byte[] Slice(byte[] raw)
    {
        int start = 0;
        for (int i = 0; i + 1 < raw.Length; i++)
            if (raw[i] == (byte)'/' && raw[i + 1] == (byte)'#') { start = i + 2; break; }
        byte[] head = new byte[Grobal2Const.DEF_BLOCK_SIZE];
        Array.Copy(raw, start, head, 0, Grobal2Const.DEF_BLOCK_SIZE);
        return head;
    }
}

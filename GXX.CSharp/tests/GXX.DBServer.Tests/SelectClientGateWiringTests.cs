using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.DBServer;
using GXX.GatewayKit;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// `SelectClientGateWiring` —— SelGate 连接 ↔ TSelectClient 的接线层
/// （对应 uFrmMain.pas:303-306 / :335-338 / :340-353）。
///
/// 本组同时是「SelGate 命令路径」的**第一批端到端用例**：在这之前，
/// `DBServerService.ProcessGateData` 的 4 条命令**一条测试都没有**（§28.3 双根搜索结果）。
/// </summary>
public class SelectClientGateWiringTests : SelectClientTestBase
{
    private readonly List<byte[]> Raw = new();
    private readonly List<SentFrame> Out = new();

    public SelectClientGateWiringTests()
    {
        SelectClientGateWiring.DetachAll();
    }

    private TSelectClient Attach(Action<byte[]>? sink = null, string remote = "10.9.9.9")
    {
        return SelectClientGateWiring.Attach(b =>
        {
            Raw.Add(b);
            Out.Add(ParseFrame(b));
        }, remote);
    }

    private static TDefaultMessage Cmd(int ident)
        => TDefaultMessage.Make((ushort)ident, 0, 0, 0, 0);

    // =====================================================================================
    // 接线 / 断线
    // =====================================================================================

    [Fact]
    public void Attach_为一条连接建一个会话表并计数()
    {
        var c = Attach();

        Assert.NotNull(c);
        Assert.Equal(1000, c.SelectCharList.Count);                               // 每连接的 1000 槽会话表
        Assert.Equal(0, c.SelectCharList.OnLineCount);
        Assert.Equal(1, SelectClientGateWiring.Count);
    }

    [Fact]
    public void Attach_出站路由到所属连接()
    {
        var c = Attach();

        c.OutOfConnect("42");

        Assert.Single(Raw);
        Assert.Equal("%42/#" + System.Text.Encoding.Latin1.GetString(EDcode.EncodeMessage(Cmd(Grobal2Const.SM_OUTOFCONNECTION))) + "!$",
                     System.Text.Encoding.Latin1.GetString(Raw[0]));
    }

    [Fact]
    public void 两条连接互不串扰()
    {
        var listA = new List<byte[]>();
        var listB = new List<byte[]>();
        var a = SelectClientGateWiring.Attach(b => listA.Add(b), "10.0.0.1");
        var b = SelectClientGateWiring.Attach(x => listB.Add(x), "10.0.0.2");

        a.OutOfConnect("1");
        b.OutOfConnect("2");
        a.OutOfConnect("3");

        Assert.Equal(2, listA.Count);
        Assert.Single(listB);
        Assert.StartsWith("%1/#", System.Text.Encoding.Latin1.GetString(listA[0]));
        Assert.StartsWith("%2/#", System.Text.Encoding.Latin1.GetString(listB[0]));
        Assert.StartsWith("%3/#", System.Text.Encoding.Latin1.GetString(listA[1]));
        Assert.Equal(2, SelectClientGateWiring.Count);
    }

    [Fact]
    public void RemoteAddress_来自Attach的第二参()
    {
        var c = SelectClientGateWiring.Attach(_ => { }, "192.168.7.7");

        c.ExecGateBuffers("%S10.0.0.1$");                                          // 非 127.0.0. 前缀 ⇒ 取 RemoteAddress

        Assert.Equal("192.168.7.7", c.m_sGateaddr);
    }

    [Fact]
    public void Detach_之后出站抛异常而不是静默丢弃()
    {
        var c = Attach();
        SelectClientGateWiring.Detach(c);

        Assert.Equal(0, SelectClientGateWiring.Count);
        var ex = Assert.Throws<InvalidOperationException>(() => c.OutOfConnect("1"));
        Assert.Contains("SelectClientGateWiring.Attach", ex.Message);
    }

    [Fact]
    public void 未Attach的客户端出站抛异常而不是静默丢弃()
    {
        TSelectClient.ResetSeams();
        var orphan = new TSelectClient();

        Assert.Throws<NotSupportedException>(() => orphan.OutOfConnect("1"));      // 接缝本身的默认实现
    }

    [Fact]
    public void DetachAll_清空全部绑定()
    {
        Attach();
        Attach();
        Assert.Equal(2, SelectClientGateWiring.Count);

        SelectClientGateWiring.DetachAll();

        Assert.Equal(0, SelectClientGateWiring.Count);
    }

    [Fact]
    public void AttachTcpLink_未连接的TcpLink也能安全接线()
    {
        var link = new TcpLink("127.0.0.1", 1);                                   // 只构造、不 Connect ⇒ 无任何 I/O

        var c = SelectClientGateWiring.AttachTcpLink(link, "10.1.1.1");

        Assert.NotNull(c);
        Assert.Equal(1, SelectClientGateWiring.Count);
        c.OutOfConnect("9");                                                      // TcpLink.Send 在未连接时是 no-op
        Assert.Equal("10.1.1.1", c.RemoteAddress);
    }

    // =====================================================================================
    // 收包
    // =====================================================================================

    [Fact]
    public void Feed_整包建槽()
    {
        var c = Attach();
        byte[] frame = System.Text.Encoding.Latin1.GetBytes("%O7/1.2.3.4/5.6.7.8$");

        SelectClientGateWiring.Feed(c, frame, 0, frame.Length);

        Assert.Equal(1, c.SelectCharList.OnLineCount);
        Assert.Same(c, c.SelectCharList.OnLineItems(0)!.Socket);
        Assert.Equal("7", c.SelectCharList.OnLineItems(0)!.sConnID);
    }

    [Fact]
    public void Feed_带偏移的切片等价于同长度整包()
    {
        var a = Attach();
        var b = Attach();
        byte[] payload = System.Text.Encoding.Latin1.GetBytes("%O7/1.2.3.4$");
        byte[] padded = new byte[payload.Length + 5];
        Array.Copy(payload, 0, padded, 3, payload.Length);

        SelectClientGateWiring.Feed(a, payload, 0, payload.Length);
        SelectClientGateWiring.Feed(b, padded, 3, payload.Length);

        Assert.Equal(a.SelectCharList.OnLineItems(0)!.sConnID, b.SelectCharList.OnLineItems(0)!.sConnID);
        Assert.Equal(a.m_sReceiveText, b.m_sReceiveText);
    }

    [Fact]
    public void Feed_半包保留在m_sReceiveText_补齐后完成()
    {
        var c = Attach();
        byte[] half = System.Text.Encoding.Latin1.GetBytes("%O7/1.2.3");

        SelectClientGateWiring.Feed(c, half, 0, half.Length);

        Assert.Equal("%O7/1.2.3", c.m_sReceiveText);                              // 没有 '$' ⇒ 原样保留
        Assert.Equal(0, c.SelectCharList.OnLineCount);

        byte[] rest = System.Text.Encoding.Latin1.GetBytes(".4/5.6.7.8$");
        SelectClientGateWiring.Feed(c, rest, 0, rest.Length);

        Assert.Equal("", c.m_sReceiveText);
        Assert.Equal(1, c.SelectCharList.OnLineCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Feed_长度非正时不做事(int len)
    {
        var c = Attach();
        byte[] frame = System.Text.Encoding.Latin1.GetBytes("%O7/1.2.3.4$");

        SelectClientGateWiring.Feed(c, frame, 0, len);

        Assert.Equal("", c.m_sReceiveText);
        Assert.Equal(0, c.SelectCharList.OnLineCount);
    }

    [Fact]
    public void Feed_空缓冲不抛异常()
    {
        var c = Attach();
        SelectClientGateWiring.Feed(c, null!, 0, 10);
        Assert.Equal("", c.m_sReceiveText);
    }

    // =====================================================================================
    // ★ SelGate 命令路径端到端（同时是 `DBServerService.cs:135` 字段位置的**验收基准**）
    // =====================================================================================

    /// <summary>接线一条连接并投递一串字节（原文 OnClientRead 的形状）。</summary>
    private TSelectClient AttachAndFeed(string remote, params string[] ansiFrames)
    {
        var c = Attach(remote: remote);
        foreach (string f in ansiFrames)
        {
            byte[] b = System.Text.Encoding.Latin1.GetBytes(f);
            SelectClientGateWiring.Feed(c, b, 0, b.Length);
        }
        return c;
    }

    /// <summary>接一条已建立的连接，并让槽 7 处于「可过 200/1000ms 节流」的状态。</summary>
    private TSelectClient AttachOpenSlot(string connID = "7", string remote = "10.9.9.9")
    {
        var c = AttachAndFeed(remote, "%O" + connID + "/1.1.1.1/2.2.2.2$");
        Assert.Equal(1, c.SelectCharList.OnLineCount);
        c.SelectCharList.OnLineItems(0)!.sAccount = "acct";
        c.SelectCharList.OnLineItems(0)!.dwChrTick = 0;
        DelphiTick.GetTickCount = () => 100000;
        return c;
    }

    private void FeedFrame(TSelectClient c, int ident, string arg, string connID = "7")
    {
        string frame = UserDataFrame(connID, Cmd(ident), arg);
        byte[] bytes = System.Text.Encoding.Latin1.GetBytes(frame);
        SelectClientGateWiring.Feed(c, bytes, 0, bytes.Length);
    }

    [Fact]
    public void 端到端_CM_QUERYCHR_角色数在Recog且Tag为1()
    {
        // ★★ 这一条是 `DBServerService.cs:135` 的**正确目标**：
        //    原文 `MakeDefaultMsg(SM_QUERYCHR, nChrCount, 0, 1, 0)`（SelectClient.pas:1228）
        //    ⇒ Ident=520、**Recog=角色数**、Param=0、Tag=1、Series=0。
        //    现行 DBServerService 写的是 `Make(520, msg.Recog, n>0?1:0, 0, min(n,16))` ⇒ Param=n>0、Tag=0，**两处都错**。
        var c = AttachOpenSlot();                                                 // HumanDb.Humans 为空 ⇒ 角色数 0
        FeedFrame(c, Grobal2Const.CM_QUERYCHR, "acct/42");

        Assert.Single(Out);
        Assert.Equal(Grobal2Const.SM_QUERYCHR, Out[0].Msg.Ident);
        Assert.Equal(520, Out[0].Msg.Ident);
        Assert.Equal(0, Out[0].Msg.Recog);
        Assert.Equal(0, Out[0].Msg.Param);                                        // ★ 不是 "n>0"
        Assert.Equal(1, Out[0].Msg.Tag);                                          // ★ 原文 wTag = 1
        Assert.Equal(0, Out[0].Msg.Series);
    }

    [Fact]
    public void 端到端_CM_QUERYCHR_角色数落在Recog上()
    {
        var c = AttachOpenSlot();
        HumanDb.Humans.Add(new TQueryHumanData { HumanName = "Aaaa", Job = 0, Hair = 1, Level = 2, Sex = 0 });
        HumanDb.Humans.Add(new TQueryHumanData { HumanName = "Bbbb", Job = 1, Hair = 2, Level = 3, Sex = 1, IsSelect = true });
        FeedFrame(c, Grobal2Const.CM_QUERYCHR, "acct/42");

        Assert.Single(Out);
        Assert.Equal(2, Out[0].Msg.Recog);                                        // ★ 角色数在 Recog
        Assert.Equal(0, Out[0].Msg.Param);
        Assert.Equal(1, Out[0].Msg.Tag);
        Assert.Equal("Aaaa/0/1/2/0/*Bbbb/1/2/3/1/", DecodeBody(Out[0].Body));
    }

    [Fact]
    public void 端到端_未知命令回SM_CHECKISMYSELFSERVER()
    {
        var c = AttachAndFeed("10.9.9.9", "%O7/1.1.1.1$");
        Assert.Equal(1, c.SelectCharList.OnLineCount);                            // 先确认槽位已建
        Assert.Equal("7", c.SelectCharList.OnLineItems(0)!.sConnID);
        FeedFrame(c, 4242, "");
        Assert.Equal("", c.SelectCharList.OnLineItems(0)!.sReceiveText);          // 帧已被取走
        Assert.Single(Raw);                                                       // 确实发了包

        Assert.Single(Out);
        Assert.Equal(8889, Out[0].Msg.Ident);                                     // 现行 ProcessGateData 回的是 UNKNOWMSG
    }

    [Fact]
    public void 端到端_百分号O之后百分号X回收槽位()
    {
        var c = Attach();
        foreach (string f in new[] { "%O7/1.1.1.1$", "%X7$" })
        {
            byte[] b = System.Text.Encoding.Latin1.GetBytes(f);
            SelectClientGateWiring.Feed(c, b, 0, b.Length);
        }

        Assert.Equal(0, c.SelectCharList.OnLineCount);
        Assert.Null(c.SelectCharList.Items(0)!.Socket);
        Assert.Equal("", c.SelectCharList.Items(0)!.sConnID);
    }

    [Fact]
    public void 端到端_心跳帧逐字节等于原文的百分号加号加号美元()
    {
        var c = Attach();
        byte[] b = System.Text.Encoding.Latin1.GetBytes("%-$");
        SelectClientGateWiring.Feed(c, b, 0, b.Length);

        Assert.Single(Raw);
        Assert.Equal("%++$", System.Text.Encoding.Latin1.GetString(Raw[0]));
    }

    [Fact]
    public void 端到端_粘包两条帧都被处理()
    {
        var c = Attach();
        byte[] b = System.Text.Encoding.Latin1.GetBytes("%O7/1.1.1.1$%-$");
        SelectClientGateWiring.Feed(c, b, 0, b.Length);

        Assert.Equal(1, c.SelectCharList.OnLineCount);
        Assert.Equal("%++$", System.Text.Encoding.Latin1.GetString(Raw[0]));
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// SelectClient.pas:444-885 —— 收包/解帧（<c>ExecGateBuffers</c>）、逐槽取帧（<c>ProcessUserMsg</c>）
/// 与消息分发内核（<c>DeCodeUserMsg</c>）。
/// </summary>
public class SelectClientFrameTests : SelectClientTestBase
{
    private static TDefaultMessage Cmd(int ident, int param = 0, int tag = 0, int series = 0)
        => TDefaultMessage.Make((ushort)ident, 0, (ushort)param, (ushort)tag, (ushort)series);

    // =====================================================================================
    // 生命周期
    // =====================================================================================

    [Fact]
    public void 构造_字段初值逐字照抄原文()
    {
        DelphiTick.GetTickCount = () => 1234;
        var c = new TSelectClient();

        Assert.Equal(1234u, c.m_dwKeepAliveTick);                                 // :267
        Assert.Equal("", c.m_sReceiveText);                                       // :268
        Assert.Equal("", c.m_sGateaddr);                                          // :269
        Assert.Equal(1234u, c.m_dwTick10);                                        // :270
        Assert.Equal(0, c.m_nGateID);                                             // :271
        Assert.Equal(IntPtr.Zero, c.m_Module);                                    // :272
        Assert.Equal(1234u, c.m_dwCheckServerTimeMin);                            // :273
        Assert.Equal(0u, c.m_dwCheckServerTimeMax);                               // :274（不是 GetTickCount）
        Assert.Equal(1234u, c.m_dwCheckRecviceTick);                              // :275
        Assert.NotNull(c.SelectCharList);                                         // :276
        Assert.Equal(1000, c.SelectCharList.Count);
        Assert.Equal(0, c.SelectCharList.OnLineCount);
    }

    [Fact]
    public void Destroy_等价于对会话表做无参Finalize()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var u = c.SelectCharList.OnLineItems(0)!;
        u.nSessionID = 321;

        c.Destroy();

        Assert.Null(u.Socket);                                                    // 被清
        Assert.Equal("", u.sConnID);
        Assert.Equal(321, u.nSessionID);                                          // Finalize 不清会话号
        Assert.Equal(1, c.SelectCharList.OnLineCount);                            // 也不清上线表
    }

    // =====================================================================================
    // ExecGateBuffers（string 重载 —— 本配置下真正生效的收包入口）
    // =====================================================================================

    [Fact]
    public void ExecGateBuffers_没有美元符时不处理任何内容()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("abcdef");
        Assert.Equal("abcdef", c.m_sReceiveText);                                 // 原文 :460 `if Pos('$', ...) <= 0 then Break`
        Assert.Empty(Sent);
    }

    [Fact]
    public void ExecGateBuffers_有美元符但没有百分号_清空缓冲()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("abc$def");
        // ArrestStringEx 找不到 '%' ⇒ s10 = '' ⇒ 走 else：清缓冲并 Break（原文 :531-536）
        Assert.Equal("", c.m_sReceiveText);
        Assert.Empty(Sent);
    }

    [Fact]
    public void ExecGateBuffers_百分号心跳_回一个百分号加号加号美元()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%-$");
        Assert.Single(Sent);
        Assert.False(Sent[0].IsUserSocket);
        Assert.Equal("%++$", Sent[0].RawAnsi);                                    // 原文 :370 `SendText('%++$')`
    }

    [Fact]
    public void ExecGateBuffers_心跳分支不消耗未知命令计数()
    {
        // 连续两次 '-'（注意 '%++$' 本身在收包侧是**未知命令**，见下一条差异断言）
        var c = new TSelectClient();
        c.ExecGateBuffers("%-$%-$");
        Assert.Equal(2, Sent.Count);
        Assert.Equal("", c.m_sReceiveText);
    }

    [Fact]
    public void ExecGateBuffers_收到的百分号加号加号美元是未知命令()
    {
        // ★ 差异断言：本端**发出**的 keepalive 是 '%++$'，但收包侧首字符是 '+' ⇒ 落 case 默认分支
        //   （不是 '-'），既不回心跳也不报错，只是把未知命令计数 0→1。
        var c = new TSelectClient();
        c.ExecGateBuffers("%++$");
        Assert.Empty(Sent);
        Assert.Equal("", c.m_sReceiveText);
    }

    [Fact]
    public void ExecGateBuffers_未知命令两次之后停止解析后续帧()
    {
        var c = new TSelectClient();
        // 第 1 帧未知（计数 0→1），第 2 帧未知 ⇒ 清空并 Break，第 3 帧的心跳**不会**被处理
        c.ExecGateBuffers("%Z$%Z$%-$");
        Assert.Empty(Sent);
        Assert.Equal("", c.m_sReceiveText);
    }

    [Fact]
    public void ExecGateBuffers_未知命令只出现一次时后续合法帧仍被处理()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%Z$%-$");                                              // 未知(0→1) 后仍继续
        Assert.Single(Sent);
        Assert.Equal("%++$", Sent[0].RawAnsi);
    }

    [Fact]
    public void ExecGateBuffers_粘包_同一缓冲里的两条帧都被处理()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O7/1.2.3.4/5.6.7.8$%-$");
        Assert.Single(Sent);                                                      // 只有心跳发包
        Assert.Equal(1, c.SelectCharList.OnLineCount);
        Assert.Equal("7", c.SelectCharList.OnLineItems(0)!.sConnID);
    }

    [Fact]
    public void ExecGateBuffers_半包_残缺帧留在缓冲中()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O7/1.2.3");
        Assert.Equal("%O7/1.2.3", c.m_sReceiveText);                              // 没有 '$' ⇒ 原样保留
        Assert.Equal(0, c.SelectCharList.OnLineCount);
        c.ExecGateBuffers(".4/5.6.7.8$");
        Assert.Equal("", c.m_sReceiveText);
        Assert.Equal(1, c.SelectCharList.OnLineCount);
    }

    [Fact]
    public void ExecGateBuffers_字节重载与字符串重载等价()
    {
        var a = new TSelectClient();
        byte[] raw = Encoding.Latin1.GetBytes("%O7/1.2.3.4/5.6.7.8$%-$");
        a.ExecGateBuffers(raw, raw.Length);

        var b = new TSelectClient();
        b.ExecGateBuffers("%O7/1.2.3.4/5.6.7.8$%-$");

        Assert.Equal(b.m_sReceiveText, a.m_sReceiveText);
        Assert.Equal(b.SelectCharList.OnLineCount, a.SelectCharList.OnLineCount);
        Assert.Equal(b.SelectCharList.OnLineItems(0)!.sConnID, a.SelectCharList.OnLineItems(0)!.sConnID);
        Assert.Equal(2, Sent.Count);                                              // 各发一次心跳
        Assert.Equal("%++$", Sent[0].RawAnsi);
        Assert.Equal("%++$", Sent[1].RawAnsi);
    }

    [Fact]
    public void ExecGateBuffers_字节重载BufLen为0_等价于追加空串()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers(new byte[] { 1, 2, 3 }, 0);
        Assert.Equal("", c.m_sReceiveText);
        Assert.Empty(Sent);
    }

    // =====================================================================================
    // %O / %X / %S
    // =====================================================================================

    [Fact]
    public void 帧O_接入用户_解析两级IP并占槽()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O21/1.2.3.4/5.6.7.8$");

        Assert.Equal(1, c.SelectCharList.OnLineCount);
        var u = c.SelectCharList.OnLineItems(0)!;
        Assert.Equal(0, u.nIndex);                                                // 原文 :690 `UserInfo.nIndex := nIndex`
        Assert.Equal("21", u.sConnID);
        Assert.Equal("1.2.3.4", u.sUserIPaddr);                                   // GetValidStr3 的第一段
        Assert.Equal("5.6.7.8", u.sGateIPaddr);                                   // 余下部分
        Assert.Same(c, u.Socket);                                                 // 原文 {$ELSE} `UserInfo.Socket := Self`
        Assert.Equal(0, u.nSelGateID);                                            // m_nGateID 恒 0
    }

    [Fact]
    public void 帧O_只有一段IP时_GateIP为空()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/9.9.9.9$");
        var u = c.SelectCharList.OnLineItems(0)!;
        Assert.Equal("9.9.9.9", u.sUserIPaddr);
        Assert.Equal("", u.sGateIPaddr);
    }

    [Fact]
    public void 帧O_同ConnID重复接入_不重复占槽()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1/2.2.2.2$");
        c.ExecGateBuffers("%O1/3.3.3.3/4.4.4.4$");                                // 原文 :682 直接 Exit
        Assert.Equal(1, c.SelectCharList.OnLineCount);
        Assert.Equal("1.1.1.1", c.SelectCharList.OnLineItems(0)!.sUserIPaddr);    // 原槽未被改写
    }

    [Fact]
    public void 帧O_不同ConnID占不同槽()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$%O2/2.2.2.2$");
        Assert.Equal(2, c.SelectCharList.OnLineCount);
        Assert.Equal("1", c.SelectCharList.OnLineItems(0)!.sConnID);
        Assert.Equal("2", c.SelectCharList.OnLineItems(1)!.sConnID);
    }

    [Fact]
    public void 帧X_离开用户_回收槽并摘上线表()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        c.ExecGateBuffers("%X1$");

        Assert.Equal(0, c.SelectCharList.OnLineCount);
        // Finalize(Index) 只清 7 个字段 ⇒ sConnID 清空、Socket 置 nil、nIndex = -1
        var u = c.SelectCharList.Items(0)!;
        Assert.Null(u.Socket);
        Assert.Equal("", u.sConnID);
        Assert.Equal(-1, u.nIndex);
        // 回收表已登记 0 ⇒ 再接入直接复用 0
        c.ExecGateBuffers("%O2/2.2.2.2$");
        Assert.Equal(0, c.SelectCharList.OnLineItems(0)!.nIndex);
    }

    [Fact]
    public void 帧X_会话仍活跃时不动IDSoc()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        c.SelectCharList.OnLineItems(0)!.nSessionID = 55;
        IdSoc.SessionStatus = true;                                               // GetGlobaSessionStatus = True
        c.ExecGateBuffers("%X1$");
        Assert.Single(IdSoc.StatusCalls);
        Assert.Empty(IdSoc.SocketMsgs);
        Assert.Empty(IdSoc.ClosedSessions);
    }

    [Fact]
    public void 帧X_会话已失效时发SS_SOFTOUTSESSION并关闭会话()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var u = c.SelectCharList.OnLineItems(0)!;
        u.nSessionID = 55;
        u.sAccount = "acct";
        IdSoc.SessionStatus = false;

        c.ExecGateBuffers("%X1$");

        Assert.Single(IdSoc.SocketMsgs);
        Assert.Equal((ushort)CommonConst.SS_SOFTOUTSESSION, IdSoc.SocketMsgs[0].Ident);
        Assert.Equal("acct/55", IdSoc.SocketMsgs[0].Msg);                         // 原文 :716 sAccount + '/' + IntToStr(nSessionID)
        Assert.Single(IdSoc.ClosedSessions);
        Assert.Equal(("acct", 55), IdSoc.ClosedSessions[0]);
    }

    [Fact]
    public void 帧X_不存在的ConnID什么都不做()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%X99$");
        Assert.Empty(IdSoc.StatusCalls);                                          // 原文只在命中槽位时才碰 FrmIDSoc
        Assert.Equal(0, c.SelectCharList.OnLineCount);
    }

    [Fact]
    public void 帧S_127点0点0点前缀_直接采用并记日志()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%S127.0.0.9$");
        Assert.Equal("127.0.0.9", c.m_sGateaddr);
        Assert.Contains(Logs, s => s == "127.0.0.9 角色网关连接");
    }

    [Fact]
    public void 帧S_其它前缀_取RemoteAddress()
    {
        TSelectClient.RemoteAddressSink = _ => "192.168.1.77";
        var c = new TSelectClient();
        c.ExecGateBuffers("%S10.0.0.1$");
        Assert.Equal("192.168.1.77", c.m_sGateaddr);
        Assert.Empty(Logs);
    }

    [Theory]
    [InlineData("127.0.0.")]        // 恰好 8 字符 ⇒ CompareLStr 为真
    [InlineData("127.0.0.1")]
    public void 帧S_前缀比较需要至少8个字符(string ip)
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%S" + ip + "$");
        Assert.Equal(ip, c.m_sGateaddr);
    }

    [Fact]
    public void 帧S_短于8字符_不匹配前缀()
    {
        // HUtil32.CompareLStr 要求 src.Length >= compn ⇒ "127.0.0" (7) 为 false ⇒ 走 RemoteAddress
        TSelectClient.RemoteAddressSink = _ => "REMOTE";
        var c = new TSelectClient();
        c.ExecGateBuffers("%S127.0.0$");
        Assert.Equal("REMOTE", c.m_sGateaddr);
    }

    // =====================================================================================
    // %A 用户数据帧 —— ★ Continue/Break 的差异是本组核心
    // =====================================================================================

    private static void AddDuplicateConnSlots(TSelectClient c, string connID)
    {
        // 原文的 OpenUser 会拦住重复 ConnID，所以这里直接构造"两个同 ConnID 的在线槽"这一状态。
        int i0 = c.SelectCharList.Add();
        c.SelectCharList.Items(i0)!.Socket = c;                                   // Add 本身不写 Socket
        int i1 = c.SelectCharList.Add();
        c.SelectCharList.Items(i1)!.Socket = c;
        Assert.Equal((0, 1), (i0, i1));
        c.SelectCharList.Items(0)!.sConnID = connID;
        c.SelectCharList.Items(1)!.sConnID = connID;
    }

    [Fact]
    public void 帧A_载荷没有感叹号_会继续把载荷追加到后续同ConnID槽()
    {
        // ★★ 差异断言：原文 :484 的 `Continue` 绑定的是 **for**（不是"结束本条帧"），
        //    于是槽 0 与槽 1 都会拿到这段载荷；只有出现 '!' 才会 Break 掉 for。
        var c = new TSelectClient();
        AddDuplicateConnSlots(c, "5");
        c.ExecGateBuffers("%A5/abcdef$");
        Assert.Equal("abcdef", c.SelectCharList.Items(0)!.sReceiveText);
        Assert.Equal("abcdef", c.SelectCharList.Items(1)!.sReceiveText);
        Assert.Empty(Sent);
    }

    [Fact]
    public void 帧A_载荷带感叹号_命中第一个槽后立即停止()
    {
        var c = new TSelectClient();
        AddDuplicateConnSlots(c, "5");
        DBShareSeam.g_nCreateChrNameCount = 20;

        c.ExecGateBuffers(UserDataFrame("5", Cmd(Grobal2Const.CM_QUERYCHR), "acct/7"));

        // ★ 槽 0 处理并清掉了自己的 sReceiveText（ProcessUserMsg），槽 1 **完全没被触碰**
        Assert.Equal("", c.SelectCharList.Items(0)!.sReceiveText);
        Assert.Equal("", c.SelectCharList.Items(1)!.sReceiveText);
        Assert.Single(Sent);                                                      // 只有槽 0 发了 SM_QUERYCHR
    }

    [Fact]
    public void 帧A_ConnID不匹配时静默丢弃()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        c.ExecGateBuffers("%A2/#" + new string('x', 40) + "!$");
        Assert.Equal("", c.SelectCharList.OnLineItems(0)!.sReceiveText);
        Assert.Empty(Sent);
    }

    // =====================================================================================
    // ProcessUserMsg
    // =====================================================================================

    [Fact]
    public void ProcessUserMsg_载荷不足22字节时不派发()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        // '#' 后只有 5 字节 ⇒ 不满足 Length(s10) >= DEF_BLOCK_SIZE
        c.ExecGateBuffers("%A1/#abcde!$");
        Assert.Empty(Sent);
        Assert.Equal("", c.SelectCharList.OnLineItems(0)!.sReceiveText);
    }

    [Fact]
    public void ProcessUserMsg_同一槽里两条连续帧_第二条被第一条刷新的dwChrTick挡住()
    {
        // ★ 原文如此：dwChrTick 是**每槽一个**、被每一条进入节流分支的命令刷新（:854/:868），
        //   所以同一批收到两条节流命令时，**第二条必然落到 [Hacker Attack] 分支**。
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        c.SelectCharList.OnLineItems(0)!.dwChrTick = 0;
        DelphiTick.GetTickCount = () => 5000;                                     // 时钟不前进
        string p1 = EncodedPayload(Cmd(Grobal2Const.CM_QUERYDELCHR), "acct");
        string p2 = EncodedPayload(Cmd(Grobal2Const.CM_GETBACKDELCHR), "acct/name");
        c.ExecGateBuffers("%A1/#1" + p1 + "!#1" + p2 + "!$");

        Assert.Single(Sent);
        Assert.Equal(Grobal2Const.SM_QUERYDELCHR, Sent[0].Msg.Ident);
        Assert.Contains("[Hacker Attack] _GETBACKDELCHR 1.1.1.1", Logs);
    }

    [Fact]
    public void ProcessUserMsg_时钟前进时两条帧都被派发()
    {
        var c = new TSelectClient();
        uint tick = 0;
        DelphiTick.GetTickCount = () => { tick += 1000; return tick; };
        c.ExecGateBuffers("%O1/1.1.1.1$");
        c.SelectCharList.OnLineItems(0)!.dwChrTick = 0;

        string p1 = EncodedPayload(Cmd(Grobal2Const.CM_QUERYDELCHR), "acct");
        string p2 = EncodedPayload(Cmd(Grobal2Const.CM_GETBACKDELCHR), "acct/name");
        c.ExecGateBuffers("%A1/#1" + p1 + "!#1" + p2 + "!$");

        Assert.Equal(2, Sent.Count);
        Assert.Equal(Grobal2Const.SM_QUERYDELCHR, Sent[0].Msg.Ident);
        Assert.Equal(Grobal2Const.SM_GETBAKCHAR_SUCCESS, Sent[1].Msg.Ident);
    }

    [Fact]
    public void ProcessUserMsg_空段累计两次后清空该槽的接收缓冲()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        // 空段保护（原文 :660-664）：'#' 紧跟 '!' ⇒ ArrestStringEx 得空串 ⇒ nC 0→1 时不清；
        // 第二次 nC >= 1 ⇒ 清空该槽缓冲
        c.ExecGateBuffers("%A1/#!/!/$");
        Assert.Equal("", c.SelectCharList.OnLineItems(0)!.sReceiveText);
        Assert.Empty(Sent);
    }

    // =====================================================================================
    // DeCodeUserMsg 分派
    // =====================================================================================

    [Fact]
    public void 分派_未知Ident回SM_CHECKISMYSELFSERVER()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        c.ExecGateBuffers(UserDataFrame("1", Cmd(9999)));

        Assert.Single(Sent);
        Assert.Equal(Grobal2Const.SM_CHECKISMYSELFSERVER, Sent[0].Msg.Ident);
        Assert.Equal(8889, Sent[0].Msg.Ident);
    }

    [Theory]
    [InlineData(104)]        // CM_ 里被跳过的号
    [InlineData(0)]
    [InlineData(3007)]       // 紧邻 CM_GETBACKDELCHR(3006) 的下一个
    public void 分派_非分派表内的Ident都走else(int ident)
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        c.ExecGateBuffers(UserDataFrame("1", Cmd(ident)));
        Assert.Equal(Grobal2Const.SM_CHECKISMYSELFSERVER, Sent[0].Msg.Ident);
    }

    [Theory]
    [InlineData(Grobal2Const.CM_QUERYCHR, 100)]
    [InlineData(Grobal2Const.CM_NEWCHR, 101)]
    [InlineData(Grobal2Const.CM_DELCHR, 102)]
    [InlineData(Grobal2Const.CM_SELCHR, 103)]
    [InlineData(Grobal2Const.CM_QUERYDELCHR, 105)]
    [InlineData(Grobal2Const.CM_RANDOMNAME, 106)]
    [InlineData(Grobal2Const.CM_GETBACKDELCHR, 3006)]
    public void 分派表_常量值与Grobal2一致(int ident, int expected)
    {
        Assert.Equal(expected, ident);
    }

    // ---- CM_QUERYCHR：`not boChrQueryed or (tick - dwChrTick) > 200`

    [Fact]
    public void 查询角色_boChrQueryed为假时无条件处理()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.boChrQueryed = false;
        slot.dwChrTick = 1000;
        DelphiTick.GetTickCount = () => 1000;                                     // 差 0，但 not boChrQueryed 已足够

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_QUERYCHR), "acct/-2"));

        Assert.Equal(Grobal2Const.SM_QUERYCHR, Sent[0].Msg.Ident);
    }

    [Fact]
    public void 查询角色_已查过且未过200毫秒_记Hacker并回包()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.boChrQueryed = true;
        slot.dwChrTick = 1000;
        slot.sUserIPaddr = "8.8.8.8";
        DelphiTick.GetTickCount = () => 1200;                                     // 差恰好 200 ⇒ 不满足 >

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_QUERYCHR), "acct/-2"));

        Assert.Empty(Sent);
        Assert.Contains("[Hacker Attack] _QUERYCHR 8.8.8.8", Logs);
    }

    [Fact]
    public void 查询角色_已查过且刚好超过200毫秒_放行()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.boChrQueryed = true;
        slot.dwChrTick = 1000;
        DelphiTick.GetTickCount = () => 1201;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_QUERYCHR), "acct/-2"));

        Assert.Equal(Grobal2Const.SM_QUERYCHR, Sent[0].Msg.Ident);
        Assert.Equal(1201u, slot.dwChrTick);                                      // 原文 :741 先刷新 tick
    }

    [Fact]
    public void 查询角色_时间戳是LongWord回绕语义()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.boChrQueryed = true;
        slot.dwChrTick = uint.MaxValue - 99;                                      // 90 - (2^32-100) 回绕后 = 190? 见下
        DelphiTick.GetTickCount = () => 400;                                      // 400 + 100 = 500 > 200（mod 2^32）

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_QUERYCHR), "acct/-2"));

        Assert.Equal(Grobal2Const.SM_QUERYCHR, Sent[0].Msg.Ident);
    }

    // ---- CM_RANDOMNAME：原文**没有**节流

    [Fact]
    public void 随机名_即使tick没变化也照常处理_原文没有节流()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.sAccount = "acct";
        slot.dwChrTick = 5000;
        DelphiTick.GetTickCount = () => 5000;                                     // 差 0
        SelectClientGlobals.g_FirstName.Add("Abc");
        SelectClientGlobals.g_LastName.Add("Def");
        SelectClientRandom.NextDouble = () => 0.0;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_RANDOMNAME)));

        Assert.Equal(Grobal2Const.SM_RANDOMNAME, Sent[0].Msg.Ident);
        Assert.Equal("AbcDef", DecodeBody(Sent[0].Body));
        Assert.Equal(5000u, slot.dwChrTick);                                      // ★ 原文注释掉了刷新（:756-758）
    }

    [Fact]
    public void 随机名_账号为空时发OutOfConnect并记NEWCHR错误()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.sAccount = "";

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_RANDOMNAME)));

        Assert.Equal(Grobal2Const.SM_OUTOFCONNECTION, Sent[0].Msg.Ident);
        Assert.Contains(Logs, s => s.StartsWith("[ERROR] _NEWCHR ", StringComparison.Ordinal));
    }

    [Fact]
    public void 随机名_会话校验不通过时发OutOfConnect()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        c.SelectCharList.OnLineItems(0)!.sAccount = "acct";
        IdSoc.CheckSessionResult = false;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_RANDOMNAME)));

        Assert.Equal(Grobal2Const.SM_OUTOFCONNECTION, Sent[0].Msg.Ident);
        Assert.Single(IdSoc.CheckSessionCalls);
    }

    // ---- CM_NEWCHR / CM_DELCHR：`(tick - dwChrTick) > 1000`

    [Fact]
    public void 建号_节流阈值是1000毫秒_差恰好1000不放行()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.dwChrTick = 1000;
        slot.sAccount = "acct";
        slot.sUserIPaddr = "8.8.8.8";
        DelphiTick.GetTickCount = () => 2000;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_NEWCHR), "acct/nnnn/1/1/1"));

        Assert.Empty(Sent);
        Assert.Contains("[Hacker Attack] _NEWCHR acct/8.8.8.8", Logs);
    }

    [Fact]
    public void 建号_差1001毫秒放行且把boChrQueryed置回假()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.dwChrTick = 1000;
        slot.sAccount = "acct";
        slot.boChrQueryed = true;
        DelphiTick.GetTickCount = () => 2001;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_NEWCHR), "acct/nnnn/1/1/1"));

        Assert.Equal(Grobal2Const.SM_NEWCHR_SUCCESS, Sent[0].Msg.Ident);
        Assert.False(slot.boChrQueryed);                                          // 原文 :787
        Assert.Equal(2001u, slot.dwChrTick);
    }

    [Fact]
    public void 删号_同样用1000毫秒阈值()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.dwChrTick = 1000;
        slot.sAccount = "acct";
        slot.sUserIPaddr = "8.8.8.8";
        DelphiTick.GetTickCount = () => 2000;                                     // 恰好 1000 ⇒ 挡
        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_DELCHR), "nnnn"));
        Assert.Empty(Sent);
        Assert.Contains("[Hacker Attack] _DELCHR acct/8.8.8.8", Logs);
    }

    [Fact]
    public void 删号_会话校验失败时发OutOfConnect()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.dwChrTick = 0;
        slot.sAccount = "acct";
        DelphiTick.GetTickCount = () => 5000;
        IdSoc.CheckSessionResult = false;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_DELCHR), "nnnn"));

        Assert.Equal(Grobal2Const.SM_OUTOFCONNECTION, Sent[0].Msg.Ident);
        Assert.Contains(Logs, s => s.StartsWith("[ERROR] _DELCHR ", StringComparison.Ordinal));
    }

    // ---- CM_SELCHR：`if not boChrQueryed` + 恒 False 的 QueryChr ⇒ boChrQueryed 永远为假

    [Fact]
    public void 选号_boChrQueryed为真时记DoubleSend且不发包()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.boChrQueryed = true;
        slot.sAccount = "acct";

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_SELCHR), "acct/nnnn"));

        Assert.Empty(Sent);
        Assert.Contains(Logs, s => s.StartsWith("Double send _SELCHR ", StringComparison.Ordinal));
    }

    [Fact]
    public void 选号_会话校验失败时发OutOfConnect()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.sAccount = "acct";
        IdSoc.CheckSessionResult = false;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_SELCHR), "acct/nnnn"));

        Assert.Equal(Grobal2Const.SM_OUTOFCONNECTION, Sent[0].Msg.Ident);
        Assert.Contains(Logs, s => s.StartsWith("[ERROR] _SELCHR ", StringComparison.Ordinal));
    }

    [Fact]
    public void 选号_成功后置boChrSelected()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.sAccount = "acct";
        c.m_sGateaddr = "127.0.0.1";
        DBShareSeam.g_RouteInfo[0].sSelGateIP = "127.0.0.1";
        DBShareSeam.g_RouteInfo[0].nGateCount = 1;
        DBShareSeam.g_RouteInfo[0].sGameGateIP[0] = "10.0.0.1";
        DBShareSeam.g_RouteInfo[0].nGameGatePort[0] = 7200;
        DelphiRandom.Next = _ => 0;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_SELCHR), "acct/nnnn"));

        Assert.Equal(Grobal2Const.SM_STARTPLAY, Sent[0].Msg.Ident);
        Assert.True(slot.boChrSelected);
    }

    // ---- CM_QUERYDELCHR / CM_GETBACKDELCHR：200ms

    [Fact]
    public void 查已删角色_差恰好200毫秒被挡()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.dwChrTick = 100;
        slot.sUserIPaddr = "8.8.8.8";
        DelphiTick.GetTickCount = () => 300;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_QUERYDELCHR), "acct"));

        Assert.Empty(Sent);
        Assert.Contains("[Hacker Attack] _QUERYDELCHR 8.8.8.8", Logs);
    }

    [Fact]
    public void 查已删角色_超过200毫秒放行()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.dwChrTick = 100;
        DelphiTick.GetTickCount = () => 301;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_QUERYDELCHR), "acct"));

        Assert.Equal(Grobal2Const.SM_QUERYDELCHR, Sent[0].Msg.Ident);
    }

    [Fact]
    public void 找回已删角色_差恰好200毫秒被挡()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.dwChrTick = 100;
        slot.sUserIPaddr = "8.8.8.8";
        DelphiTick.GetTickCount = () => 300;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_GETBACKDELCHR), "acct/nnnn"));

        Assert.Empty(Sent);
        Assert.Contains("[Hacker Attack] _GETBACKDELCHR 8.8.8.8", Logs);
    }

    [Fact]
    public void 找回已删角色_超过200毫秒放行()
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O1/1.1.1.1$");
        c.SelectCharList.OnLineItems(0)!.dwChrTick = 100;
        DelphiTick.GetTickCount = () => 301;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_GETBACKDELCHR), "acct/nnnn"));

        Assert.Equal(Grobal2Const.SM_GETBAKCHAR_SUCCESS, Sent[0].Msg.Ident);
        Assert.Single(HumanDb.DeleteRestoreCalls);
        Assert.Equal(("acct", "nnnn"), HumanDb.DeleteRestoreCalls[0]);
    }

    // ---- 出站帧格式

    [Fact]
    public void OutOfConnect_帧格式为百分号ConnID斜杠井号载荷感叹号美元()
    {
        var c = new TSelectClient();
        c.OutOfConnect("42");
        Assert.True(Sent[0].IsUserSocket);
        Assert.Equal("42", Sent[0].SessionID);
        Assert.Equal(Grobal2Const.SM_OUTOFCONNECTION, Sent[0].Msg.Ident);
        Assert.Equal("%42/#" + Encoding.Latin1.GetString(EDcode.EncodeMessage(Cmd(Grobal2Const.SM_OUTOFCONNECTION))) + "!$",
                     Sent[0].RawAnsi);
    }

    [Fact]
    public void SendUserSocket_载荷42号精确等于Message加EncodeString()
    {
        var c = new TSelectClient();
        byte[] body = EDcode.EncodeString("hello");
        byte[] expected = new byte[EDcode.EncodeMessage(Cmd(Grobal2Const.SM_QUERYCHR)).Length + body.Length];
        Array.Copy(EDcode.EncodeMessage(Cmd(Grobal2Const.SM_QUERYCHR)), expected, EDcode.EncodeMessage(Cmd(Grobal2Const.SM_QUERYCHR)).Length);
        Array.Copy(body, 0, expected, EDcode.EncodeMessage(Cmd(Grobal2Const.SM_QUERYCHR)).Length, body.Length);

        c.SendUserSocket("7", expected);

        Assert.Equal("7", Sent[0].SessionID);
        Assert.Equal(expected, Sent[0].Payload);
        Assert.Equal("hello", DecodeBody(Sent[0].Body));
    }

    [Fact]
    public void SendKickUser_0和1和2分别对应三种前缀()
    {
        var c = new TSelectClient();
        c.SendKickUser("H1", 0);
        c.SendKickUser("H2", 1);
        c.SendKickUser("H3", 2);
        Assert.Equal("%+-H1$", Sent[0].RawAnsi);
        Assert.Equal("%+TH2$", Sent[1].RawAnsi);
        Assert.Equal("%+BH3$", Sent[2].RawAnsi);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    [InlineData(99)]
    public void SendKickUser_非012的取值什么都不发(int kickType)
    {
        var c = new TSelectClient();
        c.SendKickUser("H", kickType);
        Assert.Empty(Sent);                                                       // 原文 case 没有 else
    }

    [Fact]
    public void SendKeepAlivePacket_发百分号加号加号美元并刷新三个tick()
    {
        var c = new TSelectClient();
        c.m_dwCheckRecviceTick = 1000;
        c.m_dwCheckServerTimeMax = 5;
        DelphiTick.GetTickCount = () => 1300;

        c.SendKeepAlivePacket();

        Assert.Equal("%++$", Sent[0].RawAnsi);
        Assert.Equal(1300u, c.m_dwKeepAliveTick);
        Assert.Equal(300u, c.m_dwCheckServerTimeMin);                             // 1300 - 1000
        Assert.Equal(300u, c.m_dwCheckServerTimeMax);                             // 超过原值 5 ⇒ 抬高
        Assert.Equal(1300u, c.m_dwCheckRecviceTick);
    }

    [Fact]
    public void SendKeepAlivePacket_最大值不被压低()
    {
        var c = new TSelectClient();
        c.m_dwCheckRecviceTick = 0;
        c.m_dwCheckServerTimeMax = 9999;
        DelphiTick.GetTickCount = () => 10;
        c.SendKeepAlivePacket();
        Assert.Equal(9999u, c.m_dwCheckServerTimeMax);
        Assert.Equal(10u, c.m_dwCheckServerTimeMin);
    }

    [Fact]
    public void SendKeepAlivePacket_m_Module非空时走接缝()
    {
        var c = new TSelectClient();
        c.m_Module = new IntPtr(1);
        c.m_dwCheckRecviceTick = 0;
        var seen = new List<(IntPtr Module, string Buffer)>();
        SelectClientModuleSeam.UpdateModuleBuffer = (m, b) => seen.Add((m, b));
        DelphiTick.GetTickCount = () => 7;

        c.SendKeepAlivePacket();

        Assert.Single(seen);
        Assert.Equal("7/7", seen[0].Buffer);                                      // DelphiFormat.Format('%d/%d', [min, max])
        Assert.Equal(new IntPtr(1), seen[0].Module);
    }

    [Fact]
    public void 未接线的接缝默认抛异常而不是静默()
    {
        var c = new TSelectClient();
        TSelectClient.ResetSeams();
        Assert.Throws<NotSupportedException>(() => c.OutOfConnect("1"));
    }

    [Fact]
    public void 未接线RemoteAddress时_S分支非127前缀抛异常()
    {
        var c = new TSelectClient();
        TSelectClient.ResetSeams();
        Assert.Throws<NotSupportedException>(() => c.ExecGateBuffers("%S10.0.0.1$"));
    }
}

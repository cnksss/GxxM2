using System;
using System.Collections.Generic;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// `Source\DBServer\IDSocCli.pas`（**464 行**）1:1 移植的测试。
/// 纯逻辑部分（会话表增删查 + 9 个会话查询/变更 + 解包 `ProcessSocketMsg` + 组包 `SendSocketMsg`）全覆盖；
/// 宿主面（socket / 定时器 / 模块表 / 在线数）用替身，**不建真 socket、不起真定时器**。
/// </summary>
public class IDSocCliTests : IDSocCliTestBase
{
    // =====================================================================================
    // 构造 / 生命周期
    // =====================================================================================

    [Fact]
    public void 构造_字段初值照抄原文()
    {
        DelphiTick.GetTickCount = () => 1234;
        using var frm = NewFrm();

        Assert.Equal("FrmIDSoc", frm.Text);                                      // DFM Caption='FrmIDSoc'
        Assert.Equal(IntPtr.Zero, frm.m_Module);                                 // :70
        Assert.Equal(1234u, frm.m_dwCheckServerTimeMin);                         // :71
        Assert.Equal(0u, frm.m_dwCheckServerTimeMax);                            // :72（不是 GetTickCount）
        Assert.Equal(1234u, frm.m_dwCheckRecviceTick);                           // :73
        Assert.Equal(0, frm.GlobaSessionCount);                                  // :68
    }

    [Fact]
    public void 构造_不触碰宿主设施接缝()
    {
        // 偏差 D-p7-15：`:66/:67` 两行 `TimerX.Enabled := False` 在 **FormCreate()** 里，不在构造函数里。
        // 否则"宿主还没装定时器"会变成"连窗体都建不出来"。
        Assert.Empty(Timer1);
        Assert.Empty(KeepAliveTimer);
        using var frm = NewFrm();
        Assert.Empty(Timer1);
        Assert.Empty(KeepAliveTimer);
    }

    [Fact]
    public void FormCreate_把两个定时器都关掉()
    {
        using var frm = NewFrm();
        frm.FormCreate();
        Assert.Equal(new[] { false }, Timer1);
        Assert.Equal(new[] { false }, KeepAliveTimer);
    }

    [Fact]
    public void FormCreate_宿主设施未接线时抛而不是静默()
    {
        IDSocCliSeam.Timer1Enabled = _ => throw new NotSupportedException("接缝：Timer1 未接线");
        using var frm = NewFrm();
        Assert.Throws<NotSupportedException>(() => frm.FormCreate());
    }

    [Fact]
    public void FormDestroy_清空会话表()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 42);
        Assert.Equal(1, frm.GlobaSessionCount);
        frm.FormDestroy();
        Assert.Equal(0, frm.GlobaSessionCount);
    }

    // =====================================================================================
    // 解包：ProcessSocketMsg / IDSocketRead
    // =====================================================================================

    [Fact]
    public void ProcessSocketMsg_无右括号时不动缓冲()
    {
        using var frm = NewFrm();
        Read(frm, "(1000/acct/42/0/x/1.1.1.1");                  // 缺 ')'
        Assert.Equal(0, frm.GlobaSessionCount);
    }

    [Fact]
    public void ProcessSocketMsg_SS_OPENSESSION_真值1000建会话()
    {
        using var frm = NewFrm();
        Read(frm, "(" + CommonConst.SS_OPENSESSION + "/acct/42/0/x/1.1.1.1)");

        Assert.Equal(1, frm.GlobaSessionCount);
        var s = frm.GlobaSessionAt(0)!;
        Assert.Equal("acct", s.sAccount);
        Assert.Equal(42, s.nSessionID);
        Assert.Equal("1.1.1.1", s.sIPaddr);
        Assert.False(s.boStartPlay);
        Assert.False(s.boLoadRcd);
        Assert.False(s.boHeroLoadRcd);
    }

    [Fact]
    public void ProcessSocketMsg_原文注释值100不是真值_不会被识别()
    {
        // ★★ 原文 :126 写作 `SS_OPENSESSION {100}`，**真值是 1000**（Common.pas:23）。
        //    照注释值写会把三条推送全部漏掉 —— 本用例把这个坑钉死。
        Assert.Equal(1000, CommonConst.SS_OPENSESSION);
        Assert.Equal(1010, CommonConst.SS_CLOSESESSION);
        Assert.Equal(1040, CommonConst.SS_KEEPALIVE);
        Assert.Equal(1030, CommonConst.SS_SERVERINFO);

        using var frm = NewFrm();
        Read(frm, "(100/acct/42/0/x/1.1.1.1)");                  // 用陈旧注释值
        Assert.Equal(0, frm.GlobaSessionCount);                                  // 不识别
    }

    [Fact]
    public void ProcessSocketMsg_SS_CLOSESESSION_真值1010删会话()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 42);
        Read(frm, "(" + CommonConst.SS_CLOSESESSION + "/acct/42)");
        Assert.Equal(0, frm.GlobaSessionCount);
    }

    [Fact]
    public void ProcessSocketMsg_SS_KEEPALIVE_真值1040刷新统计()
    {
        using var frm = NewFrm();
        frm.m_dwCheckRecviceTick = 1000;
        frm.m_Module = new IntPtr(5);
        DelphiTick.GetTickCount = () => 1300;

        Read(frm, "(" + CommonConst.SS_KEEPALIVE + "/0)");

        Assert.Equal(300u, frm.m_dwCheckServerTimeMin);
        Assert.Equal(300u, frm.m_dwCheckServerTimeMax);
        Assert.Equal(1300u, frm.m_dwCheckRecviceTick);
        Assert.Equal(new[] { (new IntPtr(5), "300/300") }, ModuleBuffers);
    }

    [Fact]
    public void ProcessSocketMsg_粘包两帧都处理()
    {
        using var frm = NewFrm();
        Read(frm, "(1000/a1/1/0/x/1.1.1.1)(1000/a2/2/0/x/1.1.1.2)");
        Assert.Equal(2, frm.GlobaSessionCount);
    }

    [Fact]
    public void ProcessSocketMsg_半包留在缓冲里_补齐后完成()
    {
        using var frm = NewFrm();
        Read(frm, "(1000/acct/42/0/x/1.1");
        Assert.Equal(0, frm.GlobaSessionCount);
        Read(frm, ".1.1)");
        Assert.Equal(1, frm.GlobaSessionCount);
    }

    [Fact]
    public void ProcessSocketMsg_非法ident被忽略但缓冲被消费()
    {
        using var frm = NewFrm();
        Read(frm, "(9999/x/y)");
        Assert.Equal(0, frm.GlobaSessionCount);
        Read(frm, "(1000/acct/42/0/x/1.1.1.1)");                  // 前一帧已消费干净
        Assert.Equal(1, frm.GlobaSessionCount);
    }

    [Fact]
    public void ProcessSocketMsg_空帧Break不再继续()
    {
        using var frm = NewFrm();
        // `()` ⇒ ArrestStringEx 的 sData 为空 ⇒ :122 `if sData = '' then Break`
        Read(frm, "()(1000/acct/42/0/x/1.1.1.1)");
        Assert.Equal(0, frm.GlobaSessionCount);                                  // 后面的帧**没有**被处理
    }

    // =====================================================================================
    // ProcessAddSession / ProcessDelSession
    // =====================================================================================

    [Fact]
    public void ProcessAddSession_段数不足时缺项按0与空串()
    {
        using var frm = NewFrm();
        frm.ProcessAddSession("acct");
        var s = frm.GlobaSessionAt(0)!;
        Assert.Equal("acct", s.sAccount);
        Assert.Equal(0, s.nSessionID);
        Assert.Equal(0, s.n24);
        Assert.Equal("", s.sIPaddr);
    }

    [Fact]
    public void ProcessAddSession_dwAddTick与dAddDate被写入()
    {
        DelphiTick.GetTickCount = () => 777;
        using var frm = NewFrm();
        frm.ProcessAddSession("acct/42/5/x/1.1.1.1");
        var s = frm.GlobaSessionAt(0)!;
        Assert.Equal(5, s.n24);
        Assert.Equal(777u, s.dwAddTick);
        Assert.True(s.dAddDate > 0);
    }

    [Fact]
    public void ProcessAddSession_bo28是零初始化的_原文是未初始化记录()
    {
        // 偏差 D-p7-14：原文 `New(GlobaSessionInfo)` 后**没有**给 `bo28` 赋值（其余 9 个字段都赋了）
        // ⇒ Delphi 的 New 给未初始化内存 ⇒ `bo28` 是垃圾值。
        // 托管侧 `new TGlobaSessionInfo()` 零初始化 ⇒ `bo28 = false`（确定性，只能更好）。
        using var frm = NewFrm();
        frm.ProcessAddSession("acct/42/0/x/1.1.1.1");
        Assert.False(frm.GlobaSessionAt(0)!.bo28);
    }

    [Fact]
    public void ProcessDelSession_按账号加会话号命中才删()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 42);
        PushSession(frm, "other", 42);

        frm.ProcessDelSession("acct/42");
        Assert.Equal(1, frm.GlobaSessionCount);
        Assert.Equal("other", frm.GlobaSessionAt(0)!.sAccount);

        frm.ProcessDelSession("acct/42");                                        // 已经没了
        Assert.Equal(1, frm.GlobaSessionCount);
    }

    [Fact]
    public void ProcessDelSession_只删一条()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 1);
        PushSession(frm, "acct", 1);
        frm.ProcessDelSession("acct/1");
        Assert.Equal(1, frm.GlobaSessionCount);                                  // 原文命中即 Break
    }

    [Fact]
    public void ProcessDelSession_会话号解析失败按0()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 0);
        frm.ProcessDelSession("acct/notanumber");                                // StrToIntDef(..., 0)
        Assert.Equal(0, frm.GlobaSessionCount);
    }

    // =====================================================================================
    // 9 个会话查询 / 变更
    // =====================================================================================

    [Fact]
    public void CheckSession_命中账号与会话号()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 42);
        Assert.True(frm.CheckSession("acct", "1.1.1.1", 42));
    }

    [Theory]
    [InlineData("other", 42)]                                                    // 账号不符
    [InlineData("acct", 43)]                                                     // 会话号不符
    [InlineData("other", 43)]
    public void CheckSession_任一项不符即False(string account, int sessionId)
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 42);
        Assert.False(frm.CheckSession(account, "1.1.1.1", sessionId));
    }

    [Fact]
    public void CheckSession_账号比较不区分大小写()
    {
        using var frm = NewFrm();
        PushSession(frm, "AcCt", 42);
        Assert.True(frm.CheckSession("acct", "", 42));                           // SameText
    }

    [Fact]
    public void CheckSession_不使用sIPaddr形参()
    {
        // 原文如此（:145-164）：函数体只比对 sAccount 与 nSessionID，**从不读 sIPaddr**。
        using var frm = NewFrm();
        PushSession(frm, "acct", 42, ip: "9.9.9.9");
        Assert.True(frm.CheckSession("acct", "完全不同的IP", 42));
    }

    [Fact]
    public void CheckSession_空表返回False()
    {
        using var frm = NewFrm();
        Assert.False(frm.CheckSession("acct", "", 42));
    }

    [Fact]
    public void CheckSessionLoadRcd_首次为真并置闩锁_第二次为假()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 42);

        bool found = false;
        Assert.True(frm.CheckSessionLoadRcd("acct", "1.1.1.1", 42, ref found));
        Assert.True(found);
        Assert.True(frm.GlobaSessionAt(0)!.boLoadRcd);

        Assert.False(frm.CheckSessionLoadRcd("acct", "1.1.1.1", 42, ref found));  // 闩锁已置
        Assert.True(found);                                                      // 但"找到"仍为真
    }

    [Fact]
    public void CheckSessionLoadRcd_未命中时两个返回值都是假()
    {
        using var frm = NewFrm();
        bool found = true;
        Assert.False(frm.CheckSessionLoadRcd("acct", "", 42, ref found));
        Assert.False(found);
    }

    [Fact]
    public void CheckSessionHeroLoadRcd_闩锁与CheckSessionLoadRcd相互独立()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 42);

        bool found = false;
        Assert.True(frm.CheckSessionLoadRcd("acct", "", 42, ref found));          // 置 boLoadRcd
        Assert.True(frm.CheckSessionHeroLoadRcd("acct", "", 42, ref found));      // boHeroLoadRcd 仍是初值 ⇒ 仍返回真
        Assert.True(frm.GlobaSessionAt(0)!.boHeroLoadRcd);
    }

    [Fact]
    public void SetSessionSaveRcd_清该账号全部会话的boLoadRcd_且不Break()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 1);
        PushSession(frm, "acct", 2);
        PushSession(frm, "other", 3);
        bool f = false;
        frm.CheckSessionLoadRcd("acct", "", 1, ref f);
        frm.CheckSessionLoadRcd("acct", "", 2, ref f);
        frm.CheckSessionLoadRcd("other", "", 3, ref f);

        Assert.True(frm.SetSessionSaveRcd("acct"));

        Assert.False(frm.GlobaSessionAt(0)!.boLoadRcd);                          // ★ 两条都被清（原文无 Break）
        Assert.False(frm.GlobaSessionAt(1)!.boLoadRcd);
        Assert.True(frm.GlobaSessionAt(2)!.boLoadRcd);                           // 别的账号不受影响
    }

    [Fact]
    public void SetSessionSaveRcd_无命中返回False()
    {
        using var frm = NewFrm();
        Assert.False(frm.SetSessionSaveRcd("nobody"));
    }

    [Fact]
    public void SetGlobaSessionPlay与NoPlay_按会话号切换boStartPlay()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 42);

        Assert.False(frm.GetGlobaSessionStatus(42));
        frm.SetGlobaSessionPlay(42);
        Assert.True(frm.GetGlobaSessionStatus(42));
        Assert.True(frm.GlobaSessionAt(0)!.boStartPlay);

        frm.SetGlobaSessionNoPlay(42);
        Assert.False(frm.GetGlobaSessionStatus(42));
    }

    [Fact]
    public void SetGlobaSessionPlay_不存在的会话号不抛也无副作用()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 42);
        frm.SetGlobaSessionPlay(99);
        Assert.False(frm.GlobaSessionAt(0)!.boStartPlay);
        Assert.False(frm.GetGlobaSessionStatus(99));                             // 找不到 ⇒ False（与"存在但未开始"同值）
    }

    [Fact]
    public void GetGlobaSessionStatus_只认第一条匹配()
    {
        using var frm = NewFrm();
        PushSession(frm, "a", 42);
        PushSession(frm, "b", 42);
        frm.SetGlobaSessionPlay(42);                                             // 只置第一条并 Break

        Assert.True(frm.GlobaSessionAt(0)!.boStartPlay);
        Assert.False(frm.GlobaSessionAt(1)!.boStartPlay);
    }

    [Fact]
    public void CloseSession_账号与会话号都要对上()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 42);

        frm.CloseSession("wrong", 42);                                           // 账号不符 ⇒ 不删
        Assert.Equal(1, frm.GlobaSessionCount);

        frm.CloseSession("acct", 43);                                            // 会话号不符 ⇒ 不删
        Assert.Equal(1, frm.GlobaSessionCount);

        frm.CloseSession("ACCT", 42);                                            // SameText ⇒ 删
        Assert.Equal(0, frm.GlobaSessionCount);
    }

    [Fact]
    public void CloseSession_只删第一条匹配()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 42);
        PushSession(frm, "acct", 42);
        frm.CloseSession("acct", 42);
        Assert.Equal(1, frm.GlobaSessionCount);
    }

    [Fact]
    public void GetSession_按账号与IP精确匹配()
    {
        using var frm = NewFrm();
        PushSession(frm, "acct", 42, ip: "1.1.1.1");

        Assert.True(frm.GetSession("acct", "1.1.1.1"));
        Assert.True(frm.GetSession("ACCT", "1.1.1.1"));                          // 账号 SameText
        Assert.False(frm.GetSession("acct", "2.2.2.2"));                         // IP 是**精确**比较
        Assert.False(frm.GetSession("acct", ""));
    }

    // =====================================================================================
    // 组包 / 发包
    // =====================================================================================

    [Fact]
    public void SendSocketMsg_格式为括号ident斜杠消息括号()
    {
        using var frm = NewFrm();
        frm.SendSocketMsg(CommonConst.SS_SOFTOUTSESSION, "acct/42");
        Assert.Equal(new[] { "(1020/acct/42)" }, Sock.EndPoint.Sent);
    }

    [Fact]
    public void SendSocketMsg_未连接时静默不发送()
    {
        // 原文如此（:141 `if IDSocket.Socket.Connected then`）—— 这是原文的真实分支，不是接缝的中性值。
        using var frm = NewFrm();
        Sock.EndPoint.Connected = false;
        frm.SendSocketMsg(CommonConst.SS_SERVERINFO, "x");
        Assert.Empty(Sock.EndPoint.Sent);
    }

    [Fact]
    public void SendSocketMsg_socket未接线时抛而不是静默()
    {
        IDSocCliSeam.IDSocket = null;
        using var frm = NewFrm();
        Assert.Throws<NotSupportedException>(() => frm.SendSocketMsg(CommonConst.SS_SERVERINFO, "x"));
    }

    private static readonly string[] KeepAliveExpected = { "(1030/GeeM2/99/7)" };

    [Fact]
    public void SendKeepAlivePacket_内容与格式()
    {
        using var frm = NewFrm();
        DelphiTick.GetTickCount = () => 4000;                                    // 过 3000ms 节流（构造时 tick=0）
        frm.SendKeepAlivePacket();
        Assert.Equal(KeepAliveExpected, Sock.EndPoint.Sent);                     // 原文 :385 注释写 (103/…) 是陈旧值
    }

    [Fact]
    public void SendKeepAlivePacket_3000毫秒节流_差值恰为3000不发()
    {
        using var frm = NewFrm();
        frm.SendKeepAlivePacket();                                               // tick 0 ⇒ 0-0 = 0，不满足 >3000？见下
        Sock.EndPoint.Sent.Clear();

        DelphiTick.GetTickCount = () => 3000;
        frm.SendKeepAlivePacket();                                               // 3000 - 0 = 3000，**不 > 3000** ⇒ 不发
        Assert.Empty(Sock.EndPoint.Sent);

        DelphiTick.GetTickCount = () => 3001;
        frm.SendKeepAlivePacket();                                               // 3001 - 0 = 3001 > 3000 ⇒ 发
        Assert.Single(Sock.EndPoint.Sent);
    }

    [Fact]
    public void SendKeepAlivePacket_未连接时不发但tick照样刷新()
    {
        // 原文 :379 的 `m_dwKeepAlivePacketTick := GetTickCount` 在 :380 的 Connected 判定**之前**。
        using var frm = NewFrm();
        Sock.EndPoint.Connected = false;
        DelphiTick.GetTickCount = () => 5000;

        frm.SendKeepAlivePacket();
        Assert.Empty(Sock.EndPoint.Sent);

        DelphiTick.GetTickCount = () => 5001;
        Sock.EndPoint.Connected = true;
        frm.SendKeepAlivePacket();                                               // 节流窗口已被上一次刷新 ⇒ 仍不发
        Assert.Empty(Sock.EndPoint.Sent);
    }

    [Fact]
    public void SendKeepAlivePacket_在线数取自宿主接缝()
    {
        SelectCharCount = 12;
        IDSocCliSeam.g_sServerName = "MySrv";
        using var frm = NewFrm();
        DelphiTick.GetTickCount = () => 4000;                                    // 过 3000ms 节流
        frm.SendKeepAlivePacket();
        Assert.Equal(new[] { "(1030/MySrv/99/12)" }, Sock.EndPoint.Sent);
    }

    [Fact]
    public void KeepAliveTimerTimer_就是SendKeepAlivePacket()
    {
        using var frm = NewFrm();
        DelphiTick.GetTickCount = () => 4000;                                    // 过 3000ms 节流
        frm.KeepAliveTimerTimer();
        Assert.Equal(KeepAliveExpected, Sock.EndPoint.Sent);
    }

    // =====================================================================================
    // 连接生命周期
    // =====================================================================================

    [Fact]
    public void OpenConnect_先设地址端口再激活_并开Timer1()
    {
        IDSocCliSeam.g_sIDServerAddr = "10.2.2.2";
        IDSocCliSeam.g_nIDServerPort = 5700;
        using var frm = NewFrm();

        frm.OpenConnect();

        Assert.Equal(new[] { true }, Timer1);                                    // :417
        Assert.Equal(new[]
        {
            "Active=False",                                                      // :418
            "Address=10.2.2.2",                                                  // :419
            "Port=5700",                                                         // :420
            "Active=True",                                                       // :421
        }, Sock.Ops);
    }

    [Fact]
    public void CloseConnect_关Timer1并失活并清模块()
    {
        using var frm = NewFrm();
        frm.m_Module = new IntPtr(9);
        Sock.ClearOps();

        frm.CloseConnect();

        Assert.Equal(new[] { false }, Timer1);                                   // :390
        Assert.Equal(new[] { "Active=False" }, Sock.Ops);                        // :391
        Assert.Equal(IntPtr.Zero, frm.m_Module);                                 // :392
    }

    [Fact]
    public void Timer1Timer_未激活时按配置激活()
    {
        IDSocCliSeam.g_sIDServerAddr = "10.3.3.3";
        IDSocCliSeam.g_nIDServerPort = 5800;
        using var frm = NewFrm();
        Sock.Active = false;
        Sock.ClearOps();

        frm.Timer1Timer();

        Assert.Equal(new[] { "Address=10.3.3.3", "Port=5800", "Active=True" }, Sock.Ops);
    }

    [Fact]
    public void Timer1Timer_已激活时什么都不做()
    {
        using var frm = NewFrm();
        Sock.Active = true;
        Sock.ClearOps();

        frm.Timer1Timer();

        Assert.Empty(Sock.Ops);
    }

    [Fact]
    public void IDSocketError_错误码归零并关socket()
    {
        using var frm = NewFrm();
        frm.IDSocketError(out int code);
        Assert.Equal(0, code);                                                   // :324
        Assert.Equal(1, Sock.EndPoint.CloseCount);                               // :325
    }

    [Fact]
    public void IDSocketConnect_清零统计_开保活_登记模块()
    {
        DelphiTick.GetTickCount = () => 900;
        using var frm = NewFrm();
        frm.m_dwCheckServerTimeMax = 12345;                                      // 应被 :445 清零

        frm.IDSocketConnect();

        Assert.Equal(900u, frm.m_dwCheckServerTimeMin);
        Assert.Equal(0u, frm.m_dwCheckServerTimeMax);
        Assert.Equal(900u, frm.m_dwCheckRecviceTick);
        Assert.Equal(new[] { true }, KeepAliveTimer);                            // :448

        Assert.Single(ModuleAdded);
        Assert.Same(frm, ModuleAdded[0].Module);                                 // :449 ModuleInfo.Module := Self
        Assert.Equal("GeeM2", ModuleAdded[0].Name);                              // :450 g_sServerName
        Assert.Equal("10.1.1.1:5601 → 10.1.1.1:5600", ModuleAdded[0].Address);   // :451 格式串
        Assert.Equal("0/0", ModuleAdded[0].Buffer);                              // :452
        Assert.Equal(new IntPtr(1), frm.m_Module);
    }

    [Fact]
    public void IDSocketDisconnect_清模块_关保活_按Self移除()
    {
        using var frm = NewFrm();
        frm.IDSocketConnect();
        ModuleRemoved.Clear();

        frm.IDSocketDisconnect();

        Assert.Equal(IntPtr.Zero, frm.m_Module);                                 // :459
        Assert.Equal(new[] { true, false }, KeepAliveTimer);                     // :460（true 来自 Connect）
        Assert.Single(ModuleRemoved);
        Assert.Same(frm, ModuleRemoved[0]);                                      // :461 RemoveModule(Self) —— 不是句柄
    }

    [Fact]
    public void IDSocketRead_从socket取文本()
    {
        using var frm = NewFrm();
        Sock.EndPoint.ReceiveText = "(1000/acct/42/0/x/1.1.1.1)";
        frm.IDSocketRead();
        Assert.Equal(1, frm.GlobaSessionCount);
    }

    [Fact]
    public void ProcessGetOnlineCount_模块为空时不更新缓冲()
    {
        using var frm = NewFrm();
        frm.m_dwCheckRecviceTick = 0;
        DelphiTick.GetTickCount = () => 10;
        frm.ProcessGetOnlineCount("");
        Assert.Empty(ModuleBuffers);                                             // :434 `if m_Module <> nil`
        Assert.Equal(10u, frm.m_dwCheckServerTimeMin);
    }

    [Fact]
    public void ProcessGetOnlineCount_模块最大值不被压低()
    {
        using var frm = NewFrm();
        frm.m_Module = new IntPtr(3);
        frm.m_dwCheckRecviceTick = 100;
        frm.m_dwCheckServerTimeMax = 9999;
        DelphiTick.GetTickCount = () => 150;

        frm.ProcessGetOnlineCount("");

        Assert.Equal(50u, frm.m_dwCheckServerTimeMin);
        Assert.Equal(9999u, frm.m_dwCheckServerTimeMax);                         // 不压低
        Assert.Equal(new[] { (new IntPtr(3), "50/9999") }, ModuleBuffers);
    }

    [Fact]
    public void 自适应_实现ITFrmIDSoc可直接接到接缝()
    {
        using var frm = NewFrm();
        IDSocCliSeam.FrmIDSoc = frm;
        Assert.True(IDSocCliSeam.Require is TFrmIDSoc);
        PushSession(frm, "acct", 42);
        Assert.True(IDSocCliSeam.Require.CheckSession("acct", "", 42));
    }
}

using System;
using GXX.Core.Rtl;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// SelectClient.pas:10-258 —— <c>TUserInfo</c>（1000 槽会话表）与 <c>TSelectChar</c>。
/// 这是本单元唯一的纯状态逻辑，逐方法覆盖（含"看起来一样实则不同"的差异断言）。
/// </summary>
public class SelectClientCharTableTests : SelectClientTestBase
{
    // ------------------------------------------------------------------ GetItem / GetCount

    [Fact]
    public void Count_恒为1000_与在线数无关()
    {
        var t = new TSelectChar();
        Assert.Equal(1000, t.Count);
        Assert.Equal(0, t.OnLineCount);
        t.Add();
        Assert.Equal(1000, t.Count);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1000)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void Items_越界返回null_不抛异常(int index)
    {
        var t = new TSelectChar();
        Assert.Null(t.Items(index));                                              // 原文 :121 `Result := nil`
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public void Items_界内返回同一实例(int index)
    {
        var t = new TSelectChar();
        Assert.Same(t.Items(index), t.Items(index));                              // 指针语义：同一槽
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1000)]
    public void OnLineItems_越界返回null(int index)
    {
        var t = new TSelectChar();
        Assert.Null(t.OnLineItems(index));                                        // 原文 :134
    }

    // ------------------------------------------------------------------ Add

    [Fact]
    public void Add_初始1000槽Socket全为nil_且首次返回0()
    {
        var t = new TSelectChar();
        for (int i = 0; i < t.Count; i++) Assert.Null(t.Items(i)!.Socket);
        Assert.Equal(0, t.Add());
        Assert.Equal(1, t.OnLineCount);
    }

    [Fact]
    public void Add_不写Socket_故连续调用恒返回0并重复入表()
    {
        // ★ 原文如此（:142-168）：Add 只把下标放进 OnLineList，**不**改 Socket、**不**查重。
        //   初始化槽位是调用方 OpenUser(:688) 的职责。
        var t = new TSelectChar();
        Assert.Equal(0, t.Add());
        Assert.Equal(0, t.Add());
        Assert.Equal(0, t.Add());
        Assert.Equal(3, t.OnLineCount);
        Assert.Same(t.Items(0), t.OnLineItems(0));
        Assert.Same(t.Items(0), t.OnLineItems(1));
    }

    [Fact]
    public void Add_依次占用空闲槽_由调用方置Socket后推进()
    {
        var c = new TSelectClient();
        var t = new TSelectChar();
        for (int i = 0; i < 5; i++)
        {
            int idx = t.Add();
            Assert.Equal(i, idx);
            t.Items(idx)!.Socket = c;                                             // 模拟 OpenUser 的赋值
        }
        Assert.Equal(5, t.OnLineCount);
    }

    [Fact]
    public void Add_表满且回收表为空返回负一()
    {
        var c = new TSelectClient();
        var t = new TSelectChar();
        for (int i = 0; i < t.Count; i++) t.Items(i)!.Socket = c;
        Assert.Equal(-1, t.Add());                                                // 原文 :147 的初值
        Assert.Equal(0, t.OnLineCount);
    }

    [Fact]
    public void Add_回收表非空时优先用回收下标_不再线性扫描()
    {
        var t = new TSelectChar();
        AddAt(t, 3);                // 辅助：把 3 放进 OnLineList 并占用 [0..3]
        t.Finalize(3);
        Assert.Equal(3, t.Add());                                                 // 槽 0..2 空闲也不用
    }

    [Fact]
    public void Add_回收表用尽后回到线性扫描_从0开始()
    {
        // ★ 差异断言：回收表是 FIFO；两格用尽后并不是返回 -1，而是**回到线性扫描**拿第一个 Socket = nil 的槽（0）。
        var t = new TSelectChar();
        t.Finalize(1);
        t.Finalize(2);
        Assert.Equal(1, t.Add());
        Assert.Equal(2, t.Add());
        Assert.Equal(0, t.Add());
        Assert.Equal(0, t.Add());                                                 // Add 不写 Socket ⇒ 又拿 0
        Assert.Equal(4, t.OnLineCount);
    }

    // ------------------------------------------------------------------ Initialize

    [Fact]
    public void Initialize_复位全部字段并清空两张表()
    {
        var t = new TSelectChar();
        AddAt(t, 0);
        t.Finalize(9);                                                            // DeleteList 里留一个 9

        DelphiTick.GetTickCount = () => 4242;
        t.Initialize();

        Assert.Equal(0, t.OnLineCount);
        var u = t.Items(0)!;
        Assert.Equal(-1, u.nIndex);
        Assert.Equal("", u.sAccount);
        Assert.Equal("", u.sUserIPaddr);
        Assert.Equal("", u.sGateIPaddr);
        Assert.Equal("", u.sConnID);
        Assert.Equal(0, u.nSessionID);
        Assert.Null(u.Socket);
        Assert.Equal("", u.sReceiveText);
        Assert.Equal(4242u, u.dwTick34);
        Assert.Equal(4242u, u.dwChrTick);
        Assert.False(u.boChrSelected);
        Assert.False(u.boChrQueryed);
        Assert.Equal(0, u.nSelGateID);
        // DeleteList 被清空 ⇒ 再次 Add 应走线性扫描拿 0
        Assert.Equal(0, t.Add());
    }

    [Fact]
    public void Initialize_Index_越界不抛且不动两张表()
    {
        var t = new TSelectChar();
        AddAt(t, 0);
        t.Initialize(-1);
        t.Initialize(1000);
        Assert.Equal(1, t.OnLineCount);                                           // 原文 :221 `if UserInfo <> nil` 整段跳过
    }

    [Fact]
    public void Initialize_Index_与无参Initialize设同一批字段_但不清表()
    {
        var t = new TSelectChar();
        AddAt(t, 5);
        DelphiTick.GetTickCount = () => 77;
        t.Initialize(5);
        var u = t.Items(5)!;
        Assert.Equal(-1, u.nIndex);
        Assert.Equal(0, u.nSessionID);
        Assert.Null(u.Socket);
        Assert.Equal(77u, u.dwTick34);
        Assert.Equal(1, t.OnLineCount);                                           // ★ 差异：无参 Initialize 会清表，这里不会
    }

    // ------------------------------------------------------------------ Finalize（**与 Initialize 的差异是本组的核心**）

    [Fact]
    public void Finalize_只清7个字段_会话号与各种标记保持()
    {
        var c = new TSelectClient();
        var t = new TSelectChar();
        var u = t.Items(4)!;
        u.nIndex = 4;
        u.sAccount = "acc";
        u.sUserIPaddr = "1.1.1.1";
        u.sGateIPaddr = "2.2.2.2";
        u.sConnID = "77";
        u.nSessionID = 12345;
        u.Socket = c;
        u.sReceiveText = "#abc!";
        u.boChrSelected = true;
        u.boChrQueryed = true;
        u.dwTick34 = 111;
        u.dwChrTick = 222;
        u.nSelGateID = 9;

        t.Finalize();

        Assert.Equal(-1, u.nIndex);
        Assert.Null(u.Socket);
        Assert.Equal("", u.sAccount);
        Assert.Equal("", u.sUserIPaddr);
        Assert.Equal("", u.sGateIPaddr);
        Assert.Equal("", u.sConnID);
        Assert.Equal("", u.sReceiveText);
        // ★★ 以下 6 项 Finalize **不**清（对比 Initialize 会清）
        Assert.Equal(12345, u.nSessionID);
        Assert.True(u.boChrSelected);
        Assert.True(u.boChrQueryed);
        Assert.Equal(111u, u.dwTick34);
        Assert.Equal(222u, u.dwChrTick);
        Assert.Equal(9, u.nSelGateID);
    }

    [Fact]
    public void Finalize_不清空OnLineList与DeleteList()
    {
        var t = new TSelectChar();
        AddAt(t, 1);
        t.Finalize();
        Assert.Equal(1, t.OnLineCount);                                           // ★ 差异：无参 Initialize 会清
    }

    [Fact]
    public void Initialize_与_Finalize_字段集差异_逐字段锁定()
    {
        var a = new TSelectChar();
        var b = new TSelectChar();
        foreach (var t in new[] { a, b })
        {
            var u = t.Items(0)!;
            u.nIndex = 7; u.sAccount = "x"; u.sUserIPaddr = "y"; u.sGateIPaddr = "z";
            u.sConnID = "c"; u.nSessionID = 99; u.Socket = new TSelectClient();
            u.sReceiveText = "r"; u.boChrSelected = true; u.boChrQueryed = true;
            u.dwTick34 = 5; u.dwChrTick = 6; u.nSelGateID = 3;
        }
        a.Initialize();
        b.Finalize();
        var ua = a.Items(0)!;
        var ub = b.Items(0)!;
        Assert.Equal(0, ua.nSessionID);        Assert.Equal(99, ub.nSessionID);
        Assert.False(ua.boChrSelected);        Assert.True(ub.boChrSelected);
        Assert.False(ua.boChrQueryed);         Assert.True(ub.boChrQueryed);
        Assert.Equal(0u, ua.dwTick34);         Assert.Equal(5u, ub.dwTick34);
        Assert.Equal(0u, ua.dwChrTick);        Assert.Equal(6u, ub.dwChrTick);
        Assert.Equal(0, ua.nSelGateID);        Assert.Equal(3, ub.nSelGateID);
        // 两边都清的 7 项
        Assert.Equal(-1, ua.nIndex);           Assert.Equal(-1, ub.nIndex);
        Assert.Null(ua.Socket);                Assert.Null(ub.Socket);
        Assert.Equal("", ua.sAccount);         Assert.Equal("", ub.sAccount);
        Assert.Equal("", ua.sReceiveText);     Assert.Equal("", ub.sReceiveText);
    }

    // ------------------------------------------------------------------ Finalize(Index)

    [Fact]
    public void Finalize_Index_摘掉OnLineList中的该下标并追加到回收表()
    {
        var t = new TSelectChar();
        AddAt(t, 2);
        AddAt(t, 5);
        Assert.Equal(2, t.OnLineCount);

        t.Finalize(2);

        Assert.Equal(1, t.OnLineCount);
        Assert.Same(t.Items(5), t.OnLineItems(0));                                // 5 顶上来
        Assert.Equal(2, t.Add());                                                 // 回收表里是 2
    }

    [Fact]
    public void Finalize_Index_越界不抛也不动两张表()
    {
        var t = new TSelectChar();
        AddAt(t, 0);
        t.Finalize(-1);
        t.Finalize(1000);
        Assert.Equal(1, t.OnLineCount);
        Assert.Equal(1, t.Add());                                                 // 回收表仍空 ⇒ 走线性扫描（槽 0 已被 AddAt 占用）
        Assert.Equal(2, t.OnLineCount);                                           // 没有多出别的条目
    }

    [Fact]
    public void Finalize_Index_重复调用同一槽会让回收表出现重复下标()
    {
        // ★ 原文如此（:248 无条件 DeleteList.Add）：重复 Finalize 后 Add 会把同一下标重复放进 OnLineList。
        var t = new TSelectChar();
        t.Finalize(6);
        t.Finalize(6);
        Assert.Equal(6, t.Add());
        Assert.Equal(6, t.Add());
        Assert.Equal(0, t.Add());                                                 // 回收表用尽 ⇒ 回到线性扫描拿槽 0
        Assert.Equal(3, t.OnLineCount);
        Assert.Same(t.Items(6), t.OnLineItems(0));
        Assert.Same(t.Items(6), t.OnLineItems(1));
        Assert.Same(t.Items(0), t.OnLineItems(2));
    }

    [Fact]
    public void Finalize_Index_只删OnLineList里的第一个匹配项()
    {
        // 先造出重复（Add 的回收分支不查重 ⇒ 两次 Finalize 后连续 Add 会重复）
        var t = new TSelectChar();
        t.Finalize(8);
        t.Finalize(8);
        t.Add();
        t.Add();                                                                  // OnLineList = [8, 8]
        Assert.Equal(2, t.OnLineCount);
        t.Finalize(8);                                                            // TList.Remove 只摘掉第一个
        Assert.Equal(1, t.OnLineCount);
        Assert.Same(t.Items(8), t.OnLineItems(0));
    }

    [Fact]
    public void Destroy_等价于无参Finalize()
    {
        var t = new TSelectChar();
        var u = t.Items(0)!;
        u.nSessionID = 5;
        u.Socket = new TSelectClient();
        t.Destroy();
        Assert.Null(u.Socket);
        Assert.Equal(5, u.nSessionID);                                            // 与 Finalize 同样的"不清 nSessionID"
    }
}

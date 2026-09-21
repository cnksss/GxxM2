using System;
using GXX.Core.Rtl;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// `Source\DBServer\DBShare.pas:731-848` 的**主动网关路由**（"只取可以连接的网关" chongchong 2015-07-22）。
///
/// ★★ **该路径默认关闭**：只有 `g_boUseActiveRunGage = True` 时 `SelectClient.pas:1138` 才走它，
///    默认 False ⇒ 走 `GateRouteIP`。所以这里的用例**必须显式打开开关**才能覆盖到真代码；
///    `SelectClientHostWiringTests` 里另有"默认关闭时不可达"的反例。
/// </summary>
public class DBShareActiveGateTests : TempDirTest
{
    public DBShareActiveGateTests()
    {
        SelectClientDbShareSeam.Reset();
        // 每条 RunGate 会话槽复位为"空"
        for (int i = 0; i < DBShareSeam.SessionRunGateArray.Length; i++)
            DBShareSeam.SessionRunGateArray[i] = new TSessionRunGateInfo();
        DelphiTick.GetTickCount = () => 0;
        DelphiRandom.Next = _ => 0;
    }

    // ---------------- 助手 ----------------

    private static TRouteInfo Route(int idx, string selGate, params (string ip, int port)[] gates)
    {
        var r = DBShareSeam.g_RouteInfo[idx];
        r.sSelGateIP = selGate;
        r.nGateCount = gates.Length;
        for (int i = 0; i < gates.Length; i++)
        {
            r.sGameGateIP[i] = gates[i].ip;
            r.nGameGatePort[i] = gates[i].port;
            r.dwGameGateConnectTick[i] = 0;                                        // 与 tick 0 同 ⇒ 视为已连通
        }
        r.EnabledRunGate2List = 0;
        r.GameGateDisconnectCount = 1;
        r.RunGate2List.Clear();
        return r;
    }

    /// <summary>
    /// 往备用列表加一条。
    /// ★ **必须配 `DoSort()`**：`TRunGateList.Add`（uRunGateList.cs:113-124）**只写 `FList`、不写 `FSortList`**，
    ///   而 `GateActiveRouteIP` 读的是 `SortItems`（= `FSortList`）—— 原文就是这个分工（uFrmMain 在装载后调 `DoSort`）。
    /// </summary>
    private static void AddRunGate(TRouteInfo r, bool enabled, string ip, ushort port, int level, uint lastResponseTick = 0)
    {
        TRunGateInfo info = r.RunGate2List.Add((byte)(enabled ? 1 : 0), ip, port, 0, level);
        info.LastResponseTick = lastResponseTick;
    }

    /// <summary>见 <see cref="AddRunGate"/> 的说明：读完原文顺序后必须显式排序。</summary>
    private static void SortRunGates(TRouteInfo r) => r.RunGate2List.DoSort();

    private static void AddSession(string addr, int port, uint receiveTick, bool live = true)
    {
        for (int i = 0; i < DBShareSeam.SessionRunGateArray.Length; i++)
        {
            var s = DBShareSeam.SessionRunGateArray[i];
            if (s.Socket == null)
            {
                if (live) s.Socket = new FakeWinSocket();
                else return;
                s.sRemoteAddr = addr;
                s.nRemotePort = port;
                s.dwReceiveTick = receiveTick;
                return;
            }
        }
    }

    private sealed class FakeWinSocket : TCustomWinSocket { }

    // =====================================================================================
    // GateActiveRouteIP —— 主列表
    // =====================================================================================

    [Fact]
    public void 未命中角色网关时返回空串与端口0()
    {
        Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        Assert.Equal("", DBShare.GateActiveRouteIP("9.9.9.9", out int port));
        Assert.Equal(0, port);                                                     // :837 `nPort := 0`
    }

    [Fact]
    public void 命中角色网关且唯一连通时返回该网关()
    {
        Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        Assert.Equal("10.0.0.1", DBShare.GateActiveRouteIP("127.0.0.1", out int port));
        Assert.Equal(7200, port);
    }

    [Fact]
    public void 只从已连通的网关里挑_超时的不入选()
    {
        var r = Route(0, "127.0.0.1", ("10.0.0.1", 7200), ("10.0.0.2", 7201));
        r.dwGameGateConnectTick[1] = 9999;                                         // 第 2 条"3000ms 没连上"…
        DelphiTick.GetTickCount = () => 0;                                         // …0 - 9999 回绕后很大 ⇒ 不入选
        Assert.Equal("10.0.0.1", DBShare.GateActiveRouteIP("127.0.0.1", out int port));
        Assert.Equal(7200, port);
    }

    [Fact]
    public void 全部超时且备用列表关闭时返回空()
    {
        var r = Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        r.dwGameGateConnectTick[0] = 5000;                                         // 0-5000 回绕 > 3000
        Assert.Equal("", DBShare.GateActiveRouteIP("127.0.0.1", out int port));
        Assert.Equal(0, port);
    }

    [Theory]
    [InlineData(3000u, true)]                                                      // 恰好 3000 ⇒ 连通（`<=`）
    [InlineData(3001u, false)]
    public void 主列表的连通判据是3000毫秒含边界(uint tick, bool connected)
    {
        var r = Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        DelphiTick.GetTickCount = () => 10000;
        r.dwGameGateConnectTick[0] = 10000 - tick;                                 // 差值恰为 tick
        string got = DBShare.GateActiveRouteIP("127.0.0.1", out _);
        Assert.Equal(connected ? "10.0.0.1" : "", got);
    }

    [Fact]
    public void 多条连通时按随机下标选()
    {
        Route(0, "127.0.0.1", ("10.0.0.1", 7200), ("10.0.0.2", 7201));
        DelphiRandom.Next = _ => 1;
        Assert.Equal("10.0.0.2", DBShare.GateActiveRouteIP("127.0.0.1", out int port));
        Assert.Equal(7201, port);

        DelphiRandom.Next = _ => 0;
        Assert.Equal("10.0.0.1", DBShare.GateActiveRouteIP("127.0.0.1", out _));
    }

    [Fact]
    public void 遍历g_RouteInfo全部20槽_先命中者胜出()
    {
        Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        Route(5, "127.0.0.2", ("10.0.0.9", 7300));

        Assert.Equal("10.0.0.9", DBShare.GateActiveRouteIP("127.0.0.2", out int port));
        Assert.Equal(7300, port);
        Assert.Equal(20, DBShareSeam.g_RouteInfo.Length);                          // array[0..19]
    }

    [Fact]
    public void 角色网关地址是精确比较()
    {
        Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        Assert.Equal("", DBShare.GateActiveRouteIP("127.0.0.2", out _));           // 原文 :842 `=`
    }

    // =====================================================================================
    // GateActiveRouteIP —— 备用列表
    // =====================================================================================

    [Fact]
    public void 备用列表_开启且掉线数达标时启用()
    {
        var r = Route(0, "127.0.0.1", ("10.0.0.1", 7200), ("10.0.0.2", 7201));
        r.EnabledRunGate2List = 1;
        r.GameGateDisconnectCount = 2;                                             // 2 - 0 = 2 >= 2 ⇒ 达标
        r.dwGameGateConnectTick[0] = 5000;
        r.dwGameGateConnectTick[1] = 5000;                                         // 主列表 0 条
        AddRunGate(r, true, "10.9.9.1", 7400, 5);                                  // 最高档
        AddRunGate(r, true, "10.9.9.2", 7401, 1);                                  // 低档（降档时已收集 ⇒ Break）
        SortRunGates(r);

        Assert.Equal("10.9.9.1", DBShare.GateActiveRouteIP("127.0.0.1", out int port));
        Assert.Equal(7400, port);
    }

    [Fact]
    public void 备用列表_掉线数不达标时不启用()
    {
        var r = Route(0, "127.0.0.1", ("10.0.0.1", 7200), ("10.0.0.2", 7201));
        r.EnabledRunGate2List = 1;
        r.GameGateDisconnectCount = 3;                                             // 2 - 0 = 2 < 3 ⇒ 不达标
        r.dwGameGateConnectTick[0] = 5000;
        r.dwGameGateConnectTick[1] = 5000;
        AddRunGate(r, true, "10.9.9.1", 7400, 5);
        AddRunGate(r, true, "10.9.9.2", 7401, 1);
        SortRunGates(r);

        Assert.Equal("", DBShare.GateActiveRouteIP("127.0.0.1", out _));
    }

    [Fact]
    public void 备用列表_关闭时即使条件满足也不启用()
    {
        var r = Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        r.EnabledRunGate2List = 0;
        r.GameGateDisconnectCount = 1;
        r.dwGameGateConnectTick[0] = 5000;
        AddRunGate(r, true, "10.9.9.1", 7400, 5);
        AddRunGate(r, true, "10.9.9.2", 7401, 1);
        SortRunGates(r);

        Assert.Equal("", DBShare.GateActiveRouteIP("127.0.0.1", out _));           // :770 空列表 + 未开启 ⇒ 直接空
    }

    [Fact]
    public void 备用列表_优先取最高Level()
    {
        var r = Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        r.EnabledRunGate2List = 1;
        r.GameGateDisconnectCount = 1;
        r.dwGameGateConnectTick[0] = 5000;                                         // 主列表空
        AddRunGate(r, true, "10.9.9.1", 7400, 1);                                  // 低档（SortItems 后在后）
        AddRunGate(r, true, "10.9.9.2", 7401, 5);                                  // 高档（DoSort 降序 ⇒ 在前）
        SortRunGates(r);

        Assert.Equal("10.9.9.2", DBShare.GateActiveRouteIP("127.0.0.1", out int port));
        Assert.Equal(7401, port);
    }

    [Fact]
    public void 备用列表_降档时若已收集到就Break()
    {
        var r = Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        r.EnabledRunGate2List = 1;
        r.GameGateDisconnectCount = 1;
        r.dwGameGateConnectTick[0] = 5000;
        AddRunGate(r, true, "10.9.9.1", 7400, 9);                                  // 最高档
        AddRunGate(r, true, "10.9.9.2", 7401, 1);                                  // 更低档 ⇒ :806 已收集 ⇒ Break
        SortRunGates(r);

        Assert.Equal("10.9.9.1", DBShare.GateActiveRouteIP("127.0.0.1", out _));
    }

    [Fact]
    public void 备用列表_未启用Enabled的条目不入选()
    {
        var r = Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        r.EnabledRunGate2List = 1;
        r.GameGateDisconnectCount = 1;
        r.dwGameGateConnectTick[0] = 5000;
        AddRunGate(r, false, "10.9.9.1", 7400, 9);
        AddRunGate(r, true, "10.9.9.2", 7401, 9);
        SortRunGates(r);

        Assert.Equal("10.9.9.2", DBShare.GateActiveRouteIP("127.0.0.1", out _));
    }

    [Fact]
    public void 备用列表_LastResponseTick超时不入选()
    {
        var r = Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        r.EnabledRunGate2List = 1;
        r.GameGateDisconnectCount = 1;
        r.dwGameGateConnectTick[0] = 5000;
        AddRunGate(r, true, "10.9.9.1", 7400, 9, lastResponseTick: 9999);          // 0 - 9999 回绕 > 3000
        AddRunGate(r, true, "10.9.9.2", 7401, 9, lastResponseTick: 9999);          // 同样超时
        SortRunGates(r);

        Assert.Equal("", DBShare.GateActiveRouteIP("127.0.0.1", out _));
    }

    [Fact]
    public void 备用列表_结果会覆盖主列表的选择_原文如此()
    {
        // ★ 原文 :776-785 先按主列表设 Result，随后 :818-827 的备用列表**直接覆盖**它（没有"仅当主列表空"的判断）。
        //   本用例把这一原文行为钉死：即使主列表成功选出 10.0.0.1，只要备用列表条件成立且选出条目，就返回备用 IP。
        var r = Route(0, "127.0.0.1", ("10.0.0.1", 7200), ("10.0.0.2", 7201));
        r.EnabledRunGate2List = 1;
        r.GameGateDisconnectCount = 1;                                             // 2 - 2 = 0 < 1 ⇒ 不达标
        AddRunGate(r, true, "10.9.9.1", 7400, 5);
        AddRunGate(r, true, "10.9.9.2", 7401, 1);
        SortRunGates(r);
        Assert.Equal("10.0.0.1", DBShare.GateActiveRouteIP("127.0.0.1", out _));   // 不达标 ⇒ 主列表结果保留

        r.GameGateDisconnectCount = 0;                                             // 2 - 2 = 0 >= 0 ⇒ 达标
        Assert.Equal("10.9.9.1", DBShare.GateActiveRouteIP("127.0.0.1", out int port));
        Assert.Equal(7400, port);                                                  // ★ 备用把主列表覆盖掉了
    }

    [Fact]
    public void 备用列表_只有一条且从未按两条以上DoSort过时_SortItems为空()
    {
        // ★★ 原文缺陷锁定（跨单元组合出来的坑，不是 GateActiveRouteIP 自己的错）：
        //   `uRunGateList.pas` 的 `DoSort` 只在 `FList.Count > 1` 时重建 `FSortList`
        //   （uRunGateList.cs:171-176 逐字保留），而 `GateActiveRouteIP` 用 **FList 的 `Count`**
        //   判"备用列表非空"（:790）、却用 **`SortItems`** 取元素（:793/:796/:824）。
        //   ⇒ **只有一条备用网关、且从未在"两条以上"时 DoSort 过** 时，`FSortList` 为空，
        //     `SortItems(0)` 返回 nil ⇒ 托管侧 NRE（Delphi 侧是空指针/下标越界）。
        //   本用例把这个边界钉死；真实部署里 uFrmMain 装载完整列表后会 DoSort，故只在这个窄条件下可达。
        var r = Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        r.EnabledRunGate2List = 1;
        r.GameGateDisconnectCount = 1;
        r.dwGameGateConnectTick[0] = 5000;                                         // 主列表空
        AddRunGate(r, true, "10.9.9.1", 7400, 5);                                  // 仅一条 ⇒ DoSort 不重建
        r.RunGate2List.DoSort();
        Assert.Null(r.RunGate2List.SortItems(0));                                  // FSortList 仍为空

        Assert.Throws<NullReferenceException>(() => DBShare.GateActiveRouteIP("127.0.0.1", out _));
    }

    // =====================================================================================
    // CheckActiveRunGate（:731-749）
    // =====================================================================================

    [Fact]
    public void CheckActiveRunGate_无会话时为假()
    {
        Assert.False(DBShare.CheckActiveRunGate("10.1.1.1", 5600));
    }

    [Fact]
    public void CheckActiveRunGate_同IP同端口且2500毫秒内为真()
    {
        AddSession("10.1.1.1", 5600, 0);
        Assert.True(DBShare.CheckActiveRunGate("10.1.1.1", 5600));
    }

    [Theory]
    [InlineData(2500u, true)]                                                      // 恰好 2500 ⇒ 真（`<=`）
    [InlineData(2501u, false)]
    public void CheckActiveRunGate_判据是2500毫秒含边界(uint elapsed, bool expected)
    {
        DelphiTick.GetTickCount = () => 10000;
        AddSession("10.1.1.1", 5600, 10000 - elapsed);
        Assert.Equal(expected, DBShare.CheckActiveRunGate("10.1.1.1", 5600));
    }

    [Fact]
    public void CheckActiveRunGate_端口不符为假()
    {
        AddSession("10.1.1.1", 5600, 0);
        Assert.False(DBShare.CheckActiveRunGate("10.1.1.1", 5601));
    }

    [Fact]
    public void CheckActiveRunGate_IP忽略大小写()
    {
        AddSession("Gateway.Example", 5600, 0);
        Assert.True(DBShare.CheckActiveRunGate("gateway.example", 5600));          // 原文 SameText
    }

    [Fact]
    public void CheckActiveRunGate_Socket为nil的槽被跳过()
    {
        for (int i = 0; i < DBShareSeam.SessionRunGateArray.Length; i++)
        {
            var s = DBShareSeam.SessionRunGateArray[i];
            s.sRemoteAddr = "10.1.1.1";                                            // 地址对，但 Socket 为 nil
            s.nRemotePort = 5600;
            s.dwReceiveTick = 0;
        }
        Assert.False(DBShare.CheckActiveRunGate("10.1.1.1", 5600));                // :740 判 Socket <> nil
    }

    [Fact]
    public void CheckActiveRunGate_遍历全部100槽()
    {
        Assert.Equal(100, DBShareSeam.SessionRunGateArray.Length);                 // RUNGATEMAXSESSION
        // 只在最后一槽放会话
        var s = DBShareSeam.SessionRunGateArray[99];
        s.Socket = new FakeWinSocket();
        s.sRemoteAddr = "10.1.1.1";
        s.nRemotePort = 5600;
        s.dwReceiveTick = 0;
        Assert.True(DBShare.CheckActiveRunGate("10.1.1.1", 5600));
    }

    // =====================================================================================
    // 接缝转调
    // =====================================================================================

    [Fact]
    public void 接缝已转调真实现_不再抛()
    {
        Route(0, "127.0.0.1", ("10.0.0.1", 7200));
        Assert.Equal("10.0.0.1", SelectClientDbShareSeam.GateActiveRouteIP("127.0.0.1", out int port));
        Assert.Equal(7200, port);

        AddSession("10.1.1.1", 5600, 0);
        Assert.True(SelectClientDbShareSeam.CheckActiveRunGate("10.1.1.1", 5600));
    }

    [Fact]
    public void 开关默认关闭_这一族是死路径()
    {
        // ★ 把"默认关闭"这件事本身钉死：`g_boUseActiveRunGage` 的声明初值是 False（DBShare.pas:227）。
        Assert.Equal(0, DBShareSeam.g_boUseActiveRunGage);
    }
}

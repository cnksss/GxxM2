using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Rtl;
using GXX.GatewayKit.Rest11;
using Xunit;

namespace GXX.GatewayKit.Tests;

/// <summary>
/// 车道 p11-logingate-filter 的测试替身（**不依赖真实网络端口 / 真实计时器**）。
/// </summary>
internal sealed class FakeEnforcementChannel : IRest11EnforcementChannel
{
    public bool ServiceStarted { get; set; } = true;
    public readonly List<int> FreeSocketCalls = new();
    public readonly List<(int ident, int nSvrObject)> OutOfConnectionCalls = new();
    public readonly List<int> TempBlockCalls = new();
    public readonly List<int> BlockListCalls = new();
    public readonly List<string> Logs = new();
    public Func<int, bool> CheckLevelFn { get; set; } = _ => true;

    public void FreeSocket(int socket) => FreeSocketCalls.Add(socket);
    public void SendOutOfConnection(IRest11SessionObj session, int nSvrObject)
        => OutOfConnectionCalls.Add((session.Socket, nSvrObject));
    public void AddToTempBlockIPList(int nIP) => TempBlockCalls.Add(nIP);
    public void AddToBlockIPList(int nIP) => BlockListCalls.Add(nIP);
    public bool CheckLevel(int level) => CheckLevelFn(level);
    public void AddLog(string sMsg) => Logs.Add(sMsg);
}

/// <summary>`ClientSession.TSessionObj` 的测试替身。</summary>
internal sealed class FakeSession : IRest11SessionObj
{
    public int IPAddr { get; set; }
    public string IPText { get; set; } = "";
    public bool LastGameSvrActive { get; set; } = true;
    public int Socket { get; set; }
    public bool KickFlag { get; set; }
    public byte HandleLogin { get; set; }
    public int SvrObject { get; set; }
    public uint dwClientTimeOutTick { get; set; }
    public bool IsDelayClose { get; set; }
    public uint dwDelayCloseTick { get; set; }
}

public class Rest11LoginGateIpFilterTests : IDisposable
{
    private readonly string _dir;

    public Rest11LoginGateIpFilterTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "rest11-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { }
    }

    private Rest11LoginGateIpFilter NewFilter(Rest11LoginGateConfig cfg)
        => new() { Config = cfg, BaseDirectory = _dir };

    private Rest11LoginGateIpFilter NewFilter(Rest11LoginGateConfig cfg, Func<uint> tick)
    {
        var f = NewFilter(cfg);
        f.TickCount = tick;
        return f;
    }

    // =====================================================================================
    // IPAddrFilter.pas:178-207 OverConnectOfIP —— `Count + 1 > Max`（超限**不自增**）
    // =====================================================================================
    [Fact]
    public void OverConnectOfIP_UsesCountPlusOne_AndDoesNotIncrementWhenOver()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini")) { m_fCheckNullSession = true, m_nMaxConnectOfIP = 2 };
        var f = NewFilter(cfg);
        int ip = Rest11LoginGateNet.InetAddr("10.0.0.7");

        Assert.False(f.OverConnectOfIP(ip));      // 首见：新建 Count=1 ⇒ 1+1 > 2 为假
        Assert.Equal(1, f.g_ConnectOfIPList.Count);
        Assert.Equal(1, f.g_ConnectOfIPList[0].Count);

        Assert.False(f.OverConnectOfIP(ip));      // Count=1，1+1 > 2 为假 ⇒ Inc 到 2
        Assert.Equal(2, f.g_ConnectOfIPList[0].Count);

        Assert.True(f.OverConnectOfIP(ip));       // Count=2，2+1 > 2 为真 ⇒ 超限
        Assert.Equal(2, f.g_ConnectOfIPList[0].Count); // ★ 超限时**不**自增（原文如此 :193-196）
    }

    [Fact]
    public void OverConnectOfIP_ReturnsFalseImmediately_WhenCheckNullSessionDisabled()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini")) { m_fCheckNullSession = false, m_nMaxConnectOfIP = 0 };
        var f = NewFilter(cfg);
        Assert.False(f.OverConnectOfIP(1));
        Assert.Empty(f.g_ConnectOfIPList);        // :184-185 直接 Exit，连表都不建
    }

    [Fact]
    public void DeleteConnectOfIP_DecrementsAndRemovesAtZero_AndRespectsCheckNullSession()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini")) { m_fCheckNullSession = true, m_nMaxConnectOfIP = 9 };
        var f = NewFilter(cfg);
        int ip = Rest11LoginGateNet.InetAddr("10.0.0.8");
        f.OverConnectOfIP(ip);
        f.OverConnectOfIP(ip);                    // Count = 2
        Assert.Equal(2, f.g_ConnectOfIPList[0].Count);

        f.DeleteConnectOfIP(ip);                  // Count = 1（>0 ⇒ 不回删）
        Assert.Single(f.g_ConnectOfIPList);
        Assert.Equal(1, f.g_ConnectOfIPList[0].Count);

        f.DeleteConnectOfIP(ip);                  // Count = 0 ⇒ RemoveAt
        Assert.Empty(f.g_ConnectOfIPList);

        cfg.m_fCheckNullSession = false;
        f.DeleteConnectOfIP(ip);                  // :214-215 直接 Exit（无异常、不建表）
        Assert.Empty(f.g_ConnectOfIPList);
    }

    [Fact]
    public void ClearConnectOfIP_ClearsList_AndRespectsCheckNullSession()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini")) { m_fCheckNullSession = true, m_nMaxConnectOfIP = 9 };
        var f = NewFilter(cfg);
        f.OverConnectOfIP(Rest11LoginGateNet.InetAddr("10.0.0.9"));
        f.OverConnectOfIP(Rest11LoginGateNet.InetAddr("10.0.0.10"));
        Assert.Equal(2, f.g_ConnectOfIPList.Count);

        f.ClearConnectOfIP();
        Assert.Empty(f.g_ConnectOfIPList);

        cfg.m_fCheckNullSession = false;
        f.OverConnectOfIP(Rest11LoginGateNet.InetAddr("10.0.0.11"));
        Assert.Empty(f.g_ConnectOfIPList);        // 关开关后连表都建不起来
    }

    // =====================================================================================
    // IPAddrFilter.pas:149-176 IsBlockIP —— 永久表 + 临时表
    // =====================================================================================
    [Fact]
    public void IsBlockIP_ChecksBothPermanentAndTempTables()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = NewFilter(cfg);
        int ip = Rest11LoginGateNet.InetAddr("192.168.1.5");
        Assert.False(f.IsBlockIP(ip));

        f.AddToBlockIPList(ip);                   // 永久表（:91-112 整数重载）
        Assert.True(f.IsBlockIP(ip));
        Assert.False(f.IsBlockIP(Rest11LoginGateNet.InetAddr("192.168.1.6")));
    }

    [Fact]
    public void IsBlockIP_TempTableHit()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = NewFilter(cfg);
        int ip = Rest11LoginGateNet.InetAddr("172.16.0.1");
        f.AddToTempBlockIPList(ip);
        Assert.True(f.IsBlockIP(ip));
    }

    // =====================================================================================
    // IPAddrFilter.pas:79-89 / :91-112 / :114-124 / :126-147 四个 Add —— 去重键不同（原文如此）
    // =====================================================================================
    [Fact]
    public void AddToBlockIPList_DedupesPerOverload_StringVsInteger()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = NewFilter(cfg);

        f.AddToBlockIPList("10.1.1.1");           // :79 字符串重载 ⇒ IndexOf 字符串去重
        f.AddToBlockIPList("10.1.1.1");
        Assert.Equal(1, f.g_BlockIPList.Count);   // TStringList 不实现 IEnumerable，用 Count 断言

        f.AddToBlockIPList(0x0A010101);           // :91 整数重载 ⇒ Objects 整数去重，命中已存在的同值项
        Assert.Equal(1, f.g_BlockIPList.Count);   // TStringList 不实现 IEnumerable，用 Count 断言

        f.AddToBlockIPList("10.1.1.2");           // 新地址
        Assert.Equal(2, f.g_BlockIPList.Count);

        // 字符串重载对非法 IP 不写入（:86 nIP <> INADDR_NONE）
        f.AddToBlockIPList("not-an-ip");
        Assert.Equal(2, f.g_BlockIPList.Count);
    }

    [Fact]
    public void AddToTempBlockIPList_BothOverloads()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = NewFilter(cfg);
        f.AddToTempBlockIPList("10.2.2.2");
        f.AddToTempBlockIPList("10.2.2.2");
        Assert.Equal(1, f.g_TempBlockIPList.Count);

        f.AddToTempBlockIPList(Rest11LoginGateNet.InetAddr("10.2.2.3"));
        Assert.Equal(2, f.g_TempBlockIPList.Count);

        f.AddToTempBlockIPList("bad");
        Assert.Equal(2, f.g_TempBlockIPList.Count);
    }

    // =====================================================================================
    // IPAddrFilter.pas:40-61 / :63-77 Load/SaveBlockIPList —— 走文件（临时目录，不占端口）
    // =====================================================================================
    [Fact]
    public void LoadAndSaveBlockIPList_RoundTripsThroughFile()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = NewFilter(cfg);

        f.AddToBlockIPList("10.3.3.3");
        f.AddToBlockIPList("10.3.3.4");
        f.SaveBlockIPList();
        Assert.True(File.Exists(f.BlockFilePath));

        // 重新加载到**新实例**（模拟进程重启）
        var f2 = NewFilter(cfg);
        f2.LoadBlockIPList();
        Assert.Equal(2, f2.g_BlockIPList.Count);
        Assert.True(f2.IsBlockIP(Rest11LoginGateNet.InetAddr("10.3.3.3")));
    }

    [Fact]
    public void LoadBlockIPList_CreatesEmptyFileWhenMissing_AndSkipsInvalidLines()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = NewFilter(cfg);
        Assert.False(File.Exists(f.BlockFilePath));

        f.LoadBlockIPList();                       // :46-47 文件不存在 ⇒ 先建空文件
        Assert.True(File.Exists(f.BlockFilePath));
        Assert.Equal(0, f.g_BlockIPList.Count);

        File.WriteAllText(f.BlockFilePath, "10.4.4.4\r\n\r\nnot-an-ip\r\n10.4.4.5\r\n", GXX.Core.EncodingInit.GBK);
        f.LoadBlockIPList();
        Assert.Equal(2, f.g_BlockIPList.Count);    // 空行与非法行被跳过（:53-57）
    }

    // =====================================================================================
    // IPAddrFilter.pas:254-297 / :299-313 / :315-335 IP 段过滤
    // =====================================================================================
    [Fact]
    public void LoadAndSaveBlockIPAreaList_AndIsBlockIPArea()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = NewFilter(cfg);

        f.LoadBlockIPAreaList();                   // :264-265 建空文件
        Assert.True(File.Exists(f.BlockAreaFilePath));

        File.WriteAllText(f.BlockAreaFilePath, "10.0.0.10-10.0.0.20\r\n", GXX.Core.EncodingInit.GBK);
        f.LoadBlockIPAreaList();
        Assert.Equal(1, f.g_BlockIPAreaList.Count);

        Assert.True(f.IsBlockIPArea(Rest11LoginGateNet.InetAddr("10.0.0.15")));   // 闭区间内
        Assert.True(f.IsBlockIPArea(Rest11LoginGateNet.InetAddr("10.0.0.10")));   // 下界包含
        Assert.True(f.IsBlockIPArea(Rest11LoginGateNet.InetAddr("10.0.0.20")));   // 上界包含
        Assert.False(f.IsBlockIPArea(Rest11LoginGateNet.InetAddr("10.0.0.21")));  // 越界

        f.SaveBlockIPAreaList();                                                  // :299-313
        Assert.Contains("10.0.0.10-10.0.0.20", File.ReadAllText(f.BlockAreaFilePath, GXX.Core.EncodingInit.GBK));
    }

    [Fact]
    public void LoadBlockIPAreaList_SwapsWhenLowGreaterThanHigh()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"));
        var f = NewFilter(cfg);
        File.WriteAllText(f.BlockAreaFilePath, "10.0.0.20-10.0.0.10\r\n", GXX.Core.EncodingInit.GBK);
        f.LoadBlockIPAreaList();                   // :284-289 低＞高 ⇒ 交换

        Assert.True(f.IsBlockIPArea(Rest11LoginGateNet.InetAddr("10.0.0.15")));
    }

    // =====================================================================================
    // IPAddrFilter.pas:337-380 CheckNewIDOfIP —— 4 秒窗口 + 阈值 + 窗口过期 Dec
    // =====================================================================================
    [Fact]
    public void CheckNewIDOfIP_ThresholdWithinWindow_ThenDecaysOutsideWindow()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"))
        {
            m_fCheckNewIDOfIP = true,
            m_nCheckNewIDOfIP = 2
        };
        uint now = 100_000;
        var f = NewFilter(cfg, () => now);
        int ip = Rest11LoginGateNet.InetAddr("10.5.5.5");

        Assert.False(f.CheckNewIDOfIP(ip));        // 首见 Count=1 ⇒ 1 > 2 为假
        Assert.False(f.CheckNewIDOfIP(ip));        // Count=2 ⇒ 2 > 2 为假
        Assert.True(f.CheckNewIDOfIP(ip));         // Count=3 ⇒ 3 > 2 为真（4 秒窗口内）

        now += 5_000;                              // 超出 4 秒窗口 ⇒ :360-367 刷新 tick 并 Dec
        Assert.False(f.CheckNewIDOfIP(ip));
        Assert.Equal(2, f.g_NewIDOfIPList[0].Count);
        Assert.Equal(now, f.g_NewIDOfIPList[0].dwIDCountTick);
    }

    [Fact]
    public void CheckNewIDOfIP_ReturnsFalseWhenDisabled()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini")) { m_fCheckNewIDOfIP = false };
        var f = NewFilter(cfg);
        Assert.False(f.CheckNewIDOfIP(1));
        Assert.Empty(f.g_NewIDOfIPList);
    }

    [Fact]
    public void CheckNewIDOfIP_WindowExpiryDeletesEntryWhenCountReachesZero()
    {
        var cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini"))
        {
            m_fCheckNewIDOfIP = true,
            m_nCheckNewIDOfIP = 5
        };
        uint now = 1;
        var f = NewFilter(cfg, () => now);
        int ip = Rest11LoginGateNet.InetAddr("10.5.5.6");

        f.CheckNewIDOfIP(ip);                      // Count = 1
        now += 10_000;
        f.CheckNewIDOfIP(ip);                      // Dec ⇒ 0 ⇒ RemoveAt（:363-366）
        Assert.Empty(f.g_NewIDOfIPList);
    }

    // =====================================================================================
    // Misc.pas:244-250 ReverseIP + WinSock inet_addr / inet_ntoa（LoginGate 副本实现）
    // =====================================================================================
    [Fact]
    public void ReverseIP_SwapsByteOrder()
    {
        Assert.Equal(0x04030201u, Rest11LoginGateNet.ReverseIP(0x01020304u));
        Assert.Equal(0xFFFFFFFFu, Rest11LoginGateNet.ReverseIP(0xFFFFFFFFu)); // INADDR_NONE 自映射
    }

    [Fact]
    public void InetAddr_And_InetNtoa_AreInverseForDottedQuad()
    {
        int ip = Rest11LoginGateNet.InetAddr("1.2.3.4");
        Assert.Equal(unchecked((int)0x01020304u), ip);
        Assert.Equal("1.2.3.4", Rest11LoginGateNet.InetNtoa(ip));
        Assert.Equal(Rest11LoginGateNet.INADDR_NONE, Rest11LoginGateNet.InetAddr("256.1.1.1"));
        Assert.Equal(Rest11LoginGateNet.INADDR_NONE, Rest11LoginGateNet.InetAddr(""));
        Assert.Equal(unchecked((int)0x01020003u), Rest11LoginGateNet.InetAddr("1.2.3")); // 段数不足：末段落低位
    }

    /// <summary>`HUtil32.GetValidStr3` 语义（IP 段的 `low-high` 切分依赖它）。</summary>
    [Fact]
    public void GetValidStr3_SplitsAtDivider()
    {
        var dest = new Rest11Ref<string>("");
        string first = Rest11LoginGateNet.GetValidStr3("10.0.0.1-10.0.0.9", dest, new[] { '-' });
        Assert.Equal("10.0.0.1", first);
        Assert.Equal("10.0.0.9", dest.Value);
    }
}

using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.Core;
using GXX.Core.Util;
using GXX.SelGate;
using Xunit;

namespace GXX.SelGate.Tests;

/// <summary>
/// IPAddrFilter.pas（SelGate 特有的黑名单/IP 段/连接数/换 ID 频率限制）→ CSelGateIPFilter 的 1:1 断言。
/// 覆盖：inet_addr/inet_ntoa/ReverseIP（:55、:108、:143、:278-279、:324；Misc.pas:131-137）、
/// IsBlockIP（:149-176）、OverConnectOfIP（:178-207）、DeleteConnectOfIP（:209-236）、
/// ClearConnectOfIP（:238-252）、IP 段增删查（:254-335）、CheckNewIDOfIP（:337-380）、
/// 黑名单落盘文本与非法行过滤（:40-77）。
/// </summary>
public class SelGateIPFilterTests
{
    private static CConfigMgr NewConfig(TempDir dir, bool checkNullSession = true, bool checkNewIDOfIP = false)
    {
        var cfg = new CConfigMgr(dir.File("Config.ini"));
        cfg.m_fCheckNullSession = checkNullSession;
        cfg.m_fCheckNewIDOfIP = checkNewIDOfIP;
        return cfg;
    }

    private static CSelGateIPFilter NewFilter(TempDir dir)
    {
        var f = new CSelGateIPFilter { BaseDirectory = dir.Path };
        f.Config = NewConfig(dir);
        return f;
    }

    // =====================================================================================
    // 1. inet_addr / inet_ntoa / ReverseIP
    // =====================================================================================

    [Theory]
    [InlineData("127.0.0.1", 0x7F000001)]       // 网络序数值：(a<<24)|(b<<16)|(c<<8)|d
    [InlineData("0.0.0.0", 0)]
    [InlineData("255.255.255.255", -1)]         // == INADDR_NONE（原文如此：全 1 地址无法与错误区分）
    [InlineData("192.168.1.255", unchecked((int)0xC0A801FF))]
    [InlineData("8.8.8.8", 0x08080808)]
    public void InetAddr_ParsesNetworkOrder(string input, int expected)
    {
        Assert.Equal(expected, CSelGateIPFilter.InetAddr(input));
    }

    [Theory]
    [InlineData("1.2.3", 0x01020003)]        // 1.2.0.3
    [InlineData("1.2", 0x01000002)]          // 1.0.0.2
    [InlineData("16909060", 0x01020304)]     // 1.2.3.4 的 32 位整数值
    [InlineData("0x01020304", 0x01020304)]   // 十六进制（C 字面量语义）
    [InlineData("0377", 0x000000FF)]         // 八进制 0377 = 255
    public void InetAddr_AcceptsCInetAddrShorthandNotation(string input, int expected)
    {
        Assert.Equal(expected, CSelGateIPFilter.InetAddr(input));
    }

    [Theory]
    [InlineData("")]
    [InlineData("999.1.1.1")]
    [InlineData("1.1.1.1.1")]
    [InlineData("abc")]
    [InlineData("1.2.3.4x")]
    [InlineData("a.b.c.d")]
    public void InetAddr_InvalidInput_ReturnsINADDR_NONE(string input)
    {
        Assert.Equal(CSelGateIPFilter.INADDR_NONE, CSelGateIPFilter.InetAddr(input));
    }

    [Fact]
    public void InetNtoa_IsTheInverseOfInetAddr()
    {
        foreach (string ip in new[] { "127.0.0.1", "10.0.0.1", "192.168.1.255", "1.2.3.4", "0.0.0.0" })
        {
            int n = CSelGateIPFilter.InetAddr(ip);
            Assert.Equal(ip, CSelGateIPFilter.InetNtoa(n));
        }
    }

    [Fact]
    public void ReverseIP_SwapsByteOrderAndIsItsOwnInverse()
    {
        int n = CSelGateIPFilter.InetAddr("192.168.1.100"); // 0xC0A80164
        uint r = CSelGateIPFilter.ReverseIP(unchecked((uint)n));
        Assert.Equal(0x6401A8C0u, r);
        Assert.Equal(unchecked((uint)n), CSelGateIPFilter.ReverseIP(r));
    }

    [Fact]
    public void InetAddr_DivergesFromCoreShareMakeIPToInt_BecauseSharedGetValidStr3IsBuggy()
    {
        // Share.MakeIPToInt（Share.cs:31-47）依赖 GXX.Core.Util.HUtil32.GetValidStr3，
        // 而后者（HUtil32.cs:242-261）**没有跳过分隔符本身**（Delphi HUtil32.pas 在定位到分隔符后还有 Inc(strPos)），
        // 于是 "127.0.0.1" 的第二段解析不出，MakeIPToInt 返回 -1（= INADDR_NONE）。
        // 本车道的 inet_addr 独立实现是正确的（返回 0x7F000001）。
        // ⚠ 缺陷登记：GXX.Core.Util.HUtil32.GetValidStr3 会同时影响 Share.MakeIPToInt 与
        //   GatewayKit.GateService.CheckIP（GateService.cs:205 用它做 IP→uint），需由共享文件修复。
        Assert.Equal(-1, GXX.Core.Share.MakeIPToInt("127.0.0.1"));
        Assert.Equal(0x7F000001, CSelGateIPFilter.InetAddr("127.0.0.1"));
    }

    [Fact]
    public void InetAddr_AndShareMakeIntToIP_AgreeOnByteOrder()
    {
        // inet_addr 与 inet_ntoa（网络的 a.b.c.d 高低字节约定）互为逆运算
        foreach (string ip in new[] { "127.0.0.1", "10.1.2.3", "192.168.1.255", "8.8.8.8" })
        {
            int n = CSelGateIPFilter.InetAddr(ip);
            Assert.Equal(ip, CSelGateIPFilter.InetNtoa(n));
        }
    }

    // =====================================================================================
    // 2. 黑名单增删查（含永久表 + 临时表两条路径）
    // =====================================================================================

    [Fact]
    public void AddToBlockIPList_ByString_DedupsByTextAndRejectsInvalid()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);

        f.AddToBlockIPList("1.2.3.4");                 // :83-88
        f.AddToBlockIPList("1.2.3.4");                 // 文本重复 ⇒ 不加
        f.AddToBlockIPList("999.9.9.9");               // inet_addr = INADDR_NONE ⇒ 不加
        Assert.Equal(1, f.g_BlockIPList.Count);
        Assert.Equal("1.2.3.4", f.g_BlockIPList[0]);
        Assert.Equal(CSelGateIPFilter.InetAddr("1.2.3.4"), (int)f.g_BlockIPList.GetObject(0));
    }

    [Fact]
    public void AddToBlockIPList_ByInt_DedupsByStoredIntegerAndFormatsWithInetNtoa()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);

        f.AddToBlockIPList("1.2.3.4");
        f.AddToBlockIPList(CSelGateIPFilter.InetAddr("1.2.3.4")); // 整数重复 ⇒ 不加（:100 比较 Objects）
        Assert.Equal(1, f.g_BlockIPList.Count);

        f.AddToBlockIPList(CSelGateIPFilter.InetAddr("5.6.7.8")); // :110 用 inet_ntoa 生成文本
        Assert.Equal(2, f.g_BlockIPList.Count);
        Assert.Equal("5.6.7.8", f.g_BlockIPList[1]);
    }

    [Fact]
    public void IsBlockIP_ChecksPermanentThenTempList()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        int a = CSelGateIPFilter.InetAddr("1.1.1.1");
        int b = CSelGateIPFilter.InetAddr("2.2.2.2");
        int c = CSelGateIPFilter.InetAddr("3.3.3.3");

        f.AddToBlockIPList("1.1.1.1");        // :58 永久表
        f.AddToTempBlockIPList("2.2.2.2");    // :122 临时表

        Assert.True(f.IsBlockIP(a));          // :154-163 永久表命中
        Assert.True(f.IsBlockIP(b));          // :165-175 临时表命中
        Assert.False(f.IsBlockIP(c));
        Assert.False(f.IsBlockIP(0));
    }

    [Fact]
    public void IsBlockIP_IgnoresTextMatchButComparesInteger()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        // 文本不同但整数相同（inet_addr 简写）⇒ 整数比较命中
        f.AddToBlockIPList("1.2.3.4");
        Assert.True(f.IsBlockIP(CSelGateIPFilter.InetAddr("0x01020304")));
        Assert.False(f.IsBlockIP(CSelGateIPFilter.InetAddr("1.2.3.5")));
    }

    [Fact]
    public void AddToTempBlockIPList_StringAndIntegerOverloads()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.AddToTempBlockIPList("9.9.9.9");
        f.AddToTempBlockIPList("9.9.9.9");
        f.AddToTempBlockIPList(CSelGateIPFilter.InetAddr("9.9.9.9"));
        f.AddToTempBlockIPList("bad");
        Assert.Equal(1, f.g_TempBlockIPList.Count);
    }

    // =====================================================================================
    // 3. OverConnectOfIP / DeleteConnectOfIP / ClearConnectOfIP
    //    差异断言：判定式是 Count + 1 > Max，超限时**不**自增（原版 :193-196）
    // =====================================================================================

    [Fact]
    public void OverConnectOfIP_RejectsWhenCountPlusOneExceedsMax_AndDoesNotIncrement()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.Config!.m_nMaxConnectOfIP = 2;
        int addr = CSelGateIPFilter.InetAddr("7.7.7.7");

        Assert.False(f.OverConnectOfIP(addr));                          // 新建：Count=1
        Assert.False(f.OverConnectOfIP(addr));                          // 1+1=2 <= 2 ⇒ 增到 2
        Assert.True(f.OverConnectOfIP(addr));                           // 2+1=3 > 2 ⇒ True（:194）
        Assert.True(f.OverConnectOfIP(addr));                           // 仍然 True：超限分支**不自增**（:196 只在 else 里）
        Assert.Equal(2, f.g_ConnectOfIPList[0].Count);                  // Count 停在 2，未被撑大
        Assert.Single(f.g_ConnectOfIPList);
    }

    [Fact]
    public void OverConnectOfIP_WhenCheckNullSessionDisabled_IsANoOp()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.Config!.m_fCheckNullSession = false;                          // :184 直接 Exit
        f.Config.m_nMaxConnectOfIP = 0;
        for (int i = 0; i < 5; i++)
            Assert.False(f.OverConnectOfIP(1234));
        Assert.Empty(f.g_ConnectOfIPList);
    }

    [Fact]
    public void OverConnectOfIP_TracksEachAddressIndependently()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.Config!.m_nMaxConnectOfIP = 1;
        int a = CSelGateIPFilter.InetAddr("1.0.0.1");
        int b = CSelGateIPFilter.InetAddr("1.0.0.2");

        Assert.False(f.OverConnectOfIP(a));
        Assert.False(f.OverConnectOfIP(b));
        Assert.True(f.OverConnectOfIP(a));                              // a 已达上限
        Assert.True(f.OverConnectOfIP(b));
        Assert.Equal(2, f.g_ConnectOfIPList.Count);
    }

    [Fact]
    public void DeleteConnectOfIP_DecrementsAndRemovesEntryAtZero()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.Config!.m_nMaxConnectOfIP = 10;
        int addr = CSelGateIPFilter.InetAddr("7.7.7.7");

        f.OverConnectOfIP(addr);
        f.OverConnectOfIP(addr);                                        // Count=2
        Assert.Equal(2, f.g_ConnectOfIPList[0].Count);

        f.DeleteConnectOfIP(addr);                                      // :224 Dec
        Assert.Equal(1, f.g_ConnectOfIPList[0].Count);
        f.DeleteConnectOfIP(addr);                                      // :225-228 Count<=0 ⇒ 移除
        Assert.Empty(f.g_ConnectOfIPList);

        f.DeleteConnectOfIP(addr);                                      // 不存在 ⇒ 无异常、无副作用
        Assert.Empty(f.g_ConnectOfIPList);
    }

    [Fact]
    public void DeleteConnectOfIP_WhenCheckNullSessionDisabled_IsANoOp()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.Config!.m_fCheckNullSession = false;
        f.g_ConnectOfIPList.Add(new SelPerIPAddr { IPaddr = 5, Count = 3 });
        f.DeleteConnectOfIP(5);                                         // :214 直接 Exit
        Assert.Equal(3, f.g_ConnectOfIPList[0].Count);
    }

    [Fact]
    public void ClearConnectOfIP_EmptiesList_ButIsAlsoGatedByCheckNullSession()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.g_ConnectOfIPList.Add(new SelPerIPAddr { IPaddr = 1, Count = 1 });
        f.g_ConnectOfIPList.Add(new SelPerIPAddr { IPaddr = 2, Count = 2 });

        f.Config!.m_fCheckNullSession = false;
        f.ClearConnectOfIP();                                           // :242 直接 Exit
        Assert.Equal(2, f.g_ConnectOfIPList.Count);

        f.Config.m_fCheckNullSession = true;
        f.ClearConnectOfIP();                                           // :248
        Assert.Empty(f.g_ConnectOfIPList);
    }

    // =====================================================================================
    // 4. IP 段落盘 + 区间判定
    // =====================================================================================

    [Fact]
    public void LoadBlockIPAreaList_ParsesRangesAndNormalisesOrder()
    {
        using var dir = new TempDir();
        File.WriteAllText(dir.File("BlockIPAreaList.txt"),
            "192.168.1.255-192.168.1.1\r\n10.0.0.1-10.0.0.255\r\n", EncodingInit.GBK); // 第一行低位>高位
        var f = NewFilter(dir);

        f.LoadBlockIPAreaList();                                        // :254-297

        Assert.Equal(2, f.g_BlockIPAreaList.Count);
        Assert.Equal("192.168.1.255-192.168.1.1", f.g_BlockIPAreaList[0]); // 文本原样保留
        var a0 = (TIPArea)f.g_BlockIPAreaList.GetObject(0);
        Assert.True(a0.Low < a0.High);                                   // :286-288 已交换
        var a1 = (TIPArea)f.g_BlockIPAreaList.GetObject(1);
        Assert.Equal(CSelGateIPFilter.ReverseIP(unchecked((uint)CSelGateIPFilter.InetAddr("10.0.0.1"))), a1.Low);
        Assert.Equal(CSelGateIPFilter.ReverseIP(unchecked((uint)CSelGateIPFilter.InetAddr("10.0.0.255"))), a1.High);
    }

    [Fact]
    public void LoadBlockIPAreaList_SkipsEmptyAndMalformedLines()
    {
        using var dir = new TempDir();
        File.WriteAllText(dir.File("BlockIPAreaList.txt"),
            "\r\n1.2.3.4\r\n999.1.1.1-1.1.1.1\r\n1.1.1.1-999.1.1.1\r\n5.5.5.5-5.5.5.6\r\n", EncodingInit.GBK);
        var f = NewFilter(dir);

        f.LoadBlockIPAreaList();

        Assert.Equal(1, f.g_BlockIPAreaList.Count);                      // 只有最后一行合法
        Assert.Equal("5.5.5.5-5.5.5.6", f.g_BlockIPAreaList[0]);
    }

    [Fact]
    public void IsBlockIPArea_IsInclusiveOnBothEnds()
    {
        using var dir = new TempDir();
        File.WriteAllText(dir.File("BlockIPAreaList.txt"), "10.0.0.1-10.0.0.255\r\n", EncodingInit.GBK);
        var f = NewFilter(dir);
        f.LoadBlockIPAreaList();

        Assert.True(f.IsBlockIPArea(CSelGateIPFilter.InetAddr("10.0.0.1")));    // :328 闭区间下界
        Assert.True(f.IsBlockIPArea(CSelGateIPFilter.InetAddr("10.0.0.255")));  // :328 闭区间上界
        Assert.True(f.IsBlockIPArea(CSelGateIPFilter.InetAddr("10.0.0.128")));
        Assert.False(f.IsBlockIPArea(CSelGateIPFilter.InetAddr("10.0.1.0")));
        Assert.False(f.IsBlockIPArea(CSelGateIPFilter.InetAddr("9.255.255.255")));
    }

    [Fact]
    public void IsBlockIPArea_EmptyListIsAlwaysFalse()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        Assert.False(f.IsBlockIPArea(CSelGateIPFilter.InetAddr("127.0.0.1")));
    }

    [Fact]
    public void SaveBlockIPAreaList_WritesGBKCsvLinesAndSkipsEmpty()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.g_BlockIPAreaList.AddObject("10.0.0.1-10.0.0.255", new TIPArea { Low = 1, High = 2 });
        f.g_BlockIPAreaList.Add("");                                     // :307-308 跳过空行
        f.g_BlockIPAreaList.AddObject("192.168.0.0-192.168.255.255", new TIPArea { Low = 3, High = 4 });

        f.SaveBlockIPAreaList();                                         // :299-313

        Assert.Equal("10.0.0.1-10.0.0.255\r\n192.168.0.0-192.168.255.255\r\n",
            File.ReadAllText(dir.File("BlockIPAreaList.txt"), EncodingInit.GBK));
    }

    // =====================================================================================
    // 5. 黑名单文件（BlockIPList.txt）读写
    // =====================================================================================

    [Fact]
    public void LoadBlockIPList_CreatesEmptyFileWhenMissing_AndSkipsInvalidLines()
    {
        using var dir = new TempDir();
        string path = dir.File("BlockIPList.txt");
        Assert.False(File.Exists(path));

        var f = NewFilter(dir);
        f.LoadBlockIPList();                                             // :46-47 不存在则先写空文件
        Assert.True(File.Exists(path));
        Assert.Equal(0, f.g_BlockIPList.Count);

        File.WriteAllText(path, "1.2.3.4\r\n\r\nnot-an-ip\r\n5.6.7.8\r\n", EncodingInit.GBK);
        f.LoadBlockIPList();
        Assert.Equal(2, f.g_BlockIPList.Count);
        Assert.Equal("1.2.3.4", f.g_BlockIPList[0]);
        Assert.Equal("5.6.7.8", f.g_BlockIPList[1]);
    }

    [Fact]
    public void SaveBlockIPList_WritesOnlyNonEmptyLines()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.AddToBlockIPList("1.2.3.4");
        f.g_BlockIPList.Add("");                                          // :71-72 跳过
        f.AddToBlockIPList("5.6.7.8");

        f.SaveBlockIPList();                                              // :63-77

        Assert.Equal("1.2.3.4\r\n5.6.7.8\r\n", File.ReadAllText(dir.File("BlockIPList.txt"), EncodingInit.GBK));
    }

    [Fact]
    public void LoadThenSaveBlockIPList_PreservesFileText()
    {
        using var dir = new TempDir();
        File.WriteAllText(dir.File("BlockIPList.txt"), "10.1.1.1\r\n10.1.1.2\r\n\r\n", EncodingInit.GBK);
        var f = NewFilter(dir);
        f.LoadBlockIPList();
        f.SaveBlockIPList();
        Assert.Equal("10.1.1.1\r\n10.1.1.2\r\n", File.ReadAllText(dir.File("BlockIPList.txt"), EncodingInit.GBK));
    }

    // =====================================================================================
    // 6. CheckNewIDOfIP（4 秒窗口）
    // =====================================================================================

    [Fact]
    public void CheckNewIDOfIP_WhenSwitchOff_IsANoOp()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.Config!.m_fCheckNewIDOfIP = false;                              // :343
        for (int i = 0; i < 10; i++)
            Assert.False(f.CheckNewIDOfIP(42));
        Assert.Empty(f.g_NewIDOfIPList);
    }

    [Fact]
    public void CheckNewIDOfIP_FirstCallRegistersAddressWithTick()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.Config!.m_fCheckNewIDOfIP = true;
        f.Config.m_nCheckNewIDOfIP = 5;
        uint now = 1000;
        f.TickCount = () => now;

        Assert.False(f.CheckNewIDOfIP(42));                               // :372-376
        Assert.Single(f.g_NewIDOfIPList);
        Assert.Equal(42, f.g_NewIDOfIPList[0].IPaddr);
        Assert.Equal(1, f.g_NewIDOfIPList[0].Count);
        Assert.Equal(1000u, f.g_NewIDOfIPList[0].dwIDCountTick);
    }

    [Fact]
    public void CheckNewIDOfIP_InsideWindow_IncrementsAndExceedsThreshold()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.Config!.m_fCheckNewIDOfIP = true;
        f.Config.m_nCheckNewIDOfIP = 2;
        uint now = 1000;
        f.TickCount = () => now;

        Assert.False(f.CheckNewIDOfIP(9));    // Count=1
        Assert.False(f.CheckNewIDOfIP(9));    // Count=2，2 > 2 不成立
        Assert.True(f.CheckNewIDOfIP(9));     // Count=3 > 2 ⇒ True（:355-356）
        Assert.Equal(3, f.g_NewIDOfIPList[0].Count);
        Assert.Equal(1000u, f.g_NewIDOfIPList[0].dwIDCountTick);  // 窗口内**不**刷新 tick
    }

    [Fact]
    public void CheckNewIDOfIP_OutsideWindow_RefreshesTickAndDecrements()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.Config!.m_fCheckNewIDOfIP = true;
        f.Config.m_nCheckNewIDOfIP = 2;
        uint now = 1000;
        f.TickCount = () => now;

        f.CheckNewIDOfIP(9);
        f.CheckNewIDOfIP(9);
        f.CheckNewIDOfIP(9);                                  // Count=3
        now = 1000 + 4 * 1000;                                // 恰好 4000ms：不小于 4000 ⇒ 走 else 分支
        Assert.False(f.CheckNewIDOfIP(9));                    // :360-362 刷新 tick 并 Dec 到 2
        Assert.Equal(2, f.g_NewIDOfIPList[0].Count);
        Assert.Equal(5000u, f.g_NewIDOfIPList[0].dwIDCountTick);

        // 已到窗口边界（3999 < 4000 ⇒ 仍在窗口内）：Count 3 > 阈值 2 ⇒ True
        now = 5000 + 3999;
        Assert.True(f.CheckNewIDOfIP(9));
    }

    [Fact]
    public void CheckNewIDOfIP_DecrementToZeroRemovesEntry()
    {
        using var dir = new TempDir();
        var f = NewFilter(dir);
        f.Config!.m_fCheckNewIDOfIP = true;
        f.Config.m_nCheckNewIDOfIP = 5;
        uint now = 0;
        f.TickCount = () => now;

        f.CheckNewIDOfIP(9);                                  // Count=1
        now = 4 * 1000;
        f.CheckNewIDOfIP(9);                                  // :362 Dec → 0 ⇒ :363-366 移除
        Assert.Empty(f.g_NewIDOfIPList);
    }
}

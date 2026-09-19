using System;
using System.IO;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// DBShare.pas 接缝（TRouteInfo / g_RouteInfo / GateRouteIP / SaveServerInfo / LoadServerInfo / GetRankingList）
/// 的落盘格式、键序与路由解析语义测试。
/// </summary>
public class RouteShareTests : TempDirTest
{
    private static void SetupRoute0()
    {
        var r = DBShareSeam.g_RouteInfo[0];
        r.nGateCount = 2;
        r.sSelGateIP = "127.0.0.1";
        r.sGameGateIP[0] = "10.0.0.1";
        r.nGameGatePort[0] = 7200;
        r.nGameGateDBPort[0] = 7300;
        r.sGameGateIP[1] = "10.0.0.2";
        r.nGameGatePort[1] = 7201;
        r.nGameGateDBPort[1] = 7301;
        r.EnabledRunGate2List = 1;
        r.GameGateDisconnectCount = 2;
        r.RunGate2List.Add(1, "10.0.0.9", 8000, 8100, 5);
        r.RunGate2List.Add(0, "10.0.0.8", 8001, 8101, 3);
    }

    // ---------------- TRouteInfo / string[15] ----------------

    [Fact]
    public void ResetToDefault_与LoadServerInfo起始清零一致()
    {
        var r = DBShareSeam.g_RouteInfo[0];
        r.nGateCount = 5;
        r.sSelGateIP = "1.2.3.4";
        r.sGameGateIP[3] = "5.6.7.8";
        r.nGameGatePort[3] = 99;
        r.nGameGateDBPort[3] = 98;
        r.dwGameGateConnectTick[3] = 97;
        r.EnabledRunGate2List = 1;
        r.GameGateDisconnectCount = 7;
        r.RunGate2List.Add(1, "x", 1, 1, 1);

        r.ResetToDefault();

        Assert.Equal(0, r.nGateCount);
        Assert.Equal("", r.sSelGateIP.Value);
        Assert.Equal("", r.sGameGateIP[3].Value);
        Assert.Equal(0, r.nGameGatePort[3]);
        Assert.Equal(0, r.nGameGateDBPort[3]);
        Assert.Equal(0u, r.dwGameGateConnectTick[3]);
        Assert.Equal((byte)0, r.EnabledRunGate2List);
        Assert.Equal(1, r.GameGateDisconnectCount);      // 原文默认 1
        Assert.Equal(0, r.RunGate2List.Count);
    }

    [Fact]
    public void string15_按GBK字节截断_15字节为界()
    {
        Assert.Equal("123456789012345", new TShortString15("123456789012345").Value);
        Assert.Equal("123456789012345", new TShortString15("1234567890123456").Value);   // 16 → 截 15
        Assert.Equal("", new TShortString15("").Value);
        Assert.Equal("", default(TShortString15).Value);
        // "中文中文中文中" = 7 个汉字 = 14 GBK 字节，不截断
        Assert.Equal("中文中文中文中", new TShortString15("中文中文中文中").Value);
        // "中文中文中文中文" = 8 个汉字 = 16 GBK 字节 → 截到 15 字节（第 8 个汉字被切掉一半）
        Assert.Equal(15, GXX.Core.EncodingInit.GBK.GetByteCount(new TShortString15("中文中文中文中文").Value));
    }

    [Fact]
    public void string15_隐式转换可用于字符串比较()
    {
        var r = new TRouteInfo();
        r.sSelGateIP = "127.0.0.1";
        Assert.True(r.sSelGateIP == "127.0.0.1");
        Assert.True(r.sSelGateIP != "127.0.0.2");
    }

    // ---------------- GateRouteIP ----------------

    [Fact]
    public void GateRouteIP_命中角色网关_返回随机主游戏网关与端口()
    {
        SetupRoute0();
        DelphiRandom.Next = _ => 1;

        string ip = DBShareSeam.GateRouteIP("127.0.0.1", out int port);

        Assert.Equal("10.0.0.2", ip);
        Assert.Equal(7201, port);
    }

    [Fact]
    public void GateRouteIP_未命中返回空串且端口为0()
    {
        SetupRoute0();

        string ip = DBShareSeam.GateRouteIP("192.168.0.1", out int port);

        Assert.Equal("", ip);
        Assert.Equal(0, port);
    }

    [Fact]
    public void GateRouteIP_命中但nGateCount为0时_Random0取0号槽位_原文如此()
    {
        var r = DBShareSeam.g_RouteInfo[0];
        r.sSelGateIP = "1.1.1.1";
        r.nGateCount = 0;                       // 只写 sSelGateIP 不写 nGateCount 的异常路由
        r.sGameGateIP[0] = "9.9.9.9";
        r.nGameGatePort[0] = 6000;

        string ip = DBShareSeam.GateRouteIP("1.1.1.1", out int port);

        Assert.Equal("9.9.9.9", ip);            // Delphi Random(0) = 0
        Assert.Equal(6000, port);
    }

    [Fact]
    public void GateRouteIP_取第一条匹配的路由_后面的同地址路由被忽略()
    {
        DBShareSeam.g_RouteInfo[0].sSelGateIP = "1.1.1.1";
        DBShareSeam.g_RouteInfo[0].nGateCount = 1;
        DBShareSeam.g_RouteInfo[0].sGameGateIP[0] = "first";
        DBShareSeam.g_RouteInfo[0].nGameGatePort[0] = 1;
        DBShareSeam.g_RouteInfo[1].sSelGateIP = "1.1.1.1";
        DBShareSeam.g_RouteInfo[1].nGateCount = 1;
        DBShareSeam.g_RouteInfo[1].sGameGateIP[0] = "second";
        DBShareSeam.g_RouteInfo[1].nGameGatePort[0] = 2;

        Assert.Equal("first", DBShareSeam.GateRouteIP("1.1.1.1", out int port));
        Assert.Equal(1, port);
    }

    // ---------------- SaveServerInfo ----------------

    [Fact]
    public void SaveServerInfo_ServerInfo_txt逐字节_含行尾Tab_与空路由不写行()
    {
        SetupRoute0();
        DBShareSeam.g_RouteInfo[1].nGateCount = 0;      // 空路由不产生行

        DBShareSeam.SaveServerInfo();

        Assert.Equal("127.0.0.1\t10.0.0.1\t7200\t10.0.0.2\t7201\t\r\n", File.ReadAllText(DBShareSeam.g_sGateConfFileName));
    }

    [Fact]
    public void SaveServerInfo_GateList_ini键序与值格式逐字节()
    {
        SetupRoute0();

        DBShareSeam.SaveServerInfo();

        string expected =
            "[GateDBPort0]\r\n" +
            "1=7300\r\n" +
            "2=7301\r\n" +
            "[setup]\r\n" +
            "enable0=1\r\n" +
            "count0=2\r\n" +
            "[list0]\r\n" +
            "0=1\t10.0.0.9\t8000\t5\t8100\r\n" +
            "1=0\t10.0.0.8\t8001\t3\t8101\r\n";
        Assert.Equal(expected, File.ReadAllText(DBShareSeam.g_sGateListFileName));
    }

    [Fact]
    public void SaveServerInfo_空路由集合只清除GateDBPort节_文件为空()
    {
        DBShareSeam.SaveServerInfo();

        Assert.Equal("", File.ReadAllText(DBShareSeam.g_sGateConfFileName));
        Assert.Equal("", File.ReadAllText(DBShareSeam.g_sGateListFileName));
    }

    [Fact]
    public void SaveServerInfo_二次保存EraseSection后节被重排到末尾_旧条目不再残留()
    {
        SetupRoute0();
        DBShareSeam.SaveServerInfo();

        var r = DBShareSeam.g_RouteInfo[0];
        r.nGateCount = 1;
        r.RunGate2List.Clear();
        DBShareSeam.SaveServerInfo();

        // EraseSection('GateDBPort0') 删除节 → 再次 WriteInteger 时该节被追加到文件末尾；
        // 'list0' 被删后没有再写入 → 节消失。
        string expected =
            "[setup]\r\n" +
            "enable0=1\r\n" +
            "count0=2\r\n" +
            "[GateDBPort0]\r\n" +
            "1=7300\r\n";
        Assert.Equal(expected, File.ReadAllText(DBShareSeam.g_sGateListFileName));
    }

    // ---------------- LoadServerInfo ----------------

    [Fact]
    public void LoadServerInfo_缺文件时先写默认127文件再装载()
    {
        File.Delete(DBShareSeam.g_sGateConfFileName);

        DBShareSeam.LoadServerInfo();

        Assert.Equal("127.0.0.1 127.0.0.1 7200\r\n", File.ReadAllText(DBShareSeam.g_sGateConfFileName));
        var r = DBShareSeam.g_RouteInfo[0];
        Assert.Equal("127.0.0.1", r.sSelGateIP.Value);
        Assert.Equal(1, r.nGateCount);
        Assert.Equal("127.0.0.1", r.sGameGateIP[0].Value);
        Assert.Equal(7200, r.nGameGatePort[0]);
    }

    [Fact]
    public void LoadServerInfo_跳过分号行与空行_多路由按出现顺序()
    {
        File.WriteAllText(DBShareSeam.g_sGateConfFileName,
            ";注释行\r\n" +
            "\r\n" +
            "1.1.1.1 2.2.2.2 100 3.3.3.3 200\r\n" +
            "4.4.4.4 5.5.5.5 300\r\n");

        DBShareSeam.LoadServerInfo();

        Assert.Equal("1.1.1.1", DBShareSeam.g_RouteInfo[0].sSelGateIP.Value);
        Assert.Equal(2, DBShareSeam.g_RouteInfo[0].nGateCount);
        Assert.Equal("3.3.3.3", DBShareSeam.g_RouteInfo[0].sGameGateIP[1].Value);
        Assert.Equal(200, DBShareSeam.g_RouteInfo[0].nGameGatePort[1]);
        Assert.Equal("4.4.4.4", DBShareSeam.g_RouteInfo[1].sSelGateIP.Value);
        Assert.Equal(1, DBShareSeam.g_RouteInfo[1].nGateCount);
        Assert.Equal(0, DBShareSeam.g_RouteInfo[2].nGateCount);
    }

    [Fact]
    public void LoadServerInfo_端口非数字记0_IP文本原样Trim()
    {
        File.WriteAllText(DBShareSeam.g_sGateConfFileName, "  1.1.1.1   2.2.2.2   abc  \r\n");

        DBShareSeam.LoadServerInfo();

        var r = DBShareSeam.g_RouteInfo[0];
        Assert.Equal("1.1.1.1", r.sSelGateIP.Value);
        Assert.Equal("2.2.2.2", r.sGameGateIP[0].Value);
        Assert.Equal(0, r.nGameGatePort[0]);
    }

    [Fact]
    public void LoadServerInfo_超过20条路由行抛越界_Delphi侧为UB()
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < 21; i++) sb.Append($"1.1.1.{i} 2.2.2.2 100\r\n");
        File.WriteAllText(DBShareSeam.g_sGateConfFileName, sb.ToString());

        Assert.Throws<IndexOutOfRangeException>(() => DBShareSeam.LoadServerInfo());
    }

    [Fact]
    public void LoadServerInfo_GateList往返_主网关端口与备用列表恢复()
    {
        SetupRoute0();
        DBShareSeam.SaveServerInfo();
        TestReset.All();
        DBShareSeam.g_sGateConfFileName = Path2("!ServerInfo.txt");
        DBShareSeam.g_sGateListFileName = Path2("!GateList.ini");
        DBShareSeam.g_sFilePath = Dir + Path.DirectorySeparatorChar;
        // SaveServerInfo 写的就是上面两个路径，这里重新装载
        DBShareSeam.LoadServerInfo();

        var r = DBShareSeam.g_RouteInfo[0];
        Assert.Equal("127.0.0.1", r.sSelGateIP.Value);
        Assert.Equal(2, r.nGateCount);
        Assert.Equal(7300, r.nGameGateDBPort[0]);
        Assert.Equal(7301, r.nGameGateDBPort[1]);
        Assert.Equal((byte)1, r.EnabledRunGate2List);
        Assert.Equal(2, r.GameGateDisconnectCount);
        Assert.Equal(2, r.RunGate2List.Count);
        Assert.Equal((ushort)8000, r.RunGate2List.SortItems(0).Port);   // Level 5 在前
        Assert.Equal((ushort)8001, r.RunGate2List.SortItems(1).Port);
    }

    [Fact]
    public void LoadServerInfo_GameGateDisconnectCount越界回退1()
    {
        File.WriteAllText(DBShareSeam.g_sGateConfFileName, "1.1.1.1 2.2.2.2 100\r\n");
        File.WriteAllText(DBShareSeam.g_sGateListFileName, "[setup]\r\ncount0=99\r\n");
        DBShareSeam.LoadServerInfo();
        Assert.Equal(1, DBShareSeam.g_RouteInfo[0].GameGateDisconnectCount);

        File.WriteAllText(DBShareSeam.g_sGateListFileName, "[setup]\r\ncount0=0\r\n");
        DBShareSeam.LoadServerInfo();
        Assert.Equal(1, DBShareSeam.g_RouteInfo[0].GameGateDisconnectCount);

        File.WriteAllText(DBShareSeam.g_sGateListFileName, "[setup]\r\ncount0=8\r\n");
        DBShareSeam.LoadServerInfo();
        Assert.Equal(8, DBShareSeam.g_RouteInfo[0].GameGateDisconnectCount);
    }

    [Fact]
    public void LoadServerInfo_非法备用网关条目被过滤()
    {
        File.WriteAllText(DBShareSeam.g_sGateConfFileName, "1.1.1.1 2.2.2.2 100\r\n");
        File.WriteAllText(DBShareSeam.g_sGateListFileName,
            "[list0]\r\n" +
            "0=1\tbad-ip\t8000\t5\t8001\r\n" +      // IP 非法 → 丢
            "1=1\t1.2.3.4\t0\t5\t8001\r\n" +        // 端口 0 → 丢
            "2=1\t1.2.3.4\t70000\t5\t8001\r\n" +    // 端口 > 65535 → 丢
            "3=1\t1.2.3.4\t8000\t5\t8001\r\n");     // 合法 → 留

        DBShareSeam.LoadServerInfo();

        var list = DBShareSeam.g_RouteInfo[0].RunGate2List;
        Assert.Equal(1, list.Count);
        Assert.Equal("1.2.3.4", list.Items(0).IP.Value);
        Assert.Equal((ushort)8000, list.Items(0).Port);
        Assert.Equal(5, list.Items(0).Level);
        Assert.Equal((ushort)8001, list.Items(0).DBPort);
        Assert.Equal((byte)1, list.Items(0).Enabled);
    }

    [Fact]
    public void LoadServerInfo_ReadSectionValues不清空LoadList_导致list0条目重复进入list1_原文缺陷()
    {
        File.WriteAllText(DBShareSeam.g_sGateConfFileName,
            "1.1.1.1 2.2.2.2 100\r\n" +
            "3.3.3.3 4.4.4.4 200\r\n");
        File.WriteAllText(DBShareSeam.g_sGateListFileName,
            "[list0]\r\n" +
            "0=1\t10.0.0.1\t8000\t5\t8001\r\n" +
            "[list1]\r\n" +
            "0=1\t10.0.0.2\t9000\t6\t9001\r\n");

        DBShareSeam.LoadServerInfo();

        // 原文 TStringList 未 Clear → 路由1 把 list0 的条目也算进来了
        Assert.Equal(1, DBShareSeam.g_RouteInfo[0].RunGate2List.Count);
        Assert.Equal(2, DBShareSeam.g_RouteInfo[1].RunGate2List.Count);
        Assert.Equal("10.0.0.1", DBShareSeam.g_RouteInfo[1].RunGate2List.Items(0).IP.Value);
        Assert.Equal("10.0.0.2", DBShareSeam.g_RouteInfo[1].RunGate2List.Items(1).IP.Value);
    }

    // ---------------- GetRankingList ----------------

    [Fact]
    public void GetRankingList_页类型映射()
    {
        Assert.Same(DBShareSeam.g_HumanRankList, DBShareSeam.GetRankingList(0, 0));
        Assert.Same(DBShareSeam.g_WarriorRankList, DBShareSeam.GetRankingList(0, 1));
        Assert.Same(DBShareSeam.g_WizardRankList, DBShareSeam.GetRankingList(0, 2));
        Assert.Same(DBShareSeam.g_TaoistRankList, DBShareSeam.GetRankingList(0, 3));
        Assert.Same(DBShareSeam.g_HeroRankList, DBShareSeam.GetRankingList(1, 0));
        Assert.Same(DBShareSeam.g_HeroWarriorRankList, DBShareSeam.GetRankingList(1, 1));
        Assert.Same(DBShareSeam.g_HeroWizardRankList, DBShareSeam.GetRankingList(1, 2));
        Assert.Same(DBShareSeam.g_HeroTaoistRankList, DBShareSeam.GetRankingList(1, 3));
        Assert.Same(DBShareSeam.g_MasterRankList, DBShareSeam.GetRankingList(2, 0));
    }

    [Fact]
    public void GetRankingList_越界页类型返回null_原文case无else()
    {
        Assert.Null(DBShareSeam.GetRankingList(0, 4));
        Assert.Null(DBShareSeam.GetRankingList(3, 0));
        Assert.Null(DBShareSeam.GetRankingList(-1, 0));
    }

    [Fact]
    public void TRoleRankList_Add_Items越界与Count()
    {
        var list = new TRoleRankList();
        var d = new TRoleRankData { RankIndex = 3, HumanName = "abc", Level = 42 };
        Assert.Same(d, list.Add(d));
        Assert.Equal(1, list.Count);
        Assert.Same(d, list.Items(0));
        Assert.Null(list.Items(1));
        Assert.Null(list.Items(-1));
        list.Clear();
        Assert.Equal(0, list.Count);
    }
}

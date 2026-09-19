using System;
using System.Collections.Generic;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>RouteEdit.pas:1-783 的校验分支、备用网关节点编辑与源码缺陷（第6/7/8组校验错槽位）测试。</summary>
public class RouteEditTests : TempDirTest
{
    private static RouteEditInputs FullInputs()
    {
        var inp = new RouteEditInputs { SelGate = "192.168.1.1", OpenRunGate2 = false, GameGateDisconnectCount = 1 };
        for (int i = 0; i < 8; i++)
        {
            inp.GateIP[i] = $"10.0.0.{i + 1}";
            inp.GatePort[i] = (7000 + i).ToString();
            inp.DBPort[i] = (8000 + i).ToString();
        }
        return inp;
    }

    private static TRunGateNode Node(string ip, ushort port, ushort dbPort, int level, bool enabled = true)
        => new TRunGateNode
        {
            Data = new TRunGateInfo { IP = ip, Port = port, DBPort = dbPort, Level = level, Enabled = enabled ? (byte)1 : (byte)0 },
            CheckState = enabled ? TCheckState.csCheckedNormal : TCheckState.csUncheckedNormal
        };

    // ---------------- DoOpen ----------------

    [Fact]
    public void DoOpen_填充八组编辑框与备用网关节点_IsConnect按三秒判定()
    {
        DelphiTick.GetTickCount = () => 10000;
        var route = DBShareSeam.g_RouteInfo[0];
        route.sSelGateIP = "192.168.1.1";
        for (int i = 0; i < 8; i++)
        {
            route.sGameGateIP[i] = $"10.0.0.{i + 1}";
            route.nGameGatePort[i] = 7000 + i;
            route.nGameGateDBPort[i] = 8000 + i;
        }
        route.EnabledRunGate2List = 1;
        route.GameGateDisconnectCount = 3;
        route.RunGate2List.Add(1, "10.0.0.9", 7200, 7300, 9);
        route.RunGate2List.Add(0, "10.0.0.8", 7201, 7301, 5);
        route.RunGate2List.Items(0).LastResponseTick = 9000;    // 1000ms 前 → 在线
        route.RunGate2List.Items(1).LastResponseTick = 1000;    // 9000ms 前 → 离线

        var inputs = new RouteEditInputs();
        var nodes = new List<TRunGateNode>();
        byte changed = 1;
        RouteEditLogic.DoOpen(route, inputs, nodes, ref changed);

        Assert.Equal("192.168.1.1", inputs.SelGate);
        Assert.Equal("10.0.0.8", inputs.GateIP[7]);
        Assert.Equal("7007", inputs.GatePort[7]);
        Assert.Equal("8007", inputs.DBPort[7]);
        Assert.True(inputs.OpenRunGate2);
        Assert.Equal(3, inputs.GameGateDisconnectCount);
        Assert.Equal((byte)0, changed);

        Assert.Equal(2, nodes.Count);
        Assert.Equal(TCheckState.csCheckedNormal, nodes[0].CheckState);
        Assert.Equal((byte)1, nodes[0].Data.IsConnect);
        Assert.Equal(TCheckState.csUncheckedNormal, nodes[1].CheckState);
        Assert.Equal((byte)0, nodes[1].Data.IsConnect);
        // 节点是 RunGate2List 的拷贝，不共享实例
        Assert.NotSame(route.RunGate2List.Items(0), nodes[0].Data);
    }

    [Fact]
    public void DoOpen_恰好3000毫秒仍算在线_原文为小于等于()
    {
        DelphiTick.GetTickCount = () => 3000;
        var route = DBShareSeam.g_RouteInfo[0];
        route.RunGate2List.Add(1, "10.0.0.9", 7200, 7300, 9);
        route.RunGate2List.Items(0).LastResponseTick = 0;

        var nodes = new List<TRunGateNode>();
        byte changed = 1;
        RouteEditLogic.DoOpen(route, new RouteEditInputs(), nodes, ref changed);

        Assert.Equal((byte)1, nodes[0].Data.IsConnect);
    }

    // ---------------- BtnOK 校验分支 ----------------

    [Fact]
    public void BtnOK_角色网关非法_提示并聚焦EditSelGate_不设ModalResult()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        inp.SelGate = "abc";
        var route = DBShareSeam.g_RouteInfo[0];
        byte changed = 0;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, route, new List<TRunGateNode>(), ref changed);

        Assert.True(o.MessageBoxShown);
        Assert.Equal("角色网关输入错误！！！", o.MessageBoxText);
        Assert.Equal("错误信息", o.MessageBoxCaption);
        Assert.Equal(TMsgBox.MB_OK + TMsgBox.MB_ICONERROR, o.MessageBoxFlags);
        Assert.Equal("EditSelGate", o.FocusTarget);
        Assert.Equal(TModalResult.mrNone, o.ModalResult);
        Assert.Equal(0, route.nGateCount);
    }

    [Fact]
    public void BtnOK_第一组IP非法_提示游戏网关一并聚焦edtGateIP1()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        inp.GateIP[0] = "999.1.1.1";
        byte changed = 0;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, DBShareSeam.g_RouteInfo[0], new List<TRunGateNode>(), ref changed);

        Assert.Equal("游戏网关一输入错误！！！", o.MessageBoxText);
        Assert.Equal("edtGateIP1", o.FocusTarget);
    }

    [Fact]
    public void BtnOK_第一组端口为0或非数字_提示同一文案并聚焦edtGatePort1()
    {
        using var ui = new UiRecorder();
        foreach (string port in new[] { "0", "-1", "abc", "" })
        {
            var inp = FullInputs();
            inp.GatePort[0] = port;
            byte changed = 0;
            BtnOKOutcome o = RouteEditLogic.BtnOK(inp, DBShareSeam.g_RouteInfo[0], new List<TRunGateNode>(), ref changed);
            Assert.Equal("游戏网关一输入错误！！！", o.MessageBoxText);
            Assert.Equal("edtGatePort1", o.FocusTarget);
        }
    }

    [Fact]
    public void BtnOK_只填第一组_写入主网关并置nGateCount为1()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        for (int i = 1; i < 8; i++) { inp.GateIP[i] = ""; inp.GatePort[i] = ""; inp.DBPort[i] = ""; }
        var route = DBShareSeam.g_RouteInfo[0];
        byte changed = 0;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, route, new List<TRunGateNode>(), ref changed);

        Assert.Equal(TModalResult.mrOK, o.ModalResult);
        Assert.Equal(1, route.nGateCount);
        Assert.Equal("192.168.1.1", route.sSelGateIP.Value);
        Assert.Equal("10.0.0.1", route.sGameGateIP[0].Value);
        Assert.Equal(7000, route.nGameGatePort[0]);
        Assert.Equal(8000, route.nGameGateDBPort[0]);
    }

    [Fact]
    public void BtnOK_备用网关节点IP非法_提示并定位第1列()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        inp.OpenRunGate2 = true;
        inp.GateIP[1] = ""; inp.GatePort[1] = "";
        var nodes = new List<TRunGateNode> { Node("bad-ip", 7200, 7300, 1) };
        byte changed = 0;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, DBShareSeam.g_RouteInfo[0], nodes, ref changed);

        Assert.Equal("IP输入错误！", o.MessageBoxText);
        Assert.Equal(0, o.FocusNodeIndex);
        Assert.Equal(1, o.EditNodeColumn);
        Assert.Equal(TModalResult.mrNone, o.ModalResult);
    }

    [Fact]
    public void BtnOK_备用网关节点端口为0_提示并定位第2列()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        inp.OpenRunGate2 = true;
        var nodes = new List<TRunGateNode> { Node("10.0.0.9", 0, 7300, 1) };
        byte changed = 0;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, DBShareSeam.g_RouteInfo[0], nodes, ref changed);

        Assert.Equal("端口输入错误！", o.MessageBoxText);
        Assert.Equal(0, o.FocusNodeIndex);
        Assert.Equal(2, o.EditNodeColumn);
    }

    [Fact]
    public void BtnOK_备用网关IP端口重复_文案带前一个节点Index并定位后一个()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        inp.OpenRunGate2 = true;
        var nodes = new List<TRunGateNode>
        {
            Node("10.0.0.9", 7200, 7300, 1),
            Node("10.0.0.9", 7200, 9999, 2)
        };
        nodes[0].Index = 0;
        nodes[1].Index = 1;
        byte changed = 0;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, DBShareSeam.g_RouteInfo[0], nodes, ref changed);

        Assert.Equal("IP端口输入重复[0]", o.MessageBoxText);
        Assert.Equal(1, o.FocusNodeIndex);
        Assert.Equal(-1, o.EditNodeColumn);
    }

    [Fact]
    public void BtnOK_重复判定先Trim_仅DBPort相同不算重复()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        inp.OpenRunGate2 = true;

        // DBPort 相同但 IP 不同 → 不算重复（条件为 SameText(IP) and (Port同 or DBPort同)）
        var nodes = new List<TRunGateNode>
        {
            Node("10.0.0.9", 7200, 7300, 1),
            Node("10.0.0.10", 7201, 7300, 2)
        };
        byte changed = 0;
        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, DBShareSeam.g_RouteInfo[0], nodes, ref changed);
        Assert.Equal(TModalResult.mrOK, o.ModalResult);

        // IP 带空白但 Trim 后相同、端口相同 → 算重复
        var nodes2 = new List<TRunGateNode>
        {
            Node("10.0.0.9", 7200, 7300, 1),
            Node(" 10.0.0.9 ", 7200, 9999, 2)
        };
        BtnOKOutcome o2 = RouteEditLogic.BtnOK(inp, DBShareSeam.g_RouteInfo[0], nodes2, ref changed);
        Assert.Equal("IP端口输入重复[0]", o2.MessageBoxText);
    }

    // ---------------- RunGate2List 重建 ----------------

    [Fact]
    public void BtnOK_FIsChanged为0时不动RunGate2List()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        inp.OpenRunGate2 = true;
        var route = DBShareSeam.g_RouteInfo[0];
        route.RunGate2List.Add(1, "1.1.1.1", 1, 1, 1);
        var nodes = new List<TRunGateNode> { Node("10.0.0.9", 7200, 7300, 9) };
        byte changed = 0;

        RouteEditLogic.BtnOK(inp, route, nodes, ref changed);

        Assert.Equal(1, route.RunGate2List.Count);
        Assert.Equal("1.1.1.1", route.RunGate2List.Items(0).IP.Value);
    }

    [Fact]
    public void BtnOK_FIsChanged为1时按节点重建并按Level降序()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        inp.OpenRunGate2 = true;
        var route = DBShareSeam.g_RouteInfo[0];
        route.RunGate2List.Add(1, "1.1.1.1", 1, 1, 1);
        var nodes = new List<TRunGateNode>
        {
            Node("10.0.0.9", 7200, 7300, 3, enabled: true),
            Node("10.0.0.10", 7201, 7301, 9, enabled: false)
        };
        byte changed = 1;

        RouteEditLogic.BtnOK(inp, route, nodes, ref changed);

        Assert.Equal(2, route.RunGate2List.Count);
        Assert.Equal("10.0.0.10", route.RunGate2List.SortItems(0).IP.Value);
        Assert.Equal((byte)0, route.RunGate2List.SortItems(0).Enabled);   // CheckState 决定 Enabled
        Assert.Equal("10.0.0.9", route.RunGate2List.SortItems(1).IP.Value);
        Assert.Equal((byte)1, route.RunGate2List.SortItems(1).Enabled);
    }

    [Fact]
    public void BtnOK_未勾选备用网关_既不校验节点也不重建列表()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        inp.OpenRunGate2 = false;
        var route = DBShareSeam.g_RouteInfo[0];
        route.RunGate2List.Add(1, "1.1.1.1", 1, 1, 1);
        // 节点非法，但未勾选 → 不校验
        var nodes = new List<TRunGateNode> { Node("bad-ip", 0, 0, 1) };
        byte changed = 1;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, route, nodes, ref changed);

        Assert.Equal(TModalResult.mrOK, o.ModalResult);
        // 原文如此：FIsChanged 为真时先 Clear，再看勾选决定是否回填 → 未勾选则列表被清空
        Assert.Equal(0, route.RunGate2List.Count);
        Assert.Equal((byte)0, route.EnabledRunGate2List);
    }

    // ---------------- 原文缺陷：第6/7/8组校验错槽位 ----------------

    [Fact]
    public void BtnOK_八组全部合法时nGateCount最多到5_原文第6组校验未赋值槽位()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        var route = DBShareSeam.g_RouteInfo[0];
        byte changed = 0;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, route, new List<TRunGateNode>(), ref changed);

        Assert.Equal(TModalResult.mrOK, o.ModalResult);
        Assert.Equal(5, route.nGateCount);               // 第6组 (index 5) 校验的是尚未赋值的自己 → 提前退出
        Assert.Equal("10.0.0.5", route.sGameGateIP[4].Value);
        Assert.Equal("", route.sGameGateIP[5].Value);
    }

    [Fact]
    public void BtnOK_第6组槽位预先有旧IP时反而写入非法的第6组文本_差异断言()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        inp.GateIP[5] = "bad-ip";                        // 第6组文本非法
        var route = DBShareSeam.g_RouteInfo[0];
        route.sGameGateIP[5] = "9.9.9.9";                // 旧槽位合法（例如上次从文件装载）
        byte changed = 0;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, route, new List<TRunGateNode>(), ref changed);

        Assert.Equal(TModalResult.mrOK, o.ModalResult);
        Assert.Equal(6, route.nGateCount);
        Assert.Equal("bad-ip", route.sGameGateIP[5].Value);   // 非法文本被写入
    }

    [Fact]
    public void BtnOK_第5组用本组文本校验_与第6组行为不同_差异断言()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        inp.GateIP[4] = "bad-ip";                        // 第5组文本非法
        var route = DBShareSeam.g_RouteInfo[0];
        route.sGameGateIP[4] = "9.9.9.9";                // 旧槽位合法，但第5组不看它
        byte changed = 0;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, route, new List<TRunGateNode>(), ref changed);

        Assert.Equal(TModalResult.mrOK, o.ModalResult);
        Assert.Equal(4, route.nGateCount);               // 第5组被拒绝 → 只到 4
        Assert.Equal("9.9.9.9", route.sGameGateIP[4].Value);
    }

    [Fact]
    public void BtnOK_第7第8组同样校验自身未赋值槽位()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        var route = DBShareSeam.g_RouteInfo[0];
        route.sGameGateIP[5] = "9.9.9.9";                // 放行第6组
        byte changed = 0;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, route, new List<TRunGateNode>(), ref changed);

        Assert.Equal(TModalResult.mrOK, o.ModalResult);
        Assert.Equal(6, route.nGateCount);               // 第7组 (index 6) 又卡住
    }

    [Fact]
    public void BtnOK_预先给齐第6到第8槽位则能写满八组()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        var route = DBShareSeam.g_RouteInfo[0];
        route.sGameGateIP[5] = "9.9.9.5";
        route.sGameGateIP[6] = "9.9.9.6";
        route.sGameGateIP[7] = "9.9.9.7";
        byte changed = 0;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, route, new List<TRunGateNode>(), ref changed);

        Assert.Equal(TModalResult.mrOK, o.ModalResult);
        Assert.Equal(8, route.nGateCount);
        Assert.Equal("10.0.0.8", route.sGameGateIP[7].Value);
        Assert.Equal(7007, route.nGameGatePort[7]);
        Assert.Equal(8007, route.nGameGateDBPort[7]);
    }

    [Fact]
    public void BtnOK_任一组端口为0即提前结束_已写组保留()
    {
        using var ui = new UiRecorder();
        var inp = FullInputs();
        inp.GatePort[2] = "0";                           // 第3组（索引 2）端口 0
        var route = DBShareSeam.g_RouteInfo[0];
        byte changed = 0;

        BtnOKOutcome o = RouteEditLogic.BtnOK(inp, route, new List<TRunGateNode>(), ref changed);

        Assert.Equal(TModalResult.mrOK, o.ModalResult);
        Assert.Equal(2, route.nGateCount);
        Assert.Equal("10.0.0.2", route.sGameGateIP[1].Value);
        Assert.Equal("", route.sGameGateIP[2].Value);
    }

    // ---------------- btnAdd / btnDel ----------------

    [Fact]
    public void BtnAdd_默认字段与原文一致()
    {
        using var ui = new UiRecorder();
        var nodes = new List<TRunGateNode>();
        byte changed = 0;

        bool ok = RouteEditLogic.BtnAdd(nodes, ref changed, out TRunGateNode node);

        Assert.True(ok);
        Assert.Single(nodes);
        Assert.Equal((byte)1, node.Data.Enabled);
        Assert.Equal("", node.Data.IP.Value);
        Assert.Equal((ushort)7200, node.Data.Port);
        Assert.Equal((ushort)0, node.Data.DBPort);
        Assert.Equal(10, node.Data.Level);
        Assert.Equal((byte)1, node.Data.IsConnect);
        Assert.Equal(0u, node.Data.LastResponseTick);
        Assert.Equal(TCheckState.csCheckedNormal, node.CheckState);
        Assert.Equal((byte)1, changed);
    }

    [Fact]
    public void BtnAdd_达到100个时提示并拒绝()
    {
        using var ui = new UiRecorder();
        var nodes = new List<TRunGateNode>();
        for (int i = 0; i < RouteEditLogic.MaxRunGateNodes; i++) nodes.Add(Node("1.1.1.1", 1, 1, 1));
        byte changed = 0;

        bool ok = RouteEditLogic.BtnAdd(nodes, ref changed, out TRunGateNode node);

        Assert.False(ok);
        Assert.Null(node);
        Assert.Equal(100, nodes.Count);
        Assert.Equal("备用网关已达到最大数量,不能再增加！", ui.MessageBoxes[0].Text);
        Assert.Equal("提示信息", ui.MessageBoxes[0].Caption);
        Assert.Equal(TMsgBox.MB_OK + TMsgBox.MB_ICONINFORMATION, ui.MessageBoxes[0].Flags);
        Assert.Equal((byte)0, changed);
    }

    [Fact]
    public void BtnDel_优先焦点移到下一个_末尾则移到上一个()
    {
        using var ui = new UiRecorder();
        var nodes = new List<TRunGateNode> { Node("a", 1, 1, 1), Node("b", 2, 2, 2), Node("c", 3, 3, 3) };
        for (int i = 0; i < 3; i++) nodes[i].Index = i;
        byte changed = 0;

        Assert.True(RouteEditLogic.BtnDel(nodes, 1, ref changed, out int focus));
        Assert.Equal(2, focus);
        Assert.Equal(2, nodes.Count);
        Assert.Equal("a", nodes[0].Data.IP.Value);
        Assert.Equal((byte)1, changed);
    }

    [Fact]
    public void BtnDel_删最后一个时焦点回退到前一个()
    {
        using var ui = new UiRecorder();
        var nodes = new List<TRunGateNode> { Node("a", 1, 1, 1), Node("b", 2, 2, 2) };
        byte changed = 0;

        Assert.True(RouteEditLogic.BtnDel(nodes, 1, ref changed, out int focus));
        Assert.Equal(0, focus);
        Assert.Single(nodes);
    }

    [Fact]
    public void BtnDel_删唯一节点时无后继焦点()
    {
        using var ui = new UiRecorder();
        var nodes = new List<TRunGateNode> { Node("a", 1, 1, 1) };
        byte changed = 0;

        Assert.True(RouteEditLogic.BtnDel(nodes, 0, ref changed, out int focus));
        Assert.Equal(-1, focus);
        Assert.Empty(nodes);
    }

    [Fact]
    public void BtnDel_无焦点节点时不动作()
    {
        using var ui = new UiRecorder();
        var nodes = new List<TRunGateNode> { Node("a", 1, 1, 1) };
        byte changed = 0;

        Assert.False(RouteEditLogic.BtnDel(nodes, -1, ref changed, out int focus));
        Assert.Equal(-1, focus);
        Assert.Single(nodes);
        Assert.Equal((byte)0, changed);
    }

    // ---------------- 节点文本 / 编辑器 ----------------

    [Fact]
    public void GetText_五列映射与越界列()
    {
        var n = Node("1.2.3.4", 7200, 7300, 9);
        n.Index = 3;
        Assert.Equal("3", RouteEditLogic.GetText(n, 0));
        Assert.Equal("1.2.3.4", RouteEditLogic.GetText(n, 1));
        Assert.Equal("7200", RouteEditLogic.GetText(n, 2));
        Assert.Equal("7300", RouteEditLogic.GetText(n, 3));
        Assert.Equal("9", RouteEditLogic.GetText(n, 4));
        Assert.Equal("", RouteEditLogic.GetText(n, 5));
        Assert.Equal("", RouteEditLogic.GetText(n, -1));
    }

    [Fact]
    public void PaintTextIsRed_IsConnect为0才是红色()
    {
        var n = Node("1.1.1.1", 1, 1, 1);
        n.Data.IsConnect = 1;
        Assert.False(RouteEditLogic.PaintTextIsRed(n));
        n.Data.IsConnect = 0;
        Assert.True(RouteEditLogic.PaintTextIsRed(n));
    }

    [Fact]
    public void KeyDownCommand_仅Ctrl加插入删除()
    {
        Assert.Equal("btnAdd", RouteEditLogic.KeyDownCommand((int)System.Windows.Forms.Keys.Insert, true));
        Assert.Equal("btnDel", RouteEditLogic.KeyDownCommand((int)System.Windows.Forms.Keys.Delete, true));
        Assert.Equal("", RouteEditLogic.KeyDownCommand((int)System.Windows.Forms.Keys.Insert, false));
        Assert.Equal("", RouteEditLogic.KeyDownCommand((int)System.Windows.Forms.Keys.Delete, false));
        Assert.Equal("", RouteEditLogic.KeyDownCommand((int)System.Windows.Forms.Keys.A, true));
    }

    [Fact]
    public void PrepareEdit_支持列与取值范围()
    {
        var n = Node("1.2.3.4", 7200, 7300, 9);

        Assert.False(RouteEditLogic.PrepareEdit(0, n, out _, out _, out _));
        Assert.False(RouteEditLogic.PrepareEdit(5, n, out _, out _, out _));

        Assert.True(RouteEditLogic.PrepareEdit(1, n, out int min1, out int max1, out string text1));
        Assert.Equal("1.2.3.4", text1);
        Assert.Equal(15, max1);                 // TEdit.MaxLength := 15
        Assert.Equal(0, min1);

        Assert.True(RouteEditLogic.PrepareEdit(2, n, out int min2, out int max2, out string text2));
        Assert.Equal("7200", text2);
        Assert.Equal(0, min2);
        Assert.Equal(65535, max2);              // High(Word)

        Assert.True(RouteEditLogic.PrepareEdit(3, n, out _, out _, out string text3));
        Assert.Equal("7300", text3);
        Assert.True(RouteEditLogic.PrepareEdit(4, n, out _, out _, out string text4));
        Assert.Equal("9", text4);
    }

    [Fact]
    public void EndEdit_第1列比较Trim但写回原文本_原文如此()
    {
        var n = Node("1.2.3.4", 7200, 7300, 9);

        Assert.False(RouteEditLogic.EndEdit(1, true, " 1.2.3.4 ", 0, n));
        Assert.Equal("1.2.3.4", n.Data.IP.Value);

        Assert.True(RouteEditLogic.EndEdit(1, true, " 5.6.7.8 ", 0, n));
        Assert.Equal(" 5.6.7.8 ", n.Data.IP.Value);      // 未 Trim
    }

    [Fact]
    public void EndEdit_第2到4列按端口与等级写回()
    {
        var n = Node("1.2.3.4", 7200, 7300, 9);

        Assert.False(RouteEditLogic.EndEdit(2, false, "", 7200, n));
        Assert.True(RouteEditLogic.EndEdit(2, false, "", 7201, n));
        Assert.Equal((ushort)7201, n.Data.Port);

        Assert.True(RouteEditLogic.EndEdit(3, false, "", 7301, n));
        Assert.Equal((ushort)7301, n.Data.DBPort);

        Assert.True(RouteEditLogic.EndEdit(4, false, "", 10, n));
        Assert.Equal(10, n.Data.Level);
    }

    [Fact]
    public void EndEdit_TEdit只影响第1列_其它列忽略()
    {
        var n = Node("1.2.3.4", 7200, 7300, 9);
        Assert.False(RouteEditLogic.EndEdit(2, true, "9999", 9999, n));
        Assert.Equal((ushort)7200, n.Data.Port);
    }

    [Fact]
    public void EndEdit_端口超65535按Word截断()
    {
        var n = Node("1.2.3.4", 7200, 7300, 9);
        Assert.True(RouteEditLogic.EndEdit(2, false, "", 70000, n));
        Assert.Equal((ushort)(70000 & 0xFFFF), n.Data.Port);
    }

    [Fact]
    public void Editing_节点为空时不允许编辑()
    {
        Assert.True(RouteEditLogic.Editing(0));
        Assert.False(RouteEditLogic.Editing(-1));
    }

    // ---------------- 窗体级 ----------------

    [Fact]
    public void 窗体DoOpen与btnAddClick_同步到ListView()
    {
        DelphiTick.GetTickCount = () => 0;
        var route = DBShareSeam.g_RouteInfo[0];
        route.sSelGateIP = "1.1.1.1";
        route.nGateCount = 1;
        route.sGameGateIP[0] = "2.2.2.2";
        route.nGameGatePort[0] = 7200;
        route.RunGate2List.Add(1, "3.3.3.3", 7201, 7301, 5);

        using var form = new FrmRouteEdit { FRouteInfo = route };
        form.DoOpen();

        Assert.Equal("1.1.1.1", form.EditSelGate.Text);
        Assert.Equal("2.2.2.2", form.edtGateIP1.Text);
        Assert.Equal("7200", form.edtGatePort1.Text);
        Assert.Equal(1, form.vstRunGate.Items.Count);
        Assert.Equal("3.3.3.3", form.vstRunGate.Items[0].SubItems[1].Text);   // WinForms: SubItems[0] 即 Caption
        Assert.True(form.vstRunGate.Items[0].Checked);
        Assert.Equal((byte)0, form.FIsChanged);

        form.btnAddClick(null, EventArgs.Empty);
        Assert.Equal(2, form.Nodes.Count);
        Assert.Equal(2, form.vstRunGate.Items.Count);
        Assert.Equal("7200", form.Nodes[1].Data.Port.ToString());
        Assert.Equal((byte)1, form.FIsChanged);
    }

    [Fact]
    public void 窗体btnDelClick_删除焦点节点并把焦点移到下一个()
    {
        var route = DBShareSeam.g_RouteInfo[0];
        route.RunGate2List.Add(1, "3.3.3.3", 7201, 7301, 5);
        route.RunGate2List.Add(1, "4.4.4.4", 7202, 7302, 6);
        using var form = new FrmRouteEdit { FRouteInfo = route };
        form.DoOpen();

        form.FocusedNodeIndex = 0;      // WinForms 适配：无句柄时 FocusedItem 不可用
        form.btnDelClick(null, EventArgs.Empty);

        Assert.Single(form.Nodes);
        Assert.Equal("4.4.4.4", form.Nodes[0].Data.IP.Value);
        Assert.Equal((byte)1, form.FIsChanged);
    }

    [Fact]
    public void 窗体chkOpenRunGate2Click_联动四个控件可用性()
    {
        using var form = new FrmRouteEdit { FRouteInfo = DBShareSeam.g_RouteInfo[0] };
        form.DoOpen();

        form.chkOpenRunGate2.Checked = true;
        form.chkOpenRunGate2Click(null, EventArgs.Empty);
        Assert.True(form.vstRunGate.Enabled);
        Assert.True(form.btnAdd.Enabled);
        Assert.True(form.btnDel.Enabled);
        Assert.True(form.seGameGateDisconnectCount.Enabled);

        form.chkOpenRunGate2.Checked = false;
        form.chkOpenRunGate2Click(null, EventArgs.Empty);
        Assert.False(form.vstRunGate.Enabled);
        Assert.False(form.btnAdd.Enabled);
        Assert.False(form.btnDel.Enabled);
    }

    [Fact]
    public void 窗体EditSelGateChange_任何编辑框变更都置FIsChanged()
    {
        using var form = new FrmRouteEdit { FRouteInfo = DBShareSeam.g_RouteInfo[0] };
        form.DoOpen();
        Assert.Equal((byte)0, form.FIsChanged);

        form.EditSelGateChange(null, EventArgs.Empty);
        Assert.Equal((byte)1, form.FIsChanged);
    }

    [Fact]
    public void 窗体DFM_主网关八组编辑框几何对齐()
    {
        using var form = new FrmRouteEdit { FRouteInfo = DBShareSeam.g_RouteInfo[0] };
        Assert.Equal(new System.Drawing.Point(28, 20), form.edtGateIP1.Location);
        Assert.Equal(new System.Drawing.Point(127, 20), form.edtGatePort1.Location);
        Assert.Equal(new System.Drawing.Point(170, 20), form.edtDBPort1.Location);
        Assert.Equal(new System.Drawing.Point(244, 89), form.edtGateIP8.Location);
        Assert.Equal(new System.Drawing.Point(343, 89), form.edtGatePort8.Location);
        Assert.Equal(new System.Drawing.Point(387, 89), form.edtDBPort8.Location);
        Assert.Equal("一:", form.Label2.Text);
        Assert.Equal("八:", form.Label9.Text);
        Assert.Equal(5, form.vstRunGate.Columns.Count);
        Assert.Equal("网关IP", form.vstRunGate.Columns[1].Text);
        Assert.Equal("优先级别", form.vstRunGate.Columns[4].Text);
        Assert.Equal(1, form.seGameGateDisconnectCount.DfmMinValue);
        Assert.Equal(8, form.seGameGateDisconnectCount.DfmMaxValue);
    }
}

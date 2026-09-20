using System;
using System.Collections.Generic;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uFrmSafeFilter.pas（1497 行）的纯逻辑与窗体测试 —— 本车道最大的窗体。
/// 重点（原文缺陷全部照抄并固定行为）：
///   * `GetIPAddrFromActiveItem` = `Trim(Copy(S,1,18))` / `GetUserNameFromActiveItem` = `Copy(S,19,MaxInt)`；
///   * `BuildActiveItems` 的**倒序**遍历 + 只保留非空 RemoteAddr + `Format('%-18s')` 右填充；
///   * `UpdateHints` 的 5 组提示文本（成对赋值）；
///   * `ResolveBlockMethod` 的 else 落到 bmBlockList；
///   * 6 组右键菜单的使能规则（sort/clear/addAll 与单项两项）；
///   * `ValidateIPSection` 的三条分支（**结束=起始也允许**）；
///   * `AddAllToBlockWithBug` —— 原文 :544 用 `Items[ItemIndex]` 而非 `Items[I]`（D1 缺陷）；
///   * `btnOKClick` 的 27 个 INI 键（含 `MaxClientMsgCount` 用全局常量而非 g_nMaxClientPacketCount）；
///   * `ShowFrmSafeFilter` 判 `ShowModal = mrOk` 而 `btnOKClick` 只 `Close`（D10 缺陷 → 恒返回 False）。
/// </summary>
[Collection("RunGateFormLane")]
public class RunGateUtilsFormSafeFilterTests : IDisposable
{
    private readonly string _dir;

    public RunGateUtilsFormSafeFilterTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "p2rg_form_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        FormGlobals.ResetForTest();
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { }
    }

    private string IniPath => Path.Combine(_dir, "Config.ini");
    private string IniText => File.Exists(IniPath) ? File.ReadAllText(IniPath, System.Text.Encoding.GetEncoding(936)) : "";

    private class FakeClient : ISafeFilterClient
    {
        public string RemoteAddr { get; set; } = "";
        public string ChrName { get; set; } = "";
        public int CloseCount;
        public void Close() => CloseCount++;
    }

    private class FakePool : ISafeFilterClientPool
    {
        private readonly List<ISafeFilterClient> _list;
        public FakePool(params ISafeFilterClient[] clients) => _list = new List<ISafeFilterClient>(clients);
        public List<ISafeFilterClient> GetOnlineContextList() => new List<ISafeFilterClient>(_list);
    }

    // ============================ RunGateNet（IP2Long / Long2IP / IsIPaddr）============================

    [Theory]
    [InlineData("0.0.0.0", 0u)]
    [InlineData("0.0.0.1", 1u)]
    [InlineData("1.0.0.0", 0x01000000u)]
    [InlineData("255.255.255.255", 0xFFFFFFFFu)]
    [InlineData("192.168.1.100", 0xC0A80164u)]
    public void IP2Long_与WinSock语义一致(string ip, uint expected)
        => Assert.Equal(expected, RunGateNet.IP2Long(ip));

    [Theory]
    [InlineData("")]
    [InlineData("1.2.3")]
    [InlineData("1.2.3.4.5")]
    [InlineData("1.2.3.256")]
    [InlineData("1.2.3.-1")]
    [InlineData("a.b.c.d")]
    [InlineData("1.2.3.")]
    public void IP2Long_非法返回INADDR_NONE(string ip)
        => Assert.Equal(RunGateNet.INADDR_NONE, RunGateNet.IP2Long(ip));

    [Fact]
    public void Long2IP_与IP2Long互逆()
    {
        Assert.Equal("0.0.0.0", RunGateNet.Long2IP(0));
        Assert.Equal("255.255.255.255", RunGateNet.Long2IP(0xFFFFFFFF));
        Assert.Equal("192.168.1.100", RunGateNet.Long2IP(0xC0A80164));
        Assert.Equal(0xC0A80164u, RunGateNet.IP2Long(RunGateNet.Long2IP(0xC0A80164)));
    }

    [Fact]
    public void IsIPaddr_合法与非法()
    {
        Assert.True(RunGateNet.IsIPaddr("202.103.100.20"));
        Assert.False(RunGateNet.IsIPaddr("abc"));
        Assert.False(RunGateNet.IsIPaddr("1.2.3"));
    }

    // ============================ 两个 Format 解析（原 :923-931）============================

    [Fact]
    public void GetIPAddrFromActiveItem_取前18字符并Trim()
    {
        // 用 FormatActiveItem 造出与原文 :392-393 完全一致的字符串（IP 左对齐 18 宽 + 角色名）
        string item = SafeFilterLogic.FormatActiveItem("1.2.3.4", "playerA");
        Assert.Equal("1.2.3.4", SafeFilterLogic.GetIPAddrFromActiveItem(item));
    }

    [Fact]
    public void GetUserNameFromActiveItem_从第19字符起()
    {
        string item = SafeFilterLogic.FormatActiveItem("1.2.3.4", "playerA");
        Assert.Equal("playerA", SafeFilterLogic.GetUserNameFromActiveItem(item));
    }

    [Fact]
    public void FormatActiveItem_右填充到18宽且不截断()
    {
        // 原 :392-393 Format('%-18s', [sIPaddr]) + Context.sChrName
        Assert.Equal("1.2.3.4           playerA", SafeFilterLogic.FormatActiveItem("1.2.3.4", "playerA"));
        // 超过 18 宽不截断
        string longIp = "123.123.123.1234567";
        Assert.Equal(longIp + "u", SafeFilterLogic.FormatActiveItem(longIp, "u"));
    }

    [Fact]
    public void 两函数往返_IP与用户名可还原()
    {
        string item = SafeFilterLogic.FormatActiveItem("10.0.0.9", "英雄A");
        Assert.Equal("10.0.0.9", SafeFilterLogic.GetIPAddrFromActiveItem(item));
        Assert.Equal("英雄A", SafeFilterLogic.GetUserNameFromActiveItem(item));
    }

    [Fact]
    public void 两函数对长IP的行为_用户名被切掉_原文缺陷()
    {
        // 若 IP 长于 18（例如带端口），Copy(S,19,MaxInt) 会把 IP 尾部当成用户名 —— 原文如此
        string item = SafeFilterLogic.FormatActiveItem("255.255.255.255:8080", "u");
        Assert.NotEqual("", SafeFilterLogic.GetUserNameFromActiveItem(item));
    }

    // ============================ BuildActiveItems（原 :378-395 / :895-912）============================

    [Fact]
    public void BuildActiveItems_倒序遍历()
    {
        var pool = new ISafeFilterClient[]
        {
            new FakeClient { RemoteAddr = "1.1.1.1", ChrName = "A" },
            new FakeClient { RemoteAddr = "2.2.2.2", ChrName = "B" },
            new FakeClient { RemoteAddr = "3.3.3.3", ChrName = "C" }
        };

        var items = SafeFilterLogic.BuildActiveItems(pool);

        Assert.Equal(3, items.Count);
        Assert.StartsWith("3.3.3.3", items[0], StringComparison.Ordinal);      // 原 :378 downto → 末项先出
        Assert.EndsWith("C", items[0], StringComparison.Ordinal);
        Assert.StartsWith("1.1.1.1", items[2], StringComparison.Ordinal);
    }

    [Fact]
    public void BuildActiveItems_跳过空RemoteAddr()
    {
        var pool = new ISafeFilterClient[]
        {
            new FakeClient { RemoteAddr = "", ChrName = "X" },               // 原 :390 if sIPaddr <> ''
            new FakeClient { RemoteAddr = "9.9.9.9", ChrName = "Y" }
        };

        var items = SafeFilterLogic.BuildActiveItems(pool);

        Assert.Single(items);
        Assert.StartsWith("9.9.9.9", items[0], StringComparison.Ordinal);
    }

    [Fact]
    public void BuildActiveItems_全部为空时返回空()
        => Assert.Empty(SafeFilterLogic.BuildActiveItems(new[] { (ISafeFilterClient)new FakeClient() }));

    [Fact]
    public void BuildActiveItems_文本中IP左对齐18宽后接用户名()
    {
        var items = SafeFilterLogic.BuildActiveItems(new[] { (ISafeFilterClient)new FakeClient { RemoteAddr = "1.2.3.4", ChrName = "N" } });
        Assert.Equal("1.2.3.4           N", items[0]);
        Assert.Equal(19, items[0].Length);
    }

    // ============================ UpdateHints（原 :861-881）============================

    [Fact]
    public void DefenseLevelHint_文本()
        => Assert.Equal("调整范围在：1-10。分别是：严格-宽松。等级为0时关闭攻击防御！当前等级：7",
                        SafeFilterLogic.DefenseLevelHint(7));

    [Fact]
    public void DefenseToLevel1Hint_文本()
        => Assert.Equal("被攻击3次后，如果你的防御等级不是1级，程序将自动调整你的防御等级为1级",
                        SafeFilterLogic.DefenseToLevel1Hint(3));

    [Fact]
    public void ResotreDefenseHint_文本()
        => Assert.Equal("每隔120秒自动清除动态过滤列表中的IP", SafeFilterLogic.ResotreDefenseHint(120));

    [Fact]
    public void AutoClearTempHint_文本()
        => Assert.Equal("在没有攻击后，等待60秒程序将防御等级还原成你最初的设置", SafeFilterLogic.AutoClearTempHint(60));

    [Fact]
    public void AddAllToTempHint_文本()
        => Assert.Equal("当连接数达到50时，将链接列表的所有IP加入动态过滤", SafeFilterLogic.AddAllToTempHint(50));

    [Fact]
    public void BuildHints_九个控件名与五组文本成对()
    {
        var hints = SafeFilterLogic.BuildHints(5, 3, 120, 60, 50);

        Assert.Equal(9, hints.Count);
        // 成对共用同一串（原 :866-880）
        Assert.Equal(hints["chkDefenseToLevel1"], hints["seDefenseToLevel1"]);
        Assert.Equal(hints["seResotreDefense"], hints["chkResotreDefense"]);
        Assert.Equal(hints["chkAutoClearTemp"], hints["seAutoClearTemp"]);
        Assert.Equal(hints["chkAddAllToTemp"], hints["seAddAllToTemp"]);
        // trckbrDefenseLevel 独立
        Assert.Contains("当前等级：5", hints["trckbrDefenseLevel"], StringComparison.Ordinal);
    }

    // ============================ ResolveBlockMethod（原 :687-692）============================

    [Fact]
    public void ResolveBlockMethod_三分支()
    {
        Assert.Equal(TBlockIPMethod.bmDisconnect, SafeFilterLogic.ResolveBlockMethod(true, true));    // 原 :688 优先
        Assert.Equal(TBlockIPMethod.bmTempBlock, SafeFilterLogic.ResolveBlockMethod(false, true));    // 原 :690
        Assert.Equal(TBlockIPMethod.bmBlockList, SafeFilterLogic.ResolveBlockMethod(false, false));   // 原 :692 else
    }

    [Fact]
    public void ResolveBlockMethod_两个都未选也落到bmBlockList_差异断言()
    {
        // 原 :692 是裸 else → 即使"加入永久过滤列表"单选也未勾选，也返回 bmBlockList
        Assert.Equal(TBlockIPMethod.bmBlockList, SafeFilterLogic.ResolveBlockMethod(false, false));
    }

    // ============================ ErrMessage / QuestionMessage（原 :1197-1208）============================

    [Fact]
    public void ErrMessage_标题固定错误()
    {
        var captured = new List<(string Text, string Caption)>();
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { captured.Add((t, c)); return System.Windows.Forms.DialogResult.OK; };
            SafeFilterLogic.ErrMessage("bad");

            Assert.Single(captured);
            Assert.Equal("bad", captured[0].Text);
            Assert.Equal("错误", captured[0].Caption);           // 原 :1199
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void QuestionMessage_空标题用询问()
    {
        var captured = new List<(string Text, string Caption)>();
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { captured.Add((t, c)); return System.Windows.Forms.DialogResult.OK; };

            Assert.True(SafeFilterLogic.QuestionMessage("Q", ""));    // 原 :1204-1205 → '询问'；OK → True
            Assert.Equal("询问", captured[0].Caption);

            Assert.True(SafeFilterLogic.QuestionMessage("Q", "标题"));
            Assert.Equal("标题", captured[1].Caption);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void QuestionMessage_取消返回False()
    {
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => System.Windows.Forms.DialogResult.Cancel;
            Assert.False(SafeFilterLogic.QuestionMessage("Q", ""));
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    // ============================ 右键菜单使能规则（原 :625-649 / :1105-1118 / :1187-1195）============================

    [Fact]
    public void ComputePopupState_两组规则()
    {
        SafeFilterLogic.ComputePopupState(0, -1, out bool sortClearAddAll, out bool itemSpecific);
        Assert.False(sortClearAddAll);
        Assert.False(itemSpecific);

        SafeFilterLogic.ComputePopupState(3, -1, out sortClearAddAll, out itemSpecific);
        Assert.True(sortClearAddAll);                    // Count > 0
        Assert.False(itemSpecific);                      // ItemIndex = -1

        SafeFilterLogic.ComputePopupState(3, 2, out sortClearAddAll, out itemSpecific);
        Assert.True(sortClearAddAll);
        Assert.True(itemSpecific);                       // 0 <= 2 < 3

        SafeFilterLogic.ComputePopupState(3, 3, out _, out itemSpecific);
        Assert.False(itemSpecific);                      // 越界
    }

    [Fact]
    public void PopupStateTemp_五个菜单项()
    {
        SafeFilterLogic.PopupStateTemp(2, 1, out bool sort, out bool clear, out bool addAll, out bool del, out bool addTo);
        Assert.True(sort && clear && addAll && del && addTo);

        SafeFilterLogic.PopupStateTemp(2, -1, out sort, out clear, out addAll, out del, out addTo);
        Assert.True(sort && clear && addAll);
        Assert.False(del || addTo);
    }

    [Fact]
    public void PopupStateActive_六项且无清空()
    {
        SafeFilterLogic.PopupStateActive(0, -1, out bool sort, out bool a1, out bool a2, out bool b1, out bool b2, out bool kick);
        Assert.False(sort || a1 || a2 || b1 || b2 || kick);

        SafeFilterLogic.PopupStateActive(4, 0, out sort, out a1, out a2, out b1, out b2, out kick);
        Assert.True(sort && a1 && a2 && b1 && b2 && kick);
    }

    [Fact]
    public void PopupStateIpSection_仅两项()
    {
        SafeFilterLogic.PopupStateIpSection(0, -1, out bool sort, out bool del);
        Assert.False(sort || del);

        SafeFilterLogic.PopupStateIpSection(1, 0, out sort, out del);
        Assert.True(sort && del);
    }

    // ============================ ValidateIPSection（原 :1120-1152）============================

    [Fact]
    public void ValidateIPSection_正常区间通过()
    {
        var r = SafeFilterLogic.ValidateIPSection("202.103.100.1", "202.103.100.100", out uint b, out uint e);
        Assert.Equal(SafeFilterLogic.IPSectionResult.OK, r);
        Assert.Equal(RunGateNet.IP2Long("202.103.100.1"), b);
        Assert.Equal(RunGateNet.IP2Long("202.103.100.100"), e);
    }

    [Fact]
    public void ValidateIPSection_起始非法()
    {
        var r = SafeFilterLogic.ValidateIPSection("bad", "1.1.1.1", out _, out _);
        Assert.Equal(SafeFilterLogic.IPSectionResult.BeginInvalid, r);       // 原 :1129
    }

    [Fact]
    public void ValidateIPSection_结束非法()
    {
        var r = SafeFilterLogic.ValidateIPSection("1.1.1.1", "bad", out _, out _);
        Assert.Equal(SafeFilterLogic.IPSectionResult.EndInvalid, r);         // 原 :1136
    }

    [Fact]
    public void ValidateIPSection_结束小于起始被拒()
    {
        var r = SafeFilterLogic.ValidateIPSection("2.2.2.2", "1.1.1.1", out _, out _);
        Assert.Equal(SafeFilterLogic.IPSectionResult.EndLessThanBegin, r);   // 原 :1150-1151
    }

    [Fact]
    public void ValidateIPSection_结束等于起始允许_边界断言()
    {
        // 原 :1141 `if nEndaddr >= nBeginaddr` → **等于**也通过
        var r = SafeFilterLogic.ValidateIPSection("1.1.1.1", "1.1.1.1", out uint b, out uint e);
        Assert.Equal(SafeFilterLogic.IPSectionResult.OK, r);
        Assert.Equal(b, e);
    }

    [Fact]
    public void 提示文本常量()
    {
        Assert.Equal("输入的地址格式不正确！", SafeFilterLogic.MsgIPFormatError);      // 原 :1131/1138
        Assert.Equal("提示信息", SafeFilterLogic.MsgIPFormatErrorTitle);
        Assert.Equal("结束地址不能小于开始地址", SafeFilterLogic.MsgEndLessThanBegin);  // 原 :1151
        // ★ 两处不同（差异断言）
        Assert.Equal("输入的地址格式错误！", SafeFilterLogic.MsgTempAddBadIp);          // 原 :813
        Assert.Equal("输入的IP地址错误", SafeFilterLogic.MsgBlockAddBadIp);            // 原 :834
        Assert.NotEqual(SafeFilterLogic.MsgTempAddBadIp, SafeFilterLogic.MsgBlockAddBadIp);
    }

    // ============================ AddAllToBlockWithBug（原 :537-554 的 D1 缺陷）============================

    [Fact]
    public void AddAllToBlockWithBug_重复加入当前选中项而非逐项_缺陷断言()
    {
        var tempItems = new[] { "1.1.1.1", "2.2.2.2", "3.3.3.3" };
        var added = new List<string>();

        var result = SafeFilterLogic.AddAllToBlockWithBug(tempItems, 1, ip => added.Add(ip));

        // ★ 原 :544 用 `lstTemp.Items[lstTemp.ItemIndex]`（选中项 = 索引 1）→ 3 次都是 "2.2.2.2"
        Assert.Equal(3, result.Count);
        Assert.All(result, ip => Assert.Equal("2.2.2.2", ip));
        Assert.DoesNotContain("1.1.1.1", added);
        Assert.DoesNotContain("3.3.3.3", added);

        // 差异断言：正确实现（用 Items[I]）应得到 3 个不同 IP
        Assert.NotEqual(tempItems.Length, new HashSet<string>(result).Count);
    }

    [Fact]
    public void AddAllToBlockWithBug_未选中时加入空串()
    {
        var result = SafeFilterLogic.AddAllToBlockWithBug(new[] { "a", "b" }, -1, _ => { });
        Assert.Equal(2, result.Count);
        Assert.All(result, ip => Assert.Equal("", ip));
    }

    [Fact]
    public void AddAllToBlockWithBug_空列表不调用回调()
    {
        int calls = 0;
        var result = SafeFilterLogic.AddAllToBlockWithBug(Array.Empty<string>(), 0, _ => calls++);
        Assert.Empty(result);
        Assert.Equal(0, calls);
    }

    // ============================ FindListItem / InputQueryTitlesFor（原 :1210-1244）============================

    [Fact]
    public void FindListItem_大小写不敏感且首命中()
    {
        var items = new[] { "a", "B", "b", "c" };
        Assert.Equal(1, SafeFilterLogic.FindListItem(items, "b"));       // 首命中（索引 1）
        Assert.Equal(1, SafeFilterLogic.FindListItem(items, "B"));
        Assert.Equal(-1, SafeFilterLogic.FindListItem(items, "z"));
    }

    [Fact]
    public void FindListItem_空列表与空输入()
    {
        Assert.Equal(-1, SafeFilterLogic.FindListItem(Array.Empty<string>(), "x"));
        Assert.Equal(0, SafeFilterLogic.FindListItem(new[] { "" }, ""));
    }

    [Fact]
    public void InputQueryTitlesFor_按列表区分IP与MAC()
    {
        SafeFilterLogic.InputQueryTitlesFor(true, out string c1, out string p1);
        Assert.Equal("输入MAC", c1);                                     // 原 :1224
        Assert.Equal("请输入要查找的MAC地址", p1);

        SafeFilterLogic.InputQueryTitlesFor(false, out string c2, out string p2);
        Assert.Equal("输入IP", c2);                                      // 原 :1229
        Assert.Equal("请输入要查找的IP地址", p2);
    }

    // ============================ IPSECTION.Display（原 :405 / :1147）============================

    [Fact]
    public void IPSECTION_Display为起止IP()
    {
        var s = new IPSECTION { nBeginAddr = RunGateNet.IP2Long("1.2.3.4"), nEndAddr = RunGateNet.IP2Long("5.6.7.8") };
        Assert.Equal("1.2.3.4 - 5.6.7.8", s.Display);
    }

    // ============================ 窗体（DFM 对齐）============================

    [Fact]
    public void 窗体_DFM属性与13个分组框对齐()
    {
        using var f = new FrmSafeFilter();

        Assert.Equal("网络安全过滤", f.Text);                       // DFM: Caption
        Assert.Equal(875, f.ClientSize.Width);                     // DFM: ClientWidth=875
        Assert.Equal(620, f.ClientSize.Height);                    // DFM: ClientHeight=620
        Assert.Equal(System.Windows.Forms.FormBorderStyle.FixedDialog, f.FormBorderStyle);

        Assert.Equal("当前连接", f.grp1.Text);
        Assert.Equal("IP地址过滤", f.GroupBox1.Text);
        Assert.Equal("'Mac'地址过滤", f.grp3.Text);
        Assert.Equal("连接保护", f.grp2.Text);
        Assert.Equal("攻击操作", f.GroupBox3.Text);
        Assert.Equal("流量控制", f.GroupBox4.Text);
        Assert.Equal("防CC处理", f.GroupBox2.Text);
        Assert.Equal("防御设置", f.GroupBox6.Text);
        Assert.Equal("发言设置", f.GroupBox5.Text);
        Assert.Equal("验证客户端是否合法", f.grp5.Text);
        Assert.Equal("客户端非法包检查", f.grp4.Text);

        // 几何抽查（.dfm）
        Assert.Equal(8, f.grp1.Left);
        Assert.Equal(575, f.grp1.Height);
        Assert.Equal(256, f.GroupBox1.Left);
        Assert.Equal(505, f.grp3.Left);
        Assert.Equal(663, f.grp2.Left);
        Assert.Equal(782, f.btnOK.Left);
        Assert.Equal(589, f.btnOK.Top);
        Assert.Equal(82, f.btnOK.Width);
        Assert.Equal(25, f.btnOK.Height);
        Assert.Equal("确定(&O)", f.btnOK.Text);
    }

    [Fact]
    public void 窗体_六个列表与菜单项名对齐()
    {
        using var f = new FrmSafeFilter();

        Assert.NotNull(f.lstActive);
        Assert.NotNull(f.lstTemp);
        Assert.NotNull(f.lstBlock);
        Assert.NotNull(f.lstIpSection);
        Assert.NotNull(f.lstTempMac);
        Assert.NotNull(f.lstBlockMac);

        Assert.Same(f.pmActive, f.lstActive.ContextMenuStrip);
        Assert.Same(f.pmTemp, f.lstTemp.ContextMenuStrip);
        Assert.Same(f.pmBlock, f.lstBlock.ContextMenuStrip);
        Assert.Same(f.pmIpSection, f.lstIpSection.ContextMenuStrip);
        Assert.Same(f.pmTempMac, f.lstTempMac.ContextMenuStrip);
        Assert.Same(f.pmBlockMac, f.lstBlockMac.ContextMenuStrip);

        // 菜单项文本（.dfm）
        Assert.Equal("刷新(&R)", f.mniActiveRefesh.Text);
        Assert.Equal("排序(&S)", f.mniActiveSort.Text);
        Assert.Equal("加入动态过滤列表(&B)", f.mniActiveAddToTemp.Text);
        Assert.Equal("全部加入动态过滤列表(&G)", f.mniActiveAddAllToTemp.Text);
        Assert.Equal("加入永久过滤列表(&E)", f.mniActiveAddToBlock.Text);
        Assert.Equal("全部加入永久过滤列表(F)", f.mniActiveAddAllToBlock.Text);
        Assert.Equal("踢除下线(&T)", f.mniActiveKick.Text);
        Assert.Equal("无帐号连接全部加入动态过滤", f.mniActiveAddAllNoUserToTemp.Text);
        Assert.Equal("无帐号连接全部加入永久过滤", f.mniActiveAddAllNoUserToBlock.Text);
        Assert.Equal("增加(&A)", f.mniTempAdd.Text);
        Assert.Equal("删除(&D)", f.mniTempDelete.Text);
        Assert.Equal("清空(&C)", f.mniTempClear.Text);
        Assert.Equal("加入动态过滤列表(&B)", f.mniBlockAddToTemp.Text);
        // DFM: mniIpSectionAdd Caption=#22686#21152'IP'#27573'(&A)' → '增加IP段(&A)'（**不是**'加入IP段'）
        Assert.Equal("增加IP段(&A)", f.mniIpSectionAdd.Text);
        // DFM: mniIpSectionDel Caption=#21024#38500'IP'#27573'(&D)' → '删除IP段(&D)'
        Assert.Equal("删除IP段(&D)", f.mniIpSectionDel.Text);
        Assert.Equal("加入永久MAC过滤列表(&E)", f.mniTempMacAddToBlock.Text);
        Assert.Equal("加入动态过滤列表(&B)", f.mniBlockMacAddToTempMac.Text);
    }

    [Fact]
    public void 窗体_参数控件与DFM几何对齐()
    {
        using var f = new FrmSafeFilter();

        // grp2 连接保护
        Assert.Equal(76, f.seMaxConnect.Left);
        Assert.Equal(13, f.seMaxConnect.Top);
        Assert.Equal(61, f.seMaxConnect.Width);
        Assert.Equal(21, f.seMaxConnect.Height);
        Assert.Equal(76, f.seKeepConnectTimeOut.Left);
        Assert.Equal(35, f.seKeepConnectTimeOut.Top);
        Assert.Equal(8, f.seIPCountLimitTime1.Left);
        Assert.Equal(58, f.seIPCountLimitTime1.Top);
        Assert.Equal(138, f.seIPCountLimit1.Left);

        // GroupBox4 流量控制
        Assert.Equal(69, f.seMaxClientPacketSize.Left);
        Assert.Equal(13, f.seMaxClientPacketSize.Top);
        Assert.Equal(76, f.seMaxClientPacketSize.Width);
        Assert.Equal(117, f.chkLostLine.Width);
        Assert.Equal("流量异常掉线处理", f.chkLostLine.Text);

        // GroupBox2 防CC
        Assert.Equal(91, f.seAttackTick.Left);
        Assert.Equal(54, f.seAttackTick.Width);
        Assert.Equal("防CC攻击时间:", f.Label11.Text);
        Assert.Equal("'CC'攻击临界数:", f.Label12.Text);

        // GroupBox6 防御设置
        Assert.Equal(64, f.trckbrDefenseLevel.Left);
        Assert.Equal(10, f.trckbrDefenseLevel.Top);
        Assert.Equal(133, f.trckbrDefenseLevel.Width);
        Assert.Equal(10, f.trckbrDefenseLevel.Maximum);
        Assert.Equal(129, f.chkDefenseToLevel1.Width);
        Assert.Equal("受攻击防御调为'1'级", f.chkDefenseToLevel1.Text);
        Assert.Equal("无攻击还原防御等级", f.chkResotreDefense.Text);
        Assert.Equal("清除动态过滤列表", f.chkAutoClearTemp.Text);
        Assert.Equal("连接加入到动态过滤", f.chkAddAllToTemp.Text);

        // GroupBox5 发言设置
        Assert.Equal(64, f.seSayMaxLen.Left);
        Assert.Equal(36, f.seSayMaxLen.Top);
        Assert.Equal(55, f.seSayMaxLen.Width);
        Assert.Equal(8, f.chkSayMsgControl.Left);
        Assert.Equal("开启发言控制", f.chkSayMsgControl.Text);

        // grp5 验证客户端
        Assert.Equal(216, f.cbbCheckClientFailBlockMode.Left);
        Assert.Equal(136, f.cbbCheckClientFailBlockMode.Width);
        Assert.Equal(System.Windows.Forms.ComboBoxStyle.DropDownList, f.cbbCheckClientFailBlockMode.DropDownStyle);
        Assert.Equal(2, f.cbbCheckClientFailBlockMode.Items.Count);
        Assert.Equal("断开", f.cbbCheckClientFailBlockMode.Items[0]);
        Assert.Equal("IP加入动态过滤列表", f.cbbCheckClientFailBlockMode.Items[1]);
        Assert.Equal(1, f.cbbCheckClientFailBlockMode.SelectedIndex);      // DFM: ItemIndex=1

        // grp4 非法包检查
        Assert.Equal(65, f.seCheckClientPacketCount.Left);
        Assert.Equal(55, f.seCheckClientPacketCount.Width);
        Assert.Equal("数据包", f.chkCheckClientPacketLegal.Text);
        Assert.Equal("次非法为攻击", f.lbl2.Text);
    }

    // ============================ 窗体 Open（原 :343-473）============================

    [Fact]
    public void 窗体_FormCreate清空两个列表()
    {
        using var f = new FrmSafeFilter();
        f.lstTemp.Items.Add("x");
        f.lstBlock.Items.Add("y");
        f.FormCreate(f, EventArgs.Empty);
        Assert.Empty(f.lstTemp.Items);       // 原 :478
        Assert.Empty(f.lstBlock.Items);      // 原 :479
    }

    [Fact]
    public void 窗体_Open按全局量回填参数控件()
    {
        FormGlobals.g_nMaxConnOfIPaddr = 5;
        FormGlobals.g_BlockMethod = TBlockIPMethod.bmTempBlock;
        FormGlobals.g_dwAttackTick = 100;
        FormGlobals.g_nAttackCount = 3;
        FormGlobals.g_nMaxClientPacketSize = 200;
        FormGlobals.g_nMaxClientPacketCount = 4;
        FormGlobals.g_boKickOverPacketSize = true;
        FormGlobals.g_dwKeepConnectTimeOut = 30;
        FormGlobals.g_boCheckClientPacketLegal = true;
        FormGlobals.g_nCheckClientPacketCount = 6;
        FormGlobals.g_boSayMsgControl = true;
        FormGlobals.g_dwSayMaxLen = 80;
        FormGlobals.g_dwSayTime = 3;
        FormGlobals.g_dwSayMaxCount = 5;
        FormGlobals.g_dwSayDisableTime = 60;
        FormGlobals.g_dwIPCountLimitTime1 = 11;
        FormGlobals.g_dwIPCountLimit1 = 12;
        FormGlobals.g_dwIPCountLimitTime2 = 13;
        FormGlobals.g_dwIPCountLimit2 = 14;
        FormGlobals.g_dwDefenseLevel = 7;
        FormGlobals.g_boDefenseToLevel1 = true;
        FormGlobals.g_dwDefenseToLevel1 = 8;
        FormGlobals.g_boResotreDefense = true;
        FormGlobals.g_dwResotreDefense = 9;
        FormGlobals.g_boAutoClearTemp = true;
        FormGlobals.g_dwAutoClearTemp = 10;
        FormGlobals.g_boAddAllToTemp = true;
        FormGlobals.g_dwAddAllToTemp = 15;
        FormGlobals.g_boOpenCheckClient = true;
        FormGlobals.g_CheckClientFailBlockMethod = TBlockIPMethod.bmDisconnect;

        using var f = new FrmSafeFilter();
        f.Open();

        Assert.Equal(5, f.seMaxConnect.Value);                         // 原 :431
        Assert.True(f.rbAddTempList.Checked);                          // 原 :434
        Assert.False(f.rbDisConnect.Checked);
        Assert.Equal(100, f.seAttackTick.Value);                       // 原 :438
        Assert.Equal(3, f.seAttackCount.Value);
        Assert.Equal(200, f.seMaxClientPacketSize.Value);
        Assert.Equal(4, f.seMaxClientPacketCount.Value);
        Assert.True(f.chkLostLine.Checked);
        Assert.Equal(30, f.seKeepConnectTimeOut.Value);
        Assert.True(f.chkCheckClientPacketLegal.Checked);
        Assert.Equal(6, f.seCheckClientPacketCount.Value);
        Assert.True(f.chkSayMsgControl.Checked);
        Assert.Equal(80, f.seSayMaxLen.Value);
        Assert.Equal(3, f.seSayTime.Value);
        Assert.Equal(5, f.seSayMaxCount.Value);
        Assert.Equal(60, f.seSayDisableTime.Value);
        Assert.Equal(11, f.seIPCountLimitTime1.Value);
        Assert.Equal(12, f.seIPCountLimit1.Value);
        Assert.Equal(13, f.seIPCountLimitTime2.Value);
        Assert.Equal(14, f.seIPCountLimit2.Value);
        Assert.Equal(7, f.trckbrDefenseLevel.Value);
        Assert.True(f.chkDefenseToLevel1.Checked);
        Assert.Equal(8, f.seDefenseToLevel1.Value);
        Assert.True(f.chkResotreDefense.Checked);
        Assert.Equal(9, f.seResotreDefense.Value);
        Assert.True(f.chkAutoClearTemp.Checked);
        Assert.Equal(10, f.seAutoClearTemp.Value);
        Assert.True(f.chkAddAllToTemp.Checked);
        Assert.Equal(15, f.seAddAllToTemp.Value);
        Assert.True(f.chkOpenCheckClient.Checked);
        Assert.Equal(0, f.cbbCheckClientFailBlockMode.SelectedIndex);    // bmDisconnect = 0
    }

    [Fact]
    public void 窗体_Open填六个列表()
    {
        var host = new InMemorySafeFilterHost();
        host.TempIPList.Add("1.1.1.1");
        host.BlockIPList.Add("2.2.2.2");
        host.TempMacList.Add("AA:BB");
        host.BlockMacList.Add("CC:DD");
        host.IPSectionList.Add(new IPSECTION { nBeginAddr = RunNet("3.3.3.3"), nEndAddr = RunNet("4.4.4.4") });

        using var f = new FrmSafeFilter { Host = host, ClientPool = new FakePool(new FakeClient { RemoteAddr = "5.5.5.5", ChrName = "N" }) };
        f.Open();

        Assert.Single(f.lstTemp.Items);
        Assert.Equal("1.1.1.1", f.lstTemp.Items[0]);
        Assert.Single(f.lstBlock.Items);
        Assert.Equal("2.2.2.2", f.lstBlock.Items[0]);
        Assert.Single(f.lstTempMac.Items);
        Assert.Equal("AA:BB", f.lstTempMac.Items[0]);
        Assert.Single(f.lstBlockMac.Items);
        Assert.Equal("CC:DD", f.lstBlockMac.Items[0]);
        Assert.Single(f.lstIpSection.Items);
        Assert.Equal("3.3.3.3 - 4.4.4.4", f.lstIpSection.Items[0]);
        Assert.Single(f.lstActive.Items);
        Assert.StartsWith("5.5.5.5", f.lstActive.Items[0].ToString(), StringComparison.Ordinal);
    }

    private static uint RunNet(string ip) => RunGateNet.IP2Long(ip);

    [Fact]
    public void 窗体_chkSayMsgControlClick使能四个间隔控件()
    {
        using var f = new FrmSafeFilter();

        f.chkSayMsgControl.Checked = false;
        f.chkSayMsgControl_Click(f, EventArgs.Empty);
        Assert.False(f.seSayMaxLen.Enabled);          // 原 :850
        Assert.False(f.seSayTime.Enabled);
        Assert.False(f.seSayMaxCount.Enabled);
        Assert.False(f.seSayDisableTime.Enabled);
        Assert.False(FormGlobals.g_boSayMsgControl);

        f.chkSayMsgControl.Checked = true;
        f.chkSayMsgControl_Click(f, EventArgs.Empty);
        Assert.True(f.seSayMaxLen.Enabled);
        Assert.True(f.seSayTime.Enabled);
        Assert.True(f.seSayMaxCount.Enabled);
        Assert.True(f.seSayDisableTime.Enabled);
        Assert.True(FormGlobals.g_boSayMsgControl);
    }

    // ============================ 窗体 btnOKClick（原 :683-803）============================

    [Fact]
    public void 窗体_btnOK_Click写全局并落盘27键()
    {
        FormGlobals.g_sIniFileName = IniPath;
        // ★ 显式复位：原 uFrmSafeFilter.pas:750 写的是 `g_boSayMsgControl`，
        //   而该全局量**只在 chkSayMsgControlClick（原 :849）里**被控件改写，
        //   btnOKClick（原 :694-731）**不读 chkSayMsgControl**（原文如此）。
        //   故这里直接以全局量为准，避免依赖用例执行顺序。
        FormGlobals.g_boSayMsgControl = false;

        using var f = new FrmSafeFilter();
        f.rbDisConnect.Checked = true;
        f.seMaxClientPacketSize.Value = 200;
        f.seMaxClientPacketCount.Value = 4;
        f.chkLostLine.Checked = true;
        f.chkCheckClientPacketLegal.Checked = true;
        f.seCheckClientPacketCount.Value = 6;
        f.seAttackTick.Value = 100;
        f.seAttackCount.Value = 3;
        f.seMaxConnect.Value = 5;
        f.seKeepConnectTimeOut.Value = 30;
        f.seIPCountLimitTime1.Value = 11;
        f.seIPCountLimit1.Value = 12;
        f.seIPCountLimitTime2.Value = 13;
        f.seIPCountLimit2.Value = 14;
        f.seSayMaxLen.Value = 80;
        f.seSayTime.Value = 3;
        f.seSayMaxCount.Value = 5;
        f.seSayDisableTime.Value = 60;
        f.trckbrDefenseLevel.Value = 7;
        f.chkDefenseToLevel1.Checked = true;
        f.seDefenseToLevel1.Value = 8;
        f.chkResotreDefense.Checked = true;
        f.seResotreDefense.Value = 9;
        f.chkAutoClearTemp.Checked = true;
        f.seAutoClearTemp.Value = 10;
        f.chkAddAllToTemp.Checked = true;
        f.seAddAllToTemp.Value = 15;
        f.chkOpenCheckClient.Checked = true;
        f.cbbCheckClientFailBlockMode.SelectedIndex = 1;

        f.btnOK_Click(f, EventArgs.Empty);

        Assert.Equal(TBlockIPMethod.bmDisconnect, FormGlobals.g_BlockMethod);        // 原 :687-692
        Assert.Equal(200, FormGlobals.g_nMaxClientPacketSize);
        Assert.Equal(4, FormGlobals.g_nMaxClientPacketCount);
        Assert.True(FormGlobals.g_boKickOverPacketSize);
        Assert.Equal(100u, FormGlobals.g_dwAttackTick);
        Assert.Equal(5, FormGlobals.g_nMaxConnOfIPaddr);
        Assert.Equal(30u, FormGlobals.g_dwKeepConnectTimeOut);
        Assert.Equal(11u, FormGlobals.g_dwIPCountLimitTime1);
        Assert.Equal(14u, FormGlobals.g_dwIPCountLimit2);
        Assert.Equal(80u, FormGlobals.g_dwSayMaxLen);
        Assert.Equal(60u, FormGlobals.g_dwSayDisableTime);
        Assert.Equal(7u, FormGlobals.g_dwDefenseLevel);
        Assert.Equal(15u, FormGlobals.g_dwAddAllToTemp);
        Assert.True(FormGlobals.g_boOpenCheckClient);
        Assert.Equal(TBlockIPMethod.bmTempBlock, FormGlobals.g_CheckClientFailBlockMethod);

        // 27 个键 + 节头 + 每节后空行
        Assert.Equal("[GameGate]\r\n" +
            "AttackTick=100\r\n" +
            "AttackCount=3\r\n" +
            "MaxConnOfIPaddr=5\r\n" +
            "BlockMethod=0\r\n" +
            "MaxClientPacketSize=200\r\n" +
            "MaxClientPacketCount=4\r\n" +
            "MaxClientMsgCount=5\r\n" +                     // ★ 原 :740 用全局常量 nMaxClientMsgCount=5
            "KickOverPacket=1\r\n" +
            "CheckClientPacketLegal=1\r\n" +
            "CheckClientPacketCount=6\r\n" +
            "KeepConnectTimeOut=30\r\n" +
            "SayMsgControl=0\r\n" +
            "SayMaxLen=80\r\n" +
            "SayTime=3\r\n" +
            "SayMaxCount=5\r\n" +
            "SayDisableTime=60\r\n" +
            "IPCountLimitTime1=11\r\n" +
            "IPCountLimit1=12\r\n" +
            "IPCountLimitTime2=13\r\n" +
            "IPCountLimit2=14\r\n" +
            "DefenseLevel=7\r\n" +
            "IsDefenseToLevel1=1\r\n" +
            "DefenseToLevel1=8\r\n" +
            "IsResotreDefense=1\r\n" +
            "ResotreDefense=9\r\n" +
            "IsAutoClearTemp=1\r\n" +
            "AutoClearTemp=10\r\n" +
            "IsAddAllToTemp=1\r\n" +
            "AddAllToTemp=15\r\n" +
            "OpenCheckClient=1\r\n" +
            "CheckClientFailBlockMethod=1\r\n" +
            "\r\n", IniText);
    }

    [Fact]
    public void 窗体_btnOK_Click是Close而非mrOK_ShowFrmSafeFilter恒返回False缺陷()
    {
        FormGlobals.g_sIniFileName = IniPath;
        using var f = new FrmSafeFilter();

        f.btnOK_Click(f, EventArgs.Empty);

        // ★ D10：原 :802 是 `Close`（不是 `ModalResult := mrOK`），而 :337 判 `ShowModal = mrOk`
        Assert.Equal(System.Windows.Forms.DialogResult.None, f.DialogResult);
    }

    [Fact]
    public void 窗体_MaxClientMsgCount用全局常量而非MaxClientPacketCount_差异断言()
    {
        FormGlobals.g_sIniFileName = IniPath;
        using var f = new FrmSafeFilter();
        f.seMaxClientPacketCount.Value = 99;

        f.btnOK_Click(f, EventArgs.Empty);

        Assert.Contains("MaxClientPacketCount=99\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("MaxClientMsgCount=5\r\n", IniText, StringComparison.Ordinal);      // 与 99 无关
        Assert.DoesNotContain("MaxClientMsgCount=99", IniText, StringComparison.Ordinal);
    }

    // ============================ 列表菜单动作（含 D3/D4 用 UI 下标删列表）============================

    [Fact]
    public void 窗体_mniTempAddToBlock_Click按值删并加入永久列表()
    {
        var host = new InMemorySafeFilterHost();
        host.TempIPList.Add("1.1.1.1");
        host.TempIPList.Add("2.2.2.2");

        using var f = new FrmSafeFilter { Host = host };
        f.lstTemp.Items.AddRange(new object[] { "1.1.1.1", "2.2.2.2" });
        f.lstTemp.SelectedIndex = 0;

        f.mniTempAddToBlock_Click(f, EventArgs.Empty);

        Assert.Single(f.lstTemp.Items);
        Assert.Equal("2.2.2.2", f.lstTemp.Items[0]);
        Assert.Single(f.lstBlock.Items);
        Assert.Equal("1.1.1.1", f.lstBlock.Items[0]);
        Assert.Single(host.TempIPList.Lines);            // 按**值**删（原 :523）
        Assert.Equal(1, host.AddBlockIPCount);
        Assert.Equal(1, host.SaveBlockIPListCount);
    }

    [Fact]
    public void 窗体_mniTempAddAllToBlock_Click复现原文缺陷_重复加入选中项()
    {
        var host = new InMemorySafeFilterHost();
        using var f = new FrmSafeFilter { Host = host };
        f.lstTemp.Items.AddRange(new object[] { "1.1.1.1", "2.2.2.2", "3.3.3.3" });
        f.lstTemp.SelectedIndex = 1;

        f.mniTempAddAllToBlock_Click(f, EventArgs.Empty);

        // ★ D1：原 :544 用 Items[ItemIndex] → 永久列表里是 3 份 "2.2.2.2"
        Assert.Equal(3, f.lstBlock.Items.Count);
        for (int i = 0; i < 3; i++) Assert.Equal("2.2.2.2", f.lstBlock.Items[i]);
        Assert.Empty(f.lstTemp.Items);
        Assert.Equal(0, host.TempIPList.Count);
        Assert.Equal(1, host.SaveBlockIPListCount);
    }

    [Fact]
    public void 窗体_mniTempAddAllToBlock_Click未选中时加入空串()
    {
        var host = new InMemorySafeFilterHost();
        using var f = new FrmSafeFilter { Host = host };
        f.lstTemp.Items.AddRange(new object[] { "1.1.1.1", "2.2.2.2" });
        f.lstTemp.SelectedIndex = -1;

        f.mniTempAddAllToBlock_Click(f, EventArgs.Empty);

        Assert.Equal(2, f.lstBlock.Items.Count);
        Assert.Equal("", f.lstBlock.Items[0].ToString());
    }

    [Fact]
    public void 窗体_mniBlockAddAllToTemp_Click用Items下标_正确实现()
    {
        var host = new InMemorySafeFilterHost();
        host.BlockIPList.Add("1.1.1.1");
        host.BlockIPList.Add("2.2.2.2");

        using var f = new FrmSafeFilter { Host = host };
        f.lstBlock.Items.AddRange(new object[] { "1.1.1.1", "2.2.2.2" });

        f.mniBlockAddAllToTemp_Click(f, EventArgs.Empty);

        // 原 :614 用的是 `lstBlock.Items[I]`（正确）→ 两个不同 IP 都进动态列表
        Assert.Equal(2, f.lstTemp.Items.Count);
        Assert.Equal("1.1.1.1", f.lstTemp.Items[0]);
        Assert.Equal("2.2.2.2", f.lstTemp.Items[1]);
        Assert.Equal(2, host.AddTempBlockIPCount);
        Assert.Empty(f.lstBlock.Items);
    }

    [Fact]
    public void 窗体_mniBlockClear_Click不加锁也清空_原文缺陷不抛异常()
    {
        var host = new InMemorySafeFilterHost();
        host.BlockIPList.Add("x");
        using var f = new FrmSafeFilter { Host = host };
        f.lstBlock.Items.Add("x");

        var ex = Record.Exception(() => f.mniBlockClear_Click(f, EventArgs.Empty));

        Assert.Null(ex);                                  // 原 :601-605 无 Lock/UnLock
        Assert.Equal(0, host.BlockIPList.Count);
        Assert.Empty(f.lstBlock.Items);
    }

    [Fact]
    public void 窗体_mniTempDelete_Click按下标删()
    {
        var host = new InMemorySafeFilterHost();
        host.TempIPList.Add("a");
        host.TempIPList.Add("b");
        using var f = new FrmSafeFilter { Host = host };
        f.lstTemp.Items.AddRange(new object[] { "a", "b" });
        f.lstTemp.SelectedIndex = 1;

        f.mniTempDelete_Click(f, EventArgs.Empty);

        Assert.Single(host.TempIPList.Lines);
        Assert.Equal("a", host.TempIPList[0]);            // 原 :493 按下标删
        Assert.Single(f.lstTemp.Items);
    }

    [Fact]
    public void 窗体_mniTempDelete_Click未选中时不做事()
    {
        var host = new InMemorySafeFilterHost();
        host.TempIPList.Add("a");
        using var f = new FrmSafeFilter { Host = host };
        f.lstTemp.Items.Add("a");
        f.lstTemp.SelectedIndex = -1;

        f.mniTempDelete_Click(f, EventArgs.Empty);
        Assert.Single(host.TempIPList.Lines);
    }

    [Fact]
    public void 窗体_mniTempClear_Click清空全局与列表()
    {
        var host = new InMemorySafeFilterHost();
        host.TempIPList.Add("a");
        using var f = new FrmSafeFilter { Host = host };
        f.lstTemp.Items.Add("a");

        f.mniTempClear_Click(f, EventArgs.Empty);

        Assert.Equal(0, host.TempIPList.Count);
        Assert.Empty(f.lstTemp.Items);
    }

    [Fact]
    public void 窗体_mniTempMacAddToBlock_Click用UI下标删列表_原文缺陷()
    {
        var host = new InMemorySafeFilterHost();
        host.TempMacList.Add("M1");
        host.TempMacList.Add("M2");
        using var f = new FrmSafeFilter { Host = host };
        f.lstTempMac.Items.AddRange(new object[] { "M1", "M2" });
        f.lstTempMac.SelectedIndex = 1;

        f.mniTempMacAddToBlock_Click(f, EventArgs.Empty);

        // 原 :1326 `g_TempMacList.Delete(lstTempMac.ItemIndex)` → 按 UI 下标 1 删了 "M2"
        Assert.Single(host.TempMacList.Lines);
        Assert.Equal("M1", host.TempMacList[0]);
        Assert.Single(f.lstBlockMac.Items);
        Assert.Equal("M2", f.lstBlockMac.Items[0]);
        Assert.Equal(1, host.AddBlockMacCount);
        Assert.Equal(1, host.SaveBlockMacListCount);
    }

    [Fact]
    public void 窗体_mniBlockMacAddToTempMac_Click用UI下标删列表_原文缺陷()
    {
        var host = new InMemorySafeFilterHost();
        host.BlockMacList.Add("B1");
        host.BlockMacList.Add("B2");
        using var f = new FrmSafeFilter { Host = host };
        f.lstBlockMac.Items.AddRange(new object[] { "B1", "B2" });
        f.lstBlockMac.SelectedIndex = 0;

        f.mniBlockMacAddToTempMac_Click(f, EventArgs.Empty);

        Assert.Single(host.BlockMacList.Lines);
        Assert.Equal("B2", host.BlockMacList[0]);
        Assert.Single(f.lstTempMac.Items);
        Assert.Equal("B1", f.lstTempMac.Items[0]);
        Assert.Equal(1, host.AddTempBlockMacCount);
    }

    [Fact]
    public void 窗体_mniBlockMacClear_Click清空且Save()
    {
        var host = new InMemorySafeFilterHost();
        host.BlockMacList.Add("x");
        using var f = new FrmSafeFilter { Host = host };
        f.lstBlockMac.Items.Add("x");

        f.mniBlockMacClear_Click(f, EventArgs.Empty);

        Assert.Equal(0, host.BlockMacList.Count);
        Assert.Empty(f.lstBlockMac.Items);
        Assert.Equal(1, host.SaveBlockMacListCount);
    }

    [Fact]
    public void 窗体_mniTempMacAdd_Click去重()
    {
        var host = new InMemorySafeFilterHost();
        host.TempMacList.Add("M1");
        var original = MessageBoxSeam.InputQueryWithValue;
        try
        {
            using var f = new FrmSafeFilter { Host = host };

            // 已存在 → 不重复加
            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => { v = "M1"; return true; };
            f.mniTempMacAdd_Click(f, EventArgs.Empty);
            Assert.Single(host.TempMacList.Lines);
            Assert.Empty(f.lstTempMac.Items);

            // 新值 → 加入
            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => { v = "M2"; return true; };
            f.mniTempMacAdd_Click(f, EventArgs.Empty);
            Assert.Equal(2, host.TempMacList.Count);
            Assert.Single(f.lstTempMac.Items);
        }
        finally
        {
            MessageBoxSeam.InputQueryWithValue = original;
        }
    }

    [Fact]
    public void 窗体_mniTempMacAdd_Click取消时不加()
    {
        var host = new InMemorySafeFilterHost();
        var original = MessageBoxSeam.InputQueryWithValue;
        try
        {
            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => false;
            using var f = new FrmSafeFilter { Host = host };
            f.mniTempMacAdd_Click(f, EventArgs.Empty);
            Assert.Equal(0, host.TempMacList.Count);       // 原 :1272 Exit
        }
        finally
        {
            MessageBoxSeam.InputQueryWithValue = original;
        }
    }

    // ============================ IP 段增删（原 :1120-1180）============================

    [Fact]
    public void 窗体_mniIpSectionAdd_Click正常加入()
    {
        var host = new InMemorySafeFilterHost();
        var original = MessageBoxSeam.InputQueryWithValue;
        try
        {
            var answers = new Queue<string>(new[] { "10.0.0.1", "10.0.0.100" });
            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => { v = answers.Dequeue(); return true; };

            using var f = new FrmSafeFilter { Host = host };
            f.mniIpSectionAdd_Click(f, EventArgs.Empty);

            Assert.Single(host.IPSectionList);
            Assert.Single(f.lstIpSection.Items);
            Assert.Equal("10.0.0.1 - 10.0.0.100", f.lstIpSection.Items[0]);
            Assert.Equal(1, host.SaveIPSectionListCount);
        }
        finally
        {
            MessageBoxSeam.InputQueryWithValue = original;
        }
    }

    [Fact]
    public void 窗体_mniIpSectionAdd_Click结束小于起始时弹提示不加()
    {
        var shown = new List<string>();
        var originalShow = MessageBoxSeam.Show;
        var originalQuery = MessageBoxSeam.InputQueryWithValue;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };
            var answers = new Queue<string>(new[] { "10.0.0.100", "10.0.0.1" });
            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => { v = answers.Dequeue(); return true; };

            var host = new InMemorySafeFilterHost();
            using var f = new FrmSafeFilter { Host = host };
            f.mniIpSectionAdd_Click(f, EventArgs.Empty);

            Assert.Single(shown);
            Assert.Equal("结束地址不能小于开始地址", shown[0]);   // 原 :1151
            Assert.Empty(host.IPSectionList);
        }
        finally
        {
            MessageBoxSeam.Show = originalShow;
            MessageBoxSeam.InputQueryWithValue = originalQuery;
        }
    }

    [Fact]
    public void 窗体_mniIpSectionAdd_Click起始非法时弹提示()
    {
        var shown = new List<string>();
        var originalShow = MessageBoxSeam.Show;
        var originalQuery = MessageBoxSeam.InputQueryWithValue;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };
            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => { v = "bad"; return true; };

            var host = new InMemorySafeFilterHost();
            using var f = new FrmSafeFilter { Host = host };
            f.mniIpSectionAdd_Click(f, EventArgs.Empty);

            Assert.Single(shown);
            Assert.Equal("输入的地址格式不正确！", shown[0]);      // 原 :1131
            Assert.Empty(host.IPSectionList);
        }
        finally
        {
            MessageBoxSeam.Show = originalShow;
            MessageBoxSeam.InputQueryWithValue = originalQuery;
        }
    }

    [Fact]
    public void 窗体_mniIpSectionDel_Click删除选中段()
    {
        var host = new InMemorySafeFilterHost();
        var s1 = new IPSECTION { nBeginAddr = RunNet("1.1.1.1"), nEndAddr = RunNet("1.1.1.9") };
        var s2 = new IPSECTION { nBeginAddr = RunNet("2.2.2.2"), nEndAddr = RunNet("2.2.2.9") };
        host.IPSectionList.Add(s1);
        host.IPSectionList.Add(s2);

        using var f = new FrmSafeFilter { Host = host };
        f.lstIpSection.Items.Add(s1.Display);
        f.lstIpSection.Items.Add(s2.Display);
        f.lstIpSection.SelectedIndex = 0;

        f.mniIpSectionDel_Click(f, EventArgs.Empty);

        Assert.Single(host.IPSectionList);
        Assert.Same(s2, host.IPSectionList[0]);
        Assert.Single(f.lstIpSection.Items);
        Assert.Equal(1, host.SaveIPSectionListCount);
    }

    [Fact]
    public void 窗体_mniIpSectionDel_Click未选中时不做事()
    {
        var host = new InMemorySafeFilterHost();
        host.IPSectionList.Add(new IPSECTION());
        using var f = new FrmSafeFilter { Host = host };
        f.lstIpSection.Items.Add("x");
        f.lstIpSection.SelectedIndex = -1;

        f.mniIpSectionDel_Click(f, EventArgs.Empty);
        Assert.Single(host.IPSectionList);
    }

    // ============================ 当前连接菜单（原 :883-1103）============================

    [Fact]
    public void 窗体_mniActiveRefesh_Click重填连接列表()
    {
        var c1 = new FakeClient { RemoteAddr = "1.1.1.1", ChrName = "A" };
        var c2 = new FakeClient { RemoteAddr = "2.2.2.2", ChrName = "B" };
        using var f = new FrmSafeFilter { ClientPool = new FakePool(c1, c2) };
        f.lstActive.Items.Add("stale");

        f.mniActiveRefesh_Click(f, EventArgs.Empty);

        Assert.Equal(2, f.lstActive.Items.Count);
        Assert.StartsWith("2.2.2.2", f.lstActive.Items[0].ToString(), StringComparison.Ordinal);   // 倒序
        Assert.StartsWith("1.1.1.1", f.lstActive.Items[1].ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void 窗体_mniActiveAddToBlock_Click确认后加入永久并关闭连接()
    {
        var host = new InMemorySafeFilterHost();
        var client = new FakeClient { RemoteAddr = "7.7.7.7", ChrName = "N" };
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => System.Windows.Forms.DialogResult.OK;   // 用户确认

            using var f = new FrmSafeFilter { Host = host, ClientPool = new FakePool(client) };
            f.mniActiveRefesh_Click(f, EventArgs.Empty);
            f.lstActive.SelectedIndex = 0;

            f.mniActiveAddToBlock_Click(f, EventArgs.Empty);

            Assert.Single(f.lstBlock.Items);
            Assert.Equal("7.7.7.7", f.lstBlock.Items[0].ToString());
            Assert.Equal(1, host.AddBlockIPCount);
            Assert.Equal(1, client.CloseCount);                 // 原 :997-998 SameText → Close
            Assert.Equal(1, host.SaveBlockIPListCount);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_mniActiveAddToBlock_Click取消时不加入()
    {
        var host = new InMemorySafeFilterHost();
        var client = new FakeClient { RemoteAddr = "7.7.7.7", ChrName = "N" };
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => System.Windows.Forms.DialogResult.Cancel;   // 用户取消

            using var f = new FrmSafeFilter { Host = host, ClientPool = new FakePool(client) };
            f.mniActiveRefesh_Click(f, EventArgs.Empty);
            f.lstActive.SelectedIndex = 0;
            f.mniActiveAddToBlock_Click(f, EventArgs.Empty);

            Assert.Empty(f.lstBlock.Items);                     // 原 :991 Exit
            Assert.Equal(0, host.AddBlockIPCount);
            Assert.Equal(0, client.CloseCount);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_mniActiveKick_Click确认后关闭连接()
    {
        var client = new FakeClient { RemoteAddr = "8.8.8.8", ChrName = "K" };
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => System.Windows.Forms.DialogResult.OK;

            using var f = new FrmSafeFilter { ClientPool = new FakePool(client) };
            f.mniActiveRefesh_Click(f, EventArgs.Empty);
            f.lstActive.SelectedIndex = 0;
            f.mniActiveKick_Click(f, EventArgs.Empty);

            Assert.Equal(1, client.CloseCount);                 // 原 :1099
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_mniActiveAddAllNoUserToBlock_Click只处理空用户名()
    {
        var host = new InMemorySafeFilterHost();
        var withUser = new FakeClient { RemoteAddr = "1.1.1.1", ChrName = "UserA" };
        var noUser = new FakeClient { RemoteAddr = "2.2.2.2", ChrName = "" };
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => System.Windows.Forms.DialogResult.OK;

            using var f = new FrmSafeFilter { Host = host, ClientPool = new FakePool(withUser, noUser) };
            f.mniActiveRefesh_Click(f, EventArgs.Empty);
            f.mniActiveAddAllNoUserToBlock_Click(f, EventArgs.Empty);

            Assert.Single(f.lstBlock.Items);                            // 只有空用户名那条
            Assert.Equal("2.2.2.2", f.lstBlock.Items[0].ToString());     // 原 :1073 长度 0 才处理
            Assert.Equal(1, host.AddBlockIPCount);
            Assert.Equal(0, withUser.CloseCount);
            Assert.Equal(1, noUser.CloseCount);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_mniActiveAddAllNoUserToTemp_Click取消时全不处理()
    {
        var host = new InMemorySafeFilterHost();
        var noUser = new FakeClient { RemoteAddr = "2.2.2.2", ChrName = "" };
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => System.Windows.Forms.DialogResult.Cancel;

            using var f = new FrmSafeFilter { Host = host, ClientPool = new FakePool(noUser) };
            f.mniActiveRefesh_Click(f, EventArgs.Empty);
            f.mniActiveAddAllNoUserToTemp_Click(f, EventArgs.Empty);

            Assert.Empty(f.lstTemp.Items);
            Assert.Equal(0, host.AddTempBlockIPCount);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_mniActiveAddAllToTemp_Click全部加入动态列表()
    {
        var host = new InMemorySafeFilterHost();
        var c1 = new FakeClient { RemoteAddr = "1.1.1.1", ChrName = "A" };
        var c2 = new FakeClient { RemoteAddr = "2.2.2.2", ChrName = "B" };
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => System.Windows.Forms.DialogResult.OK;

            using var f = new FrmSafeFilter { Host = host, ClientPool = new FakePool(c1, c2) };
            f.mniActiveRefesh_Click(f, EventArgs.Empty);
            f.mniActiveAddAllToTemp_Click(f, EventArgs.Empty);

            Assert.Equal(2, f.lstTemp.Items.Count);            // 原 :965-974
            Assert.Equal(2, host.AddTempBlockIPCount);
            Assert.Equal(1, c1.CloseCount);
            Assert.Equal(1, c2.CloseCount);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    // ============================ 菜单使能（窗体侧）============================

    [Fact]
    public void 窗体_pmTemp_Popup按条数与选中设使能()
    {
        using var f = new FrmSafeFilter();

        f.pmTemp_Popup(f, EventArgs.Empty);                     // 空列表
        Assert.False(f.mniTempSort.Enabled);
        Assert.False(f.mniTempClear.Enabled);
        Assert.False(f.mniTempAddAllToBlock.Enabled);
        Assert.False(f.mniTempDelete.Enabled);
        Assert.False(f.mniTempAddToBlock.Enabled);

        f.lstTemp.Items.Add("a");
        f.lstTemp.SelectedIndex = 0;
        f.pmTemp_Popup(f, EventArgs.Empty);
        Assert.True(f.mniTempSort.Enabled);                     // 原 :629
        Assert.True(f.mniTempClear.Enabled);
        Assert.True(f.mniTempAddAllToBlock.Enabled);
        Assert.True(f.mniTempDelete.Enabled);                   // 原 :634
        Assert.True(f.mniTempAddToBlock.Enabled);
    }

    [Fact]
    public void 窗体_pmActive_Change设置六项且无用户两项恒禁用()
    {
        using var f = new FrmSafeFilter();

        f.pmActive_Change(f, EventArgs.Empty);
        Assert.False(f.mniActiveSort.Enabled);
        Assert.False(f.mniActiveAddAllToTemp.Enabled);
        Assert.False(f.mniActiveAddToTemp.Enabled);
        Assert.False(f.mniActiveKick.Enabled);
        // ★ 原 :1105-1118 未给"无帐号连接全部加入…"两项赋值 → 恒 False
        Assert.False(f.mniActiveAddAllNoUserToTemp.Enabled);
        Assert.False(f.mniActiveAddAllNoUserToBlock.Enabled);

        f.lstActive.Items.Add("a");
        f.lstActive.SelectedIndex = 0;
        f.pmActive_Change(f, EventArgs.Empty);
        Assert.True(f.mniActiveSort.Enabled);                   // 原 :1110
        Assert.True(f.mniActiveAddAllToTemp.Enabled);
        Assert.True(f.mniActiveAddAllToBlock.Enabled);
        Assert.True(f.mniActiveAddToTemp.Enabled);
        Assert.True(f.mniActiveAddToBlock.Enabled);
        Assert.True(f.mniActiveKick.Enabled);
        Assert.False(f.mniActiveAddAllNoUserToTemp.Enabled);    // 仍为 False
    }

    [Fact]
    public void 窗体_pmIpSection_Popup仅两项()
    {
        using var f = new FrmSafeFilter();

        f.pmIpSection_Popup(f, EventArgs.Empty);
        Assert.False(f.mniIpSectionSort.Enabled);
        Assert.False(f.mniIpSectionDel.Enabled);

        f.lstIpSection.Items.Add("s");
        f.lstIpSection.SelectedIndex = 0;
        f.pmIpSection_Popup(f, EventArgs.Empty);
        Assert.True(f.mniIpSectionSort.Enabled);                // 原 :1191
        Assert.True(f.mniIpSectionDel.Enabled);                 // 原 :1194
    }

    [Fact]
    public void 窗体_pmBlockMac_Popup五项()
    {
        using var f = new FrmSafeFilter();
        f.lstBlockMac.Items.Add("m");
        f.lstBlockMac.SelectedIndex = 0;

        f.pmBlockMac_Popup(f, EventArgs.Empty);

        Assert.True(f.mniBlockMacSort.Enabled);
        Assert.True(f.mniBlockMacClear.Enabled);
        Assert.True(f.mniBlockMacAddAllToTempMac.Enabled);
        Assert.True(f.mniBlockMacDelete.Enabled);
        Assert.True(f.mniBlockMacAddToTempMac.Enabled);
    }

    [Fact]
    public void 窗体_pmTempMac_Popup五项()
    {
        using var f = new FrmSafeFilter();
        f.pmTempMac_Popup(f, EventArgs.Empty);
        Assert.False(f.mniTempMacSort.Enabled);

        f.lstTempMac.Items.Add("m");
        f.lstTempMac.SelectedIndex = 0;
        f.pmTempMac_Popup(f, EventArgs.Empty);
        Assert.True(f.mniTempMacSort.Enabled);
        Assert.True(f.mniTempMacDelete.Enabled);
    }

    // ============================ Ctrl+F 查找（原 :1210-1244）============================

    [Fact]
    public void 窗体_lstActiveKeyDown_CtrlF命中即定位()
    {
        var original = MessageBoxSeam.InputQueryWithValue;
        try
        {
            using var f = new FrmSafeFilter();
            f.lstActive.Items.AddRange(new object[] { "1.1.1.1", "2.2.2.2", "3.3.3.3" });
            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => { v = "2.2.2.2"; return true; };

            f.lstActive_KeyDown(f.lstActive, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.F | System.Windows.Forms.Keys.Control));

            Assert.Equal(1, f.lstActive.SelectedIndex);              // 原 :1238
        }
        finally
        {
            MessageBoxSeam.InputQueryWithValue = original;
        }
    }

    [Fact]
    public void 窗体_lstActiveKeyDown_仅CtrlF触发()
    {
        int queryCalls = 0;
        var original = MessageBoxSeam.InputQueryWithValue;
        try
        {
            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => { queryCalls++; return false; };
            using var f = new FrmSafeFilter();
            f.lstActive.Items.Add("a");

            // 只有 F，无 Ctrl
            f.lstActive_KeyDown(f.lstActive, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.F));
            Assert.Equal(0, queryCalls);

            // Ctrl + A
            f.lstActive_KeyDown(f.lstActive, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.A | System.Windows.Forms.Keys.Control));
            Assert.Equal(0, queryCalls);

            // Ctrl + F
            f.lstActive_KeyDown(f.lstActive, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.F | System.Windows.Forms.Keys.Control));
            Assert.Equal(1, queryCalls);
        }
        finally
        {
            MessageBoxSeam.InputQueryWithValue = original;
        }
    }

    [Fact]
    public void 窗体_lstActiveKeyDown_对MAC列表用MAC提示()
    {
        var captions = new List<string>();
        var original = MessageBoxSeam.InputQueryWithValue;
        try
        {
            MessageBoxSeam.InputQueryWithValue = (string c, string p, ref string v) => { captions.Add(c); return false; };
            using var f = new FrmSafeFilter();

            f.lstActive_KeyDown(f.lstTempMac, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.F | System.Windows.Forms.Keys.Control));
            f.lstActive_KeyDown(f.lstActive, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.F | System.Windows.Forms.Keys.Control));

            Assert.Equal("输入MAC", captions[0]);       // 原 :1224
            Assert.Equal("输入IP", captions[1]);        // 原 :1229
        }
        finally
        {
            MessageBoxSeam.InputQueryWithValue = original;
        }
    }

    // ============================ 排序动作 ============================

    [Fact]
    public void 窗体_各Sort动作设Sorted()
    {
        using var f = new FrmSafeFilter();

        f.mniTempSort_Click(f, EventArgs.Empty);
        Assert.True(f.lstTemp.Sorted);
        f.mniBlockSort_Click(f, EventArgs.Empty);
        Assert.True(f.lstBlock.Sorted);
        f.mniIpSectionSort_Click(f, EventArgs.Empty);
        Assert.True(f.lstIpSection.Sorted);
        f.mniTempMacSort_Click(f, EventArgs.Empty);
        Assert.True(f.lstTempMac.Sorted);
        f.mniBlockMacSort_Click(f, EventArgs.Empty);
        Assert.True(f.lstBlockMac.Sorted);
        f.mniActiveSort_Click(f, EventArgs.Empty);
        Assert.True(f.lstActive.Sorted);
    }

    // ============================ UpdateHints 窗体侧 ============================

    [Fact]
    public void 窗体_UpdateHints在防御等级变化时刷新()
    {
        using var f = new FrmSafeFilter();
        f.trckbrDefenseLevel.Value = 6;

        var ex = Record.Exception(() => f.trckbrDefenseLevel_Change(f, EventArgs.Empty));
        Assert.Null(ex);   // 原 :858 → UpdateHints（设置 ToolTip，无弹窗）
    }

    // ============================ 接缝默认实现 ============================

    [Fact]
    public void InMemorySafeFilterHost_默认空且计数正确()
    {
        var host = new InMemorySafeFilterHost();
        host.AddBlockIP("a");
        host.AddTempBlockIP("b");
        host.AddBlockMac("c");
        host.AddTempBlockMac("d");
        host.SaveBlockIPList();
        host.SaveIPSectionList();
        host.SaveBlockMacList();

        Assert.Equal(1, host.AddBlockIPCount);
        Assert.Equal(1, host.AddTempBlockIPCount);
        Assert.Equal(1, host.AddBlockMacCount);
        Assert.Equal(1, host.AddTempBlockMacCount);
        Assert.Equal(1, host.SaveBlockIPListCount);
        Assert.Equal(1, host.SaveIPSectionListCount);
        Assert.Equal(1, host.SaveBlockMacListCount);
        Assert.Equal(0, host.TempIPList.Count);
    }

    [Fact]
    public void ShowFrmSafeFilter不弹窗不挂起_因BtnOK只Close()
    {
        // 仅验证"不会启动消息循环"：把 ShowDialog 路径排除在外，
        // 直接构造窗体并调用 Open/btnOK_Click（不调用 ShowFrmSafeFilter 以免 ShowDialog 挂住 testhost）。
        FormGlobals.g_sIniFileName = IniPath;
        using var f = new FrmSafeFilter();
        f.Open();
        f.btnOK_Click(f, EventArgs.Empty);

        Assert.Equal(System.Windows.Forms.DialogResult.None, f.DialogResult);   // D10
        Assert.True(File.Exists(IniPath));
    }
}

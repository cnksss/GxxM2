using System;
using System.Collections.Generic;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uFrmGameSpeed.pas（1333 行）的纯逻辑与窗体测试 —— 本车道最大的窗体。
/// 重点：
///   * 列 0..8 的取文本规则（`DescribeCell`）：并发行的 ' &gt;' 后缀、18 种模式的 '设置'、
///     列 5/8 的 ' '、列 7 仅 3 个模式显示补偿值；
///   * 列 2 的**三套下拉**（`BuildProcessModeItems`）：apmRebound 起 / apmOffline 起（Names2）/ 跳 apmFakeAttackPass
///     且 `apmNoProcess` 的 ItemIndex 要 `Ord - 1` —— 全窗体最易错的一段；
///   * 编辑许可（`IsEditable`）、勾选许可（`CanToggleCheck`）、强制勾选（`ShouldBeChecked`）；
///   * `btnSaveClick` 的两个 TrackBar 夹取到 3 再判 `speedValue >= collectCount`；
///   * `IniFile` 落盘：27 个 `[Section]` × 10 键 + `[Setup]` 24 键的**键序逐字节**；
///   * `lblSpeedValue` 两种格式的差异（`'[%d/%d]'` vs `'[%d/ %d]'`）。
/// </summary>
[Collection("RunGateFormLane")]
public class RunGateUtilsFormGameSpeedTests : IDisposable
{
    private readonly string _dir;

    public RunGateUtilsFormGameSpeedTests()
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

    // ============================ 默认配置（GateShare.pas:632-1066）============================

    [Fact]
    public void 默认配置_ActionList共27项且标量初值逐条()
    {
        var c = FormGlobals.CreateDefaultConfig();

        Assert.Equal(27, c.ActionList.Length);
        Assert.Equal(0, c.btMsgType);                                   // 原 :1024
        Assert.Equal(0xFF, c.btMsgFColor);                              // 原 :1025
        Assert.Equal(0x38, c.btMsgBColor);                              // 原 :1026
        Assert.Equal(5, c.nLockTime);                                   // 原 :1028
        Assert.True(c.boSaveLockStatus);                                // 原 :1029
        Assert.False(c.boShowLockLog);                                  // 原 :1030
        Assert.Equal("超速已经被锁定(原因：%s)，%d秒后自动解锁！", c.sShowLockMsg);   // 原 :1031
        Assert.Equal(200u, c.dwUserShop_Search_Interval);               // 原 :1035
        Assert.True(c.boUserShop_Search_ShowHint);
        Assert.Equal(200u, c.dwUserShop_Buy_Interval);
        Assert.Equal(200u, c.dwTakeOn_Item_Interval);
        Assert.Equal(1500u, c.dwDealTry_Attack_Interval);               // 原 :1044
        Assert.Equal(700u, c.dwBrutal_Attack_Interval);                 // 原 :1047
        Assert.Equal(30u, c.dwContinueSpeedPassIncTime);                // 原 :1053
        Assert.Equal(4, c.nContinueSpeedCount);                         // 原 :1056
        Assert.Equal(20, c.nSumSpeedCheckTime);                         // 原 :1058
        Assert.Equal(5, c.nSumSpeedMaxCount);                           // 原 :1059
        Assert.Equal(15, c.dwCollectCount);                             // 原 :1061
        Assert.Equal(9, c.dwSpeedValue);                                // 原 :1062
        Assert.Equal(30, c.dwClientUploadPickItemsTime);                // 原 :1065
    }

    [Theory]
    [InlineData(0, 0u, TActionProcessMode.apmFakeAttackPass, "[提示]: 您的【攻击】速度出现异常")]
    [InlineData(1, 1200u, TActionProcessMode.apmFakeAttackPass, "[提示]: 您的【魔法】速度出现异常")]
    [InlineData(2, 540u, TActionProcessMode.apmRebound, "[提示]: 您的【走路】速度出现异常")]
    [InlineData(3, 540u, TActionProcessMode.apmRebound, "[提示]: 您的【跑步】速度出现异常")]
    [InlineData(4, 100u, TActionProcessMode.apmRebound, "[提示]: 您的【转向】速度出现异常")]
    [InlineData(5, 620u, TActionProcessMode.apmRebound, "[提示]: 您的【挖肉】速度出现异常")]
    [InlineData(14, 250u, TActionProcessMode.apmOffline, "[提示]: 您的游戏速度出现异常")]
    [InlineData(24, 1u, TActionProcessMode.apmFakeAttackPass, "[提示]: 请爱护游戏环境，关闭加速外挂重新登陆")]
    public void 默认配置_ActionList逐项(int index, uint interval, TActionProcessMode mode, string hint)
    {
        var c = FormGlobals.CreateDefaultConfig();
        var a = c.ActionList[index];

        Assert.False(a.boEnabled);
        Assert.Equal(interval, a.nInterval);
        Assert.Equal(mode, a.ProcessMode);
        Assert.False(a.boProcessScript);
        Assert.Equal(TSumActionProcessMode.sapmNone, a.SumProcessMode);
        Assert.False(a.boShowHint);
        Assert.Equal(hint, a.sHintText);
        Assert.Equal(0, a.nCompensationValue);
        Assert.False(a.boDebug);
    }

    // ============================ DescribeCell / IntervalCellText（原 :933-1006）============================

    [Fact]
    public void IntervalCellText_十八种模式显示设置()
    {
        Assert.Equal("设置", GameSpeedLogic.IntervalCellText(TAntiPlugActionMode.amHit));
        Assert.Equal("设置", GameSpeedLogic.IntervalCellText(TAntiPlugActionMode.amWalkToHit));
        Assert.Equal("设置", GameSpeedLogic.IntervalCellText(TAntiPlugActionMode.amCutMeatToMove));
    }

    [Fact]
    public void IntervalCellText_其余模式显示nInterval()
    {
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amTurn].nInterval = 321;
        Assert.Equal("321", GameSpeedLogic.IntervalCellText(TAntiPlugActionMode.amTurn));

        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHitConcurrent].nInterval = 1;
        Assert.Equal("1", GameSpeedLogic.IntervalCellText(TAntiPlugActionMode.amHitConcurrent));
    }

    [Fact]
    public void DescribeCell_列0带大于小于后缀()
    {
        // 原 :944-947：>= amHitConcurrent 用 ' >'，否则 ' <'
        Assert.Equal(RunGateConst.AntiPlugActionModeNames[0] + " <", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amHit, GameSpeedColumn.Enabled));
        Assert.Equal(RunGateConst.AntiPlugActionModeNames[24] + " >", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amHitConcurrent, GameSpeedColumn.Enabled));
        Assert.Equal(RunGateConst.AntiPlugActionModeNames[26] + " >", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amMoveConcurrent, GameSpeedColumn.Enabled));
    }

    [Fact]
    public void DescribeCell_列2并发行用Names2其余用Names()
    {
        // ★ 原 GateShare.pas:464-468：两张表**只在 apmFakeAttackPass（下标 4）不同**：
        //     ActionProcessModeNames [4] = '假刀放行'
        //     ActionProcessModeNames2[4] = '丢弃封包'
        //   其它下标（含 apmLost=2 的 '卡位操作'）两表**完全相同** —— 故必须用下标 4 才能区分：
        //   原 uFrmGameSpeed.pas:965-971 `if ActionMode >= amHitConcurrent then Names2[...] else Names[...]`
        Assert.Equal("假刀放行", RunGateConst.ActionProcessModeNames[(int)TActionProcessMode.apmFakeAttackPass]);
        Assert.Equal("丢弃封包", RunGateConst.ActionProcessModeNames2[(int)TActionProcessMode.apmFakeAttackPass]);

        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].ProcessMode = TActionProcessMode.apmFakeAttackPass;
        Assert.Equal("假刀放行", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amHit, GameSpeedColumn.ProcessMode));   // 原 :970

        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHitConcurrent].ProcessMode = TActionProcessMode.apmFakeAttackPass;
        Assert.Equal("丢弃封包", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amHitConcurrent, GameSpeedColumn.ProcessMode));   // 原 :968
    }

    [Fact]
    public void DescribeCell_列2对apmLost两表同名_不构成差异点()
    {
        // 差异断言的反面：apmLost 在两张表里都是 '卡位操作'，用它做断言**无法**区分 Names/Names2。
        Assert.Equal(RunGateConst.ActionProcessModeNames[(int)TActionProcessMode.apmLost],
                     RunGateConst.ActionProcessModeNames2[(int)TActionProcessMode.apmLost]);
        Assert.Equal("卡位操作", RunGateConst.ActionProcessModeNames[(int)TActionProcessMode.apmLost]);

        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHitConcurrent].ProcessMode = TActionProcessMode.apmLost;
        Assert.Equal("卡位操作", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amHitConcurrent, GameSpeedColumn.ProcessMode));
    }

    [Fact]
    public void DescribeCell_列5与列8恒为单个空格()
    {
        Assert.Equal(" ", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amHit, GameSpeedColumn.ShowHint));            // 原 :994
        Assert.Equal(" ", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amHit, GameSpeedColumn.Debug));               // 原 :1003
        Assert.Equal(" ", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amHit, GameSpeedColumn.FloatingInterval));    // 原 :994
    }

    [Fact]
    public void DescribeCell_列7仅三个模式显示补偿值()
    {
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].nCompensationValue = 55;
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amSpell].nCompensationValue = 66;

        Assert.Equal("55", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amHit, GameSpeedColumn.CompensationValue));
        Assert.Equal(" ", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amSpell, GameSpeedColumn.CompensationValue));   // amSpell 不在 [amHit,amWalk,amRun]
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amWalk].nCompensationValue = 77;
        Assert.Equal("77", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amWalk, GameSpeedColumn.CompensationValue));
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amRun].nCompensationValue = 88;
        Assert.Equal("88", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amRun, GameSpeedColumn.CompensationValue));
    }

    [Fact]
    public void DescribeCell_列3用累计处理名表()
    {
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].SumProcessMode = TSumActionProcessMode.sampLockUser;
        Assert.Equal("锁定用户", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amHit, GameSpeedColumn.SumProcessMode));
    }

    [Fact]
    public void DescribeCell_列6取sHintText()
    {
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].sHintText = "H";
        Assert.Equal("H", GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amHit, GameSpeedColumn.HintText));
    }

    // ============================ IsEditable / CanToggleCheck / ShouldBeChecked（原 :1065-1079、:1224-1234、:685-688）============================

    [Fact]
    public void IsEditable_并发行不可编辑列1与列2()
    {
        Assert.False(GameSpeedLogic.IsEditable(TAntiPlugActionMode.amHitConcurrent, GameSpeedColumn.Interval));     // 原 :1074
        Assert.False(GameSpeedLogic.IsEditable(TAntiPlugActionMode.amHitConcurrent, GameSpeedColumn.ProcessMode));
        // 其它列仍可编辑
        Assert.True(GameSpeedLogic.IsEditable(TAntiPlugActionMode.amHitConcurrent, GameSpeedColumn.SumProcessMode));
        Assert.True(GameSpeedLogic.IsEditable(TAntiPlugActionMode.amHitConcurrent, GameSpeedColumn.HintText));
        // 非并发行列1/2 可编辑
        Assert.True(GameSpeedLogic.IsEditable(TAntiPlugActionMode.amHit, GameSpeedColumn.Interval));
        Assert.True(GameSpeedLogic.IsEditable(TAntiPlugActionMode.amHit, GameSpeedColumn.ProcessMode));
    }

    [Fact]
    public void IsEditable_列7仅三个模式可编辑()
    {
        Assert.True(GameSpeedLogic.IsEditable(TAntiPlugActionMode.amHit, GameSpeedColumn.CompensationValue));     // 原 :1076
        Assert.True(GameSpeedLogic.IsEditable(TAntiPlugActionMode.amWalk, GameSpeedColumn.CompensationValue));
        Assert.True(GameSpeedLogic.IsEditable(TAntiPlugActionMode.amRun, GameSpeedColumn.CompensationValue));
        Assert.False(GameSpeedLogic.IsEditable(TAntiPlugActionMode.amSpell, GameSpeedColumn.CompensationValue));
        Assert.False(GameSpeedLogic.IsEditable(TAntiPlugActionMode.amTurn, GameSpeedColumn.CompensationValue));
        Assert.False(GameSpeedLogic.IsEditable(TAntiPlugActionMode.amCutMeat, GameSpeedColumn.CompensationValue));
    }

    [Fact]
    public void CanToggleCheck_并发行不可改()
    {
        Assert.True(GameSpeedLogic.CanToggleCheck(TAntiPlugActionMode.amHit));               // 原 :1232
        Assert.True(GameSpeedLogic.CanToggleCheck(TAntiPlugActionMode.amCutMeatToMove));     // 索引 23（最后一行）
        Assert.False(GameSpeedLogic.CanToggleCheck(TAntiPlugActionMode.amHitConcurrent));
        Assert.False(GameSpeedLogic.CanToggleCheck(TAntiPlugActionMode.amSpellConcurrent));
        Assert.False(GameSpeedLogic.CanToggleCheck(TAntiPlugActionMode.amMoveConcurrent));
    }

    [Fact]
    public void ShouldBeChecked_启用或并发行都勾选()
    {
        Assert.False(GameSpeedLogic.ShouldBeChecked(TAntiPlugActionMode.amHit));             // 原 :685
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].boEnabled = true;
        Assert.True(GameSpeedLogic.ShouldBeChecked(TAntiPlugActionMode.amHit));

        // 并发行即使 boEnabled = false 也强制勾选
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHitConcurrent].boEnabled = false;
        Assert.True(GameSpeedLogic.ShouldBeChecked(TAntiPlugActionMode.amHitConcurrent));
    }

    // ============================ GetHintText（原 :1174-1204）============================

    [Fact]
    public void GetHintText_列0三个并发模式的专属提示()
    {
        Assert.Equal("此项默认勾选，取消后游戏中将出现多倍攻击，需求封双倍请配合攻击间隔设置！",
            GameSpeedLogic.GetHintText(TAntiPlugActionMode.amHitConcurrent, GameSpeedColumn.Enabled));
        Assert.Equal("此项默认勾选，取消后游戏中将出现多倍魔法，需求封双倍请配合攻击间隔设置！",
            GameSpeedLogic.GetHintText(TAntiPlugActionMode.amSpellConcurrent, GameSpeedColumn.Enabled));
        Assert.Equal("此项默认勾选，取消后游戏中可能会出现暗杀或飞机速度的玩家！",
            GameSpeedLogic.GetHintText(TAntiPlugActionMode.amMoveConcurrent, GameSpeedColumn.Enabled));
    }

    [Fact]
    public void GetHintText_列0其它模式无提示()
        => Assert.Equal("", GameSpeedLogic.GetHintText(TAntiPlugActionMode.amHit, GameSpeedColumn.Enabled));

    [Fact]
    public void GetHintText_列7恒有提示()
        => Assert.Equal("补偿值小于或等于0表示关闭", GameSpeedLogic.GetHintText(TAntiPlugActionMode.amHit, GameSpeedColumn.CompensationValue));   // 原 :1202

    [Fact]
    public void GetHintText_列6的提示被原文注释掉_不返回()
    {
        // 原 :1180-1186 整段被注释 → 列 6 无提示（差异断言：不是'如果采集次数为8...'）
        Assert.Equal("", GameSpeedLogic.GetHintText(TAntiPlugActionMode.amHit, GameSpeedColumn.HintText));
    }

    // ============================ BuildProcessModeItems（原 :470-509）============================

    [Fact]
    public void BuildProcessModeItems_a组从apmRebound开始共5项()
    {
        var items = new List<string>();
        var raws = new List<int>();
        GameSpeedLogic.BuildProcessModeItems(TAntiPlugActionMode.amHit, TActionProcessMode.apmRebound, items, raws, out int idx);

        Assert.Equal(5, items.Count);                                   // apmRebound..apmNoProcess（跳过 apmDelay）
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, raws);
        Assert.Equal(new[] { "反弹卡刀", "卡位操作", "掉线处理", "假刀放行", "不做处理" }, items);
        Assert.Equal(0, idx);                                            // Ord(apmRebound) - Ord(apmRebound)
    }

    [Fact]
    public void BuildProcessModeItems_a组ItemIndex按apmRebound偏移()
    {
        GameSpeedLogic.BuildProcessModeItems(TAntiPlugActionMode.amHit, TActionProcessMode.apmNoProcess,
            new List<string>(), new List<int>(), out int idx);
        Assert.Equal(4, idx);                                            // 5 - 1
    }

    [Fact]
    public void BuildProcessModeItems_b组从apmOffline开始用Names2()
    {
        var items = new List<string>();
        var raws = new List<int>();
        GameSpeedLogic.BuildProcessModeItems(TAntiPlugActionMode.amHitConcurrent, TActionProcessMode.apmOffline, items, raws, out int idx);

        Assert.Equal(3, items.Count);                                    // apmOffline..apmNoProcess
        Assert.Equal(new[] { 3, 4, 5 }, raws);
        // ★ 原 :486 `Items.AddObject(ActionProcessModeNames2[ProcessMode], ...)` —— b 组用的是 **Names2**。
        //   Names2[4] = '丢弃封包'（Names[4] 是 '假刀放行'）→ 本组第 2 项应为 '丢弃封包'。
        Assert.Equal(new[] { "掉线处理", "丢弃封包", "不做处理" }, items);
        Assert.Equal(0, idx);
    }

    [Fact]
    public void BuildProcessModeItems_c组跳过apmFakeAttackPass共5项()
    {
        var items = new List<string>();
        var raws = new List<int>();
        GameSpeedLogic.BuildProcessModeItems(TAntiPlugActionMode.amTurn, TActionProcessMode.apmDelay, items, raws, out int idx);

        Assert.Equal(5, items.Count);                                    // apmDelay/Rebound/Lost/Offline/NoProcess
        Assert.Equal(new[] { 0, 1, 2, 3, 5 }, raws);                     // 跳过 4 = apmFakeAttackPass
        Assert.Equal(0, idx);
    }

    [Fact]
    public void BuildProcessModeItems_c组apmNoProcess特判Ord减1()
    {
        // ★ 最易错点：c 组少了 apmFakeAttackPass 一项，故 apmNoProcess 的下标要减 1
        GameSpeedLogic.BuildProcessModeItems(TAntiPlugActionMode.amTurn, TActionProcessMode.apmNoProcess,
            new List<string>(), new List<int>(), out int idx);
        Assert.Equal(4, idx);                                            // 5 - 1

        // 对照：c 组的 apmOffline 不减（原 :505）
        GameSpeedLogic.BuildProcessModeItems(TAntiPlugActionMode.amTurn, TActionProcessMode.apmOffline,
            new List<string>(), new List<int>(), out int idx2);
        Assert.Equal(3, idx2);
    }

    [Fact]
    public void BuildProcessModeItems_a组与c组对apmNoProcess的下标不同_差异断言()
    {
        // a 组：apmNoProcess → 4（5 - 1 = Ord 5 - Ord(apmRebound)=1）
        GameSpeedLogic.BuildProcessModeItems(TAntiPlugActionMode.amHit, TActionProcessMode.apmNoProcess,
            new List<string>(), new List<int>(), out int a);
        // c 组：apmNoProcess → 4（Ord 5 - 1 = 4）—— 数值巧合相同，但**列表内容不同**
        GameSpeedLogic.BuildProcessModeItems(TAntiPlugActionMode.amTurn, TActionProcessMode.apmNoProcess,
            new List<string>(), new List<int>(), out int c);
        Assert.Equal(a, c);

        // 真正的差异在 apmDelay：c 组能选、a 组不能
        var itemsA = new List<string>();
        GameSpeedLogic.BuildProcessModeItems(TAntiPlugActionMode.amHit, TActionProcessMode.apmDelay, itemsA, new List<int>(), out _);
        Assert.DoesNotContain("停顿操作", itemsA);
        var itemsC = new List<string>();
        GameSpeedLogic.BuildProcessModeItems(TAntiPlugActionMode.amTurn, TActionProcessMode.apmDelay, itemsC, new List<int>(), out _);
        Assert.Contains("停顿操作", itemsC);
    }

    [Fact]
    public void BuildSumProcessModeItems_三项且ItemIndex为Ord()
    {
        var items = new List<string>();
        var raws = new List<int>();
        GameSpeedLogic.BuildSumProcessModeItems(TSumActionProcessMode.sampLockUser, items, raws, out int idx);

        Assert.Equal(3, items.Count);
        Assert.Equal(new[] { "不处理", "掉线操作", "锁定用户" }, items);
        Assert.Equal(new[] { 0, 1, 2 }, raws);
        Assert.Equal(2, idx);                                            // 原 :530 Integer(SumProcessMode)
    }

    // ============================ PrepareEditSpec（原 :385-572）============================

    [Fact]
    public void PrepareEditSpec_列1在18种模式下是按钮()
    {
        var spec = GameSpeedLogic.PrepareEditSpec(TAntiPlugActionMode.amHit, 1);
        Assert.Equal(EditorKind.Button, spec.Kind);                      // 原 :403
    }

    [Fact]
    public void PrepareEditSpec_列1非按钮模式是SpinEdit_Min1_MaxHigh()
    {
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amTurn].nInterval = 500;
        var spec = GameSpeedLogic.PrepareEditSpec(TAntiPlugActionMode.amTurn, 1);

        Assert.Equal(EditorKind.SpinEdit, spec.Kind);                    // 原 :420
        Assert.Equal(1, spec.MinValue);                                  // 原 :431-432
        Assert.Equal(int.MaxValue, spec.MaxValue);                       // 原 :429
        Assert.Equal(500, spec.Value);                                   // 原 :450
    }

    [Fact]
    public void PrepareEditSpec_列4是SpinEdit_Min0_Max30000_值0()
    {
        var spec = GameSpeedLogic.PrepareEditSpec(TAntiPlugActionMode.amHit, 4);
        Assert.Equal(EditorKind.SpinEdit, spec.Kind);
        Assert.Equal(0, spec.MinValue);                                  // 原 :436
        Assert.Equal(30000, spec.MaxValue);                              // 原 :437
        Assert.Equal(0, spec.Value);                                     // 原 :451（nFloatingInterval 被注释）
    }

    [Fact]
    public void PrepareEditSpec_列7是SpinEdit_Min0_Max0_值取补偿值()
    {
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].nCompensationValue = 9;
        var spec = GameSpeedLogic.PrepareEditSpec(TAntiPlugActionMode.amHit, 7);

        Assert.Equal(EditorKind.SpinEdit, spec.Kind);
        Assert.Equal(0, spec.MinValue);                                  // 原 :563
        Assert.Equal(0, spec.MaxValue);                                  // 原 :562
        Assert.Equal(9, spec.Value);                                     // 原 :564
    }

    [Fact]
    public void PrepareEditSpec_列2与列3是ComboBox()
    {
        Assert.Equal(EditorKind.ComboBox, GameSpeedLogic.PrepareEditSpec(TAntiPlugActionMode.amHit, 2).Kind);    // 原 :460
        Assert.Equal(EditorKind.ComboBox, GameSpeedLogic.PrepareEditSpec(TAntiPlugActionMode.amHit, 3).Kind);    // 原 :515
    }

    [Fact]
    public void PrepareEditSpec_列6是Edit并取HintText()
    {
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].sHintText = "HX";
        var spec = GameSpeedLogic.PrepareEditSpec(TAntiPlugActionMode.amHit, 6);

        Assert.Equal(EditorKind.Edit, spec.Kind);                        // 原 :540
        Assert.Equal("HX", spec.Text);                                   // 原 :547
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(8)]
    public void PrepareEditSpec_其余列无编辑器(int column)
    {
        var spec = GameSpeedLogic.PrepareEditSpec(TAntiPlugActionMode.amHit, column);
        Assert.Equal(EditorKind.None, spec.Kind);                        // 原 :569-570 Result := False
    }

    [Fact]
    public void PrepareEditSpec_列2的ComboBox带原始枚举值()
    {
        var spec = GameSpeedLogic.PrepareEditSpec(TAntiPlugActionMode.amHit, 2);
        Assert.Equal(spec.Items.Count, spec.ItemRawValues.Count);        // 原文 Items.Objects 与 Items 同步
    }

    // ============================ ApplyEditorResult（原 :258-361）============================

    [Fact]
    public void ApplyEditorResult_列1间隔变化时返回True并写入()
    {
        var spec = new EditorSpec { Kind = EditorKind.SpinEdit, Value = 777 };
        bool changed = GameSpeedLogic.ApplyEditorResult(TAntiPlugActionMode.amTurn, 1, spec, "");

        Assert.True(changed);
        Assert.Equal(777u, FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amTurn].nInterval);
    }

    [Fact]
    public void ApplyEditorResult_列1值未变返回False()
    {
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amTurn].nInterval = 100;
        var spec = new EditorSpec { Kind = EditorKind.SpinEdit, Value = 100 };

        Assert.False(GameSpeedLogic.ApplyEditorResult(TAntiPlugActionMode.amTurn, 1, spec, ""));
    }

    [Fact]
    public void ApplyEditorResult_列7补偿值变化()
    {
        var spec = new EditorSpec { Kind = EditorKind.SpinEdit, Value = 42 };
        Assert.True(GameSpeedLogic.ApplyEditorResult(TAntiPlugActionMode.amHit, 7, spec, ""));
        Assert.Equal(42, FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].nCompensationValue);
    }

    [Fact]
    public void ApplyEditorResult_列2处理方式变化走Items原始值()
    {
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].ProcessMode = TActionProcessMode.apmFakeAttackPass;
        var spec = new EditorSpec { Kind = EditorKind.ComboBox, ItemRawValues = new List<int> { 0, 1, 2, 3, 5 } };
        spec.ItemIndexRaw = 5;                                           // 选中 apmNoProcess

        Assert.True(GameSpeedLogic.ApplyEditorResult(TAntiPlugActionMode.amHit, 2, spec, ""));
        Assert.Equal(TActionProcessMode.apmNoProcess, FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].ProcessMode);
    }

    [Fact]
    public void ApplyEditorResult_列3累计处理变化()
    {
        var spec = new EditorSpec { Kind = EditorKind.ComboBox, Value = (int)TSumActionProcessMode.sampOffline };
        Assert.True(GameSpeedLogic.ApplyEditorResult(TAntiPlugActionMode.amHit, 3, spec, ""));
        Assert.Equal(TSumActionProcessMode.sampOffline, FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].SumProcessMode);
    }

    [Fact]
    public void ApplyEditorResult_列6提示文本变化用SameText判等()
    {
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].sHintText = "abc";
        var specNoChange = new EditorSpec { Kind = EditorKind.Edit };
        Assert.False(GameSpeedLogic.ApplyEditorResult(TAntiPlugActionMode.amHit, 6, specNoChange, "  ABC  "));   // SameText 忽略空白与大小写

        var specChange = new EditorSpec { Kind = EditorKind.Edit };
        Assert.True(GameSpeedLogic.ApplyEditorResult(TAntiPlugActionMode.amHit, 6, specChange, "xyz"));
        Assert.Equal("xyz", FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].sHintText);
    }

    [Fact]
    public void SameText_忽略首尾空白且大小写不敏感()
    {
        Assert.True(GameSpeedLogic.SameText(" abc ", "ABC"));
        Assert.False(GameSpeedLogic.SameText("abc", "abd"));
        Assert.True(GameSpeedLogic.SameText(null, ""));
    }

    [Fact]
    public void ApplyEditorResult_无编辑器时返回False()
    {
        Assert.False(GameSpeedLogic.ApplyEditorResult(TAntiPlugActionMode.amHit, 0, new EditorSpec { Kind = EditorKind.None }, ""));
    }

    // ============================ ValidateSpeedValue（原 :768-781）============================

    [Fact]
    public void ValidateSpeedValue_两个TrackBar下限夹到3()
    {
        var r = GameSpeedLogic.ValidateSpeedValue(0, 0, out int collect, out int speed);
        Assert.Equal(GameSpeedSaveResult.SpeedValueNotLessThanCollectCount, r);   // 3 >= 3 → 失败
        Assert.Equal(3, collect);
        Assert.Equal(3, speed);
    }

    [Fact]
    public void ValidateSpeedValue_合法组合通过()
    {
        var r = GameSpeedLogic.ValidateSpeedValue(15, 9, out int collect, out int speed);
        Assert.Equal(GameSpeedSaveResult.OK, r);
        Assert.Equal(15, collect);
        Assert.Equal(9, speed);
    }

    [Fact]
    public void ValidateSpeedValue_相等时失败()
    {
        var r = GameSpeedLogic.ValidateSpeedValue(10, 10, out _, out _);
        Assert.Equal(GameSpeedSaveResult.SpeedValueNotLessThanCollectCount, r);   // 原 :776
    }

    [Fact]
    public void ValidateSpeedValue_速度大于总数时失败()
    {
        var r = GameSpeedLogic.ValidateSpeedValue(10, 11, out _, out _);
        Assert.Equal(GameSpeedSaveResult.SpeedValueNotLessThanCollectCount, r);
    }

    [Fact]
    public void ValidateSpeedValue_原文缺陷_零速度被提升到3后可能反而通过()
    {
        // ★ D9：trckbrSpeedValue=0 会被**提升到 3**；若 collectCount 也被提升到 3，
        //   则 speed(3) >= collect(3) 成立 → **失败**。但若 collect=4 而 speed=0：
        //   speed 提升到 3 < 4 → **通过**，保存后 speedValue=3、collectCount=4。
        var r = GameSpeedLogic.ValidateSpeedValue(4, 0, out int collect, out int speed);
        Assert.Equal(GameSpeedSaveResult.OK, r);
        Assert.Equal(4, collect);
        Assert.Equal(3, speed);      // 用户填 0，落盘成 3
    }

    [Fact]
    public void 错误提示文本含全角引号()
        => Assert.Equal("“超速次数”必须 < “总记录数”", GameSpeedLogic.SpeedValueErrorText);   // 原 :778

    // ============================ lblSpeedValue 两种格式（原 :725 / :748）============================

    [Fact]
    public void SpeedValueLabel_两处格式不同_差异断言()
    {
        string refresh = GameSpeedLogic.SpeedValueLabelRefresh(9, 15);
        string change = GameSpeedLogic.SpeedValueLabelOnChange(9, 15);

        Assert.Equal("[9/15]", refresh);      // 原 :725 Format('[%d/%d]', ...)
        Assert.Equal("[9/ 15]", change);      // 原 :748 Format('[%d/ %d]', ...) —— 斜杠后多一个空格
        Assert.NotEqual(refresh, change);
    }

    // ============================ Save（原 :757-855）============================

    [Fact]
    public void Save_27个动作节各10键_键名与顺序逐字节()
    {
        GameSpeedLogic.Save(IniPath);

        // 第 1 个节 Hit 的 10 个键（顺序 = 原 :795-806）
        int start = IniText.IndexOf("[Hit]\r\n", StringComparison.Ordinal);
        Assert.True(start >= 0);
        int end = IniText.IndexOf("[Spell]\r\n", StringComparison.Ordinal);
        string hit = IniText.Substring(start, end - start);
        Assert.Equal("[Hit]\r\n" +
                     "Enabled=0\r\n" +
                     "Interval=0\r\n" +
                     "ProcessMode=4\r\n" +           // 默认 apmFakeAttackPass
                     "ProcessScript=0\r\n" +
                     "SumProcessMode=0\r\n" +
                     "ShowHint=0\r\n" +
                     "HintText=[提示]: 您的【攻击】速度出现异常\r\n" +
                     "CompensationValue=0\r\n" +
                     "Debug=0\r\n\r\n", hit);

        // 节名顺序 = AntiPlugActionModeSections
        int prev = -1;
        foreach (string section in RunGateConst.AntiPlugActionModeSections)
        {
            int at = IniText.IndexOf("[" + section + "]\r\n", StringComparison.Ordinal);
            Assert.True(at > prev, "节 " + section + " 顺序错误");
            prev = at;
        }
    }

    [Fact]
    public void Save_Setup节24键顺序逐字节()
    {
        GameSpeedLogic.Save(IniPath);

        int start = IniText.IndexOf("[Setup]\r\n", StringComparison.Ordinal);
        Assert.True(start >= 0);
        string setup = IniText.Substring(start);
        Assert.Equal(
            "[Setup]\r\n" +
            "LockTime=5\r\n" +
            "SaveLockStatus=1\r\n" +
            "ShowLockLog=0\r\n" +
            "ShowLockMsg=超速已经被锁定(原因：%s)，%d秒后自动解锁！\r\n" +
            "SpeedClearData=0\r\n" +
            "MsgType=0\r\n" +
            "MsgFColor=255\r\n" +
            "MsgBColor=56\r\n" +
            "UserShopSearchInterval=200\r\n" +
            "UserShopSearchShowHint=1\r\n" +
            "UserShopBuyInterval=200\r\n" +
            "UserShopBuyShowHint=1\r\n" +
            "TakeOnItemInterval=200\r\n" +
            "TakeOnItemShowHint=1\r\n" +
            "DealTryAttackInterval=1500\r\n" +
            "DealTryAttackShowHint=1\r\n" +
            "BrutalAttackInterval=700\r\n" +
            "BrutalAttackShowHint=1\r\n" +
            "ShowAttackLog=0\r\n" +
            "ContinueSpeedPassIncTime=30\r\n" +
            "CollectCount=15\r\n" +
            "SpeedValue=9\r\n" +
            "ContinueSpeedCloseSocket=0\r\n" +
            "ContinueSpeedCount=4\r\n" +
            "SumSpeedCheckTime=20\r\n" +
            "SumSpeedMaxCount=5\r\n" +
            "ZeroCompensationValueClearPool=0\r\n" +
            "ClientUploadPickItemsTime=30\r\n" +
            "\r\n", setup);
    }

    [Fact]
    public void Save_被注释掉的键不出现()
    {
        GameSpeedLogic.Save(IniPath);

        // 原 :800-802（动作节的 FloatingInterval / CollectCount / CollectSpeedCount）与
        // 原 :836（Setup 的 ShowDropConcurrentLog）都被注释掉 → 不应出现在 INI 里
        Assert.DoesNotContain("FloatingInterval", IniText, StringComparison.Ordinal);
        Assert.DoesNotContain("CollectSpeedCount", IniText, StringComparison.Ordinal);
        Assert.DoesNotContain("ShowDropConcurrentLog", IniText, StringComparison.Ordinal);

        // Reset：动作节里没有裸的 CollectCount（只有 [Setup] 的那一个）
        int setupAt = IniText.IndexOf("[Setup]", StringComparison.Ordinal);
        string actionPart = IniText.Substring(0, setupAt);
        Assert.DoesNotContain("CollectCount", actionPart, StringComparison.Ordinal);
    }

    [Fact]
    public void Save_三个并发模式也写入_INI含27节()
    {
        GameSpeedLogic.Save(IniPath);
        foreach (string s in new[] { "HitConcurrent", "SpellConcurrent", "MoveConcurrent" })
            Assert.Contains("[" + s + "]\r\n", IniText, StringComparison.Ordinal);

        Assert.Contains("[MoveConcurrent]\r\n", IniText, StringComparison.Ordinal);
        Assert.Equal(27, RunGateConst.AntiPlugActionModeSections.Length);
    }

    [Fact]
    public void Save_空文件名不抛异常()
    {
        var ex = Record.Exception(() => GameSpeedLogic.Save(""));
        Assert.Null(ex);
    }

    // ============================ ColorIndexToTColor（HUtil32.pas:212-293 的 256 项调色板）============================

    [Fact]
    public void ColorIndexToTColor_全部256个下标都有定义_不抛异常()
    {
        // ★ 回归点：此前误写成"16 常用色 + 6×6×6 计算调色板"，下标 ≥ 200 会算出 >255 的分量并抛
        //   ArgumentException（实测 btMsgFColor = $FF 即下标 255 必崩）。
        //   原文是 256 项常量表（TRGBQuad），**0..255 全部有定义、无越界分支**。
        for (int i = 0; i <= 255; i++)
        {
            var ex = Record.Exception(() => ColorIndex.ColorIndexToTColor((byte)i));
            Assert.Null(ex);
        }
    }

    [Fact]
    public void ColorIndexToTColor_与原文ColorArray逐项一致_抽点()
    {
        // HUtil32.pas:221-285 的 ColorArray（每项 4 字节 = B,G,R,0）
        // 下标 0 → (00,00,00) 黑
        Assert.Equal(System.Drawing.Color.FromArgb(255, 0, 0, 0), ColorIndex.ColorIndexToTColor(0));
        // 下标 1 → (B=00,G=00,R=80) → RGB(128,0,0) 暗红
        Assert.Equal(System.Drawing.Color.FromArgb(255, 0x80, 0x00, 0x00), ColorIndex.ColorIndexToTColor(1));
        // 下标 7 → (C0,C0,C0) → RGB(192,192,192) 银（前 8 项是 VGA 暗半调色板）
        Assert.Equal((0xC0, 0xC0, 0xC0), ColorIndex.RawEntry(7));
        Assert.Equal(System.Drawing.Color.FromArgb(255, 0xC0, 0xC0, 0xC0), ColorIndex.ColorIndexToTColor(7));
        // 最后一项（下标 255）→ (FF,FF,FF) 白
        Assert.Equal((0xFF, 0xFF, 0xFF), ColorIndex.RawEntry(255));
        // 注意：xUnit 的 Color 相等会同时比较"已知色"状态，`Color.White` 与 `FromArgb(255,255,255)` 不相等，
        // 故统一比较 ARGB 数值。
        Assert.Equal(System.Drawing.Color.White.ToArgb(), ColorIndex.ColorIndexToTColor(255).ToArgb());
        // 下标 12 起是原引擎自带的暗色系（不是"16 常用色"里的银）：B=52,G=52,R=5A
        Assert.Equal((0x52, 0x52, 0x5A), ColorIndex.RawEntry(12));
    }

    [Fact]
    public void ColorIndexToTColor_第4字节Reserved为0且不参与RGB()
    {
        // 原文 `TRGBQuad.Reserved` 恒为 $00；若有人误把 4 字节当作 (R,G,B,A) 会导致整体错位。
        // 校验：下标 1 的原始三元组是 (B=00,G=00,R=80)，RGB(80,00,00) = 暗红而不是 (00,00,80)。
        Assert.Equal((0x00, 0x00, 0x80), ColorIndex.RawEntry(1));
        Assert.Equal(System.Drawing.Color.FromArgb(255, 128, 0, 0), ColorIndex.ColorIndexToTColor(1));
    }

    [Fact]
    public void ColorIndexToTColor_RawEntry与ColorIndexToTColor一致_全表()
    {
        for (int i = 0; i <= 255; i++)
        {
            var (b, g, r) = ColorIndex.RawEntry((byte)i);
            var c = ColorIndex.ColorIndexToTColor((byte)i);
            Assert.Equal(r, c.R);
            Assert.Equal(g, c.G);
            Assert.Equal(b, c.B);
            Assert.Equal(255, c.A);      // RGB 宏不含 alpha，托管侧统一不透明
        }
    }

    [Fact]
    public void ColorIndex_项数常量与Byte全域一致()
        => Assert.Equal(256, ColorIndex.ColorCount);

    [Fact]
    public void ColorIndexToTColor_默认配置的MsgFColor与MsgBColor能正常转换()
    {
        // 回归：RefreshCtrlsStatus/构造期事件会用默认配置的 btMsgFColor=$FF(255) 与 btMsgBColor=$38(56)
        // 直接喂给 TColorIndexEdit.ColorIndex → 触发 seFColor_Change/seBColor_Change。
        var cfg = FormGlobals.CreateDefaultConfig();
        Assert.Equal(0xFF, cfg.btMsgFColor);
        Assert.Equal(0x38, cfg.btMsgBColor);
        // 比较 ARGB 数值（`Color.White` 的已知色状态与 `FromArgb(255,255,255)` 不同，不能直接比 Color）
        Assert.Equal(System.Drawing.Color.White.ToArgb(), ColorIndex.ColorIndexToTColor(cfg.btMsgFColor).ToArgb());
        Assert.Null(Record.Exception(() => ColorIndex.ColorIndexToTColor(cfg.btMsgBColor)));
    }

    // ============================ 窗体（DFM 对齐）============================

    [Fact]
    public void 窗体_DFM属性与全部控件名对齐()
    {
        using var f = new FrmGameSpeed();

        Assert.Equal("外挂控制", f.Text);                          // DFM: Caption
        Assert.Equal(761, f.ClientSize.Width);                     // DFM: ClientWidth=761
        Assert.Equal(589, f.ClientSize.Height);                    // DFM: ClientHeight=589
        Assert.Equal(System.Windows.Forms.FormBorderStyle.FixedDialog, f.FormBorderStyle);

        // 7 个 GroupBox
        Assert.Equal("参数设置", f.GroupBox1.Text);
        Assert.Equal("提示设置", f.GroupBox2.Text);
        Assert.Equal("锁定设置", f.GroupBox3.Text);
        Assert.Equal("间隔设置 [毫秒]", f.GroupBox5.Text);
        Assert.Equal("其他设置", f.grp2.Text);
        Assert.Equal("加速规则控制", f.GroupBox6.Text);
        Assert.Equal("累计超速规则", f.GroupBox4.Text);

        // 按钮
        Assert.Equal("保存(&S)", f.btnSave.Text);
        Assert.Equal("关闭(&E)", f.btnClose.Text);

        // 全部控件非空
        Assert.NotNull(f.vstAntiPlugAction);
        Assert.NotNull(f.trckbrSpeedValue);
        Assert.NotNull(f.trckbrCollectCount);
        Assert.NotNull(f.seFColor);
        Assert.NotNull(f.seBColor);
        Assert.NotNull(f.cbbMsgType);
        Assert.NotNull(f.edtPreview);
        Assert.NotNull(f.seLockTime);
        Assert.NotNull(f.chkSaveLockStatus);
        Assert.NotNull(f.chkShowLockLog);
        Assert.NotNull(f.edtShowLockMsg);
        Assert.NotNull(f.seDealTryAttackTime);
        Assert.NotNull(f.seBrutalAttackTime);
        Assert.NotNull(f.chkDealTryAttackHint);
        Assert.NotNull(f.chkBrutalAttackHint);
        Assert.NotNull(f.seUserShopSearchTime);
        Assert.NotNull(f.chkUserShopSearchHint);
        Assert.NotNull(f.seUserShopBuyTime);
        Assert.NotNull(f.chkUserShopBuyHint);
        Assert.NotNull(f.seTakeOnItemTime);
        Assert.NotNull(f.chkTakeOnItemHint);
        Assert.NotNull(f.chkZeroCompensationValueClearPool);
        Assert.NotNull(f.seContinueSpeedPassIncTime);
        Assert.NotNull(f.chkSpeedClearData);
        Assert.NotNull(f.chkShowAttackLog);
        Assert.NotNull(f.chkShowDropConcurrentLog);
        Assert.NotNull(f.seSumSpeedCheckTime);
        Assert.NotNull(f.seSumSpeedMaxCount);
        Assert.NotNull(f.chkContinueSpeedCloseSocket);
        Assert.NotNull(f.seContinueSpeedCount);
        Assert.NotNull(f.seClientUploadPickItemsTime);
        Assert.NotNull(f.ilCheck);

        // 几何抽查（.dfm）
        Assert.Equal(566, f.GroupBox6.Left);
        Assert.Equal(396, f.GroupBox6.Top);
        Assert.Equal(187, f.GroupBox6.Width);
        Assert.Equal(65, f.GroupBox6.Height);
        Assert.Equal(607, f.btnSave.Left);
        Assert.Equal(558, f.btnSave.Top);
        Assert.Equal(70, f.btnSave.Width);
        Assert.Equal(23, f.btnSave.Height);
        Assert.Equal(682, f.btnClose.Left);
        Assert.Equal("'0'补偿时清补偿池", f.chkZeroCompensationValueClearPool.Text);
    }

    [Fact]
    public void 窗体_FormCreate建24个可编辑节点_到amCutMeatToMove为止()
    {
        using var f = new FrmGameSpeed();
        f.FormCreate(f, EventArgs.Empty);

        Assert.Equal(24, f.vstAntiPlugAction.Nodes.Count);                  // 原 :658 Low..amCutMeatToMove
        Assert.Equal(24, GameSpeedLogic.EditableRowCount);
        Assert.Equal(TAntiPlugActionMode.amHit, f.vstAntiPlugAction.GetActionMode(f.vstAntiPlugAction.Nodes[0]));
        Assert.Equal(TAntiPlugActionMode.amCutMeatToMove, f.vstAntiPlugAction.GetActionMode(f.vstAntiPlugAction.Nodes[23]));
    }

    [Fact]
    public void 窗体_FormCreate的节点不含三个并发模式_差异断言()
    {
        using var f = new FrmGameSpeed();
        f.FormCreate(f, EventArgs.Empty);

        var modes = new List<TAntiPlugActionMode>();
        for (int i = 0; i < f.vstAntiPlugAction.Nodes.Count; i++)
            modes.Add(f.vstAntiPlugAction.GetActionMode(f.vstAntiPlugAction.Nodes[i]).Value);

        Assert.DoesNotContain(TAntiPlugActionMode.amHitConcurrent, modes);
        Assert.DoesNotContain(TAntiPlugActionMode.amSpellConcurrent, modes);
        Assert.DoesNotContain(TAntiPlugActionMode.amMoveConcurrent, modes);
    }

    [Fact]
    public void 窗体_RefreshCtrlsStatus并发行强制勾选并回填全部控件()
    {
        FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].boEnabled = true;
        FormGlobals.g_Config.nLockTime = 8;
        FormGlobals.g_Config.boSaveLockStatus = false;
        FormGlobals.g_Config.boShowLockLog = true;
        FormGlobals.g_Config.sShowLockMsg = "MSG";
        FormGlobals.g_Config.dwCollectCount = 20;
        FormGlobals.g_Config.dwSpeedValue = 10;

        using var f = new FrmGameSpeed();
        f.FormCreate(f, EventArgs.Empty);
        f.RefreshCtrlsStatus();

        Assert.True(f.vstAntiPlugAction.GetCheckState(f.vstAntiPlugAction.Nodes[0]));       // boEnabled=true
        Assert.False(f.vstAntiPlugAction.GetCheckState(f.vstAntiPlugAction.Nodes[1]));      // 未启用
        Assert.Equal(8, f.seLockTime.Value);
        Assert.False(f.chkSaveLockStatus.Checked);
        Assert.True(f.chkShowLockLog.Checked);
        Assert.Equal("MSG", f.edtShowLockMsg.Text);
        Assert.Equal(20, f.trckbrCollectCount.Value);
        Assert.Equal(10, f.trckbrSpeedValue.Value);
        Assert.Equal("[10/20]", f.lblSpeedValue.Text);                                      // 原 :725 的格式
        Assert.False(f.btnSave.Enabled);                                                    // SetSaveStatus(false)
    }

    [Fact]
    public void 窗体_trckbrSpeedValueChange用带空格格式并置脏()
    {
        using var f = new FrmGameSpeed();
        f.FormCreate(f, EventArgs.Empty);
        f.trckbrSpeedValue.Value = 9;
        f.trckbrCollectCount.Value = 15;
        f.btnSave.Enabled = false;

        f.trckbrSpeedValue_Change(f, EventArgs.Empty);

        Assert.Equal("[9/ 15]", f.lblSpeedValue.Text);                                       // 原 :748 的格式
        Assert.True(f.btnSave.Enabled);                                                      // SetSaveStatus(true)
    }

    [Fact]
    public void 窗体_btnSave_Click校验失败弹窗不落盘()
    {
        var shown = new List<string>();
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };

            using var f = new FrmGameSpeed();
            f.FormCreate(f, EventArgs.Empty);
            f.trckbrCollectCount.Value = 10;
            f.trckbrSpeedValue.Value = 10;       // speed >= collect → 失败
            FormGlobals.g_sIniFileName = IniPath;

            f.btnSave_Click(f, EventArgs.Empty);

            Assert.Single(shown);
            Assert.Equal("“超速次数”必须 < “总记录数”", shown[0]);      // 原 :778
            Assert.False(File.Exists(IniPath));
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_btnSave_Click成功写全局与INI()
    {
        FormGlobals.g_sIniFileName = IniPath;

        using var f = new FrmGameSpeed();
        f.FormCreate(f, EventArgs.Empty);
        f.trckbrCollectCount.Value = 20;
        f.trckbrSpeedValue.Value = 10;
        f.seClientUploadPickItemsTime.Value = 44;

        f.btnSave_Click(f, EventArgs.Empty);

        Assert.Equal(20, FormGlobals.g_Config.dwCollectCount);      // 原 :784
        Assert.Equal(10, FormGlobals.g_Config.dwSpeedValue);        // 原 :783
        Assert.Equal(44, FormGlobals.g_Config.dwClientUploadPickItemsTime);   // 原 :786
        Assert.False(f.btnSave.Enabled);                            // 原 :854
        Assert.Contains("CollectCount=20\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("SpeedValue=10\r\n", IniText, StringComparison.Ordinal);
        Assert.Contains("ClientUploadPickItemsTime=44\r\n", IniText, StringComparison.Ordinal);
    }

    [Fact]
    public void 窗体_btnClose_Click关闭窗体()
    {
        using var f = new FrmGameSpeed();
        f.btnClose_Click(f, EventArgs.Empty);
        // 未显示窗体时 Close 不抛异常即通过（原 :754 Close）
    }

    [Fact]
    public void 窗体_各Change事件写回全局并置脏()
    {
        var cfg = FormGlobals.g_Config;

        using var f = new FrmGameSpeed();
        f.FormCreate(f, EventArgs.Empty);
        f.btnSave.Enabled = false;

        f.seLockTime.Value = 12; f.seLockTime_Change(f, EventArgs.Empty);
        Assert.Equal(12, cfg.nLockTime);
        Assert.True(f.btnSave.Enabled);

        f.btnSave.Enabled = false;
        f.chkSaveLockStatus.Checked = true; f.chkSaveLockStatus_Click(f, EventArgs.Empty);
        Assert.True(cfg.boSaveLockStatus);
        Assert.True(f.btnSave.Enabled);

        f.btnSave.Enabled = false;
        f.edtShowLockMsg.Text = "Z"; f.edtShowLockMsg_Change(f, EventArgs.Empty);
        Assert.Equal("Z", cfg.sShowLockMsg);
        Assert.True(f.btnSave.Enabled);

        f.btnSave.Enabled = false;
        f.chkSpeedClearData.Checked = true; f.chkSpeedClearData_Click(f, EventArgs.Empty);
        Assert.True(cfg.boSpeedClearData);

        f.btnSave.Enabled = false;
        f.seDealTryAttackTime.Value = 1501; f.seDealTryAttackTime_Change(f, EventArgs.Empty);
        Assert.Equal(1501u, cfg.dwDealTry_Attack_Interval);

        f.btnSave.Enabled = false;
        f.seBrutalAttackTime.Value = 701; f.seBrutalAttackTime_Change(f, EventArgs.Empty);
        Assert.Equal(701u, cfg.dwBrutal_Attack_Interval);

        f.btnSave.Enabled = false;
        f.seContinueSpeedPassIncTime.Value = 31; f.seContinueSpeedPassIncTime_Change(f, EventArgs.Empty);
        Assert.Equal(31u, cfg.dwContinueSpeedPassIncTime);

        f.btnSave.Enabled = false;
        f.seSumSpeedCheckTime.Value = 21; f.seSumCheckTime_Change(f, EventArgs.Empty);
        Assert.Equal(21, cfg.nSumSpeedCheckTime);

        f.btnSave.Enabled = false;
        f.seSumSpeedMaxCount.Value = 6; f.seSumSpeedMaxCount_Change(f, EventArgs.Empty);
        Assert.Equal(6, cfg.nSumSpeedMaxCount);

        f.btnSave.Enabled = false;
        f.chkZeroCompensationValueClearPool.Checked = true; f.chkZeroCompensationValueClearPool_Click(f, EventArgs.Empty);
        Assert.True(cfg.boZeroCompensationValueClearPool);

        f.btnSave.Enabled = false;
        f.chkContinueSpeedCloseSocket.Checked = true; f.chkContinueSpeedCloseSocket_Click(f, EventArgs.Empty);
        Assert.True(cfg.boContinueSpeedCloseSocket);

        f.btnSave.Enabled = false;
        f.seContinueSpeedCount.Value = 5; f.seContinueSpeedCount_Change(f, EventArgs.Empty);
        Assert.Equal(5, cfg.nContinueSpeedCount);

        f.btnSave.Enabled = false;
        f.seUserShopSearchTime.Value = 201; f.seUserShopSearchTime_Change(f, EventArgs.Empty);
        Assert.Equal(201u, cfg.dwUserShop_Search_Interval);

        f.btnSave.Enabled = false;
        f.seUserShopBuyTime.Value = 202; f.seUserShopBuyTime_Change(f, EventArgs.Empty);
        Assert.Equal(202u, cfg.dwUserShop_Buy_Interval);

        f.btnSave.Enabled = false;
        f.seTakeOnItemTime.Value = 203; f.seTakeOnItemTime_Change(f, EventArgs.Empty);
        Assert.Equal(203u, cfg.dwTakeOn_Item_Interval);
    }

    [Fact]
    public void 窗体_各Hint复选框写回全局()
    {
        var cfg = FormGlobals.g_Config;
        using var f = new FrmGameSpeed();
        f.FormCreate(f, EventArgs.Empty);

        f.chkDealTryAttackHint.Checked = false; f.chkDealTryAttackHint_Click(f, EventArgs.Empty);
        Assert.False(cfg.boDealTry_Attack_ShowHint);
        f.chkBrutalAttackHint.Checked = false; f.chkBrutalAttackHint_Click(f, EventArgs.Empty);
        Assert.False(cfg.boBrutal_Attack_ShowHint);
        f.chkUserShopSearchHint.Checked = false; f.chkUserShopSearchHint_Click(f, EventArgs.Empty);
        Assert.False(cfg.boUserShop_Search_ShowHint);
        f.chkUserShopBuyHint.Checked = false; f.chkUserShopBuyHint_Click(f, EventArgs.Empty);
        Assert.False(cfg.boUserShop_Buy_ShowHint);
        f.chkTakeOnItemHint.Checked = false; f.chkTakeOnItemHint_Click(f, EventArgs.Empty);
        Assert.False(cfg.boTakeOn_Item_ShowHint);
        f.chkShowAttackLog.Checked = true; f.chkShowAttackLog_Click(f, EventArgs.Empty);
        Assert.True(cfg.boShowAttackLog);
    }

    [Fact]
    public void 窗体_颜色与消息类型事件写全局并更新预览()
    {
        var cfg = FormGlobals.g_Config;
        using var f = new FrmGameSpeed();
        f.FormCreate(f, EventArgs.Empty);

        f.seBColor.ColorIndex = 9;
        f.seBColor_Change(f, EventArgs.Empty);
        Assert.Equal(9, cfg.btMsgBColor);                                    // 原 :859
        Assert.Equal(ColorIndex.ColorIndexToTColor(9), f.edtPreview.BackColor);   // 原 :860

        f.seFColor.ColorIndex = 12;
        f.seFColor_Change(f, EventArgs.Empty);
        Assert.Equal(12, cfg.btMsgFColor);                                   // 原 :867
        Assert.Equal(ColorIndex.ColorIndexToTColor(12), f.edtPreview.ForeColor);  // 原 :869

        f.cbbMsgType.SelectedIndex = 1;
        f.cbbMsgType_Change(f, EventArgs.Empty);
        Assert.Equal(1, cfg.btMsgType);                                      // 原 :875
        Assert.True(f.btnSave.Enabled);
    }

    [Fact]
    public void 窗体_chkShowDropConcurrentLogClick不写全局_只置脏()
    {
        using var f = new FrmGameSpeed();
        f.FormCreate(f, EventArgs.Empty);
        f.btnSave.Enabled = false;
        f.chkShowDropConcurrentLog.Checked = true;      // 用户勾上

        f.chkShowDropConcurrentLog_Click(f, EventArgs.Empty);

        // 原 :1220 的 `g_Config.boShowDropConcurrentLog := ...` 被注释掉 → 该字段在 TAntiPlugConfig 里
        // 甚至**不存在**（原 :290 也注释掉了），故没有可断言的全局量；只断言"置脏"行为。
        Assert.True(f.btnSave.Enabled);                 // 原 :1221 SetSaveStatus(True)
    }

    [Fact]
    public void 窗体_vstAntiPlugActionChecked写boEnabled()
    {
        using var f = new FrmGameSpeed();
        f.FormCreate(f, EventArgs.Empty);
        f.btnSave.Enabled = false;

        f.vstAntiPlugAction_Checked(TAntiPlugActionMode.amHit, true);
        Assert.True(FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].boEnabled);   // 原 :1157
        Assert.True(f.btnSave.Enabled);

        f.vstAntiPlugAction_Checked(TAntiPlugActionMode.amHit, false);
        Assert.False(FormGlobals.g_Config.ActionList[(int)TAntiPlugActionMode.amHit].boEnabled);  // 原 :1159
    }

    [Fact]
    public void 窗体_OnBtnIntervalClick仅18种模式触发()
    {
        using var f = new FrmGameSpeed();
        // 不支持的模式（amTurn）→ 不打开子窗体（若误开会在无头环境挂住，这里靠断言保护）
        var ex = Record.Exception(() => f.OnBtnIntervalClick(TAntiPlugActionMode.amTurn));
        Assert.Null(ex);
    }

    [Fact]
    public void 窗体_btnDefault_Click为空实现()
    {
        using var f = new FrmGameSpeed();
        f.FormCreate(f, EventArgs.Empty);
        var before = FormGlobals.g_Config.nLockTime;

        var ex = Record.Exception(() => f.btnDefault_Click(f, EventArgs.Empty));
        Assert.Null(ex);                                                     // 原 :1164-1172 整段注释
        Assert.Equal(before, FormGlobals.g_Config.nLockTime);
    }

    [Fact]
    public void 窗体_IsEditable与Checking的转发一致()
    {
        using var f = new FrmGameSpeed();
        Assert.False(f.vstAntiPlugAction_Editing(TAntiPlugActionMode.amHitConcurrent, GameSpeedColumn.Interval));
        Assert.True(f.vstAntiPlugAction_Editing(TAntiPlugActionMode.amHit, GameSpeedColumn.Interval));
        Assert.False(f.vstAntiPlugAction_Checking(TAntiPlugActionMode.amHitConcurrent));
        Assert.True(f.vstAntiPlugAction_Checking(TAntiPlugActionMode.amHit));
    }

    [Fact]
    public void 窗体_CreateEditor转发PrepareEditSpec()
    {
        using var f = new FrmGameSpeed();
        Assert.Equal(EditorKind.Button, f.CreateEditor(TAntiPlugActionMode.amHit, 1).Kind);
        Assert.Equal(EditorKind.ComboBox, f.CreateEditor(TAntiPlugActionMode.amHit, 2).Kind);
    }

    [Fact]
    public void 窗体_GetText与GetHint转发一致()
    {
        using var f = new FrmGameSpeed();
        Assert.Equal(GameSpeedLogic.DescribeCell(TAntiPlugActionMode.amHit, 0), f.vstAntiPlugAction_GetText(TAntiPlugActionMode.amHit, 0));
        Assert.Equal(GameSpeedLogic.GetHintText(TAntiPlugActionMode.amHit, 7), f.vstAntiPlugAction_GetHint(TAntiPlugActionMode.amHit, 7));
    }

    [Fact]
    public void 窗体_列宽共9列()
    {
        using var f = new FrmGameSpeed();
        Assert.Equal(GameSpeedColumn.Count, f.vstAntiPlugAction.ColumnWidths.Count);
        Assert.Equal(112, f.vstAntiPlugAction.ColumnWidths[GameSpeedColumn.Enabled]);   // DFM: 是否控制 Width=112
        Assert.Equal(120, f.vstAntiPlugAction.ColumnWidths[GameSpeedColumn.ProcessMode]); // DFM: 超速处理方式 Width=120
    }

    [Fact]
    public void 窗体_StartEditing仅对可编辑列生效()
    {
        using var f = new FrmGameSpeed();
        f.FormCreate(f, EventArgs.Empty);
        var node = f.vstAntiPlugAction.Nodes[24 - 1];      // amCutMeatToMove（可编辑）

        f.StartEditing(node, GameSpeedColumn.HintText);
        Assert.True(f.vstAntiPlugAction.IsEditing);
    }

    [Fact]
    public void 窗体_SetSaveStatus切换保存按钮()
    {
        using var f = new FrmGameSpeed();
        f.SetSaveStatus(true);
        Assert.True(f.btnSave.Enabled);
        f.SetSaveStatus(false);
        Assert.False(f.btnSave.Enabled);
    }
}

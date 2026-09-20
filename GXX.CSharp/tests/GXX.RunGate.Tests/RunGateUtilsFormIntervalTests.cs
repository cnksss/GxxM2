using System;
using System.Collections.Generic;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uFrmInterval.pas（279 行）的纯逻辑与窗体测试。
/// 重点：
///   * `ActionModeUseSpeedIntervals` 支持的 18 种模式（amTurn/amCutMeat 等**不支持**，ShowFrmInterval 直接返回 False）；
///   * `DescribeMode` 的 18 个分支（Caption + 首格文本），**无 else** → 未命中保持 DFM 原值；
///   * `SendSpeedIntervalsCheckboxVisible` 与 `ShouldWriteSendSpeedIntervalsToClient` 用**同一组** 4 个基础模式；
///   * `RowLabel` 的 '-200'..'0'..'+200' 规则；
///   * `btnOKClick` 的 `Value <= 0`（**拒绝负数**，与 uFrmHitInterval 的 `= 0` 不同）；
///   * `btnAllClick` 的 `Max(5, speed0 - dec*(I-200))`；
///   * INI 键名含负号（Speed-200..Speed200）。
/// </summary>
[Collection("RunGateFormLane")]
public class RunGateUtilsFormIntervalTests : IDisposable
{
    private readonly string _dir;

    public RunGateUtilsFormIntervalTests()
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
    private string IntervalsFile => Path.Combine(_dir, "Intervals.ini");
    private string IniText => File.Exists(IniPath) ? File.ReadAllText(IniPath, System.Text.Encoding.GetEncoding(936)) : "";
    private string IntervalsText => File.Exists(IntervalsFile) ? File.ReadAllText(IntervalsFile, System.Text.Encoding.GetEncoding(936)) : "";

    // ---------------- 常量与模式集合 ----------------

    [Fact]
    public void 速度间隔常量_400与401()
    {
        Assert.Equal(200, RunGateConst.HalfSpeedIntervalsCount);       // GateShare.pas:25
        Assert.Equal(401, RunGateConst.SpeedIntervalsCount);           // GateShare.pas:26
        Assert.Equal(402, IntervalLogic.GridRowCount);                 // 原 :50 RowCount := 401 + 1
    }

    [Fact]
    public void IntervalButtonModes_共18种且与原文列表一致()
    {
        Assert.Equal(18, GameSpeedLogic.IntervalButtonModes.Length);
        Assert.Contains(TAntiPlugActionMode.amHit, GameSpeedLogic.IntervalButtonModes);
        Assert.Contains(TAntiPlugActionMode.amCutMeatToMove, GameSpeedLogic.IntervalButtonModes);
        // 不在列表中的
        Assert.DoesNotContain(TAntiPlugActionMode.amTurn, GameSpeedLogic.IntervalButtonModes);
        Assert.DoesNotContain(TAntiPlugActionMode.amCutMeat, GameSpeedLogic.IntervalButtonModes);
        Assert.DoesNotContain(TAntiPlugActionMode.amHitToTurn, GameSpeedLogic.IntervalButtonModes);
        Assert.DoesNotContain(TAntiPlugActionMode.amMoveToTurn, GameSpeedLogic.IntervalButtonModes);
        Assert.DoesNotContain(TAntiPlugActionMode.amMoveToCutMeat, GameSpeedLogic.IntervalButtonModes);
        Assert.DoesNotContain(TAntiPlugActionMode.amHitConcurrent, GameSpeedLogic.IntervalButtonModes);
    }

    [Fact]
    public void ActionModeUseSpeedIntervals_与18种按钮模式完全一致()
    {
        // 两处列表必须一致（原 :47 的守卫 与 :392-401 的按钮判定）
        for (int i = 0; i < RunGateConst.ActionModeCount; i++)
        {
            var mode = (TAntiPlugActionMode)i;
            bool use = FormGlobals.ActionModeUseSpeedIntervals(mode);
            bool isButton = GameSpeedLogic.IntervalCellIsButton(mode);
            Assert.Equal(isButton, use);
        }
    }

    [Theory]
    [InlineData(TAntiPlugActionMode.amHit, true)]
    [InlineData(TAntiPlugActionMode.amSpell, true)]
    [InlineData(TAntiPlugActionMode.amWalk, true)]
    [InlineData(TAntiPlugActionMode.amRun, true)]
    [InlineData(TAntiPlugActionMode.amTurn, false)]          // 原文注释掉了
    [InlineData(TAntiPlugActionMode.amCutMeat, false)]       // 原文注释掉了
    [InlineData(TAntiPlugActionMode.amHitToTurn, false)]     // 原文注释掉了
    [InlineData(TAntiPlugActionMode.amSpellToTurn, false)]   // 原文注释掉了
    [InlineData(TAntiPlugActionMode.amMoveToTurn, false)]    // 原文注释掉了
    [InlineData(TAntiPlugActionMode.amTurnToMove, true)]
    [InlineData(TAntiPlugActionMode.amMoveToCutMeat, false)] // 原文注释掉了
    [InlineData(TAntiPlugActionMode.amCutMeatToMove, true)]
    [InlineData(TAntiPlugActionMode.amHitConcurrent, false)] // 并发不在列表中
    public void ActionModeUseSpeedIntervals_逐模式(TAntiPlugActionMode mode, bool expected)
        => Assert.Equal(expected, FormGlobals.ActionModeUseSpeedIntervals(mode));

    // ---------------- DescribeMode（原 :54-169）----------------

    [Theory]
    [InlineData(TAntiPlugActionMode.amHit, "攻击间隔设置", "攻击加速")]
    [InlineData(TAntiPlugActionMode.amSpell, "魔法间隔设置", "魔法加速")]
    [InlineData(TAntiPlugActionMode.amWalk, "走路间隔设置", "走路加速")]
    [InlineData(TAntiPlugActionMode.amRun, "跑步间隔设置", "跑步加速")]
    [InlineData(TAntiPlugActionMode.amWalkToHit, "走路到攻击间隔设置", "攻击加速")]
    [InlineData(TAntiPlugActionMode.amHitToWalk, "攻击到走路间隔设置", "走路加速")]
    [InlineData(TAntiPlugActionMode.amRunToHit, "跑步到攻击间隔设置", "攻击加速")]
    [InlineData(TAntiPlugActionMode.amHitToRun, "攻击到跑步间隔设置", "跑步加速")]
    [InlineData(TAntiPlugActionMode.amWalkToSpell, "走路到魔法间隔设置", "魔法加速")]
    [InlineData(TAntiPlugActionMode.amSpellToWalk, "魔法到走路间隔设置", "走路加速")]
    [InlineData(TAntiPlugActionMode.amRunToSpell, "跑步到魔法间隔设置", "魔法加速")]
    [InlineData(TAntiPlugActionMode.amSpellToRun, "魔法到跑步间隔设置", "跑步加速")]
    [InlineData(TAntiPlugActionMode.amTurnToHit, "转向到攻击间隔设置", "攻击加速")]
    [InlineData(TAntiPlugActionMode.amTurnToSpell, "转向到魔法间隔设置", "魔法加速")]
    [InlineData(TAntiPlugActionMode.amCutMeatToHit, "挖肉到攻击间隔设置", "攻击加速")]
    [InlineData(TAntiPlugActionMode.amCutMeatToSpell, "挖肉到魔法间隔设置", "魔法加速")]
    [InlineData(TAntiPlugActionMode.amTurnToMove, "转向到移动间隔设置", "移动加速")]
    [InlineData(TAntiPlugActionMode.amCutMeatToMove, "挖肉到移动间隔设置", "移动加速")]
    public void DescribeMode_十八个分支逐条(TAntiPlugActionMode mode, string caption, string header)
    {
        var (c, h) = IntervalLogic.DescribeMode(mode);
        Assert.Equal(caption, c);
        Assert.Equal(header, h);
    }

    [Fact]
    public void DescribeMode_未命中分支保持DFM原值_原文无else()
    {
        // amTurn 不在 18 个分支内 → Caption 与首格保持 .dfm 的 '攻击间隔设置' / 空
        var (c, h) = IntervalLogic.DescribeMode(TAntiPlugActionMode.amTurn);
        Assert.Equal("攻击间隔设置", c);      // DFM 原值（不是"转向间隔设置"）
        Assert.Equal("", h);

        var (c2, _) = IntervalLogic.DescribeMode(TAntiPlugActionMode.amMoveToTurn);
        Assert.Equal("攻击间隔设置", c2);
    }

    // ---------------- 复选框可见性 / 写回判定（D2）----------------

    [Theory]
    [InlineData(TAntiPlugActionMode.amHit, false)]
    [InlineData(TAntiPlugActionMode.amSpell, false)]
    [InlineData(TAntiPlugActionMode.amWalk, false)]
    [InlineData(TAntiPlugActionMode.amRun, false)]
    [InlineData(TAntiPlugActionMode.amWalkToHit, true)]
    [InlineData(TAntiPlugActionMode.amTurnToMove, true)]
    public void SendSpeedIntervalsCheckboxVisible_仅4个基础模式隐藏(TAntiPlugActionMode mode, bool expected)
        => Assert.Equal(expected, IntervalLogic.SendSpeedIntervalsCheckboxVisible(mode));

    [Fact]
    public void 可见性判定与写回判定必须一致_否则会出现看不见控件却写了它的值()
    {
        for (int i = 0; i < RunGateConst.ActionModeCount; i++)
        {
            var mode = (TAntiPlugActionMode)i;
            Assert.Equal(IntervalLogic.SendSpeedIntervalsCheckboxVisible(mode),
                         IntervalLogic.ShouldWriteSendSpeedIntervalsToClient(mode));
        }
    }

    // ---------------- BuildInit（原 :52-174）----------------

    [Fact]
    public void BuildInit_基础模式隐藏复选框且不回填Checked()
    {
        FormGlobals.g_boSendSpeedIntervalsToClient[(int)TAntiPlugActionMode.amHit] = true;
        var init = IntervalLogic.BuildInit(TAntiPlugActionMode.amHit);

        Assert.Equal("攻击间隔设置", init.Caption);
        Assert.Equal("攻击加速", init.FirstHeader);
        Assert.False(init.SendSpeedIntervalsCheckboxVisible);       // 原 :60
        Assert.False(init.SendSpeedIntervalsChecked);               // 原 :171 未回填
    }

    [Fact]
    public void BuildInit_非基础模式显示并回填Checked()
    {
        FormGlobals.g_boSendSpeedIntervalsToClient[(int)TAntiPlugActionMode.amWalkToHit] = true;
        var init = IntervalLogic.BuildInit(TAntiPlugActionMode.amWalkToHit);

        Assert.Equal("走路到攻击间隔设置", init.Caption);
        Assert.Equal("攻击加速", init.FirstHeader);
        Assert.True(init.SendSpeedIntervalsCheckboxVisible);        // 原 :52
        Assert.True(init.SendSpeedIntervalsChecked);                // 原 :173
    }

    // ---------------- RowLabel / RowValueText（原 :176-187）----------------

    [Fact]
    public void RowLabel_负零正三种形态()
    {
        Assert.Equal("-200", IntervalLogic.RowLabel(0));            // 原 :184
        Assert.Equal("-1", IntervalLogic.RowLabel(199));
        // ★ 原 :178 `if I >= HALF_SPEED_INTERVALS_COUNT` → I=200 走**正号分支**，
        //   于是 `'+' + IntToStr(200-200)` = "+0"（**不是 "0"**）—— 原文缺陷/易错点，照抄。
        Assert.Equal("+0", IntervalLogic.RowLabel(200));
        Assert.Equal("+1", IntervalLogic.RowLabel(201));            // 原 :180
        Assert.Equal("+200", IntervalLogic.RowLabel(400));
    }

    [Fact]
    public void RowValueText_取全局数组值()
    {
        // ★ 不依赖执行顺序：本类多个用例会往 g_wActionSpeedIntervals 写值，这里先显式清零两张表
        Array.Clear(FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHit], 0,
                    FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHit].Length);
        Array.Clear(FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amSpell], 0,
                    FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amSpell].Length);

        FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHit][0] = 1234;
        Assert.Equal("1234", IntervalLogic.RowValueText(TAntiPlugActionMode.amHit, 0));
        Assert.Equal("0", IntervalLogic.RowValueText(TAntiPlugActionMode.amSpell, 5));
    }

    // ---------------- ValidateAll（原 :203-219）----------------

    [Fact]
    public void ValidateAll_负数被拒绝_与uFrmHitInterval不同()
    {
        // ★ 差异断言：原 :206 用 `Value <= 0`
        int bad = IntervalLogic.ValidateAll(new[] { "-5" }, out string msg);
        Assert.Equal(0, bad);
        Assert.Equal("输入的数据必须大于0", msg);

        // 对照：uFrmHitInterval 接受 -5
        Assert.Equal(-1, HitIntervalLogic.ValidateAll(new[] { "-5" }, out _));
    }

    [Fact]
    public void ValidateAll_零被拒绝()
    {
        Assert.Equal(0, IntervalLogic.ValidateAll(new[] { "0" }, out _));
        Assert.Equal(1, IntervalLogic.ValidateAll(new[] { "5", "0" }, out _));
    }

    [Fact]
    public void ValidateAll_非数字退化为0被拒绝()
        => Assert.Equal(0, IntervalLogic.ValidateAll(new[] { "abc" }, out _));

    [Fact]
    public void ValidateAll_全正数通过()
        => Assert.Equal(-1, IntervalLogic.ValidateAll(new[] { "1", "200", "99999" }, out _));

    [Fact]
    public void ValidateAll_空集合通过()
        => Assert.Equal(-1, IntervalLogic.ValidateAll(Array.Empty<string>(), out _));

    // ---------------- ComputeBatchValue（原 :260-264）----------------

    [Fact]
    public void ComputeBatchValue_速度0处等于基准值()
        => Assert.Equal(500, IntervalLogic.ComputeBatchValue(200, 500, 10));

    [Fact]
    public void ComputeBatchValue_速度每加1递减()
    {
        Assert.Equal(490, IntervalLogic.ComputeBatchValue(201, 500, 10));
        Assert.Equal(510, IntervalLogic.ComputeBatchValue(199, 500, 10));
    }

    [Fact]
    public void ComputeBatchValue_下限恒为5()
    {
        Assert.Equal(5, IntervalLogic.ComputeBatchValue(400, 100, 10));      // 100 - 10*200 远小于 5
        Assert.Equal(5, IntervalLogic.ComputeBatchValue(201, 10, 10));       // 10 - 10 = 0 → 提升到 5
        Assert.Equal(5, IntervalLogic.ComputeBatchValue(200, 5, 0));         // 恰好 5
        Assert.Equal(6, IntervalLogic.ComputeBatchValue(200, 6, 0));
    }

    [Fact]
    public void ComputeBatchValue_递减量为负时等价于递增()
        => Assert.Equal(700, IntervalLogic.ComputeBatchValue(201, 500, -200));   // I=201: 500 - (-200)*1

    // ---------------- ValidateBatch（原 :253-257）----------------

    [Fact]
    public void ValidateBatch_非正数被拒绝()
    {
        Assert.False(IntervalLogic.ValidateBatch(0, out string msg));
        Assert.Equal("输入的数据必须大于0", msg);
        Assert.False(IntervalLogic.ValidateBatch(-1, out _));
        Assert.True(IntervalLogic.ValidateBatch(1, out _));
    }

    // ---------------- ZeroRowIndex（原 :266-276）----------------

    [Fact]
    public void ZeroRowIndex_为201()
        => Assert.Equal(201, IntervalLogic.ZeroRowIndex);

    // ---------------- SaveIntervalsFile（原 :221-230）----------------

    [Fact]
    public void SaveIntervalsFile_空文件名时整块跳过()
    {
        IntervalLogic.SaveIntervalsFile("", TAntiPlugActionMode.amHit);
        // 不抛异常即通过（原文 `if Length(...) > 0` 才进入）
    }

    [Fact]
    public void SaveIntervalsFile_键名含负号Speed负200到正200()
    {
        FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHit][0] = 111;     // Speed-200
        FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHit][200] = 222;   // Speed0
        FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHit][201] = 333;   // Speed+1 → 'Speed1'
        FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHit][400] = 444;   // Speed200

        IntervalLogic.SaveIntervalsFile(IntervalsFile, TAntiPlugActionMode.amHit);

        Assert.Contains("[Intervals]\r\n", IntervalsText, StringComparison.Ordinal);
        Assert.Contains("Speed-200=111\r\n", IntervalsText, StringComparison.Ordinal);
        Assert.Contains("Speed0=222\r\n", IntervalsText, StringComparison.Ordinal);
        Assert.Contains("Speed1=333\r\n", IntervalsText, StringComparison.Ordinal);
        Assert.Contains("Speed200=444\r\n", IntervalsText, StringComparison.Ordinal);

        // ★ 差异断言：uFrmHitInterval 写的是 Speed0..SpeedN（无负号）
        Assert.DoesNotContain("Speed-200", HitIntervalIniText());
    }

    private string HitIntervalIniText()
    {
        string p = Path.Combine(_dir, "Hit.ini");
        HitIntervalLogic.Save(p, new uint[] { 1 });
        return File.ReadAllText(p, System.Text.Encoding.GetEncoding(936));
    }

    // ---------------- SaveSendSpeedIntervalsToClient（原 :232-242）----------------

    [Fact]
    public void SaveSendSpeedIntervalsToClient_基础模式时整块跳过()
    {
        FormGlobals.g_boSendSpeedIntervalsToClient[(int)TAntiPlugActionMode.amHit] = false;
        IntervalLogic.SaveSendSpeedIntervalsToClient(IniPath, TAntiPlugActionMode.amHit, true);

        Assert.False(FormGlobals.g_boSendSpeedIntervalsToClient[(int)TAntiPlugActionMode.amHit]);   // 未写回
        Assert.False(File.Exists(IniPath));                                                        // 未落盘
    }

    [Fact]
    public void SaveSendSpeedIntervalsToClient_非基础模式写回全局与INI()
    {
        IntervalLogic.SaveSendSpeedIntervalsToClient(IniPath, TAntiPlugActionMode.amWalkToHit, true);

        Assert.True(FormGlobals.g_boSendSpeedIntervalsToClient[(int)TAntiPlugActionMode.amWalkToHit]);
        // 节名来自 AntiPlugActionModeSections[amWalkToHit] = 'WalkToHit'
        Assert.Contains("[WalkToHit]\r\nSendSpeedIntervalsToClient=1\r\n", IniText, StringComparison.Ordinal);
    }

    // ---------------- RunButtonClick（原 :196-247）----------------

    [Fact]
    public void RunButtonClick_校验失败弹窗不落盘()
    {
        var popups = new List<string>();
        bool ok = IntervalLogic.RunButtonClick(TAntiPlugActionMode.amHit, new[] { "1", "-1" },
            (int)TAntiPlugActionMode.amHit, false, IniPath, IntervalsFile, (t, c) => popups.Add(t));

        Assert.False(ok);
        Assert.Single(popups);
        Assert.Equal("输入的数据必须大于0", popups[0]);
    }

    [Fact]
    public void RunButtonClick_成功时写数组与两个INI并重算缓冲()
    {
        FormGlobals.g_sActionIntervalsFileNames[(int)TAntiPlugActionMode.amWalkToHit] = IntervalsFile;

        var texts = new string[RunGateConst.SpeedIntervalsCount];
        for (int i = 0; i < texts.Length; i++) texts[i] = "7";

        bool ok = IntervalLogic.RunButtonClick(TAntiPlugActionMode.amWalkToHit, texts,
            (int)TAntiPlugActionMode.amWalkToHit, true, IniPath, IntervalsFile, (t, c) => { });

        Assert.True(ok);
        Assert.Equal((ushort)7, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amWalkToHit][0]);
        Assert.Equal((ushort)7, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amWalkToHit][400]);
        Assert.Contains("Speed-200=7\r\n", IntervalsText, StringComparison.Ordinal);
        Assert.Contains("SendSpeedIntervalsToClient=1\r\n", IniText, StringComparison.Ordinal);
    }

    // ---------------- RebuildSendToClientSpeedIntervalsText（GateShare.pas:1367）----------------

    [Fact]
    public void Rebuild缓冲_布尔段长度等于模式数()
    {
        Array.Clear(FormGlobals.g_boSendSpeedIntervalsToClient, 0, FormGlobals.g_boSendSpeedIntervalsToClient.Length);
        var buf = FormGlobals.RebuildSendToClientSpeedIntervalsText();
        Assert.Equal(RunGateConst.ActionModeCount, buf.Length);     // 全 False → 只有 27 字节
        Assert.All(buf, b => Assert.Equal(0, b));
    }

    [Fact]
    public void Rebuild缓冲_仅置位模式写入其401个Word()
    {
        // 本类多个用例共享静态数组，先整体清零以保证长度与偏移断言确定
        Array.Clear(FormGlobals.g_boSendSpeedIntervalsToClient, 0, FormGlobals.g_boSendSpeedIntervalsToClient.Length);
        Array.Clear(FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHit], 0,
                    FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHit].Length);
        // 选 amHit（枚举下标 0）→ 它是**唯一**置位模式，故其数据块紧接在 27 字节布尔段之后。
        FormGlobals.g_boSendSpeedIntervalsToClient[(int)TAntiPlugActionMode.amHit] = true;
        FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHit][0] = 0x1234;

        var buf = FormGlobals.RebuildSendToClientSpeedIntervalsText();

        int expected = RunGateConst.ActionModeCount + 1 * (RunGateConst.SpeedIntervalsCount * 2);
        Assert.Equal(expected, buf.Length);

        // 原 GateShare.pas:1374-1384：先整段 Move 布尔数组（27 字节），再从 PB 起逐模式 Move 各 401 个 Word。
        // amHit 是第一个置位模式 → 偏移 = ActionModeCount；Word 按**小端**存放。
        int off = RunGateConst.ActionModeCount;
        Assert.Equal((byte)0x34, buf[off]);
        Assert.Equal((byte)0x12, buf[off + 1]);

        // 401 个 Word 之后就是缓冲末尾（只有一个模式置位）
        Assert.Equal(expected - 1, off + (RunGateConst.SpeedIntervalsCount * 2) - 1);
    }

    [Fact]
    public void Rebuild缓冲_两个模式都置位时顺序按枚举()
    {
        Array.Clear(FormGlobals.g_boSendSpeedIntervalsToClient, 0, FormGlobals.g_boSendSpeedIntervalsToClient.Length);
        FormGlobals.g_boSendSpeedIntervalsToClient[(int)TAntiPlugActionMode.amWalk] = true;
        FormGlobals.g_boSendSpeedIntervalsToClient[(int)TAntiPlugActionMode.amRun] = true;
        var buf = FormGlobals.RebuildSendToClientSpeedIntervalsText();
        Assert.Equal(RunGateConst.ActionModeCount + 2 * (RunGateConst.SpeedIntervalsCount * 2), buf.Length);
    }

    // ---------------- 窗体（DFM 对齐 + 端到端）----------------

    [Fact]
    public void 窗体_DFM属性与控件名对齐()
    {
        using var f = new FrmInterval();

        Assert.Equal("攻击间隔设置", f.Text);                          // DFM: Caption
        Assert.Equal(238, f.ClientSize.Width);                         // DFM: ClientWidth=238
        Assert.Equal(576, f.ClientSize.Height);                        // DFM: ClientHeight=576
        Assert.Equal(new System.Windows.Forms.Padding(5), f.Padding);   // DFM: BorderWidth=5

        Assert.NotNull(f.grpSetting);
        Assert.NotNull(f.grdInterval);
        Assert.NotNull(f.pnlBottom);
        Assert.NotNull(f.btnOK);
        Assert.NotNull(f.grpBatch);
        Assert.NotNull(f.btnAll);
        Assert.NotNull(f.lblSpeed0);
        Assert.NotNull(f.seSpeed0);
        Assert.NotNull(f.lblIncSpeedDecTime);
        Assert.NotNull(f.seIncSpeedDecTime);
        Assert.NotNull(f.chkSendSpeedIntervalsToClient);
        Assert.NotNull(f.btnZero);

        Assert.Equal("间隔设置", f.grpSetting.Text);
        Assert.Equal("确定", f.btnOK.Text);
        Assert.Equal(" 批量设置间隔", f.grpBatch.Text);                  // ★ Caption 前置一个空格，照抄
        Assert.Equal("当速度=0时时间间隔：", f.lblSpeed0.Text);
        Assert.Equal("速度每+1时间间隔减少：", f.lblIncSpeedDecTime.Text);
        Assert.Equal("设置", f.btnAll.Text);
        Assert.Equal("同步间隔设置到客户端", f.chkSendSpeedIntervalsToClient.Text);
        Assert.Equal("0", f.btnZero.Text);                              // DFM: btnZero Caption='0'

        // DFM: grdInterval Left=7 Top=20 Width=210 Height=458 ColCount=2 RowCount=100
        Assert.Equal(2, f.grdInterval.ColumnCount);
        Assert.Equal(100, f.grdInterval.Rows.Count);
        Assert.Equal(210, f.grdInterval.Width);
        Assert.Equal(7, f.grdInterval.Left);
        Assert.Equal(20, f.grdInterval.Top);
        // ★ 不精确断言 Height：grpSetting(Dock=Fill) + pnlBottom(Dock=Bottom) 会触发布局重算，
        //   而 grdInterval 的 Anchors 含 akBottom → 运行期高度由布局决定（DFM 原值 458）。
        Assert.True(f.grdInterval.Height > 0);

        // DFM: btnOK Left=162 Top=67 W=75 H=25；btnZero Left=220 Top=241 W=14 H=21
        Assert.Equal(162, f.btnOK.Left);
        Assert.Equal(241, f.btnZero.Top);
        Assert.Equal(14, f.btnZero.Width);
        Assert.Equal(21, f.btnZero.Height);
    }

    [Fact]
    public void 窗体_FillGrid扩展到402行并填标签()
    {
        FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amWalkToHit][0] = 999;

        using var f = new FrmInterval { FActionMode = TAntiPlugActionMode.amWalkToHit };
        f.FillGrid();

        Assert.Equal(402, f.grdInterval.Rows.Count);                   // 原 :50
        Assert.Equal("走路到攻击间隔设置", f.Text);                      // 原 :88
        Assert.Equal("攻击加速", f.grdInterval.Rows[0].Cells[0].Value);  // 原 :89
        Assert.Equal("间隔检测", f.grdInterval.Rows[0].Cells[1].Value);
        Assert.Equal("-200", f.grdInterval.Rows[1].Cells[0].Value);      // I=0
        Assert.Equal("999", f.grdInterval.Rows[1].Cells[1].Value);
        Assert.Equal("+0", f.grdInterval.Rows[201].Cells[0].Value);      // I=200（见 RowLabel 的 "+0" 说明）
        Assert.Equal("+200", f.grdInterval.Rows[401].Cells[0].Value);    // I=400
        Assert.True(f.SendSpeedIntervalsCheckboxVisible);                // 非基础模式（决策镜像，见字段说明）
    }

    [Fact]
    public void 窗体_FillGrid基础模式隐藏复选框()
    {
        using var f = new FrmInterval { FActionMode = TAntiPlugActionMode.amHit };
        f.FillGrid();

        Assert.Equal("攻击间隔设置", f.Text);
        Assert.Equal("攻击加速", f.grdInterval.Rows[0].Cells[0].Value);
        Assert.False(f.SendSpeedIntervalsCheckboxVisible);               // 原 :60
    }

    [Fact]
    public void 窗体_FillGrid回填复选框Checked仅非基础模式()
    {
        FormGlobals.g_boSendSpeedIntervalsToClient[(int)TAntiPlugActionMode.amWalkToHit] = true;

        using var f = new FrmInterval { FActionMode = TAntiPlugActionMode.amWalkToHit };
        f.FillGrid();
        Assert.True(f.chkSendSpeedIntervalsToClient.Checked);            // 原 :173

        FormGlobals.g_boSendSpeedIntervalsToClient[(int)TAntiPlugActionMode.amHit] = true;
        using var f2 = new FrmInterval { FActionMode = TAntiPlugActionMode.amHit };
        f2.FillGrid();
        Assert.False(f2.chkSendSpeedIntervalsToClient.Checked);          // 基础模式不回填（原 :171 的 if 不成立）
    }

    [Fact]
    public void 窗体_btnAll_Click批量填表()
    {
        using var f = new FrmInterval { FActionMode = TAntiPlugActionMode.amHit };
        f.FillGrid();
        f.seSpeed0.Value = 500;
        f.seIncSpeedDecTime.Value = 10;

        f.btnAll_Click(f, EventArgs.Empty);

        Assert.Equal("500", f.grdInterval.Rows[201].Cells[1].Value);     // I=200 → 速度 0
        Assert.Equal("490", f.grdInterval.Rows[202].Cells[1].Value);     // I=201 → 速度 +1
        Assert.Equal("510", f.grdInterval.Rows[200].Cells[1].Value);     // I=199 → 速度 -1
    }

    [Fact]
    public void 窗体_btnAll_Click基准值非正时弹窗()
    {
        var shown = new List<string>();
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };

            using var f = new FrmInterval { FActionMode = TAntiPlugActionMode.amHit };
            f.FillGrid();
            f.seSpeed0.Value = 0;
            f.btnAll_Click(f, EventArgs.Empty);

            Assert.Single(shown);
            Assert.Equal("输入的数据必须大于0", shown[0]);       // 原 :255
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_btnZero_Click与FormShow把行游标归到201()
    {
        using var f = new FrmInterval { FActionMode = TAntiPlugActionMode.amHit };
        f.FillGrid();

        var ex = Record.Exception(() => { f.btnZero_Click(f, EventArgs.Empty); f.FormShow(f, EventArgs.Empty); });
        Assert.Null(ex);
        Assert.Equal(IntervalLogic.ZeroRowIndex,
            f.grdInterval.CurrentCell != null ? f.grdInterval.CurrentCell.RowIndex : IntervalLogic.ZeroRowIndex);
    }

    [Fact]
    public void 窗体_btnOK_Click校验失败弹窗且不写数组()
    {
        var shown = new List<string>();
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };
            FormGlobals.g_sIniFileName = IniPath;

            using var f = new FrmInterval { FActionMode = TAntiPlugActionMode.amHit };
            f.FillGrid();
            f.grdInterval.Rows[1].Cells[1].Value = "0";     // 第一行数据置 0 → 非法
            f.btnOK_Click(f, EventArgs.Empty);

            Assert.Single(shown);
            Assert.Equal("输入的数据必须大于0", shown[0]);   // 原 :208
            Assert.Equal(System.Windows.Forms.DialogResult.None, f.DialogResult);
            Assert.Equal((ushort)0, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amHit][0]);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_btnOK_Click成功写数组与INI()
    {
        FormGlobals.g_sIniFileName = IniPath;
        FormGlobals.g_sActionIntervalsFileNames[(int)TAntiPlugActionMode.amSpell] = IntervalsFile;

        using var f = new FrmInterval { FActionMode = TAntiPlugActionMode.amSpell };
        f.FillGrid();
        for (int r = 1; r <= 401; r++) f.grdInterval.Rows[r].Cells[1].Value = "12";

        f.btnOK_Click(f, EventArgs.Empty);

        Assert.Equal(System.Windows.Forms.DialogResult.OK, f.DialogResult);
        Assert.Equal((ushort)12, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amSpell][0]);
        Assert.Equal((ushort)12, FormGlobals.g_wActionSpeedIntervals[(int)TAntiPlugActionMode.amSpell][400]);
        Assert.Contains("Speed-200=12\r\n", IntervalsText, StringComparison.Ordinal);
    }

    [Fact]
    public void ShowFrmInterval_不支持的模式直接返回False_不建窗体()
    {
        // amTurn 不在 18 种模式内 → 原 :47 `if not ActionModeUseSpeedIntervals(ActionMode) then Exit`（Result 保持 False）
        bool result = IntervalUnit.ShowFrmInterval(TAntiPlugActionMode.amTurn);
        Assert.False(result);
    }

    [Fact]
    public void 窗体_DFM原RowCount为100且FillGrid后为402()
    {
        using var f = new FrmInterval();
        Assert.Equal(100, f.grdInterval.Rows.Count);       // DFM: RowCount=100（含固定行的近似）
        f.FActionMode = TAntiPlugActionMode.amHit;
        f.FillGrid();
        Assert.Equal(IntervalLogic.GridRowCount, f.grdInterval.Rows.Count);
    }
}

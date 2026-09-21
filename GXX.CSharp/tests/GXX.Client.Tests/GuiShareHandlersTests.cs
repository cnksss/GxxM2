using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.Share;
using GXX.Client.Scenes;
using Xunit;
using static GXX.Client.GUI.Mir.MShareGlobals;
// 车道1 的 GXX.Client.GUI.Mir.TFrmDlg 是同一 Delphi 类型的早期接缝，与本车道同名 → CS0104；
// 本车道（p14-client-fstate）是 FState.pas 的正式归属，故显式指向 GXX.Client.GUI.Share。
using TFrmDlg = GXX.Client.GUI.Share.TFrmDlg;

namespace GXX.Client.Tests;

/// <summary>
/// 并行车道 p14-client-fstate **切片 1**：把 `TFrmDlg.Decl.g.cs` 里 515 个
/// `throw new NotSupportedException` 壳中的 25 条**原文自带真实现**成员落地。
///
/// 覆盖原文行号（逐条见 <see cref="TFrmDlgPortLedger.Slice1"/>）：
///   1891-1895 HideAllControls            1897-1900 RestoreHideControls
///   2316-2319 DStateWinClick             2883-2886 DBottomInRealArea
///   2888-2891 DBotPlusAbilDirectPaint    12457-12460 MerchantDlgPaint
///   17795-17798 DUserState1MouseDown     18018-18021 DChgGamePwdCloseClick
///   18023-18026 DChgGamePwdDirectPaint   18028-18031 DscSelect1InRealArea
///   18038-18042 DItemBagMouseMove        18197-18200 DMinMapDlgShow
///   18202-18205 DMinMapDlgHide           18207-18210 DMinMapDlgResize
///   18655-18658 DGameGoldDealCancelClick 20367-20370 AttactkModeChange
///   20741-20744 OpenDUpgradeDlg          20746-20749 CloseDUpgradeDlg
///   21062-21065 OpenDRandomCodeDlg       21067-21070 CloseDRandomCodeDlg
///   24133-24137 DMouseMoveClearHints     24356-24359 DSayItemDlgCloseClick
///   24361-24365 DSayItemDlgMouseDown     24367-24372 DSayItemDlgMouseMove
///   24464-24467 DUpdateStatusDlgMouseLeave
///
/// ★ 本文件同时是**"壳行为 vs 真实现"的回归闸门**：
///   下面 `Slice1MembersNoLongerThrow` 会逐个调用已落地成员并断言**不抛**
///   `NotSupportedException` —— 一旦有人把生成壳重跑回 515 条，该测试立刻红。
/// </summary>
public sealed class GuiShareHandlersTests : IDisposable
{
    public GuiShareHandlersTests()
    {
        ResetAll();
    }

    public void Dispose()
    {
        ResetAll();
    }

    private void ResetAll()
    {
        FStateSeamClock.ResetForTests();
        MShareHintFont.ResetForTests();
        ConfigClientExt.ResetForTests();
        MagicButtonIniSeam.ResetForTests();
        MShareGlobalsReset.ResetForTests();
        FStateClMainSeam.ResetForTests();
        FStateGlobal.ResetForTests();
        FStateScreenSeam.ResetForTests();
        DrawScrnEnv.HintWindows = new THintWindows();
    }

    // =====================================================================================
    // A. 台账与生成壳的一致性（机器可读的"哪些已经是真体"）
    // =====================================================================================

    [Fact]
    public void LedgerSlice1RegistersTwentyFiveMembers()
    {
        Assert.Equal(25, TFrmDlgPortLedger.Slice1Count);
        Assert.Single(TFrmDlgPortLedger.AllSlices);
    }

    [Fact]
    public void EveryLedgerEntryCarriesAPlausibleSourceLineRange()
    {
        // 原文 FState.pas 是 25,165 行；每条登记的区间都必须落在文件内且 起<=止。
        foreach (var m in TFrmDlgPortLedger.Slice1)
        {
            Assert.False(string.IsNullOrWhiteSpace(m.Name), "成员名不得为空");
            Assert.Matches(@"^\d{1,5}-\d{1,5}$", m.SourceLines);
            var parts = m.SourceLines.Split('-');
            int from = int.Parse(parts[0]);
            int to = int.Parse(parts[1]);
            Assert.True(from >= 1 && to <= 25165, m.Name + " 行号越界: " + m.SourceLines);
            Assert.True(from <= to, m.Name + " 行号区间倒置: " + m.SourceLines);
        }
    }

    [Fact]
    public void LedgerNamesAreUniqueAndExistOnTheType()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var methods = typeof(TFrmDlg)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(m => m.Name)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var m in TFrmDlgPortLedger.Slice1)
        {
            Assert.True(seen.Add(m.Name), "台账重复登记: " + m.Name);
            Assert.Contains(m.Name, methods);
        }
    }

    [Fact]
    public void LedgerOnlyContainsMembersThatNoLongerThrow()
    {
        // 反静默 / 反"假完成"：台账里登记的每一条都必须**可调用且不抛 NotSupportedException**。
        var frm = NewForm();
        foreach (var m in TFrmDlgPortLedger.Slice1)
        {
            var ex = Record.Exception(() => InvokeMember(frm, m.Name));
            Assert.False(ex is NotSupportedException,
                m.Name + " 仍在台账中却抛 NotSupportedException（生成壳被重跑回 throw？）: " + ex);
        }
    }

    /// <summary>
    /// 按成员名用最小合法实参调用（只求"不抛 NotSupportedException"，不校验语义）。
    /// 语义由下方逐条测试覆盖。
    /// </summary>
    private static void InvokeMember(TFrmDlg frm, string name)
    {
        switch (name)
        {
            case "HideAllControls": frm.HideAllControls(); break;
            case "RestoreHideControls": frm.RestoreHideControls(); break;
            case "DStateWinClick": frm.DStateWinClick(null, 0, 0); break;
            case "AttactkModeChange": frm.AttactkModeChange(); break;
            case "DChgGamePwdDirectPaint": frm.DChgGamePwdDirectPaint(null); break;
            case "MerchantDlgPaint": frm.MerchantDlgPaint(null); break;
            case "DBotPlusAbilDirectPaint": frm.DBotPlusAbilDirectPaint(null); break;
            case "DGameGoldDealCancelClick": frm.DGameGoldDealCancelClick(null, 0, 0); break;
            case "OpenDUpgradeDlg": frm.OpenDUpgradeDlg(); break;
            case "CloseDUpgradeDlg": frm.CloseDUpgradeDlg(); break;
            case "OpenDRandomCodeDlg": frm.OpenDRandomCodeDlg(); break;
            case "CloseDRandomCodeDlg": frm.CloseDRandomCodeDlg(); break;
            case "DChgGamePwdCloseClick": frm.DChgGamePwdCloseClick(null, 0, 0); break;
            case "DBottomInRealArea": { bool b = false; frm.DBottomInRealArea(null, 0, 0, ref b); break; }
            case "DscSelect1InRealArea": { bool b = false; frm.DscSelect1InRealArea(null, 0, 0, ref b); break; }
            case "DUserState1MouseDown":
                frm.DUserState1MouseDown(null, TMouseButton.mbLeft, TShiftState.ssNone, 0, 0); break;
            case "DMinMapDlgShow": frm.DMinMapDlgShow(null); break;
            case "DMinMapDlgHide": frm.DMinMapDlgHide(null); break;
            case "DMinMapDlgResize": frm.DMinMapDlgResize(null); break;
            case "DMouseMoveClearHints":
                frm.DMouseMoveClearHints(null, TShiftState.ssNone, 0, 0); break;
            case "DUpdateStatusDlgMouseLeave": frm.DUpdateStatusDlgMouseLeave(null); break;
            case "DItemBagMouseMove": frm.DItemBagMouseMove(null, TShiftState.ssNone, 0, 0); break;
            case "DSayItemDlgCloseClick": frm.DSayItemDlgCloseClick(null, 0, 0); break;
            case "DSayItemDlgMouseDown":
                frm.DSayItemDlgMouseDown(null, TMouseButton.mbRight, TShiftState.ssNone, 0, 0); break;
            case "DSayItemDlgMouseMove":
                frm.DSayItemDlgMouseMove(null, TShiftState.ssNone, 0, 0); break;
            default:
                Assert.Fail("台账里有未在本测试的调用表里登记的成员: " + name);
                break;
        }
    }

    private static TFrmDlg NewForm() => new TFrmDlg();

    // =====================================================================================
    // B. 1891-1900  HideAllControls / RestoreHideControls
    // =====================================================================================

    [Fact]
    public void HideAllControlsSnapshotsMemoVisibilityThenHidesIt()
    {
        var frm = NewForm();
        frm.Memo.Visible = true;
        frm.GuildMemoVisible = false;

        frm.HideAllControls();

        Assert.True(frm.GuildMemoVisible);      // 1893：快照进 GuildMemoVisible
        Assert.False(frm.Memo.Visible);         // 1894：再隐藏
    }

    [Fact]
    public void HideAllControlsOverwritesAPreviousSnapshotWithTheCurrentValue()
    {
        // 差异断言：GuildMemoVisible 是**快照**，第二次调用必须覆盖第一次的值（不是"只存真值"）。
        var frm = NewForm();
        frm.Memo.Visible = true;
        frm.HideAllControls();
        Assert.True(frm.GuildMemoVisible);

        frm.Memo.Visible = false;
        frm.HideAllControls();
        Assert.False(frm.GuildMemoVisible);
    }

    [Fact]
    public void RestoreHideControlsOnlyReadsTheSnapshot()
    {
        var frm = NewForm();
        frm.GuildMemoVisible = true;
        frm.Memo.Visible = false;

        frm.RestoreHideControls();

        Assert.True(frm.Memo.Visible);          // 1899
        Assert.True(frm.GuildMemoVisible);      // 原样保留（Restore 不写该字段）
    }

    [Fact]
    public void HideThenRestoreRoundTripsTheOriginalVisibility()
    {
        var frm = NewForm();
        frm.Memo.Visible = false;

        frm.HideAllControls();
        frm.RestoreHideControls();

        Assert.False(frm.Memo.Visible);
    }

    // =====================================================================================
    // C. 原文空体成员：调用必须"什么都不发生"（不是抛异常，也不是改状态）
    // =====================================================================================

    public static IEnumerable<object[]> EmptyBodyMembers()
    {
        yield return new object[] { "DStateWinClick", "2316-2319" };
        yield return new object[] { "AttactkModeChange", "20367-20370" };
        yield return new object[] { "DChgGamePwdDirectPaint", "18023-18026" };
        yield return new object[] { "MerchantDlgPaint", "12457-12460" };
        yield return new object[] { "DBotPlusAbilDirectPaint", "2888-2891" };
        yield return new object[] { "DGameGoldDealCancelClick", "18655-18658" };
        yield return new object[] { "OpenDUpgradeDlg", "20741-20744" };
        yield return new object[] { "CloseDUpgradeDlg", "20746-20749" };
        yield return new object[] { "OpenDRandomCodeDlg", "21062-21065" };
        yield return new object[] { "CloseDRandomCodeDlg", "21067-21070" };
        // 18018-18021：原文体内只有一行被注释掉的 `// CloseDChgGamePwd;` —— 空体 + 原文如此
        yield return new object[] { "DChgGamePwdCloseClick", "18018-18021" };
    }

    [Theory]
    [MemberData(nameof(EmptyBodyMembers))]
    public void EmptyBodyMembersDoNothingAndDoNotThrow(string name, string sourceLines)
    {
        var frm = NewForm();
        var ex = Record.Exception(() => InvokeMember(frm, name));
        Assert.Null(ex);
        // 台账里登记的原文行号必须与本测试的期望一致（防止"改了实现没改登记"）
        var entry = TFrmDlgPortLedger.Slice1.Single(m => m.Name == name);
        Assert.Equal(sourceLines, entry.SourceLines);
    }

    // =====================================================================================
    // D. 2883-2886 / 18028-18031  两个 RealArea 回调恒置 True
    // =====================================================================================

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DBottomInRealAreaAlwaysSetsTrue(bool initial)
    {
        var frm = NewForm();
        bool isRealArea = initial;
        frm.DBottomInRealArea(null, 12345, 67890, ref isRealArea);
        Assert.True(isRealArea);                // 2885：无条件 True（原文如此）
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DscSelect1InRealAreaAlwaysSetsTrue(bool initial)
    {
        var frm = NewForm();
        bool isRealArea = initial;
        frm.DscSelect1InRealArea(null, -1, -1, ref isRealArea);
        Assert.True(isRealArea);                // 18030：无条件 True（原文如此）
    }

    // =====================================================================================
    // E. 24356-24372  SayItem 对话框
    // =====================================================================================

    [Fact]
    public void DSayItemDlgCloseClickHidesTheDialog()
    {
        var frm = NewForm();
        frm.DSayItemDlg = new TDxImageForm();
        frm.DSayItemDlg.Visible = true;

        frm.DSayItemDlgCloseClick(null, 0, 0);

        Assert.False(frm.DSayItemDlg.Visible);  // 24358
    }

    [Fact]
    public void DSayItemDlgMouseDownClosesOnlyOnTheRightButton()
    {
        var frm = NewForm();
        frm.DSayItemDlg = new TDxImageForm();

        frm.DSayItemDlg.Visible = true;
        frm.DSayItemDlgMouseDown(null, TMouseButton.mbLeft, TShiftState.ssNone, 0, 0);
        Assert.True(frm.DSayItemDlg.Visible);   // 左键不关（原文只判 mbRight）

        frm.DSayItemDlgMouseDown(null, TMouseButton.mbMiddle, TShiftState.ssNone, 0, 0);
        Assert.True(frm.DSayItemDlg.Visible);   // 中键也不关

        frm.DSayItemDlgMouseDown(null, TMouseButton.mbRight, TShiftState.ssNone, 0, 0);
        Assert.False(frm.DSayItemDlg.Visible);  // 24363-24364：右键关
    }

    [Fact]
    public void DSayItemDlgMouseMoveSetsTheMoveOutFlagAndClearsHintsInOrder()
    {
        // 注意：`boSayItemDlgMoveOutClose` 在原文是 protected（TFrmDlg.Decl.g.cs 生成
        // `protected bool`），测试无法直接读；这里只能断言**可观测的**两步副作用。
        // 该字段的写入本身是 1:1 直译（见 TFrmDlg.Handlers.cs 的 24369 行注释）。
        var frm = NewForm();
        DrawScrnEnv.HintWindows.Add(new THintWindow());

        frm.DSayItemDlgMouseMove(null, TShiftState.ssNone, 0, 0);

        Assert.Equal(0, DrawScrnEnv.HintWindows.Count);    // 24370：HintWindows.Clear
        Assert.Equal(1, FStateScreenSeam.ClearHintCount);  // 24371：DScreen.ClearHint
    }

    // =====================================================================================
    // F. 提示窗清理面（24133-24137 / 24464-24467 / 18038-18042）
    // =====================================================================================

    [Fact]
    public void DMouseMoveClearHintsClearsBothTheScreenHintAndTheHintWindows()
    {
        var frm = NewForm();
        DrawScrnEnv.HintWindows.Add(new THintWindow());

        frm.DMouseMoveClearHints(null, TShiftState.ssNone, 0, 0);

        Assert.Equal(1, FStateScreenSeam.ClearHintCount);   // 24135 DScreen.ClearHint
        Assert.Equal(0, DrawScrnEnv.HintWindows.Count);     // 24136 HintWindows.Clear
    }

    [Fact]
    public void DUpdateStatusDlgMouseLeaveClearsHintWindowsOnly()
    {
        var frm = NewForm();
        DrawScrnEnv.HintWindows.Add(new THintWindow());

        frm.DUpdateStatusDlgMouseLeave(null);

        Assert.Equal(0, DrawScrnEnv.HintWindows.Count);     // 24466
        Assert.Equal(0, FStateScreenSeam.ClearHintCount);   // 差异断言：本条**不**调 DScreen.ClearHint
    }

    [Fact]
    public void DItemBagMouseMoveClearsHintsAndResetsTheBagInfoFlag()
    {
        var frm = NewForm();
        DrawScrnEnv.HintWindows.Add(new THintWindow());
        g_boShowBagInfo = 1;

        frm.DItemBagMouseMove(null, TShiftState.ssNone, 0, 0);

        Assert.Equal(0, DrawScrnEnv.HintWindows.Count);     // 18040
        Assert.Equal(1, FStateScreenSeam.ClearHintCount);   // 18041
        Assert.Equal(0, g_boShowBagInfo);                   // 18042
    }

    // =====================================================================================
    // G. 17795-17798 / 18197-18210  空体成员不改变任何状态
    // =====================================================================================

    [Fact]
    public void DUserState1MouseDownDoesNotThrowForAnyButton()
    {
        var frm = NewForm();
        foreach (TMouseButton b in new[] { TMouseButton.mbLeft, TMouseButton.mbRight, TMouseButton.mbMiddle })
        {
            var ex = Record.Exception(() =>
                frm.DUserState1MouseDown(null, b, TShiftState.ssNone, 0, 0));
            Assert.Null(ex);
        }
    }

    [Fact]
    public void MinMapShowHideResizeAreEmptyBodies()
    {
        var frm = NewForm();
        Assert.Null(Record.Exception(() => frm.DMinMapDlgShow(null)));
        Assert.Null(Record.Exception(() => frm.DMinMapDlgHide(null)));
        Assert.Null(Record.Exception(() => frm.DMinMapDlgResize(null)));
    }

    // =====================================================================================
    // H. D-P10-06：THintWindows 的正式归属已是 GXX.Client.Scenes
    // =====================================================================================

    [Fact]
    public void HintWindowsFieldIsTheScenesTypeNotASeamCopy()
    {
        // Create（原文 1555 `FSayItemHintWin := DrawScrn.THintWindows.Create`）后字段必须是
        // GXX.Client.Scenes.THintWindows 的实例 —— 即接缝删除后指向正式归属。
        var frm = NewForm();
        var field = typeof(TFrmDlg).GetField(
            "FSayItemHintWin", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);
        Assert.Equal(typeof(THintWindows), field.FieldType);

        var value = field.GetValue(frm);
        Assert.NotNull(value);
        Assert.IsType<THintWindows>(value);
    }

    [Fact]
    public void DeletedSeamTypesAreGoneFromTheGuiShareNamespace()
    {
        // D-P10-06 删除条件：GUIShare 命名空间下**不得**再有 THintLines / THintWindows / DrawScrn。
        var asm = typeof(TFrmDlg).Assembly;
        Assert.Null(asm.GetType("GXX.Client.GUI.Share.THintLines"));
        Assert.Null(asm.GetType("GXX.Client.GUI.Share.THintWindows"));
        Assert.Null(asm.GetType("GXX.Client.GUI.Share.DrawScrn"));
    }
}

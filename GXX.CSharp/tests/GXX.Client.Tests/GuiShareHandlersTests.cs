using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.Share;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using GXX.Core.Util;
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
    /// <summary>注入的假时钟（tick 毫秒）。切片 4 的 tick 守卫族靠它精确落点。</summary>
    private uint _fakeTick;

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
        FStateResStrSeam.ResetForTests();
        DrawScrnEnv.HintWindows = new THintWindows();
        _fakeTick = 10_000;
        FStateSeamClock.NowHandler = () => _fakeTick;
    }

    // =====================================================================================
    // A. 台账与生成壳的一致性（机器可读的"哪些已经是真体"）
    // =====================================================================================

    [Fact]
    public void LedgerSlice1RegistersTwentyFiveMembers()
    {
        Assert.Equal(25, TFrmDlgPortLedger.Slice1Count);
    }

    [Fact]
    public void LedgerSlice2RegistersTwentyFiveMembers()
    {
        Assert.Equal(25, TFrmDlgPortLedger.Slice2Count);
    }

    [Fact]
    public void LedgerAllSlicesHaveNoDuplicateNames()
    {
        var all = TFrmDlgPortLedger.AllSlices.SelectMany(s => s).Select(m => m.Name).ToList();
        Assert.Equal(all.Count, all.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Equal(TFrmDlgPortLedger.LaneCount, all.Count);
    }

    [Fact]
    public void EveryLedgerEntryCarriesAPlausibleSourceLineRange()
    {
        // 原文 FState.pas 是 25,165 行；每条登记的区间都必须落在文件内且 起<=止。
        foreach (var m in TFrmDlgPortLedger.AllSlices.SelectMany(s => s))
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

        foreach (var m in TFrmDlgPortLedger.AllSlices.SelectMany(s => s))
        {
            Assert.True(seen.Add(m.Name), "台账重复登记: " + m.Name);
            Assert.Contains(m.Name, methods);
        }
    }

    /// <summary>本车道（p14）落地的全部成员名（切片 1 + 切片 2）。</summary>
    private static IEnumerable<TFrmDlgPortLedger.PortedMember> LaneEntries()
        => TFrmDlgPortLedger.AllSlices.SelectMany(s => s);

    [Fact]
    public void LedgerMembersAreNoLongerShellsInTheGeneratedFile()
    {
        // ★ 反"假完成"闸门（本测试是本车道对"515 个 throw 壳"最直接的锁）：
        //   若某个成员**仍然**是生成壳（体里只有 throw new NotSupportedException），
        //   它的方法体 IL 会包含 `newobj NotSupportedException(string)`。
        //   真实现（无论是不是转发）都**不会**构造该异常。
        //   因此这一条能同时覆盖"纯实现"与"转发给仍未移植的方法"两种形态。
        var frm = NewForm();
        var accidentalShells = new List<string>();
        foreach (var m in LaneEntries())
        {
            var method = FindMethod(frm, m.Name);
            Assert.False(method == null, "台账成员在类型上找不到: " + m.Name);
            if (BodyConstructsNotSupportedException(method))
                accidentalShells.Add(m.Name + " (FState.pas:" + m.SourceLines + ")");
        }
        Assert.Empty(accidentalShells);
    }

    private static System.Reflection.MethodInfo FindMethod(TFrmDlg frm, string name)
        => frm.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.Ordinal));

    /// <summary>
    /// 方法体里是否**新建**了 NotSupportedException（= 仍是"未移植壳"的身体特征）。
    /// 注意：转发形态（`CloseDSellDlg();`）**不会**命中 —— 它只是 call 另一个方法。
    /// </summary>
    private static bool BodyConstructsNotSupportedException(System.Reflection.MethodInfo method)
    {
        var body = method.GetMethodBody();
        if (body == null) return false;
        var il = body.GetILAsByteArray();
        if (il == null) return false;
        for (int i = 0; i < il.Length; i++)
        {
            // 0x73 = newobj <ctor token>
            if (il[i] != 0x73) continue;
            if (i + 4 >= il.Length) break;
            int token = BitConverter.ToInt32(il, i + 1);
            try
            {
                var m = method.Module.ResolveMethod(token, method.DeclaringType.GetGenericArguments(),
                    method.GetGenericArguments());
                if (m != null && m.DeclaringType == typeof(NotSupportedException))
                    return true;
            }
            catch (ArgumentException)
            {
                // 无法解析的 token：不是我们关心的构造，继续扫
            }
        }
        return false;
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
    // 切片 2：17854-17866  公会成员列表翻行（本单元字段，可直接断言）
    // =====================================================================================

    [Fact]
    public void DGDUpClickDecrementsByThreeAndClampsAtZero()
    {
        var frm = NewForm();
        frm.GuildTopLine = 10;

        frm.DGDUpClick(null, 0, 0);
        Assert.Equal(7, frm.GuildTopLine);      // 17857：Dec(..., 3)

        frm.GuildTopLine = 1;
        frm.DGDUpClick(null, 0, 0);
        Assert.Equal(0, frm.GuildTopLine);      // 1 - 3 = -2 → 17859 夹回 0
    }

    [Fact]
    public void DGDUpClickDoesNothingWhenAlreadyAtZero()
    {
        var frm = NewForm();
        frm.GuildTopLine = 0;

        frm.DGDUpClick(null, 0, 0);

        Assert.Equal(0, frm.GuildTopLine);      // 17856 的 > 0 不成立
    }

    [Fact]
    public void DGDDownClickUsesStrictLessThanOnTopLinePlusTwelve()
    {
        var frm = NewForm();
        frm.GuildStrs = new TStringList();
        for (int i = 0; i < 12; i++) frm.GuildStrs.Add("m" + i);   // Count = 12

        // 判据 `GuildTopLine + 12 < Count`：TopLine=0 ⇒ 12 < 12 = False ⇒ 不动
        frm.GuildTopLine = 0;
        frm.DGDDownClick(null, 0, 0);
        Assert.Equal(0, frm.GuildTopLine);      // 差异断言：**严格小于**，不是 <=

        // Count = 16 ⇒ 0 + 12 < 16 = True ⇒ +3
        frm.GuildStrs.Add("m12");
        frm.GuildStrs.Add("m13");
        frm.GuildStrs.Add("m14");
        frm.GuildStrs.Add("m15");
        frm.DGDDownClick(null, 0, 0);
        Assert.Equal(3, frm.GuildTopLine);      // 17865：Inc(..., 3)
    }

    // =====================================================================================
    // 切片 2：17906-17926  公会编辑入口（原文先把资源串解码进 GuildEditHint）
    // =====================================================================================

    [Fact]
    public void DGDEditNoticeClickStoresTheDecodedResourceStringThenForwards()
    {
        var frm = NewForm();
        FStateResStrSeam.SGuildEditNotice = "SGuildEditNotice";
        FStateResStrSeam.DecodeResStr = s => "<" + s + ">";

        // 17909 的 OpenDGuildEditNoticeDlg 尚未移植 ⇒ 转发后仍抛（**抛点在被转发方**）
        var ex = Assert.Throws<NotSupportedException>(() => frm.DGDEditNoticeClick(null, 0, 0));
        Assert.Contains("OpenDGuildEditNoticeDlg", ex.Message);

        // 但 17908 的赋值**已经发生**（顺序：先写 GuildEditHint，再转发）
        Assert.Equal("<SGuildEditNotice>", frm.GuildEditHint);
    }

    [Fact]
    public void DGDEditGradeClickStoresTheDecodedGradeHintThenForwards()
    {
        var frm = NewForm();
        FStateResStrSeam.SGuildEditGradeHint = "SGuildEditGradeHint";
        FStateResStrSeam.DecodeResStr = s => "[" + s + "]";

        var ex = Assert.Throws<NotSupportedException>(() => frm.DGDEditGradeClick(null, 0, 0));
        Assert.Contains("OpenGuildEditGradeDlg", ex.Message);
        Assert.Equal("[SGuildEditGradeHint]", frm.GuildEditHint);
    }

    // =====================================================================================
    // 切片 2：24206-24314  卧龙对话框关闭（直接置 Visible）
    // =====================================================================================

    [Fact]
    public void DLieDragonCloseClickHidesTheDialog()
    {
        var frm = NewForm();
        frm.DLieDragon = new TDxImageForm();
        frm.DLieDragon.Visible = true;

        frm.DLieDragonCloseClick(null, 0, 0);

        Assert.False(frm.DLieDragon.Visible);   // 24208
    }

    [Fact]
    public void DLieDragonNpcCloseClickHidesTheNpcDialog()
    {
        var frm = NewForm();
        frm.DLieDragonNpc = new TDxImageForm();
        frm.DLieDragonNpc.Visible = true;

        frm.DLieDragonNpcCloseClick(null, 0, 0);

        Assert.False(frm.DLieDragonNpc.Visible); // 24313
    }

    [Fact]
    public void DLieDragonCloseDoesNotTouchTheOtherDialog()
    {
        // 差异断言：两条只动各自的控件（原文没有交叉）。
        var frm = NewForm();
        frm.DLieDragon = new TDxImageForm();
        frm.DLieDragonNpc = new TDxImageForm();
        frm.DLieDragon.Visible = true;
        frm.DLieDragonNpc.Visible = true;

        frm.DLieDragonCloseClick(null, 0, 0);

        Assert.False(frm.DLieDragon.Visible);
        Assert.True(frm.DLieDragonNpc.Visible);
    }

    // =====================================================================================
    // 切片 2：关闭/打开转发族 —— 转发体必须真的转发（断言**抛点行号**是每个目标自己的）
    // =====================================================================================

    public static IEnumerable<object[]> ForwarderMembers()
    {
        // 成员名, 被转发的方法名
        yield return new object[] { "DSellDlgCloseClick", "CloseDSellDlg" };
        yield return new object[] { "DMenuCloseClick", "CloseDMenuDlg" };
        yield return new object[] { "DCloseStateClick", "CloseDStateWinDlg" };
        yield return new object[] { "DCloseBagClick", "CloseDItemBagDlg" };
        yield return new object[] { "DBotRankClick", "OpenDRankingDlg" };
        yield return new object[] { "DBotWhisperClick", "OpenDWhisperDlg" };
        yield return new object[] { "DMissionDlgClick", "OpenDMissionDlg" };
        yield return new object[] { "DMissionDlgCloseClick", "CloseDMissionDlg" };
        yield return new object[] { "DOpenShopClick", "OpenDShopDlg" };
        yield return new object[] { "DBotRankingCloseClick", "CloseDRankingDlg" };
        yield return new object[] { "DGrpDlgCloseClick", "CloseDGroupDlg" };
        yield return new object[] { "DFrdCloseClick", "CloseDFriendDlg" };
        yield return new object[] { "DMyHeroStateCloseClick", "CloseDHeroStateWinDlg" };
        yield return new object[] { "DMyHeroBagCloseClick", "CloseDHeroItemBagDlg" };
        yield return new object[] { "DKsOkClick", "CloseDKeySelDlg" };
        yield return new object[] { "DCloseUS1Click", "CloseDUserState1Dlg" };
        yield return new object[] { "DNewGuildDlgCloseClick", "CloseDGuildDlg_New" };
        yield return new object[] { "DNewGuildNoticeClick", "OpenDGuildEditNoticeDlg_New" };
    }

    [Theory]
    [MemberData(nameof(ForwarderMembers))]
    public void ForwardersDelegateToTheOriginalTarget(string member, string target)
    {
        var frm = NewForm();
        var ex = Assert.Throws<NotSupportedException>(() => InvokeLaneMember(frm, member));
        // 转发的证据：异常消息里带的是**被转发方法自己**的 "TFrmDlg.<target>"
        Assert.Contains("TFrmDlg." + target + ":", ex.Message);
    }

    /// <summary>切片 2 的转发族调用表（参数一律给最小合法值）。</summary>
    private static void InvokeLaneMember(TFrmDlg frm, string name)
    {
        switch (name)
        {
            case "DSellDlgCloseClick": frm.DSellDlgCloseClick(null, 0, 0); break;
            case "DMenuCloseClick": frm.DMenuCloseClick(null, 0, 0); break;
            case "DKsOkClick": frm.DKsOkClick(null, 0, 0); break;
            case "DCloseUS1Click": frm.DCloseUS1Click(null, 0, 0); break;
            case "DNewGuildDlgCloseClick": frm.DNewGuildDlgCloseClick(null, 0, 0); break;
            case "DNewGuildNoticeClick": frm.DNewGuildNoticeClick(null, 0, 0); break;
            case "DCloseStateClick": frm.DCloseStateClick(null, 0, 0); break;
            case "DCloseBagClick": frm.DCloseBagClick(null, 0, 0); break;
            case "DBotRankClick": frm.DBotRankClick(null, 0, 0); break;
            case "DBotWhisperClick": frm.DBotWhisperClick(null, 0, 0); break;
            case "DMissionDlgClick": frm.DMissionDlgClick(null, 0, 0); break;
            case "DMissionDlgCloseClick": frm.DMissionDlgCloseClick(null, 0, 0); break;
            case "DOpenShopClick": frm.DOpenShopClick(null, 0, 0); break;
            case "DBotRankingCloseClick": frm.DBotRankingCloseClick(null, 0, 0); break;
            case "DFrdCloseClick": frm.DFrdCloseClick(null, 0, 0); break;
            case "DGrpDlgCloseClick": frm.DGrpDlgCloseClick(null, 0, 0); break;
            case "DMyHeroStateCloseClick": frm.DMyHeroStateCloseClick(null, 0, 0); break;
            case "DMyHeroBagCloseClick": frm.DMyHeroBagCloseClick(null, 0, 0); break;
            default:
                Assert.Fail("未登记的切片 2 成员: " + name);
                break;
        }
    }

    // =====================================================================================
    // 切片 3：B-2 授权后解锁的四条 + 骑马两条（frmMain / Actor 接缝注入）
    // =====================================================================================

    [Fact]
    public void LedgerSlice3RegistersSixMembers()
    {
        Assert.Equal(6, TFrmDlgPortLedger.Slice3Count);
    }

    [Fact]
    public void DWebClickNavigatesToTheConfiguredHomePage()
    {
        var frm = NewForm();
        FStateClMainSeam.sHomePage = "http://example.invalid/index.html";
        string seen = null;
        FStateClMainSeam.NavigateHandler = url => seen = url;

        frm.DWebClick(null, 0, 0);

        Assert.Equal("http://example.invalid/index.html", seen);   // 20579
    }

    [Fact]
    public void DWebClickUsesTheMirroredDefaultHomePageWhenNotOverridden()
    {
        var frm = NewForm();
        string seen = null;
        FStateClMainSeam.NavigateHandler = url => seen = url;

        frm.DWebClick(null, 0, 0);

        // 默认值与 M2 端 M2Config.sHomePage 一致（M2Config.ClientConf.cs:128）
        Assert.Equal("http://www.gxxm2.com", seen);
    }

    [Fact]
    public void DActionLogClickForwardsExactlyOnce()
    {
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.SendDActionLogClickHandler = () => calls++;

        frm.DActionLogClick(null, 0, 0);

        Assert.Equal(1, calls);                                     // 20584
    }

    [Fact]
    public void DGetBackDeleteHumanClickSkipsAnEmptySelectedName()
    {
        var frm = NewForm();
        int calls = 0;
        string seen = null;
        FStateClMainSeam.SendGetBackDeleteChrHandler = n => { calls++; seen = n; };
        FStateMShareSeam.g_SelDeleteHumanInfo_sChrName = "";

        frm.DGetBackDeleteHumanClick(null, 0, 0);

        Assert.Equal(0, calls);                                     // 20594：空名不发
        Assert.Null(seen);
    }

    [Fact]
    public void DGetBackDeleteHumanClickSendsTheSelectedName()
    {
        var frm = NewForm();
        string seen = null;
        FStateClMainSeam.SendGetBackDeleteChrHandler = n => seen = n;
        FStateMShareSeam.g_SelDeleteHumanInfo_sChrName = "英雄甲";

        frm.DGetBackDeleteHumanClick(null, 0, 0);

        Assert.Equal("英雄甲", seen);                                // 20595
    }

    [Fact]
    public void DCustomButtonClickOnlySendsForImageButtons()
    {
        var frm = NewForm();
        var sent = new List<(int Cmd, int Tag)>();
        FStateClMainSeam.SendClientMessageHandler =
            (cmd, recog, p1, p2, p3, msg) => sent.Add((cmd, recog));

        // 非 TDxImageButton ⇒ 不发（原文 24919 的 `Sender is TDxImageButton`）
        frm.DCustomButtonClick(new object(), 0, 0);
        frm.DCustomButtonClick(null, 0, 0);
        Assert.Empty(sent);

        // TDxImageButton ⇒ 发，且 Command = CM_CUSTOM_BUTTON_CLICK、Recog = Tag
        var btn = new TDxImageButton { Tag = 4321 };
        frm.DCustomButtonClick(btn, 0, 0);

        Assert.Single(sent);
        Assert.Equal(Grobal2Const.CM_CUSTOM_BUTTON_CLICK, sent[0].Cmd);
        Assert.Equal(4321, sent[0].Tag);
    }

    [Fact]
    public void DBotHorseClickTakesTheHorseUnconditionally()
    {
        var frm = NewForm();
        object seen = null;
        FStateClMainSeam.TakeHorseHandler = a => seen = a;
        g_MySelf = new TActor();

        frm.DBotHorseClick(null, 0, 0);

        Assert.Same(g_MySelf, seen);                                // 18844：无条件
    }

    [Theory]
    [InlineData(0, 0, false)]   // m_btHorse = 0 ⇒ 不满足 in [1,2]
    [InlineData(1, 0, true)]    // in [1,2] 且非双人骑 ⇒ 骑
    [InlineData(2, 0, true)]
    [InlineData(1, 1, false)]   // 双人骑被邀请人（m_btDoubleHumHorse <> 0）⇒ 不骑
    [InlineData(3, 0, false)]   // 不在 [1,2]
    public void DDownHorseClickHonoursTheOriginalTwoConditions(int horse, byte doubleHum, bool expect)
    {
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.TakeHorseHandler = _ => calls++;
        g_MySelf = new TActor();
        g_MySelf.m_btHorse = horse;                                 // ActorSelfEffectRender.cs:413（int 型）
        g_MySelf.m_btDoubleHumHorse = doubleHum;

        frm.DDownHorseClick(null, 0, 0);

        Assert.Equal(expect ? 1 : 0, calls);                        // 20572
    }

    [Fact]
    public void DDownHorseClickKeepsTheOriginalMissingNilCheck()
    {
        // 原文 20572 **没有** g_MySelf <> nil 保护 ⇒ g_MySelf = nil 时 AV（托管侧 NullReferenceException）。
        // 逐字保留该前提，不额外加保护（否则会把原文缺陷掩盖成"安全"行为）。
        var frm = NewForm();
        g_MySelf = null;
        Assert.Throws<NullReferenceException>(() => frm.DDownHorseClick(null, 0, 0));
    }

    [Fact]
    public void HorseButtonsDoNothingWhenTheSeamIsNotInjected()
    {
        // 接缝语义：未注入 handler 时什么都不发生（不是抛）—— 与"未移植壳"可区分。
        var frm = NewForm();
        g_MySelf = new TActor();
        Assert.Null(Record.Exception(() => frm.DBotHorseClick(null, 0, 0)));
        Assert.Null(Record.Exception(() => frm.DDownHorseClick(null, 0, 0)));
    }

    // =====================================================================================
    // 切片 4：tick 守卫族（时钟经 FStateSeamClock 注入 ⇒ 可精确落点）
    // =====================================================================================

    [Fact]
    public void LedgerSlice4RegistersFiveMembers()
    {
        Assert.Equal(5, TFrmDlgPortLedger.Slice4Count);
    }

    [Fact]
    public void DGDHomeClickForwardsAndRearmsWhenTheTickIsStrictlyGreater()
    {
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.SendGuildHomeHandler = () => calls++;
        FStateMShareSeam.g_dwQueryMsgTick = 9_999;      // 当前 fake tick = 10_000
        frm.BoGuildChat = true;

        frm.DGDHomeClick(null, 0, 0);

        Assert.Equal(1, calls);                          // 17878
        Assert.Equal(13_000u, FStateMShareSeam.g_dwQueryMsgTick);   // 17877：+3000
        Assert.False(frm.BoGuildChat);                   // 17879
    }

    [Fact]
    public void DGDHomeClickDoesNothingWhenTheTickIsNotGreater()
    {
        // 边界：`>` 是**严格大于** ⇒ tick == g_dwQueryMsgTick 时**不**触发。
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.SendGuildHomeHandler = () => calls++;
        FStateMShareSeam.g_dwQueryMsgTick = 10_000;     // 与当前 tick 相等
        frm.BoGuildChat = true;

        frm.DGDHomeClick(null, 0, 0);

        Assert.Equal(0, calls);                          // 差异断言：相等不触发
        Assert.Equal(10_000u, FStateMShareSeam.g_dwQueryMsgTick);   // 未重装
        Assert.True(frm.BoGuildChat);                    // 未改（17879 在守卫体内）
    }

    [Fact]
    public void DGDHomeClickIsThrottledUntilTheWindowElapses()
    {
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.SendGuildHomeHandler = () => calls++;
        FStateMShareSeam.g_dwQueryMsgTick = 0;

        frm.DGDHomeClick(null, 0, 0);
        Assert.Equal(1, calls);

        // 立刻再点：tick 仍 10_000，而 g_dwQueryMsgTick 已是 13_000 ⇒ 不触发（节流）
        frm.DGDHomeClick(null, 0, 0);
        Assert.Equal(1, calls);

        // 时钟推进到 13_000：`>` 仍不成立（严格大于）⇒ 仍不触发
        _fakeTick = 13_000;
        frm.DGDHomeClick(null, 0, 0);
        Assert.Equal(1, calls);

        // 13_001 ⇒ 触发
        _fakeTick = 13_001;
        frm.DGDHomeClick(null, 0, 0);
        Assert.Equal(2, calls);
        Assert.Equal(16_001u, FStateMShareSeam.g_dwQueryMsgTick);
    }

    [Fact]
    public void DGDListClickForwardsTheMemberListAndRearms()
    {
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.SendGuildMemberListHandler = () => calls++;
        FStateMShareSeam.g_dwQueryMsgTick = 9_999;
        frm.BoGuildChat = true;

        frm.DGDListClick(null, 0, 0);

        Assert.Equal(1, calls);                          // 17887
        Assert.Equal(13_000u, FStateMShareSeam.g_dwQueryMsgTick);   // 17886
        Assert.False(frm.BoGuildChat);                   // 17888
    }

    [Fact]
    public void DGDHomeAndListShareTheSameThrottleCounter()
    {
        // 两条用**同一个** g_dwQueryMsgTick ⇒ 先后调用时第二条被第一条的重装挡住。
        var frm = NewForm();
        int home = 0, list = 0;
        FStateClMainSeam.SendGuildHomeHandler = () => home++;
        FStateClMainSeam.SendGuildMemberListHandler = () => list++;

        frm.DGDHomeClick(null, 0, 0);
        frm.DGDListClick(null, 0, 0);

        Assert.Equal(1, home);
        Assert.Equal(0, list);                           // 差异化证据：共享计数器
    }

    [Fact]
    public void DBotUserShopClickOpensTheGameShopDialog()
    {
        var frm = NewForm();
        // OpenDGameShopDlg 仍是 throw 壳 ⇒ 转发后抛（抛点在被转发方）
        var ex = Assert.Throws<NotSupportedException>(() => frm.DBotUserShopClick(null, 0, 0));
        Assert.Contains("OpenDGameShopDlg", ex.Message);  // 20567
    }

    [Fact]
    public void RankingAndFriendEntryPointsForwardToTheirOwnTargets()
    {
        var frm = NewForm();
        var ranking = Assert.Throws<NotSupportedException>(() => frm.DBotRankingClick(null, 0, 0));
        Assert.Contains("OpenDRankingDlg", ranking.Message);      // 18885
        var friend = Assert.Throws<NotSupportedException>(() => frm.DBotFriendClick(null, 0, 0));
        Assert.Contains("OpenDFriendDlg", friend.Message);        // 18895
    }

    // =====================================================================================
    // 切片 5：交易 / 挑战的"守卫 + 转发"族
    // =====================================================================================

    [Fact]
    public void LedgerSlice5RegistersSixMembers()
    {
        Assert.Equal(6, TFrmDlgPortLedger.Slice5Count);
    }

    [Fact]
    public void DBotTradeClickSharesTheQueryTickThrottle()
    {
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.SendDealTryHandler = () => calls++;
        FStateMShareSeam.g_dwQueryMsgTick = 9_999;

        frm.DBotTradeClick(null, 0, 0);

        Assert.Equal(1, calls);                                     // 18916
        Assert.Equal(13_000u, FStateMShareSeam.g_dwQueryMsgTick);   // 18915：+3000

        // 与 DGDHomeClick 共享同一计数器 ⇒ 立刻再点不触发
        frm.DBotTradeClick(null, 0, 0);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void BotChallengeClickForwardsAndRearms()
    {
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.SendChallengeTryHandler = () => calls++;
        FStateMShareSeam.g_dwQueryMsgTick = 10_000;   // 与当前 tick 相等 ⇒ 严格 `>` 不成立

        frm.BotChallengeClick(null, 0, 0);
        Assert.Equal(0, calls);                                     // 边界：相等不触发
        Assert.Equal(10_000u, FStateMShareSeam.g_dwQueryMsgTick);   // 未重装

        FStateMShareSeam.g_dwQueryMsgTick = 9_999;
        frm.BotChallengeClick(null, 0, 0);
        Assert.Equal(1, calls);                                     // 18908
        Assert.Equal(13_000u, FStateMShareSeam.g_dwQueryMsgTick);
    }

    [Fact]
    public void DDealCloseClickForwardsAndDoesNotRearmTheDealTick()
    {
        var frm = NewForm();
        int cancels = 0;
        FStateClMainSeam.SendCancelDealHandler = () => cancels++;
        FStateMShareSeam.g_dwDealActionTick = 9_999;

        // 17536 的 CloseDDealDlg 仍是 throw 壳 ⇒ 抛点在被转发方（且发生在发送之前）
        var ex = Assert.Throws<NotSupportedException>(() => frm.DDealCloseClick(null, 0, 0));
        Assert.Contains("CloseDDealDlg", ex.Message);
        Assert.Equal(0, cancels);                                   // 17537 未执行（前一行先抛）

        // 差异断言：本条**不**重装 g_dwDealActionTick（原文 17535 的守卫体内没有赋值）
        Assert.Equal(9_999u, FStateMShareSeam.g_dwDealActionTick);
    }

    [Fact]
    public void DealZeroGoldRequiresBothFlagsAndRearmsByFourThousand()
    {
        var frm = NewForm();
        var sent = new List<int>();
        FStateClMainSeam.SendChangeDealGoldHandler = g => sent.Add(g);

        // not g_boDealEnd = False ⇒ 不触发
        FStateMShareSeam.g_boDealEnd = true;
        FStateMShareSeam.g_nDealGold = 5;
        frm.DealZeroGold();
        Assert.Empty(sent);

        // g_boDealEnd = False 但 g_nDealGold = 0 ⇒ 不触发（`> 0` 严格）
        FStateMShareSeam.g_boDealEnd = false;
        FStateMShareSeam.g_nDealGold = 0;
        frm.DealZeroGold();
        Assert.Empty(sent);

        // 两个都成立 ⇒ 发 0 + 重装 +4000
        FStateMShareSeam.g_nDealGold = 1;
        frm.DealZeroGold();
        Assert.Single(sent);
        Assert.Equal(0, sent[0]);                                   // 17750：SendChangeDealGold(0)
        Assert.Equal(14_000u, FStateMShareSeam.g_dwDealActionTick); // 17749：+4000
    }

    [Fact]
    public void ChallengeZeroGoldMirrorsDealZeroGold()
    {
        var frm = NewForm();
        var sent = new List<int>();
        FStateClMainSeam.SendChangeChallengeGoldHandler = g => sent.Add(g);

        FStateMShareSeam.g_boChallengeEnd = false;
        FStateMShareSeam.g_nChallengeGold = 3;
        frm.ChallengeZeroGold();

        Assert.Single(sent);
        Assert.Equal(0, sent[0]);                                       // 20793
        Assert.Equal(14_000u, FStateMShareSeam.g_dwChallengeActionTick); // 20792：+4000

        // 挑战/交易两条走**各自**的时间戳（差异证据）
        Assert.Equal(0u, FStateMShareSeam.g_dwDealActionTick);
    }

    [Fact]
    public void DChallengeCloseClickUsesItsOwnTickAndForwards()
    {
        var frm = NewForm();
        int cancels = 0;
        FStateClMainSeam.SendCancelChallengeHandler = () => cancels++;
        FStateMShareSeam.g_dwChallengeActionTick = 9_999;

        // CloseDChallengeDlg 仍是 throw 壳 ⇒ 抛在被转发方
        var ex = Assert.Throws<NotSupportedException>(() => frm.DChallengeCloseClick(null, 0, 0));
        Assert.Contains("CloseDChallengeDlg", ex.Message);
        Assert.Equal(0, cancels);

        // 差异证据：本条只看挑战时间戳，交易时间戳不参与
        Assert.Equal(9_999u, FStateMShareSeam.g_dwChallengeActionTick);
        Assert.Equal(0u, FStateMShareSeam.g_dwDealActionTick);
    }

    // =====================================================================================
    // 切片 6：帮助按钮节流（差判据） + 更新状态框重连
    // =====================================================================================

    [Fact]
    public void LedgerSlice6RegistersTwoMembers()
    {
        Assert.Equal(2, TFrmDlgPortLedger.Slice6Count);
    }

    [Fact]
    public void DControlHelpClickUsesAStrictDifferenceGuardOfOneThousand()
    {
        var frm = NewForm();
        var sent = new List<int>();
        FStateClMainSeam.SendClientMessageHandler =
            (cmd, recog, p1, p2, p3, msg) => sent.Add(cmd);

        // 差 == 1000 ⇒ `> 1000` 不成立（严格大于）
        SetHelpTick(frm, 9_000);
        frm.DControlHelpClick(null, 0, 0);
        Assert.Empty(sent);                                          // 21056
        Assert.Equal(9_000u, GetHelpTick(frm));                      // 未赋值

        // 差 == 1001 ⇒ 成立，且时间戳被赋成**当前 tick**（不是 +1000）
        SetHelpTick(frm, 8_999);
        frm.DControlHelpClick(null, 0, 0);
        Assert.Single(sent);
        Assert.Equal(Grobal2Const.CM_HELPBUTTONCLICK, sent[0]);       // 21058
        Assert.Equal(10_000u, GetHelpTick(frm));                     // 21057
    }

    [Fact]
    public void DControlHelpClickThrottlesUntilAnotherThousandMillisPass()
    {
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.SendClientMessageHandler = (a, b, c, d, e, f) => calls++;

        SetHelpTick(frm, 0);
        frm.DControlHelpClick(null, 0, 0);
        Assert.Equal(1, calls);

        // 立刻再点：差为 0 ⇒ 不触发
        frm.DControlHelpClick(null, 0, 0);
        Assert.Equal(1, calls);

        _fakeTick = 11_001;   // 差 1001 ⇒ 触发
        frm.DControlHelpClick(null, 0, 0);
        Assert.Equal(2, calls);
        Assert.Equal(11_001u, GetHelpTick(frm));
    }

    /// <summary>
    /// 原文 486 的 `dwControlHelpCickTick:LongWord` 在 Delphi 的 **protected** 段，
    /// 托管生成壳同样落成 `protected`（`TFrmDlg.Decl.g.cs`），测试只能经反射读写。
    /// </summary>
    private static System.Reflection.FieldInfo HelpTickField()
    {
        var f = typeof(TFrmDlg).GetField("dwControlHelpCickTick",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.NotNull(f);
        return f;
    }

    private static uint GetHelpTick(TFrmDlg frm) => (uint)HelpTickField().GetValue(frm);

    private static void SetHelpTick(TFrmDlg frm, uint value) => HelpTickField().SetValue(frm, value);

    [Fact]
    public void DUpdateStatusDlgDblClickReconnectsTheSocketGate()
    {
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.ReConnectClientSocketGateHandler = () => calls++;

        frm.DUpdateStatusDlgDblClick(null, 0, 0);

        Assert.Equal(1, calls);                                      // 24471
    }

    // =====================================================================================
    // 切片 7：组队模式开关对 + 交易物品回包 + 元宝交易菜单清场
    // =====================================================================================

    [Fact]
    public void LedgerSlice7RegistersFourMembers()
    {
        Assert.Equal(4, TFrmDlgPortLedger.Slice7Count);
    }

    [Fact]
    public void GroupModeToggleFlipsTheFlagRearmsByFiveThousandAndReportsTheNewValue()
    {
        var frm = NewForm();
        var reported = new List<bool>();
        FStateClMainSeam.SendGroupModeHandler = v => reported.Add(v);
        FStateMShareSeam.g_boAllowGroup = false;
        FStateMShareSeam.g_dwChangeGroupModeTick = 9_999;

        frm.DGrpAllowGroupClick(null, 0, 0);

        Assert.True(FStateMShareSeam.g_boAllowGroup);                        // 18943：取反
        Assert.Equal(15_000u, FStateMShareSeam.g_dwChangeGroupModeTick);     // 18944：+5000
        Assert.Single(reported);
        Assert.True(reported[0]);                                            // 18945：上报**取反后**的值
    }

    [Fact]
    public void GroupModeToggleIsThrottledAndUsesAStrictGreaterThan()
    {
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.SendGroupModeHandler = _ => calls++;
        FStateMShareSeam.g_boAllowGroup = false;

        // 相等 ⇒ 严格 `>` 不成立
        FStateMShareSeam.g_dwChangeGroupModeTick = 10_000;
        frm.DGrpAllowGroupClick(null, 0, 0);
        Assert.Equal(0, calls);
        Assert.False(FStateMShareSeam.g_boAllowGroup);                // 未被取反

        // tick 未推进时再点 ⇒ 被上一步的 +5000 挡住
        FStateMShareSeam.g_dwChangeGroupModeTick = 0;
        frm.DGrpAllowGroupClick(null, 0, 0);
        Assert.Equal(1, calls);
        frm.DGrpAllowGroupClick(null, 0, 0);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void BotGroupMouseDownOnlyTogglesOnTheRightButton()
    {
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.SendGroupModeHandler = _ => calls++;
        FStateMShareSeam.g_boAllowGroup = false;

        frm.DBotGroupMouseDown(null, TMouseButton.mbLeft, TShiftState.ssNone, 0, 0);
        Assert.Equal(0, calls);                                       // 18922：非右键直接返回
        Assert.False(FStateMShareSeam.g_boAllowGroup);

        frm.DBotGroupMouseDown(null, TMouseButton.mbRight, TShiftState.ssNone, 0, 0);
        Assert.Equal(1, calls);
        Assert.True(FStateMShareSeam.g_boAllowGroup);
    }

    [Fact]
    public void BotGroupMouseDownSharesTheSameThrottleAndBodyAsTheAllowGroupButton()
    {
        // 差异证据：两条走**同一个** g_dwChangeGroupModeTick 与同一份主体 ⇒
        // 右键点过之后，立刻点"允许组队"按钮不会再次翻转。
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.SendGroupModeHandler = _ => calls++;
        FStateMShareSeam.g_boAllowGroup = false;
        FStateMShareSeam.g_dwChangeGroupModeTick = 0;

        frm.DBotGroupMouseDown(null, TMouseButton.mbRight, TShiftState.ssNone, 0, 0);
        Assert.True(FStateMShareSeam.g_boAllowGroup);

        frm.DGrpAllowGroupClick(null, 0, 0);
        Assert.Equal(1, calls);                                       // 共享计数器挡住
        Assert.True(FStateMShareSeam.g_boAllowGroup);
    }

    [Fact]
    public void DealItemReturnBagCachesReportsAndRearms()
    {
        var frm = NewForm();
        var sent = new List<TClientItem>();
        FStateClMainSeam.SendDelDealItemHandler = it => sent.Add(it);
        FStateMShareSeam.g_boDealEnd = false;

        frm.DealItemReturnBag(new TClientItem());

        Assert.Single(sent);                                          // 17621
        Assert.Equal(14_000u, FStateMShareSeam.g_dwDealActionTick);   // 17622：+4000
    }

    [Fact]
    public void DealItemReturnBagDoesNothingAfterTheDealEnded()
    {
        // 原文**只判** not g_boDealEnd（17619）—— 交易结束后整段跳过。
        var frm = NewForm();
        int calls = 0;
        FStateClMainSeam.SendDelDealItemHandler = _ => calls++;
        FStateMShareSeam.g_boDealEnd = true;

        frm.DealItemReturnBag(new TClientItem());

        Assert.Equal(0, calls);
        Assert.Equal(0u, FStateMShareSeam.g_dwDealActionTick);        // 未重装
    }

    [Fact]
    public void DGameGoldDealMenuDlgCloseForwardsToTheMenuDialogCloser()
    {
        var frm = NewForm();
        FStateMShareSeam.g_GameGoldDeal.ItemCount = 7;

        // 18662 的 CloseDGameGoldDealMenuDlg 仍是 throw 壳 ⇒ 抛点在被转发方（且发生在清场之前）
        var ex = Assert.Throws<NotSupportedException>(() => frm.DGameGoldDealMenuDlgCloseClick(null, 0, 0));
        Assert.Contains("CloseDGameGoldDealMenuDlg", ex.Message);
        Assert.Equal(7, FStateMShareSeam.g_GameGoldDeal.ItemCount);   // 未被清（前一行先抛）
    }

    [Fact]
    public void GameGoldDealSeamHasTheOriginalNineRemoteSlots()
    {
        // 原文 `g_GameGoldDealRemoteItems:array[0..8] of TClientItem`（9 槽）。
        Assert.Equal(9, FStateMShareSeam.g_GameGoldDealRemoteItems.Length);
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

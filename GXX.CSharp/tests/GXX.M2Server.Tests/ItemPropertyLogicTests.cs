using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.M2Server.Forms.ItemProperty;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 车道 p8-m2-itemprop-misc 切片2：uFrmCustomItemProperty.pas **纯逻辑处**（CustomItemPropertyLogic）
/// 与接缝层（CustomItemPropertySeams）的 1:1 断言。
///
/// 本文件全部用例操作**静态接缝量**，故与同集合用例串行（DisableParallelization，见
/// ItemPropertyTestBase.cs），避免《并行派发台账》§19.5 的静态全局污染。
/// </summary>
[Collection("ItemPropertyLane")]
public sealed class ItemPropertyLogicTests
{
    // ------------------------------------------------------------------
    // CustomItemPropertyLogic.GetTextStr —— Delphi TStrings.GetTextStr 1:1
    // ------------------------------------------------------------------

    [Fact]
    public void GetTextStr_EmptyList_IsEmptyString()
    {
        var list = new TStringList();
        Assert.Equal("", CustomItemPropertyLogic.GetTextStr(list));
    }

    [Fact]
    public void GetTextStr_OneLine_HasTrailingCrlf()
    {
        var list = new TStringList();
        list.Add("a");
        Assert.Equal("a\r\n", CustomItemPropertyLogic.GetTextStr(list));
    }

    [Fact]
    public void GetTextStr_TwoLines_EachLineFollowedByCrlf()
    {
        var list = new TStringList();
        list.Add("a");
        list.Add("b");
        Assert.Equal("a\r\nb\r\n", CustomItemPropertyLogic.GetTextStr(list));
    }

    [Fact]
    public void GetTextStr_EmptyLine_YieldsBareCrlf()
    {
        var list = new TStringList();
        list.Add("");
        Assert.Equal("\r\n", CustomItemPropertyLogic.GetTextStr(list));
    }

    [Fact]
    public void GetTextStr_NullList_IsEmptyString()
        => Assert.Equal("", CustomItemPropertyLogic.GetTextStr(null!));

    // ------------------------------------------------------------------
    // CustomItemPropertyLogic.SetTextStr —— Delphi TStrings.SetTextStr 1:1（4 类边界）
    // ------------------------------------------------------------------

    [Fact]
    public void SetTextStr_EmptyString_YieldsNoLines()
    {
        var list = new TStringList();
        list.Add("stale");
        CustomItemPropertyLogic.SetTextStr(list, "");
        Assert.Equal(0, list.Count);                 // 原文：Clear 后 P = nil → 不进 while
    }

    [Fact]
    public void SetTextStr_TrailingCrlf_DoesNotProduceEmptyLastLine()
    {
        var list = new TStringList();
        CustomItemPropertyLogic.SetTextStr(list, "a\r\n");
        Assert.Equal(1, list.Count);
        Assert.Equal("a", list[0]);
    }

    [Fact]
    public void SetTextStr_LeadingCrlf_ProducesOneEmptyLine()
    {
        var list = new TStringList();
        CustomItemPropertyLogic.SetTextStr(list, "\r\n");
        Assert.Equal(1, list.Count);
        Assert.Equal("", list[0]);
    }

    [Fact]
    public void SetTextStr_ConsecutiveLf_ProducesEmptyMiddleLine()
    {
        var list = new TStringList();
        CustomItemPropertyLogic.SetTextStr(list, "a\n\nb");
        Assert.Equal(3, list.Count);
        Assert.Equal(new[] { "a", "", "b" }, new[] { list[0], list[1], list[2] });
    }

    [Fact]
    public void SetTextStr_BareCr_IsAlsoALineBreak()
    {
        var list = new TStringList();
        CustomItemPropertyLogic.SetTextStr(list, "a\rb");
        Assert.Equal(2, list.Count);
        Assert.Equal(new[] { "a", "b" }, new[] { list[0], list[1] });
    }

    [Fact]
    public void SetTextStr_MixedCrLf_IsSingleBreak()
    {
        var list = new TStringList();
        CustomItemPropertyLogic.SetTextStr(list, "a\r\nb");
        Assert.Equal(2, list.Count);
        Assert.Equal(new[] { "a", "b" }, new[] { list[0], list[1] });
    }

    [Fact]
    public void SetTextStr_ClearsPreviousContentFirst()
    {
        var list = new TStringList();
        list.Add("x");
        list.Add("y");
        CustomItemPropertyLogic.SetTextStr(list, "z");
        Assert.Equal(1, list.Count);
        Assert.Equal("z", list[0]);
    }

    [Fact]
    public void SetTextStr_NullList_DoesNotThrow()
        => CustomItemPropertyLogic.SetTextStr(null!, "a");

    [Fact]
    public void TextRoundTrip_GetTextStrThenSetTextStr_PreservesLines()
    {
        var src = new TStringList();
        src.Add("第一行");
        src.Add("");
        src.Add("line3");
        string text = CustomItemPropertyLogic.GetTextStr(src);
        Assert.Equal("第一行\r\n\r\nline3\r\n", text);

        var dst = new TStringList();
        CustomItemPropertyLogic.SetTextStr(dst, text);
        Assert.Equal(3, dst.Count);
        Assert.Equal("第一行", dst[0]);
        Assert.Equal("", dst[1]);
        Assert.Equal("line3", dst[2]);
    }

    // ------------------------------------------------------------------
    // CustomItemPropertyLogic.LineNumCaption —— 原文 :489/:495
    // ------------------------------------------------------------------

    [Fact]
    public void LineNumCaption_ZeroBasedCaret_YieldsOne()
        => Assert.Equal("当前行：1", CustomItemPropertyLogic.LineNumCaption(0));

    [Fact]
    public void LineNumCaption_CaretRow4_YieldsFive()
        => Assert.Equal("当前行：5", CustomItemPropertyLogic.LineNumCaption(4));

    [Fact]
    public void LineNumCaption_NegativeCaret_YieldsZero()
        => Assert.Equal("当前行：0", CustomItemPropertyLogic.LineNumCaption(-1));

    [Fact]
    public void LineNumCaption_LargeValue_IsPlainDecimal()
        => Assert.Equal("当前行：2147483647", CustomItemPropertyLogic.LineNumCaption(int.MaxValue - 1));

    // ------------------------------------------------------------------
    // 界限常量 —— 原文 :435 Low/High
    // ------------------------------------------------------------------

    [Fact]
    public void Bounds_MatchGrobal2Const()
    {
        Assert.Equal(1, CustomItemPropertyLogic.LowBindType);                     // 原文 array [1 .. 60]
        Assert.Equal(60, CustomItemPropertyLogic.HighBindType);
        Assert.Equal(Grobal2Const.CUSTOM_PROPERTY_BIND_TYPE_COUNT, CustomItemPropertyLogic.HighBindType);
        Assert.Equal(60, Grobal2Const.CUSTOM_PROPERTY_BIND_TYPE_COUNT);           // Grobal2.pas:64
    }

    [Fact]
    public void LineBreak_IsCrlf()
        => Assert.Equal("\r\n", CustomItemPropertyLogic.sLineBreak);

    // ------------------------------------------------------------------
    // CustomItemPropertyMessageBoxSeam
    // ------------------------------------------------------------------

    [Fact]
    public void MessageBoxSeam_DefaultUiEnabledIsTrue()
    {
        CustomItemPropertyGlobals.Reset();
        Assert.True(CustomItemPropertyMessageBoxSeam.UiEnabled);
        Assert.Equal(0, CustomItemPropertyMessageBoxSeam.ShowModalCalls);
    }

    [Fact]
    public void MessageBoxSeam_ShowModal_CountsCallAndReturnsFalse()
    {
        CustomItemPropertyGlobals.Reset();
        CustomItemPropertyMessageBoxSeam.UiEnabled = false;
        Assert.False(CustomItemPropertyMessageBoxSeam.ShowModal());
        Assert.Equal(1, CustomItemPropertyMessageBoxSeam.ShowModalCalls);
    }

    [Fact]
    public void MessageBoxSeam_ShowModal_HeadlessNeverBlocks()
    {
        CustomItemPropertyGlobals.Reset();
        CustomItemPropertyMessageBoxSeam.UiEnabled = false;
        for (int i = 0; i < 3; i++)
            CustomItemPropertyMessageBoxSeam.ShowModal();
        Assert.Equal(3, CustomItemPropertyMessageBoxSeam.ShowModalCalls);         // 无头：不创建窗口、不阻塞
    }

    // ------------------------------------------------------------------
    // CiPageControlSeam.SetActivePageIndex —— §21.3 Delphi 静默越界语义
    // ------------------------------------------------------------------

    [Fact]
    public void PageControl_SetActivePageIndex_InRange_SetsIndexAndPage()
    {
        var pgc = new CiPageControlSeam();
        var a = new CiTabSheetSeam();
        var b = new CiTabSheetSeam();
        pgc.Pages.Add(a);
        pgc.Pages.Add(b);
        pgc.SetActivePageIndex(1);
        Assert.Equal(1, pgc.ActivePageIndex);
        Assert.Same(b, pgc.ActivePage);
    }

    [Fact]
    public void PageControl_SetActivePageIndex_OutOfRange_SilentlyBecomesMinusOne()
    {
        var pgc = new CiPageControlSeam();
        pgc.Pages.Add(new CiTabSheetSeam());
        pgc.SetActivePageIndex(5);            // Delphi 静默置 -1（WinForms 会抛）
        Assert.Equal(-1, pgc.ActivePageIndex);
        Assert.Null(pgc.ActivePage);
    }

    [Fact]
    public void PageControl_SetActivePageIndex_Negative_SilentlyBecomesMinusOne()
    {
        var pgc = new CiPageControlSeam();
        pgc.Pages.Add(new CiTabSheetSeam());
        pgc.SetActivePageIndex(-1);
        Assert.Equal(-1, pgc.ActivePageIndex);
        Assert.Null(pgc.ActivePage);
    }

    // ------------------------------------------------------------------
    // CustomItemPropertyGlobals —— §25.2：接缝未接线必须"显式报未接线"，绝不静默返回中性值
    // ------------------------------------------------------------------

    [Fact]
    public void Globals_UnwiredChecks_ThrowsInsteadOfReturningNeutral()
    {
        CustomItemPropertyGlobals.Reset();
        var ex = Assert.Throws<InvalidOperationException>(
            () => CustomItemPropertyGlobals.g_CustomItemPropertyChecks);
        Assert.Contains("接缝未接线", ex.Message);
        Assert.Contains("M2Share.pas:4054", ex.Message);
    }

    [Fact]
    public void Globals_UnwiredBindNames_ThrowsInsteadOfReturningNeutral()
    {
        CustomItemPropertyGlobals.Reset();
        var ex = Assert.Throws<InvalidOperationException>(
            () => CustomItemPropertyGlobals.g_CustomItemPropertyBindNames);
        Assert.Contains("接缝未接线", ex.Message);
    }

    [Fact]
    public void Globals_UnwiredTextVarList_ThrowsInsteadOfReturningNull()
    {
        CustomItemPropertyGlobals.Reset();
        var ex = Assert.Throws<InvalidOperationException>(
            () => CustomItemPropertyGlobals.g_CustomItemPropertyTextVarList);
        Assert.Contains("接缝未接线", ex.Message);
    }

    [Fact]
    public void Globals_UnwiredRebuild_ThrowsOnInvoke()
    {
        CustomItemPropertyGlobals.Reset();
        Assert.Throws<InvalidOperationException>(
            CustomItemPropertyGlobals.InvokeRebuildCustomItemPropertyConfig);
    }

    [Fact]
    public void Globals_UnwiredSaveTextVarList_ThrowsOnInvoke()
    {
        CustomItemPropertyGlobals.Reset();
        Assert.Throws<InvalidOperationException>(
            CustomItemPropertyGlobals.InvokeSaveCustomItemPropertyTextVarList);
    }

    [Fact]
    public void Globals_UnwiredSendConfig_ThrowsOnInvoke()
    {
        CustomItemPropertyGlobals.Reset();
        Assert.Throws<InvalidOperationException>(
            CustomItemPropertyGlobals.InvokeSendCustomItemPropertyConfig);
    }

    [Fact]
    public void Globals_UnwiredSendTextVarList_ThrowsOnInvoke()
    {
        CustomItemPropertyGlobals.Reset();
        Assert.Throws<InvalidOperationException>(
            CustomItemPropertyGlobals.InvokeSendCustomItemPropertyTextVarList);
    }

    [Fact]
    public void Globals_WiredDelegates_AreInvokedExactlyOnce()
    {
        CustomItemPropertyGlobals.Reset();
        int rebuild = 0, save = 0, sendCfg = 0, sendText = 0;
        CustomItemPropertyGlobals.RebuildCustomItemPropertyConfig = () => rebuild++;
        CustomItemPropertyGlobals.SaveCustomItemPropertyTextVarList = () => save++;
        CustomItemPropertyGlobals.SendCustomItemPropertyConfig = () => sendCfg++;
        CustomItemPropertyGlobals.SendCustomItemPropertyTextVarList = () => sendText++;

        CustomItemPropertyGlobals.InvokeRebuildCustomItemPropertyConfig();
        CustomItemPropertyGlobals.InvokeSaveCustomItemPropertyTextVarList();
        CustomItemPropertyGlobals.InvokeSendCustomItemPropertyConfig();
        CustomItemPropertyGlobals.InvokeSendCustomItemPropertyTextVarList();

        Assert.Equal(1, rebuild);
        Assert.Equal(1, save);
        Assert.Equal(1, sendCfg);
        Assert.Equal(1, sendText);
    }

    [Fact]
    public void Globals_Reset_ClearsEverythingIncludingBoStartReady()
    {
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks = new bool[61];
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames = new string[61];
        CustomItemPropertyGlobals.g_CustomItemPropertyTextVarList = new TStringList();
        CustomItemPropertyGlobals.g_CustomItemPropertyCRC = 123u;
        CustomItemPropertyGlobals.g_CustomItemPropertyTextVarListTextCRC = 456u;
        CustomItemPropertyGlobals.boStartReady = true;
        CustomItemPropertyGlobals.SendCustomItemPropertyConfig = () => { };
        CustomItemPropertyMessageBoxSeam.UiEnabled = false;
        CustomItemPropertyMessageBoxSeam.ShowModalCalls = 7;

        CustomItemPropertyGlobals.Reset();

        Assert.Throws<InvalidOperationException>(() => CustomItemPropertyGlobals.g_CustomItemPropertyChecks);
        Assert.Throws<InvalidOperationException>(() => CustomItemPropertyGlobals.g_CustomItemPropertyBindNames);
        Assert.Throws<InvalidOperationException>(() => CustomItemPropertyGlobals.g_CustomItemPropertyTextVarList);
        Assert.Equal(0u, CustomItemPropertyGlobals.g_CustomItemPropertyCRC);
        Assert.Equal(0u, CustomItemPropertyGlobals.g_CustomItemPropertyTextVarListTextCRC);
        Assert.False(CustomItemPropertyGlobals.boStartReady);        // 原文全局初值 False
        Assert.True(CustomItemPropertyMessageBoxSeam.UiEnabled);
        Assert.Equal(0, CustomItemPropertyMessageBoxSeam.ShowModalCalls);
    }

    [Fact]
    public void Globals_BoStartReady_DefaultIsFalseMatchingDelphiGlobalInitialValue()
    {
        CustomItemPropertyGlobals.Reset();
        Assert.False(CustomItemPropertyGlobals.boStartReady);
    }
}

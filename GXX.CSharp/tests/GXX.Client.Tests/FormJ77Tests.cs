using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J77：Actor.pas 纹理装载层 1:1 测试 ——
/// LoadSaySurface(9504-9524)、LoadNameSurface(9526-9561)、LoadNumberLableSurface(9386-9398)，
/// 以及支撑它们的 HUtil32.GetValidStr3_Ex(1456-1539) 与 DxCanvas.GetTextTexture(1151-1246) 几何。
/// </summary>
public sealed class ActorSurfaceLoadTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public ActorSurfaceLoadTests()
    {
        var saved = (TActorCore.CanvasReadyFn, TActorCore.GetImageInfoFn,
            TActorCore.TextExtentWidthFn, TActorCore.LineHeightFn,
            TActorCore.CurrentFontAvailableFn, TActorCore.MyGetTickCountFn);

        _restore.Add(() =>
        {
            TActorCore.CanvasReadyFn = saved.Item1;
            TActorCore.GetImageInfoFn = saved.Item2;
            TActorCore.TextExtentWidthFn = saved.Item3;
            TActorCore.LineHeightFn = saved.Item4;
            TActorCore.CurrentFontAvailableFn = saved.Item5;
            TActorCore.MyGetTickCountFn = saved.Item6;
        });

        TActorCore.CanvasReadyFn = () => true;
        TActorCore.CurrentFontAvailableFn = () => true;
        TActorCore.TextExtentWidthFn = s => s.Length * 6;
        TActorCore.LineHeightFn = () => 14;
        TActorCore.GetImageInfoFn = s => LabelSurface.Of(s.Length * 6, 14);
        TActorCore.MyGetTickCountFn = () => 1000;
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    // ===================== GetValidStr3_Ex =====================

    [Fact]
    public void GetValidStr3Ex_SplitsAtFirstDivider()
    {
        var rest = StrUtilEx.GetValidStr3Ex("a\\b\\c", '\\', out var dest);

        Assert.Equal("a", dest);
        Assert.Equal("b\\c", rest);
    }

    [Fact]
    public void GetValidStr3Ex_NoDividerReturnsWholeAsDestAndEmptyRest()
    {
        var rest = StrUtilEx.GetValidStr3Ex("abc", '\\', out var dest);

        Assert.Equal("abc", dest);
        Assert.Equal("", rest);
    }

    [Fact]
    public void GetValidStr3Ex_LeadingDividersAreSkipped()
    {
        // StartIndex 反复后移，直到首个非分隔符起算
        var rest = StrUtilEx.GetValidStr3Ex("\\\\a\\b", '\\', out var dest);

        Assert.Equal("a", dest);
        Assert.Equal("b", rest);
    }

    [Fact]
    public void GetValidStr3Ex_OnlyLeadingDividerNoTrailing()
    {
        // 全程无命中且 StartIndex > 1 → Dest 取去头后整段，Result = ''
        var rest = StrUtilEx.GetValidStr3Ex("\\\\abc", '\\', out var dest);

        Assert.Equal("abc", dest);
        Assert.Equal("", rest);
    }

    [Fact]
    public void GetValidStr3Ex_EmptyInputKeepsDestAndReturnsEmpty()
    {
        var rest = StrUtilEx.GetValidStr3Ex("", '\\', out var dest);

        Assert.Equal("", dest);
        Assert.Equal("", rest);
    }

    [Fact]
    public void GetValidStr3Ex_AllDividersYieldsEmptyDest()
    {
        var rest = StrUtilEx.GetValidStr3Ex("\\\\\\", '\\', out var dest);

        Assert.Equal("", dest);
        Assert.Equal("", rest);
    }

    [Fact]
    public void GetValidStr3Ex_TrailingDividerYieldsEmptySegment()
    {
        var rest = StrUtilEx.GetValidStr3Ex("a\\", '\\', out var dest);

        Assert.Equal("a", dest);
        Assert.Equal("", rest);
    }

    [Fact]
    public void GetValidStr3Ex_MatchesLoadNameSurfaceLoop()
    {
        // 复刻 LoadNameSurface 9543-9547 的 while 循环：空串即停
        var sl = new List<string>();
        string s = "第一行\\第二行\\第三行";
        while (true)
        {
            if (s == "")
                break;
            s = StrUtilEx.GetValidStr3Ex(s, '\\', out var name);
            sl.Add(name);
        }

        Assert.Equal(3, sl.Count);
        Assert.Equal("第一行", sl[0]);
        Assert.Equal("第二行", sl[1]);
        Assert.Equal("第三行", sl[2]);
    }

    [Fact]
    public void GetValidStr3Ex_LoadNameLoopTerminatesOnTrailingDivider()
    {
        var sl = new List<string>();
        string s = "A\\";
        while (true)
        {
            if (s == "")
                break;
            s = StrUtilEx.GetValidStr3Ex(s, '\\', out var name);
            sl.Add(name);
        }

        // "A\" → dest="A", rest=""（循环结束）
        Assert.Single(sl);
        Assert.Equal("A", sl[0]);
    }

    // ===================== GetTextTexture 几何 =====================

    [Fact]
    public void BuildTextTexture_WidthIsMaxLineWidthHeightIsLineHeightTimesCount()
    {
        var layout = TActorCore.BuildTextTexture(new List<string> { "ab", "abcd", "a" });

        Assert.Equal(4 * 6, layout.Width);          // 最长 "abcd"
        Assert.Equal(14, layout.LineHeight);
        Assert.Equal(14 * 3, layout.Height);
    }

    [Fact]
    public void BuildTextTexture_CentersEachLineWithDtCenter()
    {
        var layout = TActorCore.BuildTextTexture(new List<string> { "ab", "abcd" });

        // nW = 24；"ab" 宽 12 → x = (24-12)/2 = 6；"abcd" 宽 24 → x = 0
        Assert.Equal(6, layout.CenteredX[0]);
        Assert.Equal(0, layout.CenteredX[1]);
    }

    [Fact]
    public void BuildTextTexture_LineYIsIndexTimesLineHeight()
    {
        var layout = TActorCore.BuildTextTexture(new List<string> { "a", "b", "c" });

        Assert.Equal(0, layout.LineY[0]);
        Assert.Equal(14, layout.LineY[1]);
        Assert.Equal(28, layout.LineY[2]);
    }

    [Fact]
    public void BuildTextTexture_EmptyListYieldsZeroSize()
    {
        var layout = TActorCore.BuildTextTexture(new List<string>());

        Assert.Equal(0, layout.Width);
        Assert.Equal(0, layout.Height);
        Assert.Empty(layout.Lines);
    }

    [Fact]
    public void BuildTextTexture_SingleEmptyLineStillCountsHeight()
    {
        var layout = TActorCore.BuildTextTexture(new List<string> { "" });

        Assert.Equal(0, layout.Width);
        Assert.Equal(14, layout.Height);            // nLH * SL.Count = 14 * 1
    }

    // ===================== LoadSaySurface =====================

    [Fact]
    public void LoadSaySurface_FirstLineEmptyClearsAllSlots()
    {
        var a = new TActor();
        a.EnsureSayingSlots();
        for (int i = 0; i < ActorSay.MaxSay; i++)
        {
            a.SayingText[i] = "脏" + i;
            a.SayingArr[i].HasImage = true;
            a.SayingArr[i].Width = 10;
        }

        a.SayingText[0] = "";                       // 首行为空
        a.LoadSaySurface();

        for (int i = 0; i < ActorSay.MaxSay; i++)
        {
            Assert.Equal("", a.SayingText[i]);
            Assert.False(a.SayingArr[i].HasImage);
            Assert.Equal(0, a.SayingArr[i].Width);
        }
    }

    [Fact]
    public void LoadSaySurface_ClearsEvenWhenCanvasNotReady()
    {
        TActorCore.CanvasReadyFn = () => false;

        var a = new TActor();
        a.EnsureSayingSlots();
        a.SayingText[0] = "";
        a.SayingText[1] = "保留?";
        a.SayingArr[1].HasImage = true;

        a.LoadSaySurface();

        // 首行为空的分支不看画布状态 → 依然全清
        Assert.Equal("", a.SayingText[1]);
        Assert.False(a.SayingArr[1].HasImage);
    }

    [Fact]
    public void LoadSaySurface_CanvasNotReadyWithTextLeavesSlotsUntouched()
    {
        TActorCore.CanvasReadyFn = () => false;

        var a = new TActor();
        a.EnsureSayingSlots();
        a.SayingText[0] = "第一行";
        a.SayingArr[0] = LabelSurface.Of(99, 99, hasImage: false);
        a.m_nSayLineCount = 1;

        a.LoadSaySurface();

        // Exit 前未改动任何槽位
        Assert.Equal(99, a.SayingArr[0].Width);
        Assert.False(a.SayingArr[0].HasImage);
    }

    [Fact]
    public void LoadSaySurface_LoadsTextureForEachNonEmptyLine()
    {
        var a = new TActor();
        a.EnsureSayingSlots();
        a.SayingText[0] = "第一行";
        a.SayingText[1] = "";
        a.SayingText[2] = "第三行";
        a.m_nSayLineCount = 3;

        a.LoadSaySurface();

        Assert.True(a.SayingArr[0].HasImage);
        Assert.Equal("第一行".Length * 6, a.SayingArr[0].Width);
        // 空行不取纹理，保持原状
        Assert.False(a.SayingArr[1].HasImage);
        Assert.True(a.SayingArr[2].HasImage);
    }

    [Fact]
    public void LoadSaySurface_OnlyIteratesUpToSayLineCount()
    {
        var a = new TActor();
        a.EnsureSayingSlots();
        a.SayingText[0] = "一";
        a.SayingText[1] = "二";
        a.SayingText[2] = "三";
        a.m_nSayLineCount = 1;                      // 只处理第 1 行

        a.LoadSaySurface();

        Assert.True(a.SayingArr[0].HasImage);
        Assert.False(a.SayingArr[1].HasImage);      // 超出 m_nSayLineCount → 未处理
        Assert.False(a.SayingArr[2].HasImage);
    }

    [Fact]
    public void LoadSaySurface_ZeroLineCountProcessesNothing()
    {
        var a = new TActor();
        a.EnsureSayingSlots();
        a.SayingText[0] = "一";
        a.m_nSayLineCount = 0;

        a.LoadSaySurface();

        Assert.False(a.SayingArr[0].HasImage);
    }

    // ===================== LoadNameSurface =====================

    [Fact]
    public void LoadNameSurface_AlwaysFreesOldTextureFirst()
    {
        TActorCore.CanvasReadyFn = () => false;

        var a = new TActor();
        a.m_NameTextSurface = LabelSurface.Of(50, 20);

        a.LoadNameSurface();

        // 9526-9534：释放发生在画布守卫之前
        Assert.Null(a.m_NameTextSurface);
    }

    [Fact]
    public void LoadNameSurface_NotReadyExitsWithoutUpdatingCurName()
    {
        TActorCore.CanvasReadyFn = () => false;

        var a = new TActor();
        a.m_sNameText = "新名字";
        a.m_sCurNameText = "旧名字";
        a.m_dwShowShopNameTimeTick = 777;

        a.LoadNameSurface();

        Assert.Equal("旧名字", a.m_sCurNameText);   // 未更新
        Assert.Equal(777u, a.m_dwShowShopNameTimeTick);
    }

    [Fact]
    public void LoadNameSurface_NoFontExitsWithoutUpdatingCurName()
    {
        TActorCore.CurrentFontAvailableFn = () => false;

        var a = new TActor();
        a.m_sNameText = "新名字";
        a.m_sCurNameText = "旧名字";

        a.LoadNameSurface();

        Assert.Equal("旧名字", a.m_sCurNameText);
        Assert.Null(a.m_NameTextSurface);
    }

    [Fact]
    public void LoadNameSurface_SingleLineNameBuildsTexture()
    {
        var a = new TActor();
        a.m_sNameText = "张三";

        a.LoadNameSurface();

        Assert.NotNull(a.m_NameTextSurface);
        Assert.Equal(2 * 6, a.m_NameTextSurface!.Width);   // 单行宽
        Assert.Equal(14, a.m_NameTextSurface.Height);      // 行高 × 1
        Assert.Equal("张三", a.m_sCurNameText);
    }

    [Fact]
    public void LoadNameSurface_BackslashSplitsIntoMultipleLines()
    {
        var a = new TActor();
        a.m_sNameText = "第一行\\第二行";

        a.LoadNameSurface();

        Assert.NotNull(a.LoadedNameLayout);
        Assert.Equal(2, a.LoadedNameLayout!.Lines.Count);
        Assert.Equal("第一行", a.LoadedNameLayout.Lines[0]);
        Assert.Equal("第二行", a.LoadedNameLayout.Lines[1]);
        Assert.Equal(14 * 2, a.m_NameTextSurface!.Height);  // 两行高
    }

    [Fact]
    public void LoadNameSurface_MultiLineWidthUsesLongestLine()
    {
        var a = new TActor();
        a.m_sNameText = "A\\ABCD";

        a.LoadNameSurface();

        // 最长 "ABCD" → 4*6 = 24
        Assert.Equal(24, a.m_NameTextSurface!.Width);
    }

    [Fact]
    public void LoadNameSurface_EmptyNameTextLeavesTextureNull()
    {
        var a = new TActor();
        a.m_sNameText = "";

        a.LoadNameSurface();

        Assert.Null(a.m_NameTextSurface);
        Assert.Null(a.LoadedNameLayout);
    }

    [Fact]
    public void LoadNameSurface_ShopStallLoadsShopNameTexture()
    {
        var a = new TActor();
        a.m_sNameText = "";
        a.m_sShopNameText = "小店";
        a.m_boShopStall = true;

        a.LoadNameSurface();

        Assert.NotNull(a.m_ShopNameImageInfo);
        Assert.Equal("小店".Length * 6, a.m_ShopNameImageInfo!.Width);
        Assert.Equal("小店", a.m_sCurShopNameText);
    }

    [Fact]
    public void LoadNameSurface_NotStallDoesNotTouchShopNameTexture()
    {
        var a = new TActor();
        a.m_sShopNameText = "小店";
        a.m_boShopStall = false;
        a.m_ShopNameImageInfo = LabelSurface.Of(11, 11);

        a.LoadNameSurface();

        Assert.Equal(11, a.m_ShopNameImageInfo!.Width);   // 未被替换
        // m_sCurShopNameText 仍会更新（9560 无条件执行）
        Assert.Equal("小店", a.m_sCurShopNameText);
    }

    [Fact]
    public void LoadNameSurface_StampsTickAndCurTexts()
    {
        TActorCore.MyGetTickCountFn = () => 5555;

        var a = new TActor();
        a.m_sNameText = "名字";
        a.m_sShopNameText = "店名";

        a.LoadNameSurface();

        Assert.Equal(5555u, a.m_dwShowShopNameTimeTick);
        Assert.Equal("名字", a.m_sCurNameText);
        Assert.Equal("店名", a.m_sCurShopNameText);
    }

    // ===================== LoadNumberLableSurface =====================

    [Fact]
    public void LoadNumberLableSurface_CanvasNotReadyExitsWithoutStamping()
    {
        TActorCore.CanvasReadyFn = () => false;

        var a = new TActor();
        a.m_dwShowShopNameTimeTick = 0;
        a.m_ShowNumberLableTimeTick = 321;
        a.m_NumberLableImageInfo = LabelSurface.Of(44, 44);

        a.LoadNumberLableSurface();

        // 9388 的 Exit 在打点之前
        Assert.Equal(321u, a.m_ShowNumberLableTimeTick);
        Assert.Equal(44, a.m_NumberLableImageInfo!.Width);   // 未清零
    }

    [Fact]
    public void LoadNumberLableSurface_StampsThenZeroesThenLoads()
    {
        TActorCore.MyGetTickCountFn = () => 8888;

        var a = new TActor();
        a.m_sNumberLableText = "12345";

        a.LoadNumberLableSurface();

        Assert.Equal(8888u, a.m_ShowNumberLableTimeTick);
        Assert.Equal("12345", a.m_sCurNumberLableText);
        Assert.NotNull(a.m_NumberLableImageInfo);
        Assert.Equal(5 * 6, a.m_NumberLableImageInfo!.Width);
    }

    [Fact]
    public void LoadNumberLableSurface_NoFontLeavesTextureZeroed()
    {
        TActorCore.CurrentFontAvailableFn = () => false;
        TActorCore.MyGetTickCountFn = () => 9999;

        var a = new TActor();
        a.m_sNumberLableText = "12345";
        a.m_sCurNumberLableText = "旧值";
        a.m_NumberLableImageInfo = LabelSurface.Of(50, 50);

        a.LoadNumberLableSurface();

        // 打点与清零已发生，但字体不可用 → 不取纹理、不更新 Cur
        Assert.Equal(9999u, a.m_ShowNumberLableTimeTick);
        Assert.Equal(0, a.m_NumberLableImageInfo!.Width);
        Assert.Equal(0, a.m_NumberLableImageInfo.Height);
        Assert.False(a.m_NumberLableImageInfo.HasImage);
        Assert.Equal("旧值", a.m_sCurNumberLableText);
    }

    [Fact]
    public void LoadNumberLableSurface_EmptyTextStillLoadsViaFont()
    {
        var a = new TActor();
        a.m_sNumberLableText = "";

        a.LoadNumberLableSurface();

        // 原文无空串守卫，直接 GetImageInfo('')
        Assert.NotNull(a.m_NumberLableImageInfo);
        Assert.Equal("", a.m_sCurNumberLableText);
    }

    [Fact]
    public void LoadNumberLableSurface_NullImageInfoYieldsZeroedSurface()
    {
        TActorCore.GetImageInfoFn = _ => null;

        var a = new TActor();
        a.m_sNumberLableText = "abc";

        a.LoadNumberLableSurface();

        Assert.NotNull(a.m_NumberLableImageInfo);
        Assert.Equal(0, a.m_NumberLableImageInfo!.Width);
    }
}

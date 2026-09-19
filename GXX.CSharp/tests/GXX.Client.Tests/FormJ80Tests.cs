using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J80：Actor.pas 飘血（伤害数字）系统 1:1 测试 ——
/// CheckLoadHealthNumber(9212-9305)、LoadHealthNumber(9035-9203)、
/// ShowHealthNumber(8667-8749)、ClearHealthNumber(9205-9210)、CheckLoadNumberLable(9307-9384)。
/// </summary>
public sealed class ActorHealthNumberTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public ActorHealthNumberTests()
    {
        var saved = (TActorCore.CanvasReadyFn, TActorCore.PlugInEnabled,
            TActorCore.HealthNumberImages, TActorCore.HealthNumberTickFn, TActorCore.CheckLoadTickFn,
            TActorCore.HpAddUnitFn,
            TActorCore.boHealthNumberText, TActorCore.boShowMoveLable, TActorCore.boShowNumberLable,
            TActorCore.boHumStruckShowNumber, TActorCore.boMonStruckShowNumber,
            TActorCore.boShowHPUnit, TActorCore.boShowJobAndLevel,
            TActorCore.nHealthNumberMoveSpeed, TActorCore.nHealthNumberOffsetX, TActorCore.nHealthNumberOffsetY,
            TActorCore.ckShowMoveLable, TActorCore.ckShowNumberLable,
            TActorCore.ckShowHPUnit, TActorCore.ckShowJobAndLevel,
            TActorCore.nHealthNumberSelfOffset, TActorCore.nHealthNumberHumOffset,
            TActorCore.boHealthNumberSeparate, TActorCore.g_boMapHumAndHeroPercentHP);

        _restore.Add(() =>
        {
            TActorCore.CanvasReadyFn = saved.Item1;
            TActorCore.PlugInEnabled = saved.Item2;
            TActorCore.HealthNumberImages = saved.Item3;
            TActorCore.HealthNumberTickFn = saved.Item4;
            TActorCore.CheckLoadTickFn = saved.Item5;
            TActorCore.HpAddUnitFn = saved.Item6;
            TActorCore.boHealthNumberText = saved.Item7;
            TActorCore.boShowMoveLable = saved.Item8;
            TActorCore.boShowNumberLable = saved.Item9;
            TActorCore.boHumStruckShowNumber = saved.Item10;
            TActorCore.boMonStruckShowNumber = saved.Item11;
            TActorCore.boShowHPUnit = saved.Item12;
            TActorCore.boShowJobAndLevel = saved.Item13;
            TActorCore.nHealthNumberMoveSpeed = saved.Item14;
            TActorCore.nHealthNumberOffsetX = saved.Item15;
            TActorCore.nHealthNumberOffsetY = saved.Item16;
            TActorCore.ckShowMoveLable = saved.Item17;
            TActorCore.ckShowNumberLable = saved.Item18;
            TActorCore.ckShowHPUnit = saved.Item19;
            TActorCore.ckShowJobAndLevel = saved.Item20;
            TActorCore.nHealthNumberSelfOffset = saved.Item21;
            TActorCore.nHealthNumberHumOffset = saved.Item22;
            TActorCore.boHealthNumberSeparate = saved.Item23;
            TActorCore.g_boMapHumAndHeroPercentHP = saved.Item24;
        });

        TActorCore.CanvasReadyFn = () => true;
        TActorCore.PlugInEnabled = true;
        TActorCore.HealthNumberImages = null;
        TActorCore.HealthNumberTickFn = () => 1000;
        TActorCore.CheckLoadTickFn = () => 1000;
        TActorCore.HpAddUnitFn = v => v.ToString();
        TActorCore.boHealthNumberText = false;
        TActorCore.boShowMoveLable = true;
        TActorCore.boShowNumberLable = false;
        TActorCore.boHumStruckShowNumber = false;
        TActorCore.boMonStruckShowNumber = false;
        TActorCore.boShowHPUnit = false;
        TActorCore.boShowJobAndLevel = false;
        TActorCore.nHealthNumberMoveSpeed = 100;
        TActorCore.nHealthNumberOffsetX = 0;
        TActorCore.nHealthNumberOffsetY = 0;
        TActorCore.ckShowMoveLable = true;
        TActorCore.ckShowNumberLable = false;
        TActorCore.ckShowHPUnit = false;
        TActorCore.ckShowJobAndLevel = false;
        TActorCore.nHealthNumberSelfOffset = 0;
        TActorCore.nHealthNumberHumOffset = 0;
        TActorCore.boHealthNumberSeparate = false;
        TActorCore.g_boMapHumAndHeroPercentHP = false;
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    private sealed class FakeImages : IHealthNumberImageSource
    {
        public readonly Dictionary<int, LabelSurface?> Newop = new();
        public readonly Dictionary<(int, int), LabelSurface?> Effect = new();

        public LabelSurface? GetNewopUIImage(int index)
            => Newop.TryGetValue(index, out var v) ? v : null;

        public LabelSurface? GetEffectImageListTexture(int resId, int index)
            => Effect.TryGetValue((resId, index), out var v) ? v : null;
    }

    /// <summary>所有数字图等宽，便于精确断言累计宽度。</summary>
    private static FakeImages Uniform(int w = 10, int h = 14, int from = 0, int to = 2200)
    {
        var f = new FakeImages();
        for (int i = from; i <= to; i++)
            f.Newop[i] = LabelSurface.Of(w, h);
        return f;
    }

    private static TActor MakeActor()
    {
        var a = new TActor();
        a.m_nSayX = 100;
        a.m_nSayY = 200;
        a.m_boCanDraw = true;
        return a;
    }

    private static THealthNumber Slot(TActor a, int i)
    {
        a.EnsureHealthNumberArray();
        return a.m_HealthNumberArray[i];
    }

    // ===================== 常量与数组布局 =====================

    [Fact]
    public void NumberTypeTablesMatchOriginal()
    {
        Assert.Equal(new[] { 0, 1, 2, 3, 4, 14, 24, 34, 44, 54 }, TActorCore.NumberTypeIndexs);
        Assert.Equal(new[] { 1, 1, 1, 1, 10, 10, 10, 10, 10, 10 }, TActorCore.NumberTypeCounts);
        Assert.Equal(65, TActorCore.HealthNumberArraySize);
    }

    [Fact]
    public void EnumValuesMatchOriginal()
    {
        Assert.Equal(0, (int)TNumberType.tHP);
        Assert.Equal(3, (int)TNumberType.tMiss);
        Assert.Equal(9, (int)TNumberType.tFatalBlow4);
        Assert.Equal(0, (int)TNumberDrawStyle.ndsNormal);
        Assert.Equal(3, (int)TNumberDrawStyle.ndsStayFadeOut);
    }

    [Fact]
    public void EnsureHealthNumberArray_Allocates65SlotsOnceWithResIdMinusOne()
    {
        var a = MakeActor();
        a.EnsureHealthNumberArray();
        a.EnsureHealthNumberArray();

        Assert.Equal(65, a.m_HealthNumberArray.Count);
        Assert.All(a.m_HealthNumberArray, s => Assert.Equal(-1, s.nResID));
        Assert.All(a.m_HealthNumberArray, s => Assert.Equal(-1, s.nResStartIdx));
    }

    // ===================== ClearHealthNumber =====================

    [Fact]
    public void ClearHealthNumber_OnlyClearsWidthIndexesAndNumber()
    {
        var hn = new THealthNumber
        {
            nWidth = 30, nHeight = 14, sNumber = "123",
            nOffsetX = 5, nOffsetY = -7, byAlpha = 99,
        };
        hn.ImageIndexs.Add(11);

        TActorCore.ClearHealthNumber(hn);

        Assert.Equal(0, hn.nWidth);
        Assert.Empty(hn.ImageIndexs);
        Assert.Equal("", hn.sNumber);
        // 原文不动这些字段
        Assert.Equal(14, hn.nHeight);
        Assert.Equal(5, hn.nOffsetX);
        Assert.Equal(-7, hn.nOffsetY);
        Assert.Equal(99, hn.byAlpha);
    }

    // ===================== CheckLoadHealthNumber =====================

    [Fact]
    public void CheckLoadHealthNumber_CanvasNotReadyReturnsFalse()
    {
        TActorCore.CanvasReadyFn = () => false;
        var a = MakeActor();
        Assert.False(a.CheckLoadHealthNumber());
    }

    [Fact]
    public void CheckLoadHealthNumber_AllThreeSwitchesRequired()
    {
        var a = MakeActor();

        TActorCore.boShowMoveLable = true; TActorCore.ckShowMoveLable = true;
        Assert.True(a.CheckLoadHealthNumber());        // 开关 false→true

        TActorCore.boShowMoveLable = false;
        Assert.False(a.CheckLoadHealthNumber());

        TActorCore.PlugInEnabled = false;
        TActorCore.boShowMoveLable = true;
        Assert.False(a.CheckLoadHealthNumber());
    }

    [Fact]
    public void CheckLoadHealthNumber_TurnOnReturnsTrueOnceThenFalse()
    {
        var a = MakeActor();

        Assert.True(a.CheckLoadHealthNumber());        // 首次开启
        Assert.True(a.m_boShowHealthNumber);
        Assert.False(a.CheckLoadHealthNumber());       // 已同步且无待载槽
    }

    [Fact]
    public void CheckLoadHealthNumber_ZeroWidthSlotRequestsLoad()
    {
        var a = MakeActor();
        var hn = Slot(a, 0);
        hn.sNumber = "12";
        hn.nWidth = 0;

        Assert.True(a.CheckLoadHealthNumber());
    }

    [Fact]
    public void CheckLoadHealthNumber_MoveSpeedGateBlocksAdvance()
    {
        var a = MakeActor();
        var hn = Slot(a, 0);
        hn.sNumber = "1";
        hn.nWidth = 5;
        hn.dwUpdateHPTick = 1000;
        TActorCore.CheckLoadTickFn = () => 1050;       // 50 < 100 → 不推进
        TActorCore.nHealthNumberMoveSpeed = 100;

        a.CheckLoadHealthNumber();

        Assert.Equal(0, hn.nOffsetX);
        Assert.Equal(0, hn.nOffsetY);
    }

    [Fact]
    public void CheckLoadHealthNumber_MoveSpeedGateAllowsAdvanceAndStampsTick()
    {
        var a = MakeActor();
        var hn = Slot(a, 0);
        hn.sNumber = "1";
        hn.nWidth = 5;
        hn.dwUpdateHPTick = 1000;
        hn.nDrawStyle = (int)TNumberDrawStyle.ndsNormal;
        TActorCore.CheckLoadTickFn = () => 1100;       // 100 >= 100

        a.CheckLoadHealthNumber();

        Assert.Equal(1, hn.nOffsetX);
        Assert.Equal(-1, hn.nOffsetY);
        Assert.Equal(1100u, hn.dwUpdateHPTick);
    }

    [Fact]
    public void CheckLoadHealthNumber_NormalStyleClearsAtFifty()
    {
        var a = MakeActor();
        var hn = Slot(a, 0);
        hn.sNumber = "1"; hn.nWidth = 5;
        hn.nDrawStyle = (int)TNumberDrawStyle.ndsNormal;
        TActorCore.nHealthNumberOffsetY = 0;
        TActorCore.CheckLoadTickFn = () => 99999;
        hn.nOffsetY = -49;                             // 推进后 -50 → |−50| >= 50

        a.CheckLoadHealthNumber();

        Assert.Equal("", hn.sNumber);                  // 已 Clear
    }

    [Fact]
    public void CheckLoadHealthNumber_FastExitClearsAtTwenty()
    {
        var a = MakeActor();
        var hn = Slot(a, 0);
        hn.sNumber = "1"; hn.nWidth = 5;
        hn.nDrawStyle = (int)TNumberDrawStyle.ndsFastExit;
        TActorCore.CheckLoadTickFn = () => 99999;
        hn.nOffsetY = -19;

        a.CheckLoadHealthNumber();

        Assert.Equal("", hn.sNumber);
        Assert.Equal(0, hn.nOffsetX);                  // FastExit 不动 X
    }

    [Fact]
    public void CheckLoadHealthNumber_MovingFadeOutStepsByTwoAndFadesAlpha()
    {
        var a = MakeActor();
        var hn = Slot(a, 0);
        hn.sNumber = "1"; hn.nWidth = 5;
        hn.nDrawStyle = (int)TNumberDrawStyle.ndsMovingFadeOut;
        TActorCore.CheckLoadTickFn = () => 99999;
        TActorCore.nHealthNumberOffsetX = 0;
        TActorCore.nHealthNumberOffsetY = -200;        // 远离退场阈值

        a.CheckLoadHealthNumber();

        Assert.Equal(2, hn.nOffsetX);
        Assert.Equal(-2, hn.nOffsetY);
        // byAlpha = max(0, 255 - |2-0|*3) = 249
        Assert.Equal(249, hn.byAlpha);
    }

    [Fact]
    public void CheckLoadHealthNumber_MovingFadeOutAlphaFloorIsZero()
    {
        var a = MakeActor();
        var hn = Slot(a, 0);
        hn.sNumber = "1"; hn.nWidth = 5;
        hn.nDrawStyle = (int)TNumberDrawStyle.ndsMovingFadeOut;
        TActorCore.CheckLoadTickFn = () => 99999;
        TActorCore.nHealthNumberOffsetX = 0;
        TActorCore.nHealthNumberOffsetY = -200;
        hn.nOffsetX = 200;                             // |202-0|*3 = 606 > 255

        a.CheckLoadHealthNumber();

        Assert.Equal(0, hn.byAlpha);
    }

    [Fact]
    public void CheckLoadHealthNumber_StayFadeOutAdvancesWhileBelowThirty()
    {
        var a = MakeActor();
        var hn = Slot(a, 0);
        hn.sNumber = "1"; hn.nWidth = 5;
        hn.nDrawStyle = (int)TNumberDrawStyle.ndsStayFadeOut;
        TActorCore.CheckLoadTickFn = () => 99999;
        TActorCore.nHealthNumberOffsetY = 0;
        hn.nOffsetY = -10;                             // |−10| < 30 → 继续升

        a.CheckLoadHealthNumber();

        Assert.Equal(1, hn.nOffsetX);
        Assert.Equal(-11, hn.nOffsetY);
    }

    [Fact]
    public void CheckLoadHealthNumber_StayFadeOutDecaysAlphaAndClears()
    {
        var a = MakeActor();
        var hn = Slot(a, 0);
        hn.sNumber = "1"; hn.nWidth = 5;
        hn.nDrawStyle = (int)TNumberDrawStyle.ndsStayFadeOut;
        TActorCore.CheckLoadTickFn = () => 99999;
        TActorCore.nHealthNumberOffsetY = 0;
        hn.nOffsetY = -40;                             // |−40| >= 30 → 渐隐
        hn.byAlpha = 30;                               // 30 >= 24 → 30-24 = 6 → <= 24 → Clear

        a.CheckLoadHealthNumber();

        Assert.Equal("", hn.sNumber);
    }

    [Fact]
    public void CheckLoadHealthNumber_StayFadeOutAlphaUnder24GoesToZeroThenClears()
    {
        var a = MakeActor();
        var hn = Slot(a, 0);
        hn.sNumber = "1"; hn.nWidth = 5;
        hn.nDrawStyle = (int)TNumberDrawStyle.ndsStayFadeOut;
        TActorCore.CheckLoadTickFn = () => 99999;
        TActorCore.nHealthNumberOffsetY = 0;
        hn.nOffsetY = -40;
        hn.byAlpha = 10;                               // < 24 → 置 0 → <= 24 → Clear

        a.CheckLoadHealthNumber();

        Assert.Equal(0, hn.byAlpha);
        Assert.Equal("", hn.sNumber);
    }

    [Fact]
    public void CheckLoadHealthNumber_EmptyNumberSlotIsSkippedEntirely()
    {
        var a = MakeActor();
        var hn = Slot(a, 0);
        hn.sNumber = "";                               // 空 → 完全不处理
        hn.nWidth = 0;
        TActorCore.CheckLoadTickFn = () => 99999;

        a.CheckLoadHealthNumber();

        Assert.Equal(0, hn.nOffsetX);
    }

    // ===================== LoadHealthNumber 索引解析 =====================

    private static TActor SetupUnit(TActor a, int slot, TNumberType type, string num, int number)
    {
        var hn = Slot(a, slot);
        hn.sNumber = num;
        hn.nNumber = number;
        hn.nNmType = (int)type;
        hn.nWidth = 0;
        hn.nHeight = 0;
        hn.nResID = -1;
        hn.nResStartIdx = -1;
        return a;
    }

    [Theory]
    [InlineData(TNumberType.tHP, 50)]
    [InlineData(TNumberType.tMP, 70)]
    [InlineData(TNumberType.tGreen, 90)]
    [InlineData(TNumberType.tTextHP, 500)]
    [InlineData(TNumberType.tBlastHP, 520)]
    [InlineData(TNumberType.tFatalBlow1, 1670)]
    [InlineData(TNumberType.tFatalBlow2, 1690)]
    [InlineData(TNumberType.tFatalBlow3, 1710)]
    [InlineData(TNumberType.tFatalBlow4, 1730)]
    public void LoadHealthNumber_DefaultOffsetIndexPerType(TNumberType type, int expectedBase)
    {
        var a = SetupUnit(MakeActor(), 0, type, "7", 7);
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        var hn = Slot(a, 0);
        // 正数 → 首图 = base + 11
        Assert.Equal(expectedBase + 11, hn.ImageIndexs[0]);
    }

    [Fact]
    public void LoadHealthNumber_MissUsesBaseIndexDirectly()
    {
        // tMiss 走 9137 的单图分支，直接用 nOffsetIndex（不 +10/+11）
        var a = SetupUnit(MakeActor(), 0, TNumberType.tMiss, "7", 7);
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        var hn = Slot(a, 0);
        Assert.Single(hn.ImageIndexs);
        Assert.Equal(204, hn.ImageIndexs[0]);
    }

    [Fact]
    public void LoadHealthNumber_NegativeNumberUsesPlusTen()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "7", -7);
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        Assert.Equal(50 + 10, Slot(a, 0).ImageIndexs[0]);
    }

    [Fact]
    public void LoadHealthNumber_AccumulatesWidthAndMaxHeightPlusTwo()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "12", 12);
        TActorCore.HealthNumberImages = Uniform(w: 10, h: 14);

        a.LoadHealthNumber();

        var hn = Slot(a, 0);
        // 符号 + 两位数字 = 3 张，每张宽 10
        Assert.Equal(30, hn.nWidth);
        Assert.Equal(14 + 2, hn.nHeight);              // Max(高) + 2
        Assert.Equal(3, hn.ImageIndexs.Count);
    }

    [Fact]
    public void LoadHealthNumber_DigitIndexUsesDigitValue()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "5", 5);
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        var hn = Slot(a, 0);
        Assert.Equal(50 + 11, hn.ImageIndexs[0]);      // 符号
        Assert.Equal(50 + 5, hn.ImageIndexs[1]);       // 数字 5
    }

    [Fact]
    public void LoadHealthNumber_MissDrawsSingleImageWithoutHeightBump()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tMiss, "x", 0);
        TActorCore.HealthNumberImages = Uniform(w: 10, h: 14);

        a.LoadHealthNumber();

        var hn = Slot(a, 0);
        Assert.Single(hn.ImageIndexs);
        Assert.Equal(204, hn.ImageIndexs[0]);
        Assert.Equal(10, hn.nWidth);
        Assert.Equal(14, hn.nHeight);                  // Miss 分支**不加 2**
    }

    [Fact]
    public void LoadHealthNumber_MissingTextureSkipsThatDigit()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "1", 1);
        var f = new FakeImages();
        f.Newop[50 + 11] = LabelSurface.Of(10, 14);    // 只有符号图
        TActorCore.HealthNumberImages = f;

        a.LoadHealthNumber();

        var hn = Slot(a, 0);
        Assert.Single(hn.ImageIndexs);
        Assert.Equal(10, hn.nWidth);
    }

    [Fact]
    public void LoadHealthNumber_OnlyLoadsWhenWidthIsZero()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "1", 1);
        Slot(a, 0).nWidth = 5;                         // 已有宽度 → 不重载
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        Assert.Empty(Slot(a, 0).ImageIndexs);
    }

    [Fact]
    public void LoadHealthNumber_EmptyNumberSlotSkipped()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "", 1);
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        Assert.Empty(Slot(a, 0).ImageIndexs);
    }

    // ---- 自定义资源分支 ----

    [Fact]
    public void LoadHealthNumber_CustomResUsesEffectImages()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "3", 3);
        var hn = Slot(a, 0);
        hn.nResID = 7;
        hn.nResStartIdx = 100;

        var f = new FakeImages();
        for (int i = 0; i < 20; i++)
            f.Effect[(7, 100 + i)] = LabelSurface.Of(10, 14);
        TActorCore.HealthNumberImages = f;

        a.LoadHealthNumber();

        Assert.Equal(100 + 11, hn.ImageIndexs[0]);     // 符号
        Assert.Equal(100 + 3, hn.ImageIndexs[1]);      // 数字 3
        Assert.Equal(20, hn.nWidth);
    }

    [Fact]
    public void LoadHealthNumber_CustomResWithNegativeStartIdxLoadsNothing()
    {
        // nResID >= 0 → 9104 令 nOffsetIndex := nResStartIdx = -1；
        // 9110 的 `nResStartIdx >= 0` 不成立 → 落到 9136 else，但 nOffsetIndex < 0
        // 使 9109 整个装载条件不成立 → 不装载任何图
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "3", 3);
        var hn = Slot(a, 0);
        hn.nResID = 7;
        hn.nResStartIdx = -1;

        var f = new FakeImages();
        f.Newop[50 + 11] = LabelSurface.Of(10, 14);
        TActorCore.HealthNumberImages = f;

        a.LoadHealthNumber();

        Assert.Empty(hn.ImageIndexs);
        Assert.Equal(0, hn.nWidth);
    }

    // ---- boHealthNumberSeparate ----

    [Fact]
    public void LoadHealthNumber_SeparateSelfUsesSelfOffset()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "1", 1);
        a.MySelfRef2 = a;
        TActorCore.boHealthNumberSeparate = true;
        TActorCore.nHealthNumberSelfOffset = 200;
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        Assert.Equal(200 + 11, Slot(a, 0).ImageIndexs[0]);
    }

    [Fact]
    public void LoadHealthNumber_SeparateSelfMpStepsByTwenty()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tMP, "1", 1);
        a.MySelfRef2 = a;
        TActorCore.boHealthNumberSeparate = true;
        TActorCore.nHealthNumberSelfOffset = 200;
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        Assert.Equal(220 + 11, Slot(a, 0).ImageIndexs[0]);
    }

    [Fact]
    public void LoadHealthNumber_SeparateHeroAlsoUsesSelfOffset()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "1", 1);
        a.MyHeroRef2 = a;
        TActorCore.boHealthNumberSeparate = true;
        TActorCore.nHealthNumberSelfOffset = 300;
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        Assert.Equal(300 + 11, Slot(a, 0).ImageIndexs[0]);
    }

    [Fact]
    public void LoadHealthNumber_SeparateOtherHumanUsesHumOffset()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "1", 1);
        a.m_btRace = 0;
        a.MySelfRef2 = new TActor();                   // 自己另有其人
        TActorCore.boHealthNumberSeparate = true;
        TActorCore.nHealthNumberHumOffset = 400;
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        Assert.Equal(400 + 11, Slot(a, 0).ImageIndexs[0]);
    }

    [Fact]
    public void LoadHealthNumber_SeparateHumanoidMonsterFallsThrough()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "1", 1);
        a.m_btRace = 0;
        a.m_boPlayMoster = true;                       // 人形怪 → 不适用 HumOffset
        a.MySelfRef2 = new TActor();
        TActorCore.boHealthNumberSeparate = true;
        TActorCore.nHealthNumberHumOffset = 400;
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        Assert.Equal(50 + 11, Slot(a, 0).ImageIndexs[0]);   // 保持默认基址
    }

    [Fact]
    public void LoadHealthNumber_SeparateFatalBlowNotOverridden()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tFatalBlow1, "1", 1);
        a.MySelfRef2 = a;
        TActorCore.boHealthNumberSeparate = true;
        TActorCore.nHealthNumberSelfOffset = 200;
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        // 原文 tFatalBlow1..4 的自定义偏移被注释掉 → 仍用 1670
        Assert.Equal(1670 + 11, Slot(a, 0).ImageIndexs[0]);
    }

    [Fact]
    public void LoadHealthNumber_SeparateWithCustomResKeepsResStartIdx()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "1", 1);
        var hn = Slot(a, 0);
        hn.nResID = 9;
        hn.nResStartIdx = 60;
        a.MySelfRef2 = a;
        TActorCore.boHealthNumberSeparate = true;
        TActorCore.nHealthNumberSelfOffset = 200;

        var f = new FakeImages();
        f.Effect[(9, 71)] = LabelSurface.Of(10, 14);
        TActorCore.HealthNumberImages = f;

        a.LoadHealthNumber();

        Assert.Equal(71, hn.ImageIndexs[0]);           // 60 + 11
    }

    [Fact]
    public void LoadHealthNumber_DisabledSwitchSkipsEntirely()
    {
        var a = SetupUnit(MakeActor(), 0, TNumberType.tHP, "1", 1);
        TActorCore.ckShowMoveLable = false;
        TActorCore.HealthNumberImages = Uniform();

        a.LoadHealthNumber();

        Assert.Empty(Slot(a, 0).ImageIndexs);
    }

    // ===================== ShowHealthNumber =====================

    [Fact]
    public void ShowHealthNumber_FlagOffClearsAllSlots()
    {
        var a = MakeActor();
        var hn = Slot(a, 0);
        hn.nWidth = 30; hn.nHeight = 14;
        hn.ImageIndexs.Add(1);
        a.m_boShowHealthNumber = false;

        var ops = a.ShowHealthNumber();

        Assert.Empty(ops);
        Assert.Equal(0, hn.nWidth);
        Assert.Equal(0, hn.nHeight);
        Assert.Empty(hn.ImageIndexs);
    }

    [Fact]
    public void ShowHealthNumber_PositionsBySayXYAndOffsets()
    {
        var a = MakeActor();
        a.m_boShowHealthNumber = true;
        var hn = Slot(a, 0);
        hn.nWidth = 30; hn.nHeight = 14;
        hn.nOffsetX = 5; hn.nOffsetY = -3;
        hn.ImageIndexs.Add(1);
        TActorCore.HealthNumberImages = Uniform(w: 10, h: 14);

        var ops = a.ShowHealthNumber();

        // nX = 100 - 15 + 5 = 90 ; nY = 200 - 15 - 14 - 3 = 168
        Assert.Single(ops);
        Assert.Equal(90, ops[0].X);
        Assert.Equal(168, ops[0].Y);
    }

    [Fact]
    public void ShowHealthNumber_AdvancesXByEachTextureWidth()
    {
        var a = MakeActor();
        a.m_boShowHealthNumber = true;
        var hn = Slot(a, 0);
        hn.nWidth = 30; hn.nHeight = 14;
        hn.ImageIndexs.Add(1);
        hn.ImageIndexs.Add(2);
        hn.ImageIndexs.Add(3);
        TActorCore.HealthNumberImages = Uniform(w: 10, h: 14);

        var ops = a.ShowHealthNumber();

        Assert.Equal(3, ops.Count);
        Assert.Equal(85, ops[0].X);                    // 100 - 15 + 0
        Assert.Equal(95, ops[1].X);
        Assert.Equal(105, ops[2].X);
    }

    [Fact]
    public void ShowHealthNumber_SkipsSlotsWithZeroWidth()
    {
        var a = MakeActor();
        a.m_boShowHealthNumber = true;
        var hn = Slot(a, 0);
        hn.nWidth = 0;
        hn.ImageIndexs.Add(1);                         // 有索引但宽度 0 → 不绘
        TActorCore.HealthNumberImages = Uniform();

        Assert.Empty(a.ShowHealthNumber());
    }

    [Fact]
    public void ShowHealthNumber_DeadWithMovingFadeOutShiftsUpByThirtyFive()
    {
        var a = MakeActor();
        a.m_boShowHealthNumber = true;
        a.m_boDeath = true;
        a.m_dwDeathTick = 1000;
        TActorCore.HealthNumberTickFn = () => 2000;    // 10 秒内
        TActorCore.boHealthNumberText = true;

        var hn = Slot(a, 0);
        hn.nWidth = 30; hn.nHeight = 14;
        hn.nDrawStyle = (int)TNumberDrawStyle.ndsMovingFadeOut;
        hn.ImageIndexs.Add(1);
        TActorCore.HealthNumberImages = Uniform(w: 10, h: 14);

        var ops = a.ShowHealthNumber();

        // nY = 200-15-14+0 = 171，死亡再 -35 = 136
        Assert.Equal(136, ops[0].Y);
    }

    [Fact]
    public void ShowHealthNumber_DeadOtherStyleNotShifted()
    {
        var a = MakeActor();
        a.m_boShowHealthNumber = true;
        a.m_boDeath = true;
        a.m_dwDeathTick = 1000;
        TActorCore.HealthNumberTickFn = () => 2000;
        TActorCore.boHealthNumberText = true;

        var hn = Slot(a, 0);
        hn.nWidth = 30; hn.nHeight = 14;
        hn.nDrawStyle = (int)TNumberDrawStyle.ndsNormal;
        hn.ImageIndexs.Add(1);
        TActorCore.HealthNumberImages = Uniform(w: 10, h: 14);

        Assert.Equal(171, a.ShowHealthNumber()[0].Y);
    }

    [Fact]
    public void ShowHealthNumber_DeadWithoutTextBeyondTenSecondsDrawsNothing()
    {
        var a = MakeActor();
        a.m_boShowHealthNumber = true;
        a.m_boDeath = true;
        a.m_dwDeathTick = 1000;
        TActorCore.HealthNumberTickFn = () => 20000;   // > 10 秒
        TActorCore.boHealthNumberText = false;

        var hn = Slot(a, 0);
        hn.nWidth = 30; hn.nHeight = 14;
        hn.ImageIndexs.Add(1);
        TActorCore.HealthNumberImages = Uniform();

        Assert.Empty(a.ShowHealthNumber());
    }

    [Fact]
    public void ShowHealthNumber_DeadWithinTenSecondsNeedsTextFlag()
    {
        var a = MakeActor();
        a.m_boShowHealthNumber = true;
        a.m_boDeath = true;
        a.m_dwDeathTick = 1000;
        TActorCore.HealthNumberTickFn = () => 2000;
        TActorCore.boHealthNumberText = false;         // 无文本开关 → 两个 or 子句皆假

        var hn = Slot(a, 0);
        hn.nWidth = 30; hn.nHeight = 14;
        hn.ImageIndexs.Add(1);
        TActorCore.HealthNumberImages = Uniform();

        Assert.Empty(a.ShowHealthNumber());
    }

    [Fact]
    public void ShowHealthNumber_AliveDrawsRegardlessOfTextFlag()
    {
        var a = MakeActor();
        a.m_boShowHealthNumber = true;
        a.m_boDeath = false;
        TActorCore.boHealthNumberText = false;

        var hn = Slot(a, 0);
        hn.nWidth = 30; hn.nHeight = 14;
        hn.ImageIndexs.Add(1);
        TActorCore.HealthNumberImages = Uniform();

        Assert.Single(a.ShowHealthNumber());
    }

    [Fact]
    public void ShowHealthNumber_NotCanDrawSuppressesButDoesNotClear()
    {
        var a = MakeActor();
        a.m_boShowHealthNumber = true;
        a.m_boCanDraw = false;
        var hn = Slot(a, 0);
        hn.nWidth = 30; hn.nHeight = 14;
        hn.ImageIndexs.Add(1);

        var ops = a.ShowHealthNumber();

        Assert.Empty(ops);
        Assert.Equal(30, hn.nWidth);                   // 未清（清空只在 flag=false 分支）
    }

    [Fact]
    public void ShowHealthNumber_CustomResUsesEffectImages()
    {
        var a = MakeActor();
        a.m_boShowHealthNumber = true;
        var hn = Slot(a, 0);
        hn.nWidth = 30; hn.nHeight = 14;
        hn.nResID = 5;
        hn.ImageIndexs.Add(11);

        var f = new FakeImages();
        f.Effect[(5, 11)] = LabelSurface.Of(10, 14);
        TActorCore.HealthNumberImages = f;

        var ops = a.ShowHealthNumber();

        Assert.Single(ops);
        Assert.Equal(11, ops[0].ImageIndex);
    }

    [Fact]
    public void ShowHealthNumber_AlphaIsForwarded()
    {
        var a = MakeActor();
        a.m_boShowHealthNumber = true;
        var hn = Slot(a, 0);
        hn.nWidth = 30; hn.nHeight = 14;
        hn.byAlpha = 77;
        hn.ImageIndexs.Add(1);
        TActorCore.HealthNumberImages = Uniform();

        Assert.Equal(77, a.ShowHealthNumber()[0].Alpha);
    }

    // ===================== CheckLoadNumberLable =====================

    [Fact]
    public void CheckLoadNumberLable_CanvasNotReadyReturnsFalse()
    {
        TActorCore.CanvasReadyFn = () => false;
        Assert.False(MakeActor().CheckLoadNumberLable());
    }

    [Fact]
    public void CheckLoadNumberLable_DeadProducesEmptyText()
    {
        var a = MakeActor();
        a.m_boDeath = true;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;

        a.CheckLoadNumberLable();

        Assert.Equal("", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_SelfNeedsSwitchForFraction()
    {
        // 9330-9333：自己只满足**外层**门；内层仍需 (boShowNumberLable and ckShowNumberLable) or m_boOpenHealth
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;

        a.CheckLoadNumberLable();
        Assert.Equal("", a.m_sNumberLableText);        // 两开关皆关

        TActorCore.boShowNumberLable = true;
        TActorCore.ckShowNumberLable = true;
        a.CheckLoadNumberLable();
        Assert.Equal("50/100", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_OpenHealthBypassesSwitchGate()
    {
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_boOpenHealth = true;

        a.CheckLoadNumberLable();

        Assert.Equal("50/100", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_OtherHumanNeedsNumberLableSwitch()
    {
        var a = MakeActor();
        a.MySelfRef2 = new TActor();
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        TActorCore.boShowNumberLable = false;
        TActorCore.ckShowNumberLable = false;

        a.CheckLoadNumberLable();

        Assert.Equal("", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_OtherHumanPercentMode()
    {
        var a = MakeActor();
        a.MySelfRef2 = new TActor();
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        TActorCore.boShowNumberLable = true;
        TActorCore.ckShowNumberLable = true;
        TActorCore.g_boMapHumAndHeroPercentHP = true;

        a.CheckLoadNumberLable();

        Assert.Equal("50%", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_PercentIsCappedAtHundred()
    {
        var a = MakeActor();
        a.MySelfRef2 = new TActor();
        a.m_Abil.HP = 500; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        TActorCore.boShowNumberLable = true;
        TActorCore.ckShowNumberLable = true;
        TActorCore.g_boMapHumAndHeroPercentHP = true;

        a.CheckLoadNumberLable();

        Assert.Equal("100%", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_SelfWithoutOpenHealthNeverUsesPercent()
    {
        // 9335-9336：`((Self <> MySelf) and (Self <> MyHero)) or m_boOpenHealth`
        // 自己且 m_boOpenHealth=false → 子句假 → 走分数
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        TActorCore.boShowNumberLable = true;
        TActorCore.ckShowNumberLable = true;
        TActorCore.g_boMapHumAndHeroPercentHP = true;

        a.CheckLoadNumberLable();

        Assert.Equal("50/100", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_SelfWithOpenHealthDoesUsePercent()
    {
        // m_boOpenHealth=true 让第二个子句成立 → 即使自己是 Self 也走百分比
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_boOpenHealth = true;
        TActorCore.g_boMapHumAndHeroPercentHP = true;

        a.CheckLoadNumberLable();

        Assert.Equal("50%", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_OtherHumanWithOpenHealthUsesPercent()
    {
        var a = MakeActor();
        a.MySelfRef2 = new TActor();
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_boOpenHealth = true;
        TActorCore.g_boMapHumAndHeroPercentHP = true;

        a.CheckLoadNumberLable();

        Assert.Equal("50%", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_HpUnitFormat()
    {
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_boOpenHealth = true;
        TActorCore.boShowHPUnit = true;
        TActorCore.ckShowHPUnit = true;
        TActorCore.HpAddUnitFn = v => v + "W";

        a.CheckLoadNumberLable();

        Assert.Equal("50W/100W", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_JobAndLevelSuffix()
    {
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_btJob = 2;
        a.m_boOpenHealth = true;
        TActorCore.boShowJobAndLevel = true;
        TActorCore.ckShowJobAndLevel = true;

        a.CheckLoadNumberLable();

        Assert.Equal("50/100/D30", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_JobSuffixWithoutFractionHasNoSlashPrefix()
    {
        // 9362-9374：只有 m_sNumberLableText 非空时才补 '/'；等级 > 0 即可单独出现职业后缀
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_btJob = 2;                                  // 两开关皆关 → 分数为空
        TActorCore.boShowJobAndLevel = true;
        TActorCore.ckShowJobAndLevel = true;

        a.CheckLoadNumberLable();

        Assert.Equal("D30", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_UnknownJobGetsUnKnow()
    {
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_btJob = 9;
        a.m_boOpenHealth = true;
        TActorCore.boShowJobAndLevel = true;
        TActorCore.ckShowJobAndLevel = true;

        a.CheckLoadNumberLable();

        Assert.Equal("50/100/UnKnow30", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_MonsterShowsFractionWithoutNpc()
    {
        var a = MakeActor();
        a.m_btRace = 80;
        a.m_Abil.HP = 30; a.m_Abil.MaxHP = 60;
        a.m_boOpenHealth = true;

        a.CheckLoadNumberLable();

        Assert.Equal("30/60", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_MerchantNeverShowsLabel()
    {
        var a = MakeActor();
        a.m_btRace = ActorLabelConsts.RC_MERCHANT;
        a.m_Abil.HP = 30; a.m_Abil.MaxHP = 60;
        a.m_boOpenHealth = true;

        a.CheckLoadNumberLable();

        Assert.Equal("", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_ZeroMaxHpSuppressesLabel()
    {
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 0; a.m_Abil.MaxHP = 0;
        a.m_Abil.Level = 30;

        a.CheckLoadNumberLable();

        Assert.Equal("", a.m_sNumberLableText);
    }

    [Fact]
    public void CheckLoadNumberLable_TimeoutForcesReload()
    {
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_boOpenHealth = true;
        a.m_sCurNumberLableText = "50/100";
        a.m_ShowNumberLableTimeTick = 1000;
        TActorCore.HealthNumberTickFn = () => 2000;    // > 30

        Assert.True(a.CheckLoadNumberLable());
    }

    [Fact]
    public void CheckLoadNumberLable_NoChangeNoTimeoutReturnsFalse()
    {
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_Abil.MP = 10; a.m_Abil.MaxMP = 20;
        a.m_boOpenHealth = true;
        a.m_OAbil = a.m_Abil;
        a.m_sCurNumberLableText = "50/100";
        a.m_ShowNumberLableTimeTick = 1000;
        TActorCore.HealthNumberTickFn = () => 1010;    // ≤ 30

        Assert.False(a.CheckLoadNumberLable());
    }

    [Fact]
    public void CheckLoadNumberLable_HpChangeForcesReload()
    {
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_boOpenHealth = true;
        a.m_OAbil = a.m_Abil;
        a.m_sCurNumberLableText = "50/100";
        a.m_ShowNumberLableTimeTick = 1000;
        TActorCore.HealthNumberTickFn = () => 1010;

        a.m_Abil.HP = 40;

        Assert.True(a.CheckLoadNumberLable());
    }

    [Fact]
    public void CheckLoadNumberLable_MaxHpChangeForcesReload()
    {
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_boOpenHealth = true;
        a.m_OAbil = a.m_Abil;
        a.m_sCurNumberLableText = "50/100";
        a.m_ShowNumberLableTimeTick = 1000;
        TActorCore.HealthNumberTickFn = () => 1010;

        a.m_Abil.MaxHP = 200;

        Assert.True(a.CheckLoadNumberLable());
    }

    [Fact]
    public void CheckLoadNumberLable_TextCompareIsCaseInsensitive()
    {
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_btJob = 0;                                  // → "Z"
        a.m_boOpenHealth = true;
        TActorCore.boShowJobAndLevel = true;
        TActorCore.ckShowJobAndLevel = true;
        a.m_OAbil = a.m_Abil;
        a.m_sCurNumberLableText = "50/100/z30";         // 小写
        a.m_ShowNumberLableTimeTick = 1000;
        TActorCore.HealthNumberTickFn = () => 1010;

        Assert.False(a.CheckLoadNumberLable());         // CompareText 忽略大小写
    }

    [Fact]
    public void CheckLoadNumberLable_MaxMpClampAppliesToLocalCopy()
    {
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.MP = 90; a.m_Abil.MaxMP = 10;
        a.m_Abil.Level = 30;

        a.CheckLoadNumberLable();

        Assert.Equal(10u, a.m_Abil.MaxMP);              // 不写回
    }

    [Fact]
    public void CheckLoadNumberLable_UpdatesOAbilOnReload()
    {
        var a = MakeActor();
        a.MySelfRef2 = a;
        a.m_Abil.HP = 50; a.m_Abil.MaxHP = 100;
        a.m_Abil.Level = 30;
        a.m_ShowNumberLableTimeTick = 1000;
        TActorCore.HealthNumberTickFn = () => 2000;

        a.CheckLoadNumberLable();

        Assert.Equal(a.m_Abil.HP, a.m_OAbil.HP);
    }
}

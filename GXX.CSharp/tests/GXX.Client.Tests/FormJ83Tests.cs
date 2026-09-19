using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J83：NearActorHintEffect.pas 全类 + PlayScn.pas 邻近提示/大血条渲染 1:1 测试 ——
/// TNearActorHintEffectMgr、DrawNearActorHintEffect(4003-4027)、DrawCurMonBigHPProgress(4208-4370)。
/// </summary>
public sealed class BigHpProgressTests
{
    private static FxImage Img(int idx, int w = 20, int h = 10, int ox = 0, int oy = 0)
        => new(idx, w, h, ox, oy, false);

    private static TNearActorHintEffectMgr Mgr(uint now = 1000)
        => new() { TickFn = () => now };

    private static TMonHPProgress Info(
        int bgIndex = 0, short bgX = 0, short bgY = 0,
        int imageIndex = -1, short imageX = 0, short imageY = 0,
        int hpIndex = 0, short hpX = 0, short hpY = 0,
        ushort blockCount = 0, short blockOffX = 0, short blockOffY = 0,
        byte align = 0, string expName = "",
        short showLevel = -1, short showMonName = -1, short showHPValue = -1,
        short showHPPercent = -1, short showExpHinter = -1)
    {
        var i = new TMonHPProgress
        {
            nHPBGIndex = bgIndex, nHPBGX = bgX, nHPBGY = bgY,
            nImageIndex = imageIndex, nImageX = imageX, nImageY = imageY,
            nHPIndex = hpIndex, nHPX = hpX, nHPY = hpY,
            wHPBlockCount = blockCount, nHPBlockOffsetX = blockOffX, nHPBlockOffsetY = blockOffY,
            btHorizAlign = align,
            boShowLevel = showLevel, nLevelX = 0, nLevelY = 0,
            boShowMonName = showMonName, nMonNameX = 0, nMonNameY = 0,
            boShowHPValue = showHPValue, nHPValueX = 0, nHPValueY = 0,
            boShowHPPercent = showHPPercent, nHPPercentX = 0, nHPPercentY = 0,
            boShowExpHinter = showExpHinter, nExpHinterX = 0, nExpHinterY = 0,
        };
        i.ExpHinterName = expName;
        return i;
    }

    private static BigHpProgressRender.BigHpResult Run(
        bool hidden = false, uint lastAttack = 0, uint now = 1000,
        bool found = true, bool death = false, bool ghost = false, bool showBig = true,
        uint hp = 50, uint maxHp = 100, uint level = 30, string name = "怪",
        TMonHPProgress? info = null, int screenWidth = 800,
        Func<int, FxImage?>? resolve = null, int barW = 100)
    {
        return BigHpProgressRender.DrawCurMonBigHPProgress(
            hidden, lastAttack, now, found, death, ghost, showBig,
            hp, maxHp, level, name, info ?? Info(), screenWidth,
            resolve ?? (_ => Img(0, 100, 20)), barW);
    }

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchOriginal()
    {
        Assert.Equal(144, BigHpProgressRender.NormalStartIndex);
        Assert.Equal(154, BigHpProgressRender.FriendStartIndex);
        Assert.Equal(150, TNearActorHintEffectMgr.FrameTickCount);
        Assert.Equal(5000, BigHpProgressRender.ShowWindowMs);
        Assert.Equal(760, BigHpProgressRender.HpBgImageBase);
        Assert.Equal(780, BigHpProgressRender.HpImageBase);
        Assert.Equal(770, BigHpProgressRender.HpBarImageBase);
        Assert.Equal(749, BigHpProgressRender.HpBlockIconIndex);
        Assert.Equal(750, BigHpProgressRender.HpBlockDigitBase);
    }

    // ===================== TNearActorHintEffectMgr =====================

    [Fact]
    public void NewManagerIsEmpty()
    {
        var m = Mgr();
        Assert.Equal(0, m.InfoCount);
        Assert.Equal(-1, m.LatestInfoIndex);      // 130-133：空表为 -1
        Assert.Equal(0, m.FrameIndex);
    }

    [Fact]
    public void AddActorHintStoresFieldsAndIncrements()
    {
        var m = Mgr();
        m.AddActorHint(11, 22, 1);

        Assert.Equal(1, m.InfoCount);
        var info = m.GetHintInfo(0);
        Assert.NotNull(info);
        Assert.Equal(11, info!.X);
        Assert.Equal(22, info.Y);
        Assert.Equal(1, info.nFriendFlag);
    }

    [Fact]
    public void AddActorHintGrowsPastInitialCapacity()
    {
        var m = Mgr();
        for (int i = 0; i < 250; i++)
            m.AddActorHint(i, i, 0);

        Assert.Equal(250, m.InfoCount);
        Assert.Equal(249, m.GetHintInfo(249)!.X);
    }

    [Fact]
    public void DeleteLatestHintDecrementsAndFloorsAtZero()
    {
        var m = Mgr();
        m.AddActorHint(1, 1, 0);
        m.DeleteLatestHint();
        Assert.Equal(0, m.InfoCount);

        m.DeleteLatestHint();                     // 已空 → 不再递减
        Assert.Equal(0, m.InfoCount);
    }

    [Fact]
    public void CleaerHintZeroesCountAndSetsFrameCountToSix()
    {
        // 65-69：原文拼写 CleaerHint；帧计数置 6 而非 0
        var m = Mgr();
        m.AddActorHint(1, 1, 0);
        m.CleaerHint();

        Assert.Equal(0, m.InfoCount);
        Assert.Equal(6u, m.FrameCount);
    }

    [Fact]
    public void GetHintInfoNegativeIndexReturnsLatest()
    {
        var m = Mgr();
        m.AddActorHint(1, 1, 0);
        m.AddActorHint(2, 2, 0);

        var info = m.GetHintInfo(-1);
        Assert.NotNull(info);
        Assert.Equal(2, info!.X);
    }

    [Fact]
    public void GetHintInfoOutOfRangeReturnsNull()
    {
        var m = Mgr();
        m.AddActorHint(1, 1, 0);

        Assert.Null(m.GetHintInfo(1));
        Assert.Null(m.GetHintInfo(99));
    }

    [Fact]
    public void GetHintInfoOnEmptyManagerReturnsNull()
    {
        var m = Mgr();
        Assert.Null(m.GetHintInfo(0));
        Assert.Null(m.GetHintInfo(-1));
    }

    [Fact]
    public void FrameDriveFirstCallOnlyStampsTick()
    {
        // 94-96：首次 m_dwLastHintEffectTick = 0 → 仅记 tick 并清零帧索引
        var m = Mgr(now: 5000);
        m.FrameDrive();

        Assert.Equal(0, m.FrameIndex);
    }

    [Fact]
    public void FrameDriveAdvancesOnlyAfterTickCount()
    {
        uint now = 1000;
        var m = new TNearActorHintEffectMgr { TickFn = () => now };
        m.FrameCount = 10;                         // 必须 > 1，否则推进即回卷
        m.FrameDrive();                            // 打点

        now = 1000 + 149;                          // 未达 150
        m.FrameDrive();
        Assert.Equal(0, m.FrameIndex);

        now = 1000 + 150;
        m.FrameDrive();
        Assert.Equal(1, m.FrameIndex);
    }

    [Fact]
    public void FrameDriveWrapsAtFrameCount()
    {
        uint now = 1000;
        var m = new TNearActorHintEffectMgr { TickFn = () => now };
        m.FrameCount = 3;
        m.FrameDrive();                            // 打点，帧 0

        for (int i = 1; i <= 3; i++)
        {
            now = (uint)(1000 + 150 * i);
            m.FrameDrive();
        }
        Assert.Equal(0, m.FrameIndex);             // 帧达 3 后回卷
    }

    [Fact]
    public void FrameDriveNeverWrapsWhenFrameCountIsZero()
    {
        // FrameCount 默认 0 → `FrameIndex >= 0` 恒真 → 每次推进后立即回卷 0
        uint now = 1000;
        var m = new TNearActorHintEffectMgr { TickFn = () => now };
        m.FrameDrive();

        now = 2000;
        m.FrameDrive();

        Assert.Equal(0, m.FrameIndex);
    }

    [Fact]
    public void FrameCountIsReadWrite()
    {
        var m = Mgr();
        m.FrameCount = 12;
        Assert.Equal(12u, m.FrameCount);
    }

    // ===================== DrawNearActorHintEffect =====================

    [Fact]
    public void NoInfoDrawsNothingButStillDrivesFrames()
    {
        // 4026 在循环之外
        uint now = 1000;
        var m = new TNearActorHintEffectMgr { TickFn = () => now };
        var ops = BigHpProgressRender.DrawNearActorHintEffect(m, _ => Img(0));

        Assert.Empty(ops);

        m.FrameCount = 1;
        now = 2000;
        BigHpProgressRender.DrawNearActorHintEffect(m, _ => Img(0));
        Assert.Equal(0, m.FrameIndex);             // 推进 1 后回卷
    }

    [Fact]
    public void NormalFlagUsesBase144()
    {
        var m = Mgr();
        m.AddActorHint(10, 20, 0);

        int asked = -1;
        BigHpProgressRender.DrawNearActorHintEffect(m, i => { asked = i; return Img(i); });

        Assert.Equal(144, asked);
    }

    [Fact]
    public void FriendFlagUsesBase154()
    {
        var m = Mgr();
        m.AddActorHint(10, 20, 7);                 // 任意非 0

        int asked = -1;
        BigHpProgressRender.DrawNearActorHintEffect(m, i => { asked = i; return Img(i); });

        Assert.Equal(154, asked);
    }

    [Fact]
    public void ImageIndexAddsManagerFrameIndex()
    {
        var m = Mgr();
        m.FrameCount = 5;
        m.AddActorHint(0, 0, 0);
        m.FrameIndex.ToString(); // no-op
        // 手动推进帧索引到 1
        uint now = 1000;
        m.TickFn = () => now;
        m.FrameDrive();
        now = 1150;
        m.FrameDrive();

        int asked = -1;
        BigHpProgressRender.DrawNearActorHintEffect(m, i => { asked = i; return Img(i); });

        Assert.Equal(145, asked);                  // 144 + 1
    }

    [Fact]
    public void DrawUsesInfoCoordsPlusOrigin()
    {
        var m = Mgr();
        m.AddActorHint(100, 200, 0);

        var ops = BigHpProgressRender.DrawNearActorHintEffect(m, _ => Img(0, 20, 10, 3, 4));

        Assert.Single(ops);
        Assert.Equal(103, ops[0].X);
        Assert.Equal(204, ops[0].Y);
    }

    [Fact]
    public void MissingTextureSkipsThatInfo()
    {
        var m = Mgr();
        m.AddActorHint(1, 1, 0);

        Assert.Empty(BigHpProgressRender.DrawNearActorHintEffect(m, _ => null));
    }

    [Fact]
    public void AllInfosDrawnInOrder()
    {
        var m = Mgr();
        m.AddActorHint(1, 1, 0);
        m.AddActorHint(2, 2, 1);
        m.AddActorHint(3, 3, 0);

        var ops = BigHpProgressRender.DrawNearActorHintEffect(m, _ => Img(0));

        Assert.Equal(3, ops.Count);
        Assert.Equal(1, ops[0].X);
        Assert.Equal(2, ops[1].X);
        Assert.Equal(3, ops[2].X);
    }

    // ===================== 大血条外层守卫 =====================

    [Fact]
    public void HiddenSwitchSuppressesEverything()
    {
        var r = Run(hidden: true);
        Assert.Empty(r.Images);
        Assert.Empty(r.Texts);
    }

    [Fact]
    public void BeyondFiveSecondsSuppressesEverything()
    {
        var r = Run(lastAttack: 0, now: 5001);
        Assert.Empty(r.Images);
        Assert.Empty(r.Texts);
    }

    [Fact]
    public void ExactlyFiveSecondsStillDraws()
    {
        // 4223 是 `<= 5000`
        var r = Run(lastAttack: 0, now: 5000);
        Assert.NotEmpty(r.Images);
    }

    [Fact]
    public void ActorNotFoundSuppresses()
    {
        var r = Run(found: false);
        Assert.Empty(r.Images);
    }

    [Fact]
    public void DeathOrGhostSuppresses()
    {
        Assert.Empty(Run(death: true).Images);
        Assert.Empty(Run(ghost: true).Images);
    }

    [Fact]
    public void BigHpFlagOffSuppresses()
    {
        Assert.Empty(Run(showBig: false).Images);
    }

    [Fact]
    public void ZeroMaxHpSuppresses()
    {
        Assert.Empty(Run(maxHp: 0).Images);
    }

    // ===================== 背景与对齐 =====================

    [Fact]
    public void BackgroundWidthComesFromTexture()
    {
        var info = Info(bgIndex: 2, align: 1);
        var r = Run(info: info, screenWidth: 800, resolve: _ => Img(0, 100, 20));

        // (800 - 100) / 2 + 0 = 350
        Assert.Equal(350, r.Images[0].X);
        Assert.Equal(762, r.Images[0].ImageIndex);
    }

    [Fact]
    public void AlignTwoIsRightAligned()
    {
        var r = Run(info: Info(align: 2), screenWidth: 800, resolve: _ => Img(0, 100, 20));
        Assert.Equal(700, r.Images[0].X);          // 800 - 100
    }

    [Fact]
    public void AlignZeroIsLeftAligned()
    {
        var r = Run(info: Info(bgX: 17, align: 0), screenWidth: 800, resolve: _ => Img(0, 100, 20));
        Assert.Equal(17, r.Images[0].X);
    }

    [Fact]
    public void MissingBackgroundGivesZeroWidth()
    {
        var r = Run(info: Info(align: 1), screenWidth: 800, resolve: _ => null);
        Assert.Empty(r.Images);                    // 背景与图像皆未命中
    }

    [Fact]
    public void BackgroundIndexOutOfRangeNotDrawn()
    {
        var r = Run(info: Info(bgIndex: 10, imageIndex: -1, hpIndex: -1), resolve: _ => Img(0));
        Assert.Empty(r.Images);
    }

    [Fact]
    public void BackgroundIndexNegativeNotDrawn()
    {
        var r = Run(info: Info(bgIndex: -1, imageIndex: -1, hpIndex: -1), resolve: _ => Img(0));
        Assert.Empty(r.Images);
    }

    // ===================== 图像层 =====================

    [Fact]
    public void ImageDrawnWithOffset()
    {
        var r = Run(info: Info(bgIndex: -1, imageIndex: 3, imageX: 5, imageY: 7, hpIndex: -1),
            resolve: _ => Img(0));
        Assert.Single(r.Images);
        Assert.Equal(5, r.Images[0].X);
        Assert.Equal(7, r.Images[0].Y);
        Assert.Equal(783, r.Images[0].ImageIndex);
    }

    [Fact]
    public void ImageIndexAtHundredNotDrawn()
    {
        var r = Run(info: Info(bgIndex: -1, imageIndex: 100, hpIndex: -1), resolve: _ => Img(0));
        Assert.Empty(r.Images);
    }

    // ===================== 单块血条（wHPBlockCount <= 1） =====================

    [Fact]
    public void SingleBarWidthScalesWithHpRatio()
    {
        var r = Run(hp: 50, maxHp: 100, info: Info(bgIndex: -1, imageIndex: -1, blockCount: 1),
            resolve: _ => Img(0, 100, 20));

        Assert.Single(r.Images);
        Assert.Equal(50, r.Images[0].SrcRight);    // Round(100 / 100 * 50)
    }

    [Fact]
    public void SingleBarZeroBlockCountStillTakesSinglePath()
    {
        // 4272：`<= 1`，故 0 也走单条
        var r = Run(hp: 25, maxHp: 100, info: Info(bgIndex: -1, imageIndex: -1, blockCount: 0),
            resolve: _ => Img(0, 100, 20));

        Assert.Single(r.Images);
        Assert.Equal(25, r.Images[0].SrcRight);
    }

    [Fact]
    public void SingleBarHpIndexOutOfRangeNotDrawn()
    {
        var r = Run(info: Info(bgIndex: -1, imageIndex: -1, hpIndex: 10, blockCount: 1),
            resolve: _ => Img(0));
        Assert.Empty(r.Images);
    }

    [Fact]
    public void SinglePathShowsTotalHpText()
    {
        var r = Run(hp: 50, maxHp: 100,
            info: Info(bgIndex: -1, imageIndex: -1, blockCount: 1, showHPValue: 255),
            resolve: _ => Img(0, 100, 20));

        Assert.Single(r.Texts);
        Assert.Equal("50/100", r.Texts[0].Text);
        Assert.Equal("HPValue", r.Texts[0].Kind);
    }

    // ===================== 分块血条 =====================

    [Fact]
    public void CalcHpBlockRoundsUpBlockSize()
    {
        // MaxHP 100 / 3 块 → (100+2)/3 = 34
        var (size, idx, pre, cur, curMax) = BigHpProgressRender.CalcHpBlock(100, 100, 3);
        Assert.Equal(34u, size);
        Assert.Equal(3u, idx);                     // 100/34 = 2 余 32 → 3
        Assert.Equal(68u, pre);
        Assert.Equal(32u, cur);
        Assert.Equal(34u, curMax);
    }

    [Fact]
    public void CalcHpBlockZeroHpGivesIndexOne()
    {
        // HP 0 → idx = 0, 余 0 → else if idx < 1 → 1
        var (size, idx, pre, cur, curMax) = BigHpProgressRender.CalcHpBlock(0, 100, 3);
        Assert.Equal(34u, size);
        Assert.Equal(1u, idx);
        Assert.Equal(0u, pre);
        Assert.Equal(0u, cur);
        Assert.Equal(34u, curMax);
    }

    [Fact]
    public void CalcHpBlockExactMultipleDoesNotIncrement()
    {
        // HP 68 / 34 = 2 余 0 → idx 2，不 +1
        var (_, idx, pre, cur, curMax) = BigHpProgressRender.CalcHpBlock(68, 100, 3);
        Assert.Equal(2u, idx);
        Assert.Equal(34u, pre);
        Assert.Equal(34u, cur);
        Assert.Equal(34u, curMax);
    }

    [Fact]
    public void CalcHpBlockPinsCurMaxWhenIndexEqualsBlockSize()
    {
        // 4304：`HPBlockIndex = HPBlockSize` 时 CurMax = MaxHP - PreSize
        // MaxHP 12 / 4 块 → size 3；HP 12 → idx 4；4 = 4 → curMax = 12 - 9 = 3
        var (size, idx, pre, cur, curMax) = BigHpProgressRender.CalcHpBlock(12, 12, 4);
        Assert.Equal(3u, size);
        Assert.Equal(4u, idx);
        Assert.Equal(9u, pre);
        Assert.Equal(3u, cur);
        Assert.Equal(3u, curMax);
    }

    [Fact]
    public void CalcHpBlockSizeFloorIsOne()
    {
        // MaxHP 1 / 10 块 → (1+9)/10 = 1，不小于 1
        var (size, _, _, _, _) = BigHpProgressRender.CalcHpBlock(1, 1, 10);
        Assert.Equal(1u, size);
    }

    [Fact]
    public void MultiBlockDrawsPreviousBlockWhenIndexAboveOne()
    {
        var r = Run(hp: 100, maxHp: 100, info: Info(bgIndex: -1, imageIndex: -1, blockCount: 3),
            resolve: _ => Img(0, 100, 20));

        Assert.Contains(r.Images, o => o.Kind == "HpBarPrev");
        Assert.Contains(r.Images, o => o.Kind == "HpBarCur");
    }

    [Fact]
    public void MultiBlockFirstBlockHasNoPrevious()
    {
        var r = Run(hp: 10, maxHp: 100, info: Info(bgIndex: -1, imageIndex: -1, blockCount: 3),
            resolve: _ => Img(0, 100, 20));

        Assert.DoesNotContain(r.Images, o => o.Kind == "HpBarPrev");
        Assert.Contains(r.Images, o => o.Kind == "HpBarCur");
    }

    [Fact]
    public void MultiBlockPreviousUsesWrappedIndex()
    {
        // idx = 3 → prev = (3 - 2 + 10) % 10 = 1 → 771
        var r = Run(hp: 100, maxHp: 100, info: Info(bgIndex: -1, imageIndex: -1, blockCount: 3),
            resolve: _ => Img(0, 100, 20));

        var prev = r.Images.Find(o => o.Kind == "HpBarPrev");
        Assert.NotNull(prev);
        Assert.Equal(771, prev!.ImageIndex);
    }

    [Fact]
    public void MultiBlockCurrentUsesWrappedIndex()
    {
        // idx = 3 → cur = (3 - 1) % 10 = 2 → 772
        var r = Run(hp: 100, maxHp: 100, info: Info(bgIndex: -1, imageIndex: -1, blockCount: 3),
            resolve: _ => Img(0, 100, 20));

        var cur = r.Images.Find(o => o.Kind == "HpBarCur");
        Assert.NotNull(cur);
        Assert.Equal(772, cur!.ImageIndex);
    }

    [Fact]
    public void MultiBlockCurrentWidthUsesCurSizeAndCurMax()
    {
        // HP 100, MaxHP 100, 3 块 → size 34, idx 3, cur 32, curMax 34
        var r = Run(hp: 100, maxHp: 100, info: Info(bgIndex: -1, imageIndex: -1, blockCount: 3),
            resolve: _ => Img(0, 100, 20));

        var cur = r.Images.Find(o => o.Kind == "HpBarCur");
        // Round(100 / 34 * 32) = Round(94.11) = 94
        Assert.Equal(94, cur!.SrcRight);
    }

    [Fact]
    public void MultiBlockValueTextUsesSegmentSizes()
    {
        var r = Run(hp: 100, maxHp: 100,
            info: Info(bgIndex: -1, imageIndex: -1, blockCount: 3, showHPValue: 255),
            resolve: _ => Img(0, 100, 20));

        var v = r.Texts.Find(t => t.Kind == "HPValue");
        Assert.NotNull(v);
        Assert.Equal("32/34", v!.Text);            // nCurSize / nCurMaxSize
    }

    [Fact]
    public void MultiBlockDigitsDrawnWithIconFirst()
    {
        // idx = 3 → 图标 749 + 字形 750+3 = 753
        var r = Run(hp: 100, maxHp: 100, info: Info(bgIndex: -1, imageIndex: -1, blockCount: 3),
            resolve: _ => Img(0, 100, 20));

        var digits = r.Images.FindAll(o => o.Kind == "HpBlockDigit");
        Assert.Equal(2, digits.Count);
        Assert.Equal(749, digits[0].ImageIndex);   // 首位图标
        Assert.Equal(753, digits[1].ImageIndex);   // 数字 '3'
    }

    [Fact]
    public void MultiBlockDigitsAdvanceByEachWidth()
    {
        var r = Run(hp: 100, maxHp: 100, info: Info(bgIndex: -1, imageIndex: -1, blockCount: 3),
            resolve: _ => Img(0, 30, 20));

        var digits = r.Images.FindAll(o => o.Kind == "HpBlockDigit");
        Assert.Equal(0, digits[0].X);
        Assert.Equal(30, digits[1].X);             // 累加首个宽
    }

    [Fact]
    public void MultiBlockTwoDigitIndexDrawsThreeImages()
    {
        // 需要 idx >= 10：MaxHP 100 / 10 块 → size 10；HP 100 → idx 10
        var r = Run(hp: 100, maxHp: 100, info: Info(bgIndex: -1, imageIndex: -1, blockCount: 10),
            resolve: _ => Img(0, 20, 10));

        var digits = r.Images.FindAll(o => o.Kind == "HpBlockDigit");
        Assert.Equal(3, digits.Count);             // 图标 + '1' + '0'
        Assert.Equal(750 + 1, digits[1].ImageIndex);
        Assert.Equal(750 + 0, digits[2].ImageIndex);
    }

    [Fact]
    public void MultiBlockMissingDigitImageIsSkippedButStillCounted()
    {
        var r = Run(hp: 100, maxHp: 100, info: Info(bgIndex: -1, imageIndex: -1, blockCount: 3),
            resolve: i => i == 749 ? null : Img(i, 20, 10));

        var digits = r.Images.FindAll(o => o.Kind == "HpBlockDigit");
        Assert.Single(digits);                     // 图标缺 → 不产 op
        Assert.Equal(753, digits[0].ImageIndex);
    }

    // ===================== HpBarWidth / 文本格式 =====================

    [Fact]
    public void HpBarWidthScalesProportionally()
    {
        Assert.Equal(50, BigHpProgressRender.HpBarWidth(100, 50, 100));
        Assert.Equal(100, BigHpProgressRender.HpBarWidth(100, 100, 100));
        Assert.Equal(0, BigHpProgressRender.HpBarWidth(100, 0, 100));
    }

    [Fact]
    public void HpBarWidthZeroDenominatorKeepsFullWidth()
    {
        Assert.Equal(100, BigHpProgressRender.HpBarWidth(100, 5, 0));
    }

    [Fact]
    public void HpBarWidthUsesBankersRounding()
    {
        // 100 / 3 * 1 = 33.33 → 33
        Assert.Equal(33, BigHpProgressRender.HpBarWidth(100, 1, 3));
        // 100 / 8 * 4 = 50 → 50
        Assert.Equal(50, BigHpProgressRender.HpBarWidth(100, 4, 8));
    }

    [Fact]
    public void HpPercentTextRounds()
    {
        Assert.Equal("50%", BigHpProgressRender.HpPercentText(50, 100));
        Assert.Equal("100%", BigHpProgressRender.HpPercentText(100, 100));
        // Round(1/3*100) = Round(33.33) = 33
        Assert.Equal("33%", BigHpProgressRender.HpPercentText(1, 3));
        // 向后兼容：超过上限仍按实算（原文无 Min 夹紧）
        Assert.Equal("200%", BigHpProgressRender.HpPercentText(200, 100));
    }

    [Fact]
    public void HpValueTextFormat()
    {
        Assert.Equal("50/100", BigHpProgressRender.HpValueText(50, 100));
        Assert.Equal("0/0", BigHpProgressRender.HpValueText(0, 0));
    }

    [Fact]
    public void ExpHinterTextFormat()
    {
        Assert.Equal("归属(张三)", BigHpProgressRender.ExpHinterText("张三"));
        Assert.Equal("归属()", BigHpProgressRender.ExpHinterText(""));
    }

    // ===================== 四个可选项文本 =====================

    [Fact]
    public void LevelTextDrawnWhenInRange()
    {
        var r = Run(level: 42, info: Info(bgIndex: -1, imageIndex: -1, showLevel: 255));
        var t = r.Texts.Find(x => x.Kind == "Level");
        Assert.NotNull(t);
        Assert.Equal("42", t!.Text);
    }

    [Fact]
    public void LevelTextNotDrawnWhenNegative()
    {
        var r = Run(info: Info(bgIndex: -1, imageIndex: -1, showLevel: -1));
        Assert.DoesNotContain(r.Texts, x => x.Kind == "Level");
    }

    [Fact]
    public void LevelTextNotDrawnWhenAboveTwoFiftyFive()
    {
        var r = Run(info: Info(bgIndex: -1, imageIndex: -1, showLevel: 256));
        Assert.DoesNotContain(r.Texts, x => x.Kind == "Level");
    }

    [Fact]
    public void MonNameTextDrawn()
    {
        var r = Run(name: "祖玛教主", info: Info(bgIndex: -1, imageIndex: -1, showMonName: 100));
        var t = r.Texts.Find(x => x.Kind == "MonName");
        Assert.NotNull(t);
        Assert.Equal("祖玛教主", t!.Text);
    }

    [Fact]
    public void PercentTextUsesFullHp()
    {
        // 4362：百分比恒用 Abil.HP / Abil.MaxHP（分块模式下亦然）
        var r = Run(hp: 50, maxHp: 100,
            info: Info(bgIndex: -1, imageIndex: -1, blockCount: 3, showHPPercent: 255));

        var t = r.Texts.Find(x => x.Kind == "HPPercent");
        Assert.NotNull(t);
        Assert.Equal("50%", t!.Text);
    }

    [Fact]
    public void ExpHinterTextUsesRecordName()
    {
        var r = Run(info: Info(bgIndex: -1, imageIndex: -1, showExpHinter: 255, expName: "李四"));
        var t = r.Texts.Find(x => x.Kind == "ExpHinter");
        Assert.NotNull(t);
        Assert.Equal("归属(李四)", t!.Text);
    }

    [Fact]
    public void AllFourOptionalTextsCanCoexist()
    {
        var r = Run(name: "怪", level: 7, hp: 50, maxHp: 100,
            info: Info(bgIndex: -1, imageIndex: -1, expName: "王五",
                showLevel: 1, showMonName: 2, showHPValue: 3, showHPPercent: 4, showExpHinter: 5));

        Assert.Contains(r.Texts, t => t.Kind == "Level");
        Assert.Contains(r.Texts, t => t.Kind == "MonName");
        Assert.Contains(r.Texts, t => t.Kind == "HPValue");
        Assert.Contains(r.Texts, t => t.Kind == "HPPercent");
        Assert.Contains(r.Texts, t => t.Kind == "ExpHinter");
    }

    [Fact]
    public void DrawOrderIsBackgroundImageBarThenTexts()
    {
        var r = Run(info: Info(bgIndex: 0, imageIndex: 1, hpIndex: 0, blockCount: 1,
            showLevel: 1), resolve: _ => Img(0, 100, 20));

        Assert.Equal("Background", r.Images[0].Kind);
        Assert.Equal("Image", r.Images[1].Kind);
        Assert.Equal("HpBar", r.Images[2].Kind);
    }

    [Fact]
    public void MissingBarTextureSkipsBarOnly()
    {
        var r = Run(info: Info(bgIndex: 0, imageIndex: -1, hpIndex: 0, blockCount: 1, showLevel: 1),
            resolve: i => i == 770 ? null : Img(i, 100, 20));

        Assert.DoesNotContain(r.Images, o => o.Kind == "HpBar");
        Assert.Contains(r.Images, o => o.Kind == "Background");
        Assert.Contains(r.Texts, t => t.Kind == "Level");
    }
}

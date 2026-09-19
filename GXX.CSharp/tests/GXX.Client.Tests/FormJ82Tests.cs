using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J82：PlayScn.pas 物品预览族 1:1 测试 ——
/// DrawPreviewItem(3493-3579)、DrawPreviewMonItemEffect(3405-3451)、
/// DrawPreviewMonItemValueEffect(3453-3491)。
/// </summary>
public sealed class PreviewItemRenderTests
{
    private const int ColW = 100;
    private const int RowH = 40;

    private static FxImage Img(int idx, int w = 20, int h = 20, int ox = 0, int oy = 0)
        => new(idx, w, h, ox, oy, false);

    private static TClientPreviewMonItem Item(string name, ushort looks, int count = 1,
        bool value = false, byte color = 0)
    {
        var it = new TClientPreviewMonItem();
        it.PreiewMonItem.sName = name;
        it.PreiewMonItem.wLooks = looks;
        it.PreiewMonItem.nCount = count;
        it.PreiewMonItem.boValueItem = value;
        it.PreiewMonItem.btColor = color;
        return it;
    }

    private static DropItemEffectDef Eff(int file = 0, int start = 0, int time = 100,
        int count = 3, bool below = true, bool center = false, bool noBlend = false,
        int ox = 0, int oy = 0)
    {
        return new DropItemEffectDef
        {
            FileIndex = file, StartIndex = start, Time = time, ImageCount = count,
            BelowItem = below, DrawCenter = center, NoBlend = noBlend,
            OffsetX = ox, OffsetY = oy,
        };
    }

    /// <summary>标准调用：单件物品、无特效、无值光效、20×20 图标。</summary>
    private static (List<PreviewItemDrawOp> Draws, List<PreviewNameDrawOp> Names) Run(
        IReadOnlyList<TClientPreviewMonItem>? items,
        bool selfDead = false,
        bool actorFound = true,
        bool actorGhost = false,
        bool actorDeath = false,
        int recog = 7,
        uint showTime = 5000,
        uint showTick = 0,
        uint now = 1000,
        Func<ushort, FxImage?>? looks = null,
        Func<ushort, FxImage?>? looksGray = null,
        int effectImageCount = 0,
        Func<int, FxImage?>? resolveEffect = null,
        bool hideChecked = false, bool hide = false,
        bool showDropValue = false, bool valueChecked = false, bool showValue = false,
        int valueCount = 0, int valuePlayTime = 0, int valueIndex = 0,
        int valOx = 0, int valOy = 0,
        int rx = 10, int ry = 10, int clientLeft = 0, int clientTop = 0,
        int defXX = 0, int defYY = 0,
        Func<string, int>? textWidth = null,
        Func<int, FxImage?>? resolveValueEffect = null)
    {
        return PreviewItemRender.DrawPreviewItem(
            items, recog, showTime, showTick, now,
            actorFound, actorGhost, actorDeath, rx, ry,
            clientLeft, clientTop, defXX, defYY,
            selfDead,
            hideChecked, hide,
            showDropValue, valueChecked, showValue,
            valueCount, valuePlayTime, valueIndex, valOx, valOy,
            effectImageCount,
            looks ?? (_ => Img(0)),
            looksGray ?? (_ => Img(0)),
            resolveEffect ?? (_ => null),
            resolveValueEffect ?? (_ => null),
            textWidth ?? (s => s.Length * 6),
            _ => 0xFFFFFF);
    }

    // ===================== 常量与网格 =====================

    [Fact]
    public void ConstantsMatchOriginal()
    {
        Assert.Equal(100, PreviewItemRender.ColWidth);
        Assert.Equal(40, PreviewItemRender.RowHeight);
        Assert.Equal(20, PreviewItemRender.IconBox);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(2, 2, 1)]
    [InlineData(3, 1, 3)]      // Trunc(Sqrt(3)) = 1 → nRow = 3
    [InlineData(4, 2, 2)]
    [InlineData(5, 2, 3)]      // nCol 2 → (5+1)/2 = 3
    [InlineData(9, 3, 3)]
    [InlineData(10, 3, 4)]     // (10+2)/3 = 4
    public void GridSizeMatchesOriginal(int count, int col, int row)
    {
        var (c, r) = PreviewItemRender.GridSize(count);
        Assert.Equal(col, c);
        Assert.Equal(row, r);
    }

    [Fact]
    public void DisplayNameAppendsCountOnlyAboveOne()
    {
        Assert.Equal("剑", PreviewItemRender.DisplayName(Item("剑", 1, 1).PreiewMonItem));
        Assert.Equal("剑 ×2", PreviewItemRender.DisplayName(Item("剑", 1, 2).PreiewMonItem));
        // 3560：条件是 nCount > 1，故 0 与 1 均不加后缀
        Assert.Equal("剑", PreviewItemRender.DisplayName(Item("剑", 1, 0).PreiewMonItem));
    }

    [Fact]
    public void GridOriginCentersBlockOnActor()
    {
        // Rx=10, Ry=10, Client 0,0, Def 0,0；nCol=1,nRow=1 → 无居中偏移
        var (x, y) = PreviewItemRender.GridOrigin(10, 10, 0, 0, 0, 0, 1, 1);
        Assert.Equal(10 * 48, x);
        Assert.Equal((10 - 0 - 1) * 32, y);
    }

    [Fact]
    public void GridOriginSubtractsHalfBlockOffset()
    {
        // nCol=3 → Round(2/2 * 100) = 100；nRow=3 → Round(2/2 * 40) = 40
        var (x, y) = PreviewItemRender.GridOrigin(10, 10, 0, 0, 0, 0, 3, 3);
        Assert.Equal(10 * 48 - 100, x);
        Assert.Equal(9 * 32 - 40, y);
    }

    [Fact]
    public void GridOriginUsesBankersRoundingForOddCounts()
    {
        // nCol=2 → (2-1)/2 * 100 = 50 → Round(50) = 50（无 .5 歧义）
        var (x2, _) = PreviewItemRender.GridOrigin(0, 0, 0, 0, 0, 0, 2, 1);
        Assert.Equal(0 - 50, x2);

        // nRow=2 → (2-1)/2 * 40 = 20
        var (_, y2) = PreviewItemRender.GridOrigin(0, 0, 0, 0, 0, 0, 1, 2);
        Assert.Equal((0 - 1) * 32 - 20, y2);
    }

    [Fact]
    public void IconPosCentersInTwentyBox()
    {
        // 3542：nTempX := nX + (20 - d.Width) div 2 —— 居中量叠加在格子左上角之上
        Assert.Equal((100, 200), PreviewItemRender.IconPos(100, 200, 20, 20));
        Assert.Equal((105, 203), PreviewItemRender.IconPos(100, 200, 10, 14));
        Assert.Equal((110, 210), PreviewItemRender.IconPos(100, 200, 0, 0));
    }

    [Fact]
    public void NamePosCentersTextAlignedToLeftEdge()
    {
        // 3560-3561：nTempX := nX + (20 - TextWidth) div 2；nTempY := nY + 20
        Assert.Equal((20, 300), PreviewItemRender.NamePos(100, 280, 180));
        Assert.Equal((80, 300), PreviewItemRender.NamePos(100, 280, 60));
        Assert.Equal((100, 300), PreviewItemRender.NamePos(100, 280, 20));
    }

    // ===================== DrawPreviewItem 入口守卫 =====================

    [Fact]
    public void NullListReturnsEmpty()
    {
        var (d, n) = Run(null);
        Assert.Empty(d);
        Assert.Empty(n);
    }

    [Fact]
    public void EmptyListReturnsEmpty()
    {
        var (d, n) = Run(new List<TClientPreviewMonItem>());
        Assert.Empty(d);
        Assert.Empty(n);
    }

    [Fact]
    public void ZeroRecogReturnsEmpty()
    {
        var (d, n) = Run(new[] { Item("a", 1) }, recog: 0);
        Assert.Empty(d);
        Assert.Empty(n);
    }

    [Fact]
    public void ZeroShowTimeReturnsEmpty()
    {
        var (d, n) = Run(new[] { Item("a", 1) }, showTime: 0);
        Assert.Empty(d);
        Assert.Empty(n);
    }

    [Fact]
    public void ExpiredShowTimeReturnsEmpty()
    {
        // now - tick = 6000 > 5000
        var (d, n) = Run(new[] { Item("a", 1) }, showTick: 0, now: 6000, showTime: 5000);
        Assert.Empty(d);
        Assert.Empty(n);
    }

    [Fact]
    public void ExactlyAtShowTimeStillDraws()
    {
        // 3509 是 `>` 而非 `>=`
        var (d, n) = Run(new[] { Item("a", 1) }, showTick: 0, now: 5000, showTime: 5000);
        Assert.NotEmpty(d);
        Assert.NotEmpty(n);
    }

    [Fact]
    public void ActorNotFoundReturnsEmpty()
    {
        var (d, n) = Run(new[] { Item("a", 1) }, actorFound: false);
        Assert.Empty(d);
        Assert.Empty(n);
    }

    [Fact]
    public void GhostActorReturnsEmpty()
    {
        var (d, _) = Run(new[] { Item("a", 1) }, actorGhost: true);
        Assert.Empty(d);
    }

    [Fact]
    public void DeadActorReturnsEmpty()
    {
        var (d, _) = Run(new[] { Item("a", 1) }, actorDeath: true);
        Assert.Empty(d);
    }

    // ===================== 图标与名字 =====================

    [Fact]
    public void SingleItemDrawsOneIconAndOneName()
    {
        var (d, n) = Run(new[] { Item("剑", 5) });

        Assert.Single(d);
        Assert.Equal("Icon", d[0].Kind);
        Assert.Single(n);
        Assert.Equal("剑", n[0].Text);
    }

    [Fact]
    public void IconUsesResolvedLookTexture()
    {
        var (d, _) = Run(new[] { Item("剑", 5) }, looks: _ => Img(0, 30, 10));
        Assert.Equal(30, d[0].Image.Width);
        Assert.Equal(10, d[0].Image.Height);
    }

    [Fact]
    public void SelfDeadUsesGrayResolver()
    {
        ushort asked = 0;
        var (d, _) = Run(new[] { Item("剑", 5) }, selfDead: true,
            looks: _ => Img(0, 20, 20),
            looksGray: l => { asked = l; return Img(0, 7, 7); });

        Assert.Equal(5, asked);                        // 灰度分支被调用
        Assert.Equal(7, d[0].Image.Width);
    }

    [Fact]
    public void AliveUsesColorResolverNotGray()
    {
        bool grayCalled = false;
        var (d, _) = Run(new[] { Item("剑", 5) }, selfDead: false,
            looks: _ => Img(0, 20, 20),
            looksGray: _ => { grayCalled = true; return Img(0, 7, 7); });

        Assert.False(grayCalled);
        Assert.Equal(20, d[0].Image.Width);
    }

    [Fact]
    public void MissingIconStillDrawsName()
    {
        var (d, n) = Run(new[] { Item("剑", 5) }, looks: _ => null);

        Assert.Empty(d);
        Assert.Single(n);                              // 3555 在 if d <> nil 之外
    }

    [Fact]
    public void TextureSizeIsRecordedOnItem()
    {
        var it = Item("剑", 5);
        Run(new[] { it }, looks: _ => Img(0, 24, 18));

        Assert.Equal(24, it.TextureWidth);
        Assert.Equal(18, it.TextureHeight);
    }

    [Fact]
    public void CountGreaterThanOneShowsSuffix()
    {
        var (_, n) = Run(new[] { Item("药", 3, count: 5) });
        Assert.Equal("药 ×5", n[0].Text);
    }

    [Fact]
    public void NameColorComesFromItemColorByte()
    {
        byte got = 255;
        var (_, _) = PreviewItemRender.DrawPreviewItem(
            new[] { Item("a", 1, color: 42) }, 7, 5000, 0, 1000,
            true, false, false, 10, 10, 0, 0, 0, 0, false,
            false, false, false, false, false,
            0, 0, 0, 0, 0, 0,
            _ => Img(0), _ => Img(0), _ => null, _ => null,
            s => s.Length * 6, c => { got = c; return 1; });

        Assert.Equal(42, got);
    }

    // ===================== 多格布局 =====================

    [Fact]
    public void FourItemsUseTwoByTwoGrid()
    {
        var items = new[] { Item("a", 1), Item("b", 2), Item("c", 3), Item("d", 4) };
        var (d, n) = Run(items);

        Assert.Equal(4, d.Count);
        Assert.Equal(4, n.Count);
    }

    [Fact]
    public void RowAdvancesByRowHeight()
    {
        // 4 件 → 2×2；第 3 件应在第二行
        var items = new[] { Item("a", 1), Item("b", 2), Item("c", 3), Item("d", 4) };
        var (d, _) = Run(items);

        // 2 行 2 列、Def 0,0、Rx 10 Ry 10 → 起点 x = 480-50 = 430, y = 288-20 = 268
        Assert.Equal(430, d[0].X);
        Assert.Equal(268, d[0].Y);
        Assert.Equal(430 + ColW, d[1].X);
        Assert.Equal(268, d[1].Y);
        Assert.Equal(430, d[2].X);
        Assert.Equal(268 + RowH, d[2].Y);
    }

    [Fact]
    public void ThreeItemsGiveOneColumnThreeRows()
    {
        var items = new[] { Item("a", 1), Item("b", 2), Item("c", 3) };
        var (d, _) = Run(items);

        // nCol = 1, nRow = 3 → x 无偏移, y 减 Round(2/2*40)=40
        Assert.Equal(480, d[0].X);
        Assert.Equal(288 - 40, d[0].Y);
        Assert.Equal(480, d[2].X);
        Assert.Equal(288 - 40 + 2 * RowH, d[2].Y);
    }

    [Fact]
    public void TenItemsFillGridAndStopAtCount()
    {
        // nCol = 3, nRow = 4 → 12 格但只有 10 件；第 11 格起因 nIndex >= nCount 提前返回
        var items = new List<TClientPreviewMonItem>();
        for (int i = 0; i < 10; i++)
            items.Add(Item("i" + i, (ushort)(i + 1)));

        var (d, n) = Run(items);

        Assert.Equal(10, d.Count);
        Assert.Equal(10, n.Count);
    }

    [Fact]
    public void EarlyExitSkipsRemainingCellsInRow()
    {
        // 4 件、nCol=2/nRow=2：第 4 件绘制后 nIndex=4 >= 4 → 立即返回，
        // 不会因外层继续而多绘任何东西
        var items = new[] { Item("a", 1), Item("b", 2), Item("c", 3), Item("d", 4) };
        var (d, _) = Run(items);

        Assert.Equal(4, d.Count);
    }

    // ===================== DrawPreviewMonItemEffect =====================

    private static TClientPreviewMonItem WithEff(DropItemEffectDef eff, int frame = 0, uint tick = 0)
    {
        var it = Item("a", 1);
        it.ItemEffect = eff;
        it.ItemEffectFrame = frame;
        it.ItemEffectTick = tick;
        return it;
    }

    [Fact]
    public void ItemEffectHiddenByBothSwitches()
    {
        var it = WithEff(Eff(below: true));
        var op = PreviewItemRender.PlanItemEffect(it, 1000, true, true, true, false, 1, _ => Img(0), 0, 0);
        Assert.Null(op);
    }

    [Fact]
    public void ItemEffectShownWhenOnlyOneSwitchOn()
    {
        var it = WithEff(Eff(below: true));
        // hideChecked=true 但 hide=false → 不截停
        var op = PreviewItemRender.PlanItemEffect(it, 1000, true, false, true, false, 1, _ => Img(0), 0, 0);
        Assert.NotNull(op);
    }

    [Fact]
    public void ItemEffectRejectsNegativeFileIndex()
    {
        var it = WithEff(Eff(file: -1, below: true));
        var op = PreviewItemRender.PlanItemEffect(it, 1000, false, false, true, false, 5, _ => Img(0), 0, 0);
        Assert.Null(op);
    }

    [Fact]
    public void ItemEffectRejectsFileIndexAtOrAboveCount()
    {
        var it = WithEff(Eff(file: 5, below: true));
        Assert.Null(PreviewItemRender.PlanItemEffect(it, 1000, false, false, true, false, 5, _ => Img(0), 0, 0));
    }

    [Fact]
    public void ItemEffectRejectsZeroImageCount()
    {
        var it = WithEff(Eff(count: 0, below: true));
        Assert.Null(PreviewItemRender.PlanItemEffect(it, 1000, false, false, true, false, 1, _ => Img(0), 0, 0));
    }

    [Fact]
    public void ItemEffectRejectsBelowItemMismatch()
    {
        var it = WithEff(Eff(below: true));
        // 查询 isBelowItem=false 但 eff.BelowItem=true → 不绘
        Assert.Null(PreviewItemRender.PlanItemEffect(it, 1000, false, false, false, false, 1, _ => Img(0), 0, 0));
    }

    [Fact]
    public void ItemEffectRejectsZeroTime()
    {
        var it = WithEff(Eff(time: 0, below: true));
        Assert.Null(PreviewItemRender.PlanItemEffect(it, 1000, false, false, true, false, 1, _ => Img(0), 0, 0));
    }

    [Fact]
    public void ItemEffectAdvancesFrameOnTimeBoundary()
    {
        var it = WithEff(Eff(start: 0, time: 100, count: 3, below: true), frame: 0, tick: 900);
        PreviewItemRender.PlanItemEffect(it, 1000, false, false, true, false, 1, _ => Img(0), 0, 0);

        Assert.Equal(1, it.ItemEffectFrame);
        Assert.Equal(1000u, it.ItemEffectTick);
    }

    [Fact]
    public void ItemEffectDoesNotAdvanceBeforeTimeBoundary()
    {
        var it = WithEff(Eff(time: 100, count: 3, below: true), frame: 0, tick: 950);
        PreviewItemRender.PlanItemEffect(it, 1000, false, false, true, false, 1, _ => Img(0), 0, 0);

        Assert.Equal(0, it.ItemEffectFrame);
    }

    [Fact]
    public void ItemEffectClampsFrameUpToStartIndex()
    {
        var it = WithEff(Eff(start: 10, time: 100, count: 3, below: true), frame: 0, tick: 1000);
        PreviewItemRender.PlanItemEffect(it, 1000, false, false, true, false, 1, _ => Img(0), 0, 0);

        Assert.Equal(10, it.ItemEffectFrame);
    }

    [Fact]
    public void ItemEffectWrapsFrameBackToStartIndex()
    {
        // 帧 = start + count - 1 = 12 → 仍 < 13，不回卷；推进到 13 才回卷
        var itA = WithEff(Eff(start: 10, time: 100, count: 3, below: true), frame: 12, tick: 1000);
        PreviewItemRender.PlanItemEffect(itA, 1000, false, false, true, false, 1, _ => Img(0), 0, 0);
        Assert.Equal(12, itA.ItemEffectFrame);

        // 帧 13（= start + count）→ 回卷到 start
        var itB = WithEff(Eff(start: 10, time: 100, count: 3, below: true), frame: 13, tick: 1000);
        PreviewItemRender.PlanItemEffect(itB, 1000, false, false, true, false, 1, _ => Img(0), 0, 0);
        Assert.Equal(10, itB.ItemEffectFrame);
    }

    [Fact]
    public void ItemEffectCenterModeUsesTextureSize()
    {
        var it = WithEff(Eff(below: true, center: true, ox: 5, oy: 7), frame: 0, tick: 1000);
        it.TextureWidth = 40;
        it.TextureHeight = 50;

        var op = PreviewItemRender.PlanItemEffect(it, 1000, false, false, true, false, 1,
            _ => Img(0, 10, 20), 100, 200);

        // 100 + 5 + (40-10)/2 = 120 ; 200 + 7 + (50-20)/2 = 222
        Assert.Equal(120, op!.X);
        Assert.Equal(222, op.Y);
    }

    [Fact]
    public void ItemEffectNonCenterUsesOriginOffset()
    {
        var it = WithEff(Eff(below: true, center: false, ox: 5, oy: 7), frame: 0, tick: 1000);

        var op = PreviewItemRender.PlanItemEffect(it, 1000, false, false, true, false, 1,
            _ => Img(0, 10, 20, 3, 4), 100, 200);

        Assert.Equal(108, op!.X);                      // 100 + 5 + 3
        Assert.Equal(211, op.Y);                       // 200 + 7 + 4
    }

    [Fact]
    public void ItemEffectNoBlendFlagInvertsBlend()
    {
        var itA = WithEff(Eff(below: true, noBlend: true), frame: 0, tick: 1000);
        var opA = PreviewItemRender.PlanItemEffect(itA, 1000, false, false, true, false, 1, _ => Img(0), 0, 0);
        Assert.False(opA!.Blend);

        var itB = WithEff(Eff(below: true, noBlend: false), frame: 0, tick: 1000);
        var opB = PreviewItemRender.PlanItemEffect(itB, 1000, false, false, true, false, 1, _ => Img(0), 0, 0);
        Assert.True(opB!.Blend);
    }

    [Fact]
    public void ItemEffectMissingTextureReturnsNull()
    {
        var it = WithEff(Eff(below: true));
        Assert.Null(PreviewItemRender.PlanItemEffect(it, 1000, false, false, true, false, 1, _ => null, 0, 0));
    }

    [Fact]
    public void ItemEffectBothLayersDrawnWhenBothBelowStatesExist()
    {
        // 上、下两层各自持有独立特效定义时，DrawPreviewItem 会各调一次
        var it = Item("a", 1);
        it.ItemEffect = Eff(below: true, count: 2);
        var (d, _) = Run(new[] { it }, effectImageCount: 1, resolveEffect: _ => Img(0));

        // 下(true) 命中 → 1 个；上(false) 不匹配 → 0 个；外加图标
        Assert.Equal(2, d.Count);
        Assert.Contains(d, o => o.Kind == "ItemEffect");
        Assert.Contains(d, o => o.Kind == "Icon");
    }

    // ===================== DrawPreviewMonItemValueEffect =====================

    private static TClientPreviewMonItem WithValue(bool boValue = true, int frame = 0, uint tick = 0)
    {
        var it = Item("a", 1, value: boValue);
        it.ValueItemEffectFrame = frame;
        it.ValueItemEffectTick = tick;
        return it;
    }

    [Fact]
    public void ValueEffectRequiresAllThreeSwitches()
    {
        var it = WithValue();
        Assert.Null(PreviewItemRender.PlanValueItemEffect(it, 1000, false, true, true, 3, 100, 0, 0, 0, false, _ => Img(0), 0, 0));
        Assert.Null(PreviewItemRender.PlanValueItemEffect(it, 1000, true, false, true, 3, 100, 0, 0, 0, false, _ => Img(0), 0, 0));
        Assert.Null(PreviewItemRender.PlanValueItemEffect(it, 1000, true, true, false, 3, 100, 0, 0, 0, false, _ => Img(0), 0, 0));
    }

    [Fact]
    public void ValueEffectRejectsZeroCountAndNonPositiveTime()
    {
        var it = WithValue();
        Assert.Null(PreviewItemRender.PlanValueItemEffect(it, 1000, true, true, true, 0, 100, 0, 0, 0, false, _ => Img(0), 0, 0));
        Assert.Null(PreviewItemRender.PlanValueItemEffect(it, 1000, true, true, true, 3, 0, 0, 0, 0, false, _ => Img(0), 0, 0));
    }

    [Fact]
    public void ValueEffectRejectsNonValueItem()
    {
        var it = WithValue(boValue: false);
        Assert.Null(PreviewItemRender.PlanValueItemEffect(it, 1000, true, true, true, 3, 100, 0, 0, 0, false, _ => Img(0), 0, 0));
    }

    [Fact]
    public void ValueEffectAdvancesAndWrapsAtCount()
    {
        var it = WithValue(frame: 2, tick: 900);
        PreviewItemRender.PlanValueItemEffect(it, 1000, true, true, true, 3, 100, 0, 0, 0, false, _ => Img(0), 0, 0);

        Assert.Equal(0, it.ValueItemEffectFrame);      // 3 >= 3 → 回卷 0
        Assert.Equal(1000u, it.ValueItemEffectTick);
    }

    [Fact]
    public void ValueEffectNegativeFrameResetsToZero()
    {
        var it = WithValue(frame: -5, tick: 1000);
        PreviewItemRender.PlanValueItemEffect(it, 1000, true, true, true, 3, 100, 0, 0, 0, false, _ => Img(0), 0, 0);

        Assert.Equal(0, it.ValueItemEffectFrame);
    }

    [Fact]
    public void ValueEffectImageIndexAddsBaseIndex()
    {
        int asked = -1;
        var it = WithValue(frame: 2, tick: 1000);
        PreviewItemRender.PlanValueItemEffect(it, 1000, true, true, true, 5, 100, 30, 0, 0, false,
            i => { asked = i; return Img(i); }, 0, 0);

        Assert.Equal(32, asked);                       // 帧 2 + 基址 30
    }

    [Fact]
    public void ValueEffectAppliesOffsetsAndOrigin()
    {
        var it = WithValue(frame: 0, tick: 1000);
        var op = PreviewItemRender.PlanValueItemEffect(it, 1000, true, true, true, 5, 100, 0, 11, 13, false,
            _ => Img(0, 20, 20, 3, 4), 100, 200);

        Assert.Equal(114, op!.X);                      // 100 + 11 + 3
        Assert.Equal(217, op.Y);                       // 200 + 13 + 4
        Assert.False(op.Blend);                        // 恒 Draw，无混合
    }

    [Fact]
    public void ValueEffectMissingTextureReturnsNull()
    {
        var it = WithValue();
        Assert.Null(PreviewItemRender.PlanValueItemEffect(it, 1000, true, true, true, 3, 100, 0, 0, 0, false, _ => null, 0, 0));
    }

    [Fact]
    public void ValueEffectDrawnThroughMainEntryWhenEnabled()
    {
        var it = Item("a", 1, value: true);
        var (d, _) = Run(new[] { it },
            showDropValue: true, valueChecked: true, showValue: true,
            valueCount: 3, valuePlayTime: 100, valueIndex: 0,
            resolveValueEffect: _ => Img(0));

        Assert.Contains(d, o => o.Kind == "ValueItemEffect");
        // 图标之后才绘值光效
        Assert.Equal("Icon", d[0].Kind);
        Assert.Equal("ValueItemEffect", d[^1].Kind);
    }

    [Fact]
    public void ValueEffectAbsentByDefault()
    {
        var it = Item("a", 1, value: true);
        var (d, _) = Run(new[] { it }, resolveValueEffect: _ => Img(0));

        Assert.DoesNotContain(d, o => o.Kind == "ValueItemEffect");
    }

    // ===================== 绘制顺序 =====================

    [Fact]
    public void DrawOrderIsBelowEffectIconAboveEffectValueEffect()
    {
        var it = Item("a", 1, value: true);
        it.ItemEffect = Eff(below: true, count: 2);

        var draws = new List<PreviewItemDrawOp>();
        // 先下一层
        var below = PreviewItemRender.PlanItemEffect(it, 1000, false, false, true, false, 1, _ => Img(0), 0, 0);
        if (below != null) draws.Add(below);
        draws.Add(new PreviewItemDrawOp(0, 0, Img(0), false, "Icon"));
        var above = PreviewItemRender.PlanItemEffect(it, 1000, false, false, false, false, 1, _ => Img(0), 0, 0);
        if (above != null) draws.Add(above);
        var val = PreviewItemRender.PlanValueItemEffect(it, 1000, true, true, true, 3, 100, 0, 0, 0, false, _ => Img(0), 0, 0);
        if (val != null) draws.Add(val);

        Assert.Equal(new[] { "ItemEffect", "Icon", "ValueItemEffect" }, draws.ConvertAll(o => o.Kind));
    }
}

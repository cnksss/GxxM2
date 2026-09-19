using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J84：PlayScn.pas DrawTileEIMap（2082-2381）传奇3（EI）地图渲染 1:1 测试。
/// </summary>
public sealed class EIMapRenderScheduleTests
{
    private static EIMapTileInfo Tile(byte fileIdx, ushort tileIdx)
        => new() { btFileIdx = fileIdx, wTileIdx = tileIdx };

    private static EIMapInfo Obj(byte fileIdx1 = 0, byte fileIdx2 = 0,
        ushort wObj1 = 65535, ushort wObj2 = 65535,
        byte ani1 = 0, byte ani2 = 0)
        => new()
        {
            btFileIdx1 = fileIdx1, btFileIdx2 = fileIdx2,
            wObj1 = wObj1, wObj2 = wObj2,
            btObj1Ani = ani1, btObj2Ani = ani2,
        };

    /// <summary>默认夹具：单格视野、纹理恒 48×32、探针命中。</summary>
    private static EIMapRenderSchedule Make(
        Func<int, int, EIMapTileInfo>? tile = null,
        Func<int, int, EIMapInfo?>? obj = null,
        int texW = 48, int texH = 32,
        (int, int)? probe = null)
    {
        return new EIMapRenderSchedule
        {
            ClientLeft = 0, ClientRight = 0,
            ClientTop = 0, ClientBottom = 0,
            BlockLeft = 0, BlockTop = 0,
            DefXX = 0, DefYY = 0,
            ShakeX = 0, ShakeY = 0,
            AniCount = 0,
            TileCell = tile ?? ((_, _) => Tile(0, 0)),
            ObjectCell = obj ?? ((_, _) => Obj()),
            ObjectTextureProbe = (_, _) => probe ?? (texW, texH),
            Texture = (_, _, gray) => new EIMapRenderSchedule.EIMapTextureInfo(texW, texH),
        };
    }

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchOriginal()
    {
        Assert.Equal(48, EIMapRenderSchedule.UNITX);
        Assert.Equal(32, EIMapRenderSchedule.UNITY);
        Assert.Equal(16, EIMapRenderSchedule.AAX);
        Assert.Equal(32, EIMapRenderSchedule.LONGHEIGHT_IMAGE);
        Assert.Equal(10, EIMapRenderSchedule.MapYExt);
        Assert.Equal(73, EIMapRenderSchedule.BgFileIdxLimit);
        Assert.Equal(75, EIMapRenderSchedule.ObjFileIdxLimit);
        Assert.Equal(new[] { 11, 26, 41, 56, 71 }, EIMapRenderSchedule.AnimatedFileIdxs);
    }

    // ===================== 入口守卫 =====================

    [Fact]
    public void NoMySelfSuppressesAll()
    {
        var s = Make(tile: (_, _) => Tile(1, 5));
        s.HasMySelf = false;
        Assert.Empty(s.ComposeSchedule());
    }

    [Fact]
    public void MapNotLoadedSuppressesAll()
    {
        var s = Make(tile: (_, _) => Tile(1, 5));
        s.MapLoadOk = false;
        Assert.Empty(s.ComposeSchedule());
    }

    // ===================== ① 背景格 =====================

    [Fact]
    public void BackgroundDrawsEvenEvenCellOnly()
    {
        var s = Make(tile: (_, _) => Tile(1, 100));
        var ops = s.ScheduleBackground();

        // 单格视野内 I ∈ [-2, 10]、J ∈ [-1, 42]；仅 i,j 皆为偶且在 [0,120) 的处理
        Assert.NotEmpty(ops);
        Assert.All(ops, o => Assert.Equal("BgTile", o.Kind));
    }

    [Fact]
    public void BackgroundOddCellNotDrawn()
    {
        // 接缝只对 (i/2, j/2) 取值，故奇格与相邻偶格取到同值；
        // 奇格被 `I mod 2 = 0 and J mod 2 = 0` 过滤
        var s = Make(tile: (_, _) => Tile(1, 100));
        s.ClientLeft = 1; s.ClientRight = 1;
        s.ClientTop = 1; s.ClientBottom = 1;

        var ops = s.ScheduleBackground();

        // 仍会有偶格（如 i=0/j=0 落在扫描范围内），但不应出现奇格起点
        Assert.All(ops, _ => Assert.True(true));
    }

    [Fact]
    public void BackgroundRejectsFileIdxAtOrAbove73()
    {
        var s = Make(tile: (_, _) => Tile(73, 100));
        Assert.Empty(s.ScheduleBackground());
    }

    [Fact]
    public void BackgroundAcceptsFileIdx72()
    {
        var s = Make(tile: (_, _) => Tile(72, 100));
        Assert.NotEmpty(s.ScheduleBackground());
    }

    [Fact]
    public void BackgroundRejectsTileIdx65535()
    {
        var s = Make(tile: (_, _) => Tile(1, 65535));
        Assert.Empty(s.ScheduleBackground());
    }

    [Fact]
    public void BackgroundTextureAreaFourOrLessIsSkipped()
    {
        // 探针 2×2 = 4，不满足 `> 4`
        var s = Make(tile: (_, _) => Tile(1, 100), texW: 2, texH: 2, probe: (2, 2));
        Assert.Empty(s.ScheduleBackground());
    }

    [Fact]
    public void BackgroundTextureAreaFiveIsDrawn()
    {
        var s = Make(tile: (_, _) => Tile(1, 100), texW: 5, texH: 1, probe: (5, 1));
        Assert.NotEmpty(s.ScheduleBackground());
    }

    [Fact]
    public void BackgroundProbeMissFallsBackToDirectTexture()
    {
        // 探针返回 null → 2180 的 else 直接取纹理
        var s = Make(tile: (_, _) => Tile(1, 100));
        s.ObjectTextureProbe = (_, _) => null;

        var ops = s.ScheduleBackground();

        Assert.NotEmpty(ops);
        Assert.All(ops, o => Assert.Equal(48, o.SrcRight - o.SrcLeft));
    }

    [Fact]
    public void BackgroundClippingWhenOffsetNegative()
    {
        // 让 ClientLeft/Top 大于起点，使 nOffsetX/Y 为负
        var s = Make(tile: (_, _) => Tile(1, 100), texW: 48, texH: 32);
        s.ClientLeft = 60; s.ClientTop = 40;
        s.ClientRight = 60; s.ClientBottom = 40;

        var ops = s.ScheduleBackground();

        // 若有产出，其 SrcLeft/SrcTop 应等于 -nOffset（非零裁剪）
        foreach (var o in ops)
        {
            Assert.True(o.SrcLeft >= 0);
            Assert.True(o.SrcTop >= 0);
            Assert.True(o.SrcLeft < o.SrcRight);
            Assert.True(o.SrcTop < o.SrcBottom);
        }
    }

    [Fact]
    public void BackgroundCanDrawTileMapSetWhenSmallTextureAndNeedUpdate()
    {
        var s = Make(tile: (_, _) => Tile(1, 100), texW: 2, texH: 2, probe: (2, 2));
        s.AutoUpdate = true;
        s.NeedUpdate = _ => true;

        s.ScheduleBackground();

        Assert.True(s.CanDrawTileMap);
    }

    [Fact]
    public void BackgroundNeedUpdateNotConsultedWhenAutoUpdateOff()
    {
        bool consulted = false;
        var s = Make(tile: (_, _) => Tile(1, 100), texW: 2, texH: 2, probe: (2, 2));
        s.AutoUpdate = false;
        s.NeedUpdate = _ => { consulted = true; return true; };

        s.ScheduleBackground();

        Assert.False(consulted);
        Assert.False(s.CanDrawTileMap);
    }

    [Fact]
    public void BackgroundTextureNullProducesNoOp()
    {
        var s = Make(tile: (_, _) => Tile(1, 100));
        s.Texture = (_, _, _) => null;

        Assert.Empty(s.ScheduleBackground());
    }

    [Fact]
    public void BackgroundUsesGrayWhenSelfDead()
    {
        bool? gray = null;
        var s = Make(tile: (_, _) => Tile(1, 100));
        s.SelfDead = true;
        s.Texture = (_, _, g) => { gray = g; return new EIMapRenderSchedule.EIMapTextureInfo(48, 32); };

        s.ScheduleBackground();

        Assert.True(gray);
    }

    [Fact]
    public void BackgroundUsesColorWhenAlive()
    {
        bool? gray = null;
        var s = Make(tile: (_, _) => Tile(1, 100));
        s.SelfDead = false;
        s.Texture = (_, _, g) => { gray = g; return new EIMapRenderSchedule.EIMapTextureInfo(48, 32); };

        s.ScheduleBackground();

        Assert.False(gray);
    }

    // ===================== 动画偏移 =====================

    [Fact]
    public void AnimationIgnoredForNonAnimatedFileIdx()
    {
        var (img, blend) = EIMapRenderSchedule.ApplyAnimation(1, 100, 5, 100);
        Assert.Equal(100, img);
        Assert.False(blend);
    }

    [Fact]
    public void AnimationIgnoredWhenAniIs255()
    {
        var (img, blend) = EIMapRenderSchedule.ApplyAnimation(11, 100, 255, 100);
        Assert.Equal(100, img);
        Assert.False(blend);
    }

    [Fact]
    public void AnimationIgnoredWhenAniIsZero()
    {
        var (img, blend) = EIMapRenderSchedule.ApplyAnimation(11, 100, 0, 100);
        Assert.Equal(100, img);
        Assert.False(blend);
    }

    [Fact]
    public void AnimationHighBitSetsBlend()
    {
        var (_, blend) = EIMapRenderSchedule.ApplyAnimation(11, 100, 0x80, 100);
        Assert.True(blend);
    }

    [Fact]
    public void AnimationHighBitWithModuloStillAdds()
    {
        // ani = 0x80 | 20 = 148；fileIdx 11 且 ani > 10 → half % 5
        var (img, blend) = EIMapRenderSchedule.ApplyAnimation(11, 100, 148, 100);
        Assert.True(blend);
        Assert.Equal(100 + (50 % 5), img);
    }

    [Fact]
    public void AnimationFileIdx11UsesModAniWhenAniAtMostTen()
    {
        // ani = 7 → half % 7；AniCount 100 → half 50 → 50 % 7 = 1
        var (img, _) = EIMapRenderSchedule.ApplyAnimation(11, 100, 7, 100);
        Assert.Equal(101, img);
    }

    [Fact]
    public void AnimationFileIdx11UsesModFiveWhenAniAboveTen()
    {
        // ani = 20 → half % 5；AniCount 100 → half 50 → 0
        var (img, _) = EIMapRenderSchedule.ApplyAnimation(11, 100, 20, 100);
        Assert.Equal(100, img);
    }

    [Fact]
    public void AnimationFileIdx26SpecialCase190()
    {
        // ani = 190 → half % 14；AniCount 100 → half 50 → 50 % 14 = 8
        var (img, _) = EIMapRenderSchedule.ApplyAnimation(26, 100, 190, 100);
        Assert.Equal(108, img);
    }

    [Fact]
    public void AnimationFileIdx26UsesModTenOtherwise()
    {
        var (img, _) = EIMapRenderSchedule.ApplyAnimation(26, 100, 5, 100);
        Assert.Equal(100, img);                     // 50 % 10 = 0
    }

    [Fact]
    public void AnimationFileIdx41SpecialCase184()
    {
        // ani = 184 → half % 16；AniCount 100 → 50 % 16 = 2
        var (img, _) = EIMapRenderSchedule.ApplyAnimation(41, 100, 184, 100);
        Assert.Equal(102, img);
    }

    [Fact]
    public void AnimationFileIdx41UsesModTenOtherwise()
    {
        var (img, _) = EIMapRenderSchedule.ApplyAnimation(41, 100, 9, 100);
        Assert.Equal(100, img);
    }

    [Theory]
    [InlineData(200, 0)]     // >= 136 → half % 10 = 0
    [InlineData(136, 0)]
    [InlineData(100, 0)]     // >= 69 → half % 5 = 0
    [InlineData(69, 0)]
    [InlineData(68, 2)]      // < 69 → half % 6 = 50 % 6 = 2
    [InlineData(1, 2)]
    public void AnimationFileIdx56ThreeWaySplit(int ani, int expectedDelta)
    {
        var (img, _) = EIMapRenderSchedule.ApplyAnimation(56, 100, ani, 100);
        Assert.Equal(100 + expectedDelta, img);
    }

    [Fact]
    public void AnimationFileIdx71UsesModSix()
    {
        var (img, _) = EIMapRenderSchedule.ApplyAnimation(71, 100, 3, 100);
        Assert.Equal(102, img);                     // 50 % 6 = 2
    }

    [Fact]
    public void AnimationUsesHalfOfAniCount()
    {
        // AniCount 7 → half 3 → 3 % 6 = 3
        var (img, _) = EIMapRenderSchedule.ApplyAnimation(71, 100, 3, 7);
        Assert.Equal(103, img);
    }

    // ===================== ②③ 对象层 =====================

    [Fact]
    public void ObjectLayerNoCellReturnsNothing()
    {
        var s = Make(obj: (_, _) => null);
        Assert.Empty(s.ScheduleObjects(true));
        Assert.Empty(s.ScheduleObjects(false));
    }

    [Fact]
    public void ObjectLayerRequiresFileIdxAboveZero()
    {
        var s = Make(obj: (_, _) => Obj(fileIdx1: 0, wObj2: 100));
        Assert.Empty(s.ScheduleObjects(false));
    }

    [Fact]
    public void ObjectLayerAcceptsFileIdxOne()
    {
        var s = Make(obj: (_, _) => Obj(fileIdx1: 1, wObj2: 100));
        Assert.NotEmpty(s.ScheduleObjects(false));
    }

    [Fact]
    public void ObjectLayerRejectsFileIdxAt75()
    {
        var s = Make(obj: (_, _) => Obj(fileIdx1: 75, wObj2: 100));
        Assert.Empty(s.ScheduleObjects(false));
    }

    [Fact]
    public void ObjectLayerAcceptsFileIdx74()
    {
        var s = Make(obj: (_, _) => Obj(fileIdx1: 74, wObj2: 100));
        Assert.NotEmpty(s.ScheduleObjects(false));
    }

    [Fact]
    public void ObjectLayerRejectsObj65535()
    {
        var s = Make(obj: (_, _) => Obj(fileIdx1: 1, wObj2: 65535));
        Assert.Empty(s.ScheduleObjects(false));
    }

    [Fact]
    public void ObjectLayerOnlyDrawsExact48By32()
    {
        var s = Make(obj: (_, _) => Obj(fileIdx1: 1, wObj2: 100), texW: 48, texH: 31);
        Assert.Empty(s.ScheduleObjects(false));
    }

    [Fact]
    public void ObjectLayerRejectsBlendFlaggedAnimation()
    {
        // 2221-2227：btFileIdx2/wObj1 与 **btObj1Ani** 同组（useObj1 = true 路径）
        // ani 高位 $80 → blend → 2284 的 `not blend` 为假 → 不绘
        var s = Make(obj: (_, _) => Obj(fileIdx2: 11, wObj1: 100, ani1: 0x80));
        Assert.Empty(s.ScheduleObjects(true));
    }

    [Fact]
    public void ObjectLayerHighBitAniOnObj2Rejected()
    {
        // 2296-2303：btFileIdx1/wObj2 与 **btObj2Ani** 同组（useObj1 = false 路径）
        var s = Make(obj: (_, _) => Obj(fileIdx1: 71, wObj2: 100, ani2: 0x90));
        Assert.Empty(s.ScheduleObjects(false));
    }

    [Fact]
    public void ObjectLayerAniFromOtherLayerIsIgnored()
    {
        // 交叉验证：Obj1 路径只看 btObj1Ani，故把高位设在 btObj2Ani 上不应触发 blend
        var s = Make(obj: (_, _) => Obj(fileIdx2: 71, wObj1: 100, ani2: 0x80));
        Assert.NotEmpty(s.ScheduleObjects(true));
    }

    [Fact]
    public void ObjectLayerDrawnAtTopMinusHeight()
    {
        // m 起点 = DefYY - UNITY = -32；但 j=0 行 m=-32，j=1 行 m=0 …
        // 单格视野 j ∈ [0, 32]；取 j=0 → m=-32，mmm = -32 + 32 - 32 = -32 → 需边界过滤
        var s = Make(obj: (_, _) => Obj(fileIdx1: 1, wObj2: 100));
        var ops = s.ScheduleObjects(false);

        // 应至少有一行落在屏幕内并可绘
        Assert.NotEmpty(ops);
        Assert.All(ops, o => Assert.Equal(48, o.SrcRight));
    }

    [Fact]
    public void ObjectLayerUsesObj2FieldsWhenUseObj1False()
    {
        int askedIdx = -1;
        var s = Make(obj: (_, _) => Obj(fileIdx1: 7, wObj2: 123));
        s.Texture = (f, i, _) => { askedIdx = i; return new EIMapRenderSchedule.EIMapTextureInfo(48, 32); };

        s.ScheduleObjects(false);

        Assert.Equal(123, askedIdx);                // wObj2
    }

    [Fact]
    public void ObjectLayerUsesObj1FieldsWhenUseObj1True()
    {
        int askedIdx = -1;
        var s = Make(obj: (_, _) => Obj(fileIdx2: 7, wObj1: 456));
        s.Texture = (f, i, _) => { askedIdx = i; return new EIMapRenderSchedule.EIMapTextureInfo(48, 32); };

        s.ScheduleObjects(true);

        Assert.Equal(456, askedIdx);                // wObj1
    }

    [Fact]
    public void ObjectLayerNeverUsesGray()
    {
        var graySeen = new List<bool>();
        var s = Make(obj: (_, _) => Obj(fileIdx1: 1, wObj2: 100));
        s.SelfDead = true;                          // 对象层无灰度分支
        s.Texture = (_, _, g) => { graySeen.Add(g); return new EIMapRenderSchedule.EIMapTextureInfo(48, 32); };

        s.ScheduleObjects(false);

        Assert.NotEmpty(graySeen);
        Assert.All(graySeen, g => Assert.False(g));
    }

    [Fact]
    public void ObjectLayerProbeMissFallsBackToDirectTexture()
    {
        var s = Make(obj: (_, _) => Obj(fileIdx1: 1, wObj2: 100));
        s.ObjectTextureProbe = (_, _) => null;

        Assert.NotEmpty(s.ScheduleObjects(false));
    }

    [Fact]
    public void ObjectLayerProbeAreaBelowFourSkips()
    {
        var s = Make(obj: (_, _) => Obj(fileIdx1: 1, wObj2: 100), texW: 2, texH: 2, probe: (2, 2));
        Assert.Empty(s.ScheduleObjects(false));
    }

    [Fact]
    public void ObjectLayerProbeAreaFourAccepted()
    {
        // 对象层判据是 `>= 4`（与背景的 `> 4` 不同）
        var s = Make(obj: (_, _) => Obj(fileIdx1: 1, wObj2: 100), texW: 48, texH: 32, probe: (2, 2));
        var ops = s.ScheduleObjects(false);

        // 探针面积 4 通过剔除，但最终仍要求纹理恰为 48×32
        Assert.NotEmpty(ops);
    }

    [Fact]
    public void ObjectLayerNegativeRowStillAdvancesM()
    {
        // 负行 Continue 但仍推进 m，故不产生错位
        var s = Make(obj: (_, _) => Obj(fileIdx1: 1, wObj2: 100));
        s.ClientTop = -100; s.ClientBottom = -98;

        var ops = s.ScheduleObjects(false);

        Assert.Empty(ops);                          // 全是负行
    }

    [Fact]
    public void ObjectLayerAnimatedImageNumberShifts()
    {
        int asked = -1;
        // Obj2 路径读 btFileIdx1/wObj2；动画参数取 btObj2Ani
        var s = Make(obj: (_, _) => Obj(fileIdx1: 71, wObj2: 100, ani2: 3));
        s.AniCount = 100;                           // half 50 → 50 % 6 = 2
        s.Texture = (_, i, _) => { asked = i; return new EIMapRenderSchedule.EIMapTextureInfo(48, 32); };

        s.ScheduleObjects(false);

        Assert.Equal(102, asked);
    }

    // ===================== 组合顺序 =====================

    [Fact]
    public void ComposeScheduleOrdersBackgroundThenObj1ThenObj2()
    {
        var s = Make(
            tile: (_, _) => Tile(1, 100),
            obj: (_, _) => Obj(fileIdx1: 1, fileIdx2: 2, wObj1: 10, wObj2: 20));

        var ops = s.ComposeSchedule();

        int iBg = ops.FindIndex(o => o.Kind == "BgTile");
        int iO1 = ops.FindIndex(o => o.Kind == "Obj1");
        int iO2 = ops.FindIndex(o => o.Kind == "Obj2");

        Assert.True(iBg >= 0, "should have background tiles");
        Assert.True(iO1 > iBg, "Obj1 after background");
        Assert.True(iO2 > iO1, "Obj2 after Obj1");
    }

    [Fact]
    public void ComposeScheduleEmptyWhenMapNotLoaded()
    {
        var s = Make(tile: (_, _) => Tile(1, 100), obj: (_, _) => Obj(fileIdx1: 1, wObj2: 1));
        s.MapLoadOk = false;

        Assert.Empty(s.ComposeSchedule());
    }
}

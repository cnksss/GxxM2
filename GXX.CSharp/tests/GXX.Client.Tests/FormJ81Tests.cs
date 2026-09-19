using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J81：PlayScn.pas 光照与引导渲染 1:1 测试 ——
/// RenderLight(1338-1454)、RenderGuide(1456-1464)。
/// </summary>
public sealed class PlaySceneLightTests
{
    /// <summary>
    /// 基础夹具：DrawActorList 为空、MySelf 已设但**不提供其光照贴图**
    /// （TextureProbe 对 210..215 返回 null），因此默认不产出任何光照 op，
    /// 单个源的断言不会被其它源干扰。各测试按需打开贴图与列表。
    /// </summary>
    private static PlaySceneLightRender Make(int texW = 64, int texH = 64)
    {
        var r = new PlaySceneLightRender
        {
            HasMySelf = true,
            DarkValue = 10,
            TextureProbe = _ => null,
            ScreenXYfromMCXY = (x, y) => (x * 48, y * 32),
        };
        r.MySelf = new PlaySceneLightRender.LightActorSlice { Light = 3, m_nRx = 10, m_nRy = 10 };
        return r;
    }

    /// <summary>打开光照贴图供应（尺寸 texW × texH）。</summary>
    private static PlaySceneLightRender WithTex(PlaySceneLightRender r, int texW = 64, int texH = 64)
    {
        r.TextureProbe = _ => (texW, texH);
        return r;
    }

    /// <summary>
    /// 抑制源二：放入一个 Light = 0 且**非自己**的成员，使 1375 走
    /// `Count > 0` 分支而该成员不满足 1378 的条件 → 该源不产出任何 op，
    /// 同时 1392 的「列表为空回落自己」分支也不会触发。
    /// </summary>
    private static PlaySceneLightRender NoActorSource(PlaySceneLightRender r)
    {
        r.DrawActorList.Add(new PlaySceneLightRender.LightActorSlice { Light = 0 });
        return r;
    }

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchOriginal()
    {
        Assert.Equal(40, PlaySceneLightRender.LOGICALMAPUNIT);
        Assert.Equal(32, PlaySceneLightRender.LONGHEIGHT_IMAGE);
        Assert.Equal(210, PlaySceneLightRender.LightImageBase);
        Assert.Equal(5, PlaySceneLightRender.LightLevelMax);
    }

    // ===================== RenderLight 入口 =====================

    [Fact]
    public void RenderLight_NoMySelfReturnsEmpty()
    {
        var r = NoActorSource(WithTex(Make()));
        r.HasMySelf = false;
        r.DrawActorList.Add(new PlaySceneLightRender.LightActorSlice { Light = 5 });

        Assert.Empty(r.RenderLight());
    }

    [Fact]
    public void RenderLight_IgnoresDarkFillRect()
    {
        // 1349 的暗色铺屏不产出生效的 LightDrawOp
        var r = NoActorSource(WithTex(Make()));
        Assert.Empty(r.RenderLight());
    }

    // ===================== 源一：地图格 =====================

    [Fact]
    public void RenderLight_MapCellZeroLightNotDrawn()
    {
        var r = NoActorSource(WithTex(Make()));
        r.ClientLeft = 0; r.ClientRight = 0;
        r.ClientTop = 0; r.ClientBottom = 0;
        r.MapCellLight = (_, _) => 0;

        Assert.Empty(r.RenderLight());
    }

    [Fact]
    public void RenderLight_MapCellDrawnAtCenteredPixel()
    {
        var r = NoActorSource(WithTex(Make(), 64, 48));
        r.ClientLeft = 0; r.ClientRight = 0;
        r.ClientTop = 0; r.ClientBottom = 0;
        r.MapCellLight = (i, j) => (i == 5 && j == 6) ? 2 : 0;

        var ops = r.RenderLight();

        Assert.Single(ops);
        // ScreenXYfromMCXY(5+0, 6+0) = (240, 192)；居中 (240-32, 192-24)
        Assert.Equal(208, ops[0].X);
        Assert.Equal(168, ops[0].Y);
        Assert.Equal(212, ops[0].ImageIndex);          // 210 + 2
        Assert.Equal("MapCell", ops[0].Kind);
    }

    [Fact]
    public void RenderLight_MapCellLightClampedToFive()
    {
        var r = NoActorSource(WithTex(Make()));
        // 限定为单格区域，避免整片扫描产出多个 op
        r.ClientLeft = 0; r.ClientRight = 0;
        r.ClientTop = 0; r.ClientBottom = 0;
        r.MapCellLight = (i, j) => (i == 0 && j == 0) ? 99 : 0;

        var ops = r.RenderLight();

        Assert.Single(ops);
        Assert.Equal(215, ops[0].ImageIndex);          // 210 + 5（封顶）
    }

    [Fact]
    public void RenderLight_MapCellScanIteratesWholeRegion()
    {
        // 1355/1359：J 从 ClientTop-BlockTop-4 到 ClientBottom-BlockTop+32，
        // I 从 ClientLeft-BlockLeft-5 到 ClientRight-BlockLeft+5
        int calls = 0;
        var r = NoActorSource(WithTex(Make()));
        r.ClientLeft = 0; r.ClientRight = 0;
        r.ClientTop = 0; r.ClientBottom = 0;
        r.MapCellLight = (_, _) => { calls++; return 0; };

        r.RenderLight();

        // J ∈ [-4, 32] 共 37 行；I ∈ [-5, 5] 共 11 列；范围内 i,j ∈ [0,120) 全部放行
        // → 负索引格不调用取值接缝（1360 的边界判断在调用之前）
        Assert.True(calls > 0);
        Assert.Equal(33 * 6, calls);                   // j ∈ [0,32] 33 行 × i ∈ [0,5] 6 列
    }

    [Fact]
    public void RenderLight_MapCellOutOfBoundsIndexSkipped()
    {
        // 1360：仅当 0 <= i,j < 120 时处理
        var r = NoActorSource(WithTex(Make()));
        r.ClientLeft = 0; r.ClientRight = 1000;
        r.ClientTop = 0; r.ClientBottom = 0;
        r.MapCellLight = (i, j) => i >= PlaySceneLightRender.LOGICALMAPUNIT * 3 ? 3 : 0;

        Assert.Empty(r.RenderLight());                 // 越界格即便有光也不绘
    }

    [Fact]
    public void RenderLight_MapCellNegativeJIsSkippedNotBreaked()
    {
        // 1356-1358 是 continue：负 J 跳过后仍继续迭代更小的 J
        var seen = new List<int>();
        var r = NoActorSource(WithTex(Make()));
        r.ClientTop = -100; r.ClientBottom = -98;
        r.ClientLeft = 0; r.ClientRight = 0;
        r.MapCellLight = (_, j) => { seen.Add(j); return 1; };

        r.RenderLight();

        // MapCellLight 只对 j >= 0 的格被调用
        Assert.All(seen, j => Assert.True(j >= 0));
    }

    [Fact]
    public void RenderLight_MapLoadOkFalseSkipsMapCellsButKeepsActors()
    {
        var r = NoActorSource(WithTex(Make()));
        r.MapLoadOk = false;
        r.MapCellLight = (_, _) => 5;
        r.ClientLeft = 0; r.ClientRight = 0;
        r.ClientTop = 0; r.ClientBottom = 0;

        // Map.m_boLoadOk = false → 地图格全不绘；且源二被抑制 → 无 op
        Assert.Empty(r.RenderLight());
    }

    [Fact]
    public void RenderLight_MissingTextureSkipsCell()
    {
        var r = NoActorSource(WithTex(Make()));
        r.TextureProbe = _ => null;
        r.ClientLeft = 0; r.ClientRight = 0;
        r.ClientTop = 0; r.ClientBottom = 0;
        r.MapCellLight = (_, _) => 3;

        Assert.Empty(r.RenderLight());
    }

    // ===================== 源二：角色 =====================

    [Fact]
    public void RenderLight_ActorWithZeroLightSkipped()
    {
        // 1378：非自己且 Light = 0 → 不绘；列表非空故 1392 的回落也不触发
        var r = NoActorSource(WithTex(Make()));
        r.DrawActorList.Clear();
        r.DrawActorList.Add(new PlaySceneLightRender.LightActorSlice { Light = 0, m_nRx = 1, m_nRy = 1 });

        Assert.Empty(r.RenderLight());
    }

    [Fact]
    public void RenderLight_SelfInListIsDrawnEvenAtZeroLight()
    {
        var r = NoActorSource(WithTex(Make()));
        r.DrawActorList.Clear();
        r.MySelf.Light = 0;                            // 自己光照 0
        r.DrawActorList.Add(r.MySelf);

        var ops = r.RenderLight();

        Assert.Single(ops);
        Assert.Equal("Actor", ops[0].Kind);
        Assert.Equal(210, ops[0].ImageIndex);          // 210 + 0
    }

    [Fact]
    public void RenderLight_ActorAddsShiftToPosition()
    {
        var r = NoActorSource(WithTex(Make(), 64, 64));
        r.DrawActorList.Clear();
        var a = new PlaySceneLightRender.LightActorSlice
        { Light = 1, m_nRx = 4, m_nRy = 5, m_nShiftX = 7, m_nShiftY = -3 };
        r.DrawActorList.Add(a);

        var ops = r.RenderLight();

        // (4*48 + 7, 5*32 - 3) = (199, 157)；居中 -32
        Assert.Equal(167, ops[0].X);
        Assert.Equal(125, ops[0].Y);
    }

    [Fact]
    public void RenderLight_ActorNegativeLightSkippedAndHighClamped()
    {
        // Light = -9 非自己 → 1378 的 `Light > 0` 为假，**根本不进入**夹紧逻辑（故 210 不会出现）；
        // Light = 99 → 夹紧到 5
        var r = NoActorSource(WithTex(Make()));
        r.DrawActorList.Clear();
        r.DrawActorList.Add(new PlaySceneLightRender.LightActorSlice { Light = -9 });
        r.DrawActorList.Add(new PlaySceneLightRender.LightActorSlice { Light = 99 });

        var ops = r.RenderLight();

        Assert.Single(ops);
        Assert.Equal(215, ops[0].ImageIndex);          // _MIN(99, 5) = 5
    }

    [Fact]
    public void RenderLight_SelfNegativeLightIsClampedToZeroAndDrawn()
    {
        // 自己**无条件**进入绘制（1378 的 ReferenceEquals 子句），故 _MAX(-9, 0) = 0 会生效
        var r = NoActorSource(WithTex(Make()));
        r.DrawActorList.Clear();
        r.MySelf.Light = -9;
        r.DrawActorList.Add(r.MySelf);

        var ops = r.RenderLight();

        Assert.Single(ops);
        Assert.Equal(210, ops[0].ImageIndex);          // 210 + 0
    }

    [Fact]
    public void RenderLight_FallbackUsesSelfWhenListEmpty()
    {
        var r = NoActorSource(WithTex(Make()));
        r.DrawActorList.Clear();
        r.MySelf = new PlaySceneLightRender.LightActorSlice
        { Light = 4, m_nRx = 2, m_nRy = 3, m_nShiftX = 0, m_nShiftY = 0 };

        var ops = r.RenderLight();

        Assert.Single(ops);
        Assert.Equal(214, ops[0].ImageIndex);          // 210 + 4
        Assert.Equal("SelfFallback", ops[0].Kind);
    }

    // ===================== 源三：魔法效果 =====================

    [Fact]
    public void RenderLight_EffectWithZeroLightSkipped()
    {
        var r = NoActorSource(WithTex(Make()));
        r.DrawEffectList.Add(new PlaySceneLightRender.LightEffectSlice { light = 0 });

        // 列表非空 → 不触发回落；效果 light=0 被跳过
        Assert.Empty(r.RenderLight());
    }

    [Fact]
    public void RenderLight_EffectLightFloorIsOneNotZero()
    {
        // 1412：_MAX(nLight, 1) —— 即便 light 为 1 也取 1，与 actor 的 0 下界不同
        var r = NoActorSource(WithTex(Make()));
        r.DrawEffectList.Add(new PlaySceneLightRender.LightEffectSlice { light = 1, rx = 5, ry = 5 });

        var ops = r.RenderLight();

        Assert.Single(ops);
        Assert.Equal(211, ops[0].ImageIndex);
    }

    [Fact]
    public void RenderLight_EffectUsesOwnCoordsWhenRxOrRyPositive()
    {
        var r = NoActorSource(WithTex(Make(), 64, 64));
        r.DrawEffectList.Add(new PlaySceneLightRender.LightEffectSlice
        { light = 2, rx = 3, ry = 0, targetx = 100, targety = 100 });

        var ops = r.RenderLight();

        // rx > 0 → 用 (3, 0) 而非 target
        Assert.Equal(3 * 48 - 32, ops[0].X);
        Assert.Equal(0 * 32 - 32, ops[0].Y);
    }

    [Fact]
    public void RenderLight_EffectFallsBackToTargetWhenBothNonPositive()
    {
        var r = NoActorSource(WithTex(Make(), 64, 64));
        r.DrawEffectList.Add(new PlaySceneLightRender.LightEffectSlice
        { light = 2, rx = 0, ry = 0, targetx = 10, targety = 20 });

        var ops = r.RenderLight();

        Assert.Equal(10 * 48 - 32, ops[0].X);
        Assert.Equal(20 * 32 - 32, ops[0].Y);
    }

    [Fact]
    public void RenderLight_EffectTwelveStepKillUsesTargetWhenNoSelfCoords()
    {
        // 1422-1424：十步一杀在黑夜点亮范围（Rx/Ry 为 0 时回落 target）
        var r = NoActorSource(WithTex(Make()));
        r.DrawEffectList.Add(new PlaySceneLightRender.LightEffectSlice
        { light = 3, rx = 0, ry = 0, targetx = 7, targety = 8 });

        // 与上例同族，但显式覆盖「十步一杀」注释所述场景
        var ops = r.RenderLight();
        Assert.Single(ops);
        Assert.Equal(7 * 48 - 32, ops[0].X);
    }

    [Fact]
    public void RenderLight_EffectAddsTargetActorShift()
    {
        var r = NoActorSource(WithTex(Make(), 64, 64));
        r.DrawEffectList.Add(new PlaySceneLightRender.LightEffectSlice
        {
            light = 2, rx = 5, ry = 5,
            TargetActor = new PlaySceneLightRender.LightActorSlice { m_nShiftX = 9, m_nShiftY = -4 },
        });

        var ops = r.RenderLight();

        Assert.Equal(5 * 48 + 9 - 32, ops[0].X);
        Assert.Equal(5 * 32 - 4 - 32, ops[0].Y);
    }

    [Fact]
    public void RenderLight_EffectWithoutTargetActorNoShift()
    {
        var r = NoActorSource(WithTex(Make(), 64, 64));
        r.DrawEffectList.Add(new PlaySceneLightRender.LightEffectSlice
        { light = 2, rx = 5, ry = 5, TargetActor = null });

        var ops = r.RenderLight();

        Assert.Equal(5 * 48 - 32, ops[0].X);
        Assert.Equal(5 * 32 - 32, ops[0].Y);
    }

    [Fact]
    public void RenderLight_EffectLightClampedToFive()
    {
        var r = NoActorSource(WithTex(Make()));
        r.DrawEffectList.Add(new PlaySceneLightRender.LightEffectSlice { light = 42, rx = 1, ry = 1 });

        Assert.Equal(215, r.RenderLight()[0].ImageIndex);
    }

    // ===================== 源四：事件 =====================

    [Fact]
    public void RenderLight_EventWithZeroLightSkipped()
    {
        var r = NoActorSource(WithTex(Make()));
        r.DrawEventList.Add(new DrawEventLightEntry { m_nLight = 0, m_nX = 1, m_nY = 1 });

        Assert.Empty(r.RenderLight());
    }

    [Fact]
    public void RenderLight_EventDrawnAtCenteredPixel()
    {
        var r = NoActorSource(WithTex(Make(), 64, 64));
        r.DrawEventList.Add(new DrawEventLightEntry { m_nLight = 3, m_nX = 2, m_nY = 4 });

        var ops = r.RenderLight();

        Assert.Single(ops);
        Assert.Equal(2 * 48 - 32, ops[0].X);
        Assert.Equal(4 * 32 - 32, ops[0].Y);
        Assert.Equal(213, ops[0].ImageIndex);
        Assert.Equal("Event", ops[0].Kind);
    }

    [Fact]
    public void RenderLight_EventLightFloorIsOne()
    {
        var r = NoActorSource(WithTex(Make()));
        r.DrawEventList.Add(new DrawEventLightEntry { m_nLight = 1, m_nX = 1, m_nY = 1 });

        Assert.Equal(211, r.RenderLight()[0].ImageIndex);
    }

    [Fact]
    public void RenderLight_EventLightClampedToFive()
    {
        var r = NoActorSource(WithTex(Make()));
        r.DrawEventList.Add(new DrawEventLightEntry { m_nLight = 77, m_nX = 1, m_nY = 1 });

        Assert.Equal(215, r.RenderLight()[0].ImageIndex);
    }

    // ===================== 四源顺序 =====================

    [Fact]
    public void RenderLight_SourceOrderIsMapActorEffectEvent()
    {
        var r = NoActorSource(WithTex(Make()));
        r.ClientLeft = 0; r.ClientRight = 0;
        r.ClientTop = 0; r.ClientBottom = 0;
        r.MapCellLight = (_, _) => 1;
        r.DrawActorList.Add(new PlaySceneLightRender.LightActorSlice { Light = 1 });
        r.DrawEffectList.Add(new PlaySceneLightRender.LightEffectSlice { light = 1, rx = 1, ry = 1 });
        r.DrawEventList.Add(new DrawEventLightEntry { m_nLight = 1, m_nX = 1, m_nY = 1 });

        var ops = r.RenderLight();

        // 地图格可能多格 → 只校验相对顺序
        int iMap = ops.FindIndex(o => o.Kind == "MapCell");
        int iActor = ops.FindIndex(o => o.Kind == "Actor");
        int iEff = ops.FindIndex(o => o.Kind == "Effect");
        int iEvn = ops.FindIndex(o => o.Kind == "Event");

        Assert.True(iMap >= 0 && iActor > iMap && iEff > iActor && iEvn > iEff);
    }

    // ===================== RenderGuide =====================

    [Fact]
    public void RenderGuide_NoMySelfReturnsEmpty()
    {
        var r = NoActorSource(WithTex(Make()));
        r.HasMySelf = false;
        r.GuideX = 0; r.GuideY = 0; r.GuideR = 100; r.GuideB = 100;

        Assert.Empty(r.RenderGuide());
    }

    [Fact]
    public void RenderGuide_FillsBoundsRect()
    {
        var r = NoActorSource(WithTex(Make()));
        r.GuideX = 10; r.GuideY = 20; r.GuideR = 110; r.GuideB = 220;

        var ops = r.RenderGuide();

        Assert.Single(ops);
        Assert.Equal(10, ops[0].X);
        Assert.Equal(20, ops[0].Y);
        Assert.Equal(100, ops[0].Width);               // R - X
        Assert.Equal(200, ops[0].Height);              // B - Y
    }

    [Fact]
    public void RenderGuide_EmptyRectStillEmitsOp()
    {
        // 原文无 R/B 非零判断（该判断在 ClMain.pas 7453 调用侧）
        var r = NoActorSource(WithTex(Make()));
        r.GuideX = 0; r.GuideY = 0; r.GuideR = 0; r.GuideB = 0;

        var ops = r.RenderGuide();

        Assert.Single(ops);
        Assert.Equal(0, ops[0].Width);
        Assert.Equal(0, ops[0].Height);
    }

    [Fact]
    public void RenderGuide_DoesNotEmitLightOps()
    {
        var r = NoActorSource(WithTex(Make()));
        r.GuideX = 0; r.GuideY = 0; r.GuideR = 50; r.GuideB = 50;

        r.RenderGuide();

        // RenderGuide 不触碰光照列表
        Assert.Empty(r.RenderLight());
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>魔法特效子类余部第一片（批次J70，18 族）。</summary>
public sealed class MagicEffectTailTests : IDisposable
{
    private sealed class Target : IMagicTarget
    {
        public int Rx { get; set; }
        public int Ry { get; set; }
        public int ShiftX { get; set; }
        public int ShiftY { get; set; }
    }

    private static uint _tick;

    private static void Advance(uint ms) => _tick += ms;

    public MagicEffectTailTests()
    {
        _tick = 1000;
        MagicEffEnv.Reset();
        MagicEffEnv.RandomFn = _ => 3;
        SceneTime.TickNow = () => _tick;
    }

    public void Dispose()
    {
        MagicEffEnv.Reset();
    }

    [Fact]
    public void GetFlyDirection_EightWay()
    {
        Assert.Equal(4, MagicTailConsts.GetFlyDirection(50, 50, 50, 60)); // 正下 DR_DOWN
        Assert.Equal(0, MagicTailConsts.GetFlyDirection(50, 50, 50, 40)); // 正上
        Assert.Equal(2, MagicTailConsts.GetFlyDirection(50, 50, 70, 50)); // 正右
        Assert.Equal(6, MagicTailConsts.GetFlyDirection(50, 50, 30, 50)); // 正左
        Assert.Equal(1, MagicTailConsts.GetFlyDirection(50, 50, 56, 44)); // 右上（-fy < fx/3? -6 vs 2 → 否；-fy>2.5*6? 否 → UPRIGHT）
    }

    [Fact]
    public void CopySelf_DirFolding()
    {
        var eff = new TCopySelf(38, 5, 10, 10, 20, 20, TMagicType.mtFly, false, 0)
        {
            m_boActive = true,
            FlyX = 480,
            FlyY = 320,
            fireX = 0,
            fireY = 0,
            FixedEffect = false,
            EffectBase = 400,
            curframe = 2,
            px = 3,
            py = 4,
        };
        eff.Dir16 = 7; // 7/2=3
        var d = eff.ComputeDraw();
        Assert.NotNull(d);
        Assert.Equal(400 + MagicEffConsts.FLYBASE + 3 * 10 + 2, d!.Value.Img);
        Assert.True(d.Value.Blend);
    }

    [Fact]
    public void FlyingFireBall_DirectionImage()
    {
        var eff = new TFlyingFireBall(38, 5, 0, 0, 100, 100, TMagicType.mtFly, false, 0)
        {
            m_boActive = true,
            FlyX = 500,
            FlyY = 500,
            fireX = 0,
            fireY = 0,
            FlyImageBase = 447,
            curframe = 1,
            px = 5,
            py = 5,
        };
        var d = eff.ComputeDraw(); // 500,500 → 100,100：fx=fy=-400 → -fy(400)>-fx*2.5(1000)?否；-fy<-fx/3(133)?否 → DR_UPLEFT(7)
        Assert.NotNull(d);
        Assert.Equal(447 + 7 * 10 + 1, d!.Value.Img);
    }

    [Fact]
    public void BujaukEffect4_MainAndShadowLayers()
    {
        var target = new Target { Rx = 5, Ry = 5 };
        var eff = new TExploBujaukEffect4(10, 10, 30, 30, target)
        {
            m_boActive = true,
            FlyX = 500,
            FlyY = 320,
            fireX = 0,
            fireY = 0,
            FixedEffect = false,
        };
        eff.Dir16 = 4;
        eff.curframe = 1;
        var layers = eff.ComputeDrawLayers();
        Assert.Equal(2, layers.Count); // 主符 + img+170 副图
        Assert.Equal(140 + 4 * 10 + 1, layers[0].Img);
        Assert.False(layers[0].Blend);  // MagicBlend=false → Draw
        Assert.Equal(140 + 4 * 10 + 170 + 1, layers[1].Img);
        Assert.True(layers[1].Blend);
        Assert.Equal(6, eff.ImgLibId);   // g_WMagic6Images
        Assert.Equal(300, eff.MagExplosionBase);
    }

    [Fact]
    public void MoonMon_NoDirectionImage()
    {
        var target = new Target();
        var eff = new TMoonMonEffect(220, 0, 0, 30, 30, target)
        {
            m_boActive = true,
            FlyX = 100,
            FlyY = 100,
            fireX = 0,
            fireY = 0,
            curframe = 3,
            px = 1,
            py = 1,
        };
        eff.Dir16 = 9; // 月灵飞行图号不含方向
        var d = eff.ComputeDraw();
        Assert.NotNull(d);
        Assert.Equal(220 + 3, d!.Value.Img);
        Assert.True(d.Value.Blend);
    }

    [Fact]
    public void BujaukGround_ArriveExplosionThenLevelImages()
    {
        var eff = new TBujaukGroundEffect(200, 11, 0, 0, 60, 0)
        {
            MagOwner = new object(),
            ExplosionFrame = 10,
        };
        eff.Tick = () => SceneTime.TickNow();
        Advance(100);
        Assert.True(eff.Run()); // 首拍推进
        Advance(100);
        eff.FlyX = 62;
        eff.FlyY = 0;
        Assert.True(eff.Run()); // |60−62|≤15 → 爆炸切固定
        Assert.True(eff.FixedEffect);
        Assert.Equal(1, eff.ExplosionSoundCalls);
        Assert.False(eff.Repetition);
        Assert.Equal(0, eff.start);
        Assert.Equal(10, eff.frame);

        // NewLevel=2 + MagicNumber=11 → 2470+curframe
        eff.NewLevel = 2;
        eff.curframe = 4;
        var d = eff.ComputeDraw();
        Assert.Equal(2470 + 4, d!.Value.Img);
        Assert.True(d.Value.Blend);
    }

    [Fact]
    public void BujaukGround_Curse46_UsesEffectBaseLookup()
    {
        var eff = new TBujaukGroundEffect(200, 46, 0, 0, 999, 999);
        eff.Tick = () => SceneTime.TickNow();
        eff.NewLevel = 0;
        // 直接触发固定态
        eff.FixedEffect = true;
        eff.curframe = 2;
        var d = eff.ComputeDraw();
        // GetEffectBase(45, 0) → 表 Effect(0,45)=(940, WMagic2)
        Assert.Equal(940 + 170 + 2, d!.Value.Img);
    }

    [Fact]
    public void NormalDraw_FramesAndC8()
    {
        var eff = new TNormalDrawEffect(100, 200, 7, 500, 4, 50, true)
        {
            m_boActive = true,
            FlyX = 96,
            FlyY = 64,
        };
        Advance(51);
        Assert.True(eff.Run());
        Assert.Equal(1, eff.curframe);
        var d = eff.ComputeDraw();
        Assert.Equal(501, d!.Value.Img);
        Assert.True(d.Value.Blend); // boC8
        // 播完回卷
        for (int i = 0; i < 10; i++)
        {
            Advance(51);
            if (!eff.Run()) break;
        }
        Assert.Equal(0, eff.curframe);
    }

    [Fact]
    public void BloodBite_LevelSegments()
    {
        var lv2 = new TBloodBiteEffect(38, 5, 0, 0, 10, 10, TMagicType.mtFly, false, 0, 2);
        Assert.Equal(690, lv2.MagExplosionBase);
        var lv5 = new TBloodBiteEffect(38, 5, 0, 0, 10, 10, TMagicType.mtFly, false, 0, 5);
        Assert.Equal(840, lv5.MagExplosionBase);
        var lv9 = new TBloodBiteEffect(38, 5, 0, 0, 10, 10, TMagicType.mtFly, false, 0, 9);
        Assert.Equal(990, lv9.MagExplosionBase);

        lv2.curframe = 25;
        var d = lv2.ComputeDraw();
        Assert.Equal(690 + (2 - 1) * 20 + 25, d!.Value.Img); // curframe≥20 → (NL−1)×20
        lv5.curframe = 25;
        Assert.Equal(840 + (5 - 4) * 20 + 25, lv5.ComputeDraw()!.Value.Img);
        lv9.curframe = 25;
        Assert.Equal(990 + (9 - 7) * 20 + 25, lv9.ComputeDraw()!.Value.Img);
        lv2.curframe = 10;
        Assert.Equal(690 + 10, lv2.ComputeDraw()!.Value.Img);
        Assert.Equal(3, lv2.light);
    }

    [Fact]
    public void Continuous_EffectNumberBranches()
    {
        var eff105 = new TContinuousEffect(300, 0, 0, 50, 50, null) { EffectNumber = 105 };
        eff105.m_boActive = true;
        eff105.FlyX = 500;
        eff105.FlyY = 500;
        eff105.fireX = 0;
        eff105.fireY = 0;
        eff105.curframe = 2;
        eff105.Dir16 = 6;
        // 惊雷爆(105)：EffectBase+FLYBASE+curframe，再 +curframe（原文 +2×curframe 缺陷保留）
        Assert.Equal(300 + MagicEffConsts.FLYBASE + 4, eff105.ComputeDraw()!.Value.Img);

        var eff109 = new TContinuousEffect(310, 0, 0, 50, 50, null) { EffectNumber = 109 };
        eff109.m_boActive = true;
        eff109.FixedEffect = true;
        eff109.MagExplosionBase = 400;
        eff109.Dir16 = 4;
        eff109.curframe = 3;
        eff109.FlyX = 100;
        eff109.FlyY = 100;
        // 八卦掌(109)：MagExplosionBase+curframe+Dir16×10
        Assert.Equal(400 + 3 + 40, eff109.ComputeDraw()!.Value.Img);
    }

    [Fact]
    public void BingTianXueDi_HalfDirImage()
    {
        var eff = new TExploBingtianxuediEffect(700, 0, 0, 40, 40, null)
        {
            m_boActive = true,
            Dir16 = 9, // div 2 → 4
            curframe = 5,
            FlyX = 100,
            FlyY = 100,
        };
        Assert.Equal(700 + 4 * 10 + 5, eff.ComputeDraw()!.Value.Img);
    }

    [Fact]
    public void SanYanZhou_FireNodeQueue()
    {
        var target = new Target();
        var eff = new TExploSanYanZhouEffect(500, 0, 0, 80, 80, target)
        {
            MagOwner = null,
        };
        // 推进三拍：首拍需 ≥2×NFT 才同时推进节点（fire2time 门）
        Advance(160);
        Assert.True(eff.Run());
        Assert.Equal(1, eff.FireNodes[0].firenumber);
        Advance(60);
        eff.Run();
        // 节点扩散：firenodes[1] = 前一节点快照
        Advance(60);
        Assert.True(eff.Run());

        eff.OutofOil = true; // 油尽后节点耗尽 → 结束
        for (int i = 0; i < 6; i++)
        {
            Advance(120);
            if (!eff.Run()) break;
        }
        Assert.All(eff.FireNodes.Take(1), n => Assert.True(n.firenumber > 0 || n.X == 0));
    }

    [Fact]
    public void HuXiaoJue_CurframeClampAndLayers()
    {
        var target = new Target();
        var eff = new TExploHuXiaoJueZhouEffect(620, 0, 0, 90, 90, target)
        {
            m_boActive = true,
            FlyX = 600,
            FlyY = 600,
            fireX = 0,
            fireY = 0,
            Dir16 = 2,
            curframe = 7, // ≥5 → 归 0
        };
        var layers = eff.ComputeDrawLayers();
        Assert.Equal(2, layers.Count);
        Assert.Equal(3580 + 2 * 5 + 0, layers[0].Img);
        Assert.False(layers[0].Blend);
        Assert.Equal(3580 + 2 * 5 + 80, layers[1].Img);
        Assert.True(layers[1].Blend);
    }

    [Fact]
    public void RedThunder_RandomSegment()
    {
        var target = new Target();
        var eff = new TRedThunderEffect(900, 40, 40, target)
        {
            m_boActive = true,
            FlyX = 100,
            FlyY = 100,
            curframe = 2,
        };
        Assert.Equal(3, eff.n0); // RandomFn 固定 3
        Assert.Equal(900 + 7 * 3 + 2, eff.ComputeDraw()!.Value.Img);
        Assert.Equal(16, eff.ImgLibId); // WDragonImg
    }

    [Fact]
    public void FireDragon_DirRange7To12()
    {
        var eff = new TFireDragonEffect(38, 600, 0, 0, 80, 80, TMagicType.mtFly, false, 0)
        {
            m_boActive = true,
            FlyX = 500,
            FlyY = 500,
            fireX = 0,
            fireY = 0,
            curframe = 1,
            px = 2,
            py = 2,
        };
        eff.Dir16 = 9; // EffectBase + FLYBASE×2
        Assert.Equal(600 + MagicEffConsts.FLYBASE * 2 + 1, eff.ComputeDraw()!.Value.Img);
        eff.Dir16 = 3; // 越界 → 不绘
        Assert.Null(eff.ComputeDraw());
    }

    [Fact]
    public void JNExploBujauk_140ShadowLayer()
    {
        var eff = new TJNExploBujaukEffect(140, 0, 0, 200, 200, null)
        {
            m_boActive = true,
            FlyX = 800,
            FlyY = 800,
            fireX = 0,
            fireY = 0,
            Dir16 = 5, // div 2 → 2
            curframe = 3,
            px = 1,
            py = 1,
        };
        var layers = eff.ComputeDrawLayers();
        Assert.Equal(2, layers.Count); // EffectBase=140 → 副图
        Assert.Equal(140 + 2 * 10 + 3, layers[0].Img);
        Assert.False(layers[0].Blend);
        Assert.Equal(140 + 2 * 10 + 3 + 170, layers[1].Img);
        Assert.True(layers[1].Blend);

        var nonShadow = new TJNExploBujaukEffect(120, 0, 0, 200, 200, null)
        {
            m_boActive = true,
            FlyX = 800,
            FlyY = 800,
            Dir16 = 4,
            curframe = 1,
        };
        Assert.Single(nonShadow.ComputeDrawLayers());
    }

    [Fact]
    public void Explosion2_SecondStageSwap()
    {
        var eff = new TExplosion2Effect(38, 5, 0, 0, 60, 60, TMagicType.mtExplosion, false, 0)
        {
            ExplosionFrame = 6,
            MagExplosionBase = 100,
            MagExplosionBase_2 = 800,
            ExplosionFrame_2 = 4,
            FixedEffect = true,
        };
        eff.Tick = () => SceneTime.TickNow();
        Advance(50);
        Assert.True(eff.Run());   // 首段推进
        eff.curframe = eff.start + eff.frame - 1;
        Advance(50);
        // 首段末拍：Shift 返回 false 的同一调用内即切二段并返回 true
        Assert.True(eff.Run());
        Assert.True(eff.IsPlayExplosion2);
        Assert.Equal(4, eff.frame);
        Assert.Equal(800, eff.MagExplosionBase);
        Assert.Equal(0, eff.curframe);
    }

    [Fact]
    public void PlayEffect_ReplayCounts()
    {
        var target = new Target();
        var eff = new TPlayEffect(5, 30, 30, 400, 4, 2, true, target)
        {
            m_boActive = true,
        };
        eff.Tick = () => SceneTime.TickNow();
        int plays = 0;
        for (int i = 0; i < 40; i++)
        {
            Advance(110);
            if (!eff.Run()) plays++;
        }
        Assert.Equal(2, eff.m_nPlayCount); // 播 2 遍
        Assert.True(plays >= 1);
        var d = eff.ComputeDraw();
        Assert.NotNull(d);
        Assert.True(d!.Value.Blend);
    }

    [Fact]
    public void HeroShow_ShiftFixedTarget()
    {
        var target = new Target { Rx = 6, Ry = 8, ShiftX = 5, ShiftY = 7 };
        var eff = new THeroShowEffect(500, 10, target);
        Assert.Equal(1, eff.ImgLibId); // WEffectImg
        eff.Tick = () => SceneTime.TickNow();
        eff.FixedEffect = true;
        eff.frame = 10;
        eff.Shift();
        Assert.Equal(6 * 48 + 5, eff.FlyX); // ScreenXY + ShiftX

        var at = new THeroShowEffect(500, 10, 96, 64);
        at.FixedEffect = true;
        at.frame = 10;
        at.Tick = () => SceneTime.TickNow();
        at.Shift();
        Assert.Equal(96 * 48, at.FlyX); // nil 目标：rx=targetx → 屏坐标
    }

    [Fact]
    public void ShowPlay_MagicId66Offset()
    {
        var target = new Target { Rx = 3, Ry = 4, ShiftX = 1, ShiftY = 2 };
        var eff = new TShowPlayEffect(900, 8, target)
        {
            m_boActive = true,
            FlyX = 200,
            FlyY = 200,
            fireX = 0,
            fireY = 0,
            MagicId = 66,
            curframe = 5,
            m_DrawBlend = true,
            FixedEffect = true,
        };
        var layers = eff.ComputeDrawLayers();
        Assert.Single(layers);
        Assert.Equal(900 + 5, layers[0].Img);
        // ny + (py 基类缺省 0 → MagicId66 修正 −225) − 16
        int expectedNy = eff.nY + 2 + (0 - 225) - 16;
        Assert.Equal(expectedNy, layers[0].Y);
        Assert.Equal(60, eff.NextFrameTime); // 与 THeroShow 的 100 差异点
    }
}

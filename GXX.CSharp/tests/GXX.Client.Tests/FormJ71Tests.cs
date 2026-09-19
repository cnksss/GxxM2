using System;
using System.Linq;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>魔法特效子类第二片（批次J71）：TCustomMonFlyEffect / TCustomMonTargetEffect。</summary>
public sealed class MagicEffectCustomMonTests : IDisposable
{
    private sealed class Target : IMagicTarget
    {
        public int Rx { get; set; }
        public int Ry { get; set; }
        public int ShiftX { get; set; }
        public int ShiftY { get; set; }
    }

    private sealed class Owner : IMagicTarget
    {
        public int Rx { get; set; }
        public int Ry { get; set; }
        public int ShiftX { get; set; }
        public int ShiftY { get; set; }
    }

    private static uint _tick;

    private static void Advance(uint ms) => _tick += ms;

    public MagicEffectCustomMonTests()
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
    public void FlyCtor_FireGunModeDetachesTarget()
    {
        var target = new Target();
        var eff = new TCustomMonFlyEffect(300, 0, 0, 50, 50, target, flyFrameCount: 6,
            explosionImgLibId: 3, explosionLockTarget: false, isFireGunMode: true);
        Assert.Null(eff.TargetActor);      // 地狱火模式脱离目标 chongchong 2016-08-19
        Assert.Equal(6, eff.FireNodes.Length);
        Assert.True(eff.IsFireGunMode);
        Assert.False(eff.OutofOil);
        Assert.Equal(50, eff.NextFrameTime);
        Assert.True(eff.MagicBlend);
        Assert.Equal(-1, eff.FlyEffImgLibId);
        Assert.Equal(-1, eff.FlyEffStartIndex);
        Assert.Equal(CustomMonEffConsts.MdmBlend, eff.FlyDrawMode);
        Assert.Equal(0, eff.FlyLightRange);
        Assert.Empty(eff.TargetList);
        Assert.False(eff.TigerOnExplosion);
        Assert.False(eff.TigerOnFinished);
        Assert.True(eff.MagicBlend2 == true); // MagicBlend2 缺省 true
        Assert.Equal(-1, eff.MagExplosionBase2);
        Assert.Equal(3, eff.ExplosionImgLibId); // 构造参数传入

        // FlyFrameCount ≤ 2：不建节点队列、不脱离目标
        var small = new TCustomMonFlyEffect(300, 0, 0, 50, 50, target, flyFrameCount: 2,
            explosionImgLibId: 3, explosionLockTarget: false, isFireGunMode: true);
        Assert.NotNull(small.TargetActor);
        Assert.Empty(small.FireNodes);
        Assert.False(small.IsFireGunMode);
    }

    [Fact]
    public void FlyRun_FireGunMode_NodeQueueAndOilOut()
    {
        var owner = new Owner { Rx = 0, Ry = 0 };
        var eff = new TCustomMonFlyEffect(300, 0, 0, 50, 50, null, flyFrameCount: 6,
            explosionImgLibId: 3, explosionLockTarget: false, isFireGunMode: true)
        {
            MagOwner = owner,
            MagOwnerPosition = owner,
        };
        eff.Tick = () => _tick;

        Advance(60);
        Assert.True(eff.Run()); // 首拍：布节点
        Assert.Equal(1, eff.FireNodes[0].firenumber);
        Assert.Equal(1, eff.FireNodes[1].firenumber);

        Advance(60);
        eff.Run();
        Advance(2000); // 远超 800ms → 油尽
        Assert.True(eff.Run());
        Assert.True(eff.OutofOil);

        // 油尽后节点耗尽 → Run 返回 false
        bool alive = true;
        for (int i = 0; i < 20; i++)
        {
            Advance(60);
            if (!eff.Run()) { alive = false; break; }
        }
        Assert.False(alive);
    }

    [Fact]
    public void FlyRun_NormalMode_ExplosionSoundOnce()
    {
        var owner = new Owner();
        var eff = new TCustomMonFlyEffect(300, 0, 0, 50, 50, null, flyFrameCount: 6,
            explosionImgLibId: 3, explosionLockTarget: true, isFireGunMode: false)
        {
            MagOwner = owner,
            MagOwnerExplosionSound = () => 4242,
            FixedEffect = true,
        };
        eff.Tick = () => _tick;
        Advance(60);
        Assert.True(eff.Run());
        Assert.True(eff.IsPlaySound);
        Assert.Equal(new[] { 4242 }, eff.PlayedSounds); // 爆炸音只播一次
        Advance(60);
        eff.Run();
        Assert.Single(eff.PlayedSounds);
    }

    [Fact]
    public void FlyDraw_Normal_FlightDualLayersAndModes()
    {
        var eff = new TCustomMonFlyEffect(300, 0, 0, 50, 50, null, flyFrameCount: 6,
            explosionImgLibId: 3, explosionLockTarget: false, isFireGunMode: false)
        {
            m_boActive = true,
            FlyX = 500,
            FlyY = 500,
            fireX = 0,
            fireY = 0,
            curframe = 2,
            EffectBase = 300,
            FlyEffImgLibId = 9,
            FlyEffStartIndex = 500,
            FlyDrawMode = CustomMonEffConsts.MdmNormal,
            FlyEffDrawMode = CustomMonEffConsts.MdmBlend,
        };
        var layers = eff.ComputeDrawLayers();
        Assert.Equal(2, layers.Count);
        Assert.Equal(500 + 2, layers[0].Img);             // FlyEffStartIndex + curframe
        Assert.True(layers[0].Blend);                     // FlyEffDrawMode=mdmBlend
        Assert.Equal(300 + 2, layers[1].Img);             // EffectBase + curframe
        Assert.False(layers[1].Blend);                    // FlyDrawMode=mdmNormal
    }

    [Fact]
    public void FlyDraw_ExplosionStage_EventsAndLockTarget()
    {
        int explosionEvents = 0, finishedEvents = 0;
        var target = new Target();
        var eff = new TCustomMonFlyEffect(300, 0, 0, 50, 50, target, flyFrameCount: 6,
            explosionImgLibId: 3, explosionLockTarget: false, isFireGunMode: false)
        {
            m_boActive = true,
            FlyX = 500,
            FlyY = 500,
            fireX = 0,
            fireY = 0,
            FixedEffect = true,
            curframe = 8,
            ExplosionFrame = 10,
            MagExplosionBase = 700,
            MagExplosionBase2 = 750,
            MagicBlend = true,
            MagicBlend2 = false,
            NextExplosionFrameTime = 90,
            OnExplosion = () => explosionEvents++,
            OnFinished = () => finishedEvents++,
        };
        var layers = eff.ComputeDrawLayers();

        Assert.Equal(1, explosionEvents);               // TigerOnExplosion 只触发一次
        Assert.Equal(1, finishedEvents);                // curframe(8) ≥ 10-2 → Finished
        Assert.True(eff.TigerOnExplosion);
        Assert.True(eff.TigerOnFinished);
        Assert.Null(eff.TargetActor);                   // ExplosionLockTarget=false → 脱离目标
        Assert.Equal(90, eff.NextFrameTime);            // 换爆燃帧间隔

        Assert.Equal(2, layers.Count);                  // MagExplosionBase 与 MagExplosionBase2 各一层
        Assert.Equal(700 + 8, layers[0].Img);
        Assert.True(layers[0].Blend);
        Assert.Equal(750 + 8, layers[1].Img);
        Assert.False(layers[1].Blend);                  // MagicBlend2=false
    }

    [Fact]
    public void FlyDraw_FireGun_NodeDualLibsWithDedup()
    {
        var eff = new TCustomMonFlyEffect(300, 0, 0, 50, 50, null, flyFrameCount: 4,
            explosionImgLibId: 3, explosionLockTarget: false, isFireGunMode: true)
        {
            m_boActive = true,
            FlyX = 100,
            FlyY = 100,
            fireX = 0,
            fireY = 0,
            FlyEffImgLibId = 9,
            FlyEffStartIndex = 500,
            FlyDrawMode = CustomMonEffConsts.MdmBlend,
            FlyEffDrawMode = CustomMonEffConsts.MdmNormal,
            EffectBase = 300,
        };
        // 手工布置两节点
        eff.FireNodes[0] = new TFireNode { firenumber = 1, X = 100, Y = 100 };
        eff.FireNodes[1] = new TFireNode { firenumber = 2, X = 110, Y = 110 };
        eff.FireNodes[2] = new TFireNode { firenumber = 3, X = 120, Y = 120 };
        eff.FireNodes[3] = new TFireNode { firenumber = 4, X = 130, Y = 130 };

        var layers = eff.ComputeDrawLayers();
        // 节点 0..3 各产出 FlyEff 层 + EffectBase 层（节点 0 的 firenumber=1 < i+1=1? 否 → 绘）
        Assert.Equal(8, layers.Count);
        Assert.Equal(500, layers[0].Img); // FlyEffStartIndex + firenumber − 1
        Assert.False(layers[0].Blend);    // FlyEffDrawMode=mdmNormal
        Assert.Equal(300, layers[1].Img);
        Assert.True(layers[1].Blend);
    }

    [Fact]
    public void TargetCtor_PositionVersion_Prescreens()
    {
        var eff = new TCustomMonTargetEffect(400, 450, 10, nX: 3, nY: 4);
        Assert.Equal(3 * 48, eff.targetx);   // ScreenXYfromMCXY 预变换
        Assert.Equal(4 * 32, eff.targety);
        Assert.Equal(1, eff.ImgLibId);       // g_WEffectImg
        Assert.Null(eff.TargetActor);
        Assert.Equal(400, eff.MagExplosionBase);
        Assert.Equal(450, eff.MagExplosionBase2);
        Assert.Equal(10, eff.ExplosionFrame);
        Assert.Equal(100, eff.NextFrameTime);
        Assert.Equal(CustomMonEffConsts.MdmBlend, eff.DrawMode);
        Assert.Equal(CustomMonEffConsts.MdmBlend, eff.DrawMode2);
    }

    [Fact]
    public void TargetCtor_TargetVersion_ShiftOnCreate()
    {
        var target = new Target { Rx = 6, Ry = 8, ShiftX = 5, ShiftY = 7 };
        var eff = new TCustomMonTargetEffect(400, 450, 10, target);
        Assert.Same(target, eff.TargetActor);
        Assert.Equal(6 * 48 + 5, eff.FlyX);  // 构造尾 Shift 已定位（修正爆炸首帧错位 2018-02-12）
        Assert.Equal(8 * 32 + 7, eff.FlyY);
    }

    [Fact]
    public void TargetDraw_FlightAndDualExplosionLayers()
    {
        var target = new Target { Rx = 2, Ry = 3 };
        var eff = new TCustomMonTargetEffect(400, 450, 10, target)
        {
            m_boActive = true,
            FlyX = 300,
            FlyY = 300,
            fireX = 0,
            fireY = 0,
            FixedEffect = false,
            DrawMode = CustomMonEffConsts.MdmNormal,
            DrawMode2 = CustomMonEffConsts.MdmBlend,
            curframe = 4,
        };
        var layers = eff.ComputeDrawLayers();
        Assert.Single(layers); // 飞行态仅单层（EffectBase + FLYBASE + Dir16×10）
        // Dir16 来自基类构造 GetFlyDirection16(2,3,2,3)=8（重合缺省向）
        Assert.Equal(400 + MagicTailConsts.FlyBase + 8 * 10 + 4, layers[0].Img);
        Assert.False(layers[0].Blend);

        // 固定态双层
        eff.FixedEffect = true;
        eff.curframe = 3;
        eff.DrawMode = CustomMonEffConsts.MdmBlend;
        var expLayers = eff.ComputeDrawLayers();
        Assert.Equal(2, expLayers.Count);
        Assert.Equal(400 + 3, expLayers[0].Img);
        Assert.True(expLayers[0].Blend);
        Assert.Equal(450 + 3, expLayers[1].Img);
        Assert.True(expLayers[1].Blend);
    }

    [Fact]
    public void Target_OnFinished_AtLastTwoFrames()
    {
        var target = new Target();
        int finished = 0;
        var eff = new TCustomMonTargetEffect(400, 450, 10, target)
        {
            m_boActive = true,
            FixedEffect = true,
            ExplosionFrame = 10,
            OnFinished = () => finished++,
        };
        eff.curframe = 5;
        eff.CheckOnFinished();
        Assert.Equal(0, finished); // 5 < 8 未到末两帧

        eff.curframe = 8;
        eff.CheckOnFinished();
        Assert.Equal(1, finished);
        eff.curframe = 9;
        eff.CheckOnFinished();
        Assert.Equal(1, finished); // TigerOnFinished 只触发一次
    }

    [Fact]
    public void Target_MagicId66BoundaryFix()
    {
        var target = new Target();
        var eff = new TCustomMonTargetEffect(400, 450, 30, target) { MagicId = 66 };
        int pyBefore = eff.py, pxBefore = eff.px;
        eff.curframe = 5;
        eff.ApplyMagicId66BoundaryFix();
        Assert.Equal(pyBefore - 225, eff.py);
        Assert.Equal(pxBefore + 25, eff.px);

        eff.curframe = 25; // ≥20 不修正
        eff.ApplyMagicId66BoundaryFix();
        Assert.Equal(pyBefore - 225, eff.py);
    }
}

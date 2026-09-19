using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>批次J48：magiceff.pas 特效类族（TMagicEff 构造/Shift/GetFlyXY/Run + 飞行系与地狱火子类）1:1 测试。</summary>
public sealed class MagicEffectTests : IDisposable
{
    public MagicEffectTests()
    {
        MagicEffEnv.Reset();
        MagicEffEnv.SelfRx = 50;
        MagicEffEnv.SelfRy = 40;
    }

    public void Dispose()
    {
        MagicEffEnv.Reset();
        SceneTime.TickNow = () => (uint)Environment.TickCount;
    }

    [Fact]
    public void Constructor_FlyFamily()
    {
        // mtFly（id>=84 不进入 <84 强制块）：start=0/frame=6/ExplosionFrame=10/非固定/可重复
        var fly = new TMagicEff(100, 9, 0, 0, 100, 50, TMagicType.mtFly, true, 0);
        Assert.Equal(0, fly.start);
        Assert.Equal(6, fly.frame);
        Assert.Equal(10, fly.ExplosionFrame);
        Assert.False(fly.FixedEffect);
        Assert.True(fly.Repetition);
        // id=38 → frame=10
        var id38 = new TMagicEff(38, 9, 0, 0, 100, 50, TMagicType.mtFly, true, 0);
        Assert.Equal(10, id38.frame);
        // id=39：frame=4/ExplosionFrame=8 随即被 id<84 块覆写为 10/10（原文死代码形态）
        var id39 = new TMagicEff(39, 9, 0, 0, 100, 50, TMagicType.mtFly, true, 0);
        Assert.Equal(10, id39.frame);
        Assert.Equal(10, id39.ExplosionFrame);
        Assert.Equal(1, id39.bt80);
        // id<84（=81）：bt80=1、强制重复、frame=10、MagExplosionBase=190、ExplosionFrame=10
        var id81 = new TMagicEff(81, 9, 100, 100, 200, 150, TMagicType.mtFly, false, 0);
        Assert.Equal(1, id81.bt80);
        Assert.True(id81.Repetition);
        Assert.Equal(10, id81.frame);
        Assert.Equal(190, id81.MagExplosionBase);
        // bt80≠0 → 不再追加 EXPLOSIONBASE
        Assert.Equal(190, id81.MagExplosionBase);
        // id=83 → bt81=3（EffectBase=180 被尾部 effnum 覆写）
        var id83 = new TMagicEff(83, 9, 100, 100, 200, 150, TMagicType.mtFly, false, 0);
        Assert.Equal(9, id83.EffectBase);
        Assert.Equal(3, id83.bt81);
    }

    [Fact]
    public void Constructor_EffectBase_Override_By81()
    {
        // id=81：SelfRx>=84 → EffectBase=130；<84 → 140
        MagicEffEnv.SelfRx = 90;
        var far = new TMagicEff(81, 9, 100, 100, 200, 150, TMagicType.mtFly, false, 0);
        Assert.Equal(9, far.EffectBase); // 原文：case 内 EffectBase=130 被尾部 EffectBase:=effnum 覆写（死代码保留）
        MagicEffEnv.SelfRx = 50;
        var near = new TMagicEff(81, 9, 100, 100, 200, 150, TMagicType.mtFly, false, 0);
        Assert.Equal(9, near.EffectBase);
        // id=82：SelfRx>=78 且 SelfRy>=48 → 150；否则 160
        MagicEffEnv.SelfRx = 90;
        MagicEffEnv.SelfRy = 50;
        var id82a = new TMagicEff(82, 9, 100, 100, 200, 150, TMagicType.mtFly, false, 0);
        Assert.Equal(9, id82a.EffectBase);
        MagicEffEnv.SelfRy = 10;
        var id82b = new TMagicEff(82, 9, 100, 100, 200, 150, TMagicType.mtFly, false, 0);
        Assert.Equal(9, id82b.EffectBase);
    }

    [Fact]
    public void Constructor_OtherTypes()
    {
        // mtExplosion：frame=-1、固定、不重复、ExplosionFrame=10
        var ex = new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mtExplosion, false, 0);
        Assert.Equal(-1, ex.frame);
        Assert.True(ex.FixedEffect);
        Assert.False(ex.Repetition);
        Assert.Equal(10, ex.ExplosionFrame);
        // bt80=0 → MagExplosionBase = EffectBase + EXPLOSIONBASE(170)
        Assert.Equal(9 + 170, ex.MagExplosionBase);
        // id=80：Random 注入 2 → EffectBase=250、light=4（尾部被覆写为 1 原文形态）、ExplosionFrame=5
        MagicEffEnv.RandomFn = _ => 2;
        var id80 = new TMagicEff(80, 9, 0, 0, 100, 50, TMagicType.mtExplosion, false, 0);
        Assert.Equal(9, id80.EffectBase); // 同上：Random 分支的 250 被尾部覆写
        Assert.Equal(5, id80.ExplosionFrame);
        Assert.Equal(1, id80.light); // 原文：构造尾部 light:=1 覆写 case 内赋值
        // id=71：ExplosionFrame=20
        var id71 = new TMagicEff(71, 9, 0, 0, 100, 50, TMagicType.mtExplosion, false, 0);
        Assert.Equal(20, id71.ExplosionFrame);
        // id=90：EffectBase=350、MagExplosionBase=350、ExplosionFrame=30
        var id90 = new TMagicEff(90, 9, 0, 0, 100, 50, TMagicType.mtExplosion, false, 0);
        Assert.Equal(9, id90.EffectBase); // 同上
        Assert.Equal(179, id90.MagExplosionBase); // bt80=0 → 尾部 MagExplosionBase=EffectBase(9)+170 覆写 350
        Assert.Equal(30, id90.ExplosionFrame);
        // mtFlyAxe / mtFlyArrow / mt14 / mt13
        var axe = new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mtFlyAxe, true, 0);
        Assert.Equal(3, axe.frame);
        Assert.Equal(3, axe.ExplosionFrame);
        Assert.False(axe.FixedEffect);
        var arrow = new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mtFlyArrow, true, 0);
        Assert.Equal(1, arrow.frame);
        Assert.Equal(1, arrow.ExplosionFrame);
        var mt14 = new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mt14, false, 0);
        Assert.Equal(-1, mt14.frame);
        Assert.True(mt14.FixedEffect);
        Assert.Equal(2, mt14.ImgLibId);
        var mt13 = new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mt13, false, 0);
        Assert.Equal(20, mt13.frame);
        Assert.True(mt13.FixedEffect);
        Assert.True(mt13.ImgLibIsMon);
    }

    [Fact]
    public void Constructor_FireDistance_And_Times()
    {
        uint tick = 900;
        var ctor = new Func<TMagicEff>(() => new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mtFly, true, 77) { Tick = () => tick });
        var eff = ctor();
        // 飞行距离：|dx|=100>|dy|=50 → tax=100 → firedisX=Round(100×5)=500、firedisY=Round(50×5)=250
        Assert.Equal(500, eff.firedisX);
        Assert.Equal(250, eff.firedisY);
        Assert.Equal(50, eff.NextFrameTime);
        Assert.Equal(77u, eff.repeattime);
        Assert.True(eff.m_boActive);
        Assert.Equal(99999, eff.prevdisx);
        // Dir16：目标在右下 → 主方向 4（右），步进判定
        Assert.Equal(5, eff.Dir16); // fy/fx = 50/100 = 0.5 → >fx/4(25) → 5；>fx/1.9(52.6)? 否 → 5
        Assert.Equal(eff.OldDir16, eff.Dir16);
        // FireMyself = SelfRx×48 / SelfRy×32
        Assert.Equal(50 * 48, eff.FireMyselfX);
        Assert.Equal(40 * 32, eff.FireMyselfY);
    }

    [Fact]
    public void GetFlyDirection16_Table()
    {
        Assert.Equal(0, TMagicEff.GetFlyDirection16(0, 0, 0, -100));   // 正上
        Assert.Equal(8, TMagicEff.GetFlyDirection16(0, 0, 0, 100));     // 正下
        Assert.Equal(12, TMagicEff.GetFlyDirection16(0, 0, -100, 0));  // 正左
        Assert.Equal(4, TMagicEff.GetFlyDirection16(0, 0, 100, 0));     // 正右
        Assert.Equal(4, TMagicEff.GetFlyDirection16(0, 0, 100, 10));    // 右略下
        Assert.Equal(8, TMagicEff.GetFlyDirection16(0, 0, 10, 100));    // 下略右（fy>fx*4）
        Assert.Equal(0, TMagicEff.GetFlyDirection16(0, 0, 10, -100));   // 上略右（-fy>fx*4）
        Assert.Equal(14, TMagicEff.GetFlyDirection16(0, 0, -10, -10));  // 左上（-fy>-fx×1.4 → 14）
    }

    [Fact]
    public void GetFlyXY_StepsByFiredis()
    {
        var eff = new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mtFly, true, 0);
        eff.GetFlyXY(90, out int fx, out int fy);
        // stepx = Round(500/900×90) = 50；stepy = Round(250/900×90) = 25
        Assert.Equal(50, fx);
        Assert.Equal(25, fy);
    }

    [Fact]
    public void Shift_NoTarget_FlightByDistance()
    {
        uint tick = 1000;
        SceneTime.TickNow = () => tick; // 构造前固定时钟（m_dwFrameTime/m_dwStartTime 落在确定性时刻）
        var eff = new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mtFly, false, 0);
        // 无目标：FlyX = fireX + Round(firedisX/900×ms)
        eff.Shift();
        // ms = tick - m_dwFrameTime（构造时已设 tick）→ 0 → FlyX/FlyY 不变
        Assert.Equal(0, eff.FlyX);
        Assert.Equal(0, eff.FlyY);
    }

    [Fact]
    public void Shift_FixedEffect_ExplodesAtTarget()
    {
        uint tick = 1000;
        var eff = new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mtExplosion, false, 0)
        {
            Tick = () => tick,
        };
        Assert.True(eff.FixedEffect);
        Assert.Equal(-1, eff.frame);
        eff.Shift();
        // frame=-1 → frame := ExplosionFrame(10)，FlyX = targetx - 主角偏移(0)
        Assert.Equal(10, eff.frame);
        Assert.Equal(100, eff.FlyX);
        Assert.Equal(50, eff.FlyY);
        // 坐标经 CXYfromMouseXY 反算
        Assert.Equal(2, eff.rx);  // 100/48
        Assert.Equal(1, eff.ry);  // 50/32 → 1
    }

    [Fact]
    public void Shift_RepeatMode_LoopsFrames()
    {
        uint tick = 1000;
        SceneTime.TickNow = () => tick;
        var eff = new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mtFly, true, 0); // Repetition=true
        Assert.Equal(0, eff.curframe);
        tick += 60; // 超过 NextFrameTime(50)
        Assert.True(eff.Shift());
        Assert.Equal(1, eff.curframe);
        // 循环到 frame 末尾回卷（frame=6 → 0..5）
        for (int i = 0; i < 10; i++)
        {
            tick += 60;
            eff.Shift();
        }
        Assert.InRange(eff.curframe, 0, 5); // 始终回卷
    }

    [Fact]
    public void Run_Timeout10s_KillsEffect()
    {
        uint tick = 1000;
        SceneTime.TickNow = () => tick;
        var eff = new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mtExplosion, false, 0);
        Assert.True(eff.Run());
        tick += 10001; // 超时 → Run 返回 false
        Assert.False(eff.Run());
    }

    [Fact]
    public void DrawEff_Compute_FlyAndExplosion()
    {
        // 飞行：img = EffectBase + FLYBASE(10) + Dir16×10 + curframe；位置含 shx/shy 补偿
        var fly = new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mtFly, false, 0);
        fly.m_boActive = true;
        fly.FixedEffect = false;
        fly.curframe = 2;
        fly.FlyX = 60; // 离开火点（|Fly-fire|>1 才绘制）
        fly.FlyY = 30;
        var draw = fly.ComputeDraw();
        Assert.NotNull(draw);
        Assert.Equal(9 + 10 + fly.Dir16 * 10 + 2, draw.Value.Img);
        Assert.True(draw.Value.Blend);
        // 爆燃：img = MagExplosionBase + curframe
        var ex = new TMagicEff(1, 9, 0, 0, 100, 50, TMagicType.mtExplosion, false, 0);
        ex.m_boActive = true;
        ex.curframe = 3;
        ex.Shift(); // frame -1 → 10 并定位
        var drawEx = ex.ComputeDraw();
        Assert.NotNull(drawEx);
        Assert.Equal(ex.MagExplosionBase + ex.curframe, drawEx.Value.Img);
        // 未激活/距离过近 → 不绘制
        var idle = new TMagicEff(1, 9, 0, 0, 0, 0, TMagicType.mtFly, false, 0);
        idle.m_boActive = true;
        Assert.Null(idle.ComputeDraw()); // FlyX==fireX 且 FlyY==fireY 且非固定
    }

    [Fact]
    public void FlyingAxe_And_Arrow_Throttles()
    {
        uint tick = 1000;
        SceneTime.TickNow = () => tick;
        var axe = new TFlyingAxe(1, 9, 0, 0, 100, 50, TMagicType.mtFlyAxe, false, 0);
        Assert.Equal(447, axe.FlyImageBase); // FLYOMAAXEBASE
        Assert.Equal(65, axe.ReadyFrame);
        Assert.True(axe.Run());              // 首次（m_dwRunTime=构造 tick）>=50? 构造后差 0 → 不推进
        tick += 10;
        Assert.True(axe.Run());              // 10ms < 50 → 节流，不推进但返回 true
        tick += 100;
        Assert.True(axe.Run());              // ≥50 → 推进

        // 箭：>100ms 节流不回写（自身分支），但 inherited TFlyingAxe.Run（≥50）会回写——原文继承链形态
        uint atick = 1000;
        SceneTime.TickNow = () => atick;
        var arrow = new TFlyingArrow(1, 9, 0, 0, 300, 300, TMagicType.mtFlyArrow, false, 0);
        atick += 101;
        // mtFlyArrow frame=1：首拍即完成（Shift 推进即到末帧）→ Run 返回 false
        Assert.False(arrow.Run());
        Assert.Equal(atick, arrow.m_dwRunTime); // 经 TFlyingAxe.Run 回写

        // 箭Ex：>100ms 推进且回写（与 TFlyingArrow 的原文差异点）
        uint etick = 1000;
        SceneTime.TickNow = () => etick;
        var ex = new TFlyingArrowEx(1, 9, 0, 0, 300, 300, TMagicType.mtFlyArrow, false, 0);
        etick += 101;
        // Ex 分支回写 m_dwRunTime=1101 后，inherited TFlyingAxe 的 ≥50 节流拦截内层推进 → 恒真
        Assert.True(ex.Run());
        Assert.Equal(etick, ex.m_dwRunTime); // 回写
    }

    [Fact]
    public void FireGun_NodePipeline_And_OilOut()
    {
        var owner = new TestTarget(0, 0); // 贴身：不触发油尽
        uint tick = 1000;
        SceneTime.TickNow = () => tick;
        var gun = new TFireGunEffect(9, 0, 0, 100, 50) { Owner = owner };

        Assert.Equal(6, gun.FireNodes.Length);
        Assert.Equal(TMagicType.mtFireGun, gun.MagicType);
        Assert.Equal(50, gun.NextFrameTime);

        // 推进三拍（Owner 未远离）→ 火节点队列推进
        for (int i = 0; i < 3; i++)
        {
            tick += 60;
            Assert.True(gun.RunFireGunStep());
        }
        Assert.Equal(1, gun.FireNodes[0].firenumber);
        Assert.Equal(2, gun.FireNodes[1].firenumber);
        Assert.Equal(3, gun.FireNodes[2].firenumber);
        Assert.False(gun.OutofOil);

        // Owner 远离（≥5 格）→ 油尽 → 节点逐个燃尽 → false
        owner.Rx = 100;
        int guard = 0;
        bool alive = true;
        while (alive && guard++ < 100)
        {
            tick += 60;
            alive = gun.RunFireGunStep();
        }
        Assert.False(alive);
    }

    [Fact]
    public void ThunderEffect_Constructor()
    {
        // TThuderEffect：同位置雷（mtThunder、不重复、固定效果）
        var thunder = new TMagicEff(111, 9, 100, 200, 100, 200, TMagicType.mtThunder, false, 0);
        Assert.True(thunder.FixedEffect);
        Assert.Equal(-1, thunder.frame);
        Assert.Equal(100, thunder.targetx);
    }

    private sealed class TestTarget : IMagicTarget
    {
        public TestTarget(int rx, int ry) { Rx = rx; Ry = ry; }
        public int Rx { get; set; }
        public int Ry { get; set; }
        public int ShiftX => 0;
        public int ShiftY => 0;
    }
}

/// <summary>TFireGunEffect.RunFireGunStep：显式节流步进包装（测试驱动用）。</summary>
public static class FireGunTestExt
{
    public static bool RunFireGunStep(this TFireGunEffect gun) => gun.Run();
}

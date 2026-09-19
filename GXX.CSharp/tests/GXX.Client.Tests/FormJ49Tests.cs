using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>批次J49：magiceff.pas 特效子类余部（TCharEffect/TMapEffect/TScrollHide/TLighting/TThunder/TLightingThunder/TExploBujauk）1:1 测试。</summary>
public sealed class MagicEffectSubclassTests : IDisposable
{
    private sealed class TestTarget : IMagicTargetEx
    {
        public TestTarget(int currX, int currY, int rx, int ry, int shiftX = 0, int shiftY = 0)
        { CurrX = currX; CurrY = currY; Rx = rx; Ry = ry; ShiftX = shiftX; ShiftY = shiftY; }
        public int CurrX { get; set; }
        public int CurrY { get; set; }
        public int Rx { get; set; }
        public int Ry { get; set; }
        public int ShiftX { get; set; }
        public int ShiftY { get; set; }
    }

    public MagicEffectSubclassTests()
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
    public void CharEffect_Constructor_And_Run()
    {
        var target = new TestTarget(10, 20, 10, 20);
        uint tick = 1000;
        SceneTime.TickNow = () => tick;
        var eff = new TCharEffect(9, 6, target);
        // 构造：以目标当前坐标为火点、mtExplosion 固定效果、frame=effframe、节流 30ms
        Assert.Equal(9, eff.EffectBase);
        Assert.Equal(6, eff.frame);
        Assert.Equal(30, eff.NextFrameTime);
        Assert.True(eff.FixedEffect);
        Assert.Equal(TMagicType.mtExplosion, eff.MagicType);
        Assert.Same(target, eff.TargetActor);

        // Run：NextFrameTime 节流推进到末帧后返回假（首拍 elapsed=0 只记录不推进）
        Assert.True(eff.Run());   // 首拍：无推进
        tick += 31;
        Assert.True(eff.Run());   // 0→1
        tick += 31;
        Assert.True(eff.Run());   // 1→2
        tick += 31;
        Assert.True(eff.Run());   // 2→3
        tick += 31;
        Assert.True(eff.Run());   // 3→4
        tick += 31;
        Assert.True(eff.Run());   // 4→5（= start+frame-1 末帧）
        tick += 31;
        Assert.False(eff.Run());  // 5→6 越界 → 停在末帧 5 → 完成
        Assert.Equal(5, eff.curframe);
        Assert.Equal(5, eff.m_nCurrentFrame);
        Assert.True(eff.SurfaceReloads > 0); // 帧变化触发 LoadSurface 调度
    }

    [Fact]
    public void CharEffect_DrawEff_FollowsTarget()
    {
        var target = new TestTarget(10, 20, 10, 20, shiftX: 6, shiftY: 4);
        var eff = new TCharEffect(9, 6, target);
        eff.curframe = 2;
        var draw = eff.ComputeDraw();
        Assert.NotNull(draw);
        Assert.Equal(9 + 2, draw.Value.Img); // EffectBase + curframe
        Assert.True(draw.Value.Blend);
        // ScreenXYfromMCXY(10,20) = (480, 640) + 目标位移
        Assert.Equal(480 + 6 + eff.px - 24, draw.Value.X);
        Assert.Equal(640 + 4 + eff.py - 16, draw.Value.Y);
    }

    [Fact]
    public void MapEffect_Run_RepeatCount_Rewind()
    {
        uint tick = 1000;
        SceneTime.TickNow = () => tick;
        var eff = new TMapEffect(9, 3, 5, 6);
        Assert.Equal(0, eff.RepeatCount);
        Assert.Null(eff.TargetActor);

        // 三拍推到末帧 → 完成（RepeatCount=0；首拍 elapsed=0 无推进）
        Assert.True(eff.Run());
        tick += 31;
        Assert.True(eff.Run());
        tick += 31;
        Assert.True(eff.Run());
        tick += 31;
        Assert.False(eff.Run());
        Assert.Equal(2, eff.curframe); // 停在末帧

        // RepeatCount=1 → 到末帧回卷 start 再播一轮
        var eff2 = new TMapEffect(9, 3, 5, 6);
        eff2.RepeatCount = 1;
        Assert.True(eff2.Run());
        tick += 31;
        Assert.True(eff2.Run());
        tick += 31;
        Assert.True(eff2.Run());
        tick += 31;
        Assert.True(eff2.Run());   // 末帧 → 回卷 start
        Assert.Equal(0, eff2.curframe);
    }

    [Fact]
    public void MapEffect_DrawEff_AtTargetXY()
    {
        var eff = new TMapEffect(9, 3, 7, 8);
        eff.curframe = 1;
        var draw = eff.ComputeDraw();
        Assert.NotNull(draw);
        Assert.Equal(9 + 1, draw.Value.Img);
        // ScreenXYfromMCXY(targetx=7, targety=8)
        Assert.Equal(7 * 48 + eff.px - 24, draw.Value.X);
        Assert.Equal(8 * 32 + eff.py - 16, draw.Value.Y);
    }

    [Fact]
    public void ScrollHide_Frame7_MarksFocusForDeletion()
    {
        uint tick = 1000;
        SceneTime.TickNow = () => tick;
        // Delphi 用法：effframe=7 → frame 字段恒为 7 → 每次 Run 都标记焦点角色删除
        var eff = new TScrollHideEffect(9, 7, 5, 6);
        long? marked = null;
        eff.FocusCretHandler = _ => { marked = 12345; return 12345L; };

        tick += 31;
        Assert.True(eff.Run());
        Assert.True(eff.LastFocusMarkedForDelete);
        Assert.Equal(12345L, marked);
    }

    [Fact]
    public void LightingEffect_AlwaysFalse()
    {
        var eff = new TLightingEffect(9, 10, 5, 6);
        Assert.False(eff.Run()); // 原文恒假
        Assert.Equal(TMagicType.mtReady, eff.MagicType); // 空构造 → 字段保持默认
    }

    [Fact]
    public void ThunderEffect_DrawAtFly()
    {
        var target = new TestTarget(10, 20, 10, 20);
        var eff = new TThuderEffect(9, 100, 200, target);
        Assert.Equal(TMagicType.mtThunder, eff.MagicType);
        Assert.True(eff.FixedEffect);
        Assert.Equal(100, eff.targetx);
        eff.curframe = 4;
        var draw = eff.ComputeDraw();
        Assert.NotNull(draw);
        Assert.Equal(9 + 4, draw.Value.Img); // EffectBase + curframe
        Assert.Equal(eff.FlyX + eff.px - 24, draw.Value.X);
        Assert.Equal(eff.FlyY + eff.py - 16, draw.Value.Y);
    }

    [Fact]
    public void LightingThunder_CurFrameGate_AndOwnerPosition()
    {
        var owner = new TestTarget(30, 40, 30, 40, shiftX: 8, shiftY: 6);
        var eff = new TLightingThunder(9, 0, 0, 100, 50, owner)
        {
            MagOwner = owner,
        };
        eff.curframe = 2;
        var draw = eff.ComputeDraw();
        Assert.NotNull(draw);
        // img = EffectBase + Dir16×10 + curframe
        Assert.Equal(9 + eff.Dir16 * 10 + 2, draw.Value.Img);
        // MagOwner 屏幕坐标 + 位移
        Assert.Equal(30 * 48 + 8 + eff.px - 24, draw.Value.X);
        Assert.Equal(40 * 32 + 6 + eff.py - 16, draw.Value.Y);

        // curframe >= 6 → 不绘制
        eff.curframe = 6;
        Assert.Null(eff.ComputeDraw());
    }

    [Fact]
    public void ExploBujauk_Plane_And_Explosion()
    {
        var target = new TestTarget(10, 20, 10, 20);
        uint tick = 1000;
        SceneTime.TickNow = () => tick; // 构造前固定时钟
        var eff = new TExploBujaukEffect(9, 0, 0, 100, 50, target);
        Assert.Equal(TMagicType.mtExploBujauk, eff.MagicType);
        Assert.Equal(3, eff.frame);
        Assert.Equal(50, eff.NextFrameTime);
        Assert.False(eff.MagicBlend);
        Assert.True(eff.Repetition);
        Assert.Same(target, eff.TargetActor);
        // 惯爆：目标距离进入阈值 → FixedEffect 切换（经基类 Shift）
        Assert.False(eff.FixedEffect);
        tick += 60;
        eff.Shift();
        // OverThrough(5→7) 方向跳变 ≥2 → 首拍即惯爆（原文行为）：frame=ExplosionFrame=10、curframe=0
        Assert.True(eff.FixedEffect);
        Assert.Equal(0, eff.curframe);
        Assert.Equal(10, eff.frame);
    }
}

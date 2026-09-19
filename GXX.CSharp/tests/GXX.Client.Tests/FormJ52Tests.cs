using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>记录式画布：复现 GameCanvas.Draw/DrawBlend/DrawAlpha 调用序。</summary>
internal sealed class RecordingCanvas : IEventCanvas
{
    public sealed record Op(string Kind, int X, int Y, EventImage Image, int Alpha, int Color);

    public List<Op> Ops { get; } = new();

    public void Draw(int x, int y, EventImage image) => Ops.Add(new("Draw", x, y, image, 0, 0));
    public void DrawBlend(int x, int y, EventImage image) => Ops.Add(new("DrawBlend", x, y, image, 0, 0));
    public void DrawAlpha(int x, int y, EventImage image, int alpha) => Ops.Add(new("DrawAlpha", x, y, image, alpha, 0));
    public void DrawColorAlpha(int x, int y, EventImage image, int color, int alpha) => Ops.Add(new("DrawColorAlpha", x, y, image, alpha, color));
}

/// <summary>clEvent.pas TClEvent 家族 1:1（批次J52）。</summary>
public sealed class ClEventTests : IDisposable
{
    private uint _now;

    public ClEventTests()
    {
        ClEventEnv.Reset();
        CustomEventEffEnv.Reset();
        ClEvent.SoundPlayed = null;
        ClEventEnv.NowFn = () => _now;
    }

    public void Dispose()
    {
        ClEventEnv.Reset();
        CustomEventEffEnv.Reset();
        ClEvent.SoundPlayed = null;
    }

    private void Advance(uint ms) => _now += ms;

    [Fact]
    public void Constructor_Defaults()
    {
        var ev = new ClEvent(77, 3, 4, Grobal2Const.ET_FIRE);
        Assert.Equal(77, ev.ServerId);
        Assert.Equal(3, ev.X);
        Assert.Equal(4, ev.Y);
        Assert.Equal(Grobal2Const.ET_FIRE, ev.EventType);
        Assert.Equal(0, ev.EventParam);
        Assert.False(ev.Blend);
        Assert.Equal(0, ev.CurFrame);
        Assert.Equal(0, ev.Light);
        Assert.True(ev.Visible);
        Assert.Equal(20u, ev.FrameTickTime);
        Assert.Null(ev.Surface);
        Assert.False(ev.KeepShow);
        Assert.False(ev.LoadSound);
    }

    [Fact]
    public void Run_FrameAdvance_OnTickTime()
    {
        var ev = new ClEvent(1, 0, 0, Grobal2Const.ET_DIGOUTZOMBI);
        Advance(10);
        ev.Run(); // 10-0=10 不大于 20 → 不推进
        Assert.Equal(0, ev.CurFrame);
        Advance(15); // 累计 25 > 20
        ev.Run();
        Assert.Equal(1, ev.CurFrame);
    }

    [Fact]
    public void LoadSurface_DigOutZombi_GrayOnDeath()
    {
        var ev = new ClEvent(1, 0, 0, Grobal2Const.ET_DIGOUTZOMBI) { Dir = 3 };
        ev.LoadSurface();
        Assert.Equal(ClEventImages.Mon6, ev.Surface!.List);
        Assert.Equal(ClEventConsts.ZombieDigUpDustBase + 3, ev.Surface.ImageIndex);
        Assert.False(ev.Surface.Gray);

        ClEventEnv.SelfDeadFn = () => true;
        ev.LoadSurface();
        Assert.True(ev.Surface!.Gray);
    }

    [Fact]
    public void LoadSurface_SafeRect_DirOffsetsAndPxClean()
    {
        var ev = new ClEvent(1, 0, 0, Grobal2Const.ET_SAFERECT) { Dir = 5, Px = 99, Py = 99 };
        ev.LoadSurface();
        Assert.Equal(ClEventImages.Magic10, ev.Surface!.List);
        Assert.Equal(2100, ev.Surface.ImageIndex);
        Assert.Equal(6, ev.Px); // DR_DOWNLEFT/DR_UPLEFT → px=6
        Assert.Equal(0, ev.Py);

        var up = new ClEvent(2, 0, 0, Grobal2Const.ET_SAFERECT) { Dir = 0 };
        up.LoadSurface();
        Assert.Equal(2050, up.Surface!.ImageIndex);
        Assert.Equal(0, up.Px);

        var upLeft = new ClEvent(3, 0, 0, Grobal2Const.ET_SAFERECT) { Dir = 7 };
        upLeft.LoadSurface();
        Assert.Equal(2040, upLeft.Surface!.ImageIndex);
        Assert.Equal(6, upLeft.Px);

        var bad = new ClEvent(4, 0, 0, Grobal2Const.ET_SAFERECT) { Dir = 9 };
        bad.LoadSurface();
        Assert.Null(bad.Surface); // Offset=-1 → 无图
    }

    [Fact]
    public void LoadSurface_FireBurnAndLevels()
    {
        var fire = new ClEvent(1, 0, 0, Grobal2Const.ET_FIRE) { CurFrame = 14 };
        fire.LoadSurface();
        Assert.Equal(ClEventImages.Magic, fire.Surface!.List);
        Assert.Equal(ClEventConsts.FireBurnBase + (14 / 2 % 6), fire.Surface.ImageIndex);

        var level2 = new ClEvent(2, 0, 0, Grobal2Const.ET_FIRELevel2) { CurFrame = 0 };
        level2.LoadSurface();
        Assert.Equal(ClEventImages.Magic7x16, level2.Surface!.List);
        Assert.Equal(90 + 10, level2.Surface.ImageIndex);
    }

    [Fact]
    public void LoadSurface_AssortedTypeImages()
    {
        var stones = new ClEvent(1, 0, 0, Grobal2Const.ET_PILESTONES) { EventParam = 3 };
        stones.LoadSurface();
        Assert.Equal(ClEventImages.Effect, stones.Surface!.List);
        Assert.Equal(ClEventConsts.StoneFragmentBase + 2, stones.Surface.ImageIndex);

        var curtain = new ClEvent(2, 0, 0, Grobal2Const.ET_HOLYCURTAIN) { CurFrame = 25 };
        curtain.LoadSurface();
        Assert.Equal(ClEventConsts.HolyCurtainBase + 5, curtain.Surface!.ImageIndex);

        var sculpture = new ClEvent(3, 0, 0, Grobal2Const.ET_SCULPEICE);
        sculpture.LoadSurface();
        Assert.Equal(ClEventImages.Mon7, sculpture.Surface!.List);
        Assert.Equal(ClEventConsts.SculptureFragment, sculpture.Surface.ImageIndex);

        var flower = new ClEvent(4, 0, 0, Grobal2Const.ET_FIREFLOWER_3) { CurFrame = 2 };
        flower.LoadSurface();
        Assert.Equal(ClEventImages.Magic3, flower.Surface!.List);
        Assert.Equal(ClEventConsts.FireFlowerBase + 20 * 2 + 2, flower.Surface.ImageIndex);

        var icepeak = new ClEvent(5, 0, 0, Grobal2Const.ET_ICEPEAK) { EventParam = 2 };
        icepeak.LoadSurface();
        Assert.Equal(ClEventImages.Mon27, icepeak.Surface!.List);
        Assert.Equal(2010 + 10 * 2 + 9, icepeak.Surface.ImageIndex);

        var door = new ClEvent(6, 0, 0, Grobal2Const.ET_DOOR3) { CurFrame = 1 };
        door.LoadSurface();
        Assert.Equal(ClEventImages.NpcImg0, door.Surface!.List);
        Assert.Equal(4490 + 20 + 1, door.Surface.ImageIndex);

        var thunder2 = new ClEvent(7, 0, 0, Grobal2Const.ET_THUNDER2) { CurFrame = 3 };
        thunder2.LoadSurface();
        Assert.Equal(ClEventImages.Magic2, thunder2.Surface!.List);
        Assert.Equal(13, thunder2.Surface.ImageIndex);

        var springsLight = new ClEvent(8, 0, 0, Grobal2Const.ET_SPRINGS_LIGHT) { CurFrame = 4 };
        springsLight.LoadSurface();
        Assert.Equal(ClEventImages.Main2, springsLight.Surface!.List);
        Assert.Equal(674, springsLight.Surface.ImageIndex);
    }

    [Fact]
    public void LoadSurface_Invisible_Noop()
    {
        var ev = new ClEvent(1, 0, 0, Grobal2Const.ET_FIRE) { Visible = false };
        ev.LoadSurface();
        Assert.Null(ev.Surface);
    }

    [Fact]
    public void Run_ThunderFourFrames_ThenInvisibleOrKeepShow()
    {
        int soundId = 0, soundCount = 0;
        ClEvent.SoundPlayed = (_, id) => { soundId = id; soundCount++; };
        var ev = new ClEvent(1, 0, 0, Grobal2Const.ET_THUNDER);

        // 推进到帧 3（<4）：加载请求 + 未隐身
        for (int i = 0; i < 3; i++)
        {
            Advance(21);
            ev.Run();
        }
        Assert.Equal(3, ev.CurFrame);
        Assert.True(ev.Visible);

        // 第 4 帧推进 → 循环：帧清零且隐身（KeepShow=false）
        Advance(21);
        ev.Run();
        Assert.Equal(0, ev.CurFrame);
        Assert.False(ev.Visible);
        Assert.True(ev.Blend);
        Assert.Equal(1, ev.Light);
        Assert.Equal(0, soundCount); // 首帧音效只在帧 0 触发

        // KeepShow 版本：循环后保持显示
        var keep = new ClEvent(2, 0, 0, Grobal2Const.ET_THUNDER) { KeepShow = true };
        for (int i = 0; i < 4; i++)
        {
            Advance(21);
            keep.Run();
        }
        Assert.True(keep.Visible);
        Assert.Equal(0, keep.CurFrame);

        // 首帧音效：帧 0 且无推进时 Run 播 1923 且只一次（帧推进先于 case，须同拍调用）
        var fresh = new ClEvent(3, 0, 0, Grobal2Const.ET_THUNDER);
        fresh.Run();
        Assert.Equal(1923, soundId);
        Assert.Equal(1, soundCount);
        Advance(21);
        fresh.Run(); // 推进到帧 1，非 0 → 不重复
        Assert.Equal(1, soundCount);
    }

    [Fact]
    public void Run_LavaTenFrames_AndLava2_ShortThrottle()
    {
        var ids = new List<int>();
        ClEvent.SoundPlayed = (_, id) => ids.Add(id);
        var lava = new ClEvent(1, 0, 0, Grobal2Const.ET_LAVA);
        lava.Run(); // 同拍：帧 0 → 首帧音效 11055
        Assert.Equal(new[] { 11055 }, ids);

        for (int i = 0; i < 10; i++)
        {
            Advance(21);
            lava.Run();
        }
        Assert.False(lava.Visible); // 10 帧循环完（KeepShow=false）
        Assert.Single(ids);

        // LAVA2：载入节流 1s（其他类型 2s）；音效 10090 后 m_boLoadSound := False（可重复）
        ids.Clear();
        var lava2 = new ClEvent(2, 0, 0, Grobal2Const.ET_LAVA2);
        lava2.LoadSurface(); // LoadSurfaceTime = 当前拍
        Advance(1500);
        lava2.Run();
        Assert.Equal(1, lava2.LoadSurfaceRequests); // 1.5s ≥ 1s（其他类型需 ≥2s）

        var fire = new ClEvent(3, 0, 0, Grobal2Const.ET_FIRE);
        fire.LoadSurface();
        Advance(1500);
        fire.Run();
        Assert.Equal(0, fire.LoadSurfaceRequests); // 1.5s < 2s
    }

    [Fact]
    public void Run_FireFlower_LoopTwentyAndSoundOnce()
    {
        int soundCount = 0;
        ClEvent.SoundPlayed = (_, _) => soundCount++;
        var ev = new ClEvent(1, 0, 0, Grobal2Const.ET_FIREFLOWER_1);
        ev.Run(); // 同拍：帧 0 → 音效一次
        Assert.Equal(1, soundCount);
        Assert.True(ev.Visible);

        ev.CurFrame = 19;
        Advance(21);
        ev.Run(); // 19→20 ≥ 20 → 清零隐身
        Assert.Equal(0, ev.CurFrame);
        Assert.False(ev.Visible);
        Assert.Equal(1, soundCount);
        Assert.True(ev.Blend);
        Assert.Equal(1, ev.Light);
    }

    [Fact]
    public void Run_CustomSafePoint_BlendGateAt55()
    {
        var low = new ClEvent(1, 0, 0, 30);
        Advance(21);
        low.Run();
        Assert.True(low.Blend);

        var high = new ClEvent(2, 0, 0, 56);
        Advance(21);
        high.Run();
        Assert.False(high.Blend);
    }

    [Fact]
    public void Run_PileStones_ParamClamp()
    {
        var ev = new ClEvent(1, 0, 0, Grobal2Const.ET_PILESTONES) { EventParam = 7 };
        ev.Run();
        Assert.Equal(5, ev.EventParam);
        Assert.Equal(1, ev.LoadSurfaceRequests); // 拍前拍后参数变化 → 重载

        var low = new ClEvent(2, 0, 0, Grobal2Const.ET_PILESTONES) { EventParam = 0 };
        low.Run();
        Assert.Equal(1, low.EventParam);
    }

    [Fact]
    public void DrawEvent_Springs_CompositeLayerThenMain()
    {
        var ev = new ClEvent(1, 10, 20, Grobal2Const.ET_SPRINGS2) { Px = 1, Py = 1, Blend = false };
        ev.LoadSurface();
        var canvas = new RecordingCanvas();
        ev.DrawEvent(100, 200, canvas);

        Assert.Equal(2, canvas.Ops.Count);
        // 合成层：530 + (SPRINGS2−SPRINGS1) 于原偏移
        Assert.Equal("Draw", canvas.Ops[0].Kind);
        Assert.Equal(530 + 1, canvas.Ops[0].Image.ImageIndex);
        Assert.Equal(101, canvas.Ops[0].X); // ax + m_nPx(原 1)
        // 主图层对坐标 px=7/py=-50
        Assert.Equal(107, canvas.Ops[1].X);
        Assert.Equal(150, canvas.Ops[1].Y); // 200 + (-50)
    }

    [Fact]
    public void DrawEvent_FireWall_DimAlpha96()
    {
        ClEventEnv.DimFireEffectFn = () => true;
        var plain = new ClEvent(1, 0, 0, Grobal2Const.ET_FIRE);
        plain.LoadSurface();
        var c1 = new RecordingCanvas();
        plain.DrawEvent(0, 0, c1);
        Assert.Equal("DrawAlpha", c1.Ops[0].Kind);
        Assert.Equal(96, c1.Ops[0].Alpha);

        plain.Blend = true;
        var c2 = new RecordingCanvas();
        plain.DrawEvent(0, 0, c2);
        Assert.Equal("DrawColorAlpha", c2.Ops[0].Kind);
        Assert.Equal(96, c2.Ops[0].Alpha);
    }

    [Fact]
    public void DrawEvent_BlendSplit()
    {
        var ev = new ClEvent(1, 0, 0, Grobal2Const.ET_DIGOUTZOMBI);
        ev.LoadSurface();
        var c1 = new RecordingCanvas();
        ev.DrawEvent(5, 6, c1);
        Assert.Equal("Draw", c1.Ops[0].Kind);
        Assert.Equal(5 + ev.Px, c1.Ops[0].X);

        ev.Blend = true;
        var c2 = new RecordingCanvas();
        ev.DrawEvent(5, 6, c2);
        Assert.Equal("DrawBlend", c2.Ops[0].Kind);
    }

    [Fact]
    public void DrawEvent_InvisibleOrNoSurface_Skips()
    {
        var ev = new ClEvent(1, 0, 0, Grobal2Const.ET_FIRE) { Visible = false };
        var canvas = new RecordingCanvas();
        ev.DrawEvent(0, 0, canvas);
        Assert.Empty(canvas.Ops);
    }
}

/// <summary>TMapEffectEvent：循环次数控制与 MAPEFFECT 图号。</summary>
public sealed class MapEffectEventTests : IDisposable
{
    private uint _now;

    private void Advance(uint ms) => _now += ms;

    public MapEffectEventTests()
    {
        ClEventEnv.Reset();
        ClEventEnv.NowFn = () => _now;
        ClEventEnv.EffectImageCountFn = () => 5;
    }

    public void Dispose() => ClEventEnv.Reset();

    [Fact]
    public void Constructor_Fields()
    {
        var ev = new MapEffectEvent(9, 1, 2, 2, 380, 20, 100, 3, boBlend: true, btLight: 4);
        Assert.Equal(Grobal2Const.ET_MAPEFFECT, ev.EventType);
        Assert.True(ev.Blend);
        Assert.Equal(2, ev.FileIndex);
        Assert.Equal(380, ev.ImageIndex);
        Assert.Equal(20, ev.ImageCount);
        Assert.Equal(100u, ev.FrameTickTime);
        Assert.Equal(3, ev.LoopCount);
        Assert.Equal(4, ev.Light);
        Assert.Equal(-1, ev.CurFrame); // 首轮 Run 立即归位 0
    }

    [Fact]
    public void Run_LoopCountExceeded_Invisible()
    {
        var ev = new MapEffectEvent(9, 1, 2, 2, 0, 3, 100, 1, false, 0);
        ev.Run(); // -1 → 回卷 0，playCount=1（1 > 1? 否）
        Assert.True(ev.Visible);
        Assert.Equal(0, ev.CurFrame);

        Advance(101);
        ev.Run(); // 0→1
        Advance(101);
        ev.Run(); // 1→2
        Advance(101);
        ev.Run(); // 2→3 ≥ 3 → 回卷 0，playCount=2 > 1 → 隐身
        Assert.False(ev.Visible);
    }

    [Fact]
    public void LoadSurface_EffectListImage()
    {
        var ev = new MapEffectEvent(9, 1, 2, 2, 380, 20, 100, 3, false, 0);
        ev.LoadSurface(); // CurFrame=-1 → 380 + (-1)
        Assert.NotNull(ev.Surface);
        Assert.Equal(ClEventImages.EffectList, ev.Surface!.List);
        Assert.Equal(379, ev.Surface.ImageIndex);

        ev.CurFrame = 2;
        ev.LoadSurface();
        Assert.Equal(382, ev.Surface.ImageIndex);
    }

    [Fact]
    public void Run_FileIndexOutOfBounds_Guard()
    {
        var ev = new MapEffectEvent(9, 1, 2, 5, 0, 3, 100, 3, false, 0); // 5 越界（count=5）
        ev.Run();
        Assert.Equal(-1, ev.CurFrame); // 守卫截停
    }
}

/// <summary>TCustomEffectEvent / TCustomMagicEffectEvent：配置解析与爆燃门。</summary>
public sealed class CustomEffEventTests : IDisposable
{
    private uint _now;

    private void Advance(uint ms) => _now += ms;

    public CustomEffEventTests()
    {
        ClEventEnv.Reset();
        CustomEventEffEnv.Reset();
        ClEventEnv.NowFn = () => _now;
        ClEventEnv.EffectImageCountFn = () => 10;
    }

    public void Dispose()
    {
        ClEventEnv.Reset();
        CustomEventEffEnv.Reset();
    }

    [Fact]
    public void AttackExplosionGate_Satisfied_UsesExplosionGroup()
    {
        CustomEventEffEnv.AttackEffFn = (_, _) => new CustomMonsterAttackEff
        {
            TargetDrawMode = 0, // mdmBlend
            TargetDrawMode2 = 0,
            TargetPlayTime = 60,
            FlyStartIndex = 10,
            FlyPlayCount = 2,
            ExplosionStartIndex = 30,
            ExplosionStartIndex2 = -1,
            ExplosionPlayCount = 5,
            ExplosionKeepPlay = true,
            ExplosionKeepTime = 100,
            ExplosionFile = 1,
            ExplosionKeepLightRange = 3,
            TargetFile = 2,
            TargetStartIndex = 40,
            TargetPlayCount = 6,
            TargetKeepLightRange = 1,
        };
        var ev = new CustomEffectEvent(1, 5, 6, 0, wAppr: 77, attackIndex: 2);
        Assert.True(ev.Blend);
        Assert.Equal(60u, ev.FrameTickTime);
        Assert.Equal(1, ev.FileIndex);
        Assert.Equal(30, ev.ImageIndex);
        Assert.Equal(5, ev.ImageCount);
        Assert.Equal(3, ev.Light);
        Assert.Equal(Grobal2Const.ET_CUSTOM_EFF, ev.EventType);
    }

    [Fact]
    public void AttackExplosionGate_Failed_UsesTargetGroup()
    {
        CustomEventEffEnv.AttackEffFn = (_, _) => new CustomMonsterAttackEff
        {
            FlyStartIndex = 10,
            FlyPlayCount = 0, // 门失败
            ExplosionStartIndex = 30,
            ExplosionPlayCount = 5,
            ExplosionKeepPlay = true,
            ExplosionKeepTime = 100,
            ExplosionFile = 1,
            TargetDrawMode = 1,
            TargetFile = 2,
            TargetStartIndex = 40,
            TargetStartIndex2 = -1,
            TargetPlayCount = 6,
            TargetKeepLightRange = 1,
        };
        var ev = new CustomEffectEvent(1, 5, 6, 0, 77, 2);
        Assert.False(ev.Blend);
        Assert.Equal(2, ev.FileIndex);
        Assert.Equal(40, ev.ImageIndex);
        Assert.Equal(6, ev.ImageCount);
        Assert.Equal(1, ev.Light);
    }

    [Fact]
    public void AttackIndexOutOfRange_NoConfig()
    {
        var ev = new CustomEffectEvent(1, 5, 6, 0, 77, attackIndex: 9);
        Assert.Equal(-2, ev.FileIndex);
        Assert.False(ev.Blend);
    }

    [Fact]
    public void MagicEvent_PlusLevelBuckets()
    {
        int? bucket = null;
        CustomEventEffEnv.MagicEffFn = (_, pl) => { bucket = pl; return new CustomMagicEff { TargetFile = 3, TargetStartIndex = 10, TargetPlayCount = 4 }; };

        new CustomMagicEffectEvent(1, 0, 0, 0, wMagicId: 7, wNewLevel: 0);
        Assert.Equal(0, bucket);
        new CustomMagicEffectEvent(2, 0, 0, 0, 7, 3);
        Assert.Equal(1, bucket);
        new CustomMagicEffectEvent(3, 0, 0, 0, 7, 5);
        Assert.Equal(2, bucket);
        var ev = new CustomMagicEffectEvent(4, 0, 0, 0, 7, 7);
        Assert.Equal(3, bucket);

        // 事件类型 = ET_CUSTOM_EFF + wMagicID（可多次播放）
        Assert.Equal(Grobal2Const.ET_CUSTOM_EFF + 7, ev.EventType);
        Assert.Equal(3, ev.FileIndex);
    }

    [Fact]
    public void MagicEvent_LoadSurface_GuardsAndImage()
    {
        CustomEventEffEnv.MagicEffFn = (_, _) => new CustomMagicEff { TargetFile = 3, TargetStartIndex = 10, TargetPlayCount = 4 };
        var ev = new CustomMagicEffectEvent(1, 0, 0, 0, 7, 0);
        ev.LoadSurface(); // file=3 < count=10 → 取图 10+0
        Assert.Equal(10, ev.Surface!.ImageIndex);
        Assert.Equal(ClEventImages.EffectList, ev.Surface.List);

        CustomEventEffEnv.MagicEffFn = (_, _) => new CustomMagicEff { TargetFile = -1 };
        var bad = new CustomMagicEffectEvent(2, 0, 0, 0, 7, 0);
        bad.LoadSurface();
        Assert.Null(bad.Surface); // FileIndex<0 守卫
    }

    [Fact]
    public void AttackEvent_Run_WrapsFrame()
    {
        CustomEventEffEnv.AttackEffFn = (_, _) => new CustomMonsterAttackEff
        {
            TargetFile = 2,
            TargetStartIndex = 5,
            TargetPlayCount = 3,
            TargetPlayTime = 10,
        };
        var ev = new CustomEffectEvent(1, 0, 0, 0, 77, 1);
        Advance(11);
        ev.Run();
        Assert.Equal(1, ev.CurFrame);
        ev.CurFrame = 2;
        Advance(11);
        ev.Run(); // 2→3 ≥ 3 → 回卷 0
        Assert.Equal(0, ev.CurFrame);
    }
}

/// <summary>TClEventManager：去重登记/删除/查找/批量执行。</summary>
public sealed class ClEventManagerTests
{
    [Fact]
    public void AddEvent_NormalDedup_OnAllFiveFields()
    {
        var mgr = new ClEventManager();
        var e1 = new ClEvent(1, 3, 4, Grobal2Const.ET_FIRE) { EventParam = 0, Dir = 2 };
        mgr.AddEvent(e1);
        Assert.Single(mgr.EventList);

        var e2 = new ClEvent(2, 3, 4, Grobal2Const.ET_FIRE) { EventParam = 0, Dir = 2 };
        mgr.AddEvent(e2);
        Assert.Single(mgr.EventList); // 命中去重 → 新事件弃置（Delphi evn.Free）、旧事件保留
        Assert.True(e2.Discarded);
        Assert.Same(e1, mgr.EventList[0]);
    }

    [Fact]
    public void AddEvent_FireFlower_ExemptFromDedup()
    {
        var mgr = new ClEventManager();
        mgr.AddEvent(new ClEvent(1, 3, 4, Grobal2Const.ET_FIREFLOWER_1) { Dir = 0 });
        var e2 = new ClEvent(2, 3, 4, Grobal2Const.ET_FIREFLOWER_1) { Dir = 0 };
        mgr.AddEvent(e2);
        Assert.Equal(2, mgr.EventList.Count); // 烟花可多次播放 chongchong 2019-03-06
        Assert.False(e2.Discarded);
    }

    [Fact]
    public void AddEvent_MapEffect_FullFieldMatch()
    {
        var mgr = new ClEventManager();
        mgr.AddEvent(new MapEffectEvent(1, 3, 4, 2, 380, 20, 100, 3, false, 0) { Dir = 0 });
        var dup = new MapEffectEvent(2, 3, 4, 2, 380, 20, 100, 3, false, 0) { Dir = 0 };
        mgr.AddEvent(dup);
        Assert.Single(mgr.EventList);
        Assert.True(dup.Discarded);

        var diff = new MapEffectEvent(3, 3, 4, 2, 380, 20, 100, 4, false, 0) { Dir = 0 }; // LoopCount 不同
        mgr.AddEvent(diff);
        Assert.Equal(2, mgr.EventList.Count); // 同坐标不同 wil 资源可共存（HZQ 20230524 修复）
    }

    [Fact]
    public void DelEvent_And_DelEventById()
    {
        var mgr = new ClEventManager();
        var e1 = new ClEvent(1, 0, 0, Grobal2Const.ET_FIRE);
        mgr.AddEvent(e1);
        mgr.DelEvent(e1);
        Assert.False(e1.Visible);

        var e2 = new ClEvent(2, 1, 1, Grobal2Const.ET_LAVA);
        mgr.AddEvent(e2);
        mgr.DelEventById(2);
        Assert.False(e2.Visible);
    }

    [Fact]
    public void GetEvent_ByXyType()
    {
        var mgr = new ClEventManager();
        var e1 = new ClEvent(1, 3, 4, Grobal2Const.ET_FIRE);
        mgr.AddEvent(e1);
        Assert.Same(e1, mgr.GetEvent(3, 4, Grobal2Const.ET_FIRE));
        Assert.Null(mgr.GetEvent(3, 4, Grobal2Const.ET_LAVA));
        Assert.Null(mgr.GetEvent(9, 9, Grobal2Const.ET_FIRE));
    }

    [Fact]
    public void Execute_RunsAll()
    {
        uint now = 0;
        ClEventEnv.NowFn = () => now;
        try
        {
            var mgr = new ClEventManager();
            var e1 = new ClEvent(1, 0, 0, Grobal2Const.ET_DIGOUTZOMBI);
            mgr.AddEvent(e1);
            mgr.Execute(); // 同拍：0-0 < 2s → 不排队
            Assert.Equal(0, e1.LoadSurfaceRequests);

            now = 2001;
            mgr.Execute(); // ≥2s → 排队重载
            Assert.Equal(1, e1.LoadSurfaceRequests);
        }
        finally
        {
            ClEventEnv.Reset();
        }
    }

    [Fact]
    public void ClearEvents_GhostTickAndFreeList()
    {
        var mgr = new ClEventManager();
        var freed = new List<ClEvent>();
        mgr.AddFreeEvent = e => freed.Add(e);
        var e1 = new ClEvent(1, 0, 0, Grobal2Const.ET_FIRE);
        mgr.AddEvent(e1);
        mgr.ClearEvents(999);
        Assert.Empty(mgr.EventList);
        Assert.Equal(999u, e1.GhostTick);
        Assert.Same(e1, Assert.Single(freed));
    }
}

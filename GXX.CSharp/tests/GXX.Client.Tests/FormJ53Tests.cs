using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>Actor.pas 绘制调度族（批次J53）：DrawEffSurface/StretchDraw/状态特效层/武器微光/施法层/技能音效。</summary>
public sealed class ActorDrawDispatchTests : IDisposable
{
    public ActorDrawDispatchTests() => ActorDrawEnv.Reset();

    public void Dispose() => ActorDrawEnv.Reset();

    // ---- DrawEffSurface（5704）----

    [Fact]
    public void DrawEffSurface_NormalColors()
    {
        var none = ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceNone, blend: false, 10, 20);
        Assert.Equal(SurfaceDrawKind.Draw, none.Kind);
        Assert.Equal(10, none.X);
        Assert.Equal(20, none.Y);

        // ceGreen 非混合 → clLime（clGreen→clLime piaoyun 2013-06-28）
        var green = ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceGreen, false, 0, 0);
        Assert.Equal(SurfaceDrawKind.DrawColor, green.Kind);
        Assert.Equal(DlColors.Lime, green.Color);

        Assert.Equal(DlColors.Black, ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceBlack, false, 0, 0).Color);
        Assert.Equal(DlColors.White, ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceWhite, false, 0, 0).Color);
        Assert.Equal(DlColors.Red, ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceRed, false, 0, 0).Color);
        Assert.Equal(DlColors.Blue, ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceBlue, false, 0, 0).Color);
        Assert.Equal(DlColors.Yellow, ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceYellow, false, 0, 0).Color);
        Assert.Equal(DlColors.Fuchsia, ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceFuchsia, false, 0, 0).Color);
        Assert.Equal(DlColors.Aqua, ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceAqua, false, 0, 0).Color);
        Assert.Equal(DlColors.Silver, ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceSilver, false, 0, 0).Color);
        Assert.Equal(DlColors.Gray, ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceGray, false, 0, 0).Color);
    }

    [Fact]
    public void DrawEffSurface_GrayAndBright_DrawPlain()
    {
        foreach (var ceff in new[] { TColorEffect.ceGrayScale, TColorEffect.ceGrayScale2, TColorEffect.ceBright })
        {
            var op = ActorDrawDispatch.DrawEffSurface(0, 0, ceff, false, 0, 0);
            Assert.Equal(SurfaceDrawKind.Draw, op.Kind);
            Assert.Equal(0, op.Color);
        }
    }

    [Fact]
    public void DrawEffSurface_Blend_Alpha150AndColorTable()
    {
        var none = ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceNone, true, 0, 0);
        Assert.Equal(SurfaceDrawKind.DrawColorAlpha, none.Kind);
        Assert.Equal(DlColors.White, none.Color);
        Assert.Equal(150, none.Alpha);

        // ceGreen 混合 → clGreen（非混合是 clLime，原版差异保留）
        var green = ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceGreen, true, 0, 0);
        Assert.Equal(DlColors.Green, green.Color);
        Assert.Equal(150, green.Alpha);

        Assert.Equal(DlColors.Black, ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceBlack, true, 0, 0).Color);
        Assert.Equal(DlColors.Gray, ActorDrawDispatch.DrawEffSurface(0, 0, TColorEffect.ceGray, true, 0, 0).Color);
    }

    [Fact]
    public void DrawEffSurface_StateMask_ForcesBlend()
    {
        var op = ActorDrawDispatch.DrawEffSurface(0x00800000, 0, TColorEffect.ceNone, blend: false, 0, 0);
        Assert.Equal(SurfaceDrawKind.DrawColorAlpha, op.Kind); // 非 blend 入参但被状态位强制
        Assert.Equal(150, op.Alpha);
    }

    [Fact]
    public void DrawEffSurface_BodyColor_UsesPaletteSeam()
    {
        ActorDrawEnv.GetRgbFn = c => c * 2;
        var plain = ActorDrawDispatch.DrawEffSurface(0, 7, TColorEffect.ceNone, false, 0, 0);
        Assert.Equal(SurfaceDrawKind.DrawColor, plain.Kind);
        Assert.Equal(14, plain.Color); // GetRGB(7)

        var blended = ActorDrawDispatch.DrawEffSurface(0, 7, TColorEffect.ceNone, true, 0, 0);
        Assert.Equal(SurfaceDrawKind.DrawColorAlpha, blended.Kind);
        Assert.Equal(14, blended.Color);
        Assert.Equal(150, blended.Alpha);
    }

    // ---- StretchDrawEffSurface（5797）----

    [Fact]
    public void StretchDraw_BoundsAndOrigin()
    {
        var op = ActorDrawDispatch.StretchDrawEffSurface(0, TColorEffect.ceNone, 100, 200, srcWidth: 20, srcHeight: 10, phantomAlpha: 200);
        Assert.Equal(SurfaceDrawKind.StretchDraw, op.Kind);
        Assert.Equal(88, op.X);  // 100 - 12
        Assert.Equal(168, op.Y); // 200 - UNITY(32)
        Assert.Equal(30, op.DestWidth);   // 20×1.5
        Assert.Equal(15, op.DestHeight);  // 10×1.5
        Assert.Equal(200, op.Alpha);      // 幻影 alpha
        Assert.Equal(0, op.Color);        // ceNone 无色参
    }

    [Fact]
    public void StretchDraw_BankerRounding()
    {
        // Delphi Round 银行家舍入：43×1.5=64.5 → 64
        var op = ActorDrawDispatch.StretchDrawEffSurface(0, TColorEffect.ceNone, 0, 0, 43, 33, 255);
        Assert.Equal(64, op.DestWidth);  // 64.5 → 64（偶）
        Assert.Equal(50, op.DestHeight); // 49.5 → 50（偶）
    }

    [Fact]
    public void StretchDraw_ColorsAndBodyColor()
    {
        var green = ActorDrawDispatch.StretchDrawEffSurface(0, TColorEffect.ceGreen, 0, 0, 10, 10, 255);
        Assert.Equal(DlColors.Green, green.Color); // 此处无 clLime 替换

        ActorDrawEnv.GetRgbFn = c => c + 100;
        var body = ActorDrawDispatch.StretchDrawEffSurface(9, TColorEffect.ceNone, 0, 0, 10, 10, 220);
        Assert.Equal(109, body.Color);
        Assert.Equal(220, body.Alpha);
    }

    // ---- DrawStateEffSurface（5654）----

    [Fact]
    public void StateFx_Cobweb_AdvanceWrapAndGates()
    {
        var st = new StateFxState();
        uint now = 0;

        var (web, _, _) = ActorDrawDispatch.DrawStateEffSurface(st, true, false, false, false, false, false, now, 100, 50,
            (_, _) => (30, 5));
        Assert.NotNull(web);
        Assert.Equal(320, web!.ImageIndex); // 帧索引 0
        Assert.Equal(100 - 15, web.X);      // sayX − 30 div 2
        Assert.Equal(50 + 5 - 20, web.Y);   // ddy + pY − 20

        // 100ms 内不推进
        now += 100;
        ActorDrawDispatch.DrawStateEffSurface(st, true, false, false, false, false, false, now, 100, 50, (_, _) => (30, 5));
        Assert.Equal(0, st.CobwebIndex);
        now += 1;
        ActorDrawDispatch.DrawStateEffSurface(st, true, false, false, false, false, false, now, 100, 50, (_, _) => (30, 5));
        Assert.Equal(1, st.CobwebIndex);

        // 推进到 10 → 回卷 0
        st.CobwebIndex = 10;
        ActorDrawDispatch.DrawStateEffSurface(st, true, false, false, false, false, false, now, 100, 50, (_, _) => (30, 5));
        Assert.Equal(0, st.CobwebIndex);

        // 断筋 / 隐身 / 死亡 → 不绘
        var (g1, _, _) = ActorDrawDispatch.DrawStateEffSurface(st, true, duanJin: true, false, false, false, false, now, 100, 50, (_, _) => (30, 5));
        Assert.Null(g1);
        var (g2, _, _) = ActorDrawDispatch.DrawStateEffSurface(st, true, false, false, false, ghost: true, false, now, 100, 50, (_, _) => (30, 5));
        Assert.Null(g2);
        var (g3, _, _) = ActorDrawDispatch.DrawStateEffSurface(st, true, false, false, false, false, death: true, now, 100, 50, (_, _) => (30, 5));
        Assert.Null(g3);
    }

    [Fact]
    public void StateFx_ToxicSmoke_80ms()
    {
        var st = new StateFxState();
        var (_, smoke, _) = ActorDrawDispatch.DrawStateEffSurface(st, false, false, true, false, false, false, 80, 60, 40,
            (lib, _) => { Assert.Equal(ActorDrawDispatch.CboEffectLib, lib); return (20, 7); });
        Assert.NotNull(smoke);
        Assert.Equal(4010, smoke!.ImageIndex);
        Assert.Equal(60 - 10, smoke.X);
        Assert.Equal(40 + 7, smoke.Y); // 无 −20
        Assert.Equal(0u, st.ToxicTick); // 80-0=80 不大于 80（严格大于）→ 不推进

        ActorDrawDispatch.DrawStateEffSurface(st, false, false, true, false, false, false, 81, 60, 40, (_, _) => (20, 7));
        Assert.Equal(81u, st.ToxicTick); // 81 > 80 → 推进
        Assert.Equal(1, st.ToxicIndex);
    }

    [Fact]
    public void StateFx_Frozen_WrapAt4()
    {
        var st = new StateFxState { FrozenIndex = 4 };
        var (_, _, frozen) = ActorDrawDispatch.DrawStateEffSurface(st, false, false, false, true, false, false, 1000, 0, 0,
            (_, _) => (10, 0));
        Assert.Equal(0, st.FrozenIndex); // >3 回卷
        Assert.NotNull(frozen);
        Assert.Equal(330, frozen!.ImageIndex);

        st.FrozenIndex = 3;
        ActorDrawDispatch.DrawStateEffSurface(st, false, false, false, true, false, false, 1000, 0, 0, (_, _) => (10, 0));
        Assert.Equal(3, st.FrozenIndex); // ≤3 不回卷
    }

    // ---- DrawWeaponGlimmer（5892）----

    [Fact]
    public void WeaponGlimmer_GateAndBlendSplit()
    {
        var op = ActorDrawDispatch.WeaponGlimmer(false, false, noBlend: false, 5, 6, 1, 2, 100, 200, "WpnFx", 7);
        Assert.Equal(SurfaceDrawKind.DrawBlend, op!.Kind);
        Assert.Equal(100 + 5 + 1, op.X);
        Assert.Equal(200 + 6 + 2, op.Y);
        Assert.Equal("WpnFx", op.Lib);

        var plain = ActorDrawDispatch.WeaponGlimmer(false, false, noBlend: true, 0, 0, 0, 0, 0, 0, "W", 0);
        Assert.Equal(SurfaceDrawKind.Draw, plain!.Kind);

        // ckHideWeaponEffect 与 PlugInEnabled 同开 → 不绘
        Assert.Null(ActorDrawDispatch.WeaponGlimmer(true, true, false, 0, 0, 0, 0, 0, 0, "W", 0));
        Assert.NotNull(ActorDrawDispatch.WeaponGlimmer(true, false, false, 0, 0, 0, 0, 0, 0, "W", 0));
        Assert.NotNull(ActorDrawDispatch.WeaponGlimmer(false, true, false, 0, 0, 0, 0, 0, 0, "W", 0));
    }

    // ---- DrawChr 施法特效层（6101）----

    [Fact]
    public void SpellEffect_FrameWindowAndBase()
    {
        var op = ActorDrawDispatch.SpellEffect(useMagic: true, effectNumber: 29, curEffFrame: 2, spellFrame: 6, newLevel: 3,
            selfDead: false, dx: 500, dy: 600, shiftX: 4, shiftY: -8,
            (mag, mtype, level) => { Assert.Equal(28, mag); Assert.Equal(0, mtype); Assert.Equal(3, level); return new("WMagic6Images", 690, 10, 20); });
        Assert.NotNull(op);
        Assert.Equal(SurfaceDrawKind.DrawBlend, op!.Kind);
        Assert.Equal(690 + 2, op.ImageIndex); // base + curEffFrame
        Assert.Equal(500 + 10 + 4, op.X);
        Assert.Equal(600 + 20 - 8, op.Y);
        Assert.False(op.Gray);
    }

    [Fact]
    public void SpellEffect_Gates()
    {
        var getBase = (int _, int _, int _) => new ActorDrawDispatch.EffectBaseRef("L", 0, 0, 0);

        Assert.Null(ActorDrawDispatch.SpellEffect(false, 29, 0, 6, 0, false, 0, 0, 0, 0, getBase));   // 未施法
        Assert.Null(ActorDrawDispatch.SpellEffect(true, 0, 0, 6, 0, false, 0, 0, 0, 0, getBase));     // EffectNumber=0
        Assert.Null(ActorDrawDispatch.SpellEffect(true, 29, 6, 6, 0, false, 0, 0, 0, 0, getBase));    // 帧 ≥ spellFrame
        Assert.Null(ActorDrawDispatch.SpellEffect(true, 29, -1, 6, 0, false, 0, 0, 0, 0, getBase));   // 帧 < 0
        Assert.Null(ActorDrawDispatch.SpellEffect(true, 29, 0, 6, 0, false, 0, 0, 0, 0,
            (_, _, _) => null));                                                                       // wimg=nil

        var dead = ActorDrawDispatch.SpellEffect(true, 29, 0, 6, 0, selfDead: true, 0, 0, 0, 0, getBase);
        Assert.True(dead!.Gray); // 观察者死亡 → 灰度
    }

    // ---- SetMagicSound（6167）----

    [Fact]
    public void MagicSound_FormulaDefault()
    {
        var ids = ActorDrawDispatch.SetMagicSound(7);
        Assert.Equal(10070, ids!.Start);
        Assert.Equal(10071, ids.Fire);
        Assert.Equal(10072, ids.Explosion);
        Assert.Null(ActorDrawDispatch.SetMagicSound(0));
    }

    [Fact]
    public void MagicSound_Corrections()
    {
        Assert.Equal(-1, ActorDrawDispatch.SetMagicSound(11)!.Start); // 雷电术去起手

        var m62 = ActorDrawDispatch.SetMagicSound(62)!;
        Assert.Equal((10520, 10521, 10522), (m62.Start, m62.Fire, m62.Explosion));

        var m57 = ActorDrawDispatch.SetMagicSound(57)!; // 噬血术 → 48 声
        Assert.Equal(10480, m57.Start);
        var m48 = ActorDrawDispatch.SetMagicSound(48)!; // 气功波 → 37 声
        Assert.Equal(10370, m48.Start);
        var m50 = ActorDrawDispatch.SetMagicSound(50)!;
        Assert.Equal(10360, m50.Start);
        var m51 = ActorDrawDispatch.SetMagicSound(51)!;
        Assert.Equal(10060, m51.Start);
        var m52 = ActorDrawDispatch.SetMagicSound(52)!;
        Assert.Equal(10470, m52.Start);
        var m71 = ActorDrawDispatch.SetMagicSound(71)!;
        Assert.Equal(10280, m71.Start);
        var m72 = ActorDrawDispatch.SetMagicSound(72)!;
        Assert.Equal(10210, m72.Start);
        var m73 = ActorDrawDispatch.SetMagicSound(73)!;
        Assert.Equal(10310, m73.Start);
        var m76 = ActorDrawDispatch.SetMagicSound(76)!;
        Assert.Equal(10300, m76.Start);

        // 73/87/88/89 同值
        var m87 = ActorDrawDispatch.SetMagicSound(87)!;
        var m88 = ActorDrawDispatch.SetMagicSound(88)!;
        var m89 = ActorDrawDispatch.SetMagicSound(89)!;
        Assert.Equal((m73.Start, m73.Fire, m73.Explosion), (m87.Start, m87.Fire, m87.Explosion));
        Assert.Equal((m73.Start, m73.Fire, m73.Explosion), (m88.Start, m88.Fire, m88.Explosion));
        Assert.Equal((m73.Start, m73.Fire, m73.Explosion), (m89.Start, m89.Fire, m89.Explosion));

        // 38/46：fire 不覆盖（保留公式值）
        var m38 = ActorDrawDispatch.SetMagicSound(38)!;
        Assert.Equal(10520, m38.Start);
        Assert.Equal(10000 + 38 * 10 + 1, m38.Fire);
        Assert.Equal(10522, m38.Explosion);
        var m46 = ActorDrawDispatch.SetMagicSound(46)!;
        Assert.Equal(10000 + 46 * 10 + 1, m46.Fire);

        // 116/117：fire 同样保留
        var m116 = ActorDrawDispatch.SetMagicSound(116)!;
        Assert.Equal((11036, 10000 + 116 * 10 + 1, 11037), (m116.Start, m116.Fire, m116.Explosion));
        var m117 = ActorDrawDispatch.SetMagicSound(117)!;
        Assert.Equal(11040, m117.Start);
        Assert.Equal(11041, m117.Explosion);

        // 199/200：fire=0
        var m199 = ActorDrawDispatch.SetMagicSound(199)!;
        Assert.Equal((11000, 0, 11002), (m199.Start, m199.Fire, m199.Explosion));
        var m200 = ActorDrawDispatch.SetMagicSound(200)!;
        Assert.Equal((11010, 0, 11012), (m200.Start, m200.Fire, m200.Explosion));

        var m201 = ActorDrawDispatch.SetMagicSound(201)!;
        Assert.Equal((10330, 10331, 10430), (m201.Start, m201.Fire, m201.Explosion));
        var m202 = ActorDrawDispatch.SetMagicSound(202)!;
        Assert.Equal((10130, 10131, 10132), (m202.Start, m202.Fire, m202.Explosion));
        var m203 = ActorDrawDispatch.SetMagicSound(203)!;
        Assert.Equal((10490, 10491, 10492), (m203.Start, m203.Fire, m203.Explosion));
        var m204 = ActorDrawDispatch.SetMagicSound(204)!;
        Assert.Equal((10461, 0, 10522), (m204.Start, m204.Fire, m204.Explosion));
        var m208 = ActorDrawDispatch.SetMagicSound(208)!;
        Assert.Equal((11060, 0, 0), (m208.Start, m208.Fire, m208.Explosion));
        var m34 = ActorDrawDispatch.SetMagicSound(34)!;
        Assert.Equal((10020, 0, 10492), (m34.Start, m34.Fire, m34.Explosion));
        var m41 = ActorDrawDispatch.SetMagicSound(41)!;
        Assert.Equal((10430, 0, 0), (m41.Start, m41.Fire, m41.Explosion));
    }

    [Fact]
    public void MagicSound_NamedSounds_ViaSeam()
    {
        ActorDrawEnv.NamedSoundFn = name => name switch
        {
            "s_hit_Lxhy_0" => 9000,
            "s_hit_Lxhy_3" => 9003,
            "s_cboFs1_start" => 9100,
            "s_cboFs1_target" => 9101,
            "s_xsls_death" => 9200,
            "s_xsws_pbec" => 9201,
            "s_xf" => 9202,
            _ => -1,
        };

        var m58 = ActorDrawDispatch.SetMagicSound(58)!; // 流星火雨
        Assert.Equal((9000, 9000, 9003), (m58.Start, m58.Fire, m58.Explosion));

        var m107 = ActorDrawDispatch.SetMagicSound(107)!; // 双龙破（fire 保留公式值）
        Assert.Equal((9100, 10000 + 107 * 10 + 1, 9101), (m107.Start, m107.Fire, m107.Explosion));

        var m114 = ActorDrawDispatch.SetMagicSound(114)!;
        Assert.Equal((9200, 10000 + 114 * 10 + 1, 9201), (m114.Start, m114.Fire, m114.Explosion));

        var m205 = ActorDrawDispatch.SetMagicSound(205)!; // fire=0
        Assert.Equal((9000, 0, 9003), (m205.Start, m205.Fire, m205.Explosion));

        var m206 = ActorDrawDispatch.SetMagicSound(206)!;
        Assert.Equal((9202, 0, 9201), (m206.Start, m206.Fire, m206.Explosion));
    }

    [Fact]
    public void MagicSound_AllNamedBranches_ResolveThroughSeam()
    {
        var seen = new List<string>();
        ActorDrawEnv.NamedSoundFn = name => { seen.Add(name); return 1; };

        foreach (var id in new[] { 104, 105, 106, 108, 109, 110, 111 })
            ActorDrawDispatch.SetMagicSound(id);
        Assert.Equal(14, seen.Count); // 每个 2 个命名音效
        Assert.Contains("s_cboFs2_start", seen);
        Assert.Contains("s_cboDs4_target", seen);
    }
}

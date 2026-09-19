using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J79：PlayScn.pas TPlayScene.DrawActorLabel（3581-3938）血条绘制族 1:1 测试。
/// 覆盖总开关、种族偏移、四类角色的 boShowHPLabel 修正、护身蓝条、NPC/自定义怪分支、守卫分支、内功黄条。
/// </summary>
public sealed class ActorHpBarTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public ActorHpBarTests()
    {
        var savedNewop = (LabelSurface?)null;
        for (int i = 0; i < 8; i++)
            savedNewop = TPlaySceneCore.NewopUI170TextureArray[i];

        var arr = new LabelSurface?[8];
        Array.Copy(TPlaySceneCore.NewopUI170TextureArray, arr, 8);

        var saved = (TPlaySceneCore.HpBarImages, TPlaySceneCore.EffectImageListCountFn,
            TPlaySceneCore.GetCustomNpcConfigFn, TPlaySceneCore.HpBarTickFn,
            TPlaySceneCore.AppExit, TPlaySceneCore.PlugInEnabled,
            TPlaySceneCore.nHumHPBarOffsetX, TPlaySceneCore.nNpcHPBarOffsetX, TPlaySceneCore.nMonHPBarOffsetX,
            TPlaySceneCore.boShowHPLabel, TPlaySceneCore.ckShowHPLabel,
            TPlaySceneCore.boHumStruckShowNumber, TPlaySceneCore.boMonStruckShowNumber,
            TPlaySceneCore.boPetNoShowHPProgress, TPlaySceneCore.boShowMagicShieldHP,
            TPlaySceneCore.boShowHighlightHPLabel, TPlaySceneCore.ckShowHighlightHPLabel,
            TPlaySceneCore.boShowNGLabel, TPlaySceneCore.ckShowNGLabel, TPlaySceneCore.ckShowNpcHPLabel);

        _restore.Add(() =>
        {
            Array.Copy(arr, TPlaySceneCore.NewopUI170TextureArray, 8);
            TPlaySceneCore.HpBarImages = saved.Item1;
            TPlaySceneCore.EffectImageListCountFn = saved.Item2;
            TPlaySceneCore.GetCustomNpcConfigFn = saved.Item3;
            TPlaySceneCore.HpBarTickFn = saved.Item4;
            TPlaySceneCore.AppExit = saved.Item5;
            TPlaySceneCore.PlugInEnabled = saved.Item6;
            TPlaySceneCore.nHumHPBarOffsetX = saved.Item7;
            TPlaySceneCore.nNpcHPBarOffsetX = saved.Item8;
            TPlaySceneCore.nMonHPBarOffsetX = saved.Item9;
            TPlaySceneCore.boShowHPLabel = saved.Item10;
            TPlaySceneCore.ckShowHPLabel = saved.Item11;
            TPlaySceneCore.boHumStruckShowNumber = saved.Item12;
            TPlaySceneCore.boMonStruckShowNumber = saved.Item13;
            TPlaySceneCore.boPetNoShowHPProgress = saved.Item14;
            TPlaySceneCore.boShowMagicShieldHP = saved.Item15;
            TPlaySceneCore.boShowHighlightHPLabel = saved.Item16;
            TPlaySceneCore.ckShowHighlightHPLabel = saved.Item17;
            TPlaySceneCore.boShowNGLabel = saved.Item18;
            TPlaySceneCore.ckShowNGLabel = saved.Item19;
            TPlaySceneCore.ckShowNpcHPLabel = saved.Item20;
        });

        for (int i = 0; i < 8; i++)
            TPlaySceneCore.NewopUI170TextureArray[i] = null;
        TPlaySceneCore.HpBarImages = null;
        TPlaySceneCore.EffectImageListCountFn = () => 0;
        TPlaySceneCore.GetCustomNpcConfigFn = _ => null;
        TPlaySceneCore.HpBarTickFn = () => 1000;
        TPlaySceneCore.AppExit = false;
        TPlaySceneCore.PlugInEnabled = true;
        TPlaySceneCore.nHumHPBarOffsetX = 0; TPlaySceneCore.nHumHPBarOffsetY = 0;
        TPlaySceneCore.nNpcHPBarOffsetX = 0; TPlaySceneCore.nNpcHPBarOffsetY = 0;
        TPlaySceneCore.nMonHPBarOffsetX = 0; TPlaySceneCore.nMonHPBarOffsetY = 0;
        TPlaySceneCore.boShowHPLabel = false; TPlaySceneCore.ckShowHPLabel = false;
        TPlaySceneCore.boHumStruckShowNumber = false;
        TPlaySceneCore.boMonStruckShowNumber = false;
        TPlaySceneCore.boPetNoShowHPProgress = false;
        TPlaySceneCore.boShowMagicShieldHP = false;
        TPlaySceneCore.boShowHighlightHPLabel = false;
        TPlaySceneCore.ckShowHighlightHPLabel = false;
        TPlaySceneCore.boShowNGLabel = false;
        TPlaySceneCore.ckShowNGLabel = false;
        TPlaySceneCore.ckShowNpcHPLabel = false;
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    private sealed class FakeImages : IHpBarImageSource
    {
        public readonly Dictionary<(int, int), LabelSurface?> Effect = new();
        public readonly Dictionary<int, LabelSurface?> Mon = new();

        public LabelSurface? GetEffectImage(int fileIndex, int imageIndex)
            => Effect.TryGetValue((fileIndex, imageIndex), out var v) ? v : null;

        public LabelSurface? GetMonImage(int appearance)
            => Mon.TryGetValue(appearance, out var v) ? v : null;

        public LabelSurface? GetMonImagePair(int appearance, int offset)
            => Mon.TryGetValue(appearance + offset, out var v) ? v : null;
    }

    private static TActor MakeActor(int race = ActorLabelConsts.RC_PLAYOBJECT)
    {
        var a = new TActor { m_btRace = (byte)race };
        a.m_boCanDraw = true;
        a.m_nSayX = 100;
        a.m_nSayY = 200;
        a.m_Abil.HP = 50;
        a.m_Abil.MaxHP = 100;
        a.m_Abil.MP = 30;
        a.m_Abil.MaxMP = 60;
        a.m_Abil.Level = 40;
        return a;
    }

    private static TPlaySceneCore MakeScene() => new();

    // ===================== 前置守卫 =====================

    [Fact]
    public void PlanActorHpBar_DeathDrawableStallAllSkip()
    {
        TPlaySceneCore.boShowHPLabel = true;
        TPlaySceneCore.ckShowHPLabel = true;

        var scene = MakeScene();
        var dead = MakeActor(); dead.m_boDeath = true;
        Assert.Empty(scene.PlanActorHpBar(dead));

        var hidden = MakeActor(); hidden.m_boCanDraw = false;
        Assert.Empty(scene.PlanActorHpBar(hidden));

        var stall = MakeActor(); stall.m_boShopStall = true;
        Assert.Empty(scene.PlanActorHpBar(stall));
    }

    [Fact]
    public void PlanActorHpBar_NoSwitchNoOpenHealthDrawsNothing()
    {
        var scene = MakeScene();
        var a = MakeActor();

        Assert.Empty(scene.PlanActorHpBar(a));
    }

    [Fact]
    public void PlanActorHpBar_OpenHealthFlagBypassesGlobalSwitch()
    {
        var a = MakeActor();
        a.m_boOpenHealth = true;

        var ops = MakeScene().PlanActorHpBar(a);

        Assert.NotEmpty(ops);
    }

    [Fact]
    public void PlanActorHpBar_PlugInDisabledBlocksGlobalSwitchButNotOpenHealth()
    {
        TPlaySceneCore.PlugInEnabled = false;
        TPlaySceneCore.boShowHPLabel = true;
        TPlaySceneCore.ckShowHPLabel = true;

        var scene = MakeScene();
        Assert.Empty(scene.PlanActorHpBar(MakeActor()));

        var a = MakeActor();
        a.m_boOpenHealth = true;
        Assert.NotEmpty(scene.PlanActorHpBar(a));
    }

    [Fact]
    public void PlanActorHpBar_NoInstanceOpenHealthExpiresAndStopsDrawing()
    {
        var a = MakeActor();
        a.m_noInstanceOpenHealth = true;
        a.m_dwOpenHealthStart = 100;
        a.m_dwOpenHealthTime = 50;
        TPlaySceneCore.HpBarTickFn = () => 200;      // 200-100 > 50 → 关掉

        var ops = MakeScene().PlanActorHpBar(a);

        // 3627-3629 先关标志，随后 3642-3643 的总开关因标志已失效而不成立
        Assert.False(a.m_noInstanceOpenHealth);
        Assert.Empty(ops);
    }

    [Fact]
    public void PlanActorHpBar_NoInstanceOpenHealthNotExpiredStaysOn()
    {
        var a = MakeActor();
        a.m_noInstanceOpenHealth = true;
        a.m_dwOpenHealthStart = 100;
        a.m_dwOpenHealthTime = 500;
        TPlaySceneCore.HpBarTickFn = () => 200;

        MakeScene().PlanActorHpBar(a);

        Assert.True(a.m_noInstanceOpenHealth);
    }

    // ===================== 贴图矩形与偏移 =====================

    [Fact]
    public void PlanActorHpBar_NoTextureUsesFallback32x3Rect()
    {
        var a = MakeActor();
        a.m_boOpenHealth = true;

        var ops = MakeScene().PlanActorHpBar(a);

        // rc = 32x3 → HPOffsetX = 16, HPOffsetY = 3*2+4 = 10
        // nX = 100 - 16 = 84, nY = 200 - 10 = 190
        var bg = ops[0];
        Assert.Equal(84, bg.X);
        Assert.Equal(190, bg.Y);
    }

    [Fact]
    public void PlanActorHpBar_TextureSizeDrivesRectAndOffsets()
    {
        TPlaySceneCore.NewopUI170TextureArray[HpBarTextureIndex.Background] = LabelSurface.Of(40, 5);

        var a = MakeActor();
        a.m_boOpenHealth = true;

        var ops = MakeScene().PlanActorHpBar(a);

        // HPOffsetX = 20, HPOffsetY = 5*2+4 = 14
        Assert.Equal(80, ops[0].X);
        Assert.Equal(186, ops[0].Y);
    }

    [Fact]
    public void PlanActorHpBar_RaceOffsetsSelectCorrectPair()
    {
        TPlaySceneCore.nHumHPBarOffsetX = 1; TPlaySceneCore.nHumHPBarOffsetY = 2;
        TPlaySceneCore.nNpcHPBarOffsetX = 3; TPlaySceneCore.nNpcHPBarOffsetY = 4;
        TPlaySceneCore.nMonHPBarOffsetX = 5; TPlaySceneCore.nMonHPBarOffsetY = 6;

        var scene = MakeScene();

        var hum = MakeActor(ActorLabelConsts.RC_PLAYOBJECT);
        hum.m_boOpenHealth = true;
        var humOps = scene.PlanActorHpBar(hum);
        Assert.Equal(84 + 1, humOps[0].X);
        Assert.Equal(190 + 2, humOps[0].Y);

        // 商人需 ckShowNpcHPLabel 才绘制（逗号后的表达式低于 && 优先级，已按原文括号化）
        TPlaySceneCore.ckShowNpcHPLabel = true;
        var npc = MakeActor(ActorLabelConsts.RC_MERCHANT);
        npc.m_boOpenHealth = true;
        var npcOps = scene.PlanActorHpBar(npc);
        Assert.Equal(84 + 3, npcOps[0].X);
        Assert.Equal(190 + 4, npcOps[0].Y);

        var mon = MakeActor(80);
        mon.m_boOpenHealth = true;
        var monOps = scene.PlanActorHpBar(mon);
        Assert.Equal(84 + 5, monOps[0].X);
        Assert.Equal(190 + 6, monOps[0].Y);
    }

    // ===================== HP 比例收缩 =====================

    [Fact]
    public void PlanActorHpBar_HpRatioShrinksRightEdge()
    {
        var a = MakeActor();
        a.m_boOpenHealth = true;
        a.m_Abil.HP = 50;
        a.m_Abil.MaxHP = 100;

        var ops = MakeScene().PlanActorHpBar(a);

        // 人物分支：rc.Right = 0 + round(32/100*50) = 16
        var hp = ops.Find(o => o.Kind == "HumHP");
        Assert.NotNull(hp);
        Assert.Equal(0, hp!.SrcLeft);
        Assert.Equal(16, hp.SrcRight);
    }

    [Fact]
    public void PlanActorHpBar_MaxHpLessThanHpClampsOnlyLocalCopy()
    {
        var a = MakeActor();
        a.m_boOpenHealth = true;
        a.m_Abil.HP = 200;
        a.m_Abil.MaxHP = 100;

        var ops = MakeScene().PlanActorHpBar(a);

        // 3645 的 Abil 是值拷贝 → 夹紧不写回 actor.m_Abil
        Assert.Equal(100u, a.m_Abil.MaxHP);
        // 但夹紧确实生效于局部副本：HP == MaxHP(200) → 不收缩，满宽
        Assert.Equal(32, ops.Find(o => o.Kind == "HumHP")!.SrcRight);
    }

    [Fact]
    public void PlanActorHpBar_MaxMpLessThanMpClampsOnlyLocalCopy()
    {
        TPlaySceneCore.boShowMagicShieldHP = true;

        var a = MakeActor();
        a.m_boOpenHealth = true;
        a.m_boMagicShield = true;
        a.m_btJob = 1;
        a.m_Abil.Level = 40;
        a.m_Abil.MP = 90;
        a.m_Abil.MaxMP = 10;

        var ops = MakeScene().PlanActorHpBar(a);

        Assert.Equal(10u, a.m_Abil.MaxMP);           // 不写回
        // 局部副本 MaxMP := 90 → MP == MaxMP → 蓝条满宽 32
        Assert.Equal(32, ops.Find(o => o.Kind == "ShieldMP")!.SrcRight);
    }

    [Fact]
    public void PlanActorHpBar_FullHpKeepsFullWidth()
    {
        var a = MakeActor();
        a.m_boOpenHealth = true;
        a.m_Abil.HP = 100;
        a.m_Abil.MaxHP = 100;

        var ops = MakeScene().PlanActorHpBar(a);
        // HP == MaxHP → 不收缩
        Assert.Equal(32, ops.Find(o => o.Kind == "HumHP")!.SrcRight);
    }

    [Fact]
    public void PlanActorHpBar_HumWithoutStruckFlagUsesRealHpViaOpenHealth()
    {
        // boHumStruckShowNumber = true 且 m_boStruckShowNumber = false，
        // 但条件里还有 `or Actor.m_boOpenHealth` → 仍为真 → 还原真实 HP
        TPlaySceneCore.boHumStruckShowNumber = true;

        var a = MakeActor();
        a.m_boOpenHealth = true;
        a.m_boStruckShowNumber = false;
        a.m_Abil.HP = 50;
        a.m_Abil.MaxHP = 100;

        var ops = MakeScene().PlanActorHpBar(a);
        Assert.Equal(16, ops.Find(o => o.Kind == "HumHP")!.SrcRight);
    }

    [Fact]
    public void PlanActorHpBar_HumStruckFlagOffGloballyKeepsRealHp()
    {
        // boHumStruckShowNumber = false → `not boHumStruckShowNumber` 为真 → 还原真实 HP
        TPlaySceneCore.boHumStruckShowNumber = false;

        var a = MakeActor();
        a.m_boOpenHealth = true;
        a.m_boStruckShowNumber = false;
        a.m_Abil.HP = 50;
        a.m_Abil.MaxHP = 100;

        var ops = MakeScene().PlanActorHpBar(a);
        Assert.Equal(16, ops.Find(o => o.Kind == "HumHP")!.SrcRight);
    }

    [Fact]
    public void PlanActorHpBar_HumStruckSuppressedShowsFullBar()
    {
        // 三个 or 子句全假才不还原：boHumStruckShowNumber=true、m_boStruckShowNumber=false、
        // 非自己/英雄、且 m_boOpenHealth=false（此时靠 noInstanceOpenHealth 进入绘制）
        TPlaySceneCore.boHumStruckShowNumber = true;

        var a = MakeActor();
        a.m_noInstanceOpenHealth = true;
        a.m_dwOpenHealthStart = 1000;
        a.m_dwOpenHealthTime = 10000;
        a.m_boStruckShowNumber = false;
        TPlaySceneCore.HpBarTickFn = () => 1000;
        a.m_Abil.HP = 50;
        a.m_Abil.MaxHP = 100;

        var ops = MakeScene().PlanActorHpBar(a);

        // Abil.HP 保持 3676 赋的 MaxHP → 满宽
        Assert.Equal(32, ops.Find(o => o.Kind == "HumHP")!.SrcRight);
    }

    [Fact]
    public void PlanActorHpBar_HumWithStruckFlagRestoresRealHp()
    {
        TPlaySceneCore.boHumStruckShowNumber = true;

        var a = MakeActor();
        a.m_boOpenHealth = true;
        a.m_boStruckShowNumber = true;
        a.m_Abil.HP = 50;
        a.m_Abil.MaxHP = 100;

        var ops = MakeScene().PlanActorHpBar(a);
        Assert.Equal(16, ops.Find(o => o.Kind == "HumHP")!.SrcRight);
    }

    [Fact]
    public void PlanActorHpBar_SelfAlwaysRestoresRealHp()
    {
        var scene = MakeScene();
        var self = MakeActor();
        self.m_boOpenHealth = true;
        self.m_Abil.HP = 25;
        self.m_Abil.MaxHP = 100;
        scene.MySelf = self;

        var ops = scene.PlanActorHpBar(self);
        Assert.Equal(8, ops.Find(o => o.Kind == "HumHP")!.SrcRight);
    }

    // ===================== 商人（NPC）分支 =====================

    [Fact]
    public void PlanActorHpBar_MerchantExcludedAppearanceHidesBar()
    {
        var a = MakeActor(ActorLabelConsts.RC_MERCHANT);
        a.m_boOpenHealth = true;
        a.m_wAppearance = 54;                       // 在排除区间

        var ops = MakeScene().PlanActorHpBar(a);
        Assert.Empty(ops);
    }

    [Fact]
    public void PlanActorHpBar_MerchantAllowedAppearanceShowsBarWhenChecked()
    {
        TPlaySceneCore.ckShowNpcHPLabel = true;

        var a = MakeActor(ActorLabelConsts.RC_MERCHANT);
        a.m_boOpenHealth = true;
        a.m_wAppearance = 100;

        var ops = MakeScene().PlanActorHpBar(a);

        Assert.Contains(ops, o => o.Kind == "NpcBg");
        Assert.Contains(ops, o => o.Kind == "NpcHP");
    }

    [Fact]
    public void PlanActorHpBar_MerchantBarHiddenWhenNpcCheckboxOff()
    {
        TPlaySceneCore.ckShowNpcHPLabel = false;

        var a = MakeActor(ActorLabelConsts.RC_MERCHANT);
        a.m_boOpenHealth = true;
        a.m_wAppearance = 100;

        Assert.Empty(MakeScene().PlanActorHpBar(a));
    }

    [Fact]
    public void PlanActorHpBar_MerchantZeroHpForcedToOneInLocalCopyOnly()
    {
        TPlaySceneCore.ckShowNpcHPLabel = true;

        var a = MakeActor(ActorLabelConsts.RC_MERCHANT);
        a.m_boOpenHealth = true;
        a.m_wAppearance = 100;
        a.m_Abil.HP = 0;
        a.m_Abil.MaxHP = 0;

        MakeScene().PlanActorHpBar(a);

        // 3668-3671 同样只改局部副本
        Assert.Equal(0u, a.m_Abil.HP);
        Assert.Equal(0u, a.m_Abil.MaxHP);
    }

    // ===================== 宠物分支 =====================

    [Fact]
    public void PlanActorHpBar_GamePetWithNoShowProgressHidesBar()
    {
        TPlaySceneCore.boPetNoShowHPProgress = true;

        var a = MakeActor(80);
        a.m_boOpenHealth = true;
        a.m_HumsBBType = THumBBType.bbGamePet;

        Assert.Empty(MakeScene().PlanActorHpBar(a));
    }

    [Fact]
    public void PlanActorHpBar_SlavePetStillShowsBar()
    {
        TPlaySceneCore.boPetNoShowHPProgress = true;

        var a = MakeActor(80);
        a.m_boOpenHealth = true;
        a.m_HumsBBType = THumBBType.bbSlave;

        Assert.NotEmpty(MakeScene().PlanActorHpBar(a));
    }

    [Fact]
    public void PlanActorHpBar_ZeroMaxHpMonsterPetHidesBar()
    {
        TPlaySceneCore.boPetNoShowHPProgress = true;

        var a = MakeActor(80);
        a.m_boOpenHealth = true;
        a.m_Abil.HP = 0;
        a.m_Abil.MaxHP = 0;
        a.m_HumsBBType = THumBBType.bbGamePet;

        Assert.Empty(MakeScene().PlanActorHpBar(a));
    }

    // ===================== 守卫分支 =====================

    [Fact]
    public void PlanActorHpBar_GuardUsesGuardTexture()
    {
        var a = MakeActor(80);
        a.m_boOpenHealth = true;
        a.m_btRealRace = PlaySceneConsts.RC_GUARD;

        var ops = MakeScene().PlanActorHpBar(a);

        Assert.Contains(ops, o => o.Kind == "GuardBg");
        Assert.Contains(ops, o => o.Kind == "GuardHP");
        Assert.DoesNotContain(ops, o => o.Kind == "MonHP");
    }

    [Fact]
    public void PlanActorHpBar_ArcherGuardAlsoUsesGuardTexture()
    {
        var a = MakeActor(80);
        a.m_boOpenHealth = true;
        a.m_btRealRace = PlaySceneConsts.RC_ARCHERGUARD;

        var ops = MakeScene().PlanActorHpBar(a);
        Assert.Contains(ops, o => o.Kind == "GuardHP");
    }

    // ===================== 高亮显血 =====================

    [Fact]
    public void PlanActorHpBar_HighlightOnlyForSelfWithBothSwitches()
    {
        TPlaySceneCore.boShowHighlightHPLabel = true;
        TPlaySceneCore.ckShowHighlightHPLabel = true;

        var scene = MakeScene();
        var self = MakeActor();
        self.m_boOpenHealth = true;
        scene.MySelf = self;

        Assert.Contains(scene.PlanActorHpBar(self), o => o.Kind == "HighlightHP");

        // 缺任一开关 → 退回普通血条
        TPlaySceneCore.ckShowHighlightHPLabel = false;
        Assert.Contains(scene.PlanActorHpBar(self), o => o.Kind == "HumHP");
    }

    [Fact]
    public void PlanActorHpBar_HighlightNotAppliedToOtherPlayers()
    {
        TPlaySceneCore.boShowHighlightHPLabel = true;
        TPlaySceneCore.ckShowHighlightHPLabel = true;

        var scene = MakeScene();
        var other = MakeActor();
        other.m_boOpenHealth = true;
        scene.MySelf = MakeActor();                 // 自己另有其人

        Assert.DoesNotContain(scene.PlanActorHpBar(other), o => o.Kind == "HighlightHP");
    }

    // ===================== 护身蓝条 =====================

    [Fact]
    public void PlanActorHpBar_MagicShieldUsesBlueBarTextures()
    {
        TPlaySceneCore.boShowMagicShieldHP = true;

        var a = MakeActor();
        a.m_boOpenHealth = true;
        a.m_boMagicShield = true;
        a.m_btJob = 0;
        a.m_Abil.Level = 40;                        // >= 28 → 走 else 分支
        a.m_Abil.MP = 30;
        a.m_Abil.MaxMP = 60;

        var ops = MakeScene().PlanActorHpBar(a);

        Assert.Contains(ops, o => o.Kind == "ShieldBg");
        Assert.Contains(ops, o => o.Kind == "ShieldHP");
        var mp = ops.Find(o => o.Kind == "ShieldMP");
        Assert.NotNull(mp);
        Assert.Equal(16, mp!.SrcRight);              // round(32/60*30) = 16
    }

    [Fact]
    public void PlanActorHpBar_MagicShieldLowLevelWarriorGetsNoMpBar()
    {
        TPlaySceneCore.boShowMagicShieldHP = true;

        var a = MakeActor();
        a.m_boOpenHealth = true;
        a.m_boMagicShield = true;
        a.m_btJob = 0;                              // 战士
        a.m_Abil.Level = 10;                        // < 28

        var ops = MakeScene().PlanActorHpBar(a);

        Assert.Contains(ops, o => o.Kind == "ShieldBg");
        Assert.DoesNotContain(ops, o => o.Kind == "ShieldMP");   // rc2 宽 0 → 不绘制
    }

    [Fact]
    public void PlanActorHpBar_MagicShieldOtherWithLevelZeroSwapsToMpOnly()
    {
        TPlaySceneCore.boShowMagicShieldHP = true;

        var scene = MakeScene();
        scene.MySelf = MakeActor();                 // 自己另有其人

        var a = MakeActor();
        a.m_boOpenHealth = true;
        a.m_boMagicShield = true;
        a.m_Abil.Level = 0;
        a.m_btJob = 1;
        a.m_Abil.HP = 50;
        a.m_Abil.MaxHP = 100;
        a.m_Abil.MP = 30;
        a.m_Abil.MaxMP = 60;

        var ops = scene.PlanActorHpBar(a);

        // rc2 := rc（已收缩的 HP 条）、rc := 宽 0 → 不绘 ShieldHP，绘 ShieldMP
        Assert.DoesNotContain(ops, o => o.Kind == "ShieldHP");
        var mp = ops.Find(o => o.Kind == "ShieldMP");
        Assert.NotNull(mp);
        Assert.Equal(16, mp!.SrcRight);              // 沿用收缩后的 HP 宽度 = 16
    }

    // ===================== 自定义怪物 =====================

    [Fact]
    public void PlanActorHpBar_CustomMonsterNegativeStartIndexUsesDefault()
    {
        var a = MakeActor(80);
        a.m_boOpenHealth = true;
        a.IsCustomActor = true;
        a.CustomBaseConfig = new HpBarBaseConfig { HPStartIndex = -1, HPOffsetX = 2, HPOffsetY = 3 };

        var ops = MakeScene().PlanActorHpBar(a);

        // nX + HPOffsetX + HpBarOffsetX = 84 + 2 + 0
        var bg = ops.Find(o => o.Kind == "CustomMonBg");
        Assert.NotNull(bg);
        Assert.Equal(86, bg!.X);
        Assert.Equal(193, bg.Y);
        Assert.Contains(ops, o => o.Kind == "CustomMonHP");
    }

    [Fact]
    public void PlanActorHpBar_CustomMonsterWithValidFileUsesEffectImage()
    {
        TPlaySceneCore.EffectImageListCountFn = () => 3;

        var a = MakeActor(80);
        a.m_boOpenHealth = true;
        a.IsCustomActor = true;
        a.CustomBaseConfig = new HpBarBaseConfig
        {
            HPFile = 1,
            HPStartIndex = 5,
            HPOffsetX = 1,
            HPOffsetY = 1,
            HPBgOffsetX = 7,
            HPBgOffsetY = 8,
        };
        a.m_Abil.HP = 50;
        a.m_Abil.MaxHP = 100;

        var images = new FakeImages();
        images.Effect[(1, 5)] = LabelSurface.Of(60, 10);
        images.Effect[(1, 6)] = LabelSurface.Of(50, 8);
        TPlaySceneCore.HpBarImages = images;

        var ops = MakeScene().PlanActorHpBar(a);

        // 背景居中：cx = 100 - 60/2 = 70
        var bg = ops.Find(o => o.Kind == "CustomMonCustomBg");
        Assert.NotNull(bg);
        Assert.Equal(70 + 7, bg!.X);
        Assert.Equal(190 + 8, bg.Y);

        var hp = ops.Find(o => o.Kind == "CustomMonCustomHP");
        Assert.NotNull(hp);
        Assert.Equal(70 + 1, hp!.X);
        Assert.Equal(25, hp.SrcRight);               // round(50/100*50) = 25（用 DHP 宽 50）
    }

    [Fact]
    public void PlanActorHpBar_CustomMonsterInvalidFileFallsBackToMonImages()
    {
        TPlaySceneCore.EffectImageListCountFn = () => 1;   // HPFile=5 越界

        var a = MakeActor(80);
        a.m_boOpenHealth = true;
        a.IsCustomActor = true;
        a.m_wAppearance = 300;
        a.CustomBaseConfig = new HpBarBaseConfig
        {
            HPFile = 5,
            HPStartIndex = 3,
            HPBgOffsetY = 2,
        };

        var images = new FakeImages();
        images.Mon[300] = LabelSurface.Of(40, 12);
        images.Mon[301] = LabelSurface.Of(30, 6);
        TPlaySceneCore.HpBarImages = images;

        var ops = MakeScene().PlanActorHpBar(a);

        Assert.Contains(ops, o => o.Kind == "CustomMonCustomBg");
        Assert.Contains(ops, o => o.Kind == "CustomMonCustomHP");
    }

    [Fact]
    public void PlanActorHpBar_CustomMonsterNullSourceUsesDefaultTextures()
    {
        TPlaySceneCore.EffectImageListCountFn = () => 3;
        TPlaySceneCore.HpBarImages = null;

        var a = MakeActor(80);
        a.m_boOpenHealth = true;
        a.IsCustomActor = true;
        a.CustomBaseConfig = new HpBarBaseConfig { HPFile = 1, HPStartIndex = 5, HPOffsetX = 4 };

        var ops = MakeScene().PlanActorHpBar(a);

        Assert.Contains(ops, o => o.Kind == "CustomDefBg");
        Assert.Contains(ops, o => o.Kind == "CustomDefHP");
    }

    // ===================== 商人自定义血条 =====================

    [Fact]
    public void PlanActorHpBar_MerchantCustomNpcConfigUsesItsTextures()
    {
        TPlaySceneCore.ckShowNpcHPLabel = true;
        TPlaySceneCore.EffectImageListCountFn = () => 2;

        var a = MakeActor(ActorLabelConsts.RC_MERCHANT);
        a.m_boOpenHealth = true;
        a.m_wAppearance = 10000;
        a.m_Abil.HP = 25;
        a.m_Abil.MaxHP = 100;

        var cfg = new HpBarNpcConfig();
        cfg.BaseConfig.HPFile = 0;
        cfg.BaseConfig.HPStartIndex = 1;
        cfg.BaseConfig.HPOffsetX = 3;
        cfg.BaseConfig.HPOffsetY = 4;
        cfg.BaseConfig.HPBgOffsetY = 9;
        TPlaySceneCore.GetCustomNpcConfigFn = _ => cfg;

        var images = new FakeImages();
        images.Effect[(0, 1)] = LabelSurface.Of(80, 10);
        images.Effect[(0, 2)] = LabelSurface.Of(64, 6);
        TPlaySceneCore.HpBarImages = images;

        var ops = MakeScene().PlanActorHpBar(a);

        // cx = 100 - 80/2 = 60
        Assert.Equal(60, ops.Find(o => o.Kind == "NpcCustomBg")!.X);
        var hp = ops.Find(o => o.Kind == "NpcCustomHP");
        Assert.NotNull(hp);
        Assert.Equal(16, hp!.SrcRight);              // round(64/100*25) = 16
    }

    [Fact]
    public void PlanActorHpBar_MerchantWithoutCustomConfigUsesDefault()
    {
        TPlaySceneCore.ckShowNpcHPLabel = true;
        TPlaySceneCore.GetCustomNpcConfigFn = _ => null;

        var a = MakeActor(ActorLabelConsts.RC_MERCHANT);
        a.m_boOpenHealth = true;
        a.m_wAppearance = 10000;

        var ops = MakeScene().PlanActorHpBar(a);

        Assert.Contains(ops, o => o.Kind == "NpcBg");
        Assert.Contains(ops, o => o.Kind == "NpcHP");
    }

    // ===================== 内功黄条 =====================

    [Fact]
    public void PlanNgLabel_RequiresAllFiveConditions()
    {
        var scene = MakeScene();
        var a = MakeActor();
        a.m_boTrainingNG = true;
        a.m_AbilNG.NH = 50;
        a.m_AbilNG.MaxNH = 100;
        TPlaySceneCore.boShowNGLabel = true;
        TPlaySceneCore.ckShowNGLabel = true;

        Assert.NotEmpty(scene.PlanNgLabel(a, 0, 0));

        TPlaySceneCore.ckShowNGLabel = false;
        Assert.Empty(scene.PlanNgLabel(a, 0, 0));
    }

    [Fact]
    public void PlanNgLabel_MonsterNeverGetsNgBar()
    {
        TPlaySceneCore.boShowNGLabel = true;
        TPlaySceneCore.ckShowNGLabel = true;

        var a = MakeActor(80);
        a.m_boTrainingNG = true;
        a.m_AbilNG.NH = 50;
        a.m_AbilNG.MaxNH = 100;

        Assert.Empty(MakeScene().PlanNgLabel(a, 0, 0));
    }

    [Fact]
    public void PlanNgLabel_ZeroMaxNhDrawsNothing()
    {
        TPlaySceneCore.boShowNGLabel = true;
        TPlaySceneCore.ckShowNGLabel = true;

        var a = MakeActor();
        a.m_boTrainingNG = true;
        a.m_AbilNG.NH = 0;
        a.m_AbilNG.MaxNH = 0;

        Assert.Empty(MakeScene().PlanNgLabel(a, 0, 0));
    }

    [Fact]
    public void PlanNgLabel_StallSkips()
    {
        TPlaySceneCore.boShowNGLabel = true;
        TPlaySceneCore.ckShowNGLabel = true;

        var a = MakeActor();
        a.m_boTrainingNG = true;
        a.m_AbilNG.NH = 50;
        a.m_AbilNG.MaxNH = 100;
        a.m_boShopStall = true;

        Assert.Empty(MakeScene().PlanNgLabel(a, 0, 0));
    }

    [Fact]
    public void PlanNgLabel_NhRatioDrivesWidthAndYOffsetsByImageHeight()
    {
        TPlaySceneCore.boShowNGLabel = true;
        TPlaySceneCore.ckShowNGLabel = true;
        TPlaySceneCore.NewopUI170TextureArray[HpBarTextureIndex.Background] = LabelSurface.Of(40, 5);

        var a = MakeActor();
        a.m_boTrainingNG = true;
        a.m_AbilNG.NH = 25;
        a.m_AbilNG.MaxNH = 100;

        var ops = MakeScene().PlanNgLabel(a, 2, 3);

        // hpOffsetX = 20, hpOffsetY = 14；x = 100-20+2 = 82；y = 200-14+5+3 = 194
        Assert.Equal(82, ops[0].X);
        Assert.Equal(194, ops[0].Y);
        Assert.Equal(10, ops[1].SrcRight);           // round(40*25/100)
    }

    // ===================== 外观排除 =====================

    [Fact]
    public void IsNpcAppearanceExcluded_MatchesOriginalRanges()
    {
        Assert.True(TPlaySceneCore.IsNpcAppearanceExcluded(54));
        Assert.True(TPlaySceneCore.IsNpcAppearanceExcluded(58));
        Assert.True(TPlaySceneCore.IsNpcAppearanceExcluded(60));
        Assert.True(TPlaySceneCore.IsNpcAppearanceExcluded(68));
        Assert.True(TPlaySceneCore.IsNpcAppearanceExcluded(90));
        Assert.True(TPlaySceneCore.IsNpcAppearanceExcluded(92));
        Assert.True(TPlaySceneCore.IsNpcAppearanceExcluded(94));
        Assert.True(TPlaySceneCore.IsNpcAppearanceExcluded(98));

        Assert.False(TPlaySceneCore.IsNpcAppearanceExcluded(53));
        Assert.False(TPlaySceneCore.IsNpcAppearanceExcluded(59));
        Assert.False(TPlaySceneCore.IsNpcAppearanceExcluded(69));
        Assert.False(TPlaySceneCore.IsNpcAppearanceExcluded(93));
        Assert.False(TPlaySceneCore.IsNpcAppearanceExcluded(99));
    }

    [Fact]
    public void HpBarTextureIndex_MatchesOriginalLiterals()
    {
        Assert.Equal(0, HpBarTextureIndex.Background);
        Assert.Equal(1, HpBarTextureIndex.HumHP);
        Assert.Equal(2, HpBarTextureIndex.HighlightHP);
        Assert.Equal(3, HpBarTextureIndex.NgYellow);
        Assert.Equal(4, HpBarTextureIndex.NpcHP);
        Assert.Equal(5, HpBarTextureIndex.MonsterHP);
        Assert.Equal(6, HpBarTextureIndex.GuardHP);
        Assert.Equal(7, HpBarTextureIndex.MagicShieldMP);
    }

    [Fact]
    public void RaceConstants_MatchGrobal2()
    {
        Assert.Equal(0, ActorLabelConsts.RC_PLAYOBJECT);
        Assert.Equal(1, ActorLabelConsts.RC_HEROOBJECT);
        Assert.Equal(50, ActorLabelConsts.RC_MERCHANT);
        Assert.Equal(15, ActorLabelConsts.RC_PEACENPC);   // Grobal2.pas 196
    }
}

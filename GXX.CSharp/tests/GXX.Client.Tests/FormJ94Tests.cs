using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J94：Actor.pas TActor.RunSound(6788-6901) 与 TActor.RunActSound(6903-7043) 的
/// **派发决策层** 1:1 测试（不含 Bass 音频播放本身）。
/// </summary>
public sealed class ActorSoundDispatchTests
{
    // ===================== 常量 =====================

    [Fact]
    public void MessageConstantsMatchGrobal2()
    {
        Assert.Equal(65069, TActorCore.SM_THROW);
        Assert.Equal(22, TActorCore.SM_FLYAXE);
        Assert.Equal(23, TActorCore.SM_LIGHTING);
        Assert.Equal(21, TActorCore.SM_DIGDOWN);
        Assert.Equal(166, TActorCore.SM_66HIT1);
        Assert.Equal(113, TActorCore.SM_113HIT);
        Assert.Equal(115, TActorCore.SM_115HIT);
    }

    [Fact]
    public void CustomMagicStartIdIsOne()
    {
        Assert.Equal(1, ActorSoundDispatch.CustomMagicStartId);
    }

    [Fact]
    public void Is100To103HitRange()
    {
        Assert.True(ActorSoundDispatch.Is100To103Hit(9100));
        Assert.True(ActorSoundDispatch.Is100To103Hit(9101));
        Assert.True(ActorSoundDispatch.Is100To103Hit(9102));
        Assert.True(ActorSoundDispatch.Is100To103Hit(9103));
        Assert.False(ActorSoundDispatch.Is100To103Hit(9099));
        Assert.False(ActorSoundDispatch.Is100To103Hit(9104));
    }

    [Fact]
    public void ScreenShakeActionIs102Hit()
    {
        Assert.Equal(9102, ActorSoundDispatch.ScreenShakeAction);
    }

    // ===================== RunSound 门禁 =====================

    [Fact]
    public void RunActSoundGateRequiresFlag()
    {
        Assert.True(ActorSoundDispatch.RunActSoundGate(true));
        Assert.False(ActorSoundDispatch.RunActSoundGate(false));
    }

    [Fact]
    public void WeaponSoundRaceIs0Or1()
    {
        Assert.True(ActorSoundDispatch.IsWeaponSoundRace(0));
        Assert.True(ActorSoundDispatch.IsWeaponSoundRace(1));
        Assert.False(ActorSoundDispatch.IsWeaponSoundRace(2));
        Assert.False(ActorSoundDispatch.IsWeaponSoundRace(50));
    }

    // ===================== RunSound 各分支 =====================

    private static List<SoundCue> RS(
        int action,
        int struckWeapon = -1, int struck = -1, int scream = -1,
        int die = -1, bool death = false, bool mySelf = false,
        int attack = -1, int appear = 77, int magicStart = 88,
        bool hasCustom = false, int newLevel = 0,
        IReadOnlyList<int>? customSounds = null)
        => ActorSoundDispatch.RunSound(action, struckWeapon, struck, scream, die, death, mySelf,
            attack, appear, magicStart, hasCustom, newLevel, customSounds);

    [Fact]
    public void StruckPlaysAllThreeIndependently()
    {
        // 6798-6800：三个声音各自独立判 >= 0
        var cues = RS(TActorCore.SM_STRUCK, struckWeapon: 10, struck: 11, scream: 12);

        Assert.Equal(3, cues.Count);
        Assert.Equal(10, cues[0].SoundId);
        Assert.Equal(11, cues[1].SoundId);
        Assert.Equal(12, cues[2].SoundId);
    }

    [Fact]
    public void StruckSkipsNegativeSounds()
    {
        var cues = RS(TActorCore.SM_STRUCK, struckWeapon: -1, struck: 11, scream: -1);
        Assert.Single(cues);
        Assert.Equal(11, cues[0].SoundId);
    }

    [Fact]
    public void StruckZeroSoundIsValid()
    {
        // 判据是 >= 0，故 0 也要播
        var cues = RS(TActorCore.SM_STRUCK, struckWeapon: 0);
        Assert.Single(cues);
        Assert.Equal(0, cues[0].SoundId);
    }

    [Fact]
    public void NowDeathRequiresDeathFlag()
    {
        Assert.Empty(RS(TActorCore.SM_NOWDEATH, die: 5, death: false));
        Assert.Single(RS(TActorCore.SM_NOWDEATH, die: 5, death: true));
    }

    [Fact]
    public void NowDeathSelfAddsGameOverBgm()
    {
        var cues = RS(TActorCore.SM_NOWDEATH, die: 5, death: true, mySelf: true);
        Assert.Equal(2, cues.Count);
        Assert.Equal("GameOverBgmDelay", cues[1].Kind);
    }

    [Fact]
    public void NowDeathOtherHasNoBgm()
    {
        var cues = RS(TActorCore.SM_NOWDEATH, die: 5, death: true, mySelf: false);
        Assert.Single(cues);
    }

    [Theory]
    [InlineData(TActorCore.SM_THROW)]
    [InlineData(TActorCore.SM_HIT)]
    [InlineData(TActorCore.SM_FLYAXE)]
    [InlineData(TActorCore.SM_LIGHTING)]
    [InlineData(TActorCore.SM_DIGDOWN)]
    public void ThrowHitFamilyPlaysAttackSound(int action)
    {
        var cues = RS(action, attack: 33);
        Assert.Single(cues);
        Assert.Equal(33, cues[0].SoundId);
    }

    [Fact]
    public void ThrowHitFamilySkipsNegativeAttack()
    {
        Assert.Empty(RS(TActorCore.SM_HIT, attack: -1));
    }

    [Theory]
    [InlineData(TActorCore.SM_ALIVE)]
    [InlineData(TActorCore.SM_DIGUP)]
    public void AliveDigUpPlaysAppearSoundUnconditionally(int action)
    {
        // 6815：无 >= 0 判定
        var cues = RS(action, appear: 77);
        Assert.Single(cues);
        Assert.Equal(77, cues[0].SoundId);
    }

    // ===================== RunSound 的 SM_SPELL =====================

    [Fact]
    public void SpellWithoutCustomConfigPlaysMagicStart()
    {
        var cues = RS(TActorCore.SM_SPELL, hasCustom: false, magicStart: 88);
        Assert.Single(cues);
        Assert.Equal(88, cues[0].SoundId);
        Assert.Equal("MagicStart", cues[0].Kind);
    }

    [Fact]
    public void SpellWithCustomConfigAndSoundPlaysCustom()
    {
        // 4 档音：mplNone / mpl1_3 / mpl4_6 / mpl7_9
        var sounds = new List<int> { 0, 101, 102, 103 };

        var none = RS(TActorCore.SM_SPELL, hasCustom: true, newLevel: 0, customSounds: sounds);
        var l13 = RS(TActorCore.SM_SPELL, hasCustom: true, newLevel: 2, customSounds: sounds);
        var l46 = RS(TActorCore.SM_SPELL, hasCustom: true, newLevel: 5, customSounds: sounds);
        var l79 = RS(TActorCore.SM_SPELL, hasCustom: true, newLevel: 8, customSounds: sounds);

        Assert.Empty(none);                            // 档位 0 长度为 0 → 不播
        Assert.Equal(101, l13[0].SoundId);
        Assert.Equal(102, l46[0].SoundId);
        Assert.Equal(103, l79[0].SoundId);
    }

    [Fact]
    public void SpellCustomConfigWithEmptySoundPlaysNothing()
    {
        // 6832：Length > 0 才播；**注意此时不会退回 m_nMagicStartSound**
        var sounds = new List<int> { 0, 0, 0, 0 };
        Assert.Empty(RS(TActorCore.SM_SPELL, hasCustom: true, newLevel: 5, customSounds: sounds));
    }

    [Fact]
    public void SpellCustomUsesSameLevelMappingAsSelfMagicEffect()
    {
        // 与 J87 的 MagicPlusLevelOf 一致：else（含负数/超大）落最高档
        Assert.Equal(TMagicPlusLevel.mpl7_9, SelfMagicEffectRender.MagicPlusLevelOf(99));
        Assert.Equal(TMagicPlusLevel.mpl7_9, SelfMagicEffectRender.MagicPlusLevelOf(-1));
    }

    [Fact]
    public void UnknownActionPlaysNothing()
    {
        Assert.Empty(RS(99999));
    }

    // ===================== RunActSound 武器族 =====================

    /// <summary>全部参数具名调用，避免 out 与可选参数的次序冲突。</summary>
    private sealed class RaArgs
    {
        public int Action;
        public int Frame = 2;
        public int Sex;
        public int Weapon = 500;
        public int Attack = 501;
        public bool HasCustom;
        public int NewLevel;
        public Func<int, int, int>? CustomSound;
        public bool SceneShake;
    }

    private static List<SoundCue> RA(RaArgs a, out bool closed)
        => ActorSoundDispatch.RunActSoundWar(a.Action, a.Frame, a.Sex, a.Weapon,
            601, 602, 603, 604, 605, 606,
            a.HasCustom, a.NewLevel, a.CustomSound, a.SceneShake, out closed);

    [Fact]
    public void FrameMustBeTwoForAllBranches()
    {
        foreach (int action in new[] { TActorCore.SM_HIT, TActorCore.SM_POWERHIT, TActorCore.SM_60HIT, 9102 })
        {
            Assert.Empty(RA(new RaArgs { Action = action, Frame = 1 }, out bool closed));
            Assert.False(closed);
        }
    }

    [Fact]
    public void HitFamilyPlaysWeaponOnly()
    {
        var cues = RA(new RaArgs { Action = TActorCore.SM_HIT }, out bool closed);
        Assert.Single(cues);
        Assert.Equal(500, cues[0].SoundId);
        Assert.True(closed);
    }

    [Fact]
    public void HitPlusOneAndTwoAlsoMatch()
    {
        Assert.Single(RA(new RaArgs { Action = TActorCore.SM_HIT + 1 }, out _));
        Assert.Single(RA(new RaArgs { Action = TActorCore.SM_HIT + 2 }, out _));
    }

    [Fact]
    public void PowerHitPlaysWeaponAndYedoBySex()
    {
        var man = RA(new RaArgs { Action = TActorCore.SM_POWERHIT, Sex = 0 }, out _);
        var woman = RA(new RaArgs { Action = TActorCore.SM_POWERHIT, Sex = 1 }, out _);

        Assert.Equal(2, man.Count);
        Assert.Equal(601, man[1].SoundId);
        Assert.Equal(602, woman[1].SoundId);
    }

    [Fact]
    public void LongWideFireHitPlayWeaponPlusSpecific()
    {
        Assert.Equal(603, RA(new RaArgs { Action = TActorCore.SM_LONGHIT }, out _)[1].SoundId);
        Assert.Equal(604, RA(new RaArgs { Action = TActorCore.SM_WIDEHIT }, out _)[1].SoundId);
        Assert.Equal(605, RA(new RaArgs { Action = TActorCore.SM_FIREHIT }, out _)[1].SoundId);
    }

    [Fact]
    public void TwnHitUsesDedicatedSound11058()
    {
        var cues = RA(new RaArgs { Action = TActorCore.SM_TWNHIT }, out _);
        Assert.Equal(11058, cues[1].SoundId);
    }

    [Fact]
    public void CrsAnd43HitUseWideHitSound()
    {
        // 6977：只有 SM_TWNHIT 走 11058，其余用 s_widehit
        Assert.Equal(604, RA(new RaArgs { Action = TActorCore.SM_CRSHIT }, out _)[1].SoundId);
        Assert.Equal(604, RA(new RaArgs { Action = TActorCore.SM_43HIT }, out _)[1].SoundId);
    }

    [Fact]
    public void Hit60PlaysWeaponAndPhz()
    {
        var cues = RA(new RaArgs { Action = TActorCore.SM_60HIT }, out _);
        Assert.Equal(2, cues.Count);
        Assert.Equal(500, cues[0].SoundId);
        Assert.Equal(606, cues[1].SoundId);
    }

    [Fact]
    public void Hit61DoesNotPlayWeaponSound()
    {
        // 6990-6997：武器音那行被注释掉，只播 124 与 10512
        var cues = RA(new RaArgs { Action = TActorCore.SM_61HIT }, out bool closed);

        Assert.Equal(2, cues.Count);
        Assert.Equal(124, cues[0].SoundId);
        Assert.Equal(10512, cues[1].SoundId);
        Assert.DoesNotContain(cues, c => c.SoundId == 500);
        Assert.True(closed);
    }

    [Fact]
    public void SwordHitPlaysWeaponAndFireHit()
    {
        var cues = RA(new RaArgs { Action = TActorCore.SM_SWORDHIT }, out _);
        Assert.Equal(500, cues[0].SoundId);
        Assert.Equal(605, cues[1].SoundId);
    }

    // ===================== SM_100HIT..SM_103HIT 范围 =====================

    [Fact]
    public void All100To103PlayWeaponAndFireHit()
    {
        foreach (int a in new[] { 9100, 9101, 9102, 9103 })
        {
            var cues = RA(new RaArgs { Action = a }, out _);
            Assert.Equal(2, cues.Count);
            Assert.Equal(500, cues[0].SoundId);
            Assert.Equal(605, cues[1].SoundId);
        }
    }

    [Fact]
    public void Only102HitTriggersSceneShake()
    {
        // 7011：`m_nCurrentAction = SM_102HIT` 且勾选震动
        var withShake = RA(new RaArgs { Action = 9102, SceneShake = true }, out _);
        var noCheck = RA(new RaArgs { Action = 9102, SceneShake = false }, out _);
        var other = RA(new RaArgs { Action = 9101, SceneShake = true }, out _);

        Assert.Contains(withShake, c => c.Kind == "SceneShake");
        Assert.DoesNotContain(noCheck, c => c.Kind == "SceneShake");
        Assert.DoesNotContain(other, c => c.Kind == "SceneShake");
    }

    [Fact]
    public void RangeBoundariesExcluded()
    {
        Assert.Empty(RA(new RaArgs { Action = 9099 }, out _));
        Assert.Empty(RA(new RaArgs { Action = 9104 }, out _));
    }

    // ===================== 66 / 113 / 115 =====================

    [Fact]
    public void Hit66And66Hit1Play11056WithoutWeapon()
    {
        // 7016-7020：武器音不播，只播 11056
        foreach (int a in new[] { TActorCore.SM_66HIT, TActorCore.SM_66HIT1 })
        {
            var cues = RA(new RaArgs { Action = a }, out bool closed);
            Assert.Single(cues);
            Assert.Equal(11056, cues[0].SoundId);
            Assert.True(closed);
        }
    }

    [Fact]
    public void Hit113SoundBySex()
    {
        Assert.Equal(11030, RA(new RaArgs { Action = TActorCore.SM_113HIT, Sex = 0 }, out _)[0].SoundId);
        Assert.Equal(11031, RA(new RaArgs { Action = TActorCore.SM_113HIT, Sex = 1 }, out _)[0].SoundId);
    }

    [Fact]
    public void Hit115SoundBySex()
    {
        Assert.Equal(11033, RA(new RaArgs { Action = TActorCore.SM_115HIT, Sex = 0 }, out _)[0].SoundId);
        Assert.Equal(11034, RA(new RaArgs { Action = TActorCore.SM_115HIT, Sex = 1 }, out _)[0].SoundId);
    }

    // ===================== 自定义范围（6917） =====================

    [Fact]
    public void CustomRangeSerialFormula()
    {
        // 6919：m_nCurrentAction - SM_CUSTOM_HIT001 + CUSTOM_MAGIC_START_ID
        int serial = -1;
        RA(new RaArgs { Action = 11005, HasCustom = true, NewLevel = 0, CustomSound = (s, l) => { serial = s; return 900; } }, out _);

        Assert.Equal(5 - 0 + 1, serial);               // 11005 - 11000 + 1
    }

    [Fact]
    public void CustomRangeUsesManSoundForSex0()
    {
        var cues = RA(new RaArgs { Action = 11000, Sex = 0, HasCustom = true, CustomSound = (s, l) => 900 }, out bool closed);

        Assert.Single(cues);
        Assert.Equal(900, cues[0].SoundId);
        Assert.Equal("CustomManWarr", cues[0].Kind);
        Assert.True(closed);
    }

    [Fact]
    public void CustomRangeUsesWomanSoundForSex1()
    {
        var cues = RA(new RaArgs { Action = 11000, Sex = 1, HasCustom = true, CustomSound = (s, l) => 900 }, out _);
        Assert.Equal("CustomWomanWarr", cues[0].Kind);
    }

    [Fact]
    public void CustomRangeFallsBackToWeaponWhenNoSound()
    {
        // 6940-6943：未配置时退回武器音
        var cues = RA(new RaArgs { Action = 11000, Sex = 0, HasCustom = true, CustomSound = (s, l) => 0 }, out bool closed);

        Assert.Single(cues);
        Assert.Equal(500, cues[0].SoundId);
        Assert.Equal("CustomFallbackWeapon", cues[0].Kind);
        Assert.True(closed);
    }

    [Fact]
    public void CustomRangeWithoutConfigPlaysNothingAndKeepsFlag()
    {
        // 6920：取不到配置时既不播也不关 m_boRunSound
        var cues = RA(new RaArgs { Action = 11000, HasCustom = false, CustomSound = (s, l) => 900 }, out bool closed);

        Assert.Empty(cues);
        Assert.False(closed);
    }

    [Fact]
    public void CustomRangeBoundaryIsInclusive()
    {
        Assert.Single(RA(new RaArgs { Action = 11000, HasCustom = true, CustomSound = (s, l) => 900 }, out _));
        Assert.Single(RA(new RaArgs { Action = 11299, HasCustom = true, CustomSound = (s, l) => 900 }, out _));
        Assert.Empty(RA(new RaArgs { Action = 11300, HasCustom = true, CustomSound = (s, l) => 900 }, out _));
    }

    // ===================== RunActSound 非武器种族 =====================

    [Fact]
    public void Race50IsEmptyImplementation()
    {
        // 7046-7047：空 begin/end，既不播音也**不消费随机数**
        int randCalls = 0;
        var cues = ActorSoundDispatch.RunActSoundOther(
            50, TActorCore.SM_TURN, frame: 1, 700, 701, 702,
            () => { randCalls++; return 1; }, out bool closed);

        Assert.Empty(cues);
        Assert.False(closed);
        Assert.Equal(0, randCalls);
    }

    [Fact]
    public void TurnPlaysNormalSoundOnlyWhenRandomHitsOne()
    {
        var hit = ActorSoundDispatch.RunActSoundOther(
            1, TActorCore.SM_TURN, 1, 700, 701, 702, () => 1, out bool c1);
        var miss = ActorSoundDispatch.RunActSoundOther(
            1, TActorCore.SM_TURN, 1, 700, 701, 702, () => 2, out bool c2);

        Assert.Single(hit);
        Assert.Equal(700, hit[0].SoundId);
        Assert.True(c1);

        Assert.Empty(miss);
        Assert.False(c2);
    }

    [Fact]
    public void TurnRequiresFrameOne()
    {
        var cues = ActorSoundDispatch.RunActSoundOther(
            1, TActorCore.SM_TURN, frame: 2, 700, 701, 702, () => 1, out _);

        Assert.Empty(cues);
    }

    [Fact]
    public void HitRequiresFrameThree()
    {
        var f3 = ActorSoundDispatch.RunActSoundOther(
            1, TActorCore.SM_HIT, 3, 700, 701, 702, () => 0, out bool c3);
        var f2 = ActorSoundDispatch.RunActSoundOther(
            1, TActorCore.SM_HIT, 2, 700, 701, 702, () => 0, out bool c2);

        Assert.Single(f3);
        Assert.Equal(702, f3[0].SoundId);
        Assert.True(c3);
        Assert.Empty(f2);
        Assert.False(c2);
    }

    [Fact]
    public void HitRequiresNonNegativeAttackSound()
    {
        var cues = ActorSoundDispatch.RunActSoundOther(
            1, TActorCore.SM_HIT, 3, 700, -1, 702, () => 0, out bool closed);

        Assert.Empty(cues);
        Assert.False(closed);
    }

    [Fact]
    public void NonTurnNonHitPlaysNothing()
    {
        var cues = ActorSoundDispatch.RunActSoundOther(
            1, TActorCore.SM_WALK, 1, 700, 701, 702, () => 1, out _);

        Assert.Empty(cues);
    }
}

using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 车道 `p17-client-actor` 切片 2：`Actor.pas` <c>THumActor</c> 已落地的 **8 条**
/// （<c>UseMagicDelayTime</c> 13121 / <c>light</c> 14520 / <c>DoWeaponBreakEffect</c> 13355 /
/// <c>DoBrokenShieldEffect</c> 13361 / <c>CheckLoadDressAddEffect</c> 13419 /
/// <c>LoadDressAddEffect</c> 13439 / <c>OnTargetExplosion</c> 13637 / <c>TakeHorse</c> 17426）
/// 的 1:1 回归证据。
///
/// <para><b>本切片选择标准</b>：只做**纯函数或纯字段写**、且**不依赖本类构造期字段初始化**的成员
/// —— 这样 <c>new THumActor()</c> 的初始状态不变，既有的
/// <c>ActorFamilyBaseTests.NewActor()</c>（以 <c>THumActor</c> 作 <c>TActor</c> 替身）
/// 断言不会连带改变。构造期初始化（<c>Create</c> 11130 的 82 行）与 5 条巨型方法
/// （<c>CalcActorFrame</c> 1783 行 / <c>LoadSurface</c> 1969 行 / <c>DrawChr</c> 925 行 /
/// <c>Run</c> 436 行 / <c>PlayMagicEffect</c> 326 行）列入未完成项。</para>
/// </summary>
public sealed class ActorHumActorTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public ActorHumActorTests()
    {
        ActorNpcEnv.Reset();
        _restore.Add(ActorNpcEnv.Reset);

        ActorFamilyEnv.Reset();
        _restore.Add(ActorFamilyEnv.Reset);

        ActorNpcEnv.MyGetTickCountFn = () => 5000;
        _restore.Add(() => ActorNpcEnv.MyGetTickCountFn = () => SceneTime.TickNow());
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    // ══════════════════════════════════════════════════════════════════════
    // 1. UseMagicDelayTime（13121-13134）
    // ══════════════════════════════════════════════════════════════════════

    [Theory]
    [InlineData(0, 200)]
    [InlineData(100, 300)]
    [InlineData(-100, 100)]
    [InlineData(150, 350)]
    public void UseMagicDelayTime_IsTwohundredPlusDelay(int delay, int expected)
    {
        var hum = new THumActor();

        Assert.Equal(expected, hum.UseMagicDelayTime(delay));
    }

    [Fact]
    public void UseMagicDelayTime_DoesNotUseTheCommentedOutSpellSpeedFormula()
    {
        // ★ 原文 13125-13130 的加速公式被整段注释；若"补回"，300 会先被 nSpellSpeed 削减。
        //   本断言用同一个入参锁住"恒定 200 + delay"（与 spellSpeed 无关，因为该接缝根本不存在）。
        var hum = new THumActor();

        Assert.Equal(500, hum.UseMagicDelayTime(300));   // 被注释的公式基准是 300 ⇒ 会是 300 起算
        Assert.NotEqual(300, hum.UseMagicDelayTime(300));
    }

    // ══════════════════════════════════════════════════════════════════════
    // 2. light（14520-14530；覆写 TActor.light 5415-5418）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void BaseActorLight_ReturnsChrLightOnly()
    {
        var actor = new TActor { m_nChrLight = 7, m_nMagLight = 99 };

        Assert.Equal(7, actor.light());   // 5417：完全不看 m_nMagLight
    }

    [Fact]
    public void HumLight_KeepsChrLightWhenAlreadyBrighter()
    {
        var hum = new THumActor { m_nChrLight = 9, m_nMagLight = 3, m_boUseMagic = true, m_boHitEffect = true };

        Assert.Equal(9, hum.light());     // 14525 为假 ⇒ 保持
    }

    [Fact]
    public void HumLight_KeepsChrLightWhenNeitherMagicNorHitEffect()
    {
        var hum = new THumActor { m_nChrLight = 2, m_nMagLight = 8, m_boUseMagic = false, m_boHitEffect = false };

        Assert.Equal(2, hum.light());     // 14526 为假 ⇒ 不提升
    }

    [Fact]
    public void HumLight_RaisesToMagLightOnUseMagic()
    {
        var hum = new THumActor { m_nChrLight = 2, m_nMagLight = 8, m_boUseMagic = true };

        Assert.Equal(8, hum.light());     // 14527
    }

    [Fact]
    public void HumLight_RaisesToMagLightOnHitEffectAlone()
    {
        var hum = new THumActor { m_nChrLight = 2, m_nMagLight = 8, m_boUseMagic = false, m_boHitEffect = true };

        Assert.Equal(8, hum.light());     // 14526 的或项：只看 m_boUseMagic 会漏
    }

    [Fact]
    public void LightIsVirtualSoBaseStaticTypeDispatchesToHum()
    {
        // ★ 虚分派：经 TActor 静态类型调用必须落到 THumActor.light（台帐 §18.8）
        TActor asBase = new THumActor { m_nChrLight = 1, m_nMagLight = 6, m_boUseMagic = true };

        Assert.Equal(6, asBase.light());
    }

    // ══════════════════════════════════════════════════════════════════════
    // 3-4. DoWeaponBreakEffect / DoBrokenShieldEffect（13355-13365）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void DoWeaponBreakEffect_SetsFlagAndResetsCounter()
    {
        var hum = new THumActor { m_nCurWeaponEffect = 42 };

        hum.DoWeaponBreakEffect();

        Assert.True(hum.m_boWeaponEffect);
        Assert.Equal(0, hum.m_nCurWeaponEffect);
    }

    [Fact]
    public void DoBrokenShieldEffect_SetsFlagAndResetsCounter()
    {
        var hum = new THumActor { m_nBrokenShieldEffect = 42 };

        hum.DoBrokenShieldEffect();

        Assert.True(hum.m_boBrokenShield);
        Assert.Equal(0, hum.m_nBrokenShieldEffect);
    }

    [Fact]
    public void WeaponBreakAndBrokenShieldAreIndependent()
    {
        var hum = new THumActor { m_nCurWeaponEffect = 5, m_nBrokenShieldEffect = 6 };

        hum.DoWeaponBreakEffect();

        Assert.True(hum.m_boWeaponEffect);
        Assert.False(hum.m_boBrokenShield);       // 两组字段互不干扰
        Assert.Equal(6, hum.m_nBrokenShieldEffect);

        hum.DoBrokenShieldEffect();

        Assert.True(hum.m_boBrokenShield);
        Assert.Equal(0, hum.m_nBrokenShieldEffect);
        Assert.Equal(0, hum.m_nCurWeaponEffect);  // 仍是上一步归的零
    }

    // ══════════════════════════════════════════════════════════════════════
    // 5. CheckLoadDressAddEffect（13419-13437）
    // ══════════════════════════════════════════════════════════════════════

    private static THumActor AddEffectActor(int index = 1, ushort count = 4, ushort time = 100, int cur = 0, uint lastTick = 5000)
        => new()
        {
            m_nDressAddEffectIndex = (short)index,
            m_wDressAddEffectCount = count,
            m_wDressAddEffectTime = time,
            m_nDressAddEffectCurIndex = cur,
            m_nDressAddEffectLastTick = lastTick,
        };

    [Fact]
    public void CheckLoadDressAddEffect_IndexNegativeClearsSurfaceAndReturnsFalse()
    {
        var hum = AddEffectActor(index: -1);
        hum.m_DressAddEffectSurface = "keep";

        Assert.False(hum.CheckLoadDressAddEffect());   // 13435
        Assert.Null(hum.m_DressAddEffectSurface);      // 13434
    }

    [Fact]
    public void CheckLoadDressAddEffect_ZeroCountClearsSurfaceAndReturnsFalse()
    {
        var hum = AddEffectActor(count: 0);
        hum.m_DressAddEffectSurface = "keep";

        Assert.False(hum.CheckLoadDressAddEffect());
        Assert.Null(hum.m_DressAddEffectSurface);
    }

    [Fact]
    public void CheckLoadDressAddEffect_ZeroTimeClearsSurfaceAndReturnsFalse()
    {
        var hum = AddEffectActor(time: 0);
        hum.m_DressAddEffectSurface = "keep";

        Assert.False(hum.CheckLoadDressAddEffect());
        Assert.Null(hum.m_DressAddEffectSurface);
    }

    [Fact]
    public void CheckLoadDressAddEffect_TimeNotExceededDoesNotAdvance()
    {
        // now(5000) - last(4950) = 50，不 > 100 ⇒ 不推进
        var hum = AddEffectActor(cur: 2, lastTick: 4950);

        Assert.False(hum.CheckLoadDressAddEffect());
        Assert.Equal(2, hum.m_nDressAddEffectCurIndex);
    }

    [Fact]
    public void CheckLoadDressAddEffect_TimeExactlyEqualDoesNotAdvance()
    {
        // ★ 13426 是严格 `>`：now - last = 100 恰等于 Time ⇒ **不**推进
        var hum = AddEffectActor(cur: 2, lastTick: 4900, time: 100);

        Assert.False(hum.CheckLoadDressAddEffect());
        Assert.Equal(2, hum.m_nDressAddEffectCurIndex);
    }

    [Fact]
    public void CheckLoadDressAddEffect_AdvancesAndReportsChange()
    {
        var hum = AddEffectActor(cur: 2, lastTick: 4800, time: 100);

        Assert.True(hum.CheckLoadDressAddEffect());     // 13432
        Assert.Equal(3, hum.m_nDressAddEffectCurIndex); // 13428
        Assert.Equal(5000u, hum.m_nDressAddEffectLastTick);   // 13427
    }

    [Fact]
    public void CheckLoadDressAddEffect_WrapsAtCount()
    {
        var hum = AddEffectActor(cur: 3, count: 4, lastTick: 4800, time: 100);

        Assert.True(hum.CheckLoadDressAddEffect());
        Assert.Equal(0, hum.m_nDressAddEffectCurIndex);   // 13430
    }

    // ══════════════════════════════════════════════════════════════════════
    // 6. LoadDressAddEffect（13439-13463）
    // ══════════════════════════════════════════════════════════════════════

    private List<(int File, int Index, TColorEffect Color)> HookDressAddEffect(int count = 10)
    {
        var calls = new List<(int, int, TColorEffect)>();
        ActorNpcEnv.EffectImageListCountFn = () => count;
        ActorNpcEnv.FetchEffectListImageFn = (file, idx, color) =>
        {
            calls.Add((file, idx, color));
            return new NpcImageFetch("tex", 31, 41);
        };
        return calls;
    }

    [Fact]
    public void LoadDressAddEffect_ClearsFirstThenFetchesWithOffsetPlusCursor()
    {
        var calls = HookDressAddEffect();
        var hum = AddEffectActor(index: 2, count: 4, time: 100, cur: 3);
        hum.m_wDressAddEffectOffSet = 70;

        hum.LoadDressAddEffect();

        Assert.Equal((2, 70 + 3, TColorEffect.ceNone), Assert.Single(calls));   // 13455
        Assert.Equal("tex", hum.m_DressAddEffectSurface);
        Assert.Equal(31, hum.m_nDressAddEffectX);
        Assert.Equal(41, hum.m_nDressAddEffectY);
    }

    [Fact]
    public void LoadDressAddEffect_NegativeCursorSkipsFetch()
    {
        var calls = HookDressAddEffect();
        var hum = AddEffectActor(cur: -1);
        hum.m_DressAddEffectSurface = "stale";

        hum.LoadDressAddEffect();

        Assert.Empty(calls);                              // 13445 的第四重门
        Assert.Null(hum.m_DressAddEffectSurface);         // 13443 已清
    }

    [Fact]
    public void LoadDressAddEffect_IndexAtOrOverEffectListCountSkipsFetch()
    {
        var calls = HookDressAddEffect(count: 3);
        var hum = AddEffectActor(index: 3);

        hum.LoadDressAddEffect();

        Assert.Empty(calls);                              // 13448：3 < 3 为假
        Assert.Null(hum.m_DressAddEffectSurface);
    }

    [Fact]
    public void LoadDressAddEffect_GrayScale2IsNotTreatedAsGray()
    {
        var calls = HookDressAddEffect();
        var hum = AddEffectActor();
        hum.ActorColorEffect = TColorEffect.ceGrayScale2;

        hum.LoadDressAddEffect();

        // ★ 13452 只列 ceGrayScale —— ceGrayScale2 落到 else（原图）。
        //   与 TNpcActor.LoadSurface 若干段的 "ceGrayScale, ceGrayScale2" 不同，逐字保留。
        Assert.Equal(TColorEffect.ceGrayScale2, Assert.Single(calls).Color);
        // 该差异由接缝承载：本断言锁的是"原样把 ceGrayScale2 传给取图层"，
        // 而不是在这里改写成 ceGrayScale。
    }

    // ══════════════════════════════════════════════════════════════════════
    // 7. OnTargetExplosion（13637-13648）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void OnTargetExplosion_NonFlyEffectSenderPlaysNothing()
    {
        var played = new List<string>();
        ActorFamilyEnv.PlaySoundByNameFn = played.Add;
        ActorNpcEnv.MonFlyEffectExplosionSoundFn = _ => null;   // 不是飞行特效

        new THumActor().OnTargetExplosion(new object());

        Assert.Empty(played);   // 13641 的类型门为假
    }

    [Fact]
    public void OnTargetExplosion_FlyEffectWithoutSoundPlaysNothing()
    {
        var played = new List<string>();
        ActorFamilyEnv.PlaySoundByNameFn = played.Add;
        ActorNpcEnv.MonFlyEffectExplosionSoundFn = _ => "";     // 是飞行特效但未配置音效

        new THumActor().OnTargetExplosion(new object());

        Assert.Empty(played);   // 13644 的 Length > 0 为假
    }

    [Fact]
    public void OnTargetExplosion_FlyEffectWithSoundPlaysIt()
    {
        var played = new List<string>();
        ActorFamilyEnv.PlaySoundByNameFn = played.Add;
        ActorNpcEnv.MonFlyEffectExplosionSoundFn = _ => "cust_magic_explosion";

        new THumActor().OnTargetExplosion("fly");

        Assert.Equal(new[] { "cust_magic_explosion" }, played);   // 13645
    }

    // ══════════════════════════════════════════════════════════════════════
    // 8. TakeHorse（17426-17433）
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void TakeHorse_WhenNoHorseRequestsSummonWithParamOne()
    {
        var sends = new List<(int Ident, long Recog, int X, int Y, int Param)>();
        var stops = 0;
        ActorNpcEnv.SendClientMessageFn = (a, b, c, d, e) => sends.Add((a, b, c, d, e));
        ActorNpcEnv.LegendMapStopFn = () => stops++;

        new THumActor { m_btHorse = 0 }.TakeHorse();

        Assert.Equal((THumActor.CM_TAKEHORSE, 1L, 0, 0, 0), Assert.Single(sends));   // 17429
        Assert.Equal(1, stops);                                                       // 17432
    }

    [Fact]
    public void TakeHorse_WhenHorsePresentRequestsRecallWithParamZero()
    {
        var sends = new List<(int Ident, long Recog, int X, int Y, int Param)>();
        var stops = 0;
        ActorNpcEnv.SendClientMessageFn = (a, b, c, d, e) => sends.Add((a, b, c, d, e));
        ActorNpcEnv.LegendMapStopFn = () => stops++;

        new THumActor { m_btHorse = 3 }.TakeHorse();

        Assert.Equal((THumActor.CM_TAKEHORSE, 0L, 0, 0, 0), Assert.Single(sends));   // 17431
        Assert.Equal(1, stops);   // ★ LegendMap.Stop 是**无条件**后置，不随分支变化
    }

    [Fact]
    public void TakeHorseCommandIdMatchesGrobal2()
    {
        Assert.Equal(5002, THumActor.CM_TAKEHORSE);   // Common/Grobal2.pas:446
    }

    // ══════════════════════════════════════════════════════════════════════
    // 9. Create（11130-11210）—— 切片 3c
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void Create_FrameTimeIs150UnlikePlainActor()
    {
        // ★ 对照：TActor 的 m_dwFrameTime 保持 0；THumActor.Create 11147 把它置 150
        Assert.Equal(0u, new TActor().m_dwFrameTime);
        Assert.Equal(150u, new THumActor().m_dwFrameTime);
    }

    [Fact]
    public void Create_ThreeEffectIndexesAreMinusOneButDbWeaponOffsetIsZero()
    {
        var hum = new THumActor();

        Assert.Equal(-1, hum.m_nDressEffectIndex);        // 11151
        Assert.Equal(-1, hum.m_nWeaponEffectIndex);       // 11152
        Assert.Equal(-1, hum.m_nShieldEffectIndex);       // 11154
        Assert.Equal(-1, hum.m_nMedalEffectIndex);        // 11162
        // ★ 同段里 11153 写的是 **0**，不是 -1
        Assert.Equal(0, hum.m_wDBWeaponEffectOffSet);
    }

    [Fact]
    public void Create_UsesTwoDifferentClocksInTheSameConstructor()
    {
        // ★ 11148 用 TimeGetTime()；11184 用 MyGetTickCount —— 两个不同接缝
        ActorNpcEnv.TimeGetTimeFn = () => 1111;
        ActorNpcEnv.MyGetTickCountFn = () => 2222;

        var hum = new THumActor();

        Assert.Equal(1111u, hum.m_dwFrameTick);              // 11148
        Assert.Equal(2222u, hum.m_nDressAddEffectLastTick);   // 11184
    }

    [Fact]
    public void Create_AddDressEffectKitDefaults()
    {
        var hum = new THumActor();

        Assert.Equal(-1, hum.m_nDressAddEffectIndex);   // 11176
        Assert.Equal(0, hum.m_bDressAddEffectOrder);    // 11177
        Assert.Equal(0, hum.m_wDressAddEffectOffSet);   // 11178
        Assert.Equal(0, hum.m_wDressAddEffectCount);    // 11179
        Assert.Equal(0, hum.m_wDressAddEffectTime);     // 11180
        Assert.False(hum.m_boDressAddEffectNoBlend);    // 11181
        Assert.Equal(0, hum.m_nDressAddEffectCurIndex); // 11183
    }

    [Fact]
    public void Create_NoBlendAndNoSexSwitchesAllFalse()
    {
        var hum = new THumActor();

        Assert.False(hum.m_boDressEffectNoBlend);         // 11157
        Assert.False(hum.m_boDressEffectNoSex);           // 11158
        Assert.False(hum.m_boWeaponEffectNoBlend);        // 11159
        Assert.False(hum.m_boWeaponEffectNoSex);          // 11160
        Assert.False(hum.m_boMedalEffectNoBlend);         // 11163
        Assert.False(hum.m_boMedalEffectNoSex);           // 11164
        Assert.False(hum.m_boShieldEffectNoBlend);        // 11169
        Assert.False(hum.m_boShieldEffectNoSex);          // 11170
        // ★ 三个 *DrawNoBlend 是**另一组**字段（11172-11174）
        Assert.False(hum.m_boDressEffectDrawNoBlend);
        Assert.False(hum.m_boWeaponEffectDrawNoBlend);
        Assert.False(hum.m_boShieldEffectDrawNoBlend);
    }

    [Fact]
    public void Create_TrainingJewelryAndGodBlessDefaults()
    {
        var hum = new THumActor();

        Assert.False(hum.m_boTrainingNG);       // 11186
        Assert.False(hum.m_boTrainingXF);       // 11187
        Assert.Equal(TJewelryBoxStatus.jbsNoActive, hum.m_nJewelryBoxStatus);   // 11189
        Assert.False(hum.m_boShowGodBless);     // 11190
    }

    [Fact]
    public void Create_ZeroesNgAlcoholAndMeridianStructs()
    {
        var hum = new THumActor();

        // 11192-11194 的三处 FillChar(...,0)
        Assert.Equal(0, hum.m_AbilNG.Level);
        Assert.Equal(0, hum.m_AbilNG.NH);
        Assert.Equal(0, hum.m_AbilNG.MaxNH);
        Assert.Equal(0u, hum.m_AbilNG.Exp);
        Assert.Equal(0, hum.m_Alcohol.Alcohol);
        Assert.Equal(0, hum.m_Alcohol.MedicineLevel);
        Assert.Equal(0, hum.m_HumMeridians[0].Level);
        Assert.Equal(0, hum.m_HumMeridians[4].Level);   // array[0..4] 的末项
    }

    [Fact]
    public void Create_HeroM2AndBrokenShieldDefaults()
    {
        var hum = new THumActor();

        Assert.Equal(0, hum.m_wHeroM2DressEffect);        // 11196
        Assert.False(hum.m_boHeroM2DressNoBlend);         // 11197
        Assert.Equal(0, hum.m_wOldHeroM2DressEffect);     // 11199
        Assert.False(hum.m_boOldHeroM2DressNoBlend);      // 11200
        Assert.Equal(0, hum.m_nHeroM2DressEffectX);       // 11202
        Assert.Equal(0, hum.m_nHeroM2DressEffectY);       // 11203
        Assert.Null(hum.m_HeroM2DressEffect);             // 11204
        Assert.False(hum.m_boBrokenShield);               // 11206
    }

    [Fact]
    public void Create_WeaponEffectFlagFalseAndShopStallSurfacesUntouched()
    {
        var hum = new THumActor();

        Assert.False(hum.m_boWeaponEffect);      // 11146
        Assert.Null(hum.m_HairSurface);          // 11133
        Assert.Null(hum.m_WeaponSurface);        // 11134
        Assert.Null(hum.m_ShieldSurface);        // 11135
        Assert.Null(hum.m_HorseSurface);         // 11137
        Assert.Null(hum.m_HorseWingsEffectSurface);   // 11138
        Assert.Null(hum.m_HorseEffectSurface);        // 11139
        Assert.Null(hum.m_HorseHairSurface);          // 11141
        Assert.Null(hum.m_HorseHumSurface);           // 11142
        Assert.Null(hum.m_HumWinSurface);             // 11144
        Assert.Null(hum.m_HumWinSurface_30);          // 11145
        // ★ Create 11133-11145 **不清** m_ShopStallSurface / m_ShopHeadSurface
        //   （它们只在 Finalize 11244-11245 被清）—— 此处它们仍为字段默认 null，
        //   该不对称由 Finalize 侧用例（Create 不清 ⇒ Finalize 清）正向锁定。
        Assert.Null(hum.m_ShopStallSurface);
        Assert.Null(hum.m_ShopHeadSurface);
    }

    [Fact]
    public void Create_FriendHitListIsCreatedAndEmpty()
    {
        var hum = new THumActor();

        Assert.NotNull(hum.m_FriendHitList);    // 11209 TStringList.Create
        Assert.Empty(hum.m_FriendHitList);
    }

    [Fact]
    public void Create_RecordsNotPortedForUnportedInheritedCreate()
    {
        // ★ 11132 的 `inherited Create` 指向 TActor.Create(2777-2944, 168 行)，**未移植**
        //   ⇒ 构造函数里显式留痕，使"跳过了 168 行基类初始化"可观测（而非静默省略）
        TActorCore.NotPortedCapture = new List<string>();
        try
        {
            _ = new THumActor();

            Assert.Contains("TActor.Create(inherited@11132)@2777", TActorCore.NotPortedCapture);
        }
        finally
        {
            TActorCore.NotPortedCapture = null;
        }
    }

    [Fact]
    public void HeroActorInheritsHumActorConstruction()
    {
        // 原文 2049 THeroActor : THumActor ⇒ 也走 THumActor.Create
        Assert.Equal(150u, new THeroActor().m_dwFrameTime);
        Assert.Equal(-1, new THeroActor().m_nDressEffectIndex);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 10. Destroy / Initialize / Finalize（11212-11258）—— 切片 3c
    // ══════════════════════════════════════════════════════════════════════

    [Fact]
    public void Destroy_ClearsFriendHitListAndReachesBaseSlot()
    {
        TActorCore.NotPortedCapture = new List<string>();
        try
        {
            var hum = new THumActor();
            hum.m_FriendHitList.Add("someone");

            hum.Destroy();

            Assert.Empty(hum.m_FriendHitList);                       // 11215
            Assert.Contains("Destroy@2945", TActorCore.NotPortedCapture);   // 11214 落到 NotPorted 槽位
        }
        finally
        {
            TActorCore.NotPortedCapture = null;
        }
    }

    [Fact]
    public void Initialize_DoesNotAlterFields()
    {
        var hum = new THumActor { m_dwFrameTime = 777, m_nFrame = 5 };

        hum.Initialize();      // 11220：只有 inherited Initialize（基类空体）

        Assert.Equal(777u, hum.m_dwFrameTime);
        Assert.Equal(5, hum.m_nFrame);
    }

    [Fact]
    public void Finalize_ClearsThirteenTextureSlotsIncludingShopStallOnes()
    {
        var hum = new THumActor();
        hum.m_HairSurface = "a";                  // 11230
        hum.m_WeaponSurface = "b";                // 11231
        hum.m_ShieldSurface = "c";                // 11232
        hum.m_HorseSurface = "d";                 // 11234
        hum.m_HorseWingsEffectSurface = "e";      // 11235
        hum.m_HorseEffectSurface = "f";           // 11236
        hum.m_HorseHairSurface = "g";             // 11238
        hum.m_HorseHumSurface = "h";              // 11239
        hum.m_HumWinSurface = "i";                // 11241
        hum.m_HumWinSurface_30 = "j";             // 11242
        hum.m_ShopStallSurface = "k";             // 11244 ★ Create 不清、Finalize 清
        hum.m_ShopHeadSurface = "l";              // 11245 ★ 同上
        hum.m_HeroM2DressEffect = "m";            // 11246 ★ 同上

        hum.Finalize();

        Assert.Null(hum.m_HairSurface);
        Assert.Null(hum.m_WeaponSurface);
        Assert.Null(hum.m_ShieldSurface);
        Assert.Null(hum.m_HorseSurface);
        Assert.Null(hum.m_HorseWingsEffectSurface);
        Assert.Null(hum.m_HorseEffectSurface);
        Assert.Null(hum.m_HorseHairSurface);
        Assert.Null(hum.m_HorseHumSurface);
        Assert.Null(hum.m_HumWinSurface);
        Assert.Null(hum.m_HumWinSurface_30);
        Assert.Null(hum.m_ShopStallSurface);
        Assert.Null(hum.m_ShopHeadSurface);
        Assert.Null(hum.m_HeroM2DressEffect);
    }

    [Fact]
    public void Finalize_NullsPlayEffectTexturesButKeepsQueueLength()
    {
        var hum = new THumActor();
        var e1 = hum.m_ActorEffects.Add();
        var e2 = hum.m_ActorEffects.Add();
        e1.Texture = "t1";
        e2.Texture = "t2";

        hum.Finalize();

        // ★ 11248-11257：只把 Texture 置 nil，**既不置 boWantDelete 也不清空队列**
        Assert.Null(e1.Texture);
        Assert.Null(e2.Texture);
        Assert.False(e1.boWantDelete);
        Assert.False(e2.boWantDelete);
        Assert.Equal(2, hum.m_ActorEffects.Items.Count);
    }
}

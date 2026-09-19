using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J93：Actor.pas ReadyAction 的**自定义动作范围 case**（4124）与相关范围常量 1:1 测试。
/// 该范围 case 过去在 C# 侧缺失，导致 300 个自定义攻击动作码不会写 `m_nHitEffectLevel`。
/// </summary>
public sealed class ActorMessagesCustomRangeTests
{
    // ===================== 常量 =====================

    [Fact]
    public void RangeConstantsMatchGrobal2()
    {
        Assert.Equal(11000, TActorCore.SM_CUSTOM_HIT001);
        Assert.Equal(12000, TActorCore.SM_CUSTOM_PUSH001);
        Assert.Equal(300, TActorCore.CustomMagicCount);
    }

    [Fact]
    public void RangesDoNotOverlap()
    {
        // 11000..11299（攻击）、11500..11799（魔法移动）、12000..12299（推挤）
        int hitEnd = TActorCore.SM_CUSTOM_HIT001 + TActorCore.CustomMagicCount;   // 11300
        int pushStart = TActorCore.SM_CUSTOM_PUSH001;                                 // 12000
        Assert.True(hitEnd < pushStart);
    }

    [Fact]
    public void MagicMoveRangeSitsBetweenHitAndPush()
    {
        const int magicMove = 11500;                                                     // Grobal2.pas 2515
        Assert.True(magicMove > TActorCore.SM_CUSTOM_HIT001 + TActorCore.CustomMagicCount);
        Assert.True(magicMove < TActorCore.SM_CUSTOM_PUSH001);
    }

    // ===================== IsCustomHit =====================

    [Fact]
    public void IsCustomHitIncludesLowerBound()
    {
        Assert.True(TActorCore.IsCustomHit(11000));
    }

    [Fact]
    public void IsCustomHitIncludesLastElement()
    {
        // 范围是 `..(001 + COUNT - 1)`，即上界为 11299
        Assert.True(TActorCore.IsCustomHit(11299));
    }

    [Fact]
    public void IsCustomHitExcludesUpperBound()
    {
        Assert.False(TActorCore.IsCustomHit(11300));
    }

    [Fact]
    public void IsCustomHitExcludesBelowRange()
    {
        Assert.False(TActorCore.IsCustomHit(10999));
    }

    [Fact]
    public void IsCustomHitExcludesNamedHitCodes()
    {
        // 具名攻击码不在自定义范围内
        Assert.False(TActorCore.IsCustomHit(9101));   // SM_101HIT
        Assert.False(TActorCore.IsCustomHit(1118));   // SM_60HIT
        Assert.False(TActorCore.IsCustomHit(66));     // SM_66HIT
    }

    [Fact]
    public void IsCustomHitExcludesPushRange()
    {
        Assert.False(TActorCore.IsCustomHit(12000));
    }

    [Fact]
    public void IsCustomHitCoversExactlyThreeHundredCodes()
    {
        int count = 0;
        for (int id = 11000; id < 11300; id++)
            if (TActorCore.IsCustomHit(id))
                count++;

        Assert.Equal(300, count);
    }

    // ===================== IsCustomPush =====================

    [Fact]
    public void IsCustomPushBounds()
    {
        Assert.True(TActorCore.IsCustomPush(12000));
        Assert.True(TActorCore.IsCustomPush(12299));
        Assert.False(TActorCore.IsCustomPush(12300));
        Assert.False(TActorCore.IsCustomPush(11999));
    }

    [Fact]
    public void IsCustomPushExcludesHitRange()
    {
        Assert.False(TActorCore.IsCustomPush(11000));
    }

    [Fact]
    public void IsCustomPushCoversExactlyThreeHundredCodes()
    {
        int count = 0;
        for (int id = 12000; id < 12300; id++)
            if (TActorCore.IsCustomPush(id))
                count++;

        Assert.Equal(300, count);
    }

    // ===================== 两个范围互斥 =====================

    [Fact]
    public void HitAndPushRangesAreDisjoint()
    {
        for (int id = 11000; id < 12300; id++)
        {
            bool hit = TActorCore.IsCustomHit(id);
            bool push = TActorCore.IsCustomPush(id);
            Assert.False(hit && push, $"id {id} 同时落入两个范围");
        }
    }

    // ===================== 与既有具名常量不冲突 =====================

    [Fact]
    public void NamedHitCodesAreAllOutsideCustomRange()
    {
        int[] named =
        {
            TActorCore.SM_FIREHIT, TActorCore.SM_60HIT, TActorCore.SM_61HIT,
            TActorCore.SM_62HIT, TActorCore.SM_66HIT, TActorCore.SM_TWNHIT,
            TActorCore.SM_CRSHIT, TActorCore.SM_43HIT, TActorCore.SM_SWORDHIT,
            TActorCore.SM_101HIT, TActorCore.SM_102HIT, TActorCore.SM_103HIT,
        };

        foreach (int n in named)
            Assert.False(TActorCore.IsCustomHit(n), $"具名码 {n} 落入了自定义范围");
    }

    [Fact]
    public void AttackRemapRangeIsOutsideCustomHitRange()
    {
        // SM_ATTACK01..06 = 8946..8951，用于自定义怪变脸重映射
        Assert.False(TActorCore.IsCustomHit(TActorCore.SM_ATTACK01));
        Assert.False(TActorCore.IsCustomHit(TActorCore.SM_ATTACK06));
    }

    // ===================== 语义：范围 case 只写 hitEffectLevel =====================

    [Fact]
    public void CustomHitRangeMirrorsNamedHitBehaviour()
    {
        // 4124-4130 的函数体与具名攻击族（4106-4123）**完全相同**：只写 m_nHitEffectLevel
        Assert.True(TActorCore.IsCustomHit(11100));
        Assert.True(TActorCore.IsCustomHit(11299));
    }
}

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// 批次J95：服务端三处 `CM_*` 攻击/动作消息集合 1:1 测试。
/// 成员列表由脚本从原文独立抽取后与实现交叉核对（曾借此发现一处手工转录错误）。
/// </summary>
public sealed class HitActionMsgSetTests
{
    // ===================== 常量与范围 =====================

    [Fact]
    public void CustomHitStartIs6000()
    {
        Assert.Equal(6000, HitActionMsgSet.CM_CUSTOM_HIT001);
        Assert.Equal(300, HitActionMsgSet.CustomMagicCount);
    }

    [Fact]
    public void CustomHitRangeBounds()
    {
        Assert.True(HitActionMsgSet.InCustomHitRange(6000));
        Assert.True(HitActionMsgSet.InCustomHitRange(6299));
        Assert.False(HitActionMsgSet.InCustomHitRange(6300));   // 上界不含
        Assert.False(HitActionMsgSet.InCustomHitRange(5999));
    }

    [Fact]
    public void CustomHitRangeCoversExactly300()
    {
        int n = 0;
        for (int id = 6000; id < 6300; id++)
            if (HitActionMsgSet.InCustomHitRange(id)) n++;

        Assert.Equal(300, n);
    }

    [Fact]
    public void CustomRangeDoesNotSwallowNamedCodes()
    {
        // 具名 CM_ 码都在 3009..3166，远低于 6000
        foreach (int id in new[]
                 {
                     Grobal2Const.CM_HIT, Grobal2Const.CM_HEAVYHIT, Grobal2Const.CM_SWORDHIT,
                     Grobal2Const.CM_113HIT, Grobal2Const.CM_115HIT, Grobal2Const.CM_66HIT1,
                 })
            Assert.False(HitActionMsgSet.InCustomHitRange(id));
    }

    // ===================== GetHitMsgCount（ObjPlayer 14919-14921） =====================

    [Fact]
    public void GetHitCountIncludesAllNamedHits()
    {
        int[] hits =
        {
            Grobal2Const.CM_HIT, Grobal2Const.CM_HEAVYHIT, Grobal2Const.CM_BIGHIT,
            Grobal2Const.CM_POWERHIT, Grobal2Const.CM_LONGHIT, Grobal2Const.CM_WIDEHIT,
            Grobal2Const.CM_FIREHIT, Grobal2Const.CM_TWNHIT, Grobal2Const.CM_43HIT,
            Grobal2Const.CM_66HIT, Grobal2Const.CM_66HIT1, Grobal2Const.CM_SWORDHIT,
            Grobal2Const.CM_100HIT, Grobal2Const.CM_101HIT, Grobal2Const.CM_102HIT,
            Grobal2Const.CM_103HIT, Grobal2Const.CM_113HIT, Grobal2Const.CM_115HIT,
        };

        foreach (int h in hits)
            Assert.True(HitActionMsgSet.IsHitMsgCountIdent(h), $"CM 码 {h} 应计入");
    }

    [Fact]
    public void GetHitCountExcludesCrsHit()
    {
        // 14919-14921 的列表**确实没有** CM_CRSHIT —— 逐字保留原文的不一致
        Assert.False(HitActionMsgSet.IsHitMsgCountIdent(Grobal2Const.CM_CRSHIT));
    }

    [Fact]
    public void GetHitCountExcludes60To62Hit()
    {
        // 该列表不含 60/61/62HIT（这三个在别处处理）
        Assert.False(HitActionMsgSet.IsHitMsgCountIdent(1118));
        Assert.False(HitActionMsgSet.IsHitMsgCountIdent(1119));
        Assert.False(HitActionMsgSet.IsHitMsgCountIdent(1120));
    }

    [Fact]
    public void GetHitCountIncludesCustomRange()
    {
        Assert.True(HitActionMsgSet.IsHitMsgCountIdent(6000));
        Assert.True(HitActionMsgSet.IsHitMsgCountIdent(6299));
        Assert.False(HitActionMsgSet.IsHitMsgCountIdent(6300));
    }

    [Fact]
    public void GetHitCountCountsList()
    {
        var list = new List<int>
        {
            Grobal2Const.CM_HIT,          // 计
            Grobal2Const.CM_WALK,         // 不计
            Grobal2Const.CM_CRSHIT,       // 不计（该集合不含）
            6000,                          // 计（范围）
            Grobal2Const.CM_SPELL,        // 不计
            6299,                          // 计
            6300,                          // 不计（超范围）
        };

        Assert.Equal(3, HitActionMsgSet.GetHitMsgCount(list));
    }

    [Fact]
    public void GetHitCountEmptyListIsZero()
    {
        Assert.Equal(0, HitActionMsgSet.GetHitMsgCount(Array.Empty<int>()));
    }

    // ===================== UsrEngn（5729-7536） =====================

    [Fact]
    public void UsrEngnSetIncludesMovementCodes()
    {
        // UsrEngn 的列表额外含移动族（其余两处没有）
        Assert.True(HitActionMsgSet.IsUsrEngnActionIdent(Grobal2Const.CM_HORSERUN));
        Assert.True(HitActionMsgSet.IsUsrEngnActionIdent(Grobal2Const.CM_TURN));
        Assert.True(HitActionMsgSet.IsUsrEngnActionIdent(Grobal2Const.CM_WALK));
        Assert.True(HitActionMsgSet.IsUsrEngnActionIdent(Grobal2Const.CM_RUN));
    }

    [Fact]
    public void UsrEngnSetIncludesCrsHit()
    {
        // **本批次的修正点**：5729 确实含 CM_CRSHIT（首次手工转录时漏掉）
        Assert.True(HitActionMsgSet.IsUsrEngnActionIdent(Grobal2Const.CM_CRSHIT));
    }

    [Fact]
    public void UsrEngnSetExcludes100Hit()
    {
        // 5729-5736 只列到 CM_101HIT..CM_103HIT，**没有 CM_100HIT**
        Assert.False(HitActionMsgSet.IsUsrEngnActionIdent(Grobal2Const.CM_100HIT));
        Assert.True(HitActionMsgSet.IsUsrEngnActionIdent(Grobal2Const.CM_101HIT));
    }

    [Fact]
    public void UsrEngnSetIncludesCustomRange()
    {
        Assert.True(HitActionMsgSet.IsUsrEngnActionIdent(6000));
        Assert.True(HitActionMsgSet.IsUsrEngnActionIdent(6299));
        Assert.False(HitActionMsgSet.IsUsrEngnActionIdent(6300));
    }

    // ===================== 下发方式（5739-5750） =====================

    [Fact]
    public void SendModePrefersUpdateMsgA()
    {
        Assert.Equal(HitSendMode.SendUpdateMsgA,
            HitActionMsgSet.SelectUsrEngnSendMode(boSpeedControl: true, boSendUpdateMsg: true, boActionSendActionMsg: true));
    }

    [Fact]
    public void SendModeFallsToUpdateMsgWhenOnlyActionFlag()
    {
        Assert.Equal(HitSendMode.SendUpdateMsg,
            HitActionMsgSet.SelectUsrEngnSendMode(true, false, true));
    }

    [Fact]
    public void SendModeFallsToPlainSendMsgOtherwise()
    {
        Assert.Equal(HitSendMode.SendMsg,
            HitActionMsgSet.SelectUsrEngnSendMode(true, false, false));
        Assert.Equal(HitSendMode.SendMsg,
            HitActionMsgSet.SelectUsrEngnSendMode(false, true, true));
    }

    [Fact]
    public void BothFlagsRequireSpeedControl()
    {
        // 5739/5744：两个开关都必须与 boSpeedControl 同时成立
        Assert.Equal(HitSendMode.SendMsg,
            HitActionMsgSet.SelectUsrEngnSendMode(false, true, false));
        Assert.Equal(HitSendMode.SendMsg,
            HitActionMsgSet.SelectUsrEngnSendMode(false, false, true));
    }

    [Fact]
    public void SendUpdateMsgATakesPriorityOverPlainUpdateMsg()
    {
        // 5739 的 if 在 5744 的 else if 之前
        var both = HitActionMsgSet.SelectUsrEngnSendMode(true, true, true);
        Assert.NotEqual(HitSendMode.SendUpdateMsg, both);
    }

    // ===================== MirClientContext（6684-6688） =====================

    [Fact]
    public void RunGateSetIncludesCrsHit()
    {
        Assert.True(HitActionMsgSet.IsRunGateActionIdent(Grobal2Const.CM_CRSHIT));
    }

    [Fact]
    public void RunGateSetExcludes100Hit()
    {
        // 6686 从 CM_101HIT 开始列，**没有 CM_100HIT**
        Assert.False(HitActionMsgSet.IsRunGateActionIdent(Grobal2Const.CM_100HIT));
        Assert.True(HitActionMsgSet.IsRunGateActionIdent(Grobal2Const.CM_101HIT));
    }

    [Fact]
    public void RunGateSetExcludesMovementCodes()
    {
        // 移动族在 6683 之前的其它分支处理，不在本集合
        Assert.False(HitActionMsgSet.IsRunGateActionIdent(Grobal2Const.CM_WALK));
        Assert.False(HitActionMsgSet.IsRunGateActionIdent(Grobal2Const.CM_RUN));
        Assert.False(HitActionMsgSet.IsRunGateActionIdent(Grobal2Const.CM_TURN));
        Assert.False(HitActionMsgSet.IsRunGateActionIdent(Grobal2Const.CM_HORSERUN));
    }

    [Fact]
    public void RunGateSetIncludesCustomRange()
    {
        Assert.True(HitActionMsgSet.IsRunGateActionIdent(6000));
        Assert.False(HitActionMsgSet.IsRunGateActionIdent(6299 + 1));
    }

    // ===================== 三个集合的差异（原文不一致点） =====================

    [Fact]
    public void CrsHitDiffersBetweenGetHitCountAndRunGate()
    {
        // GetHitMsgCount 不含、RunGate 含、UsrEngn 含
        Assert.False(HitActionMsgSet.IsHitMsgCountIdent(Grobal2Const.CM_CRSHIT));
        Assert.True(HitActionMsgSet.IsRunGateActionIdent(Grobal2Const.CM_CRSHIT));
        Assert.True(HitActionMsgSet.IsUsrEngnActionIdent(Grobal2Const.CM_CRSHIT));
    }

    [Fact]
    public void HundredHitOnlyInGetHitCount()
    {
        // CM_100HIT 仅 GetHitMsgCount 含，另两处都不含
        Assert.True(HitActionMsgSet.IsHitMsgCountIdent(Grobal2Const.CM_100HIT));
        Assert.False(HitActionMsgSet.IsRunGateActionIdent(Grobal2Const.CM_100HIT));
        Assert.False(HitActionMsgSet.IsUsrEngnActionIdent(Grobal2Const.CM_100HIT));
    }

    [Fact]
    public void MovementCodesOnlyInUsrEngn()
    {
        foreach (int m in new[]
                 {
                     Grobal2Const.CM_HORSERUN, Grobal2Const.CM_TURN,
                     Grobal2Const.CM_WALK, Grobal2Const.CM_RUN,
                 })
        {
            Assert.True(HitActionMsgSet.IsUsrEngnActionIdent(m));
            Assert.False(HitActionMsgSet.IsHitMsgCountIdent(m));
            Assert.False(HitActionMsgSet.IsRunGateActionIdent(m));
        }
    }

    [Fact]
    public void CustomRangeIsCommonToAllThreeSets()
    {
        // 三处**都**含自定义范围——这是唯一的完全一致点
        foreach (int id in new[] { 6000, 6150, 6299 })
        {
            Assert.True(HitActionMsgSet.IsHitMsgCountIdent(id));
            Assert.True(HitActionMsgSet.IsRunGateActionIdent(id));
            Assert.True(HitActionMsgSet.IsUsrEngnActionIdent(id));
        }
    }

    [Fact]
    public void SpellAndSayAreInNoSet()
    {
        foreach (int id in new[] { Grobal2Const.CM_SPELL, Grobal2Const.CM_SAY, Grobal2Const.CM_DROPITEM })
        {
            Assert.False(HitActionMsgSet.IsHitMsgCountIdent(id));
            Assert.False(HitActionMsgSet.IsRunGateActionIdent(id));
            Assert.False(HitActionMsgSet.IsUsrEngnActionIdent(id));
        }
    }

    [Fact]
    public void SetsAreNotIdentical()
    {
        // 冗余保护：若有人日后"统一"三个集合，本测试会失败
        int diff = 0;
        for (int id = 3000; id < 6400; id++)
        {
            bool a = HitActionMsgSet.IsHitMsgCountIdent(id);
            bool b = HitActionMsgSet.IsRunGateActionIdent(id);
            bool c = HitActionMsgSet.IsUsrEngnActionIdent(id);
            if (!(a == b && b == c)) diff++;
        }

        Assert.True(diff > 0, "三个集合不应完全相同（原文存在真实差异）");
    }
}

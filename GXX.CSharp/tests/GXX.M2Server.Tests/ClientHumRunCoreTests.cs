using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J182：`THumActor.Run` 1:1 测试（436 行），并与同单元
/// `TActor.Run`（282 行）与 `TNpcActor.Run`（158 行）三方对照。
/// **继承链不对称（只有 NPC 版调 inherited）、
/// 两处 MagicTimeOut 逻辑同写法异、以及十成员析取的顺序差。**
/// </summary>
public sealed class ClientHumRunCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(3000, ClientHumRunCore.MagicTimeoutSelf);
        Assert.Equal(2000, ClientHumRunCore.MagicTimeoutOther);
        Assert.Equal(300, ClientHumRunCore.CustomMagicCount);
        Assert.Equal(5, ClientHumRunCore.SM_HORSERUN);
        Assert.Equal(13, ClientHumRunCore.SM_RUN);
        Assert.Equal(17, ClientHumRunCore.SM_SPELL);
        Assert.Equal(5354, ClientHumRunCore.SM_MAGICMOVE);
        Assert.Equal(11500, ClientHumRunCore.SM_CUSTOM_MAGICMOVE001);
        Assert.Equal(12000, ClientHumRunCore.SM_CUSTOM_PUSH001);
        Assert.Equal(9100, ClientHumRunCore.SM_100HIT);
    }

    // ===================== 一、继承链 =====================

    [Fact]
    public void InheritanceChain()
    {
        Assert.True(ClientHumRunCore.BaseHasNoInherited());
        Assert.True(ClientHumRunCore.HumHasNoInherited());
        Assert.True(ClientHumRunCore.NpcCallsInherited());
        Assert.True(ClientHumRunCore.OnlyNpcCallsInherited());
        Assert.True(ClientHumRunCore.HumBypassesBase());
        Assert.True(ClientHumRunCore.InheritedThreeState());
    }

    [Fact]
    public void PositiveNamedPredicates()
    {
        // **正向命名的两个谓词，供组合使用**
        Assert.True(ClientHumRunCore.HumDoesNotCallInherited());
        Assert.True(ClientHumRunCore.BaseDoesNotCallInherited());
    }

    [Fact]
    public void BaseServesNpcOnly()
    {
        Assert.True(ClientHumRunCore.BaseRunServesNpcOnly());
        Assert.True(ClientHumRunCore.DeadCodeForHumans());
        Assert.True(ClientHumRunCore.SecondOccurrence());
    }

    // ===================== 二、MagicTimeOut =====================

    [Fact]
    public void MagicTimeOutPresence()
    {
        Assert.True(ClientHumRunCore.MagicTimeOutInBothRuns());
        Assert.True(ClientHumRunCore.SameLogic());
        Assert.True(ClientHumRunCore.ThreeThousandVsTwoThousand());
        Assert.True(ClientHumRunCore.DiffersOnlyInBraceStyle());
        Assert.True(ClientHumRunCore.NotCopyPaste());
        Assert.True(ClientHumRunCore.RewrittenSeparately());
    }

    [Fact]
    public void MagicTimeOutThresholds()
    {
        Assert.True(ClientHumRunCore.LocalPlayerMoreLenient());
        Assert.True(ClientHumRunCore.StrictGreater());
        Assert.True(ClientHumRunCore.StrictGreaterBoundary());
        Assert.True(ClientHumRunCore.ExactThresholdNotTimedOut());

        // **恰好等于阈值不算超时（严格大于）**
        Assert.False(ClientHumRunCore.IsMagicTimeout(3000, true));
        Assert.True(ClientHumRunCore.IsMagicTimeout(3001, true));
        Assert.False(ClientHumRunCore.IsMagicTimeout(2000, false));
        Assert.True(ClientHumRunCore.IsMagicTimeout(2001, false));

        // **本地玩家容忍更长**
        Assert.False(ClientHumRunCore.IsMagicTimeout(2500, true));
        Assert.True(ClientHumRunCore.IsMagicTimeout(2500, false));
    }

    [Fact]
    public void MagicTimeOutSideEffect()
    {
        Assert.True(ClientHumRunCore.HasSideEffect());
        Assert.True(ClientHumRunCore.PredicateMutatesState());
        Assert.True(ClientHumRunCore.SideEffectValues());

        Assert.Equal(0, ClientHumRunCore.ServerMagicCodeAfter(42, 4000, true));
        Assert.Equal(42, ClientHumRunCore.ServerMagicCodeAfter(42, 100, true));
    }

    // ===================== 三、动作析取 =====================

    [Fact]
    public void DisjunctionShape()
    {
        Assert.True(ClientHumRunCore.TenMemberDisjunction());
        Assert.True(ClientHumRunCore.EightSinglesTwoRanges());
        Assert.True(ClientHumRunCore.TwoHalfOpenRanges());
        Assert.True(ClientHumRunCore.SinglesDistinct());
        Assert.Equal(10, ClientHumRunCore.HumDisjunction.Length);
    }

    [Fact]
    public void SameMembersDifferentOrder()
    {
        // **集合相同、顺序不同**
        Assert.True(ClientHumRunCore.SameMemberSet());
        Assert.True(ClientHumRunCore.OrdersDiffer());
        Assert.True(ClientHumRunCore.SetSameOrderDifferent());
        Assert.True(ClientHumRunCore.SameMembersDifferentOrder());
        Assert.True(ClientHumRunCore.MagicMoveMoved());
        Assert.True(ClientHumRunCore.OrderRefutedByEnumeration());
    }

    [Fact]
    public void MagicMoveIndexes()
    {
        Assert.True(ClientHumRunCore.MagicMoveIndexes());

        // **基类索引四、人物版索引六**
        Assert.Equal(4, Array.IndexOf(ClientHumRunCore.BaseSingleMembers, ClientHumRunCore.SM_MAGICMOVE));
        Assert.Equal(6, Array.IndexOf(ClientHumRunCore.SingleMembers, ClientHumRunCore.SM_MAGICMOVE));
    }

    [Fact]
    public void MemberValues()
    {
        Assert.Equal(
            new[] { 11, 9, 13, 5, 6, 7, 5354, 9100 },
            ClientHumRunCore.SingleMembers);
        Assert.Equal(
            new[] { 11, 9, 13, 5, 5354, 6, 7, 9100 },
            ClientHumRunCore.BaseSingleMembers);
        Assert.Equal(new[] { 12000, 11500 }, ClientHumRunCore.RangeMembers);
    }

    [Fact]
    public void ContinueFlag()
    {
        Assert.True(ClientHumRunCore.HumAddsContinueFlag());
        Assert.True(ClientHumRunCore.BaseLacksFlag());
        Assert.True(ClientHumRunCore.SameCountIsCoincidence());
        Assert.True(ClientHumRunCore.FlagAloneHits());
    }

    [Fact]
    public void DisjunctionExit()
    {
        Assert.True(ClientHumRunCore.DisjunctionCausesEarlyExit());
        Assert.True(ClientHumRunCore.SkipsRestOfFrame());
    }

    [Fact]
    public void DisjunctionModel()
    {
        Assert.True(ClientHumRunCore.AllSinglesHit());
        Assert.True(ClientHumRunCore.BothRangesHit());
        Assert.True(ClientHumRunCore.OthersMiss());
    }

    [Fact]
    public void RangeBoundaries()
    {
        Assert.True(ClientHumRunCore.RangeBoundaries());

        Assert.False(ClientHumRunCore.InPushFamily(11999));
        Assert.True(ClientHumRunCore.InPushFamily(12000));
        Assert.True(ClientHumRunCore.InPushFamily(12299));
        Assert.False(ClientHumRunCore.InPushFamily(12300));

        Assert.False(ClientHumRunCore.InMagicMoveFamily(11499));
        Assert.True(ClientHumRunCore.InMagicMoveFamily(11500));
    }

    [Fact]
    public void SpellNotInDisjunction()
    {
        // **施法不在移动析取里**
        Assert.False(ClientHumRunCore.InMoveDisjunction(ClientHumRunCore.SM_SPELL, false));
        Assert.False(ClientHumRunCore.InMoveDisjunction(0, false));
    }

    // ===================== 四、效果帧推进 =====================

    [Fact]
    public void EffectFrameDivision()
    {
        Assert.True(ClientHumRunCore.HumDoesNotDivideByThree());
        Assert.True(ClientHumRunCore.NpcDividesByThree());
        Assert.True(ClientHumRunCore.DurationDiffers());
        Assert.True(ClientHumRunCore.EffectFrameTimeValues());
    }

    [Fact]
    public void EffectFrameTimeModel()
    {
        Assert.Equal(300, ClientHumRunCore.HumEffectFrameTime(300));
        Assert.Equal(100, ClientHumRunCore.NpcEffectFrameTime(300, true));
        Assert.Equal(300, ClientHumRunCore.NpcEffectFrameTime(300, false));
    }

    [Fact]
    public void EffectAdvance()
    {
        Assert.True(ClientHumRunCore.RequiresActionZero());
        Assert.True(ClientHumRunCore.OnlyWhenStanding());
        Assert.True(ClientHumRunCore.StopsAtEnd());
        Assert.True(ClientHumRunCore.NoLoop());
        Assert.True(ClientHumRunCore.EffectAdvanceTriggersReload());
        Assert.True(ClientHumRunCore.HumAdvanceStrict());

        Assert.False(ClientHumRunCore.HumAdvance(300, 300));
        Assert.True(ClientHumRunCore.HumAdvance(301, 300));
    }

    [Fact]
    public void EffectStepModel()
    {
        Assert.True(ClientHumRunCore.HumEffectStepAdvances());
        Assert.True(ClientHumRunCore.HumEffectStepStops());
        Assert.True(ClientHumRunCore.HumEffectStepNeedsStanding());
        Assert.True(ClientHumRunCore.HumEffectStepInactive());
    }

    // ===================== 五、其余结构与收尾 =====================

    [Fact]
    public void Structure()
    {
        Assert.True(ClientHumRunCore.ThirteenTopLevelStatements());
        Assert.True(ClientHumRunCore.NoSharedSkeleton());
    }

    [Fact]
    public void CommentedMsgMuch()
    {
        Assert.True(ClientHumRunCore.CommentedMsgMuchBlock());
        Assert.True(ClientHumRunCore.SimplifiedVersionActive());
        Assert.True(ClientHumRunCore.TwentyLinesCommented());
        Assert.True(ClientHumRunCore.CommentedExtraCondition());
        Assert.True(ClientHumRunCore.StraySemicolon());
        Assert.True(ClientHumRunCore.HalfChangedRemnant());
    }

    [Fact]
    public void MsgMuchModel()
    {
        Assert.True(ClientHumRunCore.MsgMuchValues());

        Assert.False(ClientHumRunCore.IsMsgMuch(true, 5));
        Assert.False(ClientHumRunCore.IsMsgMuch(false, 1));
        Assert.True(ClientHumRunCore.IsMsgMuch(false, 2));
    }

    [Fact]
    public void LoadChecks()
    {
        Assert.True(ClientHumRunCore.FiveLoadChecks());
        Assert.True(ClientHumRunCore.FengHaoLast());
        Assert.True(ClientHumRunCore.HasFixComment());
        Assert.True(ClientHumRunCore.LoadChecksDistinct());

        Assert.Equal(
            new[]
            {
                "CheckLoadSurface", "CheckLoadUserName", "CheckLoadNumberLable",
                "CheckLoadSay", "CheckLoadFengHaoSurface",
            },
            ClientHumRunCore.LoadChecks);
    }

    [Fact]
    public void ReloadTrigger()
    {
        Assert.True(ClientHumRunCore.FrameOrEffectChanged());
        Assert.True(ClientHumRunCore.BothTriggerReload());
        Assert.True(ClientHumRunCore.NeedsReloadValues());
        Assert.True(ClientHumRunCore.SnapshotsBeforeExit());
        Assert.True(ClientHumRunCore.EarlyExitSkipsCompare());
    }

    [Fact]
    public void ReloadModel()
    {
        Assert.True(ClientHumRunCore.NeedsReload(1, 2, 0, 0));
        Assert.True(ClientHumRunCore.NeedsReload(1, 1, 0, 1));
        Assert.False(ClientHumRunCore.NeedsReload(1, 1, 0, 0));
    }

    // ===================== 六、行数与跨批次 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(ClientHumRunCore.LineCounts());
        Assert.True(ClientHumRunCore.HumIsLongest());
        Assert.True(ClientHumRunCore.LongestDoesNotReuse());
        Assert.True(ClientHumRunCore.TotalRunLines());

        Assert.Equal(436, ClientHumRunCore.HumRunLines);
        Assert.Equal(282, ClientHumRunCore.BaseRunLines);
        Assert.Equal(158, ClientHumRunCore.NpcRunLines);
    }

    [Fact]
    public void Ratios()
    {
        Assert.True(ClientHumRunCore.HumToNpcRatio());
        Assert.True(ClientHumRunCore.HumToBaseRatio());
        Assert.True(ClientHumRunCore.HumIsAboutHalfOfTotal());

        Assert.Equal(275, ClientHumRunCore.HumRunLines * 100 / ClientHumRunCore.NpcRunLines);
        Assert.Equal(154, ClientHumRunCore.HumRunLines * 100 / ClientHumRunCore.BaseRunLines);
        Assert.Equal(49, ClientHumRunCore.HumRunLines * 100
            / (ClientHumRunCore.BaseRunLines + ClientHumRunCore.HumRunLines + ClientHumRunCore.NpcRunLines));
    }
}

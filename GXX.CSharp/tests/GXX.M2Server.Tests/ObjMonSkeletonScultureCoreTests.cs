using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J246：`ObjMon.pas` 中 `TWhiteSkeleton` + `TScultureMonster` 十一方法的 1:1 测试（226 行）。
/// **本批最有价值的发现**：
/// ① `TScultureMonster.LightingAttack` 是个**攻击方法、却顺手给自己回血**，
///    而它写的是 `Round(HP * 5 / 100)` —— **先乘后除、顺序是对的**；
///    对照 **J237 的 `MagicAttack5`** 写的是 `Round(HP / 100 * 110)` —— **先除后乘**、
///    于是**低血友方被治成 0 血** ⇒ 同一份文件两处几乎相同的式子、一处对一处错；
/// ② `MeltStoneAll` 是一场**连锁解石化**、而且**没有 `try..finally`**
///    ⇒ "建表者方有保护"规律的**第二个反例**（第一个是 J230 的火墙怪物）；
/// ③ 方法末尾四行注释**引用了基类 `TMonster.Run` 的代码** ⇒ 第 13 种注释用法；
/// ④ 外观 `218`（魔龙教主）是这条特判谱系的**第五个值**。
/// </summary>
public sealed class ObjMonSkeletonScultureCoreTests
{
    [Fact]
    public void Constants()
    {
        Assert.Equal(2302, ObjMonSkeletonScultureCore.WsCreateStart);
        Assert.Equal(2310, ObjMonSkeletonScultureCore.WsCreateEnd);
        Assert.Equal(9, ObjMonSkeletonScultureCore.WsCreateLines);
        Assert.Equal(2311, ObjMonSkeletonScultureCore.WsDestroyStart);
        Assert.Equal(2315, ObjMonSkeletonScultureCore.WsDestroyEnd);
        Assert.Equal(5, ObjMonSkeletonScultureCore.WsDestroyLines);
        Assert.Equal(2316, ObjMonSkeletonScultureCore.WsRecalcStart);
        Assert.Equal(2321, ObjMonSkeletonScultureCore.WsRecalcEnd);
        Assert.Equal(6, ObjMonSkeletonScultureCore.WsRecalcLines);
        Assert.Equal(2322, ObjMonSkeletonScultureCore.WsRunStart);
        Assert.Equal(2335, ObjMonSkeletonScultureCore.WsRunEnd);
        Assert.Equal(14, ObjMonSkeletonScultureCore.WsRunLines);
        Assert.Equal(2336, ObjMonSkeletonScultureCore.SubStart);
        Assert.Equal(2390, ObjMonSkeletonScultureCore.SubEnd);
        Assert.Equal(55, ObjMonSkeletonScultureCore.SubLines);

        Assert.Equal(2393, ObjMonSkeletonScultureCore.ScCreateStart);
        Assert.Equal(2400, ObjMonSkeletonScultureCore.ScCreateEnd);
        Assert.Equal(8, ObjMonSkeletonScultureCore.ScCreateLines);
        Assert.Equal(2402, ObjMonSkeletonScultureCore.ScDestroyStart);
        Assert.Equal(2405, ObjMonSkeletonScultureCore.ScDestroyEnd);
        Assert.Equal(4, ObjMonSkeletonScultureCore.ScDestroyLines);
        Assert.Equal(2407, ObjMonSkeletonScultureCore.MeltStart);
        Assert.Equal(2413, ObjMonSkeletonScultureCore.MeltEnd);
        Assert.Equal(7, ObjMonSkeletonScultureCore.MeltLines);
        Assert.Equal(2415, ObjMonSkeletonScultureCore.MeltAllStart);
        Assert.Equal(2439, ObjMonSkeletonScultureCore.MeltAllEnd);
        Assert.Equal(25, ObjMonSkeletonScultureCore.MeltAllLines);
        Assert.Equal(2441, ObjMonSkeletonScultureCore.AttackStart);
        Assert.Equal(2472, ObjMonSkeletonScultureCore.AttackEnd);
        Assert.Equal(32, ObjMonSkeletonScultureCore.AttackLines);
        Assert.Equal(2474, ObjMonSkeletonScultureCore.ScRunStart);
        Assert.Equal(2534, ObjMonSkeletonScultureCore.ScRunEnd);
        Assert.Equal(61, ObjMonSkeletonScultureCore.ScRunLines);

        Assert.Equal(226, ObjMonSkeletonScultureCore.TotalLines);
        Assert.Equal(11, ObjMonSkeletonScultureCore.MethodCount);
        Assert.Equal(2, ObjMonSkeletonScultureCore.ClassCount);

        Assert.Equal(2340, ObjMonSkeletonScultureCore.DisabledBlock[0]);
        Assert.Equal(2347, ObjMonSkeletonScultureCore.DisabledBlock[1]);
        Assert.Equal(2341, ObjMonSkeletonScultureCore.DisabledCommentLine);
        Assert.Equal(400, ObjMonSkeletonScultureCore.OldHitCoeff);
        Assert.Equal(200, ObjMonSkeletonScultureCore.OldWalkCoeff);
        Assert.Equal(3000, ObjMonSkeletonScultureCore.OldHitBase);
        Assert.Equal(1200, ObjMonSkeletonScultureCore.OldWalkBase);
        Assert.Equal(600, ObjMonSkeletonScultureCore.NewHitCoeff);
        Assert.Equal(250, ObjMonSkeletonScultureCore.NewWalkCoeff);
        Assert.Equal(200, ObjMonSkeletonScultureCore.OldAttrFloor);
        Assert.Equal(10, ObjMonSkeletonScultureCore.NewAttrWalkFloor);
        Assert.Equal(100, ObjMonSkeletonScultureCore.NewAttrHitFloor);
        Assert.Equal(3, ObjMonSkeletonScultureCore.LevelSplit);
        Assert.Equal(100, ObjMonSkeletonScultureCore.HighHitCoeff);
        Assert.Equal(50, ObjMonSkeletonScultureCore.HighWalkCoeff);
        Assert.Equal(2349, ObjMonSkeletonScultureCore.NewAttrLine);
        Assert.Equal(2370, ObjMonSkeletonScultureCore.ElseLine);

        Assert.Equal(2383, ObjMonSkeletonScultureCore.QuoteHeaderLine);
        Assert.Equal(2384, ObjMonSkeletonScultureCore.QuoteIntroLine);
        Assert.Equal("m_boWalkWaitLocked", ObjMonSkeletonScultureCore.QuotedField);
        Assert.Equal(12, ObjMonSkeletonScultureCore.PriorCommentUsages);

        Assert.Equal(2324, ObjMonSkeletonScultureCore.FirstFlagLine);
        Assert.Equal(5, ObjMonSkeletonScultureCore.DirectionValue);
        Assert.Equal(2329, ObjMonSkeletonScultureCore.DigUpLine);
        Assert.Equal(1800, ObjMonSkeletonScultureCore.DelayValue);
        Assert.Equal(2331, ObjMonSkeletonScultureCore.DelayLine);
        Assert.Equal(2333, ObjMonSkeletonScultureCore.RunInheritedLine);

        Assert.Equal(2398, ObjMonSkeletonScultureCore.StoneModeLine);
        Assert.Equal(2399, ObjMonSkeletonScultureCore.CharStatusExLine);
        Assert.Equal(1, ObjMonSkeletonScultureCore.STATE_STONE_MODE);
        Assert.Equal(7, ObjMonSkeletonScultureCore.ViewRange);
        Assert.Equal(2421, ObjMonSkeletonScultureCore.SelfMeltLine);
        Assert.Equal(2422, ObjMonSkeletonScultureCore.ListCreateLine);
        Assert.Equal(2423, ObjMonSkeletonScultureCore.GetMapLine);
        Assert.Equal(7, ObjMonSkeletonScultureCore.MeltRadius);
        Assert.Equal(2429, ObjMonSkeletonScultureCore.StoneCheckLine);
        Assert.Equal(2431, ObjMonSkeletonScultureCore.TypeCheckLine);
        Assert.Equal(2433, ObjMonSkeletonScultureCore.ChainMeltLine);
        Assert.Equal(2438, ObjMonSkeletonScultureCore.FreeLine);
        Assert.Equal(2437, ObjMonSkeletonScultureCore.ForEndLine);
        Assert.Equal(7858, ObjMonSkeletonScultureCore.FirstCounterExampleJ230);

        Assert.Equal(2448, ObjMonSkeletonScultureCore.EffectLine);
        Assert.Equal(1, ObjMonSkeletonScultureCore.EffectId);
        Assert.Equal(2453, ObjMonSkeletonScultureCore.TargetGetMapLine);
        Assert.Equal(3, ObjMonSkeletonScultureCore.AttackRadius);
        Assert.Equal(2458, ObjMonSkeletonScultureCore.GateLine);
        Assert.Equal(5, ObjMonSkeletonScultureCore.GateBound);
        Assert.Equal(2461, ObjMonSkeletonScultureCore.ParalysisLine);
        Assert.Equal(3, ObjMonSkeletonScultureCore.ParalysisDuration);
        Assert.Equal(2466, ObjMonSkeletonScultureCore.HealLine);
        Assert.Equal(5, ObjMonSkeletonScultureCore.HealPercent);
        Assert.Equal(2470, ObjMonSkeletonScultureCore.HealthChangedLine);

        Assert.Equal(2486, ObjMonSkeletonScultureCore.LockLine);
        Assert.Equal(2511, ObjMonSkeletonScultureCore.FinallyLine);
        Assert.Equal(2512, ObjMonSkeletonScultureCore.UnlockLine);
        Assert.Equal(2, ObjMonSkeletonScultureCore.RevealRadius);
        Assert.Equal(3, ObjMonSkeletonScultureCore.OtherRevealRadius);
        Assert.Equal(2504, ObjMonSkeletonScultureCore.MeltAllCallLine);
        Assert.Equal(2525, ObjMonSkeletonScultureCore.ApprLine);
        Assert.Equal(218, ObjMonSkeletonScultureCore.ApprValue);
        Assert.Equal(15, ObjMonSkeletonScultureCore.ApprGateBound);
        Assert.Equal(6, ObjMonSkeletonScultureCore.ApprRange);
        Assert.Equal(2530, ObjMonSkeletonScultureCore.AttackCallLine);
        Assert.Equal(2524, ObjMonSkeletonScultureCore.ApprCommentLine);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonSkeletonScultureCore.SpanMatches());
        Assert.True(ObjMonSkeletonScultureCore.TotalLinesAddUp());
        Assert.True(ObjMonSkeletonScultureCore.WsMethodsAscending());
        Assert.True(ObjMonSkeletonScultureCore.ScMethodsAscending());
        Assert.True(ObjMonSkeletonScultureCore.WsBeforeSc());
        Assert.True(ObjMonSkeletonScultureCore.WithinUnit());
        Assert.True(ObjMonSkeletonScultureCore.NoInstrumentation());
    }

    // ===================== 一、旧/新公式 =====================

    [Fact]
    public void DisabledOldFormulaFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.DisabledOldFormulas());
        Assert.True(ObjMonSkeletonScultureCore.CoefficientsChanged());
        Assert.True(ObjMonSkeletonScultureCore.ClampsAdded());
        Assert.True(ObjMonSkeletonScultureCore.OldHadNoFloor());
        Assert.True(ObjMonSkeletonScultureCore.BugFixChangedBothCoefficientAndBounds());
        Assert.True(ObjMonSkeletonScultureCore.TwoDifferentFloors());
        Assert.True(ObjMonSkeletonScultureCore.TenAndHundredVersusTwoHundred());
        Assert.True(ObjMonSkeletonScultureCore.SameSemanticsDifferentConstants());
        Assert.True(ObjMonSkeletonScultureCore.NewNeverBelowFloor());
        Assert.True(ObjMonSkeletonScultureCore.OldFallsBelowFloorAtHighLevel());
    }

    [Fact]
    public void FormulaBoundaries()
    {
        // **旧公式无下限、高等级会跌到 200 以下**
        Assert.Equal(2600, ObjMonSkeletonScultureCore.OldHitTime(1));
        Assert.Equal(1000, ObjMonSkeletonScultureCore.OldWalkSpeed(1));
        Assert.True(ObjMonSkeletonScultureCore.OldCanGoNegative());

        // **新公式处处不低于 200**
        Assert.Equal(2400, ObjMonSkeletonScultureCore.NewHitTime(1));
        Assert.Equal(1200, ObjMonSkeletonScultureCore.NewHitTime(3));
        Assert.Equal(1100, ObjMonSkeletonScultureCore.NewHitTime(4));
        Assert.Equal(700, ObjMonSkeletonScultureCore.NewHitTime(8));
        Assert.Equal(950, ObjMonSkeletonScultureCore.NewWalkSpeed(1));
        Assert.Equal(450, ObjMonSkeletonScultureCore.NewWalkSpeed(3));
        Assert.Equal(400, ObjMonSkeletonScultureCore.NewWalkSpeed(4));
    }

    [Fact]
    public void ThirdCopyFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.ThirdCopyOfTheNewAttrBranch());
        Assert.True(ObjMonSkeletonScultureCore.SameAsJ244BothSides());
        Assert.True(ObjMonSkeletonScultureCore.ThreeCopiesPresent());

        Assert.Equal(new[] { 2770, 2928 }, ObjMonSkeletonScultureCore.J244BranchLines);
    }

    // ---------- 引用基类代码的注释 ----------

    [Fact]
    public void QuotedCommentFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.CommentQuotesOtherCode());
        Assert.True(ObjMonSkeletonScultureCore.ThirteenthCommentUsage());
        Assert.True(ObjMonSkeletonScultureCore.ExplainsWhyTheOverrideExists());
        Assert.True(ObjMonSkeletonScultureCore.QuoteCanGoStale());
        Assert.True(ObjMonSkeletonScultureCore.QuoteMentionsTheField());
        Assert.True(ObjMonSkeletonScultureCore.IsThirteenth());

        Assert.Equal(new[] { 2383, 2389 }, ObjMonSkeletonScultureCore.QuoteLines);
    }

    [Fact]
    public void RecalcFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.RecalcCallsPrivateHelper());
        Assert.True(ObjMonSkeletonScultureCore.SameShapeAsJ244());
    }

    [Fact]
    public void RunBlockFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.FirstDigUpBlock());
        Assert.True(ObjMonSkeletonScultureCore.DirectionHardcodedFive());
        Assert.True(ObjMonSkeletonScultureCore.DelayEighteenHundred());
        Assert.True(ObjMonSkeletonScultureCore.NewDelayValue());
        Assert.True(ObjMonSkeletonScultureCore.InheritedLast());

        Assert.Equal(new[] { 800, 1000, 2000 }, ObjMonSkeletonScultureCore.OtherDelays);
    }

    // ===================== 二、TScultureMonster =====================

    [Fact]
    public void PetrifiedFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.StartsPetrified());
        Assert.True(ObjMonSkeletonScultureCore.SameAsJ233Mon38_0());
        Assert.True(ObjMonSkeletonScultureCore.TwoMembersOfTheFamily());
        Assert.True(ObjMonSkeletonScultureCore.FourthCopyOfTheUnstoneIdiom());
        Assert.True(ObjMonSkeletonScultureCore.VerbatimFourLines());
        Assert.True(ObjMonSkeletonScultureCore.SameAsJ233Live());
        Assert.True(ObjMonSkeletonScultureCore.FourSteps());

        Assert.Equal(new[] { 2409, 2410, 2411, 2412 },
            ObjMonSkeletonScultureCore.UnstoneLines);
        Assert.Equal(new[] { 8313, 8314, 8315, 8316 },
            ObjMonSkeletonScultureCore.J233UnstoneLines);
        Assert.Equal(4, ObjMonSkeletonScultureCore.UnstoneSteps.Length);
    }

    // ---------- MeltStoneAll ----------

    [Fact]
    public void ChainMeltFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.ChainReactionUnstone());
        Assert.True(ObjMonSkeletonScultureCore.RadiusSeven());
        Assert.True(ObjMonSkeletonScultureCore.CouldBeOneConjunction());
        Assert.True(ObjMonSkeletonScultureCore.NoTryFinally());
        Assert.True(ObjMonSkeletonScultureCore.SecondCounterExampleToTheRule());
        Assert.True(ObjMonSkeletonScultureCore.BothCounterExamplesRecorded());
        Assert.True(ObjMonSkeletonScultureCore.EndTaggedWithWhatItCloses());
        Assert.True(ObjMonSkeletonScultureCore.FirstEndTag());
    }

    [Fact]
    public void ChainBoundaries()
    {
        Assert.True(ObjMonSkeletonScultureCore.AllThreeChains());
        Assert.True(ObjMonSkeletonScultureCore.NullSkips());
        Assert.True(ObjMonSkeletonScultureCore.NotStoneSkips());
        Assert.True(ObjMonSkeletonScultureCore.NotScultureSkips());

        Assert.True(ObjMonSkeletonScultureCore.MeltsTarget(true, true, true));
        Assert.False(ObjMonSkeletonScultureCore.MeltsTarget(false, true, true));
        Assert.False(ObjMonSkeletonScultureCore.MeltsTarget(true, false, true));
        Assert.False(ObjMonSkeletonScultureCore.MeltsTarget(true, true, false));
    }

    [Fact]
    public void MeltRadiusBoundaries()
    {
        Assert.True(ObjMonSkeletonScultureCore.SevenInRadius());
        Assert.True(ObjMonSkeletonScultureCore.EightOut());

        Assert.True(ObjMonSkeletonScultureCore.InMeltRadius(7, 7));
        Assert.True(ObjMonSkeletonScultureCore.InMeltRadius(0, 7));
        Assert.False(ObjMonSkeletonScultureCore.InMeltRadius(8, 0));
        Assert.False(ObjMonSkeletonScultureCore.InMeltRadius(0, 8));
    }

    // ---------- LightingAttack ----------

    [Fact]
    public void ReceiverAndGateFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.ReceiverIsTheTarget());
        Assert.True(ObjMonSkeletonScultureCore.TwoReceiversInTheSeries());
        Assert.True(ObjMonSkeletonScultureCore.RadiusThree());
        Assert.True(ObjMonSkeletonScultureCore.SixConditionConjunction());
        Assert.True(ObjMonSkeletonScultureCore.OneInFiveGate());
        Assert.True(ObjMonSkeletonScultureCore.DurationThree());
        Assert.True(ObjMonSkeletonScultureCore.CommentMatchesCodeHere());
        Assert.True(ObjMonSkeletonScultureCore.NoParalysisSwitch());
    }

    [Fact]
    public void ParalysisBoundaries()
    {
        Assert.True(ObjMonSkeletonScultureCore.AllSevenParalyse());
        Assert.True(ObjMonSkeletonScultureCore.DeadSkips());
        Assert.True(ObjMonSkeletonScultureCore.GhostSkips());
        Assert.True(ObjMonSkeletonScultureCore.MissedRollSkips());
        Assert.True(ObjMonSkeletonScultureCore.ImmuneSkips());
        Assert.True(ObjMonSkeletonScultureCore.ImproperSkips());

        Assert.True(ObjMonSkeletonScultureCore.Paralyses(true, false, false, true, true, true, 0));
        Assert.False(ObjMonSkeletonScultureCore.Paralyses(true, true, false, true, true, true, 0));
        Assert.False(ObjMonSkeletonScultureCore.Paralyses(true, false, true, true, true, true, 0));
        Assert.False(ObjMonSkeletonScultureCore.Paralyses(true, false, false, true, true, true, 1));
        Assert.False(ObjMonSkeletonScultureCore.Paralyses(true, false, false, true, true, false, 0));
        Assert.False(ObjMonSkeletonScultureCore.Paralyses(true, false, false, true, false, true, 0));
        Assert.False(ObjMonSkeletonScultureCore.Paralyses(false, false, false, true, true, true, 0));
    }

    // ---------- 自愈（本批最有力） ----------

    [Fact]
    public void SelfHealFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.HealsItself());
        Assert.True(ObjMonSkeletonScultureCore.FivePercentOfCurrentHp());
        Assert.True(ObjMonSkeletonScultureCore.MultipliesBeforeDividing());
        Assert.True(ObjMonSkeletonScultureCore.ContrastsWithJ237sShape6());
        Assert.True(ObjMonSkeletonScultureCore.SameAuthorOppositeOrder());
        Assert.True(ObjMonSkeletonScultureCore.DivisorPositionDiffers());
        Assert.True(ObjMonSkeletonScultureCore.BothWorkAtHighHp());
        Assert.True(ObjMonSkeletonScultureCore.ThisWorksAtLowHp());
        Assert.True(ObjMonSkeletonScultureCore.J237CollapsesAtLowHp());
        Assert.True(ObjMonSkeletonScultureCore.DifferAtLowHp());
        Assert.True(ObjMonSkeletonScultureCore.CappedAtMaxHp());
        Assert.True(ObjMonSkeletonScultureCore.FullHpUnchanged());

        Assert.Equal("HP / 100 * 110", ObjMonSkeletonScultureCore.J237Formula);
        Assert.Equal("HP * 5 / 100", ObjMonSkeletonScultureCore.ThisFormula);
    }

    [Fact]
    public void HealBoundaries()
    {
        // **本处：先乘后除 ⇒ 低血也有救**
        Assert.Equal(1050, ObjMonSkeletonScultureCore.HealCorrect(1000, 2000));
        Assert.Equal(52, ObjMonSkeletonScultureCore.HealCorrect(50, 1000));
        Assert.Equal(2000, ObjMonSkeletonScultureCore.HealCorrect(1990, 2000));
        Assert.Equal(100, ObjMonSkeletonScultureCore.HealCorrect(100, 100));

        // **J237 那式：先除后乘 ⇒ 低血归零**
        Assert.Equal(0, ObjMonSkeletonScultureCore.HealJ237Style(50, 1000));

        // **且它在高血时也会越过上限（探针实测 1100）**
        Assert.Equal(1100, ObjMonSkeletonScultureCore.HealJ237Style(1000, 2000));
    }

    // ===================== 三、TScultureMonster.Run =====================

    [Fact]
    public void RevealScanFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.FourthCopyOfTheRevealScan());
        Assert.True(ObjMonSkeletonScultureCore.RadiusTwoHere());
        Assert.True(ObjMonSkeletonScultureCore.RadiusThreeElsewhere());
        Assert.True(ObjMonSkeletonScultureCore.ActionDiffersInAllFour());
        Assert.True(ObjMonSkeletonScultureCore.UsesLockTryFinally());
    }

    [Fact]
    public void AppearanceValueFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.FifthAppearanceValue());
        Assert.True(ObjMonSkeletonScultureCore.FiveDistinctValues());
        Assert.True(ObjMonSkeletonScultureCore.ThisIsTheFifth());
        Assert.True(ObjMonSkeletonScultureCore.OneInFifteen());
        Assert.True(ObjMonSkeletonScultureCore.Appearance218IsDemonDragonLeader());
        Assert.True(ObjMonSkeletonScultureCore.OneClassTwoMonsters());

        Assert.Equal(5, ObjMonSkeletonScultureCore.AppearanceValues.Length);
        Assert.Equal(231, ObjMonSkeletonScultureCore.AppearanceValues[0].Value);
        Assert.Equal(607, ObjMonSkeletonScultureCore.AppearanceValues[1].Value);
        Assert.Equal(640, ObjMonSkeletonScultureCore.AppearanceValues[2].Value);
        Assert.Equal(342, ObjMonSkeletonScultureCore.AppearanceValues[3].Value);
        Assert.Equal(218, ObjMonSkeletonScultureCore.AppearanceValues[4].Value);
    }

    [Fact]
    public void AppearanceAttackBoundaries()
    {
        Assert.True(ObjMonSkeletonScultureCore.AllConditionsAttack());
        Assert.True(ObjMonSkeletonScultureCore.WrongApprSkips());
        Assert.True(ObjMonSkeletonScultureCore.FiveInRange());
        Assert.True(ObjMonSkeletonScultureCore.SixOutOfRange());
        Assert.True(ObjMonSkeletonScultureCore.CooldownBlocks());

        Assert.True(ObjMonSkeletonScultureCore.ApprAttacks(true, 218, 0, 5, 5, true));
        Assert.False(ObjMonSkeletonScultureCore.ApprAttacks(true, 217, 0, 5, 5, true));
        Assert.False(ObjMonSkeletonScultureCore.ApprAttacks(true, 218, 1, 5, 5, true));
        Assert.False(ObjMonSkeletonScultureCore.ApprAttacks(true, 218, 0, 6, 0, true));
        Assert.False(ObjMonSkeletonScultureCore.ApprAttacks(true, 218, 0, 5, 5, false));
        Assert.False(ObjMonSkeletonScultureCore.ApprAttacks(false, 218, 0, 5, 5, true));
    }

    [Fact]
    public void MixedBoundaryFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.GreaterEqualHere());
        Assert.True(ObjMonSkeletonScultureCore.ThirdOccurrence());
        Assert.True(ObjMonSkeletonScultureCore.TwoThresholdsTwoOperators());
        Assert.True(ObjMonSkeletonScultureCore.OneIsLessEqualTwo());
        Assert.True(ObjMonSkeletonScultureCore.OtherIsLessThanSix());
    }

    // ===================== 四、其余 =====================

    [Fact]
    public void DestroyAndDisguiseFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.TwoPureShellDestroys());
        Assert.True(ObjMonSkeletonScultureCore.ThirtyFourTotal());
        Assert.True(ObjMonSkeletonScultureCore.ConsistentWithJ244J245());
        Assert.True(ObjMonSkeletonScultureCore.TwoDisguiseMechanisms());
        Assert.True(ObjMonSkeletonScultureCore.StoneModeVersusFixedHide());
        Assert.True(ObjMonSkeletonScultureCore.ConfirmsJ233sMechanismChange());
        Assert.True(ObjMonSkeletonScultureCore.DisguiseFieldsDiffer());
        Assert.True(ObjMonSkeletonScultureCore.WhiteSkeletonUsesFixedHide());
        Assert.True(ObjMonSkeletonScultureCore.ScultureUsesStoneMode());

        Assert.Equal(new[] { "m_boFixedHideMode", "m_boStoneMode" },
            ObjMonSkeletonScultureCore.DisguiseFields);
    }

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonSkeletonScultureCore.TwoClassesClosed());
        Assert.True(ObjMonSkeletonScultureCore.TwoRemain());
        Assert.True(ObjMonSkeletonScultureCore.DeclLinesChecked());

        Assert.Equal(new[] { 409, 420 }, ObjMonSkeletonScultureCore.DeclLines);
    }
}

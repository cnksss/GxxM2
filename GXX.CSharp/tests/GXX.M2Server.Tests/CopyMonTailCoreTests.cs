using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J191：`TCopyMon` 收尾四方法 1:1 测试（`Die` 22 行、`RunToNext` 7 行、
/// `RecalcAbilitys` 4 行、`MakeGhost` 5 行，合计 38 行）。
/// **两个核心发现都是"对照出来的"**：① `Die` 的奴仆表移除循环里有一个
/// 恒为假的死守卫（对照 `Run` 里没有守卫的同一逻辑）；
/// ② `RunToNext` 只查一个标志而父类查两个、且慢走分支的调用限定不同
/// （对照父类 3216-3222）。
/// </summary>
public sealed class CopyMonTailCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(22, CopyMonTailCore.DieLines);
        Assert.Equal(2243, CopyMonTailCore.DieStart);
        Assert.Equal(2264, CopyMonTailCore.DieEnd);
        Assert.Equal(7, CopyMonTailCore.RunToNextLines);
        Assert.Equal(2267, CopyMonTailCore.RunToNextStart);
        Assert.Equal(2273, CopyMonTailCore.RunToNextEnd);
        Assert.Equal(4, CopyMonTailCore.RecalcLines);
        Assert.Equal(2275, CopyMonTailCore.RecalcStart);
        Assert.Equal(2278, CopyMonTailCore.RecalcEnd);
        Assert.Equal(5, CopyMonTailCore.MakeGhostLines);
        Assert.Equal(2550, CopyMonTailCore.MakeGhostStart);
        Assert.Equal(2554, CopyMonTailCore.MakeGhostEnd);
        Assert.Equal(38, CopyMonTailCore.TotalLines);
        Assert.Equal(20163, CopyMonTailCore.RM_HEROLOGOUT);
        Assert.Equal(1304, CopyMonTailCore.WalkToNextDeclLine);
        Assert.Equal(47, CopyMonTailCore.SubclassWalkToNextLine);
        Assert.Equal(2014, CopyMonTailCore.DieCommentYear);
        Assert.Equal(2013, CopyMonTailCore.OrphanCommentYear);
        Assert.Equal(2265, CopyMonTailCore.OrphanCommentLine);
        Assert.Equal(5, CopyMonTailCore.HeroLogoutRefs);
        Assert.Equal(2, CopyMonTailCore.HeroLogoutSenders);
        Assert.Equal(2, CopyMonTailCore.CobwebRefs);
    }

    [Fact]
    public void SpanMatches()
    {
        Assert.True(CopyMonTailCore.SpanMatches());
        Assert.True(CopyMonTailCore.AllShort());
        Assert.True(CopyMonTailCore.ContrastWithJ186J187());
    }

    // ===================== 一、Die =====================

    [Fact]
    public void DeadGuardFacts()
    {
        Assert.True(CopyMonTailCore.GuardIsAlwaysFalse());
        Assert.True(CopyMonTailCore.GuardNeverReached());
        Assert.True(CopyMonTailCore.DeadGuardConfirmed());
        Assert.True(CopyMonTailCore.TwoCopies());
        Assert.True(CopyMonTailCore.OnlyDieHasGuard());
        Assert.True(CopyMonTailCore.GuardIneffective());
    }

    [Fact]
    public void DeadGuardIsProvablyFalse()
    {
        // **长度 0 到 5、自己放在每个位置、以及自己不在表里 —— 守卫都为假**
        Assert.True(CopyMonTailCore.GuardFalseForAllLengths());

        // **具体实证：长度 3、自己在下标 1**
        var (list, hit, guardTrue) =
            CopyMonTailCore.SimulateRemoval(new List<int> { 1, 7, 3 }, 7, true);

        Assert.Equal(0, guardTrue);   // **死守卫从未为真**
        Assert.Equal(1, hit);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void TwoCopiesBehaveIdentically()
    {
        Assert.True(CopyMonTailCore.SameBehaviour());
        Assert.True(CopyMonTailCore.BothStopAtFirstMatch());
        Assert.True(CopyMonTailCore.NotFoundLeavesUnchanged());
        Assert.True(CopyMonTailCore.EmptyListNoIteration());
    }

    [Fact]
    public void RemovalSemantics()
    {
        // **倒序找：下标 2 的 7 先被遇到（尽管下标 0 也有 7）**
        var (after, hit, _) =
            CopyMonTailCore.SimulateRemoval(new List<int> { 7, 5, 7, 9 }, 7, true);

        Assert.Equal(2, hit);
        Assert.Equal(3, after.Count);
        Assert.Equal(7, after[0]);  // **下标 0 的 7 保留**
        Assert.Equal(5, after[1]);

        // **未找到不改动**
        var (after2, hit2, _) =
            CopyMonTailCore.SimulateRemoval(new List<int> { 1, 2, 3 }, 999, true);

        Assert.Equal(-1, hit2);
        Assert.Equal(3, after2.Count);

        // **空表不进循环**
        var (after3, hit3, guard3) =
            CopyMonTailCore.SimulateRemoval(new List<int>(), 7, true);

        Assert.Empty(after3);
        Assert.Equal(-1, hit3);
        Assert.Equal(0, guard3);
    }

    [Fact]
    public void RemovalLoopLines()
    {
        Assert.Equal(new[] { 1829, 2251 }, CopyMonTailCore.RemovalLoopLines);
    }

    [Fact]
    public void DieOrdering()
    {
        Assert.True(CopyMonTailCore.InheritedFirst());
        Assert.True(CopyMonTailCore.CleanupAfter());
        Assert.True(CopyMonTailCore.OrderMatters());
        Assert.True(CopyMonTailCore.MasterNilUnconditional());
        Assert.True(CopyMonTailCore.Idempotent());
    }

    [Fact]
    public void HeroLogoutMessage()
    {
        Assert.True(CopyMonTailCore.GhostSendsHeroLogout());
        Assert.True(CopyMonTailCore.MessageId20163());
        Assert.True(CopyMonTailCore.ReusedHeroMessage());
        Assert.True(CopyMonTailCore.OnlyTwoSenders());
        Assert.True(CopyMonTailCore.SharedWithHero());
        Assert.True(CopyMonTailCore.SenderTextsExtracted());

        Assert.Equal(5, CopyMonTailCore.HeroLogoutSites.Length);
        Assert.Equal("Grobal2.pas:1106", CopyMonTailCore.HeroLogoutSites[0]);
        Assert.Equal("ObjSmartMon.pas:2552", CopyMonTailCore.HeroLogoutSites[4]);
    }

    [Fact]
    public void Comments()
    {
        Assert.True(CopyMonTailCore.DatedComment2014());
        Assert.True(CopyMonTailCore.DescribesBehaviour());
        Assert.True(CopyMonTailCore.OrphanComment());
        Assert.True(CopyMonTailCore.DescribesNextMethod());
        Assert.True(CopyMonTailCore.Dated2013());
        Assert.True(CopyMonTailCore.OrphanBetweenMethods());
        Assert.True(CopyMonTailCore.DifferentAuthors());
    }

    // ===================== 二、RunToNext =====================

    [Fact]
    public void NarrowerThanParent()
    {
        Assert.True(CopyMonTailCore.SubclassChecksOne());
        Assert.True(CopyMonTailCore.ParentChecksTwo());
        Assert.True(CopyMonTailCore.NarrowerThanParent());
        Assert.True(CopyMonTailCore.DivergeOnlyOnCobweb());
    }

    [Fact]
    public void FlagDivergence()
    {
        // **断筋：两者都慢走**
        Assert.True(CopyMonTailCore.SubclassSlowWalk(true));
        Assert.True(CopyMonTailCore.ParentSlowWalk(true, false));

        // **都不是：两者都正常走**
        Assert.False(CopyMonTailCore.SubclassSlowWalk(false));
        Assert.False(CopyMonTailCore.ParentSlowWalk(false, false));

        // **仅蛛网：子类不慢走、父类慢走 —— 唯一分歧点**
        Assert.False(CopyMonTailCore.SubclassSlowWalk(false));
        Assert.True(CopyMonTailCore.ParentSlowWalk(false, true));
    }

    [Fact]
    public void CobwebOnlyInParent()
    {
        Assert.True(CopyMonTailCore.CobwebOnlyInParent());
        Assert.True(CopyMonTailCore.AbsentFromSubclass());
        Assert.Equal(new[] { 3218, 3238 }, CopyMonTailCore.CobwebLines);
    }

    [Fact]
    public void InheritedQualifierMatters()
    {
        Assert.True(CopyMonTailCore.BareCallIsVirtual());
        Assert.True(CopyMonTailCore.QualifiedCallIsStatic());
        Assert.True(CopyMonTailCore.DifferentSideEffects());
        Assert.True(CopyMonTailCore.BareResolvesToOverride());
        Assert.True(CopyMonTailCore.ParentWalkToNextRefreshesTick());
        Assert.True(CopyMonTailCore.SingleSideEffect());
        Assert.True(CopyMonTailCore.WalkToNextIsVirtual());
        Assert.True(CopyMonTailCore.SubclassOverrideExtracted());
    }

    [Fact]
    public void DispatchTargetsDiffer()
    {
        // **裸调用 → 虚分派 → THumMon 覆写版**
        Assert.Equal("THumMon.WalkToNext", CopyMonTailCore.BareWalkToNextTarget());
        // **inherited 限定 → 静态 → 基类版**
        Assert.Equal("TBaseObject.WalkToNext", CopyMonTailCore.QualifiedWalkToNextTarget());
        Assert.True(CopyMonTailCore.TargetsDiffer());
    }

    [Fact]
    public void ReturnSemantics()
    {
        Assert.True(CopyMonTailCore.TwoBranchesExhaustive());
        Assert.True(CopyMonTailCore.ReturnsBranchResult());

        // **断筋时取慢走结果**
        Assert.True(CopyMonTailCore.RunToNext(true, true, false));
        Assert.False(CopyMonTailCore.RunToNext(true, false, true));
        // **否则取继承结果**
        Assert.True(CopyMonTailCore.RunToNext(false, false, true));
        Assert.False(CopyMonTailCore.RunToNext(false, true, false));
    }

    // ===================== 三、RecalcAbilitys =====================

    [Fact]
    public void PassThrough()
    {
        Assert.True(CopyMonTailCore.PurePassThrough());
        Assert.True(CopyMonTailCore.SingleStatement());
        Assert.True(CopyMonTailCore.AddIsTheRealOne());
        Assert.True(CopyMonTailCore.NamingConvention());
        Assert.True(CopyMonTailCore.AddFollowsImmediately());
    }

    // ===================== 四、MakeGhost =====================

    [Fact]
    public void GhostMessage()
    {
        Assert.True(CopyMonTailCore.SendThenInherit());
        Assert.True(CopyMonTailCore.MessageShape());
        Assert.True(CopyMonTailCore.EmptyTailParams());
        Assert.True(CopyMonTailCore.UsesSendRefMsg());
        Assert.True(CopyMonTailCore.BuildGhostMessageValues());
        Assert.True(CopyMonTailCore.SecondParamAlwaysZero());
        Assert.True(CopyMonTailCore.CoordsPassedThrough());

        // **六个参数逐一核对**
        var m = CopyMonTailCore.BuildGhostMessage(1234, 50, 60);
        Assert.Equal(20163, m.WIdent);
        Assert.Equal(0, m.WParam);
        Assert.Equal(1234, m.NParam1);
        Assert.Equal(50, m.NParam2);
        Assert.Equal(60, m.NParam3);
        Assert.Equal("", m.SParam);
    }
}

using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J114：副本三职业校验与失败标记（UsrEngn.pas 10636-10698 +
/// Actor.pas 1331 + M2Share.pas 2762/5350）1:1 测试。
/// </summary>
public sealed class FbJobCheckCoreTests
{
    private sealed class Member
    {
        public string Name = "";
        public bool Ghost;
        public object? Envir;
        public byte Job;
    }

    private static FbJobCheckCore.FailState Fail(bool fail = false, int time = 0)
        => new() { BoFBFail = fail, FbFailTime = time };

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(60_000, FbJobCheckCore.FailTimeoutMs);
        Assert.Equal(0, FbJobCheckCore.JobWarrior);
        Assert.Equal(1, FbJobCheckCore.JobWizard);
        Assert.Equal(2, FbJobCheckCore.JobTaos);
        Assert.Equal(3, FbJobCheckCore.JobCount);
        Assert.False(FbJobCheckCore.DefaultExitCreaterOffline);
    }

    [Fact]
    public void ConfigNameMatchesSource()
    {
        Assert.Equal("FBExitCreaterOffline", FbJobCheckCore.ConfigExitCreaterOfflineName);
    }

    [Fact]
    public void DefaultExitCreaterOfflineIsFalse()
    {
        // M2Share.pas 5350
        Assert.False(FbJobCheckCore.GetDefaultExitCreaterOffline());
    }

    [Fact]
    public void InitialPresenceIsAllFalse()
    {
        // 10641-10643
        var p = FbJobCheckCore.InitialPresence();
        Assert.True(p.None);
        Assert.False(p.AllThree);
    }

    // ===================== 职业置位（10659-10666） =====================

    [Fact]
    public void WarriorSetsWarriorFlag()
    {
        var p = FbJobCheckCore.ApplyJobFlag(FbJobCheckCore.InitialPresence(), 0);
        Assert.True(p!.Value.Warrior);
        Assert.False(p.Value.Wizard);
        Assert.False(p.Value.Taos);
    }

    [Fact]
    public void WizardSetsWizardFlag()
    {
        var p = FbJobCheckCore.ApplyJobFlag(FbJobCheckCore.InitialPresence(), 1);
        Assert.True(p!.Value.Wizard);
        Assert.False(p.Value.Warrior);
    }

    [Fact]
    public void TaosSetsTaosFlag()
    {
        var p = FbJobCheckCore.ApplyJobFlag(FbJobCheckCore.InitialPresence(), 2);
        Assert.True(p!.Value.Taos);
        Assert.False(p.Value.Warrior);
    }

    [Fact]
    public void UnknownJobIsIgnored()
    {
        // **case 无 else 分支**：3 及以上静默忽略、不置任何标志
        Assert.Null(FbJobCheckCore.ApplyJobFlag(FbJobCheckCore.InitialPresence(), 3));
        Assert.Null(FbJobCheckCore.ApplyJobFlag(FbJobCheckCore.InitialPresence(), 255));
    }

    [Fact]
    public void UnknownJobDoesNotClearExistingFlags()
    {
        var current = new FbJobCheckCore.JobPresence(true, false, false);
        Assert.Null(FbJobCheckCore.ApplyJobFlag(current, 99));
    }

    [Fact]
    public void ThreeFlagsAccumulate()
    {
        var p = FbJobCheckCore.InitialPresence();
        p = FbJobCheckCore.ApplyJobFlag(p, 0)!.Value;
        p = FbJobCheckCore.ApplyJobFlag(p, 1)!.Value;
        p = FbJobCheckCore.ApplyJobFlag(p, 2)!.Value;

        Assert.True(p.AllThree);
    }

    [Fact]
    public void DuplicateJobsDoNotComplete()
    {
        var p = FbJobCheckCore.InitialPresence();
        p = FbJobCheckCore.ApplyJobFlag(p, 0)!.Value;
        p = FbJobCheckCore.ApplyJobFlag(p, 0)!.Value;

        Assert.False(p.AllThree);
        Assert.True(p.Warrior);
        Assert.False(p.Wizard);
    }

    // ===================== 成员三条件（10657） =====================

    [Fact]
    public void MemberCountedWhenAllThreeConditions()
    {
        var envir = new object();
        Assert.True(FbJobCheckCore.ShouldCountMember(new object(), false, envir, envir));
    }

    [Fact]
    public void NullMemberNotCounted()
    {
        var envir = new object();
        Assert.False(FbJobCheckCore.ShouldCountMember(null, false, envir, envir));
    }

    [Fact]
    public void GhostMemberNotCounted()
    {
        var envir = new object();
        Assert.False(FbJobCheckCore.ShouldCountMember(new object(), true, envir, envir));
    }

    [Fact]
    public void MemberOutsideEnvirNotCounted()
    {
        // **第三条件是引用比较**：人在别的地图 → 不计入
        var envir = new object();
        var elsewhere = new object();

        Assert.False(FbJobCheckCore.ShouldCountMember(new object(), false, elsewhere, envir));
        Assert.False(FbJobCheckCore.ShouldCountMember(new object(), false, null, envir));
    }

    [Fact]
    public void EqualLookingButDistinctEnvirNotCounted()
    {
        // 引用比较而非值比较
        var a = new object();
        var b = new object();

        Assert.False(FbJobCheckCore.ShouldCountMember(new object(), false, a, b));
    }

    // ===================== 扫描（10653-10668） =====================

    private static FbJobCheckCore.JobPresence Scan(List<Member?> members, object envir)
        => FbJobCheckCore.ScanGroupJobs(members, m => m.Ghost, m => m.Envir, m => m.Job, envir);

    [Fact]
    public void ScanFindsAllThreeJobs()
    {
        var envir = new object();
        var members = new List<Member?>
        {
            new() { Job = 0, Envir = envir },
            new() { Job = 1, Envir = envir },
            new() { Job = 2, Envir = envir },
        };

        Assert.True(Scan(members, envir).AllThree);
    }

    [Fact]
    public void ScanEmptyGroupGivesNothing()
    {
        var envir = new object();
        Assert.True(Scan(new List<Member?>(), envir).None);
    }

    [Fact]
    public void ScanNullGroupGivesNothing()
    {
        // 10645：m_GroupMembers = nil → 循环不执行
        Assert.True(FbJobCheckCore.ScanGroupJobs<Member>(null!, m => false, m => null, m => 0, new object()).None);
    }

    [Fact]
    public void MemberOutsideEnvirDoesNotCount()
    {
        // **检查的核心语义**：队伍里有法师但他在副本外 → 法师标志不置位
        var envir = new object();
        var outside = new object();
        var members = new List<Member?>
        {
            new() { Job = 0, Envir = envir },
            new() { Job = 1, Envir = outside },   // 法师在外面
            new() { Job = 2, Envir = envir },
        };

        var p = Scan(members, envir);

        Assert.True(p.Warrior);
        Assert.False(p.Wizard);
        Assert.True(p.Taos);
        Assert.False(p.AllThree);
    }

    [Fact]
    public void GhostMemberSkipped()
    {
        var envir = new object();
        var members = new List<Member?>
        {
            new() { Job = 0, Envir = envir },
            new() { Job = 1, Envir = envir, Ghost = true },
            new() { Job = 2, Envir = envir },
        };

        Assert.False(Scan(members, envir).Wizard);
    }

    [Fact]
    public void NullMemberSkipped()
    {
        var envir = new object();
        var members = new List<Member?> { null, new() { Job = 0, Envir = envir } };

        Assert.True(Scan(members, envir).Warrior);
    }

    [Fact]
    public void ZeroMembersMeansNoJobs()
    {
        // **无队伍玩家**：10645 不成立 → 三标志全假 → 校验必然失败
        var p = Scan(new List<Member?>(), new object());

        Assert.True(p.None);
        Assert.False(FbJobCheckCore.IsJob3Satisfied(p));
    }

    [Fact]
    public void OnlyWarriorsFailsCheck()
    {
        var envir = new object();
        var members = new List<Member?>
        {
            new() { Job = 0, Envir = envir },
            new() { Job = 0, Envir = envir },
        };

        Assert.False(FbJobCheckCore.IsJob3Satisfied(Scan(members, envir)));
    }

    [Fact]
    public void HasGroupToScanRequiresBoth()
    {
        // 10645：队主与成员表都非 nil
        Assert.True(FbJobCheckCore.HasGroupToScan(new object(), new object()));
        Assert.False(FbJobCheckCore.HasGroupToScan(null, new object()));
        Assert.False(FbJobCheckCore.HasGroupToScan(new object(), null));
    }

    [Fact]
    public void IsJob3SatisfiedRequiresAllThree()
    {
        Assert.True(FbJobCheckCore.IsJob3Satisfied(new FbJobCheckCore.JobPresence(true, true, true)));
        Assert.False(FbJobCheckCore.IsJob3Satisfied(new FbJobCheckCore.JobPresence(true, true, false)));
        Assert.False(FbJobCheckCore.IsJob3Satisfied(new FbJobCheckCore.JobPresence(false, false, false)));
    }

    // ===================== 失败标记（10677-10689） =====================

    [Fact]
    public void SuccessClearsFailFlag()
    {
        // 10679
        var s = Fail(fail: true, time: 9999);
        FbJobCheckCore.UpdateFailFlag(s, new FbJobCheckCore.JobPresence(true, true, true), 1000);

        Assert.False(s.BoFBFail);
    }

    [Fact]
    public void SuccessDoesNotClearFailTime()
    {
        // 10679 只写 m_boFBFail，**不动 m_dwFBFailTime**
        var s = Fail(fail: true, time: 9999);
        FbJobCheckCore.UpdateFailFlag(s, new FbJobCheckCore.JobPresence(true, true, true), 1000);

        Assert.Equal(9999, s.FbFailTime);
    }

    [Fact]
    public void FailureSetsFlagAndTime()
    {
        // 10686-10687
        var s = Fail();
        FbJobCheckCore.UpdateFailFlag(s, new FbJobCheckCore.JobPresence(false, false, false), 1000);

        Assert.True(s.BoFBFail);
        Assert.Equal(61_000, s.FbFailTime);
    }

    [Fact]
    public void FailTimeIsNotRenewedOnEachCheck()
    {
        // **最关键的语义**：连续检查不续期
        var s = Fail();
        FbJobCheckCore.UpdateFailFlag(s, new FbJobCheckCore.JobPresence(false, false, false), 1000);
        int first = s.FbFailTime;

        FbJobCheckCore.UpdateFailFlag(s, new FbJobCheckCore.JobPresence(false, false, false), 5000);
        FbJobCheckCore.UpdateFailFlag(s, new FbJobCheckCore.JobPresence(false, false, false), 30_000);

        Assert.Equal(first, s.FbFailTime);   // 始终是 1000 + 60000
        Assert.Equal(61_000, s.FbFailTime);
    }

    [Fact]
    public void NaiveRenewalWouldNeverTimeout()
    {
        // **漏洞对照**：若每轮无条件续期，则 now > FbFailTime 永不成立
        var s = Fail();
        int now = 1000;

        for (int i = 0; i < 100; i++)
        {
            now += 10_000;   // 时间不断前进
            FbJobCheckCore.UpdateFailFlagNaiveRenew(s, new FbJobCheckCore.JobPresence(false, false, false), now);

            // 到期时刻总是比当前时刻大 60 秒 → 永不超时
            Assert.True(s.FbFailTime > now);
        }
    }

    [Fact]
    public void CorrectImplementationEventuallyTimesOut()
    {
        // 与上一条对照：正确实现下 now > FbFailTime 会成立
        var s = Fail();
        FbJobCheckCore.UpdateFailFlag(s, new FbJobCheckCore.JobPresence(false, false, false), 1000);

        // J113 路径一的条件
        Assert.False(s.BoFBFail && 60_999 > s.FbFailTime);
        Assert.True(s.BoFBFail && 61_001 > s.FbFailTime);
    }

    [Fact]
    public void FailTimeRefreshedAfterRecovery()
    {
        // 成功后再次失败 → 因 boFBFail 已为 False，会重新写到期时刻
        var s = Fail();
        FbJobCheckCore.UpdateFailFlag(s, new FbJobCheckCore.JobPresence(false, false, false), 1000);
        Assert.Equal(61_000, s.FbFailTime);

        FbJobCheckCore.UpdateFailFlag(s, new FbJobCheckCore.JobPresence(true, true, true), 2000);
        Assert.False(s.BoFBFail);

        FbJobCheckCore.UpdateFailFlag(s, new FbJobCheckCore.JobPresence(false, false, false), 90_000);
        Assert.True(s.BoFBFail);
        Assert.Equal(150_000, s.FbFailTime);   // 90000 + 60000
    }

    [Fact]
    public void ShouldSetFailTimeOnlyWhenNotFailed()
    {
        Assert.True(FbJobCheckCore.ShouldSetFailTime(false));
        Assert.False(FbJobCheckCore.ShouldSetFailTime(true));
    }

    [Fact]
    public void SelectFailUpdateFourOutcomes()
    {
        Assert.Equal(FbJobCheckCore.FailUpdate.SetFailAndTime,
            FbJobCheckCore.SelectFailUpdate(Fail(false), new FbJobCheckCore.JobPresence(false, false, false)));

        Assert.Equal(FbJobCheckCore.FailUpdate.AlreadyFailedNoChange,
            FbJobCheckCore.SelectFailUpdate(Fail(true), new FbJobCheckCore.JobPresence(false, false, false)));

        Assert.Equal(FbJobCheckCore.FailUpdate.ClearedFail,
            FbJobCheckCore.SelectFailUpdate(Fail(true), new FbJobCheckCore.JobPresence(true, true, true)));

        Assert.Equal(FbJobCheckCore.FailUpdate.StayedPassing,
            FbJobCheckCore.SelectFailUpdate(Fail(false), new FbJobCheckCore.JobPresence(true, true, true)));
    }

    [Fact]
    public void AlreadyFailedIsNoChange()
    {
        var s = Fail(fail: true, time: 5000);
        Assert.Equal(FbJobCheckCore.FailUpdate.AlreadyFailedNoChange,
            FbJobCheckCore.SelectFailUpdate(s, new FbJobCheckCore.JobPresence(true, false, false)));

        FbJobCheckCore.UpdateFailFlag(s, new FbJobCheckCore.JobPresence(true, false, false), 99_999);
        Assert.Equal(5000, s.FbFailTime);   // 未被改写
    }

    // ===================== 外层门与配置（10606/10639/10691） =====================

    [Fact]
    public void JobCheckRequiresMasterObject()
    {
        // 10606：创建者被清空后本块不再执行
        Assert.True(FbJobCheckCore.ShouldRunJobCheck(new object()));
        Assert.False(FbJobCheckCore.ShouldRunJobCheck(null));
    }

    [Fact]
    public void JobCheckStopsAfterMasterCleared()
    {
        // **与 J113 的耦合**：10631-10635 清 m_FBMasterObject 后，失败标记停止更新
        var s = Fail();
        object? master = new object();

        Assert.True(FbJobCheckCore.ShouldRunJobCheck(master));
        FbJobCheckCore.UpdateFailFlag(s, new FbJobCheckCore.JobPresence(false, false, false), 1000);
        Assert.True(s.BoFBFail);

        // 创建者离场 → 10631-10635 清空
        master = null;
        Assert.False(FbJobCheckCore.ShouldRunJobCheck(master));

        // 即便队伍构成变化，也不再更新
        int before = s.FbFailTime;
        Assert.False(FbJobCheckCore.ShouldRunJobCheck(master));
        Assert.Equal(before, s.FbFailTime);
    }

    [Fact]
    public void OnlyJob3LimitTriggersCheck()
    {
        // 10639
        Assert.True(FbJobCheckCore.IsJob3Limit(FbMapDeclareCore.FbEnterLimit.Job3));
        Assert.False(FbJobCheckCore.IsJob3Limit(FbMapDeclareCore.FbEnterLimit.Group));
        Assert.False(FbJobCheckCore.IsJob3Limit(FbMapDeclareCore.FbEnterLimit.OnlyCreater));
        Assert.False(FbJobCheckCore.IsJob3Limit(FbMapDeclareCore.FbEnterLimit.Guild));
    }

    [Fact]
    public void ElseClearBranchIsCommentedOut()
    {
        // 10691-10696 被注释 → 非 fbel_JOB3 副本的 m_boFBFail 不被本处清除
        Assert.False(FbJobCheckCore.HasElseClearBranch());
    }

    // ===================== 端到端 =====================

    [Fact]
    public void EndToEndIncompleteGroupSetsFailThenTimesOut()
    {
        // 队伍缺道士 → 置失败 → 60 秒后满足 J113 路径一
        var envir = new object();
        var members = new List<Member?>
        {
            new() { Job = 0, Envir = envir },
            new() { Job = 1, Envir = envir },
        };

        var presence = Scan(members, envir);
        Assert.False(FbJobCheckCore.IsJob3Satisfied(presence));

        var s = Fail();
        FbJobCheckCore.UpdateFailFlag(s, presence, 1000);

        Assert.True(s.BoFBFail);
        Assert.False(FbRecycleCore.ShouldReleaseOnFail(
            new FbRecycleCore.FbRecycleState { BoFBFail = s.BoFBFail, FbFailTime = s.FbFailTime }, 60_999));
        Assert.True(FbRecycleCore.ShouldReleaseOnFail(
            new FbRecycleCore.FbRecycleState { BoFBFail = s.BoFBFail, FbFailTime = s.FbFailTime }, 61_001));
    }

    [Fact]
    public void EndToEndCompleteGroupClearsFail()
    {
        var envir = new object();
        var members = new List<Member?>
        {
            new() { Job = 0, Envir = envir },
            new() { Job = 1, Envir = envir },
            new() { Job = 2, Envir = envir },
        };

        var s = Fail(fail: true, time: 5000);
        FbJobCheckCore.UpdateFailFlag(s, Scan(members, envir), 1000);

        Assert.False(s.BoFBFail);
        // 未失败 → J113 路径一不成立
        Assert.False(FbRecycleCore.ShouldReleaseOnFail(
            new FbRecycleCore.FbRecycleState { BoFBFail = s.BoFBFail, FbFailTime = s.FbFailTime }, 999_999));
    }
}

using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J115：副本进入流程两道门（NpcActionCmd.pas 23044-23057 / 23217-23237
/// + 入口分支 23261-23301）1:1 测试。
/// </summary>
public sealed class FbEnterCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(60_000, FbEnterCore.EnterWindowBaseMs);
        Assert.Equal(-1, FbEnterCore.DefaultCoord);
    }

    [Fact]
    public void LabelsAreFiveDistinct()
    {
        var labels = new[]
        {
            FbEnterCore.LabelCreateIn,
            FbEnterCore.LabelCreateInTime,
            FbEnterCore.LabelMoveOk,
            FbEnterCore.LabelMoveFail,
            FbEnterCore.LabelMoveFailTime,
        };

        Assert.Equal(5, new System.Collections.Generic.HashSet<string>(labels).Count);
    }

    [Fact]
    public void LabelTextsMatchSource()
    {
        Assert.Equal("@CreateEctype_IN", FbEnterCore.LabelCreateIn);
        Assert.Equal("@CreateEctype_IN_Time", FbEnterCore.LabelCreateInTime);
        Assert.Equal("@MoveEctype_OK", FbEnterCore.LabelMoveOk);
        Assert.Equal("@MoveEctype_Fail", FbEnterCore.LabelMoveFail);
        Assert.Equal("@MoveEctype_Fail_Time", FbEnterCore.LabelMoveFailTime);
    }

    // ===================== 时间窗（23047 / 23220） =====================

    [Fact]
    public void WindowOpenAtCreation()
    {
        // now = createTime → 差 0 <= 60000
        Assert.True(FbEnterCore.IsWithinEnterWindow(1000, 1000, 0));
    }

    [Fact]
    public void WindowOpenAtBoundary()
    {
        // **`<=` 闭区间**
        Assert.True(FbEnterCore.IsWithinEnterWindow(61_000, 1000, 0));
    }

    [Fact]
    public void WindowClosedPastBoundary()
    {
        Assert.False(FbEnterCore.IsWithinEnterWindow(61_001, 1000, 0));
    }

    [Fact]
    public void WindowWidenedByEnterDelay()
    {
        // 60000 + 300000（5 分）
        Assert.True(FbEnterCore.IsWithinEnterWindow(1000 + 360_000, 1000, 300_000));
        Assert.False(FbEnterCore.IsWithinEnterWindow(1000 + 360_001, 1000, 300_000));
    }

    [Fact]
    public void EnterWindowMsFormula()
    {
        Assert.Equal(60_000, FbEnterCore.EnterWindowMs(0));
        Assert.Equal(360_000, FbEnterCore.EnterWindowMs(300_000));
    }

    [Fact]
    public void CanEnterWindowMatchesJ113Formulation()
    {
        // **等价性证明**：两处写法方向相反但语义相同，防止后续被"统一"时改错方向
        for (int elapsed = 0; elapsed <= 200_000; elapsed += 997)
        {
            int now = 50_000 + elapsed;
            const int create = 50_000;
            const int delay = 120_000;

            bool a = FbEnterCore.IsWithinEnterWindow(now, create, delay);
            bool b = FbEnterCore.IsWithinEnterWindowJ113Form(now, create, delay);

            Assert.Equal(a, b);
        }
    }

    // ===================== CanEnterToMap（23044-23057） =====================

    [Fact]
    public void CanEnterInsideWindow()
    {
        var r = FbEnterCore.CanEnterToMap(1000, 1000, 0);
        Assert.True(r.CanEnter);
        Assert.Equal(FbEnterCore.LabelCreateIn, r.Label);
    }

    [Fact]
    public void CanEnterOutsideWindow()
    {
        var r = FbEnterCore.CanEnterToMap(999_999, 1000, 0);
        Assert.False(r.CanEnter);
        Assert.Equal(FbEnterCore.LabelCreateInTime, r.Label);
    }

    [Fact]
    public void CanEnterBoundaryIsInclusive()
    {
        Assert.True(FbEnterCore.CanEnterToMap(61_000, 1000, 0).CanEnter);
        Assert.False(FbEnterCore.CanEnterToMap(61_001, 1000, 0).CanEnter);
    }

    // ===================== DoEnterToMap（23217-23237） =====================

    [Fact]
    public void DoEnterSucceeds()
    {
        var r = FbEnterCore.DoEnterToMap(1000, 1000, 0, () => true);

        Assert.True(r.Entered);
        Assert.False(r.TimedOut);
        Assert.True(r.AttemptedMapChange);
        Assert.Equal(FbEnterCore.LabelMoveOk, r.Label);
    }

    [Fact]
    public void DoEnterFailsWhenMapChangeFails()
    {
        var r = FbEnterCore.DoEnterToMap(1000, 1000, 0, () => false);

        Assert.False(r.Entered);
        Assert.False(r.TimedOut);
        Assert.True(r.AttemptedMapChange);
        Assert.Equal(FbEnterCore.LabelMoveFail, r.Label);
    }

    [Fact]
    public void DoEnterTimesOutWithoutAttempting()
    {
        // **窗外根本不调用换图**
        bool called = false;
        var r = FbEnterCore.DoEnterToMap(999_999, 1000, 0, () => { called = true; return true; });

        Assert.False(called);
        Assert.False(r.Entered);
        Assert.True(r.TimedOut);
        Assert.False(r.AttemptedMapChange);
        Assert.Equal(FbEnterCore.LabelMoveFailTime, r.Label);
    }

    [Fact]
    public void TimeoutAndMapFailAreDifferentLabels()
    {
        // **两种失败不可混**
        var timeout = FbEnterCore.DoEnterToMap(999_999, 1000, 0, () => true);
        var fail = FbEnterCore.DoEnterToMap(1000, 1000, 0, () => false);

        Assert.NotEqual(timeout.Label, fail.Label);
        Assert.True(timeout.TimedOut);
        Assert.False(fail.TimedOut);
    }

    [Fact]
    public void BothGatesShareSameWindowCondition()
    {
        // 同一表达式、不同动作：给出相同的窗内/窗外判定
        for (int delta = -1000; delta <= 200_000; delta += 5000)
        {
            int now = 10_000 + delta;
            bool can = FbEnterCore.CanEnterToMap(now, 10_000, 60_000).CanEnter;

            bool attempted = false;
            FbEnterCore.DoEnterToMap(now, 10_000, 60_000, () => { attempted = true; return true; });

            Assert.Equal(can, attempted);
        }
    }

    // ===================== 三处写入（23224-23226） =====================

    [Fact]
    public void ApplyEnterWritesAllThreeFields()
    {
        var map = new object();
        var s = new FbEnterCore.EnterState();

        FbEnterCore.ApplyEnterWrites(s, map, 55_555);

        Assert.Same(map, s.PlayerFbEnvir);
        Assert.True(s.MapBoFBPlayObjectEnter);
        Assert.Equal(55_555, s.PlayerFbCreateTime);
    }

    [Fact]
    public void EnterFlagOpensExpiryBlockImmediately()
    {
        // 23225 置位后，J113 路径二外层条件立即成立（无需等宽限）
        Assert.False(FbRecycleCore.ShouldEnterExpiryBlock(
            new FbRecycleCore.FbRecycleState { FbCreateTime = 1000, FbEnterDelayMs = 0, BoFBPlayObjectEnter = false },
            1000));

        Assert.True(FbEnterCore.EnterFlagOpensExpiryBlock(1000, 1000, 0));
    }

    [Fact]
    public void EnterFlagAloneSufficesEvenLongBeforeGrace()
    {
        // now 远未到创建宽限，但标志已置 → 仍成立
        var s = new FbRecycleCore.FbRecycleState
        {
            FbCreateTime = 100_000,
            FbEnterDelayMs = 0,
            BoFBPlayObjectEnter = true,
        };

        Assert.True(FbRecycleCore.ShouldEnterExpiryBlock(s, 100_001));
    }

    [Fact]
    public void PlayerCreateTimeMatchesMapEnablesSelfOwned()
    {
        // 23226 写入后，J112 的"已在同一副本内"第三项才可能成立
        var map = new FbInstancePoolCore.FbInstance();

        Assert.False(FbEnterCore.PlayerCreateTimeMatchesMap(0, 55_555));
        Assert.True(FbEnterCore.PlayerCreateTimeMatchesMap(55_555, 55_555));

        Assert.True(FbInstancePoolCore.IsAlreadyInSameFb(map, true, 55_555, 55_555, "祖玛", "祖玛"));
    }

    [Fact]
    public void WithoutThirdWriteSelfOwnedNeverMatches()
    {
        // 漏掉 23226 的后果：玩家每次都被当作首次进入
        var map = new FbInstancePoolCore.FbInstance();

        Assert.False(FbInstancePoolCore.IsAlreadyInSameFb(map, true, 0, 55_555, "祖玛", "祖玛"));
    }

    // ===================== 坐标校验（23241-23247） =====================

    [Fact]
    public void CoordsInvalidWhenNegative()
    {
        Assert.True(FbEnterCore.AreCoordsInvalid(-1, 10));
        Assert.True(FbEnterCore.AreCoordsInvalid(10, -1));
        Assert.False(FbEnterCore.AreCoordsInvalid(0, 0));
    }

    [Fact]
    public void CoordsInvalidAtMinusOneDefault()
    {
        Assert.True(FbEnterCore.AreCoordsInvalid(-1, -1));
    }

    [Fact]
    public void ZeroCoordsAreValid()
    {
        // 边界：0 合法（`< 0` 而非 `<= 0`）
        Assert.False(FbEnterCore.AreCoordsInvalid(0, 0));
    }

    [Fact]
    public void ParseCoordsDefaultsToMinusOne()
    {
        var (x, y) = FbEnterCore.ParseCoords("", "");
        Assert.Equal(-1, x);
        Assert.Equal(-1, y);
    }

    [Fact]
    public void ParseCoordsReadsValues()
    {
        var (x, y) = FbEnterCore.ParseCoords("330", "330");
        Assert.Equal(330, x);
        Assert.Equal(330, y);
    }

    [Fact]
    public void ParseCoordsNonNumericFallsBack()
    {
        var (x, y) = FbEnterCore.ParseCoords("abc", "xyz");
        Assert.Equal(-1, x);
        Assert.Equal(-1, y);
    }

    [Fact]
    public void MissingCoordsFailValidation()
    {
        // 缺省 -1 → 被 `< 0` 拦下
        var (x, y) = FbEnterCore.ParseCoords("", "");
        Assert.True(FbEnterCore.AreCoordsInvalid(x, y));
    }

    // ===================== 查不到副本的不对称 =====================

    [Fact]
    public void MoveEctypeExitsSilently()
    {
        // 23258-23259：只 Exit、无消息、无标签
        Assert.True(FbEnterCore.MoveEctypeExitsSilentlyWhenNotFound());
        Assert.Equal("", FbEnterCore.NotFoundLabelForMove());
    }

    [Fact]
    public void MoveEctypeNotFoundDiffersFromCreateEctype()
    {
        // **不对称**：创建流程有消息与标签（J112 已固化），移动流程没有
        Assert.Equal("副本不存在", FbInstancePoolCore.FbNotExistsError);
        Assert.Equal("@CreateEctype_NoExists", FbInstancePoolCore.LabelNoExists);
        Assert.NotEqual(FbInstancePoolCore.LabelNoExists, FbEnterCore.NotFoundLabelForMove());
    }

    // ===================== 入口分支（23261-23301） =====================

    [Fact]
    public void SelfOwnedRequiresFourConditions()
    {
        var map = new object();
        Assert.True(FbEnterCore.IsSelfOwned(map, true, 500, 500, "祖玛", "祖玛"));

        Assert.False(FbEnterCore.IsSelfOwned(null, true, 500, 500, "祖玛", "祖玛"));
        Assert.False(FbEnterCore.IsSelfOwned(map, false, 500, 500, "祖玛", "祖玛"));
        Assert.False(FbEnterCore.IsSelfOwned(map, true, 500, 600, "祖玛", "祖玛"));
        Assert.False(FbEnterCore.IsSelfOwned(map, true, 500, 500, "祖玛", "赤月"));
    }

    [Fact]
    public void SelfOwnedNameIsCaseInsensitive()
    {
        var map = new object();
        Assert.True(FbEnterCore.IsSelfOwned(map, true, 500, 500, "FB", "fb"));
    }

    [Fact]
    public void LeaderOwnedRequiresThreeConditions()
    {
        var map = new object();
        Assert.True(FbEnterCore.IsLeaderOwned(map, true, "祖玛", "祖玛"));

        Assert.False(FbEnterCore.IsLeaderOwned(null, true, "祖玛", "祖玛"));
        Assert.False(FbEnterCore.IsLeaderOwned(map, false, "祖玛", "祖玛"));
        Assert.False(FbEnterCore.IsLeaderOwned(map, true, "祖玛", "赤月"));
    }

    [Fact]
    public void LeaderOwnedHasNoTimeComparison()
    {
        // 与 SelfOwned 的差异：**不比较创建时刻**
        var map = new object();
        Assert.True(FbEnterCore.IsLeaderOwned(map, true, "x", "x"));
    }

    [Fact]
    public void GroupLikeLimitCoversZeroAndOne()
    {
        Assert.True(FbEnterCore.IsGroupLikeLimit(FbMapDeclareCore.FbEnterLimit.Job3));
        Assert.True(FbEnterCore.IsGroupLikeLimit(FbMapDeclareCore.FbEnterLimit.Group));
        Assert.False(FbEnterCore.IsGroupLikeLimit(FbMapDeclareCore.FbEnterLimit.OnlyCreater));
        Assert.False(FbEnterCore.IsGroupLikeLimit(FbMapDeclareCore.FbEnterLimit.Guild));
    }

    [Fact]
    public void BranchSelfOwnedWins()
    {
        // **自己已建优先于限制类型**：即使限制是行会类，创建者仍走 SelfOwned
        Assert.Equal(FbEnterCore.EnterBranch.SelfOwned,
            FbEnterCore.SelectEnterBranch(true, FbMapDeclareCore.FbEnterLimit.Guild, false, false));

        Assert.Equal(FbEnterCore.EnterBranch.SelfOwned,
            FbEnterCore.SelectEnterBranch(true, FbMapDeclareCore.FbEnterLimit.OnlyCreater, false, false));
    }

    [Fact]
    public void BranchViaGroupOwner()
    {
        Assert.Equal(FbEnterCore.EnterBranch.ViaGroupOwner,
            FbEnterCore.SelectEnterBranch(false, FbMapDeclareCore.FbEnterLimit.Group, true, false));

        Assert.Equal(FbEnterCore.EnterBranch.ViaGroupOwner,
            FbEnterCore.SelectEnterBranch(false, FbMapDeclareCore.FbEnterLimit.Job3, true, false));
    }

    [Fact]
    public void BranchViaGuildMaster()
    {
        Assert.Equal(FbEnterCore.EnterBranch.ViaGuildMaster,
            FbEnterCore.SelectEnterBranch(false, FbMapDeclareCore.FbEnterLimit.Guild, false, true));
    }

    [Fact]
    public void BranchNoneWhenLeaderNotOwned()
    {
        Assert.Equal(FbEnterCore.EnterBranch.None,
            FbEnterCore.SelectEnterBranch(false, FbMapDeclareCore.FbEnterLimit.Group, false, false));

        Assert.Equal(FbEnterCore.EnterBranch.None,
            FbEnterCore.SelectEnterBranch(false, FbMapDeclareCore.FbEnterLimit.Guild, false, false));
    }

    [Fact]
    public void GroupLimitIgnoresGuildMaster()
    {
        // 队伍类限制下，掌门人已建也**不**走行会分支
        Assert.Equal(FbEnterCore.EnterBranch.None,
            FbEnterCore.SelectEnterBranch(false, FbMapDeclareCore.FbEnterLimit.Group, false, true));
    }

    [Fact]
    public void GuildLimitIgnoresGroupOwner()
    {
        Assert.Equal(FbEnterCore.EnterBranch.None,
            FbEnterCore.SelectEnterBranch(false, FbMapDeclareCore.FbEnterLimit.Guild, true, false));
    }

    [Fact]
    public void OnlyCreaterBranchIsNoneForOthers()
    {
        // `fbel_OnlyCreater` 下非创建者无任何分支
        Assert.Equal(FbEnterCore.EnterBranch.None,
            FbEnterCore.SelectEnterBranch(false, FbMapDeclareCore.FbEnterLimit.OnlyCreater, true, true));
    }

    [Fact]
    public void OnlyCreaterBlocksOthers()
    {
        Assert.True(FbEnterCore.OnlyCreaterBlocksOthers(FbMapDeclareCore.FbEnterLimit.OnlyCreater, false));
        Assert.False(FbEnterCore.OnlyCreaterBlocksOthers(FbMapDeclareCore.FbEnterLimit.OnlyCreater, true));
        Assert.False(FbEnterCore.OnlyCreaterBlocksOthers(FbMapDeclareCore.FbEnterLimit.Group, false));
    }

    [Fact]
    public void AllFourBranchesAreDistinct()
    {
        var seen = new System.Collections.Generic.HashSet<FbEnterCore.EnterBranch>
        {
            FbEnterCore.SelectEnterBranch(true, FbMapDeclareCore.FbEnterLimit.Group, false, false),
            FbEnterCore.SelectEnterBranch(false, FbMapDeclareCore.FbEnterLimit.Group, true, false),
            FbEnterCore.SelectEnterBranch(false, FbMapDeclareCore.FbEnterLimit.Guild, false, true),
            FbEnterCore.SelectEnterBranch(false, FbMapDeclareCore.FbEnterLimit.Guild, false, false),
        };

        Assert.Equal(4, seen.Count);
    }

    [Fact]
    public void HitBranchesCallDoEnterAndExit()
    {
        Assert.True(FbEnterCore.BranchCallsDoEnterAndExits(FbEnterCore.EnterBranch.SelfOwned));
        Assert.True(FbEnterCore.BranchCallsDoEnterAndExits(FbEnterCore.EnterBranch.ViaGroupOwner));
        Assert.True(FbEnterCore.BranchCallsDoEnterAndExits(FbEnterCore.EnterBranch.ViaGuildMaster));
        Assert.False(FbEnterCore.BranchCallsDoEnterAndExits(FbEnterCore.EnterBranch.None));
    }

    // ===================== 端到端 =====================

    [Fact]
    public void EndToEndEnterWithinWindow()
    {
        // 创建 → 三处写入 → 立即可被判定为"已在同一副本内"
        var map = new object();
        var s = new FbEnterCore.EnterState();

        var r = FbEnterCore.DoEnterToMap(1000, 1000, 0, () => true);
        Assert.True(r.Entered);
        Assert.Equal(FbEnterCore.LabelMoveOk, r.Label);

        FbEnterCore.ApplyEnterWrites(s, map, 1000);

        Assert.True(FbEnterCore.IsSelfOwned(s.PlayerFbEnvir, true, s.PlayerFbCreateTime, 1000, "祖玛", "祖玛"));
    }

    [Fact]
    public void EndToEndEnterAfterTimeout()
    {
        var r = FbEnterCore.DoEnterToMap(500_000, 1000, 0, () => true);

        Assert.False(r.Entered);
        Assert.Equal(FbEnterCore.LabelMoveFailTime, r.Label);

        // 同时 CanEnterToMap 也给出窗外标签
        var c = FbEnterCore.CanEnterToMap(500_000, 1000, 0);
        Assert.False(c.CanEnter);
        Assert.Equal(FbEnterCore.LabelCreateInTime, c.Label);
    }
}

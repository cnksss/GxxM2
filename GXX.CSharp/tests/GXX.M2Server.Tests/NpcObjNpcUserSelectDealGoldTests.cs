// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：`DealGold`（原文 2212-2254，元宝转账）+ `nNF_DealGold` 分支（2727-2731）
// ★ 重点：三处早退各跳**不同**标签（PlayError / InputFail / Fail / post），以及 2223 的
//   "**先置 Pose=2 再做金额校验**"这个**顺序**（PoseError 路径**不置**、其后路径**都置**）。
// ⚠ 未覆盖（如实登记）：2233-2248 的**成功路径与 @dealgoldpost 分支**需要构造
//   `User.GetPoseCreate()` 的**互指**对向对象；本轮预算不足，未写该组用例。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcUserSelectDealGoldTests : IDisposable
{
    private readonly List<string> _sent = new();
    private readonly List<string> _labels = new();

    public NpcObjNpcUserSelectDealGoldTests()
    {
        NpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
        NpcSeams.SendTo = (sender, target, ident, wParam, p1, p2, p3, sMsg) =>
            _sent.Add($"{ident}|{wParam}|{p1}|{p2}|{p3}|{sMsg}");
        PlayerSurfaceNpcSeams.GotoLable = (npc, p, label, ext) => { _labels.Add(label); return true; };
    }

    public void Dispose()
    {
        NpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
    }

    private static TMerchant M() => new() { m_sCharName = "商人" };

    private static TPlayObject P(int gold = 1000, int pose = 1)
        => new() { m_sCharName = "玩家", m_nGameGold = gold, m_nDealGoldPose = pose };

    // -----------------------------------------------------------------------
    // 2218-2222：Pose 门（★ Exit 且**不改** Pose）
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(-1)]
    public void DealGold_PoseNotOne_GotoPlayErrorAndLeavesPoseUnchanged(int pose)
    {
        var p = P(pose: pose);
        M().DealGold(p, "100");
        Assert.Equal("@dealgoldPlayError", Assert.Single(_labels));
        Assert.Equal(pose, p.m_nDealGoldPose);     // ★ Exit 早于 2223 → Pose **未被改动**
        Assert.Equal(1000, p.m_nGameGold);         // 也未动余额
    }

    // -----------------------------------------------------------------------
    // 2223 的**顺序**：先置 Pose=2，再校验金额
    // -----------------------------------------------------------------------

    [Fact]
    public void DealGold_InputFailPath_PoseWasSetToTwoBeforeCheck()
    {
        // ★ 顺序断言：2223 在 2224 **之前** ⇒ 即使金额非法，Pose 也已变成 2
        var p = P(pose: 1);
        M().DealGold(p, "abc");                    // 非数字 → StrToIntDef 默认 -1
        Assert.Equal("@dealgoldInputFail", Assert.Single(_labels));
        Assert.Equal(2, p.m_nDealGoldPose);
    }

    [Fact]
    public void DealGold_InsufficientGoldPath_PoseWasSetToTwo()
    {
        var p = P(gold: 50, pose: 1);
        M().DealGold(p, "100");
        Assert.Equal("@dealgoldFail", Assert.Single(_labels));
        Assert.Equal(2, p.m_nDealGoldPose);
    }

    // -----------------------------------------------------------------------
    // 2224：`nGameGold <= 0`（含 0 与负数）
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-100")]
    [InlineData("abc")]      // StrToIntDef 默认 -1
    [InlineData("")]         // 同上
    public void DealGold_NonPositiveAmount_GotoInputFail(string sMsg)
    {
        var p = P();
        M().DealGold(p, sMsg);
        Assert.Equal("@dealgoldInputFail", Assert.Single(_labels));
        Assert.Equal(1000, p.m_nGameGold);         // 未动余额
    }

    [Fact]
    public void DealGold_AmountOne_IsAboveTheZeroBoundary()
    {
        // `<= 0` 的边界：1 应当**通过**金额门（继而落到对向校验 → 无对向 → @dealgoldpost）
        var p = P();
        M().DealGold(p, "1");
        Assert.DoesNotContain("@dealgoldInputFail", _labels);
    }

    // -----------------------------------------------------------------------
    // 2230：余额不足 → `@dealgoldFail`（与 InputFail **不同**标签）
    // -----------------------------------------------------------------------

    [Fact]
    public void DealGold_ExactBalance_PassesBalanceGate()
    {
        // 2230 用 `>=` ⇒ 恰好等于应通过
        var p = P(gold: 100);
        M().DealGold(p, "100");
        Assert.DoesNotContain("@dealgoldFail", _labels);
    }

    [Fact]
    public void DealGold_OneLessThanAmount_FailsBalanceGate()
    {
        var p = P(gold: 99);
        M().DealGold(p, "100");
        Assert.Equal("@dealgoldFail", Assert.Single(_labels));
        Assert.Equal(99, p.m_nGameGold);           // 未扣
    }

    [Fact]
    public void DealGold_ThreeEarlyExitLabelsAreDistinct()
    {
        // ★ 差异断言：三条早退路径的标签两两不同（防互抄）
        var a = P(pose: 0);
        M().DealGold(a, "100");
        var b = P();
        M().DealGold(b, "0");
        var c = P(gold: 1);
        M().DealGold(c, "100");

        Assert.Equal(3, _labels.Count);
        Assert.Equal("@dealgoldPlayError", _labels[0]);
        Assert.Equal("@dealgoldInputFail", _labels[1]);
        Assert.Equal("@dealgoldFail", _labels[2]);
        Assert.Equal(3, _labels.Distinct().Count());
    }

    // -----------------------------------------------------------------------
    // 派发分支 nNF_DealGold（2727-2731，守卫 m_boDealGold）
    // -----------------------------------------------------------------------

    [Fact]
    public void DealGoldArm_FlagOn_DispatchesToDealGold()
    {
        var m = M();
        m.m_boDealGold = true;
        var p = P(pose: 1);
        Assert.True(m.UserSelectPortedArms(p, NpcProcessCmd.nNF_DealGold, "0"));
        Assert.Equal("@dealgoldInputFail", Assert.Single(_labels));   // 确实走到了 DealGold
    }

    [Fact]
    public void DealGoldArm_FlagOff_DoesNothing()
    {
        var m = M();
        m.m_boDealGold = false;
        Assert.True(m.UserSelectPortedArms(P(), NpcProcessCmd.nNF_DealGold, "100"));
        Assert.Empty(_labels);
    }

    [Fact]
    public void DealGold_CommandIdIsOriginal()
    {
        // NpcCommon.pas:110
        Assert.Equal(51, NpcProcessCmd.nNF_DealGold);
    }
}

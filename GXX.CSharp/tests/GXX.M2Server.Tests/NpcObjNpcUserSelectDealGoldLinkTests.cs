// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：`DealGold`（2212-2254）的**成功路径**与 **@dealgoldpost 分支**（2233-2248）
//   —— 切片 48 登记的未覆盖项，本片补齐。
// ★ 注入方式：`TPlayObject.GetPoseCreate()` 的实现走
//   `PlayerSurfaceBaseSeams.GetMovingObject(m_PEnvir, nX, nY, null, true)`
//   （`TCreature.PlayerSurface.Base.cs:362-369`）⇒ 用**两条不同 env 实例**把
//   "User → PoseHuman → User" 的**互指**关系注入进去，而不是"无法覆盖"。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcUserSelectDealGoldLinkTests : IDisposable
{
    private readonly List<string> _sent = new();
    private readonly List<string> _labels = new();

    private readonly TEnvirnoment _envUser = new();
    private readonly TEnvirnoment _envPose = new();

    public NpcObjNpcUserSelectDealGoldLinkTests()
    {
        NpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
        PlayerSurfaceBaseSeams.ResetDefaults();
        NpcSeams.SendTo = (sender, target, ident, wParam, p1, p2, p3, sMsg) =>
            _sent.Add($"{ident}|{wParam}|{p1}|{p2}|{p3}|{sMsg}");
        PlayerSurfaceNpcSeams.GotoLable = (npc, p, label, ext) => { _labels.Add(label); return true; };
    }

    public void Dispose()
    {
        NpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
        PlayerSurfaceBaseSeams.ResetDefaults();
    }

    private static TMerchant M() => new() { m_sCharName = "商人" };

    /// <summary>构造 **互指** 的一对玩家：`user.GetPoseCreate() == pose` 且 `pose.GetPoseCreate() == user`。</summary>
    private (TPlayObject user, TPlayObject pose) LinkedPair(int userGold = 1000, int poseGold = 50)
    {
        var user = new TPlayObject { m_sCharName = "转出者", m_nGameGold = userGold, m_nDealGoldPose = 1, m_PEnvir = _envUser };
        var pose = new TPlayObject
        {
            m_sCharName = "接收者", m_nGameGold = poseGold, m_btRaceServer = Grobal2Const.RC_PLAYOBJECT,
            m_PEnvir = _envPose,
        };
        PlayerSurfaceBaseSeams.GetMovingObject = (env, x, y, bo, b) =>
            ReferenceEquals(env, _envUser) ? pose : (ReferenceEquals(env, _envPose) ? user : null);
        return (user, pose);
    }

    // -----------------------------------------------------------------------
    // 成功路径（2233-2240）
    // -----------------------------------------------------------------------

    [Fact]
    public void DealGold_LinkedPair_TransfersGoldBothWays()
    {
        var (user, pose) = LinkedPair(userGold: 1000, poseGold: 50);
        M().DealGold(user, "300");

        Assert.Equal(700, user.m_nGameGold);       // 2236 Dec 自己
        Assert.Equal(350, pose.m_nGameGold);       // 2235 Inc 对方
        Assert.Equal(2, user.m_nDealGoldPose);     // 2223 已置 2
        Assert.Empty(_labels);                     // 成功路径**不跳任何标签**
    }

    [Fact]
    public void DealGold_LinkedPair_SendsTwoMessagesToTheRightTargets()
    {
        var (user, pose) = LinkedPair(userGold: 1000, poseGold: 50);
        M().DealGold(user, "300");

        Assert.Equal(2, _sent.Count);
        // 第一条给**转出者**（转出 + 当前余额 700）
        Assert.Contains("转出" + M2Config.sGameGoldName + "：300", _sent[0]);
        Assert.Contains("当前" + M2Config.sGameGoldName + "：700", _sent[0]);
        // 第二条给**接收者**（增加 + 当前余额 350）
        Assert.Contains("增加" + M2Config.sGameGoldName + "：300", _sent[1]);
        Assert.Contains("当前" + M2Config.sGameGoldName + "：350", _sent[1]);
    }

    [Fact]
    public void DealGold_ExactBalance_TransfersAllLeavingZero()
    {
        var (user, pose) = LinkedPair(userGold: 300, poseGold: 0);
        M().DealGold(user, "300");
        Assert.Equal(0, user.m_nGameGold);
        Assert.Equal(300, pose.m_nGameGold);
    }

    [Fact]
    public void DealGold_TransferOne_IsMinimalSuccess()
    {
        var (user, pose) = LinkedPair(userGold: 1, poseGold: 0);
        M().DealGold(user, "1");
        Assert.Equal(0, user.m_nGameGold);
        Assert.Equal(1, pose.m_nGameGold);
    }

    [Fact]
    public void DealGold_MessageUsesTabAndNewlineEscapes()
    {
        // 2239-2240：`#10`（换行）与 `#9`（制表符）—— 用字符断言，不用字面量
        var (user, _) = LinkedPair();
        M().DealGold(user, "1");
        Assert.Contains('\n', _sent[0]);
        Assert.Contains('\t', _sent[0]);
        Assert.Contains('\n', _sent[1]);
        Assert.Contains('\t', _sent[1]);
    }

    // -----------------------------------------------------------------------
    // @dealgoldpost 分支（对向三个条件之一不满足）
    // -----------------------------------------------------------------------

    [Fact]
    public void DealGold_NoPoseObject_GotoPostLabel()
    {
        // 2233 第 1 条件：`PoseHuman <> nil` —— 注缝返回 null（无对向）
        var user = new TPlayObject { m_sCharName = "转出者", m_nGameGold = 1000, m_nDealGoldPose = 1, m_PEnvir = _envUser };
        PlayerSurfaceBaseSeams.GetMovingObject = (_, _, _, _, _) => null;

        M().DealGold(user, "300");

        Assert.Equal("@dealgoldpost", Assert.Single(_labels));
        Assert.Equal(1000, user.m_nGameGold);      // **未转账**
        Assert.Empty(_sent);
    }

    [Fact]
    public void DealGold_PoseNotPointingBack_GotoPostLabel()
    {
        // 2233 第 2 条件：`PoseHuman.GetPoseCreate = User` —— 让对向**指回别人**（{user} 之外的第三者）
        var user = new TPlayObject { m_sCharName = "转出者", m_nGameGold = 1000, m_nDealGoldPose = 1, m_PEnvir = _envUser };
        var pose = new TPlayObject
        {
            m_sCharName = "接收者", m_nGameGold = 50, m_btRaceServer = Grobal2Const.RC_PLAYOBJECT, m_PEnvir = _envPose,
        };
        var third = new TPlayObject { m_sCharName = "第三者" };
        // user 看到 pose；pose 看到的是 third（**不是** user）⇒ 不互指
        PlayerSurfaceBaseSeams.GetMovingObject = (env, _, _, _, _) =>
            ReferenceEquals(env, _envUser) ? pose : third;

        M().DealGold(user, "300");

        Assert.Equal("@dealgoldpost", Assert.Single(_labels));
        Assert.Equal(1000, user.m_nGameGold);
        Assert.Equal(50, pose.m_nGameGold);
    }

    [Fact]
    public void DealGold_PoseIsNotPlayObject_GotoPostLabel()
    {
        // 2233 第 3 条件：`PoseHuman.m_btRaceServer = RC_PLAYOBJECT` —— 对向是**怪物**
        var (user, pose) = LinkedPair(userGold: 1000, poseGold: 50);
        pose.m_btRaceServer = 0;                   // 非 RC_PLAYOBJECT（RC_PLAYOBJECT = 0? 见下）

        // ⚠ 若 `RC_PLAYOBJECT` 本身是 0，则本用例需换一个**非 0** 的种族值；用断言自证
        if (Grobal2Const.RC_PLAYOBJECT == 0)
            pose.m_btRaceServer = 1;               // 任意非玩家种族

        M().DealGold(user, "300");

        Assert.Equal("@dealgoldpost", Assert.Single(_labels));
        Assert.Equal(1000, user.m_nGameGold);
    }

    [Fact]
    public void DealGold_LinkedPairIsRequired_ThirdPartyIsRejected()
    {
        // ★ 差异断言：同一金额，"互指"成功、"不互指"落 post —— 唯一变量是对向关系
        var (okUser, okPose) = LinkedPair(userGold: 100, poseGold: 0);
        M().DealGold(okUser, "100");
        Assert.Empty(_labels);
        Assert.Equal(100, okPose.m_nGameGold);

        _labels.Clear();
        _sent.Clear();

        var user = new TPlayObject { m_sCharName = "转出者", m_nGameGold = 100, m_nDealGoldPose = 1, m_PEnvir = _envUser };
        PlayerSurfaceBaseSeams.GetMovingObject = (_, _, _, _, _) => null;
        M().DealGold(user, "100");
        Assert.Equal("@dealgoldpost", Assert.Single(_labels));
    }

    [Fact]
    public void DealGold_PostLabelDiffersFromTheOtherThreeLabels()
    {
        // ★ 四标签互不相同（PlayError / InputFail / Fail / post）
        var a = new TPlayObject { m_nDealGoldPose = 0 };
        M().DealGold(a, "1");
        var b = new TPlayObject { m_nDealGoldPose = 1 };
        M().DealGold(b, "0");
        var c = new TPlayObject { m_nDealGoldPose = 1, m_nGameGold = 1 };
        M().DealGold(c, "100");
        var d = new TPlayObject { m_nDealGoldPose = 1, m_nGameGold = 100, m_PEnvir = _envUser };
        PlayerSurfaceBaseSeams.GetMovingObject = (_, _, _, _, _) => null;
        M().DealGold(d, "100");

        Assert.Equal(4, _labels.Count);
        Assert.Equal(4, _labels.Distinct().Count());
        Assert.Contains("@dealgoldpost", _labels);
    }
}

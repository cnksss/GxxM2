using System;
using GXX.Core.Protocol;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J101：ObjBase.pas SendRefMsg(30980-31428) 决策层 1:1 测试。
/// </summary>
public sealed class SendRefMsgCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ViewRangeExtYIsZero()
    {
        // M2Share.pas: C_VIEWRANGEEXTY = 0; // 9
        Assert.Equal(0, SendRefMsgCore.C_VIEWRANGEEXTY);
    }

    [Fact]
    public void RebuildIntervalIs100ms()
    {
        Assert.Equal(100, SendRefMsgCore.VisibleListRebuildInterval);   // 31108
    }

    [Fact]
    public void RefStatusGraceIs60Seconds()
    {
        Assert.Equal(60_000u, SendRefMsgCore.RefStatusGraceMs);          // 31164
    }

    // ===================== 成员集合 =====================

    [Fact]
    public void GroupNotifySetHasSixMembers()
    {
        Assert.Equal(6, SendRefMsgCore.GroupNotifySet.Length);
    }

    [Fact]
    public void GroupNotifySetContents()
    {
        Assert.Contains(Grobal2Const.RM_HEALTHSPELLCHANGED, SendRefMsgCore.GroupNotifySet);
        Assert.Contains(Grobal2Const.RM_HEALTHSPELLCHANGED_STRUCK, SendRefMsgCore.GroupNotifySet);
        Assert.Contains(Grobal2Const.RM_HPMPCHANGED_FORM_STONE, SendRefMsgCore.GroupNotifySet);
        Assert.Contains(Grobal2Const.RM_MAGICSHIELD_STRUCK, SendRefMsgCore.GroupNotifySet);
        Assert.Contains(Grobal2Const.RM_STRUCK, SendRefMsgCore.GroupNotifySet);
        Assert.Contains(Grobal2Const.RM_STRUCK_MAG, SendRefMsgCore.GroupNotifySet);
    }

    [Fact]
    public void GroupNotifySetExcludesOthers()
    {
        Assert.False(SendRefMsgCore.IsGroupNotifyIdent(Grobal2Const.RM_DEATH));
        Assert.False(SendRefMsgCore.IsGroupNotifyIdent(Grobal2Const.RM_HEAR));
        Assert.False(SendRefMsgCore.IsGroupNotifyIdent(Grobal2Const.RM_ITEMSHOW));
        Assert.False(SendRefMsgCore.IsGroupNotifyIdent(Grobal2Const.RM_RUSH));
    }

    [Fact]
    public void WantRefMsgSetHasFiveMembers()
    {
        Assert.Equal(5, SendRefMsgCore.WantRefMsgSet.Length);
    }

    [Fact]
    public void WantRefMsgSetContents()
    {
        Assert.Contains(Grobal2Const.RM_STRUCK, SendRefMsgCore.WantRefMsgSet);
        Assert.Contains(Grobal2Const.RM_HEAR, SendRefMsgCore.WantRefMsgSet);
        Assert.Contains(Grobal2Const.RM_DEATH, SendRefMsgCore.WantRefMsgSet);
        Assert.Contains(Grobal2Const.RM_CHARSTATUSCHANGED, SendRefMsgCore.WantRefMsgSet);
        Assert.Contains(Grobal2Const.RM_RUSH, SendRefMsgCore.WantRefMsgSet);
    }

    [Fact]
    public void TwoSetsDifferByMagicShieldMessages()
    {
        // 两组集合**不同**：Group 含 HEALTHSPELL/HPMP/MAGICSHIELD，WantRef 含 HEAR/DEATH/CHARSTATUS/RUSH
        Assert.NotEqual(SendRefMsgCore.GroupNotifySet.Length, SendRefMsgCore.WantRefMsgSet.Length);

        // RM_STRUCK 是两者唯一的交集
        int common = 0;
        foreach (int id in SendRefMsgCore.GroupNotifySet)
            if (SendRefMsgCore.IsWantRefMsgIdent(id)) common++;

        Assert.Equal(1, common);
    }

    [Fact]
    public void StruckIsTheOnlySharedIdent()
    {
        Assert.True(SendRefMsgCore.IsGroupNotifyIdent(Grobal2Const.RM_STRUCK));
        Assert.True(SendRefMsgCore.IsWantRefMsgIdent(Grobal2Const.RM_STRUCK));

        Assert.False(SendRefMsgCore.IsWantRefMsgIdent(Grobal2Const.RM_HEALTHSPELLCHANGED));
        Assert.False(SendRefMsgCore.IsGroupNotifyIdent(Grobal2Const.RM_HEAR));
    }

    // ===================== 隐身判定 =====================

    [Fact]
    public void TempHideRaceIsThreeRacesOnly()
    {
        Assert.True(SendRefMsgCore.IsTempHideModeRace(Grobal2Const.RC_PLAYOBJECT));  // 0
        Assert.True(SendRefMsgCore.IsTempHideModeRace(Grobal2Const.RC_HEROOBJECT));  // 1
        Assert.True(SendRefMsgCore.IsTempHideModeRace(Grobal2Const.RC_PLAYMOSTER));  // 150
    }

    [Fact]
    public void TempHideRaceExcludesMonstersAndNpcs()
    {
        Assert.False(SendRefMsgCore.IsTempHideModeRace(80));    // RC_MONSTER
        Assert.False(SendRefMsgCore.IsTempHideModeRace(10));    // RC_NPC
        Assert.False(SendRefMsgCore.IsTempHideModeRace(11));    // RC_GUARD
        Assert.False(SendRefMsgCore.IsTempHideModeRace(50));    // RC_ANIMAL
    }

    [Fact]
    public void TempHideRequiresBothRaceAndTick()
    {
        // 非限定种族：即使 tick > 0 也为假（31036 初值 False 不被覆盖）
        Assert.False(SendRefMsgCore.ComputeTempFixedHideMode(80, 999));

        // 限定种族：tick > 0 才为真
        Assert.True(SendRefMsgCore.ComputeTempFixedHideMode(Grobal2Const.RC_PLAYOBJECT, 1));
        Assert.False(SendRefMsgCore.ComputeTempFixedHideMode(Grobal2Const.RC_PLAYOBJECT, 0));
    }

    [Fact]
    public void SelfOnlyModeOnAnyFlag()
    {
        Assert.True(SendRefMsgCore.IsSelfOnlyMode(true, false, false));
        Assert.True(SendRefMsgCore.IsSelfOnlyMode(false, true, false));
        Assert.True(SendRefMsgCore.IsSelfOnlyMode(false, false, true));
        Assert.False(SendRefMsgCore.IsSelfOnlyMode(false, false, false));
    }

    [Fact]
    public void HeroDisappear65535Exits()
    {
        // 31048-31049
        Assert.True(SendRefMsgCore.ShouldExitOnDisappear(
            Grobal2Const.RC_HEROOBJECT, Grobal2Const.RM_DISAPPEAR, 65535));
    }

    [Fact]
    public void HeroDisappearOtherParamDoesNotExit()
    {
        Assert.False(SendRefMsgCore.ShouldExitOnDisappear(
            Grobal2Const.RC_HEROOBJECT, Grobal2Const.RM_DISAPPEAR, 65534));
    }

    [Fact]
    public void PlayerDisappear65535DoesNotExit()
    {
        // 仅英雄分支，玩家不适用
        Assert.False(SendRefMsgCore.ShouldExitOnDisappear(
            Grobal2Const.RC_PLAYOBJECT, Grobal2Const.RM_DISAPPEAR, 65535));
    }

    [Fact]
    public void HeroOtherIdentDoesNotExit()
    {
        Assert.False(SendRefMsgCore.ShouldExitOnDisappear(
            Grobal2Const.RC_HEROOBJECT, Grobal2Const.RM_STRUCK, 65535));
    }

    // ===================== 两种视野判定（易混） =====================

    [Fact]
    public void ViewRangeUsesOtherObjectsExtY()
    {
        // 31196-31199：门限用**对方**的 viewRange 与 ExtY
        Assert.True(SendRefMsgCore.IsInViewRange(true, 10, 10, 0, 0, otherViewRange: 10, otherViewRangeExtY: 0));
        Assert.False(SendRefMsgCore.IsInViewRange(true, 11, 10, 0, 0, 10, 0));
    }

    [Fact]
    public void ViewRangeRequiresSameEnvir()
    {
        Assert.False(SendRefMsgCore.IsInViewRange(false, 0, 0, 0, 0, 10, 0));
    }

    [Fact]
    public void WantRefViewRangeUsesMaxOfBoth()
    {
        // 31248：门限取 Max(对方, 自己)
        Assert.True(SendRefMsgCore.IsInViewRangeForWantRef(
            true, 15, 10, 0, 0, otherViewRange: 5, selfViewRange: 20, selfViewRangeExtY: 0));
    }

    [Fact]
    public void WantRefViewRangeFailsWhenBeyondBoth()
    {
        Assert.False(SendRefMsgCore.IsInViewRangeForWantRef(
            true, 21, 10, 0, 0, otherViewRange: 5, selfViewRange: 20, selfViewRangeExtY: 0));
    }

    [Fact]
    public void TwoViewRangeFunctionsCanDisagree()
    {
        // 对方视野 5、自己视野 20：wantRef 版通过（取 Max=20），普通版失败（用 5）
        bool normal = SendRefMsgCore.IsInViewRange(true, 15, 10, 0, 0, 5, 0);
        bool wantRef = SendRefMsgCore.IsInViewRangeForWantRef(true, 15, 10, 0, 0, 5, 20, 0);

        Assert.False(normal);
        Assert.True(wantRef);
        Assert.NotEqual(normal, wantRef);   // 冗余保护：两者不可互换
    }

    [Fact]
    public void ViewRangeExtYAddsToYOnly()
    {
        // ExtY=5：Y 方向容差 10+5=15，X 方向仍为 10
        Assert.True(SendRefMsgCore.IsInViewRange(true, 0, 15, 0, 0, 10, 5));
        Assert.False(SendRefMsgCore.IsInViewRange(true, 15, 0, 0, 0, 10, 5));
    }

    // ===================== 掉落物品二次判定 =====================

    [Fact]
    public void NonItemMessagesAlwaysSend()
    {
        Assert.True(SendRefMsgCore.ShouldSendItemMessage(
            Grobal2Const.RM_STRUCK, 99, 99, 0, 0, 10, 0));
    }

    [Fact]
    public void ItemShowUsesItemCoordsNotCallerParams()
    {
        // 观察者位于 (0,0)；物品落在 (1,1) → 视野内 → 发送
        // 原文 31205：comparison 为 |BaseObject.m_nCurrX - nParam2|，即**观察者坐标 vs 物品坐标**
        Assert.True(SendRefMsgCore.ShouldSendItemMessage(
            Grobal2Const.RM_ITEMSHOW, otherX: 0, otherY: 0, nParam2: 1, nParam3: 1,
            otherViewRange: 10, otherViewRangeExtY: 0));
    }

    [Fact]
    public void ItemShowFailsWhenItemOutOfRange()
    {
        // 观察者 (0,0)，物品在 (99,99) → 超出视野 → 不发送
        Assert.False(SendRefMsgCore.ShouldSendItemMessage(
            Grobal2Const.RM_ITEMSHOW, 0, 0, 99, 99, 10, 0));
    }

    [Fact]
    public void ItemShowIsRelativeToObserverNotOrigin()
    {
        // 同一物品坐标 (50,50)：观察者 (45,45) 看得到，观察者 (0,0) 看不到
        Assert.True(SendRefMsgCore.ShouldSendItemMessage(
            Grobal2Const.RM_ITEMSHOW, 45, 45, 50, 50, 10, 0));
        Assert.False(SendRefMsgCore.ShouldSendItemMessage(
            Grobal2Const.RM_ITEMSHOW, 0, 0, 50, 50, 10, 0));
    }

    [Fact]
    public void ItemHideAlsoUsesItemCoords()
    {
        Assert.True(SendRefMsgCore.ShouldSendItemMessage(
            Grobal2Const.RM_ITEMHIDE, 0, 0, 1, 1, 10, 0));
        Assert.False(SendRefMsgCore.ShouldSendItemMessage(
            Grobal2Const.RM_ITEMHIDE, 0, 0, 99, 99, 10, 0));
    }

    [Fact]
    public void ItemCoordsUseYExtY()
    {
        // Y 容差 10+5=15：物品在 (0,15) 可见，(0,16) 不可见
        Assert.True(SendRefMsgCore.ShouldSendItemMessage(
            Grobal2Const.RM_ITEMSHOW, 0, 0, 0, 15, 10, 5));
        Assert.False(SendRefMsgCore.ShouldSendItemMessage(
            Grobal2Const.RM_ITEMSHOW, 0, 0, 0, 16, 10, 5));
    }

    // ===================== ProcessRMMsg =====================

    [Fact]
    public void FeatureChangedRewritesParams()
    {
        var r = SendRefMsgCore.ProcessRMMsg(
            Grobal2Const.RM_FEATURECHANGED, 111, 222, 333, 444, "orig",
            featureWParam: 777, featureSMsg: "feat", backupMsg: "backup", isMasterIsPlayer: false);

        Assert.Equal(777, r.WParam);
        Assert.Equal(0, r.NParam1);
        Assert.Equal(0, r.NParam2);
        Assert.Equal(0, r.NParam3);
        Assert.Equal("feat", r.SMsg);
    }

    [Fact]
    public void FeatureChangedKeepsIdent()
    {
        var r = SendRefMsgCore.ProcessRMMsg(
            Grobal2Const.RM_FEATURECHANGED, 1, 2, 3, 4, "o", 9, "f", "b", false);
        Assert.Equal(Grobal2Const.RM_FEATURECHANGED, r.WIdent);
    }

    [Fact]
    public void ItemShowRestoresBackupMsg()
    {
        var r = SendRefMsgCore.ProcessRMMsg(
            Grobal2Const.RM_ITEMSHOW, 1, 2, 3, 4, "current",
            0, "", backupMsg: "backup", isMasterIsPlayer: false);

        Assert.Equal("backup", r.SMsg);
        Assert.Equal(2, r.NParam1);      // 参数**不清零**
        Assert.Equal(3, r.NParam2);
        Assert.Equal(4, r.NParam3);
    }

    [Fact]
    public void HeroLogonBecomesMyHeroLogonWhenMasterIsPlayer()
    {
        var r = SendRefMsgCore.ProcessRMMsg(
            Grobal2Const.RM_HEROLOGON, 1, 2, 3, 4, "s", 0, "", "", isMasterIsPlayer: true);

        Assert.Equal(Grobal2Const.RM_MYHEROLOGON, r.WIdent);
    }

    [Fact]
    public void HeroLogonStaysHeroLogonWhenMasterIsNotPlayer()
    {
        var r = SendRefMsgCore.ProcessRMMsg(
            Grobal2Const.RM_HEROLOGON, 1, 2, 3, 4, "s", 0, "", "", isMasterIsPlayer: false);

        Assert.Equal(Grobal2Const.RM_HEROLOGON, r.WIdent);
    }

    [Fact]
    public void MyHeroLogonAlsoRewritten()
    {
        // 两个 ident 都进入同一分支（31002 的 or）
        var r = SendRefMsgCore.ProcessRMMsg(
            Grobal2Const.RM_MYHEROLOGON, 1, 2, 3, 4, "s", 0, "", "", isMasterIsPlayer: false);

        Assert.Equal(Grobal2Const.RM_HEROLOGON, r.WIdent);
    }

    [Fact]
    public void OtherMessagesPassThrough()
    {
        var r = SendRefMsgCore.ProcessRMMsg(
            Grobal2Const.RM_STRUCK, 11, 22, 33, 44, "keep", 0, "", "b", false);

        Assert.Equal(Grobal2Const.RM_STRUCK, r.WIdent);
        Assert.Equal(11, r.WParam);
        Assert.Equal(22, r.NParam1);
        Assert.Equal("keep", r.SMsg);    // 不被 backup 覆盖
    }

    [Fact]
    public void SetMsgNotMutatedByItemShowOnOtherIdent()
    {
        // RM_ITEMHIDE 不在 ProcessRMMsg 的备份分支里 → 保持原串
        var r = SendRefMsgCore.ProcessRMMsg(
            Grobal2Const.RM_ITEMHIDE, 1, 2, 3, 4, "orig", 0, "", "backup", false);

        Assert.Equal("orig", r.SMsg);
    }

    // ===================== 可见列表重建 =====================

    [Fact]
    public void RebuildWhenIntervalElapsed()
    {
        Assert.True(SendRefMsgCore.ShouldRebuildVisibleList(
            now: 1000, sendRefMsgTick: 900, visibleCount: 5, nSendRefMsgRange: 0, gSendRefMsgRange: 0));
    }

    [Fact]
    public void NoRebuildWhenRecentAndNonEmptyAndRangeSame()
    {
        Assert.False(SendRefMsgCore.ShouldRebuildVisibleList(
            now: 1000, sendRefMsgTick: 950, visibleCount: 5, 0, 0));
    }

    [Fact]
    public void RebuildWhenVisibleListEmpty()
    {
        Assert.True(SendRefMsgCore.ShouldRebuildVisibleList(
            now: 1000, sendRefMsgTick: 950, visibleCount: 0, 0, 0));
    }

    [Fact]
    public void RebuildWhenRangeChanged()
    {
        Assert.True(SendRefMsgCore.ShouldRebuildVisibleList(
            now: 1000, sendRefMsgTick: 950, visibleCount: 5,
            nSendRefMsgRange: 3, gSendRefMsgRange: 7));
    }

    [Fact]
    public void ExactlyAt100msRebuilds()
    {
        // >= 100 为真，故 100 恰好触发
        Assert.True(SendRefMsgCore.ShouldRebuildVisibleList(1100, 1000, 5, 0, 0));
        Assert.False(SendRefMsgCore.ShouldRebuildVisibleList(1099, 1000, 5, 0, 0));
    }

    // ===================== 扫描范围 =====================

    [Fact]
    public void ScanBoundsAreSymmetricWhenExtYZero()
    {
        var (lx, hx, ly, hy) = SendRefMsgCore.ComputeScanBounds(100, 200, 8);

        Assert.Equal(92, lx);
        Assert.Equal(108, hx);
        Assert.Equal(192, ly);
        Assert.Equal(208, hy);
    }

    [Fact]
    public void ScanBoundsExtendYByViewRangeExtY()
    {
        // 当前 C_VIEWRANGEEXTY = 0，故 Y 与 X 范围相同；若常量改回 9，此处会变化
        var (lx, hx, ly, hy) = SendRefMsgCore.ComputeScanBounds(0, 0, 5);

        Assert.Equal(-5, lx);
        Assert.Equal(5, hx);
        Assert.Equal(-5 - SendRefMsgCore.C_VIEWRANGEEXTY, ly);
        Assert.Equal(5 + SendRefMsgCore.C_VIEWRANGEEXTY, hy);
    }

    // ===================== 队伍补充广播 =====================

    [Fact]
    public void GroupNotifyRequiresAllFiveConditions()
    {
        Assert.True(SendRefMsgCore.ShouldSendToGroupOwnerMembers(
            boShowNewGroupInfo: true, selfRaceServer: Grobal2Const.RC_PLAYOBJECT,
            hasGroupOwner: true, groupOwnerRaceServer: Grobal2Const.RC_PLAYOBJECT,
            wIdent: Grobal2Const.RM_STRUCK));
    }

    [Fact]
    public void GroupNotifyOffWhenConfigDisabled()
    {
        Assert.False(SendRefMsgCore.ShouldSendToGroupOwnerMembers(
            false, Grobal2Const.RC_PLAYOBJECT, true, Grobal2Const.RC_PLAYOBJECT, Grobal2Const.RM_STRUCK));
    }

    [Fact]
    public void GroupNotifyOffWhenSelfIsHero()
    {
        // 自己必须是玩家，英雄不参与
        Assert.False(SendRefMsgCore.ShouldSendToGroupOwnerMembers(
            true, Grobal2Const.RC_HEROOBJECT, true, Grobal2Const.RC_PLAYOBJECT, Grobal2Const.RM_STRUCK));
    }

    [Fact]
    public void GroupNotifyOffWhenNoGroupOwner()
    {
        Assert.False(SendRefMsgCore.ShouldSendToGroupOwnerMembers(
            true, Grobal2Const.RC_PLAYOBJECT, false, Grobal2Const.RC_PLAYOBJECT, Grobal2Const.RM_STRUCK));
    }

    [Fact]
    public void GroupNotifyOffWhenOwnerNotPlayer()
    {
        Assert.False(SendRefMsgCore.ShouldSendToGroupOwnerMembers(
            true, Grobal2Const.RC_PLAYOBJECT, true, Grobal2Const.RC_HEROOBJECT, Grobal2Const.RM_STRUCK));
    }

    [Fact]
    public void GroupNotifyOffForNonListedIdent()
    {
        Assert.False(SendRefMsgCore.ShouldSendToGroupOwnerMembers(
            true, Grobal2Const.RC_PLAYOBJECT, true, Grobal2Const.RC_PLAYOBJECT, Grobal2Const.RM_DEATH));
    }

    [Fact]
    public void GroupMemberSkippedWhenSeesSelf()
    {
        Assert.False(SendRefMsgCore.ShouldSendToGroupMember(isNull: false, isSelf: false, seesSelf: true));
    }

    [Fact]
    public void GroupMemberSkippedWhenSelf()
    {
        Assert.False(SendRefMsgCore.ShouldSendToGroupMember(false, true, false));
    }

    [Fact]
    public void GroupMemberSkippedWhenNull()
    {
        Assert.False(SendRefMsgCore.ShouldSendToGroupMember(true, false, false));
    }

    [Fact]
    public void GroupMemberSentWhenBlind()
    {
        Assert.True(SendRefMsgCore.ShouldSendToGroupMember(false, false, false));
    }

    // ===================== 英雄主人补偿广播 =====================

    [Fact]
    public void HeroSendsWhenDifferentEnvir()
    {
        Assert.True(SendRefMsgCore.ShouldSendHeroStateToMaster(
            Grobal2Const.RC_HEROOBJECT, true, Grobal2Const.RC_PLAYOBJECT,
            sameEnvir: false, 0, 0, 0, 0, 10, 0));
    }

    [Fact]
    public void HeroSendsWhenMasterTooFarX()
    {
        Assert.True(SendRefMsgCore.ShouldSendHeroStateToMaster(
            Grobal2Const.RC_HEROOBJECT, true, Grobal2Const.RC_PLAYOBJECT,
            true, masterX: 11, masterY: 0, selfX: 0, selfY: 0, masterViewRange: 10, selfViewRangeExtY: 0));
    }

    [Fact]
    public void HeroDoesNotSendWhenMasterClose()
    {
        Assert.False(SendRefMsgCore.ShouldSendHeroStateToMaster(
            Grobal2Const.RC_HEROOBJECT, true, Grobal2Const.RC_PLAYOBJECT,
            true, 10, 10, 0, 0, 10, 0));
    }

    [Fact]
    public void HeroYThresholdUsesOwnExtY()
    {
        // 主人视野 10 + 英雄自己的 ExtY 5 = 15
        Assert.False(SendRefMsgCore.ShouldSendHeroStateToMaster(
            Grobal2Const.RC_HEROOBJECT, true, Grobal2Const.RC_PLAYOBJECT,
            true, 0, 15, 0, 0, 10, 5));
        Assert.True(SendRefMsgCore.ShouldSendHeroStateToMaster(
            Grobal2Const.RC_HEROOBJECT, true, Grobal2Const.RC_PLAYOBJECT,
            true, 0, 16, 0, 0, 10, 5));
    }

    [Fact]
    public void PlayerNeverSendsViaThisPath()
    {
        Assert.False(SendRefMsgCore.ShouldSendHeroStateToMaster(
            Grobal2Const.RC_PLAYOBJECT, true, Grobal2Const.RC_PLAYOBJECT,
            false, 999, 999, 0, 0, 10, 0));
    }

    [Fact]
    public void HeroWithoutMasterDoesNotSend()
    {
        Assert.False(SendRefMsgCore.ShouldSendHeroStateToMaster(
            Grobal2Const.RC_HEROOBJECT, false, Grobal2Const.RC_PLAYOBJECT,
            false, 0, 0, 0, 0, 10, 0));
    }

    [Fact]
    public void HeroWithNonPlayerMasterDoesNotSend()
    {
        Assert.False(SendRefMsgCore.ShouldSendHeroStateToMaster(
            Grobal2Const.RC_HEROOBJECT, true, Grobal2Const.RC_HEROOBJECT,
            false, 0, 0, 0, 0, 10, 0));
    }

    // ===================== 宽限期 =====================

    [Fact]
    public void GraceActiveWhenObjJustAdded()
    {
        // 加入不到 60 秒 → IsInRefStatusGrace 为假 → 走 else 正常广播
        Assert.False(SendRefMsgCore.IsInRefStatusGrace(false, now: 1000, objAddTime: 0));
    }

    [Fact]
    public void GraceInactiveAfter60s()
    {
        Assert.True(SendRefMsgCore.IsInRefStatusGrace(false, 60_000, 0));
    }

    [Fact]
    public void DenyRefStatusForcesFalse()
    {
        Assert.False(SendRefMsgCore.IsInRefStatusGrace(true, 999_999, 0));
    }
}

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J103：ClearObject(31537-31592) / CheckMasterViewRange(31594-31612) /
/// SearchViewRange(31614-31992) 决策层 1:1 测试。
/// </summary>
public sealed class ViewRangeMaintainCoreTests
{
    // ===================== ClearObject =====================

    [Fact]
    public void ErrorCodesAreOneTwoThree()
    {
        Assert.Equal(1, ViewRangeMaintainCore.ClearObjectErrorHumanList);
        Assert.Equal(2, ViewRangeMaintainCore.ClearObjectErrorActors);
        Assert.Equal(3, ViewRangeMaintainCore.ClearObjectErrorItems);
    }

    [Fact]
    public void ErrorMsgFormat()
    {
        Assert.Equal("[Exception] TBaseObject.ClearObject 2",
            ViewRangeMaintainCore.ClearObjectErrorMsg(2));
    }

    [Fact]
    public void ClearHumanListEmpties()
    {
        var list = new List<string> { "a", "b" };
        ViewRangeMaintainCore.ClearVisibleHumanList(list);
        Assert.Empty(list);
    }

    [Fact]
    public void ClearHumanListSkipsWhenEmpty()
    {
        // 31545：Count > 0 才动
        var list = new List<string>();
        ViewRangeMaintainCore.ClearVisibleHumanList(list);
        Assert.Empty(list);
    }

    [Fact]
    public void ClearActorsDisposesEachThenClears()
    {
        // 31558：**正序 to** 遍历，逐个 Dispose，最后 Clear
        var list = new List<object> { new(), new(), new() };
        var disposed = new List<object>();

        ViewRangeMaintainCore.ClearVisibleActors(list, o => disposed.Add(o));

        Assert.Equal(3, disposed.Count);
        Assert.Empty(list);
    }

    [Fact]
    public void ClearActorsVisitsInForwardOrder()
    {
        var a = new object();
        var b = new object();
        var c = new object();
        var list = new List<object> { a, b, c };
        var order = new List<object>();

        ViewRangeMaintainCore.ClearVisibleActors(list, o => order.Add(o));

        Assert.Equal(new[] { a, b, c }, order);   // 正序，非倒序
    }

    [Fact]
    public void ClearActorsSkipsNullEntries()
    {
        var list = new List<object?> { new(), null, new() };
        int n = 0;

        ViewRangeMaintainCore.ClearVisibleActors(list!, _ => n++);

        Assert.Equal(2, n);      // null 不释放
    }

    [Fact]
    public void ClearItemsUsesCodeThree()
    {
        var list = new List<object> { new() };
        string? msg = null;

        // 让 dispose 抛异常，验证错误序号为 3
        ViewRangeMaintainCore.ClearVisibleItems<object>(list, _ => throw new InvalidOperationException(), m => msg = m);

        Assert.Equal("[Exception] TBaseObject.ClearObject 3", msg);
    }

    [Fact]
    public void ClearActorsUsesCodeTwo()
    {
        var list = new List<object> { new() };
        string? msg = null;

        ViewRangeMaintainCore.ClearVisibleActors<object>(list, _ => throw new InvalidOperationException(), m => msg = m);

        Assert.Equal("[Exception] TBaseObject.ClearObject 2", msg);
    }

    [Fact]
    public void ClearEventsDisposesAndClears()
    {
        var list = new List<object> { new(), new() };
        int n = 0;

        ViewRangeMaintainCore.ClearVisibleEvents(list, _ => n++);

        Assert.Equal(2, n);
        Assert.Empty(list);
    }

    [Fact]
    public void ClearEventsReportsItsOwnMessage()
    {
        // 11371：消息中**没有** %d 序号，且仍写作 TBaseObject.ClearObject
        var list = new List<object> { new() };
        string? msg = null;

        ViewRangeMaintainCore.ClearVisibleEvents<object>(list, _ => throw new InvalidOperationException(), m => msg = m);

        Assert.Equal("[Exception] TBaseObject.ClearObject", msg);
        Assert.DoesNotContain(" 1", msg);
    }

    // ===================== CheckMasterViewRange =====================

    [Fact]
    public void NoMasterReturnsTrue()
    {
        // 31601：默认 True
        Assert.True(ViewRangeMaintainCore.CheckMasterViewRange(
            hasMaster: false, sameEnvir: false, 0, 0, 0, 999, 999));
    }

    [Fact]
    public void DifferentEnvirReturnsTrue()
    {
        // 31602：需 m_Master.m_PEnvir = m_PEnvir
        Assert.True(ViewRangeMaintainCore.CheckMasterViewRange(
            hasMaster: true, sameEnvir: false, masterX: 0, masterY: 0, masterViewRange: 5,
            selfX: 999, selfY: 999));
    }

    [Fact]
    public void WithinRangeReturnsTrue()
    {
        Assert.True(ViewRangeMaintainCore.CheckMasterViewRange(
            true, true, 100, 100, 10, 100, 100));
    }

    [Fact]
    public void OnBoundaryReturnsTrue()
    {
        // 31608：闭区间 >= <=
        Assert.True(ViewRangeMaintainCore.CheckMasterViewRange(true, true, 100, 100, 10, 110, 110));
        Assert.True(ViewRangeMaintainCore.CheckMasterViewRange(true, true, 100, 100, 10, 90, 90));
    }

    [Fact]
    public void OutsideRangeReturnsFalse()
    {
        Assert.False(ViewRangeMaintainCore.CheckMasterViewRange(true, true, 100, 100, 10, 111, 100));
        Assert.False(ViewRangeMaintainCore.CheckMasterViewRange(true, true, 100, 100, 10, 100, 111));
    }

    [Fact]
    public void BothAxesUseMasterViewRangeNoExtY()
    {
        // 31604-31607：**两侧都用主人的 viewRange，不加 ExtY**
        // 即 Y 方向容差与 X 相同
        Assert.False(ViewRangeMaintainCore.CheckMasterViewRange(true, true, 0, 0, 10, 0, 11));
        Assert.True(ViewRangeMaintainCore.CheckMasterViewRange(true, true, 0, 0, 10, 0, 10));
    }

    [Fact]
    public void SymmetricAroundMaster()
    {
        // 矩形以主人为中心对称
        Assert.True(ViewRangeMaintainCore.CheckMasterViewRange(true, true, 50, 60, 7, 43, 53));
        Assert.True(ViewRangeMaintainCore.CheckMasterViewRange(true, true, 50, 60, 7, 57, 67));
    }

    // ===================== SearchViewRange：离线闸门 =====================

    [Fact]
    public void NilEnvirExitsWithoutClearing()
    {
        // 31643-31647：**不清空**
        Assert.Equal(ViewRangeMaintainCore.ViewRangeAction.ExitNilEnvir,
            ViewRangeMaintainCore.SelectStartupAction(
                penvirIsNull: true, selfOffline: false, selfRaceServer: 0,
                hasMaster: false, masterOffline: false, masterRaceServer: 0));
    }

    [Fact]
    public void NilEnvirBeatsOffline()
    {
        // 顺序短路：PEnvir 为 nil 优先于离线
        Assert.Equal(ViewRangeMaintainCore.ViewRangeAction.ExitNilEnvir,
            ViewRangeMaintainCore.SelectStartupAction(true, true, Grobal2Const.RC_HEROOBJECT, true, true, 0));
    }

    [Fact]
    public void SelfOfflineClearsAndExits()
    {
        Assert.Equal(ViewRangeMaintainCore.ViewRangeAction.ClearAndExit,
            ViewRangeMaintainCore.SelectStartupAction(false, true, Grobal2Const.RC_PLAYOBJECT, false, false, 0));
    }

    [Fact]
    public void HeroWithOfflineMasterClears()
    {
        // 31658-31663
        Assert.Equal(ViewRangeMaintainCore.ViewRangeAction.ClearAndExit,
            ViewRangeMaintainCore.SelectStartupAction(
                false, false, Grobal2Const.RC_HEROOBJECT, hasMaster: true, masterOffline: true, 0));
    }

    [Fact]
    public void PlayerWithOfflinePlayerMasterClears()
    {
        // 31666-31671：主人须为玩家
        Assert.Equal(ViewRangeMaintainCore.ViewRangeAction.ClearAndExit,
            ViewRangeMaintainCore.SelectStartupAction(
                false, false, Grobal2Const.RC_PLAYOBJECT, true, true, Grobal2Const.RC_PLAYOBJECT));
    }

    [Fact]
    public void MasterNonPlayerOfflineDoesNotClear()
    {
        // 主人是英雄（非玩家）→ 31666 不成立
        Assert.Equal(ViewRangeMaintainCore.ViewRangeAction.Proceed,
            ViewRangeMaintainCore.SelectStartupAction(
                false, false, Grobal2Const.RC_PLAYOBJECT, true, true, Grobal2Const.RC_HEROOBJECT));
    }

    [Fact]
    public void HeroWithOnlineMasterProceeds()
    {
        Assert.Equal(ViewRangeMaintainCore.ViewRangeAction.Proceed,
            ViewRangeMaintainCore.SelectStartupAction(
                false, false, Grobal2Const.RC_HEROOBJECT, true, false, Grobal2Const.RC_PLAYOBJECT));
    }

    [Fact]
    public void MasterOfflineButSelfIsHeroAndMasterHero()
    {
        // 自己是英雄、主人也是英雄且离线：31658 只看"自己是英雄且有主人"，故成立
        Assert.Equal(ViewRangeMaintainCore.ViewRangeAction.ClearAndExit,
            ViewRangeMaintainCore.SelectStartupAction(
                false, false, Grobal2Const.RC_HEROOBJECT, true, true, Grobal2Const.RC_HEROOBJECT));
    }

    [Fact]
    public void RaceCheck31658DoesNotRequireMasterRace()
    {
        // **差异保护**：31658（英雄分支）不检查主人种族，31666（通用分支）检查
        var asHero = ViewRangeMaintainCore.SelectStartupAction(
            false, false, Grobal2Const.RC_HEROOBJECT, true, true, Grobal2Const.RC_HEROOBJECT);
        var asPlayer = ViewRangeMaintainCore.SelectStartupAction(
            false, false, Grobal2Const.RC_PLAYOBJECT, true, true, Grobal2Const.RC_HEROOBJECT);

        Assert.NotEqual(asHero, asPlayer);
    }

    // ===================== IsSlavePickItem（31685-31694） =====================

    [Fact]
    public void NoMasterMeansNotSlavePick()
    {
        Assert.False(ViewRangeMaintainCore.ComputeIsSlavePickItem(
            false, Grobal2Const.RC_PLAYOBJECT, true, 1, true, false, true, 1));
    }

    [Fact]
    public void MasterMustBeEligibleRace()
    {
        // 31682：主人须为 玩家/英雄/宠物主人
        Assert.False(ViewRangeMaintainCore.ComputeIsSlavePickItem(
            true, 80, true, 1, true, false, true, 1));    // 怪物主人
    }

    [Fact]
    public void EligibleMasterRaces()
    {
        foreach (int m in new[] { Grobal2Const.RC_PLAYOBJECT, Grobal2Const.RC_HEROOBJECT, Grobal2Const.RC_PLAYMOSTER })
            Assert.True(ViewRangeMaintainCore.ComputeIsSlavePickItem(
                true, m, false, 0, false, false, true, 1), $"主人种族 {m} 应合格");
    }

    [Fact]
    public void GamePetRequiresPlayerMaster()
    {
        // 31686：m_Master.m_btRaceServer = RC_PLAYOBJECT
        Assert.False(ViewRangeMaintainCore.ComputeIsSlavePickItem(
            true, Grobal2Const.RC_HEROOBJECT, true, 1, true, false, true, 1));
    }

    [Fact]
    public void GamePetEnablePickZeroUsesGlobalConfig()
    {
        // 31687：(enablePick = 0 and boEnabledPetPickup) or enablePick = 1
        Assert.True(ViewRangeMaintainCore.ComputeIsSlavePickItem(
            true, Grobal2Const.RC_PLAYOBJECT, true, 0, boEnabledPetPickup: true, false, true, 1));
        Assert.False(ViewRangeMaintainCore.ComputeIsSlavePickItem(
            true, Grobal2Const.RC_PLAYOBJECT, true, 0, boEnabledPetPickup: false, false, true, 1));
    }

    [Fact]
    public void GamePetEnablePickOneAlwaysTrue()
    {
        // enablePick = 1 → 不看全局配置
        Assert.True(ViewRangeMaintainCore.ComputeIsSlavePickItem(
            true, Grobal2Const.RC_PLAYOBJECT, true, 1, boEnabledPetPickup: false, false, false, 0));
    }

    [Fact]
    public void NonGamePetRequiresAllThree()
    {
        // 31692-31693
        Assert.True(ViewRangeMaintainCore.ComputeIsSlavePickItem(
            true, Grobal2Const.RC_PLAYOBJECT, boGamePet: false, 0, false,
            envirNoAutoRangePickItem: false, boSlaveAutoPickItem: true, nKeyUseClientPickItems: 1));

        Assert.False(ViewRangeMaintainCore.ComputeIsSlavePickItem(
            true, Grobal2Const.RC_PLAYOBJECT, false, 0, false, true, true, 1));    // 地图禁止
        Assert.False(ViewRangeMaintainCore.ComputeIsSlavePickItem(
            true, Grobal2Const.RC_PLAYOBJECT, false, 0, false, false, false, 1));  // 未开自动拾取
        Assert.False(ViewRangeMaintainCore.ComputeIsSlavePickItem(
            true, Grobal2Const.RC_PLAYOBJECT, false, 0, false, false, true, 0));   // 按键为 0
    }

    [Fact]
    public void GamePetAndNonGamePetBranchesDiffer()
    {
        // **差异保护**：同一组输入下两分支结论不同
        bool asPet = ViewRangeMaintainCore.ComputeIsSlavePickItem(
            true, Grobal2Const.RC_PLAYOBJECT, true, 0, true, false, false, 1);
        bool asSlave = ViewRangeMaintainCore.ComputeIsSlavePickItem(
            true, Grobal2Const.RC_PLAYOBJECT, false, 0, true, false, false, 1);

        Assert.True(asPet);
        Assert.False(asSlave);   // boSlaveAutoPickItem = false
    }

    // ===================== 物品可见性维护门 =====================

    [Fact]
    public void MaintainForThreeRaces()
    {
        foreach (int r in new[] { Grobal2Const.RC_PLAYOBJECT, Grobal2Const.RC_HEROOBJECT, Grobal2Const.RC_PLAYMOSTER })
            Assert.True(ViewRangeMaintainCore.ShouldMaintainVisibleItems(r, false));
    }

    [Fact]
    public void MaintainForSlavePicker()
    {
        // 31698：非限定种族但 IsSlavePickItem 为真
        Assert.True(ViewRangeMaintainCore.ShouldMaintainVisibleItems(80, true));
    }

    [Fact]
    public void MonsterNonPickerDoesNotMaintain()
    {
        Assert.False(ViewRangeMaintainCore.ShouldMaintainVisibleItems(80, false));
    }

    // ===================== 扫描边界（Y 只向上） =====================

    [Fact]
    public void SearchBoundsAreSymmetricWhenExtYZero()
    {
        var (sx, ex, sy, ey) = ViewRangeMaintainCore.ComputeSearchBounds(100, 200, 8, 0);
        Assert.Equal(92, sx);
        Assert.Equal(108, ex);
        Assert.Equal(192, sy);
        Assert.Equal(208, ey);
    }

    [Fact]
    public void SearchBoundsExtendYUpwardOnly()
    {
        // 31733-31736：**Y 下界不扩展，上界扩展**
        var (_, _, sy, ey) = ViewRangeMaintainCore.ComputeSearchBounds(0, 0, 10, 5);

        Assert.Equal(-10, sy);   // 下界不加 ExtY
        Assert.Equal(15, ey);    // 上界加 ExtY
    }

    [Fact]
    public void SearchBoundsDifferFromSendRefMsgBounds()
    {
        // **差异保护**：SearchViewRange 的 Y 只向上扩展，
        // SendRefMsg 的 Y 两侧都扩展
        var search = ViewRangeMaintainCore.ComputeSearchBounds(0, 0, 10, 5);
        var sendRef = SendRefMsgCore.ComputeScanBounds(0, 0, 10);

        // search: sy = -10, ey = 15（假设 SendRefMsg 的 ExtY=0 → -10..10）
        Assert.Equal(-10, sendRef.LY);
        Assert.Equal(10, sendRef.HY);
        Assert.NotEqual(sendRef.HY, search.EndY);   // 15 != 10
    }

    [Fact]
    public void SearchBoundsXUnaffectedByExtY()
    {
        var (sx, ex, _, _) = ViewRangeMaintainCore.ComputeSearchBounds(0, 0, 10, 99);
        Assert.Equal(-10, sx);
        Assert.Equal(10, ex);
    }

    // ===================== 宽限期（31796） =====================

    [Fact]
    public void GraceFalseWhenJustAdded()
    {
        Assert.False(ViewRangeMaintainCore.IsInRefStatusGrace(false, 1000, 0));
    }

    [Fact]
    public void GraceTrueAfter60s()
    {
        Assert.True(ViewRangeMaintainCore.IsInRefStatusGrace(false, 60_000, 0));
    }

    [Fact]
    public void GraceForcedFalseByDenyRefStatus()
    {
        Assert.False(ViewRangeMaintainCore.IsInRefStatusGrace(true, 999_999, 0));
    }

    [Fact]
    public void GraceMatchesSendRefMsgVersion()
    {
        // 两处原文一致，故两者结论应始终相同
        foreach (bool deny in new[] { false, true })
        foreach (uint now in new[] { 0u, 1000u, 60_000u, 999_999u })
        {
            Assert.Equal(
                SendRefMsgCore.IsInRefStatusGrace(deny, now, 0),
                ViewRangeMaintainCore.IsInRefStatusGrace(deny, now, 0));
        }
    }

    // ===================== 帧首置位 =====================

    [Fact]
    public void MarkAllStaleCoversBothTables()
    {
        var actors = new List<TVisibleBaseObject> { new() { nVisibleFlag = 1 } };
        var items = new List<TVisibleMapItem> { new() { nVisibleFlag = 2 } };

        ViewRangeMaintainCore.MarkVisibleAllStale(
            actors, (a, f) => a.nVisibleFlag = f,
            items, (i, f) => i.nVisibleFlag = f);

        Assert.Equal(0, actors[0].nVisibleFlag);
        Assert.Equal(0, items[0].nVisibleFlag);
    }

    [Fact]
    public void EntrySetsVisibleActiveFalse()
    {
        // 31679
        Assert.False(ViewRangeMaintainCore.BoIsVisibleActiveOnEntry);
    }

    // ===================== 帧末清理 =====================

    [Fact]
    public void FinishActorsRemovesStale()
    {
        var list = new List<TVisibleBaseObject>
        {
            new() { nVisibleFlag = 0 }, new() { nVisibleFlag = 1 }, new() { nVisibleFlag = 2 },
        };
        var disposed = new List<TVisibleBaseObject>();

        ViewRangeMaintainCore.FinishVisibleActors(list,
            v => v.nVisibleFlag, i => list.RemoveAt(i), v => disposed.Add(v));

        // 3 项中仅 1 项为 stale(0)，故剩 2 项（flag 1 与 2）
        Assert.Equal(2, list.Count);
        Assert.Single(disposed);
        Assert.DoesNotContain(list, v => v.nVisibleFlag == 0);
        Assert.Contains(list, v => v.nVisibleFlag == 1);
        Assert.Contains(list, v => v.nVisibleFlag == 2);
    }

    [Fact]
    public void FinishActorsIsReverseOrder()
    {
        // 31944：downto 0——倒序删除才能安全 RemoveAt
        var a = new TVisibleBaseObject { nVisibleFlag = 0 };
        var b = new TVisibleBaseObject { nVisibleFlag = 0 };
        var list = new List<TVisibleBaseObject> { a, b };
        var order = new List<TVisibleBaseObject>();

        ViewRangeMaintainCore.FinishVisibleActors(list,
            v => v.nVisibleFlag, i => list.RemoveAt(i), v => order.Add(v));

        Assert.Equal(new[] { b, a }, order);   // 先删后一个
        Assert.Empty(list);
    }

    [Fact]
    public void FinishItemsSkippedWhenNotMaintained()
    {
        // 31968：仅玩家/英雄/宠物主人 或 IsSlavePickItem 才清理
        var list = new List<TVisibleMapItem> { new() { nVisibleFlag = 0 } };

        ViewRangeMaintainCore.FinishVisibleItems(false, list,
            v => v.nVisibleFlag, i => list.RemoveAt(i), _ => { });

        Assert.Single(list);   // 未清理
    }

    [Fact]
    public void FinishItemsRunsWhenMaintained()
    {
        var list = new List<TVisibleMapItem> { new() { nVisibleFlag = 0 }, new() { nVisibleFlag = 1 } };

        ViewRangeMaintainCore.FinishVisibleItems(true, list,
            v => v.nVisibleFlag, i => list.RemoveAt(i), _ => { });

        Assert.Single(list);
        Assert.Equal(1, list[0].nVisibleFlag);
    }

    [Fact]
    public void FinishItemsReverseOrder()
    {
        var a = new TVisibleMapItem { nVisibleFlag = 0 };
        var b = new TVisibleMapItem { nVisibleFlag = 0 };
        var list = new List<TVisibleMapItem> { a, b };
        var order = new List<TVisibleMapItem>();

        ViewRangeMaintainCore.FinishVisibleItems(true, list,
            v => v.nVisibleFlag, i => list.RemoveAt(i), v => order.Add(v));

        Assert.Equal(new[] { b, a }, order);
    }

    [Fact]
    public void FinishKeepsFlagTwoEntries()
    {
        // 2 = 新建未发送：**不**被清除
        var list = new List<TVisibleMapItem> { new() { nVisibleFlag = 2 } };

        ViewRangeMaintainCore.FinishVisibleItems(true, list,
            v => v.nVisibleFlag, i => list.RemoveAt(i), _ => { });

        Assert.Single(list);
    }

    [Fact]
    public void ClearObjectForwardVsFinishReverse()
    {
        // **差异保护**：ClearObject 正序遍历（31558/31578，只释放不删元素），
        // 帧末清理倒序（31944/31972，边遍历边删）。二者方向相反且各有其因。
        var clearOrder = new List<object>();
        var list = new List<object> { new(), new() };
        var first = list[0];

        ViewRangeMaintainCore.ClearVisibleActors(list, o => clearOrder.Add(o));
        Assert.Same(first, clearOrder[0]);       // 正序：先第一个

        var f1 = new TVisibleMapItem { nVisibleFlag = 0 };
        var f2 = new TVisibleMapItem { nVisibleFlag = 0 };
        var flist = new List<TVisibleMapItem> { f1, f2 };
        var finOrder = new List<TVisibleMapItem>();

        ViewRangeMaintainCore.FinishVisibleItems(true, flist,
            v => v.nVisibleFlag, i => flist.RemoveAt(i), v => finOrder.Add(v));

        Assert.Same(f2, finOrder[0]);            // 倒序：先最后一个
    }
}

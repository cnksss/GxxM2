using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J104：UpdateVisibleGay(31430-31474) 与 SearchViewRange 内层准入判定
/// （角色 31845-31869 / 物品 31871-31886）1:1 测试。
/// </summary>
public sealed class VisibleDiscoveryCoreTests
{
    private static VisibleDiscoveryCore.SelfViewArgs Self(
        int race = 100, int x = 0, int y = 0, int range = 10, int extY = 0, int nation = 0,
        bool hasMaster = false, bool crazy = false, bool wantRef = false, bool noSameNation = false)
        => new()
        {
            RaceServer = race, CurX = x, CurY = y, ViewRange = range, ViewRangeExtY = extY,
            Nation = nation, HasMaster = hasMaster, BoCrazyMode = crazy,
            BoWantRefMsg = wantRef, BoNoSameNationMonPK = noSameNation,
        };

    /// <summary>一个"默认可见"的对象：玩家、非 ghost、非隐身、同地图、无主人。</summary>
    private static VisibleDiscoveryCore.ActorViewArgs Other(
        int race = 0, bool ghost = false, bool fixedHide = false, bool obMode = false,
        uint tick = 0, bool masterIsSelf = false, bool hasMaster = false,
        int x = 0, int y = 0, int nation = 0)
        => new()
        {
            RaceServer = race, BoGhost = ghost, BoFixedHideMode = fixedHide, BoObMode = obMode,
            ChangeModeExTick1 = tick, MasterIsSelf = masterIsSelf, HasMaster = hasMaster,
            CurX = x, CurY = y, Nation = nation,
        };

    // ===================== UpdateVisibleGay =====================

    [Fact]
    public void GayErrorMsgFormat()
    {
        Assert.Equal("[Exception] TBaseObject.UpdateVisibleGay-->ErrCode=6",
            VisibleDiscoveryCore.UpdateVisibleGayErrorMsg(6));
    }

    [Fact]
    public void NewActorEntryAlwaysGetsFlagTwo()
    {
        // 31452：**恒为 2**，无 boSendItemShow 分支
        var d = new Dictionary<nint, TVisibleBaseObject>();
        var r = VisibleDiscoveryCore.UpdateVisibleGay(d, 1, new object(), 80, false);

        Assert.True(r.Created);
        Assert.Equal(2, r.Entry.nVisibleFlag);
    }

    [Fact]
    public void ActorFlagTwoDiffersFromItemWithSendShow()
    {
        // **差异保护**：对象新建恒为 2；物品在 sendShow=True 时为 1
        var da = new Dictionary<nint, TVisibleBaseObject>();
        var di = new Dictionary<nint, TVisibleMapItem>();

        var actor = VisibleDiscoveryCore.UpdateVisibleGay(da, 1, new object(), 80, false);
        var item = VisibleItemLifecycleCore.UpdateVisibleItem(di, 1,
            new VisibleItemLifecycleCore.UpdateItemArgs { MapItem = new object(), BoSendItemShow = true });

        Assert.NotEqual(actor.Entry.nVisibleFlag, item.nVisibleFlag);
        Assert.Equal(2, actor.Entry.nVisibleFlag);
        Assert.Equal(1, item.nVisibleFlag);
    }

    [Fact]
    public void ExistingActorBecomesVisible()
    {
        var d = new Dictionary<nint, TVisibleBaseObject>();
        var obj = new object();

        VisibleDiscoveryCore.UpdateVisibleGay(d, 1, obj, 80, false);
        d[1].nVisibleFlag = 0;

        var r = VisibleDiscoveryCore.UpdateVisibleGay(d, 1, obj, 80, false);

        Assert.False(r.Created);
        Assert.Equal(1, r.Entry.nVisibleFlag);
    }

    [Fact]
    public void ExistingActorWrongObjectDoesNothing()
    {
        var d = new Dictionary<nint, TVisibleBaseObject>();
        VisibleDiscoveryCore.UpdateVisibleGay(d, 1, new object(), 80, false);
        d[1].nVisibleFlag = 0;

        var r = VisibleDiscoveryCore.UpdateVisibleGay(d, 1, new object(), 80, false);

        Assert.Equal(0, r.Entry.nVisibleFlag);
    }

    [Fact]
    public void NewActorStoresBaseObject()
    {
        var d = new Dictionary<nint, TVisibleBaseObject>();
        var obj = new object();
        var r = VisibleDiscoveryCore.UpdateVisibleGay(d, 7, obj, 80, false);

        Assert.Same(obj, r.Entry.BaseObject);
        Assert.Same(r.Entry, d[7]);
    }

    // ===================== m_boIsVisibleActive =====================

    [Fact]
    public void VisibleActiveSetForPlayer()
    {
        // 31439-31440：玩家
        Assert.True(VisibleDiscoveryCore.ShouldSetVisibleActive(Grobal2Const.RC_PLAYOBJECT, false));
    }

    [Fact]
    public void VisibleActiveSetForSlaveWithMaster()
    {
        // 31440：有主人的宝宝（任何种族）
        Assert.True(VisibleDiscoveryCore.ShouldSetVisibleActive(80, true));
        Assert.True(VisibleDiscoveryCore.ShouldSetVisibleActive(50, true));
    }

    [Fact]
    public void VisibleActiveNotSetForPlainMonster()
    {
        Assert.False(VisibleDiscoveryCore.ShouldSetVisibleActive(80, false));
    }

    [Fact]
    public void VisibleActiveNotSetForHeroWithoutMaster()
    {
        // 英雄若无主人（异常情形）不置位；有主人则由 hasMaster 分支置位
        Assert.False(VisibleDiscoveryCore.ShouldSetVisibleActive(Grobal2Const.RC_HEROOBJECT, false));
        Assert.True(VisibleDiscoveryCore.ShouldSetVisibleActive(Grobal2Const.RC_HEROOBJECT, true));
    }

    [Fact]
    public void UpdateGayReportsVisibleActiveSet()
    {
        var d = new Dictionary<nint, TVisibleBaseObject>();
        var r = VisibleDiscoveryCore.UpdateVisibleGay(d, 1, new object(), Grobal2Const.RC_PLAYOBJECT, false);
        Assert.True(r.VisibleActiveSet);
    }

    [Fact]
    public void UpdateGayNeverClearsVisibleActive()
    {
        // 本函数**只置 True、从不置 False**；False 由 31679 在入口统一置入
        var d = new Dictionary<nint, TVisibleBaseObject>();
        var r = VisibleDiscoveryCore.UpdateVisibleGay(d, 1, new object(), 80, false);
        Assert.False(r.VisibleActiveSet);   // 只表示"本次未置位"，而非"已置为假"
    }

    [Fact]
    public void EntrySetsVisibleActiveFalseAtSearchStart()
    {
        // 31679：入口统一置 False（与 J103 的 BoIsVisibleActiveOnEntry 同源）
        Assert.False(ViewRangeMaintainCore.BoIsVisibleActiveOnEntry);
    }

    // ===================== boTempFixedHideMode（31846-31848） =====================

    [Fact]
    public void TempHideOnlyForThreeRaces()
    {
        Assert.True(VisibleDiscoveryCore.ComputeTempFixedHideMode(Grobal2Const.RC_PLAYOBJECT, 1));
        Assert.True(VisibleDiscoveryCore.ComputeTempFixedHideMode(Grobal2Const.RC_HEROOBJECT, 1));
        Assert.True(VisibleDiscoveryCore.ComputeTempFixedHideMode(Grobal2Const.RC_PLAYMOSTER, 1));
    }

    [Fact]
    public void TempHideFalseForMonsterEvenWithTick()
    {
        Assert.False(VisibleDiscoveryCore.ComputeTempFixedHideMode(80, 999));
    }

    [Fact]
    public void TempHideFalseWhenTickZero()
    {
        Assert.False(VisibleDiscoveryCore.ComputeTempFixedHideMode(Grobal2Const.RC_PLAYOBJECT, 0));
    }

    [Fact]
    public void TempHideMatchesSendRefMsgRule()
    {
        // 与 J101 同规则，应逐组合一致
        foreach (int r in new[] { 0, 1, 80, 150, 50 })
        foreach (uint t in new[] { 0u, 1u, 999u })
            Assert.Equal(
                SendRefMsgCore.ComputeTempFixedHideMode(r, t),
                VisibleDiscoveryCore.ComputeTempFixedHideMode(r, t));
    }

    // ===================== 外层准入（31851-31852） =====================

    [Fact]
    public void OuterGatePassesForNormalCase()
    {
        Assert.True(VisibleDiscoveryCore.IsVisibleActorOuterGate(
            false, false, true, false, false, false));
    }

    [Fact]
    public void GhostFailsOuterGate()
    {
        Assert.False(VisibleDiscoveryCore.IsVisibleActorOuterGate(true, false, true, false, false, false));
    }

    [Fact]
    public void FixedHideFailsOuterGate()
    {
        Assert.False(VisibleDiscoveryCore.IsVisibleActorOuterGate(false, true, true, false, false, false));
    }

    [Fact]
    public void DifferentEnvirFailsOuterGate()
    {
        Assert.False(VisibleDiscoveryCore.IsVisibleActorOuterGate(false, false, false, false, false, false));
    }

    [Fact]
    public void ObModeFailsUnlessMasterIsSelf()
    {
        // 31852：隐身对象只对其主人可见
        Assert.False(VisibleDiscoveryCore.IsVisibleActorOuterGate(false, false, true, true, false, false));
        Assert.True(VisibleDiscoveryCore.IsVisibleActorOuterGate(false, false, true, true, false, true));
    }

    [Fact]
    public void TempHideFailsUnlessMasterIsSelf()
    {
        Assert.False(VisibleDiscoveryCore.IsVisibleActorOuterGate(false, false, true, false, true, false));
        Assert.True(VisibleDiscoveryCore.IsVisibleActorOuterGate(false, false, true, false, true, true));
    }

    [Fact]
    public void BothHideModesStillPassForMaster()
    {
        Assert.True(VisibleDiscoveryCore.IsVisibleActorOuterGate(false, false, true, true, true, true));
    }

    [Fact]
    public void OuterGateParenthesizationMatters()
    {
        // **关键**：④ 是 ((not ob) and (not temp)) or masterIsSelf，
        // 而非 (not ob) and ((not temp) or masterIsSelf)。
        // 二者在 ob=True, temp=False, masterIsSelf=True 时结论不同：
        //   正确写法：(False and True) or True = True
        //   错误写法：False and (True or True) = False
        Assert.True(VisibleDiscoveryCore.IsVisibleActorOuterGate(false, false, true, true, false, true));
    }

    // ===================== 内层准入（31855-31863） =====================

    [Fact]
    public void InnerGateTrueWhenSelfBelowAnimal()
    {
        // ①：种族 < 50（如玩家 0、英雄 1、守卫 11、和平NPC 15、盒子 30）
        foreach (int r in new[] { 0, 1, 11, 12, 15, 30, 49 })
            Assert.True(VisibleDiscoveryCore.IsVisibleActorInnerGate(
                Other(), Self(race: r)), $"种族 {r} 应命中 ①");
    }

    [Fact]
    public void InnerGateFalseForHighRaceWithoutOtherConditions()
    {
        // 种族 >= 50 且其余都不满足
        Assert.False(VisibleDiscoveryCore.IsVisibleActorInnerGate(Other(race: 80), Self(race: 80)));
    }

    [Fact]
    public void InnerGateTrueWhenSelfHasMaster()
    {
        // ②
        Assert.True(VisibleDiscoveryCore.IsVisibleActorInnerGate(Other(race: 80), Self(race: 80, hasMaster: true)));
    }

    [Fact]
    public void InnerGateTrueWhenCrazy()
    {
        // ③
        Assert.True(VisibleDiscoveryCore.IsVisibleActorInnerGate(Other(race: 80), Self(race: 80, crazy: true)));
    }

    [Fact]
    public void InnerGateTrueWhenWantRefMsg()
    {
        // ④
        Assert.True(VisibleDiscoveryCore.IsVisibleActorInnerGate(Other(race: 80), Self(race: 80, wantRef: true)));
    }

    [Fact]
    public void InnerGateTrueWhenOtherIsPlayer()
    {
        // ⑥
        Assert.True(VisibleDiscoveryCore.IsVisibleActorInnerGate(
            Other(race: Grobal2Const.RC_PLAYOBJECT), Self(race: 80)));
    }

    [Fact]
    public void InnerGateFiveRequiresMasterAndDistance()
    {
        // ⑤：对方有主人 + 距离在**自身**视野内
        Assert.True(VisibleDiscoveryCore.IsVisibleActorInnerGate(
            Other(race: 80, hasMaster: true, x: 5, y: 5), Self(race: 80, x: 0, y: 0, range: 10)));

        // 无主人则不成立
        Assert.False(VisibleDiscoveryCore.IsVisibleActorInnerGate(
            Other(race: 80, hasMaster: false, x: 5, y: 5), Self(race: 80, x: 0, y: 0, range: 10)));
    }

    [Fact]
    public void InnerGateFiveUsesSelfViewRangeAndExtY()
    {
        // 31858-31859：距离门限用**自身的** range 与 extY
        // Y 容差 = 10 + 5 = 15
        Assert.True(VisibleDiscoveryCore.IsVisibleActorInnerGate(
            Other(race: 80, hasMaster: true, x: 0, y: 15),
            Self(race: 80, x: 0, y: 0, range: 10, extY: 5)));

        Assert.False(VisibleDiscoveryCore.IsVisibleActorInnerGate(
            Other(race: 80, hasMaster: true, x: 0, y: 16),
            Self(race: 80, x: 0, y: 0, range: 10, extY: 5)));
    }

    [Fact]
    public void InnerGateFiveXDoesNotUseExtY()
    {
        // X 容差仍为 10（extY 只加在 Y）
        Assert.False(VisibleDiscoveryCore.IsVisibleActorInnerGate(
            Other(race: 80, hasMaster: true, x: 11, y: 0),
            Self(race: 80, x: 0, y: 0, range: 10, extY: 5)));
    }

    [Fact]
    public void InnerGateSevenNationRule()
    {
        // ⑦：双方 nation > 0 且 noSameNationMonPK 且国号不同
        Assert.True(VisibleDiscoveryCore.IsVisibleActorInnerGate(
            Other(race: 80, nation: 2),
            Self(race: 80, nation: 1, noSameNation: true)));
    }

    [Fact]
    public void InnerGateSevenRequiresAllThree()
    {
        // 自己 nation = 0
        Assert.False(VisibleDiscoveryCore.IsVisibleActorInnerGate(
            Other(race: 80, nation: 2), Self(race: 80, nation: 0, noSameNation: true)));

        // 对方 nation = 0
        Assert.False(VisibleDiscoveryCore.IsVisibleActorInnerGate(
            Other(race: 80, nation: 0), Self(race: 80, nation: 1, noSameNation: true)));

        // 未开同国 PK 限制
        Assert.False(VisibleDiscoveryCore.IsVisibleActorInnerGate(
            Other(race: 80, nation: 2), Self(race: 80, nation: 1, noSameNation: false)));

        // 同国
        Assert.False(VisibleDiscoveryCore.IsVisibleActorInnerGate(
            Other(race: 80, nation: 1), Self(race: 80, nation: 1, noSameNation: true)));
    }

    [Fact]
    public void InnerGateConditionsAreOrNotAnd()
    {
        // 只满足 ⑦ 一项即应为真（证明是 or）
        Assert.True(VisibleDiscoveryCore.IsVisibleActorInnerGate(
            Other(race: 80, nation: 2, ghost: false, hasMaster: false),
            Self(race: 80, nation: 1, noSameNation: true, crazy: false, wantRef: false)));
    }

    // ===================== 完整准入 =====================

    [Fact]
    public void FullGateRequiresBothLayers()
    {
        // 外层失败 → 整体失败，即使内层满足
        Assert.False(VisibleDiscoveryCore.ShouldRegisterVisibleActor(
            Self(race: 80), Other(race: Grobal2Const.RC_PLAYOBJECT, ghost: true), sameEnvir: true));
    }

    [Fact]
    public void FullGatePassesNormalPlayer()
    {
        Assert.True(VisibleDiscoveryCore.ShouldRegisterVisibleActor(
            Self(race: 80), Other(race: Grobal2Const.RC_PLAYOBJECT), true));
    }

    [Fact]
    public void FullGateHiddenActorVisibleOnlyToMaster()
    {
        // 隐身对象：对主人可见，对旁人不可见
        var self = Self(race: 80);
        var hidden = Other(race: Grobal2Const.RC_PLAYOBJECT, obMode: true);

        Assert.False(VisibleDiscoveryCore.ShouldRegisterVisibleActor(self, hidden, true));

        hidden.MasterIsSelf = true;
        Assert.True(VisibleDiscoveryCore.ShouldRegisterVisibleActor(self, hidden, true));
    }

    [Fact]
    public void FullGateDifferentEnvirFails()
    {
        Assert.False(VisibleDiscoveryCore.ShouldRegisterVisibleActor(
            Self(race: 80), Other(race: Grobal2Const.RC_PLAYOBJECT), sameEnvir: false));
    }

    [Fact]
    public void FullGateTempHideUsesObjectRace()
    {
        // 31846-31848：怪物 race=80 带 tick **不算**限时隐身（走 else 分支 → False），
        // 故外层门槛得以通过；但内层无一条件成立（自身 race 80 不 < 50、
        // 自身无主人/不狂暴/不需引用消息、对方无主人、对方非玩家、双方国号均为 0），
        // 因此整体仍为 False —— 外层通过不等于整体通过。
        Assert.False(VisibleDiscoveryCore.ShouldRegisterVisibleActor(
            Self(race: 80), Other(race: 80, tick: 999, nation: 0), true));
    }

    [Fact]
    public void TempHideRaceIsReadFromObjectNotSelf()
    {
        // 31846 读的是**对象**的种族：对象是玩家且 tick > 0 → 限时隐身生效 → 外层被拦
        Assert.False(VisibleDiscoveryCore.ShouldRegisterVisibleActor(
            Self(race: 80), Other(race: Grobal2Const.RC_PLAYOBJECT, tick: 999), true));

        // 同对象但 tick = 0 → 不隐身 → 内层 ⑥（对方是玩家）命中 → True
        Assert.True(VisibleDiscoveryCore.ShouldRegisterVisibleActor(
            Self(race: 80), Other(race: Grobal2Const.RC_PLAYOBJECT, tick: 0), true));
    }

    [Fact]
    public void TempHiddenPlayerStillVisibleToItsMaster()
    {
        // 31852 的括号：限时隐身只对主人让路
        Assert.True(VisibleDiscoveryCore.ShouldRegisterVisibleActor(
            Self(race: 80),
            Other(race: Grobal2Const.RC_PLAYOBJECT, tick: 999, masterIsSelf: true),
            true));
    }

    // ===================== 物品准入（31874-31884） =====================

    [Fact]
    public void PlayerDoesNotScanItems()
    {
        // **关键**：玩家自己不在 {HERO, PLAYMOSTER} 内，且非奴隶拾取者
        Assert.False(VisibleDiscoveryCore.ShouldScanItems(Grobal2Const.RC_PLAYOBJECT, false));
    }

    [Fact]
    public void HeroScansItems()
    {
        Assert.True(VisibleDiscoveryCore.ShouldScanItems(Grobal2Const.RC_HEROOBJECT, false));
    }

    [Fact]
    public void PlayMosterScansItems()
    {
        Assert.True(VisibleDiscoveryCore.ShouldScanItems(Grobal2Const.RC_PLAYMOSTER, false));
    }

    [Fact]
    public void SlavePickerScansItems()
    {
        Assert.True(VisibleDiscoveryCore.ShouldScanItems(80, true));
    }

    [Fact]
    public void MonsterNonPickerDoesNotScanItems()
    {
        Assert.False(VisibleDiscoveryCore.ShouldScanItems(80, false));
    }

    [Fact]
    public void ItemRegisteredWhenNotGhost()
    {
        Assert.True(VisibleDiscoveryCore.ShouldRegisterVisibleItem(false));
    }

    [Fact]
    public void GhostItemNotRegistered()
    {
        Assert.False(VisibleDiscoveryCore.ShouldRegisterVisibleItem(true));
    }

    [Fact]
    public void ItemGateIsMuchSimplerThanActorGate()
    {
        // **差异保护**：角色准入需要外层四条件（含同地图、隐身、Ghost），
        // 物品准入只看 ghost。故"不同地图"对物品无影响、对角色致命。
        var itemOk = VisibleDiscoveryCore.ShouldRegisterVisibleItem(false);
        var actorDifferentEnvir = VisibleDiscoveryCore.ShouldRegisterVisibleActor(
            Self(race: 80), Other(race: Grobal2Const.RC_PLAYOBJECT), sameEnvir: false);

        Assert.True(itemOk);
        Assert.False(actorDifferentEnvir);
    }

    [Fact]
    public void ItemScanAndActorScanRaceSetsDiffer()
    {
        // 物品扫描门（{HERO, PLAYMOSTER} ∪ 奴隶）与
        // 物品维护门（J103：{PLAY, HERO, PLAYMOSTER} ∪ 奴隶）**不同**：
        // 玩家在维护门内但**不在**扫描门内
        foreach (int r in new[] { Grobal2Const.RC_PLAYOBJECT, Grobal2Const.RC_HEROOBJECT, Grobal2Const.RC_PLAYMOSTER })
        {
            bool scan = VisibleDiscoveryCore.ShouldScanItems(r, false);
            bool maintain = ViewRangeMaintainCore.ShouldMaintainVisibleItems(r, false);

            if (r == Grobal2Const.RC_PLAYOBJECT)
            {
                Assert.False(scan);
                Assert.True(maintain);
            }
            else
            {
                Assert.Equal(scan, maintain);
            }
        }
    }

    // ===================== 与 J102 物品路径的一致性 =====================

    [Fact]
    public void DiscoveryPathCreatesItemWithFlagTwo()
    {
        // 31882 调用 UpdateVisibleItem 时省略 boSendItemShow（默认 False）→ 初值 2
        var d = new Dictionary<nint, TVisibleMapItem>();
        var item = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1,
            new VisibleItemLifecycleCore.UpdateItemArgs
            {
                MapItem = new object(),
                BoSendItemShow = false,      // 31882 的默认值
                WX = 3, WY = 4,
            });

        Assert.Equal(2, item.nVisibleFlag);
        Assert.Equal(3, item.nX);
        Assert.Equal(4, item.nY);
    }

    [Fact]
    public void DiscoveryPathUsesCellCoordinates()
    {
        // 31882：传入的是循环变量 n18/n1C（格子坐标），非对象坐标
        var d = new Dictionary<nint, TVisibleMapItem>();
        var item = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1,
            new VisibleItemLifecycleCore.UpdateItemArgs
            {
                MapItem = new object(), BoSendItemShow = false, WX = 77, WY = 88,
            });

        Assert.Equal(77, item.nX);
        Assert.Equal(88, item.nY);
    }
}

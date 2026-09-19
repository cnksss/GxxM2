using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J102：可见物品三态生命周期 1:1 测试
/// （UpdateVisibleItem 31476-31530 / HideItem 11330-11342 / UpdateVisibleEvent 11344-11364
///  + CheckOverLapItem 11028 / GetDropItemValue 33204-33265）。
/// </summary>
public sealed class VisibleItemLifecycleCoreTests
{
    private static VisibleItemLifecycleCore.UpdateItemArgs Args(
        object? mapItem = null, bool sendShow = false, int x = 10, int y = 20,
        ushort effect = 0, int dropIdx = -1, int dropVal = 0, int dura = 0,
        bool stdFound = false, bool overlap = false, bool cfg83 = true,
        string name = "n", string dbName = "")
    {
        return new VisibleItemLifecycleCore.UpdateItemArgs
        {
            MapItem = mapItem ?? new object(),
            BoSendItemShow = sendShow,
            WX = x,
            WY = y,
            UserItemEffect = effect,
            DropEffectIndex = dropIdx,
            DropEffectValue = dropVal,
            UserItemDura = dura,
            StdItemFound = stdFound,
            IsOverLap = overlap,
            ClientConfig83 = cfg83,
            MapItemName = name,
            MapItemDBName = dbName,
        };
    }

    // ===================== MakeLong / HiWord / LoWord =====================

    [Fact]
    public void MakeLongPacksLoAndHi()
    {
        // Delphi MakeLong(a,b) = a or (b shl 16)
        Assert.Equal(0x0001_0002, VisibleItemLifecycleCore.MakeLong(2, 1));
    }

    [Fact]
    public void MakeLongMasksBothTo16Bits()
    {
        // 两参数均按 $FFFF 掩码：0x1FFFF → 0xFFFF
        int v = VisibleItemLifecycleCore.MakeLong(0x1FFFF, 0x1FFFF);
        Assert.Equal(0xFFFF, VisibleItemLifecycleCore.LoWord(v));
        Assert.Equal(0xFFFF, VisibleItemLifecycleCore.HiWord(v));
    }

    [Fact]
    public void HiLoWordRoundTrip()
    {
        int v = VisibleItemLifecycleCore.MakeLong(1234, 5678);
        Assert.Equal(1234, VisibleItemLifecycleCore.LoWord(v));
        Assert.Equal(5678, VisibleItemLifecycleCore.HiWord(v));
    }

    // ===================== CheckOverLapItem（11028-11032） =====================

    [Fact]
    public void OverLapRequiresAllThreeConditions()
    {
        // OverLap > 0 且 StdMode 在集合内 且 DuraMax > 1
        Assert.True(VisibleItemLifecycleCore.CheckOverLapItem(1, 0, 2));
        Assert.False(VisibleItemLifecycleCore.CheckOverLapItem(0, 0, 2));   // OverLap = 0
        Assert.False(VisibleItemLifecycleCore.CheckOverLapItem(1, 99, 2));  // StdMode 不在集合
        Assert.False(VisibleItemLifecycleCore.CheckOverLapItem(1, 0, 1));   // DuraMax = 1
    }

    [Fact]
    public void OverLapStdModeSetIsExactlyNine()
    {
        int[] expected = { 0, 2, 3, 31, 40, 41, 42, 46, 47 };

        // 集合内 9 个值全部命中
        foreach (int m in expected)
            Assert.True(VisibleItemLifecycleCore.IsOverLapStdMode(m), $"StdMode {m} 应在集合内");

        // 集合外若干常见值
        foreach (int m in new[] { 1, 4, 5, 6, 10, 11, 15, 19, 20, 21, 23, 24, 26, 28, 29, 30, 53, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 82, 84, 86 })
            Assert.False(VisibleItemLifecycleCore.IsOverLapStdMode(m), $"StdMode {m} 不应在集合内");
    }

    [Fact]
    public void DuraMaxExactlyTwoIsEnough()
    {
        // 条件是 > 1，故 2 即满足
        Assert.True(VisibleItemLifecycleCore.CheckOverLapItem(1, 0, 2));
        Assert.False(VisibleItemLifecycleCore.CheckOverLapItem(1, 0, 1));
    }

    // ===================== GetDropItemValue（33204-33265） =====================

    private static byte[] V(params byte[] v)
    {
        var a = new byte[8];
        for (int i = 0; i < v.Length && i < 8; i++) a[i] = v[i];
        return a;
    }

    [Fact]
    public void NullStdItemReturnsZero()
    {
        Assert.Equal(0, VisibleItemLifecycleCore.GetDropItemValue(true, false, true, 5, V(1)));
    }

    [Fact]
    public void NullUserItemReturnsZero()
    {
        Assert.Equal(0, VisibleItemLifecycleCore.GetDropItemValue(false, true, true, 5, V(1)));
    }

    [Fact]
    public void DisabledConfig83ReturnsZero()
    {
        // 33216-33217：ClientConfigs[83] 为假直接 Exit
        Assert.Equal(0, VisibleItemLifecycleCore.GetDropItemValue(false, false, false, 5, V(1, 1, 1)));
    }

    [Fact]
    public void GroupAReturnsOneWhenAnyPositive()
    {
        foreach (int m in new[] { 5, 6, 68, 69 })
            Assert.Equal(1, VisibleItemLifecycleCore.GetDropItemValue(false, false, true, m, V(0, 0, 0, 0, 0, 0, 0, 1)));
    }

    [Fact]
    public void GroupAReturnsZeroWhenAllZero()
    {
        Assert.Equal(0, VisibleItemLifecycleCore.GetDropItemValue(false, false, true, 5, V()));
    }

    [Fact]
    public void GroupAWatchesEightSlots()
    {
        // 组 A 看 btValue[0..7]：第 8 个槽（下标 7）也参与
        Assert.Equal(1, VisibleItemLifecycleCore.GetDropItemValue(
            false, false, true, 5, V(0, 0, 0, 0, 0, 0, 0, 1)));
    }

    [Fact]
    public void OtherGroupsWatchFiveSlots()
    {
        // 组 B 只看 btValue[0..4]：下标 5 有值也不影响
        Assert.Equal(0, VisibleItemLifecycleCore.GetDropItemValue(
            false, false, true, 10, V(0, 0, 0, 0, 0, 1, 1, 1)));
    }

    [Fact]
    public void OtherGroupsDetectWithinFiveSlots()
    {
        Assert.Equal(1, VisibleItemLifecycleCore.GetDropItemValue(
            false, false, true, 10, V(0, 0, 0, 0, 1, 0, 0, 0)));
    }

    [Fact]
    public void GroupBEightSlotsWatchingDiffersFromGroupA()
    {
        // **差异化断言**：同一 btValue，组 A 得 1、组 B 得 0
        byte[] onlySlot7 = V(0, 0, 0, 0, 0, 0, 0, 1);
        Assert.NotEqual(
            VisibleItemLifecycleCore.GetDropItemValue(false, false, true, 5, onlySlot7),
            VisibleItemLifecycleCore.GetDropItemValue(false, false, true, 10, onlySlot7));
    }

    [Fact]
    public void UnlistedStdModeReturnsZero()
    {
        // 未列出的 StdMode 保持 boRet = False
        foreach (int m in new[] { 0, 1, 2, 3, 4, 7, 8, 9, 13, 14, 17, 18, 25, 27, 31, 32, 40, 99 })
            Assert.Equal(0, VisibleItemLifecycleCore.GetDropItemValue(false, false, true, m, V(1, 1, 1, 1, 1, 1, 1, 1)));
    }

    [Fact]
    public void GroupBStdModeTableHasThirtyEightEntries()
    {
        // 由原文 33222-33257 脚本抽取：全部 case 标签共 42 项，去掉组 A 的 4 项（5,6,68,69）= 38
        Assert.Equal(38, VisibleItemLifecycleCore.GroupBStdModes.Length);
    }

    [Fact]
    public void AllCaseLabelsTotalFortyTwo()
    {
        // 冗余保护：组 A(4) + 组 B(38) 应等于原文全部 case 标签数
        Assert.Equal(42, 4 + VisibleItemLifecycleCore.GroupBStdModes.Length);
    }

    [Fact]
    public void GroupBTableContents()
    {
        foreach (int m in new[] { 10, 11, 12, 28, 65, 66, 67, 19, 70, 75, 20, 24, 52, 71, 76, 79, 86,
                                   21, 54, 72, 77, 84, 23, 74, 82, 53,
                                   15, 16, 22, 26, 51, 62, 63, 64, 29, 30, 73, 78 })
            Assert.True(VisibleItemLifecycleCore.IsGroupB(m), $"StdMode {m} 应在组 B");
    }

    [Fact]
    public void GroupAAndBRNotOverlapping()
    {
        foreach (int m in VisibleItemLifecycleCore.GroupBStdModes)
            Assert.False(VisibleItemLifecycleCore.IsGroupA(m), $"StdMode {m} 不应同时在组 A");
    }

    [Fact]
    public void ReturnsByteNotCount()
    {
        // 即使 8 个槽都有值，也返回 1（而非 8）
        Assert.Equal(1, VisibleItemLifecycleCore.GetDropItemValue(
            false, false, true, 5, V(1, 2, 3, 4, 5, 6, 7, 8)));
    }

    // ===================== UpdateVisibleItem：新建与初值 =====================

    [Fact]
    public void NewItemWithSendShowGetsFlagOne()
    {
        // 31487-31490：已发送可见消息 → 1
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(sendShow: true));

        Assert.Equal(VisibleItemLifecycleCore.FlagVisible, it.nVisibleFlag);
        Assert.Equal(1, it.nVisibleFlag);
    }

    [Fact]
    public void NewItemWithoutSendShowGetsFlagTwo()
    {
        // 31489：未发送 → 2
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(sendShow: false));

        Assert.Equal(VisibleItemLifecycleCore.FlagNewUnsent, it.nVisibleFlag);
        Assert.Equal(2, it.nVisibleFlag);
    }

    [Fact]
    public void NewItemFlagsAreDistinct()
    {
        // **差异保护**：两个初值必须不同，写反会导致新物品被立刻清掉或永远重复发送
        var d1 = new Dictionary<nint, TVisibleMapItem>();
        var d2 = new Dictionary<nint, TVisibleMapItem>();

        var a = VisibleItemLifecycleCore.UpdateVisibleItem(d1, 1, Args(sendShow: true));
        var b = VisibleItemLifecycleCore.UpdateVisibleItem(d2, 1, Args(sendShow: false));

        Assert.NotEqual(a.nVisibleFlag, b.nVisibleFlag);
    }

    [Fact]
    public void NewItemCopiesPositionAndColor()
    {
        var d = new Dictionary<nint, TVisibleMapItem>();
        var args = Args(x: 33, y: 44);
        args.MapItemColor = 7;

        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, args);

        Assert.Equal(33, it.nX);
        Assert.Equal(44, it.nY);
        Assert.Equal(7, it.btColor2);
    }

    [Fact]
    public void NewItemSendHideMsgStartsFalse()
    {
        // 31495
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args());

        Assert.False(it.boSendHideMsg);
    }

    [Fact]
    public void UpdateExistingBecomesVisible()
    {
        // 31525-31527：已存在且引用相等 → 置 1
        var d = new Dictionary<nint, TVisibleMapItem>();
        var obj = new object();

        VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(mapItem: obj, sendShow: false));
        d[1].nVisibleFlag = VisibleItemLifecycleCore.FlagStale;   // 模拟帧首置 0

        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(mapItem: obj, sendShow: false));

        Assert.Equal(VisibleItemLifecycleCore.FlagVisible, it.nVisibleFlag);
    }

    [Fact]
    public void UpdateExistingWrongObjectDoesNothing()
    {
        // 31525：**引用不相等则不置位**，保持 0
        var d = new Dictionary<nint, TVisibleMapItem>();

        VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(mapItem: new object(), sendShow: true));
        d[1].nVisibleFlag = VisibleItemLifecycleCore.FlagStale;

        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(mapItem: new object(), sendShow: true));

        Assert.Equal(VisibleItemLifecycleCore.FlagStale, it.nVisibleFlag);
    }

    [Fact]
    public void UpdateExistingDoesNotOverwriteFields()
    {
        // 已存在分支只改 nVisibleFlag，不动 nX/nY
        var d = new Dictionary<nint, TVisibleMapItem>();
        var obj = new object();

        VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(mapItem: obj, x: 1, y: 2, sendShow: true));
        VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(mapItem: obj, x: 99, y: 98, sendShow: true));

        Assert.Equal(1, d[1].nX);   // 仍是首次的值
        Assert.Equal(2, d[1].nY);
    }

    // ===================== UpdateVisibleItem：特效与叠加 =====================

    [Fact]
    public void ItemOwnEffectTakesPriority()
    {
        // 31497-31505：自带特效非 0 则不查掉落表
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1,
            Args(effect: 42, dropIdx: 0, dropVal: 99));

        Assert.Equal(42, VisibleItemLifecycleCore.HiWord(it.OverlapCountAndEffect));
    }

    [Fact]
    public void FallsBackToDropEffectTableWhenEffectZero()
    {
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1,
            Args(effect: 0, dropIdx: 0, dropVal: 99));

        Assert.Equal(99, VisibleItemLifecycleCore.HiWord(it.OverlapCountAndEffect));
    }

    [Fact]
    public void DropEffectNotFoundLeavesZero()
    {
        // 31500-31501：IndexOf = -1 时不取
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1,
            Args(effect: 0, dropIdx: -1, dropVal: 99));

        Assert.Equal(0, VisibleItemLifecycleCore.HiWord(it.OverlapCountAndEffect));
    }

    [Fact]
    public void OverLapItemStoresDuraPlusOne()
    {
        // 31507-31509：可叠加 → 低 16 位为 Dura + 1
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1,
            Args(dura: 9, stdFound: true, overlap: true, effect: 3));

        Assert.Equal(10, VisibleItemLifecycleCore.LoWord(it.OverlapCountAndEffect));
        Assert.Equal(3, VisibleItemLifecycleCore.HiWord(it.OverlapCountAndEffect));
    }

    [Fact]
    public void NonOverLapItemStoresZeroCount()
    {
        // 31513：不可叠加 → 低 16 位为 0
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1,
            Args(dura: 9, stdFound: true, overlap: false, effect: 3));

        Assert.Equal(0, VisibleItemLifecycleCore.LoWord(it.OverlapCountAndEffect));
        Assert.Equal(3, VisibleItemLifecycleCore.HiWord(it.OverlapCountAndEffect));
    }

    [Fact]
    public void StdItemMissingMeansNotOverLap()
    {
        // 31507：StdItem = nil 时短路，走 else
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1,
            Args(dura: 9, stdFound: false, overlap: true, effect: 3));

        Assert.Equal(0, VisibleItemLifecycleCore.LoWord(it.OverlapCountAndEffect));
    }

    // ===================== UpdateVisibleItem：名称拼接（31516-31519） =====================

    [Fact]
    public void NameUsesSlashWhenDBNameDiffers()
    {
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1,
            Args(name: "屠龙", dbName: "Dragon"));

        Assert.Equal("屠龙/Dragon", it.sName);
    }

    [Fact]
    public void NamePlainWhenDBNameEmpty()
    {
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1,
            Args(name: "屠龙", dbName: ""));

        Assert.Equal("屠龙", it.sName);
    }

    [Fact]
    public void NamePlainWhenNameEqualsDBName()
    {
        // 31516：CompareText 相等则不拼接
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1,
            Args(name: "Sword", dbName: "Sword"));

        Assert.Equal("Sword", it.sName);
    }

    [Fact]
    public void NameComparisonIsCaseInsensitive()
    {
        // CompareText 是不区分大小写的
        var d = new Dictionary<nint, TVisibleMapItem>();
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1,
            Args(name: "sword", dbName: "SWORD"));

        Assert.Equal("sword", it.sName);      // 不拼接
    }

    [Fact]
    public void LooksCopied()
    {
        var d = new Dictionary<nint, TVisibleMapItem>();
        var args = Args();
        args.MapItemLooks = 1234;

        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, args);

        Assert.Equal(1234, it.wLooks);
    }

    // ===================== HideItem（11330-11342） =====================

    [Fact]
    public void HideItemSetsFlagOnly()
    {
        var d = new Dictionary<nint, TVisibleMapItem>();
        var obj = new object();
        VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(mapItem: obj, sendShow: true));

        Assert.True(VisibleItemLifecycleCore.HideItem(d, 1, obj));
        Assert.True(d[1].boSendHideMsg);
    }

    [Fact]
    public void HideItemDoesNotChangeVisibleFlag()
    {
        // **关键**：HideItem 不改 nVisibleFlag，也不删条目
        var d = new Dictionary<nint, TVisibleMapItem>();
        var obj = new object();
        VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(mapItem: obj, sendShow: true));
        byte before = d[1].nVisibleFlag;

        VisibleItemLifecycleCore.HideItem(d, 1, obj);

        Assert.Equal(before, d[1].nVisibleFlag);
        Assert.Single(d);
    }

    [Fact]
    public void HideItemFailsOnWrongObject()
    {
        var d = new Dictionary<nint, TVisibleMapItem>();
        VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(mapItem: new object()));

        Assert.False(VisibleItemLifecycleCore.HideItem(d, 1, new object()));
        Assert.False(d[1].boSendHideMsg);
    }

    [Fact]
    public void HideItemFailsOnMissingKey()
    {
        var d = new Dictionary<nint, TVisibleMapItem>();
        Assert.False(VisibleItemLifecycleCore.HideItem(d, 1, new object()));
    }

    [Fact]
    public void HideItemFailsWhenBaseObjectNull()
    {
        // 11337：BaseObject <> nil 是必要条件
        var d = new Dictionary<nint, TVisibleMapItem>
        {
            [1] = new TVisibleMapItem { BaseObject = null, nVisibleFlag = 1 },
        };

        Assert.False(VisibleItemLifecycleCore.HideItem(d, 1, null));
    }

    // ===================== UpdateVisibleEvent（11344-11364） =====================

    [Fact]
    public void NewEventAlwaysGetsFlagTwo()
    {
        // 11356：**恒为 2**，无 boSendItemShow 分支
        var d = new Dictionary<string, TVisibleMapEvent>();
        var ev = VisibleItemLifecycleCore.UpdateVisibleEvent(d, "k", 5, 6, new object());

        Assert.Equal(2, ev.nVisibleFlag);
    }

    [Fact]
    public void EventFlagTwoDiffersFromItemWithSendShow()
    {
        // **差异保护**：事件新建恒为 2，物品在 sendShow 时为 1
        var de = new Dictionary<string, TVisibleMapEvent>();
        var di = new Dictionary<nint, TVisibleMapItem>();

        var ev = VisibleItemLifecycleCore.UpdateVisibleEvent(de, "k", 0, 0, new object());
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(di, 1, Args(sendShow: true));

        Assert.NotEqual(ev.nVisibleFlag, it.nVisibleFlag);
    }

    [Fact]
    public void NewEventCopiesCoords()
    {
        var d = new Dictionary<string, TVisibleMapEvent>();
        var ev = VisibleItemLifecycleCore.UpdateVisibleEvent(d, "k", 77, 88, new object());

        Assert.Equal(77, ev.nX);
        Assert.Equal(88, ev.nY);
    }

    [Fact]
    public void ExistingEventBecomesVisible()
    {
        var d = new Dictionary<string, TVisibleMapEvent>();
        var obj = new object();

        VisibleItemLifecycleCore.UpdateVisibleEvent(d, "k", 0, 0, obj);
        d["k"].nVisibleFlag = 0;

        var ev = VisibleItemLifecycleCore.UpdateVisibleEvent(d, "k", 0, 0, obj);

        Assert.Equal(1, ev.nVisibleFlag);
    }

    [Fact]
    public void ExistingEventWrongObjectDoesNothing()
    {
        var d = new Dictionary<string, TVisibleMapEvent>();
        VisibleItemLifecycleCore.UpdateVisibleEvent(d, "k", 0, 0, new object());
        d["k"].nVisibleFlag = 0;

        var ev = VisibleItemLifecycleCore.UpdateVisibleEvent(d, "k", 0, 0, new object());

        Assert.Equal(0, ev.nVisibleFlag);
    }

    [Fact]
    public void EventKeyIsPointerString()
    {
        // 11349：IntToStr(NativeInt(MapEvent))
        Assert.Equal("12345", VisibleItemLifecycleCore.EventKey(12345));
    }

    // ===================== 帧末清理 =====================

    [Fact]
    public void MarkAllStaleSetsZero()
    {
        // 31705 / 31721
        var list = new List<TVisibleMapItem>
        {
            new() { nVisibleFlag = 1 }, new() { nVisibleFlag = 2 },
        };

        VisibleItemLifecycleCore.MarkAllStale(list,
            x => x.nVisibleFlag, (x, f) => x.nVisibleFlag = f);

        Assert.All(list, x => Assert.Equal(0, x.nVisibleFlag));
    }

    [Fact]
    public void SweepRemovesOnlyStaleEntries()
    {
        // 31947：仅 nVisibleFlag = 0 者删除
        var ordered = new List<KeyValuePair<nint, TVisibleBaseObject>>
        {
            new(1, new TVisibleBaseObject { nVisibleFlag = 0 }),
            new(2, new TVisibleBaseObject { nVisibleFlag = 1 }),
            new(3, new TVisibleBaseObject { nVisibleFlag = 2 }),
        };

        var toRemove = VisibleItemLifecycleCore.SweepStaleKeys(ordered);

        Assert.Single(toRemove);
        Assert.Equal(1, toRemove[0]);
    }

    [Fact]
    public void SweepCollectsInReverseOrder()
    {
        // 31944：原文 downto 0 倒序
        var ordered = new List<KeyValuePair<nint, TVisibleBaseObject>>
        {
            new(1, new TVisibleBaseObject { nVisibleFlag = 0 }),
            new(2, new TVisibleBaseObject { nVisibleFlag = 0 }),
            new(3, new TVisibleBaseObject { nVisibleFlag = 0 }),
        };

        var toRemove = VisibleItemLifecycleCore.SweepStaleKeys(ordered);

        Assert.Equal(new nint[] { 3, 2, 1 }, toRemove);
    }

    [Fact]
    public void SweepKeepsVisibleAndNewEntries()
    {
        // 1 = 已可见，2 = 新建未发送：两者都**不**被清除
        var ordered = new List<KeyValuePair<nint, TVisibleBaseObject>>
        {
            new(1, new TVisibleBaseObject { nVisibleFlag = 1 }),
            new(2, new TVisibleBaseObject { nVisibleFlag = 2 }),
        };

        Assert.Empty(VisibleItemLifecycleCore.SweepStaleKeys(ordered));
    }

    [Fact]
    public void ItemSweepSameRule()
    {
        var ordered = new List<KeyValuePair<nint, TVisibleMapItem>>
        {
            new(1, new TVisibleMapItem { nVisibleFlag = 0 }),
            new(2, new TVisibleMapItem { nVisibleFlag = 1 }),
        };

        var toRemove = VisibleItemLifecycleCore.SweepStaleItemKeys(ordered);

        Assert.Single(toRemove);
        Assert.Equal(1, toRemove[0]);
    }

    // ===================== 三态完整流程 =====================

    [Fact]
    public void FullFrameLifecycle()
    {
        var d = new Dictionary<nint, TVisibleMapItem>();
        var obj = new object();

        // 帧 1：新建（未发送）→ 2
        var it = VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(mapItem: obj, sendShow: false));
        Assert.Equal(2, it.nVisibleFlag);
        Assert.Empty(VisibleItemLifecycleCore.SweepStaleItemKeys(new List<KeyValuePair<nint, TVisibleMapItem>>(d)));

        // 帧 2：帧首置 0 → 再次确认可见 → 1
        d[1].nVisibleFlag = 0;
        VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(mapItem: obj, sendShow: false));
        Assert.Equal(1, d[1].nVisibleFlag);

        // 帧 3：帧首置 0，本次**未**再触达 → 帧末被清除
        d[1].nVisibleFlag = 0;
        var ordered = new List<KeyValuePair<nint, TVisibleMapItem>>(d);
        Assert.Single(VisibleItemLifecycleCore.SweepStaleItemKeys(ordered));
    }

    [Fact]
    public void HideThenSweepRemoves()
    {
        // HideItem 只标记；真正移除靠帧末 flag = 0
        var d = new Dictionary<nint, TVisibleMapItem>();
        var obj = new object();
        VisibleItemLifecycleCore.UpdateVisibleItem(d, 1, Args(mapItem: obj, sendShow: true));

        VisibleItemLifecycleCore.HideItem(d, 1, obj);
        Assert.True(d[1].boSendHideMsg);
        Assert.Single(d);

        d[1].nVisibleFlag = 0;
        var ordered = new List<KeyValuePair<nint, TVisibleMapItem>>(d);
        Assert.Single(VisibleItemLifecycleCore.SweepStaleItemKeys(ordered));
    }
}

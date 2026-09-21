// 源单元：Source/M2Engine/ItemEvent.pas（389 行）
// 对应实现：src/GXX.M2Server/Sweep9/DataLayer/ItemEventManager.cs（TItemManager）
//
// 覆盖 TItemManager 的 7 个过程函数（Create/Destroy/GetItemCount/AddItem/4 个 FindItem/Run）。
// 全部只依赖内存替身，**不连真实数据库/COM/地图**。

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Sweep9.DataLayer;
using Xunit;

using TGList = GXX.Core.Protocol.SDK.TGList;

namespace GXX.M2Server.Tests;

// ===========================================================================
// 一、TItemManager 构造/析构/计数/添加（ItemEvent.pas:166-209）
// ===========================================================================
public class Sweep9DataLayerItemManagerBasicsTests
{
    [Fact]
    public void Ctor_CreatesTwoEmptyListsAndZeroCursor()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager();

        Assert.NotNull(mgr.m_ItemList);
        Assert.NotNull(mgr.m_FreeItemList);
        Assert.Equal(0, mgr.m_ItemList.Count);
        Assert.Equal(0, mgr.m_FreeItemList.Count);
        Assert.Equal(0, mgr.m_nProcItemIDx);
    }

    [Fact]
    public void ItemCount_MirrorsItemListCount()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager();
        Assert.Equal(0, mgr.ItemCount);

        mgr.AddItem(new TItemObject());
        mgr.AddItem(new TItemObject());

        Assert.Equal(2, mgr.ItemCount);
    }

    [Fact]
    public void AddItem_AppendsInOrder()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager();
        var a = new TItemObject();
        var b = new TItemObject();

        mgr.AddItem(a);
        mgr.AddItem(b);

        Assert.Same(a, mgr.m_ItemList[0]);
        Assert.Same(b, mgr.m_ItemList[1]);
    }

    /// <summary>同一个实例可被重复加入（原文不做去重）。</summary>
    [Fact]
    public void AddItem_AllowsDuplicates_OriginalDoesNotDeduplicate()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager();
        var a = new TItemObject();

        mgr.AddItem(a);
        mgr.AddItem(a);

        Assert.Equal(2, mgr.ItemCount);
        Assert.Same(a, mgr.m_ItemList[0]);
        Assert.Same(a, mgr.m_ItemList[1]);
    }

    /// <summary>回收游标：`Destroy` 遍历两张表（含空表）且不抛异常。</summary>
    [Fact]
    public void Destroy_WithItemsAndFreeItems_DoesNotThrow()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager();
        mgr.AddItem(new TItemObject());
        mgr.m_FreeItemList.Add(new TItemObject());

        mgr.Destroy();
    }

    [Fact]
    public void Destroy_OnFreshManager_DoesNotThrow()
    {
        Sweep9DataLayerTestKit.Isolate();
        new TItemManager().Destroy();
    }
}

// ===========================================================================
// 二、四个 FindItem 重载（ItemEvent.pas:211-301）
// ===========================================================================
public class Sweep9DataLayerItemManagerFindItemTests
{
    private static (TItemManager Mgr, object Envir, TItemObject Item) Setup()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager();
        object envir = new FakeItemGameEnvir();
        var item = new TItemObject { m_PEnvir = envir, m_nMapX = 10, m_nMapY = 20 };
        mgr.AddItem(item);
        return (mgr, envir, item);
    }

    // ---- 重载 1：FindItem(Envir, ItemObject) ----

    [Fact]
    public void FindItem_ByObject_ReturnsMatch()
    {
        var (mgr, envir, item) = Setup();
        Assert.Same(item, mgr.FindItem(envir, item));
    }

    [Fact]
    public void FindItem_ByObject_ReturnsNullWhenEnvirDiffers()
    {
        var (mgr, _, item) = Setup();
        Assert.Null(mgr.FindItem(new FakeItemGameEnvir(), item));
    }

    [Fact]
    public void FindItem_ByObject_ReturnsNullWhenObjectDiffers()
    {
        var (mgr, envir, _) = Setup();
        Assert.Null(mgr.FindItem(envir, new TItemObject()));
    }

    /// <summary>幽灵物品**不**被找到（原文 :220 第一项判据）。</summary>
    [Fact]
    public void FindItem_ByObject_SkipsGhostItems()
    {
        var (mgr, envir, item) = Setup();
        item.m_boGhost = true;
        Assert.Null(mgr.FindItem(envir, item));
    }

    /// <summary>`m_PEnvir` 比较是**引用相等**（原文 `=` 是对象指针比较），不是 `Equals` 重载。</summary>
    [Fact]
    public void FindItem_ByObject_ComparesEnvirByReference()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager();
        var envir1 = new FakeItemGameEnvir();
        var envir2 = new FakeItemGameEnvir();
        var item = new TItemObject { m_PEnvir = envir1 };
        mgr.AddItem(item);

        Assert.Null(mgr.FindItem(envir2, item));
        Assert.Same(item, mgr.FindItem(envir1, item));
    }

    [Fact]
    public void FindItem_ByObject_ReturnsFirstMatchOnly()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager();
        var envir = new FakeItemGameEnvir();
        var a = new TItemObject { m_PEnvir = envir };
        var b = new TItemObject { m_PEnvir = envir };
        var c = new TItemObject { m_PEnvir = envir };
        mgr.AddItem(a);
        mgr.AddItem(b);
        mgr.AddItem(c);

        Assert.Same(b, mgr.FindItem(envir, b));
    }

    // ---- 重载 2：FindItem(Envir, nX, nY) ----

    [Fact]
    public void FindItem_ByCoords_ReturnsMatch()
    {
        var (mgr, envir, item) = Setup();
        Assert.Same(item, mgr.FindItem(envir, 10, 20));
    }

    [Theory]
    [InlineData(11, 20)]
    [InlineData(10, 21)]
    public void FindItem_ByCoords_RequiresExactMatch(int x, int y)
    {
        var (mgr, envir, _) = Setup();
        Assert.Null(mgr.FindItem(envir, x, y));
    }

    [Fact]
    public void FindItem_ByCoords_SkipsGhostAndWrongEnvir()
    {
        var (mgr, envir, item) = Setup();
        item.m_boGhost = true;
        Assert.Null(mgr.FindItem(envir, 10, 20));

        item.m_boGhost = false;
        Assert.Null(mgr.FindItem(new FakeItemGameEnvir(), 10, 20));
    }

    // ---- 重载 3：FindItem(Envir, nX, nY, ItemObject) ----

    [Fact]
    public void FindItem_ByCoordsAndObject_ReturnsTheParameterObject()
    {
        var (mgr, envir, item) = Setup();
        Assert.Same(item, mgr.FindItem(envir, 10, 20, item));
    }

    /// <summary>
    /// ★ 原文缺陷 ④（差异断言）：:266-267 判据里 `(AItemObject = ItemObject)` 比列表元素、
    /// 而坐标 `(ItemObject.m_nMapX = nX)` 读**形参** `ItemObject`。
    /// 若形参的坐标与列表元素不同（此处把形参搬到别处），**仍然会命中** ——
    /// 因为坐标是拿形参自己比的，而 :266 的相等判定已由 `ReferenceEquals` 满足。
    /// </summary>
    [Fact]
    public void FindItem_ByCoordsAndObject_ReadsCoordsFromParameterNotListElement_OriginalFlaw()
    {
        var (mgr, envir, item) = Setup();
        // 列表元素的坐标是 10/20；把形参坐标改成 99/98 后仍应命中（坐标读的是形参）
        item.m_nMapX = 99;
        item.m_nMapY = 98;

        // 形参与列表元素是同一对象 ⇒ 两者坐标自然一致
        Assert.Same(item, mgr.FindItem(envir, 99, 98, item));
        // 而拿旧坐标去查必然不中
        Assert.Null(mgr.FindItem(envir, 10, 20, item));
    }

    [Fact]
    public void FindItem_ByCoordsAndObject_ReturnsNullWhenObjectNotInList()
    {
        var (mgr, envir, _) = Setup();
        var outsider = new TItemObject { m_PEnvir = envir, m_nMapX = 10, m_nMapY = 20 };
        Assert.Null(mgr.FindItem(envir, 10, 20, outsider));
    }

    [Fact]
    public void FindItem_ByCoordsAndObject_SkipsGhostItems()
    {
        var (mgr, envir, item) = Setup();
        item.m_boGhost = true;
        Assert.Null(mgr.FindItem(envir, 10, 20, item));
    }

    // ---- 重载 4：FindItem(Envir, nX, nY, nRange, List) ----

    private static (TItemManager Mgr, object Envir) SetupGrid()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager();
        object envir = new FakeItemGameEnvir();
        // 以 (10,20) 为中心布点：距中心 0 / 1 / 2 / 5（切比雪夫距离 = max(|dx|,|dy|)）
        foreach ((int x, int y) in new[] { (10, 20), (11, 21), (12, 22), (15, 25) })
            mgr.AddItem(new TItemObject { m_PEnvir = envir, m_nMapX = x, m_nMapY = y });
        return (mgr, envir);
    }

    /// <summary>
    /// 范围是 **闭区间方形** `abs(dx) &lt;= nRange and abs(dy) &lt;= nRange`（原文 :289-290）。
    /// nRange=0 只命中中心；=1 命中中心 + 对角（切比雪夫）；=2 再多一个；=5 全部。
    /// </summary>
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(5, 4)]
    public void FindItem_ByRange_CountsClosedSquare(int range, int expected)
    {
        var (mgr, envir) = SetupGrid();
        Assert.Equal(expected, mgr.FindItem(envir, 10, 20, range, null));
    }

    /// <summary>`List &lt;&gt; nil` 时同步收集；收集到的条目数与返回值一致。</summary>
    [Fact]
    public void FindItem_ByRange_CollectsIntoListWhenProvided()
    {
        var (mgr, envir) = SetupGrid();
        var collected = new TGList();

        int n = mgr.FindItem(envir, 10, 20, 1, collected);

        Assert.Equal(2, n);
        Assert.Equal(2, collected.Count);
    }

    /// <summary>`List = nil` 时**仍然计数**（原文 :292 `Inc(nCount)` 在 if 之外）。</summary>
    [Fact]
    public void FindItem_ByRange_StillCountsWhenListIsNil()
    {
        var (mgr, envir) = SetupGrid();
        Assert.Equal(4, mgr.FindItem(envir, 10, 20, 100, null));
    }

    [Fact]
    public void FindItem_ByRange_SkipsGhostAndWrongEnvir()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager();
        var envir = new FakeItemGameEnvir();
        mgr.AddItem(new TItemObject { m_PEnvir = envir, m_nMapX = 10, m_nMapY = 20, m_boGhost = true });
        mgr.AddItem(new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_nMapX = 10, m_nMapY = 20 });

        Assert.Equal(0, mgr.FindItem(envir, 10, 20, 0, null));
    }

    /// <summary>负范围 ⇒ 恒 0（`abs(...) &lt;= -1` 永假）。原文不校验 nRange。</summary>
    [Fact]
    public void FindItem_ByRange_NegativeRangeYieldsZero()
    {
        var (mgr, envir) = SetupGrid();
        Assert.Equal(0, mgr.FindItem(envir, 10, 20, -1, null));
    }

    [Fact]
    public void FindItem_OnEmptyManager_AllOverloadsReturnEmpty()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager();
        var envir = new FakeItemGameEnvir();
        var item = new TItemObject { m_PEnvir = envir };

        Assert.Null(mgr.FindItem(envir, item));
        Assert.Null(mgr.FindItem(envir, 0, 0));
        Assert.Null(mgr.FindItem(envir, 0, 0, item));
        Assert.Equal(0, mgr.FindItem(envir, 0, 0, 0, null));
    }
}

// ===========================================================================
// 三、TItemManager.Run（ItemEvent.pas:303-386）
// ===========================================================================
public class Sweep9DataLayerItemManagerRunTests
{
    /// <summary>
    /// 原文 :324：只在"非幽灵"**且**"距 m_dwRunTick 超 250 ms"时刷新 tick 并调 `Item.Run()`。
    /// 边界（== 250）**不**触发。
    /// <para>取证方式：把"到期阈值"设为 1 ms ⇒ 只要 `Item.Run()` 被执行，就会在 :102 判据成立后
    /// 把物品置为幽灵；未被执行则保持非幽灵。</para>
    /// </summary>
    [Theory]
    [InlineData(250u, false)]
    [InlineData(251u, true)]
    public void Run_CallsItemRunOnlyWhenOver250ms(uint elapsed, bool expectRun)
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => 1u;    // 只要跑起来就一定到期
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;

        var mgr = new TItemManager();
        var item = new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_dwAddTime = 0 };
        item.m_dwRunTick = 0;
        mgr.AddItem(item);

        clock.Now = elapsed;
        mgr.Run();

        Assert.Equal(expectRun, item.m_boGhost);
    }

    /// <summary>非幽灵且超时 ⇒ `Item.Run()` 真被执行（用到期置幽灵取证）。</summary>
    [Fact]
    public void Run_Over250ms_ActuallyInvokesItemRun()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => 100u;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;

        var mgr = new TItemManager();
        var envir = new FakeItemGameEnvir();
        var item = new TItemObject { m_PEnvir = envir, m_dwAddTime = 0, m_dwRunTick = 0 };
        mgr.AddItem(item);

        clock.Now = 500u;
        mgr.Run();

        Assert.True(item.m_boGhost);   // Item.Run 把到期物品标成幽灵
    }

    /// <summary>幽灵物品进 `m_FreeItemList` 并从 `m_ItemList` 删除（原文 :330-335）。</summary>
    [Fact]
    public void Run_GhostItemMovesToFreeListAndIsRemovedFromItemList()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => 100u;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;

        var mgr = new TItemManager();
        var item = new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_dwAddTime = 0, m_dwRunTick = 0 };
        mgr.AddItem(item);

        clock.Now = 500u;
        mgr.Run();

        Assert.Equal(0, mgr.m_ItemList.Count);
        Assert.Equal(1, mgr.m_FreeItemList.Count);
        Assert.Same(item, mgr.m_FreeItemList[0]);
    }

    /// <summary>
    /// 原文 :334 `Continue`（**不 `Inc(nIdx)`**）—— 删除后同位置的下一元素必须在本帧内被处理。
    /// 三个连续到期物品应**一帧内全部**进空闲表。
    /// </summary>
    [Fact]
    public void Run_ContinueAfterDelete_ProcessesNextElementAtSameIndexInSameFrame()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => 100u;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;

        var mgr = new TItemManager();
        for (int i = 0; i < 3; i++)
            mgr.AddItem(new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_dwAddTime = 0, m_dwRunTick = 0 });

        clock.Now = 500u;
        mgr.Run();

        Assert.Equal(0, mgr.m_ItemList.Count);
        Assert.Equal(3, mgr.m_FreeItemList.Count);
    }

    /// <summary>
    /// 原文 :338：本帧耗时 > 5 ms ⇒ `boCheckTimeLimit := True`、保存游标并 `Break`；
    /// 循环后 :348 的 `if not boCheckTimeLimit` **不成立** ⇒ 游标**被保留**。
    /// </summary>
    [Fact]
    public void Run_TimeLimitHit_SavesCursorForNextFrame()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => uint.MaxValue;

        var mgr = new TItemManager();
        // 三个"不需要跑"的物品（m_dwRunTick 保持为当前 tick ⇒ 差值 0，不触发 Item.Run）
        for (int i = 0; i < 3; i++)
            mgr.AddItem(new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_dwRunTick = 0 });

        // 让每次 TickCount 读都自增：dwCheckTime 读一次，之后每轮再读两次 ⇒ 很快超 5
        clock.Now = 0u;
        clock.AutoIncrement = true;

        mgr.Run();

        Assert.True(mgr.m_nProcItemIDx > 0, "被时间限制打断时应保留续跑游标");
    }

    /// <summary>
    /// 对照组：跑完全部元素（未被时间限制打断）⇒ 游标**复位为 0**（原文 :348-349）。
    /// </summary>
    [Fact]
    public void Run_CompletesWithinTimeLimit_ResetsCursorToZero()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => uint.MaxValue;

        var mgr = new TItemManager { m_nProcItemIDx = 0 };
        mgr.AddItem(new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_dwRunTick = 0 });

        clock.Now = 1000u;      // 不自增 ⇒ 恒差 0，永不超 5 ms
        mgr.Run();

        Assert.Equal(0, mgr.m_nProcItemIDx);
    }

    /// <summary>
    /// 空表：`m_ItemList.Count &lt;= nIdx`（0 &lt;= 0）⇒ 立即 `Break`，随后 :348 复位游标。
    /// <para><b>原文缺陷（差异断言）</b>：`Break` 发生在 :320-321，**在游标赋值之前**，
    /// 所以空表 + 非零游标 ⇒ 游标会被 :349 **复位为 0**。这里同时锁住"空表也复位"这个行为。</para>
    /// </summary>
    [Fact]
    public void Run_EmptyList_BreaksImmediatelyAndResetsCursor()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager { m_nProcItemIDx = 7 };

        mgr.Run();

        Assert.Equal(0, mgr.m_nProcItemIDx);
    }

    /// <summary>
    /// 对照组（差异断言的另一半）：游标落在表尾之后 ⇒ 同样立即 `Break` 并复位。
    /// </summary>
    [Fact]
    public void Run_CursorPastEnd_BreaksImmediatelyAndResetsCursor()
    {
        Sweep9DataLayerTestKit.Isolate();
        var mgr = new TItemManager { m_nProcItemIDx = 5 };
        mgr.AddItem(new TItemObject());
        mgr.AddItem(new TItemObject());

        mgr.Run();

        Assert.Equal(0, mgr.m_nProcItemIDx);
    }

    /// <summary>
    /// 续跑：`m_nProcItemIDx` 非 0 时从该位置开始（前面的元素本帧不被处理）。
    /// </summary>
    [Fact]
    public void Run_ResumesFromSavedCursor()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => 100u;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;

        var mgr = new TItemManager();
        var first = new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_dwAddTime = 0, m_dwRunTick = 0 };
        var second = new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_dwAddTime = 0, m_dwRunTick = 0 };
        mgr.AddItem(first);
        mgr.AddItem(second);
        mgr.m_nProcItemIDx = 1;    // 上一帧停在 index 1

        clock.Now = 500u;
        mgr.Run();

        Assert.False(first.m_boGhost);    // 0 号本帧没被跑到
        Assert.True(second.m_boGhost);    // 1 号被跑到并到期
        Assert.Equal(1, mgr.m_ItemList.Count);
        Assert.Same(first, mgr.m_ItemList[0]);
    }

    /// <summary>
    /// 原文 :373-382：空闲表里"幽灵时刻超 5 分钟"的条目被删除（严格大于）。
    /// </summary>
    [Theory]
    [InlineData(300000u, false)]   // == 5*60*1000 ⇒ 保留
    [InlineData(300001u, true)]    // +1 ⇒ 删除
    public void Run_FreeListCleanup_UsesStrictGreaterThan5Minutes(uint ghostAge, bool expectRemoved)
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => uint.MaxValue;

        var mgr = new TItemManager();
        var ghost = new TItemObject { m_dwGhostTick = 0 };
        mgr.m_FreeItemList.Add(ghost);

        clock.Now = ghostAge;
        mgr.Run();

        Assert.Equal(expectRemoved ? 0 : 1, mgr.m_FreeItemList.Count);
    }

    /// <summary>
    /// 原文缺陷（差异断言）：空闲表清理是**从尾到头**（:373 `downto 0`），
    /// 所以同一次 `Run` 里**可以**删掉多个（:380 的 `break` 被注释掉了）。
    /// </summary>
    [Fact]
    public void Run_FreeListCleanup_RemovesAllExpiredInOnePass_BreakIsCommentedOut_OriginalFlaw()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => uint.MaxValue;

        var mgr = new TItemManager();
        for (int i = 0; i < 3; i++)
            mgr.m_FreeItemList.Add(new TItemObject { m_dwGhostTick = 0 });

        clock.Now = 400000u;    // 全部超 5 分钟
        mgr.Run();

        Assert.Equal(0, mgr.m_FreeItemList.Count);
    }

    /// <summary>
    /// 原文 :350-351 的 `except MainOutMessage(sExceptionMsg)`：异常被吞掉、写一条日志，
    /// 且 **`m_nProcItemIDx` 不被重置**（与正常路径不对称 —— 原文缺陷，差异断言）。
    /// <para>注入点：`Item.Run()` 里 `m_PEnvir = nil` 会抛 `NullReferenceException`（见
    /// ItemEvent 的 :109 无保护硬转换）。</para>
    /// </summary>
    [Fact]
    public void Run_SwallowsExceptionLogsAndKeepsCursor_OriginalFlaw()
    {
        var (clock, log) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;

        var mgr = new TItemManager { m_nProcItemIDx = 3 };
        var bad = new TItemObject { m_PEnvir = null, m_dwAddTime = 0, m_dwRunTick = 0 };
        // 让游标正好落在 bad 上：index 0/1/2 是三个"不需要跑"的物品（m_dwRunTick = 当前 tick）
        mgr.AddItem(new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_dwRunTick = 1000u });
        mgr.AddItem(new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_dwRunTick = 1000u });
        mgr.AddItem(new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_dwRunTick = 1000u });
        mgr.AddItem(bad);

        clock.Now = 1000u;   // 差 1000 > 250 ⇒ 会调 Item.Run() ⇒ 因 m_PEnvir = nil 抛
        mgr.Run();

        Assert.Contains(TItemManager.ExceptionMsg, log);
        Assert.Equal(3, mgr.m_nProcItemIDx);   // 异常路径**不**复位游标
    }

    /// <summary>
    /// 异常消息串必须与原文 `resourcestring sExceptionMsg = '[Exception] TItemManager.Run'` 逐字一致。
    /// </summary>
    [Fact]
    public void Run_ExceptionMessageMatchesOriginalResourcestring()
    {
        Assert.Equal("[Exception] TItemManager.Run", TItemManager.ExceptionMsg);
    }
}

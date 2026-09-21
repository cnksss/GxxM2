using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// `DropItemsMgr.pas`（570 行）的边界测试：**二分查找的顺序不变式**、**对象池容量与淘汰顺序**、
/// **AddDropItem/DelDropItem 的分支差异**、**RefreshDrawList 的 3 个绘制位**、
/// 以及**原文 `string[60]` 短字符串的静默字节截断**（原文缺陷）。
/// </summary>
public sealed class ScrnDrawDropItemsTests : ScrnDrawTestBase
{
    private uint _now;

    public ScrnDrawDropItemsTests()
    {
        _now = 5000;
        DrawScrnEnv.MyGetTickCountFn = () => _now;
    }

    private static TDropItemEffect Effect(int index, ushort startIndex, ushort imageCount, short fileIndex = 0)
        => new TDropItemEffect
        {
            ItemEffectIndex = (ushort)index,
            FileIndex = fileIndex,
            StartIndex = startIndex,
            Time = 100,
            ImageCount = (byte)imageCount,
            DrawCenter = 0,
            NoBlend = 0,
            BelowItem = 0,
            OffsetX = 0,
            OffsetY = 0,
        };

    // =========================================================================================
    // Create / Free（246-295）
    // =========================================================================================

    /// <summary>Create（246-265）：三个 TList 就位，且 `FNoUseItemList` **预置 512 个空项**。</summary>
    [Fact]
    public void TDropItemsMgr_Create_Preallocates512PoolItems()
    {
        var m = new TDropItemsMgr();
        Assert.Empty(m.FSortIDItemList);
        Assert.Empty(m.FPointList);
        Assert.Equal(512, m.FNoUseItemList.Count);
        Assert.Equal(0, m.Count);
    }

    /// <summary>ClearAndFree（277-295）：点表全部释放、ID 表与池都清空。</summary>
    [Fact]
    public void TDropItemsMgr_ClearAndFree_EmptiesEverything()
    {
        var m = new TDropItemsMgr();
        m.AddDropItem(1, 1, 1, "A", 0, out _);
        m.AddDropItem(2, 2, 2, "B", 0, out _);

        m.ClearAndFree();

        Assert.Empty(m.FPointList);
        Assert.Empty(m.FSortIDItemList);
        Assert.Empty(m.FNoUseItemList);
        Assert.Equal(0, m.Count);
    }

    // =========================================================================================
    // 点表顺序（PointCompare 386-389 / PointSearch 391-414）
    // =========================================================================================

    /// <summary>
    /// 点表按 `MakeLong(X, Y)`（X 低位、Y 高位）**升序**插入；乱序 Add 后仍有序。
    /// </summary>
    [Fact]
    public void PointSearch_KeepsMakeLongAscendingOrder()
    {
        var m = new TDropItemsMgr();
        m.AddDropItem(1, 5, 10, "A", 0, out _);   // MakeLong = 0x000A0005
        m.AddDropItem(2, 3, 20, "B", 0, out _);   // 0x00140003 更大 → 插到后面
        m.AddDropItem(3, 5, 3, "C", 0, out _);    // 0x00030005 最小 → 插到最前

        Assert.Equal(3, m.Count);
        Assert.Equal(5, (int)m.Items(0).X);
        Assert.Equal(3, (int)m.Items(0).Y);
        Assert.Equal(5, (int)m.Items(1).X);
        Assert.Equal(10, (int)m.Items(1).Y);
        Assert.Equal(3, (int)m.Items(2).X);
        Assert.Equal(20, (int)m.Items(2).Y);
    }

    /// <summary>同点复用同一条 TPointDropItemList（450-473 的 PointSearch 命中分支）。</summary>
    [Fact]
    public void AddDropItem_SamePointReusesPointList()
    {
        var m = new TDropItemsMgr();
        m.AddDropItem(1, 4, 4, "A", 0, out _);
        m.AddDropItem(2, 4, 4, "B", 0, out _);

        Assert.Equal(1, m.Count);
        var list = m.Items(0);
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list.DrawCount);            // 未 RefreshDrawList 前绘制名单为空
    }

    /// <summary>`GetItemListIndexByY` 用 `PointSearch(65535, Y)` 取插入位（353-356）。</summary>
    [Fact]
    public void GetItemListIndexByY_UsesX65535AsSearchKey()
    {
        var m = new TDropItemsMgr();
        m.AddDropItem(1, 5, 3, "A", 0, out _);    // 0x00030005
        m.AddDropItem(2, 5, 10, "B", 0, out _);   // 0x000A0005
        m.AddDropItem(3, 3, 20, "C", 0, out _);   // 0x00140003

        // key = MakeLong(65535, 10) = 0x000AFFFF → 小于 0x00140003 的两项 + 自身 → 索引 2
        Assert.Equal(2, m.GetItemListIndexByY(10));
        Assert.Equal(0, m.GetItemListIndexByY(1));
        Assert.Equal(3, m.GetItemListIndexByY(30));
    }

    /// <summary>GetItems 越界返回 null（325-331）；GetItemListByPoint 未命中返回 null（343-351）。</summary>
    [Fact]
    public void GetItems_OutOfRangeReturnsNull()
    {
        var m = new TDropItemsMgr();
        Assert.Null(m.GetItems(-1));
        Assert.Null(m.GetItems(0));
        Assert.Null(m.GetItemListByPoint(9, 9));

        m.AddDropItem(1, 1, 1, "A", 0, out _);
        Assert.NotNull(m.GetItems(0));
        Assert.NotNull(m.GetItemListByPoint(1, 1));
    }

    /// <summary>PointCompare 是 **virtual**（386-389），子类可换排序键（此处验证可覆写且被 PointSearch 使用）。</summary>
    [Fact]
    public void PointCompare_IsVirtual_AndSubclassOverrideIsUsed()
    {
        var m = new ReversePointMgr();
        m.AddDropItem(1, 5, 10, "A", 0, out _);
        m.AddDropItem(2, 3, 20, "B", 0, out _);
        m.AddDropItem(3, 5, 3, "C", 0, out _);

        Assert.True(m.CompareCalls > 0);
        // 反转比较器 ⇒ 降序
        Assert.Equal(20, (int)m.Items(0).Y);
        Assert.Equal(3, (int)m.Items(0).X);
    }

    private sealed class ReversePointMgr : TDropItemsMgr
    {
        public int CompareCalls;

        protected override int PointCompare(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            CompareCalls++;
            return base.PointCompare(x2, y2, x1, y1);
        }
    }

    /// <summary>IDCompare / IDSearch 也是 virtual（358-384），子类可换键。</summary>
    [Fact]
    public void IdCompare_IsVirtual()
    {
        var m = new CountingIdMgr();
        m.AddDropItem(10, 1, 1, "A", 0, out _);
        m.AddDropItem(20, 2, 2, "B", 0, out _);

        Assert.NotNull(m.GetItemByID(20));
        Assert.True(m.CompareCalls > 0);
    }

    private sealed class CountingIdMgr : TDropItemsMgr
    {
        public int CompareCalls;

        protected override int IDCompare(int id1, int id2)
        {
            CompareCalls++;
            return base.IDCompare(id1, id2);
        }
    }

    // =========================================================================================
    // AddDropItem（416-501）
    // =========================================================================================

    /// <summary>新 ID：IsNew=True、Visible=True、从对象池取（池 -1）、Makecode_T 恒等（ENCRYPTION=0）。</summary>
    [Fact]
    public void AddDropItem_NewItem_TakesFromPoolAndSetsVisible()
    {
        var m = new TDropItemsMgr();
        var item = m.AddDropItem(7, 12, 34, "DB", 0, out bool isNew);

        Assert.True(isNew);
        Assert.Equal(7, item.ID);
        Assert.Equal((ushort)12, item.X);
        Assert.Equal((ushort)34, item.Y);
        Assert.Equal("DB", item.DBName);
        Assert.True(item.Visible);
        Assert.Equal(511, m.FNoUseItemList.Count);
        Assert.Equal(1, m.FSortIDItemList.Count);
    }

    /// <summary>重复 ID：IsNew=False、只改 DBName、**不新增点表项**（424-427）。</summary>
    [Fact]
    public void AddDropItem_DuplicateId_UpdatesDbNameOnly()
    {
        var m = new TDropItemsMgr();
        m.AddDropItem(7, 1, 1, "Old", 0, out _);
        var again = m.AddDropItem(7, 1, 1, "New", 0, out bool isNew);

        Assert.False(isNew);
        Assert.Equal("New", again.DBName);
        Assert.Equal(1, m.FSortIDItemList.Count);
        Assert.Equal(1, m.Count);
        Assert.Equal(1, m.Items(0).Count);          // 只有一条
    }

    /// <summary>对象池耗尽后**新建**（433-435），不抛异常。</summary>
    [Fact]
    public void AddDropItem_PoolExhausted_AllocatesNew()
    {
        var m = new TDropItemsMgr();
        for (int i = 0; i < 512; i++)
            m.AddDropItem(i, (ushort)i, 1, "A", 0, out _);
        Assert.Empty(m.FNoUseItemList);

        var extra = m.AddDropItem(9999, 5, 5, "B", 0, out bool isNew);
        Assert.True(isNew);
        Assert.NotNull(extra);
        Assert.Equal(513, m.FSortIDItemList.Count);
    }

    /// <summary>
    /// 命中特效（454-473）：拷贝 `TDropItemEffect`、`ItemEffectFrame = StartIndex`、
    /// `ItemEffectTick/FlashTime/FlashStepTime = MyGetTickCount`、`ShowFlash = False`、`FlashStep = 0`；
    /// 且点表**非空**时头部插入（`Insert(0, ...)`）。
    /// </summary>
    [Fact]
    public void AddDropItem_WithEffect_SetsFrameAndHeadInserts()
    {
        DropItemsMgrEnv.g_DropItemEffectList.Add(Effect(1, 5, 3));
        var m = new TDropItemsMgr();

        var a = m.AddDropItem(1, 2, 2, "A", 1, out _);
        Assert.Equal((short)0, a.ItemEffect.FileIndex);
        Assert.Equal(5, a.ItemEffectFrame);
        Assert.Equal(_now, a.ItemEffectTick);
        Assert.Equal(_now, a.FlashTime);
        Assert.Equal(_now, a.FlashStepTime);
        Assert.False(a.ShowFlash);
        Assert.Equal(0, a.FlashStep);

        var b = m.AddDropItem(2, 2, 2, "B", 1, out _);
        var list = m.Items(0);
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list.Items(0).ID);          // 头部插入
        Assert.Equal(1, list.Items(1).ID);
        Assert.Same(b, list.Items(0));
    }

    /// <summary>无特效（EffectIndex = 0 或列表未命中）→ `ItemEffect.FileIndex := -1`，尾插（466-498）。</summary>
    [Fact]
    public void AddDropItem_WithoutEffect_SetsFileIndexMinusOneAndAppends()
    {
        var m = new TDropItemsMgr();
        var a = m.AddDropItem(1, 2, 2, "A", 0, out _);
        Assert.Equal((short)-1, a.ItemEffect.FileIndex);

        var b = m.AddDropItem(2, 2, 2, "B", 999, out _);   // EffectIndex>0 但未注册
        Assert.Equal((short)-1, b.ItemEffect.FileIndex);

        var list = m.Items(0);
        Assert.Equal(1, list.Items(0).ID);          // 尾插
        Assert.Equal(2, list.Items(1).ID);
    }

    /// <summary>点表为空时用特效也走尾插（470 的 `PointDropItemList.FList.Count &gt; 0`）。</summary>
    [Fact]
    public void AddDropItem_WithEffect_FirstItemAppends()
    {
        DropItemsMgrEnv.g_DropItemEffectList.Add(Effect(1, 0, 2));
        var m = new TDropItemsMgr();
        m.AddDropItem(1, 2, 2, "A", 1, out _);
        Assert.Equal(1, m.Items(0).Items(0).ID);
    }

    /// <summary>`PlugInEnabled` 时 `ShowItem := g_ConfigDlg.GetShowItem(DBName)`（445-446）。</summary>
    [Fact]
    public void AddDropItem_PlugInEnabled_BindsShowItem()
    {
        var show = new TShowItem();
        DropItemsMgrEnv.PlugInEnabled = true;
        DrawScrnEnv.GetShowItemFn = name => show;

        var m = new TDropItemsMgr();
        var item = m.AddDropItem(1, 1, 1, "DB", 0, out _);
        Assert.Same(show, item.ShowItem);
    }

    // =========================================================================================
    // DelDropItem（503-547）
    // =========================================================================================

    /// <summary>
    /// 删除：ID 表移除、`Visible := False`、X/Y 经 `Cutecode_T` 回传、点表移除该项、
    /// 点表空了就把 TPointDropItemList 一并删掉、最后把对象**还回池**。
    /// </summary>
    [Fact]
    public void DelDropItem_RemovesFromBothLists_AndFreesEmptyPoint()
    {
        var m = new TDropItemsMgr();
        m.AddDropItem(1, 4, 5, "A", 0, out _);
        Assert.Equal(1, m.Count);
        Assert.Equal(511, m.FNoUseItemList.Count);

        var item = m.DelDropItem(1, out ushort x, out ushort y);

        Assert.NotNull(item);
        Assert.Equal((ushort)4, x);
        Assert.Equal((ushort)5, y);
        Assert.False(item.Visible);
        Assert.Empty(m.FSortIDItemList);
        Assert.Equal(0, m.Count);                     // 空点表被移除
        Assert.Equal(512, m.FNoUseItemList.Count);    // 还回池
    }

    /// <summary>点表非空时不删点表项（530 只在 `Count = 0` 时删）。</summary>
    [Fact]
    public void DelDropItem_KeepsPointListWhenNotEmpty()
    {
        var m = new TDropItemsMgr();
        m.AddDropItem(1, 4, 5, "A", 0, out _);
        m.AddDropItem(2, 4, 5, "B", 0, out _);

        m.DelDropItem(1, out _, out _);
        Assert.Equal(1, m.Count);
        Assert.Equal(1, m.Items(0).Count);
        Assert.Equal(2, m.Items(0).Items(0).ID);
    }

    /// <summary>ID 不存在 → 返回 null，且**不碰池**（509 / 546 的 `if IDSearch(...)`）。</summary>
    [Fact]
    public void DelDropItem_UnknownId_ReturnsNull()
    {
        var m = new TDropItemsMgr();
        Assert.Null(m.DelDropItem(42, out ushort x, out ushort y));
        Assert.Equal((ushort)0, x);
        Assert.Equal((ushort)0, y);
        Assert.Equal(512, m.FNoUseItemList.Count);
    }

    // =========================================================================================
    // Clear（297-318）
    // =========================================================================================

    /// <summary>
    /// Clear：ID 表项**尽量**还回池（`Count &gt;= 512` 时丢弃），点表整体释放并清空。
    /// </summary>
    [Fact]
    public void Clear_ReturnsItemsToPoolUpTo512Cap()
    {
        var m = new TDropItemsMgr();
        // 513 条 → 池已被抽干
        for (int i = 0; i < 513; i++)
            m.AddDropItem(i, (ushort)i, 1, "A", 0, out _);
        Assert.Empty(m.FNoUseItemList);

        m.Clear();

        Assert.Empty(m.FSortIDItemList);
        Assert.Empty(m.FPointList);
        Assert.Equal(512, m.FNoUseItemList.Count);   // 超出的 1 个被丢弃
    }

    /// <summary>Clear 不新增池项上限（`Count &gt;= 512` 即 Dispose）。</summary>
    [Fact]
    public void Clear_PoolCapIsFiveTwelve()
    {
        var m = new TDropItemsMgr();
        for (int i = 0; i < 3; i++)
            m.AddDropItem(i, (ushort)i, 1, "A", 0, out _);
        Assert.Equal(509, m.FNoUseItemList.Count);

        m.Clear();
        Assert.Equal(512, m.FNoUseItemList.Count);
    }

    // =========================================================================================
    // TPointDropItemList.RefreshDrawList（192-242）
    // =========================================================================================

    /// <summary>前三项「显示名字」的进 DrawList；第 4 项及之后被清掉 NameImageInfo。</summary>
    [Fact]
    public void RefreshDrawList_ThreeNameSlots_AndClearsRest()
    {
        var m = new TDropItemsMgr();
        var items = new List<TDropItem>();
        for (int i = 0; i < 5; i++)
        {
            var it = m.AddDropItem(i, 1, 1, "A", 0, out _);
            it.ShowItem = new TShowItem { boShowName = 1 };
            it.NameImageInfo = new TImageInfo { Width = 9, Height = 9, ImageIndexs = new List<int> { 1 } };
            items.Add(it);
        }

        m.RefreshDrawList();

        var list = m.Items(0);
        Assert.Equal(3, list.DrawCount);
        Assert.Equal(0, list.DrawItems(0).ID);
        Assert.Equal(1, list.DrawItems(1).ID);
        Assert.Equal(2, list.DrawItems(2).ID);

        // 第 4/5 项的 NameImageInfo 被清空
        Assert.Null(items[3].NameImageInfo.ImageIndexs);
        Assert.Equal(0, items[3].NameImageInfo.Width);
        Assert.Equal(0, items[3].NameImageInfo.Height);
        Assert.Null(items[4].NameImageInfo.ImageIndexs);
    }

    /// <summary>
    /// ★ 原文易误读点（照抄 + 断言锁死）：第二轮补足只判 `Visible`，**不判 ShowItem**
    /// （236-237）——所以「名字不可显示」的可见项照样会占绘制位。
    /// </summary>
    [Fact]
    public void RefreshDrawList_SecondPassIgnoresShowItemAndOnlyChecksVisible()
    {
        var m = new TDropItemsMgr();
        var a = m.AddDropItem(1, 1, 1, "A", 0, out _);
        var b = m.AddDropItem(2, 1, 1, "B", 0, out _);
        a.ShowItem = new TShowItem { boShowName = 1 };
        a.Visible = false;                       // 不可见 → 两轮都不进
        b.ShowItem = new TShowItem { boShowName = 0 };

        m.RefreshDrawList();
        Assert.Equal(1, m.Items(0).DrawCount);
        Assert.Equal(2, m.Items(0).DrawItems(0).ID);
    }

    /// <summary>
    /// 不足 3 个时用**可见项**补足（225-241），且同一个对象不会被重复加入
    /// （229-234 的 `FDrawList.Items[II] = DropItem` 引用比较）。
    /// </summary>
    [Fact]
    public void RefreshDrawList_FillsUpWithVisibleItemsWithoutDuplicates()
    {
        var m = new TDropItemsMgr();
        var a = m.AddDropItem(1, 1, 1, "A", 0, out _);   // 有名
        var b = m.AddDropItem(2, 1, 1, "B", 0, out _);   // 无名可见
        var c = m.AddDropItem(3, 1, 1, "C", 0, out _);   // 无名可见
        a.ShowItem = new TShowItem { boShowName = 1 };
        b.ShowItem = new TShowItem { boShowName = 0 };
        c.ShowItem = new TShowItem { boShowName = 0 };

        m.RefreshDrawList();

        var list = m.Items(0);
        Assert.Equal(3, list.DrawCount);
        Assert.Equal(1, list.DrawItems(0).ID);   // 有名项在最前
        Assert.Equal(2, list.DrawItems(1).ID);
        Assert.Equal(3, list.DrawItems(2).ID);
    }

    /// <summary>
    /// `ShowItem = nil`（插件关闭）时第一轮全落空，但第二轮仍会按 `Visible` 补足 —— 上限 3。
    /// </summary>
    [Fact]
    public void RefreshDrawList_NullShowItem_StillFillsFromVisible()
    {
        var m = new TDropItemsMgr();
        m.AddDropItem(1, 1, 1, "A", 0, out _);
        m.AddDropItem(2, 1, 1, "B", 0, out _);
        m.RefreshDrawList();
        Assert.Equal(2, m.Items(0).DrawCount);
    }

    /// <summary>`ResetShowItem := True` + 插件开启时**重新查 `g_FileItemDB`**（203-205）覆盖 ShowItem。</summary>
    [Fact]
    public void RefreshDrawList_ResetShowItem_RefetchesFromFileItemDb()
    {
        DropItemsMgrEnv.PlugInEnabled = true;

        var m = new TDropItemsMgr();
        var it = m.AddDropItem(1, 1, 1, "NotInFileItemDb", 0, out _);
        it.ShowItem = new TShowItem { boShowName = 1 };   // 先塞一个

        m.RefreshDrawList(true);

        // 条目不在 g_FileItemDB 里 → 被覆盖成 null（证明走的是 g_FileItemDB 而不是 g_ConfigDlg）
        Assert.Null(it.ShowItem);
        // 但第二轮仍按 Visible 补足 → 占 1 个绘制位
        Assert.Equal(1, m.Items(0).DrawCount);
        Assert.Same(it, m.Items(0).DrawItems(0));
    }

    /// <summary>全局 RefreshDrawList 遍历全部点表（559-568）。</summary>
    [Fact]
    public void RefreshDrawList_VisitsAllPointLists()
    {
        var m = new TDropItemsMgr();
        m.AddDropItem(1, 1, 1, "A", 0, out _);
        m.AddDropItem(2, 2, 2, "B", 0, out _);
        m.RefreshDrawList();
        Assert.Equal(2, m.Count);
        Assert.Equal(1, m.Items(0).DrawCount);
        Assert.Equal(1, m.Items(1).DrawCount);
    }

    // =========================================================================================
    // 原文缺陷：string[60] 短字符串的静默字节截断
    // =========================================================================================

    /// <summary>
    /// ★ 原文缺陷（照抄 + 差异断言 D-P10-D04）：`TDropItem.Name` / `DBName` 是 **`string[60]`**，
    /// 赋值时按 **GBK 字节**截断到 60 且**不报错**（61 个 ASCII 字符 → 只剩 60；
    /// 31 个汉字 = 62 字节 → 只剩 30 个汉字）。
    /// </summary>
    [Fact]
    public void DropItem_ShortString60_TruncatesByGbkBytes()
    {
        var item = new TDropItem();

        item.DBName = new string('A', 61);
        Assert.Equal(60, item.DBName.Length);

        item.DBName = new string('A', 60);
        Assert.Equal(60, item.DBName.Length);          // 恰好 60 不截断

        item.DBName = new string('汉', 31);            // 31 × 2 = 62 字节
        Assert.Equal(30, item.DBName.Length);          // 静默丢 1 个汉字

        item.Name = new string('汉', 30);              // 30 × 2 = 60 字节
        Assert.Equal(30, item.Name.Length);            // 边界不截断

        item.Name = new string('汉', 31);
        Assert.Equal(30, item.Name.Length);

        // 该截断对 AddDropItem 的入参同样生效
        var m = new TDropItemsMgr();
        var it = m.AddDropItem(1, 1, 1, new string('汉', 31), 0, out _);
        Assert.Equal(30, it.DBName.Length);
    }

    /// <summary>
    /// ★ 托管侧差异（登记 D-P10-12）：`TPointDropItemList.GetDrawItems` 在原文里**不判越界**
    /// （187-190 直接取 `FDrawList.Items[Index]`，越界是未定义行为），托管 `List&lt;T&gt;` 会抛
    /// `ArgumentOutOfRangeException` —— 差异已用断言锁死。
    /// </summary>
    [Fact]
    public void GetDrawItems_OutOfRange_ThrowsInManaged()
    {
        var m = new TDropItemsMgr();
        m.AddDropItem(1, 1, 1, "A", 0, out _);
        Assert.Throws<ArgumentOutOfRangeException>(() => m.Items(0).GetDrawItems(0));
    }

    /// <summary>`GetItems`（点表内）越界返回 null（174-180）—— 与 GetDrawItems 不同。</summary>
    [Fact]
    public void PointList_GetItems_OutOfRangeReturnsNull()
    {
        var m = new TDropItemsMgr();
        m.AddDropItem(1, 1, 1, "A", 0, out _);
        Assert.Null(m.Items(0).GetItems(-1));
        Assert.Null(m.Items(0).GetItems(1));
        Assert.NotNull(m.Items(0).GetItems(0));
    }

    /// <summary>TValues：Makecode_T / Cutecode_T 在 `ENCRYPOINT = 0` 下是恒等（SDK.pas:27）。</summary>
    [Fact]
    public void MakecodeAndCutecode_AreIdentityWhenEncrypointIsZero()
    {
        Assert.Equal(12345, DropItemsMgrEnv.Makecode_T(12345));
        Assert.Equal(12345, DropItemsMgrEnv.Cutecode_T(12345));
        Assert.Equal(0, DropItemsMgrEnv.Makecode_T(0));
        Assert.Equal(-1, DropItemsMgrEnv.Cutecode_T(-1));
    }

    /// <summary>Lock / UnLock 走同一临界区且可重入（THintWindows 的 Draw 也会重入）（559-568 / 549-557）。</summary>
    [Fact]
    public void LockUnLock_AreReentrant()
    {
        var m = new TDropItemsMgr();
        m.Lock();
        try
        {
            m.Lock();          // Monitor 可重入
            m.UnLock();
        }
        finally
        {
            m.UnLock();
        }
        Assert.Equal(0, m.Count);
    }
}

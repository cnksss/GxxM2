// ============================================================================
// 测试：本车道 **物品容器片**（切片 2 / 4 的另一半）。
// 被测托管源：GXX.M2Server/Engine/PlayerSurface/TCreature.PlayerSurface.Items.cs
// 原文出处：Source/M2Engine/ObjBase.pas:322 / 570 / 630 / 708 / 752 / 882 /
//           13691-13703 / 26738-26752 / 41668-41684 / 41968-41974 / 35605
//           Source/M2Engine/ObjPlayer.pas:1196-1197 / 1264 / 3360-3394 / 12640-12657 / 16298
//           Source/Common/Grobal2.pas:51 / 102 / 4169
// 用例 ≥3/方法：0 / 边界（恰好满）/ 超界 / 差异断言。
//
// ⚠ 托管侧类型偏差（已在报告登记）：原文 `pTUserItem`（指向值类型 `TUserItem`）在托管侧
//   统一用 `TUserItemView`（`Engine/AddAbility.cs:64`，引用类型，既有约定）；
//   本片因此复用既有的 `m_UseItems`（`RecalcChain.cs:101`）而**未重复声明**该字段。
// ============================================================================

using System;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

public class PlayerSurfaceItemsTests : IDisposable
{
    public PlayerSurfaceItemsTests()
    {
        PlayerSurfaceBaseSeams.ResetDefaults();
        PlayerSurfaceItemSeams.ResetDefaults();
        PlayerSurfaceMsgSeams.ResetDefaults();
    }

    public void Dispose()
    {
        PlayerSurfaceBaseSeams.ResetDefaults();
        PlayerSurfaceItemSeams.ResetDefaults();
        PlayerSurfaceMsgSeams.ResetDefaults();
    }

    /// <summary>
    /// 物品工厂。★ 2026 第六轮（方案 A 第②步）：返回**权威记录 `TUserItem`**（不再是视图
    /// `TUserItemView`）——`MakeIndex`/`NameStr`/`Dura`/`DuraMax` 现在是**真实字段**直接赋值；
    /// 原先支撑它们的 4 个 `Seam*` 字典与 `WireItemFieldSeams()` 因此**全部删除**
    /// （那 4 个"默认静默返回 0/空串"的委托容易把字段语义错伪装成分支没命中）。
    /// </summary>
    private static TUserItem Item(ushort idx, int makeIndex = 0, string name = "")
    {
        var it = new TUserItem { wIndex = idx, MakeIndex = makeIndex, Dura = 0, DuraMax = 0 };
        if (name != "") it.NameStr = name;
        return it;
    }

    private static TStdItem Std(byte stdMode = 0, string name = "")
    {
        var s = new TStdItem { StdMode = stdMode };
        if (name != "") s.NameStr = name;
        return s;
    }

    // ★ `WireItemFieldSeams()` 已删除：4 个 `ItemMakeIndex/ItemName/ItemDura/ItemDuraMax` 委托随之删除，
    //   物品字段改为直读 `TUserItem`（方案 A 第②步）。

    // ---------------------------------------------------------------
    // m_UseItems 复用既有字段（★ 防重复声明）
    // ---------------------------------------------------------------

    [Fact]
    public void UseItems_ExistingSlotArrayIsReused_NotRedeclared()
    {
        // ★ `Engine/RecalcChain.cs:101` 已声明 `public TUserItemView?[] m_UseItems`
        //   本车道**未重复声明**（否则 CS0102）。反射确认只有一个字段。
        var fields = typeof(TPlayObject).GetFields(
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        int count = 0;
        System.Type? t = null;
        foreach (var f in fields) if (f.Name == "m_UseItems") { count++; t = f.FieldType; }
        Assert.Equal(1, count);
        Assert.Equal(typeof(TUserItemView[]), t);

        // 槽位数 = UseSlots.SlotCount = 21（原文是 30，偏差已在报告登记）
        var p = new TPlayObject();
        Assert.Equal(UseSlots.SlotCount, p.m_UseItems.Length);
        Assert.Equal(21, UseSlots.SlotCount);
    }

    [Fact]
    public void UseItems_WeaponSlotIndex_IsOne_AndInRange()
    {
        // 原文 Grobal2.pas:102 `U_WEAPON = 1; // 武器`
        Assert.Equal(1, PlayerSurfaceConst.U_WEAPON);
        Assert.Equal(UseSlots.U_WEAPON, PlayerSurfaceConst.U_WEAPON);
        Assert.True(UseSlots.U_WEAPON < UseSlots.SlotCount);
    }

    [Fact]
    public void UseItems_NewSlotIsNull_NotAZeroedItem()
    {
        // ★ 差异断言：原文 `m_UseItems` 是**值数组**，新元素 = 全零 TUserItem（wIndex = 0）；
        //   托管是**引用数组**，新元素 = null → 读 `.wIndex` 会 NRE。
        var p = new TPlayObject();
        Assert.Null(p.m_UseItems[UseSlots.U_WEAPON]);

        // 写入（ObtainWapon 场景：ObjNpc.pas:1886 `User.m_UseItems[U_WEAPON].wIndex := 0;`）
        p.m_UseItems[UseSlots.U_WEAPON] = new TUserItemView { wIndex = 1234 };
        Assert.Equal((ushort)1234, p.m_UseItems[UseSlots.U_WEAPON]!.wIndex);

        // 就地清零（原文 1886 的等价写法）
        p.m_UseItems[UseSlots.U_WEAPON]!.wIndex = 0;
        Assert.Equal((ushort)0, p.m_UseItems[UseSlots.U_WEAPON]!.wIndex);
    }

    [Fact]
    public void UseItems_IsReadWrite_ForUpgradeWaponOuterBody()
    {
        // ObjNpc.pas:1850 读 `User.m_UseItems[U_WEAPON].wIndex <> 0`，
        // ObjNpc.pas:1880 读整件、1886 写回 wIndex = 0 —— 三个动作都要能表达
        var p = new TPlayObject();
        p.m_UseItems[UseSlots.U_WEAPON] = new TUserItemView { wIndex = 42 };
        Assert.NotEqual(0, p.m_UseItems[UseSlots.U_WEAPON]!.wIndex);  // 1850 的判定

        var saved = p.m_UseItems[UseSlots.U_WEAPON];                   // 1880 读整件
        Assert.Equal((ushort)42, saved!.wIndex);

        p.m_UseItems[UseSlots.U_WEAPON]!.wIndex = 0;                   // 1886 写回
        Assert.Equal((ushort)0, p.m_UseItems[UseSlots.U_WEAPON]!.wIndex);
    }

    // ---------------------------------------------------------------
    // GetMaxBagCount / IsEnoughBag / IsEnoughBagEx（原文 630/1264/13691-13703）
    // ---------------------------------------------------------------

    [Fact]
    public void GetMaxBagCount_IsVirtual_AndOverriddenByPlayObject()
    {
        // ★ 原文 ObjPlayer.pas:1264 是 `override` → 基类必须 virtual
        var baseM = typeof(TCreature).GetMethod("GetMaxBagCount", Type.EmptyTypes);
        Assert.NotNull(baseM);
        Assert.True(baseM!.IsVirtual);

        var overM = typeof(TPlayObject).GetMethod("GetMaxBagCount", Type.EmptyTypes);
        Assert.NotNull(overM);
        Assert.NotEqual(baseM, overM);
    }

    [Fact]
    public void GetMaxBagCount_DefaultSeam_ReturnsDefMaxBagItem()
    {
        var p = new TPlayObject();
        Assert.Equal(PlayerSurfaceConst.DEF_MAX_BAG_ITEM, p.GetMaxBagCount());
    }

    [Fact]
    public void GetMaxBagCount_HonoursSeamOverride()
    {
        // TPlayObject.GetMaxBagCount 覆写转发接缝 → 可注入
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 10;
        var p = new TPlayObject();
        Assert.Equal(10, p.GetMaxBagCount());
    }

    [Fact]
    public void IsEnoughBag_EmptyBag_True_WhenBelowCapacity()
    {
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 2;
        var p = new TPlayObject();
        Assert.True(p.IsEnoughBag());
    }

    [Fact]
    public void IsEnoughBag_CountEqualsCapacity_False_BecauseStrictLessThan()
    {
        // ★ 原文 13694 用的是 `<`（严格小于）→ 正好满时返回 False
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 1;
        var p = new TPlayObject();
        Assert.True(p.AddItemToBag(Item(1)));
        Assert.False(p.IsEnoughBag());
    }

    [Fact]
    public void IsEnoughBag_CountAboveCapacity_False()
    {
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 1;
        var p = new TPlayObject();
        p.AddItemToBag(Item(1));          // 满了
        Assert.False(p.AddItemToBag(Item(2)));  // 被拒
        Assert.False(p.IsEnoughBag());
        Assert.Single(p.Bag);
    }

    [Fact]
    public void IsEnoughBagEx_CountEqualsCapacity_True_DiffersFromIsEnoughBag()
    {
        // ★ 差异断言：IsEnoughBagEx 是 `Count + n <= Max`（**可等于**），
        //   而 IsEnoughBag 是 `Count < Max`（不可等于）。
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 2;
        var p = new TPlayObject();
        p.AddItemToBag(Item(1));

        Assert.True(p.IsEnoughBagEx(1));    // 1 + 1 <= 2 → True
        Assert.True(p.IsEnoughBag());       // 1 < 2    → True

        p.AddItemToBag(Item(2));
        Assert.True(p.IsEnoughBagEx(0));    // 2 + 0 <= 2 → True
        Assert.False(p.IsEnoughBag());      // 2 < 2     → False（差异点）
    }

    [Fact]
    public void IsEnoughBagEx_NegativeCount_Behaviour()
    {
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 2;
        var p = new TPlayObject();
        Assert.True(p.IsEnoughBagEx(-5));   // 0 - 5 <= 2 → True（原文不做负数守卫）
    }

    // ---------------------------------------------------------------
    // AddItemToBag（原文 26743-26752）
    // ---------------------------------------------------------------

    [Fact]
    public void AddItemToBag_UnderCapacity_AddsAndReturnsTrue()
    {
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 3;
        var p = new TPlayObject();
        Assert.True(p.AddItemToBag(Item(100, 7)));
        Assert.Single(p.Bag);
        Assert.Equal((ushort)100, p.Bag[0]!.Value.wIndex);
    }

    [Fact]
    public void AddItemToBag_AtCapacity_RejectsAndReturnsFalse()
    {
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 1;
        var p = new TPlayObject();
        Assert.True(p.AddItemToBag(Item(1)));
        Assert.False(p.AddItemToBag(Item(2)));
        Assert.Single(p.Bag);
        Assert.Equal((ushort)1, p.Bag[0]!.Value.wIndex);
    }

    [Fact]
    public void AddItemToBag_ZeroCapacity_AlwaysRejects()
    {
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 0;
        var p = new TPlayObject();
        Assert.False(p.AddItemToBag(Item(9)));
        Assert.Empty(p.Bag);
    }

    [Fact]
    public void AddItemToBag_CallsWeightChanged()
    {
        // 原文 26749：WeightChanged();
        int calls = 0;
        PlayerSurfaceBaseSeams.WeightChanged = _ => calls++;
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 5;

        var p = new TPlayObject();
        p.AddItemToBag(Item(1));

        Assert.Equal(1, calls);
    }

    [Fact]
    public void AddItemToBag_Rejected_DoesNotCallWeightChanged()
    {
        int calls = 0;
        PlayerSurfaceBaseSeams.WeightChanged = _ => calls++;
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 0;

        var p = new TPlayObject();
        Assert.False(p.AddItemToBag(Item(1)));
        Assert.Equal(0, calls);
    }

    [Fact]
    public void AddItemToBag_IsVirtual()
    {
        // ★ 原文 ObjBase.pas:708 声明带 `virtual`
        var m = typeof(TCreature).GetMethod("AddItemToBag");
        Assert.NotNull(m);
        Assert.True(m!.IsVirtual, "AddItemToBag 必须是虚方法（原文 ObjBase.pas:708）");
    }

    [Fact]
    public void AddItemToBag_KeepsReferenceSemantics_LikeOriginalPointerList()
    {
        // ★★ 语义变更（方案 A 的固有代价，2026 第六轮）：元素是**可空值类型** `TUserItem?`，
        //    与原文 `TList` + `pTUserItem` 的**别名语义不同** —— `AddItemToBag(it)` 之后 `it.wIndex = 999`
        //    **不会**反映到背包（值被复制）。原文此处反映得到。本用例因此改为断言**新语义**，
        //    并把"必须写回槽位"的做法一并演示（这是调用方契约的一部分）。
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 10;
        var p = new TPlayObject();
        var it = Item(5, 1);
        p.AddItemToBag(it);

        it.wIndex = 999;                              // 只改**本地副本**
        Assert.Equal((ushort)5, p.Bag[0]!.Value.wIndex);   // ★ 背包里仍是复制进去的旧值（别名语义已失）
    }

    // ---------------------------------------------------------------
    // CheckItems（原文 41668-41684）
    // ---------------------------------------------------------------

    [Fact]
    public void CheckItems_EmptyBag_ReturnsMinusOne()
    {
        var p = new TPlayObject();
        Assert.Equal(-1, p.CheckItems("黑铁矿", out TUserItem? found));
        Assert.Null(found);
    }

    [Fact]
    public void CheckItems_FindsByName_ReturnsIndexAndItem()
    {
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 10;
        PlayerSurfaceItemSeams.GetStdItemName = idx => idx == 100 ? "黑铁矿" : "别的";

        var p = new TPlayObject();
        var b = Item(100, 42, "custom");
        p.AddItemToBag(Item(7));
        p.AddItemToBag(b);

        int i = p.CheckItems("黑铁矿", out var found);
        Assert.Equal(1, i);
        // ★ 方案 A：元素是**值类型** `TUserItem?` → 不能再用 `Assert.Same`（引用相等），改为断言内容。
        Assert.Equal(b.wIndex, found!.Value.wIndex);
        Assert.Equal(b.MakeIndex, found!.Value.MakeIndex);
        Assert.Equal((ushort)100, found!.Value.wIndex);
    }

    [Fact]
    public void CheckItems_NameCompareIsCaseInsensitive_LikeDelphiCompareText()
    {
        // 原文 41678 用 Delphi `CompareText`（不区分大小写）
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 10;
        PlayerSurfaceItemSeams.GetStdItemName = _ => "BlackIron";

        var p = new TPlayObject();
        p.AddItemToBag(Item(1));

        Assert.Equal(0, p.CheckItems("blackiron", out TUserItem? _));
        Assert.Equal(0, p.CheckItems("BLACKIRON", out TUserItem? _));
        Assert.Equal(-1, p.CheckItems("blackironx", out TUserItem? _));
    }

    [Fact]
    public void CheckItems_FirstMatchWins_WhenDuplicates()
    {
        // 原文 41681 Break → 取第一个
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 10;
        PlayerSurfaceItemSeams.GetStdItemName = _ => "same";

        var p = new TPlayObject();
        var a = Item(1, 11);
        var b = Item(2, 22);
        p.AddItemToBag(a);
        p.AddItemToBag(b);

        Assert.Equal(0, p.CheckItems("same", out var found));
        // ★ 方案 A：元素是值类型 → `Assert.Same`（引用相等）不再适用；用 `MakeIndex` 区分
        //   "取到的是 a 而不是 b"，**仍然锁死"第一个命中者胜出"这一语义**（原文 41681 的 Break）。
        Assert.Equal(11, found!.Value.MakeIndex);
        Assert.Equal(a.MakeIndex, found!.Value.MakeIndex);
        Assert.NotEqual(b.MakeIndex, found!.Value.MakeIndex);
    }

    [Fact]
    public void CheckItems_OverloadWithoutOutParam_ReturnsIndexOnly()
    {
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 10;
        PlayerSurfaceItemSeams.GetStdItemName = _ => "x";
        var p = new TPlayObject();
        p.AddItemToBag(Item(1));
        Assert.Equal(0, p.CheckItemsIndex("x", out TUserItem? _));
    }

    // ---------------------------------------------------------------
    // IsAddWeightAvailable（原文 41968-41974）
    // ---------------------------------------------------------------

    [Fact]
    public void IsAddWeightAvailable_FitsExactly_True()
    {
        // 原文 41972 是 `<=`
        var p = new TPlayObject();
        p.m_wAbil.Weight = 10;
        p.m_wAbil.MaxWeight = 20;
        Assert.True(p.IsAddWeightAvailable(10));
    }

    [Fact]
    public void IsAddWeightAvailable_OneOver_False()
    {
        var p = new TPlayObject();
        p.m_wAbil.Weight = 10;
        p.m_wAbil.MaxWeight = 20;
        Assert.False(p.IsAddWeightAvailable(11));
    }

    [Fact]
    public void IsAddWeightAvailable_Zero_TrueWhenNotOverloaded()
    {
        var p = new TPlayObject();
        p.m_wAbil.Weight = 10;
        p.m_wAbil.MaxWeight = 20;
        Assert.True(p.IsAddWeightAvailable(0));
    }

    [Fact]
    public void IsAddWeightAvailable_OverloadedWithZero_False()
    {
        // 已超重：Weight > MaxWeight 时，连 0 都加不进去
        var p = new TPlayObject();
        p.m_wAbil.Weight = 30;
        p.m_wAbil.MaxWeight = 20;
        Assert.False(p.IsAddWeightAvailable(0));
    }

    [Fact]
    public void IsAddWeightAvailable_NegativeWeight_CanReturnTrue_NoClamp()
    {
        // ★ 原文不做「非负」守卫：负数减重后 <= Max 即 True
        var p = new TPlayObject();
        p.m_wAbil.Weight = 30;
        p.m_wAbil.MaxWeight = 20;
        Assert.True(p.IsAddWeightAvailable(-10));
        Assert.True(p.IsAddWeightAvailable(-100));   // 30 - 100 = -70 <= 20 → True
    }

    // ---------------------------------------------------------------
    // SendAddItem / SendDelItem（原文 3360-3394 / 12640-12657）
    // ---------------------------------------------------------------

    [Fact]
    public void SendAddItem_Offline_DoesNothing()
    {
        var p = new TPlayObject { m_boOffLine = true };
        int sendDef = 0;
        PlayerSurfaceMsgSeams.SendDefMessage = (_, _, _, _, _, _, _) => sendDef++;
        PlayerSurfaceItemSeams.GetStdItem = _ => Std(1);

        p.SendAddItem(Item(1));

        Assert.Equal(0, sendDef);
    }

    [Fact]
    public void SendAddItem_DummyObject_DoesNothing()
    {
        var p = new TPlayObject { m_boDummyObject = true };
        int sendDef = 0;
        PlayerSurfaceMsgSeams.SendDefMessage = (_, _, _, _, _, _, _) => sendDef++;
        PlayerSurfaceItemSeams.GetStdItem = _ => Std(1);

        p.SendAddItem(Item(1));

        Assert.Equal(0, sendDef);
    }

    [Fact]
    public void SendAddItem_UnknownStdItem_DoesNothing()
    {
        // 原文 3370-3371：StdItem = nil then Exit
        var p = new TPlayObject();
        int sendDef = 0;
        PlayerSurfaceMsgSeams.SendDefMessage = (_, _, _, _, _, _, _) => sendDef++;
        PlayerSurfaceItemSeams.GetStdItem = _ => null;

        p.SendAddItem(Item(1));

        Assert.Equal(0, sendDef);
    }

    [Fact]
    public void SendAddItem_KnownItem_SendsSmAddItem()
    {
        var p = new TPlayObject();
        ushort ident = 0;
        PlayerSurfaceItemSeams.GetStdItem = _ => Std(1);
        PlayerSurfaceMsgSeams.SendDefMessage = (_, w, _, _, _, _, _) => ident = w;

        p.SendAddItem(Item(1));

        Assert.Equal(Grobal2Const.SM_ADDITEM, ident);
    }

    [Fact]
    public void SendAddItem_PerfectDuraBead_DoesNotConsumeRecordBeadExp()
    {
        // 原文 3388：三个条件都要满足；Dura == DuraMax（不是 `<`）→ 不结算
        var p = new TPlayObject { m_dwRecordBeadExp = 500 };
        uint incBead = 0;
        PlayerSurfaceItemSeams.IncBeadExp = (_, v) => incBead = v;
        PlayerSurfaceItemSeams.GetStdItem = _ => Std(49);

        var it = Item(1);
        it.Dura = 10;
        it.DuraMax = 10;
        p.SendAddItem(it);

        Assert.Equal(0u, incBead);
        Assert.Equal(500u, p.m_dwRecordBeadExp);
    }

    [Fact]
    public void SendAddItem_BeadItemWithDuraLessThanMax_ConsumesRecordBeadExp()
    {
        // 原文 3388-3392：StdMode = 49 且 Dura < DuraMax → 清零并把**旧值**交给 IncBeadExp
        var p = new TPlayObject { m_dwRecordBeadExp = 500 };
        uint incBead = 0;
        PlayerSurfaceItemSeams.IncBeadExp = (_, v) => incBead = v;
        PlayerSurfaceItemSeams.GetStdItem = _ => Std(49);

        var it = Item(1);
        it.Dura = 9;
        it.DuraMax = 10;
        p.SendAddItem(it);

        Assert.Equal(500u, incBead);
        Assert.Equal(0u, p.m_dwRecordBeadExp);
    }

    [Fact]
    public void SendAddItem_NonBeadStdMode_DoesNotConsumeEvenWithLowDura()
    {
        var p = new TPlayObject { m_dwRecordBeadExp = 500 };
        uint incBead = 0;
        PlayerSurfaceItemSeams.IncBeadExp = (_, v) => incBead = v;
        PlayerSurfaceItemSeams.GetStdItem = _ => Std(48);

        var it = Item(1);
        it.Dura = 1;
        it.DuraMax = 10;
        p.SendAddItem(it);

        Assert.Equal(0u, incBead);
        Assert.Equal(500u, p.m_dwRecordBeadExp);
    }

    [Fact]
    public void SendAddItem_ZeroRecordBeadExp_DoesNotConsume()
    {
        var p = new TPlayObject { m_dwRecordBeadExp = 0 };
        int incCalls = 0;
        PlayerSurfaceItemSeams.IncBeadExp = (_, _) => incCalls++;
        PlayerSurfaceItemSeams.GetStdItem = _ => Std(49);

        var it = Item(1);
        it.Dura = 1;
        it.DuraMax = 10;
        p.SendAddItem(it);

        Assert.Equal(0, incCalls);
    }

    [Fact]
    public void SendAddItem_FunctionNpcBranch_SetsAndResetsCurrentItemFields()
    {
        // 原文 3374-3381：夹在置位/复位之间调用 GotoLable('@AddBag')
        var p = new TPlayObject { m_btRaceServer = Grobal2Const.RC_PLAYOBJECT };
        var fn = new object();
        PlayerSurfaceItemSeams.FunctionNPC = fn;
        PlayerSurfaceItemSeams.GetStdItem = _ => Std(1, "大刀");
        // 原文 3376：`m_nCurrentItemMakeIndex := UserItem.MakeIndex` —— MakeIndex 现在是 TUserItem 的真实字段

        string? label = null;
        int makeIndexDuringCall = -1;
        string? nameDuringCall = null;
        PlayerSurfaceItemSeams.FunctionNpcGotoLable = (host, _, l, _) =>
        {
            Assert.Same(fn, host);
            label = l;
            makeIndexDuringCall = p.m_nCurrentItemMakeIndex;
            nameDuringCall = p.m_sCurrentItemName;
        };

        // ★ 方案 A：`MakeIndex` 现在是 `TUserItem` 的真实字段（原先靠 `ItemMakeIndex` 委托注入 777）
        p.SendAddItem(Item(1, 777));

        Assert.Equal("@AddBag", label);
        Assert.Equal(777, makeIndexDuringCall);      // 置位已生效
        Assert.Equal("大刀", nameDuringCall);
        Assert.Equal(0, p.m_nCurrentItemMakeIndex);  // 已复位
        Assert.Equal("", p.m_sCurrentItemName);
    }

    [Fact]
    public void SendDelItem_UsesCustomName_WhenBtValue13IsOneAndNameNonEmpty()
    {
        // 原文 12651-12654
        var p = new TPlayObject();
        string msg = "";
        PlayerSurfaceItemSeams.GetStdItem = _ => Std();
        PlayerSurfaceMsgSeams.SendDefMessage = (_, _, _, _, _, _, s) => msg = s;

        var it = Item(1, 5, "自定义名");
        it.SetBtValue(13, 1);
        p.SendDelItem(it);

        Assert.Equal("自定义名", msg);
    }

    [Fact]
    public void SendDelItem_FallsBackToStdName_WhenBtValue13NotOne()
    {
        var p = new TPlayObject();
        string msg = "";
        PlayerSurfaceItemSeams.GetStdItem = _ => Std(0, "标准名");
        PlayerSurfaceMsgSeams.SendDefMessage = (_, _, _, _, _, _, s) => msg = s;

        var it = Item(1, 5, "自定义名");
        it.SetBtValue(13, 0);
        p.SendDelItem(it);

        Assert.Equal("标准名", msg);
    }

    [Fact]
    public void SendDelItem_FallsBackToStdName_WhenBtValue13IsOneButNameEmpty()
    {
        // ★ 差异断言：`btValue[13] = 1` **且** `Name <> ''` 两个条件都要满足
        var p = new TPlayObject();
        string msg = "";
        PlayerSurfaceItemSeams.GetStdItem = _ => Std(0, "标准名");
        PlayerSurfaceMsgSeams.SendDefMessage = (_, _, _, _, _, _, s) => msg = s;

        var it = Item(1, 5, "");
        it.SetBtValue(13, 1);
        p.SendDelItem(it);

        Assert.Equal("标准名", msg);
    }

    [Fact]
    public void SendDelItem_UnknownStdItem_DoesNotSend()
    {
        var p = new TPlayObject();
        int calls = 0;
        PlayerSurfaceItemSeams.GetStdItem = _ => null;
        PlayerSurfaceMsgSeams.SendDefMessage = (_, _, _, _, _, _, _) => calls++;

        p.SendDelItem(Item(1));

        Assert.Equal(0, calls);
    }

    [Fact]
    public void SendDelItem_Offline_DoesNothing()
    {
        var p = new TPlayObject { m_boOffLine = true };
        int calls = 0;
        PlayerSurfaceItemSeams.GetStdItem = _ => Std();
        PlayerSurfaceMsgSeams.SendDefMessage = (_, _, _, _, _, _, _) => calls++;

        p.SendDelItem(Item(1));

        Assert.Equal(0, calls);
    }

    // ---------------------------------------------------------------
    // CheckItemsNeed（原文 16298-）
    // ---------------------------------------------------------------

    [Theory]
    [InlineData(6)]
    [InlineData(60)]
    [InlineData(7)]
    [InlineData(70)]
    [InlineData(8)]
    [InlineData(0)]
    public void CheckItemsNeed_NoGuildNoCastleNoMembership_ReturnsFalseForGuardedNeeds(int need)
    {
        // 原文 16304 起的 case：6/60 需 guild；7/70 需 castle；8 需会员
        var p = new TPlayObject();
        var std = new TStdItem { Need = need };
        bool r = p.CheckItemsNeed(ref std);

        if (need == 0) Assert.True(r);      // 无对应分支 → 保持 Result := True
        else Assert.False(r);
    }

    [Fact]
    public void CheckItemsNeed_Need81_DefaultState_DoesNotTrip_WhenLoWordIsZero()
    {
        // ⚠ 原文易错点（ObjPlayer.pas:16340）：
        //   `if (m_nMemberType <> LoWord(StdItem.NeedLevel)) or (m_nMemberLevel < HiWord(...)) then`
        //   `NeedLevel = 0` 时 LoWord = 0、HiWord = 0 → 默认 m_nMemberType = 0、m_nMemberLevel = 0
        //   → **两个条件都为假** → 返回 True（即「Need = 81 且 NeedLevel 未设置时默认通过」）。
        var p = new TPlayObject();
        var std = new TStdItem { Need = 81, NeedLevel = 0 };
        Assert.True(p.CheckItemsNeed(ref std));
    }

    [Fact]
    public void CheckItemsNeed_Need6_WithGuild_ReturnsTrue()
    {
        PlayerSurfaceItemSeams.MyGuild = _ => new object();
        var p = new TPlayObject();
        var std = new TStdItem { Need = 6 };
        Assert.True(p.CheckItemsNeed(ref std));
    }

    [Fact]
    public void CheckItemsNeed_Need60_RequiresGuildAndRankOne()
    {
        var p = new TPlayObject();
        var std = new TStdItem { Need = 60 };

        PlayerSurfaceItemSeams.MyGuild = _ => new object();
        PlayerSurfaceItemSeams.GuildRankNo = _ => 2;
        Assert.False(p.CheckItemsNeed(ref std));    // 有行会但不是 1

        PlayerSurfaceItemSeams.GuildRankNo = _ => 1;
        Assert.True(p.CheckItemsNeed(ref std));
    }

    [Fact]
    public void CheckItemsNeed_Need7_RequiresCastleMembership()
    {
        var p = new TPlayObject();
        var std = new TStdItem { Need = 7 };
        Assert.False(p.CheckItemsNeed(ref std));

        PlayerSurfaceItemSeams.IsCastleMember = _ => new object();
        Assert.True(p.CheckItemsNeed(ref std));
    }

    [Fact]
    public void CheckItemsNeed_Need8_RequiresNonZeroMemberType()
    {
        var p = new TPlayObject();
        var std = new TStdItem { Need = 8 };
        Assert.False(p.CheckItemsNeed(ref std));

        PlayerSurfaceItemSeams.MemberType = _ => 3;
        Assert.True(p.CheckItemsNeed(ref std));
    }

    [Fact]
    public void CheckItemsNeed_Need81_UsesLoWordAndHiWordOfNeedLevel()
    {
        // 原文 16340：m_nMemberType <> LoWord(NeedLevel) or m_nMemberLevel < HiWord(NeedLevel)
        var p = new TPlayObject();
        var std = new TStdItem { Need = 81, NeedLevel = unchecked((5 << 16) | 3) };

        PlayerSurfaceItemSeams.MemberType = _ => 3;
        PlayerSurfaceItemSeams.MemberLevel = _ => 5;
        Assert.True(p.CheckItemsNeed(ref std));

        PlayerSurfaceItemSeams.MemberLevel = _ => 4;      // < HiWord → False
        Assert.False(p.CheckItemsNeed(ref std));

        PlayerSurfaceItemSeams.MemberLevel = _ => 5;
        PlayerSurfaceItemSeams.MemberType = _ => 4;       // <> LoWord → False
        Assert.False(p.CheckItemsNeed(ref std));
    }
}

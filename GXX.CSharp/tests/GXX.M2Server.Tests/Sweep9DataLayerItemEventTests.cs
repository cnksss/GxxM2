// 源单元：Source/M2Engine/ItemEvent.pas（389 行）
// 对应实现：src/GXX.M2Server/Sweep9/DataLayer/ItemEvent.cs（TGameObject + TItemObject）
//           src/GXX.M2Server/Sweep9/DataLayer/ItemEventManager.cs（TItemManager）
//
// 覆盖口径：**每个公开成员至少一条用例**；每个分支/边界/原文缺陷都有差异断言。
// 全部用例只依赖 Sweep9DataLayerTestKit 的内存替身，**不连真实数据库/COM/地图**。

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Sweep9.DataLayer;
using Xunit;

using TGList = GXX.Core.Protocol.SDK.TGList;

namespace GXX.M2Server.Tests;

// ===========================================================================
// 一、TItemObject 构造（ItemEvent.pas:62-83）与字段默认值
// ===========================================================================
public class Sweep9DataLayerItemObjectCreateTests
{
    [Fact]
    public void Ctor_SetsObjGameToObjItem()
    {
        Sweep9DataLayerTestKit.Isolate();
        var item = new TItemObject();
        Assert.Equal(TObjGame.Obj_Item, item.m_ObjGame);
    }

    [Fact]
    public void Ctor_InitialisesAllStringFieldsToEmpty()
    {
        Sweep9DataLayerTestKit.Isolate();
        var item = new TItemObject();
        Assert.Equal("", item.m_sName);
        Assert.Equal("", item.m_sDBName);
    }

    [Fact]
    public void Ctor_ZeroesNumericFields()
    {
        Sweep9DataLayerTestKit.Isolate();
        var item = new TItemObject();
        Assert.Equal((ushort)0, item.m_wLooks);
        Assert.Equal((ushort)0, item.m_wAniCount);
        Assert.Equal((byte)0, item.m_btReserved);
        Assert.Equal(0, item.m_nCount);
        Assert.Equal(0u, item.m_dwCanPickUpTick);
        Assert.Equal(0u, item.m_dwGhostTick);
    }

    [Fact]
    public void Ctor_NullsBothBaseObjectPointersAndEnvir()
    {
        Sweep9DataLayerTestKit.Isolate();
        var item = new TItemObject();
        Assert.Null(item.m_OfBaseObject);
        Assert.Null(item.m_DropBaseObject);
        Assert.Null(item.m_PEnvir);
    }

    [Fact]
    public void Ctor_GhostFalseAndColorIs255()
    {
        Sweep9DataLayerTestKit.Isolate();
        var item = new TItemObject();
        Assert.False(item.m_boGhost);
        Assert.Equal((byte)255, item.m_btColor);   // 原文 :80 `m_btColor := 255`
    }

    /// <summary>
    /// 原文 :25-27 三个字段**构造里不赋值**（`m_boDieDrop`/`m_boHumDrop`/`m_boNpcThrowItem`）
    /// —— C# 值类型默认 false，与 Delphi 对象内存清零后的 Boolean(0)=False 一致。差异断言锁定。
    /// </summary>
    [Fact]
    public void Ctor_LeavesThreeDropFlagsAtFalse_OriginalDoesNotAssignThem()
    {
        Sweep9DataLayerTestKit.Isolate();
        var item = new TItemObject();
        Assert.False(item.m_boDieDrop);
        Assert.False(item.m_boHumDrop);
        Assert.False(item.m_boNpcThrowItem);
    }

    /// <summary>
    /// 原文 :79 `m_dwRunTick := MyGetTickCount`、:82 `m_dwFloorItemCanPickUpTime :=
    /// g_Config.dwFloorItemCanPickUpTime` —— 两处都要被构造读到。
    /// </summary>
    [Fact]
    public void Ctor_ReadsTickCountAndConfigFloorPickUpTime()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 12345u;
        clock.Now = 7777u;

        var item = new TItemObject();
        Assert.Equal(7777u, item.m_dwRunTick);
        Assert.Equal(12345u, item.m_dwFloorItemCanPickUpTime);
    }

    /// <summary>
    /// 原文 :81 `FillChar(m_UserItem, SizeOf(TUserItem), #0)` ⇒ 托管侧 `default(TUserItem)`。
    /// **逐字节取证**（否定性断言必须计数取证，台账 §37.3）：全部字节必须为 0。
    /// </summary>
    [Fact]
    public void Ctor_ZeroesUserItemByteForByte()
    {
        Sweep9DataLayerTestKit.Isolate();
        var created = new TItemObject();
        TUserItem item = created.m_UserItem;

        int size = System.Runtime.InteropServices.Marshal.SizeOf<TUserItem>();

        Assert.True(size > 0, "TUserItem 应该是非空 packed 结构");
        // 用默认实例做对照：字段级相等 + 逐字段零值检查
        TUserItem zero = default;
        Assert.Equal(zero.MakeIndex, item.MakeIndex);
        Assert.Equal(zero.wIndex, item.wIndex);
        Assert.Equal(zero.Dura, item.Dura);
        Assert.Equal(zero.DuraMax, item.DuraMax);
        Assert.Equal(zero.btColor, item.btColor);
        Assert.Equal(zero.btBindOption, item.btBindOption);
        Assert.Equal(zero.wEffect, item.wEffect);
        Assert.Equal(zero.btFluteCount, item.btFluteCount);
        Assert.Equal(zero.nLimitTime, item.nLimitTime);
        Assert.Equal(zero.wInsuranceCount, item.wInsuranceCount);
        Assert.Equal(zero.wNewLooks, item.wNewLooks);
        Assert.Equal(zero.wNewShape, item.wNewShape);
        Assert.Equal(zero.wNewExpand3, item.wNewExpand3);
        Assert.Equal(zero.wNewExpand4, item.wNewExpand4);
        Assert.Equal(zero.NameStr, item.NameStr);
        Assert.Equal(zero.GetBtValue(0), item.GetBtValue(0));
        Assert.Equal(zero.GetBtValue(13), item.GetBtValue(13));
        Assert.Equal(zero.GetNewValue(0), item.GetNewValue(0));
        Assert.Equal(zero.GetNewValue(29), item.GetNewValue(29));
    }

    /// <summary>
    /// 逐字节取证的**指针版**：用 `unsafe` 直接比较两个栈上实例的原始字节
    /// （`sizeof` 由编译器给出，不依赖 Marshal）。
    /// </summary>
    [Fact]
    public unsafe void Ctor_UserItemRawBytesEqualDefault_ByteForByte()
    {
        Sweep9DataLayerTestKit.Isolate();
        var created = new TItemObject();
        TUserItem lhs = created.m_UserItem;
        TUserItem rhs = default;

        byte* a = (byte*)&lhs;
        byte* b = (byte*)&rhs;
        int size = sizeof(TUserItem);

        int diff = 0;
        for (int i = 0; i < size; i++) if (a[i] != b[i]) diff++;

        Assert.True(size > 0);
        Assert.Equal(0, diff);
    }
}

// ===========================================================================
// 二、TItemObject.Destroy（ItemEvent.pas:85-93）
// ===========================================================================
public class Sweep9DataLayerItemObjectDestroyTests
{
    [Fact]
    public void Destroy_WhenNoEnvir_DoesNothing()
    {
        Sweep9DataLayerTestKit.Isolate();
        var item = new TItemObject();
        item.Destroy();                       // 原文 :87 `m_PEnvir <> nil` 为假 ⇒ 直接 inherited
        Assert.Null(item.m_PEnvir);
    }

    [Fact]
    public void Destroy_WhenEnvirPresent_DeletesFromMapAtItemCoordsAndClearsEnvir()
    {
        Sweep9DataLayerTestKit.Isolate();
        var envir = new FakeItemGameEnvir();
        var item = new TItemObject { m_PEnvir = envir, m_nMapX = 33, m_nMapY = 44 };

        item.Destroy();

        Assert.Equal(new[] { "33/44" }, envir.DeleteCalls);
        Assert.Same(item, envir.DeletedObjects[0]);
        Assert.Null(item.m_PEnvir);           // 原文 :90 `m_PEnvir := nil`
    }

    /// <summary>返回值被丢弃：即使 `DeleteFromMap` 返回 false，`m_PEnvir` 仍被清空（原文如此）。</summary>
    [Fact]
    public void Destroy_IgnoresDeleteFromMapResult()
    {
        Sweep9DataLayerTestKit.Isolate();
        var envir = new FakeItemGameEnvir { DeleteResult = false };
        var item = new TItemObject { m_PEnvir = envir };

        item.Destroy();

        Assert.Equal(1, envir.DeleteCalls.Count);
        Assert.Null(item.m_PEnvir);
    }
}

// ===========================================================================
// 三、TItemObject.Run（ItemEvent.pas:95-157）
// ===========================================================================
public class Sweep9DataLayerItemObjectRunTests
{
    /// <summary>
    /// 原文 :102：`(MyGetTickCount - m_dwAddTime) > g_Config.dwClearDropOnFloorItemTime`
    /// —— 严格大于。边界（相等）**不**置幽灵。
    /// </summary>
    [Theory]
    [InlineData(1000u, 1000u, false)]   // 差 == 阈值 ⇒ 不置
    [InlineData(1001u, 1000u, true)]    // 差 == 阈值+1 ⇒ 置
    public void Run_Expiry_UsesStrictGreaterThan(uint now, uint threshold, bool expectGhost)
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => threshold;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;   // 不影响本用例

        var envir = new FakeItemGameEnvir();
        var item = new TItemObject { m_PEnvir = envir, m_dwAddTime = 0 };
        clock.Now = now;

        item.Run();

        Assert.Equal(expectGhost, item.m_boGhost);
    }

    /// <summary>到期时同时记 `m_dwGhostTick`（原文 :105）。</summary>
    [Fact]
    public void Run_Expiry_StampsGhostTick()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => 100u;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;

        var envir = new FakeItemGameEnvir();
        var item = new TItemObject { m_PEnvir = envir, m_dwAddTime = 0 };
        clock.Now = 500u;

        item.Run();

        Assert.True(item.m_boGhost);
        Assert.Equal(500u, item.m_dwGhostTick);
    }

    /// <summary>
    /// 原文缺陷 ①（差异断言）：同一 if 体内 `MyGetTickCount` 被调用**两次**
    /// （:102 判据、:105 赋值）。用自增假时钟把两次调用差暴露成 `+1`。
    /// </summary>
    [Fact]
    public void Run_Expiry_CallsTickCountTwice_TickStampIsOneAheadOfJudgement_OriginalFlaw()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => 100u;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;

        var envir = new FakeItemGameEnvir();
        var item = new TItemObject { m_PEnvir = envir, m_dwAddTime = 0 };

        clock.Now = 110u;
        clock.AutoIncrement = true;     // 每次 Peek 自增 ⇒ 判据读到 111、赋值读到 112
        clock.ResetCountersKeepingValue();

        item.Run();

        Assert.True(item.m_boGhost);
        // 判据那一次读到 111（111 - 0 > 100 成立），赋值那一次读到 112
        Assert.Equal(112u, item.m_dwGhostTick);
    }

    /// <summary>
    /// 原文 :110 `Envir.m_boFB and (not Envir.m_boFBCreate)` ⇒ 副本地图里**立即**置幽灵。
    /// </summary>
    [Fact]
    public void Run_FbMapWithoutCreator_MarksGhostImmediately()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;  // 到期永不触发
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;

        var envir = new FakeItemGameEnvir { m_boFB = true, m_boFBCreate = false };
        var item = new TItemObject { m_PEnvir = envir, m_dwAddTime = 0 };
        clock.Now = 1u;

        item.Run();

        Assert.True(item.m_boGhost);
        Assert.Equal(1u, item.m_dwGhostTick);
    }

    /// <summary>`m_boFBCreate = True`（副本创建者所在的那张图）时**不**立即清。</summary>
    [Fact]
    public void Run_FbMapWithCreator_DoesNotMarkGhost()
    {
        Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;

        var envir = new FakeItemGameEnvir { m_boFB = true, m_boFBCreate = true };
        var item = new TItemObject { m_PEnvir = envir, m_dwAddTime = 0 };

        item.Run();

        Assert.False(item.m_boGhost);
    }

    /// <summary>
    /// 原文缺陷（差异断言）：:109 是**无保护**硬转换 `TEnvirnoment(m_PEnvir)` ——
    /// `m_PEnvir = nil` 时读 `m_boFB` 即 AV。托管侧显式抛 <see cref="NullReferenceException"/>。
    /// </summary>
    [Fact]
    public void Run_NilEnvir_DereferencesAnyway_OriginalFlaw()
    {
        Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;

        var item = new TItemObject { m_PEnvir = null, m_dwAddTime = 0 };

        Assert.Throws<NullReferenceException>(() => item.Run());
    }

    /// <summary>
    /// 原文 :121：可捡期到期 ⇒ 双清 `m_OfBaseObject`/`m_DropBaseObject`（严格大于）。
    /// </summary>
    [Theory]
    [InlineData(500u, 500u, false)]
    [InlineData(501u, 500u, true)]
    public void Run_PickUpWindowExpiry_ClearsBothPointers_StrictGreaterThan(uint now, uint threshold, bool cleared)
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => threshold;

        var of = new FakeBaseObject();
        var drop = new FakeBaseObject();
        var item = new TItemObject
        {
            m_PEnvir = new FakeItemGameEnvir(),
            m_dwCanPickUpTick = 0,
            m_OfBaseObject = of,
            m_DropBaseObject = drop,
        };
        clock.Now = now;

        item.Run();

        if (cleared)
        {
            Assert.Null(item.m_OfBaseObject);
            Assert.Null(item.m_DropBaseObject);
        }
        else
        {
            Assert.Same(of, item.m_OfBaseObject);
            Assert.Same(drop, item.m_DropBaseObject);
        }
    }

    /// <summary>
    /// 原文 :127-136：未到期时，逐个检查两个基对象是否已幽灵 —— **各自独立清空**。
    /// </summary>
    [Fact]
    public void Run_GhostBaseObjects_AreClearedIndependently()
    {
        Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => uint.MaxValue;   // 可捡期永不到

        var of = new FakeBaseObject { m_boGhost = true };
        var drop = new FakeBaseObject { m_boGhost = false };
        Sweep9DataLayerSeam.IsBaseObjectGhost = o => o is FakeBaseObject b && b.m_boGhost;

        var item = new TItemObject
        {
            m_PEnvir = new FakeItemGameEnvir(),
            m_OfBaseObject = of,
            m_DropBaseObject = drop,
        };

        item.Run();

        Assert.Null(item.m_OfBaseObject);            // 已幽灵 ⇒ 清
        Assert.Same(drop, item.m_DropBaseObject);    // 未幽灵 ⇒ 留
    }

    /// <summary>对称的另一侧：`m_DropBaseObject` 幽灵、`m_OfBaseObject` 不幽灵。</summary>
    [Fact]
    public void Run_GhostDropObject_ClearedWhilePickObjectKept()
    {
        Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => uint.MaxValue;

        var of = new FakeBaseObject { m_boGhost = false };
        var drop = new FakeBaseObject { m_boGhost = true };
        Sweep9DataLayerSeam.IsBaseObjectGhost = o => o is FakeBaseObject b && b.m_boGhost;

        var item = new TItemObject
        {
            m_PEnvir = new FakeItemGameEnvir(),
            m_OfBaseObject = of,
            m_DropBaseObject = drop,
        };

        item.Run();

        Assert.Same(of, item.m_OfBaseObject);
        Assert.Null(item.m_DropBaseObject);
    }

    /// <summary>
    /// 两个基对象都为 nil ⇒ 取舍块（含 :121 的 `MyGetTickCount`）**根本不进**（原文 :119 的 `or`）。
    /// 计数取证：`Run()` 内部只发生 **1** 次 TickCount 读（:102 的到期判据）。
    /// </summary>
    [Fact]
    public void Run_BothBaseObjectsNil_SkipsWholePickUpBlock()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;

        var envir = new FakeItemGameEnvir();
        var item = new TItemObject { m_PEnvir = envir, m_OfBaseObject = null, m_DropBaseObject = null };

        clock.Now = 10u;
        int before = clock.ReadCount;
        item.Run();

        Assert.Equal(1, clock.ReadCount - before);
    }

    /// <summary>
    /// 对照组：两个基对象都非 nil ⇒ :121 的判据**再读一次** ⇒ 内部共 **2** 次读。
    /// 与上一例成对，构成"取舍块是否进入"的**计数取证**（台账 §37.3）。
    /// </summary>
    [Fact]
    public void Run_BothBaseObjectsPresent_ReadsTickCountTwice()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => uint.MaxValue;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;

        var item = new TItemObject
        {
            m_PEnvir = new FakeItemGameEnvir(),
            m_OfBaseObject = new FakeBaseObject(),
            m_DropBaseObject = new FakeBaseObject(),
        };

        clock.Now = 10u;
        int before = clock.ReadCount;
        item.Run();

        Assert.Equal(2, clock.ReadCount - before);
    }

    /// <summary>
    /// 原文 :139-156 的"已幽灵"块：删地图 + `NeedIdentify = 1` 时写日志 + 清 `m_PEnvir`。
    /// 断言 13 个实参**逐个**与原文 :150-151 一致。
    /// </summary>
    [Fact]
    public void Run_GhostWithEnvir_DeletesFromMapAndWritesDisappearLog()
    {
        Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.DwClearDropOnFloorItemTime = () => 0u;
        Sweep9DataLayerSeam.DwFloorItemCanPickUpTime = () => 0u;
        Sweep9DataLayerSeam.GetStdItem = idx => new TStdItem { NeedIdentify = 1, NameStr = "屠龙" };

        ItemEventGameDataLogArgs? captured = null;
        Sweep9DataLayerSeam.AddGameDataLog = a => captured = a;

        var envir = new FakeItemGameEnvir { sMapName = "0" };
        var item = new TItemObject { m_PEnvir = envir, m_nMapX = 5, m_nMapY = 6, m_dwAddTime = 0 };
        item.m_UserItem.wIndex = 1234;
        item.m_UserItem.MakeIndex = 99;
        item.m_boGhost = true;      // 直接进第三块

        item.Run();

        Assert.Equal(new[] { "5/6" }, envir.DeleteCalls);
        Assert.NotNull(captured);
        Assert.Equal(9, captured!.LogAction1);                 // LOG_ItemDisappear
        Assert.Equal(0, captured.LogAction2);                  // LOG_ActionNone
        Assert.Equal(0, captured.LogActorType);                // latNone
        Assert.Equal("0", captured.MapName);                   // sMapName
        Assert.Equal(5, captured.PointX);
        Assert.Equal(6, captured.PointY);
        Assert.Equal("屠龙", captured.ItemName);               // StdItem.Name
        Assert.Equal(99, captured.ItemMakeIndex);              // m_UserItem.MakeIndex
        Assert.Equal("0", captured.ActorName);                 // 字面量
        Assert.Equal("0", captured.TargetName);                // 字面量
        Assert.Equal(0, captured.Data1);
        Assert.Equal(0, captured.Data2);
        Assert.Equal("到时清理", captured.LogDesc);
        Assert.Null(item.m_PEnvir);                            // 原文 :154
    }

    /// <summary>`NeedIdentify <> 1` ⇒ **不**写日志（原文 :148）。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public void Run_Ghost_NoLogWhenNeedIdentifyIsNotOne(byte needIdentify)
    {
        Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.GetStdItem = idx => new TStdItem { NeedIdentify = needIdentify, NameStr = "X" };
        int logCount = 0;
        Sweep9DataLayerSeam.AddGameDataLog = _ => logCount++;

        var item = new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_boGhost = true };
        item.Run();

        Assert.Equal(0, logCount);
    }

    /// <summary>`GetStdItem` 返回 null ⇒ **不**写日志，且**不崩**（原文 :148 的第一项判据）。</summary>
    [Fact]
    public void Run_Ghost_NoLogWhenStdItemMissing()
    {
        Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.GetStdItem = _ => null;
        int logCount = 0;
        Sweep9DataLayerSeam.AddGameDataLog = _ => logCount++;

        var item = new TItemObject { m_PEnvir = new FakeItemGameEnvir(), m_boGhost = true };
        item.Run();

        Assert.Equal(0, logCount);
    }

    /// <summary>
    /// 原文缺陷 ③（差异断言）：:143 的 `if not DeleteFromMap(...) then begin end;` 是**空 then 块**
    /// ⇒ `DeleteFromMap` 返回 **false 也照样**写日志、照样 `m_PEnvir := nil`。
    /// </summary>
    [Fact]
    public void Run_Ghost_EmptyThenBlock_DeleteFailureStillLogsAndClearsEnvir_OriginalFlaw()
    {
        Sweep9DataLayerTestKit.Isolate();
        Sweep9DataLayerSeam.GetStdItem = _ => new TStdItem { NeedIdentify = 1, NameStr = "X" };
        int logCount = 0;
        Sweep9DataLayerSeam.AddGameDataLog = _ => logCount++;

        var envir = new FakeItemGameEnvir { DeleteResult = false };
        var item = new TItemObject { m_PEnvir = envir, m_boGhost = true };

        item.Run();

        Assert.Equal(1, envir.DeleteCalls.Count);
        Assert.Equal(1, logCount);       // 删除"失败"仍然写了日志
        Assert.Null(item.m_PEnvir);      // 也仍然清了 PEnvir
    }

    /// <summary>已幽灵但 `m_PEnvir = nil` ⇒ 第三块整体不进（原文 :141）。</summary>
    [Fact]
    public void Run_GhostWithoutEnvir_DoesNothing()
    {
        Sweep9DataLayerTestKit.Isolate();
        int logCount = 0;
        Sweep9DataLayerSeam.AddGameDataLog = _ => logCount++;

        var item = new TItemObject { m_PEnvir = null, m_boGhost = true };
        item.Run();

        Assert.Equal(0, logCount);
        Assert.Null(item.m_PEnvir);
    }
}

// ===========================================================================
// 四、TItemObject.MakeGhost（ItemEvent.pas:159-164）
// ===========================================================================
public class Sweep9DataLayerItemObjectMakeGhostTests
{
    [Fact]
    public void MakeGhost_SetsFlagAndStampsTick()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        clock.Now = 4321u;
        var item = new TItemObject();

        item.MakeGhost();

        Assert.True(item.m_boGhost);
        Assert.Equal(4321u, item.m_dwGhostTick);
    }

    /// <summary>原文 :163 `// m_PEnvir := nil;` 是**被注释掉的行** ⇒ `m_PEnvir` 保留。差异断言。</summary>
    [Fact]
    public void MakeGhost_DoesNotClearEnvir_CommentedOutInOriginal()
    {
        Sweep9DataLayerTestKit.Isolate();
        var envir = new FakeItemGameEnvir();
        var item = new TItemObject { m_PEnvir = envir };

        item.MakeGhost();

        Assert.Same(envir, item.m_PEnvir);
    }
}

// ===========================================================================
// 五、TGameObject 基类（ObjGame.pas:9-17，ItemEvent 的基类）
// ===========================================================================
public class Sweep9DataLayerGameObjectTests
{
    [Fact]
    public void GameObject_CtorDefaultsToObjNoneAndZeroCoords()
    {
        var (clock, _) = Sweep9DataLayerTestKit.Isolate();
        clock.Now = 88u;
        var go = new TGameObject();

        Assert.Equal(TObjGame.Obj_None, go.m_ObjGame);
        Assert.Equal(88u, go.m_dwAddTime);      // ObjGame.pas:54 MyGetTickCount
        Assert.Equal(0, go.m_nMapX);
        Assert.Equal(0, go.m_nMapY);
    }

    /// <summary>`TItemObject` 必须是 `TGameObject` 的派生类（原文 `class(TGameObject)`）。</summary>
    [Fact]
    public void ItemObject_IsDerivedFromGameObject()
    {
        Sweep9DataLayerTestKit.Isolate();
        Assert.True(typeof(TGameObject).IsAssignableFrom(typeof(TItemObject)));
    }
}

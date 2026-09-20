// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：TMerchant.sub_4A0218 —— 原文 `TMerchant.UpgradeWapon` 的**嵌套过程**
//           （ObjNpc.pas:1686-1828），升级材料剔除与属性聚合（143 行）。
//   外层 `UpgradeWapon` 本体（1830-1901）阻塞未做，见报告 §6.1-C 与 §7.2。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcSub4A0218Tests : IDisposable
{
    private readonly List<string> _logs = new();
    private readonly List<string> _sent = new();

    public NpcObjNpcSub4A0218Tests()
    {
        NpcSeams.ResetDefaults();
        NpcSeams.AddGameDataLog = (a1, a2, actor, item, makeIdx, target, d1, d2, desc) =>
            _logs.Add($"{a1}/{a2}/{item}/{makeIdx}/{desc}");
        NpcSeams.SendTo = (sender, target, ident, wParam, p1, p2, p3, sMsg) =>
            _sent.Add($"{ident}:{p1}:{sMsg}");
    }

    public void Dispose() => NpcSeams.ResetDefaults();

    private static TUserItem Item(ushort wIndex, ushort dura = 0, int makeIndex = 0)
        => new() { wIndex = wIndex, Dura = dura, MakeIndex = makeIndex };

    private static TStdItem Std(byte stdMode = 19, int dc1 = 0, int dc2 = 0, int sc1 = 0, int sc2 = 0,
        int mc1 = 0, int mc2 = 0, byte needIdentify = 0, string name = "STD")
    {
        var s = new TStdItem
        {
            StdMode = stdMode,
            DC1 = dc1,
            DC2 = dc2,
            SC1 = sc1,
            SC2 = sc2,
            MC1 = mc1,
            MC2 = mc2,
            NeedIdentify = needIdentify,
        };
        s.NameStr = name;
        return s;
    }

    /// <summary>把某个 wIndex 配成"黑铁矿"（走 GetStdItemName 判定）。</summary>
    private static void SetBlackStone(ushort idx)
    {
        NpcSeams.sBlackStone = "黑铁矿";
        NpcSeams.GetStdItemName = i => i == idx ? "黑铁矿" : "OTHER";
    }

    private static void SetUseItem(ushort idx, TStdItem std)
    {
        // IsUseItem 默认实现读 GetStdItem → 必须让 wIndex 命中该 StdItem
        NpcSeams.GetStdItem = i => i == idx ? std : null;
    }

    // -----------------------------------------------------------------------
    // 边界：空物品表
    // -----------------------------------------------------------------------

    [Fact]
    public void Sub4A0218_EmptyList_NoMaterialsYieldZeroAttributes()
    {
        // ⚠ 差异登记：原文 1820 的 `nDura / nItemCount`（实数除法）在 nItemCount = 0 时
        //    抛 Delphi EZeroDivide；托管侧 0/0 得 NaN，`(int)Math.Round(NaN)` 截断为 0，
        //    **不抛异常**。两侧都是"无意义值"，但一边崩一边给 0 —— 已在此锁死托管侧行为。
        var m = new TMerchant();
        var list = new List<object>();
        m.sub_4A0218(new TPlayObject(), list, out byte dc, out byte sc, out byte mc, out byte dura);
        Assert.Equal(0, dc);
        Assert.Equal(0, sc);
        Assert.Equal(0, mc);
        Assert.Equal(0, dura);
        Assert.Empty(_sent);   // DelItems 为空 → 不下发
    }

    [Fact]
    public void Sub4A0218_NullElementIsSkipped()
    {
        var m = new TMerchant();
        var list = new List<object> { null };
        m.sub_4A0218(new TPlayObject(), list, out _, out _, out _, out _);
        Assert.Single(list);   // Continue → 不删除
    }

    // -----------------------------------------------------------------------
    // 黑铁矿分支（1712-1724）
    // -----------------------------------------------------------------------

    [Fact]
    public void Sub4A0218_BlackStones_AreRemovedAndSummed()
    {
        var m = new TMerchant { m_sCharName = "NPC1" };
        SetBlackStone(10);
        NpcSeams.GetStdItem = _ => Std();

        var list = new List<object> { Item(10, dura: 3000, makeIndex: 1), Item(10, dura: 6000, makeIndex: 2), Item(10, dura: 1000, makeIndex: 3) };
        m.sub_4A0218(new TPlayObject(), list, out byte dc, out byte sc, out byte mc, out byte dura);

        Assert.Empty(list);                                  // 三条全被剔除
        // DuraList = [6,3,1]（降序后）→ nDura = 10, nItemCount = 3
        // btDura = Round(min(5,3) + min(5,3) * ((10/3)/5)) = Round(3 + 3*0.6667) = Round(5.0) = 5
        Assert.Equal(5, dura);
        Assert.Equal(0, dc);
        // DelItems 按**倒序**遍历拼接 → "黑铁矿/3/黑铁矿/2/黑铁矿/1/"（原文 1707 的 downto）
        Assert.Single(_sent);
        Assert.Contains("黑铁矿/3/黑铁矿/2/黑铁矿/1/", _sent[0]);
        Assert.Contains($"{Grobal2Const.RM_SENDDELITEMLIST}:3:", _sent[0]);
    }

    [Fact]
    public void Sub4A0218_BlackStone_NeedIdentifyWritesDisappearLog()
    {
        var m = new TMerchant { m_sCharName = "NPC2" };
        SetBlackStone(10);
        NpcSeams.GetStdItem = _ => Std(needIdentify: 1, name: "黑铁矿");

        var list = new List<object> { Item(10, dura: 1000, makeIndex: 42) };
        m.sub_4A0218(new TPlayObject(), list, out _, out _, out _, out _);

        Assert.Single(_logs);
        Assert.Equal($"{ObjNpcConst.LOG_ItemDisappear}/{ObjNpcConst.LOG_ActionNone}/黑铁矿/42/使用升级材料", _logs[0]);
    }

    [Fact]
    public void Sub4A0218_BlackStone_NoIdentifyWritesNoLog()
    {
        var m = new TMerchant();
        SetBlackStone(10);
        NpcSeams.GetStdItem = _ => Std(needIdentify: 0);
        var list = new List<object> { Item(10, dura: 1000) };
        m.sub_4A0218(new TPlayObject(), list, out _, out _, out _, out _);
        Assert.Empty(_logs);
    }

    [Fact]
    public void Sub4A0218_OnlyFirstFiveDurabilitiesCount()
    {
        var m = new TMerchant();
        SetBlackStone(10);
        NpcSeams.GetStdItem = _ => Std();
        var list = new List<object>();
        // Dura 1000..7000 → 四舍五入到 1..7；降序后取前 5 = 7+6+5+4+3 = 25
        for (int i = 1; i <= 7; i++)
            list.Add(Item(10, dura: (ushort)(i * 1000)));

        m.sub_4A0218(new TPlayObject(), list, out _, out _, out _, out byte dura);

        // btDura = Round(5 + 5 * ((25/5)/5)) = Round(5 + 5*1) = 10
        Assert.Equal(10, dura);
    }

    // -----------------------------------------------------------------------
    // 升级材料分支（1727-1800）
    // -----------------------------------------------------------------------

    [Fact]
    public void Sub4A0218_StdMode19Group_KeepsMaxAndRunnerUp()
    {
        var m = new TMerchant();
        NpcSeams.sBlackStone = "黑铁矿";
        NpcSeams.GetStdItemName = _ => "NOTBLACK";
        // 19/20/21 与 22/23 取 DC2+DC1；24/26 再 +1
        NpcSeams.GetStdItem = i => i switch
        {
            1 => Std(stdMode: 19, dc1: 10, dc2: 5),
            2 => Std(stdMode: 20, dc1: 20, dc2: 10),
            _ => Std(),
        };

        var list = new List<object> { Item(1), Item(2) };
        m.sub_4A0218(new TPlayObject(), list, out byte dc, out _, out _, out _);

        // 倒序：先 item2（nDc=30）→ nDcMin=30, nDcMax=0；再 item1（nDc=15）→ nDcMax=15
        // btDc = 30 div 5 + 15 div 3 = 6 + 5 = 11
        Assert.Equal(11, dc);
        Assert.Empty(list);
    }

    [Fact]
    public void Sub4A0218_StdMode24_AddsOneToEachAttribute()
    {
        var m = new TMerchant();
        NpcSeams.sBlackStone = "黑铁矿";
        NpcSeams.GetStdItemName = _ => "NOTBLACK";
        NpcSeams.GetStdItem = _ => Std(stdMode: 24, dc1: 4, dc2: 0);
        var list = new List<object> { Item(1) };
        m.sub_4A0218(new TPlayObject(), list, out byte dc, out _, out _, out _);
        // nDc = 0 + 4 + 1 = 5 → btDc = 5 div 5 + 0 div 3 = 1
        Assert.Equal(1, dc);
    }

    [Fact]
    public void Sub4A0218_StdMode26_AlsoAddsOne()
    {
        var m = new TMerchant();
        NpcSeams.sBlackStone = "黑铁矿";
        NpcSeams.GetStdItemName = _ => "NOTBLACK";
        NpcSeams.GetStdItem = _ => Std(stdMode: 26, sc1: 9, sc2: 0);
        var list = new List<object> { Item(1) };
        m.sub_4A0218(new TPlayObject(), list, out _, out byte sc, out _, out _);
        // nSc = 0 + 9 + 1 = 10 → btSc = 10 div 5 + 0 div 3 = 2
        Assert.Equal(2, sc);
    }

    [Fact]
    public void Sub4A0218_StdModeOutsideBothGroups_GivesZeroAttributes()
    {
        var m = new TMerchant();
        NpcSeams.sBlackStone = "黑铁矿";
        NpcSeams.GetStdItemName = _ => "NOTBLACK";
        // StdMode 5 → IsUseItem 为假 → 整项**不受影响、不删除**
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, dc1: 100);
        var list = new List<object> { Item(1) };
        m.sub_4A0218(new TPlayObject(), list, out byte dc, out _, out _, out _);
        Assert.Single(list);   // 未被剔除（IsUseItem false）
        Assert.Equal(0, dc);
    }

    [Fact]
    public void Sub4A0218_UseItem_GetItemAddValueSeamIsInvoked()
    {
        var m = new TMerchant();
        NpcSeams.sBlackStone = "黑铁矿";
        NpcSeams.GetStdItemName = _ => "NOTBLACK";
        NpcSeams.GetStdItem = _ => Std(stdMode: 19, dc1: 0, dc2: 0);
        bool called = false;
        NpcSeams.GetItemAddValue = (ref TUserItem ui, ref TStdItem std) =>
        {
            called = true;
            std.DC1 = 50;    // 加成由接缝注入
        };
        var list = new List<object> { Item(1) };
        m.sub_4A0218(new TPlayObject(), list, out byte dc, out _, out _, out _);
        Assert.True(called);
        // nDc = 0 + 50 = 50 → btDc = 50 div 5 + 0 div 3 = 10
        Assert.Equal(10, dc);
    }

    [Fact]
    public void Sub4A0218_UseItem_NeedIdentifyWritesDisappearLog()
    {
        var m = new TMerchant { m_sCharName = "NPC3" };
        NpcSeams.sBlackStone = "黑铁矿";
        NpcSeams.GetStdItemName = _ => "NOTBLACK";
        NpcSeams.GetStdItem = _ => Std(stdMode: 19, needIdentify: 1, name: "材料");
        var list = new List<object> { Item(1, makeIndex: 77) };
        m.sub_4A0218(new TPlayObject(), list, out _, out _, out _, out _);
        Assert.Single(_logs);
        Assert.Equal($"{ObjNpcConst.LOG_ItemDisappear}/{ObjNpcConst.LOG_ActionNone}/材料/77/使用升级材料", _logs[0]);
    }

    [Fact]
    public void Sub4A0218_UseItem_CustomNameUsedWhenBtValue13IsOne()
    {
        var m = new TMerchant();
        NpcSeams.sBlackStone = "黑铁矿";
        NpcSeams.GetStdItemName = _ => "NOTBLACK";
        NpcSeams.GetStdItem = _ => Std(stdMode: 19, name: "标准名");
        var custom = Item(1, makeIndex: 9);
        custom.NameStr = "自定义名";
        custom.SetBtValue(13, 1);
        var list = new List<object> { custom };

        m.sub_4A0218(new TPlayObject(), list, out _, out _, out _, out _);

        Assert.Single(_sent);
        Assert.Contains("自定义名/9/", _sent[0]);
    }

    [Fact]
    public void Sub4A0218_UseItem_FallsBackToStdItemName()
    {
        var m = new TMerchant();
        NpcSeams.sBlackStone = "黑铁矿";
        NpcSeams.GetStdItemName = _ => "NOTBLACK";
        NpcSeams.GetStdItem = _ => Std(stdMode: 19, name: "标准名");
        var noName = Item(1, makeIndex: 8);   // btValue[13] = 0 且 Name = ''
        var list = new List<object> { noName };

        m.sub_4A0218(new TPlayObject(), list, out _, out _, out _, out _);

        Assert.Single(_sent);
        Assert.Contains("标准名/8/", _sent[0]);
    }

    [Fact]
    public void Sub4A0218_IsUseItem_NoStdItemThrowsLikeOriginalNullDeref()
    {
        // 原文缺陷（M2Share.pas:11664-11665）：IsUseItem 不判 StdItem = nil，
        // 原文会 AV；托管侧 IsUseItem 默认实现读 Nullable.Value → InvalidOperationException。
        var m = new TMerchant();
        NpcSeams.sBlackStone = "黑铁矿";
        NpcSeams.GetStdItemName = _ => "NOTBLACK";
        NpcSeams.GetStdItem = _ => null;
        var list = new List<object> { Item(1) };
        Assert.Throws<InvalidOperationException>(() =>
            m.sub_4A0218(new TPlayObject(), list, out _, out _, out _, out _));
    }

    [Fact]
    public void Sub4A0218_MixedMaterials_AttributesAndDurabilityTogether()
    {
        var m = new TMerchant { m_sCharName = "MIX" };
        NpcSeams.sBlackStone = "黑铁矿";
        NpcSeams.GetStdItemName = i => i == 10 ? "黑铁矿" : "OTHER";
        NpcSeams.GetStdItem = i => i switch
        {
            10 => Std(stdMode: 0),
            1 => Std(stdMode: 19, dc1: 25, dc2: 5),
            _ => Std(),
        };

        var list = new List<object> { Item(10, dura: 5000, makeIndex: 1), Item(1, makeIndex: 2) };
        m.sub_4A0218(new TPlayObject(), list, out byte dc, out _, out _, out byte dura);

        Assert.Empty(list);
        // 黑铁矿 Dura 5000 → Round(5.0) = 5 → nDura = 5, nItemCount = 1
        // btDura = Round(1 + 1 * ((5/1)/5)) = Round(1 + 1) = 2
        Assert.Equal(2, dura);
        // 材料 DC = 5+25 = 30 → btDc = 30 div 5 + 0 div 3 = 6
        Assert.Equal(6, dc);
        Assert.Single(_sent);
    }
}

// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：`MakeDurg`（原文 2272-2304，制药列表）+ `nNF_MakedUrg` 分支（2747-2751）
// ★ 与 `BuyItem` 的"看起来一样实则不同"：MakeDurg **2287 只判 Count、2294 直接读 [0]**，
//   而 BuyItem **2110-2111 先判 nil** —— 前者对 nil 组**直接抛**（原文如此，照抄不补）。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcUserSelectMakeDurgTests : IDisposable
{
    private readonly List<string> _sent = new();

    public NpcObjNpcUserSelectMakeDurgTests()
    {
        NpcSeams.ResetDefaults();
        NpcSeams.SendTo = (sender, target, ident, wParam, p1, p2, p3, sMsg) =>
            _sent.Add($"{ident}|{wParam}|{p1}|{p2}|{p3}|{sMsg}");
    }

    public void Dispose() => NpcSeams.ResetDefaults();

    private static TMerchant M() => new() { m_sCharName = "商人", m_nRecogId = 9 };
    private static TPlayObject P() => new() { m_sCharName = "玩家" };

    private static TStdItem Std(string name)
    {
        var s = new TStdItem();
        s.NameStr = name;
        return s;
    }

    /// <summary>
    /// 造一个"商品组"。
    /// <para><b>★ 坑（`params` 族第 2 变体，勿踩）</b>：**不要写 `Group(null!)`** ——
    /// `params object[]` 收到裸 `null` 时会把**整个数组**当成 null（**不是**"含一个 null 元素的数组"），
    /// 于是 `m_GoodsList` 里放进的是 **null 组**，测试会走到**完全另一条分支** —— 而且**不报错、看起来正常**。
    /// 要造"含 null 元素的组"必须写 <c>Group((object?)null!)</c>。
    /// （第 1 变体见台账：**重载歧义** —— 单参调用被绑到多参重载，同样静默走错分支。）</para>
    /// </summary>
    private static List<object> Group(params object[] items) => new(items);

    // -----------------------------------------------------------------------
    // 2300：拼串（两个常量 0 / 1 照抄）
    // -----------------------------------------------------------------------

    [Fact]
    public void MakeDurg_OneGroupOneItem_BuildsNameSlashZeroSlashPriceSlashOneSlash()
    {
        var m = M();
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 5 }));
        NpcSeams.GetStdItem = _ => Std("金创药");
        int old = M2Config.nMakeDurgPrice;
        try
        {
            M2Config.nMakeDurgPrice = 777;
            m.MakeDurg(P());
            Assert.Equal($"{Grobal2Const.RM_USERMAKEDRUGITEMS}|0|9|0|0|金创药/0/777/1/", Assert.Single(_sent));
        }
        finally { M2Config.nMakeDurgPrice = old; }
    }

    [Fact]
    public void MakeDurg_TwoGroups_ConcatenatesInOrder()
    {
        var m = M();
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 1 }));
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 2 }));
        NpcSeams.GetStdItem = idx => Std(idx == 1 ? "甲" : "乙");
        m.MakeDurg(P());
        string msg = Assert.Single(_sent);
        Assert.EndsWith("甲/0/" + M2Config.nMakeDurgPrice + "/1/乙/0/" + M2Config.nMakeDurgPrice + "/1/", msg);
    }

    [Fact]
    public void MakeDurg_OnlyFirstItemOfEachGroupIsUsed()
    {
        // 2294 只看第 0 件 —— 组内后续物品**不参与**
        var m = M();
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 1 }, new TUserItem { wIndex = 2 }));
        NpcSeams.GetStdItem = idx => Std("W" + idx);
        m.MakeDurg(P());
        string msg = Assert.Single(_sent);
        Assert.Contains("W1/", msg);
        Assert.DoesNotContain("W2/", msg);
    }

    [Fact]
    public void MakeDurg_UnknownStdItem_IsSkippedButOthersRemain()
    {
        // 2298 `StdItem <> nil` —— nil 的组跳过拼串，但**循环继续**
        var m = M();
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 1 }));
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 2 }));
        NpcSeams.GetStdItem = idx => idx == 1 ? null : Std("乙");
        m.MakeDurg(P());
        string msg = Assert.Single(_sent);
        Assert.DoesNotContain("null", msg);
        Assert.Contains("乙/", msg);
    }

    [Fact]
    public void MakeDurg_EmptyStdItemName_StillAppendsItsEntry()
    {
        // 2298 只判 `<> nil`，**不判名字非空** ⇒ 空名也会拼出 `/0/.../1/`
        var m = M();
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 1 }));
        NpcSeams.GetStdItem = _ => Std("");
        m.MakeDurg(P());
        Assert.EndsWith("|/0/" + M2Config.nMakeDurgPrice + "/1/", Assert.Single(_sent));
    }

    [Fact]
    public void MakeDurg_NullFirstItem_IsSkipped()
    {
        // 2295 `if UserItem = nil then Continue`
        // ⚠ `Group(null)` 会命中 C# `params` 的"**整个数组为 null**"陷阱 ⇒ 必须写成 `(object?)null`
        var m = M();
        m.m_GoodsList.Add(Group((object?)null!));
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 1 }));
        NpcSeams.GetStdItem = _ => Std("甲");
        m.MakeDurg(P());
        Assert.EndsWith("|甲/0/" + M2Config.nMakeDurgPrice + "/1/", Assert.Single(_sent));
    }

    // -----------------------------------------------------------------------
    // 2303：串为空**不发包**
    // -----------------------------------------------------------------------

    [Fact]
    public void MakeDurg_EmptyGoodsList_SendsNothing()
    {
        var m = M();
        NpcSeams.GetStdItem = _ => Std("甲");
        m.MakeDurg(P());
        Assert.Empty(_sent);
    }

    [Fact]
    public void MakeDurg_AllStdItemsNil_SendsNothing()
    {
        // 串保持 '' ⇒ 不发包（与"发了空串"不同）
        var m = M();
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 1 }));
        NpcSeams.GetStdItem = _ => null;
        m.MakeDurg(P());
        Assert.Empty(_sent);
    }

    // -----------------------------------------------------------------------
    // 2287-2293：空组被删 + **goto 重扫**
    // -----------------------------------------------------------------------

    [Fact]
    public void MakeDurg_EmptyGroup_IsRemovedAndScanRestarts()
    {
        var m = M();
        m.m_GoodsList.Add(Group());                          // 空组（下标 0）
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 1 }));  // 有效组（下标 1）
        NpcSeams.GetStdItem = _ => Std("甲");
        m.MakeDurg(P());

        Assert.Single(m.m_GoodsList);                        // 空组已被删
        Assert.Equal(1, m.m_GoodsList.Count);
        Assert.EndsWith("|甲/0/" + M2Config.nMakeDurgPrice + "/1/", Assert.Single(_sent));
    }

    [Fact]
    public void MakeDurg_EmptyGroupAtTail_LeavesNoTrace()
    {
        var m = M();
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 1 }));
        m.m_GoodsList.Add(Group());                          // 尾部空组
        NpcSeams.GetStdItem = _ => Std("甲");
        m.MakeDurg(P());
        Assert.Single(m.m_GoodsList);
    }

    [Fact]
    public void MakeDurg_MultipleEmptyGroups_AllRemoved()
    {
        // goto 重扫 ⇒ 任意多个空组都会被逐个清掉
        var m = M();
        m.m_GoodsList.Add(Group());
        m.m_GoodsList.Add(Group());
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 1 }));
        m.m_GoodsList.Add(Group());
        NpcSeams.GetStdItem = _ => Std("甲");
        m.MakeDurg(P());
        Assert.Single(m.m_GoodsList);
    }

    [Fact]
    public void MakeDurg_AllGroupsEmpty_SendsNothingAndEmptiesList()
    {
        var m = M();
        m.m_GoodsList.Add(Group());
        m.m_GoodsList.Add(Group());
        m.MakeDurg(P());
        Assert.Empty(m.m_GoodsList);
        Assert.Empty(_sent);
    }

    // -----------------------------------------------------------------------
    // ★ 差异断言：与 BuyItem 的 nil 处理不同
    // -----------------------------------------------------------------------

    [Fact]
    public void MakeDurg_NullGroupEntry_Throws()
    {
        // ★★ 差异断言：`MakeDurg` **没有 nil 分支**（2287 只判 `Count`、2294 直接读 `[0]`）
        //    ⇒ 对 nil 组**直接抛**（原文如此，照抄不补）。
        var m = M();
        m.m_GoodsList.Add(null!);
        var ex = Record.Exception(() => m.MakeDurg(P()));
        Assert.NotNull(ex);                                  // NullReferenceException
    }

    [Fact]
    public void ClientBuyItem_NullGroupEntry_IsSkippedNotRemoved_ContrastWithMakeDurg()
    {
        // ★ 对照（**更正我先前的一处混淆**）：带"删除 nil 组 + goto 重扫"的是**嵌套过程 `BuyItem`**
        //   （ObjNpc.pas:2110-2116），而它**尚未移植**；
        //   `ClientBuyItem`(3400-3402) 对 nil 组只是 **`Continue`**（**不删**）—— 本用例锁后者。
        //   即：三者在 nil 处理上**三种不同**：`MakeDurg` 抛 / `ClientBuyItem` 跳过并保留 / `BuyItem` 删除并重扫。
        var m = M();
        m.m_GoodsList.Add(null!);
        NpcSeams.GetStdItem = _ => null;
        var ex = Record.Exception(() => m.ClientBuyItem(P(), "无", 1, 0, false));
        Assert.Null(ex);
        Assert.Single(m.m_GoodsList);                        // nil 组**仍在**（未被删除）
    }

    // -----------------------------------------------------------------------
    // 派发分支（2747-2751）
    // -----------------------------------------------------------------------

    [Fact]
    public void MakeDurgArm_FlagOn_Dispatches()
    {
        var m = M();
        m.m_boMakeDrug = true;
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 1 }));
        NpcSeams.GetStdItem = _ => Std("甲");
        Assert.True(m.UserSelectPortedArms(P(), NpcProcessCmd.nNF_MakedUrg));
        Assert.Single(_sent);
    }

    [Fact]
    public void MakeDurgArm_FlagOff_DoesNothing()
    {
        var m = M();
        m.m_boMakeDrug = false;
        m.m_GoodsList.Add(Group(new TUserItem { wIndex = 1 }));
        NpcSeams.GetStdItem = _ => Std("甲");
        Assert.True(m.UserSelectPortedArms(P(), NpcProcessCmd.nNF_MakedUrg));
        Assert.Empty(_sent);
    }

    [Fact]
    public void MakeDurg_CommandIdIsOriginal()
    {
        // NpcCommon.pas:42（注意原文拼写是 `nNF_MakedUrg`）
        Assert.Equal(17, NpcProcessCmd.nNF_MakedUrg);
    }

    // -----------------------------------------------------------------------
    // PlayDrink（原文 2508-2511）+ 其分支（2862-2866）
    // -----------------------------------------------------------------------

    [Fact]
    public void PlayDrink_SendsRmSendUserPlayDrinkWithSelfId()
    {
        var m = M();
        m.PlayDrink(P());
        Assert.Equal($"{Grobal2Const.RM_SENDUSERPLAYDRINK}|0|9|0|0|", Assert.Single(_sent));
    }

    [Fact]
    public void PlayDrinkArm_GuardOn_Dispatches()
    {
        var m = M();
        m.m_boPleaseDrink = true;
        Assert.True(m.UserSelectPortedArms(P(), NpcProcessCmd.nNF_PlayDrink));
        Assert.Single(_sent);
    }

    [Fact]
    public void PlayDrinkArm_GuardOff_DoesNothing()
    {
        var m = M();
        m.m_boPleaseDrink = false;
        Assert.True(m.UserSelectPortedArms(P(), NpcProcessCmd.nNF_PlayDrink));
        Assert.Empty(_sent);
    }

    [Fact]
    public void PlayDrinkArm_OtherFlagsDoNotLeakIn()
    {
        // ★ "名字相似"差异断言：命令号 `nNF_PlayDrink`、过程 `PlayDrink`、守卫 `m_boPleaseDrink`
        //   三者名字近似而**拼写各不相同**。把**其它所有**开关打开、只关 `m_boPleaseDrink`
        //   ⇒ 必须**不发包**（若谁"顺手"把守卫写成别的同类字段，本用例会失败）
        var m = M();
        m.m_boPleaseDrink = false;
        m.m_boS_repair = true;
        m.m_boRepair = true;
        m.m_boSell = true;
        m.m_boArmRemoveStone = true;
        m.m_boMakeDrug = true;
        m.m_boDealGold = true;
        m.m_boPrices = true;
        m.m_boStorage = true;
        m.m_boGetback = true;
        m.m_boBigStorage = true;
        m.m_boBigGetBack = true;
        m.m_boGetPreviousPage = true;
        m.m_boGetNextPage = true;
        m.m_boofflinemsg = true;

        Assert.True(m.UserSelectPortedArms(P(), NpcProcessCmd.nNF_PlayDrink));
        Assert.Empty(_sent);                       // 只认 m_boPleaseDrink
    }

    [Fact]
    public void PlayDrink_CommandIdIsOriginal()
    {
        // NpcCommon.pas:12 —— 排在 `nNF_ReclaimItem`(1) 之后，值 = 2
        Assert.Equal(2, NpcProcessCmd.nNF_PlayDrink);
    }

    [Fact]
    public void PurePacketSenders_AllUseDistinctPacketIds()
    {
        // ★ 五个纯发包过程的包号两两不同（防"名字相似"导致的互相抄）
        var ids = new[]
        {
            Grobal2Const.RM_SENDUSERSREPAIR,   // SuperRepairItem
            Grobal2Const.RM_SENDUSERREPAIR,    // RepairItem
            Grobal2Const.RM_SENDUSERSELL,      // SellItem
            Grobal2Const.RM_ARMREMOVESTONE,    // ArmRemoveStoneItem
            Grobal2Const.RM_SENDUSERPLAYDRINK, // PlayDrink
        };
        Assert.Equal(ids.Length, new HashSet<int>(ids).Count);
    }
}

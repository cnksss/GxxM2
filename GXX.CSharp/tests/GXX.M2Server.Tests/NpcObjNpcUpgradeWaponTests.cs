// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：TMerchant.UpgradeWapon 外层体（原文 **1830-1901，72 行**）
//           —— 嵌套过程 sub_4A0218 已在 NpcObjNpcSub4A0218Tests.cs 覆盖。
// 前置（第十轮核实；第十一轮 `m_UseItems` 口径统一为权威 `TUserItem?[]` 后改为**直填槽位**）：
//   ① `m_UseItems[U_WEAPON]` 读写 → 直接写 `p.m_UseItems[UseSlots.U_WEAPON]`（D35 契约：取出→改→写回）；
//   ② GotoLable 未移植 → PlayerSurfaceNpcSeams.GotoLable。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcUpgradeWaponTests : IDisposable
{
    private readonly List<string> _sent = new();
    private readonly List<string> _logs = new();
    private readonly List<string> _labels = new();
    private readonly List<string> _sysMsgs = new();

    /// <summary>用于观测 `RecalcAbilitys()` 是否被调用（原文 :1887）。</summary>
    private sealed class SpyPlayer : TPlayObject
    {
        public int RecalcCount;
        public override void RecalcAbilitys() => RecalcCount++;
    }

    public NpcObjNpcUpgradeWaponTests()
    {
        NpcSeams.ResetDefaults();
        OnlineMsgControl.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
        PlayerSurfaceItemSeams.ResetDefaults();
        PlayerSurfaceBaseSeams.ResetDefaults();
        NpcSeams.SendTo = (sender, target, ident, wParam, p1, p2, p3, sMsg) =>
            _sent.Add($"{ident}|{wParam}|{p1}|{p2}|{p3}|{sMsg}");
        NpcSeams.AddGameDataLog = (a1, a2, actor, item, makeIdx, target, d1, d2, desc) =>
            _logs.Add($"{a1}/{a2}/{item}/{makeIdx}/{d1}/{d2}/{desc}");
        NpcSeams.SysMsg = (t, msg, color, type) => _sysMsgs.Add($"A|{msg}|{color}|{type}");
        NpcSeams.SysMsgFB = (t, msg, f, b, type) => _sysMsgs.Add($"FB|{msg}|{f}|{b}|{type}");
        NpcSeams.MainOutMessage = _ => { };
        NpcSeams.SaveUpgradeWeaponRecord = (_, _) => { };
        // ★ 第十一轮：`SetUseItemsWeapon` 接缝已删除 → 清空武器格改为**直写权威槽位**，
        //   故 `_slotWrites` 不再由接缝填充；改为在用例里直接读 `p.m_UseItems[U_WEAPON]` 断言。
        PlayerSurfaceNpcSeams.GotoLable = (npc, player, label, ext) => { _labels.Add(label); return true; };
        PlayerSurfaceNpcSeams.MyGetTickCount = () => 12345u;
        PlayerSurfaceItemSeams.GetStdItemName = _ => NpcSeams.sBlackStone;
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 10;
    }

    public void Dispose()
    {
        NpcSeams.ResetDefaults();
        OnlineMsgControl.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
        PlayerSurfaceItemSeams.ResetDefaults();
        PlayerSurfaceBaseSeams.ResetDefaults();
    }

    private static TStdItem Std(byte stdMode = 5, byte needIdentify = 0, string name = "武器")
    {
        var s = new TStdItem { StdMode = stdMode, NeedIdentify = needIdentify };
        s.NameStr = name;
        return s;
    }

    private static TUserItem Weapon(ushort wIndex = 5, int makeIndex = 7)
        => new() { wIndex = wIndex, MakeIndex = makeIndex, Dura = 100, DuraMax = 100 };

    /// <summary>商人 + 玩家（金币充足、背包里有黑铁矿），按需覆写标准物品。</summary>
    private (TMerchant Merchant, SpyPlayer Player) Setup(TUserItem? weapon = null,
        TStdItem? std = null, uint gold = 10000, bool addBlackStone = true)
    {
        var m = new TMerchant { m_sScript = "NPC", m_sMapName = "0", m_sCharName = "商人" };
        var p = new SpyPlayer { m_sCharName = "玩家", m_nGold = gold };
        p.m_wAbil.MaxWeight = 30000;
        TStdItem s = std ?? Std();
        NpcSeams.GetStdItem = _ => s;
        // ★ 第十一轮：替身接缝已删 → 直填权威装备槽
        p.m_UseItems[UseSlots.U_WEAPON] = weapon ?? Weapon();
        if (addBlackStone)
            p.AddToBag(new TUserItem { wIndex = 99, MakeIndex = 1, Dura = 1000, DuraMax = 1000 });
        M2Config.nUpgradeWeaponPrice = 500;
        return (m, p);
    }

    // -----------------------------------------------------------------------
    // 早退分支（1839-1859）
    // -----------------------------------------------------------------------

    [Fact]
    public void UpgradeWapon_AlreadyUpgrading_GotoIngLabelAndExits()
    {
        var (m, p) = Setup();
        m.m_UpgradeWeaponList.Add(new TUpgradeInfo { sUserName = "玩家" });

        m.UpgradeWapon(p);

        Assert.Equal(NpcProcessCmd.sNF_Upgradeing, Assert.Single(_labels));
        Assert.Equal(10000u, p.m_nGold);          // 未扣费
        Assert.NotEqual((ushort)0, p.m_UseItems[UseSlots.U_WEAPON]!.Value.wIndex);   // 未清空武器格
    }

    [Fact]
    public void UpgradeWapon_OtherPlayerUpgrading_DoesNotBlock()
    {
        // 原文 1844：只比对 `sUserName = User.m_sCharName`；别人在升级不该拦住我
        var (m, p) = Setup();
        m.m_UpgradeWeaponList.Add(new TUpgradeInfo { sUserName = "别人" });

        m.UpgradeWapon(p);

        Assert.Equal(NpcProcessCmd.sNF_UpgradeOK, Assert.Single(_labels));
    }

    [Fact]
    public void UpgradeWapon_NullUpgradeInfoEntry_IsSkipped()
    {
        // 原文 1842-1843：`if UpgradeInfo = nil then Continue;`
        var (m, p) = Setup();
        m.m_UpgradeWeaponList.Add(null!);

        m.UpgradeWapon(p);

        Assert.Equal(NpcProcessCmd.sNF_UpgradeOK, Assert.Single(_labels));
    }

    [Fact]
    public void UpgradeWapon_NoWeapon_GotoFailLabel()
    {
        var (m, p) = Setup(weapon: new TUserItem { wIndex = 0 });
        m.UpgradeWapon(p);
        Assert.Equal(NpcProcessCmd.sNF_UpgradeFail, Assert.Single(_labels));
        Assert.Equal(10000u, p.m_nGold);
    }

    [Fact]
    public void UpgradeWapon_NotEnoughGold_GotoFailLabel()
    {
        var (m, p) = Setup(gold: 499);            // 499 < 500
        m.UpgradeWapon(p);
        Assert.Equal(NpcProcessCmd.sNF_UpgradeFail, Assert.Single(_labels));
    }

    [Fact]
    public void UpgradeWapon_ExactGold_IsAccepted()
    {
        // 原文 1850 用的是 `>=`
        var (m, p) = Setup(gold: 500);
        m.UpgradeWapon(p);
        Assert.Equal(NpcProcessCmd.sNF_UpgradeOK, Assert.Single(_labels));
        Assert.Equal(0u, p.m_nGold);
    }

    [Fact]
    public void UpgradeWapon_NoBlackStone_GotoFailLabel()
    {
        var (m, p) = Setup(addBlackStone: false);
        m.UpgradeWapon(p);
        Assert.Equal(NpcProcessCmd.sNF_UpgradeFail, Assert.Single(_labels));
        Assert.Equal(10000u, p.m_nGold);
    }

    [Fact]
    public void UpgradeWapon_ForbiddenByItemRule17_SysMsgReplacesPercentItemAndExitsWithoutGoto()
    {
        // ★ 原文 1854-1858：规则号是 **17**；命中后 **Exit**（**不经过** 1897-1900 的 GotoLable）
        var (m, p) = Setup(std: Std(name: "屠龙"));
        NpcSeams.GetItemRule = (idx, rule) => rule == 17 && idx == 5;

        m.UpgradeWapon(p);

        Assert.Empty(_labels);                    // 早退 → 一次都没跳
        Assert.Single(_sysMsgs);
        // `AnsiReplaceStr('你的武器[%Item]不允许升级', '%Item', '屠龙')`
        Assert.StartsWith("FB|你的武器[屠龙]不允许升级|", _sysMsgs[0]);
        Assert.Equal(10000u, p.m_nGold);          // 未扣费
    }

    [Fact]
    public void UpgradeWapon_ItemRuleOtherThan17_DoesNotBlock()
    {
        // 差异断言：规则号 4/8 都不该触发禁升级（原文写死 17）
        var (m, p) = Setup();
        NpcSeams.GetItemRule = (_, rule) => rule != 17;

        m.UpgradeWapon(p);

        Assert.Equal(NpcProcessCmd.sNF_UpgradeOK, Assert.Single(_labels));
        Assert.Empty(_sysMsgs);
    }

    [Fact]
    public void UpgradeWapon_EmptyStdItemName_DoesNotTriggerForbiddenBranch()
    {
        // 原文 1854 的第二个条件 `Length(StdItem.Name) > 0` 与之**与**在一起
        var (m, p) = Setup(std: Std(name: ""));
        NpcSeams.GetItemRule = (_, _) => true;

        m.UpgradeWapon(p);

        Assert.Equal(NpcProcessCmd.sNF_UpgradeOK, Assert.Single(_labels));   // 未被禁升级分支拦住
        Assert.Empty(_sysMsgs);
    }

    // -----------------------------------------------------------------------
    // 成功路径（1860-1895）
    // -----------------------------------------------------------------------

    [Fact]
    public void UpgradeWapon_Success_DeductsGoldClearsSlotAndRegistersUpgrade()
    {
        var weapon = Weapon(wIndex: 5, makeIndex: 7);
        var (m, p) = Setup(weapon: weapon);

        m.UpgradeWapon(p);

        Assert.Equal(9500u, p.m_nGold);                       // 1861 DecGold
        Assert.Equal(1, p.RecalcCount);                       // 1887 RecalcAbilitys
        // 1889：RM_ABILITY（⚠ 总数不止 1 —— 1900 的 `sub_4A0218` 会为被消耗的黑铁矿
        //   额外走 `SendDelItem`/日志路径，故此处只断言"**包含** RM_ABILITY"）
        Assert.Contains(_sent, s => s.StartsWith($"{Grobal2Const.RM_ABILITY}|0|0|0|0|"));
        // 1886：武器格被清空（★ D35：写回权威槽位，不是只改本地副本）
        Assert.NotNull(p.m_UseItems[UseSlots.U_WEAPON]);
        Assert.Equal((ushort)0, p.m_UseItems[UseSlots.U_WEAPON]!.Value.wIndex);
        // 1879-1880：升级记录保留了**清空之前**的整件武器
        var info = Assert.IsType<TUpgradeInfo>(Assert.Single(m.m_UpgradeWeaponList));
        Assert.Equal("玩家", info.sUserName);
        Assert.Equal((ushort)5, info.UserItem.wIndex);
        Assert.Equal(7, info.UserItem.MakeIndex);
        // 1891-1892
        Assert.True(info.dtTime > DateTime.MinValue);
        Assert.Equal(12345u, info.dwGetBackTick);
        // 出口标签
        Assert.Equal(NpcProcessCmd.sNF_UpgradeOK, Assert.Single(_labels));
    }

    [Fact]
    public void UpgradeWapon_Sub4A0218OutputsAreStoredOnUpgradeInfo()
    {
        // 1890 的 4 个 `out Byte` 必须落进 UpgradeInfo 的 btDc/btSc/btMc/btDura。
        // ⚠ `sub_4A0218` **会消耗**背包里的黑铁矿（它就是要删石头的那个过程），
        //   所以期望值必须先在**另一条等价玩家**上算出来，不能在原玩家上再算一遍。
        var (m0, p0) = Setup();
        m0.sub_4A0218(p0, p0.m_ItemList, out byte expDc, out byte expSc, out byte expMc, out byte expDura);

        var (m, p) = Setup();
        m.UpgradeWapon(p);
        var info = (TUpgradeInfo)m.m_UpgradeWeaponList[0];

        Assert.Equal(expDc, info.btDc);
        Assert.Equal(expSc, info.btSc);
        Assert.Equal(expMc, info.btMc);
        Assert.Equal(expDura, info.btDura);
    }

    [Fact]
    public void UpgradeWapon_GameLogGoldDisabled_WritesNoFeeLog()
    {
        var (m, p) = Setup();
        NpcSeams.g_boGameLogGold = false;
        m.UpgradeWapon(p);
        Assert.Empty(_logs);
    }

    [Fact]
    public void UpgradeWapon_GameLogGoldEnabled_WritesFeeLogWithOldAndNewGold()
    {
        // 原文 1862-1865：Data1 = 扣费后余额，Data2 = OldGold
        var (m, p) = Setup();
        NpcSeams.g_boGameLogGold = true;

        m.UpgradeWapon(p);

        Assert.Single(_logs.Where(l => l.Contains("扣费:")));
        Assert.Contains("14/50/金币/0/9500/10000/扣费:500", _logs);
    }

    [Fact]
    public void UpgradeWapon_NeedIdentify_WritesUseLog()
    {
        var (m, p) = Setup(std: Std(needIdentify: 1, name: "屠龙"));
        m.UpgradeWapon(p);

        // ⚠ `sub_4A0218` 会为被消耗的黑铁矿另写日志，故只断言"包含"本条目
        Assert.Contains("14/0/屠龙/7/0/0/使用升级武器", _logs);
        Assert.Single(_logs.Where(l => l.Contains("使用升级武器")));
    }

    [Fact]
    public void UpgradeWapon_NoNeedIdentify_WritesNoUseLog()
    {
        var (m, p) = Setup(std: Std(needIdentify: 0));
        m.UpgradeWapon(p);
        Assert.Empty(_logs);
    }

    [Fact]
    public void UpgradeWapon_CastleTax_ChargedUpgradeWeaponPriceOnCastle()
    {
        var (m, p) = Setup();
        m.m_boCastle = true;
        object castle = new object();
        NpcSeams.GetNpcCastle = _ => castle;
        object taxedOn = null;
        int taxed = 0;
        NpcSeams.IncRateGoldOnCastle = (c, g) => { taxedOn = c; taxed = g; };
        int managerTaxed = 0;
        NpcSeams.IncRateGoldOnCastleManager = g => managerTaxed = g;

        m.UpgradeWapon(p);

        Assert.Same(castle, taxedOn);
        Assert.Equal(500, taxed);          // ★ 此处传的就是 nUpgradeWeaponPrice（**不是** D33 缺陷）
        Assert.Equal(0, managerTaxed);
    }

    [Fact]
    public void UpgradeWapon_NoCastleButGetAllNpcTax_ManagerCharged()
    {
        var (m, p) = Setup();
        m.m_boCastle = true;
        NpcSeams.GetNpcCastle = _ => null;
        bool old = M2Config.boGetAllNpcTax;
        try
        {
            M2Config.boGetAllNpcTax = true;
            int managerTaxed = 0;
            NpcSeams.IncRateGoldOnCastleManager = g => managerTaxed = g;
            m.UpgradeWapon(p);
            Assert.Equal(500, managerTaxed);
        }
        finally
        {
            M2Config.boGetAllNpcTax = old;
        }
    }

    [Fact]
    public void UpgradeWapon_TaxBlockSkipped_WhenNeitherCastleNorAllNpcTax()
    {
        var (m, p) = Setup();
        m.m_boCastle = false;
        int taxed = -1;
        NpcSeams.IncRateGoldOnCastle = (_, g) => taxed = g;
        m.UpgradeWapon(p);
        Assert.Equal(-1, taxed);
    }

    [Fact]
    public void UpgradeWapon_SaveUpgradingListIsInvoked()
    {
        var (m, p) = Setup();
        var saved = new List<string>();
        NpcSeams.SaveUpgradeWeaponRecord = (key, list) => saved.Add(key);

        m.UpgradeWapon(p);

        Assert.Single(saved);
        Assert.Equal("NPC-0", saved[0]);   // m_sScript + '-' + m_sMapName
    }
}

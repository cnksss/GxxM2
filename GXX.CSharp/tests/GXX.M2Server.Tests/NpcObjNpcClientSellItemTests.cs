// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：TMerchant.ClientSellItem（原文 3798-3867）及其嵌套函数 sub_4A1C84（3800-3811）。
// —— "最小可验证闭环"：只依赖已落地的 `m_nGold`/`IncGold` + 本车道已覆盖的价格族。
// ============================================================================

using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcClientSellItemTests : System.IDisposable
{
    private readonly List<string> _sent = new();
    private readonly List<string> _logs = new();
    private readonly List<string> _sysMsgs = new();

    public NpcObjNpcClientSellItemTests()
    {
        NpcSeams.ResetDefaults();
        OnlineMsgControl.ResetDefaults();
        NpcSeams.SendTo = (sender, target, ident, wParam, p1, p2, p3, sMsg) =>
            _sent.Add($"{ident}|{wParam}|{p1}|{p2}|{p3}|{sMsg}");
        NpcSeams.AddGameDataLog = (a1, a2, actor, item, makeIdx, target, d1, d2, desc) =>
            _logs.Add($"{a1}/{a2}/{item}/{makeIdx}/{d1}/{d2}/{desc}");
        NpcSeams.SysMsg = (t, msg, color, type) => _sysMsgs.Add($"{msg}/{color}/{type}");
        NpcSeams.MainOutMessage = _ => { };
        NpcSeams.GetVariableText = (n, p, sMsg, sVar, nPos) => (false, sMsg, false);
    }

    public void Dispose()
    {
        NpcSeams.ResetDefaults();
        OnlineMsgControl.ResetDefaults();
    }

    private static TStdItem Std(byte stdMode = 5, ushort duraMax = 100, int price = 100, byte needIdentify = 0,
        ushort overLap = 0, string name = "物品")
    {
        var s = new TStdItem { StdMode = stdMode, DuraMax = duraMax, Price = price, NeedIdentify = needIdentify, OverLap = overLap };
        s.NameStr = name;
        return s;
    }

    private static TUserItem Item(ushort wIndex, ushort dura = 100, ushort duraMax = 100, int makeIndex = 1)
        => new() { wIndex = wIndex, Dura = dura, DuraMax = duraMax, MakeIndex = makeIndex };

    private static TPlayObject Player(uint gold = 1000) => new() { m_sCharName = "买家", m_nGold = gold };

    private static TMerchant NewMerchant(int price = 100)
    {
        var m = new TMerchant { m_sScript = "NPC", m_sMapName = "0", m_sCharName = "商人" };
        m.AddItemPrice(5, price);
        return m;
    }

    private static void Fail0()
    {
        // 默认：不禁止卖、不算规则禁售
        NpcSeams.GetUserItemBindValue = (_, _) => false;
        NpcSeams.GetItemRule = (_, _) => false;
    }

    // -----------------------------------------------------------------------
    // sub_4A1C84（3800-3811）
    // -----------------------------------------------------------------------

    [Fact]
    public void Sub4A1C84_NonSpecialStdMode_AlwaysTrue()
    {
        var m = NewMerchant();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5);
        Assert.True(m.sub_4A1C84(Item(5, dura: 0)));   // StdMode 不是 25/30 → 不看耐久
    }

    [Fact]
    public void Sub4A1C84_NullStdItem_ReturnsTrue()
    {
        var m = NewMerchant();
        NpcSeams.GetStdItem = _ => null;
        Assert.True(m.sub_4A1C84(Item(5, dura: 0)));
    }

    [Theory]
    [InlineData(25)]
    [InlineData(30)]
    public void Sub4A1C84_SpecialStdMode_RequiresDuraAtLeast4000(byte stdMode)
    {
        var m = NewMerchant();
        NpcSeams.GetStdItem = _ => Std(stdMode: stdMode);
        Assert.True(m.sub_4A1C84(Item(5, dura: 4000)));    // 边界：4000 允许（原文 `< 4000` 才 False）
        Assert.False(m.sub_4A1C84(Item(5, dura: 3999)));
        Assert.False(m.sub_4A1C84(Item(5, dura: 0)));
    }

    // -----------------------------------------------------------------------
    // ClientSellItem 门槛（3798-3829）
    // -----------------------------------------------------------------------

    [Fact]
    public void ClientSellItem_SellDisabled_ReturnsFalseSilently()
    {
        var m = NewMerchant();
        var item = Item(5);
        OnlineMsgControl.g_OnlineMsgControl.boDisableSell = true;
        Assert.False(m.ClientSellItem(Player(), ref item, false));
        Assert.Empty(_sent);
        Assert.Empty(_sysMsgs);
    }

    [Fact]
    public void ClientSellItem_BindNoSellAndIsBind_RejectedWithFailAndMessageBox()
    {
        var m = NewMerchant();
        var item = Item(5);
        item.boIsBind = 1;
        // 绑定选项第 4 位（ubNoSell）= 1
        item.btBindOption = (byte)(1 << ObjNpcConst.ubNoSell);
        Fail0();
        NpcSeams.GetUserItemBindValue = (opt, bit) => (opt & (1 << bit)) != 0;
        NpcSeams.g_sCanotUserSellItem = "禁售";

        Assert.False(m.ClientSellItem(Player(), ref item, false));
        // 原文 3825 发 FAIL(nParam1=0) + 3826 MessageBox（RM_MENU_OK）
        Assert.Equal(2, _sent.Count);
        Assert.StartsWith($"{Grobal2Const.RM_USERSELLITEM_FAIL}|0|0|0|0|", _sent[0]);
        Assert.StartsWith($"{Grobal2Const.RM_MENU_OK}|", _sent[1]);
        Assert.EndsWith("|禁售", _sent[1]);
    }

    [Fact]
    public void ClientSellItem_BindButNotIsBind_IsNotRejected()
    {
        var m = NewMerchant();
        var item = Item(5);
        item.boIsBind = 0;                                   // 未绑定 → 门槛的前半为假
        item.btBindOption = (byte)(1 << ObjNpcConst.ubNoSell);
        Fail0();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, needIdentify: 0);
        Assert.True(m.ClientSellItem(Player(), ref item, false));
    }

    [Fact]
    public void ClientSellItem_ItemRuleFourBlocked_Rejected()
    {
        var m = NewMerchant();
        var item = Item(5);
        Fail0();
        NpcSeams.GetItemRule = (idx, rule) => rule == 4 && idx == 5;
        Assert.False(m.ClientSellItem(Player(), ref item, false));
        Assert.NotEmpty(_sent);
    }

    [Fact]
    public void ClientSellItem_TradingDlg_SuppressesAllFailPackets()
    {
        var m = NewMerchant();
        var item = Item(5);
        Fail0();
        NpcSeams.GetItemRule = (_, _) => true;   // 禁售
        Assert.False(m.ClientSellItem(Player(), ref item, true));
        Assert.Empty(_sent);                     // IsFromTradingDlg = true → 不发包
    }

    [Fact]
    public void ClientSellItem_PriceNotPositive_SendsFailWithZero()
    {
        var m = new TMerchant();                 // 无价目表 + 无标准物品 → GetItemPrice = -1
        var item = Item(5);
        Fail0();
        NpcSeams.GetStdItem = _ => null;
        Assert.False(m.ClientSellItem(Player(), ref item, false));
        Assert.Single(_sent);
        Assert.StartsWith($"{Grobal2Const.RM_USERSELLITEM_FAIL}|0|0|0|0|", _sent[0]);
    }

    [Fact]
    public void ClientSellItem_Bo574BlocksSale()
    {
        var m = NewMerchant();
        m.bo574 = true;                           // 原文 3831 的 `not bo574`
        var item = Item(5);
        Fail0();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, needIdentify: 0);
        Assert.False(m.ClientSellItem(Player(), ref item, false));
        Assert.StartsWith($"{Grobal2Const.RM_USERSELLITEM_FAIL}|0|0|0|0|", _sent[0]);
    }

    [Fact]
    public void ClientSellItem_Sub4A1C84Fails_SendsFailWithZero()
    {
        var m = NewMerchant();
        var item = Item(5, dura: 100);
        Fail0();
        NpcSeams.GetStdItem = _ => Std(stdMode: 25);   // 25 且 dura < 4000 → 不许卖
        Assert.False(m.ClientSellItem(Player(), ref item, false));
        Assert.StartsWith($"{Grobal2Const.RM_USERSELLITEM_FAIL}|0|0|0|0|", _sent[0]);
    }

    // -----------------------------------------------------------------------
    // ClientSellItem 成功路径（3831-3860）
    // -----------------------------------------------------------------------

    [Fact]
    public void ClientSellItem_Success_GivesGoldSendsOkAndStoresItem()
    {
        var m = NewMerchant(price: 100);
        var item = Item(5, dura: 100, duraMax: 100, makeIndex: 77);
        var player = Player(gold: 1000);
        Fail0();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100, needIdentify: 0);

        Assert.True(m.ClientSellItem(player, ref item, false));

        // 售价：GetUserItemPrice(100, sellToNpc=true) → 非叠加、加成 0：
        //   n10 = 100 → n10 = Round(100/100*100) = 100；n20 = 100/2/100*(100-100) = 0 → 100
        // GetSellItemPrice(100) = Round(50.0) = 50
        Assert.Equal(1050u, player.m_nGold);
        Assert.Single(_sent);
        Assert.StartsWith($"{Grobal2Const.RM_USERSELLITEM_OK}|0|1050|0|0|", _sent[0]);
        // 物品进了商品列表
        Assert.Single(m.m_GoodsList);
    }

    [Fact]
    public void ClientSellItem_TradingDlgOnSuccess_SendsNoOkPacket()
    {
        var m = NewMerchant(price: 100);
        var item = Item(5);
        var player = Player(gold: 1000);
        Fail0();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100, needIdentify: 0);

        Assert.True(m.ClientSellItem(player, ref item, true));

        Assert.Empty(_sent);            // 原文 3850：只有非交易对话框才回 OK
        Assert.Equal(1050u, player.m_nGold);
        Assert.Single(m.m_GoodsList);   // 但入商品列表照做
    }

    [Fact]
    public void ClientSellItem_NeedIdentify_WritesSellLog()
    {
        var m = NewMerchant(price: 100);
        var item = Item(5, makeIndex: 9);
        Fail0();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100, needIdentify: 1, name: "屠龙");

        Assert.True(m.ClientSellItem(Player(gold: 1000), ref item, false));

        Assert.Single(_logs);
        // LOG_ItemSell=10 / LOG_GoldChange=50 / 金币=50 / Data2=nPrice=50
        Assert.Equal($"10/50/屠龙/9/1050/50/NPC卖出 [金币]", _logs[0]);
    }

    [Fact]
    public void ClientSellItem_NoNeedIdentify_WritesNoLog()
    {
        var m = NewMerchant(price: 100);
        var item = Item(5);
        Fail0();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100, needIdentify: 0);
        Assert.True(m.ClientSellItem(Player(), ref item, false));
        Assert.Empty(_logs);
    }

    [Fact]
    public void ClientSellItem_GoldOverflow_IncGoldFailsSendsFailWithMinusOne()
    {
        var m = NewMerchant(price: 100);
        var item = Item(5);
        Fail0();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100, needIdentify: 0);
        // 把金币顶到上限附近，使 IncGold 失败
        var player = Player(gold: 0);
        player.m_nGold = player.m_nGoldMax;   // IncGold 会失败（超出上限）

        Assert.False(m.ClientSellItem(player, ref item, false));

        Assert.Single(_sent);
        // 原文 3863：IncGold 失败 → FAIL(nParam1 = -1) —— 与"价格不合理"的 FAIL(0) **不同**
        Assert.StartsWith($"{Grobal2Const.RM_USERSELLITEM_FAIL}|0|-1|0|0|", _sent[0]);
    }

    [Fact]
    public void ClientSellItem_CastleTax_ChargedOnCastleInstance()
    {
        var m = NewMerchant(price: 100);
        m.m_boCastle = true;
        var item = Item(5);
        Fail0();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100, needIdentify: 0);
        object castle = new object();
        NpcSeams.GetNpcCastle = _ => castle;
        int taxed = 0;
        object taxedOn = null;
        NpcSeams.IncRateGoldOnCastle = (c, g) => { taxedOn = c; taxed = g; };
        int managerTaxed = 0;
        NpcSeams.IncRateGoldOnCastleManager = g => managerTaxed = g;

        Assert.True(m.ClientSellItem(Player(), ref item, false));

        Assert.Same(castle, taxedOn);
        Assert.Equal(50, taxed);        // 售价 = 50
        Assert.Equal(0, managerTaxed);  // 有城堡时**不**走管理器
    }

    [Fact]
    public void ClientSellItem_NoCastleButGetAllNpcTax_ManagerChargedUpgradeWeaponPrice()
    {
        // ★ D33 差异断言：原文 3847 传的是 `g_Config.nUpgradeWeaponPrice`（**不是** nPrice）
        var m = NewMerchant(price: 100);
        m.m_boCastle = true;                       // 进入税收块
        var item = Item(5);
        Fail0();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100, needIdentify: 0);
        NpcSeams.GetNpcCastle = _ => null;         // m_Castle = nil

        bool old = M2Config.boGetAllNpcTax;
        int oldPrice = M2Config.nUpgradeWeaponPrice;
        try
        {
            M2Config.boGetAllNpcTax = true;
            M2Config.nUpgradeWeaponPrice = 12345;
            int managerTaxed = 0;
            NpcSeams.IncRateGoldOnCastleManager = g => managerTaxed = g;

            Assert.True(m.ClientSellItem(Player(), ref item, false));

            Assert.Equal(12345, managerTaxed);     // 而不是 50（售价）
        }
        finally
        {
            M2Config.boGetAllNpcTax = old;
            M2Config.nUpgradeWeaponPrice = oldPrice;
        }
    }

    [Fact]
    public void ClientSellItem_CastleTaxDisabled_NoTax()
    {
        var m = NewMerchant(price: 100);
        m.m_boCastle = false;
        var item = Item(5);
        Fail0();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100, needIdentify: 0);
        int taxed = 0;
        NpcSeams.IncRateGoldOnCastle = (_, g) => taxed = g;
        Assert.True(m.ClientSellItem(Player(), ref item, false));
        Assert.Equal(0, taxed);
    }

    [Fact]
    public void ClientSellItem_StdMode43_WritesBackDuraMaxThroughRef()
    {
        // 证 `ref TUserItem` 是必需的：`GetUserItemPrice` 在 StdMode 43 时会把 DuraMax 顶到 10000
        var m = NewMerchant(price: 100);
        m.AddItemPrice(6, 100);
        var item = Item(6, dura: 5000, duraMax: 5000);
        Fail0();
        NpcSeams.GetStdItem = _ => Std(stdMode: 43, duraMax: 100, needIdentify: 0);

        Assert.True(m.ClientSellItem(Player(gold: 0), ref item, false));

        Assert.Equal((ushort)10000, item.DuraMax);
    }
}

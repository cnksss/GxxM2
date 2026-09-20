// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：GXX.M2Server.Npc 切片 7
//   · TMerchant.GetVariableText        原文 3234-3270（覆写 + 虚分派链）
//   · TMerchant.AddItemToGoodsList     原文 3869-3892
//   · TMerchant.ClearScript            原文 4164-4194
//   · TMerchant.ClearData              原文 4241-4280
//   · TMerchant.SendCustemMsg          原文 4235-4238
//   · TMerchant.LoadNPCData/SaveNPCData 原文 3052-3069
//   · TMerchant.LoadUpgradeList        原文 4196-4211
//   · TMerchant.SaveUpgradingList      原文 1674-1682
//   · TMerchant.LoadNpcScript/LoadNpcIconFile 原文 3180-3226
//   · TNormNpc.LoadNpcScript/LoadNpcIconFile  原文 9575-9602
//   · TBoxMonster.Create/Operate/Run   原文 10510-10543
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

// 见 NpcObjNpcTests.cs：Engine.TNormNpc 与本车道 Npc.TNormNpc 同名，文件级别名消歧。
using TNormNpc = GXX.M2Server.Npc.TNormNpc;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcMerchant2Tests : IDisposable
{
    private readonly List<string> _messages = new();

    public NpcObjNpcMerchant2Tests()
    {
        NpcSeams.ResetDefaults();
        NpcSeams.MainOutMessage = m => _messages.Add(m);
    }

    public void Dispose() => NpcSeams.ResetDefaults();

    private static TStdItem Std(byte stdMode = 5, ushort duraMax = 100, int price = 100, ushort overLap = 0)
        => new() { StdMode = stdMode, DuraMax = duraMax, Price = price, OverLap = overLap };

    private static TUserItem Item(ushort wIndex, ushort dura = 0, ushort duraMax = 0)
        => new() { wIndex = wIndex, Dura = dura, DuraMax = duraMax };

    private static TMerchant NewMerchant(string script = "TESTNPC", string map = "0")
        => new() { m_sScript = script, m_sMapName = map };

    // -----------------------------------------------------------------------
    // TMerchant.GetVariableText（3234-3270）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetVariableText_PriceRate_ReplacesPlaceholder()
    {
        var m = NewMerchant();
        m.m_nPriceRate = 250;
        string sMsg = "RATE=<$PRICERATE>!";
        bool brk = false;
        Assert.True(m.GetVariableText(new TPlayObject(), ref sMsg, "$PRICERATE", ref brk, 6));
        Assert.Equal("RATE=250!", sMsg);
    }

    [Fact]
    public void GetVariableText_VariableNameIsUppercasedFirst()
    {
        var m = NewMerchant();
        m.m_nPriceRate = 7;
        string sMsg = "<$pricerate>";
        bool brk = false;
        Assert.True(m.GetVariableText(new TPlayObject(), ref sMsg, "$pricerate", ref brk, 1));
        Assert.Equal("7", sMsg);
    }

    [Fact]
    public void GetVariableText_UpgradeWeaponFee_UsesM2Config()
    {
        var m = NewMerchant();
        int old = M2Config.nUpgradeWeaponPrice;
        try
        {
            M2Config.nUpgradeWeaponPrice = 12345;
            string sMsg = "<$UPGRADEWEAPONFEE>";
            bool brk = false;
            Assert.True(m.GetVariableText(new TPlayObject(), ref sMsg, "$UPGRADEWEAPONFEE", ref brk, 1));
            Assert.Equal("12345", sMsg);
        }
        finally
        {
            M2Config.nUpgradeWeaponPrice = old;
        }
    }

    [Fact]
    public void GetVariableText_UserWeapon_EmptySlotYieldsLiteral()
    {
        var m = NewMerchant();
        NpcSeams.GetUseItemsWeapon = _ => Item(0);
        string sMsg = "<$USERWEAPON>";
        bool brk = false;
        Assert.True(m.GetVariableText(new TPlayObject(), ref sMsg, "$USERWEAPON", ref brk, 1));
        Assert.Equal("无", sMsg);
    }

    [Fact]
    public void GetVariableText_UserWeapon_EquippedLooksUpStdItemName()
    {
        var m = NewMerchant();
        NpcSeams.GetUseItemsWeapon = _ => Item(77);
        NpcSeams.GetStdItemName = idx => idx == 77 ? "屠龙" : "";
        string sMsg = "<$USERWEAPON>";
        bool brk = false;
        Assert.True(m.GetVariableText(new TPlayObject(), ref sMsg, "$USERWEAPON", ref brk, 1));
        Assert.Equal("屠龙", sMsg);
    }

    [Fact]
    public void GetVariableText_UnknownVariable_ReturnsFalseAndLeavesMessage()
    {
        var m = NewMerchant();
        string sMsg = "<$NOPE>";
        bool brk = false;
        Assert.False(m.GetVariableText(new TPlayObject(), ref sMsg, "$NOPE", ref brk, 1));
        Assert.Equal("<$NOPE>", sMsg);
    }

    [Fact]
    public void GetVariableText_BaseHandled_SkipsMerchantBranches()
    {
        var m = NewMerchant();
        m.m_nPriceRate = 999;
        NpcSeams.GetVariableText = (n, p, sMsgIn, sVar, nPos) => (true, "BASEDONE", false);
        string sMsg = "<$PRICERATE>";
        bool brk = false;
        Assert.True(m.GetVariableText(new TPlayObject(), ref sMsg, "$PRICERATE", ref brk, 1));
        Assert.Equal("BASEDONE", sMsg);   // 未被商人分支改写
    }

    [Fact]
    public void GetLineVariableText_DispatchesVirtuallyToMerchantOverride()
    {
        // 关键：GetLineVariableText 对 GetVariableText 是**虚调用** →
        // TMerchant 的覆写必须生效（原文 6000 的调用点）。
        var m = NewMerchant();
        m.m_nPriceRate = 100;
        bool brk = false;
        Assert.Equal("P=100", m.GetLineVariableText(new TPlayObject(), "P=<$PRICERATE>", ref brk));
    }

    // -----------------------------------------------------------------------
    // TMerchant.AddItemToGoodsList（3869-3892）
    // -----------------------------------------------------------------------

    [Fact]
    public void AddItemToGoodsList_PositiveDura_CreatesNewGroup()
    {
        var m = NewMerchant();
        Assert.True(m.AddItemToGoodsList(Item(5, dura: 10)));
        Assert.Single(m.m_GoodsList);
        var group = (List<object>)m.m_GoodsList[0];
        Assert.Single(group);
        Assert.Equal((ushort)5, ((TUserItem)group[0]).wIndex);
    }

    [Fact]
    public void AddItemToGoodsList_ZeroDura_NoStdItem_Rejected()
    {
        var m = NewMerchant();
        NpcSeams.GetStdItem = _ => null;
        Assert.False(m.AddItemToGoodsList(Item(5, dura: 0)));
        Assert.Empty(m.m_GoodsList);
    }

    [Fact]
    public void AddItemToGoodsList_ZeroDura_NonOverlapNonAllowedStdMode_Rejected()
    {
        var m = NewMerchant();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100, overLap: 0);
        Assert.False(m.AddItemToGoodsList(Item(5, dura: 0)));
        Assert.Empty(m.m_GoodsList);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    public void AddItemToGoodsList_ZeroDura_StdModeZeroOneThree_Allowed(byte stdMode)
    {
        var m = NewMerchant();
        NpcSeams.GetStdItem = _ => Std(stdMode: stdMode, duraMax: 100, overLap: 0);
        Assert.True(m.AddItemToGoodsList(Item(5, dura: 0)));
        Assert.Single(m.m_GoodsList);
    }

    [Fact]
    public void AddItemToGoodsList_ZeroDura_OverlapItem_Allowed()
    {
        var m = NewMerchant();
        // OverLap > 0 且 StdMode ∈ {0,2,3,31,40,41,42,46,47} 且 DuraMax > 1 → 可叠加
        NpcSeams.GetStdItem = _ => Std(stdMode: 2, duraMax: 100, overLap: 1);
        Assert.True(m.AddItemToGoodsList(Item(5, dura: 0)));
    }

    [Fact]
    public void AddItemToGoodsList_ExistingGroup_InsertsAtFront()
    {
        var m = NewMerchant();
        var group = new List<object> { Item(5) };
        m.m_GoodsList.Add(group);
        Assert.True(m.AddItemToGoodsList(Item(5, dura: 1)));
        Assert.Single(m.m_GoodsList);      // 复用已有组，不新建
        Assert.Equal(2, group.Count);
        Assert.Equal((ushort)1, ((TUserItem)group[0]).Dura);   // 新元素在组首
    }

    // -----------------------------------------------------------------------
    // TMerchant.ClearScript（4164-4194）
    // -----------------------------------------------------------------------

    [Fact]
    public void ClearScript_ResetsAllTwentySevenSwitchesAndClearsScripts()
    {
        var m = NewMerchant();
        m.m_boBuy = m.m_boSell = m.m_boMakeDrug = m.m_boPrices = true;
        m.m_boStorage = m.m_boGetback = m.m_boBigStorage = m.m_boBigGetBack = true;
        m.m_boGetNextPage = m.m_boGetPreviousPage = m.m_boUpgradenow = m.m_boGetBackupgnow = true;
        m.m_boRepair = m.m_boS_repair = m.m_boGetMarry = m.m_boGetMaster = true;
        m.m_boUseItemName = m.m_boCreateHeroName = m.m_boGetSellGold = true;
        m.m_boSellOff = m.m_boBuyOff = m.m_boofflinemsg = m.m_boDealGold = true;
        m.m_boPleaseDrink = m.m_boMakeWine = m.m_boBuHero = m.m_boReclaimItem = true;
        m.m_ScriptList.Add(new TScript());

        m.ClearScript();

        Assert.False(m.m_boBuy);
        Assert.False(m.m_boSell);
        Assert.False(m.m_boMakeDrug);
        Assert.False(m.m_boPrices);
        Assert.False(m.m_boStorage);
        Assert.False(m.m_boGetback);
        Assert.False(m.m_boBigStorage);
        Assert.False(m.m_boBigGetBack);
        Assert.False(m.m_boGetNextPage);
        Assert.False(m.m_boGetPreviousPage);
        Assert.False(m.m_boUpgradenow);
        Assert.False(m.m_boGetBackupgnow);
        Assert.False(m.m_boRepair);
        Assert.False(m.m_boS_repair);
        Assert.False(m.m_boGetMarry);
        Assert.False(m.m_boGetMaster);
        Assert.False(m.m_boUseItemName);
        Assert.False(m.m_boCreateHeroName);
        Assert.False(m.m_boGetSellGold);
        Assert.False(m.m_boSellOff);
        Assert.False(m.m_boBuyOff);
        Assert.False(m.m_boofflinemsg);
        Assert.False(m.m_boDealGold);
        Assert.False(m.m_boPleaseDrink);
        Assert.False(m.m_boMakeWine);
        Assert.False(m.m_boBuHero);
        Assert.False(m.m_boReclaimItem);
        // inherited → TNormNpc.ClearScript 已 1:1 实现，会清空 m_ScriptList
        Assert.Empty(m.m_ScriptList);
    }

    // -----------------------------------------------------------------------
    // TMerchant.SendCustemMsg（4235-4238）
    // -----------------------------------------------------------------------

    [Fact]
    public void SendCustemMsg_ForwardsToBaseImplementation()
    {
        // 原文 4235-4238 只有 `inherited;` → 落到 TNormNpc.SendCustemMsg 的真实现（切片 15 起）。
        var m = NewMerchant();
        NpcSeams.boSendCustemMsg = false;
        NpcSeams.g_sSendCustMsgCanNotUseNowMsg = "关";
        var sys = new System.Collections.Generic.List<string>();
        NpcSeams.SysMsg = (t, msg, color, type) => sys.Add($"{msg}/{color}/{type}");
        m.SendCustemMsg(new TPlayObject(), "HELLO");
        Assert.Single(sys);
        Assert.StartsWith("关/", sys[0]);
    }

    // -----------------------------------------------------------------------
    // TMerchant.LoadNPCData（3052-3060）/ SaveNPCData（3062-3069）
    // -----------------------------------------------------------------------

    [Fact]
    public void LoadNPCData_UsesScriptDashMapFileAndOrder()
    {
        var m = NewMerchant("NPCX", "3");
        var calls = new List<string>();
        NpcSeams.LoadGoodRecord = (npc, f) => { Assert.Same(m, npc); calls.Add("good:" + f); };
        NpcSeams.LoadGoodPriceRecord = (npc, f) => calls.Add("price:" + f);
        NpcSeams.LoadUpgradeWeaponRecord = (f, list) => { calls.Add("upg:" + f); Assert.Same(m.m_UpgradeWeaponList, list); };

        m.LoadNPCData();

        Assert.Equal(new[] { "good:NPCX-3", "price:NPCX-3", "upg:NPCX-3" }, calls);
    }

    [Fact]
    public void SaveNPCData_UsesScriptDashMapFileAndOrder()
    {
        var m = NewMerchant("NPCX", "3");
        var calls = new List<string>();
        NpcSeams.SaveGoodRecord = (npc, f) => calls.Add("good:" + f);
        NpcSeams.SaveGoodPriceRecord = (npc, f) => calls.Add("price:" + f);

        m.SaveNPCData();

        Assert.Equal(new[] { "good:NPCX-3", "price:NPCX-3" }, calls);
    }

    // -----------------------------------------------------------------------
    // TMerchant.LoadUpgradeList（4196-4211）/ SaveUpgradingList（1674-1682）
    // -----------------------------------------------------------------------

    [Fact]
    public void LoadUpgradeList_ClearsThenLoads()
    {
        var m = NewMerchant();
        m.m_UpgradeWeaponList.Add(new TUpgradeInfo { sUserName = "OLD" });
        string seenFile = null;
        NpcSeams.LoadUpgradeWeaponRecord = (f, list) => { seenFile = f; Assert.Empty(list); };

        m.LoadUpgradeList();

        Assert.Equal("TESTNPC-0", seenFile);
        Assert.Empty(m.m_UpgradeWeaponList);
    }

    [Fact]
    public void LoadUpgradeList_ExceptionIsSwallowedAndReported()
    {
        var m = NewMerchant("NPCY");
        NpcSeams.LoadUpgradeWeaponRecord = (_, _) => throw new InvalidOperationException("boom");

        m.LoadUpgradeList();   // 不得抛出

        Assert.Single(_messages);
        Assert.Equal("Failure in loading upgradinglist - " + m.m_sCharName, _messages[0]);
    }

    [Fact]
    public void SaveUpgradingList_PassesScriptDashMapKey()
    {
        var m = NewMerchant("NPCZ", "9");
        string seenFile = null;
        NpcSeams.SaveUpgradeWeaponRecord = (f, list) => { seenFile = f; Assert.Same(m.m_UpgradeWeaponList, list); };
        m.SaveUpgradingList();
        Assert.Equal("NPCZ-9", seenFile);
    }

    [Fact]
    public void SaveUpgradingList_ExceptionIsSwallowedAndReported()
    {
        var m = NewMerchant("NPCZ");
        NpcSeams.SaveUpgradeWeaponRecord = (_, _) => throw new InvalidOperationException("boom");
        m.SaveUpgradingList();
        Assert.Single(_messages);
        Assert.StartsWith("Failure in saving upgradinglist - ", _messages[0]);
    }

    // -----------------------------------------------------------------------
    // TMerchant.ClearData（4241-4280）
    // -----------------------------------------------------------------------

    [Fact]
    public void ClearData_EmptiesGoodsAndPriceListsThenSavesNpcData()
    {
        var m = NewMerchant();
        m.m_GoodsList.Add(new List<object> { Item(1) });
        m.m_ItemPriceList.Add(new TItemPrice { wIndex = 1, nPrice = 100 });
        var saved = new List<string>();
        NpcSeams.SaveGoodRecord = (_, f) => saved.Add("good:" + f);
        NpcSeams.SaveGoodPriceRecord = (_, f) => saved.Add("price:" + f);

        m.ClearData();

        Assert.Empty(m.m_GoodsList);
        Assert.Empty(m.m_ItemPriceList);
        Assert.Equal(new[] { "good:TESTNPC-0", "price:TESTNPC-0" }, saved);
    }

    [Fact]
    public void ClearData_NullGroupIsSkippedWithoutThrowing()
    {
        var m = NewMerchant();
        m.m_GoodsList.Add(null);
        m.m_GoodsList.Add(new List<object> { Item(1) });
        m.ClearData();
        Assert.Empty(m.m_GoodsList);
        Assert.Empty(_messages);
    }

    [Fact]
    public void ClearData_OnException_ReportsExceptionMessageAndDetail()
    {
        var m = NewMerchant();
        // 放一个非 List<object> 的元素 → 强制转换抛 InvalidCastException → 走 except 分支
        m.m_GoodsList.Add("NOT-A-LIST");

        m.ClearData();   // 不得抛出

        Assert.Equal(2, _messages.Count);
        Assert.Equal("[Exception] TMerchant.ClearData", _messages[0]);
        Assert.False(string.IsNullOrEmpty(_messages[1]));
    }

    // -----------------------------------------------------------------------
    // TMerchant.LoadNpcScript（3180-3204）/ LoadNpcIconFile（3206-3226）
    // -----------------------------------------------------------------------

    [Fact]
    public void MerchantLoadNpcScript_AddMapName_NotFb_UsesScriptDashMap()
    {
        var m = NewMerchant("M1", "5");
        m.m_boFB = false;
        m.m_ItemTypeList.Add(7);
        string scriptFile = null, iconSc = null;
        string seenPath = null;
        NpcSeams.LoadScriptFile = (npc, path, sc) => { seenPath = path; scriptFile = sc; };
        NpcSeams.LoadIconFile = (npc, dir, sc) => { Assert.Equal(NpcSeams.sNpcIcons, dir); iconSc = sc; };

        m.LoadNpcScript(true);

        Assert.Empty(m.m_ItemTypeList);
        Assert.Equal(NpcSeams.sMarket_Def, m.m_sPath);
        Assert.Equal(NpcSeams.sMarket_Def, seenPath);
        Assert.Equal("M1-5", scriptFile);
        Assert.Equal("M1-5", iconSc);
    }

    [Fact]
    public void MerchantLoadNpcScript_AddMapName_Fb_UsesFbName()
    {
        var m = NewMerchant("M1", "5");
        m.m_boFB = true;
        m.m_sFBName = "FB9";
        string sc = null;
        NpcSeams.LoadScriptFile = (npc, path, name) => sc = name;
        m.LoadNpcScript(true);
        Assert.Equal("M1-FB9", sc);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MerchantLoadNpcScript_NoAddMapName_AlwaysBareScriptName(bool boFB)
    {
        // 原文 3196-3200：两个分支体**完全相同**（`SC := m_sScript`）—— 冗余照抄。
        var m = NewMerchant("M1", "5");
        m.m_boFB = boFB;
        m.m_sFBName = "FB9";
        string sc = null;
        NpcSeams.LoadScriptFile = (npc, path, name) => sc = name;
        m.LoadNpcScript(false);
        Assert.Equal("M1", sc);
    }

    [Fact]
    public void MerchantLoadNpcIconFile_DoesNotTouchItemTypeListOrPath()
    {
        var m = NewMerchant("M1", "5");
        m.m_ItemTypeList.Add(7);
        m.m_sPath = "KEEPPATH";
        string sc = null;
        NpcSeams.LoadIconFile = (npc, dir, name) => sc = name;
        m.LoadNpcIconFile(true);
        Assert.Single(m.m_ItemTypeList);
        Assert.Equal("KEEPPATH", m.m_sPath);
        Assert.Equal("M1-5", sc);
    }

    // -----------------------------------------------------------------------
    // TNormNpc.LoadNpcScript（9575-9591）/ LoadNpcIconFile（9593-9602）
    // -----------------------------------------------------------------------

    [Fact]
    public void NormNpcLoadNpcScript_Quest_UsesNpcDefPathAndLoadsIcon()
    {
        var npc = new TNormNpc { m_boIsQuest = true, m_sCharName = "GUARD", m_sMapName = "0", m_sFilePath = "P\\" };
        string path = null, name = null;
        bool iconLoaded = false;
        NpcSeams.LoadNpcScriptFile = (n, p, s) => { path = p; name = s; };
        NpcSeams.LoadIconFile = (n, dir, s) => { iconLoaded = true; Assert.Equal("GUARD-0", s); };

        npc.LoadNpcScript();

        Assert.Equal(NpcSeams.sNpc_def, npc.m_sPath);
        Assert.Equal("P\\", path);           // 仍传 m_sFilePath 作目录
        Assert.Equal("GUARD-0", name);
        Assert.True(iconLoaded);
    }

    [Fact]
    public void NormNpcLoadNpcScript_NotQuest_UsesFilePathAndNoIcon()
    {
        var npc = new TNormNpc { m_boIsQuest = false, m_sCharName = "GUARD", m_sMapName = "0", m_sFilePath = "Q\\" };
        string path = null, name = null;
        bool iconLoaded = false;
        NpcSeams.LoadNpcScriptFile = (n, p, s) => { path = p; name = s; };
        NpcSeams.LoadIconFile = (n, dir, s) => iconLoaded = true;

        npc.LoadNpcScript();

        Assert.Equal("Q\\", npc.m_sPath);
        Assert.Equal("Q\\", path);
        Assert.Equal("GUARD", name);         // 无 "-地图"
        Assert.False(iconLoaded);
    }

    [Fact]
    public void NormNpcLoadNpcIconFile_NotQuest_IsNoOp()
    {
        // 原文 9593-9602：`if m_boIsQuest` 之后**没有 else** → 非任务型是空操作。
        var npc = new TNormNpc { m_boIsQuest = false, m_sCharName = "GUARD", m_sMapName = "0" };
        bool iconLoaded = false;
        NpcSeams.LoadIconFile = (n, dir, s) => iconLoaded = true;
        npc.LoadNpcIconFile();
        Assert.False(iconLoaded);
    }

    [Fact]
    public void NormNpcLoadNpcIconFile_Quest_LoadsIconOnly()
    {
        var npc = new TNormNpc { m_boIsQuest = true, m_sCharName = "GUARD", m_sMapName = "7" };
        string dir = null, name = null;
        NpcSeams.LoadIconFile = (n, d, s) => { dir = d; name = s; };
        npc.LoadNpcIconFile();
        Assert.Equal(NpcSeams.sNpcIcons, dir);
        Assert.Equal("GUARD-7", name);
    }

    // -----------------------------------------------------------------------
    // TBoxMonster（10510-10543）
    // -----------------------------------------------------------------------

    [Fact]
    public void BoxMonster_Create_SetsRaceServerToBox()
    {
        var box = new TBoxMonster();
        Assert.Equal(Grobal2Const.RC_BOX, box.m_btRaceServer);
        Assert.Equal(30, Grobal2Const.RC_BOX);
    }

    [Fact]
    public void BoxMonster_Operate_AlwaysReturnsFalse()
    {
        var box = new TBoxMonster();
        // 原文 10530-10531 的 `RM_MAGSTRUCK` 分支被块注释 → 恒 False
        Assert.False(box.Operate(new GXX.M2Server.TProcessMessage { wIdent = Grobal2Const.RM_MAGSTRUCK }));
        Assert.False(box.Operate(new GXX.M2Server.TProcessMessage { wIdent = 0 }));
    }

    [Fact]
    public void BoxMonster_Run_AliveClampsHpToMax()
    {
        var box = new TBoxMonster { m_boDeath = false };
        box.m_wAbil.MaxHP = 50;
        box.m_wAbil.HP = 10;
        box.Run();
        Assert.Equal(50u, box.m_wAbil.HP);
    }

    [Fact]
    public void BoxMonster_Run_DeadZeroesHp()
    {
        var box = new TBoxMonster { m_boDeath = true };
        box.m_wAbil.MaxHP = 50;
        box.m_wAbil.HP = 30;
        box.Run();
        Assert.Equal(0u, box.m_wAbil.HP);
    }

    [Fact]
    public void BoxMonster_Run_ClearsMaster()
    {
        var box = new TBoxMonster();
        box.m_Master = new TBoxMonster();
        box.Run();
        Assert.Null(box.m_Master);
    }

    [Fact]
    public void BoxMonster_Run_NullMasterStaysNull()
    {
        var box = new TBoxMonster();
        box.m_Master = null;
        box.Run();
        Assert.Null(box.m_Master);
    }
}

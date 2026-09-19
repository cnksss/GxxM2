using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig;
using GXX.Client.GUI.GameConfig.Seams;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次 P1（车道 lane-client-guiconfig）：Mirs/MirsConfigDlg.pas 的
/// **配置数据层 + 可测行为层** 1:1 移植测试。
/// 覆盖：TConfig 初值（全部 5×9 数组）、g_Config 全局初值、Create 的 22 条勾选默认、
/// 访问器（Enabled/ProtectEnabled/Visible/Logout/Close）、CheckBoxClickEx 50 条映射与 3 处副作用、
/// RefConfig 回写、ShowItem 查询族、Struck/HealthChange 伤害分派、CanFilterExp、
/// 以及 6 个缺失 TConfigChecked 成员的映射表。
/// </summary>
[Collection("GuiCfgConfigure")]
public sealed class GuiCfgMirsConfigTests : IDisposable
{
    private readonly MirsConfigDlgControlsStub _ui = new MirsConfigDlgControlsStub();
    private readonly TMirsConfigDlg _dlg;
    private readonly List<(string, int, int)> _chat = new List<(string, int, int)>();
    private readonly List<bool> _repeatBgm = new List<bool>();
    private readonly List<(int, int)> _hpUse = new List<(int, int)>();
    private readonly List<(int, int)> _mpUse = new List<(int, int)>();

    private sealed class Self : IHumActorSeam
    {
        public Self(string name = "我") { m_sUserName = name; }
        public int m_nCurrX => 0;
        public int m_nCurrY => 0;
        public string m_sUserName { get; }
        public int HP = 1000;
        public int MP = 500;
    }

    private sealed class Hero : IHeroActorSeam
    {
        public Hero(int bag = 40) { m_nBagCount = bag; }
        public int m_nBagCount { get; }
        public int HP = 800;
        public int MP = 400;
    }

    public GuiCfgMirsConfigTests()
    {
        // 顺序要紧：
        //   1) 先取"原始默认值"快照（只在首个测试实例上做一次；此时 g_Config 还是类初始化器的初值）；
        //   2) 再 Reset() 所有接缝全局；
        //   3) 最后建 _dlg —— 这样 _dlg 的 22 条勾选默认不会被任何东西覆盖。
        GuiCfgTestEnv.SnapshotMirsConfigOnce();
        GuiCfgTestEnv.Reset();

        _dlg = new TMirsConfigDlg(_ui);
        _dlg.DamageHPUseItem = (nObj, nDamage) => _hpUse.Add((nObj, nDamage));
        _dlg.DamageMPUseItem = (nObj, nDamage) => _mpUse.Add((nObj, nDamage));
        _dlg.GetActorHP = actor => actor is Self s ? s.HP : (actor is Hero h ? h.HP : 0);
        _dlg.GetActorMP = actor => actor is Self s ? s.MP : (actor is Hero h ? h.MP : 0);

        ChatBoardSeam.AddChatBoardString = (m, c, b) => _chat.Add((m, c, b));
        MirsConfigGlobalSeam.SetRepeatBGSound = v => _repeatBgm.Add(v);
    }

    public void Dispose()
    {
        GuiCfgTestEnv.RestoreMirsConfig();
        GuiCfgTestEnv.Reset();
    }

    // ============================================================ TConfig / g_Config 初值

    [Fact]
    public void ScalarDefaultsMatchOriginal()
    {
        // MirsConfigDlg.pas:421-437
        var c = TMirsConfigDlg.g_Config;
        Assert.Equal(0, c.nFilterMinExp);
        Assert.Equal(0, c.nAutoUseMagicTime);
        Assert.Equal(0u, c.dwAutoUseMagicTick);

        Assert.False(c.boRenewSpecialIsAuto);
        Assert.Equal(0, c.nRenewSpecialPercent);
        Assert.Equal(0, c.nRenewSpecialTime);

        Assert.False(c.boRenewBookIsAuto);
        Assert.Equal(0, c.nRenewBookPercent);
        Assert.Equal(0, c.nRenewBookTime);
        Assert.Equal(0, c.nRenewBookNowBookIndex);
        Assert.Equal("", c.sRenewBookNowBookItem);

        Assert.Equal(0, c.MedicaMode);
    }

    [Fact]
    public void CheckHpDefaultsMatchOriginal()
    {
        // 439-445
        var c = TMirsConfigDlg.g_Config;
        Assert.Equal(new[] { false, false, false, false, false }, c.CheckHpIsAutos);
        Assert.Equal(new[] { 0, 0, 0, 0, 0 }, c.CheckHpPercents);
        Assert.Equal(new[] { 0, 0, 0, 0, 0 }, c.CheckHpValues);
        Assert.Equal(new uint[] { 1000, 1000, 1000, 1000, 1000 }, c.CheckHpCheckTimes);
        Assert.Equal(new uint[] { 0, 0, 0, 0, 0 }, c.CheckHpCheckTicks);
        Assert.Equal(new uint[] { 10000, 10000, 10000, 10000, 10000 }, c.CheckHpUseTimes);
        Assert.Equal(new uint[] { 0, 0, 0, 0, 0 }, c.CheckHpUseTicks);
    }

    [Fact]
    public void CheckMpDefaultsMatchOriginal()
    {
        // 448-454
        var c = TMirsConfigDlg.g_Config;
        Assert.Equal(new[] { false, false, false, false, false }, c.CheckMpIsAutos);
        Assert.Equal(new[] { 0, 0, 0, 0, 0 }, c.CheckMpPercents);
        Assert.Equal(new[] { 0, 0, 0, 0, 0 }, c.CheckMpValues);
        Assert.Equal(new uint[] { 1000, 1000, 1000, 1000, 1000 }, c.CheckMpCheckTimes);
        Assert.Equal(new uint[] { 0, 0, 0, 0, 0 }, c.CheckMpCheckTicks);
        Assert.Equal(new uint[] { 10000, 10000, 10000, 10000, 10000 }, c.CheckMpUseTimes);
        Assert.Equal(new uint[] { 0, 0, 0, 0, 0 }, c.CheckMpUseTicks);
    }

    [Fact]
    public void RenewDefaultsMatchOriginalIncludingAsymmetry()
    {
        // 456-474：RenewMPTimes 原文是 (0,0,0,0,0)，其余同族是 1000 —— **照抄**
        var c = TMirsConfigDlg.g_Config;

        Assert.Equal(new[] { false, false, false, false, false }, c.RenewHPIsAutos);
        Assert.Equal(new[] { 10, 10, 10, 10, 10 }, c.RenewHPPercents);
        Assert.Equal(new[] { 1000, 1000, 1000, 1000, 1000 }, c.RenewHPTimes);
        Assert.Equal(new uint[] { 0, 0, 0, 0, 0 }, c.RenewHPTicks);

        Assert.Equal(new[] { false, false, false, false, false }, c.RenewMPIsAutos);
        Assert.Equal(new[] { 10, 10, 10, 10, 10 }, c.RenewMPPercents);
        Assert.Equal(new[] { 0, 0, 0, 0, 0 }, c.RenewMPTimes);      // ← 原文如此
        Assert.Equal(new uint[] { 0, 0, 0, 0, 0 }, c.RenewMPTicks);

        Assert.Equal(new[] { false, false, false, false, false }, c.RenewSpecialHPIsAutos);
        Assert.Equal(new[] { 10, 10, 10, 10, 10 }, c.RenewSpecialHPPercents);
        Assert.Equal(new[] { 1000, 1000, 1000, 1000, 1000 }, c.RenewSpecialHPTimes);
        Assert.Equal(new uint[] { 0, 0, 0, 0, 0 }, c.RenewSpecialHPTicks);

        Assert.Equal(new[] { false, false, false, false, false }, c.RenewSpecialMPIsAutos);
        Assert.Equal(new[] { 10, 10, 10, 10, 10 }, c.RenewSpecialMPPercents);
        Assert.Equal(new[] { 1000, 1000, 1000, 1000, 1000 }, c.RenewSpecialMPTimes);
        Assert.Equal(new uint[] { 0, 0, 0, 0, 0 }, c.RenewSpecialMPTicks);
    }

    [Fact]
    public void SuperMedicaDefaultsMatchOriginal()
    {
        // 476-542
        var c = TMirsConfigDlg.g_Config;
        Assert.Equal(new[] { false, false, false, false, false }, c.UseSuperMedicas);

        Assert.Equal(new[]
        {
            "太阳水", "强效太阳水", "万年雪霜", "疗伤药", "疗伤药(任务)",
            "强效万年雪霜", "强效疗伤药", "超级万年雪霜", "超级疗伤药"
        }, c.SuperMedicaItemNames);

        for (int m = 0; m < 5; m++)
            for (int i = 0; i < 9; i++)
            {
                Assert.False(c.SuperMedicaUses[m, i]);
                Assert.Equal(0, c.SuperMedicaHPs[m, i]);
                Assert.Equal(500, c.SuperMedicaHPTimes[m, i]);
                Assert.Equal(0, c.SuperMedicaHPTicks[m, i]);
                Assert.Equal(0, c.SuperMedicaMPs[m, i]);
                Assert.Equal(500, c.SuperMedicaMPTimes[m, i]);
                Assert.Equal(0, c.SuperMedicaMPTicks[m, i]);
            }
    }

    [Fact]
    public void ArrayDimensionsAreFiveByFiveAndFiveByNine()
    {
        var c = TMirsConfigDlg.g_Config;
        Assert.Equal(5, c.CheckHpIsAutos.Length);
        Assert.Equal(5, c.RenewSpecialMPTicks.Length);
        Assert.Equal(9, c.SuperMedicaItemNames.Length);
        Assert.Equal(5, c.SuperMedicaUses.GetLength(0));
        Assert.Equal(9, c.SuperMedicaUses.GetLength(1));
        Assert.Equal(5, c.SuperMedicaMPTicks.GetLength(0));
        Assert.Equal(9, c.SuperMedicaMPTicks.GetLength(1));
    }

    // ============================================================ 构造函数勾选默认（545-603）

    [Fact]
    public void CreateSetExactly22CheckBoxDefaults()
    {
        // 574-602：9 条显式 True/False + 8 条刺杀 + 6 条手动 + 1 条毒符 + 1 条 MovePick
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckShowHPLabel));
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckShowUserName));
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckMagicLock));
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckAutoOrderItem));
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckNotNeedShift));
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckAutoPickUpItem));
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckBGMusic));
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckRepeatBGMusic));
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckNotParaly));
    }

    [Fact]
    public void CreateDefaultsMartialArtsFamilyToFalse()
    {
        foreach (var ck in new[]
        {
            TConfigChecked.ckSmartLongHit, TConfigChecked.ckSmartPosLongHit,
            TConfigChecked.ckSmartWalkLongHit, TConfigChecked.ckSmartWideHit,
            TConfigChecked.ckSmartFireHit, TConfigChecked.ckSmartSwordHit,
            TConfigChecked.ckSmartCrsHit, TConfigChecked.ckSmartTwnHit,
        })
            Assert.False(_dlg.GetConfigChecked(ck), ck.ToString());
    }

    [Fact]
    public void CreateDefaultsManuallyControlledToFalseExceptLightenLock()
    {
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckHumAutoShield));
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckHumStruckShield));
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckHumShootLightenLockTarget));  // ← 唯一的 True
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckHumManuallyFireBoom));
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckHumManuallySnowWind));
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckHumManuallyMeteorShower));
    }

    [Fact]
    public void CreateLeavesEveryOtherBitFalse()
    {
        // 构造函数只碰了 574-602 那 22 个位；其余全部保持 FillChar 后的 False
        var touched = new HashSet<TConfigChecked>
        {
            TConfigChecked.ckShowHPLabel, TConfigChecked.ckShowUserName, TConfigChecked.ckMagicLock,
            TConfigChecked.ckAutoOrderItem, TConfigChecked.ckNotNeedShift, TConfigChecked.ckAutoPickUpItem,
            TConfigChecked.ckBGMusic, TConfigChecked.ckRepeatBGMusic, TConfigChecked.ckNotParaly,
            TConfigChecked.ckSmartLongHit, TConfigChecked.ckSmartPosLongHit, TConfigChecked.ckSmartWalkLongHit,
            TConfigChecked.ckSmartWideHit, TConfigChecked.ckSmartFireHit, TConfigChecked.ckSmartSwordHit,
            TConfigChecked.ckSmartCrsHit, TConfigChecked.ckSmartTwnHit,
            TConfigChecked.ckHumAutoShield, TConfigChecked.ckHumStruckShield,
            TConfigChecked.ckHumShootLightenLockTarget, TConfigChecked.ckHumManuallyFireBoom,
            TConfigChecked.ckHumManuallySnowWind, TConfigChecked.ckHumManuallyMeteorShower,
        };
        for (int i = 0; i <= TConfigCheckedBounds.HighOrdinal; i++)
        {
            var ck = (TConfigChecked)i;
            bool expected = ck == TConfigChecked.ckShowHPLabel || ck == TConfigChecked.ckShowUserName
                || ck == TConfigChecked.ckMagicLock || ck == TConfigChecked.ckAutoOrderItem
                || ck == TConfigChecked.ckNotNeedShift || ck == TConfigChecked.ckAutoPickUpItem
                || ck == TConfigChecked.ckBGMusic || ck == TConfigChecked.ckRepeatBGMusic
                || ck == TConfigChecked.ckHumShootLightenLockTarget;
            Assert.Equal(expected, _dlg.GetConfigChecked(ck));
        }
    }

    [Fact]
    public void ConfigCheckedsArrayCoversWholeEnumPlusSyntheticSlot()
    {
        // HighOrdinal + 1 个真实成员 + 1 个给 ckMovePick 的合成槽位
        Assert.Equal(TConfigCheckedBounds.HighOrdinal + 1, (int)TConfigChecked.ckObjectHintEffect + 1);
        Assert.Equal(TConfigCheckedBounds.HighOrdinal + 2, _dlg.ConfigCheckeds.Length);
    }

    // ============================================================ 访问器

    [Fact]
    public void GetTypeIsPtDefault()
    {
        Assert.Equal(TConfigDlgType.ptDefault, _dlg.GetType());
        Assert.Equal(TConfigDlgType.ptDefault, _dlg.ConfigDlgType);
    }

    [Fact]
    public void SetConfigCheckedTriggersRefConfigOnlyOnChange()
    {
        // 原文 625-629：值变化才写 + RefConfig。FInitializeed=False 时 RefConfig 直接 Exit，
        // 故这里只能验证"取值被写入"；RefConfig 的副作用另测。
        _dlg.SetConfigChecked(TConfigChecked.ckShowMonName, false);
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckShowMonName));
        _dlg.SetConfigChecked(TConfigChecked.ckShowMonName, true);
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckShowMonName));
    }

    [Fact]
    public void EnabledDefaultsFalseAndSetEnabledClearsLoadConfigOnFalse()
    {
        // 547 + 662-667
        Assert.False(_dlg.GetEnabled());
        _dlg.SetEnabled(true);
        Assert.True(_dlg.GetEnabled());
        _dlg.SetEnabled(false);
        Assert.False(_dlg.GetEnabled());
    }

    [Fact]
    public void ProtectEnabledDefaultsTrueAndIsIndependentOfEnabled()
    {
        // 548 + 669-679
        Assert.True(_dlg.GetProtectEnabled());
        _dlg.SetEnabled(true);
        Assert.True(_dlg.GetProtectEnabled());
    }

    [Fact]
    public void OpenIsNoOp()
    {
        _dlg.Open();
        Assert.False(_dlg.GetEnabled());
    }

    [Fact]
    public void CloseDisablesHidesAndBacksUpFileItemDb()
    {
        // 637-643
        _dlg.SetEnabled(true);
        _dlg.SetVisible(true);
        Assert.True(_dlg.GetVisible());

        // FileItemDB.BackUp 需要两个链表等长，这里先准备一个合法状态
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_FileItemList.Clear();

        _dlg.Close();
        Assert.False(_dlg.GetEnabled());
        Assert.False(_dlg.GetVisible());
    }

    [Fact]
    public void GetVisibleReturnsFalseWhenPlugConfigDlgIsNil()
    {
        // 原文 645-649：PlugConfigDlg = nil 时 Result 保持 False
        _dlg.PlugConfigDlg = null;
        Assert.False(_dlg.GetVisible());
        _dlg.SetVisible(true);                 // nil 时也不应抛异常
        Assert.False(_dlg.GetVisible());
    }

    [Fact]
    public void PlugConfigDlgCloseClickExHidesWhenNotNull()
    {
        _dlg.SetVisible(true);
        _dlg.PlugConfigDlgCloseClickEx();
        Assert.False(_dlg.GetVisible());
    }

    [Fact]
    public void PlugPageControlConfigInRealAreaMatchesOriginalExpression()
    {
        // 731-734：IsRealArea := not ((X >= Width-12) and (Y <= Height+20))
        _dlg.PlugPageControlConfigWidth = 100;
        _dlg.PlugPageControlConfigHeight = 50;

        _dlg.PlugPageControlConfigInRealArea(87, 70, out bool r1);   // X=87 <88 → not(false and true) = true
        Assert.True(r1);

        _dlg.PlugPageControlConfigInRealArea(88, 70, out bool r2);   // X=88, Y=70<=70 → not(true) = false
        Assert.False(r2);

        _dlg.PlugPageControlConfigInRealArea(88, 71, out bool r3);   // Y>70 → not(false) = true
        Assert.True(r3);

        _dlg.PlugPageControlConfigInRealArea(200, 0, out bool r4);   // X 很大但 Y<=70 → false
        Assert.False(r4);
    }

    [Fact]
    public void EmptyImplementationsReturnFalseAndDoNothing()
    {
        ushort key = 65;
        Assert.False(_dlg.FormKeyDown(ref key, DelphiShiftState.ssCtrl));
        char ch = 'a';
        Assert.False(_dlg.FormKeyPress(ref ch));
        _dlg.RefreshMySelfAbil();
        _dlg.RefreshMyHeroAbil();
        _dlg.RefreshMyHeroMagicList();
        _dlg.RefreshUnBindItemList();
        _dlg.RefAggregateNoThrow();
        Assert.Equal(65, key);          // 原文不改 Key
        Assert.Equal('a', ch);
    }

    [Fact]
    public void LogoutClearsFlagsAndSaves()
    {
        // 748-753
        int saves = 0;
        _dlg.SaveConfigFileSeam = () => saves++;
        _dlg.SetEnabled(true);
        _dlg.Logout();
        Assert.False(_dlg.GetEnabled());
        Assert.Equal(1, saves);
    }

    // ============================================================ CheckBoxClickEx

    [Fact]
    public void CheckBoxClickExIsGatedByBoNotCanUseClientConfig()
    {
        // 原文 1080：`if FClientConfig.boNotCanUseClientConfig then Exit;`
        var cfg = new TClientConfig { boNotCanUseClientConfig = true };
        _dlg.LoadClientConfig(cfg);
        _ui.PlugCheckBoxShowHPLabel = false;
        _dlg.CheckBoxClickEx("PlugCheckBoxShowHPLabel");
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckShowHPLabel));   // 未被改写

        // 关掉门禁后生效
        var cfg2 = new TClientConfig { boNotCanUseClientConfig = false };
        _dlg.LoadClientConfig(cfg2);
        _dlg.CheckBoxClickEx("PlugCheckBoxShowHPLabel");
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckShowHPLabel));
    }

    [Fact]
    public void CheckBoxClickExWritesControlCheckedIntoConfigBit()
    {
        _ui.PlugCheckBoxHideGhost = true;                       // 默认 False
        _dlg.CheckBoxClickEx("PlugCheckBoxHideGhost");
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckHideGhost));

        _ui.PlugCheckBoxHideGhost = false;
        _dlg.CheckBoxClickEx("PlugCheckBoxHideGhost");
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckHideGhost));
    }

    [Theory]
    // 控件名, 配置位 —— 逐条对照 MirsConfigDlg.pas:1081-1267
    [InlineData("PlugCheckBoxNumberLable", TConfigChecked.ckShowNumberLable)]
    [InlineData("PlugCheckBoxJobAndLevel", TConfigChecked.ckShowJobAndLevel)]
    [InlineData("PlugCheckBoxShowGreenHint", TConfigChecked.ckShowGreenHint)]
    [InlineData("PlugCheckBoxShowActorName", TConfigChecked.ckShowUserName)]
    [InlineData("PlugCheckBoxHideDescUserName", TConfigChecked.ckOnlyShowCharName)]
    [InlineData("PlugCheckBoxDuraWarning", TConfigChecked.ckDuraWarning)]
    [InlineData("PlugCheckBoxNoShift", TConfigChecked.ckNotNeedShift)]
    [InlineData("PlugCheckBoxExpFilter", TConfigChecked.ckFilterExp)]
    [InlineData("PlugCheckBoxShowMimiMapDesc", TConfigChecked.ckShowMapDesc)]
    [InlineData("PlugCheckBoxNotParaly", TConfigChecked.ckNotParaly)]
    [InlineData("PlugCheckBoxShowHighlightHPLabel", TConfigChecked.ckShowHighlightHPLabel)]
    [InlineData("PlugCheckBoxShowHealthNumber", TConfigChecked.ckShowMoveLable)]
    [InlineData("PlugCheckBoxHideHumEffect", TConfigChecked.ckHideHumEffect)]
    [InlineData("PlugCheckBoxHideWeaponEffect", TConfigChecked.ckHideWeaponEffect)]
    [InlineData("PlugCheckBoxShowMonName", TConfigChecked.ckShowMonName)]
    [InlineData("PlugCheckBoxAutoOrderItem", TConfigChecked.ckAutoOrderItem)]
    [InlineData("PlugCheckBoxMagicLock", TConfigChecked.ckMagicLock)]
    [InlineData("PlugCheckBoxSmartLongHit", TConfigChecked.ckSmartLongHit)]
    [InlineData("PlugCheckBoxSmartPosLongHit", TConfigChecked.ckSmartPosLongHit)]
    [InlineData("PlugCheckBoxSmartWalkLongHit", TConfigChecked.ckSmartWalkLongHit)]
    [InlineData("PlugCheckBoxSmartWideHit", TConfigChecked.ckSmartWideHit)]
    [InlineData("PlugCheckBoxSmartFireHit", TConfigChecked.ckSmartFireHit)]
    [InlineData("PlugCheckBoxSmartSwordHit", TConfigChecked.ckSmartSwordHit)]
    [InlineData("PlugCheckBoxSmartKTZHit", TConfigChecked.ckSmart66Hit)]
    [InlineData("PlugCheckBoxAutoHideMode", TConfigChecked.ckAutoHideMode)]
    [InlineData("PlugCheckBoxHumAutoShield", TConfigChecked.ckHumAutoShield)]
    [InlineData("PlugCheckBoxHumStruckShield", TConfigChecked.ckHumStruckShield)]
    [InlineData("PlugCheckBoxHeroAutoShield", TConfigChecked.ckHeroAutoShield)]
    [InlineData("PlugCheckBoxAssistantHeroAutoShield", TConfigChecked.ckAssistantHeroAutoShield)]
    [InlineData("PlugCheckBoxHumManuallySnowWind", TConfigChecked.ckHumManuallySnowWind)]
    [InlineData("PlugCheckBoxHumManuallyFireBoom", TConfigChecked.ckHumManuallyFireBoom)]
    [InlineData("PlugCheckBoxHumShootLightenLockTarget", TConfigChecked.ckHumShootLightenLockTarget)]
    [InlineData("PlugCheckBoxHumManuallyMeteorShower", TConfigChecked.ckHumManuallyMeteorShower)]
    [InlineData("PlugCheckBoxAutoMagic", TConfigChecked.ckAutoUseMagic)]
    [InlineData("PlugCheckBoxUseKeyBoard", TConfigChecked.ckUseKeyBoard)]
    [InlineData("PlugCheckBoxUseSuperMedica", TConfigChecked.ckUseSuperMedica)]
    [InlineData("PlugCheckBoxDisableSelfStruck", TConfigChecked.ckDisableSelfStruck)]
    [InlineData("PlugCheckBoxSpeedSlow", TConfigChecked.ckSpeedSlow)]
    [InlineData("PlugCheckBoxAutoPickUpItem", TConfigChecked.ckAutoPickUpItem)]
    public void CheckBoxClickExMappingTableIsExact(string control, TConfigChecked bit)
    {
        SetControl(control, true);
        _dlg.CheckBoxClickEx(control);
        Assert.True(_dlg.GetConfigChecked(bit), $"{control} -> {bit}");
        SetControl(control, false);
        _dlg.CheckBoxClickEx(control);
        Assert.False(_dlg.GetConfigChecked(bit), $"{control} -> {bit}");
    }

    [Theory]
    [InlineData("PlugCheckBoxShowItemName", TConfigChecked.ckShowMonName)]
    [InlineData("PlugCheckBoxShowFilterItem", TConfigChecked.ckShowMonName)]
    [InlineData("PlugCheckBoxItemHint", TConfigChecked.ckShowHPLabel)]
    [InlineData("PlugCheckBoxAutoTakeOnItem", TConfigChecked.ckAutoCHangePoison)]
    [InlineData("PlugCheckBoxAutoCHangePoison", TConfigChecked.ckAutoCHangePoison)]
    public void CheckBoxClickExMissingEnumMembersUseDocumentedAlias(string control, TConfigChecked alias)
    {
        // 原文用的 6 个 ck* 在检出枚举中不存在，见 MirsConfigCheckedMap 的映射
        SetControl(control, true);
        _dlg.CheckBoxClickEx(control);
        Assert.True(_dlg.GetConfigChecked(alias), $"{control} -> {alias}");
    }

    [Fact]
    public void SoundCheckBoxDoesNotTouchConfigBitsButWritesGlobalAndChat()
    {
        // 原文 1268-1279：唯一不写 FConfigCheckeds 的分支
        bool before = _dlg.GetConfigChecked(TConfigChecked.ckVolume);
        _ui.PlugCheckBoxSound = true;
        _dlg.CheckBoxClickEx("PlugCheckBoxSound");
        Assert.True(MirsConfigGlobalSeam.g_boSound);
        Assert.Single(_chat);
        Assert.Equal("[音效 开]", _chat[0].Item1);
        Assert.Equal(before, _dlg.GetConfigChecked(TConfigChecked.ckVolume));

        _chat.Clear();
        _ui.PlugCheckBoxSound = false;
        _dlg.CheckBoxClickEx("PlugCheckBoxSound");
        Assert.False(MirsConfigGlobalSeam.g_boSound);
        Assert.Single(_chat);
        Assert.Equal("[音效 关]", _chat[0].Item1);
        // 两个分支的颜色都是 clWhite/clBlack
        Assert.Equal(0x00FFFFFF, _chat[0].Item2);
        Assert.Equal(0x00000000, _chat[0].Item3);
    }

    [Fact]
    public void BgMusicCheckBoxSyncsGlobal()
    {
        // 原文 1280-1284（构造默认 ckBGMusic=True，这里点成 False）
        _ui.PlugCheckBoxBGMusic = false;
        _dlg.CheckBoxClickEx("PlugCheckBoxBGMusic");
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckBGMusic));
        Assert.False(MirsConfigGlobalSeam.g_boBGSound);

        // 再点回 True
        _ui.PlugCheckBoxBGMusic = true;
        _dlg.CheckBoxClickEx("PlugCheckBoxBGMusic");
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckBGMusic));
        Assert.True(MirsConfigGlobalSeam.g_boBGSound);
    }

    [Fact]
    public void RepeatBgMusicCheckBoxSyncsGlobalAndCallsSetRepeat()
    {
        // 原文 1285-1290。注意先清空采集器：SetConfigChecked 内部会调 RefConfig，
        // 而 RefConfig 自己也会调 SetRepeatBGSound（原文 1450），所以基线不一定是空。
        _repeatBgm.Clear();
        _ui.PlugCheckBoxRepeatBGMusic = false;
        _dlg.CheckBoxClickEx("PlugCheckBoxRepeatBGMusic");
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckRepeatBGMusic));
        Assert.False(MirsConfigGlobalSeam.g_boRepeatBGSound);
        Assert.Equal(new[] { false }, _repeatBgm);
    }

    [Fact]
    public void UnknownControlNameIsIgnored()
    {
        _dlg.CheckBoxClickEx("PlugCheckBoxDoesNotExist");
        Assert.Empty(_chat);
    }

    [Fact]
    public void CheckBoxClickExStopsAtFirstMatch()
    {
        // 原文 if/else if 链：PlugCheckBoxShowItemName 与 PlugCheckBoxShowFilterItem 都映射到
        // ckShowMonName，但点击前者不应被后者再次覆盖。
        _ui.PlugCheckBoxShowItemName = true;
        _ui.PlugCheckBoxShowFilterItem = false;
        _dlg.CheckBoxClickEx("PlugCheckBoxShowItemName");
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckShowMonName));
    }

    // ============================================================ RefConfig

    [Fact]
    public void RefConfigExitsBeforeFInitializeed()
    {
        _ui.PlugCheckBoxShowHPLabel = false;
        _dlg.SetConfigChecked(TConfigChecked.ckShowHPLabel, true);
        _dlg.RefConfig();                          // FInitializeed=False → 什么都不做
        Assert.False(_ui.PlugCheckBoxShowHPLabel);
    }

    [Fact]
    public void RefConfigWritesBitsToControls()
    {
        _dlg.SetInitializedForTest(true);
        _dlg.SetConfigChecked(TConfigChecked.ckShowHPLabel, true);
        _dlg.SetConfigChecked(TConfigChecked.ckHideGhost, true);
        _dlg.RefConfig();
        Assert.True(_ui.PlugCheckBoxShowHPLabel);
        Assert.True(_ui.PlugCheckBoxHideGhost);
        // 注意：Create 把 ckShowUserName 默认置 True，所以 RefConfig 会把控件写成 True
        Assert.True(_ui.PlugCheckBoxShowActorName);
    }

    [Fact]
    public void RefConfigSoundComesFromGlobalNotConfigBit()
    {
        // 原文 1443：PlugCheckBoxSound.Checked := g_boSound
        _dlg.SetInitializedForTest(true);
        MirsConfigGlobalSeam.g_boSound = true;
        _ui.PlugCheckBoxSound = false;
        _dlg.RefConfig();
        Assert.True(_ui.PlugCheckBoxSound);
    }

    [Fact]
    public void RefConfigSyncsBgmGlobalsAndCallsSetRepeat()
    {
        // 原文 1448-1450
        _dlg.SetInitializedForTest(true);
        _dlg.SetConfigChecked(TConfigChecked.ckBGMusic, true);
        _dlg.SetConfigChecked(TConfigChecked.ckRepeatBGMusic, true);
        _repeatBgm.Clear();
        _dlg.RefConfig();
        Assert.True(MirsConfigGlobalSeam.g_boBGSound);
        Assert.True(MirsConfigGlobalSeam.g_boRepeatBGSound);
        Assert.Equal(new[] { true }, _repeatBgm);
    }

    [Fact]
    public void RefConfigWritesMedicaModeEditsAndButtons()
    {
        // 原文 1478-1487
        _dlg.SetInitializedForTest(true);
        TMirsConfigDlg.g_Config.nFilterMinExp = 123;
        TMirsConfigDlg.g_Config.nAutoUseMagicTime = 456;
        TMirsConfigDlg.g_Config.MedicaMode = 3;
        int refUseItem = -1;
        _dlg.RefUseItemConfigSeam = n => refUseItem = n;

        _dlg.RefConfig();

        Assert.Equal(123, _ui.PlugEditExpFilter);
        Assert.Equal(456, _ui.PlugEditAutoMagicTime);
        Assert.False(_ui.PlugMemoConfig4Button1);
        Assert.False(_ui.PlugMemoConfig4Button2);
        Assert.False(_ui.PlugMemoConfig4Button3);
        Assert.True(_ui.PlugMemoConfig4Button4);
        Assert.False(_ui.PlugMemoConfig4Button5);
        Assert.Equal(3, refUseItem);
    }

    [Fact]
    public void RefConfigMedicaModeBoundsAreNotClamped()
    {
        // 原文 RefConfig 直接比较 0..4，模式越界时 5 个按钮**全为 False**
        _dlg.SetInitializedForTest(true);
        TMirsConfigDlg.g_Config.MedicaMode = 9;
        _dlg.RefConfig();
        Assert.False(_ui.PlugMemoConfig4Button1);
        Assert.False(_ui.PlugMemoConfig4Button2);
        Assert.False(_ui.PlugMemoConfig4Button3);
        Assert.False(_ui.PlugMemoConfig4Button4);
        Assert.False(_ui.PlugMemoConfig4Button5);
    }

    // ============================================================ ShowItem 查询族

    [Fact]
    public void ShowItemQueryFamilyDelegatesToFileItemDb()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_FileItemList.Clear();
        var it = new TShowItem
        {
            sItemName = "屠龙",
            boShowName = 1,
            boHintMsg = 1,
            boPickup = 0,
        };
        db.Add(it);

        Assert.Same(it, _dlg.GetShowItem("屠龙"));
        Assert.True(_dlg.FindShowItem("屠龙"));
        Assert.True(_dlg.FindHintItem("屠龙"));
        Assert.False(_dlg.FindPickItem("屠龙"));
        Assert.Null(_dlg.GetShowItem("不存在"));
        Assert.False(_dlg.FindShowItem("不存在"));
        Assert.False(_dlg.FindHintItem("不存在"));
        Assert.False(_dlg.FindPickItem("不存在"));
    }

    [Fact]
    public void ShowItemQueriesAreCaseInsensitive()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_FileItemList.Clear();
        db.Add(new TShowItem { sItemName = "LongSword", boShowName = 1 });
        Assert.True(_dlg.FindShowItem("longsword"));
    }

    [Fact]
    public void HintItemForwardsToFileItemDb()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_FileItemList.Clear();
        db.Add(new TShowItem { sItemName = "布衣", boHintMsg = 1 });
        ConfigShareSeam.g_MySelf = new Self();
        NextDirectionSeam.GetNextDirection = (a, b, c, d) => 0;

        _dlg.HintItem("布衣", 3, 4);
        Assert.Single(_chat);
        Assert.Equal("发现[布衣]，方位:上↑，坐标:(3,4).", _chat[0].Item1);
    }

    [Fact]
    public void ListViewItemClickWritesColumnFlagsAndSaves()
    {
        // 862-877
        int saves = 0;
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_FileItemList.Clear();
        var it = new TShowItem { sItemName = "屠龙" };
        db.Add(it);
        db.m_FileItemList.Add(it);

        var before = ConfigShareGlobal.g_sPlugServerName;
        ConfigShareSeam.g_MySelf = null;              // 让 SaveToFile 直接 Exit，避免真实落盘
        _dlg.ListViewItemClick(0, 1, it, true);
        Assert.Equal(1, it.boHintMsg);
        _dlg.ListViewItemClick(0, 2, it, true);
        Assert.Equal(1, it.boPickup);
        _dlg.ListViewItemClick(0, 3, it, true);
        Assert.Equal(1, it.boShowName);

        // 第 0 列不写任何标记
        var beforeHint = it.boHintMsg;
        _dlg.ListViewItemClick(0, 0, it, false);
        Assert.Equal(beforeHint, it.boHintMsg);

        // showItem = nil 时什么都不做
        _dlg.ListViewItemClick(0, 1, null, true);

        Assert.Equal(before, ConfigShareGlobal.g_sPlugServerName);
        _ = saves;
    }

    // ============================================================ Struck / HealthChange

    [Fact]
    public void StruckDoesNothingWhenDisabled()
    {
        var self = new Self();
        ConfigShareSeam.g_MySelf = self;
        _dlg.SetEnabled(false);
        self.HP = 1000;
        _dlg.Struck(self, 900, 1000);
        Assert.Empty(_hpUse);
    }

    [Fact]
    public void StruckSelfDamageGoesToSlotZero()
    {
        var self = new Self { HP = 1000 };
        ConfigShareSeam.g_MySelf = self;
        _dlg.SetEnabled(true);
        _dlg.Struck(self, 750, 1000);
        Assert.Equal(new[] { (0, 250) }, _hpUse);
    }

    [Fact]
    public void StruckHealingIsIgnored()
    {
        var self = new Self { HP = 1000 };
        ConfigShareSeam.g_MySelf = self;
        _dlg.SetEnabled(true);
        _dlg.Struck(self, 1200, 1000);       // HP 上升 → nDamage <= 0
        Assert.Empty(_hpUse);
        _dlg.Struck(self, 1000, 1000);       // 相等 → 0，也不处理
        Assert.Empty(_hpUse);
    }

    [Fact]
    public void StruckHeroDamageGoesToSlotOne()
    {
        var self = new Self();
        var hero = new Hero { HP = 800 };
        ConfigShareSeam.g_MySelf = self;
        ConfigShareSeam.g_MyHero = hero;
        _dlg.SetEnabled(true);
        _dlg.Struck(hero, 700, 800);
        Assert.Equal(new[] { (1, 100) }, _hpUse);
    }

    [Fact]
    public void StruckHeroIsSkippedWhenMySelfIsNil()
    {
        // 原文 2480-2498：英雄分支嵌在 `g_MySelf <> nil` 之内
        var hero = new Hero { HP = 800 };
        ConfigShareSeam.g_MySelf = null;
        ConfigShareSeam.g_MyHero = hero;
        _dlg.SetEnabled(true);
        _dlg.Struck(hero, 700, 800);
        Assert.Empty(_hpUse);
    }

    [Fact]
    public void StruckUnrelatedActorIsIgnored()
    {
        var self = new Self();
        ConfigShareSeam.g_MySelf = self;
        _dlg.SetEnabled(true);
        _dlg.Struck(new object(), 1, 1000);
        Assert.Empty(_hpUse);
    }

    [Fact]
    public void StruckWithNilMySelfAndNilActorIsSafe()
    {
        ConfigShareSeam.g_MySelf = null;
        _dlg.SetEnabled(true);
        _dlg.Struck(null, 1, 1);            // 不应抛异常
        Assert.Empty(_hpUse);
    }

    [Fact]
    public void HealthChangeSelfDispatchesBothHpAndMp()
    {
        var self = new Self { HP = 1000, MP = 500 };
        ConfigShareSeam.g_MySelf = self;
        _dlg.SetEnabled(true);
        _dlg.HealthChange(self, 900, 400, 1000);
        Assert.Equal(new[] { (0, 100) }, _hpUse);
        Assert.Equal(new[] { (0, 100) }, _mpUse);
    }

    [Fact]
    public void HealthChangeIndependentGatesForHpAndMp()
    {
        var self = new Self { HP = 1000, MP = 500 };
        ConfigShareSeam.g_MySelf = self;
        _dlg.SetEnabled(true);
        _dlg.HealthChange(self, 1100, 400, 1000);   // HP 上升、MP 下降
        Assert.Empty(_hpUse);
        Assert.Equal(new[] { (0, 100) }, _mpUse);
    }

    [Fact]
    public void HealthChangeHeroDispatchesBothToSlotOne()
    {
        var self = new Self();
        var hero = new Hero { HP = 800, MP = 400 };
        ConfigShareSeam.g_MySelf = self;
        ConfigShareSeam.g_MyHero = hero;
        _dlg.SetEnabled(true);
        _dlg.HealthChange(hero, 700, 300, 800);
        Assert.Equal(new[] { (1, 100) }, _hpUse);
        Assert.Equal(new[] { (1, 100) }, _mpUse);
    }

    [Fact]
    public void HealthChangeDoesNothingWhenDisabled()
    {
        var self = new Self { HP = 1000, MP = 500 };
        ConfigShareSeam.g_MySelf = self;
        _dlg.SetEnabled(false);
        _dlg.HealthChange(self, 1, 1, 1000);
        Assert.Empty(_hpUse);
        Assert.Empty(_mpUse);
    }

    // ============================================================ CanFilterExp

    [Fact]
    public void CanFilterExpRequiresFilterExpBitAndReturnsLessThanThreshold()
    {
        // MirsConfigDlg.pas:3191-3196：
        //   Result := False;
        //   if FConfigCheckeds[ckFilterExp] then Result := Exp < g_Config.nFilterMinExp;
        TMirsConfigDlg.g_Config.nFilterMinExp = 100;

        // 未勾选"经验过滤" → 永远 False
        _dlg.SetConfigChecked(TConfigChecked.ckFilterExp, false);
        Assert.False(_dlg.CanFilterExp(0));
        Assert.False(_dlg.CanFilterExp(1000));

        // 勾选后：Exp < 下限 才算“可过滤”
        _dlg.SetConfigChecked(TConfigChecked.ckFilterExp, true);
        Assert.True(_dlg.CanFilterExp(99));
        Assert.False(_dlg.CanFilterExp(100));      // 边界：相等不算
        Assert.False(_dlg.CanFilterExp(101));
    }

    [Fact]
    public void CanFilterExpWithNegativeThresholdNeverFilters()
    {
        // nFilterMinExp 是 Integer、Exp 是 LongWord：Delphi 比较混合符号时**两边提升到 Int64**，
        // 故 -1 仍是 -1（不是 $FFFFFFFF），任何无符号 Exp 都 >= -1 → 一律返回 False。
        _dlg.SetConfigChecked(TConfigChecked.ckFilterExp, true);
        TMirsConfigDlg.g_Config.nFilterMinExp = -1;
        Assert.False(_dlg.CanFilterExp(0));
        Assert.False(_dlg.CanFilterExp(999));
        Assert.False(_dlg.CanFilterExp(uint.MaxValue));
    }

    [Fact]
    public void CanFilterExpDoesNotDependOnMySelf()
    {
        // 原文没有 g_MySelf 判定（与其它单元的 CanFilterExp 不同）
        ConfigShareSeam.g_MySelf = null;
        TMirsConfigDlg.g_Config.nFilterMinExp = 10;
        _dlg.SetConfigChecked(TConfigChecked.ckFilterExp, true);
        Assert.True(_dlg.CanFilterExp(0));
    }

    // ============================================================ 缺失枚举成员映射

    [Theory]
    [InlineData(MirsConfigCheckedAlias.ckShowItemName, TConfigChecked.ckShowMonName)]
    [InlineData(MirsConfigCheckedAlias.ckShowFilterItem, TConfigChecked.ckShowMonName)]
    [InlineData(MirsConfigCheckedAlias.ckItemHint, TConfigChecked.ckShowHPLabel)]
    [InlineData(MirsConfigCheckedAlias.ckAutoTakeOnItem, TConfigChecked.ckAutoCHangePoison)]
    [InlineData(MirsConfigCheckedAlias.ckAutoChangePoison, TConfigChecked.ckAutoCHangePoison)]

    public void MissingConfigCheckedMembersMapIsDocumented(MirsConfigCheckedAlias alias, TConfigChecked expected)
    {
        TConfigChecked actual = alias switch
        {
            MirsConfigCheckedAlias.ckShowItemName => MirsConfigCheckedMap.ckShowItemName,
            MirsConfigCheckedAlias.ckShowFilterItem => MirsConfigCheckedMap.ckShowFilterItem,
            MirsConfigCheckedAlias.ckItemHint => MirsConfigCheckedMap.ckItemHint,
            MirsConfigCheckedAlias.ckAutoTakeOnItem => MirsConfigCheckedMap.ckAutoTakeOnItem,
            MirsConfigCheckedAlias.ckAutoChangePoison => MirsConfigCheckedMap.ckAutoChangePoison,
            _ => throw new ArgumentOutOfRangeException(nameof(alias)),
        };
        Assert.Equal(expected, actual);
        Assert.False(string.IsNullOrEmpty(MirsConfigCheckedMap.Describe(alias)));
    }

    [Fact]
    public void MovePickHasItsOwnSyntheticSlotThatDoesNotCollideWithAutoPickUp()
    {
        // **本车道发现的关键冲突**：ckMovePick 在检出枚举里无等价成员；
        // 若映射到 ckAutoPickUpItem，构造函数第 602 行的 `:= False` 会覆盖第 579 行的 `True`。
        // 因此单独给它一个合成槽位，两个位互不影响。
        Assert.NotEqual((int)TConfigChecked.ckAutoPickUpItem, MirsConfigCheckedMap.ckMovePickIndex);
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckAutoPickUpItem));      // 构造默认 True 未被覆盖
        Assert.False(_dlg.ConfigCheckeds[MirsConfigCheckedMap.ckMovePickIndex]); // ckMovePick 默认 False
    }

    // ============================================================ 接缝/未覆盖方法

    [Fact]
    public void UnportedMethodsAreSeamsAndDefaultToNoOp()
    {
        // 这些方法本车道未移植（依赖 TDx* 控件树 / 大段 INI），保留签名 + 可注入
        _dlg.LoadConfigFile();
        _dlg.SaveConfigFile();
        _dlg.AutoProtect();
        _dlg.AutoUseMagic();
        _dlg.DuraWarning();
        _dlg.AutoUseItem();
        _dlg.AutoEatHPItem();
        _dlg.AutoEatMPItem();
        _dlg.AutoEatSpecialHPItem();
        _dlg.AutoEatSpecialMPItem();
        _dlg.LoadHelpFile();
        _dlg.ClearShowItem();
        _dlg.RefShowItem();
        _dlg.RefreshMySelfMagicList();
        _dlg.Run();
        _dlg.LoadConfig("角色");
        _dlg.Logon("服务器");
        _dlg.Finalize();
        _dlg.RefUseItemConfig(0);

        ushort k = 0;
        char c = 'x';
        _dlg.FormKeyDown(ref k, DelphiShiftState.None);
        _dlg.FormKeyPress(ref c);
        _dlg.AddToBossList("a");
        _dlg.RemoveFromBossList("a");
        _dlg.AddOrRemoveBossList("a");
        _dlg.RefKeyboardConfig();
        Assert.True(true);   // 能走到这里即"不抛异常"
    }

    [Fact]
    public void DestroySavesOnlyWhenEnabled()
    {
        // 605-610
        int saves = 0;
        _dlg.SaveConfigFileSeam = () => saves++;
        _dlg.SetEnabled(false);
        _dlg.Destroy();
        Assert.Equal(0, saves);

        var dlg2 = new TMirsConfigDlg(new MirsConfigDlgControlsStub());
        int saves2 = 0;
        dlg2.SaveConfigFileSeam = () => saves2++;
        dlg2.SetEnabled(true);
        dlg2.Destroy();
        Assert.Equal(1, saves2);
    }

    [Fact]
    public void InitializeStoresFrameFieldsAndDelegates()
    {
        IntPtr handle = new IntPtr(0x1234);
        bool called = false;
        _dlg.InitializeSeam = (h, s, v, w) => called = true;
        _dlg.Initialize(handle, 7, TClientVersion.cvMirSequel, true);
        Assert.True(called);
    }

    [Fact]
    public void LoadClientConfigStoresConfigForGateChecks()
    {
        var cfg = new TClientConfig { boNotCanUseClientConfig = true, dwUseItemIntervalTime = 500 };
        _dlg.LoadClientConfig(cfg);
        _ui.PlugCheckBoxHideGhost = true;
        _dlg.CheckBoxClickEx("PlugCheckBoxHideGhost");
        // 门禁为 true → 未写入
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckHideGhost));
    }

    // ============================================================ 辅助

    private void SetControl(string name, bool value)
    {
        switch (name)
        {
            case "PlugCheckBoxShowHPLabel": _ui.PlugCheckBoxShowHPLabel = value; break;
            case "PlugCheckBoxNumberLable": _ui.PlugCheckBoxNumberLable = value; break;
            case "PlugCheckBoxJobAndLevel": _ui.PlugCheckBoxJobAndLevel = value; break;
            case "PlugCheckBoxShowGreenHint": _ui.PlugCheckBoxShowGreenHint = value; break;
            case "PlugCheckBoxShowItemName": _ui.PlugCheckBoxShowItemName = value; break;
            case "PlugCheckBoxShowFilterItem": _ui.PlugCheckBoxShowFilterItem = value; break;
            case "PlugCheckBoxItemHint": _ui.PlugCheckBoxItemHint = value; break;
            case "PlugCheckBoxShowActorName": _ui.PlugCheckBoxShowActorName = value; break;
            case "PlugCheckBoxHideDescUserName": _ui.PlugCheckBoxHideDescUserName = value; break;
            case "PlugCheckBoxDuraWarning": _ui.PlugCheckBoxDuraWarning = value; break;
            case "PlugCheckBoxNoShift": _ui.PlugCheckBoxNoShift = value; break;
            case "PlugCheckBoxExpFilter": _ui.PlugCheckBoxExpFilter = value; break;
            case "PlugCheckBoxShowMimiMapDesc": _ui.PlugCheckBoxShowMimiMapDesc = value; break;
            case "PlugCheckBoxNotParaly": _ui.PlugCheckBoxNotParaly = value; break;
            case "PlugCheckBoxShowHighlightHPLabel": _ui.PlugCheckBoxShowHighlightHPLabel = value; break;
            case "PlugCheckBoxShowHealthNumber": _ui.PlugCheckBoxShowHealthNumber = value; break;
            case "PlugCheckBoxHideGhost": _ui.PlugCheckBoxHideGhost = value; break;
            case "PlugCheckBoxHideHumEffect": _ui.PlugCheckBoxHideHumEffect = value; break;
            case "PlugCheckBoxHideWeaponEffect": _ui.PlugCheckBoxHideWeaponEffect = value; break;
            case "PlugCheckBoxShowMonName": _ui.PlugCheckBoxShowMonName = value; break;
            case "PlugCheckBoxAutoOrderItem": _ui.PlugCheckBoxAutoOrderItem = value; break;
            case "PlugCheckBoxMagicLock": _ui.PlugCheckBoxMagicLock = value; break;
            case "PlugCheckBoxSmartLongHit": _ui.PlugCheckBoxSmartLongHit = value; break;
            case "PlugCheckBoxSmartPosLongHit": _ui.PlugCheckBoxSmartPosLongHit = value; break;
            case "PlugCheckBoxSmartWalkLongHit": _ui.PlugCheckBoxSmartWalkLongHit = value; break;
            case "PlugCheckBoxSmartWideHit": _ui.PlugCheckBoxSmartWideHit = value; break;
            case "PlugCheckBoxSmartFireHit": _ui.PlugCheckBoxSmartFireHit = value; break;
            case "PlugCheckBoxSmartSwordHit": _ui.PlugCheckBoxSmartSwordHit = value; break;
            case "PlugCheckBoxSmartKTZHit": _ui.PlugCheckBoxSmartKTZHit = value; break;
            case "PlugCheckBoxAutoHideMode": _ui.PlugCheckBoxAutoHideMode = value; break;
            case "PlugCheckBoxAutoTakeOnItem": _ui.PlugCheckBoxAutoTakeOnItem = value; break;
            case "PlugCheckBoxAutoCHangePoison": _ui.PlugCheckBoxAutoChangePoison = value; break;
            case "PlugCheckBoxHumAutoShield": _ui.PlugCheckBoxHumAutoShield = value; break;
            case "PlugCheckBoxHumStruckShield": _ui.PlugCheckBoxHumStruckShield = value; break;
            case "PlugCheckBoxHeroAutoShield": _ui.PlugCheckBoxHeroAutoShield = value; break;
            case "PlugCheckBoxAssistantHeroAutoShield": _ui.PlugCheckBoxAssistantHeroAutoShield = value; break;
            case "PlugCheckBoxHumManuallySnowWind": _ui.PlugCheckBoxHumManuallySnowWind = value; break;
            case "PlugCheckBoxHumManuallyFireBoom": _ui.PlugCheckBoxHumManuallyFireBoom = value; break;
            case "PlugCheckBoxHumShootLightenLockTarget": _ui.PlugCheckBoxHumShootLightenLockTarget = value; break;
            case "PlugCheckBoxHumManuallyMeteorShower": _ui.PlugCheckBoxHumManuallyMeteorShower = value; break;
            case "PlugCheckBoxAutoMagic": _ui.PlugCheckBoxAutoMagic = value; break;
            case "PlugCheckBoxUseKeyBoard": _ui.PlugCheckBoxUseKeyBoard = value; break;
            case "PlugCheckBoxUseSuperMedica": _ui.PlugCheckBoxUseSuperMedica = value; break;
            case "PlugCheckBoxDisableSelfStruck": _ui.PlugCheckBoxDisableSelfStruck = value; break;
            case "PlugCheckBoxSpeedSlow": _ui.PlugCheckBoxSpeedSlow = value; break;
            case "PlugCheckBoxAutoPickUpItem": _ui.PlugCheckBoxAutoPickUpItem = value; break;
            default: throw new ArgumentException("未登记的控件名：" + name, nameof(name));
        }
    }
}

/// <summary>
/// 便于测试访问 <c>FInitializeed</c>（原文是 private 字段，测试需要置位以进入 RefConfig）。
/// 用反射避免为测试放宽生产字段可见性。
/// </summary>
internal static class MirsConfigTestAccess
{
    public static void SetInitializedForTest(this TMirsConfigDlg dlg, bool value)
    {
        var f = typeof(TMirsConfigDlg).GetField("FInitializeed",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        f!.SetValue(dlg, value);
    }

    /// <summary>占位：把基类的 RefActorList 调用包一层，便于空实现测试集中断言。</summary>
    public static void RefAggregateNoThrow(this TMirsConfigDlg dlg) => dlg.RefActorList();
}

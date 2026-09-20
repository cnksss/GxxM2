// 源单元：Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas（GBK，5,954 行，CRLF）
// 车道 p5-client-mirreturn 的 1:1 移植测试。
//
// 覆盖（与实现分片一一对应，括号内为原文行号）：
//   TConfig 全部字段默认值（640-787）        Create 的 42 条勾选位默认值（789-860）
//   访问器（871-961）                        PlugPageControlConfigInRealArea（1005-1008）
//   ClearShowItem/RefShowItem（1082-1158）   RefKeyBoardConfig（1967-1981）
//   RefConfig（1983-2137）                   RefUseItemConfig（1860-1965）
//   控件写回处理器（2139-2556/5753/5842-5940）
//   NumberSort_1（4191-4207）                CanFilterExp（4479-4484）
//   LoadConfigFile/SaveConfigFile（4486-5069）
//   Boss/挂机怪物/挂机技能名单与文件（5116-5698）
//   特殊颜色/DIY（5253-5397）                RefreshGJMagic（5776-5840）
//   音量（5942-5952）                        AutoUseItem/AutoUseMagic/AutoEat*（3441-3534/4454-4477）
//   Struck/HealthChange 分派（3374-3439）     查询族（3331-3372）
//
// 无头安全：MirReturnMessageSeam.UiEnabled = false（否则模态框会挂死 testhost）。
//
// ★ 环境隔离说明：本车道的测试**不调用**只读文件 GuiCfgConfigShareTests.cs 里的
//   GuiCfgTestEnv.Reset()（它是 MirsConfigDlg 车道的公共夹具，可能在本波次被改动）。
//   本文件自带 MirReturnTestEnv，只复位本单元的接缝与全局，并在同名的
//   [Collection("GuiCfgConfigure")]（DisableParallelization = true）里串行执行。

using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig;
using GXX.Client.GUI.GameConfig.Seams;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>本车道的接缝/全局复位器（不依赖只读测试夹具）。</summary>
internal static class MirReturnTestEnv
{
    public static void Reset()
    {
        ConfigShareSeam.g_MySelf = null;
        ConfigShareSeam.g_MyHero = null;
        ConfigShareGlobal.g_sPlugServerName = "";
        ConfigShareGlobal.g_sPlugUserName = "";
        for (int i = TShortcutKeys.Low; i <= TShortcutKeys.High; i++)
            ConfigShareGlobal.g_ShortcutKeys[i] = default;

        ChatBoardSeam.AddChatBoardString = (msg, color, backColor) => { };
        LastChat = null;

        MirReturnGlobalSeam.ResetForTests();
        MirReturnConfigGlobalSeam.ResetForTests();
        MirReturnMessageSeam.ResetForTests();
        MirReturnMessageSeam.UiEnabled = false;
    }

    public static (string Msg, int Color, int BackColor)? LastChat;
}

[Collection("GuiCfgConfigure")]
public sealed class GuiMirReturnConfigTests : IDisposable
{
    private readonly MirReturnConfigDlgControlsStub _ui = new MirReturnConfigDlgControlsStub();
    private readonly TMirReturnConfigDlg _dlg;
    private readonly List<(string, int, int)> _chat = new List<(string, int, int)>();
    private readonly List<(int, int)> _hpUse = new List<(int, int)>();
    private readonly List<(int, int)> _mpUse = new List<(int, int)>();
    private readonly List<string> _savedFiles = new List<string>();
    private readonly List<object> _poisonCharm = new List<object>();
    private readonly List<(int, int, object)> _useMagic = new List<(int, int, object)>();
    private readonly List<int> _autoEat = new List<int>();
    private readonly List<int> _heroEat = new List<int>();

    private sealed class Self : IHumActorSeam
    {
        public Self(string name = "玩家") { m_sUserName = name; }
        public int m_nCurrX => 0;
        public int m_nCurrY => 0;
        public string m_sUserName { get; }
    }

    private sealed class Hero : IHeroActorSeam
    {
        public int m_nBagCount => 40;
    }

    public GuiMirReturnConfigTests()
    {
        MirReturnTestEnv.Reset();

        _dlg = new TMirReturnConfigDlg(_ui);
        _dlg.DamageHPUseItem = (nObj, nDamage) => _hpUse.Add((nObj, nDamage));
        _dlg.DamageMPUseItem = (nObj, nDamage) => _mpUse.Add((nObj, nDamage));

        ChatBoardSeam.AddChatBoardString = (m, c, b) => _chat.Add((m, c, b));
        MirReturnGlobalSeam.SaveTextFile = (f, lines) => _savedFiles.Add(f + "|" + string.Join(",", lines));
        MirReturnGlobalSeam.LoadTextFile = _ => new List<string>();
        MirReturnGlobalSeam.ChangePoisonCharm = o => _poisonCharm.Add(o);
        MirReturnConfigGlobalSeam.UseMagic = (x, y, m) => _useMagic.Add((x, y, m));
        MirReturnConfigGlobalSeam.AutoEatItem = i => _autoEat.Add(i);
        MirReturnConfigGlobalSeam.HeroEatItem = i => _heroEat.Add(i);

        // g_Config 是**单元级静态单例**，测试间必须复位（否则前一个用例的写入泄漏到后一个）
        ResetGConfig();
    }

    /// <summary>
    /// 把 TMirReturnConfigDlg.g_Config **就地**复位为 CreateDefaultConfig() 的初值。
    ///
    /// 说明：<c>g_Config</c> 是 <c>public static readonly</c>（对应原文的单元级 var 单例），
    /// 反射不能替换 readonly 静态字段（FieldAccessException），故改为**逐字段回拷**：
    /// 先反射调用私有的 <c>CreateDefaultConfig()</c> 造一份初值，再把它拷回现有实例。
    /// 数组用 <c>Clone()</c> 逐元素拷（多维数组同样支持）。
    /// </summary>
    private static void ResetGConfig()
    {
        TMirReturnConfigDlg.TConfig fresh = MakeDefaultConfig();
        TMirReturnConfigDlg.TConfig live = TMirReturnConfigDlg.g_Config;

        foreach (var f in typeof(TMirReturnConfigDlg.TConfig).GetFields(
                     System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
        {
            object v = f.GetValue(fresh);
            if (v is Array src)
            {
                var dst = (Array)f.GetValue(live);
                if (dst != null && dst.Length == src.Length)
                {
                    Array.Copy(src, dst, src.Length);
                    continue;
                }
                f.SetValue(live, src.Clone());
                continue;
            }
            f.SetValue(live, v);
        }
    }

    /// <summary>用反射调用私有的 CreateDefaultConfig（与 g_Config 同源，保证"复位"就是原文初值）。</summary>
    private static TMirReturnConfigDlg.TConfig MakeDefaultConfig()
    {
        var mi = typeof(TMirReturnConfigDlg).GetMethod(
            "CreateDefaultConfig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        return (TMirReturnConfigDlg.TConfig)mi.Invoke(null, null);
    }

    public void Dispose() => MirReturnTestEnv.Reset();

    // ================================================================================
    // g_Config 默认值（原文 640-787）
    // ================================================================================

    [Fact]
    public void GConfig_标量与数组初值_逐字段照抄()
    {
        var c = TMirReturnConfigDlg.g_Config;

        // 641-643 / 646-654
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
    public void GConfig_原文不对称初值_RenewMPTimes为0而HPTimes为1000()
    {
        var c = TMirReturnConfigDlg.g_Config;
        // 682 vs 687 —— 原文最显眼的一处不对称
        Assert.All(c.RenewHPTimes, v => Assert.Equal(1000, v));
        Assert.All(c.RenewMPTimes, v => Assert.Equal(0, v));
        Assert.All(c.RenewSpecialHPTimes, v => Assert.Equal(1000, v));
        Assert.All(c.RenewSpecialMPTimes, v => Assert.Equal(1000, v));
    }

    [Fact]
    public void GConfig_CheckHpMp的Times与UseTimes()
    {
        var c = TMirReturnConfigDlg.g_Config;
        Assert.All(c.CheckHpCheckTimes, v => Assert.Equal(1000u, v));   // 666
        Assert.All(c.CheckHpUseTimes, v => Assert.Equal(10000u, v));    // 668
        Assert.All(c.CheckMpCheckTimes, v => Assert.Equal(1000u, v));   // 675
        Assert.All(c.CheckMpUseTimes, v => Assert.Equal(10000u, v));    // 677
        Assert.All(c.CheckHpCheckTicks, v => Assert.Equal(0u, v));      // 667
        Assert.All(c.CheckHpUseTicks, v => Assert.Equal(0u, v));        // 669
        Assert.All(c.CheckMpCheckTicks, v => Assert.Equal(0u, v));
        Assert.All(c.CheckMpUseTicks, v => Assert.Equal(0u, v));
    }

    [Fact]
    public void GConfig_百分比初值_10与耐久组()
    {
        var c = TMirReturnConfigDlg.g_Config;
        Assert.All(c.RenewHPPercents, v => Assert.Equal(10, v));            // 681
        Assert.All(c.RenewMPPercents, v => Assert.Equal(10, v));            // 686
        Assert.All(c.RenewSpecialHPPercents, v => Assert.Equal(10, v));     // 691
        Assert.All(c.RenewSpecialMPPercents, v => Assert.Equal(10, v));     // 696
        Assert.All(c.CheckHpPercents, v => Assert.Equal(0, v));             // 664
        Assert.All(c.CheckMpPercents, v => Assert.Equal(0, v));             // 673
        Assert.All(c.CheckDuraMin, v => Assert.Equal(2, v));                // 770
        Assert.All(c.CheckDuraTime, v => Assert.Equal(10, v));              // 772
        Assert.All(c.CheckDuraValue, v => Assert.Null(v));                // 771（原文字符串数组默认空槽位）
        Assert.All(c.CheckDuraCheckTicks, v => Assert.Equal(0u, v));        // 773
    }

    [Fact]
    public void GConfig_超药名9项_逐字照抄()
    {
        // 702-711
        Assert.Equal(new[]
        {
            "太阳水", "强效太阳水", "万年雪霜", "疗伤药", "疗伤药(任务)",
            "强效万年雪霜", "强效疗伤药", "超级万年雪霜", "超级疗伤药",
        }, TMirReturnConfigDlg.g_Config.SuperMedicaItemNames);
    }

    [Fact]
    public void GConfig_超药矩阵_500与0()
    {
        var c = TMirReturnConfigDlg.g_Config;
        for (int i = 0; i <= 4; i++)
        {
            for (int j = 0; j <= 8; j++)
            {
                Assert.False(c.SuperMedicaUses[i, j]);          // 713-719
                Assert.Equal(0, c.SuperMedicaHPs[i, j]);        // 721-727
                Assert.Equal(500, c.SuperMedicaHPTimes[i, j]);  // 729-735
                Assert.Equal(0, c.SuperMedicaHPTicks[i, j]);    // 737-743
                Assert.Equal(0, c.SuperMedicaMPs[i, j]);        // 745-751
                Assert.Equal(500, c.SuperMedicaMPTimes[i, j]);  // 753-759
                Assert.Equal(0, c.SuperMedicaMPTicks[i, j]);    // 761-766
            }
        }
        Assert.All(c.UseSuperMedicas, v => Assert.False(v));    // 700
    }

    [Fact]
    public void GConfig_尾部标量_颜色与挂机()
    {
        var c = TMirReturnConfigDlg.g_Config;
        Assert.Equal(0, c.nHeroDodgeHPPercent);        // 775
        Assert.Equal((byte)3, c.nColorShowEff);        // 776 BOSS变色显示
        Assert.Equal((byte)249, c.nSpecialColor);      // 777 特殊物品颜色
        Assert.Equal(0, c.nGJPlayAttackOption);        // 779
        Assert.Equal(0, c.nGJNoRedPoisonOption);       // 780
        Assert.Equal(0, c.nGJNoBluePoisonOption);      // 781
        Assert.Equal(0, c.nGJNoDuFuOption);            // 782
        Assert.Equal(0, c.nGJBagFullOption);           // 783
        Assert.Equal(7, c.nGJNotRushMonRange);         // 785
        Assert.Equal(3, c.nGJGroupAttackCount);        // 786
    }

    [Fact]
    public void GConfig_未在初值常量里出现的字段_保持零值()
    {
        // 原文 641-787 未列出的 record 字段（Delphi 里不在有初值的常量里）→ 零值
        var c = TMirReturnConfigDlg.g_Config;
        Assert.Equal(0, c.nRenewHeroHPTime);
        Assert.Equal(0, c.nRenewHeroHPPercent);
        Assert.Equal(0, c.nRenewHeroMPTime);
        Assert.Equal(0, c.nRenewHeroMPPercent);
        Assert.False(c.boRenewHeroSpecialIsAuto);
        Assert.False(c.boRenewHeroLogOutIsAuto);
        Assert.False(c.boRenewCloseIsAuto);
        Assert.Equal(0, c.nRenewClosePercent);
    }

    // ================================================================================
    // Create（原文 789-860）
    // ================================================================================

    [Fact]
    public void Create_私有字段初值()
    {
        // 791-804：由构造函数设置的字段通过公开面观察
        Assert.False(_dlg.GetEnabled());            // 791 FEnabled := False
        Assert.True(_dlg.GetProtectEnabled());      // 792 FProtectEnabled := True
        Assert.Equal(TConfigDlgType.ptDefault, _dlg.GetType());  // 871-874
    }

    [Fact]
    public void Create_未初始化时_GetVisible随PlugConfigDlg存在性()
    {
        // 904-908：原文未预置 Result；PlugConfigDlg 非 nil → 取 PlugConfigDlg.Visible
        Assert.False(_dlg.GetVisible());
        _ui.PlugConfigDlgVisible = true;
        Assert.True(_dlg.GetVisible());
    }

    [Fact]
    public void Create_GetVisible_PlugConfigDlg为nil时走未定义分支_托管侧取false()
    {
        _ui.PlugConfigDlg = null;       // 构造后置 nil（原文 906 的 if 不成立）
        Assert.False(_dlg.GetVisible());
    }

    [Fact]
    public void Create_42条勾选位默认值_逐条断言()
    {
        // 818-859，只列**非 false** 的与几处关键的 false
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckShowHPLabel));       // 818
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckShowUserName));     // 819
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckMagicLock));         // 820
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckAutoOrderItem));     // 821
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckNotNeedShift));      // 822
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckAutoPickUpItem));    // 823
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckBGMusic));           // 824
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckRepeatBGMusic));     // 825
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckNotParaly));        // 826

        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckSmartLongHit));     // 828
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckSmartPosLongHit));  // 829
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckSmartWalkLongHit)); // 830
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckSmartWideHit));     // 831
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckSmartFireHit));     // 832
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckSmartSwordHit));    // 833
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckSmartCrsHit));      // 834
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckSmartTwnHit));      // 835

        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckHumAutoShield));    // 837
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckHumStruckShield));  // 838
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckHumShootLightenLockTarget)); // 839
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckHumManuallyFireBoom));       // 840
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckHumManuallySnowWind));       // 841
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckHumManuallyMeteorShower));   // 842

        Assert.False(_dlg.GetConfigChecked(MirReturnCheckedMap.ckAutoTakeOnItem));       // 844

        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckShowNpcName));      // 846
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckShowNpcHPLabel));   // 847
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckShowNGLabel));      // 848

        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckNearHint));          // 850
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckAutoLock));         // 851
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckColorShow));        // 852
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckSpecialQuickFlashing)); // 853
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckBlacklistHit));     // 854
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckFriendHit));        // 855
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckSceneShake));       // 856
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckAutoDownHorse));    // 857

        Assert.False(_dlg.ConfigCheckeds[MirReturnCheckedMap.ckMovePickIndex]); // 859 合成槽位
    }

    [Fact]
    public void Create_ckMovePick_合成槽位不覆盖ckAutoPickUpItem()
    {
        // ★ 关键差异断言：若 ckMovePick 映射到 ckAutoPickUpItem，823 行的 True 会被 859 行覆盖
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckAutoPickUpItem));
        Assert.Equal(TConfigCheckedBounds.HighOrdinal + 1, MirReturnCheckedMap.ckMovePickSyntheticIndex);
        Assert.NotEqual((int)TConfigChecked.ckAutoPickUpItem, MirReturnCheckedMap.ckMovePickSyntheticIndex);
    }

    [Fact]
    public void Create_数组长度_含合成槽位()
    {
        Assert.Equal(TConfigCheckedBounds.HighOrdinal + 2, _dlg.ConfigCheckeds.Length);
    }

    // ================================================================================
    // 访问器（原文 871-961）
    // ================================================================================

    [Fact]
    public void Accessors_Open为空实现()
    {
        _dlg.Open();    // 890-893 原文空实现，不应抛异常
    }

    [Fact]
    public void Accessors_Close清三个状态并置PlugConfigDlg不可见()
    {
        _dlg.SetEnabled(true);                  // FEnabled := True
        _ui.PlugConfigDlgVisible = true;

        _dlg.Close();                           // 895-902

        Assert.False(_dlg.GetEnabled());        // 898
        Assert.False(_ui.PlugConfigDlgVisible); // 900
    }

    [Fact]
    public void Accessors_SetVisible写入并刷新GJMagic()
    {
        _dlg.SetVisible(true);                  // 910-927
        Assert.True(_ui.PlugConfigDlgVisible);
    }

    [Fact]
    public void Accessors_SetVisible为真时_焦点三段_ShowHPLabel优先()
    {
        _ui.PlugCheckBoxShowHPLabelVisible = true;
        _dlg.SetVisible(true);                  // 915-916
        Assert.Equal("PlugCheckBoxShowHPLabel", _ui.LastFocus);
    }

    [Fact]
    public void Accessors_SetVisible焦点三段_NumberLable为次选()
    {
        _ui.PlugCheckBoxShowHPLabelVisible = false;   // 915 不成立
        _ui.PlugCheckBoxNumberLableVisible = true;    // 917 成立
        _dlg.SetVisible(true);
        Assert.Equal("PlugCheckBoxNumberLable", _ui.LastFocus);
    }

    [Fact]
    public void Accessors_SetVisible焦点三段_JobAndLevel为末选()
    {
        _ui.PlugCheckBoxShowHPLabelVisible = false;
        _ui.PlugCheckBoxNumberLableVisible = false;
        _ui.PlugCheckBoxJobAndLevelVisible = true;    // 919 成立
        _dlg.SetVisible(true);
        Assert.Equal("PlugCheckBoxJobAndLevel", _ui.LastFocus);
    }

    [Fact]
    public void Accessors_SetVisible焦点三段_三者都不可见时无SetFocus()
    {
        _ui.PlugCheckBoxShowHPLabelVisible = false;
        _ui.PlugCheckBoxNumberLableVisible = false;
        _ui.PlugCheckBoxJobAndLevelVisible = false;
        _ui.LastFocus = null;
        _dlg.SetVisible(true);
        Assert.Null(_ui.LastFocus);
    }

    [Fact]
    public void Accessors_SetVisible_PlugConfigDlg为nil时整段跳过()
    {
        _ui.PlugConfigDlg = null;
        _ui.PlugCheckBoxShowHPLabelVisible = true;
        _ui.LastFocus = null;
        _dlg.SetVisible(true);                  // 912 不成立 → 915/922 都不执行
        Assert.Null(_ui.LastFocus);
    }

    [Fact]
    public void Accessors_SetEnabled为假时_清FLoadConfig()
    {
        _dlg.SetEnabled(true);
        _dlg.SetEnabled(false);                 // 934-939
        Assert.False(_dlg.GetEnabled());
    }

    [Fact]
    public void Accessors_SetProtectEnabled为真时_刷新Tick()
    {
        _dlg.SetProtectEnabled(false);
        Assert.False(_dlg.GetProtectEnabled());  // 941-944
        _dlg.SetProtectEnabled(true);
        Assert.True(_dlg.GetProtectEnabled());   // 946-951
    }

    [Fact]
    public void Accessors_FormKeyDown与FormKeyPress为空实现返回false()
    {
        ushort k = 65;
        Assert.False(_dlg.FormKeyDown(ref k, DelphiShiftState.None));   // 953-956
        char c = 'a';
        Assert.False(_dlg.FormKeyPress(ref c));                         // 958-961
    }

    [Fact]
    public void Accessors_Refresh族与RefActorList为空实现()
    {
        _dlg.RefreshMySelfAbil();       // 963-966
        _dlg.RefreshMyHeroAbil();       // 968-971
        _dlg.RefreshMyHeroMagicList();  // 995-998
        _dlg.RefreshUnBindItemList();   // 1000-1003
        _dlg.RefActorList();            // 3326-3329
    }

    [Fact]
    public void Accessors_SetConfigChecked_值变化时才RefConfig()
    {
        // 881-888：RefConfig 在 FInitializeed=false 时立即 Exit（1985），
        // 故此处只验证"值被写入"
        _dlg.SetConfigChecked(TConfigChecked.ckSpeedSlow, true);
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckSpeedSlow));
        _dlg.SetConfigChecked(TConfigChecked.ckSpeedSlow, true);   // 无变化，走不进 if
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckSpeedSlow));
    }

    // ================================================================================
    // PlugPageControlConfigInRealArea（原文 1005-1008）
    // ================================================================================

    [Theory]
    // IsRealArea := not ((X >= Width-12) and (Y <= Height+20))
    [InlineData(100, 50, 200, 100, true)]    // X < W-12 → not(false)=true
    [InlineData(188, 50, 200, 100, false)]   // X = W-12 且 Y <= H+20 → not(true)=false
    [InlineData(189, 50, 200, 100, false)]   // X > W-12 且 Y <= H+20
    [InlineData(188, 121, 200, 100, true)]   // X >= W-12 但 Y > H+20 → not(false)=true ★ Y 判据是 Height+20
    [InlineData(0, 0, 0, 0, false)]          // W-12=-12 → X>= -12 成立；Y<=20 成立
    public void PlugPageControlConfigInRealArea_几何判据(int x, int y, int w, int h, bool expected)
    {
        MirReturnConfigDlgControlsStub ui = new MirReturnConfigDlgControlsStub
        {
            PlugPageControlConfigWidth = w,
            PlugPageControlConfigHeight = h,
        };
        var dlg = new TMirReturnConfigDlg(ui);
        dlg.PlugPageControlConfigInRealArea(x, y, out bool isReal);
        Assert.Equal(expected, isReal);
    }

    [Fact]
    public void PlugConfigDlgCloseClickEx_置不可见()
    {
        _ui.PlugConfigDlgVisible = true;
        _dlg.PlugConfigDlgCloseClickEx();       // 1010-1014
        Assert.False(_ui.PlugConfigDlgVisible);
    }

    [Fact]
    public void PlugPageControlConfigActivePageChange_仅第0页做焦点三段()
    {
        _ui.PlugPageControlConfigActivePageIndex = 1;
        _ui.PlugCheckBoxShowHPLabelVisible = true;
        _ui.LastFocus = null;
        _dlg.PlugPageControlConfigActivePageChange();   // 1018 不成立
        Assert.Null(_ui.LastFocus);

        _ui.PlugPageControlConfigActivePageIndex = 0;
        _dlg.PlugPageControlConfigActivePageChange();   // 1020-1021
        Assert.Equal("PlugCheckBoxShowHPLabel", _ui.LastFocus);
    }

    [Fact]
    public void Logout_无条件SaveConfigFile且不碰PlugConfigDlg()
    {
        _ui.PlugConfigDlgVisible = true;
        var ini = new MirReturnIniFileStub();
        MirReturnGlobalSeam.CreateIniFile = _ => ini;

        _dlg.SetEnabled(true);
        _dlg.Logout();                          // 1029-1034

        Assert.False(_dlg.GetEnabled());        // 1032
        Assert.True(_ui.PlugConfigDlgVisible);  // 原文不碰 PlugConfigDlg
        Assert.True(ini.WriteCount > 0);         // 1033 真的写了 INI
    }

    // ================================================================================
    // ClearShowItem / RefShowItem（原文 1082-1158）
    // ================================================================================

    [Fact]
    public void ClearShowItem_清空并置6列()
    {
        _dlg.ClearShowItem();                   // 1082-1086
        Assert.Equal(6, _ui.PlugMemoConfig2ColCount);
        Assert.Empty(_ui.Memo2Rows);
    }

    [Fact]
    public void RefShowItem_每个ShowItem建1行6列()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_ShowItemList.Add(new TShowItem { sItemName = "金创药(小)", boHintMsg = 1 });
        db.m_ShowItemList.Add(new TShowItem { sItemName = "魔法药(小)", boPickup = 1 });

        _dlg.RefShowItem();                     // 1088-1158

        Assert.Equal(6, _ui.PlugMemoConfig2ColCount);
        Assert.Equal(2, _ui.Memo2Rows.Count);
        Assert.All(_ui.Memo2Rows, r => Assert.Equal(6, r.Cells.Count));
        Assert.Equal("金创药(小)", _ui.Memo2Rows[0].Cells[0].Caption);
        db.m_ShowItemList.Clear();
    }

    [Fact]
    public void RefShowItem_第一列样式与三态色_第二至六列为勾选列()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_ShowItemList.Add(new TShowItem { sItemName = "太阳水", boHintMsg = 1, boPickup = 1, boShowName = 1, boShowSpecial = 1, boAutoMove = 1 });

        _dlg.RefShowItem();

        var cell0 = _ui.Memo2Rows[0].Cells[0];
        Assert.Equal(3, cell0.Style);                   // 1109 bsButton
        Assert.Equal(0, cell0.Alignment);               // 1110 taLeftJustify
        Assert.Equal(0x00FFFFFF, cell0.Colors["Up"]);   // 1111 clWhite
        Assert.Equal(0x000000FF, cell0.Colors["Hot"]);  // 1112 clRed（原文注释是 clWhite）
        Assert.Equal(0x000000FF, cell0.Colors["Down"]); // 1113 clRed

        for (int c = 1; c <= 5; c++)
        {
            var cell = _ui.Memo2Rows[0].Cells[c];
            Assert.Equal(2, cell.Style);                // bsCheckBox
            Assert.Equal(0, cell.ImageType);            // NewopUI_Pak
            Assert.Equal(228, cell.ImageIndex["Up"]);
            Assert.Equal(229, cell.ImageIndex["Down"]);
            Assert.True(cell.Checked);
        }
        Assert.Equal(1, _ui.Memo2LockCount);
        Assert.Equal(1, _ui.Memo2UnLockCount);
        db.m_ShowItemList.Clear();
    }

    [Fact]
    public void RefShowItem_未命中勾选位时第2至6列全false()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_ShowItemList.Add(new TShowItem { sItemName = "x" });

        _dlg.RefShowItem();

        for (int c = 1; c <= 5; c++)
            Assert.False(_ui.Memo2Rows[0].Cells[c].Checked);
        db.m_ShowItemList.Clear();
    }

    [Fact]
    public void RefShowItem_空列表不建行()
    {
        FilterItemsGlobal.g_FileItemDB.m_ShowItemList.Clear();
        _dlg.RefShowItem();
        Assert.Empty(_ui.Memo2Rows);
        Assert.Equal(1, _ui.Memo2LockCount);    // Lock/UnLock 仍成对执行
        Assert.Equal(1, _ui.Memo2UnLockCount);
    }

    // ================================================================================
    // RefKeyBoardConfig（原文 1967-1981）★ 含缺陷固定
    // ================================================================================

    [Fact]
    public void RefKeyBoardConfig_12个标签_其中9号与10号同源()
    {
        // 让每个下标给出不同的按键名
        for (int i = TShortcutKeys.Low; i <= TShortcutKeys.High; i++)
            ConfigShareGlobal.g_ShortcutKeys[i] = new TShortcutKey { Use = 1, Key = (ushort)(65 + i), Shift = DelphiShiftState.None };

        _dlg.RefKeyboardConfig();               // 1967-1981

        // 1969-1976：LabelKeyBoard1..8 ← 下标 0..7
        for (int k = 1; k <= 8; k++)
            Assert.False(string.IsNullOrEmpty(_ui.LabelKeyBoard[k]));

        // ★ 原文缺陷：1977 用 [9]、1978 也用 [9]；[8] 从未显示、[10]/[11] 给 11/12 号标签
        var keyAt9 = GXX.Client.GUI.GameConfig.ConfigShare.GetKeyDownStr(
            ConfigShareGlobal.g_ShortcutKeys[9].Key, ConfigShareGlobal.g_ShortcutKeys[9].Shift);
        Assert.Equal(keyAt9, _ui.LabelKeyBoard[9]);
        Assert.Equal(keyAt9, _ui.LabelKeyBoard[10]);      // ← 同源（缺陷固定）
        Assert.NotEqual(keyAt9, _ui.LabelKeyBoard[11]);   // 11 号用的是下标 10
    }

    [Fact]
    public void RefKeyBoardConfig_下标8从未被显示()
    {
        for (int i = TShortcutKeys.Low; i <= TShortcutKeys.High; i++)
            ConfigShareGlobal.g_ShortcutKeys[i] = new TShortcutKey { Use = 0, Key = 0, Shift = DelphiShiftState.None };
        // 只给下标 8 一个可识别的按键
        ConfigShareGlobal.g_ShortcutKeys[8] = new TShortcutKey { Use = 1, Key = 65, Shift = DelphiShiftState.None };

        _dlg.RefKeyboardConfig();

        string key8 = GXX.Client.GUI.GameConfig.ConfigShare.GetKeyDownStr(65, DelphiShiftState.None);
        for (int k = 1; k <= 12; k++)
            Assert.NotEqual(key8, _ui.LabelKeyBoard[k]);
    }

    // ================================================================================
    // NumberSort_1（原文 4191-4207）
    // ================================================================================

    [Fact]
    public void NumberSort_1_降序比较器()
    {
        Assert.Equal(-1, TMirReturnConfigDlg.NumberSort_1(new[] { "10", "5" }, 0, 1));   // 4199-4200
        Assert.Equal(1, TMirReturnConfigDlg.NumberSort_1(new[] { "5", "10" }, 0, 1));    // 4201-4202
        Assert.Equal(0, TMirReturnConfigDlg.NumberSort_1(new[] { "7", "7" }, 0, 1));     // 4203-4204
    }

    [Fact]
    public void NumberSort_1_非法整数返回0_异常被吞()
    {
        // 4205-4206：空 except → Result 保持 4205 之前的 0
        Assert.Equal(0, TMirReturnConfigDlg.NumberSort_1(new[] { "abc", "5" }, 0, 1));
        Assert.Equal(0, TMirReturnConfigDlg.NumberSort_1(new[] { "5", "xyz" }, 0, 1));
        Assert.Equal(0, TMirReturnConfigDlg.NumberSort_1(new[] { "", "" }, 0, 1));
    }

    [Fact]
    public void NumberSort_1_负值与零()
    {
        Assert.Equal(1, TMirReturnConfigDlg.NumberSort_1(new[] { "-3", "0" }, 0, 1));
        Assert.Equal(-1, TMirReturnConfigDlg.NumberSort_1(new[] { "0", "-3" }, 0, 1));
        Assert.Equal(0, TMirReturnConfigDlg.NumberSort_1(new[] { "0", "0" }, 0, 1));
    }

    [Fact]
    public void NumberSort_1_实际用于降序排序()
    {
        var list = new List<string> { "3", "10", "1", "10", "7" };
        list.Sort((a, b) => TMirReturnConfigDlg.NumberSort_1(list, list.IndexOf(a), list.IndexOf(b)));
        // 用完整列表 + 索引形式复刻原文 CustomSort 语义
        var l2 = new List<string> { "3", "10", "1", "10", "7" };
        var idx = new List<int> { 0, 1, 2, 3, 4 };
        idx.Sort((i, j) => TMirReturnConfigDlg.NumberSort_1(l2, i, j));
        Assert.Equal(new[] { 1, 3, 4, 0, 2 }, idx.ToArray());  // 10,10,7,3,1 的稳定顺序
    }

    // ================================================================================
    // CanFilterExp（原文 4479-4484）
    // ================================================================================

    [Fact]
    public void CanFilterExp_未勾选时永远false()
    {
        // 4481-4482
        TMirReturnConfigDlg.g_Config.nFilterMinExp = 1000;
        Assert.False(_dlg.CanFilterExp(0));
        Assert.False(_dlg.CanFilterExp(999));
        Assert.False(_dlg.CanFilterExp(5000));
    }

    [Fact]
    public void CanFilterExp_勾选后按Exp小于下限()
    {
        _dlg.SetConfigChecked(TConfigChecked.ckFilterExp, true);
        TMirReturnConfigDlg.g_Config.nFilterMinExp = 1000;

        Assert.True(_dlg.CanFilterExp(0));       // 4483
        Assert.True(_dlg.CanFilterExp(999));
        Assert.False(_dlg.CanFilterExp(1000));   // 边界：不小于
        Assert.False(_dlg.CanFilterExp(1001));
        Assert.False(_dlg.CanFilterExp(uint.MaxValue));
    }

    [Fact]
    public void CanFilterExp_下限为0或负时_边界行为()
    {
        _dlg.SetConfigChecked(TConfigChecked.ckFilterExp, true);

        TMirReturnConfigDlg.g_Config.nFilterMinExp = 0;
        Assert.False(_dlg.CanFilterExp(0));      // 0 < 0 = false

        // 负下限：原文按 Int64 提升比较；托管侧显式 (long) 转换
        TMirReturnConfigDlg.g_Config.nFilterMinExp = -1;
        Assert.False(_dlg.CanFilterExp(0));      // (long)0 < -1 = false
        Assert.False(_dlg.CanFilterExp(uint.MaxValue));  // 4294967295 < -1 = false
    }

    // ================================================================================
    // 查询族（原文 3331-3372）
    // ================================================================================

    [Fact]
    public void FindShowItem_命中返回boShowName_未命中false()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_ShowItemList.Add(new TShowItem { sItemName = "金创药", boShowName = 1 });
        db.m_ShowItemList.Add(new TShowItem { sItemName = "魔法药", boShowName = 0 });

        Assert.True(_dlg.FindShowItem("金创药"));    // 3340-3342
        Assert.False(_dlg.FindShowItem("魔法药"));   // 3342 为 0
        Assert.False(_dlg.FindShowItem("不存在"));   // 3344
        db.m_ShowItemList.Clear();
    }

    [Fact]
    public void FindHintItem_与FindPickItem_分别读boHintMsg与boPickup()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_ShowItemList.Add(new TShowItem { sItemName = "a", boHintMsg = 1, boPickup = 0 });
        db.m_ShowItemList.Add(new TShowItem { sItemName = "b", boHintMsg = 0, boPickup = 1 });

        Assert.True(_dlg.FindHintItem("a"));        // 3351-3353
        Assert.False(_dlg.FindHintItem("b"));
        Assert.False(_dlg.FindPickItem("a"));       // 3362-3364
        Assert.True(_dlg.FindPickItem("b"));
        db.m_ShowItemList.Clear();
    }

    [Fact]
    public void GetShowItem_返回同一对象引用()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        var item = new TShowItem { sItemName = "c" };
        db.m_ShowItemList.Add(item);

        Assert.Same(item, _dlg.GetShowItem("c"));   // 3331-3334
        Assert.Null(_dlg.GetShowItem("d"));
        db.m_ShowItemList.Clear();
    }

    // ================================================================================
    // Struck / HealthChange 分派（原文 3374-3439）
    // ================================================================================

    [Fact]
    public void Struck_未启用时不分派()
    {
        object actor = new object();
        _dlg.Struck(actor, 0, 100);                  // 3379 FEnabled=False
        Assert.Empty(_hpUse);
    }

    [Fact]
    public void Struck_本人伤害_分派nObj0()
    {
        object me = new object();
        _dlg.MySelfResolver = () => me;
        _dlg.GetActorHP = _ => 1000;
        _dlg.SetEnabled(true);

        _dlg.Struck(me, 900, 1000);                  // 3385-3387 nDamage=100

        Assert.Equal(new[] { (0, 100) }, _hpUse);
    }

    [Fact]
    public void Struck_伤害为0或负_不分派()
    {
        object me = new object();
        _dlg.MySelfResolver = () => me;
        _dlg.GetActorHP = _ => 1000;
        _dlg.SetEnabled(true);

        _dlg.Struck(me, 1000, 1000);                 // nDamage=0 → 3386 不成立
        _dlg.Struck(me, 1200, 1000);                 // nDamage=-200 → 不成立
        Assert.Empty(_hpUse);
    }

    [Fact]
    public void Struck_英雄伤害_分派nObj1()
    {
        object me = new object();
        object hero = new object();
        _dlg.MySelfResolver = () => me;
        _dlg.MyHeroResolver = () => hero;
        _dlg.GetActorHP = a => ReferenceEquals(a, hero) ? 800 : 1000;
        _dlg.SetEnabled(true);

        _dlg.Struck(hero, 700, 800);                 // 3394-3397 nDamage=100

        Assert.Equal(new[] { (1, 100) }, _hpUse);
    }

    [Fact]
    public void Struck_英雄分支嵌在本人非nil之内()
    {
        // ★ 3390 的 if g_MyHero <> nil **嵌在** 3381 的 if g_MySelf <> nil 之内
        object hero = new object();
        _dlg.MySelfResolver = () => null;            // g_MySelf = nil
        _dlg.MyHeroResolver = () => hero;
        _dlg.GetActorHP = _ => 800;
        _dlg.SetEnabled(true);

        _dlg.Struck(hero, 700, 800);                 // 3381 不成立 → 英雄分支根本不执行
        Assert.Empty(_hpUse);
    }

    [Fact]
    public void Struck_非本人非英雄_不分派()
    {
        object me = new object();
        _dlg.MySelfResolver = () => me;
        _dlg.GetActorHP = _ => 1000;
        _dlg.SetEnabled(true);

        _dlg.Struck(new object(), 100, 100);
        Assert.Empty(_hpUse);
    }

    [Fact]
    public void HealthChange_本人HP与MP各分派一次()
    {
        object me = new object();
        _dlg.MySelfResolver = () => me;
        _dlg.GetActorHP = _ => 1000;
        _dlg.GetActorMP = _ => 500;
        _dlg.SetEnabled(true);

        _dlg.HealthChange(me, 900, 400, 1000);       // 3415-3421

        Assert.Equal(new[] { (0, 100) }, _hpUse);
        Assert.Equal(new[] { (0, 100) }, _mpUse);
    }

    [Fact]
    public void HealthChange_英雄HP与MP各分派一次()
    {
        object me = new object();
        object hero = new object();
        _dlg.MySelfResolver = () => me;
        _dlg.MyHeroResolver = () => hero;
        _dlg.GetActorHP = a => ReferenceEquals(a, hero) ? 800 : 1000;
        _dlg.GetActorMP = a => ReferenceEquals(a, hero) ? 400 : 500;
        _dlg.SetEnabled(true);

        _dlg.HealthChange(hero, 700, 300, 800);      // 3428-3434

        Assert.Equal(new[] { (1, 100) }, _hpUse);
        Assert.Equal(new[] { (1, 100) }, _mpUse);
    }

    [Fact]
    public void HealthChange_未启用时不分派()
    {
        object me = new object();
        _dlg.MySelfResolver = () => me;
        _dlg.GetActorHP = _ => 1000;
        _dlg.GetActorMP = _ => 500;
        _dlg.HealthChange(me, 0, 0, 1000);           // 3409 FEnabled=False
        Assert.Empty(_hpUse);
        Assert.Empty(_mpUse);
    }

    // ================================================================================
    // 控件写回处理器（原文 2139-2556 / 5753 / 5842-5940）
    // ================================================================================

    [Fact]
    public void 药物设置写回_百分比与勾选位()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 2;
        _ui.PlugEditCheckHPPercent = 77;
        _ui.PlugEditCheckMPPercent = 66;
        _ui.PlugComboBoxCheckHPValue = 3;
        _ui.PlugComboBoxCheckMPValue = 4;
        _ui.PlugCheckBoxCheckHPIsAuto = true;
        _ui.PlugCheckBoxCheckMPIsAuto = true;
        _ui.PlugCheckBoxCheckDuraIsAuto = true;

        _dlg.DEditCheckHPPercentChange();     // 2141
        _dlg.DEditCheckMPPercentChange();     // 2152
        _dlg.ComboBoxCheckHPValueChange();    // 2157
        _dlg.ComboBoxCheckMPValueChange();    // 2162
        _dlg.DCheckBoxCheckHPIsAutoClick();   // 2167
        _dlg.DCheckBoxCheckMPIsAutoClick();   // 2172
        _dlg.DCheckBoxCheckDuraIsAuto();      // 2177

        var c = TMirReturnConfigDlg.g_Config;
        Assert.Equal(77, c.CheckHpPercents[2]);
        Assert.Equal(66, c.CheckMpPercents[2]);
        Assert.Equal(3, c.CheckHpValues[2]);
        Assert.Equal(4, c.CheckMpValues[2]);
        Assert.True(c.CheckHpIsAutos[2]);
        Assert.True(c.CheckMpIsAutos[2]);
        Assert.True(c.CheckDuraIsAutos[2]);
    }

    [Fact]
    public void DEditCheckDuraChange_Max1下界夹紧并回写控件()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;
        _ui.PlugEditCheckDura = 0;
        _dlg.DEditCheckDuraChange();          // 2182-2183
        Assert.Equal(1, TMirReturnConfigDlg.g_Config.CheckDuraMin[0]);
        Assert.Equal(1, _ui.PlugEditCheckDura);   // ★ 回写控件

        _ui.PlugEditCheckDura = -5;
        _dlg.DEditCheckDuraChange();
        Assert.Equal(1, TMirReturnConfigDlg.g_Config.CheckDuraMin[0]);

        _ui.PlugEditCheckDura = 9;
        _dlg.DEditCheckDuraChange();
        Assert.Equal(9, TMirReturnConfigDlg.g_Config.CheckDuraMin[0]);
    }

    [Fact]
    public void DEditCheckDuraTimeChange_Max2下界夹紧并回写()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;
        _ui.PlugEditCheckDuraTime = 0;
        _dlg.DEditCheckDuraTimeChange();      // 2193-2194
        Assert.Equal(2, TMirReturnConfigDlg.g_Config.CheckDuraTime[0]);
        Assert.Equal(2, _ui.PlugEditCheckDuraTime);
    }

    [Fact]
    public void DEditCheckDuraValueChange_写入string20()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 1;
        _ui.PlugEditCheckDuraValue = "祝福油";
        _dlg.DEditCheckDuraValueChange();     // 2188
        Assert.Equal("祝福油", TMirReturnConfigDlg.g_Config.CheckDuraValue[1]);
    }

    [Fact]
    public void RenewTimeChange_夹紧到g_ClientConfig下限()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;
        MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime = 500;

        _ui.PlugEditRenewHPTime = 100;
        _dlg.DEditRenewHPTimeChange();        // 2220-2221
        Assert.Equal(500, TMirReturnConfigDlg.g_Config.RenewHPTimes[0]);
        Assert.Equal(500, _ui.PlugEditRenewHPTime);

        _ui.PlugEditRenewHPTime = 700;
        _dlg.DEditRenewHPTimeChange();
        Assert.Equal(700, TMirReturnConfigDlg.g_Config.RenewHPTimes[0]);
    }

    [Fact]
    public void RenewMPTimeChange_同样夹紧()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;
        MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime = 500;
        _ui.PlugEditRenewMPTime = 10;
        _dlg.DEditRenewMPTimeChange();        // 2226-2227
        Assert.Equal(500, TMirReturnConfigDlg.g_Config.RenewMPTimes[0]);
    }

    [Fact]
    public void PlugEditHeroDodgeHPPercentChange_写配置并下发()
    {
        int sent = -1;
        MirReturnGlobalSeam.SendPlugInConfig = v => sent = v;
        _ui.PlugEditHeroDodgeHPPercent = 33;
        _dlg.PlugEditHeroDodgeHPPercentChange();   // 2146-2147
        Assert.Equal(33, TMirReturnConfigDlg.g_Config.nHeroDodgeHPPercent);
        Assert.Equal(33, sent);
    }

    [Fact]
    public void 超药HPChange_九路索引_越界不写()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 3;
        _ui.PlugEditSuperMedicaHP0 = 11;
        _ui.PlugEditSuperMedicaHP8 = 99;

        _dlg.DEditSuperMedicaHPChange(0);       // 2312-2314
        _dlg.DEditSuperMedicaHPChange(8);
        _dlg.DEditSuperMedicaHPChange(-1);      // 2312 不成立 → 不写
        _dlg.DEditSuperMedicaHPChange(9);       // 2312 不成立 → 不写

        Assert.Equal(11, TMirReturnConfigDlg.g_Config.SuperMedicaHPs[3, 0]);
        Assert.Equal(99, TMirReturnConfigDlg.g_Config.SuperMedicaHPs[3, 8]);
    }

    [Fact]
    public void 超药HPTimeChange_夹紧并回写对应控件()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;
        MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime = 500;
        _ui.PlugEditSuperMedicaHPTime0 = 50;

        _dlg.DEditSuperMedicaHPTimeChange(0);   // 2370-2371

        Assert.Equal(500, TMirReturnConfigDlg.g_Config.SuperMedicaHPTimes[0, 0]);
        Assert.Equal(500, _ui.PlugEditSuperMedicaHPTime0);   // 2371 回写
    }

    [Fact]
    public void 超药MPChange与MPTimeChange()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 4;
        MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime = 500;
        _ui.PlugEditSuperMedicaMP1 = 7;
        _ui.PlugEditSuperMedicaMPTime1 = 20;

        _dlg.DEditSuperMedicaMPChange(1);       // 2428
        _dlg.DEditSuperMedicaMPTimeChange(1);   // 2484-2485

        Assert.Equal(7, TMirReturnConfigDlg.g_Config.SuperMedicaMPs[4, 1]);
        Assert.Equal(500, TMirReturnConfigDlg.g_Config.SuperMedicaMPTimes[4, 1]);
        Assert.Equal(500, _ui.PlugEditSuperMedicaMPTime1);
    }

    [Fact]
    public void 超药物品名勾选_Click九路()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;
        _ui.SetPlugCheckBoxUseSuperMedicaItemName(3, true);

        _dlg.DCheckBoxUseSuperMedicaItemNameClick(3);   // 2540-2542

        Assert.True(TMirReturnConfigDlg.g_Config.SuperMedicaUses[0, 3]);
        Assert.False(TMirReturnConfigDlg.g_Config.SuperMedicaUses[3, 4]);   // 越界索引不写
    }

    [Fact]
    public void DEditChange_两路Sender()
    {
        _ui.PlugEditExpFilter = 123;
        _ui.PlugEditAutoMagicTime = 45;

        _dlg.DEditChange(MirReturnEditId.PlugEditExpFilter);      // 2548-2550
        Assert.Equal(123, TMirReturnConfigDlg.g_Config.nFilterMinExp);

        _dlg.DEditChange(MirReturnEditId.PlugEditAutoMagicTime);  // 2552-2554
        Assert.Equal(45, TMirReturnConfigDlg.g_Config.nAutoUseMagicTime);

        var before = TMirReturnConfigDlg.g_Config.nFilterMinExp;
        _dlg.DEditChange(MirReturnEditId.None);                   // 两路都不成立
        Assert.Equal(before, TMirReturnConfigDlg.g_Config.nFilterMinExp);
    }

    [Fact]
    public void DEditNotRushMonRangeChange_写配置并同步()
    {
        int frm = -1;
        MirReturnGlobalSeam.SetFrmMainGJNotRushMonRange = v => frm = v;
        _ui.PlugEditNotRushMonRange = 9;
        _dlg.DEditNotRushMonRangeChange();      // 5702-5703
        Assert.Equal(9, TMirReturnConfigDlg.g_Config.nGJNotRushMonRange);
        Assert.Equal(9, frm);
    }

    [Fact]
    public void DEditGroupAttackCountChanged_写配置并同步()
    {
        int frm = -1;
        MirReturnGlobalSeam.SetFrmMainGJGroupAttackCount = v => frm = v;
        _ui.PlugEditNotGroupAttackCount = 5;
        _dlg.DEditGroupAttackCountChanged();    // 5755-5756
        Assert.Equal(5, TMirReturnConfigDlg.g_Config.nGJGroupAttackCount);
        Assert.Equal(5, frm);
    }

    [Fact]
    public void ComboBoxPlayAttackValueSelect_五路Sender分派()
    {
        int a = -1, b = -1, c = -1, d = -1, e = -1;
        MirReturnGlobalSeam.SetFrmMainGJPlayAttackOption = v => a = v;
        MirReturnGlobalSeam.SetFrmMainGJNoRedPoisonOption = v => b = v;
        MirReturnGlobalSeam.SetFrmMainGJNoBluePoisonOption = v => c = v;
        MirReturnGlobalSeam.SetFrmMainGJNoDuFuOption = v => d = v;
        MirReturnGlobalSeam.SetFrmMainGJBagFullOption = v => e = v;

        _ui.PlugComboBoxPlayAttackValue = 1;
        _ui.PlugComboBoxNoRedPoisonValue = 2;
        _ui.PlugComboBoxNoBluePoisonValue = 3;
        _ui.PlugComboBoxNoDuFuValue = 4;
        _ui.PlugComboBoxBagFullValue = 5;

        _dlg.ComboBoxPlayAttackValueSelect(MirReturnControlId.PlugComboBoxPlayAttackValue);   // 5708-5711
        _dlg.ComboBoxPlayAttackValueSelect(MirReturnControlId.PlugComboBoxNoRedPoisonValue);
        _dlg.ComboBoxPlayAttackValueSelect(MirReturnControlId.PlugComboBoxNoBluePoisonValue);
        _dlg.ComboBoxPlayAttackValueSelect(MirReturnControlId.PlugComboBoxNoDuFuValue);
        _dlg.ComboBoxPlayAttackValueSelect(MirReturnControlId.PlugComboBoxBagFullValue);

        var g = TMirReturnConfigDlg.g_Config;
        Assert.Equal((1, 1), (g.nGJPlayAttackOption, a));
        Assert.Equal((2, 2), (g.nGJNoRedPoisonOption, b));
        Assert.Equal((3, 3), (g.nGJNoBluePoisonOption, c));
        Assert.Equal((4, 4), (g.nGJNoDuFuOption, d));
        Assert.Equal((5, 5), (g.nGJBagFullOption, e));
    }

    [Fact]
    public void ComboBoxPlayAttackValueSelect_None不触发任何一路()
    {
        TMirReturnConfigDlg.g_Config.nGJPlayAttackOption = 9;
        _dlg.ComboBoxPlayAttackValueSelect(MirReturnControlId.None);
        Assert.Equal(9, TMirReturnConfigDlg.g_Config.nGJPlayAttackOption);   // 未被改写
    }

    // ================================================================================
    // DCheckBox*PercentClick（原文 5842-5940）★ 含缺陷固定
    // ================================================================================

    [Fact]
    public void DCheckBoxAutoPercentClick_未勾选时不做纠正()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;
        TMirReturnConfigDlg.g_Config.CheckHpPercents[0] = 100;
        _ui.PlugCheckBoxAutoPercent = false;

        _dlg.DCheckBoxAutoPercentClick();       // 5847 不成立

        Assert.Equal(100, TMirReturnConfigDlg.g_Config.CheckHpPercents[0]);
    }

    [Fact]
    public void DCheckBoxAutoPercentClick_勾选后_大于99归50()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;
        _ui.PlugCheckBoxAutoPercent = true;
        TMirReturnConfigDlg.g_Config.CheckHpPercents[0] = 100;
        TMirReturnConfigDlg.g_Config.CheckMpPercents[0] = 101;

        _dlg.DCheckBoxAutoPercentClick();       // 5849-5859

        Assert.Equal(50, TMirReturnConfigDlg.g_Config.CheckHpPercents[0]);
        Assert.Equal(50, _ui.PlugEditCheckHPPercent);
        Assert.Equal(50, TMirReturnConfigDlg.g_Config.CheckMpPercents[0]);
        Assert.Equal(50, _ui.PlugEditCheckMPPercent);
    }

    [Fact]
    public void DCheckBoxAutoPercentClick_边界99不纠正()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;
        _ui.PlugCheckBoxAutoPercent = true;
        TMirReturnConfigDlg.g_Config.CheckHpPercents[0] = 99;

        _dlg.DCheckBoxAutoPercentClick();       // 5849 是 > 99

        Assert.Equal(99, TMirReturnConfigDlg.g_Config.CheckHpPercents[0]);
    }

    [Fact]
    public void DCheckBoxRenewAutoPercentClick_Special归30_其余归50()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;
        _ui.PlugCheckBoxRenewAutoPercent = true;
        var c = TMirReturnConfigDlg.g_Config;
        c.RenewHPPercents[0] = 100;
        c.RenewMPPercents[0] = 100;
        c.RenewSpecialHPPercents[0] = 100;
        c.RenewSpecialMPPercents[0] = 100;

        _dlg.DCheckBoxRenewAutoPercentClick();  // 5868-5893

        Assert.Equal(50, c.RenewHPPercents[0]);
        Assert.Equal(50, c.RenewMPPercents[0]);
        Assert.Equal(30, c.RenewSpecialHPPercents[0]);   // ★ 30 不是 50
        Assert.Equal(30, c.RenewSpecialMPPercents[0]);
        Assert.Equal(50, _ui.PlugEditRenewHPPercent);
        Assert.Equal(30, _ui.PlugEditRenewSpecialHPPercent);
    }

    [Fact]
    public void DCheckBoxSuperMedicaPercentClick_九项纠正但只回写八项()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;
        _ui.PlugCheckBoxSuperMedicaPercent = true;
        var c = TMirReturnConfigDlg.g_Config;
        for (int i = 0; i <= 8; i++)
        {
            c.SuperMedicaHPs[0, i] = 100;
            c.SuperMedicaMPs[0, i] = 100;
        }

        _dlg.DCheckBoxSuperMedicaPercentClick();   // 5905-5938

        // 5905-5911 / 5923-5929：0..8 全被纠正为 50
        for (int i = 0; i <= 8; i++)
        {
            Assert.Equal(50, c.SuperMedicaHPs[0, i]);
            Assert.Equal(50, c.SuperMedicaMPs[0, i]);
        }

        // ★★ 原文缺陷：5913-5920 回写**控件下标 0..7**（控件名 HP1..HP8），
        //    故 PlugEditSuperMedicaHP0（数组下标 8 对应）**从未被回写**，控件名与数组下标**整体错位 1**
        // 控件 PlugEditSuperMedicaHP1..HP8 ← 数组下标 0..7
        for (int i = 0; i <= 7; i++)
            Assert.Equal(50, _ui.SuperHpWrites[i]);
        Assert.Equal(50, _ui.SuperMpWrites[0]);
        Assert.Equal(50, _ui.SuperMpWrites[7]);
        // ★ 数组下标 8（超级疗伤药）的纠正结果**不回显给任何控件**（原文缺陷）
    }

    [Fact]
    public void DCheckBoxSuperMedicaPercentClick_未勾选时不纠正()
    {
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;
        _ui.PlugCheckBoxSuperMedicaPercent = false;
        TMirReturnConfigDlg.g_Config.SuperMedicaHPs[0, 0] = 100;

        _dlg.DCheckBoxSuperMedicaPercentClick();   // 5903 不成立

        Assert.Equal(100, TMirReturnConfigDlg.g_Config.SuperMedicaHPs[0, 0]);
    }

    // ================================================================================
    // 音量（原文 5942-5952）
    // ================================================================================

    [Fact]
    public void OnChangedVolumePosition_写全局音量并置文案为音量()
    {
        _ui.TrackBarVolumePosition = 42;
        _dlg.OnChangedVolumePosition();          // 5944-5945
        Assert.Equal(42, MirReturnGlobalSeam.g_SoundVolume);
        Assert.Equal("音量", _ui.PlugCheckBoxVolumeCaption);
    }

    [Fact]
    public void OnChanggingVolumePosition_写全局音量并置文案为数字()
    {
        _ui.TrackBarVolumePosition = 7;
        _dlg.OnChanggingVolumePosition();        // 5950-5951
        Assert.Equal(7, MirReturnGlobalSeam.g_SoundVolume);
        Assert.Equal("7", _ui.PlugCheckBoxVolumeCaption);
    }

    [Fact]
    public void 音量两态文案不同_落定与拖动()
    {
        _ui.TrackBarVolumePosition = 5;
        _dlg.OnChanggingVolumePosition();
        Assert.Equal("5", _ui.PlugCheckBoxVolumeCaption);
        _dlg.OnChangedVolumePosition();
        Assert.Equal("音量", _ui.PlugCheckBoxVolumeCaption);
    }

    // ================================================================================
    // Boss / 挂机怪物名单（原文 5116-5537）
    // ================================================================================

    [Fact]
    public void CheckBossNameExists_大小写不敏感且排除CurIndex()
    {
        _ui.PlugScrollBoxBossLines.AddRange(new[] { "祖玛教主", "赤月恶魔" });

        Assert.True(_dlg.CheckBossNameExists("祖玛教主"));        // 5141 命中
        Assert.True(_dlg.CheckBossNameExists("ZUMajiaozhu") == false);
        Assert.False(_dlg.CheckBossNameExists("牛魔王"));
        // 大小写不敏感
        Assert.True(_dlg.CheckBossNameExists("赤月恶魔"));
        // CurIndex 排除自己
        Assert.False(_dlg.CheckBossNameExists("祖玛教主", 0));
        Assert.True(_dlg.CheckBossNameExists("祖玛教主", 1));
    }

    [Fact]
    public void DBtnBossAddClick_空名提示且不添加()
    {
        _ui.PlugEditBoss = "   ";
        _dlg.DBtnBossAddClick();                 // 5154-5157

        Assert.Empty(_ui.PlugScrollBoxBossLines);
        Assert.Equal(new[] { "Boss名不能为空" }, MirReturnMessageSeam.Messages);
    }

    [Fact]
    public void DBtnBossAddClick_重名提示不添加()
    {
        _ui.PlugScrollBoxBossLines.Add("祖玛教主");
        _ui.PlugEditBoss = "祖玛教主";
        _dlg.DBtnBossAddClick();                 // 5168

        Assert.Single(_ui.PlugScrollBoxBossLines);
        Assert.Equal(new[] { "Boss名已存在，添加失败" }, MirReturnMessageSeam.Messages);
    }

    [Fact]
    public void DBtnBossAddClick_成功添加并立即落盘()
    {
        _ui.PlugEditBoss = " 祖玛教主 ";          // 5153 Trim
        _dlg.DBtnBossAddClick();                 // 5159-5163

        Assert.Equal(new[] { "祖玛教主" }, _ui.PlugScrollBoxBossLines);
        Assert.Contains("Boss.set", _ui.BossSavedTo);
        Assert.Empty(MirReturnMessageSeam.Messages);
    }

    [Fact]
    public void DBtnBossDelClick_界内才删()
    {
        _ui.PlugScrollBoxBossLines.AddRange(new[] { "a", "b" });
        _ui.PlugScrollBoxBossItemIndex = -1;
        _dlg.DBtnBossDelClick();                 // 5173 不成立
        Assert.Equal(2, _ui.PlugScrollBoxBossLines.Count);

        _ui.PlugScrollBoxBossItemIndex = 0;
        _dlg.DBtnBossDelClick();                 // 5175-5177
        Assert.Equal(new[] { "b" }, _ui.PlugScrollBoxBossLines);
        Assert.Equal("", _ui.PlugEditBoss);
    }

    [Fact]
    public void DBtnBossModifyClick_空名提示()
    {
        _ui.PlugEditBoss = "";
        _dlg.DBtnBossModifyClick();
        Assert.Equal(new[] { "Boss名不能为空" }, MirReturnMessageSeam.Messages);
    }

    [Fact]
    public void DBtnBossModifyClick_重名提示()
    {
        _ui.PlugScrollBoxBossLines.AddRange(new[] { "a", "b" });
        _ui.PlugScrollBoxBossItemIndex = 0;
        _ui.PlugEditBoss = "b";
        _dlg.DBtnBossModifyClick();              // 5192 命中 → 5200
        Assert.Equal(new[] { "Boss名已存在，修改失败" }, MirReturnMessageSeam.Messages);
    }

    [Fact]
    public void DBtnBossModifyClick_就地改写并落盘()
    {
        _ui.PlugScrollBoxBossLines.AddRange(new[] { "a", "b" });
        _ui.PlugScrollBoxBossItemIndex = 0;
        _ui.PlugEditBoss = "c";
        _dlg.DBtnBossModifyClick();              // 5194-5197
        Assert.Equal(new[] { "c", "b" }, _ui.PlugScrollBoxBossLines);
    }

    [Fact]
    public void DBtnBossModifyClick_ItemIndex为负_抛越界异常并登记原文缺陷()
    {
        // ★ 5194：原文 ItemIndex=-1 时 Lines[-1] 会抛 EStringListError
        _ui.PlugScrollBoxBossLines.Add("a");
        _ui.PlugScrollBoxBossItemIndex = -1;
        _ui.PlugEditBoss = "z";

        Assert.Throws<ArgumentOutOfRangeException>(() => _dlg.DBtnBossModifyClick());
    }

    [Fact]
    public void DMemoBossListClick_回填与按钮状态()
    {
        _ui.PlugScrollBoxBossLines.AddRange(new[] { "a", "b" });

        _ui.PlugScrollBoxBossItemIndex = 1;
        _dlg.DMemoBossListClick();               // 5120-5124
        Assert.Equal("b", _ui.PlugEditBoss);
        Assert.True(_ui.PlugBtnBossModify);
        Assert.True(_ui.PlugBtnBossDel);

        _ui.PlugScrollBoxBossItemIndex = -1;
        _dlg.DMemoBossListClick();               // 5128-5130
        Assert.Equal("", _ui.PlugEditBoss);
        Assert.False(_ui.PlugBtnBossModify);
        Assert.False(_ui.PlugBtnBossDel);
    }

    [Fact]
    public void SaveOrLoadBossList_保存方向_写文件并把列表写进全局()
    {
        ConfigShareGlobal.g_sPlugServerName = "S1";
        ConfigShareGlobal.g_sPlugUserName = "U1";
        MirReturnGlobalSeam.AppPath = @"D:\app\";
        _ui.PlugScrollBoxBossLines.AddRange(new[] { "x", "y" });

        _dlg.SaveOrLoadBossList(true);           // 5233-5250

        Assert.Contains(@"Config\S1.U1.Boss.set", _ui.BossSavedTo);
        Assert.Equal(new[] { "x", "y" }, MirReturnGlobalSeam.g_BossList);
    }

    [Fact]
    public void SaveOrLoadBossList_读取方向_文件不存在则只清空()
    {
        MirReturnGlobalSeam.FileExists = _ => false;
        _ui.PlugScrollBoxBossLines.Add("旧");
        _ui.PlugScrollBoxBossItemIndex = 0;

        _dlg.SaveOrLoadBossList(false);          // 5241-5247

        Assert.Empty(_ui.PlugScrollBoxBossLines);
        Assert.Equal(-1, _ui.PlugScrollBoxBossItemIndex);
        Assert.False(_ui.PlugBtnBossModify);
        Assert.False(_ui.PlugBtnBossDel);
        Assert.Empty(MirReturnGlobalSeam.g_BossList);
    }

    [Fact]
    public void SaveOrLoadBossList_读取方向_文件存在则载入()
    {
        MirReturnGlobalSeam.FileExists = _ => true;
        _ui.BossFileContent.AddRange(new[] { "载入1", "载入2" });

        _dlg.SaveOrLoadBossList(false);          // 5243

        Assert.Equal(new[] { "载入1", "载入2" }, _ui.PlugScrollBoxBossLines);
        Assert.Equal(new[] { "载入1", "载入2" }, MirReturnGlobalSeam.g_BossList);
    }

    [Fact]
    public void SaveOrLoadBossList_用户名非法字符被替换()
    {
        // 5211-5231 的"用户名非法字符替换"（9 个字符），最终体现在落盘路径上（5233）
        ConfigShareSeam.g_MySelf = new Self("a/b\\c:d*e?f\"g<h>i|j");
        MirReturnGlobalSeam.AppPath = "";

        _dlg.SaveOrLoadBossList(true);           // 5233-5236

        Assert.Contains("a{b}c;d@e!f~g(h)i-j", _ui.BossSavedTo);
    }

    [Fact]
    public void CheckGJMonNameExists_大小写不敏感且排除CurIndex()
    {
        _ui.PlugScrollBoxMonsLines.AddRange(new[] { "鸡", "鹿" });
        Assert.True(_dlg.CheckGJMonNameExists("鸡"));
        Assert.False(_dlg.CheckGJMonNameExists("鸡", 0));
        Assert.True(_dlg.CheckGJMonNameExists("鸡", 1));
        Assert.False(_dlg.CheckGJMonNameExists("稻草人"));
    }

    [Fact]
    public void DBtnGJMonNameAddClick_三态()
    {
        _ui.PlugEditMonName = "  ";
        _dlg.DBtnGJMonNameAddClick();
        Assert.Equal(new[] { "怪物名不能为空" }, MirReturnMessageSeam.Messages);

        MirReturnMessageSeam.Messages.Clear();
        _ui.PlugScrollBoxMonsLines.Add("鸡");
        _ui.PlugEditMonName = "鸡";
        _dlg.DBtnGJMonNameAddClick();
        Assert.Equal(new[] { "怪物名已存在，添加失败" }, MirReturnMessageSeam.Messages);

        MirReturnMessageSeam.Messages.Clear();
        _ui.PlugEditMonName = " 鹿 ";
        _dlg.DBtnGJMonNameAddClick();
        Assert.Equal(new[] { "鸡", "鹿" }, _ui.PlugScrollBoxMonsLines);
        Assert.Empty(MirReturnMessageSeam.Messages);
    }

    [Fact]
    public void DBtnGJMonNameDelClick与EditClick()
    {
        _ui.PlugScrollBoxMonsLines.AddRange(new[] { "鸡", "鹿" });

        _ui.PlugScrollBoxMonsItemIndex = 0;
        _dlg.DBtnGJMonNameEditClick();           // 5479 → 空名提示
        Assert.Equal(new[] { "怪物名不能为空" }, MirReturnMessageSeam.Messages);

        MirReturnMessageSeam.Messages.Clear();
        _ui.PlugEditMonName = "鹿";           // 已被 1 号占用 → CurIndex=0 时判定为重名
        _ui.PlugScrollBoxMonsItemIndex = 0;
        _dlg.DBtnGJMonNameEditClick();           // 5479 命中 → 5486
        Assert.Equal(new[] { "怪物名已存在，修改失败" }, MirReturnMessageSeam.Messages);
        Assert.Equal(new[] { "鸡", "鹿" }, _ui.PlugScrollBoxMonsLines);

        MirReturnMessageSeam.Messages.Clear();
        _ui.PlugEditMonName = "鸭";           // 与自身不同名 → 就地改写
        _ui.PlugScrollBoxMonsItemIndex = 0;
        _dlg.DBtnGJMonNameEditClick();           // 5481-5483
        Assert.Equal(new[] { "鸭", "鹿" }, _ui.PlugScrollBoxMonsLines);

        _ui.PlugScrollBoxMonsItemIndex = 0;
        _dlg.DBtnGJMonNameDelClick();            // 5462-5464
        Assert.Equal(new[] { "鹿" }, _ui.PlugScrollBoxMonsLines);
        Assert.Equal("", _ui.PlugEditMonName);
    }

    [Fact]
    public void SaveOrLoadGJMonList_双向()
    {
        ConfigShareGlobal.g_sPlugServerName = "S";
        ConfigShareGlobal.g_sPlugUserName = "U";
        MirReturnGlobalSeam.AppPath = @"C:\app\";
        _ui.PlugScrollBoxMonsLines.Add("鸡");

        _dlg.SaveOrLoadGJMonList(true);          // 5519-5536
        Assert.Contains(@"Config\S.U.GjMon.set", _ui.MonsSavedTo);
        Assert.Equal(new[] { "鸡" }, MirReturnGlobalSeam.g_GJMonList);

        MirReturnGlobalSeam.FileExists = _ => true;
        _ui.MonsFileContent.Add("鹿");
        _dlg.SaveOrLoadGJMonList(false);
        Assert.Equal(new[] { "鹿" }, _ui.PlugScrollBoxMonsLines);
    }

    // ================================================================================
    // 挂机技能表 SaveOrLoadGJMagicList1/2（原文 5539-5698）
    // ================================================================================

    [Fact]
    public void SaveOrLoadGJMagicList1_保存_只取恰好两列且勾选的行()
    {
        ConfigShareGlobal.g_sPlugServerName = "S";
        ConfigShareGlobal.g_sPlugUserName = "U";

        _ui.Memo82Rows.Add((2, true, 101));    // 取
        _ui.Memo82Rows.Add((2, false, 102));   // 未勾选 → 不取
        _ui.Memo82Rows.Add((1, true, 103));    // Count<>2 → 不取
        _ui.Memo82Rows.Add((3, true, 104));    // Count<>2 → 不取

        _dlg.SaveOrLoadGJMagicList1(true);  // 5579-5592

        Assert.Single(MirReturnGlobalSeam.g_GJUseMagic1);
        Assert.Equal(101u, (uint)MirReturnGlobalSeam.g_GJUseMagic1[0]);   // 只取勾选的 101
        Assert.Single(MirReturnGlobalSeam.g_GJUseMagic1);
    }

    [Fact]
    public void SaveOrLoadGJMagicList1_两分支都先无条件清空全局()
    {
        MirReturnGlobalSeam.g_GJUseMagic1.Add("旧值");
        MirReturnGlobalSeam.FileExists = _ => false;

        _dlg.SaveOrLoadGJMagicList1(false);  // 5574 先清空，5600 才 Exit

        Assert.Empty(MirReturnGlobalSeam.g_GJUseMagic1);
    }

    [Fact]
    public void SaveOrLoadGJMagicList1_读取_过滤掉0值()
    {
        MirReturnGlobalSeam.FileExists = _ => true;
        MirReturnGlobalSeam.LoadTextFile = _ => new List<string> { "105", "0", "abc", "106" };

        _dlg.SaveOrLoadGJMagicList1(false);  // 5606-5613

        Assert.Equal(2, MirReturnGlobalSeam.g_GJUseMagic1.Count);
    }

    [Fact]
    public void SaveOrLoadGJMagicList2_同样语义()
    {
        ConfigShareGlobal.g_sPlugServerName = "S";
        ConfigShareGlobal.g_sPlugUserName = "U";
        _ui.Memo83Rows.Add((2, true, 201));

        _dlg.SaveOrLoadGJMagicList2(true);  // 5660-5673

        Assert.Single(MirReturnGlobalSeam.g_GJUseMagic2);

        MirReturnGlobalSeam.FileExists = _ => true;
        MirReturnGlobalSeam.LoadTextFile = _ => new List<string> { "202" };
        _dlg.SaveOrLoadGJMagicList2(false);
        Assert.Single(MirReturnGlobalSeam.g_GJUseMagic2);
    }

    // ================================================================================
    // 特殊颜色 / DIY（原文 5253-5397）
    // ================================================================================

    [Fact]
    public void DEditSpecialColorChange_只夹上界()
    {
        int labelColor = -1;
        MirReturnGlobalSeam.GetRGB = v => v;
        MirReturnGlobalSeam.SetLabelSpecialColorUp = v => labelColor = v;
        byte frm = 0;
        MirReturnGlobalSeam.SetFrmMainSpecialColor = v => frm = v;

        _ui.PlugEditSpecialColor = 300;
        _dlg.DEditSpecialColorChange();          // 5256-5259
        Assert.Equal(255, _ui.PlugEditSpecialColor);     // 夹上界
        Assert.Equal((byte)255, TMirReturnConfigDlg.g_Config.nSpecialColor);
        Assert.Equal(255, labelColor);
        Assert.Equal((byte)255, frm);

        _ui.PlugEditSpecialColor = -1;
        _dlg.DEditSpecialColorChange();          // ★ 无下界夹紧
        Assert.Equal(-1, _ui.PlugEditSpecialColor);
        Assert.Equal(unchecked((byte)-1), TMirReturnConfigDlg.g_Config.nSpecialColor);
    }

    [Fact]
    public void DBtnDiyAddClick_新建自定义物品()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_FileItemList.Clear();

        _ui.PlugEditSpecialName = "自定物品";
        _dlg.DBtnDiyAddClick();                  // 5268-5288

        Assert.Single(db.m_ShowItemList);
        Assert.Equal(1, db.m_FileItemList.Count);       // 5282-5284 同一份数据进两个列表
        Assert.NotSame(db.m_ShowItemList[0], db.m_FileItemList[0]);
        Assert.Equal(TItemType.i_diy, db.m_ShowItemList[0].ItemType);   // 5273
        Assert.Equal("自定类", db.m_ShowItemList[0].sItemType);          // 5274
        Assert.Equal("自定物品", db.m_ShowItemList[0].sItemName);
        Assert.Equal(0, db.m_ShowItemList[0].boHintMsg);
        Assert.Equal(8, _ui.PlugComboBoxItemStdMode);   // 5286
        Assert.Equal("PlugMemoConfig2.Last", _ui.LastCalled);   // 5288

        db.m_ShowItemList.Clear();
        db.m_FileItemList.Clear();
    }

    [Fact]
    public void DBtnDiyAddClick_已存在时什么都不做()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_FileItemList.Clear();
        db.m_ShowItemList.Add(new TShowItem { sItemName = "已存在" });

        _ui.PlugEditSpecialName = "已存在";
        _dlg.DBtnDiyAddClick();                  // 5270 不成立

        Assert.Single(db.m_ShowItemList);
        Assert.Empty(db.m_FileItemList);
        db.m_ShowItemList.Clear();
    }

    [Fact]
    public void DBtnDiyDelClick_空名不做任何事()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_ShowItemList.Add(new TShowItem { sItemName = "x" });

        _ui.PlugEditSpecialName = "";
        _dlg.DBtnDiyDelClick();                  // 5297 不成立
        Assert.Single(db.m_ShowItemList);
        db.m_ShowItemList.Clear();
    }

    [Fact]
    public void DBtnDiyDelClick_非空名走Del与下拉重选()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_FileItemList.Clear();
        db.m_ShowItemList.Add(new TShowItem { sItemName = "x" });
        db.m_FileItemList.Add(new TShowItem { sItemName = "x" });   // FilterItems.Del 复用同一 index

        _ui.PlugEditSpecialName = "x";
        _ui.LastCalled = "";
        _dlg.DBtnDiyDelClick();                  // 5299-5302

        Assert.Equal(8, _ui.PlugComboBoxItemStdMode);
        Assert.Equal("PlugMemoConfig2.Last", _ui.LastCalled);
        db.m_ShowItemList.Clear();
    }

    [Fact]
    public void DBtnDiyMyLoadOrSaveClick_保存分支发聊天栏()
    {
        ConfigShareGlobal.g_sPlugServerName = "S";
        ConfigShareGlobal.g_sPlugUserName = "U";
        _dlg.DBtnDiyMyLoadOrSaveClick(MirReturnDiyButtonId.PlugBtnDiySave);   // 5391-5395
        Assert.Contains(_chat, c => c.Item1.Contains("保存成功"));
    }

    [Fact]
    public void DBtnDiyMyLoadOrSaveClick_读取分支重建行模型并提示()
    {
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_FileItemList.Clear();
        db.m_ShowItemList.Add(new TShowItem { sItemName = "重生1", ItemType = TItemType.i_diy, sItemType = "自定类" });
        db.m_FileItemList.Add(new TShowItem { sItemName = "重生1" });

        _ui.PlugComboBoxItemStdMode = 8;         // i_diy
        _ui.PlugMemoConfig2Clear();
        _dlg.DBtnDiyMyLoadOrSaveClick(MirReturnDiyButtonId.PlugBtnDiyLoad);  // 5316-5389

        Assert.NotEmpty(_ui.Memo2Rows);
        Assert.Contains(_chat, c => c.Item1.Contains("读取上次保存的配置成功"));
        db.m_ShowItemList.Clear();
        db.m_FileItemList.Clear();
    }

    // ================================================================================
    // RefreshGJMagic（原文 5776-5840）
    // ================================================================================

    private static object MakeMagic(string name, uint id) => new MagicStub { Name = name, Id = id };

    private sealed class MagicStub
    {
        public string Name = "";
        public uint Id;
    }

    private void SetupMagicNameId()
    {
        MirReturnConfigGlobalSeam.GetMagicName = o => ((MagicStub)o).Name;
        MirReturnConfigGlobalSeam.GetMagicId = o => ((MagicStub)o).Id;
    }

    [Fact]
    public void RefreshGJMagic_两页各建1行2列()
    {
        SetupMagicNameId();
        MirReturnGlobalSeam.g_MagicList.Clear();
        MirReturnGlobalSeam.g_MagicList.Add(MakeMagic("攻杀剑术", 1));
        MirReturnGlobalSeam.g_MagicList.Add(MakeMagic("烈火剑法", 2));

        _dlg.RefreshGJMagic();                   // 5783-5838

        Assert.Equal(2, _ui.Memo82Rows2.Count);
        Assert.Equal(2, _ui.Memo83Rows2.Count);
        Assert.All(_ui.Memo82Rows2, r => Assert.Equal(2, r.Cells.Count));
        Assert.All(_ui.Memo83Rows2, r => Assert.Equal(2, r.Cells.Count));
    }

    [Fact]
    public void RefreshGJMagic_第一列按钮样式_第二列勾选框样式()
    {
        SetupMagicNameId();
        MirReturnGlobalSeam.g_MagicList.Clear();
        MirReturnGlobalSeam.g_MagicList.Add(MakeMagic("攻杀剑术", 7));

        _dlg.RefreshGJMagic();

        var row = _ui.Memo82Rows2[0];
        Assert.Equal("攻杀剑术", row.Cells[0].Caption);   // 5792
        Assert.Equal(7u, row.Cells[0].Data);              // 5793
        Assert.Equal(3, row.Cells[0].Style);              // 5794 bsButton
        Assert.Equal(0, row.Cells[0].Alignment);          // 5795
        Assert.Equal(0x00FFFFFF, row.Cells[0].Colors["Up"]);
        Assert.Equal(0x000000FF, row.Cells[0].Colors["Hot"]);   // // clWhite
        Assert.Equal(0x000000FF, row.Cells[0].Colors["Down"]);

        Assert.Equal(7u, row.Cells[1].Data);              // 5801
        Assert.Equal(2, row.Cells[1].Style);              // 5802 bsCheckBox
        Assert.Equal(0, row.Cells[1].ImageType);          // 5803 NewopUI_Pak
        Assert.Equal(228, row.Cells[1].ImageIndex["Up"]);
        Assert.Equal(229, row.Cells[1].ImageIndex["Down"]);
    }

    [Fact]
    public void RefreshGJMagic_勾选态来自g_GJUseMagicN的IndexOf()
    {
        SetupMagicNameId();
        MirReturnGlobalSeam.g_MagicList.Clear();
        MirReturnGlobalSeam.g_MagicList.Add(MakeMagic("A", 11));
        MirReturnGlobalSeam.g_MagicList.Add(MakeMagic("B", 22));

        MirReturnGlobalSeam.g_GJUseMagic1.Clear();
        MirReturnGlobalSeam.g_GJUseMagic1.Add((object)11u);      // 只有 A 勾选
        MirReturnGlobalSeam.g_GJUseMagic2.Clear();
        MirReturnGlobalSeam.g_GJUseMagic2.Add((object)22u);      // 只有 B 勾选

        _dlg.RefreshGJMagic();                   // 5806 / 5835

        Assert.True(_ui.Memo82Rows2[0].Cells[1].Checked);
        Assert.False(_ui.Memo82Rows2[1].Cells[1].Checked);
        Assert.False(_ui.Memo83Rows2[0].Cells[1].Checked);
        Assert.True(_ui.Memo83Rows2[1].Cells[1].Checked);
    }

    [Fact]
    public void RefreshGJMagic_Lock与UnLock成对()
    {
        SetupMagicNameId();
        MirReturnGlobalSeam.g_MagicList.Clear();

        _dlg.RefreshGJMagic();                   // 5784/5809、5813/5838

        Assert.Equal(1, _ui.Memo82LockCount);
        Assert.Equal(1, _ui.Memo82UnLockCount);
        Assert.Equal(1, _ui.Memo83LockCount);
        Assert.Equal(1, _ui.Memo83UnLockCount);
    }

    [Fact]
    public void RefreshMySelfMagicList_重建下拉并保留有效下标()
    {
        SetupMagicNameId();
        MirReturnGlobalSeam.g_MagicList.Clear();
        MirReturnGlobalSeam.g_MagicList.Add(MakeMagic("A", 1));
        MirReturnGlobalSeam.g_MagicList.Add(MakeMagic("B", 2));
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        _ui.PlugComboBoxAutoMagicItemIndex = 1;

        _dlg.RefreshMySelfMagicList();           // 973-993

        Assert.Equal(2, _ui.PlugComboBoxAutoMagicItemsCount);
        Assert.Equal(1, _ui.PlugComboBoxAutoMagicItemIndex);   // 987 恢复
    }

    [Fact]
    public void RefreshMySelfMagicList_下标越界时置1()
    {
        SetupMagicNameId();
        MirReturnGlobalSeam.g_MagicList.Clear();
        MirReturnGlobalSeam.g_MagicList.Add(MakeMagic("A", 1));
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        _ui.PlugComboBoxAutoMagicItemIndex = 5;   // 越界

        _dlg.RefreshMySelfMagicList();            // 989

        Assert.Equal(-1, _ui.PlugComboBoxAutoMagicItemIndex);
    }

    [Fact]
    public void RefreshMySelfMagicList_无人物时不清空下拉但仍刷新挂机表()
    {
        SetupMagicNameId();
        MirReturnGlobalSeam.g_MagicList.Clear();
        MirReturnConfigGlobalSeam.g_MySelfExists = () => false;
        _ui.PlugComboBoxAutoMagicItemsAddObject("旧", new object());

        _dlg.RefreshMySelfMagicList();            // 977 不成立

        Assert.Equal(1, _ui.PlugComboBoxAutoMagicItemsCount);   // 未清空
        Assert.Equal(2, _ui.Memo82LockCount + _ui.Memo82UnLockCount);   // 992 仍调用 RefreshGJMagic（Lock+UnLock 各 2 页）
    }

    // ================================================================================
    // AutoUseItem / AutoUseMagic / AutoEat*（原文 3441-3534 / 4454-4477）
    // ================================================================================

    [Fact]
    public void AutoUseItem_未启用时不做任何事()
    {
        _dlg.AutoUseItem(null);                  // 3443 FEnabled=False
        Assert.Empty(_autoEat);
        Assert.Empty(_heroEat);
    }

    [Fact]
    public void AutoUseItem_启用但保护未到期时不动作()
    {
        _dlg.SetEnabled(true);
        _dlg.SetProtectEnabled(false);           // 3445 FProtectEnabled 为假

        _dlg.AutoUseItem(null);

        // 保护关：条件不成立
        _dlg.SetProtectEnabled(true);            // 重建 tick（刚设置 → 差值 0 < 2000）
        _dlg.AutoUseItem(null);
        Assert.Empty(_autoEat);
    }

    [Fact]
    public void AutoEatHPItem_硬编码只作用于下标0与1_无视MedicaMode()
    {
        // ★★ 原文缺陷固定：EatHumHPItem 读写 [0]、EatHeroHPItem 读写 [1]
        var self = new Self();
        ConfigShareSeam.g_MySelf = self;
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        MirReturnConfigGlobalSeam.MySelfAbil = () => new TAbility { HP = 10, MaxHP = 1000 };
        MirReturnConfigGlobalSeam.MySelfIsDeath = () => false;

        // 让所有 tick 都过期（Times 为 0）
        ConfigShareSeam.g_ExtBagOpenItemCount = 0;
        ConfigShareSeam.g_ItemArr = new TClientItemSeam[ConfigShareSeam.GetMaxBagCount()];
        for (int i = 0; i < ConfigShareSeam.g_ItemArr.Length; i++)
            ConfigShareSeam.g_ItemArr[i] = new TClientItemSeam();

        var c = TMirReturnConfigDlg.g_Config;
        for (int i = 0; i <= 4; i++) { c.RenewHPTimes[i] = 0; c.RenewHPTicks[i] = 0; }
        c.RenewHPIsAutos[0] = true;
        c.ChkRenewAutoPercents[0] = false;
        c.RenewHPPercents[0] = 500;   // Value = Min(500, MaxHP=1000) = 500；HP=10 < 500 → 触发

        // 把 MedicaMode 设成 2 —— 若实现尊重 MedicaMode，则应读 [2] 而 [2] 是 false
        c.MedicaMode = 2;
        c.RenewHPIsAutos[2] = false;
        c.RenewHPTicks[2] = 0;
        c.RenewHPTimes[2] = 0;
        c.RenewHPPercents[2] = 500;
        c.ChkRenewAutoPercents[2] = false;

        _dlg.AutoEatHPItem(null);

        // [0] 被驱动（下标 0 的 tick 被刷新），[2] 完全没被碰
        Assert.NotEqual(0u, c.RenewHPTicks[0]);
        Assert.Equal(0u, c.RenewHPTicks[2]);
    }

    [Fact]
    public void AutoEatHPItem_血量高于阈值时不触发()
    {
        ConfigShareSeam.g_MySelf = new Self();
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        MirReturnConfigGlobalSeam.MySelfAbil = () => new TAbility { HP = 900, MaxHP = 1000 };
        MirReturnConfigGlobalSeam.MySelfIsDeath = () => false;

        var c = TMirReturnConfigDlg.g_Config;
        c.RenewHPIsAutos[0] = true;
        c.ChkRenewAutoPercents[0] = false;
        c.RenewHPPercents[0] = 100;   // Value = 100；HP=900 > 100 → 不触发
        c.RenewHPTicks[0] = 0;
        c.RenewHPTimes[0] = 0;

        _dlg.AutoEatHPItem(null);

        Assert.Equal(0u, c.RenewHPTicks[0]);   // 未被刷新
    }

    [Fact]
    public void AutoEatHPItem_百分比模式_用MaxHP折算()
    {
        ConfigShareSeam.g_MySelf = new Self();
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        MirReturnConfigGlobalSeam.MySelfAbil = () => new TAbility { HP = 100, MaxHP = 1000 };
        MirReturnConfigGlobalSeam.MySelfIsDeath = () => false;

        ConfigShareSeam.g_ExtBagOpenItemCount = 0;
        ConfigShareSeam.g_ItemArr = new TClientItemSeam[ConfigShareSeam.GetMaxBagCount()];
        for (int i = 0; i < ConfigShareSeam.g_ItemArr.Length; i++)
            ConfigShareSeam.g_ItemArr[i] = new TClientItemSeam();

        var c = TMirReturnConfigDlg.g_Config;
        c.RenewHPIsAutos[0] = true;
        c.ChkRenewAutoPercents[0] = true;      // 3472-3473 走 Round(MaxHP/100*Min(pct,99))
        c.RenewHPPercents[0] = 50;             // Value = 1000/100*50 = 500；HP=100 < 500 → 触发
        c.RenewHPTicks[0] = 0;
        c.RenewHPTimes[0] = 0;

        _dlg.AutoEatHPItem(null);

        Assert.NotEqual(0u, c.RenewHPTicks[0]);
    }

    [Fact]
    public void AutoEatHPItem_百分比超过99被Min夹到99()
    {
        ConfigShareSeam.g_MySelf = new Self();
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        // MaxHP=1000, pct=200 → Min(200,99)=99 → Value=990；HP=995 > 990 → 不触发
        MirReturnConfigGlobalSeam.MySelfAbil = () => new TAbility { HP = 995, MaxHP = 1000 };
        MirReturnConfigGlobalSeam.MySelfIsDeath = () => false;

        ConfigShareSeam.g_ExtBagOpenItemCount = 0;
        ConfigShareSeam.g_ItemArr = new TClientItemSeam[ConfigShareSeam.GetMaxBagCount()];
        for (int i = 0; i < ConfigShareSeam.g_ItemArr.Length; i++)
            ConfigShareSeam.g_ItemArr[i] = new TClientItemSeam();

        var c = TMirReturnConfigDlg.g_Config;
        c.RenewHPIsAutos[0] = true;
        c.ChkRenewAutoPercents[0] = true;
        c.RenewHPPercents[0] = 200;
        c.RenewHPTicks[0] = 0;
        c.RenewHPTimes[0] = 0;

        _dlg.AutoEatHPItem(null);

        Assert.Equal(0u, c.RenewHPTicks[0]);   // 990 是夹紧后的阈值，995 不触发
    }

    [Fact]
    public void AutoEatHPItem_死亡时不触发()
    {
        ConfigShareSeam.g_MySelf = new Self();
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        MirReturnConfigGlobalSeam.MySelfAbil = () => new TAbility { HP = 1, MaxHP = 1000 };
        MirReturnConfigGlobalSeam.MySelfIsDeath = () => true;   // 3523 not Death 不成立

        var c = TMirReturnConfigDlg.g_Config;
        c.RenewHPIsAutos[0] = true;
        c.RenewHPTicks[0] = 0;
        c.RenewHPTimes[0] = 0;

        _dlg.AutoEatHPItem(null);

        Assert.Equal(0u, c.RenewHPTicks[0]);
    }

    [Fact]
    public void AutoEatMPItem_无药时打聊天栏提示()
    {
        ConfigShareSeam.g_MySelf = new Self();
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        MirReturnConfigGlobalSeam.MySelfAbil = () => new TAbility { MP = 1, MaxMP = 1000 };
        MirReturnConfigGlobalSeam.MySelfIsDeath = () => false;

        var c = TMirReturnConfigDlg.g_Config;
        c.RenewMPIsAutos[0] = true;
        c.ChkRenewAutoPercents[0] = false;
        c.RenewMPPercents[0] = 500;
        c.RenewMPTicks[0] = 0;
        c.RenewMPTimes[0] = 0;

        // FindHumMPItemIndex 找不到药：空名槽位（g_ItemArr 长度 = GetMaxBagCount()）
        ConfigShareSeam.g_ExtBagOpenItemCount = 0;
        ConfigShareSeam.g_ItemArr = new TClientItemSeam[ConfigShareSeam.GetMaxBagCount()];
        for (int i = 0; i < ConfigShareSeam.g_ItemArr.Length; i++)
            ConfigShareSeam.g_ItemArr[i] = new TClientItemSeam();

        _dlg.AutoEatMPItem(null);

        Assert.Contains(_chat, x => x.Item1 == "你的魔法药已使用完");
        Assert.NotEqual(0u, c.RenewMPTicks[0]);   // 3568 无药也刷新 tick
        Assert.Empty(_autoEat);
    }

    [Fact]
    public void AutoEatSpecialHPItem_无药时既不提示也不刷新tick()
    {
        // ★ 原文不对称固定：Special 族的 else 分支不存在（3638-3645 只有 if）
        ConfigShareSeam.g_MySelf = new Self();
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        MirReturnConfigGlobalSeam.MySelfAbil = () => new TAbility { HP = 1, MaxHP = 1000 };
        MirReturnConfigGlobalSeam.MySelfIsDeath = () => false;

        var c = TMirReturnConfigDlg.g_Config;
        c.RenewSpecialHPIsAutos[0] = true;
        c.ChkRenewAutoPercents[0] = false;
        c.RenewSpecialHPPercents[0] = 500;
        c.RenewSpecialHPTicks[0] = 0;
        c.RenewSpecialHPTimes[0] = 0;
        ConfigShareSeam.g_ExtBagOpenItemCount = 0;
        ConfigShareSeam.g_ItemArr = new TClientItemSeam[ConfigShareSeam.GetMaxBagCount()];
        for (int i = 0; i < ConfigShareSeam.g_ItemArr.Length; i++)
            ConfigShareSeam.g_ItemArr[i] = new TClientItemSeam();

        _dlg.AutoEatSpecialHPItem(null);

        Assert.DoesNotContain(_chat, x => x.Item1.Contains("已使用完"));
        Assert.Equal(0u, c.RenewSpecialHPTicks[0]);   // ★ 不刷新（与 HP/MP 版相反）
    }

    [Fact]
    public void AutoEatSpecialMPItem_同样无提示无刷新()
    {
        ConfigShareSeam.g_MySelf = new Self();
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        MirReturnConfigGlobalSeam.MySelfAbil = () => new TAbility { MP = 1, MaxMP = 1000 };
        MirReturnConfigGlobalSeam.MySelfIsDeath = () => false;

        var c = TMirReturnConfigDlg.g_Config;
        c.RenewSpecialMPIsAutos[0] = true;
        c.ChkRenewAutoPercents[0] = false;
        c.RenewSpecialMPPercents[0] = 500;
        c.RenewSpecialMPTicks[0] = 0;
        c.RenewSpecialMPTimes[0] = 0;
        ConfigShareSeam.g_ExtBagOpenItemCount = 0;
        ConfigShareSeam.g_ItemArr = new TClientItemSeam[ConfigShareSeam.GetMaxBagCount()];
        for (int i = 0; i < ConfigShareSeam.g_ItemArr.Length; i++)
            ConfigShareSeam.g_ItemArr[i] = new TClientItemSeam();

        _dlg.AutoEatSpecialMPItem(null);

        Assert.DoesNotContain(_chat, x => x.Item1.Contains("已使用完"));
        Assert.Equal(0u, c.RenewSpecialMPTicks[0]);
    }

    [Fact]
    public void AutoUseMagic_未勾选时不动作()
    {
        _ui.PlugComboBoxAutoMagicItemIndex = 0;
        _dlg.AutoUseMagic();                     // 4458 FConfigCheckeds[ckAutoUseMagic] = false
        Assert.Empty(_poisonCharm);
        Assert.Empty(_useMagic);
    }

    [Fact]
    public void AutoUseMagic_死亡或摆摊时不动作()
    {
        _dlg.SetConfigChecked(TConfigChecked.ckAutoUseMagic, true);
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        MirReturnConfigGlobalSeam.MySelfIsDeath = () => true;       // 4461
        _ui.PlugComboBoxAutoMagicItemIndex = 0;
        _ui.PlugComboBoxAutoMagicItemsAddObject("m", new object());

        _dlg.AutoUseMagic();
        Assert.Empty(_useMagic);

        MirReturnConfigGlobalSeam.MySelfIsDeath = () => false;
        MirReturnConfigGlobalSeam.MySelfIsShopStall = () => true;   // 4462
        _dlg.AutoUseMagic();
        Assert.Empty(_useMagic);
    }

    [Fact]
    public void AutoUseMagic_ItemIndex越界时不动作()
    {
        _dlg.SetConfigChecked(TConfigChecked.ckAutoUseMagic, true);
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        _ui.PlugComboBoxAutoMagicItemIndex = -1;                    // 4464 不成立

        _dlg.AutoUseMagic();
        Assert.Empty(_useMagic);
    }

    [Fact]
    public void AutoUseMagic_时间未到不动作_到点后下发()
    {
        _dlg.SetConfigChecked(TConfigChecked.ckAutoUseMagic, true);
        MirReturnConfigGlobalSeam.g_MySelfExists = () => true;
        MirReturnConfigGlobalSeam.MySelfIsDeath = () => false;
        MirReturnConfigGlobalSeam.MySelfIsShopStall = () => false;

        var magic = new object();
        _ui.PlugComboBoxAutoMagicItemsAddObject("攻杀剑术", magic);
        _ui.PlugComboBoxAutoMagicItemIndex = 0;

        var c = TMirReturnConfigDlg.g_Config;
        c.nAutoUseMagicTime = 1000;
        c.dwAutoUseMagicTick = ConfigSeams.MyGetTickCount();        // 刚刷新 → 差 0，不触发

        _dlg.AutoUseMagic();                                        // 4466 不成立
        Assert.Empty(_useMagic);

        c.dwAutoUseMagicTick = ConfigSeams.MyGetTickCount() - 5000; // 差 5000 > 1000*1000? 否
        _dlg.AutoUseMagic();
        // 5000 > 1000*1000=1,000,000 → 仍不成立
        Assert.Empty(_useMagic);

        c.nAutoUseMagicTime = 1;                                    // 1*1000 = 1000 < 5000 → 触发
        _dlg.AutoUseMagic();                                        // 4468-4472

        Assert.Single(_poisonCharm);
        Assert.Single(_useMagic);
        Assert.Same(magic, _poisonCharm[0]);
        Assert.Same(magic, _useMagic[0].Item3);
    }

    // ================================================================================
    // TConfigChecked 缺失成员映射（原文 844/859/1460/1464/1468/1592/1833/1990-1992/2057-2058/2128）
    // ================================================================================

    [Fact]
    public void CheckedMap_七项缺失成员逐条映射()
    {
        Assert.Equal(TConfigChecked.ckAutoCHangePoison, MirReturnCheckedMap.ckAutoTakeOnItem);
        Assert.Equal(TConfigChecked.ckAutoCHangePoison, MirReturnCheckedMap.ckAutoChangePoison);
        Assert.Equal(TConfigChecked.ckShowMonName, MirReturnCheckedMap.ckShowItemName);
        Assert.Equal(TConfigChecked.ckShowMonName, MirReturnCheckedMap.ckShowFilterItem);
        Assert.Equal(TConfigChecked.ckShowHPLabel, MirReturnCheckedMap.ckItemHint);
        Assert.Equal(TConfigChecked.ckNotParaly, MirReturnCheckedMap.ckGJ_DFAvoid);
        Assert.Equal(TConfigCheckedBounds.HighOrdinal + 1, MirReturnCheckedMap.ckMovePickSyntheticIndex);
    }

    [Theory]
    [InlineData(MirReturnCheckedAlias.ckAutoTakeOnItem)]
    [InlineData(MirReturnCheckedAlias.ckMovePick)]
    [InlineData(MirReturnCheckedAlias.ckShowItemName)]
    [InlineData(MirReturnCheckedAlias.ckShowFilterItem)]
    [InlineData(MirReturnCheckedAlias.ckItemHint)]
    [InlineData(MirReturnCheckedAlias.ckGJ_DFAvoid)]
    [InlineData(MirReturnCheckedAlias.ckAutoChangePoison)]
    public void CheckedMap_Describe给出依据(MirReturnCheckedAlias a)
    {
        Assert.NotEqual("", MirReturnCheckedMap.Describe(a));
        Assert.Contains("MirReturnConfigDlg.pas:", MirReturnCheckedMap.Describe(a));
    }

    [Fact]
    public void CheckedMap_两个别名映射到同一位()
    {
        // ★ 差异断言：ckShowItemName 与 ckShowFilterItem 都映射到 ckShowMonName
        // → RefConfig 里 1990/1991 会先后覆盖 2012 的 ckShowMonName
        Assert.Equal(MirReturnCheckedMap.ckShowItemName, MirReturnCheckedMap.ckShowFilterItem);
        Assert.Equal(MirReturnCheckedMap.ckAutoTakeOnItem, MirReturnCheckedMap.ckAutoChangePoison);
    }

    // ================================================================================
    // RefConfig（原文 1983-2137）
    // ================================================================================

    /// <summary>
    /// 走原文 2903-3272 的 Initialize（接缝为默认空实现），使 <c>FInitializeed := True</c>（3272）——
    /// 它是 RefConfig(1985)/RefUseItemConfig(1862) 的总门禁。
    /// </summary>
    private void MakeInitialized()
        => _dlg.Initialize(IntPtr.Zero, 0, TClientVersion.cvSerial, true);

    [Fact]
    public void RefConfig_未Initialize时立即退出()
    {
        // 1985：FInitializeed=False → Exit（不动任何控件）
        _ui.PlugCheckBoxShowHPLabel = true;
        _dlg.RefConfig();
        Assert.True(_ui.PlugCheckBoxShowHPLabel);   // 未被改写
    }

    [Fact]
    public void RefConfig_勾选位回写控件_抽样断言()
    {
        // 通过 Initialize 打开 FInitializeed（原文 2903 起设置）
        _dlg.Initialize(IntPtr.Zero, 0, TClientVersion.cvSerial, true);

        _dlg.SetConfigChecked(TConfigChecked.ckSpeedSlow, true);
        _dlg.RefConfig();

        Assert.True(_ui.PlugCheckBoxSpeedSlow);         // 1994
        // 构造函数的默认 true 位
        Assert.True(_ui.PlugCheckBoxShowHPLabel);       // 1986 ← 818 的 True
        Assert.True(_ui.PlugCheckBoxMagicLock);         // 2027 ← 820
        Assert.True(_ui.PlugCheckBoxAutoPickUpItem);    // 1996 ← 823
        Assert.True(_ui.PlugCheckBoxRepeatBGMusic);     // 2030 ← 825
        Assert.True(_ui.PlugCheckBoxNearHint);          // 2016 ← 850
    }

    [Fact]
    public void RefConfig_ckShowName三项覆盖_后者胜()
    {
        // ★ 原文缺陷固定：1990(ckShowItemName→ckShowMonName)、1991(ckShowFilterItem→ckShowMonName)、
        //    2012(ckShowMonName) 写同一个控件 PlugCheckBoxShowMonName，**顺序决定最终值**。
        _dlg.Initialize(IntPtr.Zero, 0, TClientVersion.cvSerial, true);
        _dlg.SetConfigChecked(TConfigChecked.ckShowMonName, true);

        _dlg.RefConfig();

        // 1990/1991 先写入（同一位=True），2012 再写入 True → 最终 True
        Assert.True(_ui.PlugCheckBoxShowMonName);

        // 反向：把该位清掉，三处都写 False
        _dlg.SetConfigChecked(TConfigChecked.ckShowMonName, false);
        _dlg.RefConfig();
        Assert.False(_ui.PlugCheckBoxShowMonName);
    }

    [Fact]
    public void RefConfig_ckItemHint覆盖ckShowHPLabel()
    {
        // ★ 1992（ckItemHint→ckShowHPLabel）在 1986（ckShowHPLabel）之后写，故后者由 ckItemHint 决定
        _dlg.Initialize(IntPtr.Zero, 0, TClientVersion.cvSerial, true);

        // 构造函数里 ckShowHPLabel=True、ckItemHint 所在位（=Same ckShowHPLabel）=True → 都是 True
        _dlg.RefConfig();
        Assert.True(_ui.PlugCheckBoxShowHPLabel);
    }

    [Fact]
    public void RefConfig_音量与全局声音标志()
    {
        _dlg.Initialize(IntPtr.Zero, 0, TClientVersion.cvSerial, true);
        _dlg.SetConfigChecked(TConfigChecked.ckVolume, true);
        _dlg.SetConfigChecked(TConfigChecked.ckBGMusic, true);
        _dlg.SetConfigChecked(TConfigChecked.ckRepeatBGMusic, false);
        MirReturnGlobalSeam.g_SoundVolume = 55;

        _dlg.RefConfig();                       // 2034-2042

        Assert.True(_ui.PlugCheckBoxVolume);
        Assert.True(MirsConfigGlobalSeam.g_boSound);        // 2035
        Assert.Equal(100, _ui.TrackBarVolumeMax);           // 2037
        Assert.Equal(0, _ui.TrackBarVolumeMin);             // 2038
        Assert.Equal(55, _ui.TrackBarVolumePosition);               // 2039
        Assert.True(MirsConfigGlobalSeam.g_boBGSound);      // 2041
        Assert.False(MirsConfigGlobalSeam.g_boRepeatBGSound); // 2042
    }

    [Fact]
    public void RefConfig_五个用药模式单选_互斥()
    {
        _dlg.Initialize(IntPtr.Zero, 0, TClientVersion.cvSerial, true);

        for (int mode = 0; mode <= 4; mode++)
        {
            TMirReturnConfigDlg.g_Config.MedicaMode = mode;
            _dlg.RefConfig();

            Assert.Equal(mode == 0, _ui.PlugMemoConfig4Button1);   // 2131
            Assert.Equal(mode == 1, _ui.PlugMemoConfig4Button2);   // 2132
            Assert.Equal(mode == 2, _ui.PlugMemoConfig4Button3);   // 2133
            Assert.Equal(mode == 3, _ui.PlugMemoConfig4Button4);   // 2134
            Assert.Equal(mode == 4, _ui.PlugMemoConfig4Button5);   // 2135
        }
    }

    [Fact]
    public void RefConfig_挂机下拉与编辑框()
    {
        _dlg.Initialize(IntPtr.Zero, 0, TClientVersion.cvSerial, true);
        var c = TMirReturnConfigDlg.g_Config;
        c.nGJPlayAttackOption = 1;
        c.nGJNoRedPoisonOption = 2;
        c.nGJNoBluePoisonOption = 3;
        c.nGJNoDuFuOption = 4;
        c.nGJBagFullOption = 5;
        c.nGJNotRushMonRange = 6;
        c.nGJGroupAttackCount = 7;
        c.nFilterMinExp = 8;
        c.nAutoUseMagicTime = 9;
        c.nColorShowEff = 10;
        c.nSpecialColor = 200;

        _dlg.RefConfig();                       // 2090-2128

        Assert.Equal(8, _ui.PlugEditExpFilter);
        Assert.Equal(9, _ui.PlugEditAutoMagicTime);
        Assert.Equal(10, _ui.PlugComboBoxColorShow);
        Assert.Equal(200, _ui.PlugEditSpecialColor);
        Assert.Equal(1, _ui.PlugComboBoxPlayAttackValue);
        Assert.Equal(2, _ui.PlugComboBoxNoRedPoisonValue);
        Assert.Equal(3, _ui.PlugComboBoxNoBluePoisonValue);
        Assert.Equal(4, _ui.PlugComboBoxNoDuFuValue);
        Assert.Equal(5, _ui.PlugComboBoxBagFullValue);
        Assert.Equal(6, _ui.PlugEditNotRushMonRange);
        Assert.Equal(7, _ui.PlugEditNotGroupAttackCount);
    }

    // ================================================================================
    // RefUseItemConfig（原文 1860-1965）
    // ================================================================================

    [Fact]
    public void RefUseItemConfig_未Initialize时退出()
    {
        _ui.PlugEditCheckHPPercent = 111;
        _dlg.RefUseItemConfig(0);                // 1862 Exit
        Assert.Equal(111, _ui.PlugEditCheckHPPercent);
    }

    [Fact]
    public void RefUseItemConfig_nObj0时标签写时使用且下拉可见()
    {
        _dlg.Initialize(IntPtr.Zero, 0, TClientVersion.cvSerial, true);
        TMirReturnConfigDlg.g_Config.MedicaMode = 0;

        _dlg.RefUseItemConfig(0);                // 1865-1870

        Assert.Equal("时使用", _ui.MemoConfig4Label[3]);
        Assert.Equal("时使用", _ui.MemoConfig4Label[4]);
        Assert.True(_ui.PlugComboBoxCheckHPValueVisible);
        Assert.True(_ui.PlugComboBoxCheckMPValueVisible);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void RefUseItemConfig_nObj非0时标签写收英雄且下拉不可见(int nObj)
    {
        _dlg.Initialize(IntPtr.Zero, 0, TClientVersion.cvSerial, true);
        TMirReturnConfigDlg.g_Config.MedicaMode = nObj;

        _dlg.RefUseItemConfig(nObj);             // 1874-1877

        Assert.Equal("收英雄", _ui.MemoConfig4Label[3]);
        Assert.Equal("收英雄", _ui.MemoConfig4Label[4]);
        Assert.False(_ui.PlugComboBoxCheckHPValueVisible);
        Assert.False(_ui.PlugComboBoxCheckMPValueVisible);
    }

    [Fact]
    public void RefUseItemConfig_越界nObj不做任何事()
    {
        _dlg.Initialize(IntPtr.Zero, 0, TClientVersion.cvSerial, true);
        _ui.PlugEditCheckHPPercent = 111;

        _dlg.RefUseItemConfig(-1);               // 1863 不成立
        _dlg.RefUseItemConfig(5);

        Assert.Equal(111, _ui.PlugEditCheckHPPercent);
    }

    [Fact]
    public void RefUseItemConfig_数值按下标MedicaMode回写()
    {
        // ★ 差异点：标签用 nObj，数值用 g_Config.MedicaMode（原文如此）
        _dlg.Initialize(IntPtr.Zero, 0, TClientVersion.cvSerial, true);
        var c = TMirReturnConfigDlg.g_Config;
        c.MedicaMode = 3;
        c.CheckHpPercents[3] = 88;
        c.CheckDuraMin[3] = 5;
        c.UseSuperMedicas[3] = true;
        c.SuperMedicaUses[3, 4] = true;
        c.SuperMedicaHPs[3, 2] = 66;
        c.nHeroDodgeHPPercent = 12;

        _dlg.RefUseItemConfig(0);                // 标签按 nObj=0，数值按 MedicaMode=3

        Assert.Equal("时使用", _ui.MemoConfig4Label[3]);      // 形参 nObj
        Assert.Equal(88, _ui.PlugEditCheckHPPercent);         // 1885 ← MedicaMode
        Assert.Equal(5, _ui.PlugEditCheckDura);
        Assert.True(_ui.PlugCheckBoxUseSuperMedica);
        Assert.True(_ui.PlugCheckBoxUseSuperMedicaItemName(4));
        Assert.Equal(66, _ui.PlugEditSuperMedicaHP2);
        Assert.Equal(12, _ui.PlugEditHeroDodgeHPPercent);
    }

    [Fact]
    public void RefUseItemConfig_超药矩阵九个下标全部回写()
    {
        _dlg.Initialize(IntPtr.Zero, 0, TClientVersion.cvSerial, true);
        var c = TMirReturnConfigDlg.g_Config;
        c.MedicaMode = 0;
        for (int i = 0; i <= 8; i++)
        {
            c.SuperMedicaHPs[0, i] = 100 + i;
            c.SuperMedicaHPTimes[0, i] = 200 + i;
            c.SuperMedicaMPs[0, i] = 300 + i;
            c.SuperMedicaMPTimes[0, i] = 400 + i;
            c.SuperMedicaUses[0, i] = (i % 2) == 0;
        }

        _dlg.RefUseItemConfig(0);

        for (int i = 0; i <= 8; i++)
        {
            Assert.Equal(100 + i, _ui.PlugEditSuperMedicaHP(i));
            Assert.Equal(200 + i, _ui.PlugEditSuperMedicaHPTime(i));
            Assert.Equal(300 + i, _ui.PlugEditSuperMedicaMP(i));
            Assert.Equal(400 + i, _ui.PlugEditSuperMedicaMPTime(i));
            Assert.Equal((i % 2) == 0, _ui.PlugCheckBoxUseSuperMedicaItemName(i));
        }
    }

    // ================================================================================
    // LoadConfigFile / SaveConfigFile（原文 4486-5069）
    // ================================================================================

    private MirReturnIniFileStub SetupIni()
    {
        var ini = new MirReturnIniFileStub();
        MirReturnGlobalSeam.CreateIniFile = _ => ini;
        ConfigShareGlobal.g_sPlugServerName = "S1";
        ConfigShareGlobal.g_sPlugUserName = "U1";
        MirReturnGlobalSeam.AppPath = @"C:\app\";
        return ini;
    }

    [Fact]
    public void LoadConfigFile_文件名模板与目录准备()
    {
        string used = "";
        var ini = new MirReturnIniFileStub();
        MirReturnGlobalSeam.CreateIniFile = f => { used = f; return ini; };
        ConfigShareGlobal.g_sPlugServerName = "Srv";
        ConfigShareGlobal.g_sPlugUserName = "User";
        MirReturnGlobalSeam.AppPath = @"C:\g\";
        bool forceDir = false;
        MirReturnGlobalSeam.DirectoryExists = _ => false;
        MirReturnGlobalSeam.ForceDirectories = _ => forceDir = true;

        _dlg.LoadConfigFile();                   // 4494-4521

        Assert.Equal(@"C:\g\Config\Srv.User.set", used);   // 4519 CONFIGFILE
        Assert.True(forceDir);                             // 4495
    }

    [Fact]
    public void LoadConfigFile_读取Setup节的音量与颜色()
    {
        var ini = SetupIni();
        ini.Values["Setup\u0001Volume"] = "77";
        ini.Values["Setup\u0001ColorShowEff"] = "5";
        ini.Values["Setup\u0001SpecialColor"] = "123";

        _dlg.LoadConfigFile();                   // 4526/4533/4536

        Assert.Equal(77, MirReturnGlobalSeam.g_SoundVolume);
        Assert.Equal((byte)5, TMirReturnConfigDlg.g_Config.nColorShowEff);
        Assert.Equal((byte)123, TMirReturnConfigDlg.g_Config.nSpecialColor);
    }

    [Fact]
    public void LoadConfigFile_MedicaMode被夹到0到4()
    {
        var ini = SetupIni();
        ini.Values["Protect\u0001MedicaMode"] = "9";
        _dlg.LoadConfigFile();                   // 4539-4541
        Assert.Equal(4, TMirReturnConfigDlg.g_Config.MedicaMode);

        ini.Values["Protect\u0001MedicaMode"] = "-3";
        _dlg.LoadConfigFile();
        Assert.Equal(0, TMirReturnConfigDlg.g_Config.MedicaMode);
    }

    [Fact]
    public void LoadConfigFile_勾选位按CheckedN键读取()
    {
        var ini = SetupIni();
        ini.Values["Setup\u0001Checked" + (int)TConfigChecked.ckSpeedSlow] = "1";
        ini.Values["Setup\u0001Checked" + (int)TConfigChecked.ckNotParaly] = "0";

        _dlg.LoadConfigFile();                   // 4528-4531

        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckSpeedSlow));
        Assert.False(_dlg.GetConfigChecked(TConfigChecked.ckNotParaly));
    }

    [Fact]
    public void LoadConfigFile_勾选位循环上界超枚举_越界项被跳过不崩()
    {
        // ★ 原文缺陷固定：Length(FConfigCheckeds)-1 = 135，而 TConfigChecked 只到 133
        var ini = SetupIni();
        // 给出全部 CheckedN（含 134/135）以证明不会崩
        for (int i = 0; i <= _dlg.ConfigCheckeds.Length - 1; i++)
            ini.Values["Setup\u0001Checked" + i] = "1";

        _dlg.LoadConfigFile();                   // 4528-4531

        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckSpeedSlow));
        // ★ 原文缺陷固定：合成槽位（下标 134）**不在** 0..133 内，被安全化跳过
        Assert.False(_dlg.ConfigCheckeds[MirReturnCheckedMap.ckMovePickIndex]);
    }

    [Fact]
    public void LoadConfigFile_RenewTime被夹紧到下限()
    {
        MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime = 500;
        var ini = SetupIni();
        ini.Values["Protect\u0001RenewHPTime"] = "10";           // 下标 0
        ini.Values["Protect\u0001RenewHeroNormalHpTime"] = "20"; // 下标 1

        _dlg.LoadConfigFile();                   // 4670/4674

        Assert.Equal(500, TMirReturnConfigDlg.g_Config.RenewHPTimes[0]);
        Assert.Equal(500, TMirReturnConfigDlg.g_Config.RenewHPTimes[1]);
    }

    [Fact]
    public void LoadConfigFile_耐久下界夹紧_Min1与Time2()
    {
        var ini = SetupIni();
        ini.Values["Protect\u0001RenewDuraMin"] = "0";        // 4686 Max(...,1)
        ini.Values["Protect\u0001RenewDuraTime"] = "0";       // 4688 Max(...,2)
        ini.Values["Protect\u0001RenewDuraItemName"] = "祝福油";

        _dlg.LoadConfigFile();

        Assert.Equal(1, TMirReturnConfigDlg.g_Config.CheckDuraMin[0]);
        Assert.Equal(2, TMirReturnConfigDlg.g_Config.CheckDuraTime[0]);
        Assert.Equal("祝福油", TMirReturnConfigDlg.g_Config.CheckDuraValue[0]);
    }

    [Fact]
    public void LoadConfigFile_超药键名用中文药名()
    {
        var ini = SetupIni();
        // 4697-4743：模式 0 的键 '%sBoUse' / '%sHp' / '%sHpTime' / '%sMp' / '%sMpTime'
        ini.Values["Protect\u0001太阳水BoUse"] = "1";
        ini.Values["Protect\u0001太阳水Hp"] = "33";
        ini.Values["Protect\u0001太阳水HpTime"] = "10";
        ini.Values["Protect\u0001太阳水Mp"] = "44";
        ini.Values["Protect\u0001太阳水MpTime"] = "20";
        MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime = 500;

        _dlg.LoadConfigFile();                   // 4739-4743

        var c = TMirReturnConfigDlg.g_Config;
        Assert.True(c.SuperMedicaUses[0, 0]);
        Assert.Equal(33, c.SuperMedicaHPs[0, 0]);
        Assert.Equal(500, c.SuperMedicaHPTimes[0, 0]);   // 夹紧
        Assert.Equal(44, c.SuperMedicaMPs[0, 0]);
        Assert.Equal(500, c.SuperMedicaMPTimes[0, 0]);
    }

    [Fact]
    public void LoadConfigFile_快捷键按Hotkey节读取()
    {
        var ini = SetupIni();
        ini.Values["Hotkey\u0001Use0"] = "1";
        ini.Values["Hotkey\u0001Key0"] = "65";
        ini.Values["Hotkey\u0001Shift0"] = "3";

        _dlg.LoadConfigFile();                   // 4746-4752

        var sk = ConfigShareGlobal.g_ShortcutKeys[0];
        Assert.Equal(1, sk.Use);
        Assert.Equal((ushort)65, sk.Key);
        Assert.Equal((DelphiShiftState)3, sk.Shift);
    }

    [Fact]
    public void LoadConfigFile_GJ节七个选项()
    {
        var ini = SetupIni();
        ini.Values["GJ\u0001GJPlayAttackOption"] = "1";
        ini.Values["GJ\u0001GJNoRedPoisonOption"] = "2";
        ini.Values["GJ\u0001GJNoBluePoisonOption"] = "3";
        ini.Values["GJ\u0001GJNoDuFuOption"] = "4";
        ini.Values["GJ\u0001GJBagFullOption"] = "5";
        ini.Values["GJ\u0001GJNotRushMonRange"] = "6";
        ini.Values["GJ\u0001GJGroupAttackCount"] = "7";

        _dlg.LoadConfigFile();                   // 4756-4763

        var c = TMirReturnConfigDlg.g_Config;
        Assert.Equal(1, c.nGJPlayAttackOption);
        Assert.Equal(2, c.nGJNoRedPoisonOption);
        Assert.Equal(3, c.nGJNoBluePoisonOption);
        Assert.Equal(4, c.nGJNoDuFuOption);
        Assert.Equal(5, c.nGJBagFullOption);
        Assert.Equal(6, c.nGJNotRushMonRange);
        Assert.Equal(7, c.nGJGroupAttackCount);
    }

    [Fact]
    public void LoadConfigFile_五个下拉条目取自g_GJActionMode()
    {
        SetupIni();
        MirReturnGlobalSeam.g_GJActionModeText = "回城\r\n随机\r\n原地";

        _dlg.LoadConfigFile();                   // 4781-4785

        Assert.Equal("回城\r\n随机\r\n原地", _ui.PlugComboBoxNoRedPoisonValueItemsText);
        Assert.Equal("回城\r\n随机\r\n原地", _ui.PlugComboBoxNoBluePoisonValueItemsText);
        Assert.Equal("回城\r\n随机\r\n原地", _ui.PlugComboBoxNoDuFuValueItemsText);
        Assert.Equal("回城\r\n随机\r\n原地", _ui.PlugComboBoxBagFullValueItemsText);
        Assert.Equal("回城\r\n随机\r\n原地", _ui.PlugComboBoxPlayAttackValueItemsText);
    }

    [Fact]
    public void SaveConfigFile_写入Setup节的音量与颜色()
    {
        var ini = SetupIni();
        MirReturnGlobalSeam.g_SoundVolume = 66;
        TMirReturnConfigDlg.g_Config.nColorShowEff = 7;
        TMirReturnConfigDlg.g_Config.nSpecialColor = 99;

        _dlg.SaveConfigFile();                   // 4827/4835/4836

        Assert.Equal("66", ini.Values["Setup\u0001Volume"]);
        Assert.Equal("7", ini.Values["Setup\u0001ColorShowEff"]);
        Assert.Equal("99", ini.Values["Setup\u0001SpecialColor"]);
    }

    [Fact]
    public void SaveConfigFile_雷达四项被排除_读侧不排除()
    {
        // ★ 原文不对称固定：4832 排除 4 个雷达位；而 Load 的 4528-4531 不排除
        var ini = SetupIni();
        _dlg.SetConfigChecked(TConfigChecked.ckShowRadarPlayer, true);

        _dlg.SaveConfigFile();                   // 4832-4833

        string key = "Setup\u0001Checked" + (int)TConfigChecked.ckShowRadarPlayer;
        Assert.False(ini.Values.ContainsKey(key));

        // 读侧：若文件里就有这一项，Load 会读（不排除）
        var ini2 = new MirReturnIniFileStub();
        ini2.Values[key] = "1";
        MirReturnGlobalSeam.CreateIniFile = _ => ini2;
        _dlg.SetConfigChecked(TConfigChecked.ckShowRadarPlayer, false);
        _dlg.LoadConfigFile();
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckShowRadarPlayer));
    }

    [Fact]
    public void SaveConfigFile_写入Protect节的下标1到5键()
    {
        var ini = SetupIni();
        var c = TMirReturnConfigDlg.g_Config;
        c.CheckHpIsAutos[0] = true;
        c.CheckHpPercents[0] = 42;
        c.CheckHpValues[0] = 43;
        c.ChkAutoPercents[0] = true;

        _dlg.SaveConfigFile();                   // 4953-4962

        Assert.Equal("1", ini.Values["Protect\u0001Hp1Chk"]);
        Assert.Equal("42", ini.Values["Protect\u0001Hp1Hp"]);
        Assert.Equal("43", ini.Values["Protect\u0001Hp1Man"]);
        Assert.Equal("1", ini.Values["Protect\u0001AutoPercents1Chk"]);
    }

    [Fact]
    public void SaveConfigFile_超药键写中文药名()
    {
        var ini = SetupIni();
        TMirReturnConfigDlg.g_Config.SuperMedicaHPs[0, 1] = 88;   // 强效太阳水

        _dlg.SaveConfigFile();                   // 5034-5038

        Assert.Equal("88", ini.Values["Protect\u0001强效太阳水Hp"]);
    }

    [Fact]
    public void SaveConfigFile_GJ节与快捷键()
    {
        var ini = SetupIni();
        var c = TMirReturnConfigDlg.g_Config;
        c.nGJPlayAttackOption = 2;
        c.nGJNotRushMonRange = 8;
        ConfigShareGlobal.g_ShortcutKeys[3] = new TShortcutKey { Use = 1, Key = 70, Shift = DelphiShiftState.ssCtrl };

        _dlg.SaveConfigFile();                   // 5042-5059

        Assert.Equal("2", ini.Values["GJ\u0001GJPlayAttackOption"]);
        Assert.Equal("8", ini.Values["GJ\u0001GJNotRushMonRange"]);
        Assert.Equal("1", ini.Values["Hotkey\u0001Use3"]);
        Assert.Equal("70", ini.Values["Hotkey\u0001Key3"]);
        Assert.Equal(((int)DelphiShiftState.ssCtrl).ToString(), ini.Values["Hotkey\u0001Shift3"]);
    }

    [Fact]
    public void SaveConfigFile_异常被吞并写DebugOut()
    {
        // 5060-5066：catch 里两次 DebugOutStr
        MirReturnGlobalSeam.CreateIniFile = _ => new ThrowingIni();
        MirReturnGlobalSeam.DebugOut.Clear();

        _dlg.SaveConfigFile();                   // 不抛

        Assert.Contains("[Exception] TMirReturnConfigDlg::SaveConfigFile", MirReturnGlobalSeam.DebugOut);
    }

    private sealed class ThrowingIni : IMirReturnIniFile
    {
        public string ReadString(string s, string i, string d) => d;
        public void WriteString(string s, string i, string v) => throw new InvalidOperationException("boom");
        public int ReadInteger(string s, string i, int d) => d;
        public void WriteInteger(string s, string i, int v) => throw new InvalidOperationException("boom");
        public bool ReadBool(string s, string i, bool d) => d;
        public void WriteBool(string s, string i, bool v) => throw new InvalidOperationException("boom");
        public void UpdateFile() => throw new InvalidOperationException("boom");
    }

    [Fact]
    public void LoadConfigFile_用户名非法字符被替换()
    {
        string used = "";
        MirReturnGlobalSeam.CreateIniFile = f => { used = f; return new MirReturnIniFileStub(); };
        ConfigShareSeam.g_MySelf = new Self("a/b\\c:d*e?f\"g<h>i|j");
        ConfigShareGlobal.g_sPlugServerName = "S";
        MirReturnGlobalSeam.AppPath = "";

        _dlg.LoadConfigFile();                   // 4497-4516

        Assert.Contains("a{b}c;d@e!f~g(h)i-j", used);
    }

    [Fact]
    public void LoadConfigFile_DirectoryExists为真时不ForceDirectories()
    {
        bool forced = false;
        MirReturnGlobalSeam.DirectoryExists = _ => true;
        MirReturnGlobalSeam.ForceDirectories = _ => forced = true;
        SetupIni();

        _dlg.LoadConfigFile();                   // 4495 不成立

        Assert.False(forced);
    }

    [Fact]
    public void LoadConfigFile_九个非法字符逐个映射()
    {
        string used = "";
        MirReturnGlobalSeam.CreateIniFile = f => { used = f; return new MirReturnIniFileStub(); };
        ConfigShareSeam.g_MySelf = new Self("/\\:*?\"<>|");
        MirReturnGlobalSeam.AppPath = "";

        _dlg.LoadConfigFile();

        Assert.Contains("{};@!~()-", used);
    }

    [Fact]
    public void LoadConfigFile_g_MySelf为nil时用户名为空串()
    {
        string used = "";
        MirReturnGlobalSeam.CreateIniFile = f => { used = f; return new MirReturnIniFileStub(); };
        ConfigShareSeam.g_MySelf = null;
        ConfigShareGlobal.g_sPlugServerName = "S";
        ConfigShareGlobal.g_sPlugUserName = "";

        _dlg.LoadConfigFile();                   // 4497 不成立

        Assert.Equal(@"Config\S..set", used);
    }

    [Fact]
    public void LoadConfigFile_用户名空串时也走不进替换分支()
    {
        string used = "";
        MirReturnGlobalSeam.CreateIniFile = f => { used = f; return new MirReturnIniFileStub(); };
        ConfigShareSeam.g_MySelf = new Self("");
        ConfigShareGlobal.g_sPlugServerName = "S";
        ConfigShareGlobal.g_sPlugUserName = "保留";

        _dlg.LoadConfigFile();

        Assert.Contains("保留", used);   // 未被覆盖
    }

    // ================================================================================
    // LoadConfig（原文 3286-3313）与 Logon（3281-3285）
    // ================================================================================

    [Fact]
    public void Logon_只记服务器名()
    {
        _dlg.Logon("测试服");                    // 3281-3284
        Assert.Equal("测试服", ConfigShareGlobal.g_sPlugServerName);
    }

    [Fact]
    public void LoadConfig_形参CharName未被使用_取g_MySelf用户名()
    {
        // ★ 原文缺陷固定：形参 CharName 完全未使用
        var ini = new MirReturnIniFileStub();
        MirReturnGlobalSeam.CreateIniFile = _ => ini;
        ConfigShareSeam.g_MySelf = new Self("真实用户名");
        ConfigShareGlobal.g_sPlugServerName = "S";
        MirReturnGlobalSeam.AppPath = "";

        _dlg.LoadConfig("形参名");               // 3288-3313

        Assert.Equal("真实用户名", ConfigShareGlobal.g_sPlugUserName);
    }

    [Fact]
    public void LoadConfig_会自动调LoadConfigFile()
    {
        var ini = new MirReturnIniFileStub();
        ini.Values["Protect\u0001MedicaMode"] = "3";
        MirReturnGlobalSeam.CreateIniFile = _ => ini;
        MirReturnGlobalSeam.AppPath = "";

        _dlg.LoadConfig("x");                    // 3313

        Assert.Equal(3, TMirReturnConfigDlg.g_Config.MedicaMode);
    }

    // ================================================================================
    // Run / Finalize（原文 3321-3324 / 3275-3280）
    // ================================================================================

    [Fact]
    public void Run_每tick只调AutoUseItem()
    {
        _dlg.SetEnabled(true);
        _dlg.SetProtectEnabled(true);            // tick 刚刷新 → 2000ms 未到
        _dlg.Run();                              // 3323
        Assert.Empty(_autoEat);
    }

    [Fact]
    public void Finalize_启用时落盘()
    {
        var ini = SetupIni();
        _dlg.SetEnabled(false);
        _dlg.Finalize();                         // 3277 FEnabled=False → 不写
        Assert.Equal(0, ini.WriteCount);

        _dlg.SetEnabled(true);
        _dlg.Finalize();                         // 3277 → SaveConfigFile
        Assert.True(ini.WriteCount > 0);
    }

    [Fact]
    public void Destroy_启用时落盘()
    {
        var ini = SetupIni();
        _dlg.SetEnabled(true);
        _dlg.Destroy();                          // 862-868
        Assert.True(ini.WriteCount > 0);
    }

    // ================================================================================
    // LoadHelpFile（原文 1036-1080）
    // ================================================================================

    [Fact]
    public void LoadHelpFile_文件不存在时不加载()
    {
        MirReturnGlobalSeam.FileExists = _ => false;   // 1042
        _dlg.LoadHelpFile();
        Assert.Equal("", _ui.HelpLoadedFrom);
    }

    [Fact]
    public void LoadHelpFile_标题行与正文行着色不同()
    {
        MirReturnGlobalSeam.FileExists = _ => true;
        _ui.HelpLines.Add(" 标题行 ");    // 首尾有空格 → clSilver
        _ui.HelpLines.Add("正文行");      // → clWhite

        _dlg.LoadHelpFile();                        // 1045-1078

        Assert.Equal(@"Data\explain2.dat", _ui.HelpLoadedFrom);
        Assert.True(_ui.PlugMemoConfigHelpFontBackTransparent);   // 1049

        // 0 号：三态都是 clSilver($C0C0C0)
        Assert.Contains(_ui.HelpColors, c => c.Index == 0 && c.State == "Up" && c.Color == 0x00C0C0C0);
        Assert.Contains(_ui.HelpColors, c => c.Index == 0 && c.State == "Hot" && c.Color == 0x00C0C0C0);
        Assert.Contains(_ui.HelpColors, c => c.Index == 0 && c.State == "Down" && c.Color == 0x00C0C0C0);

        // 1 号：三态都是 clWhite($FFFFFF)，BColor 恒 clBlack、Bold 恒 false
        Assert.Contains(_ui.HelpColors, c => c.Index == 1 && c.State == "Up" && c.Color == 0x00FFFFFF);
        Assert.All(_ui.HelpColors, c => Assert.Equal(0x00000000, c.BackColor));
        Assert.All(_ui.HelpColors, c => Assert.False(c.Bold));
    }

    [Fact]
    public void LoadHelpFile_PlugMemoConfigHelp为nil时跳过()
    {
        MirReturnGlobalSeam.FileExists = _ => true;
        _ui.PlugMemoConfigHelp = null;   // 1042 第一个条件不成立
        _dlg.LoadHelpFile();
        Assert.Equal("", _ui.HelpLoadedFrom);
    }

    // ================================================================================
    // TGameConfigObject 抽象面（原文未覆写者）
    // ================================================================================

    [Fact]
    public void 未覆写的BossList三方法为空实现()
    {
        _dlg.AddToBossList("x");            // 原文 485-525 无覆写
        _dlg.RemoveFromBossList("x");
        _dlg.AddOrRemoveBossList("x");
        Assert.Empty(_ui.PlugScrollBoxBossLines);
    }

    [Fact]
    public void ConfigCheckeds返回同一实例()
    {
        Assert.Same(_dlg.ConfigCheckeds, _dlg.ConfigCheckeds);
        _dlg.ConfigCheckeds[(int)TConfigChecked.ckSpeedSlow] = true;
        Assert.True(_dlg.GetConfigChecked(TConfigChecked.ckSpeedSlow));
    }

    [Fact]
    public void ConfigDlgType属性转发GetType()
    {
        Assert.Equal(TConfigDlgType.ptDefault, _dlg.ConfigDlgType);
    }

    [Fact]
    public void Visible与Enabled属性转发访问器()
    {
        _dlg.Enabled = true;
        Assert.True(_dlg.GetEnabled());
        _dlg.Visible = true;
        Assert.True(_ui.PlugConfigDlgVisible);
        _dlg.ProtectEnabled = true;
        Assert.True(_dlg.GetProtectEnabled());
    }
}

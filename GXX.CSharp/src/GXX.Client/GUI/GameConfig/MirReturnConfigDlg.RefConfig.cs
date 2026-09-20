// 源单元：Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas（GBK，5,954 行，CRLF）
// 本分片覆盖（原文行号）：
//   1860-1965  RefUseItemConfig（5 档用药模式的**控件回写**，含 1867-1877 的标签/可见性切换）
//   1983-2137  RefConfig（配置位 → 控件回写；**76 处 FConfigCheckeds[] 赋值 + 音量/下拉/单选**）
//   5842-5940  见 Controls 分片
//
// ★★ RefConfig 的三处原文缺陷（本车道登记，逐行照抄，不回填"修正"）：
//   1) 2023 行用的是 **ConfigCheckeds[ckSceneShake]**（**无 F 前缀**），而同段其余全是
//      FConfigCheckeds。原文如此 —— 若该标识符在 Delphi 侧不存在则本单元根本编译不过，
//      故按"与 FConfigCheckeds 同物"处理（托管侧两者本就是同一数组，见 ConfigCheckeds 属性）。
//   2) 1992/1990/1991 用的 ckItemHint / ckShowItemName / ckShowFilterItem 与
//      2058 的 ckAutoChangePoison 在检出枚举里**不存在**（见 MirReturnCheckedMap 的映射裁定），
//      其中 ckShowItemName 与 ckShowFilterItem 映射到**同一个** ckShowMonName 位
//      → 2012 行（ckShowMonName）会被 1990/1991 行**覆盖**，三行的最终值取决于赋值顺序。
//   3) 2029 行 **重复**赋值 ckBGMusic（1995 行已赋过一次）；2085 行 **重复**赋值 ckAutoUseMagic（2080 行）。
//      原文如此（冗余赋值，结果相同）。
//   4) 2053 行用 ckSmart66Hit（检出枚举里是 ckSmart66Hit，OK）而控件名是 PlugCheckBoxSmartKTZHit；
//      2062 行用 ckHeroAutoShield、2063 行用 ckAssistantHeroAutoShield —— 均存在，无需映射。

using System;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig;

public partial class TMirReturnConfigDlg
{
    // ================================================================================
    // MirReturnConfigDlg.pas:1860-1965  RefUseItemConfig
    // ================================================================================

    /// <summary>
    /// 原文 1860-1965：
    /// <c>if not FInitializeed then Exit;</c> → <c>if nObj in [0..4] then</c> →
    /// <c>if nObj = 0 then</c> 标签写 '时使用' + 两个下拉 <c>Visible := True</c>；
    /// <c>else</c> 标签写 '收英雄' + 两个下拉 <c>Visible := False</c>
    /// （★ 注意用的是**未夹紧的形参 <c>nObj</c>**，而 1880 起全部下标用的是
    /// <c>g_Config.MedicaMode</c> —— 原文如此，两者在正常流程下相等，但若调用方传入
    /// 与 <c>MedicaMode</c> 不同的值时会出现"标签按 nObj、数值按 MedicaMode"的错配）。
    ///
    /// 随后 20 余组控件回写全部以 <c>g_Config.MedicaMode</c> 为下标（1880-1963）。
    /// </summary>
    public void RefUseItemConfig(int nObj)
    {
        if (!FInitializeed) return;                                        // 1862
        if (nObj >= 0 && nObj <= 4)                                        // 1863
        {
            if (nObj == 0)                                                 // 1865
            {
                Plug.SetMemoConfig4Label(3, "时使用");                      // 1867
                Plug.PlugComboBoxCheckHPValueVisible = true;                // 1868
                Plug.SetMemoConfig4Label(4, "时使用");                      // 1869
                Plug.PlugComboBoxCheckMPValueVisible = true;                // 1870
            }
            else
            {
                Plug.SetMemoConfig4Label(3, "收英雄");                      // 1874
                Plug.PlugComboBoxCheckHPValueVisible = false;               // 1875
                Plug.SetMemoConfig4Label(4, "收英雄");                      // 1876
                Plug.PlugComboBoxCheckMPValueVisible = false;               // 1877
            }

            int m = g_Config.MedicaMode;

            Plug.PlugCheckBoxAutoPercent = g_Config.ChkAutoPercents[m];                       // 1880
            Plug.PlugCheckBoxRenewAutoPercent = g_Config.ChkRenewAutoPercents[m];              // 1881
            Plug.PlugCheckBoxSuperMedicaPercent = g_Config.ChkSuperMedicaPercents[m];          // 1882

            Plug.PlugCheckBoxCheckHPIsAuto = g_Config.CheckHpIsAutos[m];                      // 1884
            Plug.PlugEditCheckHPPercent = g_Config.CheckHpPercents[m];                        // 1885
            Plug.PlugComboBoxCheckHPValue = g_Config.CheckHpValues[m];                        // 1886

            Plug.PlugCheckBoxCheckMPIsAuto = g_Config.CheckMpIsAutos[m];                      // 1888
            Plug.PlugEditCheckMPPercent = g_Config.CheckMpPercents[m];                        // 1889
            Plug.PlugComboBoxCheckMPValue = g_Config.CheckMpValues[m];                        // 1890

            Plug.PlugCheckBoxRenewHPIsAuto = g_Config.RenewHPIsAutos[m];                      // 1892
            Plug.PlugCheckBoxRenewMPIsAuto = g_Config.RenewMPIsAutos[m];                      // 1893
            Plug.PlugCheckBoxRenewSpecialHPIsAuto = g_Config.RenewSpecialHPIsAutos[m];        // 1894
            Plug.PlugCheckBoxRenewSpecialMPIsAuto = g_Config.RenewSpecialMPIsAutos[m];        // 1895

            Plug.PlugEditRenewHPPercent = g_Config.RenewHPPercents[m];                         // 1897
            Plug.PlugEditRenewMPPercent = g_Config.RenewMPPercents[m];                         // 1898
            Plug.PlugEditRenewHPTime = g_Config.RenewHPTimes[m];                               // 1899
            Plug.PlugEditRenewMPTime = g_Config.RenewMPTimes[m];                               // 1900

            Plug.PlugEditRenewSpecialHPPercent = g_Config.RenewSpecialHPPercents[m];           // 1902
            Plug.PlugEditRenewSpecialMPPercent = g_Config.RenewSpecialMPPercents[m];            // 1903
            Plug.PlugEditRenewSpecialHPTime = g_Config.RenewSpecialHPTimes[m];                 // 1904
            Plug.PlugEditRenewSpecialMPTime = g_Config.RenewSpecialMPTimes[m];                 // 1905

            Plug.PlugCheckBoxCheckDuraIsAuto = g_Config.CheckDuraIsAutos[m];                  // 1907
            Plug.PlugEditCheckDura = g_Config.CheckDuraMin[m];                                // 1908
            Plug.PlugEditCheckDuraValue = g_Config.CheckDuraValue[m];                         // 1909
            Plug.PlugEditCheckDuraTime = g_Config.CheckDuraTime[m];                           // 1910

            Plug.PlugCheckBoxUseSuperMedica = g_Config.UseSuperMedicas[m];                    // 1912
            for (int I = 0; I <= 8; I++)                                                      // 1913-1921
                Plug.SetPlugCheckBoxUseSuperMedicaItemName(I, g_Config.SuperMedicaUses[m, I]);

            for (int I = 0; I <= 8; I++)                                                      // 1923-1931
                Plug.SetPlugEditSuperMedicaHP(I, g_Config.SuperMedicaHPs[m, I]);

            for (int I = 0; I <= 8; I++)                                                      // 1933-1941
                Plug.SetPlugEditSuperMedicaHPTime(I, g_Config.SuperMedicaHPTimes[m, I]);

            for (int I = 0; I <= 8; I++)                                                      // 1943-1951
                Plug.SetPlugEditSuperMedicaMP(I, g_Config.SuperMedicaMPs[m, I]);

            for (int I = 0; I <= 8; I++)                                                      // 1953-1961
                Plug.SetPlugEditSuperMedicaMPTime(I, g_Config.SuperMedicaMPTimes[m, I]);

            Plug.PlugEditHeroDodgeHPPercent = g_Config.nHeroDodgeHPPercent;                    // 1963
        }
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:1983-2137  RefConfig
    // ================================================================================

    /// <summary>
    /// 原文 1983-2137。
    ///
    /// 逐行照抄（含三处冗余赋值与一处 <c>ConfigCheckeds</c> 无前缀）—— 见文件头缺陷登记。
    /// 顺序即原文顺序，可用测试逐位比对（<c>FConfigCheckeds</c> → 控件属性）。
    /// </summary>
    public void RefConfig()
    {
        if (!FInitializeed) return;                                        // 1985

        Plug.PlugCheckBoxShowHPLabel = FConfigCheckeds[(int)TConfigChecked.ckShowHPLabel];            // 1986
        Plug.PlugCheckBoxNumberLable = FConfigCheckeds[(int)TConfigChecked.ckShowNumberLable];        // 1987
        Plug.PlugCheckBoxJobAndLevel = FConfigCheckeds[(int)TConfigChecked.ckShowJobAndLevel];        // 1988
        Plug.PlugCheckBoxShowGreenHint = FConfigCheckeds[(int)TConfigChecked.ckShowGreenHint];        // 1989
        Plug.PlugCheckBoxShowItemName = FConfigCheckeds[(int)MirReturnCheckedMap.ckShowItemName];     // 1990 ★ 映射同 ckShowMonName
        Plug.PlugCheckBoxShowFilterItem = FConfigCheckeds[(int)MirReturnCheckedMap.ckShowFilterItem]; // 1991 ★ 映射同 ckShowMonName（覆盖 1990）
        Plug.PlugCheckBoxItemHint = FConfigCheckeds[(int)MirReturnCheckedMap.ckItemHint];             // 1992 ★ 映射 → ckShowHPLabel（覆盖 1986）
        Plug.PlugCheckBoxDisableSelfStruck = FConfigCheckeds[(int)TConfigChecked.ckDisableSelfStruck]; // 1993
        Plug.PlugCheckBoxSpeedSlow = FConfigCheckeds[(int)TConfigChecked.ckSpeedSlow];                // 1994
        Plug.PlugCheckBoxBGMusic = FConfigCheckeds[(int)TConfigChecked.ckBGMusic];                    // 1995
        Plug.PlugCheckBoxAutoPickUpItem = FConfigCheckeds[(int)TConfigChecked.ckAutoPickUpItem];      // 1996

        Plug.PlugCheckBoxShowActorName = FConfigCheckeds[(int)TConfigChecked.ckShowUserName];         // 1998
        Plug.PlugCheckBoxHideDescUserName = FConfigCheckeds[(int)TConfigChecked.ckOnlyShowCharName];  // 1999
        Plug.PlugCheckBoxDuraWarning = FConfigCheckeds[(int)TConfigChecked.ckDuraWarning];            // 2000
        Plug.PlugCheckBoxNoShift = FConfigCheckeds[(int)TConfigChecked.ckNotNeedShift];               // 2001
        Plug.PlugCheckBoxShiftSwitch = FConfigCheckeds[(int)TConfigChecked.ckShiftSwitch];            // 2002
        Plug.PlugCheckBoxExpFilter = FConfigCheckeds[(int)TConfigChecked.ckFilterExp];                // 2003
        Plug.PlugCheckBoxShowMimiMapDesc = FConfigCheckeds[(int)TConfigChecked.ckShowMapDesc];        // 2004
        Plug.PlugCheckBoxShowHighlightHPLabel = FConfigCheckeds[(int)TConfigChecked.ckShowHighlightHPLabel]; // 2005
        Plug.PlugCheckBoxShowHealthNumber = FConfigCheckeds[(int)TConfigChecked.ckShowMoveLable];     // 2006
        Plug.PlugCheckBoxNotParaly = FConfigCheckeds[(int)TConfigChecked.ckNotParaly];                // 2007

        Plug.PlugCheckBoxHideGhost = FConfigCheckeds[(int)TConfigChecked.ckHideGhost];                // 2009
        Plug.PlugCheckBoxHideHumEffect = FConfigCheckeds[(int)TConfigChecked.ckHideHumEffect];        // 2010
        Plug.PlugCheckBoxHideWeaponEffect = FConfigCheckeds[(int)TConfigChecked.ckHideWeaponEffect];  // 2011
        Plug.PlugCheckBoxShowMonName = FConfigCheckeds[(int)TConfigChecked.ckShowMonName];            // 2012 ★ 被 1990/1991 覆盖
        Plug.PlugCheckBoxShowNpcName = FConfigCheckeds[(int)TConfigChecked.ckShowNpcName];            // 2013 显示NPC名 piaoyun 2013-07-31
        Plug.PlugCheckBoxShowNpcHPLabel = FConfigCheckeds[(int)TConfigChecked.ckShowNpcHPLabel];      // 2014 显示NPC血条 piaoyun 2013-07-31
        Plug.PlugCheckBoxShowNGLabel = FConfigCheckeds[(int)TConfigChecked.ckShowNGLabel];            // 2015 显示内功黄条 piaoyun 2013-09-09
        Plug.PlugCheckBoxNearHint = FConfigCheckeds[(int)TConfigChecked.ckNearHint];                  // 2016 接近提示 piaoyun 2013-09-09
        Plug.PlugCheckBoxAutoLock = FConfigCheckeds[(int)TConfigChecked.ckAutoLock];                  // 2017 自动锁定 piaoyun 2013-09-09
        Plug.PlugCheckBoxColorShow = FConfigCheckeds[(int)TConfigChecked.ckColorShow];                // 2018 变色显示 piaoyun 2013-09-09
        Plug.PlugCheckBoxSpecialQuickFlashing = FConfigCheckeds[(int)TConfigChecked.ckSpecialQuickFlashing]; // 2019 特殊物品快闪 piaoyun 2013-09-10

        Plug.PlugCheckBoxBlacklistHit = FConfigCheckeds[(int)TConfigChecked.ckBlacklistHit];          // 2021 黑名单近身提示 piaoyun 2013-09-11
        Plug.PlugCheckBoxFriendHit = FConfigCheckeds[(int)TConfigChecked.ckFriendHit];                // 2022 好友近身提示 piaoyun 2013-09-11
        // 2023 ★ 原文用的是 ConfigCheckeds[ckSceneShake]（无 F 前缀）—— 原文如此；托管侧二者同一数组
        Plug.PlugCheckBoxSceneShake = ConfigCheckeds[(int)TConfigChecked.ckSceneShake];               // 2023 屏幕震动 piaoyun 2013-09-14
        Plug.PlugCheckBoxAutoDownHorse = FConfigCheckeds[(int)TConfigChecked.ckAutoDownHorse];        // 2024 魔法攻击自动下马 chongchong 2013-10-19

        Plug.PlugCheckBoxAutoOrderItem = FConfigCheckeds[(int)TConfigChecked.ckAutoOrderItem];        // 2026
        Plug.PlugCheckBoxMagicLock = FConfigCheckeds[(int)TConfigChecked.ckMagicLock];                // 2027

        Plug.PlugCheckBoxBGMusic = FConfigCheckeds[(int)TConfigChecked.ckBGMusic];                    // 2029 ★ 重复（1995 已赋）
        Plug.PlugCheckBoxRepeatBGMusic = FConfigCheckeds[(int)TConfigChecked.ckRepeatBGMusic];        // 2030
        Plug.PlugCheckDisableChartMemoSize = FConfigCheckeds[(int)TConfigChecked.ckDisableChartMemoSize]; // 2031
        Plug.PlugCheckBoxItemCmp = FConfigCheckeds[(int)TConfigChecked.ckItemCompare];                // 2032

        Plug.PlugCheckBoxVolume = FConfigCheckeds[(int)TConfigChecked.ckVolume];                      // 2034
        MirsConfigGlobalSeam.g_boSound = Plug.PlugCheckBoxVolume;                                     // 2035

        Plug.TrackBarVolumeMax = 100;                                                                 // 2037
        Plug.TrackBarVolumeMin = 0;                                                                   // 2038
        // 2039 ★ Round(g_SoundVolume)：Delphi Round 是"银行家舍入"之外的**四舍六入五成双**吗？
        // 原文这里是 Round（半数进位到偶数）；托管侧用 Math.Round(AwayFromZero) 会与原文
        // 在 x.5 处不同 —— 故此处显式复刻 Delphi Round 的 ToEven 语义。
        Plug.TrackBarVolumePosition = (int)Math.Round((double)MirReturnGlobalSeam.g_SoundVolume, MidpointRounding.ToEven); // 2039

        MirsConfigGlobalSeam.g_boBGSound = FConfigCheckeds[(int)TConfigChecked.ckBGMusic];            // 2041
        MirsConfigGlobalSeam.g_boRepeatBGSound = FConfigCheckeds[(int)TConfigChecked.ckRepeatBGMusic]; // 2042

        Plug.PlugCheckBoxNotParaly = FConfigCheckeds[(int)TConfigChecked.ckNotParaly];                // 2044 ★ 重复（2007 已赋）

        Plug.PlugCheckBoxSmartLongHit = FConfigCheckeds[(int)TConfigChecked.ckSmartLongHit];          // 2047
        Plug.PlugCheckBoxSmartPosLongHit = FConfigCheckeds[(int)TConfigChecked.ckSmartPosLongHit];    // 2048
        Plug.PlugCheckBoxSmartWalkLongHit = FConfigCheckeds[(int)TConfigChecked.ckSmartWalkLongHit];  // 2049
        Plug.PlugCheckBoxSmartWideHit = FConfigCheckeds[(int)TConfigChecked.ckSmartWideHit];          // 2050
        Plug.PlugCheckBoxSmartFireHit = FConfigCheckeds[(int)TConfigChecked.ckSmartFireHit];          // 2051
        Plug.PlugCheckBoxSmartSwordHit = FConfigCheckeds[(int)TConfigChecked.ckSmartSwordHit];        // 2052
        Plug.PlugCheckBoxSmartKTZHit = FConfigCheckeds[(int)TConfigChecked.ckSmart66Hit];             // 2053 ★ 控件 KTZ ← 配置 ckSmart66Hit（开天斩）
        Plug.PlugCheckBoxSmartCRSHit = FConfigCheckeds[(int)TConfigChecked.ckSmartCrsHit];            // 2054
        Plug.PlugCheckBoxSmartTWNHit = FConfigCheckeds[(int)TConfigChecked.ckSmartTwnHit];            // 2055
        Plug.PlugCheckBoxAutoHideMode = FConfigCheckeds[(int)TConfigChecked.ckAutoHideMode];          // 2056
        Plug.PlugCheckBoxAutoTakeOnItem = FConfigCheckeds[(int)MirReturnCheckedMap.ckAutoTakeOnItem]; // 2057 → ckAutoCHangePoison
        Plug.PlugCheckBoxAutoCHangePoison = FConfigCheckeds[(int)MirReturnCheckedMap.ckAutoChangePoison]; // 2058 → ckAutoCHangePoison（同一位，与 2057 同值）
        Plug.PlugCheckBoxHumAutoShield = FConfigCheckeds[(int)TConfigChecked.ckHumAutoShield];        // 2059

        Plug.PlugCheckBoxHumStruckShield = FConfigCheckeds[(int)TConfigChecked.ckHumStruckShield];    // 2061
        Plug.PlugCheckBoxHeroAutoShield = FConfigCheckeds[(int)TConfigChecked.ckHeroAutoShield];      // 2062
        Plug.PlugCheckBoxAssistantHeroAutoShield = FConfigCheckeds[(int)TConfigChecked.ckAssistantHeroAutoShield]; // 2063
        Plug.PlugCheckBoxHumManuallySnowWind = FConfigCheckeds[(int)TConfigChecked.ckHumManuallySnowWind]; // 2064
        Plug.PlugCheckBoxHumManuallyFireBoom = FConfigCheckeds[(int)TConfigChecked.ckHumManuallyFireBoom]; // 2065
        Plug.PlugCheckBoxHumShootLightenLockTarget = FConfigCheckeds[(int)TConfigChecked.ckHumShootLightenLockTarget]; // 2066
        Plug.PlugCheckBoxHumManuallyMeteorShower = FConfigCheckeds[(int)TConfigChecked.ckHumManuallyMeteorShower]; // 2067

        Plug.PlugCheckBoxHeroContinuousNoHitMon = FConfigCheckeds[(int)TConfigChecked.ckHeroContinuousNoHitMon]; // 2069

        Plug.PlugCheckBoxNoRedPoison = FConfigCheckeds[(int)TConfigChecked.ckGJ_NoRedPoison];          // 2071
        Plug.PlugCheckBoxNoBluePoison = FConfigCheckeds[(int)TConfigChecked.ckGJ_NoBluePoison];       // 2072

        Plug.PlugCheckBoxHumManuallySnowWind = FConfigCheckeds[(int)TConfigChecked.ckHumManuallySnowWind]; // 2074 ★ 重复（2064）
        Plug.PlugCheckBoxHumManuallyFireBoom = FConfigCheckeds[(int)TConfigChecked.ckHumManuallyFireBoom]; // 2075 ★ 重复（2065）
        Plug.PlugCheckBoxHumShootLightenLockTarget = FConfigCheckeds[(int)TConfigChecked.ckHumShootLightenLockTarget]; // 2076 ★ 重复（2066）
        Plug.PlugCheckBoxHumManuallyMeteorShower = FConfigCheckeds[(int)TConfigChecked.ckHumManuallyMeteorShower]; // 2077 ★ 重复（2067）

        Plug.PlugCheckBoxAutoMagic = FConfigCheckeds[(int)TConfigChecked.ckAutoUseMagic];             // 2080
        Plug.PlugCheckBoxUseKeyBoard = FConfigCheckeds[(int)TConfigChecked.ckUseKeyBoard];            // 2081

        Plug.PlugCheckBoxUseSuperMedica = FConfigCheckeds[(int)TConfigChecked.ckUseSuperMedica];      // 2083

        Plug.PlugCheckBoxAutoMagic = FConfigCheckeds[(int)TConfigChecked.ckAutoUseMagic];             // 2085 ★ 重复（2080）

        Plug.PlugCheckBoxHideTitle = FConfigCheckeds[(int)TConfigChecked.ckHideTitle];                // 2087
        Plug.PlugCheckBoxContinueButchItem = FConfigCheckeds[(int)TConfigChecked.ckContinueButchItem]; // 2088

        Plug.PlugEditExpFilter = g_Config.nFilterMinExp;                                              // 2090
        Plug.PlugEditAutoMagicTime = g_Config.nAutoUseMagicTime;                                      // 2091
        Plug.PlugComboBoxColorShow = g_Config.nColorShowEff;                                          // 2092 BOSS变色显示 piaoyun 2013-09-09
        MirReturnGlobalSeam.SetFrmMainColorShowEff(g_Config.nColorShowEff);                            // 2093

        Plug.PlugEditSpecialColor = g_Config.nSpecialColor;                                           // 2095
        MirReturnGlobalSeam.SetLabelSpecialColorUp(MirReturnGlobalSeam.GetRGB(g_Config.nSpecialColor)); // 2096
        MirReturnGlobalSeam.SetFrmMainSpecialColor(g_Config.nSpecialColor);                            // 2097

        Plug.PlugCheckBoxPlayAttack = FConfigCheckeds[(int)TConfigChecked.ckGJ_PlayAttack];           // 2099 挂机 - 受玩家攻击

        Plug.PlugComboBoxPlayAttackValue = g_Config.nGJPlayAttackOption;                              // 2101 挂机 - 受玩家攻击后的操作
        MirReturnGlobalSeam.SetFrmMainGJPlayAttackOption(g_Config.nGJPlayAttackOption);                // 2102

        Plug.PlugComboBoxNoRedPoisonValue = g_Config.nGJNoRedPoisonOption;                            // 2104 挂机 - 红药用完后动作 chongchong 2014-12-06
        MirReturnGlobalSeam.SetFrmMainGJNoRedPoisonOption(g_Config.nGJNoRedPoisonOption);              // 2105

        Plug.PlugComboBoxNoBluePoisonValue = g_Config.nGJNoBluePoisonOption;                          // 2107 挂机 - 蓝药用完后动作 chongchong 2014-12-06
        MirReturnGlobalSeam.SetFrmMainGJNoBluePoisonOption(g_Config.nGJNoBluePoisonOption);            // 2108

        Plug.PlugComboBoxNoDuFuValue = g_Config.nGJNoDuFuOption;                                      // 2110 挂机 - 毒符用完后动作 chongchong 2014-12-06
        MirReturnGlobalSeam.SetFrmMainGJNoDuFuOption(g_Config.nGJNoDuFuOption);                        // 2111

        Plug.PlugComboBoxBagFullValue = g_Config.nGJBagFullOption;                                    // 2113 挂机 - 包裹满后动作 chongchong 2014-12-06
        MirReturnGlobalSeam.SetFrmMainGJBagFullOption(g_Config.nGJBagFullOption);                      // 2114

        Plug.PlugCheckBoxNotRushMon = FConfigCheckeds[(int)TConfigChecked.ckGJ_NotRushMon];           // 2116 挂机 - 不抢怪
        Plug.PlugEditNotRushMonRange = g_Config.nGJNotRushMonRange;                                   // 2117 挂机 - 怪物周围几格有玩家
        MirReturnGlobalSeam.SetFrmMainGJNotRushMonRange(g_Config.nGJNotRushMonRange);                  // 2118

        Plug.PlugCheckBoxNoDuFu = FConfigCheckeds[(int)TConfigChecked.ckGJ_NoDuFu];                   // 2120
        Plug.PlugCheckBoxBagFull = FConfigCheckeds[(int)TConfigChecked.ckGJ_BagFull];                 // 2121
        Plug.PlugCheckBoxAutoPickup = FConfigCheckeds[(int)TConfigChecked.ckGJ_AutoPickup];           // 2122

        Plug.PlugCheckBoxGroupAttack = FConfigCheckeds[(int)TConfigChecked.ckGJ_GroupAttack];         // 2124
        Plug.PlugEditNotGroupAttackCount = g_Config.nGJGroupAttackCount;                              // 2125
        MirReturnGlobalSeam.SetFrmMainGJGroupAttackCount(g_Config.nGJGroupAttackCount);                // 2126
        Plug.PlugCheckBoxLimitScreen = FConfigCheckeds[(int)TConfigChecked.ckGJ_LimitScreen];         // 2127
        Plug.PlugCheckBoxDFAvoid = FConfigCheckeds[(int)MirReturnCheckedMap.ckGJ_DFAvoid];            // 2128 → ckNotParaly（与 2007/2044 覆盖）

        RefKeyboardConfig();                                                                          // 2130
        Plug.PlugMemoConfig4Button1 = g_Config.MedicaMode == 0;                                       // 2131
        Plug.PlugMemoConfig4Button2 = g_Config.MedicaMode == 1;                                       // 2132
        Plug.PlugMemoConfig4Button3 = g_Config.MedicaMode == 2;                                       // 2133
        Plug.PlugMemoConfig4Button4 = g_Config.MedicaMode == 3;                                       // 2134
        Plug.PlugMemoConfig4Button5 = g_Config.MedicaMode == 4;                                       // 2135
        RefUseItemConfig(g_Config.MedicaMode);                                                        // 2136
    }
}

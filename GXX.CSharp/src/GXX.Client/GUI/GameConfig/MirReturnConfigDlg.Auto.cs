// 源单元：Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas（GBK，5,954 行，CRLF）
// 本分片覆盖（原文行号）**待补**：
//   4454-4477  AutoUseMagic
//   3441-3457  AutoUseItem
//   3458-3758  AutoEatHPItem / EatHumHPItem / EatHeroHPItem /
//              AutoEatMPItem / EatHumMPItem / EatHeroMPItem /
//              AutoEatSpecialHPItem / EatHumSpecialItem / EatHeroSpecialItem /
//              AutoEatSpecialMPItem / EatHumSpecialItem / EatHeroSpecialItem（同名嵌套函数两份）
//   3759-4190  AutoProtect / DuraWarning
//   4209-4452  DamageHPUseItem / DamageMPUseItem（含嵌套 NumberSort_1 的调用点）

using System;
using GXX.Client.GUI.GameConfig.Seams;
using GXX.Core.Protocol;

namespace GXX.Client.GUI.GameConfig;

public partial class TMirReturnConfigDlg
{
    /// <summary>
    /// 原文 4454-4477：
    /// <c>if FConfigCheckeds[ckAutoUseMagic]</c> → <c>g_MySelf &lt;&gt; nil</c> 且**非死亡**且**非摆摊** →
    /// <c>ItemIndex</c> 在界内 → <c>MyGetTickCount - g_Config.dwAutoUseMagicTick &gt; g_Config.nAutoUseMagicTime * 1000</c>
    /// → 刷新 tick → 取 <c>Items.Objects[ItemIndex]</c> → <c>frmMain.ChangePoisonCharm(Magic)</c> →
    /// <c>frmMain.UseMagic(g_nMouseX, g_nMouseY, Magic)</c>。
    ///
    /// ★ 注意 <c>nAutoUseMagicTime * 1000</c> 是 **Integer 乘法**：原文 <c>nAutoUseMagicTime</c> 无上界夹紧，
    /// 当它 &gt; 2147483 时会溢出为负 → 条件恒真（原文缺陷，登记不改；托管侧用 <c>unchecked</c> 复刻回绕）。
    /// </summary>
    public void AutoUseMagic()
    {
        if (FConfigCheckeds[(int)TConfigChecked.ckAutoUseMagic])                 // 4458
        {
            if (MirReturnConfigGlobalSeam.g_MySelfExists() &&                    // 4460
                !MirReturnConfigGlobalSeam.MySelfIsDeath() &&                    // 4461
                !MirReturnConfigGlobalSeam.MySelfIsShopStall())                  // 4462
            {
                if ((Plug.PlugComboBoxAutoMagicItemIndex >= 0) &&                // 4464
                    (Plug.PlugComboBoxAutoMagicItemIndex < Plug.PlugComboBoxAutoMagicItemsCount))
                {
                    if (unchecked(ConfigSeams.MyGetTickCount() - g_Config.dwAutoUseMagicTick) >
                        unchecked(g_Config.nAutoUseMagicTime * 1000))            // 4466
                    {
                        g_Config.dwAutoUseMagicTick = ConfigSeams.MyGetTickCount();   // 4468

                        object ClientMagic = Plug.PlugComboBoxAutoMagicItemsObject(
                            Plug.PlugComboBoxAutoMagicItemIndex);                   // 4470
                        MirReturnGlobalSeam.ChangePoisonCharm(ClientMagic);          // 4471
                        MirReturnConfigGlobalSeam.UseMagic(                                // 4472
                            MirReturnConfigGlobalSeam.g_nMouseX, MirReturnConfigGlobalSeam.g_nMouseY, ClientMagic);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 原文 3441-3457：
    /// <c>if FEnabled then if FProtectEnabled and (MyGetTickCount - FProtectEnabledTick &gt; 2000) then</c>
    /// → 依次 <c>AutoEatHPItem / AutoEatMPItem / AutoEatSpecialHPItem / AutoEatSpecialMPItem</c>（均为 <c>Self</c>）。
    ///
    /// ★ 原文缺陷（登记不改）：<c>FProtectEnabledTick</c> 在这 4 个子过程里被刷新，
    /// 但**本方法自己从不刷新** —— 因此一旦"保护"被关掉再打开，2000ms 窗口由 SetProtectEnabled 重置。
    /// 另有：<c>MyGetTickCount - FProtectEnabledTick</c> 是 **Cardinal 运算**（回绕安全），照抄。
    /// </summary>
    public void AutoUseItem(object Sender)
    {
        if (FEnabled)                                                           // 3443
        {
            if (FProtectEnabled &&
                unchecked(ConfigSeams.MyGetTickCount() - FProtectEnabledTick) > 2000)   // 3445
            {
                AutoEatHPItem(Sender);                                          // 3447
                AutoEatMPItem(Sender);                                          // 3448
                AutoEatSpecialHPItem(Sender);                                   // 3449
                AutoEatSpecialMPItem(Sender);                                   // 3450
            }
        }
    }

    // ================================================================================
    // 共用：原文 8 个嵌套 function 的"阈值计算"两行（3469-3473 / 3496-3500 / … 逐字同形）
    // ================================================================================

    /// <summary>
    /// 原文 <c>Value:LongWord;</c> 的赋值两行：
    /// <c>if not ChkRenewAutoPercents[k] then Value := Min(Percents[k], Abil.MaxHP)
    /// else Value := Round(Abil.MaxHP / 100 * Min(Percents[k], 99));</c>
    ///
    /// **类型语义照抄（易错点）**：Delphi 的 <c>Min(a,b)</c> 因 <c>a</c>(Integer) 与
    /// <c>b</c>(LongWord) 混合 → 提升到 **Int64** 比较；结果赋回 <c>LongWord</c> 时按位截断。
    /// 因此 <c>Percents[k]</c> 为负时 <c>Min</c> 取负值 → 截断成大正数（原文如此）。
    /// 托管侧以 <c>long</c> 比较 + <c>unchecked((uint)…)</c> 复刻。
    /// </summary>
    private static uint EatThreshold(bool chkRenewAutoPercent, int percent, uint maxValue)
    {
        if (!chkRenewAutoPercent)
            return unchecked((uint)MirReturnGlobalSeam.Min((long)percent, (long)maxValue));
        long min = MirReturnGlobalSeam.Min((long)percent, 99L);
        // Round(MaxHP / 100 * min)：Delphi 的 "/" 是真除法 → 用 Math.Round(AwayFromZero) 对齐 Delphi Round（银行家舍入相反）
        double v = (double)maxValue / 100.0 * (double)min;
        return unchecked((uint)(long)Math.Round(v, MidpointRounding.AwayFromZero));
    }

    /// <summary>
    /// 原文 8 处逐字同形的触发判定：
    /// <c>(MyGetTickCount - Ticks[k] &gt; Times[k]) and (flag or (Abil.X &lt; Value))</c>。
    /// 左侧是 <c>LongWord - LongWord &gt; Integer</c>（Cardinal 无符号比较）；右侧 <c>Abil.X &lt; Value</c>
    /// 是 <c>LongWord &lt; LongWord</c>（无符号）—— 两侧都按无符号，照抄。
    /// </summary>
    private static bool EatShouldFire(uint tickNow, uint ticks, int times, bool flag, uint cur, uint value)
        => (unchecked(tickNow - ticks) > unchecked((uint)times)) && (flag || (cur < value));

    // ================================================================================
    // MirReturnConfigDlg.pas:3458-3534  AutoEatHPItem
    //
    // ★★ 原文缺陷（**重要**，登记不改，测试固定）：
    //   嵌套 <c>EatHumHPItem</c> 读写的是 <c>[0]</c>、嵌套 <c>EatHeroHPItem</c> 读写的是 <c>[1]</c> ——
    //   **完全无视 <c>g_Config.MedicaMode</c>**（用户在"用药模式"里选了 2/3/4 副将时，
    //   本方法仍只对 [0] 主体与 [1] 英雄生效；那个 <c>MedicaMode</c> 只在本单元的
    //   控件写回处理器（2139-2260）与 ini 存取里被用到）。
    //   四个 AutoEat* 全是同一模式。
    // ================================================================================

    /// <summary>原文 3458-3534。</summary>
    public void AutoEatHPItem(object Sender) => AutoEatHPItemCore();

    private void AutoEatHPItemCore()
    {
        // 3519
        if (!MirReturnConfigGlobalSeam.g_MySelfExists()) return;

        bool Death = MirReturnConfigGlobalSeam.MySelfIsDeath();                       // 3521
        TAbility SelfAbil = MirReturnConfigGlobalSeam.MySelfAbil();                   // 3522
        if (g_Config.RenewHPIsAutos[0] && !Death && (SelfAbil.HP > 0))                // 3523
            EatHumHPItem(false);                                                      // 3524

        if (MirReturnConfigGlobalSeam.g_MyHeroExists())                               // 3526
        {
            Death = MirReturnConfigGlobalSeam.MyHeroIsDeath();                         // 3528
            TAbility HeroAbil = MirReturnConfigGlobalSeam.MyHeroAbil();                // 3529
            if (g_Config.RenewHPIsAutos[1] && !Death &&
                (HeroAbil.HP > 0) && (HeroAbil.MaxHP > 0))                             // 3530
                EatHeroHPItem(false);                                                  // 3531
        }
    }

    /// <summary>原文 3465-3490（嵌套 <c>EatHumHPItem</c>）。</summary>
    private bool EatHumHPItem(bool flag)
    {
        bool Result = false;                                                          // 3469
        TAbility SelfAbil = MirReturnConfigGlobalSeam.MySelfAbil();
        uint Value = EatThreshold(g_Config.ChkRenewAutoPercents[0],
                                  g_Config.RenewHPPercents[0], SelfAbil.MaxHP);        // 3470-3473

        if (EatShouldFire(ConfigSeams.MyGetTickCount(), g_Config.RenewHPTicks[0],
                          g_Config.RenewHPTimes[0], flag, SelfAbil.HP, Value))         // 3475
        {
            int nIndex = ConfigShare.FindHumHPItemIndex();                             // 3477
            if (nIndex >= 0)                                                          // 3478
            {
                g_Config.RenewHPTicks[0] = ConfigSeams.MyGetTickCount();              // 3480
                MirReturnConfigGlobalSeam.AutoEatItem(nIndex);                              // 3481
                Result = true;                                                        // 3482
            }
            else
            {
                ChatBoardSeam.AddChatBoardString("你的金创药已使用完", ChatClWhite, ChatClBlue); // 3486
                g_Config.RenewHPTicks[0] = ConfigSeams.MyGetTickCount();              // 3487
            }
        }
        return Result;
    }

    /// <summary>原文 3492-3516（嵌套 <c>EatHeroHPItem</c>）。</summary>
    private bool EatHeroHPItem(bool flag)
    {
        bool Result = false;                                                          // 3496
        TAbility HeroAbil = MirReturnConfigGlobalSeam.MyHeroAbil();
        uint Value = EatThreshold(g_Config.ChkRenewAutoPercents[1],
                                  g_Config.RenewHPPercents[1], HeroAbil.MaxHP);        // 3497-3500

        if (EatShouldFire(ConfigSeams.MyGetTickCount(), g_Config.RenewHPTicks[1],
                          g_Config.RenewHPTimes[1], flag, HeroAbil.HP, Value))         // 3501
        {
            int nIndex = ConfigShare.FindHeroHPItemIndex();                            // 3503
            if (nIndex >= 0)                                                          // 3504
            {
                g_Config.RenewHPTicks[1] = ConfigSeams.MyGetTickCount();              // 3506
                MirReturnConfigGlobalSeam.HeroEatItem(nIndex);                              // 3507
                Result = true;                                                        // 3508
            }
            else
            {
                ChatBoardSeam.AddChatBoardString("你英雄的金创药已使用完", ChatClWhite, ChatClBlue); // 3512
                g_Config.RenewHPTicks[1] = ConfigSeams.MyGetTickCount();              // 3513
            }
        }
        return Result;
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:3536-3616  AutoEatMPItem
    // ================================================================================

    /// <summary>原文 3536-3616：与 <c>AutoEatHPItem</c> 同形，换成 MP 组字段与提示文案。</summary>
    public void AutoEatMPItem(object Sender) => AutoEatMPItemCore();

    private void AutoEatMPItemCore()
    {
        // 3602
        if (!MirReturnConfigGlobalSeam.g_MySelfExists()) return;

        TAbility SelfAbil = MirReturnConfigGlobalSeam.MySelfAbil();                   // 3604
        bool Death = MirReturnConfigGlobalSeam.MySelfIsDeath();                        // 3605
        // 3606：★ 本人判据是 MaxMP > 0（不是 HP）
        if (g_Config.RenewMPIsAutos[0] && !Death && (SelfAbil.MaxMP > 0))
            EatHumMPItem(false);

        if (MirReturnConfigGlobalSeam.g_MyHeroExists())                                // 3608
        {
            TAbility HeroAbil = MirReturnConfigGlobalSeam.MyHeroAbil();                // 3610
            Death = MirReturnConfigGlobalSeam.MyHeroIsDeath();                         // 3611
            if (g_Config.RenewMPIsAutos[1] && !Death && (HeroAbil.MaxMP > 0))          // 3613
                EatHeroMPItem(false);
        }
    }

    /// <summary>原文 3543-3571（嵌套 <c>EatHumMPItem</c>）。</summary>
    private bool EatHumMPItem(bool flag)
    {
        bool Result = false;                                                          // 3547
        TAbility SelfAbil = MirReturnConfigGlobalSeam.MySelfAbil();
        uint Value = EatThreshold(g_Config.ChkRenewAutoPercents[0],
                                  g_Config.RenewMPPercents[0], SelfAbil.MaxMP);        // 3549-3552

        // 3556/3560 两行 DScreen.AddChatBoardString(Format('2 %d/%d…')) 被注释掉 —— 原文如此
        if (EatShouldFire(ConfigSeams.MyGetTickCount(), g_Config.RenewMPTicks[0],
                          g_Config.RenewMPTimes[0], flag, SelfAbil.MP, Value))         // 3554
        {
            int nIndex = ConfigShare.FindHumMPItemIndex();                             // 3557
            if (nIndex >= 0)                                                          // 3558
            {
                g_Config.RenewMPTicks[0] = ConfigSeams.MyGetTickCount();              // 3561
                MirReturnConfigGlobalSeam.AutoEatItem(nIndex);                              // 3562
                Result = true;                                                        // 3563
            }
            else
            {
                ChatBoardSeam.AddChatBoardString("你的魔法药已使用完", ChatClWhite, ChatClBlue); // 3567
                g_Config.RenewMPTicks[0] = ConfigSeams.MyGetTickCount();              // 3568
            }
        }
        return Result;
    }

    /// <summary>原文 3573-3599（嵌套 <c>EatHeroMPItem</c>）。</summary>
    private bool EatHeroMPItem(bool flag)
    {
        bool Result = false;                                                          // 3577
        TAbility HeroAbil = MirReturnConfigGlobalSeam.MyHeroAbil();
        uint Value = EatThreshold(g_Config.ChkRenewAutoPercents[1],
                                  g_Config.RenewMPPercents[1], HeroAbil.MaxMP);        // 3579-3582

        if (EatShouldFire(ConfigSeams.MyGetTickCount(), g_Config.RenewMPTicks[1],
                          g_Config.RenewMPTimes[1], flag, HeroAbil.MP, Value))         // 3584
        {
            int nIndex = ConfigShare.FindHeroMPItemIndex();                            // 3586
            if (nIndex >= 0)                                                          // 3587
            {
                g_Config.RenewMPTicks[1] = ConfigSeams.MyGetTickCount();              // 3589
                MirReturnConfigGlobalSeam.HeroEatItem(nIndex);                              // 3590
                Result = true;                                                        // 3591
            }
            else
            {
                ChatBoardSeam.AddChatBoardString("你英雄的魔法药已使用完", ChatClWhite, ChatClBlue); // 3595
                g_Config.RenewMPTicks[1] = ConfigSeams.MyGetTickCount();              // 3596
            }
        }
        return Result;
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:3618-3685  AutoEatSpecialHPItem
    //
    // ★ 与 HP/MP 版的两处**不对称**（原文如此，测试固定）：
    //   1) 药_用完_时**不**打聊天栏提示（3638-3645 / 3661-3668 都没有 else 分支，
    //      也**不刷新** <c>RenewSpecialHPTicks</c>）—— 即"没药"时下一次 tick 仍会再查一遍；
    //   2) 本人判据是 <c>SelfAbil.HP &gt; 0</c>（不是 MaxHP）。
    // ================================================================================

    /// <summary>原文 3618-3685。</summary>
    public void AutoEatSpecialHPItem(object Sender) => AutoEatSpecialHPItemCore();

    private void AutoEatSpecialHPItemCore()
    {
        if (!MirReturnConfigGlobalSeam.g_MySelfExists()) return;                       // 3672

        TAbility SelfAbil = MirReturnConfigGlobalSeam.MySelfAbil();                    // 3674
        bool Death = MirReturnConfigGlobalSeam.MySelfIsDeath();                         // 3675
        if (g_Config.RenewSpecialHPIsAutos[0] && !Death && (SelfAbil.HP > 0))           // 3676
            EatHumSpecialHPItem(false);

        if (MirReturnConfigGlobalSeam.g_MyHeroExists())                                 // 3678
        {
            TAbility HeroAbil = MirReturnConfigGlobalSeam.MyHeroAbil();                 // 3680
            Death = MirReturnConfigGlobalSeam.MyHeroIsDeath();                          // 3681
            if (g_Config.RenewSpecialHPIsAutos[1] && !Death &&
                (HeroAbil.HP > 0) && (HeroAbil.MaxHP > 0))                              // 3682
                EatHeroSpecialHPItem(false);
        }
    }

    /// <summary>原文 3625-3646（嵌套 <c>EatHumSpecialItem</c>）。★ 无 else 分支（原文如此）。</summary>
    private bool EatHumSpecialHPItem(bool flag)
    {
        bool Result = false;                                                           // 3629
        TAbility SelfAbil = MirReturnConfigGlobalSeam.MySelfAbil();
        uint Value = EatThreshold(g_Config.ChkRenewAutoPercents[0],
                                  g_Config.RenewSpecialHPPercents[0], SelfAbil.MaxHP); // 3631-3634

        if (EatShouldFire(ConfigSeams.MyGetTickCount(), g_Config.RenewSpecialHPTicks[0],
                          g_Config.RenewSpecialHPTimes[0], flag, SelfAbil.HP, Value))  // 3636
        {
            int nIndex = ConfigShare.FindHumSpecialItemIndex();                        // 3638
            if (nIndex >= 0)                                                           // 3639
            {
                g_Config.RenewSpecialHPTicks[0] = ConfigSeams.MyGetTickCount();        // 3641
                MirReturnConfigGlobalSeam.AutoEatItem(nIndex);                               // 3642
                Result = true;                                                         // 3643
            }
        }
        return Result;
    }

    /// <summary>原文 3648-3669（嵌套 <c>EatHeroSpecialItem</c>）。★ 无 else 分支。</summary>
    private bool EatHeroSpecialHPItem(bool flag)
    {
        bool Result = false;                                                           // 3652
        TAbility HeroAbil = MirReturnConfigGlobalSeam.MyHeroAbil();
        uint Value = EatThreshold(g_Config.ChkRenewAutoPercents[1],
                                  g_Config.RenewSpecialHPPercents[1], HeroAbil.MaxHP); // 3654-3657

        if (EatShouldFire(ConfigSeams.MyGetTickCount(), g_Config.RenewSpecialHPTicks[1],
                          g_Config.RenewSpecialHPTimes[1], flag, HeroAbil.HP, Value))  // 3659
        {
            int nIndex = ConfigShare.FindHeroSpecialItemIndex();                       // 3661
            if (nIndex >= 0)                                                           // 3662
            {
                g_Config.RenewSpecialHPTicks[1] = ConfigSeams.MyGetTickCount();        // 3664
                MirReturnConfigGlobalSeam.HeroEatItem(nIndex);                               // 3665
                Result = true;                                                         // 3666
            }
        }
        return Result;
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:3687-3757  AutoEatSpecialMPItem
    // ================================================================================

    /// <summary>
    /// 原文 3687-3757。与 <c>AutoEatSpecialHPItem</c> 同构（同样**无 else 提示**），
    /// 本人判据是 <c>MaxMP &gt; 0</c>（3747）。★ 3745-3747 与 3752-3754 的
    /// <c>if</c> 条件被**空行拆成两行**（只是一条语句）—— 原文如此。
    /// </summary>
    public void AutoEatSpecialMPItem(object Sender) => AutoEatSpecialMPItemCore();

    private void AutoEatSpecialMPItemCore()
    {
        if (!MirReturnConfigGlobalSeam.g_MySelfExists()) return;                        // 3741

        TAbility SelfAbil = MirReturnConfigGlobalSeam.MySelfAbil();                     // 3743
        bool Death = MirReturnConfigGlobalSeam.MySelfIsDeath();                          // 3744
        if (g_Config.RenewSpecialMPIsAutos[0] && !Death && (SelfAbil.MaxMP > 0))         // 3745-3747
            EatHumSpecialMPItem(false);

        if (MirReturnConfigGlobalSeam.g_MyHeroExists())                                  // 3748
        {
            TAbility HeroAbil = MirReturnConfigGlobalSeam.MyHeroAbil();                  // 3750
            Death = MirReturnConfigGlobalSeam.MyHeroIsDeath();                           // 3751
            if (g_Config.RenewSpecialMPIsAutos[1] && !Death &&
                (HeroAbil.MP > 0) && (HeroAbil.MaxMP > 0))                               // 3752-3754
                EatHeroSpecialMPItem(false);
        }
    }

    /// <summary>原文 3694-3715（嵌套 <c>EatHumSpecialItem</c>，与 HP 侧同名 —— 原文如此）。</summary>
    private bool EatHumSpecialMPItem(bool flag)
    {
        bool Result = false;                                                            // 3698
        TAbility SelfAbil = MirReturnConfigGlobalSeam.MySelfAbil();
        uint Value = EatThreshold(g_Config.ChkRenewAutoPercents[0],
                                  g_Config.RenewSpecialMPPercents[0], SelfAbil.MaxMP);  // 3700-3703

        if (EatShouldFire(ConfigSeams.MyGetTickCount(), g_Config.RenewSpecialMPTicks[0],
                          g_Config.RenewSpecialMPTimes[0], flag, SelfAbil.MP, Value))   // 3705
        {
            int nIndex = ConfigShare.FindHumSpecialItemIndex();                         // 3707
            if (nIndex >= 0)                                                            // 3708
            {
                g_Config.RenewSpecialMPTicks[0] = ConfigSeams.MyGetTickCount();         // 3710
                MirReturnConfigGlobalSeam.AutoEatItem(nIndex);                                // 3711
                Result = true;                                                          // 3712
            }
        }
        return Result;
    }

    /// <summary>原文 3717-3738（嵌套 <c>EatHeroSpecialItem</c>）。</summary>
    private bool EatHeroSpecialMPItem(bool flag)
    {
        bool Result = false;                                                            // 3721
        TAbility HeroAbil = MirReturnConfigGlobalSeam.MyHeroAbil();
        uint Value = EatThreshold(g_Config.ChkRenewAutoPercents[1],
                                  g_Config.RenewSpecialMPPercents[1], HeroAbil.MaxMP);  // 3723-3726

        if (EatShouldFire(ConfigSeams.MyGetTickCount(), g_Config.RenewSpecialMPTicks[1],
                          g_Config.RenewSpecialMPTimes[1], flag, HeroAbil.MP, Value))   // 3728
        {
            int nIndex = ConfigShare.FindHeroSpecialItemIndex();                        // 3730
            if (nIndex >= 0)                                                            // 3731
            {
                g_Config.RenewSpecialMPTicks[1] = ConfigSeams.MyGetTickCount();         // 3733
                MirReturnConfigGlobalSeam.HeroEatItem(nIndex);                                // 3734
                Result = true;                                                          // 3735
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 4209-4337 <c>DamageHPUseItem</c> / 4338-4452 <c>DamageMPUseItem</c>
    /// （"减血喝药"：按 <c>NumberSort_1</c> 排序后的伤害档位选药）。
    ///
    /// 覆盖状态：**未实现**（依赖 <c>g_ItemArr</c>/<c>FindHum*ItemIndex</c>/<c>frmMain.AutoEatItem</c>
    /// 的完整链路与 TDxListView 交互）—— 见交付报告的未覆盖清单。
    /// 此处以可注入委托承载调用点（原文 3387/3397/3417/3421/3430/3434 共 6 处），
    /// 使 <c>Struck</c>/<c>HealthChange</c> 的**分派逻辑**可被完整断言。
    /// </summary>
    public Action<int, int> DamageHPUseItem = (nObj, nDamage) => { };

    /// <summary>原文 4338-4452 <c>DamageMPUseItem</c>。见 <see cref="DamageHPUseItem"/>。</summary>
    public Action<int, int> DamageMPUseItem = (nObj, nDamage) => { };

    /// <summary>原文 Graphics clWhite = $00FFFFFF。</summary>
    private const int ChatClWhite = 0x00FFFFFF;
    /// <summary>原文 Graphics clBlue = $00FF0000。</summary>
    private const int ChatClBlue = 0x00FF0000;
}

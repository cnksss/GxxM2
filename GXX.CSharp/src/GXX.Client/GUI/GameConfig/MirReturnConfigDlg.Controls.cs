// 源单元：Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas（GBK，5,954 行，CRLF）
// 本分片覆盖（原文行号）：
//   2139-2142  DEditCheckHPPercentChange
//   2144-2148  PlugEditHeroDodgeHPPercentChange
//   2150-2153  DEditCheckMPPercentChange
//   2155-2158  ComboBoxCheckHPValueChange
//   2160-2163  ComboBoxCheckMPValueChange
//   2165-2168  DCheckBoxCheckHPIsAutoClick
//   2170-2173  DCheckBoxCheckMPIsAutoClick
//   2175-2178  DCheckBoxCheckDuraIsAuto
//   2180-2184  DEditCheckDuraChange（Max(1, …) 下界夹紧 + **回写控件**）
//   2186-2189  DEditCheckDuraValueChange
//   2191-2195  DEditCheckDuraTimeChange（Max(2, …) + 回写）
//   2198-2260  四组 Renew*Change / *Click
//   2218-2240  四个 Renew*TimeChange（夹紧到 g_ClientConfig.dwPluginMinEatItemTime + 回写）
//   2262-2545  DEditSuperMedica{HP,HPTime,MP,MPTime}Change + DCheckBoxUseSuperMedicaItemNameClick
//              （9 路 Sender 判等 → 索引 → 按 g_Config.MedicaMode 写入矩阵）
//   2546-2556  DEditChange（PlugEditExpFilter / PlugEditAutoMagicTime 两路）
//   5262-5304  见 Lists 分片
//   5753-5757  DEditGroupAttackCountChanged
//   5842-5940  DCheckBox{Auto,RenewAuto,SuperMedica}PercentClick（>99 → 归 50/30 的"纠正"）

using System;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig;

public partial class TMirReturnConfigDlg
{
    // ================================================================================
    // MirReturnConfigDlg.pas:2139-2195  Check 系列（HP/MP 百分比 / 耐久）
    // ================================================================================

    /// <summary>原文 2139-2142：<c>g_Config.CheckHpPercents[g_Config.MedicaMode] := PlugEditCheckHPPercent.Value;</c></summary>
    public void DEditCheckHPPercentChange()
        => g_Config.CheckHpPercents[g_Config.MedicaMode] = Plug.PlugEditCheckHPPercent;      // 2141

    /// <summary>
    /// 原文 2144-2148：写 <c>nHeroDodgeHPPercent</c> 后调 <c>frmMain.SendPlugInConfig(...)</c>
    /// （★ 本单元**唯一**一处 SendPlugInConfig 调用 —— 同为"英雄躲避 HP 百分比"的即时下发）。
    /// </summary>
    public void PlugEditHeroDodgeHPPercentChange()
    {
        g_Config.nHeroDodgeHPPercent = Plug.PlugEditHeroDodgeHPPercent;                       // 2146
        MirReturnGlobalSeam.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);                   // 2147
    }

    /// <summary>原文 2150-2153。</summary>
    public void DEditCheckMPPercentChange()
        => g_Config.CheckMpPercents[g_Config.MedicaMode] = Plug.PlugEditCheckMPPercent;      // 2152

    /// <summary>原文 2155-2158：★ 写的是 <c>ItemIndex</c>（下拉**下标**）而不是文本。</summary>
    public void ComboBoxCheckHPValueChange()
        => g_Config.CheckHpValues[g_Config.MedicaMode] = Plug.PlugComboBoxCheckHPValue;      // 2157

    /// <summary>原文 2160-2163：同 <see cref="ComboBoxCheckHPValueChange"/>，MP 侧。</summary>
    public void ComboBoxCheckMPValueChange()
        => g_Config.CheckMpValues[g_Config.MedicaMode] = Plug.PlugComboBoxCheckMPValue;      // 2162

    /// <summary>原文 2165-2168。</summary>
    public void DCheckBoxCheckHPIsAutoClick()
        => g_Config.CheckHpIsAutos[g_Config.MedicaMode] = Plug.PlugCheckBoxCheckHPIsAuto;    // 2167

    /// <summary>原文 2170-2173。</summary>
    public void DCheckBoxCheckMPIsAutoClick()
        => g_Config.CheckMpIsAutos[g_Config.MedicaMode] = Plug.PlugCheckBoxCheckMPIsAuto;    // 2172

    /// <summary>原文 2175-2178。</summary>
    public void DCheckBoxCheckDuraIsAuto()
        => g_Config.CheckDuraIsAutos[g_Config.MedicaMode] = Plug.PlugCheckBoxCheckDuraIsAuto; // 2177

    /// <summary>原文 2180-2184：<c>Max(1, Value)</c> 后**回写控件**（把夹紧结果回显给用户）。</summary>
    public void DEditCheckDuraChange()
    {
        g_Config.CheckDuraMin[g_Config.MedicaMode] = MirReturnGlobalSeam.MaxI(1, (long)(Plug.PlugEditCheckDura));  // 2182
        Plug.PlugEditCheckDura = g_Config.CheckDuraMin[g_Config.MedicaMode];                  // 2183
    }

    /// <summary>原文 2186-2189：★ 耐久物品名写进 <c>string[20]</c>（无长度校验，超长由控件侧截断）。</summary>
    public void DEditCheckDuraValueChange()
        => g_Config.CheckDuraValue[g_Config.MedicaMode] = Plug.PlugEditCheckDuraValue;   // 2188

    /// <summary>原文 2191-2195：<c>Max(2, Value)</c> + 回写。</summary>
    public void DEditCheckDuraTimeChange()
    {
        g_Config.CheckDuraTime[g_Config.MedicaMode] = MirReturnGlobalSeam.MaxI(2, (long)(Plug.PlugEditCheckDuraTime));  // 2193
        Plug.PlugEditCheckDuraTime = g_Config.CheckDuraTime[g_Config.MedicaMode];             // 2194
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:2198-2260  Renew 系列
    // ================================================================================

    /// <summary>原文 2198-2201。</summary>
    public void DEditRenewHPPercentChange()
        => g_Config.RenewHPPercents[g_Config.MedicaMode] = Plug.PlugEditRenewHPPercent;      // 2200

    /// <summary>原文 2203-2206。</summary>
    public void DEditRenewMPPercentChange()
        => g_Config.RenewMPPercents[g_Config.MedicaMode] = Plug.PlugEditRenewMPPercent;      // 2205

    /// <summary>原文 2208-2211。</summary>
    public void DEditRenewSpecialHPPercentChange()
        => g_Config.RenewSpecialHPPercents[g_Config.MedicaMode] = Plug.PlugEditRenewSpecialHPPercent; // 2210

    /// <summary>原文 2213-2216。</summary>
    public void DEditRenewSpecialMPPercentChange()
        => g_Config.RenewSpecialMPPercents[g_Config.MedicaMode] = Plug.PlugEditRenewSpecialMPPercent; // 2215

    /// <summary>原文 2218-2222：夹紧到 <c>g_ClientConfig.dwPluginMinEatItemTime</c>（用药间隔下限）+ 回写。</summary>
    public void DEditRenewHPTimeChange()
    {
        g_Config.RenewHPTimes[g_Config.MedicaMode] =
            MirReturnGlobalSeam.MaxI(MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime, (long)(Plug.PlugEditRenewHPTime));  // 2220
        Plug.PlugEditRenewHPTime = g_Config.RenewHPTimes[g_Config.MedicaMode];                // 2221
    }

    /// <summary>原文 2224-2228。</summary>
    public void DEditRenewMPTimeChange()
    {
        g_Config.RenewMPTimes[g_Config.MedicaMode] =
            MirReturnGlobalSeam.MaxI(MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime, (long)(Plug.PlugEditRenewMPTime));  // 2226
        Plug.PlugEditRenewMPTime = g_Config.RenewMPTimes[g_Config.MedicaMode];                // 2227
    }

    /// <summary>原文 2230-2234。</summary>
    public void DEditRenewSpecialHPTimeChange()
    {
        g_Config.RenewSpecialHPTimes[g_Config.MedicaMode] =
            MirReturnGlobalSeam.MaxI(MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime, (long)(Plug.PlugEditRenewSpecialHPTime)); // 2232
        Plug.PlugEditRenewSpecialHPTime = g_Config.RenewSpecialHPTimes[g_Config.MedicaMode];  // 2233
    }

    /// <summary>原文 2236-2240。</summary>
    public void DEditRenewSpecialMPTimeChange()
    {
        g_Config.RenewSpecialMPTimes[g_Config.MedicaMode] =
            MirReturnGlobalSeam.MaxI(MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime, (long)(Plug.PlugEditRenewSpecialMPTime)); // 2238
        Plug.PlugEditRenewSpecialMPTime = g_Config.RenewSpecialMPTimes[g_Config.MedicaMode];  // 2239
    }

    /// <summary>原文 2242-2245。</summary>
    public void DCheckBoxRenewHPIsAutoClick()
        => g_Config.RenewHPIsAutos[g_Config.MedicaMode] = Plug.PlugCheckBoxRenewHPIsAuto;    // 2244

    /// <summary>原文 2247-2250。</summary>
    public void DCheckBoxRenewMPIsAutoClick()
        => g_Config.RenewMPIsAutos[g_Config.MedicaMode] = Plug.PlugCheckBoxRenewMPIsAuto;    // 2249

    /// <summary>原文 2252-2255。</summary>
    public void DCheckBoxRenewSpecialHPIsAutoClick()
        => g_Config.RenewSpecialHPIsAutos[g_Config.MedicaMode] = Plug.PlugCheckBoxRenewSpecialHPIsAuto; // 2254

    /// <summary>原文 2257-2260。</summary>
    public void DCheckBoxRenewSpecialMPIsAutoClick()
        => g_Config.RenewSpecialMPIsAutos[g_Config.MedicaMode] = Plug.PlugCheckBoxRenewSpecialMPIsAuto; // 2259

    // ================================================================================
    // MirReturnConfigDlg.pas:2262-2545  超药矩阵 4 个 Change + 1 个 Click
    //
    // 原文的 9 路 Sender 判等（<c>if PlugEditSuperMedicaHP0 = Sender then … else if …</c>）
    // 托管侧以**索引形参**表达：原文的 9 路 if-else 链等价于"控件下标 → Index"的恒等映射，
    // 因此 <c>Index</c> 直接由调用方（控件宿主）给出；原文 <c>Index := -1</c> 的初值语义
    // 由"越界即不写"复刻（原文 2312 的 <c>if Index in [0..8]</c>）。
    // ★ 每个方法都保留原文"Index in [0..8] 之外什么都不做"的行为。
    // ================================================================================

    /// <summary>原文 2262-2316：<c>SuperMedicaHPs[MedicaMode][Index] := Value;</c>（**无夹紧、不回写**）。</summary>
    public void DEditSuperMedicaHPChange(int Index)
    {
        int Value = Index >= 0 && Index <= 8 ? Plug.PlugEditSuperMedicaHP(Index) : 0;   // 2266-2311
        if (Index >= 0 && Index <= 8)                                                   // 2312
        {
            g_Config.SuperMedicaHPs[g_Config.MedicaMode, Index] = Value;                // 2314
        }
    }

    /// <summary>
    /// 原文 2318-2373：夹紧到 <c>g_ClientConfig.dwPluginMinEatItemTime</c> 后
    /// **把夹紧值回写到同一个控件**（<c>TDxEdit(Sender).Value := …</c>，2371）。
    /// </summary>
    public void DEditSuperMedicaHPTimeChange(int Index)
    {
        int Value = Index >= 0 && Index <= 8 ? Plug.PlugEditSuperMedicaHPTime(Index) : 0;   // 2322-2367
        if (Index >= 0 && Index <= 8)                                                       // 2368
        {
            g_Config.SuperMedicaHPTimes[g_Config.MedicaMode, Index] =
                MirReturnGlobalSeam.MaxI(MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime, (long)(Value));        // 2370
            Plug.SetPlugEditSuperMedicaHPTime(Index, g_Config.SuperMedicaHPTimes[g_Config.MedicaMode, Index]); // 2371
        }
    }

    /// <summary>原文 2375-2430：与 HP 侧同形（无夹紧、不回写）。</summary>
    public void DEditSuperMedicaMPChange(int Index)
    {
        int Value = Index >= 0 && Index <= 8 ? Plug.PlugEditSuperMedicaMP(Index) : 0;    // 2379-2424
        if (Index >= 0 && Index <= 8)                                                    // 2426
        {
            g_Config.SuperMedicaMPs[g_Config.MedicaMode, Index] = Value;                 // 2428
        }
    }

    /// <summary>原文 2432-2487：夹紧 + 回写。</summary>
    public void DEditSuperMedicaMPTimeChange(int Index)
    {
        int Value = Index >= 0 && Index <= 8 ? Plug.PlugEditSuperMedicaMPTime(Index) : 0;   // 2436-2481
        if (Index >= 0 && Index <= 8)                                                       // 2482
        {
            g_Config.SuperMedicaMPTimes[g_Config.MedicaMode, Index] =
                MirReturnGlobalSeam.MaxI(MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime, (long)(Value));        // 2484
            Plug.SetPlugEditSuperMedicaMPTime(Index, g_Config.SuperMedicaMPTimes[g_Config.MedicaMode, Index]); // 2485
        }
    }

    /// <summary>
    /// 原文 2489-2544：<c>SuperMedicaUses[MedicaMode][Index] := PlugCheckBoxUseSuperMedicaItemNameN.Checked;</c>
    /// （9 路 Sender 判等 + <c>Index in [0..8]</c> 守卫）。
    /// </summary>
    public void DCheckBoxUseSuperMedicaItemNameClick(int Index)
    {
        bool Value = Index >= 0 && Index <= 8 && Plug.PlugCheckBoxUseSuperMedicaItemName(Index);  // 2494-2539
        if (Index >= 0 && Index <= 8)                                                             // 2540
        {
            g_Config.SuperMedicaUses[g_Config.MedicaMode, Index] = Value;                          // 2542
        }
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:2546-2556  DEditChange
    // ================================================================================

    /// <summary>
    /// 原文 2546-2556：两路 Sender 判等（<c>PlugEditExpFilter</c> → <c>nFilterMinExp</c>；
    /// <c>PlugEditAutoMagicTime</c> → <c>nAutoUseMagicTime</c>），
    /// **无 else 分支、无夹紧**（原文如此）。
    /// </summary>
    public void DEditChange(MirReturnEditId Sender)
    {
        if (Sender == MirReturnEditId.PlugEditExpFilter)                   // 2548
        {
            g_Config.nFilterMinExp = Plug.PlugEditExpFilter;          // 2550
        }
        else if (Sender == MirReturnEditId.PlugEditAutoMagicTime)          // 2552
        {
            g_Config.nAutoUseMagicTime = Plug.PlugEditAutoMagicTime;  // 2554
        }
    }

    /// <summary>接缝：<c>PlugEditExpFilter.Value</c>（原文 2550）。</summary>
    public int PlugEditExpFilterValue
    {
        get => Plug.PlugEditExpFilter;
        set => Plug.PlugEditExpFilter = value;
    }

    /// <summary>接缝：<c>PlugEditAutoMagicTime.Value</c>（原文 2554）。</summary>
    public int PlugEditAutoMagicTimeValue
    {
        get => Plug.PlugEditAutoMagicTime;
        set => Plug.PlugEditAutoMagicTime = value;
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5753-5757  DEditGroupAttackCountChanged
    // ================================================================================

    /// <summary>
    /// 原文 5753-5757：<c>g_Config.nGJGroupAttackCount := PlugEditNotGroupAttackCount.Value;
    /// FrmMain.nGJGroupAttackCount := …;</c>（**无夹紧**）。
    /// </summary>
    public void DEditGroupAttackCountChanged()
    {
        g_Config.nGJGroupAttackCount = Plug.PlugEditNotGroupAttackCount;                       // 5755
        MirReturnGlobalSeam.SetFrmMainGJGroupAttackCount(g_Config.nGJGroupAttackCount);        // 5756
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5842-5940  三个 *PercentClick
    // ================================================================================

    /// <summary>
    /// 原文 5842-5861：
    /// 先写 <c>ChkAutoPercents[MedicaMode]</c>；**仅当它为真**时做"纠正"：
    /// <c>CheckHpPercents &gt; 99 → := 50 并回写控件</c>、<c>CheckMpPercents &gt; 99 → := 50 并回写控件</c>。
    /// ★ 阈值写在 <c>&gt; 99</c>（即 100 及以上才纠正；99 保留）。
    /// </summary>
    public void DCheckBoxAutoPercentClick()
    {
        g_Config.ChkAutoPercents[g_Config.MedicaMode] = Plug.PlugCheckBoxAutoPercent;           // 5845

        if (g_Config.ChkAutoPercents[g_Config.MedicaMode])                                      // 5847
        {
            if (g_Config.CheckHpPercents[g_Config.MedicaMode] > 99)                             // 5849
            {
                g_Config.CheckHpPercents[g_Config.MedicaMode] = 50;                             // 5851
                Plug.PlugEditCheckHPPercent = 50;                                               // 5852
            }

            if (g_Config.CheckMpPercents[g_Config.MedicaMode] > 99)                             // 5855
            {
                g_Config.CheckMpPercents[g_Config.MedicaMode] = 50;                             // 5857
                Plug.PlugEditCheckMPPercent = 50;                                               // 5858
            }
        }
    }

    /// <summary>
    /// 原文 5863-5894：4 路纠正 —— HP/MP 归 <c>50</c>，SpecialHP/SpecialMP 归 <c>30</c>
    /// （★ 与 AutoPercent 的 50 不同 —— 原文如此）。
    /// </summary>
    public void DCheckBoxRenewAutoPercentClick()
    {
        g_Config.ChkRenewAutoPercents[g_Config.MedicaMode] = Plug.PlugCheckBoxRenewAutoPercent;  // 5866

        if (g_Config.ChkRenewAutoPercents[g_Config.MedicaMode])                                  // 5868
        {
            if (g_Config.RenewHPPercents[g_Config.MedicaMode] > 99)                              // 5870
            {
                g_Config.RenewHPPercents[g_Config.MedicaMode] = 50;                              // 5872
                Plug.PlugEditRenewHPPercent = 50;                                                // 5873
            }

            if (g_Config.RenewMPPercents[g_Config.MedicaMode] > 99)                              // 5876
            {
                g_Config.RenewMPPercents[g_Config.MedicaMode] = 50;                              // 5878
                Plug.PlugEditRenewMPPercent = 50;                                                // 5879
            }

            if (g_Config.RenewSpecialHPPercents[g_Config.MedicaMode] > 99)                       // 5882
            {
                g_Config.RenewSpecialHPPercents[g_Config.MedicaMode] = 30;                       // 5884
                Plug.PlugEditRenewSpecialHPPercent = 30;                                         // 5885
            }

            if (g_Config.RenewSpecialMPPercents[g_Config.MedicaMode] > 99)                       // 5888
            {
                g_Config.RenewSpecialMPPercents[g_Config.MedicaMode] = 30;                       // 5890
                Plug.PlugEditRenewSpecialMPPercent = 30;                                         // 5891
            }
        }
    }

    /// <summary>
    /// 原文 5896-5940：写 <c>ChkSuperMedicaPercents</c>；为真时把 9 个 <c>SuperMedicaHPs</c> 与
    /// 9 个 <c>SuperMedicaMPs</c> 里 <c>&gt; 99</c> 的归 50，然后回写控件。
    ///
    /// ★★ 原文缺陷（**本车道最重要的一处**，登记不改，测试固定）：
    /// 5905 的循环 <c>for I := Low(..) to High(..)</c> 覆盖 **0..8**（9 个），
    /// 但 5913-5920 的回写**只到 <c>PlugEditSuperMedicaHP8</c> ← 下标 [7]**：
    /// 回写序列是 <c>HP1←[0], HP2←[1], …, HP8←[7]</c> —— 即
    /// <list type="bullet">
    /// <item><c>PlugEditSuperMedicaHP0</c>（原文 136 行声明的那个控件）**从未被回写**；</item>
    /// <item>控件名后缀与数组下标**整体错位 1**（<c>HP1</c> 承载的是下标 <c>0</c>）；</item>
    /// <item>数组下标 <c>[8]</c>（'超级疗伤药'）的纠正结果**不回显**给任何控件。</item>
    /// </list>
    /// MP 侧（5931-5938）完全同构。原文如此，逐行照抄（不"修正"错位）。
    /// </summary>
    public void DCheckBoxSuperMedicaPercentClick()
    {
        g_Config.ChkSuperMedicaPercents[g_Config.MedicaMode] = Plug.PlugCheckBoxSuperMedicaPercent;  // 5901

        if (g_Config.ChkSuperMedicaPercents[g_Config.MedicaMode])                                     // 5903
        {
            // 5905-5911：Low(..)=0, High(..)=8
            for (int I = 0; I <= 8; I++)
            {
                if (g_Config.SuperMedicaHPs[g_Config.MedicaMode, I] > 99)                             // 5907
                {
                    g_Config.SuperMedicaHPs[g_Config.MedicaMode, I] = 50;                             // 5909
                }
            }

            // 5913-5920：★ 回写到控件下标 0..7（不是数组下标 0..8），且控件名是 HP1..HP8
            for (int I = 0; I <= 7; I++)
                Plug.SetPlugEditSuperMedicaHP(I, g_Config.SuperMedicaHPs[g_Config.MedicaMode, I]);

            // 5923-5929
            for (int I = 0; I <= 8; I++)
            {
                if (g_Config.SuperMedicaMPs[g_Config.MedicaMode, I] > 99)                             // 5925
                {
                    g_Config.SuperMedicaMPs[g_Config.MedicaMode, I] = 50;                             // 5927
                }
            }

            // 5931-5938：同构错位
            for (int I = 0; I <= 7; I++)
                Plug.SetPlugEditSuperMedicaMP(I, g_Config.SuperMedicaMPs[g_Config.MedicaMode, I]);
        }
    }
}

/// <summary>
/// 接缝：<c>DEditChange(Sender)</c> 的 Sender 判等（原文 2548/2552）。
/// </summary>
public enum MirReturnEditId
{
    None = 0,
    PlugEditExpFilter,
    PlugEditAutoMagicTime,
}

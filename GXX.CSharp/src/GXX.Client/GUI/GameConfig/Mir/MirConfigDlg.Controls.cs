// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
// 本分片覆盖（原文行号）：
//   2962-2965  DEditCheckHPPercentChange
//   2967-2971  PlugEditHeroDodgeHPPercentChange（★ 唯一一处 SendPlugInConfig 的即时下发）
//   2973-2976  DEditCheckMPPercentChange
//   2978-2981  ComboBoxCheckHPValueChange（★ 写的是 ItemIndex 而不是文本）
//   2983-2986  ComboBoxCheckMPValueChange
//   2988-2991  DCheckBoxCheckHPIsAutoClick
//   2993-2996  DCheckBoxCheckMPIsAutoClick
//   2998-3001  DCheckBoxCheckDuraIsAuto
//   3003-3007  DEditCheckDuraChange（Max(1, …) 下界夹紧 + **回写控件**）
//   3009-3012  DEditCheckDuraValueChange（string[20] 无长度校验）
//   3014-3018  DEditCheckDuraTimeChange（Max(2, …) + 回写）
//   3020-3062  四组 Renew{HP,MP,SpecialHP,SpecialMP}{Percent,Time}Change（Time 夹紧到 dwPluginMinEatItemTime）
//   3064-3082  四个 Renew*IsAutoClick
//   3084-3314  DEditSuperMedica{HP,HPTime,MP,MPTime}Change + DCheckBoxUseSuperMedicaItemNameClick（9 路 Sender 判等）
//   3315-3324  DEditChange
//   7065-7073  DEditSpecialColorChange
//   7716-7721  DEditNotRushMonRangeChange
//   7810-7815  DEditGroupAttackCountChanged
//   7722-7745  ComboBoxPlayAttackValueSelect
//   7903-7988  DCheckBox{Auto,RenewAuto,SuperMedica}PercentClick

using System;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig.Mir;

public partial class TMirConfigDlg
{
    // ================================================================================
    // 2962-3001  Check 系列（HP/MP 百分比、下拉、勾选、耐久）
    // ✓ 已 1:1 移入
    // ================================================================================

    /// <summary>原文 2962-2965。</summary>
    public void DEditCheckHPPercentChange(object sender)
        => g_Config.CheckHpPercents[g_Config.MedicaMode] = PlugCtl.PlugEditCheckHPPercent.Value;      // 2964

    /// <summary>
    /// 原文 2967-2971：写 <c>nHeroDodgeHPPercent</c> 后调 <c>frmMain.SendPlugInConfig(...)</c>
    /// （★ 本单元**唯一**一处即时下发；与 MirReturnConfigDlg 的同名方法同一处置）。
    /// </summary>
    public void PlugEditHeroDodgeHPPercentChange(object sender)
    {
        g_Config.nHeroDodgeHPPercent = PlugCtl.PlugEditHeroDodgeHPPercent.Value;                     // 2969
        MirConfigGlobalSeam.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);                          // 2970
    }

    /// <summary>原文 2973-2976。</summary>
    public void DEditCheckMPPercentChange(object sender)
        => g_Config.CheckMpPercents[g_Config.MedicaMode] = PlugCtl.PlugEditCheckMPPercent.Value;      // 2975

    /// <summary>原文 2978-2981：★ 写的是 <c>ItemIndex</c>（下拉**下标**）而不是文本。</summary>
    public void ComboBoxCheckHPValueChange(object sender)
        => g_Config.CheckHpValues[g_Config.MedicaMode] = PlugCtl.PlugComboBoxCheckHPValue.ItemIndex;  // 2980

    /// <summary>原文 2983-2986：同 HP 侧。</summary>
    public void ComboBoxCheckMPValueChange(object sender)
        => g_Config.CheckMpValues[g_Config.MedicaMode] = PlugCtl.PlugComboBoxCheckMPValue.ItemIndex;  // 2985

    /// <summary>原文 2988-2991。</summary>
    public void DCheckBoxCheckHPIsAutoClick(object sender, int X, int Y)
        => g_Config.CheckHpIsAutos[g_Config.MedicaMode] = PlugCtl.PlugCheckBoxCheckHPIsAuto.Checked;  // 2990

    /// <summary>原文 2993-2996。</summary>
    public void DCheckBoxCheckMPIsAutoClick(object sender, int X, int Y)
        => g_Config.CheckMpIsAutos[g_Config.MedicaMode] = PlugCtl.PlugCheckBoxCheckMPIsAuto.Checked;  // 2995

    /// <summary>原文 2998-3001。</summary>
    public void DCheckBoxCheckDuraIsAuto(object sender, int X, int Y)
        => g_Config.CheckDuraIsAutos[g_Config.MedicaMode] = PlugCtl.PlugCheckBoxCheckDuraIsAuto.Checked;  // 3000

    /// <summary>原文 3003-3007：<c>Max(1, Value)</c> 后**回写控件**。</summary>
    public void DEditCheckDuraChange(object sender)
    {
        g_Config.CheckDuraMin[g_Config.MedicaMode] = MirConfigGlobalSeam.Max(1, PlugCtl.PlugEditCheckDura.Value);  // 3005
        PlugCtl.PlugEditCheckDura.Value = g_Config.CheckDuraMin[g_Config.MedicaMode];                              // 3006
    }

    /// <summary>原文 3009-3012：★ 写进 <c>string[20]</c>（无长度校验，超长由控件侧截断）。</summary>
    public void DEditCheckDuraValueChange(object sender)
        => g_Config.CheckDuraValue[g_Config.MedicaMode] = PlugCtl.PlugEditCheckDuraValue.Text;         // 3011

    /// <summary>原文 3014-3018：<c>Max(2, Value)</c> + 回写。</summary>
    public void DEditCheckDuraTimeChange(object sender)
    {
        g_Config.CheckDuraTime[g_Config.MedicaMode] = MirConfigGlobalSeam.Max(2, PlugCtl.PlugEditCheckDuraTime.Value);  // 3016
        PlugCtl.PlugEditCheckDuraTime.Value = g_Config.CheckDuraTime[g_Config.MedicaMode];                             // 3017
    }

    // ================================================================================
    // 3020-3082  Renew* 系列
    // ✓ 已 1:1 移入
    // ================================================================================

    /// <summary>原文 3020-3023。</summary>
    public void DEditRenewHPPercentChange(object sender)
        => g_Config.RenewHPPercents[g_Config.MedicaMode] = PlugCtl.PlugEditRenewHPPercent.Value;      // 3022

    /// <summary>原文 3025-3028。</summary>
    public void DEditRenewMPPercentChange(object sender)
        => g_Config.RenewMPPercents[g_Config.MedicaMode] = PlugCtl.PlugEditRenewMPPercent.Value;      // 3027

    /// <summary>原文 3030-3033。</summary>
    public void DEditRenewSpecialHPPercentChange(object sender)
        => g_Config.RenewSpecialHPPercents[g_Config.MedicaMode] = PlugCtl.PlugEditRenewSpecialHPPercent.Value;  // 3032

    /// <summary>原文 3035-3038。</summary>
    public void DEditRenewSpecialMPPercentChange(object sender)
        => g_Config.RenewSpecialMPPercents[g_Config.MedicaMode] = PlugCtl.PlugEditRenewSpecialMPPercent.Value;  // 3037

    /// <summary>
    /// 原文 3040-3044：<c>Max(g_ClientConfig.dwPluginMinEatItemTime, PlugEditRenewHPTime.Value)</c> + 回写。
    /// ★ 类型语义照抄：<c>dwPluginMinEatItemTime</c> 是 <c>LongWord</c>，
    ///   Delphi <c>Math.Max(Integer, LongWord)</c> 提升到 Int64 比较，赋回 Integer 时按位截断。
    /// </summary>
    public void DEditRenewHPTimeChange(object sender)
    {
        g_Config.RenewHPTimes[g_Config.MedicaMode] = ClampMinTime(PlugCtl.PlugEditRenewHPTime.Value);   // 3042
        PlugCtl.PlugEditRenewHPTime.Value = g_Config.RenewHPTimes[g_Config.MedicaMode];                 // 3043
    }

    /// <summary>原文 3046-3050。</summary>
    public void DEditRenewMPTimeChange(object sender)
    {
        g_Config.RenewMPTimes[g_Config.MedicaMode] = ClampMinTime(PlugCtl.PlugEditRenewMPTime.Value);   // 3048
        PlugCtl.PlugEditRenewMPTime.Value = g_Config.RenewMPTimes[g_Config.MedicaMode];                 // 3049
    }

    /// <summary>原文 3052-3056。</summary>
    public void DEditRenewSpecialHPTimeChange(object sender)
    {
        g_Config.RenewSpecialHPTimes[g_Config.MedicaMode] = ClampMinTime(PlugCtl.PlugEditRenewSpecialHPTime.Value);  // 3054
        PlugCtl.PlugEditRenewSpecialHPTime.Value = g_Config.RenewSpecialHPTimes[g_Config.MedicaMode];                // 3055
    }

    /// <summary>原文 3058-3062。</summary>
    public void DEditRenewSpecialMPTimeChange(object sender)
    {
        g_Config.RenewSpecialMPTimes[g_Config.MedicaMode] = ClampMinTime(PlugCtl.PlugEditRenewSpecialMPTime.Value);  // 3060
        PlugCtl.PlugEditRenewSpecialMPTime.Value = g_Config.RenewSpecialMPTimes[g_Config.MedicaMode];                // 3061
    }

    /// <summary>原文 3042/3048/3054/3060 共用的 <c>Max(LongWord, Integer)</c> → Integer 截断。</summary>
    private static int ClampMinTime(int value)
        => unchecked((int)MirConfigGlobalSeam.MaxL(
               (long)MirConfigGlobalSeam.g_ClientConfig.dwPluginMinEatItemTime, value));

    /// <summary>原文 3064-3067。</summary>
    public void DCheckBoxRenewHPIsAutoClick(object sender, int X, int Y)
        => g_Config.RenewHPIsAutos[g_Config.MedicaMode] = PlugCtl.PlugCheckBoxRenewHPIsAuto.Checked;  // 3066

    /// <summary>原文 3069-3072。</summary>
    public void DCheckBoxRenewMPIsAutoClick(object sender, int X, int Y)
        => g_Config.RenewMPIsAutos[g_Config.MedicaMode] = PlugCtl.PlugCheckBoxRenewMPIsAuto.Checked;  // 3071

    /// <summary>原文 3074-3077。</summary>
    public void DCheckBoxRenewSpecialHPIsAutoClick(object sender, int X, int Y)
        => g_Config.RenewSpecialHPIsAutos[g_Config.MedicaMode] = PlugCtl.PlugCheckBoxRenewSpecialHPIsAuto.Checked;  // 3076

    /// <summary>原文 3079-3082。</summary>
    public void DCheckBoxRenewSpecialMPIsAutoClick(object sender, int X, int Y)
        => g_Config.RenewSpecialMPIsAutos[g_Config.MedicaMode] = PlugCtl.PlugCheckBoxRenewSpecialMPIsAuto.Checked;  // 3081

    // ================================================================================
    // 3084-3314  超级药（9 路 Sender 判等 → 索引 → 按下标写矩阵）
    // ✓ 已 1:1 移入（原文是四段几乎相同的 9 路 if-else 链，逐段照抄）
    // ================================================================================

    /// <summary>
    /// 原文 3084-3121：<c>DEditSuperMedicaHPChange</c>。
    /// <c>Index := -1; Value := 0; //HZQ 20230525</c> → 9 路 <c>PlugEditSuperMedicaHPn = Sender</c> 判等
    /// → <c>if Index in [0..8] then g_Config.SuperMedicaHPs[g_Config.MedicaMode][Index] := Value;</c>
    /// </summary>
    public void DEditSuperMedicaHPChange(object sender)
    {
        int Index = -1;                                        // 3088
        int Value = 0;                                         // 3089 //HZQ 20230525
        if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHP0, sender)) { Index = 0; Value = PlugCtl.PlugEditSuperMedicaHP0.Value; }       // 3090-3092
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHP1, sender)) { Index = 1; Value = PlugCtl.PlugEditSuperMedicaHP1.Value; }  // 3093-3095
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHP2, sender)) { Index = 2; Value = PlugCtl.PlugEditSuperMedicaHP2.Value; }  // 3096-3098
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHP3, sender)) { Index = 3; Value = PlugCtl.PlugEditSuperMedicaHP3.Value; }  // 3099-3101
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHP4, sender)) { Index = 4; Value = PlugCtl.PlugEditSuperMedicaHP4.Value; }  // 3102-3104
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHP5, sender)) { Index = 5; Value = PlugCtl.PlugEditSuperMedicaHP5.Value; }  // 3105-3107
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHP6, sender)) { Index = 6; Value = PlugCtl.PlugEditSuperMedicaHP6.Value; }  // 3108-3110
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHP7, sender)) { Index = 7; Value = PlugCtl.PlugEditSuperMedicaHP7.Value; }  // 3111-3113
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHP8, sender)) { Index = 8; Value = PlugCtl.PlugEditSuperMedicaHP8.Value; }  // 3114-3116
        if (Index >= 0 && Index <= 8)                          // 3118  if Index in [0..8] then
            g_Config.SuperMedicaHPs[g_Config.MedicaMode][Index] = Value;   // 3119
    }

    /// <summary>原文 3123-3169：<c>DEditSuperMedicaHPTimeChange</c>（同 9 路形态，写 <c>SuperMedicaHPTimes</c>）。</summary>
    public void DEditSuperMedicaHPTimeChange(object sender)
    {
        int Index = -1;
        int Value = 0;
        if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHPTime0, sender)) { Index = 0; Value = PlugCtl.PlugEditSuperMedicaHPTime0.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHPTime1, sender)) { Index = 1; Value = PlugCtl.PlugEditSuperMedicaHPTime1.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHPTime2, sender)) { Index = 2; Value = PlugCtl.PlugEditSuperMedicaHPTime2.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHPTime3, sender)) { Index = 3; Value = PlugCtl.PlugEditSuperMedicaHPTime3.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHPTime4, sender)) { Index = 4; Value = PlugCtl.PlugEditSuperMedicaHPTime4.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHPTime5, sender)) { Index = 5; Value = PlugCtl.PlugEditSuperMedicaHPTime5.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHPTime6, sender)) { Index = 6; Value = PlugCtl.PlugEditSuperMedicaHPTime6.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHPTime7, sender)) { Index = 7; Value = PlugCtl.PlugEditSuperMedicaHPTime7.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaHPTime8, sender)) { Index = 8; Value = PlugCtl.PlugEditSuperMedicaHPTime8.Value; }
        if (Index >= 0 && Index <= 8)
            g_Config.SuperMedicaHPTimes[g_Config.MedicaMode][Index] = Value;
    }

    /// <summary>原文 3171-3217：<c>DEditSuperMedicaMPChange</c>（写 <c>SuperMedicaMPs</c>）。</summary>
    public void DEditSuperMedicaMPChange(object sender)
    {
        int Index = -1;
        int Value = 0;
        if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMP0, sender)) { Index = 0; Value = PlugCtl.PlugEditSuperMedicaMP0.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMP1, sender)) { Index = 1; Value = PlugCtl.PlugEditSuperMedicaMP1.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMP2, sender)) { Index = 2; Value = PlugCtl.PlugEditSuperMedicaMP2.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMP3, sender)) { Index = 3; Value = PlugCtl.PlugEditSuperMedicaMP3.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMP4, sender)) { Index = 4; Value = PlugCtl.PlugEditSuperMedicaMP4.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMP5, sender)) { Index = 5; Value = PlugCtl.PlugEditSuperMedicaMP5.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMP6, sender)) { Index = 6; Value = PlugCtl.PlugEditSuperMedicaMP6.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMP7, sender)) { Index = 7; Value = PlugCtl.PlugEditSuperMedicaMP7.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMP8, sender)) { Index = 8; Value = PlugCtl.PlugEditSuperMedicaMP8.Value; }
        if (Index >= 0 && Index <= 8)
            g_Config.SuperMedicaMPs[g_Config.MedicaMode][Index] = Value;
    }

    /// <summary>原文 3219-3265：<c>DEditSuperMedicaMPTimeChange</c>（写 <c>SuperMedicaMPTimes</c>）。</summary>
    public void DEditSuperMedicaMPTimeChange(object sender)
    {
        int Index = -1;
        int Value = 0;
        if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMPTime0, sender)) { Index = 0; Value = PlugCtl.PlugEditSuperMedicaMPTime0.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMPTime1, sender)) { Index = 1; Value = PlugCtl.PlugEditSuperMedicaMPTime1.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMPTime2, sender)) { Index = 2; Value = PlugCtl.PlugEditSuperMedicaMPTime2.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMPTime3, sender)) { Index = 3; Value = PlugCtl.PlugEditSuperMedicaMPTime3.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMPTime4, sender)) { Index = 4; Value = PlugCtl.PlugEditSuperMedicaMPTime4.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMPTime5, sender)) { Index = 5; Value = PlugCtl.PlugEditSuperMedicaMPTime5.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMPTime6, sender)) { Index = 6; Value = PlugCtl.PlugEditSuperMedicaMPTime6.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMPTime7, sender)) { Index = 7; Value = PlugCtl.PlugEditSuperMedicaMPTime7.Value; }
        else if (ReferenceEquals(PlugCtl.PlugEditSuperMedicaMPTime8, sender)) { Index = 8; Value = PlugCtl.PlugEditSuperMedicaMPTime8.Value; }
        if (Index >= 0 && Index <= 8)
            g_Config.SuperMedicaMPTimes[g_Config.MedicaMode][Index] = Value;
    }

    /// <summary>
    /// 原文 3267-3313：<c>DCheckBoxUseSuperMedicaItemNameClick</c>（**9 处绑定**，同一处理器）。
    /// 9 路 <c>PlugCheckBoxUseSuperMedicaItemN = Sender</c> 判等 → 按下标写 <c>SuperMedicaUses</c>。
    /// </summary>
    public void DCheckBoxUseSuperMedicaItemNameClick(object sender, int X, int Y)
    {
        int Index = -1;
        bool Value = false;                                    // 原文如此（3270 起 Value 初值 False）
        if (ReferenceEquals(PlugCtl.PlugCheckBoxUseSuperMedicaItemName0, sender)) { Index = 0; Value = PlugCtl.PlugCheckBoxUseSuperMedicaItemName0.Checked; }
        else if (ReferenceEquals(PlugCtl.PlugCheckBoxUseSuperMedicaItemName1, sender)) { Index = 1; Value = PlugCtl.PlugCheckBoxUseSuperMedicaItemName1.Checked; }
        else if (ReferenceEquals(PlugCtl.PlugCheckBoxUseSuperMedicaItemName2, sender)) { Index = 2; Value = PlugCtl.PlugCheckBoxUseSuperMedicaItemName2.Checked; }
        else if (ReferenceEquals(PlugCtl.PlugCheckBoxUseSuperMedicaItemName3, sender)) { Index = 3; Value = PlugCtl.PlugCheckBoxUseSuperMedicaItemName3.Checked; }
        else if (ReferenceEquals(PlugCtl.PlugCheckBoxUseSuperMedicaItemName4, sender)) { Index = 4; Value = PlugCtl.PlugCheckBoxUseSuperMedicaItemName4.Checked; }
        else if (ReferenceEquals(PlugCtl.PlugCheckBoxUseSuperMedicaItemName5, sender)) { Index = 5; Value = PlugCtl.PlugCheckBoxUseSuperMedicaItemName5.Checked; }
        else if (ReferenceEquals(PlugCtl.PlugCheckBoxUseSuperMedicaItemName6, sender)) { Index = 6; Value = PlugCtl.PlugCheckBoxUseSuperMedicaItemName6.Checked; }
        else if (ReferenceEquals(PlugCtl.PlugCheckBoxUseSuperMedicaItemName7, sender)) { Index = 7; Value = PlugCtl.PlugCheckBoxUseSuperMedicaItemName7.Checked; }
        else if (ReferenceEquals(PlugCtl.PlugCheckBoxUseSuperMedicaItemName8, sender)) { Index = 8; Value = PlugCtl.PlugCheckBoxUseSuperMedicaItemName8.Checked; }
        if (Index >= 0 && Index <= 8)
            g_Config.SuperMedicaUses[g_Config.MedicaMode][Index] = Value;
    }

    // ================================================================================
    // 3315-3324 / 7065-7073 / 7716-7815 / 7722-7745 / 7903-7988  杂项处理器
    // ================================================================================

    /// <summary>
    /// 原文 3315-3324：<c>DEditChange</c>（2 处绑定：PlugEditExpFilter / PlugEditAutoMagicTime）。
    /// 托管侧用控件名分派（原文是 <c>Sender = PlugEditExpFilter</c> 判等）。
    /// </summary>
    public void DEditChange(object sender)
    {
        if (ReferenceEquals(PlugCtl.PlugEditExpFilter, sender))            // 3317
            g_Config.nFilterMinExp = PlugCtl.PlugEditExpFilter.Value;      // 3318
        else if (ReferenceEquals(PlugCtl.PlugEditAutoMagicTime, sender))   // 3319
            g_Config.nAutoUseMagicTime = PlugCtl.PlugEditAutoMagicTime.Value;  // 3320
    }

    /// <summary>原文 7065-7073：<c>DEditSpecialColorChange</c>（写 g_Config.nSpecialColor + 下发 frmMain）。</summary>
    public void DEditSpecialColorChange(object sender)
    {
        int v = PlugCtl.PlugEditSpecialColor.Value;
        if (v >= 0 && v <= 255)                                // 原文 7067 起有范围判定
        {
            g_Config.nSpecialColor = (byte)v;                  // 7068
            MirConfigGlobalSeam.SetFrmMainSpecialColor(g_Config.nSpecialColor);   // 7069
        }
        // TODO(p10 后续切片)：7065-7073 剩余分支（原文含 PlugEditSpecialName 联动）见报告
        NotPorted(nameof(DEditSpecialColorChange) + " 的 7070-7073 分支", 7070);
    }

    /// <summary>原文 7716-7721：<c>DEditNotRushMonRangeChange</c>。</summary>
    public void DEditNotRushMonRangeChange(object sender)
    {
        g_Config.nGJNotRushMonRange = PlugCtl.PlugEditNotRushMonRange.Value;              // 7718
        MirConfigGlobalSeam.SetFrmMainGJNotRushMonRange(g_Config.nGJNotRushMonRange);     // 7719
    }

    /// <summary>原文 7810-7815：<c>DEditGroupAttackCountChanged</c>。</summary>
    public void DEditGroupAttackCountChanged(object sender)
    {
        g_Config.nGJGroupAttackCount = PlugCtl.PlugEditNotGroupAttackCount.Value;         // 7812
        MirConfigGlobalSeam.SetFrmMainGJGroupAttackCount(g_Config.nGJGroupAttackCount);   // 7813
    }

    /// <summary>原文 7722-7745：<c>ComboBoxPlayAttackValueSelect</c>（**5 处绑定**：受玩家攻击/红蓝毒/毒符/包裹满）。</summary>
    public void ComboBoxPlayAttackValueSelect(object sender)
        => NotPorted(nameof(ComboBoxPlayAttackValueSelect), 7722);

    /// <summary>原文 7903-7920：<c>DCheckBoxAutoPercentClick</c>（原文缺陷：>99 归 50）。</summary>
    public void DCheckBoxAutoPercentClick(object sender, int X, int Y)
        => NotPorted(nameof(DCheckBoxAutoPercentClick), 7903);

    /// <summary>原文 7921-7948：<c>DCheckBoxRenewAutoPercentClick</c>（>99 归 30）。</summary>
    public void DCheckBoxRenewAutoPercentClick(object sender, int X, int Y)
        => NotPorted(nameof(DCheckBoxRenewAutoPercentClick), 7921);

    /// <summary>原文 7949-7988：<c>DCheckBoxSuperMedicaPercentClick</c>。</summary>
    public void DCheckBoxSuperMedicaPercentClick(object sender, int X, int Y)
        => NotPorted(nameof(DCheckBoxSuperMedicaPercentClick), 7949);
}

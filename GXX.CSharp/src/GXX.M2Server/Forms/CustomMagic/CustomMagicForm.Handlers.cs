// ============================================================================
// uFrmCustomMagic.pas（Source\M2Engine\Forms\uFrmCustomMagic.pas，4800 行，GBK）1:1 移植
// 车道 p5-m2-custommagic ｜ 命名空间 GXX.M2Server.Forms.CustomMagic
//
// 本文件 = **表驱动生成**的 151 个"单字段写回"事件处理器。
// 生成方式（禁止手工转录）：脚本从 uFrmCustomMagic.pas 原文抽取每个
//   procedure TFrmCustomMagic.XxxChange/Click(Sender: TObject);
// 的规范化体（去空行/去 // 注释），只保留形状
//   begin / if <G> <> nil then / begin / <G>.<Path> := <Rhs>; / SetConfigChanged(); / end; / end;
// 的那些，按原文行号顺序生成；<Rhs> 的窄化/枚举强转按目标字段的 C# 类型补（目标类型
// 取自 Grobal2.Types5.cs 的 TMagicClientBaseConfig/TMagicClientConfig 与
// 本目录 CustomMagicModel.cs 的 TMagicServerConfig）。逐条来源行号见方法上方注释。
//
// 覆盖行号（Delphi）：见各方法注释（uFrmCustomMagic.pas 内的 151 个区间）
// ============================================================================

using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms.CustomMagic;

public partial class TFrmCustomMagic
{
    /// <summary>原文 uFrmCustomMagic.pas:2510-2518 TFrmCustomMagic.chkClientLockClick（表驱动：FCurrentCustomConfig.ClientBaseConfig.MagicLock）</summary>
    public void ChkClientLockClick()
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.ClientBaseConfig.MagicLock = (byte)(chkClientLock.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2533-2541 TFrmCustomMagic.chkClientLockSelfClick（表驱动：FCurrentCustomConfig.ClientBaseConfig.MagicLockSelf）</summary>
    public void ChkClientLockSelfClick()
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.ClientBaseConfig.MagicLockSelf = (byte)(chkClientLockSelf.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2542-2550 TFrmCustomMagic.chkClientNotRaiseHandClick（表驱动：FCurrentCustomConfig.ClientBaseConfig.NotRaiseHand）</summary>
    public void ChkClientNotRaiseHandClick()
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.ClientBaseConfig.NotRaiseHand = (byte)(chkClientNotRaiseHand.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2551-2559 TFrmCustomMagic.cbbClientIconFileChange（表驱动：FCurrentClientConfig.Icon_File）</summary>
    public void CbbClientIconFileChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Icon_File = (short)(cbbClientIconFile.ItemIndex - 1);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2560-2568 TFrmCustomMagic.seClientIconIndexChange（表驱动：FCurrentClientConfig.Icon_Index）</summary>
    public void SeClientIconIndexChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Icon_Index = (ushort)(seClientIconIndex.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2569-2577 TFrmCustomMagic.cbbClientFlyFileChange（表驱动：FCurrentClientConfig.Fly_File）</summary>
    public void CbbClientFlyFileChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Fly_File = (short)(cbbClientFlyFile.ItemIndex - 1);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2578-2586 TFrmCustomMagic.seClientFlyStartIndexChange（表驱动：FCurrentClientConfig.Fly_StartIndex）</summary>
    public void SeClientFlyStartIndexChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Fly_StartIndex = (ushort)(seClientFlyStartIndex.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2587-2595 TFrmCustomMagic.seClientFlyPlayCountChange（表驱动：FCurrentClientConfig.Fly_PlayCount）</summary>
    public void SeClientFlyPlayCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Fly_PlayCount = (ushort)(seClientFlyPlayCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2596-2604 TFrmCustomMagic.seClientFlyEmptyCountChange（表驱动：FCurrentClientConfig.Fly_EmptyCount）</summary>
    public void SeClientFlyEmptyCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Fly_EmptyCount = (ushort)(seClientFlyEmptyCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2605-2613 TFrmCustomMagic.seClientFlyPlayTimeChange（表驱动：FCurrentClientConfig.Fly_PlayTime）</summary>
    public void SeClientFlyPlayTimeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Fly_PlayTime = (ushort)(seClientFlyPlayTime.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2614-2622 TFrmCustomMagic.cbbClientFlyDrawModeChange（表驱动：FCurrentClientConfig.Fly_DrawMode）</summary>
    public void CbbClientFlyDrawModeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Fly_DrawMode = (TCustomDrawMode)(cbbClientFlyDrawMode.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2623-2631 TFrmCustomMagic.cbbClientFlyDirCountChange（表驱动：FCurrentClientConfig.Fly_DirCount）</summary>
    public void CbbClientFlyDirCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Fly_DirCount = (TCustomDirCount)(cbbClientFlyDirCount.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2632-2640 TFrmCustomMagic.chkClientFlyCalcDirClick（表驱动：FCurrentClientConfig.Fly_CalcDir）</summary>
    public void ChkClientFlyCalcDirClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Fly_CalcDir = (byte)(chkClientFlyCalcDir.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2641-2649 TFrmCustomMagic.chkClientFlyFireGunModeClick（表驱动：FCurrentClientConfig.Fly_FireGunMode）</summary>
    public void ChkClientFlyFireGunModeClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Fly_FireGunMode = (byte)(chkClientFlyFireGunMode.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2650-2658 TFrmCustomMagic.seClientFlyLightRangeChange（表驱动：FCurrentClientConfig.Fly_LightRange）</summary>
    public void SeClientFlyLightRangeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Fly_LightRange = (byte)(seClientFlyLightRange.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2659-2667 TFrmCustomMagic.cbbClientFlyEffFileChange（表驱动：FCurrentClientConfig.FlyEff_File）</summary>
    public void CbbClientFlyEffFileChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.FlyEff_File = (short)(cbbClientFlyEffFile.ItemIndex - 1);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2668-2676 TFrmCustomMagic.seClientFlyEffStartIndexChange（表驱动：FCurrentClientConfig.FlyEff_StartIndex）</summary>
    public void SeClientFlyEffStartIndexChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.FlyEff_StartIndex = (ushort)(seClientFlyEffStartIndex.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2677-2685 TFrmCustomMagic.cbbClientFlyEffDrawModeChange（表驱动：FCurrentClientConfig.FlyEff_DrawMode）</summary>
    public void CbbClientFlyEffDrawModeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.FlyEff_DrawMode = (TCustomDrawMode)(cbbClientFlyEffDrawMode.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2686-2694 TFrmCustomMagic.cbbClientSelfFileChange（表驱动：FCurrentClientConfig.Self_File）</summary>
    public void CbbClientSelfFileChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_File = (short)(cbbClientSelfFile.ItemIndex - 1);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2695-2703 TFrmCustomMagic.seClientSelfStartIndexChange（表驱动：FCurrentClientConfig.Self_StartIndex）</summary>
    public void SeClientSelfStartIndexChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_StartIndex = (ushort)(seClientSelfStartIndex.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2704-2712 TFrmCustomMagic.seClientSelfPlayCountChange（表驱动：FCurrentClientConfig.Self_PlayCount）</summary>
    public void SeClientSelfPlayCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_PlayCount = (ushort)(seClientSelfPlayCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2713-2721 TFrmCustomMagic.seClientSelfEmptyCountChange（表驱动：FCurrentClientConfig.Self_EmptyCount）</summary>
    public void SeClientSelfEmptyCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_EmptyCount = (ushort)(seClientSelfEmptyCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2722-2730 TFrmCustomMagic.seClientSelfPlayTimeChange（表驱动：FCurrentClientConfig.Self_PlayTime）</summary>
    public void SeClientSelfPlayTimeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_PlayTime = (ushort)(seClientSelfPlayTime.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2731-2739 TFrmCustomMagic.cbbClientSelfDrawOrderChange（表驱动：FCurrentClientConfig.Self_DrawOrder）</summary>
    public void CbbClientSelfDrawOrderChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_DrawOrder = (TCustomDrawOrder)(cbbClientSelfDrawOrder.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2740-2748 TFrmCustomMagic.cbbClientSelfDrawModeChange（表驱动：FCurrentClientConfig.Self_DrawMode）</summary>
    public void CbbClientSelfDrawModeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_DrawMode = (TCustomDrawMode)(cbbClientSelfDrawMode.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2749-2757 TFrmCustomMagic.cbbClientSelfDirCountChange（表驱动：FCurrentClientConfig.Self_DirCount）</summary>
    public void CbbClientSelfDirCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_DirCount = (TCustomDirCount)(cbbClientSelfDirCount.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2758-2766 TFrmCustomMagic.cbbClientSelfDirCalcTypeChange（表驱动：FCurrentClientConfig.Self_DirCalcType）</summary>
    public void CbbClientSelfDirCalcTypeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_DirCalcType = (TCustomDirCalcType)(cbbClientSelfDirCalcType.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2767-2775 TFrmCustomMagic.chkClientSelfPlayDelayActionClick（表驱动：FCurrentClientConfig.Self_PlayDelayAction）</summary>
    public void ChkClientSelfPlayDelayActionClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_PlayDelayAction = (byte)(chkClientSelfPlayDelayAction.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2776-2784 TFrmCustomMagic.chkClientSelfPlayFailNoDrawClick（表驱动：FCurrentClientConfig.Self_PlayFailNoDraw）</summary>
    public void ChkClientSelfPlayFailNoDrawClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_PlayFailNoDraw = (byte)(chkClientSelfPlayFailNoDraw.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2785-2793 TFrmCustomMagic.seClientSelfLightRangeChange（表驱动：FCurrentClientConfig.Self_LightRange）</summary>
    public void SeClientSelfLightRangeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_LightRange = (byte)(seClientSelfLightRange.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2794-2802 TFrmCustomMagic.cbbClientPreTargetFileChange（表驱动：FCurrentClientConfig.PreTarget_File）</summary>
    public void CbbClientPreTargetFileChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.PreTarget_File = (short)(cbbClientPreTargetFile.ItemIndex - 1);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2803-2811 TFrmCustomMagic.seClientPreTargetStartIndexChange（表驱动：FCurrentClientConfig.PreTarget_StartIndex）</summary>
    public void SeClientPreTargetStartIndexChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.PreTarget_StartIndex = (ushort)(seClientPreTargetStartIndex.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2812-2820 TFrmCustomMagic.seClientPreTargetStartIndex2Change（表驱动：FCurrentClientConfig.PreTarget_StartIndex2）</summary>
    public void SeClientPreTargetStartIndex2Change()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.PreTarget_StartIndex2 = (short)(seClientPreTargetStartIndex2.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2821-2829 TFrmCustomMagic.seClientPreTargetPlayCountChange（表驱动：FCurrentClientConfig.PreTarget_PlayCount）</summary>
    public void SeClientPreTargetPlayCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.PreTarget_PlayCount = (ushort)(seClientPreTargetPlayCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2830-2838 TFrmCustomMagic.seClientPreTargetEmptyCountChange（表驱动：FCurrentClientConfig.PreTarget_EmptyCount）</summary>
    public void SeClientPreTargetEmptyCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.PreTarget_EmptyCount = (ushort)(seClientPreTargetEmptyCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2839-2847 TFrmCustomMagic.chkClientPreTargetCalcDirClick（表驱动：FCurrentClientConfig.PreTarget_CalcDir）</summary>
    public void ChkClientPreTargetCalcDirClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.PreTarget_CalcDir = (byte)(chkClientPreTargetCalcDir.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2848-2856 TFrmCustomMagic.seClientPreTargetPlayTimeChange（表驱动：FCurrentClientConfig.PreTarget_PlayTime）</summary>
    public void SeClientPreTargetPlayTimeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.PreTarget_PlayTime = (ushort)(seClientPreTargetPlayTime.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2857-2865 TFrmCustomMagic.cbbClientPreTargetDrawModeChange（表驱动：FCurrentClientConfig.PreTarget_DrawMode）</summary>
    public void CbbClientPreTargetDrawModeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.PreTarget_DrawMode = (TCustomDrawMode)(cbbClientPreTargetDrawMode.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2866-2874 TFrmCustomMagic.cbbClientPreTargetDrawMode2Change（表驱动：FCurrentClientConfig.PreTarget_DrawMode2）</summary>
    public void CbbClientPreTargetDrawMode2Change()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.PreTarget_DrawMode2 = (TCustomDrawMode)(cbbClientPreTargetDrawMode2.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2875-2883 TFrmCustomMagic.chkClientPreTargetLockDrawClick（表驱动：FCurrentClientConfig.PreTarget_LockDraw）</summary>
    public void ChkClientPreTargetLockDrawClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.PreTarget_LockDraw = (byte)(chkClientPreTargetLockDraw.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2884-2892 TFrmCustomMagic.seClientPreTargetLightRangeChange（表驱动：FCurrentClientConfig.PreTarget_LightRange）</summary>
    public void SeClientPreTargetLightRangeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.PreTarget_LightRange = (byte)(seClientPreTargetLightRange.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2893-2901 TFrmCustomMagic.cbbClientTargetFileChange（表驱动：FCurrentClientConfig.Target_File）</summary>
    public void CbbClientTargetFileChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_File = (short)(cbbClientTargetFile.ItemIndex - 1);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2902-2910 TFrmCustomMagic.seClientTargetStartIndexChange（表驱动：FCurrentClientConfig.Target_StartIndex）</summary>
    public void SeClientTargetStartIndexChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_StartIndex = (ushort)(seClientTargetStartIndex.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2911-2919 TFrmCustomMagic.seClientTargetStartIndex2Change（表驱动：FCurrentClientConfig.Target_StartIndex2）</summary>
    public void SeClientTargetStartIndex2Change()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_StartIndex2 = (short)(seClientTargetStartIndex2.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2920-2928 TFrmCustomMagic.seClientTargetPlayCountChange（表驱动：FCurrentClientConfig.Target_PlayCount）</summary>
    public void SeClientTargetPlayCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_PlayCount = (ushort)(seClientTargetPlayCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2929-2937 TFrmCustomMagic.seClientTargetPlayTimeChange（表驱动：FCurrentClientConfig.Target_PlayTime）</summary>
    public void SeClientTargetPlayTimeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_PlayTime = (ushort)(seClientTargetPlayTime.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2938-2946 TFrmCustomMagic.cbbClientTargetDrawModeChange（表驱动：FCurrentClientConfig.Target_DrawMode）</summary>
    public void CbbClientTargetDrawModeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_DrawMode = (TCustomDrawMode)(cbbClientTargetDrawMode.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2947-2955 TFrmCustomMagic.cbbClientTargetDrawMode2Change（表驱动：FCurrentClientConfig.Target_DrawMode2）</summary>
    public void CbbClientTargetDrawMode2Change()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_DrawMode2 = (TCustomDrawMode)(cbbClientTargetDrawMode2.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2956-2964 TFrmCustomMagic.chkClientTargetMultiPlayClick（表驱动：FCurrentClientConfig.Target_MultiPlay）</summary>
    public void ChkClientTargetMultiPlayClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_MultiPlay = (byte)(chkClientTargetMultiPlay.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2965-2973 TFrmCustomMagic.chkClientTargetLockDrawClick（表驱动：FCurrentClientConfig.Target_LockDraw）</summary>
    public void ChkClientTargetLockDrawClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_LockDraw = (byte)(chkClientTargetLockDraw.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2974-2982 TFrmCustomMagic.seClientTargetLightRangeChange（表驱动：FCurrentClientConfig.Target_LightRange）</summary>
    public void SeClientTargetLightRangeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_LightRange = (byte)(seClientTargetLightRange.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2983-2991 TFrmCustomMagic.chkClientTargetKeepPlayClick（表驱动：FCurrentClientConfig.Target_KeepPlay）</summary>
    public void ChkClientTargetKeepPlayClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_KeepPlay = (byte)(chkClientTargetKeepPlay.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:2992-3000 TFrmCustomMagic.seClientTargetKeepTimeChange（表驱动：FCurrentClientConfig.Target_KeepTime）</summary>
    public void SeClientTargetKeepTimeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_KeepTime = (ushort)(seClientTargetKeepTime.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3001-3009 TFrmCustomMagic.seClientTargetKeepTime2Change（表驱动：FCurrentClientConfig.Target_KeepTime2）</summary>
    public void SeClientTargetKeepTime2Change()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_KeepTime2 = seClientTargetKeepTime2.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3010-3018 TFrmCustomMagic.seClientTargetKeepAttackIntervalChange（表驱动：FCurrentClientConfig.Target_KeepAttackInterval）</summary>
    public void SeClientTargetKeepAttackIntervalChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_KeepAttackInterval = (byte)(seClientTargetKeepAttackInterval.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3019-3027 TFrmCustomMagic.seClientTargetKeepAttackRangeChange（表驱动：FCurrentClientConfig.Target_KeepAttackRange）</summary>
    public void SeClientTargetKeepAttackRangeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_KeepAttackRange = (byte)(seClientTargetKeepAttackRange.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3028-3036 TFrmCustomMagic.seTargetKeepLightRangeChange（表驱动：FCurrentClientConfig.Target_KeepLightRange）</summary>
    public void SeTargetKeepLightRangeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_KeepLightRange = (byte)(seTargetKeepLightRange.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3037-3045 TFrmCustomMagic.chkClientTargetKeepMultiPlayClick（表驱动：FCurrentClientConfig.Target_KeepMultiPlay）</summary>
    public void ChkClientTargetKeepMultiPlayClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Target_KeepMultiPlay = (byte)(chkClientTargetKeepMultiPlay.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3046-3054 TFrmCustomMagic.cbbTargetStatus1_FileChange（表驱动：FCurrentClientConfig.TargetStatus1_File）</summary>
    public void CbbTargetStatus1_FileChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.TargetStatus1_File = (short)(cbbTargetStatus1_File.ItemIndex - 1);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3055-3063 TFrmCustomMagic.seTargetStatus1_StartIndexChange（表驱动：FCurrentClientConfig.TargetStatus1_StartIndex）</summary>
    public void SeTargetStatus1_StartIndexChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.TargetStatus1_StartIndex = (ushort)(seTargetStatus1_StartIndex.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3064-3072 TFrmCustomMagic.seTargetStatus1_PlayCountChange（表驱动：FCurrentClientConfig.TargetStatus1_PlayCount）</summary>
    public void SeTargetStatus1_PlayCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.TargetStatus1_PlayCount = (ushort)(seTargetStatus1_PlayCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3073-3081 TFrmCustomMagic.seTargetStatus1_EmptyCountChange（表驱动：FCurrentClientConfig.TargetStatus1_EmptyCount）</summary>
    public void SeTargetStatus1_EmptyCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.TargetStatus1_EmptyCount = (ushort)(seTargetStatus1_EmptyCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3091-3099 TFrmCustomMagic.cbbTargetStatus1_DrawModeChange（表驱动：FCurrentClientConfig.TargetStatus1_DrawMode）</summary>
    public void CbbTargetStatus1_DrawModeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.TargetStatus1_DrawMode = (TCustomDrawMode)(cbbTargetStatus1_DrawMode.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3100-3108 TFrmCustomMagic.chkTargetStatus1_CalcDirClick（表驱动：FCurrentClientConfig.TargetStatus1_CalcDir）</summary>
    public void ChkTargetStatus1_CalcDirClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.TargetStatus1_CalcDir = (byte)(chkTargetStatus1_CalcDir.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3109-3117 TFrmCustomMagic.cbbTargetStatus2_FileChange（表驱动：FCurrentClientConfig.TargetStatus2_File）</summary>
    public void CbbTargetStatus2_FileChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.TargetStatus2_File = (short)(cbbTargetStatus2_File.ItemIndex - 1);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3118-3126 TFrmCustomMagic.seTargetStatus2_StartIndexChange（表驱动：FCurrentClientConfig.TargetStatus2_StartIndex）</summary>
    public void SeTargetStatus2_StartIndexChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.TargetStatus2_StartIndex = (ushort)(seTargetStatus2_StartIndex.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3127-3135 TFrmCustomMagic.seTargetStatus2_PlayCountChange（表驱动：FCurrentClientConfig.TargetStatus2_PlayCount）</summary>
    public void SeTargetStatus2_PlayCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.TargetStatus2_PlayCount = (ushort)(seTargetStatus2_PlayCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3136-3144 TFrmCustomMagic.seTargetStatus2_EmptyCountChange（表驱动：FCurrentClientConfig.TargetStatus2_EmptyCount）</summary>
    public void SeTargetStatus2_EmptyCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.TargetStatus2_EmptyCount = (ushort)(seTargetStatus2_EmptyCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3154-3162 TFrmCustomMagic.cbbTargetStatus2_DrawModeChange（表驱动：FCurrentClientConfig.TargetStatus2_DrawMode）</summary>
    public void CbbTargetStatus2_DrawModeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.TargetStatus2_DrawMode = (TCustomDrawMode)(cbbTargetStatus2_DrawMode.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3163-3171 TFrmCustomMagic.chkTargetStatus2_CalcDirClick（表驱动：FCurrentClientConfig.TargetStatus2_CalcDir）</summary>
    public void ChkTargetStatus2_CalcDirClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.TargetStatus2_CalcDir = (byte)(chkTargetStatus2_CalcDir.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3196-3204 TFrmCustomMagic.seUseIntervalChange（表驱动：FCurrentServerConfig.UseInterval）</summary>
    public void SeUseIntervalChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.UseInterval = seUseInterval.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3216-3224 TFrmCustomMagic.edtFailMsgChange（表驱动：FCurrentServerConfig.FailMsg）</summary>
    public void EdtFailMsgChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.FailMsg = edtFailMsg.Text;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3225-3233 TFrmCustomMagic.edtSucceedMsgChange（表驱动：FCurrentServerConfig.SucceedMsg）</summary>
    public void EdtSucceedMsgChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.SucceedMsg = edtSucceedMsg.Text;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3234-3242 TFrmCustomMagic.edtCloseMsgChange（表驱动：FCurrentServerConfig.CloseMsg）</summary>
    public void EdtCloseMsgChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.CloseMsg = edtCloseMsg.Text;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3260-3268 TFrmCustomMagic.seAttackNearRangeChange（表驱动：FCurrentServerConfig.AttackNearRange）</summary>
    public void SeAttackNearRangeChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackNearRange = seAttackNearRange.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3269-3277 TFrmCustomMagic.seAttackGroupRangeChange（表驱动：FCurrentServerConfig.AttackGroupRange）</summary>
    public void SeAttackGroupRangeChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackGroupRange = seAttackGroupRange.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3287-3295 TFrmCustomMagic.cbbAttackPowerCalcChange（表驱动：FCurrentServerConfig.AttackPowerCalc）</summary>
    public void CbbAttackPowerCalcChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackPowerCalc = (TCustomAttackPowerCalc)(cbbAttackPowerCalc.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3304-3312 TFrmCustomMagic.seAttackPowerRateChange（表驱动：FCurrentServerConfig.AttackPowerRates[cbbAttackPowerLevel.ItemIndex]）</summary>
    public void SeAttackPowerRateChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackPowerRates[cbbAttackPowerLevel.ItemIndex] = seAttackPowerRate.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3313-3321 TFrmCustomMagic.seLineAttackAddPowerChange（表驱动：FCurrentServerConfig.AttackPowerLineAdd）</summary>
    public void SeLineAttackAddPowerChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackPowerLineAdd = seLineAttackAddPower.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3322-3330 TFrmCustomMagic.seAttackPowerUndeadAddChange（表驱动：FCurrentServerConfig.AttackPowerUndeadAdd）</summary>
    public void SeAttackPowerUndeadAddChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackPowerUndeadAdd = seAttackPowerUndeadAdd.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3331-3339 TFrmCustomMagic.cbbNeedItemChange（表驱动：FCurrentServerConfig.NeedItem）</summary>
    public void CbbNeedItemChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.NeedItem = (TMagicNeedItem)(cbbNeedItem.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3340-3348 TFrmCustomMagic.seNeedItemCountChange（表驱动：FCurrentServerConfig.NeedItemCount）</summary>
    public void SeNeedItemCountChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.NeedItemCount = seNeedItemCount.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3349-3357 TFrmCustomMagic.edtNeedItemCustomItemNameChange（表驱动：FCurrentServerConfig.NeedItemCustomItemName）</summary>
    public void EdtNeedItemCustomItemNameChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.NeedItemCustomItemName = edtNeedItemCustomItemName.Text;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3358-3366 TFrmCustomMagic.chkNeedItemUseBagItemClick（表驱动：FCurrentServerConfig.NeedItemUseBagItem）</summary>
    public void ChkNeedItemUseBagItemClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.NeedItemUseBagItem = chkNeedItemUseBagItem.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3367-3375 TFrmCustomMagic.seProtectTargetRangeChange（表驱动：FCurrentServerConfig.ProtectTargetRange）</summary>
    public void SeProtectTargetRangeChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.ProtectTargetRange = seProtectTargetRange.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3460-3468 TFrmCustomMagic.seAdditionaHP0Change（表驱动：FCurrentServerConfig.AdditionalHP0）</summary>
    public void SeAdditionaHP0Change()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AdditionalHP0 = seAdditionaHP0.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3469-3477 TFrmCustomMagic.chkseAdditionaHighLevel4Click（表驱动：FCurrentServerConfig.AdditionalHighLevel4）</summary>
    public void ChkseAdditionaHighLevel4Click()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AdditionalHighLevel4 = chkseAdditionaHighLevel4.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3478-3486 TFrmCustomMagic.cbbPushedType4Change（表驱动：FCurrentServerConfig.AdditionalPushedType4）</summary>
    public void CbbPushedType4Change()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AdditionalPushedType4 = (byte)(cbbPushedType4.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3487-3495 TFrmCustomMagic.chkAttackTargetStatusClick（表驱动：FCurrentServerConfig.AttackTargetStatus）</summary>
    public void ChkAttackTargetStatusClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTargetStatus = chkAttackTargetStatus.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3496-3504 TFrmCustomMagic.seAttackTargetStatusTimeChange（表驱动：FCurrentServerConfig.AttackTargetStatusTime）</summary>
    public void SeAttackTargetStatusTimeChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTargetStatusTime = (byte)(seAttackTargetStatusTime.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3505-3513 TFrmCustomMagic.cbbAttackTargetStatusTime_1Change（表驱动：FCurrentServerConfig.AttackTargetStatusTimeUnit）</summary>
    public void CbbAttackTargetStatusTime_1Change()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTargetStatusTimeUnit = (byte)(cbbAttackTargetStatusTime_1.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3514-3522 TFrmCustomMagic.seAttackTargetStatusTime_2Change（表驱动：FCurrentServerConfig.AttackTargetStatusTime2）</summary>
    public void SeAttackTargetStatusTime_2Change()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTargetStatusTime2 = (byte)(seAttackTargetStatusTime_2.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3523-3531 TFrmCustomMagic.seAttackTargetStatusDelayChange（表驱动：FCurrentServerConfig.AttackTargetStatusDelay）</summary>
    public void SeAttackTargetStatusDelayChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTargetStatusDelay = (ushort)(seAttackTargetStatusDelay.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3701-3709 TFrmCustomMagic.seCallMonstersRateChange（表驱动：FCurrentServerConfig.CallMonstersRate）</summary>
    public void SeCallMonstersRateChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.CallMonstersRate = seCallMonstersRate.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3710-3718 TFrmCustomMagic.seCallMonstersLevelChange（表驱动：FCurrentServerConfig.CallMonstersLevel）</summary>
    public void SeCallMonstersLevelChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.CallMonstersLevel = seCallMonstersLevel.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3719-3727 TFrmCustomMagic.chkAttackTeleportAfterDamageClick（表驱动：FCurrentServerConfig.AttackTeleportAfterDamage）</summary>
    public void ChkAttackTeleportAfterDamageClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTeleportAfterDamage = chkAttackTeleportAfterDamage.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3728-3736 TFrmCustomMagic.chkAttackTeleportAttackClick（表驱动：FCurrentServerConfig.AttackTeleportAttack）</summary>
    public void ChkAttackTeleportAttackClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTeleportAttack = chkAttackTeleportAttack.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3737-3745 TFrmCustomMagic.seAttackTeleportRateChange（表驱动：FCurrentServerConfig.AttackTeleportRate）</summary>
    public void SeAttackTeleportRateChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTeleportRate = seAttackTeleportRate.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3746-3754 TFrmCustomMagic.seAttackTeleportRushCountChange（表驱动：FCurrentServerConfig.AttackTeleportRushCount）</summary>
    public void SeAttackTeleportRushCountChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTeleportRushCount = seAttackTeleportRushCount.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3755-3763 TFrmCustomMagic.chkAttackTeleportRunHumClick（表驱动：FCurrentServerConfig.AttackTeleportRunHum）</summary>
    public void ChkAttackTeleportRunHumClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTeleportRunHum = chkAttackTeleportRunHum.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3764-3772 TFrmCustomMagic.chkAttackTeleportRunMonClick（表驱动：FCurrentServerConfig.AttackTeleportRunMon）</summary>
    public void ChkAttackTeleportRunMonClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTeleportRunMon = chkAttackTeleportRunMon.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3773-3781 TFrmCustomMagic.chkAttackTeleportRunNpcClick（表驱动：FCurrentServerConfig.AttackTeleportRunNpc）</summary>
    public void ChkAttackTeleportRunNpcClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTeleportRunNpc = chkAttackTeleportRunNpc.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3782-3790 TFrmCustomMagic.chkAttackTeleportRunGuardClick（表驱动：FCurrentServerConfig.AttackTeleportRunGuard）</summary>
    public void ChkAttackTeleportRunGuardClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTeleportRunGuard = chkAttackTeleportRunGuard.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3791-3799 TFrmCustomMagic.chkAttackTeleportRunObstacleClick（表驱动：FCurrentServerConfig.AttackTeleportRunObstacle）</summary>
    public void ChkAttackTeleportRunObstacleClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTeleportRunObstacle = chkAttackTeleportRunObstacle.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3800-3808 TFrmCustomMagic.chkAttackTeleportRushClick（表驱动：FCurrentServerConfig.AttackTeleportRush）</summary>
    public void ChkAttackTeleportRushClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTeleportRush = chkAttackTeleportRush.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3809-3817 TFrmCustomMagic.chkAttackTeleportWarDisHumRunClick（表驱动：FCurrentServerConfig.AttackTeleportWarDisHumRun）</summary>
    public void ChkAttackTeleportWarDisHumRunClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTeleportWarDisHumRun = chkAttackTeleportWarDisHumRun.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3818-3826 TFrmCustomMagic.chkAttackTeleportCannotRunItemClick（表驱动：FCurrentServerConfig.AttackTeleportCannotRunItem）</summary>
    public void ChkAttackTeleportCannotRunItemClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTeleportCannotRunItem = chkAttackTeleportCannotRunItem.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3827-3835 TFrmCustomMagic.chkEnabledCallMonsterClick（表驱动：FCurrentServerConfig.EnabledCallMonster）</summary>
    public void ChkEnabledCallMonsterClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.EnabledCallMonster = chkEnabledCallMonster.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3836-3844 TFrmCustomMagic.cbbClientFastMoveFileChange（表驱动：FCurrentClientConfig.FastMove_File）</summary>
    public void CbbClientFastMoveFileChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.FastMove_File = (short)(cbbClientFastMoveFile.ItemIndex - 1);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3845-3853 TFrmCustomMagic.seClientFastMoveStartIndexChange（表驱动：FCurrentClientConfig.FastMove_StartIndex）</summary>
    public void SeClientFastMoveStartIndexChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.FastMove_StartIndex = (ushort)(seClientFastMoveStartIndex.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3854-3862 TFrmCustomMagic.seClientFastMovePlayCountChange（表驱动：FCurrentClientConfig.FastMove_PlayCount）</summary>
    public void SeClientFastMovePlayCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.FastMove_PlayCount = (ushort)(seClientFastMovePlayCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3863-3871 TFrmCustomMagic.seClientFastMoveEmptyCountChange（表驱动：FCurrentClientConfig.FastMove_EmptyCount）</summary>
    public void SeClientFastMoveEmptyCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.FastMove_EmptyCount = (ushort)(seClientFastMoveEmptyCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3872-3880 TFrmCustomMagic.seClientFastMovePlayTimeChange（表驱动：FCurrentClientConfig.FastMove_PlayTime）</summary>
    public void SeClientFastMovePlayTimeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.FastMove_PlayTime = (ushort)(seClientFastMovePlayTime.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3881-3889 TFrmCustomMagic.cbbClientFastMoveDrawModeChange（表驱动：FCurrentClientConfig.FastMove_DrawMode）</summary>
    public void CbbClientFastMoveDrawModeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.FastMove_DrawMode = (TCustomDrawMode)(cbbClientFastMoveDrawMode.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3890-3898 TFrmCustomMagic.chkClientFastMoveCalcDirClick（表驱动：FCurrentClientConfig.FastMove_CalcDir）</summary>
    public void ChkClientFastMoveCalcDirClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.FastMove_CalcDir = (byte)(chkClientFastMoveCalcDir.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3899-3907 TFrmCustomMagic.chkClientFastMoveNoHitActionClick（表驱动：FCurrentClientConfig.FastMove_NoHitAction）</summary>
    public void ChkClientFastMoveNoHitActionClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.FastMove_NoHitAction = (byte)(chkClientFastMoveNoHitAction.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3908-3916 TFrmCustomMagic.seFastMoveLightRangeChange（表驱动：FCurrentClientConfig.FastMove_LightRange）</summary>
    public void SeFastMoveLightRangeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.FastMove_LightRange = (byte)(seFastMoveLightRange.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3935-3943 TFrmCustomMagic.cbbClientSelfKeepFileChange（表驱动：FCurrentClientConfig.SelfKeep_File）</summary>
    public void CbbClientSelfKeepFileChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.SelfKeep_File = (short)(cbbClientSelfKeepFile.ItemIndex - 1);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3944-3952 TFrmCustomMagic.seClientSelfKeepStartIndexChange（表驱动：FCurrentClientConfig.SelfKeep_StartIndex）</summary>
    public void SeClientSelfKeepStartIndexChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.SelfKeep_StartIndex = (ushort)(seClientSelfKeepStartIndex.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3953-3961 TFrmCustomMagic.seClientSelfKeepPlayCountChange（表驱动：FCurrentClientConfig.SelfKeep_PlayCount）</summary>
    public void SeClientSelfKeepPlayCountChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.SelfKeep_PlayCount = (ushort)(seClientSelfKeepPlayCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3962-3970 TFrmCustomMagic.seClientSelfKeepPlayTimeChange（表驱动：FCurrentClientConfig.SelfKeep_PlayTime）</summary>
    public void SeClientSelfKeepPlayTimeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.SelfKeep_PlayTime = (ushort)(seClientSelfKeepPlayTime.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3971-3979 TFrmCustomMagic.cbbClientSelfKeepDrawModeChange（表驱动：FCurrentClientConfig.SelfKeep_DrawMode）</summary>
    public void CbbClientSelfKeepDrawModeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.SelfKeep_DrawMode = (TCustomDrawMode)(cbbClientSelfKeepDrawMode.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3980-3988 TFrmCustomMagic.seClientSelfKeepTimeChange（表驱动：FCurrentClientConfig.SelfKeep_KeepTime）</summary>
    public void SeClientSelfKeepTimeChange()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.SelfKeep_KeepTime = (ushort)(seClientSelfKeepTime.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3989-3997 TFrmCustomMagic.seClientSelfKeepTime2Change（表驱动：FCurrentClientConfig.SelfKeep_KeepTime2）</summary>
    public void SeClientSelfKeepTime2Change()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.SelfKeep_KeepTime2 = seClientSelfKeepTime2.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:3998-4006 TFrmCustomMagic.seCallMonstersRoyaltySecChange（表驱动：FCurrentServerConfig.CallMonstersRoyaltySec）</summary>
    public void SeCallMonstersRoyaltySecChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.CallMonstersRoyaltySec = seCallMonstersRoyaltySec.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4019-4027 TFrmCustomMagic.chkSwitchModeNoCloseClick（表驱动：FCurrentCustomConfig.ClientBaseConfig.SwitchModeNoClose）</summary>
    public void ChkSwitchModeNoCloseClick()
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.ClientBaseConfig.SwitchModeNoClose = (byte)(chkSwitchModeNoClose.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4028-4036 TFrmCustomMagic.chkMagicAutoOpenClick（表驱动：FCurrentCustomConfig.ClientBaseConfig.MagicAutoOpen）</summary>
    public void ChkMagicAutoOpenClick()
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.ClientBaseConfig.MagicAutoOpen = (byte)(chkMagicAutoOpen.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4037-4045 TFrmCustomMagic.cbbMagicWarrNGOptionChange（表驱动：FCurrentCustomConfig.ClientBaseConfig.MagicWarrNGOption）</summary>
    public void CbbMagicWarrNGOptionChange()
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.ClientBaseConfig.MagicWarrNGOption = (TMagicWarrNGOption)(cbbMagicWarrNGOption.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4057-4065 TFrmCustomMagic.seClientActionStartIndexChange（表驱动：FCurrentCustomConfig.ClientBaseConfig.MagicActionStartIndex）</summary>
    public void SeClientActionStartIndexChange()
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.ClientBaseConfig.MagicActionStartIndex = seClientActionStartIndex.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4066-4074 TFrmCustomMagic.seClientActionPlayCountChange（表驱动：FCurrentCustomConfig.ClientBaseConfig.MagicActionPlayCount）</summary>
    public void SeClientActionPlayCountChange()
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.ClientBaseConfig.MagicActionPlayCount = seClientActionPlayCount.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4075-4083 TFrmCustomMagic.seClientActionEmptyCountChange（表驱动：FCurrentCustomConfig.ClientBaseConfig.MagicActionEmptyCount）</summary>
    public void SeClientActionEmptyCountChange()
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.ClientBaseConfig.MagicActionEmptyCount = seClientActionEmptyCount.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4084-4092 TFrmCustomMagic.chkClientActionContinueClick（表驱动：FCurrentCustomConfig.ClientBaseConfig.MagicActionContinue）</summary>
    public void ChkClientActionContinueClick()
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.ClientBaseConfig.MagicActionContinue = (byte)(chkClientActionContinue.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4093-4101 TFrmCustomMagic.chkEnableAntiMagicClick（表驱动：FCurrentServerConfig.EnableAntiMagic）</summary>
    public void ChkEnableAntiMagicClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.EnableAntiMagic = chkEnableAntiMagic.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4102-4110 TFrmCustomMagic.chkProtectAddHPSlowClick（表驱动：FCurrentServerConfig.ProtectAddHPSlow）</summary>
    public void ChkProtectAddHPSlowClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.ProtectAddHPSlow = chkProtectAddHPSlow.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4111-4119 TFrmCustomMagic.seProtectAddHPSlowCountChange（表驱动：FCurrentServerConfig.ProtectAddHpSlowCount）</summary>
    public void SeProtectAddHPSlowCountChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.ProtectAddHpSlowCount = (ushort)(seProtectAddHPSlowCount.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4120-4128 TFrmCustomMagic.seAttackDelayTimeChange（表驱动：FCurrentServerConfig.AttackDelayTime）</summary>
    public void SeAttackDelayTimeChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackDelayTime = seAttackDelayTime.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4129-4137 TFrmCustomMagic.chkEnableHitPointClick（表驱动：FCurrentServerConfig.EnableHitPoint）</summary>
    public void ChkEnableHitPointClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.EnableHitPoint = chkEnableHitPoint.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4138-4146 TFrmCustomMagic.chkProtectTargetStatusClick（表驱动：FCurrentServerConfig.ProtectTargetStatus）</summary>
    public void ChkProtectTargetStatusClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.ProtectTargetStatus = chkProtectTargetStatus.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4147-4155 TFrmCustomMagic.seProtectTargetStatusTimeChange（表驱动：FCurrentServerConfig.ProtectTargetStatusTime）</summary>
    public void SeProtectTargetStatusTimeChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.ProtectTargetStatusTime = (byte)(seProtectTargetStatusTime.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4156-4164 TFrmCustomMagic.cbbProtectTargetStatusTime_1Change（表驱动：FCurrentServerConfig.ProtectTargetStatusTimeUnit）</summary>
    public void CbbProtectTargetStatusTime_1Change()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.ProtectTargetStatusTimeUnit = (byte)(cbbProtectTargetStatusTime_1.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4165-4173 TFrmCustomMagic.seProtectTargetStatusTime_2Change（表驱动：FCurrentServerConfig.ProtectTargetStatusTime2）</summary>
    public void SeProtectTargetStatusTime_2Change()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.ProtectTargetStatusTime2 = (byte)(seProtectTargetStatusTime_2.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4174-4182 TFrmCustomMagic.seProtectTargetStatusTimeDelayChange（表驱动：FCurrentServerConfig.ProtectTargetStatusDelay）</summary>
    public void SeProtectTargetStatusTimeDelayChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.ProtectTargetStatusDelay = (ushort)(seProtectTargetStatusTimeDelay.Value);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4209-4217 TFrmCustomMagic.edtCheckVarNameChange（表驱动：FCurrentServerConfig.CheckVarName）</summary>
    public void EdtCheckVarNameChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.CheckVarName = edtCheckVarName.Text;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4218-4226 TFrmCustomMagic.cbbCheckVarTypeChange（表驱动：FCurrentServerConfig.CheckVarType）</summary>
    public void CbbCheckVarTypeChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.CheckVarType = (TCheckVarType)(cbbCheckVarType.ItemIndex);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4227-4235 TFrmCustomMagic.seCheckVarValueChange（表驱动：FCurrentServerConfig.CheckVarValue）</summary>
    public void SeCheckVarValueChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.CheckVarValue = seCheckVarValue.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4236-4244 TFrmCustomMagic.seCheckVarAddChange（表驱动：FCurrentServerConfig.CheckVarAdd）</summary>
    public void SeCheckVarAddChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.CheckVarAdd = seCheckVarAdd.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4245-4253 TFrmCustomMagic.chkNoTeleportNoAttackClick（表驱动：FCurrentServerConfig.IsNoTeleportNoAttack）</summary>
    public void ChkNoTeleportNoAttackClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.IsNoTeleportNoAttack = chkNoTeleportNoAttack.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4254-4262 TFrmCustomMagic.chkSelf_SyncHumActionClick（表驱动：FCurrentClientConfig.Self_SyncHumAction）</summary>
    public void ChkSelf_SyncHumActionClick()
    {
        if (FCurrentClientConfig != null)
        {
            FCurrentClientConfig.Value.Self_SyncHumAction = (byte)(chkSelf_SyncHumAction.Checked ? 1 : 0);
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4263-4271 TFrmCustomMagic.chkAttackUseNGClick（表驱动：FCurrentServerConfig.IsAttackUseNG）</summary>
    public void ChkAttackUseNGClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.IsAttackUseNG = chkAttackUseNG.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4272-4280 TFrmCustomMagic.chkAttackNoChangeDirClick（表驱动：FCurrentServerConfig.NoChangeDir）</summary>
    public void ChkAttackNoChangeDirClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.NoChangeDir = chkAttackNoChangeDir.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 uFrmCustomMagic.pas:4281-4289 TFrmCustomMagic.chkDisableInSafeZoneClick（表驱动：FCurrentServerConfig.DisableInSafeZone）</summary>
    public void ChkDisableInSafeZoneClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.DisableInSafeZone = chkDisableInSafeZone.Checked;
            SetConfigChanged();
        }
    }

}

// ============================================================================
// uFrmCustomMagic.pas（Source\M2Engine\Forms\uFrmCustomMagic.pas，4800 行，GBK）1:1 移植
// 车道 p5-m2-custommagic ｜ 命名空间 GXX.M2Server.Forms.CustomMagic
//
// 本文件 = 非"单字段写回"形状的**其余全部处理器**：
//   * 27 个特殊事件处理器（Tag 驱动的数组族、布局联动、整段注释掉的死代码）
//   * 5 棵树（vstAttackDecAttr / vstDecElement / vstProtectedAddAttr / vstAddElement）的
//     Checked / NodeClick / AfterCellPaint / GetText / Editing / CreateEditor 与 4 个 WM_ 消息处理器
//   * btnSaveClick / btnMakeConfigDataClick / chkSendCustomMagicConfigClick / btnCopyConfigClick
//   * RebuildCustomMagicListText（组客户端配置包并压缩）
//
// 覆盖行号（Delphi）见每个方法上方注释。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms.CustomMagic;

public partial class TFrmCustomMagic
{
    // ========================================================================
    // cbbClientLevelChange（原文 :2378-2509）—— 强化段切换：把 ClientConfigs[level] 全量刷到控件
    // ========================================================================

    /// <summary>原文 <c>cbbClientLevelChange</c>（:2378-2509）。</summary>
    public void CbbClientLevelChange()
    {
        FCurrentClientConfig = null;
        if (FCurrentCustomConfig == null)
            return;                                   // 原文 :2384-2385

        bool oldIsConfigCanSave = FIsConfigChanged;
        int index = cbbClientLevel.ItemIndex;
        // 原文 :2389 Low/High(TMagicPlusLevel) 即 0..3
        if (index >= 0 && index <= CustomMagicUtilsConst.MagicPlusLevelCount - 1)
        {
            bool oldChanged = FCurrentCustomConfig.IsChanged;

            FCurrentClientConfig = FCurrentCustomConfig.ClientConfigs[index];   // 原文 :2394 @ClientConfigs[...]
            // grpClientAttackConfigs.Caption := AttackConfigNames[Index] + '的攻击效果配置';   ← 原文 :2395 注释掉

            var c = FCurrentClientConfig.Value;

            cbbClientIconFile.ItemIndex = (int)c.Icon_File + 1;
            seClientIconIndex.Value = c.Icon_Index;

            edtSound1.Text = c.Sounds[(int)TMagicSoundType.cmstManWarr].Value;
            edtSound2.Text = c.Sounds[(int)TMagicSoundType.cmstWomanWarr].Value;
            edtSound3.Text = c.Sounds[(int)TMagicSoundType.cmstUseMagic].Value;
            edtSound4.Text = c.Sounds[(int)TMagicSoundType.cmstMagicFly].Value;
            edtSound5.Text = c.Sounds[(int)TMagicSoundType.custMagicExplosion].Value;
            edtSound6.Text = c.Sounds[(int)TMagicSoundType.custMagicFail].Value;

            cbbClientFlyFile.ItemIndex = (int)c.Fly_File + 1;
            seClientFlyStartIndex.Value = c.Fly_StartIndex;
            seClientFlyPlayCount.Value = c.Fly_PlayCount;
            seClientFlyEmptyCount.Value = c.Fly_EmptyCount;
            seClientFlyPlayTime.Value = c.Fly_PlayTime;
            cbbClientFlyDrawMode.ItemIndex = (int)c.Fly_DrawMode;
            cbbClientFlyDirCount.ItemIndex = (int)c.Fly_DirCount;
            chkClientFlyCalcDir.Checked = c.Fly_CalcDir != 0;
            chkClientFlyFireGunMode.Checked = c.Fly_FireGunMode != 0;
            seClientFlyLightRange.Value = c.Fly_LightRange;

            cbbClientFlyEffFile.ItemIndex = (int)c.FlyEff_File + 1;
            seClientFlyEffStartIndex.Value = c.FlyEff_StartIndex;
            cbbClientFlyEffDrawMode.ItemIndex = (int)c.FlyEff_DrawMode;

            cbbClientSelfFile.ItemIndex = (int)c.Self_File + 1;
            seClientSelfStartIndex.Value = c.Self_StartIndex;
            chkSelf_SyncHumAction.Checked = c.Self_SyncHumAction != 0;
            seClientSelfPlayCount.Value = c.Self_PlayCount;
            seClientSelfEmptyCount.Value = c.Self_EmptyCount;
            seClientSelfPlayTime.Value = c.Self_PlayTime;
            // cbbClientSelfPlayMode.ItemIndex := Integer(FCurrentClientConfig.Self_PlayMode);   ← 原文 :2428 注释掉
            cbbClientSelfDrawOrder.ItemIndex = (int)c.Self_DrawOrder;
            cbbClientSelfDrawMode.ItemIndex = (int)c.Self_DrawMode;
            cbbClientSelfDirCalcType.ItemIndex = (int)c.Self_DirCalcType;
            cbbClientSelfDirCount.ItemIndex = (int)c.Self_DirCount;
            chkClientSelfPlayDelayAction.Checked = c.Self_PlayDelayAction != 0;
            seClientSelfLightRange.Value = c.Self_LightRange;
            chkClientSelfPlayFailNoDraw.Checked = c.Self_PlayFailNoDraw != 0;

            cbbClientSelfKeepFile.ItemIndex = (int)c.SelfKeep_File + 1;
            seClientSelfKeepStartIndex.Value = c.SelfKeep_StartIndex;
            seClientSelfKeepPlayCount.Value = c.SelfKeep_PlayCount;
            seClientSelfKeepPlayTime.Value = c.SelfKeep_PlayTime;
            cbbClientSelfKeepDrawMode.ItemIndex = (int)c.SelfKeep_DrawMode;
            seClientSelfKeepTime.Value = c.SelfKeep_KeepTime;
            seClientSelfKeepTime2.Value = c.SelfKeep_KeepTime2;

            cbbClientFastMoveFile.ItemIndex = (int)c.FastMove_File + 1;
            seClientFastMoveStartIndex.Value = c.FastMove_StartIndex;
            seClientFastMovePlayCount.Value = c.FastMove_PlayCount;
            seClientFastMoveEmptyCount.Value = c.FastMove_EmptyCount;
            seClientFastMovePlayTime.Value = c.FastMove_PlayTime;
            cbbClientFastMoveDrawMode.ItemIndex = (int)c.FastMove_DrawMode;
            chkClientFastMoveCalcDir.Checked = c.FastMove_CalcDir != 0;
            chkClientFastMoveNoHitAction.Checked = c.FastMove_NoHitAction != 0;
            seFastMoveLightRange.Value = c.FastMove_LightRange;

            cbbClientPreTargetFile.ItemIndex = (int)c.PreTarget_File + 1;
            seClientPreTargetStartIndex.Value = c.PreTarget_StartIndex;
            seClientPreTargetStartIndex2.Value = c.PreTarget_StartIndex2;
            seClientPreTargetPlayCount.Value = c.PreTarget_PlayCount;
            seClientPreTargetEmptyCount.Value = c.PreTarget_EmptyCount;
            seClientPreTargetPlayTime.Value = c.PreTarget_PlayTime;
            cbbClientPreTargetDrawMode.ItemIndex = (int)c.PreTarget_DrawMode;
            cbbClientPreTargetDrawMode2.ItemIndex = (int)c.PreTarget_DrawMode2;
            chkClientPreTargetCalcDir.Checked = c.PreTarget_CalcDir != 0;
            chkClientPreTargetLockDraw.Checked = c.PreTarget_LockDraw != 0;
            seClientPreTargetLightRange.Value = c.PreTarget_LightRange;

            cbbClientTargetFile.ItemIndex = (int)c.Target_File + 1;
            seClientTargetStartIndex.Value = c.Target_StartIndex;
            seClientTargetStartIndex2.Value = c.Target_StartIndex2;
            seClientTargetPlayCount.Value = c.Target_PlayCount;
            seClientTargetPlayTime.Value = c.Target_PlayTime;
            cbbClientTargetDrawMode.ItemIndex = (int)c.Target_DrawMode;
            cbbClientTargetDrawMode2.ItemIndex = (int)c.Target_DrawMode2;
            chkClientTargetMultiPlay.Checked = c.Target_MultiPlay != 0;
            chkClientTargetLockDraw.Checked = c.Target_LockDraw != 0;
            seClientTargetLightRange.Value = c.Target_LightRange;

            chkClientTargetKeepPlay.Checked = c.Target_KeepPlay != 0;
            seClientTargetKeepTime.Value = c.Target_KeepTime;
            seClientTargetKeepTime2.Value = c.Target_KeepTime2;
            seClientTargetKeepAttackRange.Value = c.Target_KeepAttackRange;
            chkClientTargetKeepMultiPlay.Checked = c.Target_KeepMultiPlay != 0;
            seClientTargetKeepAttackInterval.Value = c.Target_KeepAttackInterval;
            seTargetKeepLightRange.Value = c.Target_KeepLightRange;

            cbbTargetStatus1_File.ItemIndex = (int)c.TargetStatus1_File + 1;
            seTargetStatus1_StartIndex.Value = c.TargetStatus1_StartIndex;
            seTargetStatus1_PlayCount.Value = c.TargetStatus1_PlayCount;
            seTargetStatus1_EmptyCount.Value = c.TargetStatus1_EmptyCount;
            // seTargetStatus1_PlayTime.Value := FCurrentClientConfig.TargetStatus1_PlayTime;   ← 原文 :2490 注释掉
            cbbTargetStatus1_DrawMode.ItemIndex = (int)c.TargetStatus1_DrawMode;
            chkTargetStatus1_CalcDir.Checked = c.TargetStatus1_CalcDir != 0;

            cbbTargetStatus2_File.ItemIndex = (int)c.TargetStatus2_File + 1;
            seTargetStatus2_StartIndex.Value = c.TargetStatus2_StartIndex;
            seTargetStatus2_PlayCount.Value = c.TargetStatus2_PlayCount;
            seTargetStatus2_EmptyCount.Value = c.TargetStatus2_EmptyCount;
            // seTargetStatus2_PlayTime.Value := FCurrentClientConfig.TargetStatus2_PlayTime;   ← 原文 :2498 注释掉
            cbbTargetStatus2_DrawMode.ItemIndex = (int)c.TargetStatus2_DrawMode;
            chkTargetStatus2_CalcDir.Checked = c.TargetStatus2_CalcDir != 0;

            SetConfigChanged(oldChanged);

            FIsConfigChanged = oldIsConfigCanSave;
            if (!FIsConfigChanged)
                btnSave.Enabled = false;
        }
    }

    // ========================================================================
    // 其余特殊处理器（原文 2519-2532、3082-3090、3145-3153、3172-3215、3243-3303、
    //                 3376-3459、3672-3700、3917-3934、4007-4056、4093-4208、4724-4799）
    // ========================================================================

    /// <summary>原文 <c>cbbClientActionTypeChange</c>（:2519-2532）。</summary>
    public void CbbClientActionTypeChange()
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.ClientBaseConfig.MagicActionType = (TMagicActionType)cbbClientActionType.ItemIndex;

            seClientActionStartIndex.Enabled = FCurrentCustomConfig.ClientBaseConfig.MagicActionType == TMagicActionType.matCustom;
            seClientActionPlayCount.Enabled = seClientActionStartIndex.Enabled;
            seClientActionEmptyCount.Enabled = seClientActionStartIndex.Enabled;
            // 原文 :2528 chkClientActionContinue.Enabled := seClientActionStartIndex.Enabled { and (not chkMagicSwitchMode.Enabled) };
            chkClientActionContinue.Enabled = seClientActionStartIndex.Enabled;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>seTargetStatus1_PlayTimeChange</c>（:3082-3090）：**空体**（原文只有 <c>if ... then begin end;</c>）。</summary>
    public void SeTargetStatus1_PlayTimeChange()
    {
        if (FCurrentClientConfig != null)
        {
        }
    }

    /// <summary>原文 <c>seTargetStatus2_PlayTimeChange</c>（:3145-3153）：**空体**。</summary>
    public void SeTargetStatus2_PlayTimeChange()
    {
        if (FCurrentClientConfig != null)
        {
        }
    }

    /// <summary>原文 <c>cbbOperateModeChange</c>（:3172-3195）：写回 + 6 处 Visible + 换页。</summary>
    public void CbbOperateModeChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.OperateMode = (TCustomOperateMode)cbbOperateMode.ItemIndex;
            SetConfigChanged();

            lblAttackDelay.Visible = FCurrentServerConfig.OperateMode == TCustomOperateMode.momAttack;
            seAttackDelayTime.Visible = FCurrentServerConfig.OperateMode == TCustomOperateMode.momAttack;
            lblAttackDelayTime.Visible = FCurrentServerConfig.OperateMode == TCustomOperateMode.momAttack;
            lblProtectTargetRangeTitle.Visible = FCurrentServerConfig.OperateMode != TCustomOperateMode.momAttack;
            seProtectTargetRange.Visible = FCurrentServerConfig.OperateMode != TCustomOperateMode.momAttack;
            lblProtectTargetRangeValue.Visible = FCurrentServerConfig.OperateMode != TCustomOperateMode.momAttack;

            if (FCurrentServerConfig.OperateMode == TCustomOperateMode.momAttack)
            {
                pgcMagicType.ActivePage = tsMagicAttack;
            }
            else
            {
                pgcMagicType.ActivePage = tsMagicProtected;
            }
        }
    }

    /// <summary>原文 <c>chkFailNoShowEffClick</c>（:3205-3215）：**整段处于 <c>{ }</c> 注释内 → 死代码**。</summary>
    public void ChkFailNoShowEffClick()
    {
        // 原文如此（uFrmCustomMagic.pas:3206-3214）：
        // {
        //   if FCurrentServerConfig <> nil then
        //   begin
        //     FCurrentServerConfig.FailNoShowEff := chkFailNoShowEff.Checked;
        //     SetConfigChanged();
        //   end;
        // }
    }

    /// <summary>原文 <c>cbbAttackTargetChange</c>（:3243-3259）。</summary>
    public void CbbAttackTargetChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.AttackTarget = (TCustomAttackTarget)cbbAttackTarget.ItemIndex;
            SetConfigChanged();

            lblLineAttackAddPower.Visible = IsLineAttack(FCurrentServerConfig.AttackTarget);
            seLineAttackAddPower.Visible = lblLineAttackAddPower.Visible;
            lblLineAttackAddPowerPerc.Visible = lblLineAttackAddPower.Visible;
            seAttackLineWidth.Visible = cbbAttackTarget.ItemIndex == 2;   // 原文 :3250 手写常量 2（= matLine）
            lblAttackWidth.Visible = seAttackLineWidth.Visible;
            lblH_AttackWidth.Visible = seAttackLineWidth.Visible;
        }
    }

    /// <summary>
    /// 原文 <c>seAttackLineWidthChange</c>（:3278-3286）。
    /// **原文缺陷（guard/target 不一致）**：判定用 <c>FCurrentServerConfig &lt;&gt; nil</c>，
    /// 写回却走 <c>FCurrentCustomConfig.ServerConfig</c> —— 两者在原文里恒同时非 nil，
    /// 故未暴露；此处**照抄原文**（保留该冗余）并由测试锁定。
    /// </summary>
    public void SeAttackLineWidthChange()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentCustomConfig!.ServerConfig.AttackLineWidth = seAttackLineWidth.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>cbbAttackPowerLevelChange</c>（:3296-3303）：**没有 SetConfigChanged()**。</summary>
    public void CbbAttackPowerLevelChange()
    {
        if (FCurrentServerConfig != null)
        {
            seAttackPowerRate.Value = FCurrentServerConfig.AttackPowerRates[cbbAttackPowerLevel.ItemIndex];
        }
    }

    // ---- 附加伤害 1..11 槽（原文 :3376-3459）：Sender.Tag 即槽下标 ----

    /// <summary>原文 <c>chkAdditional0Click</c>（:3376-3389）。</summary>
    public void ChkAdditional0Click(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TCheckBoxSeam chk)
        {
            // 原文：WinCtrl := Sender as TWinControl; if (WinCtrl.Tag >= Low(Additionals)) and (<= High) then ...
            if (tag >= 0 && tag <= 10)   // Low = 0、High = 10（array[0..10]）
                FCurrentServerConfig.Additionals[tag].Checked = chk.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>seAdditionalRate0Change</c>（:3390-3403）→ <c>Additionals[Tag].Rate</c>。</summary>
    public void SeAdditionalRate0Change(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TSpinEditExSeam se)
        {
            if (tag >= 0 && tag <= 10)
                FCurrentServerConfig.Additionals[tag].Rate = (byte)se.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>seAdditionalRate0_2Change</c>（:3404-3417）→ <c>Additionals[Tag].Rate2</c>。</summary>
    public void SeAdditionalRate0_2Change(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TSpinEditExSeam se)
        {
            if (tag >= 0 && tag <= 10)
                FCurrentServerConfig.Additionals[tag].Rate2 = (byte)se.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>seAdditionalTime0Change</c>（:3418-3431）→ <c>Additionals[Tag].Time</c>。</summary>
    public void SeAdditionalTime0Change(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TSpinEditExSeam se)
        {
            if (tag >= 0 && tag <= 10)
                FCurrentServerConfig.Additionals[tag].Time = (byte)se.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>cbbAdditionalTime0_1Change</c>（:3432-3445）→ <c>Additionals[Tag].TimeUnit</c>。</summary>
    public void CbbAdditionalTime0_1Change(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TComboBoxSeam cbb)
        {
            if (tag >= 0 && tag <= 10)
                FCurrentServerConfig.Additionals[tag].TimeUnit = (byte)cbb.ItemIndex;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>seAdditionalTime0_2Change</c>（:3446-3459）→ <c>Additionals[Tag].Time2</c>。</summary>
    public void SeAdditionalTime0_2Change(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TSpinEditExSeam se)
        {
            if (tag >= 0 && tag <= 10)
                FCurrentServerConfig.Additionals[tag].Time2 = (byte)se.Value;
            SetConfigChanged();
        }
    }

    // ---- 召唤怪物（原文 :3672-3700）：Sender.Tag 即槽下标 ----

    /// <summary>原文 <c>edtCallMonster1Change</c>（:3672-3685）。</summary>
    public void EdtCallMonster1Change(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TEditSeam edt)
        {
            if (tag >= TMagicServerConfig.CallMonstersLow && tag <= TMagicServerConfig.CallMonstersHigh)
                FCurrentServerConfig.SetCallMonster(tag, edt.Text);

            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>seCallMonsterNum1Change</c>（:3686-3700）。</summary>
    public void SeCallMonsterNum1Change(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TSpinEditExSeam se)
        {
            if (tag >= TMagicServerConfig.CallMonstersLow && tag <= TMagicServerConfig.CallMonstersHigh)
                FCurrentServerConfig.CallMonsterNums[tag] = se.Value;

            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>edtSound1Change</c>（:3917-3934）：Tag 即 <c>TMagicSoundType</c> 下标。</summary>
    public void EdtSound1Change(object? sender, int tag)
    {
        if (FCurrentClientConfig == null)
            return;                                   // 原文 :3920-3921

        if (sender is TEditSeam edt)
        {
            if (tag >= 0 && tag <= 5)                 // Low/High(TMagicSoundType)
            {
                FCurrentClientConfig.Value.Sounds[tag].Value = edt.Text;
                SetConfigChanged();
            }
        }
    }

    /// <summary>原文 <c>cbbMagicSwitchModeChange</c>（:4007-4018）。</summary>
    public void CbbMagicSwitchModeChange()
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode = (TMagicSwitchMode)cbbMagicSwitchMode.ItemIndex;
            SetConfigChanged();
            chkSwitchModeNoClose.Enabled = FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode == TMagicSwitchMode.msmSwitch;
            chkMagicAutoOpen.Enabled = FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode == TMagicSwitchMode.msmSwitch;
        }
    }

    /// <summary>原文 <c>btnCopyConfigClick</c>（:4046-4056）：整段 ClientConfigs 复制。</summary>
    public void BtnCopyConfigClick()
    {
        if (FCurrentCustomConfig != null)
        {
            var destLevel = new TMagicPlusLevelBox();
            if (ShowCustomMagicCopySettingHandler((TMagicPlusLevel)cbbClientLevel.ItemIndex, destLevel))
            {
                FCurrentCustomConfig.ClientConfigs[(int)destLevel.Value].Value =
                    FCurrentCustomConfig.ClientConfigs[cbbClientLevel.ItemIndex].Value;
                SetConfigChanged();
            }
        }
    }

    /// <summary>ShowCustomMagicCopySetting 接缝（uFrmCustomMagicCopySetting.pas；C# 见 Forms\CustomMagicCopySettingForm.cs）。</summary>
    public static Func<TMagicPlusLevel, TMagicPlusLevelBox, bool> ShowCustomMagicCopySettingHandler = (_, _) => false;

    /// <summary>var 形参 <c>DestLevel: TMagicPlusLevel</c> 的托管替身（Delphi 的 var 参数）。</summary>
    public sealed class TMagicPlusLevelBox
    {
        /// <summary>值。</summary>
        public TMagicPlusLevel Value;
    }

    /// <summary>原文 <c>chkCheckVarValueClick</c>（:4183-4208）：写回 + 15 处 Visible 联动。</summary>
    public void ChkCheckVarValueClick()
    {
        if (FCurrentServerConfig != null)
        {
            FCurrentServerConfig.IsCheckVarValue = chkCheckVarValue.Checked;
            SetConfigChanged();

            lblNeedItem.Visible = !chkCheckVarValue.Checked;
            cbbNeedItem.Visible = lblNeedItem.Visible;
            lblNeedItemCount.Visible = lblNeedItem.Visible;
            seNeedItemCount.Visible = lblNeedItem.Visible;
            lblNeedItemCustomItemName.Visible = lblNeedItem.Visible;
            edtNeedItemCustomItemName.Visible = lblNeedItem.Visible;
            chkNeedItemUseBagItem.Visible = lblNeedItem.Visible;

            lblCheckVarName.Visible = chkCheckVarValue.Checked;
            edtCheckVarName.Visible = lblCheckVarName.Visible;
            lblCheckVarType.Visible = lblCheckVarName.Visible;
            cbbCheckVarType.Visible = lblCheckVarName.Visible;
            lblCheckVarValue.Visible = lblCheckVarName.Visible;
            seCheckVarValue.Visible = lblCheckVarName.Visible;
            lblCheckVarAdd.Visible = lblCheckVarName.Visible;
            seCheckVarAdd.Visible = lblCheckVarName.Visible;
        }
    }

    // ---- 破防 6 组（原文 :4724-4799）：Sender.Tag 即 TBreakDefenseType 下标 ----

    /// <summary>原文 <c>chkMagicACHumClick</c>（:4724-4738）。</summary>
    public void ChkMagicACHumClick(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TCheckBoxSeam chk)
        {
            // 原文 WinCtrl: TWinControl（不是 Sender as TCheckBox）
            if (tag >= 0 && tag <= CustomMagicUtilsConst.BreakDefenseTypeCount - 1)
                FCurrentServerConfig.AttackBreakDefense[tag].IsChecked = chk.Checked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>seMagicACHumRateChange</c>（:4739-4753）：Sender 当作 <c>TSpinEdit</c>（VCL 标准）。</summary>
    public void SeMagicACHumRateChange(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TSpinEditSeam se)
        {
            if (tag >= 0 && tag <= CustomMagicUtilsConst.BreakDefenseTypeCount - 1)
                FCurrentServerConfig.AttackBreakDefense[tag].Rate = se.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>seMagicACHumRateAddChange</c>（:4754-4768）。</summary>
    public void SeMagicACHumRateAddChange(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TSpinEditSeam se)
        {
            if (tag >= 0 && tag <= CustomMagicUtilsConst.BreakDefenseTypeCount - 1)
                FCurrentServerConfig.AttackBreakDefense[tag].RateAdd = se.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>seMagicACHumValueChange</c>（:4769-4783）。</summary>
    public void SeMagicACHumValueChange(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TSpinEditSeam se)
        {
            if (tag >= 0 && tag <= CustomMagicUtilsConst.BreakDefenseTypeCount - 1)
                FCurrentServerConfig.AttackBreakDefense[tag].Value = se.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>seMagicACHumValueAddChange</c>（:4784-4799）。</summary>
    public void SeMagicACHumValueAddChange(object? sender, int tag)
    {
        if (FCurrentServerConfig != null && sender is TSpinEditSeam se)
        {
            if (tag >= 0 && tag <= CustomMagicUtilsConst.BreakDefenseTypeCount - 1)
                FCurrentServerConfig.AttackBreakDefense[tag].ValueAdd = se.Value;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>chkSendCustomMagicConfigClick</c>（:3666-3671）：直接写 g_Config + ini（不碰当前配置）。</summary>
    public void ChkSendCustomMagicConfigClick()
    {
        CustomMagicFormGlobals.boSendCustomMagicConfig = chkSendCustomMagicConfig.Checked;
        CustomMagicFormGlobals.ConfigWriteBool("Setup", "SendCustomMagicConfig",
            CustomMagicFormGlobals.boSendCustomMagicConfig);
    }

    // ========================================================================
    // 按钮与组包（原文 :3532-3671）
    // ========================================================================

    /// <summary>原文 <c>btnSaveClick</c>（:3601-3627）。</summary>
    public void BtnSaveClick()
    {
        HostOf("vstAttackDecAttr").EndEditNode();

        var node = VstCustomMagic.GetFirst();
        while (node != null)
        {
            var configNodeData = (TMagicConfigNodeData?)VstCustomMagic.GetNodeData(node);
            if (configNodeData != null && configNodeData.Config != null && configNodeData.Config.IsChanged)
                CustomMagicConfigDefaults.SaveToIniFile?.Invoke(configNodeData.Config);
            node = VstCustomMagic.GetNext(node);
        }

        VstCustomMagic.Invalidate();
        RebuildCustomMagicListText();
        CustomMagicFormGlobals.ResetMagicCDList();
        FIsConfigChanged = false;
        btnSave.Enabled = false;
        CustomMagicFormGlobals.SendServerConfig();
    }

    /// <summary>原文 <c>btnMakeConfigDataClick</c>（:3628-3665）。</summary>
    public void BtnMakeConfigDataClick()
    {
        if (CustomMagicFormGlobals.sCustomMagicClientConfigFileName != "")
            dlgSaveMagics.FileName = CustomMagicFormGlobals.sCustomMagicClientConfigFileName;

        if (!dlgSaveMagics.Execute())
        {
            CustomMagicFormGlobals.SetCurrentDirectory(
                CustomMagicFormGlobals.ExtractFileDir(CustomMagicFormGlobals.ApplicationExeName));
            return;                                   // 原文 :3634 Exit
        }

        CustomMagicFormGlobals.SetCurrentDirectory(
            CustomMagicFormGlobals.ExtractFileDir(CustomMagicFormGlobals.ApplicationExeName));

        string fileName = dlgSaveMagics.FileName;
        fileName = CustomMagicFormGlobals.ChangeFileExt(fileName, ".dat");
        CustomMagicFormGlobals.sCustomMagicClientConfigFileName = fileName;
        CustomMagicFormGlobals.ConfigWriteString("Setup", "CustomMagicClientConfigFileName",
            CustomMagicFormGlobals.sCustomMagicClientConfigFileName);

        LockCustomMagicListIfMultiThread(5);
        try
        {
            if (CustomMagicFormGlobals.SaveCustomMagicClientConfigs != null)
                CustomMagicFormGlobals.SaveCustomMagicClientConfigs(CustomMagicFormGlobals.m_CustomMagicList, fileName);
            else
                SaveCustomMagicClientConfigsFallback(CustomMagicFormGlobals.m_CustomMagicList, fileName);
        }
        finally
        {
            UnlockCustomMagicListIfMultiThread();
        }

        CustomMagicMessageBoxSeam.ShowMessage("已经生成自定义技能登录器配置文件");
    }

    /// <summary>
    /// 接缝：uCustomMagicUtils.pas:247-317 <c>SaveCustomMagicClientConfigs</c> 未移植，
    /// 默认实现只做序列化（不落盘），供测试观察；正式实现由 uCustomMagicUtils 车道接管。
    /// </summary>
    private static void SaveCustomMagicClientConfigsFallback(List<TCustomMagicConfig> configs, string fileName)
        => LastSavedConfigBytes = BuildClientConfigBytes(configs);

    /// <summary>最近一次序列化的客户端配置字节（接缝观测位）。</summary>
    public static byte[]? LastSavedConfigBytes;

    /// <summary>
    /// 原文 <c>RebuildCustomMagicListText</c>（:3532-3600）：
    /// 把 <c>UserEngine.m_CustomMagicList</c> 逐项打包成 <c>TClientCustomMagicConfig</c> 定长记录，
    /// 再 zLib 压缩并算 CRC 存入三个全局量。
    /// </summary>
    public void RebuildCustomMagicListText()
    {
        LockCustomMagicListIfMultiThread(4);
        try
        {
            int inBytes = CustomMagicFormGlobals.m_CustomMagicList.Count * SizeOfClientConfig();
            byte[] buffer = new byte[inBytes + 1];    // 原文 GetMem(InBuf, InBytes + 1)
            try
            {
                int offset = 0;
                for (int i = 0; i < CustomMagicFormGlobals.m_CustomMagicList.Count; i++)
                {
                    TCustomMagicConfig customMagicConfig = CustomMagicFormGlobals.m_CustomMagicList[i];

                    var clientConfig = default(TClientCustomMagicConfig);
                    clientConfig.wMagicId = customMagicConfig.MagicID;
                    clientConfig.MagicBaseConfig = customMagicConfig.ClientBaseConfig;
                    for (int level = 0; level < CustomMagicUtilsConst.MagicPlusLevelCount; level++)
                        clientConfig.MagicConfigs[level] = customMagicConfig.ClientConfigs[level].Value;
                    clientConfig.boIsMagicWarr = (byte)(customMagicConfig.IsMagicWarr ? 1 : 0);

                    clientConfig.btNearAttackRange = customMagicConfig.IsMagicWarr
                        ? (byte)customMagicConfig.ServerConfig.AttackNearRange
                        : (byte)1;

                    clientConfig.IsAttackUseNG = (byte)(customMagicConfig.ServerConfig.IsAttackUseNG ? 1 : 0);
                    clientConfig.NoChangeDir = (byte)(customMagicConfig.ServerConfig.NoChangeDir ? 1 : 0);
                    // ClientConfig.FailMsg := MagicConfig.ServerConfig.FailMsg;   ← 原文 :284 注释掉

                    if (!customMagicConfig.ServerConfig.IsCheckVarValue)
                    {
                        clientConfig.NeedItem = customMagicConfig.ServerConfig.NeedItem;
                        clientConfig.NeedItemCount = customMagicConfig.ServerConfig.NeedItemCount;
                        clientConfig.NeedItemCustomItemNameStr = customMagicConfig.ServerConfig.NeedItemCustomItemName;
                        clientConfig.NeedItemUseBagItem = (byte)(customMagicConfig.ServerConfig.NeedItemUseBagItem ? 1 : 0);
                    }
                    else
                    {
                        clientConfig.NeedItem = TMagicNeedItem.meiNone;
                        clientConfig.NeedItemCount = 0;
                        clientConfig.NeedItemCustomItemNameStr = "";
                        clientConfig.NeedItemUseBagItem = 0;   // 原文 False
                    }

                    WriteStruct(buffer, offset, ref clientConfig);
                    offset += SizeOfClientConfig();
                }

                CustomMagicFormGlobals.g_CustomMagicListTextLen = inBytes;
                CustomMagicFormGlobals.g_CustomMagicListText =
                    CustomMagicFormGlobals.ZLibCompressBuffer(buffer, inBytes);
                CustomMagicFormGlobals.g_CustomMagicListTextCRC =
                    CustomMagicFormGlobals.BufferCrc(CustomMagicFormGlobals.g_CustomMagicListText,
                        CustomMagicFormGlobals.g_CustomMagicListText.Length);
            }
            finally
            {
                // FreeMem(InBuf, InBytes + 1)
            }
        }
        finally
        {
            UnlockCustomMagicListIfMultiThread();
        }
    }

    /// <summary><c>SizeOf(TClientCustomMagicConfig)</c> 1:1（packed，wire 布局）。</summary>
    public static int SizeOfClientConfig() => Marshal.SizeOf<TClientCustomMagicConfig>();

    private static void WriteStruct<T>(byte[] buffer, int offset, ref T value) where T : struct
        => MemoryMarshal.Write(buffer.AsSpan(offset), ref value);

    /// <summary>纯序列化（供接缝与测试复用，不压缩）。</summary>
    internal static byte[] BuildClientConfigBytes(List<TCustomMagicConfig> configs)
    {
        int size = SizeOfClientConfig();
        byte[] buffer = new byte[configs.Count * size];
        int offset = 0;
        foreach (var cfg in configs)
        {
            var cc = default(TClientCustomMagicConfig);
            cc.wMagicId = cfg.MagicID;
            cc.MagicBaseConfig = cfg.ClientBaseConfig;
            for (int level = 0; level < CustomMagicUtilsConst.MagicPlusLevelCount; level++)
                cc.MagicConfigs[level] = cfg.ClientConfigs[level].Value;
            cc.boIsMagicWarr = (byte)(cfg.IsMagicWarr ? 1 : 0);
            cc.btNearAttackRange = cfg.IsMagicWarr ? (byte)cfg.ServerConfig.AttackNearRange : (byte)1;
            cc.IsAttackUseNG = (byte)(cfg.ServerConfig.IsAttackUseNG ? 1 : 0);
            cc.NoChangeDir = (byte)(cfg.ServerConfig.NoChangeDir ? 1 : 0);
            if (!cfg.ServerConfig.IsCheckVarValue)
            {
                cc.NeedItem = cfg.ServerConfig.NeedItem;
                cc.NeedItemCount = cfg.ServerConfig.NeedItemCount;
                cc.NeedItemCustomItemNameStr = cfg.ServerConfig.NeedItemCustomItemName;
                cc.NeedItemUseBagItem = (byte)(cfg.ServerConfig.NeedItemUseBagItem ? 1 : 0);
            }
            WriteStruct(buffer, offset, ref cc);
            offset += size;
        }
        return buffer;
    }

    // ========================================================================
    // vstAttackDecAttr（原文 :4290-4428）
    // ========================================================================

    /// <summary>原文 <c>vstAttackDecAttrChecked</c>（:4290-4305）。</summary>
    public void VstAttackDecAttrChecked(IVirtualTreeHost sender, TVirtualNodeSeam node)
    {
        var decAttribData = (TAttackDecAttribData?)sender.GetNodeData(node);
        if (decAttribData != null && decAttribData.Data != null)
        {
            bool isChecked = decAttribData.Data.IsChecked;
            DecAttribTreeLogic.ApplyChecked(false, sender.GetCheckState(node), ref isChecked);
            decAttribData.Data.IsChecked = isChecked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>vstAttackDecAttrNodeClick</c>（:4306-4325）。</summary>
    public void VstAttackDecAttrNodeClick(IVirtualTreeHost sender, THitInfo hitInfo)
    {
        var action = DecAttribTreeLogic.ResolveNodeClick(hitInfo.HitNode == null, hitInfo.HitColumn);
        if (action == TVtNodeClickAction.None)
            return;

        var decAttribData = (TAttackDecAttribData)sender.GetNodeData(hitInfo.HitNode)!;

        if (action == TVtNodeClickAction.ToggleShowHint)
        {
            decAttribData.Data!.ShowHint = !decAttribData.Data.ShowHint;
            sender.InvalidateNode(hitInfo.HitNode);
            SetConfigChanged();
        }
        else
        {
            CustomMagicPostMessageSeam.PostMessage(Handle, CustomMagicWm.WM_STARTEDITING_DEC_ATTRIB,
                hitInfo.HitNode!.Index, hitInfo.HitColumn);
        }
    }

    /// <summary>原文 <c>vstAttackDecAttrAfterCellPaint</c>（:4326-4346）。</summary>
    public void VstAttackDecAttrAfterCellPaint(IVirtualTreeHost sender, TVirtualNodeSeam node, int column, TRectSeam cellRect)
    {
        var decAttribData = (TAttackDecAttribData)sender.GetNodeData(node)!;
        if (DecAttribTreeLogic.TryGetHintIconLayout(column, cellRect, ilCheck.Width, ilCheck.Height,
                decAttribData.Data!.ShowHint, out int x, out int y, out int imageIndex))
        {
            TargetCanvasFontColorMirror = sender.FontColor;   // TargetCanvas.Font.Color := Sender.Font.Color
            BrushStyleClearMirror = true;                     // TargetCanvas.Brush.Style := bsClear
            ilCheck.Draw(null!, x, y, imageIndex);
        }
    }

    /// <summary>AfterCellPaint 的画布镜像（无头环境不能真画）。</summary>
    public int TargetCanvasFontColorMirror { get; private set; }

    /// <summary>Brush.Style := bsClear 镜像。</summary>
    public bool BrushStyleClearMirror { get; private set; }

    /// <summary>原文 <c>vstAttackDecAttrGetText</c>（:4347-4399）。</summary>
    public string VstAttackDecAttrGetText(IVirtualTreeHost sender, TVirtualNodeSeam node, int column)
    {
        var decAttribData = (TAttackDecAttribData)sender.GetNodeData(node)!;
        return DecAttribTreeLogic.GetAttackDecAttribText(column, decAttribData, decAttribData.Data!);
    }

    /// <summary>原文 <c>vstAttackDecAttrEditing</c>（:4400-4413）。</summary>
    public void VstAttackDecAttrEditing(IVirtualTreeHost sender, TVirtualNodeSeam? node, int column, out bool allowed)
    {
        allowed = !(node == null) && column > 0 && column != DecAttribTreeLogic.HintColumn;
        if (allowed)
        {
            var decAttribData = (TAttackDecAttribData)sender.GetNodeData(node)!;
            allowed = DecAttribTreeLogic.IsAttackDecAttribEditingAllowed(false, column, decAttribData.AttribType);
        }
    }

    /// <summary>原文 <c>vstAttackDecAttrCreateEditor</c>（:4414-4419）：<c>EditLink := TDecAttribPropertyEditLink.Create;</c>。</summary>
    public void VstAttackDecAttrCreateEditor(out IVTEditLinkSeam editLink)
        => editLink = new TDecAttribPropertyEditLink();

    /// <summary>原文 <c>WMStartEditingDecAttrib</c>（:4420-4428）。</summary>
    public void WMStartEditingDecAttrib(int wParam, int lParam)
    {
        var node = ResolveNodeByIndex(HostOf("vstAttackDecAttr"), wParam);
        HostOf("vstAttackDecAttr").EditNode(node, lParam);
    }

    // ========================================================================
    // vstDecElement / vstAddElement（原文 :4429-4550）
    // 同一组事件处理器被两棵树共用 —— 原文靠 <c>Sender</c> 区分。
    // ========================================================================

    /// <summary>原文 <c>vstDecElementChecked</c>（:4429-4444）。</summary>
    public void VstDecElementChecked(IVirtualTreeHost sender, TVirtualNodeSeam node)
    {
        var magicElementData = (TMagicElementData?)sender.GetNodeData(node);
        if (magicElementData != null && magicElementData.Data != null)
        {
            bool isChecked = magicElementData.Data.IsChecked;
            ElementTreeLogic.ApplyChecked(false, sender.GetCheckState(node), ref isChecked);
            magicElementData.Data.IsChecked = isChecked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>vstDecElementNodeClick</c>（:4445-4467）。</summary>
    public void VstDecElementNodeClick(IVirtualTreeHost sender, THitInfo hitInfo)
    {
        var action = ElementTreeLogic.ResolveNodeClick(hitInfo.HitNode == null, hitInfo.HitColumn);
        if (action == TVtNodeClickAction.None)
            return;

        var magicElementData = (TMagicElementData)sender.GetNodeData(hitInfo.HitNode)!;

        if (action == TVtNodeClickAction.ToggleShowHint)
        {
            magicElementData.Data!.ShowHint = !magicElementData.Data.ShowHint;
            sender.InvalidateNode(hitInfo.HitNode);
            SetConfigChanged();
        }
        else
        {
            // 原文 :4462-4464 if Sender = vstDecElement then ... else ...
            bool senderIsDecElement = ReferenceEquals(sender, HostOf("vstDecElement"));
            CustomMagicPostMessageSeam.PostMessage(Handle, ElementTreeLogic.StartEditingMessage(senderIsDecElement),
                hitInfo.HitNode!.Index, hitInfo.HitColumn);
        }
    }

    /// <summary>原文 <c>vstDecElementAfterCellPaint</c>（:4468-4488）。</summary>
    public void VstDecElementAfterCellPaint(IVirtualTreeHost sender, TVirtualNodeSeam node, int column, TRectSeam cellRect)
    {
        var magicElementData = (TMagicElementData)sender.GetNodeData(node)!;
        if (ElementTreeLogic.TryGetHintIconLayout(column, cellRect, ilCheck.Width, ilCheck.Height,
                magicElementData.Data!.ShowHint, out int x, out int y, out int imageIndex))
        {
            TargetCanvasFontColorMirror = sender.FontColor;
            BrushStyleClearMirror = true;
            ilCheck.Draw(null!, x, y, imageIndex);
        }
    }

    /// <summary>原文 <c>vstDecElementGetText</c>（:4489-4520）。</summary>
    public string VstDecElementGetText(IVirtualTreeHost sender, TVirtualNodeSeam node, int column)
    {
        var magicElementData = (TMagicElementData)sender.GetNodeData(node)!;
        return ElementTreeLogic.GetText(column, magicElementData, magicElementData.Data!);
    }

    /// <summary>原文 <c>vstDecElementEditing</c>（:4521-4526）。</summary>
    public void VstDecElementEditing(TVirtualNodeSeam? node, int column, out bool allowed)
        => allowed = ElementTreeLogic.IsEditingAllowed(node == null, column);

    /// <summary>原文 <c>vstDecElementCreateEditor</c>（:4527-4532）。</summary>
    public void VstDecElementCreateEditor(out IVTEditLinkSeam editLink)
        => editLink = new TElementPropertyEditLink();

    /// <summary>原文 <c>WMStartEditingDecElement</c>（:4533-4541）。</summary>
    public void WMStartEditingDecElement(int wParam, int lParam)
        => HostOf("vstDecElement").EditNode(ResolveNodeByIndex(HostOf("vstDecElement"), wParam), lParam);

    /// <summary>原文 <c>WMStartEditingIncElement</c>（:4542-4550）。</summary>
    public void WMStartEditingIncElement(int wParam, int lParam)
        => HostOf("vstAddElement").EditNode(ResolveNodeByIndex(HostOf("vstAddElement"), wParam), lParam);

    // ========================================================================
    // vstProtectedAddAttr（原文 :4551-4723）
    // ========================================================================

    /// <summary>原文 <c>vstProtectedAddAttrChecked</c>（:4551-4566）。</summary>
    public void VstProtectedAddAttrChecked(IVirtualTreeHost sender, TVirtualNodeSeam node)
    {
        var incAttribData = (TProtectAddAttribData?)sender.GetNodeData(node);
        if (incAttribData != null && incAttribData.Data != null)
        {
            bool isChecked = incAttribData.Data.IsChecked;
            DecAttribTreeLogic.ApplyChecked(false, sender.GetCheckState(node), ref isChecked);
            incAttribData.Data.IsChecked = isChecked;
            SetConfigChanged();
        }
    }

    /// <summary>原文 <c>vstProtectedAddAttrNodeClick</c>（:4567-4586）。</summary>
    public void VstProtectedAddAttrNodeClick(IVirtualTreeHost sender, THitInfo hitInfo)
    {
        var action = DecAttribTreeLogic.ResolveNodeClick(hitInfo.HitNode == null, hitInfo.HitColumn);
        if (action == TVtNodeClickAction.None)
            return;

        var incAttribData = (TProtectAddAttribData)sender.GetNodeData(hitInfo.HitNode)!;

        if (action == TVtNodeClickAction.ToggleShowHint)
        {
            incAttribData.Data!.ShowHint = !incAttribData.Data.ShowHint;
            sender.InvalidateNode(hitInfo.HitNode);
            SetConfigChanged();
        }
        else
        {
            CustomMagicPostMessageSeam.PostMessage(Handle, CustomMagicWm.WM_STARTEDITING_INC_ATTRIB,
                hitInfo.HitNode!.Index, hitInfo.HitColumn);
        }
    }

    /// <summary>原文 <c>vstProtectedAddAttrAfterCellPaint</c>（:4587-4607）。</summary>
    public void VstProtectedAddAttrAfterCellPaint(IVirtualTreeHost sender, TVirtualNodeSeam node, int column, TRectSeam cellRect)
    {
        var incAttribData = (TProtectAddAttribData)sender.GetNodeData(node)!;
        if (DecAttribTreeLogic.TryGetHintIconLayout(column, cellRect, ilCheck.Width, ilCheck.Height,
                incAttribData.Data!.ShowHint, out int x, out int y, out int imageIndex))
        {
            TargetCanvasFontColorMirror = sender.FontColor;
            BrushStyleClearMirror = true;
            ilCheck.Draw(null!, x, y, imageIndex);
        }
    }

    /// <summary>原文 <c>vstProtectedAddAttrGetText</c>（:4608-4690）。</summary>
    public string VstProtectedAddAttrGetText(IVirtualTreeHost sender, TVirtualNodeSeam node, int column)
    {
        var incAttribData = (TProtectAddAttribData)sender.GetNodeData(node)!;
        return DecAttribTreeLogic.GetProtectedAddAttribText(column, incAttribData, incAttribData.Data!);
    }

    /// <summary>原文 <c>vstProtectedAddAttrEditing</c>（:4691-4708）。</summary>
    public void VstProtectedAddAttrEditing(IVirtualTreeHost sender, TVirtualNodeSeam? node, int column, out bool allowed)
    {
        allowed = !(node == null) && column > 0 && column != DecAttribTreeLogic.HintColumn;
        if (allowed)
        {
            var incAttribData = (TProtectAddAttribData)sender.GetNodeData(node)!;
            allowed = DecAttribTreeLogic.IsProtectedAddAttribEditingAllowed(false, column, incAttribData.AttribType);
        }
    }

    /// <summary>原文 <c>vstProtectedAddAttrCreateEditor</c>（:4709-4714）：
    /// <c>EditLink := TDecAttribPropertyEditLink.Create;</c>（复用减属性链接）。</summary>
    public void VstProtectedAddAttrCreateEditor(out IVTEditLinkSeam editLink)
        => editLink = new TDecAttribPropertyEditLink();

    /// <summary>原文 <c>WMStartEditingIncAttrib</c>（:4715-4723）。</summary>
    public void WMStartEditingIncAttrib(int wParam, int lParam)
        => HostOf("vstProtectedAddAttr").EditNode(ResolveNodeByIndex(HostOf("vstProtectedAddAttr"), wParam), lParam);

    /// <summary>WM_ 消息里 <c>WPARAM(HitInfo.HitNode)</c> 是**节点地址**；托管侧改用节点序号。</summary>
    private static TVirtualNodeSeam? ResolveNodeByIndex(IVirtualTreeHost host, int index)
    {
        if (host is CustomMagicTreeHost h)
        {
            if (index >= 0 && index < h.Nodes.Count)
                return h.Nodes[index];
        }
        return null;
    }
}

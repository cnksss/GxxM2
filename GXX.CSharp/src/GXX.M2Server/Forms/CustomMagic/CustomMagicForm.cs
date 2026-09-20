// ============================================================================
// uFrmCustomMagic.pas（Source\M2Engine\Forms\uFrmCustomMagic.pas，4800 行，GBK）1:1 移植
// 车道 p5-m2-custommagic ｜ 命名空间 GXX.M2Server.Forms.CustomMagic
//
// 本文件 = TFrmCustomMagic 的**主体**：私有状态 / SetConfigChanged / DoOpen / FormCreate /
// vstCustomMagic 主列表三事件 / vstCustomMagicNodeClick（全窗体最长的一段装载逻辑）。
//
// 覆盖行号（Delphi）：
//   SetConfigChanged           :1863-1875
//   DoOpen                     :1638-1666
//   FormCreate                 :1668-1861
//   vstCustomMagicDrawText     :1877-1893
//   vstCustomMagicGetNodeDataSize :1895-1898
//   vstCustomMagicGetText      :1900-1908
//   vstCustomMagicNodeClick    :1910-2376
//   SetControlEnabled          :1605-1623
//   ShowCustomMagic            :1625-1636
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms.CustomMagic;

/// <summary>
/// uFrmCustomMagic.pas:16 <c>TFrmCustomMagic = class(TForm)</c> 的托管对应物。
/// <para>
/// 组件字段（:17-616，600 个）落在 <c>CustomMagicForm.Components.cs</c>；
/// 151 个表驱动写回处理器落在 <c>CustomMagicForm.Handlers.cs</c>；
/// 其余处理器落在 <c>CustomMagicForm.Actions.cs</c>。
/// </para>
/// <para>
/// 说明：组件字段用**接缝类型**（TSpinEditExSeam/TComboBoxSeam/…）而非真实 WinForms 控件，
/// 因为 VirtualTrees.pas / SpinEditEx.pas 未移植；接缝成员面与原文调用的成员一一对应。
/// </para>
/// </summary>
public partial class TFrmCustomMagic : System.Windows.Forms.Form
{
    /// <summary>原文 <c>FCurrentCustomConfig: TCustomMagicConfig</c>（:832）。</summary>
    public TCustomMagicConfig? FCurrentCustomConfig;

    /// <summary>原文 <c>FCurrentClientConfig: PMagicClientConfig</c>（:833）—— 指向 ClientConfigs[level]。</summary>
    public TMagicClientConfigHolder? FCurrentClientConfig;

    /// <summary>原文 <c>FCurrentServerConfig: PMagicServerConfig</c>（:834）—— 指向 ServerConfig。</summary>
    public TMagicServerConfig? FCurrentServerConfig;

    /// <summary>原文 <c>FIsConfigChanged: Boolean</c>（:836）。</summary>
    public bool FIsConfigChanged;

    /// <summary>主列表树（原文 <c>vstCustomMagic</c>，:18）。构造时建立并存于 <see cref="vstCustomMagic"/> 字段的同名接缝里。</summary>
    public CustomMagicTreeHost? MainTreeHost;

    /// <summary>一棵属性树的宿主（原文 vstAttackDecAttr / vstDecElement / vstProtectedAddAttr / vstAddElement）。
    /// 仅用于把 <c>Sender.GetNodeData</c> 的接收方映射到同名接缝。</summary>
    public readonly Dictionary<string, CustomMagicTreeHost> TreeHosts = new();

    public TFrmCustomMagic()
    {
        InitializeComponentSeam();
    }

    /// <summary>
    /// 组件接缝的建立（替代 DFM 载入）。字段对象在字段初始化器里已建；
    /// 这里补建 5 棵树的内存宿主与页面集合关系。
    /// </summary>
    private void InitializeComponentSeam()
    {
        MainTreeHost = new CustomMagicTreeHost(this);
        TreeHosts["vstCustomMagic"] = MainTreeHost;

        foreach (var name in new[] { "vstAttackDecAttr", "vstDecElement", "vstProtectedAddAttr", "vstAddElement" })
            TreeHosts[name] = new CustomMagicTreeHost(this);

        // 原文 DFM：pgcMain 三页、pgcClient 若干页、pgcMagicType 两页
        pgcMain.Pages.Add(tsAttack);
        pgcMain.Pages.Add(tsServerAttack);
        pgcMain.ActivePage = tsAttack;
        pgcClient.Pages.Add(tsBase);
        pgcClient.Pages.Add(tsEffect);
        pgcClient.ActivePage = tsBase;
    }

    /// <summary>主列表树宿主的便捷访问（<c>Sender</c> = vstCustomMagic）。</summary>
    private IVirtualTreeHost VstCustomMagic => MainTreeHost!;

    /// <summary>某棵子树的宿主。</summary>
    private IVirtualTreeHost HostOf(string name) => TreeHosts[name];

    // ========================================================================
    // SetControlEnabled（原文 :1605-1623，单元级过程，非类方法）
    // ========================================================================

    /// <summary>
    /// 原文 <c>procedure SetControlEnabled(WinControl: TWinControl; Value: Boolean);</c>（:1605-1623）：
    /// 递归进 TTabSheet/TPanel/TGroupBox，其余直接置 <c>Enabled</c>。
    /// </summary>
    public static void SetControlEnabled(TWinControlSeam winControl, bool value)
    {
        for (int i = 0; i < winControl.Controls.Count; i++)
        {
            TControlSeam ctrl = winControl.Controls[i];
            if (ctrl is TWinControlSeam winCtrl)
            {
                if (ctrl is TTabSheetSeam || ctrl is TPanelSeam || ctrl is TGroupBoxSeam)
                    SetControlEnabled(winCtrl, value);
                else
                    winCtrl.Enabled = value;
            }
        }
    }

    // ========================================================================
    // SetConfigChanged（原文 :1863-1875）
    // ========================================================================

    /// <summary>原文 <c>procedure SetConfigChanged(IsChanged: Boolean = True);</c>（:1863-1875）。</summary>
    public void SetConfigChanged(bool IsChanged = true)
    {
        if (FCurrentCustomConfig != null)
        {
            FCurrentCustomConfig.SetChanged(IsChanged);
            if (VstCustomMagic.FocusedNode != null)
                VstCustomMagic.InvalidateNode(VstCustomMagic.FocusedNode);

            FIsConfigChanged = true;
            if (!btnSave.Enabled)
                btnSave.Enabled = true;
        }
    }

    // ========================================================================
    // ShowCustomMagic（原文 :1625-1636，单元级函数）
    // ========================================================================

    /// <summary>
    /// 原文 <c>function ShowCustomMagic: Boolean;</c>（:1625-1636）：
    /// Create(nil) → DoOpen → ShowModal = mrOK → Free。
    /// <c>ShowModal</c> 走 <see cref="CustomMagicMessageBoxSeam.ShowModalHandler"/> 接缝（无头安全）。
    /// </summary>
    public static bool ShowCustomMagic()
    {
        var frmCustomMagic = new TFrmCustomMagic();
        try
        {
            frmCustomMagic.DoOpen();
            int modalResult;
            if (CustomMagicMessageBoxSeam.ShowModalHandler != null)
            {
                modalResult = (int)CustomMagicMessageBoxSeam.ShowModalHandler();
            }
            else
            {
                // 接缝未接线：不弹模态框（若 UiEnabled 为真则走真实 ShowDialog）
                modalResult = CustomMagicMessageBoxSeam.UiEnabled
                    ? (int)(frmCustomMagic.ShowDialog() == System.Windows.Forms.DialogResult.OK
                        ? System.Windows.Forms.DialogResult.OK : System.Windows.Forms.DialogResult.Cancel)
                    : TModalResult.mrCancel;
            }
            return modalResult == TModalResult.mrOk;
        }
        finally
        {
            frmCustomMagic.Dispose();   // FrmCustomMagic.Free
        }
    }

    // ========================================================================
    // DoOpen（原文 :1638-1666）
    // ========================================================================

    /// <summary>原文 <c>procedure DoOpen;</c>（:1638-1666）。</summary>
    public void DoOpen()
    {
        chkSendCustomMagicConfig.Checked = CustomMagicFormGlobals.boSendCustomMagicConfig;

        // {$IF MULTI_THREAD = 1} if g_MultiThreadRun then UserEngine.m_CustomMagicList.LockR(3);
        LockCustomMagicListIfMultiThread(3);
        try
        {
            for (int i = 0; i < CustomMagicFormGlobals.m_CustomMagicList.Count; i++)
            {
                TCustomMagicConfig customMagicConfig = CustomMagicFormGlobals.m_CustomMagicList[i];

                var node = VstCustomMagic.AddChild(null);
                var configNodeData = (TMagicConfigNodeData)VstCustomMagic.GetNodeData(node)!;
                configNodeData.Config = customMagicConfig;
            }
        }
        finally
        {
            UnlockCustomMagicListIfMultiThread();
        }
    }

    /// <summary>原文 <c>{$IF MULTI_THREAD = 1} if g_MultiThreadRun then m_CustomMagicList.LockR(n);</c>。</summary>
    private static void LockCustomMagicListIfMultiThread(int reason)
    {
        if (CustomMagicFormGlobals.g_MultiThreadRun)
            CustomMagicFormGlobals.ListLockCalls.Add(("LockR", reason));
    }

    /// <summary>原文 <c>if g_MultiThreadRun then m_CustomMagicList.UnLockR;</c>。</summary>
    private static void UnlockCustomMagicListIfMultiThread()
    {
        if (CustomMagicFormGlobals.g_MultiThreadRun)
            CustomMagicFormGlobals.ListLockCalls.Add(("UnLockR", 0));
    }

    // ========================================================================
    // FormCreate（原文 :1668-1861）
    // ========================================================================

    /// <summary>原文 <c>procedure FormCreate(Sender: TObject);</c>（:1668-1861）：全部下拉的选项表与初始禁用。</summary>
    public void FormCreate()
    {
        // 原文局部量（:1669-1685）在托管侧内联为循环变量。

        vstAttackDecAttr.NodeDataSize = 0;      // SizeOf(TAttackDecAttribData)（接缝：对象引用）
        vstDecElement.NodeDataSize = 0;         // SizeOf(TMagicElementData)
        vstProtectedAddAttr.NodeDataSize = 0;   // SizeOf(TProtectAddAttribData)
        vstAddElement.NodeDataSize = 0;         // SizeOf(TMagicElementData)

        cbbClientLevel.Items.Clear();
        for (int magicPlusLevel = 0; magicPlusLevel < CustomMagicUtils.MagicPlusLevelNames.Length; magicPlusLevel++)
            cbbClientLevel.Items.Add(CustomMagicUtils.MagicPlusLevelNames[magicPlusLevel]);

        cbbClientActionType.Items.Clear();
        for (int magicActionType = 0; magicActionType < CustomMagicUtils.MagicActionTypeNames.Length; magicActionType++)
            cbbClientActionType.Items.Add(CustomMagicUtils.MagicActionTypeNames[magicActionType]);

        cbbMagicSwitchMode.Items.Clear();
        for (int magicSwitchMode = 0; magicSwitchMode < CustomMagicUtils.MagicSwitchModeNames.Length; magicSwitchMode++)
            cbbMagicSwitchMode.Items.Add(CustomMagicUtils.MagicSwitchModeNames[magicSwitchMode]);

        cbbMagicWarrNGOption.Items.Clear();
        for (int magicWarrNGOption = 0; magicWarrNGOption < CustomMagicUtils.MagicWarrNGOptionNames.Length; magicWarrNGOption++)
            cbbMagicWarrNGOption.Items.Add(CustomMagicUtils.MagicWarrNGOptionNames[magicWarrNGOption]);

        cbbClientIconFile.Items.Add("无");
        for (int i = 0; i < CustomMagicFormGlobals.EffectImageList().Count; i++)
            cbbClientIconFile.Items.Add(CustomMagicFormGlobals.EffectImageList()[i]);

        cbbClientFlyFile.Items.Clear();
        cbbClientFlyFile.Items.Add("无");
        for (int i = 0; i < CustomMagicFormGlobals.EffectImageList().Count; i++)
            cbbClientFlyFile.Items.Add(CustomMagicFormGlobals.EffectImageList()[i]);

        cbbClientFlyDrawMode.Items.Clear();
        for (int drawMode = 0; drawMode < CustomMagicCustomDrawModeNames.Length; drawMode++)
            cbbClientFlyDrawMode.Items.Add(CustomMagicCustomDrawModeNames[drawMode]);

        cbbClientFlyDirCount.Items.Clear();
        for (int dirCount = 0; dirCount < CustomMagicCustomDirNames.Length; dirCount++)
            cbbClientFlyDirCount.Items.Add(CustomMagicCustomDirNames[dirCount]);

        cbbClientFlyEffFile.Items.Clear();
        cbbClientFlyEffFile.Items.Add("无");
        for (int i = 0; i < CustomMagicFormGlobals.EffectImageList().Count; i++)
            cbbClientFlyEffFile.Items.Add(CustomMagicFormGlobals.EffectImageList()[i]);

        cbbClientFlyEffDrawMode.Items.Clear();
        for (int drawMode = 0; drawMode < CustomMagicCustomDrawModeNames.Length; drawMode++)
            cbbClientFlyEffDrawMode.Items.Add(CustomMagicCustomDrawModeNames[drawMode]);

        cbbClientSelfFile.Items.Clear();
        cbbClientSelfFile.Items.Add("无");
        for (int i = 0; i < CustomMagicFormGlobals.EffectImageList().Count; i++)
            cbbClientSelfFile.Items.Add(CustomMagicFormGlobals.EffectImageList()[i]);

        cbbClientSelfDrawOrder.Items.Clear();
        for (int drawOrder = 0; drawOrder < CustomMagicCustomDrawOrderNames.Length; drawOrder++)
            cbbClientSelfDrawOrder.Items.Add(CustomMagicCustomDrawOrderNames[drawOrder]);

        cbbClientSelfDrawMode.Items.Clear();
        for (int drawMode = 0; drawMode < CustomMagicCustomDrawModeNames.Length; drawMode++)
            cbbClientSelfDrawMode.Items.Add(CustomMagicCustomDrawModeNames[drawMode]);

        cbbClientSelfDirCount.Items.Clear();
        for (int dirCount = 0; dirCount < CustomMagicCustomDirNames.Length; dirCount++)
            cbbClientSelfDirCount.Items.Add(CustomMagicCustomDirNames[dirCount]);

        cbbClientSelfDirCalcType.Items.Clear();
        for (int dirCalcType = 0; dirCalcType < CustomMagicCustomDirCalcTypeNames.Length; dirCalcType++)
            cbbClientSelfDirCalcType.Items.Add(CustomMagicCustomDirCalcTypeNames[dirCalcType]);

        cbbClientSelfKeepFile.Items.Clear();
        cbbClientSelfKeepFile.Items.Add("无");
        for (int i = 0; i < CustomMagicFormGlobals.EffectImageList().Count; i++)
            cbbClientSelfKeepFile.Items.Add(CustomMagicFormGlobals.EffectImageList()[i]);

        cbbClientSelfKeepDrawMode.Items.Clear();
        for (int drawMode = 0; drawMode < CustomMagicCustomDrawModeNames.Length; drawMode++)
            cbbClientSelfKeepDrawMode.Items.Add(CustomMagicCustomDrawModeNames[drawMode]);

        cbbClientFastMoveFile.Items.Clear();
        cbbClientFastMoveFile.Items.Add("无");
        for (int i = 0; i < CustomMagicFormGlobals.EffectImageList().Count; i++)
            cbbClientFastMoveFile.Items.Add(CustomMagicFormGlobals.EffectImageList()[i]);

        cbbClientFastMoveDrawMode.Items.Clear();
        for (int drawMode = 0; drawMode < CustomMagicCustomDrawModeNames.Length; drawMode++)
            cbbClientFastMoveDrawMode.Items.Add(CustomMagicCustomDrawModeNames[drawMode]);

        cbbClientPreTargetDrawMode.Items.Clear();
        for (int drawMode = 0; drawMode < CustomMagicCustomDrawModeNames.Length; drawMode++)
            cbbClientPreTargetDrawMode.Items.Add(CustomMagicCustomDrawModeNames[drawMode]);

        cbbClientPreTargetDrawMode2.Items.Clear();
        for (int drawMode = 0; drawMode < CustomMagicCustomDrawModeNames.Length; drawMode++)
            cbbClientPreTargetDrawMode2.Items.Add(CustomMagicCustomDrawModeNames[drawMode]);

        cbbClientPreTargetFile.Items.Clear();
        cbbClientPreTargetFile.Items.Add("无");
        for (int i = 0; i < CustomMagicFormGlobals.EffectImageList().Count; i++)
            cbbClientPreTargetFile.Items.Add(CustomMagicFormGlobals.EffectImageList()[i]);

        cbbClientTargetDrawMode.Items.Clear();
        for (int drawMode = 0; drawMode < CustomMagicCustomDrawModeNames.Length; drawMode++)
            cbbClientTargetDrawMode.Items.Add(CustomMagicCustomDrawModeNames[drawMode]);

        cbbClientTargetDrawMode2.Items.Clear();
        for (int drawMode = 0; drawMode < CustomMagicCustomDrawModeNames.Length; drawMode++)
            cbbClientTargetDrawMode2.Items.Add(CustomMagicCustomDrawModeNames[drawMode]);

        cbbClientTargetFile.Items.Clear();
        cbbClientTargetFile.Items.Add("无");
        for (int i = 0; i < CustomMagicFormGlobals.EffectImageList().Count; i++)
            cbbClientTargetFile.Items.Add(CustomMagicFormGlobals.EffectImageList()[i]);

        cbbTargetStatus1_File.Items.Clear();
        cbbTargetStatus1_File.Items.Add("无");
        for (int i = 0; i < CustomMagicFormGlobals.EffectImageList().Count; i++)
            cbbTargetStatus1_File.Items.Add(CustomMagicFormGlobals.EffectImageList()[i]);

        cbbTargetStatus2_File.Items.Clear();
        cbbTargetStatus2_File.Items.Add("无");
        for (int i = 0; i < CustomMagicFormGlobals.EffectImageList().Count; i++)
            cbbTargetStatus2_File.Items.Add(CustomMagicFormGlobals.EffectImageList()[i]);

        cbbTargetStatus1_DrawMode.Items.Clear();
        for (int drawMode = 0; drawMode < CustomMagicCustomDrawModeNames.Length; drawMode++)
            cbbTargetStatus1_DrawMode.Items.Add(CustomMagicCustomDrawModeNames[drawMode]);

        cbbTargetStatus2_DrawMode.Items.Clear();
        for (int drawMode = 0; drawMode < CustomMagicCustomDrawModeNames.Length; drawMode++)
            cbbTargetStatus2_DrawMode.Items.Add(CustomMagicCustomDrawModeNames[drawMode]);

        cbbOperateMode.Items.Clear();
        for (int operateMode = 0; operateMode < CustomMagicCustomOperateModeNames.Length; operateMode++)
            cbbOperateMode.Items.Add(CustomMagicCustomOperateModeNames[operateMode]);

        cbbAttackTarget.Items.Clear();
        for (int attackTarget = 0; attackTarget < CustomMagicCustomAttackTargetNames.Length; attackTarget++)
            cbbAttackTarget.Items.Add(CustomMagicCustomAttackTargetNames[attackTarget]);

        cbbAttackPowerCalc.Items.Clear();
        for (int attackPowerCalc = 0; attackPowerCalc < CustomMagicCustomAttackPowerCalcNames.Length; attackPowerCalc++)
            cbbAttackPowerCalc.Items.Add(CustomMagicCustomAttackPowerCalcNames[attackPowerCalc]);

        cbbAttackPowerLevel.Items.Clear();
        for (int i = 0; i < CustomMagicUtils.CustomMagicLevelNames.Length; i++)
            cbbAttackPowerLevel.Items.Add(CustomMagicUtils.CustomMagicLevelNames[i]);

        cbbNeedItem.Items.Clear();
        for (int needItem = 0; needItem < CustomMagicMagicNeedItemNames.Length; needItem++)
            cbbNeedItem.Items.Add(CustomMagicMagicNeedItemNames[needItem]);

        cbbCheckVarType.Items.Clear();
        for (int checkVarType = 0; checkVarType < CustomMagicUtils.CheckVarTypeNames.Length; checkVarType++)
            cbbCheckVarType.Items.Add(CustomMagicUtils.CheckVarTypeNames[checkVarType]);

        lblCheckVarName.Top = lblNeedItem.Top;
        edtCheckVarName.Top = cbbNeedItem.Top;
        lblCheckVarType.Top = lblNeedItemCount.Top;
        cbbCheckVarType.Top = seNeedItemCount.Top;
        lblCheckVarValue.Top = lblNeedItemCustomItemName.Top;
        seCheckVarValue.Top = edtNeedItemCustomItemName.Top;
        lblCheckVarAdd.Top = lblCheckVarValue.Top;
        seCheckVarAdd.Top = seCheckVarValue.Top;

        SetControlEnabled(pgcMain, false);

        FCurrentCustomConfig = null;
        FCurrentClientConfig = null;
        FCurrentServerConfig = null;

        pgcMain.ActivePageIndex = 0;
        pgcClient.ActivePageIndex = 0;

        // DoOpen;   ← 原文 :1860 被注释掉
    }

    // 原文 FormCreate 用的名称表，C# 侧正式归属在 Engine\CustomMonsterState.cs:35-43
    // （与 uCustomMagicUtils.pas / M2Share.pas 同源，逐项比对一致）。
    private static readonly string[] CustomMagicCustomDrawModeNames = CustomMonsterConsts.CustomDrawModeNames;
    private static readonly string[] CustomMagicCustomDirNames = CustomMonsterConsts.CustomDirNames;
    private static readonly string[] CustomMagicCustomDrawOrderNames = CustomMonsterConsts.CustomDrawOrderNames;
    private static readonly string[] CustomMagicCustomDirCalcTypeNames = CustomMonsterConsts.CustomDirCalcTypeNames;
    private static readonly string[] CustomMagicCustomOperateModeNames = CustomMonsterConsts.CustomOperateModeNames;
    private static readonly string[] CustomMagicCustomAttackTargetNames = CustomMonsterConsts.CustomAttackTargetNames;
    private static readonly string[] CustomMagicCustomAttackPowerCalcNames = CustomMonsterConsts.CustomAttackPowerCalcNames;

    /// <summary>
    /// MagicNeedItemNames: array[TMagicNeedItem]（原文 <c>Grobal2.pas:5497</c>，
    /// <c>'无','红毒','绿毒','符','自定义物品'</c>）。该表在 GXX.Core.Protocol 中**尚未移植**，
    /// 接缝：待 GXX.Core 补 <c>MagicNeedItemNames</c> 后改为引用（精确签名见报告 §7）。
    /// </summary>
    private static readonly string[] CustomMagicMagicNeedItemNames = { "无", "红毒", "绿毒", "符", "自定义物品" };

    // ========================================================================
    // vstCustomMagic 三事件（原文 :1877-1908）
    // ========================================================================

    /// <summary>原文 <c>vstCustomMagicDrawText</c>（:1877-1893）。
    /// 返回字体颜色（<c>DefaultDraw</c> 变量在原文中是 <c>var</c> 形参，未被赋值）。</summary>
    public int VstCustomMagicDrawText(IVirtualTreeHost sender, TVirtualNodeSeam? node, bool defaultDraw)
    {
        var configNodeData = (TMagicConfigNodeData?)sender.GetNodeData(node);

        if (configNodeData != null)
        {
            if (configNodeData.Config != null && configNodeData.Config.IsChanged)
                FontColorMirror = CustomMagicColors.clRed;
            else if (sender.IsSelected(node!) && sender.Focused)
                FontColorMirror = CustomMagicColors.clHighlightText;
            else
                FontColorMirror = sender.FontColor;
        }
        return FontColorMirror;
    }

    /// <summary>DrawText 的决策镜像（生产运行时恒等于真实 TargetCanvas.Font.Color）。</summary>
    public int FontColorMirror { get; private set; }

    /// <summary>原文 <c>vstCustomMagicGetNodeDataSize</c>（:1895-1898）。</summary>
    public void VstCustomMagicGetNodeDataSize(out int nodeDataSize)
        => nodeDataSize = CustomMagicMainTreeLogic.NodeDataSize;

    /// <summary>原文 <c>vstCustomMagicGetText</c>（:1900-1908）。</summary>
    public string VstCustomMagicGetText(IVirtualTreeHost sender, TVirtualNodeSeam? node)
    {
        var configNodeData = (TMagicConfigNodeData?)sender.GetNodeData(node);
        string cellText = "";
        if (configNodeData != null)
            cellText = configNodeData.Config?.MagicName ?? "";
        return cellText;
    }

    // ========================================================================
    // vstCustomMagicNodeClick（原文 :1910-2376）
    // ========================================================================

    /// <summary>原文 <c>vstCustomMagicNodeClick</c>（:1910-2376）：把选中节点的配置**全量装载**到全部控件。</summary>
    public void VstCustomMagicNodeClick(IVirtualTreeHost sender, THitInfo hitInfo)
    {
        FCurrentCustomConfig = null;
        FCurrentClientConfig = null;
        FCurrentServerConfig = null;

        if (sender.FocusedNode == null)
            return;                                   // 原文 :1926-1927

        SetControlEnabled(pgcMain, true);

        var configNodeData = (TMagicConfigNodeData?)sender.GetNodeData(sender.FocusedNode);
        if (configNodeData == null)
            return;                                   // 原文 :1932-1933

        bool oldIsConfigCanSave = FIsConfigChanged;

        FCurrentCustomConfig = configNodeData.Config;
        // 原文 :1938 FCurrentClientConfig := @FCurrentCustomConfig.ClientBaseConfig;
        //   —— 原文用 {$T-} 下的**无类型** @，把 TMagicClientBaseConfig 的地址赋给 PMagicClientConfig；
        //      该指针在 :1944 触发 cbbClientLevelChange 时立刻被改成 ClientConfigs[0]，
        //      在被改写前**从未解引用**。托管侧无法复刻"类型双关的野指针"，
        //      故此处保留 null 并在报告"原文缺陷"#2 登记该偏差（行为等价）。
        FCurrentClientConfig = null;
        FCurrentServerConfig = FCurrentCustomConfig!.ServerConfig;

        bool oldChanged = FCurrentCustomConfig.IsChanged;

        cbbClientLevel.ItemIndex = 0;
        CbbClientLevelChange();                       // 原文 :1944 cbbClientLevel.OnChange(cbbClientLevel)

        // chkClientLevelEnabled.Checked := ...   ← 原文 :1946-1947 注释掉
        chkClientLock.Checked = FCurrentCustomConfig.ClientBaseConfig.MagicLock != 0;
        chkClientLockSelf.Checked = FCurrentCustomConfig.ClientBaseConfig.MagicLockSelf != 0;

        if (FCurrentCustomConfig.IsMagicWarr)
        {
            txtMagicWarr.Caption = "战士技能";
            txtMagicWarr.FontColor = CustomMagicColors.clRed;

            SetControlEnabled(grpFly, false);
            grpFly.Enabled = false;

            SetControlEnabled(grpFlyEff, false);
            grpFlyEff.Enabled = false;

            SetControlEnabled(grpMove, false);

            if (FCurrentServerConfig.OperateMode != TCustomOperateMode.momAttack)
            {
                cbbOperateMode.ItemIndex = (int)TCustomOperateMode.momAttack;
                FCurrentServerConfig.OperateMode = TCustomOperateMode.momAttack;

                CbbOperateModeChange();               // 原文 :1969 cbbOperateMode.OnChange(cbbOperateMode)
            }

            // lblMagicWarrNGOption.Visible := True;   ← 原文 :1972 注释掉
            cbbMagicWarrNGOption.Enabled = true;
            cbbMagicWarrNGOption.ItemIndex = (int)FCurrentCustomConfig.ClientBaseConfig.MagicWarrNGOption;

            cbbOperateMode.Enabled = false;
            cbbMagicSwitchMode.Enabled = true;
            chkSwitchModeNoClose.Enabled = FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode == TMagicSwitchMode.msmSwitch;
            chkMagicAutoOpen.Enabled = FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode == TMagicSwitchMode.msmSwitch;

            seClientSelfPlayTime.Enabled = true;
            chkClientTargetMultiPlay.Enabled = false;
            chkClientTargetKeepPlay.Enabled = false;
            seClientTargetKeepTime.Enabled = false;
            seClientTargetKeepAttackInterval.Enabled = false;
            seClientTargetKeepAttackRange.Enabled = false;
            chkClientTargetKeepMultiPlay.Enabled = false;
            seTargetKeepLightRange.Enabled = false;

            cbbClientActionType.ItemIndex = (int)FCurrentCustomConfig.ClientBaseConfig.MagicActionType;
            cbbMagicSwitchMode.ItemIndex = (int)FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode;
            chkSwitchModeNoClose.Checked = FCurrentCustomConfig.ClientBaseConfig.SwitchModeNoClose != 0;
            chkMagicAutoOpen.Checked = FCurrentCustomConfig.ClientBaseConfig.MagicAutoOpen != 0;

            // { ... }   ← 原文 :1995-2002 整段处于 {} 注释内
            //   //FCurrentCustomConfig.ClientBaseConfig.MagicActionType := matHit;
            //   cbbClientActionType.Enabled := False; ... chkClientActionContinue.Enabled := False;

            cbbClientActionType.Enabled = true;
            bool actionIndexEnabled = FCurrentCustomConfig.ClientBaseConfig.MagicActionType == TMagicActionType.matCustom;
            seClientActionStartIndex.Enabled = actionIndexEnabled;
            seClientActionPlayCount.Enabled = seClientActionStartIndex.Enabled;
            seClientActionEmptyCount.Enabled = seClientActionStartIndex.Enabled;
            // 原文 :2008 chkClientActionContinue.Enabled := seClientActionStartIndex.Enabled { and (not chkMagicSwitchMode.Enabled) };
            chkClientActionContinue.Enabled = seClientActionStartIndex.Enabled;

            seClientActionStartIndex.Value = FCurrentCustomConfig.ClientBaseConfig.MagicActionStartIndex;
            seClientActionPlayCount.Value = FCurrentCustomConfig.ClientBaseConfig.MagicActionPlayCount;
            seClientActionEmptyCount.Value = FCurrentCustomConfig.ClientBaseConfig.MagicActionEmptyCount;
            chkClientActionContinue.Checked = FCurrentCustomConfig.ClientBaseConfig.MagicActionContinue != 0;

            seClientPreTargetEmptyCount.Enabled = true;
            chkClientPreTargetCalcDir.Enabled = true;

            chkEnableAntiMagic.Enabled = false;
            chkEnableAntiMagic.Checked = false;

            chkClientNotRaiseHand.Enabled = false;
            chkClientNotRaiseHand.Checked = false;

            chkEnableHitPoint.Enabled = true;
            chkEnableHitPoint.Checked = FCurrentCustomConfig.ServerConfig.EnableHitPoint;

            FCurrentServerConfig.AttackMode = TCustomAttackMode.mamNear;
        }
        else
        {
            txtMagicWarr.Caption = "非战士技能";
            txtMagicWarr.FontColor = CustomMagicColors.clBlue;

            SetControlEnabled(grpFly, true);
            grpFly.Enabled = true;

            SetControlEnabled(grpFlyEff, true);
            grpFlyEff.Enabled = true;

            SetControlEnabled(grpMove, true);

            cbbOperateMode.Enabled = true;
            cbbMagicSwitchMode.Enabled = false;
            cbbMagicSwitchMode.ItemIndex = 0;

            chkSwitchModeNoClose.Enabled = false;
            chkSwitchModeNoClose.Checked = false;
            chkMagicAutoOpen.Enabled = false;

            // lblMagicWarrNGOption.Visible := False;   ← 原文 :2050 注释掉
            cbbMagicWarrNGOption.ItemIndex = 0;
            cbbMagicWarrNGOption.Enabled = false;

            seClientSelfPlayTime.Enabled = true;
            chkClientTargetMultiPlay.Enabled = true;
            chkClientTargetKeepPlay.Enabled = true;
            seClientTargetKeepTime.Enabled = true;
            seClientTargetKeepAttackInterval.Enabled = true;
            seClientTargetKeepAttackRange.Enabled = true;
            chkClientTargetKeepMultiPlay.Enabled = true;
            seTargetKeepLightRange.Enabled = true;

            FCurrentServerConfig.AttackMode = TCustomAttackMode.mamFar;
            FCurrentCustomConfig.ClientBaseConfig.MagicSwitchMode = TMagicSwitchMode.msmNone;

            cbbClientActionType.ItemIndex = (int)FCurrentCustomConfig.ClientBaseConfig.MagicActionType;
            cbbClientActionType.Enabled = true;

            bool actionIndexEnabled = FCurrentCustomConfig.ClientBaseConfig.MagicActionType == TMagicActionType.matCustom;
            seClientActionStartIndex.Enabled = actionIndexEnabled;
            seClientActionPlayCount.Enabled = seClientActionStartIndex.Enabled;
            seClientActionEmptyCount.Enabled = seClientActionStartIndex.Enabled;
            chkClientActionContinue.Enabled = seClientActionStartIndex.Enabled;

            seClientActionStartIndex.Value = FCurrentCustomConfig.ClientBaseConfig.MagicActionStartIndex;
            seClientActionPlayCount.Value = FCurrentCustomConfig.ClientBaseConfig.MagicActionPlayCount;
            seClientActionEmptyCount.Value = FCurrentCustomConfig.ClientBaseConfig.MagicActionEmptyCount;
            chkClientActionContinue.Checked = FCurrentCustomConfig.ClientBaseConfig.MagicActionContinue != 0;

            chkClientNotRaiseHand.Checked = FCurrentCustomConfig.ClientBaseConfig.NotRaiseHand != 0;
            chkClientNotRaiseHand.Enabled = true;

            chkEnableAntiMagic.Enabled = true;
            chkEnableAntiMagic.Checked = FCurrentCustomConfig.ServerConfig.EnableAntiMagic;

            seClientPreTargetEmptyCount.Enabled = false;
            chkClientPreTargetCalcDir.Enabled = false;

            chkEnableHitPoint.Enabled = false;
            chkEnableHitPoint.Checked = false;
        }

        // -----------------------------------server config ---------------------------
        cbbOperateMode.ItemIndex = (int)FCurrentCustomConfig.ServerConfig.OperateMode;
        CbbOperateModeChange();                       // 原文 :2094 cbbOperateMode.OnChange(cbbOperateMode)

        chkAttackUseNG.Checked = FCurrentCustomConfig.ServerConfig.IsAttackUseNG;
        chkAttackNoChangeDir.Checked = FCurrentCustomConfig.ServerConfig.NoChangeDir;
        chkDisableInSafeZone.Checked = FCurrentCustomConfig.ServerConfig.DisableInSafeZone;
        seAttackDelayTime.Value = FCurrentCustomConfig.ServerConfig.AttackDelayTime;
        seUseInterval.Value = FCurrentCustomConfig.ServerConfig.UseInterval;
        // chkFailNoShowEff.Checked := FCurrentCustomConfig.ServerConfig.FailNoShowEff;   ← 原文 :2101 注释掉
        edtFailMsg.Text = FCurrentCustomConfig.ServerConfig.FailMsg;
        edtSucceedMsg.Text = FCurrentCustomConfig.ServerConfig.SucceedMsg;
        edtCloseMsg.Text = FCurrentCustomConfig.ServerConfig.CloseMsg;

        chkCheckVarValue.Checked = FCurrentCustomConfig.ServerConfig.IsCheckVarValue;

        cbbNeedItem.ItemIndex = (int)FCurrentCustomConfig.ServerConfig.NeedItem;
        seNeedItemCount.Value = FCurrentCustomConfig.ServerConfig.NeedItemCount;
        edtNeedItemCustomItemName.Text = FCurrentCustomConfig.ServerConfig.NeedItemCustomItemName;
        chkNeedItemUseBagItem.Checked = FCurrentCustomConfig.ServerConfig.NeedItemUseBagItem;

        edtCheckVarName.Text = FCurrentCustomConfig.ServerConfig.CheckVarName;
        cbbCheckVarType.ItemIndex = (int)FCurrentCustomConfig.ServerConfig.CheckVarType;
        seCheckVarValue.Value = FCurrentCustomConfig.ServerConfig.CheckVarValue;
        seCheckVarAdd.Value = FCurrentCustomConfig.ServerConfig.CheckVarAdd;

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

        cbbAttackTarget.ItemIndex = (int)FCurrentCustomConfig.ServerConfig.AttackTarget;
        seAttackNearRange.Value = FCurrentCustomConfig.ServerConfig.AttackNearRange;
        seAttackGroupRange.Value = FCurrentCustomConfig.ServerConfig.AttackGroupRange;
        seAttackLineWidth.Value = FCurrentCustomConfig.ServerConfig.AttackLineWidth;
        chkEnableAntiMagic.Checked = FCurrentCustomConfig.ServerConfig.EnableAntiMagic;
        chkEnableHitPoint.Checked = FCurrentCustomConfig.ServerConfig.EnableHitPoint;

        cbbAttackPowerCalc.ItemIndex = (int)FCurrentCustomConfig.ServerConfig.AttackPowerCalc;
        cbbAttackPowerLevel.ItemIndex = 0;
        seAttackPowerRate.Value = FCurrentCustomConfig.ServerConfig.AttackPowerRates[0];

        lblLineAttackAddPower.Visible = IsLineAttack(FCurrentServerConfig.AttackTarget);
        seLineAttackAddPower.Visible = lblLineAttackAddPower.Visible;
        lblLineAttackAddPowerPerc.Visible = lblLineAttackAddPower.Visible;
        seLineAttackAddPower.Value = FCurrentServerConfig.AttackPowerLineAdd;
        seAttackPowerUndeadAdd.Value = FCurrentServerConfig.AttackPowerUndeadAdd;

        chkEnabledCallMonster.Checked = FCurrentServerConfig.EnabledCallMonster;
        seCallMonstersRate.Value = FCurrentServerConfig.CallMonstersRate;
        seCallMonstersRoyaltySec.Value = FCurrentServerConfig.CallMonstersRoyaltySec;
        seCallMonstersLevel.Value = FCurrentServerConfig.CallMonstersLevel;
        edtCallMonster1.Text = FCurrentServerConfig.GetCallMonster(0);
        seCallMonsterNum1.Value = FCurrentServerConfig.CallMonsterNums[0];

        edtCallMonster2.Text = FCurrentServerConfig.GetCallMonster(1);
        seCallMonsterNum2.Value = FCurrentServerConfig.CallMonsterNums[1];

        chkAttackTeleportAttack.Checked = FCurrentServerConfig.AttackTeleportAttack;
        chkNoTeleportNoAttack.Checked = FCurrentServerConfig.IsNoTeleportNoAttack;
        seAttackTeleportRate.Value = FCurrentServerConfig.AttackTeleportRate;
        chkAttackTeleportRunHum.Checked = FCurrentServerConfig.AttackTeleportRunHum;
        chkAttackTeleportRunMon.Checked = FCurrentServerConfig.AttackTeleportRunMon;
        chkAttackTeleportRunNpc.Checked = FCurrentServerConfig.AttackTeleportRunNpc;
        chkAttackTeleportRunGuard.Checked = FCurrentServerConfig.AttackTeleportRunGuard;
        chkAttackTeleportRunObstacle.Checked = FCurrentServerConfig.AttackTeleportRunObstacle;
        chkAttackTeleportWarDisHumRun.Checked = FCurrentServerConfig.AttackTeleportWarDisHumRun;
        chkAttackTeleportCannotRunItem.Checked = FCurrentServerConfig.AttackTeleportCannotRunItem;

        chkAttackTeleportRush.Checked = FCurrentServerConfig.AttackTeleportRush;
        seAttackTeleportRushCount.Value = FCurrentServerConfig.AttackTeleportRushCount;
        chkAttackTeleportAfterDamage.Checked = FCurrentServerConfig.AttackTeleportAfterDamage;

        chkProtectAddHPSlow.Checked = FCurrentCustomConfig.ServerConfig.ProtectAddHPSlow;
        seProtectAddHPSlowCount.Value = FCurrentCustomConfig.ServerConfig.ProtectAddHpSlowCount;

        chkProtectTargetStatus.Checked = FCurrentCustomConfig.ServerConfig.ProtectTargetStatus;
        seProtectTargetStatusTime.Value = FCurrentCustomConfig.ServerConfig.ProtectTargetStatusTime;
        cbbProtectTargetStatusTime_1.ItemIndex = FCurrentCustomConfig.ServerConfig.ProtectTargetStatusTimeUnit;
        seProtectTargetStatusTime_2.Value = FCurrentCustomConfig.ServerConfig.ProtectTargetStatusTime2;
        seProtectTargetStatusTimeDelay.Value = FCurrentCustomConfig.ServerConfig.ProtectTargetStatusDelay;

        seProtectTargetRange.Value = FCurrentCustomConfig.ServerConfig.ProtectTargetRange;
        // seProtectSelfRate.Value := FCurrentCustomConfig.ServerConfig.ProtectSelfRate;   ← 原文 :2187 注释掉

        var hostProtectedAddAttr = HostOf("vstProtectedAddAttr");
        hostProtectedAddAttr.Clear();
        for (int i = 0; i < CustomMagicUtilsConst.MagicProtectAddAttributesTypeCount; i++)
        {
            var addAttribType = (TMagicProtectAddAttributesType)i;
            var node = hostProtectedAddAttr.AddChild(null);
            node.CheckType = TCheckType.ctCheckBox;
            node.CheckState = FCurrentServerConfig.ProtectAddAttrib[i].IsChecked
                ? TCheckState.csCheckedNormal : TCheckState.csUnCheckedNormal;

            var addAttribData = (TProtectAddAttribData)hostProtectedAddAttr.GetNodeData(node)!;
            addAttribData.AttribType = addAttribType;
            addAttribData.Data = FCurrentServerConfig.ProtectAddAttrib[i];
        }

        var hostAddElement = HostOf("vstAddElement");
        hostAddElement.Clear();
        for (int i = 0; i < CustomMagicUtilsConst.ItemElementsTypeCount; i++)
        {
            var elementType = (TItemElementsType)i;
            var node = hostAddElement.AddChild(null);
            node.CheckType = TCheckType.ctCheckBox;
            node.CheckState = FCurrentServerConfig.ProtectAddElements[i].IsChecked
                ? TCheckState.csCheckedNormal : TCheckState.csUnCheckedNormal;

            var magicElementData = (TMagicElementData)hostAddElement.GetNodeData(node)!;
            magicElementData.ElementType = elementType;
            magicElementData.Data = FCurrentServerConfig.ProtectAddElements[i];
        }

        // MagicProtectedAddAttributesTypeNames   ← 原文 :2219 注释

        for (int i = 0; i <= 10; i++)
        {
            // 原文逐槽展开（:2221-2296），槽 4 多两项（:2255-2256）；
            // 槽 1/2/3/5/6 没有 cbbAdditionalTime*_1.ItemIndex 回填（原文如此）。
            switch (i)
            {
                case 0:
                    chkAdditional0.Checked = FCurrentServerConfig.Additionals[0].Checked;
                    seAdditionalRate0.Value = FCurrentServerConfig.Additionals[0].Rate;
                    seAdditionalRate0_2.Value = FCurrentServerConfig.Additionals[0].Rate2;
                    seAdditionalTime0.Value = FCurrentServerConfig.Additionals[0].Time;
                    cbbAdditionalTime0_1.ItemIndex = FCurrentServerConfig.Additionals[0].TimeUnit;
                    seAdditionalTime0_2.Value = FCurrentServerConfig.Additionals[0].Time2;
                    seAdditionaHP0.Value = FCurrentServerConfig.AdditionalHP0;
                    break;
                case 1:
                    chkAdditional1.Checked = FCurrentServerConfig.Additionals[1].Checked;
                    seAdditionalRate1.Value = FCurrentServerConfig.Additionals[1].Rate;
                    seAdditionalRate1_2.Value = FCurrentServerConfig.Additionals[1].Rate2;
                    seAdditionalTime1.Value = FCurrentServerConfig.Additionals[1].Time;
                    cbbAdditionalTime1_1.ItemIndex = FCurrentServerConfig.Additionals[1].TimeUnit;
                    seAdditionalTime1_2.Value = FCurrentServerConfig.Additionals[1].Time2;
                    break;
                case 2:
                    chkAdditional2.Checked = FCurrentServerConfig.Additionals[2].Checked;
                    seAdditionalRate2.Value = FCurrentServerConfig.Additionals[2].Rate;
                    seAdditionalRate2_2.Value = FCurrentServerConfig.Additionals[2].Rate2;
                    seAdditionalTime2.Value = FCurrentServerConfig.Additionals[2].Time;
                    cbbAdditionalTime2_1.ItemIndex = FCurrentServerConfig.Additionals[2].TimeUnit;
                    seAdditionalTime2_2.Value = FCurrentServerConfig.Additionals[2].Time2;
                    break;
                case 3:
                    chkAdditional3.Checked = FCurrentServerConfig.Additionals[3].Checked;
                    seAdditionalRate3.Value = FCurrentServerConfig.Additionals[3].Rate;
                    seAdditionalRate3_2.Value = FCurrentServerConfig.Additionals[3].Rate2;
                    seAdditionalTime3.Value = FCurrentServerConfig.Additionals[3].Time;
                    cbbAdditionalTime3_1.ItemIndex = FCurrentServerConfig.Additionals[3].TimeUnit;
                    seAdditionalTime3_2.Value = FCurrentServerConfig.Additionals[3].Time2;
                    break;
                case 4:
                    chkAdditional4.Checked = FCurrentServerConfig.Additionals[4].Checked;
                    seAdditionalRate4.Value = FCurrentServerConfig.Additionals[4].Rate;
                    seAdditionalRate4_2.Value = FCurrentServerConfig.Additionals[4].Rate2;
                    seAdditionalTime4.Value = FCurrentServerConfig.Additionals[4].Time;
                    seAdditionalTime4_2.Value = FCurrentServerConfig.Additionals[4].Time2;
                    chkseAdditionaHighLevel4.Checked = FCurrentServerConfig.AdditionalHighLevel4;
                    cbbPushedType4.ItemIndex = FCurrentServerConfig.AdditionalPushedType4;
                    break;
                case 5:
                    chkAdditional5.Checked = FCurrentServerConfig.Additionals[5].Checked;
                    seAdditionalRate5.Value = FCurrentServerConfig.Additionals[5].Rate;
                    seAdditionalRate5_2.Value = FCurrentServerConfig.Additionals[5].Rate2;
                    seAdditionalTime5.Value = FCurrentServerConfig.Additionals[5].Time;
                    seAdditionalTime5_2.Value = FCurrentServerConfig.Additionals[5].Time2;
                    break;
                case 6:
                    chkAdditional6.Checked = FCurrentServerConfig.Additionals[6].Checked;
                    seAdditionalRate6.Value = FCurrentServerConfig.Additionals[6].Rate;
                    seAdditionalRate6_2.Value = FCurrentServerConfig.Additionals[6].Rate2;
                    seAdditionalTime6.Value = FCurrentServerConfig.Additionals[6].Time;
                    seAdditionalTime6_2.Value = FCurrentServerConfig.Additionals[6].Time2;
                    break;
                case 7:
                    chkAdditional7.Checked = FCurrentServerConfig.Additionals[7].Checked;
                    seAdditionalRate7.Value = FCurrentServerConfig.Additionals[7].Rate;
                    seAdditionalRate7_2.Value = FCurrentServerConfig.Additionals[7].Rate2;
                    seAdditionalTime7.Value = FCurrentServerConfig.Additionals[7].Time;
                    cbbAdditionalTime7_1.ItemIndex = FCurrentServerConfig.Additionals[7].TimeUnit;
                    seAdditionalTime7_2.Value = FCurrentServerConfig.Additionals[7].Time2;
                    break;
                case 8:
                    chkAdditional8.Checked = FCurrentServerConfig.Additionals[8].Checked;
                    seAdditionalRate8.Value = FCurrentServerConfig.Additionals[8].Rate;
                    seAdditionalRate8_2.Value = FCurrentServerConfig.Additionals[8].Rate2;
                    seAdditionalTime8.Value = FCurrentServerConfig.Additionals[8].Time;
                    cbbAdditionalTime8_1.ItemIndex = FCurrentServerConfig.Additionals[8].TimeUnit;
                    seAdditionalTime8_2.Value = FCurrentServerConfig.Additionals[8].Time2;
                    break;
                case 9:
                    chkAdditional9.Checked = FCurrentServerConfig.Additionals[9].Checked;
                    seAdditionalRate9.Value = FCurrentServerConfig.Additionals[9].Rate;
                    seAdditionalRate9_2.Value = FCurrentServerConfig.Additionals[9].Rate2;
                    seAdditionalTime9.Value = FCurrentServerConfig.Additionals[9].Time;
                    cbbAdditionalTime9_1.ItemIndex = FCurrentServerConfig.Additionals[9].TimeUnit;
                    seAdditionalTime9_2.Value = FCurrentServerConfig.Additionals[9].Time2;
                    break;
                case 10:
                    chkAdditional10.Checked = FCurrentServerConfig.Additionals[10].Checked;
                    seAdditionalRate10.Value = FCurrentServerConfig.Additionals[10].Rate;
                    seAdditionalRate10_2.Value = FCurrentServerConfig.Additionals[10].Rate2;
                    seAdditionalTime10.Value = FCurrentServerConfig.Additionals[10].Time;
                    cbbAdditionalTime10_1.ItemIndex = FCurrentServerConfig.Additionals[10].TimeUnit;
                    seAdditionalTime10_2.Value = FCurrentServerConfig.Additionals[10].Time2;
                    break;
            }
        }

        chkAttackTargetStatus.Checked = FCurrentCustomConfig.ServerConfig.AttackTargetStatus;
        seAttackTargetStatusTime.Value = FCurrentCustomConfig.ServerConfig.AttackTargetStatusTime;
        cbbAttackTargetStatusTime_1.ItemIndex = FCurrentCustomConfig.ServerConfig.AttackTargetStatusTimeUnit;
        seAttackTargetStatusTime_2.Value = FCurrentCustomConfig.ServerConfig.AttackTargetStatusTime2;
        seAttackTargetStatusDelay.Value = FCurrentCustomConfig.ServerConfig.AttackTargetStatusDelay;

        var hostAttackDecAttr = HostOf("vstAttackDecAttr");
        hostAttackDecAttr.Clear();
        for (int i = 0; i < CustomMagicUtilsConst.MagicAttackDecAttributesTypeCount; i++)
        {
            var decAttribType = (TMagicAttackDecAttributesType)i;
            var node = hostAttackDecAttr.AddChild(null);
            node.CheckType = TCheckType.ctCheckBox;
            node.CheckState = FCurrentServerConfig.AttackSubAttrib[i].IsChecked
                ? TCheckState.csCheckedNormal : TCheckState.csUnCheckedNormal;

            var decAttribData = (TAttackDecAttribData)hostAttackDecAttr.GetNodeData(node)!;
            decAttribData.AttribType = decAttribType;
            decAttribData.Data = FCurrentServerConfig.AttackSubAttrib[i];
        }

        var hostDecElement = HostOf("vstDecElement");
        hostDecElement.Clear();
        for (int i = 0; i < CustomMagicUtilsConst.ItemElementsTypeCount; i++)
        {
            var elementType = (TItemElementsType)i;
            var node = hostDecElement.AddChild(null);
            node.CheckType = TCheckType.ctCheckBox;
            node.CheckState = FCurrentServerConfig.AttackSubElements[i].IsChecked
                ? TCheckState.csCheckedNormal : TCheckState.csUnCheckedNormal;

            var magicElementData = (TMagicElementData)hostDecElement.GetNodeData(node)!;
            magicElementData.ElementType = elementType;
            magicElementData.Data = FCurrentServerConfig.AttackSubElements[i];
        }

        // 破防 6 组（原文 :2334-2368 逐组展开，组与控件名一一对应）
        ApplyBreakDefenseRow(FCurrentCustomConfig.ServerConfig.AttackBreakDefense[(int)TBreakDefenseType.bdtHumDefense],
            chkMagicACHum, seMagicACHumRate, seMagicACHumRateAdd, seMagicACHumValue, seMagicACHumValueAdd);
        ApplyBreakDefenseRow(FCurrentCustomConfig.ServerConfig.AttackBreakDefense[(int)TBreakDefenseType.bdtMonDefense],
            chkMagicACMon, seMagicACMonRate, seMagicACMonRateAdd, seMagicACMonValue, seMagicACMonValueAdd);
        ApplyBreakDefenseRow(FCurrentCustomConfig.ServerConfig.AttackBreakDefense[(int)TBreakDefenseType.bdtHeroDefense],
            chkMagicACHero, seMagicACHeroRate, seMagicACHeroRateAdd, seMagicACHeroValue, seMagicACHeroValueAdd);
        ApplyBreakDefenseRow(FCurrentCustomConfig.ServerConfig.AttackBreakDefense[(int)TBreakDefenseType.bdtHumMagDefense],
            chkDefenceHum, seDefenceHumRate, seDefenceHumRateAdd, seDefenceHumValue, seDefenceHumValueAdd);
        ApplyBreakDefenseRow(FCurrentCustomConfig.ServerConfig.AttackBreakDefense[(int)TBreakDefenseType.bdtMonMagDefense],
            chkDefenceMon, seDefenceMonRate, seDefenceMonRateAdd, seDefenceMonValue, seDefenceMonValueAdd);
        ApplyBreakDefenseRow(FCurrentCustomConfig.ServerConfig.AttackBreakDefense[(int)TBreakDefenseType.bdtHeroMagDefense],
            chkDefenceHero, seDefenceHeroRate, seDefenceHeroRateAdd, seDefenceHeroValue, seDefenceHeroValueAdd);

        // 处理直接选中"直线攻击"，无法触发响应事件的问题 Cursor 2023-06-07 11:21:40
        CbbAttackTargetChange();                      // 原文 :2370 cbbAttackTargetChange(nil)

        SetConfigChanged(oldChanged);
        FIsConfigChanged = oldIsConfigCanSave;
        if (!FIsConfigChanged)
            btnSave.Enabled = false;
    }

    /// <summary>原文 :2334-2368 六组破防控件的统一回填形状。</summary>
    private static void ApplyBreakDefenseRow(TBreakDefenseInfo d, TCheckBoxSeam chk,
        TSpinEditExSeam rate, TSpinEditExSeam rateAdd, TSpinEditExSeam value, TSpinEditExSeam valueAdd)
    {
        chk.Checked = d.IsChecked;
        rate.Value = d.Rate;
        rateAdd.Value = d.RateAdd;
        value.Value = d.Value;
        valueAdd.Value = d.ValueAdd;
    }

    /// <summary>原文 <c>AttackTarget in [matLine, matDir8, matDir16]</c>（:2146/:3250）。</summary>
    internal static bool IsLineAttack(TCustomAttackTarget t)
        => t == TCustomAttackTarget.matLine || t == TCustomAttackTarget.matDir8 || t == TCustomAttackTarget.matDir16;
}

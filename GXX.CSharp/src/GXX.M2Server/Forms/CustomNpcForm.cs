using System;
using System.Collections.Generic;
using System.Globalization;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// uFrmCustomNpc.pas 1:1（批次J66，1496 行）：自定义 NPC 设置窗体。
/// 左侧 vstCustomNpc（节点 = TCustomNpcConfig，已改行标红）；右侧 vstNpcAction 8 方向 × 2 行
/// （站立行可勾选 / 动作行不可勾选），列 0=方向（Node.Index div 2）、1=站立·动作、
/// 2=图库文件、3=起始图、4=张数、5=间隔、6=特效文件、7=特效起始图；
/// 站立行单元格底色 $00F2E4D8；列 &gt; 1 可编辑；列 7 提示『特效开始图片为-1表示不使用特效』；
/// 血条/常驻特效/绘制顺序（6 种顺序 → 3 项列表框 + 上移/下移重算 DrawOrder）；
/// 批量填充图库/间隔、四路等差数列计算（起始图 + (张数+空帧) × (i-1)）；
/// 保存：逐节点 SaveToIniFile + 重建客户端压缩块 + 落 CustomNpcMoveTime；
/// 另存：SaveCustomNpcClientConfigs → .dat。
/// </summary>
public sealed class CustomNpcForm : System.Windows.Forms.Form
{
    /// <summary>PNpcConfigNodeData：左侧节点数据。</summary>
    public sealed class TNpcConfigNodeData
    {
        public TCustomNpcConfig Config = null!;
    }

    /// <summary>PNpcNodeData：右侧节点数据（Action 指向所属配置的方向条目）。</summary>
    public sealed class TNpcNodeData
    {
        public TCustomNpcConfig Owner = null!;
        public int DirIndex;
        public TNpcActionType ActionType;
    }

    // ---- 控件（按 DFM 命名） ----
    public System.Windows.Forms.ListView vstCustomNpc = null!;
    public System.Windows.Forms.ListView vstNpcAction = null!;
    public System.Windows.Forms.Panel pnlNpc = null!;
    public System.Windows.Forms.Panel pnlNpcClient = null!;
    public System.Windows.Forms.Panel pnlNpcBottom = null!;

    public System.Windows.Forms.ComboBox cbbNpcHPFile = null!;
    public System.Windows.Forms.NumericUpDown seNpcHPStartIndex = null!;
    public System.Windows.Forms.NumericUpDown seNpcHPBgOffsetX = null!;
    public System.Windows.Forms.NumericUpDown seNpcHPBgOffsetY = null!;
    public System.Windows.Forms.NumericUpDown seNpcHPOffsetX = null!;
    public System.Windows.Forms.NumericUpDown seNpcHPOffsetY = null!;
    public System.Windows.Forms.NumericUpDown seNpcHPTextOffsetX = null!;
    public System.Windows.Forms.NumericUpDown seNpcHPTextOffsetY = null!;

    public System.Windows.Forms.ComboBox cbbNpcStandDrawMode = null!;
    public System.Windows.Forms.ComboBox cbbNpcStandEffectDrawMode = null!;
    public System.Windows.Forms.ComboBox cbbNpcActionDrawMode = null!;
    public System.Windows.Forms.ComboBox cbbNpcActionEffectDrawMode = null!;

    public System.Windows.Forms.ComboBox cbbNpcKeepPlayFile = null!;
    public System.Windows.Forms.NumericUpDown seNpcKeepPlayIndex = null!;
    public System.Windows.Forms.NumericUpDown seNpcKeepPlayCount = null!;
    public System.Windows.Forms.NumericUpDown seNpcKeepPlayTime = null!;
    public System.Windows.Forms.CheckBox chkNpcKeepPlayBlendDraw = null!;
    public System.Windows.Forms.NumericUpDown seKeepPlayOffsetX = null!;
    public System.Windows.Forms.NumericUpDown seKeepPlayOffsetY = null!;

    public System.Windows.Forms.ListBox lstNpcDrawOrder = null!;
    public System.Windows.Forms.Button btnNpcMoveTop = null!;
    public System.Windows.Forms.Button btnNpcMoveBottom = null!;

    public System.Windows.Forms.ComboBox cbbNpcBatchFile = null!;
    public System.Windows.Forms.NumericUpDown seNpcBatchTime = null!;
    public System.Windows.Forms.Button btnNpcFileStand = null!;
    public System.Windows.Forms.Button btnNpcFileStandEffect = null!;
    public System.Windows.Forms.Button btnNpcFileAction = null!;
    public System.Windows.Forms.Button btnNpcFileActionEffect = null!;
    public System.Windows.Forms.Button btnNpcTimeStand = null!;
    public System.Windows.Forms.Button btnNpcTimeAction = null!;

    public System.Windows.Forms.NumericUpDown seNpcCalcStartIndex = null!;
    public System.Windows.Forms.NumericUpDown seNpcCalcPlayCount = null!;
    public System.Windows.Forms.NumericUpDown seNpcCalcEmptyCount = null!;
    public System.Windows.Forms.NumericUpDown seNpcCalcDirCount = null!;
    public System.Windows.Forms.Button btnNpcCalcStand = null!;
    public System.Windows.Forms.Button btnNpcCalcStandEffect = null!;
    public System.Windows.Forms.Button btnNpcCalcHit = null!;
    public System.Windows.Forms.Button btnNpcCalcHitEffect = null!;

    public System.Windows.Forms.CheckBox chkSendCustomNPCConfig = null!;
    public System.Windows.Forms.NumericUpDown seCustomNpcMoveTime = null!;
    public System.Windows.Forms.Button btnSaveNpc = null!;
    public System.Windows.Forms.Button btnSaveNpcToFile = null!;

    /// <summary>g_EffectImageList（WIL 名单，ViewList2 页维护）。</summary>
    public List<string> EffectImageList = ViewList2State.g_EffectImageList;

    /// <summary>UserEngine.m_CustomNpcList（Delphi 由引擎持有；此处默认接 CustomNpcUtils.CustomNpcList）。</summary>
    public IList<TCustomNpcConfig> CustomNpcList = CustomNpcUtils.CustomNpcList;

    /// <summary>FIsNpcChanged / FCurrentNpcCustomConfig。</summary>
    public bool FIsNpcChanged;
    public TCustomNpcConfig? FCurrentNpcCustomConfig;

    /// <summary>Config.WriteInteger('Setup', key, value) 接缝。</summary>
    public Action<string, int>? WriteIntegerHandler;

    /// <summary>Config.WriteBool('Setup', key, value) 接缝。</summary>
    public Action<string, bool>? WriteBoolHandler;

    /// <summary>Config.WriteString('Setup', key, value) 接缝。</summary>
    public Action<string, string>? WriteStringHandler;

    /// <summary>另存对话框接缝（返回 null 表示取消）。</summary>
    public Func<string, string?>? SaveFileDialogHandler;

    /// <summary>Showmessage 接缝。</summary>
    public Action<string>? ShowMessageHandler;

    /// <summary>gzLibCompressBuffer 接缝（返回压缩数据）。</summary>
    public Func<byte[], byte[]>? CompressHandler;

    /// <summary>最近一次重建的客户端压缩块（g_CustomNpcListText 等效）。</summary>
    public byte[] CustomNpcListText = Array.Empty<byte>();
    public int CustomNpcListTextLen;
    public uint CustomNpcListTextCrc;

    /// <summary>最近一次编辑请求（WM_STARTEDITING_NPC 等效）。</summary>
    public (int Index, int Column)? LastEditingRequest;

    /// <summary>TCustomDrawModeNames（DFM 下拉文案，顺序同枚举）。</summary>
    public static readonly string[] CustomDrawModeNames = { "混合", "普通" };

    /// <summary>绘制顺序三项文案（下标 = 对象值 0 持久播放 / 1 角色绘制 / 2 特效播放）。</summary>
    public static readonly string[] DrawOrderNames = { "持久播放", "角色绘制", "特效播放" };

    public CustomNpcForm()
    {
        InitializeComponent();
        FormCreate();
    }

    private static System.Windows.Forms.NumericUpDown NewSpin(decimal min = -32768, decimal max = 32767)
        => new() { Minimum = min, Maximum = max, Width = 80 };

    private void InitializeComponent()
    {
        Text = "自定义NPC设置";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        ClientSize = new System.Drawing.Size(900, 620);

        vstCustomNpc = NewList(new[] { "外观" }, new[] { 140 });
        vstCustomNpc.Width = 160;
        vstCustomNpc.SelectedIndexChanged += (_, _) => VstCustomNpcNodeClick();

        vstNpcAction = NewList(
            new[] { "方向", "动作", "图库文件", "起始图", "张数", "间隔", "特效文件", "特效起始图" },
            new[] { 90, 60, 130, 70, 60, 60, 130, 90 });
        vstNpcAction.Width = 660;
        vstNpcAction.ItemChecked += (_, e) => VstNpcActionChecked(e.Item.Index);
        vstNpcAction.Click += (_, _) => VstNpcActionNodeClick();

        cbbNpcHPFile = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Width = 130 };
        cbbNpcHPFile.SelectedIndexChanged += (_, _) => CbbNpcHPFileChange();
        cbbNpcKeepPlayFile = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Width = 130 };
        cbbNpcKeepPlayFile.SelectedIndexChanged += (_, _) => CbbNpcKeepPlayFileChange();
        cbbNpcBatchFile = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Width = 130 };

        cbbNpcStandDrawMode = NewDrawModeCombo();
        cbbNpcStandDrawMode.SelectedIndexChanged += (_, _) => CbbNpcStandDrawModeChange();
        cbbNpcStandEffectDrawMode = NewDrawModeCombo();
        cbbNpcStandEffectDrawMode.SelectedIndexChanged += (_, _) => CbbNpcStandEffectDrawModeChange();
        cbbNpcActionDrawMode = NewDrawModeCombo();
        cbbNpcActionDrawMode.SelectedIndexChanged += (_, _) => CbbNpcActionDrawModeChange();
        cbbNpcActionEffectDrawMode = NewDrawModeCombo();
        cbbNpcActionEffectDrawMode.SelectedIndexChanged += (_, _) => CbbNpcActionEffectDrawModeChange();

        seNpcHPStartIndex = NewSpin(); seNpcHPStartIndex.ValueChanged += (_, _) => SeNpcHPStartIndexChange();
        seNpcHPBgOffsetX = NewSpin(); seNpcHPBgOffsetX.ValueChanged += (_, _) => SeNpcHPBgOffsetXChange();
        seNpcHPBgOffsetY = NewSpin(); seNpcHPBgOffsetY.ValueChanged += (_, _) => SeNpcHPBgOffsetYChange();
        seNpcHPOffsetX = NewSpin(); seNpcHPOffsetX.ValueChanged += (_, _) => SeNpcHPOffsetXChange();
        seNpcHPOffsetY = NewSpin(); seNpcHPOffsetY.ValueChanged += (_, _) => SeNpcHPOffsetYChange();
        seNpcHPTextOffsetX = NewSpin(); seNpcHPTextOffsetX.ValueChanged += (_, _) => SeNpcHPTextOffsetXChange();
        seNpcHPTextOffsetY = NewSpin(); seNpcHPTextOffsetY.ValueChanged += (_, _) => SeNpcHPTextOffsetYChange();

        seNpcKeepPlayIndex = NewSpin(); seNpcKeepPlayIndex.ValueChanged += (_, _) => SeNpcKeepPlayIndexChange();
        seNpcKeepPlayCount = NewSpin(0); seNpcKeepPlayCount.ValueChanged += (_, _) => SeNpcKeepPlayCountChange();
        seNpcKeepPlayTime = NewSpin(0); seNpcKeepPlayTime.ValueChanged += (_, _) => SeNpcKeepPlayTimeChange();
        chkNpcKeepPlayBlendDraw = new System.Windows.Forms.CheckBox { Text = "混合绘制", AutoSize = true };
        chkNpcKeepPlayBlendDraw.CheckedChanged += (_, _) => ChkNpcKeepPlayBlendDrawClick();
        seKeepPlayOffsetX = NewSpin(); seKeepPlayOffsetX.ValueChanged += (_, _) => SeKeepPlayOffsetChanged();
        seKeepPlayOffsetY = NewSpin(); seKeepPlayOffsetY.ValueChanged += (_, _) => SeKeepPlayOffsetChanged();

        lstNpcDrawOrder = new System.Windows.Forms.ListBox { Width = 140, Height = 70 };
        btnNpcMoveTop = new System.Windows.Forms.Button { Text = "上移", Width = 60 };
        btnNpcMoveTop.Click += (_, _) => BtnNpcMoveClick(up: true);
        btnNpcMoveBottom = new System.Windows.Forms.Button { Text = "下移", Width = 60 };
        btnNpcMoveBottom.Click += (_, _) => BtnNpcMoveClick(up: false);

        seNpcBatchTime = NewSpin(0);
        btnNpcFileStand = new System.Windows.Forms.Button { Text = "站立图库", Width = 90 };
        btnNpcFileStand.Click += (_, _) => BtnNpcFileClick("stand");
        btnNpcFileStandEffect = new System.Windows.Forms.Button { Text = "站立特效", Width = 90 };
        btnNpcFileStandEffect.Click += (_, _) => BtnNpcFileClick("standEffect");
        btnNpcFileAction = new System.Windows.Forms.Button { Text = "动作图库", Width = 90 };
        btnNpcFileAction.Click += (_, _) => BtnNpcFileClick("action");
        btnNpcFileActionEffect = new System.Windows.Forms.Button { Text = "动作特效", Width = 90 };
        btnNpcFileActionEffect.Click += (_, _) => BtnNpcFileClick("actionEffect");
        btnNpcTimeStand = new System.Windows.Forms.Button { Text = "站立间隔", Width = 90 };
        btnNpcTimeStand.Click += (_, _) => BtnNpcTimeClick("stand");
        btnNpcTimeAction = new System.Windows.Forms.Button { Text = "动作间隔", Width = 90 };
        btnNpcTimeAction.Click += (_, _) => BtnNpcTimeClick("action");

        seNpcCalcStartIndex = NewSpin();
        seNpcCalcPlayCount = NewSpin(0);
        seNpcCalcEmptyCount = NewSpin(0);
        seNpcCalcDirCount = NewSpin(0, 8);
        btnNpcCalcStand = new System.Windows.Forms.Button { Text = "站立计算", Width = 90 };
        btnNpcCalcStand.Click += (_, _) => BtnNpcCalcClick("stand");
        btnNpcCalcStandEffect = new System.Windows.Forms.Button { Text = "站立特效计算", Width = 100 };
        btnNpcCalcStandEffect.Click += (_, _) => BtnNpcCalcClick("standEffect");
        btnNpcCalcHit = new System.Windows.Forms.Button { Text = "动作计算", Width = 90 };
        btnNpcCalcHit.Click += (_, _) => BtnNpcCalcClick("action");
        btnNpcCalcHitEffect = new System.Windows.Forms.Button { Text = "动作特效计算", Width = 100 };
        btnNpcCalcHitEffect.Click += (_, _) => BtnNpcCalcClick("actionEffect");

        chkSendCustomNPCConfig = new System.Windows.Forms.CheckBox { Text = "下发自定义NPC配置", AutoSize = true };
        chkSendCustomNPCConfig.CheckedChanged += (_, _) => ChkSendCustomNpcConfigClick();
        seCustomNpcMoveTime = NewSpin(0);
        btnSaveNpc = new System.Windows.Forms.Button { Text = "保存", Width = 90, Enabled = false };
        btnSaveNpc.Click += (_, _) => BtnSaveNpcClick();
        btnSaveNpcToFile = new System.Windows.Forms.Button { Text = "另存", Width = 90 };
        btnSaveNpcToFile.Click += (_, _) => BtnSaveNpcToFileClick();

        pnlNpc = new System.Windows.Forms.Panel { Dock = System.Windows.Forms.DockStyle.Right, Width = 720 };
        pnlNpcClient = new System.Windows.Forms.Panel { Dock = System.Windows.Forms.DockStyle.Top, Height = 460 };
        pnlNpcBottom = new System.Windows.Forms.Panel { Dock = System.Windows.Forms.DockStyle.Bottom, Height = 60 };

        pnlNpcClient.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            vstNpcAction, cbbNpcHPFile, seNpcHPStartIndex, seNpcHPBgOffsetX, seNpcHPBgOffsetY,
            seNpcHPOffsetX, seNpcHPOffsetY, seNpcHPTextOffsetX, seNpcHPTextOffsetY,
            cbbNpcStandDrawMode, cbbNpcStandEffectDrawMode, cbbNpcActionDrawMode, cbbNpcActionEffectDrawMode,
            cbbNpcKeepPlayFile, seNpcKeepPlayIndex, seNpcKeepPlayCount, seNpcKeepPlayTime,
            chkNpcKeepPlayBlendDraw, seKeepPlayOffsetX, seKeepPlayOffsetY,
            lstNpcDrawOrder, btnNpcMoveTop, btnNpcMoveBottom,
            cbbNpcBatchFile, seNpcBatchTime, btnNpcFileStand, btnNpcFileStandEffect,
            btnNpcFileAction, btnNpcFileActionEffect, btnNpcTimeStand, btnNpcTimeAction,
            seNpcCalcStartIndex, seNpcCalcPlayCount, seNpcCalcEmptyCount, seNpcCalcDirCount,
            btnNpcCalcStand, btnNpcCalcStandEffect, btnNpcCalcHit, btnNpcCalcHitEffect,
        });
        pnlNpcBottom.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            chkSendCustomNPCConfig, seCustomNpcMoveTime, btnSaveNpc, btnSaveNpcToFile,
        });
        pnlNpc.Controls.Add(pnlNpcClient);
        pnlNpc.Controls.Add(pnlNpcBottom);
        Controls.Add(pnlNpc);
        Controls.Add(vstCustomNpc);
    }

    private static System.Windows.Forms.ListView NewList(string[] headers, int[] widths)
    {
        var lv = new System.Windows.Forms.ListView
        {
            View = System.Windows.Forms.View.Details,
            FullRowSelect = true,
            GridLines = true,
            HideSelection = false,
        };
        for (int i = 0; i < headers.Length; i++)
            lv.Columns.Add(headers[i], widths[i]);
        return lv;
    }

    private static System.Windows.Forms.ComboBox NewDrawModeCombo()
    {
        var cbb = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Width = 80 };
        cbb.Items.AddRange(CustomDrawModeNames);
        return cbb;
    }

    /// <summary>SetControlEnabled 等效（pnlNpc 及其子控件整体使能/禁用）。</summary>
    public static void SetControlEnabled(System.Windows.Forms.Control control, bool enabled)
    {
        control.Enabled = enabled;
        foreach (System.Windows.Forms.Control child in control.Controls)
            SetControlEnabled(child, enabled);
    }

    // ==================== FormCreate / Open ====================

    /// <summary>FormCreate（638-691）1:1：三个图库下拉填 g_EffectImageList + 四个绘制模式下拉填 2 项 + 禁用右栏。</summary>
    public void FormCreate()
    {
        cbbNpcHPFile.Items.Clear();
        foreach (var name in EffectImageList)
            cbbNpcHPFile.Items.Add(name);

        cbbNpcKeepPlayFile.Items.Clear();
        foreach (var name in EffectImageList)
            cbbNpcKeepPlayFile.Items.Add(name);

        cbbNpcBatchFile.Items.Clear();
        foreach (var name in EffectImageList)
            cbbNpcBatchFile.Items.Add(name);

        foreach (var cbb in new[] { cbbNpcStandDrawMode, cbbNpcStandEffectDrawMode, cbbNpcActionDrawMode, cbbNpcActionEffectDrawMode })
        {
            cbb.Items.Clear();
            for (int mode = (int)TCustomDrawMode.mdmBlend; mode <= (int)TCustomDrawMode.mdmNormal; mode++)
                cbb.Items.Add(CustomDrawModeNames[mode]);
        }

        SetControlEnabled(pnlNpc, false);
        FCurrentNpcCustomConfig = null;
    }

    /// <summary>Open（608-636）1:1：装载 m_CustomNpcList → vstCustomNpc 节点 + 回填两开关。</summary>
    public void Open()
    {
        FIsNpcChanged = false;
        vstCustomNpc.Items.Clear();

        foreach (var config in CustomNpcList)
        {
            var item = new System.Windows.Forms.ListViewItem(config.NpcAppr.ToString(CultureInfo.InvariantCulture))
            {
                Tag = new TNpcConfigNodeData { Config = config },
            };
            vstCustomNpc.Items.Add(item);
        }

        chkSendCustomNPCConfig.Checked = M2Config.boSendCustomNpcConfig;
        seCustomNpcMoveTime.Value = Clamp(M2Config.dwCustomNpcMoveTime, seCustomNpcMoveTime);
    }

    private static decimal Clamp(int value, System.Windows.Forms.NumericUpDown control)
        => Math.Clamp((decimal)value, control.Minimum, control.Maximum);

    // ==================== 左栏 ====================

    /// <summary>GetText（802-810）：节点文本 = NpcAppr。</summary>
    public static string CustomNpcCellText(TNpcConfigNodeData nodeData)
        => nodeData.Config.NpcAppr.ToString(CultureInfo.InvariantCulture);

    /// <summary>DrawText（780-795）1:1：IsChanged 标红 / 选中聚焦用高亮文字 / 否则默认色。</summary>
    public System.Drawing.Color CustomNpcRowColor(TNpcConfigNodeData nodeData, bool selectedAndFocused)
    {
        if (nodeData.Config.IsChanged)
            return System.Drawing.Color.Red;
        if (selectedAndFocused)
            return System.Drawing.SystemColors.HighlightText;
        return vstCustomNpc.ForeColor;
    }

    /// <summary>SetNpcConfigChanged（766-778）1:1：置 IsChanged + 刷新 + FIsNpcChanged + 使能保存。</summary>
    public void SetNpcConfigChanged(bool isChanged = true)
    {
        if (FCurrentNpcCustomConfig == null)
            return;
        FCurrentNpcCustomConfig.SetChanged(isChanged);
        FIsNpcChanged = true;
        if (!btnSaveNpc.Enabled)
            btnSaveNpc.Enabled = true;
    }

    /// <summary>
    /// 选中左侧第 index 行并触发 NodeClick（等效「点击节点」；index &lt; 0 表示无选中）。
    /// WinForms 的 SelectedIndices/FocusedItem 在无窗口句柄时不可靠，故提供显式入口，
    /// VstCustomNpcNodeClick 会优先读取它。
    /// </summary>
    public void SelectCustomNpcRow(int index)
    {
        _explicitRow = index;
        VstCustomNpcNodeClick();
    }

    private int _explicitRow = -2;   // -2 = 未显式指定

    /// <summary>
    /// vstCustomNpcNodeClick（812-927）1:1：清右栏 → 建 16 行（8 方向 × 站立/动作，站立行可勾选）
    /// → 回填血条/绘制模式/常驻特效 → 按 DrawOrder 装 3 项绘制顺序 → 恢复保存按钮态。
    /// </summary>
    public void VstCustomNpcNodeClick()
    {
        vstNpcAction.Items.Clear();
        FCurrentNpcCustomConfig = null;

        // Delphi: vstCustomNpc.FocusedNode（焦点节点）。取行顺序：
        // ① 显式入口（测试/程序化调用）② WinForms FocusedItem ③ 选中项扫描
        int focused = _explicitRow != -2
            ? _explicitRow
            : vstCustomNpc.FocusedItem?.Index
              ?? (vstCustomNpc.SelectedIndices.Count > 0 ? vstCustomNpc.SelectedIndices[0] : -1);
        _explicitRow = -2;
        if (focused < 0 || focused >= vstCustomNpc.Items.Count)
            return;
        SetControlEnabled(pnlNpc, true);

        if (vstCustomNpc.Items[focused].Tag is not TNpcConfigNodeData configNodeData)
            return;
        bool oldIsConfigCanSave = FIsNpcChanged;

        FCurrentNpcCustomConfig = configNodeData.Config;
        bool oldChanged = FCurrentNpcCustomConfig.IsChanged;

        for (int dir = 0; dir < 8; dir++)
        {
            AddActionRow(dir, TNpcActionType.atStand, checkable: true);
            AddActionRow(dir, TNpcActionType.atAction, checkable: false);
        }

        var b = FCurrentNpcCustomConfig.ClientBaseConfig;
        // g_EffectImageList 为空时 WinForms 拒绝越界 SelectedIndex（Delphi 会静默忽略），故加守卫
        cbbNpcHPFile.SelectedIndex = b.HPFile >= 0 && b.HPFile < cbbNpcHPFile.Items.Count ? b.HPFile : -1;
        seNpcHPStartIndex.Value = Clamp(b.HPStartIndex, seNpcHPStartIndex);
        seNpcHPBgOffsetX.Value = Clamp(b.HPBgOffsetX, seNpcHPBgOffsetX);
        seNpcHPBgOffsetY.Value = Clamp(b.HPBgOffsetY, seNpcHPBgOffsetY);
        seNpcHPOffsetX.Value = Clamp(b.HPOffsetX, seNpcHPOffsetX);
        seNpcHPOffsetY.Value = Clamp(b.HPOffsetY, seNpcHPOffsetY);
        seNpcHPTextOffsetX.Value = Clamp(b.HPTextOffsetX, seNpcHPTextOffsetX);
        seNpcHPTextOffsetY.Value = Clamp(b.HPTextOffsetY, seNpcHPTextOffsetY);

        cbbNpcStandDrawMode.SelectedIndex = (int)b.StandDrawMode;
        cbbNpcStandEffectDrawMode.SelectedIndex = (int)b.StandEffectDrawMode;
        cbbNpcActionDrawMode.SelectedIndex = (int)b.ActionDrawMode;
        cbbNpcActionEffectDrawMode.SelectedIndex = (int)b.ActionEffectDrawMode;

        cbbNpcKeepPlayFile.SelectedIndex = b.KeepPlayFile >= 0 && b.KeepPlayFile < cbbNpcKeepPlayFile.Items.Count ? b.KeepPlayFile : -1;
        seNpcKeepPlayIndex.Value = Clamp(b.KeepPlayIndex, seNpcKeepPlayIndex);
        seNpcKeepPlayCount.Value = Clamp(b.KeepPlayCount, seNpcKeepPlayCount);
        seNpcKeepPlayTime.Value = Clamp(b.KeepPlayTime, seNpcKeepPlayTime);
        chkNpcKeepPlayBlendDraw.Checked = b.KeepPlayBlendDraw != 0;
        seKeepPlayOffsetX.Value = Clamp(b.KeepPlayOffsetX, seKeepPlayOffsetX);
        seKeepPlayOffsetY.Value = Clamp(b.KeepPlayOffsetY, seKeepPlayOffsetY);

        lstNpcDrawOrder.Items.Clear();
        foreach (int obj in DrawOrderToSequence(b.DrawOrder))
            lstNpcDrawOrder.Items.Add(DrawOrderNames[obj]);

        SetNpcConfigChanged(oldChanged);

        FIsNpcChanged = oldIsConfigCanSave;
        if (!FIsNpcChanged)
            btnSaveNpc.Enabled = false;
    }

    /// <summary>DrawOrder → 三项对象值序列（833-920 六个分支 1:1）。</summary>
    public static int[] DrawOrderToSequence(TCustomNpcDrawOrder drawOrder) => drawOrder switch
    {
        TCustomNpcDrawOrder.ndoKeep_Chr_Eff => new[] { 0, 1, 2 },
        TCustomNpcDrawOrder.ndoKeep_Eff_Chr => new[] { 0, 2, 1 },
        TCustomNpcDrawOrder.ndoChr_Keep_Eff => new[] { 1, 0, 2 },
        TCustomNpcDrawOrder.ndoChr_Eff_Keep => new[] { 1, 2, 0 },
        TCustomNpcDrawOrder.ndoEff_Keep_Chr => new[] { 2, 0, 1 },
        TCustomNpcDrawOrder.ndoEff_Chr_Keep => new[] { 2, 1, 0 },
        _ => new[] { 0, 1, 2 },
    };

    private void AddActionRow(int dirIndex, TNpcActionType actionType, bool checkable)
    {
        var nodeData = new TNpcNodeData
        {
            Owner = FCurrentNpcCustomConfig!,
            DirIndex = dirIndex,
            ActionType = actionType,
        };
        string dirText = actionType == TNpcActionType.atStand ? CustomNpcUtils.NpcDirNames[dirIndex] : "";
        var item = new System.Windows.Forms.ListViewItem(dirText) { Tag = nodeData, Checked = false };
        item.SubItems.Add(CustomNpcUtils.NpcActionNames[(int)actionType]);
        for (int col = 2; col < 8; col++)
            item.SubItems.Add(ActionCellText(nodeData, col));
        if (checkable)
        {
            var dirAction = nodeData.Owner.DirActions[dirIndex];
            item.Checked = dirAction.Enabled != 0;
        }
        vstNpcAction.Items.Add(item);
    }

    // ==================== 右栏文本 ====================

    /// <summary>GetText（981-1055）1:1：列 0 = NpcDirNames[Node.Index div 2]（仅站立行）、列 1 = 站立/动作。</summary>
    public static string ActionCellText(TNpcNodeData nodeData, int column)
    {
        var action = nodeData.Owner.DirActions[nodeData.DirIndex];
        bool isStand = nodeData.ActionType == TNpcActionType.atStand;
        switch (column)
        {
            case 0:
                return isStand ? CustomNpcUtils.NpcDirNames[nodeData.DirIndex] : "";
            case 1:
                return CustomNpcUtils.NpcActionNames[(int)nodeData.ActionType];
            case 2:
                return EffectImageName(isStand ? action.Std_File : action.Act_File);
            case 3:
                return (isStand ? action.Std_Index : action.Act_Index).ToString(CultureInfo.InvariantCulture);
            case 4:
                return (isStand ? action.Std_Count : action.Act_Count).ToString(CultureInfo.InvariantCulture);
            case 5:
                return (isStand ? action.Std_Time : action.Act_Time).ToString(CultureInfo.InvariantCulture);
            case 6:
                return EffectImageName(isStand ? action.Std_EffFile : action.Act_EffFile);
            case 7:
                return (isStand ? action.Std_EffIndex : action.Act_EffIndex).ToString(CultureInfo.InvariantCulture);
            default:
                return "";
        }
    }

    private static string EffectImageName(int index)
        => index >= 0 && index < ViewList2State.g_EffectImageList.Count
            ? ViewList2State.g_EffectImageList[index]
            : "";

    private void RefreshActionRows()
    {
        for (int i = 0; i < vstNpcAction.Items.Count; i++)
        {
            if (vstNpcAction.Items[i].Tag is not TNpcNodeData nodeData)
                continue;
            for (int col = 2; col < 8 && col < vstNpcAction.Items[i].SubItems.Count; col++)
                vstNpcAction.Items[i].SubItems[col].Text = ActionCellText(nodeData, col);
        }
    }

    /// <summary>Editing（957-961）：Node &lt;&gt; nil 且 Column &gt; 1。</summary>
    public bool ActionEditingAllowed(int index, int column)
        => index >= 0 && index < vstNpcAction.Items.Count && column > 1;

    /// <summary>PrepareEdit（449-570）：列 2·6 为图库下拉，列 3·4·5·7 为 SpinEdit。</summary>
    public bool ActionPrepareEdit(int column) => column is 2 or 3 or 4 or 5 or 6 or 7;


    /// <summary>GetHint（963-974）1:1：仅列 7 给提示。</summary>
    public static string ActionHint(int column)
        => column == 7 ? "特效开始图片为-1表示不使用特效" : "";

    /// <summary>BeforeCellPaint（1331-1345）1:1：站立行底色 $00F2E4D8（BGR → RGB D8E4F2）。</summary>
    public static System.Drawing.Color? ActionCellBackColor(TNpcActionType actionType, int column)
        => actionType == TNpcActionType.atStand
            ? System.Drawing.Color.FromArgb(0xD8, 0xE4, 0xF2)
            : null;

    /// <summary>
    /// EndEdit（288-442）1:1：按 行类型 × 列 写回；列 2/6 取下拉下标，3/4/5/7 取数值；
    /// 变更则 SetNpcConfigChanged()。
    /// </summary>
    public bool ActionEndEdit(int index, int column, int comboIndex, int spinValue)
    {
        if (index < 0 || index >= vstNpcAction.Items.Count)
            return false;
        if (vstNpcAction.Items[index].Tag is not TNpcNodeData nodeData)
            return false;

        var action = nodeData.Owner.DirActions[nodeData.DirIndex];
        bool isChanged = false;
        bool isStand = nodeData.ActionType == TNpcActionType.atStand;

        if (column is 2 or 6)
        {
            if (isStand)
            {
                if (column == 2 && action.Std_File != comboIndex) { action.Std_File = (ushort)comboIndex; isChanged = true; }
                if (column == 6 && action.Std_EffFile != comboIndex) { action.Std_EffFile = (ushort)comboIndex; isChanged = true; }
            }
            else
            {
                if (column == 2 && action.Act_File != comboIndex) { action.Act_File = (ushort)comboIndex; isChanged = true; }
                if (column == 6 && action.Act_EffFile != comboIndex) { action.Act_EffFile = (ushort)comboIndex; isChanged = true; }
            }
        }
        else if (column is 3 or 4 or 5 or 7)
        {
            if (isStand)
            {
                if (column == 3 && action.Std_Index != spinValue) { action.Std_Index = (short)spinValue; isChanged = true; }
                if (column == 4 && action.Std_Count != spinValue) { action.Std_Count = (ushort)spinValue; isChanged = true; }
                if (column == 5 && action.Std_Time != spinValue) { action.Std_Time = (ushort)spinValue; isChanged = true; }
                if (column == 7 && action.Std_EffIndex != spinValue) { action.Std_EffIndex = (short)spinValue; isChanged = true; }
            }
            else
            {
                if (column == 3 && action.Act_Index != spinValue) { action.Act_Index = (short)spinValue; isChanged = true; }
                if (column == 4 && action.Act_Count != spinValue) { action.Act_Count = (ushort)spinValue; isChanged = true; }
                if (column == 5 && action.Act_Time != spinValue) { action.Act_Time = (ushort)spinValue; isChanged = true; }
                if (column == 7 && action.Act_EffIndex != spinValue) { action.Act_EffIndex = (short)spinValue; isChanged = true; }
            }
        }

        nodeData.Owner.DirActions[nodeData.DirIndex] = action;
        RefreshActionRows();

        if (isChanged)
            SetNpcConfigChanged();
        return isChanged;
    }

    /// <summary>NodeClick（1057-1063）：列 &gt; 1 → 请求进入编辑。</summary>
    public void VstNpcActionNodeClick(int index = -1, int column = -1)
    {
        if (index < 0 || index >= vstNpcAction.Items.Count || column <= 1)
            return;
        LastEditingRequest = (index, column);
    }

    /// <summary>Checked（929-943）1:1：勾选态写回 Enabled 并置变更。</summary>
    public void VstNpcActionChecked(int index)
    {
        if (index < 0 || index >= vstNpcAction.Items.Count)
            return;
        if (vstNpcAction.Items[index].Tag is not TNpcNodeData nodeData)
            return;

        var action = nodeData.Owner.DirActions[nodeData.DirIndex];
        action.Enabled = vstNpcAction.Items[index].Checked ? 1 : 0;
        nodeData.Owner.DirActions[nodeData.DirIndex] = action;
        SetNpcConfigChanged();
    }

    // ==================== 批量 ====================

    /// <summary>btnNpcFileXXX（1065-1102）1:1：批量填充四类图库文件。</summary>
    public void BtnNpcFileClick(string which)
    {
        if (FCurrentNpcCustomConfig == null)
            return;
        int itemIndex = cbbNpcBatchFile.SelectedIndex;
        for (int i = 0; i < TNpcDirActionList.Count; i++)
        {
            var a = FCurrentNpcCustomConfig.DirActions[i];
            switch (which)
            {
                case "stand": a.Std_File = (ushort)itemIndex; break;
                case "standEffect": a.Std_EffFile = (ushort)itemIndex; break;
                case "action": a.Act_File = (ushort)itemIndex; break;
                case "actionEffect": a.Act_EffFile = (ushort)itemIndex; break;
            }
            FCurrentNpcCustomConfig.DirActions[i] = a;
        }
        SetNpcConfigChanged(true);
        RefreshActionRows();
    }

    /// <summary>btnNpcTimeXXX（1104-1145）1:1：批量填充站立/动作间隔。</summary>
    public void BtnNpcTimeClick(string which)
    {
        if (FCurrentNpcCustomConfig == null)
            return;
        ushort time = (ushort)seNpcBatchTime.Value;
        for (int i = 0; i < TNpcDirActionList.Count; i++)
        {
            var a = FCurrentNpcCustomConfig.DirActions[i];
            if (which == "stand")
                a.Std_Time = time;
            else if (which == "action")
                a.Act_Time = time;
            FCurrentNpcCustomConfig.DirActions[i] = a;
        }
        SetNpcConfigChanged(true);
        RefreshActionRows();
    }

    /// <summary>
    /// btnNpcCalcXXX（1147-1193）1:1：四路等差数列 —— 起始图 + (张数 + 空帧) × (i-1)，
    /// 循环 1..seNpcCalcDirCount；站立/动作写 Index+Count，两特效路只写 Index（张数槽位原文注释掉）。
    /// </summary>
    public void BtnNpcCalcClick(string which)
    {
        if (FCurrentNpcCustomConfig == null)
            return;
        int startIndex = (int)seNpcCalcStartIndex.Value;
        int playCount = (int)seNpcCalcPlayCount.Value;
        int emptyCount = (int)seNpcCalcEmptyCount.Value;
        int dirCount = (int)seNpcCalcDirCount.Value;

        for (int i = 1; i <= dirCount; i++)
        {
            int value = startIndex + (playCount + emptyCount) * (i - 1);
            var a = FCurrentNpcCustomConfig.DirActions[i - 1];
            switch (which)
            {
                case "stand":
                    a.Std_Index = (short)value;
                    a.Std_Count = (ushort)playCount;
                    break;
                case "standEffect":
                    a.Std_EffIndex = (short)value;
                    break;
                case "action":
                    a.Act_Index = (short)value;
                    a.Act_Count = (ushort)playCount;
                    break;
                case "actionEffect":
                    a.Act_EffIndex = (short)value;
                    break;
            }
            FCurrentNpcCustomConfig.DirActions[i - 1] = a;
        }

        SetNpcConfigChanged(true);
        RefreshActionRows();
    }

    // ==================== 保存 ====================

    /// <summary>
    /// btnSaveNpcClick（1195-1224）1:1：逐节点 IsChanged 才 SaveToIniFile → 重建客户端压缩块
    /// → 落 dwCustomNpcMoveTime → 清标志禁按钮。
    /// </summary>
    public void BtnSaveNpcClick()
    {
        foreach (System.Windows.Forms.ListViewItem item in vstCustomNpc.Items)
        {
            if (item.Tag is TNpcConfigNodeData nodeData && nodeData.Config.IsChanged)
                nodeData.Config.SaveToIniFile();
        }

        RebuildCustomNpcListText();

        M2Config.dwCustomNpcMoveTime = (int)seCustomNpcMoveTime.Value;
        WriteIntegerHandler?.Invoke("CustomNpcMoveTime", M2Config.dwCustomNpcMoveTime);

        FIsNpcChanged = false;
        btnSaveNpc.Enabled = false;
    }

    /// <summary>
    /// RebuildCustomNpcListText（705-764）1:1：按「启用优先」打包全部配置 →
    /// 压缩（zLibCompressBuffer 接缝）→ 记录长度与压缩数据 CRC。
    /// </summary>
    public void RebuildCustomNpcListText()
    {
        int recordSize = CustomNpcUtils.ClientRecordSize;
        var buffer = new List<byte>(CustomNpcList.Count * recordSize);

        foreach (var config in CustomNpcList)
        {
            var clientConfig = new TClientCustomNpcConfig
            {
                wNpcAppr = config.NpcAppr,
                BaseConfig = config.ClientBaseConfig,
            };

            int index = 0;
            for (int j = 0; j < TNpcDirActionList.Count; j++)
            {
                if (config.DirActions[j].Enabled != 0)
                {
                    clientConfig.Actions[index] = config.DirActions[j];
                    index++;
                }
            }
            clientConfig.wDirCount = (ushort)index;

            for (int j = 0; j < TNpcDirActionList.Count; j++)
            {
                if (config.DirActions[j].Enabled == 0)
                {
                    clientConfig.Actions[index] = config.DirActions[j];
                    index++;
                }
            }

            buffer.AddRange(CustomNpcUtils.StructToBytes(clientConfig));
        }

        CustomNpcListTextLen = buffer.Count;
        CustomNpcListText = CompressHandler?.Invoke(buffer.ToArray()) ?? buffer.ToArray();
        CustomNpcListTextCrc = GXX.Core.Crypto.CheckCrc.BufferCRC(CustomNpcListText, CustomNpcListText.Length);
    }

    /// <summary>
    /// btnSaveNpcToFileClick（1226-1259）1:1：另存对话框 → 强制 .dat 扩展 →
    /// 记 g_Config.sCustomNpcClientConfigFileName → SaveCustomNpcClientConfigs → 提示。
    /// </summary>
    public void BtnSaveNpcToFileClick()
    {
        string initial = M2Config.sCustomNpcClientConfigFileName;
        string? picked = SaveFileDialogHandler?.Invoke(initial);
        if (picked == null)
            return;

        string fileName = System.IO.Path.ChangeExtension(picked, ".dat");
        M2Config.sCustomNpcClientConfigFileName = fileName;
        WriteStringHandler?.Invoke("CustomNpcClientConfigFileName", fileName);

        CustomNpcUtils.SaveCustomNpcClientConfigs(new List<TCustomNpcConfig>(CustomNpcList), fileName);

        ShowMessageHandler?.Invoke("已经生成自定义NPC登录器配置文件");
    }

    // ==================== 配置项回写 ====================

    private void CbbNpcHPFileChange()
    {
        if (FCurrentNpcCustomConfig == null) return;
        var b = FCurrentNpcCustomConfig.ClientBaseConfig;
        b.HPFile = cbbNpcHPFile.SelectedIndex;
        FCurrentNpcCustomConfig.ClientBaseConfig = b;
        SetNpcConfigChanged();
    }

    private void CbbNpcKeepPlayFileChange()
    {
        if (FCurrentNpcCustomConfig == null) return;
        var b = FCurrentNpcCustomConfig.ClientBaseConfig;
        b.KeepPlayFile = cbbNpcKeepPlayFile.SelectedIndex;
        FCurrentNpcCustomConfig.ClientBaseConfig = b;
        SetNpcConfigChanged();
    }

    private void SeNpcHPStartIndexChange() => ApplyBase(b => { b.HPStartIndex = (int)seNpcHPStartIndex.Value; return b; });
    private void SeNpcHPBgOffsetXChange() => ApplyBase(b => { b.HPBgOffsetX = (int)seNpcHPBgOffsetX.Value; return b; });
    private void SeNpcHPBgOffsetYChange() => ApplyBase(b => { b.HPBgOffsetY = (int)seNpcHPBgOffsetY.Value; return b; });
    private void SeNpcHPOffsetXChange() => ApplyBase(b => { b.HPOffsetX = (int)seNpcHPOffsetX.Value; return b; });
    private void SeNpcHPOffsetYChange() => ApplyBase(b => { b.HPOffsetY = (int)seNpcHPOffsetY.Value; return b; });
    private void SeNpcHPTextOffsetXChange() => ApplyBase(b => { b.HPTextOffsetX = (int)seNpcHPTextOffsetX.Value; return b; });
    private void SeNpcHPTextOffsetYChange() => ApplyBase(b => { b.HPTextOffsetY = (int)seNpcHPTextOffsetY.Value; return b; });

    private void SeNpcKeepPlayIndexChange() => ApplyBase(b => { b.KeepPlayIndex = (int)seNpcKeepPlayIndex.Value; return b; });
    private void SeNpcKeepPlayCountChange() => ApplyBase(b => { b.KeepPlayCount = (int)seNpcKeepPlayCount.Value; return b; });
    private void SeNpcKeepPlayTimeChange() => ApplyBase(b => { b.KeepPlayTime = (int)seNpcKeepPlayTime.Value; return b; });
    private void ChkNpcKeepPlayBlendDrawClick() => ApplyBase(b => { b.KeepPlayBlendDraw = chkNpcKeepPlayBlendDraw.Checked ? (byte)1 : (byte)0; return b; });

    private void SeKeepPlayOffsetChanged()
        => ApplyBase(b =>
        {
            b.KeepPlayOffsetX = (int)seKeepPlayOffsetX.Value;
            b.KeepPlayOffsetY = (int)seKeepPlayOffsetY.Value;
            return b;
        });

    private void CbbNpcStandDrawModeChange() => ApplyBase(b => { b.StandDrawMode = (TCustomDrawMode)cbbNpcStandDrawMode.SelectedIndex; return b; });
    private void CbbNpcStandEffectDrawModeChange() => ApplyBase(b => { b.StandEffectDrawMode = (TCustomDrawMode)cbbNpcStandEffectDrawMode.SelectedIndex; return b; });
    private void CbbNpcActionDrawModeChange() => ApplyBase(b => { b.ActionDrawMode = (TCustomDrawMode)cbbNpcActionDrawMode.SelectedIndex; return b; });
    private void CbbNpcActionEffectDrawModeChange() => ApplyBase(b => { b.ActionEffectDrawMode = (TCustomDrawMode)cbbNpcActionEffectDrawMode.SelectedIndex; return b; });

    /// <summary>统一「取结构 → 改字段 → 写回 → 置变更」模式（各 Change 处理器 1:1）。</summary>
    private void ApplyBase(Func<TNpcBaseConfig, TNpcBaseConfig> mutate)
    {
        if (FCurrentNpcCustomConfig == null)
            return;
        var updated = mutate(FCurrentNpcCustomConfig.ClientBaseConfig);
        FCurrentNpcCustomConfig.ClientBaseConfig = updated;
        SetNpcConfigChanged();
    }

    /// <summary>chkSendCustomNPCConfigClick（1325-1329）。</summary>
    public void ChkSendCustomNpcConfigClick()
    {
        M2Config.boSendCustomNpcConfig = chkSendCustomNPCConfig.Checked;
        WriteBoolHandler?.Invoke("SendCustomNpcConfig", M2Config.boSendCustomNpcConfig);
    }

    // ==================== 绘制顺序 ====================



    /// <summary>显式指定绘制顺序列表选中行（WinForms 无句柄时 SelectedIndex 不可靠，Delphi ListBox 亦用 ItemIndex）。</summary>
    public void SelectDrawOrderRow(int index) => _explicitDrawOrderRow = index;

    private int _explicitDrawOrderRow = -2;

    /// <summary>
    /// btnNpcMoveTop/Bottom（1419-1470）1:1：移动选中项一位 → 按前两项对象值反推 DrawOrder。
    /// </summary>
    public void BtnNpcMoveClick(bool up)
    {
        if (FCurrentNpcCustomConfig == null)
            return;
        int index = _explicitDrawOrderRow != -2 ? _explicitDrawOrderRow : lstNpcDrawOrder.SelectedIndex;

        if (up)
        {
            if (index <= 0)
                return;
            MoveListBoxItem(index, index - 1);
            lstNpcDrawOrder.SelectedIndex = index - 1;
            _explicitDrawOrderRow = index - 1;
        }
        else
        {
            if (index < 0 || index >= lstNpcDrawOrder.Items.Count - 1)
                return;
            MoveListBoxItem(index, index + 1);
            lstNpcDrawOrder.SelectedIndex = index + 1;
            _explicitDrawOrderRow = index + 1;
        }

        int o1 = DrawOrderObjectAt(0);
        int o2 = DrawOrderObjectAt(1);
        var newOrder = SequenceToDrawOrder(o1, o2);
        ApplyBase(b => { b.DrawOrder = newOrder; return b; });
    }

    /// <summary>前两项对象值 → DrawOrder（1448-1470 六个分支 1:1）。</summary>
    public static TCustomNpcDrawOrder SequenceToDrawOrder(int o1, int o2) => o1 switch
    {
        0 => o2 == 1 ? TCustomNpcDrawOrder.ndoKeep_Chr_Eff : TCustomNpcDrawOrder.ndoKeep_Eff_Chr,
        1 => o2 == 0 ? TCustomNpcDrawOrder.ndoChr_Keep_Eff : TCustomNpcDrawOrder.ndoChr_Eff_Keep,
        2 => o2 == 0 ? TCustomNpcDrawOrder.ndoEff_Keep_Chr : TCustomNpcDrawOrder.ndoEff_Chr_Keep,
        _ => TCustomNpcDrawOrder.ndoKeep_Chr_Eff,
    };

    private void MoveListBoxItem(int from, int to)
    {
        object? item = lstNpcDrawOrder.Items[from];
        lstNpcDrawOrder.Items.RemoveAt(from);
        lstNpcDrawOrder.Items.Insert(to, item);
    }

    /// <summary>列表框第 i 项的对象值（Delphi Items.Objects[i] 等效：以文案反查下标）。</summary>
    public int DrawOrderObjectAt(int index)
    {
        string text = lstNpcDrawOrder.Items[index]?.ToString() ?? "";
        for (int i = 0; i < DrawOrderNames.Length; i++)
        {
            if (DrawOrderNames[i] == text)
                return i;
        }
        return 0;
    }

    /// <summary>ShowModal 接缝。</summary>
    public Func<System.Windows.Forms.DialogResult>? ShowModalHandler;

    public void ShowFrmCustomNpc() => _ = ShowModalHandler?.Invoke() ?? ShowDialog();
}

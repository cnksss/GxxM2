using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ItemSet.pas TfrmItemSet 巨片第一片（4011 行）：
/// ModValue/uModValue（4 保存按钮）+ 主保存组 1:1 —— 凹槽（物品笛子）组（chkDisableRightClickFluteStone/
/// 三个石头数量处理器置下发标志）、物品体验率、行会传送/攻沙毒、传送移动、异常状态（魔道麻痹/冰冻/蛛网）
/// 与 ButtonItemSetSaveClick 22 键（GroupRecallTime 键写 nAttackPosionRate 值的原文瑕疵保留）+
/// boSendServerConfig 门控下发（下发后显式复位）。
/// 极品属性（AddValue）/未鉴定物品（UnKnowItem）/新属性（NewAbil）组随余片接入。
/// </summary>
public sealed class ItemSetForm : System.Windows.Forms.Form
{
    private bool boOpened;
    private bool boModValued;
    private bool boSendServerConfig;

    // ---- 凹槽组 ----
    public System.Windows.Forms.CheckBox chkOpenItemFlute = null!;
    public System.Windows.Forms.CheckBox chkDisableRightClickFluteStone = null!;
    public System.Windows.Forms.NumericUpDown seItemFluteStoneCount = null!;
    public System.Windows.Forms.NumericUpDown seItemFluteStoneIdxCount = null!;
    public System.Windows.Forms.NumericUpDown seItemFluteStoneOverlapCount = null!;

    // ---- 体验率/行会传送/攻沙毒/传送移动 ----
    public System.Windows.Forms.NumericUpDown EditItemExpRate = null!;
    public System.Windows.Forms.NumericUpDown EditItemPowerRate = null!;
    public System.Windows.Forms.NumericUpDown EditGuildRecallTime = null!;
    public System.Windows.Forms.NumericUpDown EditAttackPosionRate = null!;
    public System.Windows.Forms.NumericUpDown EditAttackPosionTime = null!;
    public System.Windows.Forms.CheckBox CheckBoxUserMoveCanDupObj = null!;
    public System.Windows.Forms.CheckBox CheckBoxUserMoveCanOnItem = null!;
    public System.Windows.Forms.NumericUpDown EditUserMoveTime = null!;

    // ---- 异常状态 ----
    public System.Windows.Forms.NumericUpDown EditMDParalysisRate = null!;
    public System.Windows.Forms.NumericUpDown EditMDParalysisTime = null!;
    public System.Windows.Forms.NumericUpDown EditFrozenRate = null!;
    public System.Windows.Forms.NumericUpDown EditFrozenTime = null!;
    public System.Windows.Forms.CheckBox CheckBoxFrozenUseMagicStruck = null!;
    public System.Windows.Forms.NumericUpDown EditCobwebWindingRate = null!;
    public System.Windows.Forms.NumericUpDown EditCobwebWindingTime = null!;
    public System.Windows.Forms.CheckBox CheckBoxCobwebWindingUseMagicStruck = null!;

    // ---- 保存按钮 ----
    public System.Windows.Forms.Button ButtonItemSetSave = null!;
    public System.Windows.Forms.Button ButtonAddValueSave = null!;
    public System.Windows.Forms.Button ButtonUnKnowItemSave = null!;
    public System.Windows.Forms.Button ButtonNewAbilSave = null!;

    // ---- AddValue/UnKnow/NewAbil 控件（批次J35） ----
    public System.Collections.Generic.Dictionary<string, System.Windows.Forms.NumericUpDown> AddItemValueSpins = new();
    public System.Collections.Generic.Dictionary<string, System.Windows.Forms.NumericUpDown> UnknowSpins = new();
    public System.Windows.Forms.CheckBox chkItemNewAbilAllowUse = null!;

    public ItemSetForm()
    {
        InitializeComponent();
    }

    private System.Windows.Forms.NumericUpDown MakeSpin(string caption, int top, int left, System.Windows.Forms.Control parent)
    {
        parent.Controls.Add(new System.Windows.Forms.Label { Text = caption, Left = left, Top = top + 4, AutoSize = true });
        var edit = new System.Windows.Forms.NumericUpDown { Left = left + 150, Top = top, Width = 90, Maximum = 2000000000 };
        parent.Controls.Add(edit);
        return edit;
    }

    private System.Windows.Forms.CheckBox MkChk(string caption, int left, int top, System.Windows.Forms.Control parent, Action<object?> handler)
    {
        var chk = new System.Windows.Forms.CheckBox { Text = caption, Left = left, Top = top, AutoSize = true };
        chk.Click += (s, e) => handler(s);
        parent.Controls.Add(chk);
        return chk;
    }

    private System.Windows.Forms.NumericUpDown BindSpin(string caption, int top, int left, System.Windows.Forms.Control parent, Action<object?> handler)
    {
        var edit = MakeSpin(caption, top, left, parent);
        edit.ValueChanged += (s, e) => handler(s);
        return edit;
    }

    private void InitializeComponent()
    {
        Text = "物品设置";
        Width = 700;
        Height = 560;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var gb1 = new System.Windows.Forms.GroupBox { Text = "凹槽", Left = 8, Top = 4, Width = 660, Height = 110 };
        Controls.Add(gb1);
        chkOpenItemFlute = MkChk("开启凹槽功能", 12, 20, gb1, chkOpenItemFluteClick);
        chkDisableRightClickFluteStone = MkChk("禁止右键镶嵌", 200, 20, gb1, chkDisableRightClickFluteStoneClick);
        seItemFluteStoneCount = BindSpin("同属性镶嵌上限:", 46, 10, gb1, seItemFluteStoneCountChange);
        seItemFluteStoneIdxCount = BindSpin("镶嵌属性种类:", 46, 350, gb1, seItemFluteStoneIdxCountChange);
        seItemFluteStoneOverlapCount = BindSpin("单孔叠加数量:", 72, 10, gb1, seItemFluteStoneOverlapCountChange);

        var gb2 = new System.Windows.Forms.GroupBox { Text = "物品体验率", Left = 8, Top = 118, Width = 660, Height = 90 };
        Controls.Add(gb2);
        EditItemExpRate = BindSpin("物品经验率:", 16, 10, gb2, EditItemExpRateChange);
        EditItemPowerRate = BindSpin("物品威力率:", 46, 10, gb2, EditItemPowerRateChange);

        var gb3 = new System.Windows.Forms.GroupBox { Text = "行会传送/攻沙毒", Left = 8, Top = 212, Width = 660, Height = 120 };
        Controls.Add(gb3);
        EditGuildRecallTime = BindSpin("行会传送间隔(秒):", 16, 10, gb3, EditGuildRecallTimeChange);
        EditAttackPosionRate = BindSpin("攻沙毒机率:", 42, 10, gb3, EditAttackPosionRateChange);
        EditAttackPosionTime = BindSpin("攻沙毒时间:", 68, 10, gb3, EditAttackPosionTimeChange);

        var gb4 = new System.Windows.Forms.GroupBox { Text = "传送移动", Left = 8, Top = 336, Width = 660, Height = 90 };
        Controls.Add(gb4);
        CheckBoxUserMoveCanDupObj = MkChk("传送可叠加目标", 12, 20, gb4, CheckBoxUserMoveCanDupObjClick);
        CheckBoxUserMoveCanOnItem = MkChk("传送可站物品", 200, 20, gb4, CheckBoxUserMoveCanOnItemClick);
        EditUserMoveTime = BindSpin("传送间隔:", 46, 10, gb4, EditUserMoveTimeChange);

        var gb5 = new System.Windows.Forms.GroupBox { Text = "异常状态（戒指）", Left = 8, Top = 430, Width = 660, Height = 170 };
        Controls.Add(gb5);
        EditMDParalysisRate = BindSpin("魔道麻痹机率:", 16, 10, gb5, EditMDParalysisRateChange);
        EditMDParalysisTime = BindSpin("魔道麻痹时间:", 16, 350, gb5, EditMDParalysisTimeChange);
        EditFrozenRate = BindSpin("冰冻机率:", 42, 10, gb5, EditFrozenRateChange);
        EditFrozenTime = BindSpin("冰冻时间:", 42, 350, gb5, EditFrozenTimeChange);
        CheckBoxFrozenUseMagicStruck = MkChk("冰冻对魔法有效", 200, 68, gb5, CheckBoxFrozenUseMagicStruckClick);
        EditCobwebWindingRate = BindSpin("蛛网机率:", 68, 10, gb5, EditCobwebWindingRateChange);
        EditCobwebWindingTime = BindSpin("蛛网时间:", 68, 350, gb5, EditCobwebWindingTimeChange);
        CheckBoxCobwebWindingUseMagicStruck = MkChk("蛛网对魔法有效", 200, 94, gb5, CheckBoxCobwebWindingUseMagicStruckClick);

        ButtonItemSetSave = new System.Windows.Forms.Button { Text = "保存物品设置(&I)", Left = 8, Top = 606, Width = 130, Height = 26, Enabled = false };
        ButtonItemSetSave.Click += (s, e) => ButtonItemSetSaveClick(s);
        Controls.Add(ButtonItemSetSave);

        // ---- 极品属性组（批次J35）：206 项（键名循环构建，AutoScroll 容器） ----
        var addValuePanel = new System.Windows.Forms.Panel
        {
            Left = 8,
            Top = 640,
            Width = 660,
            Height = 300,
            AutoScroll = true,
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
        };
        Controls.Add(addValuePanel);
        foreach (var (key, _) in ItemSetAddValueTables.Keys)
        {
            var k = key;
            addValuePanel.Controls.Add(new System.Windows.Forms.Label { Text = k, Left = 8 + (AddItemValueSpins.Count % 2) * 330, Top = 6 + (AddItemValueSpins.Count / 2) * 30, AutoSize = true });
            var spin = new System.Windows.Forms.NumericUpDown { Left = 150 + (AddItemValueSpins.Count % 2) * 330, Top = 2 + (AddItemValueSpins.Count / 2) * 30, Width = 100, Minimum = -2000000000, Maximum = 2000000000 };
            spin.ValueChanged += (s, e) => AddValueSpinChanged(k);
            addValuePanel.Controls.Add(spin);
            AddItemValueSpins[k] = spin;
        }

        ButtonAddValueSave = new System.Windows.Forms.Button { Text = "保存极品属性(&A)", Left = 8, Top = 946, Width = 130, Height = 26, Enabled = false };
        ButtonAddValueSave.Click += (s, e) => ButtonAddValueSaveClick(s);
        Controls.Add(ButtonAddValueSave);

        // ---- 未鉴定物品组（批次J35）：30 项 ----
        var unknowPanel = new System.Windows.Forms.Panel
        {
            Left = 8,
            Top = 980,
            Width = 660,
            Height = 220,
            AutoScroll = true,
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
        };
        Controls.Add(unknowPanel);
        var unknowKeys = new[]
        {
            "UnknowRingACAddRate", "UnknowRingACAddValueMaxLimit", "UnknowRingDCAddRate", "UnknowRingDCAddValueMaxLimit",
            "UnknowRingMCAddRate", "UnknowRingMCAddValueMaxLimit", "UnknowRingSCAddRate", "UnknowRingSCAddValueMaxLimit",
            "UnknowRingMACAddRate", "UnknowRingMACAddValueMaxLimit", "UnknowNecklaceACAddRate", "UnknowNecklaceACAddValueMaxLimit",
            "UnknowNecklaceDCAddRate", "UnknowNecklaceDCAddValueMaxLimit", "UnknowNecklaceMCAddRate", "UnknowNecklaceMCAddValueMaxLimit",
            "UnknowNecklaceSCAddRate", "UnknowNecklaceSCAddValueMaxLimit", "UnknowNecklaceMACAddRate", "UnknowNecklaceMACAddValueMaxLimit",
            "UnknowHelMetACAddRate", "UnknowHelMetACAddValueMaxLimit", "UnknowHelMetDCAddRate", "UnknowHelMetDCAddValueMaxLimit",
            "UnknowHelMetMCAddRate", "UnknowHelMetMCAddValueMaxLimit", "UnknowHelMetSCAddRate", "UnknowHelMetSCAddValueMaxLimit",
            "UnknowHelMetMACAddRate", "UnknowHelMetMACAddValueMaxLimit",
        };
        foreach (var key in unknowKeys)
        {
            var k = key;
            unknowPanel.Controls.Add(new System.Windows.Forms.Label { Text = k, Left = 8 + (UnknowSpins.Count % 2) * 330, Top = 6 + (UnknowSpins.Count / 2) * 30, AutoSize = true });
            var spin = new System.Windows.Forms.NumericUpDown { Left = 260 + (UnknowSpins.Count % 2) * 330, Top = 2 + (UnknowSpins.Count / 2) * 30, Width = 100, Minimum = -2000000000, Maximum = 2000000000 };
            spin.ValueChanged += (s, e) => UnknowSpinChanged(k);
            unknowPanel.Controls.Add(spin);
            UnknowSpins[k] = spin;
        }

        ButtonUnKnowItemSave = new System.Windows.Forms.Button { Text = "保存未鉴定(&K)", Left = 8, Top = 1206, Width = 130, Height = 26, Enabled = false };
        ButtonUnKnowItemSave.Click += (s, e) => ButtonUnKnowItemSaveClick(s);
        Controls.Add(ButtonUnKnowItemSave);

        // ---- 新属性组（批次J35，标量部分） ----
        chkItemNewAbilAllowUse = MkChk("启用新属性", 12, 1210, this, chkItemNewAbilAllowUseClick);
        ButtonNewAbilSave = new System.Windows.Forms.Button { Text = "保存新属性(&N)", Left = 160, Top = 1210, Width = 120, Height = 26, Enabled = false };
        ButtonNewAbilSave.Click += (s, e) => ButtonNewAbilSaveClick(s);
        Controls.Add(ButtonNewAbilSave);
    }

    // ================= Delphi 1:1 =================

    public bool IsModValued => boModValued;

    private void ModValue()
    {
        boModValued = true;
        ButtonItemSetSave.Enabled = true;
        ButtonAddValueSave.Enabled = true;
        ButtonUnKnowItemSave.Enabled = true;
        ButtonNewAbilSave.Enabled = true;
    }

    private void uModValue()
    {
        boModValued = false;
        ButtonItemSetSave.Enabled = false;
        ButtonAddValueSave.Enabled = false;
        ButtonUnKnowItemSave.Enabled = false;
        ButtonNewAbilSave.Enabled = false;
    }

    /// <summary>Delphi Open() 主保存组子集（AddValue/UnKnow/NewAbil 组加载随巨片接入）。</summary>
    public void Open(bool showModal = true)
    {
        boOpened = false;
        uModValue();
        boSendServerConfig = false;

        chkOpenItemFlute.Checked = M2Config.boOpenItemFlute;
        chkDisableRightClickFluteStone.Checked = M2Config.boDisableRightClickFluteStone;
        seItemFluteStoneCount.Value = M2Config.nItemFluteStoneCount;
        seItemFluteStoneIdxCount.Value = M2Config.nItemFluteStoneIdxCount;
        seItemFluteStoneOverlapCount.Value = M2Config.nItemFluteStoneOverlapCount;

        EditItemExpRate.Value = M2Config.nItemExpRate;
        EditItemPowerRate.Value = M2Config.nItemPowerRate;

        EditGuildRecallTime.Value = M2Config.nGuildRecallTime;
        EditAttackPosionRate.Value = M2Config.nAttackPosionRate;
        EditAttackPosionTime.Value = M2Config.nAttackPosionTime;
        CheckBoxUserMoveCanDupObj.Checked = M2Config.boUserMoveCanDupObj;
        CheckBoxUserMoveCanOnItem.Checked = M2Config.boUserMoveCanOnItem;
        EditUserMoveTime.Value = M2Config.dwUserMoveTime;

        EditMDParalysisRate.Value = M2Config.dwMDParalysisRate;
        EditMDParalysisTime.Value = M2Config.dwMDParalysisTime;
        EditFrozenRate.Value = M2Config.dwFrozenRate;
        EditFrozenTime.Value = M2Config.dwFrozenTime;
        CheckBoxFrozenUseMagicStruck.Checked = M2Config.boFrozenUseMagicStruck;
        EditCobwebWindingRate.Value = M2Config.dwCobwebWindingRate;
        EditCobwebWindingTime.Value = M2Config.dwCobwebWindingTime;
        CheckBoxCobwebWindingUseMagicStruck.Checked = M2Config.boCobwebWindingUseMagicStruck;

        // ---- 极品属性/未鉴定/新属性组（批次J35） ----
        foreach (var (key, _) in ItemSetAddValueTables.Keys)
            AddItemValueSpins[key].Value = M2Config.ItemSetAddValues[key];
        foreach (var key in UnknowSpins.Keys)
            UnknowSpins[key].Value = M2Config.ItemSetUnknowValues[key];

        boOpened = true;
        if (showModal)
            ShowDialog();
    }

    /// <summary>极品属性数值变更（boOpened 门控写 ItemSetAddValues + ModValue）。</summary>
    public void AddValueSpinChanged(string key)
    {
        if (!boOpened)
            return;
        M2Config.ItemSetAddValues[key] = (int)AddItemValueSpins[key].Value;
        ModValue();
    }

    /// <summary>未鉴定物品数值变更（boOpened 门控写 ItemSetUnknowValues + ModValue）。</summary>
    public void UnknowSpinChanged(string key)
    {
        if (!boOpened)
            return;
        M2Config.ItemSetUnknowValues[key] = (int)UnknowSpins[key].Value;
        ModValue();
    }

    public void chkItemNewAbilAllowUseClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boItemNewAbilAllowUse = chkItemNewAbilAllowUse.Checked;
        ModValue();
    }

    /// <summary>ButtonAddValueSaveClick 1:1（按 ItemSetAddValueTables.Keys 顺序写 'Setup' 节 206 键）。</summary>
    public void ButtonAddValueSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        foreach (var (key, _) in ItemSetAddValueTables.Keys)
            Config.WriteInteger("Setup", key, M2Config.ItemSetAddValues[key]);
        uModValue();
    }

    /// <summary>ButtonUnKnowItemSaveClick 1:1（'Setup' 节 30 键，AC/MAC/DC/MC/SC 分组原文顺序）。</summary>
    public void ButtonUnKnowItemSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        foreach (var (key, _) in M2Config.ItemSetUnknowValues)
            Config.WriteInteger("Setup", key, M2Config.ItemSetUnknowValues[key]);
        uModValue();
    }

    /// <summary>ButtonNewAbilSaveClick 1:1 标量部分（6 键；ItemNewAbil[I][II] 等数组键随巨片余部）。</summary>
    public void ButtonNewAbilSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "ItemNewAbilAllowUse", M2Config.boItemNewAbilAllowUse);
        Config.WriteInteger("Setup", "ItemNewAbilMonRandomAddValue", M2Config.nItemNewAbilMonRandomAddValue);
        Config.WriteInteger("Setup", "ItemNewAbilMakeRandomAddValue", M2Config.nItemNewAbilMakeRandomAddValue);
        Config.WriteInteger("Setup", "ItemNewAbilScriptRandomAddValue", M2Config.nItemNewAbilScriptRandomAddValue);
        Config.WriteBool("Setup", "CloseDefenseUseScale", M2Config.boCloseDefenseUseScale);
        Config.WriteBool("Setup", "ReboundUseScale", M2Config.boReboundUseScale);
        Config.WriteInteger("Setup", "CritAttackHurtRate", (int)M2Config.dwCritAttackHurtRate);
        Config.WriteInteger("Setup", "DamageReboundRate", (int)M2Config.dwDamageReboundRate);
        Config.WriteInteger("Setup", "FatalBlowBasePower", (int)M2Config.dwFatalBlowBasePower);
        Config.WriteInteger("Setup", "FatalBlowNeedPower1", (int)M2Config.dwFatalBlowNeedPower1);
        Config.WriteInteger("Setup", "FatalBlowNeedPower2", (int)M2Config.dwFatalBlowNeedPower2);
        Config.WriteInteger("Setup", "FatalBlowNeedPower3", (int)M2Config.dwFatalBlowNeedPower3);
        uModValue();
    }

    // ---- 凹槽组 ----

    public void chkOpenItemFluteClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boOpenItemFlute = chkOpenItemFlute.Checked;
        ModValue();
    }

    public void chkDisableRightClickFluteStoneClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDisableRightClickFluteStone = chkDisableRightClickFluteStone.Checked;
        boSendServerConfig = true;
        ModValue();
    }

    public void seItemFluteStoneCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nItemFluteStoneCount = (int)seItemFluteStoneCount.Value;
        boSendServerConfig = true;
        ModValue();
    }

    public void seItemFluteStoneIdxCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nItemFluteStoneIdxCount = (int)seItemFluteStoneIdxCount.Value;
        boSendServerConfig = true;
        ModValue();
    }

    public void seItemFluteStoneOverlapCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nItemFluteStoneOverlapCount = (int)seItemFluteStoneOverlapCount.Value;
        boSendServerConfig = true;
        ModValue();
    }

    // ---- 体验率/行会传送/攻沙毒/传送移动 ----

    public void EditItemExpRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nItemExpRate = (int)EditItemExpRate.Value;
        ModValue();
    }

    public void EditItemPowerRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nItemPowerRate = (int)EditItemPowerRate.Value;
        ModValue();
    }

    public void EditGuildRecallTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nGuildRecallTime = (int)EditGuildRecallTime.Value;
        ModValue();
    }

    public void EditAttackPosionRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nAttackPosionRate = (int)EditAttackPosionRate.Value;
        ModValue();
    }

    public void EditAttackPosionTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nAttackPosionTime = (int)EditAttackPosionTime.Value;
        ModValue();
    }

    public void CheckBoxUserMoveCanDupObjClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boUserMoveCanDupObj = CheckBoxUserMoveCanDupObj.Checked;
        ModValue();
    }

    public void CheckBoxUserMoveCanOnItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boUserMoveCanOnItem = CheckBoxUserMoveCanOnItem.Checked;
        ModValue();
    }

    public void EditUserMoveTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwUserMoveTime = (uint)EditUserMoveTime.Value;
        ModValue();
    }

    // ---- 异常状态 ----

    public void EditMDParalysisRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMDParalysisRate = (uint)EditMDParalysisRate.Value;
        ModValue();
    }

    public void EditMDParalysisTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMDParalysisTime = (uint)EditMDParalysisTime.Value;
        ModValue();
    }

    public void EditFrozenRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwFrozenRate = (uint)EditFrozenRate.Value;
        ModValue();
    }

    public void EditFrozenTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwFrozenTime = (uint)EditFrozenTime.Value;
        ModValue();
    }

    public void CheckBoxFrozenUseMagicStruckClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boFrozenUseMagicStruck = CheckBoxFrozenUseMagicStruck.Checked;
        ModValue();
    }

    public void EditCobwebWindingRateChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwCobwebWindingRate = (uint)EditCobwebWindingRate.Value;
        ModValue();
    }

    public void EditCobwebWindingTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwCobwebWindingTime = (uint)EditCobwebWindingTime.Value;
        ModValue();
    }

    public void CheckBoxCobwebWindingUseMagicStruckClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boCobwebWindingUseMagicStruck = CheckBoxCobwebWindingUseMagicStruck.Checked;
        ModValue();
    }

    /// <summary>ButtonItemSetSaveClick 1:1（22 键；GroupRecallTime 键原文写 nAttackPosionRate 值；
    /// 尾部 boSendServerConfig 门控下发且下发后显式复位）。</summary>
    public void ButtonItemSetSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteBool("Setup", "OpenItemFlute", M2Config.boOpenItemFlute);
        Config.WriteBool("Setup", "DisableRightClickFluteStone", M2Config.boDisableRightClickFluteStone);
        Config.WriteInteger("Setup", "ItemFluteStoneCount", M2Config.nItemFluteStoneCount);
        Config.WriteInteger("Setup", "ItemFluteStoneIdxCount", M2Config.nItemFluteStoneIdxCount);
        Config.WriteInteger("Setup", "ItemFluteStoneOverlapCount", M2Config.nItemFluteStoneOverlapCount);

        Config.WriteInteger("Setup", "ItemPowerRate", M2Config.nItemPowerRate);
        Config.WriteInteger("Setup", "ItemExpRate", M2Config.nItemExpRate);
        Config.WriteInteger("Setup", "GuildRecallTime", M2Config.nGuildRecallTime);
        //Config.WriteInteger("Setup", 'GroupRecallTime', g_Config.nGroupRecallTime);
        Config.WriteInteger("Setup", "GroupRecallTime", M2Config.nAttackPosionRate); // Delphi 原文瑕疵键保留
        Config.WriteInteger("Setup", "AttackPosionRate", M2Config.nAttackPosionRate);
        Config.WriteInteger("Setup", "AttackPosionTime", M2Config.nAttackPosionTime);
        Config.WriteBool("Setup", "UserMoveCanDupObj", M2Config.boUserMoveCanDupObj);
        Config.WriteBool("Setup", "UserMoveCanOnItem", M2Config.boUserMoveCanOnItem);
        Config.WriteInteger("Setup", "UserMoveTime", (int)M2Config.dwUserMoveTime);

        Config.WriteInteger("Setup", "MDParalysisRate", (int)M2Config.dwMDParalysisRate);
        Config.WriteInteger("Setup", "MDParalysisTime", (int)M2Config.dwMDParalysisTime);
        Config.WriteInteger("Setup", "FrozenRate", (int)M2Config.dwFrozenRate);
        Config.WriteInteger("Setup", "FrozenTime", (int)M2Config.dwFrozenTime);
        Config.WriteBool("Setup", "FrozenUseMagicStruck", M2Config.boFrozenUseMagicStruck);

        Config.WriteInteger("Setup", "CobwebWindingRate", (int)M2Config.dwCobwebWindingRate);
        Config.WriteInteger("Setup", "CobwebWindingTime", (int)M2Config.dwCobwebWindingTime);
        Config.WriteBool("Setup", "CobwebWindingUseMagicStruck", M2Config.boCobwebWindingUseMagicStruck);

        uModValue();

        if (boSendServerConfig)
        {
            GameConfigState.SendServerConfig();
            boSendServerConfig = false;
        }
    }
}

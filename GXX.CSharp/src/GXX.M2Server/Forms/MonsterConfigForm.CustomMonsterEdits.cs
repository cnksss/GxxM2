using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// MonsterConfig.pas 巨片余部第三片（批次J42）：攻击配置逐控件编辑处理器——
/// 客户端组（seClientFly*/chkClientFly*/cbbClientFly* 等约 55 处，写 FCurrentMonsterClientConfig）/
/// 服务端组（chkAttack*/seAttack*/Additionals 0..11/CallMonster 1..4/Protect 等约 45 处，写 FCurrentMonsterServerConfig）/
/// 基础组（cbbClientDrawMode*/声音 11 编辑框 Tag 驱动/血条偏移/视野·类型·守护区等）——
/// 指针语义以下标取 ref 写回 + SetMonsterConfigChanged；CheckBox 为 VCL OnClick 语义（程序赋值不触发）。
/// 另含 tmrFlash 闪烁、LoadStdItems 槽位过滤、lstItemListDblClick 装备槽写入、
/// MenuItem_ShowAllClick、lstItemList·ListBoxMagicList·ListBoxMonsterList 三个 Ctrl+F 查找、
/// cbbBatchAction/Effect 批量填充与 btnCalcStartIndex/EffectIndex 计算。
/// </summary>
public sealed partial class MonsterConfigForm
{
    /// <summary>编辑处理器注册表（键 = Delphi 控件名，值 = 完整 Delphi 处理器体；测试可直接调用）。</summary>
    public readonly Dictionary<string, Action> ClientEditHandlers = new();
    public readonly Dictionary<string, Action> ServerEditHandlers = new();
    public readonly Dictionary<string, Action> BaseEditHandlers = new();

    private bool _suppressCheckCascade;

    // Delphi VCL OnClick 语义：程序赋值 Checked 不触发处理器（SetChk 内压制），用户点击才触发
    private bool HasClient => FCurrentMonsterCustomConfig != null && ClientConfigIndex >= 0;
    private bool HasServer => FCurrentMonsterCustomConfig != null && ServerConfigIndex >= 0;

    private ref TClientAttackConfig CurClient => ref FCurrentMonsterCustomConfig!.ClientAttackConfigs[ClientConfigIndex];
    private TMonsterServerConfig CurServer => FCurrentMonsterCustomConfig!.MonsterServerConfigs[ServerConfigIndex];

    // ---- 装备槽物品列表/闪烁（Delphi lstItemList/tmrFlash） ----
    public System.Windows.Forms.ListBox lstItemList = null!;
    public bool TmrFlashEnabled;
    public int TmrFlashTag;
    public System.Windows.Forms.TextBox? FlashEdit;

    /// <summary>UserEngine.StdItemList 接缝（LoadStdItems 数据源：名称 + StdMode）。</summary>
    public Func<List<(string Name, int StdMode)>>? StdItemListHandler;

    /// <summary>Delphi ShowLeftRighHandMessage 的左右手记忆（unit 全局 LeftRightIndex）。</summary>
    public static int LeftRightIndex = 0;

    /// <summary>左右手指定对话框接缝（确认返回所选 0=左手/1=右手，取消返回 null）。</summary>
    public Func<int?>? LeftRightDialogHandler;

    private System.Windows.Forms.ComboBox _cbbBatchAction = null!;
    private System.Windows.Forms.ComboBox _cbbBatchEffect = null!;

    private void InitializeComponentCustomMonsterEdits()
    {
        var gb = new System.Windows.Forms.GroupBox { Text = "物品列表", Left = 8, Top = 704, Width = 660, Height = 90 };
        Controls.Add(gb);
        lstItemList = new System.Windows.Forms.ListBox { Left = 10, Top = 18, Width = 200, Height = 60 };
        lstItemList.DoubleClick += (_, _) => LstItemListDblClick();
        lstItemList.KeyDown += (s, e) => { if (e.Control && e.KeyCode == Keys.F) LstItemListCtrlF(); };
        gb.Controls.Add(lstItemList);

        // 批量动作/特效与计算基址（FormCreate 文件下拉族补齐；条目由 EnsureEffectListsFilled 填充）
        _cbbBatchAction = new System.Windows.Forms.ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = 240, Top = 18, Width = 110 };
        _cbbBatchEffect = new System.Windows.Forms.ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = 360, Top = 18, Width = 110 };
        _cbbBatchAction.SelectedIndexChanged += (_, _) => CbbBatchActionChange();
        _cbbBatchEffect.SelectedIndexChanged += (_, _) => CbbBatchEffectChange();
        Cbbs["cbbBatchAction"] = _cbbBatchAction;
        Cbbs["cbbBatchEffect"] = _cbbBatchEffect;
        gb.Controls.Add(_cbbBatchAction);
        gb.Controls.Add(_cbbBatchEffect);
        var lblBatchAction = new System.Windows.Forms.Label { Text = "批量动作:", Left = 240, Top = 44, AutoSize = true };
        var lblBatchEffect = new System.Windows.Forms.Label { Text = "批量特效:", Left = 360, Top = 44, AutoSize = true };
        gb.Controls.Add(lblBatchAction);
        gb.Controls.Add(lblBatchEffect);
        var seCalcStart = new System.Windows.Forms.NumericUpDown { Minimum = -2000000000, Maximum = 2000000000, Left = 480, Top = 18, Width = 80 };
        var seCalcEffect = new System.Windows.Forms.NumericUpDown { Minimum = -2000000000, Maximum = 2000000000, Left = 480, Top = 44, Width = 80 };
        Spins["seClientStartIndex"] = seCalcStart;
        Spins["seClientEffectIndex"] = seCalcEffect;
        gb.Controls.Add(seCalcStart);
        gb.Controls.Add(seCalcEffect);
        var btnCalcStart = new System.Windows.Forms.Button { Text = "计算图片位", Left = 570, Top = 16, Width = 80 };
        var btnCalcEffect = new System.Windows.Forms.Button { Text = "计算特效位", Left = 570, Top = 44, Width = 80 };
        btnCalcStart.Click += (_, _) => BtnCalcStartIndexClick();
        btnCalcEffect.Click += (_, _) => BtnCalcEffectIndexClick();
        gb.Controls.Add(btnCalcStart);
        gb.Controls.Add(btnCalcEffect);

        // 斗笠/鼓 槽位（Delphi U_HAT=13/U_DRUM=14，J37 未含）
        foreach (var (slot, name) in new (int Slot, string Name)[] { (13, "斗笠"), (14, "鼓") })
        {
            var edit = new System.Windows.Forms.TextBox { Width = 80, Tag = slot };
            edit.TextChanged += (s, e) => EditDRESSNAMEChange(s);
            EquipEdits[slot] = edit;
            Controls.Add(edit);
            edit.Visible = false;
        }

        BindAllEditHandlers();
    }

    /// <summary>其余槽位全集（else 分支 CheckUserItems 等效：M2Share.pas 11034-11096 全槽位 StdMode 并集）。</summary>
    private static readonly System.Collections.Generic.HashSet<int> KnownStdModes = new()
    {
        5, 6, 7, 10, 11, 12, 15, 16, 19, 20, 21, 22, 23, 24, 25, 26, 28, 29, 30, 51, 52, 53, 54,
        62, 63, 64, 65, 66, 67, 68, 69, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 94, 96, 97,
    };

    /// <summary>lstItemList 行数据（Delphi AddObject 携带 pTStdItem 等效：名称 + StdMode）。</summary>
    private readonly List<(string Name, int StdMode)> _lstItemListData = new();

    /// <summary>LoadStdItems 1:1（-1 全量；已知槽位按 StdMode 白名单；其余 Tag 走 CheckUserItems 等效排除）。</summary>
    public void LoadStdItems(int fileter = -1)
    {
        lstItemList.Items.Clear();
        _lstItemListData.Clear();
        foreach (var (name, stdMode) in StdItemListHandler?.Invoke() ?? new List<(string, int)>())
        {
            if (fileter < 0)
            {
                _lstItemListData.Add((name, stdMode));
                lstItemList.Items.Add(name);
                continue;
            }
            bool add;
            switch (fileter)
            {
                case 0: add = stdMode is 10 or 11; break;
                case 1: add = stdMode is 5 or 6; break;
                case 2: add = stdMode is 28 or 29 or 30; break;
                case 3: add = stdMode is 19 or 20 or 21; break;
                case 4: add = stdMode == 15; break;
                case 5:
                case 6: add = stdMode is 24 or 26; break;
                case 7:
                case 8: add = stdMode is 22 or 23; break;
                case 9: add = stdMode is 25 or 51; break;
                case 10: add = stdMode is 54 or 64; break;
                case 11: add = stdMode is 52 or 62; break;
                case 12: add = stdMode is 53 or 63 or 7; break;
                case 13: add = stdMode == 16; break;
                case 14: add = stdMode == 65; break;
                case 16: add = stdMode == 12; break;
                case 31: add = stdMode == 31; break;
                case 100: add = stdMode is >= 0 and <= 3 or 25; break;
                default: add = !KnownStdModes.Contains(stdMode); break;
            }
            if (add)
            {
                _lstItemListData.Add((name, stdMode));
                lstItemList.Items.Add(name);
            }
        }
    }

    /// <summary>MenuItem_ShowAllClick 1:1（互斥勾选 → 未勾选的置勾并按 Tag 过滤）。</summary>
    public int CurrentFilterTag = -1;

    public void MenuItemShowAllClick(int tag)
    {
        CurrentFilterTag = tag; // 其余菜单项 Checked := False（单选组等效）
        LoadStdItems(tag);
    }

    /// <summary>lstItemListDblClick 1:1（GetTakeOnPosition 定槽 → 左右手对话框 → 写编辑框 → tmrFlash 闪烁）。</summary>
    public void LstItemListDblClick()
    {
        if (SelMonsterConfig == null)
            return;
        if (lstItemList.SelectedIndex < 0)
            return;
        var item = _lstItemListData[lstItemList.SelectedIndex];
        int where = GetTakeOnPosition(item.StdMode);
        int? slot = where switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 3,
            4 => 4,
            5 or 6 => LeftRightMessage() == 0 ? 5 : 6,
            7 or 8 => LeftRightMessage() == 0 ? 7 : 8,
            9 => 9,
            10 => 10,
            11 => 11,
            12 => 12,
            13 => 13,
            14 => 14,
            16 => 16,
            _ => null,
        };
        if (slot != null)
        {
            var edit = EquipEdits[slot.Value];
            edit.Text = item.Name;
            TmrFlashEnabled = false;
            if (FlashEdit != null)
                FlashEdit.BackColor = System.Drawing.SystemColors.Window;
            FlashEdit = edit;
            TmrFlashTag = 0;
            TmrFlashEnabled = true;
        }
    }

    /// <summary>ShowLeftRighHandMessage（对话框接缝 + LeftRightIndex 记忆，默认沿用上次选择）。</summary>
    private int LeftRightMessage()
    {
        var chosen = LeftRightDialogHandler?.Invoke() ?? LeftRightIndex;
        LeftRightIndex = chosen;
        return LeftRightIndex;
    }

    /// <summary>tmrFlashTimer 1:1（clWindow↔clYellow 交替，Tag 计满 10 复位停表）。</summary>
    public void TmrFlashTick()
    {
        if (FlashEdit == null)
            return;
        FlashEdit.BackColor = FlashEdit.BackColor == System.Drawing.SystemColors.Window
            ? System.Drawing.Color.Yellow
            : System.Drawing.SystemColors.Window;
        TmrFlashTag++;
        if (TmrFlashTag >= 10)
        {
            TmrFlashTag = 0;
            FlashEdit.BackColor = System.Drawing.SystemColors.Window;
            FlashEdit = null;
            TmrFlashEnabled = false;
        }
    }

    // ---- 三个 Ctrl+F 查找（lstItemList/ListBoxMagicList/ListBoxMonsterList） ----

    private void ListBoxCtrlF(System.Windows.Forms.ListBox box, string caption, string prompt)
    {
        if (InputQueryHandler == null || !InputQueryHandler(caption, prompt))
            return;
        string key = LastInputQueryText;
        if (key.Length == 0)
            return;
        for (int i = 0; i < box.Items.Count; i++)
        {
            if ((box.Items[i]?.ToString() ?? "") == key)
            {
                box.SelectedIndex = i;
                break;
            }
        }
    }

    /// <summary>lstItemListKeyDown Ctrl+F（物品查找，精确等值）。</summary>
    public void LstItemListCtrlF() => ListBoxCtrlF(lstItemList, "物品查找", "输入物品名称:");

    /// <summary>ListBoxMagicListKeyDown Ctrl+F（魔法查找）。</summary>
    public void ListBoxMagicListCtrlF() => ListBoxCtrlF(ListBoxMagicList, "魔法查找", "输入魔法名称:");

    /// <summary>ListBoxMonsterListKeyDown Ctrl+F（人形怪查找）。</summary>
    public void ListBoxMonsterListCtrlF() => ListBoxCtrlF(ListBoxMonsterList, "人形怪查找", "输入人形怪名称:");

    // ---- 批量填充与计算（cbbBatchAction/Effect、btnCalcStartIndex/EffectIndex） ----

    /// <summary>cbbBatchActionChange 1:1（全动作 ActionFile = ItemIndex-1）。</summary>
    public void CbbBatchActionChange()
    {
        if (VstAction.Items.Count == 0)
            return;
        for (int i = 0; i < 12; i++)
            FCurrentMonsterCustomConfig!.ClientActions[i].ActionFile = (short)(_cbbBatchAction.SelectedIndex - 1);
        SetMonsterConfigChanged();
    }

    /// <summary>cbbBatchEffectChange 1:1（全动作 EffectFile = ItemIndex-1）。</summary>
    public void CbbBatchEffectChange()
    {
        if (VstAction.Items.Count == 0)
            return;
        for (int i = 0; i < 12; i++)
            FCurrentMonsterCustomConfig!.ClientActions[i].EffectFile = (short)(_cbbBatchEffect.SelectedIndex - 1);
        SetMonsterConfigChanged();
    }

    private static int CalcIndexOffset(int actionType) => actionType switch
    {
        0 => 0, // matStand
        1 => 1, // matWalk
        2 => 2, // matDefAttack
        3 => 3, // matStruck
        4 => 4, // matDie
        _ => -1, // matStoneMode 注释保留；其余动作不计
    };

    /// <summary>btnCalcStartIndexClick 1:1（站/走/普攻/被击/死亡 StartIndex = 基址 + Offset×80）。</summary>
    public void BtnCalcStartIndexClick()
    {
        if (VstAction.Items.Count == 0 || FCurrentMonsterCustomConfig == null)
            return;
        int baseValue = (int)Spins["seClientStartIndex"].Value;
        for (int i = 0; i < 12; i++)
        {
            int offset = CalcIndexOffset(i);
            if (offset != -1)
                FCurrentMonsterCustomConfig.ClientActions[i].StartIndex = (short)(baseValue < 0 ? -1 : baseValue + offset * 80);
        }
        SetMonsterConfigChanged();
    }

    /// <summary>btnCalcEffectIndexClick 1:1（EffectIndex 同规则）。</summary>
    public void BtnCalcEffectIndexClick()
    {
        if (VstAction.Items.Count == 0 || FCurrentMonsterCustomConfig == null)
            return;
        int baseValue = (int)Spins["seClientEffectIndex"].Value;
        for (int i = 0; i < 12; i++)
        {
            int offset = CalcIndexOffset(i);
            if (offset != -1)
                FCurrentMonsterCustomConfig.ClientActions[i].EffectIndex = (short)(baseValue < 0 ? -1 : baseValue + offset * 80);
        }
        SetMonsterConfigChanged();
    }

    // ---- 编辑处理器绑定（Delphi 守卫 + 写回 + SetMonsterConfigChanged 三段式） ----

    private void BindSpinC(string name, Action write)
    {
        ClientEditHandlers[name] = () => { if (HasClient) { write(); SetMonsterConfigChanged(); } };
        Spins[name].ValueChanged += (_, _) => ClientEditHandlers[name]();
    }

    private void BindCbbC(string name, Action write)
    {
        ClientEditHandlers[name] = () => { if (HasClient) { write(); SetMonsterConfigChanged(); } };
        Cbbs[name].SelectedIndexChanged += (_, _) => ClientEditHandlers[name]();
    }

    private void BindChkC(string name, Action write)
    {
        ClientEditHandlers[name] = () => { if (HasClient) { write(); SetMonsterConfigChanged(); } };
        Chks[name].CheckedChanged += (_, _) => { if (!_suppressCheckCascade) ClientEditHandlers[name](); };
    }

    private void BindSpinS(string name, Action write)
    {
        ServerEditHandlers[name] = () => { if (HasServer) { write(); SetMonsterConfigChanged(); } };
        Spins[name].ValueChanged += (_, _) => ServerEditHandlers[name]();
    }

    private void BindCbbS(string name, Action write)
    {
        ServerEditHandlers[name] = () => { if (HasServer) { write(); SetMonsterConfigChanged(); } };
        Cbbs[name].SelectedIndexChanged += (_, _) => ServerEditHandlers[name]();
    }

    private void BindChkS(string name, Action write)
    {
        ServerEditHandlers[name] = () => { if (HasServer) { write(); SetMonsterConfigChanged(); } };
        Chks[name].CheckedChanged += (_, _) => { if (!_suppressCheckCascade) ServerEditHandlers[name](); };
    }

    private void BindEditS(string name, Action write)
    {
        ServerEditHandlers[name] = () => { if (HasServer) { write(); SetMonsterConfigChanged(); } };
        Edits[name].TextChanged += (_, _) => ServerEditHandlers[name]();
    }

    private void BindSpinB(string name, Action write)
    {
        BaseEditHandlers[name] = () => { if (FCurrentMonsterCustomConfig != null) { write(); SetMonsterConfigChanged(); } };
        Spins[name].ValueChanged += (_, _) => BaseEditHandlers[name]();
    }

    private void BindCbbB(string name, Action write)
    {
        BaseEditHandlers[name] = () => { if (FCurrentMonsterCustomConfig != null) { write(); SetMonsterConfigChanged(); } };
        Cbbs[name].SelectedIndexChanged += (_, _) => BaseEditHandlers[name]();
    }

    private void BindChkB(string name, Action write)
    {
        BaseEditHandlers[name] = () => { if (FCurrentMonsterCustomConfig != null) { write(); SetMonsterConfigChanged(); } };
        Chks[name].CheckedChanged += (_, _) => { if (!_suppressCheckCascade) BaseEditHandlers[name](); };
    }

    private void BindEditB(string name, Action write)
    {
        BaseEditHandlers[name] = () => { if (FCurrentMonsterCustomConfig != null) { write(); SetMonsterConfigChanged(); } };
        Edits[name].TextChanged += (_, _) => BaseEditHandlers[name]();
    }

    /// <summary>注册全部编辑处理器（Delphi 约 100 处 *Change 的绑定表）。</summary>
    private void BindAllEditHandlers()
    {
        // ================= 客户端组（FCurrentMonsterClientConfig） =================
        BindCbbC("cbbClientFlyFile", () => CurClient.Fly_File = (short)(Cbbs["cbbClientFlyFile"].SelectedIndex - 1));
        BindCbbC("cbbClientFlyEffFile", () => CurClient.FlyEff_File = (short)(Cbbs["cbbClientFlyEffFile"].SelectedIndex - 1));
        BindCbbC("cbbClientSelfFile", () => CurClient.Self_File = (short)(Cbbs["cbbClientSelfFile"].SelectedIndex - 1));
        BindCbbC("cbbClientSelfKeepFile", () => CurClient.SelfKeep_File = (short)(Cbbs["cbbClientSelfKeepFile"].SelectedIndex - 1));
        BindCbbC("cbbClientExplosionFile", () => CurClient.Explosion_File = (short)(Cbbs["cbbClientExplosionFile"].SelectedIndex - 1));
        BindCbbC("cbbClientTargetFile", () => CurClient.Target_File = (short)(Cbbs["cbbClientTargetFile"].SelectedIndex - 1));

        BindSpinC("seClientFlyStartIndex", () => CurClient.Fly_StartIndex = (short)Spins["seClientFlyStartIndex"].Value);
        BindSpinC("seClientFlyPlayCount", () => CurClient.Fly_PlayCount = (ushort)Spins["seClientFlyPlayCount"].Value);
        BindSpinC("seClientFlyEmptyCount", () => CurClient.Fly_EmptyCount = (ushort)Spins["seClientFlyEmptyCount"].Value);
        BindSpinC("seClientFlyPlayTime", () => CurClient.Fly_PlayTime = (ushort)Spins["seClientFlyPlayTime"].Value);
        BindSpinC("seFlyLightRange", () => CurClient.Fly_LightRange = (byte)Spins["seFlyLightRange"].Value);
        BindSpinC("seClientFlyEffStartIndex", () => CurClient.FlyEff_StartIndex = (short)Spins["seClientFlyEffStartIndex"].Value);
        BindSpinC("seClientSelfStartIndex", () => CurClient.Self_StartIndex = (short)Spins["seClientSelfStartIndex"].Value);
        BindSpinC("seClientSelfPlayCount", () => CurClient.Self_PlayCount = (ushort)Spins["seClientSelfPlayCount"].Value);
        BindSpinC("seClientSelfEmptyCount", () => CurClient.Self_EmptyCount = (ushort)Spins["seClientSelfEmptyCount"].Value);
        BindSpinC("seClientSelfPlayTime", () => CurClient.Self_PlayTime = (ushort)Spins["seClientSelfPlayTime"].Value);
        BindSpinC("seSelfLightRange", () => CurClient.Self_LightRange = (byte)Spins["seSelfLightRange"].Value);
        BindSpinC("seClientSelfKeepStartIndex", () => CurClient.SelfKeep_StartIndex = (short)Spins["seClientSelfKeepStartIndex"].Value);
        BindSpinC("seClientSelfKeepStartIndex2", () => CurClient.SelfKeep_StartIndex2 = (short)Spins["seClientSelfKeepStartIndex2"].Value);
        BindSpinC("seClientSelfKeepPlayCount", () => CurClient.SelfKeep_PlayCount = (ushort)Spins["seClientSelfKeepPlayCount"].Value);
        BindSpinC("seClientSelfKeepPlayTime", () => CurClient.SelfKeep_PlayTime = (ushort)Spins["seClientSelfKeepPlayTime"].Value);
        BindSpinC("seClientSelfKeepTime", () => CurClient.SelfKeep_KeepTime = (ushort)Spins["seClientSelfKeepTime"].Value);
        BindSpinC("seClientExplosionStartIndex", () => CurClient.Explosion_StartIndex = (short)Spins["seClientExplosionStartIndex"].Value);
        BindSpinC("seClientExplosionStartIndex2", () => CurClient.Explosion_StartIndex2 = (short)Spins["seClientExplosionStartIndex2"].Value);
        BindSpinC("seClientExplosionPlayCount", () => CurClient.Explosion_PlayCount = (ushort)Spins["seClientExplosionPlayCount"].Value);
        BindSpinC("seClientExplosionPlayTime", () => CurClient.Explosion_PlayTime = (ushort)Spins["seClientExplosionPlayTime"].Value);
        BindSpinC("seExplosionLightRange", () => CurClient.Explosion_LightRange = (byte)Spins["seExplosionLightRange"].Value);
        BindSpinC("seClientExplosionKeepTime", () => CurClient.Explosion_KeepTime = (ushort)Spins["seClientExplosionKeepTime"].Value);
        BindSpinC("seClientExplosionKeepAttackInterval", () => CurClient.Explosion_KeepAttackInterval = (byte)Spins["seClientExplosionKeepAttackInterval"].Value);
        BindSpinC("seClientExplosionKeepAttackRange", () => CurClient.Explosion_KeepAttackRange = (byte)Spins["seClientExplosionKeepAttackRange"].Value);
        BindSpinC("seExplosionKeepLightRange", () => CurClient.Explosion_KeepLightRange = (byte)Spins["seExplosionKeepLightRange"].Value);
        BindSpinC("seClientTargetStartIndex", () => CurClient.Target_StartIndex = (short)Spins["seClientTargetStartIndex"].Value);
        BindSpinC("seClientTargetStartIndex2", () => CurClient.Target_StartIndex2 = (short)Spins["seClientTargetStartIndex2"].Value);
        BindSpinC("seClientTargetPlayCount", () => CurClient.Target_PlayCount = (ushort)Spins["seClientTargetPlayCount"].Value);
        BindSpinC("seClientTargetPlayTime", () => CurClient.Target_PlayTime = (ushort)Spins["seClientTargetPlayTime"].Value);
        BindSpinC("seTargetLightRange", () => CurClient.Target_LightRange = (byte)Spins["seTargetLightRange"].Value);
        BindSpinC("seTargetKeepTime", () => CurClient.Target_KeepTime = (ushort)Spins["seTargetKeepTime"].Value);
        BindSpinC("seTargetKeepAttackRange", () => CurClient.Target_KeepAttackRange = (byte)Spins["seTargetKeepAttackRange"].Value);
        BindSpinC("seTargetKeepAttackInterval", () => CurClient.Target_KeepAttackInterval = (byte)Spins["seTargetKeepAttackInterval"].Value);
        BindSpinC("seTargetKeepLightRange", () => CurClient.Target_KeepLightRange = (byte)Spins["seTargetKeepLightRange"].Value);

        BindCbbC("cbbClientFlyDrawMode", () => CurClient.Fly_DrawMode = (TCustomDrawMode)Cbbs["cbbClientFlyDrawMode"].SelectedIndex);
        BindCbbC("cbbClientFlyDirCount", () => CurClient.Fly_DirCount = (TCustomDirCount)Cbbs["cbbClientFlyDirCount"].SelectedIndex);
        BindCbbC("cbbClientFlyEffDrawMode", () => CurClient.FlyEff_DrawMode = (TCustomDrawMode)Cbbs["cbbClientFlyEffDrawMode"].SelectedIndex);
        BindCbbC("cbbClientSelfDrawOrder", () => CurClient.Self_DrawOrder = (TCustomDrawOrder)Cbbs["cbbClientSelfDrawOrder"].SelectedIndex);
        BindCbbC("cbbClientSelfDrawMode", () => CurClient.Self_DrawMode = (TCustomDrawMode)Cbbs["cbbClientSelfDrawMode"].SelectedIndex);
        BindCbbC("cbbClientSelfDirCalcType", () => CurClient.Self_DirCalcType = (TCustomDirCalcType)Cbbs["cbbClientSelfDirCalcType"].SelectedIndex);
        BindCbbC("cbbClientSelfDirCount", () => CurClient.Self_DirCount = (TCustomDirCount)Cbbs["cbbClientSelfDirCount"].SelectedIndex);
        BindCbbC("cbbClientSelfKeepDrawOrder", () => CurClient.SelfKeep_DrawOrder = (TCustomDrawOrder)Cbbs["cbbClientSelfKeepDrawOrder"].SelectedIndex);
        BindCbbC("cbbClientSelfKeepDrawMode", () => CurClient.SelfKeep_DrawMode = (TCustomDrawMode)Cbbs["cbbClientSelfKeepDrawMode"].SelectedIndex);
        BindCbbC("cbbClientSelfKeepDrawMode2", () => CurClient.SelfKeep_DrawMode2 = (TCustomDrawMode)Cbbs["cbbClientSelfKeepDrawMode2"].SelectedIndex);
        BindCbbC("cbbClientExplosionDrawMode", () => CurClient.Explosion_DrawMode = (TCustomDrawMode)Cbbs["cbbClientExplosionDrawMode"].SelectedIndex);
        BindCbbC("cbbClientExplosionDrawMode2", () => CurClient.Explosion_DrawMode2 = (TCustomDrawMode)Cbbs["cbbClientExplosionDrawMode2"].SelectedIndex);
        BindCbbC("cbbClientTargetDrawMode", () => CurClient.Target_DrawMode = (TCustomDrawMode)Cbbs["cbbClientTargetDrawMode"].SelectedIndex);
        BindCbbC("cbbClientTargetDrawMode2", () => CurClient.Target_DrawMode2 = (TCustomDrawMode)Cbbs["cbbClientTargetDrawMode2"].SelectedIndex);

        BindChkC("chkClientFlyCalcDir", () => CurClient.Fly_CalcDir = (byte)(Chks["chkClientFlyCalcDir"].Checked ? 1 : 0));
        BindChkC("chkClientSelfPlayDelayAction", () => CurClient.Self_PlayDelayAction = (byte)(Chks["chkClientSelfPlayDelayAction"].Checked ? 1 : 0));
        BindChkC("chkClientExplosionLockDraw", () => CurClient.Explosion_LockDraw = (byte)(Chks["chkClientExplosionLockDraw"].Checked ? 1 : 0));
        BindChkC("chkClientExplosionKeepPlay", () => CurClient.Explosion_KeepPlay = (byte)(Chks["chkClientExplosionKeepPlay"].Checked ? 1 : 0));
        BindChkC("chkClientExplosionKeepMultiPlay", () => CurClient.Explosion_KeepMultiPlay = (byte)(Chks["chkClientExplosionKeepMultiPlay"].Checked ? 1 : 0));
        BindChkC("chkClientTargetMultiPlay", () => CurClient.Target_MultiPlay = (byte)(Chks["chkClientTargetMultiPlay"].Checked ? 1 : 0));
        BindChkC("chkClientTargetLockDraw", () => CurClient.Target_LockDraw = (byte)(Chks["chkClientTargetLockDraw"].Checked ? 1 : 0));
        BindChkC("chkTargetKeepPlay", () => CurClient.Target_KeepPlay = (byte)(Chks["chkTargetKeepPlay"].Checked ? 1 : 0));
        BindChkC("chkTargetKeepMultiPlay", () => CurClient.Target_KeepMultiPlay = (byte)(Chks["chkTargetKeepMultiPlay"].Checked ? 1 : 0));
        // cbbClientSelfPlayModeChange 原文整体注释 → 无写回（不注册）

        // ================= 服务端组（FCurrentMonsterServerConfig） =================
        BindChkS("chkAttackEnabled", () => CurServer.AttackEnabled = Chks["chkAttackEnabled"].Checked);
        BindChkS("chkAttackSelfDie", () => CurServer.AttackSelfDie = Chks["chkAttackSelfDie"].Checked);
        BindSpinS("seAttackDelayTime", () => CurServer.AttackDelayTime = (int)Spins["seAttackDelayTime"].Value);
        BindSpinS("seAttackHPPercent", () => CurServer.AttackHPPercent = (int)Spins["seAttackHPPercent"].Value);
        BindSpinS("seAttackRate", () => CurServer.AttackRate = (int)Spins["seAttackRate"].Value);
        BindSpinS("seAttackTargetCount", () => CurServer.AttackTargetCount = (int)Spins["seAttackTargetCount"].Value);
        BindCbbS("cbbAttackMode", () => CurServer.AttackMode = (TCustomAttackMode)Cbbs["cbbAttackMode"].SelectedIndex);
        BindCbbS("cbbAttackTarget", () => CurServer.AttackTarget = (TCustomAttackTarget)Cbbs["cbbAttackTarget"].SelectedIndex);
        BindCbbS("cbbAttackPowerCalc", () => CurServer.AttackPowerCalc = (TCustomAttackPowerCalc)Cbbs["cbbAttackPowerCalc"].SelectedIndex);
        BindSpinS("seAttackPowerRate", () => CurServer.AttackPowerRate = (int)Spins["seAttackPowerRate"].Value);
        BindChkS("chkAttackTeleportAttack", () => CurServer.AttackTeleportAttack = Chks["chkAttackTeleportAttack"].Checked);
        BindChkS("chkAttackTeleportRush", () => CurServer.AttackTeleportRush = Chks["chkAttackTeleportRush"].Checked);
        BindSpinS("seAttackTeleportTargetDistance", () => CurServer.AttackTeleportTargetDistance = (int)Spins["seAttackTeleportTargetDistance"].Value);
        BindSpinS("seAttackTeleportDistance", () => CurServer.AttackTeleportDistance = (int)Spins["seAttackTeleportDistance"].Value);
        BindSpinS("seAttackTeleportRate", () => CurServer.AttackTeleportRate = (int)Spins["seAttackTeleportRate"].Value);
        BindChkS("chkAttackIgnoreDefence", () => CurServer.AttackIgnoreDefence = Chks["chkAttackIgnoreDefence"].Checked);
        BindSpinS("seAttackNearRange", () => CurServer.AttackNearRange = (int)Spins["seAttackNearRange"].Value);
        BindSpinS("seAttackGroupRange", () => CurServer.AttackGroupRange = (int)Spins["seAttackGroupRange"].Value);
        BindChkS("chkNearAttackTargetCenter", () => CurServer.NearAttackTargetCenter = Chks["chkNearAttackTargetCenter"].Checked);
        BindSpinS("seAttackPowerInc", () => CurServer.AttackPowerInc = (int)Spins["seAttackPowerInc"].Value);

        // 附加状态 0..11（Delphi chkAdditional0Click/seAdditionalRate0Change/seAdditionalTime0Change 的 Tag 驱动组）
        for (int i = 0; i < 12; i++)
        {
            int idx = i;
            BindChkS($"chkAdditional{i}", () => CurServer.Additionals[idx].Checked = Chks[$"chkAdditional{idx}"].Checked);
            BindSpinS($"seAdditionalRate{i}", () => CurServer.Additionals[idx].Rate = (byte)Spins[$"seAdditionalRate{idx}"].Value);
            BindSpinS($"seAdditionalTime{i}", () => CurServer.Additionals[idx].Time = (ushort)Spins[$"seAdditionalTime{idx}"].Value);
        }
        BindSpinS("seAdditionaHP0", () => CurServer.AdditionalHP0 = (int)Spins["seAdditionaHP0"].Value);
        BindSpinS("seAdditionalImprisonRange", () => CurServer.AdditionalImprisonRange = (int)Spins["seAdditionalImprisonRange"].Value);
        BindChkS("chkseAdditionaHighLevel4", () => CurServer.AdditionalHighLevel4 = Chks["chkseAdditionaHighLevel4"].Checked);

        BindChkS("chkEnabledCallMonster", () => CurServer.EnabledCallMonster = Chks["chkEnabledCallMonster"].Checked);
        BindSpinS("seCallMonstersRate", () => CurServer.CallMonstersRate = (int)Spins["seCallMonstersRate"].Value);
        for (int i = 1; i <= 4; i++)
        {
            int idx = i - 1;
            BindEditS($"edtCallMonster{i}", () => CurServer.CallMonsters[idx] = Edits[$"edtCallMonster{idx + 1}"].Text);
            BindSpinS($"seCallMonsterNum{i}", () => CurServer.CallMonsterNums[idx] = (int)Spins[$"seCallMonsterNum{idx + 1}"].Value);
        }
        // chkCopySelf/seCopySelfMaxCount/seCopySelfTime 原文整体注释 → 无写回

        BindChkS("chkProtectAddHP", () => CurServer.ProtectAddHP = Chks["chkProtectAddHP"].Checked);
        BindSpinS("seProtectAddHPRate", () => CurServer.ProtectAddHPRate = (int)Spins["seProtectAddHPRate"].Value);
        BindSpinS("seProtectAddHPPercent", () => CurServer.ProtectAddHPPercent = (int)Spins["seProtectAddHPPercent"].Value);
        BindChkS("chkProtectAddDefence", () => CurServer.ProtectAddDefence = Chks["chkProtectAddDefence"].Checked);
        BindChkS("chkProtectAddMagDefence", () => CurServer.ProtectAddMagDefence = Chks["chkProtectAddMagDefence"].Checked);
        BindSpinS("seProtectAddDefenceRate", () => CurServer.ProtectAddDefenceRate = (int)Spins["seProtectAddDefenceRate"].Value);
        BindSpinS("seProtectAddDefencePercent", () => CurServer.ProtectAddDefencePercent = (int)Spins["seProtectAddDefencePercent"].Value);
        BindSpinS("seProtectAddDefenceTime", () => CurServer.ProtectAddDefenceTime = (int)Spins["seProtectAddDefenceTime"].Value);
        BindSpinS("seProtectAddMagDefenceRate", () => CurServer.ProtectAddMagDefenceRate = (int)Spins["seProtectAddMagDefenceRate"].Value);
        BindSpinS("seProtectAddMagDefencePercent", () => CurServer.ProtectAddMagDefencePercent = (int)Spins["seProtectAddMagDefencePercent"].Value);
        BindSpinS("seProtectAddMagDefenceTime", () => CurServer.ProtectAddMagDefenceTime = (int)Spins["seProtectAddMagDefenceTime"].Value);
        BindChkS("chkProtectAddDC", () => CurServer.ProtectAddDC = Chks["chkProtectAddDC"].Checked);
        BindSpinS("seProtectAddDCRate", () => CurServer.ProtectAddDCRate = (int)Spins["seProtectAddDCRate"].Value);
        BindSpinS("seProtectAddDCPercent", () => CurServer.ProtectAddDCPercent = (int)Spins["seProtectAddDCPercent"].Value);
        BindSpinS("seProtectAddDCTime", () => CurServer.ProtectAddDCTime = (int)Spins["seProtectAddDCTime"].Value);
        BindChkS("chkProtectAddMC", () => CurServer.ProtectAddMC = Chks["chkProtectAddMC"].Checked);
        BindSpinS("seProtectAddMCRate", () => CurServer.ProtectAddMCRate = (int)Spins["seProtectAddMCRate"].Value);
        BindSpinS("seProtectAddMCPercent", () => CurServer.ProtectAddMCPercent = (int)Spins["seProtectAddMCPercent"].Value);
        BindSpinS("seProtectAddMCTime", () => CurServer.ProtectAddMCTime = (int)Spins["seProtectAddMCTime"].Value);
        BindChkS("chkProtectAddSC", () => CurServer.ProtectAddSC = Chks["chkProtectAddSC"].Checked);
        BindSpinS("seProtectAddSCRate", () => CurServer.ProtectAddSCRate = (int)Spins["seProtectAddSCRate"].Value);
        BindSpinS("seProtectAddSCPercent", () => CurServer.ProtectAddSCPercent = (int)Spins["seProtectAddSCPercent"].Value);
        BindSpinS("seProtectAddSCTime", () => CurServer.ProtectAddSCTime = (int)Spins["seProtectAddSCTime"].Value);
        BindSpinS("seProtectTargetRange", () => CurServer.ProtectTargetRange = (int)Spins["seProtectTargetRange"].Value);
        BindSpinS("seProtectSelfRate", () => CurServer.ProtectSelfRate = (int)Spins["seProtectSelfRate"].Value);
        BindChkS("chkMoveTarget", () => CurServer.MoveTarget = Chks["chkMoveTarget"].Checked);
        BindSpinS("seMoveTargetRate", () => CurServer.MoveTargetRate = (int)Spins["seMoveTargetRate"].Value);
        BindChkS("chkMoveTargetHighLevel", () => CurServer.MoveTargetHighLevel = Chks["chkMoveTargetHighLevel"].Checked);

        // ================= 基础组（FCurrentMonsterCustomConfig 直写） =================
        BindCbbB("cbbClientDrawMode", () => FCurrentMonsterCustomConfig!.ClientBaseConfig.DrawMode = (TCustomDrawMode)Cbbs["cbbClientDrawMode"].SelectedIndex);
        BindCbbB("cbbClientDrawMode2", () => FCurrentMonsterCustomConfig!.ClientBaseConfig.DrawMode2 = (TCustomDrawMode)Cbbs["cbbClientDrawMode2"].SelectedIndex);
        BindCbbB("cbbClientDrawOrder2", () => FCurrentMonsterCustomConfig!.ClientBaseConfig.DrawOrder = (TMonsterDrawOrder2)Cbbs["cbbClientDrawOrder2"].SelectedIndex);
        BindChkB("chkDieNoCalcDir", () => FCurrentMonsterCustomConfig!.ClientBaseConfig.DieNoCalcDir = (byte)(Chks["chkDieNoCalcDir"].Checked ? 1 : 0));
        BindCbbB("cbbHPFile", () => FCurrentMonsterCustomConfig!.ClientBaseConfig.HPFile = Cbbs["cbbHPFile"].SelectedIndex - 1);
        BindSpinB("seHPBgOffsetX", () => FCurrentMonsterCustomConfig!.ClientBaseConfig.HPBgOffsetX = (int)Spins["seHPBgOffsetX"].Value);
        BindSpinB("seHPBgOffsetY", () => FCurrentMonsterCustomConfig!.ClientBaseConfig.HPBgOffsetY = (int)Spins["seHPBgOffsetY"].Value);
        BindSpinB("seHPOffsetX", () => FCurrentMonsterCustomConfig!.ClientBaseConfig.HPOffsetX = (int)Spins["seHPOffsetX"].Value);
        BindSpinB("seHPOffsetY", () => FCurrentMonsterCustomConfig!.ClientBaseConfig.HPOffsetY = (int)Spins["seHPOffsetY"].Value);
        BindSpinB("seHPStartIndex", () => FCurrentMonsterCustomConfig!.ClientBaseConfig.HPStartIndex = (int)Spins["seHPStartIndex"].Value);
        BindSpinB("seHPTextOffsetX", () => FCurrentMonsterCustomConfig!.ClientBaseConfig.HPTextOffsetX = (int)Spins["seHPTextOffsetX"].Value);
        BindSpinB("seHPTextOffsetY", () => FCurrentMonsterCustomConfig!.ClientBaseConfig.HPTextOffsetY = (int)Spins["seHPTextOffsetY"].Value);
        // 声音 11 编辑框（Delphi edtSoundNormalChange 的 Tag=mstNormal..mstAttack6 驱动组）
        for (int i = 0; i < 11; i++)
        {
            int idx = i;
            string name = new[]
            {
                "edtSoundNormal", "edtSoundDigUP", "edtSoundAttack", "edtSoundStruck", "edtSoundDie",
                "edtSoundAttack1", "edtSoundAttack2", "edtSoundAttack3", "edtSoundAttack4", "edtSoundAttack5", "edtSoundAttack6",
            }[i];
            BindEditB(name, () => FCurrentMonsterCustomConfig!.ClientBaseConfig.Sounds[idx].Value = Edits[name].Text);
        }

        BindSpinB("seViewRange", () => FCurrentMonsterCustomConfig!.ServerBaseConfig.ViewRange = (byte)Spins["seViewRange"].Value);
        BindCbbB("cbbMonsterType", () => FCurrentMonsterCustomConfig!.ServerBaseConfig.MonsterType = (TMonsterType)Cbbs["cbbMonsterType"].SelectedIndex);
        BindSpinB("seProtectRange", () => FCurrentMonsterCustomConfig!.ServerBaseConfig.ProtectRange = (int)Spins["seProtectRange"].Value);
        BindSpinB("seMinAttackNearRange", () => FCurrentMonsterCustomConfig!.ServerBaseConfig.MinAttackNearRange = (int)Spins["seMinAttackNearRange"].Value);
        BindSpinB("seLightRange", () => FCurrentMonsterCustomConfig!.ServerBaseConfig.LightRange = (byte)Spins["seLightRange"].Value);
        BindChkB("chkNoAttack", () => FCurrentMonsterCustomConfig!.ServerBaseConfig.NoAttack = Chks["chkNoAttack"].Checked);

        // OperateMode/MoveOption 已有专用处理器方法（J41），此处补事件接线（Delphi OnChange 语义）
        Cbbs["cbbOperateMode"].SelectedIndexChanged += (_, _) => CbbOperateModeChange();
        Cbbs["cbbMoveOption"].SelectedIndexChanged += (_, _) => CbbMoveOptionChange();
        // 攻击组选择下拉自身（Delphi OnChange = cbbClientAttackConfigChange/cbbServerAttackConfigChange）
        Cbbs["cbbClientAttackConfig"].SelectedIndexChanged += (_, _) => CbbClientAttackConfigChange();
        Cbbs["cbbServerAttackConfig"].SelectedIndexChanged += (_, _) => CbbServerAttackConfigChange();
    }
}

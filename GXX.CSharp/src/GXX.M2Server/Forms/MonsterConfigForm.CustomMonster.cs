using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// MonsterConfig.pas 巨片余部第二片（批次J41）：自定义怪编辑树——
/// vstCustomMonster 树构建（Open 按 m_CustomMonsterList 填充）/ NodeClick 回填（vstAction 勾选树 +
/// ClientBaseConfig 声音·血条组 + ServerBaseConfig 视野·类型·移动组 + Race 描述与页签可见性）/
/// cbbClientAttackConfigChange·cbbServerAttackConfigChange 两组攻击效果配置回填 /
/// vstActionChecked CalcDir 写回 / SetMonsterConfigChanged 红名标记 / Ctrl+F 查找 /
/// btnSave 逐怪落盘 / btn1 生成登录器配置文件（SaveCustomMonsterClientConfigs）。
/// 逐控件 *Change 编辑处理器随下一片接入；本片控件注册表以 Delphi 控件名索引。
/// </summary>
public sealed partial class MonsterConfigForm
{
    // ---- 自定义怪编辑树控件（Delphi 控件名索引） ----
    public System.Windows.Forms.ListView VstCustomMonster = null!;
    public System.Windows.Forms.ListView VstAction = null!;
    public System.Windows.Forms.Button btnSave = null!;
    public System.Windows.Forms.Button btn1 = null!;
    public string PnlMonDescCaption = "";

    /// <summary>Delphi SetControlEnabled(pgcMain, ...) 等效。</summary>
    public bool PgcMainEnabled;
    public bool TsAttackVisible = true;
    public bool TsServerAttackVisible = true;
    public bool LblProtectVisible;
    public bool SeAttackTargetCountEnabled = true;
    public bool GrpOptionsVisible = true;
    public bool GrpProtectVisible;
    public bool FIsMonsterChanged;

    public TCustomMonsterConfig? FCurrentMonsterCustomConfig;

    /// <summary>FCurrentMonsterClientConfig/FCurrentMonsterServerConfig 指针语义 → 当前数组下标（-1 = nil）。</summary>
    public int ClientConfigIndex = -1;
    public int ServerConfigIndex = -1;

    /// <summary>UserEngine.m_CustomMonsterList 接缝（Open 树构建）。</summary>
    public Func<List<TCustomMonsterConfig>>? CustomMonsterConfigsHandler;

    /// <summary>g_EffectImageList 接缝（文件下拉 = '根据Appr计算' + 特效图片名表）。</summary>
    public Func<List<string>>? EffectImageListHandler;

    /// <summary>dlgSaveMonsters 接缝（确认返回文件名，取消返回 null）。</summary>
    public Func<string?, string?>? SaveMonsterDialogHandler;

    /// <summary>ShowMessage 接缝记录（btn1 提示文案）。</summary>
    public string? LastShowMessage;

    /// <summary>控件注册表（键 = Delphi 控件名）。</summary>
    public readonly Dictionary<string, System.Windows.Forms.ComboBox> Cbbs = new();
    public readonly Dictionary<string, System.Windows.Forms.NumericUpDown> Spins = new();
    public readonly Dictionary<string, System.Windows.Forms.CheckBox> Chks = new();
    public readonly Dictionary<string, System.Windows.Forms.TextBox> Edits = new();

    private System.Windows.Forms.ComboBox _cbbClientAttackConfig = null!;
    private System.Windows.Forms.ComboBox _cbbServerAttackConfig = null!;
    private System.Windows.Forms.ComboBox _cbbOperateMode = null!;
    private System.Windows.Forms.ComboBox _cbbMoveOption = null!;

    private void InitializeComponentCustomMonster()
    {
        var gb = new System.Windows.Forms.GroupBox { Text = "自定义怪物", Left = 8, Top = 530, Width = 660, Height = 170 };
        Controls.Add(gb);
        gb.Controls.Add(new System.Windows.Forms.Label { Text = "怪物列表:", Left = 10, Top = 18, AutoSize = true });
        VstCustomMonster = new System.Windows.Forms.ListView
        {
            Left = 78, Top = 16, Width = 150, Height = 120, View = System.Windows.Forms.View.Details,
            FullRowSelect = true, HideSelection = false, MultiSelect = false,
        };
        VstCustomMonster.Columns.Add("怪物", 140);
        VstCustomMonster.Click += (_, _) => VstCustomMonsterNodeClick();
        gb.Controls.Add(VstCustomMonster);
        VstAction = new System.Windows.Forms.ListView
        {
            Left = 236, Top = 16, Width = 110, Height = 120, View = System.Windows.Forms.View.Details,
            CheckBoxes = true, FullRowSelect = true, HideSelection = false, MultiSelect = false,
        };
        VstAction.Columns.Add("动作", 100);
        VstAction.ItemChecked += (s, e) => VstActionChecked(e.Item.Index, e.Item.Checked);
        gb.Controls.Add(VstAction);
        PnlMonDescCaption = "";
        btnSave = new System.Windows.Forms.Button { Text = "保存", Left = 356, Top = 142, Width = 70 };
        btnSave.Click += (_, _) => BtnSaveClick();
        gb.Controls.Add(btnSave);
        btn1 = new System.Windows.Forms.Button { Text = "生成登录器配置", Left = 430, Top = 142, Width = 110 };
        btn1.Click += (_, _) => Btn1Click();
        gb.Controls.Add(btn1);

        BuildAttackConfigControls(gb);

        // Delphi FormCreate 尾部：禁用主面板直至选中怪物
        PgcMainEnabled = false;
        FCurrentMonsterCustomConfig = null;
        ClientConfigIndex = -1;
        ServerConfigIndex = -1;
    }

    private bool _effectListsFilled;

    /// <summary>FormCreate 文件下拉族填充（'根据Appr计算' + g_EffectImageList；Open 起始一次性执行）。</summary>
    private void EnsureEffectListsFilled()
    {
        if (_effectListsFilled)
            return;
        _effectListsFilled = true;
        var effectFiles = new List<string> { "根据Appr计算" };
        foreach (var name in EffectImageListHandler?.Invoke() ?? new List<string>())
            effectFiles.Add(name);
        foreach (var name in new[]
                 {
                     "cbbClientFlyFile", "cbbClientFlyEffFile", "cbbClientSelfFile", "cbbClientSelfKeepFile",
                     "cbbClientExplosionFile", "cbbClientTargetFile", "cbbHPFile", "cbbBatchAction", "cbbBatchEffect",
                 })
        {
            var cbb = Cbbs[name];
            cbb.Items.Clear();
            foreach (var it in effectFiles)
                cbb.Items.Add(it);
        }
    }

    /// <summary>FormCreate 攻击效果配置两组控件注册（文件下拉首项 '根据Appr计算' + g_EffectImageList）。</summary>
    private void BuildAttackConfigControls(System.Windows.Forms.Control parent)
    {
        // 文件下拉（'根据Appr计算' + g_EffectImageList）在 Open 起始的 EnsureEffectListsFilled 填充
        void RegCbb(string name, IEnumerable<string> items)
        {
            var cbb = new System.Windows.Forms.ComboBox { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Width = 110 };
            foreach (var it in items)
                cbb.Items.Add(it);
            Cbbs[name] = cbb;
        }
        void RegFileCbb(string name) => RegCbb(name, new List<string> { "根据Appr计算" });
        void RegSpin(string name)
        {
            var spin = new System.Windows.Forms.NumericUpDown { Minimum = -2000000000, Maximum = 2000000000, Width = 90 };
            Spins[name] = spin;
        }
        void RegChk(string name)
        {
            Chks[name] = new System.Windows.Forms.CheckBox { Text = name, AutoSize = true };
        }

        RegCbb("cbbClientAttackConfig", CustomMonsterConsts.AttackConfigNames);
        _cbbClientAttackConfig = Cbbs["cbbClientAttackConfig"];
        RegFileCbb("cbbClientFlyFile");
        RegCbb("cbbClientFlyDrawMode", CustomMonsterConsts.CustomDrawModeNames);
        RegCbb("cbbClientFlyDirCount", CustomMonsterConsts.CustomDirNames);
        RegFileCbb("cbbClientFlyEffFile");
        RegCbb("cbbClientFlyEffDrawMode", CustomMonsterConsts.CustomDrawModeNames);
        RegFileCbb("cbbClientSelfFile");
        RegCbb("cbbClientSelfDrawOrder", CustomMonsterConsts.CustomDrawOrderNames);
        RegCbb("cbbClientSelfDrawMode", CustomMonsterConsts.CustomDrawModeNames);
        RegCbb("cbbClientSelfDirCalcType", CustomMonsterConsts.CustomDirCalcTypeNames);
        RegCbb("cbbClientSelfDirCount", CustomMonsterConsts.CustomDirNames);
        RegFileCbb("cbbClientSelfKeepFile");
        RegCbb("cbbClientSelfKeepDrawOrder", CustomMonsterConsts.CustomDrawOrderNames);
        RegCbb("cbbClientSelfKeepDrawMode", CustomMonsterConsts.CustomDrawModeNames);
        RegCbb("cbbClientSelfKeepDrawMode2", CustomMonsterConsts.CustomDrawModeNames);
        RegFileCbb("cbbClientExplosionFile");
        RegCbb("cbbClientExplosionDrawMode", CustomMonsterConsts.CustomDrawModeNames);
        RegCbb("cbbClientExplosionDrawMode2", CustomMonsterConsts.CustomDrawModeNames);
        RegFileCbb("cbbClientTargetFile");
        RegCbb("cbbClientTargetDrawMode", CustomMonsterConsts.CustomDrawModeNames);
        RegCbb("cbbClientTargetDrawMode2", CustomMonsterConsts.CustomDrawModeNames);

        foreach (var n in new[]
                 {
                     "seClientFlyStartIndex", "seClientFlyPlayCount", "seClientFlyEmptyCount", "seClientFlyPlayTime", "seFlyLightRange",
                     "seClientFlyEffStartIndex", "seClientSelfStartIndex", "seClientSelfPlayCount", "seClientSelfEmptyCount", "seClientSelfPlayTime",
                     "seSelfLightRange", "seClientSelfKeepStartIndex", "seClientSelfKeepStartIndex2", "seClientSelfKeepPlayCount", "seClientSelfKeepPlayTime",
                     "seClientSelfKeepTime", "seClientExplosionStartIndex", "seClientExplosionStartIndex2", "seClientExplosionPlayCount", "seClientExplosionPlayTime",
                     "seExplosionLightRange", "seClientExplosionKeepTime", "seClientExplosionKeepAttackInterval", "seClientExplosionKeepAttackRange",
                     "seExplosionKeepLightRange", "seClientTargetStartIndex", "seClientTargetStartIndex2", "seClientTargetPlayCount", "seClientTargetPlayTime",
                     "seTargetLightRange", "seTargetKeepTime", "seTargetKeepAttackRange", "seTargetKeepAttackInterval", "seTargetKeepLightRange",
                 }) RegSpin(n);

        foreach (var n in new[]
                 {
                     "chkClientFlyCalcDir", "chkClientSelfPlayDelayAction", "chkClientExplosionLockDraw", "chkClientExplosionKeepPlay",
                     "chkClientExplosionKeepMultiPlay", "chkClientTargetMultiPlay", "chkClientTargetLockDraw", "chkTargetKeepPlay",
                     "chkTargetKeepMultiPlay",
                 }) RegChk(n);

        // ---- 服务端 ----
        RegCbb("cbbServerAttackConfig", CustomMonsterConsts.AttackConfigNames);
        _cbbServerAttackConfig = Cbbs["cbbServerAttackConfig"];
        RegCbb("cbbMonsterType", CustomMonsterConsts.MonsterTypeNames);
        RegCbb("cbbMoveOption", CustomMonsterConsts.MoveOptionNames);
        _cbbMoveOption = Cbbs["cbbMoveOption"];
        RegCbb("cbbOperateMode", CustomMonsterConsts.CustomOperateModeNames);
        _cbbOperateMode = Cbbs["cbbOperateMode"];
        RegCbb("cbbAttackMode", CustomMonsterConsts.CustomAttackModeNames);
        RegCbb("cbbAttackTarget", CustomMonsterConsts.CustomAttackTargetNames);
        RegCbb("cbbAttackPowerCalc", CustomMonsterConsts.CustomAttackPowerCalcNames);
        foreach (var n in new[]
                 {
                     "seViewRange", "seProtectRange", "seMinAttackNearRange", "seLightRange",
                     "seAttackDelayTime", "seAttackHPPercent", "seAttackRate", "seAttackTargetCount", "seAttackPowerRate",
                     "seAttackTeleportTargetDistance", "seAttackTeleportDistance", "seAttackTeleportRate",
                     "seAttackNearRange", "seAttackGroupRange", "seAttackPowerInc",
                     "seAdditionaHP0", "seAdditionalImprisonRange", "seCallMonstersRate", "seMoveTargetRate",
                     "seProtectAddHPRate", "seProtectAddHPPercent",
                     "seProtectAddDefenceRate", "seProtectAddDefencePercent", "seProtectAddDefenceTime",
                     "seProtectAddMagDefenceRate", "seProtectAddMagDefencePercent", "seProtectAddMagDefenceTime",
                     "seProtectAddDCRate", "seProtectAddDCPercent", "seProtectAddDCTime",
                     "seProtectAddMCRate", "seProtectAddMCPercent", "seProtectAddMCTime",
                     "seProtectAddSCRate", "seProtectAddSCPercent", "seProtectAddSCTime",
                     "seProtectTargetRange", "seProtectSelfRate",
                 }) RegSpin(n);
        for (int i = 0; i < 12; i++)
        {
            RegSpin($"seAdditionalRate{i}");
            RegSpin($"seAdditionalTime{i}");
            RegChk($"chkAdditional{i}");
        }
        for (int i = 1; i <= 4; i++)
        {
            RegSpin($"seCallMonsterNum{i}");
            Edits[$"edtCallMonster{i}"] = new System.Windows.Forms.TextBox { Width = 90 };
        }
        foreach (var n in new[]
                 {
                     "chkAttackEnabled", "chkAttackSelfDie", "chkAttackTeleportAttack", "chkAttackTeleportRush", "chkAttackIgnoreDefence",
                     "chkNearAttackTargetCenter", "chkseAdditionaHighLevel4", "chkEnabledCallMonster", "chkMoveTarget", "chkMoveTargetHighLevel",
                     "chkProtectAddHP", "chkProtectAddDefence", "chkProtectAddMagDefence", "chkProtectAddDC", "chkProtectAddMC", "chkProtectAddSC",
                     "chkNoAttack",
                 }) RegChk(n);

        // ---- 客户端基础（声音/血条） ----
        foreach (var n in new[]
                 {
                     "edtSoundNormal", "edtSoundDigUP", "edtSoundAttack", "edtSoundStruck", "edtSoundDie",
                     "edtSoundAttack1", "edtSoundAttack2", "edtSoundAttack3", "edtSoundAttack4", "edtSoundAttack5", "edtSoundAttack6",
                 }) Edits[n] = new System.Windows.Forms.TextBox { Width = 90 };
        RegCbb("cbbClientDrawMode", CustomMonsterConsts.CustomDrawModeNames);
        RegCbb("cbbClientDrawMode2", CustomMonsterConsts.CustomDrawModeNames);
        RegCbb("cbbClientDrawOrder2", CustomMonsterConsts.MonsterDrawOrder2Names);
        RegFileCbb("cbbHPFile");
        foreach (var n in new[]
                 {
                     "seHPBgOffsetX", "seHPBgOffsetY", "seHPOffsetX", "seHPOffsetY", "seHPStartIndex", "seHPTextOffsetX", "seHPTextOffsetY",
                 }) RegSpin(n);
        RegChk("chkDieNoCalcDir");

        foreach (var c in Cbbs.Values) parent.Controls.Add(c);
        foreach (var c in Spins.Values) parent.Controls.Add(c);
        foreach (var c in Chks.Values) parent.Controls.Add(c);
        foreach (var c in Edits.Values) parent.Controls.Add(c);
        foreach (var c in Chks.Values) c.Visible = false;
        foreach (var c in Spins.Values) c.Visible = false;
        foreach (var c in Cbbs.Values) c.Visible = false;
        foreach (var c in Edits.Values) c.Visible = false;
    }

    // ---- 按名取值/赋值助手（Delphi 控件属性读写等效） ----

    public System.Windows.Forms.ComboBox Cbb(string name) => Cbbs[name];
    public System.Windows.Forms.NumericUpDown Spin(string name) => Spins[name];
    public System.Windows.Forms.CheckBox Chk(string name) => Chks[name];

    private void SetCbb(string name, int index)
    {
        var cbb = Cbbs[name];
        if (cbb.Items.Count > 0)
            cbb.SelectedIndex = Math.Clamp(index, 0, cbb.Items.Count - 1);
    }

    private void SetSpin(string name, int value)
    {
        var spin = Spins[name];
        spin.Value = Math.Clamp(value, spin.Minimum, spin.Maximum);
    }

    private void SetChk(string name, bool value)
    {
        // Delphi VCL OnClick 语义：程序赋值 Checked 不触发 *Click 处理器 → 压制级联
        _suppressCheckCascade = true;
        try
        {
            Chks[name].Checked = value;
        }
        finally
        {
            _suppressCheckCascade = false;
        }
    }
    private void SetEdit(string name, string value) => Edits[name].Text = value;

    /// <summary>SetMonsterConfigChanged 1:1（置改标志 + 树节点红名（呈现层省略）+ FIsMonsterChanged/保存钮）。</summary>
    public void SetMonsterConfigChanged(bool isChanged = true)
    {
        if (FCurrentMonsterCustomConfig != null)
        {
            FCurrentMonsterCustomConfig.SetChanged(isChanged);
            FIsMonsterChanged = true;
            if (!btnSave.Enabled)
                btnSave.Enabled = true;
        }
    }

    /// <summary>vstCustomMonsterNodeClick 1:1（清动作树 → 取焦点节点 → 建 12 动作勾选节点 → 双组回填 → Race 描述/页签）。</summary>
    public void VstCustomMonsterNodeClick()
    {
        VstAction.Items.Clear();
        FCurrentMonsterCustomConfig = null;
        ClientConfigIndex = -1;
        ServerConfigIndex = -1;
        var focused = GetSelected(VstCustomMonster);
        if (focused == null)
            return;
        PgcMainEnabled = true;
        var configNodeData = (TCustomMonsterConfig?)focused.Tag;
        if (configNodeData == null)
            return;
        bool oldIsConfigCanSave = FIsMonsterChanged;
        FCurrentMonsterCustomConfig = configNodeData;
        bool oldChanged = FCurrentMonsterCustomConfig.IsChanged;
        for (int i = 0; i < 12; i++)
        {
            var row = new System.Windows.Forms.ListViewItem(CustomMonsterConsts.MonsterClientActionNames[i]) { Tag = i };
            row.Checked = FCurrentMonsterCustomConfig.ClientActions[i].CalcDir != 0;
            VstAction.Items.Add(row);
        }
        SetCbb("cbbClientAttackConfig", 0);
        CbbClientAttackConfigChange();
        SetCbb("cbbClientDrawMode", (int)FCurrentMonsterCustomConfig.ClientBaseConfig.DrawMode);
        SetCbb("cbbClientDrawMode2", (int)FCurrentMonsterCustomConfig.ClientBaseConfig.DrawMode2);
        SetCbb("cbbClientDrawOrder2", (int)FCurrentMonsterCustomConfig.ClientBaseConfig.DrawOrder);
        SetChk("chkDieNoCalcDir", FCurrentMonsterCustomConfig.ClientBaseConfig.DieNoCalcDir != 0);
        SetEdit("edtSoundNormal", FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[0].Value);
        SetEdit("edtSoundDigUP", FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[1].Value);
        SetEdit("edtSoundAttack", FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[2].Value);
        SetEdit("edtSoundStruck", FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[3].Value);
        SetEdit("edtSoundDie", FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[4].Value);
        SetEdit("edtSoundAttack1", FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[5].Value);
        SetEdit("edtSoundAttack2", FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[6].Value);
        SetEdit("edtSoundAttack3", FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[7].Value);
        SetEdit("edtSoundAttack4", FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[8].Value);
        SetEdit("edtSoundAttack5", FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[9].Value);
        SetEdit("edtSoundAttack6", FCurrentMonsterCustomConfig.ClientBaseConfig.Sounds[10].Value);
        SetSpin("seHPBgOffsetX", FCurrentMonsterCustomConfig.ClientBaseConfig.HPBgOffsetX);
        SetSpin("seHPBgOffsetY", FCurrentMonsterCustomConfig.ClientBaseConfig.HPBgOffsetY);
        SetSpin("seHPOffsetX", FCurrentMonsterCustomConfig.ClientBaseConfig.HPOffsetX);
        SetSpin("seHPOffsetY", FCurrentMonsterCustomConfig.ClientBaseConfig.HPOffsetY);
        SetCbb("cbbHPFile", FCurrentMonsterCustomConfig.ClientBaseConfig.HPFile + 1);
        SetSpin("seHPStartIndex", FCurrentMonsterCustomConfig.ClientBaseConfig.HPStartIndex);
        SetSpin("seHPTextOffsetX", FCurrentMonsterCustomConfig.ClientBaseConfig.HPTextOffsetX);
        SetSpin("seHPTextOffsetY", FCurrentMonsterCustomConfig.ClientBaseConfig.HPTextOffsetY);
        SetCbb("cbbServerAttackConfig", 0);
        CbbServerAttackConfigChange();
        SetSpin("seViewRange", FCurrentMonsterCustomConfig.ServerBaseConfig.ViewRange);
        SetCbb("cbbMonsterType", (int)FCurrentMonsterCustomConfig.ServerBaseConfig.MonsterType);
        SetCbb("cbbMoveOption", (int)FCurrentMonsterCustomConfig.ServerBaseConfig.MoveOption);
        SetSpin("seProtectRange", FCurrentMonsterCustomConfig.ServerBaseConfig.ProtectRange);
        SetSpin("seMinAttackNearRange", FCurrentMonsterCustomConfig.ServerBaseConfig.MinAttackNearRange);
        SetSpin("seLightRange", FCurrentMonsterCustomConfig.ServerBaseConfig.LightRange);
        SetChk("chkNoAttack", FCurrentMonsterCustomConfig.ServerBaseConfig.NoAttack);
        CbbMoveOptionChange();
        SetMonsterConfigChanged(oldChanged);
        PnlMonDescCaption = "  Race = " + FCurrentMonsterCustomConfig.MonsterRace + "; ";
        switch (FCurrentMonsterCustomConfig.MonsterRace)
        {
            case 154:
                PnlMonDescCaption += "魔王岭怪物; 不攻击目标";
                break;
            case 155:
                PnlMonDescCaption += "魔王岭、雕像类宝宝; 不受地图MISSION参数的限制，不回血，换地图后自动消失";
                break;
            case 156:
                PnlMonDescCaption += "普通怪物; 主动攻击目标";
                break;
            case 157:
                PnlMonDescCaption += "普通怪物; 不会主动攻击目标，如鸡羊鹿";
                break;
            case 159:
                PnlMonDescCaption += "采集类怪物；不攻击目标";
                break;
        }
        TsAttackVisible = FCurrentMonsterCustomConfig.MonsterRace is not (154 or 159);
        TsServerAttackVisible = FCurrentMonsterCustomConfig.MonsterRace is not (154 or 159);
        FIsMonsterChanged = oldIsConfigCanSave;
        if (!FIsMonsterChanged)
            btnSave.Enabled = false;
    }

    /// <summary>cbbClientAttackConfigChange 1:1（取当前攻击组 → 55+ 控件回填 → 状态位还原）。</summary>
    public void CbbClientAttackConfigChange()
    {
        ClientConfigIndex = -1;
        if (FCurrentMonsterCustomConfig == null)
            return;
        bool oldIsConfigCanSave = FIsMonsterChanged;
        int index = Cbbs["cbbClientAttackConfig"].SelectedIndex;
        if (index is >= 0 and <= 5)
        {
            bool oldChanged = FCurrentMonsterCustomConfig.IsChanged;
            ClientConfigIndex = index;
            ref var c = ref FCurrentMonsterCustomConfig.ClientAttackConfigs[index];
            GrpClientAttackConfigsCaption = CustomMonsterConsts.AttackConfigNames[index] + "的攻击效果配置";
            SetCbb("cbbClientFlyFile", c.Fly_File + 1);
            SetSpin("seClientFlyStartIndex", c.Fly_StartIndex);
            SetSpin("seClientFlyPlayCount", c.Fly_PlayCount);
            SetSpin("seClientFlyEmptyCount", c.Fly_EmptyCount);
            SetSpin("seClientFlyPlayTime", c.Fly_PlayTime);
            SetCbb("cbbClientFlyDrawMode", (int)c.Fly_DrawMode);
            SetCbb("cbbClientFlyDirCount", (int)c.Fly_DirCount);
            SetChk("chkClientFlyCalcDir", c.Fly_CalcDir != 0);
            SetSpin("seFlyLightRange", c.Fly_LightRange);
            SetCbb("cbbClientFlyEffFile", c.FlyEff_File + 1);
            SetSpin("seClientFlyEffStartIndex", c.FlyEff_StartIndex);
            SetCbb("cbbClientFlyEffDrawMode", (int)c.FlyEff_DrawMode);
            SetCbb("cbbClientSelfFile", c.Self_File + 1);
            SetSpin("seClientSelfStartIndex", c.Self_StartIndex);
            SetSpin("seClientSelfPlayCount", c.Self_PlayCount);
            SetSpin("seClientSelfEmptyCount", c.Self_EmptyCount);
            SetSpin("seClientSelfPlayTime", c.Self_PlayTime);
            SetCbb("cbbClientSelfDrawOrder", (int)c.Self_DrawOrder);
            SetCbb("cbbClientSelfDrawMode", (int)c.Self_DrawMode);
            SetCbb("cbbClientSelfDirCalcType", (int)c.Self_DirCalcType);
            SetCbb("cbbClientSelfDirCount", (int)c.Self_DirCount);
            SetChk("chkClientSelfPlayDelayAction", c.Self_PlayDelayAction != 0);
            SetSpin("seSelfLightRange", c.Self_LightRange);
            SetCbb("cbbClientSelfKeepFile", c.SelfKeep_File + 1);
            SetSpin("seClientSelfKeepStartIndex", c.SelfKeep_StartIndex);
            SetSpin("seClientSelfKeepStartIndex2", c.SelfKeep_StartIndex2);
            SetSpin("seClientSelfKeepPlayCount", c.SelfKeep_PlayCount);
            SetSpin("seClientSelfKeepPlayTime", c.SelfKeep_PlayTime);
            SetCbb("cbbClientSelfKeepDrawOrder", (int)c.SelfKeep_DrawOrder);
            SetCbb("cbbClientSelfKeepDrawMode", (int)c.SelfKeep_DrawMode);
            SetCbb("cbbClientSelfKeepDrawMode2", (int)c.SelfKeep_DrawMode2);
            SetSpin("seClientSelfKeepTime", c.SelfKeep_KeepTime);
            SetCbb("cbbClientExplosionFile", c.Explosion_File + 1);
            SetSpin("seClientExplosionStartIndex", c.Explosion_StartIndex);
            SetSpin("seClientExplosionStartIndex2", c.Explosion_StartIndex2);
            SetSpin("seClientExplosionPlayCount", c.Explosion_PlayCount);
            SetSpin("seClientExplosionPlayTime", c.Explosion_PlayTime);
            SetCbb("cbbClientExplosionDrawMode", (int)c.Explosion_DrawMode);
            SetCbb("cbbClientExplosionDrawMode2", (int)c.Explosion_DrawMode2);
            SetChk("chkClientExplosionLockDraw", c.Explosion_LockDraw != 0);
            SetSpin("seExplosionLightRange", c.Explosion_LightRange);
            SetChk("chkClientExplosionKeepPlay", c.Explosion_KeepPlay != 0);
            SetSpin("seClientExplosionKeepTime", c.Explosion_KeepTime);
            SetSpin("seClientExplosionKeepAttackInterval", c.Explosion_KeepAttackInterval);
            SetSpin("seClientExplosionKeepAttackRange", c.Explosion_KeepAttackRange);
            SetSpin("seExplosionKeepLightRange", c.Explosion_KeepLightRange);
            SetChk("chkClientExplosionKeepMultiPlay", c.Explosion_KeepMultiPlay != 0);
            SetCbb("cbbClientTargetFile", c.Target_File + 1);
            SetSpin("seClientTargetStartIndex", c.Target_StartIndex);
            SetSpin("seClientTargetStartIndex2", c.Target_StartIndex2);
            SetSpin("seClientTargetPlayCount", c.Target_PlayCount);
            SetSpin("seClientTargetPlayTime", c.Target_PlayTime);
            SetCbb("cbbClientTargetDrawMode", (int)c.Target_DrawMode);
            SetCbb("cbbClientTargetDrawMode2", (int)c.Target_DrawMode2);
            SetChk("chkClientTargetMultiPlay", c.Target_MultiPlay != 0);
            SetChk("chkClientTargetLockDraw", c.Target_LockDraw != 0);
            SetSpin("seTargetLightRange", c.Target_LightRange);
            SetChk("chkTargetKeepPlay", c.Target_KeepPlay != 0);
            SetSpin("seTargetKeepTime", c.Target_KeepTime);
            SetSpin("seTargetKeepAttackRange", c.Target_KeepAttackRange);
            SetChk("chkTargetKeepMultiPlay", c.Target_KeepMultiPlay != 0);
            SetSpin("seTargetKeepAttackInterval", c.Target_KeepAttackInterval);
            SetSpin("seTargetKeepLightRange", c.Target_KeepLightRange);
            SetMonsterConfigChanged(oldChanged);
            FIsMonsterChanged = oldIsConfigCanSave;
            if (!FIsMonsterChanged)
                btnSave.Enabled = false;
        }
    }

    /// <summary>grpClientAttackConfigs.Caption 等效。</summary>
    public string GrpClientAttackConfigsCaption = "";
    public string GrpServerAttackConfigsCaption = "";

    /// <summary>cbbServerAttackConfigChange 1:1（取当前服务端攻击组 → 攻击/附加状态/召唤/保护面板回填）。</summary>
    public void CbbServerAttackConfigChange()
    {
        ServerConfigIndex = -1;
        if (FCurrentMonsterCustomConfig == null)
            return;
        bool oldIsConfigCanSave = FIsMonsterChanged;
        int index = Cbbs["cbbServerAttackConfig"].SelectedIndex;
        if (index is >= 0 and <= 5)
        {
            bool oldChanged = FCurrentMonsterCustomConfig.IsChanged;
            ServerConfigIndex = index;
            var c = FCurrentMonsterCustomConfig.MonsterServerConfigs[index];
            GrpServerAttackConfigsCaption = CustomMonsterConsts.AttackConfigNames[index] + "的攻击效果配置";
            SetChk("chkAttackEnabled", c.AttackEnabled);
            SetCbb("cbbOperateMode", (int)c.OperateMode);
            CbbOperateModeChange();
            SetChk("chkAttackSelfDie", c.AttackSelfDie);
            SetSpin("seAttackDelayTime", c.AttackDelayTime);
            SetSpin("seAttackHPPercent", c.AttackHPPercent);
            SetSpin("seAttackRate", c.AttackRate);
            SetSpin("seAttackTargetCount", c.AttackTargetCount);
            SetCbb("cbbAttackMode", (int)c.AttackMode);
            SetCbb("cbbAttackTarget", (int)c.AttackTarget);
            SetCbb("cbbAttackPowerCalc", (int)c.AttackPowerCalc);
            SetSpin("seAttackPowerRate", c.AttackPowerRate);
            SetChk("chkAttackTeleportAttack", c.AttackTeleportAttack);
            SetChk("chkAttackTeleportRush", c.AttackTeleportRush);
            SetSpin("seAttackTeleportTargetDistance", c.AttackTeleportTargetDistance);
            SetSpin("seAttackTeleportDistance", c.AttackTeleportDistance);
            SetSpin("seAttackTeleportRate", c.AttackTeleportRate);
            SetChk("chkAttackIgnoreDefence", c.AttackIgnoreDefence);
            SetSpin("seAttackNearRange", c.AttackNearRange);
            SetSpin("seAttackGroupRange", c.AttackGroupRange);
            SetChk("chkNearAttackTargetCenter", c.NearAttackTargetCenter);
            SetSpin("seAttackPowerInc", c.AttackPowerInc);
            for (int i = 0; i < 12; i++)
            {
                SetChk($"chkAdditional{i}", c.Additionals[i].Checked);
                SetSpin($"seAdditionalRate{i}", c.Additionals[i].Rate);
                SetSpin($"seAdditionalTime{i}", c.Additionals[i].Time);
            }
            SetSpin("seAdditionaHP0", c.AdditionalHP0);
            SetChk("chkseAdditionaHighLevel4", c.AdditionalHighLevel4);
            SetSpin("seAdditionalImprisonRange", c.AdditionalImprisonRange);
            SetChk("chkEnabledCallMonster", c.EnabledCallMonster);
            SetSpin("seCallMonstersRate", c.CallMonstersRate);
            for (int i = 1; i <= 4; i++)
            {
                SetEdit($"edtCallMonster{i}", c.CallMonsters[i - 1]);
                SetSpin($"seCallMonsterNum{i}", c.CallMonsterNums[i - 1]);
            }
            SetChk("chkMoveTarget", c.MoveTarget);
            SetSpin("seMoveTargetRate", c.MoveTargetRate);
            SetChk("chkMoveTargetHighLevel", c.MoveTargetHighLevel);
            SetChk("chkProtectAddHP", c.ProtectAddHP);
            SetSpin("seProtectAddHPRate", c.ProtectAddHPRate);
            SetSpin("seProtectAddHPPercent", c.ProtectAddHPPercent);
            SetChk("chkProtectAddDefence", c.ProtectAddDefence);
            SetSpin("seProtectAddDefenceRate", c.ProtectAddDefenceRate);
            SetSpin("seProtectAddDefencePercent", c.ProtectAddDefencePercent);
            SetSpin("seProtectAddDefenceTime", c.ProtectAddDefenceTime);
            SetChk("chkProtectAddMagDefence", c.ProtectAddMagDefence);
            SetSpin("seProtectAddMagDefenceRate", c.ProtectAddMagDefenceRate);
            SetSpin("seProtectAddMagDefencePercent", c.ProtectAddMagDefencePercent);
            SetSpin("seProtectAddMagDefenceTime", c.ProtectAddMagDefenceTime);
            SetChk("chkProtectAddDC", c.ProtectAddDC);
            SetSpin("seProtectAddDCRate", c.ProtectAddDCRate);
            SetSpin("seProtectAddDCPercent", c.ProtectAddDCPercent);
            SetSpin("seProtectAddDCTime", c.ProtectAddDCTime);
            SetChk("chkProtectAddMC", c.ProtectAddMC);
            SetSpin("seProtectAddMCRate", c.ProtectAddMCRate);
            SetSpin("seProtectAddMCPercent", c.ProtectAddMCPercent);
            SetSpin("seProtectAddMCTime", c.ProtectAddMCTime);
            SetChk("chkProtectAddSC", c.ProtectAddSC);
            SetSpin("seProtectAddSCRate", c.ProtectAddSCRate);
            SetSpin("seProtectAddSCPercent", c.ProtectAddSCPercent);
            SetSpin("seProtectAddSCTime", c.ProtectAddSCTime);
            SetSpin("seProtectTargetRange", c.ProtectTargetRange);
            SetSpin("seProtectSelfRate", c.ProtectSelfRate);
            SetMonsterConfigChanged(oldChanged);
            FIsMonsterChanged = oldIsConfigCanSave;
            if (!FIsMonsterChanged)
                btnSave.Enabled = false;
        }
    }

    /// <summary>cbbOperateModeChange 1:1（写回 OperateMode + 攻击/保护面板可见性切换）。</summary>
    public void CbbOperateModeChange()
    {
        if (FCurrentMonsterCustomConfig == null || ServerConfigIndex < 0)
            return;
        var c = FCurrentMonsterCustomConfig.MonsterServerConfigs[ServerConfigIndex];
        c.OperateMode = (TCustomOperateMode)Cbbs["cbbOperateMode"].SelectedIndex;
        SetMonsterConfigChanged();
        SeAttackTargetCountEnabled = c.OperateMode == TCustomOperateMode.momAttack;
        GrpOptionsVisible = SeAttackTargetCountEnabled;
        GrpProtectVisible = !GrpOptionsVisible;
    }

    /// <summary>cbbMoveOptionChange 1:1（写回 MoveOption + 守护区控件可见性）。</summary>
    public void CbbMoveOptionChange()
    {
        if (FCurrentMonsterCustomConfig == null)
            return;
        FCurrentMonsterCustomConfig.ServerBaseConfig.MoveOption = (TMoveOption)Cbbs["cbbMoveOption"].SelectedIndex;
        SetMonsterConfigChanged();
        LblProtectVisible = Cbbs["cbbMoveOption"].SelectedIndex == (int)TMoveOption.moProtect;
    }

    /// <summary>vstActionChecked 1:1（CalcDir 写回 + SetMonsterConfigChanged）。</summary>
    public void VstActionChecked(int actionIndex, bool checkState)
    {
        if (FCurrentMonsterCustomConfig == null)
            return;
        ref var action = ref FCurrentMonsterCustomConfig.ClientActions[actionIndex];
        action.CalcDir = (byte)(checkState ? 1 : 0);
        SetMonsterConfigChanged();
    }

    /// <summary>vstCustomMonsterKeyDown Ctrl+F 1:1（InputQuery 关键字 SameText 查找 → 选中并触发 NodeClick）。</summary>
    public void VstCustomMonsterCtrlF()
    {
        if (InputQueryHandler == null || !InputQueryHandler("关键字查找", "输入关键字:"))
            return;
        string keyword = LastInputQueryText;
        if (keyword.Length == 0)
            return;
        foreach (System.Windows.Forms.ListViewItem node in VstCustomMonster.Items)
        {
            var item = (TCustomMonsterConfig?)node.Tag;
            if (item != null && string.Equals(keyword, item.MonsterName, StringComparison.OrdinalIgnoreCase))
            {
                node.Selected = true;
                VstCustomMonster.FocusedItem = node;
                VstCustomMonsterNodeClick();
                return;
            }
        }
    }

    /// <summary>btnSaveClick 1:1（逐怪 IsChanged → SaveToIniFile + 重建树文本 + 复位保存钮）。</summary>
    public void BtnSaveClick()
    {
        foreach (System.Windows.Forms.ListViewItem node in VstCustomMonster.Items)
        {
            var config = (TCustomMonsterConfig?)node.Tag;
            if (config != null && config.IsChanged)
                config.SaveToIniFile();
        }
        RebuildCustomMonsterListText();
        FIsMonsterChanged = false;
        btnSave.Enabled = false;
    }

    /// <summary>RebuildCustomMonsterListText 1:1（树文本重建；WinForms 下文本不变，保留接缝）。</summary>
    private void RebuildCustomMonsterListText()
    {
    }

    /// <summary>btn1Click 1:1（生成登录器 .dat：配置文件名写 Config 'Setup' 节 + SaveCustomMonsterClientConfigs）。</summary>
    public void Btn1Click()
    {
        string fileName = M2Config.sCustomMonsterClientConfigFileName;
        if (fileName != "")
        {
            // Delphi：dlgSaveMonsters.FileName := 已存名（对话框初值），此处直接交由接缝
        }
        fileName = SaveMonsterDialogHandler?.Invoke(fileName == "" ? null : fileName);
        if (fileName == null)
        {
            LastShowMessage = null;
            return;
        }
        fileName = System.IO.Path.ChangeExtension(fileName, ".dat");
        M2Config.sCustomMonsterClientConfigFileName = fileName;
        M2ShareState.ConfigIni.WriteString("Setup", "CustomMonsterClientConfigFileName", M2Config.sCustomMonsterClientConfigFileName);
        var list = CustomMonsterConfigsHandler?.Invoke() ?? new List<TCustomMonsterConfig>();
        CustomMonsterClientWriter.Save(list, fileName);
        LastShowMessage = "已经生成自定义怪物登录器配置文件";
    }
}

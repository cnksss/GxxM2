using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// MonsterConfig.pas TfrmMonsterConfig 巨片第一片（5256 行）：
/// Open 骨架（魔法列表/人形怪列表/自定义怪列表三个接缝 + 时序/常规回显）、
/// RefGeneralInfo 常规信息组 16 控件回显与处理器、怪物攻/走六时间处理器、
/// NeedMagicItem 单选与自定义怪物下发/伤害封顶开关、ButtonGeneralSaveClick 16 键
/// （尾部无条件下发）、ButtonMonsterConfigSaveClick 8 键。
/// 人形怪装备双列表拖拽/怪物技能双击/掉落限制树等随巨片余部接入。
/// </summary>
public sealed partial class MonsterConfigForm : System.Windows.Forms.Form
{
    private bool boOpened;
    private bool boModValued;

    // ---- 列表 ----
    public System.Windows.Forms.ListBox ListBoxMagicList = null!;
    public System.Windows.Forms.ListBox ListBoxMonsterList = null!;
    public System.Windows.Forms.ListBox ListBoxCustomMonster = null!;

    // ---- 常规信息组控件 ----
    public System.Windows.Forms.NumericUpDown seMonButchDelayClearTime = null!;
    public System.Windows.Forms.CheckBox chkNoHumanClearMon = null!;
    public System.Windows.Forms.NumericUpDown seNoHumanClearMonTime = null!;
    public System.Windows.Forms.ComboBox cbbMonsterShowLevel = null!;
    public System.Windows.Forms.TextBox edtMonsterShowFormat = null!;
    public System.Windows.Forms.NumericUpDown edtMagStruckMonLevel = null!;
    public System.Windows.Forms.NumericUpDown edtMagStruckMonDecTime = null!;
    public System.Windows.Forms.NumericUpDown edtMagStruckMonDecRandom = null!;
    public System.Windows.Forms.NumericUpDown seMonStruckFrameDelayTime = null!;
    public System.Windows.Forms.CheckBox chkEnabledMaxMapItemCount = null!;
    public System.Windows.Forms.NumericUpDown seMaxMapItemCount = null!;
    public System.Windows.Forms.CheckBox chkNotDropOverlapItemAll = null!;
    public System.Windows.Forms.NumericUpDown seScatterItemRange = null!;
    public System.Windows.Forms.NumericUpDown seMonOneDropGoldCount = null!;
    public System.Windows.Forms.CheckBox CheckBoxDropGoldToPlayBag = null!;
    public System.Windows.Forms.NumericUpDown seElfWarriorMonsterDownDelay = null!;
    public System.Windows.Forms.Button ButtonGeneralSave = null!;

    // ---- 怪物时序组控件 ----
    public System.Windows.Forms.NumericUpDown EditMonsterWarrorAttackTime = null!;
    public System.Windows.Forms.NumericUpDown EditMonsterWizardAttackTime = null!;
    public System.Windows.Forms.NumericUpDown EditMonsterTaoistAttackTime = null!;
    public System.Windows.Forms.NumericUpDown EditMonsterWarrorWalkTime = null!;
    public System.Windows.Forms.NumericUpDown EditMonsterWizardWalkTime = null!;
    public System.Windows.Forms.NumericUpDown EditMonsterTaoistWalkTime = null!;
    public System.Windows.Forms.ComboBox RadioGroupMonsterNeedMagicItem = null!;
    public System.Windows.Forms.CheckBox chkSendCustomMonsterConfig = null!;
    public System.Windows.Forms.CheckBox CheckBoxDamageLimitation = null!;
    public System.Windows.Forms.Button ButtonMonsterConfigSave = null!;

    // ---- 人形怪装备/魔法/自定义怪组控件（批次J37） ----
    public System.Windows.Forms.ListBox ListBoxMonsterMagicList = null!;
    public System.Windows.Forms.GroupBox GroupBoxMonsterConfig = null!;
    public System.Windows.Forms.ComboBox cbbJob = null!;
    public System.Windows.Forms.ComboBox cbbGender = null!;
    public System.Windows.Forms.NumericUpDown seEditHair = null!;
    public System.Windows.Forms.CheckBox chkDieDropUseItem = null!;
    public System.Windows.Forms.NumericUpDown seDieDropUseItemRate = null!;
    public System.Windows.Forms.CheckBox chkDieDropBagItem = null!;
    public System.Windows.Forms.CheckBox chkButchUseItem = null!;
    public System.Windows.Forms.NumericUpDown seButchUseItemRate = null!;
    public System.Windows.Forms.CheckBox chkButchListItem = null!;
    public System.Windows.Forms.CheckBox chkButchItemTrigger = null!;
    public System.Windows.Forms.ComboBox cbbButchChargeMode = null!;
    public System.Windows.Forms.NumericUpDown seButchChargeCount = null!;
    public System.Windows.Forms.CheckBox chkOnlyButchItemDelGold = null!;
    public System.Windows.Forms.CheckBox chkProtectMode = null!;
    public System.Windows.Forms.NumericUpDown EditRestrictMonsterRange = null!;
    public System.Windows.Forms.CheckBox CheckBoxNonUseSpellPoint = null!;
    public System.Windows.Forms.CheckBox chkRunWithAcctack = null!;
    public System.Windows.Forms.NumericUpDown seRunWithAcctackRate = null!;
    public System.Windows.Forms.CheckBox chkNoAttackMode = null!;
    public System.Windows.Forms.Button ButtonMonUseItemsChange = null!;
    public System.Windows.Forms.Button ButtonMonUseItemsSave = null!;
    public System.Collections.Generic.Dictionary<int, System.Windows.Forms.TextBox> EquipEdits = new();
    public TPlayMonsterConfig? SelMonsterConfig;

    /// <summary>InputQuery 接缝（Delphi Ctrl+F 物品/魔法/人形怪查找；测试注入 false 取消）。</summary>
    public Func<string, string, bool>? InputQueryHandler;

    public const int U_DRESS = 0;
    public const int U_WEAPON = 1;
    public const int U_RIGHTHAND = 2;
    public const int U_NECKLACE = 3;
    public const int U_HELMET = 4;
    public const int U_ARMRINGL = 5;
    public const int U_ARMRINGR = 6;
    public const int U_RINGL = 7;
    public const int U_RINGR = 8;
    public const int U_BUJUK = 9;
    public const int U_BELT = 10;
    public const int U_BOOTS = 11;
    public const int U_CHARM = 12;
    public const int U_SHIELD = 16;

    /// <summary>Delphi GetTakeOnPosition 1:1（StdMode → 装备槽位）。</summary>
    public static int GetTakeOnPosition(int stdMode)
    {
        var U_WEAPON = 1;
        var U_DRESS = 0;
        var U_HELMET = 4;
        var U_NECKLACE = 3;
        var U_RINGL = 7;
        var U_ARMRINGR = 6;
        var U_RIGHTHAND = 2;
        var U_BUJUK = 9;
        var U_BOOTS = 11;
        var U_CHARM = 12;
        var U_BELT = 10;
        var U_SHIELD = 16;
        return stdMode switch
        {
            5 or 6 => U_WEAPON,
            10 or 11 => U_DRESS,
            15 => U_HELMET,
            19 or 20 or 21 => U_NECKLACE,
            22 or 23 => U_RINGL,
            24 or 26 => U_ARMRINGR,
            30 => U_RIGHTHAND,
            25 or 51 => U_BUJUK,
            52 or 62 => U_BOOTS,
            7 or 53 or 63 or 94 => U_CHARM,
            54 or 64 => U_BELT,
            12 => U_SHIELD,
            _ => -1,
        };
    }

    /// <summary>UserEngine.m_MagicList 接缝（法术名列表；引擎批次接入真实数据）。</summary>
    public Func<List<string>>? MagicListHandler;
    /// <summary>UserEngine.MonsterList 人形怪名列表接缝（RC_PLAYMOSTER 过滤）。</summary>
    public Func<List<string>>? MonsterListHandler;
    /// <summary>UserEngine.m_CustomMonsterList 接缝。</summary>
    public Func<List<string>>? CustomMonsterListHandler;

    public MonsterConfigForm()
    {
        InitializeComponent();
        InitializeComponentDropLimit();
        InitializeComponentCustomMonster();
        InitializeComponentCustomMonsterEdits();
    }

    private System.Windows.Forms.NumericUpDown MakeSpin(string caption, int top, int left, System.Windows.Forms.Control parent)
    {
        parent.Controls.Add(new System.Windows.Forms.Label { Text = caption, Left = left, Top = top + 4, AutoSize = true });
        var edit = new System.Windows.Forms.NumericUpDown { Left = left + 160, Top = top, Width = 90, Maximum = 2000000000 };
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
        Text = "怪物设置";
        Width = 720;
        Height = 560;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var gbLists = new System.Windows.Forms.GroupBox { Text = "数据列表", Left = 8, Top = 4, Width = 660, Height = 120 };
        Controls.Add(gbLists);
        gbLists.Controls.Add(new System.Windows.Forms.Label { Text = "魔法列表:", Left = 10, Top = 20, AutoSize = true });
        ListBoxMagicList = new System.Windows.Forms.ListBox { Left = 90, Top = 16, Width = 190, Height = 90 };
        gbLists.Controls.Add(ListBoxMagicList);
        gbLists.Controls.Add(new System.Windows.Forms.Label { Text = "人形怪列表:", Left = 292, Top = 20, AutoSize = true });
        ListBoxMonsterList = new System.Windows.Forms.ListBox { Left = 380, Top = 16, Width = 130, Height = 90 };
        gbLists.Controls.Add(ListBoxMonsterList);
        gbLists.Controls.Add(new System.Windows.Forms.Label { Text = "自定义怪:", Left = 520, Top = 20, AutoSize = true });
        ListBoxCustomMonster = new System.Windows.Forms.ListBox { Left = 520, Top = 40, Width = 120, Height = 66 };
        gbLists.Controls.Add(ListBoxCustomMonster);

        var gbGen = new System.Windows.Forms.GroupBox { Text = "常规信息", Left = 8, Top = 128, Width = 660, Height = 200 };
        Controls.Add(gbGen);
        seMonButchDelayClearTime = BindSpin("尸体延时清理(秒):", 16, 10, gbGen, seMonButchDelayClearTimeChange);
        chkNoHumanClearMon = MkChk("自动清除无人地图怪物", 300, 20, gbGen, chkNoHumanClearMonClick);
        seNoHumanClearMonTime = BindSpin("清理间隔(秒):", 42, 10, gbGen, seNoHumanClearMonTimeChange);
        gbGen.Controls.Add(new System.Windows.Forms.Label { Text = "显示怪物等级:", Left = 300, Top = 46, AutoSize = true });
        cbbMonsterShowLevel = new System.Windows.Forms.ComboBox { Left = 430, Top = 42, Width = 90, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        cbbMonsterShowLevel.Items.Add("不显示");
        cbbMonsterShowLevel.Items.Add("名字后缀");
        cbbMonsterShowLevel.Items.Add("名字前缀");
        cbbMonsterShowLevel.SelectedIndexChanged += (s, e) => cbbMonsterShowLevelChange(s);
        gbGen.Controls.Add(cbbMonsterShowLevel);
        gbGen.Controls.Add(new System.Windows.Forms.Label { Text = "等级格式:", Left = 10, Top = 76, AutoSize = true });
        edtMonsterShowFormat = new System.Windows.Forms.TextBox { Left = 110, Top = 72, Width = 160, Enabled = false };
        edtMonsterShowFormat.TextChanged += (s, e) => edtMonsterShowFormatChange(s);
        gbGen.Controls.Add(edtMonsterShowFormat);
        edtMagStruckMonLevel = BindSpin("魔法顿怪等级:", 98, 10, gbGen, edtMagStruckMonLevelChange);
        edtMagStruckMonDecTime = BindSpin("顿怪延时:", 124, 10, gbGen, edtMagStruckMonDecTimeChange);
        edtMagStruckMonDecRandom = BindSpin("顿怪随机:", 150, 10, gbGen, edtMagStruckMonDecRandomChange);
        seMonStruckFrameDelayTime = BindSpin("顿怪帧延时:", 176, 10, gbGen, seMonStruckFrameDelayTimeChange);
        chkEnabledMaxMapItemCount = MkChk("启用地图物品上限", 300, 98, gbGen, chkEnabledMaxMapItemCountClick);
        seMaxMapItemCount = BindSpin("地图物品上限:", 124, 300, gbGen, chkEnabledMaxMapItemCountClick);
        chkNotDropOverlapItemAll = MkChk("禁止掉落叠加物品", 12, 176, gbGen, chkNotDropOverlapItemAllClick);
        seScatterItemRange = BindSpin("爆物品范围:", 176, 300, gbGen, seScatterItemRangeChange);
        seMonOneDropGoldCount = BindSpin("单次掉金数:", 150, 300, gbGen, seMonOneDropGoldCountChange);
        CheckBoxDropGoldToPlayBag = MkChk("金币入包", 12, 150, gbGen, CheckBoxDropGoldToPlayBagClick);
        seElfWarriorMonsterDownDelay = BindSpin("精灵战士延时:", 176, 300, gbGen, seElfWarriorMonsterDownDelayChange);

        ButtonGeneralSave = new System.Windows.Forms.Button { Text = "保存常规(&G)", Left = 8, Top = 334, Width = 110, Height = 26, Enabled = false };
        ButtonGeneralSave.Click += (s, e) => ButtonGeneralSaveClick(s);
        Controls.Add(ButtonGeneralSave);

        var gbTime = new System.Windows.Forms.GroupBox { Text = "怪物攻/走间隔", Left = 8, Top = 366, Width = 660, Height = 150 };
        Controls.Add(gbTime);
        EditMonsterWarrorAttackTime = BindSpin("战士攻击间隔:", 16, 10, gbTime, EditMonsterWarrorAttackTimeChange);
        EditMonsterWizardAttackTime = BindSpin("法师攻击间隔:", 42, 10, gbTime, EditMonsterWizardAttackTimeChange);
        EditMonsterTaoistAttackTime = BindSpin("道士攻击间隔:", 68, 10, gbTime, EditMonsterTaoistAttackTimeChange);
        EditMonsterWarrorWalkTime = BindSpin("战士走位间隔:", 94, 10, gbTime, EditMonsterWarrorWalkTimeChange);
        EditMonsterWizardWalkTime = BindSpin("法师走位间隔:", 120, 10, gbTime, EditMonsterWizardWalkTimeChange);
        EditMonsterTaoistWalkTime = BindSpin("道士走位间隔:", 146, 10, gbTime, EditMonsterTaoistWalkTimeChange);
        gbTime.Controls.Add(new System.Windows.Forms.Label { Text = "人形怪毒符需求:", Left = 300, Top = 20, AutoSize = true });
        RadioGroupMonsterNeedMagicItem = new System.Windows.Forms.ComboBox { Left = 430, Top = 16, Width = 100, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        RadioGroupMonsterNeedMagicItem.Items.Add("不需要");
        RadioGroupMonsterNeedMagicItem.Items.Add("需要");
        RadioGroupMonsterNeedMagicItem.Items.Add("全部");
        RadioGroupMonsterNeedMagicItem.Items.Add("仅行会");
        RadioGroupMonsterNeedMagicItem.SelectedIndexChanged += (s, e) => RadioGroupMonsterNeedMagicItemClick(s);
        gbTime.Controls.Add(RadioGroupMonsterNeedMagicItem);
        chkSendCustomMonsterConfig = MkChk("下发自定义怪配置", 12, 48, gbTime, chkSendCustomMonsterConfigClick);
        CheckBoxDamageLimitation = MkChk("人形怪伤害封顶", 200, 48, gbTime, CheckBoxDamageLimitationClick);

        ButtonMonsterConfigSave = new System.Windows.Forms.Button { Text = "保存怪物设置(&M)", Left = 8, Top = 522, Width = 130, Height = 26, Enabled = false };
        ButtonMonsterConfigSave.Click += (s, e) => ButtonMonsterConfigSaveClick(s);
        Controls.Add(ButtonMonsterConfigSave);

        // ---- 人形怪配置/魔法/装备组（批次J37） ----
        GroupBoxMonsterConfig = new System.Windows.Forms.GroupBox { Text = "人形怪配置", Left = 8, Top = 556, Width = 660, Height = 260, Enabled = false };
        Controls.Add(GroupBoxMonsterConfig);
        GroupBoxMonsterConfig.Controls.Add(new System.Windows.Forms.Label { Text = "职业:", Left = 10, Top = 22, AutoSize = true });
        cbbJob = new System.Windows.Forms.ComboBox { Left = 60, Top = 18, Width = 90, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        cbbJob.Items.Add("战士");
        cbbJob.Items.Add("法师");
        cbbJob.Items.Add("道士");
        cbbJob.Items.Add("弓手");
        cbbJob.SelectedIndexChanged += (s, e) => cbbJobChange(s);
        GroupBoxMonsterConfig.Controls.Add(cbbJob);
        GroupBoxMonsterConfig.Controls.Add(new System.Windows.Forms.Label { Text = "性别:", Left = 10, Top = 52, AutoSize = true });
        cbbGender = new System.Windows.Forms.ComboBox { Left = 60, Top = 48, Width = 90, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        cbbGender.Items.Add("男");
        cbbGender.Items.Add("女");
        GroupBoxMonsterConfig.Controls.Add(cbbGender);
        seEditHair = MakeSpin("发型:", 48, 220, GroupBoxMonsterConfig);
        chkDieDropUseItem = MkChk("死亡掉装备", 12, 78, GroupBoxMonsterConfig, chkDieDropUseItemClick);
        seDieDropUseItemRate = BindSpin("掉装备几率:", 74, 200, GroupBoxMonsterConfig, (s) => { if (SelMonsterConfig != null && boOpened) { SelMonsterConfig.nDieDropUseItemRate = (int)seDieDropUseItemRate.Value; ModValue(); } });
        chkDieDropBagItem = MkChk("死亡掉背包", 12, 104, GroupBoxMonsterConfig, (s) => { if (boOpened) { SelMonsterConfig!.boDieDropBagItem = chkDieDropBagItem.Checked; ModValue(); } });
        chkButchUseItem = MkChk("挖取身上物品", 12, 130, GroupBoxMonsterConfig, (s) => { if (boOpened) { SelMonsterConfig!.boButchUseItem = chkButchUseItem.Checked; ModValue(); } });
        seButchUseItemRate = BindSpin("挖取几率:", 156, 10, GroupBoxMonsterConfig, (s) => { if (boOpened) { SelMonsterConfig!.nButchUseItemRate = (int)seButchUseItemRate.Value; ModValue(); } });
        chkButchListItem = MkChk("挖取列表物品", 260, 130, GroupBoxMonsterConfig, (s) => { if (boOpened) { SelMonsterConfig!.boButchListItem = chkButchListItem.Checked; ModValue(); } });
        chkButchItemTrigger = MkChk("挖取触发", 260, 156, GroupBoxMonsterConfig, (s) => { if (boOpened) { SelMonsterConfig!.boButchItemTrigger = chkButchItemTrigger.Checked; ModValue(); } });
        GroupBoxMonsterConfig.Controls.Add(new System.Windows.Forms.Label { Text = "收费模式:", Left = 10, Top = 182, AutoSize = true });
        cbbButchChargeMode = new System.Windows.Forms.ComboBox { Left = 80, Top = 178, Width = 90, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        cbbButchChargeMode.Items.Add("金币");
        cbbButchChargeMode.Items.Add("游戏点");
        cbbButchChargeMode.Items.Add("元宝");
        GroupBoxMonsterConfig.Controls.Add(cbbButchChargeMode);
        seButchChargeCount = MakeSpin("收费值:", 178, 210, GroupBoxMonsterConfig);
        chkOnlyButchItemDelGold = MkChk("挖取扣金币", 380, 182, GroupBoxMonsterConfig, (s) => { if (boOpened) { SelMonsterConfig!.boOnlyButchItemDelGold = chkOnlyButchItemDelGold.Checked; ModValue(); } });
        ButtonMonUseItemsChange = new System.Windows.Forms.Button { Text = "修改(&C)", Left = 10, Top = 210, Width = 100, Height = 26, Enabled = false };
        ButtonMonUseItemsChange.Click += (s, e) => ButtonMonUseItemsChangeClick(s);
        GroupBoxMonsterConfig.Controls.Add(ButtonMonUseItemsChange);
        ButtonMonUseItemsSave = new System.Windows.Forms.Button { Text = "保存装备(&S)", Left = 120, Top = 210, Width = 100, Height = 26, Enabled = false };
        ButtonMonUseItemsSave.Click += (s, e) => ButtonMonUseItemsSaveClick(s);
        GroupBoxMonsterConfig.Controls.Add(ButtonMonUseItemsSave);

        var gb2 = new System.Windows.Forms.GroupBox { Text = "保护/攻击", Left = 8, Top = 822, Width = 660, Height = 150 };
        Controls.Add(gb2);
        chkProtectMode = MkChk("保护模式", 12, 20, gb2, chkProtectModeClick);
        EditRestrictMonsterRange = MakeSpin("保护范围:", 16, 200, gb2);
        EditRestrictMonsterRange.ValueChanged += (s, e) => { if (boOpened) { SelMonsterConfig!.nProtectRange = (int)EditRestrictMonsterRange.Value; ModValue(); } };
        CheckBoxNonUseSpellPoint = MkChk("不消耗魔法", 12, 50, gb2, (s) => { if (boOpened) { SelMonsterConfig!.NonUseSpellPoint = CheckBoxNonUseSpellPoint.Checked; ModValue(); } });
        chkRunWithAcctack = MkChk("跑动攻击", 200, 50, gb2, (s) => { if (boOpened) { SelMonsterConfig!.boRunWithAttack = chkRunWithAcctack.Checked; ModValue(); } });
        seRunWithAcctackRate = MakeSpin("跑攻比率:", 80, 200, gb2);
        seRunWithAcctackRate.ValueChanged += (s, e) => { if (boOpened) { SelMonsterConfig!.nRunWithAttackRate = (int)seRunWithAcctackRate.Value; ModValue(); } };
        chkNoAttackMode = MkChk("不攻击模式", 12, 110, gb2, (s) => { if (boOpened) { SelMonsterConfig!.boNoAttackMode = chkNoAttackMode.Checked; ModValue(); } });

        var gb3 = new System.Windows.Forms.GroupBox { Text = "装备槽（拖放/编辑）", Left = 8, Top = 978, Width = 660, Height = 130 };
        Controls.Add(gb3);
        var slots = new (int Slot, string Name)[]
        {
            (0, "衣服"), (1, "武器"), (2, "右手"), (3, "项链"), (4, "头盔"),
            (5, "左手镯"), (6, "右手镯"), (7, "左戒指"), (8, "右戒指"), (9, "符"),
            (10, "腰带"), (11, "鞋"), (12, "宝石"), (16, "盾牌"),
        };
        foreach (var (slot, name) in slots)
        {
            var s = slot;
            gb3.Controls.Add(new System.Windows.Forms.Label { Text = name + ":", Left = 10 + (s % 7) * 92, Top = 22 + (s / 7) * 48, AutoSize = true });
            var edit = new System.Windows.Forms.TextBox { Left = 10 + (s % 7) * 92, Top = 42 + (s / 7) * 48, Width = 80 };
            edit.Tag = s;
            edit.TextChanged += (s, e) => EditDRESSNAMEChange(s);
            gb3.Controls.Add(edit);
            EquipEdits[s] = edit;
        }

        var gb4 = new System.Windows.Forms.GroupBox { Text = "怪物魔法", Left = 8, Top = 1160, Width = 660, Height = 110 };
        Controls.Add(gb4);
        ListBoxMonsterMagicList = new System.Windows.Forms.ListBox { Left = 10, Top = 18, Width = 300, Height = 80 };
        ListBoxMonsterMagicList.SelectedIndexChanged += (s, e) => ListBoxMonsterMagicListClick(s);
        ListBoxMonsterMagicList.DoubleClick += (s, e) => ListBoxMonsterMagicListDblClick(s);
        gb4.Controls.Add(ListBoxMonsterMagicList);
    }

    // ================= Delphi 1:1 =================

    public bool IsModValued => boModValued;

    private void ModValue()
    {
        boModValued = true;
        ButtonGeneralSave.Enabled = true;
        ButtonMonsterConfigSave.Enabled = true;
    }

    private void uModValue()
    {
        boModValued = false;
        ButtonGeneralSave.Enabled = false;
        ButtonMonsterConfigSave.Enabled = false;
    }

    /// <summary>Delphi Open()（人形怪装备双列表/技能列表/自定义怪编辑/掉落限制树在本窗体内）。</summary>
    public void Open(bool showModal = true)
    {
        boOpened = false;
        uModValue();
        EnsureFormCreateDropLimitFill();
        EnsureEffectListsFilled();

        foreach (var name in MagicListHandler?.Invoke() ?? new List<string>())
            ListBoxMagicList.Items.Add(name);
        foreach (var name in MonsterListHandler?.Invoke() ?? new List<string>())
            ListBoxMonsterList.Items.Add(name);
        foreach (var name in CustomMonsterListHandler?.Invoke() ?? new List<string>())
            ListBoxCustomMonster.Items.Add(name);

        EditMonsterWarrorAttackTime.Value = M2Config.dwMonsterWarrorAttackTime;
        EditMonsterWizardAttackTime.Value = M2Config.dwMonsterWizardAttackTime;
        EditMonsterTaoistAttackTime.Value = M2Config.dwMonsterTaoistAttackTime;
        EditMonsterWarrorWalkTime.Value = M2Config.dwMonsterWarrorWalkTime;
        EditMonsterWizardWalkTime.Value = M2Config.dwMonsterWizardWalkTime;
        EditMonsterTaoistWalkTime.Value = M2Config.dwMonsterTaoistWalkTime;
        if (RadioGroupMonsterNeedMagicItem.Items.Count > 0)
            RadioGroupMonsterNeedMagicItem.SelectedIndex = Math.Clamp(M2Config.nMonsterNeedMagicItem, 0, RadioGroupMonsterNeedMagicItem.Items.Count - 1);
        chkSendCustomMonsterConfig.Checked = M2Config.boSendCustomMonsterConfig;
        CheckBoxDamageLimitation.Checked = M2Config.boDamageLimitation;
        RefGeneralInfo();

        boOpened = true;
        if (DropLimitGlobals.g_nKey_DropLimitExt == 1)
        {
            for (int i = 0; i < DropLimitGlobals.g_DropLimitMgr.Count; i++)
            {
                var it = DropLimitGlobals.g_DropLimitMgr.GetItems(i);
                if (it != null)
                    AddDropLimitNode(it);
            }
        }
        // 自定义怪物编辑树（Delphi Open：m_CustomMonsterList → vstCustomMonster）
        foreach (var config in CustomMonsterConfigsHandler?.Invoke() ?? new List<TCustomMonsterConfig>())
            VstCustomMonster.Items.Add(new System.Windows.Forms.ListViewItem(config.MonsterName) { Tag = config });
        if (showModal)
            ShowDialog();
    }

    // ================= 人形怪配置/魔法/装备（批次J37） =================

    /// <summary>ListBoxMonsterListClick 1:1（取人形怪配置并回填控件；SelMonsterConfig 设置）。</summary>
    public void ListBoxMonsterListClick(object? sender)
    {
        if (ListBoxMonsterList.SelectedIndex < 0)
            return;
        var name = ListBoxMonsterList.Items[ListBoxMonsterList.SelectedIndex]?.ToString() ?? "";
        SelMonsterConfig = M2Config.GetPlayMonsterConfig(name);
        if (SelMonsterConfig == null)
            return;
        GroupBoxMonsterConfig.Enabled = true;
        GroupBoxMonsterConfig.Text = SelMonsterConfig.Name;
        if (cbbJob.Items.Count > 0)
            cbbJob.SelectedIndex = Math.Clamp(SelMonsterConfig.Job, 0, cbbJob.Items.Count - 1);
        if (cbbGender.Items.Count > 0)
            cbbGender.SelectedIndex = Math.Clamp(SelMonsterConfig.Gender, 0, cbbGender.Items.Count - 1);
        seEditHair.Value = SelMonsterConfig.Hair;
        chkDieDropUseItem.Checked = SelMonsterConfig.boDieDropUseItem;
        seDieDropUseItemRate.Value = SelMonsterConfig.nDieDropUseItemRate;
        chkDieDropBagItem.Checked = SelMonsterConfig.boDieDropBagItem;
        chkButchUseItem.Checked = SelMonsterConfig.boButchUseItem; // 挖取身上物品
        seButchUseItemRate.Value = SelMonsterConfig.nButchUseItemRate; // 挖取身上物品几率
        chkButchListItem.Checked = SelMonsterConfig.boButchListItem; // 挖取列表物品
        chkButchItemTrigger.Checked = SelMonsterConfig.boButchItemTrigger; // 挖取触发
        if (cbbButchChargeMode.Items.Count > 0)
            cbbButchChargeMode.SelectedIndex = Math.Clamp(SelMonsterConfig.nButchChargeMode, 0, cbbButchChargeMode.Items.Count - 1);
        seButchChargeCount.Value = SelMonsterConfig.nButchChargeCount;
        chkOnlyButchItemDelGold.Checked = SelMonsterConfig.boOnlyButchItemDelGold;
        chkProtectMode.Checked = SelMonsterConfig.boProtectMode;
        EditRestrictMonsterRange.Value = SelMonsterConfig.nProtectRange;
        CheckBoxNonUseSpellPoint.Checked = SelMonsterConfig.NonUseSpellPoint;
        chkRunWithAcctack.Checked = SelMonsterConfig.boRunWithAttack;
        seRunWithAcctackRate.Value = SelMonsterConfig.nRunWithAttackRate;
        chkNoAttackMode.Checked = SelMonsterConfig.boNoAttackMode;
        ListBoxMonsterMagicList.Items.Clear();
        foreach (var m in SelMonsterConfig.Magics)
            ListBoxMonsterMagicList.Items.Add(m);
    }

    /// <summary>ListBoxMonsterMagicListDragDrop 1:1（Source=魔法列表时：CompareText 去重后加入）。</summary>
    /// <summary>ListBoxMonsterMagicListClick 1:1（回填 SelMonsterConfig.Magics 选中态，此处保持空实现同原文）。。</summary>
    public void ListBoxMonsterMagicListClick(object? sender)
    {
    }

    public void ListBoxMonsterMagicListDragDrop(object? sender)
    {
        if (SelMonsterConfig == null)
            return;
        for (int i = 0; i < ListBoxMagicList.Items.Count; i++)
        {
            if (!ListBoxMagicList.GetSelected(i))
                continue;
            var sName = ListBoxMagicList.Items[i]?.ToString() ?? "";
            var boFind = false;
            for (int ii = 0; ii < ListBoxMonsterMagicList.Items.Count; ii++)
            {
                if (string.Compare(sName, ListBoxMonsterMagicList.Items[ii]?.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ListBoxMonsterMagicList.SelectedIndex = ii;
                    boFind = true;
                    break;
                }
            }
            if (!boFind)
            {
                ButtonMonUseItemsChange.Enabled = true;
                ListBoxMonsterMagicList.Items.Add(sName);
            }
        }
    }

    /// <summary>GroupBoxMonsterUseItemDragDrop 1:1（GetTakeOnPosition 槽位匹配，戒指/手镯左右互通）。</summary>
    public void GroupBoxMonsterUseItemDragDrop(int equipTag, string itemName)
    {
        if (SelMonsterConfig == null)
            return;
        int where = GetTakeOnPosition(equipTag);
        switch (where)
        {
            case 7: // U_RINGL
                if (equipTag is 7 or 8)
                    EquipEdits[equipTag].Text = itemName;
                break;
            case 6: // U_ARMRINGR
                if (equipTag is 6 or 5)
                    EquipEdits[equipTag].Text = itemName;
                break;
            default:
                if (EquipEdits.TryGetValue(equipTag, out var edit) && edit.Tag is int tag && tag == equipTag)
                    edit.Text = itemName;
                break;
        }
    }

    /// <summary>装备拖放槽位写入测试入口（Delphi GroupBoxMonsterUseItemDragDrop 主体：stdMode 定槽，Tag 定编辑框）。</summary>
    public void DragDropEquip(int slotTag, string itemName, int stdMode)
    {
        int where = GetTakeOnPosition(stdMode);
        bool write = where switch
        {
            7 => slotTag is 7 or 8,
            6 => slotTag is 6 or 5,
            _ => slotTag == where,
        };
        if (write)
            EquipEdits[slotTag].Text = itemName;
    }

    /// <summary>Ctrl+F 查找 1:1（InputQuery 取名 → 逐项精确匹配 → ItemIndex 定位）。</summary>
    public void FindInList(System.Windows.Forms.ListBox list, string sName)
    {
        if (sName == "")
            return;
        for (int i = 0; i < list.Items.Count; i++)
        {
            if (string.Equals(list.Items[i]?.ToString(), sName, StringComparison.Ordinal))
            {
                list.SelectedIndex = i;
                return;
            }
        }
    }

    public void cbbJobChange(object? sender)
    {
        if (SelMonsterConfig == null)
            return;
        ListBoxMagicList.Items.Clear();
        foreach (var name in MagicListHandler?.Invoke() ?? new List<string>())
            ListBoxMagicList.Items.Add(name); // Delphi 按 btJob 过滤，接缝由引擎批次提供职业字段
        ButtonMonUseItemsChange.Enabled = true;
    }

    public void ListBoxMonsterMagicListDblClick(object? sender)
    {
        if (SelMonsterConfig == null)
            return;
        if (ListBoxMonsterMagicList.SelectedIndex >= 0)
            ListBoxMonsterMagicList.Items.RemoveAt(ListBoxMonsterMagicList.SelectedIndex);
        ButtonMonUseItemsChange.Enabled = true;
    }

    public void EditDRESSNAMEChange(object? sender)
    {
        if (SelMonsterConfig != null)
            ButtonMonUseItemsChange.Enabled = true;
    }

    public void chkDieDropUseItemClick(object? sender)
    {
        if (SelMonsterConfig == null || !boOpened)
            return;
        SelMonsterConfig.boDieDropUseItem = chkDieDropUseItem.Checked;
        ModValue();
    }

    public void chkProtectModeClick(object? sender)
    {
        if (SelMonsterConfig == null || !boOpened)
            return;
        SelMonsterConfig.boProtectMode = chkProtectMode.Checked;
        ModValue();
    }

    /// <summary>ButtonMonUseItemsChangeClick 1:1（全部控件写回 SelMonsterConfig；装备编辑框按 Tag 收集；
    /// 魔法列表整体写回 Magics；IsChanged=True；使能保存按钮）。</summary>
    public void ButtonMonUseItemsChangeClick(object? sender)
    {
        ButtonMonUseItemsChange.Enabled = false;
        if (SelMonsterConfig == null)
            return;
        SelMonsterConfig.Job = cbbJob.SelectedIndex;
        SelMonsterConfig.Gender = cbbGender.SelectedIndex;
        SelMonsterConfig.Hair = (int)seEditHair.Value;
        SelMonsterConfig.boDieDropUseItem = chkDieDropUseItem.Checked;
        SelMonsterConfig.nDieDropUseItemRate = (int)seDieDropUseItemRate.Value;
        SelMonsterConfig.boDieDropBagItem = chkDieDropBagItem.Checked;
        SelMonsterConfig.boButchUseItem = chkButchUseItem.Checked; // 挖取身上物品
        SelMonsterConfig.nButchUseItemRate = (int)seButchUseItemRate.Value; // 挖取身上物品几率
        SelMonsterConfig.boButchListItem = chkButchListItem.Checked; // 挖取列表物品
        SelMonsterConfig.boButchItemTrigger = chkButchItemTrigger.Checked; // 挖取触发
        SelMonsterConfig.nButchChargeMode = cbbButchChargeMode.SelectedIndex; // 挖取收费模式
        SelMonsterConfig.nButchChargeCount = (int)seButchChargeCount.Value; // 挖取收费值
        SelMonsterConfig.boOnlyButchItemDelGold = chkOnlyButchItemDelGold.Checked;
        SelMonsterConfig.boProtectMode = chkProtectMode.Checked;
        SelMonsterConfig.nProtectRange = (int)EditRestrictMonsterRange.Value;
        SelMonsterConfig.NonUseSpellPoint = CheckBoxNonUseSpellPoint.Checked;
        SelMonsterConfig.boRunWithAttack = chkRunWithAcctack.Checked;
        SelMonsterConfig.nRunWithAttackRate = (int)seRunWithAcctackRate.Value;
        SelMonsterConfig.boNoAttackMode = chkNoAttackMode.Checked;

        foreach (var (slot, edit) in EquipEdits)
            SelMonsterConfig.UseItems[slot] = edit.Text;
        SelMonsterConfig.Magics.Clear();
        foreach (var item in ListBoxMonsterMagicList.Items)
            SelMonsterConfig.Magics.Add(item?.ToString() ?? "");
        SelMonsterConfig.IsChanged = true;
        ButtonMonUseItemsSave.Enabled = true;
    }

    /// <summary>ButtonMonUseItemsSaveClick 1:1（SavePlayMonsterConfigList 接缝）。</summary>
    public void ButtonMonUseItemsSaveClick(object? sender)
    {
        ButtonMonUseItemsSave.Enabled = false;
        if (SelMonsterConfig != null)
            M2Config.SavePlayMonsterConfig(SelMonsterConfig);
    }

    /// <summary>RefGeneralInfo 1:1。</summary>
    public void RefGeneralInfo()
    {
        seMonButchDelayClearTime.Value = M2Config.dwMonButchDelayClearTime;
        chkNoHumanClearMon.Checked = M2Config.boNoHumanClearMon;
        seNoHumanClearMonTime.Value = M2Config.dwNoHumanClearMonTime;
        if (cbbMonsterShowLevel.Items.Count > 0)
            cbbMonsterShowLevel.SelectedIndex = Math.Clamp(M2Config.btMonsterShowLevel, 0, cbbMonsterShowLevel.Items.Count - 1);
        edtMonsterShowFormat.Text = M2Config.sMonsterShowFormat;
        edtMonsterShowFormat.Enabled = M2Config.btMonsterShowLevel > 0;
        edtMagStruckMonLevel.Value = M2Config.nMagStruckMonLevel;
        edtMagStruckMonDecTime.Value = M2Config.nMagStruckMonDecTime;
        edtMagStruckMonDecRandom.Value = M2Config.nMagStruckMonDecRandom;
        seMonStruckFrameDelayTime.Value = M2Config.btMonStruckFrameDelayTime;
        chkEnabledMaxMapItemCount.Checked = M2Config.boEnabledMaxMapItemCount;
        seMaxMapItemCount.Value = M2Config.nMaxMapItemCount;
        chkNotDropOverlapItemAll.Checked = M2Config.boNotDropOverlapItemAll;
        seScatterItemRange.Value = M2Config.nScatterItemRange;
        seMonOneDropGoldCount.Value = M2Config.nMonOneDropGoldCount;
        CheckBoxDropGoldToPlayBag.Checked = M2Config.boDropGoldToPlayBag;
        seElfWarriorMonsterDownDelay.Value = M2Config.nElfWarriorMonsterDownDelay;
    }

    // ---- 常规信息组处理器（批次J36） ----

    public void seMonButchDelayClearTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMonButchDelayClearTime = (uint)seMonButchDelayClearTime.Value;
        ModValue();
    }

    public void chkNoHumanClearMonClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boNoHumanClearMon = chkNoHumanClearMon.Checked;
        ModValue();
    }

    public void seNoHumanClearMonTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwNoHumanClearMonTime = (uint)seNoHumanClearMonTime.Value;
        ModValue();
    }

    public void cbbMonsterShowLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btMonsterShowLevel = (byte)cbbMonsterShowLevel.SelectedIndex;
        edtMonsterShowFormat.Enabled = M2Config.btMonsterShowLevel > 0;
        ModValue();
    }

    public void edtMonsterShowFormatChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.sMonsterShowFormat = edtMonsterShowFormat.Text;
        ModValue();
    }

    public void edtMagStruckMonLevelChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMagStruckMonLevel = (int)edtMagStruckMonLevel.Value;
        ModValue();
    }

    public void edtMagStruckMonDecTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMagStruckMonDecTime = (int)edtMagStruckMonDecTime.Value;
        ModValue();
    }

    public void edtMagStruckMonDecRandomChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMagStruckMonDecRandom = (int)edtMagStruckMonDecRandom.Value;
        ModValue();
    }

    public void seMonStruckFrameDelayTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.btMonStruckFrameDelayTime = (byte)seMonStruckFrameDelayTime.Value;
        ModValue();
    }

    public void chkEnabledMaxMapItemCountClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boEnabledMaxMapItemCount = chkEnabledMaxMapItemCount.Checked;
        ModValue();
    }

    public void seMaxMapItemCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMaxMapItemCount = (int)seMaxMapItemCount.Value;
        ModValue();
    }

    public void chkNotDropOverlapItemAllClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boNotDropOverlapItemAll = chkNotDropOverlapItemAll.Checked;
        ModValue();
    }

    public void seScatterItemRangeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nScatterItemRange = (int)seScatterItemRange.Value;
        ModValue();
    }

    public void seMonOneDropGoldCountChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMonOneDropGoldCount = (int)seMonOneDropGoldCount.Value;
        ModValue();
    }

    public void CheckBoxDropGoldToPlayBagClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDropGoldToPlayBag = CheckBoxDropGoldToPlayBag.Checked;
        ModValue();
    }

    public void seElfWarriorMonsterDownDelayChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nElfWarriorMonsterDownDelay = (int)seElfWarriorMonsterDownDelay.Value;
        ModValue();
    }

    /// <summary>ButtonGeneralSaveClick 1:1（16 键 + 尾部无条件下发 + uModValue）。</summary>
    public void ButtonGeneralSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "MonButchDelayClearTime", (int)M2Config.dwMonButchDelayClearTime);
        Config.WriteBool("Setup", "NoHumanClearMon", M2Config.boNoHumanClearMon);
        Config.WriteInteger("Setup", "NoHumanClearMonTime", (int)M2Config.dwNoHumanClearMonTime);
        Config.WriteInteger("Setup", "MonsterShowLevel", M2Config.btMonsterShowLevel);
        Config.WriteString("Setup", "MonsterShowFormat", M2Config.sMonsterShowFormat);
        Config.WriteInteger("Setup", "MagStruckMonLevel", M2Config.nMagStruckMonLevel);
        Config.WriteInteger("Setup", "MagStruckMonDecTime", M2Config.nMagStruckMonDecTime);
        Config.WriteInteger("Setup", "MagStruckMonDecRandom", M2Config.nMagStruckMonDecRandom);
        Config.WriteInteger("Setup", "ElfWarriorMonsterDownDelay", M2Config.nElfWarriorMonsterDownDelay);
        Config.WriteInteger("Setup", "MonStruckFrameDelayTime", M2Config.btMonStruckFrameDelayTime);
        // 爆物品范围
        Config.WriteInteger("Setup", "ScatterItemRange", M2Config.nScatterItemRange);
        Config.WriteBool("Setup", "EnabledMaxMapItemCount", M2Config.boEnabledMaxMapItemCount);
        Config.WriteInteger("Setup", "MaxMapItemCount", M2Config.nMaxMapItemCount);
        Config.WriteBool("Setup", "NotDropOverlapItemAll", M2Config.boNotDropOverlapItemAll);
        Config.WriteInteger("Setup", "MonOneDropGoldCount", M2Config.nMonOneDropGoldCount);
        Config.WriteBool("Setup", "DropGoldToPlayBag", M2Config.boDropGoldToPlayBag);
        GameConfigState.SendServerConfig(); // Delphi 尾部无条件下发
        uModValue();
    }

    // ---- 怪物时序组处理器（批次J36） ----

    public void EditMonsterWarrorAttackTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMonsterWarrorAttackTime = (uint)EditMonsterWarrorAttackTime.Value;
        ModValue();
    }

    public void EditMonsterWizardAttackTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMonsterWizardAttackTime = (uint)EditMonsterWizardAttackTime.Value;
        ModValue();
    }

    public void EditMonsterTaoistAttackTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMonsterTaoistAttackTime = (uint)EditMonsterTaoistAttackTime.Value;
        ModValue();
    }

    public void EditMonsterWarrorWalkTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMonsterWarrorWalkTime = (uint)EditMonsterWarrorWalkTime.Value;
        ModValue();
    }

    public void EditMonsterWizardWalkTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMonsterWizardWalkTime = (uint)EditMonsterWizardWalkTime.Value;
        ModValue();
    }

    public void EditMonsterTaoistWalkTimeChange(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.dwMonsterTaoistWalkTime = (uint)EditMonsterTaoistWalkTime.Value;
        ModValue();
    }

    public void RadioGroupMonsterNeedMagicItemClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.nMonsterNeedMagicItem = RadioGroupMonsterNeedMagicItem.SelectedIndex;
        ModValue();
    }

    public void chkSendCustomMonsterConfigClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boSendCustomMonsterConfig = chkSendCustomMonsterConfig.Checked;
        ModValue();
    }

    public void CheckBoxDamageLimitationClick(object? sender)
    {
        if (!boOpened)
            return;
        M2Config.boDamageLimitation = CheckBoxDamageLimitation.Checked;
        ModValue();
    }

    /// <summary>ButtonMonsterConfigSaveClick 1:1（8 键）。</summary>
    public void ButtonMonsterConfigSaveClick(object? sender)
    {
        var Config = M2ShareState.ConfigIni;
        Config.WriteInteger("Setup", "MonsterWarrorAttackTime", (int)M2Config.dwMonsterWarrorAttackTime);
        Config.WriteInteger("Setup", "MonsterWizardAttackTime", (int)M2Config.dwMonsterWizardAttackTime);
        Config.WriteInteger("Setup", "MonsterTaoistAttackTime", (int)M2Config.dwMonsterTaoistAttackTime);
        Config.WriteInteger("Setup", "MonsterWarrorWalkTime", (int)M2Config.dwMonsterWarrorWalkTime);
        Config.WriteInteger("Setup", "MonsterWizardWalkTime", (int)M2Config.dwMonsterWizardWalkTime);
        Config.WriteInteger("Setup", "MonsterTaoistWalkTime", (int)M2Config.dwMonsterTaoistWalkTime);
        Config.WriteInteger("Setup", "MonsterNeedMagicItem", M2Config.nMonsterNeedMagicItem);
        Config.WriteBool("Setup", "DamageLimitation", M2Config.boDamageLimitation);
        uModValue();
    }
}

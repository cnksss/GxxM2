// ============================================================================
//  源单元：Source/M2Engine/Forms/uFrmMainGamePets.pas（1,353 行，GBK）
//  同源 DFM：Source/M2Engine/Forms/uFrmMainGamePets.dfm（53,800 字节）
//  类型：TFrmGamePets（:17-227）；全局 SelGamePetConfig（:235）
//  本文件：类型声明 + 构造（DFM 控件树 1:1）+ RefreshGamePetConfigList（:521-532）
//  ⚠ 全树 0 命中（本车道开工前 git grep 确认 TFrmGamePets 未移植）。
// ============================================================================

using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms.GamePets;

/// <summary>
/// `uFrmMainGamePets.pas` `TFrmGamePets` 1:1（M2Engine 宠物管理窗体，1,353 行）。
///
/// 窗体分三页签（`pgcMain`）：
///   · 0 = `ts1`  宠物列表/属性（增删改 + 捕捉/显示/加成）
///   · 1 = `TabSheet1` 升级经验表（`GridLevelExp` + 19 种经验计划）
///   · 2 = `TabSheet2` 全局宠物开关/捡物/叠加/捕捉/命名参数
///
/// 无头 UI 处置：全部弹窗走既有 `M2Forms.MessageBox`（测试可注入 handler，
/// 既有 M2Server 窗体族的标准接缝），`InputQuery` 走本窗体 `InputQueryHandler`，
/// `ShowModal` 走 `DoOpen(bool showModal)` 参数。
/// </summary>
public sealed partial class GamePetsForm : System.Windows.Forms.Form
{
    // ---- :219-220 private 字段 ----
    /// <summary>:219 `boOpened: Boolean`（DoOpen 期间与之后为 True；未打开时处理器早退）。</summary>
    private bool boOpened;
    /// <summary>:220 `boModValued: Boolean`。</summary>
    private bool boModValued;

    // ---- :235 单元级全局 `SelGamePetConfig: PTGamePetConfig = nil` ----
    /// <summary>:235 `SelGamePetConfig`（当前选中项；原文为单元全局，此处为窗体字段）。</summary>
    public TGamePetConfig? SelGamePetConfig;

    // ========================================================================
    //  DFM 控件树（uFrmMainGamePets.dfm）
    // ========================================================================

    // ---- :18-19 页签容器 ----
    /// <summary>:18 `pgcMain: TPageControl`。</summary>
    public System.Windows.Forms.TabControl pgcMain = null!;
    /// <summary>:19 `ts1: TTabSheet`（宠物列表页）。</summary>
    public System.Windows.Forms.TabPage ts1 = null!;
    /// <summary>:37 `TabSheet1: TTabSheet`（升级经验页）。</summary>
    public System.Windows.Forms.TabPage TabSheet1 = null!;
    /// <summary>:47 `TabSheet2: TTabSheet`（全局参数页）。</summary>
    public System.Windows.Forms.TabPage TabSheet2 = null!;

    // ---- :29-36 宠物列表页 ----
    /// <summary>:29 `lstMonsterList: TListBox`（怪物名候选表）。</summary>
    public System.Windows.Forms.ListBox lstMonsterList = null!;
    /// <summary>:30 `lstGamePets: TListBox`（已配置宠物列表）。</summary>
    public System.Windows.Forms.ListBox lstGamePets = null!;
    /// <summary>:31 `btnAddPet: TButton`。</summary>
    public System.Windows.Forms.Button btnAddPet = null!;
    /// <summary>:32 `btnDelPet: TButton`。</summary>
    public System.Windows.Forms.Button btnDelPet = null!;
    /// <summary>:33 `btnEditPet: TButton`。</summary>
    public System.Windows.Forms.Button btnEditPet = null!;
    /// <summary>:34 `btnSavePet: TButton`。</summary>
    public System.Windows.Forms.Button btnSavePet = null!;
    /// <summary>:36 `edtPetName: TEdit`。</summary>
    public System.Windows.Forms.TextBox edtPetName = null!;
    /// <summary>:48 `sePetCaptureRate: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown sePetCaptureRate = null!;
    /// <summary>:129 `chkLevelDifference: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkLevelDifference = null!;
    /// <summary>:130 `seLevelDifference: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seLevelDifference = null!;
    /// <summary>:132 `seHPScale: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown seHPScale = null!;

    // ---- :26-28 / :53-59 显示文件两组 ----
    /// <summary>:26 `cbbPetShowFile1: TComboBox`。</summary>
    public System.Windows.Forms.ComboBox cbbPetShowFile1 = null!;
    /// <summary>:27 `sePetShowCount1: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown sePetShowCount1 = null!;
    /// <summary>:28 `sePetShowStart1: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown sePetShowStart1 = null!;
    /// <summary>:53 `cbbPetShowFile2: TComboBox`。</summary>
    public System.Windows.Forms.ComboBox cbbPetShowFile2 = null!;
    /// <summary>:54 `sePetShowCount2: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown sePetShowCount2 = null!;
    /// <summary>:55 `sePetShowStart2: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown sePetShowStart2 = null!;
    /// <summary>:57 `sePetShowTime1: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown sePetShowTime1 = null!;
    /// <summary>:59 `sePetShowTime2: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown sePetShowTime2 = null!;
    /// <summary>:97 `sePetShowOffsetX1: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown sePetShowOffsetX1 = null!;
    /// <summary>:99 `sePetShowOffsetY1: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown sePetShowOffsetY1 = null!;
    /// <summary>:101 `sePetShowOffsetX2: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown sePetShowOffsetX2 = null!;
    /// <summary>:103 `sePetShowOffsetY2: TSpinEditEx`。</summary>
    public System.Windows.Forms.NumericUpDown sePetShowOffsetY2 = null!;

    // ---- :61-93 属性加成组 ----
    /// <summary>:63 `sePetAddHP`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAddHP = null!;
    /// <summary>:64 `cbbPetAddHPType`。</summary>
    public System.Windows.Forms.ComboBox cbbPetAddHPType = null!;
    /// <summary>:66 `sePetAddDC1`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAddDC1 = null!;
    /// <summary>:67 `cbbPetAddDCType1`。</summary>
    public System.Windows.Forms.ComboBox cbbPetAddDCType1 = null!;
    /// <summary>:69 `sePetAddSC1`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAddSC1 = null!;
    /// <summary>:70 `cbbPetAddSCType1`。</summary>
    public System.Windows.Forms.ComboBox cbbPetAddSCType1 = null!;
    /// <summary>:71 `sePetAddSC2`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAddSC2 = null!;
    /// <summary>:72 `cbbPetAddSCType2`。</summary>
    public System.Windows.Forms.ComboBox cbbPetAddSCType2 = null!;
    /// <summary>:73 `sePetAddDC2`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAddDC2 = null!;
    /// <summary>:74 `cbbPetAddDCType2`。</summary>
    public System.Windows.Forms.ComboBox cbbPetAddDCType2 = null!;
    /// <summary>:76 `sePetAddAC1`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAddAC1 = null!;
    /// <summary>:77 `cbbPetAddACType1`。</summary>
    public System.Windows.Forms.ComboBox cbbPetAddACType1 = null!;
    /// <summary>:78 `sePetAddAC2`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAddAC2 = null!;
    /// <summary>:79 `cbbPetAddACType2`。</summary>
    public System.Windows.Forms.ComboBox cbbPetAddACType2 = null!;
    /// <summary>:81 `sePetAddMAC1`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAddMAC1 = null!;
    /// <summary>:82 `cbbPetAddMACType1`。</summary>
    public System.Windows.Forms.ComboBox cbbPetAddMACType1 = null!;
    /// <summary>:83 `sePetAddMAC2`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAddMAC2 = null!;
    /// <summary>:84 `cbbPetAddMACType2`。</summary>
    public System.Windows.Forms.ComboBox cbbPetAddMACType2 = null!;
    /// <summary>:90 `sePetAddMC1`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAddMC1 = null!;
    /// <summary>:91 `cbbPetAddMCType1`。</summary>
    public System.Windows.Forms.ComboBox cbbPetAddMCType1 = null!;
    /// <summary>:92 `sePetAddMC2`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAddMC2 = null!;
    /// <summary>:93 `cbbPetAddMCType2`。</summary>
    public System.Windows.Forms.ComboBox cbbPetAddMCType2 = null!;

    // ---- :40-41 升级经验页 ----
    /// <summary>:40 `GridLevelExp: TStringGrid`。</summary>
    public System.Windows.Forms.DataGridView GridLevelExp = null!;
    /// <summary>:41 `cbbLevelExp: TComboBox`。</summary>
    public System.Windows.Forms.ComboBox cbbLevelExp = null!;
    /// <summary>:45 `sePetHighLevel`。</summary>
    public System.Windows.Forms.NumericUpDown sePetHighLevel = null!;
    /// <summary>:46 `sePetHighLevelGetExp`。</summary>
    public System.Windows.Forms.NumericUpDown sePetHighLevelGetExp = null!;
    /// <summary>:125 `chkPetFixExp: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkPetFixExp = null!;
    /// <summary>:126 `sePetBaseExp`。</summary>
    public System.Windows.Forms.NumericUpDown sePetBaseExp = null!;
    /// <summary>:127 `sePetAddExp`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAddExp = null!;
    /// <summary>:117 `btnSaveExp: TButton`。</summary>
    public System.Windows.Forms.Button btnSaveExp = null!;
    /// <summary>:118 `btnSavePetParams: TButton`。</summary>
    public System.Windows.Forms.Button btnSavePetParams = null!;

    // ---- :105-121 全局参数页开关 ----
    /// <summary>:116 `chkOpenGamePet: TCheckBox`。</summary>
    public System.Windows.Forms.CheckBox chkOpenGamePet = null!;
    /// <summary>:153 `chkEnabledPetAttack`。</summary>
    public System.Windows.Forms.CheckBox chkEnabledPetAttack = null!;
    /// <summary>:154 `chkDisableMonAttackPet`。</summary>
    public System.Windows.Forms.CheckBox chkDisableMonAttackPet = null!;
    /// <summary>:155 `chkDisableAllAttackPet`。</summary>
    public System.Windows.Forms.CheckBox chkDisableAllAttackPet = null!;
    /// <summary>:156 `chkPetNoEntity`。</summary>
    public System.Windows.Forms.CheckBox chkPetNoEntity = null!;
    /// <summary>:157 `chkPetSleepControlBySlave`。</summary>
    public System.Windows.Forms.CheckBox chkPetSleepControlBySlave = null!;
    /// <summary>:158 `chkPetNoShowHPProgress`。</summary>
    public System.Windows.Forms.CheckBox chkPetNoShowHPProgress = null!;
    /// <summary>:160 `chkEnabledPetPickup`。</summary>
    public System.Windows.Forms.CheckBox chkEnabledPetPickup = null!;
    /// <summary>:161 `chkPetPickupFullToMaster`。</summary>
    public System.Windows.Forms.CheckBox chkPetPickupFullToMaster = null!;
    /// <summary>:162 `chkPetPickupToMaster`。</summary>
    public System.Windows.Forms.CheckBox chkPetPickupToMaster = null!;
    /// <summary>:163 `chkPetQuickPickup`。</summary>
    public System.Windows.Forms.CheckBox chkPetQuickPickup = null!;
    /// <summary>:164 `chkPetRangePickup`。</summary>
    public System.Windows.Forms.CheckBox chkPetRangePickup = null!;
    /// <summary>:165 `sePetPickupRange`。</summary>
    public System.Windows.Forms.NumericUpDown sePetPickupRange = null!;
    /// <summary>:166 `chkEnablePetUseClientPickItems`。</summary>
    public System.Windows.Forms.CheckBox chkEnablePetUseClientPickItems = null!;
    /// <summary>:167 `chkPetOnlyPickMonsterItem`。</summary>
    public System.Windows.Forms.CheckBox chkPetOnlyPickMonsterItem = null!;
    /// <summary>:168 `chkGamePetKillMonTrigger`。</summary>
    public System.Windows.Forms.CheckBox chkGamePetKillMonTrigger = null!;
    /// <summary>:106-111 叠加给主人六开关。</summary>
    public System.Windows.Forms.CheckBox chkPetHPToMaster = null!, chkPetDCToMaster = null!,
        chkPetMCToMaster = null!, chkPetSCToMaster = null!, chkPetACToMaster = null!, chkPetMACToMaster = null!;
    /// <summary>:113 `sePetAbilToMasterRate`。</summary>
    public System.Windows.Forms.NumericUpDown sePetAbilToMasterRate = null!;
    /// <summary>:121 `sePetUseItemIntervalTime`。</summary>
    public System.Windows.Forms.NumericUpDown sePetUseItemIntervalTime = null!;
    /// <summary>:125/:135 `chkPetShowMasterName`。</summary>
    public System.Windows.Forms.CheckBox chkPetShowMasterName = null!;
    /// <summary>:137 `sePetNameColor: TColorIndexEdit`（颜色索引编辑器 → NumericUpDown）。</summary>
    public System.Windows.Forms.NumericUpDown sePetNameColor = null!;
    /// <summary>:138 `edtPetSuffixName: TEdit`。</summary>
    public System.Windows.Forms.TextBox edtPetSuffixName = null!;
    /// <summary>:141 `chkCapturePetNeedItem`。</summary>
    public System.Windows.Forms.CheckBox chkCapturePetNeedItem = null!;
    /// <summary>:142 `chkCaptureOKDecDura`。</summary>
    public System.Windows.Forms.CheckBox chkCaptureOKDecDura = null!;
    /// <summary>:145 `seGamePetMaxCount`。</summary>
    public System.Windows.Forms.NumericUpDown seGamePetMaxCount = null!;
    /// <summary>:147 `seGamePetNameCount`。</summary>
    public System.Windows.Forms.NumericUpDown seGamePetNameCount = null!;
    /// <summary>:150 `seGamePetRecallTime`。</summary>
    public System.Windows.Forms.NumericUpDown seGamePetRecallTime = null!;

    // ========================================================================
    //  接缝（原文依赖的未移植单元 / 不可测试的副作用）
    // ========================================================================

    /// <summary>接缝：`UserEngine.MonsterList`（:253-260 枚举怪物名）。
    /// 返回 `(sName, btRace)` 序列；`null` 视为空列表。
    /// 接缝：待 UsrEngn.pas / ObjMon.pas 怪物表移植后接入。
    /// 要求签名：`IReadOnlyList&lt;(string sName, byte btRace)&gt; UserEngine.MonsterList`。</summary>
    public Func<IReadOnlyList<(string sName, byte btRace)>?>? MonsterListHandler;

    /// <summary>接缝：`g_MultiThreadRun` + `UserEngine.MonsterList.LockR/UnLockR`（:249-264）。
    /// 原文 `{$IF MULTI_THREAD = 1}` 包裹；托管侧默认 false = 不加锁。
    /// 接缝：待 M2Threads.pas / UsrEngn.pas 移植后接入。</summary>
    public bool g_MultiThreadRun;
    /// <summary>接缝：`UserEngine.MonsterList.LockR(11)`（:250）。</summary>
    public Action<int>? MonsterListLockR;
    /// <summary>接缝：`UserEngine.MonsterList.UnLockR`（:264）。</summary>
    public Action? MonsterListUnLockR;

    /// <summary>接缝：`g_EffectImageList: TStringList`（:273-277 填充显示文件下拉）。
    /// 接缝：待 M2Share.pas 全局表移植后接入。</summary>
    public Func<IReadOnlyList<string>?>? EffectImageListHandler;

    /// <summary>接缝：`Config.WriteBool('Setup', key, value)`（:833-900，!!Setup.txt）。
    /// 接缝：待 M2Share.pas `Config` 全局 TIniFile 移植后接入。
    /// 要求签名：`Action&lt;string key, bool value&gt;`（section 恒为 'Setup'，与既有
    /// `CombatPowerSettingForm.WriteBoolHandler` / `CustomNpcForm.WriteBoolHandler` 同形）。</summary>
    public Action<string, bool>? WriteBoolHandler;
    /// <summary>接缝：`Config.WriteInteger('Setup', key, value)`。</summary>
    public Action<string, int>? WriteIntegerHandler;
    /// <summary>接缝：`Config.WriteString('Setup', key, value)`。</summary>
    public Action<string, string>? WriteStringHandler;

    /// <summary>接缝：`ExpConfig.WriteString`（:1177，升级经验 INI）。
    /// 接缝：待 M2Share.pas `ExpConfig` 全局 TIniFile 移植后接入。</summary>
    public Action<string, string, string>? ExpConfigWriteStringHandler;

    /// <summary>接缝：`UserEngine.SendServerConfig`（:910）。</summary>
    public Action? SendServerConfigHandler;

    /// <summary>VMProtect 键控 `g_nKey_UseClientPickItems`（:346/:903）：
    /// == 1 时才勾选/写盘 `EnablePetUseClientPickItems`，否则整控件 Visible=False（:352）。</summary>
    public int g_nKey_UseClientPickItems = 1;

    /// <summary>接缝：`Dialogs.InputQuery('怪物查找', '输入怪物名称:', sMonName)`（:683）。
    /// 签名对齐既有 `MonsterConfigForm.InputQueryHandler`：返回 (Ok, Value)，Ok=false = 用户取消。</summary>
    public Func<string, string, string, (bool Ok, string Value)>? InputQueryHandler;

    /// <summary>接缝：`GridLevelExp.Row := I; GridLevelExp.SetFocus`（:1161-1162）——
    /// 无头环境下不可断言的光标定位；测试注入以断言"定位到错误行"决策。</summary>
    public Action<int>? GridLevelExpLocateRowHandler;

    /// <summary>接缝：`Application.MessageBox`（既有 M2Server 窗体族标准接缝，测试注入不弹窗）。</summary>
    public Func<string, string, int, int>? MessageBoxHandler;

    /// <summary>探针：`lstGamePets.SetFocus`（:464/:550/:622）/ `edtPetName.SetFocus`（:543/:550）
    /// 等无头不可断言的焦点调用，测试注入以断言走了哪个分支。</summary>
    public Action<string>? SetFocusProbe;

    public GamePetsForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "宠物管理";
        ClientSize = new System.Drawing.Size(900, 640);

        pgcMain = new System.Windows.Forms.TabControl { Dock = System.Windows.Forms.DockStyle.Fill };
        ts1 = new System.Windows.Forms.TabPage("宠物");
        TabSheet1 = new System.Windows.Forms.TabPage("升级经验");
        TabSheet2 = new System.Windows.Forms.TabPage("宠物设置");
        pgcMain.TabPages.Add(ts1);
        pgcMain.TabPages.Add(TabSheet1);
        pgcMain.TabPages.Add(TabSheet2);
        Controls.Add(pgcMain);

        // ---- 宠物列表页（ts1）----
        lstMonsterList = new System.Windows.Forms.ListBox { Left = 8, Top = 24, Width = 200, Height = 400, IntegralHeight = false };
        lstMonsterList.DoubleClick += (_, _) => lstMonsterListDblClick(lstMonsterList);
        lstMonsterList.KeyDown += (_, e) => lstMonsterListKeyDown(lstMonsterList, (ushort)e.KeyValue, e.Control);
        ts1.Controls.Add(lstMonsterList);

        lstGamePets = new System.Windows.Forms.ListBox { Left = 216, Top = 24, Width = 200, Height = 400, IntegralHeight = false };
        lstGamePets.Click += (_, _) => lstGamePetsClick(lstGamePets);
        ts1.Controls.Add(lstGamePets);

        edtPetName = new System.Windows.Forms.TextBox { Left = 216, Top = 0, Width = 200 };
        ts1.Controls.Add(edtPetName);

        btnAddPet = new System.Windows.Forms.Button { Left = 424, Top = 22, Width = 90, Text = "增加" };
        btnAddPet.Click += (_, _) => btnAddPetClick(btnAddPet);
        btnDelPet = new System.Windows.Forms.Button { Left = 424, Top = 50, Width = 90, Text = "删除", Enabled = false };
        btnDelPet.Click += (_, _) => btnDelPetClick(btnDelPet);
        btnEditPet = new System.Windows.Forms.Button { Left = 424, Top = 78, Width = 90, Text = "修改", Enabled = false };
        btnEditPet.Click += (_, _) => btnEditPetClick(btnEditPet);
        btnSavePet = new System.Windows.Forms.Button { Left = 424, Top = 106, Width = 90, Text = "保存", Enabled = false };
        btnSavePet.Click += (_, _) => btnSavePetClick(btnSavePet);
        ts1.Controls.AddRange(new System.Windows.Forms.Control[] { btnAddPet, btnDelPet, btnEditPet, btnSavePet });

        // 捕捉/等级差/HP 缩放
        sePetCaptureRate = MakeSpin(520, 22, 0, 100);
        chkLevelDifference = MakeCheck(520, 50, "启用等级差");
        seLevelDifference = MakeSpin(700, 50, 0, 1000);
        seHPScale = MakeSpin(520, 78, 0, 1000);
        ts1.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            sePetCaptureRate, chkLevelDifference, seLevelDifference, seHPScale,
        });

        // 显示文件两组
        cbbPetShowFile1 = MakeCombo(520, 106);
        sePetShowStart1 = MakeSpin(700, 106, 0, 100000);
        sePetShowCount1 = MakeSpin(520, 134, 0, 100000);
        sePetShowTime1 = MakeSpin(700, 134, 0, 100000);
        sePetShowOffsetX1 = MakeSpin(520, 162, -10000, 10000);
        sePetShowOffsetY1 = MakeSpin(700, 162, -10000, 10000);
        cbbPetShowFile2 = MakeCombo(520, 190);
        sePetShowStart2 = MakeSpin(700, 190, 0, 100000);
        sePetShowCount2 = MakeSpin(520, 218, 0, 100000);
        sePetShowTime2 = MakeSpin(700, 218, 0, 100000);
        sePetShowOffsetX2 = MakeSpin(520, 246, -10000, 10000);
        sePetShowOffsetY2 = MakeSpin(700, 246, -10000, 10000);
        ts1.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            cbbPetShowFile1, sePetShowStart1, sePetShowCount1, sePetShowTime1, sePetShowOffsetX1, sePetShowOffsetY1,
            cbbPetShowFile2, sePetShowStart2, sePetShowCount2, sePetShowTime2, sePetShowOffsetX2, sePetShowOffsetY2,
        });

        // 属性加成（原文 cbb*Type 均为两选项下拉：0=固定值 1=百分比）
        sePetAddHP = MakeSpin(520, 274, 0, 2000000000);
        cbbPetAddHPType = MakeTypeCombo(700, 274);
        sePetAddDC1 = MakeSpin(520, 302, 0, 2000000000);
        cbbPetAddDCType1 = MakeTypeCombo(700, 302);
        sePetAddDC2 = MakeSpin(520, 330, 0, 2000000000);
        cbbPetAddDCType2 = MakeTypeCombo(700, 330);
        sePetAddMC1 = MakeSpin(520, 358, 0, 2000000000);
        cbbPetAddMCType1 = MakeTypeCombo(700, 358);
        sePetAddMC2 = MakeSpin(520, 386, 0, 2000000000);
        cbbPetAddMCType2 = MakeTypeCombo(700, 386);
        sePetAddSC1 = MakeSpin(520, 414, 0, 2000000000);
        cbbPetAddSCType1 = MakeTypeCombo(700, 414);
        sePetAddSC2 = MakeSpin(520, 442, 0, 2000000000);
        cbbPetAddSCType2 = MakeTypeCombo(700, 442);
        sePetAddAC1 = MakeSpin(520, 470, 0, 2000000000);
        cbbPetAddACType1 = MakeTypeCombo(700, 470);
        sePetAddAC2 = MakeSpin(520, 498, 0, 2000000000);
        cbbPetAddACType2 = MakeTypeCombo(700, 498);
        sePetAddMAC1 = MakeSpin(520, 526, 0, 2000000000);
        cbbPetAddMACType1 = MakeTypeCombo(700, 526);
        sePetAddMAC2 = MakeSpin(520, 554, 0, 2000000000);
        cbbPetAddMACType2 = MakeTypeCombo(700, 554);
        ts1.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            sePetAddHP, cbbPetAddHPType,
            sePetAddDC1, cbbPetAddDCType1, sePetAddDC2, cbbPetAddDCType2,
            sePetAddMC1, cbbPetAddMCType1, sePetAddMC2, cbbPetAddMCType2,
            sePetAddSC1, cbbPetAddSCType1, sePetAddSC2, cbbPetAddSCType2,
            sePetAddAC1, cbbPetAddACType1, sePetAddAC2, cbbPetAddACType2,
            sePetAddMAC1, cbbPetAddMACType1, sePetAddMAC2, cbbPetAddMACType2,
        });

        // ---- 升级经验页（TabSheet1）----
        GridLevelExp = new System.Windows.Forms.DataGridView
        {
            Left = 8,
            Top = 40,
            Width = 700,
            Height = 500,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            ColumnHeadersVisible = false,
        };
        GridLevelExp.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Width = 50 });
        GridLevelExp.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Width = 200 });
        // :189 GridLevelExpSetEditText
        GridLevelExp.CellValueChanged += (_, e) => GridLevelExpSetEditText(GridLevelExp, e.ColumnIndex, e.RowIndex);
        TabSheet1.Controls.Add(GridLevelExp);

        cbbLevelExp = new System.Windows.Forms.ComboBox { Left = 8, Top = 10, Width = 200, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        // :188 cbbLevelExpClick
        // ⚠ 接缝说明（与原文的形式差异，行为等价）：DFM 绑的是 `OnClick`，Delphi 里
        //   `ItemIndex := n` **不会**触发 OnClick（只有用户点选才触发）；WinForms 的
        //   ComboBox 没有"用户点击"事件可绑定，故改绑 `SelectedIndexChanged`。
        //   后果：**程序化**改 `SelectedIndex` 在托管侧也会触发一次处理器（Delphi 不会）。
        //   本窗体自身代码从不程序化设置该项目（DoOpen 用 `Items.Clear()+Add` 重建，
        //   不设 SelectedIndex），故生产路径行为一致；仅测试需注意"设置索引即已触发一次"。
        cbbLevelExp.SelectedIndexChanged += (_, _) => cbbLevelExpClick(cbbLevelExp);
        TabSheet1.Controls.Add(cbbLevelExp);

        sePetHighLevel = MakeSpin(8, 560, 0, 65535);
        sePetHighLevel.ValueChanged += (_, _) => sePetHighLevelChange(sePetHighLevel);
        sePetHighLevelGetExp = MakeSpin(300, 560, 0, 2000000000);
        sePetHighLevelGetExp.ValueChanged += (_, _) => sePetHighLevelGetExpChange(sePetHighLevelGetExp);
        chkPetFixExp = MakeCheck(8, 590, "使用固定经验");
        chkPetFixExp.CheckedChanged += (_, _) => chkPetFixExpClick(chkPetFixExp);
        sePetBaseExp = MakeSpin(300, 590, 0, 2000000000);
        sePetBaseExp.ValueChanged += (_, _) => sePetBaseExpChange(sePetBaseExp);
        sePetAddExp = MakeSpin(560, 590, 0, 2000000000);
        sePetAddExp.ValueChanged += (_, _) => sePetAddExpChange(sePetAddExp);
        btnSaveExp = new System.Windows.Forms.Button { Left = 800, Top = 588, Width = 90, Text = "保存经验", Enabled = false };
        btnSaveExp.Click += (_, _) => btnSaveExpClick(btnSaveExp);
        TabSheet1.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            sePetHighLevel, sePetHighLevelGetExp, chkPetFixExp, sePetBaseExp, sePetAddExp, btnSaveExp,
        });

        // ---- 全局参数页（TabSheet2）----
        int y = 10;
        chkOpenGamePet = MakeCheck(8, y, "开启宠物系统"); y += 26;
        chkEnabledPetAttack = MakeCheck(8, y, "允许宠物攻击"); y += 26;
        chkDisableMonAttackPet = MakeCheck(8, y, "禁止怪物攻击宠物"); y += 26;
        chkDisableAllAttackPet = MakeCheck(8, y, "禁止所有攻击宠物"); y += 26;
        chkEnabledPetPickup = MakeCheck(8, y, "允许宠物捡物"); y += 26;
        chkPetOnlyPickMonsterItem = MakeCheck(8, y, "只捡怪物掉落物"); y += 26;
        chkPetPickupToMaster = MakeCheck(8, y, "直接捡到主人背包"); y += 26;
        chkPetPickupFullToMaster = MakeCheck(8, y, "包满时捡到主人包裹"); y += 26;
        chkPetQuickPickup = MakeCheck(8, y, "快速捡物"); y += 26;
        chkPetRangePickup = MakeCheck(8, y, "范围捡物"); y += 26;
        sePetPickupRange = MakeSpin(300, y, 0, 255); y += 26;
        chkPetNoEntity = MakeCheck(8, y, "宝宝无实体"); y += 26;
        chkPetSleepControlBySlave = MakeCheck(8, y, "休息受宝宝控制"); y += 26;
        chkPetNoShowHPProgress = MakeCheck(8, y, "不显示 HP 进度"); y += 26;
        chkEnablePetUseClientPickItems = MakeCheck(8, y, "使用客户端捡物列表"); y += 26;
        chkGamePetKillMonTrigger = MakeCheck(8, y, "杀怪触发宠物"); y += 26;
        chkPetHPToMaster = MakeCheck(300, 10, "叠加HP给主人");
        chkPetDCToMaster = MakeCheck(300, 36, "叠加攻击给主人");
        chkPetMCToMaster = MakeCheck(300, 62, "叠加魔法给主人");
        chkPetSCToMaster = MakeCheck(300, 88, "叠加道术给主人");
        chkPetACToMaster = MakeCheck(300, 114, "叠加防御给主人");
        chkPetMACToMaster = MakeCheck(300, 140, "叠加魔防给主人");
        sePetAbilToMasterRate = MakeSpin(300, 166, 0, 1000);
        sePetUseItemIntervalTime = MakeSpin(300, 192, 0, int.MaxValue);
        chkCapturePetNeedItem = MakeCheck(300, 218, "捕捉需要物品");
        chkCaptureOKDecDura = MakeCheck(300, 244, "捕捉成功扣持久");
        chkPetShowMasterName = MakeCheck(300, 270, "显示主人名字");
        sePetNameColor = MakeSpin(300, 296, 0, 255);
        edtPetSuffixName = new System.Windows.Forms.TextBox { Left = 300, Top = 322, Width = 200 };
        seGamePetMaxCount = MakeSpin(300, 348, 0, 100000);
        seGamePetNameCount = MakeSpin(300, 374, 0, 100000);
        seGamePetRecallTime = MakeSpin(300, 400, 0, 100000);
        btnSavePetParams = new System.Windows.Forms.Button { Left = 300, Top = 430, Width = 110, Text = "保存参数", Enabled = false };
        btnSavePetParams.Click += (_, _) => btnSavePetParamsClick(btnSavePetParams);

        TabSheet2.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            chkOpenGamePet, chkEnabledPetAttack, chkDisableMonAttackPet, chkDisableAllAttackPet,
            chkEnabledPetPickup, chkPetOnlyPickMonsterItem, chkPetPickupToMaster, chkPetPickupFullToMaster,
            chkPetQuickPickup, chkPetRangePickup, sePetPickupRange, chkPetNoEntity,
            chkPetSleepControlBySlave, chkPetNoShowHPProgress, chkEnablePetUseClientPickItems,
            chkGamePetKillMonTrigger,
            chkPetHPToMaster, chkPetDCToMaster, chkPetMCToMaster, chkPetSCToMaster, chkPetACToMaster, chkPetMACToMaster,
            sePetAbilToMasterRate, sePetUseItemIntervalTime, chkCapturePetNeedItem, chkCaptureOKDecDura,
            chkPetShowMasterName, sePetNameColor, edtPetSuffixName,
            seGamePetMaxCount, seGamePetNameCount, seGamePetRecallTime, btnSavePetParams,
        });

        HookParamHandlers();
    }

    /// <summary>
    /// DFM 事件绑定 1:1（原文 `.dfm` 共 **48** 条 `On*` 绑定，已逐条核对）。
    /// ⚠ 顺序无关：所有处理器内部都以 `boOpened` 早退，构造期（boOpened=false）不会写配置。
    /// </summary>
    /// <remarks>
    /// ⚠ 以下控件在原文 `.dfm` 里**没有**任何 `On*` 绑定（不在那 48 条里），故此处也不绑定：
    ///   `chkLevelDifference` / `seLevelDifference` / `seHPScale` / `sePetCaptureRate` ——
    ///   它们只在 :469-473（改）与 :558-562（增）里被**读取**，改值不置脏、不写 g_Config，
    ///   必须点"增加/修改"才生效。**不要**给它们补处理器（会改变原文行为）。
    ///   同理 `sePetShow*` / `sePetAdd*` 全族与 `cbbPetShowFile1/2` / `cbbPetAdd*Type` 亦无绑定。
    /// </remarks>
    private void HookParamHandlers()
    {
        // 全局参数页（纯布尔）
        chkOpenGamePet.CheckedChanged += (_, _) => chkOpenGamePetClick(chkOpenGamePet);
        chkEnabledPetAttack.CheckedChanged += (_, _) => chkEnabledPetAttackClick(chkEnabledPetAttack);
        chkDisableMonAttackPet.CheckedChanged += (_, _) => chkDisableMonAttackPetClick(chkDisableMonAttackPet);
        chkDisableAllAttackPet.CheckedChanged += (_, _) => chkDisableAllAttackPetClick(chkDisableAllAttackPet);
        chkEnabledPetPickup.CheckedChanged += (_, _) => chkEnabledPetPickupClick(chkEnabledPetPickup);
        chkPetOnlyPickMonsterItem.CheckedChanged += (_, _) => chkPetOnlyPickMonsterItemClick(chkPetOnlyPickMonsterItem);
        chkPetPickupToMaster.CheckedChanged += (_, _) => chkPetPickupToMasterClick(chkPetPickupToMaster);
        chkPetPickupFullToMaster.CheckedChanged += (_, _) => chkPetPickupFullToMasterClick(chkPetPickupFullToMaster);
        chkPetQuickPickup.CheckedChanged += (_, _) => chkPetQuickPickupClick(chkPetQuickPickup);
        chkPetRangePickup.CheckedChanged += (_, _) => chkPetRangePickupClick(chkPetRangePickup);
        chkPetNoEntity.CheckedChanged += (_, _) => chkPetNoEntityClick(chkPetNoEntity);
        chkPetSleepControlBySlave.CheckedChanged += (_, _) => chkPetSleepControlBySlaveClick(chkPetSleepControlBySlave);
        chkPetNoShowHPProgress.CheckedChanged += (_, _) => chkPetNoShowHPProgressClick(chkPetNoShowHPProgress);
        chkEnablePetUseClientPickItems.CheckedChanged += (_, _) => chkEnablePetUseClientPickItemsClick(chkEnablePetUseClientPickItems);
        chkGamePetKillMonTrigger.CheckedChanged += (_, _) => chkGamePetKillMonTriggerClick(chkGamePetKillMonTrigger);
        chkCapturePetNeedItem.CheckedChanged += (_, _) => chkCapturePetNeedItemClick(chkCapturePetNeedItem);
        chkCaptureOKDecDura.CheckedChanged += (_, _) => chkCaptureOKDecDuraClick(chkCaptureOKDecDura);
        chkPetShowMasterName.CheckedChanged += (_, _) => chkPetShowMasterNameClick(chkPetShowMasterName);
        edtPetSuffixName.TextChanged += (_, _) => edtPetSuffixNameChange(edtPetSuffixName);

        // 叠加给主人
        chkPetHPToMaster.CheckedChanged += (_, _) => chkPetHPToMasterClick(chkPetHPToMaster);
        chkPetDCToMaster.CheckedChanged += (_, _) => chkPetDCToMasterClick(chkPetDCToMaster);
        chkPetMCToMaster.CheckedChanged += (_, _) => chkPetMCToMasterClick(chkPetMCToMaster);
        chkPetSCToMaster.CheckedChanged += (_, _) => chkPetSCToMasterClick(chkPetSCToMaster);
        chkPetACToMaster.CheckedChanged += (_, _) => chkPetACToMasterClick(chkPetACToMaster);
        chkPetMACToMaster.CheckedChanged += (_, _) => chkPetMACToMasterClick(chkPetMACToMaster);

        // Spin 数值
        sePetAbilToMasterRate.ValueChanged += (_, _) => sePetAbilToMasterRateChange(sePetAbilToMasterRate);
        sePetPickupRange.ValueChanged += (_, _) => sePetPickupRangeChange(sePetPickupRange);
        sePetUseItemIntervalTime.ValueChanged += (_, _) => sePetUseItemIntervalTimeChange(sePetUseItemIntervalTime);
        sePetNameColor.ValueChanged += (_, _) => sePetNameColorChange(sePetNameColor);
        seGamePetMaxCount.ValueChanged += (_, _) => seGamePetMaxCountChange(seGamePetMaxCount);
        seGamePetNameCount.ValueChanged += (_, _) => seGamePetNameCountChange(seGamePetNameCount);
        seGamePetRecallTime.ValueChanged += (_, _) => seGamePetRecallTimeChange(seGamePetRecallTime);
    }

    /// <summary>暴露给测试：`boModValued`（原文 private 字段，:220）。</summary>
    public bool BoModValued => boModValued;

    /// <summary>暴露给测试：`boOpened`（原文 private 字段，:219）。</summary>
    public bool BoOpened => boOpened;

    /// <summary>
    /// 把 `boModValued` 复位为 false（原文只在构造期与 :1180 两处置 false）。
    /// 仅供测试在 `DoOpen` 之后构造"未置脏"初态；不改变任何生产行为。
    /// </summary>
    public void ResetModValuedForTest() => boModValued = false;

    /// <summary>暴露给测试：原文 private 处理器的直调入口（事件处理器直调 + 决策镜像）。</summary>
    public void RaiseParamHandler(string controlName, bool value)
    {
        switch (controlName)
        {
            case nameof(chkOpenGamePet): chkOpenGamePet.Checked = value; break;
            case nameof(chkEnabledPetAttack): chkEnabledPetAttack.Checked = value; break;
            case nameof(chkDisableMonAttackPet): chkDisableMonAttackPet.Checked = value; break;
            case nameof(chkDisableAllAttackPet): chkDisableAllAttackPet.Checked = value; break;
            case nameof(chkEnabledPetPickup): chkEnabledPetPickup.Checked = value; break;
            case nameof(chkPetOnlyPickMonsterItem): chkPetOnlyPickMonsterItem.Checked = value; break;
            case nameof(chkPetPickupToMaster): chkPetPickupToMaster.Checked = value; break;
            case nameof(chkPetPickupFullToMaster): chkPetPickupFullToMaster.Checked = value; break;
            case nameof(chkPetQuickPickup): chkPetQuickPickup.Checked = value; break;
            case nameof(chkPetRangePickup): chkPetRangePickup.Checked = value; break;
            case nameof(chkPetNoEntity): chkPetNoEntity.Checked = value; break;
            case nameof(chkPetSleepControlBySlave): chkPetSleepControlBySlave.Checked = value; break;
            case nameof(chkPetNoShowHPProgress): chkPetNoShowHPProgress.Checked = value; break;
            case nameof(chkEnablePetUseClientPickItems): chkEnablePetUseClientPickItems.Checked = value; break;
            case nameof(chkGamePetKillMonTrigger): chkGamePetKillMonTrigger.Checked = value; break;
            case nameof(chkCapturePetNeedItem): chkCapturePetNeedItem.Checked = value; break;
            case nameof(chkCaptureOKDecDura): chkCaptureOKDecDura.Checked = value; break;
            case nameof(chkPetShowMasterName): chkPetShowMasterName.Checked = value; break;
            case nameof(chkPetHPToMaster): chkPetHPToMaster.Checked = value; break;
            case nameof(chkPetDCToMaster): chkPetDCToMaster.Checked = value; break;
            case nameof(chkPetMCToMaster): chkPetMCToMaster.Checked = value; break;
            case nameof(chkPetSCToMaster): chkPetSCToMaster.Checked = value; break;
            case nameof(chkPetACToMaster): chkPetACToMaster.Checked = value; break;
            case nameof(chkPetMACToMaster): chkPetMACToMaster.Checked = value; break;
            case nameof(chkPetFixExp): chkPetFixExp.Checked = value; break;
            default: throw new ArgumentException("未知开关: " + controlName, nameof(controlName));
        }
    }

    /// <summary>暴露给测试：数值控件直调（原文 ValueChanged 族）。</summary>
    public void RaiseSpinHandler(string controlName, int value)
    {
        switch (controlName)
        {
            case nameof(sePetAbilToMasterRate): sePetAbilToMasterRate.Value = value; break;
            case nameof(sePetPickupRange): sePetPickupRange.Value = value; break;
            case nameof(sePetUseItemIntervalTime): sePetUseItemIntervalTime.Value = value; break;
            case nameof(sePetNameColor): sePetNameColor.Value = value; break;
            case nameof(seGamePetMaxCount): seGamePetMaxCount.Value = value; break;
            case nameof(seGamePetNameCount): seGamePetNameCount.Value = value; break;
            case nameof(seGamePetRecallTime): seGamePetRecallTime.Value = value; break;
            case nameof(sePetHighLevel): sePetHighLevel.Value = value; break;
            case nameof(sePetHighLevelGetExp): sePetHighLevelGetExp.Value = value; break;
            case nameof(sePetBaseExp): sePetBaseExp.Value = value; break;
            case nameof(sePetAddExp): sePetAddExp.Value = value; break;
            default: throw new ArgumentException("未知数值控件: " + controlName, nameof(controlName));
        }
    }

    /// <summary>暴露给测试：文本变更直调（原文 `edtPetSuffixNameChange`）。</summary>
    public void RaiseSuffixNameChanged(string text) => edtPetSuffixName.Text = text;

    /// <summary>暴露给测试：`btnAddPetClick`（:534）。</summary>
    public void RaiseAddPet() => btnAddPetClick(btnAddPet);
    /// <summary>暴露给测试：`btnDelPetClick`（:614）。</summary>
    public void RaiseDelPet() => btnDelPetClick(btnDelPet);
    /// <summary>暴露给测试：`btnEditPetClick`（:459）。</summary>
    public void RaiseEditPet() => btnEditPetClick(btnEditPet);
    /// <summary>暴露给测试：`btnSavePetClick`（:650）。</summary>
    public void RaiseSavePet() => btnSavePetClick(btnSavePet);
    /// <summary>暴露给测试：`btnSaveExpClick`（:1149）。</summary>
    public void RaiseSaveExp() => btnSaveExpClick(btnSaveExp);
    /// <summary>暴露给测试：`btnSavePetParamsClick`（:830）。</summary>
    public void RaiseSavePetParams() => btnSavePetParamsClick(btnSavePetParams);
    /// <summary>暴露给测试：`cbbLevelExpClick`（:914）。</summary>
    public void RaiseLevelExpClick() => cbbLevelExpClick(cbbLevelExp);
    /// <summary>暴露给测试：`lstMonsterListDblClick`（:656）。</summary>
    public void RaiseMonsterListDblClick() => lstMonsterListDblClick(lstMonsterList);
    /// <summary>暴露给测试：`lstMonsterListKeyDown`（:671），Key 为 Delphi Word（ASCII 码）。</summary>
    public void RaiseMonsterListKeyDown(ushort key, bool ssCtrl) => lstMonsterListKeyDown(lstMonsterList, key, ssCtrl);
    /// <summary>暴露给测试：`lstGamePetsClick`（:392）。</summary>
    public void RaiseGamePetsClick() => lstGamePetsClick(lstGamePets);
    /// <summary>暴露给测试：`GridLevelExpSetEditText`（:1142）。</summary>
    public void RaiseGridSetEditText(int aCol, int aRow) => GridLevelExpSetEditText(GridLevelExp, aCol, aRow);

    /// <summary>构造 SpinEditEx 替身（TSpinEditEx → NumericUpDown，Value 语义一致）。</summary>
    private static System.Windows.Forms.NumericUpDown MakeSpin(int left, int top, int min, int max)
        => new() { Left = left, Top = top, Width = 90, Minimum = min, Maximum = max };

    private static System.Windows.Forms.CheckBox MakeCheck(int left, int top, string text)
        => new() { Left = left, Top = top, Width = 260, Text = text };

    private static System.Windows.Forms.ComboBox MakeCombo(int left, int top)
        => new() { Left = left, Top = top, Width = 160, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };

    /// <summary>`cbbPetAdd*Type`：原文 ItemIndex = 1 表示百分比（`= 1` 判定）；两项固定。</summary>
    private static System.Windows.Forms.ComboBox MakeTypeCombo(int left, int top)
    {
        var cb = MakeCombo(left, top);
        cb.Items.Add("固定值");
        cb.Items.Add("百分比");
        return cb;
    }

    // ========================================================================
    //  :521-532 RefreshGamePetConfigList
    // ========================================================================

    /// <summary>
    /// 原文 `MAXCHANGELEVEL`（M2Share.pas:205 = 1000）。
    /// ⚠ 不直接引用 `AbilRecalc.MAXCHANGELEVEL`：`AbilRecalc` 是**静态类**而非 namespace，
    ///    `AbilRecalc.MAXCHANGELEVEL` 需全限定且在只读区内，故按 1:1 值在此自持一份常量。
    /// </summary>
    public const int MAXCHANGELEVEL = 1000;

    /// <summary>`TFrmGamePets.RefreshGamePetConfigList`（:521-532）。</summary>
    private void RefreshGamePetConfigList()
    {
        lstGamePets.Items.Clear();
        for (int I = 0; I <= GamePetsState.g_GamePetConfigList.Count - 1; I++)
        {
            TGamePetConfig GamePetConfig = GamePetsState.g_GamePetConfigList[I];
            lstGamePets.Items.Add(GamePetConfig.Name);
        }
    }

    /// <summary>测试/宿主入口：`RefreshGamePetConfigList`（原文 private）。</summary>
    public void RefreshGamePetConfigListForTest() => RefreshGamePetConfigList();
}
